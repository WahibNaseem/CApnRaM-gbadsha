using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerSearchModel
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public string CustomerNo { get; set; }

        public bool? ConsolidatedInvoice { get; set; }
        public bool? HasChild { get; set; }

        public int? StatusListId { get; set; }
        public string StatusName { get; set; }
        public int? ParentId { get; set; }
    }
}
