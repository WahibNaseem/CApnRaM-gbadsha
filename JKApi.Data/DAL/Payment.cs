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
    
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> PaymentMethodListId { get; set; }
        public string PaymentNo { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> TempTransactionStatusListId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string PaymentDescription { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<decimal> CheckAmount { get; set; }
        public Nullable<int> PrimaryPaymentId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<bool> IsPaymentReversal { get; set; }
        public Nullable<int> LBID { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}