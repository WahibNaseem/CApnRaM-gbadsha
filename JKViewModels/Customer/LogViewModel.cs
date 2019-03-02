using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class LogViewModel
    {
        #region Constants

        public static int CallBack = 1;

        public static int TypeRegular = 1;
        public static int TypeNew_Account = 2;
        public static int TypeTransfer = 3;
        public static int TypeComplaint = 4;
        public static int TypeFollow_Up = 5;
        public static int TypeInspection = 6;
        public static int TypeContact_Evalution = 7;
        public static int TypeFailed_Inspection = 8;
        public static int TypeCancellation = 9;
        public static int TypeRequest = 10;
        public static int TypePending_Cancellation = 12;
        public static int TypeClear_At_Risk = 13;
        public static int TypeIncrease = 14;
        public static int TypeDecrease = 15;
        public static int TypeSuspension = 16;
        public static int TypeFax_A_Comment = 17;
        public static int TypeMissed_Clean = 18;
        public static int TypeComplaint_Resolved = 19;
        public static int TypeClaim_Incident = 21;
        public static int TypeClear_Claim_Incident = 22;
        public static int TypeEmailed_Invoice = 23;

        public static int StatusBusy = 1;
        public static int StatusContacted = 2;
        public static int StatusLeft_Message = 3;
        public static int StatusNo_Answer = 4;
        public static int StatusOther = 5;
        public static int StatusSent_Letter = 6;
        public static int StatusWrong_No = 7;
        public static int StatusEmail = 8;

        public static int InitiatedByJaniKing = 1;
        public static int InitiatedByCustomer = 2;
        public static int InitiatedByFranchise = 3;

        #endregion

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int Internal { get; set; }
        public int InitiatedBy { get; set; }
        public int TypeId { get; set; }
        public string Action { get; set; }
        public DateTime CallBackDate { get; set; }
        public string CallBackTime { get; set; }
        public int FollowUpBy { get; set; }
        public string SpokeWith { get; set; }
        public string Area { get; set; }
        public string EmailNotesTo { get; set; }
        public string Notes { get; set; }
        public int ReferenceId { get; set; }

        //-- these values are only set when a collection of these object is being returne for display
        public string TypeName { get; set; }
        public string InitiatedByName { get; set; }
        public string FollowUpByName { get; set; }
        public string Franchisees { get; set; }
    }
}
