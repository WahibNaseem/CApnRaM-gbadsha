using System;

namespace JKViewModels.CRM
{  

    public class CRMFollowUpViewModel
    {
        public int CRM_FollowUpId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public int Close { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiededDate { get; set; }
        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public int RegionId { get; set; }

    }
}
