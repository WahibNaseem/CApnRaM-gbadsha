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
    public class CorporateDuesViewModel
    {
        [DisplayName("Bill Month/Year")]
        [Required(ErrorMessage = "Month is Required")]
        public string billMonth { get; set; }
        [DisplayName("Year")]
        [Required(ErrorMessage = "{0} is Required")]
        public string billYear { get; set; }
    }

    public class CorporationDuesReportViewModel
    {
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
    }

    public class NegativeDueReportViewModel
    {
        public int NegativeDueId { get; set; }
        public string CustomerNo{ get; set; }
        public string CustomerName{ get; set; }
        public string TransactionNumber { get; set; }        
        public decimal Amount { get; set; }
        public int PeriodId { get; set; }        
        public int RegionId { get; set; }
        public DateTime Date{ get; set; }
        public string Period { get; set; }

        

    }
}
