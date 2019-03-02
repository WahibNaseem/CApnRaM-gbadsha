using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
   public class CRMScheduleCalenderViewModel
    {
        public CRMScheduleCalenderViewModel()
        {
            lstscheduleCalendar = new List<CRMScheduleCalenderViewModel>();
        }
        public int CRM_ScheduleId { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string Location { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CRM_ScheduleTypeId { get; set; }
        public bool? IsAllDay { get; set; }
        public string Description { get; set; }
        public string backgroundColor { get; set; }
        public int PurposeId { get; set; }

        public List<CRMScheduleCalenderViewModel> lstscheduleCalendar { get; set; }
    }
}
