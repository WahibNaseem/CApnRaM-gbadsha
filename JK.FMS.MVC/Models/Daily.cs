using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JK.FMS.MVC.Models
{
    public class Daily
    {
        [DisplayName("Date-Time")]
        public DateTime dateTime { get; set; }
    }
}