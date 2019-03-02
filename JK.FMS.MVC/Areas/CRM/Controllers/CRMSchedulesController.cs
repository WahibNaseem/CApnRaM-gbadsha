using Application.Web.Core;
using JK.FMS.MVC.Areas.CRM.Common;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Outlook;
using JKViewModels;
using JKViewModels.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    //[OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    public class CRMSchedulesController : ViewControllerBase
    {
        private readonly ICRM_Service _crmService;
        private readonly IOutlookService _outlookService;

        public CRMSchedulesController(ICRM_Service ICRMService, IOutlookService outlookService)
        {
            _crmService = ICRMService;
            _outlookService = outlookService;
        }

        // GET: CRM/CRMSchedules
        public ActionResult Index()
        {
            ViewBag.HMenu = "CRMSchedules";
            DashboardViewModel model = new DashboardViewModel();
            ViewBag.CurrentUser = LoginUserId;
            var accountCustomerScheduleViewModels = new CRMScheduleUserCalendarViewModel();

            return View(model);
        }

        public ActionResult GetSchedules()
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();


            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();

            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.CreatedBy == LoginUserId).ToList();
            /* var accountStageStatusSchedules = _crmService.GetAll_CRM_StageStatusSchedule().Where(x => x.CreatedBy == LoginUserId || SelectedUserId == 0).ToList();*/

            #region Calendar

            //Schedule

            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountCustomerScheduleViewModels)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    calendar.title = accountschedule.Title;
                    calendar.start = accountschedule.StartDate.Value.ToString("s");
                    calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.Description = accountschedule.Description;
                    calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                    calendar.IsAllDay = accountschedule.IsAllDay;
                    calendar.Location = accountschedule.Location;

                    calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_StageStatusType);
                    accountCustomerScheduleCalendar.Add(calendar);
                }
            }

            #endregion

            return Json(new
            {
                success = true,
                result = accountCustomerScheduleCalendar,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewSchedule(FormCollection frm)
        {
            // bool isall;

            var vm = new CRMScheduleViewModel();
            vm.CRM_ScheduleId = Convert.ToInt32(frm["input_crmscheduleid"]);
            vm.Title = frm["input_scheduletitle"];
            vm.Description = frm["input_scheduledescription"];
            vm.Location = frm["input_schedulelocation"];
            vm.StartDate = frm["input_schedulestartdate"] != "" ? Convert.ToDateTime(frm["input_schedulestartdate"] + " " + frm["input_schedulestarttime"]) : DateTime.Now;
            vm.EndDate = frm["input_scheduleenddate"] != "" ? Convert.ToDateTime(frm["input_scheduleenddate"] + " " + frm["input_scheduleendtime"]) : DateTime.Now;
            vm.CRM_ScheduleTypeId = Convert.ToInt32(frm["input_scheduletype"]);
            vm.IsActive = true;
            //vm.IsAllDay = bool.TryParse(frm["checkbox_alldayevent"], out isall) ? isall : false;
            vm.CreatedBy = this.LoginUserId;
            var newSchedule = CRMAutoMapper.CrmScheduleViewModelToEntity(vm);
            newSchedule = _crmService.SaveCRM_Schedule(newSchedule);

            //return Json(new { success = true, id = newSchedule.CRM_ScheduleId });
            List<CRMScheduleCalenderViewModel> accountCustomerScheduleCalendar;
            List<CRMScheduleUserHierarchy> userHierarchy, lstRegion;
            GetSalesSchedule("", "", out accountCustomerScheduleCalendar, out userHierarchy, out lstRegion);
            return Json(new { success = true, result = accountCustomerScheduleCalendar, region = lstRegion, user = userHierarchy, selectedUser = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchSchedule(string look, string crm)
        {
            var accountCustomerScheduleViewModels = new List<CRMScheduleViewModel>();
            var accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();
            var outlook = Convert.ToInt32(look);
            var Crm = Convert.ToInt32(crm);
            var accountSchedule = _crmService.GetAll_CRM_Schedule().Where(x => x.CreatedBy == LoginUserId && (x.CRM_ScheduleTypeId == outlook || x.CRM_ScheduleTypeId == Crm)).ToList();
            if (accountSchedule != null)
            {
                accountCustomerScheduleViewModels = CRMAutoMapper.CrmScheduleEntitiesToViewModels(accountSchedule);
                foreach (var accountschedule in accountCustomerScheduleViewModels)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    calendar.title = accountschedule.Title;
                    calendar.start = accountschedule.StartDate.Value.ToString("s");
                    calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.Description = accountschedule.Description;
                    calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                    calendar.IsAllDay = accountschedule.IsAllDay;
                    calendar.Location = accountschedule.Location;

                    calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_StageStatusType);
                    accountCustomerScheduleCalendar.Add(calendar);
                }
            }
            return Json(new { success = true, result = accountCustomerScheduleCalendar }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesSchedule(string types, string users)
        {
            List<CRMScheduleCalenderViewModel> accountCustomerScheduleCalendar;
            List<CRMScheduleUserHierarchy> userHierarchy, lstRegion;
            GetSalesSchedule(types, users, out accountCustomerScheduleCalendar, out userHierarchy, out lstRegion);
            return Json(new { success = true, result = accountCustomerScheduleCalendar, region = lstRegion, user = userHierarchy, selectedUser = users }, JsonRequestBehavior.AllowGet);
        }

        private void GetSalesSchedule(string types, string users, out List<CRMScheduleCalenderViewModel> accountCustomerScheduleCalendar, out List<CRMScheduleUserHierarchy> userHierarchy, out List<CRMScheduleUserHierarchy> lstRegion)
        {
            var accountCustomerScheduleViewModels = new CRMScheduleUserCalendarViewModel();
            accountCustomerScheduleCalendar = new List<CRMScheduleCalenderViewModel>();
            userHierarchy = new List<CRMScheduleUserHierarchy>();
            lstRegion = new List<CRMScheduleUserHierarchy>();
            accountCustomerScheduleViewModels = _crmService.GetCRMScheduleByUser(this.LoginUserId, 0, users, types);
            lstRegion = accountCustomerScheduleViewModels.lstCRMUserHierarchy.Where(p => p.RegionId != null && p.RegionId != 0).GroupBy(x => new { x.RegionId, x.RegionName })
                .Select(d => new CRMScheduleUserHierarchy
                {
                    RegionId = d.First().RegionId,
                    RegionName = d.First().RegionName
                }).ToList();
            userHierarchy = accountCustomerScheduleViewModels.lstCRMUserHierarchy.Where(d => d.UserId != this.LoginUserId).ToList();

            if (accountCustomerScheduleViewModels.lstCRMScheduleUserCalender != null)
            {
                foreach (var accountschedule in accountCustomerScheduleViewModels.lstCRMScheduleUserCalender)
                {
                    CRMScheduleCalenderViewModel calendar = new CRMScheduleCalenderViewModel();
                    calendar.CRM_ScheduleId = accountschedule.CRM_ScheduleId;
                    calendar.title = accountschedule.Title;
                    calendar.start = accountschedule.StartDate.Value.ToString("s");
                    calendar.EndDate = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartDate = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("MM/dd/yyyy") : "";
                    calendar.StartTime = accountschedule.StartDate != null ? accountschedule.StartDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.EndTime = accountschedule.EndDate != null ? accountschedule.EndDate.Value.ToString("h:mm:ss tt") : "";
                    calendar.Description = accountschedule.Description;
                    calendar.CRM_ScheduleTypeId = accountschedule.CRM_ScheduleTypeId.ToString();
                    calendar.IsAllDay = accountschedule.IsAllDay;
                    calendar.Location = accountschedule.Location;

                    calendar.backgroundColor = CRMGraphics.GetEventBackGroundColor(accountschedule.CRM_StageStatusType);
                    accountCustomerScheduleCalendar.Add(calendar);
                }
            }
        }
    }
}