using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class CustomerTransactionCommonViewModel
    {
        public int MasterTmpTrxId { get; set; }
        public int CustomerId { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public DateTime InvoiceDate { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceDescription { get; set; }
        public List<CustomerTransactionViewModel> CustomerTransactions { get; set; }
        public List<FranchiseeTransactionViewModel> FranchiseeTransactions { get; set; }
    }
    public class CustomerTransactionViewModel
    {
        public int CustomerTransactionId { get; set; }
        public int LineNo { get; set; }
        public int ServiceTypeListId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal MarkUpTotal { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
        public int TransactionStatusListId { get; set; }
        public int TaxRate { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool Commission { get; set; }
        public decimal CommissionTotal { get; set; }
        public bool ExtraWork { get; set; }
        public bool TaxExcempt { get; set; }
        public bool BPPAdmin { get; set; }
        public bool AccountRebate { get; set; }
        public bool PrintInvoice { get; set; }
        public bool ClientSupplies { get; set; }

    }
    public class FranchiseeTransactionViewModel
    {
        public int FranchiseeTransactionId { get; set; }
        public int CustomerTransactionId { get; set; }
        public int FranchiseeId { get; set; }
        //public int Number { get; set; }
        //public string Name { get; set; }
        //public int LineNo { get; set; }
        public decimal Amount { get; set; }
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public int TransactionStatusListId { get; set; }
        public bool IsDelete { get; set; }
    }
    public class TransactionStatusListViewModel
    {
        public int TransactionStatusListId { get; set; }
        public string Name { get; set; }
    }
    public class ContractDetailServiceTypeListViewModel
    {
        public int ContractDetailServiceTypeListId { get; set; }
        public string name { get; set; }
    }
}
