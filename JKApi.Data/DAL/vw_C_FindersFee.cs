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
    
    public partial class vw_C_FindersFee
    {
        public int id { get; set; }
        public Nullable<int> customerid { get; set; }
        public int franchiseeid { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<int> noofpayments { get; set; }
        public string paymentsbilled { get; set; }
        public Nullable<decimal> monthlypayment { get; set; }
        public string status { get; set; }
    }
}
