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
    
    public partial class CPI
    {
        public int CPIId { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public decimal CPIPpercent { get; set; }
        public string Description { get; set; }
        public Nullable<int> Applied { get; set; }
        public Nullable<bool> IsEnable { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public Nullable<int> PeriodId { get; set; }
    }
}
