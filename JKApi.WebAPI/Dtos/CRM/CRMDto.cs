using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Dtos.CRM
{
    public class CrmPotentialCustomerRequestDto : PageRequestDto
    {
        public int UserId { get; set; }
        public int RegionId { get; set; }
    }

    public class CrmCallLogByAccountRequestDto : PageRequestDto
    {
        public int AccountId { get; set; }
    }

    public class CrmCallLogByAccountCustomerDetailRequestDto : PageRequestDto
    {
        public int AccountCustomerDetailId { get; set; }
    }

    public class CrmCallLogUpdateRequestDto : IRequestDto
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
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}