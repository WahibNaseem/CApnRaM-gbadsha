using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    public class APBillTransactionViewModel
    {
        public int RegionId { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public APBillViewModel APBill { get; set; }
    }

    public class APBillViewModel
    {
        public int TypeListId { get; set; }
        public int ClassId { get; set; }

        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public int CheckBookTransactionTypeListId { get; set; }
        public bool IsManual { get; set; }

        public int FranchiseeReportId { get; set; }
        public int FranchiseeTurnaroundCheckId { get; set; }
        public int ManualCheckId { get; set; }
        public int TransactionStatusListId { get; set; }
        public int PeriodId { get; set; }
        public decimal CheckAmount { get; set; }
        public int PayTypeListId { get; set; }

        public APBillDetailViewModel Detail { get; set; }
    }

    public class APBillDetailViewModel
    {
        public int MasterTrxDetailId { get; set; }
        public decimal Amount { get; set; }
    }
}
