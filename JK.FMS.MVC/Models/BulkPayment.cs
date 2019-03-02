using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JK.FMS.MVC.Models
{
    public class BulkPayment
    {
        public string type { get; set; }
        public string referenceNo { get; set; }
        public DateTime date { get; set; }
        public string amount { get; set; }
        public string balance { get; set; }
        public string searchBy { get; set; }
        public string forr { get; set; }
        public string billMonth { get; set; }
        public string billYear { get; set; }
        public bool consolidatedInvoice { get; set; }

    }
}