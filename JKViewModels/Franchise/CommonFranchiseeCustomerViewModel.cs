using JKViewModels.Franchisee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{


    public class CommonFranchiseeCustomerViewModel
    {
        public FCDetailViewModel CustomerDetail { get; set; }
        public List<FCContractDetailViewModel> lstContractDetail { get; set; }
        public FCFranchiseeDistributionViewModel FranchiseeDistribution { get; set; }
        public List<FCFranchiseeDistributionViewModel> lstFranchiseeDistribution { get; set; }
        public List<FCFranchiseeDistributionFeeViewModel> lstFranchiseeDistributionFee { get; set; }        
        public FCFindersFeeViewModel FindersFee { get; set; }
        public List<FCFindersFeeAdjustmentViewModel> lstFindersFeeAdjustment { get; set; }
        public List<FindersFeeScheduleViewModel> lstFindersFeeSchedule { get; set; }
    }
    
    public class FCDetailViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)        
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int MaintenanceTypeListId { get; set; } //(int, not null)
        public int StatusListId { get; set; } //(int, null)
        public int StatusReasonListId { get; set; } //(int, null)
        public DateTime? EffectiveDate { get; set; } //(date, null)
        public string CreatedBy { get; set; } //(varchar(50), null)
        public DateTime? CreatedDate { get; set; } //(datetime, null)
        public int RegionId { get; set; } //(int, null)

        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public string C_Address { get; set; } //(varchar(50), null)
        public string C_City { get; set; } //(varchar(50), null)
        public string C_StateName { get; set; } //(varchar(50), null)
        public string C_PostalCode { get; set; } //(varchar(50), null)

        //Franchisee Info
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public string F_Address { get; set; } //(varchar(125), null)
        public string F_City { get; set; } //(varchar(50), null)
        public string F_StateName { get; set; } //(varchar(250), null)
        public string F_PostalCode { get; set; } //(varchar(20), null)

        public int ContractId { get; set; } //(int, not null)
        public int? ContractTypeListId { get; set; }
        public string PONumber { get; set; } //(varchar(20), null)
        public string AccountTypeName { get; set; } //(varchar(75), null)
        public Nullable<int> ContractTermMonth { get; set; }
        public DateTime? StartDate { get; set; } //(date, null)
        public DateTime? ExpirationDate { get; set; } //(date, null)
        public decimal? TotalAmount { get; set; } //(decimal(18,2), null)
        public string Status { get; set; } //(varchar(250), not null)


        public int HasActiveContract { get; set; }
        public int HasActiveContractDetail { get; set; }
        public int HasActiveDistribution { get; set; }        


    }
    public class FCContractDetailViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)        
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int ContractDetailId { get; set; } //(int, not null)
        public int ContractId { get; set; } //(int, null)
        public int LineNumber { get; set; } //(int, null)
        public string ServiceTypeName { get; set; } //(varchar(50), not null)
        public string BillingFrequencyListName { get; set; } //(varchar(75), null)
        public decimal Amount { get; set; } //(decimal(18,2), null)
        public DateTime? StartTime { get; set; } //(datetime, null)
        public bool Mon { get; set; } //(bit, null)
        public bool Tues { get; set; } //(bit, null)
        public bool Wed { get; set; } //(bit, null)
        public bool Thur { get; set; } //(bit, null)
        public bool Fri { get; set; } //(bit, null)
        public bool Sat { get; set; } //(bit, null)
        public bool Sun { get; set; } //(bit, null)        
    }
    public class FCFranchiseeDistributionViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)     
        public int MaintenanceTempId { get; set; } //(int, not null)           
        public int DistributionId { get; set; } //(int, not null)
        public string DetailLineNumber { get; set; } //(int, null)
        public int ContractDetailId { get; set; } //(int, null)
        public int FranchiseeId { get; set; } //(int, null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public string FranchiseeAddress { get; set; }
        public string FranchiseeCity { get; set; }
        public string FranchiseeState { get; set; }
        public string FranchiseePostalCode { get; set; }
        public decimal? Amount { get; set; } //(decimal(12,2), null)
        public int TotalDistributionLines { get; set; }
        public DateTime? StartDate { get; set; } //(date, null)
    }

   
    public class FCFranchiseeDistributionFeeViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)     
        public int MaintenanceTempId { get; set; } //(int, not null)           
        public int DistributionFeesId { get; set; }
        public int DistributionId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ContractDetailId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> DetailLineNumber { get; set; }
        public int FeeId { get; set; }
        public string FeeName { get; set; }
        public Nullable<int> FeeRateTypeListId { get; set; }
        public string FeeRateTypeName { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    public class FCFindersFeeViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)   
        public int MaintenanceTempId { get; set; } //(int, not null)     
        public int FranchiseeId { get; set; } //(int, not null)     
        public int CustomerId { get; set; } //(int, null)
        public int FindersFeeId { get; set; } //(int, not null)
        public int StatusListId { get; set; } //(int, null)
        public string StatusListName { get; set; } //(varchar(125), null)
        public DateTime? StartDate { get; set; } //(date, null)
        public DateTime? ResumeDate { get; set; } //(date, null)
        public string Description { get; set; } //(varchar(max), null)
        public int? FindersFeeTypeListId { get; set; } //(int, not null)
        public string FindersFeeTypeListName { get; set; } //(char(150), not null)
        public decimal? ContractBillingAmount { get; set; } //(decimal(12,2), null)
        public decimal? TotalAdjustmentAmount { get; set; } //(decimal(12,2), null)
        public decimal? Factor { get; set; } //(decimal(8,2), null)
        public decimal? DownPayPercentage { get; set; } //(decimal(12,2), null)
        public decimal? Interest { get; set; } //(decimal(12,2), null)
        public int? TotalNumOfpayments { get; set; } //(int, null)
        public decimal? MonthlyPaymentAmount { get; set; } //(decimal(12,2), null)
        public decimal? FinancedAmount { get; set; } //(decimal(12,2), null)
        public decimal? DownPaymentAmount { get; set; } //(decimal(12,2), null)
        public decimal? TotalAmount { get; set; } //(decimal(12,2), null)
        public string Notes { get; set; } //(varchar(max), null)
        public decimal? MultiTenantOccupancyAmount { get; set; } //(decimal(12,2), null)
        public decimal? PaidAmount { get; set; } //(decimal(12,2), null)
        public decimal? BalanceAmount { get; set; } //(decimal(12,2), null)
        public decimal? PayableOnAmount { get; set; } //(decimal(12,2), null)
        public int? RegionId { get; set; } //(int, null)
        public decimal? InterestAmount { get; set; } //(decimal(12,2), null)
        public decimal? MonthlyPaymentPercentage { get; set; } //(decimal(12,2), null)
        public bool IncludeDownPayInFirstPay { get; set; } //(int, null)
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? DistributionId { get; set; } //(int, null)


    }
    public class FCFindersFeeAdjustmentViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)    
        public int MaintenanceTempId { get; set; } //(int, not null)    
        public int FindersFeeAdjustmentId { get; set; }
        public Nullable<int> FindersFeeId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> FindersFeeAdjustmentTypeListId { get; set; }
        public string FindersFeeAdjustmentTypeListName { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }

    public class FranCallModel
    {
        public int FranCallId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<System.DateTime> call_date { get; set; }
        public Nullable<System.TimeSpan> call_time { get; set; }
        public string call_stat { get; set; }
        public Nullable<int> StatusResultListId { get; set; }
        public string StatusResultListName{ get; set; }
        public string spoke_with { get; set; }
        public string call_back { get; set; }
        public string call_btime { get; set; }
        public string action { get; set; }
        public string action_otr { get; set; }
        public string init_call { get; set; }
        public string comments { get; set; }
    }

}

