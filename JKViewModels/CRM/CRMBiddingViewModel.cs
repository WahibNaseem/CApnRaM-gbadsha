using System;

namespace JKViewModels.CRM
{
    
    public class CRMBiddingViewModel
    {
        public int CRM_BiddingId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public bool? AnalysisWorkBook { get; set; }
        public bool? IsBidSheet { get; set; }
        public bool? IsCancellation { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public int? PriceApproved { get; set; }
        public bool? IfBidOver { get; set; }
        public decimal? IncludePrice { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiededDate { get; set; }

        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public int RegionId { get; set; }
    }
}
