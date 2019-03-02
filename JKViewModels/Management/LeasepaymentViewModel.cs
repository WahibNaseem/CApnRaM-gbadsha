using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JKViewModels.Management
{
    public class LeasepaymentViewModel
    {
        [DisplayName("Search by")]
        public string Searchby { get; set; }
        [DisplayName("Search Value")]
        public string SearchValue { get; set; }
        [DisplayName("Transaction Date")]
        public string TransactionDate { get; set; }
        public string TransactionDateTo { get; set; }
    }

    public class LeaseListViewModel
    {
        public string DateSigned { get; set; }
        public string LeaseNo { get; set; }
        public string Description { get; set; }
        public string FranchiseNo { get; set; }
        public string FranchiseName { get; set; }
        public string Payments { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentTax { get; set; }
        public string Status { get; set; }
        public string SerialNo { get; set; }
        public string PaymentBilled { get; set; }
        public string TotalPayment { get; set; }

    }
}
