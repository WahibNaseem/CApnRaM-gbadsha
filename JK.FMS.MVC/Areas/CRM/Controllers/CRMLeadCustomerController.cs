using Application.Web.Core;
using JK.FMS.MVC.Areas.CRM.Common;
using JK.Resources;
using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels;
using JKViewModels.CRM;
using JKViewModels.Customer;
using MvcBreadCrumbs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKViewModels.Common;
using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{

    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]

    /// <summary>
    /// Controller used to handle all the lead views in CRM module.
    /// </summary>
    public class CRMLeadCustomerController : ViewControllerBase
    {
        #region Life Cycle

        private readonly ICRM_Service _crmService;
        private bool _hasNewData;


        public CRMLeadCustomerController(ICacheProvider cacheProvider, ICRM_Service ICRMService, ICommonService commonService, ICustomerService customerService, ClaimView claimview, IUserService userService)
        {
            _cacheProvider = cacheProvider;
            _userService = userService;
            _crmService = ICRMService;
            _commonService = commonService;
            CustomerService = customerService;
            _claimView = claimview;
            _hasNewData = false;
            ViewBag.HMenu = "CustomerSales";
        }

        #endregion

        #region CRMLeadCustomerController > Helpers

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
                    .Where(x => x.Name == Constants.Key_UnqualifiedLead ||
                    x.Name == Constants.Key_NewLead ||
                    x.Name == Constants.Key_Potential ||
                    x.Name == Constants.Key_QualifiedLead ||
                    x.Name == Constants.Key_CallBack ||
                    x.Name == Constants.Key_NewLead);
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

        private SelectList CallResultList
        {
            get
            {
                var calllogcallresult = _crmService.GetAll_CRM_CallResult();
                var items = new List<SelectListItem>();
                foreach (var callresult in calllogcallresult)
                {
                    var item = new SelectListItem
                    {
                        Value = callresult.Type.ToString(),
                        Text = JKCRMResource.ResourceManager.GetString(callresult.Name) != null
                            ? JKCRMResource.ResourceManager.GetString(callresult.Name)
                            : "N/A"
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        private SelectList PotentialLeadStageStatusList
        {
            get
            {
                var accountstagestatus = _crmService.GetAll_CRM_StageStatus()
                    .Where(x => x.Name == Constants.Key_IntialCommunication ||
                    x.Name == Constants.Key_NeedAssessment ||
                    x.Name == Constants.Key_Prestentation ||
                    x.Name == Constants.Key_Negotiation ||
                x.Name == Constants.Key_CommettoBuy ||
                x.Name == Constants.Key_Potential);
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

        #region CRMLeadCustomerController > Leads

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");

            return View();
        }

        [HttpGet]
        public ActionResult QualifiedLeads()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            return View();
        }

        [HttpGet]
        public ActionResult UnQualifiedLeads()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            return View();
        }

        [HttpGet]
        public ActionResult CallBackLeads()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustome", new { area = "CRM" }), "Leads");

            return View();
        }

        [HttpGet]
        public ActionResult CloseLeads()
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");


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
                var SalesPersonList = _crmService.Get_AuthUserLogin(7); //CRM Sales
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }
            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.State = new SelectList(CRMUtils.GetStateDescription<CRMEnums.State>());
            ViewBag.CallTime = new SelectList(CRMUtils.GetBestTimeList());
            ViewBag.ServiceTypeList = new SelectList(CustomerService.GetServiceTypeList().OrderBy(x => x.name), "ServiceTypeListId", "name");

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

        [HttpGet]
        public ActionResult GetNewLeadPopUp()
        {
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
            return PartialView("_NewLeadVerify");
        }

        #endregion

        #region CRMLeadCustomerController > LeadDetail

        [HttpGet]
        public ActionResult Detail(int accountId)
        {
            ViewBag.HMenu = "CustomerSales";
            ViewBag.CurrentMenu = JKCRMResource.lead;
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashBoard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            var accountCustomerDetailViewModel = new CRMAccountCustomerDetailViewModel();

            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(accountId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            if (account != null)
            {
                accountCustomerDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomerDetail);
                accountCustomerDetailViewModel.FirstName = account.FirstName;
                accountCustomerDetailViewModel.LastName = account.LastName;
                accountCustomerDetailViewModel.ContactName = account.ContactName;
                accountCustomerDetailViewModel.MiddleInitial = account.MiddleInitial;
                accountCustomerDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountCustomerDetailViewModel.EmailAddress = account.EmailAddress;
                accountCustomerDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountCustomerDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountCustomerDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountCustomerDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;
                // accountCustomerDetailViewModel.CallResultType = (CallResultType)accountCustomerDetailViewModel.CRM_CallResultId?;

                var iniate = CRMAutoMapper.CrmInitialEntityToViewModel(_crmService.Get_InitialCommunication_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId));
                if (iniate != null)
                {
                    accountCustomerDetailViewModel.ContactPerson = iniate.ContactPerson;

                    if (iniate.StartDate != null)
                    {
                        accountCustomerDetailViewModel.StartDate = iniate.StartDate != null ? iniate.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        accountCustomerDetailViewModel.StartTime = iniate.StartDate.Value.ToString("h:mm:ss tt");

                        accountCustomerDetailViewModel.EndDate = iniate.EndDate != null ? iniate.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        accountCustomerDetailViewModel.EndTime = iniate.EndDate != null ? iniate.EndDate.Value.ToString("h:mm:ss tt") : "";
                    }

                    accountCustomerDetailViewModel.InterestedInPerposal = iniate.InterestedInPerposal;
                    accountCustomerDetailViewModel.Purpose = Convert.ToInt32(iniate.PURPOSE);
                }

                var leadName = $"{accountCustomerDetailViewModel.FirstName} {accountCustomerDetailViewModel.LastName}";
                BreadCrumb.Add(Url.Action("Detail", "CRMLeadCustomer", new { area = "CRM" }), leadName);
            }

            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");
            if (roleId != null)
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(roleId.RoleId, accountCustomerDetail.TerritoryId);
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }
            else
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(7, accountCustomerDetail.TerritoryId); //CRM Sales
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }

            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name", accountCustomerDetailViewModel.AccountTypeListId);
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.CallTypeResult = accountCustomerDetailViewModel.CRM_CallResultId;
            ViewBag.CallResultList = new SelectList(_crmService.GetAll_CRM_CallResult().ToList().OrderBy(cr => cr.Name), "CRM_CallResultId", "Name", accountCustomerDetailViewModel.CRM_CallResultId);
            ViewBag.NoteType = new SelectList(_crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name), "CRM_NoteTypeId", "Name", accountCustomerDetailViewModel.CRM_NoteTypeId);
            ViewBag.SalePossibilityType = new SelectList(_crmService.GetAll_CRM_SalePossibilityType().ToList().OrderBy(x => x.Name), "CRM_SalePossibilityTypeId", "Name");
            ViewBag.SalePossibility = accountCustomerDetailViewModel.CRM_SalePossibilityTypeId;
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", accountCustomerDetailViewModel.CompanyState);
            ViewBag.PurposeType = new SelectList(_crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name", accountCustomerDetailViewModel.Purpose);
            ViewBag.ScheduleType = CustomerService.GetScheduleTyoeList();
            ViewBag.StageStatusType = _crmService.GetAll_CRM_StageStatus();
            ViewBag.PurposeTypeList = _crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name);


            return View(accountCustomerDetailViewModel);

        }

        #endregion

        #region CRMLeadCustomerController > HttpRequest


        [HttpGet]
        public ActionResult GetIndex()
        {
            //Passing 4 as a first parameter for New Lead
            var customerCloseLeadViewModels = _crmService.GetAll_CRM_PotentialCustomer(4, 0, SelectedUserId, SelectedRegionId);
            var jsondata = Json(new
            {
                aadata = customerCloseLeadViewModels
            }, JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = 2147483644;
            return jsondata;
        }

        #region CRMLeadCustomer > Get Previous Or Next Lead
        /// <summary>
        /// Get Previous of Next record through id  and condition name 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNextOrPrevious(int Id, int name)
        {
            var customerLeadId = 0;

            if (Session["SearchList"] == null)
            {
                //return Json(new
                //{
                //    success = false,
                //    Id = customerLeadId
                //}, behavior:JsonRequestBehavior.AllowGet);

                var customerLeadViewModels = _crmService.GetAll_CRM_PotentialCustomer(0, 0, SelectedUserId, SelectedRegionId).OrderByDescending(o => o.CRM_AccountId).OrderByDescending(o => o.CRM_AccountId);
                if (customerLeadViewModels.Count() >= 0)
                {
                    Session["SearchList"] = customerLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
                }
            }

            var list = Session["SearchList"] as List<int>;

            if (Convert.ToBoolean(name))
            {
                customerLeadId = list.TakeWhile(x => x != Id).LastOrDefault();
            }
            else
            {
                customerLeadId = list.SkipWhile(x => x != Id).Skip(1).FirstOrDefault();

                if (customerLeadId == 0)
                    customerLeadId = list.TakeWhile(x => x != Id).LastOrDefault();
            }

            return Json(new
            {
                success = true,
                Id = customerLeadId
            }, behavior: JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadCustomerController > Get New Lead List
        [HttpGet]
        public ActionResult NewLeadListData()
        {
            /**Passing first Param 0 for New lead list*/
            var customerLeadViewModels = _crmService.GetAll_CRM_PotentialCustomer(0, 0, SelectedUserId, SelectedRegionId).OrderByDescending(o => o.CRM_AccountId);
            if (customerLeadViewModels.Count() >= 0)
            {
                Session["SearchList"] = null;
                //Session["SearchList"] = customerLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }

            var jsonResult = Json(new
            {
                success = true,
                aadata = customerLeadViewModels

            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CRMLeadCustomerController > GetLeadDetail

        /// <summary>
        /// Get Lead Detail for Previous and next Record   
        /// /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetLeadDetail(int accountId)
        {
            var accountCustomerDetailViewModel = new CRMAccountCustomerDetailViewModel();

            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetail_ByAccountId(accountId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            if (account != null)
            {
                accountCustomerDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomerDetail);
                accountCustomerDetailViewModel.FirstName = account.FirstName;
                accountCustomerDetailViewModel.LastName = account.LastName;
                accountCustomerDetailViewModel.ContactName = account.ContactName;
                accountCustomerDetailViewModel.MiddleInitial = account.MiddleInitial;
                accountCustomerDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountCustomerDetailViewModel.EmailAddress = account.EmailAddress;
                accountCustomerDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountCustomerDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountCustomerDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountCustomerDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;

                var iniate = CRMAutoMapper.CrmInitialEntityToViewModel(_crmService.Get_InitialCommunication_ByAccountCustomerDetailById(accountCustomerDetail.CRM_AccountCustomerDetailId));
                if (iniate != null)
                {
                    accountCustomerDetailViewModel.ContactPerson = iniate.ContactPerson;

                    if (iniate.StartDate != null)
                    {
                        accountCustomerDetailViewModel.StartDate = iniate.StartDate != null ? iniate.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        accountCustomerDetailViewModel.StartTime = iniate.StartDate.Value.ToString("h:mm:ss tt");

                        accountCustomerDetailViewModel.EndDate = iniate.EndDate != null ? iniate.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        accountCustomerDetailViewModel.EndTime = iniate.EndDate != null ? iniate.EndDate.Value.ToString("h:mm:ss tt") : "";
                    }
                    accountCustomerDetailViewModel.InterestedInPerposal = iniate.InterestedInPerposal;
                    accountCustomerDetailViewModel.Purpose = Convert.ToInt32(iniate.PURPOSE);
                }


            }
            var roleId = _claimView.GetCLAIM_ROLELIST().ToList().FirstOrDefault(x => x.RoleName == "CRM-Sales");
            if (roleId != null)
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(roleId.RoleId);
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }
            else
            {
                var SalesPersonList = _crmService.Get_AuthUserLogin(7); //Customer Sales
                ViewBag.UserList = new SelectList(SalesPersonList, "UserId", "FirstName");
            }

            ViewBag.LeadStageStatusList = QualifiedLeadStageStatusList;
            ViewBag.AccountTypeList = new SelectList(CustomerService.GetAccountTypeList().OrderBy(x => x.Name), "AccountTypeListId", "Name", accountCustomerDetailViewModel.AccountTypeListId);
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.CallResultList = new SelectList(_crmService.GetAll_CRM_CallResult().ToList().OrderBy(cr => cr.Name), "CRM_CallResultId", "Name", accountCustomerDetailViewModel.CRM_CallResultId);
            ViewBag.NoteType = new SelectList(_crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name), "CRM_NoteTypeId", "Name", accountCustomerDetailViewModel.CRM_NoteTypeId);
            ViewBag.SalePossibilityType = new SelectList(_crmService.GetAll_CRM_SalePossibilityType().ToList().OrderBy(x => x.Name), "CRM_SalePossibilityTypeId", "Name", accountCustomerDetailViewModel.CRM_SalePossibilityTypeId);
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", accountCustomerDetailViewModel.CompanyState);
            ViewBag.PurposeType = new SelectList(_crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMInitialCommunication == true).OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name", accountCustomerDetailViewModel.Purpose);

            var customerAccountReferenceInfo = CRMUtils.RenderPartialViewToString(this.ControllerContext, "_CRMCustomerAccountReferenceInformation", accountCustomerDetailViewModel);
            var customerAccountSummary = CRMUtils.RenderPartialViewToString(this.ControllerContext, "_CRMCustomerAccountSummary", accountCustomerDetailViewModel);
            return Json(new
            {
                success = true,
                customerAccountReferenceInfo,
                customerAccountSummary


            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadCustomerController > Get Call Back Lead List
        [HttpGet]
        public ActionResult CallBackLeadListData()
        {
            /*Passing first Param 5 for Call Back Lead List*/
            var customerLeadvm = _crmService.GetAll_CRM_PotentialCustomer(5, 0, SelectedUserId, SelectedRegionId).OrderByDescending(cb => cb.CRM_AccountId);
            if (customerLeadvm.Count() >= 0)
            {
                Session["SearchList"] = null;
                Session["SearchList"] = customerLeadvm.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }


            var jsonResult = Json(new
            {
                success = true,
                aadata = customerLeadvm,
                //sEcho = Request.Params["sEcho"],
                //iTotalDisplayRecords = customerLeadvm.Count(),
                //iTotalRecords = customerLeadvm.Count()               

            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region CRMLeadCustomerController > Get Qualified Lead List
        [HttpGet]
        public ActionResult QualifiedLeadListData()
        {
            /*Passing first Param 1 for Qualified Lead List*/
            var customerLeadViewModels = _crmService.GetAll_CRM_PotentialCustomer(1, 0, SelectedUserId, SelectedRegionId).OrderByDescending(cb => cb.CRM_AccountId);

            if (customerLeadViewModels.Count() >= 0)
            {
                Session["SearchList"] = null;
                Session["SearchList"] = customerLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }


            var jsonResult = Json(new
            {
                success = true,
                aadata = customerLeadViewModels

            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region CRMLeadCustomerController > Get UnQualified Lead List
        [HttpGet]
        public ActionResult UnQualifiedLeadListData()
        {
            /*Passing first Param 2 for UnQualified Lead List*/
            var customerLeadViewModels = _crmService.GetAll_CRM_PotentialCustomer(2, 0, SelectedUserId, SelectedRegionId).OrderByDescending(cb => cb.CRM_AccountId);
            if (customerLeadViewModels.Count() >= 0)
            {
                Session["SearchList"] = null;
                Session["SearchList"] = customerLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }

            var jsonResult = Json(new
            {
                success = true,
                aadata = customerLeadViewModels

            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region CRMLeadCustomerController > Dial Call To Lead
        [HttpGet]
        public ActionResult DialCallToLead(bool isCall, string pnum)
        {
            CRMSipUtils _crmSipUtils = CRMSipUtils.GetCRMInstance();
            // CRMSipUtils _crmSipUtils = new CRMSipUtils(); 
            var message = _crmSipUtils.Call(isCall, pnum);
            return Json(new
            {
                successs = true,
                result = message
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadCustomerController > Get Qualified Lead Detail

        [HttpGet]
        public JsonResult GetQualifiedLeadDetail(int accountId)
        {
            var accountCustomerDetailViewModel = new CRMAccountCustomerDetailViewModel();
            var accountCustomerActivityViewModels = new List<CRMActivityViewModel>();
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            //Get from Services  Account ,  AccountCustomerDetail , AccountActivity
            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountCustomerDetail = _crmService.GetCRM_AccountCustomerDetailbyId(accountId);
            var accountActivity = _crmService.GetCRM_Activity_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            var accountSchedule = _crmService.GetCRM_Schedule_ByAccountCustomerDetailId(accountCustomerDetail.CRM_AccountCustomerDetailId).ToList();
            if (account != null)
            {
                accountCustomerDetailViewModel = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomerDetail);
                accountCustomerDetailViewModel.FirstName = account.FirstName;
                accountCustomerDetailViewModel.LastName = account.LastName;
                accountCustomerDetailViewModel.MiddleInitial = account.MiddleInitial;
                accountCustomerDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountCustomerDetailViewModel.EmailAddress = account.EmailAddress;
                accountCustomerDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountCustomerDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountCustomerDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountCustomerDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;


                // Activity
                accountCustomerActivityViewModels = CRMAutoMapper.CrmActivityEntitiesToViewModels(accountActivity);
                foreach (var activityViewModel in accountCustomerActivityViewModels)
                {
                    activityViewModel.ActivityTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityTypeName(activityViewModel.ActivityType)));
                    if (activityViewModel.OutComeType != null)
                        activityViewModel.OutComeTypeName = (JKCRMResource.ResourceManager.GetString(_crmService.GetCRM_ActivityOutComeTypeName((int)activityViewModel.OutComeType)));
                }

                //Schedule
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
            }

            return Json(new
            {
                success = true,
                result = accountCustomerDetailViewModel,
                activity = accountCustomerActivityViewModels,
                schedule = accountCustomerScheduleViewModels
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region CRMLeadCustomerController > Create New Lead

        [HttpPost]
        public ActionResult Create(FormCollection accountData)
        {
            if (SelectedRegionId == 0)
            {
                NLogger.Error("No Region has selected");
                return Json(new { success = false, message = "Region did not select" });
            }

            if (accountData == null)
            {
                NLogger.Error("Requested AccountData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            NLogger.Debug($"companyname={accountData["Companyname"]}, " +
                          $"contactname={accountData["ContactName"]}, " +
                          $"mobile={accountData["PhoneNumber"]}, " +
                          $"email={accountData["EmailAddress"]}, " +
                          $"note={accountData["Note"]} ");

            var accountCustomer = _crmService.AddNewCustomerAccount(
                accountData["CompanyName"],
                accountData["ContactName"],
                accountData["PhoneNumber"],
                accountData["EmailAddress"],
                Convert.ToInt32(accountData["SourceProvider"]),
               _crmService.GetCRM_ProviderSourceIndex("inhouse"),
               Constants.Key_Customer,
               LoginUserId,
               SelectedRegionId,
               accountData["Note"]
            );

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmAccountEntityToViewModel(accountCustomer)
            });
        }

        #endregion

        #region CRMLeadCustomerController > Save Customer Account

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
            //              $"IndustryType={summaryData["IndustryType"]}, " +
            //              $"NumberOfLocations={summaryData["NumberOfLocations"]}," +
            //              $"SqFt={summaryData["SqFt"]}, " +
            //              $"ContractExpire={summaryData["ContractExpire"]}," +
            //              $"SalesPossibility={summaryData["SalesPossibility"]}," +
            //              $"CRM_ProviderType={summaryData["ProviderType"]}, " +
            //              $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
            //              $"BudgetAmount={summaryData["BudgetAmount"]}, " +
            //              $"CallResult={summaryData["CallResult"]}, " +
            //              $"SpokeWith={summaryData["SpokeWith"]}, " +
            //              $"CallLogDate={summaryData["CallLogDate"]}, " +
            //              $"CallLogTime={summaryData["CallLogTime"]}, " +
            //              $"NoteType={summaryData["NoteType"]}, " +
            //              $"CallLogNote={summaryData["CallLogNote"]}, " +
            //               $"ContactPerson={summaryData["ContactPerson"]}, " +
            //              $"InterestedInProposal={summaryData["InterestedInProposal"]}, " +
            //               $"ScheduleStartDate={summaryData["ScheduleStartDate"]}, " +
            //               $"ScheduleStartTime={summaryData["ScheduleStartTime"]}, " +
            //               $"ScheduleEndDate={summaryData["ScheduleEndDate"]}, " +
            //               $"ScheduleEndTime={summaryData["ScheduleEndTime"]}, " +
            //                $"PurposeId={summaryData["PurposeId"]}, " +
            //                 $"Purpose={summaryData["Purpose"]}, " +
            //                $"Asignee={ summaryData["Asignee"]}," +
            //              $"Note={summaryData["Note"]}");

            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(int.Parse(summaryData["CRM_AccountCustomerDetailId"]));
            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(summaryData["CRM_AccountId"]));
            if (customerAccountDetail != null && accountcustomer != null)
            {
                customerAccountDetail.AccountTypeListId = Convert.ToInt32(summaryData["IndustryType"]);
                customerAccountDetail.NumberOfLocations = summaryData["NumberOfLocations"] != "" ? Convert.ToInt32(summaryData["NumberOfLocations"]) : 0;
                customerAccountDetail.SqFt = summaryData["SqFt"];
                if (summaryData["ContractExpire"] != "")
                    customerAccountDetail.ContractExpire = Convert.ToDateTime(summaryData["ContractExpire"]);
                customerAccountDetail.CRM_SalePossibilityTypeId = summaryData["SalesPossibility"] != "" ? Convert.ToInt32(summaryData["SalesPossibility"]) : 0;
                customerAccountDetail.CRM_NoteTypeId = summaryData["NoteType"] != "" ? Convert.ToInt32(summaryData["NoteType"]) : 14; // 14 Other
                customerAccountDetail.GeneralNote = summaryData["CallLogNote"];
                customerAccountDetail.BudgetAmount = summaryData["BudgetAmount"] != "" ? Convert.ToDecimal(summaryData["BudgetAmount"]) : 0;
                customerAccountDetail.CRM_CallResultId = summaryData["CallResult"] != "" ? Convert.ToInt32(summaryData["CallResult"]) : 1; // 1 Other
                customerAccountDetail.SpokeWith = summaryData["SpokeWith"];

                if (summaryData["CallLogDate"] != "")
                {
                    customerAccountDetail.Callback = Convert.ToDateTime(summaryData["CallLogDate"] + " " + summaryData["CallLogTime"]);
                }
                customerAccountDetail.ModifiedBy = LoginUserId;

                #region Initial Communication Code

                /*Check If TeleMarketer Select Potential To Make Initial Communication*/
                if (int.Parse(summaryData["StageStatus"]) == (int)StageStatusType.Potential)
                {
                    /*Get the Initl Communication if there is any*/
                    var customerInitialCommunicate = _crmService.Get_InitialCommunication_ByAccountCustomerDetailById(customerAccountDetail.CRM_AccountCustomerDetailId);

                    /*Check if InitialCommunication Not Exist  */
                    string scheduleStartEndDateTime = "";
                    if (customerInitialCommunicate != null)
                    {
                        customerInitialCommunicate.ContactPerson = summaryData["ContactPerson"];
                        if (summaryData["InterestedInProposal"] == "1")
                            customerInitialCommunicate.InterestedInPerposal = true;
                        else if (summaryData["InterestedInProposal"] == "0")
                            customerInitialCommunicate.InterestedInPerposal = false;

                        customerInitialCommunicate.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.Now;


                        customerInitialCommunicate.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;
                        customerInitialCommunicate.CRM_AccountCustomerDetailId = Convert.ToInt32(summaryData["CRM_AccountCustomerDetailId"]);
                        customerInitialCommunicate.Note = summaryData["Note"];
                        customerInitialCommunicate.PURPOSE = Convert.ToInt32(summaryData["PurposeId"]);
                        customerInitialCommunicate.RegionId = SelectedRegionId;
                        customerInitialCommunicate.ModifiedBy = LoginUserId;

                        var initialcommunication = _crmService.SaveCRM_InitialCommunication(customerInitialCommunicate);


                        /* Save In Schedule */
                        var scheduleentity = _crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.ClassId == initialcommunication.CRM_AccountCustomerDetailId && x.CRM_StageStatusType == (int)StageStatusType.IntialCommunication);
                        if (scheduleentity != null)
                        {
                            scheduleentity.ClassId = initialcommunication.CRM_AccountCustomerDetailId;
                            scheduleentity.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
                            scheduleentity.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.UtcNow;
                            scheduleentity.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;

                            scheduleStartEndDateTime = scheduleentity.StartDate + " - " + scheduleentity.EndDate;

                            scheduleentity.Title = accountcustomer.CRM_AccountId + " - " + "FV" + " - " + summaryData["Purpose"] + " - " + accountcustomer.ContactName + " - " + initialcommunication.ContactPerson + " - " + customerAccountDetail.CompanyPhoneNumber;

                            scheduleentity.Location = customerAccountDetail.CompanyAddressLine1;
                            scheduleentity.AuthUserLoginId = summaryData["Asignee"] != "" ? Convert.ToInt32(summaryData["Asignee"]) : 0;
                            scheduleentity.Description = customerAccountDetail.CompanyName + "," + accountcustomer.ContactName + "," + accountcustomer.PhoneNumber;
                            scheduleentity.RegionId = SelectedRegionId;

                            scheduleentity.ModifiedBy = LoginUserId;
                            scheduleentity.PurposeId = Convert.ToInt32(summaryData["PurposeId"]);
                            scheduleentity.Purpose = summaryData["Purpose"];
                            scheduleentity.CRM_StageStatusType = (int)StageStatusType.FvPresentation;

                            scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                            //var schedule = _crmService.SaveCRM_Schedule(scheduleentity);
                        }
                    }
                    else
                    {
                        var initial = new CRMInitialCommunicationViewModel();
                        initial.ContactPerson = summaryData["ContactPerson"];
                        if (summaryData["InterestedInProposal"] == "1")
                            initial.InterestedInPerposal = true;
                        else if (summaryData["InterestedInProposal"] == "0")
                            initial.InterestedInPerposal = false;

                        initial.PURPOSE = Convert.ToInt32(summaryData["PurposeId"]);
                        initial.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.Now;
                        initial.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;

                        initial.CRM_AccountCustomerDetailId = Convert.ToInt32(summaryData["CRM_AccountCustomerDetailId"]);
                        initial.Note = summaryData["Note"];
                        initial.RegionId = SelectedRegionId;
                        initial.CreatedBy = LoginUserId;

                        var initialcommunication = _crmService.SaveCRM_InitialCommunication(CRMAutoMapper.CrmInitialViewModelToEntity(initial));

                        /*Save in Schedule */
                        var scheduleentity = new CRMScheduleViewModel();

                        scheduleentity.ClassId = initialcommunication.CRM_AccountCustomerDetailId;
                        scheduleentity.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
                        scheduleentity.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.UtcNow;


                        scheduleentity.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;
                        scheduleStartEndDateTime = scheduleentity.StartDate + " - " + scheduleentity.EndDate;

                        scheduleentity.Title = accountcustomer.CRM_AccountId + " - " + "FV" + " - " + summaryData["Purpose"] + " - " + accountcustomer.ContactName + " - " + initialcommunication.ContactPerson + " - " + customerAccountDetail.CompanyPhoneNumber;
                        scheduleentity.Location = customerAccountDetail.CompanyAddressLine1;
                        scheduleentity.AuthUserLoginId = summaryData["Asignee"] != "" ? Convert.ToInt32(summaryData["Asignee"]) : 0;
                        scheduleentity.Description = customerAccountDetail.CompanyName + "," + accountcustomer.ContactName + "," + accountcustomer.PhoneNumber;
                        scheduleentity.RegionId = SelectedRegionId;
                        scheduleentity.ModifiedBy = LoginUserId;
                        scheduleentity.PurposeId = Convert.ToInt32(summaryData["PurposeId"]);
                        scheduleentity.Purpose = summaryData["Purpose"];
                        scheduleentity.CRM_StageStatusType = (int)StageStatusType.FvPresentation;
                        scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                        //var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(scheduleentity));

                    }
                    if (summaryData["Asignee"] != "")
                    {
                        int AsigneeId = Convert.ToInt32(summaryData["Asignee"]);
                        UserViewModel oUserViewModel = _userService.GetUserDetail(AsigneeId);
                        if (!string.IsNullOrEmpty(oUserViewModel.Email))
                        {
                            int PurposeId = Convert.ToInt32(summaryData["PurposeId"]);
                            PotentialLeadEmailTemplateModel emailModel = new PotentialLeadEmailTemplateModel
                            {
                                AccountType = CustomerService.GetAccountTypeListById(Convert.ToInt32(summaryData["IndustryType"]))?.Name,
                                BudgetAmount = "$" + summaryData["BudgetAmount"],
                                CompanyName = customerAccountDetail.CompanyName,
                                ContactPhone = customerAccountDetail.CompanyPhoneNumber,
                                LeadNo = _crmService.GetCRM_ProviderTypeName(int.Parse(summaryData["ProviderType"])),
                                Note = summaryData["Note"],
                                Purpose = CRMUtils.GetEnumValuesAndDescriptions<CRM.Common.CRMEnums.SchedulePurpose>()
                                .FirstOrDefault(x => x.Value == PurposeId).Key,
                                Schedule = scheduleStartEndDateTime,
                                UserName = oUserViewModel.FirstName + " " + oUserViewModel.LastName
                            };

                            var html = CRMUtils.RenderPartialViewToString(this.ControllerContext, "PotentialLeadEmailTemplate", emailModel);


                            #region Email send function According to admin configuration

                            //Get Feature Type Id by Feature Name
                            var Leads_Potential_Assignee = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.Leads_Potential_Assignee.ToString().Replace("_", " ")).FirstOrDefault();

                            if (Leads_Potential_Assignee != null && Leads_Potential_Assignee.FeatureTypeId > 0)
                            {
                                //Get Feature Type Email Id by Feature Type Id
                                var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == Leads_Potential_Assignee.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                                if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                                {

                                    if (messageDetails.FromEmail != null && messageDetails.ToEmailId != null)
                                    {
                                        _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, html, "New Lead Assigned");
                                    }

                                    var UserEmailId = jkEntityModel.AuthUserLogins.Where(x => x.UserId == AsigneeId).FirstOrDefault();

                                    if (UserEmailId != null && UserEmailId != null && !string.IsNullOrWhiteSpace(UserEmailId.Email))
                                    {
                                        _mailService.SendEmailAsync(UserEmailId.Email, html, "New Lead Assigned");
                                    }

                                }
                            }
                            #endregion

                        }
                    }
                }
                #endregion

                /* Assign lead to specific sales person if Select Potential and Assignee is not emptly */
                if (int.Parse(summaryData["StageStatus"]) == (int)StageStatusType.Potential && summaryData["Asignee"] != "")
                {
                    accountcustomer.StageStatus = (int)StageStatusType.FvPresentation;
                    accountcustomer.Stage = (int)StageType.Potential;
                    accountcustomer.AssigneeId = int.Parse(summaryData["Asignee"]);
                }
                else
                {
                    if (summaryData["CallResult"] == "12" || summaryData["CallResult"] == "14" || summaryData["CallResult"] == "7")
                    {
                        accountcustomer.StageStatus = (int)StageStatusType.UnqualifiedLead;
                    }
                    else if (summaryData["CallResult"] == "13")
                    {
                        accountcustomer.StageStatus = (int)StageStatusType.CallBack;
                    }
                    else
                    {
                        accountcustomer.StageStatus = int.Parse(summaryData["StageStatus"]);
                    }
                }

                accountcustomer.ProviderType = int.Parse(summaryData["ProviderType"]);
                accountcustomer.ProviderSource = int.Parse(summaryData["ProviderSource"]);
                accountcustomer.ModifiedBy = LoginUserId;

                _crmService.SaveCRM_Account(accountcustomer);
                customerAccountDetail = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);
                //Update StageStatus
                customerAccountDetail.StageStatus = accountcustomer.StageStatus;

                #region Create Call Log
                if (accountcustomer.StageStatus != (int)StageStatusType.QualifiedLead || (summaryData["CallResult"] != "" || summaryData["NoteType"] != ""))
                {
                    var callLogvm = new CRMCallLogViewModel();
                    callLogvm.CRM_AccountCustomerDetailId = customerAccountDetail.CRM_AccountCustomerDetailId;
                    callLogvm.CRM_AccountId = accountcustomer.CRM_AccountId;
                    callLogvm.CRM_LeadSource = accountcustomer.ProviderType ?? default(int);
                    callLogvm.CRM_CallResultId = customerAccountDetail.CRM_CallResultId ?? default(int);
                    callLogvm.CRM_NoteTypeId = customerAccountDetail.CRM_NoteTypeId ?? default(int);
                    callLogvm.SpokeWith = customerAccountDetail.SpokeWith;
                    callLogvm.Callback = customerAccountDetail.Callback;
                    if (summaryData["CallLogTime"] != "")
                        callLogvm.CallBackTime = Convert.ToDateTime(summaryData["CallLogTime"]);
                    callLogvm.Note = customerAccountDetail.GeneralNote;
                    callLogvm.CallLogDate = DateTime.Now;
                    callLogvm.StageStatus = accountcustomer.StageStatus ?? default(int);
                    callLogvm.CreatedBy = LoginUserId;

                    /*Save Call Log*/
                    _crmService.SaveCRM_CallLog(CRMAutoMapper.CrmCallLogViewModelToEntity(callLogvm));
                }

                var calllog = _crmService.GetAll_CRMCallLog(accountcustomer.CRM_AccountId);

                #endregion
                _hasNewData = true;


                return Json(new
                {
                    success = true,
                    result = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(customerAccountDetail)
                });
            }
            else
            {
                NLogger.Error("CustomerAccountDetail does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail does not exist" });
            }

        }
        public ActionResult UpdateCustomerAccountSummary(FormCollection summaryData)
        {
            if (summaryData == null)
            {
                //NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
            //             $"CRM_AccountCustomerDetailId={summaryData["CRM_AccountCustomerDetailId"]}, " +
            //             $"CRM_StageStatus={summaryData["StageStatus"]}, " +
            //             $"IndustryType={summaryData["IndustryType"]}, " +
            //             $"NumberOfLocations={summaryData["NumberOfLocations"]}," +
            //             $"SqFt={summaryData["SqFt"]}, " +
            //             $"ContractExpire={summaryData["ContractExpire"]}," +
            //             $"SalesPossibility={summaryData["SalesPossibility"]}," +
            //             $"CRM_ProviderType={summaryData["ProviderType"]}, " +
            //             $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
            //             $"BudgetAmount={summaryData["BudgetAmount"]}");

            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(int.Parse(summaryData["CRM_AccountCustomerDetailId"]));
            var accountcustomer = _crmService.GetCRM_AccountbyId(int.Parse(summaryData["CRM_AccountId"]));
            if (customerAccountDetail != null && accountcustomer != null)
            {
                customerAccountDetail.AccountTypeListId = Convert.ToInt32(summaryData["IndustryType"]);
                customerAccountDetail.NumberOfLocations = summaryData["NumberOfLocations"] != "" ? Convert.ToInt32(summaryData["NumberOfLocations"]) : 0;
                customerAccountDetail.SqFt = summaryData["SqFt"];
                if (summaryData["ContractExpire"] != "")
                    customerAccountDetail.ContractExpire = Convert.ToDateTime(summaryData["ContractExpire"]);
                customerAccountDetail.CRM_SalePossibilityTypeId = summaryData["SalesPossibility"] != "" ? Convert.ToInt32(summaryData["SalesPossibility"]) : 0;
                customerAccountDetail.BudgetAmount = summaryData["BudgetAmount"] != null ? Convert.ToDecimal(summaryData["BudgetAmount"]) : 0;
                customerAccountDetail.ModifiedBy = LoginUserId;

                accountcustomer.ProviderType = int.Parse(summaryData["ProviderType"]);
                accountcustomer.StageStatus = int.Parse(summaryData["StageStatus"]);
                accountcustomer.ProviderSource = int.Parse(summaryData["ProviderSource"]);
                accountcustomer.ModifiedBy = LoginUserId;

                _crmService.SaveCRM_Account(accountcustomer);
                customerAccountDetail = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);

                _hasNewData = true;

                return Json(new
                {
                    success = true,
                    result = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(customerAccountDetail)
                });
            }
            else
            {
                NLogger.Error("CustomerAccountDetail does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail does not exist" });
            }
        }
        #endregion

        #region CRMLeadCustomerController > Get Call Log List       

        [HttpGet]
        public ActionResult CallLogListData(int accountId)
        {
            var callLogModels = _crmService.GetAll_CRMCallLog(accountId);

            var jsonResult = Json(new
            {
                success = true,
                aadata = callLogModels

            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region CRMLeadCustomerController > GetSchedulesForRegion 

        [HttpGet]
        public ActionResult GetSchedulesForRegion()
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();

            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.RegionId == SelectedRegionId).ToList();

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
                result = accountCustomerScheduleCalendar,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadCustomerController > Save Customer Activity
        [HttpPost]
        public ActionResult SaveCustomerActivitySummary(FormCollection activityData)
        {
            if (activityData == null)
            {
                //NLogger.Error("Requested activityData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            DateTime timestamp = DateTime.ParseExact(activityData["TimeStamp"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

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
            crmActivityViewModel.TimeStamp = timestamp;
            crmActivityViewModel.CRM_AccountCustomerDetailId = Convert.ToInt32(activityData["CRM_AccountCustomerDetailId"]);
            crmActivityViewModel.CreatedBy = LoginUserId;

            var activity = _crmService.SaveCRM_Activity(CRMAutoMapper.CrmActivityEntityToViewModel(crmActivityViewModel));

            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmActivityViewModelToEntity(activity)
            });
        }

        #endregion

        #region CRMLeadCustomerController > Save Customer Schedule

        [HttpPost]
        public ActionResult SaveCustomerScheduleSummary(FormCollection scheduleData)
        {
            if (scheduleData == null)
            {
                //NLogger.Error("Requested scheduleData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            DateTime startDate = DateTime.ParseExact(scheduleData["StartDate"], "dd MMMM yyyy - HH:mm", CultureInfo.InvariantCulture);

            //NLogger.Debug($"CRM_AccountCustomerDetailId={scheduleData["CRM_AccountCustomerDetailId"]}, " +
            //             $"Title={scheduleData["Title"]}, " +
            //             $"Description={scheduleData["Description"]}, " +
            //             $"StartDate={startDate}," +
            //             $"Duration={scheduleData["Duration"]}");


            var scheduleViewModel = new CRMScheduleViewModel();
            scheduleViewModel.ClassId = Convert.ToInt32(scheduleData["CRM_AccountCustomerDetailId"]);
            scheduleViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
            scheduleViewModel.Title = scheduleData["Title"];
            scheduleViewModel.Description = scheduleData["Description"];
            scheduleViewModel.StartDate = startDate;
            scheduleViewModel.CreatedBy = LoginUserId;
            //scheduleViewModel.PurposeId = Convert.ToInt32(scheduleData["PurposeId"]);
            //scheduleViewModel.Purpose = scheduleData["Purpose"];

            var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(scheduleViewModel));
            return Json(new
            {
                success = true,
                result = CRMAutoMapper.CrmScheduleEntityToViewModel(schedule)
            });
        }
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

            schedule.TypeListId = (int)JKApi.Business.Enumeration.TypeList.CRM_AccountCustomer;
            schedule.CRM_StageStatusType = (int)StageStatusType.FvPresentation;
            #endregion Mapping model.....
            var data = _crmService.SaveCRM_ScheduleData(schedule);

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region CRMLeadCustomerController > Save Customer ContactInfo

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
            //            //$"LastName={contactInfoData["LastName"]}, " +
            //            $"PhoneNumber={contactInfoData["PhoneNumber"]}, " +
            //            $"EmailAddress={contactInfoData["EmailAddress"]}," +
            //            $"CompanyName={contactInfoData["CompanyName"]}, " +
            //            $"Title={contactInfoData["Title"]}, " +
            //            $"LineOfBusiness={contactInfoData["LineOfBusiness"]}, " +
            //            $"SqFt={contactInfoData["SqFt"]}," +
            //            $"SalesVolume={contactInfoData["SalesVolume"]}, " +
            //            // $"CompanyPhoneNumber={contactInfoData["CompanyPhoneNumber"]}, " +
            //            // $"CompanyFaxNumber={contactInfoData["CompanyFaxNumber"]}, " +
            //            //$"CompanyEmailAddress={contactInfoData["CompanyEmailAddress"]}," +
            //            $"CompanyAddressLine1={contactInfoData["CompanyAddressLine1"]}, " +
            //            $"CompanyAddressLine2={contactInfoData["CompanyAddressLine2"]}, " +
            //            $"CompanyCity={contactInfoData["CompanyCity"]}," +
            //            $"CompanyCounty={contactInfoData["CompanyCounty"]}," +
            //            $"CompanyState={contactInfoData["CompanyState"]}," +
            //            $"CompanyZipCode={contactInfoData["CompanyZipCode"]}," +
            //            $"CompnayWebSite={contactInfoData["CompnayWebSite"]}");


            var accountcustomer = _crmService.GetCRM_AccountbyId(Convert.ToInt32(contactInfoData["CRM_AccountId"]));
            var customerAccountDetail = _crmService.GetCRM_AccountCustomerDetailbyId(Convert.ToInt32(contactInfoData["CRM_AccountCustomerDetailId"]));
            if (accountcustomer != null && customerAccountDetail != null)
            {
                accountcustomer.ContactName = contactInfoData["ContactName"];
                //accountcustomer.LastName = contactInfoData["LastName"];
                accountcustomer.PhoneNumber = contactInfoData["PhoneNumber"];
                accountcustomer.EmailAddress = contactInfoData["EmailAddress"];
                accountcustomer.ModifiedBy = LoginUserId;

                customerAccountDetail.CRM_AccountCustomerDetailId = Convert.ToInt32(contactInfoData["CRM_AccountCustomerDetailId"]);
                customerAccountDetail.CRM_AccountId = Convert.ToInt32(contactInfoData["CRM_AccountId"]);
                customerAccountDetail.CompanyName = contactInfoData["CompanyName"];
                //customerAccountDetail.SqFt = contactInfoData["SqFt"];
                //customerAccountDetail.SalesVolume = contactInfoData["SalesVolume"];
                ////customerAccountDetail.LineofBusiness = contactInfoData["LineOfBusiness"];
                //customerAccountDetail.AccountTypeListId = Convert.ToInt32(contactInfoData["IndustryType"]);
                //customerAccountDetail.NumberOfLocations = Convert.ToInt32(contactInfoData["NumberOfLocations"]);
                customerAccountDetail.CompanyPhoneNumber = contactInfoData["CompanyPhoneNumber"];
                //customerAccountDetail.CompanyFaxNumber = contactInfoData["CompanyFaxNumber"];
                customerAccountDetail.Title = contactInfoData["Title"];
                //customerAccountDetail.CompanyWebSite = contactInfoData["CompnayWebSite"];
                //customerAccountDetail.CompanyEmailAddress = contactInfoData["CompanyEmailAddress"];
                customerAccountDetail.CompanyAddressLine1 = contactInfoData["CompanyAddressLine1"];
                customerAccountDetail.CompanyAddressLine2 = contactInfoData["CompanyAddressLine2"];
                customerAccountDetail.CompanyCity = contactInfoData["CompanyCity"];
                customerAccountDetail.CompanyCounty = contactInfoData["CompanyCounty"];
                customerAccountDetail.CompanyState = contactInfoData["CompanyState"];
                customerAccountDetail.CompanyZipCode = contactInfoData["CompanyZipCode"];
                customerAccountDetail.ModifiedBy = LoginUserId;

                var account = _crmService.SaveCRM_Account(accountcustomer);
                var accountCustomer = _crmService.SaveCRM_AccountCustomerDetail(customerAccountDetail);
                ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", customerAccountDetail.CompanyState);

                /* Customer Account Detail to AccountCustomerDetailViewModel with Partial View String */
                var accountCustomerVm = CRMAutoMapper.CrmAccountCustomerEntityToViewModel(accountCustomer);
                accountCustomerVm.ContactName = account.ContactName;
                accountCustomerVm.PhoneNumber = account.PhoneNumber;
                accountCustomerVm.EmailAddress = account.EmailAddress;
                var customerAccountReferenceInfo = CRMUtils.RenderPartialViewToString(this.ControllerContext, "_CRMCustomerAccountReferenceInformation", accountCustomerVm);


                return Json(new
                {
                    success = true,
                    customerAccountReferenceInfo
                });
            }

            else
            {
                NLogger.Error("CustomerAccountDetail and CustomerAccount does not exist");
                return Json(new { success = false, message = "CustomerAccountDetail and CustomerAccount does not exist" });
            }

        }
        #endregion

        #region CRMLeadCustomerController > SearchCustomerLead
        [HttpPost]
        public ActionResult SearchCustomerLead(CRMSearchModel postData)
        {
            var result = _crmService.CRM_LeadCustomerSearch(postData);
            if (result.Count() >= 0)
            {
                Session["SearchList"] = null;
                Session["SearchList"] = result.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }
            return Json(new
            {
                success = true,
                aadata = result

            });
        }


        #endregion

        #endregion

        #region CRMLeadCustomerController > Upload Excel

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase FileUpload)
        {
            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = "/Upload/ImportDoc/";


                    string severFilePath = Server.MapPath(targetpath);
                    if (!Directory.Exists(severFilePath))
                    { // if it doesn't exist, create
                        Directory.CreateDirectory(severFilePath);
                    }
                    // FileUpload.SaveAs(severFilePath + filename);
                    FileUpload.SaveAs(Path.Combine(severFilePath, filename));
                    DataTable dt = ExcelToDataTable(severFilePath, filename);
                    int totalRows = dt.Rows.Count;
                    int duplicateRows = 0;
                    if (totalRows > 0)
                    {
                        duplicateRows = _crmService.ImportExcelData(dt);
                        string pathToExcelFile = severFilePath + filename;
                        if ((System.IO.File.Exists(pathToExcelFile)))
                        {
                            System.IO.File.Delete(pathToExcelFile);
                        }
                        _hasNewData = true;
                    }

                    int processedRows = totalRows - duplicateRows;
                    string message = "";
                    if (processedRows > 0)
                    {
                        message = processedRows + " records are processed successfully";
                    }

                    if (duplicateRows > 0)
                    {
                        message = message + Environment.NewLine + duplicateRows + " records are not processed becuase company name is alredy exist in database";
                    }
                    return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region CRMLeadCustomerController > Excel To Data Table
        public DataTable ExcelToDataTable(string filePath, string filename)
        {
            string pathToExcelFile = filePath + filename;
            DataTable dt = new DataTable();
            var file = new FileStream(pathToExcelFile, FileMode.Open);

            IWorkbook workbook = null;

            if (Path.GetExtension(pathToExcelFile) == ".xls")
                workbook = new HSSFWorkbook(file);
            else if (Path.GetExtension(pathToExcelFile) == ".xlsx")
                workbook = new XSSFWorkbook(file);
            else if (Path.GetExtension(pathToExcelFile) == ".csv")
                workbook = new XSSFWorkbook(file);

            ISheet sheet = workbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (Convert.ToString(cell) == "Contact Name")
                {
                    dt.Columns.Add("FirstName");
                    dt.Columns.Add("LastName");
                }
                else
                    dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                DataRow dataRow = dt.NewRow();
                if (row == null)
                {
                    break;
                }
                int dtindex = 0;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        if (Convert.ToString(sheet.GetRow(0).GetCell(j)) == "Contact Name")
                        {
                            //string[] contact = Convert.ToString(row.GetCell(j)).Split(' ');
                            var contact = Convert.ToString(row.GetCell(j));
                            var firstSpaceIndex = contact.IndexOf(" ");
                            var firstname = contact.Substring(0, firstSpaceIndex);
                            var lastname = contact.Substring(firstSpaceIndex + 1);
                            if (contact.Length > 1)
                            {
                                dataRow[j] = firstname;
                                dataRow[j + 1] = lastname;
                            }
                            dtindex = 1;
                        }
                        else
                        {
                            dataRow[j + dtindex] = row.GetCell(j).ToString();
                        }
                    }
                    else
                    {
                        if (Convert.ToString(sheet.GetRow(0).GetCell(j)) == "Contact Name")
                        {
                            dtindex = 1;
                        }
                    }
                }
                dt.Rows.Add(dataRow);
            }

            /*column should set to null but not empty*/
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                    {
                        dt.Rows[i][j] = null;
                    }
                }
            }
            return dt;
        }

        #endregion

        #region ::Check Company Name Exits ::
        public ActionResult CheckCompanynameExits(string CompanyName, string Phone)
        {
            //int IsExits = CustomerService.CheckCustomerNamePhoneIsExist(CompanyName, Phone);
            //return Json(new { IsExits = IsExits, success = true }, JsonRequestBehavior.AllowGet);  
            int AccId = CustomerService.CheckCustomerNamePhoneIsExist(CompanyName, Phone);
            return Json(new { AccId = AccId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Lead ReAssign to Sales User


        private SelectList SalesUserResultList
        {
            get
            {
                var lstSaleUsers = _crmService.GetAll_CRM_ReAssignLeadUserList();
                var items = new List<SelectListItem>();
                foreach (var user in lstSaleUsers)
                {
                    var item = new SelectListItem
                    {
                        Value = user.UserId.ToString(),
                        Text = user.FirstName
                    };
                    items.Add(item);
                }
                return new SelectList(items, "Value", "Text");
            }
        }

        [HttpGet]
        public JsonResult GetTerritoriesByUser(int UserId)
        {
            var lstTerritories = _crmService.CRM_spGet_LeadReAssignTerritoryListByUserId(UserId);
            return Json(lstTerritories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeadReAssign()
        {
            #region Sales User,Territory Dropdown       
            ViewBag.UserList = new SelectList(SalesUserResultList, "Value", "Text");

            #endregion

            return View();
        }

        [HttpGet]
        public ActionResult ReAssignLeadsList(int UserId, int TerritoryId)
        {
            var lstResult = _crmService.GetAll_CRM_ReAssignLeadList(UserId, TerritoryId);

            ViewBag.SalesUserList = new SelectList(lstResult.LeadReAssignSalesUser, "UserId", "FirstName", "Select Sales Person");

            return PartialView("_ReAssignLeadsList", lstResult.LeadReAssignLeadList);

            //return Json(new
            //{
            //    LeadReAssignTerritory = lstResult.LeadReAssignTerritory,
            //    leadreassign = RenderPartialToString(ControllerContext, "_ReAssignLeadsList", lstResult.LeadReAssignLeadList)
            //}, JsonRequestBehavior.AllowGet);
        }

        //private string RenderPartialToString(ControllerContext context, string viewName, object model)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = context.RouteData.GetRequiredString("action");

        //    var viewData = new ViewDataDictionary(model);

        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
        //        var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
        //        viewResult.View.Render(viewContext, sw);

        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
        public ActionResult LeadReAssign_Save(List<LeadListView> lstLeads)
        {
            var lstResult = _crmService.AssignLeadsToUser(lstLeads);
            return Json("sucess");
        }
        #endregion
    }
}