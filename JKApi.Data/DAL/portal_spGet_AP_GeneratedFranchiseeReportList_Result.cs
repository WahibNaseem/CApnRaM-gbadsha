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
    
    public partial class portal_spGet_AP_GeneratedFranchiseeReportList_Result
    {
        public int FranchiseeReportId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RegionAcronym { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<decimal> TotalRevenue { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> TotalDeductions { get; set; }
        public Nullable<decimal> TotalPayment { get; set; }
    }
}
