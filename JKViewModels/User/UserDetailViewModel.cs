using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.User
{
    public class UserDetailViewModel
    {
        public UserDetailViewModel()
        {
            objErrorModel = new List<ErrorModel>();
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public int GroupId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public Nullable<bool> IsEnable { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public string GroupName { get; set; }

        public int DeparmentId { get; set; }
        public string DeparmentName { get; set; }

        public string RegionName { get; set; }

        public string RoleName { get; set; }

        public string RoleIds { get; set; }

        public string RegionIds { get; set; }

        public string ActionType { get; set; }

        public List<ErrorModel> objErrorModel { get; set; }

        public string OutlookUsername { get; set; }
        public string OutlookPassword { get; set; }

        public int DefaultRegionId { get; set; }
    }

    public class UserLoginModel : BaseModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public DateTime? IsFirstTimeLogin { get; set; }

        public Guid Salt { get; set; }

        public int? GroupId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Addres { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zipcode { get; set; }

        public int? DeparmentId { get; set; }

        public string Title { get; set; }

        public string GroupName { get; set; }

        public string DeparmentName { get; set; }

        public string RegionName { get; set; }

        public string RoleName { get; set; }

        public string RoleIds { get; set; }

        public string RegionIds { get; set; }

        public int DefaultRegionId { get; set; }


        public string OutlookUsername { get; set; }
        public string OutlookPassword { get; set; }

        public bool IsAccExec { get; set; }
    }

    public class UserLoginListViewModel : PaggingModel
    {
        public UserLoginListViewModel()
        {
            this.userList = new List<UserLoginModel>();
        }

        public int? RegionId { get; set; }

        public bool? IsActve { get; set; }

        public List<UserLoginModel> userList { get; set; }
    }

    public class GroupModel : BaseModel
    {
        public int GroupId { get; set; }

        public string Name { get; set; }

        public string RegionName { get; set; }

        public string RegionIds { get; set; }

    }

    public class GroupViewModel : PaggingModel
    {
        public GroupViewModel()
        {
            groupList = new List<GroupModel>();
        }


        public List<GroupModel> groupList { get; set; }

        public int GroupId { get; set; }

        public string Name { get; set; }

        public string RegionIds { get; set; }

        public string ActionType { get; set; }
    }

    public class AuthRoleModel : BaseModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string UserName { get; set; }

        public string UserIds { get; set; }

        public int RoleTypeId { get; set; }

        public string RoleTypeName { get; set; }

    }

    public class RoleViewModel : PaggingModel
    {
        public RoleViewModel()
        {
            roleList = new List<AuthRoleModel>();
        }


        public List<AuthRoleModel> roleList { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int RoleTypeId { get; set; }

        public string RoleTypeName { get; set; }

        public string UserIds { get; set; }

        public string ActionType { get; set; }
    }


    public class FeatureTypeEmailModel : PaggingModel
    {
        public int FeatureTypeEmailId { get; set; }

        public int? FeatureTypeId { get; set; }

        public string FromEmail { get; set; }

        public string ToEmailId { get; set; }

        public bool? EmailToCustomer { get; set; }

        public string FeatureName { get; set; }


    }

    public class FeatureTypeEmailViewModel : PaggingModel
    {
        public FeatureTypeEmailViewModel()
        {
            featureTypeEmailList = new List<FeatureTypeEmailModel>();
        }


        public List<FeatureTypeEmailModel> featureTypeEmailList { get; set; }

        public int FeatureTypeEmailId { get; set; }

        public int? FeatureTypeId { get; set; }

        public string FromEmail { get; set; }

        public string ToEmailId { get; set; }

        public bool EmailToCustomer { get; set; }

        public string ActionType { get; set; }
    }


}
