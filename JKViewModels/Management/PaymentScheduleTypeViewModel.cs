using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class CompensationTypeViewModel
    {
        public int CompensationTypeListId { get; set; } //(int, not null)
        public string Description { get; set; } //(varchar(500), null)
        public int StatusListId { get; set; } //(int, not null)
        public string StatusListName { get; set; } //(varchar(250), null)
        public bool IncludeinTotalSales { get; set; } //(bit, null)
        public bool VariableSales { get; set; } //(bit, null)
        public bool CommissionBasedonTotalSale { get; set; } //(bit, null)
        public bool UserSpecific { get; set; } //(bit, null)
        public bool StartDateSpecific { get; set; } //(bit, null)
        public bool IsActive { get; set; } //(bit, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(int, null)
        public int? ModifiedBy { get; set; } //(int, null)
        public DateTime? ModifiedDate { get; set; } //(int, null)
        public int? RegionId { get; set; }
    }
}
