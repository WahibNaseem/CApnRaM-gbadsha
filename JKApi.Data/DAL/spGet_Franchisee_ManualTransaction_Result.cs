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
    
    public partial class spGet_Franchisee_ManualTransaction_Result
    {
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public string TrXStatus { get; set; }
        public string DetailDescription { get; set; }
        public string TrxType { get; set; }
        public Nullable<bool> ReSell { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> DistributionAmount { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string FranchiseeManualTransactionDescription { get; set; }
    }
}
