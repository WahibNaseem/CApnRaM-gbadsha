using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class OtherDepositsAddViewModel
    {
        [DisplayName("Reference No.")]
        public string referenceNo { get; set; }
        [DisplayName("Amount")]
        public string amount { get; set; }
        [DisplayName("Date")]
        public DateTime date { get; set; }
        [DisplayName("Type")]
        public string type { get; set; }
        [DisplayName("Description")]
        public string description { get; set; }
    }
}