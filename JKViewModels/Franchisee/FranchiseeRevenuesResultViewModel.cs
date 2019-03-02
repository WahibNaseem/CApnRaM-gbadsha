using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeRevenuesResultViewModel
    {
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public DateTime DateSign { get; set; } //(datetime, null)
        public string PlanType { get; set; } //(varchar(50), null)
        public decimal ContractBilling { get; set; } //(decimal(38,2), null)
        public decimal ActualBilling { get; set; } //(decimal(38,2), null)
        public decimal AdditionalBilling { get; set; } //(decimal(38,2), null)
        public decimal AdditionalOfficeBilling { get; set; } //(decimal(38,2), null)
        public decimal ClientSupplies { get; set; } //(decimal(38,2), null)
        public decimal MonthlyRevenue { get; set; } //(decimal(38,2), null)
        public decimal MonthlySalesTax { get; set; } //(decimal(38,2), null)
        public decimal GrandTotal { get; set; } //(decimal(38,2), null)
        public int RegionId { get; set; } //(int, not null)
        public string RegionName { get; set; } //(int, not null)

    }

}
