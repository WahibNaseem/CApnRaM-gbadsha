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
    
    public partial class portal_spGet_C_CheckDetails_Result
    {
        public int CheckBookId { get; set; }
        public string RegionName { get; set; }
        public string RegionAccountType { get; set; }
        public string RegionAddress1 { get; set; }
        public string RegionAddress2 { get; set; }
        public string BankName { get; set; }
        public string BankRegion { get; set; }
        public string BankAddress1 { get; set; }
        public string BankAddress2 { get; set; }
        public string BankPhone { get; set; }
        public string BankNumber { get; set; }
        public string PayeeNumber { get; set; }
        public string PayeeName { get; set; }
        public string PayeeAddress1 { get; set; }
        public string PayeeAddress2 { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<decimal> CheckAmount { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<int> CheckTypeListId { get; set; }
        public string CheckType { get; set; }
        public Nullable<int> FranchiseeReportId { get; set; }
        public Nullable<int> ManualCheckId { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
}