namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    using System.Web.Mvc;
    using Application.Web.Core;
    using Resources;
    using MvcBreadCrumbs;
    using System.Collections.Generic;
    using JKViewModels.CRM;
    using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;
    using JKApi.Service.ServiceContract.CRM;
    using System.Linq;
    using Common;
    using JKApi.Core;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Web;
    using JKApi.Service.ServiceContract.Customer;
    using JKViewModels.Customer;
    using JKApi.Data.DAL;
    using JKApi.Service.Helper.Extension;
    using JKViewModels.Common;
    using System.Net;
    using Newtonsoft.Json;
    using JKApi.Service;

    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    public class CRMPotentialCustomerController : ViewControllerBase
    {
        #region LifeCycle
        public readonly ICRM_Service _crmService;
        private bool _hasNewData;

        public CRMPotentialCustomerController(ICacheProvider cacheProivder, ICRM_Service ICRMService, ICustomerService customerservice, ICommonService commonService)
        {
            _cacheProvider = cacheProivder;
            _crmService = ICRMService;
            CustomerService = customerservice;
            _commonService = commonService;
            _hasNewData = false;
        }

        #endregion

        #region CRMLeadCustomerController > Helpers

        private SelectList LeadStageStatusList
        {
            get
            {
                var accountstagestatus = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_LeadNeedsToCallOrSetupMeeting ||
                    x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_JunkLead);
                var items = new List<SelectListItem>();
                foreach (var stagestatus in accountstagestatus)
                {
                    var item = new SelectListItem
                    {
                        Value = stagestatus.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(stagestatus.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(stagestatus.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        private SelectList QualifiedLeadStageStatusList
        {
            get
            {
                var accountstagestatus = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_IntialCommunication ||
                    x.Name == Constants.Key_FvPresentation ||
                    x.Name == Constants.Key_Bidding ||
                    x.Name == Constants.Key_PdAppointment ||
                x.Name == Constants.Key_FollowUp ||
                x.Name == Constants.Key_Close);
                var items = new List<SelectListItem>();
                foreach (var stagestatus in accountstagestatus)
                {
                    var item = new SelectListItem
                    {
                        Value = stagestatus.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(stagestatus.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(stagestatus.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        private SelectList IndustryTypeList
        {
            get
            {
                var industryTypes = _crmService.GetAll_CRM_IndustryType();
                var items = new List<SelectListItem>();
                foreach (var industryType in industryTypes)
                {
                    var item = new SelectListItem
                    {
                        Value = industryType.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(industryType.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(industryType.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        private SelectList ProviderSourceList
        {
            get
            {
                var providersources = _crmService.GetAll_CRM_ProviderSource();
                var items = new List<SelectListItem>();
                foreach (var providerSource in providersources)
                {
                    var item = new SelectListItem
                    {
                        Value = providerSource.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(providerSource.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(providerSource.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        private SelectList ProviderTypeList
        {
            get
            {
                var providerTypes = _crmService.GetAll_CRM_ProviderType();
                var items = new List<SelectListItem>();
                foreach (var providerType in providerTypes)
                {
                    var item = new SelectListItem
                    {
                        Value = providerType.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(providerType.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(providerType.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        #endregion

        #region Potentials


        public ActionResult Index()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMPotentialCustomer", new { area = "CRM" }), "Potential");

            //Passing 3 as a choice parameter
            //var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId);

            // Select list   
            ViewBag.UserId = SelectedUserId;
            ViewBag.UserList = new SelectList(_crmService.Get_AuthUserLogin(), "UserId", "FirstName");
            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            //ViewBag.State = new SelectList(CRMUtils.GetStateDescription<CRMEnums.State>());
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", "");
            ViewBag.CallTime = new SelectList(CRMUtils.GetBestTimeList());
            ViewBag.ServiceTypeList = new SelectList(CustomerService.GetServiceTypeList().Where(w => w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.contracttype == 1).OrderBy(x => x.name), "ServiceTypeListId", "name");

            var ServiceTypeListModel = _crmService.GetServiceTypeList().ToList();
            if (ServiceTypeListModel != null)
            {
                ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
            }
            var FrequencyListModel = _crmService.GetFrequencyList().ToList();
            if (FrequencyListModel != null)
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
            }

            var CleanFrequencyListModel = _crmService.GetCleanFrequencyList().ToList();
            if (CleanFrequencyListModel != null)
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
            }

            return View();
        }

        #endregion

        #region CRMPotentialCustomerController > HttpRequest

        [HttpGet]
        public ActionResult GetIndex()
        {
            //Passing 3 as a choice parameter
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId);
            return Json(new
            {
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult PotentialCustomerSearch(int type = 0, int usr = 0)
        {
            //Passing 3 as a choice parameter
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, type, usr, SelectedRegionId);
            return Json(new
            {
                success = true,
                result = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        #region CRMPotentialCustomerController > Get Potential Detail
        [HttpGet]
        public JsonResult GetPotentialDetail(int accountId)
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
            var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(accountId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();


            var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).Where(x => x.CreatedBy == LoginUserId).ToList();
            var accountNote = _crmService.GetCRM_Note_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            var leadschedule = _crmService.GetAll_CRM_Schedule().Where(x => x.CRM_AccountCustomerDetailId == accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();


            var accountInitialCommunication = _crmService.Get_InitialCommunication_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
            var accountFvPresentation = _crmService.Get_fvPresentation_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);

            var accountBidding = _crmService.Get_Bidding_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
            var accountPdAppointment = _crmService.Get_PdAppointment_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
            var accountFollowUp = _crmService.Get_FollowUp_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);
            var accountClose = _crmService.Get_Close_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId);



            if (account != null)
            {
                accountCustomerDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomerDetail);
                accountCustomerDetailViewModel.FirstName = account.FirstName;
                accountCustomerDetailViewModel.LastName = account.LastName;
                accountCustomerDetailViewModel.MiddleInitial = account.MiddleInitial;
                accountCustomerDetailViewModel.ContactName = account.ContactName;
                accountCustomerDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountCustomerDetailViewModel.EmailAddress = account.EmailAddress;

                /* accountCustomerDetailViewModel.ScheduleDateFormat = accountCustomerDetailViewModel.ScheduleDate != null ? accountCustomerDetailViewModel.ScheduleDate.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture) : "";
                 accountCustomerDetailViewModel.ScheduleDateFormat = accountCustomerDetailViewModel.ScheduleDate != null ? accountCustomerDetailViewModel.ScheduleDate.ToString() : " ";      */
                accountCustomerDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountCustomerDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountCustomerDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountCustomerDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;

                // Activity
                if (accountActivity != null)
                {
                    accountCustomerActivityViewModels = CRMAutoMapper.CrmActivityEntitiesToViewModels(accountActivity);
                    foreach (var activityViewModel in accountCustomerActivityViewModels)
                    {
                        activityViewModel.ActivityTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType)));
                        if (activityViewModel.OutComeType != null)
                            activityViewModel.OutComeTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType)));
                    }
                }


                //Note
                if (accountNote != null)
                    accountCustomerNoteViewModels = CRMAutoMapper.CrmNoteEntitiesToViewModels(accountNote);

                //Document
                if (accountDocument != null)
                    accountCustomerDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);

                //Initial Communication
                if (accountInitialCommunication != null)
                    accountCustomerInitialViewModel = CRMAutoMapper.CrmInitialEntityToViewModel(accountInitialCommunication);
                else
                {
                    accountCustomerInitialViewModel = null;
                }

                //Fv Presentation
                if (accountFvPresentation != null)
                    accountCustomerFvPresentationViewModel = CRMAutoMapper.CrmFvPresentationEntityToViewModel(accountFvPresentation);
                else
                {
                    accountCustomerFvPresentationViewModel = null;
                }

                //Bidding
                if (accountBidding != null)
                {
                    accountCustomerBiddingViewModel = CRMAutoMapper.CrmBiddingEntityToViewModel(accountBidding);
                    bidContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_BiddingId == accountBidding.CRM_BiddingId));
                    bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                }
                else
                {
                    accountCustomerBiddingViewModel = null;
                }

                //PdAppointment
                if (accountPdAppointment != null)
                {
                    accountCustomerPdAppointmentViewModel = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(accountPdAppointment);
                    pdContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
                    pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                    if (pdSch != null)
                        pdSch.PurposeId = CRMUtils.SchedulePurposIndex(pdSch.Title);
                }
                else
                {
                    accountCustomerPdAppointmentViewModel = null;
                }
                //Follow-Up
                if (accountFollowUp != null)
                {
                    accountCustomerFollowUpViewModel = CRMAutoMapper.CrmFollowupEntityToViewModel(accountFollowUp);
                    followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                    if (followSch != null)
                        followSch.PurposeId = CRMUtils.SchedulePurposIndex(pdSch.Title);

                }
                else
                {
                    accountCustomerFollowUpViewModel = null;
                }
                if (accountClose != null)
                {
                    accountCustomerSoldViewModel = CRMAutoMapper.CrmCloseEntityToViewModel(accountClose);
                }
                else
                {
                    accountCustomerSoldViewModel = null;
                }
            }

            return Json(new
            {
                success = true,
                result = accountCustomerDetailViewModel,
                activity = accountCustomerActivityViewModels,
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
                pdContact = pdContact,
                pdSchedule = pdSch,
                followSchedule = followSch

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion   

        [HttpGet]
        public ActionResult GetLeadSchedules(int id)
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();

            #region Calendar

            //Schedule

            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
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

            return Json(new
            {
                success = true,
                schedule = accountCustomerScheduleViewModels,
                result = accountCustomerScheduleCalendar

            }, JsonRequestBehavior.AllowGet);
        }

        #region CRMPotentialCustomerController > Save CustomerAccountSummary 

        [HttpPost]
        public ActionResult SaveCustomerAccountSummary(FormCollection summaryData)
        {
            if (summaryData == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
                          $"CRM_AccountCustomerDetailId={summaryData["CRM_AccountCustomerDetailId"]}, " +
                          $"CRM_StageStatus={summaryData["StageStatus"]}, " +
                          $"CRM_ProviderType={summaryData["ProviderType"]}, " +
                          $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
                          $"BudgetAmount={summaryData["BudgetAmount"]}");

            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(int.Parse(summaryData["CRM_AccountCustomerDetailId"]));
            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(summaryData["CRM_AccountId"]));
            if (customerAccountDetail != null && accountcustomer != null)
            {
                customerAccountDetail.BudgetAmount = Convert.ToDecimal(summaryData["BudgetAmount"]);
                customerAccountDetail.ModifiedBy = LoginUserId;

                accountcustomer.StageStatus = int.Parse(summaryData["StageStatus"]);
                accountcustomer.ProviderType = int.Parse(summaryData["ProviderType"]);
                accountcustomer.ProviderSource = int.Parse(summaryData["ProviderSource"]);
                accountcustomer.ModifiedBy = LoginUserId;

                accountcustomer = _crmService.SaveCRM_Account(accountcustomer);
                customerAccountDetail = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);

                var customerAccountDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(customerAccountDetail);
                customerAccountDetailViewModel.StageStatus = (StageStatusType)accountcustomer.StageStatus.Value;
                customerAccountDetailViewModel.ProviderSource = (AccountSourceProvider)accountcustomer.ProviderSource.Value;
                customerAccountDetailViewModel.ProviderType = (AccountProviderType)accountcustomer.ProviderType.Value;

                _hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = customerAccountDetailViewModel

                });
            }
            else
            {
                NLogger.Error("CustomerAccountDetail does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail does not exist" });
            }

        }

        #endregion

        #region CRMPotentialCustomerController > Save CustomerActivitySummary 

        [HttpPost]
        public ActionResult SaveCustomerActivitySummary(FormCollection activityData)
        {
            if (activityData == null)
            {
                NLogger.Error("Requested activityData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            DateTime timestamp = DateTime.ParseExact(activityData["TimeStamp"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            NLogger.Debug($"CRM_AccountCustomerDetailId={activityData["CRM_AccountCustomerDetailId"]}, " +
                          $"Note={activityData["Note"]}, " +
                          $"ActivityType={activityData["ActivityType"]}, " +
                           $"OutComeType={activityData["OutComeType"]}, " +
                          $"TimeStamp={timestamp}");

            var crmActivityViewModel = new CRMActivityViewModel();
            crmActivityViewModel.ActivityType = Convert.ToInt32(activityData["ActivityType"]);
            if (Convert.ToInt32(activityData["ActivityType"]) == 1)
                crmActivityViewModel.OutComeType = Convert.ToInt32(activityData["OutComeType"]);
            crmActivityViewModel.Note = activityData["Note"];
            crmActivityViewModel.TimeStamp = timestamp;
            crmActivityViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(activityData["CRM_AccountCustomerDetailId"]);
            crmActivityViewModel.CreatedBy = LoginUserId;

            var activity = _crmService.SaveCRM_Activity(CRMAutoMapper.CrmActivityEntityToViewModel(crmActivityViewModel));
            var activityViewModel = CRMAutoMapper.CrmActivityViewModelToEntity(activity);
            activityViewModel.ActivityTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType)));
            if (activityViewModel.OutComeType != null)
                activityViewModel.OutComeTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType)));

            return Json(new
            {
                success = true,
                result = activityViewModel,
            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save CustomerScheduleSummary

        [HttpPost]
        public ActionResult SaveCustomerScheduleSummary(FormCollection scheduleData)
        {

            if (scheduleData == null)
            {
                NLogger.Error("Requested scheduleData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountCustomerDetailId={scheduleData["AccountDetailId"]}, " +
                         $"Title={scheduleData["input_scheduletitle"]}, " +
                         $"Description={scheduleData["input_scheduledescription"]}, " +
                         $"StartDate={scheduleData["input_schedulestartdate"]}," +
                         $"ScheduleType={scheduleData["input_scheduletype"]}," +
                         $"EndDate={scheduleData["input_scheduleenddate"]}");

            var vm = new CRMScheduleViewModel();
            vm.CRM_AccountCustomerDetailId = Convert.ToInt32(scheduleData["AccountDetailId"]);
            vm.Title = scheduleData["input_scheduletitle"];
            vm.Description = scheduleData["input_scheduledescription"];
            vm.Location = scheduleData["input_schedulelocation"];
            vm.StartDate = scheduleData["input_schedulestartdate"] != "" ? Convert.ToDateTime(scheduleData["input_schedulestartdate"] + " " + scheduleData["input_schedulestarttime"]) : DateTime.Now;
            vm.EndDate = scheduleData["input_scheduleenddate"] != "" ? Convert.ToDateTime(scheduleData["input_scheduleenddate"] + " " + scheduleData["input_scheduleendtime"]) : DateTime.Now;
            vm.CRM_ScheduleTypeId = Convert.ToInt32(scheduleData["input_scheduletype"]);
            //vm.IsAllDay = bool.TryParse(frm["checkbox_alldayevent"], out isall) ? isall : false;
            vm.CreatedBy = this.LoginUserId;
            var newSchedule = CRMAutoMapper.CrmScheduleViewModelToEntity(vm);
            newSchedule = _crmService.SaveCRM_Schedule(newSchedule);

            return Json(new { success = true, id = newSchedule.CRM_AccountCustomerDetailId });



            /* #region Calendar

             var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();
             var accountCustomerStageScheduleViewModels = new List<CRMStageStatusScheduleViewModel>();
             var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

             var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(newSchedule.CRM_AccountCustomerDetailId).ToList();
             var accountStageStatusSchedules = _crmService.GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById(newSchedule.CRM_AccountCustomerDetailId).ToList();

             //Schedule 
             if (accountSchedule != null)
             {
                 accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                 foreach (var accountschedule in accountCustomerScheduleViewModels)
                 {
                     CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                     calendar.title = accountschedule.Title;
                     // calendar.start = accountschedule.StartDate.Value.ToString("s");
                     accountCustomerScheduleCalendar.Add(calendar);
                 }
             }
             //StageStatus Schedules
             if (accountStageStatusSchedules != null)
             {
                 accountCustomerStageScheduleViewModels = CRMAutoMapper.CrmStageStatusScheduleEntitiesToViewModels(accountStageStatusSchedules);
                 foreach (var stagestatusviewmodel in accountCustomerStageScheduleViewModels)
                 {
                     if (stagestatusviewmodel.Schedule2 != null)
                     {
                         stagestatusviewmodel.Schedule2Format = stagestatusviewmodel.Schedule2.Value.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                         CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                         calendar.title = stagestatusviewmodel.Purpose2 == 1 ? "Call Back" : "Meeting";
                         // calendar.start = stagestatusviewmodel.Schedule2.Value.ToString("s");
                         accountCustomerScheduleCalendar.Add(calendar);
                     }

                     if (stagestatusviewmodel.Schedule1 != null)
                     {
                         stagestatusviewmodel.Schedule1Format = stagestatusviewmodel.Schedule1.Value.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                         CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                         calendar.title = stagestatusviewmodel.Purpose1 == 1 ? "Call Back" : "Meeting";
                         // calendar.start = stagestatusviewmodel.Schedule1.Value.ToString("s");
                         accountCustomerScheduleCalendar.Add(calendar);
                     }
                 }
             }
             #endregion

             return Json(new
             {
                 success = true,
                 schedule = accountCustomerScheduleViewModels,
                 calenderDates = accountCustomerScheduleCalendar,
                 stagestatuschedules = accountCustomerStageScheduleViewModels
             });  */
        }
        #endregion

        #region CRMPotentialCustomerController > Save CustomerNote


        [HttpPost]
        public ActionResult SaveAccountCustomerNote(FormCollection noteData)
        {
            if (noteData == null)
            {
                NLogger.Error("Requested noteData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountCustomerDetailId={noteData["CRM_AccountCustomerDetailId"]}, " +
                         $"Title={noteData["Title"]}, " +
                         $"Description={noteData["Description"]} ");

            var noteViewModel = new CRMNoteViewModel();

            noteViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(noteData["CRM_AccountCustomerDetailId"]);
            noteViewModel.Title = noteData["Title"];
            noteViewModel.Description = noteData["Description"];
            noteViewModel.CreatedBy = LoginUserId;

            var note = _crmService.SaveCRM_Note(CRMAutoMapper.CrmNoteViewModelToEntity(noteViewModel));

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmNoteEntityToViewModel(note)
            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save CustomerContactInfo 

        [HttpPost]
        public ActionResult SaveCustomerContactInfo(FormCollection contactInfoData)
        {
            if (contactInfoData == null)
            {
                NLogger.Error("Requested SaveCustomerContactInfo is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountCustomerDetailId={contactInfoData["CRM_AccountCustomerDetailId"]}, " +
                        $"CRM_AccountId={contactInfoData["CRM_AccountId"]}, " +
                        $"ContactName={contactInfoData["ContactName"]}," +
                        $"Title={contactInfoData["Title"]}, " +
                        $"PhoneNumber={contactInfoData["PhoneNumber"]}, " +
                        $"EmailAddress={contactInfoData["EmailAddress"]}," +
                        $"CompanyName={contactInfoData["CompanyName"]}, " +
                        $"IndustryType={contactInfoData["IndustryType"]}, " +
                        $"NumberOfLocations={contactInfoData["NumberOfLocations"]}," +
                        $"CompanyPhoneNumber={contactInfoData["CompanyPhoneNumber"]}, " +
                        $"CompanyFaxNumber={contactInfoData["CompanyFaxNumber"]}, " +
                        $"CompanyEmailAddress={contactInfoData["CompanyEmailAddress"]}," +
                        $"CompanyAddressLine1={contactInfoData["CompanyAddressLine1"]}, " +
                        $"CompanyAddressLine2={contactInfoData["CompanyAddressLine2"]}, " +
                        $"CompanyCity={contactInfoData["CompanyCity"]}," +
                        $"CompanyCounty={contactInfoData["CompanyCounty"]}," +
                        $"CompanyState={contactInfoData["CompanyState"]}," +
                        $"CompanyZipCode={contactInfoData["CompanyZipCode"]}," +
                        $"SqFt={contactInfoData["Sqft"]}," +
                        $"SalesVolume={contactInfoData["SalesVolume"]}," +
                        $"LineofBusiness={contactInfoData["LineofBusiness"]}," +
                        $"CompnayWebSite={contactInfoData["CompnayWebSite"]}");

            var accountcustomer = _crmService.GetCRM_AccountbyId(Convert.ToInt32(contactInfoData["CRM_AccountId"]));
            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(Convert.ToInt32(contactInfoData["CRM_AccountCustomerDetailId"]));
            if (accountcustomer != null && customerAccountDetail != null)
            {
                accountcustomer.CRM_AccountId = Convert.ToInt32(contactInfoData["CRM_AccountId"]);
                accountcustomer.ContactName = contactInfoData["ContactName"];
                //accountcustomer.LastName = contactInfoData["LastName"];
                accountcustomer.PhoneNumber = contactInfoData["PhoneNumber"];
                accountcustomer.EmailAddress = contactInfoData["EmailAddress"];
                accountcustomer.ModifiedBy = LoginUserId;

                customerAccountDetail.Title =Convert.ToString(contactInfoData["Title"]);
                customerAccountDetail.CRM_AccountCustomerDetailId = Convert.ToInt32(contactInfoData["CRM_AccountCustomerDetailId"]);
                customerAccountDetail.CRM_AccountId = Convert.ToInt32(contactInfoData["CRM_AccountId"]);
                customerAccountDetail.CompanyName = contactInfoData["CompanyName"];
                customerAccountDetail.AccountTypeListId = (Convert.ToString(contactInfoData["IndustryType"]) != "" ? Convert.ToInt32(contactInfoData["IndustryType"]) : 0);
                customerAccountDetail.NumberOfLocations = Convert.ToInt32(contactInfoData["NumberOfLocations"]);
                customerAccountDetail.CompanyPhoneNumber = contactInfoData["CompanyPhoneNumber"];
                customerAccountDetail.CompanyFaxNumber = contactInfoData["CompanyFaxNumber"];
                customerAccountDetail.CompanyWebSite = contactInfoData["CompnayWebSite"];
                customerAccountDetail.CompanyEmailAddress = contactInfoData["CompanyEmailAddress"];
                customerAccountDetail.CompanyAddressLine1 = contactInfoData["CompanyAddressLine1"];
                customerAccountDetail.CompanyAddressLine2 = contactInfoData["CompanyAddressLine2"];
                customerAccountDetail.CompanyCity = contactInfoData["CompanyCity"];
                customerAccountDetail.CompanyCounty = contactInfoData["CompanyCounty"];
                customerAccountDetail.CompanyState = contactInfoData["CompanyState"];
                customerAccountDetail.CompanyZipCode = contactInfoData["CompanyZipCode"];

                customerAccountDetail.SqFt = Convert.ToString(contactInfoData["Sqft"]);
                customerAccountDetail.SalesVolume = Convert.ToString(contactInfoData["SalesVolume"]);
                customerAccountDetail.LineofBusiness = Convert.ToString(contactInfoData["LineofBusiness"]);

                customerAccountDetail.ModifiedBy = LoginUserId;

                var account = _crmService.SaveCRM_Account(accountcustomer);
                var accountCustomer = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);
                _hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = CRMAutoMapper.CrmAccountEntityToViewModel(account),
                    accountcustomer = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomer)
                });
            }

            else
            {
                NLogger.Error("CustomerAccountDetail and CustomerAccount does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail and CustomerAccount does not exist" });
            }

        }
        #endregion

        #region CRMPotentialCustomerController > Save Initial Communication
        [HttpPost]
        public ActionResult CreateInitialSummaryData(FormCollection initialData)
        {
            if (initialData == null)
            {
                NLogger.Error("Requested SaveInitialSummaryDate is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountId={initialData["CRM_AccountId"]}, " +
                         $"CRM_AccountCustomerDetailId={initialData["CRM_AccountCustomerDetailId"]}, " +
                        $"ContactPerson={initialData["ContactPerson"]}," +
                        $"InterestedInProposal={initialData["InterestedInProposal"]}, " +
                        $"AvailableToMeet={initialData["AvailableToMeet"]} " +
                        $"Purpose={initialData["Purpose"]}" +
                        $"Note={initialData["Note"]}");
            DateTime availabletomeet = DateTime.ParseExact(initialData["AvailableToMeet"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            var crmInital = _crmService.AddNewInitalData(
                Convert.ToInt32(initialData["CRM_AccountId"]),
                Convert.ToInt32(initialData["CRM_AccountCustomerDetailId"]),
                initialData["ContactPerson"],
               Convert.ToInt32(initialData["InterestedInProposal"]),
                availabletomeet,
                Convert.ToInt32(initialData["Purpose"]),
                initialData["Note"], LoginUserId
                );

            var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(initialData["CRM_AccountId"]));
            var stageStatusViewModel = _crmService.GetCRM_StageStatusSchedules_ByAccountCustomerDetailById(Convert.ToInt32(initialData["CRM_AccountCustomerDetailId"])).FirstOrDefault();


            stageStatusViewModel.CRM_InitialCommunicationId = crmInital.CRM_InitialCommunicationId;
            stageStatusViewModel.Schedule1 = DateTime.ParseExact(initialData["AvailableToMeet"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
            stageStatusViewModel.Purpose1 = Convert.ToInt32(initialData["Purpose"]);
            stageStatusViewModel.Title = account.FirstName + (stageStatusViewModel.Purpose1 == 1 ? " Call Back" : " Meeting");
            var stagestatusschedule = _crmService.SaveCRM_StageStatusSchedule(stageStatusViewModel);

            #region Calendar
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();
            var accountCustomerStageScheduleViewModels = new List<CRMStageStatusScheduleViewModel>();
            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId((int?)crmInital.CRM_AccountCustomerDetailId).ToList();
            var accountStageStatusSchedules = _crmService.GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById((int?)crmInital.CRM_AccountCustomerDetailId).ToList();


            //Schedule 
            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountCustomerScheduleViewModels)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    calendar.title = accountschedule.Title;
                    // calendar.start = accountschedule.StartDate.Value.ToString("s");
                    accountCustomerScheduleCalendar.Add(calendar);
                }
            }

            //StageStatus Schedules
            if (accountStageStatusSchedules != null)
            {
                accountCustomerStageScheduleViewModels = CRMAutoMapper.CrmStageStatusScheduleEntitiesToViewModels(accountStageStatusSchedules);
                foreach (var stagestatusviewmodel in accountCustomerStageScheduleViewModels)
                {
                    if (stagestatusviewmodel.Schedule2 != null)
                    {
                        stagestatusviewmodel.Schedule2Format = stagestatusviewmodel.Schedule2.Value.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = stagestatusviewmodel.Title;
                        // calendar.start = stagestatusviewmodel.Schedule2.Value.ToString("s");
                        accountCustomerScheduleCalendar.Add(calendar);
                    }

                    if (stagestatusviewmodel.Schedule1 != null)
                    {
                        stagestatusviewmodel.Schedule1Format = stagestatusviewmodel.Schedule1.Value.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = stagestatusviewmodel.Title;
                        //calendar.start = stagestatusviewmodel.Schedule1.Value.ToString("s");
                        accountCustomerScheduleCalendar.Add(calendar);
                    }
                }
            }
            #endregion


            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmInitialEntityToViewModel(crmInital),
                schedule = accountCustomerScheduleViewModels,
                calenderDates = accountCustomerScheduleCalendar,
                stagestatuschedules = accountCustomerStageScheduleViewModels,
                stageschedule = CRMAutoMapper.CrmStageStatusScheduleEntityToViewModel(stagestatusschedule),
                stage = (int)StageStatusType.FvPresentation

            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save Fv Presentation
        [HttpPost]
        public ActionResult CreateFvPresentationSummaryData(FormCollection fvSummaryData)
        {
            if (fvSummaryData == null)
            {
                NLogger.Error("Requested fvPresentationSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountId={fvSummaryData["CRM_AccountId"]}, " +
                        $"CRM_AccountCustomerDetailId={fvSummaryData["CRM_AccountCustomerDetailId"]}, " +
                       $"MeasureContactPerson={fvSummaryData["MeasureContactPerson"]}," +
                       $"MeasureFacility={fvSummaryData["MeasureFacility"]}, " +
                       $"NumberOfFloors={fvSummaryData["NumberOfFloors"]} " +
                       $"Frequency={fvSummaryData["Frequency"]}" +
                        $"CleaningDay={fvSummaryData["CleaningDay"]}, " +
                       $"ServiceLevel={fvSummaryData["ServiceLevel"]} " +
                       $"CleanFrequency={fvSummaryData["CleanFrequency"]} " +
                       $"Budget={fvSummaryData["Budget"]}" +
                       $"Note={fvSummaryData["Note"]}");

            var crmfvPresentationViewmodel = new CRMFvPresentationViewModel();
            crmfvPresentationViewmodel.CRM_AccountCustomerDetailId = Convert.ToInt32(fvSummaryData["CRM_AccountCustomerDetailId"]);
            crmfvPresentationViewmodel.MeasureContactPerson = fvSummaryData["MeasureContactPerson"];
            crmfvPresentationViewmodel.MeasureFacility = fvSummaryData["MeasureFacility"] != null ? Convert.ToDouble(fvSummaryData["MeasureFacility"]) : 0.0;
            crmfvPresentationViewmodel.NumberOfFloors = fvSummaryData["NumberOfFloors"] != null ? Convert.ToInt32(fvSummaryData["NumberOfFloors"]) : 0;
            crmfvPresentationViewmodel.BillingFrequency = (fvSummaryData["Frequency"] != null && fvSummaryData["Frequency"] != "") ? Convert.ToInt32(fvSummaryData["Frequency"]) : 0;
            crmfvPresentationViewmodel.ServiceTypeListId = fvSummaryData["ServiceType"] != null ? Convert.ToInt32(fvSummaryData["ServiceType"]) : 0;
            crmfvPresentationViewmodel.ServiceLevel = fvSummaryData["ServiceLevel"] != null ? Convert.ToInt32(fvSummaryData["ServiceLevel"]) : 0;
            crmfvPresentationViewmodel.CleanFrequency = Convert.ToInt32(fvSummaryData["CleanFrequency"]);
            crmfvPresentationViewmodel.CleanTime = fvSummaryData["CleanTime"] != null ? Convert.ToInt32(fvSummaryData["CleanTime"]) : 0;
            crmfvPresentationViewmodel.BudgetAmount = fvSummaryData["Budget"] != null ? Convert.ToDecimal(fvSummaryData["Budget"]) : 0;
            crmfvPresentationViewmodel.Note = fvSummaryData["Note"];
            crmfvPresentationViewmodel.CreatedBy = LoginUserId;

            //If clean frequency selected as Week
            if (crmfvPresentationViewmodel.CleanFrequency == 1)
            {
                var checkDayscount = fvSummaryData[6];
                if (checkDayscount.Count() >= 1)
                {
                    string[] cleaningDay = fvSummaryData[6].Split(','); ;
                    foreach (string item in cleaningDay)
                    {
                        int value = int.Parse(item);
                        if (value == 1) { crmfvPresentationViewmodel.Mon = true; }

                        if (value == 2) { crmfvPresentationViewmodel.Tue = true; }

                        if (value == 3) { crmfvPresentationViewmodel.Wed = true; }

                        if (value == 4) { crmfvPresentationViewmodel.Thu = true; }

                        if (value == 5) { crmfvPresentationViewmodel.Fri = true; }

                        if (value == 6) { crmfvPresentationViewmodel.Sat = true; }

                        if (value == 7) { crmfvPresentationViewmodel.Sun = true; }
                    }
                }
            }

            var crmfvpresentation = _crmService.SaveCRM_FvPresentation(CRMAutoMapper.CrmFvPresentationViewModelToEntity(crmfvPresentationViewmodel));
            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(fvSummaryData["CRM_AccountId"]));
            //update AccountCustomer            
            if (accountcustomer != null)
            {
                accountcustomer.StageStatus = (int)StageStatusType.Bidding;
                accountcustomer.ModifiedBy = LoginUserId;
                _crmService.SaveCRM_Account(accountcustomer);
            }
            return Json(new
            {
                success = true,
                fvpresentation = CRMAutoMapper.CrmFvPresentationEntityToViewModel(crmfvpresentation),
                stage = (int)StageStatusType.Bidding
            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save Bidding 
        [HttpPost]
        public ActionResult CreateBiddingSummaryData(FormCollection biddingData)
        {
            if (!(biddingData.Count > 0))
            {
                NLogger.Error("Requested BiddingSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountId={biddingData["CRM_AccountId"]}, " +
                       $"CRM_AccountCustomerDetailId={biddingData["CRM_AccountCustomerDetailId"]}, " +
                      $"IsAnalysisWorkBook={biddingData["IsAnalysisWorkBook"]}," +
                      $"MeetWithPerson={biddingData["MeetWithPerson"]}," +
                      $"ContactPhone={biddingData["ContactPhone"]}," +
                      $"ContactEmail={biddingData["ContactEmail"]}," +

                     $"ScheduleStartDate={biddingData["ScheduleStartDate"]}, " +
                      $"ScheduleStartTime={biddingData["ScheduleStartTime"]}, " +
                       $"ScheduleEndDate={biddingData["ScheduleEndDate"]}, " +
                       $"ScheduleEndTime={biddingData["ScheduleEndTime"]}, " +

                      $"MonthlyPrice={biddingData["MonthlyPrice"]}, " +
                      $"PriceApproved={biddingData["PriceApproved"]} " +
                      $"IfBidOver={"1"}" +
                       $"IncludePrice={biddingData["IncludePrice"]} " +
                      $"Note={biddingData["Note"]}");

            var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();
            var crmBiddingViewModel = new CRMBiddingViewModel();

            crmBiddingViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(biddingData["CRM_AccountCustomerDetailId"]);
            crmBiddingViewModel.AnalysisWorkBook = bool.Parse(biddingData["IsAnalysisWorkBook"]);
            crmBiddingViewModel.MonthlyPrice = Convert.ToDecimal(biddingData["MonthlyPrice"]);
            crmBiddingViewModel.PriceApproved = Convert.ToInt32(biddingData["PriceApproved"]);
            crmBiddingViewModel.IfBidOver = true;
            crmBiddingViewModel.IncludePrice = Convert.ToDecimal(biddingData["IncludePrice"]);
            crmBiddingViewModel.Note = biddingData["Note"];
            crmBiddingViewModel.CreatedBy = LoginUserId;

            var crmbidding = _crmService.SaveCRM_Bidding(CRMAutoMapper.CrmBiddingViewModelToEntity(crmBiddingViewModel));
            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(biddingData["CRM_AccountId"]));
            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(int.Parse(biddingData["CRM_AccountId"]));

            //Save Contact
            var contactViewModel = new CRMContactViewModel();
            contactViewModel.ContactName = biddingData["MeetWithPerson"];
            contactViewModel.ContactEmail = biddingData["ContactEmail"];
            contactViewModel.ContactPhone = biddingData["ContactPhone"];
            contactViewModel.CRM_AccountCustomerDetailId = crmbidding.CRM_AccountCustomerDetailId;
            contactViewModel.CRM_BiddingId = crmbidding.CRM_BiddingId;
            contactViewModel.CreatedBy = LoginUserId;

            var contact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.SaveCRM_Contact(CRMAutoMapper.CrmContactViewModelToEntity(contactViewModel)));

            //Save Stage Status Schedule
            var schedulevm = new CRMScheduleViewModel();
            schedulevm.CRM_AccountCustomerDetailId = crmbidding.CRM_AccountCustomerDetailId;

            schedulevm.StartDate = biddingData["ScheduleStartDate"] != "" ? Convert.ToDateTime(biddingData["ScheduleStartDate"] + " " + biddingData["ScheduleStartTime"]) : DateTime.Now;
            schedulevm.EndDate = biddingData["ScheduleEndDate"] != "" ? Convert.ToDateTime(biddingData["ScheduleEndDate"] + " " + biddingData["ScheduleEndTime"]) : DateTime.Now;
           // schedulevm.Title = accountcustomer.CRM_AccountId + " - " + "FV" + "" + CRMUtils.SchedulePurpose(initial.PURPOSE) + " - " + accountcustomer.FirstName + " - " + contact.ContactName+ " - " + customerAccountDetail.CompanyPhoneNumber;
            schedulevm.Location = customerAccountDetail.CompanyAddressLine1;
            schedulevm.CRM_AssignToId = accountcustomer.AssigneeId;
            schedulevm.CRM_StageStatusType = (int)StageStatusType.Bidding;
            schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
            schedulevm.RegionId = SelectedRegionId;
            schedulevm.CreatedBy = LoginUserId;
            var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm));

            //update AccountCustomer            
            if (accountcustomer != null)
            {
                accountcustomer.StageStatus = (int)StageStatusType.PdAppointment;
                accountcustomer.ModifiedBy = LoginUserId;
                _crmService.SaveCRM_Account(accountcustomer);
            }

            var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(crmbidding.CRM_AccountCustomerDetailId).ToList();
            //Document
            if (accountDocument != null)
                accountCustomerDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmBiddingEntityToViewModel(crmbidding),
                contact = contact,
                stageStatusSchedule = CRMAutoMapper.CrmScheduleEntityToViewModel(schedule),
                document = accountCustomerDocumentViewModels,
                stage = (int)StageStatusType.PdAppointment
            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save Pd Appointment
        [HttpPost]
        public ActionResult CreatePdAppointmentSummaryData(FormCollection pdSummaryData)
        {
            if (!(pdSummaryData.Count > 0))
            {
                NLogger.Error("Requested pdSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"CRM_AccountId={pdSummaryData["CRM_AccountId"]}, " +
                      $"CRM_AccountCustomerDetailId={pdSummaryData["CRM_AccountCustomerDetailId"]}, " +
                     $"MeetWithPerson={pdSummaryData["DMeetWithPerson"]}," +
                     $"ContactPhone={pdSummaryData["DContactPhone"]}," +
                     $"ContactEmail={pdSummaryData["DContactEmail"]}," +
                     $"MeetWithDecisionMaker={pdSummaryData["MeetWithDecisionMaker"]}, " +
                     $"PresentProposal={pdSummaryData["PresentProposal"]} " +
                     $"OverComeObjection={pdSummaryData["OverComeObjection"]}" +
                     $"Comment={pdSummaryData["Comment"]} " +
                     $"ProposalDetail={pdSummaryData["ProposalDetail"]}, " +           
                        $"CallBackStartDate={pdSummaryData["CallBackStartDate"]}, " +
                     $"CallBackStartTime={ pdSummaryData["CallBackStartTime"]}, " +
                     $"CallBackEndDate={ pdSummaryData["CallBackEndDate"]}, " +
                     $"CallBackEndTime={ pdSummaryData["CallBackEndTime"]}, " +
                     $"CallBack_Purpose={pdSummaryData["CallBack_Purpose"]}" +
                     $"Note={pdSummaryData["Note"]}");

           
            //if (pdSummaryData["CallBack_Schedule"] != null)
            //    CallBackDate = DateTime.ParseExact(pdSummaryData["CallBack_Schedule"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            var PdViewModel = new CRMPdAppointmentViewModel();

            PdViewModel.CRM_AccountCustomerDetailId = int.Parse(pdSummaryData["CRM_AccountCustomerDetailId"]);
            PdViewModel.MeetPersonName = pdSummaryData["DMeetWithPerson"];
            PdViewModel.MeetDecisionMaker = bool.Parse(pdSummaryData["MeetWithDecisionMaker"]);
            PdViewModel.PresentProposal = bool.Parse(pdSummaryData["PresentProposal"]);
            PdViewModel.OverComeObjections = Convert.ToBoolean(int.Parse(pdSummaryData["OverComeObjection"]));
            PdViewModel.Comment = pdSummaryData["Comment"];
            PdViewModel.EnteredProposalDetail = bool.Parse(pdSummaryData["ProposalDetail"]);
            PdViewModel.Note = pdSummaryData["Note"];
            PdViewModel.CreatedBy = LoginUserId;
            var pdappointment = _crmService.SaveCRM_PdAppointment(CRMAutoMapper.CrmPdAppointmentViewModelToEntity(PdViewModel));
            var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(pdSummaryData["CRM_AccountId"]));

            var contact = new CRMContactViewModel();
            if (pdappointment.MeetDecisionMaker == false)
            {
                var contactViewModel = new CRMContactViewModel();
                contactViewModel.ContactName = pdSummaryData["DMeetWithPerson"];
                contactViewModel.ContactEmail = pdSummaryData["DContactEmail"];
                contactViewModel.ContactPhone = pdSummaryData["DContactPhone"];
                contactViewModel.CRM_AccountCustomerDetailId = pdappointment.CRM_AccountCustomerDetailId;
                contactViewModel.CRM_PdAppointmentId = pdappointment.CRM_PdAppointmentId;

                contact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.SaveCRM_Contact(CRMAutoMapper.CrmContactViewModelToEntity(contactViewModel)));
            }

            var schedulevm = new CRMScheduleViewModel();

            schedulevm.CRM_AccountCustomerDetailId = pdappointment.CRM_AccountCustomerDetailId;
            schedulevm.Title = CRMUtils.SchedulePurpose(Convert.ToInt32(pdSummaryData["CallBack_Purpose"])); 
           
            schedulevm.StartDate = pdSummaryData["CallBackStartDate"] != "" ? Convert.ToDateTime(pdSummaryData["CallBackStartDate"] + " " + pdSummaryData["CallBackStartTime"]) : DateTime.Now;
            schedulevm.EndDate = pdSummaryData["CallBackEndDate"] != "" ? Convert.ToDateTime(pdSummaryData["CallBackEndDate"] + " " + pdSummaryData["CallBackEndTime"]) : DateTime.Now;
            schedulevm.CRM_StageStatusType = (int)StageStatusType.PdAppointment;
            schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
            schedulevm.RegionId = SelectedRegionId;
            schedulevm.CreatedBy = LoginUserId;
            var scheduleVM = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm)));
            scheduleVM.PurposeId = Convert.ToInt32(pdSummaryData["CallBack_Purpose"]);



            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(pdSummaryData["CRM_AccountId"]));
            //update AccountCustomer            
            if (accountcustomer != null)
            {
                accountcustomer.StageStatus = (int)StageStatusType.FollowUp;
                accountcustomer.ModifiedBy = LoginUserId;
                _crmService.SaveCRM_Account(accountcustomer);
            }

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(pdappointment),
                contact = contact,
                schedule = scheduleVM,
                stage = (int)StageStatusType.FollowUp

            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save Follow-up
        [HttpPost]
        public ActionResult CreateFollowUpSummaryData(FormCollection followSummaryData)
        {
            if (!(followSummaryData.Count > 0))
            {
                NLogger.Error("Requested followSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            NLogger.Debug($"CRM_AccountId={followSummaryData["CRM_AccountId"]}, " +
                     $"CRM_AccountCustomerDetailId={followSummaryData["CRM_AccountCustomerDetailId"]}, " +
                    $"AvailableToMeet={followSummaryData["AvailableToMeet"]}," +
                    $"AvailableToMeetPurpose={followSummaryData["AvailableToMeetPurpose"]}, " +
                    $"CloseType={followSummaryData["CloseType"]}, " +
                    $"MeetAgain={followSummaryData["MeetAgain"]} " +
                    $"PurposeAgain={followSummaryData["PurposeAgain"]}" +
                    $"Note={followSummaryData["Note"]}");


            var followupViewModel = new CRMFollowUpViewModel();
            followupViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(followSummaryData["CRM_AccountCustomerDetailId"]);
            followupViewModel.Close = Convert.ToInt32(followSummaryData["CloseType"]);
            followupViewModel.Note = followSummaryData["Note"];
            followupViewModel.CreatedBy = LoginUserId;

            var followup = _crmService.SaveCRM_FollowUp(CRMAutoMapper.CrmFollowupViewModelToEntity(followupViewModel));
            var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(followSummaryData["CRM_AccountId"]));


            var schedulevm = new CRMScheduleViewModel();

            if (followSummaryData["CloseType"] != "2")
            {
                schedulevm.CRM_AccountCustomerDetailId = Convert.ToInt32(followSummaryData["CRM_AccountCustomerDetailId"]);
                schedulevm.StartDate = DateTime.ParseExact(followSummaryData["MeetAgain"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                schedulevm.Title = CRMUtils.SchedulePurpose(Convert.ToInt32(followSummaryData["PurposeAgain"]));
                schedulevm.CRM_StageStatusType = (int)StageStatusType.FollowUp;
                schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                schedulevm.RegionId = SelectedRegionId;
                schedulevm.CreatedBy = LoginUserId;
                schedulevm = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm)));
                schedulevm.PurposeId = Convert.ToInt32(followSummaryData["CallBack_Purpose"]);
            }


            //update AccountCustomer            
            if (account != null)
            {
                if (followup.Close == 5)
                {
                    account.StageStatus = (int)StageStatusType.QualifiedLead;
                    account.Stage = (int)StageType.Lead;
                    account.ModifiedBy = LoginUserId;
                    _crmService.SaveCRM_Account(account);
                }
                else if (followup.Close == 2)
                {
                    account.StageStatus = (int)StageStatusType.Sold;
                    account.ModifiedBy = LoginUserId;
                    _crmService.SaveCRM_Account(account);
                }
            }


            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmFollowupEntityToViewModel(followup),
                schedule = schedulevm,
                stage = (int)StageStatusType.Sold
            });

        }
        #endregion

        #region CRMPotentialCustomerController > Save Close
        [HttpPost]
        public ActionResult CreateCloseSummaryData(TempDocumentViewModel closeSummaryData)
        {
            if (closeSummaryData == null)
            {
                NLogger.Error("Requested closeSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            NLogger.Debug($"CRM_AccountId={closeSummaryData.CRM_AccountId}, " +
                     $"CRM_AccountCustomerDetailId={closeSummaryData.CRM_AccountCustomerDetailId}, " +
                    $"HaveContractAgreement={closeSummaryData.HaveContractAgreement}," +
                    $"SignedAgreement={closeSummaryData.SignedAgreement}, " +
                    $"DocumentSalesCRM={closeSummaryData.DocumentSalesCRM}, " +
                    $"NotifyAccountPlacement={closeSummaryData.NotifyAccountPlacement} " +
                    $"PropAmount={closeSummaryData.PropAmount}" +
                     $"InitialClean={closeSummaryData.InitialClean}," +
                    $"ContractAmount={closeSummaryData.ContractAmount}, " +
                    $"InitialCleanAmount={closeSummaryData.InitialCleanAmount}, " +
                    $"SignDate={closeSummaryData.SignDate} " +
                    $"StartDate={closeSummaryData.StartDate}" +
                     $"PhoneNumber={closeSummaryData.PhoneNumber} " +
                    $"ContractTerm={closeSummaryData.ContractTerm}" +
                     $"ServiceType={closeSummaryData.ServiceType}," +
                    $"BillingFrequency={closeSummaryData.BillingFrequency}, " +
                    $"CleanTime={closeSummaryData.CleanTime}, " +
                    $"Cleaningday={closeSummaryData.CleanTime} " +
                    $"CleanFrequency={closeSummaryData.CleanFrequency}" +
                    $"Note={closeSummaryData.Note}");


            CRMCloseViewModel closeViewModel = new CRMCloseViewModel();
            closeViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(closeSummaryData.CRM_AccountCustomerDetailId);
            closeViewModel.HaveContractAgreement = Convert.ToBoolean(closeSummaryData.HaveContractAgreement);
            closeViewModel.SignedAgreement = Convert.ToBoolean(closeSummaryData.SignedAgreement);
            closeViewModel.DocumentSalesCRM = Convert.ToBoolean(closeSummaryData.DocumentSalesCRM);
            closeViewModel.NotifyAccountPlacement = Convert.ToBoolean(closeSummaryData.NotifyAccountPlacement);
            closeViewModel.PropAmount = Convert.ToDecimal(closeSummaryData.PropAmount);
            closeViewModel.InitialClean = Convert.ToInt32(closeSummaryData.InitialClean);
            closeViewModel.ContractAmount = Convert.ToDecimal(closeSummaryData.ContractAmount);
            closeViewModel.InitialCleanAmount = Convert.ToDecimal(closeSummaryData.InitialCleanAmount);
            closeViewModel.StartDate = closeSummaryData.StartDate != null ? closeSummaryData.StartDate : DateTime.Now;
            closeViewModel.SignDate = closeSummaryData.SignDate != null ? closeSummaryData.SignDate : DateTime.Now;
            closeViewModel.PhoneNumber = closeSummaryData.PhoneNumber;
            closeViewModel.ContractTerm = closeSummaryData.ContractTerm;
            closeViewModel.ServiceType = Convert.ToInt32(closeSummaryData.ServiceType);
            closeViewModel.BillingFrequency = Convert.ToInt32(closeSummaryData.BillingFrequency);
            closeViewModel.CleanTime = Convert.ToInt32(closeSummaryData.CleanTime);
            closeViewModel.CleanFrequency = Convert.ToInt32(closeSummaryData.CleanFrequency);
            closeViewModel.Note = closeSummaryData.Note;

            var checkDayscount = closeSummaryData.CleaningDay;
            if (checkDayscount.Count() >= 1)
            {
                string[] cleaningDay = closeSummaryData.CleaningDay[0].Split(',');
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

            var close = _crmService.SaveCRM_Close(CRMAutoMapper.CrmCloseViewModelToEntity(closeViewModel));

            //Close Lead Convert into Customer 
            var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(closeSummaryData.CRM_AccountId));
            var customerdetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(account.CRM_AccountId);


            //Customer Details
            FullCustomerViewModel fullCustomerViewModel = new FullCustomerViewModel();
            fullCustomerViewModel.CustomerViewModel.Name = customerdetail.CompanyName;
            fullCustomerViewModel.CustomerViewModel.StatusListId = 38;    //Region Operation = 38
            fullCustomerViewModel.CustomerViewModel.CustomerNo = CustomerService.getCustomerNo();
            fullCustomerViewModel.CustomerViewModel.CreatedDate = DateTime.Now;
            fullCustomerViewModel.CustomerViewModel.CreatedBy = LoginUserId;
            fullCustomerViewModel.CustomerViewModel.IsActive = true;
            fullCustomerViewModel.CustomerViewModel.RegionId = SelectedRegionId;
            fullCustomerViewModel.CustomerViewModel.ParentId = -1;
            fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(fullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();

            ////Main Information
            fullCustomerViewModel.MainContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainContact.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullCustomerViewModel.MainContact.Name = account.FirstName + account.LastName;
            fullCustomerViewModel.MainContact.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainContact.IsActive = true;
            fullCustomerViewModel.MainContact.CreatedBy = "-1";
            fullCustomerViewModel.MainContact = CustomerService.SaveContact(fullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

            fullCustomerViewModel.MainAddress.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainAddress.Address1 = customerdetail.CompanyAddressLine1;
            fullCustomerViewModel.MainAddress.Address2 = customerdetail.CompanyAddressLine2;
            fullCustomerViewModel.MainAddress.City = customerdetail.CompanyCity;
            fullCustomerViewModel.MainAddress.StateName = customerdetail.CompanyState;
            fullCustomerViewModel.MainAddress.PostalCode = customerdetail.CompanyZipCode;

            var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullCustomerViewModel.MainAddress.FullAddress));
            fullCustomerViewModel.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
            fullCustomerViewModel.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());


            if (customerdetail.CompanyState != null)
            {
                int state = CustomerService.GetStateId(customerdetail.CompanyState);
                fullCustomerViewModel.MainAddress.StateListId = state;
            }


            fullCustomerViewModel.MainAddress.IsActive = true;
            fullCustomerViewModel.MainAddress.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainAddress.CreatedBy = -1;
            fullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullCustomerViewModel.MainAddress = CustomerService.SaveAddress(fullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

            fullCustomerViewModel.MainPhone.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullCustomerViewModel.MainPhone.Cell = account.PhoneNumber;
            fullCustomerViewModel.MainPhone.IsActive = true;
            fullCustomerViewModel.MainPhone.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainPhone.CreatedBy = -1;
            fullCustomerViewModel.MainPhone = CustomerService.SavePhone(fullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

            fullCustomerViewModel.MainPhone2.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainPhone2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainPhone2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullCustomerViewModel.MainPhone2.Phone1 = customerdetail.CompanyPhoneNumber;
            fullCustomerViewModel.MainPhone2.Fax = customerdetail.CompanyFaxNumber;
            fullCustomerViewModel.MainPhone2.IsActive = true;
            fullCustomerViewModel.MainPhone2.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainPhone2.CreatedBy = LoginUserId;
            fullCustomerViewModel.MainPhone2 = CustomerService.SavePhone(fullCustomerViewModel.MainPhone2.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();


            fullCustomerViewModel.MainEmail.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullCustomerViewModel.MainEmail.EmailAddress = account.EmailAddress;
            fullCustomerViewModel.MainEmail.IsActive = true;
            fullCustomerViewModel.MainEmail.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainEmail.CreatedBy = "-1";
            fullCustomerViewModel.MainEmail = CustomerService.SaveEmail(fullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

            fullCustomerViewModel.MainEmail2.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.MainEmail2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.MainEmail2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullCustomerViewModel.MainEmail2.EmailAddress = customerdetail.CompanyEmailAddress;
            fullCustomerViewModel.MainEmail2.IsActive = true;
            fullCustomerViewModel.MainEmail2.CreatedDate = DateTime.Now;
            fullCustomerViewModel.MainEmail2.CreatedBy = "-1";
            fullCustomerViewModel.MainEmail2 = CustomerService.SaveEmail(fullCustomerViewModel.MainEmail2.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

            fullCustomerViewModel.Contract.CustomerId = fullCustomerViewModel.CustomerViewModel.CustomerId;
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
            fullCustomerViewModel.Contract.CreatedBy = LoginUserId;
            fullCustomerViewModel.Contract.AccountTypeListId = customerdetail.AccountTypeListId;
            fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

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
            fullCustomerViewModel.ContractDetail.CPIIncrease = false;
            fullCustomerViewModel.ContractDetail.LineNumber = 1;
            fullCustomerViewModel.ContractDetail.SeparateInvoice = false;
            fullCustomerViewModel.ContractDetail.CreatedDate = DateTime.Now;
            fullCustomerViewModel.ContractDetail.CreatedBy = LoginUserId;
            fullCustomerViewModel.ContractDetail = CustomerService.SaveContractDetail(fullCustomerViewModel.ContractDetail.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();


            //Save Customer-Sales-Document 
            if (closeSummaryData.file1 != null)
            {
                if (closeSummaryData.file1.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file2 != null)
            {
                if (closeSummaryData.file2.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file3 != null)
            {
                if (closeSummaryData.file3.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file4 != null)
            {
                if (closeSummaryData.file4.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file5 != null)
            {
                if (closeSummaryData.file5.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file6 != null)
            {
                if (closeSummaryData.file6.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file7 != null)
            {
                if (closeSummaryData.file7.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file8 != null)
            {
                if (closeSummaryData.file8.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file9 != null)
            {
                if (closeSummaryData.file9.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file10 != null)
            {
                if (closeSummaryData.file10.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file11 != null)
            {
                if (closeSummaryData.file11.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }
            if (closeSummaryData.file12 != null)
            {
                if (closeSummaryData.file12.ContentLength > 0) { SaveSalesDocument(fullCustomerViewModel, closeSummaryData); }
            }

            var accountcustomer = _crmService.GetCRM_AccountbyId(closeSummaryData.CRM_AccountId);
            //update AccountCustomer            
            if (accountcustomer != null)
            {
                accountcustomer.StageStatus = (int)StageStatusType.Close;
                accountcustomer.Stage = (int)StageType.Customer;
                accountcustomer.ModifiedBy = LoginUserId;
                _crmService.SaveCRM_Account(accountcustomer);
            }
            if (fullCustomerViewModel.CustomerViewModel.CustomerId > 0)
                _commonService.CommonInsertNotification(7, "", false, fullCustomerViewModel.CustomerViewModel.CustomerId, 1, null, null, null);
            return Json(new
            {
                success = true,
                sold = CRMAutoMapper.CrmCloseEntityToViewModel(close),
                stage = (int)StageStatusType.Close
            }, JsonRequestBehavior.AllowGet);
        }

        public void SaveSalesDocument(FullCustomerViewModel fullviewmodel, TempDocumentViewModel tempViewModel)
        {
            if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
            {
                Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));

            }
            if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));

            }
            if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString()));
            }

            #region Sales file from 1 to 12

            if (tempViewModel.file1 != null)
            {
                if (tempViewModel.file1.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file1.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file1.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file1.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file1.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 1, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file2 != null)
            {
                if (tempViewModel.file2.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file2.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file2.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file2.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file2.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 2, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file3 != null)
            {
                if (tempViewModel.file3.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file3.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file3.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file3.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file3.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 3, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file4 != null)
            {
                if (tempViewModel.file4.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file4.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file4.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file4.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file4.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 4, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file5 != null)
            {
                if (tempViewModel.file5.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file5.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file5.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file5.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file5.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 5, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file6 != null)
            {
                if (tempViewModel.file6.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file6.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file6.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file6.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file6.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 6, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file7 != null)
            {
                if (tempViewModel.file7.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file7.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file7.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file7.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file7.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 7, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file8 != null)
            {
                if (tempViewModel.file8.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file8.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file8.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file8.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file8.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 8, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file9 != null)
            {
                if (tempViewModel.file9.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file9.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file9.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file9.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file9.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 9, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file10 != null)
            {
                if (tempViewModel.file10.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file10.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file10.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file10.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file10.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 10, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file11 != null)
            {
                if (tempViewModel.file11.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file11.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file11.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file11.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file11.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 11, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            if (tempViewModel.file12 != null)
            {
                if (tempViewModel.file12.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(tempViewModel.file12.FileName);

                    int _FileSize = Path.GetFileName(tempViewModel.file12.FileName).Length;
                    string _FileExt = Path.GetFileName(tempViewModel.file12.FileName).Split('.').Last();
                    string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), fullviewmodel.CustomerViewModel.CustomerId.ToString(), _SFileName);
                    tempViewModel.file12.SaveAs(_path);

                    String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + fullviewmodel.CustomerViewModel.CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(fullviewmodel.CustomerViewModel.CustomerId.ToString()), 1, 12, _FilePath, _FileName, _FileExt, _FileSize);
                }
            }
            #endregion

        }

        #endregion

        #region CRMPotentialCustomerController > Save Document 
        [HttpPost]
        public ActionResult SaveDocument(CRMDocumentViewModel documentViewModel)
        {
            var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();

            if (documentViewModel == null || documentViewModel.document == null)
            {
                NLogger.Error("Requested documentViewModel is null or empty");
                return Json(new { success = false, message = "documentViewModel is null or empty" });
            }

            NLogger.Debug(
                   $"CRM_AccountCustomerDetailId={documentViewModel.CRM_AccountCustomerDetailId}, " +
                   $"CRM_AccountId={documentViewModel.CRM_AccountId}, " +
                  $"document={documentViewModel.document}," +
                  $"Description={documentViewModel.Description}, " +
                  $"File_Name={documentViewModel.File_Name}");

            documentViewModel.File_Name = documentViewModel.document.FileName;
            documentViewModel.Content_Type = documentViewModel.document.ContentType;
            documentViewModel.CreatedBy = LoginUserId;
            if (documentViewModel.Description == null)
                documentViewModel.Description = " ";



            if (!Directory.Exists(Server.MapPath("~/Areas/CRM/Documents")))
                Directory.CreateDirectory(Server.MapPath("~/Areas/CRM/Documents"));



            documentViewModel.Document_FilePath = Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), Path.GetFileName(documentViewModel.document.FileName));

            if (System.IO.File.Exists(documentViewModel.Document_FilePath)) { /*TODO Message*/ }

            var crmdocument = _crmService.SaveCRM_Document(CRMAutoMapper.CrmDocumentViewModelToEntity(documentViewModel));
            documentViewModel.document.SaveAs(documentViewModel.Document_FilePath);

            var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(documentViewModel.CRM_AccountCustomerDetailId).ToList();
            //Document
            if (accountDocument != null)
                accountCustomerDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmDocumentEntityToViewModel(crmdocument),
                document = accountCustomerDocumentViewModels
            });
        }

        [HttpPost]
        public ActionResult SaveDocuments(int id)
        {
            if (Request.Files.Count > 0)
            {
                var crmdocumentViewModel = new CRMDocumentViewModel();

                if (!Directory.Exists(Server.MapPath("~/Areas/CRM/Documents")))
                    Directory.CreateDirectory(Server.MapPath("~/Areas/CRM/Documents"));

                HttpFileCollection filecollection = System.Web.HttpContext.Current.Request.Files;

                for (int i = 0; i < filecollection.Count; i++)
                {

                    HttpPostedFile file = filecollection[i];
                    crmdocumentViewModel.CRM_AccountCustomerDetailId = id;
                    crmdocumentViewModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                    crmdocumentViewModel.File_Name = file.FileName;
                    crmdocumentViewModel.Content_Type = file.ContentType;
                    crmdocumentViewModel.Description = "";

                    crmdocumentViewModel.Document_FilePath = Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), Path.GetFileName(file.FileName));

                    if (System.IO.File.Exists(crmdocumentViewModel.Document_FilePath)) { continue; /*TODO Message*/ }

                    file.SaveAs(crmdocumentViewModel.Document_FilePath);
                    var crmdocument = _crmService.SaveCRM_Document(CRMAutoMapper.CrmDocumentViewModelToEntity(crmdocumentViewModel));

                }
                var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();

                return Json(new
                {
                    success = true,
                    document = CRMAutoMapper.CrmDocumentEntitiesToViewModels(crmdocuments)
                });
            }
            return Json(new
            {
                success = false,
                message = "There is no document to upload"
            });

        }

        [HttpPost]
        public ActionResult SaveCloseDocument(TempDocumentViewModel doc)
        {
            if (doc == null)
            {
                NLogger.Error("CRM Sales Document are Empty");
                return Json(new { success = false, message = "There is not any Document attached" });
            }
            if (doc.file1 != null) { if (doc.file1.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file1, 1); }
            if (doc.file2 != null) { if (doc.file2.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file2, 2); }
            if (doc.file3 != null) { if (doc.file3.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file3, 3); }
            if (doc.file4 != null) { if (doc.file4.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file4, 4); }
            if (doc.file5 != null) { if (doc.file5.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file5, 5); }
            if (doc.file6 != null) { if (doc.file6.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file6, 6); }
            if (doc.file7 != null) { if (doc.file7.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file7, 7); }
            if (doc.file8 != null) { if (doc.file8.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file8, 8); }
            if (doc.file9 != null) { if (doc.file9.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file9, 9); }
            if (doc.file10 != null) { if (doc.file10.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file8, 10); }
            if (doc.file11 != null) { if (doc.file11.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file11, 11); }
            if (doc.file12 != null) { if (doc.file12.ContentLength > 0) SaveTempDoc(doc.CRM_AccountCustomerDetailId, doc.file12, 12); }

            return Json(new
            {
                success = true,
                result = ""
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CRMPotentialCustomerController > Download Document
        [HttpGet]
        public ActionResult DownloadDocument(int id = 0)
        {
            var crmDocument = _crmService.GetCRM_DocumentById(id);
            //if (crmDocument != null)
            return File(Path.Combine("~/Areas/CRM/Documents/", crmDocument.File_Name), "application / octet - stream", crmDocument.File_Name);

        }
        #endregion

        #region CRMPotentialCustomerController > Display Document
        [HttpGet]
        public ActionResult DisplayDocument()
        {
            var crmDocument = _crmService.GetCRM_Document_LastRecord();
            if (crmDocument.Content_Type.Equals("application/pdf"))
                return File(Path.Combine("~/Areas/CRM/Documents/", crmDocument.File_Name), "application/pdf");
            else
                return File(Path.Combine("~/Areas/CRM/Documents/", crmDocument.File_Name), "application / octet - stream", crmDocument.File_Name);


        }
        #endregion

        #endregion
        public void SaveTempDoc(int id, HttpPostedFileBase file, int filetypeId)
        {
            var crmClosetempfile = new CRMCloseTempDocumentViewModel();
            crmClosetempfile.CRM_AccountCustomerDetailId = id;
        }

        public ActionResult CRMUploadDocumentPopup(int id)
        {
            ViewBag.Id = id;
            return PartialView("_CRMUploadDocumentPopup", CustomerService.GetCRMDocumentByAccountCustomerDetailId(id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer)));
        }

        #region :: CRM Upload Documents :: 

        [HttpPost]
        public ActionResult CRMSaveDocuments(FormCollection collection)
        {
            string selIds = Request["selIds"];
            string CRMAccCustDetailId = Request["CRMAccCustDetailId"];
            if (selIds != "")
            {
                string[] arrIds = selIds.Split(',');
                for (int i = 0; i < arrIds.Length; i++)
                {
                    if (Request.Files.Count > 0)
                    {
                        var crmdocumentViewModel = new CRMDocumentViewModel();
                        if (!Directory.Exists(Server.MapPath("~/Areas/CRM/Documents")))
                            Directory.CreateDirectory(Server.MapPath("~/Areas/CRM/Documents"));

                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[i];
                        string DocumentId = arrIds[i];

                        string fname = CRMAccCustDetailId + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + file.FileName.Trim();

                        var CRMModel = _crmService.GetCRMDocumentWithAccountCustomer_FileType(Convert.ToInt32(CRMAccCustDetailId), Convert.ToInt32(DocumentId));
                        if (CRMModel != null && CRMModel.CRM_DocumentId > 0)
                        {
                            CRMModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                            CRMModel.File_Name = fname;
                            CRMModel.Content_Type = file.ContentType;
                            //CRMModel.Document_FilePath = Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), fname);                                                        
                            CRMModel.Document_FilePath = "~/Areas/CRM/Documents/" + fname.Trim();
                            file.SaveAs(Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), fname.Trim()));
                            var crmdocument = _crmService.SaveCRM_Document(CRMModel);
                        }
                        else
                        {
                            crmdocumentViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(CRMAccCustDetailId);
                            crmdocumentViewModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                            crmdocumentViewModel.File_Name = fname;
                            crmdocumentViewModel.Content_Type = file.ContentType;
                            crmdocumentViewModel.Description = "";
                            crmdocumentViewModel.FileTypeListId = Convert.ToInt32(DocumentId);
                            //crmdocumentViewModel.Document_FilePath = Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), fname);
                            crmdocumentViewModel.Document_FilePath = "~/Areas/CRM/Documents/" + fname.Trim();

                            //if (System.IO.File.Exists(crmdocumentViewModel.Document_FilePath)) { continue; /*TODO Message*/ }

                            file.SaveAs(Path.Combine(Server.MapPath("~/Areas/CRM/Documents"), fname.Trim()));
                            var crmdocument = _crmService.SaveCRM_Document(CRMAutoMapper.CrmDocumentViewModelToEntity(crmdocumentViewModel));
                        }
                    }
                }

                int CRM_AccId = Convert.ToInt32(CRMAccCustDetailId);
                var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountCustomerDetailId == CRM_AccId).ToList();
                return Json(new
                {
                    success = true,
                    document = CRMAutoMapper.CrmDocumentEntitiesToViewModels(crmdocuments)
                });
            }
            return Json(new
            {
                success = false,
                message = "There is no document to upload"
            });
        }

        public ActionResult RemoveCRM_Document(int Id, int CRMAccId)
        {
            if (Id > 0)
            {
                _crmService.DeleteCRM_Document(Id);
            }
            int CRM_AccId = Convert.ToInt32(CRMAccId);
            var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountCustomerDetailId == CRM_AccId).ToList();
            return Json(new { success = true, document = CRMAutoMapper.CrmDocumentEntitiesToViewModels(crmdocuments) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Common
        private static RootObjectlatlngViewModel GetLatLongByAddress(string address)
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
    }
}