using JKApi.Service.Service.Administration.Company;
using JKApi.Service.Helper;
using JKApi.Service.Service;
using JKViewModels.Administration.Company;
using JKViewModels.Administration.System;
using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKApi.Service.Service.Administration.System;
using JKApi.Service.Service.Administration.General;
using JKViewModels.Administration.General;
using Application.Web.Core;
using JKApi.Service.ServiceContract.CustomerInvoice;
using JKApi.Data.DAL;
using JKApi.Service.Helper.Extension;
using JKApi.Service.ServiceContract.Franchisee;
using JKViewModels.Franchise;
using JKViewModels.Franchisee;
using JKApi.Service.ServiceContract.JKControl;
using JKViewModels.User;
using JKViewModels;
using JKApi.Service;
using System.Reflection;
using JKApi.Service.Service.Customer;
using JKApi.Service.Service.TaxAPI;
using JKApi.Core;
using JKApi.Service.Service.Company;
using JKApi.Service.Service.Inspection;
using JKApi.Service.ServiceContract.Inspection;
using JKApi.Service.ServiceContract.CRM;
using JKViewModels.Job;
using JKViewModels.Common;
using System.Web.Script.Serialization;
using JKApi.Service.Service.Job;
using Newtonsoft.Json;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Company;
using JKViewModels.Inspection;
using System.Globalization;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    //[MenuAccessAttribute]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true)]
    public class AdministrationController : ViewControllerBase
    {
        CompanyService compServ = new CompanyService();
        SystemService sysServ = new SystemService();
        CustomerService customerservice = new CustomerService();
        ImportTax ImpTax = new ImportTax();

        private IInspectionService _inspectionService;
        private ITemplateService _templateService;
        public new readonly ICRM_Service _crmService;
        private IJobService _jobService;


        public AdministrationController(
            ICustomerInvoiceService customerinvoiceservice,
            ICRM_Service crmService,
            IFranchiseeService franchiseeService,
            ICommonService commonService,
            IUserService userService,
            IInspectionService inspectionService,
            ITemplateService templateService,
            IEncryptDecrypt encryptDecrypt,
            ICacheProvider cacheProvider,
            IJobService jobService,
            ICustomerService customerService,
            ICompanyService companyservice)
        {
            CustomerService = customerService;
            _companyService = companyservice;
            _userService = userService;
            _encryptDecrypt = encryptDecrypt;
            _customerinvoiceservice = customerinvoiceservice;
            FranchiseeService = franchiseeService;
            _commonService = commonService;
            _inspectionService = inspectionService;
            _cacheProvider = cacheProvider;
            ViewBag.HMenu = "Administration";
            _crmService = crmService;
            _templateService = templateService;
            _jobService = jobService;
        }

        // GET: Portal/Administration
        public ActionResult Index()
        {
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Administration");
            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View(model);
        }

        public ActionResult Area()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "Administration";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Area", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Area", "Administration", new { area = "Portal" }), "Area");
            return View();
        }

        #region General
        GeneralService mngser;


        public ActionResult CPIList()
        {
            ViewBag.CurrentMenu = "CPIList";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("CPIList", "Administration", new { area = "Portal" }), "CPI List");
            mngser = new GeneralService();

            var data = mngser.GetCPIList();
            return View(data);
        }
        public ActionResult CPIDelete(int id)
        {
            ViewBag.CurrentMenu = "CPIDelete";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CPIList", "Administration", new { area = "Portal" }), "CPI List");
            mngser = new GeneralService();
            mngser.DeleteCPIById(id);
            return View();
        }

        /// <summary>
        /// spsave_CPI_Result
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateCPI()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCPI(CPIViewModel model)
        {

            ViewBag.CurrentMenu = "CreateCPI";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("CPIList", "Administration", new { area = "Portal" }), "CPI List");
            mngser = new GeneralService();

            var data = mngser.SaveCPI(model.id, model.billmonth, model.billyear, model.percent, model.description, model.applied, model.userid);
            //var data = mngser.SaveCPI(model);
            ViewBag.Result = data;
            return View();
        }
        public ActionResult EditCPI(int id)
        {
            ViewBag.CurrentMenu = "EditCPI";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("CPIList", "Administration", new { area = "Portal" }), "CPI List");
            mngser = new GeneralService();

            var data = mngser.SearchCPIById(id);
            return View(data);
        }
        [HttpPost]
        public ActionResult EditCPI(CPIViewModel model)
        {

            ViewBag.CurrentMenu = "EditCPI";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("CPIList", "Administration", new { area = "Portal" }), "CPI List");
            mngser = new GeneralService();

            var data = mngser.UpdateCPI(model.id, model.billmonth, model.billyear, model.percent, model.description, model.applied, model.userid);
            ViewBag.Result = data;
            return View();
        }
        #endregion
        #region Company

        // GET: Portal/Administration/CompanyConfiguration
        [HttpGet]
        public ActionResult CompanyConfiguration()
        {
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Company Configuration");

            var stateList = jkEntityModel.StateLists.OrderBy(s => s.Name).ToList();
            List<SelectListItem> usStateList = new List<SelectListItem>();

            foreach (var y in stateList)
            {
                usStateList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.abbr.ToString() });
            }
            ViewBag.usStateList = usStateList;

            //CompanyViewModel com = compServ.Load(Convert.ToInt32(Request.Cookies[Constants.SESSION_STATE].Values.Get(Constants.REGION_ID).ToString()));
            CompanyViewModel com = compServ.Load(SelectedRegionId);
            List<ConfigSettingViewModel> configSettingList = CompanyService.GetSettings();

            ViewBag.configSettingList = configSettingList;
            List<JKViewModels.Administration.Company.FeeViewModel> sysfees = FeeService.GetList();
            ViewBag.sysfees = sysfees;

            List<JKApi.Data.DAL.TransactionNumberConfig> trxIdentifierList = CompanyService.GetTransactionIdentifierList();
            var DataResult = trxIdentifierList.Where(w => w.RegionId == SelectedRegionId).ToList();
            ViewBag.trxIdentifierList = DataResult;


            return View(com);
        }
        // GET: Portal/Administration/CompanyConfiguration
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyConfiguration(FormCollection collection, string command)
        {
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("CompanyConfiguration", "Administration", new { area = "Portal" }), "Company Configuration");
            if (Request.Form["Cancel"] != null)
                return View();

            //int userid = Convert.ToInt32(Request.Cookies[Constants.SESSION_STATE].Values.Get(Constants.USER_ID).ToString());
            int userid = 878768;

            int recordid;
            int checkvalue;
            foreach (string key in collection.AllKeys)
            {
                var val = collection[key];

                if (key.StartsWith("txtfee") && val != null && val.Trim().Length > 1)
                {
                    compServ.SaveFeeSetting(Convert.ToInt32(key.Replace("txtfee", "")), val.ToString(), userid);
                }
                if (key.StartsWith("txtconfig") && val != null && val.Trim().Length > 1)
                {
                    compServ.SaveConfigurationSetting(Convert.ToInt32(key.Replace("txtconfig", "")), val.ToString(), userid);
                }
                if (key.ToString().StartsWith("chkconfig") && val != null && val.Trim().Length > 1)
                {
                    recordid = Convert.ToInt32(key.Replace("chkconfig", ""));
                    checkvalue = collection["chkconfig" + recordid.ToString()] == null || collection["chkconfig" + recordid.ToString()] == Constants.CheckboxEmptyValue ? Constants.No : Constants.Yes;
                    compServ.SaveConfigurationSetting(recordid, checkvalue.ToString(), userid);
                }

                if (key.StartsWith("startamount") && val != null && val.Trim().Length > 1)
                {
                    recordid = Convert.ToInt32(key.Replace("txtbbpastartamount", ""));
                    compServ.SaveBBPAFeeSetting(recordid, Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpastartamount" + recordid.ToString()]))
                                                        , Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpaendamount" + recordid.ToString()]))
                                                        , Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpaamount" + recordid.ToString()]))
                                                        , userid);
                }
            }

            return RedirectToAction("CompanyConfiguration");
        }

        public ActionResult TaxAPI()
        {
            ViewBag.CurrentMenu = "TaxAPI";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("TaxAPI", "Administration", new { area = "Portal" }), "Tax Import");
            return View();
        }

        public ActionResult PaymentGateway()
        {
            ViewBag.CurrentMenu = "PaymentGateway";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("PaymentGateway", "Administration", new { area = "Portal" }), "Payment Gateway");
            ViewBag.Inserted = "0";
            return View();
        }

        [HttpPost]
        public ActionResult PaymentGatewayInsert(JKApi.Data.DAL.PaymentGatewayDetail paymentGatewayDetail)
        {
            int RegionID = 0;
            string userRegion = _claimView.GetCLAIM_SELECTED_COMPANY_ID().ToString();
            if (userRegion != null)
            {
                RegionID = Convert.ToInt32(userRegion);
            }
            GeneralService generalService = new GeneralService();
            paymentGatewayDetail.CreatedDate = DateTime.Now;
            paymentGatewayDetail.RegionID = RegionID;
            generalService.PaymentGatewayInsertService(paymentGatewayDetail);
            ViewBag.Inserted = "1";
            return View("PaymentGateway");
        }


        [HttpGet]
        public JsonResult GatewayDetail(int Id)
        {
            GeneralService generalService = new GeneralService();
            var data = generalService.GetPaymentGatewayList().Where(r => r.Id == Id).Select(r => new { r.CreatedDate, r.Id, r.IsActive, r.IsDelete, r.LoginID, r.PaymentGateway, r.TransactionKey, r.merchantid }).FirstOrDefault();
            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPaymentgatewayList()
        {
            int RegionID = 0;
            string userRegion = _claimView.GetCLAIM_SELECTED_COMPANY_ID().ToString();
            if (userRegion != null)
            {
                RegionID = Convert.ToInt32(userRegion);
            }
            GeneralService generalService = new GeneralService();
            var data = generalService.GetPaymentGatewayList().Where(r => r.RegionID == RegionID).Select(r => new { r.CreatedDate, r.Id, r.IsActive, r.IsDelete, r.LoginID, r.PaymentGateway, r.TransactionKey, r.merchantid }).ToList();
            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TaxImport()
        {
            List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = customerservice.GetTaxrateList();
            List<Address> lstAddress = customerservice.GetAddressList();
            ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);

            return Json("str");
        }

        // GET: Portal/Administration/CheckLayout
        [HttpGet]
        public ActionResult CheckLayout()
        {
            ViewBag.CurrentMenu = "AdministrationGeneral";

            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Check Layout");

            CheckConfigurationViewModel vm = new CheckConfigurationViewModel();

            List<CheckLayoutViewModel> clvms = compServ.GetCheckLayoutsForRegion().ToList();

            if (clvms == null)
            {
                clvms = new List<CheckLayoutViewModel>();

                CheckLayoutViewModel clvm = new CheckLayoutViewModel();
                clvm.Id = -1;
                clvm.Elements = new List<CheckLayoutElementViewModel>();

                clvms.Add(clvm);
            }

            CheckCalibrationViewModel calibrationVM = compServ.GetCheckCalibrationForRegion();
            if (calibrationVM == null)
            {
                calibrationVM = new CheckCalibrationViewModel();
                calibrationVM.Id = -1;
            }

            vm.CheckLayouts = clvms;
            vm.Calibration = calibrationVM;

            //var existingElementTypes = vm.Elements.Select(o => o.ElementType).ToList();

            ViewBag.ElementTypes = new SelectList(compServ.GetCheckLayoutElementTypeList(), "CheckLayoutElementTypeListId", "Name");

            return View(vm);
        }

        // GET: Portal/Administration/CheckLayout
        [HttpPost, ValidateInput(false)]
        public ActionResult CheckLayout(FormCollection collection, string command)
        {
            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("CheckLayout", "Administration", new { area = "Portal" }), "Check Layout");
            if (Request.Form["Cancel"] != null)
                return View();

            var createdDate = DateTime.Now;

            using (var context = new jkDatabaseEntities())
            {
                CheckLayoutViewModel vm = null;
                var intRes = 0;
                var dblRes = 0.0;

                Int32.TryParse(collection["checkLayoutId"], out intRes);
                if (intRes != -1)
                {
                    vm = compServ.GetCheckLayout(intRes);
                    vm.ModifiedBy = this.SelectedUserId;
                    vm.ModifiedDate = createdDate;
                }
                else
                {
                    vm = new CheckLayoutViewModel();
                    vm.Id = -1;
                    vm.Elements = new List<CheckLayoutElementViewModel>();
                    vm.CreatedBy = this.SelectedUserId;
                    vm.CreateDate = createdDate;
                    vm.IsDefault = true;
                }

                var checkName = collection["checkName"];
                var checkPosition = collection["checkPosition"];
                var checkX = double.TryParse(collection["checkX"], out dblRes) ? dblRes : 0;
                var checkY = double.TryParse(collection["checkY"], out dblRes) ? dblRes : 0;

                vm.Name = checkName;
                vm.X = checkX;
                vm.Y = checkY;

                switch (checkPosition)
                {
                    case "Top":
                        vm.Position = 0;
                        break;
                    case "Bottom":
                        vm.Position = 1;
                        break;
                    default:
                        vm.Position = 2;
                        break;
                }

                foreach (string key in collection.AllKeys.Where(o => o.StartsWith("type")))
                {
                    var value = collection[key];
                    var tokens = key.Split('_');

                    if (tokens.Length < 2)
                        continue;

                    var typeIdStr = tokens[0].Substring(4); // "typeXXXXX"
                    var typeId = Convert.ToInt32(typeIdStr);

                    CheckLayoutElementViewModel clevm = vm.Elements.Where(o => o.ElementTypeId == typeId).FirstOrDefault();
                    if (clevm != null)
                    {
                        clevm.ModifiedBy = this.SelectedUserId;
                        clevm.ModifiedDate = createdDate;
                    }
                    else
                    {
                        clevm = new CheckLayoutElementViewModel();
                        clevm.Id = -1;
                        clevm.IsActive = true;
                        clevm.ElementTypeId = typeId;
                        clevm.CreatedBy = this.SelectedUserId;
                        clevm.CreateDate = createdDate;
                        vm.Elements.Add(clevm);
                    }

                    switch (tokens[1])
                    {
                        case "chk":
                            clevm.IsActive = value.Contains("true");
                            break;
                        case "elementX":
                            clevm.X = double.TryParse(value, out dblRes) ? dblRes : 0;
                            break;
                        case "elementY":
                            clevm.Y = double.TryParse(value, out dblRes) ? dblRes : 0;
                            break;
                    }

                }

                compServ.InsertOrUpdateCheckLayout(vm);

                CheckCalibrationViewModel calibrationVM = null;

                Int32.TryParse(collection["checkCalibrationId"], out intRes);
                if (intRes != -1)
                {
                    calibrationVM = compServ.GetCheckCalibration(intRes);
                    calibrationVM.ModifiedBy = this.SelectedUserId;
                    calibrationVM.ModifiedDate = createdDate;
                }
                else
                {
                    calibrationVM = new CheckCalibrationViewModel();
                    calibrationVM.Id = -1;

                    calibrationVM.CreatedBy = this.SelectedUserId;
                    calibrationVM.CreateDate = createdDate;
                }

                calibrationVM.ShiftX = double.TryParse(collection["calibrationShiftX"], out dblRes) ? dblRes : 0;
                calibrationVM.ShiftY = double.TryParse(collection["calibrationShiftY"], out dblRes) ? dblRes : 0;

                compServ.InsertOrUpdateCheckCalibration(calibrationVM);

            }

            return RedirectToAction("CompanyConfiguration");
        }

        #endregion

        #region System

        // GET: Portal/Administration/SystemConfiguration
        [HttpGet]
        public ActionResult SystemConfiguration()
        {
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "System Configuration");
            List<ConfigSettingViewModel> configSettingList = new List<ConfigSettingViewModel>();

            configSettingList = sysServ.getConfigSettings();

            return View(configSettingList);
        }
        // GET: Portal/Administration/SystemConfiguration
        [HttpPost, ValidateInput(false)]
        public ActionResult SystemConfiguration(FormCollection collection, string command)
        {
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("SystemConfiguration", "Administration", new { area = "Portal" }), "System Configuration");

            //int userid = Convert.ToInt32(Request.Cookies[Constants.SESSION_STATE].Values.Get(Constants.USER_ID).ToString());
            int userid = 878768;

            int recordid;
            int checkvalue;
            foreach (string key in collection.AllKeys)
            {
                var val = collection[key];

                if (key.StartsWith("txtfee") && val != null && val.Trim().Length > 1)
                {
                    sysServ.SaveFeeSetting(Convert.ToInt32(key.Replace("txtfee", "")), val.ToString(), userid);
                }
                if (key.StartsWith("txtconfig") && val != null && val.Trim().Length > 1)
                {
                    sysServ.SaveConfigurationSetting(Convert.ToInt32(key.Replace("txtconfig", "")), val.ToString(), userid);
                }
                if (key.ToString().StartsWith("chkconfig") && val != null && val.Trim().Length > 1)
                {
                    recordid = Convert.ToInt32(key.Replace("chkconfig", ""));
                    checkvalue = collection["chkconfig" + recordid.ToString()] == null || collection["chkconfig" + recordid.ToString()] == Constants.CheckboxEmptyValue ? Constants.No : Constants.Yes;
                    sysServ.SaveConfigurationSetting(recordid, checkvalue.ToString(), userid);
                }

                if (key.StartsWith("startamount") && val != null && val.Trim().Length > 1)
                {
                    recordid = Convert.ToInt32(key.Replace("txtbbpastartamount", ""));
                    sysServ.SaveBBPAFeeSetting(recordid, Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpastartamount" + recordid.ToString()]))
                                                        , Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpaendamount" + recordid.ToString()]))
                                                        , Convert.ToDecimal(UtilityService.ScrubCurrency(collection["txtbbpaamount" + recordid.ToString()]))
                                                        , userid);
                }
            }

            return RedirectToAction("SystemConfiguration");
        }

        #endregion

        #region FranchiseeFee
        public ActionResult FeeList()
        {
            ViewBag.CurrentMenu = "FranchiseeFeeList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeFeeList", "Administration", new { area = "Portal" }), "FranchiseeFee List");
            FullFranchiseeFeeListFeeRateTypeListCollectionViewModel FullFranchiseeFeeListFeeRateTypeListCollectionViewModel = new FullFranchiseeFeeListFeeRateTypeListCollectionViewModel();
            FullFranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();
            return View("FranchiseeFeeList", FullFranchiseeFeeListFeeRateTypeListCollectionViewModel);
        }
        public ActionResult FranchiseeFeeList()
        {
            ViewBag.CurrentMenu = "FranchiseeFeeList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeFeeList", "Administration", new { area = "Portal" }), "FranchiseeFee List");
            FullFranchiseeFeeListFeeRateTypeListCollectionViewModel FullFranchiseeFeeListFeeRateTypeListCollectionViewModel = new FullFranchiseeFeeListFeeRateTypeListCollectionViewModel();
            FullFranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();
            return View(FullFranchiseeFeeListFeeRateTypeListCollectionViewModel);
        }

        [HttpGet]
        public ActionResult AddEditFranchiseeFeeList(string id)
        {
            int _id = Convert.ToInt32(id);
            var feeRateTypeList = FranchiseeService.GetFeeRateTypeList().ToList();
            FranchiseeFeeListViewModel FranchiseeFeeListViewModel = FranchiseeService.GetFranchiseeFeeListById(_id).ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();

            if (FranchiseeFeeListViewModel == null)
            {
                FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
                ViewBag.FeeRateTypeList = new SelectList(feeRateTypeList, "FeeRateTypeListId", "Rate", FranchiseeFeeListViewModel.FeeRateTypeListId);

            }
            else
            {
                ViewBag.FeeRateTypeList = new SelectList(feeRateTypeList, "FeeRateTypeListId", "Rate", FranchiseeFeeListViewModel.FeeRateTypeListId);

            }
            return View(FranchiseeFeeListViewModel);
        }

        [HttpPost]
        public ActionResult AddEditFranchiseeFeeList(FranchiseeFeeListViewModel FranchiseeFeeListViewModel, FormCollection collection)
        {
            int? RateListId = Convert.ToInt32(collection["FeeRateTypeList"]);
            if (RateListId > 0)
            {
                FranchiseeFeeListViewModel.FeeRateTypeListId = Convert.ToInt32(RateListId);
            }
            FranchiseeFeeListViewModel = FranchiseeService.SaveFranchiseeFeeList(FranchiseeFeeListViewModel.ToModel<FranchiseeFeeList, FranchiseeFeeListViewModel>()).ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();

            return RedirectToAction("FeeList", "Administration", new { area = "Portal" });
        }


        #endregion
        public ActionResult DeleteFranchiseeFeeList(string id)
        {
            int _id = Convert.ToInt32(id);
            FranchiseeFeeListViewModel FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
            FranchiseeFeeListViewModel = FranchiseeService.DeleteFranchiseeFeeList(_id).ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();

            var jsonResult = Json(new { aaData = FranchiseeFeeListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult RegionList()
        {
            ViewBag.CurrentMenu = "RegionList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("RegionList", "Administration", new { area = "Portal" }), "Region List");

            var Regionlist = jkEntityModel.Regions.ToList();

            return View(Regionlist);

        }

        #region :: Fee ::
        public ActionResult DeleteFee(string id)
        {
            int _id = Convert.ToInt32(id);
            FeesViewModel FeesViewModel = new FeesViewModel();
            FeesViewModel = FranchiseeService.DeleteFranchiseeFee(_id).ToModel<FeesViewModel, FranchiseeFee>();


            var jsonResult = Json(new { aaData = FeesViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult DeleteFee_Temp(string id)
        {
            int _id = Convert.ToInt32(id);
            FeesViewModel FeesViewModel = new FeesViewModel();
            FeesViewModel = FranchiseeService.DeleteFranchiseeFee_Temp(_id).ToModel<FeesViewModel, FranchiseeFee_Temp>();


            var jsonResult = Json(new { aaData = FeesViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult AddEditFee(string id)
        {
            int _id = Convert.ToInt32(id);
            FeesViewModel FeesViewModel = new FeesViewModel();
            var FranchiseeFeeList = FranchiseeService.GetFranchiseeFeeList().ToList();

            var feeRateTypeList = FranchiseeService.GetFeeRateTypeList().ToList();
            if (_id > 0)
            {
                //FeesViewModel = FranchiseeService.GetFeesById(_id).ToModel<FeesViewModel, FranchiseeFee>();
                FeesViewModel = FranchiseeService.GetFranchiseeFeeById(_id).ToModel<FeesViewModel, FranchiseeFee>();
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", FeesViewModel.FeesId);
            }
            else
            {
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", 0);
            }
            return View(FeesViewModel);
        }
        [HttpGet]
        public ActionResult AddEditFeeFourStep(string id)
        {
            int _id = Convert.ToInt32(id);
            FeesViewModel FeesViewModel = new FeesViewModel();
            var FranchiseeFeeList = FranchiseeService.GetFranchiseeFeeList().ToList();

            var feeRateTypeList = FranchiseeService.GetFeeRateTypeList().ToList();
            if (_id > 0)
            {
                FeesViewModel = FranchiseeService.GetFranchiseeFeeById(_id).ToModel<FeesViewModel, FranchiseeFee>();
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", FeesViewModel.FeesId);
            }
            else
            {
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", 0);
            }
            return View(FeesViewModel);
        }
        [HttpGet]
        public ActionResult AddEditFeeFourStepTemp(string id)
        {
            int _id = Convert.ToInt32(id);
            FeesViewModel FeesViewModel = new FeesViewModel();
            var FranchiseeFeeList = FranchiseeService.GetFranchiseeFeeList().ToList();

            var feeRateTypeList = FranchiseeService.GetFeeRateTypeList().ToList();
            if (_id > 0)
            {
                FeesViewModel = FranchiseeService.GetFranchiseeFeeById_Temp(_id).ToModel<FeesViewModel, FranchiseeFee_Temp>();
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", FeesViewModel.FeesId);
            }
            else
            {
                ViewBag.FranchiseeFeeList = new SelectList(FranchiseeFeeList, "FranchiseeFeeListId", "Name", 0);
            }
            return View(FeesViewModel);
        }
        public ActionResult SaveFeeDetails(int FranchiseeId, int FeeId = 0, string FeesId = "", string Amount = "", bool IsActive = false)
        {
            FranchiseeFee FranchiseeFee = new FranchiseeFee();
            FranchiseeFee.FranchiseeFeeId = FeeId;
            FranchiseeFee.FranchiseeId = FranchiseeId;
            FranchiseeFee.FeeRateType = 1;
            FranchiseeFee.FeesId = Convert.ToInt32(FeesId);
            FranchiseeFee.Amount = Convert.ToDecimal(Amount);
            FranchiseeFee.IsActive = IsActive;
            FranchiseeFee.IsDelete = false;
            FranchiseeFee.CreatedBy = -1;
            FranchiseeFee.CreatedDate = DateTime.Now;
            FranchiseeFee = FranchiseeService.SaveFranchiseeFee(FranchiseeFee);
            return Json(new { Data = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFeeDetailsTemp(int FranchiseeId, int FeeId = 0, string FeesId = "", string Amount = "", bool IsActive = false)
        {
            FranchiseeFee_Temp FranchiseeFee = new FranchiseeFee_Temp();
            FranchiseeFee.FranchiseeFeeId = FeeId;
            FranchiseeFee.FranchiseeId = FranchiseeId;
            FranchiseeFee.FeeRateType = 1;
            FranchiseeFee.FeesId = Convert.ToInt32(FeesId);
            FranchiseeFee.Amount = Convert.ToDecimal(Amount);
            FranchiseeFee.IsActive = IsActive;
            FranchiseeFee.IsDelete = false;
            FranchiseeFee.CreatedBy = -1;
            FranchiseeFee.CreatedDate = DateTime.Now;
            FranchiseeFee = FranchiseeService.SaveFranchiseeFee_Temp(FranchiseeFee);
            return Json(new { Data = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFranchiseeFeeList(int Id)
        {
            var Data = FranchiseeService.GetFeeListCollection(Id);

            var jsonResult = Json(new { Data = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult GetFranchiseeFeeList_Temp(int Id)
        {
            var Data = FranchiseeService.GetFeeListCollection_Temp(Id);

            var jsonResult = Json(new { Data = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        #endregion

        #region User Managment
        public ActionResult AddUser(int? UserId)
        {

            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AddUser", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("AddUser", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("AddUser", "Administration", new { area = "Portal" }), "Add User");

            ViewBag.RoleList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRole.ToString()), "Value", "Text");
            ViewBag.GroupList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthGroup.ToString()), "Value", "Text");
            ViewBag.DepartmentList = new SelectList(_crmService.GetDepartment(), "DepartmentId", "Name");
            List<DropDownModel> RegionList = new List<DropDownModel>();
            RegionList.Add(new DropDownModel() { Text = "All Region", Value = 0 });
            var regionList = _commonService.DropDownListByName(MasterDropName.Region.ToString());
            foreach (var item in regionList)
            {
                RegionList.Add(item);
            }

            ViewBag.RegionList = new SelectList(RegionList, "Value", "Text");

            if (UserId > 0)
            {
                UserDetailViewModel UserViewModel = new UserDetailViewModel();

                UserViewModel.UserId = (int)UserId;
                UserViewModel.ActionType = "S";
                UserViewModel = _userService.SaveUser(UserViewModel);

                return View(UserViewModel);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddUser(UserDetailViewModel UserViewModel, FormCollection collection)
        {

            UserViewModel.CreatedBy = 1;
            UserViewModel.CreatedDate = DateTime.Now;
            UserViewModel.IsEnable = true;


            UserViewModel.ActionType = UserViewModel.UserId > 0 ? "U" : "I";

            UserViewModel = _userService.SaveUser(UserViewModel);

            if (UserViewModel != null && UserViewModel.objErrorModel.Count > 0)
            {
                return View(UserViewModel);
            }
            else
            {
                if (UserViewModel.UserId == LoginUserId)
                {
                    Session["DisPlayUserName"] = UserViewModel.FirstName;
                }
                return Redirect("ListUser");
            }

            //UserViewModel = _userService.SaveUser(UserViewModel.ToModel<AuthUserLogin, UserDetailViewModel>()).ToModel<UserDetailViewModel, AuthUserLogin>();
            //return View();
        }

        public ActionResult ListUser(int? RegionId, int? IsActive)
        {

            bool? IsActiveFlag = null;
            if (IsActive != null)
            {
                IsActiveFlag = IsActive == 1 ? true : false;
            }

            UserLoginListViewModel model = new UserLoginListViewModel() { RegionId = RegionId, IsActve = IsActiveFlag };

            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ListUser", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("ListUser", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("ListUser", "Administration", new { area = "Portal" }), "List User");

            var RegionListModel = _userService.getRegion();
            var UserList = _userService.GetUserSearchList(model);

            ViewBag.Region = new SelectList(RegionListModel, "RegionId", "Name");
            return View(UserList);
        }

        public JsonResult UserResetPassword(int UserId)
        {
            int results = 0;
            if (UserId != 0)
            {
                UserDetailViewModel UserViewModel = new UserDetailViewModel();
                UserViewModel.UserId = UserId;
                UserViewModel.ActionType = "S";
                UserViewModel = _userService.SaveUser(UserViewModel);

                var tempPassword = DateTime.Now.ToString(CultureInfo.InvariantCulture).GetHashCode().ToString("x");
                UserViewModel.ActionType = UserViewModel.UserId > 0 ? "U" : "I";
                UserViewModel.Password = tempPassword;
                UserViewModel = _userService.SaveUser(UserViewModel);
                forgetpasswordEmail(UserViewModel.FirstName, UserViewModel.UserName, UserViewModel.Email, tempPassword);
                results = 1;
            }
            return this.Json(results, JsonRequestBehavior.AllowGet);
        }

        private void forgetpasswordEmail(string name, string username, string email, string tempPass)
        {
            try
            {
                var SendEmailcontext = System.Web.HttpContext.Current.Request;
                var url = SendEmailcontext.Url.Authority;
                var scheme = SendEmailcontext.Url.Scheme;
                string body = "Forgot Password Detail-";
                if (SendEmailcontext.Url.Authority.StartsWith("localhost"))
                {
                    body += "Hello " + name + ",< br /><p>You recently requested to reset your password for your Janiking account. Click on the Reset Password below and put Temporary Password with new Password.</p><a href=\"" + scheme + "://" + url + "/JKControl/User/ResetPassword?email=" + email + "\">Reset Password</a><br />Your Temporary Password is - <b>" + tempPass + "</b>";
                }
                else
                {
                    //body += "Hello,<br /><p>User Name - " + username + "</p><br /><p>Email Id - <b>" +
                    //        email + "</b></p><br /><p>Temp Password - <b>" + tempPass + "</b></p>";

                    body += "Hello " + name + ",<br /><p><p>You recently requested to reset your password for your Janiking account. Click on the Reset Password below and put Temporary Password with new Password.</p><br /><a href=\"" + scheme + "://" + url + "/JKControl/User/ResetPassword?email=" + email + "\">Reset Password</a><br />Your Temporary Password is - <b>" + tempPass + "</b>";
                }
                string subject = "Reset Password";
                _mailService.SendEmailAsync(email, body, subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Role
        public ActionResult Role()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Role", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Role", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("Role", "Administration", new { area = "Portal" }), "Role");
            RoleViewModel model = new RoleViewModel();
            model = _userService.GetRoleList(model);

            ViewBag.UserList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthUserLogin.ToString()), "Value", "Text");
            ViewBag.RoleTypeList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRoleType.ToString()), "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public ActionResult Role(RoleViewModel model)
        {
            model.CreatedBy = 1;
            model.IsEnable = true;
            model.ActionType = model.RoleId > 0 ? "U" : "I";
            model = _userService.SaveRole(model);
            _cacheProvider.Remove(CacheKeyName.DropDownValues + MasterDropName.AuthRole.ToString());
            return Redirect("Role");
        }

        #endregion

        #region Groups
        public ActionResult Group()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "Group");

            GroupViewModel model = new GroupViewModel();
            model = _userService.GetGroupList(model);

            ViewBag.RegionList = new SelectList(_commonService.DropDownListByName(MasterDropName.Region.ToString()), "Value", "Text");
            return View(model);
        }

        [HttpPost]
        public ActionResult Group(GroupViewModel model)
        {

            model.CreatedBy = 1;
            model.IsEnable = true;
            model.ActionType = model.GroupId > 0 ? "U" : "I";
            model = _userService.SaveGroup(model);
            _cacheProvider.Remove(CacheKeyName.DropDownValues + MasterDropName.AuthGroup.ToString());
            return Redirect("Group");
        }

        public ActionResult GetRegionByGroupId(string GorupId)
        {
            if (!string.IsNullOrWhiteSpace(GorupId))
            {
                int iGorupId = Convert.ToInt32(GorupId);
                var Regions = string.Join(",", jkEntityModel.AuthGroupRegions.Where(x => x.GroupId == iGorupId).ToList().Select(x => x.RegionId)); ;
                return Json(new
                {
                    Regions = Regions
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Regions = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Menu List

        public ActionResult Menu()
        {
            ViewMenuModel objViewMenuModel = new ViewMenuModel();

            if (TempData["Message"] != null && TempData["MessageType"] != null)
            {
                objViewMenuModel.Message = Convert.ToString(TempData["Message"]);
                objViewMenuModel.MessageType = Convert.ToString(TempData["MessageType"]);
                TempData["Message"] = null;
                TempData["MessageType"] = null;
            }

            ViewBag.StatusList = new SelectList(new[]
                                          {
                                              new {ID="-1",Name="--All--"},
                                              new {ID="1",Name="Active"},
                                              new{ID="0",Name="In Active"},
                                          },
                         "ID", "Name", -1);

            objViewMenuModel.lstMenus = FillMenu(objViewMenuModel);
            return View(objViewMenuModel);
        }


        public ActionResult SearchMenu(int? PageNo, int PageSize = 0, string Sortby = "", string SortOrder = "", string FilterMenuName = "", string FilterStatus = "")
        {

            ViewMenuModel objViewMenuModel = new ViewMenuModel();
            int CurrentPage = 1;

            PageSize = 10;
            CurrentPage = (PageNo ?? 1);
            objViewMenuModel.PageSize = PageSize;
            objViewMenuModel.SortBy = Sortby;
            objViewMenuModel.SortOrder = SortOrder;
            objViewMenuModel.CurrentPage = CurrentPage;
            List<ErrorModel> objError = null;

            var objReturnModel = _userService.SearchMenu(objViewMenuModel);
            if (objError.Count > 0 && objReturnModel == null)
            {

                objViewMenuModel.MessageType = objError[0].ErrorMessage;

            }
            else
            {
                objViewMenuModel = objReturnModel;
                //TotalRecords = objReturnModel.ProductList[0].TotalCount;
            }
            return PartialView("_MenuList", objViewMenuModel);

        }


        private List<MenuModel> FillMenu(ViewMenuModel objViewMenuModel)
        {
            List<MenuModel> lstMenu = new List<MenuModel>();

            var objReturnModel = _userService.FillMenu();

            lstMenu = objReturnModel.lstMenu;

            ViewBag.MenuList = new SelectList(lstMenu, "MenuName", "MenuName", objViewMenuModel.FilterMenuName);

            return lstMenu;
        }

        #endregion

        #region Assign Menu

        public ActionResult UserPermission()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Permission");

            AssignMenuModel objAssignMenuModel = new AssignMenuModel();

            ViewBag.RoleList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRole.ToString()), "Value", "Text");

            //get Tree View of Menu
            var objReturnModel = _userService.GetRolebasedMenuAccessDetail(0);
            objAssignMenuModel.RoleBasedMenuAccessList = objReturnModel.lstRoleBasedMenuAccessDetail;
            // ViewBag.displayTreeView = MakeRoleBasedTree();
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["Message"]) ?? "") && !String.IsNullOrEmpty(Convert.ToString(TempData["MessageType"]) ?? ""))
            {
                objAssignMenuModel.Message = TempData["Message"].ToString();
                objAssignMenuModel.MessageType = TempData["MessageType"].ToString();
                TempData["Message"] = null;
                TempData["MessageType"] = null;
                TempData.Remove("Message");
                TempData.Remove("MessageType");
            }
            objAssignMenuModel.RoleId = 0;
            return View(objAssignMenuModel);
        }

        public ActionResult AssignMenu()
        {
            AssignMenuModel objAssignMenuModel = new AssignMenuModel();

            ViewBag.RoleList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRole.ToString()), "Value", "Text");

            //get Tree View of Menu
            var objReturnModel = _userService.GetRolebasedMenuAccessDetail(0);
            objAssignMenuModel.RoleBasedMenuAccessList = objReturnModel.lstRoleBasedMenuAccessDetail;
            // ViewBag.displayTreeView = MakeRoleBasedTree();
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["Message"]) ?? "") && !String.IsNullOrEmpty(Convert.ToString(TempData["MessageType"]) ?? ""))
            {
                objAssignMenuModel.Message = TempData["Message"].ToString();
                objAssignMenuModel.MessageType = TempData["MessageType"].ToString();
                TempData["Message"] = null;
                TempData["MessageType"] = null;
                TempData.Remove("Message");
                TempData.Remove("MessageType");
            }
            objAssignMenuModel.RoleId = 0;
            return View(objAssignMenuModel);
        }

        [HttpPost]
        public ActionResult AssignMenu(AssignMenuModel objAssignMenuModel)
        {
            objAssignMenuModel.IsEnable = true;
            objAssignMenuModel.CreatedBy = 0;
            objAssignMenuModel.RoleBasedMenuXml = GetBulkXML(objAssignMenuModel.RoleBasedMenuAccessList);
            var objReturnModel = _userService.InsertUpdateAssignMenu(objAssignMenuModel);

            return Redirect("UserPermission");
        }


        [HttpPost]
        public JsonResult GetRoleBasedAssignedMenu(int roleid)
        {
            var classesMenu = _userService.GetRolebasedMenuAccessDetail(roleid);
            return Json(classesMenu.lstRoleBasedMenuAccessDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ARPermission()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Permission");

            AssignARModel objAssignMenuModel = new AssignARModel();

            ViewBag.RoleList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRole.ToString()), "Value", "Text");

            //get Tree View of Menu
            var objReturnModel = _userService.GetARRolebasedMenuAccessDetail(0);
            objAssignMenuModel.RoleBasedMenuAccessList = objReturnModel.lstRoleBasedMenuAccessDetail;
            // ViewBag.displayTreeView = MakeRoleBasedTree();
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["Message"]) ?? "") && !String.IsNullOrEmpty(Convert.ToString(TempData["MessageType"]) ?? ""))
            {
                objAssignMenuModel.Message = TempData["Message"].ToString();
                objAssignMenuModel.MessageType = TempData["MessageType"].ToString();
                TempData["Message"] = null;
                TempData["MessageType"] = null;
                TempData.Remove("Message");
                TempData.Remove("MessageType");
            }
            objAssignMenuModel.RoleId = 0;
            return View(objAssignMenuModel);
        }


        [HttpPost]
        public ActionResult AssignARMenu(AssignARModel objAssignMenuModel)
        {
            objAssignMenuModel.IsEnable = true;
            objAssignMenuModel.CreatedBy = 0;
            objAssignMenuModel.RoleBasedMenuXml = GetBulkXML(objAssignMenuModel.RoleBasedMenuAccessList);
            var objReturnModel = _userService.InsertUpdateAssignARMenu(objAssignMenuModel);

            return Redirect("ARPermission");
        }


        [HttpPost]
        public JsonResult GetRoleBasedAssignedAR(int roleid)
        {
            var classesMenu = _userService.GetARRolebasedMenuAccessDetail(roleid);
            return Json(classesMenu.lstRoleBasedMenuAccessDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EDPermission()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("UserPermission", "Administration", new { area = "Portal" }), "User Permission");

            AssignEDModel objAssignMenuModel = new AssignEDModel();

            ViewBag.RoleList = new SelectList(_commonService.DropDownListByName(MasterDropName.AuthRole.ToString()), "Value", "Text");

            //get Tree View of Menu
            var objReturnModel = _userService.GetEDRolebasedMenuAccessDetail(0);
            objAssignMenuModel.RoleBasedMenuAccessList = objReturnModel.lstRoleBasedMenuAccessDetail;
            // ViewBag.displayTreeView = MakeRoleBasedTree();
            if (!String.IsNullOrEmpty(Convert.ToString(TempData["Message"]) ?? "") && !String.IsNullOrEmpty(Convert.ToString(TempData["MessageType"]) ?? ""))
            {
                objAssignMenuModel.Message = TempData["Message"].ToString();
                objAssignMenuModel.MessageType = TempData["MessageType"].ToString();
                TempData["Message"] = null;
                TempData["MessageType"] = null;
                TempData.Remove("Message");
                TempData.Remove("MessageType");
            }
            objAssignMenuModel.RoleId = 0;
            return View(objAssignMenuModel);
        }


        [HttpPost]
        public ActionResult AssignEDMenu(AssignEDModel objAssignMenuModel)
        {
            objAssignMenuModel.IsEnable = true;
            objAssignMenuModel.CreatedBy = 0;
            objAssignMenuModel.RoleBasedMenuXml = GetBulkXML(objAssignMenuModel.RoleBasedMenuAccessList);
            var objReturnModel = _userService.InsertUpdateAssignEDMenu(objAssignMenuModel);

            return Redirect("EDPermission");
        }


        [HttpPost]
        public JsonResult GetRoleBasedAssignedED(int roleid)
        {
            var classesMenu = _userService.GetEDRolebasedMenuAccessDetail(roleid);
            return Json(classesMenu.lstRoleBasedMenuAccessDetail, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Save Menu

        public ActionResult SaveMenu(string prm)
        {
            SaveMenuModel objSaveMenuModel = new SaveMenuModel();
            int menuid = 0;
            if (!String.IsNullOrEmpty(prm))
            {

                string inputParameter = _encryptDecrypt.Decrypt(prm).ToString().Replace("?", "").ToLower();

                if (inputParameter.IndexOf("pid") > -1)
                {
                    foreach (var item in inputParameter.Split('&'))
                    {
                        if (item.IndexOf("pid") > -1)
                        {
                            int.TryParse(item.Replace("pid=", ""), out menuid);
                        }
                    }
                }
            }
            MenuModel objMenu = new MenuModel();

            objSaveMenuModel = menuid != 0 ? _userService.GetMenuById(new PostMenuModel { MenuId = menuid }) : new SaveMenuModel();
            if (menuid == 0)
            {
                objSaveMenuModel.IsEnable = true;
            }

            objSaveMenuModel.SelectedPageName = objSaveMenuModel.PageName;
            objSaveMenuModel.MenuUrl = objSaveMenuModel.MenuUrl;
            if (String.IsNullOrEmpty(objSaveMenuModel.SelectedPageName))
            {
                objSaveMenuModel.SelectedPageName = string.Empty;
            }
            FillDropDown(objSaveMenuModel);
            return View(objSaveMenuModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMenu(SaveMenuModel objMenu, HttpPostedFileBase fileUpload)
        {
            string MenuImageDirectory = "MenuImageDirectory";
            objMenu.OperationType = "A";
            if (objMenu.MenuId != 0)
            {
                objMenu.OperationType = "E";
            }

            objMenu.MenuUrl = !string.IsNullOrEmpty(objMenu.MenuUrl) ? objMenu.MenuUrl.Trim() : string.Empty;
            objMenu.PageName = objMenu.SelectedPageName;
            objMenu.CreatedOn = DateTime.Now;
            objMenu.CreatedBy = 0;

            //if (fileUpload != null && fileUpload.ContentLength > 0)
            //{
            //    if (fileUpload.ContentLength > CommonUtils.MenuImageMaxFileSize)
            //    {
            //        objMenu.MessageType = EnumModel.MessageType.error.ToString();
            //        FillDropDown(objMenu);
            //        return View(objMenu);
            //    }
            //    else
            //    {
            //        var filepath = fileUpload.FileName;
            //        objMenu.MenuImageUrl = MenuImageDirectory + Path.GetFileName(fileUpload.FileName);
            //    }
            //}

            var objReturnModel = _userService.InsertUpdateMenu(objMenu);

            //if (objError != null && objError.Count > 0)
            //{
            //    TempData["Message"] = string.Format(CommonResource.msgSaveError, "Menu");
            //    TempData["MessageType"] = EnumModel.MessageType.success.ToString();
            //}
            //else if (objReturnModel != null)
            //{
            //    if (fileUpload != null && fileUpload.ContentLength > 0)
            //    {
            //        MenuImageDirectory = Server.MapPath(MenuImageDirectory);
            //        //Upload zip file in folder 
            //        if (!CommonUtils.CheckFileExist(MenuImageDirectory))
            //            CommonUtils.CreateDirectory(MenuImageDirectory);
            //        fileUpload.SaveAs(Path.Combine(MenuImageDirectory, Path.GetFileName(fileUpload.FileName)));
            //    }

            //    TempData["Message"] = string.Format(CommonResource.msgSaveSuccess, "Menu");
            //    TempData["MessageType"] = EnumModel.MessageType.success.ToString();

            //    if (HttpRuntime.Cache["AllMenus"] != null)
            //        HttpRuntime.Cache.Remove("AllMenus");
            //    List<MenuModel> lstMenus = new List<MenuModel>();
            //    lstMenus = _userService.GetAllMenu().lstMenu;
            //    HttpRuntime.Cache.Insert("AllMenus", lstMenus, null, DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);

            //    Session["Menu"] = _userService.GetAssignMenusByAccess(0).lstMenu;
            //    return RedirectToAction("Menu", "Menu");
            //}
            FillDropDown(objMenu);
            return View(objMenu);
        }

        private void FillDropDown(SaveMenuModel objMenuSave)
        {
            var menuList = _userService.GetAllMenu().lstMenu;
            List<MenuModel> lstPageName = (from a in menuList
                                           select a).ToList();
            MenuModel lstMenuItem = new MenuModel();
            lstMenuItem.MenuName = "N/A";
            lstMenuItem.MenuId = 0;
            lstPageName.Insert(0, lstMenuItem);
            ViewBag.ParentMenuList = new SelectList(lstPageName, "MenuId", "MenuName", objMenuSave.ParentMenuId);
            objMenuSave.SelectedPageName = String.IsNullOrWhiteSpace(objMenuSave.SelectedPageName) ? string.Empty : objMenuSave.SelectedPageName;
            ViewBag.PageNameList = new MultiSelectList(lstPageName, "MenuId", "MenuName", objMenuSave.SelectedPageName.Split(",".ToCharArray()));
            Assembly asm = Assembly.GetExecutingAssembly();

            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    //.Where(type => type.IsDefined(typeof(MenuAccessAttribute)))
                    .OrderBy(x => x.DeclaringType.Name).ThenBy(x => x.Name).ToList()
                    .Select(x => new { Name = "/" + x.DeclaringType.Name.Replace("Controller", "") + "/" + x.Name }).ToList();


            ViewBag.MenuURLList = new SelectList(controlleractionlist, "Name", "Name", objMenuSave.MenuUrl);
        }

        #endregion

        #region Inspection 
        #region Action Method
        //[HttpGet]
        public ActionResult Inspection()
        {
            var inspectionList = (from m in jkEntityModel.FormTemplates
                                  join tt in jkEntityModel.FormTemplateTypes on m.FormTemplateTypeId equals tt.FormTemplateTypeId
                                  join st in jkEntityModel.ServiceTypeLists on m.ServiceTypeListId equals st.ServiceTypeListid
                                  join at in jkEntityModel.AccountTypeLists on m.AccountTypeListId equals at.AccountTypeListId
                                  select new TemplateListViewModel
                                  {
                                      TemplateId = m.FormTemplateId,
                                      TemplateName = m.FormName,
                                      TemplateTypeId = m.FormTemplateTypeId,
                                      TemplateTypeName = tt.Name,
                                      HeaderCount = m.FormItemTemplates.Count,
                                      TemplateDescription = m.Description,
                                      AccountTypeName = at.Name,
                                      ServiceTypeName = st.name
                                  }).OrderBy(x => x.TemplateId).ToList();

            return View(inspectionList);
        }

        [HttpGet]
        public ActionResult AddInspection()
        {
            ViewBag.CurrentMenu = "AdministrationUsers";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AddInspection", "Administration", new { area = "Portal" }), "Inspection");
            BreadCrumb.Add(Url.Action("AddInspection", "Administration", new { area = "Portal" }), "Add Inspection");

            TemplateViewModel objTempItem = new TemplateViewModel();
            objTempItem.PageStepNo = 1;
            FillDropDownLists(objTempItem);

            return View(objTempItem);
        }

        [HttpGet]
        public ActionResult EditInspection(string id)
        {
            TemplateViewModel objTempItem = new TemplateViewModel();
            objTempItem.TemplateId = Convert.ToInt32(id);

            #region Get Template Details by TemplateId

            objTempItem = (from m in jkEntityModel.FormTemplates
                           join st in jkEntityModel.ServiceTypeLists on m.ServiceTypeListId equals st.ServiceTypeListid
                           join at in jkEntityModel.AccountTypeLists on m.AccountTypeListId equals at.AccountTypeListId
                           where m.FormTemplateId == objTempItem.TemplateId
                           select new TemplateViewModel
                           {
                               TemplateId = m.FormTemplateId,
                               Name = m.FormName,
                               Description = m.Description,
                               TemplateTypeId = m.FormTemplateTypeId,
                               AccountTypeListId = m.AccountTypeListId,
                               ServiceTypeListId = m.ServiceTypeListId,
                               AccountTypeName = at.Name,
                               ServiceTypeName = st.name,
                               //Need to Change Type Name 
                               TemplateTypeName = m.FormName,
                               PageStepNo = 1
                           }).FirstOrDefault();

            if (TempData["PageStepNo"] != null)
            {
                objTempItem.PageStepNo = Convert.ToInt32(TempData["PageStepNo"]);
                TempData["PageStepNo"] = null;
            }

            objTempItem.lstTemplateDetailListViewModel = (from m in jkEntityModel.FormItemTemplates
                                                          join ft in jkEntityModel.FormItemTypes on m.FormItemTypeId equals ft.FormItemTypeId
                                                          where m.FormTemplateId == objTempItem.TemplateId
                                                          orderby m.SectionName
                                                          select new TemplateDetailListViewModel
                                                          {
                                                              FormTypeId = (int)m.FormItemTypeId,
                                                              FormTypeName = ft.Name,
                                                              TemplateId = m.FormTemplateId,
                                                              TemplateName = m.FormTemplate.FormName,
                                                              LableName = m.FormValue,
                                                              Order = m.ItemOrder,
                                                              HeaderName = m.SectionName,
                                                              HeaderOrder = m.SectionOrder,
                                                              TemplateDetailId = m.FormItemTemplateId
                                                          }).OrderBy(x => x.HeaderOrder).OrderBy(x => x.Order).ToList();

            FillDropDownLists(objTempItem);
            #endregion

            return View("AddInspection", objTempItem);
        }

        //[HttpPost]
        //public ActionResult AddEdit(DocumentTemplateViewModel model)
        //{
        //    FillDropDownLists(model);
        //    return View("DocumentTemplate", model);
        //}

        [HttpPost]
        public ActionResult AddEditInspection(TemplateViewModel model)
        {
            using (var context = new jkDatabaseEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        switch (model.PageStepNo)
                        {
                            case 2:
                                {
                                    #region code for first form
                                    FormTemplate objDocumentTemplate = (from a in context.FormTemplates where a.FormTemplateId == model.TemplateId select a).FirstOrDefault();
                                    if (objDocumentTemplate != null && objDocumentTemplate.FormTemplateId > 0)
                                    {
                                        objDocumentTemplate.Description = model.Description;
                                        objDocumentTemplate.FormName = model.Name;
                                        objDocumentTemplate.FormTemplateTypeId = model.TemplateTypeId;

                                        objDocumentTemplate.AccountTypeListId = model.AccountTypeListId;
                                        objDocumentTemplate.ServiceTypeListId = model.ServiceTypeListId;

                                        objDocumentTemplate.ModifiedDate = DateTime.Now;
                                        context.Entry(objDocumentTemplate).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        objDocumentTemplate = new FormTemplate();
                                        objDocumentTemplate.Description = model.Description;
                                        objDocumentTemplate.FormName = model.Name;
                                        objDocumentTemplate.FormTemplateTypeId = model.TemplateTypeId;
                                        objDocumentTemplate.AccountTypeListId = model.AccountTypeListId;
                                        objDocumentTemplate.ServiceTypeListId = model.ServiceTypeListId;

                                        objDocumentTemplate.CreatedDate = DateTime.Now;
                                        objDocumentTemplate.IsEnable = true;
                                        context.FormTemplates.Add(objDocumentTemplate);
                                        context.SaveChanges();
                                        //Inserted Record ID
                                        model.TemplateId = objDocumentTemplate.FormTemplateId;
                                    }

                                    break;
                                    #endregion
                                }
                            case 3:
                                {
                                    #region code for second form
                                    List<FormItemTemplate> objDocumentDetailTemplate = (from m in jkEntityModel.FormItemTemplates
                                                                                        where m.FormTemplateId == model.TemplateId
                                                                                        orderby m.SectionOrder
                                                                                        select m).ToList();

                                    //Remove Item By DB
                                    foreach (FormItemTemplate item in objDocumentDetailTemplate)
                                    {
                                        var stuffToRemove = model.lstTemplateDetailListViewModel.SingleOrDefault(s => s.TemplateDetailId == item.FormItemTemplateId);
                                        if (stuffToRemove == null)
                                        {
                                            context.FormItemTemplates.Remove(item);
                                            context.SaveChanges();
                                        }
                                    }

                                    //Add & Update Exiting Item
                                    foreach (TemplateDetailListViewModel item in model.lstTemplateDetailListViewModel)
                                    {
                                        if (item.TemplateDetailId > 0)
                                        {
                                            FormItemTemplate dbItem = (from a in context.FormItemTemplates where a.FormItemTemplateId == item.TemplateDetailId select a).FirstOrDefault();
                                            dbItem.FormItemTypeId = item.FormTypeId;
                                            dbItem.SectionName = item.HeaderName;
                                            dbItem.SectionOrder = (int)item.HeaderOrder;
                                            dbItem.FormValue = item.LableName;
                                            dbItem.ItemOrder = (int)item.Order;
                                            dbItem.ModifiedDate = DateTime.Now;
                                            dbItem.IsEnable = true;
                                            context.Entry(dbItem).State = System.Data.Entity.EntityState.Modified;
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            FormItemTemplate newItem = new FormItemTemplate();
                                            newItem.FormItemTypeId = item.FormTypeId;
                                            newItem.FormTemplateId = model.TemplateId;
                                            newItem.SectionName = item.HeaderName;
                                            newItem.SectionOrder = (int)item.HeaderOrder;
                                            newItem.FormValue = item.Value;
                                            newItem.ItemOrder = (int)item.Order;
                                            newItem.CreatedDate = DateTime.Now;
                                            newItem.IsEnable = true;
                                            context.FormItemTemplates.Add(newItem);
                                            context.SaveChanges();
                                        }
                                    }

                                    break;
                                    #endregion
                                }
                            default:
                                {
                                    return RedirectToAction("Inspection");
                                }
                        }
                        TempData["PageStepNo"] = model.PageStepNo;
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Msg = ex.Message;
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return RedirectToAction("EditInspection", "Administration", new { id = model.TemplateId });
        }
        #endregion

        #region Private Method 
        private void FillDropDownLists(TemplateViewModel model)
        {
            ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name", model.TemplateTypeId);
            ViewBag.FormTypeList = new SelectList(jkEntityModel.FormItemTypes.Where(x => x.Type > 0 && x.MasterValue != "").ToList(), "FormItemTypeId", "Name");

            ViewBag.lstFormTypeJson = (from a in jkEntityModel.FormItemTypes where a.Type > 0 select new { FormTypeName = a.Name, FormTypeId = a.FormItemTypeId, MasterValue = a.MasterValue }).ToList();

            ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name", model.AccountTypeListId);
            ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name", model.ServiceTypeListId);
        }
        #endregion


        #region Template
        [HttpGet]
        public ActionResult TemplateArea()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Inspection");
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Area");

            TemplateAreaViewModel model = new TemplateAreaViewModel() { ActionType = "SA" };
            TemplateItemViewModel modelItem = new TemplateItemViewModel() { ActionType = "SA" };

            ViewBag.ItemList = new SelectList(_userService.SaveTemplateItem(modelItem).templateItemList, "TemplateAreaItemId", "ItemName");

            return View(_userService.SaveTemplateArea(model));
        }

        [HttpPost]
        public ActionResult TemplateArea(TemplateAreaViewModel model)
        {
            model.CreatedBy = LoginUserId;
            model.IsEnable = true;
            //model.ActionType = model.TemplateAreaId > 0 ? "U" : "I";
            _userService.SaveTemplateArea(model);
            return Redirect("TemplateArea");
        }

        [HttpGet]
        public ActionResult TemplateItem()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Inspection");
            BreadCrumb.Add(Url.Action("TemplateArea", "Administration", new { area = "Portal" }), "Item");

            TemplateItemViewModel modelItem = new TemplateItemViewModel() { ActionType = "SA" };

            return View(_userService.SaveTemplateItem(modelItem));
        }

        [HttpPost]
        public ActionResult TemplateItem(TemplateItemViewModel model, FormCollection collection)
        {
            model.CreatedBy = LoginUserId;
            model.IsEnable = true;
            model.FormItemType = Convert.ToInt32(collection["ddlItemType"]);
            //model.ActionType = model.TemplateAreaItemId > 0 ? "U" : "I";
            _userService.SaveTemplateItem(model);
            return Redirect("TemplateItem");
        }

        [HttpGet]
        public ActionResult Template()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Template", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Template", "Administration", new { area = "Portal" }), "Inspection");
            BreadCrumb.Add(Url.Action("Template", "Administration", new { area = "Portal" }), "Template");

            var formTemplateViewModel = new FormTemplateViewModel
            {
                IsEnable = true,
                SortBy = "AccountTypeListName"
            };
            formTemplateViewModel = _templateService.GetTemplates(formTemplateViewModel);
            return View(formTemplateViewModel);
        }

        [HttpPost]
        public ActionResult Template(NewFormTemplateViewModel model)
        {
            model.CreatedBy = LoginUserId;
            model.IsEnable = true;
            _userService.SaveTemplate(model);
            return Redirect("Template");
        }

        public ActionResult TemplateDetail(int id = -1)
        {
            if (id < 0)
            {
                var model = new TemplateAreaViewModel() { ActionType = "SA" };
                ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name");
                ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
                ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name");
                ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
                ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");
                var jobList = _jobService.GetJobListByRegionId(Convert.ToInt32(Session["SelectedRegionId"]));
                ViewBag.JobList = new SelectList(jobList, "JobId", "CustomerName");
            }

            int? formTemplateId = id;
            ViewBag.FormTemplateId = formTemplateId;
            var formTemplateViewModel = new FormTemplateDetailModel();

            if (id > 0)
            {
                formTemplateViewModel = _templateService.GetTemplate(id);
                {
                    var model = new TemplateAreaViewModel { ActionType = "SA" };
                    ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name");
                    ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name", formTemplateViewModel.AccountTypeListId);
                    ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name", formTemplateViewModel.ServiceTypeListId);
                    ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable), "TemplateAreaId", "AreaName");
                    ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable), "TemplateAreaItemId", "ItemName");
                    var jobList = _jobService.GetJobListByRegionId(Convert.ToInt32(Session["SelectedRegionId"]));
                    ViewBag.JobList = new SelectList(jobList, "JobId", "CustomerName");
                    ViewBag.JobId = formTemplateViewModel.JobId;
                }
            }
            return View(formTemplateViewModel);
        }

        [HttpGet]
        public ActionResult TemplateQuestion()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TemplateQuestion", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("TemplateQuestion", "Administration", new { area = "Portal" }), "Inspection");
            BreadCrumb.Add(Url.Action("TemplateQuestion", "Administration", new { area = "Portal" }), "Item");

            TemplateQuestionViewModel modelItem = new TemplateQuestionViewModel() { ActionType = "SA" };

            return View(_userService.SaveTemplateQuestion(modelItem));
        }

        [HttpPost]
        public ActionResult TemplateQuestion(TemplateQuestionViewModel model)
        {
            model.CreatedBy = LoginUserId;
            model.IsEnable = true;
            //model.ActionType = model.TemplateAreaItemId > 0 ? "U" : "I";
            _userService.SaveTemplateQuestion(model);
            return Redirect("TemplateQuestion");
        }

        public JsonResult AddItemtoArea(int _TemplateAreaId, int _TemplateAreaItemId)
        {
            var item = _templateService.GetTemplateAreaItem(_TemplateAreaItemId);
            var temp = new TemplateAreaItemModel
            {
                TemplateAreaId = _TemplateAreaId,
                TemplateAreaItemId = _TemplateAreaItemId,
                FormItemType = item.FormItemType,
                FormItemValue = item.FormItemValue,
                CreatedOn = DateTime.UtcNow
            };
            var formTemplateViewModel = _templateService.AddOrUpdateTemplateAreaItemToArea(temp);
            return Json(new { aadata = "Save" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemtoInspectionArea(int _SectionId, int _SectionItemId)
        {
            var item = _templateService.GetTemplateAreaItem(_SectionItemId);
            var temp = new InspectionFormItemModel
            {
                InspectionFormSectionId = _SectionId,
                InspectionFormItemId = _SectionItemId,
                FormItemValue = item.FormItemValue,
                FormItemType = item.FormItemType,
                CreatedDate = DateTime.UtcNow
            };
            var formTemplateViewModel = _inspectionService.AddOrUpdateInpsectionFormItem(temp);
            return Json(new { aadata = "Save" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAreaItem(int ItemId, int TempArea)
        {
            var model = new TemplateAreaItemModel
            {
                TemplateAreaId = TempArea,
                TemplateAreaItemId = ItemId
            };

            _templateService.DeleteTemplateAreaItemFromArea(model);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInspectionAreaItem(int ItemId)
        {
            _inspectionService.DeleteInspectionFormItem(ItemId);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInspectionSection(int SectionId)
        {
            _inspectionService.DeleteInspectionFormSection(SectionId);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTemplateArea(int Areaid, int formTempId)
        {
            var Data = new TemplateAreaModel
            {
                TemplateAreaId = Areaid
            };
            _templateService.DeleteTemplateAreaFromTemplate(formTempId, Data);
            return Json(new { aadata = "Deleted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemList(int id)
        {
            var Data = _templateService.GetItemList(id);

            var jsonResult = Json(new { aadata = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetTemplateAreaAdd(int id, int TempId)
        {
            var Data = _templateService.AddOrupdateTemplateAreaModel(TempId, id);
            var jsonResult = Json(new { aadata = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region Inspection 
        public ActionResult InspectionResult(int id = 0)
        {
            InspectionResultModel model = new InspectionResultModel();
            model.AccountType = "Gurlaza";
            model.AcctType = "Restaurant";
            model.Address = "40 Patel Marg Kukshi";
            model.ContactName = "application name";
            model.FranchiseeName = "Franchisee Name";
            model.Frequency = "6 time";
            model.InspectionId = 1;
            model.InspectionResultId = 1;
            model.InspectorName = "Rakesh Patidar";
            model.ListFromTemplateArea = new List<FromTemplateAreaAPIModel>();
            return View(model);
        }

        public ActionResult AssignedInspections()
        {
            var model = new ViewInspectionListModel
            {
                IsEnable = true
            };
            // TODO: Use GetInspectionFormList
            var result = new List<InspectionModel>();
            return View(result);
        }

        public ActionResult InspectionsList()
        {
            ViewBag.CurrentMenu = "Administration";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Template", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Template", "Administration", new { area = "Portal" }), "Inspection");
            // TODO: Use GetInspectionFormList
            // BreadCrumb.Add(Url.Action("Template", "InspectionsList", new { area = "Portal" }), "Inspections List");
            return View();
        }

        public JsonResult getInspections()
        {
            var model = new ViewInspectionListModel
            {
                IsEnable = true
            };
            // TODO: Use GetInspectionFormList
            // var result = _inspectionService.GetInspectionFormList(model);
            var result = new List<InspectionModel>();

            var jsonResult = Json(new { aadata = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetInspectionSectionAdd(int id, int InspForm)
        {
            var item = _templateService.GetTemplateArea(id);
            var model = new InspectionFormSectionModel
            {
                InspectionFormSectionId = id,
                InspectionFormId = InspForm,
                SectionName = item.AreaName,
                CreatedDate = DateTime.UtcNow
            };
            var Data = _inspectionService.AddOrUpdateInspectionFormSection(model);
            var jsonResult = Json(new { aadata = Data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetjobDetailFranchisee(int id)
        {
            var result = _jobService.GetJobList().Where(x => x.JobId == id);
            var jsonResult = Json(new { aadata = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult DeleteTemplate(int id)
        {
            _templateService.DeleteFormTemplate(id);
            return Json(new { aadata = "Delete" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteInspection(int id)
        {
            // TODO: Implement DeleteInspectionForm
           //  _inspectionService.DeleteInspection(id);
            return Json(new { aadata = "Delete" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TemplateDetail(FormTemplateDetailModel FullCustomerViewModel, FormCollection collection)
        {
            int id = Convert.ToInt32(collection["JobId"]);
            int tempid = Convert.ToInt32(collection["FormTemplateId"]);
            if (tempid != 0)
            {
                var formTemplateViewModel = _templateService.GetTemplate(tempid);
                if (id != 0)
                {
                    var job = _jobService.GetJob(id);
                    var result = _templateService.AssignInspectionFromTemplate(formTemplateViewModel, job);
                    var model = new TemplateAreaViewModel() { ActionType = "SA" };
                    ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name");
                    ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
                    ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name");
                    ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
                    ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");
                    var jobList = _jobService.GetJobListByRegionId(Convert.ToInt32(Session["SelectedRegionId"]));
                    ViewBag.JobList = new SelectList(jobList, "JobId", "CustomerName");
                    return View(FullCustomerViewModel);
                }
                else
                {
                    var Data = new FormTemplateModel
                    {
                        FormTemplateId = tempid,
                        AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]),
                        ServiceTypeListId = Convert.ToInt32(1),
                        FormTemplateTypeId = Convert.ToInt32(collection["TemplateType"]),
                        Description = FullCustomerViewModel.Description,
                        FormName = FullCustomerViewModel.FormName,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow
                    };
                    var Result = _templateService.AddOrUpdateFormTemplate(Data);
                    TemplateAreaViewModel model = new TemplateAreaViewModel() { ActionType = "SA" };
                    ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name");
                    ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
                    ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name");
                    ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
                    ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");
                    var jobList = _jobService.GetJobListByRegionId(Convert.ToInt32(Session["SelectedRegionId"]));
                    ViewBag.JobList = new SelectList(jobList, "JobId", "CustomerName");
                    return RedirectToAction("TemplateDetail", "Administration", new { area = "portal", id = tempid });
                }
            }
            else if (tempid == 0)
            {
                var Data = new FormTemplateModel
                {
                    FormTemplateId = 0,
                    AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]),
                    ServiceTypeListId = Convert.ToInt32(collection["ServiceType"]),
                    FormTemplateTypeId = Convert.ToInt32(collection["TemplateType"]),
                    Description = FullCustomerViewModel.Description,
                    FormName = FullCustomerViewModel.FormName,
                    CreatedOn = DateTime.UtcNow
                };
                var Result = _templateService.AddOrUpdateFormTemplate(Data);
                TemplateAreaViewModel model = new TemplateAreaViewModel() { ActionType = "SA" };
                ViewBag.FormTemplateTypeList = new SelectList(jkEntityModel.FormTemplateTypes.Where(x => x.Type > 0).ToList(), "FormTemplateTypeId", "Name");
                ViewBag.AccountTypeList = new SelectList(jkEntityModel.AccountTypeLists.ToList().OrderBy(x => x.Name), "AccountTypeListId", "Name");
                ViewBag.ServiceTypeList = new SelectList(jkEntityModel.ServiceTypeLists.ToList().OrderBy(x => x.name), "ServiceTypeListId", "name");
                ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
                ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");
                var jobList = _jobService.GetJobListByRegionId(Convert.ToInt32(Session["SelectedRegionId"]));
                ViewBag.JobList = new SelectList(jobList, "JobId", "CustomerName");
                return RedirectToAction("TemplateDetail", "Administration", new { area = "portal", id = Result.FormTemplateId });
            }

            return View(FullCustomerViewModel);
        }

        public JsonResult AssignInspection(int id, int TempId)
        {

            var formTemplateViewModel = _templateService.GetTemplate(TempId);
            var job = _jobService.GetJob(id);
            var result = _templateService.AssignInspectionFromTemplate(formTemplateViewModel, job);
            var jsonResult = Json(new { aadata = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion
        #endregion

        #region Feature Email IDs
        public ActionResult FeatureEmail()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FeatureEmail", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("FeatureEmail", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("FeatureEmail", "Administration", new { area = "Portal" }), "Feature Email");
            FeatureTypeEmailViewModel model = new FeatureTypeEmailViewModel() { ActionType = "SA" };
            model = _userService.SaveFeatureEmail(model);

            ViewBag.FeatureTypeList = new SelectList(_commonService.DropDownListByName(MasterDropName.FeatureType.ToString()), "Value", "Text");

            model.FromEmail = _claimView.GetCLAIM_PERSON_INFORMATION().Email;

            return View(model);
        }

        [HttpPost]
        public ActionResult FeatureEmail(FeatureTypeEmailViewModel model)
        {
            model.CreatedBy = 1;
            model.ActionType = model.FeatureTypeEmailId > 0 ? "U" : "I";
            model = _userService.SaveFeatureEmail(model);

            return Redirect("FeatureEmail");
        }

        #endregion


        public ActionResult UpdateLatLong()
        {
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult InspectionDetail(int Id = -1)
        {
            TemplateAreaViewModel model = new TemplateAreaViewModel() { ActionType = "SA" };
            ViewBag.AreaList = new SelectList(_userService.SaveTemplateArea(model).templateAreaList.OrderBy(x => x.AreaName).Where(o => o.IsEnable == true), "TemplateAreaId", "AreaName");
            ViewBag.TempAreaIteam = new SelectList(jkEntityModel.TemplateAreaItems.ToList().OrderBy(x => x.ItemName).Where(o => o.IsEnable == true), "TemplateAreaItemId", "ItemName");

            // TODO: Implement GetInspectionFormHistory
            // var Model = _inspectionService.GetInspectionFormReport(Id);
            var Model = new InspectionFormModel();

            return View(Model);
        }


        public JsonResult UpdateLatLongProcess(bool active, bool inactive, int regionId)
        {
            //https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8 
            //string _key = "&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";
            //string basrUrl = "https://maps.googleapis.com/maps/api/geocode/json?address="++"&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";

            CommonGMapAddressViewModel oCommonAddress = _commonService.GetGMapAddresses(active, inactive, regionId);

            foreach (var _address in oCommonAddress.lstDistinctAddress)
            {
                var destination_latLong = GetLatLongByAddress(_address.CAddress);

                if (destination_latLong.status == "OK")
                {

                    foreach (var ac in oCommonAddress.lstAllData.Where(o => o.CAddress == _address.CAddress).ToList())
                    {
                        _commonService.UpdateGMapAddress(ac.AddressId, decimal.Parse(destination_latLong.results[0].geometry.location.lat.ToString()), decimal.Parse(destination_latLong.results[0].geometry.location.lng.ToString()));
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImageLoad(int Id)
        {
            //var Data = _inspectionService.GetInspectionFormItem(Id);
            var Data = _inspectionService.GetInspectionFormItemHistory(Id);
            return PartialView("_PartialImageView", Data);
        }

        #region 

        public ActionResult SaveTab1Data(string DisplayName, string ReportName, string Address, string City = "", string State = "", string Zip = "", string Phone = "", string RemitAddress = "", string RemitCity = "", string RemitState = "", string RemitZip = "")
        {
            if (SelectedRegionId > 0)
            {
                Region objRegion = new Region();
                objRegion.RegionId = SelectedRegionId;
                objRegion.Displayname = DisplayName;
                objRegion.ReportName = ReportName;
                objRegion.Address = Address;
                objRegion.City = City;
                objRegion.State = State;
                objRegion.PostalCode = Zip;
                objRegion.Phone = Phone;

                RemitTo objRemitTo = new RemitTo();
                objRemitTo.RegionId = SelectedRegionId;
                objRemitTo.Address = RemitAddress;
                objRemitTo.City = RemitCity;
                objRemitTo.State = RemitState;
                objRemitTo.PostalCode = RemitZip;

                compServ.UpdtedRegionRemitTo(objRegion, objRemitTo);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveTab2Data(string Id, string ConfigValue)
        {
            if (Id != null && Id != "")
            {
                compServ.SaveRegionConfigurationData(Convert.ToInt32(Id), ConfigValue);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTab3Data(string Data = "")
        {
            if (Data != null && Data != "")
            {
                var FCFeeModelList = JsonConvert.DeserializeObject<List<FCFeeModel>>(Data);
                if (FCFeeModelList != null && FCFeeModelList.Count > 0)
                {
                    foreach (var item in FCFeeModelList)
                    {
                        compServ.SaveFeeSetting(Convert.ToInt32(item.Id), item.Value, SelectedUserId);
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTab4Data(string Data = "")
        {
            if (Data != null && Data != "")
            {
                var BPPAdminFeeModel = JsonConvert.DeserializeObject<List<BPPAdminFeeModel>>(Data);
                if (BPPAdminFeeModel != null && BPPAdminFeeModel.Count > 0)
                {
                    foreach (var item in BPPAdminFeeModel)
                    {
                        compServ.SaveBBPAFeeSetting(Convert.ToInt32(item.Id), Convert.ToDecimal(item.StartAmount), Convert.ToDecimal(item.EndAmount), Convert.ToDecimal(item.Amount), LoginUserId);
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTab5Data(string Data = "")
        {
            if (Data != null && Data != "")
            {
                var EmailSettingsModel = JsonConvert.DeserializeObject<List<EmailSettingsModel>>(Data);
                if (EmailSettingsModel != null && EmailSettingsModel.Count > 0)
                {
                    foreach (var item in EmailSettingsModel)
                    {
                        compServ.SaveRegionConfigurationData(Convert.ToInt32(item.Id), item.Value);
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveTab6Data(string Data = "")
        {
            if (Data != null && Data != "")
            {
                var TransactionNumberConfigModel = JsonConvert.DeserializeObject<List<TransactionNumberConfigModel>>(Data);
                if (TransactionNumberConfigModel != null && TransactionNumberConfigModel.Count > 0)
                {
                    foreach (var item in TransactionNumberConfigModel)
                    {
                        if (Convert.ToInt32((item.Id != "" ? Convert.ToInt32(item.Id) : 0)) > 0)
                        {
                            TransactionNumberConfig TrxNumConfig = new TransactionNumberConfig();
                            TrxNumConfig.TransactionNumberConfigId = Convert.ToInt32(item.Id);
                            //TrxNumConfig.Name = item.Name;
                            TrxNumConfig.Prefix = item.NamePre;
                            TrxNumConfig.LastNumber = Convert.ToInt64((item.Number != "" ? Convert.ToInt64(item.Number) : 0));
                            compServ.SaveTransactionNumberConfigDetails(TrxNumConfig);
                        }
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTab7Data(string Data = "")
        {
            if (Data != null && Data != "")
            {
                var SettingsModel = JsonConvert.DeserializeObject<List<EmailSettingsModel>>(Data);
                if (SettingsModel != null && SettingsModel.Count > 0)
                {
                    foreach (var item in SettingsModel)
                    {
                        if (item.Value != null && item.Value != "")
                        {
                            compServ.SaveRegionConfigurationData(Convert.ToInt32(item.Id), item.Value.Replace("$", "").Replace("₹", "").Replace(",", ""));
                        }
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion



        public ActionResult CustomerParentChildSetting()
        {
            ViewBag.CurrentMenu = "CustomerParentChildSetting";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Customer Sys Setting");
            BreadCrumb.Add(Url.Action("CustomerParentChildSetting", "Administration", new { area = "Portal" }), "Customer Parent/Child Setting");
            //var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            //ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            //ViewBag.selectedRegionId = this.SelectedRegionId;



            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 1);

            //ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            ViewBag.selectedRegionId = SelectedRegionId;



            return View();
        }

        [HttpPost]
        public ActionResult CustomerParentChildSetting(FormCollection collection)
        {
            var _ParentCustomerId = !String.IsNullOrEmpty(collection["pCustomerId"]) ? int.Parse(collection["pCustomerId"]) : 0;
            var _ChildCustomerId = !String.IsNullOrEmpty(collection["cCustomerId"]) ? collection["cCustomerId"].ToString() : "";
            var _ConsolidatedInvoice = !String.IsNullOrEmpty(collection["ConsolidatedInvoice"]) ? collection["ConsolidatedInvoice"].ToString() : "";


            _companyService.SaveCustomerParentChildSetting(_ParentCustomerId, _ChildCustomerId, false, _ConsolidatedInvoice);
            return RedirectToAction("CustomerParentChildSetting", "Administration", new { area = "Portal" });
        }

        [HttpPost]
        public ActionResult ThirdPartySetting(FormCollection collection)
        {
            var _ParentCustomerId = !String.IsNullOrEmpty(collection["pCustomerId"]) ? int.Parse(collection["pCustomerId"]) : 0;
            var _ChildCustomerId = !String.IsNullOrEmpty(collection["cCustomerId"]) ? collection["cCustomerId"].ToString() : "";

            _companyService.SaveCustomerParentChildSetting(_ParentCustomerId, _ChildCustomerId, true);
            return RedirectToAction("CustomerParentChildSetting", "Administration", new { area = "Portal" });
        }

        public ActionResult ThirdPartySetting()
        {
            ViewBag.CurrentMenu = "ThirdPartySetting";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Customer Sys Setting");
            BreadCrumb.Add(Url.Action("ThirdPartySetting", "Administration", new { area = "Portal" }), "Customer Third Party Setting");
            //var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            //ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            //ViewBag.selectedRegionId = this.SelectedRegionId;

            var BankTypeList = CustomerService.GetBankTypeList();
            if (BankTypeList.Count > 0)
            {
                ViewBag.BankTypeList = new SelectList(BankTypeList, "BankTypeListId", "Name");
            }
            var State = CustomerService.GetStateList();
            if (State != null)
            {
                ViewBag.State = new SelectList(State, "StateListId", "Name");
            }
            return View("CustomerThirdPartySetting");
        }


        [HttpPost]
        public ActionResult PaymentGroup(FormCollection collection)
        {
            var _PaymentGroupName = !String.IsNullOrEmpty(collection["txtPaymentGroupName"]) ? collection["txtPaymentGroupName"].ToString() : "";
            var _ChildCustomerId = !String.IsNullOrEmpty(collection["cCustomerId"]) ? collection["cCustomerId"].ToString() : "";

            _companyService.SavePaymentGroup(_PaymentGroupName, _ChildCustomerId);
            return RedirectToAction("PaymentGroup", "Administration", new { area = "Portal" });
        }

        public ActionResult PaymentGroup()
        {
            ViewBag.CurrentMenu = "PaymentGroup";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Index", "Administration", new { area = "Portal" }), "Customer Sys Setting");
            BreadCrumb.Add(Url.Action("PaymentGroup", "Administration", new { area = "Portal" }), "Customer Third Party Setting");
            return View("PaymentGroup");
        }

        [HttpGet]
        public JsonResult GetAllCustomersfor_CPCS(string status, string pid, int regionId = 0, int type = 2)
        {

            var searchData = CustomerService.GetCustomerSettingAllPerentChildList(type, regionId == 0 ? SelectedRegionId : regionId, pid != "" ? int.Parse(pid) : 0, status);

            var jsonResult = Json(new { aadata = searchData }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetAllCustomerForSetting(bool isThirdParty, int regionId = 0, int isAll = 0)
        {
            var searchData = CustomerService.GetAllCustomerForSetting(isThirdParty, regionId == 0 ? SelectedRegionId : regionId, isAll);



            return Json(new { aadata = searchData.ChildCustomer }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCustomerChild(int custId, int regionId = 0)
        {
            var searchData = CustomerService.GetAllCustomerChild(custId, regionId == 0 ? SelectedRegionId : regionId);
            return Json(searchData, JsonRequestBehavior.AllowGet);
        }



        #region User Activity

        [HttpGet]
        public ActionResult UserSession()
        {
            ViewBag.CurrentMenu = "AdministrationUserManagment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "Administration");
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "User Managment");
            BreadCrumb.Add(Url.Action("Group", "Administration", new { area = "Portal" }), "Group");

            return View();
        }

        [HttpGet]
        public JsonResult UserSessionListData(string searchtext = "", string oc = "", string d = "")
        {
            string _ToDate = "";
            string _FromDate = "";
            if (!string.IsNullOrEmpty(d) && d.Contains("-") && d != "-")
            {
                _FromDate = d.Split('-')[0];
                _ToDate = d.Split('-')[1];
            }
            var jsonResult = Json(new
            {
                aadata = jkEntityModel.Atuh_UserLoginTrackingList(_FromDate, _ToDate, oc).ToList()
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult UserSessionListDataDetail(int TrackingId)
        {
            var jsonResult = Json(new
            {
                aadata = jkEntityModel.Auth_UserLoginActivityBYId(TrackingId).ToList()
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult KillUserSession(int TrackingId)
        {
            bool IsSuccess = false;
            List<AuthLoginTracking> UserLogDetails = null;

            if (TrackingId > 0)
                UserLogDetails = jkEntityModel.AuthLoginTrackings.Where(x => x.LoginTrackingId == TrackingId).ToList();
            else
                UserLogDetails = jkEntityModel.AuthLoginTrackings.Where(x => x.LogoutDateTime == null && x.UserId != LoginUserId).ToList();

            if (UserLogDetails != null && UserLogDetails.Count > 0)
            {
                foreach (var item in UserLogDetails)
                {
                    item.LogoutDateTime = DateTime.Now;
                    item.RefUrl = "Log off by Admin";
                    jkEntityModel.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    jkEntityModel.SaveChanges();

                    IsSuccess = true;
                }
            }

            var jsonResult = Json(IsSuccess, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion
    }

}

