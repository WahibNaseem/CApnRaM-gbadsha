using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class VendorViewModel
    {
        public int VendorId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> MarkUp { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
