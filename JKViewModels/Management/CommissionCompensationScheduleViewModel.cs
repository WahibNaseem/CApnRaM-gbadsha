using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class CommissionCompensationScheduleViewModel
    {
        public int CommissionCompensationScheduleId { get; set; } //(int, not null)
        public int CompensationTypeListId { get; set; }
        public string CompensationTypeListDescription { get; set; }
        public string Description { get; set; } //(varchar(500), null)
        public decimal RangeStartAmount { get; set; }
        public decimal RangeEndAmount { get; set; }
        public decimal CompensationAmount { get; set; }
        public int CommissionPaymentScheduleId { get; set; }
        public string CommissionPaymentScheduleDescription { get; set; }
        public int StatusListId { get; set; } //(int, not null)
        public string StatusListName { get; set; } //(varchar(250), null)
        public bool IsActive { get; set; } //(bit, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(int, null)
        public int? ModifiedBy { get; set; } //(int, null)
        public DateTime? ModifiedDate { get; set; } //(int, null)
        public int? RegionId { get; set; }
        public int? CompensationAmountTypeId { get; set; }
    }


    public class CompensationAmountTypeViewModel
    {
        public int CompensationAmountTypeId { get; set; } //(int, not null)
        public string Name { get; set; }
    }

    public class CommissionAdditionalBonusScheduleViewModel
    {
        public int CommissionAdditionalBonusScheduleId { get; set; } //(int, not null)
        public string Description { get; set; }
    }
}
