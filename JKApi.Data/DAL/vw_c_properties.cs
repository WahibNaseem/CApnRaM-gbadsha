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
    
    public partial class vw_c_properties
    {
        public int id { get; set; }
        public string customerNo { get; set; }
        public string name { get; set; }
        public string statusname { get; set; }
        public Nullable<int> nationalaccount { get; set; }
        public Nullable<int> parentid { get; set; }
        public Nullable<int> reference { get; set; }
        public int ebill { get; set; }
        public int taxexempt { get; set; }
        public Nullable<int> printinvoice { get; set; }
        public Nullable<int> printpastdue { get; set; }
        public int cpiincrease { get; set; }
    }
}