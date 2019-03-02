using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerTransferViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        //Customer Info
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerPincode { get; set; }

        //Customer Billing Info
        public string CustomerBillingName { get; set; }
        public string CustomerBillingAddress { get; set; }
        public string CustomerBillingCity { get; set; }
        public string CustomerBillingState { get; set; }
        public string CustomerBillingPincode { get; set; }

        //Customer Status Info
        public int? StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public string StatusListName { get; set; }
        public Nullable<int> ReasonListId { get; set; }
        public string ReasonListName { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public ContractViewModel Contract { get; set; }
        public List<ContractDetailViewModel> ContractDetail { get; set; }
        public List<CustomerDistributionDetailViewModel> Distribution { get; set; }
        public List<CustomerDistributionFeeViewModel> DistributionFee { get; set; }
        public List<CustomerFindersFeeViewModel> FindersFee { get; set; }

    }
    
    public class CustomerFindersFeeViewModel
    {

        public int? FindersFeeId { get; set; }
        public int? ContractDetailId { get; set; }
        public int? LineNumber { get; set; }
        public int? ServiceTypeListId { get; set; }
        public string ServiceTypeListName { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<System.DateTime> FindersFeeStopDate { get; set; }
        public bool FindersFeeHasCancellationFee { get; set; }
    }

    public class CustomerDistributionDetailViewModel
    {
        public int DistributionId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public int FranchiseeId { get; set; }
        public int ContractDetailId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseName { get; set; }
        public decimal TotalDistribution { get; set; }
    }

    public class CustomerDistributionFeeViewModel
    {
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
}
