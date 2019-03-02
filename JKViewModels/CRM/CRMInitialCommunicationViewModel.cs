namespace JKViewModels.CRM
{ 
    using System;


    public class CRMInitialCommunicationViewModel
    {
        public int CRM_InitialCommunicationId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public string ContactPerson { get; set; }
        public bool? InterestedInPerposal { get; set; }
        public DateTime? StartDate { get; set; }
        public string Note { get; set; } 
        public int? PURPOSE { get; set; }
        public int? RegionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; } 
        public DateTime? ModifiedDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
