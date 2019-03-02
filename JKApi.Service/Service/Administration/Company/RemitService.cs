using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.Administration.Company;

namespace JKApi.Service.Service.Administration.Company
{

    public class RemitService
    {
        public int Save(String Id, string RegionId, string adddress, string City, string State, string Zip, string userid)
        {
            string spName = "spSave_RegionRemitTo";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@regionid", RegionId);
                cmd.Parameters.AddWithValue("@city", UtilityService.StaticScrubTextForDb(City));
                cmd.Parameters.AddWithValue("@state", UtilityService.StaticScrubTextForDb(State));
                cmd.Parameters.AddWithValue("@postalcode", UtilityService.StaticScrubTextForDb(Zip));
                cmd.Parameters.AddWithValue("@userid", userid);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }



        }

        public int Clear(int RegionId)
        {

            string spName = "spDelete_RegionRemitTo";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@regionid", RegionId);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);

            }
        }

        public static RemitToViewModel Load(int RegionId)
            {

            RemitToViewModel remit = new RemitToViewModel();

            string spName = "spGet_RegionRemitTo";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@regionid", RegionId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    remit.Id = Convert.ToInt32(reader["RemitToid"].ToString());
                    remit.RegionId = Convert.ToInt32(reader["regionid"].ToString());
                    remit.Address = reader["address"].ToString().Trim();
                    remit.City = reader["city"].ToString().Trim();
                    remit.State = reader["state"].ToString().Trim();
                    remit.Zip = reader["postalcode"].ToString().Trim();
                }
                reader.Close();

            }

            return remit;

            }

        

    }
    
}
