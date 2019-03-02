namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    using Application.Web.Core;
    using JKApi.Service;
    using MvcBreadCrumbs;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    public class CRMReportController : ViewControllerBase
    {
        #region Life Cycle       

        public CRMReportController(ICommonService commonService)
        {
            this._commonService = commonService;
        }

        #endregion
        public ActionResult Index()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            return View();
        }

        public ActionResult SearchAppointmentResult()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("SearchAppointmentResult", "CRMReport", new { area = "CRM" }), "Search Appointment Result");
            return View();
        }

        public ActionResult SearchAppResultsAnalysis()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("SearchAppResultsAnalysis", "CRMReport", new { area = "CRM" }), "Appointment Results Analysis");
            return View();
        }

        public ActionResult AppointmentStats()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("AppointmentStats", "CRMReport", new { area = "CRM" }), "FV/PD Appointment Stats");
            return View();
        }

        public ActionResult CallBacksPending()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallBacksPending", "CRMReport", new { area = "CRM" }), "Call Backs Pending");
            return View();
        }

        public ActionResult CallLog()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallLog", "CRMReport", new { area = "CRM" }), "Call Log Record");
            return View();
        }

        public ActionResult CallLogSummary()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallLogSummary", "CRMReport", new { area = "CRM" }), "Call Log Summary");
            return View();
        }

        public ActionResult CallStatistics()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallStatistics", "CRMReport", new { area = "CRM" }), "Call Statistics");
            return View();
        }

        public ActionResult ColdCallStatistics()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("ColdCallStatistics", "CRMReport", new { area = "CRM" }), "Cold Call Statistics");
            return View();
        }

        public ActionResult GoalsResultsAnalysis()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("GoalsResultsAnalysis", "CRMReport", new { area = "CRM" }), "Goals Results Analysis");
            return View();
        }

        public ActionResult InactiveAppointments()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("InactiveAppointments", "CRMReport", new { area = "CRM" }), "Inactive Appointments");
            return View();
        }

        public ActionResult LeadsEntered()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("LeadsEntered", "CRMReport", new { area = "CRM" }), "Leads Entered");
            return View();
        }

        public ActionResult LeadSourceAnalysis()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("LeadSourceAnalysis", "CRMReport", new { area = "CRM" }), "Lead Source Analysis");
            return View();
        }

        public ActionResult LeadSourceByBudget()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("LeadSourceByBudget", "CRMReport", new { area = "CRM" }), "Lead Source By Budget");
            return View();
        }

        public ActionResult NotQualified()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("NotQualified", "CRMReport", new { area = "CRM" }), "NotQualified");
            return View();
        }

        public ActionResult OfficePerformance()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("OfficePerformance", "CRMReport", new { area = "CRM" }), "Office Performance Results");
            return View();
        }

        public ActionResult ProposalsDeliveredContract()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("ProposalsDeliveredContract", "CRMReport", new { area = "CRM" }), "Proposals Delivered/Contract Sales");
            return View();
        }

        public ActionResult SalesPortfolioRecap()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("SalesPortfolioRecap", "CRMReport", new { area = "CRM" }), "Sales Portfolio Recap");
            return View();
        }

        public ActionResult CustomerLeadSource()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CustomerLeadSource", "CRMReport", new { area = "CRM" }), "Source Statistics");
            return View();
        }

        public ActionResult TelemarketerProduction()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("TelemarketerProduction", "CRMReport", new { area = "CRM" }), "Telemarketer Production");
            return View();
        }

        public ActionResult ProposalsDeliveredContractSales()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("ProposalsDeliveredContract", "CRMReport", new { area = "CRM" }), "Proposals Delivered/Contract Sales");
            return View();
        }

        public ActionResult MultiTelemarketerProduction()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("MultiTelemarketerProduction", "CRMReport", new { area = "CRM" }), "Telemarketer Production");
            return View();
        }

        #region Report Detail
        public ActionResult AppointmentStatsReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("AppointmentStatsReport", "CRMReport", new { area = "CRM" }), "FV/PD Appointment Stats Report");
            return View();
        }

        public ActionResult CallLogsReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallLogsReport", "CRMReport", new { area = "CRM" }), "Call Log Report");
            return View();
        }

        public ActionResult CallLogSummaryReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallLogSummaryReport", "CRMReport", new { area = "CRM" }), "Call Log Summary Report");
            return View();
        }

        public ActionResult CallStatisticsReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallStatisticsReport", "CRMReport", new { area = "CRM" }), "Call Statistics Report");
            return View();
        }

        public ActionResult LeadSourceAnalReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("LeadSourceAnalReport", "CRMReport", new { area = "CRM" }), "Lead Source Anal Report");
            return View();
        }

        public ActionResult LeadSourceBudgetReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("LeadSourceBudgetReport", "CRMReport", new { area = "CRM" }), "Lead Source Budget Report");
            return View();
        }

        public ActionResult TelemarketerProductionReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("TelemarketerProductionReport", "CRMReport", new { area = "CRM" }), "Telemarketer Production Report");
            return View();
        }

        public ActionResult CustomerLeadSourceReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CustomerLeadSourceReport", "CRMReport", new { area = "CRM" }), "Source Statistics Report");
            return View();
        }

        public ActionResult OfficePerformanceReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("OfficePerformanceReport", "CRMReport", new { area = "CRM" }), "Office Performance Results");
            return View();
        }

        public ActionResult NotQualifiedResult()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("NotQualifiedResult", "CRMReport", new { area = "CRM" }), "Not Qualified Result");
            return View();
        }

        public ActionResult AppResultsAnalysisResult()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("AppResultsAnalysisResult", "CRMReport", new { area = "CRM" }), "Appointment Results Analysis");
            return View();
        }

        public ActionResult CallBacksPendingReport()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "Customer Sales");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMReport", new { area = "CRM" }), "Reports");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("CallBacksPendingReport", "CRMReport", new { area = "CRM" }), "Call Backs Pending");
            return View();
        }

        #endregion

        #region CRM Reports
        public ActionResult CrmReports()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Report";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CrmReports", "CRMReport", new { area = "CRM" }), "CRMReport");
            BreadCrumb.Add(Url.Action("CrmReports", "CRMReport", new { area = "CRM" }), "CRM Reports");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).OrderBy(x => x.Name).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            var Periodlist = _commonService.GetPeriodList().Where(r => r.BillYear >= 2016 && r.BillYear <= DateTime.Now.Year || (r.BillYear == DateTime.Now.Year && r.BillMonth <= DateTime.Now.Month)).Select(x => new { text = x.BillMonth.Value.ToString().Trim() + "/" + x.BillYear.ToString().Trim(), value = x.PeriodId }).ToList();
            ViewBag.periodlist = new SelectList(Periodlist, "value", "text");

            return View();
        }
        #endregion
    }
}