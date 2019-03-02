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
    public class RevenueViewModel
    {
        [DisplayName("Bill Month/Year")]
        [Required(ErrorMessage = "Month is Required")]
        public string billMonth { get; set; }
        [DisplayName("Year")]
        [Required(ErrorMessage = "{0} is Required")]
        public string billYear { get; set; }
    }

    public class RevenueListItem
    {
        public string Region_Name { get; set; }
        public string BillingSubTotal { get; set; }
        public string BillingTax { get; set; }
        public string BillingTotal { get; set; }
        public string SupplySubTotal { get; set; }
        public string SupplyTax { get; set; }
        public string SupplyTotal { get; set; }
        public string TotalTax { get; set; }
        public string Total { get; set; }
    }
}
