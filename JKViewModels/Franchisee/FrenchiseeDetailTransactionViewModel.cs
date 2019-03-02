using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FrenchiseeDetailTransactionViewModel
    {
        public string Number { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int MasterTrxId { get; set; }
        public Nullable<int> TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
}
