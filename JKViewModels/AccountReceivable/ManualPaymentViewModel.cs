using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class ManualPaymentTransactionViewModel
    {
        public int PaymentMethodListId { get; set; }
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int CustomerId { get; set; }
        public int InvoiceId { get; set; }
        public int RegionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public decimal PaymentAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }

        public List<ManualPaymentInvoiceViewModel> Invoices { get; set; }

        public string TransactionNumber { get; set; }


        public bool? overflowApplyCredit { get; set; }
        public int? OverflowId { get; set; }
        public int? overflowSourceId { get; set; }
        public string overflowCheckNumber { get; set; }
        public decimal? overflowCheckAmount { get; set; }

        public int? FromOverflowId { get; set; }
        public decimal? FromOverflowAmount { get; set; }
    }
    public class ManualPaymentInvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public bool PaidInFull { get; set; }
        public bool IsManualInvoice { get; set; }

        public decimal overflowAmount { get; set; }
        public ManualPaymentCustomerViewModel CustomerPayment { get; set; }
        public List<ManualPaymentFranchiseeViewModel> FranchiseePayments { get; set; }

    }
    public class ManualPaymentCustomerViewModel
    {
        public int CustomerId { get; set; }

        public List<ManualPaymentViewModel> Payments { get; set; }
    }
    public class ManualPaymentFranchiseeViewModel
    {
        public int FranchiseeId { get; set; }
        public int BillingPayId { get; set; }

        public ManualPaymentViewModel Payment { get; set; }

        public string TransactionNumber { get; set; }
        public Boolean IsTARPaid { get; set; }

        public Boolean IsTurnAroundPayment { get; set; }
    }
    public class ManualPaymentViewModel
    {
        public int MasterTrxDetailId { get; set; }
        public int LineNo { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal ExtendedPrice { get; set; }
    }


    public class FullManualPaymentViewModel
    {


        public int RegionId { get; set; }
        public int PaymentMethodListId { get; set; }
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int CustomerId { get; set; }        
        public decimal PaymentAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<MPInvoiceViewModel> Invoices { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        //public bool? overflowApplyCredit { get; set; }
        //public int? OverflowId { get; set; }
        //public int? overflowSourceId { get; set; }
        //public string overflowCheckNumber { get; set; }
        //public decimal? overflowCheckAmount { get; set; }

        //public int? FromOverflowId { get; set; }
        //public decimal? FromOverflowAmount { get; set; }

        public int? OInvoiceId { get; set; }
        public string OInvoiceNo { get; set; }
    }

    public class MPInvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public int InvoiceCustomerId { get; set; }
        public decimal InvoicePayment { get; set; }
        public decimal InvoiceBalance { get; set; }
        public decimal OverflowAmount { get; set; }
        public bool PaidInFull { get; set; }
        public bool IsManualInvoice { get; set; }
        public ManualPaymentCustomerViewModel CustomerPayment { get; set; }
        public List<ManualPaymentFranchiseeViewModel> FranchiseePayments { get; set; }

    }



}
