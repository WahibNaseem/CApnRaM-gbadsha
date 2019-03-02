using JK.Repository.Uow;
using JKApi.Data.DAL;
using JKApi.Service.Service;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Outlook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;

namespace JKApi.Service.Outlook
{
    public class OutlookService : BaseService, IOutlookService
    {
        private readonly ICRM_Service _crmService;

        private bool TriedConnection = false;
        private ExchangeService _exchangeService;
        
        private static readonly Guid FMS_GUID_PROPSETID = new Guid("{e6913b97-32fe-46d2-a121-7d922f9b65a7}");
        private static readonly ExtendedPropertyDefinition FMS_GUID_PROPDEF = 
            new ExtendedPropertyDefinition(FMS_GUID_PROPSETID, "JaniKing GUID", MapiPropertyType.String);

        private static Dictionary<StreamingSubscription, int> StreamingSubscriptions;
        private static Dictionary<int, StreamingSubscriptionConnection> StreamingConnections;
        
        #region ConstructorCalls

        static OutlookService()
        {
            StreamingSubscriptions = new Dictionary<StreamingSubscription, int>();
            StreamingConnections = new Dictionary<int, StreamingSubscriptionConnection>();
        }

        public OutlookService(IJKEfUow uow, ICRM_Service crmService)
        {
            Uow = uow;
            _crmService = crmService;
        }

        #endregion

        #region Authentication

        private ExchangeService GetExchangeService()
        {
            return GetExchangeService(this.LoginUserId);
        }

        private ExchangeService GetExchangeService(int userId)
        {
            if (_exchangeService == null && !TriedConnection)
            {
                var userInfo = Uow.AuthUserLogin.GetById(userId);
                string username = userInfo?.OutlookUsername;
                string password = userInfo?.OutlookPassword;

                return GetExchangeService(username, password);
            }

            return _exchangeService;
        }

        private ExchangeService GetExchangeService(string username, string password)
        {
            if (_exchangeService == null && !TriedConnection)
            {
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    _exchangeService = ConnectToOffice365(username, password);

                TriedConnection = true;
            }

            return _exchangeService;
        }

        private ExchangeService ConnectToOffice365(string username, string password)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);

            service.Credentials = new WebCredentials(username, password);
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            //service.AutodiscoverUrl(username, RedirectionUrlValidationCallback);

            if (!TestConnection(service)) // connection doesn't work, so return null exchangeservice
                service = null;

            return service;
        }

        private bool TestConnection(ExchangeService exchangeService = null)
        {
            if (exchangeService == null)
                exchangeService = GetExchangeService();

            try
            {
                var inbox = Folder.Bind(exchangeService, WellKnownFolderName.Inbox);
            }
            catch (ServiceRequestException ex)
            {
                Console.WriteLine("Error connecting to Exchange: " + ex.Message);
                return false;
            }

            return true;
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        #endregion

        #region Streaming Notifications

        private static bool CloseStreamingConnectionIfExists(int userId)
        {
            StreamingSubscriptionConnection existingConnection = null;
            if (StreamingConnections.TryGetValue(userId, out existingConnection))
                return CloseStreamingConnection(existingConnection, userId);
            return false;
        }

        private static bool CloseStreamingConnection(StreamingSubscriptionConnection connection, int? userId = null)
        {
            foreach (var subscription in connection.CurrentSubscriptions)
            {
                subscription.Unsubscribe();
                StreamingSubscriptions.Remove(subscription);
            }
            if (userId != null)
                StreamingConnections.Remove(userId.Value);
            connection.Close();

            return true;
        }

        public bool Subscribe(int userId)
        {
            try
            {
                CloseStreamingConnectionIfExists(userId);

                var exchangeService = GetExchangeService(userId);
                if (exchangeService == null)
                    return false;

                var subscription = exchangeService.SubscribeToStreamingNotificationsOnAllFolders(EventType.NewMail, EventType.Copied, EventType.Created, EventType.Deleted, EventType.Modified);

                StreamingSubscriptionConnection connection = new StreamingSubscriptionConnection(exchangeService, 30);
                connection.AddSubscription(subscription);
                connection.OnNotificationEvent += OnOutlookStreamingEvent;
                connection.OnDisconnect += OnOutlookStreamingDisconnect;
                connection.Open();

                if (connection.IsOpen)
                {
                    StreamingSubscriptions[subscription] = userId;
                    StreamingConnections[userId] = connection;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private void OnOutlookStreamingDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            StreamingSubscriptionConnection connection = (StreamingSubscriptionConnection)sender;

            bool doReopen = connection.CurrentSubscriptions.Count() > 0;

            if (doReopen)
                connection.Open();
        }

        private void OnOutlookStreamingEvent(object sender, NotificationEventArgs args)
        {
            bool callSyncContents = false;

            StreamingSubscription subscription = args.Subscription;

            int userId = -1;
            if (!StreamingSubscriptions.TryGetValue(subscription, out userId))
                return;

            foreach (NotificationEvent notification in args.Events)
            {
                if (notification is ItemEvent)
                {
                    callSyncContents = true;
                    break;
                }
            }

            if (callSyncContents == true)
            {
                SyncAppointments(userId);
                SyncContacts(userId);
            }
        }

        static void OnOutlookStreamingError(object sender, SubscriptionErrorEventArgs args)
        {
            Exception e = args.Exception;
            Console.WriteLine("\n-------------Error ---" + e.Message + "-------------");
        }

        #endregion

        #region Appointments

        public OutlookSyncResult SyncAppointments()
        {
            return SyncAppointments(this.LoginUserId);
        }

        public OutlookSyncResult SyncAppointments(int userId)
        {
            DateTime now = DateTime.Today;
            return SyncAppointments(userId, now.AddMonths(-1), now.AddMonths(6));
        }

        public OutlookSyncResult SyncAppointments(DateTime startDate, DateTime endDate)
        {
            return SyncAppointments(this.LoginUserId, startDate, endDate);
        }

        public OutlookSyncResult SyncAppointments(int userId, DateTime startDate, DateTime endDate)
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var localTimeZone = TimeZoneInfo.Local;

            OutlookSyncResult syncResult = new OutlookSyncResult();
            syncResult.Success = false;

            var exchangeService = GetExchangeService(userId);
            if (exchangeService == null)
            {
                syncResult.Connected = false;
                return syncResult;
            }

            syncResult.Connected = true;

            ItemView view = new ItemView(25);

            SearchFilter.SearchFilterCollection filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
            filter.Add(new SearchFilter.IsGreaterThanOrEqualTo(AppointmentSchema.Start, startDate));
            filter.Add(new SearchFilter.IsLessThanOrEqualTo(AppointmentSchema.Start, endDate));

            try
            {
                var appointments = new List<Item>();
                var appointmentsToRemove = new List<Item>();

                FindItemsResults<Item> results = null;
                do
                {
                    results = exchangeService.FindItems(WellKnownFolderName.Calendar, filter, view);
                    if (results.Items.Count > 0)
                    {
                        exchangeService.LoadPropertiesForItems(results, new PropertySet(BasePropertySet.FirstClassProperties, FMS_GUID_PROPDEF, ItemSchema.TextBody));
                        appointments.AddRange(results.Items);
                        view.Offset = results.NextPageOffset ?? 0;
                    }
                }
                while (results.MoreAvailable);
                
                var schedules = _crmService.GetAll_CRM_Schedule()
                    .Where(o => (o.AuthUserLoginId ?? o.CreatedBy) == userId && startDate <= o.StartDate && o.StartDate <= endDate
                    && o.CRM_ScheduleTypeId == 1).ToList(); // also filter by CRM_ScheduleTypeId=1 (outlook)

                DateTime syncDate = DateTime.Now;
                var customerLeads = _crmService.GetAll_CRM_AccountCustomerDetail();
                var franchiseLeads = _crmService.GetAll_CRM_AccountFranchiseDetail();

                foreach (Item item in appointments)
                {
                    bool updateOutlookAppt = false;
                    bool updateFMSSchedule = false;
                    bool updateFMSScheduleSync = false;

                    Appointment appointment = item as Appointment;

                    TimeZoneInfo apptTimeZone = timeZones.Where(o => o.DisplayName == appointment.TimeZone).FirstOrDefault();
                    DateTime relevantApptTime = TimeZoneInfo.ConvertTime(appointment.Start, apptTimeZone);

                    /*if (appointment.DisplayTo == null)
                    {
                        appointmentsToRemove.Add(appointment);
                        continue;
                    }*/

                    List<string> recipients = appointment.DisplayTo?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToList();
                    CRM_AccountCustomerDetail matchedCustomerLead = null;
                    CRM_AccountFranchiseDetail matchedFranchiseLead = null;

                    if (recipients != null)
                    {    
                        foreach (string recipient in recipients)
                        {
                            matchedCustomerLead = customerLeads.FirstOrDefault(o => o.CompanyEmailAddress == recipient);
                            if (matchedCustomerLead != null)
                                break;
                            matchedFranchiseLead = franchiseLeads.FirstOrDefault(o => o.EmailAddress == recipient);
                            if (matchedFranchiseLead != null)
                                break;
                        }   
                    }

                    /*if (matchedCustomerLead == null && matchedFranchiseLead == null) // no lead found, skip this appointment
                    {
                        appointmentsToRemove.Add(appointment);
                        continue;
                    }*/

                    Guid fmsGuid = Guid.Empty;
                    var guidProperty = appointment.ExtendedProperties.FirstOrDefault(o => o.PropertyDefinition == FMS_GUID_PROPDEF);

                    if (guidProperty == null)
                    {
                        fmsGuid = Guid.NewGuid();
                        updateOutlookAppt = true;
                    }
                    else
                        fmsGuid = new Guid((string)guidProperty.Value);

                    CRM_Schedule schedule = schedules.Where(o => o.OutlookAppointmentGuid == fmsGuid).FirstOrDefault();

                    if (schedule == null)
                    {
                        schedule = new CRM_Schedule();
                        schedule.IsFromOutlook = true;
                        schedule.OutlookAppointmentGuid = fmsGuid;
                        schedule.AuthUserLoginId = userId;
                        schedule.CreatedBy = userId;

                        updateFMSSchedule = true;

                        syncResult.NumFMSRecordsCreated++;
                    }
                    else if (schedule.OutlookSyncDate == null || schedule.OutlookSyncDate < appointment.LastModifiedTime)
                    { 
                        schedule.ModifiedBy = userId;

                        updateFMSSchedule = true;

                        syncResult.NumFMSRecordsUpdated++;
                    }
                    else if (schedule.ModifiedDate > schedule.OutlookSyncDate)
                    {
                        updateFMSScheduleSync = true;
                        updateOutlookAppt = true;

                        syncResult.NumFMSRecordsUpdated++;
                    }

                    /*if (schedule.CRM_AccountCustomerDetailId != matchedCustomerLead?.CRM_AccountCustomerDetailId)
                        updateFMSSchedule = true;
                    if (schedule.CRM_AccountFranchiseDetailId != matchedFranchiseLead?.CRM_AccountFranchiseDetailId)
                        updateFMSSchedule = true;*/

                    if (updateFMSSchedule)
                    {
                        schedule.ClassId = matchedCustomerLead?.CRM_AccountCustomerDetailId;
                        schedule.CRM_AccountFranchiseDetailId = matchedFranchiseLead?.CRM_AccountFranchiseDetailId;

                       
                        schedule.Title = appointment.Subject;
                        schedule.Description = appointment.TextBody;
                        schedule.Location = appointment.Location;
                        schedule.StartDate = relevantApptTime;
                        schedule.EndDate = relevantApptTime + appointment.Duration;
                        schedule.IsAllDay = appointment.IsAllDayEvent;
                        schedule.CRM_ScheduleTypeId = 1; // outlook
                    }

                    if (updateFMSSchedule || updateFMSScheduleSync)
                    {
                        schedule.OutlookSyncDate = DateTime.Now + new TimeSpan(0, 0, 15); // set sync date in future to avoid update flip-flopping

                        schedule = _crmService.SaveCRM_Schedule(schedule);
                    }

                    if (updateOutlookAppt)
                    {
                        appointment.Subject = schedule.Title;
                        appointment.Body = schedule.Description;
                        appointment.Location = schedule.Location;
                        appointment.Start = (DateTime)schedule.StartDate;
                        appointment.End = (DateTime)schedule.EndDate;
                        appointment.IsAllDayEvent = schedule.IsAllDay ?? false;

                        appointment.SetExtendedProperty(FMS_GUID_PROPDEF, fmsGuid.ToString());

                        appointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendOnlyToChanged);

                        Console.WriteLine("Updated appointment with JaniKing GUID extended property.");

                        syncResult.NumOutlookRecordsUpdated++;
                    }

                    // we have a matched FMS schedule and outlook appointment, so remove both from consideration
                    if (schedule != null)
                        schedules.Remove(schedule);
                    if (appointment != null)
                        appointmentsToRemove.Add(appointment);
                }

                appointmentsToRemove.ForEach(o => appointments.Remove(o));
                appointmentsToRemove.Clear();

                foreach (CRM_Schedule schedule in schedules)
                {
                    // filter invalid schedules
                    if (schedule.StartDate == null)
                        continue;
                    //if (schedule.CRM_AccountCustomerDetail == null && schedule.CRM_AccountFranchiseDetail == null)
                    //    continue;

                    string leadEmail = null;//schedule.CRM_AccountCustomerDetail?.CompanyEmailAddress ?? schedule.CRM_AccountFranchiseDetail?.EmailAddress;

                    /*if (leadEmail == null)
                        continue;*/

                    // check if email is a valid smtp address
                    if (leadEmail != null)
                        if (!Regex.IsMatch(leadEmail, @"^[A-Za-z0-9._%+-]+\@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$"))
                            continue;

                    Guid fmsGuid = schedule.OutlookAppointmentGuid ?? Guid.Empty;

                    Appointment appointment = null;
                    if (fmsGuid != Guid.Empty)
                    {
                        // Find the appointment with the matching FMS GUID.
                        // This should always fail since already checked earlier, but it's a sanity check.
                        foreach (var appt in appointments)
                        {
                            var guidProperty = appt.ExtendedProperties.FirstOrDefault(o => o.PropertyDefinition == FMS_GUID_PROPDEF);
                            if (guidProperty != null && new Guid(guidProperty.Value.ToString()) == fmsGuid)
                            {
                                appointment = appt as Appointment;
                                break;
                            }
                        }
                    }
                    else
                        fmsGuid = Guid.NewGuid();

                    if (appointment == null)
                    {
                        appointment = new Appointment(exchangeService);

                        syncResult.NumOutlookRecordsCreated++;
                    }
                    else
                        syncResult.NumOutlookRecordsUpdated++;

                    appointment.Subject = schedule.Title ?? "JaniKing Auto-generated Meeting";
                    appointment.Body = schedule.Description;
                    appointment.Location = schedule.Location;
                    appointment.Start = (DateTime)schedule.StartDate;
                    appointment.End = (DateTime)schedule.EndDate;
                    appointment.IsAllDayEvent = schedule.IsAllDay ?? false;
                    
                    if (leadEmail != null)
                        appointment.RequiredAttendees.Add(leadEmail);

                    appointment.SetExtendedProperty(FMS_GUID_PROPDEF, fmsGuid.ToString());

                    appointment.Save(SendInvitationsMode.SendToNone);

                    //if (schedule.OutlookAppointmentGuid == null || schedule.OutlookAppointmentGuid == Guid.Empty)
                    if (schedule.OutlookAppointmentGuid != fmsGuid)
                    {
                        schedule.ModifiedBy = userId;

                        schedule.IsFromOutlook = false;
                        schedule.OutlookAppointmentGuid = fmsGuid;
                        schedule.OutlookSyncDate = DateTime.Now + new TimeSpan(0, 0, 15); // set sync date in future to avoid update flip-flopping
                        schedule.CRM_ScheduleTypeId = 1; // outlook

                        _crmService.SaveCRM_Schedule(schedule);

                        syncResult.NumFMSRecordsUpdated++;
                    }

                    Console.WriteLine("Created appointment with JaniKing GUID extended property.");
                }

                syncResult.Success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                syncResult.Exception = ex;
            }

            return syncResult;
        }

        #endregion

        #region Contacts

        public OutlookSyncResult SyncContacts()
        {
            return SyncContacts(this.LoginUserId);
        }

        public OutlookSyncResult SyncContacts(int userId)
        {
            DateTime startDate = new DateTime(1900, 1, 1);
            DateTime endDate = new DateTime(9999, 12, 31);

            var contacts = _crmService.GetAll_CRM_Contact()
                .Where(o => o.CreatedBy == userId /* && o.IsFromOutlook */).ToList();

            // get most recent created contact from outlook - one week as startdate
            if (contacts.Count > 0)
                startDate = contacts.Max(o => o.CreatedDate).Value.AddDays(-7);

            return SyncContacts(userId, startDate, endDate);
        }

        public OutlookSyncResult SyncContacts(DateTime startDate, DateTime endDate)
        {
            return SyncContacts(this.LoginUserId, startDate, endDate);
        }

        public OutlookSyncResult SyncContacts(int userId, DateTime startDate, DateTime endDate)
        {
            OutlookSyncResult syncResult = new OutlookSyncResult();
            syncResult.Success = false;

            var exchangeService = GetExchangeService(userId);
            if (exchangeService == null)
            {
                syncResult.Connected = false;
                return syncResult;
            }

            syncResult.Connected = true;

            ItemView view = new ItemView(25);

            try
            {
                var outlookContacts = new List<Item>();
                var outlookContactsToRemove = new List<Item>();

                FindItemsResults<Item> results = null;
                do
                {
                    results = exchangeService.FindItems(WellKnownFolderName.Contacts, view);
                    if (results.Items.Count > 0)
                    {
                        exchangeService.LoadPropertiesForItems(results, new PropertySet(BasePropertySet.FirstClassProperties, FMS_GUID_PROPDEF));
                        outlookContacts.AddRange(results.Items);
                        view.Offset = results.NextPageOffset ?? 0;
                    }
                }
                while (results.MoreAvailable);

                var fmsContacts = _crmService.GetAll_CRM_Contact()
                    .Where(o => o.CreatedBy == userId && startDate <= o.CreatedDate && o.CreatedDate <= endDate).ToList();

                foreach (Item item in outlookContacts)
                {
                    bool updateOutlookRecord = false;
                    bool updateFMSRecord = false;
                    bool updateFMSRecordSync = false;

                    var outlookContact = item as Microsoft.Exchange.WebServices.Data.Contact;

                    List<string> emailAddresses = new List<string> { outlookContact.EmailAddresses[EmailAddressKey.EmailAddress1].Address };

                    var customerLeads = _crmService.GetAll_CRM_AccountCustomerDetail();
                    var franchiseLeads = _crmService.GetAll_CRM_AccountFranchiseDetail();

                    CRM_AccountCustomerDetail matchedCustomerLead = null;
                    CRM_AccountFranchiseDetail matchedFranchiseLead = null;

                    if (emailAddresses != null)
                    {
                        foreach (string recipient in emailAddresses)
                        {
                            matchedCustomerLead = customerLeads.FirstOrDefault(o => o.CompanyEmailAddress == recipient);
                            if (matchedCustomerLead != null)
                                break;
                            matchedFranchiseLead = franchiseLeads.FirstOrDefault(o => o.EmailAddress == recipient);
                            if (matchedFranchiseLead != null)
                                break;
                        }
                    }

                    Guid fmsGuid = Guid.Empty;
                    var guidProperty = outlookContact.ExtendedProperties.FirstOrDefault(o => o.PropertyDefinition == FMS_GUID_PROPDEF);

                    if (guidProperty == null)
                    {
                        fmsGuid = Guid.NewGuid();
                        updateOutlookRecord = true;
                    }
                    else
                        fmsGuid = new Guid((string)guidProperty.Value);

                    var fmsContact = fmsContacts.Where(o => o.OutlookGuid == fmsGuid).FirstOrDefault();

                    if (fmsContact == null)
                    {
                        fmsContact = new CRM_Contact();
                        fmsContact.IsFromOutlook = true;
                        fmsContact.OutlookGuid = fmsGuid;
                        fmsContact.CreatedBy = userId;
                        fmsContact.CreatedDate = DateTime.Now;
                        updateFMSRecord = true;

                        syncResult.NumFMSRecordsCreated++;
                    }
                    else if (fmsContact.OutlookSyncDate == null || fmsContact.OutlookSyncDate < outlookContact.LastModifiedTime)
                    {
                        fmsContact.ModifiedBy = userId;
                        fmsContact.ModifiedDate = DateTime.Now;

                        updateFMSRecord = true;

                        syncResult.NumFMSRecordsUpdated++;
                    }
                    else if (fmsContact.ModifiedDate > fmsContact.OutlookSyncDate)
                    {
                        updateFMSRecordSync = true;
                        updateOutlookRecord = true;

                        syncResult.NumFMSRecordsUpdated++;
                    }

                    if (updateFMSRecord)
                    {
                        string strRes;
                        EmailAddress emailRes = null;

                        fmsContact.CRM_AccountCustomerDetailId = matchedCustomerLead?.CRM_AccountCustomerDetailId;
                        fmsContact.CRM_AccountFranchiseDetailId = matchedFranchiseLead?.CRM_AccountFranchiseDetailId;

                        fmsContact.ContactName = outlookContact.DisplayName;

                        if (outlookContact.PhoneNumbers.TryGetValue(PhoneNumberKey.PrimaryPhone, out strRes))
                            fmsContact.ContactPhone = strRes;

                        if (outlookContact.EmailAddresses.TryGetValue(EmailAddressKey.EmailAddress1, out emailRes))
                            fmsContact.ContactEmail = emailRes.Address;
                    }

                    if (updateFMSRecord || updateFMSRecordSync)
                    {
                        fmsContact.OutlookSyncDate = DateTime.Now + new TimeSpan(0, 0, 15); // set sync date in future to avoid update flip-flopping

                        fmsContact = _crmService.SaveCRM_Contact(fmsContact);
                    }

                    if (updateOutlookRecord)
                    {
                        outlookContact.DisplayName = fmsContact.ContactName;
                        outlookContact.PhoneNumbers[PhoneNumberKey.PrimaryPhone] = fmsContact.ContactPhone;
                        outlookContact.EmailAddresses[EmailAddressKey.EmailAddress1] = fmsContact.ContactEmail;

                        outlookContact.SetExtendedProperty(FMS_GUID_PROPDEF, fmsGuid.ToString());

                        outlookContact.Update(ConflictResolutionMode.AlwaysOverwrite);

                        Console.WriteLine("Updated contact with JaniKing GUID extended property.");

                        syncResult.NumOutlookRecordsUpdated++;
                    }

                    // we have a matched FMS record and outlook record, so remove both from consideration
                    if (fmsContact != null)
                        fmsContacts.Remove(fmsContact);
                    if (outlookContact != null)
                        outlookContactsToRemove.Add(outlookContact);
                }

                outlookContactsToRemove.ForEach(o => outlookContacts.Remove(o));
                outlookContactsToRemove.Clear();

                foreach (CRM_Contact fmsContact in fmsContacts)
                {
                    Guid fmsGuid = fmsContact.OutlookGuid ?? Guid.Empty;

                    Microsoft.Exchange.WebServices.Data.Contact outlookContact = null;

                    if (fmsGuid != Guid.Empty)
                    {
                        // Find the outlook record with the matching FMS GUID.
                        // This should always fail since already checked earlier, but it's a sanity check.
                        foreach (var obj in outlookContacts)
                        {
                            var guidProperty = obj.ExtendedProperties.FirstOrDefault(o => o.PropertyDefinition == FMS_GUID_PROPDEF);
                            if (guidProperty != null && new Guid(guidProperty.Value.ToString()) == fmsGuid)
                            {
                                outlookContact = obj as Microsoft.Exchange.WebServices.Data.Contact;
                                break;
                            }
                        }
                    }
                    else
                        fmsGuid = Guid.NewGuid();

                    if (outlookContact == null)
                    {
                        outlookContact = new Microsoft.Exchange.WebServices.Data.Contact(exchangeService);

                        syncResult.NumOutlookRecordsCreated++;
                    }
                    else
                        syncResult.NumOutlookRecordsUpdated++;

                    outlookContact.DisplayName = fmsContact.ContactName;
                    outlookContact.PhoneNumbers[PhoneNumberKey.PrimaryPhone] = fmsContact.ContactPhone;
                    outlookContact.EmailAddresses[EmailAddressKey.EmailAddress1] = fmsContact.ContactEmail;

                    outlookContact.SetExtendedProperty(FMS_GUID_PROPDEF, fmsGuid.ToString());

                    outlookContact.Save();

                    if (fmsContact.OutlookGuid != fmsGuid)
                    {
                        fmsContact.ModifiedBy = userId;

                        fmsContact.IsFromOutlook = false;
                        fmsContact.OutlookGuid = fmsGuid;
                        fmsContact.OutlookSyncDate = DateTime.Now + new TimeSpan(0, 0, 15); // set sync date in future to avoid update flip-flopping

                        _crmService.SaveCRM_Contact(fmsContact);

                        syncResult.NumFMSRecordsUpdated++;
                    }

                    Console.WriteLine("Created contact with JaniKing GUID extended property.");
                }

                syncResult.Success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                syncResult.Exception = ex;
            }

            return syncResult;
        }
        #endregion
    }
}
