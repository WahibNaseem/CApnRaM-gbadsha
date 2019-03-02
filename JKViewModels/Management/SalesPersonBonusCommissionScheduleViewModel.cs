using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class SalesPersonBonusCommissionScheduleViewModel
    {
        public int SalesPersonBonusCommissionScheduleId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int CommissionAdditionalBonusScheduleId { get; set; }
        public string CommissionAdditionalBonusScheduleDescription { get; set; }
        public int ContractTypeId { get; set; }
        public string ContractType { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public int BonusAmountTypeId { get; set; }
        public decimal BonusAmount { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RegionId { get; set; }        
    }   
}
