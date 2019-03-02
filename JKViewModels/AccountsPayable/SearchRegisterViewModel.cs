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
    public class SearchRegisterViewModel
    {
        [DisplayName("Search")]
        public string search { get; set; }
        [DisplayName("Search Value")]
        public string SearchValue { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; }
        [DisplayName("Transaction Date")]
        public string TransactionDate { get; set; }
        [DisplayName("To")]
        public string TransactionDateTo { get; set; }
        [DisplayName("Check #")]
        public string Check { get; set; }
        [DisplayName("To")]
        public string CheckTo { get; set; }
        [DisplayName("Filter by")]
        public string Filterby { get; set; }
        
    }
}
