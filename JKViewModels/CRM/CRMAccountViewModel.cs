namespace JKViewModels.CRM
{
    public class CRMAccountViewModel : CRMBaseViewModel
    {
        public int CRM_AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string ContactName { get; set; }        
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int CustomerId { get; set; }
        public int FranchiseId { get; set; }
        public int AssigneeId { get; set; }
        public int ReporterId { get; set; }
        public int AccountType { get; set; }
        public int Stage { get; set; }
        public int StageStatus { get; set; }
        public int ProviderSource { get; set; }
        public int ProviderType { get; set; }
        public virtual string StageStatusName { get; set; }
        public virtual string ProviderTypeName { get; set; }
    }
}
