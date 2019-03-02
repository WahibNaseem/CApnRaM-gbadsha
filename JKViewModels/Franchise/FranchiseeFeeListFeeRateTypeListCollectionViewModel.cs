using JKViewModels.Franchisee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class FranchiseeFeeListFeeRateTypeListCollectionViewModel
    {
        public FranchiseeFeeListFeeRateTypeListCollectionViewModel()
        {
            FranchiseeFeeList = new FranchiseeFeeListViewModel();
            FeeRateTypeList = new FeeRateTypeListViewModel();
        }
        public FranchiseeFeeListViewModel FranchiseeFeeList { get; set; }

        public FeeRateTypeListViewModel FeeRateTypeList { get; set; }
    }

    public class FullFranchiseeFeeListFeeRateTypeListCollectionViewModel
    {

        public FullFranchiseeFeeListFeeRateTypeListCollectionViewModel()
        {
            FranchiseeFeeListFeeRateTypeListCollectionViewModel = new List<Franchise.FranchiseeFeeListFeeRateTypeListCollectionViewModel>();
            FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
        }
        public List<FranchiseeFeeListFeeRateTypeListCollectionViewModel> FranchiseeFeeListFeeRateTypeListCollectionViewModel { get; set; }

        public FranchiseeFeeListViewModel FranchiseeFeeListViewModel { get; set; }
    }
}
