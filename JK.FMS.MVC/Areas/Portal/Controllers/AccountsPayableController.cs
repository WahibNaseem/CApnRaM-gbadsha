using Application.Web.Core;
using MvcBreadCrumbs;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.end;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using JKApi.Data.DAL;
using JKApi.Service.AccountReceivable;
using JKApi.Service.AccountPayable;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.AccountPayable;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.Service.Administration.Company;
using JKApi.Service.ServiceContract.JKControl;
using JKViewModels.AccountReceivable;
using JKViewModels.AccountsPayable;
using JKApi.Service;
using System.Globalization;
using JKViewModels;
using System.Web.Routing;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.Service.Company;
using JKApi.Service.ServiceContract.Franchisee;
using JKViewModels.Franchisee;
using JKViewModels.Franchise;
using System.Configuration;
using AjaxForm.Serialization;



namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Portal", Order = 0)]
    public class AccountsPayableController : ViewControllerBase
    {

        public AccountsPayableController(IAccountReceivableService _accountreceivableservice, IAccountPayableService _accountpayableservice, ICustomerService _customerService,
            ICompanyService companyService, ICommonService commonService, IFranchiseeService _franchiseeService, JKApi.Service.ServiceContract.Management.IManagementService _managementService)

        {
            AccountReceivableService = _accountreceivableservice;
            AccountPayableService = _accountpayableservice;
            CustomerService = _customerService;
            _companyService = companyService;
            _commonService = commonService;
            ManagementService = _managementService;
            FranchiseeService = _franchiseeService;
            ViewBag.HMenu = "AccountsPayable";
        }
        // GET: Portal/AccountsPayable
        public ActionResult Index()
        {
            BreadCrumb.Add(Url.Action("Index", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            //DashboardViewModel model = new DashboardViewModel();
            //model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            //model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View();
        }

        //Generate Checks 
        // GET: Portal/AccountsPayable/CheckGenerate
        public ActionResult CheckGenerate()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CheckGenerate", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("CheckGenerate", "AccountsPayable", new { area = "Portal" }), "Generate Checks");
            return View();
        }

        //Manual Check
        // GET: Portal/AccountsPayable/checkmanual
        public ActionResult checkmanual()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("checkmanual", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("checkmanual", "AccountsPayable", new { area = "Portal" }), "Manual Check");
            return View();
        }

        //Miscellenous Entry 
        // GET: Portal/AccountsPayable/registermaintenance
        public ActionResult registermaintenance()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("registermaintenance", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("registermaintenance", "AccountsPayable", new { area = "Portal" }), "Miscellenous Entry");
            return View();
        }

        //otherDeposits
        // GET: Portal/AccountsPayable/otherDeposits
        public ActionResult otherDeposits()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("otherDeposits", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("otherDeposits", "AccountsPayable", new { area = "Portal" }), "Other Deposits");
            return View();
        }

        //Deposit Remittance
        // GET: Portal/AccountsPayable/manualdeposit
        public ActionResult manualdeposit()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("manualdeposit", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("manualdeposit", "AccountsPayable", new { area = "Portal" }), "Deposit Remittance");
            return View();
        }

        //SearchRegister
        // GET: Portal/AccountsPayable/searchRegister
        public ActionResult searchRegister()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("searchRegister", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("searchRegister", "AccountsPayable", new { area = "Portal" }), "Search Register");
            return View();
        }

        //searchRegisterList
        // GET: Portal/AccountsPayable/searchRegisterList
        public ActionResult searchRegisterList()
        {
            ViewBag.CurrentMenu = "AccountsPayable";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("searchRegisterList", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("searchRegisterList", "AccountsPayable", new { area = "Portal" }), "Check Register");
            return View();
        }

        //TransactionsList
        public ActionResult TransactionList()
        {
            return View();
        }

        //Transaction Detail
        public ActionResult TransactionDetail()
        {
            return View();
        }

        public ActionResult WriteCheck()
        {
            ViewBag.CurrentMenu = "AccountsPayableReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("WriteCheck", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("WriteCheck", "AccountsPayable", new { area = "Portal" }), "PayBill");
            BreadCrumb.Add(Url.Action("WriteCheck", "AccountsPayable", new { area = "Portal" }), "Write Check");
            ViewBag.hasPendingChecks = 0;

            var CheckTypeList = CustomerService.GetManualCheckBookTransactionTypeList();
            ViewBag.CheckTypeList = new SelectList(CheckTypeList, "CheckBookTransactionTypeListId", "Name");

            var BankList = CustomerService.GetBanksForRegion();
            ViewBag.BankList = new SelectList(BankList, "BankId", "Name");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            List<portal_spGet_AP_PendingUnprintedManualCheckList_Result> lst = _companyService.GetPendingUnprintedManualCheckList(SelectedRegionId, 20);
            if (lst.Count > 0)
            {
                ViewBag.hasPendingChecks = 1;
            }

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        [HttpGet]
        public JsonResult UnprintedManualCheckList()
        {
            List<portal_spGet_C_UnprintedManualCheckList_Result> lst = _companyService.GetUnprintedManualCheckList();

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private int InsertAPBillForManualCheck(FormCollection frm)
        {
            int intRes = 0;
            decimal decRes = 0;
            DateTime dateRes = DateTime.Now;

            int regionId = int.TryParse(frm["regionId"], out intRes) ? intRes : SelectedRegionId;
            int typeListId = int.TryParse(frm["typeListId"], out intRes) ? intRes : 0;
            int classId = int.TryParse(frm["classId"], out intRes) ? intRes : 0;
            int addressId = int.TryParse(frm["addressId"], out intRes) ? intRes : 0;
            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;
            int checkTypeListId = int.TryParse(frm["CheckTypeList"], out intRes) ? intRes : 0;
            decimal amount = decimal.TryParse(frm["txtAmount"], out decRes) ? decRes : 0.00M;
            string memo = frm["txtMemo"];
            DateTime? date = DateTime.TryParse(frm["dtCheckDate"], out dateRes) ? (DateTime?)dateRes : null;

            int billMonth = 0;
            int billYear = 0;

            if (date != null)
            {
                billMonth = date.Value.Month;
                billYear = date.Value.Year;
            }

            ManualCheckViewModel mcvm = new ManualCheckViewModel();
            mcvm.RegionId = regionId;
            mcvm.TypeListId = typeListId;
            mcvm.ClassId = classId;
            mcvm.BankId = bankId;
            mcvm.AddressId = addressId;
            mcvm.Amount = amount;
            mcvm.BillMonth = billMonth;
            mcvm.BillYear = billYear;
            mcvm.CheckDate = date;
            mcvm.Memo = memo;
            mcvm.CreatedBy = this.LoginUserId;
            mcvm.CreatedDate = DateTime.Now;

            int manualCheckId = _companyService.InsertManualCheck(mcvm);

            JKViewModels.AccountsPayable.APBillTransactionViewModel vm = new JKViewModels.AccountsPayable.APBillTransactionViewModel();

            vm.RegionId = regionId;
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;

            JKViewModels.AccountsPayable.APBillViewModel apbvm = new JKViewModels.AccountsPayable.APBillViewModel();

            apbvm.TypeListId = typeListId;
            apbvm.ClassId = classId;
            apbvm.CheckBookTransactionTypeListId = checkTypeListId;
            apbvm.IsManual = true;
            apbvm.BillMonth = billMonth;
            apbvm.BillYear = billYear;
            apbvm.CheckAmount = amount;
            apbvm.FranchiseeReportId = -1;
            apbvm.ManualCheckId = manualCheckId;
            vm.APBill = apbvm;

            int apBillId = AccountPayableService.InsertAPBillTransaction(vm);


            //AccountPayableService.InsertFranchiseeManualTrasactionFromWriteCheck((DateTime)date, classId, typeListId, amount, memo, checkTypeListId);

            return apBillId;
        }

        [HttpPost]
        public JsonResult InsertManualCheckAndPrint(FormCollection frm)
        {
            int intRes = 0;
            DateTime dateRes = new DateTime();

            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;
            int apBillId = InsertAPBillForManualCheck(frm);
            //DateTime trxDate = DateTime.Now; // DateTime.TryParse(frm["dtCheckDate"], out dateRes) ? dateRes : DateTime.Now;
            DateTime trxDate = DateTime.TryParse(frm["dtCheckDate"], out dateRes) ? dateRes : DateTime.Now;

            int checkBookId = AccountPayableService.InsertCheckBookFromAPBill(apBillId, bankId, trxDate);
            UpdatePendingManualCheckTransactionStatus(SelectedRegionId);
            AccountPayableService.InsertFranchiseeManualTrasactionFromWriteCheck(new List<int>() { checkBookId });

            string retPath = Url.Action("Check", "Company", new { area = "Portal", ids = string.Format("{0}", checkBookId) });
            return Json(retPath, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult InsertManualCheck(FormCollection frm)
        {
            int intRes = 0;

            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;

            InsertAPBillForManualCheck(frm);

            return RedirectToAction("WriteCheck", "AccountsPayable", new { area = "Portal" });
        }

        public ActionResult RunPaymentCheck(int CheckType = 0)
        {
            ViewBag.CurrentMenu = "AccountsPayableReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("RunPaymentCheck", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("RunPaymentCheck", "AccountsPayable", new { area = "Portal" }), "PayBill");
            BreadCrumb.Add(Url.Action("RunPaymentCheck", "AccountsPayable", new { area = "Portal" }), "Run Payment Check");
            ViewBag.HasChecks = 0;

            var regionlist = _commonService.GetRegionList().Where(o => o.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            //ViewBag.CheckTypeList = new SelectList(AccountPayableService.GetAll_CheckTypeList().Where(o => (o.IsManual != true) && (o.IsSystemGenerated != true)), "CheckBookTransactionTypeListId", "Name", CheckType);
            ViewBag.CheckTypeList = new SelectList(AccountPayableService.GetAll_CheckTypeList().Where(o => (o.IsSystemGenerated == true)), "CheckBookTransactionTypeListId", "Name", CheckType);

            var BankList = CustomerService.GetBanksForRegion();
            ViewBag.BankList = new SelectList(BankList, "BankId", "Name");

            var lst = AccountPayableService.GetPendingAPBillListForCheckType(SelectedRegionId, CheckType, 20, "");
            List<int> checkBookIds = new List<int>();


            foreach (var trxApBill in lst)
            {
                if (trxApBill.CheckBookId != -1)
                {
                    checkBookIds.Add((int)trxApBill.CheckBookId);
                }
            }

            var pendingChecksLst = AccountPayableService.GetPendingAPBillListForCheckType(SelectedRegionId, 0, 20, "");
            if (pendingChecksLst.Count > 0) {
                ViewBag.HasChecks = 1;
            }
           

            return View();
        }
        public JsonResult RunPaymentCheckRegionInfo(int regionId)
        {
            var region = _commonService.GetRegionList().Where(o => o.RegionId == regionId).FirstOrDefault();
            if (region == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var regionName = region.Displayname;
            var bankList = CustomerService.GetBanksForRegion(regionId).Select(b => new
            {
                BankId = b.BankId,
                BankName = b.Name,
                Balance = _companyService.GetCurrentBankBalance(b.BankId)
            });

            return Json(new
            {
                RegionName = regionName,
                BankList = bankList
            }, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetMonthsList(int selectedMonth = 0)
        {
            List<SelectListItem> monthsList = Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
            { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            return new SelectList(monthsList, "Value", "Text", selectedMonth.ToString());
        }

        private SelectList GetYearsList(int selectedYear = 0)
        {
            // temp. todo: get valid years by invoice in the future.
            var yearList = new List<int>();
            var maxPossibleDate = DateTime.Today.AddMonths(1);
            for (int year = 2016; year <= maxPossibleDate.Year; year++)
                yearList.Add(year);
            yearList.Reverse();

            List<SelectListItem> billYearList = new List<SelectListItem>();
            foreach (var y in yearList) { billYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() }); }

            return new SelectList(billYearList, "Value", "Text", selectedYear.ToString());
        }

        #region Chargeback

        [HttpGet]
        public ActionResult FranchiseeChargeback()
        {
            ViewBag.CurrentMenu = "AccountsPayablePay";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeChargeback", "AccountsPayable", new { area = "Portal" }), "Chargeback");
            BreadCrumb.Add(Url.Action("FranchiseeChargeback", "AccountsPayable", new { area = "Portal" }), "Franchisee Chargeback");

            ViewBag.OptionList = new SelectList(AccountPayableService.GetAll_SearchDateList(), "SearchDateListId", "Name");
            ViewBag.BillMonthList = GetMonthsList();
            ViewBag.BillYearList = GetYearsList();

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.BillMonthYears = new SelectList(GetAvailableChargeBackPeriods(SelectedRegionId.ToString()), "PeriodId", "Period");

            return View();
        }

        [HttpGet]
        public ActionResult FranchiseeChargebackList()
        {
            ViewBag.CurrentMenu = "AccountsPayablePay";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeChargebackList", "AccountsPayable", new { area = "Portal" }), "Chargeback");
            BreadCrumb.Add(Url.Action("FranchiseeChargebackList", "AccountsPayable", new { area = "Portal" }), "Chargeback List");
            ViewBag.OptionList = new SelectList(AccountPayableService.GetAll_SearchDateList(), "SearchDateListId", "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.BillMonthYears = new SelectList(GetAvailableChargebackFinalizedPeriods(SelectedRegionId.ToString()), "PeriodId", "Period");

            //ViewBag.BillMonthYears = new SelectList(GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString()), "PeriodId", "Period");

            return View();
        }

        public ActionResult GetFranchiseeWiseChargebackSummaryOrDetailsResult(string RegionIds, bool IsSummaryView, DateTime? spnStartDate, DateTime? spnEndDate, int month = 0, int year = 0, int PeriodId = 0)
        {

            var data = new object();
            List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> lstCBListViewModel = (List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result>)TempData["lstARInvoiceListViewModel"];
            string ProcCBTrx = (string)TempData["ProcessedCBTrx"];

            if (lstCBListViewModel != null)
            {
                data = lstCBListViewModel;
                RegionIds = lstCBListViewModel[0].RegionId.ToString();
                month = (int)lstCBListViewModel[0].BillMonth;
                year = (int)lstCBListViewModel[0].BillYear;
            }


            data = AccountPayableService.GetFranchiseeWiseChargebackSummaryOrDetailsResult(RegionIds, IsSummaryView, spnStartDate, spnEndDate, month, year, ProcCBTrx, PeriodId);

            return Json(new
            {
                aadata = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FranchiseeChargebackListPrint(string RegionIds, bool IsSummaryView, DateTime? spnStartDate, DateTime? spnEndDate, int month = 0, int year = 0, int PeriodId = 0)
        {
            ViewBag.isViewType = (IsSummaryView == true ? 1 : 0);

            var data = new object();
            List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> lstCBListViewModel = (List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result>)TempData["lstARInvoiceListViewModel"];
            string ProcCBTrx = (string)TempData["ProcessedCBTrx"];

            if (lstCBListViewModel != null)
            {
                data = lstCBListViewModel;
                RegionIds = lstCBListViewModel[0].RegionId.ToString();
                month = (int)lstCBListViewModel[0].BillMonth;
                year = (int)lstCBListViewModel[0].BillYear;
            }

            data = AccountPayableService.GetFranchiseeWiseChargebackSummaryOrDetailsResult(RegionIds, IsSummaryView, spnStartDate, spnEndDate, month, year, ProcCBTrx, PeriodId);

            if (data != null)
            {
                var monthdate = ManagementService.getPeriod(PeriodId).ToList();
                ViewBag.billmonth = monthdate.FirstOrDefault().BillMonth;
                ViewBag.billyear = monthdate.FirstOrDefault().BillYear;

                ViewBag.RemitTo = CustomerService.GetRemitToForRegion(SelectedRegionId);
                ViewBag.SDate = spnStartDate;
                ViewBag.EDate = spnEndDate;



                string HTMLContent = string.Empty;
                HTMLContent += RenderPartialViewToString("_FranchiseeChargebackListPrint", data);

                var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HTMLContent));

                return Json(lsePath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult ChargeBackDataTable(int? rgId = null, string d = "")
        {
            try
            {
                var charges = AccountPayableService.GetChargeBackList(rgId);

                if (SelectedRegionId > 0 && rgId == 0)
                {
                    var regionIds = _commonService.GetRegionList().Select(r => r.RegionId).ToList();

                    if (regionIds.Any())
                    {
                        charges = (from i in charges
                                   join id in regionIds on i.RegionId equals id
                                   select i).ToList();
                    }
                }

                DateTime sDate;
                DateTime eDate;
                if (!string.IsNullOrEmpty(d) && d.Contains("-") && DateTime.TryParseExact(d.Split('-')[0], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out sDate)
                    && DateTime.TryParseExact(d.Split('-')[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eDate) && eDate > sDate)
                {
                    charges = charges.Where(o => o.CreatedDate.HasValue && o.CreatedDate.Value.Date >= sDate && o.CreatedDate.Value.Date <= eDate).ToList();
                }

                var result = from f in charges
                             select new
                             {
                                 f.FranchiseeNo,
                                 InvoiceDate = f.InvoiceDate != null ? f.InvoiceDate : null,
                                 FranchiseeName = f.Name,
                                 InvoiceNo = f.InvoiceNo != null ? f.InvoiceNo : string.Empty,
                                 InvoiceAmount = f.InvoiceAmount != null ? String.Format("{0:C}", f.InvoiceAmount) : "$0.00",
                                 InvoiceBalance = f.InvoiceBalance != null ? String.Format("{0:C}", f.InvoiceBalance) : "$0.00",
                                 ChargeBackAmount = f.ChargeBackAmount != null ? String.Format("{0:C}", f.ChargeBackAmount) : "$0.00",
                                 ChargeBackPeriod = f.CreatedDate != null ? f.CreatedDate : null
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

        [HttpGet]
        public JsonResult GetTurnAroundSummaryData(DateTime paymentDate, string regionIds = null)
        {
            regionIds = regionIds == "null" ? null : regionIds;
            //var lstAPTurnAroundSummaryViewModel = AccountPayableService.GetTurnAroundSummaryList(regionIds); 
            var lstAPTurnAroundSummaryViewModel = AccountPayableService.GetTurnAroundSummaryList(paymentDate, regionIds);
            var lstAPTurnAroundDetailViewModel = AccountPayableService.GetTurnAroundDetailsList(paymentDate, regionIds);

            //TempData["lstAPTurnAroundSummaryViewModel"] = lstARInvoiceDetailsViewModel;

            return Json(new
            {
                summaryData = lstAPTurnAroundSummaryViewModel,
                detailsData = lstAPTurnAroundDetailViewModel,
                success = true
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult SaveTurnAround(FormCollection frm)
        {

            FranchiseeChargebackTransactionViewModel vm = new FranchiseeChargebackTransactionViewModel();
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;

            //string SummaryView = "1";
            string ids = frm["TASelectedIds"];
            string periodId = frm["PeriodId"];
            //string selectedView = frm["SelectedView"];
            string regionIds = frm["RegionIds"];
            DateTime PaymentDate = DateTime.Now;
            string TARDate = frm["TrxDate"];
            PaymentDate = Convert.ToDateTime(TARDate);
            int TARperiodId = 0;

            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //List<string> SelectedTARtransactions = new List<string>(arrids);

            List<portal_spGet_AP_TurnAroundDetailsList_Result> lstTurnAroundtransactionList = new List<portal_spGet_AP_TurnAroundDetailsList_Result>();

            lstTurnAroundtransactionList = AccountPayableService.GetTurnAroundDetailsList(PaymentDate, regionIds);

            //int TARperiodId = Convert.ToInt32(periodId);
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var turnAroundPeriodId = context.Periods.Where(p => p.BillMonth == PaymentDate.Month && p.BillYear == PaymentDate.Year).FirstOrDefault();
                TARperiodId = turnAroundPeriodId != null ? turnAroundPeriodId.PeriodId : 0;
            }
            

           string ProcessedCBTrx = AccountPayableService.InsertFranchiseeTurnAroundTransaction(lstTurnAroundtransactionList, TARperiodId, PaymentDate);
           return RedirectToAction("TurnaroundProcess", "AccountsPayable", new { area = "Portal" });


            /*
            List<portal_spGet_AP_TurnAroundDetailsList_Result> lstTARtransactionList = new List<portal_spGet_AP_TurnAroundDetailsList_Result>();
            List<portal_spGet_AP_TurnAroundSummaryList_Result> lstTARSummaryList = new List<portal_spGet_AP_TurnAroundSummaryList_Result>();
            List<portal_spGet_AP_TurnAroundDetailsList_Result> lstTARDetailList = new List<portal_spGet_AP_TurnAroundDetailsList_Result>();

            lstTurnAroundtransactionList = AccountPayableService.GetTurnAroundDetailsList(PaymentDate, regionIds);


            if (selectedView == SummaryView)
            {
                lstTARSummaryList = AccountPayableService.GetTurnAroundSummaryList(PaymentDate, regionIds);
                foreach (var TARTRX in lstTARSummaryList)
                {
                    if (SelectedTARtransactions.Contains(TARTRX.FranchiseeId.ToString()))
                    {
                        var list = lstTurnAroundtransactionList.Where(c => c.FranchiseeId == TARTRX.FranchiseeId).ToList();

                        foreach (var item in list)
                        {
                            lstTARtransactionList.Add(item);
                        }

                    }
                }
            }
            else
            {

                foreach (var TARTRX in lstTurnAroundtransactionList)
                {
                    if (SelectedTARtransactions.Contains(TARTRX.PaymentBillingFranchiseeId.ToString()))
                    {
                        lstTARtransactionList.Add(TARTRX);
                    }
                }
            }

          

            int TARperiodId = GetPeriodId(DateTime.Now.Month, DateTime.Now.Year);
            string ProcessedCBTrx = AccountPayableService.InsertFranchiseeTurnAroundTransaction(lstTARtransactionList, TARperiodId);
            return RedirectToAction("TurnAroundList", "AccountsPayable", new { area = "Portal" });
        */

        }


        [HttpGet]
        public JsonResult InvoiceFranchiseeListForChargeback(int periodid, string regionIds = null)
        {
            regionIds = regionIds == "null" ? null : regionIds;
           
            var lstARInvoiceDetailsViewModel = AccountPayableService.GetInvoiceFranchiseeListForChargeback(periodid, regionIds);

            TempData["lstARInvoiceListViewModel"] = lstARInvoiceDetailsViewModel;

            return Json(new
            {
                
                detailsData = lstARInvoiceDetailsViewModel,
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApplyFranchiseeChargeback(FormCollection frm)
        {

            FranchiseeChargebackTransactionViewModel vm = new FranchiseeChargebackTransactionViewModel();
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;

            string ids = frm["CBSelectedIds"];
            string periodId = frm["PeriodId"];
            string regionIds = frm["RegionIds"];

            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> SelectedCBTransactions = new List<string>(arrids);

            Period cbPeriod = new Period();
            cbPeriod = AccountPayableService.GetPeriod(Convert.ToInt32(periodId));
            DateTime periodDate = Convert.ToDateTime(cbPeriod.BillMonth.ToString() + "/01/" +  cbPeriod.BillYear.ToString());
            var firstDayOfMonth = new DateTime(periodDate.Year, periodDate.Month, 1);
            var cbTrxDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> lstARChargebacktransactionList = new List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result>();
            List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> lstARChargebackDetailList = new List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result>();

            lstARChargebackDetailList = AccountPayableService.GetInvoiceFranchiseeListForChargeback(Convert.ToInt32(periodId), regionIds);
            lstARChargebacktransactionList = lstARChargebackDetailList.Where(i => SelectedCBTransactions.Contains(i.CBId.ToString())).ToList();

        
            TempData["lstARChargebacktransactionList"] = lstARChargebacktransactionList;
            string ProcessedCBTrx = AccountPayableService.InsertFranchiseeChargebackTransaction(lstARChargebacktransactionList, cbTrxDate);
            TempData["ProcessedCBTrx"] = ProcessedCBTrx;
            return RedirectToAction("FranchiseeChargebackList", "AccountsPayable", new { area = "Portal" });
        }

        public int GetPeriodId(int BillMonth, int BillYear)
        {
            Period Period;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Period = context.Periods.Where(o => o.BillMonth == BillMonth && o.BillYear == BillYear).FirstOrDefault();
                return Period.PeriodId;
            }

        }

        public List<portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods_Result> GetAvailableFranchiseeReportPeriods(string RegionId)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods_Result> lstPeriods = context.portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods(RegionId).ToList();
                return lstPeriods;
            }

        }

        public List<portal_spGet_AP_AvailableChargebackFinalizedPeriods_Result> GetAvailableChargebackFinalizedPeriods(string RegionId)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableChargebackFinalizedPeriods_Result> lstFinalizedPeriods = context.portal_spGet_AP_AvailableChargebackFinalizedPeriods(RegionId).ToList();
                return lstFinalizedPeriods;
            }

        }

        public List<portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods_Result> GetAvailableFranchiseeReportFinalizedPeriods(string RegionId)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods_Result> lstFinalizedPeriods = context.portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods(RegionId).ToList();
                return lstFinalizedPeriods;
            }

        }


        public List<portal_spGet_AP_AvailableChargebackPeriods_Result> GetAvailableChargeBackPeriods(string RegionId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableChargebackPeriods_Result> lstPeriods = context.portal_spGet_AP_AvailableChargebackPeriods(RegionId).ToList();
                return lstPeriods;
            }

        }

        #endregion


        #region Franchisee Pay

        private List<string> GetFranchiseeReportPeriods()
        {
            List<string> billMonthYears = new List<string>();
            //DateTime dt = DateTime.Now;
            //DateTime cnt = new DateTime(dt.Year, dt.Month, dt.Day);
            //DateTime stop = DateTime.Today.AddMonths(1);

            DateTime cnt = new DateTime(2017, 1, 1);
            DateTime stop = DateTime.Today.AddMonths(6);

            while (cnt < stop)
            {
                billMonthYears.Add(string.Format("{0:00}/{1}", cnt.Month, cnt.Year));
                cnt = cnt.AddMonths(1);
            }

            billMonthYears.Reverse();

            return billMonthYears;
        }

        private List<string> GetUnfinalizedFranchiseeReportPeriods()
        {
            List<string> billMonthYears = GetFranchiseeReportPeriods();

            var finalizedMonthYears = AccountPayableService.GetFinalizedFranchiseeReportPeriods().Select(o => string.Format("{0:00}/{1}", o.BillMonth, o.BillYear));
            foreach (var finalizedMonthYear in finalizedMonthYears)
                billMonthYears.Remove(finalizedMonthYear);

            return billMonthYears;
        }

        public ActionResult FranchiseePay()
        {
            ViewBag.CurrentMenu = "AccountsPayableReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseePay", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("FranchiseePay", "AccountsPayable", new { area = "Portal" }), "Franchisee Pay");

            var regionlist = _commonService.GetRegionList().Where(o => o.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            //ViewBag.BillMonthYears = new SelectList(_commonService.GetPeriodDropDownValues(PeriodDropDownName.FranchiseeReport.ToString(), _claimView.GetCLAIM_SELECTED_PERIOD_ID()), "Value", "Text");// new SelectList(GetAvailableFranchiseeReportPeriods(SelectedRegionId.ToString()), "Period", "Period");
            ViewBag.BillMonthYears = new SelectList(GetAvailableFranchiseeReportPeriods(SelectedRegionId.ToString()), "PeriodId", "Period", GetAvailableFranchiseeReportPeriods(SelectedRegionId.ToString()).FirstOrDefault());

            return View();
        }

        public ActionResult FranchiseePayProcess(int? BillMonth, int? BillYear)
        {
            ViewBag.CurrentMenu = "AccountsPayableReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseePayProcess", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("FranchiseePayProcess", "AccountsPayable", new { area = "Portal" }), "Franchisee Pay Process");

            var regionlist = _commonService.GetRegionList().Where(o => o.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            //ViewBag.BillMonthYears = new SelectList(GetFranchiseeReportPeriods(), string.Format("{0:00}/{1}", DateTime.Today.Month, DateTime.Today.Year));
            ViewBag.BillMonthYears = new SelectList(GetAvailableFranchiseeReportFinalizedPeriods(SelectedRegionId.ToString()), "Period", "Period", BillMonth.ToString() + "/" + BillYear.ToString());

            return View();
        }

        [HttpGet]
        public ActionResult FranchiseePayDetail(int Id)
        {
            return PartialView("_PartialFranchiseePayDetail", AccountPayableService.GetFranchiseeReportDetails(Id));
        }

        [HttpGet]
        public ActionResult FranchiseePayDeductions(int Id)
        {
            return PartialView("_PartialFranchiseePayDeductions", AccountPayableService.GetFranchiseeReportDetails(Id));
        }

        [HttpGet]
        public bool DeleteGeneratedFranchiseeReport(string ids)
        {

            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string a in arrids)
            {
                AccountPayableService.DeleteGeneratedFranchiseeReport(int.Parse(a.Trim()));
            }


            return true;
        }


        [HttpGet]
        public bool ClearGeneratedFranchiseeReport(string RegionIds, int PeriodId)
        {
            AccountPayableService.ClearGeneratedFranchiseeReport(RegionIds, PeriodId);

            return true;
        }

        [HttpGet]
        public ActionResult FranchiseeReport(string ids, bool print = false)
        {
            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ViewBag.IsPrintView = print;

            List<FranchiseeReportDetailsViewModel> vms = new List<FranchiseeReportDetailsViewModel>();
            foreach (string a in arrids)
            {
                vms.Add(AccountPayableService.GetFranchiseeReportDetails(int.Parse(a.Trim())));
            }

            return View(vms);
        }

        public ActionResult FranchiseePayFinalizedDetail(int Id)
        {
            return PartialView("_PartialFranchiseePayFinalizedDetail", AccountPayableService.GetFranchiseeReportDetailsFinalized(Id));
        }

        [HttpGet]
        public ActionResult FranchiseePayFinalizedDeductions(int Id)
        {
            return PartialView("_PartialFranchiseePayFinalizedDeductions", AccountPayableService.GetFranchiseeReportDetailsFinalized(Id));
        }

        [HttpGet]
        public JsonResult GetFranchiseeReportList(int? periodId = null, string rgId = null)
        {

            Period period = new Period();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                period = context.Periods.Where(p => p.PeriodId == periodId).FirstOrDefault();
            }

            var reports = AccountPayableService.GetGeneratedFranchiseeReportList(period.BillMonth, period.BillYear, rgId);


            if (SelectedRegionId > 0 && string.IsNullOrEmpty(rgId))
            {
                var regionIds = _commonService.GetRegionList().Select(r => r.RegionId).ToList();

                if (regionIds.Any())
                {
                    reports = (from i in reports
                               join id in regionIds on i.RegionId equals id
                               select i).ToList();
                }
            }

            var jsonResult = Json(new { aaData = reports, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GeneratedFranchiseeReportList(bool doGenerate = false, int? periodId = null, string rgId = null)
        {
            Period period = new Period();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                period = context.Periods.Where(p => p.PeriodId == periodId).FirstOrDefault();
            }


            if (doGenerate && !string.IsNullOrEmpty(rgId))
            {
                string[] arrids = rgId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string regionIdStr in arrids)
                {
                    int regionId = 0;
                    if (int.TryParse(regionIdStr, out regionId))
                        AccountPayableService.GenerateFranchiseeReports((int)period.BillMonth, (int)period.BillYear, regionId);
                }
            }

            var reports = AccountPayableService.GetGeneratedFranchiseeReportList(period.BillMonth, period.BillYear, rgId);


            if (SelectedRegionId > 0 && string.IsNullOrEmpty(rgId))
            {
                var regionIds = _commonService.GetRegionList().Select(r => r.RegionId).ToList();

                if (regionIds.Any())
                {
                    reports = (from i in reports
                               join id in regionIds on i.RegionId equals id
                               select i).ToList();
                }
            }

            var jsonResult = Json(new { aaData = reports, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult FinalizedFranchiseeReportList(int? month = null, int? year = null, string rgId = null)
        {
            var reports = AccountPayableService.GetFinalizedFranchiseeReportList(month, year, rgId);
            if (SelectedRegionId > 0 && string.IsNullOrEmpty(rgId))
            {
                var regionIds = _commonService.GetRegionList().Select(r => r.RegionId).ToList();

                if (regionIds.Any())
                {
                    reports = (from i in reports
                               join id in regionIds on i.RegionId equals id
                               select i).ToList();
                }
            }

            /*DateTime sDate;
            DateTime eDate;
            if (!string.IsNullOrEmpty(d) && d.Contains("-") && DateTime.TryParseExact(d.Split('-')[0], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out sDate)
                && DateTime.TryParseExact(d.Split('-')[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eDate) && eDate > sDate)
            {
                invoices = invoices.Where(o => o.FinalizedDate.HasValue && o.FinalizedDate.Value.Date >= sDate && o.FinalizedDate.Value.Date <= eDate).ToList();
            }*/

            var jsonResult = Json(new { aaData = reports, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult FinalizeFranchiseeReports(FormCollection frm)
        {
            string ids = frm["finalizeIds"];
            string regionIds = frm["regionIds"];
            int billMonth = 0;
            int billYear = 0;

            int periodId = Convert.ToInt32(frm["periodId"]);
            Period period = new Period();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                period = context.Periods.Where(p => p.PeriodId == periodId).FirstOrDefault();
            }

            billMonth = (int)period.BillMonth;
            billYear = (int)period.BillYear;

            var finalized = AccountPayableService.FinalizeFranchiseeReportCreate(regionIds, periodId, ids, this.LoginUserId);
            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return RedirectToAction("RunPaymentCheck", "AccountsPayable", new { area = "Portal", CheckType = 1 });

            /*
            AccountPayableService.FinalizeFranchiseeReportCreate(regionIds, periodId, ids, this.LoginUserId);

            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string a in arrids)
            {
                int reportId = int.Parse(a.Trim());
                if (AccountPayableService.FinalizeFranchiseeReport(reportId))
                    AccountPayableService.InsertAPBillTransactionForFranchiseeReport(reportId);
            }
            */



            return RedirectToAction("FranchiseePayProcess", new RouteValueDictionary(new { controller = "AccountsPayable", action = "FranchiseePayProcess", BillMonth = billMonth, BillYear = billYear }));

        }

        [HttpGet]
        public JsonResult GeneratedChecksList(int regionId, int typeId, int TransactionStatusListId)
        {
            var lst = AccountPayableService.GetAPBillListForCheckType(regionId, typeId, TransactionStatusListId);
         
            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult UpdateTransactionStatus(int RegionId, int CheckbookTransactionTypeListId, string StatusStr)
        {
            var lst = AccountPayableService.GetPendingAPBillListForCheckType(RegionId, CheckbookTransactionTypeListId, 20, "");
            List<int> checkBookIds = new List<int>();
            string transactionStatus = StatusStr;

            foreach (var apBillTrx in lst)
            {
                if (apBillTrx.CheckBookId != -1)
                {
                    AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, transactionStatus);
                }
            }

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public JsonResult UpdateCloseTransactionStatus(int RegionId, int CheckbookTransactionTypeListId, string GetTrxStatusId, string UpdateTrxStatusId, string APBillList = "")
        {
            int PENDING_TRXSTATUSID = 20;

            var lst = AccountPayableService.GetPendingAPBillListForCheckType(RegionId, CheckbookTransactionTypeListId, PENDING_TRXSTATUSID, APBillList);
            List<int> checkBookIds = new List<int>();
            //string transactionStatus = "CLOSE";

            foreach (var apBillTrx in lst)
            {
                if (apBillTrx.CheckBookId != -1)
                {
                    AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, UpdateTrxStatusId);
                    checkBookIds.Add((int)apBillTrx.CheckBookId);

                }
            }

            AccountPayableService.InsertFranchiseeManualTrasactionFromWriteCheck(checkBookIds);

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public JsonResult UpdatePendingTransactionStatus(int RegionId, int CheckbookTransactionTypeListId)
        {
            var lst = AccountPayableService.GetPendingAPBillListForCheckType(RegionId, CheckbookTransactionTypeListId, 4, "");
            List<int> checkBookIds = new List<int>();
            string transactionStatus = "PENDING";

            foreach (var apBillTrx in lst)
            {
                if (apBillTrx.CheckBookId != -1)
                {
                    AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, transactionStatus);
                }
            }

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public JsonResult UpdateOpenManualCheckTransactionStatus(int regionId)
        {
            var lst = _companyService.GetPendingUnprintedManualCheckList(regionId, 20);
            List<int> checkBookIds = new List<int>();
            string transactionStatus = "open";

            foreach (var apBillTrx in lst)
            {
                if (apBillTrx.CheckBookId != -1)
                {
                    AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, transactionStatus);
                }
            }

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        //public JsonResult UpdateCloseManualCheckTransactionStatus(int regionId)
        //{
        //    var lst = _companyService.GetPendingUnprintedManualCheckList(regionId, 20);
        //    List<int> checkBookIds = new List<int>();
        //    string transactionStatus = "close";

        //    foreach (var apBillTrx in lst)
        //    {
        //        if (apBillTrx.CheckBookId != -1)
        //        {
        //            AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, transactionStatus);
        //        }
        //    }

        //    var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;

        //}

        public JsonResult UpdatePendingManualCheckTransactionStatus(int regionId)
        {
            var lst = _companyService.GetPendingUnprintedManualCheckList(regionId, 4);
            List<int> checkBookIds = new List<int>();
            string transactionStatus = "PENDING";

            foreach (var apBillTrx in lst)
            {
                if (apBillTrx.CheckBookId != -1)
                {
                    AccountPayableService.CheckSourceDataWorker(apBillTrx.APBillId, (int)apBillTrx.CheckBookTransactionTypeListId, transactionStatus);
                }
            }

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        [HttpPost]
        public JsonResult RePrintChecks(int RegionId, int checkTypeId, string GetTRXStatusId, string UpdateTRXStatusId, string APBillList)
        {
            int TRXStatusId = 0;

            switch (GetTRXStatusId)
            {
                case "PENDING_TRXSTATUSID":
                    TRXStatusId = 20;
                    break;
                default:
                    TRXStatusId = 20;
                    break;
            }


            var lst = AccountPayableService.GetAPBillListForCheckType(RegionId, checkTypeId, TRXStatusId, APBillList);
            List<int> checkBookIds = new List<int>();

            foreach (var apBill in lst)
            {
                if (apBill.CheckBookId != null)
                {
                    checkBookIds.Add((int)apBill.CheckBookId);
                    AccountPayableService.CheckSourceDataWorker(apBill.APBillId, checkTypeId, UpdateTRXStatusId);
                }
                
            }

          
            string retPath = "";
            if (checkTypeId == 1)
            {
                retPath = Url.Action("CheckFinalizedReport", "Company", new { area = "Portal", ids = string.Join(",", checkBookIds) });
            }
            else
            {
                retPath = Url.Action("Check", "Company", new { area = "Portal", ids = string.Join(",", checkBookIds), print = false, showSample = false, rt = "", c = "", CheckTypeId = checkTypeId });

            }

            return Json(retPath, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public JsonResult RunChecks(FormCollection frm)
        {
            int intRes = 0;
            DateTime dateRes = DateTime.Now;

            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;
            int checkTypeId = int.TryParse(frm["CheckTypeList"], out intRes) ? intRes : 0;
            int regionId = int.TryParse(frm["regionlist"], out intRes) ? intRes : this.SelectedRegionId;
            DateTime trxDate = DateTime.TryParse(frm["dtCheckDate"], out dateRes) ? dateRes : DateTime.Now;
            int TransactionStatusListId = 4;

            var lst = AccountPayableService.GetAPBillListForCheckType(regionId, checkTypeId, TransactionStatusListId);
            List<int> checkBookIds = new List<int>();

            foreach (var apBill in lst)
            {
                if (apBill.CheckBookId == null)
                {

                    var checkBookId = AccountPayableService.InsertCheckBookFromAPBill(apBill.APBillId, bankId, trxDate);

                    if (checkBookId != -1)
                        checkBookIds.Add(checkBookId);
                }else{

                    checkBookIds.Add((int)apBill.CheckBookId);
                }
            }

            var leftopenlst = AccountPayableService.GetOpenNotPrintedAPBillListForCheckType(regionId, checkTypeId);

            foreach (var apbillOpen in leftopenlst)
            {
                int updateAPBillId = AccountPayableService.UpdateOpenNotPrintedAPBill(apbillOpen.APBillId);
            }

            UpdatePendingTransactionStatus(SelectedRegionId, checkTypeId);

            string retPath = "";
            if (checkTypeId == 1)
            { 
                retPath = Url.Action("CheckFinalizedReport", "Company", new { area = "Portal", ids = string.Join(",", checkBookIds) });
            }
            else
            {
                retPath = Url.Action("Check", "Company", new { area = "Portal", ids = string.Join(",", checkBookIds), print = false, showSample = false, rt = "", c = "", CheckTypeId = checkTypeId });

            }

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RunManualChecks(FormCollection frm)
        {
            int intRes = 0;
            DateTime dateRes = DateTime.Now;

            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;
            string checks = frm["runCheckIds"];
            int regionId = int.TryParse(frm["regionlist"], out intRes) ? intRes : this.SelectedRegionId;
            DateTime trxDate = DateTime.TryParse(frm["dtCheckDate"], out dateRes) ? dateRes : DateTime.Now;

            var lst = AccountPayableService.GetAPBillListForCheckById(regionId, checks);
            List<int> checkBookIds = new List<int>();

            foreach (var apBill in lst)
            {

                if (apBill.CheckBookId == null)
                {

                    var checkBookId = AccountPayableService.InsertCheckBookFromAPBill(apBill.APBillId, bankId, trxDate);

                    if (checkBookId != -1)
                        checkBookIds.Add(checkBookId);
                }
                else
                {
                    checkBookIds.Add((int)apBill.CheckBookId);
                }
                
            }

            UpdatePendingManualCheckTransactionStatus(SelectedRegionId);

            
            

            //var leftopenlst = AccountPayableService.GetOpenNotPrintedAPBillListForCheckType(regionId, checkTypeId);

            //foreach (var apbillOpen in leftopenlst)
            //{
            //    int updateAPBillId = AccountPayableService.UpdateOpenNotPrintedAPBill(apbillOpen.APBillId);
            //}


            string retPath = "";
            retPath = Url.Action("Check", "Company", new { area = "Portal", ids = string.Join(",", checkBookIds), print = false, showSample = false, rt = "", c = "", CheckTypeId = -1 });

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FranchiseePayReportPopupDetail(string ids, bool print = false)
        {
            string[] arrids = ids.Split(',');

            ViewBag.IsPrintView = print;

            List<FranchiseeReportDetailsViewModel> vms = new List<FranchiseeReportDetailsViewModel>();
            foreach (string a in arrids)
            {
                vms.Add(AccountPayableService.GetFranchiseeReportDetails(int.Parse(a.Trim())));
                
            }

            return PartialView("_FranchiseePayReportPopup", vms);
        }




        public ActionResult FranchiseePayReportPopupDetailFinalized(string ids, bool print = false)
        {
            string[] arrids = ids.Split(',');

            ViewBag.IsPrintView = print;

            List<FranchiseeReportFinalizedDetailsViewModel> vms = new List<FranchiseeReportFinalizedDetailsViewModel>();
            foreach (string a in arrids)
            {
                vms.Add(AccountPayableService.GetFranchiseeReportDetailsFinalized(int.Parse(a.Trim())));
            }

            return PartialView("_FranchiseePayReportFinalizedPopup", vms);
        }

        [HttpGet]
        public FileResult FranchiseeReportExportToPDFFinalized(string ids)
        {
            string[] arrids = ids.Split(',');

            List<FranchiseeReportFinalizedDetailsViewModel> vms = new List<FranchiseeReportFinalizedDetailsViewModel>();
            foreach (string a in arrids)
            {
                var vm = AccountPayableService.GetFranchiseeReportDetailsFinalized(int.Parse(a.Trim()));
                vms.Add(vm);
            }

            string HTMLContent = RenderViewToString("FranchiseeReportFinalized", vms);

            var doc = new Document(PageSize.A4, 10f, 1f, 10f, 30f);

            var memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;

            doc.Open();

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(Server.MapPath("~/Content/FranchiseeReport.css"), true);

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

            string filename = string.Format("{0}_FranchiseeReportFinalized.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Response.AddHeader("content-disposition", "inline;filename=\"" + filename + "\"");

            return File(buf, "application/pdf");
        }

        [HttpGet]
        public JsonResult PrintFranchiseePayReportFinalized(string ids)
        {
            string[] arrids = ids.Split(',');

            List<FranchiseeReportFinalizedDetailsViewModel> vms = new List<FranchiseeReportFinalizedDetailsViewModel>();
            foreach (string a in arrids)
            {
                var vm = AccountPayableService.GetFranchiseeReportDetailsFinalized(int.Parse(a.Trim()));
                vms.Add(vm);
            }

            string HTMLContent = RenderViewToString("FranchiseeReportFinalized", vms);

            var doc = new Document(PageSize.A4, 10f, 1f, 10f, 30f);

            var memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;

            doc.Open();

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(Server.MapPath("~/Content/FranchiseeReport.css"), true);

            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(doc, writer)));
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

            var _crData = "temp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt");
            var retPath = "/Upload/InvoiceFiles/" + _crData + ".pdf";
            var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), _crData + ".pdf");
            System.IO.File.WriteAllBytes(path, buf);

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region :: FranchiseeReport  Export To PDF ::

        //Franchisee Report Export To PDF File
        [HttpGet]
        public FileResult FranchiseeReportExportToPDF(string ids)
        {
            string[] arrids = ids.Split(',');

            List<FranchiseeReportDetailsViewModel> vms = new List<FranchiseeReportDetailsViewModel>();
            foreach (string a in arrids)
            {
                var vm = AccountPayableService.GetFranchiseeReportDetails(int.Parse(a.Trim()));
                vms.Add(vm);
            }

            string HTMLContent = RenderViewToString("FranchiseeReport", vms);

            var doc = new Document(PageSize.A4, 10f, 1f, 10f, 30f);

            var memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;

            doc.Open();

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(Server.MapPath("~/Content/FranchiseeReport.css"), true);

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

            string filename = string.Format("{0}_FranchiseeReport.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Response.AddHeader("content-disposition", "inline;filename=\"" + filename + "\"");

            return File(buf, "application/pdf");
        }

        public byte[] GetPDF(string pHTML)
        {
            /*#region -- styles --

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
            styles.LoadStyle("col4", "width", "128");
            styles.LoadStyle("col5", "width", "35");
            styles.LoadStyle("col6", "width", "37");

            styles.LoadStyle("t2col1", "width", "35");
            styles.LoadStyle("t2col2", "width", "149");
            styles.LoadStyle("t2col3", "width", "33");
            styles.LoadStyle("t2col4", "width", "32");
            styles.LoadStyle("t2col5", "width", "35");
            styles.LoadStyle("t2col6", "width", "37");

            #endregion -- styles --*/

            byte[] bPDF = null;
            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            HTMLWorker htmlWorker = new HTMLWorker(doc);
            //htmlWorker.SetStyleSheet(styles);
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

        #region Common For ViewToHtml

        protected string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            ViewBag.DomainUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority + "/";

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext,
                                                                         viewName, "_LayoutEmpty");
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion Common For ViewToHtml

        #endregion :: FranchiseeReport  Export To PDF ::

        #region Billing
        public ActionResult FranchiseeTRXList()
        {
            ViewBag.CurrentMenu = "AccountsPayableGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeTRXList", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("FranchiseeTRXList", "AccountsPayable", new { area = "Portal" }), "List");

            return View();
        }

        public ActionResult BillingPayList()
        {
            ViewBag.CurrentMenu = "AccountsPayableGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("BillingPayList", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("BillingPayList", "AccountsPayable", new { area = "Portal" }), "BillingPay List");
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            return View();
        }

        [HttpGet]
        public JsonResult BillingPayListData(string d = "", string st = "", bool consolidated = false, int? rgId = null, int month = 0, int year = 0)
        {
            var invoices = new List<FPBillingPay>();
            invoices = AccountPayableService.GetFPInvoiceListWithSearchForPayment(st, consolidated, rgId, month, year);
            DateTime sDate;
            DateTime eDate;
            if (!string.IsNullOrEmpty(d) && d.Contains("-") &&
                DateTime.TryParseExact(d.Split('-')[0], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out sDate)
                &&
                DateTime.TryParseExact(d.Split('-')[1], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out eDate) && eDate > sDate)
            {
                invoices = invoices.Where(o => o.CreatedDate >= sDate && o.CreatedDate <= eDate).ToList();

                var jsonResult1 = Json(new { aaData = invoices, success = true }, JsonRequestBehavior.AllowGet);
                jsonResult1.MaxJsonLength = int.MaxValue;
                return jsonResult1;
            }
            else if (!string.IsNullOrEmpty(d) && d.Contains("-") &&
                     DateTime.TryParseExact(d.Split('-')[0], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                         DateTimeStyles.None, out sDate)
                     &&
                     DateTime.TryParseExact(d.Split('-')[1], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                         DateTimeStyles.None, out eDate) && eDate == sDate)
            {
                invoices = invoices.Where(o => o.CreatedDate == sDate).ToList();

                var jsonResult2 = Json(new { aaData = invoices, success = true }, JsonRequestBehavior.AllowGet);
                jsonResult2.MaxJsonLength = int.MaxValue;
                return jsonResult2;
            }


            var jsonResult = Json(new { aaData = invoices, success = false }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

            //    DateTime sDate;
            //DateTime eDate;
            //if (!string.IsNullOrEmpty(d) && d.Contains("-") && DateTime.TryParseExact(d.Split('-')[0], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out sDate)
            //    && DateTime.TryParseExact(d.Split('-')[1], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eDate) && eDate > sDate)
            //{

            //    return Json(new { aaData = invoices, success = true }, JsonRequestBehavior.AllowGet);
            //}

            //return Json(new { aaData = invoices, success = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult FranchiseeBillingDetailPopup(int id)
        {
            var model = AccountPayableService.GetFranchiseBillingDetails(id);
            return PartialView("_FranchiseeBillingDetailPopup", model);
        }
        public ActionResult FranchiseeBillingDetailPopupWithbillno(string billno)
        {
            var model = AccountPayableService.GetFranchiseBillingDetailsWithbillno(billno);
            return PartialView("_FranchiseeBillingDetailPopup", model);
        }

        #region TurnAround
        //
        [HttpGet]
        public ActionResult FranchiseeTurnaroundCheck()
        {
            ViewBag.CurrentMenu = "AccountsPayablePay";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("FranchiseeTurnaroundCheck", "AccountsPayable", new { area = "Portal" }), "Billing");
            BreadCrumb.Add(Url.Action("FranchiseeTurnaroundCheck", "AccountsPayable", new { area = "Portal" }), "Turn Around Check");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();

        }

        [HttpGet]
        public ActionResult TurnaroundProcess()
        {
            ViewBag.CurrentMenu = "TurnaroundProcess";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TurnaroundProcess", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("TurnaroundProcess", "AccountsPayable", new { area = "Portal" }), "Turnaround");
            BreadCrumb.Add(Url.Action("TurnaroundProcess", "AccountsPayable", new { area = "Portal" }), "Turnaround Process");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public ActionResult TurnaroundList()
        {
            ViewBag.CurrentMenu = "TurnaroundList";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("TurnaroundList", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("TurnaroundList", "AccountsPayable", new { area = "Portal" }), "Turnaround");
            BreadCrumb.Add(Url.Action("TurnaroundList", "AccountsPayable", new { area = "Portal" }), "Turnaround List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        public ActionResult TurnAroundListResultData(string regionIds, DateTime from, DateTime to, int viewmode,string sortby="")
        {
            if (regionIds == "null") regionIds = null;

            if (viewmode == 1)
            {
                try
                {
                    var response = AccountPayableService.GetTurnAroundListListSummary(regionIds, from, to);
                    var result = from f in response
                                 orderby f.FranchiseeNo ascending
                                 select new
                                 {

                                     //FranchiseeNo = f.FranchiseeNo,
                                     //FranchiseeName = f.Name,
                                     //ChargeBackAmount = f.ChargeBackAmount != null ? String.Format("{0:C}", f.ChargeBackAmount) : "$0.00",

                                     //PaymentAmount = f.ChargeBackPayAmount != null ? String.Format("{0:C}", f.ChargeBackPayAmount) : "$0.00",
                                     //ChargeBackPayAmount = f.ChargeBackPayAmount != null ? String.Format("{0:C}", f.ChargeBackPayAmount) : "$0.00",
                                     //RegionName = f.RegionName,

                                     FranchiseeNo = f.FranchiseeNo,
                                     FranchiseeName = f.Name,
                                     ChargeBackAmount = (decimal)(f.ChargeBackAmount != null ? f.ChargeBackAmount : 0.00m),
                                     PaymentAmount = (decimal)(f.PaymentAmount != null ? f.PaymentAmount : 0.00m),
                                     ChargeBackPayAmount = (decimal)(f.ChargeBackPayAmount != null ? f.ChargeBackPayAmount : 0.00m),
                                     RegionName = f.RegionName,
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

            else
            {
                try
                {
                    var response = AccountPayableService.GetTurnAroundListList(regionIds, from, to);
                    List<APTurnAroundListModel> ListDataResult = new List<APTurnAroundListModel>();

                    var result = from f in response
                                 orderby f.FranchiseeNo ascending
                                 select new APTurnAroundListModel()
                                 {
                                     //TurnAroundId = f.TurnAroundId,
                                     //FranchiseeNo = f.FranchiseeNo,
                                     //FranchiseeName = f.Name,
                                     //CustomerName = f.CustomerName,
                                     //InvoiceNo = f.InvoiceNo != null ? f.InvoiceNo : string.Empty,
                                     //InvoiceId = f.InvoiceId,
                                     //ChargeBackDate = f.ChargebackDate,
                                     //ChargeBackAmount = f.ChargeBackAmount != null ? String.Format("{0:C}", f.ChargeBackAmount) : "$0.00",
                                     //PaymentDate = f.PaymentDate,
                                     //PaymentAmount = f.ChargeBackPayAmount != null ? String.Format("{0:C}", f.ChargeBackPayAmount) : "$0.00",
                                     //ChargeBackPayAmount = f.ChargeBackPayAmount != null ? String.Format("{0:C}", f.ChargeBackPayAmount) : "$0.00",
                                     //RegionName = f.RegionName,

                                     TurnAroundId = f.TurnAroundId,
                                     FranchiseeNo = f.FranchiseeNo,
                                     FranchiseeName = f.Name,                                     
                                     CustomerName = f.CustomerName,
                                     InvoiceNo = f.InvoiceNo != null ? f.InvoiceNo : string.Empty,
                                     InvoiceId = f.InvoiceId,
                                     ChargeBackDate = f.ChargebackDate,
                                     ChargeBackAmount = (decimal)(f.ChargeBackAmount != null ? f.ChargeBackAmount : 0.00m),
                                     PaymentDate = f.PaymentDate,
                                     PaymentAmount = (decimal)(f.PaymentAmount != null ? f.PaymentAmount : 0.00m),
                                     ChargeBackPayAmount = (decimal)(f.ChargeBackPayAmount != null ? f.ChargeBackPayAmount : 0.00m),
                                     RegionName = f.RegionName,
                                     TARCBDate = f.TARCBDate,
                                     NegativeDueAmount = f.NegativeDueAmount
                                 };
                    if (result != null && result.Count() > 0)
                    {
                        ListDataResult = result.ToList();
                    }
                    if (sortby != "none")
                    {
                        if (sortby == "cNameAsc") {
                            ListDataResult = ListDataResult.OrderBy(o => o.FranchiseeName).ToList();
                        }
                        else if (sortby == "cNameDesc")
                        {
                            ListDataResult = ListDataResult.OrderByDescending(o => o.FranchiseeName).ToList();
                        }
                        else if (sortby == "cNoAsc")
                        {
                            ListDataResult = ListDataResult.OrderBy(o => o.FranchiseeNo).ToList();
                        }
                        else if (sortby == "cNoDesc")
                        {
                            ListDataResult = ListDataResult.OrderByDescending(o => o.FranchiseeNo).ToList();
                        }
                        else if (sortby == "cRegionAsc")
                        {
                            ListDataResult = ListDataResult.OrderBy(o => o.RegionName).ToList();
                        }
                        else if (sortby == "cRegionDesc")
                        {
                            ListDataResult = ListDataResult.OrderByDescending(o => o.RegionName).ToList();
                        }
                    }
                    return Json(new
                    {
                        aadata = ListDataResult,
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public JsonResult FranchiseeTurnaroundCheckList(int? regionId, int TurnAroundCheckType)
        {
            // todo: use periodType parameter (assume weekly and -7 days for now)
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);

            var data = "";
            //var data = AccountPayableService.GetFranchiseeTurnaroundCheckList(regionId.ToString(), null);
            
            var jsonResult = Json(new { Data = data, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult FranchiseeTurnaroundCheckDetails(int? regionId, int franchiseeId, int periodType = 1)
        {
            // todo: use periodType parameter (assume weekly and -7 days for now)
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);

            var data = "";
            //var data = AccountPayableService.GetFranchiseeTurnaroundCheckDetails(regionId, franchiseeId, startDate, endDate);

            var jsonResult = Json(new { Data = data, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CreateTurnaroundChecks(FormCollection frm)
        {
            int intRes = 0;

            int regionId = int.TryParse(frm["regionlist"], out intRes) ? intRes : this.SelectedRegionId;
            int periodType = int.TryParse(frm["slPeriodType"], out intRes) ? intRes : 1;

            string[] franchiseeIds = frm["finalizeIds"].Split(',');

            // todo: use periodType parameter (assume weekly and -7 days for now)
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);

            var checkList = "";
            //var checkList = AccountPayableService.GetTurnaroundCheckList(regionId.ToString(), null);

            foreach (string a in franchiseeIds)
            {
                if (!int.TryParse(a, out intRes))
                    continue;

                int franchiseeId = intRes;
                var franchiseeData = "";
                //var franchiseeData = checkList.Where(o => o.FranchiseeId == franchiseeId).FirstOrDefault();
                if (franchiseeData == null)
                    continue;

                var tacvm = new FranchiseeTurnaroundCheckViewModel();
                tacvm.RegionId = regionId;
                tacvm.FranchiseeId = franchiseeId;
                //tacvm.StartDate = (DateTime)franchiseeData.StartDate;
                //tacvm.EndDate = (DateTime)franchiseeData.EndDate;
                //tacvm.Amount = (decimal)franchiseeData.TurnaroundAmount;
                tacvm.CreatedBy = this.LoginUserId;
                tacvm.CreatedDate = DateTime.Now;

                int turnaroundCheckId = AccountPayableService.InsertFranchiseeTurnaroundCheck(tacvm);

                var vm = new APBillTransactionViewModel();

                vm.RegionId = regionId;
                vm.CreatedBy = this.LoginUserId;
                vm.CreatedDate = DateTime.Now;

                var apbvm = new JKViewModels.AccountsPayable.APBillViewModel();

                apbvm.TypeListId = 2; // franchisee
                apbvm.ClassId = franchiseeId;
                apbvm.CheckBookTransactionTypeListId = 33; // turnaround check
                apbvm.IsManual = false;
                apbvm.BillMonth = tacvm.CreatedDate.Month;
                apbvm.BillYear = tacvm.CreatedDate.Year;

                apbvm.FranchiseeReportId = -1;
                apbvm.FranchiseeTurnaroundCheckId = turnaroundCheckId;
                apbvm.ManualCheckId = -1;

                vm.APBill = apbvm;

                AccountPayableService.InsertAPBillTransaction(vm);
            }

            string retPath = Url.Action("RunPaymentCheck", "AccountsPayable", new { area = "Portal" });

            return RedirectToAction(retPath, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public JsonResult PrintFranchiseePayReport(string ids)
        {
            string[] arrids = ids.Split(',');

            List<FranchiseeReportDetailsViewModel> vms = new List<FranchiseeReportDetailsViewModel>();
            foreach (string a in arrids)
            {
                var vm = AccountPayableService.GetFranchiseeReportDetails(int.Parse(a.Trim()));
                vms.Add(vm);
            }

            string HTMLContent = RenderViewToString("FranchiseeReport", vms);

            var doc = new Document(PageSize.A4, 10f, 1f, 10f, 30f);

            var memStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, memStream);
            writer.CloseStream = false;

            doc.Open();

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(Server.MapPath("~/Content/FranchiseeReport.css"), true);

            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(doc, writer)));
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

            var _crData = "temp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt");
            var retPath = "/Upload/InvoiceFiles/" + _crData + ".pdf";
            var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), _crData + ".pdf");
            System.IO.File.WriteAllBytes(path, buf);

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChargeBackReport()
        {
            ViewBag.CurrentMenu = "AccountsPayableGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargeBackReport", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("ChargeBackReport", "AccountsPayable", new { area = "Portal" }), "ChargeBackReport");
            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            return View();
        }


        public JsonResult ChargeBackHistoryReportData(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0)
        {
            var lstARChargebackSummaryViewModel = AccountPayableService.GetChargebackHistorySummaryReportList(regionId, startDate, endDate, month, year);
            var lstARChargebackDetailsViewModel = AccountPayableService.GetChargebackHistoryReportList(regionId, startDate, endDate, month, year);

            TempData["lstARChargebackListViewModel"] = lstARChargebackDetailsViewModel;

            return Json(new
            {
                summaryData = lstARChargebackSummaryViewModel,
                detailsData = lstARChargebackDetailsViewModel,
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ChargeBackHistoryReportDataPrint(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0, string selView = "")
        {
            var lstARChargebackSummaryViewModel = AccountPayableService.GetChargebackHistorySummaryReportList(regionId, startDate, endDate, month, year);
            var lstARChargebackDetailsViewModel = AccountPayableService.GetChargebackHistoryReportList(regionId, startDate, endDate, month, year);

            ViewBag.selView = selView;
            ViewBag.SummaryViewModel = lstARChargebackSummaryViewModel;
            ViewBag.DetailsViewModel = lstARChargebackDetailsViewModel;

            if (selView == "1")
            {
                if (lstARChargebackSummaryViewModel.ToList().Count > 0)
                {
                    String HtmlContent = String.Empty;
                    HtmlContent += RenderPartialViewToString("_PartialChargeBackHistoryReportDataPrint", null);
                    var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                    var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                    var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                    System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HtmlContent));

                    return Json(lsePath, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("");
            }
            else
            {
                if (lstARChargebackDetailsViewModel.ToList().Count > 0)
                {
                    String HtmlContent = String.Empty;
                    HtmlContent += RenderPartialViewToString("_PartialChargeBackHistoryReportDataPrint", null);
                    var lseData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                    var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
                    var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
                    System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HtmlContent));

                    return Json(lsePath, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("");
            }



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
        public JsonResult GetChargebackHistoryReportList(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0)
        {
            return Json(AccountPayableService.GetChargebackHistoryReportList(regionId, startDate, endDate, month, year), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChargeBackCredit()
        {
            ViewBag.CurrentMenu = "AccountsPayableGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ChargeBackCredit", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("ChargeBackCredit", "AccountsPayable", new { area = "Portal" }), "ChargeBackCredit");
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            //ViewBag.FranchiseeTransactionTypeList = new SelectList(FranchiseeService.GetFranchiseeTransactionTypeList(), "FranchiseeTransactionTypeListId", "Name");
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.IsCBCredit == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");
            return View(new JKViewModels.Franchise.FranchiseeTransactionViewModel());
        }

        [HttpGet]
        public ActionResult PartialChargeBackCredit()
        {
            ViewBag.ReasonList = new SelectList(FranchiseeService.GetAll_ReasonList(), "FranchiseeManualTrxCreditReasonListId", "Name");
            //ViewBag.FranchiseeTransactionTypeList = new SelectList(FranchiseeService.GetFranchiseeTransactionTypeList(), "FranchiseeTransactionTypeListId", "Name");
            ViewBag.ServiceTypeList = new SelectList(FranchiseeService.GetServiceTypeList().Where(o => o.IsCBCredit == true).OrderBy(x => x.name), "ServiceTypeListId", "Name");
            ViewBag.StatusList = new SelectList(FranchiseeService.GetStatusList(), "StatusListId", "Name");
            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId), "Code", "Name");

            return PartialView("_PartialChargeBackCredit", new FranchiseeChargebackCreditViewModel());
        }

        [HttpGet]
        public JsonResult GetFranchiseeComleteDetail(int FranchiseeId)
        {
            var jsonResult = Json(new { detail = FranchiseeService.GetFranchiseeDetailData(FranchiseeId), fees = FranchiseeService.GetFranchiseeFee(FranchiseeId) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult GetBillingPayInfoFromInvoiceNo(string InvoiceNo, int FranchiseeId)
        {
            FranchiseeBillingPayInfoViewModel result = FranchiseeService.GetFranchiseeBillingPayInfoByInvoiceNo(SelectedRegionId, InvoiceNo, FranchiseeId);
            if (result != null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { InvoiceId = -1, BillingPayId = -1, ChargebackId = -1, ChargebackAmount = 0, ApplyAmount = 0, lstFranchiseeFee = result.lstFranchiseeFee }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FranchiseeChargeBackCreditSave(FranchiseeChargebackCreditViewModel model, FormCollection collection)
        {
            if (collection["SaveNew"] != null)
            {
                FranchiseeService.SaveFranchiseeChargeBackCredit(model, false);
                return RedirectToAction("ChargeBackCredit", "AccountsPayable", new { area = "Portal" });
            }
            else if (collection["SaveClosePP"] != null)
            {
                FranchiseeService.SaveFranchiseeChargeBackCredit(model, false);
                return RedirectToAction("FranchiseeChargebackList", "AccountsPayable", new { area = "Portal" });
            }
            else
            {
                FranchiseeService.SaveFranchiseeChargeBackCredit(model, false);
                return RedirectToAction("FranchiseeChargeback", "AccountsPayable", new { area = "Portal" });
            }
        }

        public ActionResult NegativeDue()
        {
            ViewBag.CurrentMenu = "NegativeDue";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("NegativeDue", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("NegativeDue", "AccountsPayable", new { area = "Portal" }), "NegativeDue");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.apiEndpoint = ConfigurationManager.AppSettings["apiEndpoint"].ToString();

            //TODO: Move api keys to user login session. Token needs to be dynamic
            /* Api keys and token needs to be pulled at the time user logs/starts the session and passed back to the client. 
             * I will add it on my next commit GermanSosa 11/5/2018 */
            ViewBag.apiKey= "KwyTNnJBndBsdJQ9341kzw==";
            ViewBag.apiToken = "sH6SsZOCC0Yn0OZxNodCi6mBd35N9jtTVBhjsIwxk3JzLbz7EsT3Cct,,trkV2ietoEB4hFyNWa14rinXHyW4O2fcZMUKaulTCW~3Qa9JdBAdJsEKWbER2E63DoXv3mcnCz9wzv9WHGX3K8KjFxEKr8Eg6oSuSl3ec4mSjmzidc3gNJPbaKB6N9wYIXbMzw~VyJ1~K8bK2QMRtpwlDo94LlTtbYAqwb0UciN1AsKUho2KcDC4A85sb7HkLDa6wHdk6RD6t1w5YZ,,gy02QED0zCTDiOmvbVLUzdNSSNc3nPTGmEUzXhysxSHwTDZro9n4lJHfToqGkZ1ch4Y2H9LMxu77OWJPnUlO85g8xBt4cT3c=";

            return View();
        }

        //public ActionResult NegativeDueData(string regionIds, int franchiseeStatus)
        //{
        //    if (regionIds == "null") regionIds = null;

        //    try
        //    {
        //        var response = AccountPayableService.GetNagativeDue(franchiseeStatus, regionIds);
        //        var result = from f in response
        //                     orderby f.FranchiseeNo ascending
        //                     select new
        //                     {
        //                         FranchiseeNo = f.FranchiseeNo,
        //                         FranchiseeName = f.Name,
        //                         Balance = (decimal)(f.Balance != null ? f.Balance : 0.00m),
        //                         Name = f.Name,
        //                         FranchiseeId = f.FranchiseeId,
        //                         RegionName = f.RegionName,
        //                         NegativeDueId = f.NegativeDueId
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

        public JsonResult AddNegativeDue(decimal ParAmt, int NdId)
        {
            var data = AccountPayableService.AddNegativeDue(ParAmt, NdId);
            return Json(new { aaData = "", success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNegativeDuecheckRoll(string checkRollId)
        {
            if (checkRollId != "")
            {
                int[] NdId = checkRollId.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                foreach (int id in NdId)
                {
                    var data = AccountPayableService.AddNegativeDueRoll(id);
                }
            }
            return Json(new { aaData = "", success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNegativeDueFullPayment(string checkfullId)
        {
            if (checkfullId != "")
            {
                int[] NdId = checkfullId.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                foreach (int id in NdId)
                {
                    var data = AccountPayableService.AddNegativeDueFullPayment(id);
                }
            }
            return Json(new { aaData = "", success = true }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult NegativeDueFinalize(int selectedPeriodId)
        {
            bool dataResult = AccountPayableService.UpdatePeriodClosed(selectedPeriodId);
            //return Json(new { aaData = "", success = true }, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(new { aaData = "", success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;


        }


        public JsonResult GetNegativeDue(int selectedPeriodId)
        {

            var resultdata = AccountPayableService.GetPeriodClosed(selectedPeriodId);

            var jsonResult = Json(new { aaData = resultdata }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult AccountingFeeRebate()
        {
            ViewBag.CurrentMenu = "AccountingFeeRebate";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("AccountingFeeRebate", "AccountsPayable", new { area = "Portal" }), "Accounts Payable");
            BreadCrumb.Add(Url.Action("TurnaroundProcess", "AccountsPayable", new { area = "Portal" }), "PayBill");
            BreadCrumb.Add(Url.Action("AccountingFeeRebate", "AccountsPayable", new { area = "Portal" }), "AccountingFeeRebate");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;

            JKViewModels.AccountsPayable.AccountingFeeRebateFullViewModel AFeemodel = new JKViewModels.AccountsPayable.AccountingFeeRebateFullViewModel();

            AFeemodel = AccountPayableService.GetAccountingFeeRebate(SelectedRegionId.ToString());
            ViewData.Model = AFeemodel;

            return View(ViewData.Model);
        }

        [HttpPost]
        public ActionResult AccountingFeeRebate(FormCollection frm)
        {

            int periodClosedId = Convert.ToInt32(frm["periodClosedId"]);
            PeriodClosed periodClosed = new PeriodClosed();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                periodClosed = context.PeriodCloseds.Where(p => p.PeriodClosedId == periodClosedId).FirstOrDefault();
            }

            AccountingFeeRebateFullViewModel AFeemodel = new AccountingFeeRebateFullViewModel();
            AFeemodel = AccountPayableService.GetAccountingFeeRebate(SelectedRegionId.ToString());
            string AcctFeeRebatePr = AccountPayableService.InsertFranchiseeAccountingFeeRebate(AFeemodel.AccountingFeeRebateList, Convert.ToInt32(periodClosed.PeriodId));
            return RedirectToAction("RunPaymentCheck", "AccountsPayable", new { area = "Portal", CheckType = 2 });

        }

        public JsonResult GetPreviousPeriodInfo(int selectedPeriodId)
        {

            var resultdata = AccountPayableService.GetPreviousPeriod(selectedPeriodId, SelectedRegionId);

            var jsonResult = Json(new { aaData = resultdata }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
    }
}