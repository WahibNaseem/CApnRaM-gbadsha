using JKApi.Data.DAL;
using JKApi.Data.DTOObject;
using JKViewModels;
using JKViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JKApi.Service
{
    public interface IUserService
    {
        string UserResetPassword(string Type, int UserId, string OldPassword, string NewPassword);
        UserViewModel Login(UserLoginViewModel loginViewModel);
        UserViewModel GetUserDetail(int _userid);
        AuthUserLogin SaveUser(AuthUserLogin User); 
        UserDetailViewModel SaveUser(UserDetailViewModel model); 

        List<Data.DAL.Region> getRegion();
        UserLoginListViewModel GetUserSearchList(UserLoginListViewModel model);
        UserViewModel GetUserDetailByEmail(string _emailid);
        void UpdateIsAccExec(string checkedIsAccExec, string uncheckedIsAccExec);

        #region Group
        GroupViewModel GetGroupList(GroupViewModel model);

        GroupViewModel SaveGroup(GroupViewModel model);
        #endregion

        #region Role
        RoleViewModel GetRoleList(RoleViewModel model);

        RoleViewModel SaveRole(RoleViewModel model);
        #endregion


        #region Menu List


        MenuListModel FillMenu();


        ViewMenuModel SearchMenu(ViewMenuModel objPostMenuModel);

        #endregion

        #region Save Menu


        SaveMenuModel GetMenuById(PostMenuModel objPostMenuModel);


        SaveMenuModel InsertUpdateMenu(SaveMenuModel objSaveMenuModel);


        MenuListModel GetAllMenu();

        #endregion

        #region Assign Menu
        AssignARModel InsertUpdateAssignARMenu(AssignARModel objAssignMenuModel);

        AssignEDModel InsertUpdateAssignEDMenu(AssignEDModel objAssignMenuModel);


        AssignMenuModel InsertUpdateAssignMenu(AssignMenuModel objAssignMenuModel);

         
        MenuListModel GetAssignMenusByAccess(int RoleId);

         
        RolebaseMenuAccessDetailListModel GetRolebasedMenuAccessDetail(int RoleId);

        RolebaseARAccessDetailListModel GetARRolebasedMenuAccessDetail(int RoleId);

        RolebaseEDAccessDetailListModel GetEDRolebasedMenuAccessDetail(int RoleId);

        #endregion

        #region Inspection
        FormCustomItemTemplateModel SaveCustomTemplate(FormCustomItemTemplateModel model);

        FormTemplateModel SaveTemplate(FormTemplateModel model);

        InspectionSaveModel SaveInspection(InspectionSaveModel model);

        TemplateItemViewModel SaveTemplateItem(TemplateItemViewModel model);

        TemplateAreaViewModel SaveTemplateArea(TemplateAreaViewModel model);

        TemplateQuestionViewModel SaveTemplateQuestion(TemplateQuestionViewModel model);

        NewFormTemplateViewModel SaveTemplate(NewFormTemplateViewModel model);
        #endregion

        #region Feature EMail
        FeatureTypeEmailViewModel SaveFeatureEmail(FeatureTypeEmailViewModel model);
        #endregion

        int UpdateFMSPassword(int _userid,string strfmsnewpwd);
    }
}
