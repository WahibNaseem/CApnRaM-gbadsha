using Application.Web.Core;
using JKApi.Service;
using JKApi.Service.ServiceContract.Management;
using JKViewModels;
using JKViewModels.Common;
using Microsoft.Owin.Security;
using MvcBreadCrumbs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{

    [BreadCrumb(Clear = true, Label = "Portal", Order = 0)]
    public class DashBoardController : ViewControllerBase
    {
        public DashBoardController(ICommonService commonService, IManagementService managementService)
        {
            this._commonService = commonService;
            ManagementService = managementService;
            ViewBag.HMenu = "DashBoard";
        }

        [Filter.RoleBasedAuthorize]
        // GET: Portal/DashBoard
        public ActionResult Index()
        {
            BreadCrumb.Add(Url.Action("Index", "DashBoard", new { area = "Portal" }), "Dash Board");
            DashboardViewModel model = new DashboardViewModel();
            model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
            model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
            //int year = DateTime.Now.Year;
            //DateTime fromDate = new DateTime(year, 1, 1);
            //DateTime toDate = DateTime.Now;

            //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            return View(model);
        }
        public ActionResult GetDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetDashboardData(spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDashboardRevenueChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetDashboardChartData(spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDashboardMonthlyRevenueChartData()
        {
            var data = _commonService.GetDashboardMonthRevenueDataChartData();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDashboardAccountTypeWiseChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var data = _commonService.GetDashboardAccountTypeWiseChartData(spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region chart details report.......
        public ActionResult GetBillingAccountBreakdownBySizeData(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountBreakdownBySizeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingAccountBreakdownBySizeDataView(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountBreakdownBySizeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return PartialView("_BillingAccountBreakdownBySizeData", data);
        }

        public ActionResult GetBillingAccountBreakdownByContractData(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountBreakdownByContractData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBillingAccountBreakdownByContractDataView(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountBreakdownByContractData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return PartialView("_BillingAccountBreakdownByContractDataView", data);
        }
        public ActionResult GetBillingAccountRevenueByAccountTypeData(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountRevenueByAccountTypeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingAccountRevenueByAccountTypeDataView(string flagId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            var regionIds = string.Empty;
            if (SelectedRegionId > 0)
            {
                regionIds = SelectedRegionId.ToString();
            }
            var data = ManagementService.GetBillingAccountRevenueByAccountTypeData(flagId, regionIds, spnStartDate, spnEndDate, billMonth, billYear);
            return PartialView("_BillingAccountRevenueByAccountTypeDataView", data);
        }

        #endregion


        public ActionResult RegionSelect(int RegionId)
        {
            var userViewModel = _claimView.GetCLAIM_PERSON_INFORMATION();
            if (userViewModel != null && userViewModel.Regions != null)
            {
                var selectedCompanyDetail = userViewModel.Regions.Where(x => x.RegionId == RegionId).FirstOrDefault();

                if (selectedCompanyDetail != null)
                {
                    Session["SelectedRegionId"] = RegionId;
                    if (userViewModel.lstPeriodAccess.Where(r => r.RegionId == Convert.ToInt32(Session["SelectedRegionId"])).Count() > 0)
                        Session["SelectedPeriodId"] = userViewModel.lstPeriodAccess.Where(r => r.RegionId == SelectedRegionId).FirstOrDefault()?.PeriodId;
                    else
                        Session["SelectedPeriodId"] = 0;

                    #region OLD Login Code due to some issue commenting this code
                    //var newIdentity = new ClaimsIdentity(User.Identity);


                    //if (newIdentity.Claims.Where(x => x.Type == ClaimTypeName.CLAIM_SELECTED_COMPANY_ID).Any())
                    //{
                    //    newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID));
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(RegionId)));
                    //}
                    //else
                    //{
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(RegionId)));
                    //}

                    //if (newIdentity.Claims.Where(x => x.Type == ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS).Any())
                    //{
                    //    newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS));
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(selectedCompanyDetail)));
                    //}
                    //else
                    //{
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(selectedCompanyDetail)));
                    //}

                    ////Regenerate user identity
                    //HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(newIdentity), new AuthenticationProperties() { IsPersistent = false });

                    #endregion

                    List<JKViewModels.RoleAccessModel> AllMenus = null;

                    if (userViewModel != null && userViewModel.RoleAccesss != null)
                    {
                        AllMenus = userViewModel.RoleAccesss;

                        var menu = AllMenus.Where(x => x.ParentMenuId == 0 && x.IsViewAccess == true).OrderBy(x => x.MenuOrder).ToList();
                        if (menu != null && menu.Count > 0)
                        {
                            return Redirect(menu[0].MenuUrl);
                        }
                    }
                }
            }
            return RedirectToAction("LogOff", "User", new { area = "JKControl" });
        }

        //[Filter.RoleBasedAuthorize]
        public ActionResult Region()
        {
            UserViewModel userViewModel = _claimView.GetCLAIM_PERSON_INFORMATION();
            if (userViewModel == null || userViewModel.Regions == null)
            {
                return RedirectToAction("LogOff", "User", new { area = "JKControl" });
            }
            else if (userViewModel.Regions.Count == 1)
            {
                if (userViewModel != null && userViewModel.Regions != null)
                {
                    var selectedCompanyDetail = userViewModel.Regions.FirstOrDefault();

                    if (selectedCompanyDetail != null)
                    {
                        Session["SelectedRegionId"] = userViewModel.Regions.FirstOrDefault().RegionId;

                        #region OLD Login Code due to some issue commenting this code
                        //var newIdentity = new ClaimsIdentity(User.Identity);


                        //if (newIdentity.Claims.Where(x => x.Type == ClaimTypeName.CLAIM_SELECTED_COMPANY_ID).Any())
                        //{
                        //    newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID));
                        //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(RegionId)));
                        //}
                        //else
                        //{
                        //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(RegionId)));
                        //}

                        //if (newIdentity.Claims.Where(x => x.Type == ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS).Any())
                        //{
                        //    newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS));
                        //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(selectedCompanyDetail)));
                        //}
                        //else
                        //{
                        //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(selectedCompanyDetail)));
                        //}

                        ////Regenerate user identity
                        //HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(newIdentity), new AuthenticationProperties() { IsPersistent = false });

                        #endregion

                        List<JKViewModels.RoleAccessModel> AllMenus = null;

                        if (userViewModel != null && userViewModel.RoleAccesss != null)
                        {
                            AllMenus = userViewModel.RoleAccesss;

                            var menu = AllMenus.Where(x => x.ParentMenuId == 0 && x.IsViewAccess == true).OrderBy(x => x.MenuOrder).ToList();
                            if (menu != null && menu.Count > 0)
                            {
                                return Redirect(menu[0].MenuUrl);
                            }
                        }
                    }
                }
            }

            //BreadCrumb.Add(Url.Action("Index", "DashBoard", new { area = "Portal" }), "Dash Board");
            return View(userViewModel);
        }


        public JsonResult SelectPeriod(int Id)
        {
            var userViewModel = _claimView.GetCLAIM_PERSON_INFORMATION();
            if (userViewModel != null && userViewModel.Regions != null)
            {
                var PeriodIdDetail = userViewModel.lstPeriodAccess.Where(x => x.PeriodId == Id).FirstOrDefault();

                if (PeriodIdDetail != null)
                {

                    Session["SelectedPeriodId"] = Id;

                    //var newIdentity = new ClaimsIdentity(User.Identity);


                    //if (newIdentity.Claims.Where(x => x.Type == ClaimTypeName.CLAIM_SELECTED_PERIOD_ID).Any())
                    //{
                    //    newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_PERIOD_ID));
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_PERIOD_ID, Convert.ToString(Id)));
                    //}
                    //else
                    //{
                    //    newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(Id)));
                    //}

                    ////Regenerate user identity
                    //HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(newIdentity), new AuthenticationProperties() { IsPersistent = false });


                    return Json(true, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}