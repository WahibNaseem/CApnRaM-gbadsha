using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace JKApi.Service.Service
{
   public class DBService
    {
        static string connectionStringJkBuf = System.Configuration.ConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString;
         
        static JKApi.Data.DAL.jkDatabaseEntities contextJkBuf = new Data.DAL.jkDatabaseEntities();
        static SqlConnection con = null;


        public static string getConnectionStringJkBuf()
        {
            return new EntityConnectionStringBuilder(connectionStringJkBuf).ProviderConnectionString;
            //return contextJkBuf..StoreConnection.ConnectionString; // connectionStringJkBuf;
        }
 

        public static SqlConnection getConnnectionJkBuf()
        {
            try {

                if (con == null)
                {
                    con = new SqlConnection(connectionStringJkBuf);
                    //con.Open();
                }

            }
            catch( Exception e)
            {
                Console.WriteLine(e.Message); 
            }

            return con;

        }

        public static SqlConnection getConnnectionJkControl()
        {
            try
            {

                if (con == null)
                {
                    con = new SqlConnection(getConnectionStringJkBuf());
                    //con.Open();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return con;

        }

        public static SqlDataReader executeSP(string spName, List<string> paramz)
        {
            SqlDataReader reader = null;

            SqlCommand cmd = new SqlCommand(spName, getConnnectionJkBuf());

            string stmt = "exec  " + spName + " ";
            foreach (string s in paramz)
            {
                stmt += s + ",";
            }

            stmt = stmt.Substring(0, stmt.Length - 1);
            reader = cmd.ExecuteReader();
            
            return reader;
        }


        //public static List<Customer> GetOutOfSuspensionCustomers(oJKDB odb, int BillMonth, int BillYear)
        //{
        //    string spName = "spGet_C_SuspendedCustomers";
        //    List<string> paramz = new List<string>();
        //    paramz.Add(BillMonth.ToString());
        //    paramz.Add(BillYear.ToString());
        //    JKApi.Data.DAL.
        //    List<Customer> ocustomerList = new List<Customer>();
        //    SqlDataReader odata = executeSP(spName, paramz);
        //    while (odata.Read())
        //    {
        //        Customer c = new Customer();
        //        c.Id = Convert.ToInt32(odata["customerid"]);
        //        c.Name = odata["name"].ToString();
        //        c.Number = odata["customerNo"].ToString();
        //        c.ResumeDate.Value = odata["resumedate"];
        //        ocustomerList.Add(c);
        //    }

        //    odata.Close();
        //    return ocustomerList;

        //}




    }
}
