using Application.Web.Core;
using JKApi.Service;
using JKApi.Service.ServiceContract.CRM;
using JKViewModels;
using JKViewModels.CRM;
using JKViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    public class CRMAdministrationController : ViewControllerBase
    {

        #region LifeCycle
        public new readonly ICRM_Service _crmService;
        public CRMAdministrationController(IUserService userService, ICRM_Service ICRMService, ICommonService commonservice)
        {
            this._userService = userService;
            _crmService = ICRMService;
            _commonService = commonservice;
        }

        #endregion

        public CRMAdministrationController()
        {
            ViewBag.HMenu = "Administration";
        }

        [HttpGet]
        // GET: CRM/CRMAdministration
        public ActionResult Index()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "Record";
            return View();
        }

        [HttpGet]
        public ActionResult ZipCodeAssignment()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "ZipCode Assignment";
            ZipCodeAssignmentViewModel model = new ZipCodeAssignmentViewModel();
            model.TerritoryList = _crmService.GetAll_CRM_Territory_New().Where(x => (x.RegionId == SelectedRegionId) && (x.Name== "Buffalo North" || x.Name== "Buffalo South" || x.Name == "Exception")).Select(x => new TerritoryViewModel { Id = x.CRM_TerritoryId, Name = x.Name }).ToList();
            model.ZipCodeList = _crmService.GetAll_CRM_Territory_Assignment_New().Select(x => new ZipCodeModel
            { TerritoryId = x.CRM_TerritoryId, TerritoryAssignmentId = x.CRM_TerriAssignmentId, ZipCode = x.ZipCode }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult TerritoryAssignment()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "Territory Assignment";
            TerritoryAssignmentViewModel model = new TerritoryAssignmentViewModel();
            model.UserList = _userService.GetUserSearchList(new UserLoginListViewModel { RegionId = SelectedRegionId, IsActve = true }).userList
                            .Where(x => x.FirstName != "" && x.FirstName != null).ToList();
            model.TerritoryList = _crmService.GetAll_CRM_Territory_New().Where(x => x.RegionId == SelectedRegionId).Select(x => new TerritoryViewModel { Id = x.CRM_TerritoryId, Name = x.Name }).ToList();
            model.SalesTerritoryAssignmentList = _crmService.GetAll_CRM_SalesTerriAssignment().Select(x => new TerritoryAssignmentModel
            { TerritoryID = x.CRM_TerritoryId, UserID = x.UserId }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveTerritoryAssignment(SaveTerritoryAssignmentViewModel model)
        {
            _crmService.DeleteAllCRMSalesTerritoryAssignment();
            _crmService.SaveAllCRMSalesTerritoryAssignment(model);
            _userService.UpdateIsAccExec(model.CheckedUserIsAccExec, model.NoNCheckedUserIsAccExec);
            return Json("saved");
        }

        [HttpPost]
        public ActionResult MoveZipCodeAssignmentPopup(ZipCodeAssignmentPopupModel model)
        {
            try
            {
                _crmService.UpdateMultiple_CRM_TerriAssignmentNew(model);
            }
            catch (Exception ex)
            {
                return Json("Some error occured.");
            }
            return Json("Updated");
        }

        [HttpPost]
        public ActionResult ZipCodeAssignmentPopup(ZipCodeAssignmentPopupModel model)
        {
            model.TerritoryList = _crmService.GetAll_CRM_Territory_New()
                .Where(x => x.RegionId == SelectedRegionId && x.CRM_TerritoryId != model.CurrentTerritoryId)
                .Select(x => new TerritoryViewModel { Id = x.CRM_TerritoryId, Name = x.Name }).ToList();
            return PartialView("ZipCodeAssignmentPopup", model);
        }

        [HttpPost]
        public ActionResult ShowAddZipCodePopup()
        {
            AddZipCodePopupModel model = new AddZipCodePopupModel();
            model.TerritoryList = _crmService.GetAll_CRM_Territory_New()
                .Where(x => (x.RegionId == SelectedRegionId) && (x.Name == "Buffalo North" || x.Name == "Buffalo South" || x.Name == "Exception"))
                .Select(x => new TerritoryViewModel { Id = x.CRM_TerritoryId, Name = x.Name }).ToList();
            return PartialView("AddZipCodePopup", model);
        }
        public ActionResult ShowAddTerritoryPopup()
        {
            return PartialView("AddTerritoryPopup");
        }

        [HttpPost]
        public ActionResult AddZipCode(AddZipCodePopupModel model)
        {
            try
            {
                if (_crmService.GetCRM_Territory_Assignment_NewByZipCode(model.ZipCode) == null)
                {
                    _crmService.AddZipCode(model);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                return Json("Some error occured.");
            }
            return Json("Saved");
        }
        [HttpPost]
        public ActionResult AddTerritory(AddTerritoryPopupModel model)
        {
            //CRM_Territory_New
            try
            {
                if (_crmService.GetCRM_Territory_NewByNameandRegionID(model.Name, SelectedRegionId) == null)
                {
                    _crmService.AddTerritory(model);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json("Saved");
        }

        #region Role
        public ActionResult StageSetting()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "CRMSetting";

            StageSettingViewModel model = new StageSettingViewModel();
            model = _crmService.GetStageSettingList(model,"");

            ViewBag.StateTypeList = new SelectList(model.StageSettingList, "CRM_StageStatusId", "StageStatus");
            List<DropDownModel> DayLeft = new List<DropDownModel>();
            for (int i = 0; i <= 20; i++)
            {
                DayLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }

            List<DropDownModel> HourLeft = new List<DropDownModel>();
            for (int i = 0; i <= 23; i++)
            {
                HourLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }

            List<DropDownModel> MinuteLeft = new List<DropDownModel>();
            for (int i = 0; i <= 60; i++)
            {
                MinuteLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }
            ViewBag.DayLeftList = new SelectList(DayLeft, "Value", "Text");
            ViewBag.HourLeftList = new SelectList(HourLeft, "Value", "Text");
            ViewBag.MinuteLeftList = new SelectList(MinuteLeft, "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public ActionResult StageSetting(StageSettingViewModel model)
        {
            model.CreatedBy = 1;
            model.IsEnable = true;
            model.ActionType = model.CRM_StageTimeCalculationId > 0 ? "U" : "I";
            model = _crmService.SaveStageSetting(model);
            return Redirect("StageSetting");
        }

        public ActionResult CancellationProcessSetting()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "CRMSetting";

            StageSettingViewModel model = new StageSettingViewModel();
            model = _crmService.GetStageSettingList(model, "Cancellation Process");

            ViewBag.StateTypeList = new SelectList(model.StageSettingList, "CRM_StageStatusId", "StageStatus");
            List<DropDownModel> DayLeft = new List<DropDownModel>();
            for (int i = 0; i <= 20; i++)
            {
                DayLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }

            List<DropDownModel> HourLeft = new List<DropDownModel>();
            for (int i = 0; i <= 23; i++)
            {
                HourLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }

            List<DropDownModel> MinuteLeft = new List<DropDownModel>();
            for (int i = 0; i <= 60; i++)
            {
                MinuteLeft.Add(new DropDownModel() { Value = i, Text = i.ToString() });
            }
            ViewBag.DayLeftList = new SelectList(DayLeft, "Value", "Text");
            ViewBag.HourLeftList = new SelectList(HourLeft, "Value", "Text");
            ViewBag.MinuteLeftList = new SelectList(MinuteLeft, "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public ActionResult CancellationProcessSetting(StageSettingViewModel model)
        {
            model.CreatedBy = 1;
            model.IsEnable = true;
            model.ActionType = model.CRM_StageTimeCalculationId > 0 ? "U" : "I";
            model = _crmService.SaveStageSetting(model);
            return Redirect("CancellationProcessSetting");
        }

        public ActionResult UserRelations()
        {
            ViewBag.HMenu = "Administration";
            ViewBag.CurrentMenu = "UserRelations";

            ViewBag.UserList = _crmService.GetAllUserList().OrderBy(x => x.UserName).Select(x => new SelectListItem { Text = (x.UserName), Value = x.UserId.ToString() });
            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            ViewBag.selectedRegionId = SelectedRegionId;
            var result = _crmService.GetAllUserRelation();
            return View(result);
        }

        [HttpPost]
        public JsonResult SaveUserRelation(CRMScheduleUserHierarchy model)
        {
            CRMScheduleUserHierarchy user = new CRMScheduleUserHierarchy
            {
                ParentUserId = model.ParentUserId,
                lstUserId = model.lstUserId,
                Id = model.Id,
                lstRegionId = model.lstRegionId,
                Condition = model.Condition,
                UserId = model.UserId,
                RegionId = model.RegionId
            };
            var result = _crmService.SaveUserRelation(user, this.LoginUserId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}