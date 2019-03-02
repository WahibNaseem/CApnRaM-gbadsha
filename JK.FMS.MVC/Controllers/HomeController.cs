using Application.Web.Core;
using JKApi.Service;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKViewModels.Common;

namespace JK.FMS.MVC.Controllers
{
    public class HomeController : ViewControllerBase
    {
        public HomeController(ICommonService commonService, ICustomerService customerService, ICRM_Service ICRMService)
        {
            CustomerService = customerService;
            _commonService = commonService;
            _crmService = ICRMService;
        }
        public ActionResult Index()
        {
            return RedirectToActionPermanent("Index", "User", new { area = "JKControl" });
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult HView(string MenuName = "")
        {
            DashboardViewModel model = new DashboardViewModel();

            int year = DateTime.Now.Year;
            DateTime fromDate = new DateTime(year, 1, 1);
            DateTime toDate = DateTime.Now;

            if (HMenu.AccountReceivable.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);

            }
            else if (HMenu.AccountsPayable.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.Administration.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.BIAdministration.ToString() == MenuName)
            { }
            else if (HMenu.BIDashboard.ToString() == MenuName)
            { }
            else if (HMenu.BIManagement.ToString() == MenuName)
            { }
            else if (HMenu.Company.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.CRMAdministration.ToString() == MenuName)
            { }
            else if (HMenu.CRMDashboard.ToString() == MenuName)
            {
               model.crmdashboardModel = _commonService.GetCRMDashBoard();
            }
            else if (HMenu.CRMLeadFranchise.ToString() == MenuName)
            { }
            else if (HMenu.Customer.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.CustomerService.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                model.CustomerServiceQuickActionModel = CustomerService.GetCustomerServiceQuickAction(SelectedRegionId);
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.CustomerSales.ToString() == MenuName)
            {
                var _dt = _crmService.GetAll_CRM_PurposeType();
                if (_dt != null && _dt.Count() > 0)
                {
                    ViewBag.IC_Code =_dt.Where(w => w.CRM_PurposeTypeId == 6).FirstOrDefault().ColorCode;
                    ViewBag.FVP_Code = _dt.Where(w => w.CRM_PurposeTypeId == 1).FirstOrDefault().ColorCode;
                    ViewBag.BD_Code = _dt.Where(w => w.CRM_PurposeTypeId == 3).FirstOrDefault().ColorCode;
                    ViewBag.PD_Code = _dt.Where(w => w.CRM_PurposeTypeId == 2).FirstOrDefault().ColorCode;
                    ViewBag.FLP_Code = _dt.Where(w => w.CRM_PurposeTypeId == 9).FirstOrDefault().ColorCode;
                    ViewBag.SD_Code = _dt.Where(w => w.CRM_PurposeTypeId == 11).FirstOrDefault().ColorCode;
                    ViewBag.CL_Code = _dt.Where(w => w.CRM_PurposeTypeId == 12).FirstOrDefault().ColorCode;
                }                
                model.crmdashboardModel = _commonService.GetCRMDashBoard();

            }
            else if (HMenu.Dashboard.ToString() == MenuName || MenuName == "Home")
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //var dashboardData = _commonService.GetDashboardData(fromDate, toDate);
                //TempData["DashboardData"] = dashboardData;
                //model.DashboardModelForBlock = dashboardData;
            }
            else if (HMenu.Franchise.ToString() == MenuName)
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.Management.ToString() == MenuName)
            {
                //model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                //model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            else if (HMenu.CRMSchedules.ToString() == MenuName)
            { }
            else
            {
                model.dashboardModel.lstQuickLinks = _commonService.GetDashboardQuickLinks();
                model.dashboardModel.lstPendingData = _commonService.GetDashboardPendingData(int.Parse(_claimView.GetCLAIM_USERID()));
                //model.DashboardModelForBlock = _commonService.GetDashboardData(fromDate, toDate);
            }
            
            if (!string.IsNullOrWhiteSpace(MenuName))
            {
                ViewBag.HMenu = MenuName;
            }
            
            ViewBag.IsBreadCrumb = true;

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            return View(model);
        }

    }
}