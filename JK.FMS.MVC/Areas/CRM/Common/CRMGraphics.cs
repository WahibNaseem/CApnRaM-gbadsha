using JKViewModels.CRM;

namespace JK.FMS.MVC.Areas.CRM.Common
{
    public static class CRMGraphics
    {
        public static string GetLabelForStageStatus(StageStatusType stageStatusType)
        {
            switch (stageStatusType)
            {
                case StageStatusType.Unknown:
                    return @"label-default";
                case StageStatusType.NewLead:
                    return @"label-info";
                case StageStatusType.QualifiedLead:
                    return @"label-success";
                case StageStatusType.UnqualifiedLead:
                    return @"label-danger";
                default:
                    return @"label-default";
            }
        }

        public static string GetEventBackGroundColor(int? scheduleType)
        {
            
            if (scheduleType == 1)
                return "#e6e600";
            if (scheduleType == 16)  
                return "#377def";     //Fv Presentation Color Code
            //if (scheduleType == 21)
            //    return "#a2c3f9";
            if (scheduleType == 22)
                return "#4ca344";  //Bidding Color Code b3b36f
            if (scheduleType == 23)
                return "#b7a45f";    //PdAppointment
            if (scheduleType == 24)
                return "#b3b36f";          //Follow up
            if (scheduleType == 30)
                return "#2b761"; // 
            else
                return "#e88504";
        }                
    }
}                              