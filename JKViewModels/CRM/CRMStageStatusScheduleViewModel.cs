namespace JKViewModels.CRM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public  class CRMStageStatusScheduleViewModel
    {
        public int CRM_StageStatusScheduleId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public int CRM_InitialCommunicationId { get; set; }
        public int CRM_PdAppointmentId { get; set; }
        public int CRM_FollowUpId { get; set; }
        public int? CRM_BiddingId { get; set; }
        public DateTime? Schedule1 { get; set; }
        public DateTime? Schedule2 { get; set; }
        public string Schedule2Format { get; set; }
        public string Schedule1Format { get; set; }
        public int Purpose1 { get; set; }
        public int Purpose2 { get; set; }
        public string Note { get; set; }
        public string Title { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

    }
}
