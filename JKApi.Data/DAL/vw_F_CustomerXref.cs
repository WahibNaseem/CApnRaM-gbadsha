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
    
    public partial class vw_F_CustomerXref
    {
        public int id { get; set; }
        public int franid { get; set; }
        public int customerid { get; set; }
        public Nullable<decimal> responsibility { get; set; }
        public System.DateTime startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public int status { get; set; }
        public string notes { get; set; }
        public int createdby { get; set; }
        public System.DateTime createdate { get; set; }
        public string tmp_dlrcode { get; set; }
        public string tmp_status { get; set; }
    }
}
