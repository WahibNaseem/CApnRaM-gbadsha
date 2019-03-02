using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{


    public class CommonCustomerrTransferPendingDetailViewModel
    {
        public CustomerrTransferPendingDetailViewModel CustomerrTransferPendingDetail { get; set; }
        public List<CTPDetailContractDetailViewModel> lstContractDetail { get; set; }
        public List<CTPDetailFranchiseeDistributionViewModel> lstFranchiseeDistribution { get; set; }
        public List<CTPDetailFranchiseeStopFindersFeeViewModel> lstFranchiseeStopFindersFee { get; set; }
        public List<CTPDetailInvoiceFranchiseeDistributionViewModel> lstRevenueDistribution { get; set; }
        public List<CTPDetailInvoiceFranchiseeDistributionViewModel> lstRevenueDistributionOld { get; set; }

        public List<CTPDetailInvoiceFranchiseeDistributionViewModel> lstRevenueDistributionCombine { get; set; }

        public List<CIDFindersFeeViewModel> lstFindersFee { get; set; }
        public List<CIDFindersFeeAdjustmentViewModel> lstFindersFeeAdjustment { get; set; }
    }
    public class CustomerTransferPendingViewModel
    {
        public int MasterCustomerTransferTempId { get; set; } //(int, not null)
        public int CustomerId { get; set; } //(int, null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public int TransferReasonId { get; set; } //(int, null)
        public string TransferReasonName { get; set; } //(varchar(100), null)
        public DateTime EffectiveDate { get; set; } //(date, null)
        public DateTime LastServiceDate { get; set; } //(date, null)
        public int RegionId { get; set; } //(int, null)
        public string RegionName { get; set; } //(varchar(30), null)
    }    
    public class CustomerrTransferPendingDetailViewModel
    {
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int StatusListId { get; set; } //(int, null)
        public int StatusReasonListId { get; set; } //(int, null)
        public DateTime EffectiveDate { get; set; } //(date, null)
        public DateTime LastServiceDate { get; set; } //(date, null)
        public string CreatedBy { get; set; } //(varchar(50), null)
        public DateTime CreatedDate { get; set; } //(datetime, null)
        public int RegionId { get; set; } //(int, null)
        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public int ContractId { get; set; } //(int, not null)
        public string PONumber { get; set; } //(varchar(20), null)
        public int AccountTypeListId { get; set; } //(int, null)
        public string AccountTypeName { get; set; } //(varchar(75), null)
        public int Term { get; set; } //(int, null)
        public DateTime StartDate { get; set; } //(date, null)
        public DateTime ExpirationDate { get; set; } //(date, null)
        public decimal TotalAmount { get; set; } //(decimal(18,2), null)
        public string Status { get; set; } //(varchar(250), not null)
        public string StatusReason { get; set; } //(varchar(250), not null)
        public string StatusName { get; set; } //(varchar(250), not null)

    }
    public class CTPDetailContractDetailViewModel
    {
        public int ContractDetailId { get; set; } //(int, not null)
        public int ContractId { get; set; } //(int, null)
        public int LineNumber { get; set; } //(int, null)
        public int ServiceTypeListId { get; set; } //(int, null)
        public string ServiceTypeName { get; set; } //(varchar(50), not null)
        public decimal Amount { get; set; } //(decimal(18,2), null)
        public int BillingFrequencyListId { get; set; } //(int, null)
        public string BillingFrequencyListName { get; set; } //(varchar(75), null)
        public int CustomerId { get; set; } //(int, null)
        public string Mon { get; set; } //(varchar(5), not null)
        public string Tues { get; set; } //(varchar(5), not null)
        public string Wed { get; set; } //(varchar(5), not null)
        public string Thur { get; set; } //(varchar(5), not null)
        public string Fri { get; set; } //(varchar(5), not null)
        public string Sat { get; set; } //(varchar(5), not null)
        public string Sun { get; set; } //(varchar(5), not null)
    }
    public class CTPDetailFranchiseeDistributionViewModel
    {
        public int MaintenanceDetailDistributionTempId { get; set; } //(int, not null)
        public int DetailLineNumber { get; set; } //(int, null)
        public int FranchiseeId { get; set; } //(int, null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public decimal Amount { get; set; } //(decimal(12,2), null)
        public decimal ContractDetailAmount { get; set; } //(decimal(18,2), null)
        public int CustomerId { get; set; } //(int, not null)
        public int ContractDetailId { get; set; } //(int, null)
        public string RecordType { get; set; } //(varchar(15), null)
    }
    public class CTPDetailFranchiseeStopFindersFeeViewModel
    {
        public int MaintenanceDetailTempId { get; set; } //(int, not null)
        public int MaintenanceTempId { get; set; } //(int, null)
        public int ContractDetailId { get; set; } //(int, null)
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public DateTime FindersFeeStopDate { get; set; } //(datetime, null)
        public string Notes { get; set; } //(nvarchar(max), null)
        public bool IsTroubleWithFee { get; set; } //(bit, null)
        public bool IsIsTroubleWithoutFee { get; set; } //(bit, null)
        public bool IsNotATroubleAccount { get; set; } //(bit, null)
        public string TroubleFee { get {

                return (IsTroubleWithFee == true ? "IsTroubleWithFee" : (IsIsTroubleWithoutFee == true ? "IsIsTroubleWithoutFee" : "IsNotATroubleAccount"));
            } } //(decimal(16,2), null)
    }
    public class CTPDetailInvoiceFranchiseeDistributionViewModel
    {
        public int MaintenanceDetailDistributionTempId { get; set; } //(int, not null)
        public int MaintenanceTempId { get; set; } //(int, null)
        public int ContractDetailId { get; set; } //(int, null)
        public int LineNumber { get; set; } //(int, null)
        public int FranchiseeId { get; set; } //(int, null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public decimal Amount { get; set; } //(decimal(12,2), null)
        public string RecordType { get; set; } //(varchar(15), null)
        public decimal FeeAmount { get; set; } //(decimal(12,2), null)
        public decimal TotalAmount { get; set; } //(decimal(12,2), null)
        public int InvoiceId { get; set; } //(int, null)
        public string InvoiceNo { get; set; } //(char(25), null)
        public string Period { get; set; } //(char(25), null)
        public DateTime InvoiceDate { get; set; } //(char(25), null)

        public decimal ContractDetailAmount { get; set; } //(decimal(12,2), null)
        public string BillingPayNo { get; set; } //(char(25), null)
        public decimal FeePer {
            get {
                if (Amount > 0)
                {
                    decimal Per = 100 - ((FeeAmount * 100) / Amount);

                    return Per.ToString().Length > 0 ? Math.Round(Per, 2) : 0;
                }
                else
                    return 0;
            }
        } //(decimal(12,2), null)
    }
   

}

