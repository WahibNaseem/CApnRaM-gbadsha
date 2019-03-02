using Application.Web.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.Service.Inspection;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Inspection;
using JKViewModels;
using JKViewModels.Customer;
using JKViewModels.Inspection;
using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKViewModels.Common;
using Newtonsoft.Json;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    public class CustomerServiceController : ViewControllerBase
    {

        private ITemplateService _templateService;
        public CustomerServiceController(ICommonService commonService, ICustomerService customerService, IAccountReceivableService _AccountReceivableService, ICacheProvider cacheProvider, JKApi.Service.ServiceContract.Franchisee.IFranchiseeService _FranchiseeService, IInspectionService inspectionService, ICRM_Service CRM_Service, ITemplateService templateService, IUserService userService)
        {
            _commonService = commonService;
            CustomerService = customerService;
            AccountReceivableService = _AccountReceivableService;
            _cacheProvider = cacheProvider;
            FranchiseeService = _FranchiseeService;
            _inspectionService = inspectionService;
            _crmService = CRM_Service;
            _templateService = templateService;
            _userService = userService;
            ViewBag.HMenu = "CustomerService";
        }

        // GET: Portal/CustomerService
        public ActionResult Index()
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CustomerService", new { area = "Portal" }), "CustomerService");
            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            model.CustomerServiceQuickActionModel = CustomerService.GetCustomerServiceQuickAction(SelectedRegionId);



            //ViewBag.CallBackCount = _QuickActionLink.CallBackCount;
            //ViewBag.ComplaintCount = _QuickActionLink.ComplaintCount;
            //ViewBag.FailedInspectionCount = _QuickActionLink.FailedInspectionCount;
            //ViewBag.NewAccountCount = _QuickActionLink.NewAccountCount;
            //ViewBag.PendingCancellationCount = _QuickActionLink.PendingCancellationCount;
            //ViewBag.TransferCount = _QuickActionLink.TransferCount;



            return View(model);
        }

        public ActionResult CustomerCancellationPending()
        {
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 65);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult CustomerCancellationPendingDetails(int Id, int SerCallId = 0)
        {
            ViewBag.CustomerId = Id;
            ViewBag.ServiceCallLogId = SerCallId;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(Id));

            var _dataModel = CustomerService.GetServiceCallLogById(SerCallId);
            string strCallDate = string.Empty;
            string strStatusReason = string.Empty;
            if (_dataModel != null)
            {
                strCallDate = (_dataModel.CallDate != null ? Convert.ToDateTime(_dataModel.CallDate).ToString("dd-MM-yyyy") : string.Empty);
                var lstStatusReasonList = CustomerService.GetStatusReasonList();
                lstStatusReasonList = lstStatusReasonList.Where(w => w.StatusReasonListId == _dataModel.StatusReasonListId);
                strStatusReason = ((lstStatusReasonList != null && lstStatusReasonList.Count() > 0) ? lstStatusReasonList.FirstOrDefault().Name : string.Empty);
            }
            ViewBag.CallDate = strCallDate;
            ViewBag.StatusReason = strStatusReason;

            string AccountType = string.Empty;
            string ContractType = string.Empty;
            string AgreementType = string.Empty;
            string BillingFrequencyName = string.Empty;
            string CleanFrequencyName = string.Empty;
            string ServiceTypeName = string.Empty;
            if (CustomerViewModel.Contract != null)
            {
                if (CustomerViewModel.Contract.AccountTypeListId > 0)
                {
                    var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
                    AccountType = AccountTypeList.Where(w => w.AccountTypeListId == CustomerViewModel.Contract.AccountTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.AgreementTypeListId > 0)
                {
                    var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
                    AgreementType = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.ContractTypeListId > 0)
                {
                    var ContractTypeList = CustomerService.GetContractTypeList().ToList();
                    ContractType = ContractTypeList.Where(w => w.ContractTypeListId == CustomerViewModel.Contract.ContractTypeListId).FirstOrDefault().Name;
                }
            }
            if (CustomerViewModel.ContractDetail != null)
            {
                if (CustomerViewModel.ContractDetail.BillingFrequencyListId > 0)
                {
                    var FrequencyList = CustomerService.GetFrequencyList().ToList();
                    BillingFrequencyName = FrequencyList.Where(w => w.FrequencyListId == CustomerViewModel.ContractDetail.BillingFrequencyListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                {
                    var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                    CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                }

                if (CustomerViewModel.ContractDetail.ServiceTypeListId > 0)
                {
                    var ServiceTypeListList = CustomerService.GetServiceTypeList().ToList();
                    ServiceTypeName = ServiceTypeListList.Where(w => w.ServiceTypeListid == CustomerViewModel.ContractDetail.ServiceTypeListId).FirstOrDefault().name;
                }

            }
            ViewBag.AccountType = AccountType;
            ViewBag.AgreementType = AgreementType;
            ViewBag.ContractType = ContractType;
            ViewBag.BillingFrequencyName = BillingFrequencyName;
            ViewBag.CleanFrequencyName = CleanFrequencyName;
            ViewBag.ServiceTypeName = ServiceTypeName;

            var DataModel = CustomerService.GetFranchiseeDistributionWithCustomer(Id);
            if (DataModel != null && DataModel.Count() > 0)
            {
                ViewBag.FranchiseeDistribution = DataModel;
            }
            else
            {
                ViewBag.FranchiseeDistribution = null;
            }

            return View(CustomerViewModel);
        }
        public ActionResult CustomerCancellationPendingStageData(int Id)
        {
            int ActiveStageId = 0;
            var ActivityData = CustomerService.GetCSActivityByClassId(Id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending));

            if (ActivityData == null || ActivityData.Count() == 0)
            {
                int FTStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.NotifytheFranchiseeOwne);
                var ItemList = CustomerService.ValidationItemListStatus(FTStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending));
                ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });

                //Default stage
                ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.NotifytheFranchiseeOwne);

                ViewBag.ActiveStageId = ActiveStageId;
                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("_CustomerCancellationStageActivity", null);
                return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var StageId = ActivityData.Max(w => w.StatusListId);

                var StageActivityData = ActivityData.Where(w => w.StatusListId == StageId);
                if (StageActivityData != null && StageActivityData.Count() > 0)
                {
                    var ItemListstage = CustomerService.ValidationItemListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending));

                    int StagItemCount = 0;
                    if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ContacttheCustomer || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.LetertotheCustomer) // stage: 1,2,5
                    {
                        StagItemCount = 4;
                    }
                    else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.InspecttheAccount || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.DefectnotifytoFranchisee || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack) // stage: 3,4,6,7
                    {
                        StagItemCount = 3;
                    }
                    else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.NotifytheFranchiseeOwne)
                    {
                        StagItemCount = 5;
                    }

                    //if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != 4)                    
                    if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != StagItemCount)
                    {
                        ActiveStageId = Convert.ToInt32(StageId);
                        List<CustomerCancellationStageModel> ListDataModel = new List<CustomerCancellationStageModel>();
                        foreach (var itm in ItemListstage)
                        {
                            var chkIsItemChecked = StageActivityData.Where(g => g.ValidationItemId == itm.ValidationItemId && g.IsItemChecked == true);
                            CustomerCancellationStageModel ItmModel = new CustomerCancellationStageModel();
                            ItmModel.ValidationItemId = itm.ValidationItemId;
                            ItmModel.Name = itm.Name;
                            ItmModel.StatusListID = itm.StatusListID;
                            ItmModel.Selected = (chkIsItemChecked.Count() > 0 ? true : false);
                            ListDataModel.Add(ItmModel);
                        }
                        ViewBag.ItemList = ListDataModel;

                        ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending), Id);
                        ViewBag.CSstageModel = (CSstagemodal != null ? CSstagemodal.FirstOrDefault() : null);

                        ViewBag.ActiveStageId = ActiveStageId;
                        string HTMLContent = string.Empty;
                        HTMLContent += RenderPartialViewToString("_CustomerCancellationStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.NotifytheFranchiseeOwne))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ContacttheCustomer);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ContacttheCustomer))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.InspecttheAccount);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.InspecttheAccount))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.DefectnotifytoFranchisee);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.DefectnotifytoFranchisee))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.LetertotheCustomer);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.LetertotheCustomer))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack);

                            var ItemList7 = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending));
                            ViewBag.ItemList = ItemList7.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = true, StatusListID = s.StatusListID });


                            //stage note
                            var CSstagemodal7 = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending), Id);
                            ViewBag.CSstageModel = (CSstagemodal7 != null ? CSstagemodal7.FirstOrDefault() : null);

                            ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                            string HTMLContent7 = string.Empty;
                            ViewBag.ActiveStageId = ActiveStageId;
                            HTMLContent7 += RenderPartialViewToString("_CustomerCancellationStageActivity", null);
                            return Json(new { Data = HTMLContent7, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                        }


                        var ItemList = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending));
                        ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });
                        string HTMLContent = string.Empty;

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CancellationPending), Id);
                        if (CSstagemodal != null && CSstagemodal.Count() > 0)
                        {
                            ViewBag.CSstageModel = CSstagemodal.FirstOrDefault();
                        }

                        ViewBag.ActiveStageId = ActiveStageId;
                        HTMLContent += RenderPartialViewToString("_CustomerCancellationStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            //string HTMLContent = string.Empty;            
            //HTMLContent += RenderPartialViewToString("_CustomerCancellationStageActivity", null);
            return Json(new { Data = "", ActiveStageId = "-1" }, JsonRequestBehavior.AllowGet);

        }
        protected string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            ViewBag.DomainUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority + "/";

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        //stage 1
        public ActionResult SaveCSActivityNotifytheFranchiseeOwneData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityNotifytheFranchiseeOwneData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3, optitem4, optitem5);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 2
        public ActionResult SaveCSActivityContacttheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityContacttheCustomerData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3Date, optitem3Time, optitem4, optitem3EndTime);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 3
        public ActionResult SaveCSActivityInspecttheAccountData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, string optitem2, int optitem3, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityInspecttheAccountData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 4
        public ActionResult SaveCSActivityDefectnotifytoFranchiseeData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem2Note, int optitem3, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityDefectnotifytoFranchiseeData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem2Note, optitem3);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 5
        public ActionResult SaveCSActivityLetertotheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, string optitem1note, int optitem2, int optitem3, string optitem4Date, string optitem4Time, string optitem4endTime, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityLetertotheCustomerData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem1note, optitem2, optitem3, optitem4Date, optitem4Time, optitem4endTime);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 6
        public ActionResult SaveCSActivityReInspecttherAccountData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, string optitem1Note, int optitem2, int optitem3, string optitem3Note, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityReInspecttherAccountData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem1Note, optitem2, optitem3, optitem3Note);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 7
        public ActionResult SaveCSActivityFollowupBackontrackData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem2Note, int optitem3, string valIds = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                Id = CustomerService.SaveCSActivityFollowupBackontrackData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem2Note, optitem3);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchCustomer()
        {
            var regionlist = _commonService.GetRegionList();
            ViewBag.SelectedRegionId = SelectedRegionId;
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");

            Session["SearchCustomerIds"] = null;
            Session["SearchResultURL"] = null;
            Session["callFrom"] = 0;
            return View();
        }
        public ActionResult SearchCustomerList(string s = "", decimal ato = 0, decimal afrm = 0, decimal sto = 0, decimal sfrm = 0, string ord = "", int status = 0, string regionIds = "", int ctypeId = 0)
        {
            Session["SearchCustomerIds"] = null;
            Session["SearchResultURL"] = s + "," + ato + "," + afrm + "," + sto + "," + sfrm + "," + ord + "," + status + "," + regionIds + "," + ctypeId;
            Session["callFrom"] = 0;
            TempData["SearchRegionsId"] = regionIds;

            ViewBag.CurrentMenu = "SearchCustomer";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("SearchCustomer", "CustomerService", new { area = "Portal" }), "SearchCustomer");
            BreadCrumb.Add(Url.Action("SearchCustomer", "CustomerService", new { area = "Portal" }), "Search Customer List");

            ViewBag.search = s;
            ViewBag.amountTo = ato;
            ViewBag.amountFrom = afrm;
            ViewBag.sqrFtTo = sto;
            ViewBag.sqrFtFrom = sfrm;
            ViewBag.status = status;
            ViewBag.ord = ord;
            ViewBag.ctypeId = ctypeId;

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            if (regionIds != "")
            {
                IEnumerable<string> selectedRegions = regionIds.Split(',');
                ViewBag.regionlist = new MultiSelectList(regionlist, "RegionId", "Name", selectedRegions);
            }
            else
            {
                ViewBag.regionlist = new MultiSelectList(regionlist, "RegionId", "Name");
            }
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;

            var modelData = CustomerService.GetCustomerSearchResultList(SelectedRegionId, s, ato, afrm, sto, sfrm, ord, status, regionIds, ctypeId);

            return View(modelData);
        }
        public ActionResult BacktoSearchResult()
        {
            if (Session["callFrom"] != null && Convert.ToInt32(Session["callFrom"]) == 1)
            {
                return RedirectToAction("ServiceCallBackList", "Customer", new { area = "Portal" });
            }
            var item = Session["SearchResultURL"]?.ToString().Split(',');
            string s = item[0];
            string ato = item[1];
            string afrm = item[2];
            string sto = item[3];
            string sfrm = item[4];
            string ord = item[5];
            string status = item[6];
            string ctypeId = item[8];
            string regionsIds = TempData["SearchRegionsId"]?.ToString();
            return RedirectToAction("SearchCustomerList", "CustomerService", new { s = s, ato = ato, afrm = afrm, sto = sto, sfrm = sfrm, ord = ord, status = status, regionIds = regionsIds, ctypeId = ctypeId });

        }

        public ActionResult SearchCustomerDetails(int CustID, int step = 0, int RId = 0, string startDate = "", string endDate = "", string timeDropDown = "7", int colidx = 0, string sort = "", int? callFromSchedule = 0, string stflt = "")
        {

            //(a.Start.Date >= startDate.Date && a.Start.Date <= endDate)

            if (startDate == "")
            {
                var sd = new DateTime(DateTime.Now.Year, 1, 1);
                var ed = new DateTime(DateTime.Now.Year, 12, 31);

                startDate = sd.ToString();
                endDate = ed.ToString();
            }

            ViewBag.timeDropDown = timeDropDown;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.CallFromSchedule = callFromSchedule;
            ViewBag.stflt = stflt;
            #region :: Next/Prev ::

            bool PrevBtn = true;
            bool NextBtn = true;

            var ResultDataList = (List<SearchCustomerListId>)Session["SearchCustomerIds"];
            if (colidx != 0 && sort != "")
            {
                #region 
                // Note: Column Index 2: Customer No && Column Index 2: Customer Name && Column Index 3: Contract Amount && Column Index 6: Franchisee No && Column Index 7: Franchisee Name                 
                if (colidx == 1)
                {
                    #region Customer No sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.CustomerNo).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.CustomerNo).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    #endregion
                }
                if (colidx == 2)
                {
                    #region Customer Name sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.CustomerName).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.CustomerName).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    #endregion
                }
                else if (colidx == 3)
                {
                    #region Contract Amount sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.ContractAmount).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.ContractAmount).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    #endregion
                }
                else if (colidx == 6)
                {
                    #region Franchisee No sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.FranchiseeNo).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.FranchiseeNo).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }

                    #endregion
                }
                else if (colidx == 7)
                {
                    #region Franchisee Name sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.FranchiseeName).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.FranchiseeName).ToList();
                        List<JKViewModels.Customer.SearchCustomerListId> ListItemData = new List<JKViewModels.Customer.SearchCustomerListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchCustomerListId itm = new JKViewModels.Customer.SearchCustomerListId();
                            itm.Id = n;
                            itm.CustomerId = item.CustomerId;
                            itm.CustomerNo = item.CustomerNo;
                            itm.CustomerName = item.CustomerName;
                            itm.ContractAmount = item.ContractAmount;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    #endregion
                }
                #endregion
            }

            int RecordId = 0;
            if (RId == 0)
            {
                if (ResultDataList == null || ResultDataList.Count() == 0 || ResultDataList.Count() == 1)
                {
                    PrevBtn = false;
                    NextBtn = false;
                }
                var CustId = ResultDataList.Where(w => w.CustomerId == CustID);
                if (CustId != null && CustId.Count() > 0)
                {
                    RecordId = CustId.FirstOrDefault().Id;
                }
            }
            else
            {
                RecordId = RId;
            }
            ViewBag.RecordId = RecordId;

            if (step != 0)
            {
                if (RecordId == 1)
                {
                    PrevBtn = false;
                }
                if (RecordId == ResultDataList.Count())
                {
                    NextBtn = false;
                }
                var CustData = ResultDataList.Where(w => w.Id == RecordId);
                if (CustData != null && CustData.Count() > 0)
                {
                    CustID = CustData.FirstOrDefault().CustomerId;
                }
            }
            ViewBag.PrevBtn = PrevBtn;
            ViewBag.NextBtn = NextBtn;
            ViewBag.colidx = colidx;
            ViewBag.sort = sort;

            #endregion

            #region :: Customer Info ::

            ViewBag.CustomerID = CustID;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(CustID));
            //Customer Invoice 
            ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(CustID);

            //Customer Distribution
            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(CustID));
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
                ViewBag.FranchiseeInfo = FranchiseeService.GetFranchiseeBasicInfo(Convert.ToInt32(CustomerDistributions[0].FranchiseeId));
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }


            string CleanFrequencyName = string.Empty;
            int _notesDetailId = 0;
            string _notes = string.Empty;

            if (CustomerViewModel != null)
            {
                if (CustomerViewModel.ContractDetail != null)
                {
                    if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                    {
                        var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                        CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                    }
                }
                var _notemodel = CustomerService.GetCustomerNotes(Convert.ToInt32(CustID), SelectedRegionId, (int)JKApi.Business.Enumeration.TypeList.Customer);
                if (_notemodel != null)
                {
                    _notesDetailId = Convert.ToInt32(_notemodel.FirstOrDefault().Key);
                    _notes = Convert.ToString(_notemodel.FirstOrDefault().Value);
                }
            }
            ViewBag.CleanFrequencyName = CleanFrequencyName;
            ViewBag.custNotes = _notes;
            ViewBag.notesDetailId = _notesDetailId;
            string ContractTypeName = string.Empty;
            if (CustomerViewModel != null)
            {
                if (CustomerViewModel.CustomerViewModel != null)
                {
                    if (CustomerViewModel.CustomerViewModel.ContractTypeListId > 0)
                    {
                        var ContractTypeList = CustomerService.GetContractTypeList().ToList();
                        ContractTypeName = ContractTypeList.Where(w => w.ContractTypeListId == CustomerViewModel.CustomerViewModel.ContractTypeListId).FirstOrDefault().Name;
                    }
                }
            }
            ViewBag.ContractTypeName = ContractTypeName;


            #region log message

            if (CustomerViewModel != null && CustomerViewModel.CustomerViewModel != null && CustomerViewModel.CustomerViewModel.LogId > 0)
            {
                var LogDetails = jkEntityModel.LogMsgs.Where(x => x.LogMsgID == CustomerViewModel.CustomerViewModel.LogId).FirstOrDefault();
                if (LogDetails != null)
                {
                    ViewBag.LogMessage = LogDetails.Massaqe;
                }
            }

            #endregion

            #endregion

            #region :: Franchisee Info ::

            string FranchiseeId = "0";
            string FranchiseeNo = string.Empty;
            string FranchiseeName = string.Empty;
            var FranData = CustomerService.GetFrancisesBySearchCustomerId(CustID);
            if (FranData != null && FranData.Count() > 0)
            {
                string[] arrFran = FranData.FirstOrDefault().Key.Split('-');
                FranchiseeId = (arrFran[0] != "" ? Convert.ToString(arrFran[0]) : "0");
                FranchiseeNo = (arrFran[1] != "" ? Convert.ToString(arrFran[1]) : "");
                FranchiseeName = FranData.FirstOrDefault().Value;

            }
            ViewBag.FranchiseeId = FranchiseeId;
            ViewBag.FranchiseeNo = FranchiseeNo;
            ViewBag.FranchiseeName = FranchiseeName;

            #endregion

            #region :: Other  Info :: 

            // Call Log List
            //var LogResult = CustomerService.GetServiceCallLogCustomerSearchResultDetails(CustID, startDate, endDate);
            //int statusid = 0;
            //if (stflt != "")
            //{
            //    statusid = Convert.ToInt32(stflt);
            //}
            if (stflt == "null")
            {
                stflt = "";
            }
            var LogResult = CustomerService.GetServiceCallLogCustomersListForSearch(CustID, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), "", stflt);

            if (LogResult != null && LogResult.Count() > 0)
            {
                ViewBag.CustomerCallList = LogResult;
            }

            if (CustID > 0)
            {
                var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(CustID));
                if (BillSetting != null)
                {
                    ViewBag.ARStatus = ((BillSetting.ARStatus != null && BillSetting.ARStatus != 0) ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty); //BillSetting.ARStatus;
                }

                var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(CustID));
                if (AgingData != null)
                {
                    ViewBag.AgingData = AgingData;
                }

                //decimal ContractAmount = 0;
                //var ContractData = CustomerService.GetContractByCustomerId(Convert.ToInt32(CustID));
                //if (ContractData != null)
                //{
                //    ContractAmount = (ContractData.Amount.HasValue ? ContractData.Amount.Value : 0);
                //}
                //ViewBag.ContractAmount = ContractAmount;

            }

            #endregion
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");

            int CSTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CustomerServicesCallLogStatus);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == CSTypeList).ToList();
            if (stflt != "")
            {
                ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", stflt);
            }
            else
            {
                ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name");
            }
            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            ViewBag.userLista = userLista;

            return View(CustomerViewModel);
        }
        public List<ARStatusReasonList> getARStatusResonList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getARStatusResonList))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getARStatusResonList, CustomerService.GetARStatusReasonList().ToList());
                //_cacheProvider.Set(CacheKeyName.Customer_getARStatusList, CustomerService.getARStatusList());
            }
            return (List<ARStatusReasonList>)_cacheProvider.Get(CacheKeyName.Customer_getARStatusResonList);
        }
        private string PhoneNoformat(string phone)
        {
            if (phone.Length == 10)
            {
                phone = '(' + phone.Substring(0, 3) + ')' + ' ' + phone.Substring(3, 3) + '-' + phone.Substring(6, 4);
            }
            return phone;
        }
        [HttpGet]
        public ActionResult CustomerSearchCancallationPendingList(string status, string rgId)
        {
            try
            {
                int ServiceLogTypeListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation;
                var customers = CustomerService.GetCustomerSearchCancallationPendingListNew(status, rgId, ServiceLogTypeListId);
                var result = (from f in customers
                              select new
                              {
                                  f.ServiceCallLogId,
                                  f.CustomerId,
                                  f.CustomerNo,
                                  f.CustomerName,
                                  f.Address,
                                  f.StateName,
                                  f.City,
                                  f.PostalCode,
                                  //CustomerName = "CustomerName",
                                  //Address= "Address",
                                  //StateName= "StateName",
                                  //City= "City",
                                  //PostalCode= "PostalCode",
                                  Amount = string.Format("{0:c}", f.Amount),
                                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  f.FranchiseeNo,
                                  f.FranchiseeName,
                                  f.Reason,
                                  f.ComplaintsDate,
                                  f.FollowUpBy,
                                  f.Comments,
                                  f.LeftDay
                              }).ToList();

                return Json(new
                {
                    aadata = result,
                }, JsonRequestBehavior.AllowGet);

                /* var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                 var customers = CustomerService.GetCustomerSearchCancallationPendingList(status, rgId);
                 var result = (from f in customers
                               select new
                               {
                                   f.CustomerId,
                                   f.CustomerNo,
                                   f.CustomerName,
                                   f.Address,
                                   f.StateName,
                                   f.City,
                                   f.PostalCode,
                                   //CustomerName = "CustomerName",
                                   //Address= "Address",
                                   //StateName= "StateName",
                                   //City= "City",
                                   //PostalCode= "PostalCode",
                                   Amount = string.Format("{0:c}", f.Amount),
                                   Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                   f.RegionName,
                                   StatusName = (f.StatusName ?? "").Trim(),
                                   AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                   f.FranchiseeNo,
                                   f.FranchiseeName,
                                   f.Reason,
                                   f.EffectiveDate,
                                   f.LeftDay
                               }).ToList();

                 return Json(new
                 {
                     aadata = result,
                 }, JsonRequestBehavior.AllowGet); */
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowActivityDetail(int Id)
        {
            List<CustomerCancellationActivityModel> ListData = new List<CustomerCancellationActivityModel>();
            ListData = CustomerService.GetCustomerCancellationActivityDetails(Id);

            var CustStageListModel = CustomerService.CSstageListWithCustomerWise((int)JKApi.Business.Enumeration.TypeList.CancellationPending, Id).ToList();

            ViewBag.StageListCustomerWise = CustStageListModel;
            string HTMLContent = string.Empty;
            HTMLContent += RenderPartialViewToString("_CustomerCancellationActivityDetailPopup", ListData);
            return Json(new { Data = HTMLContent }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDistributionFeesDetail(int Id)
        {
            List<DistributionFeesDetailModel> ListData = new List<DistributionFeesDetailModel>();
            ListData = CustomerService.GetDistributionFeesDetail(Id);

            string HTMLContent = string.Empty;
            HTMLContent += RenderPartialViewToString("_CustomerDistributionFeesDetailPopup", ListData);
            return Json(new { Data = HTMLContent }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchCustomerCancellationRequestPopup()
        {
            return PartialView("_SearchCustomerCancellationRequestPopup");
        }
        public ActionResult CustomerCancellationRequestPopup(int Customerid = 0)
        {
            ViewBag.CustomerId = Customerid;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(Customerid));

            return PartialView("_CustomerCancellationRequestPopup", CustomerViewModel);
        }

        //Customer Controller call History
        [HttpGet]
        public ActionResult InspectionHistoryCustomer(int? id)
        {
            ViewBag.HMenu = "Customer";
            ViewBag.CurrentMenu = "CustomerService";

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Inspection History");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            int CustID = id ?? -1;
            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            ViewBag.CustId = id;
            if (CustID > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.ContactName != null)
                    {
                        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    }
                    if (a.ContactTitle != null)
                    {
                        customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                    }
                    if (a.Cell != null)
                    {
                        customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.Ext != null)
                    {
                        customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                    }

                    if (a.Amount != null)
                    {
                        if (a.Amount.ToString().Trim().Length > 0)
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    else
                    {
                        customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                    }
                }
                ViewBag.CustomerDetail = customerDetailViewModel;
                TempData["CustomerDetail"] = customerDetailViewModel;
                //FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }

            if (CustID > 0)
            {
                // TODO: Use GetInspectionFormHistoryListByCustomer
                var inspectionViewModel = _inspectionService.GetInspectionFormHistoryListByCustomer(new ViewInspectionFormListModel
                {
                    CustomerId = CustID,
                    IsEnable = true
                });

                var inspectionList = new List<InspectionFormModel>();
                foreach (var inspection in inspectionViewModel.InspectionFormList)
                {
                    inspectionList.Add(inspection);
                }
                return View(inspectionList);
            }
            return View("InspectionHistory", new ViewInspectionFormListModel().InspectionFormList);
        }

        [HttpGet]
        public ActionResult InspectionHistoryList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Inspection History");
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            return View();
        }

        public ActionResult inspectionHistoryListResultData(string date = "")
        {
            DateTime sDate;
            DateTime eDate;
            try
            {
                List<InspectionFormHistoryModel> _InspectionFormHistoryModel = new List<InspectionFormHistoryModel>();
                if (!string.IsNullOrEmpty(date) && date.Contains("-") &&
                   DateTime.TryParseExact(date.Split('-')[0], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                       DateTimeStyles.None, out sDate) &&
                   DateTime.TryParseExact(date.Split('-')[1], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                       DateTimeStyles.None, out eDate) && eDate > sDate)
                {
                    IEnumerable<int> response = _inspectionService.GetInspectionFormHistoryList().Where(o => o.RegionId == SelectedRegionId && o.RecordedDate >= sDate && o.RecordedDate <= eDate).Select(x => x.CustomerId);

                    foreach (int item in response)
                    {
                        var data = _inspectionService.GetConsolidatedInpsectionFormHistoryByCustomer(item);
                        _InspectionFormHistoryModel.Add(data);

                    }
                }


                return Json(new
                {
                    aadata = _InspectionFormHistoryModel,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult InspectionHistory(int? id)
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Customer", new { area = "Portal" }), "Inspection History");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            int CustID = id ?? -1;
            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            ViewBag.CustId = id;
            if (CustID > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.ContactName != null)
                    {
                        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    }
                    if (a.ContactTitle != null)
                    {
                        customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                    }
                    if (a.Cell != null)
                    {
                        customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.Ext != null)
                    {
                        customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                    }

                    if (a.Amount != null)
                    {
                        if (a.Amount.ToString().Trim().Length > 0)
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    else
                    {
                        customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                    }
                }
                ViewBag.CustomerDetail = customerDetailViewModel;
                TempData["CustomerDetail"] = customerDetailViewModel;
                //FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }

            if (CustID > 0)
            {
                // TODO: Use GetInspectionFormHistoryListByCustomer
                var inspectionViewModel = _inspectionService.GetInspectionFormHistoryListByCustomer(new ViewInspectionFormListModel
                {
                    CustomerId = CustID,
                    IsEnable = true
                });

                var inspectionList = new List<InspectionFormModel>();
                foreach (var inspection in inspectionViewModel.InspectionFormList)
                {
                    inspectionList.Add(inspection);
                }
                return View(inspectionList);
            }
            return View(new ViewInspectionFormListModel().InspectionFormList);
        }

        [HttpGet]
        public ActionResult InspectionReport(int Id, int CustId)
        {
            // TODO: Implement GetInspectionFormHistory
            //            var data = _inspectionService.GetInspectionFormReport(Id);
            var data = _inspectionService.GetDetailInspectionFormHistory(Id);
            //var data = new InspectionFormModel();
            if (data != null)
            {
                ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
                int CustID = CustId;
                int? CustomerID = CustID;
                ViewBag.CustomerID = CustId;
                ViewBag.CustId = CustId;
                if (CustID > 0)
                {
                    var response = jkEntityModel.portal_spGet_CustomerDetail(CustId);

                    CustomerDetailViewModel customerDetailViewModel = null;
                    foreach (var a in response.ToList())
                    {
                        customerDetailViewModel = new CustomerDetailViewModel();

                        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                        customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                        customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                        if (a.Phone != null)
                        {
                            customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                        }

                        if (a.Fax != null)
                        {
                            customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                        }
                        if (a.CustomerNo != null)
                        {
                            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.ContactName != null)
                        {
                            customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                        }
                        if (a.ContactTitle != null)
                        {
                            customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                        }
                        if (a.Cell != null)
                        {
                            customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.Ext != null)
                        {
                            customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                        }

                        if (a.Amount != null)
                        {
                            if (a.Amount.ToString().Trim().Length > 0)
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                            }
                            else
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                            }
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    ViewBag.CustomerDetail = customerDetailViewModel;
                    TempData["CustomerDetail"] = customerDetailViewModel;
                    //FullCustomerViewModel = Maintennacepopups(CustID);

                    FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                }
            }
            return View(data);
        }

        public ActionResult InspectionFormList(int? id)
        {
            ViewBag.CurrentMenu = "InspectionFormList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InspectionFormList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("InspectionFormList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("InspectionFormList", "Customer", new { area = "Portal" }), "Inspection Form");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            int CustID = id ?? -1;
            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            ViewBag.CustId = id;
            if (CustID > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.ContactName != null)
                    {
                        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    }
                    if (a.ContactTitle != null)
                    {
                        customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                    }
                    if (a.Cell != null)
                    {
                        customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.Ext != null)
                    {
                        customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                    }

                    if (a.Amount != null)
                    {
                        if (a.Amount.ToString().Trim().Length > 0)
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    else
                    {
                        customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                    }
                }
                ViewBag.CustomerDetail = customerDetailViewModel;
                TempData["CustomerDetail"] = customerDetailViewModel;
                //FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }

            if (CustID > 0)
            {
                // TODO: Use GetInspectionFormListByCustomer();
                var inspectionViewModel = _inspectionService.GetInspectionFormListByCustomer(new ViewInspectionFormListModel
                {
                    CustomerId = CustID,
                    IsEnable = true
                });
                //var inspectionViewModel = new ViewInspectionListModel();

                var inspectionList = new List<InspectionFormModel>();
                foreach (var inspection in inspectionViewModel.InspectionFormList)
                {
                    inspectionList.Add(inspection);
                }
                return View(inspectionList);
            }
            return View(new ViewInspectionFormListModel().InspectionFormList);
        }

        [HttpGet]
        public ActionResult InspectionForm(int Id, int CustId)
        {
            ViewBag.CurrentMenu = "InspectionForm";
            ViewBag.Id = Id;
            var data = _inspectionService.GetDetailInspectionForm(Id);
            if (data != null)
            {
                ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
                int CustID = CustId;
                int? CustomerID = CustID;
                ViewBag.CustomerID = CustId;
                ViewBag.CustId = CustId;
                if (CustID > 0)
                {
                    var response = jkEntityModel.portal_spGet_CustomerDetail(CustId);

                    CustomerDetailViewModel customerDetailViewModel = null;
                    foreach (var a in response.ToList())
                    {
                        customerDetailViewModel = new CustomerDetailViewModel();
                        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                        customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                        customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                        if (a.Phone != null)
                        {
                            customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                        }

                        if (a.Fax != null)
                        {
                            customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                        }
                        if (a.CustomerNo != null)
                        {
                            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.ContactName != null)
                        {
                            customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                        }
                        if (a.ContactTitle != null)
                        {
                            customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                        }
                        if (a.Cell != null)
                        {
                            customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.Ext != null)
                        {
                            customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                        }

                        if (a.Amount != null)
                        {
                            if (a.Amount.ToString().Trim().Length > 0)
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                            }
                            else
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                            }
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    ViewBag.CustomerDetail = customerDetailViewModel;
                    TempData["CustomerDetail"] = customerDetailViewModel;
                    FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                }
            }
            var model = new TemplateAreaViewModel() { ActionType = "SA" };
            ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
            ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");
            return View(data);
        }

        public JsonResult InspectionFormSectionAdd(int id, int InsId)
        {
            var temArea = jkEntityModel.TemplateAreas.Where(x => x.TemplateAreaId == id).FirstOrDefault();
            var sections = _inspectionService.GetInspectionFormSectionListByForm(InsId);
            var order = sections.Count + 1;
            InspectionFormSectionModel InspectionFormSectionModel = new InspectionFormSectionModel
            {
                InspectionFormSectionId = 0,
                InspectionFormId = InsId,
                SectionOrder = order,
                SectionName = temArea.AreaName,
                SectionStatus = 0,
                ScorePercent = 0,
                PassPoints = 0,
                FailPoints = 0,
                NeedImprovementPoints = 0,
                SectionAutoFail = false,
                SectionAutoFailReason = "{\"Reasons\":[{\"Label\":\"Area does not meet standard\",\"Selected\":true},{\"Label\":\"Other reasons\",\"Selected\":false}]}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = SelectedUserId
            };
            var Data = _inspectionService.AddOrUpdateInspectionFormSection(InspectionFormSectionModel);
            var jsonResult = Json(new { aadata = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult InspectionFormItemtoSection(int _TemplateAreaId, int _TemplateAreaItemId)
        {
            var item = _templateService.GetTemplateAreaItem(_TemplateAreaItemId);
            var sections = _inspectionService.GetInspectionFormItemListBySection(_TemplateAreaId);
            int order = sections.Count + 1;
            var temp = new InspectionFormItemModel
            {
                InspectionFormItemId = _TemplateAreaItemId,
                InspectionFormSectionId = _TemplateAreaId,
                FormItemType = item.FormItemType,
                FormItemOrder = order,
                FormItemValue = item.FormItemValue,
                IsDirty = false,
                IsRequired = true,
                CreatedBy = SelectedUserId,
                CreatedDate = DateTime.Now,
                ModifiedBy = 0,
                ModifiedDate = DateTime.Now,
                IsDelete = false,
                IsEnable = true
            };
            var formTemplateViewModel = _inspectionService.AddOrUpdateInpsectionFormItem(temp);
            return Json(new { aadata = "Save" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInspectionSection(int secId)
        {
            _inspectionService.DeleteInspectionFormSection(secId);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInspectionSectionItem(int ItemId)
        {
            _inspectionService.DeleteInspectionFormItem(ItemId);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InspectionFormSave(InspectionFormModel _InspectionFormModel, FormCollection collection)
        {
            //Here pass all update model with Section
            if (_InspectionFormModel.InspectionFormId != 0)
            {
                _InspectionFormModel.InspectorId = int.Parse(_claimView.GetCLAIM_USERID());
                var user = _userService.GetUserDetail(int.Parse(_claimView.GetCLAIM_USERID()));
                _InspectionFormModel.InspectedBy = user.FirstName + " " + user.LastName;
                _inspectionService.AddOrUpdateInspectionForm(_InspectionFormModel);
                _inspectionService.CompleteInspectionForm(_InspectionFormModel);
            }
            return RedirectToAction("InspectionForm", "CustomerService", new { area = "Portal" });
        }

        public ActionResult InspectionFormItemUpdate(int inspectionFormItemId,int inspectionFormSectionId, int status)
        {
            var data = _inspectionService.GetInspectionFormItem(inspectionFormItemId);
            dynamic formItemValue = JsonConvert.DeserializeObject<dynamic>(data.FormItemValue);
            var label = formItemValue.Label;
            string _status = "0";
            if(status == 0)
            {
                _status = data.FormItemValue;
            }
            if(status == 1)
            {
                _status = $"{{\"Text\":\"\",\"Label\":\"{label}\",\"Status\":{status},\"Rating\":0,\"Items\":[]}}";
            }
            if (status == 2)
            {
                _status = $"{{\"Text\":\"\",\"Label\":\"{label}\",\"Status\":{status},\"Rating\":0,\"Items\":[]}}";
            }
            if (status == 3)
            {
                _status = $"{{\"Text\":\"\",\"Label\":\"{label}\",\"Status\":{status},\"Rating\":0,\"Items\":[]}}";
            }
            var TempModel = new InspectionFormItemModel {
                InspectionFormItemId = inspectionFormItemId,
                InspectionFormSectionId = inspectionFormSectionId,
                FormItemType = data.FormItemType,
                FormItemOrder = data.FormItemOrder,
                FormItemValue = _status,
                IsDirty = data.IsDirty,
                IsRequired = true,
                CreatedBy= (int.Parse(_claimView.GetCLAIM_USERID())),
                CreatedDate = DateTime.Now,
                ModifiedBy= (int.Parse(_claimView.GetCLAIM_USERID())),
                ModifiedDate =DateTime.Now
            };

            _inspectionService.AddOrUpdateInpsectionFormItem(TempModel);
            return Json(new { aadata = "Save" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult InspectionFormExportToPDF(int Id, int CustId)
        {
            var data = _inspectionService.GetDetailInspectionForm(Id);
            if (data != null)
            {
                ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
                int CustID = CustId;
                int? CustomerID = CustID;
                ViewBag.CustomerID = CustId;
                ViewBag.CustId = CustId;
                if (CustID > 0)
                {
                    var response = jkEntityModel.portal_spGet_CustomerDetail(CustId);

                    CustomerDetailViewModel customerDetailViewModel = null;
                    foreach (var a in response.ToList())
                    {
                        customerDetailViewModel = new CustomerDetailViewModel();

                        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                        customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                        customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                        if (a.Phone != null)
                        {
                            customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                        }

                        if (a.Fax != null)
                        {
                            customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                        }
                        if (a.CustomerNo != null)
                        {
                            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.ContactName != null)
                        {
                            customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                        }
                        if (a.ContactTitle != null)
                        {
                            customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                        }
                        if (a.Cell != null)
                        {
                            customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.Ext != null)
                        {
                            customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                        }

                        if (a.Amount != null)
                        {
                            if (a.Amount.ToString().Trim().Length > 0)
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                            }
                            else
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                            }
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    ViewBag.CustomerDetail = customerDetailViewModel;
                    TempData["CustomerDetail"] = customerDetailViewModel;
                    FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                }
            }

            string HTMLContent = string.Empty;
            HTMLContent += RenderPartialViewToString("_PartialInspectionFormExportToPDFResult", data);
            return File(GetPDFWithoutRotate(HTMLContent), "application/pdf", "_InspectionFormExportPDF.pdf");

        }

        public byte[] GetPDFWithoutRotate(string pHTML) //without Rotate
        {
            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                    {
                        document.Open();
                        using (var strReader = new StringReader(pHTML))
                        {
                            //Set factories
                            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                            //Set css
                            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                            cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
                            //Export
                            IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
                            var worker = new XMLWorker(pipeline, true);
                            var xmlParse = new XMLParser(true, worker);
                            xmlParse.Parse(strReader);
                            xmlParse.Flush();
                        }
                        document.Close();
                    }
                }
                bytesArray = ms.ToArray();
            }
            return bytesArray;
        }

        [HttpGet]
        public ActionResult ProcessInspectionForm(int Id, int CustId)
        {
            var data = _inspectionService.GetInspectionForm(Id);
            if (data != null)
            {
                ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
                int CustID = CustId;
                int? CustomerID = CustID;
                ViewBag.CustomerID = CustId;
                ViewBag.CustId = CustId;
                if (CustID > 0)
                {
                    var response = jkEntityModel.portal_spGet_CustomerDetail(CustId);

                    CustomerDetailViewModel customerDetailViewModel = null;
                    foreach (var a in response.ToList())
                    {
                        customerDetailViewModel = new CustomerDetailViewModel();
                        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                        customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                        customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                        if (a.Phone != null)
                        {
                            customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                        }

                        if (a.Fax != null)
                        {
                            customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                        }
                        if (a.CustomerNo != null)
                        {
                            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.ContactName != null)
                        {
                            customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                        }
                        if (a.ContactTitle != null)
                        {
                            customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                        }
                        if (a.Cell != null)
                        {
                            customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                        }
                        if (a.EmailAddress != null)
                        {
                            customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                        }
                        if (a.Ext != null)
                        {
                            customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                        }

                        if (a.Amount != null)
                        {
                            if (a.Amount.ToString().Trim().Length > 0)
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                            }
                            else
                            {
                                customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                            }
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    ViewBag.CustomerDetail = customerDetailViewModel;
                    TempData["CustomerDetail"] = customerDetailViewModel;
                    FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                }
            }
            return PartialView("_PartialInspectionFormProcess", data);
        }

        [HttpGet]
        public ActionResult ProcessInspectionFormItem(int Id)
        {
            var data = _inspectionService.GetInspectionFormItemListBySection(Id);

            return PartialView("_PartialInspectionFormItemsProcess", data);
        }

        #region :: Customer Complaint :: 

        [HttpGet]
        public ActionResult CustomerServiceComplainList()
        {
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage);
            //int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 71);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public ActionResult CustomerServiceComplainListResult(string status, string rgId)
        {
            try
            {
                int ServiceLogTypeListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint;
                //if (status != "")
                //{
                //    status = status + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                //}
                //string statusids = (int)JKApi.Business.Enumeration.CustomerStatusList.Open + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.InProcess + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.Closed + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.ReOpen;
                string statusids = (int)JKApi.Business.Enumeration.CustomerStatusList.Open + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.InProcess;
                var customers = CustomerService.GetCustomerServiceComplainListResult(statusids, rgId, ServiceLogTypeListId, status);
                var result = (from f in customers
                              select new
                              {
                                  f.CustomerId,
                                  f.CustomerNo,
                                  f.CustomerName,
                                  //f.Address,
                                  //f.StateName,
                                  //f.City,
                                  //f.PostalCode,
                                  //CustomerName = "CustomerName",
                                  //Address= "Address",
                                  //StateName= "StateName",
                                  //City= "City",
                                  //PostalCode= "PostalCode",
                                  Amount = string.Format("{0:c}", f.Amount),
                                  //Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  //AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  f.FranchiseeNo,
                                  f.FranchiseeName,
                                  //f.Reason,
                                  f.ComplaintsDate,
                                  f.FollowUpBy,
                                  f.Comments,
                                  f.ServiceCallLogId,
                                  f.StageStatusId,
                                  f.StageStatusName,
                                  f.CreatedBy,
                                  f.EmailNotesTo
                              }).ToList();

                return Json(new
                {
                    aadata = result,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CustomerComplainDetails(int Id, int SerCallId = 0, int callFromSchedule = 0)
        {
            ViewBag.CustomerId = Id;
            ViewBag.ServiceCallLogId = SerCallId;
            ViewBag.CallFromSchedule = callFromSchedule;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(Id));

            var _dataModel = CustomerService.GetServiceCallLogById(SerCallId);
            string strCallDate = string.Empty;
            string strStatusReason = string.Empty;

            if (_dataModel != null)
            {
                strCallDate = (_dataModel.CallDate != null ? Convert.ToDateTime(_dataModel.CallDate).ToString("dd-MM-yyyy") : string.Empty);
                var lstStatusReasonList = CustomerService.GetStatusReasonList();
                lstStatusReasonList = lstStatusReasonList.Where(w => w.StatusReasonListId == _dataModel.StatusReasonListId);
                strStatusReason = ((lstStatusReasonList != null && lstStatusReasonList.Count() > 0) ? lstStatusReasonList.FirstOrDefault().Name : string.Empty);

            }
            ViewBag.CallDate = strCallDate;
            ViewBag.StatusReason = strStatusReason;

            string AccountType = string.Empty;
            string ContractType = string.Empty;
            string AgreementType = string.Empty;
            string BillingFrequencyName = string.Empty;
            string CleanFrequencyName = string.Empty;
            string ServiceTypeName = string.Empty;
            if (CustomerViewModel.Contract != null)
            {
                if (CustomerViewModel.Contract.AccountTypeListId > 0)
                {
                    var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
                    AccountType = AccountTypeList.Where(w => w.AccountTypeListId == CustomerViewModel.Contract.AccountTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.AgreementTypeListId > 0)
                {
                    var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
                    AgreementType = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.ContractTypeListId > 0)
                {
                    var ContractTypeList = CustomerService.GetContractTypeList().ToList();
                    ContractType = ContractTypeList.Where(w => w.ContractTypeListId == CustomerViewModel.Contract.ContractTypeListId).FirstOrDefault().Name;
                }
            }
            if (CustomerViewModel.ContractDetail != null)
            {
                if (CustomerViewModel.ContractDetail.BillingFrequencyListId > 0)
                {
                    var FrequencyList = CustomerService.GetFrequencyList().ToList();
                    BillingFrequencyName = FrequencyList.Where(w => w.FrequencyListId == CustomerViewModel.ContractDetail.BillingFrequencyListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                {
                    var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                    CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                }

                if (CustomerViewModel.ContractDetail.ServiceTypeListId > 0)
                {
                    var ServiceTypeListList = CustomerService.GetServiceTypeList().ToList();
                    ServiceTypeName = ServiceTypeListList.Where(w => w.ServiceTypeListid == CustomerViewModel.ContractDetail.ServiceTypeListId).FirstOrDefault().name;
                }

            }
            ViewBag.AccountType = AccountType;
            ViewBag.AgreementType = AgreementType;
            ViewBag.ContractType = ContractType;
            ViewBag.BillingFrequencyName = BillingFrequencyName;
            ViewBag.CleanFrequencyName = CleanFrequencyName;
            ViewBag.ServiceTypeName = ServiceTypeName;

            var DataModel = CustomerService.GetFranchiseeDistributionWithCustomer(Id);
            if (DataModel != null && DataModel.Count() > 0)
            {
                ViewBag.FranchiseeDistribution = DataModel;
            }
            else
            {
                ViewBag.FranchiseeDistribution = null;
            }

            return View(CustomerViewModel);
        }

        public ActionResult CustomerServiceComplainStageData(int Id)
        {
            int ActiveStageId = 0;
            var ActivityData = CustomerService.GetCSActivityByClassId(Id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));

            if (ActivityData == null || ActivityData.Count() == 0)
            {
                int FTStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ComplaintLogged);
                var ItemList = CustomerService.ValidationItemListStatus(FTStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));
                ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });

                //Default stage
                ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ComplaintLogged);

                ViewBag.ActiveStageId = ActiveStageId;
                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
                return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var StageId = ActivityData.Max(w => w.StatusListId);

                var StageActivityData = ActivityData.Where(w => w.StatusListId == StageId);
                if (StageActivityData != null && StageActivityData.Count() > 0)
                {
                    var ItemListstage = CustomerService.ValidationItemListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));

                    int StagItemCount = 0;
                    //if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ComplaintLogged ) // stage: 1,2,5
                    //{
                    //    StagItemCount = 5;
                    //}
                    //else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.FollowUp || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack) // stage: 3,4,6,7
                    //{
                    //    StagItemCount = 3;
                    //}
                    //else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ActionsFollowUp)
                    //{
                    //    StagItemCount = 4;
                    //}

                    //if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != 4)                    
                    if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != StagItemCount)
                    {
                        ActiveStageId = Convert.ToInt32(StageId);
                        List<CustomerCancellationStageModel> ListDataModel = new List<CustomerCancellationStageModel>();
                        foreach (var itm in ItemListstage)
                        {
                            var chkIsItemChecked = StageActivityData.Where(g => g.ValidationItemId == itm.ValidationItemId && g.IsItemChecked == true);
                            CustomerCancellationStageModel ItmModel = new CustomerCancellationStageModel();
                            ItmModel.ValidationItemId = itm.ValidationItemId;
                            ItmModel.Name = itm.Name;
                            ItmModel.StatusListID = itm.StatusListID;
                            ItmModel.Selected = (chkIsItemChecked.Count() > 0 ? true : false);
                            ItmModel.ItemNote = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignNote : "");
                            ItmModel.StaticDesignScheduleDate = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignScheduleDate : null);
                            ItmModel.StaticDesignScheduleEndTime = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignScheduleEndTime : null);
                            ListDataModel.Add(ItmModel);
                        }
                        ViewBag.ItemList = ListDataModel;

                        ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage), Id);
                        ViewBag.CSstageModel = (CSstagemodal != null ? CSstagemodal.FirstOrDefault() : null);

                        ViewBag.ActiveStageId = ActiveStageId;
                        string HTMLContent = string.Empty;
                        HTMLContent += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ComplaintLogged))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ActionsFollowUp);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ActionsFollowUp))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowUp);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowUp))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed))
                        {
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.LetertotheCustomer))
                            //{
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount))
                            //{
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack))
                            //{
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed);

                            List<CustomerCancellationStageModel> ListDataModel = new List<CustomerCancellationStageModel>();
                            foreach (var itm in ItemListstage)
                            {
                                var chkIsItemChecked = StageActivityData.Where(g => g.ValidationItemId == itm.ValidationItemId && g.IsItemChecked == true);
                                CustomerCancellationStageModel ItmModel = new CustomerCancellationStageModel();
                                ItmModel.ValidationItemId = itm.ValidationItemId;
                                ItmModel.Name = itm.Name;
                                ItmModel.StatusListID = itm.StatusListID;
                                ItmModel.Selected = (chkIsItemChecked.Count() > 0 ? true : false);
                                ItmModel.ItemNote = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignNote : "");
                                ItmModel.StaticDesignScheduleDate = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignScheduleDate : null);
                                ItmModel.StaticDesignScheduleEndTime = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignScheduleEndTime : null);
                                ListDataModel.Add(ItmModel);
                            }
                            ViewBag.ItemList = ListDataModel;

                            //var ItemList7 = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));
                            //ViewBag.ItemList = ItemList7.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = true, StatusListID = s.StatusListID });


                            //stage note
                            var CSstagemodal7 = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage), Id);
                            ViewBag.CSstageModel = (CSstagemodal7 != null ? CSstagemodal7.FirstOrDefault() : null);

                            ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                            string HTMLContent7 = string.Empty;
                            ViewBag.ActiveStageId = ActiveStageId;
                            HTMLContent7 += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
                            return Json(new { Data = HTMLContent7, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                        }


                        var ItemList = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));
                        ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });
                        string HTMLContent = string.Empty;

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage), Id);
                        if (CSstagemodal != null && CSstagemodal.Count() > 0)
                        {
                            ViewBag.CSstageModel = CSstagemodal.FirstOrDefault();
                        }

                        ViewBag.ActiveStageId = ActiveStageId;
                        HTMLContent += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            //string HTMLContent = string.Empty;            
            //HTMLContent += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
            return Json(new { Data = "", ActiveStageId = "-1" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ShowCustomerServiceComplainActivityDetail(int Id)
        {
            List<CustomerCancellationActivityModel> ListData = new List<CustomerCancellationActivityModel>();
            ListData = CustomerService.GetCustomerComplaintActivityDetails(Id);

            var CustStageListModel = CustomerService.CSstageListWithCustomerWise((int)JKApi.Business.Enumeration.TypeList.ComplaintStage, Id).ToList();

            ViewBag.StageListCustomerWise = CustStageListModel;
            string HTMLContent = string.Empty;
            HTMLContent += RenderPartialViewToString("_CustomerComplainActivityDetailPopup", ListData);
            return Json(new { Data = HTMLContent }, JsonRequestBehavior.AllowGet);
        }

        //stage 1 Complaint Logged 
        public ActionResult SaveCSActivityComplaintLoggedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityComplaintLoggedData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3, optitem4, optitem5, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 2 Action- Follow up 
        public ActionResult SaveCSActivityActionsFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityActionsFollowUpData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3Date, optitem3Time, optitem4, optitem3EndTime, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 3 Follow-Up
        public ActionResult SaveCSActivityFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, int optitem1, string optitem2, int optitem3, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityFollowUpData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 4 Completion Closed
        public ActionResult SaveCSActivityCompletionClosedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem2Note, int optitem3, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityCompletionClosedData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem2Note, optitem3, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  :: Failed Inspection ::

        [HttpGet]
        public ActionResult CustomerServiceFailedInspectionList()
        {
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 67);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public ActionResult CustomerServiceFailedInspectionListResult(string status, string rgId)
        {
            try
            {
                int ServiceLogTypeListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.FailedInspection;
                string statusids = (int)JKApi.Business.Enumeration.CustomerStatusList.Open + "," + (int)JKApi.Business.Enumeration.CustomerStatusList.InProcess;
                var customers = CustomerService.GetCustomerServiceComplainListResult(statusids, rgId, ServiceLogTypeListId, status);
                var result = (from f in customers
                              select new
                              {
                                  f.CustomerId,
                                  f.CustomerNo,
                                  f.CustomerName,
                                  //f.Address,
                                  //f.StateName,
                                  //f.City,
                                  //f.PostalCode,
                                  //CustomerName = "CustomerName",
                                  //Address= "Address",
                                  //StateName= "StateName",
                                  //City= "City",
                                  //PostalCode= "PostalCode",
                                  Amount = string.Format("{0:c}", f.Amount),
                                  //Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  //AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  f.FranchiseeNo,
                                  f.FranchiseeName,
                                  //f.Reason,
                                  f.ComplaintsDate,
                                  f.FollowUpBy,
                                  f.Comments,
                                  f.ServiceCallLogId,
                                  f.StageStatusId,
                                  f.StageStatusName,
                                  f.CreatedBy,
                                  f.EmailNotesTo
                              }).ToList();

                return Json(new
                {
                    aadata = result,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CustomerFailedInspectionDetails(int Id, int SerCallId = 0)
        {
            ViewBag.CustomerId = Id;
            ViewBag.ServiceCallLogId = SerCallId;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(Id));

            var _dataModel = CustomerService.GetServiceCallLogById(SerCallId);
            string strCallDate = string.Empty;
            string strStatusReason = string.Empty;
            if (_dataModel != null)
            {
                strCallDate = (_dataModel.CallDate != null ? Convert.ToDateTime(_dataModel.CallDate).ToString("dd-MM-yyyy") : string.Empty);
                var lstStatusReasonList = CustomerService.GetStatusReasonList();
                lstStatusReasonList = lstStatusReasonList.Where(w => w.StatusReasonListId == _dataModel.StatusReasonListId);
                strStatusReason = ((lstStatusReasonList != null && lstStatusReasonList.Count() > 0) ? lstStatusReasonList.FirstOrDefault().Name : string.Empty);
            }
            ViewBag.CallDate = strCallDate;
            ViewBag.StatusReason = strStatusReason;

            string AccountType = string.Empty;
            string ContractType = string.Empty;
            string AgreementType = string.Empty;
            string BillingFrequencyName = string.Empty;
            string CleanFrequencyName = string.Empty;
            string ServiceTypeName = string.Empty;
            if (CustomerViewModel.Contract != null)
            {
                if (CustomerViewModel.Contract.AccountTypeListId > 0)
                {
                    var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
                    AccountType = AccountTypeList.Where(w => w.AccountTypeListId == CustomerViewModel.Contract.AccountTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.AgreementTypeListId > 0)
                {
                    var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
                    AgreementType = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.Contract.ContractTypeListId > 0)
                {
                    var ContractTypeList = CustomerService.GetContractTypeList().ToList();
                    ContractType = ContractTypeList.Where(w => w.ContractTypeListId == CustomerViewModel.Contract.ContractTypeListId).FirstOrDefault().Name;
                }
            }
            if (CustomerViewModel.ContractDetail != null)
            {
                if (CustomerViewModel.ContractDetail.BillingFrequencyListId > 0)
                {
                    var FrequencyList = CustomerService.GetFrequencyList().ToList();
                    BillingFrequencyName = FrequencyList.Where(w => w.FrequencyListId == CustomerViewModel.ContractDetail.BillingFrequencyListId).FirstOrDefault().Name;
                }
                if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                {
                    var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                    CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                }

                if (CustomerViewModel.ContractDetail.ServiceTypeListId > 0)
                {
                    var ServiceTypeListList = CustomerService.GetServiceTypeList().ToList();
                    ServiceTypeName = ServiceTypeListList.Where(w => w.ServiceTypeListid == CustomerViewModel.ContractDetail.ServiceTypeListId).FirstOrDefault().name;
                }

            }
            ViewBag.AccountType = AccountType;
            ViewBag.AgreementType = AgreementType;
            ViewBag.ContractType = ContractType;
            ViewBag.BillingFrequencyName = BillingFrequencyName;
            ViewBag.CleanFrequencyName = CleanFrequencyName;
            ViewBag.ServiceTypeName = ServiceTypeName;

            var DataModel = CustomerService.GetFranchiseeDistributionWithCustomer(Id);
            if (DataModel != null && DataModel.Count() > 0)
            {
                ViewBag.FranchiseeDistribution = DataModel;
            }
            else
            {
                ViewBag.FranchiseeDistribution = null;
            }

            return View(CustomerViewModel);
        }

        public ActionResult CustomerServiceFailedInspectionStageData(int Id)
        {
            int ActiveStageId = 0;
            var ActivityData = CustomerService.GetCSActivityByClassId(Id, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection));

            if (ActivityData == null || ActivityData.Count() == 0)
            {
                int FTStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIComplaintLogged);
                var ItemList = CustomerService.ValidationItemListStatus(FTStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection));
                ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });

                //Default stage
                ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIComplaintLogged);

                ViewBag.ActiveStageId = ActiveStageId;
                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("_CustomerServiceFailedInspectionStageActivity", null);
                return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var StageId = ActivityData.Max(w => w.StatusListId);

                var StageActivityData = ActivityData.Where(w => w.StatusListId == StageId);
                if (StageActivityData != null && StageActivityData.Count() > 0)
                {
                    var ItemListstage = CustomerService.ValidationItemListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection));

                    int StagItemCount = 0;
                    //if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ComplaintLogged ) // stage: 1,2,5
                    //{
                    //    StagItemCount = 5;
                    //}
                    //else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.FollowUp || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount || StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack) // stage: 3,4,6,7
                    //{
                    //    StagItemCount = 3;
                    //}
                    //else if (StageId == (int)JKApi.Business.Enumeration.CustomerStatusList.ActionsFollowUp)
                    //{
                    //    StagItemCount = 4;
                    //}

                    //if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != 4)                    
                    if ((ItemListstage.Count() != StageActivityData.Where(f => f.IsItemChecked == true && (f.IsStaticDesign == null || f.IsStaticDesign == false)).Count()) || StageActivityData.Where(f => (f.IsStaticDesign == true)).Count() != StagItemCount)
                    {
                        ActiveStageId = Convert.ToInt32(StageId);
                        List<CustomerCancellationStageModel> ListDataModel = new List<CustomerCancellationStageModel>();
                        foreach (var itm in ItemListstage)
                        {
                            var chkIsItemChecked = StageActivityData.Where(g => g.ValidationItemId == itm.ValidationItemId && g.IsItemChecked == true);
                            CustomerCancellationStageModel ItmModel = new CustomerCancellationStageModel();
                            ItmModel.ValidationItemId = itm.ValidationItemId;
                            ItmModel.Name = itm.Name;
                            ItmModel.StatusListID = itm.StatusListID;
                            ItmModel.Selected = (chkIsItemChecked.Count() > 0 ? true : false);
                            ItmModel.ItemNote = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignNote : "");
                            ListDataModel.Add(ItmModel);
                        }
                        ViewBag.ItemList = ListDataModel;

                        ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(Convert.ToInt32(StageId), Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection), Id);
                        ViewBag.CSstageModel = (CSstagemodal != null ? CSstagemodal.FirstOrDefault() : null);

                        ViewBag.ActiveStageId = ActiveStageId;
                        string HTMLContent = string.Empty;
                        HTMLContent += RenderPartialViewToString("_CustomerServiceFailedInspectionStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIComplaintLogged))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIActionsFollowUp);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIActionsFollowUp))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIFollowUp);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FIFollowUp))
                        {
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FICompletionClosed);
                        }
                        else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FICompletionClosed))
                        {
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.CompletionClosed);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.LetertotheCustomer))
                            //{
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.ReInspecttherAccount))
                            //{
                            //    ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack);
                            //}
                            //else if (Convert.ToInt32(StageId) == Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FollowupBackontrack))
                            //{
                            ActiveStageId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.FICompletionClosed);

                            List<CustomerCancellationStageModel> ListDataModel = new List<CustomerCancellationStageModel>();
                            foreach (var itm in ItemListstage)
                            {
                                var chkIsItemChecked = StageActivityData.Where(g => g.ValidationItemId == itm.ValidationItemId && g.IsItemChecked == true);
                                CustomerCancellationStageModel ItmModel = new CustomerCancellationStageModel();
                                ItmModel.ValidationItemId = itm.ValidationItemId;
                                ItmModel.Name = itm.Name;
                                ItmModel.StatusListID = itm.StatusListID;
                                ItmModel.Selected = (chkIsItemChecked.Count() > 0 ? true : false);
                                ItmModel.ItemNote = (chkIsItemChecked.Count() > 0 ? chkIsItemChecked.FirstOrDefault().StaticDesignNote : "");
                                ListDataModel.Add(ItmModel);
                            }
                            ViewBag.ItemList = ListDataModel;

                            //var ItemList7 = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.ComplaintStage));
                            //ViewBag.ItemList = ItemList7.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = true, StatusListID = s.StatusListID });


                            //stage note
                            var CSstagemodal7 = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection), Id);
                            ViewBag.CSstageModel = (CSstagemodal7 != null ? CSstagemodal7.FirstOrDefault() : null);

                            ViewBag.StageActivityStaticData = StageActivityData.Where(f => f.IsStaticDesign == true).ToList();

                            string HTMLContent7 = string.Empty;
                            ViewBag.ActiveStageId = ActiveStageId;
                            HTMLContent7 += RenderPartialViewToString("_CustomerServiceFailedInspectionStageActivity", null);
                            return Json(new { Data = HTMLContent7, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                        }


                        var ItemList = CustomerService.ValidationItemListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection));
                        ViewBag.ItemList = ItemList.Select(s => new CustomerCancellationStageModel() { ValidationItemId = s.ValidationItemId, Name = s.Name, Selected = false, StatusListID = s.StatusListID });
                        string HTMLContent = string.Empty;

                        //stage note
                        var CSstagemodal = CustomerService.CSstageListStatus(ActiveStageId, Convert.ToInt32(JKApi.Business.Enumeration.TypeList.FailedInspection), Id);
                        if (CSstagemodal != null && CSstagemodal.Count() > 0)
                        {
                            ViewBag.CSstageModel = CSstagemodal.FirstOrDefault();
                        }

                        ViewBag.ActiveStageId = ActiveStageId;
                        HTMLContent += RenderPartialViewToString("_CustomerServiceFailedInspectionStageActivity", null);
                        return Json(new { Data = HTMLContent, ActiveStageId = ActiveStageId }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            //string HTMLContent = string.Empty;            
            //HTMLContent += RenderPartialViewToString("_CustomerServiceComplainStageActivity", null);
            return Json(new { Data = "", ActiveStageId = "-1" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ShowCustomerServiceFailedInspectionActivityDetail(int Id)
        {
            List<CustomerCancellationActivityModel> ListData = new List<CustomerCancellationActivityModel>();
            ListData = CustomerService.GetCustomerFailedInspectionActivityDetails(Id);

            var CustStageListModel = CustomerService.CSstageListWithCustomerWise((int)JKApi.Business.Enumeration.TypeList.FailedInspection, Id).ToList();

            ViewBag.StageListCustomerWise = CustStageListModel;
            string HTMLContent = string.Empty;
            HTMLContent += RenderPartialViewToString("_CustomerFailedInspectionActivityDetailPopup", ListData);
            return Json(new { Data = HTMLContent }, JsonRequestBehavior.AllowGet);
        }

        //stage 1
        public ActionResult SaveCSActivityFIComplaintLoggedData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityFIComplaintLoggedData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3, optitem4, optitem5, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }
        //stage 2
        public ActionResult SaveCSActivityFIActionsFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityFIActionsFollowUpData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3Date, optitem3Time, optitem4, optitem3EndTime, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 3
        public ActionResult SaveCSActivityFIFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, string optitem2, int optitem3, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityFIFollowUpData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem3, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }

        //stage 4
        public ActionResult SaveCSActivityFICompletionClosedData(int CustomerId, int StagId, int CSstageId, string Note, int optitem1, int optitem2, string optitem2Note, int optitem3, string valIds = "", string strvalJsonList = "")
        {
            int Id = 0;
            if (CustomerId > 0 && StagId > 0)
            {
                List<ValidationItemDataModel> lstValidationItemlist = new List<ValidationItemDataModel>();
                if (strvalJsonList != "")
                {
                    lstValidationItemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValidationItemDataModel>>(strvalJsonList);
                }
                Id = CustomerService.SaveCSActivityFICompletionClosedData(CustomerId, StagId, CSstageId, Note, valIds, optitem1, optitem2, optitem2Note, optitem3, lstValidationItemlist);
            }
            return Json(new { Data = Id, success = (Id > 0 ? true : false) }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region :: Customer Service Reports :: 

        public ActionResult CustomerServiceReports()
        {
            ViewBag.CurrentMenu = "CustomerServiceReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Reports", "CustomerService", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("CS Reports", "CustomerService", new { area = "Portal" }), "CS Reports");

            return View();
        }
        public ActionResult CustomerServiceSearchWednesdayReport()
        {
            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name");
            return View();
        }
        public ActionResult CustomerServiceWednesdayReport(int Days = 0, string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0, string GroupByValue = "", string AtLeast = "", string SearchAmount = "", string AtRisk = "")
        {
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            if (regionId == "")
            {
                regionId = SelectedRegionId.ToString();
            }
            ViewBag.days = Days;
            ViewBag.acctypeId = ServiceLogTypeListId;
            ViewBag.groupBy = GroupByValue;
            ViewBag.regionids = regionId;
            ViewBag.AtLeast = AtLeast;
            ViewBag.SearchAmount = SearchAmount;
            ViewBag.AtRisk = AtRisk;

            var Model = CustomerService.GetCustomerServiceWednesdayReport(Convert.ToInt32(Days), "58,59,60,61,62,63,64,65", regionId, Convert.ToInt32(ServiceLogTypeListId), GroupByValue, AtLeast == "1" ? AtLeast : "", AtLeast == "2" ? AtLeast : "", SearchAmount, AtRisk);
            return View(Model);
        }
        #endregion

        public ActionResult CustomerSchedule(int? customerId, string regionId = null, bool isMonthly = false, int dayToAdd = 0, int userId = 0, DateTime? startDate = null, string callBackAction = null, string callBackController = null, string passId = null, int? statusListId = 0)
        {
            if (customerId <= 0 || customerId == null)
            {
                customerId = null;
            }

            if (regionId == "0" || regionId == null)
            {
                regionId = SelectedRegionId.ToString();
            }

            DateTime stDate = new DateTime();

            if (startDate == null)
            {
                stDate = DateTime.Now.Date;
            }
            else
            {
                stDate = Convert.ToDateTime(startDate).Date;
            }

            var days = DateTime.DaysInMonth(stDate.Year, stDate.Month);
            dayToAdd = days - stDate.Day;
            DateTime endDate = new DateTime();

            if (!isMonthly)
            {
                var week = 0;
                decimal div = Convert.ToDecimal(stDate.Day) / 7;
                week = Convert.ToInt32(Math.Ceiling(div));

                switch (week)
                {
                    case 1:
                        stDate = new DateTime(stDate.Year, stDate.Month, 1);
                        endDate = new DateTime(stDate.Year, stDate.Month, 7); ;
                        break;
                    case 2:
                        stDate = new DateTime(stDate.Year, stDate.Month, 8);
                        endDate = new DateTime(stDate.Year, stDate.Month, 14);
                        break;
                    case 3:
                        stDate = new DateTime(stDate.Year, stDate.Month, 15);
                        endDate = new DateTime(stDate.Year, stDate.Month, 21);
                        break;
                    case 4:
                        stDate = new DateTime(stDate.Year, stDate.Month, 22);
                        endDate = new DateTime(stDate.Year, stDate.Month, 28);
                        break;
                    default:
                        stDate = new DateTime(stDate.Year, stDate.Month, 29);
                        endDate = new DateTime(stDate.Year, stDate.Month, days);
                        break;
                }
            }
            else
            {

                stDate = new DateTime(stDate.Year, stDate.Month, 1);
                endDate = new DateTime(stDate.Year, stDate.Month, days);
            }

            if (statusListId <= 0)
            {
                statusListId = (int)PurposeForDiffForm.CustomerServiceAndOperations;
            }
            var purposeType = _crmService.GetAllPurposeType(Convert.ToInt32(statusListId));

            if (callBackAction == null)
            {
                callBackAction = "CustomerSchedule";
            }
            if (callBackController == null)
            {
                callBackController = "CustomerService";
            }
            if (passId == null)
            {
                passId = "customerId";
            }


            ViewBag.PurposeType = purposeType;

            ViewBag.StatusListId = statusListId;
            ViewBag.StartDate = stDate;
            ViewBag.EndDate = endDate;
            ViewBag.CustomerId = customerId;
            ViewBag.DaysToAdd = dayToAdd;
            ViewBag.IsMonthly = isMonthly;
            ViewBag.CallBackAction = callBackAction;
            ViewBag.CallBackController = callBackController;
            ViewBag.PassId = passId;

            var data = CustomerService.GetCustomerServiceScheduleData(customerId, regionId, dayToAdd, userId, stDate, endDate);

            return View(data);
        }
    }
}