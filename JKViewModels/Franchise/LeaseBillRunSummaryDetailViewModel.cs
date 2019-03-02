using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class LeaseBillRunSummaryDetailViewModel
    {
        public long BatchNumber { get; set; }
        public decimal TotalLeaseCount { get; set; }
        public string TotalLeaseAmount { get; set; }
        public string TotalLeaseTax { get; set; }
        public string LeaseCreatedOn { get; set; }
    }
}
