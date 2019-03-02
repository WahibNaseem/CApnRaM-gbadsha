using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeObligationViewModel
    {
        public int FranchiseeId { get; set; } 
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; } 
        public string Status { get; set; } 
        public int CustomerId { get; set; } 
        public string CustomerNo { get; set; } 
        public DateTime OfferedDate { get; set; } 
        public string CustomerName { get; set; } 
        public string DaysKept { get; set; } 
        public string SalesRep { get; set; } 
        public decimal ContractAmount { get; set; } 
        public decimal OriginalContractAmount { get; set; } 
        public decimal LegalBalanceAmount { get; set; }
        public decimal OtherBalanceAmount { get; set; } 
    }
}
