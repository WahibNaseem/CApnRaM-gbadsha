using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class MenuModel : PaggingModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int? MenuLevel { get; set; }
        public int? MenuOrder { get; set; }
        public int? ParentMenuId { get; set; }
        public string PageName { get; set; }
        public int ModuleId { get; set; }
        public bool IsDisplay { get; set; }
        public string SelectedPageName { get; set; }

        //public int ParentId { get; set; }
        public string OperationType { get; set; }
        public string IsMenuActive { get; set; }
        public string ParentMenu { get; set; }
        public string MenuImageUrl { get; set; }
        public int TotalRecords { get; set; }
        public bool IsWriteAccess { get; set; }
    }

    public class ViewMenuModel : PaggingModel
    {
        public ViewMenuModel()
        {
            lstMenus = new List<MenuModel>();
        }
        public String FilterMenuName { get; set; }
        public String FilterStatus { get; set; }

        public int TotalRecords { get; set; }
        public List<MenuModel> lstMenus { get; set; }
    }


    public class SaveMenuModel : MenuModel
    {

        public string ActionType { get; set; }

        public string ErrorMessage { get; set; }

    }

    public class RolebasedMenuAccessModel
    {
        public int AccessId { get; set; }
        public int RoleId { get; set; }
        public int BusinessSubCatId { get; set; }
        public int MenuId { get; set; }
        public bool IsReadAccess { get; set; }
        public bool IsWriteAccess { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }

    public class RolebasedMenuAccessDetailModel : MenuModel
    {
        public int AccessId { get; set; }
        public int RoleId { get; set; }
        public bool IsReadAccess { get; set; }
        //public bool IsWriteAccess { get; set; }
        public bool  IsDeleteAccess { get; set; }
        public bool IsChanged { get; set; }
    }

    public class RolebasedARAccessDetailModel : BaseModel
    {
        public int ARPermissionId    { get; set; }

        public int ARRoleAccessId { get; set; }
        public string EnumName { get; set; }

        public string DisplayName { get; set; }
        public int RoleId { get; set; }
        public bool IsApprove { get; set; }
        //public bool IsWriteAccess { get; set; }
        public bool IsReject { get; set; }

        public bool IsApproveEmail { get; set; }
        
        public bool IsRejectEmail { get; set; }



    }

    public class RolebasedEDAccessDetailModel : BaseModel
    {
        public int EDPermissionId { get; set; }

        public int EDRoleAccessId { get; set; }
        public string EnumName { get; set; }

        public string DisplayName { get; set; }
        public int RoleId { get; set; }
        public bool IsEdit { get; set; }
        //public bool IsWriteAccess { get; set; }
        public bool IsDeletePer { get; set; }



    }

    public class AssignMenuModel : BaseModel
    {
        public int RoleId { get; set; }
        //public bool IsActive { get; set; }
        //public int CreatedBy { get; set; }
        //public string ActionType { get; set; }
        //public string Message { get; set; }
        //public string MessageType { get; set; }
        public int ErrorCode { get; set; }
        public string RoleBasedMenuXml { get; set; }
        public List<RolebasedMenuAccessDetailModel> RoleBasedMenuAccessList { get; set; }
    }

    public class AssignARModel : BaseModel
    {
        public int RoleId { get; set; }
        public int ErrorCode { get; set; }
        public string RoleBasedMenuXml { get; set; }
        public List<RolebasedARAccessDetailModel> RoleBasedMenuAccessList { get; set; }
    }


    

    public class AssignEDModel : BaseModel
    {
        public int RoleId { get; set; }
        public int ErrorCode { get; set; }
        public string RoleBasedMenuXml { get; set; }
        public List<RolebasedEDAccessDetailModel> RoleBasedMenuAccessList { get; set; }
    }

    public class MenuListModel : BaseModel
    {
        public List<MenuModel> lstMenu { get; set; }
    }

    public class PostMenuModel : BaseModel
    {
        public int MenuId { get; set; }
    }

    public class RolebaseMenuAccessDetailListModel : BaseModel
    {
        public List<RolebasedMenuAccessDetailModel> lstRoleBasedMenuAccessDetail { get; set; }
    }

    public class RolebaseARAccessDetailListModel : BaseModel
    {
        public List<RolebasedARAccessDetailModel> lstRoleBasedMenuAccessDetail { get; set; }
    }

    public class RolebaseEDAccessDetailListModel : BaseModel
    {
        public List<RolebasedEDAccessDetailModel> lstRoleBasedMenuAccessDetail { get; set; }
    }

}
