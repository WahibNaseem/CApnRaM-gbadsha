using System;
using System.Collections.Generic;
using System.Linq;
using JKApi.Data.DAL;
using JKViewModels.User;
using System.Data;
using System.Data.SqlClient;
using JKViewModels;
using JK.Resources;
using JKApi.Service.Service;
using JKApi.Service.Helper.Extension;
using System.Web;
using JKViewModels.Common;

namespace JKApi.Service
{
    public class UserService : BaseService, IUserService
    {
        static jkDatabaseEntities jkEntityModel = new jkDatabaseEntities();


        public string UserResetPassword(string Type, int UserId, string OldPassword, string NewPassword)
        {

            const int errorCode = 0;
            const string errorMessage = "";

            var pErrorCode = new SqlParameter("@ErrorCode", errorCode) { Direction = ParameterDirection.Output };
            var pErrorMessage = new SqlParameter("@ErrorMessage", errorMessage)
            {
                Direction = ParameterDirection.Output,
                Size = 8000
            };

            SqlParameter[] parmList =
            {
                new SqlParameter("@UserId",UserId),
                new SqlParameter("@Type",Type),
                new SqlParameter("@OldPassword",OldPassword),
                new SqlParameter("@NewPassword",NewPassword),
                pErrorCode,
                pErrorMessage
            };

            using (var userDetailsDs = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_UserPasswordReset, parmList))
            {

                return Convert.ToString(pErrorMessage.Value);

            }

        }

        public UserViewModel Login(UserLoginViewModel loginViewModel)
        {
            var userViewModel = new UserViewModel();
            const int errorCode = 0;
            const string errorMessage = "";

            var pErrorCode = new SqlParameter("@ErrorCode", errorCode) { Direction = ParameterDirection.Output };
            var pErrorMessage = new SqlParameter("@ErrorMessage", errorMessage)
            {
                Direction = ParameterDirection.Output,
                Size = 8000
            };

            SqlParameter[] parmList =
            {
                new SqlParameter("@Username",loginViewModel.Username.Trim()),
                new SqlParameter("@Password",loginViewModel.Password.Trim()),
                new SqlParameter("@IPAddress",loginViewModel.IPAddress),
                pErrorCode,
                pErrorMessage
            };

            using (var userDetailsDs = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_UserLogin, parmList))
            {

                if (userDetailsDs != null && userDetailsDs.Tables.Count > 3)
                {
                    // User Details 
                    if (userDetailsDs.Tables[0].Rows.Count > 0)
                    {
                        userViewModel = GetDataRowToEntity<UserViewModel>(userDetailsDs.Tables[0].Rows[0]);
                    }

                    // Role List with All Details 
                    if (userDetailsDs.Tables[1].Rows.Count > 0)
                    {
                        userViewModel.Roles = new List<RoleModel>();
                        foreach (DataRow item in userDetailsDs.Tables[1].Rows)
                        {
                            userViewModel.Roles.Add(GetDataRowToEntity<RoleModel>(item));
                        }
                    }

                    // Company List with All Details 
                    if (userDetailsDs.Tables[2].Rows.Count > 0)
                    {
                        userViewModel.Regions = new List<RegionInfoViewModel>();
                        foreach (DataRow item in userDetailsDs.Tables[2].Rows)
                        {
                            userViewModel.Regions.Add(GetDataRowToEntity<RegionInfoViewModel>(item));
                        }
                    }

                    //RoleAccess List with All Details 
                    if (userDetailsDs.Tables[3].Rows.Count > 0)
                    {
                        userViewModel.RoleAccesss = new List<RoleAccessModel>();
                        foreach (DataRow item in userDetailsDs.Tables[3].Rows)
                        {
                            userViewModel.RoleAccesss.Add(GetDataRowToEntity<RoleAccessModel>(item));
                        }
                    }

                    //Period Access List with All Details 
                    if (userDetailsDs.Tables[4].Rows.Count > 0)
                    {
                        userViewModel.lstPeriodAccess = new List<PeriodAccessModel>();
                        foreach (DataRow item in userDetailsDs.Tables[4].Rows)
                        {
                            userViewModel.lstPeriodAccess.Add(GetDataRowToEntity<PeriodAccessModel>(item));
                        }
                    }

                    //AR Permission List with All Details 
                    if (userDetailsDs.Tables[5].Rows.Count > 0)
                    {
                        userViewModel.lstRolebasedARAccessDetailModel = new List<RolebasedARAccessDetailModel>();
                        foreach (DataRow item in userDetailsDs.Tables[5].Rows)
                        {
                            userViewModel.lstRolebasedARAccessDetailModel.Add(GetDataRowToEntity<RolebasedARAccessDetailModel>(item));
                        }
                    }

                    //ED Permission Access List with All Details 
                    if (userDetailsDs.Tables[6].Rows.Count > 0)
                    {
                        userViewModel.lstRolebasedEDAccessDetailModel = new List<RolebasedEDAccessDetailModel>();
                        foreach (DataRow item in userDetailsDs.Tables[6].Rows)
                        {
                            userViewModel.lstRolebasedEDAccessDetailModel.Add(GetDataRowToEntity<RolebasedEDAccessDetailModel>(item));
                        }
                    }

                }
                else if (Convert.ToInt32(pErrorCode.Value) > 0)
                    userViewModel.objErrorModel.Add(new ErrorModel() { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
                else
                    userViewModel.objErrorModel.Add(new ErrorModel() { ErrorCode = 0, ErrorMessage = string.Format(CommonResource.msgCommonError, "{0}", "login") });
            }

            // check session, so Login method can be used from WebAPI as well
            if (HttpContext.Current.Session != null)
            {
                //Store session
                var appSession = new AppSession();
                appSession.UserInfo = userViewModel;

                HttpContext.Current.Session[AppSetting.UserSession] = appSession;
            }

            return userViewModel;
        }

        public UserViewModel GetUserDetail(int _userid)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = context.AuthUserLogins.Where(o => o.UserId == _userid).MapEnumerable<UserViewModel, AuthUserLogin>().FirstOrDefault();
                return data;
            }
        }

        public UserViewModel GetUserDetailByEmail(string _emailid)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = context.AuthUserLogins.Where(o => o.Email == _emailid).MapEnumerable<UserViewModel, AuthUserLogin>().FirstOrDefault();
                return data;
            }
        }

        public UserInsertListViewModel GetUserSearchList(UserInsertListViewModel userInsertListViewModel)
        {
            UserInsertListViewModel userInsertListViewModelReturn = new UserInsertListViewModel();
            userInsertListViewModelReturn.userList = new List<UserInsertViewModel>();

            SqlParameter[] parmList = {
                                         new SqlParameter("@SearchBy", userInsertListViewModel.SearchBy),
                                         new SqlParameter("@PageNo",userInsertListViewModel.CurrentPage),
                                         new SqlParameter("@PageSize",userInsertListViewModel.PageSize),
                                         new SqlParameter("@SortColumn",userInsertListViewModel.SortBy),
                                         new SqlParameter("@SortOrder",userInsertListViewModel.SortOrder),
                                      };

            //Get all the active user without super admin.
            using (DataSet userDS = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_UserList, parmList))
            {

                if (userDS != null && userDS.Tables != null && userDS.Tables.Count > 0)
                {
                    DataTable ContentDT = userDS.Tables[0];
                    if (ContentDT != null && ContentDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ContentDT.Rows)
                        {
                            userInsertListViewModelReturn.userList.Add(GetDataRowToEntity<UserInsertViewModel>(dr));
                        }
                        userInsertListViewModelReturn.TotalCount = Convert.ToInt32(ContentDT.Rows[0]["TotalRecords"]);
                    }
                }
                else
                {
                    userInsertListViewModelReturn.objErrorModel.Add(new ErrorModel() { ErrorCode = 1, ErrorMessage = "No user found." });
                }
            }

            userInsertListViewModelReturn.PageSize = userInsertListViewModel.PageSize;
            userInsertListViewModelReturn.CurrentPage = userInsertListViewModel.CurrentPage;
            userInsertListViewModelReturn.SortBy = userInsertListViewModel.SortBy;
            userInsertListViewModelReturn.SortOrder = userInsertListViewModel.SortOrder;

            return userInsertListViewModelReturn;
        }

        public AuthUserLogin SaveUser(AuthUserLogin User)
        {
            var ID = User.UserId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                using (jkEntityModel = new jkDatabaseEntities())
                {
                    jkEntityModel.AuthUserLogins.Add(User);
                    jkEntityModel.SaveChanges();
                }
            }
            else //update existing entry
            {

            }

            return User;
        }

        public UserDetailViewModel SaveUser(UserDetailViewModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@UserId",model.UserId),
            new SqlParameter("@UserName",model.UserName),
            new SqlParameter("@PasswordHash",model.Password),
            new SqlParameter("@GroupId",model.GroupId),
            new SqlParameter("@FirstName",model.FirstName),
            new SqlParameter("@LastName",model.LastName),
            new SqlParameter("@Email",model.Email),
            new SqlParameter("@Phone",model.PhoneNumber),
            new SqlParameter("@Addres",model.Address),
            new SqlParameter("@City",model.City),
            new SqlParameter("@State",model.State),
            new SqlParameter("@Zipcode",model.ZipCode),
            new SqlParameter("@DeparmentId",model.DeparmentId),
            new SqlParameter("@Title",model.Title),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            new SqlParameter("@RoleIds",model.RoleIds),
            new SqlParameter("@RegionIds",model.RegionIds),
            new SqlParameter("@OutlookUsername",model.OutlookUsername),
            new SqlParameter("@OutlookPassword",model.OutlookPassword),
            new SqlParameter("@DefaultRegionId",model.DefaultRegionId),
            pErrorCode,
            pErrorMessage,
        };
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_SaveUserLogin, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    model = GetDataRowToEntity<UserDetailViewModel>(ds.Tables[0].Rows[0]);

                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            }

            return model;
        }


        public List<Data.DAL.Region> getRegion()
        {
            using (jkEntityModel = new jkDatabaseEntities())
            {
                var data = jkEntityModel.Regions.ToList();
                return data;
            }
        }
        public UserLoginListViewModel GetUserSearchList(UserLoginListViewModel model)
        {
            model.userList = new List<UserLoginModel>();

            SqlParameter[] parmList = {
                                         new SqlParameter("@RegionId", model.RegionId),
                                         new SqlParameter("@IsActive",model.IsActve),
                                      };

            //Get all the active user without super admin.
            using (DataSet userDS = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_UserList, parmList))
            {

                if (userDS != null && userDS.Tables != null && userDS.Tables.Count > 0)
                {
                    DataTable ContentDT = userDS.Tables[0];
                    if (ContentDT != null && ContentDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ContentDT.Rows)
                        {
                            model.userList.Add(GetDataRowToEntity<UserLoginModel>(dr));
                        }
                        //model.TotalCount = Convert.ToInt32(ContentDT.Rows[0]["TotalRecords"]);
                    }
                }
            }

            return model;
        }

        public void UpdateIsAccExec(string checkedIsAccExec, string uncheckedIsAccExec)
        {
            SqlParameter[] parmList = {
                                         new SqlParameter("@checkedIsAccExec", checkedIsAccExec),
                                         new SqlParameter("@uncheckedIsAccExec",uncheckedIsAccExec),
                                      };
            SQLHelper.ExecuteScalar(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_UpdateIsAccExec, parmList);
        }

        #region Group
        public GroupViewModel GetGroupList(GroupViewModel model)
        {
            model.groupList = new List<GroupModel>();

            //Get all the active user without super admin.
            using (DataSet userDS = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GroupList))
            {
                if (userDS != null && userDS.Tables != null && userDS.Tables.Count > 0)
                {
                    DataTable ContentDT = userDS.Tables[0];
                    if (ContentDT != null && ContentDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ContentDT.Rows)
                        {
                            model.groupList.Add(GetDataRowToEntity<GroupModel>(dr));
                        }
                    }
                }
            }

            return model;
        }

        public GroupViewModel SaveGroup(GroupViewModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@GroupId",model.GroupId),
            new SqlParameter("@RegionIds",model.RegionIds),
            new SqlParameter("@Name",model.Name),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_SaveGroup, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    model = GetDataRowToEntity<GroupViewModel>(ds.Tables[0].Rows[0]);
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }
        #endregion

        #region Role
        public RoleViewModel GetRoleList(RoleViewModel model)
        {
            model.roleList = new List<AuthRoleModel>();

            //Get all the active user without super admin.
            using (DataSet userDS = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_RoleList))
            {
                if (userDS != null && userDS.Tables != null && userDS.Tables.Count > 0)
                {
                    DataTable ContentDT = userDS.Tables[0];
                    if (ContentDT != null && ContentDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ContentDT.Rows)
                        {
                            model.roleList.Add(GetDataRowToEntity<AuthRoleModel>(dr));
                        }
                    }
                }
            }

            return model;
        }

        public RoleViewModel SaveRole(RoleViewModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@RoleId",model.RoleId),
             new SqlParameter("@RoleTypeId",model.RoleTypeId),
            new SqlParameter("@UserIds",model.UserIds),
            new SqlParameter("@Name",model.RoleName),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_SaveRole, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    model = GetDataRowToEntity<RoleViewModel>(ds.Tables[0].Rows[0]);
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }
        #endregion


        #region Menu List

        /// <summary>
        /// This method is used to get all the menus for dropdown purpose.
        /// </summary>
        /// <returns></returns>
        public MenuListModel FillMenu()
        {
            MenuListModel objMenuListModel = new MenuListModel();
            objMenuListModel.lstMenu = new List<MenuModel>();


            //get All Menu in data set.
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_FillAllMenu))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            MenuModel objMenuModel = new MenuModel();
                            //Convert data row in to Model object
                            objMenuModel = GetDataRowToEntity<MenuModel>(dr);
                            //add model object  in to List of Model
                            objMenuListModel.lstMenu.Add(objMenuModel);
                        }
                    }
                }
            }
            return objMenuListModel;
        }

        /// <summary>
        /// This method is used to get menu list with searching, paging, sorting.
        /// </summary>
        /// <param name="objPostMenuModel"></param>
        /// <returns></returns>
        public ViewMenuModel SearchMenu(ViewMenuModel objPostMenuModel)
        {
            ViewMenuModel objViewMenuModel = new ViewMenuModel();
            SqlParameter[] Param = {
                                           new SqlParameter("@MenuName",objPostMenuModel.FilterMenuName),
                                           new SqlParameter("@Status",objPostMenuModel.FilterStatus),
                                           new SqlParameter("@PageNo",objPostMenuModel.CurrentPage),
                                           new SqlParameter("@PageSize",objPostMenuModel.PageSize),
                                           new SqlParameter("@SortColumn",objPostMenuModel.SortBy),
                                           new SqlParameter("@SortOrder",objPostMenuModel.SortOrder),
                                       };
            //get country detail in data set
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_SearchMenu, Param))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MenuModel objMenuModel = new MenuModel();
                        //Convert data row in to Model object
                        objMenuModel = GetDataRowToEntity<MenuModel>(dr);
                        objViewMenuModel.lstMenus.Add(objMenuModel);
                    }
                    if (objViewMenuModel != null && objViewMenuModel.lstMenus != null && objViewMenuModel.lstMenus.Count > 0)
                    {
                        int totalRecord = objViewMenuModel.lstMenus[0].TotalCount;
                        if (decimal.Remainder(totalRecord, objViewMenuModel.PageSize) > 0)
                            objViewMenuModel.TotalPages = (totalRecord / objViewMenuModel.PageSize + 1);
                        else
                            objViewMenuModel.TotalPages = totalRecord / objViewMenuModel.PageSize;
                        objViewMenuModel.TotalRecords = totalRecord;
                    }
                    else
                    {
                        objViewMenuModel.TotalPages = 0;
                    }
                    objViewMenuModel.PageSize = objPostMenuModel.PageSize;
                    objViewMenuModel.CurrentPage = objPostMenuModel.CurrentPage;
                    objViewMenuModel.SortBy = objPostMenuModel.SortBy;
                    objViewMenuModel.SortOrder = objPostMenuModel.SortOrder;
                }
                else
                {
                    objViewMenuModel.TotalRecords = 0;
                }
            }

            return objViewMenuModel;
        }

        #endregion

        #region Save Menu

        /// <summary>
        /// this method is used to Get Menu Details by Id.
        /// </summary>
        /// <param name="objPostMenuModel"></param>
        /// <returns></returns>
        public SaveMenuModel GetMenuById(PostMenuModel objPostMenuModel)
        {
            SaveMenuModel objSaveMenuModel = new SaveMenuModel();
            SqlParameter[] parmList =  {
                                       new SqlParameter("@MenuId",objPostMenuModel.MenuId)
            };
            using (DataSet MenuDetailDs = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetMenuById, parmList))
            {
                if (MenuDetailDs != null && MenuDetailDs.Tables != null)
                {

                    //get Product Details from Table[0]
                    if (MenuDetailDs.Tables.Count > 0)
                    {
                        DataTable menuDT = MenuDetailDs.Tables[0];
                        if (menuDT != null && menuDT.Rows.Count > 0)
                        {
                            DataRow menuRow = menuDT.Rows[0];
                            objSaveMenuModel = GetDataRowToEntity<SaveMenuModel>(menuRow);
                        }
                    }
                    else
                    {
                        objSaveMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = 2, ErrorMessage = "No Menu found" });
                    }

                }
                else
                {
                    objSaveMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = 1, ErrorMessage = "No record found" });
                }
            }
            return objSaveMenuModel;

        }

        /// <summary>
        /// This method is used to insert update Menu.
        /// </summary>
        /// <param name="objSaveMenuModel"></param>
        /// <returns></returns>
        public SaveMenuModel InsertUpdateMenu(SaveMenuModel objSaveMenuModel)
        {
            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", 0);
            pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", "");
            pErrorMessage.Direction = ParameterDirection.Output;
            SqlParameter[] Param = {
                                        new SqlParameter("@MenuID",objSaveMenuModel.MenuId),
                                        new SqlParameter("@MenuName",objSaveMenuModel.MenuName),
                                        new SqlParameter("@MenuURLName",objSaveMenuModel.MenuUrl),
                                        new SqlParameter("@MenuLevel",objSaveMenuModel.MenuLevel),
                                        new SqlParameter("@MenuOrder",objSaveMenuModel.MenuOrder),
                                        new SqlParameter("@ParentMenuID",objSaveMenuModel.ParentMenuId),
                                        new SqlParameter("@OperationType",objSaveMenuModel.OperationType),
                                        new SqlParameter("@IsActive",objSaveMenuModel.IsEnable),
                                        new SqlParameter("@CreatedBy",objSaveMenuModel.CreatedBy),
                                        new SqlParameter("@PageName",objSaveMenuModel.PageName),
                                        new SqlParameter("@IsDisplay",objSaveMenuModel.IsDisplay),
                                        new SqlParameter("@MenuImageUrl",objSaveMenuModel.MenuImageUrl),
                                        pErrorCode,
                                        pErrorMessage
                                       };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_InsertUpdateMenu, Param);
            if (Convert.ToInt32(pErrorCode.Value) > 0)
                objSaveMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            return objSaveMenuModel;
        }

        /// <summary>
        /// This method is used to get all active Menu for dropdown purpose.
        /// </summary>
        /// <returns></returns>
        public MenuListModel GetAllMenu()
        {
            MenuListModel objMenuListModel = new MenuListModel();
            objMenuListModel.lstMenu = new List<MenuModel>();

            //get All Menu in data set.
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetAllMenu))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            MenuModel objMenuModel = new MenuModel();
                            //Convert data row in to Model object
                            objMenuModel = GetDataRowToEntity<MenuModel>(dr);
                            //add model object  in to List of Model
                            objMenuListModel.lstMenu.Add(objMenuModel);
                        }
                    }
                }
            }
            return objMenuListModel;
        }
        #endregion

        #region Assign Menu

        /// <summary>
        /// This method is used to insert update the menu access with role.
        /// </summary>
        /// <param name="objAssignMenuModel"></param>
        /// <returns></returns>
        public AssignMenuModel InsertUpdateAssignMenu(AssignMenuModel objAssignMenuModel)
        {
            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", 0);
            pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", "");
            pErrorMessage.Direction = ParameterDirection.Output;
            pErrorMessage.Size = 8000;
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",objAssignMenuModel.RoleId),
                                          new SqlParameter("@RoleBasedMenuXML",objAssignMenuModel.RoleBasedMenuXml),
                                          new SqlParameter("@IsActive",objAssignMenuModel.IsEnable),
                                          new SqlParameter("@CreatedBy",objAssignMenuModel.CreatedBy),
                                         pErrorCode,
                                          pErrorMessage
                                       };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_InsertUpdateAssignMenu, Param);
            if (Convert.ToInt32(pErrorCode.Value) > 0)
                objAssignMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            return objAssignMenuModel;
        }

        public AssignARModel InsertUpdateAssignARMenu(AssignARModel objAssignMenuModel)
        {
            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", 0);
            pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", "");
            pErrorMessage.Direction = ParameterDirection.Output;
            pErrorMessage.Size = 8000;
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",objAssignMenuModel.RoleId),
                                          new SqlParameter("@RoleBasedMenuXML",objAssignMenuModel.RoleBasedMenuXml),
                                          new SqlParameter("@IsActive",objAssignMenuModel.IsEnable),
                                          new SqlParameter("@CreatedBy",objAssignMenuModel.CreatedBy),
                                         pErrorCode,
                                          pErrorMessage
                                       };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_InsertUpdateAssignARMenu, Param);
            if (Convert.ToInt32(pErrorCode.Value) > 0)
                objAssignMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            return objAssignMenuModel;
        }

        public AssignEDModel InsertUpdateAssignEDMenu(AssignEDModel objAssignMenuModel)
        {
            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", 0);
            pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", "");
            pErrorMessage.Direction = ParameterDirection.Output;
            pErrorMessage.Size = 8000;
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",objAssignMenuModel.RoleId),
                                          new SqlParameter("@RoleBasedMenuXML",objAssignMenuModel.RoleBasedMenuXml),
                                          new SqlParameter("@IsActive",objAssignMenuModel.IsEnable),
                                          new SqlParameter("@CreatedBy",objAssignMenuModel.CreatedBy),
                                         pErrorCode,
                                          pErrorMessage
                                       };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_InsertUpdateAssignEDMenu, Param);
            if (Convert.ToInt32(pErrorCode.Value) > 0)
                objAssignMenuModel.objErrorModel.Add(new ErrorModel() { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            return objAssignMenuModel;
        }

        /// <summary>
        /// This method is used to get assign menus by Role Id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public MenuListModel GetAssignMenusByAccess(int RoleId)
        {
            MenuListModel objMenuListModel = new MenuListModel();
            objMenuListModel.lstMenu = new List<MenuModel>();
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleIds",RoleId)
                                       };
            //get Assign Menus of User by access in data set
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetAssignMenusByAccess, Param))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MenuModel objMenuModel = new MenuModel();
                        //Convert data row in to Model object
                        objMenuModel = GetDataRowToEntity<MenuModel>(dr);
                        //add model object  in to List of Model
                        objMenuListModel.lstMenu.Add(objMenuModel);
                    }

                }
            }
            return objMenuListModel;
        }

        /// <summary>
        /// This method is used to get Role based menu access details by Role Id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RolebaseMenuAccessDetailListModel GetRolebasedMenuAccessDetail(int RoleId)
        {

            RolebaseMenuAccessDetailListModel objRolebasedMenuAccessDetailListModel = new RolebaseMenuAccessDetailListModel();
            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail = new List<RolebasedMenuAccessDetailModel>();
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",RoleId)
                                       };
            //get  Role based Menu Access in data set
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetRolebasedMenuAccessDetail, Param))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RolebasedMenuAccessDetailModel objRolebasedMenuAccessDetailModel = new RolebasedMenuAccessDetailModel();
                            //Convert data row in to Model object
                            objRolebasedMenuAccessDetailModel = GetDataRowToEntity<RolebasedMenuAccessDetailModel>(dr);
                            //add model object  in to lstRoleModel
                            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail.Add(objRolebasedMenuAccessDetailModel);
                        }
                    }
                }
            }
            //return list of Role based Menu Access Model
            return objRolebasedMenuAccessDetailListModel;
        }

        public RolebaseARAccessDetailListModel GetARRolebasedMenuAccessDetail(int RoleId)
        {

            RolebaseARAccessDetailListModel objRolebasedMenuAccessDetailListModel = new RolebaseARAccessDetailListModel();
            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail = new List<RolebasedARAccessDetailModel>();
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",RoleId)
                                       };
            //get  Role based Menu Access in data set
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetARRolebasedMenuAccessDetail, Param))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RolebasedARAccessDetailModel objRolebasedMenuAccessDetailModel = new RolebasedARAccessDetailModel();
                            //Convert data row in to Model object
                            objRolebasedMenuAccessDetailModel = GetDataRowToEntity<RolebasedARAccessDetailModel>(dr);
                            //add model object  in to lstRoleModel
                            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail.Add(objRolebasedMenuAccessDetailModel);
                        }
                    }
                }
            }
            //return list of Role based Menu Access Model
            return objRolebasedMenuAccessDetailListModel;
        }

        public RolebaseEDAccessDetailListModel GetEDRolebasedMenuAccessDetail(int RoleId)
        {

            RolebaseEDAccessDetailListModel objRolebasedMenuAccessDetailListModel = new RolebaseEDAccessDetailListModel();
            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail = new List<RolebasedEDAccessDetailModel>();
            SqlParameter[] Param = {
                                           new SqlParameter("@RoleId",RoleId)
                                       };
            //get  Role based Menu Access in data set
            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.Auth_GetEDRolebasedMenuAccessDetail, Param))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RolebasedEDAccessDetailModel objRolebasedMenuAccessDetailModel = new RolebasedEDAccessDetailModel();
                            //Convert data row in to Model object
                            objRolebasedMenuAccessDetailModel = GetDataRowToEntity<RolebasedEDAccessDetailModel>(dr);
                            //add model object  in to lstRoleModel
                            objRolebasedMenuAccessDetailListModel.lstRoleBasedMenuAccessDetail.Add(objRolebasedMenuAccessDetailModel);
                        }
                    }
                }
            }
            //return list of Role based Menu Access Model
            return objRolebasedMenuAccessDetailListModel;
        }


        #endregion


        #region Inspection

        public FormCustomItemTemplateModel SaveCustomTemplate(FormCustomItemTemplateModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@FormCustomItemTemplateId",model.FormCustomItemTemplateId),
            new SqlParameter("@FormItemTypeId",model.FormItemTypeId),
            new SqlParameter("@FormTemplateTypeId",model.FormTemplateTypeId),
            new SqlParameter("@Value",model.Value),
            new SqlParameter("@ItemOrder",model.ItemOrder),
            new SqlParameter("@SectionName",model.SectionName),
             new SqlParameter("@SectionOrder",model.SectionOrder),

            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Administration_Inspection_SaveCustomTemplate, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    model = GetDataRowToEntity<FormCustomItemTemplateModel>(ds.Tables[0].Rows[0]);
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }



        public FormTemplateModel SaveTemplate(FormTemplateModel model)
        {
            //            int ErrorCode = 0;
            //            string ErrorMessage = string.Empty;
            //
            //            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            //            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            //            SqlParameter[] parmList = {
            //            new SqlParameter("@FormTemplateId",model.FormTemplateId),
            //            new SqlParameter("@AccountTypeListId",model.AccountTypeListId),
            //            new SqlParameter("@ServiceTypeListId",model.ServiceTypeListId),
            //            new SqlParameter("@FormTemplateTypeId",model.FormTemplateTypeId),
            //            new SqlParameter("@FormName",model.FormName),
            //            new SqlParameter("@dtitemTemplate",ToDataTable<FormItemTemplateModel>(model.lstFormItemTemplate)),
            //            new SqlParameter("@Description",model.Description),
            //            new SqlParameter("@IsEnable",model.IsEnable),
            //            new SqlParameter("@CreatedBy",model.CreatedBy),
            //
            //            pErrorCode,
            //            pErrorMessage,
            //            };
            //
            //            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Administration_Inspection_SaveTemplate, parmList);
            //
            //            if (Convert.ToInt32(pErrorCode.Value) > 0)
            //                model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            //
            //            return model;
            return null;
        }


        public InspectionSaveModel SaveInspection(InspectionSaveModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@InspectionId",model.InspectionId),
            new SqlParameter("@ContractDetailId",model.ContractDetailId),
            new SqlParameter("@InspectorId",model.InspectorId),
            new SqlParameter("@InspectionStatusId",model.InspectionStatusId),
            new SqlParameter("@CallDate",model.CallDate),
            new SqlParameter("@RecordedDate",model.RecordedDate),
            new SqlParameter("@UploadedDate",model.UploadedDate),
            new SqlParameter("@CustomerRating",model.CustomerRating),
            new SqlParameter("@ScorePercent",model.ScorePercent),
            new SqlParameter("@Action",model.Action),
            new SqlParameter("@CustomerEvaluation",model.CustomerEvaluation),
            new SqlParameter("@FormTemplateId",model.FormTemplateId),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),

            pErrorCode,
            pErrorMessage,
            };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Administration_Inspection_SaveInspection, parmList);

            if (Convert.ToInt32(pErrorCode.Value) > 0)
                model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            return model;
        }

        #endregion

        #region Template 

        public TemplateAreaViewModel SaveTemplateArea(TemplateAreaViewModel model)
        {
            model.templateAreaList = new List<TemplateAreaModel>();
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@TemplateAreaId",model.TemplateAreaId),
            new SqlParameter("@ItemIds",model.ItemIds),
            new SqlParameter("@AreaName",model.AreaName),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Template_SaveTemplateArea, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.templateAreaList.Add(GetDataRowToEntity<TemplateAreaModel>(dr));
                    }
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }

        public TemplateItemViewModel SaveTemplateItem(TemplateItemViewModel model)
        {
            model.templateItemList = new List<TemplateItemModel>();
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@TemplateItemId",model.TemplateAreaItemId),
            new SqlParameter("@ItemName",model.ItemName),
            new SqlParameter("@FormItemType",model.FormItemType),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Template_SaveTemplateItem, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.templateItemList.Add(GetDataRowToEntity<TemplateItemModel>(dr));
                    }
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }


        public TemplateQuestionViewModel SaveTemplateQuestion(TemplateQuestionViewModel model)
        {
            model.TemplateQuestionList = new List<TemplateQuestionModel>();
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@TemplateQuestionId",model.TemplateQuestionId),
            new SqlParameter("@Question",model.Question),
            new SqlParameter("@QuestionType",model.QuestionType),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Template_SaveTemplateQuestion, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.TemplateQuestionList.Add(GetDataRowToEntity<TemplateQuestionModel>(dr));
                    }
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }

        public NewFormTemplateViewModel SaveTemplate(NewFormTemplateViewModel model)
        {
            model.listFormTemplateModel = new List<NewFormTemplateModel>();
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@AccountTypeListId",model.AccountTypeListId),
            new SqlParameter("@AreaIds",model.AreaIds),
            new SqlParameter("@Description",model.Description),
            new SqlParameter("@FormName",model.FormName),
            new SqlParameter("@FormTemplateId",model.FormTemplateId),
            new SqlParameter("@FormTemplateTypeId",model.FormTemplateTypeId),
            new SqlParameter("@ServiceTypeListId",model.ServiceTypeListId),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.sp_Template_SaveTemplate, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.listFormTemplateModel.Add(GetDataRowToEntity<NewFormTemplateModel>(dr));
                    }
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }
        #endregion


        #region Feature EMailIds

        public FeatureTypeEmailViewModel SaveFeatureEmail(FeatureTypeEmailViewModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            model.featureTypeEmailList = new List<FeatureTypeEmailModel>();

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
                    new SqlParameter("@FeatureTypeEmailId",model.FeatureTypeEmailId),
                    new SqlParameter("@FeatureTypeId",model.FeatureTypeId),
                    new SqlParameter("@FromEmail",model.FromEmail),
                    new SqlParameter("@ToEmailId",model.ToEmailId),
                    new SqlParameter("@EmailToCustomer",model.EmailToCustomer),
                    new SqlParameter("@IsEnable",model.IsEnable),
                    new SqlParameter("@CreatedBy",model.CreatedBy),
                    new SqlParameter("@ActionType",model.ActionType),
                    pErrorCode,
                    pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.spSave_Sys_FeatureEmail, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        model.featureTypeEmailList.Add(GetDataRowToEntity<FeatureTypeEmailModel>(item));
                    }

                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }
        #endregion

        public int UpdateFMSPassword(int _userid, string strfmsnewpwd)
        {
            //using (jkEntityModel = new jkDatabaseEntities())
            //{
            //    AuthUserLogin _model = new AuthUserLogin(); 
            //    var data = jkEntityModel.AuthUserLogins.Where(w=>w.UserId == _userid);
            //    if (data != null && data.Count() > 0)
            //    {
            //        _model = data.FirstOrDefault();
            //        _model.PasswordHash = strfmsnewpwd;
            //        _model.ModifiedBy = _userid;
            //        _model.ModifiedOn = DateTime.Now;
            //        jkEntityModel.SaveChanges();
            //    }
            //}
            return 1;
        }

    }
}
