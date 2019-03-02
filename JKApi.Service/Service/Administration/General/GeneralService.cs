using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using JKViewModels.Administration.System;
using JKViewModels.Administration.Company;
using JKApi.Service.Helper;
using JKApi.Service.ServiceContract.JKControl;
using JKApi.Data.DAL;
//using JKApi.Data.JkControl;
using JKViewModels.Administration.General;
using JKApi.Service.Helper.Extension;

namespace JKApi.Service.Service.Administration.General
{
    public class GeneralService : IGeneralService
    {
        public void DeleteCPIById(int id)
        {
            //using (var context = new JkControlEntities())
            //{
            //    var result = context.spdelete_CPI(id);
            //}
            string spName = "spdelete_CPI";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

            }
        }

        // public IEnumerable<spget_CPIList_Result> GetCPIList()
        public IEnumerable<CPI> GetCPIList()
        {
            using (var context = new Data.DAL.jkDatabaseEntities())
            {
                var result = context.CPIs.ToList();
                return result;
            }
        }

        public string SaveCPI(Nullable<int> id, Nullable<int> billmonth, Nullable<int> billyear, Nullable<decimal> percent, string description, Nullable<int> applied, Nullable<int> userid)
        {
            //using (var context = new JkControlEntities())
            //{
            //    id = -1;applied = 0;
            //    var result = context.spsave_CPI(id, billmonth, billyear, percent, description, applied, userid);
            //    return result.ToString();
            //}
            string spName = "spsave_CPI";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                id = -1; applied = 1; userid = 5610;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@billmonth", billmonth);
                cmd.Parameters.AddWithValue("@billyear", billyear);
                cmd.Parameters.AddWithValue("@percent", percent);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@applied", applied);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.ExecuteNonQuery();
                string str = "success";
                return str;
            }
        }
        //public string SaveCPI(CPIViewModel model)
        //{
        //    //using (var context = new JkControlEntities())
        //    //{
        //    //    id = -1;applied = 0;
        //    //    var result = context.spsave_CPI(id, billmonth, billyear, percent, description, applied, userid);
        //    //    return result.ToString();
        //    //}


        //    using (var context = new JkControlEntities())
        //    {
        //        var data = new tbl_sys_CPI
        //        {

        //            billmonth = model.billmonth,
        //            billyear = model.billyear,
        //            percent = model.percent,
        //            description = model.description,
        //            applied = model.applied,
        //            createdby = 5610,
        //            createdate = DateTime.UtcNow,
        //            modifiedby = 5610,
        //            modifieddate = DateTime.UtcNow
        //        };
        //        context.tbl_sys_CPI.Add(data);
        //        context.SaveChanges();
        //        string str = "success";
        //        return str;
        //    }
        //}
        public string EditCPI(CPIViewModel model)
        {
           
            using (var context = new Data.DAL.jkDatabaseEntities())
            {
                var data = context.CPIs.Where(c => c.CPIId.Equals(model.id)).FirstOrDefault();
                if (data != null)
                {

                    data.BillMonth = model.billmonth;
                    data.BillYear = model.billyear;
                    data.CPIPpercent = model.percent;
                    data.Description = model.description;
                    data.Applied = model.applied;
                    data.CreatedBy = 5610;
                    data.CreatedOn = DateTime.UtcNow;
                    data.ModifiedBy = 5610;
                    data.ModifiedOn = DateTime.UtcNow;
                    context.SaveChanges();
                }
             
                string str = "success";
                return str;
            }
        }
        public CPIViewModel SearchCPIById(int id)
        {
            CPIViewModel cpi = new CPIViewModel();
            using (var context = new jkDatabaseEntities())
            {
                var result = context.CPIs.Where(o=>o.CPIId==id).MapEnumerable<CPIViewModel, CPI>().ToList();
                //cpi.id = result.id;
                //cpi.billmonth = result.billmonth;
                //cpi.billyear = result.billyear;
                //cpi.percent = result.percent;
                //cpi.description = result.description;
                //cpi.applied = result.applied;
                //return cpi;
                return result.FirstOrDefault();
            }
            
        }

        public string UpdateCPI(Nullable<int> id, Nullable<int> billmonth, Nullable<int> billyear, Nullable<decimal> percent, string description, Nullable<int> applied, Nullable<int> userid)
        {
            //using (var context = new JkControlEntities())
            //{
            //    var result = context.spsave_CPI(id, billmonth, billyear, percent, description, applied, userid);
            //    return result.ToString();
            //}
            string spName = "spsave_CPI";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                applied = 1; userid = 5610;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@billmonth", billmonth);
                cmd.Parameters.AddWithValue("@billyear", billyear);
                cmd.Parameters.AddWithValue("@percent", percent);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@applied", applied);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.ExecuteNonQuery();
                string str = "success";
                return str;
            }

        }

        public string PaymentGatewayInsertService(PaymentGatewayDetail paymentGatewayDetail)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            if (paymentGatewayDetail.Id <= 0)
            {
                jkDatabaseEntities.PaymentGatewayDetails.Add(paymentGatewayDetail);
            }
            else
            {
                PaymentGatewayDetail pm = jkDatabaseEntities.PaymentGatewayDetails.Where(r => r.Id == paymentGatewayDetail.Id).FirstOrDefault();
                if(pm != null)
                {
                    pm.PaymentGateway = paymentGatewayDetail.PaymentGateway;
                    pm.TransactionKey = paymentGatewayDetail.TransactionKey;
                    pm.merchantid = paymentGatewayDetail.merchantid;
                    pm.LoginID = paymentGatewayDetail.LoginID;
                    pm.IsActive = paymentGatewayDetail.IsActive;
                }
            }
            jkDatabaseEntities.SaveChanges();
            //await jkDatabaseEntities.Database.SqlQuery<Task>("EXEC SpSetActivePaymentGateway {0}", paymentGatewayDetail.Id).ToArrayAsync();

            jkDatabaseEntities.SpSetActivePaymentGateway(paymentGatewayDetail.Id);
            return "success";
        }
       
        public void InsertCCtransaction(List<CCTransaction> cCTransaction)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            foreach(CCTransaction cc in cCTransaction)
            {
                jkDatabaseEntities.CCTransactions.Add(cc);
                jkDatabaseEntities.SaveChangesAsync();
            }
        }


        public List<PaymentGatewayDetail> GetPaymentGatewayList()
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            List<PaymentGatewayDetail> list = jkDatabaseEntities.PaymentGatewayDetails.ToList();
            return list;
        }

        public portal_SpGetCustomersDataUsingInoviceID_Result AddressList(string ClassID)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            var data = jkDatabaseEntities.portal_SpGetCustomersDataUsingInoviceID(ClassID).FirstOrDefault();
            return data;
        }

        public PaymentProfileDetail PaymentProfileDetail(string ClassID)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            int CLassID = Convert.ToInt32(ClassID);
            var data = jkDatabaseEntities.PaymentProfileDetails.Where(r => r.FKClassId == CLassID && r.FKTypeListId == 1 && r.AccountNumber != null && r.CustomerPaymentProfileID != null && r.CustomerProfileID != null).FirstOrDefault();
            return data;
        }

        #region OrderTransaction

        public OrderTransaction OrderTransaction(OrderTransaction orderTransaction)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            jkDatabaseEntities.OrderTransactions.Add(orderTransaction);
            jkDatabaseEntities.SaveChangesAsync();
            return orderTransaction;
        }

        public OrderTransactionsResponse OrderTransactionsResponse(OrderTransactionsResponse orderTransactionsResponse)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            jkDatabaseEntities.OrderTransactionsResponses.Add(orderTransactionsResponse);
            jkDatabaseEntities.SaveChangesAsync();

            return orderTransactionsResponse;
        }

        public PaymentProfileDetail InsertPaymentProfileDetail(PaymentProfileDetail paymentProfileDetail)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();
            jkDatabaseEntities.PaymentProfileDetails.Add(paymentProfileDetail);
            jkDatabaseEntities.SaveChangesAsync();
            return paymentProfileDetail;
        }

        public PaymentProfileDetail UpdatePaymentProfile(PaymentProfileDetail paymentProfileDetail)
        {
            jkDatabaseEntities jkDatabaseEntities = new jkDatabaseEntities();

            PaymentProfileDetail PPD = jkDatabaseEntities.PaymentProfileDetails.Where(r => r.Id == paymentProfileDetail.Id).FirstOrDefault();

            if (PPD != null)
            {
                PPD.AccountNumber =paymentProfileDetail.AccountNumber;
                PPD.AccountType =paymentProfileDetail.AccountType;
                PPD.CustomerProfileID = paymentProfileDetail.CustomerProfileID;
                PPD.CustomerPaymentProfileID = paymentProfileDetail.CustomerPaymentProfileID;
            }
            jkDatabaseEntities.SaveChangesAsync();
            return paymentProfileDetail;
        }

        #endregion
    }
}

