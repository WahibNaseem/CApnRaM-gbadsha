using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class LBViewModel
    {
        public int LockboxEDIDetailId { get; set; }
        public int CustomerId { get; set; }
        public string ChaqueNumber { get; set; }
        public decimal CheckAmount { get; set; }
        public int InvoiceId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal ApplyAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal Overflowamount { get; set; }
        public bool NewMatch { get; set; }
        public bool IsOtherDeposit { get; set; }
        public List<PartialLockboxPaymentItemViewModel> PartialLockboxPaymentItems { get; set; }
    }
}
