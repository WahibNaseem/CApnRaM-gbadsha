using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMPotentialFranchiseeViewModel
    {
        public int CRM_AccountId { get; set; }
        public int? assigneeId { get; set; }
        public int? Regionid { get; set; }
        public int? CRM_BiddingId { get; set; }
        public int? CRM_CloseId { get; set; }
        public string StageStatusName { get; set; }
        public int? StageStatus { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public string FranchiseeName { get; set; }
        public DateTime? CallBack { get; set; }
        public int? AccountTypeListId { get; set; }
        public string AccountTypeName { get; set; }
        public int? NumberOfLocations { get; set; }
        public decimal? BudgetAmount { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public decimal? ContractAmount { get; set; } 
        public string ProviderName { get; set; }
        public string LeftDay { get; set; }
    }
}
