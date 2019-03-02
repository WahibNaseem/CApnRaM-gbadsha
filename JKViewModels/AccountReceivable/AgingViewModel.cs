using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class AgingViewModel
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

        // data table columns

        [DisplayName("Id")]
        public string id { get; set; }
        [DisplayName("Franchise Id")]
        public string franchiseId { get; set; }
        [DisplayName("Franchise No")]
        public string franchiseNo { get; set; }
        [DisplayName("Franchise Name")]
        public string franchiseName { get; set; }
        [DisplayName("Customer Id")]
        public string customerId { get; set; }
        [DisplayName("Customer No")]
        public string customerNo { get; set; }
        [DisplayName("Customer Name")]
        public string customerName { get; set; }
        [DisplayName("Phone")]
        public string phone { get; set; }
        [DisplayName("Inv Date")]
        //[DataType(DataType.Date)]
        public string invDate { get; set; }
        [DisplayName("Inv Number")]
        public string invNumber { get; set; }
        [DisplayName("Due Date")]
        public string dueDate { get; set; }
        [DisplayName("Invoice Total")]
        public string totalAmount { get; set; }
        [DisplayName("Current")]
        public string onemo { get; set; }
        [DisplayName("1-30")]
        public string twomo { get; set; }
        [DisplayName("31-60")]
        public string threemo { get; set; }
        [DisplayName("61-90")]
        public string fourmo { get; set; }
        [DisplayName("91+")]
        public string fivemo { get; set; }
        

    }
}