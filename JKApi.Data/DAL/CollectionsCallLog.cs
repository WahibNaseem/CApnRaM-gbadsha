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
    
    public partial class CollectionsCallLog
    {
        public int CollectionsCallLogId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> CallLogAssociateId { get; set; }
        public Nullable<System.DateTime> CallDate { get; set; }
        public string CallTime { get; set; }
        public Nullable<int> Internal { get; set; }
        public Nullable<int> CallLogInitiatedByTypeListId { get; set; }
        public Nullable<int> InitiatedById { get; set; }
        public Nullable<int> StatusResultListId { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        public Nullable<int> FollowUpBy { get; set; }
        public string EmailNotesTo { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsCallBack { get; set; }
        public Nullable<System.TimeSpan> CallBackTime { get; set; }
    }
}
