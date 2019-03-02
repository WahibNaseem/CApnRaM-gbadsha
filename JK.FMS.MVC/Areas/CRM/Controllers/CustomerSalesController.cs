using Application.Web.Core;
using JK.FMS.MVC.Areas.CRM.Common;
using JK.Resources;
using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.Helper.Extension;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels;
using JKViewModels.Common;
using JKViewModels.CRM;
using JKViewModels.Customer;
using MvcBreadCrumbs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]

    public class CustomerSalesController : ViewControllerBase
    {
        #region LifeCycle
        public readonly ICRM_Service _crmService;
        public string _fileDirectory;
        public CustomerSalesController(ICacheProvider cacheProivder, ICRM_Service ICRMService, ICustomerService customerservice, ICommonService commonService, ClaimView claimview)
        {
            _cacheProvider = cacheProivder;
            _crmService = ICRMService;
            CustomerService = customerservice;
            _claimView = claimview;
            _commonService = commonService;
            _fileDirectory = ConfigurationManager.AppSettings[AppConstants.APPSETTINGS_FILESDIRECTORY] + "//";
        }

        #endregion

        #region CustomerSalesController > Helper
        private SelectList StageStatusList
        {
            get
            {
                var accountstagestatus = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_FvPresentation ||
                    x.Name == Constants.Key_Bidding ||
                    x.Name == Constants.Key_PdAppointment ||
                    x.Name == Constants.Key_FollowUp ||
                    x.Name == Constants.Key_Sold);

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
        #endregion

        // GET: CRM/CustomerSales
        public ActionResult Index()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "DashBoard", new { area = "CRM" }), "Sales DashBoard");
            return View();
        }

        #region Potential Customer
        public ActionResult Potential()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CustomerSales", new { area = "CRM" }), "Potential");

            //Passing 3 as a choice parameter
            //var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId);

            // Select list   
            ViewBag.UserId = SelectedUserId;
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");

            var SalesPersonList = _crmService.Get_AuthUserLogin_Potential(roleId != null ? roleId.RoleId : 7, this.LoginUserId, this.SelectedRegionId);
            ViewBag.SalesPersonList = SalesPersonList;
            ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");

            var lstRegion = SalesPersonList.Where(d => d.RegionId != null).GroupBy(x => new { x.RegionId, x.RegionName })
                .Select(d => new CRMScheduleUserHierarchy
                {
                    RegionId = d.First().RegionId,
                    RegionName = d.First().RegionName
                }).ToList();

            ViewBag.LstRegion = new SelectList(lstRegion, "RegionId", "RegionName", SelectedRegionId);
            ViewBag.SelectedRegionId = this.SelectedRegionId;
            //lstRegion.Select(x => new SelectListItem { Text = x.RegionName, Value = x.RegionId.ToString() });

            //if (roleId != null)
            //{
            //    var SalesPersonList = _crmService.Get_AuthUserLogin_Potential(roleId.RoleId, this.LoginUserId, this.SelectedRegionId);
            //    ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            //}
            //else
            //{
            //    var SalesPersonList = _crmService.Get_AuthUserLogin_Potential(7); //CRM Sales
            //    ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            //}
            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name");
            ViewBag.CallTime = new SelectList(CRMUtils.GetBestTimeList());
            ViewBag.ServiceTypeListModel = ServiceTypeListModel;
            ViewBag.CallResultList = new SelectList(_crmService.GetAll_CRM_CallResult().ToList().OrderBy(cr => cr.Name), "CRM_CallResultId", "Name");
            ViewBag.NoteType = new SelectList(_crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name), "CRM_NoteTypeId", "Name");
            ViewBag.SaleStageStatus = StageStatusList;

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

            ViewBag.ScheduleType = CustomerService.GetScheduleTyoeList();
            ViewBag.StageStatusType = _crmService.GetAll_CRM_StageStatus();
            ViewBag.PurposeTypeList = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name);
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId, this.LoginUserId);
            return View(potentialcustomerViewModel);
        }
        public JsonResult PotentialListData()
        {

            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId, this.LoginUserId);

            var jsonResult = Json(new
            {
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult PartialPotentialLoad(int id, int accountid, int ssid)
        {
            // Select list   
            ViewBag.UserId = SelectedUserId;
            //ViewBag.UserList = new SelectList(_crmService.Get_AuthUserLogin(), "UserId", "FirstName");
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");
            if (roleId != null)
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(roleId.RoleId);
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }

            if (!_cacheProvider.Contains(CacheKeyName.Potential_QualifiedLeadStageStatusList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_QualifiedLeadStageStatusList, QualifiedLeadStageStatusList);
            }
            ViewBag.QualifiedLeadStageStatusList = _cacheProvider.Get(CacheKeyName.Potential_QualifiedLeadStageStatusList); ;

            if (!_cacheProvider.Contains(CacheKeyName.Potential_GetAccountTypeList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_GetAccountTypeList, new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name"));
            }
            ViewBag.AccountTypeList = _cacheProvider.Get(CacheKeyName.Potential_GetAccountTypeList); ;

            if (!_cacheProvider.Contains(CacheKeyName.Potential_ProviderTypeList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_ProviderTypeList, ProviderTypeList);
            }
            ViewBag.ProviderTypeList = _cacheProvider.Get(CacheKeyName.Potential_ProviderTypeList);

            if (!_cacheProvider.Contains(CacheKeyName.Potential_ProviderSourceList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_ProviderSourceList, ProviderSourceList);
            }
            ViewBag.ProviderSourceList = _cacheProvider.Get(CacheKeyName.Potential_ProviderSourceList);


            if (!_cacheProvider.Contains(CacheKeyName.Potential_BiddingPurposeTypeList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_BiddingPurposeTypeList, new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMBidding == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name"));
            }
            ViewBag.BiddingPurposeTypeList = _cacheProvider.Get(CacheKeyName.Potential_BiddingPurposeTypeList);

            if (!_cacheProvider.Contains(CacheKeyName.Potential_FollowUpPurposeTypeList))
            {
                _cacheProvider.Set(CacheKeyName.Potential_FollowUpPurposeTypeList, new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMFollowup == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name"));
            }
            ViewBag.FollowUpPurposeTypeList = _cacheProvider.Get(CacheKeyName.Potential_FollowUpPurposeTypeList);

            if (!_cacheProvider.Contains(CacheKeyName.Potential_CloseType))
            {
                _cacheProvider.Set(CacheKeyName.Potential_CloseType, new SelectList(_crmService.GetAll_CRM_CloseType().Where(x => x.CRMFollowUp == true).ToList().OrderBy(x => x.Name), "CRM_CloseTypeId", "Name"));
            }
            ViewBag.CloseType = _cacheProvider.Get(CacheKeyName.Potential_CloseType);


            //Array itemValues = System.Enum.GetValues(typeof(Response));
            //Array itemNames = System.Enum.GetNames(typeof(Response));

            //for (int i = 0; i <= itemNames.Length - 1; i++)
            //{
            //    ListItem item = new ListItem(itemNames[i], itemValues[i]);
            //    dropdownlist.Items.Add(item);
            //}

            if (!_cacheProvider.Contains(CacheKeyName.Potential_State))
            {
                _cacheProvider.Set(CacheKeyName.Potential_State, new SelectList(CustomerService.GetStateList(), "abbr", "Name"));
            }
            ViewBag.State = _cacheProvider.Get(CacheKeyName.Potential_State);


            ViewBag.CallTime = new SelectList(CRMUtils.GetBestTimeList());

            if (!_cacheProvider.Contains(CacheKeyName.Potential_ServiceTypeListModel))
            {
                _cacheProvider.Set(CacheKeyName.Potential_ServiceTypeListModel, ServiceTypeListModel);
            }
            ViewBag.ServiceTypeListModel = _cacheProvider.Get(CacheKeyName.Potential_ServiceTypeListModel);

            if (!_cacheProvider.Contains(CacheKeyName.Potential_FrequencyListModel))
            {
                _cacheProvider.Set(CacheKeyName.Potential_FrequencyListModel, _crmService.GetFrequencyList().ToList());
            }
            var FrequencyListModel = (List<FrequencyList>)_cacheProvider.Get(CacheKeyName.Potential_FrequencyListModel);
            if (FrequencyListModel != null)
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
            }

            if (!_cacheProvider.Contains(CacheKeyName.Potential_CleanFrequencyListModel))
            {
                _cacheProvider.Set(CacheKeyName.Potential_CleanFrequencyListModel, _crmService.GetCleanFrequencyList().ToList());
            }
            var CleanFrequencyListModel = (List<CleanFrequencyListViewModel>)_cacheProvider.Get(CacheKeyName.Potential_CleanFrequencyListModel);
            if (CleanFrequencyListModel != null)
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
            }


            if (ssid == 21)
            {
                return PartialView("_Partial_FV_Presentation");
            }
            else if (ssid == 22)
            {
                return PartialView("_Partial_BiddingDetail");
            }
            else if (ssid == 23)
            {
                return PartialView("_Partial_PD_Appointment");
            }
            else if (ssid == 24)
            {
                return PartialView("_Partial_FollowUp");
            }
            else if (ssid == 25)
            {
                return PartialView("_Partial_Close");
            }
            else if (ssid == 30)
            {
                return PartialView("_Partial_Sold");
            }
            return View();
        }

        #region CRMLeadCustomerController > Helpers

        private SelectList ServiceTypeListModel
        {
            get
            {
                var list = _crmService.GetServiceTypeList().ToList().Where(o => o.TypeListId == 1 && o.contracttype == 1).ToList();
                if (list != null)
                    return new SelectList(list.OrderBy(x => x.name).ThenBy(x => x.name), "ServiceTypeListId", "name");
                return null;
            }
        }

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
                 x.Name == Constants.Key_Sold ||
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




        #endregion

        #region CRMPotentialCustomerController > HttpRequest   

        [HttpGet]
        public ActionResult PotentialCustomerSearch(int type = 0, int usr = 0, string region = null)
        {
            //Passing 3 as a choice parameter
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer_SearchRegionSalesPerson(3, type, usr, region, this.LoginUserId);
            return Json(new
            {
                success = true,
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartialPotentialLoadPerf(int accountId)
        {
            DateTime start = DateTime.Now;
            var account = _crmService.GetCRM_AccountbyId(accountId);
            ViewBag.UserId = SelectedUserId;
            //ViewBag.UserList = new SelectList(_crmService.Get_AuthUserLogin(), "UserId", "FirstName");
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");
            if (roleId != null)
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(roleId.RoleId);
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }
            if (account.StageStatus == 21)
            {
                if (!_cacheProvider.Contains(CacheKeyName.Potential_ServiceTypeListModel))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_ServiceTypeListModel, ServiceTypeListModel);
                }
                ViewBag.ServiceTypeListModel = _cacheProvider.Get(CacheKeyName.Potential_ServiceTypeListModel);
                return PartialView("_Partial_FV_Presentation");
            }
            if (account.StageStatus == 22)
            {
                if (!_cacheProvider.Contains(CacheKeyName.Potential_BiddingPurposeTypeList))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_BiddingPurposeTypeList, new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMBidding == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name"));
                }
                ViewBag.BiddingPurposeTypeList = _cacheProvider.Get(CacheKeyName.Potential_BiddingPurposeTypeList);
                return PartialView("_Partial_BiddingDetail");
            }
            if (account.StageStatus == 23)
            {
                if (!_cacheProvider.Contains(CacheKeyName.Potential_BiddingPurposeTypeList))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_BiddingPurposeTypeList, new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMBidding == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name"));
                }
                ViewBag.BiddingPurposeTypeList = _cacheProvider.Get(CacheKeyName.Potential_BiddingPurposeTypeList);
                return PartialView("_Partial_PD_Appointment");
            }
            if (account.StageStatus == 24)
            {
                if (!_cacheProvider.Contains(CacheKeyName.Potential_CloseType))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_CloseType, new SelectList(_crmService.GetAll_CRM_CloseType().Where(x => x.CRMFollowUp == true).ToList().OrderBy(x => x.Name), "CRM_CloseTypeId", "Name"));
                }
                ViewBag.CloseType = _cacheProvider.Get(CacheKeyName.Potential_CloseType);
                if (!_cacheProvider.Contains(CacheKeyName.Potential_FollowUpPurposeTypeList))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_FollowUpPurposeTypeList, new SelectList(_crmService.GetAll_CRM_PurposeType().Where(x => x.CRMFollowup == true).ToList().OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name"));
                }
                ViewBag.FollowUpPurposeTypeList = _cacheProvider.Get(CacheKeyName.Potential_FollowUpPurposeTypeList);
                return PartialView("_Partial_FollowUp");
            }

            if (account.StageStatus == 25 || account.StageStatus == 30)
            {
                if (!_cacheProvider.Contains(CacheKeyName.Potential_ServiceTypeListModel))
                {
                    _cacheProvider.Set(CacheKeyName.Potential_ServiceTypeListModel, ServiceTypeListModel);
                }
                ViewBag.ServiceTypeListModel = _cacheProvider.Get(CacheKeyName.Potential_ServiceTypeListModel);

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
                if (account.StageStatus == 25)
                    return PartialView("_Partial_Close");
                if (account.StageStatus == 30)
                    return PartialView("_Partial_Sold");
            }

            return View();
        }
        #endregion
        [HttpGet]
        public JsonResult GetPotentialDetail(int accountId)
        {
            DateTime start = DateTime.Now;
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

            using (jkDatabaseEntities db = new jkDatabaseEntities())
            {

                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[dbo].[crm_spGet_Potential_Details]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AccountId", accountId));
                cmd.Parameters.Add(new SqlParameter("@LoginUser", LoginUserId));
                db.Database.Connection.Open();
                // Run the sproc
                var reader = cmd.ExecuteReader();
                var account = ((IObjectContextAdapter)db)
                .ObjectContext
                .Translate<CRM_Account>(reader).FirstOrDefault();

                reader.NextResult();
                var accountCustomerDetail = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_AccountCustomerDetail>(reader).FirstOrDefault();

                reader.NextResult();
                var accountActivity = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Activity>(reader).ToList();

                reader.NextResult();
                var accountSchedule = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Schedule>(reader);

                reader.NextResult();
                var accountNote = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Note>(reader).ToList();

                reader.NextResult();
                var accountDocument = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Document>(reader).ToList();

                reader.NextResult();
                var leadschedule = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Schedule>(reader);

                reader.NextResult();
                var accountInitialCommunication = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_InitialCommunication>(reader).FirstOrDefault();

                reader.NextResult();
                var accountFvPresentation = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_FvPresentation>(reader).FirstOrDefault();

                reader.NextResult();
                var accountBidding = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Bidding>(reader).FirstOrDefault();

                reader.NextResult();
                var accountPdAppointment = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_PdAppointment>(reader).FirstOrDefault();

                reader.NextResult();
                var accountFollowUp = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_FollowUp>(reader).FirstOrDefault();

                reader.NextResult();
                var accountClose = ((IObjectContextAdapter)db)
                 .ObjectContext
                 .Translate<CRM_Close>(reader).FirstOrDefault();



                if (account != null)
                {
                    accountCustomerDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomerDetail);
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
                        //bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                        bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                    }
                    else
                    {
                        accountCustomerBiddingViewModel = null;
                    }

                    //PdAppointment
                    //pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                    pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));

                    if (accountPdAppointment != null)
                    {
                        accountCustomerPdAppointmentViewModel = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(accountPdAppointment);
                        pdContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
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
                        accountCustomerFollowUpViewModel = CRMAutoMapper.CrmFollowupEntityToViewModel(accountFollowUp);
                        //followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.FollowUp));
                        followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.FollowUp));

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
                        accountCustomerSoldViewModel = CRMAutoMapper.CrmCloseEntityToViewModel(accountClose);
                    }
                    else
                    {
                        accountCustomerSoldViewModel = null;
                    }
                }

                DateTime end = DateTime.Now;
                double ms = (end - start).TotalMilliseconds;
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
                    pdContact,
                    pdSchedule = pdSch,
                    followSchedule = followSch,
                    assigneeId = account.AssigneeId

                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region CRMPotentialCustomerController > Get Potential Detail
        [HttpGet]
        public JsonResult GetPotentialDetailOld(int accountId)
        {
            //GetPotentialDetailPerf1(accountId);
            DateTime start = DateTime.Now;
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
            //var leadschedule = _crmService.GetAll_CRM_Schedule().Where(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();   //old
            var leadschedule = _crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId);

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
                    //bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                    bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.Bidding));
                }
                else
                {
                    accountCustomerBiddingViewModel = null;
                }

                //PdAppointment
                //pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));

                if (accountPdAppointment != null)
                {
                    accountCustomerPdAppointmentViewModel = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(accountPdAppointment);
                    pdContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
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
                    accountCustomerFollowUpViewModel = CRMAutoMapper.CrmFollowupEntityToViewModel(accountFollowUp);
                    //followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.ClassId == accountCustomerDetail.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.FollowUp));
                    followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_ScheduleWithCustomer(accountCustomerDetail.CRM_AccountCustomerDetailId).OrderByDescending(x => x.CreatedDate).FirstOrDefault(x => x.CRM_StageStatusType == (int)StageStatusType.FollowUp));

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
                    accountCustomerSoldViewModel = CRMAutoMapper.CrmCloseEntityToViewModel(accountClose);
                }
                else
                {
                    accountCustomerSoldViewModel = null;
                }
            }
            DateTime end = DateTime.Now;
            double ms = (end - start).TotalMilliseconds;
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
                pdContact,
                pdSchedule = pdSch,
                followSchedule = followSch,
                assigneeId = account.AssigneeId

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion   

        [HttpGet]
        public ActionResult GetLeadSchedules(int id)
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.ClassId == id).ToList();

            #region Calendar    
            //Schedule

            DateTime mostRecentDate = DateTime.MinValue;

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

                    if (mostRecentDate == null || accountschedule.EndDate > mostRecentDate)
                        mostRecentDate = (DateTime)accountschedule.EndDate;
                }
            }

            #endregion

            return Json(new
            {
                success = true,
                schedule = accountCustomerScheduleViewModels,
                result = accountCustomerScheduleCalendar,
                mostrecent = mostRecentDate
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchedulesForToday()
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.StartDate).Value == DateTime.Today.Date).ToList();

            #region Calendar

            //Schedule

            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountCustomerScheduleViewModels)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    if (accountschedule.Title == null)
                        accountschedule.Title = string.Empty;
                    calendar.title = accountschedule.Title;
                    calendar.start = accountschedule.StartDate.Value.ToString("s");
                    calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                    if (accountschedule.Description == null)
                        accountschedule.Description = string.Empty;
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
                schedule = accountCustomerScheduleViewModels.OrderBy(o => o.StartDate),
                result = accountCustomerScheduleCalendar.OrderBy(o => o.StartDate),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchedulesForLeadAssignee(int userId)
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.AuthUserLoginId == userId).ToList();
            var userDetails = _crmService.GetAuthUserLoginById(userId);

            #region Calendar

            //Schedule

            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountCustomerScheduleViewModels)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    calendar.CRM_ScheduleId = accountschedule.CRM_ScheduleId;
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
                    calendar.PurposeId = accountschedule.PurposeId;

                    calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_ScheduleTypeId);
                    accountCustomerScheduleCalendar.Add(calendar);
                }
            }

            #endregion

            return Json(new
            {
                success = true,
                schedule = accountCustomerScheduleViewModels,
                result = accountCustomerScheduleCalendar,
                username = userDetails == null ? "" : userDetails.FirstName + " " + userDetails.LastName
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidateScheduleAvailability(int userId, DateTime startDate, DateTime endDate)
        {
            if (userId <= 0 || startDate > endDate) // something went wrong with parameters, cancel
            {
                return Json(new
                {
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.AuthUserLoginId == userId).ToList();

            accountSchedule = accountSchedule.Where(t => t.StartDate > startDate && t.StartDate < endDate).ToList();

            var isFree = true;

            if (accountSchedule != null && accountSchedule.Count > 0)
            {
                //foreach (var sch in accountSchedule)
                //{
                //    if ((sch.StartDate >= startDate && sch.StartDate <= endDate) || // schedule starts inside timespan
                //        (sch.EndDate >= startDate && sch.EndDate <= endDate) || // schedule ends inside timespan
                //        (sch.StartDate <= startDate && sch.EndDate >= endDate)) // schedule encompasses whole timespan
                //    {
                //        isFree = false;
                //        break;
                //    }


                //}

                isFree = false;
            }

            #endregion

            return Json(new
            {
                success = true,
                result = isFree
            }, JsonRequestBehavior.AllowGet);
        }

        #region CRMPotentialCustomerController > Save CustomerAccountSummary 

        [HttpPost]
        public ActionResult SaveCustomerAccountSummary(FormCollection summaryData)
        {
            if (summaryData == null)
            {
                //NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
            //              $"CRM_AccountCustomerDetailId={summaryData["CRM_AccountCustomerDetailId"]}, " +
            //              $"CRM_StageStatus={summaryData["StageStatus"]}, " +
            //              $"CRM_ProviderType={summaryData["ProviderType"]}, " +
            //              $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
            //              $"BudgetAmount={summaryData["BudgetAmount"]}");

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

                //_hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = customerAccountDetailViewModel

                });
            }
            else
            {
                //NLogger.Error("CustomerAccountDetail does not exist");
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
                //NLogger.Error("Requested activityData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //DateTime timestamp = DateTime.ParseExact(activityData["TimeStamp"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            //NLogger.Debug($"CRM_AccountCustomerDetailId={activityData["CRM_AccountCustomerDetailId"]}, " +
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
                //NLogger.Error("Requested scheduleData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountCustomerDetailId={scheduleData["AccountDetailId"]}, " +
            //             $"Title={scheduleData["input_scheduletitle"]}, " +
            //             $"Description={scheduleData["input_scheduledescription"]}, " +
            //             $"StartDate={scheduleData["input_schedulestartdate"]}," +
            //             $"ScheduleType={scheduleData["input_scheduletype"]}," +
            //             $"EndDate={scheduleData["input_scheduleenddate"]}");

            var vm = new CRMScheduleViewModel();
            vm.ClassId = Convert.ToInt32(scheduleData["AccountDetailId"]);
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

            return Json(new { success = true, id = newSchedule.ClassId });



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

        public ActionResult SaveCRMSchedule(CustomerServiceScheduleDataModel model)
        {
            model.RegionId = SelectedRegionId;

            #region Mapping model.....
            var schedule = new CRM_Schedule();

            var stDate = DateTime.Now;
            var eDate = DateTime.Now;

            if (model.StartDateString != null)
            {
                stDate = Convert.ToDateTime(model.StartDateString);

                if (model.IsAllDay)
                {
                    stDate = new DateTime(stDate.Year, stDate.Month, stDate.Day, 8, 0, 0);
                }
                else
                {
                    stDate = model.StartDateString != null ? Convert.ToDateTime(model.StartDateString + " " + model.StartTimeString) : DateTime.Now;
                }

            }
            if (model.EndDateString != null)
            {
                eDate = Convert.ToDateTime(model.EndDateString);

                if (model.IsAllDay)
                {
                    eDate = new DateTime(eDate.Year, eDate.Month, eDate.Day, 17, 0, 0);
                }
                else
                {
                    eDate = model.EndDateString != null ? Convert.ToDateTime(model.EndDateString + " " + model.EndTimeString) : DateTime.Now;
                }

            }

            schedule.CRM_ScheduleId = model.CRM_ScheduleId;
            schedule.Title = model.Title;
            schedule.Description = model.Description;


            schedule.StartDate = stDate; // model.StartDateString!=null?Convert.ToDateTime(model.StartDateString + " " + model.StartTimeString) : DateTime.Now;
            schedule.IsFromOutlook = model.IsFromOutlook;

            schedule.OutlookSyncDate = null;
            schedule.Location = model.Location;
            schedule.EndDate = eDate; // model.EndDateString != null ? Convert.ToDateTime(model.EndDateString + " " + model.EndTimeString) : DateTime.Now;
            schedule.IsAllDay = model.IsAllDay;
            schedule.CRM_ScheduleTypeId = model.CRM_ScheduleTypeId;
            schedule.RegionId = SelectedRegionId;
            schedule.AuthUserLoginId = model.AuthUserLoginId;
            schedule.PurposeId = model.PurposeId;
            schedule.Purpose = model.Purpose;
            schedule.IsActive = true;
            schedule.ClassId = model.ClassId;

            schedule.TypeListId = model.TypeListId;
            schedule.CRM_StageStatusType = model.CRM_StageStatusType;
            #endregion Mapping model.....
            var data = _crmService.SaveCRM_ScheduleData(schedule);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region CRMPotentialCustomerController > Save CustomerNote


        [HttpPost]
        public ActionResult SaveAccountCustomerNote(FormCollection noteData)
        {
            if (noteData == null)
            {
                //NLogger.Error("Requested noteData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountCustomerDetailId={noteData["CRM_AccountCustomerDetailId"]}, " +
            //             $"Title={noteData["Title"]}, " +
            //             $"Description={noteData["Description"]} ");

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

        #region CRMPotentialCustomerController > Save Call Log


        [HttpPost]
        public ActionResult SaveAccountCustomerCallLog(FormCollection calllogData)
        {
            if (calllogData == null)
            {
                //NLogger.Error("Requested noteData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountCustomerDetailId={noteData["CRM_AccountCustomerDetailId"]}, " +
            //             $"Title={noteData["Title"]}, " +
            //             $"Description={noteData["Description"]} ");

            #region Create Call Log
            var callLogvm = new CRMCallLogViewModel();
            callLogvm.CRM_AccountCustomerDetailId = Convert.ToInt32(calllogData["CRM_AccountCustomerDetailId"]);
            callLogvm.CRM_AccountId = Convert.ToInt32(calllogData["CRM_AccountId"]);
            //callLogvm.CRM_LeadSource = accountcustomer.ProviderType ?? default(int);
            callLogvm.CRM_CallResultId = Convert.ToInt32(calllogData["CallResultId"]);
            callLogvm.CRM_NoteTypeId = Convert.ToInt32(calllogData["NoteType"]);
            callLogvm.SpokeWith = calllogData["SpokeWith"];
            callLogvm.Callback = Convert.ToDateTime(calllogData["CallBack"]);
            callLogvm.CRM_LeadSource = Convert.ToInt32(calllogData["LeadSource"]);
            callLogvm.Note = calllogData["Note"];
            callLogvm.CallLogDate = DateTime.Now;
            callLogvm.StageStatus = Convert.ToInt32(calllogData["StatusId"]);
            callLogvm.CreatedBy = LoginUserId;

            /*Save Call Log*/
            var calllog = _crmService.SaveCRM_CallLog(CRMAutoMapper.CrmCallLogViewModelToEntity(callLogvm));
            #endregion

            return Json(new
            {
                success = true,
                result = calllog

            }, JsonRequestBehavior.AllowGet);

            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
        }

        #endregion

        #region CRMPotentialCustomerController > Save CustomerContactInfo 

        [HttpPost]
        public ActionResult SaveCustomerContactInfo(FormCollection contactInfoData)
        {
            if (contactInfoData == null)
            {
                //NLogger.Error("Requested SaveCustomerContactInfo is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountCustomerDetailId={contactInfoData["CRM_AccountCustomerDetailId"]}, " +
            //            $"CRM_AccountId={contactInfoData["CRM_AccountId"]}, " +
            //            $"ContactName={contactInfoData["ContactName"]}," +
            //            $"Title={contactInfoData["Title"]}, " +
            //            $"PhoneNumber={contactInfoData["PhoneNumber"]}, " +
            //            $"EmailAddress={contactInfoData["EmailAddress"]}," +
            //            $"CompanyName={contactInfoData["CompanyName"]}, " +
            //            $"IndustryType={contactInfoData["IndustryType"]}, " +
            //            $"NumberOfLocations={contactInfoData["NumberOfLocations"]}," +
            //            $"CompanyPhoneNumber={contactInfoData["CompanyPhoneNumber"]}, " +
            //            $"CompanyFaxNumber={contactInfoData["CompanyFaxNumber"]}, " +
            //            $"CompanyEmailAddress={contactInfoData["CompanyEmailAddress"]}," +
            //            $"CompanyAddressLine1={contactInfoData["CompanyAddressLine1"]}, " +
            //            $"CompanyAddressLine2={contactInfoData["CompanyAddressLine2"]}, " +
            //            $"CompanyCity={contactInfoData["CompanyCity"]}," +
            //            $"CompanyCounty={contactInfoData["CompanyCounty"]}," +
            //            $"CompanyState={contactInfoData["CompanyState"]}," +
            //            $"CompanyZipCode={contactInfoData["CompanyZipCode"]}," +
            //            $"SqFt={contactInfoData["Sqft"]}," +
            //            $"SalesVolume={contactInfoData["SalesVolume"]}," +
            //            $"LineofBusiness={contactInfoData["LineofBusiness"]}," +
            //            $"CompnayWebSite={contactInfoData["CompnayWebSite"]}");

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

                customerAccountDetail.Title = Convert.ToString(contactInfoData["Title"]);
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
                customerAccountDetail.SalesVolume = Convert.ToString(contactInfoData["LineofBusiness"]);
                customerAccountDetail.LineofBusiness = Convert.ToString(contactInfoData["SalesVolume"]);

                customerAccountDetail.ModifiedBy = LoginUserId;

                var account = _crmService.SaveCRM_Account(accountcustomer);
                var accountCustomer = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);
                //_hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = CRMAutoMapper.CrmAccountEntityToViewModel(account),
                    accountcustomer = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomer)
                });
            }

            else
            {
                //NLogger.Error("CustomerAccountDetail and CustomerAccount does not exist");
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
                //NLogger.Error("Requested SaveInitialSummaryDate is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={initialData["CRM_AccountId"]}, " +
            //             $"CRM_AccountCustomerDetailId={initialData["CRM_AccountCustomerDetailId"]}, " +
            //            $"ContactPerson={initialData["ContactPerson"]}," +
            //            $"InterestedInProposal={initialData["InterestedInProposal"]}, " +
            //            $"AvailableToMeet={initialData["AvailableToMeet"]} " +
            //            $"Purpose={initialData["Purpose"]}" +
            //            $"Note={initialData["Note"]}");
            DateTime availabletomeet = DateTime.ParseExact(initialData["AvailableToMeet"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            var crmInital = _crmService.AddNewInitalData(
                Convert.ToInt32(initialData["CRM_AccountId"]),
                Convert.ToInt32(initialData["CRM_AccountCustomerDetailId"]),
                initialData["ContactPerson"],
               Convert.ToInt32(initialData["InterestedInProposal"]),
                availabletomeet,
                Convert.ToInt32(initialData["Purpose"]),
                initialData["Note"], LoginUserId, SelectedRegionId
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
                //NLogger.Error("Requested fvPresentationSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={fvSummaryData["CRM_AccountId"]}, " +
            //            $"CRM_AccountCustomerDetailId={fvSummaryData["CRM_AccountCustomerDetailId"]}, " +
            //           $"MeasureContactPerson={fvSummaryData["MeasureContactPerson"]}," +
            //           $"MeasureFacility={fvSummaryData["MeasureFacility"]}, " +
            //           $"NumberOfFloors={fvSummaryData["NumberOfFloors"]} " +
            //           $"Frequency={fvSummaryData["Frequency"]}" +
            //            $"CleaningDay={fvSummaryData["CleaningDay"]}, " +
            //           $"ServiceLevel={fvSummaryData["ServiceLevel"]} " +
            //           $"CleanFrequency={fvSummaryData["CleanFrequency"]} " +
            //           $"Budget={fvSummaryData["Budget"]}" +
            //           $"Note={fvSummaryData["Note"]}");

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


            crmfvPresentationViewmodel.Mon = fvSummaryData["Mon"] != null ? Convert.ToBoolean(fvSummaryData["Mon"]) : false;
            crmfvPresentationViewmodel.Tue = fvSummaryData["Tue"] != null ? Convert.ToBoolean(fvSummaryData["Tue"]) : false;
            crmfvPresentationViewmodel.Wed = fvSummaryData["Wed"] != null ? Convert.ToBoolean(fvSummaryData["Wed"]) : false;
            crmfvPresentationViewmodel.Thu = fvSummaryData["Thu"] != null ? Convert.ToBoolean(fvSummaryData["Thu"]) : false;
            crmfvPresentationViewmodel.Fri = fvSummaryData["Fri"] != null ? Convert.ToBoolean(fvSummaryData["Fri"]) : false;
            crmfvPresentationViewmodel.Sat = fvSummaryData["Sat"] != null ? Convert.ToBoolean(fvSummaryData["Sat"]) : false;
            crmfvPresentationViewmodel.Sun = fvSummaryData["Sun"] != null ? Convert.ToBoolean(fvSummaryData["Sun"]) : false;
            crmfvPresentationViewmodel.Weekend = fvSummaryData["Weekend"] != null ? Convert.ToBoolean(fvSummaryData["Weekend"]) : false;

            crmfvPresentationViewmodel.IsActive = true;
            crmfvPresentationViewmodel.RegionId = SelectedRegionId;
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
                //NLogger.Error("Requested BiddingSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            _crmService.UpdateInActiveCRMLeadStageData(Convert.ToInt32(biddingData["CRM_AccountCustomerDetailId"]));

            var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();
            var crmBiddingViewModel = new CRMBiddingViewModel();

            crmBiddingViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(biddingData["CRM_AccountCustomerDetailId"]);
            crmBiddingViewModel.AnalysisWorkBook = bool.Parse(biddingData["IsAnalysisWorkBook"]);
            crmBiddingViewModel.IsBidSheet = bool.Parse(biddingData["IsBidSheet"]);
            crmBiddingViewModel.IsCancellation = bool.Parse(biddingData["IsCancellation"]);

            crmBiddingViewModel.MonthlyPrice = Convert.ToDecimal(biddingData["MonthlyPrice"]);
            crmBiddingViewModel.PriceApproved = Convert.ToInt32(biddingData["PriceApproved"]);
            crmBiddingViewModel.IfBidOver = true;
            crmBiddingViewModel.IncludePrice = Convert.ToDecimal(biddingData["IncludePrice"]);
            crmBiddingViewModel.Note = biddingData["Note"];
            crmBiddingViewModel.CreatedBy = LoginUserId;
            crmBiddingViewModel.PurposeId = Convert.ToInt32(biddingData["PruposeId"]);
            crmBiddingViewModel.Purpose = biddingData["Prupose"];
            crmBiddingViewModel.IsActive = true;
            crmBiddingViewModel.RegionId = SelectedRegionId;
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
            var schedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(crmbidding.CRM_AccountCustomerDetailId).FirstOrDefault();
            schedule.ClassId = crmbidding.CRM_AccountCustomerDetailId;
            schedule.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
            //schedule.StartDate = biddingData["ScheduleStartDate"] != "" ? Convert.ToDateTime(biddingData["ScheduleStartDate"] + " " + biddingData["ScheduleStartTime"]) : DateTime.Now;
            //schedule.EndDate = biddingData["ScheduleEndDate"] != "" ? Convert.ToDateTime(biddingData["ScheduleEndDate"] + " " + biddingData["ScheduleEndTime"]) : DateTime.Now;
            //schedulevm.Title = accountcustomer.CRM_AccountId + " - " + "BD" + " " + biddingData["Prupose"] + " - " + accountcustomer.ContactName + " - " + contact.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;
            //schedulevm.Location = customerAccountDetail.CompanyAddressLine1;
            schedule.AuthUserLoginId = accountcustomer.AssigneeId;
            schedule.CRM_StageStatusType = (int)StageStatusType.Bidding;
            //schedule.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
            schedule.RegionId = SelectedRegionId;
            schedule.CreatedBy = LoginUserId;

            //schedule.PurposeId = Convert.ToInt32(biddingData["PruposeId"]);
            //schedule.Purpose = biddingData["Prupose"];
            schedule.IsActive = true;
            var sched = _crmService.SaveCRM_Schedule(schedule);

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

            if (Convert.ToInt32(biddingData["PriceApproved"]) > 0)
            {

                string PriceApprovedName = "";

                Array enumValueArray = Enum.GetValues(typeof(priceapproved));
                foreach (int enumValue in enumValueArray)
                {
                    if (enumValue == Convert.ToInt32(biddingData["PriceApproved"]))
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
                            var getRegionUser = jkEntityModel.AuthUserRegions.Where(x => x.RegionId == SelectedRegionId).ToList();

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

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmBiddingEntityToViewModel(crmbidding),
                contact = contact,
                stageStatusSchedule = CRMAutoMapper.CrmScheduleEntityToViewModel(schedule), //schedulevm
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
                //NLogger.Error("Requested pdSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            int intRes;

            var PdViewModel = new CRMPdAppointmentViewModel();

            PdViewModel.CRM_AccountCustomerDetailId = int.Parse(pdSummaryData["CRM_AccountCustomerDetailId"]);
            PdViewModel.MeetPersonName = pdSummaryData["DMeetWithPerson"];
            PdViewModel.MeetDecisionMaker = bool.Parse(pdSummaryData["MeetWithDecisionMaker"]);
            PdViewModel.PresentProposal = bool.Parse(pdSummaryData["PresentProposal"]);

            if (int.TryParse(pdSummaryData["OverComeObjection"], out intRes))
                PdViewModel.OverComeObjections = Convert.ToBoolean(intRes);
            else
                PdViewModel.OverComeObjections = true;

            PdViewModel.Comment = pdSummaryData["Comment"];
            //PdViewModel.EnteredProposalDetail = bool.Parse(pdSummaryData["ProposalDetail"]);
            PdViewModel.Note = pdSummaryData["Note"];
            PdViewModel.CreatedBy = LoginUserId;
            PdViewModel.PurposeId = Convert.ToInt32(pdSummaryData["CallBack_PurposeId"]);
            PdViewModel.Purpose = pdSummaryData["CallBack_Purpose"];
            PdViewModel.IsActive = true;
            PdViewModel.RegionId = SelectedRegionId;
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
            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(int.Parse(pdSummaryData["CRM_AccountId"]));

            //CRMScheduleViewModel scheduleVM = null;
            var schedule = new CRM_Schedule();

            if (PdViewModel.PurposeId != 12) // create Schedule only if not "Contract Signed"
            {
                var schedulevm = new CRMScheduleViewModel();
                schedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(pdappointment.CRM_AccountCustomerDetailId).LastOrDefault();

                schedule.ClassId = pdappointment.CRM_AccountCustomerDetailId;
                schedule.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
                //schedulevm.Title = CRMUtils.SchedulePurpose(Convert.ToInt32(pdSummaryData["CallBack_Purpose"]));

                //schedulevm.Title = account.CRM_AccountId + " - " + "PD" + " - " + pdSummaryData["CallBack_Purpose"] + " - " + account.ContactName + " - " + contact.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;

                //schedulevm.StartDate = pdSummaryData["CallBackStartDate"] != "" ? Convert.ToDateTime(pdSummaryData["CallBackStartDate"] + " " + pdSummaryData["CallBackStartTime"]) : DateTime.Now;
                //schedulevm.EndDate = pdSummaryData["CallBackEndDate"] != "" ? Convert.ToDateTime(pdSummaryData["CallBackEndDate"] + " " + pdSummaryData["CallBackEndTime"]) : DateTime.Now;
                schedule.CRM_StageStatusType = (int)StageStatusType.PdAppointment;
                //schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                schedule.RegionId = SelectedRegionId;
                schedule.CreatedBy = LoginUserId;
                schedule.AuthUserLoginId = account.AssigneeId;
                //schedulevm.PurposeId = Convert.ToInt32(pdSummaryData["CallBack_PurposeId"]);
                //schedulevm.Purpose = pdSummaryData["CallBack_Purpose"];
                schedule.IsActive = true;
                //scheduleVM = schedulevm;
                //scheduleVM = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm)));
                //scheduleVM.PurposeId = Convert.ToInt32(pdSummaryData["CallBack_PurposeId"]);
                _crmService.SaveCRM_Schedule(schedule);

            }

            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(pdSummaryData["CRM_AccountId"]));
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

                accountcustomer.ModifiedBy = LoginUserId;
                _crmService.SaveCRM_Account(accountcustomer);
            }

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(pdappointment),
                contact = contact,
                schedule = CRMAutoMapper.CrmScheduleEntityToViewModel(schedule),
                stage = accountcustomer?.StageStatus ?? (int)StageStatusType.FollowUp

            });
        }
        #endregion



        #region CRMPotentialCustomerController > Save Follow-up
        [HttpPost]
        public ActionResult CreateFollowUpSummaryData(FormCollection followSummaryData)
        {
            if (!(followSummaryData.Count > 0))
            {
                //NLogger.Error("Requested followSummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            var followupViewModel = new CRMFollowUpViewModel();
            followupViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(followSummaryData["CRM_AccountCustomerDetailId"]);
            followupViewModel.Close = Convert.ToInt32(followSummaryData["CloseType"]);
            followupViewModel.Note = followSummaryData["Note"];
            followupViewModel.CreatedBy = LoginUserId;
            if (followupViewModel.Close != 1 && followupViewModel.Close != 2 && followupViewModel.Close != 5)
            {
                followupViewModel.PurposeId = Convert.ToInt32(followSummaryData["PurposeAgainId"]);
                followupViewModel.Purpose = followSummaryData["PurposeAgain"];
            }
            followupViewModel.IsActive = true;
            followupViewModel.RegionId = SelectedRegionId;
            var followup = _crmService.SaveCRM_FollowUp(CRMAutoMapper.CrmFollowupViewModelToEntity(followupViewModel));
            var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(followSummaryData["CRM_AccountId"]));


            var schedulevm = new CRMScheduleViewModel();

            if (followSummaryData["CloseType"] != "1" && followSummaryData["CloseType"] != "2" && followSummaryData["CloseType"] != "5")
            {
                var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(int.Parse(followSummaryData["CRM_AccountId"]));
                schedulevm.ClassId = Convert.ToInt32(followSummaryData["CRM_AccountCustomerDetailId"]);
                schedulevm.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
                schedulevm.Title = account.CRM_AccountId + " - " + "FU" + " - " + followSummaryData["PurposeAgain"] + " - " + account.ContactName + " - " + customerAccountDetail.CompanyPhoneNumber;
                schedulevm.StartDate = followSummaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(followSummaryData["ScheduleStartDate"] + " " + followSummaryData["ScheduleStartTime"]) : DateTime.Now;
                schedulevm.EndDate = followSummaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(followSummaryData["ScheduleEndDate"] + " " + followSummaryData["ScheduleEndTime"]) : DateTime.Now;
                schedulevm.CRM_StageStatusType = (int)StageStatusType.FollowUp;
                schedulevm.CRM_ScheduleTypeId = 1; /* ScheduleType 1 CRM*/
                schedulevm.RegionId = SelectedRegionId;
                schedulevm.AuthUserLoginId = account.AssigneeId;
                schedulevm.CreatedBy = LoginUserId;
                schedulevm.PurposeId = Convert.ToInt32(followSummaryData["PurposeAgainId"]);
                schedulevm.Purpose = followSummaryData["PurposeAgain"];
                schedulevm.IsActive = true;
                //var scheduleVM = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(schedulevm)));
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
                    return Json(new
                    {
                        success = true,
                        result = CRMAutoMapper.CrmFollowupEntityToViewModel(followup),
                        schedule = schedulevm,
                        stage = 5
                    });
                }
                else if (followup.Close == 2)
                {
                    account.StageStatus = (int)StageStatusType.Sold;
                    account.ModifiedBy = LoginUserId;
                    _crmService.SaveCRM_Account(account);
                }
                else if (followup.Close == 1)
                {
                    account.StageStatus = (int)StageStatusType.Bidding;
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
            try
            {
                if (closeSummaryData == null)
                {
                    //NLogger.Error("Requested closeSummaryData is null or empty");
                    return Json(new { success = false, message = "Data is null or empty" });
                }
                //NLogger.Debug($"CRM_AccountId={closeSummaryData.CRM_AccountId}, " +
                //         $"CRM_AccountCustomerDetailId={closeSummaryData.CRM_AccountCustomerDetailId}, " +
                //        $"HaveBackgroundCheck={closeSummaryData.HaveBackgroundCheck}," +
                //        $"SignedAgreement={closeSummaryData.SignedAgreement}, " +
                //        $"DocumentSalesCRM={closeSummaryData.DocumentSalesCRM}, " +
                //        $"NotifyAccountPlacement={closeSummaryData.NotifyAccountPlacement} " +
                //        $"PropAmount={closeSummaryData.PropAmount}" +
                //         $"InitialClean={closeSummaryData.InitialClean}," +
                //        $"ContractAmount={closeSummaryData.ContractAmount}, " +
                //        $"InitialCleanAmount={closeSummaryData.InitialCleanAmount}, " +
                //        $"SignDate={closeSummaryData.SignDate} " +
                //        $"StartDate={closeSummaryData.StartDate}" +
                //         $"PhoneNumber={closeSummaryData.PhoneNumber} " +
                //        $"ContractTerm={closeSummaryData.ContractTerm}" +
                //         $"ServiceType={closeSummaryData.ServiceType}," +
                //        $"BillingFrequency={closeSummaryData.BillingFrequency}, " +
                //        $"CleanTime={closeSummaryData.CleanTime}, " +
                //        $"Cleaningday={closeSummaryData.CleanTime} " +
                //        $"CleanFrequency={closeSummaryData.CleanFrequency}" +
                //        $"Note={closeSummaryData.Note}");


                CRMCloseViewModel closeViewModel = new CRMCloseViewModel();
                closeViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(closeSummaryData.CRM_AccountCustomerDetailId);
                closeViewModel.HaveBackgroundCheck = Convert.ToBoolean(closeSummaryData.HaveBackgroundCheck);
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
                closeViewModel.IsActive = true;
                var checkDayscount = closeSummaryData.CleaningDay;
                if (checkDayscount.Count() >= 1 && checkDayscount[0] != "")
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

                        if (value == 8) { closeViewModel.Weekend = true; }
                    }
                }

                var close = _crmService.SaveCRM_Close(CRMAutoMapper.CrmCloseViewModelToEntity(closeViewModel));

                //Close Lead Convert into Customer 
                var account = _crmService.GetCRM_AccountbyId(Convert.ToInt32(closeSummaryData.CRM_AccountId));

                var accountcustomer = _crmService.GetCRM_AccountbyId(closeSummaryData.CRM_AccountId);

                //update AccountCustomer            
                if (accountcustomer != null)
                {
                    accountcustomer.StageStatus = (int)StageStatusType.Close;
                    accountcustomer.Stage = (int)StageType.Customer;
                    accountcustomer.ModifiedBy = LoginUserId;
                    _crmService.SaveCRM_Account(accountcustomer);
                }

                var customerdetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(account.CRM_AccountId);


                //Customer Details
                FullCustomerViewModel fullCustomerViewModel = new FullCustomerViewModel();
                fullCustomerViewModel.CustomerViewModel.Name = customerdetail.CompanyName;
                fullCustomerViewModel.CustomerViewModel.StatusListId = 38;    //Region Operation = 38
                fullCustomerViewModel.CustomerViewModel.CustomerNo = CustomerService.getCustomerNo(SelectedRegionId);
                fullCustomerViewModel.CustomerViewModel.CreatedDate = DateTime.Now;
                fullCustomerViewModel.CustomerViewModel.CreatedBy = LoginUserId;
                fullCustomerViewModel.CustomerViewModel.IsActive = true;
                fullCustomerViewModel.CustomerViewModel.RegionId = SelectedRegionId;
                fullCustomerViewModel.CustomerViewModel.ParentId = -1;
                fullCustomerViewModel.CustomerViewModel.ContractTypeListId = (int)JKApi.Business.Enumeration.ContractTypeList.Recurring;
                fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(fullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                int _CustomerIdTemp = fullCustomerViewModel.CustomerViewModel.CustomerId;
                RegionSetting regData = CustomerService.GetRegionConfigurationbyId(3, SelectedRegionId);
                int valueadd = Convert.ToInt32(regData.Value) + 1;
                regData.Value = valueadd.ToString();
                CustomerService.SaveRegionConfiguration(regData);

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
                fullCustomerViewModel.MainContact.CreatedBy = LoginUserId.ToString();
                fullCustomerViewModel.MainContact = CustomerService.SaveContact(fullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                fullCustomerViewModel.MainAddress.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainAddress.Address1 = customerdetail.CompanyAddressLine1;
                fullCustomerViewModel.MainAddress.Address2 = customerdetail.CompanyAddressLine2;
                fullCustomerViewModel.MainAddress.City = customerdetail.CompanyCity;
                fullCustomerViewModel.MainAddress.StateName = customerdetail.CompanyState;
                fullCustomerViewModel.MainAddress.PostalCode = customerdetail.CompanyZipCode;

                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullCustomerViewModel.MainAddress.FullAddress));
                if (_latlng.results.Count > 0)
                {
                    fullCustomerViewModel.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    fullCustomerViewModel.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }


                if (customerdetail.CompanyState != null)
                {
                    int state = CustomerService.GetStateId(customerdetail.CompanyState);
                    fullCustomerViewModel.MainAddress.StateListId = state;
                }


                fullCustomerViewModel.MainAddress.IsActive = true;
                fullCustomerViewModel.MainAddress.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainAddress.CreatedBy = LoginUserId;
                fullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainAddress = CustomerService.SaveAddress(fullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                fullCustomerViewModel.MainPhone.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainPhone.Phone1 = account.PhoneNumber;
                fullCustomerViewModel.MainPhone.Cell = account.PhoneNumber;
                fullCustomerViewModel.MainPhone.IsActive = true;
                fullCustomerViewModel.MainPhone.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainPhone.CreatedBy = LoginUserId;
                fullCustomerViewModel.MainPhone = CustomerService.SavePhone(fullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                fullCustomerViewModel.MainPhone2.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainPhone2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainPhone2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                fullCustomerViewModel.MainPhone2.Phone1 = customerdetail.CompanyPhoneNumber;
                fullCustomerViewModel.MainPhone2.Fax = customerdetail.CompanyFaxNumber;
                fullCustomerViewModel.MainPhone2.Cell = customerdetail.CompanyPhoneNumber;
                fullCustomerViewModel.MainPhone2.IsActive = true;
                fullCustomerViewModel.MainPhone2.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainPhone2.CreatedBy = LoginUserId;
                fullCustomerViewModel.MainPhone2 = CustomerService.SavePhone(fullCustomerViewModel.MainPhone2.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                fullCustomerViewModel.MainEmail.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                fullCustomerViewModel.MainEmail.EmailAddress = account.EmailAddress;
                fullCustomerViewModel.MainEmail.IsActive = true;
                fullCustomerViewModel.MainEmail.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainEmail.CreatedBy = LoginUserId.ToString();
                fullCustomerViewModel.MainEmail = CustomerService.SaveEmail(fullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                fullCustomerViewModel.MainEmail2.ClassId = _CustomerIdTemp;
                fullCustomerViewModel.MainEmail2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.MainEmail2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                fullCustomerViewModel.MainEmail2.EmailAddress = customerdetail.CompanyEmailAddress;
                fullCustomerViewModel.MainEmail2.IsActive = true;
                fullCustomerViewModel.MainEmail2.CreatedDate = DateTime.Now;
                fullCustomerViewModel.MainEmail2.CreatedBy = LoginUserId.ToString();
                fullCustomerViewModel.MainEmail2 = CustomerService.SaveEmail(fullCustomerViewModel.MainEmail2.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

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
                fullCustomerViewModel.Contract.PurchaseOrderNo = closeSummaryData.PhoneNumber;
                fullCustomerViewModel.Contract.CreatedBy = LoginUserId;
                fullCustomerViewModel.Contract.AccountTypeListId = customerdetail.AccountTypeListId;
                fullCustomerViewModel.Contract.InitialCleanAmount = closeSummaryData.InitialCleanAmount;
                fullCustomerViewModel.Contract.StatusListId = 38; //Region Operation = 38
                fullCustomerViewModel.Contract.StatusId = fullCustomerViewModel.CustomerViewModel.StatusId;
                fullCustomerViewModel.Contract.ContractDescription = "MONTHLY CONTRACT BILLING";
                fullCustomerViewModel.Contract.SoldById = account.AssigneeId;
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
                fullCustomerViewModel.ContractDetail.CPIIncrease = true;
                fullCustomerViewModel.ContractDetail.LineNumber = 1;
                fullCustomerViewModel.ContractDetail.SeparateInvoice = false;
                fullCustomerViewModel.ContractDetail.SquareFootage = customerdetail.SqFt;
                fullCustomerViewModel.ContractDetail.CreatedDate = DateTime.Now;
                fullCustomerViewModel.ContractDetail.CreatedBy = LoginUserId;
                fullCustomerViewModel.ContractDetail.StartTime = DateTime.Now;
                fullCustomerViewModel.ContractDetail.EndTime = DateTime.Now.AddHours(1);
                fullCustomerViewModel.ContractDetail = CustomerService.SaveContractDetail(fullCustomerViewModel.ContractDetail.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

                //Save Customer-Sales-Document 
                if (closeSummaryData.file1 != null)
                {
                    if (closeSummaryData.file1.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file2 != null)
                {
                    if (closeSummaryData.file2.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file3 != null)
                {
                    if (closeSummaryData.file3.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file4 != null)
                {
                    if (closeSummaryData.file4.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file5 != null)
                {
                    if (closeSummaryData.file5.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file6 != null)
                {
                    if (closeSummaryData.file6.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file7 != null)
                {
                    if (closeSummaryData.file7.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file8 != null)
                {
                    if (closeSummaryData.file8.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file9 != null)
                {
                    if (closeSummaryData.file9.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file10 != null)
                {
                    if (closeSummaryData.file10.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file11 != null)
                {
                    if (closeSummaryData.file11.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }
                if (closeSummaryData.file12 != null)
                {
                    if (closeSummaryData.file12.ContentLength > 0) { SaveSalesDocument(_CustomerIdTemp, closeSummaryData); }
                }

                var docList = CustomerService.GetCRMDocumentByAccountCustomerDetailId(closeSummaryData.CRM_AccountCustomerDetailId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer));

                foreach (var item in docList)
                {
                    if (item.FileTypeListId == 1 && closeSummaryData.file1 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 2 && closeSummaryData.file2 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 3 && closeSummaryData.file3 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 4 && closeSummaryData.file4 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 5 && closeSummaryData.file5 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 6 && closeSummaryData.file6 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 7 && closeSummaryData.file7 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 8 && closeSummaryData.file8 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 9 && closeSummaryData.file9 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 10 && closeSummaryData.file10 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 11 && closeSummaryData.file11 == null) { MoveSalesDocument(_CustomerIdTemp, item); }
                    else if (item.FileTypeListId == 12 && closeSummaryData.file12 == null) { MoveSalesDocument(_CustomerIdTemp, item); }



                }


                //var accountcustomer = _crmService.GetCRM_AccountbyId(closeSummaryData.CRM_AccountId);

                //update AccountCustomer            
                //if (accountcustomer != null)
                //{
                //    NLogger.Debug("accountcustomer id " + accountcustomer.CRM_AccountId.ToString());
                //    accountcustomer.StageStatus = (int)StageStatusType.Close;
                //    accountcustomer.Stage = (int)StageType.Customer;
                //    accountcustomer.ModifiedBy = LoginUserId;
                //    _crmService.SaveCRM_Account(accountcustomer);
                //}
                //else
                //{
                //    NLogger.Debug("No accountcustomer");
                //}

                if (_CustomerIdTemp > 0)
                    _commonService.CommonInsertNotification(7, "", false, _CustomerIdTemp, 1, null, null, null, LoginUserId);

                return Json(new
                {
                    success = true,
                    sold = CRMAutoMapper.CrmCloseEntityToViewModel(close),
                    stage = (int)StageStatusType.Close
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                NLogger.Debug("Exception when creating close data: " + ex.Message);
                NLogger.Debug(ex.StackTrace);

                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public void MoveSalesDocument(int CustomerId, CRMDocumentViewModel tempViewModel)
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, _FilePath, _FileName, _FileExt, _FileSize);
                }
                else
                {
                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, "", "", "", 0);

                }
            }
            catch (Exception ex)
            {
                CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)tempViewModel.FileTypeListId, ex.Message, ex.Message, "", 0);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 1, _FilePath, _FileName, _FileExt, _FileSize);
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


                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 2, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 3, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 4, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 5, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 6, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 7, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 8, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 9, _FilePath, _FileName, _FileExt, _FileSize);
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


                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 10, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 11, _FilePath, _FileName, _FileExt, _FileSize);
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

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, 12, _FilePath, _FileName, _FileExt, _FileSize);
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



            if (!Directory.Exists(_fileDirectory + "Areas/CRM/Documents"))
                Directory.CreateDirectory(_fileDirectory + "Areas/CRM/Documents");



            documentViewModel.Document_FilePath = Path.Combine(_fileDirectory + "Areas/CRM/Documents", Path.GetFileName(documentViewModel.document.FileName));

            if (System.IO.File.Exists(documentViewModel.Document_FilePath)) { /*TODO Message*/ }
            documentViewModel.RegionId = SelectedRegionId;
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

                if (!Directory.Exists(_fileDirectory + "Areas/CRM/Documents"))
                    Directory.CreateDirectory(_fileDirectory + "Areas/CRM/Documents");

                HttpFileCollection filecollection = System.Web.HttpContext.Current.Request.Files;

                for (int i = 0; i < filecollection.Count; i++)
                {

                    HttpPostedFile file = filecollection[i];
                    crmdocumentViewModel.CRM_AccountCustomerDetailId = id;
                    crmdocumentViewModel.File_Title = Path.GetFileNameWithoutExtension(file.FileName);
                    crmdocumentViewModel.File_Name = file.FileName;
                    crmdocumentViewModel.Content_Type = file.ContentType;
                    crmdocumentViewModel.Description = "";

                    crmdocumentViewModel.Document_FilePath = Path.Combine(_fileDirectory + "Areas/CRM/Documents", Path.GetFileName(file.FileName));

                    if (System.IO.File.Exists(crmdocumentViewModel.Document_FilePath)) { continue; /*TODO Message*/ }

                    crmdocumentViewModel.RegionId = SelectedRegionId;
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

            string mimeType = MimeMapping.GetMimeMapping(crmDocument.File_Name);
            if (string.IsNullOrEmpty(mimeType))
                mimeType = "application/octet-stream";

            Response.AddHeader("Content-Disposition", "inline; filename=" + crmDocument.File_Name);
            Response.AddHeader("X-Content-Type-Options", "nosniff");

            return File(Path.Combine(_fileDirectory + "Areas/CRM/Documents/", crmDocument.File_Name), mimeType);

        }
        #endregion

        #region CRMPotentialCustomerController > Display Document
        [HttpGet]
        public ActionResult DisplayDocument()
        {
            var crmDocument = _crmService.GetCRM_Document_LastRecord();
            if (crmDocument.Content_Type.Equals("application/pdf"))
                return File(Path.Combine(_fileDirectory + "Areas/CRM/Documents/", crmDocument.File_Name), "application/pdf");
            else
                return File(Path.Combine(_fileDirectory + "Areas/CRM/Documents/", crmDocument.File_Name), "application / octet - stream", crmDocument.File_Name);


        }
        #endregion


        public void SaveTempDoc(int id, HttpPostedFileBase file, int filetypeId)
        {
            var crmClosetempfile = new CRMCloseTempDocumentViewModel();
            crmClosetempfile.CRM_AccountCustomerDetailId = id;
        }

        public ActionResult CRMUploadDocumentPopup(int id, int sold = 0)
        {
            ViewBag.Id = id;
            ViewBag.Stage = sold;
            var docType = jkEntityModel.FileTypeLists.Where(x => x.TypeListId == 1).ToList();
            ViewBag.DocFileType = new SelectList(docType, "FileTypeListId", "Name");
            return PartialView("_CRMUploadDocumentPopup", CustomerService.GetCRMDocumentByAccountCustomerDetailId(id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer)));
        }

        #region :: CRM Upload Documents :: 

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

        [HttpPost]
        public ActionResult CRMSaveDocuments(FormCollection collection)
        {
            string selIds = Request["selIds"];
            string CRMAccCustDetailId = Request["CRMAccCustDetailId"];

            UploadCustomerFile(Convert.ToInt32(CRMAccCustDetailId));

            if (selIds != "")
            {
                string[] arrIds = selIds.Split(',');
                for (int i = 0; i < arrIds.Length; i++)
                {
                    if (Request.Files.Count > 0)
                    {
                        var crmdocumentViewModel = new CRMDocumentViewModel();


                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = files[i];
                        string DocumentId = arrIds[i];

                        string _FileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                        string _FileExt = Path.GetFileName(file.FileName).Split('.').Last();
                        string fname = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                        //string fname = CRMAccCustDetailId + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + file.FileName.Trim();

                        int CRM_DocumentId = 0;
                        if (!DocumentId.Contains("Other_"))
                        {
                            CRM_DocumentId = Convert.ToInt32(DocumentId);
                        }
                        else
                        {
                            DocumentId = DocumentId.Replace("Other_", "");
                        }

                        var CRMModel = _crmService.GetCRMDocumentWithAccountCustomer_FileType(Convert.ToInt32(CRMAccCustDetailId), Convert.ToInt32(DocumentId));

                        if (CRMModel != null && CRM_DocumentId > 0)
                        {
                            CRMModel.RegionId = SelectedRegionId;
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

                            if (Request["txtDocName_" + Convert.ToInt32(DocumentId)] != null)
                                crmdocumentViewModel.File_Title = Request["txtDocName_" + Convert.ToInt32(DocumentId)];

                            crmdocumentViewModel.File_Name = fname;
                            crmdocumentViewModel.Content_Type = file.ContentType;
                            crmdocumentViewModel.Description = "";
                            crmdocumentViewModel.FileTypeListId = Convert.ToInt32(DocumentId);
                            //crmdocumentViewModel.Document_FilePath = Path.Combine(_fileDirectory+ "Areas/CRM/Documents", fname);
                            crmdocumentViewModel.Document_FilePath = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname);

                            //if (System.IO.File.Exists(crmdocumentViewModel.Document_FilePath)) { continue; /*TODO Message*/ }

                            file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CRMAccCustDetailId.ToString(), fname));
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
            return Json(new { success = true, document = CRMAutoMapper.CrmDocumentEntitiesToViewModels(crmdocuments) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NewDocType(string NewDocName, int TypeListId)
        {
            var newDoc = jkEntityModel.FileTypeLists.Where(x => x.Name == NewDocName && x.TypeListId == TypeListId).FirstOrDefault();
            if (newDoc == null)
            {
                newDoc = new FileTypeList();
                newDoc.Name = NewDocName;
                newDoc.TypeListId = TypeListId;
                newDoc.IsActive = true;

                jkEntityModel.FileTypeLists.Add(newDoc);
                jkEntityModel.SaveChanges();

                return Json(new { success = "", docType = jkEntityModel.FileTypeLists.Where(x => x.Name == NewDocName && x.TypeListId == TypeListId).FirstOrDefault().ToJSON() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = "This doc name already exist", docType = newDoc.ToJSON() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsDocViewToFranchisee(int id)
        {
            var newDoc = jkEntityModel.CRM_Document.Where(x => x.CRM_DocumentId == id).FirstOrDefault();
            if (newDoc != null)
            {
                newDoc.IsViewToFranchisee = newDoc.IsViewToFranchisee == true ? false : true;
                jkEntityModel.CRM_Document.Add(newDoc);
                jkEntityModel.SaveChanges();
                return Json(new { success = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = "This is not valid doc.", docType = newDoc.ToJSON() }, JsonRequestBehavior.AllowGet);
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

        #region CloseLeads
        public ActionResult CloseLeads()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CustomerSales", new { area = "CRM" }), "Close Leads");

            //Passing 3 as a choice parameter
            //var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(3, 0, SelectedUserId, SelectedRegionId);

            // Select list   
            ViewBag.UserId = SelectedUserId;
            //ViewBag.UserList = new SelectList(_crmService.Get_AuthUserLogin(), "UserId", "FirstName");
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");
            if (roleId != null)
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(roleId.RoleId);
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }
            else
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(7); //CRM-Sales
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }

            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name");
            ViewBag.CallTime = new SelectList(CRMUtils.GetBestTimeList());
            ViewBag.ServiceTypeListModel = ServiceTypeListModel;

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

        public JsonResult CloseLeadListData()
        {
            var potentialcustomerViewModel = _crmService.GetAll_CRM_PotentialCustomer(4, 0, SelectedUserId, SelectedRegionId);


            var jsonResult = Json(new
            {
                aadata = potentialcustomerViewModel
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region :: All stag popup action :: 

        public ActionResult InitialCommunicationPopup(int CustomerDetailbyId)
        {
            var accountCustomerInitialViewModel = new CRMInitialCommunicationViewModel();
            var accountInitialCommunication = _crmService.Get_InitialCommunication_ByAccountCustomerDetailById(CustomerDetailbyId);
            if (accountInitialCommunication != null)
                accountCustomerInitialViewModel = CRMAutoMapper.CrmInitialEntityToViewModel(accountInitialCommunication);
            else
            {
                accountCustomerInitialViewModel = null;
            }
            return PartialView("_Partial_PP_InitialCommunication", accountCustomerInitialViewModel);
        }
        public ActionResult FVPresentationPopup(int CustomerDetailbyId)
        {
            var accountCustomerFvPresentationViewModel = new CRMFvPresentationViewModel();
            var accountFvPresentation = _crmService.Get_fvPresentation_ByAccountCustomerDetailById(CustomerDetailbyId);

            //Fv Presentation
            if (accountFvPresentation != null)
                accountCustomerFvPresentationViewModel = CRMAutoMapper.CrmFvPresentationEntityToViewModel(accountFvPresentation);
            else
            {
                accountCustomerFvPresentationViewModel = null;
            }
            ViewBag.ServiceTypeListModel = ServiceTypeListModel;
            return PartialView("_Partial_PP_FVPresentation", accountCustomerFvPresentationViewModel);
        }
        public ActionResult BiddingPopup(int CustomerDetailbyId)
        {
            var accountCustomerBiddingViewModel = new CRMBiddingViewModel();
            var bidContact = new CRMContactViewModel();
            var bidSch = new CRMScheduleViewModel();

            var accountBidding = _crmService.Get_Bidding_ByAccountCustomerDetailById(CustomerDetailbyId);
            if (accountBidding != null)
            {
                accountCustomerBiddingViewModel = CRMAutoMapper.CrmBiddingEntityToViewModel(accountBidding);
                bidContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_BiddingId == accountBidding.CRM_BiddingId));
                bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
            }
            else
            {
                accountCustomerBiddingViewModel = null;
            }

            // CRM Customer Document
            var accountCustomerDocumentViewModels = new List<CRMDocumentViewModel>();
            var accountDocument = _crmService.GetCRM_Document_ByAccountCustomerDetailId(CustomerDetailbyId).ToList();
            if (accountDocument != null)
            {
                accountCustomerDocumentViewModels = CRMAutoMapper.CrmDocumentEntitiesToViewModels(accountDocument);
            }

            ViewBag.AccountDocument = accountCustomerDocumentViewModels;
            ViewBag.bidContact = bidContact;
            ViewBag.bidSch = bidSch;
            return PartialView("_Partial_PP_Bidding", accountCustomerBiddingViewModel);
        }
        public ActionResult PDAppointmentPopup(int CustomerDetailbyId)
        {
            var bidSch = new CRMScheduleViewModel();
            var bidContact = new CRMContactViewModel();

            var accountBidding = _crmService.Get_Bidding_ByAccountCustomerDetailById(CustomerDetailbyId);
            if (accountBidding != null)
            {
                bidContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_BiddingId == accountBidding.CRM_BiddingId));
                bidSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.Bidding));
            }

            var accountCustomerPdAppointmentViewModel = new CRMPdAppointmentViewModel();
            var pdSch = new CRMScheduleViewModel();
            var pdContact = new CRMContactViewModel();

            var accountPdAppointment = _crmService.Get_PdAppointment_ByAccountCustomerDetailById(CustomerDetailbyId);
            //PdAppointment
            if (accountPdAppointment != null)
            {
                accountCustomerPdAppointmentViewModel = CRMAutoMapper.CrmPdAppointmentEntityToViewModel(accountPdAppointment);
                pdContact = CRMAutoMapper.CrmContactEntityToViewModel(_crmService.GetAll_CRM_Contact().FirstOrDefault(x => x.CRM_PdAppointmentId == accountPdAppointment.CRM_PdAppointmentId));
                pdSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
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
            ViewBag.pdContact = pdContact;
            ViewBag.pdSch = pdSch;

            ViewBag.bidContact = bidContact;
            ViewBag.bidSch = bidSch;

            return PartialView("_Partial_PP_PDAppointment", accountCustomerPdAppointmentViewModel);
        }
        public ActionResult PDFollowUpPopup(int CustomerDetailbyId)
        {
            var accountCustomerFollowUpViewModel = new CRMFollowUpViewModel();
            var followSch = new CRMScheduleViewModel();
            var followPD = new CRMScheduleViewModel();
            var accountFollowUp = _crmService.Get_FollowUp_ByAccountCustomerDetailById(CustomerDetailbyId);
            //Follow-Up
            if (accountFollowUp != null)
            {
                accountCustomerFollowUpViewModel = CRMAutoMapper.CrmFollowupEntityToViewModel(accountFollowUp);
                followSch = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.FollowUp));
                if (followSch != null)
                {
                    followSch.PurposeId = followSch.PurposeId;// CRMUtils.SchedulePurposIndex(pdSch.Title);
                    followSch.Purpose = followSch.Purpose;
                }

                followPD = CRMAutoMapper.CrmScheduleEntityToViewModel(_crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == CustomerDetailbyId && x.CRM_StageStatusType == (int)StageStatusType.PdAppointment));
                if (followPD != null)
                {
                    followPD.PurposeId = followPD.PurposeId;// CRMUtils.SchedulePurposIndex(pdSch.Title);
                    followPD.Purpose = followPD.Purpose;
                }

            }
            else
            {
                accountCustomerFollowUpViewModel = null;
            }
            ViewBag.followSch = followSch;
            ViewBag.followPD = followPD;

            return PartialView("_Partial_PP_FollowUp", accountCustomerFollowUpViewModel);
        }

        public ActionResult SoldPopup(int CustomerDetailbyId)
        {
            var accountCustomerSoldViewModel = new CRMCloseViewModel();

            var accountClose = _crmService.Get_Close_ByAccountCustomerDetailById(CustomerDetailbyId);
            if (accountClose != null)
            {
                accountCustomerSoldViewModel = CRMAutoMapper.CrmCloseEntityToViewModel(accountClose);
            }
            else
            {
                accountCustomerSoldViewModel = null;
            }

            ViewBag.ServiceTypeListModel = ServiceTypeListModel;

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
            return PartialView("_Partial_PP_Sold", accountCustomerSoldViewModel);
        }

        #endregion
    }
}