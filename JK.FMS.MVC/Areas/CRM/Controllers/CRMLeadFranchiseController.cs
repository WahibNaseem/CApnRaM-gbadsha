using Application.Web.Core;
using JK.FMS.MVC.Areas.CRM.Common;
using JKViewModels.CRM;
using MvcBreadCrumbs;
using System.Linq;
using System.Web.Mvc;
using Constants = JKApi.Service.ServiceContract.CRM.CRM_ServiceConstants;
using JK.Resources;
using JKApi.Service.ServiceContract.CRM;
using System.Collections.Generic;
using System;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Core.Common;
using System.Data;
using System.Web;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    public class CRMLeadFranchiseController : ViewControllerBase
    {

        #region Life Cycle
        private readonly ICRM_Service _crmService;
        private bool _hasNewData;
        private const bool V = true;
        public CRMLeadFranchiseController(ICRM_Service ICRMService, ICustomerService customerService, ClaimView claimview)
        {
            _crmService = ICRMService;
            CustomerService = customerService;
            _claimView = claimview;
            ViewBag.HMenu = "CRMLeadFranchise";
            _hasNewData = false;

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
                    .Where(x => x.Name == Constants.Key_UnqualifiedLead ||
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

        private SelectList CallResultList
        {
            get
            {
                var calllogcallresult = _crmService.GetAll_CRM_CallResult();
                var items = new List<SelectListItem>();
                foreach (var stagestatus in calllogcallresult)
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

        #region CRMLeadFranchiseController> Franchise
        // GET: CRM/CRMLeadFranchise
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Detail", "CRMLeadFranchise", new { area = "CRM" }), "Leads");

            ViewBag.ProviderTypeList = ProviderTypeList;
            return View();
        }

        public ActionResult QualifiedLeads()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Detail", "CRMLeadFranchise", new { area = "CRM" }), "Leads");

            return View();
        }
        public ActionResult UnQualifiedLeads()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            return View();
        }

        public ActionResult CallBackLeads()
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadCustomer", new { area = "CRM" }), "Leads");

            return View();
        }

        #endregion

        #region CRMLeadFranchiseController > LeadDetail
        [HttpGet]
        public ActionResult Detail(int accountId)
        {
            ViewBag.HMenu = "CRMLeadFranchise";
            ViewBag.CurrentMenu = JKCRMResource.lead;
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashBoard", new { area = "CRM" }), "CRM");
            BreadCrumb.Add(Url.Action("Index", "CRMLeadFranchise", new { area = "CRM" }), "Leads");

            var accountFranchiseDetailViewModel = new CRMAccountFranchiseDetailViewModel();

            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountFranchiseDetail = _crmService.GetAll_CRM_AccountFranchiseDetail().ToList().FirstOrDefault(x => x.CRM_AccountId == accountId);


            if (account != null && accountFranchiseDetail != null)
            {
                accountFranchiseDetailViewModel = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(accountFranchiseDetail);
                accountFranchiseDetailViewModel.ContactName = Convert.ToString(account.ContactName.ToString());

                accountFranchiseDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountFranchiseDetailViewModel.EmailAddress = account.EmailAddress;


                var iniate = CRMAutoMapper.CrmInitialEntityToViewModel(_crmService.GetCRM_InitialCommunication_ByAccountFranchiseDetailById(accountFranchiseDetail.CRM_AccountFranchiseDetailId));
                if (iniate != null)
                {
                    accountFranchiseDetailViewModel.ContactPerson = iniate.ContactPerson;

                    if (iniate.StartDate != null)
                    {
                        accountFranchiseDetailViewModel.datestart = iniate.StartDate != null ? iniate.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        accountFranchiseDetailViewModel.timestart = iniate.StartDate.Value.ToString("hh:mm tt");

                        accountFranchiseDetailViewModel.dateend = iniate.EndDate != null ? iniate.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        accountFranchiseDetailViewModel.timeend = iniate.EndDate != null ? iniate.EndDate.Value.ToString("hh:mm tt") : "";
                    }
                    accountFranchiseDetailViewModel.InterestedInPerposal = iniate.InterestedInPerposal;
                    accountFranchiseDetailViewModel.Purpose = Convert.ToInt32(iniate.PURPOSE);
                }

                accountFranchiseDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountFranchiseDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountFranchiseDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountFranchiseDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;

                var leadName = $"{accountFranchiseDetailViewModel.FirstName} {accountFranchiseDetailViewModel.LastName}";
                BreadCrumb.Add(Url.Action("Detail", "CRMLeadFranchise", new { area = "CRM" }), leadName);
            }

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
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.CallTypeResult = accountFranchiseDetailViewModel.CRM_CallResultId;
            ViewBag.CallResultList = new SelectList(_crmService.GetAll_CRM_CallResult().ToList().OrderBy(x => x.Name), "CRM_CallResultId", "Name", accountFranchiseDetailViewModel.CRM_CallResultId);
            ViewBag.NoteType = new SelectList(_crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name), "CRM_NoteTypeId", "Name", accountFranchiseDetailViewModel.CRM_NoteTypeId);
            var types = from CRMEnums.JopType t in Enum.GetValues(typeof(CRMEnums.JopType))
                        select new { Id = (int)t, name = t.ToString() };
            ViewBag.JobType = new SelectList(types, "Id", "name", accountFranchiseDetailViewModel.JkFull);

            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", accountFranchiseDetailViewModel.State);
            ViewBag.PurposeType = new SelectList(_crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMLeadGeneration == true).OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name", accountFranchiseDetailViewModel.Purpose);

            return View(accountFranchiseDetailViewModel);
        }
        #endregion

        #region CRMLeadFranchiseController > HttpRequest

        #region Get Franchise Lead  Detail 

        /// <summary>
        /// Get Franchise Lead Detail when Next or Previous Button Click
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>

        public ActionResult GetFranchiseLeadDetail(int accountId)
        {
            var accountFranchiseDetailViewModel = new CRMAccountFranchiseDetailViewModel();

            var account = _crmService.GetCRM_AccountbyId(accountId);
            var accountFranchiseDetail = _crmService.GetAll_CRM_AccountFranchiseDetail().ToList().FirstOrDefault(x => x.CRM_AccountId == accountId);


            if (account != null && accountFranchiseDetail != null)
            {
                accountFranchiseDetailViewModel = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(accountFranchiseDetail);
                accountFranchiseDetailViewModel.ContactName = Convert.ToString(account.ContactName.ToString());

                accountFranchiseDetailViewModel.PhoneNumber = account.PhoneNumber;
                accountFranchiseDetailViewModel.EmailAddress = account.EmailAddress;


                var iniate = CRMAutoMapper.CrmInitialEntityToViewModel(_crmService.GetCRM_InitialCommunication_ByAccountFranchiseDetailById(accountFranchiseDetail.CRM_AccountFranchiseDetailId));
                if (iniate != null)
                {
                    accountFranchiseDetailViewModel.ContactPerson = iniate.ContactPerson;

                    if (iniate.StartDate != null)
                    {
                        accountFranchiseDetailViewModel.datestart = iniate.StartDate != null ? iniate.StartDate.Value.ToString("MM/dd/yyyy") : "";
                        accountFranchiseDetailViewModel.timestart = iniate.StartDate.Value.ToString("h:mm:ss tt");

                        accountFranchiseDetailViewModel.dateend = iniate.EndDate != null ? iniate.EndDate.Value.ToString("MM/dd/yyyy") : "";
                        accountFranchiseDetailViewModel.timeend = iniate.EndDate != null ? iniate.EndDate.Value.ToString("h:mm:ss tt") : "";
                    }
                    accountFranchiseDetailViewModel.InterestedInPerposal = iniate.InterestedInPerposal;
                    accountFranchiseDetailViewModel.Purpose = Convert.ToInt32(iniate.PURPOSE);
                }

                accountFranchiseDetailViewModel.Stage = (StageType)account.Stage.Value;
                accountFranchiseDetailViewModel.StageStatus = (StageStatusType)account.StageStatus.Value;
                accountFranchiseDetailViewModel.ProviderSource = (AccountSourceProvider)account.ProviderSource.Value;
                accountFranchiseDetailViewModel.ProviderType = (AccountProviderType)account.ProviderType.Value;

                var leadName = $"{accountFranchiseDetailViewModel.FirstName} {accountFranchiseDetailViewModel.LastName}";
            }

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
            ViewBag.IndustryTypeList = IndustryTypeList;
            ViewBag.ProviderTypeList = ProviderTypeList;
            ViewBag.ProviderSourceList = ProviderSourceList;
            ViewBag.CallResultList = new SelectList(_crmService.GetAll_CRM_CallResult().ToList().OrderBy(cr => cr.Name), "CRM_CallResultId", "Name", accountFranchiseDetailViewModel.CRM_CallResultId);
            ViewBag.NoteType = new SelectList(_crmService.GetAll_CRM_NoteType().ToList().OrderBy(x => x.Name), "CRM_NoteTypeId", "Name", accountFranchiseDetailViewModel.CRM_NoteTypeId);
            var types = from CRMEnums.JopType t in Enum.GetValues(typeof(CRMEnums.JopType))
                        select new { Id = (int)t, name = t.ToString() };
            ViewBag.JobType = new SelectList(types, "Id", "name", accountFranchiseDetailViewModel.JkFull);
            ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", accountFranchiseDetailViewModel.State);
            ViewBag.PurposeType = new SelectList(_crmService.GetAll_CRM_PurposeType().ToList().Where(x => x.CRMLeadGeneration == true).OrderBy(x => x.Name), "CRM_PurposeTypeId", "Name", accountFranchiseDetailViewModel.Purpose);

            var franchiseAccountReferenceInfo = CRMUtils.RenderPartialViewToString(controllercontext: ControllerContext, viewname: "_CRMFranchiseAccountReferenceInformation", model: accountFranchiseDetailViewModel);
            var franchiseAccountSummary = CRMUtils.RenderPartialViewToString(controllercontext: ControllerContext, viewname: "_CRMFranchiseAccountSummary", model: accountFranchiseDetailViewModel);

            var data = new
            {
                success = V,
                franchiseAccountReferenceInfo,
                franchiseAccountSummary
            };

            return Json(data, behavior: JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CRMLeadFranchiseController > Franchisee New Lead List
        public ActionResult FranchiseNewLeadListData()
        {
            /*Passing first Parameter 0 for New Lead*/
            var franchiseeLeadViewModels = _crmService.GetAll_CRM_PotentialFranchisee(0, 0, SelectedUserId, SelectedRegionId);

            /*Check if franchise have new leads*/
            if (franchiseeLeadViewModels.Count > 0)
            {
                /*Make Null Search list */
                Session["SearchList"] = null;
                /*Put Franhise New Lead Id into SearchList*/
                Session["SearchList"] = franchiseeLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }

            var jsonResult = Json(new
            {
                success = V,
                aadata = franchiseeLeadViewModels
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CRMLeadFranchiseController > Franchisee Qualified Lead List
        public ActionResult FranchiseQualifiedListData()
        {
            /*Passing First Parameter 1 for Qualifeid Lead*/
            var franchiseeLeadViewModels = _crmService.GetAll_CRM_PotentialFranchisee(1, 0, SelectedUserId, SelectedRegionId);

            /* Check if Franhise have Qualified Leads */
            if (franchiseeLeadViewModels.Count > 0)
            {
                /*Make Null Search list*/
                Session["SearchList"] = null;
                /*Put Franchise Qualified Lead Id into SearchList in a order*/
                Session["SearchList"] = franchiseeLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();

            }

            var jsonResult = Json(new
            {
                success = V,
                aadata = franchiseeLeadViewModels
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CRMLeadFranchiseController > Franchisee UnQualified Lead List
        public ActionResult FranchiseUnQualifiedListData()
        {
            /*Passing First Parameter 2 for UnQualifeid Lead*/
            var franchiseeLeadViewModels = _crmService.GetAll_CRM_PotentialFranchisee(2, 0, SelectedUserId, SelectedRegionId);

            /* Check if Franchise have UnQualified Leads*/
            if (franchiseeLeadViewModels.Count > 0)
            {
                /*Make Null the SearchList*/
                Session["SearchList"] = null;
                /*Put Franchise UnQualified Lead Ids into SearchList */
                Session["SearchList"] = franchiseeLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }

            var jsonResult = Json(new
            {
                success = V,
                aadata = franchiseeLeadViewModels
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion   

        #region CRMLeadFranchiseController > Franchisee Call Back Lead List

        [HttpGet]
        public ActionResult FranchiseCallBackListData()
        {
            /*Passing first Parameter 5 for CallBack Leads */
            var franchiseeLeadViewModels = _crmService.GetAll_CRM_PotentialFranchisee(5, 0, SelectedUserId, SelectedRegionId);

            /*Check if franchise have CallBack Leads*/
            if (franchiseeLeadViewModels.Count > 0)
            {
                Session["SearchList"] = null;
                Session["SearchList"] = franchiseeLeadViewModels.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId).ToList();
            }

            var jsonResult = Json(new
            {
                success = V,
                aadata = franchiseeLeadViewModels
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CRMLeadFranchiseController > Creat Lead
        [HttpPost]
        public ActionResult Create(FormCollection accountData)
        {
            if (!(accountData.Count > 0))
            {
                //NLogger.Error("Requested AccountData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"companyname={accountData["Companyname"]}, " +
            //             $"contactname={accountData["ContactName"]}, " +
            //             $"mobile={accountData["PhoneNumber"]}, " +
            //             $"email={accountData["EmailAddress"]}");

            var accountCustomer = _crmService.AddNewCustomerAccount(
                accountData["CompanyName"],
                accountData["ContactName"],
                accountData["PhoneNumber"],
                accountData["EmailAddress"],
                Convert.ToInt32(accountData["SourceProvider"]),
               //_crmService.GetCRM_ProviderTypeIndex(accountData["SourceProvider"].ToLower()),
               _crmService.GetCRM_ProviderSourceIndex("inhouse"), Constants.Key_Franchise, LoginUserId, SelectedRegionId, accountData["Note"]
            );

            return Json(new
            {
                success = V,
                result = CRMAutoMapper.CrmAccountEntityToViewModel(accountCustomer)
            });
        }

        #endregion

        #region CRMLeadFranchiseController > Save Franchise Account Summary
        [HttpPost]
        public ActionResult SaveFranchiseAccountSummary(FormCollection summaryData)
        {
            if (!(summaryData.Count > 0))
            {
                //NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
            //              $"CRM_AccountFranchiseDetailId={summaryData["CRM_AccountFranchiseDetailId"]}, " +
            //              $"CRM_StageStatus={summaryData["StageStatus"]}, " +
            //              $"CRM_ProviderType={summaryData["ProviderType"]}, " +
            //              $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
            //              $"BudgetAmount={summaryData["BudgetAmount"]}, " +
            //               $"CallResult={summaryData["CallResult"]}, " +
            //              $"SpokeWith={summaryData["SpokeWith"]}, " +
            //              $"CallLogDate={summaryData["CallLogDate"]}, " +
            //              $"CallLogTime={summaryData["CallLogTime"]}, " +
            //              $"NoteType={summaryData["NoteType"]}, " +
            //              $"CallLogNote={summaryData["CallLogNote"]}, " +
            //              $"InterestedInProposal={summaryData["InterestedInProposal"]}, " +
            //              $"ScheduleStartDate={summaryData["ScheduleStartDate"]}, " +
            //              $"ScheduleStartTime={summaryData["ScheduleStartTime"]}, " +
            //              $"ScheduleEndDate={summaryData["ScheduleEndDate"]}, " +
            //              $"ScheduleEndTime={summaryData["ScheduleEndTime"]}, " +
            //               $"ScheduleEndTime={summaryData["ScheduleEndTime"]}, " +
            //               $"PurposeId={summaryData["PurposeId"]}, " +
            //               $"Purpose={summaryData["Purpose"]}, " +
            //              $"Asignee={summaryData["Asignee"]}, " +
            //              $"Note={summaryData["Note"]}");

            var accountId = Convert.ToInt32(summaryData["CRM_AccountId"]);

            var franchiseAccountDetail = _crmService.GetAll_CRM_AccountFranchiseDetail().FirstOrDefault(x => x.CRM_AccountId == accountId);
            var accountfranchise = _crmService.GetCRM_AccountbyId(accountId);
            if (franchiseAccountDetail != null && accountfranchise != null)
            {
                franchiseAccountDetail.AmtToInvest = Convert.ToDecimal(summaryData["BudgetAmount"]);
                franchiseAccountDetail.CRM_NoteTypeId = summaryData["NoteType"] != "" ? Convert.ToInt32(summaryData["NoteType"]) : 0;
                franchiseAccountDetail.Notes = summaryData["Note"];

                franchiseAccountDetail.CRM_CallResultId = summaryData["CallResult"] != "" ? Convert.ToInt32(summaryData["CallResult"]) : (int?)null;
                franchiseAccountDetail.SpokeWith = summaryData["SpokeWith"];
                franchiseAccountDetail.CRM_Note = summaryData["CallLogNote"];

                if (summaryData["CallLogDate"] != "")
                {
                    franchiseAccountDetail.CallBack = Convert.ToDateTime(summaryData["CallLogDate"] + " " + summaryData["CallLogTime"]);
                }
                franchiseAccountDetail.ModifiedBy = LoginUserId;


                #region Initial Communication Code As Lead generation  

                franchiseAccountDetail.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.Now;
                franchiseAccountDetail.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;
                franchiseAccountDetail.Purpose = int.Parse(summaryData["PurposeId"]);

                /*Check If TeleMarketer Select Potential To Make Initial Communication*/
                if (int.Parse(summaryData["StageStatus"]) == (int)StageStatusType.Potential)
                {
                    /*Initial Communication */
                    var franchiseInitialCommunicate = _crmService.GetCRM_InitialCommunication_ByAccountFranchiseDetailById(franchiseAccountDetail.CRM_AccountFranchiseDetailId);

                    #region Check InitialCommunication does Exist  

                    /*Check if InitialCommunication Not Exist  */
                    if (franchiseInitialCommunicate != null)
                    {
                        franchiseInitialCommunicate.ContactPerson = summaryData["ContactPerson"];
                        if (summaryData["InterestedInProposal"] != "")
                            franchiseInitialCommunicate.InterestedInPerposal = Convert.ToBoolean(Convert.ToInt32(summaryData["InterestedInProposal"]));

                        franchiseInitialCommunicate.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.Now;


                        franchiseInitialCommunicate.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;
                        franchiseInitialCommunicate.CRM_AccountFranchiseDetailId = Convert.ToInt32(summaryData["CRM_AccountFranchiseDetailId"]);
                        franchiseInitialCommunicate.Note = summaryData["Note"];
                        franchiseInitialCommunicate.PURPOSE = Convert.ToInt32(summaryData["PurposeId"]);
                        franchiseInitialCommunicate.RegionId = SelectedRegionId;
                        franchiseInitialCommunicate.ModifiedBy = LoginUserId;

                        var franchiseinitialcommunication = _crmService.SaveCRM_InitialCommunication(franchiseInitialCommunicate);


                        var scheduleentity = _crmService.GetAll_CRM_Schedule().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == franchiseAccountDetail.CRM_AccountFranchiseDetailId && x.CRM_StageStatusType == (int)StageStatusType.LeadGeneration);
                        if (scheduleentity != null)
                        {
                            scheduleentity.CRM_AccountFranchiseDetailId = franchiseAccountDetail.CRM_AccountFranchiseDetailId;
                            scheduleentity.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.UtcNow;
                            scheduleentity.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.UtcNow;
                            scheduleentity.Title = accountfranchise.CRM_AccountId + " - " + "LG" + " - " + summaryData["Purpose"] + " - " + accountfranchise.ContactName + " - " + accountfranchise.PhoneNumber;

                            scheduleentity.Location = franchiseAccountDetail.Address1;
                            scheduleentity.AuthUserLoginId = summaryData["Asignee"] != "" ? Convert.ToInt32(summaryData["Asignee"]) : 0;
                            scheduleentity.Description = franchiseAccountDetail.FranchiseeName + "," + accountfranchise.ContactName + "," + accountfranchise.PhoneNumber;
                            scheduleentity.RegionId = SelectedRegionId;

                            scheduleentity.ModifiedBy = LoginUserId;
                            scheduleentity.PurposeId = Convert.ToInt32(summaryData["PurposeId"]);
                            scheduleentity.Purpose = summaryData["Purpose"];
                            scheduleentity.CRM_StageStatusType = (int)StageStatusType.IntialCommunication;

                            scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                            var schedule = _crmService.SaveCRM_Schedule(scheduleentity);
                        }

                    }
                    #endregion

                    #region Create InitialCommunication as Lead Generation


                    else
                    {
                        var franchiseInitial = new CRMInitialCommunicationViewModel();
                        franchiseInitial.ContactPerson = summaryData["ContactPerson"];
                        if (summaryData["InterestedInProposal"] != "")
                            franchiseInitial.InterestedInPerposal = Convert.ToBoolean(Convert.ToInt32(summaryData["InterestedInProposal"]));

                        franchiseInitial.PURPOSE = Convert.ToInt32(summaryData["PurposeId"]);
                        franchiseInitial.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.UtcNow;
                        franchiseInitial.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.UtcNow;
                        franchiseInitial.CRM_AccountFranchiseDetailId = Convert.ToInt32(summaryData["CRM_AccountFranchiseDetailId"]);

                        franchiseInitial.Note = summaryData["Note"];
                        franchiseInitial.RegionId = SelectedRegionId;
                        franchiseInitial.CreatedBy = LoginUserId;

                        var initialcommunication = _crmService.SaveCRM_InitialCommunication(CRMAutoMapper.CrmInitialViewModelToEntity(franchiseInitial));

                        /*Save in Schedule */
                        var scheduleentity = new CRMScheduleViewModel();

                        scheduleentity.CRM_AccountFranchiseDetailId = initialcommunication.CRM_AccountFranchiseDetailId;
                        scheduleentity.StartDate = summaryData["ScheduleStartDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleStartDate"] + " " + summaryData["ScheduleStartTime"]) : DateTime.UtcNow;
                        scheduleentity.EndDate = summaryData["ScheduleEndDate"] != "" ? Convert.ToDateTime(summaryData["ScheduleEndDate"] + " " + summaryData["ScheduleEndTime"]) : DateTime.Now;

                        scheduleentity.Location = franchiseAccountDetail.Address1;
                        scheduleentity.AuthUserLoginId = summaryData["Asignee"] != "" ? Convert.ToInt32(summaryData["Asignee"]) : 0;
                        scheduleentity.Description = franchiseAccountDetail.FranchiseeName + "," + accountfranchise.ContactName + "," + accountfranchise.PhoneNumber;
                        scheduleentity.RegionId = SelectedRegionId;

                        scheduleentity.ModifiedBy = LoginUserId;
                        scheduleentity.PurposeId = Convert.ToInt32(summaryData["PurposeId"]);
                        scheduleentity.Purpose = summaryData["Purpose"];
                        scheduleentity.CRM_StageStatusType = (int)StageStatusType.IntialCommunication;

                        scheduleentity.CRM_ScheduleTypeId = 2; /* Schedule type = CRM*/
                        var schedule = _crmService.SaveCRM_Schedule(CRMAutoMapper.CrmScheduleViewModelToEntity(scheduleentity));
                    }
                    #endregion
                }

                #endregion

                /* Assign lead to specific sales person */
                if (int.Parse(summaryData["StageStatus"]) == (int)StageStatusType.Potential && summaryData["Asignee"] != "")
                {
                    accountfranchise.Stage = (int)StageType.Potential;
                    accountfranchise.StageStatus = (int)StageStatusType.PotentialInquary;
                    accountfranchise.AssigneeId = int.Parse(summaryData["Asignee"]);
                }

                else
                {
                    accountfranchise.StageStatus = int.Parse(summaryData["StageStatus"]);
                }

                accountfranchise.ProviderType = int.Parse(summaryData["ProviderType"]);
                accountfranchise.ProviderSource = int.Parse(summaryData["ProviderSource"]);

                _crmService.SaveCRM_Account(accountfranchise);
                franchiseAccountDetail = _crmService.SaveCRM_AccountFranchiseDetail(franchiseAccountDetail);

                return Json(new
                {
                    success = V,
                    result = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(franchiseAccountDetail)
                });
            }
            else
            {
                NLogger.Error("FranchiseAccountDetail does not exist");
                return Json(new { success = false, message = "FranchiseAccountDetail does not exist" });
            }
        }

        #endregion

        #region CRMLeadFranchiseController > UpdateFranchiseAccountSummary

        /// <summary>
        /// Update Account Summary Data like , Lead Source , Current Proivder, Budget
        /// </summary>
        /// <param name="summaryData"></param>
        /// <returns></returns>
        public ActionResult UpdateFranchiseAccountSummary(FormCollection summaryData)
        {
            if (!(summaryData.Count > 0))
            {
                /*  NLogger.Error("Requested SummaryData is null or empty");*/
                return Json(new { success = false, message = "Data is null or empty" });
            }

            /*   NLogger.Debug($"CRM_AccountId={summaryData["CRM_AccountId"]}, " +
                             $"CRM_AccountFranchiseDetailId={summaryData["CRM_AccountFranchiseDetailId"]}, " +
                             $"CRM_StageStatus={summaryData["StageStatus"]}, " +
                             $"CRM_ProviderType={summaryData["ProviderType"]}, " +
                             $"CRM_ProviderSource={summaryData["ProviderSource"]}, " +
                             $"BudgetAmount={summaryData["BudgetAmount"]}");*/

            var accountId = Convert.ToInt32(summaryData["CRM_AccountId"]);

            var franchiseAccountDetail = _crmService.GetAll_CRM_AccountFranchiseDetail().FirstOrDefault(x => x.CRM_AccountId == accountId);
            var accountfranchise = _crmService.GetCRM_AccountbyId(accountId);
            if (franchiseAccountDetail != null && accountfranchise != null)
            {
                /* Update Franchise Detail */
                franchiseAccountDetail.AmtToInvest = Convert.ToDecimal(summaryData["BudgetAmount"]);
                franchiseAccountDetail.CRM_NoteTypeId = summaryData["NoteType"] != "" ? Convert.ToInt32(summaryData["NoteType"]) : 0;
                franchiseAccountDetail.Notes = summaryData["Note"];
                franchiseAccountDetail.ModifiedBy = LoginUserId;

                /* Update Franchise account */
                if (summaryData["ProviderSource"] != null)
                    accountfranchise.ProviderSource = Convert.ToInt32(summaryData["ProviderSource"]);
                if (summaryData["ProviderType"] != null)
                    accountfranchise.ProviderType = Convert.ToInt32(summaryData["ProviderType"]);
                if (summaryData["StageStatus"] != null)
                    accountfranchise.StageStatus = Convert.ToInt32(summaryData["StageStatus"]);


                _crmService.SaveCRM_Account(accountfranchise);
                franchiseAccountDetail = _crmService.SaveCRM_AccountFranchiseDetail(franchiseAccountDetail);

                var data = new
                {
                    success = true,
                    result = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(franchiseAccountDetail)
                };
                return Json(data);
            }
            else
            {
                /* NLogger.Error("FranchiseAccountDetail does not exist");*/
                return Json(new { success = false, message = "FranchiseAccountDetail does not exist" });
            }
        }
        #endregion

        #region CRMLeadFranchiseController > Save Franchise Contact info
        [HttpPost]
        public ActionResult SaveFranchiseContactInfo(FormCollection contactInfoData)
        {
            if (contactInfoData == null)
            {
                //NLogger.Error("Requested SaveCustomerContactInfo is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            //NLogger.Debug($"CRM_AccountFranchiseDetailId={contactInfoData["CRM_AccountFranchiseDetailId"]}, " +
            //            $"CRM_AccountId={contactInfoData["CRM_AccountId"]}, " +
            //            $"FirstName={contactInfoData["FirstName"]}," +
            //            $"LastName={contactInfoData["LastName"]}, " +
            //            $"PhoneNumber={contactInfoData["PhoneNumber"]}, " +
            //            $"EmailAddress={contactInfoData["EmailAddress"]}," +
            //            $"franchisename={contactInfoData["franchisename"]}, " +
            //            $"Position={contactInfoData["Position"]}, " +
            //            $"JobType={contactInfoData["JobType"]}," +
            //            $"Address1={contactInfoData["Address1"]}, " +
            //            $"Address2={contactInfoData["Address2"]}, " +
            //            $"City={contactInfoData["City"]}," +
            //            $"County={contactInfoData["County"]}, " +
            //            $"State={contactInfoData["State"]}, " +
            //            $"HomePhoneNumber={contactInfoData["HomePhoneNumber"]}," +
            //            $"FaxNo={contactInfoData["FaxNo"]}," +
            //            $"ZipCode={contactInfoData["ZipCode"]},");

            var accountfranchise = _crmService.GetCRM_AccountbyId(Convert.ToInt32(contactInfoData["CRM_AccountId"]));
            var franchiseAccountDetail = _crmService.GetCRM_AccountFranchiseDetailbyId(Convert.ToInt32(contactInfoData["CRM_AccountFranchiseDetailId"]));
            if (accountfranchise != null && franchiseAccountDetail != null)
            {
                accountfranchise.ContactName = contactInfoData["FirstName"];
                //accountfranchise.FirstName = contactInfoData["FirstName"];
                //accountfranchise.LastName = contactInfoData["LastName"];
                accountfranchise.PhoneNumber = contactInfoData["PhoneNumber"];
                accountfranchise.EmailAddress = contactInfoData["EmailAddress"];

                franchiseAccountDetail.FranchiseeName = contactInfoData["franchisename"];
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

                /*FranchiseAccountDetail into FranchiseAccountDetailViewModel PartialView String*/
                ViewBag.State = new SelectList(CustomerService.GetStateList(), "abbr", "Name", franchiseAccountDetail.State);
                var types = from CRMEnums.JopType t in Enum.GetValues(typeof(CRMEnums.JopType))
                            select new { Id = (int)t, name = t.ToString() };
                ViewBag.JobType = new SelectList(types, "Id", "name", franchiseAccountDetail.JkFull);
                var accountfranchisevm = CRMAutoMapper.CrmAccountFranchiseDetailEntityToViewModel(accountFranchise);
                accountfranchisevm.ContactName = account.ContactName;
                accountfranchisevm.PhoneNumber = account.PhoneNumber;
                accountfranchisevm.EmailAddress = account.EmailAddress;
                var franchiseaccountreferenceinfo = CRMUtils.RenderPartialViewToString(this.ControllerContext, "_CRMFranchiseAccountReferenceInformation", accountfranchisevm);


                return Json(new
                {
                    success = V,
                    franchiseaccountreferenceinfo = franchiseaccountreferenceinfo
                });

            }

            return Json(new
            {

                success = V,
                result = ""

            });
        }
        #endregion

        #region CRMLeadFranchiseController > SearchFranchiseLead
        [HttpPost]
        public ActionResult SearchFranchiseLead(CRMSearchModel postData)
        {
            var result = _crmService.CRM_LeadFranchiseSearch(postData);

            /* Check Search Result count */
            if (result.Count > 0)
            {
                /* Make NULL the Search List */
                Session["SearchList"] = null;
                /*Put Search Result Lead Ids into SearchList*/
                Session["SearchList"] = result.OrderBy(x => x.CRM_AccountId).Select(x => x.CRM_AccountId);
            }
            return Json(new
            {
                success = V,
                aadata = result
            });
        }

        #endregion

        #region CRMLeadFranchiseController > Get NextOrPreviousFranchise Lead
        /// <summary>
        /// Method to Get Next Or Previous Franchise Lead on param condition
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNextOrPrevious(int Id, bool isNext)
        {
            var franchiseLeadId = 0;
            if (Session["SearchList"] as List<int> == null)
            {
                var data = new
                {
                    success = false,
                    Id = franchiseLeadId
                };
                return Json(data, behavior: JsonRequestBehavior.AllowGet);
            }

            if (Convert.ToBoolean(isNext))
            {
                /*Get Next Recored Id */
                franchiseLeadId = (Session["SearchList"] as List<int>).SkipWhile(x => x != Id).Skip(1).FirstOrDefault();
            }
            else
            {
                franchiseLeadId = (Session["SearchList"] as List<int>).TakeWhile(x => x != Id).LastOrDefault();
            }

            return Json(new
            {
                success = V,
                Id = franchiseLeadId
            }, behavior: JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CRMLeadFranchiseController > Get Schedules For Specific Region

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
                success = V,
                schedule = accountCustomerScheduleViewModels,
                result = accountCustomerScheduleCalendar,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadFranchiseController > Validate Schedule Availability
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

            var isFree = V;

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



            return Json(new
            {
                success = V,
                result = isFree
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CRMLeadCustomerController > Upload Excel

        [HttpPost]
        public ActionResult UploadExcel(CRMAccountCustomerDetailViewModel import, HttpPostedFileBase FileUpload)
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
                    var accountCustomer = _crmService.ImportFranchiseExcelData(dt);
                    string pathToExcelFile = severFilePath + filename;
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    //return Json("success", JsonRequestBehavior. AllowGet);
                    _hasNewData = V;
                    return RedirectToAction("Index");
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
            else if (Path.GetExtension(pathToExcelFile) == ".XLS")
                workbook = new XSSFWorkbook(file);

            ISheet sheet = workbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                if (row == null)
                {
                    break;
                }
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        #endregion

        #region ::Check Franchisee Name Exits ::
        public ActionResult CheckFranchiseenameExits(string CompanyName, string Phone)
        {
            //int IsExits = CustomerService.CheckCustomerNamePhoneIsExist(CompanyName, Phone);
            //return Json(new { IsExits = IsExits, success = true }, JsonRequestBehavior.AllowGet);  
            int AccId = CustomerService.CheckFranchiseeNamePhoneIsExist(CompanyName, Phone);
            return Json(new { AccId = AccId, success = V }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion
    }
}