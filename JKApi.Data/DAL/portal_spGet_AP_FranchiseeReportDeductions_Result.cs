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
    
    public partial class portal_spGet_AP_FranchiseeReportDeductions_Result
    {
        public int FranchiseeReportDetailId { get; set; }
        public Nullable<int> FranchiseeReportId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public Nullable<bool> IsDeduction { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public Nullable<bool> DisplaySubReport { get; set; }
        public Nullable<int> ReSell { get; set; }
        public string Description { get; set; }
        public string DescriptionMisc { get; set; }
        public Nullable<decimal> Subtotal { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int MasterTrxTypeListId { get; set; }
        public int MasterTrxDetailId { get; set; }
        public int InvoiceId { get; set; }
        public int BillingPayId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }
        public int LineNo { get; set; }
        public int IsCommission { get; set; }
        public System.DateTime LeaseDate { get; set; }
        public string LeaseNo { get; set; }
        public string LeaseSerialNo { get; set; }
        public string CurrentPaymentNo { get; set; }
        public int TotalPaymentNo { get; set; }
        public int MasterTrxFeeDetailId { get; set; }
        public int FeeId { get; set; }
        public bool IsSpecial { get; set; }
        public bool DisplayLeaseMessage { get; set; }
        public decimal TotalCBFee { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> ServiceTypeGroupListId { get; set; }
    }
}
