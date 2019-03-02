using JKApi.WebAPI.Models;
using JKViewModels.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Dtos
{
    public class CRMNewLeadRequestModel : IModelBase
    {
        public string companyname { get; set; }
        public string contactname { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
        public int? providerType { get; set; }

        public int userId { get; set; }

        public int regionId { get; set; }
        public string Note { get; set; }
    }

    public class CRMNewLeadResponseModel : IModelBase
    {
    }


    public class CRMAccountCustomerDetailResponseModel : IModelBase
    {
        public int CRM_AccountCustomerDetailId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddressLine1 { get; set; }
        public string CompanyAddressLine2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCounty { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZipCode { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyFaxNumber { get; set; }
        public string CompanyWebSite { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NumberOfLocations { get; set; }
        public int BudgetAmount { get; set; }
        public int AccountTypeListId { get; set; }
        public int CRM_AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string SalesVolume { get; set; }
        public string LineofBusiness { get; set; }
        public string SqFt { get; set; }
        public StageType Stage { get; set; }
        public StageStatusType StageStatus { get; set; }
        public AccountSourceProvider ProviderSource { get; set; }
        public AccountProviderType ProviderType { get; set; }
        public CallResultType CallResultType { get; set; }
        public int CRM_NoteTypeId { get; set; }
        public int CRM_SalePossibilityTypeId { get; set; }
        public DateTime? ContractExpire { get; set; }
        public string StageStatusName { get; set; }
        public bool AMorPM { get; set; }
        public string ContactPerson { get; set; }
        public bool? InterestedInPerposal { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public int CRM_CallResultId { get; set; }
        public string SpokeWith { get; set; }
        public DateTime? Callback { get; set; }
        public int Purpose { get; set; }
        public string GeneralNote { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class CRMAccountCustomerDetailDropDownResponseModel : IModelBase
    {
        public List<DropDownResponseModel> UserList { get; set; }
        public List<DropDownResponseModel> LeadStageStatusList { get; set; }
        public List<DropDownResponseModel> AccountTypeList { get; set; }
        public List<DropDownResponseModel> ProviderTypeList { get; set; }
        public List<DropDownResponseModel> ProviderSourceList { get; set; }
        public List<DropDownResponseModel> CallResultList { get; set; }
        public List<DropDownResponseModel> NoteType { get; set; }

        public List<DropDownResponseModel> SalePossibilityType { get; set; }
        public List<DropDownResponseModel> State { get; set; }

        public List<DropDownResponseModel> PurposeType { get; set; }

        public List<DropDownResponseModel> ServiceTypeList { get; set; }

        public List<DropDownResponseModel> FrequencyListModel { get; set; }

        public List<DropDownResponseModel> CleanFrequencyListModel { get; set; }

        public List<string> CallTime { get; set; }

    }

    public class SaveCustomerContactInfoRequestModel : IModelBase
    {
        public int CRM_AccountId { get; set; }

        public int CRM_AccountCustomerDetailId { get; set; }

        public string ContactName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public int UserId { get; set; }

        public string CompanyName { get; set; }

        public string Title { get; set; }

        public string CompanyAddressLine1 { get; set; }

        public string CompanyAddressLine2 { get; set; }

        public string CompanyCity { get; set; }

        public string CompanyCounty { get; set; }

        public string CompanyState { get; set; }

        public string CompanyZipCode { get; set; }

        public int RegionId { get; set; }
    }

    public class SaveCustomerAccounttInfoRequestModel : IModelBase
    {
        public int CRM_AccountId { get; set; }

        public int CRM_AccountCustomerDetailId { get; set; }

        public int IndustryType { get; set; }

        public int? NumberOfLocations { get; set; }

        public string SqFt { get; set; }

        public string ContractExpire { get; set; }

        public int? SalesPossibility { get; set; }

        public decimal? BudgetAmount { get; set; }

        public int UserId { get; set; }

        public int ProviderType { get; set; }

        public int StageStatus { get; set; }

        public int ProviderSource { get; set; }

        public int RegionId { get; set; }
    }

    public class SaveCustomerAccountSummaryRequestModel : IModelBase
    {

        public int CRM_AccountCustomerDetailId { get; set; }

        public int CRM_AccountId { get; set; }

        public int IndustryType { get; set; }

        public int? NumberOfLocations { get; set; }

        public string SqFt { get; set; }

        public string ContractExpire { get; set; }

        public int? SalesPossibility { get; set; }

        public int? NoteType { get; set; }

        public string CallLogNote { get; set; }

        public decimal? BudgetAmount { get; set; }

        public int? CallResult { get; set; }

        public string SpokeWith { get; set; }

        public string CallLogDate { get; set; }

        public int UserId { get; set; }

        public int? StageStatus { get; set; }

        public int InterestedInProposal { get; set; }

        public string ScheduleStartDate { get; set; }

        public string ScheduleEndDate { get; set; }

        public string Note { get; set; }

        public int? PurposeId { get; set; }

        public int RegionId { get; set; }

        public string Purpose { get; set; }

        public string ContactPerson { get; set; }

        public string Asignee { get; set; }

        public int ProviderType { get; set; }

        public string CallLogTime { get; set; }

        public string ScheduleStartTime { get; set; }

        public string ScheduleEndTime { get; set; }

        public int? ProviderSource { get; set; }
    }

    public class CustomerPerndingResponseModel : IModelBase
    {

        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        public string RegionName { get; set; }
        public string Address { get; set; }
        public string StateName { get; set; }

        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Amount { get; set; }

        public string Phone { get; set; }

        public string StatusName { get; set; }

        public string AcTypeListName { get; set; }
        public string CreatedBy { get; set; }

    }

    public class SaveCustomerContactInfoPotentialRequestModel
    {
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountId { get; set; }

        public string ContactName { get; set; }

        public string Title { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string CompanyName { get; set; }

        public int IndustryType { get; set; }

        public int NumberOfLocations { get; set; }

        public string CompanyPhoneNumber { get; set; }

        public string CompanyFaxNumber { get; set; }

        public string CompanyEmailAddress { get; set; }

        public string CompnayWebSite { get; set; }

        public string CompanyAddressLine1 { get; set; }

        public string CompanyAddressLine2 { get; set; }

        public string CompanyCity { get; set; }

        public string CompanyCounty { get; set; }

        public string CompanyState { get; set; }

        public string CompanyZipCode { get; set; }

        public decimal Sqft { get; set; }

        public string LineofBusiness { get; set; }
            
        public string SalesVolume { get; set; }

        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

    public class CreateFvPresentationSummaryDataRequestModel
    {

        public int CRM_AccountId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public string MeasureContactPerson { get; set; }
        public decimal? MeasureFacility { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? Frequency { get; set; }
        public int CleaningDay { get; set; }
        public int? ServiceType { get; set; }
        public int? ServiceLevel { get; set; }
        public int CleanFrequency { get; set; }
        public int? CleanTime { get; set; }
        public decimal? Budget { get; set; }
        public string Note { get; set; }
        public bool? Mon { get; set; }
        public bool? Tue { get; set; }
        public bool? Wed { get; set; }

        public bool? Thu { get; set; }
        public bool? Fri { get; set; }
        public bool? Sat { get; set; }

        public bool? Sun { get; set; }

        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

    public class CreateBiddingSummaryDataRequestModel
    {
        public int CRM_AccountId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public bool? IsAnalysisWorkBook { get; set; }
        public string MeetWithPerson { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleStartTime { get; set; }
        public string ScheduleEndDate { get; set; }
        public string ScheduleEndTime { get; set; }
        public int PruposeId { get; set; }
        public string Prupose { get; set; }
        public decimal? MonthlyPrice { get; set; }

        public int PriceApproved { get; set; }
        public decimal IncludePrice { get; set; }
        public string Note { get; set; }

        public bool? IsBidSheet { get; set; }
        public bool? IsCancellation { get; set; }
        
        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

    public class CreatePdAppointmentSummaryDataRequestModel
    {
        public int CRM_AccountId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }

        public string DMeetWithPerson { get; set; }

        public string DContactPhone { get; set; }

        public string DContactEmail { get; set; }

        public bool MeetWithDecisionMaker { get; set; }

        public bool PresentProposal { get; set; }

        public string OverComeObjection { get; set; }

        public string CallBackStartDate { get; set; }

        public string CallBackStartTime { get; set; }
        
        public string CallBackEndDate { get; set; }
        
        public string CallBackEndTime { get; set; }

        public int? CallBack_PurposeId { get; set; }

        public string CallBack_Purpose { get; set; }

        public string Note { get; set; }

        public string Comment { get; set; }
        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

    public class CreateFollowUpSummaryDataRequestModel
    {
        public int CRM_AccountId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }

        public int CloseType { get; set; }

        public string Note { get; set; }

        public int PurposeAgainId { get; set; }

        public string PurposeAgain { get; set; }

        public string ScheduleStartDate { get; set; }

        public string ScheduleStartTime { get; set; }

        public string ScheduleEndDate { get; set; }

        public string ScheduleEndTime { get; set; }

        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

    public class CreateCloseSummaryDataRequestModel
    {

        public int CRM_AccountCustomerDetailId { get; set; }
        public HttpPostedFileBase file1 { get; set; }
        public HttpPostedFileBase file2 { get; set; }
        public HttpPostedFileBase file3 { get; set; }
        public HttpPostedFileBase file4 { get; set; }
        public HttpPostedFileBase file5 { get; set; }
        public HttpPostedFileBase file6 { get; set; }
        public HttpPostedFileBase file7 { get; set; }
        public HttpPostedFileBase file8 { get; set; }
        public HttpPostedFileBase file9 { get; set; }
        public HttpPostedFileBase file10 { get; set; }
        public HttpPostedFileBase file11 { get; set; }
        public HttpPostedFileBase file12 { get; set; }
        public string[] CleaningDay { get; set; }
        public int CRM_AccountId { get; set; }

        public bool? HaveBackgroundCheck { get; set; }
        public bool? SignedAgreement { get; set; }
        public bool? DocumentSalesCRM { get; set; }
        public bool? NotifyAccountPlacement { get; set; }
        public string Note { get; set; }

        public bool? Mon { get; set; }
        public bool? Fri { get; set; }
        public bool? Sat { get; set; }
        public bool? Sun { get; set; }
        public bool? Wed { get; set; }
        public bool? Thu { get; set; }
        public bool? Tue { get; set; }
        public decimal? PropAmount { get; set; }
        public int? InitialClean { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? InitialCleanAmount { get; set; }
        public DateTime? SignDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ContractTerm { get; set; }
        public int? ServiceType { get; set; }
        public int? BillingFrequency { get; set; }
        public int? CleanTime { get; set; }
        public int? CleanFrequency { get; set; }

        public int UserId { get; set; }

        public int RegionId { get; set; }

    }

}