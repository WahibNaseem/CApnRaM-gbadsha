using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class ARLogViewModel
    {
        [DisplayName("Date Type")]
        public string dateType { get; set; }
        [DisplayName("Date Range")]
        //[DataType(DataType.Date)]
        public string startDate { get; set; }
        //[DisplayName("")]
        //[DataType(DataType.Date)]
        public string endDate { get; set; }

        [DisplayName("Customer No")]
        public string customerNo { get; set; }
        [DisplayName("Customer Name")]
        public string customerName { get; set; }
        [DisplayName("Invoice No")]
        public string invoiceNo { get; set; }
        [DisplayName("Reference No")]
        public string referenceNo { get; set; }
        [DisplayName("Date")]
        public string date { get; set; }
        [DisplayName("Total")]
        public decimal total { get; set; }



    }
}