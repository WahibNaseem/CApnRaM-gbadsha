using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class LadgerAccountViewModel
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public int AmountTypeListId { get; set; }
        public string AmountTypName { get; set; }

        public List<LadgerSubAccountViewModel> LadgerSubAccounts { get; set; }
    }


    public class LadgerSubAccountViewModel
    {
        public int SubAccountId { get; set; }
        public int AccountId { get; set; }
        public string SubAccountNumber { get; set; }
        public string SubAccountName { get; set; }
        public string SubAccountDescription { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }       
    }

    public class LadgerAccountDetailViewResult
    {
        public int MasterTrxId { get; set; }
        public Nullable<int> TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionDescription { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public string TypeListName { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public string LedgerAcctNumber { get; set; }
        public string LedgerAcctName { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public string LedgerSubAcctNumber { get; set; }
        public string LedgerSubAcctName { get; set; }
        public string Payee_Payer { get; set; }

        public string Number { get; set; }
    }
}
