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
    
    public partial class vw_F_FindersFee
    {
        public int id { get; set; }
        public Nullable<int> franid { get; set; }
        public Nullable<int> customerid { get; set; }
        public Nullable<int> distributionid { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> typeid { get; set; }
        public Nullable<decimal> factor { get; set; }
        public Nullable<decimal> dppercentage { get; set; }
        public Nullable<decimal> interest { get; set; }
        public Nullable<decimal> billing { get; set; }
        public Nullable<decimal> adjustment { get; set; }
        public Nullable<decimal> payableon { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> downpayment { get; set; }
        public Nullable<decimal> amountfinanced { get; set; }
        public Nullable<decimal> balance { get; set; }
        public Nullable<int> noofpayments { get; set; }
        public Nullable<int> paymentsbilled { get; set; }
        public Nullable<decimal> monthlypayment { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> resumedate { get; set; }
        public Nullable<System.DateTime> statusdate { get; set; }
        public string statusnotes { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string typename { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public string customerNo { get; set; }
        public Nullable<decimal> amountpaid { get; set; }
        public string statusname { get; set; }
    }
}
