using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FindersFeeAdjustmentViewModel
    {
        public int FindersFeeAdjustmentId { get; set; }

        public Nullable<int> FindersFeeId { get; set; }

        public Nullable<int> FranchiseeId { get; set; }

        public Nullable<int> FindersFeeAdjustmentTypeListId { get; set; }

        public string Description { get; set; }

        public Nullable<decimal> Amount { get; set; }

        public Nullable<int> RegionId { get; set; }

        public Nullable<int> CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public Nullable<int> ModifiedBy { get; set; }

        public Nullable<System.DateTime> ModifiedDate { get; set; }
        
    }
}
