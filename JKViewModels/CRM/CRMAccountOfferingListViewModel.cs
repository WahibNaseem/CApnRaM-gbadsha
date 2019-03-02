using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMAccountOfferingListViewModel
    {
        public int OfferingId { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string AccountType { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public decimal? Distance { get; set; }
        public decimal? ContractBillingAmount { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? OfferedDate { get; set; }
        public string SpecialNote { get; set; }
        public string Note { get; set; }
        public string DeclineReason { get; set; }
        public string OfferAcceptedNote { get; set; }
        public string OfferDeclinedNote { get; set; }

    }
}
