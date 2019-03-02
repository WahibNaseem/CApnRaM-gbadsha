using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class AccountAcceptanceInfoViewModel
    {

        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(10), null)
        public string CustomerName { get; set; } //(varchar(150), null)
        public string Address1 { get; set; } //(varchar(125), null)
        public string Address2 { get; set; } //(varchar(125), null)
        public string City { get; set; } //(varchar(50), null)
        public string StateName { get; set; } //(varchar(50), null)
        public string PostalCode { get; set; } //(varchar(20), null)

        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public string FAddress1 { get; set; } //(varchar(125), null)
        public string FAddress2 { get; set; } //(varchar(125), null)
        public string FCity { get; set; } //(varchar(50), null)
        public string FStateName { get; set; } //(varchar(50), null)
        public string FPostalCode { get; set; } //(varchar(20), null)
        public string RegionName { get; set; } //(varchar(20), null)

        public decimal ContractBillingAmount { get; set; } //(decimal(12,2), null)
        public DateTime StartDate { get; set; } //(date, null)
    }
}
