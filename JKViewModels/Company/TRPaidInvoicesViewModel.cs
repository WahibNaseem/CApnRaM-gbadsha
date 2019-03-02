using JKViewModels.Franchise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JKViewModels.Company
{
    /*************************************************************************/
    /*9-20-2018 : Maria: Used in Traverse function to get vendor invoice data*/
    /************************************************************************/
    public class TRPaidInvoicesViewModel
    {

        public string VendorId { get; set; }
        public string InvoiceNum { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime CheckDate { get; set; }
        public string CheckNum { get; set; }
        public decimal ChkGrossAmtDue { get; set; }
        public decimal ChkDiscAmt { get; set; }
        public decimal ChkNetPaid { get; set; }
        public decimal CheckAmount { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorCity { get; set; }
        public string VendorpostalCode { get; set; }
        public string VendorPhone { get; set; }

        public class VendorInvoiceViewModel
        {

            public string VendorId { get; set; }
            public string InvoiceNum { get; set; }
            public DateTime InvoiceDate { get; set; }
            public DateTime CheckDate { get; set; }
            public string CheckNum { get; set; }
            public decimal ChkGrossAmtDue { get; set; }
            public decimal ChkDiscAmt { get; set; }
            public decimal ChkNetPaid { get; set; }
            public string VendorName { get; set; }
            public string VendorAddress1 { get; set; }
            public string VendorCity { get; set; }
            public string VendorpostalCode { get; set; }
            public string VendorPhone { get; set; }

       
        }

    }

   
}
