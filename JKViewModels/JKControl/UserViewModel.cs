using JKViewModels.Administration.Company;
using JKViewModels.JKControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{

    public class UserViewModel : BaseModel
    {
        public UserViewModel()
        {
            Roles = new List<RoleModel>();
            Regions = new List<RegionInfoViewModel>();
            RoleAccesss = new List<RoleAccessModel>();
            lstMenu = new List<MenuModel>();
            lstPeriodAccess = new List<PeriodAccessModel>();
            lstRolebasedARAccessDetailModel = new List<RolebasedARAccessDetailModel>();
            lstRolebasedEDAccessDetailModel = new List<RolebasedEDAccessDetailModel>();
        }

        public List<RolebasedARAccessDetailModel> lstRolebasedARAccessDetailModel { get; set; }

        public List<RolebasedEDAccessDetailModel> lstRolebasedEDAccessDetailModel { get; set; }

        public List<PeriodAccessModel> lstPeriodAccess { get; set; }

        public List<MenuModel> lstMenu { get; set; }

        public List<RoleModel> Roles { get; set; }
        public List<RegionInfoViewModel> Regions { get; set; }

        public string RoleIds { get; set; }

        public string RoleName { get; set; }
        public int RoleId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public DateTime? IsFirstTimeLogin { get; set; }

        public Guid Salt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Addres { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zipcode { get; set; }

        public int GroupId { get; set; }

        public List<RoleAccessModel> RoleAccesss { get; set; }

        public int DefaultRegionId { get; set; }


        public int LoginTrackingId { get; set; }
        public string OutlookUsername { get; set; }
        public string OutlookPassword { get; set; }

    }

    public class UserViewListModel : PaggingModel
    {
        public UserViewListModel()
        {
            userListModel = new List<UserViewModel>();
        }

        public string SelectedUserId { get; set; }

        public List<UserViewModel> userListModel { get; set; }
    }

    public class RoleModel : BaseModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public int RoleTypeId { get; set; }
        public string RoleTypeName { get; set; }
    }

    public class RoleListModel : BaseModel
    {
        public List<RoleModel> lstRole { get; set; }
    }


    public class RoleAccessModel : BaseModel
    {
        public int RoleAccessId { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }

        public bool? IsDeleteAccess { get; set; }

        public bool? IsInsertAccess { get; set; }

        public bool? IsViewAccess { get; set; }

        public int RegionId { get; set; }

        public string MenuName { get; set; }

        public string PageName { get; set; }

        public int ParentMenuId { get; set; }

        public string MenuUrl { get; set; }
        public int? MenuLevel { get; set; }
        public int? MenuOrder { get; set; }

        public int ModuleId { get; set; }
        public bool IsDisplay { get; set; }
        public string SelectedPageName { get; set; }

        public string OperationType { get; set; }
        public string IsMenuActive { get; set; }
        public string ParentMenu { get; set; }
        public string MenuImageUrl { get; set; }


    }


    public class PeriodAccessModel
    {
        public int PeriodId { get; set; }
        public int RegionId { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
    }


}
