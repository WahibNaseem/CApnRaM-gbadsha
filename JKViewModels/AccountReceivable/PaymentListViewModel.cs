using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class PaymentListViewModel
    {
        public int PaymentId { get; set; }
        public string RegionName { get; set; }
        public string PaymentType { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<decimal> CheckAmount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string PaymentNo { get; set; }
        public string PaymentDescription { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public decimal InvoiceBalance { get; set; }
    }
}
