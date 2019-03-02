using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FinderFeeBillDetailViewModel
    {
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<System.DateTime> FindersFeeBillDate { get; set; }
        public string Dateformat { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string transactionNumber { get; set; }
        public Nullable<int> FindersFeeId { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public string CustomerNo { get; set; }
        public string Name { get; set; }
        public string FindersFeeType { get; set; }
        public string FFStatus { get; set; }
        public string PaymentNumber { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string DetailDescription { get; set; }
        public string Notes { get; set; }
    }

    public class FinderfeeReportListResultModel
    {
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public string transactionNumber { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public string CustomerNo { get; set; }
        public string Name { get; set; }
        public string FindersFeeType { get; set; }
        public string FFStatus { get; set; }
        public string PaymentNumber { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string RegionName { get; set; }

        public string SequenceNum { get; set; }
        public Nullable<System.DateTime> FindersFeeBillDate { get; set; }
        public Nullable<decimal> ContractBillingAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> PayableOnAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
         
    }
}
