using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
   public class FranchiseeFeeListViewModel
    {
        public int FranchiseeFeeListId { get; set; }
        public string Name { get; set; }
        [Display(Name="Rate")]
        public Nullable<int> FeeRateTypeListId { get; set; }
        [Display(Name = "Value")]
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }

        public int? IsDelete { get; set; }

    }
}
