using JKViewModels.Franchise;
using JKViewModels.Franchisee;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class FullCustomerViewModel
    {

        public FullCustomerViewModel()
        {
            CustomerViewModel = new CustomerViewModel();
            CurrentAddressViewModel = new AddressViewModel();
            PermanentAddressViewModel = new AddressViewModel();
            BillingAddress = new AddressViewModel();
            ContactInformation = new ContactViewModel();
            ContactInformationAddress = new AddressViewModel();
            ContactInformationPhone = new PhoneViewModel();
            ContactInformationEmail = new EmailViewModel();
            BillingInformation = new ContactViewModel();
            BillingInformationAddress = new AddressViewModel();
            BillingInformationPhone = new PhoneViewModel();
            BillingInformationEmail = new EmailViewModel();
            MainContact = new ContactViewModel();
            MainAddress = new AddressViewModel();
            MainPhone = new PhoneViewModel();
            MainEmail = new EmailViewModel();

            BillingPhone = new Customer.PhoneViewModel();
            BillingEmail= new Customer.EmailViewModel();
            BillingContact = new ContactViewModel();
            BillingAddress = new AddressViewModel();

            ContactInformation = new ContactViewModel();
            ContactInformationAddress = new AddressViewModel();
            ContactInformationPhone = new PhoneViewModel();
            ContactInformationEmail = new EmailViewModel();

            BillingInformation = new ContactViewModel();
            BillingInformationAddress = new AddressViewModel();
            BillingInformationPhone = new PhoneViewModel();
            BillingInformationEmail = new EmailViewModel();
            Contract = new ContractViewModel();
            ContractDetail = new ContractDetailViewModel();
            _ContractDetail = new List<ContractDetailViewModel>();
            //ContractDetailDescription = new List<ContractDetailDescriptionViewModel>();
            //_ContractDetailDescriptionViewModel = new ContractDetailDescriptionViewModel();
            CustomerDetail = new CustomerDetailViewModel();
            MainPhone2 = new PhoneViewModel();
            MainEmail2 = new EmailViewModel();
            BillingContactInformation2 = new Customer.ContactViewModel();
            ContractAddress = new AddressViewModel();


        }
        public CustomerViewModel CustomerViewModel { get; set; }
        public AddressViewModel CurrentAddressViewModel { get; set; }
        public AddressViewModel PermanentAddressViewModel { get; set; }



        public List<FranchiseeViewModel> Franchisees { get; set; }
        public List<FranchiseeViewModel> _custfranchisees { get; set; }
        public TaxViewModel Taxes { get; set; }



        public ContactViewModel MainContact { get; set; }
        public AddressViewModel MainAddress { get; set; }
        public PhoneViewModel MainPhone { get; set; }
        public EmailViewModel MainEmail { get; set; }
        public PhoneViewModel BillingPhone { get; set; }
        public EmailViewModel BillingEmail { get; set; }
        public ContactViewModel BillingContact { get; set; }
        public AddressViewModel BillingAddress { get; set; }
        public ContactViewModel ContactInformation { get; set; }
        public AddressViewModel ContactInformationAddress { get; set; }
        public PhoneViewModel ContactInformationPhone { get; set; }
        public EmailViewModel ContactInformationEmail { get; set; }
        public ContactViewModel BillingInformation { get; set; }
        public AddressViewModel BillingInformationAddress { get; set; }
        public PhoneViewModel BillingInformationPhone { get; set; }
        public EmailViewModel BillingInformationEmail { get; set; }

        public AddressViewModel ContractAddress { get; set; }


        // BillSetting View
        public BillSettingViewModel BillSetting { get; set; }


        // Customer Contract

        public ContractViewModel Contract { get; set; }



        public ContractDetailViewModel ContractDetail { get; set; }

        public IEnumerable<ContractDetailViewModel> _ContractDetail { get; set; }

        public IEnumerable<ContractDetailDescriptionViewModel> ContractDetailDescription { get; set; }

        public ContractDetailDescriptionViewModel _ContractDetailDescriptionViewModel { get; set; }

        public List<OfficeContactViewModel> olist { get; set; }

        public int contractDetailDescriptionId { get; set; }

        public int? ButtonType { get; set; }

        public CustomerDetailViewModel CustomerDetail { get; set; }

        public PhoneViewModel MainPhone2 { get; set; }
        public EmailViewModel MainEmail2 { get; set; }
        public ContactViewModel BillingContactInformation2 { get; set; }

        public string  EBill_Emails { get; set; }
        public List<EmailViewModel> lstEBillEmails { get; set; }
    }


    public class FullCustomerViewModel1
    {

        public FullCustomerViewModel1()
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
            MainEmail2 = new EmailViewModel();
            BillingContactInformation2 = new Customer.ContactViewModel();

            EBill_Emails = "";
            lstEBillEmails = new List<EmailViewModel>();
            MaintenanceTempId = 0;

    }
        public int MaintenanceTempId { get; set; }
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
        public EmailViewModel MainEmail2 { get; set; }
        public ContactViewModel BillingContactInformation2 { get; set; }

        public int ARStatus { get; set; }

        public int InvoiceDate { get; set; }
        public int TermDate { get; set; }

        public List<PendingDashboardDataModel> MessagesData { get; set; } 

        public string EBill_Emails { get; set; }
        public List<EmailViewModel> lstEBillEmails { get; set; }
        public int USERID { get; set; }
    }


    public class CustomerDistributionDetailsModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalDistribution { get; set; }
        public List<CustomerDistributionModel> CustomerDistributionList { get; set; }
    }
    public class CustomerDistributionModel
    {
        public int ID { get; set; }
        public string FranchiseNo { get; set; }
        public string FranchiseName { get; set; }
        public string ServiceType { get; set; }
        public decimal DistributionAmount { get; set; }
    }

    public class CustomerStatementViewModel
    {
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal StartingBalance { get; set; }
        public List<CustomerDetailTransactionViewModel> Transactions { get; set; }

        public RemitToViewModel RemitTo { get; set; }
    }

    public class RemitToViewModel
    {
        public AddressViewModel Address { get; set; }

        public string RegionName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }

    public class CustomerFranchiseeDistribution
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }        
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string BAddress1 { get; set; }
        public string BAddress2 { get; set; }
        public string BCity { get; set; }
        public string BStateName { get; set; }
        public string BPostalCode { get; set; }
        public string EmailAddress { get; set; }

        public string FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }        
        public string FranchiseePhone { get; set; }
        public string FranchiseeEmail { get; set; }
        public string FranchiseeContactName { get; set; }
        public DateTime ? StartDate { get; set; }        
        public decimal Amount { get; set; }
        public decimal InitialCleanAmount { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tues { get; set; }
        public bool Wed { get; set; }
        public bool Thur { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }

        public string FAddress1 { get; set; }
        public string FAddress2 { get; set; }
        public string FCity { get; set; }
        public string FStateName { get; set; }
        public string FPostalCode { get; set; }
    }
    public class BasicInfoCustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public string PhoneNo { get; set; }
    }
    public class NewAccountFormFieldModel
    {
        public int Id { get; set; }
        public int FieldValue { get; set; }
        public string FieldText { get; set; }

    }

}


