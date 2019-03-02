using Application.Web.Core;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using JKApi.Core.Common;
using JKApi.Service;
using JKApi.Service.Service;
using JKApi.Service.Service.Management;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Management;
using JKViewModels;
using JKViewModels.Commission;
using JKViewModels.Management;
using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKViewModels.Common;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    public class CommissionController : ViewControllerBase
    {
        #region Constructor
        ManagementService mngser;

        FranchiseService serv = new FranchiseService();


        public CommissionController(IManagementService managementService, ICommonService commonService, ICustomerService customerService)
        {
            CustomerService = customerService;
            ManagementService = managementService;
            this._commonService = commonService;
            ViewBag.HMenu = "Commission";
        }

        #endregion


        // GET: Portal/Commission
        public ActionResult Index()
        {
            return View();
        }


















        #region CommissionCompensationSchedule
        public ActionResult CommissionCompensationSchedule()
        {
            ViewBag.CurrentMenu = "CommissionCompensationSchedule";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("CommissionCompensationSchedule", "Commission", new { area = "Portal" }), "Commission Compensation Schedule");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");

            ViewBag.compensationTypeList = new SelectList(ManagementService.GetCompensationTypeListData(SelectedRegionId), "CompensationTypeListId", "Description", "");
            ViewBag.CommissionPaymentScheduleList = new SelectList(ManagementService.GetCommissionPaymentPlanData(SelectedRegionId), "CommissionPaymentScheduleId", "Description", "");
            return View();
        }


        public JsonResult GetCommissionCompensationScheduleData()
        {
            var Item = ManagementService.GetCommissionCompensationScheduleData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCommissionCompensationScheduleData(int CompensationScheduleId)
        {
            var Item = ManagementService.DeleteCommissionCompensationScheduleData(CompensationScheduleId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertUpdateCommissionCompensationScheduleData(int CommissionCompensationScheduleId, string Description, int StatusListId, int CommissionPaymentScheduleId,
            int CompensationTypeListId, decimal CompAmount, decimal RangeEnd, decimal RangeStart, bool IsActive, int CompensationAmountTypeId)
        {
            CommissionCompensationScheduleViewModel oCommissionCompensationSchedule = new CommissionCompensationScheduleViewModel();
            oCommissionCompensationSchedule.CommissionCompensationScheduleId = CommissionCompensationScheduleId;
            oCommissionCompensationSchedule.Description = Description;
            oCommissionCompensationSchedule.StatusListId = StatusListId;
            oCommissionCompensationSchedule.CommissionPaymentScheduleId = CommissionPaymentScheduleId;
            oCommissionCompensationSchedule.CompensationAmount = CompAmount;
            oCommissionCompensationSchedule.CompensationTypeListId = CompensationTypeListId;
            oCommissionCompensationSchedule.CreatedBy = LoginUserId;
            oCommissionCompensationSchedule.CreatedDate = DateTime.Now;
            oCommissionCompensationSchedule.IsActive = true;
            oCommissionCompensationSchedule.RangeEndAmount = RangeEnd;
            oCommissionCompensationSchedule.RangeStartAmount = RangeStart;
            oCommissionCompensationSchedule.RegionId = SelectedRegionId;
            oCommissionCompensationSchedule.CompensationAmountTypeId = CompensationAmountTypeId;
            var Item = ManagementService.InsertCommissionCompensationScheduleData(oCommissionCompensationSchedule);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Compensation Type
        //Compensation Type
        public ActionResult CompensationType()
        {
            ViewBag.CurrentMenu = "CompensationType";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("CompensationType", "Commission", new { area = "Portal" }), "Compensation Type");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");

            return View();
        }
        public JsonResult GetCompensationTypeData()
        {
            var Item = ManagementService.GetCompensationTypeListData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCompensationTypeData(int CompensationTypeListId)
        {
            var Item = ManagementService.DeleteCompensationTypeListData(CompensationTypeListId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertUpdateCompensationTypeData(int CompensationTypeListId, string Description, int StatusListId, bool IncludeinTotalSales, bool VariableSales
            , bool CommissionBasedonTotalSale, bool UserSpecific, bool StartDateSpecific, bool IsActive)
        {
            CompensationTypeViewModel oCompensationType = new CompensationTypeViewModel();
            oCompensationType.CompensationTypeListId = CompensationTypeListId;
            oCompensationType.Description = Description;
            oCompensationType.StatusListId = StatusListId;
            oCompensationType.IsActive = IsActive;
            oCompensationType.CreatedBy = LoginUserId;
            oCompensationType.CommissionBasedonTotalSale = CommissionBasedonTotalSale;
            oCompensationType.CreatedDate = DateTime.Now;
            oCompensationType.IncludeinTotalSales = IncludeinTotalSales;
            oCompensationType.StartDateSpecific = StartDateSpecific;
            oCompensationType.UserSpecific = UserSpecific;
            oCompensationType.VariableSales = VariableSales;
            oCompensationType.RegionId = SelectedRegionId;
            var Item = ManagementService.InsertCompensationTypeListData(oCompensationType);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Commission Payment Plan / Commission Payment Schedule
        //Commission Payment Plan
        public ActionResult CommissionPaymentPlan()
        {
            ViewBag.CurrentMenu = "CommissionPaymentPlans";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Management", new { area = "Portal" }), "Management");
            BreadCrumb.Add(Url.Action("CommissionPaymentPlan", "Management", new { area = "Portal" }), "Compensation Type");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.PaymentScheduleTypeList = new SelectList(ManagementService.GetPaymentScheduleTypeListData(), "CommissionPaymentScheduleTypeId", "Name", "");
            return View();
        }

        public JsonResult GetCommissionPaymentScheduleListData()
        {
            var Item = ManagementService.GetCommissionPaymentScheduleListData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCommissionPaymentScheduleListData(int CommissionPaymentScheduleId)
        {
            var Item = ManagementService.DeleteCommissionPaymentScheduleListData(CommissionPaymentScheduleId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertUpdateCommissionPaymentScheduleData(int CommissionPaymentScheduleId, decimal Amount, string Description, bool IsActive, int PaymentScheduleTypeId, int StatusListId,
            int T1SM, int T1EM, decimal T1P, int T2SM, int T2EM, decimal T2P, int T3SM, int T3EM, decimal T3P, int T4SM, int T4EM, decimal T4P)
        {
            CommissionPaymentScheduleViewModel oCommissionPaymentSchedule = new CommissionPaymentScheduleViewModel();
            oCommissionPaymentSchedule.Amount = Amount;
            oCommissionPaymentSchedule.CommissionPaymentScheduleId = CommissionPaymentScheduleId;
            oCommissionPaymentSchedule.CreatedBy = LoginUserId;
            oCommissionPaymentSchedule.CreatedDate = DateTime.Now;
            oCommissionPaymentSchedule.Description = Description;
            oCommissionPaymentSchedule.IsActive = IsActive;
            oCommissionPaymentSchedule.PaymentScheduleTypeId = PaymentScheduleTypeId;
            oCommissionPaymentSchedule.RegionId = SelectedRegionId;
            oCommissionPaymentSchedule.StatusListId = StatusListId;
            oCommissionPaymentSchedule.Term1_EndMonth = T1EM;
            oCommissionPaymentSchedule.Term1_Percent = T1P;
            oCommissionPaymentSchedule.Term1_StartMonth = T1SM;
            oCommissionPaymentSchedule.Term2_EndMonth = T2EM;
            oCommissionPaymentSchedule.Term2_Percent = T2P;
            oCommissionPaymentSchedule.Term2_StartMonth = T2SM;
            oCommissionPaymentSchedule.Term3_EndMonth = T3EM;
            oCommissionPaymentSchedule.Term3_Percent = T3P;
            oCommissionPaymentSchedule.Term3_StartMonth = T3SM;
            oCommissionPaymentSchedule.Term4_EndMonth = T4EM;
            oCommissionPaymentSchedule.Term4_Percent = T4P;
            oCommissionPaymentSchedule.Term4_StartMonth = T4SM;

            var Item = ManagementService.InsertCommissionPaymentScheduleData(oCommissionPaymentSchedule);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Bonus + Additional Bonus Schedule
        public ActionResult Bonus()
        {
            ViewBag.CurrentMenu = "Bonus";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("Bonus", "Commission", new { area = "Portal" }), "Bonus");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");

            return View();
        }

        public JsonResult InsertUpdateBonusData(int BonusId, int saleAE_UserId, decimal bonusAmount = 0, string bonusExplanation = "", string bonusDescription = "")
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            BonusViewModel oBonus = new BonusViewModel();
            oBonus.BonusId = BonusId;
            oBonus.CreatedBy = LoginUserId;
            oBonus.CreatedDate = DateTime.Now;
            oBonus.RegionId = SelectedRegionId;
            oBonus.SaleAE_UserId = saleAE_UserId;
            oBonus.BonusAmount = bonusAmount;
            oBonus.BonusExplanation = bonusExplanation;
            oBonus.BonusDescription = bonusDescription;
            oBonus.PeriodId = _periodId;

            var Item = ManagementService.InsertBonusData(oBonus);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBonusListData()
        {
            var Item = ManagementService.GetBonusListData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteBonusListData(int BonusId)
        {
            var Item = ManagementService.DeleteBonusListData(BonusId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        //SalesPersonCommSch
        public ActionResult SalesPersonCommSch()
        {
            ViewBag.CurrentMenu = "SalesPersonCommSch";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("SalesPersonCommSch", "Commission", new { area = "Portal" }), "SA/Sales CommSch");

            ViewBag.compensationTypeList = new SelectList(ManagementService.GetCompensationTypeListData(SelectedRegionId), "CompensationTypeListId", "Description", "");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommSchSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.CommissionCompensationScheduleList = new SelectList(ManagementService.GetCommissionCompensationScheduleDropdown(SelectedRegionId), "CommissionCompensationScheduleId", "Description", "");
            ViewBag.ContractTypeList = new SelectList(CustomerService.GetContractTypeList().ToList(), "ContractTypeListId", "Name", "");

            return View();
        }
        public ActionResult SalesPersonCommSchManage(int SalesPersonCommSchId)
        {
            ViewBag.compensationTypeList = new SelectList(ManagementService.GetCompensationTypeListData(SelectedRegionId), "CompensationTypeListId", "Description", "");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.CommissionCompensationScheduleList = new SelectList(ManagementService.GetCommissionCompensationScheduleDropdown(SelectedRegionId), "CommissionCompensationScheduleId", "Description", "");
            ViewBag.ContractTypeList = new SelectList(CustomerService.GetContractTypeList().ToList(), "ContractTypeListId", "Name", "");
            var item = ManagementService.GetSalesPersonCommSchData(SalesPersonCommSchId);
            return PartialView("_PatialSalesPersonCommSchManage", item);
        }

        public JsonResult GetSalesPersonCommSchListData()
        {
            var Item = ManagementService.GetSalesPersonCommSchListData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesPersonCommSchData(int salespersonid)
        {
            var Item = ManagementService.GetSalesPersonCommSchData(salespersonid, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSalesPersonCommSchListData(int SalesPersonCommSchId)
        {
            var Item = ManagementService.DeleteSalesPersonCommSchListData(SalesPersonCommSchId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckSalesPersonCommSchData(int SalesPersonCommSchId,int SalesPersonId, int ContractTypeId)
        {
            var Item = ManagementService.CheckSalesPersonCommSchData(SalesPersonCommSchId, SalesPersonId, ContractTypeId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateSalesPersonCommSchData(int SalesPersonCommSchId, int CommissionCompensationScheduleId, DateTime? EffectiveDate, bool IsActive, int SalesPersonId, int statusListId, int ContractTypeId)
        {
            SalesPersonCommSchViewModel oSalesPersonCommSch = new SalesPersonCommSchViewModel();
            oSalesPersonCommSch.CommissionCompensationScheduleId = CommissionCompensationScheduleId;
            oSalesPersonCommSch.CreatedBy = LoginUserId;
            oSalesPersonCommSch.CreatedDate = DateTime.Now;
            oSalesPersonCommSch.EffectiveDate = EffectiveDate;
            oSalesPersonCommSch.IsActive = IsActive;
            oSalesPersonCommSch.RegionId = SelectedRegionId;
            oSalesPersonCommSch.SalesPersonCommSchId = SalesPersonCommSchId;
            oSalesPersonCommSch.SalesPersonId = SalesPersonId;
            oSalesPersonCommSch.StatusListId = statusListId;
            oSalesPersonCommSch.ContractTypeId = ContractTypeId;
            //oSalesPersonCommSch.StatusListName;
            //oSalesPersonCommSch.SalesPersonName;
            //oSalesPersonCommSch.CommissionCompensationScheduleDescription;
            //oSalesPersonCommSch.ModifiedBy;
            //oSalesPersonCommSch.ModifiedDate;
            var Item = ManagementService.InsertSalesPersonCommSchData(oSalesPersonCommSch);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }


        //SalesPersonCommSchBonus
        public ActionResult SalesPersonBonusSchedule()
        {
            ViewBag.CurrentMenu = "SalesPersonBonusSchedule";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("SalesPersonBonusSchedule", "Commission", new { area = "Portal" }), "SA/Sales Bonus Schedule");

            ViewBag.compensationTypeList = new SelectList(ManagementService.GetCompensationTypeListData(SelectedRegionId), "CompensationTypeListId", "Description", "");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommSchSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.CommissionAdditionalBonusScheduleList = new SelectList(ManagementService.GetCommissionAdditionalBonusScheduleDropdown(SelectedRegionId), "CommissionAdditionalBonusScheduleId", "Description", "");
            ViewBag.ContractTypeList = new SelectList(CustomerService.GetContractTypeList().ToList(), "ContractTypeListId", "Name", "");

            return View();
        }
        public JsonResult GetSalesPersonBonusScheduleListData()
        {
            var Item = ManagementService.GetSalesPersonCommSchBonusListData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesPersonBonusScheduleManage(int SalesPersonBonusCommissionScheduleId)
        {            
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.CommissionAdditionalBonusScheduleList = new SelectList(ManagementService.GetCommissionAdditionalBonusScheduleDropdown(SelectedRegionId), "CommissionAdditionalBonusScheduleId", "Description", "");
            ViewBag.ContractTypeList = new SelectList(CustomerService.GetContractTypeList().ToList(), "ContractTypeListId", "Name", "");
            var item = ManagementService.GetSalesPersonBonusCommissionScheduleData(SalesPersonBonusCommissionScheduleId, SelectedRegionId);
            return PartialView("_PatialSalesPersonBonusScheduleManage", item);
        }

        public JsonResult CheckSalesPersonBonusCommSchData(int SalesPersonBonusCommissionScheduleId, int SalesPersonId, int ContractTypeId)
        {
            var Item = ManagementService.CheckSalesPersonBonusCommSchData(SalesPersonBonusCommissionScheduleId, SalesPersonId, ContractTypeId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateSalesPersonBonusScheduleData(int SalesPersonBonusCommissionScheduleId, int CommissionAdditionalBonusScheduleId, DateTime? EffectiveDate, bool IsActive, int SalesPersonId, int statusListId, int ContractTypeId,int BonusAmountType,decimal BonusAmount)
        {
            SalesPersonBonusCommissionScheduleViewModel oSalesPersonCommSch = new SalesPersonBonusCommissionScheduleViewModel();
            oSalesPersonCommSch.CommissionAdditionalBonusScheduleId = CommissionAdditionalBonusScheduleId;
            oSalesPersonCommSch.CreatedBy = LoginUserId;
            oSalesPersonCommSch.CreatedDate = DateTime.Now;
            oSalesPersonCommSch.EffectiveDate = EffectiveDate;
            oSalesPersonCommSch.IsActive = IsActive;
            oSalesPersonCommSch.RegionId = SelectedRegionId;
            oSalesPersonCommSch.SalesPersonBonusCommissionScheduleId = SalesPersonBonusCommissionScheduleId;
            oSalesPersonCommSch.SalesPersonId = SalesPersonId;
            oSalesPersonCommSch.StatusListId = statusListId;
            oSalesPersonCommSch.ContractTypeId = ContractTypeId;
            oSalesPersonCommSch.BonusAmountTypeId = BonusAmountType;
            oSalesPersonCommSch.BonusAmount = BonusAmount;
            var Item = ManagementService.InsertSalesPersonBonusCommSchData(oSalesPersonCommSch);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSalesPersonBonusScheduleListData(int SalesPersonBonusCommissionScheduleId)
        {
            var Item = ManagementService.DeleteSalesPersonBonusCommSchListData(SalesPersonBonusCommissionScheduleId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ImportCommissionScheduleFromContract(int regonId = 0)
        {
            var Item = ManagementService.ImportCommissionScheduleList(regonId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetSalesPersonBonusScheduleData(int salespersonid)
        //{
        //    var Item = ManagementService.GetSalesPersonCommSchBonusData(salespersonid, SelectedRegionId);
        //    return Json(Item, JsonRequestBehavior.AllowGet);
        //}











        //Additional Bonus Schedule
        public ActionResult AdditionalBonusSchedule()
        {
            ViewBag.CurrentMenu = "AdditionalBonusSchedule";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("CommissionCompensationSchedule", "Commission", new { area = "Portal" }), "Commission Compensation Schedule");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            return View();
        }

        public JsonResult GetAdditionalBonusScheduleData()
        {
            var Item = ManagementService.GetCommissionAdditionalBonusScheduleData(SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdditionalBonusScheduleData(int CompensationScheduleId)
        {
            var Item = ManagementService.DeleteCommissionAdditionalBonusScheduleData(CompensationScheduleId, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdditionalBonusAmount(decimal Amount)
        {
            var Item = ManagementService.GetAdditionalBonusAmount(Amount, SelectedRegionId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertUpdateAdditionalBonusScheduleData(int AdditionalBonusScheduleId, int StatusListId, decimal CompAmount, decimal RangeEnd, decimal RangeStart, bool IsActive)
        {
            AdditionalBonusScheduleViewModel oAdditionalBonusSchedule = new AdditionalBonusScheduleViewModel();
            oAdditionalBonusSchedule.CommissionAdditionalBonusScheduleId = AdditionalBonusScheduleId;
            oAdditionalBonusSchedule.StatusListId = StatusListId;
            oAdditionalBonusSchedule.Amount = CompAmount;
            oAdditionalBonusSchedule.CreatedBy = LoginUserId;
            oAdditionalBonusSchedule.CreatedDate = DateTime.Now;
            oAdditionalBonusSchedule.IsActive = true;
            oAdditionalBonusSchedule.RangeEndAmount = RangeEnd;
            oAdditionalBonusSchedule.RangeStartAmount = RangeStart;
            oAdditionalBonusSchedule.RegionId = SelectedRegionId;
            var Item = ManagementService.InsertCommissionAdditionalBonusScheduleData(oAdditionalBonusSchedule);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdditionalBonusScheduleMaintenance()
        {
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            return PartialView("_PartialAdditionalBonusScheduleMaintenance");
        }

        #endregion
        #region Commission Schedule
        public ActionResult CommissionSchedule()
        {
            ViewBag.CurrentMenu = "CommissionSchedule";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Management", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("Commission", "Management", new { area = "Portal" }), "Commission Schedule");

            ViewBag.compensationTypeList = new SelectList(ManagementService.GetCompensationTypeListData(SelectedRegionId), "CompensationTypeListId", "Description", "");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.CommissionCompensationScheduleList = new SelectList(ManagementService.GetCommissionCompensationScheduleDropdown(SelectedRegionId), "CommissionCompensationScheduleId", "Description", "");

            return View();
        }
        public JsonResult GetCommissionScheduleCustomerData(int customerId)
        {
            var Item = ManagementService.GetCommissionScheduleCustomerData(customerId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommissionScheduleListData()
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            var per = _commonService.GetPeriodList().FirstOrDefault(o => o.PeriodId == _periodId);

            var Item = ManagementService.GetCommissionScheduleListData(SelectedRegionId, _periodId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }



        public JsonResult DeleteCommissionScheduleListData(int CommissionScheduleId)
        {
            var Item = ManagementService.DeleteCommissionScheduleListData(CommissionScheduleId, SelectedRegionId);
            return GetCommissionScheduleListData();
        }

        public JsonResult DeleteCommissionScheduleContractListData(int contractId)
        {
            var Item = ManagementService.DeleteCommissionScheduleContractListData(contractId, SelectedRegionId);
            return GetCommissionScheduleListData();
        }

        public JsonResult InsertUpdateCommissionScheduleData(int commissionScheduleId, int CommissionCompensationScheduleId, decimal contractAmount, DateTime? contractStartDate, DateTime? contractEndDate,
            int contractId, int customerId, string notes, bool isActive, int SalesPersonId, int statusListId, string description, decimal bonusAmount = 0, string bonusExplanation = "", string bonusDescription = "")
        {
            CommissionScheduleViewModel oCommissionSchedule = new CommissionScheduleViewModel();
            oCommissionSchedule.CommissionScheduleId = commissionScheduleId;
            oCommissionSchedule.CommissionCompensationScheduleId = CommissionCompensationScheduleId;
            oCommissionSchedule.CompensationTypeListId = 0;
            oCommissionSchedule.ContractAmount = contractAmount;
            oCommissionSchedule.ContractStartDate = contractStartDate;
            oCommissionSchedule.ContractEndDate = contractEndDate;
            oCommissionSchedule.ContractId = contractId;
            oCommissionSchedule.CustomerId = customerId;
            oCommissionSchedule.CreatedBy = LoginUserId;
            oCommissionSchedule.CreatedDate = DateTime.Now;
            oCommissionSchedule.Notes = notes;
            oCommissionSchedule.RegionId = SelectedRegionId;
            oCommissionSchedule.IsActive = isActive;
            oCommissionSchedule.Description = description;
            oCommissionSchedule.SalesPersonId = SalesPersonId;
            oCommissionSchedule.StatusListId = statusListId;
            oCommissionSchedule.BonusAmount = bonusAmount;
            oCommissionSchedule.BonusExplanation = bonusExplanation;
            oCommissionSchedule.BonusDescription = bonusDescription;



            //oCommissionSchedule.CustomerName;
            //oCommissionSchedule.SaleAE_Name;
            //oCommissionSchedule.StatusListName;
            //oCommissionSchedule.CompensationTypeListDescription;

            var Item = ManagementService.InsertCommissionScheduleData(oCommissionSchedule);
            return GetCommissionScheduleListData();
        }


        #endregion
        #region Commission Payment Schedule

        #endregion

        #region Report:: Commissions Earned 
        //Commissions Earned 
        public ActionResult CommissionsEarnedReport()
        {
            ViewBag.CurrentMenu = "CommissionsEarnedReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("CommissionsEarnedReport", "Commission", new { area = "Portal" }), "Commission Compensation Schedule");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");

            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");

            return View();
        }
        public ActionResult CommissionsEarnedReportResult(int month, int year, int userId)
        {

            var Item = ManagementService.BindCommissionsEarnedReportData(month, year, userId, SelectedRegionId);
            return PartialView("_PartialCommissionsEarnedReportResult", Item);
        }
        #endregion

        #region Report:: Commissions Earned 
        //Commissions Earned 
        public ActionResult PaymentHistoryReport()
        {
            ViewBag.CurrentMenu = "PaymentHistoryReport";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("PaymentHistoryReport", "Commission", new { area = "Portal" }), "Commission Compensation Schedule");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            ViewBag.salesUserList = new SelectList(ManagementService.GetCommissionScheduleSalesPerssonList(SelectedRegionId), "UserId", "FullName", "");
            return View();
        }

        public ActionResult PaymentHistoryReportResult(int month, int year, int userId)
        {
            ViewBag.SelectedPeriod = (new DateTime(year, month, 1)).ToString("MMMM") + " - " + year;

            var Item = ManagementService.BindPaymentHistoryReportData(month, year, userId, SelectedRegionId);
            return PartialView("_PartialPaymentHistoryReportResult", Item);
        }


        public ActionResult CurrentPaymentList()
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            var per = _commonService.GetPeriodList().FirstOrDefault(o => o.PeriodId == _periodId);
            ViewBag.SelectedPeriod = (new DateTime((int)per.BillYear, (int)per.BillMonth, 1)).ToString("MMMM") + " - " + per.BillYear;

            var Item = ManagementService.BindPaymentHistoryReportData((int)per.BillMonth, (int)per.BillYear, 0, SelectedRegionId);
            return View(Item);
        }



        public ActionResult PrintPayout(int month, int year, int userId)
        {
            ViewBag.SelectedPeriod = (new DateTime(year, month, 1)).ToString("MMMM") + " - " + year;

            var Item = ManagementService.BindPaymentHistoryReportData(month, year, userId, SelectedRegionId);
            return PartialView("_PartialPrintPayout", Item);
        }

        public FileResult PrintPayoutPDF(int month, int year, int userId)
        {
            ViewBag.SelectedPeriod = (new DateTime(year, month, 1)).ToString("MMMM") + " - " + year;

            var Item = ManagementService.BindPaymentHistoryReportData(month, year, userId, SelectedRegionId);

            string HTMLContent = string.Empty;
            HTMLContent += RenderViewToString("_PartialPrintPayoutPDF", Item);
            string filename = string.Format("{0}_PrintPayout.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + "\"");
            return File(GetCustomerStatementPDF(HTMLContent), "application/pdf");

            //if (Ids != null && Ids.Count() > 0)
            //{
            //    string HTMLContent = string.Empty;
            //    foreach (var item in Ids)
            //    {
            //        //var vm = GetCustomerStatementViewModel(Convert.ToInt32(item), start, end);
            //        HTMLContent += RenderViewToString("_CustomerStatementExportToPDF", vm);
            //    }
            //    string filename = string.Format("{0}_CustomerStatement.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            //    Response.AddHeader("content-disposition", contentDisposition + ";filename=\"" + filename + "\"");
            //    return File(GetCustomerStatementPDF(HTMLContent), "application/pdf");
            //}
            //return null;
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


        #endregion




        #region List Current / List Scheduled Commissions Review/Generate
        //List Current
        public ActionResult ListScheduledCommissions()
        {
            ViewBag.CurrentMenu = "ListScheduledCommissions";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "Commission", new { area = "Portal" }), "Commission");
            BreadCrumb.Add(Url.Action("ListScheduledCommissions", "Commission", new { area = "Portal" }), "List Current");
            ViewBag.statusList = new SelectList(_commonService.DropDownListByName(MasterDropName.CommissionStatusList.ToString()), "Value", "Text", "");
            return View();
        }

        public ActionResult PartialListScheduledCommissionReview()
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            var per = _commonService.GetPeriodList().FirstOrDefault(o => o.PeriodId == _periodId);
            ViewBag.SelectedPeriod = (new DateTime((int)per.BillYear, (int)per.BillMonth, 1)).ToString("MMMM") + " - " + per.BillYear;

            var Item = ManagementService.GetCurrentScheduledCommissionReviewData(SelectedRegionId, _periodId);
            return PartialView("_PartialListScheduledCommissionReview", Item);
        }
        public ActionResult PartialListScheduledCommissionGenerate()
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            var per = _commonService.GetPeriodList().FirstOrDefault(o => o.PeriodId == _periodId);
            ViewBag.SelectedPeriod = (new DateTime((int)per.BillYear, (int)per.BillMonth, 1)).ToString("MMMM") + " - " + per.BillYear;

            ScheduledCommissionGenerateViewModel Item = ManagementService.GetCurrentScheduledCommissionGenerateData(SelectedRegionId, _periodId);

            return PartialView("_PartialListScheduledCommissionGenerate", Item);
        }



        public JsonResult GenerateCommissionCurrentList()
        {
            var claimView = ClaimView.Instance;
            int _periodId = claimView.GetCLAIM_PERIOD_ID();
            var Item = ManagementService.GenerateCurrentScheduledCommissionData(SelectedRegionId, _periodId);
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}