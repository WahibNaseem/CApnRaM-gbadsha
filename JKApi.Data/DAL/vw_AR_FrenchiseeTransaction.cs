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
    
    public partial class vw_AR_FrenchiseeTransaction
    {
        public int ManualInvoiceTmpDistributionId { get; set; }
        public Nullable<int> ManualInvoiceTmpDetailId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<int> MasterTmpTrxId { get; set; }
        public Nullable<int> LineNo { get; set; }
        public int TransactionStatusListId { get; set; }
    }
}
