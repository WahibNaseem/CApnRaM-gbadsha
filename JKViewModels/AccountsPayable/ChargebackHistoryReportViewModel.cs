using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    public class ChargebackHistoryReportViewModel
    {
        public int ChargebackId { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime TrxDate { get; set; }
        public decimal? CBAmount { get; set; }
        public string TARCheckNo { get; set; }
        public decimal? TARAmount { get; set; }
        public decimal? BalanceAmount { get; set; }

    }

    public class ChargebackHistoryReportSummaryViewModel
    {
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public DateTime TrxDate { get; set; }
        public decimal? CBAmount { get; set; }
        public string TARCheckNo { get; set; }
        public decimal? TARAmount { get; set; }
        public decimal? BalanceAmount { get; set; }

    }
}
