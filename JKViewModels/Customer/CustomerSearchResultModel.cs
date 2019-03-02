using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerSearchResultModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string RegionName { get; set; }
        public int FranchiseeId { get; set; }

        //public string City { get; set; }
        //public string StateName { get; set; }
        //public string PostalCode { get; set; }
        //public Nullable<int> AccountTypeListId { get; set; }
        //public string AccountTypeListName { get; set; }
        //public string StatusName { get; set; }
        //public string CreatedBy { get; set; }

    }

    public class SearchCustomerListId
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public decimal ContractAmount{ get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public bool Selected{ get; set; }
    }
    public class SearchFranchiseeListId
    {
        public int Id { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public decimal DistributionAmount { get; set; }        
        public bool Selected { get; set; }
    }



    public class CustomerSearchCancallationPendingResultViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public string AccountTypeListName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string RegionName { get; set; }
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string Reason { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string LeftDay { get; set; }
        public string Comments { get; set; }
        public DateTime? ComplaintsDate { get; set; }
        public string FollowUpBy { get; set; }        
        public string ServiceCallLogId { get; set; }
        public int StageStatusId { get; set; }
        public string StageStatusName { get; set; }
        public string EmailNotesTo { get; set; }         
    }

    public class CustomerServiceWednesdayReportResultViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public string AccountTypeListName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string RegionName { get; set; }
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string Reason { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string LeftDay { get; set; }
        public string Comments { get; set; }
        public DateTime? CallDate { get; set; }
        public TimeSpan? CallTime { get; set; }
        public string FollowUpBy { get; set; }
        public string ServiceCallLogId { get; set; }
        public DateTime? StartDate { get; set; }         
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public DateTime? CallBack { get; set; }
        public string InitiatedById { get; set; }
        public int CleanTimes { get; set; }
        public string CleanFrequencyListName { get; set; }
        public string CallLogInitiatedByTypeListName { get; set; }
        public string ServiceCallLogTypeListName { get; set; }
        public string StatusResultListName { get; set; }       
    }

    public class CustomerCancellationStageModel
    { 
        public int ValidationItemId { get; set; }
        public string Name { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> StatusListID { get; set; }                 
        public bool Selected { get; set; }
        public string ItemNote { get; set; }
        public Nullable<DateTime> StaticDesignScheduleDate { get; set; }       
        public Nullable<DateTime> StaticDesignScheduleEndTime { get; set; }
    }

    public class CustomerCancellationActivityModel
    {
        public int CSActivityId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ValidationItemId { get; set; }
        public Nullable<bool> IsItemChecked { get; set; }
        public Nullable<int> StatusListId { get; set; } 
        public string StageName { get; set; }
        public string ItemOptionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }


        public Nullable<bool> IsStaticDesign { get; set; }
        public Nullable<int> StaticDesignItemNo { get; set; }
        public Nullable<bool> StaticDesignIsItemChecked { get; set; }
        public string StaticDesignNote { get; set; }
        public Nullable<DateTime> StaticDesignScheduleDate { get; set; }
        public Nullable<DateTime> StaticDesignScheduleTime { get; set; }
        public Nullable<DateTime> StaticDesignScheduleEndTime { get; set; }

    }

    public class CustomerFranchiseeDistributionModel
    {
        public int RowNo { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }

        public string Main1Address1 { get; set; }
        public string Main1Address2 { get; set; }
        public string Main1City { get; set; }
        public string Main1StateName { get; set; }
        public string Main1PostalCode { get; set; }
         
        public string Main2Address1 { get; set; }
        public string Main2Address2 { get; set; }
        public string Main2City { get; set; }
        public string Main2StateName { get; set; }
        public string Main2PostalCode { get; set; }
         
        public string Phone1Phone { get; set; }
        public string Phone1PhoneExt { get; set; }
        public string Phone1Fax { get; set; }

        public string Phone2Phone { get; set; }
        public string Phone2PhoneExt { get; set; }
        public string Phone2Fax { get; set; }

        public string EmailAddress { get; set; }

        public int CustomerId { get; set; }
        public int DistributionId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
         
    }
    public class DistributionFeesDetailModel
    {             
        public int DistributionFeesId { get; set; }
        public Nullable<int> DistributionId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Name { get; set; }
        public string Rate { get; set; }
    }

    public class CustomerServiceScheduleDataModel
    {
        public int CRM_SalesTerriAssignmentId { get; set; }
        public int CRM_TerritoryId { get; set; }
        public int UserId { get; set; }
        public string ScheduleName { get; set; }
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CRM_ScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAllDay { get; set; }
        public int RegionId { get; set; }
        public bool IsActive { get; set; }
        public int ClassId { get; set; }
        public int TypeListId { get; set; }
        public string ColorCode { get; set; }
        public string CustomerName { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public bool IsFromOutlook { get; set; }
        public string OutlookAppointmentGuid { get; set; }
        public Nullable<DateTime> OutlookSyncDate { get; set; }

        public string Location { get; set; }
        public int CRM_ScheduleTypeId { get; set; }

        public int CRM_StageStatusType { get; set; }
        public int AuthUserLoginId { get; set; }
        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public bool IsSameAddress { get; set; }
        public string RegionName { get; set; }
        public string CustomerNo { get; set; }
        public string CRM_AccountId { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string Contactperson { get; set; }
        public string ContactName { get; set; }
        public string StartTimeString { get; set; }
        public string EndTimeString { get; set; }
        public string StartDateString { get; set; }
        public string EndDateString { get; set; }

    }
    public class CRM_PurposeTypeModel
    {
        public int CRM_PurposeTypeId { get; set; }
        public string Name { get; set; }
        public Nullable<int> CRMStageStatusId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> CRMInitialCommunication { get; set; }
        public Nullable<bool> CRMLeadGeneration { get; set; }
        public Nullable<bool> CRMPotentialInquary { get; set; }
        public Nullable<bool> CRMFollowup { get; set; }
        public Nullable<bool> CRMFranchiseFollowUp { get; set; }
        public Nullable<bool> PdAppointment { get; set; }
        public Nullable<bool> CRMBidding { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public string ColorCode { get; set; }
        public int StatusListId { get; set; }
    }


    public enum PurposeForDiffForm {
        CustomerServiceAndOperations=75,
        CustomerSales=76,
        FranchiseeSales=77
    }

    public class CRM_ScheduleTypeModel {
        public int CRM_ScheduleTypeId { get; set; }
        public Nullable<int> Type { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ColorCode { get; set; }

    }

}
