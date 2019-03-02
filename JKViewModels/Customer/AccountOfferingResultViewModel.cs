using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CommonAccountOfferingViewModel
    {
        public List<AccountOfferingResultViewModel> lstAccountOfferingResult { get; set; }
        public AccountOfferingCustomerDetailViewModel CustomerDetail { get; set; }
    }
    public class AccountOfferingResultViewModel
    {
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public double Distance { get; set; } //(float, null)
        public int DistanceSigned { get; set; } //(int, not null)
        public string Plan { get; set; } //(varchar(50), null)
        public int ACCTS { get; set; } //(varchar(1), not null)
        public decimal BILLING { get; set; } //(varchar(1), not null)
        public decimal CONTEVAL { get; set; } //(varchar(1), not null)
        public decimal GRP1TOT { get; set; } //(decimal(2,2), not null)
        public int COMPLAINT { get; set; } //(varchar(1), not null)
        public int FAILINSP { get; set; } //(varchar(1), not null)
        public int CANCEL { get; set; } //(varchar(1), not null)
        public int PENDINGCANCEL { get; set; } //(varchar(1), not null)
        public DateTime? LASTOFFERED { get; set; } //(varchar(1), not null)
        public decimal OBLG { get; set; } //(varchar(1), not null)
        public decimal ExtendedPrice { get; set; } //(decimal(38,2), null)       
        public decimal Latitude { get; set; } //(decimal(18,8), null)
        public decimal Longitude { get; set; } //(decimal(18,8), null)
               
    }

    public class AccountOfferingCustomerDetailViewModel
    {
        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(10), null)
        public string CustomerName { get; set; } //(varchar(150), null)
        public string CustomerAddress { get; set; } //(varchar(150), null)
        public string CustomerAddress1 { get; set; } //(varchar(150), null)
        public decimal CustomerLatitude { get; set; } //(decimal(18,8), null)
        public decimal CustomerLongitude { get; set; } //(decimal(18,8), null)
        public decimal DistanceWithin { get; set; } //(varchar(1), not null)
        public int RegionId { get; set; } //(int, null)

        public string City { get; set; } //(varchar(1), not null)
        public string PostalCode { get; set; } //(int, null)
        
        public string State { get; set; } //(int, null)

        public string Phone { get; set; }

    }
}
