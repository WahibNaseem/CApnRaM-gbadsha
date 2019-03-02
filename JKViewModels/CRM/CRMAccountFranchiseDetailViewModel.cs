namespace JKViewModels.CRM
{
    using System;

    public class CRMAccountFranchiseDetailViewModel
    {
        public int CRM_AccountFranchiseDetailId { get; set; }
        public int CRM_AccountId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string CellNumber { get; set; }
        public string FaxNumber { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string Employer { get; set; }
        public string Position { get; set; }
        public int LeadSource { get; set; }
        public int JkFull { get; set; }
        public decimal AmtToInvest { get; set; }
        public DateTime InfoSentDate { get; set; }
        public DateTime DisclosedDate { get; set; }
        public DateTime C5DayPaperDate { get; set; }
        public DateTime EstCloseDate { get; set; }
        public DateTime SoldDate { get; set; }
        public string FranPlan { get; set; }
        public string SoldBy { get; set; }
        public decimal FranAmount { get; set; }
        public decimal DownAmount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string FranchiseeName { get; set; }
        public StageType Stage { get; set; }
        public StageStatusType StageStatus { get; set; }
        public int StageStatusId { get; set; }
        public AccountSourceProvider ProviderSource { get; set; }
        public AccountProviderType ProviderType { get; set; }
        public DateTime? CallBack { get; set; }

        public string ContactPerson { get; set; }
        public bool? InterestedInPerposal { get; set; }
        public string datestart { get; set; }
        public string timestart { get; set; }
        public string dateend { get; set; }
        public string timeend { get; set; }
        public string Representative { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Purpose { get; set; }
        public bool AMorPM { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? CRM_CallResultId { get; set; }
        public string SpokeWith { get; set; }
        public int? CRM_NoteTypeId { get; set; }
        public string CRM_Note { get; set; }

    }
}
