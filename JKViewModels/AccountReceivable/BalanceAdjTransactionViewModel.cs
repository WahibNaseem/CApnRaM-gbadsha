using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
   public class BalanceAdjTransactionViewModel
    {
        public int PaymentMethodListId { get; set; }
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int CustomerId { get; set; }
        public int RegionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public decimal PaymentAmount { get; set; }
        public decimal Balance { get; set; }

        public List<BalanceAdjInvoiceViewModel> Invoices { get; set; }
    }

    public class BalanceAdjInvoiceViewModel
    {
        public int InvoiceId { get; set; }
       
        public BalanceAdjCustomerViewModel CustomerPayment { get; set; }
    }

    public class BalanceAdjCustomerViewModel
    {
        public int CustomerId { get; set; }

    }
}
