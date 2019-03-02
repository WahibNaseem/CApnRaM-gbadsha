using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class ManualTransactionViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<int> FranchiseeManualTransactionId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string TransactionNumber { get; set; }
        public string FranchiseeManualTransactionDescription { get; set; }
        public bool ReSell { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public DateTime? VendorInvoiceDate { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public DateTime? trxdate { get; set; }
        public string TrxStatus { get; set; }
        public string TrxType { get; set; }
        public Nullable<int> MastertrxDetailId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> DistributionAmount { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string DetailDescription { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }

    }
}
