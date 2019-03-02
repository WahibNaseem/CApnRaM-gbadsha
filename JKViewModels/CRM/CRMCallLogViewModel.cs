using System;

namespace JKViewModels.CRM
{
   public class CRMCallLogViewModel
    {
        public int CRM_CallLogId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountId { get; set; }
        public int CRM_LeadSource { get; set; }
        public int CRM_CallResultId { get; set; }
        public int CRM_NoteTypeId { get; set; }
        public string SpokeWith { get; set; }
        public string Note { get; set; }
        public DateTime? CallLogDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int StageStatus { get; set; }
        public DateTime? Callback { get; set; }
        public DateTime? CallBackTime { get; set; }
    }
}
