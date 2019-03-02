using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class FranchiseeChargebackCreditViewModel
    {
        public int FranchiseeChargebackCreditId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        //public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> FranchiseeManualTrxCreditReasonListId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Fee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Description { get; set; }
        public Nullable<int> RegionId { get; set; }

        //public Nullable<System.DateTime> StartDate { get; set; }
        //public Nullable<int> NumOfPayments { get; set; }
        //public Nullable<int> PaymentsBilled { get; set; }
        //public Nullable<decimal> GrossTotal { get; set; }
        //public Nullable<int> Quantity { get; set; }
        //public Nullable<decimal> ItemAmount { get; set; }

        //public Nullable<bool> IsActive { get; set; }

        //public Nullable<int> CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public Nullable<int> ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public Nullable<int> StatusListId { get; set; }
        //public bool ReSell { get; set; }
        //public int? VendorId { get; set; }
        //public string VendorInvoiceNumber { get; set; }
        //public DateTime? VendorInvoiceDate { get; set; }
        //public bool IsCredit { get; set; }
        //public int? FranchsieeTrxTypeListId { get; set; }
        //public string PaymentNo { get; set; }


        //public int? BillingPayId { get; set; }
        public string InvoiceNo { get; set; }
        public int? InvoiceId { get; set; }
        public int? BillingPayId { get; set; }
        public int? ChargebackId { get; set; }
        public decimal? ChargebackAmount { get; set; }
        public decimal? ApplyAmount { get; set; }
    }
}
