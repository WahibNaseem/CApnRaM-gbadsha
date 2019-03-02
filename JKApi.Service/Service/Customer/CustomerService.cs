using Dapper;
using JK.Repository.Uow;
using JKApi.Data.DAL;
using JKApi.Data.DTOObject;
using JKApi.Service.AccountReceivable;
using JKApi.Service.Helper;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service.File;
using JKApi.Service.ServiceContract.Customer;
using JKViewModels.AccountReceivable;
using JKViewModels.Common;
using JKViewModels.CRM;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Franchisee;
using JKViewModels.Generic;
using JKViewModels.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace JKApi.Service.Service.Customer
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICommonService _commonService;

        #region ConstructorCalls

        public CustomerService(IJKEfUow uow, ICommonService commonService)
        {
            Uow = uow;
            _commonService = commonService;
        }

        public CustomerService()
        {
            _commonService = new CommonService();
        }

        #endregion

        static JKApi.Data.DAL.jkDatabaseEntities jkEntityModel = new Data.DAL.jkDatabaseEntities();


        public IQueryable<JKApi.Data.DAL.Address> GetAddressByCustomerId(int id)
        {
            var qry = Uow.Address.GetAll().Where(x => x.ClassId == id && x.TypeListId == 1);
            return qry;
        }

        #region Methods

        public CustomerMaintenanceApproval CustomerMaintenanceApprovalByTempMaintenanceID(int MaintenanceTempId)
        {
            CustomerMaintenanceApproval oApproval = new CustomerMaintenanceApproval();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                MaintenanceTemp Maintenance = context.MaintenanceTemps.Where(x => x.MaintenanceTempId == MaintenanceTempId).FirstOrDefault();
                if (Maintenance != null)
                {
                    oApproval.MaintenanceTempId = Maintenance.MaintenanceTempId;
                    oApproval.CustomerId = Convert.ToInt32(Maintenance.ClassId);
                    oApproval.StatusListId = Convert.ToInt32(Maintenance.StatusListId);
                    oApproval.EffectiveDate = Convert.ToDateTime(Maintenance.EffectiveDate);
                    oApproval.ResumeDate = Convert.ToDateTime(Maintenance.ResumeDate);
                    oApproval.ReasonListId = Convert.ToInt32(Maintenance.StatusReasonListId);
                    oApproval.Comments = Convert.ToString(Maintenance.Comments);



                }
            }
            return oApproval;
        }

        public static List<NoteViewModel> GetCustomerNote(int CustomerId)
        {
            string spName = "spGet_C_Note";
            List<NoteViewModel> l = new List<NoteViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerId", CustomerId);
                NoteViewModel c = null;
                SqlDataReader odata = cmd.ExecuteReader();
                try
                {
                    if (odata.Read())
                    {
                        c = new NoteViewModel();
                        c.Id = Convert.ToInt32(odata["id"].ToString());
                        c.CNote = odata["note"].ToString();
                        c.CreatedDate = Convert.ToDateTime(odata["createddate"].ToString());
                        l.Add(c);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    odata.Close();
                }
            }

            return l;

        }

        //Gets billing email address
        public static string GetMainEmailAddress(int CustomerId)
        {
            string result = "";

            var data = (from f in jkEntityModel.vw_C_Contact
                        select f).Where(cc => cc.contactTypeId == 1 && cc.customerId == CustomerId).FirstOrDefault();

            if (data != null)
            {
                result = data.email.Trim();
            }

            return result;

        }

        //Gets billing email address
        public static string GetBillingEmailAddress(int CustomerId)
        {
            string spName = "spGet_C_EmailAddressesBilling";
            string result = "";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerid", CustomerId);

                SqlDataReader odata = cmd.ExecuteReader();
                try
                {
                    while (odata.Read())
                    {
                        if (odata["email"].ToString().Trim() != "")
                        {
                            result += odata["email"].ToString().Trim() + Constants.SemiColon;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    odata.Close();
                }
            }

            if (result != "") { result = result.Substring(0, result.Length - 1); }
            return result;
        }


        public List<ARStatu> getARStatusList()
        {
            var data = (from f in jkEntityModel.ARStatus
                        select f).OrderBy(cc => cc.ARStatusId).ToList();

            return data;
        }

        public List<InvoiceDateList> getInvoiceDate()
        {
            var data = (from f in jkEntityModel.InvoiceDateLists
                        select f).OrderBy(cc => cc.Name).ToList();

            return data;
        }
        public List<InvoiceTermList> getTermDate()
        {
            var data = (from f in jkEntityModel.InvoiceTermLists
                        select f).OrderBy(cc => cc.Name).ToList();

            return data;
        }
        //public List<Data.JkControl.vw_sys_term> getTermDate()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_term
        //                select f).OrderBy(cc => cc.name).ToList();

        //    return data;
        //}


        public Dictionary<string, string> _GetStateList()
        {
            var data = Uow.StateList.GetAll();
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.ToList().Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.StateListId.ToString(), y.abbr.ToString());
                }
            }
            return dataDict;
        }

        public Dictionary<string, string> _GetAccountTypeList()
        {
            var data = Uow.AccountTypeList.GetAll();
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.ToList().Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.AccountTypeListId.ToString(), Convert.ToString(y.Name));
                }
            }
            return dataDict;
        }
        public Dictionary<string, string> _GetCleanFrequencyList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = context.CleanFrequencyLists.MapEnumerable<CleanFrequencyListViewModel, CleanFrequencyList>().ToList();




                Dictionary<string, string> dataDict = new Dictionary<string, string>();
                if (data.Count > 0)
                {
                    foreach (var y in data)
                    {
                        dataDict.Add(y.CleanFrequencyListId.ToString(), y.Name.ToString());
                    }
                }
                return dataDict;
            }
        }
        public Dictionary<string, string> getTaxAuthority()
        {
            var data = (from f in jkEntityModel.vw_Sys_CountyTaxAuthority
                        select f).OrderBy(cc => cc.county).ToList();

            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.id.ToString(), y.county.ToString());
                }
            }

            return dataDict;
        }
        public Dictionary<string, string> getSoldBy()
        {
            var data = (from f in jkEntityModel.vw_Sys_CountyTaxAuthority
                        select f).OrderBy(cc => cc.county).ToList();

            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.id.ToString(), y.county.ToString());
                }
            }

            return dataDict;
        }
        public Dictionary<string, string> _GetStatusList(int typelistid)
        {
            var data = Uow.StatusList.GetAll().Where(s => s.TypeListId == typelistid).ToList();
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            if (data.ToList().Count > 0)
            {
                foreach (var y in data)
                {
                    dataDict.Add(y.StatusListId.ToString(), y.Name.ToString());
                }
            }

            return dataDict;
        }


        //public Dictionary<string, string> _GetStatusList()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_C_Status
        //                select f).OrderBy(cc => cc.name).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    if (data.Count > 0)
        //    {
        //        foreach (var y in data)
        //        {
        //            dataDict.Add(y.id.ToString(), y.name.ToString());
        //        }
        //    }

        //    return dataDict;
        //}

        //public Dictionary<string, string> GetCAccountType()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_C_AccountTypes
        //                select f).OrderBy(cc => cc.name).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    if (data.Count > 0)
        //    {
        //        foreach (var y in data)
        //        {
        //            dataDict.Add(y.id.ToString(), y.name.ToString());
        //        }
        //    }

        //    return dataDict;

        //}

        //public Dictionary<string, string> GetUsStatesList()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_State
        //                select f).OrderBy(cc => cc.Name).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    //if (data > 0)
        //    //{
        //    //    foreach (var y in data)
        //    //    {
        //    //        dataDict.Add(y.Id.ToString(), y.Name.ToString());
        //    //    }
        //    //}

        //    return dataDict;

        //}

        public IQueryable<MasterTrxTypeList> GetMasterTrxTypeListsForCustomers()
        {
            var qry = Uow.MasterTrxTypeList.GetAll().Where(x => x.TypeListId == 1 || x.TypeListId == null);
            return qry;
        }

        //public List<Data.JkControl.vw_sys_State> GetStatesList()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_State
        //                select f).OrderBy(cc => cc.Name).ToList();

        //    return data;

        //}

        public string GetStatesName(int id)
        {
            var data = Uow.StateList.GetAll().Where(x => x.StateListId == id).Select(x => new { x.abbr }).FirstOrDefault();
            string name = data.abbr.ToString();
            return name;
        }
        public int GetStateId(string state)
        {
            var data = Uow.StateList.GetAll().Where(x => x.abbr == state || x.Name == state).Select(x => x.StateListId).FirstOrDefault();
            return Convert.ToInt32(data);
        }
        //public static string GetBillingTermName(int id)
        //{
        //    string termName = "";

        //    var data = (from b in jkControlEntites.vw_sys_term
        //                select b).Where(b => b.id == id).OrderBy(c => c.name).ToList();


        //    if (data.Count > 0)
        //    {
        //        foreach (var d in data)
        //        {
        //            termName = d.name;
        //        }
        //    }

        //    return termName;
        //}


        //public Dictionary<string, string> _GetFrequencyList()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_Frequency
        //                select f).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    if (data.Count > 0)
        //    {
        //        foreach (var y in data)
        //        {
        //            dataDict.Add(y.abbr.ToString(), y.name.ToString());
        //        }
        //    }

        //    return dataDict;
        //}

        //public static Dictionary<string, string> GetStatusReasonList()
        //{
        //    var data = (from f in jkControlEntites.vw_sys_C_StatusReason
        //                select f).OrderBy(cc => cc.reason).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    if (data.Count > 0)
        //    {
        //        foreach (var y in data)
        //        {
        //            dataDict.Add(y.id.ToString(), y.reason.ToString());
        //        }
        //    }

        //    return dataDict;
        //}

        //public static Dictionary<string, string> GetCancelledStatusReasonList(ref List<string> Display, ref List<string> Values)
        //{
        //    var data = (from f in jkControlEntites.vw_sys_CancelReason
        //                select f).OrderBy(cc => cc.reason).ToList();

        //    Dictionary<string, string> dataDict = new Dictionary<string, string>();
        //    if (data.Count > 0)
        //    {
        //        foreach (var y in data)
        //        {
        //            dataDict.Add(y.id.ToString(), y.reason.ToString());
        //        }
        //    }

        //    return dataDict;
        //}

        //public static List<Customer> GetOutOfSuspensionCustomers(oJKDB odb, int BillMonth, int BillYear)
        //{
        //    //MG -- 8/26/2014 -- Added customerid as a parameter to GetIncompleteDistribution. Pass empty (-1) to get all customers.            
        //    string stmt = "exec spGet_C_SuspendedCustomers " + BillMonth + "," + BillYear;
        //    List<Customer> ocustomerList = new List<Customer>();
        //    SqlDataReader odata = odb.GetData(stmt);
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


        public static List<CustomerViewModel> GetIncompleteDistribution(int CustomerID)
        {
            string spName = "spGet_C_IncompleteDistribution";
            List<CustomerViewModel> custList = new List<CustomerViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerid", CustomerID);

                SqlDataReader odata = cmd.ExecuteReader();
                try
                {
                    CustomerViewModel c = new CustomerViewModel();
                    //c.Id = Convert.ToInt32(odata["customerid"]);
                    //c.Name = odata["name"].ToString();
                    //c.Number = odata["customerNo"].ToString();
                    custList.Add(c);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    odata.Close();
                }
            }

            return custList;

        }
        public static List<CustomerViewModel> GetPending(int CustomerID)
        {
            string spName = "spGet_C_Pending";
            List<CustomerViewModel> custList = new List<CustomerViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerid", CustomerID);

                SqlDataReader odata = cmd.ExecuteReader();
                try
                {
                    CustomerViewModel c = new CustomerViewModel();
                    //c.Id = Convert.ToInt32(odata["customerid"]);
                    //c.Name = odata["name"].ToString();
                    //c.Number = odata["customerNo"].ToString();
                    custList.Add(c);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    odata.Close();
                }
            }

            return custList;

        }

        /*4-13-15 - MG -- Returns the id of vw_sys_c_accounttypesgroup or the id of the vw_sys_c_accounttypesgroup 
         depending on what the value is selected on the Account types drop down. If a value with a G is passed, 
         this means the user selected a group entry from the droip down.*/
        public static string GetAccountType(string Val, ref string AcctTypeId, ref string AcctTypeGroupId)
        {
            if (Val != Constants.Empty.ToString())
            {
                if (Val.Substring(0, 1) == "G")
                {
                    return AcctTypeGroupId = Val.Substring(1);
                }
                else
                {
                    return AcctTypeId = Val;
                }
            }
            else
            {
                return AcctTypeId = Constants.Empty.ToString();
            }
        }

        public List<JKApi.Data.DAL.Customer> GetCustomerListData(string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                List<JKApi.Data.DAL.Customer> lstCustomer = context.Customers.Where(c => (c.RegionId == SelectedRegionId || SelectedRegionId == 0) && c.Name != null).ToList();
                //context.Configuration.ProxyCreationEnabled = true;
                return lstCustomer;
            }
        }


        public string hasExistManintenance(int customerid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                var result = (from mt in context.MaintenanceTemps
                              join mtl in context.MaintenanceTypeLists
                                on mt.MaintenanceTypeListId equals mtl.MaintenanceTypeListId
                              where mt.ClassId == customerid && mt.TypeListId == 1 && mt.IsActive == true
                              select new
                              {
                                  mt.MaintenanceTempId,
                                  MaintenanceTempName = mtl.Name
                              }).ToList().FirstOrDefault();
                string resultSTR = "|";
                if (result != null)
                    resultSTR = result.MaintenanceTempId + "|" + result.MaintenanceTempName;


                return resultSTR;
            }
        }

        public List<CustomerSearchModel> GetSearchCustomer(string searchText)
        {
            List<CustomerSearchModel> lstFranchiseeSearch = new List<CustomerSearchModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SearchText", searchText);
                parmas.Add("@RegionId", SelectedRegionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerSearch", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeSearch = multipleresult.Read<CustomerSearchModel>().ToList();
                    }
                }
            }
            return lstFranchiseeSearch;
        }




        public portal_spGet_C_Distribution_Result GetDistributionDetailData(int customerid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                portal_spGet_C_Distribution_Result oDistributionDetail = context.portal_spGet_C_Distribution(customerid).FirstOrDefault();
                return oDistributionDetail;
            }
        }

        public SpGetCustomerDetailsByCustomerID_Result SpGetCustomerDetailsByCustomerID(int ClassID)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                SpGetCustomerDetailsByCustomerID_Result oDistributionDetail = context.SpGetCustomerDetailsByCustomerID(ClassID).FirstOrDefault();
                return oDistributionDetail;
            }
        }

        //lahwinder methods
        public Dictionary<string, string> getFrancisesByCustomerId(int custId)
        {

            Dictionary<string, string> lstFrancises = new Dictionary<string, string>();
            try
            {
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    var lst = (from f in context.Franchisees
                               join d in context.Distributions
                               on f.FranchiseeId equals d.FranchiseeId
                               where d.CustomerId == custId
                               select new { f.FranchiseeId, f.FranchiseeNo, f.Name }).ToList();
                    foreach (var item in lst)
                    {
                        lstFrancises.Add(item.FranchiseeId.ToString(), (item.FranchiseeNo + " " + item.Name));
                    }

                }
            }
            catch { }

            return lstFrancises;
        }

        //lahwinder methods
        #endregion

        #region OldCode
        //public ContractViewModel LoadContract(int Id, int customerid)
        //{
        //    if (Id == Constants.Empty && customerid == Constants.Empty)
        //    { return null; }
        //    string spName = "spget_C_Contract";
        //    ContractViewModel c = null;
        //    using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand(spName, con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@id", Id);
        //        cmd.Parameters.AddWithValue("@customerId", customerid);

        //        SqlDataReader odata = cmd.ExecuteReader();
        //        try
        //        {
        //            if (odata.Read())
        //            {
        //                c = new ContractViewModel();
        //                c.Id = Convert.ToInt32(odata["id"].ToString());
        //                c.CustomerId = Convert.ToInt32(odata["customerid"].ToString());
        //                c.StatusId = Convert.ToInt32(odata["status"].ToString());
        //                c.AccountTypeId = Convert.ToInt32(odata["accounttypeid"].ToString());
        //                c.ContractTypeId = Convert.ToInt32(odata["contracttypeid"].ToString());
        //                c.PurchaseOrderNo = odata["purchaseorderno"].ToString().Trim();
        //                c.ContractBilling = Convert.ToDecimal(odata["amount"].ToString());
        //                c.BillingSubjectToFees = Convert.ToDecimal(odata["amountsubjecttofee"].ToString());
        //                c.SignDate = Convert.ToDateTime(odata["signdate"].ToString().Trim());
        //                c.StartDate = Convert.ToDateTime(odata["StartDate"].ToString().Trim());
        //                c.EndDate = Convert.ToDateTime(odata["ExpirationDate"].ToString().Trim());
        //                c.SquareFootage = odata["SquareFootage"].ToString().Trim();
        //                c.Frequency = odata["Frequency"].ToString();
        //                c.CleanTimes = Convert.ToInt32(odata["Cleantimes"].ToString());
        //                c.Monday = Convert.ToInt32(odata["Mon"].ToString());
        //                c.Tuesday = Convert.ToInt32(odata["Tue"].ToString());
        //                c.Wednesday = Convert.ToInt32(odata["Wed"].ToString());
        //                c.Thursday = Convert.ToInt32(odata["Thu"].ToString());
        //                c.Friday = Convert.ToInt32(odata["Fri"].ToString());
        //                c.Saturday = Convert.ToInt32(odata["Sat"].ToString());
        //                c.Sunday = Convert.ToInt32(odata["Sun"].ToString());
        //                c.Qualified = Convert.ToInt32(odata["qualified"].ToString());
        //                c.CreatedBy = Convert.ToInt32(odata["createdby"].ToString());
        //                c.CreateDate = Convert.ToDateTime(odata["createdate"].ToString());
        //                c.ModifiedBy = Convert.ToInt32(odata["modifiedby"].ToString());
        //                c.ModifiedDate = Convert.ToDateTime(odata["modifieddate"].ToString());
        //                c.Terms = Convert.ToInt32(odata["terms"].ToString());
        //                c.SoldById = Convert.ToInt32(odata["soldbyid"].ToString());
        //                c.tmpSalesPerson = odata["tmp_soldby"].ToString().Trim();

        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.StackTrace);
        //        }
        //        finally
        //        {
        //            odata.Close();
        //        }
        //    }

        //    return c;

        //}
        public TaxViewModel LoadTax(object _object)
        {
            int id = Convert.ToInt32(_object.GetType().GetField("Id").GetValue(_object).ToString());
            TaxViewModel t = null;
            if (id == Constants.Empty)
            { return null; }
            string spName = "";
            if (_object.GetType() == typeof(FranchiseeViewModel))
            {
                spName = "spget_F_Tax ";
            }
            else if (_object.GetType() == typeof(CustomerViewModel))
            {
                spName = "spget_C_Tax ";
            }

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerid", id);

                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        t = new TaxViewModel();
                        t.Id = Convert.ToInt32(reader["id"].ToString());

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

            return t;

        }
        private void SetFranchiseResponsibility(List<FranchiseeViewModel> fs, int customerId)
        {
            var data = jkEntityModel.vw_C_TrxDistribution
                        .Where(b => b.customerid == customerId).FirstOrDefault();

            if (data != null)
            {
                if (fs != null)
                {
                    FranchiseeViewModel f = fs.Find(m => m.FranchiseeId == data.franid);
                    if (f != null)
                    {
                        f.FranchiseeNo = Convert.ToString(data.rate);
                        f = null;
                    }
                }
            }

        }

        public CustomerViewModel loadCustomer(int id)
        {
            CustomerViewModel c = new CustomerViewModel();
            string spName = "spget_C_Information";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader odata = cmd.ExecuteReader();
                try
                {
                    if (odata.Read())
                    {
                        //c.Id = Convert.ToInt32(odata["id"].ToString());
                        //c.RegionId = Convert.ToInt32(odata["regionid"].ToString());
                        //c.Number = odata["customerno"].ToString().Trim();
                        //c.Name = odata["name"].ToString().Trim();
                        //c.Address = odata["address1"].ToString().Trim();
                        //c.Address2 = odata["address2"].ToString().Trim();
                        //c.City = odata["city"].ToString().Trim();
                        //c.StateAbbr = odata["statename"].ToString().Trim();
                        //c.PostalCode = odata["postalcode"].ToString().Trim();
                        //c.StatusId = Convert.ToInt32(odata["status"].ToString());

                        //c.ServiceAddressSameAsMain = Convert.ToInt32(odata["sameserviceaddress"].ToString());
                        //c.BillingAddressSameAsMain = Convert.ToInt32(odata["samebillingaddress"].ToString());
                        //c.BillingContactSaveAsMain = Convert.ToInt32(odata["samebillingcontact"].ToString());

                        //c.MainPhone = UtilityService.FormatPhone(odata["phone"].ToString().Trim());
                        //c.MainFax = UtilityService.FormatPhone(odata["fax"].ToString().Trim());

                        //if (c.BillingAddressSameAsMain == Constants.Yes)
                        //{
                        //    c.BillingName = c.Name;
                        //    c.BillingAddress = c.Address;
                        //    c.BillingAddress2 = c.Address2;
                        //    c.BillingCity = c.City;
                        //    c.BillingState = c.StateAbbr;
                        //    c.BillingPostalCode = c.PostalCode;
                        //}
                        //else
                        //{
                        //    c.BillingName = odata["billname"].ToString().Trim();
                        //    c.BillingAddress = odata["billaddress1"].ToString().Trim();
                        //    c.BillingAddress2 = odata["billaddress2"].ToString().Trim();
                        //    c.BillingCity = odata["billcity"].ToString().Trim();
                        //    c.BillingState = odata["billstate"].ToString().Trim();
                        //    c.BillingPostalCode = odata["billpostalcode"].ToString().Trim();
                        //}

                        //c.Parent = Convert.ToInt32(odata["parent"].ToString());
                        //c.ParentId = Convert.ToInt32(odata["parentid"].ToString());
                        //c.Reference = Convert.ToInt32(odata["reference"].ToString());
                        //c.NationalAccount = Convert.ToInt32(odata["nationalaccount"].ToString());

                        //c.CreatedBy = Convert.ToInt32(odata["createdby"].ToString());
                        //c.CreateDate = Convert.ToDateTime(odata["createdate"].ToString());
                        //c.ModifiedBy = Convert.ToInt32(odata["modifiedby"].ToString());
                        //c.ModifiedDate = Convert.ToDateTime(odata["modifieddate"].ToString());
                        //c.StatusDate = Convert.ToDateTime(odata["statusdate"].ToString());
                        //c.StatusNotes = odata["statusnotes"].ToString().Trim();
                        //c.ResumeDate = Convert.ToDateTime(odata["resumeDate"].ToString());
                        //c.LastServiceDate = Convert.ToDateTime(odata["lastservicedate"].ToString());
                        //if (c.StatusId == Constants.Suspended || c.StatusId == Constants.Cancelled)
                        //{ c.ReadOnly = Constants.Yes; }
                        //c.StatusReasonId = Convert.ToInt32(odata["reasonid"].ToString());
                        //c.StatusReason = odata["statusreason"].ToString().Trim();

                        ///****** contacts *******/
                        //c.MainContact = new ContactViewModel();
                        //c.MainContact = LoadContact(Constants.Empty, c.Id, ContactViewModel.ContactType_Main);

                        //c.BillingContact = new ContactViewModel();
                        //if (c.BillingContactSaveAsMain == Constants.Yes)
                        //{
                        //    c.BillingContact = c.MainContact;
                        //}
                        //else
                        //{
                        //    c.BillingContact = LoadContact(Constants.Empty, c.Id, ContactViewModel.ContactType_Billing);
                        //}

                        //if (c.StatusId == Constants.Cancelled)
                        //    c.Disabled = Constants.Yes;


                        ///*07-20-2017 FOXPRO RELATED REMOVE 
                        // * This is setting a group based on customer postal code
                        // * ask Carlos
                        //Group = _setGroup(this.PostalCode);
                        // * */

                        //c.Standing = Convert.ToInt32(odata["standingid"].ToString());
                        //c.StandingName = odata["standingname"].ToString().Trim();
                        //c.OpenClaim = Convert.ToInt32(odata["openclaim"].ToString());

                        //c.BillingAttention = odata["billingattention"].ToString().Trim();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    odata.Close();
                }
                return c;

            }

        }
        public int HasChildren(int custId)
        {
            int result = Constants.No;
            string spName = "spGet_C_InformationChildren";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerId", custId);

                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        result = Constants.Yes;
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

            return result;

        }


        public int deleteContactType(int CustomerId, int ContactType)
        {
            string spName = "spdelete_C_ContactType";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@customerid", CustomerId);
                cmd.Parameters.AddWithValue("@contacttypeid", ContactType);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }

        }

        public void saveContacts(CustomerViewModel c)
        {

            // save main contact
            // c.MainContact.CustomerId = c.Id;
            //  c.MainContact.TypeId = ContactViewModel.ContactType_Main;
            //c.MainContact.Save(CreatedBy);
            //   SaveContact(c.MainContact);

            //if the billing contact is the same as the main contact
            // delete the existing billing contact, ELSE add the second contact (billing contact)
            //   if (c.BillingContactSaveAsMain == Constants.Yes)
            //    {
            //        deleteContactType(c.Id, ContactViewModel.ContactType_Billing);
            //}
            //else
            //{

            //    c.BillingContact.CustomerId = c.Id;
            //    c.BillingContact.TypeId = ContactViewModel.ContactType_Billing;
            //    SaveContact(c.BillingContact);
            //    //c.BillingContact.Save(CreatedBy);
            //}

        }

        public int disableParentOption(CustomerViewModel custVm)
        {
            // int result = custVm.Parent;
            int result = 1;
            if (result == Constants.Yes)
            {
                result = this.HasChildren(1);
            }
            else
            {
                result = Constants.Yes;
                if (Constants.Empty == 1)
                {
                    result = Constants.No;
                }
            }
            return result;
        }
        public int deleteNote(int noteId, int userId)
        {
            string spName = "spDelete_C_Note";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", noteId);
                cmd.Parameters.AddWithValue("@userid", userId);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }

        }
        public int deleteEmailAddress(int emailaddressid, int userId)
        {
            string spName = "spDelete_C_EmailAddress";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", emailaddressid);
                cmd.Parameters.AddWithValue("@userid", userId);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }



        }
        public int DeleteOfficeContact(int CustomerId, int UserId)
        {
            string spName = "spDelete_C_OfficeContact";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@customerid", CustomerId);
                cmd.Parameters.AddWithValue("@userid", UserId);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }

        }
        public BillingViewModel LoadBillSettings(int CustomerId)
        {
            BillingViewModel bvm = null;

            var data = jkEntityModel.vw_C_BillSettings
                        .Where(b => b.customerId == CustomerId).FirstOrDefault();

            if (data != null)
            {
                bvm = new BillingViewModel();
                bvm.Id = data.id;
                //bvm.ARStatus = data.arstatus;
                //bvm.ARStatusName = bvm.ARStatusName;
                bvm.AllowCPIIncreases = bvm.AllowCPIIncreases;
                bvm.ARStatus = data.arstatus;
                bvm.ARStatusName = data.arstatusname;
                bvm.Term = (int)data.term;
                bvm.EBill = data.ebill;
                bvm.ConsolidatedInvoice = (int)data.consolidatedinvoice;
                bvm.CreateInvoice = data.createinvoice;
                bvm.PrintInvoice = (int)data.printinvoice;
                bvm.PrintPastDue = (int)data.printpastdue;
                bvm.EffectiveDate = (DateTime)data.effectivedate;
                bvm.TaxExempt = data.taxexempt;
                bvm.TaxAuthority = (int)data.taxauthorityid;
                bvm.TaxAuthorityName = data.taxauthorityname;
                bvm.InvoiceMessage = data.invoicemessage;
                bvm.InvoiceDate = (int)data.invoicedate;
                bvm.CreatedBy = data.createdby;
                bvm.CreateDate = data.createdate;
                bvm.ModifiedBy = data.modifiedby;
                bvm.ModifiedDate = data.modifieddate;
            }

            return bvm;
        }

        public BillSetting GetBillSettingWithCustomer(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var qry = context.BillSettings.OrderByDescending(o => o.BillSettingsId).Where(w => (w.IsActive == null || w.IsActive == 1) && w.CustomerId == CustomerId).FirstOrDefault();
                return qry;
            }
        }

        #endregion

        #region ServiceCalls


        public IQueryable<JKApi.Data.DAL.Customer> GetCustomers()
        {
            var qry = Uow.Customer.GetAll();
            return qry;
        }

        public List<JKApi.Data.DAL.Customer> GetCustomersList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var qry = context.Customers.ToList();
                return qry;
            }
        }

        public List<SearchDateList> GetAll_OptionList()
        {
            List<SearchDateList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.SearchDateLists.ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        public List<FranchiseeTypeList> GetAll_FranchiseeTypeList()
        {
            List<FranchiseeTypeList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.FranchiseeTypeLists.ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        //lakhwinder bind list start//
        public List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogCustomersList(int CustID)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.CollectionsCallLogModel> CollectionCallLog = (from
                                                tbCollactionlog in context.CollectionsCallLogs
                                                                                         join tbStatus in context.StatusResultLists on tbCollactionlog.StatusResultListId equals tbStatus.StatusResultListId
                                                                                         join tbcust in context.Customers on tbCollactionlog.ClassId equals tbcust.CustomerId
                                                                                         select new CollectionsCallLogModel
                                                                                         {
                                                                                             CustomerNo = tbcust.CustomerNo,
                                                                                             CallDate = tbCollactionlog.CallDate,
                                                                                             CallTime = tbCollactionlog.CallTime,
                                                                                             status = tbStatus.Name,
                                                                                             SpokeWith = tbCollactionlog.SpokeWith,
                                                                                             Action = tbCollactionlog.Action,
                                                                                             CallBack = tbCollactionlog.CallBack,
                                                                                             Comments = tbCollactionlog.Comments,
                                                                                             ClassId = tbCollactionlog.ClassId
                                                                                         }).Where(x => x.ClassId == CustID).ToList();
                return CollectionCallLog;
            }
        }
        public List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.CollectionsCallLogModel> CollectionCallLog = (from
                                                tbCollactionlog in context.CollectionsCallLogs
                                                                                         join tbStatus in context.StatusResultLists on tbCollactionlog.StatusResultListId equals tbStatus.StatusResultListId
                                                                                         join tbcust in context.Customers on tbCollactionlog.ClassId equals tbcust.CustomerId
                                                                                         select new CollectionsCallLogModel
                                                                                         {
                                                                                             CustomerNo = tbcust.CustomerNo,
                                                                                             CallDate = tbCollactionlog.CallDate,
                                                                                             CallTime = tbCollactionlog.CallTime,
                                                                                             status = tbStatus.Name,
                                                                                             SpokeWith = tbCollactionlog.SpokeWith,
                                                                                             Action = tbCollactionlog.Action,
                                                                                             CallBack = tbCollactionlog.CallBack,
                                                                                             Comments = tbCollactionlog.Comments,
                                                                                             ClassId = tbCollactionlog.ClassId
                                                                                         }).ToList();
                return CollectionCallLog;
            }
        }
        public List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogParticularCustomersList(int CustID)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLog = (from tbcust in context.Customers
                                                                                  join
                                                tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                                                                  join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId
                                                                                  select new ServiceCallLogModel
                                                                                  {
                                                                                      CustomerNo = tbcust.CustomerNo,
                                                                                      CallDate = tbservicecalllog.CallDate,
                                                                                      CallTime = tbservicecalllog.CallTime.ToString(),
                                                                                      Status = tbStatus.Name,
                                                                                      SpokeWith = tbservicecalllog.SpokeWith,
                                                                                      Action = tbservicecalllog.Action,
                                                                                      CallBack = tbservicecalllog.CallBack,
                                                                                      Comments = tbservicecalllog.Comments,
                                                                                      ClassId = tbservicecalllog.ClassId
                                                                                  }).Where(x => x.ClassId == CustID).ToList();
                return ServiceCallLog;
            }
        }
        public List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogLists()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLogList = (from tbcust in context.Customers
                                                                                      join
                                                    tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                                                                      join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId
                                                                                      select new ServiceCallLogModel
                                                                                      {
                                                                                          CustomerNo = tbcust.CustomerNo,
                                                                                          CallDate = tbservicecalllog.CallDate,
                                                                                          CallTime = tbservicecalllog.CallTime.ToString(),
                                                                                          Status = tbStatus.Name,
                                                                                          SpokeWith = tbservicecalllog.SpokeWith,
                                                                                          Action = tbservicecalllog.Action,
                                                                                          CallBack = tbservicecalllog.CallBack,
                                                                                          Comments = tbservicecalllog.Comments,
                                                                                          ClassId = tbcust.CustomerId

                                                                                      }).ToList();
                return ServiceCallLogList;
            }

        }
        public List<JKViewModels.Customer.CollectionsCallLogList> GetCustomerInvoice(int CustID)
        {
            //List<JKViewModels.Customer.CollectionsCallLogModel> GetCustomerInvoiceList = new List<CollectionsCallLogModel>();
            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    GetCustomerInvoiceList = (from tbcust in context.Customers
            //                              join tbinvoice in context.Invoices on tbcust.CustomerId equals tbinvoice.ClassId
            //                              select new CollectionsCallLogModel
            //                              {
            //                                  InvoiceNo = tbinvoice.InvoiceNo,
            //                                  InvoiceDate = tbinvoice.InvoiceDate ?? DateTime.Now,
            //                                  CustomerName = tbcust.Name,
            //                                  CustomerNo = tbcust.CustomerNo,
            //                                  InvoiceDescription = tbinvoice.InvoiceDescription,
            //                                  ClassId = tbcust.CustomerId
            //                              }).Where(x => x.ClassId == CustID).ToList();
            //}

            //return GetCustomerInvoiceList;


            List<CollectionsCallLogList> lstCollectionsCallLog = new List<CollectionsCallLogList>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustID);
                parmas.Add("@RegionId", SelectedRegionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_GetInvoiceListWithCustomerId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCollectionsCallLog = multipleresult.Read<CollectionsCallLogList>().ToList();
                    }
                }
            }
            return lstCollectionsCallLog;
        }

        public List<CreditReasonList> GetAll_ReasonList()
        {
            List<CreditReasonList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.CreditReasonLists.ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        public List<ARInvoiceListViewModel> GetInvoiceListWithSearchForCredit(int _customerId)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                List<portal_spGet_AR_InvoiceSearchListForCreditTransaction_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoiceSearchListForCreditTransaction(_customerId).ToList();

                foreach (portal_spGet_AR_InvoiceSearchListForCreditTransaction_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        //lakhwinder bind list end//

        public string getCustomerNo(int RegionId = 0)
        {
            //var regionno = Uow.RegionConfiguration.GetAll().Where(x => x.RegionConfigurationId == 34).Select(x => new { x.value }).FirstOrDefault();
            //var customerindex = Uow.RegionConfiguration.GetAll().Where(x => x.RegionConfigurationId == 3).Select(x => new { x.value }).FirstOrDefault();
            //string customerno = regionno.value + "" + customerindex.value;
            //return customerno;

            RegionId = RegionId == 0 ? SelectedRegionId : Convert.ToInt32(RegionId);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var regionno = context.RegionSettings.Where(x => x.RegionConfigurationId == 34 && x.RegionId == RegionId).Select(x => new { x.Value }).FirstOrDefault();
                var customerindex = context.RegionSettings.Where(x => x.RegionConfigurationId == 3 && x.RegionId == RegionId).Select(x => new { x.Value }).FirstOrDefault();
                string customerno = regionno?.Value + "" + customerindex?.Value;
                return customerno;
            }
        }


        public JKApi.Data.DAL.Customer GetCustomerById(int id)
        {
            return Uow.Customer.GetById(id);
        }

        public RegionSetting GetRegionConfigurationbyId(int id, int RegionId)
        {
            //return Uow.RegionConfiguration.GetById(id);
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.RegionSettings.Where(x => x.RegionConfigurationId == id && x.RegionId == RegionId).FirstOrDefault();
            }
        }

        public RegionSetting SaveRegionConfiguration(RegionSetting RegionSetting)
        {
            var ID = RegionSetting.RegionConfigurationId;
            var isNew = ID == 0;
            ////add new entry
            //if (isNew)
            //{
            //    Uow.RegionConfiguration.Add(RegionConfiguration);
            //    Uow.Commit();
            //}
            //else //update existing entry
            //{
            //    Uow.RegionConfiguration.Update(RegionConfiguration);
            //    Uow.Commit();
            //}
            //return RegionConfiguration;

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                if (RegionSetting != null && RegionSetting.RegionSettingId > 0)
                    context.Entry(RegionSetting).State = EntityState.Modified;

                context.SaveChanges();

                return RegionSetting;
            }
        }

        public JKApi.Data.DAL.Customer SaveCustomers(JKApi.Data.DAL.Customer Customer)
        {
            var ID = Customer.CustomerId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                //Customer.RegionId = SelectedRegionId;

                Uow.Customer.Add(Customer);
                Uow.Commit();

                Status oStatus = new Status();

                oStatus.ClassId = Customer.CustomerId;
                oStatus.CreatedBy = Customer.CreatedBy > 0 ? Customer.CreatedBy.ToString() : LoginUserId.ToString();
                oStatus.CreatedDate = DateTime.Now;
                oStatus.IsActive = true;
                oStatus.StatusDate = DateTime.Now;
                oStatus.TypeListId = 1;
                //oStatus.StatusListId = Customer.StatusListId != null ? Customer.StatusListId : 4;     //Check if It doesn't create by CRM           
                oStatus.StatusListId = Customer.StatusListId != null ? Customer.StatusListId : 38;     //Check if It doesn't create by CRM 
                Uow.Status.Add(oStatus);
                Uow.Commit();

                Customer.StatusId = oStatus.StatusId;
                Customer.StatusListId = oStatus.StatusListId;
                Customer.sys_cust = Customer.sys_cust != null ? Customer.sys_cust : Customer.CustomerId;
                Uow.Customer.Update(Customer);
                Uow.Commit();


            }
            else //update existing entry
            {

                Uow.Customer.Update(Customer);
                Uow.Commit();
            }

            return Customer;
        }



        public Email SaveEBill_Emails(Email _Email)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Email oEmail = context.Emails.Where(e => e.EmailAddress == _Email.EmailAddress && e.ContactTypeListId == _Email.ContactTypeListId && e.ClassId == _Email.ClassId && _Email.TypeListId == e.TypeListId).ToList().FirstOrDefault();

                if (oEmail != null)
                {
                    var entity = Uow.Email.GetById(oEmail.EmailId);
                    Uow.Email.Update(entity);
                    Uow.Commit();
                }
                else
                {
                    Uow.Email.Add(_Email);
                    Uow.Commit();
                }
            }



            return _Email;
        }

        public void DeleteCustomer(int id)
        {

            var entity = Uow.Customer.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.Customer.Update(entity);
            Uow.Commit();
        }

        public bool CheckCustomerIsExist(string Name)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Customers.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public int CheckCustomerNamePhoneIsExist(string Name, string Phone, string APISelectedRegionId = "")
        {
            int regionId = (APISelectedRegionId != "" ? Convert.ToInt32(APISelectedRegionId) : SelectedRegionId);
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Customers.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == regionId));
            if (data != null && data.Count() > 0)
            {
                return -1;
            }
            else
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return -2;
                }
                else
                {
                    /*var CRM_AccountCustomer = context.CRM_AccountCustomerDetail.Where(w => w.CompanyName.ToLower().Trim() == Name.ToLower().Trim());
                    if (CRM_AccountCustomer != null && CRM_AccountCustomer.Count() > 0)
                    {
                        return CRM_AccountCustomer.FirstOrDefault().CRM_AccountCustomerDetailId;
                    }
                    else {
                        var CRM_AccountCusPhone = context.CRM_AccountCustomerDetail.Where(w => w.CompanyPhoneNumber.ToLower().Trim() == Phone.Trim());
                        if (CRM_AccountCusPhone != null && CRM_AccountCusPhone.Count() > 0)
                        {
                            return CRM_AccountCusPhone.FirstOrDefault().CRM_AccountCustomerDetailId;
                        }
                    }*/
                    var CRM_AccountCustomer = context.CRM_AccountCustomerDetail.Where(w => w.CompanyName.ToLower().Trim() == Name.ToLower().Trim() && w.CompanyPhoneNumber.ToLower().Trim() == Phone.Trim());
                    if (CRM_AccountCustomer != null && CRM_AccountCustomer.Count() > 0)
                    {
                        return Convert.ToInt32(CRM_AccountCustomer.FirstOrDefault().CRM_AccountId);
                    }
                    else
                    {
                        var CRM_AccCustomer = context.CRM_AccountCustomerDetail.Where(w => w.CompanyName.ToLower().Trim() == Name.ToLower().Trim());
                        if (CRM_AccCustomer != null && CRM_AccCustomer.Count() > 0)
                        {
                            return -3;
                        }
                        else
                        {
                            var CRM_AccountCusPhone = context.CRM_AccountCustomerDetail.Where(w => w.CompanyPhoneNumber.ToLower().Trim() == Phone.Trim());
                            if (CRM_AccountCusPhone != null && CRM_AccountCusPhone.Count() > 0)
                            {
                                return -4;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        public int CheckOnlyCustomerNamePhoneIsExist(string Name, string Phone)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Customers.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return data.FirstOrDefault().CustomerId;
                }
                else
                {
                    return data.FirstOrDefault().CustomerId;
                }
            }
            else
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return -3;
                }
                return -1;
            }

            //jkDatabaseEntities context = new jkDatabaseEntities();
            //var data = context.Customers.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            //if (data != null && data.Count() > 0)
            //{
            //    var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
            //    if (PhoneModel != null && PhoneModel.Count() > 0)
            //    {
            //        return data.FirstOrDefault().CustomerId;
            //    }
            //    else
            //    {
            //        return -2;
            //    }
            //}
            //else
            //{
            //    var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
            //    if (PhoneModel != null && PhoneModel.Count() > 0)
            //    {
            //        return -3;
            //    }
            //    return -1;
            //}
        }

        public int CheckFranchiseeNamePhoneIsExist(string Name, string Phone)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Franchisees.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                return -1;
            }
            else
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return -2;
                }
                else
                {
                    var CRM_AccountCustomer = context.CRM_AccountFranchiseDetail.Where(w => w.FranchiseeName.ToLower().Trim() == Name.ToLower().Trim());
                    if (CRM_AccountCustomer != null && CRM_AccountCustomer.Count() > 0)
                    {
                        return (CRM_AccountCustomer.FirstOrDefault().CRM_AccountId.HasValue ? CRM_AccountCustomer.FirstOrDefault().CRM_AccountId.Value : 0);
                    }
                    else
                    {
                        var CRM_AccountCusPhone = context.CRM_AccountFranchiseDetail.Where(w => w.CellNumber.ToLower().Trim() == Phone.Trim());
                        if (CRM_AccountCusPhone != null && CRM_AccountCusPhone.Count() > 0)
                        {
                            return (CRM_AccountCusPhone.FirstOrDefault().CRM_AccountId.HasValue ? CRM_AccountCusPhone.FirstOrDefault().CRM_AccountId.Value : 0);
                        }
                    }
                }
            }
            return 0;
        }

        public IQueryable<Phone> GetPhone()
        {
            var qry = Uow.Phone.GetAll();
            return qry;
        }

        public IQueryable<Phone_Temp> GetPhoneTemp()
        {
            var qry = Uow.Phone_Temp.GetAll();
            return qry;
        }

        public Phone GetPhoneById(int id)
        {
            return Uow.Phone.GetById(id);
        }

        public Phone SavePhone(Phone Phone)
        {
            var ID = Phone.PhoneId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Phone.Add(Phone);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Phone.Update(Phone);
                Uow.Commit();
            }

            return Phone;
        }

        public Phone_Temp SavePhone_Temp(Phone_Temp Phone_Temp)
        {
            var ID = Phone_Temp.PhoneId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Phone_Temp.Add(Phone_Temp);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Phone_Temp.Update(Phone_Temp);
                Uow.Commit();
            }

            return Phone_Temp;
        }
        public void DeletePhone(int id)
        {
            var entity = Uow.Phone.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.Phone.Update(entity);
            Uow.Commit();
        }

        public Phone AddNewPhoneOldInactive(Phone Phone)
        {
            if (Phone.PhoneId > 0)
            {
                var entity = Uow.Phone.GetById(Phone.PhoneId);
                entity.IsActive = false;
                Uow.Phone.Update(entity);
                Uow.Commit();
            }
            Uow.Phone.Add(Phone);
            Uow.Commit();
            return Phone;
        }
        public Phone_Temp AddNewPhoneOldInactiveTemp(Phone_Temp Phone)
        {
            if (Phone.PhoneId > 0)
            {
                var entity = Uow.Phone_Temp.GetById(Phone.PhoneId);
                entity.IsActive = false;
                Uow.Phone_Temp.Update(entity);
                Uow.Commit();
            }
            Uow.Phone_Temp.Add(Phone);
            Uow.Commit();
            return Phone;
        }


        public IQueryable<Address> GetAddress()
        {
            var qry = Uow.Address.GetAll();
            return qry;
        }
        public IQueryable<Address_Temp> GetAddressTemp()
        {
            var qry = Uow.Address_Temp.GetAll();
            return qry;
        }

        public Address GetAddressById(int id)
        {
            return Uow.Address.GetById(id);
        }

        public Address SaveAddress(Address Address)
        {
            var ID = Address.AddressId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Address.Add(Address);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Address.Update(Address);
                Uow.Commit();
            }

            return Address;
        }
        public Address_Temp SaveAddress_Temp(Address_Temp Address_Temp)
        {
            var ID = Address_Temp.AddressId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Address_Temp.Add(Address_Temp);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Address_Temp.Update(Address_Temp);
                Uow.Commit();
            }

            return Address_Temp;
        }

        public void DeleteAddress(int id)
        {
            var entity = Uow.Address.GetById(id);


            // Need a Column for soft Delete 
            Uow.Address.Update(entity);
            Uow.Commit();
        }

        public Address AddNewAddressOldInactive(Address Address)
        {
            if (Address.AddressId > 0)
            {
                var entity = Uow.Address.GetById(Address.AddressId);
                entity.IsActive = false;
                Uow.Address.Update(entity);
                Uow.Commit();
            }
            Uow.Address.Add(Address);
            Uow.Commit();
            return Address;
        }
        public Address_Temp AddNewAddressOldInactiveTemp(Address_Temp Address)
        {
            if (Address.AddressId > 0)
            {
                var entity = Uow.Address_Temp.GetById(Address.AddressId);
                entity.IsActive = false;
                Uow.Address_Temp.Update(entity);
                Uow.Commit();
            }
            Uow.Address_Temp.Add(Address);
            Uow.Commit();
            return Address;
        }

        public IQueryable<CountryCodeList> GetCountryCodeList()
        {
            var qry = Uow.CountryCodeList.GetAll();
            return qry;
        }

        public CountryCodeList GetCountryCodeListById(int id)
        {
            return Uow.CountryCodeList.GetById(id);
        }

        public CountryCodeList SaveCountryCodeList(CountryCodeList CountryCodeList)
        {
            var ID = CountryCodeList.CountryCodeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.CountryCodeList.Add(CountryCodeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.CountryCodeList.Update(CountryCodeList);
                Uow.Commit();
            }

            return CountryCodeList;
        }

        public CountryCodeList DeleteCountryCodeList(int id)
        {
            var entity = Uow.CountryCodeList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.CountryCodeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<TypeList> GetTypeList()
        {
            var qry = Uow.TypeList.GetAll();
            return qry;
        }

        public TypeList GetTypeListById(int id)
        {
            return Uow.TypeList.GetById(id);
        }

        public TypeList SaveTypeList(TypeList TypeList)
        {
            var ID = TypeList.TypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.TypeList.Add(TypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.TypeList.Update(TypeList);
                Uow.Commit();
            }

            return TypeList;
        }

        public TypeList DeleteTypeList(int id)
        {
            var entity = Uow.TypeList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.TypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<ContactTypeList> GetContactTypeList()
        {
            var qry = Uow.ContactTypeList.GetAll();
            return qry;
        }

        public ContactTypeList GetContactTypeListById(int id)
        {
            return Uow.ContactTypeList.GetById(id);
        }

        public ContactTypeList SaveContactTypeList(ContactTypeList ContactTypeList)
        {
            var ID = ContactTypeList.ContactTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ContactTypeList.Add(ContactTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ContactTypeList.Update(ContactTypeList);
                Uow.Commit();
            }

            return ContactTypeList;
        }

        public ContactTypeList DeleteContactTypeList(int id)
        {
            var entity = Uow.ContactTypeList.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.ContactTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<StateList> GetStateList()
        {
            var qry = Uow.StateList.GetAll();
            return qry;
        }



        public StateList GetStateListById(int id)
        {
            return Uow.StateList.GetById(id);
        }

        public StateList SaveStateList(StateList StateList)
        {
            var ID = StateList.StateListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.StateList.Add(StateList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.StateList.Update(StateList);
                Uow.Commit();
            }

            return StateList;
        }

        public StateList DeleteStateList(int id)
        {
            var entity = Uow.StateList.GetById(id);


            // Need a Column for soft Delete 
            Uow.StateList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<Status> GetStatus()
        {
            var qry = Uow.Status.GetAll();
            return qry;
        }

        public Status GetStatusByClassId(int ClassId, int StatusListId)
        {
            return Uow.Status.GetAll().Where(x => x.ClassId == ClassId && x.StatusListId == StatusListId).FirstOrDefault();
        }

        public Status GetStatusById(int id)
        {
            return Uow.Status.GetById(id);
        }

        public Status SaveStatus(Status Status)
        {
            var ID = Status.StatusId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Status.Add(Status);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Status.Update(Status);
                Uow.Commit();
            }

            return Status;
        }

        public Status DeleteStatus(int id)
        {
            var entity = Uow.Status.GetById(id);


            // Need a Column for soft Delete 
            Uow.Status.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<Email> GetEmail()
        {
            var qry = Uow.Email.GetAll();
            return qry;
        }
        public IQueryable<Email_Temp> GetEmailTemp()
        {
            var qry = Uow.Email_Temp.GetAll();
            return qry;
        }

        public Email GetEmailById(int id)
        {
            return Uow.Email.GetById(id);
        }

        public Email SaveEmail(Email Email)
        {
            var ID = Email.EmailId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Email.Add(Email);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Email.Update(Email);
                Uow.Commit();
            }

            return Email;
        }
        public Email_Temp SaveEmail_Temp(Email_Temp Email_Temp)
        {
            var ID = Email_Temp.EmailId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Email_Temp.Add(Email_Temp);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Email_Temp.Update(Email_Temp);
                Uow.Commit();
            }

            return Email_Temp;
        }

        public Email DeleteEmail(int id)
        {
            var entity = Uow.Email.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.Email.Update(entity);
            Uow.Commit();
            return entity;
        }

        public Email AddNewEmailOldInactive(Email Email)
        {
            if (Email.EmailId > 0)
            {
                var entity = Uow.Email.GetById(Email.EmailId);
                entity.IsActive = false;
                Uow.Email.Update(entity);
                Uow.Commit();
            }
            Uow.Email.Add(Email);
            Uow.Commit();
            return Email;
        }
        public Email_Temp AddNewEmailOldInactiveTemp(Email_Temp Email)
        {
            if (Email.EmailId > 0)
            {
                var entity = Uow.Email_Temp.GetById(Email.EmailId);
                entity.IsActive = false;
                Uow.Email_Temp.Update(entity);
                Uow.Commit();
            }
            Uow.Email_Temp.Add(Email);
            Uow.Commit();
            return Email;
        }

        public IQueryable<StatusList> GetStatusList()
        {
            var qry = Uow.StatusList.GetAll();
            return qry;
        }


        public List<TransactionStatusList> GetTrasactionStatusList()
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.TransactionStatusLists.ToList();
            }
        }

        public StatusList GetStatusListById(int id)
        {
            return Uow.StatusList.GetById(id);
        }

        public StatusList SaveStatusList(StatusList StatusList)
        {
            var ID = StatusList.StatusListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.StatusList.Add(StatusList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.StatusList.Update(StatusList);
                Uow.Commit();
            }

            return StatusList;
        }

        public StatusList DeleteStatusList(int id)
        {
            var entity = Uow.StatusList.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.StatusList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<Contact> GetContact()
        {
            var qry = Uow.Contact.GetAll();
            return qry;
        }
        public IQueryable<Contact_Temp> GetContactTemp()
        {
            var qry = Uow.Contact_Temp.GetAll();
            return qry;
        }

        public Contact GetContactById(int id)
        {
            return Uow.Contact.GetById(id);
        }

        public Contact SaveContact(Contact Contact)
        {
            var ID = Contact.ContactId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Contact.Add(Contact);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Contact.Update(Contact);
                Uow.Commit();
            }

            return Contact;
        }
        public Contact_Temp SaveContact_Temp(Contact_Temp Contact_Temp)
        {
            var ID = Contact_Temp.ContactId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Contact_Temp.Add(Contact_Temp);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Contact_Temp.Update(Contact_Temp);
                Uow.Commit();
            }

            return Contact_Temp;
        }

        public Contact DeleteContact(int id)
        {
            var entity = Uow.Contact.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.Contact.Update(entity);
            Uow.Commit();
            return entity;
        }

        public Contact AddNewContactOldInactive(Contact Contact)
        {
            if (Contact.ContactId > 0)
            {
                var entity = Uow.Contact.GetById(Contact.ContactId);
                entity.IsActive = false;
                Uow.Contact.Update(entity);
                Uow.Commit();
            }
            Uow.Contact.Add(Contact);
            Uow.Commit();
            return Contact;
        }
        public Contact_Temp AddNewContactOldInactiveTemp(Contact_Temp Contact)
        {
            if (Contact.ContactId > 0)
            {
                var entity = Uow.Contact_Temp.GetById(Contact.ContactId);
                entity.IsActive = false;
                Uow.Contact_Temp.Update(entity);
                Uow.Commit();
            }
            Uow.Contact_Temp.Add(Contact);
            Uow.Commit();
            return Contact;
        }

        IQueryable<StatusReasonList> ICustomerService.GetStatusReasonList()
        {
            var qry = Uow.StatusReasonList.GetAll();
            return qry;
        }

        public StatusReasonList GetStatusReasonListById(int id)
        {
            return Uow.StatusReasonList.GetById(id);
        }

        public StatusReasonList SaveStatusReasonList(StatusReasonList StatusReasonList)
        {
            var ID = StatusReasonList.StatusReasonListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.StatusReasonList.Add(StatusReasonList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.StatusReasonList.Update(StatusReasonList);
                Uow.Commit();
            }

            return StatusReasonList;
        }

        public StatusReasonList DeleteStatusReasonList(int id)
        {
            var entity = Uow.StatusReasonList.GetById(id);


            // Need a Column for soft Delete 

            Uow.StatusReasonList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<BillSetting> GetBillSetting()
        {
            var qry = Uow.BillSetting.GetAll();
            return qry;
        }

        public BillSetting GetBillSettingById(int id)
        {
            return Uow.BillSetting.GetById(id);
        }

        public BillSetting SaveBillSetting(BillSetting BillSetting)
        {
            var ID = BillSetting.BillSettingsId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.BillSetting.Add(BillSetting);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.BillSetting.Update(BillSetting);
                Uow.Commit();
            }

            return BillSetting;
        }

        public BillSetting DeleteBillSetting(int id)
        {
            var entity = Uow.BillSetting.GetById(id);


            // Need a Column for soft Delete 

            Uow.BillSetting.Update(entity);
            Uow.Commit();
            return entity;
        }
        public BillSetting AddNewBillSettingOldInactive(BillSetting BillSetting)
        {
            if (BillSetting.BillSettingsId > 0)
            {
                var entity = Uow.BillSetting.GetById(BillSetting.BillSettingsId);
                entity.IsActive = 0;
                Uow.BillSetting.Update(entity);
                Uow.Commit();
            }
            //BillSetting.CreatedBy = -1;
            Uow.BillSetting.Add(BillSetting);
            Uow.Commit();
            return BillSetting;


        }

        public IQueryable<CountyTaxAuthorityList> GetCountyTaxAuthorityList()
        {
            var qry = Uow.CountyTaxAuthorityList.GetAll();
            return qry;
        }

        public CountyTaxAuthorityList GetCountyTaxAuthorityListById(int id)
        {
            return Uow.CountyTaxAuthorityList.GetById(id);
        }

        public CountyTaxAuthorityList SaveCountyTaxAuthorityList(CountyTaxAuthorityList CountyTaxAuthorityList)
        {
            var ID = CountyTaxAuthorityList.CountyTaxAuthorityListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.CountyTaxAuthorityList.Add(CountyTaxAuthorityList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.CountyTaxAuthorityList.Update(CountyTaxAuthorityList);
                Uow.Commit();
            }

            return CountyTaxAuthorityList;
        }

        public CountyTaxAuthorityList DeleteCountyTaxAuthorityList(int id)
        {
            var entity = Uow.CountyTaxAuthorityList.GetById(id);


            // Need a Column for soft Delete 

            Uow.CountyTaxAuthorityList.Update(entity);
            Uow.Commit();
            return entity;
        }



        public IQueryable<PayTypeList> GetPayTypeList()
        {
            var qry = Uow.PayTypeList.GetAll();
            return qry;
        }

        public PayTypeList GetPayTypeListById(int id)
        {
            return Uow.PayTypeList.GetById(id);
        }

        public PayTypeList SavePayTypeList(PayTypeList PayTypeList)
        {
            var ID = PayTypeList.PayTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.PayTypeList.Add(PayTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.PayTypeList.Update(PayTypeList);
                Uow.Commit();
            }

            return PayTypeList;
        }

        public PayTypeList DeletePayTypeList(int id)
        {
            var entity = Uow.PayTypeList.GetById(id);


            // Need a Column for soft Delete 

            Uow.PayTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }


        public IQueryable<ARStatu> GetARStatu()
        {
            var qry = Uow.ARStatu.GetAll();
            return qry;
        }

        public ARStatu GetARStatuById(int id)
        {
            return Uow.ARStatu.GetById(id);
        }

        public ARStatu SaveARStatu(ARStatu ARStatu)
        {
            var ID = ARStatu.ARStatusId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ARStatu.Add(ARStatu);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ARStatu.Update(ARStatu);
                Uow.Commit();
            }

            return ARStatu;
        }

        public ARStatu DeleteARStatu(int id)
        {
            var entity = Uow.ARStatu.GetById(id);


            // Need a Column for soft Delete 

            Uow.ARStatu.Update(entity);
            Uow.Commit();
            return entity;
        }

        public ARStatu AddNewARStatuOldInactive(ARStatu ARStatu)
        {
            var data = Uow.ARStatu.GetAll().Where(w => w.BillSettingsId == ARStatu.BillSettingsId);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    var ItemRecord = Uow.ARStatu.GetById(item.ARStatusId);
                    if (ItemRecord != null)
                    {
                        var entity = ItemRecord;
                        entity.isActive = 0;
                        Uow.ARStatu.Update(entity);
                        Uow.Commit();
                    }
                }
            }
            Uow.ARStatu.Add(ARStatu);
            Uow.Commit();
            return ARStatu;
        }

        public IQueryable<ARStatusReasonList> GetARStatusReasonList()
        {
            var qry = Uow.ARStatusReasonList.GetAll();
            return qry;
        }

        //public List<Data.DAL.Region> GetRegionList()
        //{
        //    using (var context = new jkDatabaseEntities())
        //    {
        //        context.Configuration.ProxyCreationEnabled = false;
        //        var qry = context.Regions.Where(x => (SelectedRegionId == 0 || x.RegionId == SelectedRegionId)).ToList();
        //        context.Configuration.ProxyCreationEnabled = true;
        //        return qry;
        //    }
        //}

        public ARStatusReasonList GetARStatusReasonListById(int id)
        {
            return Uow.ARStatusReasonList.GetById(id);
        }

        public ARStatusReasonList SaveARStatusReasonList(ARStatusReasonList ARStatusReasonList)
        {
            var ID = ARStatusReasonList.ARStatusReasonListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ARStatusReasonList.Add(ARStatusReasonList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ARStatusReasonList.Update(ARStatusReasonList);
                Uow.Commit();
            }

            return ARStatusReasonList;
        }

        public ARStatusReasonList DeleteARStatusReasonList(int id)
        {
            var entity = Uow.ARStatusReasonList.GetById(id);


            // Need a Column for soft Delete 

            Uow.ARStatusReasonList.Update(entity);
            Uow.Commit();
            return entity;
        }


        public IQueryable<Data.DAL.Contract> GetContract()
        {
            var qry = Uow.Contract.GetAll();
            return qry;
        }

        public IQueryable<Data.DAL.Distribution> GetDistribution()
        {
            var qry = Uow.Distribution.GetAll();
            return qry;

        }

        public IQueryable<Data.DAL.FindersFee> GetFinderFee()
        {
            var qry = Uow.FinderFee.GetAll();
            return qry;

        }


        public Data.DAL.Contract GetContractById(int id)
        {
            return Uow.Contract.GetById(id);
        }

        public Data.DAL.Contract SaveContract(Data.DAL.Contract Contract)
        {
            var ID = Contract.ContractId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Contract.Add(Contract);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Contract.Update(Contract);
                Uow.Commit();
            }

            return Contract;
        }

        public Data.DAL.Contract DeleteContract(int id)
        {
            var entity = Uow.Contract.GetById(id);


            // Need a Column for soft Delete 

            Uow.Contract.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<ContractStatusList> GetContractStatusList()
        {
            var qry = Uow.ContractStatusList.GetAll();
            return qry;
        }

        public ContractStatusList GetContractStatusListById(int id)
        {
            return Uow.ContractStatusList.GetById(id);
        }

        public Data.DAL.Contract GetContractByCustomerId(int CustomerId)
        {
            return Uow.Contract.GetAll().Where(w => w.CustomerId == CustomerId).FirstOrDefault();
        }

        public ContractStatusList SaveContractStatusList(ContractStatusList ContractStatusList)
        {
            var ID = ContractStatusList.ContractStatusListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ContractStatusList.Add(ContractStatusList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ContractStatusList.Update(ContractStatusList);
                Uow.Commit();
            }

            return ContractStatusList;
        }

        public ContractStatusList DeleteContractStatusList(int id)
        {
            var entity = Uow.ContractStatusList.GetById(id);


            // Need a Column for soft Delete 

            Uow.ContractStatusList.Update(entity);
            Uow.Commit();
            return entity;
        }


        public IQueryable<CustomerLog> GetCustomerLog()
        {
            var qry = Uow.CustomerLog.GetAll();
            return qry;
        }

        public CustomerLog GetCustomerLogById(int id)
        {
            return Uow.CustomerLog.GetById(id);
        }

        public CustomerLog SaveCustomerLog(CustomerLog CustomerLog)
        {
            var ID = CustomerLog.CustomerLogId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.CustomerLog.Add(CustomerLog);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.CustomerLog.Update(CustomerLog);
                Uow.Commit();
            }

            return CustomerLog;
        }

        public CustomerLog DeleteCustomerLog(int id)
        {
            var entity = Uow.CustomerLog.GetById(id);


            // Need a Column for soft Delete  
            Uow.CustomerLog.Update(entity);
            Uow.Commit();
            return entity;
        }

        public List<ContractDetailViewModel> GetContractDetail2()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<ContractDetailViewModel> lstContractDetailViewModel = new List<ContractDetailViewModel>();
                lstContractDetailViewModel.AddRange(
                (from Dtl in context.vw_ContractDetail
                 select new { Dtl }).Select(i => new ContractDetailViewModel
                 {
                     ContractId = i.Dtl.ContractId,
                     ServiceTypeListId = i.Dtl.ServiceTypeListId,
                     BillingFrequencyListId = i.Dtl.BillingFrequencyListId,
                     SquareFootage = i.Dtl.SquareFootage


                 }).ToList().OrderBy(c => c.ContractDetailId));

                return lstContractDetailViewModel;
            }
        }


        public IQueryable<ContractDetail> GetContractDetail()
        {
            var qry = Uow.ContractDetail.GetAll();
            return qry;
        }

        public List<FileTypeListViewModel> GetFileTypeList(int typelistid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<FileTypeListViewModel> lstFileTypeList = context.FileTypeLists.Where(f => f.TypeListId == typelistid).MapEnumerable<FileTypeListViewModel, FileTypeList>().ToList();

                return lstFileTypeList;
            }
        }

        //public List<ContractDetailViewModel> GetContractDetailResult1()
        //{
        //    using (jkDatabaseEntities context = new jkDatabaseEntities())
        //    {
        //        List<ContractDetailViewModel> lstContractDetailViewModel = new List<ContractDetailViewModel>();
        //        lstContractDetailViewModel.AddRange(
        //        (from Dtl in context.vw_ContractDetail
        //         select new { Dtl }).Select(i => new ContractDetailViewModel
        //         {
        //             ContractId = i.Dtl.ContractId,
        //             ContractDetailId = i.Dtl.ContractDetailId,
        //             ServiceTypeListId = i.Dtl.ServiceTypeListId,
        //             FrequencyListId = i.Dtl.FrequencyListId,
        //             LineNumber = i.Dtl.LineNumber,
        //             SquareFootage = i.Dtl.SquareFootage,
        //             CleanTimes = i.Dtl.CleanTimes,
        //             Mon = i.Dtl.Mon,
        //             Tues = i.Dtl.Tues,
        //             Wed = i.Dtl.Wed,
        //             Thur = i.Dtl.Thur,
        //             Fr = i.Dtl.Fr,
        //             CleanFrequency = i.Dtl.CleanFrequency,
        //             Amount = i.Dtl.Amount,
        //             //BPPAdmin = i.Dtl.BPPAdmin,
        //             //SeparateInvoice = i.Dtl.SeparateInvoice,
        //             //SubjectToFees = i.Dtl.SubjectToFees,
        //             //CPIIncrease = i.Dtl.CPIIncrease,
        //             //AccountRebate = i.Dtl.AccountRebate,
        //             CreatedBy = i.Dtl.CreatedBy,
        //             CreatedDate = i.Dtl.CreatedDate
        //         }).ToList().OrderBy(c => c.ContractDetailId));

        //        return lstContractDetailViewModel;
        //    }
        //}

        public IEnumerable<ContractDetail> GetContractDetailResult(FullCustomerViewModel model)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = (from a in context.ContractDetails.Include("ContractDetailsDescription")
                            select new
                            {
                                ContractId = a.Contract.ContractId,
                                ContractDetailId = a.ContractDetailId,
                                ServiceTypeListId = a.ServiceTypeListId,
                                BillingFrequencyListId = a.BillingFrequencyListId,
                                LineNumber = a.LineNumber,
                                SquareFootage = a.SquareFootage,
                                CleanTimes = a.CleanTimes,
                                Mon = a.Mon,
                                Tues = a.Tues,
                                Wed = a.Wed,
                                Thur = a.Thur,
                                Fr = a.Fri,
                                CleanFrequencyListId = a.CleanFrequencyListId,
                                Amount = a.Amount,
                                BPPAdmin = a.BPPAdmin,
                                //SeparateInvoice = a.SeparateInvoice,
                                //SubjectToFees = a.SubjectToFees,
                                CPIIncrease = a.CPIIncrease,
                                AccountRebate = a.AccountRebate,
                                CreatedBy = a.CreatedBy,
                                CreatedDate = a.CreatedDate,
                            }).AsEnumerable().Select(x => new ContractDetail
                            {
                                ContractId = x.ContractId,
                                ContractDetailId = x.ContractDetailId,
                                ServiceTypeListId = x.ServiceTypeListId,
                                BillingFrequencyListId = x.BillingFrequencyListId,
                                LineNumber = x.LineNumber,
                                SquareFootage = x.SquareFootage,
                                CleanTimes = x.CleanTimes,
                                Mon = x.Mon,
                                Tues = x.Tues,
                                Wed = x.Wed,
                                Thur = x.Thur,
                                Fri = x.Fr,
                                CleanFrequencyListId = x.CleanFrequencyListId,
                                Amount = x.Amount,
                                BPPAdmin = x.BPPAdmin,
                                //SeparateInvoice = x.SeparateInvoice,
                                //SubjectToFees = x.SubjectToFees,
                                CPIIncrease = x.CPIIncrease,
                                AccountRebate = x.AccountRebate,
                                CreatedBy = x.CreatedBy,
                                CreatedDate = x.CreatedDate
                            }).ToList();


                model._ContractDetail = data.Where(one => one.ContractDetailId == model.ContractDetail.ContractDetailId).MapEnumerable<ContractDetailViewModel, ContractDetail>();
                return data;
            }
        }

        public IEnumerable<ContractDetail> GetContractDetailResultModel(FullCustomerViewModel model, int Id)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = (from a in context.ContractDetails.Include("ContractDetailsDescription")
                            select new
                            {
                                ContractId = a.Contract.ContractId,
                                ContractDetailId = a.ContractDetailId,
                                ServiceTypeListId = a.ServiceTypeListId,
                                BillingFrequencyListId = a.BillingFrequencyListId,
                                LineNumber = a.LineNumber,
                                SquareFootage = a.SquareFootage,
                                CleanTimes = a.CleanTimes,
                                Mon = a.Mon,
                                Tues = a.Tues,
                                Wed = a.Wed,
                                Thur = a.Thur,
                                Fr = a.Fri,
                                CleanFrequencyListId = a.CleanFrequencyListId,
                                Amount = a.Amount,
                                BPPAdmin = a.BPPAdmin,
                                //SeparateInvoice = a.SeparateInvoice,
                                //SubjectToFees = a.SubjectToFees,
                                CPIIncrease = a.CPIIncrease,
                                AccountRebate = a.AccountRebate,
                                CreatedBy = a.CreatedBy,
                                CreatedDate = a.CreatedDate,
                            }).AsEnumerable().Select(x => new ContractDetail
                            {
                                ContractId = x.ContractId,
                                ContractDetailId = x.ContractDetailId,
                                ServiceTypeListId = x.ServiceTypeListId,
                                BillingFrequencyListId = x.BillingFrequencyListId,
                                LineNumber = x.LineNumber,
                                SquareFootage = x.SquareFootage,
                                CleanTimes = x.CleanTimes,
                                Mon = x.Mon,
                                Tues = x.Tues,
                                Wed = x.Wed,
                                Thur = x.Thur,
                                Fri = x.Fr,
                                CleanFrequencyListId = x.CleanFrequencyListId,
                                Amount = x.Amount,
                                BPPAdmin = x.BPPAdmin,
                                //SeparateInvoice = x.SeparateInvoice,
                                //SubjectToFees = x.SubjectToFees,
                                CPIIncrease = x.CPIIncrease,
                                AccountRebate = x.AccountRebate,
                                CreatedBy = x.CreatedBy,
                                CreatedDate = x.CreatedDate
                            }).ToList();


                model.ContractDetail = data.Where(one => one.ContractDetailId == Id).FirstOrDefault().ToModel<ContractDetailViewModel, ContractDetail>();
                return data;
            }
        }

        //public List<ContractDetailViewModel> GetContractDetail()
        //{
        //    using (jkDatabaseEntities context = new jkDatabaseEntities())
        //    {
        //        List<ContractDetailViewModel> lstContractDetailViewModel = new List<ContractDetailViewModel>();
        //        lstContractDetailViewModel.AddRange(
        //        (from Dtl in context.portal_spGet_CustomerDetail
        //         select new { Dtl }).Select(i => new ContractDetailViewModel
        //         {
        //             ContractId = i.ContractId,

        //         }).ToList().OrderBy(c => c.AccountNo));

        //        return lstContractDetailViewModel;
        //    }
        //}

        public ContractDetail GetContractDetailById(int id)
        {
            return Uow.ContractDetail.GetById(id);
        }

        public ContractDetail SaveContractDetail(ContractDetail ContractDetail)
        {
            var ID = ContractDetail.ContractDetailId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                //_uow.ContractDetail.Add(ContractDetail);
                //_uow.Commit();
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {


                    try
                    {
                        // Your code...
                        // Could also be before try if you know the exception occurs in SaveChanges
                        ContractDetail.IsActive = true;
                        ContractDetail.CreatedDate = DateTime.UtcNow;
                        context.Configuration.ProxyCreationEnabled = true;
                        context.ContractDetails.Add(ContractDetail);
                        context.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
            else //update existing entry
            {
                //using (var context = new jkDatabaseEntities())
                //{
                //    ContractDetail cd = new ContractDetail();
                //    cd.Amount = ContractDetail.Amount;
                //    cd.SeparateInvoice = ContractDetail.SeparateInvoice;
                //    cd.ServiceTypeListId = ContractDetail.ServiceTypeListId;
                //    cd.CleanTimes = ContractDetail.CleanTimes;
                //    cd.FrequencyListId = ContractDetail.FrequencyListId;
                //    cd.Description = ContractDetail.Description;
                //    context.SaveChanges();
                //}
                ContractDetail.IsActive = true;
                Uow.ContractDetail.Update(ContractDetail);
                Uow.Commit();
            }

            return ContractDetail;
        }
        public ContractDetail DeleteContractDetail(int id)
        {
            var entity = Uow.ContractDetail.GetById(id);


            // Need a Column for soft Delete 

            Uow.ContractDetail.Delete(entity);
            Uow.Commit();
            return entity;
        }

        public bool HasContractAndContractDetailByCustomerId(int CustomerId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = (
                            from a in context.Contracts
                            join b in context.ContractDetails on a.ContractId equals b.ContractId
                            where a.CustomerId == CustomerId
                            select a).ToList();

                if (data.Count > 0)
                    return true;
                else
                    return false;

            }
        }

        public bool HasDocumentByCustomerId(int CustomerId, int statusid)
        {
            using (var context = new jkDatabaseEntities())
            {
                int ct = 0;
                if (statusid == 38)
                    ct = (from a in context.UploadDocuments where a.ClassId == CustomerId && a.TypeListId == 1 && a.FileTypeListId == 6 select a).ToList().Count;
                else
                    ct = (from a in context.UploadDocuments where a.ClassId == CustomerId && a.TypeListId == 1 && a.FileTypeListId == 6 select a).ToList().Count;

                if (ct > 0)
                    return true;
                else
                    return false;

            }
        }

        //public IQueryable<ContractDetailDescription> GetContractDetailDescription()
        //{
        //    var qry = _uow.ContractDetailDescription.GetAll();
        //    return qry;
        //}

        //public ContractDetailDescription GetContractDetailDescriptionById(int id)
        //{
        //    return _uow.ContractDetailDescription.GetById(id);
        //}

        //public ContractDetailDescription SaveContractDetailDescription(ContractDetailDescription ContractDetailDescription)
        //{
        //    var ID = ContractDetailDescription.ContractDetailDescriptionId;
        //    var isNew = ID == 0;
        //    //add new entry
        //    if (isNew)
        //    {
        //        _uow.ContractDetailDescription.Add(ContractDetailDescription);
        //        _uow.Commit();
        //    }
        //    else //update existing entry
        //    {

        //        _uow.ContractDetailDescription.Update(ContractDetailDescription);
        //        _uow.Commit();
        //    }

        //    return ContractDetailDescription;
        //}

        //public ContractDetailDescription DeleteContractDetailDescription(int id)
        //{
        //    var entity = _uow.ContractDetailDescription.GetById(id);


        //    // Need a Column for soft Delete 

        //    _uow.ContractDetailDescription.Update(entity);
        //    _uow.Commit();
        //    return entity;
        //}



        public IQueryable<FrequencyList> GetFrequencyList()
        {
            var qry = Uow.FrequencyList.GetAll();
            return qry;
        }

        public List<CleanFrequencyListViewModel> GetCleanFrequencyList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = context.CleanFrequencyLists.MapEnumerable<CleanFrequencyListViewModel, CleanFrequencyList>().ToList();
                return qry;
            }
        }

        public FrequencyList GetFrequencyListById(int id)
        {
            return Uow.FrequencyList.GetById(id);
        }

        public FrequencyList SaveFrequencyList(FrequencyList FrequencyList)
        {
            var ID = FrequencyList.FrequencyListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FrequencyList.Add(FrequencyList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FrequencyList.Update(FrequencyList);
                Uow.Commit();
            }

            return FrequencyList;
        }

        public FrequencyList DeleteFrequencyList(int id)
        {
            var entity = Uow.FrequencyList.GetById(id);


            // Need a Column for soft Delete 

            Uow.FrequencyList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<ServiceTypeList> GetServiceTypeList()
        {
            var qry = Uow.ServiceTypeList.GetAll();
            return qry;
        }

        public ServiceTypeList GetServiceTypeListById(int id)
        {
            return Uow.ServiceTypeList.GetById(id);
        }

        public ServiceTypeList SaveServiceTypeList(ServiceTypeList ServiceTypeList)
        {
            var ID = ServiceTypeList.ServiceTypeListid;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ServiceTypeList.Add(ServiceTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ServiceTypeList.Update(ServiceTypeList);
                Uow.Commit();
            }

            return ServiceTypeList;
        }

        public ServiceTypeList DeleteServiceTypeList(int id)
        {
            var entity = Uow.ServiceTypeList.GetById(id);


            // Need a Column for soft Delete 

            Uow.ServiceTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }


        public IQueryable<ContractTypeList> GetContractTypeList()
        {
            var qry = Uow.ContractTypeList.GetAll();
            return qry;
        }

        public ContractTypeList GetContractTypeListById(int id)
        {
            return Uow.ContractTypeList.GetById(id);
        }

        public ContractTypeList SaveContractTypeList(ContractTypeList ContractTypeList)
        {
            var ID = ContractTypeList.ContractTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ContractTypeList.Add(ContractTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ContractTypeList.Update(ContractTypeList);
                Uow.Commit();
            }

            return ContractTypeList;
        }

        public ContractTypeList DeleteContractTypeList(int id)
        {
            var entity = Uow.ContractTypeList.GetById(id);


            // Need a Column for soft Delete 

            Uow.ContractTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        #region  Agreement Type List
        public IQueryable<AgreementTypeList> GetAgreementTypeList()
        {
            var List = Uow.AgreementTypeList.GetAll();
            return List;
        }
        public AgreementTypeList GetAgreementTypeListById(int id)
        {
            return Uow.AgreementTypeList.GetById(id);
        }
        #endregion

        public int UpdateLineNo(int ContractId)
        {
            List<ContractDetail> data = Uow.ContractDetail.GetAll().Where(x => x.ContractId == ContractId).ToList();
            int lineno = 0;
            foreach (var item in data)
            {
                item.LineNumber = lineno + 1;
                SaveContractDetail(item);
                lineno++;
            }
            return ContractId;

        }

        public int getLineNo(int? id)
        {
            int count = Uow.ContractDetail.GetAll().Where(x => x.ContractId == id).Count();
            if (count > 0)
            {
                return Uow.ContractDetail.GetAll().Where(x => x.ContractId == id).Max(x => x.LineNumber).Value;
            }
            return 0;
        }

        public IQueryable<Identification> GetIdentification()
        {
            var qry = Uow.Identification.GetAll();
            return qry;
        }
        public IQueryable<Identification_Temp> GetIdentificationTemp()
        {
            var qry = Uow.Identification_Temp.GetAll();
            return qry;
        }

        public Identification GetIdentificationById(int id)
        {
            return Uow.Identification.GetById(id);
        }

        public Identification SaveIdentification(Identification Identification)
        {
            var ID = Identification.IdentificationId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Identification.Add(Identification);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Identification.Update(Identification);
                Uow.Commit();
            }

            return Identification;
        }
        public Identification_Temp SaveIdentification_Temp(Identification_Temp Identification)
        {
            var ID = Identification.IdentificationId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Identification_Temp.Add(Identification);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Identification_Temp.Update(Identification);
                Uow.Commit();
            }

            return Identification;
        }

        public Identification DeleteIdentification(int id)
        {
            var entity = Uow.Identification.GetById(id);


            // Need a Column for soft Delete 

            Uow.Identification.Update(entity);
            Uow.Commit();
            return entity;
        }



        public List<BankTypeList> GetBankTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = context.BankTypeLists.ToList();
                return qry;
            }
        }

        public List<Bank> GetBanksForRegion()
        {
            return GetBanksForRegion(this.SelectedRegionId);
        }

        public List<Bank> GetBanksForRegion(int regionId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = (from b in context.Banks
                           from rb in context.RegionBanks.Where(o => o.BankId == b.BankId && (o.RegionId == regionId || regionId == 0)).OrderByDescending(o => o.IsDefault)
                           select b).ToList();

                return qry;
            }
        }

        public List<CheckBookTransactionTypeList> GetManualCheckBookTransactionTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = context.CheckBookTransactionTypeLists.Where(o => o.IsManual == true).ToList();
                return qry;
            }
        }

        public IQueryable<AccountTypeList> GetAccountTypeList()
        {
            var qry = Uow.AccountTypeList.GetAll();
            return qry;
        }

        public AccountTypeList GetAccountTypeListById(int id)
        {
            return Uow.AccountTypeList.GetById(id);
        }

        public AccountTypeList SaveAccountTypeList(AccountTypeList AccountTypeList)
        {
            var ID = AccountTypeList.AccountTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.AccountTypeList.Add(AccountTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.AccountTypeList.Update(AccountTypeList);
                Uow.Commit();
            }

            return AccountTypeList;
        }

        public AccountTypeList DeleteAccountTypeList(int id)
        {
            var entity = Uow.AccountTypeList.GetById(id);


            // Need a Column for soft Delete  
            Uow.AccountTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }





        /*
        public IQueryable<ContractTermList> GetContractTermList()
        {
            var qry = _uow.ContractTermList.GetAll();
            return qry;
        }

        public ContractTermList GetContractTermListById(int id)
        {
            return _uow.ContractTermList.GetById(id);
        }
        */

        /*
        public ContractTermList SaveContractTermList(ContractTermList ContractTermList)
        {
            var ID = ContractTermList.ContractTermListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                _uow.ContractTermList.Add(ContractTermList);
                _uow.Commit();
            }
            else //update existing entry
            {

                _uow.ContractTermList.Update(ContractTermList);
                _uow.Commit();
            }

            return ContractTermList;
        }
        */
        /*
        public ContractTermList DeleteContractTermList(int id)
        {
            var entity = _uow.ContractTermList.GetById(id);


            // Need a Column for soft Delete  
            _uow.ContractTermList.Update(entity);
            _uow.Commit();
            return entity;
        }
        */





        public IQueryable<ContractStatusReasonList> GetContractStatusReasonList()
        {
            var qry = Uow.ContractStatusReasonList.GetAll();
            return qry;
        }

        public ContractStatusReasonList GetContractStatusReasonListById(int id)
        {
            return Uow.ContractStatusReasonList.GetById(id);
        }

        public ContractStatusReasonList SaveContractStatusReasonList(ContractStatusReasonList ContractStatusReasonList)
        {
            var ID = ContractStatusReasonList.ContractStatusReasonListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ContractStatusReasonList.Add(ContractStatusReasonList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ContractStatusReasonList.Update(ContractStatusReasonList);
                Uow.Commit();
            }

            return ContractStatusReasonList;
        }

        public ContractStatusReasonList DeleteContractStatusReasonList(int id)
        {
            var entity = Uow.ContractStatusReasonList.GetById(id);


            // Need a Column for soft Delete  
            Uow.ContractStatusReasonList.Update(entity);
            Uow.Commit();
            return entity;
        }





        public CustomerContractViewModel GetCustomerContractByCustomerId(int id)
        {
            CustomerContractViewModel oCustomerContract = new CustomerContractViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", id);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerContract", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCustomerContract = multipleresult.Read<CustomerContractViewModel>().ToList().FirstOrDefault();
                        oCustomerContract.lstContractDetail = multipleresult.Read<CustomerContractDetailViewModel>().ToList();
                    }
                }
            }
            return oCustomerContract;
        }

        public CustomerContractDetailViewModel GetCustomerContractDetailByCustomerId(int ContractDetailId, int ContractId)
        {
            CustomerContractDetailViewModel oCustomerContractDetail = new CustomerContractDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@ContractDetailId", ContractDetailId);
                parmas.Add("@ContractId", ContractId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerContractDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCustomerContractDetail = multipleresult.Read<CustomerContractDetailViewModel>().ToList().FirstOrDefault();
                    }
                }
            }
            return oCustomerContractDetail != null ? oCustomerContractDetail : new CustomerContractDetailViewModel();
        }

        public FullCustomerViewModel GetCustomerDetailsById(int id)
        {
            var context = new jkDatabaseEntities();
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            SqlParameter[] parmList = {
                new SqlParameter("@id",id)
            };
            using (var reader = SQLHelper.ExecuteReader(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.GetCustomerDetailsByIdWithActive, parmList))
            {

                JKApi.Data.DAL.Customer _customer = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Customer>(reader).FirstOrDefault();
                reader.NextResult();
                JKApi.Data.DAL.Contact _Contact = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contact>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Phone _Phone = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Address _Address = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Address>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Email _Email = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Email>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contact _ContactBillingContactData = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contact>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Address _AddressBillingContactData = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Address>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contact _ContactInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contact>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Phone _PhoneInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Address _AddressInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Address>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Email _EmailInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Email>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contact _ContactBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contact>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Phone _PhoneBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Address _AddressBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Address>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Email _EmailBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Email>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.BillSetting _BillSettings = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.BillSetting>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contract _Contract = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contract>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.ContractDetail _ContractDetail = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.ContractDetail>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Phone _PhoneMainType2 = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contact _ContactBillingContactType2 = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contact>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Phone _PhoneBillingContactType = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Email _EmailBillingContactType = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Email>(reader).FirstOrDefault();

                CustomerViewModel.CustomerViewModel = _customer.ToModel<CustomerViewModel, JKApi.Data.DAL.Customer>();

                CustomerViewModel.MainContact = _Contact.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.MainAddress = _Address.ToModel<AddressViewModel, Address>();
                CustomerViewModel.MainPhone = _Phone.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.MainEmail = _Email.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillingContact = _ContactBillingContactData.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingAddress = _AddressBillingContactData.ToModel<AddressViewModel, Address>();
                CustomerViewModel.ContactInformation = _ContactInfo.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.ContactInformationAddress = _AddressInfo.ToModel<AddressViewModel, Address>();
                CustomerViewModel.ContactInformationPhone = _PhoneInfo.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.ContactInformationEmail = _EmailInfo.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillingInformation = _ContactBillingInfo.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingInformationAddress = _AddressBillingInfo.ToModel<AddressViewModel, Address>();
                CustomerViewModel.BillingInformationPhone = _PhoneBillingInfo.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingInformationEmail = _EmailBillingInfo.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillSetting = _BillSettings.ToModel<BillSettingViewModel, BillSetting>();
                CustomerViewModel.Contract = _Contract.ToModel<ContractViewModel, Data.DAL.Contract>();
                CustomerViewModel.ContractDetail = _ContractDetail.ToModel<ContractDetailViewModel, ContractDetail>();
                CustomerViewModel.MainPhone2 = _PhoneMainType2.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingContactInformation2 = _ContactBillingContactType2.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingPhone = _PhoneBillingContactType.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingEmail = _EmailBillingContactType.ToModel<EmailViewModel, Email>();
                CustomerViewModel.lstEBillEmails = context.Emails.Where(one => one.ClassId == id && one.TypeListId == 1 && one.ContactTypeListId == 8 && one.IsActive == true).MapEnumerable<EmailViewModel, Email>().ToList();

                foreach (EmailViewModel op in CustomerViewModel.lstEBillEmails)
                {
                    CustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                }
                if (!String.IsNullOrEmpty(CustomerViewModel.EBill_Emails))
                    CustomerViewModel.EBill_Emails = CustomerViewModel.EBill_Emails.Trim(',');
            }

            return CustomerViewModel;
        }

        public FullCustomerViewModel GetCustomerDetailsByIdWithActive(int id)
        {

            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            int CustomerTypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            int CustomerMainContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            int CustomerBillingContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
            int CustomerContactInformationContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            int CustomerBillingInformationContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;

            int CustomerMainContactType2ListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            int CustomerBillingContactType2ListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;

            using (var context = new jkDatabaseEntities())
            {
                List<FullFranchiseeViewModel> FullFranchiseeViewModel = new List<FullFranchiseeViewModel>();
                var result = (from c in context.Customers
                              where c.CustomerId == id
                              join mc in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals mc.ClassId
                              into temp0
                              from t0 in temp0.DefaultIfEmpty()
                              join mp in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals mp.ClassId
                              into temp1
                              from t1 in temp1.DefaultIfEmpty()
                              join ma in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals ma.ClassId
                              into temp2
                              from t2 in temp2.DefaultIfEmpty()
                              join me in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals me.ClassId
                              into temp3
                              from t3 in temp3.DefaultIfEmpty()


                              join bc in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bc.ClassId
                              into temp4
                              from t4 in temp4.DefaultIfEmpty()
                              join ba in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals ba.ClassId
                              into temp6
                              from t6 in temp6.DefaultIfEmpty()


                              join cic in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerContactInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals cic.ClassId
                              into temp8
                              from t8 in temp8.DefaultIfEmpty()
                              join cip in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerContactInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals cip.ClassId
                              into temp9
                              from t9 in temp9.DefaultIfEmpty()
                              join cia in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerContactInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals cia.ClassId
                              into temp10
                              from t10 in temp10.DefaultIfEmpty()
                              join cie in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerContactInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals cie.ClassId
                              into temp11
                              from t11 in temp11.DefaultIfEmpty()


                              join bic in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bic.ClassId
                              into temp12
                              from t12 in temp12.DefaultIfEmpty()
                              join bip in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bip.ClassId
                              into temp13
                              from t13 in temp13.DefaultIfEmpty()
                              join bia in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bia.ClassId
                              into temp14
                              from t14 in temp14.DefaultIfEmpty()
                              join bie in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bie.ClassId
                              into temp15
                              from t15 in temp15.DefaultIfEmpty()
                              join bs in context.BillSettings.Where(one => one.CustomerId == id && one.IsActive == 1) on c.CustomerId equals bs.CustomerId
                              into temp16
                              from t16 in temp16.DefaultIfEmpty()
                              join cc in context.Contracts.Where(one => one.CustomerId == id) on c.CustomerId equals cc.CustomerId
                              into temp17
                              from t17 in temp17.DefaultIfEmpty()
                              join cd in context.ContractDetails on t17.ContractId equals cd.ContractId
                              into temp18
                              from t18 in temp18.DefaultIfEmpty()


                              join mct in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactType2ListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals mct.ClassId
                              into temp19
                              from t19 in temp19.DefaultIfEmpty()

                              join bct in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactType2ListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bct.ClassId
                              into temp20
                              from t20 in temp20.DefaultIfEmpty()

                              join bp in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bp.ClassId
                              into temp21
                              from t21 in temp21.DefaultIfEmpty()
                              join be in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals be.ClassId
                              into temp22
                              from t22 in temp22.DefaultIfEmpty()


                              where c.RegionId == SelectedRegionId || SelectedRegionId == 0
                              select new CustomerCollection
                              {
                                  Customer = c,
                                  MainContact = t0,
                                  MainPhone = t1,
                                  MainAddress = t2,
                                  MainEmail = t3,
                                  BillingContact = t4,
                                  BillingAddress = t6,
                                  ContactInformation = t8,
                                  ContactInformationPhone = t9,
                                  ContactInformationAddress = t10,
                                  ContactInformationEmail = t11,
                                  BillingInformation = t12,
                                  BillingInformationPhone = t13,
                                  BillingInformationAddress = t14,
                                  BillingInformationEmail = t15,
                                  BillSetting = t16,
                                  Contract = t17,
                                  ContractDetail = t18,
                                  MainPhone2 = t19,
                                  BillingContactInformation2 = t20,
                                  BillingPhone = t21,
                                  BillingEmail = t22

                              }).FirstOrDefault();
                if (result != null)
                {
                    CustomerViewModel.CustomerViewModel = result.Customer.ToModel<CustomerViewModel, JKApi.Data.DAL.Customer>();
                    CustomerViewModel.MainContact = result.MainContact.ToModel<ContactViewModel, Contact>();
                    CustomerViewModel.MainAddress = result.MainAddress.ToModel<AddressViewModel, Address>();
                    CustomerViewModel.MainPhone = result.MainPhone.ToModel<PhoneViewModel, Phone>();
                    CustomerViewModel.MainEmail = result.MainEmail.ToModel<EmailViewModel, Email>();
                    CustomerViewModel.BillingContact = result.BillingContact.ToModel<ContactViewModel, Contact>();
                    CustomerViewModel.BillingAddress = result.BillingAddress.ToModel<AddressViewModel, Address>();
                    CustomerViewModel.ContactInformation = result.ContactInformation.ToModel<ContactViewModel, Contact>();
                    CustomerViewModel.ContactInformationAddress = result.ContactInformationAddress.ToModel<AddressViewModel, Address>();
                    CustomerViewModel.ContactInformationPhone = result.ContactInformationPhone.ToModel<PhoneViewModel, Phone>();
                    CustomerViewModel.ContactInformationEmail = result.ContactInformationEmail.ToModel<EmailViewModel, Email>();
                    CustomerViewModel.BillingInformation = result.BillingInformation.ToModel<ContactViewModel, Contact>();
                    CustomerViewModel.BillingInformationAddress = result.BillingInformationAddress.ToModel<AddressViewModel, Address>();
                    CustomerViewModel.BillingInformationPhone = result.BillingInformationPhone.ToModel<PhoneViewModel, Phone>();
                    CustomerViewModel.BillingInformationEmail = result.BillingInformationEmail.ToModel<EmailViewModel, Email>();
                    CustomerViewModel.BillSetting = result.BillSetting.ToModel<BillSettingViewModel, BillSetting>();
                    CustomerViewModel.Contract = result.Contract.ToModel<ContractViewModel, Data.DAL.Contract>();
                    CustomerViewModel.ContractDetail = result.ContractDetail.ToModel<ContractDetailViewModel, ContractDetail>();
                    CustomerViewModel.MainPhone2 = result.MainPhone2.ToModel<PhoneViewModel, Phone>();
                    CustomerViewModel.BillingContactInformation2 = result.BillingContactInformation2.ToModel<ContactViewModel, Contact>();
                    CustomerViewModel.BillingPhone = result.BillingPhone.ToModel<PhoneViewModel, Phone>();
                    CustomerViewModel.BillingEmail = result.BillingEmail.ToModel<EmailViewModel, Email>();

                    CustomerViewModel.lstEBillEmails = context.Emails.Where(one => one.ClassId == id && one.TypeListId == 1 && one.ContactTypeListId == 8 && one.IsActive == true).MapEnumerable<EmailViewModel, Email>().ToList();

                    foreach (EmailViewModel op in CustomerViewModel.lstEBillEmails)
                    {
                        CustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                    }
                    if (!String.IsNullOrEmpty(CustomerViewModel.EBill_Emails))
                        CustomerViewModel.EBill_Emails = CustomerViewModel.EBill_Emails.Trim(',');
                }
                return CustomerViewModel;
            }
        }
        public FullCustomerViewModel GetCustomerDetailsByIdWithActiveData(int id)
        {
            var context = new jkDatabaseEntities();
            FullCustomerViewModel CustomerViewModel = new FullCustomerViewModel();
            SqlParameter[] parmList = {
                new SqlParameter("@id",id)
            };
            using (var reader = SQLHelper.ExecuteReader(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.GetCustomerDetailsByIdWithActive, parmList))
            {

                Data.DAL.Customer _customer = ((IObjectContextAdapter)context).ObjectContext.Translate<Data.DAL.Customer>(reader).FirstOrDefault();
                reader.NextResult();
                Contact _Contact = ((IObjectContextAdapter)context).ObjectContext.Translate<Contact>(reader).FirstOrDefault();

                reader.NextResult();
                Phone _Phone = ((IObjectContextAdapter)context).ObjectContext.Translate<Phone>(reader).FirstOrDefault();

                reader.NextResult();
                Address _Address = ((IObjectContextAdapter)context).ObjectContext.Translate<Address>(reader).FirstOrDefault();

                reader.NextResult();
                Email _Email = ((IObjectContextAdapter)context).ObjectContext.Translate<Email>(reader).FirstOrDefault();

                reader.NextResult();
                Contact _ContactBillingContactData = ((IObjectContextAdapter)context).ObjectContext.Translate<Contact>(reader).FirstOrDefault();

                reader.NextResult();
                Address _AddressBillingContactData = ((IObjectContextAdapter)context).ObjectContext.Translate<Address>(reader).FirstOrDefault();

                reader.NextResult();
                Contact _ContactInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<Contact>(reader).FirstOrDefault();

                reader.NextResult();
                Phone _PhoneInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<Phone>(reader).FirstOrDefault();

                reader.NextResult();
                Address _AddressInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<Address>(reader).FirstOrDefault();

                reader.NextResult();
                Email _EmailInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<Email>(reader).FirstOrDefault();

                reader.NextResult();
                Contact _ContactBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<Contact>(reader).FirstOrDefault();

                reader.NextResult();
                Phone _PhoneBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Phone>(reader).FirstOrDefault();

                reader.NextResult();
                Address _AddressBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Address>(reader).FirstOrDefault();

                reader.NextResult();
                Email _EmailBillingInfo = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Email>(reader).FirstOrDefault();

                reader.NextResult();
                BillSetting _BillSettings = ((IObjectContextAdapter)context).ObjectContext.Translate<BillSetting>(reader).FirstOrDefault();

                reader.NextResult();
                JKApi.Data.DAL.Contract _Contract = ((IObjectContextAdapter)context).ObjectContext.Translate<JKApi.Data.DAL.Contract>(reader).FirstOrDefault();

                reader.NextResult();
                ContractDetail _ContractDetail = ((IObjectContextAdapter)context).ObjectContext.Translate<ContractDetail>(reader).FirstOrDefault();

                reader.NextResult();
                Phone _PhoneMainType2 = ((IObjectContextAdapter)context).ObjectContext.Translate<Phone>(reader).FirstOrDefault();

                reader.NextResult();
                Contact _ContactBillingContactType2 = ((IObjectContextAdapter)context).ObjectContext.Translate<Contact>(reader).FirstOrDefault();

                reader.NextResult();
                Phone _PhoneBillingContactType = ((IObjectContextAdapter)context).ObjectContext.Translate<Phone>(reader).FirstOrDefault();

                reader.NextResult();
                Email _EmailBillingContactType = ((IObjectContextAdapter)context).ObjectContext.Translate<Email>(reader).FirstOrDefault();

                CustomerViewModel.CustomerViewModel = _customer.ToModel<CustomerViewModel, JKApi.Data.DAL.Customer>();

                CustomerViewModel.MainContact = _Contact.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.MainAddress = _Address.ToModel<AddressViewModel, Address>();
                CustomerViewModel.MainPhone = _Phone.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.MainEmail = _Email.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillingContact = _ContactBillingContactData.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingAddress = _AddressBillingContactData.ToModel<AddressViewModel, Address>();
                CustomerViewModel.ContactInformation = _ContactInfo.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.ContactInformationAddress = _AddressInfo.ToModel<AddressViewModel, Address>();
                CustomerViewModel.ContactInformationPhone = _PhoneInfo.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.ContactInformationEmail = _EmailInfo.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillingInformation = _ContactBillingInfo.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingInformationAddress = _AddressBillingInfo.ToModel<AddressViewModel, Address>();
                CustomerViewModel.BillingInformationPhone = _PhoneBillingInfo.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingInformationEmail = _EmailBillingInfo.ToModel<EmailViewModel, Email>();
                CustomerViewModel.BillSetting = _BillSettings.ToModel<BillSettingViewModel, BillSetting>();
                CustomerViewModel.Contract = _Contract.ToModel<ContractViewModel, Data.DAL.Contract>();
                CustomerViewModel.ContractDetail = _ContractDetail.ToModel<ContractDetailViewModel, ContractDetail>();
                CustomerViewModel.MainPhone2 = _PhoneMainType2.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingContactInformation2 = _ContactBillingContactType2.ToModel<ContactViewModel, Contact>();
                CustomerViewModel.BillingPhone = _PhoneBillingContactType.ToModel<PhoneViewModel, Phone>();
                CustomerViewModel.BillingEmail = _EmailBillingContactType.ToModel<EmailViewModel, Email>();
                CustomerViewModel.lstEBillEmails = context.Emails.Where(one => one.ClassId == id && one.TypeListId == 1 && one.ContactTypeListId == 8 && one.IsActive == true).MapEnumerable<EmailViewModel, Email>().ToList();

                foreach (EmailViewModel op in CustomerViewModel.lstEBillEmails)
                {
                    CustomerViewModel.EBill_Emails += op.EmailAddress + ",";
                }
                if (!String.IsNullOrEmpty(CustomerViewModel.EBill_Emails))
                    CustomerViewModel.EBill_Emails = CustomerViewModel.EBill_Emails.Trim(',');
            }

            return CustomerViewModel;
        }

        public IEnumerable<CustomerCollection> GetCustomerByStatus(int StatusId, int TypeListId, int ContactTypeListId)
        {

            using (var context = new jkDatabaseEntities())
            {
                var result = (from c in context.Customers
                              join cc in context.Contacts.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on c.CustomerId equals cc.ClassId
                              into temp0
                              from t0 in temp0.DefaultIfEmpty()
                              join cp in context.Phones.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on c.CustomerId equals cp.ClassId
                              into temp1
                              from t1 in temp1.DefaultIfEmpty()
                              join ca in context.Addresses.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on c.CustomerId equals ca.ClassId
                              into temp2
                              from t2 in temp2.DefaultIfEmpty()
                              join ce in context.Emails.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on c.CustomerId equals ce.ClassId
                              into temp3
                              from t3 in temp3.DefaultIfEmpty()
                              join ccc in context.Contracts on c.CustomerId equals ccc.CustomerId
                              into temp4
                              from t4 in temp4.DefaultIfEmpty()
                              join stl in context.AccountTypeLists on t4.AccountTypeListId equals stl.AccountTypeListId
                              into temp5
                              from t5 in temp5.DefaultIfEmpty()
                              join stl1 in context.StateLists on t2.StateListId equals stl1.StateListId
                             into temp6
                              from t6 in temp6.DefaultIfEmpty()
                              where c.RegionId == SelectedRegionId || SelectedRegionId == 0
                              select new CustomerCollection
                              {
                                  Customer = c,
                                  MainContact = t0,
                                  MainPhone = t1,
                                  MainAddress = t2,
                                  MainEmail = t3,
                                  Contract = t4,
                                  AccountTypeList = t5,
                                  StateList = t6,
                              }).Distinct().ToList();

                //var result = (from c in context.Customers
                //              join cc in context.Contacts on c.CustomerId equals cc.ClassId
                //              join cp in context.Phones on c.CustomerId equals cp.ClassId
                //              join ca in context.Addresses on c.CustomerId equals ca.ClassId
                //              join ce in context.Emails on c.CustomerId equals ce.ClassId
                //              join ccc in context.Contracts on c.CustomerId equals ccc.CustomerId
                //              join st1 in context.AccountTypeLists on ccc.AccountTypeListId equals st1.AccountTypeListId
                //              where cc.TypeListId == TypeListId && cc.ContactTypeListId == ContactTypeListId
                //              select new CustomerCollection
                //              {
                //                  Customer = c,
                //                  MainContact = cc,
                //                  MainPhone = cp,
                //                  MainAddress = ca,
                //                  MainEmail = ce,
                //                  Contract = ccc,
                //                  AccountTypeList = st1
                //              }).Distinct().ToList();


                return result;
            }
        }

        public List<CustomerSearchResultViewModel> GetCustomerSearchList(int StatusId, int ContactTypeListId, string StatusListIds = "")
        {
            List<CustomerSearchResultViewModel> lstCustomerSearchResultViewModel = new List<CustomerSearchResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", SelectedRegionId.ToString());
                parmas.Add("@ContactTypeListId", ContactTypeListId);
                parmas.Add("@StatusList", StatusListIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerSearchList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerSearchResultViewModel = multipleresult.Read<CustomerSearchResultViewModel>().ToList();
                    }
                }
            }
            return lstCustomerSearchResultViewModel;

            //using (var context = new jkDatabaseEntities())
            //{
            //    return context.portal_spGet_C_CustomerSearchList(SelectedRegionId.ToString(), ContactTypeListId, StatusListIds).MapEnumerable<CustomerSearchResultViewModel, portal_spGet_C_CustomerSearchList_Result>().ToList();
            //}
        }

        public CustomerServiceQuickActionViewModel GetCustomerServiceQuickAction(int selectedRegion)
        {
            List<CustomerServiceQuickActionViewModel> lstCustomerPendingMaintenance = new List<CustomerServiceQuickActionViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", selectedRegion);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CS_GetCustomerServiceQuickAction", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerPendingMaintenance = multipleresult.Read<CustomerServiceQuickActionViewModel>().ToList();
                    }
                }

            }
            return lstCustomerPendingMaintenance.FirstOrDefault();
        }



        public List<CustomerSearchResultViewModel> GetCustomerSearchList(int contactTypeListId, string statusListIds = "0", string regionId = "")
        {

            List<CustomerSearchResultViewModel> lstCustomerSearchResultViewModel = new List<CustomerSearchResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@ContactTypeListId", contactTypeListId);
                parmas.Add("@StatusList", statusListIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerSearchList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerSearchResultViewModel = multipleresult.Read<CustomerSearchResultViewModel>().ToList();
                    }
                }

            }
            return lstCustomerSearchResultViewModel;
            //using (var context = new jkDatabaseEntities())
            //{
            //    return context.portal_spGet_C_CustomerSearchList(!string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString(), contactTypeListId, statusListIds)
            //        .MapEnumerable<CustomerSearchResultViewModel, portal_spGet_C_CustomerSearchList_Result>().ToList();
            //}
        }

        public List<CustomerPendingMaintenanceListViewModel> GetCustomerPendingMaintenanceList(string selectedRegion)
        {
            List<CustomerPendingMaintenanceListViewModel> lstCustomerPendingMaintenance = new List<CustomerPendingMaintenanceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", selectedRegion);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CustomerPendingMaintenanceList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerPendingMaintenance = multipleresult.Read<CustomerPendingMaintenanceListViewModel>().ToList();
                    }
                }

            }
            return lstCustomerPendingMaintenance;
        }

        public List<StatusList> GetAll_StatusList()
        {
            return jkEntityModel.StatusLists.AsNoTracking().ToList();
        }

        public List<MaintenanceTypeList> GetAll_MaintenanceTypeList(int typeListid)
        {
            return jkEntityModel.MaintenanceTypeLists.AsNoTracking().Where(o => o.TypeListId == typeListid).ToList();
        }

        public List<StatusList> GetAll_StatusListByTypeListId(int TypeListId)
        {
            return jkEntityModel.StatusLists.AsNoTracking().Where(r => r.TypeListId == TypeListId).ToList();
        }
        public List<StatusReasonList> GetAll_StatusReasonList(int TypeListId)
        {
            return jkEntityModel.StatusReasonLists.AsNoTracking().Where(r => r.TypeListId == TypeListId).ToList();
        }

        public CustomerMaintenanceViewModel GetCustomerMaintenanceDetailsById(int id)
        {
            CustomerMaintenanceViewModel oCustomerMaintenanceViewModel = new CustomerMaintenanceViewModel();
            using (var context = new jkDatabaseEntities())
            {
                List<portal_spGet_CustomerMaintenanceList_Result> lstCustomerMaintenanceList = jkEntityModel.portal_spGet_CustomerMaintenanceList(id).ToList();
                if (lstCustomerMaintenanceList.Count > 0)
                {
                    portal_spGet_CustomerMaintenanceList_Result ob = lstCustomerMaintenanceList.FirstOrDefault();
                    oCustomerMaintenanceViewModel.CustomerId = ob.CustomerId;
                    oCustomerMaintenanceViewModel.CustomerNo = ob.CustomerNo;
                    oCustomerMaintenanceViewModel.CustomerName = ob.CustomerName;
                    oCustomerMaintenanceViewModel.CustomerAddress = ob.CustomerAddress;
                    oCustomerMaintenanceViewModel.CustomerCity = ob.CustomerCity;
                    oCustomerMaintenanceViewModel.CustomerState = ob.CustomerState;
                    oCustomerMaintenanceViewModel.CustomerPincode = ob.CustomerPincode;
                    oCustomerMaintenanceViewModel.CustomerBillingName = ob.CustomerBillingName;
                    oCustomerMaintenanceViewModel.CustomerBillingAddress = ob.CustomerBillingAddress;
                    oCustomerMaintenanceViewModel.CustomerBillingCity = ob.CustomerBillingCity;
                    oCustomerMaintenanceViewModel.CustomerBillingState = ob.CustomerBillingState;
                    oCustomerMaintenanceViewModel.CustomerBillingPincode = ob.CustomerBillingPincode;
                    oCustomerMaintenanceViewModel.StatusId = ob.StatusId;
                    oCustomerMaintenanceViewModel.StatusListId = ob.StatusListId;
                    oCustomerMaintenanceViewModel.StatusListName = ob.StatusListName;
                    oCustomerMaintenanceViewModel.ReasonListId = ob.ReasonListId;
                    oCustomerMaintenanceViewModel.ReasonListName = ob.ReasonListName;
                    oCustomerMaintenanceViewModel.StatusDate = ob.StatusDate;
                    oCustomerMaintenanceViewModel.StatusNotes = ob.StatusNotes;
                    oCustomerMaintenanceViewModel.ResumeDate = ob.ResumeDate;
                    oCustomerMaintenanceViewModel.LastServiceDate = ob.LastServiceDate;
                    oCustomerMaintenanceViewModel.IsActive = ob.IsActive;
                    oCustomerMaintenanceViewModel.CreatedBy = ob.CreatedBy;
                    oCustomerMaintenanceViewModel.CreatedDate = ob.CreatedDate;

                    List<CustomerStatusFindersFeeViewModel> lstCustomerStatusFindersFeeViewModel = new List<CustomerStatusFindersFeeViewModel>();
                    CustomerStatusFindersFeeViewModel oCustomerStatusFindersFeeViewModel;
                    foreach (portal_spGet_CustomerMaintenanceList_Result o in lstCustomerMaintenanceList)
                    {
                        oCustomerStatusFindersFeeViewModel = new CustomerStatusFindersFeeViewModel();
                        oCustomerStatusFindersFeeViewModel.FindersFeeId = o.FindersFeeId;
                        oCustomerStatusFindersFeeViewModel.LineNumber = o.LineNumber;
                        oCustomerStatusFindersFeeViewModel.ServiceTypeListId = o.ServiceTypeListid;
                        oCustomerStatusFindersFeeViewModel.ServiceTypeListName = o.ServiceTypeListName;
                        oCustomerStatusFindersFeeViewModel.FranchiseeId = o.FranchiseeId;
                        oCustomerStatusFindersFeeViewModel.FranchiseeNo = o.FranchiseeNo;
                        oCustomerStatusFindersFeeViewModel.FranchiseeName = o.FranchiseeName;
                        //oCustomerStatusFindersFeeViewModel.FindersFeeStopDate = o.StopDate;
                        oCustomerStatusFindersFeeViewModel.FindersFeeHasCancellationFee = o.HasCancellationFee != null ? (bool)o.HasCancellationFee : false;

                        oCustomerStatusFindersFeeViewModel.FindersFeeNumber = o.FindersFeeNumber;
                        oCustomerStatusFindersFeeViewModel.TotalNumOfpayments = o.TotalNumOfpayments;
                        oCustomerStatusFindersFeeViewModel.NumOfPaymentsPaid = o.NumOfPaymentsPaid;
                        oCustomerStatusFindersFeeViewModel.BalanceAmount = o.BalanceAmount;
                        oCustomerStatusFindersFeeViewModel.TotalAmount = o.TotalAmount;
                        oCustomerStatusFindersFeeViewModel.Description = o.Description;

                        lstCustomerStatusFindersFeeViewModel.Add(oCustomerStatusFindersFeeViewModel);

                    }
                    oCustomerMaintenanceViewModel.FindersFee = lstCustomerStatusFindersFeeViewModel;

                }

                return oCustomerMaintenanceViewModel;
            }
        }




        public ContractDetail SaveContractDetailbyModel(ContractDetail ContractDetail)
        {
            var ID = ContractDetail.ContractDetailId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ContractDetail.Add(ContractDetail);
                Uow.Commit();
            }
            else //update existing entry
            {
                using (var context = new jkDatabaseEntities())
                {
                    ContractDetail cd = new ContractDetail();
                    cd.Amount = ContractDetail.Amount;
                    //cd.SeparateInvoice = ContractDetail.SeparateInvoice;
                    cd.ServiceTypeListId = ContractDetail.ServiceTypeListId;
                    cd.CleanTimes = ContractDetail.CleanTimes;
                    cd.BillingFrequencyListId = ContractDetail.BillingFrequencyListId;
                    context.SaveChanges();
                }
                Uow.ContractDetail.Update(ContractDetail);
                Uow.Commit();
            }

            return ContractDetail;
        }


        public JKApi.Data.DAL.Bank SaveBank(JKApi.Data.DAL.Bank Bank)
        {
            var ID = Bank.BankId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Bank.Add(Bank);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Bank.Update(Bank);
                Uow.Commit();
            }

            return Bank;
        }


        public List<CustomerDetailTransactionViewModel> GetCustomerDetailTransactions(int customerId, int typeId, System.DateTime startDate, System.DateTime endDate)
        {
            return jkEntityModel.portal_spGet_C_TransactionDetail(customerId, typeId, startDate, endDate).MapEnumerable<CustomerDetailTransactionViewModel, portal_spGet_C_TransactionDetail_Result>().ToList();
        }

        public decimal GetCustomerBalanceAsOfDate(int customerId, DateTime? asOfDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.fn_GetCustomerBalanceAsOfDate(customerId, asOfDate).FirstOrDefault();
                return data?.Balance ?? 0;
            }
        }

        public RemitToViewModel GetRemitToForRegion(int regionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var vm = new RemitToViewModel();

                var region = context.Regions.Where(o => o.RegionId == regionId).FirstOrDefault();
                if (region != null)
                {
                    if (Convert.ToBoolean(region.RemitSameAsMain) == true)
                    {
                        vm.Address = new AddressViewModel();
                        vm.Address.Address1 = region.Address;
                        vm.Address.City = region.City;
                        vm.Address.StateName = region.State;
                        vm.Address.PostalCode = region.PostalCode;
                        vm.RegionName = region.Displayname;
                        vm.ContactName = region.Director;
                        vm.ContactPhone = String.Format("{0:(###) ###-####}", double.Parse(region.Phone));
                    }
                    else
                    {
                        var RemitTo = context.RemitToes.Where(o => o.RegionId == regionId).FirstOrDefault();
                        if (RemitTo != null)
                        {
                            vm.Address = new AddressViewModel();
                            vm.Address.Address1 = RemitTo.Address;
                            vm.Address.City = RemitTo.City;
                            vm.Address.StateName = RemitTo.State;
                            vm.Address.PostalCode = RemitTo.PostalCode;
                            vm.RegionName = RemitTo.RegionName;
                            vm.ContactName = "";
                            vm.ContactPhone = "";
                        }
                    }
                }
                else
                {
                    var RemitTo = context.RemitToes.Where(o => o.RegionId == regionId).FirstOrDefault();
                    if (RemitTo != null)
                    {
                        vm.Address = new AddressViewModel();
                        vm.Address.Address1 = RemitTo.Address;
                        vm.Address.City = RemitTo.City;
                        vm.Address.StateName = RemitTo.State;
                        vm.Address.PostalCode = RemitTo.PostalCode;
                        vm.RegionName = RemitTo.RegionName;
                        vm.ContactName = "";
                        vm.ContactPhone = "";
                    }
                }
                //var remitTo = context.RemitToes.Where(o => o.RegionId == regionId).FirstOrDefault();
                //if (remitTo == null)
                //    return vm;

                //vm.Address = new AddressViewModel();
                //vm.Address.Address1 = remitTo.Address;
                //vm.Address.City = remitTo.City;
                //vm.Address.StateName = remitTo.State;
                //vm.Address.PostalCode = remitTo.PostalCode;

                //vm.RegionName = remitTo.RegionName;

                //var region = context.Regions.Where(o => o.RegionId == regionId).FirstOrDefault();
                //if (region == null)
                //    return vm;

                //vm.ContactName = region.Director;
                //vm.ContactPhone = String.Format("{0:(###) ###-####}", double.Parse(region.Phone));

                return vm;
            }
        }

        public IQueryable<JKApi.Data.DAL.Customer> GetSearchCustomers()
        {
            return Uow.Customer.GetAll();
        }

        public List<DistributionFee> GetDistributionFeeData(int id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.DistributionFees.AsNoTracking().Where(o => o.DistributionId == id).ToList();
                return data;
            }
        }

        public List<vw_Fee> GetFeeData()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.vw_Fee.AsNoTracking().ToList();
                return data;
            }
        }

        public List<FranchiseeDistributionFeesViewModel> GetFranchiseeDistributionFeesData(int FranchiseeId, int ContractDetailId, int DistributionId = 0)
        {
            List<FranchiseeDistributionFeesViewModel> lstFranchiseeDistributionFees = new List<FranchiseeDistributionFeesViewModel>();

            lstFranchiseeDistributionFees = jkEntityModel.vw_FranchiseeDistributionFees.AsNoTracking().Where(o => o.FranchiseeId == FranchiseeId && o.ContractDetailId == ContractDetailId).MapEnumerable<FranchiseeDistributionFeesViewModel, vw_FranchiseeDistributionFees>().ToList();

            if (lstFranchiseeDistributionFees.Count == 0)
            {
                if (ContractDetailId > 0)
                {
                    lstFranchiseeDistributionFees = (from ff in jkEntityModel.FranchiseeFees
                                                     join f in jkEntityModel.vw_Fee on ff.FeesId equals f.FeeId
                                                     join cnt in jkEntityModel.Franchisees on ff.FranchiseeId equals cnt.FranchiseeId
                                                     join cd in jkEntityModel.ContractDetails on ContractDetailId equals cd.ContractDetailId
                                                     where ff.FranchiseeId == FranchiseeId
                                                     select new FranchiseeDistributionFeesViewModel
                                                     {
                                                         DistributionFeesId = 0,
                                                         DistributionId = DistributionId,
                                                         CustomerId = cd.Contract.CustomerId,
                                                         ContractDetailId = cd.ContractDetailId,
                                                         FranchiseeId = FranchiseeId,
                                                         DetailLineNumber = cd.LineNumber,
                                                         FeeId = f.FeeId,
                                                         FeeName = f.FeeName,
                                                         FeeRateTypeListId = f.FeeRateTypeListId,
                                                         FeeRateTypeName = f.FeeRateTypeName,
                                                         Amount = (ff.Amount.HasValue ? ff.Amount.Value : 0)
                                                     }).Distinct().ToList();
                }
                else
                {
                    lstFranchiseeDistributionFees = (from ff in jkEntityModel.FranchiseeFees
                                                     join f in jkEntityModel.vw_Fee on ff.FeesId equals f.FeeId
                                                     join cnt in jkEntityModel.Franchisees on ff.FranchiseeId equals cnt.FranchiseeId

                                                     where ff.FranchiseeId == FranchiseeId
                                                     select new FranchiseeDistributionFeesViewModel
                                                     {
                                                         DistributionFeesId = 0,
                                                         DistributionId = DistributionId,
                                                         CustomerId = 0,
                                                         ContractDetailId = 0,
                                                         FranchiseeId = FranchiseeId,
                                                         DetailLineNumber = 0,
                                                         FeeId = f.FeeId,
                                                         FeeName = f.FeeName,
                                                         FeeRateTypeListId = f.FeeRateTypeListId,
                                                         FeeRateTypeName = f.FeeRateTypeName,
                                                         Amount = (ff.Amount.HasValue ? ff.Amount.Value : 0)
                                                     }).Distinct().ToList();
                }
            }



            return lstFranchiseeDistributionFees;
        }


        public CFranchiseeDistributionViewModel GetCustomerFranchiseeDistribution(int customerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CFranchiseeDistributionViewModel oFranchiseeDistribution = new CFranchiseeDistributionViewModel();

                JKApi.Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(o => o.CustomerId == customerId);

                if (oCustomer != null)
                {

                    oFranchiseeDistribution.CustomerId = oCustomer.CustomerId;
                    oFranchiseeDistribution.CustomerNo = oCustomer.CustomerNo;
                    oFranchiseeDistribution.CustomerName = oCustomer.Name;

                    oFranchiseeDistribution.lstContractDetail = context.vw_ContractDetail.Where(o => o.CustomerId == customerId).ToList();
                    using (jkDatabaseEntities contextjk = new jkDatabaseEntities())
                    {
                        JKApi.Data.DAL.Contract oContract = contextjk.Contracts.Where(x => x.isActive == true).FirstOrDefault(o => o.CustomerId == oCustomer.CustomerId);
                        oFranchiseeDistribution.lstContractDetailDistribution = context.vw_ContractDetailDestribution.Where(o => o.ContractId == oContract.ContractId).ToList();
                        JKApi.Data.DAL.ContractDetail oContractDetail = contextjk.ContractDetails.FirstOrDefault(o => o.ContractId == oContract.ContractId);
                        oFranchiseeDistribution.lstdistributionlist = context.vw_Distribution.Where(o => o.ContractDetailId == oContractDetail.ContractDetailId).ToList();
                    }

                }
                return oFranchiseeDistribution;
            }
        }

        public CustomerFranchiseeDistributionViewModel GetCustomerFranchiseeDistributionData(int customerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CustomerFranchiseeDistributionViewModel oFranchiseeDistribution = new CustomerFranchiseeDistributionViewModel();

                JKApi.Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(o => o.CustomerId == customerId);

                if (oCustomer != null)
                {
                    JKApi.Data.DAL.Contract oContract = context.Contracts.Where(x => x.isActive == true).FirstOrDefault(o => o.CustomerId == oCustomer.CustomerId);

                    oFranchiseeDistribution.CustomerId = oCustomer.CustomerId;
                    oFranchiseeDistribution.CustomerNo = oCustomer.CustomerNo;
                    oFranchiseeDistribution.CustomerName = oCustomer.Name;

                    AccountTypeList oAccountTypeList = context.AccountTypeLists.FirstOrDefault(o => o.AccountTypeListId == oContract.AccountTypeListId);

                    oFranchiseeDistribution.AccountTypeName = (oAccountTypeList != null ? oAccountTypeList.Name : string.Empty);
                    oFranchiseeDistribution.ExpirationDate = oContract.ExpirationDate;
                    oFranchiseeDistribution.PONumber = oContract.PurchaseOrderNo;
                    oFranchiseeDistribution.StartDate = oContract.StartDate;
                    oFranchiseeDistribution.Status = oContract.isActive != null ? ((bool)oContract.isActive == true ? "Active" : "InActive") : "InActive";

                    var statuslist = context.StatusLists.Where(w => w.StatusListId == oCustomer.StatusListId);
                    if (statuslist != null && statuslist.Count() > 0)
                    {
                        oFranchiseeDistribution.Status = statuslist.FirstOrDefault().Name;
                    }
                    oFranchiseeDistribution.Term = "";
                    oFranchiseeDistribution.TotalAmount = (decimal)oContract.Amount;

                    oFranchiseeDistribution.listContractDetail = context.portal_spGet_ContractDetailByCustomerID(customerId).ToList();
                    oFranchiseeDistribution.listFranchiseeDistribution = context.portal_spGet_CustomerDetailFranchiseeDistribution(oContract.ContractId).ToList();

                }
                return oFranchiseeDistribution;
            }
        }




        public CFranchiseeDistributionViewModel GetFranchiseeDistribution(int FranchiseeId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CFranchiseeDistributionViewModel oFranchiseeDistribution = new CFranchiseeDistributionViewModel();

                JKApi.Data.DAL.Franchisee oFranchisee = context.Franchisees.SingleOrDefault(o => o.FranchiseeId == FranchiseeId);

                if (oFranchisee != null)
                {
                    oFranchiseeDistribution.CustomerId = oFranchisee.FranchiseeId;
                    oFranchiseeDistribution.CustomerNo = oFranchisee.FranchiseeNo;
                    oFranchiseeDistribution.CustomerName = oFranchisee.Name;

                    oFranchiseeDistribution.lstDistribution = context.vw_CustomerDetailFranchiseeDistribution.Where(o => o.FranchiseeId == FranchiseeId).ToList();
                    int ContractDetailId = (oFranchiseeDistribution.lstDistribution.Count > 0 ? Convert.ToInt32(oFranchiseeDistribution.lstDistribution.FirstOrDefault().ContractDetailId) : 0);
                    if (ContractDetailId != 0)
                    {
                        oFranchiseeDistribution.lstContractDetail = context.vw_ContractDetail.Where(o => o.ContractDetailId == ContractDetailId).ToList();
                    }

                }
                return oFranchiseeDistribution;
            }
        }

        //lakhwinder
        public List<ServiceCallLogAreaList> GetServiceCallLogAreaList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.ServiceCallLogAreaLists.AsNoTracking().ToList();
            }
        }

        public List<UserLoginModel> GetUserOfDefaultRegion(int RegionId, bool isActive)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmList = new DynamicParameters();

                parmList.Add("@RegionId", RegionId);
                parmList.Add("@isActive", isActive);


                using (var multipleresult = conn.QueryMultiple(DBConstants.Auth_UserDefaultRegionList, parmList, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<UserLoginModel>().ToList();
                    }
                }
            }
            return new List<UserLoginModel>();
        }
        public List<ServiceCallLogTypeList> GetServiceCallLogTypeList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.ServiceCallLogTypeLists.OrderBy(o => o.RankNo).AsNoTracking().ToList();
            }
        }

        public List<StatusResultList> GetStatusResultList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.StatusResultLists.AsNoTracking().ToList();
            }
        }

        public bool SaveServiceCallLog(ServiceCallLog serviceCallLogModel)
        {
            try
            {
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    serviceCallLogModel.CreatedDate = DateTime.Now;
                    serviceCallLogModel.CreatedBy = LoginUserId;
                    serviceCallLogModel.StatusListId = 67;//DeFult STATUS OPEN = 67
                    context.ServiceCallLogs.Add(serviceCallLogModel);
                    context.SaveChanges();
                }

                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    var data = context.portal_sp_UpdateCustomerLogId(serviceCallLogModel.ClassId, serviceCallLogModel.TypeListId, serviceCallLogModel.ServiceLogTypeListId, LoginUserId);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateServiceCallLogDetails(ServiceCallLog serviceCallLogModel)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                serviceCallLogModel.ModifiedDate = DateTime.Now;
                serviceCallLogModel.ModifiedBy = LoginUserId;
                //context.SaveChanges();
                Uow.ServiceCallLog.Update(serviceCallLogModel);
                Uow.Commit();
            }
            return true;
        }
        public bool ServiceCallLogDetailsUpdatePopup(ServiceCallLog serviceCallLogModel)
        {
             
                var model = Uow.ServiceCallLog.GetById(serviceCallLogModel.ServiceCallLogId);
                if (model != null)
                {
                    //ServiceCallLog model = new ServiceCallLog();
                    model.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Closed;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = LoginUserId;
                    Uow.ServiceCallLog.Update(model);
                    Uow.Commit();
                }

                serviceCallLogModel.ModifiedDate = DateTime.Now;
                serviceCallLogModel.ModifiedBy = LoginUserId;
                //context.SaveChanges();
                Uow.ServiceCallLog.Add(serviceCallLogModel);
                Uow.Commit();
            
            return true;
        }

        public bool SaveCollectionCallLog(CollectionsCallLogModel collectionCallLogModel)
        {
            try
            {
                dynamic franchisees;
                collectionCallLogModel.CreatedDate = DateTime.Now;
                collectionCallLogModel.CreatedBy = -1;
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CollectionsCallLogModel, CollectionsCallLog>();
                });
                CollectionsCallLog objCollectionsCallLog = AutoMapper.Mapper.Map<CollectionsCallLog>(collectionCallLogModel);
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    context.CollectionsCallLogs.Add(objCollectionsCallLog);
                    context.SaveChanges();

                    franchisees = (collectionCallLogModel.arrFranchiseIds != null ? context.Franchisees.AsNoTracking().AsQueryable().Where(x => collectionCallLogModel.arrFranchiseIds.Contains(x.FranchiseeId)).Select(x => x).ToList() : null);

                }
                //getFranchisess to save to save in a CallLogAssociate

                string spName = "dbo.poratal_CallLogAssociate";
                using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (franchisees != null)
                    {
                        foreach (var franchise in franchisees)
                        {
                            cmd.Parameters.AddWithValue("@Classid", franchise.FranchiseeId);
                            cmd.Parameters.AddWithValue("@TypeListid", (int)Business.Enumeration.TypeList.Franchisee);
                            cmd.Parameters.AddWithValue("@CallLogAssociateTypeListid", (int)Business.Enumeration.CallLogAssociateTypeList.CollectionCallLog);
                            cmd.Parameters.AddWithValue("@Regionid", franchise.RegionId);
                            cmd.Parameters.AddWithValue("@CreatedBy", -1);
                            cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now.ToString("dd/MM/yyyy"));
                            cmd.Parameters.AddWithValue("@ModifiedBy", null);
                            cmd.Parameters.AddWithValue("@ModifiedDate", null);
                            cmd.ExecuteNonQuery();

                        }
                        con.Close();


                    }
                    //getFranchisess to save to save in a CallLogAssociate
                    //CallLogAssociate objCallLogAssociate;
                    //List<CallLogAssociate> lstCallLogAssociate = new List<CallLogAssociate>();
                    //var franchisees = context.Franchisees.AsQueryable().Where(x => collectionCallLogModel.arrFranchiseIds.Contains(x.FranchiseeId)).Select(x => x);
                    //if (franchisees != null)
                    //{
                    //    foreach (var franchise in franchisees)
                    //    {
                    //        objCallLogAssociate = new CallLogAssociate();
                    //        objCallLogAssociate.ClassId = franchise.FranchiseeId;
                    //        objCallLogAssociate.TypeListId = (int)Business.Enumeration.TypeList.Franachisee;
                    //        objCallLogAssociate.CallLogAssociateTypeListId = (int)Business.Enumeration.CallLogAssociateTypeList.CollectionCallLog;
                    //        objCallLogAssociate.RegionId = franchise.RegionId;
                    //        objCallLogAssociate.CreatedBy = -1;
                    //        objCallLogAssociate.CreatedDate = DateTime.Now;
                    //        lstCallLogAssociate.Add(objCallLogAssociate);
                    //    }
                    //}
                    //if (lstCallLogAssociate.Any())
                    //{
                    //    context.CallLogAssociates.AddRange(lstCallLogAssociate);
                    //    context.SaveChanges();
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerRevenuesResultViewModel> GetCustomerRevenuesReportData(string regionId, int periodid)
        {
            List<CustomerRevenuesResultViewModel> lstCustomerRevenue = new List<CustomerRevenuesResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@PeriodId", periodid);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_RevenueReportDetailsDataForCustomer", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerRevenue = multipleresult.Read<CustomerRevenuesResultViewModel>().ToList();
                    }
                }
            }
            return lstCustomerRevenue;
        }

        #endregion

        public bool SaveFranchiseeDistribution(List<FranchiseeDistribution> lstFranchiseeDistribution)
        {
            try
            {
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    foreach (FranchiseeDistribution oD in lstFranchiseeDistribution)
                    {
                        JKApi.Data.DAL.Distribution oDistribution = context.Distributions.SingleOrDefault(o => o.ContractDetailId == oD.ContractDetailId && o.FranchiseeId == oD.FranchiseeId && o.CustomerId == oD.CustomerId);
                        if (oDistribution != null)
                        {
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = LoginUserId;
                            oDistribution.CreatedDate = DateTime.Now;
                        }
                        else
                        {
                            oDistribution = new Data.DAL.Distribution();
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = LoginUserId;
                            oDistribution.CreatedDate = DateTime.Now;
                            context.Distributions.Add(oDistribution);
                        }
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveFranchiseeDistributionFee(List<FranchiseeDistributionFeesViewModel> lstFranchiseeDistributionFee)
        {
            try
            {
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    JKApi.Data.DAL.Distribution oDistribution = new Data.DAL.Distribution();
                    foreach (FranchiseeDistributionFeesViewModel oD in lstFranchiseeDistributionFee)
                    {
                        oDistribution = context.Distributions.SingleOrDefault(o => o.FranchiseeId == oD.FranchiseeId && o.ContractDetailId == oD.ContractDetailId);

                        if (oDistribution != null)
                        {
                            oDistribution.CustomerId = oDistribution.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = -1;
                            oDistribution.CreatedDate = DateTime.Now;
                            context.SaveChanges();
                        }
                        else
                        {
                            oDistribution = new Data.DAL.Distribution();
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = -1;
                            oDistribution.CreatedDate = DateTime.Now;
                            context.Distributions.Add(oDistribution);
                            context.SaveChanges();
                        }





                        JKApi.Data.DAL.DistributionFee oDistributionFee = context.DistributionFees.SingleOrDefault(o => o.FeeId == oD.FeeId.ToString() && o.DistributionId == oDistribution.DistributionId);
                        if (oDistributionFee != null)
                        {
                            oDistributionFee.Amount = oD.Amount;
                        }
                        else
                        {
                            oDistributionFee = new Data.DAL.DistributionFee();
                            oDistributionFee.Amount = oD.Amount;
                            oDistributionFee.CreatedBy = LoginUserId;
                            oDistributionFee.CreatedDate = DateTime.Now;
                            oDistributionFee.DistributionId = oDistribution.DistributionId;
                            oDistributionFee.FeeId = oD.FeeId.ToString();
                            oDistributionFee.FeeRateTypeListId = oD.FeeRateTypeListId;
                            oDistributionFee.IsActive = true;
                            context.DistributionFees.Add(oDistributionFee);
                        }
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void savePendingMessage(string message, int customerID, int status)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.SpInsertPendingData(int.Parse(ClaimView.GetCLAIM_USERID()), message, status == 2 ? true : false, status == 1 ? true : false, customerID, DateTime.Now, "Customer", null
                    , null, null, null, null, null);
            }
        }

        public bool SaveFranchiseeDetailsDistribution(List<FranchiseeDistribution> lstFranchiseeDistribution)
        {
            try
            {

                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    foreach (FranchiseeDistribution oD in lstFranchiseeDistribution)
                    {
                        JKApi.Data.DAL.Distribution oDistribution = context.Distributions.SingleOrDefault(o => o.ContractDetailId == oD.ContractDetailId && o.FranchiseeId == oD.FranchiseeId);

                        if (oDistribution != null)
                        {
                            oDistribution.CustomerId = oDistribution.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            oDistribution.CreatedDate = DateTime.Now;
                        }
                        else
                        {
                            oDistribution = new Data.DAL.Distribution();
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.Amount = oD.Amount;
                            oDistribution.FranchiseeId = oD.FranchiseeId;
                            oDistribution.DetailLineNumber = oD.DetailLineNumber;
                            oDistribution.ContractDetailId = oD.ContractDetailId;
                            oDistribution.CustomerId = oD.CustomerId;
                            oDistribution.isActive = true;
                            oDistribution.StartDate = DateTime.Now;
                            oDistribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            oDistribution.CreatedDate = DateTime.Now;
                            context.Distributions.Add(oDistribution);
                        }
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetStateName(int stateid)
        {
            var statename = Uow.StateList.GetAll().Where(x => x.StateListId == stateid).Select(x => new { x.abbr }).FirstOrDefault();
            return statename.ToString();
        }

        public List<Address> GetAddressList()
        {
            List<JKApi.Data.DAL.Address> lstAddress = new List<Address>();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                lstAddress = context.Addresses.AsNoTracking().Where(a => ((a.ContactTypeListId == 3 || a.ContactTypeListId == 1) && (a.TypeListId == 1 || a.TypeListId == 2))).ToList();
                //context.Configuration.ProxyCreationEnabled = true;
                return lstAddress;
            }
        }

        public List<portal_spGet_AP_TaxRateAPI_Result> GetTaxrateList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                List<JKApi.Data.DAL.portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = context.portal_spGet_AP_TaxRateAPI().ToList();
                return lstTaxRate;
            }
        }

        public List<Address> GetAddressList(int classId, int typelistId)
        {
            List<JKApi.Data.DAL.Address> lstAddress = new List<Address>();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                lstAddress = context.Addresses.AsNoTracking().Where(a => a.ClassId == classId && a.TypeListId == typelistId && ((a.ContactTypeListId == 3 || a.ContactTypeListId == 1))).ToList();
                //context.Configuration.ProxyCreationEnabled = true;
                return lstAddress;
            }
        }
        public List<portal_spGet_AP_TaxRateAPI_Result> GetTaxrateList(int classId, int typelistId)
        {
            List<JKApi.Data.DAL.portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = new List<portal_spGet_AP_TaxRateAPI_Result>();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var result = (from a in context.TaxRates
                              join b in context.Addresses on a.AddressId equals b.AddressId
                              where b.ClassId == classId && b.TypeListId == typelistId && !String.IsNullOrEmpty(b.PostalCode.ToString())
                              select new
                              {
                                  AddressId = a.AddressId,
                                  PostalCode = b.PostalCode.ToString().Replace("-", ""),
                                  TaxRateId = a.TaxRateId,
                                  City = b.City,
                                  StateName = b.StateName,
                                  RegionId = a.RegionId
                              }).ToList();

                portal_spGet_AP_TaxRateAPI_Result oInsert;
                foreach (var item in result)
                {
                    oInsert = new portal_spGet_AP_TaxRateAPI_Result();
                    oInsert.AddressId = item.AddressId;
                    oInsert.PostalCode = item.PostalCode;
                    oInsert.TaxRateId = item.TaxRateId;
                    oInsert.City = item.City;
                    oInsert.StateName = item.StateName;
                    oInsert.RegionId = item.RegionId;

                    lstTaxRate.Add(oInsert);
                }
                return lstTaxRate;
            }
        }

        public List<ARInvoiceListViewModel> GetInvoiceListWithSearchForPayment(int customerId, string OpenClose)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                // todo: make new SP to filter invoices properly for payment
                List<portal_spGet_AR_InvoicePaymentSearchList_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoicePaymentSearchList(customerId, SelectedRegionId, OpenClose).ToList();

                foreach (portal_spGet_AR_InvoicePaymentSearchList_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId,
                        HasMultipleLineItems = (o.HasMultipleLineItems == 1)
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public List<ServiceCallLog> GetServiceLog(int id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                List<ServiceCallLog> list = context.ServiceCallLogs.AsNoTracking().Where(x => x.ClassId == id).ToList();
                //context.Configuration.ProxyCreationEnabled = true;
                return list;
            }
        }
        public ServiceCallLog GetServiceCallLogById(int Id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                ServiceCallLog model = context.ServiceCallLogs.AsNoTracking().Where(x => x.ServiceCallLogId == Id).FirstOrDefault();
                return model;
            }
        }

        public List<CollectionsCallLog> GetCollectionLog(int id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                List<CollectionsCallLog> list = context.CollectionsCallLogs.AsNoTracking().Where(x => x.ClassId == id).ToList();
                //context.Configuration.ProxyCreationEnabled = true;
                return list;
            }
        }

        public string GetStatus(int? id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                var data = context.StatusReasonLists.AsNoTracking().Where(x => x.StatusReasonListId == id).Select(x => x.Name).FirstOrDefault();
                //context.Configuration.ProxyCreationEnabled = true;
                return data;
            }
        }

        public List<portal_spGet_C_CancellationInvoiceList_Result> GetCancellationInvoiceList(int customerId, int month, int year)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_C_CancellationInvoiceList(customerId, month, year).ToList();
            }
        }

        public bool InsertUpdateCustomerMaintenance(CustomerMaintenanceViewModel inputData)
        {
            bool retVal = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<Status> lstStatus = context.Status.Where(o => o.ClassId == inputData.CustomerId && o.TypeListId == 1).ToList();
                foreach (Status ob in lstStatus)
                {
                    ob.IsActive = false;
                }
                context.SaveChanges();

                Status oStatus = new Status();
                oStatus.ClassId = inputData.CustomerId;
                oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                oStatus.TypeListId = 1;
                oStatus.StatusListId = inputData.StatusListId;
                oStatus.StatusNotes = inputData.StatusNotes;
                oStatus.StatusDate = inputData.StatusDate;
                oStatus.IsActive = true;
                oStatus.LastServiceDate = inputData.LastServiceDate;
                oStatus.ReasonListId = inputData.ReasonListId;
                oStatus.ResumeDate = inputData.ResumeDate;
                context.Status.Add(oStatus);
                context.SaveChanges();

                int statusid = oStatus.StatusId;
                Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(o => o.CustomerId == inputData.CustomerId);
                if (oCustomer != null)
                {
                    oCustomer.StatusId = statusid;
                    oCustomer.StatusListId = inputData.StatusListId;
                    context.SaveChanges();
                }

                if (inputData.StatusListId == 2)
                {
                    if (inputData.FindersFee.Count > 0)
                    {
                        List<FindersFee> lstFindersFee = context.FindersFees.Where(o => o.CustomerId == inputData.CustomerId && o.IsActive == true).ToList();
                        foreach (FindersFee ob in lstFindersFee)
                        {
                            if (ob.FindersFeeId > 0)
                            {
                                CustomerStatusFindersFeeViewModel oF = inputData.FindersFee.SingleOrDefault(k => k.FindersFeeId == ob.FindersFeeId);
                                //ob.StatusDate = DateTime.Now;
                                //ob.StopDate = oF.FindersFeeStopDate;
                                //ob.ActionStatusListId = 7;

                                if (oF.FindersFeeHasCancellationFee == true)
                                {
                                    context.portal_spCreate_C_FinderFeeBillGenerateFirenchisee(SelectedRegionId, SelectedUserId, ob.FindersFeeId, true, inputData.CustomerId);
                                }
                                context.SaveChanges();
                            }

                        }

                    }
                }
                else if (inputData.StatusListId == 3)
                {
                    List<FindersFee> lstFindersFee = context.FindersFees.Where(o => o.CustomerId == inputData.CustomerId && o.IsActive == true).ToList();
                    foreach (FindersFee ob in lstFindersFee)
                    {
                        //ob.StatusDate = DateTime.Now;
                        //ob.ActionStatusListId = 7;
                        ob.ResumeDate = inputData.ResumeDate;
                    }
                    context.SaveChanges();
                }
                foreach (CustomerStatusFindersFeeViewModel ob in inputData.FindersFee.Where(f => f.FindersFeeHasCancellationFee == true && f.FindersFeeId > 0))
                {
                    context.portal_spCreate_C_FinderFeeBillGenerateFirenchisee(SelectedRegionId, SelectedUserId, ob.FindersFeeId, true, inputData.CustomerId);
                }


            }



            return retVal;

        }

        public int InsertUpdateCustomerMaintenanceTemp(CustomerMaintenanceViewModel inputData)
        {
            int retVal = 0;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                List<MaintenanceTemp> lstMaintenanceTemp = context.MaintenanceTemps.Where(o => o.ClassId == inputData.CustomerId && o.TypeListId == 1).ToList();

                foreach (MaintenanceTemp ob in lstMaintenanceTemp)
                {
                    ob.IsActive = false;
                }
                context.SaveChanges();


                MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                oMaintenanceTemp.ClassId = inputData.CustomerId;
                oMaintenanceTemp.TypeListId = 1;
                oMaintenanceTemp.CreatedBy = LoginUserId;
                oMaintenanceTemp.CreatedDate = DateTime.Now;
                oMaintenanceTemp.ResumeDate = inputData.ResumeDate;
                oMaintenanceTemp.Comments = inputData.StatusNotes;
                oMaintenanceTemp.EffectiveDate = inputData.StatusDate;
                oMaintenanceTemp.IsActive = true;
                oMaintenanceTemp.LastServiceDate = inputData.LastServiceDate;
                oMaintenanceTemp.ModifiedBy = LoginUserId;
                oMaintenanceTemp.ModifiedDate = DateTime.Now;
                oMaintenanceTemp.RegionId = SelectedRegionId;
                oMaintenanceTemp.RequestChangeNotes = null;
                oMaintenanceTemp.RequestChangeStatusListId = 1;
                oMaintenanceTemp.StatusListId = inputData.StatusListId;
                oMaintenanceTemp.StatusReasonListId = inputData.ReasonListId;
                oMaintenanceTemp.MaintenanceStatusListId = 1;
                oMaintenanceTemp.MaintenanceTypeListId = inputData.StatusListId;//Cancel
                oMaintenanceTemp.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());


                JKApi.Data.DAL.Customer customer = context.Customers.Where(x => x.CustomerId == inputData.CustomerId).FirstOrDefault();
                if (customer != null) oMaintenanceTemp.RegionId = customer.RegionId;

                JKApi.Data.DAL.Period period = context.Periods.Where(x => x.BillMonth == inputData.StatusDate.Value.Month && x.BillYear == inputData.StatusDate.Value.Year).FirstOrDefault();
                if (period != null) oMaintenanceTemp.PeriodId = period.PeriodId;



                context.MaintenanceTemps.Add(oMaintenanceTemp);
                context.SaveChanges();


                retVal = oMaintenanceTemp.MaintenanceTempId;

                //if (inputData.StatusListId == 1)//For Customer Active Again
                //{
                //    context.Portal_sp_Customer_ReActivate(inputData.CustomerId, LoginUserId);
                //}
                //else if (inputData.StatusListId == 16)//For Customer Suspended
                //{
                //    context.Portal_sp_Customer_Suspended(inputData.CustomerId, LoginUserId);
                //}

                //List<Status> lstStatus = context.Status.Where(o => o.ClassId == inputData.CustomerId && o.TypeListId == 1).ToList();
                //foreach (Status ob in lstStatus)
                //{
                //    ob.IsActive = false;
                //}
                //context.SaveChanges();

                //Status oStatus = new Status();
                //oStatus.ClassId = inputData.CustomerId;
                //oStatus.CreatedBy = SelectedUserId.ToString();
                //oStatus.TypeListId = 1;
                //oStatus.StatusListId = inputData.StatusListId;
                //oStatus.StatusNotes = inputData.StatusNotes;
                //oStatus.StatusDate = inputData.StatusDate;
                //oStatus.IsActive = true;
                //oStatus.LastServiceDate = inputData.LastServiceDate;
                //oStatus.ReasonListId = inputData.ReasonListId;
                //oStatus.ResumeDate = inputData.ResumeDate;
                //context.Status.Add(oStatus);
                //context.SaveChanges();

                //int statusid = oStatus.StatusId;
                //Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(o => o.CustomerId == inputData.CustomerId);
                //if (oCustomer != null)
                //{
                //    oCustomer.StatusId = statusid;
                //    context.SaveChanges();
                //}

                if (inputData.StatusListId == 10 && inputData.FindersFee != null)
                {
                    if (inputData.FindersFee.Count > 0)
                    {

                        if (inputData.FindersFee[0].FindersFeeId == -1 && inputData.FindersFee[0].FindersFeeHasCancellationFee == true)
                        {
                            List<Data.DAL.Distribution> lstDist = context.Distributions.Where(o => o.CustomerId == inputData.CustomerId && o.isActive == true).ToList();
                            foreach (var ob in lstDist)
                            {

                                MaintenanceTempDetail oMaintenanceDetailTemp = new MaintenanceTempDetail();
                                oMaintenanceDetailTemp.FFIsCancellationFee = true;
                                oMaintenanceDetailTemp.FFCancellationFeeAmount = 50;
                                oMaintenanceDetailTemp.CustomerId = inputData.CustomerId;
                                oMaintenanceDetailTemp.DFranchiseeId = ob.FranchiseeId;

                                //oMaintenanceDetailTemp.IsActive = true;
                                //oMaintenanceDetailTemp.LineNumber = 0;
                                oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                                oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                                //oMaintenanceDetailTemp.RegionId = SelectedRegionId;
                                //oMaintenanceDetailTemp.ServiceTypeListId = 0;
                                //oMaintenanceDetailTemp.TypeListId = 2;
                                oMaintenanceDetailTemp.FindersFeeId = -1;
                                oMaintenanceDetailTemp.FFStopDate = DateTime.Now;
                                oMaintenanceDetailTemp.MaintenanceDetailTypeListId = 5;
                                oMaintenanceDetailTemp.RecordType = "CANCELLATION";
                                oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                                oMaintenanceDetailTemp.CreatedDate = DateTime.Now;

                                context.MaintenanceTempDetails.Add(oMaintenanceDetailTemp);
                                context.SaveChanges();
                            }

                        }
                        else
                        {

                            List<FindersFee> lstFindersFee = context.FindersFees.Where(o => o.CustomerId == inputData.CustomerId && o.IsActive == true).ToList();
                            foreach (FindersFee ob in lstFindersFee)
                            {
                                CustomerStatusFindersFeeViewModel oF = inputData.FindersFee.FirstOrDefault(k => k.FindersFeeId == ob.FindersFeeId);
                                if (oF == null) continue;

                                MaintenanceTempDetail oMaintenanceDetailTemp = new MaintenanceTempDetail();
                                oMaintenanceDetailTemp.FFIsCancellationFee = oF.FindersFeeHasCancellationFee;
                                oMaintenanceDetailTemp.FFCancellationFeeAmount = 50;
                                oMaintenanceDetailTemp.CustomerId = inputData.CustomerId;
                                oMaintenanceDetailTemp.DFranchiseeId = ob.FranchiseeId;

                                //oMaintenanceDetailTemp.IsActive = true;
                                //oMaintenanceDetailTemp.LineNumber = 0;
                                oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                                oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                                //oMaintenanceDetailTemp.RegionId = SelectedRegionId;
                                //oMaintenanceDetailTemp.ServiceTypeListId = 0;
                                //oMaintenanceDetailTemp.TypeListId = 2;
                                oMaintenanceDetailTemp.FindersFeeId = ob.FindersFeeId;
                                oMaintenanceDetailTemp.FFStopDate = DateTime.Now;
                                oMaintenanceDetailTemp.MaintenanceDetailTypeListId = 5;
                                oMaintenanceDetailTemp.RecordType = "CANCELLATION";
                                oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                                oMaintenanceDetailTemp.CreatedDate = DateTime.Now;

                                context.MaintenanceTempDetails.Add(oMaintenanceDetailTemp);
                                context.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    //else if (inputData.StatusListId == 3)
                    //{
                    //    List<FindersFee> lstFindersFee = context.FindersFees.Where(o => o.CustomerId == inputData.CustomerId).ToList();
                    //    foreach (FindersFee ob in lstFindersFee)
                    //    {
                    //        ob.StatusDate = DateTime.Now;
                    //        ob.ActionStatusListId = 7;
                    //        ob.ResumeDate = inputData.ResumeDate;
                    //    }
                    //    context.SaveChanges();
                    //}
                    if (inputData.FindersFee != null)
                    {
                        foreach (CustomerStatusFindersFeeViewModel ob in inputData.FindersFee.Where(f => f.FindersFeeHasCancellationFee == true))
                        {
                            MaintenanceTempDetail oMaintenanceDetailTemp = new MaintenanceTempDetail();
                            oMaintenanceDetailTemp.FFIsCancellationFee = ob.FindersFeeHasCancellationFee;
                            oMaintenanceDetailTemp.FFCancellationFeeAmount = 50;
                            oMaintenanceDetailTemp.CustomerId = inputData.CustomerId;
                            oMaintenanceDetailTemp.DFranchiseeId = ob.FranchiseeId;

                            //oMaintenanceDetailTemp.IsActive = true;
                            //oMaintenanceDetailTemp.LineNumber = 0;
                            oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                            oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                            oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                            //oMaintenanceDetailTemp.RegionId = SelectedRegionId;
                            //oMaintenanceDetailTemp.ServiceTypeListId = 0;
                            //oMaintenanceDetailTemp.TypeListId = 2;
                            oMaintenanceDetailTemp.FindersFeeId = ob.FindersFeeId;
                            oMaintenanceDetailTemp.FFStopDate = DateTime.Now;
                            oMaintenanceDetailTemp.MaintenanceDetailTypeListId = (int)inputData.StatusListId;
                            oMaintenanceDetailTemp.RecordType = inputData.StatusListId == 10 ? "CANCELLATION" : (inputData.StatusListId == 16 ? "SUSPENDED" : "");
                            oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                            oMaintenanceDetailTemp.CreatedDate = DateTime.Now;

                            context.MaintenanceTempDetails.Add(oMaintenanceDetailTemp);


                            //MaintenanceDetailTemp oMaintenanceDetailTemp = new MaintenanceDetailTemp();
                            //oMaintenanceDetailTemp.CancellationFee = true;
                            //oMaintenanceDetailTemp.ClassId = ob.FranchiseeId;
                            //oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                            //oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                            //oMaintenanceDetailTemp.IsActive = true;
                            //oMaintenanceDetailTemp.LineNumber = ob.LineNumber;
                            //oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                            //oMaintenanceDetailTemp.ModifiedBy = LoginUserId;
                            //oMaintenanceDetailTemp.ModifiedDate = DateTime.Now;
                            //oMaintenanceDetailTemp.RegionId = SelectedRegionId;
                            //oMaintenanceDetailTemp.ServiceTypeListId = ob.ServiceTypeListId;
                            //oMaintenanceDetailTemp.TypeListId = 2;
                            //oMaintenanceDetailTemp.FindersFeeId = ob.FindersFeeId;
                            //oMaintenanceDetailTemp.FindersFeeStopDate = DateTime.Now;
                            //context.MaintenanceDetailTemps.Add(oMaintenanceDetailTemp);
                            context.SaveChanges();
                        }
                    }

                }
            }



            return retVal;

        }

        #region :: Franchise Distribution :: 

        public CustomerDistributionDetailsModel GetCustomerDistributionDetails(int CustomerId)
        {
            using (var context = new jkDatabaseEntities())
            {
                CustomerDistributionDetailsModel CustomerDistributionModel = new CustomerDistributionDetailsModel();
                var CustomerData = context.Customers.Where(w => w.CustomerId == CustomerId).FirstOrDefault();

                CustomerDistributionModel.CustomerNo = (CustomerData != null ? CustomerData.CustomerNo : string.Empty);
                CustomerDistributionModel.CustomerName = (CustomerData != null ? CustomerData.Name : string.Empty);

                var result = (from dst in context.Distributions
                              join cnt in context.ContractDetails on dst.ContractDetailId equals cnt.ContractDetailId
                              join ct in context.Franchisees on dst.FranchiseeId equals ct.FranchiseeId
                              join st in context.ServiceTypeLists on cnt.ServiceTypeListId equals st.ServiceTypeListid
                              where dst.CustomerId == CustomerId && dst.isActive == true
                              select new CustomerDistributionModel
                              {
                                  FranchiseName = ct.Name,
                                  FranchiseNo = ct.FranchiseeNo,
                                  ServiceType = st.name,
                                  DistributionAmount = (dst.Amount.HasValue ? dst.Amount.Value : 0)
                              }).Distinct().ToList();
                if (result != null && result.Count() > 0)
                {
                    CustomerDistributionModel.CustomerDistributionList = result;
                    CustomerDistributionModel.TotalDistribution = Convert.ToDecimal(result.Sum(d => d.DistributionAmount));
                }

                return CustomerDistributionModel;
            }
        }


        public Data.DAL.Distribution GetCustomerDistribution(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.Distributions.FirstOrDefault(d => d.CustomerId == CustomerId);
            }

        }
        public List<Data.DAL.Distribution> GetCustomerDistributionList(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.Distributions.Where(d => d.CustomerId == CustomerId && d.isActive == true).OrderByDescending(o => o.DistributionId).ToList();
            }
        }

        public FCFindersFeeViewModel GetFindersFeewithCustomerId(int CustomerId, int DistributionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.FindersFees.FirstOrDefault(d => d.CustomerId == CustomerId && d.DistributionId == DistributionId).ToModel<FCFindersFeeViewModel, JKApi.Data.DAL.FindersFee>();
            }
        }

        public int SaveDistribution(FranchiseeDistribution _FranchiseeDistribution)
        {
            Data.DAL.Distribution oDistribution;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                if (_FranchiseeDistribution.DistributionId > 0)
                {
                    oDistribution = context.Distributions.SingleOrDefault(d => d.DistributionId == _FranchiseeDistribution.DistributionId);
                    oDistribution.Amount = _FranchiseeDistribution.Amount;
                    context.SaveChanges();
                }
                else
                {
                    oDistribution = new Data.DAL.Distribution();
                    oDistribution.Amount = _FranchiseeDistribution.Amount;
                    oDistribution.ContractDetailId = _FranchiseeDistribution.ContractDetailId;
                    oDistribution.ContractId = _FranchiseeDistribution.ContractId;
                    oDistribution.CreatedBy = LoginUserId;
                    oDistribution.CreatedDate = DateTime.Now;
                    oDistribution.CustomerId = _FranchiseeDistribution.CustomerId;
                    oDistribution.DetailLineNumber = _FranchiseeDistribution.DetailLineNumber;
                    oDistribution.StartDate = DateTime.Now;
                    context.Distributions.Add(oDistribution);
                    context.SaveChanges();
                }

            }



            return oDistribution.DistributionId;
        }

        #endregion 

        public List<FranchiseeFinderFeeDetailViewModel> GetCustomerFinderFeeDetailList(int regionid, int customerid)
        {
            using (var context = new jkDatabaseEntities())
            {

                List<FranchiseeFinderFeeDetailViewModel> lstFranchiseeFinderFeeDetailViewModel = new List<FranchiseeFinderFeeDetailViewModel>();
                if (regionid == 0)
                {
                    lstFranchiseeFinderFeeDetailViewModel = context.vw_F_FinderFeeDetailList.Where(o => o.CustomerId == customerid).MapEnumerable<FranchiseeFinderFeeDetailViewModel, vw_F_FinderFeeDetailList>().ToList();
                }
                else
                {
                    lstFranchiseeFinderFeeDetailViewModel = context.vw_F_FinderFeeDetailList.Where(o => o.RegionId == regionid && o.CustomerId == customerid).MapEnumerable<FranchiseeFinderFeeDetailViewModel, vw_F_FinderFeeDetailList>().ToList();
                }

                if (lstFranchiseeFinderFeeDetailViewModel.Count > 0)
                {
                    return lstFranchiseeFinderFeeDetailViewModel;
                }
                else
                {
                    FranchiseeFinderFeeDetailViewModel oFranchiseeFinderFeeDetailViewModel = new FranchiseeFinderFeeDetailViewModel();

                    Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(f => f.CustomerId == customerid);
                    if (oCustomer != null)
                    {
                        oFranchiseeFinderFeeDetailViewModel.CustomerId = oCustomer.CustomerId;
                        oFranchiseeFinderFeeDetailViewModel.CustomerNo = oCustomer.CustomerNo;
                        oFranchiseeFinderFeeDetailViewModel.CustomerName = oCustomer.Name;

                        oFranchiseeFinderFeeDetailViewModel.FranchiseeId = 0;
                        oFranchiseeFinderFeeDetailViewModel.TotalFinderFee = 0;
                        oFranchiseeFinderFeeDetailViewModel.TotalDownPayment = 0;
                        oFranchiseeFinderFeeDetailViewModel.MonthlyPaymentAmount = 0;
                        oFranchiseeFinderFeeDetailViewModel.PaidAmount = 0;
                        oFranchiseeFinderFeeDetailViewModel.TotalBalance = 0;

                        lstFranchiseeFinderFeeDetailViewModel.Add(oFranchiseeFinderFeeDetailViewModel);
                    }


                    return lstFranchiseeFinderFeeDetailViewModel;
                }

            }
        }

        public FFinderFeeDetailFullViewModel GetCCFinderFeeDetail(int regionid, int findersfeeid)
        {
            using (var context = new jkDatabaseEntities())
            {
                FFinderFeeDetailFullViewModel oFFinderFeeDetailFullViewModel = new FFinderFeeDetailFullViewModel();
                oFFinderFeeDetailFullViewModel = context.portal_spGet_F_GetFindersFeeDetailByFindersFeeId(findersfeeid, regionid).MapEnumerable<FFinderFeeDetailFullViewModel, portal_spGet_F_GetFindersFeeDetailByFindersFeeId_Result>().ToList().FirstOrDefault();
                oFFinderFeeDetailFullViewModel.FindersFeeAdjustments = context.FindersFeeAdjustments.Where(o => o.FindersFeeId == findersfeeid).MapEnumerable<FindersFeeAdjustmentViewModel, FindersFeeAdjustment>().ToList();

                return oFFinderFeeDetailFullViewModel;
            }
        }

        public List<FindersFeeAdjustmentTypeListViewModel> GetFindersFeeAdjustmentTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FindersFeeAdjustmentTypeLists.MapEnumerable<FindersFeeAdjustmentTypeListViewModel, FindersFeeAdjustmentTypeList>().ToList();

            }
        }


        public List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.CollectionsCallLogModel> CollectionCallLog = (from
                                                tbCollactionlog in context.CollectionsCallLogs
                                                                                         join tbStatus in context.StatusResultLists on tbCollactionlog.StatusResultListId equals tbStatus.StatusResultListId into tempJoin
                                                                                         from t2 in tempJoin.DefaultIfEmpty()
                                                                                         join tbcust in context.Customers on tbCollactionlog.ClassId equals tbcust.CustomerId
                                                                                         select new CollectionsCallLogModel
                                                                                         {
                                                                                             CustomerNo = tbcust.CustomerNo,
                                                                                             CallDate = tbCollactionlog.CallDate,
                                                                                             CallTime = tbCollactionlog.CallTime,
                                                                                             //status = tbStatus.Name,
                                                                                             status = t2.Name,
                                                                                             SpokeWith = tbCollactionlog.SpokeWith,
                                                                                             Action = tbCollactionlog.Action,
                                                                                             CallBack = tbCollactionlog.CallBack,
                                                                                             Comments = tbCollactionlog.Comments,
                                                                                             ClassId = tbCollactionlog.ClassId,
                                                                                             CreatedDate = tbCollactionlog.CreatedDate
                                                                                         })
                                                    .Where(x => x.ClassId == CustID && x.CallDate >= fromdate && x.CallDate <= todate)
                                                    .ToList();
                //
                return CollectionCallLog;
            }
        }

        public List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext, int statusid = 0)
        {
            //new code
            //List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLogListResult = new List<ServiceCallLogModel>();
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var parmas = new DynamicParameters();
            //    parmas.Add("@CustomerId", CustID);
            //    parmas.Add("@fromdate", fromdate);
            //    parmas.Add("@todate", todate);
            //    parmas.Add("@searchtext", searchtext);
            //    parmas.Add("@RegionIds", SelectedRegionId);

            //    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerServiceCallLogListResult", parmas, commandType: CommandType.StoredProcedure))
            //    {
            //        if (multipleresult != null)
            //        {
            //            ServiceCallLogListResult = multipleresult.Read<JKViewModels.Customer.ServiceCallLogModel>().ToList();
            //        }
            //    }
            //}
            //return ServiceCallLogListResult;

            #region :: Old Code ::  

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLog = (from tbcust in context.Customers
                                                                                  join tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                                                                  join tbScalllog in context.ServiceCallLogTypeLists on tbservicecalllog.ServiceLogTypeListId equals tbScalllog.ServiceCallLogTypeListId
                                                                                  join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId into tempJoin
                                                                                  from t2 in tempJoin.DefaultIfEmpty()
                                                                                  join tbluser in context.AuthUserLogins on tbservicecalllog.CreatedBy equals tbluser.UserId
                                                                                  into cutlog
                                                                                  from tbluser in cutlog.DefaultIfEmpty()

                                                                                  join tblFollowUpBy in context.AuthUserLogins on tbservicecalllog.FollowUpBy equals tblFollowUpBy.UserId
                                                                                  into FollowUpBy
                                                                                  from tblFollowUpBy in FollowUpBy.DefaultIfEmpty()

                                                                                  join tblStatusList in context.StatusLists on tbservicecalllog.StatusListId equals tblStatusList.StatusListId
                                                                                  into StatusListName
                                                                                  from tblStatusList in StatusListName.DefaultIfEmpty()

                                                                                  select new ServiceCallLogModel
                                                                                  {
                                                                                      ServiceCallLogId = tbservicecalllog.ServiceCallLogId,
                                                                                      CustomerNo = tbcust.CustomerNo,
                                                                                      CallDate = tbservicecalllog.CallDate,
                                                                                      CallTime = tbservicecalllog.CallTime.ToString(),
                                                                                      Status = t2.Name,
                                                                                      SpokeWith = tbservicecalllog.SpokeWith,
                                                                                      Action = tbservicecalllog.Action,
                                                                                      CallBack = tbservicecalllog.CallBack,
                                                                                      Comments = tbservicecalllog.Comments,
                                                                                      ClassId = tbservicecalllog.ClassId,
                                                                                      ServiceLogTypeListName = tbScalllog.Name,
                                                                                      InitiatedBy = tbservicecalllog.InitiatedById,
                                                                                      CreatedByName = tbluser.FirstName + " " + tbluser.LastName,
                                                                                      FollowUpByName = tblFollowUpBy.FirstName + " " + tblFollowUpBy.LastName,
                                                                                      StatusListName = tblStatusList.Name,
                                                                                      StatusListId = tbservicecalllog.StatusListId,
                                                                                      EmailNotesTo = tbservicecalllog.EmailNotesTo,
                                                                                  }).Where(x => x.ClassId == CustID && x.CallDate >= fromdate && x.CallDate <= todate && (statusid == 0 || x.StatusListId == statusid)).OrderByDescending(o => o.CallDate).ToList();
                return ServiceCallLog;
            }
            #endregion
        }

        public List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext, string statusid)
        {
            //new code
            //List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLogListResult = new List<ServiceCallLogModel>();
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var parmas = new DynamicParameters();
            //    parmas.Add("@CustomerId", CustID);
            //    parmas.Add("@fromdate", fromdate);
            //    parmas.Add("@todate", todate);
            //    parmas.Add("@searchtext", searchtext);
            //    parmas.Add("@RegionIds", SelectedRegionId);

            //    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerServiceCallLogListResult", parmas, commandType: CommandType.StoredProcedure))
            //    {
            //        if (multipleresult != null)
            //        {
            //            ServiceCallLogListResult = multipleresult.Read<JKViewModels.Customer.ServiceCallLogModel>().ToList();
            //        }
            //    }
            //}
            //return ServiceCallLogListResult;

            #region :: Old Code ::  

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLog = (from tbcust in context.Customers
                                                                                  join tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                                                                  join tbScalllog in context.ServiceCallLogTypeLists on tbservicecalllog.ServiceLogTypeListId equals tbScalllog.ServiceCallLogTypeListId
                                                                                  join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId into tempJoin
                                                                                  from t2 in tempJoin.DefaultIfEmpty()
                                                                                  join tbluser in context.AuthUserLogins on tbservicecalllog.CreatedBy equals tbluser.UserId
                                                                                  into cutlog
                                                                                  from tbluser in cutlog.DefaultIfEmpty()

                                                                                  join tblFollowUpBy in context.AuthUserLogins on tbservicecalllog.FollowUpBy equals tblFollowUpBy.UserId
                                                                                  into FollowUpBy
                                                                                  from tblFollowUpBy in FollowUpBy.DefaultIfEmpty()

                                                                                  join tblStatusList in context.StatusLists on tbservicecalllog.StatusListId equals tblStatusList.StatusListId
                                                                                  into StatusListName
                                                                                  from tblStatusList in StatusListName.DefaultIfEmpty()

                                                                                  select new ServiceCallLogModel
                                                                                  {
                                                                                      ServiceCallLogId = tbservicecalllog.ServiceCallLogId,
                                                                                      CustomerNo = tbcust.CustomerNo,
                                                                                      CallDate = tbservicecalllog.CallDate,
                                                                                      CallTime = tbservicecalllog.CallTime.ToString(),
                                                                                      Status = t2.Name,
                                                                                      SpokeWith = tbservicecalllog.SpokeWith,
                                                                                      Action = tbservicecalllog.Action,
                                                                                      CallBack = tbservicecalllog.CallBack,
                                                                                      Comments = tbservicecalllog.Comments,
                                                                                      ClassId = tbservicecalllog.ClassId,
                                                                                      ServiceLogTypeListName = tbScalllog.Name,
                                                                                      InitiatedBy = tbservicecalllog.InitiatedById,
                                                                                      CreatedByName = tbluser.FirstName + " " + tbluser.LastName,
                                                                                      FollowUpByName = tblFollowUpBy.FirstName + " " + tblFollowUpBy.LastName,
                                                                                      StatusListName = tblStatusList.Name,
                                                                                      StatusListId = tbservicecalllog.StatusListId,
                                                                                      EmailNotesTo = tbservicecalllog.EmailNotesTo,
                                                                                  }).Where(x => x.ClassId == CustID && x.CallDate >= fromdate && x.CallDate <= todate && (statusid == "" || statusid.Contains(x.StatusListId.ToString()))).OrderByDescending(o => o.CallDate).ToList();
                return ServiceCallLog;
            }
            #endregion
        }

        public JKViewModels.Customer.ServiceCallLogModel GetServiceCallLogCustomersListForSearch(int ServiceCallLogId)
        {
            ServiceCallLogModel ServiceCallLogModel = new ServiceCallLogModel();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var ServiceCallLog = (from tbcust in context.Customers
                                      join tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                      where tbservicecalllog.ServiceCallLogId == ServiceCallLogId
                                      join tbScalllog in context.ServiceCallLogTypeLists on tbservicecalllog.ServiceLogTypeListId equals tbScalllog.ServiceCallLogTypeListId
                                      join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId into tempJoin
                                      from t2 in tempJoin.DefaultIfEmpty()
                                      join tbluser in context.AuthUserLogins on tbservicecalllog.CreatedBy equals tbluser.UserId
                                      into cutlog
                                      from tbluser in cutlog.DefaultIfEmpty()
                                      select new ServiceCallLogModel
                                      {
                                          ServiceCallLogId = tbservicecalllog.ServiceCallLogId,
                                          CustomerNo = tbcust.CustomerNo,
                                          CallDate = tbservicecalllog.CallDate,
                                          CallTime = tbservicecalllog.CallTime.ToString(),
                                          Status = t2.Name,
                                          SpokeWith = tbservicecalllog.SpokeWith,
                                          Action = tbservicecalllog.Action,
                                          CallBack = tbservicecalllog.CallBack,
                                          Comments = tbservicecalllog.Comments,
                                          ClassId = tbservicecalllog.ClassId,
                                          ServiceLogTypeListName = tbScalllog.Name,
                                          InitiatedBy = tbservicecalllog.InitiatedById,
                                          CreatedByName = tbluser.FirstName + " " + tbluser.LastName,
                                          Internal = tbservicecalllog.Internal,
                                          IsCallBack = ((tbservicecalllog.IsCallBack == null || tbservicecalllog.IsCallBack == false) ? false : true),
                                          ServiceLogTypeListId = tbservicecalllog.ServiceLogTypeListId,
                                          ServiceLogAreaListId = tbservicecalllog.ServiceLogAreaListId,
                                          StatusResultListId = tbservicecalllog.StatusResultListId,
                                          FollowUpBy = tbservicecalllog.FollowUpBy,
                                          EmailNotesTo = tbservicecalllog.EmailNotesTo,
                                          StatusListId = tbservicecalllog.StatusListId
                                      });
                if (ServiceCallLog != null)
                {
                    ServiceCallLogModel = ServiceCallLog.FirstOrDefault();
                }
                return ServiceCallLogModel;
            }
        }

        public List<JKViewModels.Customer.ServiceCallListViewModel> GetServiceCallListForSearch(string regionId, DateTime fromdate, DateTime todate, string Statusids, string TypeIds, int InitiatedBy = 0, int CreatedBy = 0, int IsCallBack = 0)
        {

            List<ServiceCallListViewModel> lstServiceCallList = new List<ServiceCallListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                if (string.IsNullOrEmpty(regionId) || regionId == "null")
                {
                    regionId = "0";
                }
                if (string.IsNullOrEmpty(Statusids) || Statusids == "null")
                {
                    Statusids = "0";
                }
                if (string.IsNullOrEmpty(TypeIds) || TypeIds == "null")
                {
                    TypeIds = "0";
                }

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StartDate", fromdate);
                parmas.Add("@EndDate", todate);
                parmas.Add("@StatusId", Statusids);
                parmas.Add("@TypeId", TypeIds);
                parmas.Add("@InitiatedBy", InitiatedBy);
                parmas.Add("@CreatedBy", CreatedBy);
                parmas.Add("@IsCallBack", IsCallBack);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_GetServiceCallList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstServiceCallList = multipleresult.Read<ServiceCallListViewModel>().ToList();
                    }
                }
            }
            return lstServiceCallList;



            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLog = 
            //        (from tbcust in context.Customers join tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
            //         join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId
            //         join tbScalllog in context.ServiceCallLogTypeLists on tbservicecalllog.ServiceLogTypeListId equals tbScalllog.ServiceCallLogTypeListId
            //         select new ServiceCallLogModel
            //         {
            //             CustomerNo = tbcust.CustomerNo,
            //             CustomerName = tbcust.Name,
            //             CallDate = tbservicecalllog.CallDate,
            //             CallTime = tbservicecalllog.CallTime.ToString(),
            //             Status = tbStatus.Name,
            //             SpokeWith = tbservicecalllog.SpokeWith,
            //             Action = tbservicecalllog.Action,
            //             CallBack = tbservicecalllog.CallBack,
            //             Comments = tbservicecalllog.Comments,
            //             ClassId = tbservicecalllog.ClassId,
            //             ServiceLogTypeListName = tbScalllog.Name,
            //             InitiatedBy = tbservicecalllog.InitiatedById
            //         }).Where(x => x.CallDate >= fromdate && x.CallDate <= todate).ToList();
            //    return ServiceCallLog;
            //}
        }

        public List<JKViewModels.Customer.ServiceCallListViewModel> GetServiceCallbackListForSearch(string regionId, DateTime fromdate, DateTime todate, string Statusids, string TypeIds, int InitiatedBy = 0, int CreatedBy = 0, int IsCallBack = 0, string fltdate = "", string ServiceStatusList = "")
        {

            List<ServiceCallListViewModel> lstServiceCallList = new List<ServiceCallListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StartDate", fromdate);
                parmas.Add("@EndDate", todate);
                parmas.Add("@StatusId", Statusids);
                parmas.Add("@TypeId", TypeIds);
                parmas.Add("@InitiatedBy", InitiatedBy);
                parmas.Add("@CreatedBy", CreatedBy);
                parmas.Add("@IsCallBack", IsCallBack);
                parmas.Add("@Filterdate", fltdate);
                parmas.Add("@FilterStatusList", ServiceStatusList);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_GetServiceCallBackList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstServiceCallList = multipleresult.Read<ServiceCallListViewModel>().ToList();
                    }
                }
            }
            return lstServiceCallList;
        }

        public CustomerDetailInfoSummaryModel GetServiceCallListForSearch(int custid)
        {

            CustomerDetailInfoSummaryModel oCustomerDetailInfoSummaryModel = new CustomerDetailInfoSummaryModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", custid);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_C_GetCustomerDetailInfoSummary", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCustomerDetailInfoSummaryModel = multipleresult.Read<CustomerDetailInfoSummaryModel>().ToList().FirstOrDefault();
                    }
                }
            }
            return oCustomerDetailInfoSummaryModel != null ? oCustomerDetailInfoSummaryModel : new CustomerDetailInfoSummaryModel();
        }


        public List<CustomerUploadDocumentViewModel> GetCUploadDocument(int classid, int typelistid)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.portal_spGet_C_GetUploadDocument(classid, typelistid, SelectedRegionId).MapEnumerable<CustomerUploadDocumentViewModel, portal_spGet_C_GetUploadDocument_Result>().ToList();

            }
        }

        public UploadDocument GetUploadDocumenwithFileType(int CustomerId, int FileTypeListId, int TypeListId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var modelData = context.UploadDocuments.Where(w => w.ClassId == CustomerId && w.FileTypeListId == FileTypeListId && w.TypeListId == TypeListId);
            if (modelData != null)
            {
                return modelData.FirstOrDefault();
            }
            return new UploadDocument();

        }

        public UploadDocument GetUploadDocumentById(int id)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var entity = context.UploadDocuments.Where(w => w.UploadDocumentId == id);
            if (entity != null)
            {
                return entity.FirstOrDefault();
            }
            else
            {
                return new UploadDocument();
            }
        }

        public void DeleteUploadDocument(int id)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var entity = context.UploadDocuments.Where(w => w.UploadDocumentId == id);
            if (entity != null)
            {
                context.UploadDocuments.Remove(entity.FirstOrDefault());
                context.SaveChanges();
            }
        }

        public bool SaveUploadDocument(int _ClassId, int _TypeListId, int _FileTypeListId, string _FilePath, string _FileName, string _FileExt, int _FileSize, bool IsNew = false)
        {
            try
            {
                using (var context = new jkDatabaseEntities())
                {
                    UploadDocument oUploadDocument = context.UploadDocuments.Where(c => c.ClassId == _ClassId && c.TypeListId == _TypeListId && c.FileTypeListId == _FileTypeListId).FirstOrDefault();
                    if (oUploadDocument != null && IsNew == false)
                    {
                        oUploadDocument.FileExt = _FileExt;
                        oUploadDocument.FileName = _FileName;
                        oUploadDocument.FilePath = _FilePath;
                        oUploadDocument.FileSize = _FileSize;
                    }
                    else
                    {
                        oUploadDocument = new UploadDocument();
                        oUploadDocument.ClassId = _ClassId;
                        oUploadDocument.CreatedBy = LoginUserId;
                        oUploadDocument.CreatedOn = DateTime.Now; ;
                        oUploadDocument.FileExt = _FileExt;
                        oUploadDocument.FileName = _FileName;
                        oUploadDocument.FilePath = _FilePath;
                        oUploadDocument.FileSize = _FileSize;
                        oUploadDocument.FileTypeListId = _FileTypeListId;
                        oUploadDocument.RegionId = SelectedRegionId;
                        oUploadDocument.TypeListId = _TypeListId;

                        context.UploadDocuments.Add(oUploadDocument);

                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Invoice GetInvoiceDetailForCN(int _InvoiceId)
        {
            using (var context = new jkDatabaseEntities())
            {
                Invoice oInvoice = context.Invoices.SingleOrDefault(c => c.InvoiceId == _InvoiceId);
                return oInvoice;
            }
        }


        public RevenueDistributionInvoiceDetailViewModel GetRevenueDistributionDetail(int _InvoiceId)
        {
            RevenueDistributionInvoiceDetailViewModel oInvoiceDetailViewModel = new RevenueDistributionInvoiceDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@InvoiceId", _InvoiceId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_GetRevenueDistributionDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oInvoiceDetailViewModel.InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList().FirstOrDefault();
                        oInvoiceDetailViewModel.InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                        oInvoiceDetailViewModel.FranchiseeDistributionItems = multipleresult.Read<FranchiseeDistribution>().ToList();
                        oInvoiceDetailViewModel.InvoiceRegion = multipleresult.Read<JKApi.Data.DAL.Region>().ToList().FirstOrDefault();
                    }
                }

            }

            return oInvoiceDetailViewModel;
        }
        public List<CalanderDatesModel> GetCalanderDates(DateTime _from, DateTime _to)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.fn_GetCalanderDatesFromDateRange(_from, _to).MapEnumerable<CalanderDatesModel, fn_GetCalanderDatesFromDateRange_Result>().ToList();
            }
        }


        public List<InvoiceRevenueDistributionDetailViewModel> CheckInvoiceForRevenueDistributionDetail(int _CustomerId, int ContractDetailId, DateTime _EffectiveDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", _CustomerId);
                parmas.Add("@ContractDetailId", ContractDetailId);
                parmas.Add("@EffectiveDate", _EffectiveDate);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CheckRevenueDistributionDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<InvoiceRevenueDistributionDetailViewModel>().ToList();
                    }
                }
            }
            return new List<InvoiceRevenueDistributionDetailViewModel>();
        }
        public List<InvoiceRevenueDistributionDetailViewModel> CheckInvoiceForDecreaseCreditDetail(int _CustomerId, DateTime _EffectiveDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", _CustomerId);
                parmas.Add("@EffectiveDate", _EffectiveDate);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CheckInvoiceForDecreaseCreditDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<InvoiceRevenueDistributionDetailViewModel>().ToList();
                    }
                }
            }
            return new List<InvoiceRevenueDistributionDetailViewModel>();
        }

        public bool InsertCustomerTransferData(List<CommonRevenueDistributionDetailViewModel> DistributionDetail, List<CommonRevenueDistributionFeeDetailViewModel> FeeDetail, CommonFranchiseeCustomerViewModel InputData)
        {
            bool retVal = true;
            using (var context = new jkDatabaseEntities())
            {

                bool isFirst = true;
                int MaintenanceTempId = 0;
                int MaintenanceTempDetailId = 0;
                int? _reasonid = 0;

                foreach (CommonRevenueDistributionDetailViewModel o in DistributionDetail)
                {
                    if (isFirst)
                    {
                        _reasonid = context.Customers.SingleOrDefault(c => c.CustomerId == o.CustomerId).RegionId;


                        //MaintenanceTemp Header
                        MaintenanceTemp oMaster = new MaintenanceTemp();

                        oMaster.MaintenanceTypeListId = 2; // #2	Transfer
                        oMaster.MaintenanceTempId = 0;
                        oMaster.ClassId = o.CustomerId;
                        oMaster.TypeListId = 1;
                        oMaster.EffectiveDate = o.EffectiveDate;
                        oMaster.IsActive = true;
                        oMaster.CreatedBy = LoginUserId;
                        oMaster.CreatedDate = DateTime.Now;

                        oMaster.Reason = "";
                        oMaster.StatusListId = o.StatusListId;
                        oMaster.StatusReasonListId = o.StatusReasonListId;
                        //oHeader.RequestChangeNotes;
                        //oHeader.RequestChangeStatusListId;
                        //oHeader.ResumeDate;
                        //oHeader.Comments;
                        //oHeader.LastServiceDate;
                        //oHeader.ModifiedBy;
                        //oHeader.ModifiedDate;

                        oMaster.MaintenanceStatusListId = 1;
                        JKApi.Data.DAL.Customer customer = context.Customers.Where(x => x.CustomerId == o.CustomerId).FirstOrDefault();
                        if (customer != null) oMaster.RegionId = customer.RegionId;

                        JKApi.Data.DAL.Period period = context.Periods.Where(x => x.BillMonth == o.EffectiveDate.Month && x.BillYear == o.EffectiveDate.Year).FirstOrDefault();
                        if (period != null) oMaster.PeriodId = period.PeriodId;



                        context.MaintenanceTemps.Add(oMaster);
                        context.SaveChanges();
                        MaintenanceTempId = oMaster.MaintenanceTempId;
                        isFirst = false;
                    }

                    //MaintenanceTempDetail Detail's 

                    MaintenanceTempDetail oMasterDetail;



                    if (o.RecordType == "STOPFF")
                    {
                        //  #   5   Finder Fee // FOR OLD FINDERFEE STOP
                        oMasterDetail = new MaintenanceTempDetail();
                        oMasterDetail.MaintenanceDetailTypeListId = 5;
                        oMasterDetail.CustomerId = o.CustomerId;
                        oMasterDetail.MaintenanceTempId = MaintenanceTempId;
                        oMasterDetail.DFranchiseeId = o.FranchiseeId;
                        oMasterDetail.ContractDetailId = o.ContractDetailId;
                        oMasterDetail.DistributionId = o.DistributionId;
                        oMasterDetail.FFIsIsTroubleWithoutFee = o.ApplyTroubleWithoutFee;
                        oMasterDetail.FFIsNotATroubleAccount = o.ApplyNotATroubleAccoun;
                        oMasterDetail.FFIsTroubleWithFee = o.ApplyTroubleWithFee;
                        oMasterDetail.FFTroubleWithFeeAmount = o.ApplyTroubleWithFeeAmount;
                        oMasterDetail.FFStopDate = o.FFStopDate;
                        oMasterDetail.FFNotes = o.FFNotes;
                        oMasterDetail.FFAmount = o.Amount;
                        oMasterDetail.CreatedBy = LoginUserId;
                        oMasterDetail.CreatedDate = DateTime.Now;
                        oMasterDetail.RecordType = "STOPFF";
                        context.MaintenanceTempDetails.Add(oMasterDetail);
                        context.SaveChanges();

                    }
                    else
                    {
                        MaintenanceTempDetailId = 0;
                        //  #   3   Distribution //FOR OLD & FOR NEW TRANSFER
                        oMasterDetail = new MaintenanceTempDetail();
                        oMasterDetail.MaintenanceDetailTypeListId = 3;

                        oMasterDetail.CustomerId = o.CustomerId;
                        oMasterDetail.MaintenanceTempId = MaintenanceTempId;
                        oMasterDetail.DFranchiseeId = o.FranchiseeId;
                        oMasterDetail.ContractDetailId = o.ContractDetailId;
                        oMasterDetail.DContractDetailId = o.ContractDetailId;
                        oMasterDetail.DistributionId = o.DistributionId;
                        oMasterDetail.DAmount = o.Amount;
                        oMasterDetail.DFeeAmount = o.FeeAmount;
                        oMasterDetail.DTotalAmount = o.TotalAmount;
                        if (o.RecordType == "INVDIST" || o.RecordType == "OLDINVDIST")
                        {
                            oMasterDetail.MaintenanceDetailTypeListId = 7;//  #   7   Invoice Distribution                            
                            oMasterDetail.DInvoiceId = o.DistributionId;
                        }

                        oMasterDetail.RecordType = o.RecordType;
                        oMasterDetail.CreatedBy = LoginUserId;
                        oMasterDetail.CreatedDate = DateTime.Now;
                        context.MaintenanceTempDetails.Add(oMasterDetail);
                        context.SaveChanges();

                        MaintenanceTempDetailId = oMasterDetail.MaintenanceTempDetailId;

                        if (o.RecordType == "NEW")
                        {
                            foreach (CommonRevenueDistributionFeeDetailViewModel f in FeeDetail)
                            {
                                if (f.FranchiseeId == o.FranchiseeId && f.ContractDetailId == o.ContractDetailId)
                                {
                                    //  #   8   Invoice Distribution Fee
                                    oMasterDetail = new MaintenanceTempDetail();
                                    oMasterDetail.MaintenanceDetailTypeListId = 8;
                                    oMasterDetail.CustomerId = o.CustomerId;
                                    oMasterDetail.MaintenanceTempId = MaintenanceTempId;
                                    oMasterDetail.DFranchiseeId = o.FranchiseeId;
                                    oMasterDetail.ContractDetailId = o.ContractDetailId;
                                    oMasterDetail.DContractDetailId = o.ContractDetailId;
                                    oMasterDetail.DistributionId = o.DistributionId;
                                    oMasterDetail.FeeId = f.FeeId.ToString();
                                    oMasterDetail.DFAmount = f.Amount;
                                    oMasterDetail.RecordType = o.RecordType;
                                    oMasterDetail.CreatedBy = LoginUserId;
                                    oMasterDetail.CreatedDate = DateTime.Now;
                                    context.MaintenanceTempDetails.Add(oMasterDetail);
                                    context.SaveChanges();
                                }

                            }
                        }
                    }
                }


                //MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();


                if (InputData.CustomerDetail != null)
                {
                    //Finder Fee
                    MaintenanceTempDetail oMaintenanceTempDetail_FinderFee;
                    oMaintenanceTempDetail_FinderFee = new MaintenanceTempDetail();
                    oMaintenanceTempDetail_FinderFee.MaintenanceTempId = MaintenanceTempId;
                    oMaintenanceTempDetail_FinderFee.MaintenanceDetailTypeListId = 5; //Finder Fee
                    oMaintenanceTempDetail_FinderFee.CustomerId = InputData.CustomerDetail.CustomerId;
                    oMaintenanceTempDetail_FinderFee.ContractId = InputData.CustomerDetail.ContractId;
                    oMaintenanceTempDetail_FinderFee.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;

                    oMaintenanceTempDetail_FinderFee.FindersFeeId = InputData.FindersFee.FindersFeeId;
                    oMaintenanceTempDetail_FinderFee.FFTransactionStatusListId = InputData.FindersFee.StatusListId;
                    oMaintenanceTempDetail_FinderFee.FFStartDate = InputData.FindersFee.StartDate;
                    oMaintenanceTempDetail_FinderFee.FFResumeDate = InputData.FindersFee.ResumeDate;
                    oMaintenanceTempDetail_FinderFee.FFFindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    oMaintenanceTempDetail_FinderFee.FFContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    oMaintenanceTempDetail_FinderFee.FFTotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    oMaintenanceTempDetail_FinderFee.FFFactor = InputData.FindersFee.Factor;
                    oMaintenanceTempDetail_FinderFee.FFDownPayPercentage = InputData.FindersFee.DownPayPercentage;
                    oMaintenanceTempDetail_FinderFee.FFInterest = InputData.FindersFee.Interest;
                    oMaintenanceTempDetail_FinderFee.FFTotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    oMaintenanceTempDetail_FinderFee.FFMonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    oMaintenanceTempDetail_FinderFee.FFFinancedAmount = InputData.FindersFee.FinancedAmount;
                    oMaintenanceTempDetail_FinderFee.FFDownPaymentAmount = InputData.FindersFee.DownPaymentAmount;
                    oMaintenanceTempDetail_FinderFee.FFTotalAmount = InputData.FindersFee.TotalAmount;
                    oMaintenanceTempDetail_FinderFee.FFMultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                    oMaintenanceTempDetail_FinderFee.FFPaidAmount = InputData.FindersFee.PaidAmount;
                    oMaintenanceTempDetail_FinderFee.FFBalanceAmount = InputData.FindersFee.BalanceAmount;
                    oMaintenanceTempDetail_FinderFee.FFPayableOnAmount = InputData.FindersFee.PayableOnAmount;
                    oMaintenanceTempDetail_FinderFee.FFNotes = InputData.FindersFee.Notes;
                    oMaintenanceTempDetail_FinderFee.FFInterestAmount = InputData.FindersFee.InterestAmount;
                    oMaintenanceTempDetail_FinderFee.FFMonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                    oMaintenanceTempDetail_FinderFee.FFIncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                    oMaintenanceTempDetail_FinderFee.FFADescription = InputData.FindersFee.Description;
                    oMaintenanceTempDetail_FinderFee.RecordType = "New";
                    oMaintenanceTempDetail_FinderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    oMaintenanceTempDetail_FinderFee.CreatedDate = DateTime.Now;
                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFee);
                    context.SaveChanges();


                    //Finder Fee Adjustment
                    MaintenanceTempDetail oMaintenanceTempDetail_FinderFeeAdjustment;
                    if (InputData.lstFindersFeeAdjustment != null)
                    {
                        foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                        {
                            oMaintenanceTempDetail_FinderFeeAdjustment = new MaintenanceTempDetail();
                            oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceTempId = MaintenanceTempId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceDetailTypeListId = 6; //Finder Fee Adjustment
                            oMaintenanceTempDetail_FinderFeeAdjustment.CustomerId = InputData.CustomerDetail.CustomerId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.ContractId = InputData.CustomerDetail.ContractId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;

                            oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                            oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                            oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.RecordType = "New";
                            oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = LoginUserId;
                            oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;
                            context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFeeAdjustment);
                            context.SaveChanges();
                        }
                    }
                }
                //List<MaintenanceTempDetailViewModel> lstMaintenanceTempDetailViewModel = new List<MaintenanceTempDetailViewModel>();
                //MaintenanceTempDetailViewModel oDetail;

                //  #   3   Distribution //FOR OLD
                //  #   3   Distribution // FOR NEW TRANSFER
                //  #   5   Finder Fee // FOR OLD FINDERFEE STOP



                //  #   7   Invoice Distribution
                //  #   8   Invoice Distribution Fee
                //  #   5   Finder Fee // FOR NEW FINDERFEE
                //  #   6   Finder Fee Adjustment



                retVal = true;
            }
            return retVal;
        }

        public List<CustomerTransferPendingViewModel> GetCustomerTransferPendingList(string regionId = "")
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerTransferPendingList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CustomerTransferPendingViewModel>().ToList();
                    }
                }
            }
            return new List<CustomerTransferPendingViewModel>();
        }

        public CommonCustomerrTransferPendingDetailViewModel GetCustomerrTransferPendingDetailData(int MasterCustomerTransferTempId)
        {

            CommonCustomerrTransferPendingDetailViewModel oCommon = new CommonCustomerrTransferPendingDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MasterCustomerTransferTempId", MasterCustomerTransferTempId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerTransferPendingDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerrTransferPendingDetail = multipleresult.Read<CustomerrTransferPendingDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<CTPDetailContractDetailViewModel>().ToList();
                        oCommon.lstFranchiseeDistribution = multipleresult.Read<CTPDetailFranchiseeDistributionViewModel>().ToList();
                        oCommon.lstFranchiseeStopFindersFee = multipleresult.Read<CTPDetailFranchiseeStopFindersFeeViewModel>().ToList();
                        oCommon.lstRevenueDistribution = multipleresult.Read<CTPDetailInvoiceFranchiseeDistributionViewModel>().ToList();
                        oCommon.lstRevenueDistributionOld = multipleresult.Read<CTPDetailInvoiceFranchiseeDistributionViewModel>().ToList();

                        List<CTPDetailInvoiceFranchiseeDistributionViewModel> lsttemp = new List<CTPDetailInvoiceFranchiseeDistributionViewModel>();
                        lsttemp.AddRange(oCommon.lstRevenueDistribution);
                        lsttemp.AddRange(oCommon.lstRevenueDistributionOld);
                        oCommon.lstRevenueDistributionCombine = lsttemp.OrderBy(o => o.InvoiceId).ThenByDescending(p => p.BillingPayNo).ToList();

                        oCommon.lstFindersFee = multipleresult.Read<CIDFindersFeeViewModel>().ToList();
                        oCommon.lstFindersFeeAdjustment = multipleresult.Read<CIDFindersFeeAdjustmentViewModel>().ToList();
                    }
                }
            }

            return oCommon;

        }

        public CommonCustomerMaintenanceDetailViewModel GetCustomerMaintenanceDetailDataPP(int MasterCustomerTransferTempId)
        {

            CommonCustomerMaintenanceDetailViewModel oCommon = new CommonCustomerMaintenanceDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MasterCustomerTransferTempId", MasterCustomerTransferTempId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerMaintenanceDetailPP", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerrMaintenanceDetail = multipleresult.Read<CustomerMaintenanceDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<CMContractDetailViewModel>().ToList();
                        oCommon.lstFranchiseeDistribution = multipleresult.Read<CMFranchiseeDistributionViewModel>().ToList();
                        oCommon.lstStopFindersFeeCancellation = multipleresult.Read<CMStopFindersFeeCancellationViewModel>().ToList();

                        oCommon.lstCreditInvoiceItem = multipleresult.Read<CMCreditInvoiceItemsViewModel>().ToList();
                        oCommon.lstCreditBillingPay = multipleresult.Read<CMCreditBillingPayViewModel>().ToList();


                        //oCommon.lstFranchiseeStopFindersFee = multipleresult.Read<CTPDetailFranchiseeStopFindersFeeViewModel>().ToList();
                        //oCommon.lstRevenueDistribution = multipleresult.Read<CTPDetailInvoiceFranchiseeDistributionViewModel>().ToList();
                        //oCommon.lstRevenueDistributionOld = multipleresult.Read<CTPDetailInvoiceFranchiseeDistributionViewModel>().ToList();

                        //oCommon.lstFindersFee = multipleresult.Read<CIDFindersFeeViewModel>().ToList();
                        //oCommon.lstFindersFeeAdjustment = multipleresult.Read<CIDFindersFeeAdjustmentViewModel>().ToList();
                    }
                }
            }

            return oCommon;

        }


        public bool GetCustomerMaintenanceDataDelete(int MasterCustomerTransferTempId)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {
                var lstdetailDelete = context.MaintenanceTempDetails.Where(m => m.MaintenanceTempId == MasterCustomerTransferTempId);
                var objectDelete = context.MaintenanceTemps.SingleOrDefault(m => m.MaintenanceTempId == MasterCustomerTransferTempId);
                context.MaintenanceTempDetails.RemoveRange(lstdetailDelete);
                context.MaintenanceTemps.Remove(objectDelete);
                context.SaveChanges();

            }

            return retVal;

        }

        public List<ServiceCallLog> ServiceCallLogList(int Id, DateTime? startDate, DateTime? endDate, int month, int year)
        {
            using (var conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", Id);
                parmas.Add("@StartDate", startDate);
                parmas.Add("@ENDDate", endDate);
                parmas.Add("@CallMonth", month);
                parmas.Add("@CallYear", year);

                using (var multipleresult = conn.QueryMultiple("portal_spGet_C_ServiceCallLogList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<ServiceCallLog>().ToList();
                    }
                }
            }
            return new List<ServiceCallLog>();
        }


        public CommonCustomerrTransferPendingDetailViewModel GetCustomerrTransferPendingApproval(int MasterCustomerTransferTempId, bool IsApprove = true)
        {

            CommonCustomerrTransferPendingDetailViewModel oCommon = new CommonCustomerrTransferPendingDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MaintenanceTempId", MasterCustomerTransferTempId);
                parmas.Add("@CreatedBy", LoginUserId);
                parmas.Add("@IsApprove", IsApprove);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_C_CustomerTransferApproval", parmas, commandType: CommandType.StoredProcedure))
                {

                }
            }
            return oCommon;

        }

        public CommonCustomerMaintenanceDetailViewModel GetCustomerMaintenanceDetailPPApproval(int MasterCustomerTempId, bool IsApprove)
        {

            CommonCustomerMaintenanceDetailViewModel oCommon = new CommonCustomerMaintenanceDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MaintenanceTempId", MasterCustomerTempId);
                parmas.Add("@IsApprove", IsApprove);
                parmas.Add("@CreatedBy", LoginUserId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_C_CustomerMaintenanceDetailPPApproval", parmas, commandType: CommandType.StoredProcedure))
                {

                }
            }

            return oCommon;

        }

        public CommonFranchiseeCustomerViewModel GetCustomerTransferFindersFeeDetailData(int CustomerId, int FranchiseeId)
        {

            //CommonCustomerIncreaseDecreaseDetailViewModel oCommon = new CommonCustomerIncreaseDecreaseDetailViewModel();
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{

            //    List<CIDFindersFeeViewModel> lstFindersFee = new List<CIDFindersFeeViewModel>();
            //    lstFindersFee.Add(new CIDFindersFeeViewModel());

            //   var parmas = new DynamicParameters();
            //    parmas.Add("@CustomerId", CustomerId);
            //    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerIncreaseDecreaseDetail", parmas, commandType: CommandType.StoredProcedure))
            //    {
            //        if (multipleresult != null)
            //        {
            //            oCommon.CustomerIncreaseDecreaseDetail = multipleresult.Read<CIDDetailViewModel>().ToList().FirstOrDefault();
            //            oCommon.lstContractDetail = multipleresult.Read<CIDContractDetailViewModel>().ToList();
            //            oCommon.lstFranchiseeDistribution = multipleresult.Read<CIDFranchiseeDistributionViewModel>().ToList();
            //            oCommon.lstFindersFee = lstFindersFee;// multipleresult.Read<CIDFindersFeeViewModel>().ToList();
            //            oCommon.lstFindersFeeAdjustment = multipleresult.Read<CIDFindersFeeAdjustmentViewModel>().ToList();


            //        }
            //    }
            //}

            //return oCommon;

            CommonFranchiseeCustomerViewModel oCommon = new CommonFranchiseeCustomerViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                parmas.Add("@FranchiseeId", FranchiseeId);
                parmas.Add("@ContractDetailDistributionLineNo", 1);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_F_FranchiseeCustomerDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerDetail = multipleresult.Read<FCDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<FCContractDetailViewModel>().ToList();

                        oCommon.lstFranchiseeDistribution = multipleresult.Read<FCFranchiseeDistributionViewModel>().ToList();

                        oCommon.FranchiseeDistribution = oCommon.lstFranchiseeDistribution.FirstOrDefault();

                        List<FCFindersFeeViewModel> lstFindersFee = multipleresult.Read<FCFindersFeeViewModel>().ToList();
                        FCFindersFeeViewModel oFCFindersFeeViewModel = new FCFindersFeeViewModel();
                        oFCFindersFeeViewModel.Description = "Finder Fees on Contract Billing";
                        oCommon.FindersFee = lstFindersFee.Count > 0 ? lstFindersFee.FirstOrDefault() : oFCFindersFeeViewModel;
                        oCommon.lstFindersFeeAdjustment = multipleresult.Read<FCFindersFeeAdjustmentViewModel>().ToList();


                    }
                }
            }

            return oCommon;

        }

        public List<IncreaseInvoiceItemViewModel> GetCustomerIncreaseInvoice(int customerid, DateTime effectivedate, int monthDays, int workingDays, decimal applyamount)
        {

            CommonCustomerrTransferPendingDetailViewModel oCommon = new CommonCustomerrTransferPendingDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", customerid);
                parmas.Add("@EffectiveDate", effectivedate);
                parmas.Add("@ApplyAmount", applyamount);
                parmas.Add("@MonthDays", monthDays);
                parmas.Add("@WorkingDays", workingDays);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_GetInvoiceForIncrease", parmas, commandType: CommandType.StoredProcedure))
                {
                    var lst = multipleresult.Read<IncreaseInvoiceItemViewModel>().OrderBy(o => o.ItemDescription).ToList();
                    return lst;
                }
            }

            return new List<IncreaseInvoiceItemViewModel>();
        }


        //Customer Increment

        public CommonCustomerIncreaseDecreaseDetailViewModel GetCustomerIncreaseDecreaseDetailData(int CustomerId)
        {

            CommonCustomerIncreaseDecreaseDetailViewModel oCommon = new CommonCustomerIncreaseDecreaseDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerIncreaseDecreaseDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerIncreaseDecreaseDetail = multipleresult.Read<CIDDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<CIDContractDetailViewModel>().ToList();
                        oCommon.lstFranchiseeDistribution = multipleresult.Read<CIDFranchiseeDistributionViewModel>().ToList();
                        oCommon.lstFindersFee = multipleresult.Read<CIDFindersFeeViewModel>().ToList();
                        oCommon.lstFindersFeeAdjustment = multipleresult.Read<CIDFindersFeeAdjustmentViewModel>().ToList();


                    }
                }
            }
            if (oCommon.lstFindersFee.Count == 0)
            {
                CIDFindersFeeViewModel oCIDFindersFeeViewModel = new CIDFindersFeeViewModel();
                oCommon.lstFindersFee.Add(oCIDFindersFeeViewModel);
            }

            return oCommon;

        }


        public bool InsertCustomerIncreaseDecreaseDetail(CommonCustomerIncreaseDecreaseDetailViewModel InputData, List<CreditTransactionViewModel> lstCreditTransaction)
        {
            bool retVal = true;
            AccountReceivableService oAccountReceivableService = new AccountReceivableService();
            using (var context = new jkDatabaseEntities())
            {
                MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();


                if (InputData.CustomerIncreaseDecreaseDetail != null)
                {
                    oMaintenanceTemp = new MaintenanceTemp();
                    oMaintenanceTemp.ClassId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                    oMaintenanceTemp.TypeListId = 1;
                    oMaintenanceTemp.StatusListId = InputData.CustomerIncreaseDecreaseDetail.StatusListId;
                    oMaintenanceTemp.StatusReasonListId = InputData.CustomerIncreaseDecreaseDetail.StatusReasonListId;
                    oMaintenanceTemp.Reason = InputData.CustomerIncreaseDecreaseDetail.Reason;
                    oMaintenanceTemp.EffectiveDate = InputData.CustomerIncreaseDecreaseDetail.EffectiveDate;
                    oMaintenanceTemp.IsActive = true;
                    oMaintenanceTemp.CreatedBy = LoginUserId;
                    oMaintenanceTemp.CreatedDate = DateTime.Now;
                    oMaintenanceTemp.MaintenanceTypeListId = InputData.CustomerIncreaseDecreaseDetail.MaintenanceTypeListId;
                    oMaintenanceTemp.RegionId = InputData.CustomerIncreaseDecreaseDetail.RegionId;
                    oMaintenanceTemp.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());
                    context.MaintenanceTemps.Add(oMaintenanceTemp);
                    context.SaveChanges();



                    var lstInvoices = context.ManualInvoiceTmps.Where(o => o.CustomerId == InputData.CustomerIncreaseDecreaseDetail.CustomerId && o.IsActive == true).ToList();
                    if (lstInvoices.Count > 0)
                    {
                        var oInvoice = lstInvoices.OrderByDescending(o => o.ManualInvoiceTmpId).FirstOrDefault();
                        oInvoice.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;

                        context.SaveChanges();

                    }


                    //Contract
                    oMaintenanceTempDetail = new MaintenanceTempDetail();
                    oMaintenanceTempDetail.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                    oMaintenanceTempDetail.MaintenanceDetailTypeListId = 1; //Contract
                    oMaintenanceTempDetail.CustomerId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                    oMaintenanceTempDetail.ContractId = InputData.CustomerIncreaseDecreaseDetail.ContractId;
                    oMaintenanceTempDetail.CPONumber = (InputData.CustomerIncreaseDecreaseDetail.PONumber != null ? InputData.CustomerIncreaseDecreaseDetail.PONumber : "");
                    oMaintenanceTempDetail.CAccountTypeListId = InputData.CustomerIncreaseDecreaseDetail.AccountTypeListId;
                    oMaintenanceTempDetail.CQualified = InputData.CustomerIncreaseDecreaseDetail.Qualified;
                    oMaintenanceTempDetail.CSignDate = InputData.CustomerIncreaseDecreaseDetail.SignDate;
                    oMaintenanceTempDetail.CStartDate = InputData.CustomerIncreaseDecreaseDetail.StartDate;
                    oMaintenanceTempDetail.CExpirationDate = InputData.CustomerIncreaseDecreaseDetail.ExpirationDate;
                    oMaintenanceTempDetail.CContractTermMonth = InputData.CustomerIncreaseDecreaseDetail.ContractTermMonth;
                    oMaintenanceTempDetail.CTotalAmount = InputData.CustomerIncreaseDecreaseDetail.TotalAmount;
                    oMaintenanceTempDetail.COTotalAmount = InputData.CustomerIncreaseDecreaseDetail.OTotalAmount;
                    oMaintenanceTempDetail.CreatedBy = LoginUserId;
                    oMaintenanceTempDetail.CreatedDate = DateTime.Now;
                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail);
                    context.SaveChanges();

                    //Contract Detail
                    MaintenanceTempDetail oMaintenanceTempDetail_ContractDetail;
                    foreach (CIDContractDetailViewModel oContractDetail in InputData.lstContractDetail)
                    {
                        oMaintenanceTempDetail_ContractDetail = new MaintenanceTempDetail();
                        oMaintenanceTempDetail_ContractDetail.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                        oMaintenanceTempDetail_ContractDetail.MaintenanceDetailTypeListId = 2; //Contract Detail
                        oMaintenanceTempDetail_ContractDetail.CustomerId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                        oMaintenanceTempDetail_ContractDetail.ContractId = InputData.CustomerIncreaseDecreaseDetail.ContractId;
                        oMaintenanceTempDetail_ContractDetail.CPONumber = InputData.CustomerIncreaseDecreaseDetail.PONumber;
                        oMaintenanceTempDetail_ContractDetail.CAccountTypeListId = InputData.CustomerIncreaseDecreaseDetail.AccountTypeListId;
                        oMaintenanceTempDetail_ContractDetail.CQualified = InputData.CustomerIncreaseDecreaseDetail.Qualified;
                        oMaintenanceTempDetail_ContractDetail.CSignDate = InputData.CustomerIncreaseDecreaseDetail.SignDate;
                        oMaintenanceTempDetail_ContractDetail.CStartDate = InputData.CustomerIncreaseDecreaseDetail.StartDate;
                        oMaintenanceTempDetail_ContractDetail.CExpirationDate = InputData.CustomerIncreaseDecreaseDetail.ExpirationDate;
                        oMaintenanceTempDetail_ContractDetail.CContractTermMonth = InputData.CustomerIncreaseDecreaseDetail.ContractTermMonth;
                        oMaintenanceTempDetail_ContractDetail.CTotalAmount = InputData.CustomerIncreaseDecreaseDetail.TotalAmount;
                        oMaintenanceTempDetail_ContractDetail.COTotalAmount = InputData.CustomerIncreaseDecreaseDetail.OTotalAmount;

                        oMaintenanceTempDetail_ContractDetail.ContractDetailId = oContractDetail.ContractDetailId;
                        oMaintenanceTempDetail_ContractDetail.CDLineNumber = oContractDetail.LineNumber;
                        oMaintenanceTempDetail_ContractDetail.CDServiceTypeListId = oContractDetail.ServiceTypeListId;
                        oMaintenanceTempDetail_ContractDetail.CDBillingFrequencyListId = oContractDetail.BillingFrequencyListId;
                        oMaintenanceTempDetail_ContractDetail.CDSquareFootage = oContractDetail.SquareFootage;
                        oMaintenanceTempDetail_ContractDetail.CDAmount = oContractDetail.Amount;
                        oMaintenanceTempDetail_ContractDetail.CDOAmount = oContractDetail.OAmount;
                        oMaintenanceTempDetail_ContractDetail.CDStartTime = oContractDetail.StartTime;
                        oMaintenanceTempDetail_ContractDetail.CDEndTime = oContractDetail.EndTime;
                        oMaintenanceTempDetail_ContractDetail.CDCleanTimes = oContractDetail.CleanTimes;
                        oMaintenanceTempDetail_ContractDetail.CDCleanFrequencyListId = oContractDetail.CleanFrequencyListId;
                        oMaintenanceTempDetail_ContractDetail.CDMon = oContractDetail.Mon;
                        oMaintenanceTempDetail_ContractDetail.CDTues = oContractDetail.Tues;
                        oMaintenanceTempDetail_ContractDetail.CDWed = oContractDetail.Wed;
                        oMaintenanceTempDetail_ContractDetail.CDThur = oContractDetail.Thur;
                        oMaintenanceTempDetail_ContractDetail.CDFri = oContractDetail.Fri;
                        oMaintenanceTempDetail_ContractDetail.CDSat = oContractDetail.Sat;
                        oMaintenanceTempDetail_ContractDetail.CDSun = oContractDetail.Sun;
                        oMaintenanceTempDetail_ContractDetail.CDCPIIncrease = oContractDetail.CPIIncrease;
                        oMaintenanceTempDetail_ContractDetail.CDSeparateInvoice = oContractDetail.SeparateInvoice;
                        oMaintenanceTempDetail_ContractDetail.CDDescription = oContractDetail.Description;
                        oMaintenanceTempDetail_ContractDetail.CDNotes = oContractDetail.Notes;
                        oMaintenanceTempDetail_ContractDetail.CreatedBy = LoginUserId;
                        oMaintenanceTempDetail_ContractDetail.CreatedDate = DateTime.Now;

                        context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_ContractDetail);
                        context.SaveChanges();
                    }

                    //Distribution
                    MaintenanceTempDetail oMaintenanceTempDetail_Distribution;
                    foreach (CIDFranchiseeDistributionViewModel oFranchiseeDistribution in InputData.lstFranchiseeDistribution)
                    {
                        oMaintenanceTempDetail_Distribution = new MaintenanceTempDetail();
                        oMaintenanceTempDetail_Distribution.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                        oMaintenanceTempDetail_Distribution.MaintenanceDetailTypeListId = 3; //Contract Detail
                        oMaintenanceTempDetail_Distribution.CustomerId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                        oMaintenanceTempDetail_Distribution.ContractId = InputData.CustomerIncreaseDecreaseDetail.ContractId;
                        oMaintenanceTempDetail_Distribution.DistributionId = oFranchiseeDistribution.DistributionId;
                        oMaintenanceTempDetail_Distribution.DContractDetailId = oFranchiseeDistribution.ContractDetailId;
                        oMaintenanceTempDetail_Distribution.DFranchiseeId = oFranchiseeDistribution.FranchiseeId;
                        oMaintenanceTempDetail_Distribution.DAmount = oFranchiseeDistribution.Amount;
                        oMaintenanceTempDetail_Distribution.DFeeAmount = 0;
                        oMaintenanceTempDetail_Distribution.DTotalAmount = oFranchiseeDistribution.Amount;
                        oMaintenanceTempDetail_Distribution.DInvoiceId = 0;
                        oMaintenanceTempDetail_Distribution.CreatedBy = LoginUserId;
                        oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                        context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_Distribution);
                        context.SaveChanges();
                    }


                    //Finder Fee
                    MaintenanceTempDetail oMaintenanceTempDetail_FinderFee;
                    if (InputData.lstFindersFee != null)
                    {
                        foreach (CIDFindersFeeViewModel oFindersFee in InputData.lstFindersFee)
                        {
                            oMaintenanceTempDetail_FinderFee = new MaintenanceTempDetail();
                            oMaintenanceTempDetail_FinderFee.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                            oMaintenanceTempDetail_FinderFee.MaintenanceDetailTypeListId = 5; //Contract Detail
                            oMaintenanceTempDetail_FinderFee.CustomerId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                            oMaintenanceTempDetail_FinderFee.ContractId = InputData.CustomerIncreaseDecreaseDetail.ContractId;
                            oMaintenanceTempDetail_FinderFee.DFranchiseeId = oFindersFee.FranchiseeId;
                            oMaintenanceTempDetail_FinderFee.FindersFeeId = oFindersFee.FindersFeeId;
                            oMaintenanceTempDetail_FinderFee.FFTransactionStatusListId = oFindersFee.StatusListId;
                            oMaintenanceTempDetail_FinderFee.FFStartDate = oFindersFee.StartDate;
                            oMaintenanceTempDetail_FinderFee.FFResumeDate = oFindersFee.ResumeDate;
                            oMaintenanceTempDetail_FinderFee.FFFindersFeeTypeListId = oFindersFee.FindersFeeTypeListId;
                            oMaintenanceTempDetail_FinderFee.FFContractBillingAmount = oFindersFee.ContractBillingAmount;
                            oMaintenanceTempDetail_FinderFee.FFTotalAdjustmentAmount = oFindersFee.TotalAdjustmentAmount;
                            oMaintenanceTempDetail_FinderFee.FFFactor = oFindersFee.Factor;
                            oMaintenanceTempDetail_FinderFee.FFDownPayPercentage = oFindersFee.DownPayPercentage;
                            oMaintenanceTempDetail_FinderFee.FFInterest = oFindersFee.Interest;
                            oMaintenanceTempDetail_FinderFee.FFTotalNumOfpayments = oFindersFee.TotalNumOfpayments;
                            oMaintenanceTempDetail_FinderFee.FFMonthlyPaymentAmount = oFindersFee.MonthlyPaymentAmount;
                            oMaintenanceTempDetail_FinderFee.FFFinancedAmount = oFindersFee.FinancedAmount;
                            oMaintenanceTempDetail_FinderFee.FFDownPaymentAmount = oFindersFee.DownPaymentAmount;
                            oMaintenanceTempDetail_FinderFee.FFTotalAmount = oFindersFee.TotalAmount;
                            oMaintenanceTempDetail_FinderFee.FFMultiTenantOccupancyAmount = oFindersFee.MultiTenantOccupancyAmount;
                            oMaintenanceTempDetail_FinderFee.FFPaidAmount = oFindersFee.PaidAmount;
                            oMaintenanceTempDetail_FinderFee.FFBalanceAmount = oFindersFee.BalanceAmount;
                            oMaintenanceTempDetail_FinderFee.FFPayableOnAmount = oFindersFee.PayableOnAmount;
                            oMaintenanceTempDetail_FinderFee.FFNotes = oFindersFee.Notes;
                            oMaintenanceTempDetail_FinderFee.FFADescription = oFindersFee.Description;
                            oMaintenanceTempDetail_FinderFee.FFInterestAmount = oFindersFee.InterestAmount;
                            oMaintenanceTempDetail_FinderFee.FFMonthlyPaymentPercentage = oFindersFee.MonthlyPaymentPercentage;
                            oMaintenanceTempDetail_FinderFee.FFIncludeDownPayInFirstPay = oFindersFee.IncludeDownPayInFirstPay;

                            oMaintenanceTempDetail_FinderFee.CreatedBy = LoginUserId;
                            oMaintenanceTempDetail_FinderFee.CreatedDate = DateTime.Now;
                            context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFee);
                            context.SaveChanges();



                        }

                        //Finder Fee Adjustment
                        MaintenanceTempDetail oMaintenanceTempDetail_FinderFeeAdjustment;
                        if (InputData.lstFindersFeeAdjustment != null)
                        {
                            foreach (CIDFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                            {
                                oMaintenanceTempDetail_FinderFeeAdjustment = new MaintenanceTempDetail();
                                oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceDetailTypeListId = 6; //Contract Detail
                                oMaintenanceTempDetail_FinderFeeAdjustment.CustomerId = InputData.CustomerIncreaseDecreaseDetail.CustomerId;
                                oMaintenanceTempDetail_FinderFeeAdjustment.ContractId = InputData.CustomerIncreaseDecreaseDetail.ContractId;

                                oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                                oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                                oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                                oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;


                                oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = LoginUserId;
                                oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;
                                context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFeeAdjustment);
                                context.SaveChanges();
                            }
                        }

                    }

                    //Credit Records for credit
                    foreach (var vm in lstCreditTransaction)
                    {
                        oAccountReceivableService.InsertUpdateCustomerCreditTransactionMaintenanceTemp(vm, oMaintenanceTemp.MaintenanceTempId);
                    }
                }
            }

            return retVal;

        }


        public CommonCustomerIncreaseDecreaseDetailViewModel GetCustomerIncreaseDecreaseDetailDataForEdit(int MaintenanceTempId)
        {

            CommonCustomerIncreaseDecreaseDetailViewModel oCommon = new CommonCustomerIncreaseDecreaseDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MaintenanceTempId", MaintenanceTempId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_C_CustomerIncreaseDecreaseDetailForEdit", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerIncreaseDecreaseDetail = multipleresult.Read<CIDDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<CIDContractDetailViewModel>().ToList();
                        oCommon.lstFranchiseeDistribution = multipleresult.Read<CIDFranchiseeDistributionViewModel>().ToList();
                        oCommon.lstFindersFee = multipleresult.Read<CIDFindersFeeViewModel>().ToList();
                        oCommon.lstFindersFeeAdjustment = multipleresult.Read<CIDFindersFeeAdjustmentViewModel>().ToList();

                        oCommon.lstCreditInvoiceItem = multipleresult.Read<CMCreditInvoiceItemsViewModel>().ToList();
                        oCommon.lstCreditBillingPay = multipleresult.Read<CMCreditBillingPayViewModel>().ToList();
                    }
                }
            }

            return oCommon;

        }

        public bool UpdateCustomerIncreaseDecreaseDetailApprove(CommonCustomerIncreaseDecreaseDetailViewModel InputData, bool IsApprove = true)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {

                if (!IsApprove)
                {
                    MaintenanceTemp oMaintenanceTempReject = new MaintenanceTemp();
                    if (InputData.CustomerIncreaseDecreaseDetail != null)
                    {
                        oMaintenanceTempReject = context.MaintenanceTemps.SingleOrDefault(o => o.MaintenanceTempId == InputData.CustomerIncreaseDecreaseDetail.MaintenanceTempId);
                        if (oMaintenanceTempReject != null)
                        {
                            oMaintenanceTempReject.MaintenanceStatusListId = 2;
                            oMaintenanceTempReject.ModifiedBy = LoginUserId;
                            oMaintenanceTempReject.ModifiedDate = DateTime.Now;
                            context.SaveChanges();
                        }
                    }
                    return true;
                }

                //MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                //MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();


                //if (InputData.CustomerIncreaseDecreaseDetail != null)
                //{
                //    oMaintenanceTemp = context.MaintenanceTemps.SingleOrDefault(o => o.MaintenanceTempId == InputData.CustomerIncreaseDecreaseDetail.MaintenanceTempId);

                //    if (oMaintenanceTemp != null)
                //    {
                //        oMaintenanceTemp.StatusReasonListId = InputData.CustomerIncreaseDecreaseDetail.StatusReasonListId;
                //        oMaintenanceTemp.EffectiveDate = InputData.CustomerIncreaseDecreaseDetail.EffectiveDate;
                //        context.SaveChanges();

                //        //Contract

                //        MaintenanceTempDetail oMaintenanceTempDetail_Contract = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == InputData.CustomerIncreaseDecreaseDetail.MaintenanceTempDetailId);
                //        if (oMaintenanceTempDetail_Contract != null)
                //        {
                //            oMaintenanceTempDetail_Contract.CPONumber = InputData.CustomerIncreaseDecreaseDetail.PONumber;
                //            oMaintenanceTempDetail_Contract.CAccountTypeListId = InputData.CustomerIncreaseDecreaseDetail.AccountTypeListId;
                //            oMaintenanceTempDetail_Contract.CQualified = InputData.CustomerIncreaseDecreaseDetail.Qualified;
                //            oMaintenanceTempDetail_Contract.CSignDate = InputData.CustomerIncreaseDecreaseDetail.SignDate;
                //            oMaintenanceTempDetail_Contract.CStartDate = InputData.CustomerIncreaseDecreaseDetail.StartDate;
                //            oMaintenanceTempDetail_Contract.CExpirationDate = InputData.CustomerIncreaseDecreaseDetail.ExpirationDate;
                //            oMaintenanceTempDetail_Contract.CContractTermMonth = InputData.CustomerIncreaseDecreaseDetail.ContractTermMonth;
                //            oMaintenanceTempDetail_Contract.CTotalAmount = InputData.CustomerIncreaseDecreaseDetail.TotalAmount;
                //            oMaintenanceTempDetail_Contract.CContractDescription = InputData.CustomerIncreaseDecreaseDetail.ContractDescription;
                //            oMaintenanceTempDetail_Contract.CreatedBy = LoginUserId;
                //            oMaintenanceTempDetail_Contract.CreatedDate = DateTime.Now;
                //            context.SaveChanges();



                //            //Contract Detail
                //            foreach (CIDContractDetailViewModel oContractDetail in InputData.lstContractDetail)
                //            {
                //                MaintenanceTempDetail oMaintenanceTempDetail_ContractDetail = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oContractDetail.MaintenanceTempDetailId);
                //                if (oMaintenanceTempDetail_ContractDetail != null)
                //                {
                //                    oMaintenanceTempDetail_ContractDetail.CPONumber = InputData.CustomerIncreaseDecreaseDetail.PONumber;
                //                    oMaintenanceTempDetail_ContractDetail.CAccountTypeListId = InputData.CustomerIncreaseDecreaseDetail.AccountTypeListId;
                //                    oMaintenanceTempDetail_ContractDetail.CQualified = InputData.CustomerIncreaseDecreaseDetail.Qualified;
                //                    oMaintenanceTempDetail_ContractDetail.CSignDate = InputData.CustomerIncreaseDecreaseDetail.SignDate;
                //                    oMaintenanceTempDetail_ContractDetail.CStartDate = InputData.CustomerIncreaseDecreaseDetail.StartDate;
                //                    oMaintenanceTempDetail_ContractDetail.CExpirationDate = InputData.CustomerIncreaseDecreaseDetail.ExpirationDate;
                //                    oMaintenanceTempDetail_ContractDetail.CContractTermMonth = InputData.CustomerIncreaseDecreaseDetail.ContractTermMonth;
                //                    oMaintenanceTempDetail_ContractDetail.CTotalAmount = InputData.CustomerIncreaseDecreaseDetail.TotalAmount;
                //                    oMaintenanceTempDetail_ContractDetail.CDServiceTypeListId = oContractDetail.ServiceTypeListId;
                //                    oMaintenanceTempDetail_ContractDetail.CDBillingFrequencyListId = oContractDetail.BillingFrequencyListId;
                //                    oMaintenanceTempDetail_ContractDetail.CDSquareFootage = oContractDetail.SquareFootage;
                //                    oMaintenanceTempDetail_ContractDetail.CDAmount = oContractDetail.Amount;
                //                    oMaintenanceTempDetail_ContractDetail.CDStartTime = oContractDetail.StartTime;
                //                    oMaintenanceTempDetail_ContractDetail.CDEndTime = oContractDetail.EndTime;
                //                    oMaintenanceTempDetail_ContractDetail.CDCleanTimes = oContractDetail.CleanTimes;
                //                    oMaintenanceTempDetail_ContractDetail.CDCleanFrequencyListId = oContractDetail.CleanFrequencyListId;
                //                    oMaintenanceTempDetail_ContractDetail.CDMon = oContractDetail.Mon;
                //                    oMaintenanceTempDetail_ContractDetail.CDTues = oContractDetail.Thur;
                //                    oMaintenanceTempDetail_ContractDetail.CDWed = oContractDetail.Wed;
                //                    oMaintenanceTempDetail_ContractDetail.CDThur = oContractDetail.Thur;
                //                    oMaintenanceTempDetail_ContractDetail.CDFri = oContractDetail.Fri;
                //                    oMaintenanceTempDetail_ContractDetail.CDSat = oContractDetail.Sat;
                //                    oMaintenanceTempDetail_ContractDetail.CDSun = oContractDetail.Sun;
                //                    oMaintenanceTempDetail_ContractDetail.CDCPIIncrease = oContractDetail.CPIIncrease;
                //                    oMaintenanceTempDetail_ContractDetail.CDSeparateInvoice = oContractDetail.SeparateInvoice;
                //                    oMaintenanceTempDetail_ContractDetail.CDDescription = oContractDetail.Description;
                //                    oMaintenanceTempDetail_ContractDetail.CDNotes = oContractDetail.Notes;
                //                    oMaintenanceTempDetail_ContractDetail.CreatedBy = LoginUserId;
                //                    oMaintenanceTempDetail_ContractDetail.CreatedDate = DateTime.Now;
                //                    context.SaveChanges();
                //                }
                //            }

                //            //Distribution

                //            foreach (CIDFranchiseeDistributionViewModel oFranchiseeDistribution in InputData.lstFranchiseeDistribution)
                //            {
                //                MaintenanceTempDetail oMaintenanceTempDetail_Distribution = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oFranchiseeDistribution.MaintenanceTempDetailId);
                //                if (oMaintenanceTempDetail_Distribution != null)
                //                {
                //                    oMaintenanceTempDetail_Distribution.DistributionId = oFranchiseeDistribution.DistributionId;
                //                    oMaintenanceTempDetail_Distribution.DContractDetailId = oFranchiseeDistribution.ContractDetailId;
                //                    oMaintenanceTempDetail_Distribution.DFranchiseeId = oFranchiseeDistribution.FranchiseeId;
                //                    oMaintenanceTempDetail_Distribution.DAmount = oFranchiseeDistribution.Amount;
                //                    oMaintenanceTempDetail_Distribution.DTotalAmount = oFranchiseeDistribution.Amount;
                //                    oMaintenanceTempDetail_Distribution.CreatedBy = LoginUserId;
                //                    oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                //                    context.SaveChanges();
                //                }
                //            }


                //            //Finder Fee
                //            foreach (CIDFindersFeeViewModel oFindersFee in InputData.lstFindersFee)
                //            {
                //                MaintenanceTempDetail oMaintenanceTempDetail_FinderFee = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oFindersFee.MaintenanceTempDetailId);
                //                if (oMaintenanceTempDetail_FinderFee != null)
                //                {
                //                    oMaintenanceTempDetail_FinderFee.FFTransactionStatusListId = oFindersFee.StatusListId;
                //                    oMaintenanceTempDetail_FinderFee.FFStartDate = oFindersFee.StartDate;
                //                    oMaintenanceTempDetail_FinderFee.FFResumeDate = oFindersFee.ResumeDate;
                //                    oMaintenanceTempDetail_FinderFee.FFFindersFeeTypeListId = oFindersFee.FindersFeeTypeListId;
                //                    oMaintenanceTempDetail_FinderFee.FFContractBillingAmount = oFindersFee.ContractBillingAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFTotalAdjustmentAmount = oFindersFee.TotalAdjustmentAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFFactor = oFindersFee.Factor;
                //                    oMaintenanceTempDetail_FinderFee.FFDownPayPercentage = oFindersFee.DownPayPercentage;
                //                    oMaintenanceTempDetail_FinderFee.FFInterest = oFindersFee.Interest;
                //                    oMaintenanceTempDetail_FinderFee.FFTotalNumOfpayments = oFindersFee.TotalNumOfpayments;
                //                    oMaintenanceTempDetail_FinderFee.FFMonthlyPaymentAmount = oFindersFee.MonthlyPaymentAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFFinancedAmount = oFindersFee.FinancedAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFDownPaymentAmount = oFindersFee.DownPaymentAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFTotalAmount = oFindersFee.TotalAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFMultiTenantOccupancyAmount = oFindersFee.MultiTenantOccupancyAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFPaidAmount = oFindersFee.PaidAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFBalanceAmount = oFindersFee.BalanceAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFPayableOnAmount = oFindersFee.PayableOnAmount;
                //                    oMaintenanceTempDetail_FinderFee.FFNotes = oFindersFee.Notes;
                //                    oMaintenanceTempDetail_FinderFee.FFADescription = oFindersFee.Description;
                //                    oMaintenanceTempDetail_FinderFee.CreatedBy = LoginUserId;
                //                    oMaintenanceTempDetail_FinderFee.CreatedDate = DateTime.Now;
                //                    context.SaveChanges();
                //                }

                //            }

                //            //Finder Fee Adjustment
                //            if (InputData.lstFindersFeeAdjustment != null)
                //            {
                //                foreach (CIDFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                //                {
                //                    MaintenanceTempDetail oMaintenanceTempDetail_FinderFeeAdjustment = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oFindersFeeAdjustment.MaintenanceTempDetailId);
                //                    if (oMaintenanceTempDetail_FinderFeeAdjustment != null)
                //                    {
                //                        oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                //                        oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                //                        oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;
                //                        oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = LoginUserId;
                //                        oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;
                //                        context.SaveChanges();
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MaintenanceTempId", InputData.CustomerIncreaseDecreaseDetail.MaintenanceTempId);
                parmas.Add("@UserId", LoginUserId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_C_CustomerIncreaseDecreaseApproval", parmas, commandType: CommandType.StoredProcedure))
                {

                }
            }

            return retVal;

        }


        // ======================================================================================
        #region API
        // ======================================================================================

        public List<CustomerModel> GetCustomerList()
        {
            var parameters = new DynamicParameters();

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCustomerList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public List<CustomerModel> GetCustomerListByRegion(int regionId, int pageSize, int page)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", regionId);
            parameters.Add("@PageNo", page);
            parameters.Add("@PageSize", pageSize);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCustomerListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public List<CustomerModel> GetCustomerListByFranchisee(int franchiseeId, int pageSize, int page)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", franchiseeId);
            parameters.Add("@PageNo", page);
            parameters.Add("@PageSize", pageSize);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCustomerListByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public List<CustomerModel> GetNearByCustomerListByFranchisee(int franchiseeId, double latitude, double longitude, double distance)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", franchiseeId);
            parameters.Add("@Latitude", latitude);
            parameters.Add("@Longitude", longitude);
            parameters.Add("@Distance", distance);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetNearByCustomerListByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public List<CustomerModel> GetNearByCustomerListByRegion(int regionId, double latitude, double longitude, double distance)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", regionId);
            parameters.Add("@Latitude", latitude);
            parameters.Add("@Longitude", longitude);
            parameters.Add("@Distance", distance);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetNearbyCustomerListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public List<CustomerLeadModel> GetNearByLeadListByRegion(int regionId, double latitude, double longitude, double distance)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", regionId);
            parameters.Add("@Latitude", latitude);
            parameters.Add("@Longitude", longitude);
            parameters.Add("@Distance", distance);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_getNearByLeadListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerLeadModel, AddressModel, CustomerLeadModel>((lead, address) =>
                {
                    lead.Address = address;
                    return lead;
                }, "Address1").ToList();
            }
        }

        public CustomerLeadModel GetLeadDetail(int leadDetailId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CRM_AccountCustomerDetailId", leadDetailId);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetLeadDetail, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerLeadModel, AddressModel, CustomerLeadModel>((lead, address) =>
                {
                    lead.Address = address;
                    return lead;
                }, "Address1").First();
            }
        }

        public List<CustomerModel> GetCustomerPendingListByRegion(int regionId, int pageSize, int page)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", regionId);
            parameters.Add("@PageNo", page);
            parameters.Add("@PageSize", pageSize);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCustomerPendingListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId").ToList();
            }
        }

        public CustomerModel GetCustomer(int customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", customerId);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var result = multipleResult?.Read<CustomerModel, AddressModel, CustomerModel>((customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                }, "AddressId");
                return result.Count() > 0 ? result.First() : null;
            }
        }

        public List<AccountWalkThruItemModel> GetAccountWalkThruItemListByCustomer(int customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", customerId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetAccountWalkThruItemListByCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<AccountWalkThruItemModel>().ToList();
            }
        }

        public AccountWalkThruItemModel GetAccountWalkThruItemById(int accountWalkThruItemId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AccountWalkThruItemId", accountWalkThruItemId);
            parameters.Add("@IsEnable", true);

            using (var result = FmsDbConn.QuerySingleOrDefault<AccountWalkThruItemModel>(DBConstants.sp_GetAccountWalkThruItemById, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public AccountWalkThruItemModel AddOrUpdateAccountWalkThruItem(AccountWalkThruItemModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AccountWalkThruItemId", model.AccountWalkThruItemId);
            parameters.Add("@AccountWalkThruType", model.AccountWalkThruType);
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@FranchiseeId", model.FranchiseeId);
            parameters.Add("@FieldValue", model.FieldValue);
            parameters.Add("@FieldText", model.FieldText);
            parameters.Add("@FileUrl", model.FileUrl);
            parameters.Add("@IsEnable", true);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.AccountWalkThruItemId == 0 ? DateTime.Now : model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate ?? DateTime.Now);

            using (var result = FmsDbConn.QuerySingleOrDefault<AccountWalkThruItemModel>(DBConstants.sp_AddOrUpdateAccountWalkThru, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public CustomerSimpleViewModel GetCustomerN(int customerId)
        {
            CustomerSimpleViewModel oCustomerSimpleViewModel = new CustomerSimpleViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", customerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_C_GetCustomer", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCustomerSimpleViewModel = multipleresult.Read<CustomerSimpleViewModel>().ToList().FirstOrDefault();
                    }
                }
            }
            return oCustomerSimpleViewModel;
        }

        public int SaveServiceCallLogDetailsByFORM(ServiceCallLogModel ServiceCallLogModel)
        {
            ServiceCallLog ServiceCallLog = new ServiceCallLog();
            ServiceCallLog.TypeListId = 1;
            ServiceCallLog.RegionId = ServiceCallLogModel.RegionId;
            ServiceCallLog.ClassId = ServiceCallLogModel.ClassId;
            ServiceCallLog.InitiatedById = Convert.ToInt32(ServiceCallLogModel.InitiatedBy);
            ServiceCallLog.ServiceLogAreaListId = Convert.ToInt32(ServiceCallLogModel.ServiceLogAreaListId);
            ServiceCallLog.ServiceLogTypeListId = Convert.ToInt32(ServiceCallLogModel.ServiceLogTypeListId);
            ServiceCallLog.SpokeWith = ServiceCallLogModel.SpokeWith;
            ServiceCallLog.Action = ServiceCallLogModel.Action;
            ServiceCallLog.Comments = ServiceCallLogModel.Comments;
            ServiceCallLog.StatusResultListId = Convert.ToInt32(ServiceCallLogModel.StatusResultListId);

            if (ServiceCallLogModel.CallBack != null)
            {
                //DateTime dtCallBack = DateTime.ParseExact(ServiceCallLogModel.CallBack.ToString(), "dd-MM-yyyy", System.Globalization.CultureInfo.InstalledUICulture);
                //ServiceCallLog.CallBack = dtCallBack;                
                ServiceCallLog.CallBack = ServiceCallLogModel.CallBack;
            }
            ServiceCallLog.FollowUpBy = ServiceCallLogModel.FollowUpBy;
            ServiceCallLog.EmailNotesTo = ServiceCallLogModel.EmailNotesTo;
            ServiceCallLog.CallDate = DateTime.Now;
            ServiceCallLog.CallTime = DateTime.Now.TimeOfDay;
            SaveServiceCallLog(ServiceCallLog);
            return ServiceCallLog.ServiceCallLogId;
        }

        #endregion
        // ======================================================================================

        public int UpdateApproveReject(int CustomerId, string Note, int StatusListId, string valIds)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var Customer = GetCustomerById(CustomerId);
            if (Customer != null)
            {
                int StatusId = (Customer.StatusId.HasValue ? Customer.StatusId.Value : 0);
                if (StatusId > 0)
                {
                    var StatusData = GetStatusById(StatusId);
                    if (StatusData != null)
                    {
                        #region :: Customer ::

                        // Update Status as Inactive                         
                        StatusData.IsActive = false;
                        SaveStatus(StatusData);

                        // Add New Status as Approve/Reject
                        Status Statusmodel = new Status();
                        Statusmodel.ClassId = CustomerId;
                        Statusmodel.StatusListId = StatusListId;
                        Statusmodel.StatusDate = DateTime.Now;
                        Statusmodel.StatusNotes = Note;
                        Statusmodel.IsActive = true;
                        Statusmodel.CreatedBy = LoginUserId.ToString();
                        Statusmodel.CreatedDate = DateTime.Now;
                        Statusmodel.TypeListId = (int)Business.Enumeration.TypeList.Customer;
                        Uow.Status.Add(Statusmodel);
                        Uow.Commit();

                        //Update Customer Status
                        Customer.StatusId = Statusmodel.StatusId;
                        Customer.StatusListId = StatusListId;
                        Customer.IsActive = true;

                        if (StatusListId == 1) // active, so customer is being approved
                        {
                            Customer.ApprovedBy = this.LoginUserId;
                            Customer.ApprovedDate = DateTime.Now;
                        }

                        Uow.Customer.Update(Customer);
                        Uow.Commit();

                        #endregion 

                        #region :: Contract Details Update ::

                        // if status active then update contract status
                        if (StatusListId == 1)
                        {
                            var ContractData = GetContract();
                            if (ContractData != null && ContractData.Count() > 0)
                            {
                                var Contract = ContractData.Where(w => w.CustomerId == CustomerId);
                                if (Contract != null)
                                {
                                    // Add new record for Contract  
                                    Status contractStatus = new Status();
                                    contractStatus.ClassId = Contract.FirstOrDefault().ContractId;
                                    contractStatus.StatusListId = StatusListId;
                                    contractStatus.StatusDate = DateTime.Now;
                                    contractStatus.StatusNotes = Note;
                                    contractStatus.IsActive = true;
                                    contractStatus.CreatedBy = LoginUserId.ToString();
                                    contractStatus.CreatedDate = DateTime.Now;
                                    contractStatus.TypeListId = (int)Business.Enumeration.TypeList.Contract;
                                    Uow.Status.Add(contractStatus);
                                    Uow.Commit();

                                    //Update Contract record
                                    var RowData = Contract.FirstOrDefault();
                                    RowData.StatusListId = StatusListId;
                                    RowData.StatusId = contractStatus.StatusId;
                                    RowData.CommissionTransactionStatusListId = (RowData.CommissionTransactionStatusListId != 4 ? 1 : RowData.CommissionTransactionStatusListId);
                                    Uow.Contract.Update(RowData);
                                    Uow.Commit();
                                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                                    {
                                        List<MaintenanceTemp> lstMaintenanceTemp = context.MaintenanceTemps.Where(o => o.ClassId == CustomerId && o.TypeListId == 1 && o.IsActive == true && o.MaintenanceTypeListId == 7).ToList();
                                        foreach (MaintenanceTemp o in lstMaintenanceTemp)
                                        {
                                            var parmas = new DynamicParameters();
                                            parmas.Add("@MaintenanceTempId", o.MaintenanceTempId);
                                            parmas.Add("@UserId", LoginUserId);
                                            conn.Query("dbo.portal_spCreate_C_CustomerFranchiseeDistributionApproval", parmas, commandType: CommandType.StoredProcedure);
                                        }
                                    }
                                }
                            }

                            var DistributionData = GetDistribution();
                            if (DistributionData != null && DistributionData.Count() > 0)
                            {
                                var Distribution = DistributionData.Where(w => w.CustomerId == CustomerId);
                                if (Distribution != null)
                                {
                                    //Update Distribution record
                                    var RowData = Distribution.FirstOrDefault();
                                    if (RowData != null)
                                    {
                                        RowData.StatusListId = StatusListId;
                                        RowData.StatusId = Statusmodel.StatusId;
                                        Uow.Distribution.Update(RowData);
                                        Uow.Commit();
                                    }
                                }
                            }

                            var FFData = GetFinderFee();
                            if (FFData != null && FFData.Count() > 0)
                            {
                                var ff = FFData.Where(w => w.CustomerId == CustomerId);
                                if (ff != null)
                                {
                                    //Update Distribution record
                                    var RowData = ff.FirstOrDefault();
                                    if (RowData != null)
                                    {
                                        RowData.StatusListId = 31;
                                        RowData.StatusId = Statusmodel.StatusId;
                                        Uow.FinderFee.Update(RowData);
                                        Uow.Commit();
                                    }

                                }
                            }
                        }

                        #endregion 

                        #region :: validation update :: 

                        if (valIds != "")
                        {
                            string[] arrId = valIds.Split(',');
                            foreach (var item in arrId)
                            {
                                if (item != "")
                                {
                                    Validation Validationmodel = new Validation();
                                    Validationmodel.ClassId = CustomerId;
                                    Validationmodel.ValidationItemId = Convert.ToInt32(item);
                                    Validationmodel.TypeListId = (int)Business.Enumeration.TypeList.Customer;
                                    Validationmodel.StatusListId = StatusListId;
                                    Validationmodel.StatusId = Statusmodel.StatusId;
                                    Validationmodel.IsActive = true;
                                    Validationmodel.CreatedBy = LoginUserId.ToString();
                                    Validationmodel.CreatedDate = DateTime.Now;
                                    Uow.Validation.Add(Validationmodel);
                                    Uow.Commit();
                                }
                            }
                        }


                        #endregion
                    }
                }
            }
            return CustomerId;
        }
        public Status UpdateCustomerStatus(Status model)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();

            model.IsActive = true;
            model.CreatedBy = LoginUserId.ToString();
            model.CreatedDate = DateTime.Now;
            model.TypeListId = (int)Business.Enumeration.TypeList.Customer;
            Uow.Status.Add(model);
            Uow.Commit();

            var Customer = GetCustomerById(Convert.ToInt32(model.ClassId));
            if (Customer != null)
            {
                Customer.StatusId = model.StatusId;
                Customer.StatusListId = model.StatusListId;
                Uow.Customer.Update(Customer);
                Uow.Commit();
            }
            return model;
        }

        public int UpdatePendingStatus(int CustomerId, string Note, int StatusListId)
        {
            var StatusData = GetStatus();
            if (StatusData != null)
            {
                var model = StatusData.Where(w => w.ClassId == CustomerId && w.StatusListId == StatusListId);
                if (model != null)
                {
                    var data = model.FirstOrDefault();
                    data.StatusNotes = Note;
                    SaveStatus(data);
                }
            }
            return CustomerId;
        }

        public int UpdateRejectCustomerMaintenance(int MaintenanceTempId, string Reason, int MaintenanceTypeListId)
        {
            CustomerMaintenanceApproval oApproval = new CustomerMaintenanceApproval();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                MaintenanceTemp Maintenance = context.MaintenanceTemps.Where(x => x.MaintenanceTempId == MaintenanceTempId).FirstOrDefault();
                if (Maintenance != null)
                {
                    Maintenance.Reason = Reason;
                    Maintenance.MaintenanceTypeListId = MaintenanceTypeListId;
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        MaintenanceTempId = 0;
                    }
                }
            }
            return MaintenanceTempId;
        }


        public int UpdateApprovalCustomerMaintenance(int MaintenanceTempId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                MaintenanceTemp Maintenance = context.MaintenanceTemps.Where(x => x.MaintenanceTempId == MaintenanceTempId).FirstOrDefault();
                if (Maintenance != null)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@MaintenanceTempId", MaintenanceTempId);
                            parmas.Add("@CreatedBy", LoginUserId.ToString());
                            conn.Query("dbo.AcceptCustomerMaintenanceSuspension", parmas, commandType: CommandType.StoredProcedure);
                        }
                    }
                    catch (Exception e)
                    {
                        MaintenanceTempId = 0;
                    }
                }

            }
            return MaintenanceTempId;
        }



        public int UpdateStatus(int CustomerId, string Note, int OldStatusListId, int NewStatusListId, string valIds)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var Customer = GetCustomerById(CustomerId);
            if (Customer != null)
            {
                int StatusId = (Customer.StatusId.HasValue ? Customer.StatusId.Value : 0);
                if (StatusId > 0)
                {
                    var StatusData = GetStatusById(StatusId);
                    if (StatusData != null)
                    {
                        #region :: Customer ::

                        // Update Status as Inactive                         
                        StatusData.IsActive = false;
                        SaveStatus(StatusData);

                        // Add New Status as Approve/Reject
                        Status Statusmodel = new Status();
                        Statusmodel.ClassId = CustomerId;
                        Statusmodel.StatusListId = NewStatusListId;
                        Statusmodel.StatusDate = DateTime.Now;
                        Statusmodel.StatusNotes = Note;
                        Statusmodel.IsActive = true;
                        Statusmodel.CreatedBy = LoginUserId.ToString();
                        Statusmodel.CreatedDate = DateTime.Now;
                        Statusmodel.TypeListId = (int)Business.Enumeration.TypeList.Customer;
                        Uow.Status.Add(Statusmodel);
                        Uow.Commit();

                        //Updat Customer Status
                        Customer.StatusId = Statusmodel.StatusId;
                        Customer.StatusListId = NewStatusListId;
                        Customer.IsActive = true;
                        Uow.Customer.Update(Customer);
                        Uow.Commit();

                        #endregion

                        var ContractData = GetContract();
                        if (ContractData != null && ContractData.Count() > 0)
                        {
                            var Contract = ContractData.Where(w => w.CustomerId == CustomerId);
                            if (Contract != null)
                            {
                                //Update Contract record
                                var RowData = Contract.FirstOrDefault();
                                RowData.StatusListId = NewStatusListId;
                                RowData.StatusId = Statusmodel.StatusId;
                                Uow.Contract.Update(RowData);
                                Uow.Commit();

                            }
                        }

                        var DistributionData = GetDistribution();
                        if (DistributionData != null && DistributionData.Count() > 0)
                        {
                            var Distribution = DistributionData.Where(w => w.CustomerId == CustomerId).FirstOrDefault();
                            if (Distribution != null)
                            {
                                //Update Distribution record
                                var RowData = Distribution;
                                RowData.StatusListId = NewStatusListId;
                                RowData.StatusId = Statusmodel.StatusId;
                                Uow.Distribution.Update(RowData);
                                Uow.Commit();

                            }
                        }

                        if (NewStatusListId == 4)
                        {
                            var FFData = GetFinderFee();
                            if (FFData != null && FFData.Count() > 0)
                            {
                                var ff = FFData.Where(w => w.CustomerId == CustomerId).FirstOrDefault();
                                if (ff != null)
                                {
                                    //Update Distribution record
                                    var RowData = ff;
                                    RowData.StatusListId = 34;
                                    RowData.StatusId = Statusmodel.StatusId;
                                    Uow.FinderFee.Update(RowData);
                                    Uow.Commit();

                                }
                            }
                        }

                        #region :: validation update :: 

                        if (!string.IsNullOrEmpty(valIds?.Trim()))
                        {
                            string[] arrId = valIds.Split(',');
                            foreach (var item in arrId)
                            {
                                if (item != "")
                                {
                                    Validation Validationmodel = new Validation();
                                    Validationmodel.ClassId = CustomerId;
                                    Validationmodel.ValidationItemId = Convert.ToInt32(item);
                                    Validationmodel.TypeListId = (int)Business.Enumeration.TypeList.Customer;
                                    Validationmodel.StatusListId = NewStatusListId;
                                    Validationmodel.StatusId = Statusmodel.StatusId;
                                    Validationmodel.IsActive = true;
                                    Validationmodel.CreatedBy = LoginUserId.ToString();
                                    Validationmodel.CreatedDate = DateTime.Now;
                                    Uow.Validation.Add(Validationmodel);
                                    Uow.Commit();
                                }
                            }
                        }


                        #endregion
                    }
                }
            }
            return CustomerId;
        }

        public int UpdateStatusToNextStep(int CustomerId, string Note, int StatusListId, string valIds)
        {
            int NewStatusListId = 0;

            if (StatusListId == (int)JKApi.Business.Enumeration.CustomerStatusList.RegionOperation)
            {
                NewStatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.RegionAccounting;

            }
            else if (StatusListId == (int)JKApi.Business.Enumeration.CustomerStatusList.RegionAccounting)
            {
                NewStatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Pending;

                //using (jkDatabaseEntities context = new jkDatabaseEntities())
                //{

                //    List<FindersFee> lstFindersFee = context.FindersFees.Where(c => c.CustomerId == CustomerId).ToList();
                //    // Add New Status as Approve/Reject
                //    //Status Statusmodel = new Status();
                //    //Statusmodel.ClassId = CustomerId;
                //    //Statusmodel.StatusListId = NewStatusListId;
                //    //Statusmodel.StatusDate = DateTime.Now;
                //    //Statusmodel.StatusNotes = Note;
                //    //Statusmodel.IsActive = true;
                //    //Statusmodel.CreatedBy = LoginUserId.ToString();
                //    //Statusmodel.CreatedDate = DateTime.Now;
                //    //Statusmodel.TypeListId = (int)Business.Enumeration.TypeList.Customer;
                //    //Uow.Status.Add(Statusmodel);
                //    //Uow.Commit();
                //    foreach(FindersFee o in lstFindersFee)
                //    {
                //        //o.StatusId = Statusmodel.StatusId;
                //        o.StatusListId = 34;
                //        o.ModifiedBy = LoginUserId;
                //        o.ModifiedDate = DateTime.Now;
                //        //o.IsActive = true;
                //    }
                //    context.SaveChanges();

                //}



            }

            return UpdateStatus(CustomerId, Note, StatusListId, NewStatusListId, valIds);
        }

        public void UpdateCustomerInactive(int customerId, int userId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Data.DAL.Contract oContract = context.Contracts.Where(cn => cn.CustomerId == customerId && cn.isActive == true).FirstOrDefault();
                oContract.isActive = false;

                List<Data.DAL.ContractDetail> lstContractDetail = context.ContractDetails.Where(cn => cn.ContractId == oContract.ContractId && cn.IsActive == true).ToList();
                foreach (var item in lstContractDetail)
                {
                    item.IsActive = false;
                }

                context.SaveChanges();
            }
        }

        public void CreateCustomerInvoice(int customerId, int regionId, int userId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //   context.portal_spCreate_AR_GenerateInvoiceByCustomer(customerId, regionId, DateTime.Now.Month, DateTime.Now.Year, userId); 
            }
        }


        #region :: Customer Reports  :: 
        //Distribution Reports  
        public List<DistributionReportViewModel> DistributionReportList(string searchtext, int status, string regionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<DistributionReportViewModel> lstDistributionReport = new List<DistributionReportViewModel>();
                var DistributionReportData = context.portal_spGet_C_DistributionReport(regionId, searchtext, Convert.ToInt32(status)).ToList();
                if (DistributionReportData != null)
                {
                    foreach (portal_spGet_C_DistributionReport_Result item in DistributionReportData)
                    {
                        lstDistributionReport.Add(new DistributionReportViewModel()
                        {
                            DistributionId = item.DistributionId,
                            CustomerNo = item.CustomerNo,
                            CustomerName = item.CustomerName,
                            FranchiseeNo = item.FranchiseeNo,
                            FranchiseeName = item.FranchiseeName,
                            ContractDescription = item.ContractDescription,
                            Notes = item.Notes,
                            Amount = item.Amount,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate
                        });
                    }
                }
                return lstDistributionReport;
            }
        }

        //New Customer
        public List<portal_spGet_C_GetNewCustomersReport_Result> NewCustomersReportList(string searchtext, int status, string sdata, string edate, int regionId, string regionIds)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_C_GetNewCustomersReport_Result> lstNewCustomersReportResult = new List<portal_spGet_C_GetNewCustomersReport_Result>();
                lstNewCustomersReportResult = context.portal_spGet_C_GetNewCustomersReport(searchtext, status, sdata, edate, regionId, regionIds).ToList();
                return lstNewCustomersReportResult;
            }
        }

        public IEnumerable<CustomerMaintenanceViewModel> GetRegionWiseCustomer(string customerName)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = "EXEC portal_spGet_C_CustomerSearchByNameOrNumber @CustomerName, @RegionId";

                customerName = String.IsNullOrEmpty(customerName) ? "%%" : "%" + customerName.ToLower() + "%";

                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<CustomerMaintenanceViewModel>(query.ToString(), new
                {
                    CustomerName = customerName,
                    RegionId = regionId
                });
            }
        }

        public IEnumerable<CustomerDetailTransactionWithStatusViewModel> GetAllCustomerTransactions(int? customerId, int? typeId, DateTime? startDate, DateTime? endDate, int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_CustomerAllTransactions @CustomerId,@RegionId,@LTypeId,@LStartDate,@LEndDate,@BillMonth,@BillYear";
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<CustomerDetailTransactionWithStatusViewModel>(query, new
                {
                    CustomerId = customerId,
                    RegionId = regionId > 0 ? regionId : null,
                    LTypeId = typeId > 0 ? typeId : null,
                    LStartDate = startDate,
                    LEndDate = endDate,
                    BillMonth = month,
                    BillYear = year
                });
            }
        }


        #endregion

        #region :: Customer Account History ::

        public CustomerAccountHistoryViewModel GetCustomerAccountHistory(int customerId, DateTime startDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var vm = new CustomerAccountHistoryViewModel();

                vm.Customer = GetCustomerN(customerId);

                var results = context.portal_spGet_C_CustomerInvoiceHistory(customerId, startDate).ToList();

                if (results == null)
                    return vm;

                vm.Franchisees = new List<CustomerAccountHistoryPerFranchiseeViewModel>();

                vm.InvoiceTotal = 0;
                vm.OtherTrxTotal = 0;
                vm.TaxTotal = 0;
                vm.AmountDue = 0;

                foreach (var franchiseeId in results.Select(o => o.FranchiseeId).Distinct())
                {
                    var franchiseeRows = results.Where(o => o.FranchiseeId == franchiseeId).ToList();

                    if (franchiseeRows.Count == 0)
                        continue;

                    var franchisee = new CustomerAccountHistoryPerFranchiseeViewModel();
                    franchisee.FranchiseeNo = franchiseeRows[0].FranchiseeNo;
                    franchisee.FranchiseeName = franchiseeRows[0].FranchiseeName;
                    franchisee.Invoices = new List<CustomerAccountHistoryPerInvoiceViewModel>();

                    foreach (var invoiceId in franchiseeRows.Select(o => o.InvoiceId).Distinct())
                    {
                        var invoiceRows = franchiseeRows.Where(o => o.InvoiceId == invoiceId).ToList();

                        if (invoiceRows.Count == 0)
                            continue;

                        var invoice = new CustomerAccountHistoryPerInvoiceViewModel();
                        invoice.Balance = (decimal)invoiceRows[0].BalanceTotal;
                        invoice.Payments = new List<CustomerAccountHistoryPerTransactionViewModel>();
                        invoice.Credits = new List<CustomerAccountHistoryPerTransactionViewModel>();

                        foreach (var details in invoiceRows)
                        {
                            var trxDetail = new CustomerAccountHistoryPerTransactionViewModel();
                            trxDetail.Date = (DateTime)details.TrxDate;
                            trxDetail.Amount = (decimal)details.Total;
                            trxDetail.Description = details.Description;

                            if (details.MasterTrxTypeListId == 1) // base invoice MTD
                            {
                                trxDetail.TrxNumber = details.InvoiceNo;

                                invoice.Invoice = trxDetail;

                                vm.InvoiceTotal += trxDetail.Amount;
                            }
                            else // other transactions
                            {
                                if (details.MasterTrxTypeListId == 2)
                                    invoice.Payments.Add(trxDetail);
                                else
                                    invoice.Credits.Add(trxDetail);

                                vm.OtherTrxTotal += trxDetail.Amount;
                            }

                            vm.TaxTotal += (decimal)details.TotalTax;
                        }

                        vm.AmountDue += invoice.Balance;

                        franchisee.Invoices.Add(invoice);
                    }

                    vm.Franchisees.Add(franchisee);
                }

                return vm;
            }
        }

        #endregion

        #region :: CRM Upload Document ::

        public IEnumerable<JKViewModels.CRM.CRMDocumentViewModel> GetCRMDocumentByAccountCustomerDetailId(int AccountCustomerDetailId, int TypeListId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_C_GetCRM_Document @CRM_AccountCustomerDetailId,@TypeListId";

                return conn.Query<JKViewModels.CRM.CRMDocumentViewModel>(query, new
                {
                    CRM_AccountCustomerDetailId = AccountCustomerDetailId,
                    TypeListId = TypeListId
                });
            }
        }

        public IEnumerable<JKViewModels.CRM.CRMDocumentViewModel> GetCRMDocumentByAccountFranchiseDetailId(int AccountFranchiseDetailId, int TypeListId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_F_GetCRM_Document @CRM_AccountFranchiseDetailId,@TypeListId";

                return conn.Query<JKViewModels.CRM.CRMDocumentViewModel>(query, new
                {
                    CRM_AccountFranchiseDetailId = AccountFranchiseDetailId,
                    TypeListId = TypeListId
                });
            }
        }



        #endregion

        #region :: Account Offring ::

        public CommonAccountOfferingViewModel GetAccountOfferingResult(int CustomerId, string FranchiseeType, decimal Distance, string sDate, string eDate)
        {
            CommonAccountOfferingViewModel oAccountOfferingResult = new CommonAccountOfferingViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                parmas.Add("@FranchiseeType", FranchiseeType);
                parmas.Add("@Distance", Distance);
                parmas.Add("@StartDate", sDate);
                parmas.Add("@EndDate", eDate);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_GetAccountOfferingResult", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oAccountOfferingResult.CustomerDetail = multipleresult.Read<AccountOfferingCustomerDetailViewModel>().ToList().FirstOrDefault();
                        oAccountOfferingResult.lstAccountOfferingResult = multipleresult.Read<AccountOfferingResultViewModel>().ToList();
                    }
                    else
                    {
                        oAccountOfferingResult.CustomerDetail = new AccountOfferingCustomerDetailViewModel();
                        oAccountOfferingResult.lstAccountOfferingResult = new List<AccountOfferingResultViewModel>();
                    }
                }

            }
            return oAccountOfferingResult;
        }

        public List<AccountOfferingResponceViewModel> InsertOfferingData(int CustomerId, string FranchiseeIds, DateTime exDate, DateTime extime, string note)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@UserId", LoginUserId);
                parmas.Add("@FranchiseeIds", FranchiseeIds);
                parmas.Add("@CustomerId", CustomerId);
                parmas.Add("@ExpireDate", exDate);
                parmas.Add("@ExpireTime", extime);
                parmas.Add("@SpNote", note);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_InsertOffering", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<AccountOfferingResponceViewModel>().ToList();
                    }
                }

            }
            return new List<AccountOfferingResponceViewModel>();
        }

        public Email GetEmailAddrress(int classid, int typelistid)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@ClassIds", classid);
                parmas.Add("@TypeListId", typelistid);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_EmailsbyClassId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<Email>().ToList().FirstOrDefault();
                    }
                    else
                        return new Email();
                }

            }
            //using (var context = new jkDatabaseEntities())
            //{
            //    var ob = context.Emails.ToList().FirstOrDefault();
            //    return context.Emails.Where(o => o.ClassId == classid && o.TypeListId == typelistid && o.IsActive == true).ToList().FirstOrDefault();

            //}
        }


        public List<CRMAccountOfferingListViewModel> GetAccountOfferingData(string regionid, string statusid)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@Region", regionid);
                parmas.Add("@Status", statusid);
                using (var multipleresult = conn.QueryMultiple("dbo.crm_spGet_GetOfferingList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CRMAccountOfferingListViewModel>().ToList();
                    }
                }

            }
            return new List<CRMAccountOfferingListViewModel>();
        }

        public List<CRMAccountOfferingListViewModel> GetAccountOfferingDataWithFranchiseeId(int FranchiseeId)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", FranchiseeId);
                using (var multipleresult = conn.QueryMultiple("dbo.crm_spGet_GetOfferingListWithFranchiseeId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CRMAccountOfferingListViewModel>().ToList();
                    }
                }

            }
            return new List<CRMAccountOfferingListViewModel>();
        }


        #endregion

        #region :: Validation ::
        public IQueryable<Validation> GetValidationList()
        {
            var qry = Uow.Validation.GetAll();
            return qry;
        }

        public List<Validation> GetValidationByClassId(int ClassId, int StatusListId)
        {
            var qry = Uow.Validation.GetAll().Where(x => x.ClassId == ClassId && x.StatusListId == StatusListId && x.IsActive == true).ToList();
            return qry;
        }

        public Validation GetValidationListById(int id)
        {
            return Uow.Validation.GetById(id);
        }

        public IQueryable<ValidationItem> GetValidationItemList()
        {
            var qry = Uow.ValidationItem.GetAll();
            return qry;
        }

        public ValidationItem GetValidationItemListById(int id)
        {
            return Uow.ValidationItem.GetById(id);
        }


        public List<ValidationItem> ValidationItemListStatus(int StatusListId, int TypeListId)
        {
            var qry = Uow.ValidationItem.GetAll();
            if (qry != null)
            {
                return qry.Where(w => w.StatusListID == StatusListId && w.TypeListId == TypeListId).ToList();
            }
            return null;
        }

        #endregion

        #region :: CSActivity ::

        public IQueryable<CSActivity> GetCSActivityList()
        {
            var qry = Uow.CSActivity.GetAll();
            return qry;
        }

        public List<CSActivity> GetCSActivityByClassId(int ClassId, int TypeListId)
        {
            var qry = Uow.CSActivity.GetAll().Where(x => x.ClassId == ClassId && x.TypeListId == TypeListId && x.IsActive == true).ToList();
            return qry;
        }

        public CSActivity GetCSActivityListById(int id)
        {
            return Uow.CSActivity.GetById(id);
        }

        public IQueryable<CSstage> GetCSstageItemList()
        {
            var qry = Uow.CSstage.GetAll();
            return qry;
        }

        public CSstage GetCSstageListById(int id)
        {
            return Uow.CSstage.GetById(id);
        }

        public List<CSstage> CSstageListStatus(int StatusListId, int TypeListId, int CustomerId)
        {
            var qry = Uow.CSstage.GetAll();
            if (qry != null)
            {
                return qry.Where(w => w.ClassId == CustomerId && w.StatusListId == StatusListId && w.TypeListId == TypeListId && w.IsActive == true).ToList();
            }
            return null;
        }
        public List<CSstage> CSstageListWithCustomerWise(int TypeListId, int CustomerId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var qry = context.CSstages.Where(w => w.ClassId == CustomerId && w.TypeListId == TypeListId && w.IsActive == true).ToList();
            if (qry != null)
            {
                return qry;
            }
            return null;
        }

        //Activity stage 1 : save data 
        public int SaveCSActivityNotifytheFranchiseeOwneData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem5 != -1)
            {
                #region -: Item5 :-
                var Item5 = ItmDataModel.Where(g => g.StaticDesignItemNo == 5);
                if (Item5 != null && Item5.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item5.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 5;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }

            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }
        
        //Activity stage 2 : save data 
        public int SaveCSActivityContacttheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3Date != "")
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }

            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }

            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        //Activity stage 3 : save data 
        public int SaveCSActivityInspecttheAccountData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != "")
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-

                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        //Activity stage 4 : save data 
        public int SaveCSActivityDefectnotifytoFranchiseeData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        //Activity stage 5 : save data 
        public int SaveCSActivityLetertotheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem1note, int optitem2, int optitem3, string optitem4Date, string optitem4Time, string optitem4endTime)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem1 == 0 ? optitem1note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem1 == 0 ? optitem1note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            if (optitem4Date != "")
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem4Date);
                    if (optitem4Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem4Time);
                    }
                    if (optitem4endTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem4endTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem4Date);
                    if (optitem4Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem4Time);
                    }
                    if (optitem4endTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem4endTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }

            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        //Activity stage 6 : save data 
        public int SaveCSActivityReInspecttherAccountData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem1Note, int optitem2, int optitem3, string optitem3Note)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem1 == 0 ? optitem1Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem1 == 0 ? optitem1Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem3 == 0 ? optitem3Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem3 == 0 ? optitem3Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        //Activity stage 7 : save data 
        public int SaveCSActivityFollowupBackontrackData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Item Update :-

            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            #endregion

            int Id = 0;
            Id = SaveCSActivityData(CustomerId, StagId, CSstageId, Note, valIds);
            return Id;
        }

        public int SaveCSActivityData(int CustomerId, int StagId, int CSstageId, string Note, string valIds)
        {
            #region :: CSActivity update :: 

            if (valIds != "")
            {
                string[] arrId = valIds.Split(',');
                foreach (var item in arrId)
                {
                    if (item != "")
                    {
                        int vlId = Convert.ToInt32(item);
                        var DataModel = GetCSActivityList().Where(w => w.ClassId == CustomerId && w.StatusListId == StagId && w.ValidationItemId == vlId);
                        if (DataModel != null && DataModel.Count() > 0)
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel = DataModel.FirstOrDefault();
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.ModifiedDate = DateTime.Now;
                            CSActivitymodel.ModifiedBy = LoginUserId;
                            Uow.CSActivity.Update(CSActivitymodel);
                            Uow.Commit();
                        }
                        else
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel.ClassId = CustomerId;
                            CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                            CSActivitymodel.ValidationItemId = Convert.ToInt32(item);
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StatusListId = StagId;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.CreatedBy = LoginUserId;
                            CSActivitymodel.CreatedDate = DateTime.Now;
                            Uow.CSActivity.Add(CSActivitymodel);
                            Uow.Commit();
                        }
                    }
                }
            }
            #endregion

            #region :: CSstage :: 

            int Id = 0;
            var CSstageData = GetCSstageListById(CSstageId);
            if (CSstageData != null)
            {
                Id = CSstageData.CSstageId;
                CSstageData.Note = Note;
                CSstageData.IsActive = true;
                CSstageData.ModifiedDate = DateTime.Now;
                CSstageData.ModifiedBy = LoginUserId;
                Uow.CSstage.Update(CSstageData);
                Uow.Commit();
            }
            else
            {

                CSstage CSstagemodal = new CSstage();
                CSstagemodal.ClassId = CustomerId;
                CSstagemodal.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
                CSstagemodal.StageNo = StagId;
                CSstagemodal.StatusListId = StagId;
                CSstagemodal.Note = Note;
                CSstagemodal.IsActive = true;
                CSstagemodal.CreatedBy = LoginUserId;
                CSstagemodal.CreatedDate = DateTime.Now;
                Uow.CSstage.Add(CSstagemodal);
                Uow.Commit();
                Id = CSstagemodal.CSstageId;
            }


            #endregion

            #region :: update Customer Status :: 

            jkDatabaseEntities context = new jkDatabaseEntities();
            var _data = context.ServiceCallLogs.Where(w => w.ServiceCallLogId == CustomerId);
            if (_data != null && _data.Count() > 0)
            {
                JKApi.Data.DAL.ServiceCallLog model = new JKApi.Data.DAL.ServiceCallLog();
                model = _data.FirstOrDefault();
                model.StatusListId = StagId;
                context.SaveChanges();
            }
            //var StatusDataa = context.Status.Where(w => w.StatusListId == StagId && w.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending);
            //if (StatusDataa == null || StatusDataa.Count() == 0)
            //{
            //    Status oStatus = new Status();
            //    oStatus.ClassId = CustomerId;
            //    oStatus.CreatedBy = LoginUserId.ToString();
            //    oStatus.CreatedDate = DateTime.Now;
            //    oStatus.IsActive = true;
            //    oStatus.StatusDate = DateTime.Now;
            //    oStatus.TypeListId = (int)Business.Enumeration.TypeList.CancellationPending;
            //    oStatus.StatusListId = StagId;
            //    Uow.Status.Add(oStatus);
            //    Uow.Commit();

            //    var CustModel = GetCustomerById(CustomerId);
            //    if (CustModel != null)
            //    {
            //        CustModel.StatusId = oStatus.StatusId;
            //        CustModel.StatusListId = oStatus.StatusListId;
            //        Uow.Customer.Update(CustModel);
            //        Uow.Commit();
            //    }
            //}
            #endregion

            return Id;
        }

        public List<CustomerCancellationActivityModel> GetCustomerCancellationActivityDetails(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CustomerCancellationActivityModel> ListData = (from tbcsact in context.CSActivities
                                                                    join tbstatuslist in context.StatusLists on tbcsact.StatusListId equals tbstatuslist.StatusListId
                                                                    join tbvalitm in context.ValidationItems on tbcsact.ValidationItemId equals tbvalitm.ValidationItemId into tempJoin
                                                                    from t2 in tempJoin.DefaultIfEmpty()
                                                                    join tbluser in context.AuthUserLogins on tbcsact.CreatedBy equals tbluser.UserId
                                                                    where tbcsact.IsActive == true && tbcsact.ClassId == CustomerId && tbcsact.TypeListId == (int)Business.Enumeration.TypeList.CancellationPending
                                                                    select new CustomerCancellationActivityModel
                                                                    {
                                                                        CSActivityId = tbcsact.CSActivityId,
                                                                        ClassId = tbcsact.ClassId,
                                                                        TypeListId = tbcsact.TypeListId,
                                                                        ValidationItemId = tbcsact.ValidationItemId,
                                                                        IsItemChecked = tbcsact.IsItemChecked,
                                                                        StatusListId = tbcsact.StatusListId,
                                                                        StageName = tbstatuslist.Name,
                                                                        ItemOptionName = t2.Name,
                                                                        CreatedDate = tbcsact.CreatedDate,
                                                                        CreatedBy = tbluser.FirstName + " " + tbluser.LastName,

                                                                        IsStaticDesign = tbcsact.IsStaticDesign,
                                                                        StaticDesignItemNo = tbcsact.StaticDesignItemNo,
                                                                        StaticDesignIsItemChecked = tbcsact.StaticDesignIsItemChecked,
                                                                        StaticDesignNote = tbcsact.StaticDesignNote,
                                                                        StaticDesignScheduleDate = tbcsact.StaticDesignScheduleDate,
                                                                        StaticDesignScheduleTime = tbcsact.StaticDesignScheduleTime,
                                                                        StaticDesignScheduleEndTime = tbcsact.StaticDesignScheduleEndTime
                                                                    }).ToList();
                return ListData;
            }
        }

        public List<CustomerCancellationActivityModel> GetCustomerComplaintActivityDetails(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CustomerCancellationActivityModel> ListData = (from tbcsact in context.CSActivities
                                                                    join tbstatuslist in context.StatusLists on tbcsact.StatusListId equals tbstatuslist.StatusListId
                                                                    join tbvalitm in context.ValidationItems on tbcsact.ValidationItemId equals tbvalitm.ValidationItemId into tempJoin
                                                                    from t2 in tempJoin.DefaultIfEmpty()
                                                                    join tbluser in context.AuthUserLogins on tbcsact.CreatedBy equals tbluser.UserId
                                                                    where tbcsact.IsActive == true && tbcsact.ClassId == CustomerId && tbcsact.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage
                                                                    select new CustomerCancellationActivityModel
                                                                    {
                                                                        CSActivityId = tbcsact.CSActivityId,
                                                                        ClassId = tbcsact.ClassId,
                                                                        TypeListId = tbcsact.TypeListId,
                                                                        ValidationItemId = tbcsact.ValidationItemId,
                                                                        IsItemChecked = tbcsact.IsItemChecked,
                                                                        StatusListId = tbcsact.StatusListId,
                                                                        StageName = tbstatuslist.Name,
                                                                        ItemOptionName = t2.Name,
                                                                        CreatedDate = tbcsact.CreatedDate,
                                                                        CreatedBy = tbluser.FirstName + " " + tbluser.LastName,

                                                                        IsStaticDesign = tbcsact.IsStaticDesign,
                                                                        StaticDesignItemNo = tbcsact.StaticDesignItemNo,
                                                                        StaticDesignIsItemChecked = tbcsact.StaticDesignIsItemChecked,
                                                                        StaticDesignNote = tbcsact.StaticDesignNote,
                                                                        StaticDesignScheduleDate = tbcsact.StaticDesignScheduleDate,
                                                                        StaticDesignScheduleTime = tbcsact.StaticDesignScheduleTime,
                                                                        StaticDesignScheduleEndTime = tbcsact.StaticDesignScheduleEndTime
                                                                    }).ToList();
                return ListData;
            }
        }
        public List<CustomerCancellationActivityModel> GetCustomerFailedInspectionActivityDetails(int CustomerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CustomerCancellationActivityModel> ListData = (from tbcsact in context.CSActivities
                                                                    join tbstatuslist in context.StatusLists on tbcsact.StatusListId equals tbstatuslist.StatusListId
                                                                    join tbvalitm in context.ValidationItems on tbcsact.ValidationItemId equals tbvalitm.ValidationItemId into tempJoin
                                                                    from t2 in tempJoin.DefaultIfEmpty()
                                                                    join tbluser in context.AuthUserLogins on tbcsact.CreatedBy equals tbluser.UserId
                                                                    where tbcsact.IsActive == true && tbcsact.ClassId == CustomerId && tbcsact.TypeListId == (int)Business.Enumeration.TypeList.FailedInspection
                                                                    select new CustomerCancellationActivityModel
                                                                    {
                                                                        CSActivityId = tbcsact.CSActivityId,
                                                                        ClassId = tbcsact.ClassId,
                                                                        TypeListId = tbcsact.TypeListId,
                                                                        ValidationItemId = tbcsact.ValidationItemId,
                                                                        IsItemChecked = tbcsact.IsItemChecked,
                                                                        StatusListId = tbcsact.StatusListId,
                                                                        StageName = tbstatuslist.Name,
                                                                        ItemOptionName = t2.Name,
                                                                        CreatedDate = tbcsact.CreatedDate,
                                                                        CreatedBy = tbluser.FirstName + " " + tbluser.LastName,

                                                                        IsStaticDesign = tbcsact.IsStaticDesign,
                                                                        StaticDesignItemNo = tbcsact.StaticDesignItemNo,
                                                                        StaticDesignIsItemChecked = tbcsact.StaticDesignIsItemChecked,
                                                                        StaticDesignNote = tbcsact.StaticDesignNote,
                                                                        StaticDesignScheduleDate = tbcsact.StaticDesignScheduleDate,
                                                                        StaticDesignScheduleTime = tbcsact.StaticDesignScheduleTime,
                                                                        StaticDesignScheduleEndTime = tbcsact.StaticDesignScheduleEndTime
                                                                    }).ToList();
                return ListData;
            }
        }

        #endregion

        #region :: Complaint Activity ::

        //Activity Complaint Logged stage 1 : save data 
        public int SaveCSActivityComplaintLoggedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem5 != -1)
            {
                #region -: Item5 :-
                var Item5 = ItmDataModel.Where(g => g.StaticDesignItemNo == 5);
                if (Item5 != null && Item5.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item5.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 5;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }

            */

            #endregion

            int Id = 0;
            Id = SaveCSActivityComplaintData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity Actions Follow-up stage 2 : save data 
        public int SaveCSActivityActionsFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*             
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3Date != "")
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }

            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityComplaintData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity Follow-Up stage 3 : save data 
        public int SaveCSActivityFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != "")
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-

                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityComplaintData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity stage 4 : save data 
        public int SaveCSActivityCompletionClosedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            //var serviceCallLog = Uow.ServiceCallLog.GetAll().Where(x =>x.ClassId == CustomerId)
            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityComplaintData(ServiceCalllogId, CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        public int SaveCSActivityComplaintData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, List<ValidationItemDataModel> ItemList)
        {
            #region :: CSActivity Complaint update :: 
            if (ItemList != null && ItemList.Count() > 0)
            {
                foreach (var item in ItemList)
                {
                    if (item.ValidationId > 0)
                    {
                        var DataModel = GetCSActivityList().Where(w => w.ClassId == CustomerId && w.StatusListId == StagId && w.ValidationItemId == item.ValidationId);
                        if (DataModel != null && DataModel.Count() > 0)
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel = DataModel.FirstOrDefault();
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StaticDesignNote = item.ValidationNote;
                            if (item.StartDatetime != null && item.StartDatetime != "")
                            {
                                CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(item.StartDatetime);
                            }
                            if (item.EndDatetime != null && item.EndDatetime != "")
                            {
                                CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(item.EndDatetime);
                            }
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.ModifiedDate = DateTime.Now;
                            CSActivitymodel.ModifiedBy = LoginUserId;
                            Uow.CSActivity.Update(CSActivitymodel);
                            Uow.Commit();
                        }
                        else
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel.ClassId = CustomerId;
                            CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                            CSActivitymodel.ValidationItemId = item.ValidationId;
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StaticDesignNote = item.ValidationNote;                            
                            if (item.StartDatetime != null && item.StartDatetime != "")
                            {
                                CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(item.StartDatetime);
                            }
                            if (item.EndDatetime  != null && item.EndDatetime != "")
                            {
                                CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(item.EndDatetime);
                            }
                            CSActivitymodel.StatusListId = StagId;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.CreatedBy = LoginUserId;
                            CSActivitymodel.CreatedDate = DateTime.Now;
                            Uow.CSActivity.Add(CSActivitymodel);
                            Uow.Commit();
                        }
                    }
                }
            }

            /*
             
            if (valIds != "")
            {
                string[] arrId = valIds.Split(',');
                foreach (var item in arrId)
                {
                    if (item != "")
                    {
                        int vlId = Convert.ToInt32(item);
                        var DataModel = GetCSActivityList().Where(w => w.ClassId == CustomerId && w.StatusListId == StagId && w.ValidationItemId == vlId);
                        if (DataModel != null && DataModel.Count() > 0)
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel = DataModel.FirstOrDefault();
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.ModifiedDate = DateTime.Now;
                            CSActivitymodel.ModifiedBy = LoginUserId;
                            Uow.CSActivity.Update(CSActivitymodel);
                            Uow.Commit();
                        }
                        else
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel.ClassId = CustomerId;
                            CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                            CSActivitymodel.ValidationItemId = Convert.ToInt32(item);
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StatusListId = StagId;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.CreatedBy = LoginUserId;
                            CSActivitymodel.CreatedDate = DateTime.Now;
                            Uow.CSActivity.Add(CSActivitymodel);
                            Uow.Commit();
                        }
                    }
                }
            }
            */
            #endregion

            #region :: CSstage :: 

            int Id = 0;
            var CSstageData = GetCSstageListById(CSstageId);
            if (CSstageData != null)
            {
                Id = CSstageData.CSstageId;
                CSstageData.Note = Note;
                CSstageData.IsActive = true;
                CSstageData.ModifiedDate = DateTime.Now;
                CSstageData.ModifiedBy = LoginUserId;
                Uow.CSstage.Update(CSstageData);
                Uow.Commit();
            }
            else
            {

                CSstage CSstagemodal = new CSstage();
                CSstagemodal.ClassId = CustomerId;
                CSstagemodal.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                CSstagemodal.StageNo = StagId;
                CSstagemodal.StatusListId = StagId;
                CSstagemodal.Note = Note;
                CSstagemodal.IsActive = true;
                CSstagemodal.CreatedBy = LoginUserId;
                CSstagemodal.CreatedDate = DateTime.Now;
                Uow.CSstage.Add(CSstagemodal);
                Uow.Commit();
                Id = CSstagemodal.CSstageId;
            }
            /*Update Service CallLog Status*/
            var servicecallog = Uow.ServiceCallLog.GetAll().SingleOrDefault(x => x.ServiceCallLogId == ServiceCalllogId);
            if (servicecallog != null)
            {
                servicecallog.StageStatusId = StagId;
                if (StagId == (int)Business.Enumeration.CustomerStatusList.ComplaintLogged)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess;
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.ActionsFollowUp)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess; 
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.FollowUp)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess; 
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.CompletionClosed)
                {
                    //servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.Closed;
                }                 
                Uow.ServiceCallLog.Update(servicecallog);
                Uow.Commit();
            }

            #endregion

            #region :: update Customer Status :: 

            jkDatabaseEntities context = new jkDatabaseEntities();

            //var StatusDataa = context.Status.Where(w => w.StatusListId == StagId && w.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage);
            //if (StatusDataa == null || StatusDataa.Count() == 0)
            //{
            //    Status oStatus = new Status();
            //    oStatus.ClassId = CustomerId;
            //    oStatus.CreatedBy = LoginUserId.ToString();
            //    oStatus.CreatedDate = DateTime.Now;
            //    oStatus.IsActive = true;
            //    oStatus.StatusDate = DateTime.Now;
            //    oStatus.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
            //    oStatus.StatusListId = StagId;
            //    Uow.Status.Add(oStatus);
            //    Uow.Commit();

            //    var CustModel = GetCustomerById(CustomerId);
            //    if (CustModel != null)
            //    {
            //        CustModel.StatusId = oStatus.StatusId;
            //        CustModel.StatusListId = oStatus.StatusListId;
            //        Uow.Customer.Update(CustModel);
            //        Uow.Commit();
            //    }
            //}
            #endregion

            return Id;
        }

        #endregion

        #region :: Failed Inspection Activity ::

        //Activity stage 1 : save data 
        public int SaveCSActivityFIComplaintLoggedData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.FailedInspection && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem5 != -1)
            {
                #region -: Item5 :-
                var Item5 = ItmDataModel.Where(g => g.StaticDesignItemNo == 5);
                if (Item5 != null && Item5.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item5.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 5;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem5 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }

            */

            #endregion

            int Id = 0;
            Id = SaveCSActivityFailedInspectionData(CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity stage 2 : save data 
        public int SaveCSActivityFIActionsFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.FailedInspection && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*             
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3Date != "")
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignScheduleDate = Convert.ToDateTime(optitem3Date);
                    if (optitem3Time != "")
                    {
                        CSActivitymodel.StaticDesignScheduleTime = Convert.ToDateTime(optitem3Time);
                    }
                    if (optitem3EndTime != "")
                    {
                        CSActivitymodel.StaticDesignScheduleEndTime = Convert.ToDateTime(optitem3EndTime);
                    }
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }

            if (optitem4 != -1)
            {
                #region -: Item4 :-
                var Item4 = ItmDataModel.Where(g => g.StaticDesignItemNo == 4);
                if (Item4 != null && Item4.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item4.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 4;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem4 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityFailedInspectionData(CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity stage 3 : save data 
        public int SaveCSActivityFIFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.FailedInspection && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != "")
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignNote = optitem2;
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-

                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityFailedInspectionData(CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        //Activity stage 4 : save data 
        public int SaveCSActivityFICompletionClosedData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3, List<ValidationItemDataModel> ItemList)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var ItmDataModel = context.CSActivities.Where(f => f.ClassId == CustomerId && f.TypeListId == (int)Business.Enumeration.TypeList.FailedInspection && f.StatusListId == StagId && f.IsActive == true && f.IsStaticDesign == true).ToList();

            #region -: Static Item Update :-

            /*
            if (optitem1 != -1)
            {
                #region -: Item1 :-
                var Item1 = ItmDataModel.Where(g => g.StaticDesignItemNo == 1);
                if (Item1 != null && Item1.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item1.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 1;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem1 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem2 != -1)
            {
                #region -: Item2 :-
                var Item2 = ItmDataModel.Where(g => g.StaticDesignItemNo == 2);
                if (Item2 != null && Item2.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item2.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 2;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem2 == 1 ? true : false);
                    CSActivitymodel.StaticDesignNote = (optitem2 == 0 ? optitem2Note : string.Empty);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion 
            }
            if (optitem3 != -1)
            {
                #region -: Item3 :-
                var Item3 = ItmDataModel.Where(g => g.StaticDesignItemNo == 3);
                if (Item3 != null && Item3.Count() > 0)
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel = Item3.FirstOrDefault();
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.ModifiedDate = DateTime.Now;
                    CSActivitymodel.ModifiedBy = LoginUserId;
                    Uow.CSActivity.Update(CSActivitymodel);
                    Uow.Commit();
                }
                else
                {
                    CSActivity CSActivitymodel = new CSActivity();
                    CSActivitymodel.ClassId = CustomerId;
                    CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                    CSActivitymodel.StatusListId = StagId;
                    CSActivitymodel.IsStaticDesign = true;
                    CSActivitymodel.StaticDesignItemNo = 3;
                    CSActivitymodel.StaticDesignIsItemChecked = (optitem3 == 1 ? true : false);
                    CSActivitymodel.IsActive = true;
                    CSActivitymodel.CreatedBy = LoginUserId;
                    CSActivitymodel.CreatedDate = DateTime.Now;
                    Uow.CSActivity.Add(CSActivitymodel);
                    Uow.Commit();
                }
                #endregion
            }
            */
            #endregion

            int Id = 0;
            Id = SaveCSActivityFailedInspectionData(CustomerId, StagId, CSstageId, Note, valIds, ItemList);
            return Id;
        }

        public int SaveCSActivityFailedInspectionData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, List<ValidationItemDataModel> ItemList)
        {
            #region :: CSActivity Failed Inspection update :: 
            if (ItemList != null && ItemList.Count() > 0)
            {
                foreach (var item in ItemList)
                {
                    if (item.ValidationId > 0)
                    {
                        var DataModel = GetCSActivityList().Where(w => w.ClassId == CustomerId && w.StatusListId == StagId && w.ValidationItemId == item.ValidationId);
                        if (DataModel != null && DataModel.Count() > 0)
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel = DataModel.FirstOrDefault();
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StaticDesignNote = item.ValidationNote;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.ModifiedDate = DateTime.Now;
                            CSActivitymodel.ModifiedBy = LoginUserId;
                            Uow.CSActivity.Update(CSActivitymodel);
                            Uow.Commit();
                        }
                        else
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel.ClassId = CustomerId;
                            CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.FailedInspection;
                            CSActivitymodel.ValidationItemId = item.ValidationId;
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StaticDesignNote = item.ValidationNote;
                            CSActivitymodel.StatusListId = StagId;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.CreatedBy = LoginUserId;
                            CSActivitymodel.CreatedDate = DateTime.Now;
                            Uow.CSActivity.Add(CSActivitymodel);
                            Uow.Commit();
                        }
                    }
                }
            }

            /*
             
            if (valIds != "")
            {
                string[] arrId = valIds.Split(',');
                foreach (var item in arrId)
                {
                    if (item != "")
                    {
                        int vlId = Convert.ToInt32(item);
                        var DataModel = GetCSActivityList().Where(w => w.ClassId == CustomerId && w.StatusListId == StagId && w.ValidationItemId == vlId);
                        if (DataModel != null && DataModel.Count() > 0)
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel = DataModel.FirstOrDefault();
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.ModifiedDate = DateTime.Now;
                            CSActivitymodel.ModifiedBy = LoginUserId;
                            Uow.CSActivity.Update(CSActivitymodel);
                            Uow.Commit();
                        }
                        else
                        {
                            CSActivity CSActivitymodel = new CSActivity();
                            CSActivitymodel.ClassId = CustomerId;
                            CSActivitymodel.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
                            CSActivitymodel.ValidationItemId = Convert.ToInt32(item);
                            CSActivitymodel.IsItemChecked = true;
                            CSActivitymodel.StatusListId = StagId;
                            CSActivitymodel.IsActive = true;
                            CSActivitymodel.CreatedBy = LoginUserId;
                            CSActivitymodel.CreatedDate = DateTime.Now;
                            Uow.CSActivity.Add(CSActivitymodel);
                            Uow.Commit();
                        }
                    }
                }
            }
            */
            #endregion

            #region :: CSstage :: 

            int Id = 0;
            var CSstageData = GetCSstageListById(CSstageId);
            if (CSstageData != null)
            {
                Id = CSstageData.CSstageId;
                CSstageData.Note = Note;
                CSstageData.IsActive = true;
                CSstageData.ModifiedDate = DateTime.Now;
                CSstageData.ModifiedBy = LoginUserId;
                Uow.CSstage.Update(CSstageData);
                Uow.Commit();
            }
            else
            {

                CSstage CSstagemodal = new CSstage();
                CSstagemodal.ClassId = CustomerId;
                CSstagemodal.TypeListId = (int)Business.Enumeration.TypeList.FailedInspection;
                CSstagemodal.StageNo = StagId;
                CSstagemodal.StatusListId = StagId;
                CSstagemodal.Note = Note;
                CSstagemodal.IsActive = true;
                CSstagemodal.CreatedBy = LoginUserId;
                CSstagemodal.CreatedDate = DateTime.Now;
                Uow.CSstage.Add(CSstagemodal);
                Uow.Commit();
                Id = CSstagemodal.CSstageId;
            }

            #endregion

            #region :: update Customer Status :: 

            /*Update Service CallLog Status*/
            var servicecallog = Uow.ServiceCallLog.GetAll().SingleOrDefault(x => x.ServiceCallLogId == CustomerId);
            if (servicecallog != null)
            {
                servicecallog.StageStatusId = StagId;
                if (StagId == (int)Business.Enumeration.CustomerStatusList.ComplaintLogged)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess;
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.ActionsFollowUp)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess;
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.FollowUp)
                {
                    servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.InProcess;
                }
                else if (StagId == (int)Business.Enumeration.CustomerStatusList.CompletionClosed)
                {
                    //servicecallog.StatusListId = (int)Business.Enumeration.CustomerStatusList.Closed;
                }
                Uow.ServiceCallLog.Update(servicecallog);
                Uow.Commit();
            }

            jkDatabaseEntities context = new jkDatabaseEntities();

            //var StatusDataa = context.Status.Where(w => w.StatusListId == StagId && w.TypeListId == (int)Business.Enumeration.TypeList.ComplaintStage);
            //if (StatusDataa == null || StatusDataa.Count() == 0)
            //{
            //    Status oStatus = new Status();
            //    oStatus.ClassId = CustomerId;
            //    oStatus.CreatedBy = LoginUserId.ToString();
            //    oStatus.CreatedDate = DateTime.Now;
            //    oStatus.IsActive = true;
            //    oStatus.StatusDate = DateTime.Now;
            //    oStatus.TypeListId = (int)Business.Enumeration.TypeList.ComplaintStage;
            //    oStatus.StatusListId = StagId;
            //    Uow.Status.Add(oStatus);
            //    Uow.Commit();

            //    var CustModel = GetCustomerById(CustomerId);
            //    if (CustModel != null)
            //    {
            //        CustModel.StatusId = oStatus.StatusId;
            //        CustModel.StatusListId = oStatus.StatusListId;
            //        Uow.Customer.Update(CustModel);
            //        Uow.Commit();
            //    }
            //}
            #endregion

            return Id;
        }

        #endregion

        public List<CustomerFranchiseeDistribution> GetCustomerFranchiseeForNewAccountForm(int CustomerId)
        {
            List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                using (var multipleresult = conn.QueryMultiple("dbo.GetCustomerFranchiseeForNewAccountForm", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerFranchiseeDistribution = multipleresult.Read<CustomerFranchiseeDistribution>().ToList();
                    }
                }
            }
            return lstCustomerFranchiseeDistribution;
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var query = @"EXEC GetCustomerFranchiseeForNewAccountForm @CustomerId";

            //    return conn.Query<CustomerFranchiseeDistribution>(query, new
            //    {
            //        CustomerId = CustomerId,                     
            //    });
            //}
        }

        public AccountAcceptanceInfoViewModel GetAccountAcceptanceInfoOffer(int CustomerId)
        {
            AccountAcceptanceInfoViewModel retVal = null;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_GetAccountAcceptanceInfoForOfferingByCustomerId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        retVal = multipleresult.Read<AccountAcceptanceInfoViewModel>().ToList().FirstOrDefault();
                        if (retVal == null)
                            retVal = new AccountAcceptanceInfoViewModel();
                    }
                }
            }

            return retVal;
        }

        public CustomerForSettingViewModel GetAllCustomerForSetting(bool IsThirdParty, int regionId = 0, int isAll = 0)
        {
            CustomerForSettingViewModel oCustomerForSetting = new CustomerForSettingViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", (regionId == 0 ? SelectedRegionId : regionId));
                parmas.Add("@IsAll", isAll);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerAllForSetting", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        if (isAll == 1)
                        {
                            oCustomerForSetting.ChildCustomer = multipleresult.Read<CustomerSearchModel>().ToList();
                        }
                        else
                        {
                            oCustomerForSetting.ParentCustomer = multipleresult.Read<CustomerSearchModel>().ToList();
                            oCustomerForSetting.ChildCustomer = multipleresult.Read<CustomerSearchModel>().ToList();
                            if (IsThirdParty)
                                oCustomerForSetting.ParentCustomer = multipleresult.Read<CustomerSearchModel>().ToList();
                        }

                    }
                }
            }
            return oCustomerForSetting;
        }
        public List<CustomerSearchModel> GetCustomerSettingAllPerentChildList(int type, int regionId = 0, int ParentId = 0, string StatusListIds = "1", int IsAll = 0)
        {
            List<CustomerSearchModel> lstCustomerSearchModel = new List<CustomerSearchModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();

                parmas.Add("@RegionId", (regionId == 0 ? SelectedRegionId : regionId));
                parmas.Add("@Type", type);
                parmas.Add("@ParentId", ParentId > 0 ? ParentId : 0);
                parmas.Add("@StatusListIds", StatusListIds != "" ? StatusListIds : "1");
                parmas.Add("@IsAll", IsAll);
                using (var multipleresult = conn.QueryMultiple("dbo.[portal_spGet_C_CustomerSettingAllPerentChildList]", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomerSearchModel = multipleresult.Read<CustomerSearchModel>().ToList();

                    }
                }
            }
            return lstCustomerSearchModel;
        }

        public List<CustomerSearchModel> GetAllCustomerChild(int custId, int regionId = 0)
        {
            List<CustomerSearchModel> oCustomerForSetting = new List<CustomerSearchModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", (regionId == 0 ? SelectedRegionId : regionId));
                parmas.Add("@CustomerId", custId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerAllChildForSetting", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {

                        oCustomerForSetting = multipleresult.Read<CustomerSearchModel>().ToList();
                    }
                }
            }
            return oCustomerForSetting;
        }

        #region :: Search Customer ::

        public List<CustomerSearchResultModel> GetCustomerSearchResultList(int RegionId, string Search, decimal AmountTo, decimal AmountFrom, decimal SqrFtTo, decimal SqrFtFrom, string Orderby, int Status, string regionIds = "", int ctypeId = 0)
        {
            List<CustomerSearchResultModel> lstCustomer = new List<CustomerSearchResultModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", RegionId);
                parmas.Add("@Search", Search);
                parmas.Add("@AmountTo", AmountTo);
                parmas.Add("@AmountFrom", AmountFrom);
                parmas.Add("@SqrFtTo", SqrFtTo);
                parmas.Add("@SqrFtFrom", SqrFtFrom);
                parmas.Add("@Orderby", Orderby);
                parmas.Add("@Status", Status);
                parmas.Add("@regionIds", regionIds);
                parmas.Add("@contractTypeId", ctypeId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerSearchResult", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomer = multipleresult.Read<CustomerSearchResultModel>().ToList();
                    }
                }
            }
            return lstCustomer;
        }

        public List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomerSearchResultDetails(int CustID, string startDate, string endDate)
        {
            DateTime startDT = Convert.ToDateTime(startDate);
            DateTime endDT = Convert.ToDateTime(endDate);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<JKViewModels.Customer.ServiceCallLogModel> ServiceCallLog = (from tbcust in context.Customers
                                                                                  join
                                                tbservicecalllog in context.ServiceCallLogs on tbcust.CustomerId equals tbservicecalllog.ClassId
                                                                                  join tbStatus in context.StatusResultLists on tbservicecalllog.StatusResultListId equals tbStatus.StatusResultListId
                                                                                  join tbScalllog in context.ServiceCallLogTypeLists on tbservicecalllog.ServiceLogTypeListId equals tbScalllog.ServiceCallLogTypeListId
                                                                                  select new ServiceCallLogModel
                                                                                  {
                                                                                      CustomerNo = tbcust.CustomerNo,
                                                                                      CallDate = tbservicecalllog.CallDate,
                                                                                      CallTime = tbservicecalllog.CallTime.ToString(),
                                                                                      Status = tbStatus.Name,
                                                                                      SpokeWith = tbservicecalllog.SpokeWith,
                                                                                      Action = tbservicecalllog.Action,
                                                                                      CallBack = tbservicecalllog.CallBack,
                                                                                      Comments = tbservicecalllog.Comments,
                                                                                      ClassId = tbservicecalllog.ClassId,
                                                                                      ServiceLogTypeListName = tbScalllog.Name,
                                                                                      InitiatedBy = tbservicecalllog.InitiatedById
                                                                                  }).Where(x => x.ClassId == CustID && (x.CallDate >= startDT && x.CallDate <= endDT)).OrderByDescending(x => x.CallDate).ToList();
                return ServiceCallLog;
            }
        }

        public List<CustomerSearchCancallationPendingResultViewModel> GetCustomerSearchCancallationPendingList(string statusListIds = "0", string regionId = "")
        {
            List<CustomerSearchCancallationPendingResultViewModel> lstCustomer = new List<CustomerSearchCancallationPendingResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StatusList", statusListIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerSearchCancallationPendingList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomer = multipleresult.Read<CustomerSearchCancallationPendingResultViewModel>().ToList();
                    }
                }
            }
            return lstCustomer;
        }

        public List<CustomerSearchCancallationPendingResultViewModel> GetCustomerSearchCancallationPendingListNew(string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0)
        {
            List<CustomerSearchCancallationPendingResultViewModel> lstCustomer = new List<CustomerSearchCancallationPendingResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StatusList", statusListIds);
                parmas.Add("@ServiceLogTypeListId", ServiceLogTypeListId);
                using (var multipleresult = conn.QueryMultiple("portal_spGet_C_CustomerSearchCancallationPendingList_New", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomer = multipleresult.Read<CustomerSearchCancallationPendingResultViewModel>().ToList();
                    }
                }
            }
            return lstCustomer;
        }
        public List<CustomerServiceWednesdayReportResultViewModel> GetCustomerServiceWednesdayReport(int Days = 0, string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0, string GroupByValue = "", string AtLeast = "", string LessThan = "", string SearchAmount = "", string AtRisk = "")
        {
            List<CustomerServiceWednesdayReportResultViewModel> lstResult = new List<CustomerServiceWednesdayReportResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StatusList", statusListIds);
                parmas.Add("@ServiceLogTypeListId", ServiceLogTypeListId);
                parmas.Add("@NoOfDays", Days);
                parmas.Add("@GroupByValue", GroupByValue);
                parmas.Add("@AtLeast", AtLeast);
                parmas.Add("@LessThan", LessThan);
                parmas.Add("@SearchAmount", SearchAmount);
                parmas.Add("@AtRisk", AtRisk);
                using (var multipleresult = conn.QueryMultiple("portal_spGet_C_CustomerServiceWednesdayReport", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstResult = multipleresult.Read<CustomerServiceWednesdayReportResultViewModel>().ToList();
                    }
                }
            }
            return lstResult;
        }
        public List<CustomerFranchiseeDistributionModel> GetFranchiseeDistributionWithCustomer(int CustomerId)
        {
            List<CustomerFranchiseeDistributionModel> lstCustomer = new List<CustomerFranchiseeDistributionModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_GetFranchiseeListByCustomerId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomer = multipleresult.Read<CustomerFranchiseeDistributionModel>().ToList();
                    }
                }
            }
            return lstCustomer;
        }
        public List<CustomerSearchCancallationPendingResultViewModel> GetCustomerServiceComplainListResult(string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0,string StageStatusIds = "")
        {
            List<CustomerSearchCancallationPendingResultViewModel> lstCustomer = new List<CustomerSearchCancallationPendingResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StatusList", statusListIds);
                parmas.Add("@ServiceLogTypeListId", ServiceLogTypeListId);
                parmas.Add("@StageStatusIds", StageStatusIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_C_CustomerServiceComplainListResult", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCustomer = multipleresult.Read<CustomerSearchCancallationPendingResultViewModel>().ToList();
                    }
                }
            }
            return lstCustomer;
        }

        public List<DistributionFeesDetailModel> GetDistributionFeesDetail(int DistributionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<DistributionFeesDetailModel> list = (from tbdisf in context.DistributionFees
                                                          join tblfrt in context.FeeRateTypeLists on tbdisf.FeeRateTypeListId equals tblfrt.FeeRateTypeListId
                                                          join tblff in context.Fees on tbdisf.FeeId equals tblff.FeeId.ToString()
                                                          select new DistributionFeesDetailModel
                                                          {
                                                              DistributionFeesId = tbdisf.DistributionFeesId,
                                                              DistributionId = tbdisf.DistributionId,
                                                              Amount = tbdisf.Amount,
                                                              Name = tblff.Name,
                                                              Rate = tblfrt.Rate,

                                                          }).Where(w => w.DistributionId == DistributionId).ToList();
                return list;
            }
        }

        public Dictionary<string, string> GetFrancisesBySearchCustomerId(int custId)
        {

            Dictionary<string, string> lstFrancises = new Dictionary<string, string>();
            try
            {
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    var lst = (from f in context.Franchisees
                               join d in context.Distributions
                               on f.FranchiseeId equals d.FranchiseeId
                               where d.CustomerId == custId && d.isActive == true
                               select new { f.FranchiseeId, f.FranchiseeNo, f.Name }).FirstOrDefault();
                    if (lst != null)
                    {
                        lstFrancises.Add(lst.FranchiseeId + "-" + lst.FranchiseeNo, lst.Name);
                    }
                }
            }
            catch { }

            return lstFrancises;
        }
        #endregion

        public bool CheckCustomerOfferingExits(int CustomerId, int FranchiseeId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var dataModel = context.Offerings.Where(w => w.CustomerId == CustomerId && w.FranchiseeId == FranchiseeId);
                if (dataModel != null && dataModel.Count() != 0)
                {
                    return false;
                }
                return true;
            }
        }
        public bool CustomerOfferAcceptedStatusSave(int OfferingId, DateTime ResponseDateTime, string ResponseName, string ReasonNote)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var dataModel = context.Offerings.Where(w => w.OfferingId == OfferingId);
                if (dataModel != null && dataModel.Count() != 0)
                {
                    Offering objOffering = new Offering();
                    objOffering = dataModel.FirstOrDefault();
                    objOffering.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Accepted;
                    objOffering.AcceptedDate = ResponseDateTime;
                    objOffering.AcceptedNote = ReasonNote;
                    objOffering.ModifiedDate = System.DateTime.Now;
                    objOffering.ModifiedBy = LoginUserId;
                    context.SaveChanges();
                }
                return true;
            }


        }
        public bool CustomerOfferDeclineStatusSave(int OfferingId, int DeclineReasonListId, string DeclineReasonNote)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var dataModel = context.Offerings.Where(w => w.OfferingId == OfferingId);
                if (dataModel != null && dataModel.Count() != 0)
                {
                    Offering objOffering = new Offering();
                    objOffering = dataModel.FirstOrDefault();
                    objOffering.StatusListId = (int)JKApi.Business.Enumeration.CustomerStatusList.Declined;
                    objOffering.DeclinedDate = DateTime.Now;
                    objOffering.DeclineReasonListId = DeclineReasonListId;
                    objOffering.DeclineReasonNote = DeclineReasonNote;
                    objOffering.ModifiedDate = System.DateTime.Now;
                    objOffering.ModifiedBy = LoginUserId;
                    context.SaveChanges();
                }
                return true;
            }
        }
        public Offering GetOfferingsById(int OfferingId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var dataModel = context.Offerings.Where(w => w.OfferingId == OfferingId);
                if (dataModel != null && dataModel.Count() > 0)
                {
                    return dataModel.FirstOrDefault();
                }
                return new Offering();
            }

        }
        public List<AuthUserLogin> GetAllLoginUserList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var dataModel = context.AuthUserLogins.Where(w => w.IsDelete == false && w.IsEnable == true).ToList();
                if (dataModel != null && dataModel.Count() > 0)
                {
                    return dataModel;
                }
                return new List<AuthUserLogin>();
            }
        }

        public List<IncreaseDecreaseHistoryModel> GetIncreaseDecreaseHistoryList(string regionIds = "", DateTime? from = null, DateTime? to = null, int month = 0, int year = 0, int customerId = 0)
        {
            List<IncreaseDecreaseHistoryModel> lstIncreaseDecreaseHistory = new List<IncreaseDecreaseHistoryModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", regionIds);
                parmas.Add("@FromDate", from);
                parmas.Add("@ToDate", to);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                parmas.Add("@CustomerId", customerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spIncreaseDecreaseHistoryList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstIncreaseDecreaseHistory = multipleresult.Read<IncreaseDecreaseHistoryModel>().ToList();
                    }
                }
            }
            return lstIncreaseDecreaseHistory;
        }

        public void UpdateTaxRateAddress(int OldAddId, int AddressId, int CustomerId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var Modeldata = context.TaxRates.Where(w => w.classId == CustomerId && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Customer && (OldAddId == 0 || w.AddressId == OldAddId));
            if (Modeldata != null && Modeldata.Count() > 0)
            {
                TaxRate model = new TaxRate();
                model = Modeldata.FirstOrDefault();
                model.AddressId = AddressId;
                context.SaveChanges();
            }
        }
        public BasicInfoCustomerModel GetBasicInfoCustomer(int Id)
        {
            BasicInfoCustomerModel model = new BasicInfoCustomerModel();

            int CustomerTypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
            int CustomerMainContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            using (var context = new jkDatabaseEntities())
            {
                var result = (from c in context.Customers
                              where c.CustomerId == Id
                              join mc in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == 6 && one.ClassId == Id && one.IsActive == true) on c.CustomerId equals mc.ClassId
                              into temp0
                              from t0 in temp0.DefaultIfEmpty()
                              join mp in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactTypeListId && one.ClassId == Id && one.IsActive == true) on c.CustomerId equals mp.ClassId
                              into temp1
                              from t1 in temp1.DefaultIfEmpty()
                              where c.RegionId == SelectedRegionId || SelectedRegionId == 0
                              select new BasicInfoCustomerModel
                              {
                                  CustomerId = c.CustomerId,
                                  CustomerName = c.Name,
                                  CustomerNo = c.CustomerNo,
                                  Contact = t0.Name,
                                  PhoneNo = t1.Phone1
                              });
                if (result != null && result.Count() > 0)
                {
                    model = result.FirstOrDefault();
                }
            }
            return model;
        }
        public Dictionary<int, string> GetCustomerNotes(int CustomerId, int SelectedRegionId, int TypeListId=1)
        {
            Dictionary<int, string> lstFrancises = new Dictionary<int, string>();
            using (var context = new jkDatabaseEntities())
            {
                //int CustomerTypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                var result = (from c in context.Customers
                              where c.CustomerId == CustomerId
                              join nt in context.Notes on c.CustomerId equals nt.ClassId
                              where nt.ClassId == CustomerId && nt.RegionId == SelectedRegionId && nt.TypeListId == TypeListId
                              join ntd in context.NotesDetails on nt.NotesId equals ntd.NotesId
                              where ntd.IsActive == true
                              select new { ntd.NotesDetailId, ntd.Notes });

                if (result != null && result.Count() > 0)
                {
                    lstFrancises.Add(result.FirstOrDefault().NotesDetailId, result.FirstOrDefault().Notes.ToString());
                }
            }
            return lstFrancises;
        }
        public int SaveNotesDetail(int Id, int ClassId, string Notes, int _type)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            if (Id > 0)
            {
                var dataModel = context.NotesDetails.Where(w => w.NotesDetailId == Id);
                if (dataModel != null && dataModel.Count() > 0)
                {
                    NotesDetail model = new NotesDetail();
                    model = dataModel.FirstOrDefault();
                    model.Notes = Notes;
                    model.ModifiedBy = LoginUserId;
                    model.ModifiedDate = DateTime.Now;
                    context.SaveChanges();
                }
            }
            else
            {
                Note _Note = new Note();
                _Note.ClassId = ClassId;
                _Note.TypeListId = (_type == 2 ? 2: 1);
                _Note.RegionId = SelectedRegionId;
                _Note.CreatedBy = LoginUserId;
                _Note.CreatedDate = DateTime.Now;
                context.Notes.Add(_Note);
                context.SaveChanges();

                NotesDetail _NotesDetail = new NotesDetail();
                _NotesDetail.NotesId = _Note.NotesId;
                _NotesDetail.Notes = Notes;
                _NotesDetail.IsActive = true;
                _NotesDetail.RegionId = SelectedRegionId;
                _NotesDetail.CreatedBy = LoginUserId;
                _NotesDetail.CreatedDate = DateTime.Now;
                context.NotesDetails.Add(_NotesDetail);
                context.SaveChanges();
                Id = _NotesDetail.NotesDetailId;
            }
            return Id;
        }

        public IEnumerable<CustomerServiceScheduleDataModel> GetCustomerServiceScheduleData(int? customerId, string regionId, int? dayToAdd, int? userId, DateTime? startDate, DateTime? endDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                try
                {
                    if (string.IsNullOrEmpty(regionId) || regionId == "null")
                    {
                        regionId = "0";
                    }

                    var query = @"EXEC CRM_Get_ScheduleReport 1,@ClassId,@StartDate,@EndDate,@DayToAdd,@UserId,@RegionId,null";
                    return conn.Query<CustomerServiceScheduleDataModel>(query, new
                    {
                        ClassId = customerId,
                        RegionId = regionId,
                        StartDate = startDate,
                        EndDate = endDate,
                        DayToAdd = dayToAdd,
                        UserId = userId,

                    });

                }
                catch
                {
                    return null;
                }

            }
        }

        public IEnumerable<CustomerServiceScheduleDataModel> GetCRMScheduleData(int customerId, DateTime dateTime, int userId, string regionId, DateTime endDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                try
                {
                    if (string.IsNullOrEmpty(regionId) || regionId == "null")
                    {
                        regionId = "0";
                    }

                    var query = @"EXEC CRM_Get_ScheduleReport 2,@ClassId,@StartDate,@EndDate,@DayToAdd,@UserId,@RegionId,null";
                    return conn.Query<CustomerServiceScheduleDataModel>(query, new
                    {
                        ClassId = customerId,
                        RegionId = regionId,
                        StartDate = dateTime,
                        EndDate = endDate,
                        DayToAdd = 0,
                        UserId = userId,

                    });

                }
                catch
                {
                    return null;
                }

            }
        }

        public IEnumerable<CRM_ScheduleTypeModel> GetScheduleTyoeList()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                try
                {

                    var query = @"EXEC CRM_Get_ScheduleReport 3,null,null,null,null,null,null";
                    return conn.Query<CRM_ScheduleTypeModel>(query);

                }
                catch
                {
                    return null;
                }

            }
        }

        public void SaveCSAccountWalkThursFormFieldDetail(int customerId, int FranchiseeId, List<NewAccountFormFieldModel> List)
        {
            if (List.Count() > 0)
            {
                jkDatabaseEntities context = new jkDatabaseEntities();
                foreach (var item in List)
                {
                    var data = context.CSAccountWalkThursFormFieldDetails.Where(w => (FranchiseeId == 0 || w.FranchiseeId == FranchiseeId) && w.CustomerId == customerId && w.CSAccountWalkThursFormFieldId == item.Id && w.IsActive == true);
                    if (data != null && data.Count() > 0)
                    {
                        CSAccountWalkThursFormFieldDetail _model = new CSAccountWalkThursFormFieldDetail();
                        _model = data.FirstOrDefault();
                        _model.FieldValue = (item.FieldValue == 1 ? true : false);
                        _model.FieldText = item.FieldText;
                        _model.IsActive = true;
                        context.SaveChanges();
                    }
                    else
                    {
                        CSAccountWalkThursFormFieldDetail _model = new CSAccountWalkThursFormFieldDetail();
                        _model.CSAccountWalkThursFormFieldId = item.Id;
                        _model.CustomerId = customerId;
                        if (FranchiseeId != 0)
                        {
                            _model.FranchiseeId = FranchiseeId;
                        }
                        _model.FieldValue = (item.FieldValue == 1 ? true : false);
                        _model.FieldText = item.FieldText;
                        _model.CreatedBy = LoginUserId;
                        _model.IsActive = true;
                        _model.CreatedDate = DateTime.Now;
                        context.CSAccountWalkThursFormFieldDetails.Add(_model);
                        context.SaveChanges();
                    }

                }
            }
        }

        public List<CSAccountWalkThursFormFieldDetail> GetCSAccountWalkThursFormFieldDetailWithCustomer(int customerId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            List<CSAccountWalkThursFormFieldDetail> list = new List<CSAccountWalkThursFormFieldDetail>();
            list = context.CSAccountWalkThursFormFieldDetails.Where(w => w.CustomerId == customerId && w.IsActive == true).ToList();
            return list;
        }

    }

    public class CustomerForSettingViewModel
    {
        public List<CustomerSearchModel> ParentCustomer { get; set; }
        public List<CustomerSearchModel> ChildCustomer { get; set; }
    }

    public class CFranchiseeDistributionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public List<vw_ContractDetail> lstContractDetail { get; set; }
        public List<vw_CustomerDetailFranchiseeDistribution> lstDistribution { get; set; }
        public List<vw_ContractDetailDestribution> lstContractDetailDistribution { get; set; }
        public List<vw_Distribution> lstdistributionlist { get; set; }
    }



    public class CustomerFranchiseeDistributionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string PONumber { get; set; }
        public string AccountTypeName { get; set; }
        public string Term { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<vw_ContractDetail> lstContractDetail { get; set; }
        public List<vw_CustomerDetailFranchiseeDistribution> lstFranchiseeDistribution { get; set; }
        public List<portal_spGet_ContractDetailByCustomerID_Result> listContractDetail { get; set; }
        public List<portal_spGet_CustomerDetailFranchiseeDistribution_Result> listFranchiseeDistribution { get; set; }
    }

    public class InvoiceDetailViewModel
    {
        public vw_InvoiceDetailViewModel InvoiceDetail { get; set; }
        public List<vw_InvoiceContractDetailList> InvoiceDetailItems { get; set; }
        public JKApi.Data.DAL.Region InvoiceRegion { get; set; }
    }

    public class RevenueDistributionInvoiceDetailViewModel
    {
        public vw_InvoiceDetailViewModel InvoiceDetail { get; set; }
        public List<vw_InvoiceContractDetailList> InvoiceDetailItems { get; set; }
        public List<FranchiseeDistribution> FranchiseeDistributionItems { get; set; }
        public JKApi.Data.DAL.Region InvoiceRegion { get; set; }
    }

    public class FranchiseeDistribution
    {
        public int DistributionId { get; set; }
        public int ContractId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ContractDetailId { get; set; }
        public Nullable<int> DetailLineNumber { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<decimal> DistributionAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string FranchiseeNo { get; set; }
        public Nullable<decimal> Fee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class InvoiceRevenueDistributionDetailViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public int BillMonth { get; set; }
        public int IsEffectiveBillPeriod { get; set; }
        public int PartialDays { get; set; }
        public int IsPartial { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? EffectiveAMT { get; set; }
    }

}