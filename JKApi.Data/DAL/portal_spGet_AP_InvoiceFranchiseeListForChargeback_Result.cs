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
    
    public partial class portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result
    {
        public int CBId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RegionName { get; set; }
        public Nullable<int> BillingPayId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> PaymentsAndCredits { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public Nullable<decimal> ChargebackAmount { get; set; }
        public Nullable<decimal> CustomerId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> PeriodId { get; set; }
    }
}