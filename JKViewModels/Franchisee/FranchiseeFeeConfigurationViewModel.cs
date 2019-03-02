using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeFeeConfigurationViewModel
    {
        public int FeeConfigurationId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> FeeId { get; set; }
        public Nullable<decimal> MinimumAmount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> FeeConfigurationImpId { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class FranchiseeFeeConfigurationCustomModel
    {
        public int FeeConfigurationId { get; set; }
        public string MinimumAmount { get; set; }
        public string  EffectiveDate { get; set; }

    }

}
