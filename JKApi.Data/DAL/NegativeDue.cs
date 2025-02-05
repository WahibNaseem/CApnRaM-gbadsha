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
    
    public partial class NegativeDue
    {
        public int NegativeDueId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> ApplySourceId { get; set; }
        public Nullable<bool> Rollover { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AppliedAmount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> TransactionNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ImpId { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
