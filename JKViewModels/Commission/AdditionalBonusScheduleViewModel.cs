using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Commission
{
    public class AdditionalBonusScheduleViewModel
    {
       
        public int CommissionAdditionalBonusScheduleId { get; set; }
        public decimal RangeStartAmount { get; set; }
        public decimal RangeEndAmount { get; set; }
        public decimal Amount { get; set; }
        public int StatusListId { get; set; } //(int, not null)
        public string StatusListName { get; set; } //(varchar(250), null)
        public bool IsActive { get; set; } //(bit, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(int, null)
        public int? ModifiedBy { get; set; } //(int, null)
        public DateTime? ModifiedDate { get; set; } //(int, null)
        public int? RegionId { get; set; }
   
    }
}
