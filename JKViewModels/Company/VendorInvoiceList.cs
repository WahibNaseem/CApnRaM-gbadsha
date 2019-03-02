using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class VendorInvoiceList
    {
        public int VendorInvoiceId { get; set; } //(int, not null)
        public int VendorId { get; set; } //(int, null)
        public string VendorName { get; set; } //(char(50), null)
        public string VendorInvoiceNo { get; set; } //(varchar(15), null)
        public DateTime InvoiceDate { get; set; } //(date, null)
        public DateTime InvoiceDueDate { get; set; } //(date, null)
        public decimal GrossDue { get; set; } //(decimal(12,2), null)
        public int TransactionStatusListId { get; set; } //(int, not null)
        public string Status { get; set; } //(varchar(125), null)

        
    }

}
