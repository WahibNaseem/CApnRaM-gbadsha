using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Company
{
    public class LedgerSubAcctViewModel
    {
        public LedgerSubAcctViewModel()
        {
            IsMasterAccount = false;
            Balance = 0;
            BalanceTotal = 0;

        }
        public int LedgerSubAcctId { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public Nullable<int> GLSubAcct_Number { get; set; }
        public string GLSubAcct_Name { get; set; }
        public string Description { get; set; }
        public bool IsMasterAccount { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceTotal { get; set; }
        public string Attach { get; set; }
        public string GLTypeName { get; set; }
    }
}
