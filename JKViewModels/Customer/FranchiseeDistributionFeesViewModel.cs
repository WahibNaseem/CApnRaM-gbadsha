using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public partial class FranchiseeDistributionFeesViewModel
    {
        public int DistributionFeesId { get; set; }
        public int DistributionId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ContractDetailId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> DetailLineNumber { get; set; }
        public int FeeId { get; set; }
        public string FeeName { get; set; }
        public Nullable<int> FeeRateTypeListId { get; set; }
        public string FeeRateTypeName { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
