using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeFullFeeListFeeRateTypeViewModel
    {
        public FranchiseeFeeListViewModel FranchiseeFeeInfo { get; set; }
        public FeeRateTypeListViewModel FeeRateTypeInfo { get; set; }
    }

    public class FullFranchiseeFullFeeListFeeRateTypeViewModel
    {
       
        public List<FeeRateTypeListViewModel> FeeRateTypeInfo { get; set; }

        public FranchiseeFeeListViewModel FranchiseeFeeInfo { get; set; }
    }

    
}
