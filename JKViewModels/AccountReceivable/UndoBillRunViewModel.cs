using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class UndoBillRunViewModel
    {
        [DisplayName("Bill Month/Year")]
        public string billMonthYear { get; set; }
    }
}