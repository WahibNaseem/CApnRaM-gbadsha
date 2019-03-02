//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillingPay
    {
        public int BillingPayId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> BillingPayTypeListId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string BillingPayDescription { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<bool> ExtraWork { get; set; }
        public bool HasBeenChargedBack { get; set; }
        public Nullable<System.DateTime> ChargebackDate { get; set; }
        public Nullable<bool> IsTARPayment { get; set; }
        public Nullable<bool> IsChargebackPaid { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}