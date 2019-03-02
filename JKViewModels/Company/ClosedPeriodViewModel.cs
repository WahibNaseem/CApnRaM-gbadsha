using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class ClosedPeriodViewModel
    {
        public int PeriodClosedId { get; set; }
        public int PeriodId { get; set; }
        public int RegionId { get; set; }
        public int TransactionStatusListId { get; set; }
        public bool? ChargebackFinalized { get; set; }
        public bool? FranchiseeReport { get; set; }
        public bool? MonthlyBillRun { get; set; }
        public bool? Closed { get; set; }


        public int BillMonth { get; set; }
        public int BillYear { get; set; }

    }
}
