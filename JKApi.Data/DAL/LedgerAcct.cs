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
    
    public partial class LedgerAcct
    {
        public int LedgerAcctId { get; set; }
        public Nullable<int> GLAccountTypeListId { get; set; }
        public string GL_Number { get; set; }
        public string GL_Name { get; set; }
        public string LedgerAcctDescription { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}