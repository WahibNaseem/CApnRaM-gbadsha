using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class CommonFranchiseeAccountHistoryReportViewModel
    {
        public FranchiseeDetailViewModel FranchiseeDetail { get; set; }
        public List<FranchiseeAccountHistoryReportViewModel> lstFranchiseeAccountHistoryReport { get; set; }
    }
    public class FranchiseeAccountHistoryReportViewModel
    {
        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public decimal ContractAmount { get; set; } //(decimal(18,2), null)
        public int IBCreditAmount { get; set; } //(int, not null)
        public int FFCreditAmount { get; set; } //(int, not null)
        public decimal FFTotalDueAmount { get; set; } //(decimal(38,2), null)
        public decimal FFPaidAmount { get; set; } //(decimal(38,2), null)
        public int FFHold { get; set; } //(int, not null)
        public int DaysKept { get; set; } //(int, not null)
        public int? StatusListId { get; set; } //(int, null)
        public string StatusName { get; set; } //(varchar(250), null)
        public int? StatusId { get; set; } //(int, null)
        public bool isActive { get; set; } //(bit, null)
        public DateTime? StartDate { get; set; }
    }

}
