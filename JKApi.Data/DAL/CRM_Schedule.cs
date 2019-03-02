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
    
    public partial class CRM_Schedule
    {
        public int CRM_ScheduleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CRM_AccountFranchiseDetailId { get; set; }
        public bool IsFromOutlook { get; set; }
        public Nullable<System.Guid> OutlookAppointmentGuid { get; set; }
        public Nullable<System.DateTime> OutlookSyncDate { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> IsAllDay { get; set; }
        public Nullable<int> CRM_ScheduleTypeId { get; set; }
        public Nullable<int> CRM_StageStatusType { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> AuthUserLoginId { get; set; }
        public Nullable<int> PurposeId { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
}