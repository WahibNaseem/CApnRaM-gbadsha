using System;


namespace JKApi.Service.ServiceContract.Outlook
{
    public interface IOutlookService
    {
        #region Streaming Notifications

        bool Subscribe(int userId);

        #endregion

        #region Appointments

        OutlookSyncResult SyncAppointments();
        OutlookSyncResult SyncAppointments(int userId);
        OutlookSyncResult SyncAppointments(DateTime startDate, DateTime endDate);
        OutlookSyncResult SyncAppointments(int userId, DateTime startDate, DateTime endDate);

        #endregion

        #region Contacts

        OutlookSyncResult SyncContacts();
        OutlookSyncResult SyncContacts(int userId);
        OutlookSyncResult SyncContacts(DateTime startDate, DateTime endDate);
        OutlookSyncResult SyncContacts(int userId, DateTime startDate, DateTime endDate);

        #endregion

    }

    public class OutlookSyncResult
    {
        public bool Connected { get; set; }

        public bool Success { get; set; }
        public Exception Exception { get; set; }

        public int NumOutlookRecordsCreated { get; set; }
        public int NumOutlookRecordsUpdated { get; set; }
        public int NumFMSRecordsCreated { get; set; }
        public int NumFMSRecordsUpdated { get; set; }
    }
}
