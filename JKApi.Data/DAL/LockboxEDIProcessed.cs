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
    
    public partial class LockboxEDIProcessed
    {
        public int LockboxEDIProcessedId { get; set; }
        public int LockboxEDIId { get; set; }
        public int LockboxEDIHistoryId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<decimal> CheckAmount { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<decimal> ApplyAmount { get; set; }
        public Nullable<bool> IsProcessed { get; set; }
        public Nullable<bool> ProcessedOn { get; set; }
    }
}
