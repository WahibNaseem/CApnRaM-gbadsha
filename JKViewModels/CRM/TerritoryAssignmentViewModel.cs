using JKViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class TerritoryAssignmentViewModel
    {
        public List<TerritoryViewModel> TerritoryList { get; set; }
        public List<TerritoryAssignmentModel> SalesTerritoryAssignmentList { get; set; }
        public List<UserLoginModel> UserList { get; set; }
    }

    public class TerritoryViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class SaveTerritoryAssignmentViewModel
    {
        public List<TerritoryAssignmentModel> TerritoryAssignmentList { get; set; }
        public string CheckedUserIsAccExec { get; set; }
        public string NoNCheckedUserIsAccExec { get; set; }

    }
    
    public class TerritoryAssignmentModel
    {
        public int? TerritoryID { get; set; }
        public int? UserID { get; set; }
    }
}
