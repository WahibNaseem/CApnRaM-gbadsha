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
    
    public partial class vw_C_TrxDistribution
    {
        public int id { get; set; }
        public Nullable<int> customerid { get; set; }
        public Nullable<int> franid { get; set; }
        public Nullable<int> crecurringid { get; set; }
        public Nullable<int> contractdetailid { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<int> ratetype { get; set; }
        public Nullable<int> status { get; set; }
        public string notes { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
    }
}
