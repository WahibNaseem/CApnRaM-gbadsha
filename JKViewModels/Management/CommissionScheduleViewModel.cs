using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class CommissionScheduleViewModel
    {
        public int CommissionScheduleId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerAddress { get; set; }
        public int ContractId { get; set; }
        public decimal ContractAmount { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int CompensationTypeListId { get; set; }
        public string CompensationTypeListDescription { get; set; }
        public int CommissionCompensationScheduleId { get; set; }
        public string CommissionCompensationScheduleDescription { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RegionId { get; set; }
        public int? ContractTypeListId { get; set; }
        public string ContractTypeListName { get; set; }
        public int TransactionStatusListId { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusExplanation { get; set; }
        public string BonusDescription { get; set; }

    }

    public class CommissionScheduleListViewModel
    {
        public List<CommissionScheduleViewModel> lstSchedules { get; set; }
        public List<CommissionScheduleViewModel> lstContracts { get; set; }
    }

    public class BonusViewModel
    {
        public int BonusId { get; set; }       
        public int SaleAE_UserId { get; set; }
        public string SaleAE_Name { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RegionId { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusExplanation { get; set; }
        public string BonusDescription { get; set; }

        public bool? IsAdditionalBonus { get; set; }
        public int? CommissionScheduleId { get; set; }
        public int PeriodId { get; set; }

    }
}
