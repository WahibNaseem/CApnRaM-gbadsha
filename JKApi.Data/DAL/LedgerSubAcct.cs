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
    
    public partial class LedgerSubAcct
    {
        public int LedgerSubAcctId { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public Nullable<int> GLSubAcct_Number { get; set; }
        public string GLSubAcct_Name { get; set; }
        public string LedgerSubAcctDescription { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
