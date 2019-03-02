using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Commission
{
    public class CurrentScheduledCommissionViewModel
    {
        public int CommissionScheduleId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public int ContractId { get; set; }
        public decimal ContractAmount { get; set; }
        public DateTime? ContractStartDate { get; set; }        
        public int CompensationTypeListId { get; set; }
        public string CompensationTypeListDescription { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public bool? IsActive { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RegionId { get; set; }
        public decimal BonusAmount { get; set; }
        public int CompensationAmountTypeId { get; set; }
        public decimal BonusPercentage { get; set; }
        
        public decimal AdditionalBonusAmount { get; set; }

        public string RoleName { get; set; }

        public int? CommissionTrxId { get; set; }
        public int CommissionTrxTypeListId { get; set; }
    }

    public class ScheduledCommissionGenerateViewModel
    {
        public List<CurrentScheduledCommissionViewModel> lstScheduledCommission { get; set; }
        public List<CurrentPaymentHistoryCommissionViewModel> lstScheduledCommissionProcessed { get; set; }
    }

}
