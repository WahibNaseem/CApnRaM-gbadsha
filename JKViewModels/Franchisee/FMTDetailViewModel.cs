using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
   public class FMTDetailViewModel
    {
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public string TrXStatus { get; set; }
        public string DetailDescription { get; set; }
        public string TrxType { get; set; }
        public Nullable<bool> ReSell { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> DistributionAmount { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string FranchiseeManualTransactionDescription { get; set; }
        public string Dateformat { get; set; }
        public string reSellName{ get; set; }
    }
}
