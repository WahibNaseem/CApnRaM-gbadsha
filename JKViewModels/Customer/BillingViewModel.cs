using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class BillingViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AllowCPIIncreases { get; set; }
        public int ARStatus { get; set; }
        public int Term { get; set; }
        public string ARStatusName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int ConsolidatedInvoice { get; set; }
        public int CreateInvoice { get; set; }
        public int PrintInvoice { get; set; }
        public int PrintPastDue { get; set; }
        public int EBill { get; set; }
        public int TaxExempt { get; set; }
        public int TaxAuthority { get; set; }
        public string TaxAuthorityName { get; set; }
        public string InvoiceMessage { get; set; }
        public int InvoiceDate { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        // cj - 4/27/2016 - this only by the customer header ui... it is based on 30 day block (JK Way) only
        public decimal AgingDayCurrent = 0.00m;
        public decimal AgingDay30 = 0.00m;
        public decimal AgingDay60 = 0.00m;
        public decimal AgingDay90 = 0.00m;
        public decimal AgingDay120Plus = 0.00m;



    }
}
