using Application.Web.Core;
using AuthorizeNet.Api.Contracts.V1;
using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.AccountReceivable;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service;
using JKApi.Service.Service.Administration.General;
using JKApi.Service.Service.Customer;
using JKApi.Service.Service.TaxAPI;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Franchisee;
using JKApi.Service.ServiceContract.Inspection;
using JKApi.Service.ServiceContract.Management;
using JKViewModels;
using JKViewModels.AccountReceivable;
using JKViewModels.Common;
using JKViewModels.CRM;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using MoreLinq;
using MvcBreadCrumbs;
using MxMarchantQuickType;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using JKViewModels.Inspection;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    //[OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Portal", Order = 0)]
    public class CustomerController : ViewControllerBase
    {
        //static JKApi.Data.DAL.jkDatabaseEntities jkEntityModel = new JKApi.Data.DAL.jkDatabaseEntities();
        //static JKApi.Data.JkControl.jkControlEntities jkControlEntites = new JKApi.Data.JkControl.jkControlEntities();
        private FranchiseService serv = new FranchiseService();
        ImportTax _ImpTax = new ImportTax();

        //private jkDatabaseEntities db = new jkDatabaseEntities();
        public CustomerController(ICustomerService customerService, IManagementService managementService, ICacheProvider CacheProvider, IFranchiseeService _FranchiseeService, ICommonService commonService, IAccountReceivableService _AccountReceivableService, IUserService userService, IInspectionService inspectionService, ICRM_Service CRM_Service)
        {
            FranchiseeService = _FranchiseeService;
            ManagementService = managementService;
            AccountReceivableService = _AccountReceivableService;
            CustomerService = customerService;
            this._cacheProvider = CacheProvider;
            _commonService = commonService;
            this._userService = userService;
            _inspectionService = inspectionService;
            _crmService = CRM_Service;
            ViewBag.HMenu = "Customer";
        }

        // GET: Portal/Customer
        public ActionResult Index()
        {
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "General");

            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View(model);
        }

        #region chart data.....
        public ActionResult GetRevenueWiseTopCustomerChartData(int recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = ManagementService.GetRevenueWiseTopCustomerChartData(recordNumber, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRevenueWiseTopCustomerChartDataDetails(string flag, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = ManagementService.GetRevenueWiseTopCustomerChartDataDetails(flag, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRevenueWiseTopCustomerChartDataDetailsView(string flag, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();
            var data = ManagementService.GetRevenueWiseTopCustomerChartDataDetails(flag, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return PartialView("_RevenueWiseTopCustomerChartDataView", data);
        }

        public ActionResult GetNewAndCanceledCustomerChartData(int recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId.ToString();

            var data = ManagementService.GetNewAndCanceledCustomerChartData(recordNumber, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            List<CustomertChartDetailsData> detailsList = new List<CustomertChartDetailsData>();
            if (data != null)
            {
                var addedYears = data.GroupBy(x => x.BillYear).Select(x => x.FirstOrDefault()).OrderBy(o => o.BillYear);
                foreach (var item in addedYears)
                {
                    var customerList = data.Where(o => o.BillYear == item.BillYear);
                    decimal cancelCustomer = 0;
                    decimal newCustomer = 0;
                    foreach (var cl in customerList)
                    {
                        if (cl.Flag == 1)
                        {
                            cancelCustomer = cl.TotalRevenue;
                        }
                        if (cl.Flag == 2)
                        {
                            newCustomer = cl.TotalRevenue;
                        }
                    }

                    var model = new CustomertChartDetailsData
                    {
                        TotalCancel = cancelCustomer,
                        TotalNew = newCustomer,
                        ColorCode = item.ColorCode,
                        RangeName = item.BillYear.ToString()
                    };
                    detailsList.Add(model);
                }
            }
            return Json(detailsList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCancelVsNewCustomerDetail(string flag, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetCancelVsNewCustomerDetail(flag, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCancelVsNewCustomerDetails(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetCancelVsNewCustomerDetail(flag, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion chart data.....

        #region Customer > General

        [HttpGet]
        public ActionResult Search()
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Search", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("Search", "Customer", new { area = "Portal" }), "Search");

            ViewBag.SearchIn = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.SearchIn>().Select(k => new SelectListItem { Text = k.Key, Value = k.Value.ToString() }).OrderBy(s => s.Text).ToList();
            ViewBag.Status = serv.GetFranchiseStatusListItem().ToList().Select(k => new SelectListItem { Text = k.Name, Value = k.StatusListId.ToString() }).OrderBy(s => s.Text).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            if (collection["collection"] != null)
            {
                var id = (collection["customernumber"]);
                ViewBag.CurrentMenu = "CustomerGeneral";
                int CustId = CustomerService.GetCustomers().Where(x => x.CustomerNo == id).FirstOrDefault().CustomerId;
                return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = id });
                //return RedirectToAction("SearchList", new { searchin = collection["SearchIn"].ToString(), searchvalue = collection["For"].ToString(), status = collection["Status"].ToString() });
            }
            return View();
        }

        [HttpGet]
        public JsonResult CustomerAutoComplete(string keyword)
        {
            //if (namePrefix == null)
            //{
            //    NLogger.Error("Requested SummaryData is null or empty");
            //    return Json(new { success = false, message = "Data is null or empty" });
            //}

            //List<Customer> SearchData = null;
            //if (InvoicePrefix == "1")
            //{
            //    SearchData = CustomerService.GetSearchCustomers().Where(x => x.Name.Contains(namePrefix.ToUpper())).ToList();
            //}
            //if (InvoicePrefix == "2")
            //{
            //    SearchData = CustomerService.GetSearchCustomers().Where(x => x.CustomerNo.Contains(namePrefix)).ToList();
            //}

            //var result = from cust in SearchData select new { cust.CustomerId, cust.Name, cust.CustomerNo };

            //return Json(result, JsonRequestBehavior.AllowGet);

            List<Customer> lstCustomer = CustomerService.GetCustomerListData(keyword);

            var result = lstCustomer.Select(o => new { CustomerId = o.CustomerId, Name = o.Name.Trim(), CustomerNo = o.CustomerNo });/* from cust in lstCustomer select new { cust.CustomerId, cust.Name };*/

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult Customeraddress(int? CustomerId)
        {
            if (CustomerId == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var address = CustomerService.GetAddressByCustomerId((int)CustomerId).Select(x => new { x.Address1, x.Address2, x.City, x.StateName, x.PostalCode, x.AddressId }).FirstOrDefault();
            return this.Json(address, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCustomer(string searchText = "")
        {
            var searchData = CustomerService.GetSearchCustomer(searchText);

            var jsonResult = Json(searchData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult SearchList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("SearchList", "Customer", new { area = "Portal" }) + "?searchin=7&status=1", "Customer");
            BreadCrumb.Add(Url.Action("SearchList", "Customer", new { area = "Portal" }) + "?searchin=7&status=1", "Customer List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 1);

            //ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            ViewBag.selectedRegionId = SelectedRegionId;
            //var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).DistinctBy(one => one.Customer).ToList();
            //response.Select(x => x.MainAddress.StateListId);
            return View();
        }

        [HttpGet]
        public ActionResult CustomerSearchList(string status, string rgId)
        {
            try
            {
                var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var customers = CustomerService.GetCustomerSearchList(contactTypeList, status, rgId);
                
                //var result = (from f in customers
                //              select new
                //              {
                //                  f.CustomerId,
                //                  f.CustomerNo,
                //                  f.CustomerName,
                //                  f.Address,
                //                  f.StateName,
                //                  f.City,
                //                  f.PostalCode,
                //                  //CustomerName = "CustomerName",
                //                  //Address= "Address",
                //                  //StateName= "StateName",
                //                  //City= "City",
                //                  //PostalCode= "PostalCode",
                //                  Amount = string.Format("{0:c}", f.Amount),
                //                  Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty,
                //                  f.RegionName,
                //                  StatusName = (f.StatusName ?? "").Trim(),
                //                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                //                  f.LogMessage
                //              }).ToList();



                var jsonResult = Json(new
                {
                    aadata = customers,
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SearchList(CustomerSearchResultViewModelListModel model)
        {
            string StatusList = string.Empty;
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("SearchList", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("SearchList", "Customer", new { area = "Portal" }) + "?searchin=7&status=1", "List");

            int CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            model.lstCustomerSearchResultViewModel = CustomerService.GetCustomerSearchList(0, CustomerContactTypeList, model.StatusIds);

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name");

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text");

            //var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).DistinctBy(one => one.Customer).ToList();
            //response.Select(x => x.MainAddress.StateListId);
            return View(model);
        }

        //public ActionResult CustomerSearchDataTable(string status)
        //{
        //    try
        //    {
        //        int StatusId = Convert.ToInt32(id);
        //        int CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
        //        int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);

        //        var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).DistinctBy(one => one.Customer).ToList();

        //        //int StatusId = Convert.ToInt32(status);
        //        //int FranchiseTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franachisee);
        //        //int FranchiseContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
        //        //var response = FranchiseeService.GetFranchiseeList();
        //        var result = from f in response.
        //                     select new
        //                     {
        //                         ID = f.FranchiseeId,
        //                         Number = f.FranchiseeNo,
        //                         Name = f.Name,
        //                         Address = f.Address1,
        //                         DistributionAmount = string.Format("{0:c}", f.DistributionAmount),
        //                         Phone = f.Phone != null ? PhoneNoformat(f.Phone) : string.Empty
        //                     };

        //        return Json(new
        //        {
        //            aadata = result,
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public ActionResult CustomerMaintenance(int id = 0)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerMaintenance", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CustomerMaintenance", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CustomerMaintenance", "Customer", new { area = "Portal" }), "Maintenance");
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            //ViewBag.StatusList = new SelectList(CustomerService.GetAll_StatusList().Where(x => x.TypeListId == TypeList && x.MaintenanceTypeListId == 8), "StatusListId", "Name");
            ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList).Where(x => x.MaintenanceTypeListId == 1 || x.MaintenanceTypeListId == 10 || x.MaintenanceTypeListId == 16), "MaintenanceTypeListId", "Name");
            ViewBag.StatusReasonList = new SelectList(CustomerService.GetAll_StatusReasonList(1), "StatusReasonListId", "Name");
            CustomerMaintenanceViewModel oCustomerMaintenanceViewModel;
            if (id > 0)
            {
                oCustomerMaintenanceViewModel = CustomerService.GetCustomerMaintenanceDetailsById(id);
                if (oCustomerMaintenanceViewModel.StatusListId == 1)
                {
                    ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList)
                               .Where(x => x.MaintenanceTypeListId == 10 || x.MaintenanceTypeListId == 16), "MaintenanceTypeListId", "Name");
                }
                else if (oCustomerMaintenanceViewModel.StatusListId == 2)
                {
                    ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList)
                               .Where(x => x.MaintenanceTypeListId == 1), "MaintenanceTypeListId", "Name");
                }
            }
            else
            {
                oCustomerMaintenanceViewModel = new CustomerMaintenanceViewModel();
                oCustomerMaintenanceViewModel.FindersFee = new List<CustomerStatusFindersFeeViewModel>();
            }
            return View(oCustomerMaintenanceViewModel);
        }

        [HttpGet]
        public ActionResult CustomerMaintenancePartial(int id = 0)
        {
            //ViewBag.CurrentMenu = "CustomerGeneral";
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            //BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
            //BreadCrumb.Add(Url.Action("CustomerMaintenance", "Customer", new { area = "Portal" }), "Maintenance");


            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList)
                                .Where(x => x.MaintenanceTypeListId == 1 || x.MaintenanceTypeListId == 10 || x.MaintenanceTypeListId == 16), "MaintenanceTypeListId", "Name");
            //ViewBag.StatusList = new SelectList(CustomerService.GetAll_StatusList(), "StatusListId", "Name");
            ViewBag.StatusReasonList = new SelectList(CustomerService.GetAll_StatusReasonList(1), "StatusReasonListId", "Name");
            CustomerMaintenanceViewModel oCustomerMaintenanceViewModel;
            if (id > 0)
            {
                oCustomerMaintenanceViewModel = CustomerService.GetCustomerMaintenanceDetailsById(id);

                if (oCustomerMaintenanceViewModel.StatusListId == 1)
                {
                    ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList)
                               .Where(x => x.MaintenanceTypeListId == 10 || x.MaintenanceTypeListId == 16), "MaintenanceTypeListId", "Name");
                }
                else if (oCustomerMaintenanceViewModel.StatusListId == 2)
                {
                    ViewBag.StatusList = new SelectList(CustomerService.GetAll_MaintenanceTypeList(TypeList)
                               .Where(x => x.MaintenanceTypeListId == 1), "MaintenanceTypeListId", "Name");
                }

            }
            else
            {
                oCustomerMaintenanceViewModel = new CustomerMaintenanceViewModel();
                oCustomerMaintenanceViewModel.FindersFee = new List<CustomerStatusFindersFeeViewModel>();
            }






            return PartialView("_CustomerMaintenance", oCustomerMaintenanceViewModel);
        }

        [HttpGet]
        public JsonResult GetCustomerMaintenanceCancellationInvoiceDetails(int customerId, DateTime date)
        {
            var result = CustomerService.GetCancellationInvoiceList(customerId, date.Month, date.Year);

            var minResult = result.Select(o => new
            {
                InvoiceId = o.InvoiceId,
                CreditPercent = CalculateCancellationCreditPercent(o, date)
            }).Where(o => o.CreditPercent > 0);

            var jsonResult = Json(minResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private decimal CalculateCancellationCreditPercent(portal_spGet_C_CancellationInvoiceList_Result details, DateTime effectiveDate)
        {
            DateTime invoiceDate = details.InvoiceDate.Value;
            if (invoiceDate.Year > effectiveDate.Year ||
                (invoiceDate.Year == effectiveDate.Year && invoiceDate.Month > effectiveDate.Month))
                return 1; // full credit back if invoice is for a month after last service date

            if (details.CleanFrequencyListId != 1) return 0;

            int numDaysBilled = 0;
            int numDaysAlreadyWorked = 0;

            DateTime checkedDay = new DateTime(effectiveDate.Year, effectiveDate.Month, 1);

            while (checkedDay.Month == effectiveDate.Month)
            {
                var dayOfWeek = checkedDay.DayOfWeek;
                var isDayOfWeekWorked = false;

                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday: isDayOfWeekWorked = (details.Mon ?? false); break;
                    case DayOfWeek.Tuesday: isDayOfWeekWorked = (details.Tues ?? false); break;
                    case DayOfWeek.Wednesday: isDayOfWeekWorked = (details.Wed ?? false); break;
                    case DayOfWeek.Thursday: isDayOfWeekWorked = (details.Thur ?? false); break;
                    case DayOfWeek.Friday: isDayOfWeekWorked = (details.Fri ?? false); break;
                    case DayOfWeek.Saturday: isDayOfWeekWorked = (details.Sat ?? false); break;
                    case DayOfWeek.Sunday: isDayOfWeekWorked = (details.Sun ?? false); break;
                }

                if (isDayOfWeekWorked)
                {
                    numDaysBilled++;
                    if (checkedDay.Day <= effectiveDate.Day)
                        numDaysAlreadyWorked++;
                }

                checkedDay = checkedDay.AddDays(1);
            }

            if (numDaysBilled == 0) return 0;

            return 1 - ((decimal)numDaysAlreadyWorked / (decimal)numDaysBilled);
        }

        [HttpPost]
        public JsonResult CustomerMaintenancePartial(CustomerMaintenanceViewModel model, FormCollection frm)
        {
            if (!string.IsNullOrEmpty(frm["item.FindersFeeId"]))
            {
                List<CustomerStatusFindersFeeViewModel> lstFindersFee = new List<CustomerStatusFindersFeeViewModel>();
                CustomerStatusFindersFeeViewModel oFindersFee = new CustomerStatusFindersFeeViewModel();

                if (!string.IsNullOrEmpty(frm["item.FindersFeeId"]))
                {
                    string[] strFFI = frm["item.FindersFeeId"].ToString().Split(',');
                    string[] strFFSD = frm["item.FindersFeeStopDate"].ToString().Split(',');
                    string[] strFFCF = frm["item.FindersFeeHasCancellationFee"].ToString().Split(',');

                    for (int i = 0; i < strFFI.Length; i++)
                    {
                        oFindersFee = new CustomerStatusFindersFeeViewModel();
                        oFindersFee.FindersFeeId = !string.IsNullOrEmpty(strFFI[i]) ? int.Parse(strFFI[i].ToString()) : 0;

                        if (strFFSD != null && i <= strFFSD.Length - 1)
                            oFindersFee.FindersFeeStopDate = !string.IsNullOrEmpty(strFFSD[i]) ? DateTime.Parse(strFFSD[i].ToString()) : DateTime.Now;
                        else
                            oFindersFee.FindersFeeStopDate = DateTime.Now;

                        if (strFFCF != null && i <= strFFCF.Length - 1)
                            oFindersFee.FindersFeeHasCancellationFee = !string.IsNullOrEmpty(strFFCF[i]) ? bool.Parse(strFFCF[i].ToString()) : false;
                        else
                            oFindersFee.FindersFeeHasCancellationFee = false;

                        lstFindersFee.Add(oFindersFee);
                    }
                }

                model.FindersFee = lstFindersFee;
            }
            //CustomerService.InsertUpdateCustomerMaintenance(model);
            CustomerService.InsertUpdateCustomerMaintenanceTemp(model);

            return Json("Sucess");
        }
        [HttpPost]
        public JsonResult CustomerMaintenancePartialJson(CustomerMaintenanceViewModel model, FormCollection frm)
        {

            if (!string.IsNullOrEmpty(frm["item.FindersFeeId"]))
            {
                List<CustomerStatusFindersFeeViewModel> lstFindersFee = new List<CustomerStatusFindersFeeViewModel>();
                CustomerStatusFindersFeeViewModel oFindersFee = new CustomerStatusFindersFeeViewModel();

                if (!string.IsNullOrEmpty(frm["item.FindersFeeId"]))
                {
                    string[] strFFI = frm["item.FindersFeeId"].ToString().Split(',');
                    string[] strFFSD = frm["item.FindersFeeStopDate"].ToString().Split(',');
                    string[] strFFCF = frm["item.FindersFeeHasCancellationFee"].ToString().Split(',');

                    for (int i = 0; i < strFFI.Length; i++)
                    {
                        oFindersFee = new CustomerStatusFindersFeeViewModel();
                        oFindersFee.FindersFeeId = !string.IsNullOrEmpty(strFFI[i]) ? int.Parse(strFFI[i].ToString()) : 0;
                        oFindersFee.FindersFeeStopDate = !string.IsNullOrEmpty(strFFSD[i]) ? DateTime.Parse(strFFSD[i].ToString()) : DateTime.Now;

                        if (i < strFFCF.Length)
                        {
                            oFindersFee.FindersFeeHasCancellationFee = !string.IsNullOrEmpty(strFFCF[i]) ? bool.Parse(strFFCF[i].ToString()) : false;
                        }
                        else
                        {
                            oFindersFee.FindersFeeHasCancellationFee = false;
                        }
                        lstFindersFee.Add(oFindersFee);
                    }
                }

                model.FindersFee = lstFindersFee;
            }
            model.LastServiceDate = model.StatusDate;
            model.ResumeDate = model.StatusDate;
            var MaintenanceId = CustomerService.InsertUpdateCustomerMaintenanceTemp(model);
            return Json(MaintenanceId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CustomerMaintenance(CustomerMaintenanceViewModel model, FormCollection frm)
        {

            foreach (var item in model.FindersFee)
            {
                item.FindersFeeId = !string.IsNullOrEmpty(item.FindersFeeId.ToString()) ? int.Parse(item.FindersFeeId.ToString()) : 0;
                item.FindersFeeStopDate = !string.IsNullOrEmpty(item.FindersFeeStopDate.ToString()) ? DateTime.Parse(item.FindersFeeStopDate.ToString()) : DateTime.Now;
                item.FindersFeeHasCancellationFee = !string.IsNullOrEmpty(item.FindersFeeHasCancellationFee.ToString()) ? bool.Parse(item.FindersFeeHasCancellationFee.ToString()) : false;
            }

            //if (!string.IsNullOrEmpty(frm["item.FindersFeeId"]))
            //{
            //    List<CustomerStatusFindersFeeViewModel> lstFindersFee = new List<CustomerStatusFindersFeeViewModel>();
            //    CustomerStatusFindersFeeViewModel oFindersFee = new CustomerStatusFindersFeeViewModel();

            //    oFindersFee.FindersFeeId = !string.IsNullOrEmpty(frm["item.FindersFeeId"]) ? int.Parse(frm["item.FindersFeeId"].ToString()) : 0;
            //    oFindersFee.FindersFeeStopDate = !string.IsNullOrEmpty(frm["item.FindersFeeStopDate"]) ? DateTime.Parse(frm["item.FindersFeeId"].ToString()) : DateTime.Now;
            //    oFindersFee.FindersFeeHasCancellationFee = !string.IsNullOrEmpty(frm["item.FindersFeeHasCancellationFee"]) ? bool.Parse(frm["item.FindersFeeId"].ToString()) : false;
            //    lstFindersFee.Add(oFindersFee);
            //    model.FindersFee = lstFindersFee;
            //}
            var MaintenanceId = CustomerService.InsertUpdateCustomerMaintenanceTemp(model);

            //return RedirectToAction("CustomerMaintenance", "Customer", new { area = "portal", id = model.CustomerId });
            return RedirectToAction("SearchList", "Customer", new { area = "portal", @searchin = 1, @status = 1 });
        }



        //[HttpGet]
        //public JsonResult Customerdata(string keyword)
        //{
        //    List<Customer> lstCustomer = CustomerService.GetCustomerListData(keyword);

        //    var result = lstCustomer.Select(o => new { CustomerId = o.CustomerId, Name = o.CustomerNo + " " + o.Name.Trim() });/* from cust in lstCustomer select new { cust.CustomerId, cust.Name };*/
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult GetCustomerInfo(int ClassID)
        {
            var result = CustomerService.SpGetCustomerDetailsByCustomerID(ClassID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DistributionDetaildata(int customerid)
        {
            return Json(CustomerService.GetDistributionDetailData(customerid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetServiceCallList(int ClassID)
        {
            jkDatabaseEntities jk = new jkDatabaseEntities();
            List<ServiceCallGrid> result = new List<ServiceCallGrid>();
            var results = jk.SpGetServicesCallLog(ClassID).Select(r => new { r.Action, r.CallBack, r.Comments, r.CustomerNo, r.SpokeWith, r.Status }).ToList();
            foreach (var r in results)
            {
                ServiceCallGrid rr = new ServiceCallGrid();
                rr.Action = r.Action;
                rr.CallBack = r.CallBack;
                rr.Comments = r.Comments;
                rr.CustomerNo = r.CustomerNo;
                rr.SpokeWith = r.SpokeWith;
                rr.Status = r.Status;
                result.Add(rr);
            }


            var jsonResult = Json(new { aaData = result, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult Maintenance()
        {
            int currentId = 0;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            // CustomerViewModel.olist = CustomerService.GetOfficeContacts(currentId);
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.ArStatus = getARStatusResonList();
            ViewBag.InvoiceDate = getInvoiceDate();
            ViewBag.TermDate = getTermDate();
            ViewBag.TaxAuthority = getTaxAuthority();

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).ToList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return View(CustomerViewModel);
        }

        [HttpGet]
        public ActionResult MaintenanceStepOne(int? id, string title = "", string phone = "")
        {
            //int currentId = 0;
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            if (id == null)
            {
                ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();
                ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();


                ViewBag.cFrequencyList = getFrequencyList();
                ViewBag.cAccountType = new SelectList(getCAccountType());
                ViewBag.statusList = new SelectList(getStatusList());
                ViewBag.MainStatesList = new SelectList(getUsStatesList(), "Value", "Text");

                ViewBag.BillingStatesList = new SelectList(getUsStatesList(), "Value", "Text");
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate());
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
                ViewBag.TaxAuthority = new SelectList(getTaxAuthority());

                ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();
                ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

                //ViewBag.cFrequencyList = getFrequencyList();
                //ViewBag.cAccountType = getCAccountType();
                //ViewBag.statusList = getStatusList();
                // ViewBag.MainStatesList = getUsStatesList();
                // ViewBag.BillingStatesList = getUsStatesList();
                //ViewBag.TaxAuthority = getTaxAuthority();

                if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
                {
                    ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", CustomerViewModel.BillSetting.ARStatusListId);
                }
                else
                {
                    ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                }
                if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
                {
                    ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name", CustomerViewModel.BillSetting.InvoiceDateListId);
                }
                else
                {
                    ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                }

                if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
                {
                    ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
                }
                else
                {
                    ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
                }

                //var data = jkControlEntites.vw_sys_C_Status.OrderBy(s => s.name).ToList();
                //List<SelectListItem> statusList = new List<SelectListItem>();

                //foreach (var y in data)
                //{
                //    statusList.Add(new SelectListItem { Text = y.name.ToString(), Value = y.id.ToString() });
                //}
                //ViewBag.statusList = statusList;

                //var stateData = jkControlEntites.vw_sys_State.OrderBy(s => s.Name).ToList();
                //List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.Abbr }).ToList();

                //ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
                //ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            }
            else
            {
                BreadCrumb.Add(Url.Action("MaintenanceStepOne", "Customer", new { area = "Portal" }) + "/" + id, "Manage");
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

                CustomerViewModel.CustomerDetail.CustomerId = id.ToString();
            }
            //CustomerViewModel.olist = CustomerService.GetOfficeContacts(Convert.ToInt16( id));
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var States = CustomerService.GetStateList();

            ViewBag.StateAbbr = new SelectList(States, "StateListId", "Name");
            ViewBag.BillingState = new SelectList(States, "StateListId", "Name");
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", CustomerViewModel.BillSetting.ARStatusListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name", CustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null && CustomerViewModel.MainAddress.StateListId != null)
            {
                ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name", CustomerViewModel.MainAddress.StateListId);
            }
            else
            {
                ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name");
            }
            //if (CustomerViewModel != null && CustomerViewModel.MainAddress != null && CustomerViewModel.MainAddress.StateName != null)
            //{
            //    ViewBag.MainStateName = new SelectList(States, "Id", "Name", CustomerViewModel.MainAddress.StateName);
            //}
            //else
            //{
            //    ViewBag.MainStateName = new SelectList(States, "Id", "Name");
            //}
            if (CustomerViewModel != null && CustomerViewModel.BillingAddress != null && CustomerViewModel.BillingAddress.StateListId != null)
            {
                ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name", CustomerViewModel.BillingAddress.StateListId);
            }
            else
            {
                ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name");
            }
            int IsExist = 0;
            if (TempData["IsExist"] != null)
            {
                IsExist = Convert.ToInt32(TempData["IsExist"]);
            }
            ViewBag.IsExist = IsExist;
            ViewBag.CustomerName = title;
            ViewBag.CustomerPhone = phone;
            return View(CustomerViewModel);
        }

        [HttpGet]
        public ActionResult MaintenanceStepTwo(int id = -1)
        {
            //int currentId = 0;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsByIdWithActiveData(Convert.ToInt32(id));
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address += " " + CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address;
            }

            //  CustomerViewModel.olist = CustomerService.GetOfficeContacts(id);
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", CustomerViewModel.BillSetting.ARStatusListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name", CustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return View(CustomerViewModel);
        }

        public static ContractDetailViewModel ContractDetailViewModelToEntity(ContractDetail contractDetail)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ContractDetail, ContractDetailViewModel>();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<ContractDetail, ContractDetailViewModel>(contractDetail);
        }

        [HttpGet]
        public ActionResult MaintenanceStepThree(int id = -1)
        {
            ViewBag.Id = id;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address1 = string.Empty;
            string Address2 = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            var StateList = CustomerService.GetStateList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null)
            {
                //var result = CustomerService.GetContractDetailResult(CustomerViewModel);
                if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null)
                {
                    CustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == CustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
                }
                else
                {
                    CustomerViewModel._ContractDetail = null;
                }
                var servicename = CustomerService.GetServiceTypeList();
                if (CustomerViewModel._ContractDetail != null)
                {
                    foreach (var row in CustomerViewModel._ContractDetail)
                    {
                        var name = servicename.Where(x => x.ServiceTypeListid == row.ServiceTypeListId).FirstOrDefault().name;
                        row.ServiceTypeName = name.ToString();
                    }
                }
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = StateList;
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;
            }

            // CustomerViewModel.olist = CustomerService.GetOfficeContacts(id);
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            //ViewBag.MainStatesList = getUsStatesList();
            //ViewBag.ContractStatesList = CustomerService.GetStatesList();
            //ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();
            ViewBag.SoldBy = getTaxAuthority();
            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name", CustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name");
            }
            var FrequencyList = CustomerService.GetFrequencyList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            {
                ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            }
            else
            {
                ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name");
            }
            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", CustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AgreementTypeListId != null)
            {
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", CustomerViewModel.Contract.AgreementTypeListId);
                var AgreementTypeCPI = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId);
                if (AgreementTypeCPI != null)
                {
                    ViewBag.AgreementTypeCPI = (AgreementTypeCPI.FirstOrDefault().CPI.HasValue ? AgreementTypeCPI.FirstOrDefault().CPI.Value : false);
                }
            }
            else
            {
                ViewBag.AgreementTypeCPI = false;
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", 2);
            }

            //var ContractTermList = CustomerService.GetContractTermList().ToList();
            //if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractTermListId != null)
            //{
            //    ViewBag.ContractTermList = new SelectList(ContractTermList, "ContractTermListId", "Number", CustomerViewModel.Contract.ContractTermListId);
            //}
            //else
            //{
            //    ViewBag.ContractTermList = new SelectList(ContractTermList, "ContractTermListId", "Number");
            //}

            //var ContractServiceTypeList = CustomerService.GetContractServiceTypeList().ToList();
            //if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractServiceTypeListId != null)
            //{
            //    ViewBag.ContractServiceTypeList = new SelectList(ContractServiceTypeList, "ContractServiceTypeListId", "Name", CustomerViewModel.Contract.ContractServiceTypeListId);
            //}
            //else
            //{
            //    ViewBag.ContractServiceTypeList = new SelectList(ContractServiceTypeList, "ContractServiceTypeListId", "Name");
            //}

            var ContractStatusList = CustomerService.GetContractStatusReasonList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractStatusReasonListId != null)
            {
                ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", CustomerViewModel.Contract.ContractStatusReasonListId);
            }
            else
            {
                ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;
            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.ServiceTypeListId != null)
            {
                ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", CustomerViewModel.ContractDetail.ServiceTypeListId);
            }
            else
            {
                ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
            }
            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            }
            else
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
            }

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            if (CleanFrequencyListModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.CleanFrequencyListId != null)
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", CustomerViewModel.ContractDetail.CleanFrequencyListId);
            }
            else
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
            }

            CustomerViewModel.ContractAddress = CustomerViewModel.ContactInformationAddress;

            if (StateList != null && CustomerViewModel.ContractAddress != null && CustomerViewModel.ContractAddress.StateListId != null)
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name", CustomerViewModel.ContractAddress.StateListId);
            }
            else
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name");
            }

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true).Where(x => x.IsAccExec).ToList();
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            //var stateData = jkControlEntites.vw_sys_State.OrderBy(s => s.Name).ToList();
            //List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.Abbr }).ToList();
            //ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            //ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            int IsSave = 0;
            if (TempData["IsSave"] != null)
            {
                IsSave = Convert.ToInt32(TempData["IsSave"]);
            }
            ViewBag.IsSave = IsSave;

            return View(CustomerViewModel);
        }

        public ActionResult CustomerUploadDocument(int id = -1)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");

            ViewBag.Id = id;
            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(1), "FileTypeListId", "Name");

            string Address1 = string.Empty;
            string Address2 = string.Empty;
            FullCustomerViewModel CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.CustomerNo = CustomerViewModel.CustomerViewModel.CustomerNo;
                ViewBag.CustomerName = CustomerViewModel.CustomerViewModel.Name;
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;
            }

            return View(CustomerService.GetCUploadDocument(id, 1));
        }

        public string UploadCustomerFile(HttpPostedFileBase File, int CustomerId, int FileTypeId, string File_Title = "")
        {
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument")))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument"));
            }
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));
            }
            if (File != null)
            {
                if (File.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(File.FileName).Replace(" ", "_");

                    int _FileSize = Path.GetFileName(File.FileName).Length;
                    string _FileExt = Path.GetFileName(File.FileName).Split('.').Last();
                    string _SFileName = _FileName.Trim() + "_" + DateTime.Now.ToString("HHmmsstt") + "." + _FileExt;

                    string _path = Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString(), _SFileName);
                    File.SaveAs(_path);

                    String _FilePath = "/CustomerDocument/" + CustomerId.ToString() + "/" + _SFileName;

                    CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, FileTypeId, _FilePath, string.IsNullOrWhiteSpace(File_Title) ? _FileName : File_Title, _FileExt, _FileSize, string.IsNullOrWhiteSpace(File_Title) ? false : true);
                }
            }
            return string.Empty;
        }

        [HttpPost]
        public ActionResult CustomerUploadDocument(FormCollection collection, HttpPostedFileBase[] file)
        {
            try
            {
                string[] strFTL = collection["hdfFiletypeListIds"].ToString().Split(',');

                int len = strFTL.Length;
                for (int i = 0; i < len; i++)
                {
                    if (strFTL[i] != "")
                    {
                        int ftlID = int.Parse(strFTL[i]);

                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString()));
                        //}
                        //if (file[i] != null)
                        //{
                        //    if (file[i].ContentLength > 0)
                        //    {
                        //        string _FileName = Path.GetFileName(file[i].FileName);

                        //        int _FileSize = Path.GetFileName(file[i].FileName).Length;
                        //        string _FileExt = Path.GetFileName(file[i].FileName).Split('.').Last();
                        //        string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                        //        string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString(), _SFileName);
                        //        file[i].SaveAs(_path);

                        //        String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + collection["hdfCustomerId"].ToString() + "/" + _SFileName;

                        //        CustomerService.SaveUploadDocument(int.Parse(collection["hdfCustomerId"].ToString()), 1, ftlID, _FilePath, _FileName, _FileExt, _FileSize);
                        //    }
                        //}
                        UploadCustomerFile(file[i], int.Parse(collection["hdfCustomerId"].ToString()), ftlID);
                    }
                }
                ViewBag.Message = "File Uploaded Successfully!!";
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }
            //return RedirectToAction("PendingApprovalList", "Customer", new { area = "Portal", @id = 4 });
            //return RedirectToAction("MaintenanceStepFifth", "Customer", new { area = "Portal", id = Convert.ToInt32(collection["hdfCustomerId"].ToString()) });
            return RedirectToAction("Index", "CRMRegionOperation", new { area = "CRM" });

        }

        [HttpPost]
        public ActionResult PendingCustomerUploadDocument(FormCollection collection, HttpPostedFileBase[] file)
        {
            try
            {
                string[] strFTL = collection["hdfFiletypeListIds"].ToString().Split(',');

                int len = strFTL.Length;
                for (int i = 0; i < len; i++)
                {
                    if (strFTL[i] != "")
                    {


                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString()));
                        //}
                        //if (file[i] != null)
                        //{
                        //    if (file[i].ContentLength > 0)
                        //    {
                        //        string _FileName = Path.GetFileName(file[i].FileName);

                        //        int _FileSize = Path.GetFileName(file[i].FileName).Length;
                        //        string _FileExt = Path.GetFileName(file[i].FileName).Split('.').Last();
                        //        string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                        //        string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), collection["hdfCustomerId"].ToString(), _SFileName);
                        //        file[i].SaveAs(_path);

                        //        String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + collection["hdfCustomerId"].ToString() + "/" + _SFileName;

                        //        CustomerService.SaveUploadDocument(int.Parse(collection["hdfCustomerId"].ToString()), 1, ftlID, _FilePath, _FileName, _FileExt, _FileSize);
                        //    }
                        //}

                        int FileTypeId = Convert.ToInt32(strFTL[i].Replace("Other_", ""));

                        string File_Title = string.Empty;
                        if (Request["txtDocName_" + Convert.ToInt32(FileTypeId)] != null)
                            File_Title = Request["txtDocName_" + Convert.ToInt32(FileTypeId)];

                        UploadCustomerFile(file[i], int.Parse(collection["hdfCustomerId"].ToString()), FileTypeId, File_Title);
                    }
                }
                ViewBag.Message = "File Uploaded Successfully!!";
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }
            return RedirectToAction("PendingApprovalList", "Customer", new { area = "Portal", id = 4 });
        }

        [HttpPost]
        public ActionResult SaveCustomerUploadDocumentPopup(FormCollection collection)
        {
            string selIds = Request["selIds"];
            string CustomerId = Request["CustomerId"];
            if (selIds != "")
            {
                string[] arrIds = selIds.Split(',');
                for (int i = 0; i < arrIds.Length; i++)
                {
                    if (Request.Files.Count > 0)
                    {

                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));
                        //}
                        //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString())))
                        //{
                        //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString()));
                        //}
                        //HttpFileCollectionBase files = Request.Files;
                        //HttpPostedFileBase file = files[i];
                        //string DocumentId = arrIds[i];

                        //if (file != null)
                        //{
                        //    if (file.ContentLength > 0)
                        //    {
                        //        string _FileName = Path.GetFileName(file.FileName);

                        //        int _FileSize = Path.GetFileName(file.FileName).Length;
                        //        string _FileExt = Path.GetFileName(file.FileName).Split('.').Last();
                        //        string _SFileName = DateTime.Now.ToString("MMddyyyyHHmmsstt") + "." + _FileExt;

                        //        string _path = Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString(), _SFileName);
                        //        file.SaveAs(_path);

                        //        String _FilePath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + CustomerId.ToString() + "/" + _SFileName;

                        //        CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), (int)JKApi.Business.Enumeration.TypeList.Customer, Convert.ToInt32(DocumentId), _FilePath, _FileName, _FileExt, _FileSize);
                        //    }
                        //}
                        string File_Title = string.Empty;
                        int FileTypeId = Convert.ToInt32(arrIds[i].Replace("Other_", ""));

                        if (Request["txtDocName_" + Convert.ToInt32(FileTypeId)] != null)
                            File_Title = Request["txtDocName_" + Convert.ToInt32(FileTypeId)];

                        UploadCustomerFile(Request.Files[i], int.Parse(CustomerId.ToString()), Convert.ToInt32(FileTypeId), File_Title);
                    }
                }
                return Json(new
                {
                    success = true,
                    message = ""
                });
            }
            return Json(new
            {
                success = false,
                message = "There is no document to upload"
            });
        }

        public ActionResult FileView(int CustomerId)
        {
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument")))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument"));
            }
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));
            }

            List<FileViewModel> objFileViewModel = new List<FileViewModel>();
            //string[] fileList = Directory.GetFiles(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));

            foreach (FileInfo flInfo in dir.GetFiles())
            {
                objFileViewModel.Add(new FileViewModel() { Path = flInfo.FullName.Replace(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), @"\docs"), FileName = flInfo.Name });
            }

            return Json(new { fileList = objFileViewModel }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FranchiseFileView(int FranchiseId)
        {
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument")))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument"));
            }
            if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", FranchiseId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", FranchiseId.ToString()));
            }

            List<FileViewModel> objFileViewModel = new List<FileViewModel>();
            //string[] fileList = Directory.GetFiles(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "CustomerDocument", CustomerId.ToString()));
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), "FranchiseDocument", FranchiseId.ToString()));

            foreach (FileInfo flInfo in dir.GetFiles())
            {
                objFileViewModel.Add(new FileViewModel() { Path = flInfo.FullName.Replace(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), @"\docs"), FileName = flInfo.Name });
            }

            return Json(new { fileList = objFileViewModel }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RemoveUploadDocument(int Id)
        {
            if (Id > 0)
            {
                var getData = CustomerService.GetUploadDocumentById(Id);
                if (getData != null && getData.UploadDocumentId > 0 && getData.FilePath != null)
                {
                    if (Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), getData.FilePath)))
                    {
                        Directory.Delete(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"].ToString(), getData.FilePath));
                    }
                }
                CustomerService.DeleteUploadDocument(Id);
            }
            return Json(new { success = true, document = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaintenanceStepFifth(int id = -1)
        {
            int? CustomerID = id;
            ViewBag.CustomerID = id;
            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1(); ;
            if (id > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());
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
                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }
            return View(FullCustomerViewModel);
        }

        [HttpPost]
        public ActionResult Maintenance(FullCustomerViewModel FullCustomerViewModel, FormCollection collection)
        {
            if (FullCustomerViewModel.CustomerViewModel != null)
            {
                FullCustomerViewModel.CustomerViewModel.Name = FullCustomerViewModel.MainContact.FirstName;
                String guid = Guid.NewGuid().ToString();
                FullCustomerViewModel.CustomerViewModel.CustomerNo = guid;
                FullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(FullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                FullCustomerViewModel.MainContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.MainContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.MainContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.MainAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                if (collection["MainStatesList"] != null)
                {
                    FullCustomerViewModel.MainAddress.StateListId = Convert.ToInt32(collection["MainStatesList"]);
                }
                FullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.MainPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.MainEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                FullCustomerViewModel.BillingContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel.BillingAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                if (collection["BillingStatesList"] != null)
                {
                    FullCustomerViewModel.BillingAddress.StateListId = Convert.ToInt32(collection["BillingStatesList"]);
                }

                FullCustomerViewModel.ContactInformation.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.ContactInformation.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.ContactInformation.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.ContactInformationAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.ContactInformationAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.ContactInformationAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.ContactInformationPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.ContactInformationPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.ContactInformationPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel.ContactInformationEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.ContactInformationEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.ContactInformationEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                FullCustomerViewModel.BillingInformation.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingInformation.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingInformation.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel.BillingInformationAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingInformationAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingInformationAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel.BillingInformationPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingInformationPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingInformationPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel.BillingInformationEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                FullCustomerViewModel.BillingInformationEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel.BillingInformationEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                FullCustomerViewModel.MainContact = CustomerService.SaveContact(FullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
                FullCustomerViewModel.MainAddress = CustomerService.SaveAddress(FullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullCustomerViewModel.MainPhone = CustomerService.SavePhone(FullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullCustomerViewModel.MainEmail = CustomerService.SaveEmail(FullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullCustomerViewModel.BillingContact = CustomerService.SaveContact(FullCustomerViewModel.BillingContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
                FullCustomerViewModel.BillingAddress = CustomerService.SaveAddress(FullCustomerViewModel.BillingAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullCustomerViewModel.ContactInformation = CustomerService.SaveContact(FullCustomerViewModel.ContactInformation.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
                FullCustomerViewModel.ContactInformationAddress = CustomerService.SaveAddress(FullCustomerViewModel.ContactInformationAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullCustomerViewModel.ContactInformationPhone = CustomerService.SavePhone(FullCustomerViewModel.ContactInformationPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullCustomerViewModel.ContactInformationEmail = CustomerService.SaveEmail(FullCustomerViewModel.ContactInformationEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                FullCustomerViewModel.BillingInformation = CustomerService.SaveContact(FullCustomerViewModel.BillingInformation.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();
                FullCustomerViewModel.BillingInformationAddress = CustomerService.SaveAddress(FullCustomerViewModel.BillingInformationAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                FullCustomerViewModel.BillingInformationPhone = CustomerService.SavePhone(FullCustomerViewModel.BillingInformationPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
                FullCustomerViewModel.BillingInformationEmail = CustomerService.SaveEmail(FullCustomerViewModel.BillingInformationEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                return RedirectToAction("SearchList", "Customer");
            }

            return null;
        }

        [HttpPost]
        public ActionResult MaintenanceStepOne(FullCustomerViewModel FullCustomerViewModel, FormCollection collection)
        {
            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            if (collection["CustomerViewModel.CustomerId"] == null)
            {
                if (FullCustomerViewModel.CustomerViewModel != null)
                {
                    //Check Customer IsExist
                    //if (CustomerService.CheckCustomerIsExist(FullCustomerViewModel.CustomerViewModel.Name))
                    //{
                    //    TempData["IsExist"] = 1;
                    //    return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", searchin = 1, status = 1 });
                    //}

                    //Customer Details
                    FullCustomerViewModel.CustomerViewModel.Name = FullCustomerViewModel.CustomerViewModel.Name;
                    FullCustomerViewModel.CustomerViewModel.CustomerNo = GetCustomerNo();
                    FullCustomerViewModel.CustomerViewModel.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.CustomerViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.CustomerViewModel.ParentId = -1;
                    FullCustomerViewModel.CustomerViewModel.IsActive = true;
                    FullCustomerViewModel.CustomerViewModel.RegionId = SelectedRegionId;
                    FullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(FullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    UpdateCustomerIndex();

                    //Main Information
                    FullCustomerViewModel.MainContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainContact.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainContact.IsActive = true;
                    FullCustomerViewModel.MainContact.CreatedBy = _claimView.GetCLAIM_USERID();

                    FullCustomerViewModel.MainContact = CustomerService.SaveContact(FullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.MainAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    if (collection["MainStatesList"] != null)
                    {
                        FullCustomerViewModel.MainAddress.StateListId = Convert.ToInt32(collection["MainStatesList"]);
                    }
                    if (collection["MainStatesList"] != null)
                    {
                        int id = Convert.ToInt32(collection["MainStatesList"]);
                        string state = CustomerService.GetStatesName(id);
                        FullCustomerViewModel.MainAddress.StateName = state.Trim();
                    }
                    FullCustomerViewModel.MainAddress.IsActive = true;
                    FullCustomerViewModel.MainAddress.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel.MainAddress.FullAddress));
                    if (_latlng.results.Count() > 0)
                    {
                        FullCustomerViewModel.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                        FullCustomerViewModel.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                    }

                    FullCustomerViewModel.MainAddress = CustomerService.SaveAddress(FullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                    FullCustomerViewModel.MainPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                    FullCustomerViewModel.MainPhone.Phone1 = FullCustomerViewModel.MainPhone.Phone1;
                    FullCustomerViewModel.MainPhone.IsActive = true;
                    FullCustomerViewModel.MainPhone.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainPhone = CustomerService.SavePhone(FullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.MainPhone2.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainPhone2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainPhone2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainPhone2.Phone1 = FullCustomerViewModel.MainPhone2.Phone1;
                    FullCustomerViewModel.MainPhone2.IsActive = true;
                    FullCustomerViewModel.MainPhone2.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainPhone2.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainPhone2 = CustomerService.SavePhone(FullCustomerViewModel.MainPhone2.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.MainEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainEmail.IsActive = true;
                    FullCustomerViewModel.MainEmail.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.MainEmail = CustomerService.SaveEmail(FullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                    // Billing Information

                    FullCustomerViewModel.BillingContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                    FullCustomerViewModel.BillingContact.IsActive = true;
                    FullCustomerViewModel.BillingContact.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingContact.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingContact = CustomerService.SaveContact(FullCustomerViewModel.BillingContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.BillingAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                    if (collection["BillingStatesList"] != null)
                    {
                        FullCustomerViewModel.BillingAddress.StateListId = Convert.ToInt32(collection["BillingStatesList"]);
                    }
                    if (collection["BillingStatesList"] != null)
                    {
                        int id = Convert.ToInt32(collection["BillingStatesList"]);
                        string state = CustomerService.GetStatesName(id);
                        FullCustomerViewModel.BillingAddress.StateName = state.Trim();
                    }
                    FullCustomerViewModel.BillingAddress.IsActive = true;
                    FullCustomerViewModel.BillingAddress.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                    var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel.BillingAddress.FullAddress));
                    if (_latlngBL.results.Count() > 0)
                    {
                        FullCustomerViewModel.BillingAddress.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                        FullCustomerViewModel.BillingAddress.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
                    }
                    FullCustomerViewModel.BillingAddress = CustomerService.SaveAddress(FullCustomerViewModel.BillingAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                    FullCustomerViewModel.BillingContactInformation2.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingContactInformation2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingContactInformation2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                    FullCustomerViewModel.BillingContactInformation2.IsActive = true;
                    FullCustomerViewModel.BillingContactInformation2.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingContactInformation2.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingContactInformation2 = CustomerService.SaveContact(FullCustomerViewModel.BillingContactInformation2.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.BillingPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                    FullCustomerViewModel.BillingPhone.IsActive = true;
                    FullCustomerViewModel.BillingPhone.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.BillingPhone = CustomerService.SavePhone(FullCustomerViewModel.BillingPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.BillingEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                    FullCustomerViewModel.BillingEmail.IsActive = true;
                    FullCustomerViewModel.BillingEmail.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingEmail = CustomerService.SaveEmail(FullCustomerViewModel.BillingEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                    string tempEBillEmails = FullCustomerViewModel.EBill_Emails;

                    if (tempEBillEmails != "")
                    {
                        EmailViewModel oEmailViewModel;
                        foreach (string o in tempEBillEmails.Split(',').Distinct())
                        {
                            oEmailViewModel = new EmailViewModel();
                            oEmailViewModel.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                            oEmailViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                            oEmailViewModel.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.EBillEmail;
                            oEmailViewModel.IsActive = true;
                            oEmailViewModel.CreatedDate = DateTime.Now;
                            oEmailViewModel.CreatedBy = _claimView.GetCLAIM_USERID();
                            oEmailViewModel.EmailAddress = o;
                            oEmailViewModel.EmailId = 0;
                            oEmailViewModel = CustomerService.SaveEBill_Emails(oEmailViewModel.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                        }
                    }

                    var States = CustomerService.GetStateList();

                    ViewBag.StateAbbr = new SelectList(States, "StateListId", "Name");
                    ViewBag.BillingState = new SelectList(States, "StateListId", "Name");

                    if (FullCustomerViewModel != null && FullCustomerViewModel.MainAddress != null && FullCustomerViewModel.MainAddress.StateListId != null)
                    {
                        ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name", FullCustomerViewModel.MainAddress.StateListId);
                    }
                    else
                    {
                        ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name");
                    }
                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillingAddress != null && FullCustomerViewModel.BillingAddress.StateListId != null)
                    {
                        ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name", FullCustomerViewModel.BillingAddress.StateListId);
                    }
                    else
                    {
                        ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name");
                    }

                    //Save Bill Setting
                    if (collection["ArStatus"] != null)
                    {
                        FullCustomerViewModel.BillSetting.ARStatusListId = Convert.ToInt32(collection["ArStatus"]);
                    }

                    if (collection["InvoiceDate"] != null)
                    {
                        FullCustomerViewModel.BillSetting.InvoiceDateListId = Convert.ToInt32(collection["InvoiceDate"]);
                    }

                    if (collection["TermDate"] != null)
                    {
                        FullCustomerViewModel.BillSetting.InvoiceTermListId = Convert.ToInt32(collection["TermDate"]);
                    }
                    FullCustomerViewModel.BillSetting.CustomerId = Convert.ToInt32(FullCustomerViewModel.CustomerViewModel.CustomerId);
                    FullCustomerViewModel.BillSetting.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillSetting = CustomerService.SaveBillSetting(FullCustomerViewModel.BillSetting.ToModel<BillSetting, BillSettingViewModel>()).ToModel<BillSettingViewModel, BillSetting>();
                    ViewBag.MainStatesList = getUsStatesList();
                    ViewBag.BillingStatesList = getUsStatesList();
                    ViewBag.TaxAuthority = getTaxAuthority();

                    FullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(FullCustomerViewModel.CustomerViewModel.CustomerId));
                    if (FullCustomerViewModel != null && FullCustomerViewModel.MainAddress != null)
                    {
                        string Address = string.Empty;
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.Address1))
                        {
                            Address = FullCustomerViewModel.MainAddress.Address1;
                        }
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.City))
                        {
                            Address += " " + FullCustomerViewModel.MainAddress.City;
                        }
                        if (FullCustomerViewModel.MainAddress.StateListId != null && FullCustomerViewModel.MainAddress.StateListId > 0)
                        {
                            var States1 = CustomerService.GetStateList();
                            string StateName = States1.Where(one => one.StateListId == FullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                            Address += " " + StateName;
                        }
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.PostalCode))
                        {
                            Address += " " + FullCustomerViewModel.MainAddress.PostalCode;
                        }
                        ViewBag.Address = Address;
                    }

                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.ARStatusListId != null)
                    {
                        ViewBag.ArStatus = new SelectList(getARStatusList(), "Id", "Name", FullCustomerViewModel.BillSetting.ARStatusListId);
                    }
                    else
                    {
                        ViewBag.ArStatus = new SelectList(getARStatusList(), "Id", "Name");
                    }
                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.InvoiceDateListId != null)
                    {
                        ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term", FullCustomerViewModel.BillSetting.InvoiceDateListId);
                    }
                    else
                    {
                        ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term");
                    }

                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.InvoiceTermListId != null)
                    {
                        ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", FullCustomerViewModel.BillSetting.InvoiceTermListId);
                    }
                    else
                    {
                        ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
                    }

                    //if (FullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
                    if (Convert.ToInt32(collection["ButtonType"]) == 1)
                    {
                        return View(FullCustomerViewModel);
                    }
                    //else if (FullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
                    else if (Convert.ToInt32(collection["ButtonType"]) == 2)
                    {
                        return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = FullCustomerViewModel.CustomerViewModel.CustomerId });
                    }
                    return View(FullCustomerViewModel);
                }
            }

            {
                if (FullCustomerViewModel.CustomerViewModel != null)
                {
                    // Check Customer IsExist
                    //if (CustomerService.CheckCustomerIsExist(FullCustomerViewModel.CustomerViewModel.Name))
                    //{
                    //    TempData["IsExist"] = 1;
                    //    return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", searchin = 1, status = 1 });
                    //}
                    //Customer Details
                    FullCustomerViewModel.CustomerViewModel.CustomerId = Convert.ToInt32(collection["CustomerViewModel.CustomerId"]);
                    FullCustomerViewModel.CustomerViewModel.Name = FullCustomerViewModel.CustomerViewModel.Name;
                    FullCustomerViewModel.CustomerViewModel.CustomerNo = GetCustomerNo();
                    FullCustomerViewModel.CustomerViewModel.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.CustomerViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID()); ;
                    FullCustomerViewModel.CustomerViewModel.ParentId = -1;
                    FullCustomerViewModel.CustomerViewModel.IsActive = true;
                    FullCustomerViewModel.CustomerViewModel.RegionId = SelectedRegionId;
                    FullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(FullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    UpdateCustomerIndex();

                    //Main Information
                    FullCustomerViewModel.MainContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainContact.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainContact.IsActive = true;
                    FullCustomerViewModel.MainContact.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.MainContact = CustomerService.SaveContact(FullCustomerViewModel.MainContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.MainAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    if (collection["MainStatesList"] != null)
                    {
                        FullCustomerViewModel.MainAddress.StateListId = Convert.ToInt32(collection["MainStatesList"]);
                    }
                    if (collection["MainStatesList"] != null)
                    {
                        int id = Convert.ToInt32(collection["MainStatesList"]);
                        string state = CustomerService.GetStatesName(id);
                        FullCustomerViewModel.MainAddress.StateName = state.Trim();
                    }
                    FullCustomerViewModel.MainAddress.IsActive = true;
                    FullCustomerViewModel.MainAddress.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel.MainAddress.FullAddress));
                    FullCustomerViewModel.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullCustomerViewModel.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());


                    FullCustomerViewModel.MainAddress = CustomerService.SaveAddress(FullCustomerViewModel.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                    FullCustomerViewModel.MainPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                    FullCustomerViewModel.MainPhone.Cell = collection["ContactInformationPhone.Cell"];
                    FullCustomerViewModel.MainPhone.PhoneExt = collection["ContactInformationPhone.PhoneExt"];
                    FullCustomerViewModel.MainPhone.IsActive = true;
                    FullCustomerViewModel.MainPhone.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainPhone = CustomerService.SavePhone(FullCustomerViewModel.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.MainPhone2.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainPhone2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainPhone2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainPhone2.Phone1 = FullCustomerViewModel.MainPhone2.Phone1;
                    FullCustomerViewModel.MainPhone2.IsActive = true;
                    FullCustomerViewModel.MainPhone2.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainPhone2.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.MainPhone2 = CustomerService.SavePhone(FullCustomerViewModel.MainPhone2.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.MainEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.MainEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.MainEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                    FullCustomerViewModel.MainEmail.IsActive = true;
                    FullCustomerViewModel.MainEmail.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.MainEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.MainEmail = CustomerService.SaveEmail(FullCustomerViewModel.MainEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                    // Billing Information

                    FullCustomerViewModel.BillingContact.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                    FullCustomerViewModel.BillingContact.IsActive = true;
                    FullCustomerViewModel.BillingContact.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingContact.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingContact = CustomerService.SaveContact(FullCustomerViewModel.BillingContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.BillingAddress.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                    if (collection["BillingStatesList"] != null)
                    {
                        FullCustomerViewModel.BillingAddress.StateListId = Convert.ToInt32(collection["BillingStatesList"]);
                    }
                    if (collection["BillingStatesList"] != null)
                    {
                        int id = Convert.ToInt32(collection["BillingStatesList"]);
                        string state = CustomerService.GetStatesName(id);
                        FullCustomerViewModel.BillingAddress.StateName = state.Trim();
                    }

                    FullCustomerViewModel.BillingAddress.IsActive = true;
                    FullCustomerViewModel.BillingAddress.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                    var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel.BillingAddress.FullAddress));
                    if (_latlngBL.results.Count() > 0)
                    {
                        FullCustomerViewModel.BillingAddress.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                        FullCustomerViewModel.BillingAddress.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
                    }
                    FullCustomerViewModel.BillingAddress = CustomerService.SaveAddress(FullCustomerViewModel.BillingAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                    FullCustomerViewModel.BillingContactInformation2.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingContactInformation2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingContactInformation2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                    FullCustomerViewModel.BillingContactInformation2.IsActive = true;
                    FullCustomerViewModel.BillingContactInformation2.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingContactInformation2.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingContactInformation2 = CustomerService.SaveContact(FullCustomerViewModel.BillingContactInformation2.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                    FullCustomerViewModel.BillingPhone.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                    FullCustomerViewModel.BillingPhone.IsActive = true;
                    FullCustomerViewModel.BillingPhone.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                    FullCustomerViewModel.BillingPhone = CustomerService.SavePhone(FullCustomerViewModel.BillingPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                    FullCustomerViewModel.BillingEmail.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                    FullCustomerViewModel.BillingEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    FullCustomerViewModel.BillingEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                    FullCustomerViewModel.BillingEmail.IsActive = true;
                    FullCustomerViewModel.BillingEmail.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillingEmail.CreatedBy = _claimView.GetCLAIM_USERID();
                    FullCustomerViewModel.BillingEmail = CustomerService.SaveEmail(FullCustomerViewModel.BillingEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();

                    string tempEBillEmails = FullCustomerViewModel.EBill_Emails;

                    if (tempEBillEmails != "" && tempEBillEmails != null)
                    {
                        EmailViewModel oEmailViewModel;
                        foreach (string o in tempEBillEmails.Split(',').Distinct())
                        {
                            oEmailViewModel = new EmailViewModel();
                            oEmailViewModel.ClassId = FullCustomerViewModel.CustomerViewModel.CustomerId;
                            oEmailViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                            oEmailViewModel.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.EBillEmail;
                            oEmailViewModel.IsActive = true;
                            oEmailViewModel.CreatedDate = DateTime.Now;
                            oEmailViewModel.CreatedBy = _claimView.GetCLAIM_USERID();
                            oEmailViewModel.EmailAddress = o;
                            oEmailViewModel.EmailId = 0;
                            oEmailViewModel = CustomerService.SaveEBill_Emails(oEmailViewModel.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                        }
                    }

                    //return RedirectToAction("SearchList", "Customer");
                    var States = CustomerService.GetStateList();

                    ViewBag.StateAbbr = new SelectList(States, "StateListId", "Name");
                    ViewBag.BillingState = new SelectList(States, "StateListId", "Name");

                    if (FullCustomerViewModel != null && FullCustomerViewModel.MainAddress != null && FullCustomerViewModel.MainAddress.StateListId != null)
                    {
                        ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name", FullCustomerViewModel.MainAddress.StateListId);
                    }
                    else
                    {
                        ViewBag.MainStatesList = new SelectList(States, "StateListId", "Name");
                    }
                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillingAddress != null && FullCustomerViewModel.BillingAddress.StateListId != null)
                    {
                        ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name", FullCustomerViewModel.BillingAddress.StateListId);
                    }
                    else
                    {
                        ViewBag.BillingStatesList = new SelectList(States, "StateListId", "Name");
                    }

                    //Save Bill Setting
                    if (collection["ArStatus"] != null)
                    {
                        FullCustomerViewModel.BillSetting.ARStatusListId = Convert.ToInt32(collection["ArStatus"]);
                        FullCustomerViewModel.BillSetting.ARStatus = Convert.ToInt32(collection["ArStatus"]);
                    }

                    if (collection["InvoiceDate"] != null)
                    {
                        FullCustomerViewModel.BillSetting.InvoiceDateListId = Convert.ToInt32(collection["InvoiceDate"]);
                    }

                    if (collection["TermDate"] != null)
                    {
                        FullCustomerViewModel.BillSetting.InvoiceTermListId = Convert.ToInt32(collection["TermDate"]);
                    }
                    FullCustomerViewModel.BillSetting.CustomerId = Convert.ToInt32(FullCustomerViewModel.CustomerViewModel.CustomerId);
                    FullCustomerViewModel.BillSetting.CreatedDate = DateTime.Now;
                    FullCustomerViewModel.BillSetting.CreatedBy = 1;
                    FullCustomerViewModel.BillSetting.IsActive = 1;
                    FullCustomerViewModel.BillSetting = CustomerService.SaveBillSetting(FullCustomerViewModel.BillSetting.ToModel<BillSetting, BillSettingViewModel>()).ToModel<BillSettingViewModel, BillSetting>();
                    if (FullCustomerViewModel.BillSetting != null)
                    {
                        //ARStatu modelARStatu = new ARStatu();
                        //modelARStatu.BillSettingsId = FullCustomerViewModel.BillSetting.BillSettingsId;
                        //modelARStatu.ARStatusReasonListId = Convert.ToInt32(FullCustomerViewModel.BillSetting.ARStatusListId != 0 ? FullCustomerViewModel.BillSetting.ARStatusListId : 0);
                        //modelARStatu.ARStatusDate = DateTime.Now;
                        //modelARStatu.isActive = 1;
                        //CustomerService.AddNewARStatuOldInactive(modelARStatu);
                    }

                    ViewBag.MainStatesList = getUsStatesList();
                    ViewBag.BillingStatesList = getUsStatesList();
                    ViewBag.TaxAuthority = getTaxAuthority();

                    FullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(FullCustomerViewModel.CustomerViewModel.CustomerId));
                    if (FullCustomerViewModel != null && FullCustomerViewModel.MainAddress != null)
                    {
                        string Address = string.Empty;
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.Address1))
                        {
                            Address = FullCustomerViewModel.MainAddress.Address1;
                        }
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.City))
                        {
                            Address += " " + FullCustomerViewModel.MainAddress.City;
                        }
                        if (FullCustomerViewModel.MainAddress.StateListId != null && FullCustomerViewModel.MainAddress.StateListId > 0)
                        {
                            var States1 = CustomerService.GetStateList();
                            string StateName = States1.Where(one => one.StateListId == FullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                            Address += " " + StateName;
                        }
                        if (!string.IsNullOrEmpty(FullCustomerViewModel.MainAddress.PostalCode))
                        {
                            Address += " " + FullCustomerViewModel.MainAddress.PostalCode;
                        }
                        ViewBag.Address = Address;
                    }

                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.ARStatusListId != null)
                    {
                        ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", FullCustomerViewModel.BillSetting.ARStatusListId);
                    }
                    else
                    {
                        ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                    }
                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.InvoiceDateListId != null)
                    {
                        ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term", FullCustomerViewModel.BillSetting.InvoiceDateListId);
                    }
                    else
                    {
                        ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term");
                    }

                    if (FullCustomerViewModel != null && FullCustomerViewModel.BillSetting != null && FullCustomerViewModel.BillSetting.InvoiceTermListId != null)
                    {
                        ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", FullCustomerViewModel.BillSetting.InvoiceTermListId);
                    }
                    else
                    {
                        ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
                    }

                    //if (FullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
                    if (Convert.ToInt32(collection["ButtonType"]) == 1)
                    {
                        return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = FullCustomerViewModel.CustomerViewModel.CustomerId });
                    }
                    //else if (FullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
                    else if (Convert.ToInt32(collection["ButtonType"]) == 2)
                    {
                        return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = FullCustomerViewModel.CustomerViewModel.CustomerId });
                    }
                    return View(FullCustomerViewModel);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult MaintenanceStepTwo(FullCustomerViewModel fullCustomerViewModel, FormCollection collection)
        {
            int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
            if (fullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }

            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "General");
            if (fullCustomerViewModel.BillingContact.ClassId < 1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullCustomerViewModel != null)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }) + "/" + fullCustomerViewModel.CustomerViewModel.CustomerId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            if (collection["ArStatus"] != null)
            {
                fullCustomerViewModel.BillSetting.ARStatusListId = Convert.ToInt32(collection["ArStatus"]);
            }

            if (collection["InvoiceDate"] != null)
            {
                fullCustomerViewModel.BillSetting.InvoiceDateListId = Convert.ToInt32(collection["InvoiceDate"]);
            }

            if (collection["TermDate"] != null)
            {
                fullCustomerViewModel.BillSetting.InvoiceTermListId = Convert.ToInt32(collection["TermDate"]);
            }

            fullCustomerViewModel.BillSetting = CustomerService.SaveBillSetting(fullCustomerViewModel.BillSetting.ToModel<BillSetting, BillSettingViewModel>()).ToModel<BillSettingViewModel, BillSetting>();

            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();

            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));
            if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
            {
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
                {
                    Address = fullCustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.City;
                }
                if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address;
            }

            if (fullCustomerViewModel != null && fullCustomerViewModel.BillSetting != null && fullCustomerViewModel.BillSetting.ARStatusListId != null)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", fullCustomerViewModel.BillSetting.ARStatusListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (fullCustomerViewModel != null && fullCustomerViewModel.BillSetting != null && fullCustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term", fullCustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term");
            }

            if (fullCustomerViewModel != null && fullCustomerViewModel.BillSetting != null && fullCustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", fullCustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                return View(fullCustomerViewModel);
            }
            else if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            return View(fullCustomerViewModel);
        }

        [HttpPost]
        public ActionResult MaintenanceStepThree(FullCustomerViewModel fullCustomerViewModel, FormCollection collection)
        {
            ViewBag.BillingStatesList = getUsStatesList();
            int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
            if (fullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("CustomerUploadDocument", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "General");
            if (fullCustomerViewModel.BillingContact.ClassId < 1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullCustomerViewModel != null)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }) + "/" + fullCustomerViewModel.CustomerViewModel.CustomerId, "Manage");
                // return View(serv.GetFranchiseForManage(id));
            }

            fullCustomerViewModel.ContractAddress.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            fullCustomerViewModel.ContractAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            fullCustomerViewModel.ContractAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            if (collection["ContractStatesList"] != null)
            {
                fullCustomerViewModel.ContractAddress.StateListId = Convert.ToInt32(collection["ContractStatesList"]);
            }
            if (collection["ContractStatesList"] != null)
            {
                int id = Convert.ToInt32(collection["ContractStatesList"]);
                string state = CustomerService.GetStatesName(id);
                fullCustomerViewModel.ContractAddress.StateName = state.Trim();
            }
            fullCustomerViewModel.ContractAddress.IsActive = true;
            fullCustomerViewModel.ContractAddress.CreatedDate = DateTime.Now;
            fullCustomerViewModel.ContractAddress.CreatedBy = LoginUserId;
            var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(fullCustomerViewModel.ContractAddress.FullAddress));
            if (_latlngBL.results.Count() > 0)
            {
                fullCustomerViewModel.ContractAddress.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                fullCustomerViewModel.ContractAddress.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
            }
            fullCustomerViewModel.ContractAddress = CustomerService.SaveAddress(fullCustomerViewModel.ContractAddress.ToModel<Address, AddressViewModel>
                ()).ToModel<AddressViewModel, Address>();

            if (collection["ContractStatusList"] != null)
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractStatusList"]);
            }

            if (collection["ContractServiceTypeList"] != null)
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractServiceTypeList"]);
            }

            if (collection["ContractTypeList"] != null)
            {
                fullCustomerViewModel.Contract.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            }

            if (collection["AccountTypeList"] != null)
            {
                fullCustomerViewModel.Contract.AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]);
            }
            if (collection["AgreementTypeList"] != null)
            {
                fullCustomerViewModel.Contract.AgreementTypeListId = Convert.ToInt32(collection["AgreementTypeList"]);
            }
            fullCustomerViewModel.Contract.CustomerId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            //fullCustomerViewModel.Contract.ContractTermListId = fullCustomerViewModel.Contract.ContractTermListId;
            fullCustomerViewModel.Contract.ContractTermMonth = fullCustomerViewModel.Contract.ContractTermMonth;
            fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
            fullCustomerViewModel.Contract.isActive = true;
            fullCustomerViewModel.Contract.CreatedBy = LoginUserId;
            if (fullCustomerViewModel.ContractAddress.AddressId != 0)
            {
                fullCustomerViewModel.Contract.AddressId = fullCustomerViewModel.ContractAddress.AddressId;
            }

            fullCustomerViewModel.Contract.StatusId = Convert.ToInt32(collection["CustomerViewModel.StatusId"]);
            //if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            //{
            //    fullCustomerViewModel.Contract.StatusListId = 4;
            //}
            //else
            {
                fullCustomerViewModel.Contract.StatusListId = Convert.ToInt32(collection["CustomerViewModel.StatusListId"]);
            }
            fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();
            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));

            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));
            //if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            //{
            //    fullCustomerViewModel.CustomerViewModel.StatusListId = 4;
            //    fullCustomerViewModel.CustomerViewModel.RegionId = SelectedRegionId;
            //    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(fullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
            //}
            //else if (fullCustomerViewModel.Contract.ContractTypeListId == 2)
            //{
            //    fullCustomerViewModel.CustomerViewModel.StatusListId = 4;
            //    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(fullCustomerViewModel.CustomerViewModel.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
            //}

            if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
            {
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
                {
                    Address = fullCustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.City;
                }
                if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address;
            }

            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", fullCustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            //  var ContractServiceTypeList = CustomerService.GetContractServiceTypeList().ToList();
            //if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractServiceTypeListId != null)
            //{
            //    ViewBag.ContractServiceTypeList = new SelectList(ContractServiceTypeList, "ContractServiceTypeListId", "Name", fullCustomerViewModel.Contract.ContractServiceTypeListId);
            //}
            //else
            //{
            //    ViewBag.ContractServiceTypeList = new SelectList(ContractServiceTypeList, "ContractServiceTypeListId", "Name");
            //}

            var ContractStatusList = CustomerService.GetARStatusReasonList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractStatusReasonListId != null)
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", fullCustomerViewModel.Contract.ContractStatusReasonListId);
            }
            else
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            }

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", fullCustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }

            if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null)
            {
                fullCustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == fullCustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
            }

            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
            {
                TempData["IsSave"] = 1;
                //if (fullCustomerViewModel.CustomerViewModel.CustomerId > 0)
                //return RedirectToAction("CustomerUploadDocument", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                //return View(fullCustomerViewModel);
                // return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                //return RedirectToAction("SearchList", "Customer", new { area = "Portal" });
                //return RedirectToAction("PendingApprovalList", "Customer", new { area = "Portal", id = 4 });
                return RedirectToAction("Index", "CRMRegionOperation", new { area = "CRM" });
            }

            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                TempData["IsSave"] = 1;
                //if (fullCustomerViewModel.CustomerViewModel.CustomerId > 0)
                //return RedirectToAction("CustomerUploadDocument", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                //return View(fullCustomerViewModel);
                // return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                //return RedirectToAction("SearchList", "Customer", new { area = "Portal" });
                return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            else if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("CustomerUploadDocument", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            return View(fullCustomerViewModel);
        }

        //[HttpPost]
        //public ActionResult MaintenanceStepFour(FullCustomerViewModel fullCustomerViewModel, FormCollection collection)
        //{
        //    int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
        //    if (fullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
        //    {
        //        return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
        //    }

        //    ViewBag.CurrentMenu = "CustomerGeneral";

        //    ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
        //    BreadCrumb.Clear();
        //    BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
        //    BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "General");
        //    if (fullCustomerViewModel.BillingContact.ClassId < 1)
        //    {
        //        BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Add");
        //        // return View();
        //    }
        //    else if (fullCustomerViewModel != null)
        //    {
        //        BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }) + "/" + fullCustomerViewModel.CustomerViewModel.CustomerId, "Manage");
        //        // return View(serv.GetFranchiseForManage(id));
        //    }

        //    if (collection["FrequencyTypeList"] != null)
        //    {
        //        fullCustomerViewModel.ContractDetail.FrequencyListId = Convert.ToInt32(collection["FrequencyTypeList"]);
        //    }

        //    if (collection["AccountTypeList"] != null)
        //    {
        //        fullCustomerViewModel.ContractDetail.ServiceTypeListId = Convert.ToInt32(collection["AccountTypeList"]);
        //    }

        //    fullCustomerViewModel.ContractDetail.ContractId = fullCustomerViewModel.Contract.ContractId;
        //    fullCustomerViewModel.ContractDetail.CreatedDate = DateTime.Now;
        //    fullCustomerViewModel.ContractDetail = CustomerService.SaveContractDetail(fullCustomerViewModel.ContractDetail.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

        //    ViewBag.MainStatesList = getUsStatesList();
        //    ViewBag.BillingStatesList = getUsStatesList();
        //    ViewBag.TaxAuthority = getTaxAuthority();

        //    fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));

        //    if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null)
        //    {
        //        fullCustomerViewModel.ContractDetailDescription = CustomerService.GetContractDetailDescription().Where(one => one.ContractDetailId == fullCustomerViewModel.ContractDetail.ContractDetailId).MapEnumerable<ContractDetailDescriptionViewModel, ContractDetailDescription>();
        //    }
        //    if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
        //    {
        //        string Address = string.Empty;
        //        if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
        //        {
        //            Address = fullCustomerViewModel.MainAddress.Address1;
        //        }
        //        if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
        //        {
        //            Address += " " + fullCustomerViewModel.MainAddress.City;
        //        }
        //        if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
        //        {
        //            var States = CustomerService.GetStatesList();
        //            string StateName = States.Where(one => one.Id == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.Abbr).FirstOrDefault();
        //            Address += " " + StateName;
        //        }
        //        if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
        //        {
        //            Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
        //        }
        //        ViewBag.Address = Address;
        //    }

        //    var AccountTypeList = CustomerService.GetAccountTypeList().ToList();

        //    if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.AccountTypeListId != null)
        //    {
        //        ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", fullCustomerViewModel.Contract.AccountTypeListId);
        //    }
        //    else
        //    {
        //        ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
        //    }

        //    var FrequencyList = CustomerService.GetFrequencyList().ToList();
        //    if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null && fullCustomerViewModel.ContractDetail.FrequencyListId != null)
        //    {
        //        ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name", fullCustomerViewModel.ContractDetail.FrequencyListId);
        //    }
        //    else
        //    {
        //        ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name");
        //    }

        //    if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
        //    {
        //        return View(fullCustomerViewModel);
        //    }
        //    else if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Submit))
        //    {
        //        return RedirectToAction("SearchList", "Customer", new { area = "Portal" });
        //    }
        //    return View(fullCustomerViewModel);
        //}
        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = "All Transactions" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", list.SelectedValue);
        }

        private SelectList AddFirstItemInSelecetList(SelectList list, string optionalText)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = optionalText });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", list.SelectedValue);
        }

        public JsonResult hasExistManintenance(int cid)
        {
            return Json(new { ManintenanceId = CustomerService.hasExistManintenance(cid).Split('|')[0], MaintenanceTypeName = CustomerService.hasExistManintenance(cid).Split('|')[1] }, JsonRequestBehavior.AllowGet);
        }

        private CustomerDetailViewModel GetCustomerDetailViewModel(int? CustomerID)
        {
            var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

            CustomerDetailViewModel customerDetailViewModel = null;
            foreach (var a in response.ToList())
            {
                customerDetailViewModel = new CustomerDetailViewModel();
                customerDetailViewModel.StatusName = String.IsNullOrEmpty(a.StatusName.ToString()) ? String.Empty : a.StatusName.ToString();
                customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();
                customerDetailViewModel.RegionId = String.IsNullOrEmpty(a.RegionId.ToString()) ? String.Empty : a.RegionId.ToString();
                customerDetailViewModel.StatusDate = String.IsNullOrEmpty(a.StatusDate.ToString()) ? String.Empty : a.StatusDate.Value.ToString("dd-MM-yyyy");
                customerDetailViewModel.ResumeDate = String.IsNullOrEmpty(a.ResumeDate.ToString()) ? String.Empty : a.ResumeDate.Value.ToString("dd-MM-yyyy");
                customerDetailViewModel.MaintenanceId = String.IsNullOrEmpty(a.MaintenanceId.ToString()) ? 0 : int.Parse(a.MaintenanceId.ToString());
                customerDetailViewModel.MaintenanceTypeName = String.IsNullOrEmpty(a.MaintenanceTypeName) ? String.Empty : a.MaintenanceTypeName;
                customerDetailViewModel.ContractTypeList = String.IsNullOrEmpty(a.ContractType) ? String.Empty : a.ContractType;
                customerDetailViewModel.LogId = (int)a.LogId;
                customerDetailViewModel.LogMessage = a.LogMessage;
                customerDetailViewModel.LogMessageColor = a.LogMessageColor;

                if (a.Phone != null)
                {
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                }
                //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                //{
                //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                //}

                if (a.Fax != null)
                {
                    customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                }
                //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                //{
                //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                //}
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
                // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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

            return customerDetailViewModel;
        }

        [HttpGet]
        public ActionResult CustomerDetail(int id = -1)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerDetail", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CustomerDetail", "Customer", new { area = "Portal" }), "Customer Detail");
            if (id < 0)
            {
                dynamic GetUsStates = getUsStatesList();
                ViewBag.MainStatesList = GetUsStates;
                ViewBag.BillingStatesList = GetUsStates;
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            ViewBag.PaymentMethodList = new SelectList(AccountReceivableService.GetAll_PaymentMethodList(), "PaymentMethodListId", "Name");

            int? CustomerID = id;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().Where(x => x.CustomerDetailView == true).ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1();
            if (id > 0)
            {
                CustomerDetailViewModel customerDetailViewModel = GetCustomerDetailViewModel(CustomerID);
                customerDetailViewModel.Balance = AccountReceivableService.GetCustomerCreditBalance(Convert.ToInt32(CustomerID));
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopup(id);

                //foreach (EmailViewModel op in FullCustomerViewModel.lstEBillEmails)
                //{
                //    FullCustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                //}
                //if (FullCustomerViewModel.EBill_Emails.Length > 0)
                //    FullCustomerViewModel.EBill_Emails = FullCustomerViewModel.EBill_Emails.Trim(',');

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }
            else
            {
                //FullCustomerViewModel = new FullCustomerViewModel1();
                //FullCustomerViewModel.CustomerDetail = new CustomerDetailViewModel();
            }
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(id);
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }
            if (FullCustomerViewModel != null)
            {
                ViewBag.CustBillingEmail = (FullCustomerViewModel.BillingEmail != null ? FullCustomerViewModel.BillingEmail.EmailAddress : string.Empty);
            }
            else
            {
                ViewBag.CustBillingEmail = "";
            }
            return View(FullCustomerViewModel);
        }

        public JsonResult CustomerDetailTransactions(int CustomerId, int TypeId, DateTime StartDate, DateTime EndDate)
        {
            EndDate += new TimeSpan(23, 59, 59);
            return Json(new
            {
                StartingBalance = CustomerService.GetCustomerBalanceAsOfDate(CustomerId, StartDate),
                Transactions = CustomerService.GetCustomerDetailTransactions(CustomerId, TypeId, StartDate, EndDate)
            }, JsonRequestBehavior.AllowGet);
        }

        private CustomerStatementViewModel GetCustomerStatementViewModel(int customerId, DateTime start, DateTime? end = null)
        {
            CustomerStatementViewModel vm = new CustomerStatementViewModel();
            vm.CustomerDetail = GetCustomerDetailViewModel(customerId);
            vm.StartingBalance = CustomerService.GetCustomerBalanceAsOfDate(customerId, start);
            vm.Transactions = CustomerService.GetCustomerDetailTransactions(customerId, 0, start, end ?? DateTime.Now);
            vm.AsOfDate = start;

            int regionId = 0;
            if (Int32.TryParse(vm.CustomerDetail.RegionId, out regionId))
                vm.RemitTo = CustomerService.GetRemitToForRegion(regionId);

            return vm;
        }

        [HttpGet]
        public ActionResult CustomerStatement(int customerId, DateTime start, DateTime? end = null)
        {
            var vm = GetCustomerStatementViewModel(customerId, start, end);

            return View("_CustomerStatementExportToPDF", vm);
        }

        protected string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            ViewBag.DomainUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority + "/";

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext,
                                                                         viewName, "_Layout");
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public FileResult SelectedCustomerStatementPrint(string customerIds, string contentDisposition, DateTime start, DateTime? end = null)
        {
            string[] Ids = customerIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    var vm = GetCustomerStatementViewModel(Convert.ToInt32(item), start, end);
                    HTMLContent += RenderViewToString("_CustomerStatementExportToPDF", vm);
                }
                string filename = string.Format("{0}_CustomerStatement.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Response.AddHeader("content-disposition", contentDisposition + ";filename=\"" + filename + "\"");
                return File(GetCustomerStatementPDF(HTMLContent), "application/pdf");
            }
            return null;
        }
        public byte[] GetCustomerStatementPDF(string pHTML)
        {
            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                StyleSheet styles = new StyleSheet();
                styles.LoadStyle("t1col1", "border", "0.1");
                using (var document = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    document.SetPageSize(iTextSharp.text.PageSize.A4);
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
                            cssResolver.AddCssFile(
                                System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
                            //Export
                            IPipeline pipeline = new CssResolverPipeline(cssResolver,
                                new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
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





        public FileResult CustomerStatementExportToPDF(int customerId, DateTime start, DateTime? end = null)
        {
            return CustomerStatementExport("attachment", customerId, start, end);
        }

        public FileResult CustomerStatementPrint(int customerId, DateTime start, DateTime? end = null)
        {
            return CustomerStatementExport("inline", customerId, start, end);
        }

        private FileResult CustomerStatementExport(string contentDisposition, int customerId, DateTime start, DateTime? end = null)
        {
            var vm = GetCustomerStatementViewModel(customerId, start, end);

            return GetCustomerStatementPDF(vm, contentDisposition);
        }

        private FileResult GetCustomerStatementPDF(object viewModel, string contentDisposition)
        {
            string HTMLContent = RenderViewToString("_CustomerDetailStatementExportToPDF", viewModel);
            var doc = new Document(PageSize.A4, 25, 25, 25, 25);
            var memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;

            doc.Open();

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            //cssResolver.AddCssFile(Server.MapPath("~/Content/FranchiseeReport.css"), true);

            var pipeline = new CssResolverPipeline(cssResolver,
                                                   new HtmlPipeline(htmlContext, new PdfWriterPipeline(doc, writer)));

            var worker = new XMLWorker(pipeline, true);
            var parser = new XMLParser(worker);

            using (var sr = new StringReader(HTMLContent))
            {
                parser.Parse(sr);
            }

            doc.Close();

            var buf = new byte[memStream.Position];
            memStream.Position = 0;
            memStream.Read(buf, 0, buf.Length);

            string filename = string.Format("{0}_CustomerStatement.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Response.AddHeader("content-disposition", contentDisposition + ";filename=\"" + filename + "\"");

            return File(buf, "application/pdf");
        }


        [HttpGet]
        public JsonResult CustomerStatementSendEmailPopup(string FromEmail, string ToEmail, string CCEmail, string SubjectEmail, string BodyEmail, int customerId, DateTime start, DateTime? end = null)
        {
            string retVal = "";
            try
            {
                var viewModel = GetCustomerStatementViewModel(customerId, start, end);
                string HTMLContent = RenderViewToString("_CustomerDetailStatementExportToPDF", viewModel);

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress(FromEmail);

                mail.To.Add(ToEmail);
                if (CCEmail != "")
                    mail.CC.Add(CCEmail);
                mail.Subject = SubjectEmail;

                BodyEmail +=
                    "<p>To open the attached pdf, You need the free Adobe Reader software whitch can be here <a href='#'>Adobe</a></p>";
                BodyEmail += "<hr />";
                BodyEmail +=
                    "<p> This email is intended for the party listed in the 'Sold To' field of the attached invoice. Delivery of this email to anyone other than the party to which is was intended is unintentional. In the event this email was misdirected to a party other that the intended party, please notify the sender destroy this email.</p>";

                mail.Body = BodyEmail;
                mail.IsBodyHtml = true;
                mail.Attachments.Add(new Attachment(new MemoryStream(GetPDF(HTMLContent)),
                    "CUS- " + viewModel.CustomerDetail.CustomerNo + ".pdf"));
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("janikingtest@gmail.com", "Test#12345");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                retVal = "Success";
            }
            catch (Exception ex)
            {
                retVal = ex.Message;

                //throw ex;
            }
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CustomerDetailListByRegion(int RegionId)
        {
            int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var response = CustomerService.GetCustomerSearchList(RegionId, CustomerContactTypeList);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerMaintenanceDetail(int id)
        {
            return PartialView("_PartialCustomerMaintenance", AccountReceivableService.GetInvoiceDetail(id));
        }

        #region Customer Transfer

        [HttpGet]
        public ActionResult CustomerDetailTransferDistribution(int id)
        {
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.TStatusList.ToString()), "Value", "Text", 0);
            ViewBag.statusreasonlist = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerTransferStatusReasonList.ToString()), "Value", "Text", 0);
            var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
            ViewBag.YearList = yearList.ToList().OrderByDescending(x => x);
            return PartialView("_CustomerTransferDistribution");
        }

        [HttpGet]
        public ActionResult TransferRevenueDistribution(int Id, DateTime? EffectiveDate)
        {
            RevenueDistributionInvoiceDetailViewModel oRevenueDistributionInvoiceDetailViewModel = CustomerService.GetRevenueDistributionDetail(Id);

            //oRevenueDistributionInvoiceDetailViewModel.FranchiseeDistributionItems = oRevenueDistributionInvoiceDetailViewModel.FranchiseeDistributionItems.Where(d => d.DistributionId == null).ToList();

            DateTime INVDate = (DateTime)oRevenueDistributionInvoiceDetailViewModel.InvoiceDetail.InvoiceDate;
            DateTime lastOfThisMonth = (new DateTime(INVDate.Year, INVDate.Month, 1).AddMonths(1)).AddDays(-1);
            List<CalanderDatesModel> lstWorkingDays = CustomerService.GetCalanderDates(INVDate, lastOfThisMonth);

            var claimView = ClaimView.Instance;
            bool isClosedPeriod = claimView.GetCLAIM_PERSON_INFORMATION().lstPeriodAccess.Where(x => x.RegionId == SelectedRegionId && x.Month == INVDate.Month && x.Year == INVDate.Year).ToList().Count > 0 ? false : true;

            CustomerFranchiseeDistributionViewModel OData = CustomerService.GetCustomerFranchiseeDistributionData((int)oRevenueDistributionInvoiceDetailViewModel.InvoiceDetail.CustomerId);
            var item = OData.listContractDetail.FirstOrDefault();
            decimal? _invAmount = AccountReceivableService.GetInvoiceDetailData((int)oRevenueDistributionInvoiceDetailViewModel.InvoiceDetail.InvoiceId).InvoiceDetailItems.ToList().Sum(d => d.ExtendedPrice);
            int wAllDay = 0;
            if (item != null)
            {

                if (item.Mon == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Monday").Count();
                if (item.Tues == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                if (item.Wed == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                if (item.Thur == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Thursday").Count();
                if (item.Fri == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Friday").Count();
                if (item.Sat == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Saturday").Count();
                if (item.Sun == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Sunday").Count();
            }

            var _EffectiveDate = (DateTime)EffectiveDate;
            if ((DateTime)EffectiveDate < INVDate)
            {
                _EffectiveDate = INVDate;
            }
            List<CalanderDatesModel> lstEffectiveWorkingDays = CustomerService.GetCalanderDates(INVDate, _EffectiveDate).Where(t => t.Date != _EffectiveDate).ToList();
            int wDeffDay = 0;
            if (item != null)
            {
                if (item.Mon == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Monday").Count();
                if (item.Tues == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                if (item.Wed == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                if (item.Thur == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Thursday").Count();
                if (item.Fri == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Friday").Count();
                if (item.Sat == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Saturday").Count();
                if (item.Sun == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Sunday").Count();
            }

            int defDays = wAllDay - wDeffDay;//+ 1;
            int tMonthDays = wAllDay;

            //decimal TransferAmt = ((decimal)item.Amount / tMonthDays) * defDays;
            decimal TransferAmt = ((decimal)_invAmount / tMonthDays) * defDays;

            ViewBag.defDays = defDays;
            ViewBag.MonthDays = wAllDay;

            decimal BalTransferAmt = TransferAmt;
            foreach (var ob in oRevenueDistributionInvoiceDetailViewModel.FranchiseeDistributionItems.OrderByDescending(f => f.DistributionId))
            {


                decimal payPer = 0;
                if (isClosedPeriod)
                {
                    if (ob.Amount >= BalTransferAmt)
                    {
                        payPer = (Math.Round(BalTransferAmt, 2) / Math.Round((decimal)ob.Amount, 2)) * 100;
                        ob.Amount = -BalTransferAmt;
                        ob.Fee = ((ob.Fee * payPer) / 100);
                        ob.Total = -(BalTransferAmt - ob.Fee);
                        BalTransferAmt = 0;
                    }
                    else
                    {
                        BalTransferAmt = BalTransferAmt - (decimal)ob.Amount;
                        ob.Amount = 0;
                        ob.Fee = 0;
                        ob.Total = 0;
                    }
                }
                else
                {
                    if (ob.Amount >= BalTransferAmt)
                    {
                        payPer = (BalTransferAmt / (decimal)ob.Amount) * 100;
                        ob.Amount = (decimal)ob.Amount - BalTransferAmt;
                        ob.Fee = ob.Fee - ((ob.Fee * payPer) / 100);
                        ob.Total = ob.Amount - ob.Fee;
                        BalTransferAmt = 0;
                    }
                    else
                    {
                        BalTransferAmt = BalTransferAmt - (decimal)ob.Amount;
                        ob.Amount = 0;
                        ob.Fee = 0;
                        ob.Total = 0;
                    }
                }


            }

            return PartialView("_PartialRevenueDistribution", oRevenueDistributionInvoiceDetailViewModel);
        }

        [HttpGet]
        public JsonResult GetTransferRevenueDistributionInvoice(int CustomerId, int ContractDetailId, DateTime EffectiveDate)
        {
            var resultData = CustomerService.CheckInvoiceForRevenueDistributionDetail(CustomerId, ContractDetailId, EffectiveDate).OrderBy(f => f.BillMonth);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDecreaseCreditInvoice(int CustomerId, DateTime EffectiveDate, decimal DecreaseAmount = 0)
        {
            List<InvoiceRevenueDistributionDetailViewModel> resultData = CustomerService.CheckInvoiceForDecreaseCreditDetail(CustomerId, EffectiveDate).ToList();

            foreach (InvoiceRevenueDistributionDetailViewModel obj in resultData.ToList())
            {

                DateTime INVDate = (DateTime)obj.InvoiceDate;
                DateTime lastOfThisMonth = (new DateTime(INVDate.Year, INVDate.Month, 1).AddMonths(1)).AddDays(-1);
                List<CalanderDatesModel> lstWorkingDays = CustomerService.GetCalanderDates(INVDate, lastOfThisMonth);

                CustomerFranchiseeDistributionViewModel OData = CustomerService.GetCustomerFranchiseeDistributionData(CustomerId);
                var item = OData.listContractDetail.FirstOrDefault();
                int wAllDay = 0;
                if (item != null)
                {

                    if (item.Mon == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Monday").Count();
                    if (item.Tues == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                    if (item.Wed == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                    if (item.Thur == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Thursday").Count();
                    if (item.Fri == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Friday").Count();
                    if (item.Sat == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Saturday").Count();
                    if (item.Sun == "true")
                        wAllDay += lstWorkingDays.Where(t => t.DayName == "Sunday").Count();
                }

                List<CalanderDatesModel> lstEffectiveWorkingDays = CustomerService.GetCalanderDates(INVDate, (DateTime)EffectiveDate).Where(t => t.Date != (DateTime)EffectiveDate).ToList();
                int wDeffDay = 0;
                if (item != null)
                {
                    if (item.Mon == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Monday").Count();
                    if (item.Tues == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                    if (item.Wed == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                    if (item.Thur == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Thursday").Count();
                    if (item.Fri == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Friday").Count();
                    if (item.Sat == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Saturday").Count();
                    if (item.Sun == "true")
                        wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Sunday").Count();
                }

                int defDays = wAllDay - wDeffDay;//+ 1;
                int tMonthDays = wAllDay;


                decimal TransferAmt = ((decimal)DecreaseAmount / tMonthDays) * defDays;

                if (obj.IsPartial != 1)
                {
                    TransferAmt = DecreaseAmount;// ((decimal)(DecreaseAmount) / tMonthDays) * defDays;
                }
                //decimal TransferAmt = ((decimal)item.Amount / tMonthDays) * defDays;

                ViewBag.defDays = defDays;
                ViewBag.MonthDays = wAllDay;

                obj.EffectiveAMT = TransferAmt;

                //decimal BalTransferAmt = TransferAmt;
                //foreach (var ob in oRevenueDistributionInvoiceDetailViewModel.FranchiseeDistributionItems.OrderByDescending(f => f.DistributionId))
                //{
                //    decimal payPer = 0;
                //    if (ob.Amount >= BalTransferAmt)
                //    {
                //        payPer = (BalTransferAmt / (decimal)ob.Amount) * 100;
                //        ob.Amount = (decimal)ob.Amount - BalTransferAmt;
                //        ob.Fee = ob.Fee - ((ob.Fee * payPer) / 100);
                //        ob.Total = ob.Amount - ob.Fee;
                //        BalTransferAmt = 0;
                //    }
                //    else
                //    {
                //        BalTransferAmt = BalTransferAmt - (decimal)ob.Amount;
                //        ob.Amount = 0;
                //        ob.Fee = 0;
                //        ob.Total = 0;
                //    }
                //}


            }
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerTransferFindersFee(int cid, int fid)
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

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");

            oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";

            ViewBag.SelectedFranchiseeId = fid;


            return PartialView("_CustomerTransferFindersFee", oCID);
        }



        [HttpPost]
        public ActionResult CustomerDetailTransferDistribution(FormCollection collection, string distFee, CommonFranchiseeCustomerViewModel model, string inputStr)
        {
            List<CommonRevenueDistributionDetailViewModel> lstInputdata = new List<CommonRevenueDistributionDetailViewModel>();
            CommonRevenueDistributionDetailViewModel oInputData = new CommonRevenueDistributionDetailViewModel();

            List<CommonRevenueDistributionFeeDetailViewModel> lstDistributionFee = new List<CommonRevenueDistributionFeeDetailViewModel>();
            CommonRevenueDistributionFeeDetailViewModel oDistributionFee = new CommonRevenueDistributionFeeDetailViewModel();

            int _distributionId, _contractdetailId, _frenchiseeId, _StatusListId, _StatusReasonListId, _CustomerId = 0, _ContractDetailCount = 0, _FrenchiseeDetailCount = 0, _InvoiceId = 0, _FeeCount = 0, _FeeId = 0, _DetailLineNumber = 0;
            decimal _distributionAmount, _distributionFee, _distributionTotal, _FeeAmount, _FranchiseeDistributionCount;
            string _TroubleTransferFee, _FFNotes;
            DateTime _EffectiveDate, _StopDate;

            _ContractDetailCount = collection["hdftransfercontractdetail_linecount"] != null ? Convert.ToInt32(collection["hdftransfercontractdetail_linecount"]) : 0;
            _FrenchiseeDetailCount = collection["hdftransferfrenchiseedetail_linecount"] != null ? Convert.ToInt32(collection["hdftransferfrenchiseedetail_linecount"]) : 0;



            _CustomerId = collection["hdftransfer_customerid"] != null ? Convert.ToInt32(collection["hdftransfer_customerid"]) : 0;
            _EffectiveDate = collection["trans_ddlTransferEffectiveDate"] != null ? Convert.ToDateTime(collection["trans_ddlTransferEffectiveDate"].ToString()) : DateTime.Now;
            _StatusListId = collection["statuslist"] != null ? Convert.ToInt32(collection["statuslist"]) : 0;
            _StatusReasonListId = collection["statusreasonlist"] != null ? Convert.ToInt32(collection["statusreasonlist"]) : 0;









            //Distribution  & Transfered
            for (int i = 1; i <= _FrenchiseeDetailCount; i++)
            {
                oInputData = new CommonRevenueDistributionDetailViewModel();

                _distributionId = collection["fdt_hdfdistributionid" + i] != null ? Convert.ToInt32(collection["fdt_hdfdistributionid" + i]) : 0;
                _contractdetailId = collection["fdt_hdfcontractdetailid" + i] != null ? Convert.ToInt32(collection["fdt_hdfcontractdetailid" + i]) : 0;
                _frenchiseeId = collection["fdt_hdfFrenchiseeId" + i] != null ? Convert.ToInt32(collection["fdt_hdfFrenchiseeId" + i]) : 0;
                _distributionAmount = collection["fdt_txtfranchiseeamount" + i] != null ? Convert.ToDecimal(collection["fdt_txtfranchiseeamount" + i].ToString().Replace("$", "").Replace(",", "")) : 0;

                oInputData.CustomerId = _CustomerId;
                oInputData.StatusListId = _StatusListId;
                oInputData.StatusReasonListId = _StatusReasonListId;
                oInputData.EffectiveDate = _EffectiveDate;

                oInputData.DistributionId = _distributionId;
                oInputData.ContractDetailId = _contractdetailId;
                oInputData.FranchiseeId = _frenchiseeId;
                oInputData.Amount = _distributionAmount;

                //New Transfered Franchisee
                if (_distributionId > 0 && _distributionAmount == 0)
                {
                    oInputData.RecordType = "OLD";
                }
                else if (_distributionId == 0)
                {
                    oInputData.RecordType = "NEW";
                }

                oInputData.FeeAmount = 0;
                oInputData.TotalAmount = _distributionAmount;
                oInputData.CreatedBy = LoginUserId;
                oInputData.CreatedOn = DateTime.Now;

                lstInputdata.Add(oInputData);
            }

            _FeeCount = !String.IsNullOrEmpty(collection["hdfFranchiseeDistFeeCount"].ToString()) ? Convert.ToInt32(collection["hdfFranchiseeDistFeeCount"]) : 0;
            //Invoice Distribution Fee
            for (int i = 0; i < _FeeCount; i++)
            {
                if (collection["hdfFranchiseeDistFee_CustomerId" + i] != null)
                {
                    oDistributionFee = new CommonRevenueDistributionFeeDetailViewModel();

                    //_InvoiceId = collection["hdfFranchiseeDistFee_CustomerId" + i] != null ? Convert.ToInt32(collection["hdfFranchiseeDistFee_CustomerId" + i]) : 0;
                    _contractdetailId = collection["hdfFranchiseeDistFee_ContractDetailId" + i] != null ? Convert.ToInt32(collection["hdfFranchiseeDistFee_ContractDetailId" + i]) : 0;
                    _frenchiseeId = collection["hdfFranchiseeDistFee_FranchiseeId" + i] != null ? Convert.ToInt32(collection["hdfFranchiseeDistFee_FranchiseeId" + i]) : 0;
                    _FeeId = collection["hdfFranchiseeDistFee_FeeId" + i] != null ? Convert.ToInt32(collection["hdfFranchiseeDistFee_FeeId" + i]) : 0;
                    _FeeAmount = collection["hdfFranchiseeDistFee_FeeAmount" + i] != null ? Convert.ToDecimal(collection["hdfFranchiseeDistFee_FeeAmount" + i].ToString().Replace("$", "").Replace(",", "")) : 0;
                    _DetailLineNumber = collection["hdfFranchiseeDistFee_DetailLineNumber" + i] != null ? Convert.ToInt32(collection["hdfFranchiseeDistFee_DetailLineNumber" + i]) : 0;

                    oInputData.ContractDetailId = _contractdetailId;
                    oInputData.FranchiseeId = _frenchiseeId;

                    oDistributionFee.RecordType = "NEW";
                    oDistributionFee.Amount = _FeeAmount;
                    oDistributionFee.ContractDetailId = _contractdetailId;
                    oDistributionFee.FeeId = _FeeId;
                    oDistributionFee.FranchiseeId = _frenchiseeId;

                    lstDistributionFee.Add(oDistributionFee);
                }
            }

            //Stop Finder Fee
            for (int i = 0; i < _FrenchiseeDetailCount; i++)
            {
                if (collection["trans_txtTransferDistributionContractDetailId" + i] != null)
                {
                    oInputData = new CommonRevenueDistributionDetailViewModel();
                    oInputData.CustomerId = _CustomerId;
                    oInputData.StatusListId = _StatusListId;
                    oInputData.StatusReasonListId = _StatusReasonListId;
                    oInputData.EffectiveDate = _EffectiveDate;

                    _contractdetailId = collection["trans_txtTransferDistributionContractDetailId" + i] != null ? Convert.ToInt32(collection["trans_txtTransferDistributionContractDetailId" + i]) : 0;
                    _frenchiseeId = collection["trans_txtTransferDistributionFranchiseeId" + i] != null ? Convert.ToInt32(collection["trans_txtTransferDistributionFranchiseeId" + i]) : 0;

                    _StopDate = collection["trans_ddlTransferDistributionFranchiseeStopDate" + i] != null ? Convert.ToDateTime(collection["trans_ddlTransferDistributionFranchiseeStopDate" + i].ToString()) : DateTime.Now;
                    _FFNotes = collection["trans_ddlTransferDistributionFranchiseeNote" + i] != null ? collection["trans_ddlTransferDistributionFranchiseeNote" + i].ToString() : "";
                    _TroubleTransferFee = collection["TroubleTransferFee"] != null ? collection["TroubleTransferFee"].ToString() : "";

                    oInputData.ContractDetailId = _contractdetailId;
                    oInputData.FranchiseeId = _frenchiseeId;

                    oInputData.RecordType = "STOPFF";
                    oInputData.FFNotes = _FFNotes;
                    oInputData.FFStopDate = _StopDate;

                    oInputData.ApplyTroubleWithFee = false;
                    oInputData.ApplyTroubleWithoutFee = false;
                    oInputData.ApplyNotATroubleAccoun = false;

                    if (_TroubleTransferFee == "TroubleWithFee")
                    {
                        oInputData.ApplyTroubleWithFee = true;
                        oInputData.ApplyTroubleWithFeeAmount = 50;
                    }
                    else if (_TroubleTransferFee == "TroubleWithoutFee")
                        oInputData.ApplyTroubleWithoutFee = true;
                    else
                        oInputData.ApplyNotATroubleAccoun = true;

                    oInputData.FeeAmount = 0;
                    oInputData.TotalAmount = 0;
                    oInputData.CreatedBy = LoginUserId;
                    oInputData.CreatedOn = DateTime.Now;

                    lstInputdata.Add(oInputData);
                }
            }

            foreach (string _invoiceStr in collection.AllKeys)
            {
                if (_invoiceStr.StartsWith("hdfInvoiceDetail_InvoiceId_"))
                {
                    string _InvId = _invoiceStr.Replace("hdfInvoiceDetail_InvoiceId_", "");

                    bool _IsSkip = !String.IsNullOrEmpty(collection["hdfRevenueDistributionInvoiceDetail_Skip" + _InvId]) ? bool.Parse(collection["hdfRevenueDistributionInvoiceDetail_Skip" + _InvId]) : false;
                    if (!_IsSkip)
                    {
                        _FranchiseeDistributionCount = !String.IsNullOrEmpty(collection["hdfInvoiceDetail_FranchiseeDistributionCount_" + _InvId]) ? Convert.ToInt32(collection["hdfInvoiceDetail_FranchiseeDistributionCount_" + _InvId]) : 0;
                        for (int i = 1; i <= _FranchiseeDistributionCount; i++)
                        {
                            oInputData = new CommonRevenueDistributionDetailViewModel();
                            oInputData.RecordType = "INVDIST";
                            _InvoiceId = collection["hdfInvoiceDetail_InvoiceId_" + _InvId] != null ? Convert.ToInt32(collection["hdfInvoiceDetail_InvoiceId_" + _InvId]) : 0;
                            _contractdetailId = !String.IsNullOrEmpty(collection["fdt_INV_hdfcontractdetailid_" + _InvId + "_" + i]) ? Convert.ToInt32(collection["fdt_INV_hdfcontractdetailid_" + _InvId + "_" + i]) : 0;
                            _frenchiseeId = collection["fdt_INV_hdfFrenchiseeId_" + _InvId + "_" + i] != null ? Convert.ToInt32(collection["fdt_INV_hdfFrenchiseeId_" + _InvId + "_" + i]) : 0;
                            _distributionAmount = collection["fdt_inv_txtfranchiseeamount_" + _InvId + "_" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamount_" + _InvId + "_" + i]) : 0;
                            _distributionFee = collection["fdt_inv_txtfranchiseeamountfee_" + _InvId + "_" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamountfee_" + _InvId + "_" + i]) : 0;
                            _distributionTotal = collection["fdt_inv_txtfranchiseeamounttotal_" + _InvId + "_" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamounttotal_" + _InvId + "_" + i]) : 0;


                            for (int ii = 1; ii <= _FrenchiseeDetailCount; ii++)
                            {
                                int _distributionIdCC = collection["fdt_hdfdistributionid" + ii] != null ? Convert.ToInt32(collection["fdt_hdfdistributionid" + ii]) : 0;
                                int _frenchiseeIdCC = collection["fdt_hdfFrenchiseeId" + ii] != null ? Convert.ToInt32(collection["fdt_hdfFrenchiseeId" + ii]) : 0;
                                decimal _distributionAmountCC = collection["fdt_txtfranchiseeamount" + ii] != null ? Convert.ToDecimal(collection["fdt_txtfranchiseeamount" + ii].ToString().Replace("$", "").Replace(",", "")) : 0;

                                if (_distributionIdCC > 0 && _distributionAmountCC == 0 && _frenchiseeId == _frenchiseeIdCC)
                                {
                                    oInputData.RecordType = "OLDINVDIST";
                                }
                            }
                            oInputData.DistributionId = _InvoiceId;
                            oInputData.CustomerId = _CustomerId;
                            oInputData.StatusListId = _StatusListId;
                            oInputData.StatusReasonListId = _StatusReasonListId;
                            oInputData.EffectiveDate = _EffectiveDate;
                            oInputData.ContractDetailId = _contractdetailId;
                            oInputData.FranchiseeId = _frenchiseeId;
                            oInputData.Amount = _distributionAmount;
                            oInputData.FeeAmount = _distributionFee;
                            oInputData.TotalAmount = _distributionTotal;
                            oInputData.CreatedBy = LoginUserId;
                            oInputData.CreatedOn = DateTime.Now;

                            lstInputdata.Add(oInputData);
                        }
                    }
                }

                //
            }

            int FindersFeeCount = !String.IsNullOrEmpty(collection["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(collection["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;

            if (model.FindersFee != null)
            {
                for (int i = 1; i <= FindersFeeCount; i++)
                {
                    if (collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                    {
                        int FFAdjustmentId = !String.IsNullOrEmpty(collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                        if (FFAdjustmentId > 0)
                        {
                            string FFAdjustment_Description = !String.IsNullOrEmpty(collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                            decimal FFAdjustment_Amount = !String.IsNullOrEmpty(collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(collection["txtFindersFeeAdjustment_" + model.FindersFee.FindersFeeId + "_" + i + "_Amount"].ToString().Replace("$", "").Replace(",", "").Trim()) : 0;

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



            //Invoice Distribution
            //if (!string.IsNullOrEmpty(collection["InvoiceDetail.InvoiceId"]))
            //{
            //    for (int i = 1; i <= _FrenchiseeDetailCount; i++)
            //    {
            //        oInputData = new CommonRevenueDistributionDetailViewModel();
            //        oInputData.RecordType = "INVDIST";
            //        _InvoiceId = collection["InvoiceDetail.InvoiceId"] != null ? Convert.ToInt32(collection["InvoiceDetail.InvoiceId"]) : 0;
            //        _contractdetailId = collection["fdt_INV_hdfcontractdetailid" + i] != null ? Convert.ToInt32(collection["fdt_INV_hdfcontractdetailid" + i]) : 0;
            //        _frenchiseeId = collection["fdt_INV_hdfFrenchiseeId" + i] != null ? Convert.ToInt32(collection["fdt_INV_hdfFrenchiseeId" + i]) : 0;
            //        _distributionAmount = collection["fdt_inv_txtfranchiseeamount" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamount" + i]) : 0;
            //        _distributionFee = collection["fdt_inv_txtfranchiseeamountfee" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamountfee" + i]) : 0;
            //        _distributionTotal = collection["fdt_inv_txtfranchiseeamounttotal" + i] != null ? Convert.ToDecimal(collection["fdt_inv_txtfranchiseeamounttotal" + i]) : 0;

            //        oInputData.DistributionId = _InvoiceId;
            //        oInputData.CustomerId = _CustomerId;
            //        oInputData.StatusListId = _StatusListId;
            //        oInputData.StatusReasonListId = _StatusReasonListId;
            //        oInputData.EffectiveDate = _EffectiveDate;
            //        oInputData.ContractDetailId = _contractdetailId;
            //        oInputData.FranchiseeId = _frenchiseeId;
            //        oInputData.Amount = _distributionAmount;
            //        oInputData.FeeAmount = _distributionFee;
            //        oInputData.TotalAmount = _distributionTotal;
            //        oInputData.CreatedBy = LoginUserId;
            //        oInputData.CreatedOn = DateTime.Now;

            //        lstInputdata.Add(oInputData);
            //    }
            //}

            CustomerService.InsertCustomerTransferData(lstInputdata, lstDistributionFee, model);

            return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = _CustomerId });// PartialView("_CustomerTransferDistribution");
        }

        [HttpGet]
        public ActionResult CustomerTransferPendingList()
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerTransferPendingList", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("CustomerTransferPendingList", "Customer", new { area = "Portal" }), "Customer Transfer Pending List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            List<DropDownModel> sList = _commonService.DropDownListByName(MasterDropName.StatusList.ToString());

            ViewBag.statusList = new SelectList(sList.OrderBy(j => j.Value), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        [HttpGet]
        public ActionResult CustomerTransferListData(string rgId)
        {
            try
            {
                var customertransfers = CustomerService.GetCustomerTransferPendingList(rgId);

                var jsonResult = Json(new
                {
                    aadata = customertransfers,
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region ::  Franchise Distribution ::

        //[HttpGet]
        //public ActionResult RenderFranchiseDistributionDetailPopup(int id)
        //{
        //    FranchiseeDistributionDetailsModel FranchiseeDistributionDetailsModel = new FranchiseeDistributionDetailsModel();
        //    if (id > 0)
        //    {
        //        FranchiseeDistributionDetailsModel = FranchiseeService.GetFranchiseeDistributionDetails(id);
        //    }

        //    return PartialView("_FranchiseDistributionDetailPopup", FranchiseeDistributionDetailsModel);
        //}

        [HttpGet]
        public ActionResult RenderFranchiseCustomerDistributionPopup(int cid, int fid, string callfrom = "")
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

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");

            //ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();
            oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";
            ViewBag.CallFrom = callfrom;
            ViewBag.SelectedFranchiseeId = fid;
            return PartialView("_CustomerFranchiseeDistribution", oCID);
        }
        [HttpGet]
        public ActionResult RenderFranchiseCustomerOnlyDistributionPopup(int cid, int fid, string callfrom = "")
        {
            CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
            oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, (fid > 0 ? fid : -1), -1);

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

            //ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();
            oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";
            ViewBag.CallFrom = callfrom;
            ViewBag.SelectedFranchiseeId = fid;
            return PartialView("_CustomerDistributionPP", oCID);
        }


        //[HttpGet]
        //public ActionResult RenderFranchiseCustomerOnlyDistributionPopup(int cid, int fid, string callfrom = "")
        //{
        //    CommonFranchiseeCustomerViewModel oCID = new CommonFranchiseeCustomerViewModel();
        //    oCID = FranchiseeService.GetFranchiseeCustomerDistributionData(cid, (fid > 0 ? fid : -1), -1);

        //    ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
        //    ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");

        //    ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

        //    var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
        //    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

        //    var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
        //    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

        //    var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
        //    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

        //    ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
        //    ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

        //    ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");

        //    //ViewBag.FFSchedule = oCID.lstFindersFeeSchedule.ToJSON();
        //    oCID.FindersFee.Description = oCID.FindersFee.Description != "" ? oCID.FindersFee.Description : "Finder Fees on Contract Billing";
        //    ViewBag.CallFrom = callfrom;
        //    ViewBag.SelectedFranchiseeId = fid;
        //    return PartialView("_CustomerFranchiseeDistributionRender", oCID);
        //}


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
        public ActionResult FranchiseCustomerDistributionPopup(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
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

        [HttpPost]
        public ActionResult SaveFranchiseCustomerDistributionPopup(CommonFranchiseeCustomerViewModel model, FormCollection fromcollection, string inputStr)
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
            var retVal = FranchiseeService.InsertFranchiseeCustomerOnlyDistributionDetail(model, 1);
            return Json(new { CustomerId = model.CustomerDetail.CustomerId, CallFrom = CallFrom }, JsonRequestBehavior.AllowGet);
            //if (CallFrom == "RO")
            //{                
            //    return RedirectToAction("Index", "CRMRegionOperation", new { area = "CRM", id = model.CustomerDetail.CustomerId });
            //}
            //else if (CallFrom == "AO")
            //{
            //    return RedirectToAction("Index", "CRMRegionAccounting", new { area = "CRM", id = model.CustomerDetail.CustomerId });
            //}
            //else if (CallFrom == "FF")
            //{
            //    return RedirectToAction("FinderFeeList", "Franchise", new { area = "Portal" });
            //}
            //else
            //{
            //    return RedirectToAction("MaintenanceStepFifth", "Customer", new { area = "Portal", id = model.CustomerDetail.CustomerId });
            //}            
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

            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.FStatusList = new SelectList(_commonService.DropDownListByName(MasterDropName.FranchiseeStatusList.ToString()), "Value", "Text");
            return PartialView("_CustomerFranchiseeDistributionMaintenanceDetail", oCID);
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

            if (lstDistributionFee != null)
            {
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
            }

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
            return RedirectToAction("PendingApprovalList", "Customer", new { area = "Portal" });
        }

        #endregion ::  Franchise Distribution ::

        [HttpGet]
        public JsonResult GetCustomerTransferDetailData(int id)
        {
            CustomerFranchiseeDistributionViewModel oFranchiseeDistribution = CustomerService.GetCustomerFranchiseeDistributionData(id);
            return Json(oFranchiseeDistribution, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerDetailTransferApprovalDetail(int id)
        {
            ViewBag.FindersFeeAdjustmentTypeListRaw = CustomerService.GetFindersFeeAdjustmentTypeList();//, "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusListRaw = FranchiseeService.GetAll_TransactionStatusList();//, "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeListRaw = FranchiseeService.GetAll_FindersFeeTypeList();//, "FindersFeeTypeListId", "Name");

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);
            CommonCustomerrTransferPendingDetailViewModel oRetVal = CustomerService.GetCustomerrTransferPendingDetailData(id);
            return PartialView("_CustomerTransferPendingDetail", oRetVal);
        }

        [HttpGet]
        public JsonResult CustomerDetailTransferApproval(int id)
        {
            CommonCustomerrTransferPendingDetailViewModel oRetVal = CustomerService.GetCustomerrTransferPendingApproval(id);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerMaintenanceApproval(int id)
        {
            CustomerMaintenanceApproval oCustomerMaintenanceApproval = CustomerService.CustomerMaintenanceApprovalByTempMaintenanceID(id);

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            ViewBag.StatusList = new SelectList(CustomerService.GetAll_StatusList().Where(x => x.TypeListId == TypeList && x.MaintenanceTypeListId == 8), "StatusListId", "Name");
            ViewBag.StatusReasonList = new SelectList(CustomerService.GetAll_StatusReasonList(1), "StatusReasonListId", "Name");
            CustomerMaintenanceViewModel oCustomerMaintenanceViewModel;
            int custID = oCustomerMaintenanceApproval.CustomerId;
            if (custID > 0)
            {
                oCustomerMaintenanceViewModel = CustomerService.GetCustomerMaintenanceDetailsById(custID);
            }
            else
            {
                oCustomerMaintenanceViewModel = new CustomerMaintenanceViewModel();
                oCustomerMaintenanceViewModel.FindersFee = new List<CustomerStatusFindersFeeViewModel>();
            }
            oCustomerMaintenanceApproval.CustomerMaintenance = oCustomerMaintenanceViewModel;
            return PartialView("_CustomerMaintenanceApproval", oCustomerMaintenanceApproval);
        }

        [HttpPost]
        public ActionResult CustomerDetailTransferApprovalSubmit(CommonCustomerrTransferPendingDetailViewModel model, FormCollection formc)
        {
            var ab = model;
            var isApprove = true;
            if (!String.IsNullOrEmpty(formc["hdftransfer_Callfor"].ToString()))
            {
                isApprove = (formc["hdftransfer_Callfor"].ToString() == "Approve" ? true : false);
            }
            CommonCustomerrTransferPendingDetailViewModel oRetVal = CustomerService.GetCustomerrTransferPendingApproval(model.CustomerrTransferPendingDetail.MaintenanceTempId, isApprove);
            return RedirectToAction("CustomerMaintenancePendingList", "Customer", new { area = "Portal" });
        }

        [HttpPost]
        public ActionResult CustomerMaintenanceDetailPPApprovalSubmit(CommonCustomerMaintenanceDetailViewModel model, FormCollection formc)
        {
            bool isApprove = true;
            if (formc["hdfmaintenancedetail_submitfor"].ToString() == "Approve")
                isApprove = true;
            else
                isApprove = false;
            CommonCustomerMaintenanceDetailViewModel oRetVal = CustomerService.GetCustomerMaintenanceDetailPPApproval(model.CustomerrMaintenanceDetail.MaintenanceTempId, isApprove);
            return RedirectToAction("CustomerMaintenancePendingList", "Customer", new { area = "Portal" });
        }


        [HttpGet]
        public JsonResult GetCustomerIncreaseInvoice(int customerid, DateTime effectivedate, decimal applyamount)
        {
            DateTime firstOfThisMonth = new DateTime(effectivedate.Year, effectivedate.Month, 1);

            DateTime lastOfThisMonth = (new DateTime(effectivedate.Year, effectivedate.Month, 1).AddMonths(1)).AddDays(-1);
            List<CalanderDatesModel> lstWorkingDays = CustomerService.GetCalanderDates(firstOfThisMonth, lastOfThisMonth);


            CustomerFranchiseeDistributionViewModel OData = CustomerService.GetCustomerFranchiseeDistributionData(customerid);
            var item = OData.listContractDetail.FirstOrDefault();
            int wAllDay = 0;
            if (item != null)
            {

                if (item.Mon == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Monday").Count();
                if (item.Tues == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                if (item.Wed == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                if (item.Thur == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Thursday").Count();
                if (item.Fri == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Friday").Count();
                if (item.Sat == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Saturday").Count();
                if (item.Sun == "true")
                    wAllDay += lstWorkingDays.Where(t => t.DayName == "Sunday").Count();
            }

            List<CalanderDatesModel> lstEffectiveWorkingDays = CustomerService.GetCalanderDates(effectivedate, lastOfThisMonth);
            int wDeffDay = 0;
            if (item != null)
            {
                if (item.Mon == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Monday").Count();
                if (item.Tues == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Tuesday").Count();
                if (item.Wed == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Wednesday").Count();
                if (item.Thur == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Thursday").Count();
                if (item.Fri == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Friday").Count();
                if (item.Sat == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Saturday").Count();
                if (item.Sun == "true")
                    wDeffDay += lstEffectiveWorkingDays.Where(t => t.DayName == "Sunday").Count();
            }


            int workingDays = wDeffDay;// + 1;
            int monthDays = wAllDay;

            decimal _perdayAMT = applyamount / monthDays;
            return Json(CustomerService.GetCustomerIncreaseInvoice(customerid, effectivedate, monthDays, workingDays, applyamount), JsonRequestBehavior.AllowGet);
        }

        #endregion Customer Transfer

        #region Customer Increase/Decrease

        [HttpGet]
        public ActionResult CustomerDetailIncrease(int id)
        {
            CommonCustomerIncreaseDecreaseDetailViewModel oCID = new CommonCustomerIncreaseDecreaseDetailViewModel();
            oCID = CustomerService.GetCustomerIncreaseDecreaseDetailData(id);
            oCID.CustomerIncreaseDecreaseDetail.OTotalAmount = oCID.CustomerIncreaseDecreaseDetail.TotalAmount;
            oCID.CustomerIncreaseDecreaseDetail.EffectiveDate = DateTime.Now;

            foreach (var detail in oCID.lstContractDetail)
            {
                detail.OAmount = detail.Amount;
            }

            ViewBag.FindersFeeAdjustmentTypeListRaw = CustomerService.GetFindersFeeAdjustmentTypeList();
            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");

            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            ViewBag.FindersFeeTypeListRaw = FranchiseeService.GetAll_FindersFeeTypeList();
            //CustomerService.GetCCFinderFeeDetail(SelectedRegionId, id);
            //return PartialView("_FinderFeeDetailPopup", );


            var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
            ViewBag.ServiceTypeListModelRaw = ServiceTypeListModel;


            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (oCID.CustomerIncreaseDecreaseDetail != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", oCID.CustomerIncreaseDecreaseDetail.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            return PartialView("_CustomerDetailIncrease", oCID);
        }

        [HttpGet]
        public JsonResult GetFindersFeeAdjustmentList()
        {
            return Json(new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name"), JsonRequestBehavior.AllowGet);
        }

        private CreditTransactionViewModel _GetCreditTransactionViewModelFromForm(FormCollection frm, int InvId)
        {
            int intRes;
            decimal decRes;

            var updateCreditId = frm["updateCreditId" + "_" + InvId] != null ? int.Parse(frm["updateCreditId" + "_" + InvId]) : -1;

            var invoiceId = frm["invoiceId" + "_" + InvId] != null ? int.Parse(frm["invoiceId" + "_" + InvId]) : 0;
            var creditReasonListId = int.TryParse(frm["slReasonList" + "_" + InvId], out intRes) ? intRes : 0;
            var creditDesc = frm["creditDesc" + "_" + InvId];



            var creditDate = frm["creditDate" + "_" + InvId] != null ? DateTime.Parse(frm["creditDate" + "_" + InvId]) : DateTime.Now;
            var billMonth = creditDate.Month;
            var billYear = creditDate.Year;

            var totalCreditAmt = decimal.TryParse(frm["creditAmt" + "_" + InvId], out decRes) ? decRes : 0;
            var newBalance = decimal.TryParse(frm["newBalance" + "_" + InvId], out decRes) ? decRes : 0;

            decimal applyCreditAmount = 0;
            var applyCreditAmt = decimal.TryParse(frm["requestCreditAmt" + "_" + InvId], out applyCreditAmount) ? applyCreditAmount : 0;

            var invoiceDetails = AccountReceivableService.GetCreditDetailForInvoice(invoiceId);

            var vm = new CreditTransactionViewModel
            {
                Id = updateCreditId,
                RegionId = (int)invoiceDetails.Invoice.InvoiceDetail.RegionId,
                InvoiceId = invoiceId,
                CreditReasonListId = creditReasonListId,
                CreditDescription = creditDesc,
                TotalCredit = totalCreditAmt,
                IsExtraCredit = applyCreditAmt > totalCreditAmt,// newBalance < 0,
                PaidInFull = invoiceDetails.InvoiceBalance == totalCreditAmt,// && newBalance <= 0,
                BillMonth = billMonth,
                BillYear = billYear,
                CreatedBy = LoginUserId,
                CreatedDate = creditDate,
                ApplyTotalCredit = applyCreditAmt
            };

            var ccvm = new CustomerCreditViewModel
            {
                CustomerId = (int)invoiceDetails.Invoice.InvoiceDetail.CustomerId,
                Credits = new List<CreditViewModel>()
            };

            for (var i = 0; i < invoiceDetails.Invoice.InvoiceDetailItems.Count(); i++)
            {
                var foundFields = true;
                decimal oldBalance = 0;
                decimal creditAmt = 0;
                decimal total = 0;

                // only supply values if we are a normal credit
                if (!vm.IsExtraCredit)
                {
                    foundFields = foundFields &&
                                  decimal.TryParse(frm[string.Format("item{0}_oldBalance" + "_" + InvId, i)], out oldBalance);
                    foundFields = foundFields &&
                                  decimal.TryParse(frm[string.Format("item{0}_creditAmt" + "_" + InvId, i)], out creditAmt);
                    foundFields = foundFields && decimal.TryParse(frm[string.Format("item{0}_total" + "_" + InvId, i)], out total);
                }

                if (foundFields)
                {
                    // sanity checks if local validation failed
                    creditAmt = Math.Min(oldBalance, creditAmt);
                    total = Math.Min(oldBalance, total);

                    var cvm = new CreditViewModel
                    {
                        BaseMasterTrxDetailId = invoiceDetails.Invoice.InvoiceDetailItems[i].MasterTrxDetailId,
                        LineNo = (int)invoiceDetails.Invoice.InvoiceDetailItems[i].LineNumber,
                        ServiceTypeListId = (int)invoiceDetails.Invoice.InvoiceDetailItems[i].ServiceTypeListId,
                        CreditAmount = total,
                        Tax = creditAmt - total,
                        Total = creditAmt
                    };
                    ccvm.Credits.Add(cvm);
                }
            }

            vm.CustomerCredit = ccvm;

            var fcvms = new List<FranchiseeCreditViewModel>();

            foreach (var item in invoiceDetails.FranchiseeItems)
            {
                var r = item.InvoiceFranchiseeDetailItem;
                var bpId = r.BillingPayId;
                decimal res;

                // old code
                //if (decimal.TryParse(frm[$"bp{bpId}_creditAmt"], out res))

                // changes done by Ajay Prakash
                if (decimal.TryParse(frm[$"bp{bpId}_creditAmt" + "_" + InvId], out res))
                {
                    var fcvm = new FranchiseeCreditViewModel
                    {
                        FranchiseeId = r.FranchiseeId,
                        BillingPayId = r.BillingPayId
                    };

                    var cvm = new CreditViewModel
                    {
                        BaseMasterTrxDetailId = r.MasterTrxDetailId,
                        LineNo = (int)r.LineNo
                    };

                    var customerCvm = ccvm.Credits.FirstOrDefault(o => o.LineNo == cvm.LineNo);
                    if (customerCvm != null)
                    {
                        cvm.ServiceTypeListId = customerCvm.ServiceTypeListId;
                    }

                    cvm.CreditAmount = res;
                    fcvm.Credit = cvm;

                    fcvms.Add(fcvm);
                }
            }

            vm.FranchiseeCredits = fcvms;

            return vm;
        }

        [HttpPost]
        public ActionResult CustomerDetailIncrease(CommonCustomerIncreaseDecreaseDetailViewModel model, FormCollection fromcollection)
        {
            var t = model;
            var frm = fromcollection;

            int _CustomerIncreaseDecreaseDetail_Type = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_Type"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_Type"].ToString()) : 0;

            if (_CustomerIncreaseDecreaseDetail_Type != 1)
            {
                model.lstFindersFee = new List<CIDFindersFeeViewModel>();
                model.lstFindersFeeAdjustment = new List<CIDFindersFeeAdjustmentViewModel>();
            }
            else
            {
                int FindersFeeCount = !String.IsNullOrEmpty(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) ? int.Parse(frm["CustomerIncreaseDecreaseDetail_FindersFeeAdjustmentCount"].ToString()) : 0;
                if (model.lstFindersFee != null)
                {
                    foreach (CIDFindersFeeViewModel oFindersFee in model.lstFindersFee)
                    {
                        for (int i = 1; i <= FindersFeeCount; i++)
                        {
                            if (frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Adjustment"] != null)
                            {
                                int FFAdjustmentId = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) ? int.Parse(frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Adjustment"].ToString()) : 0;

                                if (FFAdjustmentId > 0)
                                {
                                    string FFAdjustment_Description = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Description"].ToString()) ? frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Description"].ToString() : "";
                                    decimal FFAdjustment_Amount = !String.IsNullOrEmpty(frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) ? decimal.Parse(frm["txtFindersFeeAdjustment_" + oFindersFee.FindersFeeId + "_" + i + "_Amount"].ToString()) : 0;

                                    CIDFindersFeeAdjustmentViewModel oFFAdjustment = new CIDFindersFeeAdjustmentViewModel();
                                    oFFAdjustment.FindersFeeAdjustmentId = 0;
                                    oFFAdjustment.FindersFeeAdjustmentTypeListId = FFAdjustmentId;
                                    oFFAdjustment.FindersFeeId = oFindersFee.FindersFeeId;
                                    oFFAdjustment.FranchiseeId = oFindersFee.FranchiseeId;
                                    oFFAdjustment.Description = FFAdjustment_Description;
                                    oFFAdjustment.Amount = FFAdjustment_Amount;
                                    if (model.lstFindersFeeAdjustment == null)
                                        model.lstFindersFeeAdjustment = new List<CIDFindersFeeAdjustmentViewModel>();

                                    model.lstFindersFeeAdjustment.Add(oFFAdjustment);
                                }
                            }
                        }
                    }
                }
            }
            List<CreditTransactionViewModel> lstVM = new List<CreditTransactionViewModel>();
            if (model.CustomerIncreaseDecreaseDetail.MaintenanceTypeListId == 5)
            {
                if (!String.IsNullOrEmpty(fromcollection["Customercustomermanualinvoice_CRInvoiceIds"].ToString()))
                {
                    foreach (string o in fromcollection["Customercustomermanualinvoice_CRInvoiceIds"].ToString().Split(','))
                    {
                        if (o.Trim() != string.Empty)
                        {
                            lstVM.Add(_GetCreditTransactionViewModelFromForm(fromcollection, int.Parse(o.Trim())));
                        }
                    }
                }
            }
            var retVal = CustomerService.InsertCustomerIncreaseDecreaseDetail(model, lstVM);

            return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = model.CustomerIncreaseDecreaseDetail.CustomerId });
        }

        public ActionResult CustomerDetailIncreaseDecreasePopUp(int id)
        {
            CommonCustomerIncreaseDecreaseDetailViewModel oCID = new CommonCustomerIncreaseDecreaseDetailViewModel();
            oCID = CustomerService.GetCustomerIncreaseDecreaseDetailDataForEdit(id);

            ViewBag.FindersFeeAdjustmentTypeListRaw = CustomerService.GetFindersFeeAdjustmentTypeList();//, "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusListRaw = FranchiseeService.GetAll_TransactionStatusList();//, "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeListRaw = FranchiseeService.GetAll_FindersFeeTypeList();//, "FindersFeeTypeListId", "Name");

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (oCID.CustomerIncreaseDecreaseDetail != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", oCID.CustomerIncreaseDecreaseDetail.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);

            return PartialView("_CustomerIncreaseDecreaseDetailPopup", oCID);
        }

        [HttpPost]
        public ActionResult CustomerDetailIncreaseDecreasePopUp(CommonCustomerIncreaseDecreaseDetailViewModel model, FormCollection fromcollection)
        {
            var t = model;
            var frm = fromcollection;
            var isApprove = true;
            if (!String.IsNullOrEmpty(frm["hdftransfer_Callfor"].ToString()))
            {
                isApprove = (frm["hdftransfer_Callfor"].ToString() == "Approve" ? true : false);
            }
            var retVal = CustomerService.UpdateCustomerIncreaseDecreaseDetailApprove(model, isApprove);
            return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = model.CustomerIncreaseDecreaseDetail.CustomerId });
        }

        #endregion Customer Increase/Decrease

        [HttpGet]
        public ActionResult CustomerDetailFranchiseeDistribution(int id)
        {
            return PartialView("_CustomerDetailFranchiseeDistribution");
        }

        [HttpGet]
        public ActionResult CustomerDetailIncreaseDistribution()
        {
            return PartialView("_CustomerDetailFranchiseeDistributionIncrease");
        }

        [HttpGet]
        public JsonResult CustomerDetailFranchiseeDistributionData(int id)
        {
            CFranchiseeDistributionViewModel oFranchiseeDistribution = CustomerService.GetCustomerFranchiseeDistribution(id);
            return Json(oFranchiseeDistribution, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomerFranchiseeDistributionData(int id)
        {
            CustomerFranchiseeDistributionViewModel oFranchiseeDistribution = CustomerService.GetCustomerFranchiseeDistributionData(id);
            return Json(oFranchiseeDistribution, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DistributionFeeData(int id)
        {
            var data = CustomerService.GetDistributionFeeData(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFeeData()
        {
            var data = CustomerService.GetFeeData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FranchiseeDistributionFeeData(int FranchiseeId, int ContractDetailId, int DistributionId = 0)
        {
            var data = CustomerService.GetFranchiseeDistributionFeesData(FranchiseeId, ContractDetailId, DistributionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FranchiseeDistributionFeeSave(string inputdata)
        {
            List<FranchiseeDistributionFeesViewModel> lstDistributionFee = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FranchiseeDistributionFeesViewModel>>(inputdata);
            if (lstDistributionFee != null && lstDistributionFee.Count > 0)
            {
                var data = CustomerService.SaveFranchiseeDistributionFee(lstDistributionFee);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FranchiseeDistributionData(int id)
        {
            CFranchiseeDistributionViewModel oFranchiseeDistribution = CustomerService.GetFranchiseeDistribution(id);
            return Json(oFranchiseeDistribution, JsonRequestBehavior.AllowGet);
        }

        #endregion Customer > General

        #region Customer > Transactions

        public ActionResult Transactions()
        {
            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();
            ViewBag.CurrentMenu = "Transactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Transactions", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Transactions", "Customer", new { area = "Portal" }), "Transactions");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return View();
        }

        [HttpGet]
        public ActionResult BillRun()
        {
            ViewBag.CurrentMenu = "CustomerTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ProcessBillRun", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("ProcessBillRun", "Customer", new { area = "Portal" }), "BillRun");

            BreadCrumb.Add(Url.Action("ProcessBillRun", "AccountReceivable", new { area = "Portal" }), "Account Receivable");

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

            return View();
        }

        [HttpGet]
        public ActionResult AllTransactions()
        {
            ViewBag.CurrentMenu = "Transactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AllTransactions", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("AllTransactions", "Customer", new { area = "Portal" }), "Transactions");
            BreadCrumb.Add(Url.Action("AllTransactions", "Customer", new { area = "Portal" }), "All Transactions");
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            var combodata = TransactionsTypeList.Select(o => new SelectListItem
            {
                Value = o.MasterTrxTypeListId.ToString(),
                Text = o.Name
            });
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.CustomerTransactionType = combodata;
            return View();
        }

        [HttpGet]
        public ActionResult GetRegionWiseCustomer(string customerName)
        {
            var data = CustomerService.GetRegionWiseCustomer(customerName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCustomerDetailsbyId(int customerId)
        {
            var data = GetCustomerDetailViewModel(customerId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllCustomerTransactions(int CustomerId, int? TypeId, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            EndDate += new TimeSpan(23, 59, 59);
            return Json(new
            {
                StartingBalance = CustomerService.GetCustomerBalanceAsOfDate(CustomerId, StartDate),
                Transactions = CustomerService.GetAllCustomerTransactions(CustomerId, TypeId, StartDate, EndDate, month, year)
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Customer > Transactions

        #region Customer > Service

        public ActionResult ServiceCall()
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCall", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCall", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCall", "Customer", new { area = "Portal" }), "Service Call");
            return View();
        }

        public ActionResult CollectCall()
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CollectCall", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CollectCall", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("CollectCall", "Customer", new { area = "Portal" }), "Collect Call");
            return View();
        }

        [HttpGet]
        [OutputCache(Duration = 100, VaryByParam = "id")]
        public ActionResult CollectionCallLog(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != null && id != 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(id);
                ViewBag.CollectionCallLog = CustomerService.GetServiceCollectionCallLogCustomersList(id ?? 0);
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();
                    var customer = CustomerService.GetCustomerById(id ?? 0);
                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    customerDetailViewModel.RegionId = String.IsNullOrEmpty(customer.RegionId.ToString()) ? String.Empty : customer.RegionId.ToString();
                }
            }
            ViewBag.CustomerDetail = customerDetailViewModel;

            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CollectionCallLog", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CollectionCallLog", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("CollectionCallLog", "Customer", new { area = "Portal" }), "Collect Call Log");

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            //Franchises

            ViewBag.Franchisees = CustomerService.getFrancisesByCustomerId((id ?? 0));
            //var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            //ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            return View();
        }

        [HttpPost]
        public ActionResult CollectionCallLog(CollectionsCallLogModel objCollectionsCallLog)
        {
            DateTime dtCallBack = DateTime.ParseExact(objCollectionsCallLog.strCallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            objCollectionsCallLog.CallBack = dtCallBack;
            objCollectionsCallLog.CallDate = DateTime.Now;
            objCollectionsCallLog.Internal = (objCollectionsCallLog.boolInternal ? 1 : 0);
            objCollectionsCallLog.CallTime = DateTime.Now.ToString("HH:mm:ss");
            var ret = CustomerService.SaveCollectionCallLog(objCollectionsCallLog);
            return RedirectToAction("CollectionCallLog", new { id = objCollectionsCallLog.ClassId });
        }

        [HttpGet]
        public ActionResult ServiceCallLog(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != null && id != 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(id);

                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();
                    var customer = CustomerService.GetCustomerById(id ?? 0);
                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    customerDetailViewModel.RegionId = String.IsNullOrEmpty(customer.RegionId.ToString()) ? String.Empty : customer.RegionId.ToString();
                }
            }
            ViewBag.CustomerDetail = customerDetailViewModel;
            TempData["CustomerInformation"] = customerDetailViewModel;

            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCallLog", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCallLog", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCallLog", "Customer", new { area = "Portal" }), "Service Call Log");

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            return View(new JKViewModels.Customer.ServiceCallLogModel());
        }

        [HttpPost]
        public ActionResult ServiceCallLog(ServiceCallLog ServiceCallLog, string CallBack)
        {
            DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            ServiceCallLog.CallBack = dtCallBack;
            ServiceCallLog.CallDate = DateTime.Now;
            ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;
            CustomerService.SaveServiceCallLog(ServiceCallLog);
            string SaveAndNew = Request.Form["SaveNew"].ToString();
            string SaveAndClose = Request.Form["SaveClose"].ToString();
            if (SaveAndClose != "" && SaveAndClose != null)
            {
                TempData["CustomerId"] = ServiceCallLog.ClassId;
                return RedirectToAction("ServiceCallList");
            }
            else
            {
                ModelState.Clear();
                TempData["CustomerId"] = null;
                TempData["CustomerInformation"] = null;
                return Redirect("ServiceCallLog");
            }

            //return View();
        }


        public ActionResult ServiceCallLogPop(ServiceCallLog ServiceCallLog, string CallBack)
        {
            //if (CallBack != null && CallBack != "")
            //{
            //    //DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            //    DateTime dtCallBack = Convert.ToDateTime(CallBack);
            //    ServiceCallLog.CallBack = dtCallBack;
            //}
            //ServiceCallLog.CallDate = DateTime.Now;
            //ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

            if (Request.Form["hdnActiveType"] == "2")
            {
                #region :: Internal Office Notes  ::

                if (Request.Form["hdnIsCallBack2"] != null && Request.Form["hdnIsCallBack2"] != "")
                {
                    if (Request.Form["hdnIsCallBack2"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack2"] != null && Request.Form["txtCallBack2"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack2"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //ServiceCallLog.CallTime = TMspan;
                        //    ServiceCallLog.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["txtSpokeWith2"] != null && Request.Form["txtSpokeWith2"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["txtSpokeWith2"];
                }
                if (Request.Form["txtAction2"] != null && Request.Form["txtAction2"] != "")
                {
                    ServiceCallLog.Action = Request.Form["txtAction2"];
                }
                if (Request.Form["txtComments2"] != null && Request.Form["txtComments2"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["txtComments2"];
                }
                if (Request.Form["ddlServiceLogTypeListId2"] != null && Request.Form["ddlServiceLogTypeListId2"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ddlServiceLogTypeListId2"]);
                }
                if (Request.Form["ddlFollowUpBy2"] != null && Request.Form["ddlFollowUpBy2"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["ddlFollowUpBy2"]);
                }
                if (Request.Form["hdnEmailNotesTo2"] != null && Request.Form["hdnEmailNotesTo2"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["hdnEmailNotesTo2"]);
                }
                ServiceCallLog.Internal = 1;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;
                ServiceCallLog.IsCallBack = false;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }

                CustomerService.SaveServiceCallLog(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }
                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["hdnEmailNotesTo2"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["hdnEmailNotesTo2"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            //if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>JK</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Customer</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Franachisee</p>";
                            //}
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["txtSpokeWith2"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["txtAction2"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["txtComments2"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion                
            }
            else
            {
                #region :: Call Log  ::
                if (Request.Form["hdnIsCallBack"] != null && Request.Form["hdnIsCallBack"] != "")
                {
                    if (Request.Form["hdnIsCallBack"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack"] != null && Request.Form["txtCallBack"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //ServiceCallLog.CallTime = TMspan;
                        //    ServiceCallLog.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }

                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    ServiceCallLog.InitiatedById = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["SpokeWith"] != null && Request.Form["SpokeWith"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["SpokeWith"];
                }
                if (Request.Form["Action"] != null && Request.Form["Action"] != "")
                {
                    ServiceCallLog.Action = Request.Form["Action"];
                }
                if (Request.Form["Comments"] != null && Request.Form["Comments"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["Comments"];
                }
                if (Request.Form["ServiceLogAreaListId"] != null && Request.Form["ServiceLogAreaListId"] != "")
                {
                    ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(Request.Form["ServiceLogAreaListId"]);
                }
                if (Request.Form["ddlStatusReasonListId"] != null && Request.Form["ddlStatusReasonListId"] != "")
                {
                    ServiceCallLog.StatusReasonListId = Convert.ToInt32(Request.Form["ddlStatusReasonListId"]);
                }                
                if (Request.Form["ServiceLogTypeListId"] != null && Request.Form["ServiceLogTypeListId"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ServiceLogTypeListId"]);
                }
                if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                {
                    ServiceCallLog.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                }
                if (Request.Form["FollowUpBy"] != null && Request.Form["FollowUpBy"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["FollowUpBy"]);
                }
                if (Request.Form["EmailNotesTo"] != null && Request.Form["EmailNotesTo"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["EmailNotesTo"]);
                }
                ServiceCallLog.Internal = 0;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }

                CustomerService.SaveServiceCallLog(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Active;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }

                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["EmailNotesTo"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["EmailNotesTo"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>JK</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 1)
                            {
                                body += "<p><b> Initiated By:</b>Customer</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 2)
                            {
                                body += "<p><b> Initiated By:</b>Franachisee</p>";
                            }
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["SpokeWith"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["Action"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["Comments"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion
            }
            if (Request.Form["hdnCallFrompop"] == "0")
            {
                return RedirectToAction("CustomerServiceCallList", "Customer", new { id = ServiceCallLog.ClassId });
            }
            else if (Request.Form["hdnCallFrompop"] == "1")
            {
                return RedirectToAction("ServiceCallList", "Customer", new { id = ServiceCallLog.ClassId });
            }
            else if (Request.Form["hdnCallFrompop"] == "3")
            {
                return RedirectToAction("SearchCustomerDetails", "CustomerService", new { CustID = ServiceCallLog.ClassId });
            }
            else
            {
                return RedirectToAction("ServiceCallList", "Customer", new { id = ServiceCallLog.ClassId });
            }

        }

        [HttpPost]
        public ActionResult ServiceCallLogPopupCustomerDetails(ServiceCallLog ServiceCallLog)
        {
            //if (CallBack != null && CallBack != "")
            //{
            //    //DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            //    DateTime dtCallBack = Convert.ToDateTime(CallBack);
            //    ServiceCallLog.CallBack = dtCallBack;
            //}
            //ServiceCallLog.CallDate = DateTime.Now;
            //ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

            if (Request.Form["hdnActiveType"] == "2")
            {
                #region :: Internal Office Notes  ::
                if (Request.Form["hdnIsCallBack2"] != null && Request.Form["hdnIsCallBack2"] != "")
                {
                    if (Request.Form["hdnIsCallBack2"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack2"] != null && Request.Form["txtCallBack2"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack2"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //ServiceCallLog.CallTime = TMspan;
                        //    ServiceCallLog.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["txtSpokeWith2"] != null && Request.Form["txtSpokeWith2"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["txtSpokeWith2"];
                }
                if (Request.Form["txtAction2"] != null && Request.Form["txtAction2"] != "")
                {
                    ServiceCallLog.Action = Request.Form["txtAction2"];
                }
                if (Request.Form["txtComments2"] != null && Request.Form["txtComments2"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["txtComments2"];
                }
                if (Request.Form["ddlServiceLogTypeListId2"] != null && Request.Form["ddlServiceLogTypeListId2"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ddlServiceLogTypeListId2"]);
                }
                if (Request.Form["ddlFollowUpBy2"] != null && Request.Form["ddlFollowUpBy2"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["ddlFollowUpBy2"]);
                }
                if (Request.Form["hdnEmailNotesTo2"] != null && Request.Form["hdnEmailNotesTo2"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["hdnEmailNotesTo2"]);
                }
                ServiceCallLog.Internal = 1;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }
                CustomerService.SaveServiceCallLog(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }
                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["hdnEmailNotesTo2"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["hdnEmailNotesTo2"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            //if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>JK</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Customer</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Franachisee</p>";
                            //}
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["txtSpokeWith2"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["txtAction2"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["txtComments2"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion                
            }
            else
            {
                #region :: Call Log  ::
                if (Request.Form["hdnIsCallBack"] != null && Request.Form["hdnIsCallBack"] != "")
                {
                    if (Request.Form["hdnIsCallBack"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack"] != null && Request.Form["txtCallBack"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack"]);
                        }
                        if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        {
                            TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                            //ServiceCallLog.CallTime = TMspan;
                            ServiceCallLog.CallBackTime = TMspan;
                        }
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }

                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    ServiceCallLog.InitiatedById = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["SpokeWith"] != null && Request.Form["SpokeWith"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["SpokeWith"];
                }
                if (Request.Form["Action"] != null && Request.Form["Action"] != "")
                {
                    ServiceCallLog.Action = Request.Form["Action"];
                }
                if (Request.Form["Comments"] != null && Request.Form["Comments"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["Comments"];
                }
                if (Request.Form["ServiceLogAreaListId"] != null && Request.Form["ServiceLogAreaListId"] != "")
                {
                    ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(Request.Form["ServiceLogAreaListId"]);
                }
                if (Request.Form["ServiceLogTypeListId"] != null && Request.Form["ServiceLogTypeListId"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ServiceLogTypeListId"]);
                }
                if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                {
                    ServiceCallLog.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                }
                if (Request.Form["FollowUpBy"] != null && Request.Form["FollowUpBy"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["FollowUpBy"]);
                }
                if (Request.Form["EmailNotesTo"] != null && Request.Form["EmailNotesTo"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["EmailNotesTo"]);
                }
                ServiceCallLog.Internal = 0;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }
                CustomerService.SaveServiceCallLog(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Active;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }

                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["EmailNotesTo"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["EmailNotesTo"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>JK</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Customer</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Franachisee</p>";
                            }
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["SpokeWith"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["Action"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["Comments"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion
            }

            return Json(ServiceCallLog.ClassId, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult EditServiceCallLogDetailsPopupData(ServiceCallLog ServiceCallLog)
        {
            int? StatusListid = ServiceCallLog.StatusListId;
            if (ServiceCallLog.ServiceCallLogId > 0)
            {
                ServiceCallLog = CustomerService.GetServiceCallLogById(ServiceCallLog.ServiceCallLogId);
            }
            
            //if (CallBack != null && CallBack != "")
            //{
            //    //DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            //    DateTime dtCallBack = Convert.ToDateTime(CallBack);
            //    ServiceCallLog.CallBack = dtCallBack;
            //}
            //ServiceCallLog.CallDate = DateTime.Now;
            //ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

            if (Request.Form["hdnActiveType"] == "2")
            {
                #region :: Internal Office Notes  ::
                if (Request.Form["hdnIsCallBack2"] != null && Request.Form["hdnIsCallBack2"] != "")
                {
                    if (Request.Form["hdnIsCallBack2"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack2"] != null && Request.Form["txtCallBack2"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack2"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //ServiceCallLog.CallTime = TMspan;
                        //    ServiceCallLog.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["txtSpokeWith2"] != null && Request.Form["txtSpokeWith2"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["txtSpokeWith2"];
                }
                if (Request.Form["txtAction2"] != null && Request.Form["txtAction2"] != "")
                {
                    ServiceCallLog.Action = Request.Form["txtAction2"];
                }
                if (Request.Form["txtComments2"] != null && Request.Form["txtComments2"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["txtComments2"];
                }
                if (Request.Form["ddlServiceLogTypeListId2"] != null && Request.Form["ddlServiceLogTypeListId2"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ddlServiceLogTypeListId2"]);
                }
                if (Request.Form["ddlFollowUpBy2"] != null && Request.Form["ddlFollowUpBy2"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["ddlFollowUpBy2"]);
                }
                if (Request.Form["hdnEmailNotesTo2"] != null && Request.Form["hdnEmailNotesTo2"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["hdnEmailNotesTo2"]);
                }
                ServiceCallLog.Internal = 1;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }

                CustomerService.UpdateServiceCallLogDetails(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }
                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["hdnEmailNotesTo2"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["hdnEmailNotesTo2"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            //if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>JK</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Customer</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Franachisee</p>";
                            //}
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["txtSpokeWith2"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["txtAction2"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["txtComments2"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion                
            }
            else
            {
                #region :: Call Log  ::
                if (Request.Form["hdnIsCallBack"] != null && Request.Form["hdnIsCallBack"] != "")
                {
                    if (Request.Form["hdnIsCallBack"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack"] != null && Request.Form["txtCallBack"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack"]);
                        }
                        if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        {
                            TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                            //ServiceCallLog.CallTime = TMspan;
                            ServiceCallLog.CallBackTime = TMspan;
                        }
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }

                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    ServiceCallLog.InitiatedById = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["SpokeWith"] != null && Request.Form["SpokeWith"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["SpokeWith"];
                }
                if (Request.Form["Action"] != null && Request.Form["Action"] != "")
                {
                    ServiceCallLog.Action = Request.Form["Action"];
                }
                if (Request.Form["Comments"] != null && Request.Form["Comments"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["Comments"];
                }
                if (Request.Form["ServiceLogAreaListId"] != null && Request.Form["ServiceLogAreaListId"] != "")
                {
                    ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(Request.Form["ServiceLogAreaListId"]);
                }
                if (Request.Form["ServiceLogTypeListId"] != null && Request.Form["ServiceLogTypeListId"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ServiceLogTypeListId"]);
                }
                if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                {
                    ServiceCallLog.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                }
                if (Request.Form["FollowUpBy"] != null && Request.Form["FollowUpBy"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["FollowUpBy"]);
                }
                if (Request.Form["EmailNotesTo"] != null && Request.Form["EmailNotesTo"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["EmailNotesTo"]);
                }
                ServiceCallLog.Internal = 0;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                //Comments by Rakesh due to StatusListId update By UI before this it was ServiceCallLogTypeList

                //if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                //{
                //    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                //}
                //else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                //{
                //    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                //}

                ServiceCallLog.StatusListId = StatusListid;
                CustomerService.UpdateServiceCallLogDetails(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Active;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }

                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["EmailNotesTo"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["EmailNotesTo"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>JK</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Customer</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Franachisee</p>";
                            }
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["SpokeWith"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["Action"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["Comments"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion
            }
            return Json(ServiceCallLog.ClassId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditServiceCallLogDetailsUpdatePopupData(ServiceCallLog ServiceCallLog)
        {
            int? StatusListid = ServiceCallLog.StatusListId;
            if (ServiceCallLog.ServiceCallLogId > 0)
            {
                ServiceCallLog = CustomerService.GetServiceCallLogById(ServiceCallLog.ServiceCallLogId);
            }

            //if (CallBack != null && CallBack != "")
            //{
            //    //DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
            //    DateTime dtCallBack = Convert.ToDateTime(CallBack);
            //    ServiceCallLog.CallBack = dtCallBack;
            //}
            //ServiceCallLog.CallDate = DateTime.Now;
            //ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

            if (Request.Form["hdnActiveType"] == "2")
            {
                #region :: Internal Office Notes  ::
                if (Request.Form["hdnIsCallBack2"] != null && Request.Form["hdnIsCallBack2"] != "")
                {
                    if (Request.Form["hdnIsCallBack2"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack2"] != null && Request.Form["txtCallBack2"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack2"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //ServiceCallLog.CallTime = TMspan;
                        //    ServiceCallLog.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["txtSpokeWith2"] != null && Request.Form["txtSpokeWith2"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["txtSpokeWith2"];
                }
                if (Request.Form["txtAction2"] != null && Request.Form["txtAction2"] != "")
                {
                    ServiceCallLog.Action = Request.Form["txtAction2"];
                }
                if (Request.Form["txtComments2"] != null && Request.Form["txtComments2"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["txtComments2"];
                }
                if (Request.Form["ddlServiceLogTypeListId2"] != null && Request.Form["ddlServiceLogTypeListId2"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ddlServiceLogTypeListId2"]);
                }
                if (Request.Form["ddlFollowUpBy2"] != null && Request.Form["ddlFollowUpBy2"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["ddlFollowUpBy2"]);
                }
                if (Request.Form["hdnEmailNotesTo2"] != null && Request.Form["hdnEmailNotesTo2"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["hdnEmailNotesTo2"]);
                }
                ServiceCallLog.Internal = 1;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                }

                CustomerService.ServiceCallLogDetailsUpdatePopup(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }
                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["hdnEmailNotesTo2"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["hdnEmailNotesTo2"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            //if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>JK</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Customer</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>Franachisee</p>";
                            //}
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["txtSpokeWith2"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["txtAction2"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["txtComments2"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion                
            }
            else
            {
                #region :: Call Log  ::
                if (Request.Form["hdnIsCallBack"] != null && Request.Form["hdnIsCallBack"] != "")
                {
                    if (Request.Form["hdnIsCallBack"] == "1")
                    {
                        ServiceCallLog.IsCallBack = true;
                        if (Request.Form["txtCallBack"] != null && Request.Form["txtCallBack"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            ServiceCallLog.CallBack = Convert.ToDateTime(Request.Form["txtCallBack"]);
                        }
                        if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        {
                            TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                            //ServiceCallLog.CallTime = TMspan;
                            ServiceCallLog.CallBackTime = TMspan;
                        }
                    }
                    else
                    {
                        ServiceCallLog.IsCallBack = false;
                    }
                }
                else
                {
                    ServiceCallLog.IsCallBack = false;
                }

                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    ServiceCallLog.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    ServiceCallLog.InitiatedById = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    ServiceCallLog.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    ServiceCallLog.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["SpokeWith"] != null && Request.Form["SpokeWith"] != "")
                {
                    ServiceCallLog.SpokeWith = Request.Form["SpokeWith"];
                }
                if (Request.Form["Action"] != null && Request.Form["Action"] != "")
                {
                    ServiceCallLog.Action = Request.Form["Action"];
                }
                if (Request.Form["Comments"] != null && Request.Form["Comments"] != "")
                {
                    ServiceCallLog.Comments = Request.Form["Comments"];
                }
                if (Request.Form["ServiceLogAreaListId"] != null && Request.Form["ServiceLogAreaListId"] != "")
                {
                    ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(Request.Form["ServiceLogAreaListId"]);
                }
                if (Request.Form["ServiceLogTypeListId"] != null && Request.Form["ServiceLogTypeListId"] != "")
                {
                    ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(Request.Form["ServiceLogTypeListId"]);
                }
                if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                {
                    ServiceCallLog.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                }
                if (Request.Form["FollowUpBy"] != null && Request.Form["FollowUpBy"] != "")
                {
                    ServiceCallLog.FollowUpBy = Convert.ToInt32(Request.Form["FollowUpBy"]);
                }
                if (Request.Form["EmailNotesTo"] != null && Request.Form["EmailNotesTo"] != "")
                {
                    ServiceCallLog.EmailNotesTo = Convert.ToString(Request.Form["EmailNotesTo"]);
                }
                ServiceCallLog.Internal = 0;
                ServiceCallLog.CallDate = DateTime.Now;
                ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

                //Comments by Rakesh due to StatusListId update By UI before this it was ServiceCallLogTypeList

                //if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.Complaint)
                //{
                //    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Open;
                //}
                //else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                //{
                //    ServiceCallLog.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                //}

                ServiceCallLog.StatusListId = StatusListid;
                CustomerService.ServiceCallLogDetailsUpdatePopup(ServiceCallLog);

                //update customer status if status select as 'Pending Cancellation Type(Id=11) '      
                if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.PendingCancellation)
                {
                    //Status model = new Status();
                    //model.ClassId = ServiceCallLog.ClassId;
                    //model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.CancallationPending;
                    //model.StatusDate = DateTime.Now;
                    //model.StatusNotes = ServiceCallLog.Comments;
                    //CustomerService.UpdateCustomerStatus(model);
                }
                else if (ServiceCallLog.ServiceLogTypeListId == (int)JKApi.Business.Enumeration.ServiceCallLogTypeList.ClearAtRisk)
                {
                    Status model = new Status();
                    model.ClassId = ServiceCallLog.ClassId;
                    model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Active;
                    model.StatusDate = DateTime.Now;
                    model.StatusNotes = ServiceCallLog.Comments;
                    CustomerService.UpdateCustomerStatus(model);
                }

                #endregion

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["EmailNotesTo"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["EmailNotesTo"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Service Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>JK</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Customer</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>Franachisee</p>";
                            }
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["SpokeWith"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["Action"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["Comments"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion
            }
            return Json(ServiceCallLog.ClassId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CollectionsCallLog(ServiceCallLog ServiceCallLog, string CallBack)
        {
            CollectionsCallLogModel collectionCallLogModel = new CollectionsCallLogModel();
            if (Request.Form["hdnActiveType"] == "2")
            {
                #region :: Internal Office Notes  ::

                if (Request.Form["hdnIsCallBack2"] != null && Request.Form["hdnIsCallBack2"] != "")
                {
                    if (Request.Form["hdnIsCallBack2"] == "1")
                    {
                        collectionCallLogModel.IsCallBack = true;
                        if (Request.Form["txtCallBack2"] != null && Request.Form["txtCallBack2"] != "")
                        {
                            //collectionCallLogModel.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            collectionCallLogModel.CallBack = Convert.ToDateTime(Request.Form["txtCallBack2"]);
                        }
                        //if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        //{
                        //    TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                        //    //collectionCallLogModel.CallTime = TMspan;
                        //    collectionCallLogModel.CallBackTime = TMspan;
                        //}
                    }
                    else
                    {
                        collectionCallLogModel.IsCallBack = false;
                    }
                }
                else
                {
                    collectionCallLogModel.IsCallBack = false;
                }


                collectionCallLogModel.CallDate = DateTime.Now;
                collectionCallLogModel.CallTime = DateTime.Now.ToString("HH:mm:ss");
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    collectionCallLogModel.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    collectionCallLogModel.InitiatedBy = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    collectionCallLogModel.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    collectionCallLogModel.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["txtSpokeWith2"] != null && Request.Form["txtSpokeWith2"] != "")
                {
                    collectionCallLogModel.SpokeWith = Request.Form["txtSpokeWith2"];
                }
                if (Request.Form["txtAction2"] != null && Request.Form["txtAction2"] != "")
                {
                    collectionCallLogModel.Action = Request.Form["txtAction2"];
                }
                if (Request.Form["txtComments2"] != null && Request.Form["txtComments2"] != "")
                {
                    collectionCallLogModel.Comments = Request.Form["txtComments2"];
                }
                if (Request.Form["ddlServiceLogTypeListId2"] != null && Request.Form["ddlServiceLogTypeListId2"] != "")
                {
                    collectionCallLogModel.CallLogInitiatedByTypeListId = Convert.ToInt32(Request.Form["ddlServiceLogTypeListId2"]);
                }
                //if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                //{
                //    collectionCallLogModel.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                //}
                if (Request.Form["ddlFollowUpBy2"] != null && Request.Form["ddlFollowUpBy2"] != "")
                {
                    collectionCallLogModel.FollowUpBy = Convert.ToInt32(Request.Form["ddlFollowUpBy2"]);
                }
                if (Request.Form["hdnEmailNotesTo2"] != null && Request.Form["hdnEmailNotesTo2"] != "")
                {
                    collectionCallLogModel.EmailNotesTo = Convert.ToString(Request.Form["hdnEmailNotesTo2"]);
                }
                //if (Request.Form["boolInternal"] != null && Request.Form["boolInternal"] != "")
                //{
                //    collectionCallLogModel.boolInternal = Convert.ToBoolean(Request.Form["boolInternal"] == "true" ? true : false);
                //}
                collectionCallLogModel.Internal = 1;

                CustomerService.SaveCollectionCallLog(collectionCallLogModel);

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["hdnEmailNotesTo2"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["hdnEmailNotesTo2"]).Split(',');
                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Collection Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            //if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            //{
                            //    body += "<p><b> Initiated By:</b>JK</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 1)
                            //{
                            //    body += "<p><b> Initiated By:</b>Customer</p>";
                            //}
                            //else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 2)
                            //{
                            //    body += "<p><b> Initiated By:</b>Franachisee</p>";
                            //}
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["txtSpokeWith2"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["txtAction2"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["txtComments2"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion

                #endregion
            }
            else
            {
                #region :: Call Log  :: 
                if (Request.Form["hdnIsCallBack"] != null && Request.Form["hdnIsCallBack"] != "")
                {
                    if (Request.Form["hdnIsCallBack"] == "1")
                    {
                        collectionCallLogModel.IsCallBack = true;
                        if (Request.Form["txtCallBack"] != null && Request.Form["txtCallBack"] != "")
                        {
                            //ServiceCallLog.CallDate = Convert.ToDateTime(Request.Form["txtCallBack"]);
                            collectionCallLogModel.CallBack = Convert.ToDateTime(Request.Form["txtCallBack"]);
                        }
                        if (Request.Form["CallTime"] != null && Request.Form["CallTime"] != "")
                        {
                            TimeSpan TMspan = DateTime.Parse(Request.Form["CallTime"]).TimeOfDay;
                            //ServiceCallLog.CallTime = TMspan;
                            collectionCallLogModel.CallBackTime = TMspan;
                        }
                    }
                    else
                    {
                        collectionCallLogModel.IsCallBack = false;
                    }
                }
                else
                {
                    collectionCallLogModel.IsCallBack = false;
                }
                //if (CallBack != null && CallBack != "")
                //{
                //    //DateTime dtCallBack = DateTime.ParseExact(CallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                //    DateTime dtCallBack = Convert.ToDateTime(CallBack);
                //    collectionCallLogModel.CallBack = dtCallBack;
                //}
                collectionCallLogModel.CallDate = DateTime.Now;
                collectionCallLogModel.CallTime = DateTime.Now.ToString("HH:mm:ss");
                if (Request.Form["ClassId"] != null && Request.Form["ClassId"] != "")
                {
                    collectionCallLogModel.ClassId = Convert.ToInt32(Request.Form["ClassId"]);
                }
                if (Request.Form["InitiatedBy"] != null && Request.Form["InitiatedBy"] != "")
                {
                    collectionCallLogModel.InitiatedBy = Convert.ToInt32(Request.Form["InitiatedBy"]);
                }
                if (Request.Form["TypeListId"] != null && Request.Form["TypeListId"] != "")
                {
                    collectionCallLogModel.TypeListId = Convert.ToInt32(Request.Form["TypeListId"]);
                }
                if (Request.Form["RegionId"] != null && Request.Form["RegionId"] != "")
                {
                    collectionCallLogModel.RegionId = Convert.ToInt32(Request.Form["RegionId"]);
                }
                if (Request.Form["SpokeWith"] != null && Request.Form["SpokeWith"] != "")
                {
                    collectionCallLogModel.SpokeWith = Request.Form["SpokeWith"];
                }
                if (Request.Form["Action"] != null && Request.Form["Action"] != "")
                {
                    collectionCallLogModel.Action = Request.Form["Action"];
                }
                if (Request.Form["Comments"] != null && Request.Form["Comments"] != "")
                {
                    collectionCallLogModel.Comments = Request.Form["Comments"];
                }
                if (Request.Form["ServiceLogTypeListId"] != null && Request.Form["ServiceLogTypeListId"] != "")
                {
                    collectionCallLogModel.CallLogInitiatedByTypeListId = Convert.ToInt32(Request.Form["ServiceLogTypeListId"]);
                }
                if (Request.Form["StatusResultListId"] != null && Request.Form["StatusResultListId"] != "")
                {
                    collectionCallLogModel.StatusResultListId = Convert.ToInt32(Request.Form["StatusResultListId"]);
                }
                if (Request.Form["FollowUpBy"] != null && Request.Form["FollowUpBy"] != "")
                {
                    collectionCallLogModel.FollowUpBy = Convert.ToInt32(Request.Form["FollowUpBy"]);
                }
                if (Request.Form["EmailNotesTo"] != null && Request.Form["EmailNotesTo"] != "")
                {
                    collectionCallLogModel.EmailNotesTo = Convert.ToString(Request.Form["EmailNotesTo"]);
                }
                //if (Request.Form["boolInternal"] != null && Request.Form["boolInternal"] != "")
                //{
                //    collectionCallLogModel.boolInternal = Convert.ToBoolean(Request.Form["boolInternal"] == "true" ? true : false);
                //}
                collectionCallLogModel.Internal = 0;
                CustomerService.SaveCollectionCallLog(collectionCallLogModel);

                #region :: Send Email ::

                string EmailTo = string.Empty;
                var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

                if (userList.Count > 0 && Request.Form["EmailNotesTo"] != null)
                {
                    string[] userIds = Convert.ToString(Request.Form["EmailNotesTo"]).Split(',');

                    for (int i = 0; i < userIds.Count(); i++)
                    {
                        if (userIds[i] != "")
                        {
                            int UserId = Convert.ToInt32(userIds[i]);

                            EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                            string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                            string subject = "Collection Call Log Details";
                            string body = "";
                            body += "<div style=\"width:600px;padding:15px;\">";
                            body += "<p>Hi " + DisplayName + "</p>";

                            body += "<p><b>Service call log details</b></p>";
                            if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 0)
                            {
                                body += "<p><b> Initiated By:</b>JK</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 1)
                            {
                                body += "<p><b> Initiated By:</b>Customer</p>";
                            }
                            else if (Convert.ToInt32(Request.Form["InitiatedBy"]) == 2)
                            {
                                body += "<p><b> Initiated By:</b>Franachisee</p>";
                            }
                            body += "<p><b>SpokeWith:</b>" + Convert.ToString(Request.Form["SpokeWith"]) + "</p>";
                            body += "<p><b>Action:</b>" + Convert.ToString(Request.Form["Action"]) + "</p>";
                            body += "<p><b>Comments:</b>" + Convert.ToString(Request.Form["Comments"]) + "</p>";
                            body += "<p>Thanks You</p>";
                            body += "</div>";

                            _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
                        }
                    }
                }

                #endregion

                #endregion 
            }
            return RedirectToAction("CollectionCallList", "Customer", new { id = collectionCallLogModel.ClassId });
        }

        public JsonResult GetCustomerDataByID(string CustomerID)
        {
            int id = Convert.ToInt32(CustomerID);
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != 0)
            {
                if (id != 0)
                {
                    var response = jkEntityModel.portal_spGet_CustomerDetail(id);

                    foreach (var a in response.ToList())
                    {
                        customerDetailViewModel = new CustomerDetailViewModel();
                        var customer = CustomerService.GetCustomerById(id);
                        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                        customerDetailViewModel.RegionId = String.IsNullOrEmpty(customer.RegionId.ToString()) ? String.Empty : customer.RegionId.ToString();
                    }
                }
            }
            return Json(customerDetailViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFranchiseeCall(int FranchiseeId, int CustomerId = 0,int callfrom=0)
        {
            ViewBag.FranchiseeId = FranchiseeId;
            int _notesDetailId = 0;
            string _notes = string.Empty;
            var FranchiseeModel = FranchiseeService.GetFranchiseeDetail(FranchiseeId);
            if (FranchiseeModel != null)
            {
                ViewBag.FranchiseeInfo = FranchiseeModel;
                var _notemodel = CustomerService.GetCustomerNotes(Convert.ToInt32(FranchiseeId), SelectedRegionId, (int)JKApi.Business.Enumeration.TypeList.Franchisee);
                if (_notemodel != null)
                {
                    _notesDetailId = Convert.ToInt32(_notemodel.FirstOrDefault().Key);
                    _notes = Convert.ToString(_notemodel.FirstOrDefault().Value);
                }
            }
            ViewBag.custNotes = _notes;
            ViewBag.notesDetailId = _notesDetailId;
            ViewBag.CustomerID = CustomerId;
            ViewBag.callfrom = callfrom;
            if (CustomerId > 0)
            {
                FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(CustomerId));
                ViewBag.CustomerInfo = CustomerViewModel;
            }            
            var model = FranchiseeService.GetFranchiseeCall(FranchiseeId, CustomerId);
            return PartialView("_PartialFranchiseeCall", model);
        }

        [HttpGet]
        public ActionResult GetLogCall(int FranchiseeId, int CustomerId)
        {
            ViewBag.FranchiseeId = FranchiseeId;
            var FranchiseeModel = FranchiseeService.GetFranchiseeDetail(FranchiseeId);
            if (FranchiseeModel != null)
            {
                ViewBag.FranchiseeInfo = FranchiseeModel;
            }
            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();
            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            ViewBag.CustomerId = CustomerId;
            if (CustomerId > 0)
            {
                FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(CustomerId));
                ViewBag.CustomerInfo = CustomerViewModel;
            }

            return PartialView("_PartialLogCall");
        }

       

        //[HttpGet]
        //public JsonResult getCustomerDetailByNo(string customerNo)
        //{
        //    FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
        //    CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        public ActionResult CollectionCallList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CollectionCallList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CollectionCallList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("CollectionCallList", "Customer", new { area = "Portal" }), "Collection Call List");

            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            ViewBag.CustId = id;
            int CollectionsCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CollectionsCallLog = CustomerService.GetServiceCollectionCallLogCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                CollectionsCallLogCount = CustomerService.GetServiceCollectionCallLogCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CollectionsCallLog = CustomerService.GetServiceCollectionCallLogList();
            }
            ViewBag.CollectionsCallLogCount = CollectionsCallLogCount;

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");


            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            if (userLista.Count > 0 && ServiceCallFollowBy != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            if (userLista.Count > 0 && ServiceEmailCall != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }



            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            #region Customer Info

            int CustID = id ?? -1;
            //if (CustID < 0)
            //{
            //    ViewBag.MainStatesList = getUsStatesList();
            //    ViewBag.BillingStatesList = getUsStatesList();
            //    ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            //    ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "term");
            //    ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            //}

            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

            if (CustID > 0)
            {
                //int? CustomerID = CustID;
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
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                //  return View(FullCustomerViewModel);

                if (id > 0)
                {
                    var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(id));
                    if (BillSetting != null)
                    {
                        ViewBag.ARStatus = ((BillSetting.ARStatus != null && BillSetting.ARStatus != 0) ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty); //BillSetting.ARStatus;
                    }

                    var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(id));
                    if (AgingData != null)
                    {
                        ViewBag.AgingData = AgingData;
                    }
                }
            }

            #endregion Customer Info

            return View(FullCustomerViewModel);
        }

        #region HttpRequest  Customer 
        [HttpGet]
        public ActionResult ServiceCallLogList(int? id)
        {

            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();

            ViewBag.CustId = id;
            //int ServiceCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                //ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                //ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            //ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            if (userLista.Count > 0 && ServiceCallFollowBy != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            if (userLista.Count > 0 && ServiceEmailCall != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }



            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            //#region Customer Info

            int CustID = id ?? -1;
            //if (CustID < 0)
            //{
            //    ViewBag.MainStatesList = getUsStatesList();
            //    ViewBag.BillingStatesList = getUsStatesList();
            //    ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            //    ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
            //    ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            //}

            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            if (TransactionsTypeList != null)
            {
                //ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

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
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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
                // FullCustomerViewModel = new FullCustomerViewModel1();
                //FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                //  return View(FullCustomerViewModel);
            }

            //#endregion Customer Info

            //var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
            //ViewBag.Distributions = CustomerDistributions;
            //if (CustomerDistributions.Count > 0)
            //{
            //    ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
            //    ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            //}
            //else
            //{
            //    ViewBag.DistributionFranchiseeId = 0;
            //    ViewBag.DistributionId = 0;
            //}
            //Have issue return this 
            //return PartialView("_CustomerServiceListCallLogPopup",FullCustomerViewModel);
            var data = this.RenderPartialViewToString("_CustomerServiceListCallLogPopup", FullCustomerViewModel);
            return Json(new
            {
                success = true,
                ViewHtml = data
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        [HttpGet]
        public ActionResult ServiceCallList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerService";
            ViewBag.CallFromCustomer = "1";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service Call List");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();

            ViewBag.CustId = id;
            ViewBag.CustomerID = id;
            //int ServiceCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                //ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            //ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            #region Customer Info
            int _notesDetailId = 0;
            string _notes = string.Empty;
            string CleanFrequencyName = string.Empty;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            if (id != 0 && id != null)
            {
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

                var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(id));
                if (BillSetting != null)
                {
                    ViewBag.ARStatus = ((BillSetting.ARStatus != null && BillSetting.ARStatus != 0) ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty); //BillSetting.ARStatus;
                }
                var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(id));
                if (AgingData != null)
                {
                    ViewBag.AgingData = AgingData;
                }
                var _notemodel = CustomerService.GetCustomerNotes(Convert.ToInt32(id), SelectedRegionId, (int)JKApi.Business.Enumeration.TypeList.Customer);
                if (_notemodel != null)
                {
                    _notesDetailId = Convert.ToInt32(_notemodel.FirstOrDefault().Key);
                    _notes = Convert.ToString(_notemodel.FirstOrDefault().Value);
                }
                if (CustomerViewModel.ContractDetail != null)
                {
                    if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                    {
                        var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                        CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                    }
                }
            }
            ViewBag.CustomerDetail = CustomerViewModel;
            ViewBag.custNotes = _notes;
            ViewBag.notesDetailId = _notesDetailId;
            ViewBag.CleanFrequencyName = CleanFrequencyName;

            #endregion Customer Info

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
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
            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
  
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CustomerServicesCallLogStatus);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name");

            return View(FullCustomerViewModel);
        }

        //Customer Service
        [HttpGet]
        public ActionResult CustomerServiceCallList(int id = 0, int flt = 0, int? callFromSchedule = 0)
        {
            ViewBag.HMenu = "CustomerService";
            ViewBag.CurrentMenu = "LogServiceCall";
            ViewBag.CallFromCustomer = "0";
            ViewBag.FilterArea = flt;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LogServiceCall", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("LogServiceCall", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("LogServiceCall", "Customer", new { area = "Portal" }), "Service Call List");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();

            ViewBag.CustId = id;
            ViewBag.CustomerID = id;
            ViewBag.CallFromSchedule = callFromSchedule;
            //int ServiceCallLogCount = 0;
            if (id != 0 )
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id);
                //ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            //ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            #region Customer Info

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            int _notesDetailId = 0;
            string _notes = string.Empty;
            string CleanFrequencyName = string.Empty;
            if (id != 0 && id != null)
            {
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

                var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(id));
                if (BillSetting != null)
                {
                    ViewBag.ARStatus = ((BillSetting.ARStatus != null && BillSetting.ARStatus != 0) ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty); //BillSetting.ARStatus;
                }
                var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(id));
                if (AgingData != null)
                {
                    ViewBag.AgingData = AgingData;
                }
                var _notemodel = CustomerService.GetCustomerNotes(Convert.ToInt32(id), SelectedRegionId, (int)JKApi.Business.Enumeration.TypeList.Customer);
                if (_notemodel != null)
                {
                    _notesDetailId = Convert.ToInt32(_notemodel.FirstOrDefault().Key);
                    _notes = Convert.ToString(_notemodel.FirstOrDefault().Value);
                }
                //strcustnotes = CustomerService.GetCustomerNotes(Convert.ToInt32(id),SelectedRegionId);    
                if (CustomerViewModel.ContractDetail != null)
                {
                    if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                    {
                        var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                        CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                    }
                }

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
            }
            ViewBag.CleanFrequencyName = CleanFrequencyName;
            ViewBag.CustomerDetail = CustomerViewModel;
            ViewBag.custNotes = _notes;
            ViewBag.notesDetailId = _notesDetailId;
            #endregion Customer Info

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
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

            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.CustomerServicesCallLogStatus);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name",67);
            return View("ServiceCallList", FullCustomerViewModel);
        }
        public ActionResult showServiceCallLogPopup(int id, string callfrom = "")
        {

            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            ViewBag.CustomerId = id;
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.CallFrom = callfrom;

            BasicInfoCustomerModel BasicInfoModel = new BasicInfoCustomerModel();
            if (id != 0)
            {
                BasicInfoModel = CustomerService.GetBasicInfoCustomer(Convert.ToInt32(id));
            }
            ViewBag.CustomerDetail = BasicInfoModel;

            //FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            //if (id != 0)
            //{
            //    CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            //}
            //ViewBag.CustomerDetail = CustomerViewModel;


            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            //var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            int CRM_TerritoryId = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Call Follow By");
            if (userLista.Count > 0 && CRM_TerritoryId != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = new SelectList(userLista, "UserID", "Name");
            }

            //var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            int CRM_TerritoryIdEmail = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Email Call");

            if (userLista.Count > 0 && CRM_TerritoryIdEmail != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryIdEmail).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogType = ServiceCallLogTypeList;
            ViewBag.ServiceCallLogTypeList = new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = new SelectList(lstStatusResultList, "StatusResultListId", "Name");

            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(1), "FileTypeListId", "Name");

            var lstStatusReasonList = CustomerService.GetStatusReasonList();
            int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            lstStatusReasonList = lstStatusReasonList.Where(w => w.TypeListId == TypeListId);
            ViewBag.StatusReasonList = new SelectList(lstStatusReasonList, "StatusReasonListId", "Name");

            return PartialView("_PartialServiceCallLogPopup", FullCustomerViewModel);
        }

        [HttpGet]
        public ActionResult ServiceCallsList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerService";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service Call List");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();

            ViewBag.IsCallBack = 0; //Iscallback = false
            ViewBag.CustId = id;
            //int ServiceCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                //ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            //ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            var AllLoginUserList = CustomerService.GetAllLoginUserList();
            if (AllLoginUserList.Count > 0)
            {
                AllLoginUserList.ForEach(x => x.UserName = x.FirstName + " " + x.LastName);
                ViewBag.AllLoginUserList = AddFirstItemInSelecetList(new SelectList(AllLoginUserList.OrderBy(o => o.UserName), "UserID", "UserName"), "Select User");
            }
            else
            {
                ViewBag.AllLoginUserList = new List<AuthUserLogin>();
            }

            #region Customer Info

            int CustID = id ?? -1;
            if (CustID < 0)
            {
                ViewBag.MainStatesList = getUsStatesList();
                ViewBag.BillingStatesList = getUsStatesList();
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

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
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                //  return View(FullCustomerViewModel);
            }

            #endregion Customer Info

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }


            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
            ViewBag.selectedRegionId = SelectedRegionId;

            return View(FullCustomerViewModel);
        }

        [HttpGet]
        public ActionResult CustomerServices(int? id)
        {
            ViewBag.HMenu = "CustomerService";
            ViewBag.CurrentMenu = "ServiceCallsList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service Call List");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            ViewBag.IsCallBack = 0; //Iscallback = false
            ViewBag.CustId = id;
            int ServiceCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            var AllLoginUserList = CustomerService.GetAllLoginUserList();
            if (AllLoginUserList.Count > 0)
            {
                AllLoginUserList.ForEach(x => x.UserName = x.FirstName + " " + x.LastName);
                ViewBag.AllLoginUserList = AddFirstItemInSelecetList(new SelectList(AllLoginUserList.OrderBy(o => o.UserName), "UserID", "UserName"), "Select User");
            }
            else
            {
                ViewBag.AllLoginUserList = new List<AuthUserLogin>();
            }
            #region Customer Info

            int CustID = id ?? -1;
            if (CustID < 0)
            {
                ViewBag.MainStatesList = getUsStatesList();
                ViewBag.BillingStatesList = getUsStatesList();
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

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
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                //  return View(FullCustomerViewModel);
            }

            #endregion Customer Info

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }


            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
            ViewBag.selectedRegionId = SelectedRegionId;

            return View("ServiceCallsList", FullCustomerViewModel);
        }

        [HttpGet]
        public ActionResult ServiceCallBackList(int? id)
        {
            ViewBag.HMenu = "CustomerService";
            ViewBag.CurrentMenu = "ServiceCallsList";
            Session["callFrom"] = 1;
            Session["SearchCustomerIds"] = null;
            Session["SaershResultURL"] = null;
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service");
            BreadCrumb.Add(Url.Action("ServiceCallList", "Customer", new { area = "Portal" }), "Service Call List");
            //int CustomerId = Convert.ToInt32(TempData["CustomerId"]);
            //ViewBag.CustomerDetail = TempData["CustomerInformation"];
            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            ViewBag.IsCallBack = 1; //Iscallback = true

            ViewBag.CustId = id;
            int ServiceCallLogCount = 0;
            if (id != 0 && id != null)
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0);
                ViewBag.CustomerInvoice = CustomerService.GetCustomerInvoice(id ?? 0);
                ServiceCallLogCount = CustomerService.GetServiceCallLogParticularCustomersList(id ?? 0).Count();
            }
            else
            {
                //ViewBag.CustomerServiceCallLog = CustomerService.GetServiceCallLogLists();
            }
            ViewBag.ServiceCallLogCount = ServiceCallLogCount;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            var AllLoginUserList = CustomerService.GetAllLoginUserList();
            if (AllLoginUserList.Count > 0)
            {
                AllLoginUserList.ForEach(x => x.UserName = x.FirstName + " " + x.LastName);
                ViewBag.AllLoginUserList = AddFirstItemInSelecetList(new SelectList(AllLoginUserList.OrderBy(o => o.UserName), "UserID", "UserName"), "Select User");
            }
            else
            {
                ViewBag.AllLoginUserList = new List<AuthUserLogin>();
            }
            #region Customer Info

            int CustID = id ?? -1;
            if (CustID < 0)
            {
                ViewBag.MainStatesList = getUsStatesList();
                ViewBag.BillingStatesList = getUsStatesList();
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            int? CustomerID = CustID;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

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
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    if (a.CustomerNo != null)
                    {
                        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopups(CustID);

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                //  return View(FullCustomerViewModel);
            }

            #endregion Customer Info

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }


            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
            ViewBag.selectedRegionId = SelectedRegionId;

            ViewBag.ServiceStatuslist = new SelectList(jkEntityModel.StatusLists.Where(x => x.TypeListId == 19).ToList(), "StatusListId", "Name", 67);

            return View("~/Areas/Portal/Views/CustomerService/ServiceCallBackList.cshtml", FullCustomerViewModel);
        }

        [HttpGet]
        public JsonResult ServiceCallListResultData(string regionId, string fd = "", string td = "", string st = "", string tp = "", int ib = 0, int usr = 0, int iscallback = 0)
        {
            DateTime fromDate = DateTime.Parse(fd.ToString());
            DateTime toDate = DateTime.Parse(td.ToString());
            var list = CustomerService.GetServiceCallListForSearch(regionId, fromDate, toDate, st, tp, ib, usr, iscallback);

            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult ServiceCallbackListResultData(string regionId, string fd = "", string td = "", string st = "", string tp = "", int ib = 0, int usr = 0, int iscallback = 0, string fltdate = "", string ServiceStatuslist = "")
        {
            //DateTime fromDate = DateTime.Parse(fd.ToString());
            // DateTime toDate = DateTime.Parse(td.ToString());
            var list = CustomerService.GetServiceCallbackListForSearch(regionId, DateTime.Now, DateTime.Now, st, tp, ib, usr, iscallback, fltdate, ServiceStatuslist);

            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;

            return PartialView("~/Areas/Portal/Views/CustomerService/_PartialServiceCallBackList.cshtml", list);
        }

        public ActionResult ServiceCallLogDetailsPopup(int id = 0, int serviceCallId = 0,int iscbk= 0)
        {

            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            var dataModel = CustomerService.GetServiceCallLogCustomersListForSearch(serviceCallId);
                        
            if (dataModel != null)
            {
                FullCustomerViewModel = dataModel;
            }

            ViewBag.ServiceStatuslist = new SelectList(jkEntityModel.StatusLists.Where(x => x.TypeListId == 19).ToList(), "StatusListId", "Name", dataModel.StatusListId);

            ViewBag.CustomerId = id;
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.CallFrom = "";
            ViewBag.iscbk = iscbk;

            BasicInfoCustomerModel BasicInfoModel = new BasicInfoCustomerModel();
            if (id != 0)
            {
                BasicInfoModel = CustomerService.GetBasicInfoCustomer(Convert.ToInt32(id));
            }
            ViewBag.CustomerDetail = BasicInfoModel;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            //var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            int CRM_TerritoryId = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Call Follow By");
            if (userLista.Count > 0 && CRM_TerritoryId != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = new SelectList(userLista, "UserID", "Name");
            }

            //var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            int CRM_TerritoryIdEmail = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Email Call");

            if (userLista.Count > 0 && CRM_TerritoryIdEmail != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryIdEmail).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = new SelectList(lstStatusResultList, "StatusResultListId", "Name");

            return PartialView("~/Areas/Portal/Views/CustomerService/_PartialServiceCallLogDetailsPopup.cshtml", FullCustomerViewModel);
        }
        public ActionResult ServiceCallLogDetailsUpdatePopup(int id = 0, int serviceCallId = 0, int iscbk = 0)
        {

            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            var dataModel = CustomerService.GetServiceCallLogCustomersListForSearch(serviceCallId);

            if (dataModel != null)
            {
                FullCustomerViewModel = dataModel;
            }

            ViewBag.ServiceStatuslist = new SelectList(jkEntityModel.StatusLists.Where(x => x.TypeListId == 19).ToList(), "StatusListId", "Name", dataModel.StatusListId);

            ViewBag.CustomerId = id;
            ViewBag.RegionId = SelectedRegionId;
            ViewBag.CallFrom = "";
            ViewBag.iscbk = iscbk;

            BasicInfoCustomerModel BasicInfoModel = new BasicInfoCustomerModel();
            if (id != 0)
            {
                BasicInfoModel = CustomerService.GetBasicInfoCustomer(Convert.ToInt32(id));
            }
            ViewBag.CustomerDetail = BasicInfoModel;

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            //var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            int CRM_TerritoryId = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Call Follow By");
            if (userLista.Count > 0 && CRM_TerritoryId != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = new SelectList(userLista, "UserID", "Name");
            }

            //var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            int CRM_TerritoryIdEmail = _crmService.GetCRMTerritoryId(SelectedRegionId, "Service Email Call");

            if (userLista.Count > 0 && CRM_TerritoryIdEmail != 0)
            {
                //var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == CRM_TerritoryIdEmail).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = new SelectList(lstStatusResultList, "StatusResultListId", "Name");

            FullCustomerViewModel.SpokeWith = "";
            FullCustomerViewModel.Action = "";
            FullCustomerViewModel.Comments = "";
            FullCustomerViewModel.StatusResultListId = null;
            return PartialView("~/Areas/Portal/Views/CustomerService/_PartialServiceCallLogDetailsUpdatePopup.cshtml", FullCustomerViewModel);
        }
        private ServiceCallLogModel Maintennacepopups(int id)
        {
            int currentId = 0;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsByIdWithActive(Convert.ToInt32(id));

            ViewBag.Address1 = string.Empty;
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += "" + StateName;
                }

                ViewBag.Address = Address;

                if (Address.Trim().ToLower() == "newyork")
                {
                    Address = "NY";
                }
            }

            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.TaxAuthority = getTaxAuthority();

            int ARStatusReasonListId = 0;
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null)
            {
                var ARStatu = CustomerService.GetARStatu().Where(w => w.BillSettingsId == CustomerViewModel.BillSetting.BillSettingsId);
                if (ARStatu != null && ARStatu.Count() > 0)
                {
                    ARStatusReasonListId = (ARStatu.FirstOrDefault().ARStatusReasonListId.HasValue ? ARStatu.FirstOrDefault().ARStatusReasonListId.Value : 0);
                }
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null && CustomerViewModel.MainAddress.StateListId != null)
            {
                ViewBag.MainStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name", CustomerViewModel.MainAddress.StateListId);
            }
            else
            {
                ViewBag.MainStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillingAddress != null && CustomerViewModel.BillingAddress.StateListId != null)
            {
                ViewBag.BillingStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name", CustomerViewModel.BillingAddress.StateListId);
            }
            else
            {
                ViewBag.BillingStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            }

            // if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && ARStatusReasonListId != 0)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", ARStatusReasonListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term", CustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "Id", "term");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");

            ServiceCallLogModel fullcustomer = new ServiceCallLogModel();

            fullcustomer.CustomerViewModel = CustomerViewModel.CustomerViewModel;
            fullcustomer.BillSetting = CustomerViewModel.BillSetting;
            fullcustomer.MainAddress = CustomerViewModel.MainAddress;
            fullcustomer.MainContact = CustomerViewModel.MainContact;
            fullcustomer.MainEmail = CustomerViewModel.MainEmail;
            fullcustomer.CustomerDetail = CustomerViewModel.CustomerDetail;
            fullcustomer.MainPhone = CustomerViewModel.MainPhone;
            fullcustomer.BillingContact = CustomerViewModel.BillingContact;
            fullcustomer.BillingAddress = CustomerViewModel.BillingAddress;
            fullcustomer.BillingPhone = CustomerViewModel.BillingPhone;
            fullcustomer.BillingEmail = CustomerViewModel.BillingEmail;
            fullcustomer.ContactInformation = CustomerViewModel.ContactInformation;
            fullcustomer.ContactInformationPhone = CustomerViewModel.ContactInformationPhone;
            fullcustomer.ContactInformationEmail = CustomerViewModel.ContactInformationEmail;
            fullcustomer.BillingContact = CustomerViewModel.BillingContact;
            fullcustomer.BillingInformationPhone = CustomerViewModel.BillingInformationPhone;
            fullcustomer.BillingInformationEmail = CustomerViewModel.BillingInformationEmail;
            fullcustomer.BillingInformation = CustomerViewModel.BillingInformation;
            fullcustomer.BillingInformationAddress = CustomerViewModel.BillingInformationAddress;

            fullcustomer.MainPhone2 = CustomerViewModel.MainPhone2;
            fullcustomer.BillingContactInformation2 = CustomerViewModel.BillingContactInformation2;
            fullcustomer.lstEBillEmails = CustomerViewModel.lstEBillEmails;
            if (fullcustomer.lstEBillEmails == null)
                fullcustomer.lstEBillEmails = new List<EmailViewModel>();
            foreach (EmailViewModel op in fullcustomer.lstEBillEmails)
            {
                fullcustomer.EBill_Emails += op.EmailAddress + ",";
            }
            if (fullcustomer.EBill_Emails.Length > 0)
                fullcustomer.EBill_Emails = fullcustomer.EBill_Emails.Trim(',');

            return fullcustomer;
        }

        #endregion Customer > Service

        #region Customer > Reports

        public ActionResult Distribution()
        {
            ViewBag.CurrentMenu = "Distribution";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Distribution", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("Distribution", "Customer", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("Distribution", "Customer", new { area = "Portal" }), "Distribution");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public JsonResult DistributionReportData(string regionId = "", string searchtext = "", int status = 0)
        {
            var list = CustomerService.DistributionReportList(searchtext, Convert.ToInt32(status), regionId);
            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult NewCustomersReportData(int regId = 0, string st = "", string sd = "", string ed = "")
        {
            var list = CustomerService.NewCustomersReportList(st, 0, sd, ed, regId, "");
            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult IncreaseDecreaseReport()
        {
            ViewBag.CurrentMenu = "IncreaseDecreaseReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("IncreaseDecreaseReport", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("IncreaseDecreaseReport", "Customer", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("IncreaseDecreaseReport", "Customer", new { area = "Portal" }), "Increase/Decrease Report");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult NewCustomers()
        {
            ViewBag.CurrentMenu = "NewCustomers";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NewCustomers", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("NewCustomers", "Customer", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("NewCustomers", "Customer", new { area = "Portal" }), "New Customers Report");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult CustomerStatements()
        {
            ViewBag.CurrentMenu = "CustomerStatements";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerStatements", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("CustomerStatements", "Customer", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("CustomerStatements", "Customer", new { area = "Portal" }), "Customer Statements");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        #endregion Customer > Reports



        [HttpGet]
        public JsonResult InvoicesSearchForPaymentResultData(int customerId, string OpenClose)
        {
            List<ARInvoiceListViewModel> lstARInvoiceListViewModel = CustomerService.GetInvoiceListWithSearchForPayment(customerId, OpenClose).ToList();

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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

        public ActionResult BillRun(FormCollection collection)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            return RedirectToAction("InvoiceList", new { searchin = collection["SearchIn"].ToString(), searchvalue = collection["For"].ToString(), status = collection["Status"].ToString() });
        }

        public IEnumerable<SelectListItem> getStatusList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getStatusList))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getStatusList, CustomerService._GetStatusList(1));
            }
            Dictionary<string, string> dataDict = (Dictionary<string, string>)_cacheProvider.Get(CacheKeyName.Customer_getStatusList);

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();
        }

        [HttpGet]
        public JsonResult GetCreditResultData(int CustomerId)
        {
            List<ARInvoiceListViewModel> lstARInvoiceListViewModel = CustomerService.GetInvoiceListWithSearchForCredit(CustomerId);

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult ApplyCreditForm(int Id)
        {
            ViewBag.ReasonList = new SelectList(CustomerService.GetAll_ReasonList(), "CreditReasonListId", "Name");

            var model = AccountReceivableService.GetCreditDetailForInvoice(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");
            if (model.Invoice.InvoiceDetailItems.Count() > 1 && model.FranchiseeItems.Count() == 1)
            {
                ViewBag.LineNoList = new SelectList(new List<SelectListItem> { new SelectListItem() { Selected = true, Text = "All", Value = "-1" } }, "Value", "Text", -1); ;
            }

            ViewBag.BillMonthList = GetMonthsList(model.Invoice.InvoiceDetail.BillMonth ?? 0);
            ViewBag.BillYearList = GetYearsList(model.Invoice.InvoiceDetail.BillYear ?? 0);

            return PartialView("_PartialApplyCreditForm", model);
        }

        private SelectList GetMonthsList(int selectedMonth = 0)
        {
            List<SelectListItem> monthsList = Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
            { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            return new SelectList(monthsList, "Value", "Text", selectedMonth.ToString());
        }

        private SelectList GetYearsList(int selectedYear = 0)
        {
            var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
            List<SelectListItem> billYearList = new List<SelectListItem>();
            foreach (var y in yearList) { billYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() }); }

            return new SelectList(billYearList, "Value", "Text", selectedYear.ToString());
        }

        public List<StatusList> getARStatusList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getARStatusList))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getARStatusList, CustomerService.GetStatusList().Where(s => s.TypeListId == 1).ToList());
                //_cacheProvider.Set(CacheKeyName.Customer_getARStatusList, CustomerService.getARStatusList());
            }
            return (List<StatusList>)_cacheProvider.Get(CacheKeyName.Customer_getARStatusList);
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

        [HttpGet]
        public ActionResult ApplyManualPaymentForm(int Id)
        {
            var model = AccountReceivableService.GetCreditDetailForInvoice(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");

            return PartialView("_PartialApplyManualPaymentForm", model);
        }

        public List<InvoiceDateList> getInvoiceDate()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getInvoiceDate))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getInvoiceDate, CustomerService.getInvoiceDate());
            }
            return (List<InvoiceDateList>)_cacheProvider.Get(CacheKeyName.Customer_getInvoiceDate);
        }

        public List<InvoiceTermList> getTermDate()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getTermDate))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getTermDate, CustomerService.getTermDate());
            }
            return (List<InvoiceTermList>)_cacheProvider.Get(CacheKeyName.Customer_getTermDate);
        }

        public IEnumerable<SelectListItem> getTaxAuthority()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getTaxAuthority))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getTaxAuthority, CustomerService.getTaxAuthority());
            }
            Dictionary<string, string> dataDict = (Dictionary<string, string>)_cacheProvider.Get(CacheKeyName.Customer_getTaxAuthority);

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();
        }

        public IEnumerable<SelectListItem> getUsStatesList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getUsStatesList))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getUsStatesList, CustomerService._GetStateList());
            }
            Dictionary<string, string> dataDict = (Dictionary<string, string>)_cacheProvider.Get(CacheKeyName.Customer_getUsStatesList);

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();
        }

        public IEnumerable<SelectListItem> getCAccountType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_getCAccountType))
            {
                _cacheProvider.Set(CacheKeyName.Customer_getCAccountType, CustomerService._GetAccountTypeList());
            }
            Dictionary<string, string> dataDict = (Dictionary<string, string>)_cacheProvider.Get(CacheKeyName.Customer_getCAccountType);

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();
        }

        public IEnumerable<SelectListItem> getFrequencyList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Customer_GetFrequencyList))
            {
                _cacheProvider.Set(CacheKeyName.Customer_GetFrequencyList, CustomerService._GetCleanFrequencyList());
            }
            Dictionary<string, string> dataDict = (Dictionary<string, string>)_cacheProvider.Get(CacheKeyName.Customer_GetFrequencyList);

            return (from d in dataDict
                    select new SelectListItem
                    {
                        Value = d.Key.ToString().Trim(),
                        Text = d.Value.Trim()
                    }).ToList();
        }

        [HttpGet]
        public JsonResult GetAllCustomersStatements(string StartDate, string EndDate, string RegionIds)
        {
            List<PortalSpGetCustomerStatements_Result> data = new jkDatabaseEntities().PortalSpGetCustomerStatements(Convert.ToDateTime(StartDate), Convert.ToDateTime(EndDate), RegionIds).ToList();

            var jsonResult = Json(new { aaData = data, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        #region AjaxCalls

        [HttpGet]
        public ActionResult AddEditContractDetail(int ContractId, int ContractDetailId)
        {
            //int ContractId = Convert.ToInt32(id);
            //int ContractDetailId = Convert.ToInt32(ContractDetailId);

            FullCustomerViewModel FullCustomerViewModel = new FullCustomerViewModel();

            if (ContractId > 0 && ContractDetailId > 0)
            {
                // var result = CustomerService.GetContractDetailResultModel(FullCustomerViewModel, ContractDetailId);
                FullCustomerViewModel.ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractDetailId == ContractDetailId).FirstOrDefault().ToModel<ContractDetailViewModel, ContractDetail>();
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(x => x.TypeListId == 1).ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }

                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }

                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
            else
            {
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(x => x.TypeListId == 1).ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }

                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }

                FullCustomerViewModel.ContractDetail.ContractId = ContractId;
                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
        }

        [HttpPost]
        public ActionResult AddEditContractDetail(ContractDetailViewModel ContractDetailDescriptionViewModel, FormCollection collection)
        {
            if (ContractDetailDescriptionViewModel != null && ContractDetailDescriptionViewModel.ContractDetailId > 0)
            {
                ContractDetailDescriptionViewModel.CreatedBy = LoginUserId;
                ContractDetailDescriptionViewModel = CustomerService.SaveContractDetail(ContractDetailDescriptionViewModel.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

                return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractId, success = true }, JsonRequestBehavior.AllowGet);
            }
            else if (ContractDetailDescriptionViewModel.ContractId > 0)
            {
                ContractDetailDescriptionViewModel.CreatedBy = LoginUserId;
                ContractDetailDescriptionViewModel.LineNumber = getLineNo(ContractDetailDescriptionViewModel.ContractId) + 1;
                ContractDetailDescriptionViewModel.ContractId = ContractDetailDescriptionViewModel.ContractId;
                ContractDetailDescriptionViewModel = CustomerService.SaveContractDetail(ContractDetailDescriptionViewModel.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();
                return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractId, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractDetailId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public int getLineNo(int? ContractId)
        {
            return CustomerService.getLineNo(ContractId);
        }

        [HttpPost]
        public ActionResult RemoveContractDetail(int ContractDetailId, int ContarctId)
        {
            if (ContractDetailId > 0)
            {
                var data = CustomerService.DeleteContractDetail(ContractDetailId);
                UpdateLineNo(ContarctId);
                //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
            }
            return Json(new { id = ContarctId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateLineNo(int ContractId)
        {
            CustomerService.UpdateLineNo(ContractId);
        }

        [HttpGet]
        public ActionResult GetContractDetail(string id)
        {
            int ContractId = Convert.ToInt32(id);
            FullCustomerViewModel FullCustomerViewModel = new FullCustomerViewModel();
            FullCustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
            var servicename = CustomerService.GetServiceTypeList();
            foreach (var row in FullCustomerViewModel._ContractDetail)
            {
                var name = servicename.Where(x => x.ServiceTypeListid == row.ServiceTypeListId).FirstOrDefault().name;
                row.ServiceTypeName = name.ToString();
            }
            return View("~/Areas/Portal/Views/Customer/_ContractDetailDescription.cshtml", FullCustomerViewModel._ContractDetail);
        }

        #endregion AjaxCalls

        private FullCustomerViewModel1 Maintennacepopup(int id)
        {
            int currentId = 0;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address = string.Empty;
            string CustomerStatus = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

            ViewBag.Address1 = string.Empty;
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += "" + StateName;
                }

                ViewBag.Address = Address;

                if (Address.Trim().ToLower() == "newyork")
                {
                    Address = "NY";
                }
            }

            if (CustomerViewModel.CustomerViewModel != null && CustomerViewModel.CustomerViewModel.StatusId != null)
            {
                if (CustomerViewModel.CustomerViewModel.StatusListId == 1) //Active status
                {
                    CustomerStatus = "Active";
                }
                else if (CustomerViewModel.CustomerViewModel.StatusListId == 4) //Pending status
                {
                    CustomerStatus = "Pending";
                }
            }
            ViewBag.CustomerStatus = CustomerStatus;
            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.TaxAuthority = getTaxAuthority();

            int ARStatusReasonListId = 0;
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null)
            {
                var ARStatu = CustomerService.GetARStatu().Where(w => w.BillSettingsId == CustomerViewModel.BillSetting.BillSettingsId);
                if (ARStatu != null && ARStatu.Count() > 0)
                {
                    ARStatusReasonListId = (ARStatu.FirstOrDefault().ARStatusReasonListId.HasValue ? ARStatu.FirstOrDefault().ARStatusReasonListId.Value : 0);
                }
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null && CustomerViewModel.MainAddress.StateListId != null)
            {
                ViewBag.MainStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name", CustomerViewModel.MainAddress.StateListId);
            }
            else
            {
                ViewBag.MainStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillingAddress != null && CustomerViewModel.BillingAddress.StateListId != null)
            {
                ViewBag.BillingStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name", CustomerViewModel.BillingAddress.StateListId);
            }
            else
            {
                ViewBag.BillingStatesList = new SelectList(CustomerService.GetStateList(), "StateListId", "Name");
            }

            // if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && ARStatusReasonListId != 0)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", ARStatusReasonListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name", CustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");

            FullCustomerViewModel1 fullcustomer = new FullCustomerViewModel1();

            fullcustomer.CustomerViewModel = CustomerViewModel.CustomerViewModel;
            fullcustomer.BillSetting = CustomerViewModel.BillSetting;
            fullcustomer.MainAddress = CustomerViewModel.MainAddress;
            fullcustomer.MainContact = CustomerViewModel.MainContact;
            fullcustomer.MainEmail = CustomerViewModel.MainEmail;
            fullcustomer.CustomerDetail = CustomerViewModel.CustomerDetail;
            fullcustomer.MainPhone = CustomerViewModel.MainPhone;
            fullcustomer.BillingContact = CustomerViewModel.BillingContact;
            fullcustomer.BillingAddress = CustomerViewModel.BillingAddress;
            fullcustomer.BillingPhone = CustomerViewModel.BillingPhone;
            fullcustomer.BillingEmail = CustomerViewModel.BillingEmail;
            fullcustomer.ContactInformation = CustomerViewModel.ContactInformation;
            fullcustomer.ContactInformationPhone = CustomerViewModel.ContactInformationPhone;
            fullcustomer.ContactInformationEmail = CustomerViewModel.ContactInformationEmail;
            fullcustomer.BillingContact = CustomerViewModel.BillingContact;
            fullcustomer.BillingInformationPhone = CustomerViewModel.BillingInformationPhone;
            fullcustomer.BillingInformationEmail = CustomerViewModel.BillingInformationEmail;
            fullcustomer.BillingInformation = CustomerViewModel.BillingInformation;
            fullcustomer.BillingInformationAddress = CustomerViewModel.BillingInformationAddress;

            fullcustomer.MainPhone2 = CustomerViewModel.MainPhone2;
            fullcustomer.BillingContactInformation2 = CustomerViewModel.BillingContactInformation2;
            fullcustomer.lstEBillEmails = CustomerViewModel.lstEBillEmails;
            if (fullcustomer.lstEBillEmails == null)
                fullcustomer.lstEBillEmails = new List<EmailViewModel>();
            foreach (EmailViewModel op in fullcustomer.lstEBillEmails)
            {
                fullcustomer.EBill_Emails += op.EmailAddress + ",";
            }
            if (fullcustomer.EBill_Emails.Length > 0)
                fullcustomer.EBill_Emails = fullcustomer.EBill_Emails.Trim(',');

            return fullcustomer;
        }

        private FullCustomerViewModel GetDataMaintennacepopup(int id)
        {
            int currentId = 0;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

            ViewBag.Address1 = string.Empty;
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += "" + StateName;
                }

                ViewBag.Address = Address;
            }

            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.ARStatusListId != null)
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name", CustomerViewModel.BillSetting.ARStatusListId);
            }
            else
            {
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
            }
            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceDateListId != null)
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "term", CustomerViewModel.BillSetting.InvoiceDateListId);
            }
            else
            {
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "term");
            }

            if (CustomerViewModel != null && CustomerViewModel.BillSetting != null && CustomerViewModel.BillSetting.InvoiceTermListId != null)
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name", CustomerViewModel.BillSetting.InvoiceTermListId);
            }
            else
            {
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");

            return CustomerViewModel;
        }

        public ActionResult MaintenanceStepTwopop(FullCustomerViewModel1 para_billseting, string ArStatus = "")
        {
            FullCustomerViewModel fullCustomerViewModel = new FullCustomerViewModel();

            fullCustomerViewModel = GetDataMaintennacepopup(Convert.ToInt32(para_billseting.BillSetting.CustomerId));

            para_billseting.BillSetting.ARStatusListId = Convert.ToInt32(ArStatus != "" ? ArStatus : "0");
            para_billseting.BillSetting.InvoiceTermListId = para_billseting.TermDate;
            para_billseting.BillSetting.InvoiceDateListId = para_billseting.InvoiceDate;
            para_billseting.BillSetting.Notes = (para_billseting.BillSetting.Notes != null ? para_billseting.BillSetting.Notes.Replace("\"", "") : "");

            fullCustomerViewModel.BillSetting = para_billseting.BillSetting;
            fullCustomerViewModel.BillSetting.CreatedDate = System.DateTime.Now;

            fullCustomerViewModel.BillSetting = CustomerService.SaveBillSetting(fullCustomerViewModel.BillSetting.ToModel<BillSetting, BillSettingViewModel>()).ToModel<BillSettingViewModel, BillSetting>();

            return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.BillSetting.CustomerId });
        }

        public ActionResult AddContractDetail(FullCustomerViewModel fullCustomerViewModel)
        {
            //ContractDetail cd = new ContractDetail();
            fullCustomerViewModel.ContractDetail = CustomerService.SaveContractDetail(fullCustomerViewModel.ContractDetail.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

            return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
        }

        public ActionResult GetCustomerDetailTransactionList(int customerId)
        {
            return PartialView("_CustomerDetailTransaction", CustomerService.GetCustomerDetailTransactions(547, 0, DateTime.Parse("01/01/2017"), DateTime.Parse("12/12/2017")));
        }

        [HttpPost]
        public JsonResult SaveDistribution(FormCollection frm)
        {
            var ff = frm;

            int contractdetail_linecount = frm["hdfcontractdetail_linecount"] != null ? int.Parse(frm["hdfcontractdetail_linecount"]) : 0;
            int frenchiseedetail_linecount = frm["hdffrenchiseedetail_linecount"] != null ? int.Parse(frm["hdffrenchiseedetail_linecount"]) : 0;
            int customerid = frm["hdffrenchiseedetail_customerid"] != null ? int.Parse(frm["hdffrenchiseedetail_customerid"]) : 0;

            int frenchiseeid = 0;
            int DistributionContractDetailId = 0;
            int DistributionDetailId = 0;
            decimal DistributionAmountId = 0;
            List<FranchiseeDistribution> lstFranchiseeDistribution = new List<FranchiseeDistribution>();
            FranchiseeDistribution oFranchiseeDistribution;

            for (int i = 1; i <= frenchiseedetail_linecount; i++)
            {
                oFranchiseeDistribution = new FranchiseeDistribution();

                var fd_hdfdistributionid = frm["fd_hdfdistributionid" + i] != null ? int.Parse(frm["fd_hdfdistributionid" + i]) : 0;
                var fd_hdfcontractdetailid = frm["fd_hdfcontractdetailid" + i] != null ? int.Parse(frm["fd_hdfcontractdetailid" + i]) : 0;
                var fd_hdfFrenchiseeId = frm["fd_hdfFrenchiseeId" + i] != null ? int.Parse(frm["fd_hdfFrenchiseeId" + i]) : 0;
                var fd_txtlinenumber = frm["fd_txtlinenumber" + i] != null ? int.Parse(frm["fd_txtlinenumber" + i]) : 0;
                var fd_txtfranchiseeno = frm["fd_txtfranchiseeno" + i] != null ? frm["fd_txtfranchiseeno" + i].ToString() : "";
                var fd_txtfranchiseename = frm["fd_txtfranchiseename" + i] != null ? frm["fd_txtfranchiseename" + i].ToString() : "";
                var fd_txtfranchiseeamount = frm["fd_txtfranchiseeamount" + i] != null ? decimal.Parse(frm["fd_txtfranchiseeamount" + i]) : 0;

                oFranchiseeDistribution.Amount = fd_txtfranchiseeamount;
                oFranchiseeDistribution.ContractDetailId = fd_hdfcontractdetailid;
                oFranchiseeDistribution.CustomerId = customerid;
                oFranchiseeDistribution.DetailLineNumber = fd_txtlinenumber;
                oFranchiseeDistribution.FranchiseeId = fd_hdfFrenchiseeId;
                oFranchiseeDistribution.ContractId = 0;
                oFranchiseeDistribution.FranchiseeName = fd_txtfranchiseename;
                oFranchiseeDistribution.FranchiseeNo = fd_txtfranchiseeno;

                lstFranchiseeDistribution.Add(oFranchiseeDistribution);
            }

            if (lstFranchiseeDistribution.Count > 0)
            {
                bool retVal = CustomerService.SaveFranchiseeDistribution(lstFranchiseeDistribution);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFrenchiseeDistribution(FormCollection frm)
        {
            var ff = frm;

            int contractdetail_linecount = frm["hdfcontractdetail_linecount"] != null ? int.Parse(frm["hdfcontractdetail_linecount"]) : 0;
            int frenchiseedetail_linecount = frm["hdffrenchiseedetail_linecount"] != null ? int.Parse(frm["hdffrenchiseedetail_linecount"]) : 0;
            int customerid = frm["hdffrenchiseedetail_customerid"] != null ? int.Parse(frm["hdffrenchiseedetail_customerid"]) : 0;

            int frenchiseeid = 0;
            int DistributionContractDetailId = 0;
            int DistributionDetailId = 0;
            decimal DistributionAmountId = 0;
            List<FranchiseeDistribution> lstFranchiseeDistribution = new List<FranchiseeDistribution>();
            FranchiseeDistribution oFranchiseeDistribution;
            for (int j = 1; j <= frenchiseedetail_linecount; j++)
            {
                for (int i = 1; i <= contractdetail_linecount; i++)
                {
                    frenchiseeid = frm["FranchiseeDistributionFranchiseeId_" + i + "_" + j] != null ? int.Parse(frm["FranchiseeDistributionFranchiseeId_" + i + "_" + j]) : 0;
                    if (frenchiseeid > 0)
                    {
                        oFranchiseeDistribution = new FranchiseeDistribution();

                        DistributionContractDetailId = frm["FranchiseeDistributionContractDetailId_" + i + "_" + j] != null ? int.Parse(frm["FranchiseeDistributionContractDetailId_" + i + "_" + j]) : 0;
                        DistributionDetailId = frm["FranchiseeDistributionDetailId_" + i + "_" + j] != null ? int.Parse(frm["FranchiseeDistributionDetailId_" + i + "_" + j]) : 0;
                        DistributionAmountId = frm["FranchiseeDistributionAmountId_" + i + "_" + j] != null ? Convert.ToDecimal(frm["FranchiseeDistributionAmountId_" + i + "_" + j]) : 0;

                        oFranchiseeDistribution.Amount = DistributionAmountId;
                        oFranchiseeDistribution.ContractDetailId = DistributionContractDetailId;
                        oFranchiseeDistribution.CustomerId = customerid;
                        oFranchiseeDistribution.DetailLineNumber = i;
                        oFranchiseeDistribution.FranchiseeId = frenchiseeid;
                        lstFranchiseeDistribution.Add(oFranchiseeDistribution);
                    }
                }
            }

            if (lstFranchiseeDistribution.Count > 0)
            {
                bool retVal = CustomerService.SaveFranchiseeDetailsDistribution(lstFranchiseeDistribution);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region CustomerDatatable

        public ActionResult CustomerDataTable(string status)
        {
            try
            {
                int StatusId = Convert.ToInt32(status);
                int CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
                int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).ToList();
                var result = from c in response
                             select new
                             {
                                 ID = c.Customer.CustomerId,
                                 Number = c.Customer.CustomerNo,
                                 Name = c.MainContact.Name,
                                 Address = getAddress(c.MainAddress),
                                 Phone = c.MainPhone.Phone1
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

        #endregion CustomerDatatable

        #region Helpers

        public string getAddress(Address _Address)
        {
            if (_Address != null)
            {
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(_Address.Address1))
                {
                    Address = _Address.Address1;
                }
                if (!string.IsNullOrEmpty(_Address.City))
                {
                    Address += " " + _Address.City;
                }
                if (_Address.StateListId != null && _Address.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == _Address.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(_Address.PostalCode))
                {
                    Address += " " + _Address.PostalCode;
                }
                return Address;
            }
            return string.Empty;
        }

        public JsonResult getName(string _Customer)
        {
            var customerName = CustomerService.GetCustomers().Where(x => x.Name == _Customer).Count();
            return this.Json(customerName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddParent(FormCollection summaryData)
        {
            if (summaryData == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            //var Parentname = CustomerService.GetCustomers().Where(x => x.Name == summaryData["parentname"]);
            string parentname = summaryData["parentname"];
            var Parentname = CustomerService.GetCustomers().FirstOrDefault(x => x.Name.Equals(parentname));
            if (Parentname != null)
            {
                return Json(new
                {
                    success = true,
                    result = Parentname.Name,
                    id = Parentname.CustomerId
                });
            }
            else
            {
                NLogger.Error("Parent does not exist");
                return Json(new { success = false, message = "Parent does not exist" });
            }
        }

        [HttpPost]
        public ActionResult GetCustomerId(FormCollection summaryData)
        {
            if (summaryData == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            //var Parentname = CustomerService.GetCustomers().Where(x => x.Name == summaryData["parentname"]);
            string parentname = summaryData["parentname"];
            var Parentname = CustomerService.GetCustomers().FirstOrDefault(x => x.Name.Equals(parentname));
            if (Parentname != null)
            {
                return Json(new
                {
                    success = true,
                    result = Parentname.Name,
                    id = Parentname.CustomerId
                });
            }
            else
            {
                NLogger.Error("Parent does not exist");
                return Json(new { success = false, message = "Parent does not exist" });
            }
        }

        [HttpPost]
        public JsonResult ParentAutoComplete(string namePrefix)
        {
            if (namePrefix == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }
            var Parentname = CustomerService.GetCustomers().Where(x => x.Name.Contains(namePrefix) && (x.RegionId == SelectedRegionId || SelectedRegionId == 0)).Select(x => new { x.Name, x.CustomerId }).ToList();
            return this.Json(Parentname, JsonRequestBehavior.AllowGet);
        }

        public string GetCustomerNo()
        {
            string customerno = CustomerService.getCustomerNo(SelectedRegionId);
            return customerno;
        }

        public void UpdateCustomerIndex()
        {
            RegionSetting regData = CustomerService.GetRegionConfigurationbyId(3, SelectedRegionId);
            int valueadd = Convert.ToInt32(regData.Value) + 1;
            regData.Value = valueadd.ToString();

            CustomerService.SaveRegionConfiguration(regData);
        }

        public string getStatename(int id)
        {
            var States = CustomerService.GetStatesName(id);
            return States.ToString();
        }

        private string PhoneNoformat(string phone)
        {
            if (phone.Length == 10)
            {
                phone = '(' + phone.Substring(0, 3) + ')' + ' ' + phone.Substring(3, 3) + '-' + phone.Substring(6, 4);
            }
            return phone;
        }

        #endregion Helpers

        public ActionResult EditCustomerPopupMainContactTab1(int CustomerId, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode, int PhoneId, string Phone1, string Fax, string CustName = "")
        {
            FullCustomerViewModel1 FullCustomerViewModel1 = new FullCustomerViewModel1();
            if (CustomerId > 0)
            {
                //Update Customer Info

                var CustModel = CustomerService.GetCustomerById(CustomerId);
                if (CustModel != null)
                {
                    CustModel.Name = CustName;
                    CustomerService.SaveCustomers(CustModel);
                }

                FullCustomerViewModel1.CustomerViewModel.CustomerId = CustomerId;
                FullCustomerViewModel1.MainAddress.AddressId = AddressId;
                FullCustomerViewModel1.MainAddress.ClassId = CustomerId;
                FullCustomerViewModel1.MainAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.MainAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel1.MainAddress.Address1 = Address1;
                FullCustomerViewModel1.MainAddress.Address2 = Address2;
                FullCustomerViewModel1.MainAddress.City = City;
                if (StateId != "")
                {
                    FullCustomerViewModel1.MainAddress.StateListId = Convert.ToInt32(StateId);

                    int id = Convert.ToInt32(StateId);
                    string state = CustomerService.GetStatesName(id);
                    FullCustomerViewModel1.MainAddress.StateName = state.Trim();
                }
                FullCustomerViewModel1.MainAddress.PostalCode = PostalCode;
                FullCustomerViewModel1.MainAddress.IsActive = true;
                FullCustomerViewModel1.MainAddress.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.MainAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel1.MainAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    FullCustomerViewModel1.MainAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    FullCustomerViewModel1.MainAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }
                FullCustomerViewModel1.MainAddress = CustomerService.AddNewAddressOldInactive(FullCustomerViewModel1.MainAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();

                FullCustomerViewModel1.MainPhone.PhoneId = PhoneId;
                FullCustomerViewModel1.MainPhone.ClassId = CustomerId;
                FullCustomerViewModel1.MainPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.MainPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                FullCustomerViewModel1.MainPhone.Phone1 = Phone1;
                FullCustomerViewModel1.MainPhone.Fax = Fax;
                FullCustomerViewModel1.MainPhone.IsActive = true;
                FullCustomerViewModel1.MainPhone.CountryCodeListId = 1;
                FullCustomerViewModel1.MainPhone.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.MainPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                FullCustomerViewModel1.MainPhone = CustomerService.AddNewPhoneOldInactive(FullCustomerViewModel1.MainPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();
            }

            List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
            List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
            _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);

            return Json(new { AddressId = FullCustomerViewModel1.MainAddress.AddressId, PhoneId = FullCustomerViewModel1.MainPhone.PhoneId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = CustomerId });
        }

        public ActionResult EditCustomerPopupMainContactTab2(int CustomerId, int ContactId, string Name, string Title, int PhoneId, string Phone1, string PhoneExt, string Cell, string Fax, int EmailId, string EmailAddress)
        {
            FullCustomerViewModel1 FullCustomerViewModel1 = new FullCustomerViewModel1();
            if (CustomerId > 0)
            {
                //Contact Information
                FullCustomerViewModel1.ContactInformation.ContactId = ContactId;
                FullCustomerViewModel1.ContactInformation.ClassId = CustomerId;
                FullCustomerViewModel1.ContactInformation.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.ContactInformation.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                FullCustomerViewModel1.ContactInformation.Name = Name;
                //FullCustomerViewModel1.ContactInformation.LastName = LastName;
                FullCustomerViewModel1.ContactInformation.Title = Title;
                FullCustomerViewModel1.ContactInformation.IsActive = true;
                FullCustomerViewModel1.ContactInformation.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.ContactInformation.CreatedBy = _claimView.GetCLAIM_USERID();

                FullCustomerViewModel1.ContactInformation = CustomerService.AddNewContactOldInactive(FullCustomerViewModel1.ContactInformation.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullCustomerViewModel1.ContactInformationPhone.PhoneId = PhoneId;
                FullCustomerViewModel1.ContactInformationPhone.ClassId = CustomerId;
                FullCustomerViewModel1.ContactInformationPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.ContactInformationPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                FullCustomerViewModel1.ContactInformationPhone.Phone1 = Phone1;
                FullCustomerViewModel1.ContactInformationPhone.PhoneExt = PhoneExt;
                FullCustomerViewModel1.ContactInformationPhone.Cell = Cell;
                FullCustomerViewModel1.ContactInformationPhone.Fax = Fax;
                FullCustomerViewModel1.ContactInformationPhone.CountryCodeListId = 1;
                FullCustomerViewModel1.ContactInformationPhone.IsActive = true;
                FullCustomerViewModel1.ContactInformationPhone.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.ContactInformationPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                FullCustomerViewModel1.ContactInformationPhone = CustomerService.AddNewPhoneOldInactive(FullCustomerViewModel1.ContactInformationPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                FullCustomerViewModel1.ContactInformationEmail.EmailId = EmailId;
                FullCustomerViewModel1.ContactInformationEmail.ClassId = CustomerId;
                FullCustomerViewModel1.ContactInformationEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.ContactInformationEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
                FullCustomerViewModel1.ContactInformationEmail.EmailAddress = EmailAddress;
                FullCustomerViewModel1.ContactInformationEmail.IsActive = true;
                FullCustomerViewModel1.ContactInformationEmail.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.ContactInformationEmail.CreatedBy = _claimView.GetCLAIM_USERID();

                FullCustomerViewModel1.ContactInformationEmail = CustomerService.AddNewEmailOldInactive(FullCustomerViewModel1.ContactInformationEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
            }
            List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
            List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
            _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);
            return Json(new { ContactId = FullCustomerViewModel1.ContactInformation.ContactId, EmailId = FullCustomerViewModel1.ContactInformationEmail.EmailId, PhoneId = FullCustomerViewModel1.ContactInformationPhone.PhoneId }, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = CustomerId });
        }

        public ActionResult EditCustomerPopupBillingTab1(int CustomerId, int ContactId, string Name, string Attention, int AddressId, string Address1, string Address2, string City, string StateId, string PostalCode)
        {
            FullCustomerViewModel1 FullCustomerViewModel1 = new FullCustomerViewModel1();
            if (CustomerId > 0)
            {
                FullCustomerViewModel1.CustomerViewModel.CustomerId = CustomerId;
                FullCustomerViewModel1.BillingContact.ContactId = ContactId;
                FullCustomerViewModel1.BillingContact.ClassId = CustomerId;
                FullCustomerViewModel1.BillingContact.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.BillingContact.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel1.BillingContact.Name = Name;
                FullCustomerViewModel1.BillingContact.Attention = Attention;
                FullCustomerViewModel1.BillingContact.IsActive = true;
                FullCustomerViewModel1.BillingContact.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.BillingContact.CreatedBy = _claimView.GetCLAIM_USERID();
                FullCustomerViewModel1.BillingContact = CustomerService.AddNewContactOldInactive(FullCustomerViewModel1.BillingContact.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullCustomerViewModel1.BillingAddress.AddressId = AddressId;
                FullCustomerViewModel1.BillingAddress.ClassId = CustomerId;
                FullCustomerViewModel1.BillingAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.BillingAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
                FullCustomerViewModel1.BillingAddress.Address1 = Address1;
                FullCustomerViewModel1.BillingAddress.Address2 = Address2;
                FullCustomerViewModel1.BillingAddress.City = City;
                FullCustomerViewModel1.BillingAddress.PostalCode = PostalCode;
                if (StateId != "")
                {
                    FullCustomerViewModel1.BillingAddress.StateListId = Convert.ToInt32(StateId);
                }
                FullCustomerViewModel1.BillingAddress.IsActive = true;
                FullCustomerViewModel1.BillingAddress.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.BillingAddress.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                var _latlngBL = GetLatLongByAddress(HttpUtility.UrlEncode(FullCustomerViewModel1.BillingAddress.FullAddress));
                if (_latlngBL.results.Count() > 0)
                {
                    FullCustomerViewModel1.BillingAddress.Latitude = decimal.Parse(_latlngBL.results[0].geometry.location.lat.ToString());
                    FullCustomerViewModel1.BillingAddress.Longitude = decimal.Parse(_latlngBL.results[0].geometry.location.lng.ToString());
                }
                FullCustomerViewModel1.BillingAddress = CustomerService.AddNewAddressOldInactive(FullCustomerViewModel1.BillingAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            }
            List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
            List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
            _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);
            return Json(new { ContactId = FullCustomerViewModel1.BillingContact.ContactId, AddressId = FullCustomerViewModel1.BillingAddress.AddressId }, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = CustomerId });
        }

        public ActionResult EditCustomerPopupBillingTab2(int CustomerId, int ContactId, string Name, string Title, int PhoneId, string Phone1, string PhoneExt, string Cell, string Fax, int EmailId, string EmailAddress)
        {
            FullCustomerViewModel1 FullCustomerViewModel1 = new FullCustomerViewModel1();
            if (CustomerId > 0)
            {
                FullCustomerViewModel1.CustomerViewModel.CustomerId = CustomerId;
                FullCustomerViewModel1.BillingContactInformation2.ContactId = ContactId;
                FullCustomerViewModel1.BillingContactInformation2.ClassId = CustomerId;
                FullCustomerViewModel1.BillingContactInformation2.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.BillingContactInformation2.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                FullCustomerViewModel1.BillingContactInformation2.Name = Name;
                FullCustomerViewModel1.BillingContactInformation2.Title = Title;
                FullCustomerViewModel1.BillingContactInformation2.IsActive = true;
                FullCustomerViewModel1.BillingContactInformation2.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.BillingContactInformation2.CreatedBy = _claimView.GetCLAIM_USERID();
                FullCustomerViewModel1.BillingContactInformation2 = CustomerService.AddNewContactOldInactive(FullCustomerViewModel1.BillingContactInformation2.ToModel<Contact, ContactViewModel>()).ToModel<ContactViewModel, Contact>();

                FullCustomerViewModel1.BillingPhone.PhoneId = PhoneId;
                FullCustomerViewModel1.BillingPhone.ClassId = CustomerId;
                FullCustomerViewModel1.BillingPhone.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.BillingPhone.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                FullCustomerViewModel1.BillingPhone.Phone1 = Phone1;
                FullCustomerViewModel1.BillingPhone.PhoneExt = PhoneExt;
                FullCustomerViewModel1.BillingPhone.Cell = Cell;
                FullCustomerViewModel1.BillingPhone.Fax = Fax;
                FullCustomerViewModel1.BillingPhone.IsActive = true;
                FullCustomerViewModel1.BillingPhone.CountryCodeListId = 1;
                FullCustomerViewModel1.BillingPhone.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.BillingPhone.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                FullCustomerViewModel1.BillingPhone = CustomerService.AddNewPhoneOldInactive(FullCustomerViewModel1.BillingPhone.ToModel<Phone, PhoneViewModel>()).ToModel<PhoneViewModel, Phone>();

                FullCustomerViewModel1.BillingEmail.EmailId = EmailId;
                FullCustomerViewModel1.BillingEmail.ClassId = CustomerId;
                FullCustomerViewModel1.BillingEmail.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                FullCustomerViewModel1.BillingEmail.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;
                FullCustomerViewModel1.BillingEmail.EmailAddress = EmailAddress;
                FullCustomerViewModel1.BillingEmail.IsActive = true;
                FullCustomerViewModel1.BillingEmail.CreatedDate = DateTime.Now;
                FullCustomerViewModel1.BillingEmail.CreatedBy = _claimView.GetCLAIM_USERID();

                FullCustomerViewModel1.BillingEmail = CustomerService.AddNewEmailOldInactive(FullCustomerViewModel1.BillingEmail.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
            }
            List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
            List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
            _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);
            return Json(new { ContactId = FullCustomerViewModel1.BillingContactInformation2.ContactId, PhoneId = FullCustomerViewModel1.BillingPhone.PhoneId, EmailId = FullCustomerViewModel1.BillingEmail.EmailId }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = CustomerId });
        }

        public ActionResult SaveBillSettings(int CustomerId, int BillSettingsId, string ArStatus, string EffectiveDate, int InvoiceDateListId, int InvoiceTermListId, int EBill, int PrintPastDue, int TaxExcempt, int ConsolidatedInvoice, string Note, string EBillEmails, int PBill = 0, int CreateInvoice = 0)
        {
            FullCustomerViewModel1 para_billseting = new FullCustomerViewModel1();
            para_billseting.BillSetting.CustomerId = CustomerId;
            para_billseting.BillSetting.BillSettingsId = BillSettingsId;
            para_billseting.BillSetting.EffectiveDate = (string.IsNullOrWhiteSpace(EffectiveDate) || EffectiveDate == "01/01/0001") ? DateTime.Now : Convert.ToDateTime(EffectiveDate);
            para_billseting.BillSetting.ARStatusListId = Convert.ToInt32(ArStatus != "" ? ArStatus : "0");
            para_billseting.BillSetting.InvoiceDateListId = InvoiceDateListId;
            para_billseting.BillSetting.InvoiceTermListId = InvoiceTermListId;
            para_billseting.BillSetting.EBill = (EBill == 0 ? false : true);
            para_billseting.BillSetting.PBill = (PBill == 0 ? false : true);
            para_billseting.BillSetting.PrintPastDue = (PrintPastDue == 0 ? false : true);
            para_billseting.BillSetting.TaxExcempt = (TaxExcempt == 0 ? false : true);
            para_billseting.BillSetting.ConsolidatedInvoice = (ConsolidatedInvoice == 0 ? false : true);
            para_billseting.BillSetting.CreatedDate = DateTime.Now;
            para_billseting.BillSetting.Notes = (Note != "" ? Note.Replace("\"", "") : "");
            para_billseting.BillSetting.IsActive = 1;
            para_billseting.BillSetting.PBill = (PBill == 0 ? false : true);
            para_billseting.BillSetting.ARStatus = Convert.ToInt32(ArStatus != "" ? ArStatus : "0");
            para_billseting.BillSetting.CreateInvoice = (CreateInvoice == 0 ? false : true);
            para_billseting.BillSetting.CreatedBy = LoginUserId;


            para_billseting.BillSetting = CustomerService.AddNewBillSettingOldInactive(para_billseting.BillSetting.ToModel<BillSetting, BillSettingViewModel>()).ToModel<BillSettingViewModel, BillSetting>();

            string tempEBillEmails = EBillEmails;

            if (tempEBillEmails != "")
            {
                EmailViewModel oEmailViewModel;
                foreach (string o in tempEBillEmails.Split(',').Distinct())
                {
                    oEmailViewModel = new EmailViewModel();
                    oEmailViewModel.ClassId = CustomerId;
                    oEmailViewModel.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    oEmailViewModel.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.EBillEmail;
                    oEmailViewModel.IsActive = true;
                    oEmailViewModel.CreatedDate = DateTime.Now;
                    oEmailViewModel.CreatedBy = _claimView.GetCLAIM_USERID();
                    oEmailViewModel.EmailAddress = o;
                    oEmailViewModel.EmailId = 0;
                    oEmailViewModel = CustomerService.SaveEBill_Emails(oEmailViewModel.ToModel<Email, EmailViewModel>()).ToModel<EmailViewModel, Email>();
                }
            }

            #region :: ARStatu  :: 

            //ARStatu modelARStatu = new ARStatu();
            //modelARStatu.BillSettingsId = para_billseting.BillSetting.BillSettingsId;
            //modelARStatu.ARStatusReasonListId = Convert.ToInt32(ArStatus != "" ? ArStatus : "0");
            //modelARStatu.ARStatusDate = DateTime.Now;
            //modelARStatu.isActive = 1;
            //CustomerService.AddNewARStatuOldInactive(modelARStatu);

            #endregion 

            return Json(new { BillSettingsId = para_billseting.BillSetting.BillSettingsId }, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = CustomerId });
        }

        [HttpGet]
        public ActionResult RenderCustomerContractDetails(int id, int callfrom = 0)
        {
            ViewBag.Id = id;
            ViewBag.callfrom = callfrom;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address1 = string.Empty;
            string Address2 = string.Empty;
            var StateList = CustomerService.GetStateList();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null)
            {
                //var result = CustomerService.GetContractDetailResult(CustomerViewModel);
                CustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == CustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
                var servicename = CustomerService.GetServiceTypeList();
                foreach (var row in CustomerViewModel._ContractDetail)
                {
                    var name = servicename.Where(x => x.ServiceTypeListid == row.ServiceTypeListId).FirstOrDefault().name;
                    row.ServiceTypeName = name.ToString();
                }
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = StateList;
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;
            }

            // CustomerViewModel.olist = CustomerService.GetOfficeContacts(id);
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");
            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            //ViewBag.cFrequencyList = getFrequencyList();
            //ViewBag.cAccountType = getCAccountType();
            //ViewBag.statusList = getStatusList();
            // ViewBag.MainStatesList = getUsStatesList();
            //ViewBag.BillingStatesList = getUsStatesList();
            //ViewBag.TaxAuthority = getTaxAuthority();
            //ViewBag.SoldBy = getTaxAuthority();
            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name", CustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name");
            }
            //var FrequencyList = CustomerService.GetFrequencyList().ToList();
            //if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            //{
            //    ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            //}
            //else
            //{
            //    ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name");
            //}
            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", CustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            //var ContractStatusList = CustomerService.GetContractStatusReasonList().ToList();
            //if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractStatusReasonListId != null)
            //{
            //    ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", CustomerViewModel.Contract.ContractStatusReasonListId);
            //}
            //else
            //{
            //    ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            //}

            //var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            //List<SelectListItem> statusList = new List<SelectListItem>();

            //foreach (var y in data)
            //{
            //    statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            //}
            //ViewBag.statusList = statusList;
            //var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList().Where(o => o.TypeListId == 1 && o.contracttype == 1).ToList();
            //if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.ServiceTypeListId != null)
            //{
            //    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name", CustomerViewModel.ContractDetail.ServiceTypeListId);
            //}
            //else
            //{
            //    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
            //}
            //var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            //if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            //{
            //    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            //}
            //else
            //{
            //    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
            //}

            //var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            //if (CleanFrequencyListModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.CleanFrequencyListId != null)
            //{
            //    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", CustomerViewModel.ContractDetail.CleanFrequencyListId);
            //}
            //else
            //{
            //    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
            //}
            CustomerViewModel.ContractAddress = CustomerViewModel.ContactInformationAddress;

            if (StateList != null && CustomerViewModel.ContractAddress != null && CustomerViewModel.ContractAddress.StateListId != null)
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name", CustomerViewModel.ContractAddress.StateListId);
            }
            else
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name");
            }

            var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AgreementTypeListId != null)
            {
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", CustomerViewModel.Contract.AgreementTypeListId);
                var AgreementTypeCPI = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId);
                if (AgreementTypeCPI != null && AgreementTypeCPI.Count() > 0)
                {
                    ViewBag.AgreementTypeCPI = (AgreementTypeCPI.FirstOrDefault().CPI.HasValue ? AgreementTypeCPI.FirstOrDefault().CPI.Value : false);
                }
            }
            else
            {
                ViewBag.AgreementTypeCPI = false;
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", 2);
            }

            //var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true).Where(x => x.IsAccExec).ToList();
            //if (userLista.Count > 0)
            //{
            //    userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

            //    ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            //}
            //else
            //{
            //    ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            //}

            //var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            //ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            //var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            //ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            //var stateData = jkControlEntites.vw_sys_State.OrderBy(s => s.Name).ToList();
            //List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.Abbr }).ToList();

            //ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            //ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return PartialView("_CustomerContractDetailsPopup", CustomerViewModel);
        }

        [HttpGet]
        public ActionResult RenderCustomerContract(int id, int callfrom = 0)
        {

            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Index", "Customer", new { area = "Portal" }), "Customer");


            ViewBag.Id = id; //id: CustomerId
            ViewBag.callfrom = callfrom;
            CustomerContractViewModel oCustomerContract = CustomerService.GetCustomerContractByCustomerId(Convert.ToInt32(id));

            var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
            if (oCustomerContract.AgreementTypeListId > 0)
            {
                var AgreementTypeCPI = AgreementTypeList.Where(w => w.AgreementTypeListId == oCustomerContract.AgreementTypeListId);
                if (AgreementTypeCPI != null && AgreementTypeCPI.Count() > 0)
                {
                    ViewBag.AgreementTypeCPI = (AgreementTypeCPI.FirstOrDefault().CPI.HasValue ? AgreementTypeCPI.FirstOrDefault().CPI.Value : false);
                }
                
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", oCustomerContract.AgreementTypeListId);
            }
            else
            {
                ViewBag.AgreementTypeCPI = false;
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", 2);
            }
            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (oCustomerContract.AccountTypeListId > 0)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name", oCustomerContract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList.OrderBy(x => x.Name), "AccountTypeListId", "Name");
            }
            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (oCustomerContract.ContractTypeListId > 0)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", oCustomerContract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }
            
            var StateList = CustomerService.GetStateList();
            if (StateList != null && oCustomerContract.StateListId > 0)
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name", oCustomerContract.StateListId);
            }
            else
            {
                ViewBag.ContractStatesList = new SelectList(StateList, "StateListId", "Name");
            }

            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            ViewBag.Address = "";
            ViewBag.Address2 = "";



            return PartialView("_ContractMaintenancePP", oCustomerContract);
        }

        [HttpGet]
        public ActionResult RenderCustomerContractDetail(int ContractDetailId, int ContractId = 0, int lineNumber = 1)
        {

            CustomerContractDetailViewModel oCustomerContractDetail = new CustomerContractDetailViewModel();
            

            if (ContractDetailId > 0)
            {
                oCustomerContractDetail = CustomerService.GetCustomerContractDetailByCustomerId(ContractDetailId, ContractId);
               

                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
                if (oCustomerContractDetail.ServiceTypeListId > 0)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name", oCustomerContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (oCustomerContractDetail.BillingFrequencyListId > 0)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", oCustomerContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && oCustomerContractDetail.CleanFrequencyListId > 0)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", oCustomerContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }

                
            }
            else
            {
                oCustomerContractDetail.LineNumber = lineNumber;
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
                if (oCustomerContractDetail.ServiceTypeListId > 0)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name", oCustomerContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (oCustomerContractDetail.BillingFrequencyListId > 0)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", oCustomerContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && oCustomerContractDetail.CleanFrequencyListId > 0)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", oCustomerContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }
                
                
            }


            return PartialView("_ContractDetailMaintenancePP", oCustomerContractDetail);
        }

        [HttpPost]
        public ActionResult SaveContractDetailMaintenance(CustomerContractViewModel model, FormCollection collection)
        {
            FullCustomerViewModel fullCustomerViewModel = new FullCustomerViewModel();
            ContractViewModel oContractViewModel = new ContractViewModel();

            int CN_hdnContractCustomerId = (!String.IsNullOrEmpty(collection["hdnContractCustomerId"])?int.Parse(collection["hdnContractCustomerId"]) :0);
            int CN_AccountTypeList = (!String.IsNullOrEmpty(collection["AccountTypeList"]) ? int.Parse(collection["AccountTypeList"]) : 0);
            int CN_ContractTypeList = (!String.IsNullOrEmpty(collection["ContractTypeList"]) ? int.Parse(collection["ContractTypeList"]) : 0);
            int CN_AgreementTypeList = (!String.IsNullOrEmpty(collection["AgreementTypeList"]) ? int.Parse(collection["AgreementTypeList"]) : 0);
            int CN_SoldById = (!String.IsNullOrEmpty(collection["SoldById"]) ? int.Parse(collection["SoldById"]) : 0);
            string CN_PurchaseOrderNo = (!String.IsNullOrEmpty(collection["PurchaseOrderNo"]) ? Convert.ToString(collection["PurchaseOrderNo"]) : "");
            DateTime? CN_Contract_SignDate = (!String.IsNullOrEmpty(collection["Contract.SignDate"]) ? Convert.ToDateTime(collection["Contract.SignDate"]) : DateTime.Now);
            DateTime? CN_Contract_StartDate = (!String.IsNullOrEmpty(collection["Contract.StartDate"]) ? Convert.ToDateTime(collection["Contract.StartDate"]) : DateTime.Now);
            DateTime? CN_Contract_ExpirationDate = (!String.IsNullOrEmpty(collection["Contract.ExpirationDate"]) ? Convert.ToDateTime(collection["Contract.ExpirationDate"]) : DateTime.Now);
            int CN_ContractTermMonth = (!String.IsNullOrEmpty(collection["ContractTermMonth"]) ? int.Parse(collection["ContractTermMonth"]) : 0);
            decimal? CN_Amount = (!String.IsNullOrEmpty(collection["Amount"]) ? Convert.ToDecimal(collection["Amount"]) : 0);
            string CN_ContractDescription = (!String.IsNullOrEmpty(collection["ContractDescription"]) ? Convert.ToString(collection["ContractDescription"]) : "");


            int CN_hdnContractId = (!String.IsNullOrEmpty(collection["hdnContractId"]) ? int.Parse(collection["hdnContractId"]) : 0);
            int CN_hdnStatuslistId = (!String.IsNullOrEmpty(collection["hdnStatuslistId"]) ? int.Parse(collection["hdnStatuslistId"]) : 0);
            int CN_hdncallfrom = (!String.IsNullOrEmpty(collection["hdncallfrom"]) ? int.Parse(collection["hdncallfrom"]) : 0);
            int CN_hdnContractTypeListId = (!String.IsNullOrEmpty(collection["hdnContractTypeListId"]) ? int.Parse(collection["hdnContractTypeListId"]) : 0);
            string CN_hdnRedirect = (!String.IsNullOrEmpty(collection["hdnRedirect"]) ? Convert.ToString(collection["hdnRedirect"]) : "");
            string CN_hdnIsAddressChange = (!String.IsNullOrEmpty(collection["hdnIsAddressChange"]) ? Convert.ToString(collection["hdnIsAddressChange"]) : "");
            string CN_hdnAddress1 = (!String.IsNullOrEmpty(collection["hdnAddress1"]) ? Convert.ToString(collection["hdnAddress1"]) : "");
            string CN_hdnAddress2 = (!String.IsNullOrEmpty(collection["hdnAddress2"]) ? Convert.ToString(collection["hdnAddress2"]) : "");
            string CN_hdnCity = (!String.IsNullOrEmpty(collection["hdnCity"]) ? Convert.ToString(collection["hdnCity"]) : "");
            int CN_hdnStateListId = (!String.IsNullOrEmpty(collection["hdnStateListId"]) ? int.Parse(collection["hdnStateListId"]) : 0);
            string CN_hdnPostalCode = (!String.IsNullOrEmpty(collection["hdnPostalCode"]) ? Convert.ToString(collection["hdnPostalCode"]) : "");
            decimal? CN_hdnAmount = (!String.IsNullOrEmpty(collection["hdnAmount"]) ? Convert.ToDecimal(collection["hdnAmount"]) : 0);


            oContractViewModel.AccountTypeListId = CN_AccountTypeList;
            oContractViewModel.AddressId = 0;
            oContractViewModel.AgreementTypeListId = CN_AgreementTypeList;
            oContractViewModel.Amount = CN_Amount;
            oContractViewModel.AmountSubjectToFee = 0;
            oContractViewModel.ContractDescription = CN_ContractDescription;
            oContractViewModel.ContractId = CN_hdnContractId;
            oContractViewModel.ContractTermMonth = CN_ContractTermMonth;
            oContractViewModel.ContractTypeListId = CN_ContractTypeList;
            oContractViewModel.CreatedBy = LoginUserId;
            oContractViewModel.CreatedDate = DateTime.Now;
            oContractViewModel.CustomerId = CN_hdnContractCustomerId;
            oContractViewModel.ExpirationDate = CN_Contract_ExpirationDate;
            oContractViewModel.InitialCleanAmount = 0;
            oContractViewModel.isActive = true;
            oContractViewModel.PurchaseOrderNo = CN_PurchaseOrderNo;
            oContractViewModel.SignDate = CN_Contract_SignDate;
            oContractViewModel.SoldById = CN_SoldById;
            oContractViewModel.StartDate = CN_Contract_StartDate;
            oContractViewModel.StatusListId = CN_hdnStateListId;
            oContractViewModel.ContractTermMonth = CN_ContractTermMonth;



            
            //fullCustomerViewModel.Contract.ContractStatusList = Convert.ToInt32(collection["ContractStatusList"]);
            //fullCustomerViewModel.Contract.ContractStatusReasonListId = CN_hdnContractTypeListId;
            //fullCustomerViewModel.Contract.ContractTypeListId = CN_ContractTypeList;
            //fullCustomerViewModel.Contract.AccountTypeListId = CN_AccountTypeList;
            //fullCustomerViewModel.Contract.AgreementTypeListId = CN_AgreementTypeList;
            if (CN_ContractTypeList == 2)
            {
                if (Convert.ToInt32(collection["ContractTypeList"]) == 1)
                {
                    oContractViewModel.StatusListId = 38;
                    var data = CustomerService.GetCustomerById(CN_hdnContractCustomerId);
                    data.StatusListId = 38;
                    data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
                    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId, LoginUserId);
                    //return Redirect("/CRM/CRMRegionOperation/Index");
                }

                if (Convert.ToInt32(collection["ContractTypeList"]) == 3)
                {
                    oContractViewModel.StatusListId = 38;
                    var data = CustomerService.GetCustomerById(CN_hdnContractCustomerId);
                    data.StatusListId = 38;
                    data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
                    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    //return Redirect("/CRM/CRMRegionOperation/Index");
                    //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId,LoginUserId);
                }
            }

            fullCustomerViewModel.Contract = oContractViewModel;

            //fullCustomerViewModel.Contract.CustomerId = CN_hdnContractCustomerId;
            ////fullCustomerViewModel.Contract.ContractTermListId = fullCustomerViewModel.Contract.ContractTermListId;
            //fullCustomerViewModel.Contract.ContractTermMonth = CN_ContractTermMonth;
            //fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
            //fullCustomerViewModel.Contract.isActive = true;
            fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();



            //ContractDetail 
            ContractDetailViewModel oContractDetail = new ContractDetailViewModel();
            List<ContractDetailViewModel>  lstContractDetail = new List<ContractDetailViewModel>();



            
            var frmC = collection;
            int ln = 0;
            for (int i = 1; i <= 20; i++)
            {
                if(!String.IsNullOrEmpty(collection["ContractDetailId" + i]))
                {
                    ln++;
                    oContractDetail = new ContractDetailViewModel();
                    oContractDetail.ContractDetailId = (!String.IsNullOrEmpty(collection["ContractDetailId"+i]) ? int.Parse(collection["ContractDetailId" + i]) : 0);//
                    oContractDetail.ContractId = (!String.IsNullOrEmpty(collection["ContractId" + i]) ? int.Parse(collection["ContractId" + i]) : 0);//;ContractId1
                    oContractDetail.AccountRebate = false;// (!String.IsNullOrEmpty(collection["ContractDetailId1"]) ? int.Parse(collection["ContractDetailId1"]) : 0);//;
                    oContractDetail.Amount = (!String.IsNullOrEmpty(collection["ContractDetail_Amount" + i]) ? decimal.Parse(collection["ContractDetail_Amount" + i]) : 0);//;
                    oContractDetail.BillingFrequencyListId = (!String.IsNullOrEmpty(collection["BillingFrequencyListId" + i]) ? int.Parse(collection["BillingFrequencyListId" + i]) : 0);//;
                    oContractDetail.BPPAdmin = false;
                    oContractDetail.CleanFrequencyListId = (!String.IsNullOrEmpty(collection["ContractDetail_CleanFrequency" + i]) ? int.Parse(collection["ContractDetail_CleanFrequency" + i]) : 0);//;
                    oContractDetail.CleanTimes = (!String.IsNullOrEmpty(collection["ContractDetail_CleanTimes" + i]) ? int.Parse(collection["ContractDetail_CleanTimes" + i]) : 0);//;      
                    oContractDetail.CPIIncrease = (!String.IsNullOrEmpty(collection["ContractDetail_CPIIncrease" + i]) ? bool.Parse(collection["ContractDetail_CPIIncrease" + i]) : false);//;
                    oContractDetail.CreatedBy = LoginUserId;
                    oContractDetail.CreatedDate = DateTime.Now;
                    oContractDetail.Description = (!String.IsNullOrEmpty(collection["ContractDetail_Description" + i]) ? collection["ContractDetail_Description" + i].ToString() : "");
                    oContractDetail.EndTime = (!String.IsNullOrEmpty(collection["Contract_EndTime" + i]) ? DateTime.Parse(collection["Contract_EndTime" + i]) : DateTime.Now);
                    oContractDetail.Fri = (!String.IsNullOrEmpty(collection["ContractDetail_Fri" + i]) ? bool.Parse(collection["ContractDetail_Fri" + i]) : false);
                    oContractDetail.LineNumber = ln;
                    oContractDetail.Mon = (!String.IsNullOrEmpty(collection["ContractDetail_Mon"+i]) ? bool.Parse(collection["ContractDetail_Mon" + i]) : false);//;
                    oContractDetail.Sat = (!String.IsNullOrEmpty(collection["ContractDetail_Sat" + i]) ? bool.Parse(collection["ContractDetail_Sat" + i]) : false);//;
                    oContractDetail.SeparateInvoice = (!String.IsNullOrEmpty(collection["ContractDetail_SeparateInvoice" + i]) ? bool.Parse(collection["ContractDetail_SeparateInvoice" + i]) : false);//;
                    oContractDetail.ServiceTypeListId = (!String.IsNullOrEmpty(collection["ServiceTypeListId" + i]) ? int.Parse(collection["ServiceTypeListId" + i]) : 0);//;
                    oContractDetail.SquareFootage = (!String.IsNullOrEmpty(collection["ContractDetail_SquareFootage" + i]) ? (collection["ContractDetail_SquareFootage" + i]).ToString() : "");//;
                    oContractDetail.StartTime = (!String.IsNullOrEmpty(collection["Contract_EndTime" + i]) ? DateTime.Parse(collection["Contract_StartTime" + i]) : DateTime.Now);//;
                    oContractDetail.SubjectToFees = false;
                    oContractDetail.Sun = (!String.IsNullOrEmpty(collection["ContractDetail_Sun" + i]) ? bool.Parse(collection["ContractDetail_Sun" + i]) : false);//;1
                    oContractDetail.TaxExcempt = (!String.IsNullOrEmpty(collection["ContractDetail_TaxExcempt"+i]) ? bool.Parse(collection["ContractDetail_TaxExcempt" + i]) : false);//;ContractDetail_TaxExcempt1
                    oContractDetail.Thur = (!String.IsNullOrEmpty(collection["ContractDetail_Thur" + i]) ? bool.Parse(collection["ContractDetail_Thur" + i]) : false);//;1
                    oContractDetail.Tues = (!String.IsNullOrEmpty(collection["ContractDetail_Tues" + i]) ? bool.Parse(collection["ContractDetail_Tues" + i]) : false);//;1
                    oContractDetail.Wed = (!String.IsNullOrEmpty(collection["ContractDetail_Wed" + i]) ? bool.Parse(collection["ContractDetail_Wed"+i]) : false);//;
                    oContractDetail.WeekEnd = (!String.IsNullOrEmpty(collection["ContractDetail_WeekEnd" + i]) ? bool.Parse(collection["ContractDetail_WeekEnd" + i]) : false);//;
                    lstContractDetail.Add(oContractDetail);
                }
            }


            //int CN_hdnContractId = (!String.IsNullOrEmpty(collection["hdnContractId"]) ? int.Parse(collection["hdnContractId"]) : 0);


            //int? ContId = fullCustomerViewModel.Contract.ContractTypeListId;
            int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
            //if (fullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            //{
            //    return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            //}

            //ViewBag.CurrentMenu = "CustomerGeneral";

            //ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Customer");
            //BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "General");
            //if (fullCustomerViewModel.BillingContact.ClassId < 1)
            //{
            //    BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Add");
            //    // return View();
            //}
            //else if (fullCustomerViewModel != null)
            //{
            //    BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }) + "/" + fullCustomerViewModel.CustomerViewModel.CustomerId, "Manage");
            //}
            int addressId = 0;
            //if (collection["hdnIsAddressChange"] == "1")
            //{
            //    fullCustomerViewModel.ContractAddress.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            //    fullCustomerViewModel.ContractAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            //    fullCustomerViewModel.ContractAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            //    if (collection["ContractStatesList"] != null && collection["ContractStatesList"] != "")
            //    {
            //        fullCustomerViewModel.ContractAddress.StateListId = Convert.ToInt32(collection["ContractStatesList"]);
            //        int id = Convert.ToInt32(collection["ContractStatesList"]);
            //        string state = CustomerService.GetStatesName(id);
            //        fullCustomerViewModel.ContractAddress.StateName = state.Trim();

            //    }
            //    fullCustomerViewModel.Contract.StatusListId = Convert.ToInt32(collection["hdncallfrom"]);
            //    fullCustomerViewModel.ContractAddress.IsActive = true;
            //    fullCustomerViewModel.ContractAddress.CreatedDate = DateTime.Now;
            //    fullCustomerViewModel.ContractAddress.CreatedBy = LoginUserId;
            //    var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullCustomerViewModel.ContractAddress.FullAddress));
            //    if (_latlng.results.Count() > 0)
            //    {
            //        fullCustomerViewModel.ContractAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
            //        fullCustomerViewModel.ContractAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
            //    }
            //    int OldAddId = (fullCustomerViewModel.ContractAddress != null ? fullCustomerViewModel.ContractAddress.AddressId : 0);

            //    fullCustomerViewModel.ContractAddress = CustomerService.AddNewAddressOldInactive(fullCustomerViewModel.ContractAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
            //    // Update TaxRate Address Id 
            //    CustomerService.UpdateTaxRateAddress(OldAddId, fullCustomerViewModel.ContractAddress.AddressId, fullCustomerViewModel.CustomerViewModel.CustomerId);
            //    addressId = fullCustomerViewModel.ContractAddress.AddressId;
            //}
            //if (collection["ContractStatusList"] != null && collection["ContractStatusList"] != "")
            //{
            //    fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractStatusList"]);
            //}

            //if (collection["ContractServiceTypeList"] != null && collection["ContractServiceTypeList"] != "")
            //{
            //    fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractServiceTypeList"]);
            //}

            //if (collection["ContractTypeList"] != null && collection["ContractTypeList"] != "")
            //{
            //    fullCustomerViewModel.Contract.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            //}

            //if (collection["AccountTypeList"] != null && collection["AccountTypeList"] != "")
            //{
            //    fullCustomerViewModel.Contract.AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]);
            //}
            //if (collection["AgreementTypeList"] != null && collection["AgreementTypeList"] != "")
            //{
            //    fullCustomerViewModel.Contract.AgreementTypeListId = Convert.ToInt32(collection["AgreementTypeList"]);
            //}
            //if (ContId == 2)
            //{
            //    if (Convert.ToInt32(collection["ContractTypeList"]) == 1)
            //    {
            //        fullCustomerViewModel.Contract.StatusListId = 38;
            //        var data = CustomerService.GetCustomerById(fullCustomerViewModel.CustomerViewModel.CustomerId);
            //        data.StatusListId = 38;
            //        data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            //        fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
            //        //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId, LoginUserId);
            //        //return Redirect("/CRM/CRMRegionOperation/Index");
            //    }

            //    if (Convert.ToInt32(collection["ContractTypeList"]) == 3)
            //    {
            //        fullCustomerViewModel.Contract.StatusListId = 38;
            //        var data = CustomerService.GetCustomerById(fullCustomerViewModel.CustomerViewModel.CustomerId);
            //        data.StatusListId = 38;
            //        data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            //        fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
            //        //return Redirect("/CRM/CRMRegionOperation/Index");

            //        //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId,LoginUserId);

            //    }
            //}


            //fullCustomerViewModel.Contract.CustomerId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            ////fullCustomerViewModel.Contract.ContractTermListId = fullCustomerViewModel.Contract.ContractTermListId;
            //fullCustomerViewModel.Contract.ContractTermMonth = fullCustomerViewModel.Contract.ContractTermMonth;
            //fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
            //fullCustomerViewModel.Contract.isActive = true;
            //fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

            //ViewBag.MainStatesList = getUsStatesList();
            //ViewBag.BillingStatesList = getUsStatesList();
            //ViewBag.TaxAuthority = getTaxAuthority();
            //fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));

            //fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));
            //if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
            //{
            //    string Address = string.Empty;
            //    if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
            //    {
            //        Address = fullCustomerViewModel.MainAddress.Address1;
            //    }
            //    if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
            //    {
            //        Address += " " + fullCustomerViewModel.MainAddress.City;
            //    }
            //    if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
            //    {
            //        var States = CustomerService.GetStateList();
            //        string StateName = States.Where(one => one.StateListId == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
            //        Address += " " + StateName;
            //    }
            //    if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
            //    {
            //        Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
            //    }
            //    ViewBag.Address = Address;
            //}

            //var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            //if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractTypeListId != null)
            //{
            //    ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", fullCustomerViewModel.Contract.ContractTypeListId);
            //}
            //else
            //{
            //    ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            //}

            //var ContractStatusList = CustomerService.GetARStatusReasonList().ToList();
            //if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractStatusReasonListId != null)
            //{
            //    ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", fullCustomerViewModel.Contract.ContractStatusReasonListId);
            //}
            //else
            //{
            //    ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            //}

            //var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            //if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.AccountTypeListId != null)
            //{
            //    ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", fullCustomerViewModel.Contract.AccountTypeListId);
            //}
            //else
            //{
            //    ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            //}

            //if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null)
            //{
            //    fullCustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == fullCustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
            //}



            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                if (fullCustomerViewModel.CustomerViewModel.CustomerId > 0)
                    return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                return View(fullCustomerViewModel);
            }
            else if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("MaintenanceStepFour", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            return Json(addressId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveCustomerContractDetails(FullCustomerViewModel fullCustomerViewModel, FormCollection collection)
        {



            int? ContId = fullCustomerViewModel.Contract.ContractTypeListId;
            int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
            if (fullCustomerViewModel.ButtonType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Back))
            {
                return RedirectToAction("MaintenanceStepOne", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }

            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Customer");
            BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "General");
            if (fullCustomerViewModel.BillingContact.ClassId < 1)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }), "Add");
                // return View();
            }
            else if (fullCustomerViewModel != null)
            {
                BreadCrumb.Add(Url.Action("Maintenance", "Customer", new { area = "Portal" }) + "/" + fullCustomerViewModel.CustomerViewModel.CustomerId, "Manage");
            }
            int addressId = 0;
            if (collection["hdnIsAddressChange"] == "1")
            {
                fullCustomerViewModel.ContractAddress.ClassId = fullCustomerViewModel.CustomerViewModel.CustomerId;
                fullCustomerViewModel.ContractAddress.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                fullCustomerViewModel.ContractAddress.ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
                if (collection["ContractStatesList"] != null && collection["ContractStatesList"] != "")
                {
                    fullCustomerViewModel.ContractAddress.StateListId = Convert.ToInt32(collection["ContractStatesList"]);
                    int id = Convert.ToInt32(collection["ContractStatesList"]);
                    string state = CustomerService.GetStatesName(id);
                    fullCustomerViewModel.ContractAddress.StateName = state.Trim();

                }
                fullCustomerViewModel.Contract.StatusListId = Convert.ToInt32(collection["hdncallfrom"]);
                fullCustomerViewModel.ContractAddress.IsActive = true;
                fullCustomerViewModel.ContractAddress.CreatedDate = DateTime.Now;
                fullCustomerViewModel.ContractAddress.CreatedBy = LoginUserId;
                var _latlng = GetLatLongByAddress(HttpUtility.UrlEncode(fullCustomerViewModel.ContractAddress.FullAddress));
                if (_latlng.results.Count() > 0)
                {
                    fullCustomerViewModel.ContractAddress.Latitude = decimal.Parse(_latlng.results[0].geometry.location.lat.ToString());
                    fullCustomerViewModel.ContractAddress.Longitude = decimal.Parse(_latlng.results[0].geometry.location.lng.ToString());
                }
                int OldAddId = (fullCustomerViewModel.ContractAddress != null ? fullCustomerViewModel.ContractAddress.AddressId : 0);

                fullCustomerViewModel.ContractAddress = CustomerService.AddNewAddressOldInactive(fullCustomerViewModel.ContractAddress.ToModel<Address, AddressViewModel>()).ToModel<AddressViewModel, Address>();
                // Update TaxRate Address Id 
                CustomerService.UpdateTaxRateAddress(OldAddId, fullCustomerViewModel.ContractAddress.AddressId, fullCustomerViewModel.CustomerViewModel.CustomerId);
                addressId = fullCustomerViewModel.ContractAddress.AddressId;
            }
            if (collection["ContractStatusList"] != null && collection["ContractStatusList"] != "")
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractStatusList"]);
            }

            if (collection["ContractServiceTypeList"] != null && collection["ContractServiceTypeList"] != "")
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractServiceTypeList"]);
            }

            if (collection["ContractTypeList"] != null && collection["ContractTypeList"] != "")
            {
                fullCustomerViewModel.Contract.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            }

            if (collection["AccountTypeList"] != null && collection["AccountTypeList"] != "")
            {
                fullCustomerViewModel.Contract.AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]);
            }
            if (collection["AgreementTypeList"] != null && collection["AgreementTypeList"] != "")
            {
                fullCustomerViewModel.Contract.AgreementTypeListId = Convert.ToInt32(collection["AgreementTypeList"]);
            }
            if (ContId == 2)
            {
                if (Convert.ToInt32(collection["ContractTypeList"]) == 1)
                {
                    fullCustomerViewModel.Contract.StatusListId = 38;
                    var data = CustomerService.GetCustomerById(fullCustomerViewModel.CustomerViewModel.CustomerId);
                    data.StatusListId = 38;
                    data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
                    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId, LoginUserId);
                    //return Redirect("/CRM/CRMRegionOperation/Index");
                }

                if (Convert.ToInt32(collection["ContractTypeList"]) == 3)
                {
                    fullCustomerViewModel.Contract.StatusListId = 38;
                    var data = CustomerService.GetCustomerById(fullCustomerViewModel.CustomerViewModel.CustomerId);
                    data.StatusListId = 38;
                    data.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
                    fullCustomerViewModel.CustomerViewModel = CustomerService.SaveCustomers(data.ToModel<Customer, CustomerViewModel>()).ToModel<CustomerViewModel, Customer>();
                    //return Redirect("/CRM/CRMRegionOperation/Index");

                    //CustomerService.UpdateCustomerInactive(fullCustomerViewModel.CustomerViewModel.CustomerId,LoginUserId);

                }
            }


            fullCustomerViewModel.Contract.CustomerId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            //fullCustomerViewModel.Contract.ContractTermListId = fullCustomerViewModel.Contract.ContractTermListId;
            fullCustomerViewModel.Contract.ContractTermMonth = fullCustomerViewModel.Contract.ContractTermMonth;
            fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
            fullCustomerViewModel.Contract.isActive = true;
            fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();
            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));

            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));
            if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
            {
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
                {
                    Address = fullCustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.City;
                }
                if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address;
            }

            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", fullCustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            var ContractStatusList = CustomerService.GetARStatusReasonList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractStatusReasonListId != null)
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", fullCustomerViewModel.Contract.ContractStatusReasonListId);
            }
            else
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            }

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", fullCustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }

            if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null)
            {
                fullCustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == fullCustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
            }



            if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Save))
            {
                if (fullCustomerViewModel.CustomerViewModel.CustomerId > 0)
                    return RedirectToAction("MaintenanceStepThree", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
                return View(fullCustomerViewModel);
            }
            else if (btnType == Convert.ToInt32(JKApi.Business.Enumeration.ButtonType.Continue))
            {
                return RedirectToAction("MaintenanceStepFour", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            }
            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            return Json(addressId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCustomerContractPopupDetail(int ContractId, int ContractDetailId)
        {
            FullCustomerViewModel FullCustomerViewModel = new FullCustomerViewModel();

            if (ContractId > 0 && ContractDetailId > 0)
            {
                FullCustomerViewModel.ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractDetailId == ContractDetailId).FirstOrDefault().ToModel<ContractDetailViewModel, ContractDetail>();
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }

                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractPopupDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
            else
            {
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel.OrderBy(x => x.name), "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }
                FullCustomerViewModel.ContractDetail.ContractId = ContractId;
                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractPopupDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
        }

        [HttpGet]
        public ActionResult AddCustomerContractDescriptionDetail(int ContractId, int ContractDetailId)
        {
            FullCustomerViewModel FullCustomerViewModel = new FullCustomerViewModel();

            if (ContractId > 0 && ContractDetailId > 0)
            {
                FullCustomerViewModel.ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractDetailId == ContractDetailId).FirstOrDefault().ToModel<ContractDetailViewModel, ContractDetail>();
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }

                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractDescriptionDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
            else
            {
                var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.ServiceTypeListId != null)
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", FullCustomerViewModel.ContractDetail.ServiceTypeListId);
                }
                else
                {
                    ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
                }
                var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
                if (FullCustomerViewModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.BillingFrequencyListId != null)
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", FullCustomerViewModel.ContractDetail.BillingFrequencyListId);
                }
                else
                {
                    ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
                }
                var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
                if (CleanFrequencyListModel != null && FullCustomerViewModel.ContractDetail != null && FullCustomerViewModel.ContractDetail.CleanFrequencyListId != null)
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", FullCustomerViewModel.ContractDetail.CleanFrequencyListId);
                }
                else
                {
                    ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
                }
                FullCustomerViewModel.ContractDetail.ContractId = ContractId;
                return View("~/Areas/Portal/Views/Customer/_AddCustomerContractDescriptionDetail.cshtml", FullCustomerViewModel.ContractDetail);
            }
        }

        [HttpPost]
        public ActionResult SaveCustomerContractPopupDetail(ContractDetailViewModel ContractDetailDescriptionViewModel, FormCollection collection)
        {
            if (ContractDetailDescriptionViewModel != null && ContractDetailDescriptionViewModel.ContractDetailId > 0)
            {
                ContractDetailDescriptionViewModel.CreatedDate = DateTime.Now;
                ContractDetailDescriptionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                ContractDetailDescriptionViewModel = CustomerService.SaveContractDetail(ContractDetailDescriptionViewModel.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();

                return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractId, success = true }, JsonRequestBehavior.AllowGet);
            }
            else if (ContractDetailDescriptionViewModel.ContractId > 0)
            {
                ContractDetailDescriptionViewModel.LineNumber = getLineNo(ContractDetailDescriptionViewModel.ContractId) + 1;
                ContractDetailDescriptionViewModel.ContractId = ContractDetailDescriptionViewModel.ContractId;
                ContractDetailDescriptionViewModel.CreatedDate = DateTime.Now;
                ContractDetailDescriptionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());

                ContractDetailDescriptionViewModel = CustomerService.SaveContractDetail(ContractDetailDescriptionViewModel.ToModel<ContractDetail, ContractDetailViewModel>()).ToModel<ContractDetailViewModel, ContractDetail>();
                return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractId, success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { aaData = ContractDetailDescriptionViewModel, id = ContractDetailDescriptionViewModel.ContractDetailId, success = true }, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("MaintenanceStepTwo", "Franchise", new { area = "Portal", id = FullFranchiseeViewModel.BusinessInfo.FranchiseeId });
        }

        #region ::  Customer Distribution ::

        [HttpGet]
        public ActionResult RenderCustomerDistributionDetailPopup(int id, bool isCRM = false)
        {
            CustomerDistributionDetailsModel CustomerDistributionDetailsModel = new CustomerDistributionDetailsModel();
            if (id > 0)
            {
                CustomerDistributionDetailsModel = CustomerService.GetCustomerDistributionDetails(id);
            }
            ViewBag.CustomerId = id;
            ViewBag.isCRM = isCRM;
            return PartialView("_CustomerDistributionDetailPopup", CustomerDistributionDetailsModel);
        }

        #endregion ::  Customer Distribution ::

        public JsonResult StateName(int stateid)
        {
            return Json(CustomerService.GetStateName(stateid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLogCustomerDetail(int id)
        {
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            return Json(new
            {
                success = true,
                result = CustomerViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFeeFinderList(int id)
        {
            return PartialView("_CustomerFinderFeeDetailListPopup", CustomerService.GetCustomerFinderFeeDetailList(SelectedRegionId, id));
        }

        public ActionResult GetFeeFinderDetail(int id)
        {
            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");
            return PartialView("_FinderFeeDetailPopup", CustomerService.GetCCFinderFeeDetail(SelectedRegionId, id));
        }

        public FileResult CFFDetailListExportPDF(int id)
        {
            string HTMLContent = string.Empty;
            HTMLContent += CRenderPartialViewToString("_CustomerFinderFeeDetailPDF", CustomerService.GetCustomerFinderFeeDetailList(SelectedRegionId, id));
            return File(GetCPDF(HTMLContent), "application/pdf", "CFF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
        }

        public JsonResult CFFDetailListPrint(int id)
        {
            string HTMLContent = string.Empty;

            HTMLContent += CRenderPartialViewToString("_CustomerFinderFeeDetailPDF", CustomerService.GetCustomerFinderFeeDetailList(SelectedRegionId, id));
            var retFileName = "CFF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
            var retPath = "/Upload/InvoiceFiles/" + retFileName;
            var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), retFileName);
            System.IO.File.WriteAllBytes(path, GetCPDF(HTMLContent)); // Requires System.IO
            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        protected string CRenderPartialViewToString(string viewName, object model)
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

        public byte[] GetCPDF(string pHTML)
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
        }

        public byte[] GetPDFPortrait(string pHTML)
        {
            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    document.SetPageSize(iTextSharp.text.PageSize.A4);
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

        public ActionResult GetServiceLog(int id)
        {
            try
            {
                var response = CustomerService.GetServiceLog(id);
                var result = from f in response
                             select new
                             {
                                 CallDate = f.CallDate != null ? f.CallDate : null,
                                 CallTime = f.CallTime != null ? f.CallTime.Value.ToString() : string.Empty,
                                 Status = f.StatusResultListId != null ? CustomerService.GetStatus(f.StatusResultListId) : null,
                                 SpokeWith = f.SpokeWith != null ? f.SpokeWith : string.Empty,
                                 Action = f.Action != null ? f.Action : string.Empty,
                                 CallBack = f.CallBack != null ? f.CallBack : null,
                                 Comments = f.Comments,
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

        public ActionResult GetCollectionLog(int id)
        {
            try
            {
                var response = CustomerService.GetCollectionLog(id);
                var result = from f in response
                             select new
                             {
                                 CallDate = f.CallDate != null ? f.CallDate : null,
                                 CallTime = f.CallTime != null ? f.CallTime : null,
                                 Status = f.StatusResultListId != null ? CustomerService.GetStatus(f.StatusResultListId) : null,
                                 SpokeWith = f.SpokeWith != null ? f.SpokeWith : string.Empty,
                                 Action = f.Action != null ? f.Action : string.Empty,
                                 CallBack = f.CallBack != null ? f.CallBack : null,
                                 Comments = f.Comments,
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

        public ActionResult GetServiceLogList(int CustomerId, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            try
            {
                var response = CustomerService.ServiceCallLogList(CustomerId, StartDate, EndDate, month, year);
                var result = from f in response
                             select new
                             {
                                 CallDate = f.CallDate != null ? f.CallDate : null,
                                 CallTime = f.CallTime != null ? f.CallTime.Value.ToString() : string.Empty,
                                 Status = f.StatusResultListId != null ? CustomerService.GetStatus(f.StatusResultListId) : null,
                                 SpokeWith = f.SpokeWith != null ? f.SpokeWith : string.Empty,
                                 Action = f.Action != null ? f.Action : string.Empty,
                                 CallBack = f.CallBack != null ? f.CallBack : null,
                                 Comments = f.Comments,
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

        public ActionResult GetStatus(int id)
        {
            try
            {
                var response = CustomerService.GetStatus(id);
                return Json(new
                {
                    response,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region PaymentGateway

        [HttpGet]
        public JsonResult PayWithCC(string CardNumber, string CardHolderName, string CardExpiry, string CardCVC, decimal Amount, string ClassID, bool IsProfile, string CustomerNo, bool IsCheckedPrevCard, string PaymentProfileID)
        {
            GeneralService generalService = new GeneralService();
            jkDatabaseEntities jk = new jkDatabaseEntities();
            Guid PaymentPRofileID = Guid.NewGuid();
            PaymentProfileDetail PaymentProfileDetail;
            dynamic response = null;
            Guid? OrderTransactionID = null;
            int RegionID = 0;
            string userRegion = _claimView.GetCLAIM_SELECTED_COMPANY_ID().ToString();
            if (userRegion != null)
            {
                RegionID = Convert.ToInt32(userRegion);
            }
            PaymentGatewayDetail PGD = generalService.GetPaymentGatewayList().Where(r => r.RegionID == RegionID && r.IsActive == true).FirstOrDefault();
            if (PGD != null && PGD.merchantid != null && PGD.merchantid != "")
            {
                var data = generalService.AddressList(ClassID);
                PaymentProfileDetail = new PaymentProfileDetail();
                PaymentProfileDetail.CreatedDate = DateTime.Now;
                PaymentProfileDetail.Id = PaymentPRofileID;
                PaymentProfileDetail.FKClassId = Convert.ToInt32(ClassID);
                PaymentProfileDetail.FKPaymentGatewayID = PGD.Id;
                PaymentProfileDetail.FKTypeListId = 1;
                generalService.InsertPaymentProfileDetail(PaymentProfileDetail);
                OrderTransactionID = OrderTransaction(PaymentProfileDetail, Amount);

                #region MxMarchantPayment

                string baseURL = "https://sandbox.api.mxmerchant.com/checkout/v3";
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                string endPointRequestToken = baseURL + "/oauth/1a/requesttoken";
                string endPointAccessToken = baseURL + "/oauth/1a/accesstoken";
                string createPayment = baseURL + "/payment";
                Mxmerchant mxmerchant = new Mxmerchant();
                string getPayment = baseURL + "/payment/{0}";
                string getPayments = baseURL + "/payment";
                string ConsumerKey = PGD.LoginID;
                string ConsumerSecret = PGD.TransactionKey;

                var queryString = new Dictionary<string, string>();
                queryString.Add("echo", "true");

                var p = new PriorityPayment.Payment();
                p.merchantId = PGD.merchantid;
                p.tenderType = "Card";
                p.amount = Amount.ToString();
                p.cardAccount = new PriorityPayment.CardAccount();
                p.cardAccount.number = CardNumber;
                p.cardAccount.expiryMonth = CardExpiry.Split('/')[0];
                p.cardAccount.expiryYear = CardExpiry.Split('/')[1];
                p.cardAccount.cvv = CardCVC;
                p.cardAccount.avsZip = data.PostalCode;
                p.cardAccount.avsStreet = data.Address1;

                PriorityPayment.PpsApiRequest apiRequest = new PriorityPayment.PpsApiRequest(baseURL, ConsumerKey, ConsumerSecret, PriorityPayment.AuthenticationMethod.OAuth);
                using (var httpRequest = apiRequest.BuildRequest(createPayment, queryString, System.Net.Http.HttpMethod.Post, p))
                {
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage Mxresponse = httpClient.SendAsync(httpRequest).Result;

                    var json = Mxresponse.Content.ReadAsStringAsync().Result;

                    mxmerchant = Mxmerchant.FromJson(json);

                    if (mxmerchant.Status == "Approved")
                    {
                        ViewBag.PGD = PGD;
                        TempData["PGD"] = PGD;
                        TempData["response"] = response;
                        TempData["PaymentProfileDetail"] = PaymentProfileDetail;
                        CreateOrderTransactionsResponse(mxmerchant, OrderTransactionID.Value, PaymentPRofileID, PGD);
                    }
                }

                #endregion MxMarchantPayment

                return Json(new
                {
                    aadata = mxmerchant,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!IsCheckedPrevCard)
                {
                    var data = generalService.AddressList(ClassID);
                    PaymentProfileDetail = new PaymentProfileDetail();
                    PaymentProfileDetail.CreatedDate = DateTime.Now;
                    PaymentProfileDetail.Id = PaymentPRofileID;
                    PaymentProfileDetail.FKClassId = Convert.ToInt32(ClassID);
                    PaymentProfileDetail.FKPaymentGatewayID = PGD.Id;
                    PaymentProfileDetail.FKTypeListId = 1;
                    generalService.InsertPaymentProfileDetail(PaymentProfileDetail);
                    var objCreditCardType = new creditCardType
                    {
                        cardNumber = CardNumber,
                        expirationDate = CardExpiry,
                        cardCode = CardCVC
                    };
                    var objcustomerAddressType = new customerAddressType
                    {
                        address = data.Address1,
                        zip = data.PostalCode,
                    };
                    OrderTransactionID = OrderTransaction(PaymentProfileDetail, Amount);
                    response = CreditCardPayment(objCreditCardType, objcustomerAddressType, PGD.LoginID, PGD.TransactionKey, Amount, "TestMode", CustomerNo);
                }
                else
                {
                    PaymentProfileDetail = jk.PaymentProfileDetails.Where(r => r.Id == new Guid(PaymentProfileID)).FirstOrDefault();

                    if (PaymentProfileDetail != null)
                    {
                        OrderTransactionID = OrderTransaction(PaymentProfileDetail, Amount);
                        response = AuthorizePaymentGateway.ChargeCustomerProfile(PGD.LoginID, PGD.TransactionKey, PaymentProfileDetail.CustomerProfileID, PaymentProfileDetail.CustomerPaymentProfileID, "TestMode", Amount);
                    }
                }

                bool isCreditCardPaymentSuccess = AuthorizenetCommon.CheckAuthorizeNetApiResponse(response);

                if (isCreditCardPaymentSuccess)
                {
                    ViewBag.PGD = PGD;
                    TempData["PGD"] = PGD;
                    TempData["response"] = response;
                    TempData["PaymentProfileDetail"] = PaymentProfileDetail;
                    CreateOrderTransactionsResponse(response, OrderTransactionID.Value, PaymentPRofileID, PGD);
                }
                return Json(new
                {
                    aadata = response,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private dynamic CreditCardPayment(creditCardType objCreditCardType, customerAddressType objcustomerAddressType, string ApiLoginID, string ApiTransactionKey, decimal TotalOrderPrice, string PaymentGatewayMode, string OrderId)
        {
            #region Order Items

            var i = 1;

            int j = 0;

            var lineItems = new lineItemType[i];
            lineItems[j] = new lineItemType
            {
                itemId = Convert.ToString(1),
                name = Convert.ToString("Invoices Payment"),
                quantity = 1,
                unitPrice = Convert.ToDecimal(TotalOrderPrice)
            };

            #endregion Order Items

            dynamic response = AuthorizePaymentGateway.ChargeCreditCard(ApiLoginID, ApiTransactionKey, Convert.ToDecimal(TotalOrderPrice), objCreditCardType, objcustomerAddressType, lineItems, PaymentGatewayMode, Convert.ToString(OrderId));

            return response;
        }

        private dynamic CreateCustomerProfileFromTransaction(PaymentGatewayDetail PGD, dynamic CCresponse)
        {
            dynamic response = AuthorizePaymentGateway.CreateCustomerProfileFromTransaction(PGD.LoginID, PGD.TransactionKey, CCresponse.transactionResponse.transId, "TestMode");
            return response;
        }

        public Guid OrderTransaction(PaymentProfileDetail PPD, decimal Amount)
        {
            Guid OrderID = Guid.NewGuid();

            OrderTransaction order = new JKApi.Data.DAL.OrderTransaction();

            order.Id = OrderID;
            order.FKPaymentProfileID = PPD.Id;
            order.Price = Amount;
            order.PriceWithTax = Amount;
            order.Tax = 0;
            order.CreatedDate = DateTime.Now;
            GeneralService generalService = new GeneralService();
            generalService.OrderTransaction(order);

            return OrderID;
        }

        private void CreateOrderTransactionsResponse(dynamic response, Guid orderId, Guid PaymentProfileID, PaymentGatewayDetail PGD)
        {
            OrderTransactionsResponse objOrderTransactionsResponse = new OrderTransactionsResponse();
            GeneralService generalService = new GeneralService();
            objOrderTransactionsResponse.FKOrderID = orderId;
            objOrderTransactionsResponse.CreatedDate = DateTime.Now;
            objOrderTransactionsResponse.FKPaymentProfileID = PaymentProfileID;
            objOrderTransactionsResponse.OrderTxReponseID = Guid.NewGuid();
            if (PGD.merchantid == null || PGD.merchantid == "")
            {
                objOrderTransactionsResponse.MessagesResultCode = Convert.ToString(response.messages.resultCode);
                objOrderTransactionsResponse.MessagesCode = Convert.ToString(response.messages.message[0].code);
                objOrderTransactionsResponse.MessagesText = Convert.ToString(response.messages.message[0].text);

                if (AuthorizenetCommon.IstransactionResponsePropertyExist(response))
                {
                    objOrderTransactionsResponse.ResponseCode = response.transactionResponse.responseCode;
                    objOrderTransactionsResponse.AuthCode = Convert.ToString(response.transactionResponse.authCode);
                    objOrderTransactionsResponse.AvsResultCode = Convert.ToString(response.transactionResponse.avsResultCode);

                    objOrderTransactionsResponse.CvvResultCode = Convert.ToString(response.transactionResponse.cvvResultCode);
                    objOrderTransactionsResponse.CavvResultCode = Convert.ToString(response.transactionResponse.cavvResultCode);

                    objOrderTransactionsResponse.TransId = Convert.ToString(response.transactionResponse.transId);
                    objOrderTransactionsResponse.RefTransId = Convert.ToString(response.transactionResponse.refTransID);
                    objOrderTransactionsResponse.TransHash = Convert.ToString(response.transactionResponse.transHash);

                    objOrderTransactionsResponse.AccountNumber = Convert.ToString(response.transactionResponse.accountNumber);
                    objOrderTransactionsResponse.EntryMode = null;
                    objOrderTransactionsResponse.AccountType = Convert.ToString(response.transactionResponse.accountType);

                    if (response.messages.resultCode == messageTypeEnum.Ok)
                    {
                        objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.messages[0].code);
                        objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.messages[0].description);
                    }
                    else
                    {
                        try
                        {
                            objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.errors[0].errorCode);
                            objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.errors[0].errorText);
                        }
                        catch
                        {
                            objOrderTransactionsResponse.TMessagesCode = null;
                            objOrderTransactionsResponse.TMessagesDescription = null;
                        }
                    }
                }
            }
            else
            {
                objOrderTransactionsResponse.MessagesResultCode = Convert.ToString(response.Status);
                objOrderTransactionsResponse.MessagesCode = "";
                objOrderTransactionsResponse.MessagesText = "";
                if (response.Status == "Approved")
                {
                    objOrderTransactionsResponse.ResponseCode = Convert.ToString(response.Id);
                    objOrderTransactionsResponse.AuthCode = Convert.ToString(response.AuthCode);
                    objOrderTransactionsResponse.AvsResultCode = Convert.ToString(response.Risk.AvsResponseCode);

                    objOrderTransactionsResponse.CvvResultCode = Convert.ToString(response.Risk.CvvResponseCode);
                    objOrderTransactionsResponse.CavvResultCode = "";

                    objOrderTransactionsResponse.TransId = Convert.ToString(response.BatchId);
                    objOrderTransactionsResponse.RefTransId = Convert.ToString(response.ClientReference);
                    objOrderTransactionsResponse.TransHash = Convert.ToString(response.CardAccount.Token);

                    objOrderTransactionsResponse.AccountNumber = Convert.ToString("XXXX" + response.CardAccount.Last4);
                    objOrderTransactionsResponse.EntryMode = null;
                    objOrderTransactionsResponse.AccountType = Convert.ToString(response.CardAccount.CardType);
                    objOrderTransactionsResponse.TMessagesCode = null;
                    objOrderTransactionsResponse.TMessagesDescription = null;

                    //if (response.messages.resultCode == messageTypeEnum.Ok)
                    //{
                    //    objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.messages[0].code);
                    //    objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.messages[0].description);
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        objOrderTransactionsResponse.TMessagesCode = Convert.ToString(response.transactionResponse.errors[0].errorCode);
                    //        objOrderTransactionsResponse.TMessagesDescription = Convert.ToString(response.transactionResponse.errors[0].errorText);
                    //    }
                    //    catch
                    //    {
                    //        objOrderTransactionsResponse.TMessagesCode = null;
                    //        objOrderTransactionsResponse.TMessagesDescription = null;
                    //    }

                    //}
                }
            }
            generalService.OrderTransactionsResponse(objOrderTransactionsResponse);
        }

        [HttpGet]
        public JsonResult CreateProfile()
        {
            PaymentGatewayDetail PGD = (PaymentGatewayDetail)TempData["PGD"];
            GeneralService generalService = new GeneralService();
            PaymentProfileDetail paymentProfileDetail;
            dynamic response = TempData["response"];
            paymentProfileDetail = (PaymentProfileDetail)TempData["PaymentProfileDetail"];
            paymentProfileDetail.AccountNumber = response.transactionResponse.accountNumber;
            paymentProfileDetail.AccountType = response.transactionResponse.accountType;
            dynamic CustomerProfileResponse = CreateCustomerProfileFromTransaction(PGD, response);
            bool isCustomerProfileCreatedSuccessfully = AuthorizenetCommon.CheckAuthorizeNetApiResponse(CustomerProfileResponse);
            if (isCustomerProfileCreatedSuccessfully)
            {
                paymentProfileDetail.CustomerProfileID = CustomerProfileResponse.customerProfileId;
                paymentProfileDetail.CustomerPaymentProfileID = CustomerProfileResponse.customerPaymentProfileIdList[0];
                generalService.UpdatePaymentProfile(paymentProfileDetail);
            }

            return Json(new
            {
                aadata = CustomerProfileResponse,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBillingAddress(string id)
        {
            GeneralService generalService = new GeneralService();
            //var data = generalService.AddressList(id);
            // return Json(new
            // {
            //     aadata = data,
            // }, JsonRequestBehavior.AllowGet);

            var PPD = generalService.PaymentProfileDetail(id);
            if (PPD != null)
            {
                var data = new
                {
                    PPD.AccountNumber,
                    PPD.AccountType,
                    PPD.Id
                };
                return Json(new
                {
                    aadata = data,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    aadata = "",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion PaymentGateway

        #region :: Pending Approval List ::

        public ActionResult PendingApprovalList(int? id)
        {
            CustomerSearchResultViewModelListModel model = new CustomerSearchResultViewModelListModel();
            string StatusList = "";
            //if (_claimView.GetCLAIM_ROLE_TYPE() == "Accounting-User")
            //{
            //    StatusList = "35";
            //}
            //else
            //{
            //    //StatusList = "4,35";
            //    StatusList = "4";
            //}
            StatusList = "4";
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PendingApprovalList", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("PendingApprovalList", "Customer", new { area = "Portal" }) + "?id=4", "List");
            int StatusId = Convert.ToInt32(4);
            int CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            model.lstCustomerSearchResultViewModel = CustomerService.GetCustomerSearchList(StatusId, CustomerContactTypeList, StatusList);

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name");

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text");

            //var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).DistinctBy(one => one.Customer).ToList();
            //response.Select(x => x.MainAddress.StateListId);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View(model);
        }

        public ActionResult DashboardRejectedActionClick(int CustomerId)
        {
            TempData["CustomerID"] = CustomerId;
            bool flag = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.NotificationMessageForDashboards.Where(r => r.CustomerID == CustomerId).FirstOrDefault();
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

        #endregion :: Pending Approval List ::

        #region :: Customer Maintenance Pending List ::

        public ActionResult CustomerMaintenancePendingList(int? id, string regions = "")
        {
            List<CustomerPendingMaintenanceListViewModel> model = new List<CustomerPendingMaintenanceListViewModel>();
            string StatusList = "4";
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerMaintenancePendingList", "Customer", new { area = "Portal" }), "General");
            BreadCrumb.Add(Url.Action("CustomerMaintenancePendingList", "Customer", new { area = "Portal" }) + "?id=4", "Customer Maintenance Pending List");
            int StatusId = Convert.ToInt32(4);
            int CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);

            model = CustomerService.GetCustomerPendingMaintenanceList(regions != "" ? regions : SelectedRegionId.ToString());

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name");

            ViewBag.selectedRegionId = SelectedRegionId;

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text");

            //var response = CustomerService.GetCustomerByStatus(StatusId, CustomerTypeList, CustomerContactTypeList).DistinctBy(one => one.Customer).ToList();
            //response.Select(x => x.MainAddress.StateListId);
            return View(model);
        }

        #endregion :: Customer Maintenance Pending List ::

        #region :: Collection Call Log ::

        [HttpGet]
        public JsonResult CollectionCallListResultData(int? id, string fd = "", string td = "", string st = "")
        {
            DateTime fromDate = DateTime.Parse(fd);
            DateTime toDate = DateTime.Parse(td);
            var list = CustomerService.GetServiceCollectionCallLogCustomersListForSearch(id ?? 0, fromDate, toDate, st);
            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult CustomerCollectionLogPopup(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != null && id != 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(id);
                ViewBag.CollectionCallLog = CustomerService.GetServiceCollectionCallLogCustomersList(id ?? 0);
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();
                    var customer = CustomerService.GetCustomerById(id ?? 0);
                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    customerDetailViewModel.RegionId = String.IsNullOrEmpty(customer.RegionId.ToString()) ? String.Empty : customer.RegionId.ToString();
                }
            }
            ViewBag.CustomerID = id;
            ViewBag.CustomerDetail = customerDetailViewModel;

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            //Franchises

            ViewBag.Franchisees = CustomerService.getFrancisesByCustomerId((id ?? 0));
            //var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            //ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");

            var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(id));
            if (BillSetting != null)
            {
                ViewBag.ARStatus = (BillSetting.ARStatus != null ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty);
            }

            var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(id));
            if (AgingData != null)
            {
                ViewBag.AgingData = AgingData;
            }

            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            if (userLista.Count > 0)
            {
                userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);

                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            return PartialView("_CustomerCollectionsCallLogPopup");
        }

        [HttpPost]
        public ActionResult SaveCollectionCallLogDetails(CollectionsCallLogModel objCollectionsCallLog)
        {
            if (objCollectionsCallLog.strCallBack != null)
            {
                //DateTime dtCallBack = DateTime.ParseExact(objCollectionsCallLog.strCallBack, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                DateTime dtCallBack = Convert.ToDateTime(objCollectionsCallLog.strCallBack);
                objCollectionsCallLog.CallBack = dtCallBack;
            }
            objCollectionsCallLog.CallDate = DateTime.Now;
            objCollectionsCallLog.Internal = (objCollectionsCallLog.boolInternal ? 1 : 0);
            objCollectionsCallLog.CallTime = DateTime.Now.ToString("HH:mm:ss");
            var ret = CustomerService.SaveCollectionCallLog(objCollectionsCallLog);

            #region :: Send Email ::

            string EmailTo = string.Empty;
            var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            if (userList.Count > 0)
            {
                int UserId = Convert.ToInt32(objCollectionsCallLog.EmailNotesTo);
                EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                string subject = "Collection Call Log Details";
                string body = "";
                body += "<div style=\"width:600px;padding:15px;\">";
                body += "<p>Hi " + DisplayName + "</p>";

                body += "<p><b>Service call log details</b></p>";
                if (Convert.ToInt32(objCollectionsCallLog.CallLogInitiatedByTypeListId) == 0)
                {
                    body += "<p><b> Initiated By:</b>JK</p>";
                }
                else if (Convert.ToInt32(objCollectionsCallLog.CallLogInitiatedByTypeListId) == 1)
                {
                    body += "<p><b> Initiated By:</b>Customer</p>";
                }
                else if (Convert.ToInt32(objCollectionsCallLog.CallLogInitiatedByTypeListId) == 2)
                {
                    body += "<p><b> Initiated By:</b>Franachisee</p>";
                }
                body += "<p><b>SpokeWith:</b>" + objCollectionsCallLog.SpokeWith + "</p>";
                body += "<p><b>Action:</b>" + objCollectionsCallLog.Action + "</p>";
                body += "<p><b>Comments:</b>" + objCollectionsCallLog.Comments + "</p>";
                body += "<p>Thanks You</p>";
                body += "</div>";

                _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
            }

            #endregion
            return Json(new { aaData = "", id = objCollectionsCallLog.ClassId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion :: Collection Call Log ::

        #region :: Service Call Log ::

        [HttpGet]
        public JsonResult ServiceCallLogListResultData(int? id, string fd = "", string td = "", string st = "", int statusid = 0)
        {
            DateTime fromDate = DateTime.Parse(fd.ToString());
            DateTime toDate = DateTime.Parse(td.ToString());
            var list = CustomerService.GetServiceCallLogCustomersListForSearch(id ?? 0, fromDate, toDate, st, statusid);
            var jsonResult = Json(new { aaData = list, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult CreditTransactionPopup(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != null && id != 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(id).ToList();
                foreach (var customer in response)
                {
                    customerDetailViewModel = new CustomerDetailViewModel();
                    var cust = CustomerService.GetCustomerById(id ?? 00);
                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(customer.CustomerName.ToString()) ? String.Empty : customer.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(customer.CustomerNo.ToString()) ? String.Empty : customer.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(customer.CustomerId.ToString()) ? String.Empty : customer.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(customer.AccountType.ToString()) ? String.Empty : customer.AccountType.ToString();
                    customerDetailViewModel.ContactName = String.IsNullOrEmpty(customer.ContactName.ToString()) ? String.Empty : customer.ContactName.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(customer.Phone.ToString()) ? String.Empty : customer.Phone.ToString();
                    customerDetailViewModel.RegionId = String.IsNullOrEmpty(cust.RegionId.ToString()) ? String.Empty : cust.RegionId.ToString();
                }
            }
            ViewBag.CustomerDetail = customerDetailViewModel;
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            return PartialView("_CustomerTransactionViewModel");
        }

        [HttpGet]
        public ActionResult ManualPaymentPopup(int? id)
        {
            CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            if (id != null && id != 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(id).ToList();
                foreach (var customer in response)
                {
                    customerDetailViewModel = new CustomerDetailViewModel();
                    var cust = CustomerService.GetCustomerById(id ?? 00);
                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(customer.CustomerName.ToString()) ? String.Empty : customer.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(customer.CustomerNo.ToString()) ? String.Empty : customer.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(customer.CustomerId.ToString()) ? String.Empty : customer.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(customer.AccountType.ToString()) ? String.Empty : customer.AccountType.ToString();
                    customerDetailViewModel.ContactName = String.IsNullOrEmpty(customer.ContactName.ToString()) ? String.Empty : customer.ContactName.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(customer.Phone.ToString()) ? String.Empty : customer.Phone.ToString();
                    customerDetailViewModel.RegionId = String.IsNullOrEmpty(cust.RegionId.ToString()) ? String.Empty : cust.RegionId.ToString();
                    customerDetailViewModel.Balance = AccountReceivableService.GetCustomerCreditBalance(Convert.ToInt32(customer.CustomerId));
                }
            }
            ViewBag.CustomerDetail = customerDetailViewModel;
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            return PartialView("_ManualPaymentDetailPopup");
        }

        [HttpPost]
        public ActionResult ApplyManualPayment(FormCollection frm)
        {
            //var hdMPCallFrom = !String.IsNullOrEmpty(frm["hdMPCallFrom"]) ? frm["hdMPCallFrom"].ToString() : "";

            //var paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentTypeMP"]) ? int.Parse(frm["slPaymentTypeMP"]) : 0;
            //var referenceNo = frm["referenceNoMP"];
            //var notes = frm["txtNotes"];
            //var ClassID = !String.IsNullOrEmpty(frm["hdfCustomerIdMP"]) ? frm["hdfCustomerIdMP"].ToString() : "";

            //string Last4CC = frm["Last4CC"];
            //Last4CC = !String.IsNullOrEmpty(frm["Last4CC"]) ? Last4CC.Replace("XXXX", "") : frm["Last4CC"];
            //var paymentDate = !String.IsNullOrEmpty(frm["paymentDateMP"]) ? DateTime.Parse(frm["paymentDateMP"]) : DateTime.Now;
            //var paymentAmt = !String.IsNullOrEmpty(frm["paymentAmtMP"]) ? Decimal.Parse(frm["paymentAmtMP"]) : 0.00M;
            //var creditAmt = !String.IsNullOrEmpty(frm["customerCreditAmtMP"]) ? Decimal.Parse(frm["customerCreditAmtMP"]) : 0.00M;
            //var balance = !String.IsNullOrEmpty(frm["balanceMP"]) ? Decimal.Parse(frm["balanceMP"]) : 0.00M;

            //if (balance > 0 && creditAmt > 0) // amount leftover and credit used
            //{
            //    if (balance >= creditAmt) // since balance > 0, not all customer credit is necessary
            //    {
            //        balance -= creditAmt;
            //        creditAmt = 0;
            //    }
            //    else // since creditAmt > balance, there is not really a balance
            //    {
            //        creditAmt -= balance;
            //        balance = 0;
            //    }
            //}

            //var afterSave = frm["SaveMethod"];
            //int RegionID = 0;
            //int PGID = 0;
            //string userRegion = _claimView.GetCLAIM_SELECTED_COMPANY_ID().ToString();
            //if (userRegion != null)
            //{
            //    RegionID = Convert.ToInt32(userRegion);
            //}
            //jkDatabaseEntities jk = new jkDatabaseEntities();
            //var PG = jk.PaymentGatewayDetails.Where(r => r.RegionID == RegionID && r.IsActive == true).FirstOrDefault();
            //if (PG != null)
            //{
            //    PGID = PG.Id;
            //}
            //ManualPaymentTransactionViewModel vm = new ManualPaymentTransactionViewModel();
            //vm.PaymentAmount = paymentAmt;
            //vm.CreditAmount = creditAmt;
            //vm.Balance = balance;
            //vm.PaymentMethodListId = paymentMethodListId;
            //vm.ReferenceNo = referenceNo;
            //vm.Notes = notes;
            //vm.CustomerId = ClassID != "" ? int.Parse(ClassID) : -1; // to be filled on first invoice
            //vm.RegionId = -1; // to be filled on first invoice
            //vm.CreatedBy = this.LoginUserId;
            //vm.CreatedDate = paymentDate;
            //vm.TransactionDate = paymentDate;

            //List<ManualPaymentInvoiceViewModel> invoices = new List<ManualPaymentInvoiceViewModel>();
            //List<CCTransaction> cc = new List<CCTransaction>();

            //foreach (string chkKey in frm.AllKeys.Where(k => k.EndsWith("_chk")))
            //{
            //    string invChunk = chkKey.Split('_')[0]; // "inv#####"
            //    string invStr = invChunk.Substring(3); // "#####"

            //    int invId = 0;
            //    if (!Int32.TryParse(invStr, out invId)) // failed to parse invoice id
            //        continue;

            //    var invPayment = frm[invChunk + "_totalPayment"] != null ? Decimal.Parse(frm[invChunk + "_totalPayment"]) : 0.00M;
            //    var invBalance = frm[invChunk + "_balance"] != null ? Decimal.Parse(frm[invChunk + "_balance"]) : 0.00M;

            //    var invoiceDetails = AccountReceivableService.GetCreditDetailForInvoice(invId);

            //    if (vm.RegionId == -1 && invoiceDetails.Invoice.InvoiceDetail.RegionId != null)
            //        vm.RegionId = (int)invoiceDetails.Invoice.InvoiceDetail.RegionId;
            //    if (vm.CustomerId == -1 && invoiceDetails.Invoice.InvoiceDetail.CustomerId != null)
            //        vm.CustomerId = (int)invoiceDetails.Invoice.InvoiceDetail.CustomerId;

            //    ManualPaymentInvoiceViewModel mpivm = new ManualPaymentInvoiceViewModel();
            //    mpivm.InvoiceId = invoiceDetails.Invoice.InvoiceDetail.InvoiceId;
            //    mpivm.PaidInFull = invoiceDetails.InvoiceBalance == invPayment && invBalance == 0;

            //    ManualPaymentCustomerViewModel mpcvm = new ManualPaymentCustomerViewModel();
            //    mpcvm.CustomerId = (int)invoiceDetails.Invoice.InvoiceDetail.CustomerId;
            //    mpcvm.Payments = new List<ManualPaymentViewModel>();

            //    for (int i = 0; i < invoiceDetails.Invoice.InvoiceDetailItems.Count(); i++)
            //    {
            //        var item = invoiceDetails.Invoice.InvoiceDetailItems[i];

            //        bool foundFields = true;
            //        decimal itemPaymentAmt = 0;
            //        decimal itemTotal = 0;

            //        // sanity check to see if payment details were set, whole invoice was paid, or there is only one line item
            //        if (mpivm.PaidInFull || invoiceDetails.Invoice.InvoiceDetailItems.Count == 1)
            //        {
            //            var taxRate = item.TAXAmount / item.Total;
            //            itemPaymentAmt = invPayment;
            //            var taxAmount = (decimal)(invPayment * taxRate);
            //            itemTotal = invPayment - taxAmount;
            //        }
            //        else
            //        {
            //            foundFields = foundFields && decimal.TryParse(frm[string.Format(invChunk + "_item{0}_paymentAmt", i)], out itemPaymentAmt);
            //            foundFields = foundFields && decimal.TryParse(frm[string.Format(invChunk + "_item{0}_total", i)], out itemTotal);
            //        }

            //        if (foundFields)
            //        {
            //            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
            //            mpvm.MasterTrxDetailId = item.MasterTrxDetailId;
            //            mpvm.LineNo = (int)item.LineNumber;
            //            mpvm.PaymentAmount = itemPaymentAmt;
            //            mpvm.Tax = itemPaymentAmt - itemTotal;
            //            mpvm.Total = itemTotal;
            //            mpcvm.Payments.Add(mpvm);
            //        }
            //    }

            //    mpivm.CustomerPayment = mpcvm;

            //    List<ManualPaymentFranchiseeViewModel> mpfvms = new List<ManualPaymentFranchiseeViewModel>();


            //    foreach (var item in invoiceDetails.FranchiseeItems)
            //    {
            //        var r = item.InvoiceFranchiseeDetailItem;

            //        int bpId = r.BillingPayId;

            //        bool foundFields = true;
            //        decimal bpPaymentAmt = 0;

            //        // sanity check to see if payment details were set or if whole invoice was paid
            //        if (mpivm.PaidInFull)
            //        {
            //            bpPaymentAmt = (decimal)r.Balance; // pay whole balance because whole invoice was paid
            //        }
            //        else if (invoiceDetails.Invoice.InvoiceDetailItems.Count == 1)
            //        {
            //            // only one line item, so distribute paid amount (after taxes) to all franchisees proportionally
            //            var totalFranchiseeBalance = invoiceDetails.FranchiseeItems.Sum(o => o.InvoiceFranchiseeDetailItem.Balance);
            //            var percentage = r.Balance / totalFranchiseeBalance;
            //            bpPaymentAmt = mpivm.CustomerPayment.Payments[0].Total * (decimal)percentage;
            //        }
            //        else
            //        {
            //            foundFields = foundFields && decimal.TryParse(frm[string.Format(invChunk + "_bp{0}_paymentAmt", bpId)], out bpPaymentAmt);
            //        }

            //        if (foundFields)
            //        {
            //            ManualPaymentFranchiseeViewModel mpfvm = new ManualPaymentFranchiseeViewModel();
            //            mpfvm.FranchiseeId = r.FranchiseeId;
            //            mpfvm.BillingPayId = r.BillingPayId;

            //            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
            //            mpvm.MasterTrxDetailId = r.MasterTrxDetailId;
            //            mpvm.LineNo = (int)r.LineNo;
            //            mpvm.PaymentAmount = bpPaymentAmt;
            //            mpfvm.Payment = mpvm;

            //            mpfvms.Add(mpfvm);
            //        }
            //    }

            //    mpivm.FranchiseePayments = mpfvms;

            //    invoices.Add(mpivm);

            //    if (paymentMethodListId == 2) // credit card
            //    {
            //        CCTransaction cCTransaction = new CCTransaction();
            //        cCTransaction.Amount = invPayment;
            //        cCTransaction.BatchID = notes.ToString();
            //        cCTransaction.ClassID = Convert.ToInt32(ClassID);
            //        cCTransaction.CreateByID = _claimView.GetCLAIM_PERSON_INFORMATION().UserId;
            //        cCTransaction.invoiceID = invoiceDetails.Invoice.InvoiceDetail.InvoiceId;
            //        cCTransaction.TransactionDate = DateTime.Now;
            //        cCTransaction.TransactionID = notes.ToString();
            //        cCTransaction.TypeID = 1;
            //        cCTransaction.Last4CCNo = Convert.ToInt32(Last4CC);
            //        cCTransaction.GatewayID = PGID;

            //        cc.Add(cCTransaction);
            //    }
            //}

            //if (paymentMethodListId == 2) // credit card 
            //{
            //    GeneralService generalService = new GeneralService();
            //    generalService.InsertCCtransaction(cc);
            //}

            //vm.Invoices = invoices;

            //if (vm.Invoices.Count > 0) // sanity check
            //    AccountReceivableService.InsertManualPaymentTransaction(vm);


            //if (hdMPCallFrom == "invoice")
            //{
            //    return RedirectToAction("InvoiceList", "AccountReceivable", new { area = "Portal", id = ClassID });
            //}
            //if (afterSave == "SaveClose")
            //{
            //    return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = ClassID });
            //}

            var hdMPCallFrom = !String.IsNullOrEmpty(frm["hdMPCallFrom"]) ? frm["hdMPCallFrom"].ToString() : "";

            //var paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentTypeMP"]) ? int.Parse(frm["slPaymentTypeMP"]) : 0;
            //var referenceNo = frm["referenceNoMP"];
            //var notes = frm["txtNotes"];
            //var ClassID = !String.IsNullOrEmpty(frm["hdfCustomerIdMP"]) ? frm["hdfCustomerIdMP"].ToString() : "";

            //string Last4CC = frm["Last4CC"];
            //Last4CC = !String.IsNullOrEmpty(frm["Last4CC"]) ? Last4CC.Replace("XXXX", "") : frm["Last4CC"];
            //var paymentDate = !String.IsNullOrEmpty(frm["paymentDateMP"]) ? DateTime.Parse(frm["paymentDateMP"]) : DateTime.Now;
            //var paymentAmt = !String.IsNullOrEmpty(frm["paymentAmtMP"]) ? Decimal.Parse(frm["paymentAmtMP"]) : 0.00M;
            //var creditAmt = !String.IsNullOrEmpty(frm["customerCreditAmtMP"]) ? Decimal.Parse(frm["customerCreditAmtMP"]) : 0.00M;
            //var balance = !String.IsNullOrEmpty(frm["balanceMP"]) ? Decimal.Parse(frm["balanceMP"]) : 0.00M;

            int paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentTypeMP"].ToString().Trim()) ? int.Parse(frm["slPaymentTypeMP"].ToString().Trim()) : 0;
            string referenceNo = !String.IsNullOrEmpty(frm["referenceNoMP"].ToString()) ? frm["referenceNoMP"].ToString() : "";
            string notes = !String.IsNullOrEmpty(frm["txtNotes"].ToString()) ? frm["txtNotes"].ToString() : "";
            int ClassId = !String.IsNullOrEmpty(frm["hdfCustomerIdMP"].ToString()) ? int.Parse(frm["hdfCustomerIdMP"].ToString()) : -1;
            string Last4CC = !String.IsNullOrEmpty(frm["Last4CC"].ToString()) ? frm["Last4CC"].ToString().Replace("XXXX", "") : "";
            DateTime paymentDate = !String.IsNullOrEmpty(frm["paymentDateMP"].ToString()) ? DateTime.Parse(frm["paymentDateMP"]) : DateTime.Now;
            decimal paymentAmt = !string.IsNullOrEmpty(frm["paymentAmtMP"]) ? Decimal.Parse(frm["paymentAmtMP"]) : 0.00M;
            decimal creditAmt = !string.IsNullOrEmpty(frm["customerCreditAmtMP"]) ? Decimal.Parse(frm["customerCreditAmtMP"]) : 0.00M;
            decimal balance = !string.IsNullOrEmpty(frm["balanceMP"]) ? Decimal.Parse(frm["balanceMP"]) : 0.00M;
            //bool mp_chkApplyCredit = !String.IsNullOrEmpty(frm["mp_chkApplyCredit"]) ? bool.Parse(frm["mp_chkApplyCredit"]) : false;
            var afterSave = frm["SaveMethod"];
            int _RegionId = SelectedRegionId;

            if (paymentAmt > 0 || creditAmt > 0)
            {
                FullManualPaymentViewModel oMainObject = new FullManualPaymentViewModel();
                oMainObject.PaymentMethodListId = paymentMethodListId;
                oMainObject.ReferenceNo = referenceNo;
                oMainObject.Notes = notes;
                oMainObject.CustomerId = ClassId;
                oMainObject.PaymentAmount = paymentAmt;
                oMainObject.CreditAmount = creditAmt;
                oMainObject.Balance = balance;
                oMainObject.TransactionDate = paymentDate;

                //oMainObject.TransactionNumber;
                //oMainObject.RegionId;

                oMainObject.CreatedBy = LoginUserId;
                oMainObject.CreatedDate = DateTime.Now;


                List<MPInvoiceViewModel> lstManualInvoices = new List<MPInvoiceViewModel>();
                List<CCTransaction> cc = new List<CCTransaction>();
                MPInvoiceViewModel oManualInvoice = new MPInvoiceViewModel();

                foreach (string chkKey in frm.AllKeys.Where(k => k.EndsWith("_chk")))
                {



                    string invChunk = chkKey.Split('_')[0]; // "inv#####"
                    string invStr = invChunk.Substring(3); // "#####"

                    int invId = 0;
                    if (!Int32.TryParse(invStr, out invId)) // failed to parse invoice id
                        continue;
                    var invPayment = !String.IsNullOrEmpty(frm[invChunk + "_totalPayment"])
                        ? Decimal.Parse(frm[invChunk + "_totalPayment"])
                        : 0.00M;
                    var invBalance = !String.IsNullOrEmpty(frm[invChunk + "_balance"])
                        ? Decimal.Parse(frm[invChunk + "_balance"])
                        : 0.00M;

                    oManualInvoice = new MPInvoiceViewModel();
                    oManualInvoice.InvoiceId = invId;
                    oManualInvoice.InvoicePayment = invPayment;
                    decimal applyAmountforPartialPay = invPayment;
                    if (invBalance < 0)
                    {
                        oManualInvoice.InvoiceBalance = 0;
                        oManualInvoice.OverflowAmount = Math.Abs(invBalance);
                        oManualInvoice.InvoicePayment = invPayment - Math.Abs(invBalance);
                        applyAmountforPartialPay = invPayment - Math.Abs(invBalance);
                    }
                    else
                    {
                        oManualInvoice.InvoiceBalance = invBalance;
                        oManualInvoice.OverflowAmount = 0;
                    }
                    oManualInvoice.PaidInFull = oManualInvoice.InvoiceBalance == 0 ? true : false;



                    //InvoiceDetail 
                    CreditDetailViewModel invoiceDetail = AccountReceivableService.GetCreditDetailForInvoice(invId);

                    _RegionId = (int)invoiceDetail.Invoice.InvoiceDetail.RegionId;

                    if ((oMainObject.RegionId == -1 || oMainObject.RegionId == 0) && invoiceDetail.Invoice.InvoiceDetail.RegionId != null)
                        oMainObject.RegionId = (int)invoiceDetail.Invoice.InvoiceDetail.RegionId;
                    if (oMainObject.CustomerId == -1 && invoiceDetail.Invoice.InvoiceDetail.CustomerId != null)
                        oMainObject.CustomerId = (int)invoiceDetail.Invoice.InvoiceDetail.CustomerId;
                    if (ClassId == -1 || ClassId == 0)
                        ClassId = (int)invoiceDetail.Invoice.InvoiceDetail.CustomerId;



                    ManualPaymentCustomerViewModel mpcvm = new ManualPaymentCustomerViewModel();
                    mpcvm.CustomerId = (int)invoiceDetail.Invoice.InvoiceDetail.CustomerId;
                    mpcvm.Payments = new List<ManualPaymentViewModel>();


                    for (int i = 0; i < invoiceDetail.Invoice.InvoiceDetailItems.Count(); i++)
                    {
                        var item = invoiceDetail.Invoice.InvoiceDetailItems[i];

                        bool foundFields = true;
                        decimal itemPaymentAmt = 0;
                        decimal itemTotal = 0;

                        // sanity check to see if payment details were set, whole invoice was paid, or there is only one line item
                        if (oManualInvoice.PaidInFull || invoiceDetail.Invoice.InvoiceDetailItems.Count == 1)
                        {
                            var taxRate = item.TAXAmount / item.Total;
                            itemPaymentAmt = applyAmountforPartialPay;
                            var taxAmount = (decimal)(applyAmountforPartialPay * taxRate);
                            itemTotal = applyAmountforPartialPay - taxAmount;
                        }
                        else if (!oManualInvoice.PaidInFull && invoiceDetail.FranchiseeItems.Count() == 1)
                        {
                            if (item.Balance > 0)
                            {
                                if (item.Balance < applyAmountforPartialPay)
                                {
                                    invPayment = (decimal)item.Balance;
                                    applyAmountforPartialPay = applyAmountforPartialPay - (decimal)item.Balance;
                                }
                                else
                                {
                                    invPayment = applyAmountforPartialPay;
                                    applyAmountforPartialPay = 0;
                                }
                                var taxRate = item.TAXAmount / item.Total;
                                itemPaymentAmt = invPayment;
                                var taxAmount = (decimal)(invPayment * taxRate);
                                itemTotal = invPayment - taxAmount;
                            }

                        }
                        else
                        {
                            foundFields = foundFields &&
                                          decimal.TryParse(frm[string.Format(invChunk + "_item{0}_paymentAmt", i)],
                                              out itemPaymentAmt);
                            foundFields = foundFields &&
                                          decimal.TryParse(frm[string.Format(invChunk + "_item{0}_total", i)],
                                              out itemTotal);
                        }

                        if (foundFields)
                        //if (foundFields && itemTotal > 0)
                        {
                            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
                            mpvm.MasterTrxDetailId = item.MasterTrxDetailId;
                            mpvm.LineNo = (int)item.LineNumber;
                            mpvm.PaymentAmount = itemPaymentAmt;
                            mpvm.Tax = itemPaymentAmt - itemTotal;
                            mpvm.Total = itemTotal;
                            mpvm.ExtendedPrice = itemPaymentAmt - (itemPaymentAmt - itemTotal);
                            mpcvm.Payments.Add(mpvm);
                        }
                    }

                    oManualInvoice.CustomerPayment = mpcvm;

                    List<ManualPaymentFranchiseeViewModel> mpfvms = new List<ManualPaymentFranchiseeViewModel>();

                    foreach (var item in invoiceDetail.FranchiseeItems)
                    {
                        var r = item.InvoiceFranchiseeDetailItem;

                        int bpId = r.BillingPayId;

                        bool foundFields = true;
                        decimal bpPaymentAmt = 0;

                        // sanity check to see if payment details were set or if whole invoice was paid
                        if (oManualInvoice.PaidInFull)
                        {
                            bpPaymentAmt = (decimal)r.Balance; // pay whole balance because whole invoice was paid
                        }
                        else if (invoiceDetail.Invoice.InvoiceDetailItems.Count == 1 || invoiceDetail.FranchiseeItems.Count == 1)
                        {
                            // only one line item, so distribute paid amount (after taxes) to all franchisees proportionally
                            var totalFranchiseeBalance =
                                invoiceDetail.FranchiseeItems.Sum(o => o.InvoiceFranchiseeDetailItem.Balance);
                            var percentage = r.Balance / totalFranchiseeBalance;
                            bpPaymentAmt = oManualInvoice.CustomerPayment.Payments[0].Total * (decimal)percentage;
                        }
                        else
                        {
                            foundFields = foundFields &&
                                          decimal.TryParse(frm[string.Format(invChunk + "_bp{0}_paymentAmt", bpId)],
                                              out bpPaymentAmt);
                        }

                        if (foundFields)
                        {
                            ManualPaymentFranchiseeViewModel mpfvm = new ManualPaymentFranchiseeViewModel();
                            mpfvm.FranchiseeId = r.FranchiseeId;
                            mpfvm.BillingPayId = r.BillingPayId;

                            using (jkDatabaseEntities context = new jkDatabaseEntities())
                            {
                                var billingPay = context.BillingPays.Where(o => o.BillingPayId == r.BillingPayId).FirstOrDefault();
                                mpfvm.IsTurnAroundPayment = billingPay.HasBeenChargedBack ? true : false;
                            }

                            mpfvm.IsTARPaid = false;

                            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
                            mpvm.MasterTrxDetailId = r.MasterTrxDetailId;
                            mpvm.LineNo = (int)r.LineNo;
                            mpvm.PaymentAmount = bpPaymentAmt;
                            mpfvm.Payment = mpvm;

                            mpfvms.Add(mpfvm);
                        }
                    }

                    oManualInvoice.FranchiseePayments = mpfvms;



                    //oManualInvoice.IsManualInvoice = invoiceDetail.Invoice.InvoiceDetailItems.
                    lstManualInvoices.Add(oManualInvoice);

                    if (paymentMethodListId == 2) // credit card
                    {
                        CCTransaction cCTransaction = new CCTransaction();
                        cCTransaction.Amount = invPayment;
                        cCTransaction.BatchID = notes.ToString();
                        cCTransaction.ClassID = Convert.ToInt32(ClassId);
                        cCTransaction.CreateByID = _claimView.GetCLAIM_PERSON_INFORMATION().UserId;
                        cCTransaction.invoiceID = invoiceDetail.Invoice.InvoiceDetail.InvoiceId;
                        cCTransaction.TransactionDate = DateTime.Now;
                        cCTransaction.TransactionID = notes.ToString();
                        cCTransaction.TypeID = 1;
                        cCTransaction.Last4CCNo = Convert.ToInt32(Last4CC);
                        //cCTransaction.GatewayID = PGID;

                        cc.Add(cCTransaction);
                    }
                }

                if (paymentMethodListId == 2) // credit card 
                {
                    GeneralService generalService = new GeneralService();
                    generalService.InsertCCtransaction(cc);
                }

                oMainObject.Invoices = lstManualInvoices;
                oMainObject.RegionId = _RegionId;
                if (oMainObject.Invoices.Count > 0) // sanity check
                    AccountReceivableService.InsertManualPaymentTransactionUpdated(oMainObject);

            }

            if (hdMPCallFrom == "invoice")
            {
                return RedirectToAction("InvoiceList", "AccountReceivable", new { area = "Portal", id = ClassId });
            }
            if (afterSave == "SaveClose")
            {
                return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = ClassId });
            }

            return View();
        }

        [HttpGet]
        public ActionResult CustomerServiceCallLogPopup(int? id)
        {

            ServiceCallLogModel FullCustomerViewModel = new ServiceCallLogModel();
            ViewBag.CustomerId = id;
            ViewBag.RegionId = SelectedRegionId;
            //ViewBag.CallFrom = callfrom;

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            if (id != 0)
            {
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            }
            ViewBag.CustomerDetail = CustomerViewModel;


            var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            var ServiceCallFollowBy = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Call Follow By").FirstOrDefault();
            if (userLista.Count > 0 && ServiceCallFollowBy != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceCallFollowBy.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();

                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name"), "Select User");
            }
            else
            {
                ViewBag.ServiceCallFollowBy = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            }

            var ServiceEmailCall = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId && x.Name == "Service Email Call").FirstOrDefault();
            if (userLista.Count > 0 && ServiceEmailCall != null)
            {
                var SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Where(x => x.CRM_TerritoryId == ServiceEmailCall.CRM_TerritoryId).ToList();
                var SalesTerritoryAssignmentListViewBeg = (from a in userLista
                                                           join b in SalesTerritoryAssignmentList on a.UserId equals b.UserId
                                                           select new
                                                           {
                                                               UserID = a.UserId,
                                                               Name = a.FirstName + " " + a.LastName
                                                           }).ToList();
                ViewBag.ServiceEmailCall = new SelectList(SalesTerritoryAssignmentListViewBeg, "UserID", "Name");
            }
            else
            {
                ViewBag.ServiceEmailCall = new SelectList(userLista, "UserID", "Name");
            }

            ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            var lstStatusResultList = CustomerService.GetStatusResultList();
            ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");


            //CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
            //if (id != null && id != 0)
            //{
            //    var response = jkEntityModel.portal_spGet_CustomerDetail(id);

            //    foreach (var a in response.ToList())
            //    {
            //        customerDetailViewModel = new CustomerDetailViewModel();
            //        var customer = CustomerService.GetCustomerById(id ?? 0);
            //        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
            //        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
            //        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
            //        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
            //        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
            //        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
            //        customerDetailViewModel.RegionId = String.IsNullOrEmpty(customer.RegionId.ToString()) ? String.Empty : customer.RegionId.ToString();
            //    }
            //}
            //ViewBag.CustomerID = id;
            //ViewBag.CustomerDetail = customerDetailViewModel;
            //TempData["CustomerInformation"] = customerDetailViewModel;

            //ViewBag.InitiatedBy = GetEnumValuesAndDescriptions<JKApi.Business.Enumeration.InitiatedByList>();

            //var ServiceCallAreaList = CustomerService.GetServiceCallLogAreaList();
            //ViewBag.ServiceCallAreaList = AddFirstItemInSelecetList(new SelectList(ServiceCallAreaList, "ServiceCallLogAreaListId", "Name"), "All Area");

            //var ServiceCallLogTypeList = CustomerService.GetServiceCallLogTypeList();
            //ViewBag.ServiceCallLogTypeList = AddFirstItemInSelecetList(new SelectList(ServiceCallLogTypeList, "ServiceCallLogTypeListId", "Name"), "All Type");

            //var userLista = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);
            //if (userLista.Count > 0)
            //{
            //    userLista.ForEach(x => x.Name = x.FirstName + " " + x.LastName);
            //    ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            //}
            //else
            //{
            //    ViewBag.UserList = AddFirstItemInSelecetList(new SelectList(userLista, "UserID", "Name"), "Select User");
            //}

            //var lstStatusResultList = CustomerService.GetStatusResultList();
            //ViewBag.StatusResultList = AddFirstItemInSelecetList(new SelectList(lstStatusResultList, "StatusResultListId", "Name"), "All Status");
            if (id > 0)
            {
                var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
                if (CustomerDistributions.Count > 0)
                {
                    ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                }
            }
            return PartialView("_CustomerServiceCallLogPopup", new JKViewModels.Customer.ServiceCallLogModel());
        }

        public ActionResult SaveServiceCallLogDetails(int CustomerId, int RegionId, string InitiatedById, string AreaId, string TypeId, string SpokeWith, string Action, string Comments, string StatusId, string Callback, string FollowBy, string EmailCallNotes)
        {
            ServiceCallLog ServiceCallLog = new ServiceCallLog();
            ServiceCallLog.TypeListId = 1;
            ServiceCallLog.RegionId = RegionId;
            ServiceCallLog.ClassId = CustomerId;
            ServiceCallLog.InitiatedById = Convert.ToInt32(InitiatedById);
            ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(AreaId);
            ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(TypeId);
            ServiceCallLog.SpokeWith = SpokeWith;
            ServiceCallLog.Action = Action;
            ServiceCallLog.Comments = Comments;
            ServiceCallLog.StatusResultListId = Convert.ToInt32(StatusId);

            if (Callback != "" && Callback != null)
            {
                //DateTime dtCallBack = DateTime.ParseExact(Callback, "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                DateTime dtCallBack = Convert.ToDateTime(Callback);
                ServiceCallLog.CallBack = dtCallBack;
            }
            ServiceCallLog.CallDate = DateTime.Now; //.ToString("dd-MM-yyyy")
            ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;

            if (FollowBy != null && FollowBy != "")
            {
                ServiceCallLog.FollowUpBy = Convert.ToInt32(FollowBy);
            }
            if (EmailCallNotes != null && EmailCallNotes != "")
            {
                ServiceCallLog.EmailNotesTo = EmailCallNotes;
            }

            CustomerService.SaveServiceCallLog(ServiceCallLog);

            #region :: Send Email ::

            string EmailTo = string.Empty;
            var userList = CustomerService.GetUserOfDefaultRegion(SelectedRegionId, true);

            if (userList.Count > 0)
            {
                int UserId = Convert.ToInt32(EmailCallNotes);
                EmailTo = userList.Where(w => w.UserId == UserId).FirstOrDefault().Email;
                string DisplayName = userList.Where(w => w.UserId == UserId).FirstOrDefault().Name;
                string subject = "Service Call Log Details";
                string body = "";
                body += "<div style=\"width:600px;padding:15px;\">";
                body += "<p>Hi " + DisplayName + "</p>";

                body += "<p><b>Service call log details</b></p>";
                if (Convert.ToInt32(InitiatedById) == 0)
                {
                    body += "<p><b> Initiated By:</b>JK</p>";
                }
                else if (Convert.ToInt32(InitiatedById) == 0)
                {
                    body += "<p><b> Initiated By:</b>Customer</p>";
                }
                else if (Convert.ToInt32(InitiatedById) == 0)
                {
                    body += "<p><b> Initiated By:</b>Franachisee</p>";
                }
                body += "<p><b>SpokeWith:</b>" + SpokeWith + "</p>";
                body += "<p><b>Action:</b>" + Action + "</p>";
                body += "<p><b>Comments:</b>" + Comments + "</p>";
                body += "<p>Thanks You</p>";
                body += "</div>";

                _mailService.SendEmailAsync(EmailTo, body, subject, emailToDisplayName: DisplayName);
            }

            #endregion

            return Json(new { aaData = "", id = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion :: Service Call Log ::

        #region :: Pending Approval List ::

        [HttpGet]
        public JsonResult PendingDashboarddata()
        {
            List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
            MessageData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID())).ToList<PendingDashboardDataModel>();
            return Json(MessageData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PendingDashboardTaskData()
        {
            List<PendingDashboardTasksDataModel> TasksData = new List<PendingDashboardTasksDataModel>();
            TasksData = _commonService.GetDashboardPendingTasksData(int.Parse(_claimView.GetCLAIM_USERID())).ToList<PendingDashboardTasksDataModel>();
            return Json(TasksData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UpdateViewedMessage(int id)
        {
            _commonService.ChildViewMessageDashboard(id);
            return Json("updated", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PendingApprovalListCustomerDetailPopup(int id = -1)
        {
            int? CustomerID = id;
            ViewBag.CustomerID = id;
            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1(); ;
            if (id > 0)
            {


                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);
                List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
                MessageData = _commonService.GetDashboardPendingData(null).Where(r => r.CustomerID == id).OrderBy(r => r.MessageDate).ToList<PendingDashboardDataModel>();
                //MaintenanceTemp oMaintenanceTemp = jkEntityModel.MaintenanceTemps.Where(o => o.ClassId == CustomerID && o.TypeListId == 1 && o.MaintenanceTypeListId == 7).ToList().FirstOrDefault();

                Distribution oDistributionTemp = jkEntityModel.Distributions.Where(o => o.CustomerId == CustomerID && o.isActive == true).ToList().FirstOrDefault();


                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.ContractTypeList = String.IsNullOrEmpty(a.ContractType.ToString()) ? String.Empty : a.ContractType.ToString();
                    customerDetailViewModel.ContractTypeListId = String.IsNullOrEmpty(a.ContractTypeListId.ToString()) ? String.Empty : a.ContractTypeListId.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(Convert.ToString(a.Address2)) ? String.Empty : a.Address2.ToString();
                    customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone) ? String.Empty : a.Phone.ToString();

                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax) ? String.Empty : a.Fax.ToString();

                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());
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
                if (oDistributionTemp != null)
                {
                    //FullCustomerViewModel.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                    FullCustomerViewModel.MaintenanceTempId = oDistributionTemp.DistributionId;
                }
                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                FullCustomerViewModel.MessagesData = MessageData;
                FullCustomerViewModel.USERID = int.Parse(_claimView.GetCLAIM_USERID());

                var ValidationItem = CustomerService.ValidationItemListStatus((int)JKApi.Business.Enumeration.CustomerStatusList.Pending, (int)JKApi.Business.Enumeration.TypeList.Customer);
                ViewBag.ValidationItem = ValidationItem;


                #region For Region Accounting
                var ValidationItemOpration = CustomerService.ValidationItemListStatus(38, (int)JKApi.Business.Enumeration.TypeList.Customer);
                ViewBag.ValidationItemOpration = ValidationItemOpration;
                var ValidationItemResultOpration = CustomerService.GetValidationByClassId((int)CustomerID, 39);
                var statusNoteOpration = CustomerService.GetStatusByClassId((int)CustomerID, 39);
                ViewBag.ValidationItemResultOpration = ValidationItemResultOpration;
                ViewBag.statusNoteOpration = statusNoteOpration?.StatusNotes;

                var ValidationItemAccounting = CustomerService.ValidationItemListStatus(39, (int)JKApi.Business.Enumeration.TypeList.Customer);
                ViewBag.ValidationItemAccounting = ValidationItemAccounting;
                var ValidationItemResultAccounting = CustomerService.GetValidationByClassId((int)CustomerID, 4);
                var statusNoteAccounting = CustomerService.GetStatusByClassId((int)CustomerID, 4);
                ViewBag.ValidationItemResultAccounting = ValidationItemResultAccounting;
                ViewBag.statusNoteAccounting = statusNoteAccounting?.StatusNotes;
                #endregion

                List<portal_spGet_C_DistributionWithNoFinderFee_Result> custDistWithNoFF = new List<portal_spGet_C_DistributionWithNoFinderFee_Result>();
                custDistWithNoFF = FranchiseeService.GetCustomerDistributionWithNoFinderFee(id);

                CustomerDistributionDetailsModel custDistribution = new CustomerDistributionDetailsModel();
                custDistribution = CustomerService.GetCustomerDistributionDetails(id);

                ViewBag.DistributionDetailLineNeedsFF = (custDistWithNoFF.Count > 0 ? custDistWithNoFF[0].DetailLineNumber : 0);

                int CustomerContractId = 0;
                var CustomerContract = CustomerService.GetContractByCustomerId(id);
                if (CustomerContract != null)
                {
                    CustomerContractId = CustomerContract.ContractId;
                }
                ViewBag.CustomerContract = CustomerContractId;
                ViewBag.TotalDistribution = custDistribution.TotalDistribution;

                var CustomerDistributions = CustomerService.GetCustomerDistributionList(id);
                ViewBag.Distributions = CustomerDistributions;
                //var CustomerDistribution = CustomerService.GetCustomerDistribution(id);
                if (CustomerDistributions.Count > 0)
                {
                    ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                    ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
                }
                else
                {
                    ViewBag.DistributionFranchiseeId = 0;
                    ViewBag.DistributionId = 0;
                }


                var FindersFeeCustomer = CustomerService.GetFindersFeewithCustomerId(id, (CustomerDistributions.Count > 0 ? CustomerDistributions[0].DistributionId : 0));
                if (FindersFeeCustomer != null)
                {
                    ViewBag.FindersFeeCustomer = FindersFeeCustomer.FindersFeeId;
                }
                else
                {
                    ViewBag.FindersFeeCustomer = 0;
                }


            }
            return PartialView("_PendingApprovalListCustomerDetailPopup", FullCustomerViewModel);
        }

        [HttpGet]
        public ActionResult PendingApprovalListEditCustomerPopup(int id = -1)
        {
            if (id < 0)
            {
                dynamic GetUsStates = getUsStatesList();
                ViewBag.MainStatesList = GetUsStates;
                ViewBag.BillingStatesList = GetUsStates;
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }
            //dynamic GetUsStates = getUsStatesList();
            //ViewBag.MainStatesList = GetUsStates;
            //ViewBag.BillingStatesList = GetUsStates;
            //ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonList", "Name");
            //ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
            //ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");

            int? CustomerID = id;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            //var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().ToList();
            //if (TransactionsTypeList != null)
            //{
            //    ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            //    //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            //}

            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1(); ;
            if (id > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();

                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
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
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());

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

                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopup(id);

                //foreach (EmailViewModel op in FullCustomerViewModel.lstEBillEmails)
                //{
                //    FullCustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                //}
                //if (FullCustomerViewModel.EBill_Emails.Length > 0)
                //    FullCustomerViewModel.EBill_Emails = FullCustomerViewModel.EBill_Emails.Trim(',');

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }
            else
            {
                //FullCustomerViewModel = new FullCustomerViewModel1();
                //FullCustomerViewModel.CustomerDetail = new CustomerDetailViewModel();
            }
            //ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            return PartialView("_PendingApprovalListEditCustomerPopup", FullCustomerViewModel);
        }

        public ActionResult CustomerUploadDocumentPopup(int id = -1, bool isCRM = false)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            ViewBag.Id = id;
            ViewBag.CFileTypeList = new SelectList(CustomerService.GetFileTypeList(1), "FileTypeListId", "Name");

            string Address1 = string.Empty;
            string Address2 = string.Empty;
            FullCustomerViewModel CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.CustomerNo = CustomerViewModel.CustomerViewModel.CustomerNo;
                ViewBag.CustomerName = CustomerViewModel.CustomerViewModel.Name;
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;
            }
            ViewBag.isCRM = isCRM;
            string NewAccountFormpath = string.Empty;
            string AccountAcceptanceFormpath = string.Empty;
            if (isCRM == true)
            {
                if (Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), id.ToString())))
                {
                    var retPath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + id.ToString() + "/" + id + "_NewAccountForm.pdf";
                    string strformpath = id + "_NewAccountForm.pdf";
                    if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), id.ToString()) + "/" + id + "_NewAccountForm.pdf"))
                    {
                        NewAccountFormpath = "/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + id.ToString() + "/" + id + "_NewAccountForm.pdf";
                    }
                }
                if (Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), id.ToString())))
                {
                    var retPath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + id.ToString() + "/" + id + "_AccountAcceptanceForm.pdf";
                    string strformpath = id + "_AccountAcceptanceForm.pdf";
                    if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), id.ToString()) + "/" + id + "_AccountAcceptanceForm.pdf"))
                    {
                        AccountAcceptanceFormpath = "/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + id.ToString() + "/" + id + "_AccountAcceptanceForm.pdf";
                    }
                }
            }
            ViewBag.NewAccountForm = NewAccountFormpath;
            ViewBag.AccountAcceptanceForm = AccountAcceptanceFormpath;
            return PartialView("_CustomerUploadDocumentPopup", CustomerService.GetCUploadDocument(id, 1));
        }

        public JsonResult IsDocViewToFranchiseeUploadDocument(int id)
        {
            var newDoc = jkEntityModel.UploadDocuments.Where(x => x.UploadDocumentId == id).FirstOrDefault();
            if (newDoc != null)
            {
                newDoc.IsViewToFranchisee = newDoc.IsViewToFranchisee == true ? false : true;
                jkEntityModel.UploadDocuments.Add(newDoc);
                jkEntityModel.SaveChanges();
                return Json(new { success = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = "This is not valid doc.", docType = newDoc.ToJSON() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateApproveReject(int CustomerId, string Note, int Status, string valIds = "")
        {
            if (CustomerId > 0)
            {
                CustomerService.UpdateApproveReject(CustomerId, Note, Status, valIds);
                CustomerService.savePendingMessage(Note, CustomerId, Status);
                CustomerService.CreateCustomerInvoice(CustomerId, SelectedRegionId, this.LoginUserId);
                var taxEmp = CustomerService.GetBillSetting().Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (Status == 1)
                {
                    if (taxEmp != null && taxEmp.TaxExcempt == false)
                    {
                        List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
                        List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
                        _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);
                    }
                }
            }
            return Json(new { Data = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectCustomerMaintenanceSuspension(int MaintenanceTempId, string Reason)
        {
            if (MaintenanceTempId > 0)
            {
                int MaintenanceTypeListId = 10;
                CustomerService.UpdateRejectCustomerMaintenance(MaintenanceTempId, Reason, MaintenanceTypeListId);
            }
            return Json(new { Data = MaintenanceTempId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AcceptCustomerMaintenanceSuspension(int MaintenanceTempId)
        {
            if (MaintenanceTempId > 0)
            {
                CustomerService.UpdateApprovalCustomerMaintenance(MaintenanceTempId);
            }
            return Json(new { Data = MaintenanceTempId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePendingStatus(int CustomerId, string Note, int Status)
        {
            if (CustomerId > 0)
            {

                CustomerService.UpdatePendingStatus(CustomerId, Note, Status);


                //var GetCustomerInfo = CustomerService.GetCustomerDetailsById(CustomerId);
                var GetCustomerInfo = jkEntityModel.Customers.Where(x => x.CustomerId == CustomerId).FirstOrDefault();


                if (GetCustomerInfo != null)
                {
                    var RegionName = jkEntityModel.Regions.Where(x => x.RegionId == GetCustomerInfo.RegionId).FirstOrDefault();

                    #region Email send function According to Admin configuration

                    //Get Feature Type Id by Feature Name
                    var getNew_Customer_Submit = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.New_Customer_Submit.ToString().Replace("_", " ")).FirstOrDefault();

                    if (getNew_Customer_Submit != null && getNew_Customer_Submit.FeatureTypeId > 0)
                    {
                        //Get Feature Type Email Id by Feature Type Id
                        var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == getNew_Customer_Submit.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                        if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                        {
                            //Get Feature Email Body By Message Name Or Tempalte Name 
                            var messageBody = jkEntityModel.MailMessageTemplates.Where(x => x.MailMessageTemplateId == messageDetails.MailMessageTemplateId).FirstOrDefault();
                            //MailMessageTemplateModel objItem = _commonService.GetEmailTemplate(MessageNameModel.FranchiseSecion1.ToString());
                            if (messageBody.MailMessageTemplateId > 0)
                            {
                                string MessageBody = messageBody.MessageBody;
                                string Subject = messageBody.Subject;

                                MessageBody = MessageBody.Replace("<<custname>>", !string.IsNullOrWhiteSpace(GetCustomerInfo.Name) ? GetCustomerInfo.Name : GetCustomerInfo.Name2);
                                MessageBody = MessageBody.Replace("<<regionname>>", RegionName.Name);

                                _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, MessageBody, Subject);

                                var CustEmailId = jkEntityModel.Emails.Where(x => x.TypeListId == 1 && x.ClassId == CustomerId && x.ContactTypeListId == 6).FirstOrDefault();
                                //Email send to Franchise if Admin Set to True 
                                if (messageDetails.EmailToCustomer == true && GetCustomerInfo != null && CustEmailId != null && !string.IsNullOrWhiteSpace(CustEmailId.EmailAddress))
                                {
                                    _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, CustEmailId.EmailAddress, MessageBody, Subject);
                                }
                            }


                        }
                    }
                    #endregion
                }



            }
            return Json(new { Data = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #region :: Customer Contract Detail ::

        [HttpGet]
        public ActionResult PendingApprovalCustomerContractDetailPopup(int id)
        {
            ViewBag.Id = id;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            string Address1 = string.Empty;
            string Address2 = string.Empty;
            CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null)
            {
                //var result = CustomerService.GetContractDetailResult(CustomerViewModel);
                CustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == CustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
                var servicename = CustomerService.GetServiceTypeList();
                foreach (var row in CustomerViewModel._ContractDetail)
                {
                    var name = servicename.Where(x => x.ServiceTypeListid == row.ServiceTypeListId).FirstOrDefault().name;
                    row.ServiceTypeName = name.ToString();
                }
            }
            if (CustomerViewModel != null && CustomerViewModel.MainAddress != null)
            {
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.Address1))
                {
                    Address1 = CustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.City))
                {
                    Address2 = CustomerViewModel.MainAddress.City;
                }
                if (CustomerViewModel.MainAddress.StateListId != null && CustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == CustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address2 += " " + StateName;
                }
                if (!string.IsNullOrEmpty(CustomerViewModel.MainAddress.PostalCode))
                {
                    Address2 += " " + CustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address1;
                ViewBag.Address2 = Address2;
            }

            ViewBag.cAgreement = Enum.GetValues(typeof(CAgreement)).Cast<CAgreement>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.cTerms = Enum.GetValues(typeof(CTerms)).Cast<CTerms>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.cFrequencyList = getFrequencyList();
            ViewBag.cAccountType = getCAccountType();
            ViewBag.statusList = getStatusList();
            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();
            ViewBag.SoldBy = getTaxAuthority();
            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", CustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }
            var FrequencyList = CustomerService.GetFrequencyList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            {
                ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            }
            else
            {
                ViewBag.FrequencyTypeList = new SelectList(FrequencyList, "FrequencyListId", "Name");
            }
            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", CustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            var ContractStatusList = CustomerService.GetContractStatusReasonList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.ContractStatusReasonListId != null)
            {
                ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", CustomerViewModel.Contract.ContractStatusReasonListId);
            }
            else
            {
                ViewBag.ContractStatusReasonList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            }

            var data = CustomerService.GetStatusList().Where(st => st.TypeListId == 1).OrderBy(s => s.Name).ToList();
            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (var y in data)
            {
                statusList.Add(new SelectListItem { Text = y.Name.ToString(), Value = y.StatusListId.ToString() });
            }
            ViewBag.statusList = statusList;
            var ServiceTypeListModel = CustomerService.GetServiceTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.ServiceTypeListId != null)
            {
                ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name", CustomerViewModel.ContractDetail.ServiceTypeListId);
            }
            else
            {
                ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");
            }
            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.BillingFrequencyListId != null)
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name", CustomerViewModel.ContractDetail.BillingFrequencyListId);
            }
            else
            {
                ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");
            }

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            if (CleanFrequencyListModel != null && CustomerViewModel.ContractDetail != null && CustomerViewModel.ContractDetail.CleanFrequencyListId != null)
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name", CustomerViewModel.ContractDetail.CleanFrequencyListId);
            }
            else
            {
                ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");
            }
            var AgreementTypeList = CustomerService.GetAgreementTypeList().ToList();
            if (CustomerViewModel != null && CustomerViewModel.Contract != null && CustomerViewModel.Contract.AgreementTypeListId != null)
            {
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name", CustomerViewModel.Contract.AgreementTypeListId);
                var AgreementTypeCPI = AgreementTypeList.Where(w => w.AgreementTypeListId == CustomerViewModel.Contract.AgreementTypeListId);
                if (AgreementTypeCPI != null)
                {
                    ViewBag.AgreementTypeCPI = (AgreementTypeCPI.FirstOrDefault().CPI.HasValue ? AgreementTypeCPI.FirstOrDefault().CPI.Value : false);
                }
            }
            else
            {
                ViewBag.AgreementTypeCPI = false;
                ViewBag.AgreementTypeList = new SelectList(AgreementTypeList, "AgreementTypeListId", "Name");
            }

            var stateData = CustomerService.GetStateList().OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList = stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();

            ViewBag.StateAbbr = new SelectList(statesList, "Value", "Text");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return PartialView("_PendingApprovalCustomerContractDetailPopup", CustomerViewModel);
        }

        [HttpPost]
        public ActionResult SavePendingApprovalCustomerContractDetails(FullCustomerViewModel fullCustomerViewModel, FormCollection collection)
        {
            int btnType = Convert.ToInt32(fullCustomerViewModel.ButtonType);
            ViewBag.CurrentMenu = "CustomerGeneral";

            ViewBag.PayeeInfoAddress_StateList = getUsStatesList();

            if (collection["ContractStatusList"] != null)
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractStatusList"]);
            }

            if (collection["ContractServiceTypeList"] != null)
            {
                fullCustomerViewModel.Contract.ContractStatusReasonListId = Convert.ToInt32(collection["ContractServiceTypeList"]);
            }

            if (collection["ContractTypeList"] != null)
            {
                fullCustomerViewModel.Contract.ContractTypeListId = Convert.ToInt32(collection["ContractTypeList"]);
            }

            if (collection["AccountTypeList"] != null)
            {
                fullCustomerViewModel.Contract.AccountTypeListId = Convert.ToInt32(collection["AccountTypeList"]);
            }
            if (collection["AgreementTypeList"] != null)
            {
                fullCustomerViewModel.Contract.AgreementTypeListId = Convert.ToInt32(collection["AgreementTypeList"]);
            }

            fullCustomerViewModel.Contract.CustomerId = fullCustomerViewModel.CustomerViewModel.CustomerId;
            //fullCustomerViewModel.Contract.ContractTermListId = fullCustomerViewModel.Contract.ContractTermListId;
            fullCustomerViewModel.Contract.ContractTermMonth = fullCustomerViewModel.Contract.ContractTermMonth;
            fullCustomerViewModel.Contract.CreatedDate = DateTime.Now;
            fullCustomerViewModel.Contract.isActive = true;
            fullCustomerViewModel.Contract = CustomerService.SaveContract(fullCustomerViewModel.Contract.ToModel<Contract, ContractViewModel>()).ToModel<ContractViewModel, Contract>();

            ViewBag.MainStatesList = getUsStatesList();
            ViewBag.BillingStatesList = getUsStatesList();
            ViewBag.TaxAuthority = getTaxAuthority();
            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));

            fullCustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(fullCustomerViewModel.CustomerViewModel.CustomerId));
            if (fullCustomerViewModel != null && fullCustomerViewModel.MainAddress != null)
            {
                string Address = string.Empty;
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.Address1))
                {
                    Address = fullCustomerViewModel.MainAddress.Address1;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.City))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.City;
                }
                if (fullCustomerViewModel.MainAddress.StateListId != null && fullCustomerViewModel.MainAddress.StateListId > 0)
                {
                    var States = CustomerService.GetStateList();
                    string StateName = States.Where(one => one.StateListId == fullCustomerViewModel.MainAddress.StateListId).Select(one => one.abbr).FirstOrDefault();
                    Address += " " + StateName;
                }
                if (!string.IsNullOrEmpty(fullCustomerViewModel.MainAddress.PostalCode))
                {
                    Address += " " + fullCustomerViewModel.MainAddress.PostalCode;
                }
                ViewBag.Address = Address;
            }

            var ContractTypeList = CustomerService.GetContractTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractTypeListId != null)
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name", fullCustomerViewModel.Contract.ContractTypeListId);
            }
            else
            {
                ViewBag.ContractTypeList = new SelectList(ContractTypeList, "ContractTypeListId", "Name");
            }

            var ContractStatusList = CustomerService.GetARStatusReasonList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.ContractStatusReasonListId != null)
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name", fullCustomerViewModel.Contract.ContractStatusReasonListId);
            }
            else
            {
                ViewBag.ContractStatusList = new SelectList(ContractStatusList, "ContractStatusListId", "Name");
            }

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();
            if (fullCustomerViewModel != null && fullCustomerViewModel.Contract != null && fullCustomerViewModel.Contract.AccountTypeListId != null)
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name", fullCustomerViewModel.Contract.AccountTypeListId);
            }
            else
            {
                ViewBag.AccountTypeList = new SelectList(AccountTypeList, "AccountTypeListId", "Name");
            }

            if (fullCustomerViewModel != null && fullCustomerViewModel.ContractDetail != null)
            {
                fullCustomerViewModel._ContractDetail = CustomerService.GetContractDetail().Where(one => one.ContractId == fullCustomerViewModel.ContractDetail.ContractId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
            }
            //return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = fullCustomerViewModel.CustomerViewModel.CustomerId });
            //return RedirectToAction("PendingApprovalList", "Customer", new { area = "Portal", @id = 4 });
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        #endregion :: Customer Contract Detail ::

        #endregion :: Pending Approval List ::

        #region ::  Customer Detail Manual Invoice ::

        [HttpGet]
        public ActionResult RenderCustomerDetailManualInvoicePopup(int id)
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Manual Invoice");

            ViewBag.ContractDetailServiceTypeList = AccountReceivableService.GetContractDetailServiceTypeList().Select(one => new SelectListItem { Text = one.Name, Value = one.ContractDetailServiceTypeListId.ToString() }).ToList();

            CustomerTransactionCommonViewModel oCustomerTransactionCommonViewModel = new CustomerTransactionCommonViewModel();
            oCustomerTransactionCommonViewModel.CustomerId = id;

            return PartialView("_CustomerDetailManualInvoice", oCustomerTransactionCommonViewModel);
        }

        [HttpPost]
        public ActionResult ManualInvoice(FormCollection frm)
        {
            CustomerTransactionCommonViewModel oCustomerTransactionCommonViewModel = new CustomerTransactionCommonViewModel();
            List<CustomerTransactionViewModel> lstCustomerTransactionViewModel = new List<CustomerTransactionViewModel>();
            List<JKViewModels.AccountReceivable.FranchiseeTransactionViewModel> lstFranchiseeTransactionViewModel = new List<JKViewModels.AccountReceivable.FranchiseeTransactionViewModel>();
            CustomerTransactionViewModel oCustomerTransactionViewModel;
            JKViewModels.AccountReceivable.FranchiseeTransactionViewModel oFranchiseeTransactionViewModel;

            var TaxRateId = !String.IsNullOrEmpty(frm["hdfTaxRateId"]) ? int.Parse(frm["hdfTaxRateId"]) : 0;
            var ContractTaxRate = !String.IsNullOrEmpty(frm["hdfContractTaxRate"]) ? decimal.Parse(frm["hdfContractTaxRate"]) : 0;
            var LeaseTaxRate = !String.IsNullOrEmpty(frm["hdfLeaseTaxRate"]) ? decimal.Parse(frm["hdfLeaseTaxRate"]) : 0;
            var SupplyTaxRate = !String.IsNullOrEmpty(frm["hdfSupplyTaxRate"]) ? decimal.Parse(frm["hdfSupplyTaxRate"]) : 0;

            oCustomerTransactionCommonViewModel.BillMonth = 0;
            oCustomerTransactionCommonViewModel.BillYear = 0;
            oCustomerTransactionCommonViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            oCustomerTransactionCommonViewModel.CreatedDate = DateTime.Now;
            if (Request.Form["SaveClose1"] != null)
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            else
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            oCustomerTransactionCommonViewModel.InvoiceDate = DateTime.Parse(frm["txtInvoicedate"]);
            oCustomerTransactionCommonViewModel.InvoiceDescription = frm["txtInvoiceDescription"] != null ? frm["txtInvoiceDescription"].ToString() : "";

            for (int i = 1; i <= int.Parse(frm["hdftotallineno"]); i++)
            {
                oCustomerTransactionViewModel = new CustomerTransactionViewModel();
                oCustomerTransactionViewModel.CustomerTransactionId = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.ServiceTypeListId = int.Parse(frm["ContractDetailServiceTypeList" + i]);
                oCustomerTransactionViewModel.LineNo = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.Description = frm["txtdescription" + i].ToString();
                oCustomerTransactionViewModel.MarkUpTotal = decimal.Parse(frm["txtmarkup" + i] != "" ? frm["txtmarkup" + i] : "0");
                oCustomerTransactionViewModel.Quantity = int.Parse(frm["txtqty" + i]);
                oCustomerTransactionViewModel.UnitPrice = decimal.Parse(frm["txtunitprice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.ExtendedPrice = decimal.Parse(frm["txtExtendedPrice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TaxAmount = decimal.Parse(frm["txttax" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.Total = decimal.Parse(frm["txttotal" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TransactionStatusListId = 1;
                if (int.Parse(frm["tablerdo" + i]) == 1)
                {
                    oCustomerTransactionViewModel.Commission = true;
                    oCustomerTransactionViewModel.CommissionTotal = Decimal.Parse(frm["txtCommissionAmount" + i]);
                }
                else
                {
                    oCustomerTransactionViewModel.Commission = false;
                }
                if (int.Parse(frm["tablerdo" + i]) == 2)
                {
                    oCustomerTransactionViewModel.ExtraWork = true;
                }
                else { oCustomerTransactionViewModel.ExtraWork = false; }

                if (int.Parse(frm["tablerdo" + i]) == 4)
                {
                    oCustomerTransactionViewModel.ClientSupplies = true;
                }
                else { oCustomerTransactionViewModel.ClientSupplies = false; }

                //oCustomerTransactionViewModel.Commission = bool.Parse(frm["chkCommission" + i] != null ? "true" : "false");
                //oCustomerTransactionViewModel.CommissionTotal = Decimal.Parse(String.IsNullOrEmpty(frm["txtCommissionAmount" + i]) ? "0" : frm["txtCommissionAmount" + i]);
                //oCustomerTransactionViewModel.ExtraWork = bool.Parse(frm["chkExtraWork" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.TaxExcempt = bool.Parse(frm["chkTaxExcempt" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.AccountRebate = false;// bool.Parse(frm["chkAcctRebate" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.BPPAdmin = false; //bool.Parse(frm["chkBPPAdmin" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.PrintInvoice = bool.Parse(frm["chkPrintInvoice" + i] != null ? "true" : "false");


                if (!oCustomerTransactionViewModel.TaxExcempt)
                {
                    oCustomerTransactionViewModel.TaxRate = TaxRateId;
                    oCustomerTransactionViewModel.TaxPercentage = ContractTaxRate;
                }
                else
                {
                    oCustomerTransactionViewModel.TaxRate = 0;
                    oCustomerTransactionViewModel.TaxPercentage = 0;
                }

                lstCustomerTransactionViewModel.Add(oCustomerTransactionViewModel);

                for (int j = 1; j <= int.Parse(frm["hdfftotallineno"]); j++)
                {
                    if (!String.IsNullOrEmpty(frm["hdfFrenchiseeId" + j]) && (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == i || int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1))
                    {
                        oFranchiseeTransactionViewModel = new JKViewModels.AccountReceivable.FranchiseeTransactionViewModel();
                        oFranchiseeTransactionViewModel.FranchiseeTransactionId = j;
                        if (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1)
                            oFranchiseeTransactionViewModel.Amount = oCustomerTransactionViewModel.ExtendedPrice;
                        else
                            oFranchiseeTransactionViewModel.Amount = decimal.Parse(frm["frfranchiseeamount" + j].ToString().Replace("$", "").Replace(",", ""));
                        oFranchiseeTransactionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                        oFranchiseeTransactionViewModel.CreatedDate = DateTime.Now;
                        oFranchiseeTransactionViewModel.CustomerTransactionId = oCustomerTransactionViewModel.CustomerTransactionId;
                        oFranchiseeTransactionViewModel.FranchiseeId = int.Parse(frm["hdfFrenchiseeId" + j]);
                        oFranchiseeTransactionViewModel.IsDelete = false;
                        oFranchiseeTransactionViewModel.TransactionStatusListId = 1;
                        lstFranchiseeTransactionViewModel.Add(oFranchiseeTransactionViewModel);
                    }
                }
            }
            oCustomerTransactionCommonViewModel.CustomerTransactions = lstCustomerTransactionViewModel;
            oCustomerTransactionCommonViewModel.FranchiseeTransactions = lstFranchiseeTransactionViewModel;
            bool retVal = AccountReceivableService.InsertCustomerTransaction(oCustomerTransactionCommonViewModel);

            if (Request.Form["SaveNew"] != null)
            {
                ViewBag.CurrentMenu = "AccountsReceivableInvoices";
                BreadCrumb.Clear();
                BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
                BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Invoices");
                BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Manual Invoice");

                ViewBag.ContractDetailServiceTypeList = AccountReceivableService.GetContractDetailServiceTypeList().Select(one => new SelectListItem { Text = one.Name, Value = one.ContractDetailServiceTypeListId.ToString() }).ToList();
                return View();
            }
            else if (Request.Form["SaveClose"] != null)
            {
                return RedirectToAction("GenerateInvoice", "AccountReceivable", new { area = "Portal" });
            }
            else if (Request.Form["SaveClose1"] != null)
            {
                return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = int.Parse(frm["CustomerId"]) });
            }

            return RedirectToAction("CustomerDetail", "Customer", new { area = "Portal", id = int.Parse(frm["CustomerId"]) });
        }

        [HttpPost]
        public JsonResult ManualInvoiceIncrease(FormCollection frm)
        {
            CustomerTransactionCommonViewModel oCustomerTransactionCommonViewModel = new CustomerTransactionCommonViewModel();
            List<CustomerTransactionViewModel> lstCustomerTransactionViewModel = new List<CustomerTransactionViewModel>();
            List<JKViewModels.AccountReceivable.FranchiseeTransactionViewModel> lstFranchiseeTransactionViewModel = new List<JKViewModels.AccountReceivable.FranchiseeTransactionViewModel>();
            CustomerTransactionViewModel oCustomerTransactionViewModel;
            JKViewModels.AccountReceivable.FranchiseeTransactionViewModel oFranchiseeTransactionViewModel;

            var TaxRateId = !String.IsNullOrEmpty(frm["hdfTaxRateId"].ToString()) ? int.Parse(frm["hdfTaxRateId"]) : 0;
            var ContractTaxRate = frm["hdfContractTaxRate"] != null ? decimal.Parse(frm["hdfContractTaxRate"]) : 0;
            var LeaseTaxRate = !String.IsNullOrEmpty(frm["hdfLeaseTaxRate"].ToString()) ? decimal.Parse(frm["hdfLeaseTaxRate"]) : 0;
            var SupplyTaxRate = !String.IsNullOrEmpty(frm["hdfSupplyTaxRate"].ToString()) ? decimal.Parse(frm["hdfSupplyTaxRate"]) : 0;

            oCustomerTransactionCommonViewModel.BillMonth = 0;
            oCustomerTransactionCommonViewModel.BillYear = 0;
            oCustomerTransactionCommonViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            oCustomerTransactionCommonViewModel.CreatedDate = DateTime.Now;
            if (Request.Form["SaveClose1"] != null)
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            else
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            oCustomerTransactionCommonViewModel.InvoiceDate = DateTime.Parse(frm["txtInvoicedate"]);
            oCustomerTransactionCommonViewModel.InvoiceDescription = frm["txtInvoiceDescription"] != null ? frm["txtInvoiceDescription"].ToString() : "";

            for (int i = 1; i <= int.Parse(frm["hdftotallineno"]); i++)
            {
                oCustomerTransactionViewModel = new CustomerTransactionViewModel();
                oCustomerTransactionViewModel.CustomerTransactionId = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.ServiceTypeListId = !string.IsNullOrEmpty(frm["ContractDetailServiceTypeList" + i]) ? int.Parse(frm["ContractDetailServiceTypeList" + i]) : 0;
                oCustomerTransactionViewModel.LineNo = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.Description = frm["txtdescription" + i].ToString();
                oCustomerTransactionViewModel.MarkUpTotal = decimal.Parse(frm["txtmarkup" + i] != "" ? frm["txtmarkup" + i] : "0");
                oCustomerTransactionViewModel.Quantity = int.Parse(frm["txtqty" + i]);
                oCustomerTransactionViewModel.UnitPrice = decimal.Parse(frm["txtunitprice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.ExtendedPrice = decimal.Parse(frm["txtExtendedPrice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TaxAmount = decimal.Parse(frm["txttax" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.Total = decimal.Parse(frm["txttotal" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TransactionStatusListId = 1;
                oCustomerTransactionViewModel.Commission = bool.Parse(frm["chkCommission" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.CommissionTotal = Decimal.Parse(String.IsNullOrEmpty(frm["txtCommissionAmount" + i]) ? "0" : frm["txtCommissionAmount" + i]);
                oCustomerTransactionViewModel.ExtraWork = bool.Parse(frm["chkExtraWork" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.TaxExcempt = bool.Parse(frm["chkTaxExcempt" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.AccountRebate = bool.Parse(frm["chkAcctRebate" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.BPPAdmin = bool.Parse(frm["chkBPPAdmin" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.PrintInvoice = bool.Parse(frm["chkPrintInvoice" + i] != null ? "true" : "false");


                if (!oCustomerTransactionViewModel.TaxExcempt)
                {
                    oCustomerTransactionViewModel.TaxRate = TaxRateId;
                    oCustomerTransactionViewModel.TaxPercentage = ContractTaxRate;
                }
                else
                {
                    oCustomerTransactionViewModel.TaxRate = 0;
                    oCustomerTransactionViewModel.TaxPercentage = 0;
                }

                lstCustomerTransactionViewModel.Add(oCustomerTransactionViewModel);

                for (int j = 1; j <= int.Parse(frm["hdfftotallineno"]); j++)
                {
                    if (frm["hdfFrenchiseeId" + j] != null && (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == i || int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1))
                    {
                        oFranchiseeTransactionViewModel = new JKViewModels.AccountReceivable.FranchiseeTransactionViewModel();
                        oFranchiseeTransactionViewModel.FranchiseeTransactionId = j;
                        if (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1)
                            oFranchiseeTransactionViewModel.Amount = oCustomerTransactionViewModel.ExtendedPrice;
                        else
                            oFranchiseeTransactionViewModel.Amount = decimal.Parse(frm["frfranchiseeamount" + j].ToString().Replace("$", "").Replace(",", ""));
                        oFranchiseeTransactionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                        oFranchiseeTransactionViewModel.CreatedDate = DateTime.Now;
                        oFranchiseeTransactionViewModel.CustomerTransactionId = oCustomerTransactionViewModel.CustomerTransactionId;
                        oFranchiseeTransactionViewModel.FranchiseeId = int.Parse(frm["hdfFrenchiseeId" + j]);
                        oFranchiseeTransactionViewModel.IsDelete = false;
                        oFranchiseeTransactionViewModel.TransactionStatusListId = 1;
                        lstFranchiseeTransactionViewModel.Add(oFranchiseeTransactionViewModel);
                    }
                }
            }
            oCustomerTransactionCommonViewModel.CustomerTransactions = lstCustomerTransactionViewModel;
            oCustomerTransactionCommonViewModel.FranchiseeTransactions = lstFranchiseeTransactionViewModel;
            bool retVal = AccountReceivableService.InsertCustomerTransaction(oCustomerTransactionCommonViewModel);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion ::  Customer Distribution ::

        public ActionResult GetAgreementTypeDetailWithId(int Id)
        {
            var Data = CustomerService.GetAgreementTypeListById(Id);
            if (Data != null && Data.CPI != null)
            {
                return Json((Data.CPI == true ? 1 : 0), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region :: Customer Account History ::
        public ActionResult CustomerAccountHistoryPopup(int id)
        {
            var vm = CustomerService.GetCustomerN(id);

            return PartialView("_CustomerAccountHistoryPopup", vm);
        }

        public ActionResult IncreaseDecreaseReportPopup(int id)
        {

            ViewBag.CustomerId = id;
            ViewBag.selectedRegionId = SelectedRegionId;

            return PartialView("_IncreaseDecreaseReportPopup");
        }

        public ActionResult CustomerAccountHistoryPopupDetails(int customerId, int numberOfMonths)
        {
            var startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var startOfReport = startOfThisMonth.AddMonths(-(numberOfMonths - 1));

            var vm = CustomerService.GetCustomerAccountHistory(customerId, startOfReport);

            return PartialView("_CustomerAccountHistoryPopupDetails", vm);
        }
        public JsonResult CustomerAccountHistoryDetailsPrint(int customerId, int numberOfMonths)
        {
            if (customerId > 0)
            {
                var Customer = CustomerService.GetCustomerN(customerId);
                ViewBag.CustomerData = Customer;

                var startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var startOfReport = startOfThisMonth.AddMonths(-(numberOfMonths - 1));
                var vm = CustomerService.GetCustomerAccountHistory(customerId, startOfReport);

                string HTMLContent = string.Empty;
                HTMLContent += CRenderPartialViewToString("_CustomerAccountHistoryDetailsPrint", vm);

                var retPath = "/Upload/InvoiceFiles/" + "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDFPortrait(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public FileResult CustomerAccountHistoryExportPdf(int customerId, int numberOfMonths)
        {
            if (customerId > 0)
            {
                var Customer = CustomerService.GetCustomerN(customerId);
                ViewBag.CustomerData = Customer;

                var startOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var startOfReport = startOfThisMonth.AddMonths(-(numberOfMonths - 1));
                var vm = CustomerService.GetCustomerAccountHistory(customerId, startOfReport);

                string HTMLContent = string.Empty;
                HTMLContent += CRenderPartialViewToString("_CustomerAccountHistoryDetailsPrint", vm);

                return File(GetPDFPortrait(HTMLContent), "application/pdf", "CFF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
            }
            return null;
        }

        #endregion

        #region :: Customer Account Offring ::

        public ActionResult GetAccountOffringResult(int id, string frt, decimal dis, string sdate, string edate)
        {
            //ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            //ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            //ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");


            return PartialView("_PartialCustomerAccountOffringResult", CustomerService.GetAccountOfferingResult(id, frt, dis, sdate, edate));
        }

        public ActionResult PartialBindMapAccountOffringResult(int id, string frt, decimal dis, string sdate, string edate)
        {

            //jOb = "[{ \"title\":\"BUFFALO BILLS, LLC. - SEASONAL\",\"lat\":\"42.77326570\",\"lng\":\"-78.78585660\",\"description\":118556},{ \"title\":\"THE REGNET ENTERPRISE, LLC\",\"lat\":\"42.8337105\",\"lng\":\"-78.8208438\",\"description\":20},{ \"title\":\"TANYA SOLER\",\"lat\":\"42.8682845\",\"lng\":\"-78.8388063\",\"description\":67},{ \"title\":\"ANDREW GERACE\",\"lat\":\"42.8703805\",\"lng\":\"-78.837269\",\"description\":73},{ \"title\":\"ROMEO SANTIANO\",\"lat\":\"42.865932\",\"lng\":\"-78.724708\",\"description\":75},{ \"title\":\"BAJRO HADZIC\",\"lat\":\"42.9060911\",\"lng\":\"-78.8314734\",\"description\":85},{ \"title\":\"JOHNNY ROBINSON\",\"lat\":\"42.7977814\",\"lng\":\"-78.8181415\",\"description\":95},{ \"title\":\"SAMO ENTERPRISES, LLC\",\"lat\":\"42.8856052\",\"lng\":\"-78.814505\",\"description\":96},{ \"title\":\"GRANDNUBIAN, LLC\",\"lat\":\"42.9114215\",\"lng\":\"-78.8100578\",\"description\":97},{ \"title\":\"WILLIAM BROWN, JR.\",\"lat\":\"42.9001406\",\"lng\":\"-78.8305886\",\"description\":102},{ \"title\":\"LEHMAN CORP.\",\"lat\":\"42.864588\",\"lng\":\"-78.6997648\",\"description\":111}]";


            CommonAccountOfferingViewModel oCommonAccountOfferingViewModel = CustomerService.GetAccountOfferingResult(id, frt, dis, sdate, edate);
            List<markers> lstmarkers = new List<markers>();
            markers omarkers;

            if (oCommonAccountOfferingViewModel != null)
            {
                omarkers = new markers();
                omarkers.title = oCommonAccountOfferingViewModel.CustomerDetail.CustomerName;
                omarkers.lat = oCommonAccountOfferingViewModel.CustomerDetail.CustomerLatitude;
                omarkers.lng = oCommonAccountOfferingViewModel.CustomerDetail.CustomerLongitude;
                omarkers.description = oCommonAccountOfferingViewModel.CustomerDetail.CustomerId.ToString();
                omarkers.number = oCommonAccountOfferingViewModel.CustomerDetail.CustomerNo.ToString();
                lstmarkers.Add(omarkers);

                foreach (var item in oCommonAccountOfferingViewModel.lstAccountOfferingResult)
                {
                    omarkers = new markers();
                    omarkers.title = item.FranchiseeName;
                    omarkers.lat = item.Latitude;
                    omarkers.lng = item.Longitude;
                    omarkers.description = item.FranchiseeId.ToString();
                    omarkers.number = item.FranchiseeNo;
                    lstmarkers.Add(omarkers);

                }
            }



            return View("PartialBindMap", lstmarkers);
        }


        public ActionResult PartialCustomerAccountOffringListResult(int id, string frt, decimal dis, string sdate, string edate)
        {
            CommonAccountOfferingViewModel oCommonAccountOfferingViewModel = CustomerService.GetAccountOfferingResult(id, frt, dis, sdate, edate);
            return PartialView("_PartialCustomerAccountOffringListResult", oCommonAccountOfferingViewModel);
        }



        public JsonResult InsertOfferingData(int cid, string fids, DateTime exDate, DateTime extime, string note)
        {
            List<AccountOfferingResponceViewModel> lstAccountOfferingResponce = CustomerService.InsertOfferingData(cid, fids, exDate, extime, note);
            var claimView = ClaimView.Instance;
            var regionName = claimView.GetCLAIM_PERSON_INFORMATION().Regions.FirstOrDefault(x => x.RegionId == SelectedRegionId).Displayname;
            foreach (var item in lstAccountOfferingResponce)
            {
                var SendEmailcontext = System.Web.HttpContext.Current.Request;
                string body = "";
                string subject = "Notice of Account Offering";
                var url = SendEmailcontext.Url.Authority;
                var scheme = SendEmailcontext.Url.Scheme;
                var fomP = Convert.ToString(ConfigurationManager.AppSettings["fomProd"]);
                var fomD = Convert.ToString(ConfigurationManager.AppSettings["fomDev"]);
                if (SendEmailcontext.Url.Authority.StartsWith("localhost"))
                {
                    body += "<div style=\"width:600px;padding:15px;\">";
                    body += "<p>" + item.Name + "</p>";
                    //body += "<h3>Offer email content will go here </h3>";
                    body += "<p>You are being offered the opportunity to provide services to an account.  Please click on the link below to view specifics about the account.</p>";
                    body += "<h4><a href=\"" + scheme + "://" + url + "/Portal/Verification/Offering/" + item.OfferingId + "\" >Review to Accept or Reject offer by click here!</a></h4>";
                    body += "<p>This offer is exclusive to you for the time period listed below.</p>";
                    body += "<p>Date and time of expire <span style='color: red;'> " + exDate.ToShortDateString() + " , " + extime.ToShortTimeString() + "</span></p>";
                    body += "<p>If you have any questions, please contact the regional office.</p>";
                    body += "<hr/>";
                    body += "<strong>" + regionName + "</strong><br />";
                    body += "<img src='~/Content/admin/assets/global/img/mailSign.jpg' />";
                    body += "</div>";
                }
                else
                {
                    body += "<div style=\"width:600px;padding:15px;\">";
                    body += "<p>" + item.Name + "</p>";
                    //body += "<h3>Offer email content will go here </h3>";
                    body += "<p>You are being offered the opportunity to provide services to an account.  Please click on the link below to view specifics about the account.</p>";
                    if (SendEmailcontext.Url.Authority == "fms.ecom.bid")
                    {
                        body += "<h4><a href=\"" + fomD + "/Portal/Verification/Offering/" + item.OfferingId + "\" >Review to Accept or Reject offer by click here!</a></h4>";
                    }
                    if (SendEmailcontext.Url.Authority == "fms.janiking.com")
                    {
                        body += "<h4><a href=\"" + fomP + "/Portal/Verification/Offering/" + item.OfferingId + "\" >Review to Accept or Reject offer by click here!</a></h4>";
                    }
                    body += "<p>This offer is exclusive to you for the time period listed below.</p>";
                    body += "<p>Date and time of expire <span style='color: red;'> " + exDate.ToShortDateString() + " , " + extime.ToShortTimeString() + "</span></p>";
                    body += "<p>If you have any questions, please contact the regional office.</p>";
                    body += "<hr/>";
                    body += "<strong>" + regionName + "</strong><br />";
                    body += "<img src='~/Content/admin/assets/global/img/mailSign.jpg' />";
                    body += "</div>";
                }
                //JKApi.Core.Common.ClaimView.Instance.GetCLAIM_PERSON_INFORMATION().Email
                //_mailService.SendEmailAsync("pnguyen@janiking.com", body, subject, emailToDisplayName: item.Name);
                _mailService.SendEmailAsync(item.EmailAddress, body, subject, emailToDisplayName: item.Name);

            }



            return Json(true, JsonRequestBehavior.AllowGet);




        }

        public JsonResult AccountOfferingEmailSend(string _emailFrom, string _emailto, string _emailcc, string _emailsubject, string _emailBody, int cid, string fids)
        {
            string emailTo = string.Empty;
            string emailtext = " Need content from Oprations\nText also continue here...";
            foreach (string franchiseeId in fids.Split(','))
            {
                var emai = CustomerService.GetEmailAddrress(int.Parse(franchiseeId), 2);
                emailTo = (emailTo != "" ? (emailTo + ";") : "") + emai.EmailAddress;
            }
            int userid = JKApi.Core.Common.ClaimView.Instance.GetCLAIM_PERSON_INFORMATION().UserId;
            var newEmailMessages = JKApi.Core.MailService.Instance.GetNewEmail(userid);

            var emailBody = "{'emailfrom':'" + newEmailMessages + "','emailto':'" + emailTo + "','emailsubject':'Client Contract Offer','emailbody':'" + emailtext + "'}";
            return Json(emailBody, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region ::Check Company Name Exits ::
        public ActionResult CheckCustomernameExits(string CustomerName, string Phone)
        {
            int CustomerId = CustomerService.CheckOnlyCustomerNamePhoneIsExist(CustomerName, Phone);
            return Json(new { CustomerId = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerValidationPopup()
        {
            return PartialView("_NewCustomerDetailVefiry");
        }
        #endregion



        #region :: Customer Maintenance ::
        [HttpGet]
        public ActionResult CustomerMaintenanceDetailPP(int id)
        {
            ViewBag.FindersFeeAdjustmentTypeListRaw = CustomerService.GetFindersFeeAdjustmentTypeList();//, "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusListRaw = FranchiseeService.GetAll_TransactionStatusList();//, "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeListRaw = FranchiseeService.GetAll_FindersFeeTypeList();//, "FindersFeeTypeListId", "Name");

            ViewBag.FindersFeeAdjustmentTypeList = new SelectList(CustomerService.GetFindersFeeAdjustmentTypeList(), "FindersFeeAdjustmentTypeListId", "Name");
            ViewBag.TransactionStatusList = new SelectList(FranchiseeService.GetAll_TransactionStatusList(), "TransactionStatusListId", "Name");
            ViewBag.FindersFeeTypeList = new SelectList(FranchiseeService.GetAll_FindersFeeTypeList(), "FindersFeeTypeListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(o => o.contracttype == 1 && o.TypeListId == 1).ToList();
            ViewBag.ServiceTypeListModel = new SelectList(ServiceTypeListModel, "ServiceTypeListId", "name");

            var FrequencyListModel = CustomerService.GetFrequencyList().ToList();
            ViewBag.FrequencyListModel = new SelectList(FrequencyListModel, "FrequencyListId", "Name");

            var CleanFrequencyListModel = CustomerService.GetCleanFrequencyList().ToList();
            ViewBag.CleanFrequencyListModel = new SelectList(CleanFrequencyListModel, "CleanFrequencyListId", "Name");

            var AccountTypeList = CustomerService.GetAccountTypeList().ToList();

            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusList.ToString()), "Value", "Text", 0);
            ViewBag.StatusReasonList = new SelectList(_commonService.DropDownListByName(MasterDropName.CustomerStatusReasonList.ToString()), "Value", "Text", 0);
            CommonCustomerMaintenanceDetailViewModel oRetVal = CustomerService.GetCustomerMaintenanceDetailDataPP(id);
            return PartialView("_CustomerMaintenanceDetailPP", oRetVal);
        }

        [HttpGet]
        public JsonResult CustomerMaintenanceDelete(int id)
        {
            CustomerService.GetCustomerMaintenanceDataDelete(id);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CustomerRevenuesResult(string regions = "0", int periodid = 0)
        {
            return PartialView("_PartialCustomerRevenueResult", CustomerService.GetCustomerRevenuesReportData(regions, periodid));
        }

        public FileResult CustomerRevenuesResultExportPDFfile(string regions = "0", int periodid = 197)
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
                var model = CustomerService.GetCustomerRevenuesReportData(regions, periodid);

                HTMLContent += RenderPartialViewToString("_PartialCustomerRevenueExportPDFResult", model);

                return File(GetPDF(HTMLContent), "application/pdf", "_CustomerRevenueExportToPDF.pdf");
            }
            return null;
        }

        public byte[] GetPDF(string pHTML)
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

        [HttpGet]
        public JsonResult GetServiceTypeListManualInvoice()
        {
            List<ServiceTypeList> lstServiceTypeList =
                CustomerService.GetServiceTypeList().Where(x => x.TypeListId == 1).OrderBy(o => o.name).ToList();
            return Json(lstServiceTypeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IncreaseDecreaseHistoryListDataTable(string regionIds, DateTime? from, DateTime? to, int month = 0, int year = 0, int customerId = 0)
        {
            if (regionIds == "null") regionIds = null;
            try
            {
                var response = CustomerService.GetIncreaseDecreaseHistoryList(regionIds, from, to, month, year, customerId);
                var result = from f in response
                             select new
                             {
                                 IncreaseDecreaseHistoryId = f.IncreaseDecreaseHistoryId,
                                 CustomerNo = f.CustomerNo,
                                 CustomerName = f.CustomerName,
                                 MaintenanceTypeListName = f.MaintenanceTypeListName,
                                 MaintenanceDetailTypeListName = f.MaintenanceDetailTypeListName,
                                 IncreaseDecreaseTypeListName = f.IncreaseDecreaseTypeListName,
                                 SourceType = f.SourceType,
                                 TransactionDate = f.TransactionDate != null ? f.TransactionDate : null,
                                 Description = f.Description,
                                 Amount = String.Format("{0:C}", f.Amount),
                                 CPIPresent = f.CPIPresent,
                                 PeriodId = f.PeriodId,
                                 DetailPrevAmt = String.Format("{0:C}", f.DetailPrevAmt),
                                 DetailVarAmt = String.Format("{0:C}", f.DetailVarAmt),
                                 DetailNewAmt = String.Format("{0:C}", f.DetailNewAmt),
                                 ContractPrevAmt = String.Format("{0:C}", f.ContractPrevAmt),
                                 ContractVarAmt = String.Format("{0:C}", f.ContractVarAmt),
                                 ContractNewAmt = String.Format("{0:C}", f.ContractNewAmt),
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

        public ActionResult SaveCustomerNotesDetails(int Id, int ClassId, string Notes = "",int _type = 0)
        {
            if (ClassId > 0)
            {
                Id = CustomerService.SaveNotesDetail(Id, ClassId, Notes, _type);
            }
            return Json(Id, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenCustomerDetailPopup(int id)
        {

            if (id < 0)
            {
                dynamic GetUsStates = getUsStatesList();
                ViewBag.MainStatesList = GetUsStates;
                ViewBag.BillingStatesList = GetUsStates;
                ViewBag.ArStatus = new SelectList(getARStatusResonList(), "ARStatusReasonListId", "Name");
                ViewBag.InvoiceDate = new SelectList(getInvoiceDate(), "InvoiceDateListId", "Name");
                ViewBag.TermDate = new SelectList(getTermDate(), "InvoiceTermListId", "Name");
            }

            ViewBag.PaymentMethodList = new SelectList(AccountReceivableService.GetAll_PaymentMethodList(), "PaymentMethodListId", "Name");

            int? CustomerID = id;
            ViewBag.CustomerID = CustomerID;
            int? CustomerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            int? CustomerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers().Where(x => x.CustomerDetailView == true).ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList = AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
                //ViewBag.TransactionsTypeList = new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name");
            }

            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1();
            if (id > 0)
            {
                CustomerDetailViewModel customerDetailViewModel = GetCustomerDetailViewModel(CustomerID);
                customerDetailViewModel.Balance = AccountReceivableService.GetCustomerCreditBalance(Convert.ToInt32(CustomerID));
                // FullCustomerViewModel = new FullCustomerViewModel1();
                FullCustomerViewModel = Maintennacepopup(id);

                //foreach (EmailViewModel op in FullCustomerViewModel.lstEBillEmails)
                //{
                //    FullCustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                //}
                //if (FullCustomerViewModel.EBill_Emails.Length > 0)
                //    FullCustomerViewModel.EBill_Emails = FullCustomerViewModel.EBill_Emails.Trim(',');

                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
            }
            else
            {
                //FullCustomerViewModel = new FullCustomerViewModel1();
                //FullCustomerViewModel.CustomerDetail = new CustomerDetailViewModel();
            }
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(id);
            ViewBag.Distributions = CustomerDistributions;
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }
            if (FullCustomerViewModel != null)
            {
                ViewBag.CustBillingEmail = (FullCustomerViewModel.BillingEmail != null ? FullCustomerViewModel.BillingEmail.EmailAddress : string.Empty);
            }
            else
            {
                ViewBag.CustBillingEmail = "";
            }
            //return View(FullCustomerViewModel);
            return PartialView("_PartialCustomerDetailPopup", FullCustomerViewModel);
        }

        public ActionResult OpenCustomerInspectionHistoryPopup(int id)
        {
            if (id > 0)
            {
                // TODO: Use GetInspectionFormListByCustomer
                // var inspectionViewModel = _inspectionService.GetInspectionListByCustomer(new ViewInspectionListModel
                // { CustomerId = id, IsEnable = true });
                var inspectionViewModel = new ViewInspectionListModel();
                var inspectionList = new List<InspectionModel>();
                foreach (var inspection in inspectionViewModel.InspectionList)
                {
                    if (inspection.IsCompleted)
                    {
                        inspectionList.Add(inspection);
                    }
                }
                return PartialView("_PartialCustomerInspectionHistoryPopup", inspectionList);
            }
            return PartialView("_PartialCustomerInspectionHistoryPopup", new ViewInspectionListModel().InspectionList);
        }

        public ActionResult OpenCustomerInspectionFormPopup(int id)
        {

            ViewBag.CustId = id;
            string CleanFrequencyName = string.Empty;
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            if (id != 0 && id != null)
            {
                CustomerViewModel = CustomerService.GetCustomerDetailsById(Convert.ToInt32(id));

                var BillSetting = CustomerService.GetBillSettingWithCustomer(Convert.ToInt32(id));
                if (BillSetting != null)
                {
                    ViewBag.ARStatus = ((BillSetting.ARStatus != null && BillSetting.ARStatus != 0) ? getARStatusResonList().FirstOrDefault(x => x.ARStatusReasonListId == BillSetting.ARStatus).Name : string.Empty); //BillSetting.ARStatus;
                }
                var AgingData = AccountReceivableService.AgingDataForCollectionCall(Convert.ToInt32(id));
                if (AgingData != null)
                {
                    ViewBag.AgingData = AgingData;
                }

                if (CustomerViewModel.ContractDetail != null)
                {
                    if (CustomerViewModel.ContractDetail.CleanFrequencyListId > 0)
                    {
                        var CleanFrequencyList = CustomerService.GetCleanFrequencyList().ToList();
                        CleanFrequencyName = CleanFrequencyList.Where(w => w.CleanFrequencyListId == CustomerViewModel.ContractDetail.CleanFrequencyListId).FirstOrDefault().Name;
                    }
                }
            }
            ViewBag.CustomerDetail = CustomerViewModel;

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(Convert.ToInt32(id));
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
            return PartialView("_PartialCustomerInspectionFormPopup", CustomerViewModel);
        }

        public ActionResult CustomerServiceSchedulePopup(int customerId, string regionId, bool isMonthly = false, int dayToAdd = 0, int userId = 0, DateTime? startDate = null, string callBackAction = null, string callBackController = null, string passId = null,int? statusListId=0,int passIdValueOne=0,string passIdTwo=null,int passIdValueTwo=0)
        {
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
            ViewBag.PassIdValueOne = passIdValueOne;
            ViewBag.PassIdTwo = passIdTwo;
            ViewBag.PassIdValueTwo = passIdValueTwo;

            var data = CustomerService.GetCustomerServiceScheduleData(customerId, regionId, dayToAdd, userId, stDate, endDate);
            return PartialView("_PartialCustomerServiceSchedulePopup", data);
        }

        
        public ActionResult AddCRMSchedule(int customerId, DateTime dateTime, string callBackAction = null, string callBackController = null, string passId = null,int? statusListId = 0, int passIdValueOne = 0, string passIdTwo = null, int passIdValueTwo = 0)
        {
            DateTime endDate = dateTime;
            var userId = 0;
            var regionId = SelectedRegionId.ToString();

            ViewBag.StartDate = dateTime;
            ViewBag.EndDate = endDate;
            ViewBag.CustomerId = customerId;
            ViewBag.DaysToAdd = 0;
            var scheduleType = CustomerService.GetScheduleTyoeList();
            
            if (statusListId<=0) 
            {
                statusListId = (int)PurposeForDiffForm.CustomerServiceAndOperations;
            }
            var purposeType = _crmService.GetAllPurposeType(Convert.ToInt32(statusListId));

            var stageStatusType = _crmService.GetAll_CRM_StageStatus();
            var regions = _commonService.GetRegionList();

            var reg = regions.FirstOrDefault(o => o.RegionId == SelectedRegionId);

            var custAddress = string.Empty;
            var custData = CustomerService.GetCustomerDetailsById(customerId);
            if (custData.BillingAddress != null)
            {
                custAddress = custData.BillingAddress.FullAddress;
            }

            ViewBag.StatusListId = statusListId;
            ViewBag.ScheduleType = scheduleType;
            ViewBag.PurposeType = purposeType;
            ViewBag.StageStatusType = stageStatusType;
            ViewBag.RegionName = reg != null ? reg.Name : "";
            ViewBag.RegionList = regions;
            ViewBag.CustAddress = custAddress;
            ViewBag.CallBackAction = callBackAction;
            ViewBag.CallBackController = callBackController;
            ViewBag.PassId = passId;
            ViewBag.PassIdValueOne = passIdValueOne;
            ViewBag.PassIdTwo = passIdTwo;
            ViewBag.PassIdValueTwo = passIdValueTwo;

            var data = CustomerService.GetCRMScheduleData(customerId, dateTime, userId, regionId, endDate);
            return View(data);
        }

        public ActionResult SaveCRMSchedule(CustomerServiceScheduleDataModel model)
        {
            if (model.StartTime != null)
            {
                DateTime stDate = new DateTime();
                if (model.IsAllDay)
                {
                    stDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 8, 0, 0);
                }
                else
                {
                    stDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
                }

                model.StartDate = stDate;
            }
            if (model.EndTime != null)
            {
                DateTime eDate = new DateTime();
                if (model.IsAllDay)
                {
                    eDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 17, 0, 0);
                }
                else
                {
                    eDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);
                }

                model.EndDate = eDate;
            }
            if (model.IsSameAddress)
            {
                var custData = CustomerService.GetCustomerDetailsById(model.ClassId);
                if (custData.BillingAddress != null)
                {
                    model.Location = custData.BillingAddress.FullAddress;
                }

            }
            model.RegionId = SelectedRegionId;

            #region Mapping model.....

            var schedule = new CRM_Schedule();
            schedule.CRM_ScheduleId = model.CRM_ScheduleId;
            schedule.Title = model.Title;
            schedule.Description = model.Description;
            schedule.StartDate = model.StartDate;
            schedule.CRM_AccountFranchiseDetailId = model.CRM_AccountFranchiseDetailId;
            schedule.IsFromOutlook = model.IsFromOutlook;
            schedule.OutlookSyncDate = model.OutlookSyncDate;
            schedule.Location = model.Location;
            schedule.EndDate = model.EndDate;
            schedule.IsAllDay = model.IsAllDay;
            schedule.CRM_ScheduleTypeId = model.CRM_ScheduleTypeId;
            schedule.CRM_StageStatusType = model.CRM_StageStatusType;
            schedule.RegionId = SelectedRegionId;
            schedule.AuthUserLoginId = model.AuthUserLoginId;
            schedule.PurposeId = model.PurposeId;
            schedule.Purpose = model.Purpose;
            schedule.IsActive = true;
            schedule.ClassId = model.ClassId;
            schedule.TypeListId = model.TypeListId;
            #endregion Mapping model.....
            var data = _crmService.SaveCRM_ScheduleData(schedule);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerSchedule(int? customerId, string regionId=null, bool isMonthly = false, int dayToAdd = 0, int userId = 0, DateTime? startDate = null, string callBackAction = null, string callBackController = null, string passId = null, int? statusListId = 0, int passIdValueOne = 0, string passIdTwo = null, int passIdValueTwo = 0)
        {
            if (customerId <= 0 || customerId==null) {
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

            if (callBackAction == null) {
                callBackAction = "CustomerSchedule";
            }
            if (callBackController == null)
            {
                callBackController = "Customer";
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
            ViewBag.PassIdValueOne = passIdValueOne;
            ViewBag.PassIdTwo = passIdTwo;
            ViewBag.PassIdValueTwo = passIdValueTwo;

            var data = CustomerService.GetCustomerServiceScheduleData(customerId, regionId, dayToAdd, userId, stDate, endDate);
           
            return View(data);
        }

    }
}