using System;

namespace JKViewModels.Administration.Company
{
    public class GeneralLadgerAccountListViewModel
    {
        public int LedgerAcctId { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public string AccountNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceTotal { get; set; }

        public bool IsMasterAccount { get; set; }
        public string Attach { get; set; }
    }

    public class CommonLadgerAccountViewModel
    {
        public int AccountId { get; set; }
        public Nullable<int> PerentAccountId { get; set; }
        public string AccountNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public bool IsSubAccount { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Attach { get; set; }
    }
}
