using System;
using System.Collections.Generic;

namespace JKViewModels.CRM
{
    public class CRMScheduleUserCalendarViewModel
    {
        public List<CRMScheduleViewModel> lstCRMScheduleUserCalender { get; set; }
        public List<CRMScheduleUserHierarchy> lstCRMUserHierarchy { get; set; }
    }

    public class CRMScheduleViewModel
    {
        public int CRM_ScheduleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        //public int? CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public string Location { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAllDay { get; set; }
        public int? CRM_ScheduleTypeId { get; set; }
        public int? CRM_StageStatusType { get; set; }
        public int? RegionId { get; set; }
        public int? CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public int? AuthUserLoginId { get; set; }
        public bool IsFromOutlook { get; set; }
        public Nullable<System.Guid> OutlookAppointmentGuid { get; set; }
        public Nullable<System.DateTime> OutlookSyncDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }

    public class CRMScheduleUserHierarchy
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ParentUserId { get; set; }
        public string UserName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int? OrderBy { get; set; }
        public string ParentUserName { get; set; }
        public string Condition { get; set; }
        public string lstUserId { get; set; }
        public string lstRegionId { get; set; }
        public string FirstName { get; set; }

    }
}
