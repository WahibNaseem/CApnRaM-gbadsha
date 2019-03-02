using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
   public class CRMFranchiseFollowUpViewModel
    {
        public int CRM_FranchiseFollowUpId { get; set; }
        public bool? DiscloseAdditional { get; set; }
        public bool? StatusCreationConfirmed { get; set; }
        public bool? NotifyNextTraining { get; set; }
        public bool? KeepActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public string Note { get; set; }
    }
}
