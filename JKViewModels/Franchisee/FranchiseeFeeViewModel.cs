using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    

    public class FeesViewModel
    {
        public int FeeId { get; set; }
        public int FranchiseeFeeId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> FeeRateType { get; set; }
        public Nullable<int> FeesId { get; set; }                
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }    
    public class FeeFranchiseeFeeRateTypeListCollectionViewModel
    {
        public FeeFranchiseeFeeRateTypeListCollectionViewModel()
        {
            FeesList = new FeesViewModel();
            FranchiseeFeeList = new FranchiseeFeeListViewModel();
            FeeRateTypeList = new FeeRateTypeListViewModel();
        }
        public FeesViewModel FeesList { get; set; }
        public FranchiseeFeeListViewModel FranchiseeFeeList { get; set; }    
        public FeeRateTypeListViewModel FeeRateTypeList { get; set; }
    }

    public class FranchiseeFeeViewModel
    {
        public int FranchiseeFeeId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> FeeRateType { get; set; }
        public Nullable<int> FeesId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }

    //public class FullFeeFranchiseeFeeRateTypeListCollectionViewModel
    //{

    //    public FullFeeFranchiseeFeeRateTypeListCollectionViewModel()
    //    {
    //        FeeFranchiseeFeeRateTypeListCollectionViewModel = new List<FeeFranchiseeFeeRateTypeListCollectionViewModel>();
    //        FranchiseeFeeListViewModel = new FranchiseeFeeListViewModel();
    //    }
    //    public List<FeeFranchiseeFeeRateTypeListCollectionViewModel> FeeFranchiseeFeeRateTypeListCollectionViewModel { get; set; }

    //    public FranchiseeFeeListViewModel FranchiseeFeeListViewModel { get; set; }
    //}

}
