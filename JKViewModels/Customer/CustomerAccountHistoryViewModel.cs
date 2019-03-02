using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerAccountHistoryViewModel
    {
        public CustomerSimpleViewModel Customer { get; set; }

        public List<CustomerAccountHistoryPerFranchiseeViewModel> Franchisees { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal OtherTrxTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal AmountDue { get; set; }
    }

    public class CustomerAccountHistoryPerFranchiseeViewModel
    {
        public string FranchiseeNo;
        public string FranchiseeName;

        public List<CustomerAccountHistoryPerInvoiceViewModel> Invoices { get; set; }
    }

    public class CustomerAccountHistoryPerInvoiceViewModel
    {
        public CustomerAccountHistoryPerTransactionViewModel Invoice { get; set; }
        public List<CustomerAccountHistoryPerTransactionViewModel> Payments { get; set; }
        public List<CustomerAccountHistoryPerTransactionViewModel> Credits { get; set; }

        public decimal Balance { get; set; }
    }

    public class CustomerAccountHistoryPerTransactionViewModel
    {
        public string TrxNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
