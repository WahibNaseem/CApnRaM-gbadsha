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
    
    public partial class GeneralLedger
    {
        public int GeneralLedgerId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
    }
}
