//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.MasterDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class hisMenu
    {
        public Nullable<int> MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int MenuLevel { get; set; }
        public int MenuOrder { get; set; }
        public int ParentMenuId { get; set; }
        public string PageName { get; set; }
        public int ModuleId { get; set; }
        public bool IsDisplay { get; set; }
        public string MenuImageUrl { get; set; }
        public Nullable<int> RegionId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEnable { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}