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
    
    public partial class transLoginTracking
    {
        public long LoginTrackingId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> LoginDateTime { get; set; }
        public Nullable<System.DateTime> LogoutDateTime { get; set; }
        public string IPAddress { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual mstrRegion mstrRegion { get; set; }
        public virtual transUser transUser { get; set; }
    }
}
