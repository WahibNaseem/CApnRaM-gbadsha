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
    
    public partial class portal_spGet_AP_APBillListById_Result
    {
        public int APBillId { get; set; }
        public string RegionAcronym { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> FranchiseeReportId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> CheckBookId { get; set; }
    }
}
