namespace JKViewModels.CRM
{
    using System;

    public class CRMTaskViewModel
    {
        public int CRM_TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDateTime { get; set; }
        public int TaskType { get; set; }
        public int Assignee_ { get; set; }
        public DateTime EmailReminder { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
