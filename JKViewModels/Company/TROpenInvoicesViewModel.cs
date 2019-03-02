using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class TROpenInvoicesViewModel
    {
        public string VendorID { get; set; } 
        public string VendorName { get; set; } 
        public decimal GrossDue { get; set; } 
        public bool Ten99InvoiceYN { get; set; } 
        public string InvoiceNum { get; set; } 
        public DateTime InvoiceDate { get; set; } 
        public DateTime DiscDueDate { get; set; } 
        public DateTime NetDueDate { get; set; } 
        public int Status { get; set; } 
        public decimal ExchRate { get; set; } 
        public string CurrencyId { get; set; } 
        public decimal CurrentBalance { get; set; } 
        public decimal GrossAmt { get; set; } 
        public decimal Discount { get; set; } 
        public decimal DiscountFgn { get; set; } 
        public decimal GrossDueAmt { get; set; } 
        public decimal Payments { get; set; } 
        public decimal NetDue { get; set; } 
    }
    
    public class apTraverseOpenInvoiceList
    {
        public string VendorID { get; set; } 
        public string VendorName { get; set; } 
        public string InvoiceNum { get; set; } 
//          public DateTime DiscDueDate { get; set; } 
//          public DateTime NetDueDate { get; set; } 
//          public int Status { get; set; } 
        public decimal GrossDueAmt { get; set; } 
    }

    
    
}