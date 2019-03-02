using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class DistributionReportViewModel
    {
        public int DistributionId { get; set; }
        public string  CustomerNo { get; set; }
        public string  CustomerName { get; set; }        
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }        
        public string ContractDescription { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> StatusId { get; set; }
    }
}
