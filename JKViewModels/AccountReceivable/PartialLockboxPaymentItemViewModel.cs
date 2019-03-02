using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class PartialLockboxPaymentItemViewModel
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
