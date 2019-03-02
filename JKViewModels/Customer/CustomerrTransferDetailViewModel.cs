using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{


    public class CommonCustomerrTransferViewModel
    {
        public CTMaintenanceViewModel CustomerIncreaseDecreaseDetail { get; set; }
        public List<CTContractDetailViewModel> lstContractDetail { get; set; }
        public List<CTFranchiseeDistributionViewModel> lstFranchiseeDistribution { get; set; }
      
    }

    public class CTMaintenanceViewModel
    {
        public int MaintenanceTempDetailId { get; set; }      
        public int MaintenanceTempId { get; set; }
        public int MaintenanceTypeListId { get; set; }
        public int StatusListId { get; set; }
        public string StatusName { get; set; } 
        public int StatusReasonListId { get; set; } 
        public string StatusReason { get; set; } 
        public string Reason { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int RegionId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string C_Address { get; set; }
        public string C_City { get; set; }
        public string C_StateName { get; set; }
        public string C_PostalCode { get; set; }

        public int ContractId { get; set; }
        public string PONumber { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountTypeName { get; set; } 
        public Nullable<int> ContractTermMonth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Status { get; set; }
        
        public bool Qualified { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public string ContractDescription { get; set; }
        public Nullable<int> ContractTypeListId { get; set; }

    }
    public class CTContractDetailViewModel
    {
        public int MaintenanceTempDetailId { get; set; }
        public int MaintenanceTempId { get; set; }
        public int ContractDetailId { get; set; }
        public int ContractId { get; set; }
        public int LineNumber { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceTypeName { get; set; }
        public int BillingFrequencyListId { get; set; }
        public string BillingFrequencyListName { get; set; }
        public string SquareFootage { get; set; }
        public decimal Amount { get; set; }
        public DateTime? StartTime { get; set; } 
        public DateTime? EndTime { get; set; }
        public int CleanTimes { get; set; }
        public int CleanFrequencyListId { get; set; }
        public string CleanFrequencyListName { get; set; }
        public int CustomerId { get; set; }
        public bool Mon { get; set; }
        public bool Tues { get; set; }
        public bool Wed { get; set; }
        public bool Thur { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public bool CPIIncrease { get; set; }
        public bool SeparateInvoice { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }
    public class CTFranchiseeDistributionViewModel
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

    //public class CommonCustomerrTransferDetailViewModel
    //{
    //    public CTDetailViewModel CustomerIncreaseDecreaseDetail { get; set; }
    //    public List<CTContractDetailViewModel> lstContractDetail { get; set; }
    //    public List<CTFranchiseeDistributionViewModel> lstFranchiseeDistribution { get; set; }
    //    public List<CTFindersFeeViewModel> lstFindersFee { get; set; }
    //    public List<CTFindersFeeAdjustmentViewModel> lstFindersFeeAdjustment { get; set; }
    //}

    //public class CTDetailViewModel
    //{
    //    public int MaintenanceTempDetailId { get; set; } //(int, not null)        
    //    public int MaintenanceTempId { get; set; } //(int, not null)
    //    public int MaintenanceTypeListId { get; set; } //(int, not null)
    //    public int StatusListId { get; set; } //(int, null)
    //    public int StatusReasonListId { get; set; } //(int, null)
    //    public string Reason { get; set; } //(varchar(50), null)
    //    public DateTime? EffectiveDate { get; set; } //(date, null)
    //    public string CreatedBy { get; set; } //(varchar(50), null)
    //    public DateTime? CreatedDate { get; set; } //(datetime, null)
    //    public int RegionId { get; set; } //(int, null)
    //    public int CustomerId { get; set; } //(int, not null)
    //    public string CustomerNo { get; set; } //(varchar(25), null)
    //    public string CustomerName { get; set; } //(varchar(125), null)
    //    public string C_Address { get; set; } //(varchar(50), null)
    //    public string C_City { get; set; } //(varchar(50), null)
    //    public string C_StateName { get; set; } //(varchar(50), null)
    //    public string C_PostalCode { get; set; } //(varchar(50), null)
    //    public int ContractId { get; set; } //(int, not null)
    //    public string PONumber { get; set; } //(varchar(20), null)
    //    public int AccountTypeListId { get; set; } //(int, null)
    //    public string AccountTypeName { get; set; } //(varchar(75), null)
    //    public DateTime? StartDate { get; set; } //(date, null)
    //    public DateTime? ExpirationDate { get; set; } //(date, null)
    //    public decimal? TotalAmount { get; set; } //(decimal(18,2), null)
    //    public string Status { get; set; } //(varchar(250), not null)
    //    public string StatusReason { get; set; } //(varchar(250), not null)
    //    public string StatusName { get; set; } //(varchar(250), not null)
    //    public bool Qualified { get; set; }
    //    public Nullable<System.DateTime> SignDate { get; set; }
    //    public Nullable<int> ContractTermMonth { get; set; }
    //    public string ContractDescription { get; set; }
    //    public Nullable<int> ContractTypeListId { get; set; }
    //}   



}

