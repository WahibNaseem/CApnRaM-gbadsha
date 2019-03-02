using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Service.Administration.Company
{
    public class BBankStatmentViewModel
    {
        public Nullable<DateTime> TransactionDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string TrxType { get; set; }
        public string PayeeNo { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Code { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Balance { get; set; }

        public string LegerAccount { get; set; }

    }
}
