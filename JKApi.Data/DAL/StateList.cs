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
    
    public partial class StateList
    {
        public int StateListId { get; set; }
        public string Name { get; set; }
        public string abbr { get; set; }
        public Nullable<int> CountryCodeListId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}