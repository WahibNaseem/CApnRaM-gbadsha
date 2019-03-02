using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Outlook;
using JKApi.WebAPI.Common;
using Microsoft.Web.Http;
using JKApi.Service.ServiceContract.Customer;
using System.Linq;
using JKApi.WebAPI.Dtos;
using JKApi.Data.DAL;
using JKViewModels.CRM;
using AutoMapper;
using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;
using JKViewModels;
using JKApi.Core.Email;
using JKApi.Service;
using System.ComponentModel;
using System.IO;
using JKApi.Core;
using static JKApi.WebAPI.CRMEnums;
using System.Configuration;
using JKViewModels.Customer;
using JKViewModels.Common;
using Newtonsoft.Json;
using System.Net;
using JKApi.Service.Helper.Extension;
using System.Web;
using JKApi.WebAPI.Dtos.CRM;
using JKApi.WebAPI.Filters;
using JKViewModels.CRM.CRMSPModels;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/crm")]
    [Authorized]
    public class CRMController : BaseApiController
    {
        private readonly ICRM_Service _crmService;
        private readonly IOutlookService _outlookService;
        private readonly ICustomerService _customerService;
        private readonly MailService _mailService;
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        public jkDatabaseEntities jkEntityModel = jkDatabaseEntities.Instance;

        // ======================================================================================
        #region CRMController > Constructor
        // ======================================================================================

        /// <summary>
        /// Construct with dependencies.
        /// </summary>
        public CRMController(ICRM_Service crmService, IOutlookService outlookService, ICustomerService customerService, IUserService userService, ICommonService commonService)
        {
            _crmService = crmService;
            _outlookService = outlookService;
            _customerService = customerService;
            _mailService = new MailService();
            _userService = userService;
            _commonService = commonService;
        }

        #endregion
        // ======================================================================================
        #region CRMController > API Calls
        // ======================================================================================

        [Route("syncscheduleswithoutlook/{userId}")]
        [HttpGet]
        [ResponseType(typeof(OutlookSyncResult))]
        public IHttpActionResult SyncSchedulesWithOutlook(int userid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _outlookService.SyncAppointments(userid);
                return ResponseSuccessResult(result);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("synccontactswithoutlook/{userId}")]
        [HttpGet]
        [ResponseType(typeof(OutlookSyncResult))]
        public IHttpActionResult SyncContactsWithOutlook(int userid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _outlookService.SyncContacts(userid);
                return ResponseSuccessResult(result);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("subscribetooutlooknotifications/{userId}")]
        [HttpGet]
        [ResponseType(typeof(OutlookSyncResult))]
        public IHttpActionResult SubscribeToOutlookNotifications(int userid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _outlookService.Subscribe(userid);
                return ResponseSuccessResult(result);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("CheckCompanynameExits")]
        [HttpPost]
        [ResponseType(typeof(CheckCompanynameExitsResponseModel))]
        public IHttpActionResult CheckCompanynameExits(CheckCompanynameExitsRequestModel requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _customerService.CheckCustomerNamePhoneIsExist(requestDto.CompanyName, requestDto.Phone, (string)requestDto.SelectedRegionId);

                if (result == -1)
                    return ResponseSuccessResult(result, ApiException.ErrorCode.CustomError, "Company name already in system so please contact to administrator.");
                else if (result == -2)
                    return ResponseSuccessResult(result, ApiException.ErrorCode.CustomError, "Phone number already in system so please contact to administrator.");
                else
                    return ResponseSuccessResult("created");
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetSourceTypeList")]
        [HttpGet]
        public IHttpActionResult GetSourceTypeList()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var providerTypes = _crmService.GetAll_CRM_ProviderType().Select(o => new
                {
                    Id = o.CRM_ProviderTypeId,
                    Value = o.Name != null ? o.Name : "N/A"
                }).ToList();

                return ResponseSuccessResult(providerTypes);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetAccountTypeList")]
        [HttpGet]
        public IHttpActionResult GetAccountTypeList()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _customerService.GetAccountTypeList().Select(o => new
                {
                    Id = o.AccountTypeListId,
                    Value = o.Name
                }).ToList();
                return ResponseSuccessResult(result);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("CRMNewLeadCustomer")]
        [HttpPost]
        [ResponseType(typeof(CRM_Account))]
        public IHttpActionResult CRMNewLeadCustomer(CRMNewLeadRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomer = _crmService.AddNewCustomerAccount(
                    model.companyname,
                    model.contactname,
                    model.phoneNumber,
                    model.emailAddress,
                    Convert.ToInt32(model.providerType),
                   _crmService.GetCRM_ProviderSourceIndex("inhouse"), JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants.Key_Customer, model.userId, model.regionId, model.Note
                );

                return ResponseSuccessResult(accountCustomer);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetLeadCustomerById")]
        [HttpGet]
        [ResponseType(typeof(CRMAccountCustomerDetailResponseModel))]
        public IHttpActionResult GetLeadCustomerById(int Id)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomerDetailViewModel = _getLeadCustomerById(Id);
                return ResponseSuccessResult(accountCustomerDetailViewModel);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetLeadCustomerDropDown")]
        [HttpGet]
        [ResponseType(typeof(CRMAccountCustomerDetailDropDownResponseModel))]
        public IHttpActionResult GetLeadCustomerDropDown()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomerDetailViewModel = new CRMAccountCustomerDetailDropDownResponseModel();

                accountCustomerDetailViewModel.UserList = _crmService.Get_AuthUserLogin(7, null).Select(o => new DropDownResponseModel
                {
                    Id = o.UserId.ToString(),
                    Value = o.FirstName
                }).ToList(); ; //CRM Sales

                accountCustomerDetailViewModel.LeadStageStatusList = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_Potential ||
                    x.Name == Constants.Key_QualifiedLead ||
                    x.Name == Constants.Key_CallBack ||
                    x.Name == Constants.Key_NewLead).Select(o => new DropDownResponseModel
                    {
                        Id = o.Type.ToString(),
                        Value = o.Name
                    }).ToList();

                accountCustomerDetailViewModel.AccountTypeList = _customerService.GetAccountTypeList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.AccountTypeListId.ToString(),
                    Value = o.Name
                }).ToList(); ;

                accountCustomerDetailViewModel.ProviderTypeList = _crmService.GetAll_CRM_ProviderType().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.ProviderSourceList = _crmService.GetAll_CRM_ProviderSource().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderSourceId.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.CallResultList = _crmService.GetAll_CRM_CallResult().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_CallResultId.ToString(),
                    Value = o.Name
                }).ToList();


                accountCustomerDetailViewModel.NoteType = _crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_NoteTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.SalePossibilityType = _crmService.GetAll_CRM_SalePossibilityType().ToList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_SalePossibilityTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.State = _customerService.GetStateList().Select(o => new DropDownResponseModel
                {
                    Id = o.abbr.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.PurposeType = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_PurposeTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                return ResponseSuccessResult(accountCustomerDetailViewModel);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("updatecustomercontactinfo")]
        [HttpPut]
        [ResponseType(typeof(CRMAccountCustomerDetailResponseModel))]
        public IHttpActionResult UpdateCustomerContactInfo(SaveCustomerContactInfoRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                CRMAccountCustomerDetailResponseModel customerAccount = null;
                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(model.CRM_AccountCustomerDetailId);
                if (accountcustomer != null && customerAccountDetail != null)
                {
                    accountcustomer.ContactName = model.ContactName;
                    accountcustomer.PhoneNumber = model.PhoneNumber;
                    accountcustomer.EmailAddress = model.EmailAddress;
                    accountcustomer.ModifiedBy = model.UserId;
                    customerAccountDetail.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                    customerAccountDetail.CRM_AccountId = Convert.ToInt32(model.CRM_AccountId);
                    customerAccountDetail.CompanyName = model.CompanyName;
                    customerAccountDetail.Title = model.Title;
                    customerAccountDetail.CompanyAddressLine1 = model.CompanyAddressLine1;
                    customerAccountDetail.CompanyAddressLine2 = model.CompanyAddressLine2;
                    customerAccountDetail.CompanyCity = model.CompanyCity;
                    customerAccountDetail.CompanyState = model.CompanyState;
                    customerAccountDetail.CompanyZipCode = model.CompanyZipCode;
                    customerAccountDetail.ModifiedBy = model.UserId;

                    _crmService.SaveCRM_Account(accountcustomer);
                    _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);

                    customerAccount = _getLeadCustomerById(model.CRM_AccountId);
                }

                return ResponseSuccessResult(customerAccount);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("updatecustomeraccountsummary")]
        [HttpPut]
        [ResponseType(typeof(CRMAccountCustomerDetailResponseModel))]
        public IHttpActionResult UpdateCustomerAccountSummary(SaveCustomerAccounttInfoRequestModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                CRMAccountCustomerDetailResponseModel customerAccount = null;
                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(model.CRM_AccountCustomerDetailId); 
                if (customerAccountDetail != null && accountcustomer != null)
                {
                    customerAccountDetail.AccountTypeListId = model.IndustryType;
                    customerAccountDetail.NumberOfLocations = model.NumberOfLocations ?? 0;
                    customerAccountDetail.SqFt = model.SqFt;
                    if (model.ContractExpire != "")
                        customerAccountDetail.ContractExpire = Convert.ToDateTime(model.ContractExpire);
                    customerAccountDetail.CRM_SalePossibilityTypeId = model.SalesPossibility ?? 0;
                    customerAccountDetail.BudgetAmount = model.BudgetAmount ?? 0;
                    customerAccountDetail.ModifiedBy = model.UserId;
                    accountcustomer.ProviderType = model.ProviderType;
                    accountcustomer.StageStatus = model.StageStatus;
                    accountcustomer.ProviderSource = model.ProviderSource;
                    accountcustomer.ModifiedBy = model.UserId;
                    accountcustomer.ModifiedBy = model.RegionId;

                    _crmService.SaveCRM_Account(accountcustomer);
                    _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);

                    customerAccount = _getLeadCustomerById(model.CRM_AccountId);
                }

                return ResponseSuccessResult(customerAccount);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("savecustomeraccountsummary")]
        [HttpPut]
        [ResponseType(typeof(CRMAccountCustomerDetailResponseModel))]
        public IHttpActionResult SaveCustomerAccountSummary(SaveCustomerAccountSummaryRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                CRMAccountCustomerDetailResponseModel customerAccount = null;
                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(model.CRM_AccountCustomerDetailId);
                
                if (customerAccountDetail != null && accountcustomer != null)
                {
                    customerAccountDetail.AccountTypeListId = model.IndustryType;
                    customerAccountDetail.NumberOfLocations = model.NumberOfLocations != null ? model.NumberOfLocations : 0;
                    customerAccountDetail.SqFt = model.SqFt;
                    if (model.ContractExpire != "")
                        customerAccountDetail.ContractExpire = Convert.ToDateTime(model.ContractExpire);
                    customerAccountDetail.CRM_SalePossibilityTypeId = model.SalesPossibility != null ? Convert.ToInt32(model.SalesPossibility) : 0;
                    customerAccountDetail.CRM_NoteTypeId = model.NoteType != null ? Convert.ToInt32(model.NoteType) : 0;
                    customerAccountDetail.GeneralNote = model.CallLogNote;
                    customerAccountDetail.BudgetAmount = model.BudgetAmount != null ? Convert.ToDecimal(model.BudgetAmount) : 0;
                    customerAccountDetail.CRM_CallResultId = model.CallResult != null ? Convert.ToInt32(model.CallResult) : (int?)null;
                    customerAccountDetail.SpokeWith = model.SpokeWith;

                    if (model.CallLogDate != "")
                    {
                        customerAccountDetail.Callback = Convert.ToDateTime(model.CallLogDate + " " + model.CallLogTime);
                    }
                    customerAccountDetail.ModifiedBy = model.UserId;

                    #region Initial Communication Code

                    /*Check If TeleMarketer Select Potential To Make Initial Communication*/
                    if (Convert.ToInt32(model.StageStatus) == (int)StageStatusType.Potential)
                    {
                        /*Get the Initl Communication if there is any*/
                        var customerInitialCommunicate = _crmService.Get_InitialCommunication_ByAccountCustomerDetailById(customerAccountDetail.CRM_AccountCustomerDetailId);

                        /*Check if InitialCommunication Not Exist  */
                        string scheduleStartEndDateTime = "";
                        if (customerInitialCommunicate != null)
                        {
                            customerInitialCommunicate.ContactPerson = model.ContactPerson;
                            if (model.InterestedInProposal == 1)
                                customerInitialCommunicate.InterestedInPerposal = true;
                            else if (model.InterestedInProposal == 0)
                                customerInitialCommunicate.InterestedInPerposal = false;

                            customerInitialCommunicate.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.Now;


                            customerInitialCommunicate.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;
                            customerInitialCommunicate.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                            customerInitialCommunicate.Note = model.Note;
                            customerInitialCommunicate.PURPOSE = Convert.ToInt32(model.PurposeId);
                            customerInitialCommunicate.RegionId = model.RegionId;
                            customerInitialCommunicate.ModifiedBy = model.UserId;

                            var initialcommunication = _crmService.SaveCRM_InitialCommunication(customerInitialCommunicate);


                            /* Save In Schedule */
                            var scheduleentity = _crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == initialcommunication.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.IntialCommunication);
                            if (scheduleentity != null)
                            {
                                scheduleentity.ClassId = initialcommunication.CRM_AccountCustomerDetailId;
                                scheduleentity.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.UtcNow;
                                scheduleentity.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;

                                scheduleStartEndDateTime = scheduleentity.StartDate + " - " + scheduleentity.EndDate;

                                scheduleentity.Title = accountcustomer.CRM_AccountId + " - " + "FV" + " - " + model.Purpose + " - " + accountcustomer.ContactName + " - " + initialcommunication.ContactPerson + " - " + customerAccountDetail.CompanyPhoneNumber;

                                scheduleentity.Location = customerAccountDetail.CompanyAddressLine1;
                                scheduleentity.AuthUserLoginId = model.Asignee != "" ? Convert.ToInt32(model.Asignee) : 0;
                                scheduleentity.Description = customerAccountDetail.CompanyName + "," + accountcustomer.ContactName + "," + accountcustomer.PhoneNumber;
                                scheduleentity.RegionId = model.RegionId;

                                scheduleentity.ModifiedBy = model.UserId;
                                scheduleentity.PurposeId = Convert.ToInt32(model.PurposeId);
                                scheduleentity.Purpose = model.Purpose;
                                scheduleentity.CRM_StageStatusType = (int)StageStatusType.FvPresentation;

                                scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                                var schedule = _crmService.SaveCRM_Schedule(scheduleentity);
                            }
                        }
                        else
                        {
                            var initial = new CRMInitialCommunicationViewModel();
                            initial.ContactPerson = model.ContactPerson;
                            if (model.InterestedInProposal == 1)
                                initial.InterestedInPerposal = true;
                            else if (model.InterestedInProposal == 0)
                                initial.InterestedInPerposal = false;

                            initial.PURPOSE = Convert.ToInt32(model.PurposeId);
                            initial.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.Now;
                            initial.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;

                            initial.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                            initial.Note = model.Note;
                            initial.RegionId = model.RegionId;
                            initial.CreatedBy = model.UserId;
                            initial.RegionId = model.RegionId;

                            var initialcommunication = _crmService.SaveCRM_InitialCommunication(_crmInitialViewModelToEntity(initial));

                            /*Save in Schedule */
                            var scheduleentity = new CRMScheduleViewModel();

                            scheduleentity.ClassId = initialcommunication.CRM_AccountCustomerDetailId;
                            scheduleentity.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.UtcNow;


                            scheduleentity.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;
                            scheduleStartEndDateTime = scheduleentity.StartDate + " - " + scheduleentity.EndDate;

                            scheduleentity.Title = accountcustomer.CRM_AccountId + " - " + "FV" + " - " + model.Purpose + " - " + accountcustomer.ContactName + " - " + initialcommunication.ContactPerson + " - " + customerAccountDetail.CompanyPhoneNumber;
                            scheduleentity.Location = customerAccountDetail.CompanyAddressLine1;
                            scheduleentity.AuthUserLoginId = model.Asignee != "" ? Convert.ToInt32(model.Asignee) : 0;
                            scheduleentity.Description = customerAccountDetail.CompanyName + "," + accountcustomer.ContactName + "," + accountcustomer.PhoneNumber;
                            scheduleentity.RegionId = model.RegionId;
                            scheduleentity.ModifiedBy = model.UserId;
                            scheduleentity.PurposeId = Convert.ToInt32(model.PurposeId);
                            scheduleentity.Purpose = model.Purpose;
                            scheduleentity.CRM_StageStatusType = (int)StageStatusType.FvPresentation;
                            scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                            var schedule = _crmService.SaveCRM_Schedule(_crmScheduleViewModelToEntity(scheduleentity));

                        }
                        if (model.Asignee != "")
                        {
                            int AsigneeId = Convert.ToInt32(model.Asignee);
                            UserViewModel oUserViewModel = _userService.GetUserDetail(AsigneeId);
                            if (!string.IsNullOrEmpty(oUserViewModel.Email))
                            {
                                int PurposeId = Convert.ToInt32(model.PurposeId);
                                PotentialLeadEmailTemplateModel emailModel = new PotentialLeadEmailTemplateModel
                                {
                                    AccountType = _customerService.GetAccountTypeListById(Convert.ToInt32(model.IndustryType))?.Name,
                                    BudgetAmount = "$" + model.BudgetAmount,
                                    CompanyName = customerAccountDetail.CompanyName,
                                    ContactPhone = customerAccountDetail.CompanyPhoneNumber,
                                    LeadNo = _crmService.GetCRM_ProviderTypeName(Convert.ToInt32(model.ProviderType)),
                                    Note = model.Note,
                                    Purpose = _getEnumValuesAndDescriptions<CRMEnums.SchedulePurpose>()
                                    .FirstOrDefault(x => x.Value == PurposeId).Key,
                                    Schedule = scheduleStartEndDateTime,
                                    UserName = oUserViewModel.FirstName + " " + oUserViewModel.LastName
                                };

                                //var html = RenderPartialViewToString(this.ControllerContext, "PotentialLeadEmailTemplate", emailModel);
                                // Will Update Email Template for CRM
                                _mailService.SendEmailAsync(oUserViewModel.Email, "New Lead Assigned", "New Lead Assigned");
                            }
                        }
                    }
                    #endregion

                    /* Assign lead to specific sales person if Select Potential and Assignee is not emptly */
                    if (Convert.ToInt32(model.StageStatus) == (int)StageStatusType.Potential && model.Asignee != "")
                    {
                        accountcustomer.StageStatus = (int)StageStatusType.FvPresentation;
                        accountcustomer.Stage = (int)StageType.Potential;
                        accountcustomer.AssigneeId = int.Parse(model.Asignee);
                    }
                    else
                    {
                        if (model.CallResult == 12 || model.CallResult == 14 || model.CallResult == 7)
                        {
                            accountcustomer.StageStatus = (int)StageStatusType.UnqualifiedLead;
                        }
                        else if (model.CallResult == 13)
                        {
                            accountcustomer.StageStatus = (int)StageStatusType.CallBack;
                        }
                        else
                        {
                            accountcustomer.StageStatus = Convert.ToInt32(model.StageStatus);
                        }
                    }

                    accountcustomer.ProviderType = Convert.ToInt32(model.ProviderType);
                    accountcustomer.ProviderSource = Convert.ToInt32(model.ProviderSource);
                    accountcustomer.ModifiedBy = model.UserId;

                    _crmService.SaveCRM_Account(accountcustomer);
                    _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);

                    if (model.Asignee != "")
                    {
                        int AssignID = int.Parse(model.Asignee);

                        #region Email send function According to admin configuration

                        //Get Feature Type Id by Feature Name
                        var Leads_Potential_Assignee = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.Leads_Potential_Assignee.ToString().Replace("_", " ")).FirstOrDefault();

                        if (Leads_Potential_Assignee != null && Leads_Potential_Assignee.FeatureTypeId > 0)
                        {
                            //Get Feature Type Email Id by Feature Type Id
                            var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == Leads_Potential_Assignee.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                            if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                            {
                                //Get Feature Email Body By Message Name Or Tempalte Name 
                                var messageBody = jkEntityModel.MailMessageTemplates.Where(x => x.MailMessageTemplateId == messageDetails.MailMessageTemplateId).FirstOrDefault();
                                //MailMessageTemplateModel objItem = _commonService.GetEmailTemplate(MessageNameModel.FranchiseSecion1.ToString());
                                if (messageBody != null && messageBody.MailMessageTemplateId > 0)
                                {
                                    string MessageBody = messageBody.MessageBody;
                                    string Subject = messageBody.Subject;

                                    if (messageDetails.FromEmail != null && messageDetails.ToEmailId != null)
                                    {
                                        _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, MessageBody, Subject);
                                    }

                                    var UserEmailId = jkEntityModel.AuthUserLogins.Where(x => x.UserId == AssignID).FirstOrDefault();

                                    if (UserEmailId != null && UserEmailId != null && !string.IsNullOrWhiteSpace(UserEmailId.Email))
                                    {
                                        _mailService.SendEmailAsync(UserEmailId.Email, MessageBody, Subject);
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    customerAccount = _getLeadCustomerById(model.CRM_AccountId);
                }

                return ResponseSuccessResult(customerAccount);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }

        }

        [Route("newleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult NewLeadList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.NewLead);
        }

        [Route("qualifiedleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult QualifiedLeadList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.QualifiedLead);
        }

        [Route("unqualifiedleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult UnQualifiedLeadList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.UnqualifiedLead);
        }

        [Route("potentialleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult PotentialList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.PotentialLead);
        }

        [Route("closeleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult CloseLeadList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.CloseLead);
        }

        [Route("callbackleadlist")]
        [HttpPost]
        [ResponseType(typeof(List<CRMPotentialCustomerViewModel>))]
        public IHttpActionResult CallBackLeadList(CrmPotentialCustomerRequestDto requestDto)
        {
            return _getPotentialCustomerList(requestDto, CrmFilterChoice.CallbackLead);
        }

        [Route("CustomerPendingList")]
        [HttpGet]
        [ResponseType(typeof(List<CustomerPerndingResponseModel>))]
        public IHttpActionResult CustomerPendingList(int UserId, int RegionId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var customers = _customerService.GetCustomerSearchList(contactTypeList, "38", RegionId.ToString());
                var result = (from f in customers
                              select new CustomerPerndingResponseModel
                              {
                                  CustomerId = f.CustomerId,
                                  CustomerNo = f.CustomerNo,
                                  CustomerName = f.CustomerName,
                                  Address = f.Address,
                                  StateName = f.StateName,
                                  City = f.City,
                                  PostalCode = f.PostalCode,
                                  Amount = string.Format("{0:c}", f.Amount),
                                  Phone = f.Phone != null ? _formatUsPhoneNumber(f.Phone) : string.Empty,
                                  RegionName = f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  CreatedBy = f.CreatedBy
                              }).ToList();

                return ResponseSuccessResult(result);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("OprationsOfferingListData")]
        [HttpGet]
        [ResponseType(typeof(List<CRMAccountOfferingListViewModel>))]
        public IHttpActionResult OprationsOfferingListData(int UserId, int RegionId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _customerService.GetAccountOfferingData(RegionId.ToString(), "19,40,41");

                return ResponseSuccessResult(result);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("CustomerRegionAccountingList")]
        [HttpGet]
        [ResponseType(typeof(List<CustomerPerndingResponseModel>))]
        public IHttpActionResult CustomerRegionAccountingList(int UserId, int RegionId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {

                var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var customers = _customerService.GetCustomerSearchList(contactTypeList, "39,35", RegionId.ToString());
                var result = (from f in customers
                              select new CustomerPerndingResponseModel
                              {
                                  CustomerId = f.CustomerId,
                                  CustomerNo = f.CustomerNo,
                                  CustomerName = f.CustomerName,
                                  Address = f.Address,
                                  StateName = f.StateName,
                                  City = f.City,
                                  PostalCode = f.PostalCode,
                                  Amount = string.Format("{0:c}", f.Amount),
                                  Phone = f.Phone != null ? _formatUsPhoneNumber(f.Phone) : string.Empty,
                                  RegionName = f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  CreatedBy = f.CreatedBy
                              }).ToList();

                return ResponseSuccessResult(result);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }

        }

        [Route("AccountingOfferingListData")]
        [HttpGet]
        [ResponseType(typeof(List<CRMAccountOfferingListViewModel>))]
        public IHttpActionResult AccountingOfferingListData(int UserId, int RegionId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var result = _customerService.GetAccountOfferingData(RegionId.ToString(), "19,40,41");

                return ResponseSuccessResult(result);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetSchedulesForToday")]
        [HttpGet]
        [ResponseType(typeof(List<CRMAccountOfferingListViewModel>))]
        public IHttpActionResult GetSchedulesForToday(int UserId, int RegionId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();
                var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();
                var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.StartDate).Value == DateTime.Today.Date).ToList();

                if (accountSchedule != null)
                {
                    accountCustomerScheduleViewModels = _crmScheduleEntitiesToViewModels(accountSchedule);
                    foreach (var accountschedule in accountCustomerScheduleViewModels)
                    {
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = accountschedule.Title;
                        calendar.start = accountschedule.StartDate.Value.ToString("s");
                        calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.Description = accountschedule.Description;
                        calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                        calendar.IsAllDay = accountschedule.IsAllDay;
                        calendar.Location = accountschedule.Location;

                        calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_ScheduleTypeId);
                        accountCustomerScheduleCalendar.Add(calendar);
                    }
                }

                var data = new
                {
                    accountCustomerScheduleViewModels = accountCustomerScheduleViewModels.OrderBy(o => o.StartDate),
                    accountCustomerScheduleCalendar = accountCustomerScheduleCalendar.OrderBy(o => o.StartDate)
                };

                return ResponseSuccessResult(data);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetSalesSchedule")]
        [HttpGet]
        [ResponseType(typeof(List<CRMAccountOfferingListViewModel>))]
        public IHttpActionResult GetSalesSchedule(int UserId, string types, string users)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();
                var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

                types = types.Replace(" ", "");
                users = users.Replace(" ", "");

                var typeList = types.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var userList = users.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                userList.Add(UserId.ToString());

                var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => userList.Contains(x.CreatedBy.ToString()) || userList.Contains(x.AuthUserLoginId.ToString()) && (typeList.Contains(x.CRM_ScheduleTypeId.ToString()))).ToList();
                if (accountSchedule != null)
                {
                    accountCustomerScheduleViewModels = _crmScheduleEntitiesToViewModels(accountSchedule);
                    foreach (var accountschedule in accountCustomerScheduleViewModels)
                    {
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = accountschedule.Title;
                        calendar.start = accountschedule.StartDate.Value.ToString("s");
                        calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.Description = accountschedule.Description;
                        calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                        calendar.IsAllDay = accountschedule.IsAllDay;
                        calendar.Location = accountschedule.Location;

                        calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_StageStatusType);
                        accountCustomerScheduleCalendar.Add(calendar);
                    }
                }

                return ResponseSuccessResult(accountCustomerScheduleCalendar);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetPotentialDropDown")]
        [HttpGet]
        [ResponseType(typeof(CRMAccountCustomerDetailDropDownResponseModel))]
        public IHttpActionResult GetPotentialDropDown()
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var accountCustomerDetailViewModel = new CRMAccountCustomerDetailDropDownResponseModel();

                accountCustomerDetailViewModel.UserList = _crmService.Get_AuthUserLogin(7, null).Select(o => new DropDownResponseModel
                {
                    Id = o.UserId.ToString(),
                    Value = o.FirstName
                }).ToList(); ; //CRM Sales

                accountCustomerDetailViewModel.LeadStageStatusList = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_Potential ||
                    x.Name == Constants.Key_QualifiedLead ||
                    x.Name == Constants.Key_CallBack ||
                    x.Name == Constants.Key_NewLead).Select(o => new DropDownResponseModel
                    {
                        Id = o.Type.ToString(),
                        Value = o.Name
                    }).ToList();

                accountCustomerDetailViewModel.AccountTypeList = _customerService.GetAccountTypeList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.AccountTypeListId.ToString(),
                    Value = o.Name
                }).ToList(); ;

                accountCustomerDetailViewModel.ProviderTypeList = _crmService.GetAll_CRM_ProviderType().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.ProviderSourceList = _crmService.GetAll_CRM_ProviderSource().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderSourceId.ToString(),
                    Value = o.Name
                }).ToList();



                accountCustomerDetailViewModel.NoteType = _crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_NoteTypeId.ToString(),
                    Value = o.Name
                }).ToList();



                accountCustomerDetailViewModel.State = _customerService.GetStateList().Select(o => new DropDownResponseModel
                {
                    Id = o.abbr.ToString(),
                    Value = o.Name
                }).ToList();

                accountCustomerDetailViewModel.PurposeType = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_PurposeTypeId.ToString(),
                    Value = o.Name
                }).ToList();


                accountCustomerDetailViewModel.CallTime = _getBestTimeList().ToList();

                accountCustomerDetailViewModel.ServiceTypeList = _crmService.GetServiceTypeList().ToList().Where(o => o.TypeListId == 1 && o.contracttype == 1).ToList().OrderBy(x => x.name).Select(o => new DropDownResponseModel
                {
                    Id = o.ServiceTypeListid.ToString(),
                    Value = o.name
                }).ToList();



                accountCustomerDetailViewModel.FrequencyListModel = _crmService.GetFrequencyList().ToList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.FrequencyListId.ToString(),
                    Value = o.Name
                }).ToList();


                accountCustomerDetailViewModel.CleanFrequencyListModel = _crmService.GetCleanFrequencyList().ToList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CleanFrequencyListId.ToString(),
                    Value = o.Name
                }).ToList();




                return ResponseSuccessResult(accountCustomerDetailViewModel);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetPotentialDetail")]
        [HttpGet]
        public IHttpActionResult GetPotentialDetail(int UserId, int accountId)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {

                var accountCustomerDetailViewModel = new CRMAccountCustomerDetailViewModel();
                var accountCustomerActivityViewModels = new List<CRMActivityViewModel>();

                var accountCustomerNoteViewModels = new List<CRMNoteViewModel>();
                var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();


                var accountCustomerInitialViewModel = new CRMInitialCommunicationViewModel();
                var accountCustomerFvPresentationViewModel = new CRMFvPresentationViewModel();
                var accountCustomerBiddingViewModel = new CRMBiddingViewModel();

                var accountCustomerPdAppointmentViewModel = new CRMPdAppointmentViewModel();
                var accountCustomerFollowUpViewModel = new CRMFollowUpViewModel();
                var accountCustomerSoldViewModel = new CRMCloseViewModel();
                var pdSch = new CRMScheduleViewModel();
                var bidSch = new CRMScheduleViewModel();
                var followSch = new CRMScheduleViewModel();
                var bidContact = new CRMContactViewModel();
                var pdContact = new CRMContactViewModel();


                /*Get from Services  Account ,  AccountCustomerDetail , AccountActivity , AccountSchedule,AccountNote,AccountDocument
                 AccountInitialCommunication , Account FvPresentation AccountBidding */
                var account = _crmService.GetCRM_AccountbyId(accountId);
                //var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetailbyId(accountId);

                if (account != null)
                {

                    var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(accountId);
                    var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();


                    var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).Where(x => x.CreatedBy == UserId).ToList();
                    var accountNote = _crmService.GetCRM_Note_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
                    var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
                    var leadschedule = _crmService.GetAll_CRM_Schedule().Where(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();


                    var accountInitialCommunication = _crmService.Get_InitialCommunication_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
                    var accountFvPresentation = _crmService.Get_fvPresentation_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);

                    var accountBidding = _crmService.Get_Bidding_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
                    var accountPdAppointment = _crmService.Get_PdAppointment_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
                    var accountFollowUp = _crmService.Get_FollowUp_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
                    var accountClose = _crmService.Get_Close_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);




                    accountCustomerDetailViewModel = _oldCrmAccountCustomerEntityToViewModel(accountCustomerDetail);
                    accountCustomerDetailViewModel.FirstName = account.FirstName;
                    accountCustomerDetailViewModel.LastName = account.LastName;
                    accountCustomerDetailViewModel.MiddleInitial = account.MiddleInitial;
                    accountCustomerDetailViewModel.ContactName = account.ContactName;
                    accountCustomerDetailViewModel.PhoneNumber = account.PhoneNumber;
                    accountCustomerDetailViewModel.CompanyFaxNumber = accountCustomerDetail.CompanyFaxNumber;
                    accountCustomerDetailViewModel.EmailAddress = account.EmailAddress;

                    /* accountCustomerDetailViewModel.ScheduleDateFormat = accountCustomerDetailViewModel.ScheduleDate != null ? accountCustomerDetailViewModel.ScheduleDate.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture) : "";
                     accountCustomerDetailViewModel.ScheduleDateFormat = accountCustomerDetailViewModel.ScheduleDate != null ? accountCustomerDetailViewModel.ScheduleDate.ToString() : " ";      */
                    accountCustomerDetailViewModel.Stage = (StageType)account.Stage.Value;
                    accountCustomerDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                    accountCustomerDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                    accountCustomerDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;
                    accountCustomerDetailViewModel.SqFt = accountCustomerDetail.SqFt;
                    accountCustomerDetailViewModel.LineofBusiness = accountCustomerDetail.LineofBusiness;
                    accountCustomerDetailViewModel.SalesVolume = accountCustomerDetail.SalesVolume;

                    // Activity
                    if (accountActivity != null)
                    {
                        accountCustomerActivityViewModels = _crmActivityEntitiesToViewModels(accountActivity);
                        foreach (var activityViewModel in accountCustomerActivityViewModels)
                        {
                            activityViewModel.ActivityTypeName = (_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType));
                            if (activityViewModel.OutComeType != null)
                                activityViewModel.OutComeTypeName = (_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType));
                        }
                    }


                    //Note
                    if (accountNote != null)
                        accountCustomerNoteViewModels = _crmNoteEntitiesToViewModels(accountNote);

                    //Document
                    if (accountDocument != null)
                        accountCustomerDocumentViewModels = _crmDocumentEntitiesToViewModels(accountDocument);

                    //Initial Communication
                    if (accountInitialCommunication != null)
                        accountCustomerInitialViewModel = _crmInitialEntityToViewModel(accountInitialCommunication);
                    else
                    {
                        accountCustomerInitialViewModel = null;
                    }

                    //Fv Presentation
                    if (accountFvPresentation != null)
                        accountCustomerFvPresentationViewModel = _crmFvPresentationEntityToViewModel(accountFvPresentation);
                    else
                    {
                        accountCustomerFvPresentationViewModel = null;
                    }

                    //Bidding
                    if (accountBidding != null)
                    {
                        accountCustomerBiddingViewModel = _crmBiddingEntityToViewModel(accountBidding);
                        bidContact = _crmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_BiddingId == accountBidding.CRM_BiddingId));
                        bidSch = _crmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                    }
                    else
                    {
                        accountCustomerBiddingViewModel = null;
                    }

                    //PdAppointment
                    if (accountPdAppointment != null)
                    {
                        accountCustomerPdAppointmentViewModel = _crmPdAppointmentEntityToViewModel(accountPdAppointment);
                        pdContact = _crmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
                        pdSch = _crmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                        if (pdSch != null)
                        {
                            pdSch.PurposeId = pdSch.PurposeId;// CRMUtils.SchedulePurposIndex(pdSch.Title);
                            pdSch.Purpose = pdSch.Purpose;
                        }
                    }
                    else
                    {
                        accountCustomerPdAppointmentViewModel = null;
                    }
                    //Follow-Up
                    if (accountFollowUp != null)
                    {
                        accountCustomerFollowUpViewModel = _crmFollowupEntityToViewModel(accountFollowUp);
                        followSch = _crmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.FollowUp));
                        if (followSch != null)
                        {
                            followSch.PurposeId = followSch.PurposeId;// CRMUtils.SchedulePurposIndex(pdSch.Title);
                            followSch.Purpose = followSch.Purpose;
                        }


                    }
                    else
                    {
                        accountCustomerFollowUpViewModel = null;
                    }
                    if (accountClose != null)
                    {
                        accountCustomerSoldViewModel = _crmCloseEntityToViewModel(accountClose);
                    }
                    else
                    {
                        accountCustomerSoldViewModel = null;
                    }
                }
                else
                {
                    return ResponseSuccessResult("", ApiException.ErrorCode.CustomError, "Please enter valid account id");
                }

                var activityTable = new { sectionName = "Initial Communication", Note = accountCustomerInitialViewModel.Note, Date = accountCustomerInitialViewModel.StartDate };
                List<object> activityList = new List<object>();
                activityList.Add(activityTable);
                if (accountCustomerFvPresentationViewModel != null)
                {
                    activityList.Add(new { sectionName = "FvPresentation", Note = accountCustomerFvPresentationViewModel.Note, Date = accountCustomerInitialViewModel.StartDate });
                }

                if (accountCustomerBiddingViewModel != null)
                {
                    activityList.Add(new { sectionName = "Bidding", Note = accountCustomerBiddingViewModel.Note, Date = bidSch == null ? accountCustomerBiddingViewModel.CreatedDate : bidSch.StartDate });
                }
                if (accountCustomerPdAppointmentViewModel != null)
                {
                    activityList.Add(new { sectionName = "PdAppointment", Note = accountCustomerPdAppointmentViewModel.Note, Date = pdSch == null ? accountCustomerPdAppointmentViewModel.CreatedDate : pdSch.StartDate });
                }
                if (accountCustomerFollowUpViewModel != null)
                {
                    activityList.Add(new { sectionName = "FollowUp", Note = accountCustomerFollowUpViewModel.Note, Date = followSch == null ? accountCustomerFollowUpViewModel.CreatedDate : followSch.StartDate });
                }

                if (accountCustomerSoldViewModel != null)
                {
                    activityList.Add(new { sectionName = "PdAppointment", Note = accountCustomerSoldViewModel.Note, Date = accountCustomerSoldViewModel.StartDate });
                }



                var resultData = new
                {
                    result = accountCustomerDetailViewModel,
                    //activity = accountCustomerActivityViewModels,
                    activity = activityList,
                    note = accountCustomerNoteViewModels,
                    document = accountCustomerDocumentViewModels,
                    initial = accountCustomerInitialViewModel,
                    fvpresentation = accountCustomerFvPresentationViewModel,
                    bidding = accountCustomerBiddingViewModel,
                    bidSchedule = bidSch,
                    pdappointment = accountCustomerPdAppointmentViewModel,
                    followup = accountCustomerFollowUpViewModel,
                    sold = accountCustomerSoldViewModel,
                    contact = bidContact,
                    pdContact,
                    pdSchedule = pdSch,
                    followSchedule = followSch,
                    assigneeId = account.AssigneeId

                };
                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("PartialPotentialLoad")]
        [HttpGet]
        public IHttpActionResult PartialPotentialLoad(int UserId, int id, int accountid, int ssid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {


                var SalesPersonList = _crmService.Get_AuthUserLogin(7).Select(o => new DropDownResponseModel
                {
                    Id = o.UserId.ToString(),
                    Value = o.FirstName ?? "" + ' ' + o.LastName ?? ""
                }).ToList(); ;


                var LeadStageStatusList = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_Potential ||
                    x.Name == Constants.Key_QualifiedLead ||
                    x.Name == Constants.Key_CallBack ||
                    x.Name == Constants.Key_NewLead).Select(o => new DropDownResponseModel
                    {
                        Id = o.Type.ToString(),
                        Value = o.Name
                    }).ToList(); ;

                var AccountTypeList = _customerService.GetAccountTypeList().OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.AccountTypeListId.ToString(),
                    Value = o.Name
                }).ToList(); ;

                var ProviderTypeList = _crmService.GetAll_CRM_ProviderType().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                var ProviderSourceList = _crmService.GetAll_CRM_ProviderSource().Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_ProviderSourceId.ToString(),
                    Value = o.Name
                }).ToList();

                var BiddingPurposeTypeList = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMBidding == true).OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_PurposeTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                var FollowUpPurposeTypeList = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMFollowup == true).OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_PurposeTypeId.ToString(),
                    Value = o.Name
                }).ToList();

                var CloseType = _crmService.GetAll_CRM_CloseType().ToList().Where(x => x.CRMFollowUp == true).OrderBy(x => x.Name).Select(o => new DropDownResponseModel
                {
                    Id = o.CRM_CloseTypeId.ToString(),
                    Value = o.Name
                }).ToList();


                var State = _customerService.GetStateList().Select(o => new DropDownResponseModel
                {
                    Id = o.abbr.ToString(),
                    Value = o.Name
                }).ToList();


                var CallTime = _getBestTimeList().ToList();

                var ServiceTypeListModel = _crmService.GetServiceTypeList().Where(o => o.TypeListId == 1 && o.contracttype == 1).Select(o => new DropDownResponseModel
                {
                    Id = o.ServiceTypeListid.ToString(),
                    Value = o.name
                }).ToList();

                var FrequencyListModel = _crmService.GetFrequencyList().Select(o => new DropDownResponseModel
                {
                    Id = o.FrequencyListId.ToString(),
                    Value = o.Name
                }).ToList();


                var CleanFrequencyListModel = _crmService.GetCleanFrequencyList().Select(o => new DropDownResponseModel
                {
                    Id = o.CleanFrequencyListId.ToString(),
                    Value = o.Name
                }).ToList();


                var resultData = new
                {
                    SalesPersonList = SalesPersonList,
                    LeadStageStatusList = LeadStageStatusList,
                    AccountTypeList = AccountTypeList,
                    ProviderTypeList = ProviderTypeList,
                    ProviderSourceList = ProviderSourceList,
                    BiddingPurposeTypeList = BiddingPurposeTypeList,
                    FollowUpPurposeTypeList = FollowUpPurposeTypeList,
                    CloseType = CloseType,
                    State = State,
                    CallTime = CallTime,
                    ServiceTypeListModel = ServiceTypeListModel,
                    FrequencyListModel = FrequencyListModel,
                    CleanFrequencyListModel = CleanFrequencyListModel
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("GetSchedulesForRegion")]
        [HttpGet]
        public IHttpActionResult GetSchedulesForRegion(int UserId, int RegionId)
        {
            try
            {

                var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

                var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

                var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.RegionId == RegionId).ToList();

                #region Calendar

                //Schedule

                if (accountSchedule != null)
                {
                    accountCustomerScheduleViewModels = _crmScheduleEntitiesToViewModels(accountSchedule);
                    foreach (var accountschedule in accountCustomerScheduleViewModels)
                    {
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = accountschedule.Title;
                        calendar.start = accountschedule.StartDate.Value.ToString("s");
                        calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.Description = accountschedule.Description;
                        calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                        calendar.IsAllDay = accountschedule.IsAllDay;
                        calendar.Location = accountschedule.Location;

                        calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_ScheduleTypeId);
                        accountCustomerScheduleCalendar.Add(calendar);
                    }
                }

                #endregion

                var resultData = new
                {
                    schedule = accountCustomerScheduleViewModels,
                    result = accountCustomerScheduleCalendar,
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("SaveCustomerContactInfoPotential")]
        [HttpPost]
        public IHttpActionResult SaveCustomerContactInfoPotential(SaveCustomerContactInfoPotentialRequestModel model)
        {
            try
            {
                var accountcustomer = _crmService.GetCRM_AccountbyId(Convert.ToInt32(model.CRM_AccountId));
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(Convert.ToInt32(model.CRM_AccountCustomerDetailId));
                if (accountcustomer != null && customerAccountDetail != null)
                {
                    accountcustomer.CRM_AccountId = Convert.ToInt32(model.CRM_AccountId);
                    accountcustomer.ContactName = model.ContactName;
                    //accountcustomer.LastName = model.LastName;
                    accountcustomer.PhoneNumber = model.PhoneNumber;
                    accountcustomer.EmailAddress = model.EmailAddress;
                    accountcustomer.ModifiedBy = model.UserId;

                    customerAccountDetail.Title = Convert.ToString(model.Title);
                    customerAccountDetail.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                    customerAccountDetail.CRM_AccountId = Convert.ToInt32(model.CRM_AccountId);
                    customerAccountDetail.CompanyName = model.CompanyName;
                    customerAccountDetail.AccountTypeListId = (Convert.ToString(model.IndustryType) != "" ? Convert.ToInt32(model.IndustryType) : 0);
                    customerAccountDetail.NumberOfLocations = Convert.ToInt32(model.NumberOfLocations);
                    customerAccountDetail.CompanyPhoneNumber = model.CompanyPhoneNumber;
                    customerAccountDetail.CompanyFaxNumber = model.CompanyFaxNumber;
                    customerAccountDetail.CompanyWebSite = model.CompnayWebSite;
                    customerAccountDetail.CompanyEmailAddress = model.CompanyEmailAddress;
                    customerAccountDetail.CompanyAddressLine1 = model.CompanyAddressLine1;
                    customerAccountDetail.CompanyAddressLine2 = model.CompanyAddressLine2;
                    customerAccountDetail.CompanyCity = model.CompanyCity;
                    customerAccountDetail.CompanyCounty = model.CompanyCounty;
                    customerAccountDetail.CompanyState = model.CompanyState;
                    customerAccountDetail.CompanyZipCode = model.CompanyZipCode;

                    customerAccountDetail.SqFt = Convert.ToString(model.Sqft);
                    customerAccountDetail.SalesVolume = Convert.ToString(model.LineofBusiness);
                    customerAccountDetail.LineofBusiness = Convert.ToString(model.SalesVolume);

                    customerAccountDetail.ModifiedBy = model.UserId;

                    var account = _crmService.SaveCRM_Account(accountcustomer);
                    var accountCustomer = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);
                    //_hasNewData = true;

                    var resultDate = new
                    {
                        result = _crmAccountEntityToViewModel(account),
                        accountcustomer = _crmAccountCustomerEntityToViewModel(accountCustomer)
                    };

                    return ResponseSuccessResult(resultDate);
                }

                else
                {
                    return ResponseSuccessResult("error", ApiException.ErrorCode.CustomError, "CustomerAccountDetail and CustomerAccount does not exist");
                }
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CreateFvPresentationSummaryData")]
        [HttpPost]
        public IHttpActionResult CreateFvPresentationSummaryData(CreateFvPresentationSummaryDataRequestModel model)
        {
            try
            {

                var crmfvPresentationViewmodel = new CRMFvPresentationViewModel();
                crmfvPresentationViewmodel.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                crmfvPresentationViewmodel.MeasureContactPerson = model.MeasureContactPerson;
                crmfvPresentationViewmodel.MeasureFacility = model.MeasureFacility != null ? Convert.ToDouble(model.MeasureFacility) : 0.0;
                crmfvPresentationViewmodel.NumberOfFloors = model.NumberOfFloors != null ? Convert.ToInt32(model.NumberOfFloors) : 0;
                crmfvPresentationViewmodel.BillingFrequency = model.Frequency != null ? Convert.ToInt32(model.Frequency) : 0;
                crmfvPresentationViewmodel.ServiceTypeListId = model.ServiceType != null ? Convert.ToInt32(model.ServiceType) : 0;
                crmfvPresentationViewmodel.ServiceLevel = model.ServiceLevel != null ? Convert.ToInt32(model.ServiceLevel) : 0;
                crmfvPresentationViewmodel.CleanFrequency = Convert.ToInt32(model.CleanFrequency);
                crmfvPresentationViewmodel.CleanTime = model.CleanTime != null ? Convert.ToInt32(model.CleanTime) : 0;
                crmfvPresentationViewmodel.BudgetAmount = model.Budget != null ? Convert.ToDecimal(model.Budget) : 0;
                crmfvPresentationViewmodel.Note = model.Note;
                crmfvPresentationViewmodel.CreatedBy = model.UserId;


                crmfvPresentationViewmodel.Mon = model.Mon != null ? Convert.ToBoolean(model.Mon) : false;
                crmfvPresentationViewmodel.Tue = model.Tue != null ? Convert.ToBoolean(model.Tue) : false;
                crmfvPresentationViewmodel.Wed = model.Wed != null ? Convert.ToBoolean(model.Wed) : false;
                crmfvPresentationViewmodel.Thu = model.Thu != null ? Convert.ToBoolean(model.Thu) : false;
                crmfvPresentationViewmodel.Fri = model.Fri != null ? Convert.ToBoolean(model.Fri) : false;
                crmfvPresentationViewmodel.Sat = model.Sat != null ? Convert.ToBoolean(model.Sat) : false;
                crmfvPresentationViewmodel.Sun = model.Sun != null ? Convert.ToBoolean(model.Sun) : false;
                crmfvPresentationViewmodel.IsActive = true;
                crmfvPresentationViewmodel.RegionId = model.RegionId;
                var crmfvpresentation = _crmService.SaveCRM_FvPresentation(_crmFvPresentationViewModelToEntity(crmfvPresentationViewmodel));
                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                //update AccountCustomer            
                if (accountcustomer != null)
                {
                    accountcustomer.StageStatus = (int)StageStatusType.Bidding;
                    accountcustomer.ModifiedBy = model.UserId;
                    _crmService.SaveCRM_Account(accountcustomer);
                }
                var resultDate = new
                {
                    success = true,
                    fvpresentation = _crmFvPresentationEntityToViewModel(crmfvpresentation),
                    stage = (int)StageStatusType.Bidding
                };
                return ResponseSuccessResult(resultDate);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("GetSchedulesForLeadAssignee")]
        [HttpGet]
        public IHttpActionResult GetSchedulesForLeadAssignee(int UserId, int RegionId)
        {
            try
            {

                var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

                var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

                var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.AuthUserLoginId == UserId).ToList();

                #region Calendar

                //Schedule

                if (accountSchedule != null)
                {
                    accountCustomerScheduleViewModels = _crmScheduleEntitiesToViewModels(accountSchedule);
                    foreach (var accountschedule in accountCustomerScheduleViewModels)
                    {
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = accountschedule.Title;
                        calendar.start = accountschedule.StartDate.Value.ToString("s");
                        calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                        calendar.Description = accountschedule.Description;
                        calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                        calendar.IsAllDay = accountschedule.IsAllDay;
                        calendar.Location = accountschedule.Location;

                        calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_ScheduleTypeId);
                        accountCustomerScheduleCalendar.Add(calendar);
                    }
                }

                #endregion

                var resultData = new
                {
                    success = true,
                    schedule = accountCustomerScheduleViewModels,
                    result = accountCustomerScheduleCalendar,
                };


                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("ValidateScheduleAvailability")]
        [HttpGet]
        public IHttpActionResult ValidateScheduleAvailability(int UserId, DateTime startDate, DateTime endDate)
        {
            try
            {

                if (UserId <= 0 || startDate > endDate) // something went wrong with parameters, cancel
                {
                    return ResponseSuccessResult("error", ApiException.ErrorCode.CustomError, "something went wrong with parameters, cancel");
                }

                var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.AuthUserLoginId == UserId).ToList();

                var isFree = true;

                if (accountSchedule != null)
                {
                    foreach (var sch in accountSchedule)
                    {
                        if ((sch.StartDate >= startDate && sch.StartDate <= endDate) || // schedule starts inside timespan
                            (sch.EndDate >= startDate && sch.EndDate <= endDate) || // schedule ends inside timespan
                            (sch.StartDate <= startDate && sch.EndDate >= endDate)) // schedule encompasses whole timespan
                        {
                            isFree = false;
                            break;
                        }
                    }
                }

                return ResponseSuccessResult(isFree);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CRMUploadDocumentPopup")]
        [HttpGet]
        public IHttpActionResult CRMUploadDocumentPopup(int UserId, int RegionId, int AccountCustomerDetailId)
        {
            try
            {

                var resultData = _customerService.GetCRMDocumentByAccountCustomerDetailId(AccountCustomerDetailId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer));

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CreateBiddingSummaryData")]
        [HttpPost]
        public IHttpActionResult CreateBiddingSummaryData(CreateBiddingSummaryDataRequestModel model)
        {
            try
            {
                _crmService.UpdateInActiveCRMLeadStageData(Convert.ToInt32(model.CRM_AccountCustomerDetailId));

                var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();
                var crmBiddingViewModel = new CRMBiddingViewModel();

                crmBiddingViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                crmBiddingViewModel.AnalysisWorkBook = model.IsAnalysisWorkBook;
                crmBiddingViewModel.IsBidSheet = model.IsBidSheet;
                crmBiddingViewModel.IsCancellation = model.IsCancellation;

                crmBiddingViewModel.MonthlyPrice = Convert.ToDecimal(model.MonthlyPrice);
                crmBiddingViewModel.PriceApproved = Convert.ToInt32(model.PriceApproved);
                crmBiddingViewModel.IfBidOver = true;
                crmBiddingViewModel.IncludePrice = Convert.ToDecimal(model.IncludePrice);
                crmBiddingViewModel.Note = model.Note;
                crmBiddingViewModel.CreatedBy = model.UserId;
                crmBiddingViewModel.PurposeId = Convert.ToInt32(model.PruposeId);
                crmBiddingViewModel.Purpose = model.Prupose;
                crmBiddingViewModel.IsActive = true;
                crmBiddingViewModel.RegionId = model.RegionId;
                var crmbidding = _crmService.SaveCRM_Bidding(_crmBiddingViewModelToEntity(crmBiddingViewModel));
                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(model.CRM_AccountId);

                //Save Contact
                var contactViewModel = new CRMContactViewModel();
                contactViewModel.ContactName = model.MeetWithPerson;
                contactViewModel.ContactEmail = model.ContactEmail;
                contactViewModel.ContactPhone = model.ContactPhone;
                contactViewModel.CRM_AccountCustomerDetailId = crmbidding.CRM_AccountCustomerDetailId;
                contactViewModel.CRM_BiddingId = crmbidding.CRM_BiddingId;
                contactViewModel.CreatedBy = model.UserId;

                var contact = _crmContactEntityToViewModel(_crmService.SaveCRM_Contact(_crmContactViewModelToEntity(contactViewModel)));

                //Save Stage Status Schedule
                var schedulevm = new CRMScheduleViewModel();
                schedulevm.ClassId = crmbidding.CRM_AccountCustomerDetailId;

                schedulevm.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.Now;
                schedulevm.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;
                schedulevm.Title = accountcustomer.CRM_AccountId + " - " + "BD" + " " + model.Prupose + " - " + accountcustomer.ContactName + " - " + contact.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;
                schedulevm.Location = customerAccountDetail.CompanyAddressLine1;
                schedulevm.AuthUserLoginId = accountcustomer.AssigneeId;
                schedulevm.CRM_StageStatusType = (int)StageStatusType.Bidding;
                schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                schedulevm.RegionId = model.RegionId;
                schedulevm.CreatedBy = model.UserId;

                schedulevm.PurposeId = Convert.ToInt32(model.PruposeId);
                schedulevm.Purpose = model.Prupose;
                schedulevm.IsActive = true;
                var schedule = _crmService.SaveCRM_Schedule(_crmScheduleViewModelToEntity(schedulevm));

                //update AccountCustomer            
                if (accountcustomer != null)
                {
                    accountcustomer.StageStatus = (int)StageStatusType.PdAppointment;
                    accountcustomer.ModifiedBy = model.UserId;
                    _crmService.SaveCRM_Account(accountcustomer);
                }

                var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(crmbidding.CRM_AccountCustomerDetailId).ToList();
                //Document
                if (accountDocument != null)
                    accountCustomerDocumentViewModels = _crmDocumentEntitiesToViewModels(accountDocument);

                if (Convert.ToInt32(model.PriceApproved) > 0)
                {

                    string PriceApprovedName = "";

                    Array enumValueArray = Enum.GetValues(typeof(priceapproved));
                    foreach (int enumValue in enumValueArray)
                    {
                        if (enumValue == Convert.ToInt32(model.PriceApproved))
                            PriceApprovedName = Enum.GetName(typeof(priceapproved), enumValue).Replace("_", " ");
                    }

                    #region Email send function According to admin configuration

                    //Get Feature Type Id by Feature Name
                    var Leads_Potential_Assignee = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.Potential_Price_Approver.ToString().Replace("_", " ")).FirstOrDefault();

                    if (Leads_Potential_Assignee != null && Leads_Potential_Assignee.FeatureTypeId > 0)
                    {
                        //Get Feature Type Email Id by Feature Type Id
                        var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == Leads_Potential_Assignee.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                        if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                        {
                            //Get Feature Email Body By Message Name Or Tempalte Name 
                            var messageBody = jkEntityModel.MailMessageTemplates.Where(x => x.MailMessageTemplateId == messageDetails.MailMessageTemplateId).FirstOrDefault();
                            //MailMessageTemplateModel objItem = _commonService.GetEmailTemplate(MessageNameModel.FranchiseSecion1.ToString());
                            if (messageBody != null && messageBody.MailMessageTemplateId > 0)
                            {
                                string MessageBody = messageBody.MessageBody;
                                string Subject = messageBody.Subject;

                                if (messageDetails.FromEmail != null && messageDetails.ToEmailId != null)
                                {
                                    _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, MessageBody, Subject);
                                }
                                var UserEmailId = jkEntityModel.AuthUserLogins.Where(x => x.Title == PriceApprovedName).ToList();
                                var getRegionUser = jkEntityModel.AuthUserRegions.Where(x => x.RegionId == model.RegionId).ToList();

                                if (UserEmailId != null && getRegionUser != null)
                                {
                                    foreach (var item in UserEmailId)
                                    {
                                        if (getRegionUser.Where(x => x.UserId == item.UserId).Any())
                                        {
                                            _mailService.SendEmailAsync(item.Email, MessageBody, Subject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                var resultData = new
                {
                    success = true,
                    result = _crmBiddingEntityToViewModel(crmbidding),
                    contact = contact,
                    stageStatusSchedule = _crmScheduleEntityToViewModel(schedule),
                    document = accountCustomerDocumentViewModels,
                    stage = (int)StageStatusType.PdAppointment
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CreatePdAppointmentSummaryData")]
        [HttpPost]
        public IHttpActionResult CreatePdAppointmentSummaryData(CreatePdAppointmentSummaryDataRequestModel model)
        {
            try
            {

                int intRes;

                var PdViewModel = new CRMPdAppointmentViewModel();

                PdViewModel.CRM_AccountCustomerDetailId = model.CRM_AccountCustomerDetailId;
                PdViewModel.MeetPersonName = model.DMeetWithPerson;
                PdViewModel.MeetDecisionMaker = model.MeetWithDecisionMaker;
                PdViewModel.PresentProposal = model.PresentProposal;

                if (int.TryParse(model.OverComeObjection, out intRes))
                    PdViewModel.OverComeObjections = Convert.ToBoolean(intRes);
                else
                    PdViewModel.OverComeObjections = true;

                PdViewModel.Comment = model.Comment;
                //PdViewModel.EnteredProposalDetail = bool.Parse(model.ProposalDetail);
                PdViewModel.Note = model.Note;
                PdViewModel.CreatedBy = model.UserId;
                PdViewModel.PurposeId = Convert.ToInt32(model.CallBack_PurposeId);
                PdViewModel.Purpose = model.CallBack_Purpose;
                PdViewModel.IsActive = true;
                PdViewModel.RegionId = model.RegionId;
                var pdappointment = _crmService.SaveCRM_PdAppointment(_crmPdAppointmentViewModelToEntity(PdViewModel));
                var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(model.CRM_AccountId));

                var contact = new CRMContactViewModel();
                if (pdappointment.MeetDecisionMaker == false)
                {
                    var contactViewModel = new CRMContactViewModel();
                    contactViewModel.ContactName = model.DMeetWithPerson;
                    contactViewModel.ContactEmail = model.DContactEmail;
                    contactViewModel.ContactPhone = model.DContactPhone;
                    contactViewModel.CRM_AccountCustomerDetailId = pdappointment.CRM_AccountCustomerDetailId;
                    contactViewModel.CRM_PdAppointmentId = pdappointment.CRM_PdAppointmentId;

                    contact = _crmContactEntityToViewModel(_crmService.SaveCRM_Contact(_crmContactViewModelToEntity(contactViewModel)));
                }
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(model.CRM_AccountId);

                CRMScheduleViewModel scheduleVM = null;

                if (PdViewModel.PurposeId != 12) // create Schedule only if not "Contract Signed"
                {
                    var schedulevm = new CRMScheduleViewModel();

                    schedulevm.ClassId = pdappointment.CRM_AccountCustomerDetailId;
                    //schedulevm.Title = CRMUtils.SchedulePurpose(Convert.ToInt32(model.CallBack_Purpose));

                    schedulevm.Title = account.CRM_AccountId + " - " + "PD" + " - " + model.CallBack_Purpose + " - " + account.ContactName + " - " + contact.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;

                    schedulevm.StartDate = model.CallBackStartDate != "" ? Convert.ToDateTime(model.CallBackStartDate + " " + model.CallBackStartTime) : DateTime.Now;
                    schedulevm.EndDate = model.CallBackEndDate != "" ? Convert.ToDateTime(model.CallBackEndDate + " " + model.CallBackEndTime) : DateTime.Now;
                    schedulevm.CRM_StageStatusType = (int)StageStatusType.PdAppointment;
                    schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                    schedulevm.RegionId = model.RegionId;
                    schedulevm.CreatedBy = model.UserId;
                    schedulevm.AuthUserLoginId = account.AssigneeId;
                    schedulevm.PurposeId = Convert.ToInt32(model.CallBack_PurposeId);
                    schedulevm.Purpose = model.CallBack_Purpose;
                    schedulevm.IsActive = true;

                    scheduleVM = _crmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(_crmScheduleViewModelToEntity(schedulevm)));
                    scheduleVM.PurposeId = Convert.ToInt32(model.CallBack_PurposeId);
                }

                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);
                //update AccountCustomer            
                if (accountcustomer != null)
                {
                    switch (PdViewModel.PurposeId)
                    {
                        case 12: // contract signed
                            accountcustomer.StageStatus = (int)StageStatusType.Sold;
                            break;
                        default:
                            accountcustomer.StageStatus = (int)StageStatusType.FollowUp;
                            break;
                    }

                    accountcustomer.ModifiedBy = model.UserId;
                    _crmService.SaveCRM_Account(accountcustomer);
                }

                var resultData = new
                {
                    result = _crmPdAppointmentEntityToViewModel(pdappointment),
                    contact = contact,
                    schedule = scheduleVM,
                    stage = accountcustomer?.StageStatus ?? (int)StageStatusType.FollowUp

                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CreateFollowUpSummaryData")]
        [HttpPost]
        public IHttpActionResult CreateFollowUpSummaryData(CreateFollowUpSummaryDataRequestModel model)
        {
            try
            {

                var followupViewModel = new CRMFollowUpViewModel();
                followupViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                followupViewModel.Close = Convert.ToInt32(model.CloseType);
                followupViewModel.Note = model.Note;
                followupViewModel.CreatedBy = model.UserId;
                if (followupViewModel.Close != 1 && followupViewModel.Close != 2 && followupViewModel.Close != 5)
                {
                    followupViewModel.PurposeId = Convert.ToInt32(model.PurposeAgainId);
                    followupViewModel.Purpose = model.PurposeAgain;
                }
                followupViewModel.IsActive = true;
                followupViewModel.RegionId = model.RegionId;
                var followup = _crmService.SaveCRM_FollowUp(_crmFollowupViewModelToEntity(followupViewModel));
                var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(model.CRM_AccountId));


                var schedulevm = new CRMScheduleViewModel();

                if (model.CloseType != 1 && model.CloseType != 2 && model.CloseType != 5)
                {
                    var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(model.CRM_AccountId);
                    schedulevm.ClassId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                    schedulevm.Title = account.CRM_AccountId + " - " + "FU" + " - " + model.PurposeAgain + " - " + account.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;
                    schedulevm.StartDate = model.ScheduleStartDate != "" ? Convert.ToDateTime(model.ScheduleStartDate + " " + model.ScheduleStartTime) : DateTime.Now;
                    schedulevm.EndDate = model.ScheduleEndDate != "" ? Convert.ToDateTime(model.ScheduleEndDate + " " + model.ScheduleEndTime) : DateTime.Now;
                    schedulevm.CRM_StageStatusType = (int)StageStatusType.FollowUp;
                    schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                    schedulevm.RegionId = model.RegionId;
                    schedulevm.AuthUserLoginId = account.AssigneeId;
                    schedulevm.CreatedBy = model.UserId;
                    schedulevm.PurposeId = Convert.ToInt32(model.PurposeAgainId);
                    schedulevm.Purpose = model.PurposeAgain;
                    schedulevm.IsActive = true;
                    var scheduleVM = _crmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(_crmScheduleViewModelToEntity(schedulevm)));
                    scheduleVM.PurposeId = Convert.ToInt32(model.PurposeAgainId);
                }

                //update AccountCustomer            
                if (account != null)
                {
                    if (followup.Close == 5)
                    {
                        account.StageStatus = (int)StageStatusType.QualifiedLead;
                        account.Stage = (int)StageType.Lead;
                        account.ModifiedBy = model.UserId;
                        _crmService.SaveCRM_Account(account);
                        return Json(new
                        {
                            success = true,
                            result = _crmFollowupEntityToViewModel(followup),
                            schedule = schedulevm,
                            stage = 5
                        });
                    }
                    else if (followup.Close == 2)
                    {
                        account.StageStatus = (int)StageStatusType.Sold;
                        account.ModifiedBy = model.UserId;
                        _crmService.SaveCRM_Account(account);
                    }
                    else if (followup.Close == 1)
                    {
                        account.StageStatus = (int)StageStatusType.Bidding;
                        account.ModifiedBy = model.UserId;
                        _crmService.SaveCRM_Account(account);
                    }
                }


                var resultData = new
                {
                    success = true,
                    result = _crmFollowupEntityToViewModel(followup),
                    schedule = schedulevm,
                    stage = (int)StageStatusType.Sold
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("CreateCloseSummaryData")]
        [HttpPost]
        public IHttpActionResult CreateCloseSummaryData(CreateCloseSummaryDataRequestModel model)
        {
            try
            {

                CRMCloseViewModel closeViewModel = new CRMCloseViewModel();
                closeViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(model.CRM_AccountCustomerDetailId);
                closeViewModel.HaveBackgroundCheck = Convert.ToBoolean(model.HaveBackgroundCheck);
                closeViewModel.SignedAgreement = Convert.ToBoolean(model.SignedAgreement);
                closeViewModel.DocumentSalesCRM = Convert.ToBoolean(model.DocumentSalesCRM);
                closeViewModel.NotifyAccountPlacement = Convert.ToBoolean(model.NotifyAccountPlacement);
                closeViewModel.PropAmount = Convert.ToDecimal(model.PropAmount);
                closeViewModel.InitialClean = Convert.ToInt32(model.InitialClean);
                closeViewModel.ContractAmount = Convert.ToDecimal(model.ContractAmount);
                closeViewModel.InitialCleanAmount = Convert.ToDecimal(model.InitialCleanAmount);
                closeViewModel.StartDate = model.StartDate != null ? model.StartDate : DateTime.Now;
                closeViewModel.SignDate = model.SignDate != null ? model.SignDate : DateTime.Now;
                closeViewModel.PhoneNumber = model.PhoneNumber;
                closeViewModel.ContractTerm = model.ContractTerm;
                closeViewModel.ServiceType = Convert.ToInt32(model.ServiceType);
                closeViewModel.BillingFrequency = Convert.ToInt32(model.BillingFrequency);
                closeViewModel.CleanTime = Convert.ToInt32(model.CleanTime);
                closeViewModel.CleanFrequency = Convert.ToInt32(model.CleanFrequency);
                closeViewModel.Note = model.Note;
                closeViewModel.IsActive = true;
                var checkDayscount = model.CleaningDay;
                if (checkDayscount.Count() >= 1)
                {
                    string[] cleaningDay = model.CleaningDay[0].Split(',');
                    foreach (string item in cleaningDay)
                    {
                        int value = int.Parse(item);

                        if (value == 1) { closeViewModel.Mon = true; }

                        if (value == 2) { closeViewModel.Tue = true; }

                        if (value == 3) { closeViewModel.Wed = true; }

                        if (value == 4) { closeViewModel.Thu = true; }

                        if (value == 5) { closeViewModel.Fri = true; }

                        if (value == 6) { closeViewModel.Sat = true; }

                        if (value == 7) { closeViewModel.Sun = true; }
                    }
                }

                var close = _crmService.SaveCRM_Close(_crmCloseViewModelToEntity(closeViewModel));

                //Close Lead Convert into Customer 
                var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(model.CRM_AccountId));

                var accountcustomer = _crmService.GetCRM_AccountbyId(model.CRM_AccountId);

                //update AccountCustomer            
                if (accountcustomer != null)
                {
                    accountcustomer.StageStatus = (int)StageStatusType.Close;
                    accountcustomer.Stage = (int)StageType.Customer;
                    accountcustomer.ModifiedBy = model.UserId;
                    _crmService.SaveCRM_Account(accountcustomer);
                }

                var customerdetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(account.CRM_AccountId);


                //Customer Details
                FullCustomerViewModel fullCustomerViewModel = new FullCustomerViewModel();
                fullCustomerViewModel.CustomerViewModel.Name = customerdetail.CompanyName;
                fullCustomerViewModel.CustomerViewModel.StatusListId = 38;    //Region Operation = 38
                fullCustomerViewModel.CustomerViewModel.CustomerNo = _customerService.getCustomerNo(model.RegionId);
                fullCustomerViewModel.CustomerViewModel.CreatedDate = DateTime.Now;
                fullCustomerViewModel.CustomerViewModel.CreatedBy = model.UserId;
                fullCustomerViewModel.CustomerViewModel.IsActive = true;
                fullCustomerViewModel.CustomerViewModel.RegionId = model.RegionId;
                fullCustomerViewModel.CustomerViewModel.ParentId = -1;
                fullCustomerViewModel.CustomerViewModel = _customerService.SaveCustomers(fullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                int _CustomerIdTemp = fullCustomerViewModel.CustomerViewModel.CustomerId;
                RegionSetting regData = _customerService.GetRegionConfigurationbyId(3, model.RegionId);
                int valueadd = Convert.ToInt32(regData.Value) + 1;
                regData.Value = valueadd.ToString();
                _customerService.SaveRegionConfiguration(regData);

                ////Main Information

                string contactName = string.Format("{0} {1}", account.FirstName?.Trim(), account.LastName?.Trim()).Trim();
                if (string.IsNullOrEmpty(contactName))
                    contactName = account.ContactName;

                fullCustomerViewModel.MainContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainContact.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                fullCustomerViewModel.MainContact.Name = contactName;
                fullCustomerViewModel.MainContact.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainContact.IsActive = true;
                fullCustomerViewModel.MainContact.CreatedBy = model.UserId.ToString();
                fullCustomerViewModel.MainContact = _customerService.SaveContact(fullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                fullCustomerViewModel.MainAddress.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainAddress.Address1 = customerdetail.CompanyAddressLine1;
                fullCustomerViewModel.MainAddress.Address2 = customerdetail.CompanyAddressLine2;
                fullCustomerViewModel.MainAddress.City = customerdetail.CompanyCity;
                fullCustomerViewModel.MainAddress.StateName = customerdetail.CompanyState;
                fullCustomerViewModel.MainAddress.PostalCode = customerdetail.CompanyZipCode;

                var _latlng = _getLatLongByAddress(System.Web.HttpUtility.UrlEncode(fullCustomerViewModel.MainAddress.FullAddress));
                if (_latlng.results.Count > 0)
                {
                    fullCustomerViewModel.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    fullCustomerViewModel.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }


                if (customerdetail.CompanyState != null)
                {
                    int state = _customerService.GetStateId(customerdetail.CompanyState);
                    fullCustomerViewModel.MainAddress.StateListId = state;
                }


                fullCustomerViewModel.MainAddress.IsActive = true;
                fullCustomerViewModel.MainAddress.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainAddress.CreatedBy = model.UserId;
                fullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainAddress = _customerService.SaveAddress(fullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                fullCustomerViewModel.MainPhone.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainPhone.Phone1 = account.PhoneNumber;
                fullCustomerViewModel.MainPhone.Cell = account.PhoneNumber;
                fullCustomerViewModel.MainPhone.IsActive = true;
                fullCustomerViewModel.MainPhone.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainPhone.CreatedBy = model.UserId;
                fullCustomerViewModel.MainPhone = _customerService.SavePhone(fullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                fullCustomerViewModel.MainPhone2.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainPhone2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainPhone2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                fullCustomerViewModel.MainPhone2.Phone1 = customerdetail.CompanyPhoneNumber;
                fullCustomerViewModel.MainPhone2.Fax = customerdetail.CompanyFaxNumber;
                fullCustomerViewModel.MainPhone2.Cell = customerdetail.CompanyPhoneNumber;
                fullCustomerViewModel.MainPhone2.IsActive = true;
                fullCustomerViewModel.MainPhone2.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainPhone2.CreatedBy = model.UserId;
                fullCustomerViewModel.MainPhone2 = _customerService.SavePhone(fullCustomerViewModel.MainPhone2.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                fullCustomerViewModel.MainEmail.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainEmail.EmailAddress = account.EmailAddress;
                fullCustomerViewModel.MainEmail.IsActive = true;
                fullCustomerViewModel.MainEmail.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainEmail.CreatedBy = model.UserId.ToString();
                fullCustomerViewModel.MainEmail = _customerService.SaveEmail(fullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                fullCustomerViewModel.MainEmail2.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainEmail2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainEmail2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                fullCustomerViewModel.MainEmail2.EmailAddress = customerdetail.CompanyEmailAddress;
                fullCustomerViewModel.MainEmail2.IsActive = true;
                fullCustomerViewModel.MainEmail2.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainEmail2.CreatedBy = model.UserId.ToString();
                fullCustomerViewModel.MainEmail2 = _customerService.SaveEmail(fullCustomerViewModel.MainEmail2.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                fullCustomerViewModel.Contract.CustomerId = _CustomerIdTemp;
                fullCustomerViewModel.Contract.AgreementTypeListId = 2;
                //fullCustomerViewModel.Contract.AccountTypeListId;

                fullCustomerViewModel.Contract.PurchaseOrderNo = close.PhoneNumber;
                fullCustomerViewModel.Contract.ContractTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.Contract.SignDate = close.SignDate;
                fullCustomerViewModel.Contract.StartDate = close.StartDate;
                fullCustomerViewModel.Contract.ExpirationDate = DateTime.Parse(close.StartDate.ToString()).AddMonths(Convert.ToInt32(close.ContractTerm));
                fullCustomerViewModel.Contract.Amount = close.ContractAmount;
                fullCustomerViewModel.Contract.ContractTermMonth = Convert.ToInt32(close.ContractTerm);
                fullCustomerViewModel.Contract.AddressId = fullCustomerViewModel.MainAddress.AddressId;
                fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
                fullCustomerViewModel.Contract.isActive = true;
                fullCustomerViewModel.Contract.PurchaseOrderNo = model.PhoneNumber;
                fullCustomerViewModel.Contract.CreatedBy = model.UserId;
                fullCustomerViewModel.Contract.AccountTypeListId = customerdetail.AccountTypeListId;
                fullCustomerViewModel.Contract.InitialCleanAmount = model.InitialCleanAmount;
                fullCustomerViewModel.Contract.StatusListId = 38; //Region Operation = 38
                fullCustomerViewModel.Contract = _customerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

                fullCustomerViewModel.ContractDetail.ContractId = fullCustomerViewModel.Contract.ContractId;
                fullCustomerViewModel.ContractDetail.ServiceTypeListId = close.ServiceType;
                fullCustomerViewModel.ContractDetail.BillingFrequencyListId = close.BillingFrequency;
                fullCustomerViewModel.ContractDetail.CleanTimes = close.CleanTime;
                fullCustomerViewModel.ContractDetail.CleanFrequencyListId = (int)close.CleanFrequency;
                fullCustomerViewModel.ContractDetail.Mon = close.Mon != null ? true : false;
                fullCustomerViewModel.ContractDetail.Tues = close.Tue != null ? true : false;
                fullCustomerViewModel.ContractDetail.Wed = close.Wed != null ? true : false;
                fullCustomerViewModel.ContractDetail.Thur = close.Thu != null ? true : false;
                fullCustomerViewModel.ContractDetail.Fri = close.Fri != null ? true : false;
                fullCustomerViewModel.ContractDetail.Sat = close.Sat != null ? true : false;
                fullCustomerViewModel.ContractDetail.Sun = close.Sun != null ? true : false;
                fullCustomerViewModel.ContractDetail.Amount = close.ContractAmount;
                fullCustomerViewModel.ContractDetail.CPIIncrease = true;
                fullCustomerViewModel.ContractDetail.LineNumber = 1;
                fullCustomerViewModel.ContractDetail.SeparateInvoice = false;
                fullCustomerViewModel.ContractDetail.SquareFootage = customerdetail.SqFt;
                fullCustomerViewModel.ContractDetail.CreatedDate = DateTime.Now;
                fullCustomerViewModel.ContractDetail.CreatedBy = model.UserId;
                fullCustomerViewModel.ContractDetail.StartTime = DateTime.Now;
                fullCustomerViewModel.ContractDetail.EndTime = DateTime.Now.AddHours(1);
                fullCustomerViewModel.ContractDetail = _customerService.SaveContractDetail(fullCustomerViewModel.ContractDetail.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

                //Save Customer-Sales-Document 
                if (model.file1 != null)
                {
                    if (model.file1.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file2 != null)
                {
                    if (model.file2.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file3 != null)
                {
                    if (model.file3.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file4 != null)
                {
                    if (model.file4.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file5 != null)
                {
                    if (model.file5.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file6 != null)
                {
                    if (model.file6.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file7 != null)
                {
                    if (model.file7.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file8 != null)
                {
                    if (model.file8.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file9 != null)
                {
                    if (model.file9.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file10 != null)
                {
                    if (model.file10.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file11 != null)
                {
                    if (model.file11.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }
                if (model.file12 != null)
                {
                    if (model.file12.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, model.ToModel<TempDocumentViewModel, CreateCloseSummaryDataRequestModel>()); }
                }

                var docList = _customerService.GetCRMDocumentByAccountCustomerDetailId(model.CRM_AccountCustomerDetailId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer));

                foreach (var item in docList)
                {
                    if (item.FileTypeListId == 1 && model.file1 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 2 && model.file2 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 3 && model.file3 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 4 && model.file4 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 5 && model.file5 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 6 && model.file6 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 7 && model.file7 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 8 && model.file8 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 9 && model.file9 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 10 && model.file10 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 11 && model.file11 == null) { _moveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 12 && model.file12 == null) { _moveSalesDocument(_CustomerIdTemp, item); }



                }

                if (_CustomerIdTemp > 0)
                    _commonService.CommonInsertNotification(7, "", false, _CustomerIdTemp, 1, null, null, null, model.UserId);

                var resultData = new
                {
                    sold = _crmCloseEntityToViewModel(close),
                    stage = (int)StageStatusType.Close
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("PDAppointmentPopup")]
        [HttpGet]
        public IHttpActionResult PDAppointmentPopup(int UserId, int RegionId, int CustomerDetailbyId)
        {
            try
            {
                var bidSch = new CRMScheduleViewModel();
                var bidContact = new CRMContactViewModel();

                var accountBidding = _crmService.Get_Bidding_ByAccountCustomerDetailById(CustomerDetailbyId);
                if (accountBidding != null)
                {
                    bidContact = _crmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_BiddingId == accountBidding.CRM_BiddingId));
                    bidSch = _crmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                }

                var accountCustomerPdAppointmentViewModel = new CRMPdAppointmentViewModel();
                var pdSch = new CRMScheduleViewModel();
                var pdContact = new CRMContactViewModel();

                var accountPdAppointment = _crmService.Get_PdAppointment_ByAccountCustomerDetailById(CustomerDetailbyId);
                //PdAppointment
                if (accountPdAppointment != null)
                {
                    accountCustomerPdAppointmentViewModel = _crmPdAppointmentEntityToViewModel(accountPdAppointment);
                    pdContact = _crmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
                    pdSch = _crmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                    if (pdSch != null)
                    {
                        pdSch.PurposeId = pdSch.PurposeId;// CRMUtils.SchedulePurposIndex(pdSch.Title);
                        pdSch.Purpose = pdSch.Purpose;
                    }
                }
                else
                {
                    accountCustomerPdAppointmentViewModel = null;
                }

                var resultData = new
                {
                    pdnfo = accountCustomerPdAppointmentViewModel,
                    pdContact = pdContact,
                    pdSch = pdSch,
                    bidContact = bidContact,
                    bidSch = bidSch
                };

                return ResponseSuccessResult(resultData);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }


        }

        [Route("GetRegionList")]
        [HttpGet]
        public IHttpActionResult GetRegionList(int UserId, int RegionId)
        {
            try
            {
                var getRegionUser = jkEntityModel.Regions.Where(x => x.Corporate == 1).ToList().Select(o => new DropDownResponseModel
                {
                    Id = o.RegionId.ToString(),
                    Value = o.Name
                }).Distinct().ToList(); ;
                return ResponseSuccessResult(getRegionUser);

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("calllog/listbyaccount")]
        [HttpPost]
        [ResponseType(typeof(List<CallLogModel>))]
        public IHttpActionResult CallLogListByAccount(CrmCallLogByAccountRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewCallLogListModel
                {
                    AccountId = requestDto.AccountId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _crmService.GetCallLogListByAccount(listModel);
                return ResponseSuccessResult(listModel.CallLogList);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("calllog/listbyaccountcustomerdetail")]
        [HttpPost]
        [ResponseType(typeof(List<CallLogModel>))]
        public IHttpActionResult CallLogListByAccountCustomerDetail(CrmCallLogByAccountCustomerDetailRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewCallLogListModel
                {
                    AccountCustomerDetailId = requestDto.AccountCustomerDetailId,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _crmService.GetCallLogListByAccountCustomerDetail(listModel);
                return ResponseSuccessResult(listModel.CallLogList);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("calllog/{calllogid}")]
        [HttpGet]
        [ResponseType(typeof(List<CallLogModel>))]
        public IHttpActionResult CallLogById(int calllogid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var callLog = _crmService.GetCallLog(calllogid);
                return ResponseSuccessResult(callLog);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("calllog/addorupdate")]
        [HttpPut]
        [ResponseType(typeof(CallLogModel))]
        public IHttpActionResult AddOrUpdate(CrmCallLogUpdateRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var callLog = new CallLogModel
                {
                    CallLogId = requestDto.CallLogId,
                    AccountId = requestDto.AccountId,
                    AccountCustomerDetailId = requestDto.AccountCustomerDetailId,
                    LeadSource = requestDto.LeadSource,
                    CallResultId = requestDto.CallResultId,
                    StageStatus = requestDto.StageStatus,
                    NoteTypeId = requestDto.NoteTypeId,
                    SpokeWith = requestDto.SpokeWith,
                    Note = requestDto.SpokeWith,
                    CallLogDate = requestDto.CallLogDate,
                    CallBack = requestDto.CallBack,
                    CallBackTime = requestDto.CallBackTime,
                    CreatedBy = requestDto.CreatedBy,
                    CreatedDate = requestDto.CreatedDate,
                    ModifiedBy = requestDto.ModifiedBy,
                    ModifiedDate = requestDto.ModifiedDate
                };

                callLog = _crmService.AddOrUpdateCallLog(callLog);
                return ResponseSuccessResult(callLog);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        #endregion
        // ======================================================================================
        #region CRMController > Private
        // ======================================================================================
        private CRMAccountCustomerDetailResponseModel _crmAccountCustomerEntityToViewModel(CRM_AccountCustomerDetail accountCustomerDetailEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_AccountCustomerDetail, CRMAccountCustomerDetailResponseModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_AccountCustomerDetail, CRMAccountCustomerDetailResponseModel>(accountCustomerDetailEntity);
        }

        private CRMInitialCommunicationViewModel _crmInitialEntityToViewModel(CRM_InitialCommunication initialEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_InitialCommunication, CRMInitialCommunicationViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_InitialCommunication, CRMInitialCommunicationViewModel>(initialEntity);
        }

        private CRM_InitialCommunication _crmInitialViewModelToEntity(CRMInitialCommunicationViewModel initialViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMInitialCommunicationViewModel, CRM_InitialCommunication>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMInitialCommunicationViewModel, CRM_InitialCommunication>(initialViewModel);
        }

        private CRM_Schedule _crmScheduleViewModelToEntity(CRMScheduleViewModel scheduleViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMScheduleViewModel, CRM_Schedule>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMScheduleViewModel, CRM_Schedule>(scheduleViewModel);
        }

        private IEnumerable<KeyValuePair<string, int>> _getEnumValuesAndDescriptions<T>()
        {
            Type enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T is not System.Enum");

            List<KeyValuePair<string, int>> enumValList = new List<KeyValuePair<string, int>>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                enumValList.Add(new KeyValuePair<string, int>((attributes.Length > 0) ? attributes[0].Description : e.ToString(), (int)e));
            }

            return enumValList;
        }

        private string _formatUsPhoneNumber(string value)
        {
            if (value != "" && value != null)
            {
                value = new System.Text.RegularExpressions.Regex(@"\D").Replace(value, string.Empty);
                value = value.TrimStart('1');
                if (value.Length == 7)
                    return Convert.ToInt64(value).ToString("###-####");
                if (value.Length == 10)
                    return Convert.ToInt64(value).ToString("(###) ###-####");
                if (value.Length > 10)
                    return Convert.ToInt64(value)
                        .ToString("(###) ###-#### " + new string('#', (value.Length - 10)));
            }
            return value;
        }

        private List<CRMScheduleViewModel> _crmScheduleEntitiesToViewModels(List<CRM_Schedule> scheduleEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Schedule, CRMScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            var scheduleViewModels = mapper.Map<IEnumerable<CRM_Schedule>, IEnumerable<CRMScheduleViewModel>>(scheduleEntities) as List<CRMScheduleViewModel>;
            return scheduleViewModels;

        }

        private IEnumerable<string> _getBestTimeList()
        {
            var valueList = new List<string>();
            var lists = Enum.GetValues(typeof(CallTime)).Cast<CallTime>();
            foreach (var list in lists)
            {
                valueList.Add(list.ToString() ?? "N/A");
            }
            return valueList;
        }

        private CRMAccountCustomerDetailViewModel _oldCrmAccountCustomerEntityToViewModel(CRM_AccountCustomerDetail accountCustomerDetailEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_AccountCustomerDetail, CRMAccountCustomerDetailViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_AccountCustomerDetail, CRMAccountCustomerDetailViewModel>(accountCustomerDetailEntity);
        }

        private List<CRMActivityViewModel> _crmActivityEntitiesToViewModels(List<CRM_Activity> activityEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Activity, CRMActivityViewModel>();
            });
            var mapper = config.CreateMapper();
            var activityViewModels = mapper.Map<IEnumerable<CRM_Activity>, IEnumerable<CRMActivityViewModel>>(activityEntities) as List<CRMActivityViewModel>;
            return activityViewModels;
        }

        private List<CRMNoteViewModel> _crmNoteEntitiesToViewModels(List<CRM_Note> noteEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Note, CRMNoteViewModel>();
            });
            var mapper = config.CreateMapper();
            var activityViewModels = mapper.Map<IEnumerable<CRM_Note>, IEnumerable<CRMNoteViewModel>>(noteEntities) as List<CRMNoteViewModel>;
            return activityViewModels;
        }
        private List<CRMDocumentViewModel> _crmDocumentEntitiesToViewModels(List<CRM_Document> documentEntities)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Document, CRMDocumentViewModel>();
            });
            var mapper = config.CreateMapper();
            var documentViewModels = mapper.Map<IEnumerable<CRM_Document>, IEnumerable<CRMDocumentViewModel>>(documentEntities) as List<CRMDocumentViewModel>;
            return documentViewModels;
        }

        private CRMScheduleViewModel _crmScheduleEntityToViewModel(CRM_Schedule scheduleEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Schedule, CRMScheduleViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Schedule, CRMScheduleViewModel>(scheduleEntity);
        }

        private CRMFvPresentationViewModel _crmFvPresentationEntityToViewModel(CRM_FvPresentation fvpresentationEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FvPresentation, CRMFvPresentationViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FvPresentation, CRMFvPresentationViewModel>(fvpresentationEntity);
        }

        private CRMBiddingViewModel _crmBiddingEntityToViewModel(CRM_Bidding biddingEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Bidding, CRMBiddingViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Bidding, CRMBiddingViewModel>(biddingEntity);
        }
        private CRMCloseViewModel _crmCloseEntityToViewModel(CRM_Close crmCloseEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Close, CRMCloseViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Close, CRMCloseViewModel>(crmCloseEntity);
        }

        private CRMContactViewModel _crmContactEntityToViewModel(CRM_Contact crmContactEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Contact, CRMContactViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Contact, CRMContactViewModel>(crmContactEntity);
        }

        private CRMPdAppointmentViewModel _crmPdAppointmentEntityToViewModel(CRM_PdAppointment pdEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_PdAppointment, CRMPdAppointmentViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_PdAppointment, CRMPdAppointmentViewModel>(pdEntity);
        }

        private CRMFollowUpViewModel _crmFollowupEntityToViewModel(CRM_FollowUp followEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_FollowUp, CRMFollowUpViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_FollowUp, CRMFollowUpViewModel>(followEntity);
        }

        private CRMAccountViewModel _crmAccountEntityToViewModel(CRM_Account accountEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRM_Account, CRMAccountViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRM_Account, CRMAccountViewModel>(accountEntity);
        }

        private CRM_FvPresentation _crmFvPresentationViewModelToEntity(CRMFvPresentationViewModel fvpresentationViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFvPresentationViewModel, CRM_FvPresentation>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFvPresentationViewModel, CRM_FvPresentation>(fvpresentationViewModel);
        }

        private CRM_Contact _crmContactViewModelToEntity(CRMContactViewModel crmContactViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMContactViewModel, CRM_Contact>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMContactViewModel, CRM_Contact>(crmContactViewModel);

        }

        private CRM_Bidding _crmBiddingViewModelToEntity(CRMBiddingViewModel biddingViewViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMBiddingViewModel, CRM_Bidding>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMBiddingViewModel, CRM_Bidding>(biddingViewViewModel);
        }

        private CRM_PdAppointment _crmPdAppointmentViewModelToEntity(CRMPdAppointmentViewModel pdViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMPdAppointmentViewModel, CRM_PdAppointment>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMPdAppointmentViewModel, CRM_PdAppointment>(pdViewModel);
        }

        private CRM_FollowUp _crmFollowupViewModelToEntity(CRMFollowUpViewModel followViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMFollowUpViewModel, CRM_FollowUp>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMFollowUpViewModel, CRM_FollowUp>(followViewModel);
        }

        private CRM_Close _crmCloseViewModelToEntity(CRMCloseViewModel crmCloseViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMCloseViewModel, CRM_Close>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMCloseViewModel, CRM_Close>(crmCloseViewModel);
        }

        private RootObjectlatlngViewModel _getLatLongByAddress(string address)
        {
            var root = new RootObjectlatlngViewModel();
            //string.Format("https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(AddSTR) + "&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";
            try
            {


                var url =
                    string.Format(
                        "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8", address);
                var req = (HttpWebRequest)WebRequest.Create(url);

                var res = (HttpWebResponse)req.GetResponse();

                using (var streamreader = new StreamReader(res.GetResponseStream()))
                {
                    var result = streamreader.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        root = JsonConvert.DeserializeObject<RootObjectlatlngViewModel>(result);
                    }
                }
            }
            catch (Exception)
            {


            }
            return root;


        }

        private void _moveSalesDocument(int CustomerId, CRMDocumentViewModel tempViewModel)
        {
            try
            {
                UploadCustomerFile(CustomerId);

                if (!String.IsNullOrEmpty(tempViewModel.File_Name))
                {
                    //string _FileName = Path.GetFileName(tempViewModel.File_Name);

                    int _FileSize = Path.GetFileName(tempViewModel.File_Name).Length;
                    //string _FileExt = Path.GetFileName(tempViewModel.File_Name).Split('.').Last();
                    //string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _FileName = Path.GetFileName(tempViewModel.File_Name).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.File_Name).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);

                    if (System.IO.File.Exists(tempViewModel.Document_FilePath))
                    {
                        System.IO.File.Copy(tempViewModel.Document_FilePath, _path, true);
                    }
                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, _FilePath, _FileName, _FileExt, _FileSize);
                }
                else
                {
                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, "", "", "", 0);

                }
            }
            catch (Exception ex)
            {
                _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, ex.Message, ex.Message, "", 0);
            }




        }

        private CRM_Document _crmDocumentViewModelToEntity(CRMDocumentViewModel documentViewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CRMDocumentViewModel, CRM_Document>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<CRMDocumentViewModel, CRM_Document>(documentViewModel);
        }

        private int _getSelectedUserId(int userId)
        {
            var roleId = (from a in jkEntityModel.AuthUserRoles
                          join b in jkEntityModel.AuthRoles on a.RoleId equals b.RoleId
                          where a.UserId == userId && (b.RoleTypeId == 1 || b.RoleTypeId == 2)
                          select new { id = a.UserId }
                         ).ToList();
            if (roleId.Count > 0)
            {
                return 0;
            }
            else
            {
                return userId;
            }
        }

        private IHttpActionResult _getPotentialCustomerList(CrmPotentialCustomerRequestDto requestDto, CrmFilterChoice choice)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new CRMPotentialCustomerListViewModel
                {
                    RegionId = requestDto.RegionId,
                    UserId = _getSelectedUserId(requestDto.UserId),
                    LoginUserId = requestDto.UserId,
                    Choice = choice,
                    Type = 0,
                    PageSize = requestDto.PageSize > 0 ? requestDto.PageSize : 10,
                    CurrentPage = requestDto.Page > 0 ? requestDto.Page : 1,
                    IsEnable = true
                };
                listModel = _crmService.GetPotentialCustomerList(listModel);
                return ResponseSuccessResult(listModel.PotentialCustomerList);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        private CRMAccountCustomerDetailResponseModel _getLeadCustomerById(int leadId)
        {
            var accountCustomerDetailModel = new CRMAccountCustomerDetailResponseModel();
            var account = _crmService.GetCRM_AccountbyId(leadId);
            var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(leadId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            if (account != null)
            {
                accountCustomerDetailModel = _crmAccountCustomerEntityToViewModel(accountCustomerDetail);
                accountCustomerDetailModel.FirstName = account.FirstName;
                accountCustomerDetailModel.LastName = account.LastName;
                accountCustomerDetailModel.ContactName = account.ContactName;
                accountCustomerDetailModel.MiddleInitial = account.MiddleInitial;
                accountCustomerDetailModel.PhoneNumber = account.PhoneNumber;
                accountCustomerDetailModel.EmailAddress = account.EmailAddress;
                accountCustomerDetailModel.Stage = (StageType)account.Stage.Value;
                accountCustomerDetailModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountCustomerDetailModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountCustomerDetailModel.ProviderType = (AccountProviderType)account.ProviderType.Value;

                var iniate = _crmInitialEntityToViewModel(_crmService.Get_InitialCommunication_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId));
                if (iniate != null)
                {
                    accountCustomerDetailModel.ContactPerson = iniate.ContactPerson;

                    if (iniate.StartDate != null)
                    {
                        accountCustomerDetailModel.StartDate = iniate.StartDate?.ToString("MM/dd/yyyy") ?? "";
                        accountCustomerDetailModel.StartTime = iniate.StartDate.Value.ToString("h:mm:ss tt");

                        accountCustomerDetailModel.EndDate = iniate.EndDate?.ToString("MM/dd/yyyy") ?? "";
                        accountCustomerDetailModel.EndTime = iniate.EndDate?.ToString("h:mm:ss tt") ?? "";
                    }

                    accountCustomerDetailModel.InterestedInPerposal = iniate.InterestedInPerposal;
                    accountCustomerDetailModel.Purpose = Convert.ToInt32(iniate.PURPOSE);
                }
            }
            return accountCustomerDetailModel;
        }

        #endregion
        // ======================================================================================
        #region CRM Upload Documents

        public void UploadCustomerFile(int CustomerId)
        {
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument")))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument"));
            }
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));
            }

        }

        public void SaveSalesDocument(int CustomerId, TempDocumentViewModel tempViewModel)
        {
            UploadCustomerFile(CustomerId);

            #region Sales file from 1 to 12

            if (tempViewModel.file1 != null)
            {
                if (tempViewModel.file1.ContentLength > 0)
                {

                    int _FileSize = Path.GetFileName(tempViewModel.file1.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file1.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file1.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file1.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 1, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file2 != null)
            {
                if (tempViewModel.file2.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file2.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file2.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file2.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file2.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;


                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 2, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file3 != null)
            {
                if (tempViewModel.file3.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file3.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file3.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file3.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file3.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 3, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file4 != null)
            {
                if (tempViewModel.file4.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file4.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file4.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file4.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file4.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 4, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file5 != null)
            {
                if (tempViewModel.file5.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file5.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file5.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file5.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file5.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 5, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file6 != null)
            {
                if (tempViewModel.file6.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file6.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file6.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file6.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file6.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 6, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file7 != null)
            {
                if (tempViewModel.file7.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file7.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file7.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file7.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file7.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 7, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file8 != null)
            {
                if (tempViewModel.file8.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file8.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file8.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file8.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file8.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 8, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file9 != null)
            {
                if (tempViewModel.file9.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file9.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file9.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file9.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file9.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 9, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file10 != null)
            {
                if (tempViewModel.file10.ContentLength > 0)
                {

                    int _FileSize = Path.GetFileName(tempViewModel.file10.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file10.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file10.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file10.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;


                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 10, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file11 != null)
            {
                if (tempViewModel.file11.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file11.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file11.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file11.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file11.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 11, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file12 != null)
            {
                if (tempViewModel.file12.ContentLength > 0)
                {
                    int _FileSize = Path.GetFileName(tempViewModel.file12.FileName).Length;
                    string _FileName = Path.GetFileName(tempViewModel.file12.FileName).Replace(" ", "_");
                    string _FileExt = Path.GetFileName(tempViewModel.file12.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    tempViewModel.file12.SaveAs(_path);

                    String _FilePath = ConfigurationManager.AppSettings["FilesDirectory"].ToString() + "CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    _customerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 12, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            #endregion

        }


        [Route("CRMSaveDocuments")]
        [HttpPost]
        public IHttpActionResult CRMSaveDocuments(int UserId, int RegionId, int CRMAccCustDetailId, string selIds)
        {
            try
            {
                UploadCustomerFile(Convert.ToInt32(CRMAccCustDetailId));

                if (selIds != "")
                {
                    string[] arrIds = selIds.Split(',');
                    for (int i = 0; i < arrIds.Length; i++)
                    {
                        if (HttpContext.Current.Request.Files.Count > 0)
                        {
                            var crmdocumentViewModel = new CRMDocumentViewModel();


                            //System.Web.HttpFileCollectionBase files = Request.Files;
                            var file = HttpContext.Current.Request.Files[0];
                            string DocumentId = arrIds[i];

                            string _FileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                            string _FileExt = Path.GetFileName(file.FileName).Split('.').Last();
                            string fname = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                            //string fname = CRMAccCustDetailId + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + file.FileName.Trim();

                            var CRMModel = _crmService.GetCRMDocumentWithAccountCustomer_FileType(Convert.ToInt32(CRMAccCustDetailId), Convert.ToInt32(DocumentId));

                            if (CRMModel != null && CRMModel.CRM_DocumentId > 0)
                            {
                                CRMModel.RegionId = RegionId;
                                CRMModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
                                CRMModel.File_Name = fname;
                                CRMModel.Content_Type = file.ContentType;
                                //CRMModel.Document_FilePath = Path.Combine(_fileDirectory+ "Areas/CRM/Documents", fname);                                                        
                                CRMModel.Document_FilePath = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname);
                                file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname));
                                var crmdocument = _crmService.SaveCRM_Document(CRMModel);
                            }
                            else
                            {
                                crmdocumentViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(CRMAccCustDetailId);
                                crmdocumentViewModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
                                crmdocumentViewModel.File_Name = fname;
                                crmdocumentViewModel.Content_Type = file.ContentType;
                                crmdocumentViewModel.Description = "";
                                crmdocumentViewModel.FileTypeListId = Convert.ToInt32(DocumentId);
                                //crmdocumentViewModel.Document_FilePath = Path.Combine(_fileDirectory+ "Areas/CRM/Documents", fname);
                                crmdocumentViewModel.Document_FilePath = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname);

                                //if (System.IO.File.Exists(crmdocumentViewModel.Document_FilePath)) { continue; /*TODO Message*/ }

                                file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname));
                                var crmdocument = _crmService.SaveCRM_Document(_crmDocumentViewModelToEntity(crmdocumentViewModel));
                            }
                        }
                    }

                    int CRM_AccId = Convert.ToInt32(CRMAccCustDetailId);
                    var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountCustomerDetailId == CRM_AccId).ToList();


                    return ResponseSuccessResult(_crmDocumentEntitiesToViewModels(crmdocuments));
                }

                return ResponseSuccessResult(false, ApiException.ErrorCode.CustomError, "There is no document to upload");

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("RemoveCRM_Document")]
        [HttpGet]
        public IHttpActionResult RemoveCRM_Document(int Id, int CRMAccId)
        {
            try
            {
                if (Id > 0)
                {
                    var getData = _crmService.GetUploadDocumentById(Id);
                    if (getData != null && getData.CRM_DocumentId > 0 && getData.Document_FilePath != null)
                    {
                        if (Directory.Exists(ConfigurationManager.AppSettings["FilesDirectory"].ToString() + getData.Document_FilePath))
                        {
                            Directory.Delete(ConfigurationManager.AppSettings["FilesDirectory"].ToString() + getData.Document_FilePath);
                        }
                    }
                    _crmService.DeleteCRM_Document(Id);
                }
                int CRM_AccId = Convert.ToInt32(CRMAccId);
                var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountCustomerDetailId == CRM_AccId).ToList();

                return ResponseSuccessResult(_crmDocumentEntitiesToViewModels(crmdocuments));

            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }
        #endregion
        // ======================================================================================
    }
}