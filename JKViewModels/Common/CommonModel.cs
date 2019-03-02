using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class DropDownModel
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }

    public class DashboardQuickLinkModel
    {
        public int ItemCount { get; set; }

        public string LinkText { get; set; }

        public string PageUrl { get; set; }

        public int Type { get; set; }

        public string TypeName { get; set; }

        public string DashBoardBox { get; set; }

        public string iconURL { get; set; }
    }

    public class PendingDashboardDataModel
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public string Message { get; set; }
        public bool IsRejacted { get; set; }
        public bool Isapproved { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime MessageDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsView { get; set; }
        public int MessageID { get; set; }
        public int ChildUID { get; set; }
        public int CreatedByChild { get; set; }
        public string EntrySource { get; set; }
        public int MasterTmpTrxId { get; set; }
    }

    public class PendingDashboardTasksDataModel
    {
        public int Id { get; set; }
        public int AccountCustomerDetailId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int AccountFranchiseDetailId { get; set; }
        public bool IsFromOutlook { get; set; }
        public Guid OutlookAppointmentGuid { get; set; }
        public DateTime OutlookSyncDate { get; set; }
        public string Location { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAllDay { get; set; }
        public int ScheduleTypeId { get; set; }
        public int StageStatusType { get; set; }
        public int RegionId { get; set; }
        public int AssignToId { get; set; }
        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public bool IsActive { get; set; }

    }

    public class MailMessageTemplateModel : BaseModel
    {
        public int MailMessageTemplateId { get; set; }

        public string MessageName { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }


    }


    public class PeriodViewModel
    {
        public int PeriodId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public string Period{
            get {return BillMonth.ToString() +"/"+ BillYear.ToString();}
            set {this.Period = value;}
        }


    }

}
