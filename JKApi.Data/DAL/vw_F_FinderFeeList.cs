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
    
    public partial class vw_F_FinderFeeList
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<decimal> TotalFinderFee { get; set; }
        public Nullable<decimal> TotalDownPayment { get; set; }
        public Nullable<decimal> Expr1 { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RegionName { get; set; }
    }
}