using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class LeaseBillDetailViewModel
    {
        public Nullable<int> ClassId { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string Dateformat { get; set; }
        public string LeaseBillNo { get; set; }
        public string TransactionNumber { get; set; }
        public string LeaseNumber { get; set; }
        public string Description { get; set; }
        public string PaymentNumber { get; set; }
        public Nullable<int> InstallmentCount { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string TransactionStatus { get; set; }
        public string RegionName { get; set; }
        public string DetailDescription { get; set; }
    }
}
