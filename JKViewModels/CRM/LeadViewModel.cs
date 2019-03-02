namespace JKViewModels.CRM
{
    using Customer;
    using Franchise;
    using Franchisee;

    public class LeadViewModel : CRMBaseViewModel
    {
        public int LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Company { get; set; }
        public int NumberofLocations { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Website { get; set; }
        public decimal BudgetAmount { get; set; }
        public System.DateTime Callback { get; set; }
        public bool IsAM { get; set; }
        public string Notes { get; set; }
        public LeadAccountType AccountType { get; set; }
        public LeadSource LeadSource { get; set; }
        public LeadStatusType LeadStatus { get; set; }
        public Territory Territory { get; set; }
        public LeadSourceProvider LeadProvider { get; set; }
        public int FranchiseId { get; set; }
        public int AssigneeId { get; set; }
        public int ReporterId { get; set; }
        public bool IsQualified { get; set; }
        public bool IsAgreewithQuote { get; set; }
        public CustomerViewModel Customer { get; set; }
        public FranchiseeViewModel Franchise { get; set; }
    }
}
