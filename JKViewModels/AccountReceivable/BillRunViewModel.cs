using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class BillRunViewModel
    {
        [DisplayName("Bill Month/Year")]
        public string billMonth { get; set; }
        [DisplayName("")]
        public string billYear { get; set; }
        [DisplayName("Apply CPI Increase")]
        public bool applyCPIIncrease { get; set; }
    }


    public class BillRunSummaryDetailViewModel
    {
        public string BatchIds { get; set; }
        public string BatchNumber { get; set; }
        public decimal TotalInvoiceCount { get; set; }
        public string TotalInvoiceAmount { get; set; }
        public string InvoiceCreatedOn { get; set; }
    }

    public class ARLBInvoiceListViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
    }

    public class ARInvoiceListViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public string Ebill { get; set; }
        public string PrintInvoice { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Cpi { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string IsOpen { get; set; }
        public bool HasMultipleLineItems { get; set; } // needed to know if partial payment form must be shown
        public string Region { get; set; }
        public string TransactionStatus { get; set; }
        public Nullable<int> BillingPayId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<int> BillingPayCount { get; set; }
        public string BillingPayIds { get; set; }
        public string BillingPayNumbers { get; set; }
        public int ParentCustomerId { get; set; }
    }

    public class ARCustomerWithCreditListViewModel
    {
        public int CreditId { get; set; }
        public string TransactionNumber { get; set; }
        public string CreditDate { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public decimal InvAmount { get; set; }
        public decimal CrdAmount { get; set; }
        public string Other { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public DateTime CreditDateForCreditList { get; set; }
        public int CustomerId { get; set; }
        public int TransactionStatusListId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public bool IsTaxCredit { get; set; }

    }

    public class InvoiceDetailsForNumber
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public decimal Total { get; set; }
        public int TransactionStatusListId { get; set; }
    }
}