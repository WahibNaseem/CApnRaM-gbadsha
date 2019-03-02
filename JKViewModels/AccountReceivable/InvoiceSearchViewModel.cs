using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class InvoiceSearchViewModel
    {
        [DisplayName("Consolidated Invoice")]
        public bool consolidatedInvoice { get; set; }
        [DisplayName("Search By")]
        public string searchBy { get; set; }
        [DisplayName("Search Value")]
        public string searchValue { get; set; }
        [DisplayName("Bill Month/Year")]
        public string billMonth { get; set; }
        [DisplayName("")]
        public string billYear { get; set; }
        [DisplayName("Filter By")]
        public string filterBy { get; set; }
        [DisplayName("Open Invoice Only")]
        public bool openInvoiceOnly { get; set; }
        [DisplayName("EOM only")]
        public bool eomOnly { get; set; }
        
    }
}