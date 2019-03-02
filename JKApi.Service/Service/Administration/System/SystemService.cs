using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using JKViewModels.Administration.System;

namespace JKApi.Service.Service.Administration.System
{
    public class SystemService
    {
        public List<ConfigSettingViewModel> getConfigSettings()
        {
            string spName = "spGet_System_Settings";
            List<ConfigSettingViewModel> configSettingList = new List<ConfigSettingViewModel>();

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        ConfigSettingViewModel configSetting = new ConfigSettingViewModel();
                        configSetting.Id = Convert.ToInt32(reader["Id"]);
                        configSetting.Name = reader["name"].ToString().Trim();
                        configSetting.Value = reader["value"].ToString().Trim();
                        configSetting.ValueType = Convert.ToInt32(reader["valuetype"]);
                        configSetting.StatusId = Convert.ToInt32(reader["status"]);
                        configSetting.Applied = Convert.ToInt32(reader["applied"]);
                        configSetting.Grouptype = Convert.ToInt32(reader["group"]);
                        configSettingList.Add(configSetting);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    reader.Close();
                }
            }

                return configSettingList;
            }

            public int SaveConfigurationSetting(int id, string value, int userid)
            {
                string spName = "spsave_Sys_ConfigurationSetting";

                using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@value", UtilityService.StaticScrubTextForDb(value));
                    cmd.Parameters.AddWithValue("@userid", userid);

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();
                    var result = returnParameter.Value;

                    return Convert.ToInt32(result);
                }
                
            }

            public int SaveFeeSetting(int id, string value, int userid)
            {
                string spName = "spsave_Sys_FeeSetting";

                using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@rate", UtilityService.StaticScrubNumberForDb(value));

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();
                    var result = returnParameter.Value;

                    return Convert.ToInt32(result);


                }
            }

            public int SaveBBPAFeeSetting(int id, decimal startamount, decimal endamount, decimal value, int userid)
            {
                string spName = "spsave_Sys_BBPAFeeSetting";

                using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@startamount", startamount);
                    cmd.Parameters.AddWithValue("@endamount", endamount);
                    cmd.Parameters.AddWithValue("@amount", value);
                    cmd.Parameters.AddWithValue("@userid", userid);

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();
                    var result = returnParameter.Value;

                    return Convert.ToInt32(result);
                }
                
            }


      
    }
}
