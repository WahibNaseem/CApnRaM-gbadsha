using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.ServiceContract
{
    interface CPeriodClosed
    {
        int PeriodClosedId { get; }
        int PeriodId { get;}
        int RegionId { get; }
        int TransactionStatusListId { get; set; }
        int ChargebackFinalized { get; set; }
        int FranchiseeReport { get; set; }
        int MonthlyBillRun { get; set; }
        int Closed { get; set; }
        int NegativeDueFinalized { get; set; }
        int BillMonth { get; set; }
        int BillYear { get; set; }




    }
}
