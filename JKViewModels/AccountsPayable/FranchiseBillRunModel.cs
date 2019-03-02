using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    class FranchiseBillRunModel
    {
    }

    public class FPInvoiceListViewModel
    {
        public int BillingPayId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate{ get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string DetailDescription { get; set; }
        //public string Ebill { get; set; }
        //public string PrintInvoice { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string InvoiceNo { get; set; }


    }

    public class FPBillingPay
    {
        public int BillingPayId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string DetailDescription { get; set; }
        //public string Ebill { get; set; }
        //public string PrintInvoice { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string InvoiceNo { get; set; }
        public int? InvoiceId { get; set; }
        public string RegionName { get; set; }

    }
}
