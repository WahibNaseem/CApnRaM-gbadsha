using System;
using System.Collections.Generic;
using JKViewModels.Inspection;

namespace JKViewModels.CRM.CRMSPModels
{
    public class CRM_CallLogListModel
    {
        public int CRM_CallLogId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountId { get; set; }
        public int CRM_LeadSource { get; set; }
        public int CRM_CallResultId { get; set; }
        public int CRM_NoteTypeId { get; set; }
        public string SpokeWith { get; set; }
        public string Note { get; set; }
        public DateTime CallLogDate { get; set; }
        public string LeadSource { get; set; }
        public string CallResult { get; set; }
        public string NoteType { get; set; }
        public string StageStatusName { get; set; }
        public DateTime? Callback { get; set; }
        public DateTime? CallBackTime { get; set; }
    }

    public class CallLogModel : BaseEntityModel
    {
        public int CallLogId { get; set; }
        public int AccountId { get; set; }
        public int AccountCustomerDetailId { get; set; }      
        public int LeadSource { get; set; }
        public int CallResultId { get; set; }
        public int StageStatus { get; set; }
        public int NoteTypeId { get; set; }
        public string SpokeWith { get; set; }
        public string Note { get; set; }
        public DateTime? CallLogDate { get; set; }
        public DateTime? CallBack { get; set; }
        public DateTime? CallBackTime { get; set; }
    }

    public class ViewCallLogListModel : PaggingModel
    {
        public int AccountId { get; set; }
        public int AccountCustomerDetailId { get; set; }
        public List<CallLogModel> CallLogList { get; set; }

        public ViewCallLogListModel()
        {
            CallLogList = new List<CallLogModel>();
        }
    }
}
