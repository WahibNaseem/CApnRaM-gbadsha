using JK.Repository.Uow;
using JKApi.Core.Common;
using JKViewModels;
using System;
using System.Data;
using System.Reflection;
using JKApi.Core;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using JKViewModels.Common;
using Newtonsoft.Json;

namespace JKApi.Service.Service
{
    #region ICachedService

    public interface ICachedService
    {
        void ClearCache();
    }

    #endregion

    #region BaseService

    public class BaseService
    {
        public ClaimView ClaimView = ClaimView.Instance;
        protected NLogger NLogger = NLogger.Instance;
        protected IJKEfUow Uow;
        protected SqlConnection FmsDbConn => new SqlConnection(SQLHelper.ConnectionStringTransaction);

        public int SelectedRegionId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ClaimView.GetCLAIM_SELECTED_COMPANY_ID()))
                {
                    return Convert.ToInt32(ClaimView.GetCLAIM_SELECTED_COMPANY_ID());
                }
                return 0;
            }
        }

        public int SelectedUserId
        {
            get
            {
                var roles = ClaimView.GetCLAIM_ROLELIST();
                if (roles != null && roles.Count>0)
                {
                    if (roles.FindAll(x => x.RoleTypeName == RoleType.Admin.ToString() || x.RoleTypeName == RoleType.SuperAdmin.ToString()).Count > 0)
                    {
                        return 0;
                    }
                    return LoginUserId;
                }
                return LoginUserId;
            }
        }

        public int LoginUserId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ClaimView.GetCLAIM_USERID()))
                {
                    return Convert.ToInt32(ClaimView.GetCLAIM_USERID());
                }
                return 0;
            }
        }

        public Business.Common.JKError Error { get; set; }

        public object CheckError(object model)
        {
            if (Error != null && Error.ErrorCode >= 0)
            {
                return Error;
            }
            return model;   
        }

        public T GetDataRowToEntity<T>(DataRow dr) where T : new()
        {
            if (dr == null) return default(T);
            var objT = new T();

            try
            {
                var objprop = objT.GetType().GetProperties();
                foreach (var pr in objprop)
                {
                    if (dr.Table.Columns[pr.Name] != null && dr[pr.Name] != null && dr[pr.Name] != DBNull.Value)
                    {
                        var obj = dr[pr.Name];
                        pr.SetValue(objT, obj, null);
                    }
                }
            }
            catch (Exception exception)
            {
                NLogger.Error($"Exception: {exception}"); ;
            }

            return objT;
        }


        public DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public int ComputeTotalPages(int totalRecords, int count, int pageSize)
        {
            if (count > 0)
            {
                if (decimal.Remainder(totalRecords, pageSize) > 0)
                {
                    return totalRecords / pageSize + 1;
                }
                return totalRecords / pageSize;
            }
            else
            {
                return 0;
            }
        }

        public RootObjectlatlngViewModel GetCooridatesFromAddress(string address)
        {
            var root = new RootObjectlatlngViewModel();
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";
                var req = (HttpWebRequest)WebRequest.Create(url);
                var res = (HttpWebResponse)req.GetResponse();

                using (var streamreader = new StreamReader(res.GetResponseStream()))
                {
                    var result = streamreader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        root = JsonConvert.DeserializeObject<RootObjectlatlngViewModel>(result);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return root;
        }
    }

    #endregion
}
