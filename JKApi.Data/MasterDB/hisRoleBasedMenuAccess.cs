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
    
    public partial class hisRoleBasedMenuAccess
    {
        public Nullable<int> AccessId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsWriteAccess { get; set; }
        public Nullable<int> RegionId { get; set; }
        public bool IsEnable { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
