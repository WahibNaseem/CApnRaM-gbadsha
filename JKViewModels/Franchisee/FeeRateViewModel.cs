using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FeeRateViewModel
    {
        public int FeeRateId { get; set; }
        public Nullable<int> Name { get; set; }
        public Nullable<decimal> Rate { get; set; }
    }
}
