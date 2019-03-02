namespace JKViewModels.CRM
{
    using System;


    public class CRMAccountCustomerDetailViewModel
    {
        public int CRM_AccountCustomerDetailId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddressLine1 { get; set; }
        public string CompanyAddressLine2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCounty { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZipCode { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyFaxNumber { get; set; }
        public string CompanyWebSite { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NumberOfLocations { get; set; }
        public int BudgetAmount { get; set; }
        public int? AccountTypeListId { get; set; }
        public int CRM_AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; } 
        public string Title { get; set; }
        public string SalesVolume { get; set; }
        public string LineofBusiness { get; set; }
        public string SqFt { get; set; }
        public StageType Stage { get; set; }
        public StageStatusType StageStatus { get; set; }
        public AccountSourceProvider ProviderSource { get; set; }
        public AccountProviderType ProviderType { get; set; }
        public CallResultType CallResultType { get; set; }
        public int? CRM_NoteTypeId { get; set; }
        public int? CRM_SalePossibilityTypeId { get; set; }
        public DateTime? ContractExpire { get; set; }
        public  string StageStatusName { get; set; }
        public bool AMorPM { get; set; }
        public string ContactPerson { get; set; }
        public bool? InterestedInPerposal { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; } 
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public int? CRM_CallResultId { get; set; }
        public string SpokeWith { get; set; }
        public DateTime? Callback { get; set; }
        public int Purpose { get; set; }
        public string GeneralNote { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
