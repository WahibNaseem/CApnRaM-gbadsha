using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JK.FMS.MVC.Models
{
    public class Aging
    {
        [DisplayName("Search By")]
        public string searchBy { get; set; }
        [DisplayName("Search Value")]
        public string searchValue { get; set; }
        [DisplayName("Aging Date")]
        [DataType(DataType.Date)]
        public DateTime agingDate { get; set; }
        [DisplayName("Payment Date")]
        [DataType(DataType.Date)]
        public DateTime paymentDate { get; set; }
        [DisplayName("Months To Include")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int monthsToInclude { get; set; }
        [DisplayName("Order By")]
        public string orderBy { get; set; }
        [DisplayName("With Balance")]
        public string withBalance { get; set; }
        [DisplayName("Is Non Chargeable Only")]
        public bool isNonChargebackOnly { get; set; }
        
    }
}