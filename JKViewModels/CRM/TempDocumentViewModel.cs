using System;
using System.Web;

namespace JKViewModels.CRM
{
   public class TempDocumentViewModel
    {
        public int CRM_AccountCustomerDetailId { get; set; }        
        public HttpPostedFileBase file1 { get; set; }
        public HttpPostedFileBase file2 { get; set; }
        public HttpPostedFileBase file3 { get; set; }
        public HttpPostedFileBase file4 { get; set; }
        public HttpPostedFileBase file5 { get; set; }
        public HttpPostedFileBase file6 { get; set; }
        public HttpPostedFileBase file7 { get; set; }
        public HttpPostedFileBase file8 { get; set; }
        public HttpPostedFileBase file9 { get; set; }
        public HttpPostedFileBase file10 { get; set; }
        public HttpPostedFileBase file11 { get; set; }
        public HttpPostedFileBase file12 { get; set; }
        public string[] CleaningDay { get; set; }
        public int CRM_CloseId { get; set; } 
        public int CRM_AccountId { get; set; }
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

    }
}
