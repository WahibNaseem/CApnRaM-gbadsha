using System;
using JKViewModels.Common;

namespace JKViewModels.Customer
{
    public class CustomerModel : BaseEntityModel
    {
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        public int SoldById { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int ContractTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public string ContractType { get; set; }
        public string AccountType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ContractDescription { get; set; }
        public int ContractTermMonth { get; set; }
        public decimal Amount { get; set; }
        public decimal SquareFootage { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public AddressModel Address { get; set; }
        public bool IsActive => IsEnable;
    }

    public class PendingCustomerModel : BaseEntityModel
    {
        public string RegionName { get; set; }
        public int RegionId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountTypeListName { get; set; }
        public decimal? Amount { get; set; }
        public string StatusName { get; set; }
        public AddressModel Address { get; set; }
    }

    public class CustomerLeadModel : BaseEntityModel
    {
        public int AccountCustomerDetailId { get; set; }
        public int AccountId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyFaxNumber { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NumberOfLocations { get; set; }
        public decimal BudgetAmount { get; set; }
        public int AccountTypeListId { get; set; }
        public DateTime? Callback { get; set; }
        public bool AMOrPM { get; set; }
        public string Ext { get; set; }
        public string Title { get; set; }
        public string SicCode { get; set; }
        public string SalesVolume { get; set; }
        public string LineofBusiness { get; set; }
        public string SquareFoot { get; set; }
        public DateTime? StartDate { get; set; }
        public int Purpose { get; set; }
        public int RegionId { get; set; }
        public decimal AttemptCount { get; set; }
        public string LastAttempt { get; set; }
        public string ContactByAmOrPm { get; set; }
        public string ContactTime { get; set; }
        public string ContextPrice { get; set; }
        public string LastContact { get; set; }
        public string Status { get; set; }
        public string CallbackBy { get; set; }
        public string CallbackDate { get; set; }
        public string LastMailDate { get; set; }
        public string ContractTerm { get; set; }
        public string SoldDate { get; set; }
        public string AccountType { get; set; }
        public int CallResultId { get; set; }
        public string SpokeWith { get; set; }
        public int NoteTypeId { get; set; }
        public int SalePossibilityTypeId { get; set; }
        public DateTime? ContractExpire { get; set; }
        public int TerritoryId { get; set; }
        public string SicDescription { get; set; }
        public int AssigneeId { get; set; }
        public int StageStatus { get; set; }
        public int Stage { get; set; }
        public int ProviderSource { get; set; }
        public int ProviderType { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public AddressModel Address { get; set; }
    }

    public class CustomerSimpleViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string AccountType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CommissionCustomerDetailViewModel
    {
        public int CustomerId { get; set; }
        public int RegionId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public int ContractId { get; set; }       
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ContractTypeListId { get; set; }
        public string ContractTypeListName { get; set; }

        public decimal? BonusAmount { get; set; }
        public string BonusExplanation { get; set; }
        public string BonusDescription { get; set; }

    }
}
