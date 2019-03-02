using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JKViewModels.AccountsPayable
{
    public class CheckManualViewModel
    {
        [DisplayName("Check Types")]
        [Required(ErrorMessage = "{0} is Required")]
        public string CheckTypes { get; set; }
        [DisplayName("Search")]
        public string search { get; set; }
        public string searchtext { get; set; }
        [DisplayName("Pay To")]
        [Required(ErrorMessage = "{0} is Required")]
        public string payto { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Address 2")]
        public string Address2 { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [DisplayName("Zip")]
        public string Zip { get; set; }
        [DisplayName("Check Date")]
        [Required(ErrorMessage = "{0} is Required")]
        public string CheckDate { get; set; }
        [DisplayName("Check Amt.")]
        [Required(ErrorMessage = "{0} is Required")]
        public string CheckAmt { get; set; }
        [DisplayName("Memo")]
        public string memo { get; set; }

    }
}
