using JK.Repository.Uow;
using JKApi.Service.ServiceContract.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;
using System.Data;
using System.Data.SqlClient;
using JKViewModels;
using JK.Resources;
using JKViewModels.Administration.Company;
using JKViewModels.JKControl;

namespace JKApi.Service.Service.JKControl
{
    public class UserService : BaseService, IUserService
    {
        


        public UserInsertListViewModel DeleteContentDetail(UserInsertListViewModel userInsertListViewModel)
        {
            int ErrorCode = 0;
            string ErrorMessage = ""; 

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode);
            pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage);
            pErrorMessage.Direction = ParameterDirection.Output;
            pErrorMessage.Size = 8000;

            SqlParameter[] parmList = {
                                           new SqlParameter("@UserId",userInsertListViewModel.SelectedId),
                                           new SqlParameter("@CreatedBy",userInsertListViewModel.CreatedBy),
                                           new SqlParameter("@IsEnable", userInsertListViewModel.IsEnable),
                                           new SqlParameter("@IsDelete", userInsertListViewModel.IsDelete)
                                          ,pErrorCode
                                          ,pErrorMessage
                                          };

            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionStringCMSTransaction, CommandType.StoredProcedure, DBConstants.admin_UserDelete, parmList);

            if (Convert.ToInt32(pErrorCode.Value) > 0)
                userInsertListViewModel.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });
            else
                userInsertListViewModel = GetUserSearchList(userInsertListViewModel);

            return userInsertListViewModel;
        }
    }
}
