using System;

namespace JKViewModels.CRM
{
   

    public class CRMFvPresentationViewModel
    {
        public int CRM_FvPresentationId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public string MeasureContactPerson { get; set; }
        public double MeasureFacility { get; set; }
        public int NumberOfFloors { get; set; }
        public int BillingFrequency { get; set; }
        public int ServiceLevel { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public bool Weekend { get; set; }
        public decimal BudgetAmount { get; set; }
        public string Note { get; set; }
        public int CleanFrequency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CleanTime { get; set; }
        public int? ServiceTypeListId { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public int RegionId { get; set; }


    }
}
