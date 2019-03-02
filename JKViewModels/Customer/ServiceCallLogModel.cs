using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ServiceCallLogModel
    {
        public ServiceCallLogModel()
        {
            CustomerViewModel = new CustomerViewModel();
            BillSetting = new BillSettingViewModel();
            CustomerDetail = new CustomerDetailViewModel();
            MainAddress = new AddressViewModel();
            MainContact = new ContactViewModel();
            MainPhone = new PhoneViewModel();
            MainEmail = new EmailViewModel();
            BillingContact = new ContactViewModel();
            BillingAddress = new AddressViewModel();
            BillingPhone = new PhoneViewModel();
            BillingEmail = new EmailViewModel();
            ContactInformation = new ContactViewModel();
            ContactInformationPhone = new PhoneViewModel();
            ContactInformationEmail = new EmailViewModel();
            BillingInformationPhone = new PhoneViewModel();
            BillingInformationEmail = new EmailViewModel();
            BillingInformation = new ContactViewModel();
            BillingInformationAddress = new AddressViewModel();
            MainPhone2 = new PhoneViewModel();
            BillingContactInformation2 = new Customer.ContactViewModel();

            EBill_Emails = "";
            lstEBillEmails = new List<EmailViewModel>();
        }
        public CustomerViewModel CustomerViewModel { get; set; }
        public BillSettingViewModel BillSetting { get; set; }

        public CustomerDetailViewModel CustomerDetail { get; set; }
        public AddressViewModel MainAddress { get; set; }
        public ContactViewModel MainContact { get; set; }
        public PhoneViewModel MainPhone { get; set; }
        public EmailViewModel MainEmail { get; set; }
        public ContactViewModel BillingContact { get; set; }
        public AddressViewModel BillingAddress { get; set; }

        public PhoneViewModel BillingPhone { get; set; }
        public EmailViewModel BillingEmail { get; set; }

        public ContactViewModel ContactInformation { get; set; }
        public PhoneViewModel ContactInformationPhone { get; set; }
        public EmailViewModel ContactInformationEmail { get; set; }
        public PhoneViewModel BillingInformationPhone { get; set; }
        public EmailViewModel BillingInformationEmail { get; set; }
        public ContactViewModel BillingInformation { get; set; }
        public AddressViewModel BillingInformationAddress { get; set; }

        public PhoneViewModel MainPhone2 { get; set; }
        public ContactViewModel BillingContactInformation2 { get; set; }

        public int ARStatus { get; set; }

        public int InvoiceDate { get; set; }
        public int TermDate { get; set; }


        public string EBill_Emails { get; set; }
        public List<EmailViewModel> lstEBillEmails { get; set; }


        public int ServiceCallLogId { get; set; }
        public Nullable<System.DateTime> CallDate { get; set; }
       // public Nullable<System.TimeSpan> CallTime { get; set; }
        public string CallTime { get; set; }
        public string SaveClose { get; set; }
        public string SaveNew { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> Internal { get; set; }
        public Nullable<int> InitiatedBy { get; set; }
        public Nullable<int> ServiceLogTypeListId { get; set; }
        public string ServiceLogTypeListName { get; set; }
        public Nullable<int> StatusResultListId { get; set; }
        public Nullable<int> ServiceLogAreaListId { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        public Nullable<int> FollowUpBy { get; set; }
        public string EmailNotesTo { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public bool boolInternal { get; set; }
        public string  CreatedByName { get; set; }
        public bool IsCallBack { get; set; }
        public Nullable<int> StatusListId { get; set; }
        //public virtual StatusResultList StatusResultList { get; set; }
                
        public string CallLogInitiatedByTypeListName { get; set; }
        public string ServiceCallLogTypeListName { get; set; }
        public string ServiceCallLogAreaListName { get; set; }
        public string FollowUpByName { get; set; }
        public string StatusListName { get; set; }


    }

    public class ServiceCallGrid
    {
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        //public Nullable<int> FollowUpBy { get; set; }
        //public string EmailNotesTo { get; set; }
        //public Nullable<int> ReferenceId { get; set; }
        public string Comments { get; set; }
        //public Nullable<int> RegionId { get; set; }
        //public Nullable<int> CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public Nullable<int> ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string CustomerNo { get; set; }
        public string Status { get; set; }
    }
    public class ValidationItemDataModel
    {
        public int ValidationId { get; set; }
        public string ValidationNote { get; set; }
        public string StartDatetime { get; set; }
        public string EndDatetime { get; set; }

    }

}
