
using System;

namespace JKViewModels.CRM
{
  public class CRMCloseViewModel
    {
        public int CRM_CloseId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public bool? HaveBackgroundCheck { get; set; }
        public bool? SignedAgreement { get; set; }
        public bool? DocumentSalesCRM { get; set; }
        public bool? NotifyAccountPlacement { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? Mon { get; set; }
        public bool? Fri { get; set; }
        public bool? Sat { get; set; }
        public bool? Sun { get; set; }
        public bool? Wed { get; set; }
        public bool? Thu { get; set; }
        public bool? Tue { get; set; }
        public bool? Weekend { get; set; }
        public decimal? PropAmount { get; set; }
        public int? InitialClean { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? InitialCleanAmount { get; set; }
        public DateTime? SignDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ContractTerm { get; set; }
        public int? ServiceType { get; set; }
        public int? BillingFrequency { get; set; }
        public int? CleanTime { get; set; }
        public int? CleanFrequency { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
