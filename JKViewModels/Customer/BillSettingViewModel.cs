using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class BillSettingViewModel
    {
        public int BillSettingsId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ARStatusListId { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<int> InvoiceDateListId { get; set; }
        public Nullable<int> InvoiceTermListId { get; set; }
        public bool EBill { get; set; }
        public bool CreateInvoice { get; set; }
        public bool PrintInvoice { get; set; }
        public bool PrintPastDue { get; set; }
        public bool TaxExcempt { get; set; }
        public bool ConsolidatedInvoice { get; set; }
        public string InvoiceMessage { get; set; } 
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> IsActive { get; set; }
        public bool PBill { get; set; }
        public Nullable<int> ARStatus { get; set; }
    }
}
