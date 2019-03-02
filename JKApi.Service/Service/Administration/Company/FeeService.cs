using JKApi.Service.Helper;
using JKViewModels.Administration.Company;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Service.Administration.Company
{
    public class FeeService
    {
        #region Fee

        public static List<FeeViewModel> GetFranchiseList(int Franchiseeid)
        {

            string stmt = "select * from vw_f_fee where franid = " + Franchiseeid;
            List<FeeViewModel> feeList = new List<FeeViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(stmt, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    FeeViewModel fee = new FeeViewModel();
                    fee._object = null;
                    fee.Id = Convert.ToInt32(reader["id"].ToString());
                    fee.TypeName = reader["feename"].ToString().Trim();
                    fee.Rate = Convert.ToDecimal(reader["rate"].ToString());
                    fee.RateType = Convert.ToInt32(reader["ratetype"].ToString());
                    fee.TypeId = Convert.ToInt32(reader["trxtypeid"].ToString());
                    fee.Amount = Convert.ToDecimal(reader["amount"].ToString());
                    fee.Description = reader["notes"].ToString().Trim();

                    if (fee._object != null)
                    {
                        fee.RelatedToId = Convert.ToInt32(reader["relatedtoid"].ToString());
                        fee.StatusId = Convert.ToInt32(reader["statusid"].ToString());
                        fee.Status = reader["statusname"].ToString().Trim();
                        fee.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        fee.CreateDate = Convert.ToDateTime( reader["createdate"].ToString());
                        fee.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        fee.ModifiedDate = Convert.ToDateTime( reader["modifieddate"].ToString());
                    }

                    feeList.Add(fee);
                    
                }
                reader.Close();
            }
             
            return feeList;
        }

        public static List<FeeViewModel> GetList()
        {
            string stmt = "select * from vw_sys_Fees_New where feetype = 1 and status = 1 order by feename";
            //string stmt = "select * from vw_sys_Fees where feetype = 1 and status = 1 order by feename";
            List<FeeViewModel> feeList = new List<FeeViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(stmt, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    FeeViewModel fee = new FeeViewModel();
                    fee._object = null;
                    fee.Id = Convert.ToInt32(reader["id"].ToString());
                    fee.TypeName = reader["feename"].ToString().Trim();
                    fee.Rate = Convert.ToDecimal(reader["rate"].ToString());
                    fee.RateType = Convert.ToInt32(reader["ratetype"].ToString());
                    fee.TypeId = Convert.ToInt32(reader["trxtypeid"].ToString());
                    fee.Amount = Convert.ToDecimal(reader["amount"].ToString());
                    fee.Description = reader["notes"].ToString().Trim();

                    if (fee._object != null)
                    {
                        fee.RelatedToId = Convert.ToInt32(reader["relatedtoid"].ToString());
                        fee.StatusId = Convert.ToInt32(reader["statusid"].ToString());
                        fee.Status = reader["statusname"].ToString().Trim();
                        fee.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        fee.CreateDate = Convert.ToDateTime(reader["createdate"].ToString());
                        fee.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        fee.ModifiedDate = Convert.ToDateTime(reader["modifieddate"].ToString());
                    }
                    feeList.Add(fee);
                }// end while

                reader.Close();

            }
            return feeList;
        }

        public static List<FeeViewModel> GetOtherList()
        {
            //string stmt = "select * from vw_sys_Fees_New where feetype = 0 order by feename";
            string stmt = "select * from vw_sys_Fees_New order by feename";
            List<FeeViewModel> feeList = new List<FeeViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(stmt, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    FeeViewModel fee = new FeeViewModel();
                    fee._object = null;
                    fee.Id = Convert.ToInt32(reader["id"].ToString());
                    fee.TypeName = reader["feename"].ToString().Trim();
                    fee.Rate = Convert.ToDecimal(reader["rate"].ToString());
                    fee.RateType = Convert.ToInt32(reader["ratetype"].ToString());
                    fee.TypeId = Convert.ToInt32(reader["trxtypeid"].ToString());
                    fee.Amount = Convert.ToDecimal(reader["amount"].ToString());
                    fee.Description = reader["notes"].ToString().Trim();

                    if (fee._object != null)
                    {
                        fee.RelatedToId = Convert.ToInt32(reader["relatedtoid"].ToString());
                        fee.StatusId = Convert.ToInt32(reader["statusid"].ToString());
                        fee.Status = reader["statusname"].ToString().Trim();
                        fee.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        fee.CreateDate = Convert.ToDateTime(reader["createdate"].ToString());
                        fee.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        fee.ModifiedDate = Convert.ToDateTime(reader["modifieddate"].ToString());
                    }

                    feeList.Add(fee);
                   
                }
                reader.Close();
            }

            return feeList;
        }


        //public void Load(int Id)
        //{
        //    string stmt = "";

        //    if (_object.GetType() == typeof(Contract))
        //    {
        //        stmt = "";

        //    }
        //    else if (_object.GetType() == typeof(Contract.Detail))
        //    {
        //        stmt = "";

        //    }
        //    else if (_object.GetType() == typeof(Franchisee))
        //    {
        //        stmt = "exec spget_F_fee " + Id;
        //    }

        //    _load(stmt);

        //}


        #endregion



        #region Minimum

        public void Delete(int Id)
        {
            string spName = "spDelete_F_FeeOverride";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", Id);
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

            }
            
        }

        

        public MinimumViewModel LoadMinimum(int Id) {

            if (Id == -1) { return null; }

            string spName = "spGet_F_FeeOverride";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Id);
                MinimumViewModel min = new MinimumViewModel();
                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {

                        min.Id = Convert.ToInt32(reader["id"].ToString());
                        min.FeeId = Convert.ToInt32(reader["feeid"].ToString());
                        min.FeeName = reader["feename"].ToString().Trim();
                        min.FranchiseeId = Convert.ToInt32(reader["franid"].ToString());
                        min.StartDate = Convert.ToDateTime(reader["startdate"].ToString());
                        min.Amount = Convert.ToDecimal(reader["minimumamount"].ToString().Trim());
                        min.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        min.CreateDate = Convert.ToDateTime(reader["createddate"].ToString().Trim());
                        min.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        min.ModifiedDate = Convert.ToDateTime(reader["modifieddate"].ToString().Trim());

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

                return min;
            }
        }

        public List<MinimumViewModel> LoadListMinimum(int FranchiseeId)
            {
                if (FranchiseeId == Constants.Empty) { return null; }

                List<MinimumViewModel> list = new List<MinimumViewModel>();
                
                string spName = "spGet_F_FeeOverride";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@franid", FranchiseeId);
                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        MinimumViewModel min = new MinimumViewModel();
                        min.Id = Convert.ToInt32(reader["id"].ToString());
                        min.FeeId = Convert.ToInt32(reader["feeid"].ToString());
                        min.FeeName = reader["feename"].ToString().Trim();
                        min.FranchiseeId = Convert.ToInt32(reader["franid"].ToString());
                        min.StartDate = Convert.ToDateTime(reader["startdate"].ToString());
                        min.Amount = Convert.ToDecimal(reader["minimumamount"].ToString().Trim());
                        min.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        min.CreateDate = Convert.ToDateTime(reader["createddate"].ToString().Trim());
                        min.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        min.ModifiedDate = Convert.ToDateTime(reader["modifieddate"].ToString().Trim());

                        list.Add(min);

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

                    return list;

                }

        public int SaveMinimum(int Id, int FeeId, int FranchiseeId, decimal Amount, string Startdate, int UserId)
        {
            string spName = "spSave_F_FeeOverride";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@value", FeeId);
                cmd.Parameters.AddWithValue("@value", FranchiseeId);
                cmd.Parameters.AddWithValue("@value", Amount);
                cmd.Parameters.AddWithValue("@value", UtilityService.StaticScrubDateForDb(Startdate));
                cmd.Parameters.AddWithValue("@userid", UserId);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                con.Close();

                return Convert.ToInt32(result);
            }
            
        }

        #endregion



    }
}
