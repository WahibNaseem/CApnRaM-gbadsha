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
    
    public partial class PeriodClosed
    {
        public int PeriodClosedId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<bool> ChargebackFinalized { get; set; }
        public Nullable<bool> FranchiseeReport { get; set; }
        public Nullable<bool> MonthlyBillRun { get; set; }
        public Nullable<bool> Closed { get; set; }
        public Nullable<bool> NegativeDueFinalized { get; set; }
    }
}
