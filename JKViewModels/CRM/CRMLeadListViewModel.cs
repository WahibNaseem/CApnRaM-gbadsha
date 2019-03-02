using System.Collections.Generic;

namespace JKViewModels.CRM
{
    public class SalesUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
    }
    public class CRM_ReAssignTerritory
    {
        public int CRM_TerritoryId { get; set; }
        public string Name { get; set; }
    }
    public class UserLeadList
    {
        public List<SalesUser> lstSalesUser { get; set; }
        public List<CRM_ReAssignTerritory> lstCRM_Territory { get; set; }
    }
    public class CRMLeadListViewModel
    {
        public int LeadId { get; set; }
        public string CompanyName { get; set; }
        public string StageStatus { get; set; }
        //public StageStatusType Status { get; set; }
    }

    public class LeadReAssignViewModel
    {
        public List<CRMLeadListViewModel> LeadReAssignLeadList { get; set; }
        public List<CRM_ReAssignTerritory> LeadReAssignTerritory { get; set; }
        public List<SalesUser> LeadReAssignSalesUser { get; set; }
    }

    public class LeadListView
    {
        public int LeadId { get; set; }
        public int AssigneeId { get; set; }
        public string Comments { get; set; }
    }
}
