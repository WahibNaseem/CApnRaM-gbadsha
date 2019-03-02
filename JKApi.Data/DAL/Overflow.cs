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
    
    public partial class Overflow
    {
        public int OverflowId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> MasterTrxDetailId { get; set; }
        public Nullable<int> SourceId { get; set; }
        public Nullable<int> SourceTypeListId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<decimal> CheckAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> ApplyAmount { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> PrimaryPaymentId { get; set; }
        public Nullable<int> sys_cust { get; set; }
        public string OverflowDescription { get; set; }
    
        public virtual Invoice Invoice { get; set; }
    }
}
