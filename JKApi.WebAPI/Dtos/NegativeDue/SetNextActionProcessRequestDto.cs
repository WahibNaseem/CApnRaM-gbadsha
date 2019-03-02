using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Dtos.NegativeDue
{

    public class SetNextActionRequestDto : IRequestDto
    {
        public int RegionId { get; set; }
        public int SelectedPeriodId { get; set; }
        //public string Rows { get; set; }
        public List<NegativeDueRows> SelectedRows { get; set; }
        public bool IsSelectedPeriodIdFinalized { get; set; }
        public string SelectedPeriodProcessStatus { get; set; }
    }


    public class NegativeDueRows : IRequestDto
    {
        public int NegativeDueId { get; set; }
        public decimal Rollover { get; set; }
        public decimal BalanceAfterRollover { get; set; }
        public bool IsNegativeDueProcessed { get; set; }
        public string ProcessStatusDescription { get; set; }
    }
}
