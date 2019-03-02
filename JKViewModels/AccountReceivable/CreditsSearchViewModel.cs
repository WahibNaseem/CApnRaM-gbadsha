using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class CreditsSearchViewModel
    {
        [DisplayName("Search By")]
        public string searchBy { get; set; }
        [DisplayName("Search Value")]
        public string searchValue { get; set; }
        [DisplayName("Bill Month/Year")]
        public string billMonth { get; set; }
        [DisplayName("")]
        public string billYear { get; set; }
       
    }
}