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
    
    public partial class vw_AR_PeriodOpen
    {
        public int id { get; set; }
        public Nullable<int> billmonth { get; set; }
        public Nullable<int> billyear { get; set; }
        public Nullable<int> isopen { get; set; }
        public Nullable<int> undocapability { get; set; }
        public Nullable<System.DateTime> billrundate { get; set; }
        public Nullable<int> isfinalized { get; set; }
        public Nullable<int> franchiseerptsgenerated { get; set; }
        public Nullable<System.DateTime> franchiseerptsdate { get; set; }
        public Nullable<int> chargebackgenerated { get; set; }
        public Nullable<System.DateTime> chargebackgenerateddate { get; set; }
        public Nullable<int> posted { get; set; }
        public Nullable<System.DateTime> posteddate { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public int invoicecount { get; set; }
        public int totalbilling { get; set; }
        public int currentperiod { get; set; }
    }
}
