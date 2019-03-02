using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class StageSettingModel : BaseModel
    {
        public int CRM_StageTimeCalculationId { get; set; }
        public int CRM_StageStatusId { get; set; }
        public int DayLeft { get; set; }
        public int HourLeft { get; set; }

        public int MinuteLeft { get; set; }

        public string StageStatus { get; set; }
    }

    public class StageSettingViewModel : PaggingModel
    {
        public StageSettingViewModel()
        {
            StageSettingList = new List<StageSettingModel>();
        }


        public List<StageSettingModel> StageSettingList { get; set; }

        public int CRM_StageTimeCalculationId { get; set; }
        public int CRM_StageStatusId { get; set; }
        public int DayLeft { get; set; }
        public int HourLeft { get; set; }

        public int MinuteLeft { get; set; }

        public string ActionType { get; set; }

        public string StageStatus { get; set; }
    }
}
