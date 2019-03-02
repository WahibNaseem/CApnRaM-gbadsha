using Application.Web.Core;
using AuthorizeNet.Api.Contracts.V1;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.AccountReceivable;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service;
using JKApi.Service.Service.Administration.General;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels;
using JKViewModels.AccountReceivable;
using JKViewModels.Common;
using JKViewModels.Customer;
using MvcAjaxPager;
using MvcBreadCrumbs;
using MxMarchantQuickType;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Account Receivable", Order = -1)]
    //[Authorize]
    public class AccountReceivableController : ViewControllerBase
    {
        public AccountReceivableController(ICommonService commonservice,
            IAccountReceivableService _accountreceivableservice, IUserService userService, ICustomerService _customerservice)
        {
            _commonService = commonservice;
            _userService = userService;
            AccountReceivableService = _accountreceivableservice;
            CustomerService = _customerservice;
            ViewBag.HMenu = "AccountReceivable";
        }

        [Filter.RoleBasedAuthorize]
        // GET: Portal/AccountReceivable
        public ActionResult Index()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "AccountReceivable", new { area = "Portal" }), "Invoices");
            //DashboardViewModel model = new DashboardViewModel();
            //model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            //model.dashboardModel.lstPendingData =
            //  _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View();
        }

        #region for AR Chart........

        public ActionResult GetReceivableByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetReceivableByAgeCategory(spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopRevenueWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetTopRevenueWiseCustomer(recordNumber, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopPaymentWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetTopPaymentWiseCustomer(recordNumber, spnStartDate, spnEndDate, billMonth, billYear);

            List<GrantChartViewModel> chartData = new List<GrantChartViewModel>();

            if (data != null)
            {
                var customerIds = data.GroupBy(x => x.CustomerNo)
                    .Select(x => x.FirstOrDefault())
                    .OrderBy(o => o.Ranking);
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

        public ActionResult GetInvoiceDueByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetInvoiceDueByAgeCategory(spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceivableAndPayment()
        {
            var data = _commonService.GetReceivableAndPayment();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceivableByAgeCategoryDetailsData(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = _commonService.GetReceivableByAgeCategoryDetailsData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceivableByAgeCategoryDetailsDataView(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = _commonService.GetReceivableByAgeCategoryDetailsData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return PartialView("_ReceivableByAgeCategoryDetailsDataView", data); ;
        }

        #endregion

        #region Payment

        // GET: Portal/AccountReceivable/InvoicesLockboxImport
        public ActionResult LockboxImport()
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Payment");
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Lockbox Payment");
            return View();
        }

        [HttpPost]
        public ActionResult LockboxImport(FormCollection frm)
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            int lbid = frm["lockboxedi_id"] != null ? int.Parse(frm["lockboxedi_id"]) : 0;

            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Lockbox Payment");

            return RedirectToAction("LockboxDetail", "AccountReceivable", new { area = "Portal", id = lbid });
        }


        public ActionResult LockboxPendingList()
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LockboxPendingList", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("LockboxPendingList", "AccountReceivable", new { area = "Portal" }), "Lockbox Pending List");

            List<LockboxPendingViewModel> lstLockboxEDIDataViewModel = AccountReceivableService.GetLockboxPendingListData(SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;

            return View(lstLockboxEDIDataViewModel);
        }

        public ActionResult LockboxDetail(int id = 0)
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Lockbox Payment");
            ViewBag.LockboxId = id;
            ViewBag.LockboxProcessed = true;
            List<LockboxEDIDataViewModel> lstLockboxEDIDataViewModel = AccountReceivableService.GetLockboxData(id);
            if (lstLockboxEDIDataViewModel.Where(o => o.StatusListId != 52).Count() > 0)
                ViewBag.LockboxProcessed = false;
            ViewBag.LockboxDate = lstLockboxEDIDataViewModel.FirstOrDefault().LockboxDate;
            return View(lstLockboxEDIDataViewModel);
        }
        [HttpPost]
        public ActionResult LockboxDetail(FormCollection frm)
        {
            var fr = frm;
            int LockboxId = frm["hfLockboxEDIId"] != null
                   ? int.Parse(frm["hfLockboxEDIId"].ToString())
                   : 0;

            DateTime LockboxDate = frm["hfLockboxEDIDate"] != null
                   ? DateTime.Parse(frm["hfLockboxEDIDate"].ToString())
                   : DateTime.Now;

            string LockboxSaveType = frm["hfSaveValue"] != null
                  ? frm["hfSaveValue"].ToString()
                  : "";


            int LockboxDepositCount = !String.IsNullOrEmpty(frm["hflockboxsummaryListDepositCount"])
                  ? int.Parse(frm["hflockboxsummaryListDepositCount"].ToString())
                  : 0;

            string pattern = @"hf_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)_LockboxEDIDetailId";
            //var countL = frm.AllKeys.Where(w => Regex.Match(w, pattern).Success).Count();

            List<string> lstMatchedId = new List<string>();


            foreach (string strKey in frm.AllKeys.Where(w => Regex.Match(w, pattern).Success))
            {
                var ibdID = strKey.Split('_')[1].ToString();
                var invId = strKey.Split('_')[2].ToString();
                string _rowId = ibdID + "_" + invId;

                lstMatchedId.Add(_rowId);

                string ChaqueNumber = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_chkNumber"]) ? frm["hf_" + _rowId + "_chkNumber"].ToString() : "";
                bool _NewMatch = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_newmatch"]) ? bool.Parse(frm["hf_" + _rowId + "_newmatch"].ToString()) : false;


                if (_NewMatch)
                    AccountReceivableService.UpdateLockboxDetailCheckInactive(LockboxId, ChaqueNumber);
            }




            foreach (string strKey in lstMatchedId)
            {

                string _rowId = strKey;

                string ChaqueNumber = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_chkNumber"].ToString().Trim()) ? frm["hf_" + _rowId + "_chkNumber"].ToString() : "";
                bool _NewMatch = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_newmatch"]) ? bool.Parse(frm["hf_" + _rowId + "_newmatch"].ToString()) : false;
                int LockboxEDIDetailId = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_LockboxEDIDetailId"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_LockboxEDIDetailId"].ToString()) : 0;
                int Customerid = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_customerid"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_customerid"].ToString()) : 0;
                int InvoiceId = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_invoiceid"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_invoiceid"].ToString()) : 0;
                decimal InvoiceAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_invoiceamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_invoiceamount"].ToString()) : 0;
                decimal ApplyAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_applyamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_applyamount"].ToString()) : 0;
                decimal BalanceAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_balanceamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_balanceamount"].ToString()) : 0;
                decimal _Overflowamount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_overflowamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_overflowamount"].ToString()) : 0;

                //if (LockboxSaveType == "Save")
                AccountReceivableService.UpdateLockboxDetail(LockboxId, LockboxEDIDetailId, ChaqueNumber, Customerid, InvoiceId, InvoiceAmount, BalanceAmount, ApplyAmount, _Overflowamount,
                    49, _NewMatch);
                //else
                //    AccountReceivableService.UpdateLockboxDetail(LockboxId, LockboxEDIDetailId, ChaqueNumber, Customerid, InvoiceId, InvoiceAmount, BalanceAmount, ApplyAmount, _Overflowamount,
                //        52, _NewMatch);

            }

            //Deposit
            for (int i = 1; i <= LockboxDepositCount; i++)
            {
                bool _NewMatch = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_newmatch"].ToString().Trim()) ? bool.Parse(frm["hfdeposit_" + i + "_newmatch"].ToString()) : false;
                bool _IsODeposit = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_deposit"].ToString().Trim()) ? bool.Parse(frm["hfdeposit_" + i + "_deposit"].ToString()) : false;

                string ChaqueNumber = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_chkNumber"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_chkNumber"].ToString()) : "";
                int LockboxEDIDetailId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_LockboxEDIDetailId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_LockboxEDIDetailId"].ToString()) : 0;
                int Customerid = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_customerid"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_customerid"].ToString()) : 0;
                int InvoiceId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_invoiceid"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_invoiceid"].ToString()) : 0;
                decimal InvoiceAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_invoiceamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_invoiceamount"].ToString()) : 0;
                decimal ApplyAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_applyamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_applyamount"].ToString()) : 0;
                decimal BalanceAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_balanceamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_balanceamount"].ToString()) : 0;
                decimal _Overflowamount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_overflowamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_overflowamount"].ToString()) : 0;
                int _DepositServiceTypeListId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositServiceTypeListId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_DepositServiceTypeListId"].ToString()) : 0;
                int _DepositPayeeId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_DepositPayeeId"].ToString()) : 0;
                string _DepositReason = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositReason"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositReason"].ToString()) : "";
                string _DepositPayeeType = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeType"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeType"].ToString()) : "";
                string _DepositPayeeName = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeName"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeName"].ToString()) : "";
                string _DepositPayeeNo = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeNo"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeNo"].ToString()) : "";


                if (_NewMatch)
                    AccountReceivableService.UpdateLockboxDetailCheckInactive(LockboxId, ChaqueNumber);


                //if (LockboxSaveType == "Save")
                AccountReceivableService.UpdateLockboxDetail(LockboxId, LockboxEDIDetailId, ChaqueNumber, Customerid, InvoiceId, InvoiceAmount, BalanceAmount, ApplyAmount, _Overflowamount,
                    49, _NewMatch, _IsODeposit, _DepositReason, _DepositPayeeType, _DepositServiceTypeListId, _DepositPayeeId, _DepositPayeeName, _DepositPayeeNo);
                //else
                //    AccountReceivableService.UpdateLockboxDetail(LockboxId, LockboxEDIDetailId, ChaqueNumber, Customerid, InvoiceId, InvoiceAmount, BalanceAmount, ApplyAmount, _Overflowamount,
                //        52, _NewMatch, _IsODeposit, _DepositReason, _DepositPayeeType, _DepositServiceTypeListId, _DepositPayeeId, _DepositPayeeName, _DepositPayeeNo);

            }


            if (LockboxSaveType == "Save")
            {
                return RedirectToAction("LockboxPendingList", "AccountReceivable", new { area = "Portal" });
            }

            var M_retVal = AccountReceivableService.ProcessLockboxPayment(LockboxId);

            //decimal _CheckbookAmount = 0;

            //List<LockboxPaymentViewModel> lstLockboxPayment = new List<LockboxPaymentViewModel>();

            //LockboxPaymentViewModel oLockboxPayment = new LockboxPaymentViewModel();

            //foreach (string strKey in lstMatchedId)
            //{

            //    string _rowId = strKey;

            //    string ChaqueNumber = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_chkNumber"].ToString().Trim()) ? frm["hf_" + _rowId + "_chkNumber"].ToString() : "";
            //    bool _NewMatch = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_newmatch"]) ? bool.Parse(frm["hf_" + _rowId + "_newmatch"].ToString()) : false;
            //    int LockboxEDIDetailId = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_LockboxEDIDetailId"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_LockboxEDIDetailId"].ToString()) : 0;
            //    int Customerid = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_customerid"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_customerid"].ToString()) : 0;
            //    int InvoiceId = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_invoiceid"].ToString().Trim()) ? int.Parse(frm["hf_" + _rowId + "_invoiceid"].ToString()) : 0;
            //    decimal InvoiceAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_invoiceamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_invoiceamount"].ToString()) : 0;
            //    decimal ApplyAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_applyamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_applyamount"].ToString()) : 0;
            //    decimal BalanceAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_balanceamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_balanceamount"].ToString()) : 0;
            //    decimal _Overflowamount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_overflowamount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_overflowamount"].ToString()) : 0;
            //    decimal CheckAmount = !String.IsNullOrEmpty(frm["hf_" + _rowId + "_chkAmount"].ToString().Trim()) ? decimal.Parse(frm["hf_" + _rowId + "_chkAmount"].ToString()) : 0;


            //    oLockboxPayment = new LockboxPaymentViewModel();

            //    _CheckbookAmount += (ApplyAmount + _Overflowamount);
            //    oLockboxPayment.ApplyAmount = ApplyAmount;
            //    oLockboxPayment.ChaqueNumber = ChaqueNumber;
            //    oLockboxPayment.CheckAmount = CheckAmount;
            //    oLockboxPayment.InvoiceAmount = InvoiceAmount;
            //    oLockboxPayment.InvoiceId = InvoiceId;
            //    oLockboxPayment.LockboxDate = LockboxDate;
            //    oLockboxPayment.LockboxId = LockboxId;
            //    oLockboxPayment.LockboxEDIDetailId = LockboxEDIDetailId;
            //    oLockboxPayment.PaymentMethodListId = 4;
            //    oLockboxPayment.CustomerId = Customerid;
            //    oLockboxPayment.NewMatch = _NewMatch;
            //    oLockboxPayment.Overflowamount = _Overflowamount;
            //    oLockboxPayment.IsODeposit = false;

            //    bool IsFullPaid = false;
            //    if (BalanceAmount == 0)
            //    {

            //        oLockboxPayment.TransactionStatusListId = 6;
            //        lstLockboxPayment.Add(oLockboxPayment);



            //        IsFullPaid = true;
            //        var M_retVal = AccountReceivableService.ProcessPayment(ChaqueNumber, InvoiceId, Customerid, InvoiceAmount,
            //            ApplyAmount, 4, 6, new List<PartialLockboxPaymentItemViewModel>(), LockboxDate, CheckAmount, _Overflowamount);
            //        if (M_retVal)
            //            AccountReceivableService.InsertLockboxEDIProcessed(LockboxId, ChaqueNumber, InvoiceId, Customerid, ApplyAmount);

            //        //LockboxDate,-1,LoginUserId,NULL,CheckAmount,"LockboxNumber", ""
            //        //@TransactionDate Date = NULL,@RegionId INT,@CreatedBy int,@CustomerId INT = -1,@ApplyAmount DECIMAL(18,2),@ReferenceNo varchar(500)= '',@Notes varchar(500)= NULL

            //    }
            //    else
            //    {
            //        IsFullPaid = false;
            //        List<PartialLockboxPaymentItemViewModel> lstPartialLockboxPaymentItemViewModel =
            //            new List<PartialLockboxPaymentItemViewModel>();
            //        PartialLockboxPaymentItemViewModel oPartialLockboxPaymentItemViewModel =
            //            new PartialLockboxPaymentItemViewModel();


            //        List<LockboxPaymentItemViewModel> lstLockboxPaymentItem = new List<LockboxPaymentItemViewModel>();
            //        LockboxPaymentItemViewModel oLockboxPaymentItem = new LockboxPaymentItemViewModel();


            //        for (int i = 0; i < 50; i++)
            //        {
            //            if (frm[string.Format("inv{0}_item{1}_LineNumber", InvoiceId, i)] == null)
            //                break;
            //            else
            //            {
            //                oPartialLockboxPaymentItemViewModel = new PartialLockboxPaymentItemViewModel();

            //                //decimal CTaxRate = frm[string.Format("inv{0}_item{1}_taxRate", InvId, i)] != null ? decimal.Parse(frm[string.Format("inv{0}_item{1}_taxRate", InvId, i)].ToString()) : 0;
            //                int CLineNumber = frm[string.Format("inv{0}_item{1}_LineNumber", InvoiceId, i)] != null
            //                    ? int.Parse(frm[string.Format("inv{0}_item{1}_LineNumber", InvoiceId, i)].ToString())
            //                    : 0;
            //                decimal CApplyAmount = frm[string.Format("inv{0}_item{1}_paymentAmt", InvoiceId, i)] != null
            //                    ? decimal.Parse(frm[string.Format("inv{0}_item{1}_paymentAmt", InvoiceId, i)].ToString())
            //                    : 0;
            //                //decimal CTaxAmount = frm[string.Format("inv{0}_item{1}_tax", InvId, i)] != null ? decimal.Parse(frm["hf_" + InvId + "_chkNumber"].ToString()) : 0;
            //                //decimal CExpandedAmount = frm[string.Format("inv{0}_item{1}_total", InvId, i)] != null ? decimal.Parse(frm[string.Format("inv{0}_item{1}_total", InvId, i)].ToString()) : 0;

            //                oPartialLockboxPaymentItemViewModel.InvoiceId = InvoiceId;
            //                oPartialLockboxPaymentItemViewModel.ApplyAmount = CApplyAmount;
            //                oPartialLockboxPaymentItemViewModel.LineNumber = CLineNumber;
            //                oPartialLockboxPaymentItemViewModel.CustomerId = Customerid;
            //                oPartialLockboxPaymentItemViewModel.IsCustomerSide = true;

            //                lstPartialLockboxPaymentItemViewModel.Add(oPartialLockboxPaymentItemViewModel);


            //                oLockboxPaymentItem = new LockboxPaymentItemViewModel();
            //                oLockboxPaymentItem.InvoiceId = InvoiceId;
            //                oLockboxPaymentItem.ApplyAmount = CApplyAmount;
            //                oLockboxPaymentItem.BillPayId = -1;
            //                oLockboxPaymentItem.CustomerId = Customerid;
            //                oLockboxPaymentItem.FranchiseeId = -1;
            //                oLockboxPaymentItem.IsCustomerSide = false;
            //                oLockboxPaymentItem.LineNumber = CLineNumber;
            //                lstLockboxPaymentItem.Add(oLockboxPaymentItem);
            //            }
            //        }


            //        foreach (string strFKey in frm.AllKeys.Where(w => w.StartsWith("inv" + InvoiceId + "_bp") && w.EndsWith("_franchiseeid")))
            //        {
            //            oPartialLockboxPaymentItemViewModel = new PartialLockboxPaymentItemViewModel();
            //            string bpId = strFKey.Replace("inv" + InvoiceId + "_bp", "").Split('_')[0];

            //            int FFranchiseeid = frm[string.Format("inv{0}_bp{1}_franchiseeid", InvoiceId, bpId)] != null
            //                ? int.Parse(frm[string.Format("inv{0}_bp{1}_franchiseeid", InvoiceId, bpId)].ToString())
            //                : 0;
            //            decimal FKey = frm[string.Format("inv{0}_bp{1}_key", InvoiceId, bpId)] != null
            //                ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_key", InvoiceId, bpId)].ToString())
            //                : 0;
            //            int FItem = frm[string.Format("inv{0}_bp{1}_item", InvoiceId, bpId)] != null
            //                ? int.Parse(frm[string.Format("inv{0}_bp{1}_item", InvoiceId, bpId)].ToString()
            //                    .Replace("item", ""))
            //                : 0;
            //            decimal FApplyAmount = frm[string.Format("inv{0}_bp{1}_paymentAmt", InvoiceId, bpId)] != null
            //                ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_paymentAmt", InvoiceId, bpId)].ToString())
            //                : 0;
            //            decimal FTaxRate = frm[string.Format("inv{0}_bp{1}_newBalance", InvoiceId, bpId)] != null
            //                ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_newBalance", InvoiceId, bpId)].ToString())
            //                : 0;

            //            int FFLineNumber = frm[string.Format("inv{0}_bp{1}_flinenumber", InvoiceId, bpId)] != null
            //                ? int.Parse(frm[string.Format("inv{0}_bp{1}_flinenumber", InvoiceId, bpId)].ToString())
            //                : 0;

            //            oPartialLockboxPaymentItemViewModel.InvoiceId = InvoiceId;
            //            oPartialLockboxPaymentItemViewModel.ApplyAmount = FApplyAmount;
            //            oPartialLockboxPaymentItemViewModel.BillPayId = int.Parse(bpId);
            //            oPartialLockboxPaymentItemViewModel.CustomerId = Customerid;
            //            oPartialLockboxPaymentItemViewModel.FranchiseeId = FFranchiseeid;
            //            oPartialLockboxPaymentItemViewModel.LineNumber = FFLineNumber;
            //            oPartialLockboxPaymentItemViewModel.IsCustomerSide = false;

            //            lstPartialLockboxPaymentItemViewModel.Add(oPartialLockboxPaymentItemViewModel);


            //            oLockboxPaymentItem = new LockboxPaymentItemViewModel();
            //            oLockboxPaymentItem.InvoiceId = InvoiceId;
            //            oLockboxPaymentItem.ApplyAmount = FApplyAmount;
            //            oLockboxPaymentItem.BillPayId = int.Parse(bpId);
            //            oLockboxPaymentItem.CustomerId = Customerid;
            //            oLockboxPaymentItem.FranchiseeId = FFranchiseeid;
            //            oLockboxPaymentItem.IsCustomerSide = false;
            //            oLockboxPaymentItem.LineNumber = FFLineNumber;
            //            lstLockboxPaymentItem.Add(oLockboxPaymentItem);

            //        }

            //        var M_retVal = AccountReceivableService.ProcessPayment(ChaqueNumber, InvoiceId, Customerid, InvoiceAmount,
            //            ApplyAmount, 4, 7, lstPartialLockboxPaymentItemViewModel, LockboxDate, CheckAmount);

            //        if (M_retVal)
            //            AccountReceivableService.InsertLockboxEDIProcessed(LockboxId, ChaqueNumber, InvoiceId, Customerid, ApplyAmount);




            //        oLockboxPayment.TransactionStatusListId = 7;
            //        oLockboxPayment.Items = lstLockboxPaymentItem;
            //        lstLockboxPayment.Add(oLockboxPayment);

            //    }
            //}


            ////Other Deposit
            //for (int i = 1; i <= LockboxDepositCount; i++)
            //{
            //    bool _NewMatch = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_newmatch"].ToString().Trim()) ? bool.Parse(frm["hfdeposit_" + i + "_newmatch"].ToString()) : false;
            //    bool _IsODeposit = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_deposit"].ToString().Trim()) ? bool.Parse(frm["hfdeposit_" + i + "_deposit"].ToString()) : false;

            //    string ChaqueNumber = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_chkNumber"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_chkNumber"].ToString()) : "";
            //    int LockboxEDIDetailId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_LockboxEDIDetailId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_LockboxEDIDetailId"].ToString()) : 0;
            //    int Customerid = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_customerid"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_customerid"].ToString()) : 0;
            //    int InvoiceId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_invoiceid"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_invoiceid"].ToString()) : 0;
            //    decimal InvoiceAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_invoiceamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_invoiceamount"].ToString()) : 0;
            //    decimal ApplyAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_applyamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_applyamount"].ToString()) : 0;
            //    decimal BalanceAmount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_balanceamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_balanceamount"].ToString()) : 0;
            //    decimal _Overflowamount = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_overflowamount"].ToString().Trim()) ? decimal.Parse(frm["hfdeposit_" + i + "_overflowamount"].ToString()) : 0;
            //    int _DepositServiceTypeListId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositServiceTypeListId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_DepositServiceTypeListId"].ToString()) : 0;
            //    int _DepositPayeeId = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeId"].ToString().Trim()) ? int.Parse(frm["hfdeposit_" + i + "_DepositPayeeId"].ToString()) : 0;
            //    string _DepositReason = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositReason"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositReason"].ToString()) : "";
            //    string _DepositPayeeType = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeType"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeType"].ToString()) : "";
            //    string _DepositPayeeName = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeName"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeName"].ToString()) : "";
            //    string _DepositPayeeNo = !String.IsNullOrEmpty(frm["hfdeposit_" + i + "_DepositPayeeNo"].ToString().Trim()) ? (frm["hfdeposit_" + i + "_DepositPayeeNo"].ToString()) : "";



            //    _CheckbookAmount += ApplyAmount;
            //    AccountReceivableService.ProcessLockboxOtherDeposit(LockboxDate, _DepositPayeeId, _DepositPayeeType, _DepositReason, _DepositServiceTypeListId, ApplyAmount, ChaqueNumber, LockboxId);
            //}

            //Comment because insert by post functionality
            ////Checkbook Entry             
            //AccountReceivableService.ProcessCheckbookPaymentLockbox(LockboxDate, _CheckbookAmount, LockboxId);

            return RedirectToAction("PaymentList", "AccountReceivable", new { area = "Portal", @duration = 9 });

            //?
            //return RedirectToAction("PaymentList", "AccountReceivable", new { area = "Portal" });
        }

        public JsonResult LockboxDetailData(int id = 0)
        {
            ViewBag.LockboxId = id;
            ViewBag.LockboxProcessed = true;
            List<LockboxEDIDataViewModel> lstLockboxEDIDataViewModel = AccountReceivableService.GetLockboxData(id);
            if (lstLockboxEDIDataViewModel.Where(o => o.StatusListId != 52).Count() > 0)
                ViewBag.LockboxProcessed = false;

            return Json(new
            {
                LockboxData = lstLockboxEDIDataViewModel,
                LockboxId = id,
                LockboxProcessed = ViewBag.LockboxProcessed
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Portal/AccountReceivable/PaymentList
        public ActionResult PaymentList(int duration = 3)
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PaymentList", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("PaymentList", "AccountReceivable", new { area = "Portal" }), "Payment");
            BreadCrumb.Add(Url.Action("PaymentList", "AccountReceivable", new { area = "Portal" }), "List");
            //ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_OptionList(), "SearchDateListId", "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = this.SelectedRegionId;

            return View();
        }

        public bool ReversalPayment(int Id)
        {
            return AccountReceivableService.ReversalPayment(Id);
        }

        // GET: Portal/AccountReceivable/PostPayment
        public ActionResult PostPayment()
        {
            ViewBag.CurrentMenu = "AccountsReceivableCredits";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PostPayment", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("PostPayment", "AccountReceivable", new { area = "Portal" }), "Customer Credits");
            BreadCrumb.Add(Url.Action("PostPayment", "AccountReceivable", new { area = "Portal" }), "Post Payment");
            return View();
        }

        public ActionResult LockboxFileSummary(int id = 0)
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("LockboxImport", "AccountReceivable", new { area = "Portal" }), "Lockbox Payment");
            ViewBag.LockboxId = id;
            return View();
        }

        public JsonResult LockboxFileSummaryData(int id = 0)
        {
            List<LockboxEDIDataViewModel> lstLockboxEDIDataViewModel = AccountReceivableService.GetLockboxData(id);
            return Json(lstLockboxEDIDataViewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LockboxFileSummaryDataN(int id = 0)
        {
            LockboxEDIData oLockboxEDIData = AccountReceivableService.GetLockboxDataDetail(id);
            return Json(oLockboxEDIData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LockboxFileSummary(FormCollection frm)
        {
            var fr = frm;
            int LockboxId = frm["hfLockboxEDIId"] != null
                   ? int.Parse(frm["hfLockboxEDIId"].ToString())
                   : 0;

            foreach (string strKey in frm.AllKeys.Where(w => w.Contains("_invoiceid")))
            {
                var InvId = strKey.Split('_')[1];


                int Customerid = frm["hf_" + InvId + "_customerid"] != null
                    ? int.Parse(frm["hf_" + InvId + "_customerid"].ToString())
                    : 0;
                string ChaqueNumber = frm["hf_" + InvId + "_chkNumber"] != null
                    ? frm["hf_" + InvId + "_chkNumber"].ToString()
                    : "";
                int InvoiceId = frm["hf_" + InvId + "_invoiceid"] != null
                    ? int.Parse(frm["hf_" + InvId + "_invoiceid"].ToString())
                    : 0;
                decimal InvoiceAmount = frm["hf_" + InvId + "_invoiceamount"] != null
                    ? decimal.Parse(frm["hf_" + InvId + "_invoiceamount"].ToString())
                    : 0;
                decimal ApplyAmount = frm["hf_" + InvId + "_applyamount"] != null
                    ? decimal.Parse(frm["hf_" + InvId + "_applyamount"].ToString())
                    : 0;
                decimal BalanceAmount = frm["hf_" + InvId + "_balanceamount"] != null
                    ? decimal.Parse(frm["hf_" + InvId + "_balanceamount"].ToString())
                    : 0;

                bool IsFullPaid = false;
                if (BalanceAmount == 0)
                {
                    IsFullPaid = true;
                    var M_retVal = AccountReceivableService.ProcessPayment(ChaqueNumber, InvoiceId, Customerid, InvoiceAmount,
                        ApplyAmount, 4, 6, new List<PartialLockboxPaymentItemViewModel>());

                    if (M_retVal)
                        AccountReceivableService.InsertLockboxEDIProcessed(LockboxId, ChaqueNumber, InvoiceId, Customerid, ApplyAmount);
                }
                else
                {
                    IsFullPaid = false;
                    List<PartialLockboxPaymentItemViewModel> lstPartialLockboxPaymentItemViewModel =
                        new List<PartialLockboxPaymentItemViewModel>();
                    PartialLockboxPaymentItemViewModel oPartialLockboxPaymentItemViewModel =
                        new PartialLockboxPaymentItemViewModel();

                    for (int i = 0; i < 50; i++)
                    {
                        if (frm[string.Format("inv{0}_item{1}_LineNumber", InvId, i)] == null)
                            break;
                        else
                        {
                            oPartialLockboxPaymentItemViewModel = new PartialLockboxPaymentItemViewModel();

                            //decimal CTaxRate = frm[string.Format("inv{0}_item{1}_taxRate", InvId, i)] != null ? decimal.Parse(frm[string.Format("inv{0}_item{1}_taxRate", InvId, i)].ToString()) : 0;
                            int CLineNumber = frm[string.Format("inv{0}_item{1}_LineNumber", InvId, i)] != null
                                ? int.Parse(frm[string.Format("inv{0}_item{1}_LineNumber", InvId, i)].ToString())
                                : 0;
                            decimal CApplyAmount = frm[string.Format("inv{0}_item{1}_paymentAmt", InvId, i)] != null
                                ? decimal.Parse(frm[string.Format("inv{0}_item{1}_paymentAmt", InvId, i)].ToString())
                                : 0;
                            //decimal CTaxAmount = frm[string.Format("inv{0}_item{1}_tax", InvId, i)] != null ? decimal.Parse(frm["hf_" + InvId + "_chkNumber"].ToString()) : 0;
                            //decimal CExpandedAmount = frm[string.Format("inv{0}_item{1}_total", InvId, i)] != null ? decimal.Parse(frm[string.Format("inv{0}_item{1}_total", InvId, i)].ToString()) : 0;

                            oPartialLockboxPaymentItemViewModel.InvoiceId = InvoiceId;
                            oPartialLockboxPaymentItemViewModel.ApplyAmount = CApplyAmount;
                            oPartialLockboxPaymentItemViewModel.LineNumber = CLineNumber;
                            oPartialLockboxPaymentItemViewModel.CustomerId = Customerid;
                            oPartialLockboxPaymentItemViewModel.IsCustomerSide = true;

                            lstPartialLockboxPaymentItemViewModel.Add(oPartialLockboxPaymentItemViewModel);
                        }
                    }


                    foreach (string strFKey in frm.AllKeys.Where(w => w.StartsWith("inv" + InvId + "_bp") && w.EndsWith("_franchiseeid")))
                    {
                        oPartialLockboxPaymentItemViewModel = new PartialLockboxPaymentItemViewModel();
                        string bpId = strFKey.Replace("inv" + InvId + "_bp", "").Split('_')[0];

                        int FFranchiseeid = frm[string.Format("inv{0}_bp{1}_franchiseeid", InvId, bpId)] != null
                            ? int.Parse(frm[string.Format("inv{0}_bp{1}_franchiseeid", InvId, bpId)].ToString())
                            : 0;
                        decimal FKey = frm[string.Format("inv{0}_bp{1}_key", InvId, bpId)] != null
                            ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_key", InvId, bpId)].ToString())
                            : 0;
                        int FItem = frm[string.Format("inv{0}_bp{1}_item", InvId, bpId)] != null
                            ? int.Parse(frm[string.Format("inv{0}_bp{1}_item", InvId, bpId)].ToString()
                                .Replace("item", ""))
                            : 0;
                        decimal FApplyAmount = frm[string.Format("inv{0}_bp{1}_paymentAmt", InvId, bpId)] != null
                            ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_paymentAmt", InvId, bpId)].ToString())
                            : 0;
                        decimal FTaxRate = frm[string.Format("inv{0}_bp{1}_newBalance", InvId, bpId)] != null
                            ? decimal.Parse(frm[string.Format("inv{0}_bp{1}_newBalance", InvId, bpId)].ToString())
                            : 0;

                        int FFLineNumber = frm[string.Format("inv{0}_bp{1}_flinenumber", InvId, bpId)] != null
                            ? int.Parse(frm[string.Format("inv{0}_bp{1}_flinenumber", InvId, bpId)].ToString())
                            : 0;

                        oPartialLockboxPaymentItemViewModel.InvoiceId = InvoiceId;
                        oPartialLockboxPaymentItemViewModel.ApplyAmount = FApplyAmount;
                        oPartialLockboxPaymentItemViewModel.BillPayId = int.Parse(bpId);
                        oPartialLockboxPaymentItemViewModel.CustomerId = Customerid;
                        oPartialLockboxPaymentItemViewModel.FranchiseeId = FFranchiseeid;
                        oPartialLockboxPaymentItemViewModel.LineNumber = FFLineNumber;
                        oPartialLockboxPaymentItemViewModel.IsCustomerSide = false;

                        lstPartialLockboxPaymentItemViewModel.Add(oPartialLockboxPaymentItemViewModel);
                    }

                    var M_retVal = AccountReceivableService.ProcessPayment(ChaqueNumber, InvoiceId, Customerid, InvoiceAmount,
                        ApplyAmount, 4, 7, lstPartialLockboxPaymentItemViewModel);
                    if (M_retVal)
                        AccountReceivableService.InsertLockboxEDIProcessed(LockboxId, ChaqueNumber, InvoiceId, Customerid, ApplyAmount);


                }
            }

            return View(); // Json("Call Done", JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult ApplyManualPaymentForm(int Id)
        {
            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForInvoice(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");


            ManualPaymentInvoiceDetailViewModel _model = new ManualPaymentInvoiceDetailViewModel();

            _model.InvoiceId = model.Invoice.InvoiceDetail.InvoiceId;
            _model.InvoiceNo = model.Invoice.InvoiceDetail.InvoiceNo;
            _model.InvoiceDate = model.Invoice.InvoiceDetail.InvoiceDate;
            _model.DueDate = model.Invoice.InvoiceDetail.DueDate;
            _model.InvoiceDescription = model.Invoice.InvoiceDetail.Message;
            _model.InvoiceMessage = model.Invoice.InvoiceDetail.InvoiceMessage;
            _model.CustomerId = model.Invoice.InvoiceDetail.CustomerId;
            _model.CustomerName = model.Invoice.InvoiceDetail.Customer;
            _model.CustomerNo = model.Invoice.InvoiceDetail.CustomerNo;
            _model.InvoiceAmount = (decimal)model.Invoice.InvoiceDetailItems.Sum(o => o.ExtendedPrice); ;
            _model.InvoiceBalanceAmount = model.InvoiceBalance;
            _model.InvoiceNewBalanceAmount = model.InvoiceAmount;
            _model.InvoiceTaxAmount = (decimal)model.Invoice.InvoiceDetailItems.Sum(o => o.TAXAmount);
            _model.InvoiceTotalAmount = (decimal)model.Invoice.InvoiceDetailItems.Sum(o => o.Total);
            _model.PaymentApplyAmount = 0;

            _model.InvoiceTaxPercentage = ((decimal)model.Invoice.InvoiceDetailItems.Sum(o => o.TAXAmount)) * 100 / ((decimal)model.Invoice.InvoiceDetailItems.Sum(o => o.ExtendedPrice));

            List<ManualPaymentInvoiceFranchiseeItemDetailViewModel> lstItems = new List<ManualPaymentInvoiceFranchiseeItemDetailViewModel>();

            ManualPaymentInvoiceFranchiseeItemDetailViewModel oItem;
            foreach (var item in model.FranchiseeItems)
            {
                oItem = new ManualPaymentInvoiceFranchiseeItemDetailViewModel();

                oItem.BillingPayId = item.InvoiceFranchiseeDetailItem.BillingPayId;
                oItem.InvoiceId = model.Invoice.InvoiceDetail.InvoiceId;
                oItem.FranchiseeId = item.InvoiceFranchiseeDetailItem.FranchiseeId;
                oItem.FranchiseeName = item.InvoiceFranchiseeDetailItem.Name;
                oItem.FranchiseeNo = item.InvoiceFranchiseeDetailItem.FranchiseeNo;
                oItem.ExtendedPrice = item.InvoiceFranchiseeDetailItem.Amount;
                oItem.FeeAmount = (decimal)item.InvoiceFranchiseeDetailItem.BalanceFees;
                oItem.FeePercentage = ((decimal)item.InvoiceFranchiseeDetailItem.BalanceFees * 100) / (decimal)item.InvoiceFranchiseeDetailItem.Balance; ;
                oItem.Total = item.InvoiceFranchiseeDetailItem.Amount;
                oItem.Balance = item.InvoiceFranchiseeDetailItem.Balance; ;
                oItem.NewBalance = item.InvoiceFranchiseeDetailItem.Balance; ;

                //oItem.ContractAmount;
                //oItem.ContractDate;
                //oItem.Description;
                //oItem.ServiceTypeListId;
                //oItem.ServiceTypeListName;
                lstItems.Add(oItem);
            }




            _model.InvoiceItems = new List<ManualPaymentInvoiceItemDetailViewModel>();
            _model.InvoiceFranchiseeItems = lstItems;
            _model.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

            return PartialView("_PartialApplyManualPaymentForm", _model);
        }

        public ActionResult PaymentDataTable(string regionIds, DateTime? from, DateTime? to, int month = 0, int year = 0)
        {
            if (regionIds == "null") regionIds = null;

            try
            {
                var response = AccountReceivableService.GetPaymentList(regionIds, from, to, month, year);
                var result = from f in response
                             select new
                             {
                                 PaymentId = f.PaymentId,
                                 PaymentNo = f.TransactionNumber,
                                 CreatedDate = f.CreatedDate != null ? f.CreatedDate : null,
                                 CustomerName = f.Name,
                                 CustomerNo = f.CustomerNo,
                                 PaymentType = f.PaymentType,
                                 CheckNo = f.PaymentNo != null ? f.PaymentNo : string.Empty,
                                 InvoiceNo = f.InvoiceNo != null ? f.InvoiceNo : string.Empty,
                                 InvoiceAmount = f.InvoiceAmount != null ? String.Format("{0:C}", f.InvoiceAmount) : "$0.00",
                                 InvoiceBalance = f.InvoiceBalance != null ? String.Format("{0:C}", f.InvoiceBalance) : "$0.00",
                                 InvoiceBalanceOR = f.InvoiceBalance != null ? (decimal)f.InvoiceBalance : (decimal)0.0,
                                 PaymentAmount = f.PaymentAmount != null ? String.Format("{0:C}", f.PaymentAmount) : "$0.00",
                                 PaymentAmountOR = f.PaymentAmount != null ? (decimal)f.PaymentAmount : (decimal)0.0,
                                 Reference = f.PaymentDescription != null ? f.PaymentDescription : string.Empty,
                                 RegionName = f.RegionName,
                                 InvoiceId = f.InvoiceId,
                                 CheckAmount = f.CheckAmount != null ? String.Format("{0:C}", f.CheckAmount) : "$0.00"
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

        #endregion Payment

        #region Reports

        // GET: Portal/AccountReceivable/Aging
        [HttpGet]
        public ActionResult Aging()
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Aging");
            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            //List<SelectListItem> searchByList = Enum.GetValues(typeof(SearchByAging)).Cast<SearchByAging>().Select(v => new SelectListItem
            //{
            //    Text = v.ToString(),
            //    Value = ((int)v).ToString()
            //}).ToList();

            List<SelectListItem> orderByList =
                Enum.GetValues(typeof(OrderByAging)).Cast<OrderByAging>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

            List<SelectListItem> includeList =
                Enum.GetValues(typeof(IncludeListAging)).Cast<IncludeListAging>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

            List<SelectListItem> balanceList =
                Enum.GetValues(typeof(balanceListAging)).Cast<balanceListAging>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

            List<SelectListItem> billMonths =
                Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

            var regionViewModel = _claimView.GetCLAIM_PERSON_INFORMATION().Regions;

            //var regionList = regionViewModel.Select(o => new SelectListItem
            //{
            //    Text = o.Name,
            //    Value = o.RegionId.ToString()
            //});

            //ViewBag.searchByList = searchByList;
            ViewBag.orderByList = orderByList;
            ViewBag.includeList = includeList;
            ViewBag.balanceList = balanceList;
            ViewBag.billMonths = billMonths;
            //ViewBag.RegionList = regionList;

            ViewBag.SelectedRegionId = SelectedRegionId;

            return View();
        }


        [HttpGet]
        public ActionResult AgingReport(DateTime? reportDate, DateTime? agingDate, DateTime? paymentDate, int? monthsToInclude, bool? isNonChargebackOnly,
             string regionIds, bool isSummaryView = false, int pageNo = 1, int pageSize = int.MaxValue - 1, string sortBy = "cNameAsc", string IsMonthView = "")
        {

            ViewBag.CurrentMenu = "AccountsReceivableReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Report");
            BreadCrumb.Add(Url.Action("Aging", "AccountReceivable", new { area = "Portal" }), "Aging Report");

            ICollection<KeyValuePair<string, string>> objPageSizeList = new Dictionary<string, string>();
            objPageSizeList.Add(new KeyValuePair<string, string>("0", "All"));
            objPageSizeList.Add(new KeyValuePair<string, string>("25", "25"));
            objPageSizeList.Add(new KeyValuePair<string, string>("50", "50"));
            objPageSizeList.Add(new KeyValuePair<string, string>("100", "100"));

            SelectList objPageSize = new SelectList(objPageSizeList, "Key", "Value", pageSize > 100 ? 0 : pageSize);
            ViewBag.PageSizeList = objPageSize;

            ICollection<KeyValuePair<string, string>> ddlSortItems = new Dictionary<string, string>();
            ddlSortItems.Add(new KeyValuePair<string, string>("none", "Default"));
            ddlSortItems.Add(new KeyValuePair<string, string>("cNameAsc", "Customer Name Ascending"));
            ddlSortItems.Add(new KeyValuePair<string, string>("cNameDesc", "Customer Name Descending"));
            //ddlSortItems.Add(new KeyValuePair<string, string>("invDateAsc", "Invoice Date Ascending"));
            //ddlSortItems.Add(new KeyValuePair<string, string>("invDateDesc", "Invoice Date Descending"));

            SelectList ddlSort = new SelectList(ddlSortItems, "Key", "Value", sortBy);
            ViewBag.objDdlSort = ddlSort;
            ViewBag.sortBy = sortBy;
            ViewBag.IsMonthView = IsMonthView;

            ViewBag.isAjaxRequest = Request.IsAjaxRequest();


            var agingReportViewModel = new AgingReportViewModel
            {
                ReportDate = reportDate,
                agingDate = agingDate,
                PaymentDate = paymentDate,
                monthsToInclude = monthsToInclude,
                isNonChargebackOnly = isNonChargebackOnly,
                isSummaryView = isSummaryView,
                regionIds = regionIds,
                IsMonthView = IsMonthView
            };

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.statusList = new SelectList(
                _commonService.DropDownListByName(MasterDropName.StatusList.ToString()), "Value", "Text", 1);
            ViewBag.IsSummary = isSummaryView;
            ViewBag.AgingReportFilters = agingReportViewModel;

            var agingList = AccountReceivableService.AgingReport(agingReportViewModel);

            // sorting
            if (agingList != null && agingList.Count() > 0 && sortBy != "none")
            {
                agingList = sortBy == "cNameAsc" ? agingList.OrderBy(x => x.customerName).ToList() : agingList;
                agingList = sortBy == "cNameDesc" ? agingList.OrderByDescending(x => x.customerName).ToList() : agingList;
                agingList = sortBy == "invDateAsc" ? agingList.OrderBy(x => Convert.ToDateTime(x.invDate)).ToList() : agingList;
                agingList = sortBy == "invDateDesc" ? agingList.OrderBy(x => Convert.ToDateTime(x.invDate)).ToList() : agingList;
            }

            var customers = agingList.GroupBy(x => x.customerId).Select(d => new { id = d.Key }).ToArray();

            pageSize = pageSize == 0 ? customers.Count() : pageSize;
            var customerPagedList = customers.ToPagedList(pageNo, pageSize);

            var resultSet = (from a in agingList
                             join c in customerPagedList on a.customerId equals c.id
                             select a).ToList();


            var pagedCustomers = new StaticPagedList<AgingReportViewModel>(resultSet, pageNo, pageSize, customers.Count());

            ViewBag.pagedCustomers = pagedCustomers;
            ViewBag.RegionWiseTotal = agingList.GroupBy(x => x.RegionId).Select(x => new RegionGroupBy
            {
                RegionId = x.First().RegionId,
                RegionName = x.First().RegionName,
                FirstTotal = x.Sum(first => Convert.ToDecimal(first.onemo)),
                SecondTotal = x.Sum(second => Convert.ToDecimal(second.twomo)),
                ThirdTotal = x.Sum(third => Convert.ToDecimal(third.threemo)),
                FourthTotal = x.Sum(fourth => Convert.ToDecimal(fourth.fourmo)),
                FifthTotal = x.Sum(fifth => Convert.ToDecimal(fifth.sixmo)),
                FinalTotal = x.Sum(final => Convert.ToDecimal(final.totalAmount))
            }).ToList();
            if (!Request.IsAjaxRequest())
            {
                return View(agingList);
            }

            return PartialView("~/Areas/Portal/Views/AccountReceivable/_PartialAgingReportList.cshtml", agingList);

        }

        [HttpGet]
        public ActionResult AgingReportDataTable(DateTime? agingDate, int? monthsToInclude, bool? isNonChargebackOnly,
            bool? isSummaryView, string regionIds)
        {
            var agingReportViewModel = new AgingReportViewModel
            {
                agingDate = agingDate,
                monthsToInclude = monthsToInclude,
                isNonChargebackOnly = isNonChargebackOnly,
                isSummaryView = isSummaryView,
                regionIds = regionIds
            };
            var AgingReportViewModel = new List<AgingReportViewModel>();
            var agingList = AccountReceivableService.AgingReport(agingReportViewModel);
            if (agingList != null)
            {
                var unique_customer_ids = agingList.Select(s => s.customerId).Distinct().ToList();
                foreach (var customer_id in unique_customer_ids)
                {
                    var entries = agingList.Where(s => s.customerId == customer_id).ToList();

                    if (entries.Count() != 0)
                    {
                        var first_item = entries.First();
                        decimal currentTotal = 0;
                        decimal oneMoTotal = 0;
                        decimal twoMoTotal = 0;
                        decimal threeMoTotal = 0;
                        decimal fourMoTotal = 0;
                        decimal fiveMoTotal = 0;

                        foreach (var item in entries)
                        {
                            currentTotal += Convert.ToDecimal(item.totalAmount);
                            oneMoTotal += Convert.ToDecimal(item.onemo);
                            twoMoTotal += Convert.ToDecimal(item.twomo);
                            threeMoTotal += Convert.ToDecimal(item.threemo);
                            fourMoTotal += Convert.ToDecimal(item.fourmo);
                            fiveMoTotal += Convert.ToDecimal(item.fivemo);
                        }
                        var allTotal = currentTotal + oneMoTotal + twoMoTotal + threeMoTotal + fourMoTotal;
                        var list = new AgingReportViewModel
                        {
                            customerName = agingList.FirstOrDefault(o => o.customerId == customer_id).customerName,
                            totalAmount = currentTotal.ToString(),
                            onemo = oneMoTotal.ToString(),
                            twomo = twoMoTotal.ToString(),
                            threemo = threeMoTotal.ToString(),
                            fourmo = fourMoTotal.ToString(),
                            fivemo = fiveMoTotal.ToString(),
                            allTotal = allTotal.ToString()
                        };

                        AgingReportViewModel.Add(list);
                    }
                }
            }

            var jsonResult = Json(new { aadata = AgingReportViewModel }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult Aging(AgingReportViewModel agingReportViewModel)
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            TempData["AgingReportViewModel"] = agingReportViewModel;
            TempData["RegionIdsViewModel"] = agingReportViewModel.RegionIdsViewModel;
            //TempData["searchBy"] = collection["searchBy"].ToString();
            //TempData["franchiseid"] = collection["franchiseid"] == null ? "" : collection["franchiseid"].ToString();
            //TempData["searchvalue"] = collection["searchValue"].ToString();
            //TempData["agingdate"] = collection["agingDate"].ToString();
            //TempData["paymentdate"] = collection["paymentDate"].ToString();
            //TempData["months"] = collection["monthsToInclude"].ToString();
            //TempData["orderby"] = collection["orderBy"].ToString();
            //TempData["include"] = collection["include"] == null ? "" : collection["include"].ToString();
            //TempData["balance"] = collection["withBalance"].ToString();
            //TempData["nonchargebackonly"] = collection["isNonChargebackOnly"].ToString();


            //return RedirectToAction("AgingReport", "AccountReceivable", new { area = "Portal" });
            return RedirectToAction("AgingReport");
        }

        public ActionResult AgingList()
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            string franchiseid = TempData["franchiseid"].ToString();
            string searchby = TempData["searchby"].ToString();
            string searchvalue = TempData["searchvalue"].ToString();
            string agingdate = TempData["agingdate"].ToString();
            string paymentdate = TempData["paymentdate"].ToString();
            string months = TempData["months"].ToString();
            string orderby = TempData["orderby"].ToString();
            string include = TempData["include"].ToString();
            string balance = TempData["balance"].ToString();
            string nonchargebackonly = TempData["nonchargebackonly"].ToString();

            if (franchiseid != null && franchiseid.Trim().Length > 0 && !franchiseid.Equals("-1"))
            {
                searchby = "3";
                searchvalue = franchiseid;
            }
            if (franchiseid == null || franchiseid.Trim().Length == 0)
                franchiseid = "-1";
            if (searchby == null || searchby.Trim().Length == 0)
                searchby = "1";
            if (agingdate == null || agingdate.Trim().Length == 0)
                agingdate = DateTime.Now.ToShortDateString();
            if (paymentdate == null || paymentdate.Trim().Length == 0)
                paymentdate = DateTime.Now.ToShortDateString();

            if (months == null || months.Trim().Length == 0)
                months = "24";
            if (orderby == null || orderby.Trim().Length == 0)
                orderby = "1";
            if (include == null || include.Trim().Length == 0)
                include = "1";
            if (balance == null || balance.Trim().Length == 0)
                balance = "-1";
            if (nonchargebackonly == null || nonchargebackonly.Trim().Length == 0)
                nonchargebackonly = "0";
            if (nonchargebackonly.Equals("false"))
                nonchargebackonly = "0";
            if (nonchargebackonly.Equals("true"))
                nonchargebackonly = "1";

            //AccountReceivableService serv = new AccountReceivableService();  // Sohel: Service has already a reference.

            List<AgingViewModel> agingList = AccountReceivableService.getAgingList(franchiseid, searchby, searchvalue,
                agingdate, paymentdate, months, orderby, include, balance, nonchargebackonly);

            //return agingList;
            return View(agingList);
        }

        // GET: Portal/AccountReceivable/OverPaymentReport
        public ActionResult OverPaymentReport()
        {
            ViewBag.CurrentMenu = "OverPaymentReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("OverPaymentReport", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("OverPaymentReport", "AccountReceivable", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("OverPaymentReport", "AccountReceivable", new { area = "Portal" }), "Over Payment Report");

            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;


            return View();
        }

        public JsonResult OverPaymentReportData(string regionIds, DateTime? FromDate, DateTime? ToDate, string SearchText)
        {
            var _result = AccountReceivableService.OverPaymentReportData(regionIds, FromDate, ToDate, SearchText);

            var jsonResult = Json(new { aadata = _result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //  [HttpPost]
        //public List<AgingViewModel> getAgingList( )
        //{
        //    string franchiseid = TempData["franchiseid"].ToString();
        //    string searchby = TempData["searchby"].ToString();
        //    string searchvalue = TempData["searchvalue"].ToString();
        //    string agingdate = TempData["agingdate"].ToString();
        //    string paymentdate = TempData["paymentdate"].ToString();
        //    string months = TempData["months"].ToString();
        //    string orderby = TempData["orderby"].ToString();
        //    string include = TempData["include"].ToString();
        //    string balance = TempData["balance"].ToString();
        //    string nonchargebackonly = TempData["nonchargebackonly"].ToString();

        //    int totalRecords, recFilter = 0;
        //    int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
        //    int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

        //    if (franchiseid != null && franchiseid.Trim().Length > 0 && !franchiseid.Equals("-1"))
        //    {
        //        searchby = "3";
        //        searchvalue = franchiseid;
        //    }
        //    if (franchiseid == null || franchiseid.Trim().Length == 0)
        //        franchiseid = "-1";
        //    if (searchby == null || searchby.Trim().Length == 0)
        //        searchby = "1";
        //    if (agingdate == null || agingdate.Trim().Length == 0)
        //        agingdate = DateTime.Now.ToShortDateString();
        //    if (paymentdate == null || paymentdate.Trim().Length == 0)
        //        paymentdate = DateTime.Now.ToShortDateString();

        //    if (months == null || months.Trim().Length == 0)
        //        months = "24";
        //    if (orderby == null || orderby.Trim().Length == 0)
        //        orderby = "1";
        //    if (include == null || include.Trim().Length == 0)
        //        include = "1";
        //    if (balance == null || balance.Trim().Length == 0)
        //        balance = "-1";
        //    if (nonchargebackonly == null || nonchargebackonly.Trim().Length == 0)
        //        nonchargebackonly = "0";
        //    if (nonchargebackonly.Equals("false"))
        //        nonchargebackonly = "0";
        //    if (nonchargebackonly.Equals("true"))
        //        nonchargebackonly = "1";

        //    AccountReceivableService serv = new AccountReceivableService();
        //    List<AgingViewModel> agingList = serv.getAgingList(franchiseid, searchby, searchvalue, agingdate, paymentdate, months, orderby, include, balance, nonchargebackonly);

        //    return agingList;

        //    //totalRecords = agingList.Count;
        //    //recFilter = agingList.Count;

        //    //// Apply pagination.
        //    //agingList = agingList.Skip(startRec).Take(pageSize).ToList();

        //    //return this.Json(new { recordsTotal = totalRecords, recordsFiltered = recFilter, data = agingList }, JsonRequestBehavior.AllowGet);

        //}

        //[HttpPost]
        //public ActionResult getAgingListByFranchiseId(string franchiseid, string searchby, string searchvalue, string agingdate, string paymentdate, string months, string orderby, string include, string balance, string nonchargebackonly)
        //{
        //    AccountReceivableService serv = new AccountReceivableService();
        //    List<AgingViewModel> agingList = serv.getAgingListByFranchise( franchiseid,  searchby,  searchvalue,  agingdate,  paymentdate,  months,  orderby,  include,  balance, nonchargebackonly);

        //    return PartialView("_agingListByFranchisePartial", agingList);

        //    //JavaScriptSerializer jss = new JavaScriptSerializer();
        //    //string output = jss.Serialize(agingList);
        //    //
        //    //return Json(output, JsonRequestBehavior.AllowGet);

        //}

        // GET: Portal/AccountReceivable/ARLog
        [HttpGet]
        public ActionResult ARLog()
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "AR Log");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = this.SelectedRegionId;


            return View();
        }

        [HttpPost]
        public ActionResult ARLog(FormCollection collection)
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("ARLog", "AccountReceivable", new { area = "Portal" }), "AR Log");
            TempData["dateType"] = collection["dateTypeList"].ToString();
            TempData["startDate"] = collection["startDate"].ToString();
            TempData["endDate"] = collection["endDate"].ToString();


            return RedirectToAction("ARLogList");
        }


        [HttpGet]
        public ActionResult ARLogReportData(string regionIds, string createddate)
        {
            List<ARLogListFinalViewModel> lstResult = AccountReceivableService.GetARLogListData(regionIds,
                DateTime.Parse(createddate));
            return PartialView("_PartialARLogReport", lstResult);
        }

        //Invoice Export To PDF File
        public FileResult ARLogReportExportToPDF(string regionIds, string createddate)
        {
            List<ARLogListFinalViewModel> lstResult = AccountReceivableService.GetARLogListData(regionIds,
                DateTime.Parse(createddate));
            string HTMLContent = RenderPartialViewToString("_PartialARLogReport", lstResult);
            return File(GetARPDF(HTMLContent), "application/pdf",
                "_PartialARLogReportPDF" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
        }

        public JsonResult ARLogReportPrint(string regionIds, string createddate)
        {
            List<ARLogListFinalViewModel> lstResult = AccountReceivableService.GetARLogListData(regionIds,
                DateTime.Parse(createddate));
            string HTMLContent = RenderPartialViewToString("_PartialARLogReportPDF", lstResult);
            var retPath = "/Upload/ARLogReportFiles/" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
            var path = Path.Combine(Server.MapPath("~/Upload/ARLogReportFiles/"),
                DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
            System.IO.File.WriteAllBytes(path, GetARPDF(HTMLContent)); // Requires System.IO
            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        public byte[] GetARPDF(string pHTML)
        {
            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                StyleSheet styles = new StyleSheet();
                styles.LoadStyle("t1col1", "border", "0.1");
                using (var document = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
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

            //#region -- styles --

            //StyleSheet styles = new StyleSheet();

            //styles.LoadStyle("tabborder", "border", ".01");

            //styles.LoadTagStyle("th", "size", "10pt");
            //styles.LoadTagStyle("td", "size", "10pt");


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
            //Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            //PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            //HTMLWorker htmlWorker = new HTMLWorker(doc);
            //htmlWorker.SetStyleSheet(styles);
            //doc.Open();

            ////var pages = HTMLWorker.ParseToList(txtReader, styles);
            ////foreach (var page in pages)
            ////{
            ////    if (page is PdfPTable)
            ////    {
            ////        (page as PdfPTable).SplitLate = false;
            ////    }
            ////    doc.Add(page as IElement);
            ////}
            //htmlWorker.StartDocument();
            //htmlWorker.Parse(txtReader);
            //htmlWorker.EndDocument();

            //htmlWorker.Close();
            //doc.Close();
            //bPDF = ms.ToArray();
            //return bPDF;
        }

        public ActionResult ARLogList()
        {
            ViewBag.CurrentMenu = "AccountsReceivableReports";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ARLogList", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("ARLogList", "AccountReceivable", new { area = "Portal" }), "Reports");
            BreadCrumb.Add(Url.Action("ARLogList", "AccountReceivable", new { area = "Portal" }), "AR Log List");
            string dateType = TempData["dateType"].ToString();
            string startDate = TempData["startDate"].ToString();
            string endDate = TempData["endDate"].ToString();

            //string dateType = "1";
            //string startDate = "2016-07-15 17:52:49.447";
            //string endDate = "2017-07-15 17:52:49.447";

            string s = "";

            if (dateType.ToString() == "2")
            {
                s = "With Transactions Dated ";
            }
            else
            {
                s = "Entered on ";
            }

            if (startDate.ToString() == endDate.ToString())
            {
                s += " on " + startDate.ToString();
            }
            else
            {
                s += " between " + startDate.ToString() + " and " + endDate.ToString();
            }

            ViewBag.subTitle = s;

            if (startDate == null || startDate.Trim().Length == 0)
                startDate = "-1";

            if (endDate == null || endDate.Trim().Length == 0)
                endDate = "-1";

            //AccountReceivableService serv = new AccountReceivableService();  // Sohel: Service has already a reference.
            List<ARLogViewModel> arLogList = AccountReceivableService.getARLogList(dateType, startDate, endDate);

            //return agingList;
            return View(arLogList);
        }

        #endregion Reports

        #region Credits

        // GET: Portal/AccountReceivable/CreditsSearch
        public ActionResult CreditsSearch()
        {
            ViewBag.CurrentMenu = "AccountsReceivableCredits";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }), "Customer Credits");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }), "Search");
            return View();
        }

        // GET: Portal/AccountReceivable/CreditList
        public ActionResult CreditList()
        {
            ViewBag.CurrentMenu = "AccountsReceivableCredits";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CreditList", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("CreditList", "AccountReceivable", new { area = "Portal" }), "Customer Credits");
            BreadCrumb.Add(Url.Action("CreditList", "AccountReceivable", new { area = "Portal" }), "List");

            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;


            return View();
        }

        [HttpGet]
        public ActionResult GetCustomerWiseCreditList(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int month = 0, int year = 0)
        {
            var data = AccountReceivableService.GetCustomerWiseCreditList(regionIds, spnStartDate, spnEndDate, month, year);


            var jsonResult = Json(new { aadata = data }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        // GET: Portal/AccountReceivable/PendingCreditList
        public ActionResult PendingCreditList()
        {
            ViewBag.CurrentMenu = "AccountsReceivableCredits";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PendingCreditList", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("PendingCreditList", "AccountReceivable", new { area = "Portal" }),
                "Customer Credits");
            BreadCrumb.Add(Url.Action("PendingCreditList", "AccountReceivable", new { area = "Portal" }),
                "Pending Approval");

            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = this.SelectedRegionId;
            return View();
        }

        // GET: Portal/AccountReceivable/CustomerCredits
        public ActionResult CustomerCredits()
        {
            ViewBag.CurrentMenu = "AccountsReceivableCredits";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CustomerCredits", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("CustomerCredits", "AccountReceivable", new { area = "Portal" }), "Customer Credits");
            BreadCrumb.Add(Url.Action("CustomerCredits", "AccountReceivable", new { area = "Portal" }), "Customer Credits");

            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        #endregion Credits

        #region Invoices

        // GET: Portal/AccountReceivable/InvoicesBulkPayment
        public ActionResult InvoicesBulkPayment()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }), "Invoice");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }),
                "Invoices Bulk Payment");
            return View();
        }

        // GET: Portal/AccountReceivable/InvoicesDaily
        [HttpGet]
        public ActionResult InvoicesDaily()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }),
                "Accounts Receivable");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }), "Invoice");
            BreadCrumb.Add(Url.Action("CreditsSearch", "AccountReceivable", new { area = "Portal" }), "Daily Invoices");
            /*
            var monthYear = new SelectList(db.vw_AR_DailyTransactionsTmp.
                OrderByDescending(x => x.datetime).
                Select(m => m.billyear).Distinct().ToList());
            */

            return View();
        }

        [HttpPost]
        public ActionResult InvoicesDaily(FormCollection collection, string command)
        {
            TempData["searchValue"] = collection["dateTime"].ToString();

            if (Request.Form["Print"] != null)
            {
                TempData["searchBy"] = "991";
                TempData["printOnly"] = "1";
            }
            else if (Request.Form["Email"] != null)
            {
                TempData["searchBy"] = "992";
                TempData["emailOnly"] = "1";
            }
            Console.WriteLine("email");

            TempData["invoicelist"] = collection["invoicelist"].ToString();
            return RedirectToAction("SearchInvoiceList");
        }

        public ActionResult SearchInvoiceList()
        {
            string searchBy = TempData["searchBy"].ToString();
            string emailOnly = TempData["emailOnly"].ToString();
            string printOnly = TempData["printOnly"].ToString();
            string searchValue = TempData["searchValue"].ToString();

            //if (startDate == null || startDate.Trim().Length == 0)
            //    startDate = "-1";

            //if (endDate == null || endDate.Trim().Length == 0)
            //    endDate = "-1";

            //AccountReceivableService serv = new AccountReceivableService();
            //List<ARLogViewModel> arLogList = serv.getSearchInvoiceList(searchBy, searchValue, emailOnly, printOnly);

            //return agingList;
            //return View(arLogList);
            return View();
        }

        // GET: Portal/AccountReceivable/InvoicesLockboxImport
        public ActionResult InvoicesLockboxImport()
        {
            BreadCrumb.Add(Url.Action("InvoicesLockboxImport", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            return View();
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

        // GET: Portal/AccountReceivable/InvoicesSearch
        public ActionResult InvoicesSearch()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InvoicesSearch", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            BreadCrumb.Add(Url.Action("InvoicesSearch", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("InvoicesSearch", "AccountReceivable", new { area = "Portal" }), "Search");

            ViewBag.billMonthsList = GetMonthsList();
            ViewBag.billYearList = GetYearsList().ToList().OrderByDescending(x => x.Text);

            ViewBag.selectedRegionId = SelectedRegionId;

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View();
        }

        [HttpPost]
        public ActionResult InvoicesSearch(FormCollection _collection, InvoiceSearchViewModel model)
        {
            InvoiceSearchViewModel one = new InvoiceSearchViewModel();
            one.billMonth = model.billMonth;
            one.billYear = model.billYear;
            one.consolidatedInvoice = model.consolidatedInvoice;
            one.eomOnly = model.eomOnly;
            one.filterBy = model.filterBy;
            one.openInvoiceOnly = model.openInvoiceOnly;
            one.searchBy = model.searchBy;
            one.searchValue = model.searchValue;


            return View("InvoiceSearchResult", one);
        }

        public ActionResult InvoiceSearchResult()
        {
            return View();
        }

        // GET: Portal/AccountReceivable/CreditSearch
        public ActionResult CreditSearch()
        {
            ViewBag.CurrentMenu = "CreditSearch";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("CreditSearch", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("CreditSearch", "AccountReceivable", new { area = "Portal" }), "Credit");
            BreadCrumb.Add(Url.Action("CreditSearch", "AccountReceivable", new { area = "Portal" }), "Credit Search");

            ViewBag.billMonthsList = GetMonthsList();
            ViewBag.billYearList = GetYearsList().ToList().OrderByDescending(x => x.Text);

            return View();
        }

        [HttpPost]
        public ActionResult CreditSearch(FormCollection _collection, CreditsSearchViewModel model)
        {
            CreditsSearchViewModel one = new CreditsSearchViewModel();
            one.billMonth = model.billMonth;
            one.billYear = model.billYear;
            one.searchBy = model.searchBy;
            one.searchValue = model.searchValue;

            return View("CreditSearchResult", one);
        }

        public ActionResult CreditSearchResult()
        {
            return View();
        }

        [HttpGet]
        public JsonResult PayWithCC(string CardNumber, string CardHolderName, string CardExpiry, string CardCVC,
            decimal Amount, string ClassID, bool IsProfile, string CustomerNo, bool IsCheckedPrevCard,
            string PaymentProfileID, string regID)
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
            if (RegionID == 0)
            {
                RegionID = Convert.ToInt32(regID);
            }
            PaymentGatewayDetail PGD =
                generalService.GetPaymentGatewayList()
                    .Where(r => r.RegionID == RegionID && r.IsActive == true)
                    .FirstOrDefault();
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
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                                  SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

                PriorityPayment.PpsApiRequest apiRequest = new PriorityPayment.PpsApiRequest(baseURL, ConsumerKey,
                    ConsumerSecret, PriorityPayment.AuthenticationMethod.OAuth);
                using (
                    var httpRequest = apiRequest.BuildRequest(createPayment, queryString,
                        System.Net.Http.HttpMethod.Post, p))
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

                #endregion

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
                    response = CreditCardPayment(objCreditCardType, objcustomerAddressType, PGD.LoginID,
                        PGD.TransactionKey, Amount, "TestMode", CustomerNo);
                }
                else
                {
                    PaymentProfileDetail =
                        jk.PaymentProfileDetails.Where(r => r.Id == new Guid(PaymentProfileID)).FirstOrDefault();

                    if (PaymentProfileDetail != null)
                    {
                        OrderTransactionID = OrderTransaction(PaymentProfileDetail, Amount);
                        response = AuthorizePaymentGateway.ChargeCustomerProfile(PGD.LoginID, PGD.TransactionKey,
                            PaymentProfileDetail.CustomerProfileID, PaymentProfileDetail.CustomerPaymentProfileID,
                            "TestMode", Amount);
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

        private dynamic CreditCardPayment(creditCardType objCreditCardType, customerAddressType objcustomerAddressType,
            string ApiLoginID, string ApiTransactionKey, decimal TotalOrderPrice, string PaymentGatewayMode,
            string OrderId)
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

            #endregion

            dynamic response = AuthorizePaymentGateway.ChargeCreditCard(ApiLoginID, ApiTransactionKey,
                Convert.ToDecimal(TotalOrderPrice), objCreditCardType, objcustomerAddressType, lineItems,
                PaymentGatewayMode, Convert.ToString(OrderId));

            return response;
        }

        private dynamic CreateCustomerProfileFromTransaction(PaymentGatewayDetail PGD, dynamic CCresponse)
        {
            dynamic response = AuthorizePaymentGateway.CreateCustomerProfileFromTransaction(PGD.LoginID,
                PGD.TransactionKey, CCresponse.transactionResponse.transId, "TestMode");
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


        private void CreateOrderTransactionsResponse(dynamic response, Guid orderId, Guid PaymentProfileID,
            PaymentGatewayDetail PGD)
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
                    objOrderTransactionsResponse.AvsResultCode =
                        Convert.ToString(response.transactionResponse.avsResultCode);

                    objOrderTransactionsResponse.CvvResultCode =
                        Convert.ToString(response.transactionResponse.cvvResultCode);
                    objOrderTransactionsResponse.CavvResultCode =
                        Convert.ToString(response.transactionResponse.cavvResultCode);

                    objOrderTransactionsResponse.TransId = Convert.ToString(response.transactionResponse.transId);
                    objOrderTransactionsResponse.RefTransId = Convert.ToString(response.transactionResponse.refTransID);
                    objOrderTransactionsResponse.TransHash = Convert.ToString(response.transactionResponse.transHash);

                    objOrderTransactionsResponse.AccountNumber =
                        Convert.ToString(response.transactionResponse.accountNumber);
                    objOrderTransactionsResponse.EntryMode = null;
                    objOrderTransactionsResponse.AccountType = Convert.ToString(response.transactionResponse.accountType);


                    if (response.messages.resultCode == messageTypeEnum.Ok)
                    {
                        objOrderTransactionsResponse.TMessagesCode =
                            Convert.ToString(response.transactionResponse.messages[0].code);
                        objOrderTransactionsResponse.TMessagesDescription =
                            Convert.ToString(response.transactionResponse.messages[0].description);
                    }
                    else
                    {
                        try
                        {
                            objOrderTransactionsResponse.TMessagesCode =
                                Convert.ToString(response.transactionResponse.errors[0].errorCode);
                            objOrderTransactionsResponse.TMessagesDescription =
                                Convert.ToString(response.transactionResponse.errors[0].errorText);
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
            bool isCustomerProfileCreatedSuccessfully =
                AuthorizenetCommon.CheckAuthorizeNetApiResponse(CustomerProfileResponse);
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
        public JsonResult InvoicesSearchResultData(int m = 0, int y = 0, string st = "", int fb = 0, bool eo = false,
            string sv = "", int sb = 0, bool cb = false, string region = "")
        {
            // month, year, searchtext, filterby, searchBy, searchValue, eomOnly, consolidatedInvoice, invoicetypelistid
            List<ARInvoiceListViewModel> lstARInvoiceListViewModel = AccountReceivableService.GetInvoiceListWithSearch(
                m, y, st, fb, sb, sv, eo, cb, 2, region);

            return Json(lstARInvoiceListViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CreditSearchResultData(int m = 0, int y = 0, string sv = "", int sb = 0)
        {
            var data = AccountReceivableService.GetCreditListWithSearch(m, y, sb, sv);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult InvoicesSearchResultData(int consolidatedInvoice,int searchBy,int billMonth,int billYear,int filterBy,int openInvoiceOnly,int eomOnly)
        //{
        //    List<ARInvoiceListViewModel> lstReturn = AccountReceivableService.GetInvoiceListWithSearch(0, 0, "", 2);
        //    return Json(lstReturn,JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult InvoicesSearchForCreditResultData(string rgId, string fd = "", string td = "", string st = "",
            bool closed = false, bool consolidated = false, int? customerId = null, int sb = 0)
        {
            rgId = rgId == "null" ? "" : rgId;
            DateTime fromDate = DateTime.Parse(fd);
            DateTime toDate = DateTime.Parse(td);

            List<ARInvoiceListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetInvoiceListWithSearchForCredit(rgId, fromDate, toDate, st, closed,
                    consolidated, sb, customerId == null ? 0 : Convert.ToInt32(customerId));

            //if (customerId != null)
            //    lstARInvoiceListViewModel = lstARInvoiceListViewModel.Where(o => o.CustomerId == customerId.Value).ToList();

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public JsonResult CustomerForCreditListResultData(int? rgId = null, string fd = "", string td = "",
            string st = "", bool closed = false, bool consolidated = false)
        {
            DateTime fromDate = DateTime.Parse(fd);
            DateTime toDate = DateTime.Parse(td);

            List<ARCustomerWithCreditListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetCreditList(rgId, fromDate, toDate, st, closed, consolidated);

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult PendingCreditListResultData(int? rgId = null, string fd = "", string td = "", string st = "",
            bool consolidated = false)
        {
            DateTime fromDate = DateTime.Parse(fd);
            DateTime toDate = DateTime.Parse(td);

            List<ARCustomerWithCreditListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetPendingCreditTempList(rgId, fromDate, toDate, st, consolidated);

            //List<ARCustomerWithCreditListViewModel> lstARInvoiceListViewModel =
            //   AccountReceivableService.GetPendingCreditList(rgId, fromDate, toDate, st, consolidated);

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult InvoicesSearchForPaymentResultData(string rgId, string date = "", string st = "",
            bool consolidated = false, string OCValue = "", int month = 0, int year = 0)
        {
            rgId = rgId == "null" ? "" : rgId;

            //DateTime fromDate = DateTime.Parse(fd);
            //DateTime toDate = DateTime.Parse(td);

            List<ARInvoiceListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetInvoiceListWithSearchForPayment(rgId, st, consolidated,
                    OCValue, month, year);

            DateTime sDate;
            DateTime eDate;
            if (!string.IsNullOrEmpty(date) && date.Contains("-") &&
                DateTime.TryParseExact(date.Split('-')[0], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out sDate) &&
                DateTime.TryParseExact(date.Split('-')[1], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out eDate) && eDate > sDate)
            {
                lstARInvoiceListViewModel = lstARInvoiceListViewModel.Where(o => o.InvoiceDate >= sDate && o.InvoiceDate <= eDate).ToList();
            }

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult InvoicesSearchForManualPayment(string rgId, string date = "", string st = "", int sb = 0, bool consolidated = false, string OCValue = "", int month = 0, int year = 0)
        {
            rgId = rgId == "null" ? "" : rgId;

            //DateTime fromDate = DateTime.Parse(fd);
            //DateTime toDate = DateTime.Parse(td);

            List<ARInvoiceListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetInvoiceWithSearchForManualPayment(rgId, OCValue, st, sb);
            decimal amt = 0M;
            List<Overflow> lstOverflows = new List<Overflow>();
            if (sb == 2)
            {
                //amt = AccountReceivableService.GetCustomerBalance(st);
                lstOverflows = AccountReceivableService.GetCustomerOverflowBalance(st, out amt);
            }

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true, CustomerBalance = amt, overflows = lstOverflows }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult InvoicesSearchForManualPaymentCustomerBalance(string customerno)
        {

            decimal _balAMT = 0;
            var amt = AccountReceivableService.GetCustomerBalance(customerno);
            var lstOverflows = AccountReceivableService.GetCustomerOverflowBalance(customerno, out _balAMT);

            return Json(_balAMT, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult InvoicesSearchForManualPaymentCustomerBalanceByOverPayment(string customerno)
        {

            decimal _balAMT = 0;
            var lstOverflows = AccountReceivableService.GetCustomerOverflowBalance(customerno, out _balAMT);

            return Json(new { balance = _balAMT, overflows = lstOverflows }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCustomerBalanceByOverPaymentById(int customerId)
        {

            decimal _balAMT = 0;
            var lstOverflows = AccountReceivableService.GetCustomerOverflowBalanceById(customerId, out _balAMT);

            return Json(new { balance = _balAMT, overflows = lstOverflows }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInvoiceWithSearchForBalanceAdjustment(string rgId, decimal amountFrom, decimal amountTo, string st = "", int sb = 0, string OCValue = "", int servicet = 76)
        {
            rgId = rgId == "null" ? "" : rgId;
            List<ARInvoiceListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetInvoiceWithSearchForBalanceAdjustment(rgId, OCValue, amountFrom, amountTo, st, sb, servicet);

            var jsonResult = Json(new { aaData = lstARInvoiceListViewModel, success = true }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion Invoices

        // GET: Portal/AccountReceivable/OtherDepositsAdd
        public ActionResult OtherDepositsAdd()
        {
            BreadCrumb.Add(Url.Action("OtherDepositsAdd", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            return View();
        }

        // GET: Portal/AccountReceivable/OtherDepositsView
        public ActionResult OtherDepositsView()
        {
            BreadCrumb.Add(Url.Action("OtherDepositsView", "AccountReceivable", new { area = "Portal" }),
                "Other Deposits View");
            return View();
        }

        // GET: Portal/AccountReceivable/ProcessBillRun
        public ActionResult ProcessBillRun()
        {
            BreadCrumb.Add(Url.Action("ProcessBillRun", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");

            ViewBag.billMonthsList = GetMonthsList();
            ViewBag.billYearList = GetYearsList();

            return View();
        }

        // GET: Portal/AccountReceivable/InvoicesSearch
        public ActionResult ProcessUndoBillRun()
        {
            BreadCrumb.Add(Url.Action("ProcessUndoBillRun", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            return View();
        }

        // GET: Portal/AccountReceivable/Overpaymentlist
        public ActionResult Overpaymentlist()
        {
            BreadCrumb.Add(Url.Action("Overpaymentlist", "AccountReceivable", new { area = "Portal" }), "Over Payment");
            return View();
        }

        // GET: Portal/AccountReceivable/Lockboximportresult
        public ActionResult Lockboximportresult()
        {
            BreadCrumb.Add(Url.Action("Lockboximportresult", "AccountReceivable", new { area = "Portal" }),
                "Lockbox import result");
            return View();
        }

        // GET: Portal/AccountReceivable/GenerateInvoice
        public ActionResult GenerateInvoice()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("GenerateInvoice", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            BreadCrumb.Add(Url.Action("GenerateInvoice", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("GenerateInvoice", "AccountReceivable", new { area = "Portal" }), "Pending Approval");

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View();
        }

        [HttpGet]
        public ActionResult GenerateInvoiceByID(string Ids, int status, string note = null)
        {
            string[] arrids = Ids.Split(',');

            foreach (string a in arrids)
            {
                int tmpId = int.Parse(a.Trim());
                bool success = AccountReceivableService.GenerateInvoice(tmpId, status);

                if (note == null) continue; // no note, no need to send message
                if (!success) continue; // generation failed, no need to send message

                var tmpDetail = AccountReceivableService.GetManualInvoiceDetail(tmpId);
                if (tmpDetail == null) continue;

                var customerId = tmpDetail?.CustomerTransactionItems?.FirstOrDefault()?.CustomerId;
                if (customerId == null) continue;

                AccountReceivableService.savePendingMessage(note, (int)customerId, status, tmpId);
            }

            return RedirectToAction("GenerateInvoice", "AccountReceivable", new { area = "Portal" });
        }

        [HttpGet]
        public ActionResult GenerateInvoiceByIDOnly(string Id, string note, int CustomerId)
        {
            AccountReceivableService.GenerateInvoice(int.Parse(Id.Trim()), 1);
            AccountReceivableService.savePendingMessage(note, CustomerId, 1, int.Parse(Id.Trim()));
            return RedirectToAction("GenerateInvoice", "AccountReceivable", new { area = "Portal" });
        }

        [HttpPost]
        public ActionResult GenerateInvoice(FormCollection frm)
        {
            BreadCrumb.Add(Url.Action("GenerateInvoice", "AccountReceivable", new { area = "Portal" }), "Generate Invoice");

            return View();
        }

        [HttpGet]
        public JsonResult InvoicesGenerateResultData(int? rgId = null)
        {
            List<portal_spGet_AR_GenerateInvoiceList_Result> lstGenerateInvoiceViewModel =
                new List<portal_spGet_AR_GenerateInvoiceList_Result>();
            //if (_claimView.GetCLAIM_ROLE_TYPE() == "Accounting-User")
            //{
            //    lstGenerateInvoiceViewModel =
            //        AccountReceivableService.GetGenerateInvoiceList(rgId)
            //            .Where(r => r.TransactionStatusListId == 12)
            //            .ToList();
            //}
            //else
            //{
            //    lstGenerateInvoiceViewModel = AccountReceivableService.GetGenerateInvoiceList(rgId);
            //}
            lstGenerateInvoiceViewModel = AccountReceivableService.GetGenerateInvoiceList(rgId);
            return Json(lstGenerateInvoiceViewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PastDue()
        {
            BreadCrumb.Add(Url.Action("PastDue", "AccountReceivable", new { area = "Portal" }), "Past Due");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            return View();
        }
        // GET: Portal/AccountReceivable/pastduestatementlist
        public ActionResult PastDueStatementlist(DateTime? reportDate, int? monthsToInclude, string regionIds)
        {
            BreadCrumb.Add(Url.Action("pastduestatementlist", "AccountReceivable", new { area = "Portal" }), "Past due statement list");

            ViewBag.ReportDate = reportDate;
            ViewBag.RegionIds = regionIds;
            ViewBag.MonthsToInclude = monthsToInclude;
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            var data = AccountReceivableService.GetAllPastDueStatement(reportDate, monthsToInclude, regionIds);
            ViewBag.RegionWiseTotal = data.GroupBy(x => x.RegionId).Select(x => new RegionGroupBy
            {
                RegionId = x.First().RegionId,
                RegionName = x.First().RegionName,
                FirstTotal = x.Sum(first => first.FirstSegment),
                SecondTotal = x.Sum(second => second.SecondSegment),
                ThirdTotal = x.Sum(third => third.ThirdSegment),
                FourthTotal = x.Sum(fourth => fourth.FourthSegment),
                FinalTotal = x.Sum(final => final.Total)
            }).ToList();
            return View(data);
        }

        //
        [HttpPost]
        public JsonResult CustomerAutoComplete(string namePrefix, string InvoicePrefix)
        {
            if (namePrefix == null)
            {
                NLogger.Error("Requested SummaryData is null or empty");
                return Json(new { success = false, message = "Data is null or empty" });
            }

            List<string> SearchData = null;
            if (InvoicePrefix == "1")
            {
                SearchData =
                    AccountReceivableService.GetCustomers(namePrefix.ToUpper(), InvoicePrefix)
                        .Where(x => x.Name.Contains(namePrefix.ToUpper()))
                        .Select(x => x.Name.Trim())
                        .ToList();
            }
            if (InvoicePrefix == "2")
            {
                SearchData =
                    AccountReceivableService.GetCustomers(namePrefix, InvoicePrefix)
                        .Where(x => x.InvoiceNo.Contains(namePrefix))
                        .Select(x => x.InvoiceNo.Trim())
                        .ToList();
            }

            return this.Json(SearchData, JsonRequestBehavior.AllowGet);
        }

        #region Transactions

        public ActionResult Transactions()
        {
            var stateData = jkEntityModel.StateLists.OrderBy(s => s.Name).ToList();
            List<SelectListItem> statesList =
                stateData.Select(one => new SelectListItem { Text = one.Name, Value = one.abbr }).ToList();
            ViewBag.CurrentMenu = "AccountsReceivableTransactions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Transactions", "Customer", new { area = "Portal" }), "Portal");
            BreadCrumb.Add(Url.Action("Transactions", "Customer", new { area = "Portal" }), "Transactions");
            ViewBag.BillingState = new SelectList(statesList, "Value", "Text");
            return View();
        }

        #region :: Bill Run ::

        [HttpGet]
        public ActionResult BillRun()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("BillRun", "AccountReceivable", new { area = "Portal" }), "Bill Run");

            var regionlist = _commonService.GetRegionList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.billYearList = GetYearsList();
            ViewBag.billMonthsList = new SelectList(_commonService.GetPeriodDropDownValues(PeriodDropDownName.MonthlyBillRun.ToString(), _claimView.GetCLAIM_SELECTED_PERIOD_ID()), "Value", "Text");

            return View();
        }

        [HttpGet]
        public JsonResult BillRunDetailData(int month, int year, int batchid, string selectedRegionId = "")
        {
            bool isClosed = AccountReceivableService.GetBillValidated(month, year, batchid);
            BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel = new BillRunSummaryDetailViewModel();
            if (!isClosed)
            {
                oBillRunSummaryDetailViewModel = AccountReceivableService.GetBillRunSummaryDetail(month, year, batchid, selectedRegionId);
            }
            return Json(new { PeriodClose = isClosed, data = oBillRunSummaryDetailViewModel }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult BillRunSummaryDetailJson(int month, int year, int batchid, string selectedRegionId = "")
        {
            BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel =
                AccountReceivableService.GetBillRunSummaryDetail(month, year, batchid, selectedRegionId);
            return Json(oBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GenerateInvoiceBillRun(int month, int year, string selectedRegionId = "")
        {
            BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel =
                AccountReceivableService.GenerateInvoiceBillRun(month, year, selectedRegionId);
            return Json(oBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UndoInvoiceBillRun(string batchid, int month, int year, string selectedRegionId = "")
        {
            if (batchid.Contains(','))
            {
                foreach (string _str in batchid.Split(','))
                {
                    if (_str.Trim() != "")
                    {
                        AccountReceivableService.GetUndoBillRun(int.Parse(_str.Trim()), month, year, selectedRegionId);
                    }
                }
            }
            else
            {
                if (batchid != "")
                    AccountReceivableService.GetUndoBillRun(int.Parse(batchid.Trim()), month, year, selectedRegionId);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }



            return Json(true, JsonRequestBehavior.AllowGet);
        }


        private FranchiseService serv = new FranchiseService();

        [HttpGet]
        public ActionResult InvoiceList(int month = 0, int year = 0)
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("InvoiceList", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("InvoiceList", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("InvoiceList", "AccountReceivable", new { area = "Portal" }), "InvoiceList");
            //var regionlist = _commonService.GetRegionList();
            //ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceListdata(int month = 0, int year = 0, string searchtext = "", string pe = "", string oc = "", string d = "", string r = "", string consolidated = "", int typeid = 0)
        {

            string _RegionId = r == "null" ? "" : r;
            string _ToDate = "";
            string _FromDate = "";
            if (!string.IsNullOrEmpty(d) && d.Contains("-") && d != "-")
            {
                _FromDate = d.Split('-')[0];
                _ToDate = d.Split('-')[1];
            }
            else if (month > 0 && year > 0)
            {
                var first = new DateTime(year, month, 1);
                var last = first.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");
                _FromDate = first.ToString("MM/dd/yyyy");
                _ToDate = last;
            }

            List<InvoiceListViewModel> invoiceList = AccountReceivableService.GetInvoiceList(_RegionId, _FromDate, _ToDate);

            if (!string.IsNullOrWhiteSpace(consolidated))
                invoiceList = invoiceList.Where(o => o.ConsolidatedInvoice == (consolidated == "Y" ? true : o.ConsolidatedInvoice)).ToList();

            if (typeid != 0)
            {
                if (typeid == 3)
                {
                    invoiceList = invoiceList.Where(o => o.MasterTrxTypeListId == 3).ToList();
                }
                else if (typeid == 1)
                {
                    invoiceList = invoiceList.Where(o => o.MasterTrxTypeListId != 3).ToList();
                }
            }

            decimal _InvoiceTotAmount = (decimal)invoiceList.Sum(i => i.InvoiceTotal);
            decimal _InvoiceCloseAmount = (decimal)invoiceList.Where(p => p.TransactionStatusListId == 6).Sum(i => i.InvoiceTotal);
            decimal _InvoiceOpenAmount = (decimal)invoiceList.Where(p => p.TransactionStatusListId != 6).Sum(i => i.InvoiceTotal);
            decimal _InvoiceOverdueAmount = (decimal)invoiceList.Where(o => o.DueDate < DateTime.UtcNow.Date && (o.TransactionStatusListId == 4 || o.TransactionStatusListId == 7)).Sum(o => o.InvoiceTotal);

            int _InvoiceTotCount = invoiceList.Count();
            int _InvoiceCloseCount = invoiceList.Where(p => p.TransactionStatusListId == 6).Count();
            int _InvoiceOpenCount = invoiceList.Where(p => p.TransactionStatusListId != 6).Count();
            int _InvoiceOverdueCount = invoiceList.Where(o => o.DueDate < DateTime.UtcNow.Date && (o.TransactionStatusListId == 4 || o.TransactionStatusListId == 7)).Count();

            if (!string.IsNullOrWhiteSpace(pe) && pe != "E" && pe.Length == 1)
                invoiceList = invoiceList.Where(o => o.EBill == true).ToList();
            else if (!string.IsNullOrWhiteSpace(pe) && pe != "P" && pe.Length == 1)
                invoiceList = invoiceList.Where(o => o.PrintInvoice == true).ToList();

            if (!string.IsNullOrWhiteSpace(oc) && oc.Length > 1)
                invoiceList = invoiceList.Where(o => o.IsOpen == "Y" || o.IsOpen == "N").ToList();
            else if (!string.IsNullOrWhiteSpace(oc))
                invoiceList = invoiceList.Where(o => o.IsOpen == oc).ToList();

            decimal _FTInvoiceAmount = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceAmount) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceAmount));
            decimal _FTInvoiceTax = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceTax) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceTax));
            decimal _FTInvoiceTotal = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceTotal) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceTotal));

            var jsonResult = Json(new
            {
                aadata = invoiceList.OrderByDescending(o => o.InvoiceDate),
                headerData = new
                {
                    Total = _InvoiceTotCount,
                    open = _InvoiceOpenCount,
                    closed = _InvoiceCloseCount,
                    overPaid = _InvoiceOverdueCount,
                    overDue = _InvoiceOverdueAmount,

                    totalOpen = _InvoiceOpenAmount,
                    totalClosed = _InvoiceCloseAmount,
                    totalOverPaid = _InvoiceOverdueCount,
                    totalOverDue = _InvoiceOverdueAmount,
                    totalInvoice = _InvoiceTotAmount,
                    ftInvoiceAmount = _FTInvoiceAmount,
                    ftInvoiceTax = _FTInvoiceTax,
                    ftInvoiceTotal = _FTInvoiceTotal
                }
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult InvoiceListdataDetail(int month = 0, int year = 0, string searchtext = "")
        {
            List<ARInvoiceListViewModel> lstARInvoiceListViewModel =
                AccountReceivableService.GetInvoiceListWithSearch(month, year, searchtext, 2);
            return Json(lstARInvoiceListViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult InvoiceDetail(int Id)
        {
            var ModelData = AccountReceivableService.GetInvoiceDetail(Id);

            ViewBag.CustBillingEmail = AccountReceivableService.GetCustomerBillingEmail(Convert.ToInt32(ModelData.InvoiceDetail.CustomerId));

            return PartialView("_PartialInvoiceDetail", ModelData);
        }

        [HttpGet]
        public ActionResult ConsolidatedInvoiceDetail(int Id)
        {
            ViewBag.ConsolidatedInvoiceId = Id;
            var ModelData = AccountReceivableService.GetConsolidatedInvoiceDetail(Id);
            //var ModelData = AccountReceivableService.GetInvoiceDetail(Id);            
            //ViewBag.CustBillingEmail = AccountReceivableService.GetCustomerBillingEmail(Convert.ToInt32(ModelData.InvoiceDetail.CustomerId));
            return PartialView("_PartialConsolidatedInvoiceDetail", ModelData);
        }

        [HttpGet]
        public JsonResult InvoiceRevertStatus(int Id)
        {
            var ModelData = AccountReceivableService.InvoiceRevert(Id);
            string _retVal = "Invoice revert Successfully.";
            string _status = "Success";

            if (!ModelData)
            {
                _retVal = "Invoice have credit or payment records.";
                _status = "Failure";
            }


            return Json(new { status = _status, message = _retVal }, JsonRequestBehavior.AllowGet);


        }


        [HttpGet]
        public JsonResult OpenInvoiceListdata(int regionid)
        {
            List<ARLBInvoiceListViewModel> invoiceList =
                AccountReceivableService.GetOpenInvoiceListForLockbox(regionid, "", "").Where(o => o.Balance > 0).ToList();


            var jsonResult = Json(new { aadata = invoiceList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult OpenInvoiceListdataNew(string rgId, string date = "", string st = "", int sb = 0, bool consolidated = false, string OCValue = "", int month = 0, int year = 0)
        {
            List<ARLBInvoiceListViewModel> invoiceList = new List<ARLBInvoiceListViewModel>();


            DateTime sDate;
            DateTime eDate;
            if (!string.IsNullOrEmpty(date) && date.Contains("-") &&
                DateTime.TryParseExact(date.Split('-')[0], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out sDate) &&
                DateTime.TryParseExact(date.Split('-')[1], "MM/dd/yyyy", CultureInfo.InstalledUICulture,
                    DateTimeStyles.None, out eDate) && eDate > sDate)
            {
                //invoiceList = invoiceList.Where(o => o.InvoiceDate >= sDate && o.InvoiceDate <= eDate.AddDays(1)).ToList();
                invoiceList = AccountReceivableService.GetOpenInvoiceListForLockbox(int.Parse(rgId), sDate.ToString("MM/dd/yyyy"), eDate.ToString("MM/dd/yyyy"), OCValue, consolidated).ToList();
            }
            else if (month > 0 && year > 0)
            {

                sDate = new DateTime(year, month, 1);
                eDate = sDate.AddMonths(1).AddDays(-1);

                invoiceList = AccountReceivableService.GetOpenInvoiceListForLockbox(int.Parse(rgId), sDate.ToString("MM/dd/yyyy"), eDate.ToString("MM/dd/yyyy"), OCValue, consolidated).ToList();
            }


            return Json(new { aadata = invoiceList }, JsonRequestBehavior.AllowGet);
        }

        #endregion :: Bill Run ::

        #region :: Customer Invoice Transaction ::

        [HttpGet]
        public ActionResult ManualInvoice()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("ManualInvoice", "AccountReceivable", new { area = "Portal" }), "Manual Invoice");

            ViewBag.ContractDetailServiceTypeList =
                AccountReceivableService.GetContractDetailServiceTypeList()
                    .Select(
                        one =>
                            new SelectListItem { Text = one.Name, Value = one.ContractDetailServiceTypeListId.ToString() })
                    .ToList();
            return View();
        }

        [HttpGet]
        public ActionResult BalanceAdjustment()
        {
            ViewBag.CurrentMenu = "BalanceAdjustment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("BalanceAdjustment", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("BalanceAdjustment", "AccountReceivable", new { area = "Portal" }), "Invoice");
            BreadCrumb.Add(Url.Action("BalanceAdjustment", "AccountReceivable", new { area = "Portal" }), "Balance Adjustment");
            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");
            ViewBag.PaymentMethodList = new SelectList(AccountReceivableService.GetAll_PaymentMethodList(),
                "PaymentMethodListId", "Name");
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;

            ViewBag.AdjustmentReasonList = new SelectList(AccountReceivableService.GetAll_AdjustmentReasonList(),
               "AdjustmentReasonListId", "Name");

            var ServiceTypeListModel = CustomerService.GetServiceTypeList().Where(x => x.level == "A").ToList();
            ViewBag.ServiceTypeList = new SelectList(ServiceTypeListModel,
                "ServiceTypeListid", "name");
            return View();
        }

        [HttpPost]
        public ActionResult ApplyBalanceAdjustment(FormCollection frm)
        {
            var slServiceTypeList = frm["slServiceTypeList"];
            var action = frm["action"];
            var isForCancellation = (frm["isForCancellation"] != null ? bool.Parse(frm["isForCancellation"]) : false);
            var isRefund = !String.IsNullOrEmpty(frm["hdfCustBA_Refund"]) ? bool.Parse(frm["hdfCustBA_Refund"]) : false;
            int CancellationMaintenancetempId = (frm["isForCancellationMaintenancetempId"] != null ? int.Parse(frm["isForCancellationMaintenancetempId"]) : 0);
            var vm = _GetAdjustmentTransactionViewModelFromForm(frm);
            int _adMasterTrxId = 0;
            foreach (var item in vm)
            {
                AccountReceivableService.InsertOrUpdateBalanceAdjustment(item, slServiceTypeList == "77" ? 43 : 61, out _adMasterTrxId);
            }
            if (isRefund)
                AccountReceivableService.InsertOrUpdateBalanceAdjustmentRefund(_GetAdjustmentTransactionViewModelFromRefund(frm), _adMasterTrxId);

            var retPage = action == "SaveClose" ? "CreditList" : "CustomerCredits";
            var retPath = Url.Action(retPage, "AccountReceivable", new { area = "Portal" });

            var afterSave = frm["SaveMethod"];
            if (afterSave == "SaveNew")
            {
                return RedirectToAction("BalanceAdjustment", "AccountReceivable", new { area = "Portal" });
            }
            else if (afterSave == "SaveClose")
            {
                return RedirectToAction("InvoiceList", "AccountReceivable", new { area = "Portal" });
            }

            return View();
        }

        [HttpGet]
        public ActionResult ManualInvoiceDetail(int Id)
        {
            return PartialView("_PartialManualInvoiceDetail", AccountReceivableService.GetManualInvoiceDetail(Id));
        }

        [HttpGet]
        public ActionResult ManualInvoiceEdit(int Id)
        {
            ViewBag.ContractDetailServiceTypeList =
                AccountReceivableService.GetContractDetailServiceTypeList()
                    .Select(
                        one =>
                            new SelectListItem { Text = one.Name, Value = one.ContractDetailServiceTypeListId.ToString() })
                    .ToList();
            List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
            MessageData =
                _commonService.GetDashboardPendingData(null)
                    .Where(r => r.MasterTmpTrxId == Id)
                    .OrderBy(r => r.MessageDate)
                    .ToList<PendingDashboardDataModel>();
            ManualInvoiceDetailViewModel manualInvoiceDetailViewModel = new ManualInvoiceDetailViewModel();
            manualInvoiceDetailViewModel = AccountReceivableService.GetManualInvoiceDetail(Id);
            manualInvoiceDetailViewModel.PendingDashboardDataModel = MessageData;
            manualInvoiceDetailViewModel.USERID = int.Parse(_claimView.GetCLAIM_USERID());
            return PartialView("_PartialManualInvoiceEdit", manualInvoiceDetailViewModel);
        }

        [HttpGet]
        public ActionResult ManualInvoiceDelete(int Id)
        {
            AccountReceivableService.DeleteManualInvoice(Id);
            return RedirectToAction("GenerateInvoice", "AccountReceivable", new { area = "Portal" });
        }


        [HttpPost]
        public ActionResult ManualInvoiceEdit(FormCollection frm)
        {
            CustomerTransactionCommonViewModel oCustomerTransactionCommonViewModel =
                new CustomerTransactionCommonViewModel();
            List<CustomerTransactionViewModel> lstCustomerTransactionViewModel =
                new List<CustomerTransactionViewModel>();
            List<FranchiseeTransactionViewModel> lstFranchiseeTransactionViewModel =
                new List<FranchiseeTransactionViewModel>();
            CustomerTransactionViewModel oCustomerTransactionViewModel;
            FranchiseeTransactionViewModel oFranchiseeTransactionViewModel;

            int MasterTmpTrxId = frm["MasterTmpTrxId"] != null ? int.Parse(frm["MasterTmpTrxId"]) : 0;

            oCustomerTransactionCommonViewModel.MasterTmpTrxId = MasterTmpTrxId;

            decimal decRes = 0;

            var TaxRateId = frm["hdfTaxRateId"] != null ? Convert.ToInt32(Convert.ToDecimal(frm["hdfTaxRateId"])) : 0;
            var ContractTaxRate = decimal.TryParse(frm["hdfContractTaxRate"], out decRes) ? decRes : 0;
            var LeaseTaxRate = decimal.TryParse(frm["hdfLeaseTaxRate"], out decRes) ? decRes : 0;
            var SupplyTaxRate = decimal.TryParse(frm["hdfSupplyTaxRate"], out decRes) ? decRes : 0;

            oCustomerTransactionCommonViewModel.BillMonth = 0;
            oCustomerTransactionCommonViewModel.BillYear = 0;
            oCustomerTransactionCommonViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            oCustomerTransactionCommonViewModel.CreatedDate = DateTime.Now;
            oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            oCustomerTransactionCommonViewModel.InvoiceDate = DateTime.Parse(frm["txtInvoicedate"]);

            oCustomerTransactionCommonViewModel.InvoiceDescription = frm["txtInvoiceDescription"] != null
                ? frm["txtInvoiceDescription"].ToString()
                : "";

            for (int i = 1; i <= int.Parse(frm["hdftotallineno"]); i++)
            {
                oCustomerTransactionViewModel = new CustomerTransactionViewModel();

                //oCustomerTransactionViewModel.Commission = bool.Parse(frm["chkCommission" + i] != null ? "true" : "false");
                //oCustomerTransactionViewModel.CommissionTotal =Decimal.Parse(String.IsNullOrEmpty(frm["txtCommissionAmount" + i])? "0": frm["txtCommissionAmount" + i]);
                //oCustomerTransactionViewModel.ExtraWork = bool.Parse(frm["chkExtraWork" + i] != null ? "true" : "false");
                // oCustomerTransactionViewModel.AccountRebate = bool.Parse(frm["chkAcctRebate" + i] != null ? "true" : "false");
                //oCustomerTransactionViewModel.BPPAdmin = bool.Parse(frm["chkBPPAdmin" + i] != null ? "true" : "false");
                if (int.Parse(frm["tablerdo" + i]) == 1)
                {
                    oCustomerTransactionViewModel.Commission = true;
                    oCustomerTransactionViewModel.CommissionTotal = Decimal.Parse(String.IsNullOrEmpty(frm["txtCommissionAmount" + i]) ? "0" : frm["txtCommissionAmount" + i]);
                }
                else
                {
                    oCustomerTransactionViewModel.Commission = false;
                    oCustomerTransactionViewModel.CommissionTotal = 0;
                }
                if (int.Parse(frm["tablerdo" + i]) == 2) { oCustomerTransactionViewModel.ExtraWork = true; }
                else { oCustomerTransactionViewModel.ExtraWork = false; }

                if (int.Parse(frm["tablerdo" + i]) == 4) { oCustomerTransactionViewModel.ClientSupplies = true; }
                else { oCustomerTransactionViewModel.ClientSupplies = false; }

                oCustomerTransactionViewModel.TaxExcempt = bool.Parse(frm["chkTaxExcempt" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.AccountRebate = false;// bool.Parse(frm["chkAcctRebate" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.BPPAdmin = false; //bool.Parse(frm["chkBPPAdmin" + i] != null ? "true" : "false");               
                oCustomerTransactionViewModel.CustomerTransactionId = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.ServiceTypeListId = int.Parse(frm["ContractDetailServiceTypeList" + i]);
                oCustomerTransactionViewModel.LineNo = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.Description = frm["txtdescription" + i].ToString();
                oCustomerTransactionViewModel.MarkUpTotal =
                    decimal.Parse(frm["txtmarkup" + i] != "" ? frm["txtmarkup" + i] : "0");
                oCustomerTransactionViewModel.Quantity = int.Parse(frm["txtqty" + i]);
                oCustomerTransactionViewModel.UnitPrice =
                    decimal.Parse(frm["txtunitprice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.ExtendedPrice =
                    decimal.Parse(frm["txtExtendedPrice" + i].ToString().Replace("$", "").Replace(",", ""));
                // oCustomerTransactionViewModel.Quantity * oCustomerTransactionViewModel.UnitPrice;
                oCustomerTransactionViewModel.TaxAmount =
                    decimal.Parse(frm["txttax" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.Total =
                    decimal.Parse(frm["txttotal" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TransactionStatusListId = 4;

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
                    if (frm["hdfFrenchiseeId" + j + "_" + i] != null)
                    {
                        oFranchiseeTransactionViewModel = new FranchiseeTransactionViewModel();
                        oFranchiseeTransactionViewModel.FranchiseeTransactionId = j;
                        oFranchiseeTransactionViewModel.Amount =
                            decimal.Parse(
                                frm["frfranchiseeamount" + j + "_" + i].ToString().Replace("$", "").Replace(",", ""));
                        oFranchiseeTransactionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                        oFranchiseeTransactionViewModel.CreatedDate = DateTime.Now;
                        oFranchiseeTransactionViewModel.CustomerTransactionId =
                            oCustomerTransactionViewModel.CustomerTransactionId;
                        oFranchiseeTransactionViewModel.FranchiseeId = int.Parse(frm["hdfFrenchiseeId" + j + "_" + i]);
                        oFranchiseeTransactionViewModel.IsDelete = false;
                        oFranchiseeTransactionViewModel.TransactionStatusListId = 4;
                        lstFranchiseeTransactionViewModel.Add(oFranchiseeTransactionViewModel);
                    }
                }
            }
            oCustomerTransactionCommonViewModel.CustomerTransactions = lstCustomerTransactionViewModel;
            oCustomerTransactionCommonViewModel.FranchiseeTransactions = lstFranchiseeTransactionViewModel;
            bool retVal = AccountReceivableService.InsertCustomerTransaction(oCustomerTransactionCommonViewModel);

            return RedirectToAction("GenerateInvoice", "AccountReceivable", new { area = "Portal" });
        }

        public ActionResult UpdateApproveReject(int CustomerId, string Note, int Status, int MasterTmpTrxId)
        {
            if (CustomerId > 0)
            {
                AccountReceivableService.GenerateInvoice(MasterTmpTrxId, Status);
                AccountReceivableService.savePendingMessage(Note, CustomerId, Status, MasterTmpTrxId);
            }
            return Json(new { Data = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DashboardRejectedActionClick(int CustomerId, int MID)
        {
            TempData["CustomerID"] = CustomerId;
            TempData["MID"] = MID;
            bool flag = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data =
                    context.NotificationMessageForDashboards.Where(r => r.CustomerID == CustomerId && r.MID == MID)
                        .FirstOrDefault();
                if (data != null)
                {
                    if (_claimView.GetCLAIM_USERID() != data.UID.ToString())
                    {
                        flag = false;
                    }
                }
            }
            TempData["flag"] = flag;
            return RedirectToAction("GenerateInvoice");
        }

        [HttpPost]
        public ActionResult ManualInvoice(FormCollection frm)
        {
            CustomerTransactionCommonViewModel oCustomerTransactionCommonViewModel =
                new CustomerTransactionCommonViewModel();
            List<CustomerTransactionViewModel> lstCustomerTransactionViewModel =
                new List<CustomerTransactionViewModel>();
            List<FranchiseeTransactionViewModel> lstFranchiseeTransactionViewModel =
                new List<FranchiseeTransactionViewModel>();
            CustomerTransactionViewModel oCustomerTransactionViewModel;
            FranchiseeTransactionViewModel oFranchiseeTransactionViewModel;

            var TaxRateId = !String.IsNullOrEmpty(frm["hdfTaxRateId"].ToString()) ? int.Parse(frm["hdfTaxRateId"]) : 0;
            var ContractTaxRate = !String.IsNullOrEmpty(frm["hdfContractTaxRate"].ToString()) ? decimal.Parse(frm["hdfContractTaxRate"]) : 0;
            var LeaseTaxRate = !String.IsNullOrEmpty(frm["hdfLeaseTaxRate"].ToString()) ? decimal.Parse(frm["hdfLeaseTaxRate"]) : 0;
            var SupplyTaxRate = !String.IsNullOrEmpty(frm["hdfSupplyTaxRate"].ToString()) ? decimal.Parse(frm["hdfSupplyTaxRate"]) : 0;

            oCustomerTransactionCommonViewModel.BillMonth = 0;
            oCustomerTransactionCommonViewModel.BillYear = 0;
            oCustomerTransactionCommonViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
            oCustomerTransactionCommonViewModel.CreatedDate = DateTime.Now;
            if (Request.Form["SaveClose1"] != null)
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerDetail.CustomerId"]);
            else
                oCustomerTransactionCommonViewModel.CustomerId = int.Parse(frm["CustomerId"]);
            try
            {
                oCustomerTransactionCommonViewModel.InvoiceDate = DateTime.Parse(frm["txtInvoicedate"]);
            }
            catch (Exception ex)
            {
                oCustomerTransactionCommonViewModel.InvoiceDate = DateTime.Now;
            }
            oCustomerTransactionCommonViewModel.InvoiceDescription = frm["txtInvoiceDescription"] != null
                ? frm["txtInvoiceDescription"].ToString()
                : "";

            for (int i = 1; i <= int.Parse(frm["hdftotallineno"]); i++)
            {
                oCustomerTransactionViewModel = new CustomerTransactionViewModel();
                oCustomerTransactionViewModel.CustomerTransactionId = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.ServiceTypeListId = int.Parse(frm["ContractDetailServiceTypeList" + i]);
                oCustomerTransactionViewModel.LineNo = int.Parse(frm["txtlinenumber" + i]);
                oCustomerTransactionViewModel.Description = frm["txtdescription" + i].ToString();
                oCustomerTransactionViewModel.MarkUpTotal =
                    decimal.Parse(frm["txtmarkup" + i] != "" ? frm["txtmarkup" + i] : "0");
                oCustomerTransactionViewModel.Quantity = int.Parse(frm["txtqty" + i]);
                oCustomerTransactionViewModel.UnitPrice =
                    decimal.Parse(frm["txtunitprice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.ExtendedPrice =
                    decimal.Parse(frm["txtExtendedPrice" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TaxAmount =
                    decimal.Parse(frm["txttax" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.Total =
                    decimal.Parse(frm["txttotal" + i].ToString().Replace("$", "").Replace(",", ""));
                oCustomerTransactionViewModel.TransactionStatusListId = 4;

                if (int.Parse(frm["tablerdo" + i]) == 1)
                {
                    oCustomerTransactionViewModel.Commission = true;
                    oCustomerTransactionViewModel.CommissionTotal = Decimal.Parse(frm["txtCommissionAmount" + i]);
                }
                else
                {
                    oCustomerTransactionViewModel.Commission = false;
                }

                if (int.Parse(frm["tablerdo" + i]) == 2) { oCustomerTransactionViewModel.ExtraWork = true; }
                else { oCustomerTransactionViewModel.ExtraWork = false; }

                if (int.Parse(frm["tablerdo" + i]) == 4) { oCustomerTransactionViewModel.ClientSupplies = true; }
                else { oCustomerTransactionViewModel.ClientSupplies = false; }

                //oCustomerTransactionViewModel.Commission =
                //    bool.Parse(frm["rdoAdditional" + i] != null ? "true" : "false");
                //oCustomerTransactionViewModel.CommissionTotal =
                //    Decimal.Parse(String.IsNullOrEmpty(frm["txtCommissionAmount" + i])
                //        ? "0"
                //        : frm["txtCommissionAmount" + i]);
                //oCustomerTransactionViewModel.ExtraWork = bool.Parse(frm["rdoExtraWork" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.TaxExcempt =
                    bool.Parse(frm["chkTaxExcempt" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.AccountRebate = false;
                //bool.Parse(frm["chkAcctRebate" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.BPPAdmin = false;// bool.Parse(frm["chkBPPAdmin" + i] != null ? "true" : "false");
                oCustomerTransactionViewModel.PrintInvoice = bool.Parse(frm["membership"] == "1" ? "true" : "false");

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
                    if (frm["hdfFrenchiseeId" + j] != null &&
                            (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == i ||
                             int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1))
                    {
                        oFranchiseeTransactionViewModel = new FranchiseeTransactionViewModel();
                        oFranchiseeTransactionViewModel.FranchiseeTransactionId = j;
                        if (int.Parse(frm["ddldetaillinenumber" + j].ToString()) == -1)
                            oFranchiseeTransactionViewModel.Amount = oCustomerTransactionViewModel.ExtendedPrice;
                        else
                            oFranchiseeTransactionViewModel.Amount =
                                decimal.Parse(frm["frfranchiseeamount" + j].ToString().Replace("$", "").Replace(",", ""));

                        oFranchiseeTransactionViewModel.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                        oFranchiseeTransactionViewModel.CreatedDate = DateTime.Now;
                        oFranchiseeTransactionViewModel.CustomerTransactionId = oCustomerTransactionViewModel.CustomerTransactionId;
                        oFranchiseeTransactionViewModel.FranchiseeId = int.Parse(frm["hdfFrenchiseeId" + j]);
                        oFranchiseeTransactionViewModel.IsDelete = false;
                        oFranchiseeTransactionViewModel.TransactionStatusListId = 4;
                        lstFranchiseeTransactionViewModel.Add(oFranchiseeTransactionViewModel);
                    }
                }
            }

            oCustomerTransactionCommonViewModel.CustomerTransactions = lstCustomerTransactionViewModel;
            oCustomerTransactionCommonViewModel.FranchiseeTransactions = lstFranchiseeTransactionViewModel;
            bool retVal = AccountReceivableService.InsertCustomerTransaction(oCustomerTransactionCommonViewModel);


            if (retVal)
            {
                //var GetCustomerInfo = CustomerService.GetCustomerDetailsById(CustomerId);
                var GetCustomerInfo =
                    jkEntityModel.Customers.Where(x => x.CustomerId == oCustomerTransactionCommonViewModel.CustomerId)
                        .FirstOrDefault();


                if (GetCustomerInfo != null)
                {
                    var RegionName =
                        jkEntityModel.Regions.Where(x => x.RegionId == GetCustomerInfo.RegionId).FirstOrDefault();

                    #region Email send function According to Admin configuration

                    //Get Feature Type Id by Feature Name
                    var getNew_invoice_Submit =
                        jkEntityModel.FeatureTypes.Where(
                                x => x.FeatureName == FeatureNameModel.New_Invoice_Create.ToString().Replace("_", " "))
                            .FirstOrDefault();

                    if (getNew_invoice_Submit != null && getNew_invoice_Submit.FeatureTypeId > 0)
                    {
                        //Get Feature Type Email Id by Feature Type Id
                        var messageDetails =
                            jkEntityModel.FeatureTypeEmails.Where(
                                x => x.FeatureTypeId == getNew_invoice_Submit.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                        if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                        {
                            //Get Feature Email Body By Message Name Or Tempalte Name 
                            var messageBody =
                                jkEntityModel.MailMessageTemplates.Where(
                                        x => x.MailMessageTemplateId == messageDetails.MailMessageTemplateId)
                                    .FirstOrDefault();
                            //MailMessageTemplateModel objItem = _commonService.GetEmailTemplate(MessageNameModel.FranchiseSecion1.ToString());
                            if (messageBody.MailMessageTemplateId > 0)
                            {
                                messageBody.MessageBody = messageBody.MessageBody.Replace("<<custname>>",
                                    !string.IsNullOrWhiteSpace(GetCustomerInfo.Name) ? GetCustomerInfo.Name : GetCustomerInfo.Name2);
                                messageBody.MessageBody = messageBody.MessageBody.Replace("<<regionname>>",
                                    RegionName.Name);

                                _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId,
                                    messageBody.MessageBody, messageBody.Subject);

                                var CustEmailId =
                                    jkEntityModel.Emails.Where(
                                        x =>
                                            x.TypeListId == 1 &&
                                            x.ClassId == oCustomerTransactionCommonViewModel.CustomerId &&
                                            x.ContactTypeListId == 6).FirstOrDefault();
                                //Email send to Franchise if Admin Set to True 
                                if (messageDetails.EmailToCustomer == true && GetCustomerInfo != null &&
                                    CustEmailId != null && !string.IsNullOrWhiteSpace(CustEmailId.EmailAddress))
                                {
                                    _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail,
                                        CustEmailId.EmailAddress, messageBody.MessageBody, messageBody.Subject);
                                }
                            }
                        }
                    }

                    #endregion
                }
            }

            if (frm["hdfCallBySave"] == "SaveNew")
            {
                return RedirectToAction("ManualInvoice", "AccountReceivable", new { area = "Portal" });
            }
            else if (frm["hdfCallBySave"] == "SaveClose")
            {
                return RedirectToAction("InvoiceList", "AccountReceivable", new { area = "Portal" });
            }
            else if (Request.Form["SaveClose1"] != null)
            {
                return RedirectToAction("CustomerDetail", "Customer",
                    new { area = "Portal", id = int.Parse(frm["CustomerDetail.CustomerId"]) });
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetContractDetailServiceTypeList()
        {
            //List<ContractDetailServiceTypeList> lstContractDetailServiceTypeList =
            //    AccountReceivableService.GetContractDetailServiceTypeList().OrderBy(o => o.Name).ToList();
            //return Json(lstContractDetailServiceTypeList, JsonRequestBehavior.AllowGet);
            List<ServiceTypeList> lstServiceTypeList =
               CustomerService.GetServiceTypeList().Where(x => x.TypeListId == 1).OrderBy(o => o.name).ToList();
            return Json(lstServiceTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Customerdata(string keyword)
        {
            var lstCustomer = AccountReceivableService.GetCustomerListData(keyword);

            return Json(lstCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCustomerdata(string keyword = "")
        {
            var lstCustomer = AccountReceivableService.GetCustomerListData(keyword);

            return Json(lstCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerDetaildata(int customerid)
        {
            return Json(AccountReceivableService.GetCustomerDetailData(customerid), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerDistributionDetailFranchiseedata(int customerid)
        {
            var ab = AccountReceivableService.GetCustomerDistributionDetailFranchiseedata(customerid);
            return Json(ab, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Frenchiseedata(string keyword = "")
        {
            List<Franchisee> lstFranchisee = new List<Franchisee>();
            if (!String.IsNullOrEmpty(keyword))
                lstFranchisee = AccountReceivableService.GetFranchiseeListData(keyword);
            else
                lstFranchisee = AccountReceivableService.GetFranchiseeListData();

            var result = from fren in lstFranchisee select new { fren.FranchiseeId, fren.Name, fren.FranchiseeNo };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion :: Customer Invoice Transaction ::

        #region :: Payments ::

        [HttpGet]
        public ActionResult ManualPayment()
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("ManualPayment", "AccountReceivable", new { area = "Portal" }), "Account Receivable");
            BreadCrumb.Add(Url.Action("ManualPayment", "AccountReceivable", new { area = "Portal" }), "Payment");
            BreadCrumb.Add(Url.Action("ManualPayment", "AccountReceivable", new { area = "Portal" }), "Manual Payment");
            ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_SearchDateList(), "SearchDateListId",
                "Name");

            ViewBag.PaymentMethodList = new SelectList(AccountReceivableService.GetAll_PaymentMethodList(),
                "PaymentMethodListId", "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            //CustomerGateway cg = new CustomerGateway("9Q8vG2ha7", "73AdwcKb8GV9323L", ServiceMode.Test);
            //string[] str = cg.GetCustomerIDs();
            ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
            return View();
        }

        [HttpGet]
        public JsonResult GetCustomerCreditBalance(int customerId)
        {
            var balance = AccountReceivableService.GetCustomerCreditBalance(customerId);

            return Json(balance, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApplyManualPayment(FormCollection frm)
        {
            int paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentType"].ToString().Trim()) ? int.Parse(frm["slPaymentType"].ToString().Trim()) : 0;
            string referenceNo = !String.IsNullOrEmpty(frm["referenceNo"].ToString()) ? frm["referenceNo"].ToString() : "";
            string notes = !String.IsNullOrEmpty(frm["txtNotes"].ToString()) ? frm["txtNotes"].ToString() : "";
            int ClassId = !String.IsNullOrEmpty(frm["hdfCustomerId"].ToString()) ? int.Parse(frm["hdfCustomerId"].ToString()) : -1;
            string Last4CC = !String.IsNullOrEmpty(frm["Last4CC"].ToString()) ? frm["Last4CC"].ToString().Replace("XXXX", "") : "";
            DateTime paymentDate = !String.IsNullOrEmpty(frm["paymentDate"].ToString()) ? DateTime.Parse(frm["paymentDate"]) : DateTime.Now;
            decimal paymentAmt = !string.IsNullOrEmpty(frm["paymentAmt"]) ? Decimal.Parse(frm["paymentAmt"]) : 0.00M;
            decimal creditAmt = !string.IsNullOrEmpty(frm["customerCreditAmt"]) ? Decimal.Parse(frm["customerCreditAmt"]) : 0.00M;
            decimal balance = !string.IsNullOrEmpty(frm["balance"]) ? Decimal.Parse(frm["balance"]) : 0.00M;
            bool mp_chkApplyCredit = !String.IsNullOrEmpty(frm["mp_chkApplyCredit"]) ? bool.Parse(frm["mp_chkApplyCredit"]) : false;
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

                    var invCustomerId = !String.IsNullOrEmpty(frm[invChunk + "_customerId"])
                       ? int.Parse(frm[invChunk + "_customerId"])
                       : 0;

                    //InvoiceDetail 
                    CreditDetailViewModel invoiceDetail = AccountReceivableService.GetCreditDetailForInvoicePayment(invId);
                    //invoiceDetail.InvoiceAmount;
                    invBalance = invoiceDetail.InvoiceBalance - invPayment;

                    oManualInvoice = new MPInvoiceViewModel();
                    oManualInvoice.InvoiceId = invId;
                    oManualInvoice.InvoiceCustomerId = invCustomerId;
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


                    if (invoiceDetail.Invoice.InvoiceDetailItems.Count() > 0)
                    {

                        decimal _itemBalance = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Balance);
                        decimal _itemTAXAmount = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.TAXAmount);
                        decimal _itemTotal = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Total);

                        //var item = invoiceDetail.Invoice.InvoiceDetailItems[0];
                        bool foundFields = true;
                        decimal itemPaymentAmt = 0;
                        decimal itemTotal = 0;

                        // sanity check to see if payment details were set, whole invoice was paid, or there is only one line item
                        if (oManualInvoice.PaidInFull || invoiceDetail.Invoice.InvoiceDetailItems.Count == 1)
                        {
                            //var taxRate = item.TAXAmount / item.Total;
                            itemPaymentAmt = applyAmountforPartialPay;
                            //var taxAmount = (decimal)(applyAmountforPartialPay * taxRate);
                            //itemTotal = applyAmountforPartialPay - taxAmount;

                            if (invoiceDetail.InvoiceBalance > 0)
                            {
                                if (invoiceDetail.InvoiceBalance <= applyAmountforPartialPay)
                                {
                                    invPayment = (decimal)invoiceDetail.InvoiceBalance;
                                    applyAmountforPartialPay = applyAmountforPartialPay - (decimal)invoiceDetail.InvoiceBalance;
                                }
                                else
                                {
                                    invPayment = applyAmountforPartialPay;
                                    applyAmountforPartialPay = 0;
                                }
                                var taxRate = _itemTAXAmount / _itemTotal;
                                itemPaymentAmt = invPayment;
                                var taxAmount = (decimal)(invPayment * taxRate);
                                itemTotal = invPayment - taxAmount;
                            }

                        }
                        else if (!oManualInvoice.PaidInFull && invoiceDetail.FranchiseeItems.Count() == 1)
                        {
                            if (invBalance > 0)
                            {
                                if (invoiceDetail.InvoiceBalance <= applyAmountforPartialPay)
                                {
                                    invPayment = (decimal)invoiceDetail.InvoiceBalance;
                                    applyAmountforPartialPay = applyAmountforPartialPay - (decimal)invBalance;
                                }
                                else
                                {
                                    invPayment = applyAmountforPartialPay;
                                    applyAmountforPartialPay = 0;
                                }
                                var taxRate = _itemTAXAmount / _itemTotal;
                                itemPaymentAmt = invPayment;
                                var taxAmount = (decimal)(invPayment * taxRate);
                                itemTotal = invPayment - taxAmount;
                            }

                        }
                        else
                        {
                            foundFields = foundFields &&
                                          decimal.TryParse(frm[invChunk + "_paymentAmt"],
                                              out itemPaymentAmt);
                            foundFields = foundFields &&
                                          decimal.TryParse(frm[invChunk + "_total"],
                                              out itemTotal);
                        }

                        if (foundFields)
                        //if (foundFields && itemTotal > 0)
                        {
                            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
                            mpvm.MasterTrxDetailId = -1;
                            mpvm.LineNo = -1;
                            mpvm.PaymentAmount = itemPaymentAmt;
                            mpvm.Tax = itemPaymentAmt - itemTotal;
                            mpvm.Total = itemPaymentAmt;
                            mpvm.ExtendedPrice = itemTotal;
                            mpcvm.Payments.Add(mpvm);


                            //mpvm.MasterTrxDetailId = -1;
                            //mpvm.LineNo = -1;
                            //mpvm.PaymentAmount = itemTotal;
                            //mpvm.Tax = itemTotal - itemPaymentAmt;
                            //mpvm.Total = itemTotal;
                            //mpvm.ExtendedPrice = itemPaymentAmt;
                            //mpcvm.Payments.Add(mpvm);
                        }
                    }





                    //for (int i = 0; i < invoiceDetail.Invoice.InvoiceDetailItems.Count(); i++)
                    //{
                    //    var item = invoiceDetail.Invoice.InvoiceDetailItems[i];

                    //    bool foundFields = true;
                    //    decimal itemPaymentAmt = 0;
                    //    decimal itemTotal = 0;

                    //    // sanity check to see if payment details were set, whole invoice was paid, or there is only one line item
                    //    if (oManualInvoice.PaidInFull || invoiceDetail.Invoice.InvoiceDetailItems.Count == 1)
                    //    {
                    //        //var taxRate = item.TAXAmount / item.Total;
                    //        itemPaymentAmt = applyAmountforPartialPay;
                    //        //var taxAmount = (decimal)(applyAmountforPartialPay * taxRate);
                    //        //itemTotal = applyAmountforPartialPay - taxAmount;

                    //        if (item.Balance > 0)
                    //        {
                    //            if (item.Balance < applyAmountforPartialPay)
                    //            {
                    //                invPayment = (decimal)item.Balance;
                    //                applyAmountforPartialPay = applyAmountforPartialPay - (decimal)item.Balance;
                    //            }
                    //            else
                    //            {
                    //                invPayment = applyAmountforPartialPay;
                    //                applyAmountforPartialPay = 0;
                    //            }
                    //            var taxRate = item.TAXAmount / item.Total;
                    //            itemPaymentAmt = invPayment;
                    //            var taxAmount = (decimal)(invPayment * taxRate);
                    //            itemTotal = invPayment - taxAmount;
                    //        }

                    //    }
                    //    else if (!oManualInvoice.PaidInFull && invoiceDetail.FranchiseeItems.Count() == 1)
                    //    {
                    //        if (item.Balance > 0)
                    //        {
                    //            if (item.Balance < applyAmountforPartialPay)
                    //            {
                    //                invPayment = (decimal)item.Balance;
                    //                applyAmountforPartialPay = applyAmountforPartialPay - (decimal)item.Balance;
                    //            }
                    //            else
                    //            {
                    //                invPayment = applyAmountforPartialPay;
                    //                applyAmountforPartialPay = 0;
                    //            }
                    //            var taxRate = item.TAXAmount / item.Total;
                    //            itemPaymentAmt = invPayment;
                    //            var taxAmount = (decimal)(invPayment * taxRate);
                    //            itemTotal = invPayment - taxAmount;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        foundFields = foundFields &&
                    //                      decimal.TryParse(frm[string.Format(invChunk + "_item{0}_paymentAmt", i)],
                    //                          out itemPaymentAmt);
                    //        foundFields = foundFields &&
                    //                      decimal.TryParse(frm[string.Format(invChunk + "_item{0}_total", i)],
                    //                          out itemTotal);
                    //    }

                    //    if (foundFields)
                    //    //if (foundFields && itemTotal > 0)
                    //    {
                    //        ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
                    //        mpvm.MasterTrxDetailId = item.MasterTrxDetailId;
                    //        mpvm.LineNo = (int)item.LineNumber;
                    //        mpvm.PaymentAmount = itemPaymentAmt;
                    //        mpvm.Tax = itemPaymentAmt - itemTotal;
                    //        mpvm.Total = itemTotal;
                    //        mpvm.ExtendedPrice = itemPaymentAmt - (itemPaymentAmt - itemTotal);
                    //        mpcvm.Payments.Add(mpvm);
                    //    }
                    //}

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
                            bpPaymentAmt = (decimal)r.Balance + (decimal)r.BalanceFees; // pay whole balance because whole invoice was paid
                        }
                        else if (invoiceDetail.Invoice.InvoiceDetailItems.Count == 1 || invoiceDetail.FranchiseeItems.Count == 1)
                        {
                            // only one line item, so distribute paid amount (after taxes) to all franchisees proportionally
                            var totalFranchiseeBalance =
                                invoiceDetail.FranchiseeItems.Sum(o => o.InvoiceFranchiseeDetailItem.Balance);
                            var percentage = r.Balance / totalFranchiseeBalance;
                            bpPaymentAmt = oManualInvoice.CustomerPayment.Payments[0].ExtendedPrice * (decimal)percentage;
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
                //if (oMainObject.Invoices.Count > 0) // sanity check
                //    AccountReceivableService.InsertManualPaymentTransactionUpdated(oMainObject);
                if (oMainObject.Invoices.Count > 0) // sanity check
                    AccountReceivableService.InsertManualPaymentTransactionInTemp(oMainObject);


            }

            if (afterSave == "SaveNew")
            {
                return RedirectToAction("ManualPayment", "AccountReceivable", new { area = "Portal" });
            }
            else if (afterSave == "SaveClose")
            {
                return RedirectToAction("PaymentList", "AccountReceivable", new { area = "Portal" });
            }

            return View();
        }

        #endregion :: Payments ::

        #region :: Credits ::

        [HttpGet]
        public ActionResult ApplyCreditFormInv(int Id)
        {
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                "Name");

            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForInvoiceN(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");

            if (model.Invoice.InvoiceDetailItems.Count() > 1 && model.FranchiseeItems.Count() == 1)
            {
                //var aaa = new SelectList(new List<SelectListItem>{new SelectListItem() { Selected = true, Text = "All", Value = "-1" }}, "Value" , "Text", -1);

                ViewBag.LineNoList = new SelectList(new List<SelectListItem> { new SelectListItem() { Selected = true, Text = "All", Value = "-1" } }, "Value", "Text", -1); ;
            }


            ViewBag.BillMonthList = GetMonthsList(model.Invoice.InvoiceDetail.BillMonth ?? 0);
            ViewBag.BillYearList = GetYearsList(model.Invoice.InvoiceDetail.BillYear ?? 0);


            ViewBag.IsPendingCredit = false;
            return PartialView("_PartialApplyCreditFormInvoice", model);
        }


        [HttpGet]
        public ActionResult ApplyCreditTaxForm(int Id)
        {
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(true), "CreditReasonListId",
                "Name", 16);

            CreditDetailViewModel model = AccountReceivableService.GetTaxCreditDetailForInvoice(Id);
            ViewBag.InvId = Id;
            return PartialView("_PartialApplyTaxCreditForm", model);
        }

        [HttpGet]
        public bool ApplyCreditTaxFormPost(int Id, decimal cramt, string trxdate, string trxdesc, int reasonListId)
        {
            //AccountReceivableService.InsertTaxCreditDetailForInvoiceTransaction(Id, cramt, DateTime.Parse(trxdate), trxdesc);
            var creditDate = !String.IsNullOrEmpty(trxdate) ? DateTime.Parse(trxdate) : DateTime.Now;
            AccountReceivableService.InsertOrUpdateCreditTaxTransactionInTemp(Id, cramt, creditDate, trxdesc, reasonListId);
            return true;
        }


        [HttpGet]
        public ActionResult ApplyCreditForm(int Id)
        {
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                "Name");

            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForInvoiceN(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");

            if (model.Invoice.InvoiceDetailItems.Count() > 1 && model.FranchiseeItems.Count() == 1)
            {
                //var aaa = new SelectList(new List<SelectListItem>{new SelectListItem() { Selected = true, Text = "All", Value = "-1" }}, "Value" , "Text", -1);

                ViewBag.LineNoList = new SelectList(new List<SelectListItem> { new SelectListItem() { Selected = true, Text = "All", Value = "-1" } }, "Value", "Text", -1); ;
            }


            ViewBag.BillMonthList = GetMonthsList(model.Invoice.InvoiceDetail.BillMonth ?? 0);
            ViewBag.BillYearList = GetYearsList(model.Invoice.InvoiceDetail.BillYear ?? 0);
            ViewBag.IsPendingCredit = false;
            return PartialView("_PartialApplyCreditForm", model);
        }

        [HttpGet]
        public ActionResult ApplyCreditFormForCancellation(int Id, DateTime lastServiceDate)
        {
            ViewBag.IsForCancellation = true;
            ViewBag.LastServiceDate = lastServiceDate;

            ViewBag.IsLastOne = false;



            return ApplyCreditForm(Id);
        }

        [HttpGet]
        public JsonResult ApplyCreditFormForCancellationWorkingDays(int Id, DateTime lastServiceDate)
        {
            //ViewBag.IsForCancellation = true;
            //ViewBag.LastServiceDate = lastServiceDate;

            //ViewBag.IsLastOne = false;
            Invoice OInvoice = CustomerService.GetInvoiceDetailForCN(Id);
            //JKApi.Service.Service.Customer.RevenueDistributionInvoiceDetailViewModel oRevenueDistributionInvoiceDetailViewModel = CustomerService.GetRevenueDistributionDetail(Id);
            DateTime INVDate = (DateTime)OInvoice.InvoiceDate;
            DateTime lastOfThisMonth = (new DateTime(INVDate.Year, INVDate.Month, 1).AddMonths(1)).AddDays(-1);
            List<CalanderDatesModel> lstWorkingDays = CustomerService.GetCalanderDates(INVDate, lastOfThisMonth);

            JKApi.Service.Service.Customer.CustomerFranchiseeDistributionViewModel OData = CustomerService.GetCustomerFranchiseeDistributionData((int)OInvoice.ClassId);
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

            List<CalanderDatesModel> lstEffectiveWorkingDays = CustomerService.GetCalanderDates(INVDate, lastServiceDate).ToList();
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

            //List<CalanderDatesModel>  lstWorkingDays = CustomerService.GetCalanderDates((DateTime)oRevenueDistributionInvoiceDetailViewModel.InvoiceDetail.InvoiceDate, (DateTime)EffectiveDate);

            //List<CalanderDatesModel> lstWorkingDays = CustomerService.GetCalanderDates((DateTime)oRevenueDistributionInvoiceDetailViewModel.InvoiceDetail.InvoiceDate, (DateTime)EffectiveDate);


            int defDays = wAllDay - wDeffDay;

            if (INVDate > lastServiceDate)
                defDays = wAllDay;//+ 1;

            int tMonthDays = wAllDay;

            ViewBag.defDays = defDays;
            ViewBag.MonthDays = wAllDay;


            return Json(new { monthDays = wAllDay, workingDays = defDays }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateCreditForm(int Id)
        {
            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForCreditN(Id);
            //CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForCreditN(Id);

            //CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForCredit(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");
            if (model.Invoice.InvoiceDetailItems.Count() > 1 && model.FranchiseeItems.Count() == 1)
            {
                ViewBag.LineNoList = new SelectList(new List<SelectListItem> { new SelectListItem() { Selected = true, Text = "All", Value = "-1" } }, "Value", "Text", -1); ;
            }
            ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                "Name", model.Credit.CreditReasonListId);
            ViewBag.BillMonthList = GetMonthsList(model.CreditBillMonth);
            ViewBag.BillYearList = GetYearsList(model.CreditBillYear);
            ViewBag.IsPendingCredit = false;
            return PartialView("_PartialApplyCreditForm", model);
        }

        [HttpGet]
        public ActionResult UpdateCreditFormPending(int Id)
        {
            CreditTemp creditTemp = null;
            CreditDetailViewModel model = null;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                creditTemp = context.CreditTemps.SingleOrDefault(c => c.CreditTempId == Id);
            }
            if (creditTemp.MasterTrxTypeListId == 58)
            {
                model = AccountReceivableService.GetCreditDetailForTaxCreditTempN(Id);
                ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(true), "CreditReasonListId",
                   "Name", creditTemp.CreditReasonListId);
            }
            else
            {
                model = AccountReceivableService.GetCreditDetailForCreditTempN(Id);
                ViewBag.ReasonList = new SelectList(AccountReceivableService.GetAll_CreditReasonList(false), "CreditReasonListId",
                    "Name", model.Credit.CreditReasonListId);
            }

            ViewBag.BillMonthList = GetMonthsList(model.CreditBillMonth);
            ViewBag.BillYearList = GetYearsList(model.CreditBillYear);
            ViewBag.IsPendingCredit = true;
            if (creditTemp.MasterTrxTypeListId == 58)
            {
                return PartialView("_PartialApplyTaxCreditForm", model);
            }
            else
            {
                ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");
                if (model.Invoice.InvoiceDetailItems.Count() > 1 && model.FranchiseeItems.Count() == 1)
                {
                    ViewBag.LineNoList = new SelectList(new List<SelectListItem> { new SelectListItem() { Selected = true, Text = "All", Value = "-1" } }, "Value", "Text", -1); ;
                }
                return PartialView("_PartialApplyCreditForm", model);
            }
        }

        [HttpGet]
        public ActionResult PendingCreditDelete(int Id)
        {
            AccountReceivableService.DeleteCredit(Id);
            return RedirectToAction("PendingCreditList", "AccountReceivable", new { area = "Portal" });
        }

        [HttpGet]
        public ActionResult PendingCreditTempDelete(int Id)
        {
            AccountReceivableService.DeleteCreditTemp(Id);
            return RedirectToAction("PendingCreditList", "AccountReceivable", new { area = "Portal" });
        }

        private CreditTransactionViewModel _GetCreditTransactionViewModelFromForm(FormCollection frm)
        {
            int intRes;
            decimal decRes;

            var updateCreditId = frm["updateCreditId"] != null ? int.Parse(frm["updateCreditId"]) : -1;

            var invoiceId = frm["invoiceId"] != null ? int.Parse(frm["invoiceId"]) : 0;
            var creditReasonListId = int.TryParse(frm["slReasonList"], out intRes) ? intRes : 0;
            var creditDesc = frm["creditDesc"];



            var creditDate = frm["creditDate"] != null ? DateTime.Parse(frm["creditDate"]) : DateTime.Now;
            var billMonth = creditDate.Month;
            var billYear = creditDate.Year;

            var totalCreditAmt = decimal.TryParse(frm["creditAmt"], out decRes) ? decRes : 0;
            var newBalance = decimal.TryParse(frm["newBalance"], out decRes) ? decRes : 0;

            decimal applyCreditAmount = 0;
            var applyCreditAmt = decimal.TryParse(frm["requestCreditAmt"], out applyCreditAmount) ? applyCreditAmount : 0;

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
                                  decimal.TryParse(frm[string.Format("item{0}_oldBalance", i)], out oldBalance);
                    foundFields = foundFields &&
                                  decimal.TryParse(frm[string.Format("item{0}_creditAmt", i)], out creditAmt);
                    foundFields = foundFields && decimal.TryParse(frm[string.Format("item{0}_total", i)], out total);
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
                if (decimal.TryParse(frm[$"bp{bpId}_creditAmt"], out res))
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

        private List<CreditTransactionViewModel> _GetAdjustmentTransactionViewModelFromForm(FormCollection frm)
        {
            List<CreditTransactionViewModel> lstCreditTransactionViewModel = new List<CreditTransactionViewModel>();
            List<int> lstInvocieIds = new List<int>();
            foreach (string _str in frm.AllKeys)
            {
                if (_str.StartsWith("inv") && _str.EndsWith("_chk"))
                {
                    string _invId = _str.Replace("inv", "").Replace("_chk", "");
                    string _ss = _invId;

                    lstInvocieIds.Add(int.Parse(_invId));
                }
            }


            foreach (int _InvID in lstInvocieIds)
            {

                int intRes;
                decimal decRes;

                var invoiceId = _InvID;
                var creditDate = DateTime.Now;
                var billMonth = creditDate.Month;
                var billYear = creditDate.Year;

                var updateCreditId = frm["updateCreditId"] != null ? int.Parse(frm["updateCreditId"]) : -1;
                var creditReasonListId = int.TryParse(frm["slReasonList"], out intRes) ? intRes : 0;
                var creditDesc = frm["creditDesc"];
                var Note = frm["txtNotes"];

                var totalCreditAmt = decimal.TryParse(frm["inv" + _InvID + "_totalPayment"], out decRes) ? decRes : 0;
                var newBalance = decimal.TryParse(frm["inv" + _InvID + "_balance"], out decRes) ? decRes : 0;

                var invoiceDetails = AccountReceivableService.GetCreditDetailForInvoice(invoiceId);

                var vm = new CreditTransactionViewModel
                {
                    Id = updateCreditId,
                    RegionId = (int)invoiceDetails.Invoice.InvoiceDetail.RegionId,
                    InvoiceId = invoiceId,
                    CreditReasonListId = creditReasonListId,
                    CreditDescription = creditDesc,
                    TotalCredit = totalCreditAmt,
                    IsExtraCredit = newBalance < 0,
                    PaidInFull = invoiceDetails.InvoiceBalance <= totalCreditAmt && newBalance <= 0,
                    BillMonth = billMonth,
                    BillYear = billYear,
                    CreatedBy = LoginUserId,
                    CreatedDate = creditDate
                };

                var ccvm = new CustomerCreditViewModel
                {
                    CustomerId = (int)invoiceDetails.Invoice.InvoiceDetail.CustomerId,
                    Credits = new List<CreditViewModel>()
                };

                var fcvms = new List<FranchiseeCreditViewModel>();

                decimal applyitemAmount = totalCreditAmt;
                decimal applyitembpAmount = 0;
                for (var i = 0; i < invoiceDetails.Invoice.InvoiceDetailItems.Count(); i++)
                {
                    var foundFields = true;
                    decimal oldBalance = (decimal)invoiceDetails.Invoice.InvoiceDetailItems[i].Balance;//0;
                    decimal creditAmt = 0;
                    decimal total = 0;
                    if (oldBalance >= applyitemAmount)
                    {
                        creditAmt = applyitemAmount;
                    }
                    else
                    {
                        creditAmt = oldBalance;
                        applyitemAmount -= oldBalance;
                    }




                    decimal _ExtendedPrice = (decimal)invoiceDetails.Invoice.InvoiceDetailItems[i].ExtendedPrice;
                    decimal _TAXAmount = (decimal)invoiceDetails.Invoice.InvoiceDetailItems[i].TAXAmount;
                    decimal _Total = (decimal)invoiceDetails.Invoice.InvoiceDetailItems[i].Total;


                    var _per = (_TAXAmount / _ExtendedPrice) * 100;

                    total = ((100 * creditAmt) / (100 + _per));

                    // only supply values if we are a normal credit
                    if (!vm.IsExtraCredit)
                    {
                        //    foundFields = foundFields &&
                        //                  decimal.TryParse(frm[string.Format("item{0}_oldBalance", i)], out oldBalance);
                        //    foundFields = foundFields &&
                        //                  decimal.TryParse(frm[string.Format("item{0}_creditAmt", i)], out creditAmt);
                        //    foundFields = foundFields && decimal.TryParse(frm[string.Format("item{0}_total", i)], out total);
                        //}

                        //if (foundFields)
                        //{
                        // sanity checks if local validation failed
                        //creditAmt = Math.Min(oldBalance, creditAmt);
                        //total = Math.Min(oldBalance, total);

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


                    applyitembpAmount = total;
                    foreach (var item in invoiceDetails.FranchiseeItems)
                    {
                        var r = item.InvoiceFranchiseeDetailItem;
                        if (invoiceDetails.Invoice.InvoiceDetailItems[i].LineNumber == (int)r.LineNo)
                        {

                            var bpId = r.BillingPayId;
                            decimal res = applyitembpAmount;


                            if (res > 0)
                            {
                                var fcvm = new FranchiseeCreditViewModel
                                {
                                    FranchiseeId = r.FranchiseeId,
                                    BillingPayId = r.BillingPayId
                                };

                                var cvmf = new CreditViewModel
                                {
                                    BaseMasterTrxDetailId = r.MasterTrxDetailId,
                                    LineNo = (int)r.LineNo
                                };

                                var customerCvm = ccvm.Credits.FirstOrDefault(o => o.LineNo == cvmf.LineNo);
                                if (customerCvm != null)
                                {
                                    cvmf.ServiceTypeListId = customerCvm.ServiceTypeListId;
                                }

                                cvmf.CreditAmount = res;
                                fcvm.Credit = cvmf;

                                fcvms.Add(fcvm);
                            }

                        }
                    }
                }

                vm.CustomerCredit = ccvm;
                vm.FranchiseeCredits = fcvms;
                vm.Note = Note;
                lstCreditTransactionViewModel.Add(vm);
            }

            return lstCreditTransactionViewModel;
        }

        private List<CreditTransactionViewModel> _GetAdjustmentTransactionViewModelFromRefund(FormCollection frm)
        {
            List<CreditTransactionViewModel> lstCreditTransactionViewModel = new List<CreditTransactionViewModel>();
            List<int> lstInvocieIds = new List<int>();
            foreach (string _str in frm.AllKeys)
            {
                if (_str.StartsWith("inv") && _str.EndsWith("_chk"))
                {
                    string _invId = _str.Replace("inv", "").Replace("_chk", "");
                    string _ss = _invId;

                    lstInvocieIds.Add(int.Parse(_invId));
                }
            }


            foreach (int _InvID in lstInvocieIds)
            {

                int intRes;
                decimal decRes;

                var invoiceId = _InvID;
                var creditDate = DateTime.Now;
                var billMonth = creditDate.Month;
                var billYear = creditDate.Year;

                var updateCreditId = frm["updateCreditId"] != null ? int.Parse(frm["updateCreditId"]) : -1;
                var creditReasonListId = int.TryParse(frm["slReasonList"], out intRes) ? intRes : 0;
                var creditDesc = frm["creditDesc"];


                var totalCreditAmt = Math.Abs(decimal.TryParse(frm["inv" + _InvID + "_totalPayment"], out decRes) ? decRes : 0);
                var newBalance = Math.Abs(decimal.TryParse(frm["inv" + _InvID + "_balance"], out decRes) ? decRes : 0);

                var invoiceDetails = AccountReceivableService.GetCreditDetailForInvoice(invoiceId);

                var vm = new CreditTransactionViewModel
                {
                    Id = updateCreditId,
                    RegionId = (int)invoiceDetails.Invoice.InvoiceDetail.RegionId,
                    InvoiceId = invoiceId,
                    CreditReasonListId = creditReasonListId,
                    CreditDescription = creditDesc,
                    TotalCredit = totalCreditAmt,
                    IsExtraCredit = newBalance < 0,
                    PaidInFull = invoiceDetails.InvoiceBalance <= totalCreditAmt && newBalance <= 0,
                    BillMonth = billMonth,
                    BillYear = billYear,
                    CreatedBy = LoginUserId,
                    CreatedDate = creditDate
                };

                var ccvm = new CustomerCreditViewModel
                {
                    CustomerId = (int)invoiceDetails.Invoice.InvoiceDetail.CustomerId,
                    Credits = new List<CreditViewModel>()
                };

                var fcvms = new List<FranchiseeCreditViewModel>();

                decimal applyitemAmount = totalCreditAmt;
                decimal applyitembpAmount = 0;


                for (var i = 0; i < invoiceDetails.Invoice.InvoiceDetailItems.Count(); i++)
                {
                    var foundFields = true;
                    decimal oldBalance = (decimal)invoiceDetails.Invoice.InvoiceDetailItems[i].Balance;//0;
                    decimal creditAmt = 0;
                    decimal total = 0;
                    creditAmt = applyitemAmount;
                    //applyitemAmount -= oldBalance;
                    //if (oldBalance >= applyitemAmount)
                    //{

                    //}
                    //else
                    //{
                    //    creditAmt = oldBalance;
                    //    applyitemAmount -= oldBalance;
                    //}




                    decimal _ExtendedPrice = applyitemAmount;
                    decimal _TAXAmount = 0;
                    decimal _Total = applyitemAmount;


                    var _per = (_TAXAmount / _ExtendedPrice) * 100;

                    total = ((100 * creditAmt) / (100 + _per));

                    //// only supply values if we are a normal credit
                    //if (!vm.IsExtraCredit)
                    //{
                    //    //    foundFields = foundFields &&
                    //    //                  decimal.TryParse(frm[string.Format("item{0}_oldBalance", i)], out oldBalance);
                    //    //    foundFields = foundFields &&
                    //    //                  decimal.TryParse(frm[string.Format("item{0}_creditAmt", i)], out creditAmt);
                    //    //    foundFields = foundFields && decimal.TryParse(frm[string.Format("item{0}_total", i)], out total);
                    //    //}

                    //    //if (foundFields)
                    //    //{
                    //    // sanity checks if local validation failed
                    //    //creditAmt = Math.Min(oldBalance, creditAmt);
                    //    //total = Math.Min(oldBalance, total);

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
                    //}
                    //applyitembpAmount = total;
                    //foreach (var item in invoiceDetails.FranchiseeItems)
                    //{
                    //    var r = item.InvoiceFranchiseeDetailItem;
                    //    if (invoiceDetails.Invoice.InvoiceDetailItems[i].LineNumber == (int)r.LineNo)
                    //    {

                    //        var bpId = r.BillingPayId;
                    //        decimal res = applyitembpAmount;


                    //        if (res > 0)
                    //        {
                    //            var fcvm = new FranchiseeCreditViewModel
                    //            {
                    //                FranchiseeId = r.FranchiseeId,
                    //                BillingPayId = r.BillingPayId
                    //            };

                    //            var cvmf = new CreditViewModel
                    //            {
                    //                BaseMasterTrxDetailId = r.MasterTrxDetailId,
                    //                LineNo = (int)r.LineNo
                    //            };

                    //            var customerCvm = ccvm.Credits.FirstOrDefault(o => o.LineNo == cvmf.LineNo);
                    //            if (customerCvm != null)
                    //            {
                    //                cvmf.ServiceTypeListId = customerCvm.ServiceTypeListId;
                    //            }

                    //            cvmf.CreditAmount = res;
                    //            fcvm.Credit = cvmf;

                    //            fcvms.Add(fcvm);
                    //        }

                    //    }
                    //}
                }

                vm.CustomerCredit = ccvm;
                //vm.FranchiseeCredits = fcvms;

                lstCreditTransactionViewModel.Add(vm);
            }

            return lstCreditTransactionViewModel;
        }

        [HttpPost]
        public JsonResult ApplyCredit(FormCollection frm)
        {
            var action = frm["action"];
            var isForCancellation = (!String.IsNullOrEmpty(frm["isForCancellation"]) ? bool.Parse(frm["isForCancellation"]) : false);
            int CancellationMaintenancetempId = (!String.IsNullOrEmpty(frm["isForCancellationMaintenancetempId"]) ? int.Parse(frm["isForCancellationMaintenancetempId"]) : 0);
            var vm = _GetCreditTransactionViewModelFromForm(frm);

            if (isForCancellation)
            {
                AccountReceivableService.InsertUpdateCustomerCreditTransactionMaintenanceTemp(vm, CancellationMaintenancetempId);
            }
            else
            {
                //AccountReceivableService.InsertOrUpdateCreditTransaction(vm);

                AccountReceivableService.InsertOrUpdateCreditTransactionInTemp(vm);


            }
            var retPage = action == "SaveClose" ? "CreditList" : "CustomerCredits";
            var retPath = Url.Action(retPage, "AccountReceivable", new { area = "Portal" });

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCredit(FormCollection frm)
        {
            var action = frm["action"];
            var vm = _GetCreditTransactionViewModelFromForm(frm);
            AccountReceivableService.InsertOrUpdateCreditTransaction(vm);

            if (action == "SaveReject")
            {
                string txtCraditStatusNote = !String.IsNullOrEmpty(frm["txtCraditStatusNote"].ToString())
                    ? frm["txtCraditStatusNote"].ToString()
                    : "";
                AccountReceivableService.ApproveCredit(vm.Id, 12, txtCraditStatusNote);
            }
            else if (action == "SaveApprove")
            {
                string txtCraditStatusNote = !String.IsNullOrEmpty(frm["txtCraditStatusNote"].ToString())
                    ? frm["txtCraditStatusNote"].ToString()
                    : "";
                AccountReceivableService.ApproveCredit(vm.Id, 3, txtCraditStatusNote);
            }

            string retPath = Url.Action("PendingCreditList", "AccountReceivable", new { area = "Portal" });

            return Json(retPath, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApproveCredits(FormCollection frm)
        {
            string ids = frm["creditIds"];

            string[] arrids = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string a in arrids)
            {
                string txtCraditStatusNote = !String.IsNullOrEmpty(frm["txtCraditStatusNote"].ToString())
                    ? frm["txtCraditStatusNote"].ToString()
                    : "";
                AccountReceivableService.ApproveCredit(int.Parse(a.Trim()), 3, txtCraditStatusNote);
            }

            return RedirectToAction("PendingCreditList", "AccountReceivable", new { area = "Portal" });
        }

        [HttpGet]
        public ActionResult ApproveRejectCredits(int creditId, string creditStatusNote, bool isApprove)
        {

            if (isApprove)
                AccountReceivableService.ApproveCredit(creditId, 3, creditStatusNote);
            else
                AccountReceivableService.ApproveCredit(creditId, 12, creditStatusNote);



            return RedirectToAction("PendingCreditList", "AccountReceivable", new { area = "Portal" });
        }


        [HttpGet]
        public ActionResult ApproveRejectTempCredits(int creditId, string creditStatusNote, bool isApprove)
        {
            if (isApprove)
                AccountReceivableService.ApproveTempCredit(creditId, 3, creditStatusNote);
            else
                AccountReceivableService.ApproveTempCredit(creditId, 12, creditStatusNote);
            return RedirectToAction("PendingCreditList", "AccountReceivable", new { area = "Portal" });
        }




        #endregion :: Credits ::

        #endregion Transactions

        #region :: Invoice  Export To PDF ::

        //Invoice Export To PDF File
        public FileResult InvoiceInfoExportToPDF(int Id)
        {
            InvoiceDetailViewModel objInvoiceDetailViewModel = new InvoiceDetailViewModel();
            objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(Id);
            string HTMLContent = RenderPartialViewToString("_InvoiceInfoExportToPDF", objInvoiceDetailViewModel);
            //Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.End();
            return File(GetPDF(HTMLContent), "application/pdf", Id + "_InvoiceExportToPDF.pdf");
        }

        // InvoiceList Export to PDF Files
        public FileResult InvoiceListExportPDF(string InvoiceIds)
        {
            string[] Ids = InvoiceIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    InvoiceDetailViewModel objInvoiceDetailViewModel = new InvoiceDetailViewModel();
                    objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(Convert.ToInt32(item));
                    HTMLContent += RenderPartialViewToString("_InvoiceInfoExportToPDF", objInvoiceDetailViewModel);
                }
                return File(GetPDF(HTMLContent), "application/pdf", "_InvoiceListExportToPDF.pdf");
            }
            return null;

            //InvoiceDetailViewModel objInvoiceDetailViewModel = new InvoiceDetailViewModel();
            //objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(0);
            //string HTMLContent = RenderPartialViewToString("_InvoiceInfoExportToPDF", objInvoiceDetailViewModel);
            //string HTMLContent = "Wel come to test" + InvoiceIds;
            //return File(GetPDF(HTMLContent), "application/pdf","InvoiceListExportPDF.pdf");

            //using (MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    StringReader sr = new StringReader(GridHtml);
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            //    pdfDoc.Open();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //    pdfDoc.Close();
            //    return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            //}
        }

        [HttpGet]
        public JsonResult InvoiceListSendEmail(string InvoiceIds)
        {
            try
            {
                var ids = InvoiceIds.Split(',');
                if (ids.Any())
                {
                    foreach (var item in ids)
                    {
                        var objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(Convert.ToInt32(item));
                        var htmlContentTemp = RenderPartialViewToString("_InvoiceInfoExportToPDF",
                            objInvoiceDetailViewModel);
                        var emails =
                            AccountReceivableService.GetCustomerEbillEmails(
                                (int)objInvoiceDetailViewModel.InvoiceDetail.CustomerId);
                        foreach (var email in emails)
                        {
                            if (string.IsNullOrEmpty(email.EmailAddress)) continue;
                            var subject = "Email of " + objInvoiceDetailViewModel.InvoiceDetail.InvoiceNo;
                            var body = string.Empty;
                            body += "<div><table><tr><td colspan='3'>" +
                                    objInvoiceDetailViewModel.InvoiceDetail.Customer + "</td></tr>";
                            body += "<tr><td colspan='3'><br />" + objInvoiceDetailViewModel.InvoiceDetail.InvoiceNo +
                                    "<br /></td></tr>";
                            body += "<tr><td colspan='3'><span>From,</span><br />JaniKing</td></tr></table></div>";
                            var attachments = new List<Attachment>();
                            attachments.Add(new Attachment(new MemoryStream(GetPDF(htmlContentTemp)),
                                "INV- " + objInvoiceDetailViewModel.InvoiceDetail.InvoiceNo + ".pdf"));
                            //_mailService.SendEmailAsync(email.EmailAddress, body, subject, attachments);



                            #region Email send function According to Admin configuration

                            //Get Feature Type Id by Feature Name
                            var Invoice_Ebill = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.Invoice_Ebill.ToString().Replace("_", " ")).FirstOrDefault();

                            if (Invoice_Ebill != null && Invoice_Ebill.FeatureTypeId > 0)
                            {
                                //Get Feature Type Email Id by Feature Type Id
                                var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == Invoice_Ebill.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                                if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                                {
                                    _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, body, subject, attachments);

                                    if (messageDetails.EmailToCustomer == true && !string.IsNullOrWhiteSpace(email.EmailAddress))
                                    {
                                        _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, email.EmailAddress, body, subject, attachments);
                                    }

                                }
                            }
                            #endregion
                        }
                    }
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                //throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetEmailsforSendEmail(string CustomerId)
        {
            UserViewModel oUserViewModel = _userService.GetUserDetail(LoginUserId);

            List<EmailViewModel> lstEmailViewModel =
                AccountReceivableService.GetCustomerEbillEmails(int.Parse(CustomerId));

            string eMail = "";
            foreach (EmailViewModel eEmail in lstEmailViewModel)
            {
                eMail += eEmail.EmailAddress + ";";
            }

            return Json("{\"FromEmail\":\"" + oUserViewModel.Email + "\",\"CCEmail\":\"" + eMail + "\"}",
                JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult InvoiceListSendEmailPopup(string InvoiceId, string FromEmail, string ToEmail, string CCEmail,
            string SubjectEmail, string BodyEmail)
        {
            string retVal = "";
            try
            {
                string _InvoiceId = InvoiceId;

                string HTMLContent = string.Empty;

                InvoiceDetailViewModel objInvoiceDetailViewModel = new InvoiceDetailViewModel();
                objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(Convert.ToInt32(_InvoiceId));
                HTMLContent = RenderPartialViewToString("_InvoiceInfoExportToPDF", objInvoiceDetailViewModel);

                //System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                //mail.From = new MailAddress(FromEmail);

                //mail.To.Add(ToEmail);
                //if (CCEmail != "")
                //    mail.CC.Add(CCEmail);
                //mail.Subject = SubjectEmail;

                BodyEmail +=
                    "<p>To open the attached pdf, You need the free Adobe Reader software whitch can be here <a href='#'>Adobe</a></p>";
                BodyEmail += "<hr />";
                BodyEmail +=
                    "<p> This email is intended for the party listed in the 'Sold To' field of the attached invoice. Delivery of this email to anyone other than the party to which is was intended is unintentional. In the event this email was misdirected to a party other that the intended party, please notify the sender destroy this email.</p>";

                //mail.Body = BodyEmail;
                //mail.IsBodyHtml = true;

                var attachments = new List<Attachment>();

                attachments.Add(new Attachment(new MemoryStream(GetPDF(HTMLContent)),
                    "INV- " + objInvoiceDetailViewModel.InvoiceDetail.InvoiceNo + ".pdf"));
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //SmtpServer.Port = 587;
                //SmtpServer.UseDefaultCredentials = false;
                //SmtpServer.Credentials = new System.Net.NetworkCredential("janikingtest@gmail.com", "Test#12345");
                //SmtpServer.EnableSsl = true;
                //SmtpServer.Send(mail);

                #region Email send function According to Admin configuration

                //Get Feature Type Id by Feature Name
                var Invoice_Ebill = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.Invoice_Ebill.ToString().Replace("_", " ")).FirstOrDefault();

                if (Invoice_Ebill != null && Invoice_Ebill.FeatureTypeId > 0)
                {
                    //Get Feature Type Email Id by Feature Type Id
                    var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == Invoice_Ebill.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                    if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                    {
                        _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, BodyEmail, SubjectEmail, attachments);

                        if (messageDetails.EmailToCustomer == true)
                        {
                            _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, ToEmail, BodyEmail, SubjectEmail, attachments);
                        }

                    }
                }
                #endregion

                retVal = "Success";
            }
            catch (Exception ex)
            {
                retVal = ex.Message;

                //throw ex;
            }
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceListPrint(string InvoiceIds)
        {
            string[] Ids = InvoiceIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    if (item.Trim() != "on")
                    {
                        InvoiceDetailViewModel objInvoiceDetailViewModel = new InvoiceDetailViewModel();
                        objInvoiceDetailViewModel = AccountReceivableService.GetInvoiceDetail(Convert.ToInt32(item));
                        HTMLContent += RenderPartialViewToString("_InvoiceInfoExportToPDF", objInvoiceDetailViewModel);
                    }
                }
                var _crData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var retPath = "/Upload/InvoiceFiles/" + _crData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), _crData + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult ConsolidatedInvoiceListPrint(string ConsolidatedInvoiceIds)
        {
            string[] Ids = ConsolidatedInvoiceIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    if (item.Trim() != "on")
                    {
                        ConsolidatedInvoiceDetailViewModel objInvoiceDetailViewModel = new ConsolidatedInvoiceDetailViewModel();
                        objInvoiceDetailViewModel = AccountReceivableService.GetConsolidatedInvoiceDetail(Convert.ToInt32(item));
                        HTMLContent += RenderPartialViewToString("_ConsolidatedInvoiceInfoExportToPDF", objInvoiceDetailViewModel);
                    }
                }
                var _crData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var retPath = "/Upload/InvoiceFiles/" + _crData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), _crData + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult GetConsolidatedInvoiceDetailPrint(int consolidatedInvoiceid)
        {
            string HTMLContent = string.Empty;
            ConsolidatedInvoiceDetailViewModel objInvoiceDetailViewModel = new ConsolidatedInvoiceDetailViewModel();
            objInvoiceDetailViewModel = AccountReceivableService.GetConsolidatedInvoiceDetail(Convert.ToInt32(consolidatedInvoiceid));
            HTMLContent += RenderPartialViewToString("_ConsolidateInvoiceDetailsPrint", objInvoiceDetailViewModel);

            var lseData = "coninv_" + DateTime.Now.ToString("MMddyyyyHHmmsstt");
            var lsePath = "/Upload/InvoiceFiles/" + lseData + ".pdf";
            var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), lseData + ".pdf");
            System.IO.File.WriteAllBytes(path, GetPDFWithoutRotate(HTMLContent));
            return Json(lsePath, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public FileResult GetConsolidatedInvoiceDetailExportToPDF(int consolidatedInvoiceid)
        {
            if (consolidatedInvoiceid > 0)
            {
                string HTMLContent = string.Empty;
                ConsolidatedInvoiceDetailViewModel objInvoiceDetailViewModel = new ConsolidatedInvoiceDetailViewModel();
                objInvoiceDetailViewModel = AccountReceivableService.GetConsolidatedInvoiceDetail(Convert.ToInt32(consolidatedInvoiceid));
                HTMLContent += RenderPartialViewToString("_ConsolidateInvoiceDetailsPrint", objInvoiceDetailViewModel);
                return File(GetPDFWithoutRotate(HTMLContent), "application/pdf", "_ConsolidatedInvoiceDetailExportToPDF.pdf");
            }
            return null;
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

        // CreditList Export to PDF Files
        public FileResult CreditListExportPDF(string CreditIds)
        {
            string[] Ids = CreditIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    CreditDetailPrintViewModel objCreditDetailPrintvm = new CreditDetailPrintViewModel();
                    objCreditDetailPrintvm = AccountReceivableService.GetCreditDetailPrint(Convert.ToInt32(item));
                    HTMLContent += RenderPartialViewToString("_CreditInfoExportToPDF", objCreditDetailPrintvm);
                }
                return File(GetPDF(HTMLContent), "application/pdf", "_CreditInfoExportToPDF.pdf");
            }
            return null;
        }
        // PaymentList Export to PDF Files
        public FileResult PaymentListExportPDF(string PaymentIds)
        {
            string[] Ids = PaymentIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    PaymentDetailPrintViewModel objPaymentDetailPrintvm = new PaymentDetailPrintViewModel();
                    objPaymentDetailPrintvm = AccountReceivableService.GetPaymentDetailPrint(Convert.ToInt32(item));
                    HTMLContent += RenderPartialViewToString("_PaymentInfoExportToPDF", objPaymentDetailPrintvm);
                }
                return File(GetPDF(HTMLContent), "application/pdf", "_PaymentInfoExportToPDF.pdf");
            }
            return null;
        }

        public JsonResult CreditListPrint(string CreditIds)
        {
            string[] Ids = CreditIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    if (item.Trim() != "on")
                    {
                        CreditDetailPrintViewModel objCreditDetailPrintvm = new CreditDetailPrintViewModel();
                        objCreditDetailPrintvm = AccountReceivableService.GetCreditDetailPrint(Convert.ToInt32(item));
                        HTMLContent += RenderPartialViewToString("_CreditInfoExportToPDF", objCreditDetailPrintvm);
                    }
                }
                var _crData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var retPath = "/Upload/CreditFiles/" + _crData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/CreditFiles/"), _crData + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult PaymentListPrint(string PaymentIds)
        {
            string[] Ids = PaymentIds.Split(',');
            if (Ids != null && Ids.Count() > 0)
            {
                string HTMLContent = string.Empty;
                foreach (var item in Ids)
                {
                    if (item.Trim() != "on")
                    {
                        PaymentDetailPrintViewModel objPaymentDetailPrintvm = new PaymentDetailPrintViewModel();
                        objPaymentDetailPrintvm = AccountReceivableService.GetPaymentDetailPrint(Convert.ToInt32(item));
                        HTMLContent += RenderPartialViewToString("_PaymentInfoExportToPDF", objPaymentDetailPrintvm);
                    }
                }
                var _crData = DateTime.Now.ToString("MMddyyyyHHmmsstt");
                var retPath = "/Upload/PaymentFiles/" + _crData + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/PaymentFiles/"), _crData + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public byte[] GetPDF(string pHTML)
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
            styles.LoadStyle("col4", "width", "128");
            styles.LoadStyle("col5", "width", "35");
            styles.LoadStyle("col6", "width", "37");

            styles.LoadStyle("t22col1", "width", "35");

            styles.LoadStyle("t2col1", "width", "30");
            styles.LoadStyle("t2col2", "width", "140");
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

        #region Common For PatialToHtml

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

        #endregion Common For PatialToHtml

        #endregion :: Invoice  Export To PDF ::



        //Lockbox
        private List<CommonTransmissionViewModel> ProcessLockboxFile(string[] _input, string filePath, DateTime _LockboxDate)
        {
            string fileMetaData = _input.ToString();
            List<CommonTransmissionViewModel> resultList = new List<CommonTransmissionViewModel>();

            CommonTransmissionViewModel oCommonTransmissionViewModel = new CommonTransmissionViewModel();

            CommonTransmissionViewModel oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

            HttpClient client;

            string tempPriorityCode = "";
            string tempDestination = "";
            string tempOrigin = "";
            string tempYYMMDD = "";
            string tempHHMM = "";
            string tempReferenceCode = "";
            string tempServiceType = "";
            string tempRecordSize = "";
            string tempBlockSize = "";
            string tempFormatCodeUncompressed = "";

            string tempRowDestination = "";
            string tempRowOrigin = "";
            string tempRowYYMMDD = "";

            string tempRegionBankName = "";
            string tempRowBankName = "";
            string tempRowBankState = "";
            string tempLockboxNumber = "", tempBatchNumber = "";


            int tempRowRegionId = 0;
            string tempRowRegionName = "";
            string tempRowRegionAbbr = "";





            for (int i = 0; i < _input.Length; i++)
            {
                string line = _input[i];

                //Record Type '1'
                //Field Field Position Length  Description Fill Contents Fill Char Justifica-tion
                string Recordtype = line.Substring(0, 1);

                switch (Recordtype)
                {
                    case "1":
                        tempPriorityCode = line.Substring(1, 2);
                        tempDestination = line.Substring(3, 10);
                        tempOrigin = line.Substring(13, 10);
                        tempYYMMDD = line.Substring(23, 6);
                        tempHHMM = line.Substring(29, 4);

                        client = new HttpClient();
                        client.BaseAddress = new Uri("https://www.routingnumbers.info/");

                        // Add an Accept header for JSON format.
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage responseM =
                            client.GetAsync("api/data.json?rn=" + long.Parse(tempOrigin)).Result;

                        if (responseM.IsSuccessStatusCode)
                        {
                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                            BankDetailByRouting routes_list =
                                json_serializer.Deserialize<BankDetailByRouting>(
                                    responseM.Content.ReadAsStringAsync().Result);

                            tempRegionBankName = routes_list.customer_name;
                        }

                        break;

                    case "2":
                        tempReferenceCode = line.Substring(21, 10);
                        tempServiceType = line.Substring(31, 3);
                        tempRecordSize = line.Substring(34, 3);
                        tempBlockSize = line.Substring(37, 4);
                        tempFormatCodeUncompressed = line.Substring(41, 1);
                        break;

                    case "5":
                        tempBatchNumber = "";
                        tempLockboxNumber = "";
                        tempRowYYMMDD = "";
                        tempRowDestination = "";
                        tempRowOrigin = "";

                        tempBatchNumber = line.Substring(1, 3);
                        tempLockboxNumber = line.Substring(7, 7);
                        tempRowYYMMDD = line.Substring(14, 6);
                        tempRowDestination = line.Substring(20, 10);
                        tempRowOrigin = line.Substring(30, 10);

                        var oRegion = AccountReceivableService.GetRegionByLockboxNumber(tempLockboxNumber);
                        if (oRegion != null)
                        {
                            tempRowRegionId = oRegion.RegionId;
                            tempRowRegionName = oRegion.Name;
                            tempRowRegionAbbr = oRegion.Acronym;
                        }

                        break;

                    case "6":
                        oCommonTransmissionViewModel = new CommonTransmissionViewModel();
                        oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

                        tempBatchNumber = line.Substring(1, 3);

                        oCommonTransmissionViewModel.RegionId = tempRowRegionId;
                        oCommonTransmissionViewModel.RegionName = tempRowRegionName;

                        //INFORMATION GET FROM RECORDTYPE 1 & 2
                        oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
                        oCommonTransmissionViewModel.Destination = tempDestination;
                        oCommonTransmissionViewModel.Origin = tempOrigin;
                        oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
                        oCommonTransmissionViewModel.HHMM = tempHHMM;
                        oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
                        oCommonTransmissionViewModel.ServiceType = tempServiceType;
                        oCommonTransmissionViewModel.RecordSize = tempRecordSize;
                        oCommonTransmissionViewModel.BlockSize = tempBlockSize;
                        oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

                        //RECORD TYPE 6
                        oCommonTransmissionViewModel.LockboxRaw = line;
                        oCommonTransmissionViewModel.RecordType = Recordtype;
                        oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
                        oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
                        oCommonTransmissionViewModel.ItemNumber = line.Substring(4, 3);

                        oCommonTransmissionViewModel.DollarAmount =
                            decimal.Parse(line.Substring(7, 10).Substring(0, 8) + "." +
                                          line.Substring(7, 10).Substring(8));

                        oCommonTransmissionViewModel.TransitRoutingNumber = line.Substring(17, 9);

                        oCommonTransmissionViewModel.AccountNumber = line.Substring(26, 10);
                        oCommonTransmissionViewModel.CheckNumber = line.Substring(36, 8);
                        oCommonTransmissionViewModel.CustomerNo = line.Substring(44, 6);
                        oCommonTransmissionViewModel.InvoiceNo = line.Substring(50, 18).Substring(3);
                        oCommonTransmissionViewModel.ApplyAmount =
                            decimal.Parse(line.Substring(68, 9).Substring(0, 7) + "." +
                                          line.Substring(68, 9).Substring(7));

                        client = new HttpClient();
                        client.BaseAddress = new Uri("https://www.routingnumbers.info/");
                        // Add an Accept header for JSON format.
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response =
                            client.GetAsync("api/data.json?rn=" + long.Parse(line.Substring(17, 9))).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                            BankDetailByRouting routes_list =
                                json_serializer.Deserialize<BankDetailByRouting>(
                                    response.Content.ReadAsStringAsync().Result);

                            tempRowBankName = routes_list.customer_name;
                            tempRowBankState = routes_list.state;
                        }

                        oCommonTransmissionViewModel.BankName = tempRowBankName;
                        oCommonTransmissionViewModel.BankState = tempRowBankState;
                        oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

                        oCommonTransmissionViewModel.LastOverflowIndicator = "";
                        oCommonTransmissionViewModel.LockboxDate = _LockboxDate;
                        oCommonTransmissionViewModel.LockboxEDIId = 0;
                        oCommonTransmissionViewModel.LockboxFileName = filePath;
                        oCommonTransmissionViewModel.RecordCount = "";
                        oCommonTransmissionViewModel.SequenceNumber = "";
                        oCommonTransmissionViewModel.TotalDollars = 0;
                        oCommonTransmissionViewModel.TotalItems = "";
                        oCommonTransmissionViewModel.TypeofOverFlowingRecord = "";

                        oCommonTransmissionViewModel.LockboxData = string.Concat(_input);

                        oTCommonTransmissionViewModel = oCommonTransmissionViewModel;
                        resultList.Add(oCommonTransmissionViewModel);

                        break;

                    case "4":
                        oCommonTransmissionViewModel = new CommonTransmissionViewModel();
                        oCommonTransmissionViewModel.RegionId = tempRowRegionId;
                        oCommonTransmissionViewModel.RegionName = tempRowRegionName;

                        //INFORMATION GET FROM RECORDTYPE 1 & 2
                        oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
                        oCommonTransmissionViewModel.Destination = tempDestination;
                        oCommonTransmissionViewModel.Origin = tempOrigin;
                        oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
                        oCommonTransmissionViewModel.HHMM = tempHHMM;
                        oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
                        oCommonTransmissionViewModel.ServiceType = tempServiceType;
                        oCommonTransmissionViewModel.RecordSize = tempRecordSize;
                        oCommonTransmissionViewModel.BlockSize = tempBlockSize;
                        oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

                        //RECORD TYPE 6
                        oCommonTransmissionViewModel.LockboxRaw = line;
                        oCommonTransmissionViewModel.RecordType = Recordtype;
                        oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
                        oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
                        oCommonTransmissionViewModel.ItemNumber = oTCommonTransmissionViewModel.ItemNumber;
                        oCommonTransmissionViewModel.DollarAmount =
                            oTCommonTransmissionViewModel.DollarAmount;
                        oCommonTransmissionViewModel.TransitRoutingNumber =
                            oTCommonTransmissionViewModel.TransitRoutingNumber;

                        oCommonTransmissionViewModel.AccountNumber =
                            oTCommonTransmissionViewModel.AccountNumber;
                        oCommonTransmissionViewModel.CheckNumber = oTCommonTransmissionViewModel.CheckNumber;
                        oCommonTransmissionViewModel.CustomerNo = line.Substring(11, 6);
                        oCommonTransmissionViewModel.InvoiceNo = line.Substring(17, 18).Substring(3);
                        oCommonTransmissionViewModel.ApplyAmount =
                            decimal.Parse(line.Substring(35, 9).Substring(0, 7) + "." +
                                          line.Substring(35, 9).Substring(7));

                        oCommonTransmissionViewModel.BankName = oTCommonTransmissionViewModel.BankName;
                        oCommonTransmissionViewModel.BankState = oTCommonTransmissionViewModel.BankState;
                        oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

                        oCommonTransmissionViewModel.LastOverflowIndicator = line.Substring(10, 1);
                        oCommonTransmissionViewModel.LockboxDate = _LockboxDate;
                        oCommonTransmissionViewModel.LockboxEDIId = 0;
                        oCommonTransmissionViewModel.LockboxFileName = "";
                        oCommonTransmissionViewModel.RecordCount = "";
                        oCommonTransmissionViewModel.SequenceNumber = line.Substring(8, 2);
                        oCommonTransmissionViewModel.TotalDollars = 0;
                        oCommonTransmissionViewModel.TotalItems = "";
                        oCommonTransmissionViewModel.TypeofOverFlowingRecord = line.Substring(7, 1);
                        oCommonTransmissionViewModel.LockboxData = string.Concat(_input);

                        resultList.Add(oCommonTransmissionViewModel);

                        break;

                    case "7":
                        foreach (
                            CommonTransmissionViewModel o in
                            resultList.Where(
                                t =>
                                    t.BatchNumber == line.Substring(1, 3) &&
                                    t.LockboxNumber == line.Substring(7, 7)).ToList())
                        {
                            o.TotalItems = line.Substring(20, 3);
                            o.TotalDollars =
                                decimal.Parse(line.Substring(23, 10).Substring(0, 8) + "." +
                                              line.Substring(23, 10).Substring(8));
                        }

                        break;

                    case "8":
                        foreach (
                            CommonTransmissionViewModel o in
                            resultList.Where(
                                t => t.LockboxNumber == line.Substring(7, 7)).ToList())
                        {
                            o.TotalDollars =
                                decimal.Parse(line.Substring(24, 10).Substring(0, 8) + "." +
                                              line.Substring(24, 10).Substring(8));
                        }
                        break;

                    case "9":

                        foreach (CommonTransmissionViewModel o in resultList)
                        {
                            o.RecordCount = line.Substring(1, 6);
                            o.LockboxDate = _LockboxDate;
                            o.LockboxFileName = filePath;
                        }
                        break;
                }
            }
            return resultList;
        }

        [HttpPost]
        public JsonResult LockboxUpload()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    DateTime CheckDate = !String.IsNullOrEmpty(Request.Form["checkdate"].ToString()) ? Convert.ToDateTime(Request.Form["checkdate"].ToString()) : DateTime.Now;
                    string fileTitle = string.Empty;
                    string basePath = Server.MapPath("~");
                    string filePath = string.Empty;
                    string fileLocation = string.Empty;
                    string fileMetaData = string.Empty;


                    var inputFile = Request.Files[0];
                    if (inputFile != null && inputFile.ContentLength > 0)
                    {
                        fileTitle = Path.GetFileName(inputFile.FileName);
                        filePath = "Upload/LockboxImport/" + fileTitle.Replace("." + fileTitle.Split('.').LastOrDefault(), "") +
                                       DateTime.Now.ToString("MMddyyyyHHmmss") + "." +
                                       fileTitle.Split('.').LastOrDefault();

                        fileLocation = Path.Combine(basePath, filePath);
                        inputFile.SaveAs(fileLocation);
                    }

                    //fileMetaData = System.IO.File.ReadAllText(fileLocation);
                    int _AlreadyExists = 0;
                    List<LockboxEDIDataViewModel> res = new List<LockboxEDIDataViewModel>();
                    LockboxEDI _LockboxEDI = new LockboxEDI();
                    string[] inputData = System.IO.File.ReadAllLines(fileLocation);
                    List<CommonTransmissionViewModel> ProcessResults = new List<CommonTransmissionViewModel>();
                    if (inputData.Length > 0)
                    {
                        string line = inputData[0];
                        _LockboxEDI = AccountReceivableService.AlreadyExistFileUploadLockboxUpdated(line);
                        if (_LockboxEDI.LockboxEDIId > 0)
                        {
                            _AlreadyExists = 1;// return Json(new { AlreadyExist = 1, LockboxData = _LockboxData, filePath = filePath });
                            AccountReceivableService.UploadLockboxDate(_LockboxEDI.LockboxEDIId, CheckDate);
                            res = AccountReceivableService.GetLockboxData(_LockboxEDI.LockboxEDIId);
                        }
                        else
                        {
                            ProcessResults = ProcessLockboxFile(inputData, filePath, CheckDate);
                            res = AccountReceivableService.UploadLockboxUpdated(ProcessResults);
                        }
                    }


                    if (res.Count > 0)
                    {
                        ViewBag.LockboxId = res.FirstOrDefault().LockboxEDIId;
                        ViewBag.LockboxProcessed = true;
                        if (res.Where(o => o.StatusListId != 52).Count() > 0)
                            ViewBag.LockboxProcessed = false;


                        if (_AlreadyExists == 1)
                        {
                            return Json(new
                            {
                                Lockbox = _LockboxEDI,
                                AlreadyExist = _AlreadyExists,
                                LockboxData = res,
                                LockboxId = ViewBag.LockboxId,
                                LockboxProcessed = ViewBag.LockboxProcessed,
                                filePath = filePath
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                AlreadyExist = _AlreadyExists,
                                LockboxData = res,
                                LockboxId = ViewBag.LockboxId,
                                LockboxProcessed = ViewBag.LockboxProcessed,
                                filePath = filePath
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Lockbox = _LockboxEDI,
                            AlreadyExist = _AlreadyExists,
                            LockboxData = res,
                            LockboxId = -1,
                            LockboxProcessed = false,
                            filePath = filePath
                        }, JsonRequestBehavior.AllowGet);


                    }







                    //decimal amt = ProcessResults.FirstOrDefault().TotalDollars;
                    //// decimal.Parse(lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(0, 8) + "." + lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(8));

                    //string retString = ProcessResults.FirstOrDefault().LockboxNumber + "|" +
                    //                   ProcessResults.Count() + "|" + amt + "|" + res;
                    //return Json(retString);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                    throw ex;
                }
            }
            else
            {
                return Json("No files selected.");
            }

        }

        [HttpPost]
        public JsonResult LockboxUploadByPath(string _path, string _chDate)
        {
            try
            {
                if (_path != "")
                {
                    string basePath = Server.MapPath("~");
                    DateTime CheckDate = !String.IsNullOrEmpty(_chDate) ? Convert.ToDateTime(_chDate) : DateTime.Now;
                    string[] inputData = System.IO.File.ReadAllLines(Path.Combine(basePath, _path));
                    List<CommonTransmissionViewModel> ProcessResults = new List<CommonTransmissionViewModel>();
                    if (inputData.Length > 0)
                    {
                        ProcessResults = ProcessLockboxFile(inputData, _path, CheckDate);
                    }
                    var res = AccountReceivableService.UploadLockboxUpdated(ProcessResults);

                    ViewBag.LockboxId = res.FirstOrDefault().LockboxEDIId;
                    ViewBag.LockboxProcessed = true;

                    if (res.Where(o => o.StatusListId != 52).Count() > 0)
                        ViewBag.LockboxProcessed = false;

                    return Json(new
                    {
                        AlreadyExist = 0,
                        LockboxData = res,
                        LockboxId = ViewBag.LockboxId,
                        LockboxProcessed = ViewBag.LockboxProcessed,
                        filePath = _path
                    }, JsonRequestBehavior.AllowGet);


                    //decimal amt = ProcessResults.FirstOrDefault().TotalDollars;
                    //string retString = ProcessResults.FirstOrDefault().LockboxNumber + "|" + ProcessResults.Count() + "|" + amt + "|" + res.FirstOrDefault().LockboxEDIId;
                    //return Json(retString);
                }
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
                throw ex;
            }
            return Json("No files selected.");
        }







        //[HttpPost]
        //public JsonResult LockboxImportUploadUpdated()
        //{
        //    ViewBag.CurrentMenu = "AccountsReceivablePayment";
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            var file = Request.Files[0];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = Path.GetFileName(file.FileName);
        //                var basePath = Server.MapPath("~");
        //                var path = "/Upload/LockboxImport/" + fileName;
        //                var locationPath = basePath + path;
        //                if (System.IO.File.Exists(path))
        //                {
        //                    fileName = fileName.Replace("." + fileName.Split('.').LastOrDefault(), "") +
        //                               DateTime.Now.ToString("MMddyyyyHHmmss") + "." +
        //                               fileName.Split('.').LastOrDefault();
        //                    locationPath = Path.Combine(basePath, path);
        //                }
        //                file.SaveAs(locationPath);

        //                string txtLockboxData = System.IO.File.ReadAllText(locationPath);

        //                List<CommonTransmissionViewModel> lstCommonTransmissionViewModel = new List<CommonTransmissionViewModel>();

        //                CommonTransmissionViewModel oCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //                CommonTransmissionViewModel oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //                HttpClient client;

        //                string tempPriorityCode = "";
        //                string tempDestination = "";
        //                string tempOrigin = "";
        //                string tempYYMMDD = "";
        //                string tempHHMM = "";
        //                string tempReferenceCode = "";
        //                string tempServiceType = "";
        //                string tempRecordSize = "";
        //                string tempBlockSize = "";
        //                string tempFormatCodeUncompressed = "";

        //                string tempRowDestination = "";
        //                string tempRowOrigin = "";
        //                string tempRowYYMMDD = "";

        //                string tempRegionBankName = "";
        //                string tempRowBankName = "";
        //                string tempRowBankState = "";
        //                string tempLockboxNumber = "", tempBatchNumber = "";

        //                string[] lines = System.IO.File.ReadAllLines(locationPath);


        //                if (lines.Length > 0)
        //                {
        //                    string line = lines[0];
        //                    var _LockboxData = AccountReceivableService.AlreadyExistFileUploadLockboxUpdated(line);
        //                    if (_LockboxData.LockboxEDIId > 0)
        //                        return Json(new { LockboxData = _LockboxData, filePath = path });
        //                }


        //                for (int i = 0; i < lines.Length; i++)
        //                {
        //                    string line = lines[i];

        //                    //Record Type '1'
        //                    //Field Field Position Length  Description Fill Contents Fill Char Justifica-tion
        //                    string Recordtype = line.Substring(0, 1);

        //                    switch (Recordtype)
        //                    {
        //                        case "1":
        //                            tempPriorityCode = line.Substring(1, 2);
        //                            tempDestination = line.Substring(3, 10);
        //                            tempOrigin = line.Substring(13, 10);
        //                            tempYYMMDD = line.Substring(23, 6);
        //                            tempHHMM = line.Substring(29, 4);

        //                            client = new HttpClient();
        //                            client.BaseAddress = new Uri("https://www.routingnumbers.info/");

        //                            // Add an Accept header for JSON format.
        //                            client.DefaultRequestHeaders.Accept.Add(
        //                                new MediaTypeWithQualityHeaderValue("application/json"));
        //                            HttpResponseMessage responseM =
        //                                client.GetAsync("api/data.json?rn=" + long.Parse(tempOrigin)).Result;

        //                            if (responseM.IsSuccessStatusCode)
        //                            {
        //                                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //                                BankDetailByRouting routes_list =
        //                                    json_serializer.Deserialize<BankDetailByRouting>(
        //                                        responseM.Content.ReadAsStringAsync().Result);

        //                                tempRegionBankName = routes_list.customer_name;
        //                            }

        //                            break;

        //                        case "2":
        //                            tempReferenceCode = line.Substring(21, 10);
        //                            tempServiceType = line.Substring(31, 3);
        //                            tempRecordSize = line.Substring(34, 3);
        //                            tempBlockSize = line.Substring(37, 4);
        //                            tempFormatCodeUncompressed = line.Substring(41, 1);
        //                            break;

        //                        case "5":
        //                            tempBatchNumber = "";
        //                            tempLockboxNumber = "";
        //                            tempRowYYMMDD = "";
        //                            tempRowDestination = "";
        //                            tempRowOrigin = "";

        //                            tempBatchNumber = line.Substring(1, 3);
        //                            tempLockboxNumber = line.Substring(7, 7);
        //                            tempRowYYMMDD = line.Substring(14, 6);
        //                            tempRowDestination = line.Substring(20, 10);
        //                            tempRowOrigin = line.Substring(30, 10);
        //                            break;

        //                        case "6":
        //                            oCommonTransmissionViewModel = new CommonTransmissionViewModel();
        //                            oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //                            tempBatchNumber = line.Substring(1, 3);

        //                            //INFORMATION GET FROM RECORDTYPE 1 & 2
        //                            oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
        //                            oCommonTransmissionViewModel.Destination = tempDestination;
        //                            oCommonTransmissionViewModel.Origin = tempOrigin;
        //                            oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
        //                            oCommonTransmissionViewModel.HHMM = tempHHMM;
        //                            oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
        //                            oCommonTransmissionViewModel.ServiceType = tempServiceType;
        //                            oCommonTransmissionViewModel.RecordSize = tempRecordSize;
        //                            oCommonTransmissionViewModel.BlockSize = tempBlockSize;
        //                            oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

        //                            //RECORD TYPE 6
        //                            oCommonTransmissionViewModel.LockboxRaw = line;
        //                            oCommonTransmissionViewModel.RecordType = Recordtype;
        //                            oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
        //                            oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
        //                            oCommonTransmissionViewModel.ItemNumber = line.Substring(4, 3);

        //                            oCommonTransmissionViewModel.DollarAmount =
        //                                decimal.Parse(line.Substring(7, 10).Substring(0, 8) + "." +
        //                                              line.Substring(7, 10).Substring(8));

        //                            oCommonTransmissionViewModel.TransitRoutingNumber = line.Substring(17, 9);

        //                            oCommonTransmissionViewModel.AccountNumber = line.Substring(26, 10);
        //                            oCommonTransmissionViewModel.CheckNumber = line.Substring(36, 8);
        //                            oCommonTransmissionViewModel.CustomerNo = line.Substring(44, 6);
        //                            oCommonTransmissionViewModel.InvoiceNo = line.Substring(50, 18).Substring(3);
        //                            oCommonTransmissionViewModel.ApplyAmount =
        //                                decimal.Parse(line.Substring(68, 9).Substring(0, 7) + "." +
        //                                              line.Substring(68, 9).Substring(7));

        //                            client = new HttpClient();
        //                            client.BaseAddress = new Uri("https://www.routingnumbers.info/");
        //                            // Add an Accept header for JSON format.
        //                            client.DefaultRequestHeaders.Accept.Add(
        //                                new MediaTypeWithQualityHeaderValue("application/json"));
        //                            HttpResponseMessage response =
        //                                client.GetAsync("api/data.json?rn=" + long.Parse(line.Substring(17, 9))).Result;

        //                            if (response.IsSuccessStatusCode)
        //                            {
        //                                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //                                BankDetailByRouting routes_list =
        //                                    json_serializer.Deserialize<BankDetailByRouting>(
        //                                        response.Content.ReadAsStringAsync().Result);

        //                                tempRowBankName = routes_list.customer_name;
        //                                tempRowBankState = routes_list.state;
        //                            }

        //                            oCommonTransmissionViewModel.BankName = tempRowBankName;
        //                            oCommonTransmissionViewModel.BankState = tempRowBankState;
        //                            oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

        //                            oCommonTransmissionViewModel.LastOverflowIndicator = "";
        //                            oCommonTransmissionViewModel.LockboxDate = DateTime.Now;
        //                            oCommonTransmissionViewModel.LockboxEDIId = 0;
        //                            oCommonTransmissionViewModel.LockboxFileName = path;
        //                            oCommonTransmissionViewModel.RecordCount = "";
        //                            oCommonTransmissionViewModel.SequenceNumber = "";
        //                            oCommonTransmissionViewModel.TotalDollars = 0;
        //                            oCommonTransmissionViewModel.TotalItems = "";
        //                            oCommonTransmissionViewModel.TypeofOverFlowingRecord = "";

        //                            oCommonTransmissionViewModel.LockboxData = string.Concat(lines);

        //                            oTCommonTransmissionViewModel = oCommonTransmissionViewModel;
        //                            lstCommonTransmissionViewModel.Add(oCommonTransmissionViewModel);

        //                            break;

        //                        case "4":
        //                            oCommonTransmissionViewModel = new CommonTransmissionViewModel();
        //                            //INFORMATION GET FROM RECORDTYPE 1 & 2
        //                            oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
        //                            oCommonTransmissionViewModel.Destination = tempDestination;
        //                            oCommonTransmissionViewModel.Origin = tempOrigin;
        //                            oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
        //                            oCommonTransmissionViewModel.HHMM = tempHHMM;
        //                            oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
        //                            oCommonTransmissionViewModel.ServiceType = tempServiceType;
        //                            oCommonTransmissionViewModel.RecordSize = tempRecordSize;
        //                            oCommonTransmissionViewModel.BlockSize = tempBlockSize;
        //                            oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

        //                            //RECORD TYPE 6
        //                            oCommonTransmissionViewModel.LockboxRaw = line;
        //                            oCommonTransmissionViewModel.RecordType = Recordtype;
        //                            oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
        //                            oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
        //                            oCommonTransmissionViewModel.ItemNumber = oTCommonTransmissionViewModel.ItemNumber;
        //                            oCommonTransmissionViewModel.DollarAmount =
        //                                oTCommonTransmissionViewModel.DollarAmount;
        //                            oCommonTransmissionViewModel.TransitRoutingNumber =
        //                                oTCommonTransmissionViewModel.TransitRoutingNumber;

        //                            oCommonTransmissionViewModel.AccountNumber =
        //                                oTCommonTransmissionViewModel.AccountNumber;
        //                            oCommonTransmissionViewModel.CheckNumber = oTCommonTransmissionViewModel.CheckNumber;
        //                            oCommonTransmissionViewModel.CustomerNo = line.Substring(11, 6);
        //                            oCommonTransmissionViewModel.InvoiceNo = line.Substring(17, 18).Substring(3);
        //                            oCommonTransmissionViewModel.ApplyAmount =
        //                                decimal.Parse(line.Substring(35, 9).Substring(0, 7) + "." +
        //                                              line.Substring(35, 9).Substring(7));

        //                            oCommonTransmissionViewModel.BankName = oTCommonTransmissionViewModel.BankName;
        //                            oCommonTransmissionViewModel.BankState = oTCommonTransmissionViewModel.BankState;
        //                            oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

        //                            oCommonTransmissionViewModel.LastOverflowIndicator = line.Substring(10, 1);
        //                            oCommonTransmissionViewModel.LockboxDate = DateTime.Now;
        //                            oCommonTransmissionViewModel.LockboxEDIId = 0;
        //                            oCommonTransmissionViewModel.LockboxFileName = "";
        //                            oCommonTransmissionViewModel.RecordCount = "";
        //                            oCommonTransmissionViewModel.SequenceNumber = line.Substring(8, 2);
        //                            oCommonTransmissionViewModel.TotalDollars = 0;
        //                            oCommonTransmissionViewModel.TotalItems = "";
        //                            oCommonTransmissionViewModel.TypeofOverFlowingRecord = line.Substring(7, 1);
        //                            oCommonTransmissionViewModel.LockboxData = string.Concat(lines);

        //                            lstCommonTransmissionViewModel.Add(oCommonTransmissionViewModel);

        //                            break;

        //                        case "7":
        //                            foreach (
        //                                CommonTransmissionViewModel o in
        //                                lstCommonTransmissionViewModel.Where(
        //                                    t =>
        //                                        t.BatchNumber == line.Substring(1, 3) &&
        //                                        t.LockboxNumber == line.Substring(7, 7)).ToList())
        //                            {
        //                                o.TotalItems = line.Substring(20, 3);
        //                                o.TotalDollars =
        //                                    decimal.Parse(line.Substring(23, 10).Substring(0, 8) + "." +
        //                                                  line.Substring(23, 10).Substring(8));
        //                            }

        //                            break;

        //                        case "8":
        //                            foreach (
        //                                CommonTransmissionViewModel o in
        //                                lstCommonTransmissionViewModel.Where(
        //                                    t => t.LockboxNumber == line.Substring(7, 7)).ToList())
        //                            {
        //                                o.TotalDollars =
        //                                    decimal.Parse(line.Substring(24, 10).Substring(0, 8) + "." +
        //                                                  line.Substring(24, 10).Substring(8));
        //                            }
        //                            break;

        //                        case "9":

        //                            foreach (CommonTransmissionViewModel o in lstCommonTransmissionViewModel)
        //                            {
        //                                o.RecordCount = line.Substring(1, 6);
        //                                o.LockboxDate = DateTime.Now;
        //                                o.LockboxFileName = path;
        //                            }
        //                            break;
        //                    }
        //                }

        //                int res = AccountReceivableService.UploadLockboxUpdated(lstCommonTransmissionViewModel);

        //                decimal amt = lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars;
        //                // decimal.Parse(lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(0, 8) + "." + lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(8));

        //                string retString = lstCommonTransmissionViewModel.FirstOrDefault().LockboxNumber + "|" +
        //                                   lstCommonTransmissionViewModel.Count() + "|" + amt + "|" + res;
        //                return Json(retString);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //    return Json("No files selected.");
        //}


        //[HttpPost]
        //public JsonResult LockboxImportUploadUpdatedSubmit(string _path)
        //{
        //    ViewBag.CurrentMenu = "AccountsReceivablePayment";

        //    try
        //    {
        //        if (_path != "")
        //        {
        //            string txtLockboxData = System.IO.File.ReadAllText(_path);

        //            List<CommonTransmissionViewModel> lstCommonTransmissionViewModel =
        //                new List<CommonTransmissionViewModel>();

        //            CommonTransmissionViewModel oCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //            CommonTransmissionViewModel oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //            HttpClient client;

        //            string tempPriorityCode = "";
        //            string tempDestination = "";
        //            string tempOrigin = "";
        //            string tempYYMMDD = "";
        //            string tempHHMM = "";
        //            string tempReferenceCode = "";
        //            string tempServiceType = "";
        //            string tempRecordSize = "";
        //            string tempBlockSize = "";
        //            string tempFormatCodeUncompressed = "";

        //            string tempRowDestination = "";
        //            string tempRowOrigin = "";
        //            string tempRowYYMMDD = "";

        //            string tempRegionBankName = "";
        //            string tempRowBankName = "";
        //            string tempRowBankState = "";
        //            string tempLockboxNumber = "", tempBatchNumber = "";

        //            string[] lines = System.IO.File.ReadAllLines(_path);

        //            for (int i = 0; i < lines.Length; i++)
        //            {
        //                string line = lines[i];

        //                //Record Type '1'
        //                //Field Field Position Length  Description Fill Contents Fill Char Justifica-tion
        //                string Recordtype = line.Substring(0, 1);

        //                switch (Recordtype)
        //                {
        //                    case "1":
        //                        tempPriorityCode = line.Substring(1, 2);
        //                        tempDestination = line.Substring(3, 10);
        //                        tempOrigin = line.Substring(13, 10);
        //                        tempYYMMDD = line.Substring(23, 6);
        //                        tempHHMM = line.Substring(29, 4);

        //                        client = new HttpClient();
        //                        client.BaseAddress = new Uri("https://www.routingnumbers.info/");

        //                        // Add an Accept header for JSON format.
        //                        client.DefaultRequestHeaders.Accept.Add(
        //                            new MediaTypeWithQualityHeaderValue("application/json"));
        //                        HttpResponseMessage responseM =
        //                            client.GetAsync("api/data.json?rn=" + long.Parse(tempOrigin)).Result;

        //                        if (responseM.IsSuccessStatusCode)
        //                        {
        //                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //                            BankDetailByRouting routes_list =
        //                                json_serializer.Deserialize<BankDetailByRouting>(
        //                                    responseM.Content.ReadAsStringAsync().Result);

        //                            tempRegionBankName = routes_list.customer_name;
        //                        }

        //                        break;

        //                    case "2":
        //                        tempReferenceCode = line.Substring(21, 10);
        //                        tempServiceType = line.Substring(31, 3);
        //                        tempRecordSize = line.Substring(34, 3);
        //                        tempBlockSize = line.Substring(37, 4);
        //                        tempFormatCodeUncompressed = line.Substring(41, 1);
        //                        break;

        //                    case "5":
        //                        tempBatchNumber = "";
        //                        tempLockboxNumber = "";
        //                        tempRowYYMMDD = "";
        //                        tempRowDestination = "";
        //                        tempRowOrigin = "";

        //                        tempBatchNumber = line.Substring(1, 3);
        //                        tempLockboxNumber = line.Substring(7, 7);
        //                        tempRowYYMMDD = line.Substring(14, 6);
        //                        tempRowDestination = line.Substring(20, 10);
        //                        tempRowOrigin = line.Substring(30, 10);
        //                        break;

        //                    case "6":
        //                        oCommonTransmissionViewModel = new CommonTransmissionViewModel();
        //                        oTCommonTransmissionViewModel = new CommonTransmissionViewModel();

        //                        tempBatchNumber = line.Substring(1, 3);

        //                        //INFORMATION GET FROM RECORDTYPE 1 & 2
        //                        oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
        //                        oCommonTransmissionViewModel.Destination = tempDestination;
        //                        oCommonTransmissionViewModel.Origin = tempOrigin;
        //                        oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
        //                        oCommonTransmissionViewModel.HHMM = tempHHMM;
        //                        oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
        //                        oCommonTransmissionViewModel.ServiceType = tempServiceType;
        //                        oCommonTransmissionViewModel.RecordSize = tempRecordSize;
        //                        oCommonTransmissionViewModel.BlockSize = tempBlockSize;
        //                        oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

        //                        //RECORD TYPE 6
        //                        oCommonTransmissionViewModel.LockboxRaw = line;
        //                        oCommonTransmissionViewModel.RecordType = Recordtype;
        //                        oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
        //                        oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
        //                        oCommonTransmissionViewModel.ItemNumber = line.Substring(4, 3);
        //                        oCommonTransmissionViewModel.DollarAmount =
        //                            decimal.Parse(line.Substring(7, 10).Substring(0, 8) + "." +
        //                                          line.Substring(7, 10).Substring(8));
        //                        oCommonTransmissionViewModel.TransitRoutingNumber = line.Substring(17, 9);

        //                        oCommonTransmissionViewModel.AccountNumber = line.Substring(26, 10);
        //                        oCommonTransmissionViewModel.CheckNumber = line.Substring(36, 8);
        //                        oCommonTransmissionViewModel.CustomerNo = line.Substring(44, 6);
        //                        oCommonTransmissionViewModel.InvoiceNo = line.Substring(50, 18).Substring(3);
        //                        oCommonTransmissionViewModel.ApplyAmount =
        //                            decimal.Parse(line.Substring(68, 9).Substring(0, 7) + "." +
        //                                          line.Substring(68, 9).Substring(7));

        //                        client = new HttpClient();
        //                        client.BaseAddress = new Uri("https://www.routingnumbers.info/");
        //                        // Add an Accept header for JSON format.
        //                        client.DefaultRequestHeaders.Accept.Add(
        //                            new MediaTypeWithQualityHeaderValue("application/json"));
        //                        HttpResponseMessage response =
        //                            client.GetAsync("api/data.json?rn=" + long.Parse(line.Substring(17, 9))).Result;

        //                        if (response.IsSuccessStatusCode)
        //                        {
        //                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //                            BankDetailByRouting routes_list =
        //                                json_serializer.Deserialize<BankDetailByRouting>(
        //                                    response.Content.ReadAsStringAsync().Result);

        //                            tempRowBankName = routes_list.customer_name;
        //                            tempRowBankState = routes_list.state;
        //                        }

        //                        oCommonTransmissionViewModel.BankName = tempRowBankName;
        //                        oCommonTransmissionViewModel.BankState = tempRowBankState;
        //                        oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

        //                        oCommonTransmissionViewModel.LastOverflowIndicator = "";
        //                        oCommonTransmissionViewModel.LockboxDate = DateTime.Now;
        //                        oCommonTransmissionViewModel.LockboxEDIId = 0;
        //                        oCommonTransmissionViewModel.LockboxFileName = _path;
        //                        oCommonTransmissionViewModel.RecordCount = "";
        //                        oCommonTransmissionViewModel.SequenceNumber = "";
        //                        oCommonTransmissionViewModel.TotalDollars = 0;
        //                        oCommonTransmissionViewModel.TotalItems = "";
        //                        oCommonTransmissionViewModel.TypeofOverFlowingRecord = "";

        //                        oCommonTransmissionViewModel.LockboxData = string.Concat(lines);

        //                        oTCommonTransmissionViewModel = oCommonTransmissionViewModel;
        //                        lstCommonTransmissionViewModel.Add(oCommonTransmissionViewModel);

        //                        break;

        //                    case "4":
        //                        oCommonTransmissionViewModel = new CommonTransmissionViewModel();
        //                        //INFORMATION GET FROM RECORDTYPE 1 & 2
        //                        oCommonTransmissionViewModel.PriorityCode = tempPriorityCode;
        //                        oCommonTransmissionViewModel.Destination = tempDestination;
        //                        oCommonTransmissionViewModel.Origin = tempOrigin;
        //                        oCommonTransmissionViewModel.YYMMDD = tempYYMMDD;
        //                        oCommonTransmissionViewModel.HHMM = tempHHMM;
        //                        oCommonTransmissionViewModel.ReferenceCode = tempReferenceCode;
        //                        oCommonTransmissionViewModel.ServiceType = tempServiceType;
        //                        oCommonTransmissionViewModel.RecordSize = tempRecordSize;
        //                        oCommonTransmissionViewModel.BlockSize = tempBlockSize;
        //                        oCommonTransmissionViewModel.FormatCodeUncompressed = tempFormatCodeUncompressed;

        //                        //RECORD TYPE 6
        //                        oCommonTransmissionViewModel.LockboxRaw = line;
        //                        oCommonTransmissionViewModel.RecordType = Recordtype;
        //                        oCommonTransmissionViewModel.BatchNumber = tempBatchNumber;
        //                        oCommonTransmissionViewModel.LockboxNumber = tempLockboxNumber;
        //                        oCommonTransmissionViewModel.ItemNumber = oTCommonTransmissionViewModel.ItemNumber;
        //                        oCommonTransmissionViewModel.DollarAmount = oTCommonTransmissionViewModel.DollarAmount;
        //                        oCommonTransmissionViewModel.TransitRoutingNumber =
        //                            oTCommonTransmissionViewModel.TransitRoutingNumber;

        //                        oCommonTransmissionViewModel.AccountNumber = oTCommonTransmissionViewModel.AccountNumber;
        //                        oCommonTransmissionViewModel.CheckNumber = oTCommonTransmissionViewModel.CheckNumber;
        //                        oCommonTransmissionViewModel.CustomerNo = line.Substring(11, 6);
        //                        oCommonTransmissionViewModel.InvoiceNo = line.Substring(17, 18).Substring(3);
        //                        oCommonTransmissionViewModel.ApplyAmount =
        //                            decimal.Parse(line.Substring(35, 9).Substring(0, 7) + "." +
        //                                          line.Substring(35, 9).Substring(7));

        //                        oCommonTransmissionViewModel.BankName = oTCommonTransmissionViewModel.BankName;
        //                        oCommonTransmissionViewModel.BankState = oTCommonTransmissionViewModel.BankState;
        //                        oCommonTransmissionViewModel.RegionBankName = tempRegionBankName;

        //                        oCommonTransmissionViewModel.LastOverflowIndicator = line.Substring(10, 1);
        //                        oCommonTransmissionViewModel.LockboxDate = DateTime.Now;
        //                        oCommonTransmissionViewModel.LockboxEDIId = 0;
        //                        oCommonTransmissionViewModel.LockboxFileName = "";
        //                        oCommonTransmissionViewModel.RecordCount = "";
        //                        oCommonTransmissionViewModel.SequenceNumber = line.Substring(8, 2);
        //                        oCommonTransmissionViewModel.TotalDollars = 0;
        //                        oCommonTransmissionViewModel.TotalItems = "";
        //                        oCommonTransmissionViewModel.TypeofOverFlowingRecord = line.Substring(7, 1);
        //                        oCommonTransmissionViewModel.LockboxData = string.Concat(lines);

        //                        lstCommonTransmissionViewModel.Add(oCommonTransmissionViewModel);

        //                        break;

        //                    case "7":
        //                        foreach (
        //                            CommonTransmissionViewModel o in
        //                            lstCommonTransmissionViewModel.Where(
        //                                t =>
        //                                    t.BatchNumber == line.Substring(1, 3) &&
        //                                    t.LockboxNumber == line.Substring(7, 7)).ToList())
        //                        {
        //                            o.TotalItems = line.Substring(20, 3);
        //                            o.TotalDollars =
        //                                decimal.Parse(line.Substring(23, 10).Substring(0, 8) + "." +
        //                                              line.Substring(23, 10).Substring(8));
        //                        }

        //                        break;

        //                    case "8":
        //                        foreach (
        //                            CommonTransmissionViewModel o in
        //                            lstCommonTransmissionViewModel.Where(t => t.LockboxNumber == line.Substring(7, 7))
        //                                .ToList())
        //                        {
        //                            o.TotalDollars =
        //                                decimal.Parse(line.Substring(24, 10).Substring(0, 8) + "." +
        //                                              line.Substring(24, 10).Substring(8));
        //                        }
        //                        break;

        //                    case "9":

        //                        foreach (CommonTransmissionViewModel o in lstCommonTransmissionViewModel)
        //                        {
        //                            o.RecordCount = line.Substring(1, 6);
        //                            o.LockboxDate = DateTime.Now;
        //                            o.LockboxFileName = _path;
        //                        }
        //                        break;
        //                }
        //            }

        //            int res = AccountReceivableService.UploadLockboxUpdated(lstCommonTransmissionViewModel);

        //            decimal amt = lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars;
        //            // decimal.Parse(lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(0, 8) + "." + lstCommonTransmissionViewModel.FirstOrDefault().TotalDollars.ToString().Substring(8));

        //            string retString = lstCommonTransmissionViewModel.FirstOrDefault().LockboxNumber + "|" +
        //                               lstCommonTransmissionViewModel.Count() + "|" + amt + "|" + res;
        //            return Json(retString);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("Error occurred. Error details: " + ex.Message);
        //    }

        //    return Json("No files selected.");
        //}

        [HttpGet]
        public ActionResult ApplyLockboxPaymentFormAllow(int Id)
        {
            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForInvoice(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");

            return PartialView("_PartialApplyLockboxPaymentForm", model);
        }

        [HttpGet]
        public ActionResult ApplyLockboxPaymentForm(int Id)
        {
            CreditDetailViewModel model = AccountReceivableService.GetCreditDetailForInvoice(Id);
            ViewBag.LineNoList = new SelectList(model.Invoice.InvoiceDetailItems, "LineNumber", "LineNumber");

            return PartialView("_PartialApplyLockboxPaymentForm", model);
        }

        [HttpGet]
        public JsonResult GetApplyLockboxPaymentForm(int Id)
        {
            bool model = AccountReceivableService.HaveMultipleInvoiceDistribution(Id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PaymentDetailPopup(int paymentId)
        {
            PaymentDetailsPopupModel PaymentDetailsPopupModel = new PaymentDetailsPopupModel();
            PaymentDetailsPopupModel = AccountReceivableService.PaymentDetailPopup(paymentId);

            return PartialView("_PartialPaymentDetailPopup", PaymentDetailsPopupModel);
        }

        public JsonResult PaymentDetailPrint(int paymentId)
        {
            if (paymentId > 0)
            {
                string HTMLContent = string.Empty;

                var data = AccountReceivableService.GetPaymentDetailPrint(paymentId);
                HTMLContent += RenderPartialViewToString("_PartialPaymentPrint", data);

                var retPath = "/Upload/InvoiceFiles/" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"),
                    DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");

                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult EditPaymentDetailPopup(int Id)
        {
            PaymentDetailsPopupModel PaymentDetailsPopupModel = new PaymentDetailsPopupModel();
            //PaymentDetailsPopupModel = AccountReceivableService.PaymentDetailPopup(Id);
            PaymentDetailsPopupModel = AccountReceivableService.PaymentDetailPopupFromTemp(Id, true);
            PaymentDetailsPopupModel.RegionId = SelectedRegionId;
            //return PartialView("_PartialPaymentDetailPopup", PaymentDetailsPopupModel);
            ViewBag.PaymentMethodList = new SelectList(AccountReceivableService.GetAll_PaymentMethodList(),
                "PaymentMethodListId", "Name", PaymentDetailsPopupModel.PaymentTypeId);
            return PartialView("_PartialEditPaymentDetailPopup", PaymentDetailsPopupModel);
        }
        [HttpPost]
        public ActionResult UpdatePaymentDetailPopup(FormCollection frm)
        {
            int Id = !string.IsNullOrEmpty(frm["hdnPaymentId"]) ? int.Parse(frm["hdnPaymentId"].ToString()) : 0;
            DateTime PaymentDate = !string.IsNullOrEmpty(frm["txtPaymentDate"]) ? DateTime.Parse(frm["txtPaymentDate"].ToString()) : DateTime.Now;
            int PaymentType = !string.IsNullOrEmpty(frm["slPaymentType"]) ? int.Parse(frm["slPaymentType"].ToString()) : 0;
            string PaymentNo = !string.IsNullOrEmpty(frm["txtPaymentNo"]) ? (frm["txtPaymentNo"].ToString()) : "";
            decimal Amount = !string.IsNullOrEmpty(frm["creditAmt"]) ? decimal.Parse(frm["creditAmt"].ToString().Replace(",", "").Replace("$", "")) : 0;
            string Note = !string.IsNullOrEmpty(frm["creditDesc"]) ? (frm["creditDesc"].ToString()) : "";



            int ischangeInvoiceData = !string.IsNullOrEmpty(frm["oinv_changeInvoiceData"]) ? int.Parse(frm["oinv_changeInvoiceData"].ToString()) : 0;


            int paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentType"].ToString().Trim()) ? int.Parse(frm["slPaymentType"].ToString().Trim()) : 0;
            string referenceNo = !String.IsNullOrEmpty(frm["txtPaymentNo"].ToString()) ? frm["txtPaymentNo"].ToString() : "";
            string notes = !String.IsNullOrEmpty(frm["creditDesc"].ToString()) ? frm["creditDesc"].ToString() : "";
            DateTime paymentDate = !String.IsNullOrEmpty(frm["txtPaymentDate"].ToString()) ? DateTime.Parse(frm["txtPaymentDate"]) : DateTime.Now;
            decimal paymentAmt = !string.IsNullOrEmpty(frm["oldBalance"].ToString().Replace(",", "").Replace("$", "")) ? Decimal.Parse(frm["oldBalance"].ToString().Replace(",", "").Replace("$", "")) : 0.00M;
            decimal creditAmt = !string.IsNullOrEmpty(frm["creditAmt"].ToString().Replace(",", "").Replace("$", "")) ? Decimal.Parse(frm["creditAmt"].ToString().Replace(",", "").Replace("$", "")) : 0.00M;
            decimal balance = !string.IsNullOrEmpty(frm["newBalance"].ToString().Replace(",", "").Replace("$", "")) ? Decimal.Parse(frm["newBalance"].ToString().Replace(",", "").Replace("$", "")) : 0.00M;
            bool mp_chkApplyCredit = false;
            int ClassId = -1;// !String.IsNullOrEmpty(frm["hdfCustomerId"].ToString()) ? int.Parse(frm["hdfCustomerId"].ToString()) : -1;
                             //string Last4CC = !String.IsNullOrEmpty(frm["Last4CC"].ToString()) ? frm["Last4CC"].ToString().Replace("XXXX", "") : "";




            int _RegionId = SelectedRegionId;

            if ((paymentAmt > 0 || creditAmt > 0))
            {

                FullManualPaymentViewModel oMainObject = new FullManualPaymentViewModel();
                if (ischangeInvoiceData > 0)
                {
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

                    foreach (string strKey in frm.AllKeys.Where(w => Regex.Match(w, @"oinv_InvoiceId_([a-zA-Z0-9]*)").Success))
                    {
                        var invId = int.Parse(strKey.Split('_')[2].ToString());
                        var invPayment = !String.IsNullOrEmpty(frm["oinv_ApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                            ? Decimal.Parse(frm["oinv_ApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                            : 0.00M;
                        var invBalance = !String.IsNullOrEmpty(frm["oinv_ApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                            ? Decimal.Parse(frm["oinv_ApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                            : 0.00M;
                        var invOverflow = !String.IsNullOrEmpty(frm["oinv_OApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                           ? Decimal.Parse(frm["oinv_OApplyAmount_" + invId].ToString().Replace(",", "").Replace("$", ""))
                           : 0.00M;

                        var invCustomerId = !String.IsNullOrEmpty(frm["oinv_CustomerId_" + invId].ToString().Replace(",", "").Replace("$", ""))
                           ? int.Parse(frm["oinv_CustomerId_" + invId].ToString().Replace(",", "").Replace("$", ""))
                           : -1;


                        //InvoiceDetail 
                        CreditDetailViewModel invoiceDetail = AccountReceivableService.GetCreditDetailForInvoicePayment(invId);
                        //invoiceDetail.InvoiceAmount;
                        invBalance = invoiceDetail.InvoiceBalance - invPayment;

                        oManualInvoice = new MPInvoiceViewModel();
                        oManualInvoice.InvoiceId = invId;
                        oManualInvoice.InvoiceCustomerId = invCustomerId;
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


                        if (invoiceDetail.Invoice.InvoiceDetailItems.Count() > 0)
                        {

                            decimal _itemBalance = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Balance);
                            decimal _itemTAXAmount = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.TAXAmount);
                            decimal _itemTotal = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Total);

                            //var item = invoiceDetail.Invoice.InvoiceDetailItems[0];
                            bool foundFields = true;
                            decimal itemPaymentAmt = 0;
                            decimal itemTotal = 0;

                            // sanity check to see if payment details were set, whole invoice was paid, or there is only one line item
                            if (oManualInvoice.PaidInFull || invoiceDetail.Invoice.InvoiceDetailItems.Count == 1)
                            {
                                //var taxRate = item.TAXAmount / item.Total;
                                itemPaymentAmt = applyAmountforPartialPay;
                                //var taxAmount = (decimal)(applyAmountforPartialPay * taxRate);
                                //itemTotal = applyAmountforPartialPay - taxAmount;

                                if (invoiceDetail.InvoiceBalance > 0)
                                {
                                    if (invoiceDetail.InvoiceBalance <= applyAmountforPartialPay)
                                    {
                                        invPayment = (decimal)invoiceDetail.InvoiceBalance;
                                        applyAmountforPartialPay = applyAmountforPartialPay - (decimal)invoiceDetail.InvoiceBalance;
                                    }
                                    else
                                    {
                                        invPayment = applyAmountforPartialPay;
                                        applyAmountforPartialPay = 0;
                                    }
                                    var taxRate = _itemTAXAmount / _itemTotal;
                                    itemPaymentAmt = invPayment;
                                    var taxAmount = (decimal)(invPayment * taxRate);
                                    itemTotal = invPayment - taxAmount;
                                }

                            }
                            else if (!oManualInvoice.PaidInFull && invoiceDetail.FranchiseeItems.Count() == 1)
                            {
                                if (invBalance > 0)
                                {
                                    if (invBalance <= applyAmountforPartialPay)
                                    {
                                        invPayment = (decimal)invBalance;
                                        applyAmountforPartialPay = applyAmountforPartialPay - (decimal)invBalance;
                                    }
                                    else
                                    {
                                        invPayment = applyAmountforPartialPay;
                                        applyAmountforPartialPay = 0;
                                    }
                                    var taxRate = _itemTAXAmount / _itemTotal;
                                    itemPaymentAmt = invPayment;
                                    var taxAmount = (decimal)(invPayment * taxRate);
                                    itemTotal = invPayment - taxAmount;
                                }

                            }
                            //else
                            //{
                            //    foundFields = foundFields &&
                            //                  decimal.TryParse(frm[invChunk + "_paymentAmt"],
                            //                      out itemPaymentAmt);
                            //    foundFields = foundFields &&
                            //                  decimal.TryParse(frm[invChunk + "_total"],
                            //                      out itemTotal);
                            //}

                            if (foundFields)
                            //if (foundFields && itemTotal > 0)
                            {
                                ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
                                mpvm.MasterTrxDetailId = -1;
                                mpvm.LineNo = -1;
                                mpvm.PaymentAmount = itemPaymentAmt;
                                mpvm.Tax = itemPaymentAmt - itemTotal;
                                mpvm.Total = itemPaymentAmt;
                                mpvm.ExtendedPrice = itemTotal;
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
                                bpPaymentAmt = (decimal)r.Balance + (decimal)r.BalanceFees; // pay whole balance because whole invoice was paid
                            }
                            else if (invoiceDetail.Invoice.InvoiceDetailItems.Count == 1 || invoiceDetail.FranchiseeItems.Count == 1)
                            {
                                // only one line item, so distribute paid amount (after taxes) to all franchisees proportionally
                                var totalFranchiseeBalance =
                                    invoiceDetail.FranchiseeItems.Sum(o => o.InvoiceFranchiseeDetailItem.Balance);
                                var percentage = r.Balance / totalFranchiseeBalance;
                                bpPaymentAmt = oManualInvoice.CustomerPayment.Payments[0].ExtendedPrice * (decimal)percentage;
                            }
                            //else
                            //{
                            //    foundFields = foundFields &&
                            //                  decimal.TryParse(frm[string.Format(invChunk + "_bp{0}_paymentAmt", bpId)],
                            //                      out bpPaymentAmt);
                            //}

                            if (foundFields)
                            {
                                ManualPaymentFranchiseeViewModel mpfvm = new ManualPaymentFranchiseeViewModel();
                                mpfvm.FranchiseeId = r.FranchiseeId;
                                mpfvm.BillingPayId = r.BillingPayId;

                                using (jkDatabaseEntities context = new jkDatabaseEntities())
                                {
                                    var billingPay = context.BillingPays.Where(o => o.BillingPayId == r.BillingPayId).FirstOrDefault();
                                    mpfvm.IsTurnAroundPayment = (billingPay.HasBeenChargedBack == true && billingPay.IsChargebackPaid == false) ? true : false;
                                    mpfvm.IsTARPaid = (billingPay.HasBeenChargedBack == true && billingPay.IsChargebackPaid == false) ? false : true;
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

                        //if (paymentMethodListId == 2) // credit card
                        //{
                        //    CCTransaction cCTransaction = new CCTransaction();
                        //    cCTransaction.Amount = invPayment;
                        //    cCTransaction.BatchID = notes.ToString();
                        //    cCTransaction.ClassID = Convert.ToInt32(ClassId);
                        //    cCTransaction.CreateByID = _claimView.GetCLAIM_PERSON_INFORMATION().UserId;
                        //    cCTransaction.invoiceID = invoiceDetail.Invoice.InvoiceDetail.InvoiceId;
                        //    cCTransaction.TransactionDate = DateTime.Now;
                        //    cCTransaction.TransactionID = notes.ToString();
                        //    cCTransaction.TypeID = 1;
                        //    cCTransaction.Last4CCNo = Convert.ToInt32(Last4CC);
                        //    //cCTransaction.GatewayID = PGID;

                        //    cc.Add(cCTransaction);
                        //}
                    }

                    //if (paymentMethodListId == 2) // credit card 
                    //{
                    //    GeneralService generalService = new GeneralService();
                    //    generalService.InsertCCtransaction(cc);
                    //}

                    oMainObject.Invoices = lstManualInvoices;
                    oMainObject.RegionId = _RegionId;

                    //if (oMainObject.Invoices.Count > 0) // sanity check
                    //AccountReceivableService.InsertManualPaymentTransactionInTemp(oMainObject);
                    if (oMainObject.Invoices.Count > 0) // sanity check
                        Id = AccountReceivableService.UpdatePaymentDetailPopup(Id, PaymentDate, PaymentType, PaymentNo, Amount, Note, (ischangeInvoiceData > 0 ? false : true), oMainObject);
                }
                else
                {

                    Id = AccountReceivableService.UpdatePaymentDetailPopup(Id, PaymentDate, PaymentType, PaymentNo, Amount, Note, (ischangeInvoiceData > 0 ? false : true), oMainObject);
                }



            }


            return Json(new { success = true, message = "success", PaymentTempId = Id }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CustomerCreditDetailPopup(int Id)
        {
            CustomerCreditDetailsPopupModel CustomerCreditDetailsPopupModel = new CustomerCreditDetailsPopupModel();
            CustomerCreditDetailsPopupModel = AccountReceivableService.CustomerCreditDetailPopup(Id);

            return PartialView("_PartialCustomerCreditDetailPopup", CustomerCreditDetailsPopupModel);
        }

        public ActionResult EditCreditDetailsPopup(int Id)
        {
            //CustomerCreditDetailsPopupModel CustomerCreditDetailsPopupModel = new CustomerCreditDetailsPopupModel();
            //CustomerCreditDetailsPopupModel = AccountReceivableService.CustomerCreditDetailPopup(Id);
            //return PartialView("_PartialEditCreditDetailsPopup", CustomerCreditDetailsPopupModel);
            return PartialView("_PartialEditCreditDetailsPopup");
        }


        #region Monthly Bill Run

        [HttpGet]
        public ActionResult MonthlyBillRun()
        {
            ViewBag.CurrentMenu = "AccountsReceivableInvoices";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("MonthlyBillRun", "AccountReceivable", new { area = "Portal" }),
                "Account Receivable");
            BreadCrumb.Add(Url.Action("MonthlyBillRun", "AccountReceivable", new { area = "Portal" }), "Invoices");
            BreadCrumb.Add(Url.Action("MonthlyBillRun", "AccountReceivable", new { area = "Portal" }), "Monthly Bill Run");

            //List<SelectListItem> monthsList = Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new SelectListItem
            //{
            //    Text = v.ToString(),
            //    Value = ((int)v).ToString()
            //}).ToList();

            ViewBag.billMonthsList = GetMonthsList();

            ViewBag.billYearList = GetYearsList().OrderByDescending(o => o.Value);

            //var regionlist = _commonService.GetRegionList();
            //ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        [HttpGet]
        public JsonResult GenerateMonthlyBillRun(int month, int year, string selectedRegionId = "")
        {
            selectedRegionId = selectedRegionId == "null" ? "" : selectedRegionId;
            var oBillRunSummaryDetailViewModel = AccountReceivableService.GenerateMonthlyBillRun(month, year,
                selectedRegionId);
            return Json(oBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);
            //var temp = new List<portal_spCreate_AR_MonthlyBillRunGenerateList_Result>();
            //return Json(temp, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMonthlyBillRunData(int month, int year)
        {
            var oBillRunSummaryDetailViewModel = AccountReceivableService.GetMonthlyBillRunResultData(month, year,
                SelectedRegionId.ToString());
            return Json(oBillRunSummaryDetailViewModel, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult ManualInvoiceView()
        {
            return PartialView("_ManualInvoiceView");
        }

        public ActionResult PaymentPendingList()
        {
            ViewBag.CurrentMenu = "AccountsReceivablePayment";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("PaymentList", "AccountReceivable", new { area = "Portal" }), "Accounts Receivable");
            BreadCrumb.Add(Url.Action("PaymentList", "AccountReceivable", new { area = "Portal" }), "Payment");
            BreadCrumb.Add(Url.Action("PaymentPendingList", "AccountReceivable", new { area = "Portal" }), "List");
            // ViewBag.OptionList = new SelectList(AccountReceivableService.GetAll_OptionList(), "SearchDateListId", "Name");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.RegionList = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = this.SelectedRegionId;

            return View();
        }

        public ActionResult PendingPaymentListResultData(string regionIds, DateTime from, DateTime to)
        {
            if (regionIds == "null") regionIds = null;

            try
            {
                //var response = AccountReceivableService.GetPendingPaymentList(regionIds, from, to);
                var response = AccountReceivableService.GetPendingtempPaymentList(regionIds, from, to);
                var result = from f in response
                             select new
                             {
                                 PaymentId = f.PaymentId,
                                 PaymentNo = f.TransactionNumber,
                                 CreatedDate = f.CreatedDate != null ? f.CreatedDate : null,
                                 CustomerName = f.Name,
                                 PaymentType = f.PaymentType,
                                 CheckNo = f.PaymentNo != null ? f.PaymentNo : string.Empty,
                                 InvoiceNo = f.InvoiceNo != null ? f.InvoiceNo : string.Empty,
                                 InvoiceAmount = f.InvoiceAmount != null ? String.Format("{0:C}", f.InvoiceAmount) : "$0.00",
                                 PaymentAmount = f.PaymentAmount != null ? String.Format("{0:C}", f.PaymentAmount) : "$0.00",
                                 Reference = f.PaymentDescription != null ? f.PaymentDescription : string.Empty,
                                 RegionName = f.RegionName,
                                 InvoiceId = f.InvoiceId
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



        public ActionResult PendingPaymentDetailPopup(int paymentId, bool ispending = false)
        {
            PaymentDetailsPopupModel PaymentDetailsPopupModel = new PaymentDetailsPopupModel();

            if (ispending == true)
            {

                PaymentDetailsPopupModel = AccountReceivableService.PaymentDetailPopupFromTemp(paymentId, true);
            }
            else
            {
                PaymentDetailsPopupModel = AccountReceivableService.PaymentDetailPopup(paymentId, true);
            }



            return PartialView("_PartialPendingPaymentDetailPopup", PaymentDetailsPopupModel);
        }

        [HttpPost]
        public ActionResult UpdatePendingPaymentApproveReject(int customerid, int paymentId, string Note, int Status)
        {
            if (paymentId > 0)
            {
                int i = 0;

                List<int> lstPayments = AccountReceivableService.GetPaymentIdsforApprove(paymentId);
                //foreach (int _pId in lstPayments)
                //{
                //    //AccountReceivableService.GenerateInvoice(MasterTmpTrxId, Status);
                //    //string message, int status, int MasterTmpTrxId, string EntrySource, int ClassId, int TypeListId, int MasterTrxTypeListId, int HeaderId
                //    AccountReceivableService.saveCommonPendingMessage(Note, Status, 0, "Payment", customerid, 1, 2,
                //        paymentId);
                //}

                AccountReceivableService.saveCommonPendingMessage(Note, Status, 0, "Payment", customerid, 1, 2,
                        paymentId);

            }
            return Json(new { Data = paymentId, success = true }, JsonRequestBehavior.AllowGet);
        }

        #region :: PastDue Statement Details :: 
        public ActionResult PastDueStatementDetailsPopup(int Id, DateTime? reportDate)
        {
            PastDueStatementDetailModel ModelView = new PastDueStatementDetailModel();
            ModelView = AccountReceivableService.GetPastDueStatementDetailsPopup(Id, reportDate);
            ViewBag.CustomerID = Id;
            ViewBag.reportDate = reportDate;

            ViewBag.CustBillingEmail = AccountReceivableService.GetCustomerBillingEmail(Convert.ToInt32(Id));

            return PartialView("_PastDueStatementDetailsPopup", ModelView);
        }

        public FileResult PastDueStatementDetailsExportToPDF(int Id, DateTime? reportDate)
        {
            if (Id > 0)
            {
                string HTMLContent = string.Empty;
                PastDueStatementDetailModel ModelView = new PastDueStatementDetailModel();
                ModelView = AccountReceivableService.GetPastDueStatementDetailsPopup(Id, reportDate);
                if (ModelView.RegionId > 0)
                {
                    ModelView.RemitTo = CustomerService.GetRemitToForRegion(ModelView.RegionId);
                }
                HTMLContent += RenderViewToString("_PastDueStatementDetailsExportToPDF", ModelView);
                return File(GetPDFWithHTML(HTMLContent), "application/pdf", "_PastDueStatementDetailsExportToPDF.pdf");
            }
            return null;
        }
        public byte[] GetPDFWithHTML(string pHTML)
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

        public FileResult PastDueStatementDetailsPrint(string customerIds, DateTime? reportDate)
        {
            if (customerIds != null && customerIds != "")
            {
                string contentDisposition = "inline";
                string[] Ids = customerIds.Split(',');
                if (Ids != null && Ids.Count() > 0)
                {
                    string HTMLContent = string.Empty;
                    foreach (var item in Ids)
                    {
                        PastDueStatementDetailModel ModelView = new PastDueStatementDetailModel();
                        ModelView = AccountReceivableService.GetPastDueStatementDetailsPopup(Convert.ToInt32(item), reportDate);
                        if (ModelView.RegionId > 0)
                        {
                            ModelView.RemitTo = CustomerService.GetRemitToForRegion(ModelView.RegionId);
                        }

                        HTMLContent += RenderViewToString("_PastDueStatementDetailsExportToPDF", ModelView);
                    }
                    string filename = string.Format("{0}_PastDueStatementDetailsPrint.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    Response.AddHeader("content-disposition", contentDisposition + ";filename=\"" + filename + "\"");
                    return File(GetPDFWithHTML(HTMLContent), "application/pdf");
                }
                return null;
            }
            return null;
        }

        [HttpGet]
        public JsonResult PastDueStatementSendEmailPopup(string customerIds, string FromEmail, string ToEmail, string CCEmail, string SubjectEmail, string BodyEmail, DateTime? reportDate)
        {
            string retVal = "";
            try
            {
                if (customerIds != null && customerIds != "")
                {
                    string[] Ids = customerIds.Split(',');
                    string HTMLContent = string.Empty;
                    if (Ids != null && Ids.Count() > 0)
                    {
                        foreach (var item in Ids)
                        {
                            if (Convert.ToInt32(item) > 0)
                            {
                                PastDueStatementDetailModel ModelView = new PastDueStatementDetailModel();
                                ModelView = AccountReceivableService.GetPastDueStatementDetailsPopup(Convert.ToInt32(item), reportDate);
                                if (ModelView.RegionId > 0)
                                {
                                    ModelView.RemitTo = CustomerService.GetRemitToForRegion(ModelView.RegionId);
                                }
                                HTMLContent += RenderViewToString("_PastDueStatementDetailsExportToPDF", ModelView);
                            }
                        }
                    }

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
                    mail.Attachments.Add(new Attachment(new MemoryStream(GetPDFWithHTML(HTMLContent)), "PastDueStatementReport.pdf"));
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    SmtpServer.Port = 587;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("janikingtest@gmail.com", "Test#12345");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);

                    retVal = "Success";
                }
            }
            catch (Exception ex)
            {
                retVal = ex.Message;

                //throw ex;
            }
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAdditionalBilling()
        {
            if (SelectedRegionId > 0)
            {
                var searchData = AccountReceivableService.GetAdditionalBilling(SelectedRegionId);
                if (searchData != null)
                    return Json(searchData.Value, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult OtherDepositePopup(int? Id)
        {
            List<ServiceTypeList> lstServiceTypeList = CustomerService.GetServiceTypeList().Where(x => x.IsDeposit == true).OrderBy(o => o.name).ToList();
            ViewBag.ServiceTypeList = new SelectList(lstServiceTypeList, "ServiceTypeListId", "Name");
            return PartialView("_PartialOtherDepositePopup");
        }

        public JsonResult HaveChargebackforInvoiceId(int invoiceId)
        {
            return Json(AccountReceivableService.HaveChargebackforInvoiceId(invoiceId), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ApplyOverflowPaymentDetailPP(int? id, int OId = 0, decimal OAmount = 0)
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


            OverPaymentCustomerInvoiceViewModel OverPaymentCustomerInvoice = new OverPaymentCustomerInvoiceViewModel();
            if (id != null && id != 0)
            {
                OverPaymentCustomerInvoice = AccountReceivableService.GetOverPaymentCustomerInvoiceDetail(id ?? 00, OId, OAmount);
            }

            return PartialView("_ApplyOverflowPaymentDetailPP", OverPaymentCustomerInvoice);
        }

        [HttpPost]
        public ActionResult ApplyOverflowPayment(FormCollection frm)
        {
            int _OInvoiceId = !String.IsNullOrEmpty(frm["hdfOPI_InvoiceId"]) ? int.Parse(frm["hdfOPI_InvoiceId"].ToString()) : 0;
            string _OInvoiceNo = !String.IsNullOrEmpty(frm["hdfOPI_InvoiceNo"]) ? frm["hdfOPI_InvoiceNo"].ToString() : "";
            int ClassId = !String.IsNullOrEmpty(frm["hdfOPI_CustomerId"]) ? int.Parse(frm["hdfOPI_CustomerId"].ToString()) : -1;
            string notes = "Move From " + _OInvoiceNo;
            DateTime paymentDate = !String.IsNullOrEmpty(frm["paymentDateMP"]) ? DateTime.Parse(frm["paymentDateMP"]) : DateTime.Now;
            decimal paymentAmt = !String.IsNullOrEmpty(frm["OPI_IBalanceAmount"]) ? Decimal.Parse(frm["OPI_IBalanceAmount"].ToString().Replace("$", "").Replace(",", "")) : 0.00M;
            decimal creditAmt = 0.00M;
            decimal balance = !String.IsNullOrEmpty(frm["OPI_IBalanceAmountBAL"]) ? Decimal.Parse(frm["OPI_IBalanceAmountBAL"]) : 0.00M;

            var hdMPCallFrom = !String.IsNullOrEmpty(frm["hdMPCallFrom"]) ? frm["hdMPCallFrom"].ToString() : "";
            var paymentMethodListId = !String.IsNullOrEmpty(frm["slPaymentTypeMP"]) ? int.Parse(frm["slPaymentTypeMP"]) : 0;
            var referenceNo = frm["referenceNoMP"];

            bool mp_chkApplyCredit = !String.IsNullOrEmpty(frm["mp_chkApplyCredit"]) ? bool.Parse(frm["mp_chkApplyCredit"]) : false;


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
                oMainObject.OInvoiceId = _OInvoiceId;
                oMainObject.OInvoiceNo = _OInvoiceNo;


                List<MPInvoiceViewModel> lstManualInvoices = new List<MPInvoiceViewModel>();
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
                    lstManualInvoices.Add(oManualInvoice);
                }

                oMainObject.Invoices = lstManualInvoices;
                oMainObject.RegionId = _RegionId;
                if (oMainObject.Invoices.Count > 0) // sanity check
                    AccountReceivableService.InsertApplyOverflowPaymentTransaction(oMainObject);

            }

            return RedirectToAction("OverPaymentReport", "AccountReceivable", new { area = "Portal" });

        }



        //#region --Payment--

        //[HttpGet]
        //public ActionResult PartialManualPaymentPP(int? cid=0)
        //{
        //    CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
        //    if (cid != null && cid != 0)
        //    {
        //        var response = jkEntityModel.portal_spGet_CustomerDetail(cid).ToList();
        //        foreach (var customer in response)
        //        {
        //            customerDetailViewModel = new CustomerDetailViewModel();
        //            var cust = CustomerService.GetCustomerById(cid ?? 00);
        //            customerDetailViewModel.CustomerName = String.IsNullOrEmpty(customer.CustomerName.ToString()) ? String.Empty : customer.CustomerName.ToString();
        //            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(customer.CustomerNo.ToString()) ? String.Empty : customer.CustomerNo.ToString();
        //            customerDetailViewModel.CustomerId = String.IsNullOrEmpty(customer.CustomerId.ToString()) ? String.Empty : customer.CustomerId.ToString();
        //            customerDetailViewModel.Account_Type = String.IsNullOrEmpty(customer.AccountType.ToString()) ? String.Empty : customer.AccountType.ToString();
        //            customerDetailViewModel.ContactName = String.IsNullOrEmpty(customer.ContactName.ToString()) ? String.Empty : customer.ContactName.ToString();
        //            customerDetailViewModel.Phone = String.IsNullOrEmpty(customer.Phone.ToString()) ? String.Empty : customer.Phone.ToString();
        //            customerDetailViewModel.RegionId = String.IsNullOrEmpty(cust.RegionId.ToString()) ? String.Empty : cust.RegionId.ToString();
        //            customerDetailViewModel.Balance = AccountReceivableService.GetCustomerCreditBalance(Convert.ToInt32(customer.CustomerId));
        //        }
        //    }
        //    ViewBag.CustomerDetail = customerDetailViewModel;
        //    ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);

        //    return PartialView("_PartialManualPaymentPP");
        //}

        //[HttpPost]
        //public ActionResult PartialManualPaymentPP(FormCollection collection)
        //{
        //    //CustomerDetailViewModel customerDetailViewModel = new CustomerDetailViewModel();
        //    //if (cid != null && cid != 0)
        //    //{
        //    //    var response = jkEntityModel.portal_spGet_CustomerDetail(cid).ToList();
        //    //    foreach (var customer in response)
        //    //    {
        //    //        customerDetailViewModel = new CustomerDetailViewModel();
        //    //        var cust = CustomerService.GetCustomerById(cid ?? 00);
        //    //        customerDetailViewModel.CustomerName = String.IsNullOrEmpty(customer.CustomerName.ToString()) ? String.Empty : customer.CustomerName.ToString();
        //    //        customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(customer.CustomerNo.ToString()) ? String.Empty : customer.CustomerNo.ToString();
        //    //        customerDetailViewModel.CustomerId = String.IsNullOrEmpty(customer.CustomerId.ToString()) ? String.Empty : customer.CustomerId.ToString();
        //    //        customerDetailViewModel.Account_Type = String.IsNullOrEmpty(customer.AccountType.ToString()) ? String.Empty : customer.AccountType.ToString();
        //    //        customerDetailViewModel.ContactName = String.IsNullOrEmpty(customer.ContactName.ToString()) ? String.Empty : customer.ContactName.ToString();
        //    //        customerDetailViewModel.Phone = String.IsNullOrEmpty(customer.Phone.ToString()) ? String.Empty : customer.Phone.ToString();
        //    //        customerDetailViewModel.RegionId = String.IsNullOrEmpty(cust.RegionId.ToString()) ? String.Empty : cust.RegionId.ToString();
        //    //        customerDetailViewModel.Balance = AccountReceivableService.GetCustomerCreditBalance(Convert.ToInt32(customer.CustomerId));
        //    //    }
        //    //}
        //    //ViewBag.CustomerDetail = customerDetailViewModel;
        //    //ViewBag.OptionList = new SelectList(CustomerService.GetAll_OptionList(), "SearchDateListId", "Name", 3);
        //    return PartialView("_PartialManualPaymentPP");
        //}

        //#endregion




        public bool PostPayment2GLCHECK()
        {
            return true;
            //AccountReceivableService.PostPayment2GeneralLedgerTrx_Checkbook(0);
        }

        [HttpGet]
        public JsonResult InvoiceListdataExportExcel(int month = 0, int year = 0, string searchtext = "", string pe = "", string oc = "", string d = "", string r = "", string consolidated = "", int typeid = 0)
        {

            string _RegionId = r == "null" ? "" : r;
            string _ToDate = "";
            string _FromDate = "";
            if (!string.IsNullOrEmpty(d) && d.Contains("-") && d != "-")
            {
                _FromDate = d.Split('-')[0];
                _ToDate = d.Split('-')[1];
            }
            else if (month > 0 && year > 0)
            {
                var first = new DateTime(year, month, 1);
                var last = first.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy");
                _FromDate = first.ToString("MM/dd/yyyy");
                _ToDate = last;
            }

            List<InvoiceListViewModel> invoiceList = AccountReceivableService.GetInvoiceList(_RegionId, _FromDate, _ToDate);

            if (!string.IsNullOrWhiteSpace(consolidated))
                invoiceList = invoiceList.Where(o => o.ConsolidatedInvoice == (consolidated == "Y" ? true : o.ConsolidatedInvoice)).ToList();

            if (typeid != 0)
            {
                if (typeid == 3)
                {
                    invoiceList = invoiceList.Where(o => o.MasterTrxTypeListId == 3).ToList();
                }
                else if (typeid == 1)
                {
                    invoiceList = invoiceList.Where(o => o.MasterTrxTypeListId != 3).ToList();
                }
            }

            decimal _InvoiceTotAmount = (decimal)invoiceList.Sum(i => i.InvoiceTotal);
            decimal _InvoiceCloseAmount = (decimal)invoiceList.Where(p => p.TransactionStatusListId == 6).Sum(i => i.InvoiceTotal);
            decimal _InvoiceOpenAmount = (decimal)invoiceList.Where(p => p.TransactionStatusListId != 6).Sum(i => i.InvoiceTotal);
            decimal _InvoiceOverdueAmount = (decimal)invoiceList.Where(o => o.DueDate < DateTime.UtcNow.Date && (o.TransactionStatusListId == 4 || o.TransactionStatusListId == 7)).Sum(o => o.InvoiceTotal);

            int _InvoiceTotCount = invoiceList.Count();
            int _InvoiceCloseCount = invoiceList.Where(p => p.TransactionStatusListId == 6).Count();
            int _InvoiceOpenCount = invoiceList.Where(p => p.TransactionStatusListId != 6).Count();
            int _InvoiceOverdueCount = invoiceList.Where(o => o.DueDate < DateTime.UtcNow.Date && (o.TransactionStatusListId == 4 || o.TransactionStatusListId == 7)).Count();

            if (!string.IsNullOrWhiteSpace(pe) && pe != "E" && pe.Length == 1)
                invoiceList = invoiceList.Where(o => o.EBill == true).ToList();
            else if (!string.IsNullOrWhiteSpace(pe) && pe != "P" && pe.Length == 1)
                invoiceList = invoiceList.Where(o => o.PrintInvoice == true).ToList();

            if (!string.IsNullOrWhiteSpace(oc) && oc.Length > 1)
                invoiceList = invoiceList.Where(o => o.IsOpen == "Y" || o.IsOpen == "N").ToList();
            else if (!string.IsNullOrWhiteSpace(oc))
                invoiceList = invoiceList.Where(o => o.IsOpen == oc).ToList();

            decimal _FTInvoiceAmount = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceAmount) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceAmount));
            decimal _FTInvoiceTax = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceTax) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceTax));
            decimal _FTInvoiceTotal = ((decimal)invoiceList.Where(w => w.MasterTrxTypeListId != 3).Sum(i => i.InvoiceTotal) - (decimal)invoiceList.Where(w => w.MasterTrxTypeListId == 3).Sum(i => i.InvoiceTotal));


            string rowText = string.Empty;
            if (invoiceList != null && invoiceList.Count() > 0)
            {
                rowText += "<table border='1'>";
                rowText += "<tr>";
                rowText += "<td style='text-align:center'><b>Region<b></td>";
                rowText += "<td style='text-align:center'><b>Invoice No</b></td>";
                rowText += "<td style='text-align:center'><b>Invoice Date</b></td>";
                rowText += "<td style='text-align:center'><b>Customer No</b></td>";
                rowText += "<td style='text-align:center'><b>Customer Name</b></td>";
                rowText += "<td style='text-align:center'><b>E/P</b></td>";
                rowText += "<td style='text-align:center'><b>C</b></td>";
                rowText += "<td style='text-align:center'><b>Description</b></td>";
                rowText += "<td style='text-align:center'>CPI</td>";
                rowText += "<td style='text-align:center'>Invoice Amount</td>";
                rowText += "<td style='text-align:center'>Invoice Tax</td>";
                rowText += "<td style='text-align:center'>Invoice Total</td>";
                rowText += "<td style='text-align:center'>Status</td>";
                rowText += "</tr>";
                foreach (var item in invoiceList.OrderByDescending(o => o.InvoiceDate))
                {
                    rowText += "<tr>";
                    rowText += "<td>" + item.RegionName + "</td>";
                    rowText += "<td>" + item.InvoiceNo + "</td>";
                    rowText += "<td>" + (item.InvoiceDate != null ? Convert.ToDateTime(item.InvoiceDate).ToString("MM/dd/yyyy") : string.Empty) + "</td>";
                    rowText += "<td>" + item.CustomerNo + "</td>";
                    rowText += "<td>" + item.CustomerName + "</td>";
                    rowText += "<td>" + item.EBillText + " " + item.PrintInvoiceText + "</td>";
                    rowText += "<td>" + (item.ConsolidatedInvoice == true ? "" : string.Empty) + "</td>";
                    rowText += "<td>" + item.InvoiceDescription + "</td>";
                    rowText += "<td>" + (item.CPI == 0 ? "" : item.CPI + "%") + "</td>";
                    if (item.MasterTrxTypeListId == 3)
                    {
                        rowText += "<td style='background - color:#f8c471;'>(" + item.InvoiceAmount + ")</td>";
                        rowText += "<td style='background - color:#f8c471;'>(" + item.InvoiceTax + ")</td>";
                        rowText += "<td style='background - color:#f8c471;'>(" + item.InvoiceTotal + ")</td>";
                    }
                    else
                    {
                        rowText += "<td>" + item.InvoiceAmount + "</td>";
                        rowText += "<td>" + item.InvoiceTax + "</td>";
                        rowText += "<td>" + item.InvoiceTotal + "</td>";
                    }
                    rowText += "<td>" + item.TransactionStatus + "</td>";
                    rowText += "</tr>";

                }
                rowText += "</table>";
            }
            //return Json(rowText.ToString(), JsonRequestBehavior.AllowGet);
            var jsonResult = Json(rowText.ToString(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


    }
}