using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class OverPaymentCustomerInvoiceViewModel
    {
        public int OverflowId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public decimal OverflowAmount { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string InvoiceDescription { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal BalanceAmount { get; set; }
    }

    public class OverPaymentListViewModel
    {
        public int OverflowId { get; set; }
        public string Region { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CheckNumber { get; set; }
        public decimal CheckAmount { get; set; }
        public decimal PaymentApplyAmount { get; set; }
        public decimal OverflowAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public int SourceTypeListId { get; set; }
        public int SourceId { get; set; }
        public string InvoiceDescription { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoiceBalance { get; set; }
    }
}
