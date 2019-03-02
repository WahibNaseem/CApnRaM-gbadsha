namespace JKViewModels.CRM
{
    public class CRMNoteViewModel
    {
        public int CRM_NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
