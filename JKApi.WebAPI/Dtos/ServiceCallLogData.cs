using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Dtos
{
    public class ServiceCallLogData : IResponseDto
    {
       public List<ServiceCallLogAreaList> ServiceCallLogAreaList { get; set; }
        public List<ServiceCallLogTypeList> ServiceCallLogTypeList { get; set; }
        public List<StatusResultList> StatusResultList { get; set; }
        public List<UserResultList> UserResultList { get; set; }
    }
    public class ServiceCallLogAreaList
    {
        public int ServiceCallLogAreaListId { get; set; }
        public string Name { get; set; }
    }
    public  class ServiceCallLogTypeList
    {
        public int ServiceCallLogTypeListId { get; set; }
        public string Name { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
    public  class StatusResultList
    {
        
        public int StatusResultListId { get; set; }
        public string Name { get; set; }         
    }
    public class UserResultList
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CollectionsCallLogData : IResponseDto
    {
        public int CollectionsCallLogId { get; set; }        
        public Nullable<System.DateTime> CallDate { get; set; }
        public string CallTime { get; set; }
        public string Status { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }                
        public string Comments { get; set; }    
    }

    public class ServiceCallLogList : IResponseDto
    {
        public int ServiceCallLogId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> CallLogAssociateId { get; set; }
        public string CallDate { get; set; }
        public string CallTime { get; set; }
        public Nullable<int> Internal { get; set; }
        public Nullable<int> CallLogInitiatedByTypeListId { get; set; }
        public Nullable<int> InitiatedById { get; set; }
        public Nullable<int> ServiceLogTypeListId { get; set; }
        public Nullable<int> StatusResultListId { get; set; }
        public Nullable<int> ServiceLogAreaListId { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        public Nullable<int> FollowUpBy { get; set; }
        public string EmailNotesTo { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }        
        public Nullable<int> RegionId { get; set; }
      
         

    }

    public class FranchiseModel : IResponseDto
    {
        public string FranchiseId { get; set; }
        public string FranchiseName { get; set; }
    }
}