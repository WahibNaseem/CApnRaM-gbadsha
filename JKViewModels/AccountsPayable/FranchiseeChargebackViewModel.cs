using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    public class FranchiseeChargebackTransactionViewModel
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<string> FranchiseeChargebackTrxList { get; set; }

        public List<FranchiseeChargebackViewModel> Chargebacks { get; set; }


    }

    public class FranchiseeChargebackViewModel
    {
        public int FranchiseeId { get; set; }
        public int RegionId { get; set; }

        public int BillMonth { get; set; }
        public int BillYear { get; set; }

        public List<FranchiseeChargebackDetailViewModel> Distributions { get; set; }
    }

    public class FranchiseeChargebackDetailViewModel
    {
        public int InvoiceId { get; set; }
        public int MasterTrxDetailId { get; set; }
        public int LineNo { get; set; }
        public decimal Balance { get; set; }
    }

    public class TurnAroundTransactionViewModel
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<string> FranchiseeTurnAroundTrxList { get; set; }

        public List<TurnAroundTransactionViewModel> TurnArounds { get; set; }


    }
    public class APTurnAroundListModel
    {
        public string RegionName { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> ChargeBackDate { get; set; }
        public Nullable<decimal> ChargeBackAmount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<decimal> ChargeBackPayAmount { get; set; }
        public int TurnAroundId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<System.DateTime> TARCBDate { get; set; }
        public decimal NegativeDueAmount { get; set; }
    }

}
