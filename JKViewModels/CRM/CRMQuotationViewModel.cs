namespace JKViewModels.CRM
{
    using System;

    public class CRMQuotationViewModel
    {
        public int CRM_QuotationId { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }
        public DateTime CloseDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
