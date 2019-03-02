using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Commission
{
    public class CurrentPaymentHistoryCommissionViewModel
    {            
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public int SalePersonId { get; set; }
        public string SalePersonName { get; set; }
        public string SalePersonRoleName { get; set; }
        public int ContractId { get; set; }
        public decimal ContractAmount { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public decimal? InitialContract { get; set; }
        public decimal? CurrentBilling { get; set; }
        public decimal? OpenAR { get; set; }
        public decimal? CurrentCredits { get; set; }
        public int? CommissionScheduleId { get; set; }
        public int? CommissionCompensationScheduleId { get; set; }        
        public string CommissionCompensationScheduleDescription { get; set; }
        public int CompensationTypeListId { get; set; }
        public string CompensationTypeListDescription { get; set; }
        public int? PaymentNumber { get; set; }
        public int? TotalPaymentNumber { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public decimal? CommissionAmount { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal? BonusPercentage { get; set; }
        public decimal? AdditionalBonusAmount { get; set; }
        public bool? IsActive { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public int RegionId { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }


    
}
