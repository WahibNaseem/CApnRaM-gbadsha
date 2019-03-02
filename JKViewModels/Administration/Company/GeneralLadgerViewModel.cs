using System;

namespace JKViewModels.Administration.Company
{
    public class GeneralLadgerViewModel
    {
        public int GeneralLedgerId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
    }
}
