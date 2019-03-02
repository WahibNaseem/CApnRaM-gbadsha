using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JK.FMS.MVC.Models
{
    public class ARLog
    {
        [DisplayName("Date Type")]
        public string dateType { get; set; }
        [DisplayName("Date Range")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }
        [DisplayName()]
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }
    }
}