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
    
    public partial class vw_sys_Fees
    {
        public Nullable<int> id { get; set; }
        public string feename { get; set; }
        public Nullable<decimal> rate { get; set; }
        public Nullable<int> ratetype { get; set; }
        public Nullable<int> feetype { get; set; }
        public Nullable<int> trxtypeid { get; set; }
        public Nullable<int> displayorder { get; set; }
        public Nullable<int> displaysubreport { get; set; }
        public Nullable<int> groupid { get; set; }
        public Nullable<int> status { get; set; }
        public int amount { get; set; }
        public string notes { get; set; }
    }
}