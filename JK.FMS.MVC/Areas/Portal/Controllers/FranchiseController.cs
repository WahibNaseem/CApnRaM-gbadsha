using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using JKViewModels.Franchise;
using JKApi.Service.Service;
using JKApi.Service.Helper.Extension;
using JKApi.Data.DAL;
using JKApi.Core;
using Application.Web.Core;
using JKViewModels;
using JKApi.Service.ServiceContract.Franchisee;
using JKViewModels.Franchisee;
using JKViewModels.Customer;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels.Common;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.end;
using JKApi.Service;
using System.Web;
using JKApi.Service.ServiceContract.AccountReceivable;
using iTextSharp.text.html.simpleparser;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.ServiceContract.Management;
using JKViewModels.Inspection;
using JKApi.Service.ServiceContract.Inspection;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true)]
    public class FranchiseController : ViewControllerBase
    {
        string connectionString = string.Empty;

        FranchiseService serv = new FranchiseService();

        #region Constructor

        public FranchiseController(ICacheProvider CacheProvider, IFranchiseeService franchiseeService, ICustomerService customerService, ICommonService commonService, IAccountReceivableService _accountreceivableservice, ICompanyService companyService, IManagementService _ManagementService, IInspectionService inspectionService)
        {
            ViewBag.HMenu = "Franchise";
            //BreadCrumb.SetLabel("Franchise "); 
            this._cacheProvider = CacheProvider;
            FranchiseeService = franchiseeService;
            this._commonService = commonService;
            CustomerService = customerService;
            _companyService = companyService;
            ManagementService = _ManagementService;
            AccountReceivableService = _accountreceivableservice;
            _inspectionService = inspectionService;
        }

        #endregion

        #region Methods
        // GET: Admin/Franchise 
        public ActionResult Index()
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "General");
            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));

            return View(model);
        }

        #region Dashboard Chart.....
        public ActionResult GetFranchiseeDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = FranchiseeService.GetFranchiseeDashboardData(regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFranchiseRevenueByMonthChartData(int recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = FranchiseeService.GetFranchiseRevenueByMonthChartData(recordNumber, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRevenueWiseTopFranchiseeChartData(int recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = FranchiseeService.GetRevenueWiseTopFranchiseeChartData(recordNumber, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Dashboard Chart.....


        #region Franchisee > General

        [HttpGet]
        public ActionResult Maintenance(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            return View(FullFranchiseeViewModel);
        }

        public ActionResult MaintenanceTest(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult Maintenance(FullFranchiseeViewModel fullfranchiseeviewmodel)
        {

            return View(fullfranchiseeviewmodel);
        }

        #region  :: Step 1  ::

        [HttpGet]
        public ActionResult MaintenanceStepOne_Old(int? id, string title = "", string phone = "")
        {

            //int currentId = 0;
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);


            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int IsExist = 0;
            if (TempData["IsExist"] != null)
            {
                IsExist = Convert.ToInt32(TempData["IsExist"]);
            }
            ViewBag.IsExist = IsExist;
            ViewBag.FranchiseeName = title;
            ViewBag.FranchiseePhone = phone;

            if (id == null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Add");
                // return View();
                var State = CustomerService.GetStateList();
                if (FullFranchiseeViewModel.BusinessInfoAddress != null)
                {
                    ViewBag.BusinessInfoAddress_State = new SelectList(State, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
                }
                else
                {
                    ViewBag.BusinessInfoAddress_State = new SelectList(State, "StateListId", "Name");
                }
                if (FullFranchiseeViewModel.ContactInfoAddress != null)
                {
                    ViewBag.ContactInfoAddress_State = new SelectList(State, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
                }
                else
                {
                    ViewBag.ContactInfoAddress_State = new SelectList(State, "StateListId", "Name");
                }
                return View(FullFranchiseeViewModel);
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation);
            FullFranchiseeViewModel.BusinessInfo = FranchiseeService.GetFranchisee().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee>();
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContact().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
            FullFranchiseeViewModel.ContactInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();



            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();

            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "General");
            if (id == 0)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepOne_Old(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            fullfranchiseeviewmodel.BusinessInfo.RegionId = SelectedRegionId;
            ViewBag.CurrentMenu = "FranchiseGeneral";

            // Check Franchisee IsExist
            if (FranchiseeService.CheckFranchiseeIsExist(fullfranchiseeviewmodel.BusinessInfo.Name))
            {
                TempData["IsExist"] = 1;
                return RedirectToAction("MaintenanceStepOne", "Franchise", new { area = "Portal", searchin = 1, status = 1 });
            }

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            fullfranchiseeviewmodel.BusinessInfo.FranchiseeNo = GetFranchiseeNo();
            fullfranchiseeviewmodel.BusinessInfo = FranchiseeService.SaveFranchisee(fullfranchiseeviewmodel.BusinessInfo.ToModel<Franchisee, FranchiseeViewModel>()).ToModel<FranchiseeViewModel, Franchisee>();
            UpdateFranchiseeIndex();
            fullfranchiseeviewmodel.BusinessInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (collection["BusinessInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.StateListId = Convert.ToInt32(collection["BusinessInfoAddress_State"]);
            }
            fullfranchiseeviewmodel.BusinessInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoAddress.IsActive = true;

            var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.BusinessInfoAddress.FullAddress));
            if (_latlng.results.Count() > 0)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.BusinessInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
            }


            fullfranchiseeviewmodel.BusinessInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.BusinessInfoEmail.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (collection["ContactInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.ContactInfoAddress.StateListId = Convert.ToInt32(collection["ContactInfoAddress_State"]);
            }
            fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoAddress.IsActive = true;
            var _latlngC = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.ContactInfoAddress.FullAddress));
            if (_latlngC.results.Count() > 0)
            {
                fullfranchiseeviewmodel.ContactInfoAddress.Latitude = decimal.Parse(_latlngC.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.ContactInfoAddress.Longitude = decimal.Parse(_latlngC.results[0].geometry.location.lng.ToString());
            }

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;


            fullfranchiseeviewmodel.ContactInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;

            if (collection["ContactInfoAddress_State"] != null)
            {
                int id = Convert.ToInt32(collection["ContactInfoAddress_State"]);
                string state = CustomerService.GetStatesName(id);
                fullfranchiseeviewmodel.BusinessInfoAddress.StateName = state.Trim();
            }
            fullfranchiseeviewmodel.BusinessInfoAddress = CustomerService.SaveAddress(fullfranchiseeviewmodel.BusinessInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            fullfranchiseeviewmodel.BusinessInfoPhone = CustomerService.SavePhone(fullfranchiseeviewmodel.BusinessInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            fullfranchiseeviewmodel.BusinessInfoEmail = CustomerService.SaveEmail(fullfranchiseeviewmodel.BusinessInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();


            fullfranchiseeviewmodel.ContactInfo = CustomerService.SaveContact(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
            fullfranchiseeviewmodel.ContactInfoAddress = CustomerService.SaveAddress(fullfranchiseeviewmodel.ContactInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.SavePhone(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.SaveEmail(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();


            //fullfranchiseeviewmodel.Status.StatusListId = (int)JKApi.Business.Enumeration.FranchiseeStatusList.Active;
            //fullfranchiseeviewmodel.Status.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franachisee;
            //fullfranchiseeviewmodel.Status.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            //fullfranchiseeviewmodel.Status = CustomerService.SaveStatus(fullfranchiseeviewmodel.Status.ToModel<Status, StatusViewModel>()).ToModel<StatusViewModel, Status>();





            var States = CustomerService.GetStateList();

            ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", fullfranchiseeviewmodel.BusinessInfoAddress.StateListId);
            ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", fullfranchiseeviewmodel.ContactInfoAddress.StateListId);
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            return View(fullfranchiseeviewmodel);
        }

        [HttpGet]
        public ActionResult MaintenanceStepOne(int? id, string title = "", string phone = "")
        {

            //int currentId = 0;
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);


            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int IsExist = 0;
            if (TempData["IsExist"] != null)
            {
                IsExist = Convert.ToInt32(TempData["IsExist"]);
            }
            ViewBag.IsExist = IsExist;
            ViewBag.FranchiseeName = title;
            ViewBag.FranchiseePhone = phone;

            if (id == null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Add");
                // return View();
                var State = CustomerService.GetStateList();
                if (FullFranchiseeViewModel.BusinessInfoAddress != null)
                {
                    ViewBag.BusinessInfoAddress_State = new SelectList(State, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
                }
                else
                {
                    ViewBag.BusinessInfoAddress_State = new SelectList(State, "StateListId", "Name");
                }
                if (FullFranchiseeViewModel.ContactInfoAddress != null)
                {
                    ViewBag.ContactInfoAddress_State = new SelectList(State, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
                }
                else
                {
                    ViewBag.ContactInfoAddress_State = new SelectList(State, "StateListId", "Name");
                }
                return View(FullFranchiseeViewModel);
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);
            FullFranchiseeViewModel.BusinessInfo = FranchiseeService.GetFranchiseeTemp().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee>();
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContactTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
            FullFranchiseeViewModel.ContactInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();



            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();

            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "General");
            if (id == 0)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepOne(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            fullfranchiseeviewmodel.BusinessInfo.RegionId = SelectedRegionId;
            ViewBag.CurrentMenu = "FranchiseGeneral";

            // Check Franchisee IsExist
            if (FranchiseeService.CheckFranchiseeIsExistTemp(fullfranchiseeviewmodel.BusinessInfo.Name))
            {
                TempData["IsExist"] = 1;
                return RedirectToAction("MaintenanceStepOne", "Franchise", new { area = "Portal", searchin = 1, status = 1 });
            }

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            fullfranchiseeviewmodel.BusinessInfo.FranchiseeNo = GetFranchiseeNo();
            fullfranchiseeviewmodel.BusinessInfo = FranchiseeService.SaveFranchisee_Temp(fullfranchiseeviewmodel.BusinessInfo.ToModel<Franchisee_Temp, FranchiseeViewModel>()).ToModel<FranchiseeViewModel, Franchisee_Temp>();
            UpdateFranchiseeIndex();
            fullfranchiseeviewmodel.BusinessInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (collection["BusinessInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.StateListId = Convert.ToInt32(collection["BusinessInfoAddress_State"]);
            }
            fullfranchiseeviewmodel.BusinessInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoAddress.IsActive = true;

            var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.BusinessInfoAddress.FullAddress));
            if (_latlng.results.Count() > 0)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.BusinessInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
            }


            fullfranchiseeviewmodel.BusinessInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.BusinessInfoEmail.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (collection["ContactInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.ContactInfoAddress.StateListId = Convert.ToInt32(collection["ContactInfoAddress_State"]);
            }
            fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoAddress.IsActive = true;
            var _latlngC = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.ContactInfoAddress.FullAddress));
            if (_latlngC.results.Count() > 0)
            {
                fullfranchiseeviewmodel.ContactInfoAddress.Latitude = decimal.Parse(_latlngC.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.ContactInfoAddress.Longitude = decimal.Parse(_latlngC.results[0].geometry.location.lng.ToString());
            }

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;


            fullfranchiseeviewmodel.ContactInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;

            if (collection["ContactInfoAddress_State"] != null)
            {
                int id = Convert.ToInt32(collection["ContactInfoAddress_State"]);
                string state = CustomerService.GetStatesName(id);
                fullfranchiseeviewmodel.BusinessInfoAddress.StateName = state.Trim();
            }
            fullfranchiseeviewmodel.BusinessInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.BusinessInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.BusinessInfoPhone = CustomerService.SavePhone_Temp(fullfranchiseeviewmodel.BusinessInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.BusinessInfoEmail = CustomerService.SaveEmail_Temp(fullfranchiseeviewmodel.BusinessInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();


            fullfranchiseeviewmodel.ContactInfo = CustomerService.SaveContact_Temp(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();
            fullfranchiseeviewmodel.ContactInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.ContactInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.SavePhone_Temp(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.SaveEmail_Temp(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();


            //fullfranchiseeviewmodel.Status.StatusListId = (int)JKApi.Business.Enumeration.FranchiseeStatusList.Active;
            //fullfranchiseeviewmodel.Status.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franachisee;
            //fullfranchiseeviewmodel.Status.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            //fullfranchiseeviewmodel.Status = CustomerService.SaveStatus(fullfranchiseeviewmodel.Status.ToModel<Status, StatusViewModel>()).ToModel<StatusViewModel, Status>();


            var States = CustomerService.GetStateList();

            ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", fullfranchiseeviewmodel.BusinessInfoAddress.StateListId);
            ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", fullfranchiseeviewmodel.ContactInfoAddress.StateListId);
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            return View(fullfranchiseeviewmodel);
        }

        #endregion  

        #region  :: Step 2  ::

        [HttpGet]
        public ActionResult MaintenanceStepTwo_Old(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            if (id > 0)
            {
                var FranchiseeDetails = FranchiseeService.GetFranchiseeById(id);
                if (FranchiseeDetails != null)
                {
                    FullFranchiseeViewModel.BusinessInfo.Name = FranchiseeDetails.Name;
                }
            }
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner));


            FullFranchiseeViewModel.ACHBankInfo = FranchiseeService.GetACHBank().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList).OrderByDescending(one => one.ACHBankId).FirstOrDefault().ToModel<ACHBankViewModel, ACHBank>();
            if (FullFranchiseeViewModel.ACHBankInfo == null)
            {
                FullFranchiseeViewModel.ACHBankInfo = new ACHBankViewModel();
                FullFranchiseeViewModel.ACHBankInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.ACHBankInfo.TypeListId = BusinessTypeList;
            }

            FullFranchiseeViewModel.PayeeIdentification = CustomerService.GetIdentification().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification>();
            if (FullFranchiseeViewModel.PayeeIdentification == null)
            {
                FullFranchiseeViewModel.PayeeIdentification = new IdentificationViewModel();
                FullFranchiseeViewModel.PayeeIdentification.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.PayeeIdentification.TypeListId = BusinessTypeList;
                FullFranchiseeViewModel.PayeeIdentification.ContactTypeListId = BusinessContactTypeList;
            }

            FullFranchiseeViewModel.PayeeInfo = FranchiseeService.GetFranchiseeBillSettings().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeBillSettingsId).FirstOrDefault().ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSetting>();
            if (FullFranchiseeViewModel.PayeeInfo == null)
            {
                FullFranchiseeViewModel.PayeeInfo = new FranchiseeBillSettingViewModel();
                FullFranchiseeViewModel.PayeeInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            }

            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FranchiseeId; // FullFranchiseeViewModel.PayeeInfo.FranchiseeBillSettingsId;
            FullFranchiseeViewModel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            FullFranchiseeViewModel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            FullFranchiseeViewModel.PayeeInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FullFranchiseeViewModel.PayeeInfoAddress.ClassId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();

            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepTwo_Old(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepOne", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            fullfranchiseeviewmodel.PayeeInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;

            if (collection["PayeeInfo.Incorporated"] != null && collection["PayeeInfo.Incorporated"] != "")
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = (collection["PayeeInfo.Incorporated"] == "on" ? true : false);
            }
            else
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = false;
            }

            if (fullfranchiseeviewmodel.PayeeInfo != null && !string.IsNullOrEmpty(fullfranchiseeviewmodel.PayeeInfo.PayeeName))
            {
                fullfranchiseeviewmodel.PayeeInfo = FranchiseeService.SaveFranchiseeBillSettings(fullfranchiseeviewmodel.PayeeInfo.ToModel<FranchiseeBillSetting, FranchiseeBillSettingViewModel>()).ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSetting>();
            }
            if (collection["PayeeInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.PayeeInfoAddress.StateListId = Convert.ToInt32(collection["PayeeInfoAddress_State"]);
            }
            //fullfranchiseeviewmodel.PayeeInfoAddress.ClassId = fullfranchiseeviewmodel.PayeeInfo.FranchiseeBillSettingsId;
            fullfranchiseeviewmodel.PayeeInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfoAddress.IsActive = true;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.PayeeInfoAddress = CustomerService.SaveAddress(fullfranchiseeviewmodel.PayeeInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

            fullfranchiseeviewmodel.PayeeIdentification = CustomerService.SaveIdentification(fullfranchiseeviewmodel.PayeeIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();
            if (fullfranchiseeviewmodel.ACHBankInfo != null && !string.IsNullOrEmpty(fullfranchiseeviewmodel.ACHBankInfo.Name))
            {
                fullfranchiseeviewmodel.ACHBankInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                fullfranchiseeviewmodel.ACHBankInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
                fullfranchiseeviewmodel.ACHBankInfo = FranchiseeService.SaveACHBank(fullfranchiseeviewmodel.ACHBankInfo.ToModel<ACHBank, ACHBankViewModel>()).ToModel<ACHBankViewModel, ACHBank>();
            }

            var States = CustomerService.GetStateList();

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }



            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                // return RedirectToAction("MaintenanceStepThree", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
                return RedirectToAction("MaintenanceStepFour", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            return View(fullfranchiseeviewmodel);
        }

        [HttpGet]
        public ActionResult MaintenanceStepTwo(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            if (id > 0)
            {
                var FranchiseeDetails = FranchiseeService.GetFranchiseeById_Temp(id);
                if (FranchiseeDetails != null)
                {
                    FullFranchiseeViewModel.BusinessInfo.Name = FranchiseeDetails.Name;
                }
            }
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner));
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListByIdTemp(FranchiseeId);

            FullFranchiseeViewModel.ACHBankInfo = FranchiseeService.GetACHBankTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList).OrderByDescending(one => one.ACHBankId).FirstOrDefault().ToModel<ACHBankViewModel, ACHBank_Temp>();
            if (FullFranchiseeViewModel.ACHBankInfo == null)
            {
                FullFranchiseeViewModel.ACHBankInfo = new ACHBankViewModel();
                FullFranchiseeViewModel.ACHBankInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.ACHBankInfo.TypeListId = BusinessTypeList;
            }

            FullFranchiseeViewModel.PayeeIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Payee).OrderByDescending(one => one.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();
            if (FullFranchiseeViewModel.PayeeIdentification == null)
            {
                FullFranchiseeViewModel.PayeeIdentification = new IdentificationViewModel();
                FullFranchiseeViewModel.PayeeIdentification.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.PayeeIdentification.TypeListId = BusinessTypeList;
                FullFranchiseeViewModel.PayeeIdentification.ContactTypeListId = BusinessContactTypeList;
            }

            FullFranchiseeViewModel.PayeeInfo = FranchiseeService.GetFranchiseeBillSettingsTemp().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeBillSettingsId).FirstOrDefault().ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSettings_Temp>();
            if (FullFranchiseeViewModel.PayeeInfo == null)
            {
                FullFranchiseeViewModel.PayeeInfo = new FranchiseeBillSettingViewModel();
                FullFranchiseeViewModel.PayeeInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            }

            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FranchiseeId; // FullFranchiseeViewModel.PayeeInfo.FranchiseeBillSettingsId;
            FullFranchiseeViewModel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            FullFranchiseeViewModel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            FullFranchiseeViewModel.PayeeInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FullFranchiseeViewModel.PayeeInfoAddress.ClassId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();

            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepTwo(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepOne", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepTwo", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            fullfranchiseeviewmodel.PayeeInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;

            if (collection["PayeeInfo.Incorporated"] != null && collection["PayeeInfo.Incorporated"] != "")
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = (collection["PayeeInfo.Incorporated"] == "on" ? true : false);
            }
            else
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = false;
            }

            if (fullfranchiseeviewmodel.PayeeInfo != null && !string.IsNullOrEmpty(fullfranchiseeviewmodel.PayeeInfo.PayeeName))
            {
                fullfranchiseeviewmodel.PayeeInfo = FranchiseeService.SaveFranchiseeBillSettings_Temp(fullfranchiseeviewmodel.PayeeInfo.ToModel<FranchiseeBillSettings_Temp, FranchiseeBillSettingViewModel>()).ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSettings_Temp>();
            }
            if (collection["PayeeInfoAddress_State"] != null)
            {
                fullfranchiseeviewmodel.PayeeInfoAddress.StateListId = Convert.ToInt32(collection["PayeeInfoAddress_State"]);
            }
            //fullfranchiseeviewmodel.PayeeInfoAddress.ClassId = fullfranchiseeviewmodel.PayeeInfo.FranchiseeBillSettingsId;
            fullfranchiseeviewmodel.PayeeInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfoAddress.IsActive = true;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.PayeeInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.PayeeInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();

            fullfranchiseeviewmodel.PayeeIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;

            fullfranchiseeviewmodel.PayeeIdentification = CustomerService.SaveIdentification_Temp(fullfranchiseeviewmodel.PayeeIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();
            if (fullfranchiseeviewmodel.ACHBankInfo != null && !string.IsNullOrEmpty(fullfranchiseeviewmodel.ACHBankInfo.Name))
            {
                fullfranchiseeviewmodel.ACHBankInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                fullfranchiseeviewmodel.ACHBankInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
                fullfranchiseeviewmodel.ACHBankInfo = FranchiseeService.SaveACHBank_Temp(fullfranchiseeviewmodel.ACHBankInfo.ToModel<ACHBank_Temp, ACHBankViewModel>()).ToModel<ACHBankViewModel, ACHBank_Temp>();
            }

            var States = CustomerService.GetStateList();

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }



            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                // return RedirectToAction("MaintenanceStepThree", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
                return RedirectToAction("MaintenanceStepFour", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            return View(fullfranchiseeviewmodel);
        }

        #endregion

        #region  :: Step 3  ::

        [HttpGet]
        public ActionResult MaintenanceStepThree(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }


            FullFranchiseeViewModel.FullfillmentInfo = FranchiseeService.GetFranchiseeFullfillment().Where(one => one.FranchiseeId == FullFranchiseeViewModel.BusinessInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeFullfillmentId).FirstOrDefault().ToModel<FranchiseeFullfillmentViewModel, FranchiseeFullfillment>();


            return View(FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepThree(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("FranchiseeUpdateDocument", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "General");

            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepThree", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            fullfranchiseeviewmodel.FullfillmentInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.FullfillmentInfo = FranchiseeService.SaveFranchiseeFullfillment(fullfranchiseeviewmodel.FullfillmentInfo.ToModel<FranchiseeFullfillment, FranchiseeFullfillmentViewModel>()).ToModel<FranchiseeFullfillmentViewModel, FranchiseeFullfillment>();


            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                //return RedirectToAction("MaintenanceStepFour", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
                return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            return View(fullfranchiseeviewmodel);
        }

        #endregion

        #region  :: Step 3  ::

        [HttpGet]
        public ActionResult MaintenanceStepFourOld(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContract().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract>();
            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();
            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            if (id != -1)
                FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection(id);
            else
                FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollectionAll(id);


            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();


            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            //Add Default Franchisee Fee Details
            if (id > 0)
            {
                FranchiseeService.AddDefaultFranchiseeFees(id, 1, 3);     // 1  : Accounting Fee 
                FranchiseeService.AddDefaultFranchiseeFees(id, 9, 3);   // 9  : Technology Fee    
                FranchiseeService.AddDefaultFranchiseeFees(id, 10, 3);  // 10 :	Advertising Fee
                FranchiseeService.AddDefaultFranchiseeFees(id, 17, 10);  // 17 : Royalty
                FranchiseeService.AddDefaultFranchiseeFees(id, 23, 5.50M);  // 23 : Business Protection    
            }
            return View(FullFranchiseeViewModel);
        }
        [HttpGet]
        public JsonResult GetFeeFranchiseeFeeRateListFranchiseeIdOld(int Id)
        {
            if (Id > 0)
            {
                var Data = FranchiseeService.GetFeeListCollection(Id);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            return Json(Id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MaintenanceStepFourOld(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }
            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract>();

            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();

            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (fullfranchiseeviewmodel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            {
                //return RedirectToAction("SearchList", "Franchise", new { area = "Portal", searchin = 7, status = 1 });
                return RedirectToAction("FranchiseeUpdateDocument", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });

            }
            return View(fullfranchiseeviewmodel);
        }

        [HttpGet]
        public ActionResult MaintenanceStepFour(int id = -1)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContractTemp().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract_Temp>();
            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();
            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            if (id != -1)
                FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection_Temp(id);
            else
                FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollectionAll(id);


            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();


            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            //Add Default Franchisee Fee Details
            if (id > 0)
            {
                FranchiseeService.AddDefaultFranchiseeFees_Temp(id, 1, 3);     // 1  : Accounting Fee 
                FranchiseeService.AddDefaultFranchiseeFees_Temp(id, 9, 3);   // 9  : Technology Fee    
                FranchiseeService.AddDefaultFranchiseeFees_Temp(id, 10, 3);  // 10 :	Advertising Fee
                FranchiseeService.AddDefaultFranchiseeFees_Temp(id, 17, 10);  // 17 : Royalty
                FranchiseeService.AddDefaultFranchiseeFees_Temp(id, 23, 5.50M);  // 23 : Business Protection    
            }
            return View(FullFranchiseeViewModel);
        }
        [HttpGet]
        public JsonResult GetFeeFranchiseeFeeRateListFranchiseeId(int Id)
        {
            if (Id > 0)
            {
                var Data = FranchiseeService.GetFeeListCollection_Temp(Id);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            return Json(Id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MaintenanceStepFour(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            }
            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "General");
            if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepFour", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }
            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }
            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract_Temp(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract_Temp, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract_Temp>();

            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();

            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (fullfranchiseeviewmodel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullfranchiseeviewmodel);
            }
            else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            {
                //return RedirectToAction("SearchList", "Franchise", new { area = "Portal", searchin = 7, status = 1 });
                return RedirectToAction("FranchiseeUpdateDocument", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });

            }
            return View(fullfranchiseeviewmodel);
        }

        #endregion

        [HttpGet]
        public ActionResult Search()
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Search", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Search", "Franchise", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("Search", "Franchise", new { area = "Portal" }), "Search");

            ViewBag.SearchIn = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.SearchIn>().Select(k => new SelectListItem { Text = k.Key, Value = k.Value.ToString() }).OrderBy(s => s.Text).ToList();
            ViewBag.Status = serv.GetFranchiseStatusListItem().ToList().Select(k => new SelectListItem { Text = k.Name, Value = k.StatusListId.ToString() }).OrderBy(s => s.Text).ToList();

            return View();
        }

        [HttpGet]
        public JsonResult FranchiseAutoComplete(string keyword)
        {
            //if (namePrefix == null)
            //{
            //    NLogger.Error("Requested SummaryData is null or empty");
            //    return Json(new { success = false, message = "Data is null or empty" });
            //}

            //List<Franchisee> SearchData = null;
            //if (InvoicePrefix == "1")
            //{
            //    SearchData = FranchiseeService.GetSearchFranchisee().Where(x => x.Name.Contains(namePrefix.ToUpper())).ToList();
            //}
            //if (InvoicePrefix == "2")
            //{
            //    SearchData = FranchiseeService.GetSearchFranchisee().Where(x => x.FranchiseeNo.Contains(namePrefix)).ToList();
            //}

            //var result = from cust in SearchData select new { cust.FranchiseeId, cust.Name, cust.FranchiseeNo };

            //return Json(result, JsonRequestBehavior.AllowGet);

            var searchData = FranchiseeService.GetSearchFranchisee(keyword ?? "").ToList();
            return Json(searchData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FranchiseDetailAutoComplete(int franchiseid)
        {
            return Json(FranchiseeService.GetFranchiseeDetailData(franchiseid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FranchiseDetailByFransSupplyId(int Id)
        {
            return Json(FranchiseeService.GetFranchiseebySupplyId(Id, SelectedRegionId), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetAllFranchisees(string searchText = "")
        {
            var searchData = FranchiseeService.GetSearchFranchisee(searchText);
            return Json(searchData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            // return RedirectToAction("SearchList", new { searchin = collection["SearchIn"].ToString(), searchvalue = collection["For"].ToString(), status = collection["Status"].ToString() });
            return View();
        }

        [HttpGet]
        public ActionResult SearchList(string searchin, string status, string searchvalue = "")
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("SearchList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("SearchList", "Franchise", new { area = "Portal" }) + "?searchin=7&status=1", "Franchise List");
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);

            //int preSelect = int.Parse(_claimView.GetCLAIM_SELECTED_COMPANY_ID() ?? "0");
            //var regionlist = _commonService.GetRegionList();

            ////if (preSelect == 0)
            ////{
            ////    regionlist.Insert(0, new Region { Name = "All" });
            ////}

            //ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            // var json = serv.GetFranchiseSearchList(searchin, searchvalue, int.Parse(status)).ToList();
            // ViewBag.FranchiseSearchList = json;
            return View();

        }
        public ActionResult FranchiseeSearch()
        {
            var regionlist = _commonService.GetRegionList();
            ViewBag.SelectedRegionId = SelectedRegionId;
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);

            var PlanTypeList = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            ViewBag.PlanTypeList = new SelectList(PlanTypeList, "FranchiseeContractTypeListId", "Name");

            Session["SearchFranchiseeIds"] = null;
            Session["SearchFranchiseeResultURL"] = null;
            return View();
        }

        public ActionResult SearchFranchiseeList(string s = "", string sdt = "", string edt = "", string ptId = "", string statusIds = "")
        {
            //if (statusIds == "")
            //{
            //    statusIds = Convert.ToInt32(JKApi.Business.Enumeration.FranchiseeStatusList.Active).ToString();
            //}
            Session["SearchFranchiseeIds"] = null;
            Session["SearchFranchiseeResultURL"] = s + "," + sdt + "," + edt + "," + ptId + "," + statusIds;
            TempData["SearchStatusIds"] = statusIds;

            ViewBag.CurrentMenu = "FranchiseeSearch";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeSearch", "Franchise", new { area = "Portal" }), "FranchiseeSearch");
            BreadCrumb.Add(Url.Action("FranchiseeSearch", "Franchise", new { area = "Portal" }), "Search Franchisee List");

            ViewBag.s = s;
            ViewBag.sdt = sdt;
            ViewBag.edt = edt;
            ViewBag.ptId = ptId;
            ViewBag.statusIds = statusIds;

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult SearchFranchiseeListDataTable(string rId = "", string s = "", string sdt = "", string edt = "", string ptId = "", string statusIds = "")
        {
            try
            {
                int FranchiseTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranchiseContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                //if (_claimView.GetCLAIM_ROLE_TYPE() == "Accounting-User")
                //{
                //    status = "36";
                //}
                //var response = FranchiseeService.GetFranchiseeList(statusIds, rId);

                var response = FranchiseeService.GetSearchFranchiseeList(s, sdt, edt, (ptId == "" ? 0 : Convert.ToInt32(ptId)), statusIds, rId);

                var result = (from f in response
                              select new
                              {
                                  ID = f.FranchiseeId,
                                  Number = f.FranchiseeNo,
                                  Name = f.Name,
                                  Address = f.Address1,
                                  DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
                                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  RegionName = (f.RegionName != null ? f.RegionName : ""),
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  IsTemp = f.IsTemp
                              }).ToList();

                List<SearchFranchiseeListId> ListData = new List<SearchFranchiseeListId>();
                ListData = (from f in response
                            select new SearchFranchiseeListId()
                            {
                                Id = f.RowNo,
                                FranchiseeId = f.FranchiseeId,
                                FranchiseeNo = f.FranchiseeNo,
                                FranchiseeName = f.Name,
                                DistributionAmount = f.DistributionAmount,
                            }).ToList();
                Session["SearchFranchiseeIds"] = ListData;

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

        public ActionResult SearchFranchiseeDetails(int FranId, int step = 0, int RId = 0, int colidx = 0, string sort = "")
        {

            #region :: Next/Prev ::

            bool PrevBtn = true;
            bool NextBtn = true;

            var ResultDataList = (List<SearchFranchiseeListId>)Session["SearchFranchiseeIds"];
            if (colidx != 0 && sort != "")
            {
                #region 
                // Note: Column Index 2: Customer No && Column Index 2: Customer Name && Column Index 3: Contract Amount && Column Index 6: Franchisee No && Column Index 7: Franchisee Name                 
                if (colidx == 1)
                {
                    #region Customer No sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.FranchiseeNo).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.FranchiseeNo).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
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
                        var dataResult = ResultDataList.OrderBy(o => o.FranchiseeName).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.FranchiseeName).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
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
                        var dataResult = ResultDataList.OrderBy(o => o.DistributionAmount).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    else
                    {
                        var dataResult = ResultDataList.OrderByDescending(o => o.DistributionAmount).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
                            itm.Id = n;
                            itm.FranchiseeId = item.FranchiseeId;
                            itm.FranchiseeNo = item.FranchiseeNo;
                            itm.FranchiseeName = item.FranchiseeName;
                            itm.DistributionAmount = item.DistributionAmount;
                            ListItemData.Add(itm);
                            n = n + 1;
                        }
                        ResultDataList = ListItemData;
                    }
                    #endregion
                }
                /* else if (colidx == 6)
                {
                    #region Franchisee No sort
                    if (sort == "asc")
                    {
                        var dataResult = ResultDataList.OrderBy(o => o.FranchiseeNo).ToList();
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
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
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
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
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
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
                        List<JKViewModels.Customer.SearchFranchiseeListId> ListItemData = new List<JKViewModels.Customer.SearchFranchiseeListId>();
                        int n = 1;
                        foreach (var item in dataResult)
                        {
                            JKViewModels.Customer.SearchFranchiseeListId itm = new JKViewModels.Customer.SearchFranchiseeListId();
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
                } */
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
                var CustId = ResultDataList.Where(w => w.FranchiseeId == FranId);
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
                    FranId = CustData.FirstOrDefault().FranchiseeId;
                }
            }
            ViewBag.PrevBtn = PrevBtn;
            ViewBag.NextBtn = NextBtn;
            ViewBag.colidx = colidx;
            ViewBag.sort = sort;

            #endregion

            ViewBag.FranchiseID = FranId;
            ViewBag.callFrom = 1;

            ViewBag.CurrentMenu = "FranchiseeSearch";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeSearch", "Franchise", new { area = "Portal" }), "FranchiseeSearch");
            BreadCrumb.Add(Url.Action("FranchiseeSearch", "Franchise", new { area = "Portal" }), "Search Franchisee List");

            //ViewBag.CurrentMenu = "FranchiseGeneral";
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("FranchiseeDetail", "Franchise", new { area = "Portal" }), "Franchise");
            //BreadCrumb.Add(Url.Action("FranchiseeDetail", "Franchise", new { area = "Portal" }), "General");
            var TransactionsTypeList = FranchiseeService.GetUsMasterTrxTypeList().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }

            var _periodlst = FranchiseeService.GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString());
            if (_periodlst.Count > 0)
                ViewBag.BillMonthYears = new SelectList(FranchiseeService.GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString()), "Period", "Period");
            else
                ViewBag.BillMonthYears = new SelectList("Select");
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 10).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 31);
            var leaseStatuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 7).ToList();
            ViewBag.LeaseStatusList = new SelectList(leaseStatuslist, "StatusListId", "Name", 21);
            string Franchiseestatus = string.Empty;
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (FranId > 0)
            {
                FranchiseeDetail = GetFranchiseeDetailData(FranId, true);
                Franchiseestatus = FranchiseeService.GetFranchiseeStatus(FranId);
            }
            ViewBag.Franchiseestatus = Franchiseestatus;
            ViewBag.Leasestaus = (FranId > 0 ? FranchiseeService.GetFranchiseeLeaseStatus(FranId) : 0);
            return View("FranchiseeDetail", FranchiseeDetail);
        }

        public ActionResult BacktoSearchResult()
        {
            var item = Session["SearchFranchiseeResultURL"]?.ToString().Split(',');
            string s = item[0];
            string sdt = item[1];
            string edt = item[2];
            string ptId = item[3];
            string statusIds = TempData["SearchStatusIds"]?.ToString();
            return RedirectToAction("SearchFranchiseeList", "Franchise", new { s = s, sdt = sdt, edt = edt, ptId = ptId, statusIds = statusIds });

        }

        [HttpGet]
        public ActionResult FranchiseeDetail(int id = -1)
        {

            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeDetail", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeDetail", "Franchise", new { area = "Portal" }), "General");
            var TransactionsTypeList = FranchiseeService.GetUsMasterTrxTypeList().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }
            ViewBag.FranchiseID = id;
            ViewBag.callFrom = 0;
            var _periodlst = FranchiseeService.GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString());
            if (_periodlst.Count > 0)
                ViewBag.BillMonthYears = new SelectList(FranchiseeService.GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString()), "Period", "Period");
            else
                ViewBag.BillMonthYears = new SelectList("Select");
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 10).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 31);
            var leaseStatuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 7).ToList();
            ViewBag.LeaseStatusList = new SelectList(leaseStatuslist, "StatusListId", "Name", 21);
            string Franchiseestatus = string.Empty;
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (id > 0)
            {
                FranchiseeDetail = GetFranchiseeDetailData(id, true);
                Franchiseestatus = FranchiseeService.GetFranchiseeStatus(id);
            }
            var fr = FranchiseeService.GetFranchiseeById(id);
            if (fr != null)
            {
                if (fr.ParentId != null)
                {
                    if (fr.ParentId != 0)
                    {
                        int prId = (int)(fr.ParentId);
                        var checkPerfix = FranchiseeService.GetFranchiseeByParentId(prId);
                        ViewBag.Prefixlist = new SelectList(checkPerfix, "FranchiseeId", "Perfix", id);
                    }
                }
            }
            else
            {
                ViewBag.Prefixlist = null;
            }
            ViewBag.Franchiseestatus = Franchiseestatus;
            ViewBag.Leasestaus = (id > 0 ? FranchiseeService.GetFranchiseeLeaseStatus(id) : 0);
            return View(FranchiseeDetail);
        }

        [HttpGet]
        public ActionResult GetFranchiseeLoad(int id)
        {
            string Franchiseestatus = string.Empty;
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (id > 0)
            {
                FranchiseeDetail = GetFranchiseeDetailData(id, true);
                Franchiseestatus = FranchiseeService.GetFranchiseeStatus(id);
            }
            ViewBag.Franchiseestatus = Franchiseestatus;
            ViewBag.Leasestaus = (id > 0 ? FranchiseeService.GetFranchiseeLeaseStatus(id) : 0);
            return PartialView("~/Areas/Portal/Views/Franchise/_PartialFranchiseeDetailInfo.cshtml", FranchiseeDetail);
        }

        [HttpGet]
        public JsonResult FinalizedFranchiseeReport(int? frId = null, int? month = null, int? year = null)
        {
            string rgId = SelectedRegionId.ToString();
            var reports = FranchiseeService.GetFinalizedFranchiseeReport(month, year, rgId, frId);
            return Json(new { Data = reports, success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFinderFeeReport(int FranchiseeID, string FromDate, string ToDate)
        {
            var FranchiseeFinder = serv.FranchiseeFinderFeeReportByFranchisee(FranchiseeID, FromDate, ToDate);
            return Json(new { data = FranchiseeFinder }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFranchiseeDataByID(int id)
        {
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            FranchiseeDetail = GetFranchiseeDetailData(id, true);

            return Json(FranchiseeDetail, JsonRequestBehavior.AllowGet);
        }

        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = "All Transactions" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", list.SelectedValue);
        }

        [HttpGet]
        public ActionResult Report1099(int isback = 0)
        {
            ViewBag.CurrentMenu = "FranchiseAccount";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Report1099", "Franchise", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("Report1099", "Franchise", new { area = "Portal" }), "1099 Report");
            ViewBag.isback = isback;

            return View();
        }

        public ActionResult AccountHistoryDetail()
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AccountHistoryDetail", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("AccountHistoryDetail", "Franchise", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("AccountHistoryDetail", "Franchise", new { area = "Portal" }), "Account History");
            //ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            return View();
        }


        public PartialViewResult FranchiseeAccountHistoryDetail(int FranchiseeId)
        {
            return PartialView("_PartialFranchiseeAccountHistoryReport", FranchiseeService.GetFranchiseeAccountHistoryReport(FranchiseeId));
        }

        public PartialViewResult FranchiseeObligationDetail(int FranchiseeId)
        {
            return PartialView("_PartialFranchiseeObligation", FranchiseeService.GetFranchiseeObligationList(FranchiseeId));
        }

        public FileResult FranchiseeAccountHistoryReportExport(int FranchiseeId)
        {
            if (FranchiseeId > 0)
            {
                string HTMLContent = string.Empty;

                var data = FranchiseeService.GetFranchiseeAccountHistoryReport(FranchiseeId);
                HTMLContent += RenderPartialViewToString("_PartialFranchiseeAccountHistoryReport", data);

                return File(GetPDF(HTMLContent), "application/pdf", "_ChargeBackExportToPDF.pdf");
            }
            return null;
        }

        #region AjaxCalls 


        [HttpGet]
        public ActionResult AddEditFranchiseeOwner(string id)
        {
            int _id = Convert.ToInt32(id);
            var feeRateTypeList = FranchiseeService.GetFeeRateTypeList().ToList();
            ViewBag.FeeRateTypeList = new SelectList(feeRateTypeList, "FeeRateTypeListId", "Rate");
            FranchiseeFeeListViewModel FranchiseeFeeListViewModel = FranchiseeService.GetFranchiseeFeeListById(_id).ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
            if (FranchiseeFeeListViewModel == null)
            {
                FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
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
            FranchiseeFeeListViewModel _FranchiseeFeeListViewModel = FranchiseeService.GetFranchiseeFeeList().Where(one => one.Name == FranchiseeFeeListViewModel.Name && one.Amount == FranchiseeFeeListViewModel.Amount).FirstOrDefault().ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
            if (_FranchiseeFeeListViewModel == null)
            {
                _FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
                FranchiseeFeeListViewModel.IsActive = true;
                FranchiseeFeeListViewModel = FranchiseeService.SaveFranchiseeFeeList(FranchiseeFeeListViewModel.ToModel<FranchiseeFeeList, FranchiseeFeeListViewModel>()).ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();

            }
            return RedirectToAction("FranchiseeFeeList", "Administration", new { area = "Portal" });
        }

        [HttpPost]
        public JsonResult FranchiseeMaintenancePartial(FranchiseeMaintenanceViewModel model, FormCollection frm)
        {
            FranchiseeService.InsertUpdateFranchiseeMaintenanceTemp(model);
            return Json("Sucess");
        }

        [HttpPost]
        public JsonResult UpdateFranchiseeMaintenanceTemp(EditFranchiseeMaintenanceViewModel model, FormCollection frm)
        {
            FranchiseeService.UpdateFranchiseeMaintenanceTemp(model);
            return Json("Sucess");
        }



        [HttpGet]
        public ActionResult AddEditOwnerOld(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContact().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentification().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList).FirstOrDefault().ToModel<IdentificationViewModel, Identification>();

                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwner.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwner.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpPost]
        public ActionResult AddEditOwnerOld(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();

                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        }

        [HttpGet]
        public ActionResult GetFranchiseeOwnerOld(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwner.cshtml", FullFranchiseeViewModel.FranchiseeOwners);
        }
        [HttpGet]
        public ActionResult AddEditOwnerTemp(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContactTemp().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();

                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopupTemp.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopupTemp.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult StepTwoAddEditOwnerTemp(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContactTemp().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.GetFranchiseeOwnerListByOwnerId_Temp(ownerId);
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_MaintenenceTwoAddEditOwner.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_MaintenenceTwoAddEditOwner.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult AddEditOwner(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContactTemp().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();

                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwner.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwner.cshtml", FullFranchiseeViewModel);
            }
        }

        //[HttpPost]
        //public ActionResult AddEditOwner(FullFranchiseeViewModel FullFranchiseeViewModel)
        //{
        //    if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
        //    {
        //        int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
        //        int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

        //        return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

        //    }
        //    else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
        //    {
        //        int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
        //        int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = FullFranchiseeViewModel.FranchiseeIdByOwner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;


        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
        //        FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

        //        return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
        //    //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        //}

        [HttpPost]
        public ActionResult AddEditOwner(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();

                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        }

        [HttpPost]
        public ActionResult AddEditOwnerModel(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;
                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();


                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();
                int? classid = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList(classid, FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId);
                if (FranchiseeOwnerData == null)
                {
                    FranchiseeOwnerListModel franchiseeOwnerListModel = new FranchiseeOwnerListModel();
                    franchiseeOwnerListModel.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                    franchiseeOwnerListModel.EmailId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.EmailId;
                    franchiseeOwnerListModel.AddressId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.AddressId;
                    franchiseeOwnerListModel.PhoneId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.PhoneId;
                    franchiseeOwnerListModel.ContactId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                    franchiseeOwnerListModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    franchiseeOwnerListModel.CreatedDate = DateTime.Now;
                    franchiseeOwnerListModel = FranchiseeService.SaveFranchiseeOwnerList(franchiseeOwnerListModel.ToModel<FranchiseeOwnerList, FranchiseeOwnerListModel>()).ToModel<FranchiseeOwnerListModel, FranchiseeOwnerList>();
                }


                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;
                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();


                FranchiseeOwnerListModel franchiseeOwnerListModel = new FranchiseeOwnerListModel();
                franchiseeOwnerListModel.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                franchiseeOwnerListModel.EmailId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.EmailId;
                franchiseeOwnerListModel.AddressId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.AddressId;
                franchiseeOwnerListModel.PhoneId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.PhoneId;
                franchiseeOwnerListModel.ContactId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                franchiseeOwnerListModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                franchiseeOwnerListModel.CreatedDate = DateTime.Now;
                franchiseeOwnerListModel = FranchiseeService.SaveFranchiseeOwnerList(franchiseeOwnerListModel.ToModel<FranchiseeOwnerList, FranchiseeOwnerListModel>()).ToModel<FranchiseeOwnerListModel, FranchiseeOwnerList>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        }

        //New
        [HttpPost]
        public ActionResult AddEditFranchiseeOwnerModel(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0 && FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeOwnerListId > 0)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList>();

                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner == -1)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList_Temp(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList_Temp, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
        }

        //Add New Function for Add owner Temp in Step2
        //New
        [HttpPost]
        public ActionResult StepTwoAddEditFranchiseeOwnerTemp(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0 && FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeOwnerListId > 0)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                //FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList>();
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList_Temp(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList_Temp, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList_Temp>();
                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList_Temp(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList_Temp, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner == -1)
            {
                FullFranchiseeViewModel.FranchiseOwnerList.FranchiseeId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseOwnerList.ModifiedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseOwnerList.IsActive = true;
                FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.SaveFranchiseeOwnerList_Temp(FullFranchiseeViewModel.FranchiseOwnerList.ToModel<FranchiseeOwnerList_Temp, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFranchiseeOwner(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListById(FranchiseeId);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwner.cshtml", FullFranchiseeViewModel.FranchiseeOwners);
        }

        //New
        [HttpGet]
        public ActionResult GetFranchiseeOwnerList(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListById(FranchiseeId);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwner.cshtml", FullFranchiseeViewModel.FranchiseOwnersList);
        }

        [HttpGet]
        public ActionResult GetFranchiseeOwnerTemp(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListByIdTemp(FranchiseeId);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwner.cshtml", FullFranchiseeViewModel.FranchiseOwnersList);
        }

        [HttpGet]
        public ActionResult GetFranchiseeOwner_Temp(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListByIdTemp(FranchiseeId);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwner.cshtml", FullFranchiseeViewModel.FranchiseeOwners);
        }

        [HttpGet]
        public ActionResult MaintenanceGetFranchiseeOwner_Temp(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListByIdTemp(FranchiseeId);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            return View("~/Areas/Portal/Views/Franchise/_FranchiseeOwnerTemp.cshtml", FullFranchiseeViewModel.FranchiseeOwners);
        }

        [HttpGet]
        public ActionResult GetFranchiseeContact(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContact().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();

            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            return View("~/Areas/Portal/Views/Franchise/_EditFranchiseeMainContact.cshtml", FullFranchiseeViewModel);
        }

        [HttpGet]
        public ActionResult GetFranchiseeContactTemp(string id)
        {
            int FranchiseeId = Convert.ToInt32(id);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContactTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();

            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
            return View("~/Areas/Portal/Views/Franchise/_EditFranchiseeMainContactTemp.cshtml", FullFranchiseeViewModel);
        }

        #endregion


        #endregion

        #region Franchisee > Chargebacks

        //Letters
        [HttpGet]
        public ActionResult ChargebackLetterGenerate()
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackLetterGenerate", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("ChargebackLetterGenerate", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackLetterGenerate", "Franchise", new { area = "Portal" }), "Letters");

            ViewBag.billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ViewBag.billMonthsList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult ChargebackLetterGenerate(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            return RedirectToAction("ChargebackLetterList", new { month = collection["BillMonth"].ToString(), year = collection["BillYear"].ToString() });
        }

        [HttpGet]
        public ActionResult ChargebackLetterList(string month, string year)
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackLetterGenerate", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackLetterGenerate", "Franchise", new { area = "Portal" }), "Letters");

            return View();
        }

        //Possible
        [HttpGet]
        public ActionResult ChargebackPossible()
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";

            serv = new FranchiseService();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackPossible", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackPossible", "Franchise", new { area = "Portal" }), "Possible");

            ICollection<SelectListItem> billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> billMonthList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billMonthsList = billMonthList;
            ViewBag.billYearList = billYearList;

            return View();
        }
        [HttpPost]
        public ActionResult ChargebackPossible(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            Session["BillMonth"] = collection["BillMonth"].ToString();
            Session["BillYear"] = collection["BillYear"].ToString();
            return RedirectToAction("ChargebackPossibleList");

        }
        [HttpGet]
        public ActionResult ChargebackPossibleList()
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackPossible", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackPossible", "Franchise", new { area = "Portal" }), "Possible");
            serv = new FranchiseService();

            string BillMonth = Session["BillMonth"].ToString();
            string BillYear = Session["BillYear"].ToString();
            //List<TransactionFranchiseSearch> lstfranchsrch = serv.GetTransactionSearchList(int.Parse(Searchby), SearchValue, TransactionDateFrom, TransactionDateTo, CreateDateFrom, CreateDateTo, int.Parse(BillMonth), int.Parse(BillYear), int.Parse(FilterBy), int.Parse(TransactionType)).ToList();
            //return View(lstfranchsrch);
            return View(new List<TransactionFranchiseSearch>());
        }


        //Turnaround
        [HttpGet]
        public ActionResult ChargebackTurnaround()
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            serv = new FranchiseService();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackTurnaround", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackTurnaround", "Franchise", new { area = "Portal" }), "Turnaround");

            ICollection<SelectListItem> billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> billMonthList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billMonthsList = billMonthList;
            ViewBag.billYearList = billYearList;

            return View(new List<TransactionFranchiseSearch>());
        }

        //View
        [HttpGet]
        public ActionResult ChargebackSearch()
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            serv = new FranchiseService();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackSearch", "Franchise", new { area = "Portal" }), "Chargebacks");
            BreadCrumb.Add(Url.Action("ChargebackSearch", "Franchise", new { area = "Portal" }), "View");

            ICollection<SelectListItem> billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> billMonthList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billMonthsList = billMonthList;
            ViewBag.billYearList = billYearList;

            return View();
        }
        [HttpPost]
        public ActionResult ChargebackSearch(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseChargebacks";
            Session["FranchiseNo"] = collection["txtfranchiseno"].ToString();
            Session["BillMonth"] = collection["BillMonth"].ToString();
            Session["BillYear"] = collection["BillYear"].ToString();
            return RedirectToAction("ChargebackList");

        }
        [HttpGet]
        public ActionResult ChargebackList()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargebackList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("ChargebackList", "Franchise", new { area = "Portal" }), "Transactions");
            BreadCrumb.Add(Url.Action("ChargebackList", "Franchise", new { area = "Portal" }), "Chargebacks");
            var comboFranchise = FranchiseeService.GetMasterTrxTypeListForFranchise();
            var combodata = comboFranchise.Select(o => new SelectListItem
            {
                Value = o.MasterTrxTypeListId.ToString(),
                Text = o.Name
            });
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.FranchiseTransactionType = combodata;
            return View();
        }


        #endregion

        #region Franchisee > Negative Dues
        [HttpGet]
        public ActionResult NegativeDuePossible()
        {
            ViewBag.CurrentMenu = "FranchiseNegativeDues";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NegativeDuePossible", "Franchise", new { area = "Portal" }), "Negative Dues");
            BreadCrumb.Add(Url.Action("NegativeDuePossible", "Franchise", new { area = "Portal" }), "Possible");
            return View();
        }
        //View
        [HttpGet]
        public ActionResult NegativeDueSearch()
        {
            ViewBag.CurrentMenu = "FranchiseNegativeDues";
            serv = new FranchiseService();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NegativeDueSearch", "Franchise", new { area = "Portal" }), "Possible");
            BreadCrumb.Add(Url.Action("NegativeDueSearch", "Franchise", new { area = "Portal" }), "View");

            ViewBag.billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ViewBag.billMonthsList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult NegativeDueSearch(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseNegativeDues";
            Session["FranchiseNo"] = collection["txtfranchiseno"].ToString();
            Session["BillMonth"] = collection["BillMonth"].ToString();
            Session["BillYear"] = collection["BillYear"].ToString();
            return RedirectToAction("NegativeDueList");

        }
        [HttpGet]
        public ActionResult NegativeDueList()
        {
            ViewBag.CurrentMenu = "FranchiseNegativeDues";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NegativeDueSearch", "Franchise", new { area = "Portal" }), "Possible");
            BreadCrumb.Add(Url.Action("NegativeDueSearch", "Franchise", new { area = "Portal" }), "View");
            serv = new FranchiseService();

            string FranchiseNo = Session["FranchiseNo"].ToString();
            string BillMonth = Session["BillMonth"].ToString();
            string BillYear = Session["BillYear"].ToString();
            //List<TransactionFranchiseSearch> lstfranchsrch = serv.GetTransactionSearchList(int.Parse(Searchby), SearchValue, TransactionDateFrom, TransactionDateTo, CreateDateFrom, CreateDateTo, int.Parse(BillMonth), int.Parse(BillYear), int.Parse(FilterBy), int.Parse(TransactionType)).ToList();
            //return View(lstfranchsrch);
            return View(new List<TransactionFranchiseSearch>());
        }

        #endregion

        #region Franchisee > Transactions

        //Franchisee > Transactions > Single


        [HttpGet]
        public ActionResult Transaction(string recurring)
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            bool IsRecurring = false;
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Transaction", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Transaction", "Franchise", new { area = "Portal" }), "Transaction");

            if (!String.IsNullOrEmpty(recurring) && recurring == "1")
            {
                IsRecurring = true;
                BreadCrumb.Add(Url.Action("Transaction", "Franchise", new { area = "Portal" }) + "?recurring=1", "Recurring");
            }
            else
            {
                BreadCrumb.Add(Url.Action("Transaction", "Franchise", new { area = "Portal" }), "Single");
            }

            FranchiseService serv = new FranchiseService();

            //Check In Cache if exists or not 
            if (_cacheProvider.Get(CacheKeyName.GetTransactionTypeList) == null)
            {
                _cacheProvider.Set(CacheKeyName.GetTransactionTypeList, serv.GetTransactionTypeListItem(0, 1).Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList());
            }

            //Check In Cache if exists or not 
            if (_cacheProvider.Get(CacheKeyName.GetFranchiseStatusList) == null)
            {
                _cacheProvider.Set(CacheKeyName.GetFranchiseStatusList, serv.GetFranchiseStatusListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.StatusListId.ToString() }).OrderBy(s => s.Text).ToList());
            }

            ICollection<SelectListItem> bindTypeList = (ICollection<SelectListItem>)_cacheProvider.Get(CacheKeyName.GetTransactionTypeList);
            ICollection<SelectListItem> bindStatusList = (ICollection<SelectListItem>)_cacheProvider.Get(CacheKeyName.GetFranchiseStatusList);

            ViewBag.TransactionType = bindTypeList;
            ViewBag.Status = bindStatusList;
            ViewBag.Type = IsRecurring;

            return View();
        }



        [HttpGet]
        public JsonResult GetFranchiseeDetail(string franchiseeid, bool? recurring)
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            serv = new FranchiseService();

            List<vFranchiseViewModel> lstFranchiseViewModel = serv.FranchiseSearchByNumber(franchiseeid).ToList();
            List<FranchiseTransaction> lstFranchiseTransaction = serv.ListRecurringTransaction(lstFranchiseViewModel.FirstOrDefault().id);

            var oFranchiseViewModel = lstFranchiseViewModel
                                      .Select(x => new
                                      {
                                          x,
                                          lstT = lstFranchiseTransaction
                                      }).ToList();


            return Json(oFranchiseViewModel, JsonRequestBehavior.AllowGet);
        }

        public FranchiseeDetailViewModel GetFranchiseeDetailData(int franchiseeid, bool? Isactive)
        {
            ViewBag.CurrentMenu = "FranchiseDetail";
            FranchiseeDetailViewModel FranchiseeViewModellist = new FranchiseeDetailViewModel();
            try
            {
                FranchiseeViewModellist = FranchiseeService.GetFranchiseeDetail(franchiseeid);
                return FranchiseeViewModellist;
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return FranchiseeViewModellist;
            }
        }

        public FranchiseeDetailViewModel GetFranchiseeDetailDataTemp(int franchiseeid, bool? Isactive)
        {
            ViewBag.CurrentMenu = "FranchiseDetail";
            FranchiseeDetailViewModel FranchiseeViewModellist = new FranchiseeDetailViewModel();
            try
            {
                FranchiseeViewModellist = FranchiseeService.GetFranchiseeDetailTemp(franchiseeid);
                return FranchiseeViewModellist;
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return FranchiseeViewModellist;
            }
        }


        [HttpGet]
        public ActionResult TransactionList(int id)
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            serv = new FranchiseService();
            try
            {
                List<FranchiseTransaction> lstFranchiseTransaction = serv.ListRecurringTransaction(id);
                return PartialView("_ViewTrasactionList", lstFranchiseTransaction);

            }
            catch (Exception)
            {
            }
            finally
            {
                serv = null;
            }
            return PartialView(new List<FranchiseTransaction>());
        }

        [HttpGet]
        public ActionResult TransactionRecurringList(int id)
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            serv = new FranchiseService();
            try
            {
                List<FranchiseTransaction> lstFranchiseTransaction = serv.ListRecurringTransaction(id);
                return PartialView("_ViewTrasactionRecuringList", lstFranchiseTransaction);

            }
            catch (Exception)
            {
            }
            finally
            {
                serv = null;
            }
            return PartialView(new List<FranchiseTransaction>());
        }

        [HttpPost]
        public JsonResult SaveFranchiseeTransaction(int? id, int? franid, int? trxtypeid, DateTime? trxdate, int? credit, int? resell, decimal? quantity, decimal? unitprice, decimal? extendedprice, decimal? subtotal, decimal? tax, decimal? trxtotal, string description, int? noofpayments, int? paymentsbilled, DateTime? startdate, int? status, decimal? grosstotal, int? userid)
        {
            FranchiseService serv = new FranchiseService();
            var retVal = false;
            if (serv.SaveTransaction(id, franid, trxtypeid, trxdate, credit, resell, quantity, unitprice, extendedprice, subtotal, tax, trxtotal, description, noofpayments, paymentsbilled, startdate, status, grosstotal, 1))
            {
                retVal = true;
            }
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        //Transaction Single
        [HttpGet]
        public ActionResult TransactionSingle()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionSingle", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("TransactionSingle", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionSingle", "Franchise", new { area = "Portal" }), "Single");

            FranchiseService serv = new FranchiseService();

            ICollection<SelectListItem> bindTypeList = serv.GetTransactionTypeListItem(0, 1).Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> bindStatusList = serv.GetFranchiseStatusListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.StatusListId.ToString() }).OrderBy(s => s.Text).ToList();

            ViewBag.TransactionType = bindTypeList;
            ViewBag.Status = bindStatusList;

            return View();
        }

        [HttpGet]
        public ActionResult TransactionSearch()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Search", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("Search", "Franchise", new { area = "Portal" }), "Search");

            ViewBag.FranchiseeId = -1;
            FranchiseService serv = new FranchiseService();

            List<KeyValuePair<string, int>> bindfilterby = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.FilterBy>();
            List<KeyValuePair<string, int>> bindSearchby = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.SearchBy>();
            ICollection<SelectListItem> bindTransactionTypelist = serv.GetTransactionTypeListItem(0).Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> bindfilterbylist = bindfilterby.Select(k => new SelectListItem { Text = k.Key, Value = k.Value.ToString() }).ToList();
            ICollection<SelectListItem> bindSearchbytlist = bindSearchby.Select(k => new SelectListItem { Text = k.Key, Value = k.Value.ToString() }).ToList();
            ICollection<SelectListItem> billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> billMonthList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billMonthsList = billMonthList;
            ViewBag.billYearList = billYearList;
            ViewBag.FilterBy = bindfilterbylist;
            ViewBag.SearchBy = bindSearchbytlist;
            ViewBag.TransactionType = bindTransactionTypelist;


            return View();
        }

        public JsonResult GetFranchiseeDetail(string franchiseeid)
        {
            FranchiseService serv = new FranchiseService();
            List<vFranchiseViewModel> lstFranchiseViewModel = serv.FranchiseSearchByNumber(franchiseeid).ToList();
            List<FranchiseTransaction> lstFranchiseTransaction = serv.ListRecurringTransaction(lstFranchiseViewModel.FirstOrDefault().id);

            var oFranchiseViewModel = lstFranchiseViewModel
                                      .Select(x => new
                                      {
                                          x,
                                          lstT = lstFranchiseTransaction
                                      }).ToList();


            return Json(oFranchiseViewModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFranchiseeDetail(int? id, int? franid, int? trxtypeid, DateTime? trxdate, int? credit, int? resell, decimal? quantity, decimal? unitprice, decimal? extendedprice, decimal? subtotal, decimal? tax, decimal? trxtotal, string description, int? noofpayments, int? paymentsbilled, DateTime? startdate, int? status, decimal? grosstotal, int? userid)
        {
            FranchiseService serv = new FranchiseService();
            var retVal = false;
            if (serv.SaveTransaction(id, franid, trxtypeid, trxdate, credit, resell, quantity, unitprice, extendedprice, subtotal, tax, trxtotal, description, noofpayments, paymentsbilled, startdate, status, grosstotal, userid))
            {
                retVal = true;
            }
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TransactionSearch(FormCollection collection)
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";

            Session["Searchby"] = collection["Searchby"].ToString();
            Session["SearchValue"] = collection["SearchValue"].ToString();
            Session["TransactionDateFrom"] = collection["TransactionDateFrom"].ToString();
            Session["TransactionDateTo"] = collection["TransactionDateTo"].ToString();
            Session["CreateDateFrom"] = collection["CreateDateFrom"].ToString();
            Session["CreateDateTo"] = collection["CreateDateTo"].ToString();
            Session["BillMonth"] = collection["BillMonth"].ToString();
            Session["BillYear"] = collection["BillYear"].ToString();
            Session["TransactionType"] = collection["TransactionType"].ToString();
            Session["FilterBy"] = collection["FilterBy"].ToString();
            return RedirectToAction("TransactionSearchList");

        }
        [HttpGet]
        public ActionResult TransactionSearchList()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionSearch", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionSearch", "Franchise", new { area = "Portal" }), "Search");
            FranchiseService serv = new FranchiseService();
            string Searchby = Session["Searchby"].ToString();
            string SearchValue = Session["SearchValue"].ToString();
            string TransactionDateFrom = Session["TransactionDateFrom"].ToString();
            string TransactionDateTo = Session["TransactionDateTo"].ToString();
            string CreateDateFrom = Session["CreateDateFrom"].ToString();
            string CreateDateTo = Session["CreateDateTo"].ToString();
            string BillMonth = Session["BillMonth"].ToString();
            string BillYear = Session["BillYear"].ToString();
            string TransactionType = Session["TransactionType"].ToString();
            string FilterBy = Session["FilterBy"].ToString();
            List<TransactionFranchiseSearch> lstfranchsrch = serv.GetTransactionSearchList(int.Parse(Searchby), SearchValue, TransactionDateFrom, TransactionDateTo, CreateDateFrom, CreateDateTo, int.Parse(BillMonth), int.Parse(BillYear), int.Parse(FilterBy), int.Parse(TransactionType)).ToList();
            return View(lstfranchsrch);
        }
        [HttpGet]
        public ActionResult TransactionPendingList()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionPendingList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("TransactionPendingList", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionPendingList", "Franchise", new { area = "Portal" }), "Pending");

            FranchiseService serv = new FranchiseService();
            List<TransactionFranchisePending> lstfranchsrch = serv.GetTransactionPendingList("-1").ToList();

            return View(lstfranchsrch);
        }

        public ActionResult TransactionImport()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionImport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("TransactionImport", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionImport", "Franchise", new { area = "Portal" }), "Import");

            FranchiseService serv = new FranchiseService();
            List<TransactionFranchisePending> lstfranchsrch = serv.GetTransactionPendingList("-1").ToList();

            return View(lstfranchsrch);
        }
        [HttpGet]
        public ActionResult TransactionRecurring()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionRecurring", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("TransactionRecurring", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionRecurring", "Franchise", new { area = "Portal" }), "Recurring");

            FranchiseService serv = new FranchiseService();

            ICollection<SelectListItem> bindTypeList = serv.GetTransactionTypeListItem(0, 1).Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ICollection<SelectListItem> bindStatusList = serv.GetFranchiseStatusListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.StatusListId.ToString() }).OrderBy(s => s.Text).ToList();

            ViewBag.TransactionType = bindTypeList;
            ViewBag.Status = bindStatusList;

            return View();
        }

        public ActionResult AddNewTransaction()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Add New");

            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.FranchiseeManualTransaction == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList().Where(o => o.TypeListId == 2).ToList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");

            return View(new FranchiseeTransactionViewModel());
        }

        [HttpPost]
        public ActionResult AddNewTransaction(FranchiseeTransactionViewModel model, FormCollection collection)
        {
            model.IsActive = true;
            model.NumOfPayments = model.NumOfPayments != null ? model.NumOfPayments : 1;
            model.PaymentsBilled = model.PaymentsBilled != null ? model.PaymentsBilled : 0;
            model.StatusListId = 1;// model.StatusListId != null ? model.StatusListId : 1;
            model.GrossTotal = model.NumOfPayments * model.Total;
            model.MasterTrxTypeListId = model.MasterTrxTypeListId != null ? model.MasterTrxTypeListId : 14;
            model.StartDate = model.StartDate != null ? model.StartDate : DateTime.Now.Date;
            model.MasterTrxTypeListId = 14;
            FranchiseeService.CreateFranchiseeManualTrasactionSave(model);
            if (collection["SaveClose1"] != null)
                return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
            else if (collection["hdfSaveClose"] == "SaveClose")
                return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
            else
                return RedirectToAction("AddNewTransaction", "Franchise", new { area = "Portal" });
        }

        [HttpPost]
        public ActionResult FranchiseeManualTrasactionEditUpdate(ManualTransactionViewModel model, FormCollection collection)
        {
            FranchiseeService.UpdateFranchiseeManualTrasaction(model);

            return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
        }

        [HttpPost]
        public ActionResult AddNewCreditTransaction(FranchiseeTransactionViewModel model, FormCollection collection)
        {
            model.IsActive = true;
            model.NumOfPayments = model.NumOfPayments != null ? model.NumOfPayments : 1;
            model.PaymentsBilled = model.PaymentsBilled != null ? model.PaymentsBilled : 0;
            model.StatusListId = 1;// model.StatusListId != null ? model.StatusListId : 1;
            model.GrossTotal = model.NumOfPayments * model.Total;
            model.MasterTrxTypeListId = 20;
            model.FranchiseeManualTrxCreditReasonListId = (collection["slReasonList"] == null ? 0 : Convert.ToInt32(collection["slReasonList"]));
            FranchiseeService.CreateFranchiseeManualTrasactionSave(model);
            if (collection["SaveClose1"] != null)
                return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
            else
                return RedirectToAction("FranchiseeCredit", "Franchise", new { area = "Portal" });
        }

        [HttpGet]
        public ActionResult FranchiseeChargeback()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeChargeback", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("FranchiseeChargeback", "Franchise", new { area = "Portal" }), "Franchisee Chargeback");
            ViewBag.OptionList = new SelectList(FranchiseeService.GetAll_OptionList(), "SearchDateListId", "Name");
            return View();
        }

        [HttpGet]
        public ActionResult FranchiseeChargebackList()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeChargebackList", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("FranchiseeChargebackList", "Franchise", new { area = "Portal" }), "Franchisee Chargeback");
            ViewBag.OptionList = new SelectList(FranchiseeService.GetAll_OptionList(), "SearchDateListId", "Name");
            return View();
        }
        public JsonResult FrenchiseeDetailTransactions(int FrenchiseeId, int TypeId, DateTime StartDate, DateTime EndDate)
        {
            return Json(serv.GetFrenchiseeDetailTeasactions(FrenchiseeId, TypeId, StartDate, EndDate), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FranchiseeCredit()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeCredit", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeCredit", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("FranchiseeCredit", "Franchise", new { area = "Portal" }), "Franchisee Credit");
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.IsCredit == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");
            return View(new FranchiseeTransactionViewModel());
        }

        public ActionResult TransactionPending()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TransactionPending", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("TransactionPending", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("TransactionPending", "Franchise", new { area = "Portal" }), "Transaction Pending");
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            FranchiseService serv = new FranchiseService();
            List<FranchiseeManualTransactionResultViewModel> lstfranchsrch = serv.GetManualTransactionPendingList(SelectedRegionId.ToString()).ToList();

            return View(lstfranchsrch);
        }

        public ActionResult ManualTrasactionForEdit(int id)
        {
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList().Where(s => s.TypeListId == 2).ToList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");
            FranchiseeTransactionViewModel oFranchiseeTransaction = FranchiseeService.GetFranchiseeManualTrasactionForEdit(id);

            if (oFranchiseeTransaction.MasterTrxTypeListId == 8)
            {
                ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.IsCredit == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
                return PartialView("_PartialCreditTransaction", oFranchiseeTransaction);
            }
            else
            {
                ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().OrderBy(x => x.name), "ServiceTypeListId", "Name");
                return PartialView("_PartialManualTransaction", oFranchiseeTransaction);
            }
        }

        public ActionResult EditManualTrasaction(string TrxNo)
        {
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList().Where(s => s.TypeListId == 2).ToList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");
            ManualTransactionViewModel oFranchiseeTransaction = FranchiseeService.EditManualTrasaction(TrxNo);
            //ViewBag.FranchiseeTransactionTypeList = new SelectList(FranchiseeService.GetFranchiseeTransactionTypeList().OrderBy(x => x.Name), "FranchiseeTransactionTypeListId", "Name");
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.FranchiseeManualTransaction == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            return PartialView("_PartialEditUpdateManualTransaction", oFranchiseeTransaction);
        }

        public ActionResult ManualTrasactionForDelete(int id)
        {
            FranchiseeService.GetFranchiseeManualTrasactionForDelete(id);
            return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
        }

        [HttpPost]
        public ActionResult FranchiseeManualTrasactionSave(FranchiseeTransactionViewModel model, FormCollection collection)
        {
            model.IsActive = true;
            model.NumOfPayments = model.NumOfPayments != null ? model.NumOfPayments : 1;
            model.PaymentsBilled = model.PaymentsBilled != null ? model.PaymentsBilled : 0;
            model.StatusListId = 1; // model.StatusListId != null ? model.StatusListId : 12;
            model.GrossTotal = model.NumOfPayments * model.Total;

            if (collection["SaveApprove"] != null)
            {
                FranchiseeService.SaveFranchiseeManualTrasactionForEdit(model, true);
                AccountReceivableService.saveCommonPendingMessage(collection["txtFranchiseeTrasactionStatusNote"].ToString(), 1, 0, "Franchisee Manual Transaction", Convert.ToInt32(model.FranchiseeId), 2, 14, model.FranchiseeManualTransactionTempId);
                return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
            }
            else if (collection["SaveReject"] != null)
            {
                FranchiseeService.SaveFranchiseeManualTrasactionForEdit(model, false);
                AccountReceivableService.saveCommonPendingMessage(collection["txtFranchiseeTrasactionStatusNote"].ToString(), 2, 0, "Franchisee Manual Transaction", Convert.ToInt32(model.FranchiseeId), 2, 14, model.FranchiseeManualTransactionTempId);
                return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
            }
            else
            {
                FranchiseeService.SaveFranchiseeManualTrasactionForEdit(model, false);
                return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
            }
        }




        [HttpGet]
        public ActionResult FranchiseeManualTrasactionApproved(string Ids, string note)
        {
            if (Ids != "")
            {
                foreach (string id in Ids.Split(','))
                {
                    if (id.Trim() != "")
                        FranchiseeService.SaveFranchiseeManualTrasactionForApproved(id, true, note);
                }
            }
            return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
        }
        [HttpGet]
        public ActionResult FranchiseeManualTrasactionRejected(string Ids, string note)
        {
            if (Ids != "")
            {
                foreach (string id in Ids.Split(','))
                {
                    if (id.Trim() != "")
                        FranchiseeService.SaveFranchiseeManualTrasactionForApproved(id, false, note);
                }
            }
            return RedirectToAction("TransactionPending", "Franchise", new { area = "Portal" });
        }

        #endregion

        #region Franchisee > Franchisee Report

        [HttpGet]
        public ActionResult FranchiseeReportsView()
        {
            ViewBag.CurrentMenu = "FranchiseFranchiseeReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeReportsView", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeReportsView", "Franchise", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("FranchiseeReportsView", "Franchise", new { area = "Portal" }), "View");
            return View();
        }
        [HttpGet]
        public ActionResult FranchiseeReportsFinalize()
        {
            ViewBag.CurrentMenu = "FranchiseFranchiseeReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeReportsFinalize", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeReportsFinalize", "Franchise", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("FranchiseeReportsFinalize", "Franchise", new { area = "Portal" }), "Finalize");
            ViewBag.billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ViewBag.billMonthsList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            return View();
        }
        [HttpGet]
        public ActionResult FranchiseeReportsGenerate()
        {
            ViewBag.CurrentMenu = "FranchiseFranchiseeReport";
            BreadCrumb.Add(Url.Action("FranchiseeReportsGenerate", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeReportsGenerate", "Franchise", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("FranchiseeReportsGenerate", "Franchise", new { area = "Portal" }), "Generate");

            return View();
        }
        #endregion

        #region Franchisee > Customer Leads
        [HttpGet]
        public ActionResult CustomerLeadsReport()
        {
            ViewBag.CurrentMenu = "FranchiseCustomerLeads";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "Customer Leads");
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "Report");
            return View();
        }
        [HttpGet]
        public ActionResult FindersFeeBalances()
        {
            ViewBag.CurrentMenu = "FranchiseCustomerLeads";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FindersFeeBalances", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FindersFeeBalances", "Franchise", new { area = "Portal" }), "Customer Leads");
            BreadCrumb.Add(Url.Action("FindersFeeBalances", "Franchise", new { area = "Portal" }), "Finders Fees");
            ViewBag.billYearList = serv.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ViewBag.billMonthsList = serv.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }
        [HttpGet]
        public ActionResult NoteList()
        {
            ViewBag.CurrentMenu = "FranchiseCustomerLeads";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NoteList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("NoteList", "Franchise", new { area = "Portal" }), "Customer Leads");
            BreadCrumb.Add(Url.Action("NoteList", "Franchise", new { area = "Portal" }), "Notes");
            return View();
        }
        #endregion

        #region Franchisee > Lease
        public ActionResult FranchiseeLease(int id = 0)
        {
            ViewBag.CurrentMenu = "FranchiseLease";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "Lease");
            BreadCrumb.Add(Url.Action("CustomerLeadsReport", "Franchise", new { area = "Portal" }), "New Lease");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            ViewBag.franchiseeId = id;
            LeaseViewModel ViewModel = new LeaseViewModel();
            return View(ViewModel);
        }

        [HttpPost]
        public ActionResult FranchiseeLease(LeaseViewModel leaseViewModel, FormCollection collection)
        {
            if (leaseViewModel.LeaseNumber != null)
            {
                leaseViewModel.CreatedDate = DateTime.Now;
                leaseViewModel.ModifiedDate = DateTime.Now;
                leaseViewModel.TypeListId = 2;
                leaseViewModel.IsActive = 1;
                leaseViewModel.RegionId = SelectedRegionId;
                leaseViewModel.CreatedBy = SelectedUserId;

                leaseViewModel.DownPaymentPaid = false;

                if (leaseViewModel.InstallmentDownPaymentNum > 0)
                {
                    leaseViewModel.NumOfPaymentsPaid = -1;
                }
                else
                {
                    leaseViewModel.NumOfPaymentsPaid = 0;
                }
                leaseViewModel.TotalAmountPaid = 0;
                leaseViewModel = FranchiseeService.SaveLease(leaseViewModel.ToModel<Lease, LeaseViewModel>()).ToModel<LeaseViewModel, Lease>();
            }

            if (Convert.ToInt32(collection["ButtonType"]) == 1)
            {
                return RedirectToAction("FranchiseeLease", "Franchise", new { area = "Portal" });
            }
            else if (Convert.ToInt32(collection["ButtonType"]) == 2)
            {
                return RedirectToAction("LeaseList", "Franchise", new { area = "Portal" });
            }
            return RedirectToAction("FranchiseeLease", "Franchise", new { area = "Portal" });
        }

        public ActionResult LeaseReport()
        {
            ViewBag.CurrentMenu = "FranchiseLease";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Lease");
            BreadCrumb.Add(Url.Action("LeaseReport", "Franchise", new { area = "Portal" }), "Report");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult LeaseList()
        {
            ViewBag.CurrentMenu = "FranchiseLease";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LeaseList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("LeaseList", "Franchise", new { area = "Portal" }), "Lease");
            BreadCrumb.Add(Url.Action("LeaseList", "Franchise", new { area = "Portal" }), "Lease List");
            ViewBag.OptionList = new SelectList(FranchiseeService.GetAll_OptionList(), "SearchDateListId", "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            //ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 7).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 21);

            return View();
        }

        public JsonResult LeaseListDataWithSearch(string statusIds = "", string RegionIds = "", string searchtext = "")
        {
            var retVal = serv.FranchiseeLeaseListDataSearch(statusIds, RegionIds, searchtext);
            var result = (from f in retVal
                          select new
                          {
                              f.FranchiseeId,
                              f.FranchiseeNo,
                              f.FranchiseeName,
                              f.LeaseCount,
                              f.IsActive,
                              RegionName = serv.getRegionName(f.RegionId),
                              f.TotalBalance,
                              f.TotalMonthlyPaymentAmount,
                              f.TotalPaidAmount,
                              f.Status,
                              f.StatusListId,
                          }).ToList();
            return Json(new { LeaseData = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FranchiseeManualTransactionsListDataWithSearch(string searchtext, string startDate, string endDate, string RegionIds, int month = 0, int year = 0)
        {
            var retVal = serv.FranchiseeManualTransactionsListData(RegionIds, searchtext, startDate, endDate, month, year);
            var result = (from f in retVal
                          select new
                          {
                              f.Region,
                              f.FranchiseeNo,
                              f.Name,
                              f.TrxType,
                              f.TrxDate,
                              f.Number,
                              f.Description,
                              f.ExtendedPrice,
                              f.Tax,
                              f.Fees,
                              f.TotalTrxAmount,
                              f.Status,
                              f.MasterTrxTypeListId,
                              f.AmountTypelistId
                          }).OrderBy(f => f.FranchiseeNo).ToList();
            return Json(new { FranisessData = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeaseReportListDataWithSearch(string searchtext, string startDate, string endDate, string RegionIds)
        {
            var retVal = serv.FranchiseeLeaseReportListData(RegionIds, searchtext, startDate, endDate);
            return Json(new { LeaseReportData = retVal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinderFeeListDataWithSearch(string startDate, string endDate, string RegionIds)
        {
            var retVal = serv.FranchiseeFinderFeeReportListData(RegionIds, startDate, endDate);
            return Json(new { ReportData = retVal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FLeaseListData(int franchiseeid, bool? IsActive, int? StatusListId)
        {
            var retVal = serv.GetFranchiseeLeaseList(franchiseeid, IsActive, StatusListId);

            var jsonResult = Json(new { FLeaseList = retVal }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult FranchiseeLeaseListDataForTransfer(int franchiseeid)
        {
            var retVal = serv.GetFranchiseeLeaseList(franchiseeid, true, null);
            return Json(new { FLeaseList = retVal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FranchiseeLeaseListDataForTransferSubmit(string Ids, int fid, int tofid)
        {
            var retVal = serv.TransferFranchiseeLease(fid, tofid, Ids);
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FranchiseeLeaseDetailData(int leaseId)
        {
            var retVal = serv.GetFranchiseeLeaseDetail(leaseId);
            //   var retVal = serv.FranchiseeLeaseListDataSearch(statusIds, RegionIds, searchtext);

            var jsonResult = Json(new { LeaseDetail = retVal }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult LeaseDetail()
        {
            ViewBag.CurrentMenu = "FranchiseLease";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LeaseDetail", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("LeaseDetail", "Franchise", new { area = "Portal" }), "Lease");
            BreadCrumb.Add(Url.Action("LeaseDetail", "Franchise", new { area = "Portal" }), "Lease Detail");
            return View();
        }



        [HttpGet]
        public ActionResult LeaseBill()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Bill Run");

            List<SelectListItem> monthsList = Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.billMonthsList = monthsList;

            var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
            List<SelectListItem> billYearList = new List<SelectListItem>();
            foreach (var y in yearList) { billYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() }); }

            ViewBag.billYearList = billYearList;


            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);



            return View();
        }

        [HttpGet]
        public JsonResult LeaseBillDetailData(int month, int year, int batchid)
        {
            LeaseBillRunSummaryDetailViewModel oLeaseBillRunSummaryDetailViewModel = serv.GetLeaseBillRunSummaryDetail(month, year, batchid);
            return Json(oLeaseBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GenerateInvoiceLeaseBill(int month, int year, bool IsCPIIncrease)
        {
            LeaseBillRunSummaryDetailViewModel oLeaseBillRunSummaryDetailViewModel = serv.GenerateInvoiceLeaseBillRun(month, year);
            return Json(oLeaseBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Franchisee > Report
        public ActionResult AccountHistory()
        {
            ViewBag.CurrentMenu = "FranchiseFranchiseeReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AccountHistory", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("AccountHistory", "Franchise", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("AccountHistory", "Franchise", new { area = "Portal" }), "Account History");
            //ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            return View();
        }

        [HttpGet]
        public ActionResult FinderFeeReport()
        {
            ViewBag.CurrentMenu = "FranchiseFinder";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FinderFeeReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FinderFeeReport", "Franchise", new { area = "Portal" }), "Finder Fee");
            BreadCrumb.Add(Url.Action("FinderFeeReport", "Franchise", new { area = "Portal" }), "Finder Fee Report");
            return View();
        }
        #endregion

        #region Franchisee > Finder Fee
        public ActionResult NewFinderFee()
        {
            ViewBag.CurrentMenu = "FranchiseFinder";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NewFinderFee", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("NewFinderFee", "Franchise", new { area = "Portal" }), "Finder Fee");
            BreadCrumb.Add(Url.Action("NewFinderFee", "Franchise", new { area = "Portal" }), "New Finder Fee");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);


            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);
            var franchisestatuslst = CustomerService.GetStatusList().Where(st => st.TypeListId == 10).ToList();
            ViewBag.FranchiseStatuslst = new SelectList(franchisestatuslst, "StatusListId", "Name", 31);
            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }
        [HttpGet]
        public JsonResult CustomerListForFinderFee(int regionid, int franchiseeid, string statuslistid)
        {
            try
            {

                //var franchisestatuslst = FranchiseeService.GetFinderFeeCustomerListStatus(regionid, statuslistid);
                //ViewBag.FranchiseStatuslst = new SelectList(franchisestatuslst, "StatusListId", "StatusName", 1);
                var customers = FranchiseeService.GetFinderFeeCustomersList(regionid, franchiseeid, statuslistid);


                var data = new
                {
                    aadata = customers,
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult FinderFeeList()
        {
            ViewBag.CurrentMenu = "FinderFeeList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FinderFeeList", "Franchise", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("FinderFeeList", "Franchise", new { area = "Portal" }), "Finder Fee List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;


            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 10).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 31);


            return View();
        }

        [HttpGet]
        public JsonResult FinderFeeListData(string regionIds = "", string statusIds = "")
        {

            var jsonResult = Json(FranchiseeService.GetFinderFeeList(regionIds, statusIds), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult FinderFeeListDataResult(string regionIds = "", string statusIds = "")
        {
            var data = FranchiseeService.GetFinderFeeList(regionIds, statusIds);
            //var data = FranchiseeService.GetFinderFeeList(regionIds);
            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult FinderFeeDetailListData(int frenchiseid, string regionIds = "0", string statusIds = "")
        {
            var jsonResult = Json(FranchiseeService.GetFinderFeeDetailList(frenchiseid, regionIds, statusIds), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult FinderFeeDetailDataPopup(int id)
        {
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == 10).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 31);
            return PartialView("_FinderFeeDetailPopup", FranchiseeService.GetFranchiseeFinderFeeDetailList(SelectedRegionId, id));
        }

        public ActionResult MonthlyFinderFeeReport()
        {
            ViewBag.CurrentMenu = "FranchiseFinder";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Finder Fee");
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Monthly Finder Fee Report");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult MonthlyFinderFeeListReport(string startDate = "", string endDate = "", string RegionIds = "", string ftype = "")
        {
            if (RegionIds == "")
            {
                RegionIds = SelectedRegionId.ToString();
            }
            if (startDate == "")
            {
                startDate = DateTime.Now.Month + "/01/" + DateTime.Now.Year;
                ftype = "3";
            }
            if (endDate == "")
            {
                endDate = DateTime.Now.Month + "/" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "/" + DateTime.Now.Year;
            }

            var retVal = serv.FranchiseeFinderFeeReportListData(RegionIds, startDate, endDate);

            ViewBag.CurrentMenu = "FranchiseFinder";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Finder Fee");
            BreadCrumb.Add(Url.Action("MonthlyFinderFeeReport", "Franchise", new { area = "Portal" }), "Monthly Finder Fee Report");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;

            ViewBag.ftype = ftype;
            ViewBag.fstartDate = startDate;
            ViewBag.fendDate = endDate;
            ViewBag.fregionIds = RegionIds;
            return View(retVal);
        }

        [HttpGet]
        public ActionResult MonthlyFinderFeeListReportPrint(string startDate = "", string endDate = "", string RegionIds = "", string ftype = "")
        {
            var retVal = serv.FranchiseeFinderFeeReportListDataForPrint(RegionIds, startDate, endDate);
            if (retVal != null && retVal.Count() > 0)
            {
                ViewBag.RemitTo = CustomerService.GetRemitToForRegion(SelectedRegionId);
                ViewBag.SDate = startDate;
                ViewBag.EDate = endDate;

                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("_MonthlyFinderFeeListReportPrint", retVal);

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HTMLContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion

        #region :: Edit FranchiseDetails Popup  :: 

        [HttpGet]
        public ActionResult RenderEditFranchiseDetailsPopup(int id, bool edit = true)
        {


            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            //int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);

            FullFranchiseeViewModel.BusinessInfo = FranchiseeService.GetFranchisee().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee>();
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            //FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            //FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContact().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
            FullFranchiseeViewModel.ContactInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();



            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner));
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListById(FranchiseeId);

            FullFranchiseeViewModel.ACHBankInfo = FranchiseeService.GetACHBank().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList).OrderByDescending(one => one.ACHBankId).FirstOrDefault().ToModel<ACHBankViewModel, ACHBank>();
            if (FullFranchiseeViewModel.ACHBankInfo == null)
            {
                FullFranchiseeViewModel.ACHBankInfo = new ACHBankViewModel();
                FullFranchiseeViewModel.ACHBankInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.ACHBankInfo.TypeListId = BusinessTypeList;
            }
            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            FullFranchiseeViewModel.PayeeIdentification = CustomerService.GetIdentification().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification>();
            if (FullFranchiseeViewModel.PayeeIdentification == null)
            {
                FullFranchiseeViewModel.PayeeIdentification = new IdentificationViewModel();
                FullFranchiseeViewModel.PayeeIdentification.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.PayeeIdentification.TypeListId = BusinessTypeList;
                FullFranchiseeViewModel.PayeeIdentification.ContactTypeListId = BusinessContactTypeList;
            }

            FullFranchiseeViewModel.PayeeInfo = FranchiseeService.GetFranchiseeBillSettings().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeBillSettingsId).FirstOrDefault().ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSetting>();
            if (FullFranchiseeViewModel.PayeeInfo == null)
            {
                FullFranchiseeViewModel.PayeeInfo = new FranchiseeBillSettingViewModel();
                FullFranchiseeViewModel.PayeeInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            }


            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FullFranchiseeViewModel.PayeeInfo.FranchiseeBillSettingsId;
            FullFranchiseeViewModel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            FullFranchiseeViewModel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            FullFranchiseeViewModel.PayeeInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FullFranchiseeViewModel.PayeeInfoAddress.ClassId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();

            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (edit)
            {
                ViewBag.Edit = true;
            }
            else
            {
                ViewBag.Edit = false;

            }

            return PartialView("_EditFranchisePopup", FullFranchiseeViewModel);
        }

        public ActionResult SaveEditFranchiseDetailsPopupTab1(int FranchiseeId, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode, int PhoneId, string Phone1, int EmailId, string EmailAddress, string Name)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0)
            {
                FranchiseeService.UpdateFranchiseeDetails(FranchiseeId, Name);
            }

            fullfranchiseeviewmodel.BusinessInfoAddress.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (StateId != "" && StateId != "0")
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.StateListId = Convert.ToInt32(StateId);
            }
            fullfranchiseeviewmodel.BusinessInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoAddress.AddressId = AddressId;
            fullfranchiseeviewmodel.BusinessInfoAddress.Address1 = Address1;
            fullfranchiseeviewmodel.BusinessInfoAddress.Address2 = Address2;
            fullfranchiseeviewmodel.BusinessInfoAddress.City = City;
            fullfranchiseeviewmodel.BusinessInfoAddress.PostalCode = PostalCode;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.BusinessInfoAddress.IsActive = true;
            var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.BusinessInfoAddress.FullAddress));
            if (_latlngBL.results.Count() > 0)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.BusinessInfoAddress.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
            }


            fullfranchiseeviewmodel.BusinessInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.BusinessInfoPhone.Phone1 = Phone1;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.BusinessInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.BusinessInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.BusinessInfoEmail.IsActive = true;

            //if (StateId != "" && StateId != "0")
            //{
            //    int id = Convert.ToInt32(StateId);
            //    string state = CustomerService.GetStatesName(id);
            //    fullfranchiseeviewmodel.BusinessInfoAddress.StateName = state.Trim();
            //}
            fullfranchiseeviewmodel.BusinessInfoAddress = CustomerService.AddNewAddressOldInactive(fullfranchiseeviewmodel.BusinessInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            fullfranchiseeviewmodel.BusinessInfoPhone = CustomerService.AddNewPhoneOldInactive(fullfranchiseeviewmodel.BusinessInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            fullfranchiseeviewmodel.BusinessInfoEmail = CustomerService.AddNewEmailOldInactive(fullfranchiseeviewmodel.BusinessInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

            return Json(new { AddressId = fullfranchiseeviewmodel.BusinessInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.BusinessInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.BusinessInfoEmail.EmailId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }
        public ActionResult SaveEditFranchiseDetailsPopupTab1Temp(int FranchiseeId, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode, int PhoneId, string Phone1, int EmailId, string EmailAddress, string Name)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0)
            {
                FranchiseeService.UpdateFranchiseeDetails(FranchiseeId, Name);
            }

            fullfranchiseeviewmodel.BusinessInfoAddress.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (StateId != "" && StateId != "0")
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.StateListId = Convert.ToInt32(StateId);
            }
            fullfranchiseeviewmodel.BusinessInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoAddress.AddressId = AddressId;
            fullfranchiseeviewmodel.BusinessInfoAddress.Address1 = Address1;
            fullfranchiseeviewmodel.BusinessInfoAddress.Address2 = Address2;
            fullfranchiseeviewmodel.BusinessInfoAddress.City = City;
            fullfranchiseeviewmodel.BusinessInfoAddress.PostalCode = PostalCode;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoAddress.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.BusinessInfoPhone.Phone = Phone1;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.BusinessInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.BusinessInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.BusinessInfoEmail.IsActive = true;

            //if (StateId != "" && StateId != "0")
            //{
            //    int id = Convert.ToInt32(StateId);
            //    string state = CustomerService.GetStatesName(id);
            //    fullfranchiseeviewmodel.BusinessInfoAddress.StateName = state.Trim();
            //}
            fullfranchiseeviewmodel.BusinessInfoAddress = CustomerService.AddNewAddressOldInactiveTemp(fullfranchiseeviewmodel.BusinessInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.BusinessInfoPhone = CustomerService.AddNewPhoneOldInactiveTemp(fullfranchiseeviewmodel.BusinessInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.BusinessInfoEmail = CustomerService.AddNewEmailOldInactiveTemp(fullfranchiseeviewmodel.BusinessInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();

            return Json(new { AddressId = fullfranchiseeviewmodel.BusinessInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.BusinessInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.BusinessInfoEmail.EmailId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }
        public ActionResult SaveEditFranchiseDetailsPopupTab2(int FranchiseeId, int ContactId, string Name, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode, int PhoneId, string Phone1, string PhoneExt, string Cell, int EmailId, string EmailAddress)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();

            fullfranchiseeviewmodel.ContactInfoAddress.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;

            if (StateId != "" && StateId != "0")
            {
                fullfranchiseeviewmodel.ContactInfoAddress.StateListId = Convert.ToInt32(StateId);
            }
            //fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoAddress.AddressId = AddressId;
            fullfranchiseeviewmodel.ContactInfoAddress.Address1 = Address1;
            fullfranchiseeviewmodel.ContactInfoAddress.Address2 = Address2;
            fullfranchiseeviewmodel.ContactInfoAddress.City = City;
            fullfranchiseeviewmodel.ContactInfoAddress.PostalCode = PostalCode;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoAddress.IsActive = true;

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone1 = Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneExt = PhoneExt;
            fullfranchiseeviewmodel.ContactInfoPhone.Cell = Cell;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;

            fullfranchiseeviewmodel.ContactInfo.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.ContactId = ContactId;
            fullfranchiseeviewmodel.ContactInfo.Name = Name;
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;

            fullfranchiseeviewmodel.ContactInfo = CustomerService.AddNewContactOldInactive(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
            fullfranchiseeviewmodel.ContactInfoAddress = CustomerService.AddNewAddressOldInactive(fullfranchiseeviewmodel.ContactInfoAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.AddNewPhoneOldInactive(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.AddNewEmailOldInactive(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

            return Json(new { ContactId = fullfranchiseeviewmodel.ContactInfo.ContactId, AddressId = fullfranchiseeviewmodel.ContactInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.ContactInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.ContactInfoEmail.EmailId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }

        public ActionResult SaveEditFranchiseContact(int FranchiseeId, int ContactId, string Name, int PhoneId, string Phone1, string PhoneExt, int EmailId, string EmailAddress, string Cell)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone1 = Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneExt = PhoneExt;
            fullfranchiseeviewmodel.ContactInfoPhone.Cell = Cell;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfo.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.ContactId = ContactId;
            fullfranchiseeviewmodel.ContactInfo.Name = Name;
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfo = CustomerService.AddNewContactOldInactive(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.AddNewPhoneOldInactive(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.AddNewEmailOldInactive(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

            return Json(new { ContactId = fullfranchiseeviewmodel.ContactInfo.ContactId, AddressId = fullfranchiseeviewmodel.ContactInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.ContactInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.ContactInfoEmail.EmailId, FranchiseeId = FranchiseeId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }

        public ActionResult SaveEditFranchiseContactTemp(int FranchiseeId, int ContactId, string Name, int PhoneId, string Phone1, string PhoneExt, int EmailId, string EmailAddress, string Cell)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone1 = Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone = Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneExt = PhoneExt;
            fullfranchiseeviewmodel.ContactInfoPhone.Cell = Cell;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfo.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.ContactId = ContactId;
            fullfranchiseeviewmodel.ContactInfo.Name = Name;
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;

            fullfranchiseeviewmodel.ContactInfo = CustomerService.AddNewContactOldInactiveTemp(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.AddNewPhoneOldInactiveTemp(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.AddNewEmailOldInactiveTemp(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();

            return Json(new { ContactId = fullfranchiseeviewmodel.ContactInfo.ContactId, AddressId = fullfranchiseeviewmodel.ContactInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.ContactInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.ContactInfoEmail.EmailId, FranchiseeId = FranchiseeId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }

        public ActionResult SaveEditFranchiseDetailsPopupTab2Temp(int FranchiseeId, int ContactId, string Name, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode, int PhoneId, string Phone1, string PhoneExt, string Cell, int EmailId, string EmailAddress)
        {
            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();

            fullfranchiseeviewmodel.ContactInfoAddress.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;

            if (StateId != "" && StateId != "0")
            {
                fullfranchiseeviewmodel.ContactInfoAddress.StateListId = Convert.ToInt32(StateId);
            }
            //fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoAddress.AddressId = AddressId;
            fullfranchiseeviewmodel.ContactInfoAddress.Address1 = Address1;
            fullfranchiseeviewmodel.ContactInfoAddress.Address2 = Address2;
            fullfranchiseeviewmodel.ContactInfoAddress.City = City;
            fullfranchiseeviewmodel.ContactInfoAddress.PostalCode = PostalCode;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoAddress.IsActive = true;

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneId = PhoneId;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone = Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.PhoneExt = PhoneExt;
            fullfranchiseeviewmodel.ContactInfoPhone.Cell = Cell;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = -1;
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailId = EmailId;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailAddress = EmailAddress;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;

            fullfranchiseeviewmodel.ContactInfo.ClassId = FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.ContactId = ContactId;
            fullfranchiseeviewmodel.ContactInfo.Name = Name;
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = "-1";
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;

            fullfranchiseeviewmodel.ContactInfo = CustomerService.AddNewContactOldInactiveTemp(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();
            fullfranchiseeviewmodel.ContactInfoAddress = CustomerService.AddNewAddressOldInactiveTemp(fullfranchiseeviewmodel.ContactInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.AddNewPhoneOldInactiveTemp(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.AddNewEmailOldInactiveTemp(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();

            return Json(new { ContactId = fullfranchiseeviewmodel.ContactInfo.ContactId, AddressId = fullfranchiseeviewmodel.ContactInfoAddress.AddressId, PhoneId = fullfranchiseeviewmodel.ContactInfoPhone.PhoneId, EmailId = fullfranchiseeviewmodel.ContactInfoEmail.EmailId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = FranchiseeId });
        }
        public ActionResult SaveEditFranchiseDetailsPopupTab3(int Id)
        {
            return Json(new { Id = Id }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveEditFranchiseDetailsPopupTab4(int FranchiseeId, int BillSettingsId, int IdentificationId, int IdentifierTypeListId, string IdentifierNumer, bool Incorporated, string PayeeName, string Name1099, bool Print1099, int AddressId, string Address, string City, string StateId, string PostalCode, int ACHBankId = 0, string BankName = "", decimal Routing = 0, decimal Account = 0, string Description = "", string RemittanceNotes = "")
        {

            if (FranchiseeId > 0)
            {
                IdentificationViewModel IdentificationViewModel = new IdentificationViewModel();
                IdentificationViewModel.IdentificationId = IdentificationId;
                IdentificationViewModel.ClassId = FranchiseeId;
                IdentificationViewModel.TypeListId = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                IdentificationViewModel.ContactTypeListId = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
                IdentificationViewModel.IdentifierTypeListId = IdentifierTypeListId;
                IdentificationViewModel.IdentifierNumer = IdentifierNumer;
                IdentificationViewModel = CustomerService.SaveIdentification(IdentificationViewModel.ToModel<Identification, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification>();

                AddressViewModel AddressViewModel = new AddressViewModel();
                AddressViewModel.ClassId = BillSettingsId;
                AddressViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                AddressViewModel.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
                AddressViewModel.AddressId = AddressId;
                if (StateId != null && StateId != "")
                {
                    AddressViewModel.StateListId = Convert.ToInt32(StateId);
                }
                AddressViewModel.Address1 = Address;
                AddressViewModel.City = City;
                AddressViewModel.PostalCode = PostalCode;
                AddressViewModel.IsActive = true;
                AddressViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                AddressViewModel.CreatedDate = DateTime.Now;

                var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(AddressViewModel.FullAddress));
                if (_latlngBL.results.Count() > 0)
                {
                    AddressViewModel.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                    AddressViewModel.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
                }
                AddressViewModel = CustomerService.AddNewAddressOldInactive(AddressViewModel.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                FranchiseeBillSetting FranchiseeBillSettingModel = new FranchiseeBillSetting();
                FranchiseeBillSettingModel = FranchiseeService.GetFranchiseeBillSettingsById(BillSettingsId);
                FranchiseeBillSettingModel.Incorporated = Incorporated;
                FranchiseeBillSettingModel.PayeeName = PayeeName;
                FranchiseeBillSettingModel.C1099Name = Name1099;
                FranchiseeBillSettingModel.Print1099 = Print1099;
                FranchiseeBillSettingModel = FranchiseeService.SaveFranchiseeBillSettings(FranchiseeBillSettingModel);

                ACHBank ACHBankModel = new ACHBank();
                ACHBankModel.ACHBankId = ACHBankId;
                ACHBankModel.ClassId = FranchiseeId;
                ACHBankModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                ACHBankModel.Name = BankName;
                ACHBankModel.RoutingNumber = Routing;
                ACHBankModel.AccountNumber = Account;
                ACHBankModel.Descrption = Description;
                ACHBankModel.RemittanceNotes = RemittanceNotes;
                ACHBankModel = FranchiseeService.SaveACHBank(ACHBankModel);

                return Json(new { Id = FranchiseeId, IdentificationId = IdentificationViewModel.IdentificationId, AddressId = AddressViewModel.AddressId, BillSettingsId = FranchiseeBillSettingModel.FranchiseeBillSettingsId, ACHBankId = ACHBankModel.ACHBankId }, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        public ActionResult SaveEditFranchiseDetailsPopupTab4Temp(int FranchiseeId, int BillSettingsId, int IdentificationId, int IdentifierTypeListId, string IdentifierNumer, bool Incorporated, string PayeeName, string Name1099, bool Print1099, int AddressId, string Address, string City, string StateId, string PostalCode, int ACHBankId = 0, string BankName = "", decimal Routing = 0, decimal Account = 0, string Description = "", string RemittanceNotes = "")
        {

            if (FranchiseeId > 0)
            {
                IdentificationViewModel IdentificationViewModel = new IdentificationViewModel();
                IdentificationViewModel.IdentificationId = IdentificationId;
                IdentificationViewModel.ClassId = FranchiseeId;
                IdentificationViewModel.TypeListId = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                IdentificationViewModel.ContactTypeListId = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
                IdentificationViewModel.IdentifierTypeListId = IdentifierTypeListId;
                IdentificationViewModel.IdentifierNumer = IdentifierNumer;
                IdentificationViewModel = CustomerService.SaveIdentification_Temp(IdentificationViewModel.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

                AddressViewModel AddressViewModel = new AddressViewModel();
                AddressViewModel.ClassId = BillSettingsId;
                AddressViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                AddressViewModel.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
                AddressViewModel.AddressId = AddressId;
                if (StateId != null && StateId != "")
                {
                    AddressViewModel.StateListId = Convert.ToInt32(StateId);
                }
                AddressViewModel.Address1 = Address;
                AddressViewModel.City = City;
                AddressViewModel.PostalCode = PostalCode;
                AddressViewModel.IsActive = true;
                AddressViewModel.CreatedBy = -1;
                AddressViewModel.CreatedDate = DateTime.Now;
                AddressViewModel = CustomerService.AddNewAddressOldInactiveTemp(AddressViewModel.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();

                FranchiseeBillSettings_Temp FranchiseeBillSettingModel = new FranchiseeBillSettings_Temp();
                FranchiseeBillSettingModel = FranchiseeService.GetFranchiseeBillSettingsByIdTemp(BillSettingsId);
                FranchiseeBillSettingModel.Incorporated = Incorporated;
                FranchiseeBillSettingModel.PayeeName = PayeeName;
                FranchiseeBillSettingModel.C1099Name = Name1099;
                FranchiseeBillSettingModel.Print1099 = Print1099;
                FranchiseeBillSettingModel = FranchiseeService.SaveFranchiseeBillSettings_Temp(FranchiseeBillSettingModel);

                ACHBank_Temp ACHBankModel = new ACHBank_Temp();
                ACHBankModel.ACHBankId = ACHBankId;
                ACHBankModel.ClassId = FranchiseeId;
                ACHBankModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                ACHBankModel.Name = BankName;
                ACHBankModel.RoutingNumber = Routing;
                ACHBankModel.AccountNumber = Account;
                ACHBankModel.Descrption = Description;
                ACHBankModel.RemittanceNotes = RemittanceNotes;
                ACHBankModel = FranchiseeService.SaveACHBank_Temp(ACHBankModel);

                return Json(new { Id = FranchiseeId, IdentificationId = IdentificationViewModel.IdentificationId, AddressId = AddressViewModel.AddressId, BillSettingsId = FranchiseeBillSettingModel.FranchiseeBillSettingsId, ACHBankId = ACHBankModel.ACHBankId }, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveEditFranchiseDetailsPopupTab5(int BillSettingsId, bool Chargeback, bool BBPAdministrationFee, bool AccountRebate, bool GenerateReport)
        {
            if (BillSettingsId > 0)
            {
                FranchiseeBillSetting FranchiseeBillSettingModel = new FranchiseeBillSetting();

                FranchiseeBillSettingModel = FranchiseeService.GetFranchiseeBillSettingsById(BillSettingsId);
                FranchiseeBillSettingModel.Chargeback = Chargeback;
                FranchiseeBillSettingModel.BBPAdministrationFee = BBPAdministrationFee;
                FranchiseeBillSettingModel.AccountRebate = AccountRebate;
                FranchiseeBillSettingModel.GenerateReport = GenerateReport;

                FranchiseeBillSettingModel = FranchiseeService.SaveFranchiseeBillSettings(FranchiseeBillSettingModel);
            }
            return Json(new { Id = BillSettingsId }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveEditFranchiseDetailsPopupTab5Temp(int BillSettingsId, bool Chargeback, bool BBPAdministrationFee, bool AccountRebate, bool GenerateReport)
        {
            if (BillSettingsId > 0)
            {
                FranchiseeBillSettings_Temp FranchiseeBillSettingModel = new FranchiseeBillSettings_Temp();

                FranchiseeBillSettingModel = FranchiseeService.GetFranchiseeBillSettingsByIdTemp(BillSettingsId);
                FranchiseeBillSettingModel.Chargeback = Chargeback;
                FranchiseeBillSettingModel.BBPAdministrationFee = BBPAdministrationFee;
                FranchiseeBillSettingModel.AccountRebate = AccountRebate;
                FranchiseeBillSettingModel.GenerateReport = GenerateReport;

                FranchiseeBillSettingModel = FranchiseeService.SaveFranchiseeBillSettings_Temp(FranchiseeBillSettingModel);
            }
            return Json(new { Id = BillSettingsId }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEditOwnerFranchisePopup(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList(FranchiseeId, ownerId);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContact().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
                if (FranchiseeOwnerData != null)
                {
                    //FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeOwnerData.ClassId && one.AddressId == FranchiseeOwnerData.AddressId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address>();
                    //FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeOwnerData.ClassId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList && one.EmailId == FranchiseeOwnerData.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
                    //FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeOwnerData.ClassId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList && one.PhoneId == FranchiseeOwnerData.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
                    //FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentification().Where(one => one.ClassId == FranchiseeOwnerData.ClassId && one.TypeListId == OwnerTypeList && one.IdentificationId == FranchiseeOwnerData.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification>();
                }
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
        }

        //New
        [HttpGet]
        public ActionResult AddEditFranchiseOwnerPopup(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
            FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.GetFranchiseeOwnerListByOwnerId(ownerId);
            if (FranchiseeId > 0 && ownerId > 0)
            {
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult AddEditOwnerFranchiseTempPopup(string id, string OwnerId)
        {
            int FranchiseeId = Convert.ToInt32(-1);
            int ownerId = Convert.ToInt32(OwnerId);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList_Temp(ownerId);

            FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
            FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.GetFranchiseeOwnerListByOwnerId_Temp(ownerId);

            if (ownerId > 0)
            {
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult DeleteOwner(string OwnerId)
        {
            int FranchiseeOwnweId = Convert.ToInt32(OwnerId);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();

            var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList_Temp(FranchiseeOwnweId);

            //FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
            //FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.GetFranchiseeOwnerListByOwnerId(ownerId);

            if (FranchiseeOwnweId > 0)
            {
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeOwnweId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult DeleteOwnerTemp(string id, string OwnerId)
        {
            int FranchiseeId = Convert.ToInt32(-1);
            int ownerId = Convert.ToInt32(OwnerId);
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList_Temp(ownerId);

            FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
            FullFranchiseeViewModel.FranchiseOwnerList = FranchiseeService.GetFranchiseeOwnerListByOwnerId(ownerId);

            if (ownerId > 0)
            {
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopup.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult AddEditContactFranchiseTempPopup()
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            return View("~/Areas/Portal/Views/Franchise/AddEditFranchiseContactPopup.cshtml", FullFranchiseeViewModel);
        }


        [HttpGet]
        public ActionResult AddEditFranchiseContactPopup(string id, string contactId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ContactId = Convert.ToInt32(contactId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);
                //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
                FullFranchiseeViewModel.ContactInfo = CustomerService.GetContact().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();

                FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
                FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditFranchiseContactPopup.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditFranchiseContactPopup.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult AddEditFranchiseContactPopupTemp(string id, string contactId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ContactId = Convert.ToInt32(contactId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);
                //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, OwnerContactTypeList);
                FullFranchiseeViewModel.ContactInfo = CustomerService.GetContactTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();

                FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditFranchiseContactPopupTemp.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditFranchiseContactPopupTemp.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpGet]
        public ActionResult AddEditOwnerFranchisePopupTemp(string id, string OwnerId)
        {
            //ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            int ownerId = Convert.ToInt32(OwnerId);
            ViewBag.StateList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            if (FranchiseeId > 0 && ownerId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.GetContactTemp().Where(one => one.ContactId == ownerId && one.ClassId == FranchiseeId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList && one.ContactTypeListId == OwnerContactTypeList).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == ownerId && one.TypeListId == OwnerTypeList).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();

                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name", FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.StateListId);
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopupTemp.cshtml", FullFranchiseeViewModel);
            }
            else
            {
                FullFranchiseeViewModel.FranchiseeIdByOwner = FranchiseeId;
                //ViewBag.OwnerInfoAddress_State = new SelectList(States, "Id", "Name");
                return View("~/Areas/Portal/Views/Franchise/_AddEditOwnerFranchisePopupTemp.cshtml", FullFranchiseeViewModel);
            }
        }

        [HttpPost]
        public ActionResult AddEditOwnerTemp(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else if (FullFranchiseeViewModel.FranchiseeIdByOwner > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = FullFranchiseeViewModel.FranchiseeIdByOwner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = "-1";
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = -1;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        }

        [HttpPost]
        public ActionResult AddEditOwnerTempModel(FullFranchiseeViewModel FullFranchiseeViewModel)
        {
            if (FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId > 0 && FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId > 0)
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;
                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();


                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();
                int? classid = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                var FranchiseeOwnerData = FranchiseeService.GetFranchiseeOwnerList_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId);
                if (FranchiseeOwnerData == null)
                {
                    FranchiseeOwnerListModel franchiseeOwnerListModel = new FranchiseeOwnerListModel();
                    franchiseeOwnerListModel.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                    franchiseeOwnerListModel.EmailId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.EmailId;
                    franchiseeOwnerListModel.AddressId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.AddressId;
                    franchiseeOwnerListModel.PhoneId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.PhoneId;
                    franchiseeOwnerListModel.ContactId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                    franchiseeOwnerListModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    franchiseeOwnerListModel.CreatedDate = DateTime.Now;
                    franchiseeOwnerListModel = FranchiseeService.SaveFranchiseeOwnerList_Temp(franchiseeOwnerListModel.ToModel<FranchiseeOwnerList_Temp, FranchiseeOwnerListModel>()).ToModel<FranchiseeOwnerListModel, FranchiseeOwnerList_Temp>();
                }


                return Json(new { aaData = FullFranchiseeViewModel, success = true }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId = 0;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.TypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactTypeListId = OwnerContactTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo = CustomerService.SaveContact_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.CreatedDate = DateTime.Now;


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.CreatedDate = DateTime.Now;

                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.IsActive = true;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.CreatedDate = DateTime.Now;
                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }


                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress = CustomerService.SaveAddress_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone = CustomerService.SavePhone_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail = CustomerService.SaveEmail_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = OwnerTypeList;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.IdentifierTypeListId = (int)JKApi.Business.Enumeration.IdentifierTypeList.SSN;
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee; //---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Owner;//---
                FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification = CustomerService.SaveIdentification_Temp(FullFranchiseeViewModel.FranchiseeOwner.OwnerIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();


                FranchiseeOwnerListModel franchiseeOwnerListModel = new FranchiseeOwnerListModel();
                franchiseeOwnerListModel.ClassId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ClassId;
                franchiseeOwnerListModel.EmailId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoEmail.EmailId;
                franchiseeOwnerListModel.AddressId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoAddress.AddressId;
                franchiseeOwnerListModel.PhoneId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfoPhone.PhoneId;
                franchiseeOwnerListModel.ContactId = FullFranchiseeViewModel.FranchiseeOwner.OwnerInfo.ContactId;
                franchiseeOwnerListModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                franchiseeOwnerListModel.CreatedDate = DateTime.Now;
                franchiseeOwnerListModel = FranchiseeService.SaveFranchiseeOwnerList_Temp(franchiseeOwnerListModel.ToModel<FranchiseeOwnerList_Temp, FranchiseeOwnerListModel>()).ToModel<FranchiseeOwnerListModel, FranchiseeOwnerList_Temp>();

                return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = FullFranchiseeViewModel, id = FullFranchiseeViewModel.FranchiseeIdByOwner, success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::  Edit Franchise Plan Information :: 
        [HttpGet]
        public ActionResult RenderEditFranchiseFourStepPopup(int id)
        {

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.CurrentMenu = "FranchiseGeneral";
            ViewBag.StateList = getUsStatesList();
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                //BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                //BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContract().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract>();
            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();

            //FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration((int)FullFranchiseeViewModel.ContractInfo.FranchiseeId).ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            //.Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault()


            //var FranchiseeFeeConfig = FranchiseeService.GetFranchiseeFeeConfiguration().Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault();

            FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            //FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration().Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault().ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            var lsFranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration().Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true && (F.IsDelete == null || F.IsDelete == false)).ToList();
            ViewBag.lsFranchiseeFeeConfigurationInfo = lsFranchiseeFeeConfigurationInfo;
            ViewBag.lsFranchiseeFeeConfigurationInfoCount = lsFranchiseeFeeConfigurationInfo.Count();
            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection(id);
            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }

            return PartialView("_EditFranchiseFourStepPopup", FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult SaveFranchiseFourStepPopup(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            //if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            //{
            //    return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            //}
            //ViewBag.CurrentMenu = "FranchiseGeneral";

            //ViewBag.StateList = getUsStatesList();
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "General");
            //if (fullfranchiseeviewmodel.BusinessInfo.FranchiseeId < 1)
            //{
            //    BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
            //    // return View();
            //}
            //else if (fullfranchiseeviewmodel != null && fullfranchiseeviewmodel.BusinessInfo != null)
            //{
            //    BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, "Manage");
            //    // return View(serv.GetFranchiseForManage(id));
            //}
            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }

            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }

            if (collection["FranchiseeFeeConfigurationInfo.MinimumAmount"] != null && collection["FranchiseeFeeConfigurationInfo.MinimumAmount"] != "")
            {
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.MinimumAmount = Convert.ToDecimal(collection["FranchiseeFeeConfigurationInfo.MinimumAmount"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.RegionId = Convert.ToInt32(collection["FranchiseeFeeConfigurationInfo.RegionId"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.FeeId = Convert.ToInt32(collection["FranchiseeFeeConfigurationInfo.FeeId"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.TypeListId = 2;
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.IsDelete = false;

            }

            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract>();


            fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            //fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo = FranchiseeService.SaveFranchiseeFeeConfiguration(fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.ToModel<FeeConfiguration, FranchiseeFeeConfigurationViewModel>(), this.LoginUserId).ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();

            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (fullfranchiseeviewmodel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            //if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            //{
            //    return View(fullfranchiseeviewmodel);
            //}
            //else if (fullfranchiseeviewmodel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            //{
            //    return RedirectToAction("SearchList", "Franchise", new { area = "Portal", searchin = 7, status = 1 });
            //}
            //return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId });
            return Json(fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ::  Franchise Distribution :: 

        [HttpGet]
        public ActionResult RenderFranchiseDistributionDetailPopup(int id)
        {
            FranchiseeDistributionDetailsModel FranchiseeDistributionDetailsModel = new FranchiseeDistributionDetailsModel();
            if (id > 0)
            {
                FranchiseeDistributionDetailsModel = FranchiseeService.GetFranchiseeDistributionDetails(id);
            }

            return PartialView("_FranchiseDistributionDetailPopup", FranchiseeDistributionDetailsModel);
        }

        [HttpGet]
        public JsonResult CustomerIsViewDocumnetPOPUp(int id)
        {

            if (id > 0)
            {

                try
                {
                    var result = (from dst in jkEntityModel.Distributions
                                  join cnt in jkEntityModel.ContractDetails on dst.ContractDetailId equals cnt.ContractDetailId
                                  join ct in jkEntityModel.Customers on dst.CustomerId equals ct.CustomerId
                                  join st in jkEntityModel.ServiceTypeLists on cnt.ServiceTypeListId equals st.ServiceTypeListid
                                  join u in jkEntityModel.UploadDocuments on ct.CustomerId equals u.ClassId
                                  where dst.FranchiseeId == id && dst.isActive == true
                                  select new
                                  {
                                      CustomerName = ct.Name,
                                      CustomerNo = ct.CustomerNo,
                                      CustomerId = u.ClassId,
                                      Path = u.FilePath,
                                      FileName = u.FileName
                                  }).Distinct().ToList();


                    if (result != null && result.Count > 0)
                    {
                        return Json(new { success = "", dataItem = result.OrderBy(x => x.CustomerId).ToJSON() }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return Json(new { success = "no" }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult RenderFranchiseCustomerDistributionPopup(int cid, int fid, int contractDetailDistributionLineNo = -1, bool AddFinderFeeOnly = false)
        {

            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, fid, contractDetailDistributionLineNo);


            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");



            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();


            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var franchisestatuslst = CustomerService.GetStatusList().Where(st => st.TypeListId == 10).ToList();
            ViewBag.FranchiseStatusListModel = new SelectList(franchisestatuslst, "StatusListId", "Name", oCID.FindersFee != null ? oCID.FindersFee.StatusListId : 0);


            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");
            ViewBag.FinderFeeOnly = AddFinderFeeOnly;
            ViewBag.ContractDetailDistributionLineNo = contractDetailDistributionLineNo;
            ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();

            oCID.FindersFee.Description = "Finder Fees on Contract Billing";

            string CustomerContractStartDate = string.Empty;
            var CustomerContract = CustomerService.GetContractByCustomerId(cid);
            if (CustomerContract != null)
            {
                CustomerContractStartDate = (CustomerContract.StartDate != null ? Convert.ToDateTime(CustomerContract.StartDate).ToString("MM/dd/yyyy") : string.Empty);
            }
            ViewBag.CustomerContractStartDate = CustomerContractStartDate;

            return PartialView("_FranchiseeCustomers", oCID);
        }
        [HttpGet]
        public ActionResult RenderFranchiseCustomerDistributionPopupFindersFee(int cid, int fid, string callfrom = "")
        {
            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, fid, -1);

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");

            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var franchisestatuslst = CustomerService.GetStatusList().Where(st => st.TypeListId == 10).ToList();
            ViewBag.FranchiseStatusListModel = new SelectList(franchisestatuslst, "StatusListId", "Name", oCID.FindersFee != null ? oCID.FindersFee.StatusListId : 0);

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");

            //ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();
            oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";
            ViewBag.CallFrom = callfrom;
            ViewBag.SelectedFranchiseeId = fid;
            return PartialView("_CustomerFranchiseeDistributionFindersFee", oCID);
        }

        [HttpGet]
        public ActionResult RenderFranchiseFindersFeePP(int ffid, int cid, int fid, string callfrom = "")
        {
            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, fid, -1);

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");

            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var franchisestatuslst = CustomerService.GetStatusList().Where(st => st.TypeListId == 10).ToList();
            ViewBag.FranchiseStatusListModel = new SelectList(franchisestatuslst, "StatusListId", "Name", oCID.FindersFee != null ? oCID.FindersFee.StatusListId : 0);

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");

            //ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();
            oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";
            ViewBag.CallFrom = callfrom;
            ViewBag.SelectedFranchiseeId = fid;
            return PartialView("_FranchiseeFindersFee", oCID);
        }

        [HttpPost]
        public ActionResult RenderFranchiseFindersFeePP(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
        {
            var t = model;
            var frm = fromcollection;
            var DistributionFeeJSON = !String.IsNullOrEmpty(frm["CustomerDistributionDetail_FeeString"].ToString()) ? frm["CustomerDistributionDetail_FeeString"].ToString() : "";
            var CallFrom = !String.IsNullOrEmpty(frm["hdfCustomerDetailCallFrom"].ToString()) ? frm["hdfCustomerDetailCallFrom"].ToString() : "";
            var FranchiseeDistribution = !String.IsNullOrEmpty(frm["FranchiseeDistributionDetail_String"].ToString()) ? frm["FranchiseeDistributionDetail_String"].ToString() : "";
            List<FCFranchiseeDistributionViewModel> lstFranchiseeDistribution = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FCFranchiseeDistributionViewModel>>(FranchiseeDistribution);
            model.lstFranchiseeDistribution = lstFranchiseeDistribution;




            List<FranchiseeDistributionFeesViewModel> lstDistributionFee = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeDistributionFeesViewModel>>(DistributionFeeJSON);

            int FindersFeeCount = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;

            List<FCFranchiseeDistributionFeeViewModel> lstFranchiseeDistributionFee = new List<FCFranchiseeDistributionFeeViewModel>();
            FCFranchiseeDistributionFeeViewModel oFCFranchiseeDistributionFeeViewModel;
            if (lstDistributionFee != null)
                foreach (FranchiseeDistributionFeesViewModel o in lstDistributionFee)
                {
                    oFCFranchiseeDistributionFeeViewModel = new FCFranchiseeDistributionFeeViewModel();
                    oFCFranchiseeDistributionFeeViewModel.Amount = o.Amount;
                    oFCFranchiseeDistributionFeeViewModel.ContractDetailId = o.ContractDetailId;
                    oFCFranchiseeDistributionFeeViewModel.DetailLineNumber = o.DetailLineNumber;
                    oFCFranchiseeDistributionFeeViewModel.DistributionId = o.DistributionFeesId;
                    oFCFranchiseeDistributionFeeViewModel.FranchiseeId = o.FranchiseeId;
                    oFCFranchiseeDistributionFeeViewModel.FeeId = o.FeeId;
                    oFCFranchiseeDistributionFeeViewModel.FeeRateTypeListId = o.FeeRateTypeListId;
                    oFCFranchiseeDistributionFeeViewModel.FeeName = o.FeeName;
                    oFCFranchiseeDistributionFeeViewModel.MaintenanceTempDetailId = 0;
                    oFCFranchiseeDistributionFeeViewModel.MaintenanceTempId = 0;

                    lstFranchiseeDistributionFee.Add(oFCFranchiseeDistributionFeeViewModel);
                }
            model.lstFranchiseeDistributionFee = lstFranchiseeDistributionFee;

            if (model.FindersFee != null)
            {
                for (int i = 1; i <= FindersFeeCount; i++)
                {
                    if (frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                    {
                        int FFAdjustmentId = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                        if (FFAdjustmentId > 0)
                        {
                            string FFAdjustment_Description = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                            decimal FFAdjustment_Amount = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString().Replace("$", "").Replace(",", "").Trim()) : 0;

                            FCFindersFeeAdjustmentViewModel oFFAdjustment = new FCFindersFeeAdjustmentViewModel();
                            oFFAdjustment.FindersFeeAdjustmentId = 0;
                            oFFAdjustment.FindersFeeAdjustmentTypeListId = FFAdjustmentId;
                            oFFAdjustment.FindersFeeId = model.FindersFee.FindersFeeId;
                            oFFAdjustment.FranchiseeId = model.FindersFee.FranchiseeId;
                            oFFAdjustment.Description = FFAdjustment_Description;
                            oFFAdjustment.Amount = FFAdjustment_Amount;
                            if (model.lstFindersFeeAdjustment == null)
                                model.lstFindersFeeAdjustment = new List<FCFindersFeeAdjustmentViewModel>();

                            model.lstFindersFeeAdjustment.Add(oFFAdjustment);
                        }
                    }
                }
            }
            var retVal = FranchiseeService.InsertFranchiseeCustomerDistributionDetail(model, 1);
            if (CallFrom == "RO")
            {
                return RedirectToAction("Index", "CRMRegionOperation", new { area = "CRM", id = model.CustomerDetail.CustomerId });
            }
            else if (CallFrom == "AO")
            {
                return RedirectToAction("Index", "CRMRegionAccounting", new { area = "CRM", id = model.CustomerDetail.CustomerId });
            }
            else if (CallFrom == "FF")
            {
                return RedirectToAction("FinderFeeList", "Franchise", new { area = "Portal" });
            }
            else if (CallFrom == "NFF")
            {
                return RedirectToAction("NewFinderFee", "Franchise", new { area = "Portal" });
            }
            else
            {
                return RedirectToAction("MaintenanceStepFifth", "Customer", new { area = "Portal", id = model.CustomerDetail.CustomerId });
            }



        }

        [HttpGet]
        public ActionResult RenderFranchiseCustomerOnlyFranchiseeFeePopup(int cid, int fid, int contractDetailDistributionLineNo = -1, bool AddFinderFeeOnly = false, int DistributionId = 0)
        {
            contractDetailDistributionLineNo = contractDetailDistributionLineNo == 0 ? 1 : contractDetailDistributionLineNo;
            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, fid, contractDetailDistributionLineNo);

            var datamodel = CustomerService.GetFindersFeewithCustomerId(cid, DistributionId);

            oCID.FindersFee = (datamodel != null ? datamodel : new FCFindersFeeViewModel());


            if (String.IsNullOrEmpty(datamodel?.ContractBillingAmount.ToString()))
            {
                oCID.FindersFee.ContractBillingAmount = oCID.lstContractDetail.Sum(o => o.Amount);
            }


            if (oCID.FranchiseeDistribution == null)
            {
                oCID.FranchiseeDistribution = new FCFranchiseeDistributionViewModel();
                oCID.FranchiseeDistribution.DistributionId = oCID.lstContractDetail.FirstOrDefault().ContractDetailId;
                oCID.FranchiseeDistribution.DistributionId = DistributionId;
                oCID.FranchiseeDistribution.Amount = oCID.lstContractDetail.Sum(o => o.Amount);
            }

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");



            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();


            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");


            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");
            ViewBag.FinderFeeOnly = AddFinderFeeOnly;
            ViewBag.ContractDetailDistributionLineNo = contractDetailDistributionLineNo;
            ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();

            oCID.FindersFee.Description = "Finder Fees on Contract Billing";
            oCID.FindersFee.DistributionId = DistributionId;
            string CustomerContractStartDate = string.Empty;
            var CustomerContract = CustomerService.GetContractByCustomerId(cid);
            if (CustomerContract != null)
            {
                CustomerContractStartDate = (CustomerContract.StartDate != null ? Convert.ToDateTime(CustomerContract.StartDate).ToString("MM/dd/yyyy") : string.Empty);
            }
            ViewBag.CustomerContractStartDate = CustomerContractStartDate;
            ViewBag.CustomerId = cid;
            ViewBag.FFCallFrom = "";
            return PartialView("_RenderFranchiseCustomerOnlyFranchiseeFeePopup", oCID);
        }

        [HttpGet]
        public JsonResult GetFindersFeeScheduleList()
        {
            try
            {
                var response = FranchiseeService.GetFindersFeeScheduleListData();
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveFranchiseCustomerOnlyFranchiseeFeePopup(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
        {
            var t = model;
            var frm = fromcollection;
            var totaldistributions = frm["hdftotaldistributions"];
            var franid = frm["lstFindersFeeAdjustment[0].FranchiseeId"];
            var DistributionFeeJSON = !String.IsNullOrEmpty(frm["CustomerDistributionDetail_FeeString"].ToString()) ? frm["CustomerDistributionDetail_FeeString"].ToString() : "";
            //FranchiseeDistribution.StartDate

            List<FranchiseeDistributionFeesViewModel> lstDistributionFee = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeDistributionFeesViewModel>>(DistributionFeeJSON);

            int FindersFeeCount = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;

            List<FCFranchiseeDistributionFeeViewModel> lstFranchiseeDistributionFee = new List<FCFranchiseeDistributionFeeViewModel>();
            FCFranchiseeDistributionFeeViewModel oFCFranchiseeDistributionFeeViewModel;
            foreach (FranchiseeDistributionFeesViewModel o in lstDistributionFee)
            {
                oFCFranchiseeDistributionFeeViewModel = new FCFranchiseeDistributionFeeViewModel();
                oFCFranchiseeDistributionFeeViewModel.Amount = o.Amount;
                oFCFranchiseeDistributionFeeViewModel.ContractDetailId = o.ContractDetailId;
                oFCFranchiseeDistributionFeeViewModel.DetailLineNumber = o.DetailLineNumber;
                oFCFranchiseeDistributionFeeViewModel.DistributionId = o.DistributionFeesId;
                oFCFranchiseeDistributionFeeViewModel.FranchiseeId = o.FranchiseeId;
                oFCFranchiseeDistributionFeeViewModel.FeeId = o.FeeId;
                oFCFranchiseeDistributionFeeViewModel.FeeRateTypeListId = o.FeeRateTypeListId;
                oFCFranchiseeDistributionFeeViewModel.FeeName = o.FeeName;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempDetailId = 0;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempId = 0;

                lstFranchiseeDistributionFee.Add(oFCFranchiseeDistributionFeeViewModel);
            }
            model.lstFranchiseeDistributionFee = lstFranchiseeDistributionFee;

            if (model.FindersFee != null)
            {

                for (int i = 1; i <= FindersFeeCount; i++)
                {
                    if (frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                    {
                        int FFAdjustmentId = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                        if (FFAdjustmentId > 0)
                        {
                            string FFAdjustment_Description = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                            decimal FFAdjustment_Amount = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString().Replace("$", "").Replace(",", "").Trim()) : 0;

                            FCFindersFeeAdjustmentViewModel oFFAdjustment = new FCFindersFeeAdjustmentViewModel();
                            oFFAdjustment.FindersFeeAdjustmentId = 0;
                            oFFAdjustment.FindersFeeAdjustmentTypeListId = FFAdjustmentId;
                            oFFAdjustment.FindersFeeId = model.FindersFee.FindersFeeId;
                            oFFAdjustment.FranchiseeId = model.FindersFee.FranchiseeId;
                            oFFAdjustment.Description = FFAdjustment_Description;
                            oFFAdjustment.Amount = FFAdjustment_Amount;
                            if (model.lstFindersFeeAdjustment == null)
                                model.lstFindersFeeAdjustment = new List<FCFindersFeeAdjustmentViewModel>();

                            model.lstFindersFeeAdjustment.Add(oFFAdjustment);
                        }
                    }
                }

            }
            var retVal = FranchiseeService.SaveFindersFeeDetailsOnlyFF(model, 2);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FranchiseCustomerDistributionPopup(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
        {
            var t = model;
            var frm = fromcollection;
            var totaldistributions = frm["hdftotaldistributions"];
            var franid = frm["lstFindersFeeAdjustment[0].FranchiseeId"];
            var DistributionFeeJSON = !String.IsNullOrEmpty(frm["CustomerDistributionDetail_FeeString"].ToString()) ? frm["CustomerDistributionDetail_FeeString"].ToString() : "";

            List<FranchiseeDistributionFeesViewModel> lstDistributionFee = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeDistributionFeesViewModel>>(DistributionFeeJSON);

            int FindersFeeCount = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;

            List<FCFranchiseeDistributionFeeViewModel> lstFranchiseeDistributionFee = new List<FCFranchiseeDistributionFeeViewModel>();
            FCFranchiseeDistributionFeeViewModel oFCFranchiseeDistributionFeeViewModel;
            foreach (FranchiseeDistributionFeesViewModel o in lstDistributionFee)
            {
                oFCFranchiseeDistributionFeeViewModel = new FCFranchiseeDistributionFeeViewModel();
                oFCFranchiseeDistributionFeeViewModel.Amount = o.Amount;
                oFCFranchiseeDistributionFeeViewModel.ContractDetailId = o.ContractDetailId;
                oFCFranchiseeDistributionFeeViewModel.DetailLineNumber = o.DetailLineNumber;
                oFCFranchiseeDistributionFeeViewModel.DistributionId = o.DistributionFeesId;
                oFCFranchiseeDistributionFeeViewModel.FranchiseeId = o.FranchiseeId;
                oFCFranchiseeDistributionFeeViewModel.FeeId = o.FeeId;
                oFCFranchiseeDistributionFeeViewModel.FeeRateTypeListId = o.FeeRateTypeListId;
                oFCFranchiseeDistributionFeeViewModel.FeeName = o.FeeName;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempDetailId = 0;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempId = 0;

                lstFranchiseeDistributionFee.Add(oFCFranchiseeDistributionFeeViewModel);
            }
            model.lstFranchiseeDistributionFee = lstFranchiseeDistributionFee;

            if (model.FindersFee != null)
            {

                for (int i = 1; i <= FindersFeeCount; i++)
                {
                    if (frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                    {
                        int FFAdjustmentId = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                        if (FFAdjustmentId > 0)
                        {
                            string FFAdjustment_Description = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                            decimal FFAdjustment_Amount = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString().Replace("$", "").Replace(",", "").Trim()) : 0;

                            FCFindersFeeAdjustmentViewModel oFFAdjustment = new FCFindersFeeAdjustmentViewModel();
                            oFFAdjustment.FindersFeeAdjustmentId = 0;
                            oFFAdjustment.FindersFeeAdjustmentTypeListId = FFAdjustmentId;
                            oFFAdjustment.FindersFeeId = model.FindersFee.FindersFeeId;
                            oFFAdjustment.FranchiseeId = model.FindersFee.FranchiseeId;
                            oFFAdjustment.Description = FFAdjustment_Description;
                            oFFAdjustment.Amount = FFAdjustment_Amount;
                            if (model.lstFindersFeeAdjustment == null)
                                model.lstFindersFeeAdjustment = new List<FCFindersFeeAdjustmentViewModel>();

                            model.lstFindersFeeAdjustment.Add(oFFAdjustment);
                        }
                    }
                }

            }
            var retVal = FranchiseeService.InsertFranchiseeCustomerDistributionDetail(model, 2);



            if (Convert.ToInt32(totaldistributions) > 1)
            {
                return RedirectToAction("Index", "CRMRegionAccounting", new { area = "CRM", id = model.CustomerDetail.CustomerId });
            }
            else
            {
                return RedirectToAction("FranchiseeDetail", "Franchise", new { area = "Portal", id = model.CustomerDetail.FranchiseeId });
            }

        }
        #endregion

        #endregion

        [HttpGet]
        public ActionResult DetailFranchiseeDistribution(int id)
        {
            return PartialView("_DetailFranchiseeDistribution");
        }
        [HttpGet]
        public ActionResult PartialAddNewTransaction()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.FranchiseeManualTransaction == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            //ViewBag.FranchiseeTransactionTypeList = new SelectList(FranchiseeService.GetFranchiseeTransactionTypeList().OrderBy(x => x.Name), "FranchiseeTransactionTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.TRVGetVendorList(SelectedRegionId).OrderBy(x => x.Name), "Code", "Name");

            return PartialView("_AddNewTransaction", new FranchiseeTransactionViewModel());
        }
        [HttpPost]
        public ActionResult PartialAddNewTransaction(FranchiseeTransactionViewModel model, FormCollection collection)
        {
            model.IsActive = true;
            model.NumOfPayments = model.NumOfPayments != null ? model.NumOfPayments : 1;
            model.PaymentsBilled = model.PaymentsBilled != null ? model.PaymentsBilled : 0;
            model.StatusListId = 1;// model.StatusListId != null ? model.StatusListId : 1;
            model.GrossTotal = model.NumOfPayments * model.Total;
            model.MasterTrxTypeListId = model.MasterTrxTypeListId != null ? model.MasterTrxTypeListId : 14;
            model.StartDate = model.StartDate != null ? model.StartDate : DateTime.Now.Date;
            model.IsCredit = false;
            model.MasterTrxTypeListId = 14;
            FranchiseeService.CreateFranchiseeManualTrasactionSave(model);
            return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
        }

        [HttpGet]
        public ActionResult PartialFranchiseeCredit()
        {
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            //ViewBag.FranchiseeTransactionTypeList = new SelectList(FranchiseeService.GetFranchiseeTransactionTypeList(), "FranchiseeTransactionTypeListId", "Name");
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.IsCredit == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");
            return PartialView("_PartialFranchiseeCredit", new FranchiseeTransactionViewModel());
        }

        [HttpPost]
        public ActionResult PartialFranchiseeCredit(FranchiseeTransactionViewModel model, FormCollection collection)
        {
            model.IsActive = true;
            model.NumOfPayments = model.NumOfPayments != null ? model.NumOfPayments : 1;
            model.PaymentsBilled = model.PaymentsBilled != null ? model.PaymentsBilled : 0;
            model.StatusListId = 1;// model.StatusListId != null ? model.StatusListId : 1;
            model.GrossTotal = model.NumOfPayments * model.Total;
            model.MasterTrxTypeListId = 20;
            model.StartDate = model.StartDate != null ? model.StartDate : DateTime.Now.Date;
            model.IsCredit = true;

            if (!String.IsNullOrEmpty(collection["membership1"]))
            {
                if (collection["membership1"].ToString() == "3")
                {
                    model.MasterTrxTypeListId = 62;
                }
            }

            if (model.FranchsieeTrxTypeListId == 1)
            {
                model.FranchsieeTrxTypeListId = 1;
            }
            else
            {
                model.FranchsieeTrxTypeListId = 3;
            }
            FranchiseeService.CreateFranchiseeManualTrasactionSave(model);
            return RedirectToAction("Franchiseedetail", "Franchise", new { area = "Portal", id = model.FranchiseeId });
        }

        [HttpGet]
        public ActionResult FranchiseeMaintenance(int id = 0)
        {
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int MaintenanceTypeList = Convert.ToInt32(JKApi.Business.Enumeration.MaintenanceTypeList.FranchiseeStatusMaintenance);
            ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_StatusList().Where(x => x.TypeListId == TypeList && x.MaintenanceTypeListId == MaintenanceTypeList), "StatusListId", "Name");
            ViewBag.StatusReasonList = new SelectList(FranchiseeService.GetAll_StatusReasonList(TypeList), "StatusReasonListId", "Name");

            FranchiseeMaintenanceViewModel oFranchiseeMaintenanceViewModel;
            if (id > 0)
            {
                oFranchiseeMaintenanceViewModel = FranchiseeService.GetFranchiseeMaintenanceViewModelById(id);
            }
            else
            {
                oFranchiseeMaintenanceViewModel = new FranchiseeMaintenanceViewModel();
            }

            return PartialView("_FranchiseeMaintenance", oFranchiseeMaintenanceViewModel);
        }


        [HttpGet]
        public ActionResult AllTransactions()
        {
            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AllTransactions", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("AllTransactions", "Franchise", new { area = "Portal" }), "Transactions");
            BreadCrumb.Add(Url.Action("AllTransactions", "Franchise", new { area = "Portal" }), "All Transactions");
            var comboFranchise = FranchiseeService.GetMasterTrxTypeListForFranchise();
            var combodata = comboFranchise.Select(o => new SelectListItem
            {
                Value = o.MasterTrxTypeListId.ToString(),
                Text = o.Name
            });
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.FranchiseTransactionType = combodata;
            return View();
        }

        [HttpGet]
        public ActionResult GetRegionWiseFranchaise(string franchiseName, int? regionId)
        {
            var data = FranchiseeService.GetRegionWiseFranchaise(franchiseName, regionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFranchaiseDetailsbyId(int franchiseId)
        {
            var data = FranchiseeService.GetFranchiseeDetail(franchiseId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFranchaiseDetailbyId(int franchiseId)
        {
            var data = FranchiseeService.GetFranchiseeDetail(franchiseId);
            //TaxRateViewModel oTaxRate = FranchiseeService.GetTaxRateDetail(franchiseId, 2, 0);
            TaxRateViewModel oTaxRate = FranchiseeService.GetTaxRateDetailForFranchiseeSupply(SelectedRegionId);
            return Json(new { Franchisee = data, TaxRate = oTaxRate }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFranchaiseTaxRateById(int franchiseId)
        {
            var data = FranchiseeService.GetFranchiseeDetail(franchiseId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllFranchiseTransactions(string franchiseName, int? franchiseId, int? regionId, int? recordNumber, int? masterTrxTypeId, DateTime? spnStartDate, DateTime? spnEndDate, int month = 0, int year = 0)
        {
            var data = FranchiseeService.GetAllFranchiseTransactions(franchiseName, franchiseId, regionId, recordNumber, masterTrxTypeId, spnStartDate, spnEndDate, month, year);
            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllFranchiseTransactionsWithStatus(int? franchiseId, int? masterTrxTypeId, DateTime? spnStartDate, DateTime? spnEndDate, int month = 0, int year = 0)
        {
            return Json(new
            {
                StartingBalance = FranchiseeService.GetFranchiseeBalanceAsOfDate(franchiseId, spnStartDate),
                Transactions = FranchiseeService.GetAllFranchiseTransactions(null, franchiseId, SelectedRegionId, 100, masterTrxTypeId, spnStartDate, spnEndDate, month, year)
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetFranchiseeChargebacks(int franchiseId, DateTime? spnStartDate, DateTime? spnEndDate)
        {
            var data = FranchiseeService.GetFranchiseeChargebacksData(franchiseId, spnStartDate, spnEndDate);
            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MaintenancePending()
        {
            ViewBag.CurrentMenu = "MaintenancePending";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MaintenancePending", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("MaintenancePending", "Franchise", new { area = "Portal" }), "Pending Approval");
            BreadCrumb.Add(Url.Action("MaintenancePending", "Franchise", new { area = "Portal" }), "MaintenancePending");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;



            List<FranchiseePendingMaintenanceListViewModel> lstMaintenance = FranchiseeService.GetFranchiseePendingMaintenanceList();

            return View(lstMaintenance);
        }

        public ActionResult FranchiseeMaintenancePendingDetail(int id)
        {
            //int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_StatusList().Where(x => x.TypeListId == TypeList), "StatusListId", "Name");
            //ViewBag.StatusReasonList = new SelectList(FranchiseeService.GetAll_StatusReasonList(TypeList), "StatusReasonListId", "Name");

            //EditFranchiseeMaintenanceViewModel oEditModel;
            //oEditModel = FranchiseeService.GetFranchiseeMaintenanceTemp(id);
            //return PartialView("_EditFranchiseeMaintenance", oEditModel);

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_StatusList().Where(x => x.TypeListId == TypeList), "StatusListId", "Name");
            ViewBag.StatusReasonList = new SelectList(FranchiseeService.GetAll_StatusReasonList(TypeList), "StatusReasonListId", "Name");

            EditFranchiseeMaintenanceViewModel oFranchiseeMaintenanceViewModel;
            if (id > 0)
            {
                oFranchiseeMaintenanceViewModel = FranchiseeService.GetFranchiseeMaintenanceTemp(id);
            }
            else
            {
                oFranchiseeMaintenanceViewModel = new EditFranchiseeMaintenanceViewModel();
            }

            return PartialView("_EditFranchiseeMaintenance", oFranchiseeMaintenanceViewModel);
        }

        public ActionResult FranchiseeCustomerMaintenancePendingDetail(int id)
        {

            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionDataForEdit(id);

            ViewBag.FindersFeeAdjustmentTypeListRaw = CustomerService.GetFindersFeeAdjustmentTypeList();

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");



            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");


            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");
            return PartialView("_FranchiseeCustomerMaintenanceDetail", oCID);
        }
        [HttpPost]
        public ActionResult FranchiseeCustomerMaintenancePendingDetail(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
        {
            var t = model;
            var frm = fromcollection;
            var DistributionFeeJSON = !String.IsNullOrEmpty(frm["CustomerDistributionDetail_FeeString"].ToString()) ? frm["CustomerDistributionDetail_FeeString"].ToString() : "";

            List<FranchiseeDistributionFeesViewModel> lstDistributionFee = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeDistributionFeesViewModel>>(DistributionFeeJSON);


            int FindersFeeCount = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;

            List<FCFranchiseeDistributionFeeViewModel> lstFranchiseeDistributionFee = new List<FCFranchiseeDistributionFeeViewModel>();
            FCFranchiseeDistributionFeeViewModel oFCFranchiseeDistributionFeeViewModel;
            foreach (FranchiseeDistributionFeesViewModel o in lstDistributionFee)
            {
                oFCFranchiseeDistributionFeeViewModel = new FCFranchiseeDistributionFeeViewModel();
                oFCFranchiseeDistributionFeeViewModel.Amount = o.Amount;
                oFCFranchiseeDistributionFeeViewModel.ContractDetailId = o.ContractDetailId;
                oFCFranchiseeDistributionFeeViewModel.DetailLineNumber = o.DetailLineNumber;
                oFCFranchiseeDistributionFeeViewModel.DistributionId = o.DistributionFeesId;
                oFCFranchiseeDistributionFeeViewModel.FranchiseeId = o.FranchiseeId;
                oFCFranchiseeDistributionFeeViewModel.FeeId = o.FeeId;
                oFCFranchiseeDistributionFeeViewModel.FeeRateTypeListId = o.FeeRateTypeListId;
                oFCFranchiseeDistributionFeeViewModel.FeeName = o.FeeName;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempDetailId = 0;
                oFCFranchiseeDistributionFeeViewModel.MaintenanceTempId = 0;

                lstFranchiseeDistributionFee.Add(oFCFranchiseeDistributionFeeViewModel);
            }
            model.lstFranchiseeDistributionFee = lstFranchiseeDistributionFee;

            if (model.FindersFee != null)
            {

                for (int i = 1; i <= FindersFeeCount; i++)
                {
                    if (frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                    {
                        int FFAdjustmentId = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                        if (FFAdjustmentId > 0)
                        {
                            string FFAdjustment_Description = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                            decimal FFAdjustment_Amount = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(frm["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString().Replace("$", "").Replace(",", "").Trim()) : 0;

                            FCFindersFeeAdjustmentViewModel oFFAdjustment = new FCFindersFeeAdjustmentViewModel();
                            oFFAdjustment.FindersFeeAdjustmentId = 0;
                            oFFAdjustment.FindersFeeAdjustmentTypeListId = FFAdjustmentId;
                            oFFAdjustment.FindersFeeId = model.FindersFee.FindersFeeId;
                            oFFAdjustment.FranchiseeId = model.FindersFee.FranchiseeId;
                            oFFAdjustment.Description = FFAdjustment_Description;
                            oFFAdjustment.Amount = FFAdjustment_Amount;
                            if (model.lstFindersFeeAdjustment == null)
                                model.lstFindersFeeAdjustment = new List<FCFindersFeeAdjustmentViewModel>();

                            model.lstFindersFeeAdjustment.Add(oFFAdjustment);
                        }
                    }
                }

            }
            var retVal = FranchiseeService.UpdateFranchiseeCustomerDistributionDetail(model);
            return RedirectToAction("FranchiseeDetail", "Franchise", new { area = "Portal", id = model.CustomerDetail.FranchiseeId });
        }

        [HttpGet]
        public ActionResult MaintenancePendingApproved(string ids, bool isApprove)
        {
            FranchiseeService.GetFranchiseePendingMaintenanceApproved(ids, isApprove);
            return RedirectToAction("MaintenancePending", "Franchise", new { area = "portal" });
        }

        [HttpGet]
        public ActionResult FinderFeeBillDetail(string TrNo)
        {
            IEnumerable<FinderFeeBillDetailViewModel> oBillDetail = FranchiseeService.GetFinderFeeBillDetail(TrNo);
            if (oBillDetail.ToList().Count > 0)
            {
                oBillDetail.FirstOrDefault().Dateformat = oBillDetail.FirstOrDefault().FindersFeeBillDate.Value.ToString("MM/dd/yyyy");
                return PartialView("_PartialFinderFeeBillDetail", oBillDetail.FirstOrDefault());
            }
            else
                return PartialView("_PartialFinderFeeBillDetail", new FinderFeeBillDetailViewModel());
        }

        [HttpGet]
        public ActionResult FMTDetail(string TrNo)
        {
            IEnumerable<FMTDetailViewModel> oFMTDetail = FranchiseeService.GetFMTDetail(TrNo);
            if (oFMTDetail.ToList().Count > 0)
            {
                oFMTDetail.FirstOrDefault().Dateformat = oFMTDetail.FirstOrDefault().trxdate.Value.ToString("MM/dd/yyyy");
                if (oFMTDetail.FirstOrDefault().ReSell == true)
                    oFMTDetail.FirstOrDefault().reSellName = "Y";
                if (oFMTDetail.FirstOrDefault().ReSell == false)
                    oFMTDetail.FirstOrDefault().reSellName = "N";

                return PartialView("_PartialFranchiseeManualTransaction", oFMTDetail.FirstOrDefault());
            }
            else
                return PartialView("_PartialFranchiseeManualTransaction", new FMTDetailViewModel());
        }

        [HttpGet]
        public ActionResult FinderFeeDetailsPrint(string TrNo)
        {
            IEnumerable<FinderFeeBillDetailViewModel> oBillDetail = FranchiseeService.GetFinderFeeBillDetail(TrNo);
            if (oBillDetail.ToList().Count > 0)
            {
                String HtmlContent = String.Empty;
                oBillDetail.FirstOrDefault().Dateformat = oBillDetail.FirstOrDefault().FindersFeeBillDate.Value.ToString("MM/dd/yyyy");
                HtmlContent += RenderPartialViewToString("_PartialFinderFeeBillDetailPrint", oBillDetail.FirstOrDefault());

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetReportPDF(HtmlContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("");
        }

        [HttpGet]
        public ActionResult LeaseBillReportDetail(string TrNo)
        {
            IEnumerable<LeaseBillDetailViewModel> oLeaseDetail = FranchiseeService.GetLeaseBillReportDetail(TrNo);
            if (oLeaseDetail.ToList().Count > 0)
            {
                oLeaseDetail.FirstOrDefault().Dateformat = oLeaseDetail.FirstOrDefault().TransactionDate.Value.ToString("MM/dd/yyyy");
                return PartialView("_PartialLeaseBillDetail", oLeaseDetail.FirstOrDefault());
            }
            else
                return PartialView("_PartialLeaseBillDetail", new LeaseBillDetailViewModel());
        }

        [HttpGet]
        public JsonResult LeaseBillReportDetailPrint(string TrNo)
        {
            IEnumerable<LeaseBillDetailViewModel> oLeaseDetail = FranchiseeService.GetLeaseBillReportDetail(TrNo.Trim());
            if (oLeaseDetail.ToList().Count() > 0)
            {
                String HtmlContent = String.Empty;
                oLeaseDetail.FirstOrDefault().Dateformat = oLeaseDetail.FirstOrDefault().TransactionDate.Value.ToString("MM/dd/yyyy");
                var leasedetail = oLeaseDetail.FirstOrDefault();
                HtmlContent += RenderPartialViewToString("_LeaseReportDetailPdf", leasedetail);

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetReportPDF(HtmlContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            return Json("");
        }

        [HttpGet]
        public JsonResult CheckPeriod(int PeriodClsId)
        {
            var month = FranchiseeService.getCheckPeriod(PeriodClsId);
            if (month != null)
            {
                return Json(month, JsonRequestBehavior.AllowGet);
            }
            return Json("");
        }

        [HttpGet]
        public ActionResult ChargeBackDetailPopUp(string TrNo)
        {
            var data = FranchiseeService.ChargeBackDetailPopUp(TrNo.Trim());
            return PartialView("_PartialChargeBackDetail", data);
        }
        public JsonResult ChargeBackPrint(string transactionNumber)
        {
            if (transactionNumber != null)
            {
                string HTMLContent = string.Empty;

                var data = FranchiseeService.ChargeBackDetailPopUp(transactionNumber.Trim());
                HTMLContent += RenderPartialViewToString("_ChargeBackExportToPDF", data);

                var retPath = "/Upload/InvoiceFiles/" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public FileResult ChargeBackExportPDF(string transactionNumber)
        {
            if (transactionNumber != null)
            {
                string HTMLContent = string.Empty;

                var data = FranchiseeService.ChargeBackDetailPopUp(transactionNumber.Trim());
                HTMLContent += RenderPartialViewToString("_ChargeBackExportToPDF", data);

                return File(GetPDF(HTMLContent), "application/pdf", "_ChargeBackExportToPDF.pdf");
            }
            return null;
        }

        [HttpGet]
        public ActionResult FranchiseeRevenues(int isback = 0)
        {
            ViewBag.CurrentMenu = "FranchiseeRevenues";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeRevenues", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeRevenues", "Franchise", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("FranchiseeRevenues", "Franchise", new { area = "Portal" }), "Franchisee Revenues");
            ViewBag.isback = isback;
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            int pId = _commonService.GetPeriodList().SingleOrDefault(a => a.BillMonth == DateTime.Now.Month && a.BillYear == DateTime.Now.Year).PeriodId;

            ViewBag.PeriodList = new SelectList(_commonService.GetPeriodList().Where(k => k.PeriodId <= pId).OrderByDescending(p => p.PeriodId), "PeriodId", "Period", _commonService.GetPeriodList().LastOrDefault().PeriodId);
            return View();
        }

        [HttpGet]
        public ActionResult FranchiseeRevenuesResult(string regions = "0", int periodid = 197)
        {
            //return Json(FranchiseeService.GetFranchiseeRevenuesReportData(regions, periodid), JsonRequestBehavior.AllowGet);
            return PartialView("_PartialFranchiseeRevenueResult", FranchiseeService.GetFranchiseeRevenuesReportData(regions, periodid));
        }
        [HttpGet]
        public ActionResult FranchiseeDeductions()
        {
            ViewBag.CurrentMenu = "FranchiseeDeductions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeDeductions", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeDeductions", "Franchise", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("FranchiseeDeductions", "Franchise", new { area = "Portal" }), "Franchisee Deductions");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            int pId = _commonService.GetPeriodList().SingleOrDefault(a => a.BillMonth == DateTime.Now.Month && a.BillYear == DateTime.Now.Year).PeriodId;
            ViewBag.PeriodList = new SelectList(_commonService.GetPeriodList().Where(k => k.PeriodId <= pId).OrderByDescending(p => p.PeriodId), "PeriodId", "Period", _commonService.GetPeriodList().LastOrDefault().PeriodId);
            return View();
        }

        [HttpGet]
        public ActionResult FranchiseeDeductionsResult(string regions = "0", int periodid = 197)
        {
            DataTable dt = FranchiseeService.GetFranchiseeDeductionReportData((regions != "null" ? regions : "0"), periodid);
            return PartialView("_PartialFranchiseeDeductionsResult", dt);
        }




        #region DataTableCalls

        [HttpGet]
        public ActionResult GetFranchiseeOwnerById(string id)
        {
            ViewBag.StateList = getUsStatesList();
            int FranchiseeId = Convert.ToInt32(id);
            List<FranchiseOwnersList> FranchiseeOwner = new List<JKViewModels.Franchisee.FranchiseOwnersList>();
            if (FranchiseeId > 0)
            {
                int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner);
                //FranchiseeOwner = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, FranachiseeTypeList, OwnerContactTypeList);
                FranchiseeOwner = FranchiseeService.GetFranchiseeOwnerListById(FranchiseeId);
            }
            return View(FranchiseeOwner);
        }

        public string PhoneNoformat(string phone)
        {
            string phoneno = phone;
            if (phone.Length == 10)
            {
                phoneno = '(' + phone.Substring(0, 3) + ')' + ' ' + phone.Substring(3, 3) + '-' + phone.Substring(6, 4);
            }
            return phoneno;
        }

        public ActionResult FranchiseDataTable(string status, int? rId)
        {
            try
            {
                int FranchiseTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranchiseContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                //if (_claimView.GetCLAIM_ROLE_TYPE() == "Accounting-User")
                //{
                //    status = "36";
                //}
                var response = FranchiseeService.GetFranchiseeList(status, rId);

                var result = (from f in response
                              select new
                              {
                                  ID = f.FranchiseeId,
                                  Number = f.FranchiseeNo,
                                  Name = f.Name,
                                  Address = string.Concat(f.Address1, ", ", f.City, ", ", f.StateName, " ", f.PostalCode),
                                  f.Address1,
                                  DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
                                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  RegionName = (f.RegionName != null ? f.RegionName : ""),
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  IsTemp = f.IsTemp
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

        public ActionResult FranchiseList(string status, int? rId, int? isTemp)
        {
            try
            {
                int FranchiseTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranchiseContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                
                var response = FranchiseeService.GetFranchiseeList(status, rId);
                var result = (Object)null;

                if (isTemp == 1)
                {
                    result = (from f in response
                              select new
                              {
                                  ID = f.FranchiseeId,
                                  Number = f.FranchiseeNo,
                                  Name = f.Name,
                                  Address = string.Concat(f.Address1, ", ", f.City, ", ", f.StateName, " ", f.PostalCode),
                                  f.Address1,
                                  DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
                                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  RegionName = (f.RegionName != null ? f.RegionName : ""),
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  IsTemp = f.IsTemp
                              }).Where(x=> x.IsTemp == 1).ToList();
                }
                else
                {
                    result = (from f in response
                              select new
                              {
                                  ID = f.FranchiseeId,
                                  Number = f.FranchiseeNo,
                                  Name = f.Name,
                                  Address = string.Concat(f.Address1, ", ", f.City, ", ", f.StateName, " ", f.PostalCode),
                                  f.Address1,
                                  DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
                                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                  RegionName = (f.RegionName != null ? f.RegionName : ""),
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  IsTemp = f.IsTemp
                              }).Where(x => x.IsTemp == 0).ToList();
                }
               

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

        public JsonResult Franchiselist(int RegionId)
        {
            try
            {
                //int StatusId = Convert.ToInt32(status);
                int FranchiseTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int FranchiseContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var response = FranchiseeService.GetFranchiseeListData(RegionId);
                var result = from f in response
                             select new
                             {
                                 ID = f.FranchiseeId,
                                 Number = f.FranchiseeNo,
                                 Name = f.Name,
                                 Address = f.Address1,
                                 DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
                                 Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                                 RegionName = f.RegionName
                             };


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


        public string getAddress(AddressViewModel BusinessInfoAddress)
        {
            string Address = string.Empty;
            if (BusinessInfoAddress != null)
            {
                if (!string.IsNullOrEmpty(BusinessInfoAddress.Address1))
                {
                    Address += BusinessInfoAddress.Address1;
                }
                if (!string.IsNullOrEmpty(BusinessInfoAddress.City))
                {
                    Address += " " + BusinessInfoAddress.City + " ";
                }
                if (!string.IsNullOrEmpty(BusinessInfoAddress.PostalCode))
                {
                    Address += " " + BusinessInfoAddress.PostalCode;
                }
            }
            return Address;
        }

        #endregion

        #region :: Franchisee Pending/Approval List ::

        [HttpGet]
        public ActionResult PendingApprovalList(int? id)
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PendingApprovalList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("PendingApprovalList", "Franchise", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("PendingApprovalList", "Franchise", new { area = "Portal" }) + "?id=12", "List");
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);

            //int preSelect = int.Parse(_claimView.GetCLAIM_SELECTED_COMPANY_ID() ?? "0");
            var regionlist = _commonService.GetRegionList();

            //if (preSelect == 0)
            //{
            //    regionlist.Insert(0, new Region { Name = "All" });
            //}

            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            // var json = serv.GetFranchiseSearchList(searchin, searchvalue, int.Parse(status)).ToList();
            // ViewBag.FranchiseSearchList = json;
            return View();
        }

        #endregion

        #region :: Franchisee Update Document ::

        [HttpGet]
        public ActionResult FranchiseeUpdateDocument(int id = -1)
        {
            ViewBag.CurrentMenu = "FranchiseeGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");

            ViewBag.Id = id;
            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(2), "FileTypeListId", "Name");

            string Address1 = string.Empty;
            string Address2 = string.Empty;

            FranchiseeDetailViewModel FranchiseeViewModellist = new FranchiseeDetailViewModel();
            FranchiseeViewModellist = FranchiseeService.GetFranchiseeDetailTemp(id);
            if (FranchiseeViewModellist != null)
            {
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.Address1))
                {
                    Address1 = FranchiseeViewModellist.Address1;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.City))
                {
                    Address2 = FranchiseeViewModellist.City;
                }
                if (FranchiseeViewModellist.StateName != null)
                {
                    Address2 += " " + FranchiseeViewModellist.StateName;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.PostalCode))
                {
                    Address2 += " " + FranchiseeViewModellist.PostalCode;
                }
                ViewBag.FranchiseeNo = FranchiseeViewModellist.FranchiseeNo;
                ViewBag.FranchiseeName = FranchiseeViewModellist.Name;
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;

            }
            return View(CustomerService.GetCUploadDocument(id, 2));
        }

        [HttpPost]
        public ActionResult FranchiseUploadDocument(FormCollection collection, HttpPostedFileBase[] file)
        {
            try
            {
                if (collection["ButtonType"] == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back).ToString())
                {
                    return RedirectToAction("MaintenanceStepFour", "Franchise", new { area = "Portal", id = int.Parse(collection["hdfFranchiseId"].ToString()) });
                }

                String[] strFTL = collection["hdfFiletypeListIds"].ToString().Split(',');

                int len = strFTL.Length;
                for (int i = 0; i < len; i++)
                {
                    if (strFTL[i] != "")
                    {
                        int ftlID = int.Parse(strFTL[i]);

                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument")))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument"));

                        }
                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString())))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString()));

                        }
                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString())))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString()));
                        }
                        if (file[i] != null)
                        {
                            if (file[i].ContentLength > 0)
                            {
                                string _FileName = Path.GetFileName(file[i].FileName);

                                int _FileSize = Path.GetFileName(file[i].FileName).Length;
                                string _FileExt = Path.GetFileName(file[i].FileName).Split('.').Last();
                                string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                                string _path = Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString(), _SFileName);
                                file[i].SaveAs(_path);

                                String _FilePath = "~/Upload/FranchiseDocument/" + SelectedRegionId.ToString() + "/" + collection["hdfFranchiseId"].ToString() + "/" + _SFileName;

                                CustomerService.SaveUploadDocument(int.Parse(collection["hdfFranchiseId"].ToString()), 2, ftlID, _FilePath, _FileName, _FileExt, _FileSize);
                            }
                        }
                    }
                }
                ViewBag.Message = "File Uploaded Successfully!!";
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";

            }
            //return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal", @id = 12 });
            //return RedirectToAction("MaintenanceStepThree", "Franchise", new { area = "Portal", id = int.Parse(collection["hdfFranchiseId"].ToString()) });
            return RedirectToAction("MaintenanceLastStep", "Franchise", new { area = "Portal", id = int.Parse(collection["hdfFranchiseId"].ToString()) });
        }
        public ActionResult MaintenanceLastStep(int id = -1)
        {
            ViewBag.FranchiseeId = id;
            ViewBag.CurrentMenu = "FranchiseeGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");

            return View();
        }
        [HttpPost]
        public ActionResult MaintenanceLastStep(FormCollection frm)
        {

            var _FranchiseeId = frm["FranchiseeId"] != null ? int.Parse(frm["FranchiseeId"]) : 0;
            if (_FranchiseeId > 0)
            {
                //_commonService.CommonInsertNotification(2, "", true, _FranchiseeId, 2, null, null, null, LoginUserId);
            }
            return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal" });
        }
        #endregion

        #region :: Pending Approval List :: 

        [HttpGet]
        public ActionResult PendingApprovalListFranchiseDetailPopup(int id = -1)
        {
            var TransactionsTypeList = FranchiseeService.GetUsMasterTrxTypeList().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (id > 0)
                FranchiseeDetail = GetFranchiseeDetailData(id, true);

            List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
            MessageData = _commonService.GetDashboardPendingData(null).Where(r => r.CustomerID == id && (r.EntrySource == "FranchiseeLegal" || r.EntrySource == "Franchisee")).OrderBy(r => r.MessageDate).ToList<PendingDashboardDataModel>();
            FranchiseeDetail.MessageData = MessageData;
            FranchiseeDetail.USERID = int.Parse(_claimView.GetCLAIM_USERID());
            return PartialView("_PendingApprovalListFranchiseDetailPopup", FranchiseeDetail);
        }

        [HttpGet]
        public ActionResult PendingApprovalListFranchiseDetailPopupTemp(int id = -1)
        {
            var TransactionsTypeList = FranchiseeService.GetUsMasterTrxTypeList().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (id > 0)
                FranchiseeDetail = GetFranchiseeDetailDataTemp(id, true);

            List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
            MessageData = _commonService.GetDashboardPendingData(null).Where(r => r.CustomerID == id && (r.EntrySource == "FranchiseeLegal" || r.EntrySource == "Franchisee")).OrderBy(r => r.MessageDate).ToList<PendingDashboardDataModel>();
            FranchiseeDetail.MessageData = MessageData;
            FranchiseeDetail.USERID = int.Parse(_claimView.GetCLAIM_USERID());
            return PartialView("_PendingApprovalListFranchiseDetailPopupTemp", FranchiseeDetail);
        }

        #region :: Edit Franchise Information :: 

        [HttpGet]
        public ActionResult PendingApprovalListEditFranchiseDetailsPopup(int id)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);

            FullFranchiseeViewModel.BusinessInfo = FranchiseeService.GetFranchisee().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee>();
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            //FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            //FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContact().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact>();
            FullFranchiseeViewModel.ContactInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();

            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerById(FranchiseeId, OwnerTypeList, Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner));
            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListById(FranchiseeId);

            FullFranchiseeViewModel.ACHBankInfo = FranchiseeService.GetACHBank().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList).OrderByDescending(one => one.ACHBankId).FirstOrDefault().ToModel<ACHBankViewModel, ACHBank>();
            if (FullFranchiseeViewModel.ACHBankInfo == null)
            {
                FullFranchiseeViewModel.ACHBankInfo = new ACHBankViewModel();
                FullFranchiseeViewModel.ACHBankInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.ACHBankInfo.TypeListId = BusinessTypeList;
            }
            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            FullFranchiseeViewModel.PayeeIdentification = CustomerService.GetIdentification().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification>();
            if (FullFranchiseeViewModel.PayeeIdentification == null)
            {
                FullFranchiseeViewModel.PayeeIdentification = new IdentificationViewModel();
                FullFranchiseeViewModel.PayeeIdentification.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.PayeeIdentification.TypeListId = BusinessTypeList;
                FullFranchiseeViewModel.PayeeIdentification.ContactTypeListId = BusinessContactTypeList;
            }

            FullFranchiseeViewModel.PayeeInfo = FranchiseeService.GetFranchiseeBillSettings().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeBillSettingsId).FirstOrDefault().ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSetting>();
            if (FullFranchiseeViewModel.PayeeInfo == null)
            {
                FullFranchiseeViewModel.PayeeInfo = new FranchiseeBillSettingViewModel();
                FullFranchiseeViewModel.PayeeInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            }


            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FullFranchiseeViewModel.PayeeInfo.FranchiseeBillSettingsId;
            FullFranchiseeViewModel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            FullFranchiseeViewModel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            FullFranchiseeViewModel.PayeeInfoAddress = CustomerService.GetAddress().Where(one => one.ClassId == FullFranchiseeViewModel.PayeeInfoAddress.ClassId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();

            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            return PartialView("_PendingApprovalListEditFranchiseDetailsPopup", FullFranchiseeViewModel);
        }

        [HttpGet]
        public ActionResult PendingApprovalListEditFranchiseDetailsPopupTemp(int id)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);

            FullFranchiseeViewModel.BusinessInfo = FranchiseeService.GetFranchiseeTemp().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee_Temp>();
            FullFranchiseeViewModel.BusinessInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
            //FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmail().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email>();
            //FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhone().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone>();
            FullFranchiseeViewModel.ContactInfo = CustomerService.GetContactTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.ContactId).FirstOrDefault().ToModel<ContactViewModel, Contact_Temp>();
            FullFranchiseeViewModel.ContactInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();
            FullFranchiseeViewModel.ContactInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
            FullFranchiseeViewModel.ContactInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == ContactContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();

            FullFranchiseeViewModel.BusinessInfoEmail = CustomerService.GetEmailTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.EmailId).FirstOrDefault().ToModel<EmailViewModel, Email_Temp>();
            FullFranchiseeViewModel.BusinessInfoPhone = CustomerService.GetPhoneTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.PhoneId).FirstOrDefault().ToModel<PhoneViewModel, Phone_Temp>();
            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            //FullFranchiseeViewModel.FranchiseeOwners = FranchiseeService.GetFranchiseeOwnerByIdTemp(FranchiseeId, OwnerTypeList, Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Owner));


            FullFranchiseeViewModel.ACHBankInfo = FranchiseeService.GetACHBankTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList).OrderByDescending(one => one.ACHBankId).FirstOrDefault().ToModel<ACHBankViewModel, ACHBank_Temp>();
            if (FullFranchiseeViewModel.ACHBankInfo == null)
            {
                FullFranchiseeViewModel.ACHBankInfo = new ACHBankViewModel();
                FullFranchiseeViewModel.ACHBankInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.ACHBankInfo.TypeListId = BusinessTypeList;
            }
            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            FullFranchiseeViewModel.PayeeIdentification = CustomerService.GetIdentificationTemp().Where(one => one.ClassId == FranchiseeId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.IdentificationId).FirstOrDefault().ToModel<IdentificationViewModel, Identification_Temp>();
            if (FullFranchiseeViewModel.PayeeIdentification == null)
            {
                FullFranchiseeViewModel.PayeeIdentification = new IdentificationViewModel();
                FullFranchiseeViewModel.PayeeIdentification.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
                FullFranchiseeViewModel.PayeeIdentification.TypeListId = BusinessTypeList;
                FullFranchiseeViewModel.PayeeIdentification.ContactTypeListId = BusinessContactTypeList;
            }

            FullFranchiseeViewModel.PayeeInfo = FranchiseeService.GetFranchiseeBillSettingsTemp().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeBillSettingsId).FirstOrDefault().ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSettings_Temp>();
            if (FullFranchiseeViewModel.PayeeInfo == null)
            {
                FullFranchiseeViewModel.PayeeInfo = new FranchiseeBillSettingViewModel();
                FullFranchiseeViewModel.PayeeInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            }


            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FullFranchiseeViewModel.PayeeInfo.FranchiseeBillSettingsId;
            FullFranchiseeViewModel.PayeeInfoAddress.ClassId = FranchiseeId;
            FullFranchiseeViewModel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            FullFranchiseeViewModel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            FullFranchiseeViewModel.PayeeInfoAddress = CustomerService.GetAddressTemp().Where(one => one.ClassId == FullFranchiseeViewModel.PayeeInfoAddress.ClassId && one.TypeListId == BusinessTypeList && one.ContactTypeListId == PayeeContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address_Temp>();

            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListByIdTemp(FranchiseeId);
            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            return PartialView("_PendingApprovalListEditFranchiseDetailsPopupTemp", FullFranchiseeViewModel);
        }

        #endregion 

        #region :: Edit Franchise Plan Information :: 

        [HttpGet]
        public ActionResult RenderEditFranchisePlanInfoPopup(int id)
        {

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.StateList = getUsStatesList();

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContract().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract>();
            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();
            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection(id);
            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            return PartialView("_PendingApprovalListFranchisePlanInfoPopup", FullFranchiseeViewModel);
        }

        [HttpGet]
        public ActionResult RenderEditFranchisePlanInfoPopupTemp(int id)
        {

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            ViewBag.StateList = getUsStatesList();

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContractTemp().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract_Temp>();
            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();
            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection_Temp(id);
            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            return PartialView("_PendingApprovalListFranchisePlanInfoPopupTemp", FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult SavePendingApprovalListFranchisePlanInfoPopup(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {

            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();

            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }
            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract>();

            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();

            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (fullfranchiseeviewmodel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal", id = 12 });

        }

        [HttpPost]
        public ActionResult SavePendingApprovalListFranchisePlanInfoPopupTemp(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {

            ViewBag.CurrentMenu = "FranchiseGeneral";

            ViewBag.StateList = getUsStatesList();

            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }
            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract_Temp(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract_Temp, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract_Temp>();

            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();

            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (fullfranchiseeviewmodel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }
            return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal", id = 12 });

        }

        #endregion

        #region :: Edit Franchise Upload Documents :: 

        [HttpGet]
        public ActionResult PendingApprovalListFranchiseeUploadDocumentPopup(int id = -1)
        {
            ViewBag.CurrentMenu = "FranchiseeGeneral";
            ViewBag.Id = id;
            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(2), "FileTypeListId", "Name");

            string Address1 = string.Empty;
            string Address2 = string.Empty;

            FranchiseeDetailViewModel FranchiseeViewModellist = new FranchiseeDetailViewModel();
            FranchiseeViewModellist = FranchiseeService.GetFranchiseeDetail(id);
            if (FranchiseeViewModellist != null)
            {
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.Address1))
                {
                    Address1 = FranchiseeViewModellist.Address1;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.City))
                {
                    Address2 = FranchiseeViewModellist.City;
                }
                if (FranchiseeViewModellist.StateName != null)
                {
                    Address2 += " " + FranchiseeViewModellist.StateName;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.PostalCode))
                {
                    Address2 += " " + FranchiseeViewModellist.PostalCode;
                }
                ViewBag.FranchiseeNo = FranchiseeViewModellist.FranchiseeNo;
                ViewBag.FranchiseeName = FranchiseeViewModellist.Name;
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;

            }
            return PartialView("_PendingApprovalListFranchiseeUploadDocPopup", CustomerService.GetCUploadDocument(id, 2));
        }

        [HttpGet]
        public ActionResult PendingApprovalListFranchiseeUploadDocumentPopupTemp(int id = -1)
        {
            ViewBag.CurrentMenu = "FranchiseeGeneral";
            ViewBag.Id = id;
            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(2), "FileTypeListId", "Name");

            string Address1 = string.Empty;
            string Address2 = string.Empty;

            FranchiseeDetailViewModel FranchiseeViewModellist = new FranchiseeDetailViewModel();
            FranchiseeViewModellist = FranchiseeService.GetFranchiseeDetailTemp(id);
            if (FranchiseeViewModellist != null)
            {
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.Address1))
                {
                    Address1 = FranchiseeViewModellist.Address1;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.City))
                {
                    Address2 = FranchiseeViewModellist.City;
                }
                if (FranchiseeViewModellist.StateName != null)
                {
                    Address2 += " " + FranchiseeViewModellist.StateName;
                }
                if (!string.IsNullOrEmpty(FranchiseeViewModellist.PostalCode))
                {
                    Address2 += " " + FranchiseeViewModellist.PostalCode;
                }
                ViewBag.FranchiseeNo = FranchiseeViewModellist.FranchiseeNo;
                ViewBag.FranchiseeName = FranchiseeViewModellist.Name;
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;

            }
            return PartialView("_PendingApprovalListFranchiseeUploadDocPopup", CustomerService.GetCUploadDocument(id, 2));
        }

        [HttpPost]
        public ActionResult SavePendingApprovalListFranchiseUploadDocument(FormCollection collection, HttpPostedFileBase[] file)
        {
            try
            {
                if (collection["ButtonType"] == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back).ToString())
                {
                    return RedirectToAction("MaintenanceStepFour", "Franchise", new { area = "Portal", id = int.Parse(collection["hdfFranchiseId"].ToString()) });
                }

                String[] strFTL = collection["hdfFiletypeListIds"].ToString().Split(',');

                int len = strFTL.Length;
                for (int i = 0; i < len; i++)
                {
                    if (strFTL[i] != "")
                    {
                        int ftlID = int.Parse(strFTL[i]);

                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument")))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument"));

                        }
                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString())))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString()));

                        }
                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString())))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString()));
                        }
                        if (file[i] != null)
                        {
                            if (file[i].ContentLength > 0)
                            {
                                string _FileName = Path.GetFileName(file[i].FileName);

                                int _FileSize = Path.GetFileName(file[i].FileName).Length;
                                string _FileExt = Path.GetFileName(file[i].FileName).Split('.').Last();
                                string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                                string _path = Path.Combine(Server.MapPath("~/Upload"), "FranchiseDocument", SelectedRegionId.ToString(), collection["hdfFranchiseId"].ToString(), _SFileName);
                                file[i].SaveAs(_path);

                                String _FilePath = "~/Upload/FranchiseDocument/" + SelectedRegionId.ToString() + "/" + collection["hdfFranchiseId"].ToString() + "/" + _SFileName;

                                CustomerService.SaveUploadDocument(int.Parse(collection["hdfFranchiseId"].ToString()), 2, ftlID, _FilePath, _FileName, _FileExt, _FileSize);
                            }
                        }
                    }
                }
                ViewBag.Message = "File Uploaded Successfully!!";
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";

            }
            return RedirectToAction("PendingApprovalList", "Franchise", new { area = "Portal", id = 12 });
        }


        #endregion

        public ActionResult UpdateApproveReject(int FranchiseeId, string Note, int Status)
        {
            if (FranchiseeId > 0)
            {
                FranchiseeService.UpdateApproveReject(FranchiseeId, Note, Status, "");
                //FranchiseeService.savePendingMessage(Note, FranchiseeId, Status);
                _commonService.CommonInsertNotification(4, "", (Status == 19 ? false : true), FranchiseeId, 2, null, null, null, LoginUserId);
            }
            return Json(new { Data = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateFranchiseeTempStatus(int FranchiseeId, string Note, int Status)
        {
            if (FranchiseeId > 0)
            {
                FranchiseeService.UpdateFranchiseeTempStatus(FranchiseeId, Note, Status);
                //FranchiseeService.savePendingMessage(Note, FranchiseeId, Status);
                //_commonService.CommonInsertNotification(4, "", (Status == 19 ? false : true), FranchiseeId, 2, null, null, null, LoginUserId);
                if (Status == (int)JKApi.Business.Enumeration.FranchiseeStatusList.Active)
                {
                    // Move Temp franchisee data to Real franchisee data
                    FranchiseeId = FranchiseeService.MoveFranchiseeTempDataToRealTable(FranchiseeId);

                    //Perfix Update
                    var data = FranchiseeService.GetFranchiseeById(FranchiseeId);
                    if (data.ParentId == 0)
                    {
                        data.ParentId = FranchiseeId;
                        data.Perfix = 000;
                        FranchiseeService.UpdateFranchiseePrefix(data);
                    }
                    else if (data != null)
                    {
                        if (data.ParentId != data.FranchiseeId)
                        {
                            if (data.ParentId.HasValue)
                            {
                                int frId = (int)(data.ParentId);
                                List<Franchisee> listFranchisee = FranchiseeService.GetFranchiseeByParentId(frId);
                                int _perfix = 000;
                                int StatusCount = 1;
                                int listcount = listFranchisee.Count();
                                int? _franchiseeTypeListId = 0;
                                int? _dlr = 0;
                                foreach (var item in listFranchisee)
                                {
                                    if(_franchiseeTypeListId == 0)
                                    {
                                        _franchiseeTypeListId = item.FranchiseeTypeListId;
                                        _dlr = item.dlr_id;
                                    }
                                    if (listcount == StatusCount)
                                    {
                                        item.IsActive = true;
                                    }
                                    else
                                    {
                                        item.IsActive = false;
                                    }
                                    item.FranchiseeTypeListId = _franchiseeTypeListId;
                                    item.dlr_id = _dlr;
                                    item.Perfix = _perfix;
                                    FranchiseeService.UpdateFranchiseePrefix(item);

                                    _perfix += 001;
                                    StatusCount += 1;
                                }
                            }
                        }
                    }
                }

            }
            return Json(new { Data = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DashboardRejectedActionClick(int FranchiseeID)
        {
            TempData["FranchiseeID"] = FranchiseeID;
            bool flag = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.NotificationMessageForDashboards.Where(r => r.CustomerID == FranchiseeID && (r.EntrySource == "FranchiseeLegal" || r.EntrySource == "FranchiseeLegal")).FirstOrDefault();
                if (data != null)
                {
                    if (_claimView.GetCLAIM_USERID() != data.UID.ToString())
                    {
                        flag = false;
                    }
                }
            }
            TempData["flag"] = flag;
            return RedirectToAction("PendingApprovalList");
        }
        #endregion

        #region  :: Service Call Log :: 

        [HttpGet]
        public ActionResult FranchiseServiceCallLogPopup(int? id)
        {
            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var CustomerList = FranchiseeService.GetFranchiseeCustomerList(Convert.ToInt32(id));
            ViewBag.CustomerList = AddFirstItemInSelecetList(new SelectList(CustomerList, "CustomerId", "Name"), "Select Customer");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            return PartialView("_FranchiseeServiceCallLogPopup", new JKViewModels.Customer.ServiceCallLogModel());
        }

        public ActionResult SaveFranchiseeServiceCallLogDetails(int CustomerId, string InitiatedById, string AreaId, string TypeId, string SpokeWith, string Action, string Comments, string StatusId, string Callback, string FollowBy, string EmailCallNotes)
        {
            ServiceCallLog ServiceCallLog = new ServiceCallLog();
            ServiceCallLog.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            ServiceCallLog.RegionId = SelectedRegionId;
            ServiceCallLog.ClassId = CustomerId;
            ServiceCallLog.InitiatedById = Convert.ToInt32(InitiatedById);
            ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(AreaId);
            ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(TypeId);
            ServiceCallLog.SpokeWith = SpokeWith;
            ServiceCallLog.Action = Action;
            ServiceCallLog.Comments = Comments;
            ServiceCallLog.StatusResultListId = Convert.ToInt32(StatusId);

            if (Callback != "")
            {
                DateTime dtCallBack = DateTime.ParseExact(Callback, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                ServiceCallLog.CallBack = dtCallBack;
            }
            ServiceCallLog.CallDate = DateTime.Now;
            ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;
            CustomerService.SaveServiceCallLog(ServiceCallLog);

            return Json(new { aaData = "", id = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        private SelectList AddFirstItemInSelecetList(SelectList list, string optionalText)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = optionalText });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", list.SelectedValue);
        }
        public JsonResult GetCustomerDetails(int CustomerId)
        {
            var Data = FranchiseeService.GetCustomerDetails(CustomerId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region :: Collections Call Log  :: 

        public ActionResult FranchiseCollectionsCallLogPopup(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            //Franchises
            ViewBag.Franchisees = CustomerService.getFrancisesByCustomerId((id ?? 0));
            //var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            //ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var CustomerList = FranchiseeService.GetFranchiseeCustomerList(Convert.ToInt32(id));
            ViewBag.CustomerList = AddFirstItemInSelecetList(new SelectList(CustomerList, "CustomerId", "Name"), "Select Customer");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            return PartialView("_FranchiseeCollectionsCallLogPopup");
        }

        [HttpPost]
        public ActionResult SaveCollectionCallLogDetails(CollectionsCallLogModel objCollectionsCallLog)
        {
            if (objCollectionsCallLog.strCallBack != null)
            {
                DateTime dtCallBack = DateTime.ParseExact(objCollectionsCallLog.strCallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                objCollectionsCallLog.CallBack = dtCallBack;
            }
            objCollectionsCallLog.RegionId = SelectedRegionId;
            objCollectionsCallLog.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            objCollectionsCallLog.CallDate = DateTime.Now;
            objCollectionsCallLog.Internal = (objCollectionsCallLog.boolInternal ? 1 : 0);
            objCollectionsCallLog.CallTime = DateTime.Now.ToString("HH:mm:ss");
            var ret = CustomerService.SaveCollectionCallLog(objCollectionsCallLog);
            return Json(new { aaData = "", id = objCollectionsCallLog.ClassId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helpers

        public IEnumerable<SelectListItem> getUsStatesList()
        {
            //Dictionary<string, string> dataDict = CustomerService.GetStateList();



            var data = CustomerService.GetStateList();

            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.ToList().Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.StateListId.ToString(), y.abbr.ToString());
                }
            }

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();

        }

        public List<KeyValuePair<string, int>> GetEnumValuesAndDescriptions<T>()
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

        protected void setGrid()
        {


            /*
            string searchby = Request["lstsearchtype"] == null || Request["lstsearchtype"].ToString() == "" ? "4" : Request["lstsearchtype"].ToString();
            string searchvalue = Request["txtsearchterm"] == null || Request["txtsearchterm"].ToString() == "" ? " " : Request["txtsearchterm"].ToString();
            status = Request["lststatus"] == null || Request["lststatus"].ToString() == "" ? AppCore.Empty.ToString() : Request["lststatus"].ToString();

            listdatasource.ConnectionString = ApplicationCore.AppCore.GetConnectionString(Request.Cookies[jSecurity.SESSION_STATE].Values.Get(jSecurity.REGION_DATABASE).ToString());
            listdatasource.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
            listdatasource.SelectCommand = "spsearch_F_Information";
            while (listdatasource.SelectParameters.Count > 0)
            {
                listdatasource.SelectParameters.RemoveAt(listdatasource.SelectParameters.Count - 1);
            }

            HiddenField hf = (HiddenField)Page.FindControl("lstsearchtype");
            if (hf != null)
            {
                hf.Value = hf.Value == "" ? searchby : hf.Value;

                HiddenField hf1 = (HiddenField)Page.FindControl("txtsearchterm");
                hf1.Value = hf1.Value == "" ? searchvalue : hf1.Value;

                HiddenField hf2 = (HiddenField)Page.FindControl("lststatus");
                hf2.Value = hf2.Value == "" ? status : hf2.Value;
            }

            listdatasource.SelectParameters.Add("searchType", DbType.Int32, searchby);
            listdatasource.SelectParameters.Add("searchTerm", DbType.String, searchvalue);
            listdatasource.SelectParameters.Add("status", DbType.Int32, status);
            */
        }

        public string GetFranchiseeNo()
        {
            string franchiseeno = FranchiseeService.getfranchiseeno();
            return franchiseeno;
        }

        public void UpdateFranchiseeIndex()
        {
            RegionConfiguration regData = FranchiseeService.GetRegionConfigurationbyId(4);
            int valueadd = Convert.ToInt32(regData.value) + 1;
            regData.value = valueadd.ToString();

            FranchiseeService.SaveRegionConfiguration(regData);
        }

        [HttpPost]
        public JsonResult FranchiseeAutoComplete(string namePrefix)
        {
            if (namePrefix == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            var Parentname = FranchiseeService.GetFranchisee()
                .Where(x => (x.RegionId == this.SelectedRegionId || this.SelectedRegionId == 0) &&
                    x.Name.Contains(namePrefix)).Select(x => new { x.Name, x.FranchiseeId, x.RegionId }).ToList();

            return this.Json(Parentname, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Franchiseeaddress(int? franchiseeId)
        {
            if (franchiseeId == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var address = FranchiseeService.GetAddress((int)franchiseeId).Select(x => new { x.Address1, x.Address2, x.City, x.StateName, x.PostalCode, x.AddressId }).FirstOrDefault();
            return this.Json(address, JsonRequestBehavior.AllowGet);
        }



        public FileResult FFDetailListExportPDF(int id)
        {
            string HTMLContent = string.Empty;
            FranchiseeService.GetFinderFeeDetailList(id, SelectedRegionId.ToString(), "");
            HTMLContent += RenderPartialViewToString("_FinderFeeDetailPDF", FranchiseeService.GetFranchiseeFinderFeeDetailList(SelectedRegionId, id));
            return File(GetPDF(HTMLContent), "application/pdf", "FF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
        }

        public JsonResult FFDetailListPrint(int id)
        {
            string HTMLContent = string.Empty;

            HTMLContent += RenderPartialViewToString("_FinderFeeDetailPDF", FranchiseeService.GetFranchiseeFinderFeeDetailList(SelectedRegionId, id));
            var retFileName = "FF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
            var retPath = "/Upload/InvoiceFiles/" + retFileName;
            var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), retFileName);
            System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO
            return Json(retPath, JsonRequestBehavior.AllowGet);
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

        public byte[] GetReportPDF(string pHTML)
        {
            #region -- styles --

            StyleSheet styles = new StyleSheet();

            styles.LoadStyle("tabborder", "border", ".01");

            styles.LoadStyle("t1col1", "width", "60");
            styles.LoadStyle("t1col2", "width", "120");
            styles.LoadStyle("t1col3", "width", "20");
            styles.LoadStyle("t1col4", "width", "90");

            styles.LoadStyle("t3col1", "width", "20");
            styles.LoadStyle("t3col2", "width", "140");
            styles.LoadStyle("t3col3", "width", "50");

            styles.LoadStyle("col1", "width", "35");
            styles.LoadStyle("col2", "width", "43");
            styles.LoadStyle("col3", "width", "43");
            styles.LoadStyle("collease3", "width", "63");
            styles.LoadStyle("col4", "width", "128");
            styles.LoadStyle("col5", "width", "35");
            styles.LoadStyle("col6", "width", "37");

            styles.LoadStyle("t22col1", "width", "35");

            styles.LoadStyle("t2col1", "width", "30");
            styles.LoadStyle("t2col2", "width", "145");
            styles.LoadStyle("t2col3", "width", "33");
            styles.LoadStyle("t2col4", "width", "33");
            styles.LoadStyle("t2col5", "width", "35");
            styles.LoadStyle("t2col6", "width", "45");


            #endregion -- styles --

            byte[] bPDF = null;
            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            HTMLWorker htmlWorker = new HTMLWorker(doc);
            htmlWorker.SetStyleSheet(styles);
            doc.Open();

            //var pages = HTMLWorker.ParseToList(txtReader, styles);
            //foreach (var page in pages)
            //{
            //    if (page is PdfPTable)
            //    {
            //        (page as PdfPTable).SplitLate = false;
            //    }
            //    doc.Add(page as IElement);
            //}
            htmlWorker.StartDocument();
            htmlWorker.Parse(txtReader);
            htmlWorker.EndDocument();

            htmlWorker.Close();
            doc.Close();
            bPDF = ms.ToArray();
            return bPDF;
        }
        public byte[] GetPDF(string pHTML) //With Rotate
        {


            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25))
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


            //#region -- styles --

            //StyleSheet styles = new StyleSheet();

            //styles.LoadStyle("tabborder", "border", ".01");

            //styles.LoadStyle("t1col1", "width", "60");
            //styles.LoadStyle("t1col2", "width", "120");
            //styles.LoadStyle("t1col3", "width", "20");
            //styles.LoadStyle("t1col4", "width", "90");

            //styles.LoadStyle("t3col1", "width", "20");
            //styles.LoadStyle("t3col2", "width", "140");
            //styles.LoadStyle("t3col3", "width", "50");

            //styles.LoadStyle("col1", "width", "35");
            //styles.LoadStyle("col2", "width", "43");
            //styles.LoadStyle("col3", "width", "43");
            //styles.LoadStyle("col4", "width", "128");
            //styles.LoadStyle("col5", "width", "35");
            //styles.LoadStyle("col6", "width", "37");

            //styles.LoadStyle("t2col1", "width", "35");
            //styles.LoadStyle("t2col2", "width", "148");
            //styles.LoadStyle("t2col3", "width", "33");
            //styles.LoadStyle("t2col4", "width", "33");
            //styles.LoadStyle("t2col5", "width", "35");
            //styles.LoadStyle("t2col6", "width", "37");

            //#endregion -- styles --

            //byte[] bPDF = null;
            //MemoryStream ms = new MemoryStream();
            //TextReader txtReader = new StringReader(pHTML);
            //Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);
            //PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            //HTMLWorker htmlWorker = new HTMLWorker(doc);
            //htmlWorker.SetStyleSheet(styles);
            //doc.Open();

            //Tag.GetHtmlTagProcessorFactory()
            //htmlWorker.StartDocument();
            //htmlWorker.Parse(txtReader);
            //htmlWorker.EndDocument();

            //htmlWorker.Close();
            //doc.Close();
            //bPDF = ms.ToArray();
            //return bPDF;
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
        #endregion

        #region :: Franchisee Legal Compliance Pending List ::

        [HttpGet]
        public ActionResult LegalCompliancePendingList(int? id)
        {
            ViewBag.CurrentMenu = "FranchiseGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("LegalCompliancePendingList", "Franchise", new { area = "Portal" }) + "?id=37", "List");
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.StatusList = new SelectList(statuslist, "StatusListId", "Name", 9);

            //int preSelect = int.Parse(_claimView.GetCLAIM_SELECTED_COMPANY_ID() ?? "0");
            var regionlist = _commonService.GetRegionList();

            //if (preSelect == 0)
            //{
            //    regionlist.Insert(0, new Region { Name = "All" });
            //}

            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            // var json = serv.GetFranchiseSearchList(searchin, searchvalue, int.Parse(status)).ToList();
            // ViewBag.FranchiseSearchList = json;
            return View();
        }

        [HttpGet]
        public ActionResult LegalCompliancePendingDetailPopup(int id = -1)
        {
            var TransactionsTypeList = FranchiseeService.GetUsMasterTrxTypeList().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }
            FranchiseeDetailViewModel FranchiseeDetail = new FranchiseeDetailViewModel();
            if (id > 0)
                FranchiseeDetail = GetFranchiseeDetailData(id, true);

            var ValidationItem = CustomerService.ValidationItemListStatus(37, (int)JKApi.Business.Enumeration.TypeList.Franchisee);
            ViewBag.ValidationItem = ValidationItem;

            return PartialView("_LegalCompliancePendingDetailDetailPopup", FranchiseeDetail);
        }
        public ActionResult UpdateLegalCompliancePending(int FranchiseeId, string Note, string BackgroundCheckDate, string BusinessProtectionDate, string BusinessLicenseDate, string EquipmentAndSupplyDate, string TrainingDate, string valIds)
        {
            if (FranchiseeId > 0)
            {
                // Update Fullfillment
                FranchiseeFullfillment FranchiseeFullfillmentModel = new FranchiseeFullfillment();
                var CheckExit = FranchiseeService.GetFullfillmentWithFranchisee(FranchiseeId);
                if (CheckExit > 1)
                {
                    FranchiseeFullfillmentModel.FranchiseeFullfillmentId = CheckExit;
                }
                FranchiseeFullfillmentModel.FranchiseeId = FranchiseeId;
                FranchiseeFullfillmentModel.BackgroundCheckDate = Convert.ToDateTime(BackgroundCheckDate);
                FranchiseeFullfillmentModel.BusinessProtectionDate = Convert.ToDateTime(BusinessProtectionDate);
                FranchiseeFullfillmentModel.BusinessLicensedate = Convert.ToDateTime(BusinessLicenseDate);
                FranchiseeFullfillmentModel.EquipmentAndSupplyDate = Convert.ToDateTime(EquipmentAndSupplyDate);
                FranchiseeFullfillmentModel.TrainingDate = Convert.ToDateTime(TrainingDate);
                FranchiseeService.SaveFranchiseeFullfillment(FranchiseeFullfillmentModel);
                FranchiseeService.savePendingMessageForLegal(Note, FranchiseeId, 4);
                //Update Status
                FranchiseeService.UpdateApproveReject(FranchiseeId, Note, 12, valIds);

                _commonService.CommonInsertNotification(3, "", true, FranchiseeId, 2, null, null, null, LoginUserId);
            }
            return Json(new { Data = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FranchiseeLegalCompliancePopup(int id = -1)
        {
            FranchiseeFullfillmentViewModel FullfillmentModel = new FranchiseeFullfillmentViewModel();
            string LegalComplianceNote = string.Empty;
            if (id > 0)
            {
                FullfillmentModel = FranchiseeService.GetFranchiseeFullfillment().Where(one => one.FranchiseeId == id).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeFullfillmentViewModel, FranchiseeFullfillment>();
                var LegalCompliance = FranchiseeService.GetLegalComplianceStatusByFranchiseeid(id, 12);
                if (LegalCompliance != null)
                {
                    LegalComplianceNote = LegalCompliance.StatusNotes;
                }
            }
            ViewBag.LegalComplianceNote = LegalComplianceNote;

            return PartialView("_FranchiseeLegalCompliancePopup", FullfillmentModel);
        }
        public ActionResult FranchiseeLegalCompliancePopupTemp(int id = -1)
        {
            FranchiseeFullfillmentViewModel FullfillmentModel = new FranchiseeFullfillmentViewModel();
            int LegalComplianceStatuId = 0;
            string LegalComplianceNote = string.Empty;
            if (id > 0)
            {
                FullfillmentModel = FranchiseeService.GetFranchiseeFullfillmentTemp().Where(one => one.FranchiseeId == id).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeFullfillmentViewModel, FranchiseeFullfillment_Temp>();
                var LegalCompliance = FranchiseeService.GetLegalComplianceStatusByFranchiseeid(id, 37);
                if (LegalCompliance != null)
                {
                    LegalComplianceNote = LegalCompliance.StatusNotes;
                    LegalComplianceStatuId = LegalCompliance.StatusId;
                }
            }
            ViewBag.FranchiseeId = id;
            ViewBag.LegalComplianceNote = LegalComplianceNote;
            ViewBag.LegalComplianceStatuId = LegalComplianceStatuId;

            return PartialView("_FranchiseeLegalCompliancePopup", FullfillmentModel);
        }
        [HttpPost]
        public ActionResult UpdateLegalComplianceDetail(FranchiseeFullfillmentViewModel model, string LegalComplianceNote)
        {
            FranchiseeService.SaveFranchiseeFullfillment(model.ToModel<FranchiseeFullfillment, FranchiseeFullfillmentViewModel>());
            return Json(new { Data = "1", success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateLegalComplianceDetailTemp(FranchiseeFullfillmentViewModel model, int LegalComplianceStatuId, string LegalComplianceNote)
        {
            FranchiseeService.SaveFranchiseeFullfillmentTemp(model.ToModel<FranchiseeFullfillment_Temp, FranchiseeFullfillmentViewModel>());
            FranchiseeService.UpdateLegalComplianceNote(LegalComplianceStatuId, LegalComplianceNote);
            return Json(new { Data = "1", success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region :: Franchisee Deductions/Revenues Export PDF :: 

        public FileResult FranchiseeDeductionsExportPDFfile(string regions = "0", int periodid = 197)
        {
            if (periodid != 0)
            {
                //Period
                var Period = _commonService.GetPeriodList();
                if (Period != null)
                {
                    var Periodmodel = Period.Where(w => w.PeriodId == periodid);
                    if (Periodmodel != null)
                    {
                        int iMonthNo = Convert.ToInt32(Periodmodel.FirstOrDefault().BillMonth);
                        DateTime dtDate = new DateTime(2000, iMonthNo, 1);
                        string sMonthFullName = dtDate.ToString("MMMM");
                        ViewBag.PeriodTtitle = sMonthFullName + ", " + Periodmodel.FirstOrDefault().BillYear;
                    }
                }
                string HTMLContent = string.Empty;
                DataTable dt = FranchiseeService.GetFranchiseeDeductionReportData((regions != "null" ? regions : "0"), periodid);
                HTMLContent += RenderPartialViewToString("_PartialFranchiseeDeductionsExportPDFResult", dt);

                return File(GetPDF(HTMLContent), "application/pdf", "_FranchiseeDeductionsExportToPDF.pdf");
            }
            return null;
        }

        public FileResult FranchiseeRevenuesResultExportPDFfile(string regions = "0", int periodid = 197)
        {
            if (periodid != 0)
            {
                //Period
                var Period = _commonService.GetPeriodList();
                if (Period != null)
                {
                    var Periodmodel = Period.Where(w => w.PeriodId == periodid);
                    if (Periodmodel != null)
                    {
                        int iMonthNo = Convert.ToInt32(Periodmodel.FirstOrDefault().BillMonth);
                        DateTime dtDate = new DateTime(2000, iMonthNo, 1);
                        string sMonthFullName = dtDate.ToString("MMMM");
                        ViewBag.PeriodTtitle = sMonthFullName + ", " + Periodmodel.FirstOrDefault().BillYear;
                    }
                }
                string HTMLContent = string.Empty;
                var model = FranchiseeService.GetFranchiseeRevenuesReportData(regions, periodid);

                HTMLContent += RenderPartialViewToString("_PartialFranchiseeRevenueExportPDFResult", model);

                return File(GetPDF(HTMLContent), "application/pdf", "_FranchiseeRevenueExportToPDF.pdf");
            }
            return null;
        }
        //Lease Bill Report PDF
        public FileResult FranchiseeLeaseBillReportResultExportPDFfile(string searchtext, string startDate, string endDate, string RegionIds)
        {
            if (RegionIds != "")
            {
                string HTMLContent = string.Empty;
                var model = serv.FranchiseeLeaseReportListData(RegionIds, searchtext, startDate, endDate);
                HTMLContent += RenderPartialViewToString("_PartialFranchiseeLeaseBillReportExportPDFResult", model);
                return File(GetPDF(HTMLContent), "application/pdf", "_FranchiseeLeaseBillReportExportToPDF.pdf");
            }
            return null;
        }

        //Finder Fee Report PDF
        public FileResult FranchiseeFinderFeeReportResultExportPDFfile(string searchtext, string startDate, string endDate, string RegionIds)
        {
            if (RegionIds != "")
            {
                string HTMLContent = string.Empty;
                var model = serv.FranchiseeFinderFeeReportListData(RegionIds, startDate, endDate);
                HTMLContent += RenderPartialViewToString("_PartialFinderFeeReportExportPDFResult", model);
                return File(GetPDF(HTMLContent), "application/pdf", "_FranchiseeLeaseBillReportExportToPDF.pdf");
            }
            return null;
        }

        #endregion

        #region ::Check Company Name Exits ::
        public ActionResult CheckFranchiseenameExits(string FranchiseeName, string Phone)
        {
            int FranchiseeId = FranchiseeService.CheckOnlyFranchiseeNamePhoneIsExist(FranchiseeName, Phone);
            return Json(new { FranchiseeId = FranchiseeId, success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFranchiseeValidationPopup()
        {
            return PartialView("_NewFranchiseeDetailVerify");
        }
        #endregion

        #region :: Account Offering :: 

        public ActionResult AccountOfferingPopup(int CustomerId)
        {
            string Address1 = string.Empty;
            string Address2 = string.Empty;
            FullCustomerViewModel CustomerViewModel = CustomerService.GetCustomerDetailsById(CustomerId);

            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City + ",";
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName + (StateName != "" ? "," : string.Empty);
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.CustomerNo = CustomerViewModel.CustomerViewModel.CustomerNo;
                ViewBag.CustomerName = CustomerViewModel.CustomerViewModel.Name;
                ViewBag.Address1 = Address1;
                ViewBag.Address2 = Address2;

                string ServiceType = string.Empty;
                if (CustomerViewModel.Contract != null)
                {
                    ViewBag.ContractAmount = (CustomerViewModel.Contract.Amount > 0 ? CustomerViewModel.Contract.Amount : 0);
                    ViewBag.InitialBusiness = (CustomerViewModel.Contract.InitialCleanAmount > 0 ? CustomerViewModel.Contract.InitialCleanAmount : 0);

                    var ServiceTypeList = CustomerService.GetServiceTypeList();
                    if (CustomerViewModel.ContractDetail != null)
                    {
                        ServiceType = ServiceTypeList.Where(w => w.ServiceTypeListid == CustomerViewModel.ContractDetail.ServiceTypeListId).FirstOrDefault().name;
                    }
                }
                ViewBag.ServiceType = ServiceType;
            }

            var DeclineReasonList = FranchiseeService.GetDeclineReasonListList().ToList();
            ViewBag.DeclineReasonList = new SelectList(DeclineReasonList, "DeclineReasonListId", "Name", "");

            return PartialView("_AccountOfferingPopup");
        }

        public ActionResult SaveFranchiseeOffering(int OfferingId, int FranchiseeId, int CustomerId, string DateOffered, string AmountOffered, int Accepted = 0, int Decline = 0, string strDate = "", string DeclineReason = "", string Description = "", string InitialBusinessAmount = "0")
        {
            Offering Offeringmodel = new Offering();
            Offeringmodel.OfferingId = OfferingId;
            Offeringmodel.FranchiseeId = FranchiseeId;
            Offeringmodel.CustomerId = CustomerId;
            Offeringmodel.RegionId = SelectedRegionId;
            Offeringmodel.OfferedDate = Convert.ToDateTime(DateOffered);
            Offeringmodel.ContractBillingAmount = Convert.ToDecimal(AmountOffered);
            Offeringmodel.InitialBusiness = ((InitialBusinessAmount == "" || InitialBusinessAmount == "0") ? false : true);
            Offeringmodel.InitialBusinessAmount = Convert.ToDecimal(InitialBusinessAmount != "" ? Convert.ToDecimal(InitialBusinessAmount) : 0);
            Offeringmodel.CreatedBy = -1;
            Offeringmodel.CreatedDate = DateTime.Now;
            if (Accepted == 1)
            {
                if (strDate != "")
                {
                    Offeringmodel.AcceptedDate = Convert.ToDateTime(strDate);
                }
                Offeringmodel.AcceptedNote = Description;
            }
            else
            {
                if (strDate != "")
                {
                    Offeringmodel.DeclinedDate = Convert.ToDateTime(strDate);
                }
                Offeringmodel.DeclineReasonListId = Convert.ToInt32(DeclineReason);
                Offeringmodel.DeclineReasonNote = Description;
            }
            Offeringmodel.StatusListId = 1;
            FranchiseeService.SaveFranchiseeOfferingData(Offeringmodel);

            return Json(new { Data = "1", success = true }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult SaveFranchiseeOfferingInfo(int OfferingId,int FranchiseeId, int CustomerId,string DateOffered,string AmountOffered,int Accepted= 0, int Decline = 0, string strDate = "")
        //{
        //    return Json(new { Data = "1", success = true }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult AccountOfferingListPopup(int Id)
        {
            ViewBag.FranchiseeId = Id;
            if (Id > 0)
            {
                var FranchiseeDetails = FranchiseeService.GetFranchiseeById(Id);
                int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
                int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);

                if (FranchiseeDetails != null)
                {
                    ViewBag.FranchiseeNo = FranchiseeDetails.FranchiseeNo;
                    ViewBag.FranchiseeName = FranchiseeDetails.Name;

                    var Address = CustomerService.GetAddress().Where(one => one.ClassId == Id && one.TypeListId == BusinessTypeList && one.ContactTypeListId == BusinessContactTypeList).OrderByDescending(one => one.AddressId).FirstOrDefault().ToModel<AddressViewModel, Address>();
                    if (Address != null)
                    {
                        string Address1 = "";
                        string Address2 = "";

                        if (!string.IsNullOrEmpty(Address.Address1))
                        {
                            Address1 = Address.Address1;
                        }
                        if (!string.IsNullOrEmpty(Address.City))
                        {
                            Address2 = Address.City + ",";
                        }
                        if (Address.StateListId != null && Address.StateListId > 0)
                        {
                            var States = CustomerService.GetStateList();
                            string StateName = States.Where(one => one.StateListId == Address.StateListId).Select(one => one.abbr).FirstOrDefault();
                            Address2 += " " + StateName + (StateName != "" ? "," : string.Empty);
                        }
                        if (!string.IsNullOrEmpty(Address.PostalCode))
                        {
                            Address2 += " " + Address.PostalCode;
                        }
                        ViewBag.Address1 = Address1;
                        ViewBag.Address2 = Address2;
                    }
                }
            }
            return PartialView("_AccountOfferingListPopup");
        }

        public JsonResult OfferingListData(int Id)
        {
            var result = CustomerService.GetAccountOfferingDataWithFranchiseeId(Id);

            var jsonResult = Json(new { aadata = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        // Bank Statement List Report Print
        [HttpGet]
        public ActionResult BankStatementListReportPrint(string regionIds, DateTime from, DateTime to)
        {
            var response = _companyService.GetBankStatementDetailList(regionIds, from, to);//.OrderBy(o=>o.TransactionDate).ThenBy(t=>t.AmountTypeListId).ThenBy(g => g.Total);

            List<JKApi.Service.Service.Administration.Company.BBankStatmentViewModel> lstBBankStatment = new List<JKApi.Service.Service.Administration.Company.BBankStatmentViewModel>();
            JKApi.Service.Service.Administration.Company.BBankStatmentViewModel oBBankStatment = new JKApi.Service.Service.Administration.Company.BBankStatmentViewModel();
            decimal _balanceAMT = 0;

            foreach (var item in response)
            {
                oBBankStatment = new JKApi.Service.Service.Administration.Company.BBankStatmentViewModel();
                oBBankStatment.TransactionDate = item.TransactionDate;
                oBBankStatment.TrxType = item.TrxType;
                oBBankStatment.Name = item.Name;
                oBBankStatment.ReferenceNumber = item.ReferenceNumber;
                oBBankStatment.Notes = item.Notes;
                oBBankStatment.AmountTypeListId = item.AmountTypeListId;

                if (item.AmountTypeListId == 1)
                {
                    oBBankStatment.Debit = item.Total;
                    _balanceAMT = _balanceAMT + (decimal)item.Total;
                }
                else
                {
                    oBBankStatment.Credit = item.Total;
                    _balanceAMT = _balanceAMT - (decimal)item.Total;
                }

                oBBankStatment.Balance = _balanceAMT;//(item.AmountTypeListId == 1 ? Total + f.Total : Total - f.Total),// != null ? String.Format("{0:C}", f.Total) : "$0.00",
                oBBankStatment.PayeeNo = item.PayeeNo;
                oBBankStatment.Code = item.code;
                lstBBankStatment.Add(oBBankStatment);

            }

            if (lstBBankStatment != null && lstBBankStatment.Count() > 0)
            {
                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("~/Areas/Portal/Views/Company/_BankStatementReportPrint.cshtml", lstBBankStatment);

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HTMLContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        //Obligation Report Print
        [HttpGet]
        public ActionResult ObligationReportPrint(string regionId = "")
        {
            List<JKViewModels.Management.ObligationReportViewModel> ObligationList = ManagementService.getObligationList(regionId.ToString());

            ViewBag.RemitTo = CustomerService.GetRemitToForRegion(SelectedRegionId);

            if (ObligationList != null && ObligationList.Count() > 0)
            {
                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("~/Areas/Portal/Views/Management/_ObligationReportPrint.cshtml", ObligationList);

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HTMLContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult GetFranchiseeContractTypeList(int Id)
        {
            List<FranchiseeContractTypeList> ListResult = new List<FranchiseeContractTypeList>();
            var DataResult = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (DataResult != null)
            {
                ListResult = DataResult.Where(w => w.FranchiseeContractTypeListId == Id).ToList();
            }
            return Json(ListResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FranchiseeManualTransactionsList()
        {

            ViewBag.CurrentMenu = "FranchiseTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeManualTransactionsList", "Franchise", new { area = "Portal" }), "Franchise");
            BreadCrumb.Add(Url.Action("FranchiseeManualTransactionsList", "Franchise", new { area = "Portal" }), "Transaction");
            BreadCrumb.Add(Url.Action("FranchiseeManualTransactionsList", "Franchise", new { area = "Portal" }), "FRAN Manual TRX List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        public ActionResult SaveFranCallsDetails(int FranchiseeId, string InitiatedBy, int StatusResultListId, string SpokeWith, string CallAction, string CallBack, string CallBackTime, string Comments, int CustomerId, int IsCallBack = 0)
        {
            if (FranchiseeId > 0)
            {
                FranchiseeService.SaveFranCallsDetails(FranchiseeId, InitiatedBy, StatusResultListId, SpokeWith, CallAction, CallBack, CallBackTime, Comments, CustomerId, IsCallBack);
            }
            return Json(FranchiseeId, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFeeConfigurationData(int FeeConfigurationInfoId = 0, int ClassId = 0, decimal MinimumAmoun = 0, string EffectiveDate = "", string strJsonList = "")
        {
            List<FranchiseeFeeConfigurationCustomModel> lstFranchiseeFeeConfiguration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeFeeConfigurationCustomModel>>(strJsonList);
            if (lstFranchiseeFeeConfiguration != null && lstFranchiseeFeeConfiguration.Count() > 0)
            {
                foreach (var item in lstFranchiseeFeeConfiguration)
                {
                    FranchiseeService.SaveFranchiseeFeeConfigurationData(item.FeeConfigurationId, ClassId, Convert.ToDecimal(item.MinimumAmount), item.EffectiveDate);
                }
            }
            int Id = 0;
            //Id = FranchiseeService.SaveFranchiseeFeeConfigurationData(FeeConfigurationInfoId, ClassId, MinimumAmoun, EffectiveDate);
            return Json(Id, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFeeConfigurationData_Temp(int FeeConfigurationInfoId = 0, int ClassId = 0, decimal MinimumAmoun = 0, string EffectiveDate = "", string strJsonList = "")
        {
            List<FranchiseeFeeConfigurationCustomModel> lstFranchiseeFeeConfiguration = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeFeeConfigurationCustomModel>>(strJsonList);
            if (lstFranchiseeFeeConfiguration != null && lstFranchiseeFeeConfiguration.Count() > 0)
            {
                foreach (var item in lstFranchiseeFeeConfiguration)
                {
                    FranchiseeService.SaveFranchiseeFeeConfigurationData_Temp(item.FeeConfigurationId, ClassId, Convert.ToDecimal(item.MinimumAmount), item.EffectiveDate);
                }
            }
            int Id = 0;
            //Id = FranchiseeService.SaveFranchiseeFeeConfigurationData(FeeConfigurationInfoId, ClassId, MinimumAmoun, EffectiveDate);
            return Json(Id, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveFeeConfigurationRecord(int Id)
        {
            if (Id > 0)
            {
                FranchiseeService.RemoveFeeConfigurationRecord(Id);
            }
            return Json(Id, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FranchiseEditLeasePopup(int lId, int fid)
        {
            decimal oTaxRate = 0;
            if (fid > 0)
            {
                var data = FranchiseeService.GetFranchiseeDetail(fid);
                TaxRateViewModel oTaxRateModel = FranchiseeService.GetTaxRateDetailForFranchiseeSupply(SelectedRegionId);
                ViewBag.Franchisee = data;
                if (oTaxRateModel != null)
                {
                    oTaxRate = Convert.ToDecimal(oTaxRateModel.LeaseTaxRate);
                }
            }
            ViewBag.oTaxRate = oTaxRate;
            ViewBag.fid = fid;
            ViewBag.lId = lId;

            LeaseViewModel ViewModel = new LeaseViewModel();
            var dataModel = FranchiseeService.GetLeaseModel(lId);
            if (dataModel != null)
            {
                ViewModel = dataModel;
            }
            //ViewBag.StatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            return PartialView("_PartialFranchiseEditLeasePopup", ViewModel);
        }
        [HttpPost]
        public ActionResult SaveFranchiseEditLease(LeaseViewModel leaseViewModel, FormCollection collection)
        {
            if (leaseViewModel.LeaseNumber != null)
            {
                leaseViewModel.CreatedDate = DateTime.Now;
                leaseViewModel.ModifiedDate = DateTime.Now;
                leaseViewModel.TypeListId = 2;
                leaseViewModel.IsActive = 1;
                leaseViewModel.RegionId = SelectedRegionId;
                leaseViewModel.CreatedBy = SelectedUserId;

                leaseViewModel.DownPaymentPaid = false;

                if (leaseViewModel.InstallmentDownPaymentNum > 0)
                {
                    leaseViewModel.NumOfPaymentsPaid = -1;
                }
                else
                {
                    leaseViewModel.NumOfPaymentsPaid = 0;
                }
                leaseViewModel.TotalAmountPaid = 0;
                leaseViewModel = FranchiseeService.SaveLease(leaseViewModel.ToModel<Lease, LeaseViewModel>()).ToModel<LeaseViewModel, Lease>();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region :: Maintenance Franchise Details Popup  :: 

        [HttpGet]
        public ActionResult RenderEditFranchiseForMaintenancePopup(int id, bool edit = true)
        {


            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = id;
            int FranchiseeId = Convert.ToInt32(id);
            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            //int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Contact);

            var Data = FranchiseeService.GetFranchisee().Where(one => one.FranchiseeId == FranchiseeId).OrderByDescending(one => one.FranchiseeId).FirstOrDefault().ToModel<FranchiseeViewModel, Franchisee>();
            if (Data != null)
            {
                FullFranchiseeViewModel.BusinessInfo.FranchiseeNo = Data.FranchiseeNo;
                ViewBag.parentid = Data.ParentId;
            }

            FullFranchiseeViewModel.FranchiseOwnersList = FranchiseeService.GetFranchiseeOwnerListById(-1);


            ViewBag.CurrentMenu = "FranchiseGeneral";


            var States = CustomerService.GetStateList();
            if (FullFranchiseeViewModel.BusinessInfoAddress != null)
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.BusinessInfoAddress.StateListId);
            }
            else
            {
                ViewBag.BusinessInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }
            if (FullFranchiseeViewModel.ContactInfoAddress != null)
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.ContactInfoAddress.StateListId);
            }
            else
            {
                ViewBag.ContactInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (FullFranchiseeViewModel.PayeeInfoAddress != null && FullFranchiseeViewModel.PayeeInfoAddress.StateListId > 0)
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name", FullFranchiseeViewModel.PayeeInfoAddress.StateListId);
            }
            else
            {
                ViewBag.PayeeInfoAddress_State = new SelectList(States, "StateListId", "Name");
            }

            if (edit)
            {
                ViewBag.Edit = true;
            }
            else
            {
                ViewBag.Edit = false;

            }

            return PartialView("_EditFranchiseForMaintenancePopup", FullFranchiseeViewModel);
        }


        [HttpPost]
        public ActionResult SaveFranchiseNewdataSave(ParentFranchiseeTabModel ParentFranchiseeTabModel)
        {
            FranchiseeTab1 FranchiseeTab1 = new FranchiseeTab1();
            FranchiseeTab2 FranchiseeTab2 = new FranchiseeTab2();
            FranchiseeTab4 FranchiseeTab4 = new FranchiseeTab4();
            FranchiseeTab5 FranchiseeTab5 = new FranchiseeTab5();

            FranchiseeTab1 = ParentFranchiseeTabModel.FranchiseeTab1;
            FranchiseeTab2 = ParentFranchiseeTabModel.FranchiseeTab2;
            FranchiseeTab4 = ParentFranchiseeTabModel.FranchiseeTab4;
            FranchiseeTab5 = ParentFranchiseeTabModel.FranchiseeTab5;

            FullFranchiseeViewModel fullfranchiseeviewmodel = new FullFranchiseeViewModel();

            #region  :: Tab 1 Data ::

            fullfranchiseeviewmodel.BusinessInfo.RegionId = SelectedRegionId;
            fullfranchiseeviewmodel.BusinessInfo.FranchiseeNo = FranchiseeTab1.FranchiseeNo;
            fullfranchiseeviewmodel.BusinessInfo.Name = FranchiseeTab1.Name;
            fullfranchiseeviewmodel.BusinessInfo.StatusListId = (int)JKApi.Business.Enumeration.FranchiseeStatusList.PendingTransfer;
            if (ParentFranchiseeTabModel.ParentId == 0)
            {
                fullfranchiseeviewmodel.BusinessInfo.ParentId = ParentFranchiseeTabModel.FranchiseeId;
            }
            else
            {
                fullfranchiseeviewmodel.BusinessInfo.ParentId = ParentFranchiseeTabModel.ParentId;
            }
            fullfranchiseeviewmodel.BusinessInfo = FranchiseeService.SaveFranchisee_Temp(fullfranchiseeviewmodel.BusinessInfo.ToModel<Franchisee_Temp, FranchiseeViewModel>(), false).ToModel<FranchiseeViewModel, Franchisee_Temp>();
            //UpdateFranchiseeIndex();

            fullfranchiseeviewmodel.BusinessInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            if (FranchiseeTab1.StateId != "" && FranchiseeTab1.StateId != "0")
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.StateListId = Convert.ToInt32(FranchiseeTab1.StateId);
                int id = Convert.ToInt32(FranchiseeTab1.StateId);
                string state = CustomerService.GetStatesName(id);
                fullfranchiseeviewmodel.BusinessInfoAddress.StateName = state.Trim();
            }
            fullfranchiseeviewmodel.BusinessInfoAddress.Address1 = FranchiseeTab1.Address1;
            fullfranchiseeviewmodel.BusinessInfoAddress.Address2 = FranchiseeTab1.Address2;
            fullfranchiseeviewmodel.BusinessInfoAddress.City = FranchiseeTab1.City;
            fullfranchiseeviewmodel.BusinessInfoAddress.PostalCode = FranchiseeTab1.PostalCode;
            fullfranchiseeviewmodel.BusinessInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.BusinessInfoAddress.IsActive = true;

            var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.BusinessInfoAddress.FullAddress));
            if (_latlng.results.Count() > 0)
            {
                fullfranchiseeviewmodel.BusinessInfoAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.BusinessInfoAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
            }

            fullfranchiseeviewmodel.BusinessInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoPhone.Phone = FranchiseeTab1.Phone1;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.BusinessInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.BusinessInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.BusinessInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.BusinessInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            fullfranchiseeviewmodel.BusinessInfoEmail.EmailAddress = FranchiseeTab1.EmailAddress;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.BusinessInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.BusinessInfoEmail.IsActive = true;

            fullfranchiseeviewmodel.BusinessInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.BusinessInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.BusinessInfoPhone = CustomerService.SavePhone_Temp(fullfranchiseeviewmodel.BusinessInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.BusinessInfoEmail = CustomerService.SaveEmail_Temp(fullfranchiseeviewmodel.BusinessInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();

            #endregion

            #region :: Tab 2 Data ::

            var ListOwner = FranchiseeService.GetFranchiseeOwnerListByIdTemp(-1);
            if (ListOwner.Count() > 0)
            {
                foreach (var item in ListOwner)
                {
                    FranchiseOwnersList fo = new FranchiseOwnersList();
                    fo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
                    fo.FranchiseeOwnerListId = item.FranchiseeOwnerListId;
                    fo.ContactName = item.ContactName;
                    fo.Title = item.Title;
                    fo.IsActive = item.IsActive;
                    fo.CreatedBy = item.CreatedBy;
                    fo.CreatedDate = item.CreatedDate;
                    fo.ModifiedBy = item.ModifiedBy;
                    fo.ModifiedDate = item.ModifiedDate;
                    fo.Email = item.Email;
                    fo.Address = item.Address;
                    fo.City = item.City;
                    fo.StateListId = item.StateListId;
                    fo.PostalCode = item.PostalCode;
                    fo.Phone = item.Phone;
                    fo = FranchiseeService.SaveFranchiseeOwnerList_Temp(fo.ToModel<FranchiseeOwnerList_Temp, FranchiseOwnersList>()).ToModel<FranchiseOwnersList, FranchiseeOwnerList_Temp>();
                }
            }
            fullfranchiseeviewmodel.ContactInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //if (FranchiseeTab2.StateId != "" && FranchiseeTab2.StateId != "0")
            //{
            //    fullfranchiseeviewmodel.ContactInfoAddress.StateListId = Convert.ToInt32(FranchiseeTab2.StateId);
            //    int id = Convert.ToInt32(FranchiseeTab2.StateId);
            //    string state = CustomerService.GetStatesName(id);
            //    fullfranchiseeviewmodel.ContactInfoAddress.StateName = state.Trim();
            //}
            //fullfranchiseeviewmodel.ContactInfoAddress.Address1 = FranchiseeTab2.Address1;
            //fullfranchiseeviewmodel.ContactInfoAddress.Address2 = FranchiseeTab2.Address2;
            //fullfranchiseeviewmodel.ContactInfoAddress.City = FranchiseeTab2.City;
            //fullfranchiseeviewmodel.ContactInfoAddress.PostalCode = FranchiseeTab2.PostalCode;
            //fullfranchiseeviewmodel.ContactInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            //fullfranchiseeviewmodel.ContactInfoAddress.CreatedDate = DateTime.Now;
            //fullfranchiseeviewmodel.ContactInfoAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            //fullfranchiseeviewmodel.ContactInfoAddress.IsActive = true;
            //var _latlngC = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.ContactInfoAddress.FullAddress));
            //if (_latlngC.results.Count() > 0)
            //{
            //    fullfranchiseeviewmodel.ContactInfoAddress.Latitude = decimal.Parse(_latlngC.results[0].geometry.location.lat.ToString());
            //    fullfranchiseeviewmodel.ContactInfoAddress.Longitude = decimal.Parse(_latlngC.results[0].geometry.location.lng.ToString());
            //}

            fullfranchiseeviewmodel.ContactInfoPhone.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoPhone.Phone = FranchiseeTab2.Phone1;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            fullfranchiseeviewmodel.ContactInfoPhone.IsActive = true;


            fullfranchiseeviewmodel.ContactInfoEmail.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfoEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfoEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfoEmail.EmailAddress = FranchiseeTab2.EmailAddress;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfoEmail.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfoEmail.IsActive = true;


            fullfranchiseeviewmodel.ContactInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContactInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.ContactInfo.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            fullfranchiseeviewmodel.ContactInfo.Name = FranchiseeTab2.Name;
            fullfranchiseeviewmodel.ContactInfo.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.ContactInfo.CreatedBy = _claimView.GetCLAIM_USERID();
            fullfranchiseeviewmodel.ContactInfo.IsActive = true;

            fullfranchiseeviewmodel.ContactInfo = CustomerService.SaveContact_Temp(fullfranchiseeviewmodel.ContactInfo.ToModel<Contact_Temp, ContactViewModel>()).ToModel<ContactViewModel, Contact_Temp>();
            //fullfranchiseeviewmodel.ContactInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.ContactInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();
            fullfranchiseeviewmodel.ContactInfoPhone = CustomerService.SavePhone_Temp(fullfranchiseeviewmodel.ContactInfoPhone.ToModel<Phone_Temp, PhoneViewModel>()).ToModel<PhoneViewModel, Phone_Temp>();
            fullfranchiseeviewmodel.ContactInfoEmail = CustomerService.SaveEmail_Temp(fullfranchiseeviewmodel.ContactInfoEmail.ToModel<Email_Temp, EmailViewModel>()).ToModel<EmailViewModel, Email_Temp>();

            #endregion

            #region :: Tab 4/5 Data ::

            //Bill Setting
            fullfranchiseeviewmodel.PayeeInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfo.PayeeName = FranchiseeTab4.PayeeName;
            fullfranchiseeviewmodel.PayeeInfo.C1099Name = FranchiseeTab4.Name1099;
            fullfranchiseeviewmodel.PayeeInfo.Print1099 = FranchiseeTab4.Print1099;
            fullfranchiseeviewmodel.PayeeInfo.Chargeback = FranchiseeTab5.Chargeback;
            fullfranchiseeviewmodel.PayeeInfo.BBPAdministrationFee = FranchiseeTab5.BBPAdministrationFee;
            fullfranchiseeviewmodel.PayeeInfo.AccountRebate = FranchiseeTab5.AccountRebate;
            fullfranchiseeviewmodel.PayeeInfo.GenerateReport = FranchiseeTab5.GenerateReport;


            if (FranchiseeTab4.Incorporated != true)
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = true;
            }
            else
            {
                fullfranchiseeviewmodel.PayeeInfo.Incorporated = false;
            }
            if (fullfranchiseeviewmodel.PayeeInfo != null && !string.IsNullOrEmpty(fullfranchiseeviewmodel.PayeeInfo.PayeeName))
            {
                fullfranchiseeviewmodel.PayeeInfo = FranchiseeService.SaveFranchiseeBillSettings_Temp(fullfranchiseeviewmodel.PayeeInfo.ToModel<FranchiseeBillSettings_Temp, FranchiseeBillSettingViewModel>()).ToModel<FranchiseeBillSettingViewModel, FranchiseeBillSettings_Temp>();
            }

            //Address
            fullfranchiseeviewmodel.PayeeInfoAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.PayeeInfoAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;
            if (FranchiseeTab4.StateId != "" && FranchiseeTab4.StateId != "0")
            {
                fullfranchiseeviewmodel.PayeeInfoAddress.StateListId = Convert.ToInt32(FranchiseeTab4.StateId);
                int id = Convert.ToInt32(FranchiseeTab4.StateId);
                string state = CustomerService.GetStatesName(id);
                fullfranchiseeviewmodel.PayeeInfoAddress.StateName = state.Trim();
            }
            fullfranchiseeviewmodel.PayeeInfoAddress.Address1 = FranchiseeTab4.Address;
            fullfranchiseeviewmodel.PayeeInfoAddress.City = FranchiseeTab4.City;
            fullfranchiseeviewmodel.PayeeInfoAddress.PostalCode = FranchiseeTab4.PostalCode;

            var _latlngP = GetLatLongByAddress(HttpUtility.UrlEncode(fullfranchiseeviewmodel.PayeeInfoAddress.FullAddress));
            if (_latlngP.results.Count() > 0)
            {
                fullfranchiseeviewmodel.PayeeInfoAddress.Latitude = decimal.Parse(_latlngP.results[0].geometry.location.lat.ToString());
                fullfranchiseeviewmodel.PayeeInfoAddress.Longitude = decimal.Parse(_latlngP.results[0].geometry.location.lng.ToString());
            }

            fullfranchiseeviewmodel.PayeeInfoAddress.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeInfoAddress.IsActive = true;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedDate = DateTime.Now;
            fullfranchiseeviewmodel.PayeeInfoAddress.CreatedBy = -1;
            fullfranchiseeviewmodel.PayeeInfoAddress = CustomerService.SaveAddress_Temp(fullfranchiseeviewmodel.PayeeInfoAddress.ToModel<Address_Temp, AddressViewModel>()).ToModel<AddressViewModel, Address_Temp>();

            //PayeeIdentification
            fullfranchiseeviewmodel.PayeeIdentification.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.PayeeIdentification.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            fullfranchiseeviewmodel.PayeeIdentification.ContactTypeListId = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);
            fullfranchiseeviewmodel.PayeeIdentification.IdentifierTypeListId = FranchiseeTab4.IdentifierTypeListId;
            fullfranchiseeviewmodel.PayeeIdentification.IdentifierNumer = FranchiseeTab4.IdentifierNumer;
            fullfranchiseeviewmodel.PayeeIdentification = CustomerService.SaveIdentification_Temp(fullfranchiseeviewmodel.PayeeIdentification.ToModel<Identification_Temp, IdentificationViewModel>()).ToModel<IdentificationViewModel, Identification_Temp>();

            //Bank            
            if (FranchiseeTab4.BankName != null && FranchiseeTab4.BankName != "")
            {
                fullfranchiseeviewmodel.ACHBankInfo.Name = FranchiseeTab4.BankName;
                fullfranchiseeviewmodel.ACHBankInfo.RoutingNumber = FranchiseeTab4.Routing;
                fullfranchiseeviewmodel.ACHBankInfo.AccountNumber = FranchiseeTab4.Account;
                fullfranchiseeviewmodel.ACHBankInfo.Descrption = FranchiseeTab4.Description;
                fullfranchiseeviewmodel.ACHBankInfo.RemittanceNotes = FranchiseeTab4.RemittanceNotes;
                fullfranchiseeviewmodel.ACHBankInfo.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                fullfranchiseeviewmodel.ACHBankInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
                fullfranchiseeviewmodel.ACHBankInfo = FranchiseeService.SaveACHBank_Temp(fullfranchiseeviewmodel.ACHBankInfo.ToModel<ACHBank_Temp, ACHBankViewModel>()).ToModel<ACHBankViewModel, ACHBank_Temp>();
            }
            #endregion

            // Update Franchisee status              
            return Json(fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RenderEditFranchisePlanInfoForMaintenancePopup(int id, int NewId = 0)
        {
            //Add Default Franchisee Fee Details
            if (id > 0)
            {
                FranchiseeService.moveFeeConfigurationDataOldToNewFranchisee(id, NewId);

                FranchiseeService.AddDefaultFranchiseeFees_Temp(NewId, 1, 3);     // 1  : Accounting Fee 
                FranchiseeService.AddDefaultFranchiseeFees_Temp(NewId, 9, 3);   // 9  : Technology Fee    
                FranchiseeService.AddDefaultFranchiseeFees_Temp(NewId, 10, 3);  // 10 :	Advertising Fee
                FranchiseeService.AddDefaultFranchiseeFees_Temp(NewId, 17, 10);  // 17 : Royalty
                FranchiseeService.AddDefaultFranchiseeFees_Temp(NewId, 23, 5.50M);  // 23 : Business Protection    
            }

            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();
            FullFranchiseeViewModel.BusinessInfo.FranchiseeId = Convert.ToInt32(id);
            //ViewBag.CurrentMenu = "FranchiseGeneral";
            //ViewBag.StateList = getUsStatesList();
            ViewBag.NewFranchiseeId = NewId;
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "Franchise");
            //BreadCrumb.Add(Url.Action("Index", "Franchise", new { area = "Portal" }), "General");
            if (id == -1)
            {
                //BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }), "Add");
                // return View();
            }
            else
            {
                //BreadCrumb.Add(Url.Action("Maintenance", "Franchise", new { area = "Portal" }) + "/" + id, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            FullFranchiseeViewModel.ContractInfo.FranchiseeId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            //FullFranchiseeViewModel.ContractInfo = FranchiseeService.GetFranchiseeContract().Where(one => one.FranchiseeId == FullFranchiseeViewModel.ContractInfo.FranchiseeId).OrderByDescending(one => one.FranchiseeContractId).FirstOrDefault().ToModel<FranchiseeContractViewModel, FranchiseeContract>();

            FullFranchiseeViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection().Where(w => (w.FranchiseeFeeList.IsDelete == null || w.FranchiseeFeeList.IsDelete == 0)).ToList();
            FullFranchiseeViewModel.ContractInfo = null;
            //FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration((int)FullFranchiseeViewModel.ContractInfo.FranchiseeId).ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            //.Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault()


            //var FranchiseeFeeConfig = FranchiseeService.GetFranchiseeFeeConfiguration().Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault();

            FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo.ClassId = FullFranchiseeViewModel.BusinessInfo.FranchiseeId;
            //FullFranchiseeViewModel.FranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration().Where(F => F.ClassId == FullFranchiseeViewModel.ContractInfo.FranchiseeId && F.IsActive == true).FirstOrDefault().ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            var lsFranchiseeFeeConfigurationInfo = FranchiseeService.GetFranchiseeFeeConfiguration_Temp().Where(F => F.ClassId == NewId && F.IsActive == true && (F.IsDelete == null || F.IsDelete == false)).ToList();
            ViewBag.lsFranchiseeFeeConfigurationInfo = lsFranchiseeFeeConfigurationInfo;

            //FullFranchiseeViewModel.FeesViewModelList = FranchiseeService.GetFeeListwithFranchiseeId(id);
            FullFranchiseeViewModel.FeeFranchiseeFeeRateTypeListCollectionViewModel = FranchiseeService.GetFeeListCollection_Temp(NewId);
            var lstFranchiseeContractTypeListId = FranchiseeService.GetFranchiseeContractTypeList().ToList();
            if (FullFranchiseeViewModel.ContractInfo != null)
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name", FullFranchiseeViewModel.ContractInfo.FranchiseeContractTypeListId);
            }
            else
            {
                ViewBag.ContractInfo_FranchiseeContractTypeListId = new SelectList(lstFranchiseeContractTypeListId, "FranchiseeContractTypeListId", "Name");
            }

            return PartialView("_EditFranchisePlanInfoForMaintenancePopup", FullFranchiseeViewModel);
        }

        [HttpPost]
        public ActionResult SaveFranchisePlanInfoForMaintenancePopup(FullFranchiseeViewModel fullfranchiseeviewmodel, FormCollection collection)
        {
            int FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            if (collection["hdnNewFranchiseeId"] != null && collection["hdnNewFranchiseeId"] != "")
            {
                fullfranchiseeviewmodel.BusinessInfo.FranchiseeId = Convert.ToInt32(collection["hdnNewFranchiseeId"]);
                fullfranchiseeviewmodel.ContractInfo.FranchiseeId = Convert.ToInt32(collection["hdnNewFranchiseeId"]);
                fullfranchiseeviewmodel.FranchiseeIdByOwner = Convert.ToInt32(collection["hdnNewFranchiseeId"]);
            }

            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }

            if (collection["ContractInfo_FranchiseeContractTypeListId"] != null && collection["ContractInfo_FranchiseeContractTypeListId"] != "")
            {
                fullfranchiseeviewmodel.ContractInfo.FranchiseeContractTypeListId = Convert.ToInt32(collection["ContractInfo_FranchiseeContractTypeListId"]);
            }

            if (collection["FranchiseeFeeConfigurationInfo.MinimumAmount"] != null && collection["FranchiseeFeeConfigurationInfo.MinimumAmount"] != "")
            {
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.MinimumAmount = Convert.ToDecimal(collection["FranchiseeFeeConfigurationInfo.MinimumAmount"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.RegionId = Convert.ToInt32(collection["FranchiseeFeeConfigurationInfo.RegionId"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.FeeId = Convert.ToInt32(collection["FranchiseeFeeConfigurationInfo.FeeId"]);
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.TypeListId = 2;
                fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.IsDelete = false;

            }

            fullfranchiseeviewmodel.ContractInfo.FranchiseeId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            fullfranchiseeviewmodel.ContractInfo.FranchiseeContractId = 0;
            fullfranchiseeviewmodel.ContractInfo = FranchiseeService.SaveFranchiseeContract_Temp(fullfranchiseeviewmodel.ContractInfo.ToModel<FranchiseeContract_Temp, FranchiseeContractViewModel>()).ToModel<FranchiseeContractViewModel, FranchiseeContract_Temp>();


            fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.ClassId = fullfranchiseeviewmodel.BusinessInfo.FranchiseeId;
            //fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo = FranchiseeService.SaveFranchiseeFeeConfiguration(fullfranchiseeviewmodel.FranchiseeFeeConfigurationInfo.ToModel<FeeConfiguration, FranchiseeFeeConfigurationViewModel>(), this.LoginUserId).ToModel<FranchiseeFeeConfigurationViewModel, FeeConfiguration>();
            fullfranchiseeviewmodel.FranchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeListFeeRateTypeListCollectionViewModel = FranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection();


            //Update Status New  Franchisee Status
            FranchiseeService.UpdateFranchiseeStatus_Temp(fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, (int)JKApi.Business.Enumeration.FranchiseeStatusList.Pending);

            //Update Status Old Franchisee Status
            FranchiseeService.UpdateFranchiseeStatus(FranchiseeId, (int)JKApi.Business.Enumeration.FranchiseeStatusList.Transferred);
            var data = FranchiseeService.GetFranchiseeById(FranchiseeId);
            if (data.ParentId == null || data.ParentId == 0)
            {
                data.ParentId = FranchiseeId;
                FranchiseeService.UpdateFranchiseeParentId(data);
            }

            return Json(fullfranchiseeviewmodel.BusinessInfo.FranchiseeId, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeleteCustomerRecord(int Id)
        {
            if (Id > 0)
            {
                FranchiseeService.DeleteFranchisee_Temp(Id);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult InspectionHistory(int? id)
        {
            //ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InspectionHistory", "Franchisee", new { area = "Portal" }), "Franchisee");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Franchisee", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("InspectionHistory", "Franchisee", new { area = "Portal" }), "Inspection History");
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            int FranchissID = id ?? -1;
            ViewBag.FranchissID = FranchissID;
            //ViewBag.CustId = id;
            if (FranchissID > 0)
            {
                var response = jkEntityModel.vw_F_FranchiseeDetail.Where(f => f.FranchiseeId == FranchissID).MapEnumerable<FranchiseeDetailViewModel, vw_F_FranchiseeDetail>().ToList();

                FranchiseeDetailViewModel franchiseeDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    franchiseeDetailViewModel = new FranchiseeDetailViewModel();
                    franchiseeDetailViewModel.Name = String.IsNullOrEmpty(a.Name.ToString()) ? String.Empty : a.Name.ToString();
                    franchiseeDetailViewModel.FranchiseeNo = String.IsNullOrEmpty(a.FranchiseeNo.ToString()) ? String.Empty : a.FranchiseeNo.ToString();
                    franchiseeDetailViewModel.Address1 = String.IsNullOrEmpty(a.Address1.ToString()) ? String.Empty : a.Address1.ToString();
                    franchiseeDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2) ? null : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        franchiseeDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }

                    if (a.Fax != null)
                    {
                        franchiseeDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.FranchiseeNo != null)
                    {
                        franchiseeDetailViewModel.FranchiseeNo = String.IsNullOrEmpty(a.FranchiseeNo.ToString()) ? String.Empty : a.FranchiseeNo.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        franchiseeDetailViewModel.EmailAddress = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.ContactName != null)
                    {
                        franchiseeDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    }
                    //if (a.ContactTitle != null)
                    //{
                    //    customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                    //}
                    if (a.Cell != null)
                    {
                        franchiseeDetailViewModel.Cell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                    }
                    //if (a.EmailAddress != null)
                    //{
                    //    franchiseeDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    //}
                    if (a.Ext != null)
                    {
                        franchiseeDetailViewModel.Ext = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                    }

                    if (a.Amount != null)
                    {
                        if (a.Amount.ToString().Trim().Length > 0)
                        {
                            franchiseeDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                        }
                        else
                        {
                            franchiseeDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    else
                    {
                        franchiseeDetailViewModel.Amount = Convert.ToDecimal("0.00");
                    }
                }
                ViewBag.FranchiseeDetail = franchiseeDetailViewModel;
                TempData["FranchiseeDetail"] = franchiseeDetailViewModel;
                //FullCustomerViewModel = Maintennacepopups(CustID);

                //FullCustomerViewModel.CustomerDetail = franchiseeDetailViewModel;
            }

            if (FranchissID > 0)
            {
                // TODO: Use GetInspectionFormHistoryListByCustomer
                var inspectionViewModel = _inspectionService.GetInspectionFormHistoryListByFranchisee(new ViewInspectionFormListModel
                {
                    FranchiseeId = FranchissID,
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
    }
}