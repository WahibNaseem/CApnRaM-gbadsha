using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ServiceCallListViewModel
    {        
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CallDate { get; set; }
        public string CallTime { get; set; }
        public int? StatusResultListId { get; set; }
        public string Status { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public DateTime? CallBack { get; set; }
        public string Comments { get; set; }
        public int ServiceCallLogTypeListId { get; set; }
        public string ServiceLogTypeListName { get; set; }
        public int? InitiatedBy { get; set; }
        public string InitiatedByName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public string CreatedByName { get; set; }
        public string FollowUpBy { get; set; }
        public int FollowUpId { get; set; }
        public int ServiceCallLogId { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public decimal MonthlyBilling { get; set; }
    }

}
