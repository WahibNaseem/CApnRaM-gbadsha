using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeWiseTransactionViewModel
    {
        public int MasterTrxId { get; set; }
        public int RegionId { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public int MasterTrxTypeListId { get; set; }
        public int TypeListId { get; set; }
        public DateTime TrxDate { get; set; }
        public int BillingPayId { get; set; }
        public int InvoiceId { get; set; }
        public int HeaderId { get; set; }
        public int TransactionStatusListId { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string PayDescription { get; set; }
        public int MasterTrxDetailId { get; set; }
        public string DetailDescription { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CPIPercentage { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal DistributionAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Total { get; set; }
        public int AmountTypeListId { get; set; }
        public string AmountTypeName { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string StatusName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }

        public string InvoiceNo { get; set; }
    }
    public class FranchiseeInitialBalanceViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
    }
}
