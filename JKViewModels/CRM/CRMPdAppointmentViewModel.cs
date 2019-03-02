using System;

namespace JKViewModels.CRM
{  


    public class CRMPdAppointmentViewModel
    {
        public int CRM_PdAppointmentId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public string MeetPersonName { get; set; }
        public bool MeetDecisionMaker { get; set; }
        public bool PresentProposal { get; set; }
        public bool OverComeObjections { get; set; }
        public string Comment { get; set; }
        public bool EnteredProposalDetail { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public int RegionId { get; set; }

    }
}
