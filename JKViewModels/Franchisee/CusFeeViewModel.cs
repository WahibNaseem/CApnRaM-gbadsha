using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class CusFeeViewModel
    {
        public int FeesId { get; set; }
        public Nullable<int> Name { get; set; }
        public Nullable<int> FeeRateId { get; set; }
        public Nullable<int> RateType { get; set; }
        public Nullable<decimal> StartRange { get; set; }
        public Nullable<decimal> EndRange { get; set; }
        public Nullable<int> StatusId { get; set; }
    }
}
