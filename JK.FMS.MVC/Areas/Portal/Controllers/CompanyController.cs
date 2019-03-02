//using JKApi.Service.Service.Administration.Company;
//using JKApi.Service.Helper;
using Application.Web.Core;
using JKApi.Core;
using JKApi.Core.Common;
using JKApi.Data.DAL;
//using JKApi.Service.ServiceContract.JKControl;
using JKApi.Service;
//using JKApi.Service.Service;
//using JKViewModels.Customer;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service.Administration.Company;
using JKApi.Service.Service.Company;
using JKApi.Service.ServiceContract.AccountPayable;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.CustomerInvoice;
using JKApi.Service.ServiceContract.Franchisee;
//using MoreLinq;
//using System.Data.Entity.Core.Objects;
//using System.Globalization;
using JKViewModels;
using JKViewModels.Administration.Company;
//using AutoMapper;
using JKViewModels.Company;
using MvcBreadCrumbs;
//using System.Net.Mail;
//using System.Web.Script.Serialization;
//using System.Web.Script;
//using iTextSharp.text.pdf.parser;
//using JKApi.Service.Service.TaxAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
//using System.Web;
using System.Web.Mvc;

//using Newtonsoft.Json.Linq;


namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Portal", Order = 0)]
    public class CompanyController : ViewControllerBase
    {

        JKApi.Service.CPeriodClosed periodClose = new JKApi.Service.CPeriodClosed();

        public CompanyController(ICacheProvider CacheProvider, ICustomerService customerService,
            ICustomerInvoiceService customerinvoiceservice, ICompanyService companyservice,
            IAccountPayableService accountPayableService, ICommonService commonService,
            IFranchiseeService franchiseeService
            , IAccountReceivableService _accountreceivableservice)
        {
            CustomerService = customerService;
            AccountPayableService = accountPayableService;
            AccountReceivableService = _accountreceivableservice;
            _customerinvoiceservice = customerinvoiceservice;
            _commonService = commonService;
            _companyService = companyservice;
            FranchiseeService = franchiseeService;
            ViewBag.HMenu = "Company";
        }

        // GET: Portal/Company
        public ActionResult Index()
        {
            ViewBag.CurrentMenu = "CompanyBankAccounts";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData =
                _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View(model);
        }

        [HttpPost]
        public JsonResult PayeeAutoComplete(string namePrefix, int limit)
        {
            if (namePrefix == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var Parentname = _companyService.SearchPayeeList(namePrefix, limit);

            return this.Json(Parentname, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Deposits()
        {
            ViewBag.CurrentMenu = "CompanyBankAccounts";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Bank Account");
            BreadCrumb.Add(Url.Action("Deposits", "Company", new { area = "Portal" }), "Deposits");

            var DepositTypeList = _companyService.GetDepositTypeList();
            ViewBag.DepositTypeList = new SelectList(DepositTypeList, "DepositTypeId", "Name");
            var BankList = CustomerService.GetBanksForRegion();
            ViewBag.BankList = new SelectList(BankList, "BankId", "Name");

            List<ServiceTypeList> lstServiceTypeList = CustomerService.GetServiceTypeList()
                .Where(x => x.IsDeposit == true).OrderBy(o => o.name).ToList();
            ViewBag.ServiceTypeList = new SelectList(lstServiceTypeList, "ServiceTypeListId", "Name");

            return View();
        }

        [HttpGet]
        public JsonResult DepositList(DateTime from, DateTime to)
        {
            List<portal_spGet_C_DepositList_Result> lst = _companyService.GetDepositList(from, to);

            var jsonResult = Json(new { aaData = lst, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        private DepositViewModel GetDepositViewModel(FormCollection frm)
        {
            int intRes = 0;
            decimal decRes = 0;
            DateTime dateRes = DateTime.UtcNow;

            int regionId = SelectedRegionId; /*int.TryParse(frm["regionId"], out intRes) ? intRes : 0;*/
            int typeListId = int.TryParse(frm["typeListId"], out intRes) ? intRes : 0;
            int classId = int.TryParse(frm["classId"], out intRes) ? intRes : 0;
            int bankId = int.TryParse(frm["BankList"], out intRes) ? intRes : 0;
            int depositTypeListId = int.TryParse(frm["DepositTypeList"], out intRes) ? intRes : 0;
            decimal amount = decimal.TryParse(frm["txtAmount"], out decRes) ? decRes : 0.00M;
            string desc = frm["txtDescription"];
            string refNo = frm["txtReferenceNo"];
            DateTime? date = DateTime.TryParse(frm["dtDepositDate"], out dateRes) ? (DateTime?)dateRes : null;

            DepositViewModel dvm = new DepositViewModel();
            dvm.RegionId = regionId;
            dvm.TypeListId = typeListId;
            dvm.ClassId = classId;
            dvm.BankId = bankId;
            dvm.DepositTypeId = depositTypeListId;
            dvm.Amount = amount;
            dvm.DepositDate = (DateTime)date;
            dvm.Description = desc;
            dvm.ReferenceNo = refNo;
            dvm.BankId = bankId;
            dvm.CreatedBy = this.LoginUserId;
            dvm.CreatedDate = DateTime.UtcNow;

            return dvm;
        }

        [HttpPost]
        public ActionResult InsertDeposit(FormCollection frm)
        {
            int _DepositServiceTypeListId = !String.IsNullOrEmpty(frm["OD_ServiceTypeListIdTrans"])
                ? int.Parse(frm["OD_ServiceTypeListIdTrans"].ToString())
                : 0;
            string _DepositPayeeType =
                !String.IsNullOrEmpty(frm["OD_txtPayeeType"]) ? frm["OD_txtPayeeType"].ToString() : "";
            int _DepositPayeeId = !String.IsNullOrEmpty(frm["OD_txtPayeeId"])
                ? int.Parse(frm["OD_txtPayeeId"].ToString())
                : 0;
            string _DepositPayeeName =
                !String.IsNullOrEmpty(frm["OD_txtPayeeName"]) ? frm["OD_txtPayeeName"].ToString() : "";
            string _DepositPayeeNo = !String.IsNullOrEmpty(frm["OD_txtPayeeNumber"])
                ? frm["OD_txtPayeeNumber"].ToString()
                : "";
            string _DepositReason = !String.IsNullOrEmpty(frm["OD_txtReason"]) ? frm["OD_txtReason"].ToString() : "";
            string ChaqueNumber = !String.IsNullOrEmpty(frm["OD_txtReferenceNo"])
                ? frm["OD_txtReferenceNo"].ToString()
                : "";
            decimal ApplyAmount = !String.IsNullOrEmpty(frm["OD_txtAmount"])
                ? decimal.Parse(frm["OD_txtAmount"].ToString())
                : 0;
            DateTime TrxDate = !String.IsNullOrEmpty(frm["OD_txtDate"])
                ? DateTime.Parse(frm["OD_txtDate"].ToString())
                : DateTime.Now;

            AccountReceivableService.InsertOtherDeposit(TrxDate, _DepositPayeeId, _DepositPayeeType, _DepositReason,
                _DepositServiceTypeListId, ApplyAmount, ChaqueNumber, _DepositPayeeName, _DepositPayeeNo);

            //var dvm = GetDepositViewModel(frm);
            //var depositId = _companyService.InsertDeposit(dvm);

            return RedirectToAction("Deposits", "Company", new { area = "Portal" });
        }

        [HttpGet]
        public ActionResult CheckFinalizedReport(string ids = "", bool print = false, bool showSample = false,
            string rt = null, string c = null)
        {
            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ViewBag.IsPrintView = print;
            ViewBag.ShowSample = showSample;

            //if (rt == null || c == null)
            //{
            //    rt = "RunPaymentCheck";
            //    c = "AccountsPayable";
            //}
            //ViewBag.ReturnAction = Url.Action(rt, c, new { area = "Portal" });


            List<CheckViewModelFinalizedReport> vms = new List<CheckViewModelFinalizedReport>();
            foreach (string a in arrids)
            {
                var id = int.Parse(a.Trim());
                var vm = _companyService.GetCheckDetailsFinalizedReport(id);
                vm.FranchiseeReport = AccountPayableService.GetFranchiseeReportFinalizedDetailsForCheck(id);
                vm.ManualCheck = _companyService.GetManualCheckForCheck(id);
                vm.Calibration = _companyService.GetCheckCalibrationForRegion();
                vms.Add(vm);
            }

            return View(vms);
        }


        public ActionResult Check(string ids = "", bool print = false, bool showSample = false, string rt = null,
            string c = null, int CheckTypeId = -1)
        {
            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ViewBag.IsPrintView = print;
            ViewBag.ShowSample = showSample;

            if (rt == null || c == null)
            {
                rt = "RunPaymentCheck";
                c = "AccountsPayable";
            }

            ViewBag.ReturnAction = Url.Action(rt, c, new { area = "Portal" });

            bool IsSysGenerated = CheckTypeId != -1 ? AccountPayableService.IsCheckSystemGenerated(CheckTypeId) : false;

            List<CheckViewModel> vms = new List<CheckViewModel>();
            foreach (string a in arrids)
            {
                var id = int.Parse(a.Trim());
                var vm = _companyService.GetCheckDetails(id);

                if (CheckTypeId == 3)
                {
                    vm.TurnAround = AccountPayableService.GetTurnAroundDetailsForCheck(id);
                    vm.CheckTemplate = 3; /*Trun around*/
                }
                else if (CheckTypeId == 2)
                {
                    vm.AccountingFeeebateDetails = AccountPayableService.GetAccountingFeeRebateDetailsForCheck(id);
                    vm.CheckTemplate = 4; /*Acct Fee Rebate*/
                }
                else
                {
                    if (IsSysGenerated)
                    {
                        vm.APBillViewModel = _companyService.GetAPBillCheck(id);
                        vm.IsCheckSystemGenerated = IsSysGenerated;
                    }
                    else
                    {
                        vm.ManualCheck = _companyService.GetManualCheckForCheck(id);
                    }
                    vm.CheckTemplate = 1;
                }

                vm.Calibration = _companyService.GetCheckCalibrationForRegion();
                vms.Add(vm);


            }

            return View(vms);
        }

        public ActionResult BankStatement()
        {
            ViewBag.CurrentMenu = "CompanyBankAccounts";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Bank Account");
            BreadCrumb.Add(Url.Action("BankStatement", "Company", new { area = "Portal" }), "Bank Statement");

            ViewBag.OptionList =
                new SelectList(AccountPayableService.GetAll_SearchDateList(), "SearchDateListId", "Name");

            var BankList = CustomerService.GetBanksForRegion();
            ViewBag.BankList = new SelectList(BankList, "BankId", "Name");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = this.SelectedRegionId;
            return View();
        }

        [HttpGet]
        public JsonResult GetBankStatementDetails(int bankId, DateTime startDate, DateTime endDate)
        {
            var results = _companyService.GetBankStatementDetails(bankId, startDate, endDate);

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankStatementDetailList(string regionIds, DateTime from, DateTime to)
        {
            var response = _companyService.GetBankStatementDetailList(regionIds, from, to)
                .OrderBy(o => o.TransactionDate).ToList(); //.ThenBy(t=>t.AmountTypeListId).ThenBy(g => g.Total);


            List<BBankStatmentViewModel> lstBBankStatment = new List<BBankStatmentViewModel>();
            BBankStatmentViewModel oBBankStatment = new BBankStatmentViewModel();
            decimal _balanceAMT = 0;

            foreach (var item in response)
            {
                oBBankStatment = new BBankStatmentViewModel();
                oBBankStatment.TransactionDate = item.TransactionDate;
                oBBankStatment.TrxType = item.TrxType;
                oBBankStatment.Name = item.Name;
                oBBankStatment.ReferenceNumber = item.ReferenceNumber;
                oBBankStatment.Notes = item.Notes;
                oBBankStatment.AmountTypeListId = item.AmountTypeListId;

                if (item.TrxType.Contains("Forwarded Balance"))
                {
                    _balanceAMT = _balanceAMT + (decimal)item.Total;
                }
                else
                {
                    if (item.AmountTypeListId == 2)
                    {
                        oBBankStatment.Debit = item.Total;
                        _balanceAMT = _balanceAMT + (decimal)item.Total;
                    }
                    else
                    {
                        oBBankStatment.Credit = item.Total;
                        _balanceAMT = _balanceAMT - (decimal)item.Total;
                    }
                }

                oBBankStatment.Balance =
                    _balanceAMT; //(item.AmountTypeListId == 1 ? Total + f.Total : Total - f.Total),// != null ? String.Format("{0:C}", f.Total) : "$0.00",
                oBBankStatment.PayeeNo = item.PayeeNo;
                oBBankStatment.Code = item.code;
                lstBBankStatment.Add(oBBankStatment);

            }

            //var result = from f in response
            //             select new
            //             {
            //                 TransactionDate = f.TransactionDate,
            //                 TrxType = f.TrxType,
            //                 Name = f.Name,
            //                 ReferenceNumber = f.ReferenceNumber,
            //                 Notes = f.Notes,
            //                 AmountTypeListId = f.AmountTypeListId,
            //                 Total = (f.AmountTypeListId==1?Total + f.Total: Total - f.Total),// != null ? String.Format("{0:C}", f.Total) : "$0.00",
            //                 PayeeNo = f.PayeeNo,
            //             };


            return Json(new
            {
                aadata = lstBBankStatment.OrderBy(g => g.TransactionDate),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddBankAccount()
        {
            ViewBag.CurrentMenu = "CompanyBankAccounts";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Bank Account");
            BreadCrumb.Add(Url.Action("AddBankStatement", "Company", new { area = "Portal" }), "Add Bank Statement");
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

            return View();
        }

        [HttpPost]
        public ActionResult AddBankAccount(BankViewModel bankviewmodel, FormCollection collection)
        {
            if (bankviewmodel.AccountNumber != null)
            {
                bankviewmodel.CreatedDate = DateTime.Now;
                bankviewmodel.ModifiedDate = DateTime.Now;
                bankviewmodel = CustomerService.SaveBank(bankviewmodel.ToModel<Bank, BankViewModel>())
                    .ToModel<BankViewModel, Bank>();
                var BankTypeList = CustomerService.GetBankTypeList();
                if (BankTypeList.Count > 0)
                {
                    ViewBag.BankTypeList = new SelectList(BankTypeList, "BankTypeListId", "Name");
                }

                var State = CustomerService.GetStateList();
                if (State != null)
                {
                    ViewBag.State = new SelectList(State, "StateListIportal_spGet_BankStatmentd", "Name");
                }
            }

            return RedirectToAction("AddBankAccount", "Company", new { area = "Portal" });
        }

        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = "All Transactions" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text", list.SelectedValue);
        }

        [HttpGet]
        public ActionResult General()
        {
            ViewBag.HMenu = "Company";
            ViewBag.CurrentMenu = "CompanyGeneralLedger";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "General Ledgers");
            BreadCrumb.Add(Url.Action("General", "Company", new { area = "Portal" }), "Chart of Accounts");


            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            var TransactionsTypeList = CustomerService.GetMasterTrxTypeListsForCustomers()
                .Where(x => x.CustomerDetailView == true).ToList();
            if (TransactionsTypeList != null)
            {
                ViewBag.TransactionsTypeList =
                    AddFirstItem(new SelectList(TransactionsTypeList, "MasterTrxTypeListId", "Name"));
            }

            //ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name");

            ViewBag.selectedRegionId = SelectedRegionId;


            FullLedgerAccountViewModel oFullGenralLedgerAccount = new FullLedgerAccountViewModel();
            oFullGenralLedgerAccount.CLadgerAccount = new CommonLadgerAccountViewModel();
            oFullGenralLedgerAccount.GeneralLadgerAccountList = new List<CommonLadgerAccountViewModel>();
            oFullGenralLedgerAccount.GLAccountTypeList = new GLAccountTypeListViewModel();

            List<SelectListItem> selectlistGLAccountTypeList = new List<SelectListItem>();
            //selectlistGLAccountTypeList.Add(new SelectListItem { Text = "Add New", Value = "-1" });
            selectlistGLAccountTypeList.AddRange(_customerinvoiceservice.GetGLAccountTypeList().ToList()
                .Select(one => new SelectListItem { Text = one.Name, Value = one.GLAccountTypeListId.ToString() })
                .ToList());


            ViewBag.GLAccountTypeList = new SelectList(selectlistGLAccountTypeList, "Value", "Text");

            //ViewBag.GenralLedgers = _customerinvoiceservice.GetChartofAccountList(); //oFullGenralLedgerAccount.GeneralLadgerAccountList;
            //List<SelectListItem> selectlistLedgerAccounts = new List<SelectListItem>();
            //selectlistLedgerAccounts.AddRange(_customerinvoiceservice.GetLedgerMasterAccountList().Select(one => new SelectListItem { Text = (one.AccountNo + ": " + one.Name), Value = one.LedgerAcctId.ToString() }).ToList());

            ViewBag.GLLedgerAccounts =
                new SelectList(
                    _customerinvoiceservice.GetLedgerMasterAccountList().Select(one => new SelectListItem
                    { Text = (one.AccountNo + ": " + one.Name), Value = one.LedgerAcctId.ToString() }).ToList(),
                    "Value", "Text");

            //List<SelectListItem> lstselectlistMasterAccounts = _customerinvoiceservice.GetLedgerAcct().ToList().Select(one => new SelectListItem { Text = one.GL_Name, Value = one.GLAccountTypeListId.ToString() }).ToList();

            //ViewBag.LedgerAccounts = _companyService.GetChartofAccounts();


            return View(oFullGenralLedgerAccount);
        }


        [HttpGet]
        public JsonResult GeneralLedgerList(string selectedRegion, DateTime? StartDate, DateTime? EndDate,
            int month = 0, int year = 0)
        {
            return Json(_companyService.GetChartofAccounts(selectedRegion, StartDate, EndDate, month, year),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult LadgerAccountTransactions(int ledgerid, bool issubaccount, string selectedRegion,
            DateTime? StartDate, DateTime? EndDate, bool debit, bool credit, int month = 0, int year = 0)
        {
            var result = _companyService.GetLedgerMasterTrasactionList(ledgerid, issubaccount, selectedRegion,
                StartDate, EndDate, month, year);
            if (debit == true && credit == false)
            {
                result = result.Where(x => x.AmountTypeListId == 2).ToList();
            }

            if (debit == false && credit == true)
            {
                result = result.Where(x => x.AmountTypeListId == 1).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult General(FullLedgerAccountViewModel fullledgeraccountviewmodel, FormCollection collection)
        {
            LedgerAccount oLedgerAccount;
            int GLAccountTypeListId = Convert.ToInt32(collection["GLAccountTypeList"]);
            if (fullledgeraccountviewmodel.CLadgerAccount.IsSubAccount)
            {
                oLedgerAccount = new LedgerAccount();
                oLedgerAccount.ParentLedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.PerentAccountId;
                oLedgerAccount.LedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
                oLedgerAccount.LedgerName = fullledgeraccountviewmodel.CLadgerAccount.Name;
                oLedgerAccount.LedgerNumber = fullledgeraccountviewmodel.CLadgerAccount.AccountNo;
                oLedgerAccount.LedgerDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
                oLedgerAccount.GLAccountTypeListId = GLAccountTypeListId;
                oLedgerAccount.IsActive = true;
                oLedgerAccount.IsDelete = false;
                oLedgerAccount.ModifiedBy = LoginUserId;
                oLedgerAccount.ModifiedDate = DateTime.Now;
                oLedgerAccount.CreatedBy = LoginUserId;
                oLedgerAccount.CreatedDate = DateTime.Now;
                _customerinvoiceservice.SaveLedgerAccount(oLedgerAccount);
            }
            else
            {
                oLedgerAccount = new LedgerAccount();
                oLedgerAccount.ParentLedgerAccountId = 0;
                oLedgerAccount.LedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
                oLedgerAccount.LedgerName = fullledgeraccountviewmodel.CLadgerAccount.Name;
                oLedgerAccount.LedgerNumber = fullledgeraccountviewmodel.CLadgerAccount.AccountNo;
                oLedgerAccount.LedgerDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
                oLedgerAccount.GLAccountTypeListId = GLAccountTypeListId;
                oLedgerAccount.IsActive = true;
                oLedgerAccount.IsDelete = false;
                oLedgerAccount.ModifiedBy = LoginUserId;
                oLedgerAccount.ModifiedDate = DateTime.Now;
                oLedgerAccount.CreatedBy = LoginUserId;
                oLedgerAccount.CreatedDate = DateTime.Now;
                _customerinvoiceservice.SaveLedgerAccount(oLedgerAccount);
            }



            return RedirectToAction("General", "Company", new { area = "Portal" });
        }


        [HttpPost]
        public JsonResult AddGeneralLedgerAccountType(FormCollection collection)
        {

            string GLAccountTypeListName = Convert.ToString(collection["GLAccountTypeList.Name"]);

            GLAccountTypeList oGLAccountTypeList = new GLAccountTypeList();
            oGLAccountTypeList.Name = GLAccountTypeListName;
            _customerinvoiceservice.SaveGLAccountTypeList(oGLAccountTypeList);
            List<SelectListItem> selectlistGLAccountTypeList = new List<SelectListItem>();
            selectlistGLAccountTypeList.Add(new SelectListItem { Text = "Add New", Value = "-1" });
            selectlistGLAccountTypeList.AddRange(_customerinvoiceservice.GetGLAccountTypeList().ToList()
                .Select(one => new SelectListItem { Text = one.Name, Value = one.GLAccountTypeListId.ToString() })
                .ToList());
            //ViewBag.GLAccountTypeList = new SelectList(selectlistGLAccountTypeList, "Value", "Text");
            return Json(new SelectList(selectlistGLAccountTypeList, "Value", "Text",
                oGLAccountTypeList.GLAccountTypeListId));

        }


        [HttpGet]
        public JsonResult GetGeneralLedgerAccountForEdit(int accid, bool issubacct)
        {

            CommonLadgerAccountViewModel GetLedgerAccountList =
                _customerinvoiceservice.GetLedgerAccountDetailforEdit(accid, issubacct);



            //string GLAccountTypeListName = Convert.ToString(collection["GLAccountTypeList.Name"]);

            //GLAccountTypeList oGLAccountTypeList = new GLAccountTypeList();
            //oGLAccountTypeList.Name = GLAccountTypeListName;
            //_customerinvoiceservice.SaveGLAccountTypeList(oGLAccountTypeList);
            //List<SelectListItem> selectlistGLAccountTypeList = new List<SelectListItem>();
            //selectlistGLAccountTypeList.Add(new SelectListItem { Text = "Add New", Value = "-1" });
            //selectlistGLAccountTypeList.AddRange(_customerinvoiceservice.GetGLAccountTypeList().ToList().Select(one => new SelectListItem { Text = one.Name, Value = one.GLAccountTypeListId.ToString() }).ToList());
            ////ViewBag.GLAccountTypeList = new SelectList(selectlistGLAccountTypeList, "Value", "Text");
            return Json(GetLedgerAccountList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddGeneralLedgerAccount(FullLedgerAccountViewModel fullledgeraccountviewmodel,
            FormCollection collection)
        {
            LedgerAccount oLedgerAccount;
            int GLAccountTypeListId = Convert.ToInt32(collection["GLAccountTypeList"]);
            if (fullledgeraccountviewmodel.CLadgerAccount.IsSubAccount)
            {
                oLedgerAccount = new LedgerAccount();
                oLedgerAccount.ParentLedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.PerentAccountId;
                oLedgerAccount.LedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
                oLedgerAccount.LedgerName = fullledgeraccountviewmodel.CLadgerAccount.Name;
                oLedgerAccount.LedgerNumber = fullledgeraccountviewmodel.CLadgerAccount.AccountNo;
                oLedgerAccount.LedgerDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
                oLedgerAccount.GLAccountTypeListId = fullledgeraccountviewmodel.CLadgerAccount.TypeId;
                oLedgerAccount.IsActive = true;
                oLedgerAccount.IsDelete = false;
                oLedgerAccount.ModifiedBy = LoginUserId;
                oLedgerAccount.ModifiedDate = DateTime.Now;
                oLedgerAccount.CreatedBy = LoginUserId;
                oLedgerAccount.CreatedDate = DateTime.Now;
                _customerinvoiceservice.SaveLedgerAccount(oLedgerAccount);
            }
            else
            {
                oLedgerAccount = new LedgerAccount();
                oLedgerAccount.ParentLedgerAccountId = 0;
                oLedgerAccount.LedgerAccountId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
                oLedgerAccount.LedgerName = fullledgeraccountviewmodel.CLadgerAccount.Name;
                oLedgerAccount.LedgerNumber = fullledgeraccountviewmodel.CLadgerAccount.AccountNo;
                oLedgerAccount.LedgerDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
                oLedgerAccount.GLAccountTypeListId = fullledgeraccountviewmodel.CLadgerAccount.TypeId;
                oLedgerAccount.IsActive = true;
                oLedgerAccount.IsDelete = false;
                oLedgerAccount.ModifiedBy = LoginUserId;
                oLedgerAccount.ModifiedDate = DateTime.Now;
                oLedgerAccount.CreatedBy = LoginUserId;
                oLedgerAccount.CreatedDate = DateTime.Now;
                _customerinvoiceservice.SaveLedgerAccount(oLedgerAccount);
            }

            List<SelectListItem> selectlistLedgerAccounts = new List<SelectListItem>();
            selectlistLedgerAccounts.AddRange(_customerinvoiceservice.GetLedgerMasterAccountList().Select(one =>
                    new SelectListItem { Text = (one.AccountNo + ": " + one.Name), Value = one.LedgerAcctId.ToString() })
                .ToList());

            return Json(new SelectList(selectlistLedgerAccounts, "Value", "Text"));

            //List<SelectListItem> selectlistLedgerAccounts = new List<SelectListItem>();
            //if (fullledgeraccountviewmodel.CLadgerAccount.IsSubAccount)
            //{


            //    LedgerSubAcct oLedgerSubAcct = new LedgerSubAcct();
            //    oLedgerSubAcct.LedgerAcctId = fullledgeraccountviewmodel.CLadgerAccount.PerentAccountId;
            //    oLedgerSubAcct.LedgerSubAcctId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
            //    oLedgerSubAcct.GLSubAcct_Name = fullledgeraccountviewmodel.CLadgerAccount.Name;
            //    oLedgerSubAcct.GLSubAcct_Number = Convert.ToInt32(fullledgeraccountviewmodel.CLadgerAccount.AccountNo);
            //    oLedgerSubAcct.LedgerSubAcctDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
            //    _customerinvoiceservice.SaveLedgerSubAcct(oLedgerSubAcct);

            //    selectlistLedgerAccounts.AddRange(_customerinvoiceservice.GetLedgerMasterAccountList().Select(one => new SelectListItem { Text = (one.AccountNo + ": " + one.Name), Value = one.LedgerAcctId.ToString() }).ToList());
            //    return Json(new SelectList(selectlistLedgerAccounts, "Value", "Text"));
            //}
            //else
            //{


            //    LedgerAcct oLedgerAcct = new LedgerAcct();
            //    oLedgerAcct.LedgerAcctId = fullledgeraccountviewmodel.CLadgerAccount.AccountId;
            //    oLedgerAcct.GL_Name = fullledgeraccountviewmodel.CLadgerAccount.Name;
            //    oLedgerAcct.GL_Number = fullledgeraccountviewmodel.CLadgerAccount.AccountNo.ToString();
            //    oLedgerAcct.GLAccountTypeListId = fullledgeraccountviewmodel.CLadgerAccount.TypeId;
            //    oLedgerAcct.LedgerAcctDescription = fullledgeraccountviewmodel.CLadgerAccount.Description;
            //    _customerinvoiceservice.SaveLedgerAcct(oLedgerAcct);

            //    selectlistLedgerAccounts.AddRange(_customerinvoiceservice.GetLedgerMasterAccountList().Select(one => new SelectListItem { Text = (one.AccountNo + ": " + one.Name), Value = one.LedgerAcctId.ToString() }).ToList());

            //    return Json(new SelectList(selectlistLedgerAccounts, "Value", "Text"));
            //}
        }


        //




        #region :: Closed Period ::

        public ActionResult CloseCurrentPeriod()
        {
            ViewBag.CurrentMenu = "CloseCurrentPeriod";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;

            int selectedPeriod = Session["SelectedPeriodId"] != null ? Convert.ToInt32(Session["SelectedPeriodId"]) : -1;


            //ClosedPeriodViewModel oCP = AccountReceivableService.GetClosedPeriodForClose(SelectedRegionId, selectedPeriod);

            periodClose = JKApi.Service.CPeriodClosed.GetPeriodToClose(SelectedRegionId, selectedPeriod);

            ViewBag.Period = "";
            ViewBag.ResultMsg = "";
            ViewBag.ResultMsgConfig = "";
            ViewBag.IsVerified = false;
            if (periodClose != null)
            {
                ViewBag.IsVerified = false;
                ViewBag.ResultMsgConfig = "Do you want to close Period: " + periodClose.BillMonth + "/" + periodClose.BillYear;
                ViewBag.Period = periodClose.BillMonth + "/" + periodClose.BillYear;

                if (periodClose.ChargebackFinalized == 1 && periodClose.FranchiseeReport == 1 && periodClose.MonthlyBillRun == 1)
                {
                    ViewBag.ResultMsg = "Do you want to close Period: " + periodClose.BillMonth + "/" + periodClose.BillYear;
                    ViewBag.IsVerified = true;
                }
                else
                {
                    string _finA = "";
                    if (periodClose.ChargebackFinalized != 1)
                        _finA = "Chargeback" + ",";
                    if (periodClose.FranchiseeReport != 1)
                        _finA += "Franchisee Report" + ",";
                    if (periodClose.MonthlyBillRun != 1)
                        _finA += "Monthly BillRun" + ",";

                    ViewBag.ResultMsg = "Please finalize " + _finA.TrimEnd(',') + " before you can close period";
                }
            }

            return View();
        }

        public JsonResult CloseCurrentPeriodData()
        {

            int selectedPeriod = Session["SelectedPeriodId"] != null ? Convert.ToInt32(Session["SelectedPeriodId"]) : -1;
            periodClose = JKApi.Service.CPeriodClosed.GetPeriodToClose(SelectedRegionId, selectedPeriod);

            bool isEarliest = JKApi.Service.CPeriodClosed.Validate_EarliestPeriod(SelectedRegionId, selectedPeriod);

            ViewBag.PeriodClosedId = 0;
            ViewBag.Period = "";
            ViewBag.ResultMsg = "";

            if (periodClose != null)
            {
                ViewBag.Period = periodClose.BillMonth + "/" + periodClose.BillYear;
                ViewBag.IsVerified = false;
                ViewBag.ResultMsgConfig = "Close Period: " + periodClose.BillMonth + "/" + periodClose.BillYear;
                ViewBag.PeriodClosedId = periodClose.PeriodClosedId;

                if (!isEarliest)
                {
                    ViewBag.ResultMsg = "Period " + periodClose.BillMonth + "/" + periodClose.BillYear + " cannot be closed. Please close previous open period first.";
                    ViewBag.IsVerified = false;
                }
                else if (periodClose.Closed == 1)
                {
                    ViewBag.ResultMsg = "Period " + periodClose.BillMonth + "/" + periodClose.BillYear + " has already been closed.";
                    ViewBag.IsVerified = false;

                }
                else if (periodClose.ChargebackFinalized == 1 && periodClose.FranchiseeReport == 1 && periodClose.MonthlyBillRun == 1)
                {
                    ViewBag.ResultMsg = "Are you sure you want to close period " + periodClose.BillMonth + "/" + periodClose.BillYear + "? You will not be able to undo this action!";
                    ViewBag.IsVerified = true;
                }
                else
                {
                    string _finA = "";
                    if (periodClose.ChargebackFinalized != 1)
                        _finA = "Chargeback" + ",";
                    if (periodClose.FranchiseeReport != 1)
                        _finA += "Franchisee Report" + ",";
                    if (periodClose.MonthlyBillRun != 1)
                        _finA += "Monthly BillRun" + ",";

                    ViewBag.ResultMsg = "Please finalize " + _finA.TrimEnd(',') + " before you can close period";
                }
            }

            return Json(
                new
                {
                    PeriodClosedId = ViewBag.PeriodClosedId,
                    ResultMsg = ViewBag.ResultMsg,
                    IsVerified = ViewBag.IsVerified
                }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CloseCurrentPeriodDataClosed(int PeriodClosedId)
        {
            return Json(new { msg = JKApi.Service.CPeriodClosed.UpdateStatusClosedPeriod(PeriodClosedId, LoginUserId) },
                JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult CompanyAddress(int? CompanyId)
        {
            if (CompanyId == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            var address = _companyService.GetAddress((int)CompanyId).Select(x => new { x.Name, x.Address, x.City, x.State, x.Zip }).FirstOrDefault();
            return this.Json(address, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCompanies(string searchText = "")
        {
            var searchData = _companyService.GetSearchCompanies(searchText);
            return Json(searchData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BankTRXEntry()
        {
            ViewBag.CurrentMenu = "BankTRXEntry";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Company");
            BreadCrumb.Add(Url.Action("Index", "Company", new { area = "Portal" }), "Bank Account");
            BreadCrumb.Add(Url.Action("BankTRXEntry", "Company", new { area = "Portal" }), "Bank TRX Entry");


            ViewBag.PaymentTrxType = new SelectList(_companyService.GetAll_checkbookTransactionTypeList(),
                "CheckBookTransactionTypeListId", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult InsertBankTrx(FormCollection frm)
        {
            int _PaymentTrxTypeId = !String.IsNullOrEmpty(frm["PaymentTrxType"])
                ? int.Parse(frm["PaymentTrxType"].ToString())
                : 0;
            string _DepositPayeeType =
                !String.IsNullOrEmpty(frm["OD_txtPayeeType"]) ? frm["OD_txtPayeeType"].ToString() : "";
            int _DepositPayeeId = !String.IsNullOrEmpty(frm["OD_txtPayeeId"])
                ? int.Parse(frm["OD_txtPayeeId"].ToString())
                : 0;
            string _DepositPayeeName =
                !String.IsNullOrEmpty(frm["OD_txtPayorName"]) ? frm["OD_txtPayorName"].ToString() : "";
            string _DepositPayeeNo = !String.IsNullOrEmpty(frm["OD_txtPayeeNumber"])
                ? frm["OD_txtPayeeNumber"].ToString()
                : "";
            string _DepositReason = !String.IsNullOrEmpty(frm["OD_txtReason"]) ? frm["OD_txtReason"].ToString() : "";
            string ChaqueNumber = !String.IsNullOrEmpty(frm["OD_txtReferenceNo"])
                ? frm["OD_txtReferenceNo"].ToString()
                : "";
            decimal ApplyAmount = !String.IsNullOrEmpty(frm["txtAmount"])
                ? decimal.Parse(frm["txtAmount"].ToString())
                : 0;
            DateTime TrxDate = !String.IsNullOrEmpty(frm["OD_txtDate"])
                ? DateTime.Parse(frm["OD_txtDate"].ToString())
                : DateTime.Now;

            _companyService.InsertBankTrx(TrxDate, _DepositPayeeId, _DepositPayeeType, _DepositReason,
                _PaymentTrxTypeId, ApplyAmount, ChaqueNumber, _DepositPayeeName, _DepositPayeeNo);

            //var dvm = GetDepositViewModel(frm);
            //var depositId = _companyService.InsertDeposit(dvm);

            return RedirectToAction("BankTRXEntry", "Company", new { area = "Portal" });
        }

        public ActionResult RegularCHKAccount()
        {
            //            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            //            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.billMonthsList = GetMonthsList();
            ViewBag.billYearList = GetYearsList().ToList().OrderByDescending(x => x.Text);
            var claimView = ClaimView.Instance;
            int periodId = Convert.ToInt32(claimView.GetCLAIM_SELECTED_PERIOD_ID());
            var periodDetails = claimView.GetCLAIM_PERSON_INFORMATION().lstPeriodAccess
                .Where(x => x.RegionId == SelectedRegionId).ToList();

            ViewBag.selectedRegionId = this.SelectedRegionId;
            ViewBag.periodId = periodId;
            ViewBag.currentPeriodMonth = periodDetails[0].Month;
            ViewBag.currentPeriodYear = periodDetails[0].Year;

            return View();
        }

        [HttpGet]
        public JsonResult GetRegularAccountTransactions(int region, int month, int year)
        {
            var transactions = _companyService.GetRegularAccountTransactions(region, month, year);

            var jsonResult = Json(new
            {
                returnedData = transactions,
            }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }


        [HttpGet]
        public JsonResult ViewTransactionDetail(int region, string checkNum)
        {
            List<RegularCheckRegisterDetailViewModel> ModelData =
                _companyService.GetRegularAccountTransactionDetail(region, checkNum);
            var jsonResult = Json(new
            {
                returnedData = ModelData.OrderByDescending(o => o.CheckNum),
            }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        private SelectList GetMonthsList(int selectedMonth = 0)
        {
            List<SelectListItem> monthsList =
                Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
                { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            return new SelectList(monthsList, "Value", "Text", selectedMonth.ToString());
        }

        private SelectList GetYearsList(int selectedYear = 0)
        {
            var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
            List<SelectListItem> billYearList = new List<SelectListItem>();
            foreach (var y in yearList)
            {
                billYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() });
            }

            return new SelectList(billYearList, "Value", "Text", selectedYear.ToString());
        }

        public JsonResult ExportToExcelBankStatementDetail(string regionIds, DateTime from, DateTime to)
        {
            var response =
                _companyService.GetBankStatementDetailList(regionIds, from,
                    to); //.OrderBy(o=>o.TransactionDate).ThenBy(t=>t.AmountTypeListId).ThenBy(g => g.Total);

            List<BBankStatmentViewModel> lstBBankStatment = new List<BBankStatmentViewModel>();
            BBankStatmentViewModel oBBankStatment = new BBankStatmentViewModel();
            decimal _balanceAMT = 0;

            foreach (var item in response)
            {
                oBBankStatment = new BBankStatmentViewModel();
                oBBankStatment.TransactionDate = item.TransactionDate;
                oBBankStatment.ReferenceNumber = item.ReferenceNumber;
                oBBankStatment.TrxType = item.TrxType;
                oBBankStatment.PayeeNo = item.PayeeNo;
                oBBankStatment.Name = item.Name;
                oBBankStatment.Code = item.code;
                oBBankStatment.Notes = item.Notes;
                //oBBankStatment.AmountTypeListId = item.AmountTypeListId;

                if (item.TrxType.Contains("Forwarded Balance"))
                {
                    _balanceAMT = _balanceAMT + (decimal)item.Total;
                }
                else
                {
                    if (item.AmountTypeListId == 2)
                    {
                        oBBankStatment.Debit = item.Total;
                        _balanceAMT = _balanceAMT + (decimal)item.Total;
                    }
                    else
                    {
                        oBBankStatment.Credit = item.Total;
                        _balanceAMT = _balanceAMT - (decimal)item.Total;
                    }
                }

                oBBankStatment.Balance =
                    _balanceAMT; //(item.AmountTypeListId == 1 ? Total + f.Total : Total - f.Total),// != null ? String.Format("{0:C}", f.Total) : "$0.00",                               
                lstBBankStatment.Add(oBBankStatment);
            }

            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = lstBBankStatment;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=export_excel_bankstatement.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

            grid.RenderControl(htw);

            return Json(sw.ToString(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult ExportExcelGeneralLedgerDatail(string selectedRegion, DateTime? StartDate, DateTime? EndDate,
            int month = 0, int year = 0)
        {
            StringBuilder rowText = new StringBuilder();
            //strinBu rowText = string.Empty;
            var DataModel =
                _companyService.GetLedgerMasterTrasactionListAllDataExcel(0, false, selectedRegion, StartDate, EndDate,
                    month, year);


            if (DataModel != null && DataModel.Count() > 0)
            {

                rowText.Append("<table border='1'>");
                rowText.Append("<tr>");
                rowText.Append("<td style='text-align:center'><b>ACCOUNT NO#<b></td>");
                rowText.Append("<td style='text-align:center'><b>NAME</b></td>");
                rowText.Append("<td style='text-align:center'><b>TYPE</b></td>");
                rowText.Append("<td style='text-align:center'><b>DATE</b></td>");
                rowText.Append("<td style='text-align:center'><b>TRX TYPE</b></td>");
                rowText.Append("<td style='text-align:center'><b>TRX NUMBER</b></td>");
                rowText.Append("<td style='text-align:center'><b>PAYER/PAYEE NO</b></td>");
                rowText.Append("<td style='text-align:center'><b>PAYER/PAYEE</b></td>");
                rowText.Append("<td style='text-align:center'><b>DESCRIPTION</b></td>");
                rowText.Append("<td style='text-align:center'><b>DEBIT</b></td>");
                rowText.Append("<td style='text-align:center'><b>CREDIT</b></td>");
                rowText.Append("</tr>");
                foreach (var item in DataModel)
                {
                    rowText.Append("<tr>");
                    rowText.Append("<td style='text-align:center'>" + item.LedgerAcctNumber + "</td>");
                    rowText.Append("<td>" + item.LedgerAcctNumber + ": " + item.LedgerAcctName + "</td>");
                    rowText.Append("<td>" + item.GLName + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" +
                                   Convert.ToDateTime(item.TransactionDate).ToString("MM-dd-yyyy") + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" +
                                   item.TransactionTypeName + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" + item.TransactionNumber +
                                   "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.Number + "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.Payee_Payer + "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.TransactionDescription + "</td>");
                    rowText.Append("<td style='text-align:right;font-size: 11px;'>" +
                                   (item.AmountTypeListId == 2 ? item.Amount.ToString() : "0.00") + "</td>");
                    rowText.Append("<td style='text-align:right;font-size: 11px;'>" +
                                   (item.AmountTypeListId == 1 ? item.Amount.ToString() : "0.00") + "</td>");
                    rowText.Append("</tr>");
                }
                rowText.Append("</table>");
            }

            var jsondata = Json(rowText.ToString(), JsonRequestBehavior.AllowGet);
            jsondata.MaxJsonLength = 2147483644;
            return jsondata;
        }

        public FileResult ExportExcelGeneralLedgerDatailnew(string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            StringBuilder rowText = new StringBuilder();
            string strHTML = string.Empty;
            var DataModel =
                _companyService.GetLedgerMasterTrasactionListAllDataExcel(0, false, selectedRegion, StartDate, EndDate,
                    month, year);

            int i = 0;
            if (DataModel != null && DataModel.Count() > 0)
            {
                strHTML += "<table  border='1' style='font-size: 11px;'>";
                strHTML += "<tr>";
                strHTML += "<td style='text-align:center'><b>ACCOUNT NO</b></td>";
                strHTML += "<td style='text-align:center'><b>NAME</b></td>";
                strHTML += "<td style='text-align:center'><b>TYPE</b></td>";
                strHTML += "<td style='text-align:center'><b>DATE</b></td>";
                strHTML += "<td style='text-align:center'><b>TRX TYPE</b></td>";
                strHTML += "<td style='text-align:center'><b>TRX NUMBER</b></td>";
                strHTML += "<td style='text-align:center'><b>PAYER/PAYEE NO</b></td>";
                strHTML += "<td style='text-align:center'><b>PAYER/PAYEE</b></td>";
                strHTML += "<td style='text-align:center'><b>DESCRIPTION</b></td>";
                strHTML += "<td style='text-align:center'><b>DEBIT</b></td>";
                strHTML += "<td style='text-align:center'><b>CREDIT</b></td>";
                strHTML += "<td style='text-align:center'><b>NET</b></td>";
                strHTML += "</tr>";

                foreach (var item in DataModel)
                {
                    strHTML += "<tr>";
                    strHTML += "<td style='text-align:center;'>" + item.LedgerAcctNumber + "</td>";
                    strHTML += "<td>" + item.LedgerAcctNumber + ": " + item.LedgerAcctName + "</td>";
                    strHTML += "<td>" + item.GLName + "</td>";
                    strHTML += "<td style='text-align:center;'>" + Convert.ToDateTime(item.TransactionDate).ToString("MM-dd-yyyy") + "</td>";
                    strHTML += "<td style='text-align:center;'>" + item.TransactionTypeName + "</td>";
                    strHTML += "<td style='text-align:center;'>" + item.TransactionNumber + "</td>";
                    strHTML += "<td>" + item.Number + "</td>";
                    strHTML += "<td>" + item.Payee_Payer + "</td>";
                    strHTML += "<td>" + item.TransactionDescription + "</td>";
                    strHTML += "<td style='text-align:right;'>" + (item.AmountTypeListId == 2 ? item.Amount.ToString() : "0.00") + "</td>";
                    strHTML += "<td style='text-align:right;'>" + (item.AmountTypeListId == 1 ? item.Amount.ToString() : "0.00") + "</td>";

                    if (item.AmountTypeListId == 2)
                    {
                        strHTML += "<td style='text-align:right;'>" + item.Amount.ToString() + "</td>";
                    }
                    else if (item.AmountTypeListId == 1)
                    {
                        strHTML += "<td style='text-align:right;color: red;'>-" + item.Amount.ToString() + "</td>";
                    }
                    else
                    {
                        strHTML += "<td style='text-align:right;'></td>";
                    }
                    strHTML += "</tr>";
                    i = i + 1;
                }
                strHTML += "</table>";
            }
            string strfilename = "GeneralAccountDetails_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            return File(Encoding.ASCII.GetBytes(strHTML), "application/vnd.ms-excel", strfilename + ".xls");
        }
        public ActionResult ExportExcelGeneralLedgerDatailnew123(string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            StringBuilder rowText = new StringBuilder();
            //strinBu rowText = string.Empty;
            var DataModel =
                _companyService.GetLedgerMasterTrasactionListAllDataExcel(0, false, selectedRegion, StartDate, EndDate,
                    month, year);

            int i = 0;
            if (DataModel != null && DataModel.Count() > 0)
            {
                rowText.Append("<table>");
                //rowText.Append("<tr>");
                //rowText.Append("<td style='text-align:center'><b>ACCOUNT NO#<b></td>");
                //rowText.Append("<td style='text-align:center'><b>NAME</b></td>");
                //rowText.Append("<td style='text-align:center'><b>TYPE</b></td>");
                //rowText.Append("<td style='text-align:center'><b>DATE</b></td>");
                //rowText.Append("<td style='text-align:center'><b>TRX TYPE</b></td>");
                //rowText.Append("<td style='text-align:center'><b>TRX NUMBER</b></td>");
                //rowText.Append("<td style='text-align:center'><b>PAYER/PAYEE NO</b></td>");
                //rowText.Append("<td style='text-align:center'><b>PAYER/PAYEE</b></td>");
                //rowText.Append("<td style='text-align:center'><b>DESCRIPTION</b></td>");
                //rowText.Append("<td style='text-align:center'><b>DEBIT</b></td>");
                //rowText.Append("<td style='text-align:center'><b>CREDIT</b></td>");
                //rowText.Append("<td style='text-align:center'><b>NET</b></td>");
                //rowText.Append("</tr>");

                rowText.Append("<tr>");
                rowText.Append("<td>ACCOUNT NO</td>");
                rowText.Append("<td>NAME</td>");
                rowText.Append("<td>TYPE</td>");
                rowText.Append("<td>DATE</td>");
                rowText.Append("<td>TRX TYPE</td>");
                rowText.Append("<td>TRX NUMBER</td>");
                rowText.Append("<td>PAYER/PAYEE NO</td>");
                rowText.Append("<td>PAYER/PAYEE</td>");
                rowText.Append("<td>DESCRIPTION</td>");
                rowText.Append("<td>DEBIT</td>");
                rowText.Append("<td>CREDIT</td>");
                rowText.Append("<td>NET</td>");
                rowText.Append("</tr>");

                foreach (var item in DataModel)
                {
                    /*rowText.Append("<tr>");
                    rowText.Append("<td style='text-align:center'>" + item.LedgerAcctNumber + "</td>");
                    rowText.Append("<td>" + item.LedgerAcctNumber + ": " + item.LedgerAcctName + "</td>");
                    rowText.Append("<td>" + item.GLName + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" +
                                   Convert.ToDateTime(item.TransactionDate).ToString("MM-dd-yyyy") + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" +
                                   item.TransactionTypeName + "</td>");
                    rowText.Append("<td style='text-align:center;font-size: 11px;'>" + item.TransactionNumber +
                                   "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.Number + "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.Payee_Payer + "</td>");
                    rowText.Append("<td style='font-size: 11px;'>" + item.TransactionDescription + "</td>");
                    rowText.Append("<td style='text-align:right;font-size: 11px;'>" +
                                   (item.AmountTypeListId == 2 ? item.Amount.ToString() : "0.00") + "</td>");
                    rowText.Append("<td style='text-align:right;font-size: 11px;'>" +
                                   (item.AmountTypeListId == 1 ? item.Amount.ToString() : "0.00") + "</td>");

                    if (item.AmountTypeListId == 2)
                    {
                        rowText.Append("<td style='text-align:right;font-size: 11px;'>" + item.Amount.ToString() + "</td>");
                    }
                    else if (item.AmountTypeListId == 1)
                    {
                        rowText.Append("<td style='text-align:right;font-size: 11px;color: red;'>-" + item.Amount.ToString() + "</td>");
                    }
                    else {
                        rowText.Append("<td style='text-align:right;font-size: 11px;'></td>");
                    }                                        
                    rowText.Append("</tr>");*/

                    rowText.Append("<tr>");
                    rowText.Append("<td>" + item.LedgerAcctNumber + "</td>");
                    rowText.Append("<td>" + item.LedgerAcctNumber + ": " + item.LedgerAcctName + "</td>");
                    rowText.Append("<td>" + item.GLName + "</td>");
                    rowText.Append("<td>" + Convert.ToDateTime(item.TransactionDate).ToString("MM-dd-yyyy") + "</td>");
                    rowText.Append("<td>" + item.TransactionTypeName + "</td>");
                    rowText.Append("<td>" + item.TransactionNumber + "</td>");
                    rowText.Append("<td>" + item.Number + "</td>");
                    rowText.Append("<td>" + item.Payee_Payer + "</td>");
                    rowText.Append("<td>" + item.TransactionDescription + "</td>");
                    rowText.Append("<td>" + (item.AmountTypeListId == 2 ? item.Amount.ToString() : "0.00") + "</td>");
                    rowText.Append("<td>" + (item.AmountTypeListId == 1 ? item.Amount.ToString() : "0.00") + "</td>");

                    if (item.AmountTypeListId == 2)
                    {
                        rowText.Append("<td>" + item.Amount.ToString() + "</td>");
                    }
                    else if (item.AmountTypeListId == 1)
                    {
                        rowText.Append("<td>-" + item.Amount.ToString() + "</td>");
                    }
                    else
                    {
                        rowText.Append("<td></td>");
                    }
                    rowText.Append("</tr>");

                    i = i + 1;
                }
                rowText.Append("</table>");
            }

            Response.Clear();
            //Response.Buffer = true;
            string strfilename = "GeneralAccountDetails_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Date + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            Response.AddHeader("content-disposition", "attachment;filename=" + strfilename + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(rowText);
            //Response.Flush();
            Response.End();

            return View("");
        }

        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }

            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                        prop.GetValue(item)));
                    output.Write("\t");
                }

                output.WriteLine();
            }
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

        #region :: Vendor Invoice List ::

        [HttpGet]
        public ActionResult VendorInvoiceList(int? id)
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "", "Company");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "",
                "Corporation Accounting");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "",
                "AP - Vendor Invoices");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            var TransactionStatusList = CustomerService.GetTrasactionStatusList().ToList();
            ViewBag.TransactionStatusList = new SelectList(TransactionStatusList, "TransactionStatusListId", "Name", 4);
            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        public JsonResult VendorListData(string status, string rgId)
        {
            var jsonResult = Json(new { aadata = FranchiseeService.GetVendorInvoiceList(status, rgId) },
                JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public ActionResult PaidInvoiceSearch()
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "", "Company");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "",
                "Corporation Accounting");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "", "AP - Paid Invoice");

            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId).OrderBy(x => x.Name),
                "Code", "Name");
            ViewBag.OrderbyList = new SelectList(_companyService.PaidInvoiceSearchOrderByList(), "SearchValue", "Name");

            return View();
        }


        public JsonResult GetPaidInvoiceData(string FromDate, string ToDate, string FromCheck, string ToCheck,
            string FromInvoice, string ToInvoice, string FromVendor, string OrderBy)
        {
            var resultdata = _companyService.GetPaidInvoicesList(FromDate, ToDate, FromCheck, ToCheck, FromInvoice,
                ToInvoice, FromVendor, OrderBy, SelectedRegionId);
            var jsonResult = Json(new { aaData = resultdata }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        [HttpGet]
        public ActionResult OpenInvoices()
        {
            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.selectedRegionName = _claimView.GetCLAIM_PERSON_INFORMATION().Regions.FirstOrDefault(x => x.RegionId == SelectedRegionId).Name;

            ViewBag.VendorList = new SelectList(FranchiseeService.GetVendorList(SelectedRegionId).OrderBy(x => x.Name), "Code", "Name");
            return View();
        }



        public JsonResult GetTROpenInvoices(string fromDate, string toDate, int regionId, string vendorId = " ")
        {
            var resultData = _companyService.GetTROpenInvoices(fromDate, toDate, regionId, vendorId);
            var jsonResult = Json(new { returnedData = resultData }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        [HttpGet]
        public ActionResult PostToTraverse()
        {
            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public JsonResult GetRegionData()
        {
            int regionid = Convert.ToInt32(Session["SelectedRegionId"]);
            var regionData = _companyService.GetSelectedRegionData(regionid);
            var jsonResult = Json(new { returnedData = regionData }, JsonRequestBehavior.AllowGet);
            return Json(new { returnedData = regionData }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetRegionalOffices()
        {
            /*
             * Used by PostToTraverse() controller action
             */
            var regionList = _commonService.GetRegionList().Where(r => r.RegionId != 0);
            var jsonResult = Json(new { returnedData = regionList }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public JsonResult PostToTransactionsTraverse(string regions)
        {
            var selectedRegions = JsonConvert.DeserializeObject<List<RegionInfoViewModel>>(regions);
            var isSuccess = false;
            foreach (var regionalOffice in selectedRegions)
            {
                //Call process per office
                isSuccess = PostToTransactionsTraverseProcess(regionalOffice.RegionId);

            }

            var jsonResult = Json(new { returnedData = isSuccess }, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }

        public bool PostToTransactionsTraverseProcess(int RegionId)
        {
            Console.WriteLine("Processed " + RegionId);
            //Pull transaction data for particular regional office
            //Insert into traverse 
            return true;
        }

        [HttpGet]
        public ActionResult InvoiceSearch()
        {
            ViewBag.CurrentMenu = "CustomerGeneral";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "", "Company");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "",
                "Corporation Accounting");
            BreadCrumb.Add(Url.Action("VendorInvoiceList", "Company", new { area = "Portal" }) + "",
                "AP - Invoice Search");


            return View();
        }

        public JsonResult GetInvoiceSearchData(string InvoiceNum)
        {
            var resultdata = _companyService.GetInvoiceSearchList(InvoiceNum, SelectedRegionId);
            var jsonResult = Json(new { aaData = resultdata }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        [HttpGet]
        public JsonResult SendAnEmail(string to, string body, string cc = "", string subject = "", string invoices = "")
        {
            var jsonInvoices = JsonConvert.DeserializeObject<List<apTraverseOpenInvoiceList>>(invoices);
            string invoiceTableOpen = "<table style='width=100%'>";
            string invoiceTableRows = "";
            string invoiceTableClose = "</table>";
            string invoiceTableHeader = "<thead><tr>" +
                                "<th style='width=20%; text-align: center'>Vendor ID</th>" +
                                "<th style='width=90%; text-align: left;'>Vendor Name</th>" +
                                "<th style='width=50%;  text-align: center;'>Invoice Number</th>" +
                                "<th style='width=50%; text-align: right;'>Amount</th></tr></thead>";

            foreach (var invoiceItem in jsonInvoices)
            {
                invoiceTableRows += "<tr>" +
                                    "<td style=' text-align: center;'>" + invoiceItem.VendorID + "</td>" +
                                    "<td style=' text-align: left;'>" + invoiceItem.VendorName + "</td>" +
                                    "<td style=' text-align: center;'>" + invoiceItem.InvoiceNum + "</td>" +
                                    "<td style=' text-align: right;'>" + invoiceItem.GrossDueAmt + "</td></tr>";
            }

            var emailBody = body + "<br><hr>" + invoiceTableOpen + invoiceTableHeader + "<tbody>" + invoiceTableRows + "</tbody>" + invoiceTableClose;

            var jsonResult = Json(new { returnedData = _mailService.SendEmailAsync(to, emailBody, subject, null, null, cc) }, JsonRequestBehavior.AllowGet);
            return jsonResult;
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



        #region Company 3rd Party Tab Actions

        /** 
         * Company/3rd Party List View */

        [HttpGet]
        public ActionResult CompanyList()
        {
            ViewBag.HMenu = "Company";
            ViewBag.CurrentMenu = "Company/3rd Party";

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("CompanyList", "Company", new { area = "Portal" }), "Company/3rdParty");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View();
        }
       
        /**  
         * Add New Company/3rd Party View */
        [HttpGet]
        public ActionResult AddNewCompany()
        {
            ViewBag.State = new SelectList(CustomerService.GetStateList().OrderBy(s =>s.Name), "abbr", "Name");
            return View();
        }

        #region Company 3rd Party Http Request

        /**
         * Save Company
         * */
      
        [HttpPost]
        public ActionResult AddNewCompany(FormCollection company3RdParty)
        {
            if(company3RdParty == null)
            {
                return Json(new { success = true, message = "Data is null or empty" });
            }

            //Create a Company
            var company3rdpty = new Company();
            company3rdpty.CompanyName = company3RdParty["MainName"];
            company3rdpty.MainPhone = company3RdParty["MainPhone"];
            company3rdpty.ContactPhone = company3RdParty["ContactPhone"];
            company3rdpty.ContactName = company3RdParty["ContactName"];
            company3rdpty.ContactTitle = company3RdParty["ContactTitle"];
            company3rdpty.Email = company3RdParty["ContactEmail"];
            company3rdpty.RegionId = SelectedRegionId;
            company3rdpty.TypeListId = 13; //Company
            company3rdpty.IsActive = true;
            company3rdpty.CreatedBy = LoginUserId;

            var company = _companyService.Save_Company3rdParty(company3rdpty);

            var companyAddress = new Address();
            companyAddress.ClassId = company.CompanyId;
            companyAddress.TypeListId = 13; //Company
            companyAddress.ContactTypeListId = 1;
            companyAddress.Address1 = company3RdParty["MainAddress"];
            companyAddress.City = company3RdParty["MainCity"];
            companyAddress.StateName = company3RdParty["MainState"];
            companyAddress.PostalCode = company3RdParty["MainZip"];
            companyAddress.StateListId = CustomerService.GetStateId(company3RdParty["MainState"]);
            companyAddress.IsActive = true;
            companyAddress.CreatedBy = LoginUserId;
            companyAddress.CreatedDate = DateTime.Now;
            var address = CustomerService.SaveAddress(companyAddress);

            //Update company
            company3rdpty.AddressId = address.AddressId;
            var companyUpdate = _companyService.Save_Company3rdParty(company3rdpty);

            return Json(new
            {
                success = true                
            });

        }

        /** Get Company List*/
         
        [HttpGet]
        public ActionResult GetCompanyList(string rgId)
        {
            var Company3rdparty = new List<Company3rdPartyViewModel>();
            var company = _companyService.GetAll_Company3rdParty(rgId, -1);
            return Json(new
            {
                succcess = true,
                aadata = company

            },JsonRequestBehavior.AllowGet);
        }


        #endregion

        #endregion





    }
}