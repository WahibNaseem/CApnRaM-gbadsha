using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
   public class CRMLeadGenerationViewModel :BaseModel
    {
        public int CRM_LeadGenerationId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int PurposeId { get; set; }
        public string Note { get; set; }
      

    }
}
