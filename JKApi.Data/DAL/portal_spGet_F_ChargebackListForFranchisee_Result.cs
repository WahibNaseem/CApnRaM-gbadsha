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
    
    public partial class portal_spGet_F_ChargebackListForFranchisee_Result
    {
        public int ChargebackId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public string TransactionNumber { get; set; }
        public string FranchiseeNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> InvoiceBalance { get; set; }
        public Nullable<decimal> ChargeBackAmount { get; set; }
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
    }
}
