using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{

    public class IncreaseInvoiceItemViewModel
    {
        public int ServiceListId { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemAmount { get; set; }
    }
    public class CommonCustomerIncreaseDecreaseDetailViewModel
    {
        public CIDDetailViewModel CustomerIncreaseDecreaseDetail { get; set; }
        public List<CIDContractDetailViewModel> lstContractDetail { get; set; }
        public List<CIDFranchiseeDistributionViewModel> lstFranchiseeDistribution { get; set; }
        public List<CIDFindersFeeViewModel> lstFindersFee { get; set; }
        public List<CIDFindersFeeAdjustmentViewModel> lstFindersFeeAdjustment { get; set; }

        public List<CMCreditInvoiceItemsViewModel> lstCreditInvoiceItem { get; set; }
        public List<CMCreditBillingPayViewModel> lstCreditBillingPay { get; set; }
    }


    //public class CustomerIncreaseDecreaseViewModel
    //{
    //    public int MasterCustomerTransferTempId { get; set; } //(int, not null)
    //    public int CustomerId { get; set; } //(int, null)
    //    public string CustomerNo { get; set; } //(varchar(25), null)
    //    public string CustomerName { get; set; } //(varchar(125), null)
    //    public int TransferReasonId { get; set; } //(int, null)
    //    public string TransferReasonName { get; set; } //(varchar(100), null)
    //    public DateTime EffectiveDate { get; set; } //(date, null)
    //    public int RegionId { get; set; } //(int, null)
    //    public string RegionName { get; set; } //(varchar(30), null)
    //}

    
    public class CIDDetailViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)        
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int MaintenanceTypeListId { get; set; } //(int, not null)
        public int StatusListId { get; set; } //(int, null)
        public int StatusReasonListId { get; set; } //(int, null)
        public string Reason { get; set; } //(varchar(50), null)
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
        public int ContractId { get; set; } //(int, not null)
        public string PONumber { get; set; } //(varchar(20), null)
        public int AccountTypeListId { get; set; } //(int, null)
        public string AccountTypeName { get; set; } //(varchar(75), null)
        public DateTime? StartDate { get; set; } //(date, null)
        public DateTime? ExpirationDate { get; set; } //(date, null)
        public decimal? TotalAmount { get; set; } //(decimal(18,2), null)
        public decimal? OTotalAmount { get; set; } //(decimal(18,2), null)
        public string Status { get; set; } //(varchar(250), not null)
        public string StatusReason { get; set; } //(varchar(250), not null)
        public string StatusName { get; set; } //(varchar(250), not null)
        public bool Qualified { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<int> ContractTermMonth { get; set; }
        public string ContractDescription { get; set; }
        public Nullable<int> ContractTypeListId { get; set; }        
    }
    public class CIDContractDetailViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)        
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int ContractDetailId { get; set; } //(int, not null)
        public int ContractId { get; set; } //(int, null)
        public int LineNumber { get; set; } //(int, null)
        public int ServiceTypeListId { get; set; } //(int, null)
        public string ServiceTypeName { get; set; } //(varchar(50), not null)
        public int BillingFrequencyListId { get; set; } //(int, null)
        public string BillingFrequencyListName { get; set; } //(varchar(75), null)
        public string SquareFootage { get; set; } //(varchar(12), null)
        public decimal Amount { get; set; } //(decimal(18,2), null)
        public decimal OAmount { get; set; } //(decimal(18,2), null)
        public DateTime? StartTime { get; set; } //(datetime, null)
        public DateTime? EndTime { get; set; } //(datetime, null)
        public int CleanTimes { get; set; } //(int, null)
        public int CleanFrequencyListId { get; set; } //(int, null)
        public string CleanFrequencyListName { get; set; } //(varchar(125), null)
        public int CustomerId { get; set; } //(int, null)
        public bool Mon { get; set; } //(bit, null)
        public bool Tues { get; set; } //(bit, null)
        public bool Wed { get; set; } //(bit, null)
        public bool Thur { get; set; } //(bit, null)
        public bool Fri { get; set; } //(bit, null)
        public bool Sat { get; set; } //(bit, null)
        public bool Sun { get; set; } //(bit, null)
        public bool CPIIncrease { get; set; } //(bit, null)
        public bool SeparateInvoice { get; set; } //(int, null)
        public string Description { get; set; } //(varchar(max), null)
        public string Notes { get; set; } //(varchar(max), null)
    }
    public class CIDFranchiseeDistributionViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)     
        public int MaintenanceTempId { get; set; } //(int, not null)           
        public int DistributionId { get; set; } //(int, not null)
        public int DetailLineNumber { get; set; } //(int, null)
        public int CustomerId { get; set; } //(int, not null)
        public int ContractDetailId { get; set; } //(int, null)
        public int FranchiseeId { get; set; } //(int, null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public decimal? Amount { get; set; } //(decimal(12,2), null)
        public decimal? ContractDetailAmount { get; set; } //(decimal(18,2), null)
        public decimal? DPercentage { get; set; } //(decimal(18,2), null)
        

    }
    public class CIDFindersFeeViewModel
    {
        public int MaintenanceTempDetailId { get; set; } //(int, not null)   
        public int MaintenanceTempId { get; set; } //(int, not null)     
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public string F_Address { get; set; } //(varchar(125), null)
        public string F_City { get; set; } //(varchar(50), null)
        public string F_StateName { get; set; } //(varchar(250), null)
        public string F_PostalCode { get; set; } //(varchar(20), null)
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
    }
    public class CIDFindersFeeAdjustmentViewModel
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

}

