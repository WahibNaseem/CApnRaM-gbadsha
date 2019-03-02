using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 


namespace JKViewModels.AccountReceivable
{
    public class CreditTransactionViewModel
    {
        public int Id { get; set; } // Credit
        public int RegionId { get; set; }
        public int InvoiceId { get; set; }
        public int CreditReasonListId { get; set; }
        public string CreditDescription { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ApplyTotalCredit { get; set; }
        public bool IsExtraCredit { get; set; }
        public bool PaidInFull { get; set; }

        public int BillMonth { get; set; }
        public int BillYear { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public CustomerCreditViewModel CustomerCredit { get; set; }
        public List<FranchiseeCreditViewModel> FranchiseeCredits { get; set; }

        public string Note { get; set; }
    }
    public class CustomerCreditViewModel
    {
        public int CustomerId { get; set; }

        public List<CreditViewModel> Credits { get; set; }
    }
    public class FranchiseeCreditViewModel
    {
        public int FranchiseeId { get; set; }
        public int BillingPayId { get; set; }

        public CreditViewModel Credit { get; set; }
    }

    public class CreditViewModel
    {
        public int BaseMasterTrxDetailId { get; set; }
        public int LineNo { get; set; }
        public int ServiceTypeListId { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class CustomerCreditDetailsPopupModel
    {
        public int CreditId{ get; set; }
        public string CreditNo { get; set; }
        public DateTime? CreditDate { get; set; }         
        public decimal CreditAmount { get; set; }        

        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }        

        public List<CustomerCreditPaymentType> CustomerCreditPaymentTypeList { get; set; }
        public List<InvoiceTransactionHistoryViewModel> InvoiceTransactionHistoryList { get; set; }
        public List<InvoiceContractDetailListViewModel> InvoiceDetailItems { get; set; }

        public string Description{ get; set; }
    }

    public class InvoiceContractDetailListViewModel
    {
        public int InvoiceId { get; set; }
        public int? LineNumber { get; set; }
        public int? Quantity { get; set; }
        public int MasterTrxDetailId { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? ExtendedPrice { get; set; }
        public decimal TAXAmount { get; set; }
        public decimal? Total { get; set; }
        public int? MasterTrxId { get; set; }
        public decimal? Balance { get; set; }
        public int? ServiceTypeListId { get; set; }
    }

    public class CustomerCreditPaymentType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentDetailsPopupModel
    {
        public int PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal PaymentAmount { get; set; }

        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }

        public List<PaymentDetailType> PaymentDetailType { get; set; } 
        public List<InvoiceTransactionHistoryViewModel> InvoiceTransactionHistoryList { get; set; }
        public List<InvoiceContractDetailListViewModel> InvoiceDetailItems { get; set; }

        public string Note { get; set; }
        public string TransactionNumber { get; set; }

        public int? RegionId { get; set; }

        public List<PaymentDetailInvoiceTransactionHistory> PaymentDetailInvoiceTransactionHistoryList { get; set; }
    }
    public class PaymentDetailInvoiceTransactionHistory
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public List<InvoiceTransactionHistoryViewModel> InvoiceTransactionHistoryList { get; set; }

    }


    public class PaymentDetailType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime ? Date { get; set; }
        public string Number { get; set; }

        public string TransactionNumber { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public decimal BalanceAmount { get; set; }
        public int MasterTrxTypeListId { get; set; }
        public int InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int? AmountTypeListId { get; set; }

        public int? CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

    }
}
