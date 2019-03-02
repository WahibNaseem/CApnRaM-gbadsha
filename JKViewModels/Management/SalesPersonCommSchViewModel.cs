using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class SalesPersonCommSchViewModel
    {
        public int SalesPersonCommSchId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int CommissionCompensationScheduleId { get; set; }
        public string CommissionCompensationScheduleDescription { get; set; }
        public int ContractTypeId { get; set; }
        public string ContractType { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RegionId { get; set; }        
    }   
}
