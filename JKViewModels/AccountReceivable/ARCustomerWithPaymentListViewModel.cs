using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class ARCustomerWithPaymentListViewModel
    {
        public int PaymentId { get; set; }
        public string TransactionNumber { get; set; }
        public string CreditDate { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public decimal InvAmount { get; set; }
        public decimal CrdAmount { get; set; }
        public string Other { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public DateTime CreditDateForCreditList { get; set; }
        public int CustomerId { get; set; }
        public int TransactionStatusListId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }
}
