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
    
    public partial class portal_spGet_AP_FranchiseeReportDetails_Result
    {
        public Nullable<int> FranchiseeReportId { get; set; }
        public Nullable<int> FranchiseeReportDetailId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public Nullable<bool> IsDeduction { get; set; }
        public string Description { get; set; }
        public string DescriptionMisc { get; set; }
        public Nullable<int> ReSell { get; set; }
        public Nullable<decimal> Subtotal { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> BillingPayId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> LineNo { get; set; }
        public Nullable<int> IsCommission { get; set; }
        public string InvoiceType { get; set; }
    }
}