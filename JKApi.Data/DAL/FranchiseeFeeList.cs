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
    
    public partial class FranchiseeFeeList
    {
        public int FranchiseeFeeListId { get; set; }
        public string Name { get; set; }
        public Nullable<int> FeeRateTypeListId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    
        public virtual FeeRateTypeList FeeRateTypeList { get; set; }
    }
}
