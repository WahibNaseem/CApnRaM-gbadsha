using Application.Web.Core;
using JK.FMS.MVC.Areas.CRM.Common;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    public class CRMRegionAccountingController : ViewControllerBase
    {
        public CRMRegionAccountingController(ICacheProvider cacheProivder, ICustomerService customerservice, ICommonService commonService)
        {
            _cacheProvider = cacheProivder;
            CustomerService = customerservice;
            _commonService = commonService;
        }
        // GET: CRM/CRMRegionAccounting
        public ActionResult Index(int id = -1)
        {
            CustomerSearchResultViewModelListModel model = new CustomerSearchResultViewModelListModel();
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Customer";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMCustomer", new { area = "CRM" }), "Customer");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
       
            ViewBag.CustomerId = id;
            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }
        [HttpGet]
        public ActionResult CustomerRegionAccountingList(string rgId)
        {
            try
            {
                var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var customers = CustomerService.GetCustomerSearchList(contactTypeList, "39,35", rgId);
                var result = (from f in customers
                              select new
                              {
                                  f.CustomerId,
                                  f.CustomerNo,
                                  f.CustomerName,
                                  f.Address,
                                  f.StateName,
                                  f.City,
                                  f.PostalCode,
                                  //CustomerName = "CustomerName",
                                  //Address= "Address",
                                  //StateName= "StateName",
                                  //City= "City",
                                  //PostalCode= "PostalCode",
                                  Amount = string.Format("{0:c}", f.Amount),
                                  Phone = f.Phone != null ? CRMUtils.FormatUsPhoneNumber(f.Phone) : string.Empty,
                                  f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  CreatedBy = f.CreatedBy
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
        public ActionResult FranchiseeOffering()
        {
            return View();
        }

        public ActionResult OfferingList()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Customer";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("OfferingList", "CRMDashboard", new { area = "CRM" }), "CRM");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("OfferingList", "CRMCustomer", new { area = "CRM" }), "Offering List");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Offering);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", "19,40,41");


            ViewBag.selectedRegionId = SelectedRegionId;
            return View();
        }

        //[HttpGet]
        //public ActionResult CustomerDetailPopup(int id = -1)
        //{
        //    int? CustomerID = id;
        //    ViewBag.CustomerID = id;
        //    FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1(); ;
        //    if (id > 0)
        //    {
        //        var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);
        //        List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
        //        MessageData = _commonService.GetDashboardPendingData(null).Where(r => r.CustomerID == id).OrderBy(r => r.MessageDate).ToList<PendingDashboardDataModel>();
        //        MaintenanceTemp oMaintenanceTemp = jkEntityModel.MaintenanceTemps.Where(o => o.ClassId == CustomerID && o.TypeListId == 1 && o.MaintenanceTypeListId == 7).ToList().FirstOrDefault();

        //        CustomerDetailViewModel customerDetailViewModel = null;
        //        foreach (var a in response.ToList())
        //        {
        //            customerDetailViewModel = new CustomerDetailViewModel();

        //            customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
        //            customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
        //            customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
        //            customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
        //            customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
        //            customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2.ToString()) ? String.Empty : a.Address2.ToString();

        //            if (a.Phone != null)
        //            {
        //                customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
        //            }
        //            //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
        //            //{
        //            //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
        //            //}

        //            if (a.Fax != null)
        //            {
        //                customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
        //            }
        //            //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
        //            //{
        //            //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
        //            //}
        //            if (a.EmailAddress != null)
        //            {
        //                customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
        //            }
        //            if (a.ContactName != null)
        //            {
        //                customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
        //            }
        //            if (a.ContactTitle != null)
        //            {
        //                customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
        //            }
        //            if (a.Cell != null)
        //            {
        //                customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
        //            }
        //            if (a.EmailAddress != null)
        //            {
        //                customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
        //            }
        //            if (a.Ext != null)
        //            {
        //                customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
        //            }
        //            // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());
        //            if (a.Amount != null)
        //            {
        //                if (a.Amount.ToString().Trim().Length > 0)
        //                {
        //                    customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
        //                }
        //                else
        //                {
        //                    customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
        //                }
        //            }
        //            else
        //            {
        //                customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
        //            }
        //        }
        //        if (oMaintenanceTemp != null)
        //        {
        //            FullCustomerViewModel.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
        //        }
        //        FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
        //        FullCustomerViewModel.MessagesData = MessageData;
        //        FullCustomerViewModel.USERID = int.Parse(_claimView.GetCLAIM_USERID());

        //        var ValidationItem = CustomerService.ValidationItemListStatus((int)JKApi.Business.Enumeration.CustomerStatusList.RegionAccounting, (int)JKApi.Business.Enumeration.TypeList.Customer);
        //        ViewBag.ValidationItem = ValidationItem;
        //    }
        //    return PartialView("_CustomerDetailPopup", FullCustomerViewModel);
        //}

    }
}