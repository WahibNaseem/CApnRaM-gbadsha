using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class LockboxImportSummaryViewModel
    {
        public int LockboxImportId { get; set; }
        public string LockboxDate { get; set; }
        public string Batch { get; set; }
        public string BatchNumber { get; set; }
        public string RegionName { get; set; }
        public string RegionNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalApply { get; set; }
        public decimal TotalBalance { get; set; }
        public virtual List<LockboxImportSummaryDetailViewModel> SummaryDetails { get; set; }        
    }

    public class LockboxImportSummaryDetailViewModel
    {        
        public int LockboxImportDetailId { get; set; }
        public bool? IsProcessed { get; set; }
        public string CheckNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public decimal Apply { get; set; }
        public decimal Balance { get; set; }
        public int LockboxImportId { get; set; }
    }
}