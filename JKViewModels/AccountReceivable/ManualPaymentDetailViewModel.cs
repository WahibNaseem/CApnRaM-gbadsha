using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class ManualPaymentDetailViewModel
    {
        public int PaymentId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ReferenceNo{ get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentDescription { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public List<ManualPaymentInvoiceDetailViewModel> Invoices { get; set; }

    }
    public class ManualPaymentInvoiceDetailViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string InvoiceMessage { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoiceBalanceAmount { get; set; }
        public decimal InvoiceTaxAmount { get; set; }
        public decimal InvoiceTaxPercentage { get; set; }
        public decimal InvoiceTotalAmount { get; set; }
        public decimal? PaymentApplyAmount { get; set; }
        public decimal? InvoiceNewBalanceAmount { get; set; }

        public List<ManualPaymentInvoiceItemDetailViewModel> InvoiceItems { get; set; }
        public List<ManualPaymentInvoiceFranchiseeItemDetailViewModel> InvoiceFranchiseeItems { get; set; }
        public List<InvoiceTransactionHistoryViewModel> lstInvoiceTransactionHistory { get; set; }
    }
    public class ManualPaymentInvoiceFranchiseeItemDetailViewModel
    {
        public int InvoiceId { get; set; }
        public int BillingPayId { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string Description { get; set; }
        public decimal? ContractAmount { get; set; }
        public DateTime? ContractDate { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal FeePercentage { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> NewBalance { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public string ServiceTypeListName { get; set; }
    }
    public class ManualPaymentInvoiceItemDetailViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string InvoiceMessage { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoiceBalanceAmount { get; set; }
        public decimal InvoiceTaxAmount { get; set; }
        public decimal InvoiceTotalAmount { get; set; }
        public decimal? PaymentApplyAmount { get; set; }
        public decimal? InvoiceNewBalanceAmount { get; set; }
    }
}
