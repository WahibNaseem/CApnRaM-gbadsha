using JKApi.Data;
using JKApi.Service.Helper.Extension;
using JKViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;
using JKViewModels.Franchise;
using System.Data.Entity.Core.EntityClient;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using JKViewModels.Franchisee;
using System.Globalization;
using System.Threading;
using System.Data.Entity.Core.Objects;
using JKApi.Service.Service;
using Dapper;

namespace JKApi.Service.Service
{
    public class FranchiseService : BaseService
    {
        public FranchiseService()
        {
        }

        public IEnumerable<FranchiseSearch> GetFranchiseSearchList(string _searchby, string _searchvalue, int _status)
        {


            string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

            List<FranchiseSearch> lstfranchsrch = new List<FranchiseSearch>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spsearch_F_Information", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@searchType", _searchby);
                cmd.Parameters.AddWithValue("@searchTerm", _searchvalue);
                cmd.Parameters.AddWithValue("@status", _status);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstfranchsrch.Add(new FranchiseSearch
                    {
                        Number = reader["number"].ToString(),
                        Name = reader["name"].ToString(),
                        Id = reader["id"].ToString(),
                        Address1 = reader["address1"].ToString(),
                        City = reader["city"].ToString(),
                        Statename = reader["statename"].ToString(),
                        Postalcode = reader["postalcode"].ToString(),
                        Phone = FormatPhone(reader["phone"].ToString()),
                        Distribution = reader["distribution"].ToString(),

                    });


                }
                reader.Close();


            }
            return lstfranchsrch;
        }
        //public ManageFranchiseViewModel GetFranchiseForManage(int _id)
        //{


        //    string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

        //    List<ManageFranchiseViewModel> lstfranchsrch = new List<ManageFranchiseViewModel>();
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("spget_F_Information", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@id", _id);

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            lstfranchsrch.Add(new ManageFranchiseViewModel
        //            {
        //                txtname = reader["name"].ToString()
        //                //id = reader["id"].ToString(),                        
        //                //regionId = reader["regionId"].ToString(),
        //                //number = reader["number"].ToString(),
        //                //signdate = reader["signdate"].ToString(),
        //                //term = reader["term"].ToString(),
        //                //expirationdate = reader["expirationdate"].ToString(),
        //                //status = reader["status"].ToString(),
        //                //chargeback = reader["chargeback"].ToString(),
        //                //chargebackbppadmin = reader["chargebackbppadmin"].ToString(),
        //                //generatereport = reader["generatereport"].ToString(),
        //                //plantype = reader["plantype"].ToString(),
        //                //planamount = reader["planamount"].ToString(),
        //                //ibamount = reader["ibamount"].ToString(),
        //                //equipmentfulfill = reader["equipmentfulfill"].ToString(),
        //                //supplyfulfill = reader["supplyfulfill"].ToString(),
        //                //trainingfulfill = reader["trainingfulfill"].ToString(),
        //                //legaloblstart = reader["legaloblstart"].ToString(),
        //                //legaloblfulfilled = reader["legaloblfulfilled"].ToString(),
        //                //createdby = reader["createdby"].ToString(),
        //                //createdate = reader["createdate"].ToString(),
        //                //modifiedby = reader["modifiedby"].ToString(),
        //                //modifieddate = reader["modifieddate"].ToString(),
        //                //incorporated = reader["incorporated"].ToString(),
        //                //print1099 = reader["print1099"].ToString(),
        //                //bbpadminfee = reader["bbpadminfee"].ToString(),
        //                //downpayment = reader["downpayment"].ToString(),
        //                //noofpayments = reader["noofpayments"].ToString(),
        //                //paymentamount = reader["paymentamount"].ToString(),
        //                //name = reader["name"].ToString(),
        //                //address1 = reader["address1"].ToString(),
        //                //address2 = reader["address2"].ToString(),
        //                //city = reader["city"].ToString(),
        //                //state = reader["state"].ToString(),
        //                //postalcode = reader["postalcode"].ToString(),
        //                //phone = reader["phone"].ToString(),
        //                //ext = reader["ext"].ToString(),
        //                //cell = reader["cell"].ToString(),
        //                //fax = reader["fax"].ToString(),
        //                //email = reader["email"].ToString(),
        //                //name1099 = reader["name1099"].ToString(),
        //                //taxAuthorityId = reader["taxAuthorityId"].ToString(),
        //                //ein = reader["ein"].ToString(),
        //                //payeesameas = reader["payeesameas"].ToString(),
        //                //statusdate = reader["statusdate"].ToString(),
        //                //statusnotes = reader["statusnotes"].ToString(),
        //                //resumedate = reader["resumedate"].ToString(),
        //                //statusreasonid = reader["statusreasonid"].ToString(),
        //                //finderfeetypeid = reader["finderfeetypeid"].ToString(),
        //                //legalobldue = reader["legalobldue"].ToString(),
        //                //planinterest = reader["planinterest"].ToString(),
        //                //businesslicense = reader["businesslicense"].ToString(),
        //                //inhouse = reader["inhouse"].ToString(),
        //                //accountrebate = reader["accountrebate"].ToString(),
        //                //payeename = reader["payeename"].ToString(),
        //                //payeeaddress = reader["payeeaddress"].ToString(),
        //                //payeeaddress2 = reader["payeeaddress2"].ToString(),
        //                //payeecity = reader["payeecity"].ToString(),
        //                //payeecounty = reader["payeecounty"].ToString(),
        //                //payeestate = reader["payeestate"].ToString(),
        //                //payeepostalcode = reader["payeepostalcode"].ToString()



        //            });


        //        }
        //        reader.Close();


        //    }
        //    return lstfranchsrch.FirstOrDefault();
        //}
        public AddFranchiseViewModel GetFranchiseForManage(int _id)
        {


            string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

            List<AddFranchiseViewModel> lstfranchsrch = new List<AddFranchiseViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spget_F_Information", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", _id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstfranchsrch.Add(new AddFranchiseViewModel
                    {
                        txtname = reader["name"].ToString()
                        //id = reader["id"].ToString(),                        
                        //regionId = reader["regionId"].ToString(),
                        //number = reader["number"].ToString(),
                        //signdate = reader["signdate"].ToString(),
                        //term = reader["term"].ToString(),
                        //expirationdate = reader["expirationdate"].ToString(),
                        //status = reader["status"].ToString(),
                        //chargeback = reader["chargeback"].ToString(),
                        //chargebackbppadmin = reader["chargebackbppadmin"].ToString(),
                        //generatereport = reader["generatereport"].ToString(),
                        //plantype = reader["plantype"].ToString(),
                        //planamount = reader["planamount"].ToString(),
                        //ibamount = reader["ibamount"].ToString(),
                        //equipmentfulfill = reader["equipmentfulfill"].ToString(),
                        //supplyfulfill = reader["supplyfulfill"].ToString(),
                        //trainingfulfill = reader["trainingfulfill"].ToString(),
                        //legaloblstart = reader["legaloblstart"].ToString(),
                        //legaloblfulfilled = reader["legaloblfulfilled"].ToString(),
                        //createdby = reader["createdby"].ToString(),
                        //createdate = reader["createdate"].ToString(),
                        //modifiedby = reader["modifiedby"].ToString(),
                        //modifieddate = reader["modifieddate"].ToString(),
                        //incorporated = reader["incorporated"].ToString(),
                        //print1099 = reader["print1099"].ToString(),
                        //bbpadminfee = reader["bbpadminfee"].ToString(),
                        //downpayment = reader["downpayment"].ToString(),
                        //noofpayments = reader["noofpayments"].ToString(),
                        //paymentamount = reader["paymentamount"].ToString(),
                        //name = reader["name"].ToString(),
                        //address1 = reader["address1"].ToString(),
                        //address2 = reader["address2"].ToString(),
                        //city = reader["city"].ToString(),
                        //state = reader["state"].ToString(),
                        //postalcode = reader["postalcode"].ToString(),
                        //phone = reader["phone"].ToString(),
                        //ext = reader["ext"].ToString(),
                        //cell = reader["cell"].ToString(),
                        //fax = reader["fax"].ToString(),
                        //email = reader["email"].ToString(),
                        //name1099 = reader["name1099"].ToString(),
                        //taxAuthorityId = reader["taxAuthorityId"].ToString(),
                        //ein = reader["ein"].ToString(),
                        //payeesameas = reader["payeesameas"].ToString(),
                        //statusdate = reader["statusdate"].ToString(),
                        //statusnotes = reader["statusnotes"].ToString(),
                        //resumedate = reader["resumedate"].ToString(),
                        //statusreasonid = reader["statusreasonid"].ToString(),
                        //finderfeetypeid = reader["finderfeetypeid"].ToString(),
                        //legalobldue = reader["legalobldue"].ToString(),
                        //planinterest = reader["planinterest"].ToString(),
                        //businesslicense = reader["businesslicense"].ToString(),
                        //inhouse = reader["inhouse"].ToString(),
                        //accountrebate = reader["accountrebate"].ToString(),
                        //payeename = reader["payeename"].ToString(),
                        //payeeaddress = reader["payeeaddress"].ToString(),
                        //payeeaddress2 = reader["payeeaddress2"].ToString(),
                        //payeecity = reader["payeecity"].ToString(),
                        //payeecounty = reader["payeecounty"].ToString(),
                        //payeestate = reader["payeestate"].ToString(),
                        //payeepostalcode = reader["payeepostalcode"].ToString()



                    });


                }
                reader.Close();


            }
            return lstfranchsrch.FirstOrDefault();
        }
        public IEnumerable<FranchiseListItem> GetTransactionTypeList(int? _franid)
        {


            string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

            List<FranchiseListItem> lstfranchsrch = new List<FranchiseListItem>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spGet_F_TrxType", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@franid", _franid);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstfranchsrch.Add(new FranchiseListItem
                    {

                        Name = reader["name"].ToString(),
                        Id = reader["id"].ToString(),


                    });


                }
                reader.Close();


            }
            return lstfranchsrch;
        }
        public IEnumerable<TransactionFranchiseSearch> GetTransactionSearchList(int? lstsearchby, string searchvalue, string transactionstartdate, string transactionenddate, string createstartdate, string createenddate, int? lstbillmonth, int? lstbillyear, int lstfilterby, int lsttransactiontype)
        {


            string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

            List<TransactionFranchiseSearch> lstTransactionFranchiseSearch = new List<TransactionFranchiseSearch>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spGet_F_TrxListClosed", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@searchby", lstsearchby);
                cmd.Parameters.AddWithValue("@searchvalue", searchvalue);
                cmd.Parameters.AddWithValue("@trxstartdate", transactionstartdate != "" ? transactionstartdate : "-1");
                cmd.Parameters.AddWithValue("@trxenddate", transactionenddate != "" ? transactionenddate : "-1");
                cmd.Parameters.AddWithValue("@enteredstartdate", createstartdate != "" ? createstartdate : "-1");
                cmd.Parameters.AddWithValue("@enteredenddate", createenddate != "" ? createenddate : "-1");
                cmd.Parameters.AddWithValue("@billmonth", lstbillmonth);
                cmd.Parameters.AddWithValue("@billyear", lstbillyear);
                cmd.Parameters.AddWithValue("@filterby", lstfilterby);
                cmd.Parameters.AddWithValue("@transactiontype", lsttransactiontype);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstTransactionFranchiseSearch.Add(new TransactionFranchiseSearch
                    {
                        id = int.Parse(reader["id"].ToString()),
                        franid = int.Parse(reader["franid"].ToString()),
                        trxdate = ScrubDateToString(DateTime.Parse(reader["trxdate"].ToString())),
                        unitprice = reader["unitprice"].ToString(),
                        tax = reader["tax"].ToString(),
                        trxtotal = reader["trxtotal"].ToString(),
                        description = reader["description"].ToString(),
                        billmonth = reader["billmonth"].ToString(),
                        billyear = reader["billyear"].ToString(),
                        name = reader["name"].ToString(),
                        number = reader["number"].ToString(),
                        franname = reader["franname"].ToString(),
                        customername = reader["customername"].ToString(),
                        customerno = reader["customerno"].ToString(),
                        customerid = int.Parse(reader["customerid"].ToString()),
                        invoiceno = reader["invoiceno"].ToString()
                    });


                }
                reader.Close();


            }
            return lstTransactionFranchiseSearch;
        }

        public IEnumerable<FranchiseeManualTransactionResultViewModel> GetManualTransactionPendingList(string RegionIds)
        {
            List<FranchiseeManualTransactionResultViewModel> lstTransaction = new List<FranchiseeManualTransactionResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", RegionIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_TransactionPendingList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstTransaction = multipleresult.Read<FranchiseeManualTransactionResultViewModel>().ToList();
                    }
                }

            }

            return lstTransaction;
        }

        public IEnumerable<TransactionFranchisePending> GetTransactionPendingList(string _importid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                IEnumerable<TransactionFranchisePending> lstTransactionFranchisePending = context.portal_spGet_F_TrxPostList(_importid).MapEnumerable<TransactionFranchisePending, portal_spGet_F_TrxPostList_Result>().ToList();
                return lstTransactionFranchisePending;
            }
        }
       

        #region Franchisee
        public IEnumerable<StatusList> GetFranchiseStatusListItem()
        {
            using (Data.DAL.jkDatabaseEntities context = new Data.DAL.jkDatabaseEntities())
            {
                return context.StatusLists.Where(st=>st.TypeListId==2).ToList();
            }
        }
        public IEnumerable<FranchiseListItem> GetTransactionTypeListItem(int? _franchiseid, int IsSearch = 0)
        {
            List<FranchiseListItem> lstTransactionType = new List<FranchiseListItem>();
            try
            {
                string connectionString = new EntityConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["jkDatabaseEntities"].ConnectionString).ProviderConnectionString;

                if (IsSearch == 0)
                    lstTransactionType.Add(new FranchiseListItem() { Id = "-1", Name = "All" });
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spGet_F_TrxType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@franid", _franchiseid);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lstTransactionType.Add(new FranchiseListItem
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["name"].ToString()
                        });
                    }
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
            }

            return lstTransactionType;
        }
        public IEnumerable<FranchiseListItem> GetYearListItem()
        {
            List<FranchiseListItem> lstYear = new List<FranchiseListItem>();
            try
            {
                lstYear.Add(new FranchiseListItem { Name = "-Select One-", Id = "-1" });
                using (jkDatabaseEntities context = new jkDatabaseEntities())
                {
                    var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
                    foreach (var y in yearList) { lstYear.Add(new FranchiseListItem { Name = y.ToString(), Id = y.ToString() }); }
                }
                    
            }
            catch (Exception ex)
            {
            }

            return lstYear;
        }
        public IEnumerable<FranchiseListItem> GetMonthListItem()
        {
            List<FranchiseListItem> lstMonth = new List<FranchiseListItem>();
            try
            {
                lstMonth.Add(new FranchiseListItem { Name = "-Select One-", Id = "-1" });
                lstMonth.AddRange(Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new FranchiseListItem { Name = v.ToString(), Id = ((int)v).ToString() }).ToList());
            }
            catch (Exception ex)
            {
            }

            return lstMonth;
        }

        public List<FrenchiseeDetailTransactionViewModel> GetFrenchiseeDetailTeasactions(int frenchiseeid, int typeId, System.DateTime startDate, System.DateTime endDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_TransactionDetail(frenchiseeid, typeId, startDate, endDate).MapEnumerable<FrenchiseeDetailTransactionViewModel, portal_spGet_F_TransactionDetail_Result>().ToList();

            }
        }
        #endregion


        #region Franchisee > General
        #endregion

        #region Franchisee > Chargebacks
        #endregion


        #region Franchisee > Negative Dues
        #endregion

        #region Franchisee > Transactions


        public IEnumerable<vFranchiseViewModel> FranchiseSearchByNumber(string Number)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                IEnumerable<vFranchiseViewModel> lstFranchiseViewModel = context.vw_F_Information.Where(i => i.number == Number).MapEnumerable<vFranchiseViewModel, vw_F_Information>().ToList();
                return lstFranchiseViewModel;
            }
        }

        public List<FranchiseTransaction> ListRecurringTransaction(int _franchiseid, int _transactiontype = -1)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<FranchiseTransaction> lstFranchiseTransaction = context.portal_spGet_F_TrxRecurringList(_franchiseid, _transactiontype).MapEnumerable<FranchiseTransaction, portal_spGet_F_TrxRecurringList_Result>().ToList();
                return lstFranchiseTransaction;
            }
        }
        public bool SaveTransaction(int? _id, int? _franid, int? _trxtypeid, DateTime? _trxdate, int? _credit, int? _resell, decimal? _quantity, decimal? _unitprice, decimal? _extendedprice, decimal? _subtotal, decimal? _tax, decimal? _trxtotal, string _description, int? _noofpayments, int? _paymentsbilled, DateTime? _startdate, int? _status, decimal? _grosstotal, int? _userid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var result = context.spSave_F_TrxRecurring(_id, _franid, _trxtypeid, _trxdate, _credit, _resell, _quantity, _unitprice, _extendedprice, _subtotal, _tax, _trxtotal, _description, _noofpayments, _paymentsbilled, _startdate, _status, _grosstotal, _userid).ToList().FirstOrDefault().Column1;
                if (result > 0)
                    return true;
                else
                    return false;
            }
        }


        public LeaseBillRunSummaryDetailViewModel GetLeaseBillRunSummaryDetail(int month, int year, int batchid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //var aaa = context.portal_spGet_F_LeaseBillRunSummaryDetail(RegionId, month, year, batchid);

                List<portal_spGet_F_LeaseBillRunSummaryDetail_Result> lst = context.portal_spGet_F_LeaseBillRunSummaryDetail(SelectedRegionId.ToString(), month, year, batchid).ToList();
                LeaseBillRunSummaryDetailViewModel oLeaseBillRunSummaryDetailViewModel = new LeaseBillRunSummaryDetailViewModel();
                if (batchid > 0)
                {
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseCount = (int)lst.FirstOrDefault().TotalLeaseCount;
                    oLeaseBillRunSummaryDetailViewModel.LeaseCreatedOn = lst.FirstOrDefault().LeaseCreatedOn != null ? lst.FirstOrDefault().LeaseCreatedOn.ToString() : "";
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseAmount));
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseTax = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseTax));

                    oLeaseBillRunSummaryDetailViewModel.BatchNumber = (long)lst.FirstOrDefault().BatchNumber;

                }
                else
                {
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseCount = (int)lst.Sum(a => a.TotalLeaseCount);
                    oLeaseBillRunSummaryDetailViewModel.LeaseCreatedOn = Convert.ToDateTime(lst.Max(a => a.LeaseCreatedOn)).ToString("MM/dd/yyyy hh:mm tt");

                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    Console.WriteLine("{0:c}", 4321.2);
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseAmount));
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseTax = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseTax));

                    oLeaseBillRunSummaryDetailViewModel.BatchNumber = 0;
                }


                return oLeaseBillRunSummaryDetailViewModel;
            }
        }

        public LeaseBillRunSummaryDetailViewModel GenerateInvoiceLeaseBillRun(int month, int year)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                ObjectParameter batchid = new ObjectParameter("BatchId", typeof(int));
                context.portal_spCreate_AR_GenerateBillRun_Lease(SelectedRegionId.ToString(), month, year, -1, 1, batchid);

                List<portal_spGet_F_LeaseBillRunSummaryDetail_Result> lst = context.portal_spGet_F_LeaseBillRunSummaryDetail(SelectedRegionId.ToString(), month, year, (int)batchid.Value).ToList();
                LeaseBillRunSummaryDetailViewModel oLeaseBillRunSummaryDetailViewModel = new LeaseBillRunSummaryDetailViewModel();
                if ((int)batchid.Value > 0 && lst != null && lst.Count > 0)
                {
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseCount = (int)lst.FirstOrDefault().TotalLeaseCount;
                    oLeaseBillRunSummaryDetailViewModel.LeaseCreatedOn = lst.FirstOrDefault().LeaseCreatedOn != null ? lst.FirstOrDefault().LeaseCreatedOn.ToString() : "";
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseAmount));
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseTax = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseTax));
                    oLeaseBillRunSummaryDetailViewModel.BatchNumber = (long)lst.FirstOrDefault().BatchNumber;

                }
                else
                {
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseCount = (int)lst.Sum(a => a.TotalLeaseCount);
                    oLeaseBillRunSummaryDetailViewModel.LeaseCreatedOn = Convert.ToDateTime(lst.Max(a => a.LeaseCreatedOn)).ToString("MM/dd/yyyy hh:mm tt");
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseAmount));
                    oLeaseBillRunSummaryDetailViewModel.TotalLeaseTax = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalLeaseTax));
                    var aae = lst.Select(l => new { BatchNumber = string.Join(",", l.BatchNumber.ToString().ToArray()) }).ToString();

                    oLeaseBillRunSummaryDetailViewModel.BatchNumber = 0;
                }

                return oLeaseBillRunSummaryDetailViewModel;
            }
        }


        public List<portal_spGet_F_LeaseList_Result> FranchiseeLeaseListData(bool? IsActive, String regionid, string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_LeaseList(IsActive, regionid, searchtext).ToList();
            }
        }


        public List<portal_spGet_F_LeaseListSearch_Result> FranchiseeLeaseListDataSearch(string statusIds, string regionid, string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_LeaseListSearch(statusIds, regionid, searchtext).ToList();
            }
        }

        public List<portal_spGet_F_LeaseBillReportList_Result> FranchiseeLeaseReportListData(string regionid, string searchtext, string startDate, string endDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_LeaseBillReportList(regionid, searchtext, DateTime.Parse(startDate), DateTime.Parse(endDate)).ToList();
            }
        }
        public List<portal_spGet_F_FranchiseeManualTransactionsList_Result> FranchiseeManualTransactionsListData(string regionid, string searchtext, string startDate, string endDate, int month = 0, int year = 0)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_FranchiseeManualTransactionsList(regionid, searchtext, DateTime.Parse(startDate).ToUniversalTime(), DateTime.Parse(endDate).ToUniversalTime(), month, year).ToList();
            }
        }

        public List<portal_spGet_F_FinderfeeReportList_Result> FranchiseeFinderFeeReportListData(string regionid, string startDate, string endDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_FinderfeeReportList(regionid, DateTime.Parse(startDate), DateTime.Parse(endDate)).ToList();
            }
        }
        public List<FinderfeeReportListResultModel> FranchiseeFinderFeeReportListDataForPrint(string regionid, string startDate, string endDate)
        {
            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    return context.portal_spGet_F_FinderfeeReportList(regionid, DateTime.Parse(startDate), DateTime.Parse(endDate)).ToList();
            //}

            List<FinderfeeReportListResultModel> lstData = new List<FinderfeeReportListResultModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionid);
                parmas.Add("@StartDate", startDate);
                parmas.Add("@EndDate", endDate);                
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_FinderfeeReportListForPrint", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstData = multipleresult.Read<FinderfeeReportListResultModel>().ToList();
                    }
                }
            }
            return lstData;
             
        }

        public List<spGet_Franchisee_Finderfee_ReportByFranchisee_Result> FranchiseeFinderFeeReportByFranchisee(int? FranchiseeId,string FromDate,string ToDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                DateTime? frmdt = DateTime.Parse(FromDate);
                DateTime? todt = DateTime.Parse(ToDate);
                return context.spGet_Franchisee_Finderfee_ReportByFranchisee(FranchiseeId,frmdt ,todt,null).ToList();
            }
        }

        public List<FranchiseeLeaseDetailViewModel> GetFranchiseeLeaseList(int FranchiseeId,bool? IsActive, int? StatusListId)
        {
            List<FranchiseeLeaseDetailViewModel> lstTransaction = new List<FranchiseeLeaseDetailViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LeaseId", 0);
                parmas.Add("@FranchiseeId", FranchiseeId);
                parmas.Add("@IsActive", IsActive);
                if(StatusListId != null)
                {
                    parmas.Add("@StatusListId", StatusListId);
                }
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_LeaseDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstTransaction = multipleresult.Read<FranchiseeLeaseDetailViewModel>().ToList();
                    }
                }

            }
            return lstTransaction;
           
        }

        public bool TransferFranchiseeLease(int FromFranchiseeId, int TOFranchiseeId, string LeaseIds)
        {
            List<FranchiseeLeaseDetailViewModel> lstTransaction = new List<FranchiseeLeaseDetailViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LeaseIds", LeaseIds);
                parmas.Add("@FromFranchiseeId", FromFranchiseeId);
                parmas.Add("@TOFranchiseeId", TOFranchiseeId);
                parmas.Add("@CreatedBy", LoginUserId);              


                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_TransferLeaseToAntherFranchisee", parmas, commandType: CommandType.StoredProcedure))
                {
                    //if (multipleresult != null)
                    //{
                    //    lstTransaction = multipleresult.Read<FranchiseeLeaseDetailViewModel>().ToList();
                    //}
                }

            }
            return true;

        }

        public FranchiseeLeaseDetailViewModel GetFranchiseeLeaseDetail(int LeaseId)
        {
            FranchiseeLeaseDetailViewModel lstTransaction = new FranchiseeLeaseDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LeaseId", LeaseId);
                parmas.Add("@FranchiseeId", 0);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_LeaseDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstTransaction = multipleresult.Read<FranchiseeLeaseDetailViewModel>().ToList().FirstOrDefault();
                    }
                }

            }
            return lstTransaction;
        }       

        #endregion

        #region Franchisee > Franchisee Report
        #endregion

        #region Franchisee > Customer Leads
        #endregion


        #region Helpers
        public static String FormatPhone(String value)
        {
            String s = "";
            if (value != null && value != "" && value.Trim().Length == 10)
            {
                value = value.Trim().Replace("(", "");
                value = value.Trim().Replace(")", "");
                value = value.Trim().Replace(" ", "");
                value = value.Trim().Replace("-", "");

                s = String.Format("{0:(###) ###-####}", double.Parse(value.Trim()));

            }
            return s;
        }
        public static String ScrubDateToString(DateTime dt)
        {
            String s = dt.ToShortDateString();
            if (dt.Year == 1900)
            {
                s = "";
            }
            return s;
        }
        #endregion

        public string getRegionName(int? id)
        {
            using (jkDatabaseEntities db = new jkDatabaseEntities())
            {
                var name = db.Regions.Where(x => x.RegionId == id).FirstOrDefault();
                return name.Acronym;
            }
        }

    }
}