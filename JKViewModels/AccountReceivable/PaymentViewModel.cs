using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{    
    public class LockboxPaymentViewModel
    {
        public int LockboxId { get; set; }
        public int LockboxEDIDetailId { get; set; }
        public DateTime LockboxDate { get; set; }
        public int CustomerId { get; set; }
        public string ChaqueNumber { get; set; }
        public int InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal ApplyAmount { get; set; }
        public decimal Overflowamount { get; set; }
        public decimal CheckAmount { get; set; }
        public int PaymentMethodListId { get; set; }
        public int TransactionStatusListId { get; set; }
        public bool NewMatch { get; set; }
        public bool IsODeposit { get; set; }
        public List<LockboxPaymentItemViewModel> Items { get; set; }
    }

    public class LockboxPaymentItemViewModel
    {
        public int InvoiceId { get; set; }
        public int BillPayId { get; set; }
        public int LineNumber { get; set; }
        public int CustomerId { get; set; }
        public int FranchiseeId { get; set; }
        public decimal ApplyAmount { get; set; }
        public bool IsCustomerSide { get; set; }
    }
}
