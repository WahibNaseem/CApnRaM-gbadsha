using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeContractTypeListViewModel
    {
        public int FranchiseeContractTypeListId { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> BusinessAmount { get; set; }
        public Nullable<decimal> DownPayment { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<int> NoOfPayments { get; set; }
        public Nullable<int> DaysToFullfill { get; set; }
    }
}
