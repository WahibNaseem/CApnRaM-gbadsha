using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKApi.Data.DAL;
using JKViewModels.Management;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using JKApi.Service.Helper.Extension;
using JKApi.Service;
using JKApi.Service.Service.Management;
using Application.Web.Core;
using JKApi.Service.ServiceContract.Management;
using JKApi.Data.DTOObject;
using JKViewModels;
using JKViewModels.Customer;
using MoreLinq;
using JKApi.Service.Service;
using OfficeExcel = Microsoft.Office.Interop.Excel;
using System.IO;
using JKViewModels.Common;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Management", Order = -1)]
    public class ManagementController : ViewControllerBase
    {
        #region Constructor
        ManagementService mngser;

        FranchiseService serv = new FranchiseService();
        

        public ManagementController(IManagementService managementService, ICommonService commonService)
        {
            ManagementService = managementService;
            this._commonService = commonService;
            ViewBag.HMenu = "Management";
        }

        #endregion


        #region Methods

        // GET: Portal/Management
        public ActionResult Index()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("Reports", "Management", new { area = "Portal" }), "Reports");
            DashboardViewModel model = new DashboardViewModel();
            //model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            //model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            var regionId = 0;
            if (SelectedRegionId > 0)
            {
                regionId = SelectedRegionId;
            }
            ViewBag.SelectedRegionId = regionId;
            return View(model);
        }

        #region Chart and Report......

        public ActionResult OfficeOverview()
        {
            ViewBag.CurrentMenu = "OfficeOverview";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("OfficeOverview", "Management", new { area = "Portal" }), "Office Overview");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult RevenueReport()
        {
            ViewBag.CurrentMenu = "RevenueReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("RevenueReport", "Management", new { area = "Portal" }), "Revenue Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult CustomerReport()
        {
            ViewBag.CurrentMenu = "CustomerReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("CustomerReport", "Management", new { area = "Portal" }), "Customer Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult ReportByFranchise()
        {
            DashboardViewModel model = new DashboardViewModel();

            int year = DateTime.Now.Year;
            DateTime fromDate = new DateTime(year, 1, 1);
            DateTime toDate = DateTime.Now;

            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));

            return View(model);
        }

        public ActionResult BillingAccountBreakdownBySizeReport()
        {
            ViewBag.CurrentMenu = "BillingAccountBreakdownBySizeReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("BillingAccountBreakdownBySizeReport", "Management", new { area = "Portal" }), "Billing Account By Size");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        #region dashboard ......
        public ActionResult GetDashboardRevenueChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetDashboardRevenueChart(regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllManagementDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds = null, string flag = null)
        {
            var data = _commonService.GetAllManagementDashboardData(spnStartDate, spnEndDate, monthToAdd, yearToAdd, billMonth, billYear, rowNumber, regionIds, flag);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion
        #region Chart Data.....
        #region office overview report.....
        public ActionResult GetBillingBreakdownBySizeChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetBillingBreakdownBySizeChartData(regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingBreakdownByContractChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetBillingBreakdownByContractChartData(regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAccountTypeWiseChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetAccountTypeWiseChartData(regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllOfficeOverviewData(DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds = null, string flag = null)
        {
            var data = _commonService.GetAllOfficeOverviewData(spnStartDate, spnEndDate, monthToAdd, yearToAdd, billMonth, billYear, rowNumber, regionIds, flag);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion office overview report.....

        #region revenue report.....
        public ActionResult GetRevenueByMonthChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRevenueByMonthChartData(monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRevenueByYearChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRevenueByYearChartData(yearToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRevenueAndPaymentChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRevenueAndPaymentChartData(monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YearlyRevenueReport(int yearToShow, string regionIds)
        {
            var regionlist = _commonService.GetRegionList().Where(o => o.RegionId != 0);
            if (regionIds == null || regionIds == "null" || string.IsNullOrEmpty(regionIds))
            {
                var i = 0;
                foreach (var item in regionlist)
                {
                    if (item.RegionId != 0)
                    {
                        if (i == 0)
                        {
                            regionIds = item.RegionId.ToString();
                        }
                        else
                        {
                            regionIds = regionIds + "," + item.RegionId.ToString();
                        }
                        i++;
                    }
                }
            }

            ViewBag.YearToShow = yearToShow;
            ViewBag.RegionIds = regionIds;
            string[] regionCount = regionIds.Split(',');
            var model = new List<DashboardModel>();
            foreach (string ids in regionCount)
            {
                var id = new DashboardModel
                {
                    RegionId = ids
                };
                model.Add(id);
            }

            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View(model);
        }
        public ActionResult GetRegionWiseYearRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRegionWiseYearRevenueChartData(yearToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MonthlyRevenueReport(int monthToShow, string regionIds)
        {
            var regionlist = _commonService.GetRegionList();
            if (regionIds == null || regionIds == "null" || string.IsNullOrEmpty(regionIds))
            {
                var i = 0;
                foreach (var item in regionlist)
                {
                    if (item.RegionId != 0)
                    {
                        if (i == 0)
                        {
                            regionIds = item.RegionId.ToString();
                        }
                        else
                        {
                            regionIds = regionIds + "," + item.RegionId.ToString();
                        }
                        i++;
                    }
                }
            }

            ViewBag.MonthToShow = monthToShow;
            ViewBag.RegionIds = regionIds;
            string[] regionCount = regionIds.Split(',');
            var model = new List<DashboardModel>();
            foreach (string ids in regionCount)
            {
                var id = new DashboardModel
                {
                    RegionId = ids
                };
                model.Add(id);
            }

            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View(model);
        }
        public ActionResult GetRegionWiseMonthlyRevenueChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRegionWiseMonthlyRevenueChartData(monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGrossAndContractBillingRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetGrossAndContractBillingRevenueChartData(yearToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #region revenue report details.....

        public ActionResult GetMonthlyRevenueDetailsByCustomerData(string flag, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetMonthlyRevenueDetailsByCustomerData(flag, monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMonthlyRevenueDetailsByAccountTypeData(string flag, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetMonthlyRevenueDetailsByAccountTypeData(flag, monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMonthlyRevenueDetailsByAccountTypeAndCustomerData(string flag, int accountTypeListId, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetMonthlyRevenueDetailsByAccountTypeAndCustomerData(flag, accountTypeListId, monthToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegionWiseYearRevenueComparisonChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRegionWiseYearRevenueComparisonChartData(yearToAdd, regionIds, spnStartDate, spnEndDate, billMonth, billYear);

            var chartData = new List<RegionWiseRevenueComparison>();

            if (data != null)
            {
                var years = data.GroupBy(x => x.RangeName).Select(x => x.FirstOrDefault());
                var regions = data.GroupBy(x => x.RegionId).Select(g => g.First()).ToList().Count();
                int i = 0;
                foreach (var item in years)
                {
                    var details = data.Where(o => o.RangeName == item.RangeName).OrderBy(o => o.RangeName);

                    var cl = new RegionWiseRevenueComparison
                    {
                        RegionId = item.RegionId,
                        RegionName = item.RegionName,
                        Year = item.RangeName,
                        ColorCode = item.ColorCode,
                        TotalRegions = regions
                    };
                    List<DashboardModel> detailsList = new List<DashboardModel>();
                    if (details != null)
                    {
                        foreach (var dl in details)
                        {
                            var d = new DashboardModel
                            {
                                TotalRevenue = dl.TotalRevenue,
                                RegionId = dl.RegionId,
                                RegionName = dl.RegionName,
                                RangeName = dl.RangeName,
                                ColorCode = dl.ColorCode
                            };
                            detailsList.Add(d);
                        }
                    }

                    cl.regionWiseYearlyDetails = detailsList;

                    chartData.Add(cl);
                    i++;
                }
            }

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CompareRevenueWithRegion(int yearToShow)
        {
            var regionlist = _commonService.GetRegionList().Where(o => o.RegionId != 0);
            var id = 0;
            if (SelectedRegionId == 0)
            {
                id = 2;
            }
            else
            {
                id = SelectedRegionId;
            }
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.YearToShow = yearToShow;
            ViewBag.SelectedRegionId = id;
            return View();
        }

        #endregion revenue report details.....
        public ActionResult GetMonthlyDataForASelectedYearChartData(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetMonthlyDataForASelectedYearChartData(flag, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion revenue report.....

        #region customer report.....
        public ActionResult GetRevenueWiseTopCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRevenueWiseTopCustomerChartData(recordNumber, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRevenueWiseTopCustomerChartDataDetails(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetRevenueWiseTopCustomerChartDataDetails(flag, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopTenRevenueByAccountTypeChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetTopTenRevenueByAccountTypeChartData(recordNumber, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Chart Details Data......
        public ActionResult MonthlyTopCustomerRevenue(int recordNumber, string regionIds, int dateRangeId)
        {
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            if (regionIds == null || regionIds == "null" || string.IsNullOrEmpty(regionIds))
            {
                var i = 0;
                foreach (var item in regionlist)
                {
                    if (item.RegionId != 0)
                    {
                        if (i == 0)
                        {
                            regionIds = item.RegionId.ToString();
                        }
                        else
                        {
                            regionIds = regionIds + "," + item.RegionId.ToString();
                        }
                        i++;
                    }
                }
            }

            ViewBag.CustomerNumber = recordNumber;
            ViewBag.RegionIds = regionIds;
            ViewBag.DateRangeId = dateRangeId;
            string[] regionCount = regionIds.Split(',');
            var model = new List<DashboardModel>();
            foreach (string ids in regionCount)
            {
                var id = new DashboardModel
                {
                    RegionId = ids
                };
                model.Add(id);
            }
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View(model);
        }

        public ActionResult GetTopCustomerRevenueChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetTopCustomerRevenueChartData(recordNumber, regionIds, spnStartDate, spnEndDate, billMonth, billYear);

            List<CustomertForChart> chartData = new List<CustomertForChart>();

            if (data != null)
            {
                var customerIds = data.GroupBy(x => x.CustomerId).Select(x => x.FirstOrDefault()).OrderBy(o => o.Ranking);
                foreach (var item in customerIds)
                {
                    var details = data.Where(o => o.CustomerId == item.CustomerId);

                    var cl = new CustomertForChart
                    {
                        CustomerId = item.CustomerId,
                        CustomerNo = item.CustomerNo,
                        CustomerName = item.CustomerName
                    };
                    List<CustomertChartDetailsData> detailsList = new List<CustomertChartDetailsData>();
                    if (details != null)
                    {
                        foreach (var dl in details)
                        {
                            var d = new CustomertChartDetailsData
                            {
                                CustomerName = dl.CustomerName,
                                CustomerNo = dl.CustomerNo,
                                ColorCode = dl.ColorCode,
                                PeriodId = dl.PeriodId,
                                RangeName = dl.RangeName,
                                Ranking = dl.Ranking,
                                TotalRevenue = dl.TotalRevenue
                            };
                            detailsList.Add(d);
                        }
                    }

                    cl.customertChartDetailsData = detailsList;

                    chartData.Add(cl);
                }
            }
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewAndCanceledCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetNewAndCanceledCustomerChartData(recordNumber, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
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

        public ActionResult GetTopPaymentWiseCustomer(int? recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetTopPaymentWiseCustomer(recordNumber, regionIds, spnStartDate, spnEndDate, billMonth, billYear);

            List<GrantChartViewModel> chartData = new List<GrantChartViewModel>();

            if (data != null)
            {
                var customerIds = data.GroupBy(x => x.CustomerNo).Select(x => x.FirstOrDefault()).OrderBy(o => o.Ranking);
                foreach (var item in customerIds)
                {
                    var details = data.Where(o => o.CustomerNo == item.CustomerNo);

                    var cl = new GrantChartViewModel
                    {
                        category = item.RangeName
                    };
                    List<GrantChartDetailsViewModel> detailsList = new List<GrantChartDetailsViewModel>();
                    if (details != null)
                    {
                        foreach (var dl in details)
                        {
                            var d = new GrantChartDetailsViewModel
                            {
                                start = dl.Start,
                                duration = dl.Duration,
                                color = dl.ColorCode,
                                task = dl.TotalPayment.ToString()
                            };
                            detailsList.Add(d);
                        }
                    }

                    cl.GrantChartDetailsViewModel = detailsList;

                    chartData.Add(cl);
                }
            }
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }



        #endregion Chart Details Data......


        #endregion customer report.....

        #region management dashboard.....

        public ActionResult GetManagementDashboardData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetManagementDashboardData(regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult GetBillingBreakdownByContractChartDataForDashboard(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = Convert.ToString(SelectedRegionId);
            var data = ManagementService.GetBillingBreakdownByContractChartData(regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Details Chart Data.....
        public ActionResult GetBillingAccountBreakdownData(int flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionId = SelectedRegionId;
            var data = ManagementService.GetBillingAccountBreakdownData(flagId, regionId, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillingAccountBreakdownBySizeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetBillingAccountBreakdownBySizeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingAccountBreakdownByContractData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetBillingAccountBreakdownByContractData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingAccountRevenueByAccountTypeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = ManagementService.GetBillingAccountRevenueByAccountTypeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion Details Chart Data.....

        //Corporate Dues
        // GET: Portal/Management/CorporateDues
        public ActionResult CorporateDues()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CorporateDues", "Management", new { area = "Portal" }), "Corporate Dues");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        public ActionResult ManagementReports()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management Reports");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            var Periodlist = _commonService.GetPeriodList().Where(r => r.BillYear >= 2016 && r.BillYear <= DateTime.Now.Year || (r.BillYear == DateTime.Now.Year && r.BillMonth <= DateTime.Now.Month)).Select(x => new { text = x.BillMonth.Value.ToString().Trim() + "/" + x.BillYear.ToString().Trim(), value = x.PeriodId }).ToList();
            ViewBag.periodlist = new SelectList(Periodlist, "value", "text");

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult CorporationDuesReport(string RegionId, int? Periodid)
        {
            ViewBag.CurrentMenu = "CorporationDuesReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CorporationDuesReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("CorporationDuesReport", "Management", new { area = "Portal" }), "Corporation Dues Report");
            string regId = string.Empty;
            if (string.IsNullOrEmpty(RegionId) || RegionId.ToLower() == "null")
            {
                regId = SelectedRegionId.ToString();
            }
            else
            {
                regId = RegionId;
            }
            int perId = 0;
            var periodData = _commonService.GetPeriodList();
            var preData = new PeriodViewModel();

            if (periodData != null)
            {
                if (Periodid > 0)
                {
                    preData = periodData.Where(o => o.PeriodId == Periodid).FirstOrDefault();
                }
                else
                {
                    preData = periodData.Where(o => o.BillYear == Convert.ToInt32(DateTime.Now.Year) && o.BillMonth == Convert.ToInt32(DateTime.Now.Month)).FirstOrDefault();
                }
            }

            if (Periodid <= 0 || Periodid == null)
            {
                perId = preData.PeriodId;
            }
            else
            {
                perId = Convert.ToInt32(Periodid);
            }
            var data = ManagementService.GetCorporationDuesReportData(regId, perId);
            //var monthdate = ManagementService.getPeriod(perId).ToList();
            ViewBag.billmonth = preData.BillMonth;
            ViewBag.billyear = preData.BillYear;

            ViewBag.InHouseWork = data?.Where(x => x.FeeId == -1).FirstOrDefault()?.Amount;
            ViewBag.FranchiseeSupplies = data?.Where(x => x.FeeId == -2).FirstOrDefault()?.Amount;
            ViewBag.RegionPercent = data?.Where(x => x.FeeId == -3).FirstOrDefault()?.Amount;
            ViewBag.AdministrationFee = data?.Where(x => x.FeeId == -4).FirstOrDefault()?.Amount;

            return View(data.Where(x => x.FeeId >= 0).ToList());
        }

        public ActionResult NegativeDueCollected(string RegionIds, string fromMonth = "", string fromYear = "", string toMonth = "", string toYear = "")
        {
            ViewBag.CurrentMenu = "NegativeDueCollected";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NegativeDueCollected", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("NegativeDueCollected", "Management", new { area = "Portal" }), "Negative Due Collected Report");

            var data = ManagementService.GetNegativeDueCollectedReportData(RegionIds, fromMonth, fromYear, toMonth, toYear);
            //var monthdate = ManagementService.getPeriod(Periodid).ToList();
            ViewBag.billFromDate = (fromMonth.Length == 1 ? "0" + fromMonth : fromMonth) + "/" + fromYear;
            ViewBag.billToDate = (toMonth.Length == 1 ? "0" + toMonth : toMonth) + "/" + toYear;
            return View(data);
        }

        public ActionResult NegativeDueReport()
        {
            ViewBag.HMenu = "AccountsPayable";

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            return View();
        }

        public ActionResult NegativeDueResultData(string regionIds, DateTime from, DateTime to)
        {
            try
            {
                var response = ManagementService.GetNegativeDueReportData(regionIds, from, to);
                return Json(new
                {
                    aadata = response,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DROReport()
        {
            ViewBag.CurrentMenu = "DROReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("DROReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("DROReport", "Management", new { area = "Portal" }), "DRO Report");
            return View();
        }

        public JsonResult CorporationDuesReportdata(string RegionIds, int Periodid)
        {
            //ViewBag.CurrentMenu = "CorporationDuesReport";
            //BreadCrumb.Clear();
            //BreadCrumb.Add(Url.Action("CorporationDuesReport", "Management", new { area = "Portal" }), "Management");
            //BreadCrumb.Add(Url.Action("CorporationDuesReport", "Management", new { area = "Portal" }), "Corporation Dues Report");
            var data = ManagementService.GetCorporationDuesReportData(RegionIds, Periodid);

            var jsonResult = Json(new { aaData = data, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult PercentPaidByDateReport(string[] RegionIds, string MonthYear)
        {
            ViewBag.CurrentMenu = "PercentPaidByDateReport";
            List<GetPercentPaidByDateReport> list = new List<GetPercentPaidByDateReport>();
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PercentPaidByDateReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("PercentPaidByDateReport", "Management", new { area = "Portal" }), "Percent Paid By Date Report");
            if (MonthYear != null)
            {
                string Month = MonthYear.Split('/')[0];
                string Year = MonthYear.Split('/')[1];
                //var startDate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(MonthYear), 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);

                String StattDate = Month + "/01/" + Year;
                string EndDate = Month + "/" + DateTime.DaysInMonth(int.Parse(Year), int.Parse(Month)) + "/" + Year;
                ViewBag.regionList = _claimView.GETCLAIM_REGIONLIST();
                list = ManagementService.GetPercentageDailyReportbyMonthly(RegionIds, StattDate, EndDate);
                //list = ManagementService.GetPercentageDailyReportbyMonthly(RegionIds, startDate.ToString("MM/dd/YYYY"), endDate.ToString("MM/dd/YYYY"));
            }

            var regionlistDDL = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlistForddl = new SelectList(regionlistDDL, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.MonthYear = MonthYear;
            return View(list);
        }

        public ActionResult FranchiessSupplyRecap()
        {
            ViewBag.CurrentMenu = "FranchiessSupplyRecap";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiessSupplyRecap", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("FranchiessSupplyRecap", "Management", new { area = "Portal" }), "Franchiess Supply Recap Report");
            return View();
        }

        //Post:
        [HttpPost]
        public ActionResult CorporateDues(CorporateDuesViewModel cr)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("CorporateDuesList", "Management", new { month = cr.billMonth, year = cr.billYear });
            }
            else
            {
                return View("CorporateDues");
            }
        }

        //GET: Portal/Management/CorporateDuesList
        public ActionResult CorporateDuesList(string month, string year)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = month;
            ViewBag.billYear = year;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("CorporateDuesList", "Management", new { area = "Portal" }), "Corporate Dues List");
            //mngser = new ManagementService();  //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.getCorporateDuesList(month, year);
            return View(data);
        }

        //Deduction
        // GET: Portal/Management/Deductions
        public ActionResult Deductions()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("Deductions", "Management", new { area = "Portal" }), "Deductions");

            //mngser = new ManagementService();  //Sohel: Already injected the service, and had an instance of it;
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        //Post:
        [HttpPost]
        public ActionResult Deductions(DeductionsViewModel dedvm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("DeductionsList", "Management", new { month = dedvm.billMonth, year = dedvm.billYear });
            }
            else
            {
                return View("Deductions");
            }
        }


        //GET: Portal/Management/DeductionsList
        public ActionResult DeductionsList(string month, string year)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = month;
            ViewBag.billYear = year;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("DeductionsList", "Management", new { area = "Portal" }), "Deductions List");
            //mngser = new ManagementService();  //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.getDeductionsList(month, year);
            return View(data);
        }

        //DRO
        // GET: Portal/Management/DRO
        public ActionResult DRO()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("DRO", "Management", new { area = "Portal" }), "DRO");

            //mngser = new ManagementService();  //Sohel: Already injected the service, and had an instance of it;
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        //Post:
        [HttpPost]
        public ActionResult DRO(DeductionsViewModel dedvm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("DroList", "Management", new { month = dedvm.billMonth, year = dedvm.billYear });
            }
            else
            {
                return View("DRO");
            }
        }

        //GET: Portal/Management/DroList
        public ActionResult DroList(string month, string year)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = month;
            ViewBag.billYear = year;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("DroList", "Management", new { area = "Portal" }), "Dro List");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.getDroList(month, year);
            return View(data);
        }

        //STA Recap
        // GET: Portal/Management/starecap
        public ActionResult starecap()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("starecap", "Management", new { area = "Portal" }), "STA Recap");

            //mngser = new ManagementService();
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        //Post: STA Recap
        [HttpPost]
        public ActionResult starecap(StarecapViewModel dedvm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("starecapList", "Management", new { month = dedvm.billMonth, year = dedvm.billYear });
            }
            else
            {
                return View("starecap");
            }
        }

        //STA Recap List
        //GET: Portal/Management/starecapList
        public ActionResult starecapList(string month, string year)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = month;
            ViewBag.billYear = year;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("starecapList", "Management", new { area = "Portal" }), "STA Recap List");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.getstarecapList(month, year);
            return View(data);
        }


        //Monthy Revenue
        // GET: Portal/Management/revenue
        public ActionResult revenue()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("revenue", "Management", new { area = "Portal" }), "Monthy Revenue");

            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        //Post:Monthy Revenue
        [HttpPost]
        public ActionResult revenue(RevenueViewModel dedvm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("revenueList", "Management", new { month = dedvm.billMonth, year = dedvm.billYear });
            }
            else
            {
                return View("revenue");
            }
        }

        //Monthy Revenue List
        //GET: Portal/Management/revenueList
        public ActionResult revenueList(int month, int year)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = month;
            ViewBag.billYear = year;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("revenueList", "Management", new { area = "Portal" }), "Monthy Revenue List");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.GetRevenueList_All(month, year);
            return View(data);
        }

        //Tax Liability 
        // GET: Portal/Management/taxliability
        public ActionResult taxliability()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("taxliability", "Management", new { area = "Portal" }), "Tax Liability");

            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            ViewBag.billMonthsList = ManagementService.GetMonthListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();

            ViewBag.billYearList = ManagementService.GetYearListItem().Select(k => new SelectListItem { Text = k.Name, Value = k.Id.ToString() }).ToList();
            return View();
        }

        //Post:Tax Liability  List
        [HttpPost]
        public ActionResult taxliability(TaxLiabilityViewModel dedvm)
        {
            return RedirectToAction("taxliabilityList", new { billmonth = Convert.ToInt32(dedvm.billMonth), billyear = Convert.ToInt32(dedvm.billYear), userid = dedvm.userid });

        }

        //Tax Liability  List
        //GET: Portal/Management/taxliabilityList
        public ActionResult taxliabilityList(int billmonth, int billyear, int userid)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            ViewBag.billMonth = billmonth;
            ViewBag.billYear = billyear;
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("taxliabilityList", "Management", new { area = "Portal" }), "Tax Liability  List");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            var data = ManagementService.GetMonthlyTax_All(billmonth, billyear, userid);
            return View(data);
        }

        //Lease Report
        // GET: Portal/Management/leasepayment
        public ActionResult leasepayment()
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("leasepayment", "Management", new { area = "Portal" }), "Lease Payment");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it (No further use);

            return View();
        }

        //Post:Lease Report  List
        [HttpPost]
        public ActionResult leasepayment(LeasepaymentViewModel dedvm)
        {
            if (dedvm.Searchby == "-1")
            {
                dedvm.SearchValue = "1"; dedvm.Searchby = "1"; dedvm.TransactionDate = "1"; dedvm.TransactionDateTo = "1";
                return RedirectToAction("leasepaymentList", new { searchby = Convert.ToInt32(dedvm.SearchValue), searchvalue = dedvm.Searchby, transactionstartdate = dedvm.TransactionDateTo, transactionenddate = dedvm.TransactionDate });
            }
            return RedirectToAction("leasepaymentList", new { searchby = dedvm.Searchby, searchvalue = dedvm.SearchValue, transactionstartdate = dedvm.TransactionDateTo, transactionenddate = dedvm.TransactionDate });
        }

        //Lease Report List 
        //GET: Portal/Management/leasepayment
        [HttpGet]
        [AllowAnonymous]
        public ActionResult leasepaymentList(int searchby, string searchvalue, string transactionstartdate, string transactionenddate)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("leasepaymentList", "Management", new { area = "Portal" }), "Lease Report List");
            //mngser = new ManagementService(); //Sohel: Already injected the service, and had an instance of it;
            if (searchby == 1 && searchvalue == "1")
            {
                searchby = 0; searchvalue = ""; transactionstartdate = ""; transactionenddate = "";

            }
            var data = ManagementService.getLeasePaymentsList(searchby, searchvalue, transactionstartdate, transactionenddate);
            //LeaseListViewModel

            //var data = mngser.getstarecapList(month, year);
            return View(data);
        }

        //Leases
        //GET: Portal/Management/leaseList
        public ActionResult leaseList(string id)
        {
            ViewBag.CurrentMenu = "ManagementReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("leaseList", "Management", new { area = "Portal" }), "Leases");
            //mngser = new ManagementService();   //Sohel: Already injected the service, and had an instance of it (No further use of it);
            //var data = mngser.portal_spget_F_LeaseHistory(-1);
            FranchiseLeaseHistoryPayeeCollection FranchiseLeaseHistoryPayeeCollection = new FranchiseLeaseHistoryPayeeCollection();
            FranchiseLeaseHistoryPayeeCollection.portal_spget_F_LeaseHistory_all_Result = new List<portal_spget_F_LeaseHistory_all_Result>();
            FranchiseLeaseHistoryPayeeCollection.portal_spget_F_LeaseHistory_Specific_Result = new List<portal_spget_F_LeaseHistory_Specific_Result>();

            if (!string.IsNullOrEmpty(id))
            {

                var result = ManagementService.GetLeaseList_All(id).ToList();
                FranchiseLeaseHistoryPayeeCollection.portal_spget_F_LeaseHistory_all_Result = result;
                if (result != null && result.Count > 0)
                {
                    FranchiseLeaseHistoryPayeeCollection.PaymentTax = Convert.ToString(result.Select(one => one.Payment_Tax).Sum());
                    FranchiseLeaseHistoryPayeeCollection.PaymentAmount = Convert.ToString(result.Select(one => one.Payment_Amount).Sum());
                }

                return View(FranchiseLeaseHistoryPayeeCollection);
            }
            else
            {

                var result = ManagementService.GetLeaseList_Specific().ToList();
                FranchiseLeaseHistoryPayeeCollection.portal_spget_F_LeaseHistory_Specific_Result = result;

                if (result != null && result.Count > 0)
                {
                    FranchiseLeaseHistoryPayeeCollection.PaymentTax = Convert.ToString(result.Select(one => one.Payment_Tax).Sum());
                    FranchiseLeaseHistoryPayeeCollection.PaymentAmount = Convert.ToString(result.Select(one => one.Payment_Amount).Sum());
                }
                return View(FranchiseLeaseHistoryPayeeCollection);

            }


        }

        //BankRegister
        //GET: Portal/Management/BankRegister
        public ActionResult BankRegister()
        {
            ViewBag.CurrentMenu = "BankRegister";
            BreadCrumb.Clear();

            BreadCrumb.Add(Url.Action("BankRegister", "Management", new { area = "Portal" }), "BankRegister");
            //mngser = new ManagementService();  //Sohel: Already injected the service, and had an instance of it (No further use of it);

            return View();
        }

        public ActionResult ObligationReport()
        {
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            //List<ObligationReportViewModel> ObligationList = ManagementService.getObligationList();
            return View();
        }

        [HttpGet]
        public JsonResult GetObligationReport(string regionId)
        {
            return Json(ManagementService.getObligationList(regionId), JsonRequestBehavior.AllowGet);
        }

        #region eDocs

        public ActionResult CustomerDoc()
        {
            ViewBag.CurrentMenu = "ManagementReports";

            return View();
        }
        public ActionResult FranDoc()
        {
            return View();
        }

        public ActionResult _NewAccount(bool print = false)
        {
            ViewBag.IsPrintView = print;

            //List<FranchiseeReportDetailsViewModel> vms = new List<FranchiseeReportDetailsViewModel>();
            //foreach (string a in arrids)
            //{
            //    vms.Add(AccountPayableService.GetFranchiseeReportDetails(int.Parse(a.Trim())));
            //}

            return PartialView("_NewAccount");
        }

        public ActionResult _FinderFeeForm(bool print = false)
        {
            ViewBag.IsPrintView = print;
            return PartialView("_AccountFinderFeeForm");
        }

        #endregion




        #endregion

        #region :: Report Action ::

        public ActionResult FranchiseeRevenues()
        {
            ViewBag.CurrentMenu = "FranchiseeRevenues";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeRevenues", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("FranchiseeRevenues", "Management", new { area = "Portal" }), "Franchisee Revenues");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            int pId = _commonService.GetPeriodList().SingleOrDefault(a => a.BillMonth == DateTime.Now.Month && a.BillYear == DateTime.Now.Year).PeriodId;

            ViewBag.PeriodList = new SelectList(_commonService.GetPeriodList().Where(k => k.PeriodId <= pId).OrderByDescending(p => p.PeriodId), "PeriodId", "Period", _commonService.GetPeriodList().LastOrDefault().PeriodId);

            return View();
        }

        public ActionResult Report1099()
        {
            ViewBag.CurrentMenu = "FranchiseAccount";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Report1099", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("Report1099", "Management", new { area = "Portal" }), "1099 Report");

            return View();
        }

        public ActionResult CustomerRevenues()
        {
            ViewBag.CurrentMenu = "CustomerRevenues";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerRevenues", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("CustomerRevenues", "Management", new { area = "Portal" }), "Customer Revenues");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            int pId = _commonService.GetPeriodList().SingleOrDefault(a => a.BillMonth == DateTime.Now.Month && a.BillYear == DateTime.Now.Year).PeriodId;

            ViewBag.PeriodList = new SelectList(_commonService.GetPeriodList().Where(k => k.PeriodId <= pId).OrderByDescending(p => p.PeriodId), "PeriodId", "Period", _commonService.GetPeriodList().LastOrDefault().PeriodId);

            return View();
        }

        public ActionResult LeaseBillReport()
        {
            ViewBag.CurrentMenu = "LeaseBillReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LeaseBillReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("LeaseBillReport", "Management", new { area = "Portal" }), "Lease Bill Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }
        public ActionResult FinderFeeReport(string startDate = "", string endDate = "", string RegionIds = "", string ftype = "")
        {
            ViewBag.CurrentMenu = "FinderFeeReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FinderFeeReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("FinderFeeReport", "Management", new { area = "Portal" }), "Finder Fee Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);

            ViewBag.selectedRegionId = SelectedRegionId;


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

            ViewBag.ftype = ftype;
            ViewBag.fstartDate = startDate;
            ViewBag.fendDate = endDate;
            ViewBag.fregionIds = RegionIds;
            return View(retVal);
            //return View();
        }
        public ActionResult DeductionsReport()
        {
            ViewBag.CurrentMenu = "DeductionsReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("DeductionsReport", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("DeductionsReport", "Management", new { area = "Portal" }), "Deductions Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            int pId = _commonService.GetPeriodList().SingleOrDefault(a => a.BillMonth == DateTime.Now.Month && a.BillYear == DateTime.Now.Year).PeriodId;
            ViewBag.PeriodList = new SelectList(_commonService.GetPeriodList().Where(k => k.PeriodId <= pId).OrderByDescending(p => p.PeriodId), "PeriodId", "Period", _commonService.GetPeriodList().LastOrDefault().PeriodId);
            return View();
        }

        public ActionResult DynamicReport()
        {
            ViewBag.CurrentMenu = "DynamicReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManagementReports", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("DynamicReport", "Management", new { area = "Portal" }), "Dynamic Report");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            var reportList = _commonService.GetAllDynamicReportList();

            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.ReportList = reportList;
            return View();
        }

        public ActionResult GetColumList(int reportId)
        {
            var data = _commonService.GetDynamicReportColumnList(reportId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportExcel(string columnList, int reportId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear, int? rowNumber)
        {
            DataSet ds = new DataSet();
            var name = string.Empty;
            var reportDetails = _commonService.GetAllDynamicReportDetails(reportId);
            if (reportDetails != null)
            {
                if (string.IsNullOrEmpty(regionIds) || regionIds.ToLower() == "null")
                {
                    if (SelectedRegionId > 0)
                    {
                        regionIds = SelectedRegionId.ToString();
                    }
                    else {
                        regionIds = "0";
                    }
                }

                DataTable dt = _commonService.GetDynamicReportData(columnList, reportDetails.ProcedureName, regionIds, spnStartDate, spnEndDate, billMonth, billYear, rowNumber);
                if (dt != null)
                {
                    ds.Tables.Add(dt);
                }
                name = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                ExportDataSetToExcel(ds, name, reportDetails.ReportName);
                name = name + ".xlsx";
            }

            var hasPath = "No"; //need to check path exist in server or not?
            var relativePath = "~/Upload/DynamicReport/"+ name;
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                hasPath = "Yes";
            }

            return Json(new { name= name,hasPath=hasPath } , JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Upload/DynamicReport/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }

        private string ExportDataSetToExcel(DataSet ds, string lseData, string reportName)
        {
            int inHeaderLength = 3, inColumn = 0, inRow = 0;
            System.Reflection.Missing Default = System.Reflection.Missing.Value;
            //Create Excel File
            System.IO.Directory.CreateDirectory(Server.MapPath("~/Upload/DynamicReport/"));
            var strPath = Path.Combine(Server.MapPath("~/Upload/DynamicReport/"), lseData + ".xlsx");
            OfficeExcel.Application excelApp = new OfficeExcel.Application();
            OfficeExcel.Workbook excelWorkBook = excelApp.Workbooks.Add(1);
            foreach (DataTable dtbl in ds.Tables)
            {
                //Create Excel WorkSheet
                OfficeExcel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add(Default, excelWorkBook.Sheets[excelWorkBook.Sheets.Count], 1, Default);
                excelWorkSheet.Name = dtbl.TableName;//Name worksheet

                var clId = 0;
                //Write Column Name
                for (int i = 0; i < dtbl.Columns.Count; i++)
                {
                    clId += 1;
                    excelWorkSheet.Cells[inHeaderLength + 1, i + 1] = dtbl.Columns[i].ColumnName;
                }

                //Write Rows
                for (int m = 0; m < dtbl.Rows.Count; m++)
                {
                    for (int n = 0; n < dtbl.Columns.Count; n++)
                    {
                        inColumn = n + 1;
                        inRow = inHeaderLength + 2 + m;
                        excelWorkSheet.Cells[inRow, inColumn] = dtbl.Rows[m].ItemArray[n].ToString();
                        if (m % 2 == 0)
                            excelWorkSheet.get_Range("A" + inRow.ToString(), GetExcelColumnName(inColumn) + inRow.ToString()).Interior.Color = System.Drawing.ColorTranslator.FromHtml("#FCE4D6");
                    }
                }
                if (clId == 0)
                {
                    clId = 3;
                }
                ////Excel Header
                OfficeExcel.Range cellRang = excelWorkSheet.get_Range("A1", GetExcelColumnName(clId) + "3");
                cellRang.Merge(false);
                cellRang.Interior.Color = System.Drawing.Color.White;
                cellRang.Font.Color = System.Drawing.Color.Gray;
                cellRang.HorizontalAlignment = OfficeExcel.XlHAlign.xlHAlignCenter;
                cellRang.VerticalAlignment = OfficeExcel.XlVAlign.xlVAlignCenter;
                cellRang.Font.Size = 16;
                excelWorkSheet.Cells[1, 1] = reportName;

                //Style table column names
                cellRang = excelWorkSheet.get_Range("A4", GetExcelColumnName(clId) + "4");
                cellRang.Font.Bold = true;
                cellRang.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                cellRang.Interior.Color = System.Drawing.ColorTranslator.FromHtml("#ED7D31");

                //Auto fit columns
                excelWorkSheet.Columns.AutoFit();
            }

            //Delete First Page
            excelApp.DisplayAlerts = false;
            Microsoft.Office.Interop.Excel.Worksheet lastWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelWorkBook.Worksheets[1];
            lastWorkSheet.Delete();
            excelApp.DisplayAlerts = true;

            //Set Defualt Page
            (excelWorkBook.Sheets[1] as OfficeExcel._Worksheet).Activate();

            excelWorkBook.SaveAs(strPath, Default, Default, Default, false, Default, OfficeExcel.XlSaveAsAccessMode.xlNoChange, Default, Default, Default, Default, Default);
            excelWorkBook.Close();
            excelApp.Quit();
            return strPath;


        }
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
        #endregion
    }
}