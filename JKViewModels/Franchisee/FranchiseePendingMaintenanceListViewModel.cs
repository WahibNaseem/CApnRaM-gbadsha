using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseePendingMaintenanceListViewModel
    {
        public int MaintenanceTempId { get; set; } //(int, not null)
        public int MaintenanceTypeListId { get; set; } //(int, null)
        public string MaintenanceTypeListName { get; set; } //(varchar(100), null)
        public int RegionId { get; set; } //(int, not null)
        public string RegionName { get; set; } //(varchar(30), null)
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string FranchiseeName { get; set; } //(varchar(150), null)
        public int StatusListId { get; set; } //(int, not null)
        public string StatusListName { get; set; } //(varchar(250), null)
        public string Comments { get; set; } //(varchar(max), null)
        public DateTime EffectiveDate { get; set; } //(date, null)
        public DateTime RequestDate { get; set; } //(datetime, null)
        public int StatusReasonListId { get; set; } //(int, null)
        public string StatusReasonListName { get; set; } //(varchar(100), null)
        public int MaintenanceStatusListId { get; set; } //(int, null)
        public string MaintenanceStatusListName { get; set; } //(varchar(100), null)
        public string CreatedBy { get; set; } //(varchar(100), null)
    }

}
