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
    
    public partial class GeneralLedgerTrx
    {
        public int GeneralLedgerTrxId { get; set; }
        public Nullable<int> MasterTrxDetailId { get; set; }
        public Nullable<int> LedgerAccountId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<System.DateTime> TrxDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> TrxId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public string Number { get; set; }
        public string TransactionNumber { get; set; }
        public string Payee_Payer { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public Nullable<int> GLAmountTypeListId { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
    }
}
