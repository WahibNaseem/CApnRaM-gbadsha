using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerMaintenanceApproval
    {
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int StatusListId { get; set; } //(int, null)
        public int ReasonListId { get; set; } //(int, null)
        public DateTime EffectiveDate { get; set; } //(date, null)
        public DateTime ResumeDate { get; set; } //(date, null)
        public string Comments { get; set; } //(varchar(125), null)
        public int CustomerId { get; set; } //(int, not null)
        public string Status { get; set; } //(varchar(250), not null)
        public string StatusReason { get; set; } //(varchar(250), not null)
        public string StatusName { get; set; } //(varchar(250), not null)

        public CustomerMaintenanceViewModel CustomerMaintenance { get; set; }
    }
   
}
