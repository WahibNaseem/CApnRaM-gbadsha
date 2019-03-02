namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    using Application.Web.Core;
    using Resources;
    using JKApi.Service.ServiceContract.CRM;
    using MvcBreadCrumbs;
    using System.Collections.Generic;
    using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;
    using System.Linq;
    using System.Web.Mvc;
    using Common;
    using JKViewModels.CRM;
    using System;
    using System.Globalization;
    using System.IO;
    using JKApi.Core;
    using JKApi.Service.ServiceContract.Customer;
    using JKApi.Service;
    using System.Configuration;
    using System.Web;
    using JKApi.Service.ServiceContract.Franchisee;
    using JKApi.Core.Common;

    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    public class CRMPotentialFranchiseController : ViewControllerBase
    {
        private const bool V = true;
        #region Life Cycle
        private readonly ICRM_Service _crmService;
        private bool _hasNewData;
        public string _fileDirectory;
        public CRMPotentialFranchiseController(ICacheProvider cacheProivder, ICRM_Service ICRMService, ICustomerService customerservice, ICommonService commonService, IFranchiseeService franchiseeService, ClaimView claimView)
        {
            _cacheProvider = cacheProivder;
            _crmService = ICRMService;
            CustomerService = customerservice;
            FranchiseeService = franchiseeService;
            _commonService = commonService;
            _claimView = claimView;
            ViewBag.HMenu = "FranchiseSales";
            _fileDirectory = ConfigurationManager.AppSettings[AppConstants.APPSETTINGS_FILESDIRECTORY] + "//";

        }
        #endregion

        #region CRMLeadFranchiseController > Helpers

        private SelectList LeadStageStatusList
        {
            get
            {
                var accountstagestatus = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_QualifiedLead ||
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
                    .Where(x => x.Name == Constants.Key_PotentialInquary ||
                    x.Name == Constants.Key_FranchiseContract ||
                    x.Name == Constants.Key_FollowUp ||
                    x.Name == Constants.Key_SignAgreement ||
                    x.Name == Constants.Key_SoldToLegal);

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

        // GET: CRM/CRMPotentialFranchise
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Detail", "CRMLeadFranchise", new { area = "CRM" }), "Leads");

            /*Get the Role CRM-Sales*/
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");

            /*Get the Sale Person list of CRM-Sales role*/
            var salesPersonList = _crmService.Get_AuthUserLogin_Potential(roleId != null ? roleId.RoleId : 7, this.LoginUserId, this.SelectedRegionId);
            ViewBag.SalePersonList = salesPersonList;
            ViewBag.UserList = new SelectList(salesPersonList, "UserId", "FirstName");

            var lstRegion = salesPersonList
                          .Where(r => r.RegionId != null)
                          .GroupBy(x => new { x.RegionId, x.RegionName })
                          .Select(csuh => new CRMScheduleUserHierarchy
                          {
                              RegionId = csuh.First().RegionId,
                              RegionName = csuh.First().RegionName

                          }).ToList();
            ViewBag.LstRegion = new SelectList(lstRegion, "RegionId", "RegionName", this.SelectedRegionId);
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.SelectedRegionId = this.SelectedRegionId;

            // Select list
            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name");

            return View();
        }

        [HttpGet]
        public ActionResult GetIndex()
        {
            //First parameter passing 3 for Potential Leads
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialFranchisee(3, 0, SelectedUserId, SelectedRegionId);

            return Json(new
            {
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SoldFranchiseLead()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "FranchiseSales", new { area = "CRM" }), "Sold Franchise Leads");

            // Select list
            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name");
            return View();
        }

        [HttpGet]
        public ActionResult GetSoldFranchise()
        {
            //Passing First parameter 4 for Sold lead list
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialFranchisee(4, 0, SelectedUserId, SelectedRegionId);
            return Json(new
            {
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }


        #region CRMLeadFranchiseController > HttpRequest

        /// <summary>
        /// Get Partial View of Stages of respective Id
        /// </summary>
        /// <param name="ssid"></param>
        /// <returns></returns>
        public ActionResult PartialPotentialLoad(int ssid, int fdid)
        {

            ViewBag.States = new SelectList(CustomerService.GetStateList(), "abbr", "Name");
            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            ViewBag.FranchiseeContractTypeList = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");

            ViewBag.PotentialInquaryPurpose = new SelectList(_crmService.GetAll_CRM_PurposeType().ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name");
            /*Comment this for the moment*/
            // ViewBag.PotentialInquaryPurpose = new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMPotentialInquary == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name");
            ViewBag.PurposeType = new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMFranchiseFollowUp == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name");
            ViewBag.CloseType = new SelectList(_crmService.GetAll_CRM_CloseType().Where(x => x.CRMFranchiseFollowUp == true).ToList().OrderBy(x => x.Name), "CRM_CloseTypeId", "Name");

            if (ssid == 24)
            {
                return PartialView("_Partial_FollowUp");
            }
            else if (ssid == 31)
            {
                return PartialView("_Partial_PotentialInquary");
            }
            else if (ssid == 28)
            {
                return PartialView("_Partial_FranchiseContract");
            }
            else if (ssid == 29)
            {
                return PartialView("_Partial_SignAgreement");
            }
            else if (ssid == 32)
            {
                var signagreementvm = CRMAutoMapper.CrmSignAgreementEntityToViewModel(_crmService.GetCRM_SignAgreement_ByAccountFranchiseId(fdid));
                ViewBag.FranchiseeContractTypeList = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", signagreementvm.PlanType);

                var data = new
                {
                    success = V,
                    signagreementvm = CRMUtils.RenderPartialViewToString(controllercontext: ControllerContext, viewname: "_Partial_SoldToLegal", model: signagreementvm)

                };
                return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        #region CRMPotentialFranchiseController > GetPotentialDetail
        /// <summary>
        /// Get Detail of the Franchise Lead that has potential Status by accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPotentialDetail(int accountId)
        {
            var accountFranchiseDetailViewModel = new CRMAccountFranchiseDetailViewModel();
            var accountFranchiseActivityViewModels = new List<CRMActivityViewModel>();
            var accountFranchiseNoteViewModels = new List<CRMNoteViewModel>();

            var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();
            var accountFranchiseScheduleViewModels = new List<CRMScheduleViewModel>();


            var accountCustomerInitialViewModel = new CRMInitialCommunicationViewModel();
            var accountFranchiseLeadGenViewModel = new CRMLeadGenerationViewModel();
            var accountFranchiseContractViewModel = new CRMFranchiseContractViewModel();
            var accountFranchiseFollowUpViewModel = new CRMFranchiseFollowUpViewModel();

            var accountFranchiseStageScheduleViewModels = new List<CRMStageStatusScheduleViewModel>();
            var accountFranchiseScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            /*Get from Services  Account ,  AccountCustomerDetail , AccountActivity , AccountSchedule,AccountNote,AccountDocument
             AccountInitialCommunication , Account FvPresentation AccountBidding */
            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountFranchiseDetail = _crmService.GetCRM_AccountFranchiseDetail_ByAccountId(accountId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountFranchiseDetailId(accountFranchiseDetail.CRM_AccountFranchiseDetailId).ToList();

            var accountNote = _crmService.GetCRM_Note_ByAccountFranchiseDetailId(accountFranchiseDetail.CRM_AccountFranchiseDetailId).ToList();
            var accountDocument = _crmService.GetCRM_Document_ByAccountFranchiseDetailId(accountFranchiseDetail.CRM_AccountFranchiseDetailId).ToList();
            var accountSchedule = _crmService.GetCRM_Schedule_ByAccountFranchiseDetailId(accountFranchiseDetail.CRM_AccountFranchiseDetailId).ToList();

            var accountStageStatusSchedules = _crmService.GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById(accountFranchiseDetail.CRM_AccountFranchiseDetailId).ToList();
            var accountLeadGen = _crmService.GetCRM_LeadGeneration_ByAccountFranchiseId(accountFranchiseDetail.CRM_AccountFranchiseDetailId);
            var accountInitialCommunication = _crmService.GetCRM_InitialCommunication_ByAccountFranchiseId(accountFranchiseDetail.CRM_AccountFranchiseDetailId);

            var accountfranchiseContract = _crmService.GetCRM_FranchiseContract_ByAccountFranchiseId(accountFranchiseDetail.CRM_AccountFranchiseDetailId);
            var accountfranchiseFollowUp = _crmService.GetCRM_FranchiseFollowUp_ByAccountFranchiseDetailId(accountFranchiseDetail.CRM_AccountFranchiseDetailId);

            if (account != null)
            {
                accountFranchiseDetailViewModel = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(accountFranchiseDetail);
                accountFranchiseDetailViewModel.CRM_AccountId = account.CRM_AccountId;
                accountFranchiseDetailViewModel.FirstName = account.FirstName;
                accountFranchiseDetailViewModel.LastName = account.LastName;
                accountFranchiseDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountFranchiseDetailViewModel.EmailAddress = account.EmailAddress;
                accountFranchiseDetailViewModel.ContactName = account.ContactName;


                accountFranchiseDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountFranchiseDetailViewModel.StageStatusId = account.StageStatus.Value;
                accountFranchiseDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;
                accountFranchiseDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;


                // Activity
                if (accountActivity != null)
                {
                    accountFranchiseActivityViewModels = CRMAutoMapper.CrmActivityEntitiesToViewModels(accountActivity);
                    foreach (var activityViewModel in accountFranchiseActivityViewModels)
                    {
                        activityViewModel.ActivityTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType)));
                        if (activityViewModel.OutComeType != null)
                            activityViewModel.OutComeTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType)));
                    }
                }


                //Note
                if (accountNote != null)
                    accountFranchiseNoteViewModels = CRMAutoMapper.CrmNoteEntitiesToViewModels(accountNote);

                //Schedule
                if (accountSchedule != null)
                {
                    accountFranchiseScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                    foreach (var accountschedule in accountFranchiseScheduleViewModels)
                    {
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = accountschedule.Title;
                        // calendar.start = accountschedule.StartDate.Value.ToString("s");
                        accountFranchiseScheduleCalendar.Add(calendar);
                    }
                }
                //Document
                if (accountDocument != null)
                    accountCustomerDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);

                //Lead Generation
                if (accountLeadGen != null)
                {
                    accountFranchiseLeadGenViewModel = CRMAutoMapper.CrmLeadGenEntityToViewModel(accountLeadGen);
                }
                else
                {
                    accountFranchiseLeadGenViewModel = null;
                }

                //Initial Communication
                if (accountInitialCommunication != null)
                    accountCustomerInitialViewModel = CRMAutoMapper.CrmInitialEntityToViewModel(accountInitialCommunication);
                else
                {
                    accountCustomerInitialViewModel = null;
                }

                /* Franchise Contract */
                if (accountfranchiseContract != null)
                {
                    accountFranchiseContractViewModel = CRMAutoMapper.CrmFranchiseContractEntityToViewModel(accountfranchiseContract);
                }
                else
                {
                    accountFranchiseContractViewModel = null;
                }

                /* Franchise Follow Up */
                if (accountfranchiseFollowUp != null)
                {
                    accountFranchiseFollowUpViewModel = CRMAutoMapper.CrmFranchiseFollowUpEntityToViewModel(accountfranchiseFollowUp);
                }
                else
                {
                    accountFranchiseFollowUpViewModel = null;
                }
            }


            var data = new
            {

                success = true,
                franchisedetailvm = accountFranchiseDetailViewModel,
                activity = accountFranchiseActivityViewModels,
                note = accountFranchiseNoteViewModels,
                document = accountCustomerDocumentViewModels,
                schedule = accountFranchiseScheduleViewModels,
                initial = accountInitialCommunication,
                leadgen = accountFranchiseLeadGenViewModel,
                franchiseContract = accountFranchiseContractViewModel,
                franchiseFollowUp = accountFranchiseFollowUpViewModel,
                stagestatuschedules = accountFranchiseStageScheduleViewModels,
                calenderDates = accountFranchiseScheduleCalendar,
                assigneeId = account.AssigneeId
            };

            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region CRMPotentialFranchiseController > SaveLeadGenerationData
        [HttpPost]
        public ActionResult SaveLeadGenerationData(System.Web.Mvc.FormCollection leadgendata)
        {
            if (leadgendata == null)
            {
                //NLogger.Error("Requested leadgendata is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={leadgendata["CRM_AccountId"]}, " +
            //         $"CRM_AccountFranchiseDetailId={leadgendata["CRM_AccountFranchiseDetailId"]}, " +
            //        $"Name={leadgendata["Name"]}," +
            //        $"Address={leadgendata["Address"]}, " +

            //        $"City={leadgendata["City"]}, " +
            //        $"State={leadgendata["State"]} " +
            //        $"ZipCode={leadgendata["ZipCode"]}" +
            //        $"ContactPerson={leadgendata["ContactPerson"]} " +

            //        $"PhoneNumber={leadgendata["PhoneNumber"]}, " +
            //        $"StartDate={leadgendata["StartDate"]} " +
            //        $"StartTime={leadgendata["StartTime"]}" +
            //        $"EndDate={leadgendata["EndDate"]} " +

            //        $"EndTime={leadgendata["EndTime"]}" +
            //        $"PurposeId={leadgendata["PurposeId"]} " +
            //        $"Purpose={leadgendata["Purpose"]} " +
            //        $"Note={leadgendata["Note"]}");

            /* Lead Generation as Potential Inquary */
            var leadgenViewModel = new CRMLeadGenerationViewModel
            {
                CRM_AccountFranchiseDetailId = Convert.ToInt32(leadgendata["CRM_AccountFranchiseDetailId"]),
                Name = leadgendata["Name"],
                Address = leadgendata["Address"],
                City = leadgendata["City"],
                State = leadgendata["State"],
                ZipCode = leadgendata["ZipCode"],
                ContactPerson = leadgendata["ContactPerson"],
                PhoneNumber = leadgendata["PhoneNumber"],
                Note = leadgendata["Note"]
            };

            var leadgen = _crmService.SaveCRM_LeadGeneration(CRMAutoMapper.CrmLeadGenViewModelToEntity(leadgenViewModel));
            var accountfranchise = _crmService.GetCRM_AccountbyId(int.Parse(leadgendata["CRM_AccountId"]));
            var franchisedetail = _crmService.GetCRM_AccountFranchiseDetail_ByAccountId(int.Parse(leadgendata["CRM_AccountId"]));

            var schedulevm = new CRMScheduleViewModel
            {
                CRM_AccountFranchiseDetailId = leadgen.CRM_AccountFranchiseDetailId,

                StartDate = leadgendata["StartDate"] != "" ? Convert.ToDateTime(leadgendata["StartDate"] + " " + leadgendata["StartTime"]) : DateTime.Now,
                EndDate = leadgendata["ScheduleEndDate"] != "" ? Convert.ToDateTime(leadgendata["EndDate"] + " " + leadgendata["EndTime"]) : DateTime.Now,
                Title = accountfranchise.CRM_AccountId + " - " + "PI" + " " + leadgendata["Prupose"] + " - " + accountfranchise.ContactName + " - " + leadgen.ContactPerson + " - " + franchisedetail.WorkNumber,
                Location = franchisedetail.Address1,
                AuthUserLoginId = accountfranchise.AssigneeId,
                CRM_StageStatusType = (int)StageStatusType.PotentialInquary,
                CRM_ScheduleTypeId = 1, /* ScheduleType 1 CRM*/
                RegionId = SelectedRegionId,
                CreatedBy = LoginUserId,

                PurposeId = Convert.ToInt32(leadgendata["PruposeId"]),
                Purpose = leadgendata["Prupose"],
                IsActive = true
            };
            var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm));



            var account = _crmService.GetCRM_AccountbyId(int.Parse(leadgendata["CRM_AccountId"]));
            //update AccountCustomer            
            if (accountfranchise != null)
            {
                account.StageStatus = (int)StageStatusType.FranchiseContract;
                _crmService.SaveCRM_Account(account);
            }

            return Json(new
            {
                success = true,
                result = ""
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMPotentialFranchiseController > SaveFranchiseContractData

        [HttpPost]
        public ActionResult SaveFranchiseContractData(System.Web.Mvc.FormCollection franchisecontractdata)
        {
            if (franchisecontractdata == null)
            {
                //NLogger.Error("Requested franchisecontractdata is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={franchisecontractdata["CRM_AccountId"]}, " +
            //         $"CRM_AccountFranchiseDetailId={franchisecontractdata["CRM_AccountFranchiseDetailId"]}, " +
            //        $"PresentFranchise={franchisecontractdata["PresentFranchise"]}," +
            //        $"CompleteQuestionnaire={franchisecontractdata["CompleteQuestionnaire"]}, " +

            //        $"AllPrinciple={franchisecontractdata["AllPrinciple"]}, " +
            //        $"SignAgreement={franchisecontractdata["SignAgreement"]} " +
            //        $"CompanyCreated={franchisecontractdata["CompanyCreated"]}" +
            //        $"FranchiseApp={franchisecontractdata["FranchiseApp"]} " + 

            //        $"Note={franchisecontractdata["Note"]}");

            var franchiiseContrctVM = new CRMFranchiseContractViewModel();

            franchiiseContrctVM.CRM_AccountFranchiseDetailId = Convert.ToInt32(franchisecontractdata["CRM_AccountFranchiseDetailId"]);
            franchiiseContrctVM.FranchiseDisclosure = Convert.ToBoolean(franchisecontractdata["DSignAcknowledgement"]);
            franchiiseContrctVM.FranchiseQuestionaire = Convert.ToBoolean(franchisecontractdata["FranchiseQuestionaire"]);
            franchiiseContrctVM.AllPrincipleClosed = Convert.ToBoolean(franchisecontractdata["AllPrinciple"]);
            franchiiseContrctVM.SignAgreement = Convert.ToBoolean(franchisecontractdata["SignAgreement"]);
            franchiiseContrctVM.CompanyCreated = Convert.ToInt32(franchisecontractdata["CompanyCreated"]);
            franchiiseContrctVM.CompleteFranchiseApplication = Convert.ToBoolean(franchisecontractdata["FranchiseApp"]);
            franchiiseContrctVM.RegionId = SelectedRegionId;
            franchiiseContrctVM.Notes = franchisecontractdata["Note"];

            var franchiseentity = _crmService.SaveCRM_FranchiseContract(CRMAutoMapper.CrmFranchiseContractViewModelToEntity(franchiiseContrctVM));

            var account = _crmService.GetCRM_AccountbyId(int.Parse(franchisecontractdata["CRM_AccountId"]));
            //update AccountCustomer            
            if (account != null)
            {
                account.StageStatus = (int)StageStatusType.FollowUp;
                _crmService.SaveCRM_Account(account);
            }

            return Json(new
            {
                success = true,
                result = ""
            });
        }
        #endregion

        #region CRMPotentialFranchiseController > SaveFranchiseFollowUp
        [HttpPost]
        public ActionResult SaveFranchiseFollowUp(FormCollection crmFranchiseFollowUp)
        {
            if (crmFranchiseFollowUp == null)
            {
                //NLogger.Error("Requested leadgendata is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var franchiseFollowUpvm = new CRMFranchiseFollowUpViewModel();
            franchiseFollowUpvm.CRM_AccountFranchiseDetailId = Convert.ToInt32(crmFranchiseFollowUp["CRM_AccountFranchiseDetailId"]);
            franchiseFollowUpvm.DiscloseAdditional = Convert.ToBoolean(crmFranchiseFollowUp["AdditionalPurpose"]);
            franchiseFollowUpvm.StatusCreationConfirmed = Convert.ToBoolean(crmFranchiseFollowUp["CreationConfirmed"]);
            franchiseFollowUpvm.KeepActive = Convert.ToBoolean(crmFranchiseFollowUp["NotifyTraining"]);
            franchiseFollowUpvm.NotifyNextTraining = Convert.ToBoolean(crmFranchiseFollowUp["KeepItActive"]);
            franchiseFollowUpvm.Note = crmFranchiseFollowUp["Note"];
            franchiseFollowUpvm.CreatedBy = LoginUserId;

            var franchisefollowup = _crmService.SaveCRM_FranchiseFollowUp(CRMAutoMapper.CrmFranchiseFollowUpViewModelToEntity(franchiseFollowUpvm));

            /* Signing Schedule  */
            var accountfranchise = _crmService.GetCRM_AccountbyId(int.Parse(crmFranchiseFollowUp["CRM_AccountId"]));
            var franchisedetail = _crmService.GetCRM_AccountFranchiseDetail_ByAccountId(int.Parse(crmFranchiseFollowUp["CRM_AccountId"]));
            var schedulevm = new CRMScheduleViewModel();

            schedulevm.CRM_AccountFranchiseDetailId = franchisefollowup.CRM_AccountFranchiseDetailId;

            schedulevm.StartDate = crmFranchiseFollowUp["SigningDate"] != "" ? Convert.ToDateTime(crmFranchiseFollowUp["SigningDate"] + " " + crmFranchiseFollowUp["SigningTime"]) : DateTime.Now;
            schedulevm.EndDate = DateTime.Now;
            schedulevm.Title = accountfranchise.CRM_AccountId + " - " + "FU" + "-" + "Signing Schedule" + " " + crmFranchiseFollowUp["Purpose"] + " - " + accountfranchise.ContactName + " - " + accountfranchise.ContactName + " - " + franchisedetail.WorkNumber;
            schedulevm.Location = franchisedetail.Address1;
            schedulevm.AuthUserLoginId = accountfranchise.AssigneeId;
            schedulevm.CRM_StageStatusType = (int)StageStatusType.FollowUp;
            schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
            schedulevm.RegionId = SelectedRegionId;
            schedulevm.CreatedBy = LoginUserId;

            schedulevm.PurposeId = Convert.ToInt32(crmFranchiseFollowUp["PurposeId"]);
            schedulevm.Purpose = crmFranchiseFollowUp["Purpose"];
            schedulevm.IsActive = true;

            var schedule_signing = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm));

            /* Save FollowUp Available To Meet  Schedule */
            if (schedule_signing.Purpose == "Contract Signed" || schedule_signing.Purpose == "Close Sale Cycle")
            {
                var scheduleviewmodel = new CRMScheduleViewModel();

                scheduleviewmodel.CRM_AccountFranchiseDetailId = franchisefollowup.CRM_AccountFranchiseDetailId;

                scheduleviewmodel.StartDate = crmFranchiseFollowUp["StartDate"] != "" ? Convert.ToDateTime(crmFranchiseFollowUp["StartDate"] + " " + crmFranchiseFollowUp["StartTime"]) : DateTime.Now;
                scheduleviewmodel.EndDate = crmFranchiseFollowUp["EndDate"] != "" ? Convert.ToDateTime(crmFranchiseFollowUp["EndDate"] + " " + crmFranchiseFollowUp["EndTime"]) : DateTime.Now;
                scheduleviewmodel.Title = accountfranchise.CRM_AccountId + " - " + "FU" + " " + crmFranchiseFollowUp["FollowPurpose"] + " - " + accountfranchise.ContactName + " - " + accountfranchise.ContactName + " - " + franchisedetail.WorkNumber;
                scheduleviewmodel.Location = franchisedetail.Address1;
                scheduleviewmodel.AuthUserLoginId = accountfranchise.AssigneeId;
                scheduleviewmodel.CRM_StageStatusType = (int)StageStatusType.FollowUp;
                scheduleviewmodel.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                scheduleviewmodel.RegionId = SelectedRegionId;
                scheduleviewmodel.CreatedBy = LoginUserId;

                scheduleviewmodel.PurposeId = Convert.ToInt32(crmFranchiseFollowUp["FollowPurposeId"]);
                scheduleviewmodel.Purpose = crmFranchiseFollowUp["FollowPurpose"];
                scheduleviewmodel.IsActive = true;


                var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(scheduleviewmodel));
            }

            //update AccountCustomer            
            if (accountfranchise != null)
            {
                accountfranchise.StageStatus = (int)StageStatusType.SignAgreement;
                _crmService.SaveCRM_Account(accountfranchise);
            }

            return Json(new
            {
                success = true,
                result = ""
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Save the Sign Agreement Data into DataBase
        /// </summary>
        /// <param name="signagreementdata"></param>
        /// <returns></returns>
        /// 

        [HttpPost]
        public ActionResult SaveSignAgreement(FormCollection signagreementdata)
        {
            if (signagreementdata == null)
            {
                //NLogger.Error("Requested leadgendata is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var signAgreementvm = new CRMSignAgreementViewModel();
            signAgreementvm.CRM_AccountFranchiseDetailId = Convert.ToInt32(signagreementdata["CRM_AccountFranchiseDetailId"]);
            signAgreementvm.SignFranchiseAgreement = Convert.ToBoolean(signagreementdata["SignFranchseAgreement"]);
            signAgreementvm.GuaranteesSigned = Convert.ToBoolean(signagreementdata["GuaranteesSigned"]);
            signAgreementvm.RequiredDocument = Convert.ToBoolean(signagreementdata["RequiredDocument"]);
            signAgreementvm.LegalBackGround = Convert.ToBoolean(signagreementdata["LegalBackGround"]);
            signAgreementvm.DateSign = Convert.ToDateTime(signagreementdata["DateSign"]);
            signAgreementvm.Term = Convert.ToDecimal(signagreementdata["Term"]);
            signAgreementvm.ExpDate = Convert.ToDateTime(signagreementdata["ExpDate"]);
            signAgreementvm.PlanType = Convert.ToInt32(signagreementdata["PlanType"]);
            signAgreementvm.PlanAmount = Convert.ToDecimal(signagreementdata["PlanAmount"]);
            if (signagreementdata["IBAmount"] != "")
            {
                signAgreementvm.IBAmount = Convert.ToDecimal(signagreementdata["IBAmount"]);
            }
            if (signagreementdata["DownPayment"] != "")
            {
                signAgreementvm.DownPayment = Convert.ToDecimal(signagreementdata["DownPayment"]);
            }
            if (signagreementdata["Interest"] != "")
            {
                signAgreementvm.Interest = Convert.ToDecimal(signagreementdata["Interest"]);
            }
            if (signagreementdata["PaymentAmount"] != "")
            {
                signAgreementvm.PaymentAmount = Convert.ToDecimal(signagreementdata["PaymentAmount"]);
            }

            signAgreementvm.NoOfPayments = Convert.ToDecimal(signagreementdata["NoPayments"]);

            if (signagreementdata["CurrentPayment"] != "")
            {
                signAgreementvm.CurrentPayment = Convert.ToDecimal(signagreementdata["CurrentPayment"]);
            }
            signAgreementvm.PaymentStartDate = Convert.ToDateTime(signagreementdata["PaymentStartDate"]);
            signAgreementvm.TriggerAmount = Convert.ToDecimal(signagreementdata["TriggerAmount"]);
            signAgreementvm.legalOblStart = Convert.ToDateTime(signagreementdata["LegalOblStart"]);
            if (signagreementdata["LegalOblEnd"] != "")
            {
                signAgreementvm.LegalOblEnd = Convert.ToDateTime(signagreementdata["LegalOblEnd"]);
            }

            signAgreementvm.LegalOblDue = signagreementdata["LegalOblDue"];
            signAgreementvm.Note = signagreementdata["Note"];

            var signagreement = _crmService.SaveCRM_SignAgreement(CRMAutoMapper.CrmSignAgreementViewModelToEntity(signAgreementvm));

            var crmaccount = _crmService.GetCRM_AccountbyId(Convert.ToInt32(signagreementdata["CRM_AccountId"]));
            if (crmaccount != null)
            {
                crmaccount.StageStatus = (int)StageStatusType.SoldToLegal;
                crmaccount.Stage = (int)StageType.Franchisee;

                _crmService.SaveCRM_Account(crmaccount);
            }

            var data = new
            {
                success = V
            };
            return Json(data: data);
        }

        #region CRMPotentialFranchiseController > SaveFranchiseActivity

        [HttpPost]
        public ActionResult SaveFranchiseActivitySummary(System.Web.Mvc.FormCollection activityData)
        {
            if (activityData == null)
            {
                //NLogger.Error("Requested activityData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //DateTime timestamp = DateTime.ParseExact(activityData["TimeStamp"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            //NLogger.Debug($"CRM_AccountFranchiseDetailId={activityData["CRM_AccountFranchiseDetailId"]}, " +
            //              $"Note={activityData["Note"]}, " +
            //              $"ActivityType={activityData["ActivityType"]}, " +
            //               $"OutComeType={activityData["OutComeType"]}, " +
            //              $"TimeStamp={timestamp}");

            var crmActivityViewModel = new CRMActivityViewModel();
            crmActivityViewModel.ActivityType = Convert.ToInt32(activityData["ActivityType"]);
            if (Convert.ToInt32(activityData["ActivityType"]) == 1)
                crmActivityViewModel.OutComeType = Convert.ToInt32(activityData["OutComeType"]);
            crmActivityViewModel.Note = activityData["Note"];
            crmActivityViewModel.TimeStamp = activityData["StartDate"] != "" ? Convert.ToDateTime(activityData["StartDate"] + " " + activityData["StartTime"]) : DateTime.Now;
            crmActivityViewModel.CRM_AccountFranchiseDetailId = Convert.ToInt32(activityData["CRM_AccountFranchiseDetailId"]);
            crmActivityViewModel.CreatedBy = LoginUserId;

            var activity = _crmService.SaveCRM_Activity(CRMAutoMapper.CrmActivityEntityToViewModel(crmActivityViewModel));
            var activityViewModel = CRMAutoMapper.CrmActivityViewModelToEntity(activity);
            activityViewModel.ActivityTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType)));
            if (activityViewModel.OutComeType != null)
                activityViewModel.OutComeTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType)));

            return Json(new
            {
                success = true,
                result = activityViewModel
            });
        }

        #endregion

        #region CRMPotentialFranchiseController > SaveFranchiseSchedule
        [HttpPost]
        public ActionResult SaveFranchiseScheduleSummary(System.Web.Mvc.FormCollection scheduleData)
        {
            if (scheduleData == null)
            {
                NLogger.Error("Requested scheduleData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            DateTime startDate = DateTime.ParseExact(scheduleData["StartDate"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            NLogger.Debug($"CRM_AccountCustomerDetailId={scheduleData["CRM_AccountCustomerDetailId"]}, " +
                         $"Title={scheduleData["Title"]}, " +
                         $"Description={scheduleData["Description"]}, " +
                         $"StartDate={startDate}," +
                         $"Duration={scheduleData["Duration"]}");


            var scheduleViewModel = new CRMScheduleViewModel();
            scheduleViewModel.CRM_AccountFranchiseDetailId = Convert.ToInt32(scheduleData["CRM_AccountFranchiseDetailId"]);
            scheduleViewModel.Title = scheduleData["Title"];
            scheduleViewModel.Description = scheduleData["Description"];
            scheduleViewModel.StartDate = startDate;
            var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(scheduleViewModel));


            #region Calendar

            var accountFranchiseScheduleViewModels = new List<CRMScheduleViewModel>();
            var accountFranchiseStageScheduleViewModels = new List<CRMStageStatusScheduleViewModel>();
            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(schedule.CRM_AccountFranchiseDetailId).ToList();
            var accountStageStatusSchedules = _crmService.GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById(schedule.CRM_AccountFranchiseDetailId).ToList();

            //Schedule 
            if (accountSchedule != null)
            {
                accountFranchiseScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountFranchiseScheduleViewModels)
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
                accountFranchiseStageScheduleViewModels = CRMAutoMapper.CrmStageStatusScheduleEntitiesToViewModels(accountStageStatusSchedules);
                foreach (var stagestatusviewmodel in accountFranchiseStageScheduleViewModels)
                {
                    if (stagestatusviewmodel.Schedule2 != null)
                    {
                        stagestatusviewmodel.Schedule2Format = stagestatusviewmodel.Schedule2.Value.ToString("dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);
                        CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                        calendar.title = stagestatusviewmodel.Purpose2 == 1 ? "Call Back" : "Meeting";
                        //calendar.start = stagestatusviewmodel.Schedule2.Value.ToString("s");
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
                schedule = accountFranchiseScheduleViewModels,
                calenderDates = accountFranchiseStageScheduleViewModels
            });
        }

        #endregion

        #region CRMPotentialFranchiseController > SaveFranchiseNote

        [HttpPost]
        public ActionResult SaveFranchiseNote(System.Web.Mvc.FormCollection noteData)
        {
            if (noteData == null)
            {
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var noteViewModel = new CRMNoteViewModel
            {
                CRM_AccountFranchiseDetailId = Convert.ToInt32(noteData["CRM_AccountFranchiseDetailId"]),
                Title = noteData["Title"],
                Description = noteData["Description"]
            };

            var data = new
            {
                success = true,
                note = CRMAutoMapper.CrmNoteEntityToViewModel(_crmService.SaveCRM_Note(CRMAutoMapper.CrmNoteViewModelToEntity(noteViewModel)))
            };

            return Json(data);


        }

        #endregion

        #region CRMPotentialFranchiseController > SaveDocument
        [HttpPost]
        public ActionResult SaveDocument(CRMDocumentViewModel documentViewModel)
        {
            if (documentViewModel == null || documentViewModel.document == null)
            {
                //NLogger.Error("Requested documentViewModel is null or empty");
                return Json(new { success = false, message = "documentViewModel is null or empty" });
            }

            //NLogger.Debug(
            //       $"CRM_AccountCustomerDetailId={documentViewModel.CRM_AccountCustomerDetailId}, " +
            //       $"CRM_AccountId={documentViewModel.CRM_AccountId}, " +
            //      $"document={documentViewModel.document}," +
            //      $"Description={documentViewModel.Description}, " +
            //      $"File_Name={documentViewModel.File_Name}");


            documentViewModel.File_Name = documentViewModel.document.FileName;
            documentViewModel.Content_Type = documentViewModel.document.ContentType;
            if (documentViewModel.Description == null)
                documentViewModel.Description = " ";




            if (!Directory.Exists(_fileDirectory + "Areas/CRM/Documents"))
                Directory.CreateDirectory(_fileDirectory + "Areas/CRM/Documents");



            documentViewModel.Document_FilePath = Path.Combine(_fileDirectory + "Areas/CRM/Documents", Path.GetFileName(documentViewModel.document.FileName));

            if (System.IO.File.Exists(documentViewModel.Document_FilePath)) { /*TODO Message*/ }

            var crmdocument = _crmService.SaveCRM_Document(CRMAutoMapper.CrmDocumentViewModelToEntity(documentViewModel));
            documentViewModel.document.SaveAs(documentViewModel.Document_FilePath);

            var accountFranchiseDocumentViewModels = new List<CRMDocumentViewModel>();

            var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(documentViewModel.CRM_AccountCustomerDetailId).ToList();
            //Document
            if (accountDocument != null)
                accountFranchiseDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);


            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmDocumentEntityToViewModel(crmdocument),
                document = accountFranchiseDocumentViewModels

            });
        }

        #endregion

        /* #region CRMPotentialFranchiseController > DownloadDocument
         [HttpGet]
         public ActionResult DownloadDocument(int id = 0)
         {
             var crmDocument = _crmService.GetCRM_DocumentById(id);
             //if (crmDocument != null)
             return File(Path.Combine(_fileDirectory + "Areas/CRM/Documents/", crmDocument.File_Name), "application / octet - stream", crmDocument.File_Name);
         }

         #endregion*/

        #region CRMPotentialFranchiseController >   Save Franchise Contact Info

        [HttpPost]
        public ActionResult SaveFranchiseContactInfo(System.Web.Mvc.FormCollection contactInfoData)
        {
            if (contactInfoData == null)
            {
                //NLogger.Error("Requested SaveCustomerContactInfo is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            /* NLogger.Debug($"CRM_AccountFranchiseDetailId={contactInfoData["CRM_AccountFranchiseDetailId"]}, " +
                         $"CRM_AccountId={contactInfoData["CRM_AccountId"]}, " +
                         $"FirstName={contactInfoData["ContactName"]}," +                       
                         $"PhoneNumber={contactInfoData["PhoneNumber"]}, " +
                         $"EmailAddress={contactInfoData["EmailAddress"]}," +
                         $"FranchiseName={contactInfoData["FranchiseName"]}, " +
                         $"Position={contactInfoData["Position"]}, " +
                         $"JobType={contactInfoData["JobType"]}," +
                         $"Address1={contactInfoData["Address1"]}, " +
                         $"Address2={contactInfoData["Address2"]}, " +
                         $"City={contactInfoData["City"]}," +
                         $"County={contactInfoData["County"]}, " +
                         $"State={contactInfoData["State"]}, " +
                         $"HomePhoneNumber={contactInfoData["HomePhoneNumber"]}," +
                         $"FaxNo={contactInfoData["FaxNo"]}," +
                         $"ZipCode={contactInfoData["ZipCode"]},");*/

            var accountfranchise = _crmService.GetCRM_AccountbyId(Convert.ToInt32(contactInfoData["CRM_AccountId"]));
            var franchiseAccountDetail = _crmService.GetCRM_AccountFranchiseDetailbyId(Convert.ToInt32(contactInfoData["CRM_AccountFranchiseDetailId"]));
            if (accountfranchise != null && franchiseAccountDetail != null)
            {
                accountfranchise.ContactName = contactInfoData["ContactName"];
                accountfranchise.PhoneNumber = contactInfoData["PhoneNumber"];
                accountfranchise.EmailAddress = contactInfoData["EmailAddress"];

                franchiseAccountDetail.FranchiseeName = contactInfoData["FranchiseName"];
                franchiseAccountDetail.Position = contactInfoData["Position"];
                franchiseAccountDetail.JkFull = Convert.ToInt32(contactInfoData["JobType"]);

                franchiseAccountDetail.Address1 = contactInfoData["Address1"];
                franchiseAccountDetail.Address2 = contactInfoData["Address2"];
                franchiseAccountDetail.City = contactInfoData["City"];
                franchiseAccountDetail.County = contactInfoData["County"];
                franchiseAccountDetail.State = contactInfoData["State"];
                franchiseAccountDetail.HomeNumber = contactInfoData["HomePhoneNumber"];
                franchiseAccountDetail.FaxNumber = contactInfoData["FaxNo"];
                franchiseAccountDetail.ZipCode = contactInfoData["ZipCode"];

                //Save Data Into account & FranchiseDetail
                var account = _crmService.SaveCRM_Account(accountfranchise);
                var accountFranchise = _crmService.SaveCRM_AccountFranchiseDetail(franchiseAccountDetail);

                return Json(new
                {
                    success = true,
                    accountFranchise = CRMAutoMapper.CrmAccountEntityToViewModel(account),
                    accountFranchiseDetail = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(accountFranchise)
                });
            }
            return Json(new
            {
                success = false,
                message = "No Account Or Franchise Data"
            });
        }
        #endregion

        #region CRMPotentialCustomerController > Save CustomerAccountSummary 

        [HttpPost]
        public ActionResult SaveFranchiseAccountSummary(System.Web.Mvc.FormCollection summaryData)
        {
            if (summaryData == null)
            {
                //NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
            //              $"CRM_AccountFranchiseDetailId={summaryData["CRM_AccountFranchiseDetailId"]}, " +
            //              $"CRM_StageStatus={summaryData["StageStatus"]}, " +
            //              $"CRM_ProviderType={summaryData["ProviderType"]}, " +
            //              $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
            //              $"BudgetAmount={summaryData["BudgetAmount"]}");

            var franchiseAccountDetail = _crmService.GetCRM_AccountFranchiseDetailbyId(int.Parse(summaryData["CRM_AccountFranchiseDetailId"]));
            var accountFranchise = _crmService.GetCRM_AccountbyId(int.Parse(summaryData["CRM_AccountId"]));
            if (franchiseAccountDetail != null && accountFranchise != null)
            {
                franchiseAccountDetail.AmtToInvest = Convert.ToDecimal(summaryData["BudgetAmount"]);
                franchiseAccountDetail.ModifiedBy = LoginUserId;

                accountFranchise.StageStatus = int.Parse(summaryData["StageStatus"]);
                accountFranchise.ProviderType = int.Parse(summaryData["ProviderType"]);
                accountFranchise.ProviderSource = int.Parse(summaryData["ProviderSource"]);
                accountFranchise.ModifiedBy = LoginUserId;

                accountFranchise = _crmService.SaveCRM_Account(accountFranchise);
                franchiseAccountDetail = _crmService.SaveCRM_AccountFranchiseDetail(franchiseAccountDetail);

                var franchiseAccountDetailViewModel = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(franchiseAccountDetail);
                franchiseAccountDetailViewModel.StageStatus = (StageStatusType)accountFranchise.StageStatus.Value;
                franchiseAccountDetailViewModel.ProviderSource = (AccountSourceProvider)accountFranchise.ProviderSource.Value;
                franchiseAccountDetailViewModel.ProviderType = (AccountProviderType)accountFranchise.ProviderType.Value;

                _hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = franchiseAccountDetailViewModel

                });
            }
            else
            {
                NLogger.Error("CustomerAccountDetail does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail does not exist" });
            }

        }

        #endregion

        #region Get Potential Lead Schedule
        [HttpGet]
        public ActionResult GetPotentialLeadSchedule(int accountFranchiseId)
        {
            var schedulevm = _crmService.GetCRM_Schedule_ByAccountFranchiseDetailId(accountFranchiseId);
            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();
            #region Calendar
            /*Schedules */

            DateTime mostRecentDate = DateTime.MinValue;

            if (schedulevm != null)
            {
                foreach (var schedule in schedulevm)
                {
                    CRMScheduleCalenderViewModel calendarvm = new CRMScheduleCalenderViewModel();

                    calendarvm.title = schedule.Title;
                    calendarvm.start = schedule.StartDate.Value.ToString("s");
                    calendarvm.EndDate = schedule.EndDate != null ? schedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                    calendarvm.StartDate = schedule.StartDate != null ? schedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                    calendarvm.StartTime = schedule.StartDate != null ? schedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                    calendarvm.EndTime = schedule.EndDate != null ? schedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                    calendarvm.Description = schedule.Description;
                    calendarvm.CRM_ScheduleTypeId = schedule.CRM_ScheduleTypeId.ToString();
                    calendarvm.IsAllDay = schedule.IsAllDay;
                    calendarvm.Location = schedule.Location;

                    calendarvm.backgroundColor = CRMGraphics.GetEventBackGroundColor(schedule.CRM_ScheduleTypeId);
                    accountCustomerScheduleCalendar.Add(calendarvm);

                    /*Set the Most Recent Date*/
                    if (mostRecentDate == null || schedule.EndDate > mostRecentDate)
                        mostRecentDate = (DateTime)schedule.EndDate;

                }
            }

            #endregion

            var data = new
            {
                success = true,
                calendar = accountCustomerScheduleCalendar,
                schedule = CRMAutoMapper.CrmScheduleEntitiesToViewModels(schedulevm),
                mostrecentDate = mostRecentDate

            };

            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMPotentialFranchiseController > GetContractTypeById

        [HttpGet]
        public ActionResult GetFranchiseContractTypeById(int id)
        {
            var franchiseeContractType = FranchiseeService.GetFranchiseeContractTypeList().FirstOrDefault(x => x.FranchiseeContractTypeListId == id);

            // var franchiseeContractType =  FranchiseeService.GetFranchiseeContractTypeList().Where(x => x.FranchiseeContractTypeListId == id).Select(x => new { Name = x.Name, Price = x.Price, Business = x.BusinessAmount , interest = x.Interest, NoOfPayments = x.NoOfPayments , DownPayments = x.DownPayment});

            if (franchiseeContractType != null)
            {
                var data = new
                {
                    success = V,
                    franchiseContractTypevm = new CRMFranchiseContractTypeListViewModel
                    {
                        Name = franchiseeContractType.Name,
                        Price = franchiseeContractType.Price,
                        BusinessAmount = franchiseeContractType.BusinessAmount,
                        DownPayment = franchiseeContractType.DownPayment,
                        Interest = franchiseeContractType.Interest,
                        NoOfPayments = franchiseeContractType.NoOfPayments,
                        DaysToFullfill = franchiseeContractType.DaysToFullfill
                    }

                };

                return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "The franchiseeContractType with " + id + "Not Found" }, behavior: JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Activity POPUP Http Request

        /// <summary>
        /// Get the Initial communication data as Lead Generation
        /// </summary>
        /// <param name="franchiseDetailbyId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LeadGenerationPopup(int franchiseDetailbyId)
        {
            var accountFranchiseInitialvm = CRMAutoMapper.CrmInitialEntityToViewModel(_crmService.GetCRM_InitialCommunication_ByAccountFranchiseDetailById(franchiseDetailbyId));

            CRMUtils.RenderPartialViewToString(this.ControllerContext, "_Partial_PP_LeadGeneration", accountFranchiseInitialvm);
            return PartialView("_Partial_PP_LeadGeneration", accountFranchiseInitialvm);

        }

        /// <summary>
        /// Get the Potential Inquary Data from the Lead generation Table
        /// </summary>
        /// <param name="franchiseDetailById"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult LeadPotentialInquaryPopup(int franchiseDetailById)
        {
            var schedulevm = new CRMScheduleViewModel();
            var leadgenvm = CRMAutoMapper.CrmLeadGenEntityToViewModel(_crmService.GetCRM_LeadGeneration_ByAccountFranchiseId(franchiseDetailById));

            if (leadgenvm != null)
            {
                schedulevm = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == leadgenvm.CRM_AccountFranchiseDetailId && x.CRM_StageStatusType == (int)StageStatusType.PotentialInquary));
                ViewBag.PotentialInquaryPurpose = new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMPotentialInquary == true).ToList(), "CRM_PurposeTypeId", "Name", schedulevm.PurposeId);
            };

            /* Put  Potential Inquary Schedule into ViewBag */
            ViewBag.Schedule = schedulevm;

            //return PartialView("_Partial_PP_PotentialInquary", leadgenvm);

            var data = new
            {
                success = V,
                leadgen = CRMUtils.RenderPartialViewToString(ControllerContext, "_Partial_PP_PotentialInquary", leadgenvm)
            };

            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get CRM Contract by CRM_AccountFranchiceDetailId 
        /// </summary>
        /// <param name="franchiseDetailById"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult PotentialContractPopup(int franchiseDetailById)
        {
            var docvm = new List<CRMDocumentViewModel>();
            var franchisecontractvm = CRMAutoMapper.CrmFranchiseContractEntityToViewModel(_crmService.GetCRM_FranchiseContract_ByAccountFranchiseId(franchiseDetailById));

            if (franchisecontractvm != null)
            {
                docvm = CRMAutoMapper.CrmDocumentEntitiesToViewModels(_crmService.GetCRM_Document_ByAccountFranchiseDetailId(franchisecontractvm.CRM_AccountFranchiseDetailId));
            }
            /*Put Franchise Document into the ViewBag*/
            ViewBag.Documents = docvm;

            var data = new
            {
                success = V,
                franchisecontract = CRMUtils.RenderPartialViewToString(ControllerContext, "_Partial_PP_FranchiseContract", franchisecontractvm)
            };
            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FollowUpPopUp(int franchiseDetailById)
        {

            var followupvm = CRMAutoMapper.CrmFranchiseFollowUpEntityToViewModel(_crmService.GetCRM_FranchiseFollowUp_ByAccountFranchiseDetailId(franchiseDetailById));


            if (followupvm != null)
            {
                var schedules = CRMAutoMapper.CrmScheduleEntitiesToViewModels(_crmService.GetCRM_Schedule_ByAccountFranchiseDetailId(franchiseDetailById).Where(x => x.CRM_StageStatusType == (int)StageStatusType.FollowUp).ToList());
                ViewBag.CloseType = new SelectList(_crmService.GetAll_CRM_CloseType().Where(x => x.CRMFranchiseFollowUp == true).ToList().OrderBy(x => x.Name), schedules[0].PurposeId, "Name");
                ViewBag.followupschedule = schedules;
            }
            else
            {
                ViewBag.followupschedule = null;
            }

            var data = new
            {
                success = V,
                followup = CRMUtils.RenderPartialViewToString(controllercontext: ControllerContext, viewname: "_Partial_PP_FollowUp", model: followupvm)
            };

            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OpenSignAgreementPP(int franchiseDetailById)
        {
            var data = new
            {
                success = V
            };

            return Json(data: data, behavior: JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult CRMUploadDocumentPopup(int id, int signagreement = 0)
        {
            ViewBag.Id = id;
            ViewBag.Stage = signagreement;
            var document = new List<CRMDocumentViewModel>();
            return PartialView("_CRMFranchiseUploadDocumentPopup", CustomerService.GetCRMDocumentByAccountFranchiseDetailId(id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee)));
            //return PartialView("_CRMUploadDocumentPopup");
        }

        #region :: CRM Upload Documents :: 

        [HttpPost]
        public ActionResult CRMSaveDocuments(System.Web.Mvc.FormCollection collection)
        {

            string selIds = Request["selIds"];
            string CRMAccFranchDetailId = Request["CRMAccFranchDetailId"];
            if (selIds != "")
            {
                string[] arrIds = selIds.Split(',');
                for (int i = 0; i < arrIds.Length; i++)
                {
                    if (Request.Files.Count > 0)
                    {
                        var crmdocumentViewModel = new CRMDocumentViewModel();

                        if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument")))
                        {
                            Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument"));
                        }
                        if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", CRMAccFranchDetailId.ToString())))
                        {
                            Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", CRMAccFranchDetailId.ToString()));
                        }


                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[i];
                        string DocumentId = arrIds[i];

                        //string fname = CRMAccFranchDetailId + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + file.FileName.Trim();
                        string _FileName = Path.GetFileName(file.FileName).Replace(" ", "_");

                        int _FileSize = Path.GetFileName(file.FileName).Length;
                        string _FileExt = Path.GetFileName(file.FileName).Split('.').Last();
                        string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                        string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", CRMAccFranchDetailId.ToString(), _SFileName);
                        //file.SaveAs(_path);

                        String _FilePath = "/FranchiseDocument/" + CRMAccFranchDetailId.ToString() + "/" + _SFileName;

                        var CRMModel = _crmService.GetCRMDocumentWithAccountCustomer_FileType(Convert.ToInt32(CRMAccFranchDetailId), Convert.ToInt32(DocumentId));
                        if (CRMModel != null && CRMModel.CRM_DocumentId > 0)
                        {
                            CRMModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                            CRMModel.File_Name = _SFileName;
                            CRMModel.Content_Type = file.ContentType;
                            CRMModel.Document_FilePath = _FilePath;
                            file.SaveAs(_path);
                            var crmdocument = _crmService.SaveCRM_Document(CRMModel);
                        }
                        else
                        {
                            crmdocumentViewModel.CRM_AccountFranchiseDetailId = Convert.ToInt32(CRMAccFranchDetailId);
                            crmdocumentViewModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                            crmdocumentViewModel.File_Name = _SFileName;
                            crmdocumentViewModel.Content_Type = file.ContentType;
                            crmdocumentViewModel.Description = "";
                            crmdocumentViewModel.FileTypeListId = Convert.ToInt32(DocumentId);
                            crmdocumentViewModel.Document_FilePath = _FilePath;
                            file.SaveAs(_path);
                            var crmdocument = _crmService.SaveCRM_Document(CRMAutoMapper.CrmDocumentViewModelToEntity(crmdocumentViewModel));
                        }
                    }
                }

                int CRM_AccFranchId = Convert.ToInt32(CRMAccFranchDetailId);
                var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountFranchiseDetailId == CRM_AccFranchId).ToList();
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

        public ActionResult RemoveCRM_Document(int Id, int CRMAccFranId)
        {
            if (Id > 0)
            {
                var getData = _crmService.GetUploadDocumentById(Id);
                if (getData != null && getData.CRM_DocumentId > 0 && getData.Document_FilePath != null)
                {
                    if (Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), getData.Document_FilePath)))
                    {
                        Directory.Delete(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), getData.Document_FilePath));
                    }
                }
                _crmService.DeleteCRM_Document(Id);
            }
            int CRM_AccFranId = Convert.ToInt32(CRMAccFranId);
            var crmdocuments = _crmService.GetAll_CRM_Document().Where(x => x.CRM_AccountFranchiseDetailId == CRM_AccFranId).ToList();
            return Json(new { success = true, document = CRMAutoMapper.CrmDocumentEntitiesToViewModels(crmdocuments) }, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public ActionResult DownloadDocument(int id = 0)
        {
            var crmDocument = _crmService.GetCRM_DocumentById(id);

            string mimeType = MimeMapping.GetMimeMapping(crmDocument.File_Name);
            if (string.IsNullOrEmpty(mimeType))
                mimeType = "application/octet-stream";

            Response.AddHeader("Content-Disposition", "inline; filename=" + crmDocument.File_Name);
            Response.AddHeader("X-Content-Type-Options", "nosniff");

            return File(Path.Combine(_fileDirectory + "Areas/CRM/Documents/", crmDocument.File_Name), mimeType);

        }

        #endregion
    }
}