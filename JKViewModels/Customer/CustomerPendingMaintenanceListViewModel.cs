using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerPendingMaintenanceListViewModel
    {
        public int MaintenanceTempId { get; set; }
        public int? MaintenanceTypeListId { get; set; }
        public string MaintenanceTypeListName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        public string Comments { get; set; }
        public string Reason { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }

    }
}
