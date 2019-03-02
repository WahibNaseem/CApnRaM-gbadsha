using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeMaintenanceViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }

        //Customer Info
        public string FranchiseeAddress { get; set; }
        public string FranchiseeCity { get; set; }
        public string FranchiseeState { get; set; }
        public string FranchiseePincode { get; set; }

        //Customer Billing Info
        //public string CustomerBillingName { get; set; }
        //public string CustomerBillingAddress { get; set; }
        //public string CustomerBillingCity { get; set; }
        //public string CustomerBillingState { get; set; }
        //public string CustomerBillingPincode { get; set; }

        //Customer Status Info
        public int? StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public string StatusListName { get; set; }
        public Nullable<int> ReasonListId { get; set; }
        public string ReasonListName { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

       
    }

    public class EditFranchiseeMaintenanceViewModel
    {
        public int MaintenanceTempId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> StatusReasonListId { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RequestChangeStatusListId { get; set; }
        public Nullable<int> RequestChangeNotes { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> MaintenanceTypeListId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Reason { get; set; }
    }
}
