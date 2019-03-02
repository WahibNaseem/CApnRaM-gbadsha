using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class InvoiceListViewModel
    {
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public bool EBill { get; set; }
        public bool PrintInvoice { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> InvoiceTax { get; set; }
        public Nullable<decimal> InvoiceTotal { get; set; }
        public Nullable<decimal> CPI { get; set; }
        public int TransactionStatusListId { get; set; }
        public string TransactionStatus { get; set; }
        public int InvoiceBalanceAmount { get; set; }
        public int InvoiceBalanceTax { get; set; }
        public int InvoiceBalanceTotal { get; set; }
        public string EBillText { get { return this.EBill == true ? "E" : ""; } }
        public string PrintInvoiceText { get { return this.PrintInvoice == true ? "P" : ""; } }
        public string IsOpen { get { return string.Equals(this.TransactionStatus, "Open", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(this.TransactionStatus, "Paid Partial", StringComparison.InvariantCultureIgnoreCase) 
                    || string.Equals(this.TransactionStatus, "Completed", StringComparison.InvariantCultureIgnoreCase) ? "Y" : "N"; } }

        public bool? ConsolidatedInvoice { get; set; }
        public int? ConsolidatedInvoiceId { get; set; }
        public string ConsolidatedInvoiceNo { get; set; }
        public int CreditId { get; set; }
    }
}
