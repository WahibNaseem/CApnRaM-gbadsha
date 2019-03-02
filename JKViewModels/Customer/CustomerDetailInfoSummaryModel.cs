using System;
using JKViewModels.Common;

namespace JKViewModels.Customer
{
    public class CustomerDetailInfoSummaryModel
    {
        public int CustomerId { get; set; }
        public int ContractId { get; set; }        
        public int ContractTypeListId { get; set; }
        public int ContractDetailCount { get; set; }
        public int DistributionCount { get; set; }
        public int BillSettingsCount { get; set; }
        public int MainAddressCount { get; set; }
        public int BillingAddressCount { get; set; }
    }
}
