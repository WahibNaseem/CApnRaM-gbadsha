namespace JKViewModels.CRM
{
    using System;

    public class CRMActivityViewModel
    {
        public int CRM_ActivityId { get; set; }
        public int ActivityType { get; set; }
        public int? OutComeType { get; set; }
        public string Note { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual string ActivityTypeName { get; set; }
        public virtual string OutComeTypeName { get; set; }
    }
}
