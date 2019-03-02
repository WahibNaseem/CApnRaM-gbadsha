using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContractDetailDescriptionViewModel
    {
        public int ContractDetailDescriptionId { get; set; }
        public Nullable<int> ContractDetailId { get; set; }
        public Nullable<int> Recurringid { get; set; }
        public Nullable<int> Sortorder { get; set; }
        public string Description { get; set; } 
    }
}
