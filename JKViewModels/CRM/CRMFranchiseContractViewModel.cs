using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
   public class CRMFranchiseContractViewModel :CRMBaseViewModel
    {
        public int CRM_FranchiseContractId { get; set; }
        public int CRM_AccountFranchiseDetailId { get; set; }
        public bool? FranchiseDisclosure { get; set; }
        public bool? FranchiseQuestionaire { get; set; }
        public bool? SignAgreement { get; set; }
        public int? CompanyCreated { get; set; }
        public bool? AllPrincipleClosed { get; set; }
        public bool? CompleteFranchiseApplication { get; set; }
        public int? RegionId { get; set; }        
        public DateTime? CreatedDate { get; set; }        
        public DateTime? ModifiedDate { get; set; }
        public string Notes { get; set; }
        public bool? SignedAcknowledgement { get; set; }
    }
}
