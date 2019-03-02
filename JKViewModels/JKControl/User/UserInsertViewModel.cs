using JKViewModels.JKControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class UserInsertViewModel : BaseModel
    {
        public UserInsertViewModel()
        {
            Roles = new List<RoleModel>();
            Regions = new List<UserRoleRegionModel>();
        }
        public List<RoleModel> Roles { get; set; }
        public List<UserRoleRegionModel> Regions { get; set; }
 
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Title { get; set; }

        public int  DeparmentId { get; set; }
    }

    public class UserRoleRegionModel
    {
        public UserRoleRegionModel()
        {
            Roles = new List<RoleModel>();
        }
        public List<RoleModel> Roles { get; set; }

        public int RegionId { get; set; }

        public int RegionName { get; set; }
    }

    public class UserInsertListViewModel : PaggingModel
    {
        public UserInsertListViewModel()
        {
            userList = new List<UserInsertViewModel>();
        }

        public List<UserInsertViewModel> userList { get; set; }
    }
}
