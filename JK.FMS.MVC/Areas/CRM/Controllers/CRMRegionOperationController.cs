using Application.Web.Core;
using JKApi.Core;
using JKApi.Service;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    public class CRMRegionOperationController : ViewControllerBase
    {
       
        public CRMRegionOperationController(ICacheProvider cacheProivder,  ICustomerService customerservice, ICommonService commonService, JKApi.Service.ServiceContract.Franchisee.IFranchiseeService franchiseeService)
        {
            _cacheProvider = cacheProivder; 
            CustomerService = customerservice;
            FranchiseeService = franchiseeService;
            _commonService = commonService;
        }
        // GET: CRM/CRMRegionOperation
        [HttpGet]
        public ActionResult Index(int id= 0)
        {
            CustomerSearchResultViewModelListModel model = new CustomerSearchResultViewModelListModel();
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Customer";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMCustomer", new { area = "CRM" }), "Customer");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;
            ViewBag.CustomerId = id;

            return View();
        }
        public ActionResult ContractSold()
        {
            return View();
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

        public JsonResult OfferingListData(string status, string rgId)
        {
            var result = CustomerService.GetAccountOfferingData(rgId, status);
            
            var jsonResult = Json(new { aadata = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult CustomerOfferingPopup(int CustomerId,int FranchiseeId)
        {
            ViewBag.CustomerId = CustomerId;
            ViewBag.FranchiseeId = FranchiseeId;

            var CustData = CustomerService.GetCustomerById(CustomerId);
            string CustomerNo = string.Empty;
            string CustomerName = string.Empty;
            decimal ContractAmount = 0;
            string StrDate = string.Empty;

            if (CustData != null)
            {
                CustomerNo = CustData.CustomerNo;
                CustomerName = CustData.Name;
            }
            ViewBag.CustomerNo = CustData.CustomerNo;
            ViewBag.CustomerName = CustData.Name;
           
            var ContractData = CustomerService.GetContractByCustomerId(Convert.ToInt32(CustomerId));
            if (ContractData != null)
            {
                ContractAmount = (ContractData.Amount.HasValue ? ContractData.Amount.Value : 0);
                StrDate = (ContractData.SignDate.HasValue ? ContractData.SignDate.Value.ToString("MM-dd-yyyy") : string.Empty);
            }
            ViewBag.ContractAmount = ContractAmount;
            ViewBag.ContractDate = StrDate;
            
            var FranData= FranchiseeService.GetFranchiseeDetail(FranchiseeId);
            if (FranData != null)
            {
                ViewBag.FranchiseeModel = FranData;
            }
             
            return PartialView("_ManualEnterOfferPopup");            
        }
        public ActionResult CustomerOfferSaveDetail(int CustomerId,int FranchiseeId,string Note,string ContractAmount,string ExpDate, string ExpTime)
        {
            //check record exits
            if (CustomerId != 0 && FranchiseeId != 0)
            {
                if (CustomerService.CheckCustomerOfferingExits(CustomerId, FranchiseeId))
                {
                    CustomerService.InsertOfferingData(CustomerId, FranchiseeId.ToString(), Convert.ToDateTime(ExpDate), Convert.ToDateTime(ExpTime), Note);
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json("-1", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManualOffringStatusChangePopup(int Id)
        {
            ViewBag.OfferingId = Id;
            var DataModel = CustomerService.GetOfferingsById(Id);
            ViewBag.DataModel = DataModel;


            var DeclineReasonList = FranchiseeService.GetDeclineReasonListList().ToList();
            ViewBag.DeclineReasonList = new SelectList(DeclineReasonList, "DeclineReasonListId", "Name", (DataModel.DeclineReasonListId != null ? DataModel.DeclineReasonListId:0));

            return PartialView("_ManualOffringStatusChangePopup", DataModel);
        }
        public ActionResult CustomerOfferAcceptedStatusSave(int OfferingId,string ResponseDate,string ResponseTime,string ResponseName,string ReasonNote)
        {
            if (OfferingId > 0)
            {
                CustomerService.CustomerOfferAcceptedStatusSave(OfferingId, Convert.ToDateTime(ResponseDate + " " + ResponseTime), ResponseName, ReasonNote);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerOfferDeclineStatusSave(int OfferingId, int DeclineReasonListId, string DeclineReasonNote)
        {
            if (OfferingId > 0)
            {
                CustomerService.CustomerOfferDeclineStatusSave(OfferingId, DeclineReasonListId, DeclineReasonNote);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }
    }
}