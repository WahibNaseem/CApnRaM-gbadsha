using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;
using JKApi.Service.Helper.Extension;
using JKViewModels.Management;
using System.Data;
using System.Data.SqlClient;
using JKViewModels.Customer;
using JKApi.Service.ServiceContract.Management;
using JKApi.Data.DTOObject;
using JKViewModels;
using Dapper;
using JKViewModels.AccountsPayable;
using JKViewModels.Commission;

namespace JKApi.Service.Service.Management
{
    public class ManagementService : IManagementService
    {

        public int portal_spGet_R_CorporateDues(int? month, int? year)
        {
            using (var context = new jkDatabaseEntities())
            {
                var result = context.portal_spGet_R_CorporateDues(month, year);
                return result;
            }
        }

        //Gel Year list
        public IEnumerable<CorporateDuesSearch> GetYearListItem()
        {
            List<CorporateDuesSearch> lstYear = new List<CorporateDuesSearch>();

            lstYear.Add(new CorporateDuesSearch { Name = "-Select One-", Id = "" });
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var yearList = new List<int>() { 2014, 2015, 2016, 2017, 2018 };
                foreach (var y in yearList) { lstYear.Add(new CorporateDuesSearch { Name = y.ToString(), Id = y.ToString() }); }
            }


            return lstYear;
        }

        //Get Month list
        public IEnumerable<CorporateDuesSearch> GetMonthListItem()
        {
            List<CorporateDuesSearch> lstMonth = new List<CorporateDuesSearch>();

            lstMonth.Add(new CorporateDuesSearch { Name = "-Select One-", Id = "" });
            lstMonth.AddRange(Enum.GetValues(typeof(BillMonths)).Cast<BillMonths>().Select(v => new CorporateDuesSearch { Name = v.ToString(), Id = ((int)v).ToString() }).ToList());
            return lstMonth;
        }

        //Get CorporateDueslist
        public List<CorporateDuesListItem> getCorporateDuesList(string month, string year)
        {
            string spName = "portal_spGet_R_CorporateDues";
            List<CorporateDuesListItem> CorporateDuesList = new List<CorporateDuesListItem>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CorporateDuesListItem cdmodel = new CorporateDuesListItem();
                    cdmodel.Description = reader["Description"].ToString() == "-9" ? "" : reader["Description"].ToString();
                    cdmodel.Amount = (reader["Amount"]).ToString() == "" ? "" : Convert.ToDecimal(reader["Amount"]).ToString();
                    cdmodel.billmonth = (reader["Bill Month"]).ToString();
                    cdmodel.billyear = (reader["Bill Year"]).ToString();
                    cdmodel.typeid = (reader["typeid"]).ToString();
                    cdmodel.groupid = Convert.ToInt16(reader["groupid"]).ToString();
                    cdmodel.sortorder = Convert.ToInt16(reader["sortorder"]).ToString();
                    CorporateDuesList.Add(cdmodel);
                }


                return CorporateDuesList;
            }
        }

        //Get DeductionsList
        public List<DeductionsListItem> getDeductionsList(string month, string year)
        {
            string spName = "portal_spGet_R_MonthlyDeductions";
            List<DeductionsListItem> DeductionsList = new List<DeductionsListItem>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DeductionsListItem dmodel = new DeductionsListItem();
                    dmodel.Description = reader["Description"].ToString() == "-9" ? "" : reader["Description"].ToString();
                    dmodel.Amount = (reader["Amount"]).ToString() == "" ? "" : Convert.ToDecimal(reader["Amount"]).ToString();
                    dmodel.billmonth = (reader["Bill Month"]).ToString();
                    dmodel.billyear = (reader["Bill Year"]).ToString();
                    DeductionsList.Add(dmodel);
                }

                return DeductionsList;
            }
        }

        //Get DroList
        public List<DroListItem> getDroList(string month, string year)
        {
            string spName = "portal_spGet_R_DRO";
            List<DroListItem> DroList = new List<DroListItem>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DroListItem dmodel = new DroListItem();
                    dmodel.Description = reader["Description"].ToString() == "-9" ? "" : reader["Description"].ToString();
                    dmodel.Amount = (reader["Amount"]).ToString() == "" ? "" : Convert.ToDecimal(reader["Amount"]).ToString();
                    dmodel.billmonth = (reader["Bill Month"]).ToString();
                    dmodel.billyear = (reader["Bill Year"]).ToString();
                    dmodel.typeid = (reader["typeid"]).ToString();
                    DroList.Add(dmodel);
                }

                return DroList;
            }
        }

        //Get starecapList
        public List<StarecapListItem> getstarecapList(string month, string year)
        {
            string spName = "portal_spGet_R_STARecap";
            List<StarecapListItem> strList = new List<StarecapListItem>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StarecapListItem strmodel = new StarecapListItem();
                    strmodel.Description = reader["Description"].ToString() == "divider" ? "" : reader["Description"].ToString();
                    strmodel.Amount = (reader["Amount"]).ToString() == "-1.00" ? "" : (reader["Amount"]).ToString() == DBNull.Value.ToString() ? "" : Convert.ToDecimal(reader["Amount"]).ToString();
                    strmodel.Balance = (reader["Balance"]).ToString();
                    strmodel.grossrevenue = (reader["Gross Revenue"]).ToString();
                    strmodel.groupby = (reader["groupby"]).ToString();
                    strmodel.orderby = (reader["orderby"]).ToString();
                    strList.Add(strmodel);
                }

                return strList;
            }
        }

        //Get revenueList
        public List<RevenueListItem> getrevenueList(string month, string year)
        {
            string spName = "portal_spGet_R_RevenueList";
            List<RevenueListItem> revenueList = new List<RevenueListItem>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RevenueListItem rmodel = new RevenueListItem();
                    rmodel.Region_Name = reader["Region Name"].ToString() == "divider" ? "" : reader["Region Name"].ToString();
                    rmodel.BillingSubTotal = (reader["Billing Sub-Total"]).ToString() == "-1.00" ? "" : (reader["Billing Sub-Total"]).ToString() == DBNull.Value.ToString() ? "" : Convert.ToDecimal(reader["Billing Sub-Total"]).ToString();
                    rmodel.BillingTax = (reader["Billing Tax"]).ToString();
                    rmodel.BillingTotal = (reader["Billing Total"]).ToString();
                    rmodel.TotalTax = (reader["Total Tax"]).ToString();
                    rmodel.Total = (reader["Total"]).ToString();
                    revenueList.Add(rmodel);
                }

                return revenueList;
            }
        }

        //Get getleaseList
        public IEnumerable<Franchisee> GetLeaseList(string id)
        {
            int _id = Convert.ToInt32(id);
            using (var context = new jkDatabaseEntities())
            {
                var sameasme = context.Franchisees.Where(one => one.FranchiseeId == _id).ToList();
                return sameasme;
                //if (sameasme != null && sameasme == 1)
                //{
                //    if (sameasme == 1)
                //    {
                //        var result = (from f in context.tbl_F_Information
                //                      join p in context.tbl_F_Payee on f.id equals p.franid into fp
                //                      from p in fp.DefaultIfEmpty()
                //                      select new FranchisePayeeCollection
                //                      {

                //                          tbl_F_Information = f,
                //                          tbl_F_Payee = p
                //                      }).ToList();
                //        return result;
                //    }
                //    else
                //    {
                //        var result = (from f in context.tbl_F_Information
                //                      where f.id == _id
                //                      join p in context.tbl_F_Payee on f.id equals p.franid into fp
                //                      from p in fp.DefaultIfEmpty()
                //                      select new FranchisePayeeCollection
                //                      {

                //                          tbl_F_Information = f,
                //                          tbl_F_Payee = p
                //                      }).ToList();
                //        return result;
                //    }

                // }
                //else
                //{
                //    var result = (from f in context.tbl_F_Information 
                //                  join p in context.tbl_F_Payee on f.id equals p.franid into fp
                //                  from p in fp.DefaultIfEmpty()
                //                  select new FranchisePayeeCollection
                //                  {

                //                      tbl_F_Information = f,
                //                      tbl_F_Payee = p
                //                  }).ToList();
                //    return result;
                //} 
            }
        }
        public List<portal_spGet_F_Lease_Result> Get_F_Lease_Result(int franchiseeId)
        {
            string spName = "portal_spget_F_LeaseHistory";
            List<portal_spGet_F_Lease_Result> CorporateDuesList = new List<portal_spGet_F_Lease_Result>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@franchiseeId", franchiseeId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    portal_spGet_F_Lease_Result cdmodel = new portal_spGet_F_Lease_Result();
                    cdmodel.franchiseeleaseid = Convert.ToInt32(reader["leaseid"]);
                    cdmodel.leaseid = Convert.ToInt32(reader["leaseid"]);
                    cdmodel.description = (reader["Description"]).ToString();
                    CorporateDuesList.Add(cdmodel);
                }
                return CorporateDuesList;
            }
        }

        public List<portal_spget_F_LeaseHistory_New_Result> GetLeaseList_All(int id)
        {

            using (var context = new jkDatabaseEntities())
            {
                if (id != 0)
                {
                    var result = context.portal_spget_F_LeaseHistory_New(id).ToList();
                    return result;
                }
                else
                {
                    var result = context.portal_spget_F_LeaseHistory_New(0).ToList();
                    return result;
                }

            }
        }

        public List<portal_spGet_F_Information_Result> GetLeasePayment(int id)
        {
            using (var context = new jkDatabaseEntities())
            {
                if (id != 0)
                {
                    var result = context.portal_spGet_F_Information(id).ToList();
                    return result;
                }
                else
                {
                    var result = context.portal_spGet_F_Information(0).ToList();
                    return result;
                }

            }
        }

        public List<portal_spGet_R_RevenueList_Result> GetRevenueList_All(int billmonth, int billyear)
        {
            using (var context = new jkDatabaseEntities())
            {
                if (billmonth != 0 && billyear != 0)
                {
                    var result = context.portal_spGet_R_RevenueList(billmonth, billyear).ToList();
                    return result;
                }
                else
                {
                    return null;
                }


            }
        }
        public List<portal_spGet_R_MonthlyTax_Result> GetMonthlyTax_All(int billmonth, int billyear, int userid)
        {
            using (var context = new jkDatabaseEntities())
            {
                try
                {
                    if (billmonth != 0 && billyear != 0)
                    {
                        var result = context.portal_spGet_R_MonthlyTax(billmonth, billyear, userid).ToList() ?? null;
                        return result;
                    }

                }
                catch (Exception)
                {

                    return null;
                }
                return null;


            }
        }
        public List<LeaseListViewModel> getLeasePaymentsList(int searchby, string searchvalue, string transactionstartdate, string transactionenddate)
        {
            string spName = "portal_spGet_F_LeasePayments";
            List<LeaseListViewModel> revenueList = new List<LeaseListViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@searchby", searchby);
                cmd.Parameters.AddWithValue("@searchvalue", searchvalue);
                cmd.Parameters.AddWithValue("@transactionstartdate", transactionstartdate);
                cmd.Parameters.AddWithValue("@transactionenddate", transactionenddate);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LeaseListViewModel rmodel = new LeaseListViewModel();
                    rmodel.FranchiseNo = reader["Franchise No"].ToString();
                    rmodel.FranchiseName = reader["Franchise Name"].ToString();
                    rmodel.LeaseNo = (reader["Lease No"]).ToString();
                    rmodel.SerialNo = (reader["Serial No"]).ToString();
                    rmodel.PaymentBilled = reader["Payments Billed"].ToString();
                    rmodel.PaymentAmount = (reader["Payment Amount"]).ToString();
                    rmodel.PaymentTax = (reader["Tax"]).ToString();
                    rmodel.TotalPayment = (reader["Total"]).ToString();
                    revenueList.Add(rmodel);
                }

                return revenueList;
            }
        }

        public List<LeaseListViewModel> getrevenueList(int searchby, string searchvalue, string transactionstartdate, string transactionenddate)
        {
            throw new NotImplementedException();
        }


        //Get getleaseList
        public IEnumerable<portal_spget_F_LeaseHistory_all_Result> GetLeaseList_All(string id)
        {
            int _id = Convert.ToInt32(id);
            using (var context = new jkDatabaseEntities())
            {

                var result = context.portal_spget_F_LeaseHistory_all(-1).ToList();
                return result;

            }
        }

        //Get getleaseList
        public IEnumerable<portal_spget_F_LeaseHistory_Specific_Result> GetLeaseList_Specific()
        {

            using (var context = new jkDatabaseEntities())
            {

                var result = context.portal_spget_F_LeaseHistory_Specific().ToList();
                return result;

            }
        }

        public IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownData(int flagId, int? regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_DashboardChartDataDetails 1,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                int? region = 0;
                if (regionId == 0)
                {
                    region = null;
                }
                else
                {
                    region = regionId;
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = region,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flagId,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetBillingBreakdownByContractChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportData 2,@RegionId,@StartDate,@EndDate,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetBillingBreakdownBySizeChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportData 1,@RegionId,@StartDate,@EndDate,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRevenueByMonthChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDataForManagement 1,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRevenueByYearChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDataForManagement 2,@RegionId,@StartDate,@EndDate,NULL,@YearToAdd,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    YearToAdd = yearToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRevenueWiseTopCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportForManagement 1,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetRevenueWiseTopCustomerChartDataDetails(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportDetailsForManagement 2,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }


        public IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownBySizeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportDetailsData 1,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flagId,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownByContractData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportDetailsData 2,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flagId,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public List<CorporationDuesReportViewModel> GetCorporationDuesReportData(string RegionIds, int Periodid)
        {

            List<CorporationDuesReportViewModel> Reportlist = new List<CorporationDuesReportViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", RegionIds);
                parmas.Add("@Periodid", Periodid);
                Reportlist = conn.Query<CorporationDuesReportViewModel>("dbo.portal_spGet_CorporationDuesReport", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
            }
            return Reportlist;
        }
        public List<NegativeDueReportViewModel> GetNegativeDueCollectedReportData(string RegionIds, string fromMonth, string fromYear, string toMonth, string toYear)
        {

            List<NegativeDueReportViewModel> Reportlist = new List<NegativeDueReportViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", RegionIds);
                parmas.Add("@fromMonth", fromMonth);
                parmas.Add("@fromYear", fromYear);
                parmas.Add("@toMonth", toMonth);
                parmas.Add("@toYear", toYear);
                Reportlist = conn.Query<NegativeDueReportViewModel>("dbo.portal_spGet_NegativeDueReport", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
            }
            return Reportlist;
        }

        public List<Period> getPeriod(int PeriodId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var data = context.Periods.Where(x => x.PeriodId == PeriodId).ToList();
                return data;
            }
        }
        public List<GetPercentPaidByDateReport> GetPercentageDailyReportbyMonthly(string[] RegionIds, string StartDate, string EndDate)
        {
            List<GetPercentPaidByDateReport> ReportList = new List<GetPercentPaidByDateReport>();
            //using (var context = new jkDatabaseEntities())
            //{
            //    var data = context.SpGetPercentPaidByDateReport(Convert.ToDateTime(StartDate), Convert.ToDateTime(EndDate)).ToList();

            //}
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@StartDate", StartDate);
                parmas.Add("@EndDate", EndDate);

                ReportList = conn.Query<GetPercentPaidByDateReport>("dbo.SpGetPercentPaidByDateReport", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
            }
            return ReportList;
        }

        public IEnumerable<DashboardModel> GetAccountTypeWiseChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportData 3,@RegionId,@StartDate,@EndDate,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetBillingAccountRevenueByAccountTypeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportDetailsData 3,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flagId,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRevenueAndPaymentChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDataForManagement 3,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRegionWiseYearRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 2,@RegionId,@StartDate,@EndDate,NULL,@YearToAdd,NULL,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    YearToAdd = yearToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetRegionWiseMonthlyRevenueChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 1,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,NULL,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetTopCustomerRevenueChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportForManagement 2,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetNewAndCanceledCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportForManagement 3,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetTopPaymentWiseCustomer(int? recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportForManagement 4,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetGrossAndContractBillingRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDataForManagement 4,@RegionId,@StartDate,@EndDate,NULL,@YearToAdd,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    YearToAdd = yearToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetMonthlyDataForASelectedYearChartData(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 4,@RegionId,@StartDate,@EndDate,NULL,NULL,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByCustomerData(string flag, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 5,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByAccountTypeData(string flag, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 6,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByAccountTypeAndCustomerData(string flag, int accountTypeListId, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 7,@RegionId,@StartDate,@EndDate,@MonthToAdd,NULL,@Flag,@BillMonth,@BillYear,@AccountTypeListId";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    MonthToAdd = monthToAdd,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear,
                    AccountTypeListId = accountTypeListId
                });
            }
        }

        public IEnumerable<DashboardModel> GetRegionWiseYearRevenueComparisonChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDetailsDataForManagement 8,@RegionId,@StartDate,@EndDate,NULL,@YearToAdd,NULL,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    YearToAdd = yearToAdd,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }


        public CommissionCustomerDetailViewModel GetCommissionScheduleCustomerData(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", customerId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionScheduleCustomerDetail", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionCustomerDetailViewModel>().ToList().FirstOrDefault();
                    }
                }
            }
            return new CommissionCustomerDetailViewModel();
        }

        public List<UserViewModel> GetCommissionScheduleSalesPerssonList(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionScheduleSalesPerssonList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<UserViewModel>().ToList();
                    }
                }
            }
            return new List<UserViewModel>();
        }

        public List<UserViewModel> GetCommSchSalesPerssonList(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommSchSalesPerssonList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<UserViewModel>().ToList();
                    }
                }
            }
            return new List<UserViewModel>();
        }

        public List<CompensationTypeViewModel> DeleteCompensationTypeListData(int CompensationTypeListId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CompensationTypeListId", CompensationTypeListId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_DeleteCompensationTypeList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CompensationTypeViewModel>().ToList();
                    }
                }
            }
            return new List<CompensationTypeViewModel>();
        }
        public List<PaymentScheduleTypeViewModel> GetPaymentScheduleTypeListData()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionPaymentScheduleTypeList", commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<PaymentScheduleTypeViewModel>().ToList();
                    }
                }
            }
            return new List<PaymentScheduleTypeViewModel>();
        }

        public List<CompensationTypeViewModel> GetCompensationTypeListData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCompensationTypeList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CompensationTypeViewModel>().ToList();
                    }
                }
            }
            return new List<CompensationTypeViewModel>();
        }
        public List<CompensationTypeViewModel> InsertCompensationTypeListData(CompensationTypeViewModel _inputData)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CompensationTypeListId", _inputData.CompensationTypeListId);
                parmas.Add("@Description", _inputData.Description);
                parmas.Add("@StatusListId", _inputData.StatusListId);
                parmas.Add("@IncludeinTotalSales", _inputData.IncludeinTotalSales);
                parmas.Add("@VariableSales", _inputData.VariableSales);
                parmas.Add("@CommissionBasedonTotalSale", _inputData.CommissionBasedonTotalSale);
                parmas.Add("@UserSpecific", _inputData.UserSpecific);
                parmas.Add("@StartDateSpecific", _inputData.StartDateSpecific);
                parmas.Add("@IsActive", _inputData.IsActive);
                parmas.Add("@UserId ", _inputData.CreatedBy);
                parmas.Add("@RegionId ", _inputData.RegionId);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_InsertUpdateCompensationTypeList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CompensationTypeViewModel>().ToList();
                    }
                }
            }
            return new List<CompensationTypeViewModel>();
        }
        public List<CommissionPaymentScheduleViewModel> GetCommissionPaymentPlanData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionPaymentPlan", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionPaymentScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionPaymentScheduleViewModel>();
        }




        public List<CommissionCompensationScheduleViewModel> GetCommissionCompensationScheduleData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionCompensationSchedule", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionCompensationScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionCompensationScheduleViewModel>();
        }
        public List<CommissionCompensationScheduleViewModel> DeleteCommissionCompensationScheduleData(int CompensationScheduleId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CompensationScheduleId", CompensationScheduleId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_DeleteCommissionCompensationSchedule", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionCompensationScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionCompensationScheduleViewModel>();
        }
        public List<CommissionCompensationScheduleViewModel> InsertCommissionCompensationScheduleData(CommissionCompensationScheduleViewModel _inputData)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CommissionCompensationScheduleId", _inputData.CommissionCompensationScheduleId);
                parmas.Add("@CompensationTypeListId", _inputData.CompensationTypeListId);
                parmas.Add("@Description", _inputData.Description);
                parmas.Add("@RangeStartAmount", _inputData.RangeStartAmount);
                parmas.Add("@RangeEndAmount", _inputData.RangeEndAmount);
                parmas.Add("@CompensationAmount", _inputData.CompensationAmount);
                parmas.Add("@@CommissionPaymentScheduleId", _inputData.CommissionPaymentScheduleId);
                parmas.Add("@StatusListId", _inputData.StatusListId);
                parmas.Add("@IsActive", _inputData.IsActive);
                parmas.Add("@UserId ", _inputData.CreatedBy);
                parmas.Add("@RegionId ", _inputData.RegionId);
                parmas.Add("@CompensationAmountTypeId ", _inputData.CompensationAmountTypeId);


                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_InsertUpdateCommissionCompensationSchedule", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionCompensationScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionCompensationScheduleViewModel>();
        }

        public List<AdditionalBonusScheduleViewModel> GetCommissionAdditionalBonusScheduleData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId ", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetAdditionalBonusSchedule", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<AdditionalBonusScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<AdditionalBonusScheduleViewModel>();
        }
        public List<AdditionalBonusScheduleViewModel> InsertCommissionAdditionalBonusScheduleData(AdditionalBonusScheduleViewModel _inputData)
        {
            using (var _context = new jkDatabaseEntities())
            {
                CommissionAdditionalBonusSchedule oCommissionAdditionalBonusSchedule = _context.CommissionAdditionalBonusSchedules.FirstOrDefault(o => o.CommissionAdditionalBonusScheduleId == _inputData.CommissionAdditionalBonusScheduleId);
                if (oCommissionAdditionalBonusSchedule != null)
                {
                    oCommissionAdditionalBonusSchedule.Amount = _inputData.Amount;
                    oCommissionAdditionalBonusSchedule.CreatedBy = _inputData.CreatedBy;
                    oCommissionAdditionalBonusSchedule.CreatedDate = DateTime.Now;
                    oCommissionAdditionalBonusSchedule.IsActive = _inputData.IsActive;
                    oCommissionAdditionalBonusSchedule.RangeEndAmount = _inputData.RangeEndAmount;
                    oCommissionAdditionalBonusSchedule.RangeStartAmount = _inputData.RangeStartAmount;
                    oCommissionAdditionalBonusSchedule.RegionId = _inputData.RegionId;
                    oCommissionAdditionalBonusSchedule.StatusListId = _inputData.StatusListId;
                    _context.SaveChanges();
                }
                else
                {
                    oCommissionAdditionalBonusSchedule = new CommissionAdditionalBonusSchedule();
                    oCommissionAdditionalBonusSchedule.Amount = _inputData.Amount;
                    oCommissionAdditionalBonusSchedule.CreatedBy = _inputData.CreatedBy;
                    oCommissionAdditionalBonusSchedule.CreatedDate = DateTime.Now;
                    oCommissionAdditionalBonusSchedule.IsActive = _inputData.IsActive;
                    oCommissionAdditionalBonusSchedule.RangeEndAmount = _inputData.RangeEndAmount;
                    oCommissionAdditionalBonusSchedule.RangeStartAmount = _inputData.RangeStartAmount;
                    oCommissionAdditionalBonusSchedule.RegionId = _inputData.RegionId;
                    oCommissionAdditionalBonusSchedule.StatusListId = _inputData.StatusListId;
                    _context.CommissionAdditionalBonusSchedules.Add(oCommissionAdditionalBonusSchedule);
                    _context.SaveChanges();

                }

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId ", _inputData.RegionId);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetAdditionalBonusSchedule", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return multipleresult.Read<AdditionalBonusScheduleViewModel>().ToList();
                        }
                    }
                }

            }


            return new List<AdditionalBonusScheduleViewModel>();
        }
        public List<AdditionalBonusScheduleViewModel> DeleteCommissionAdditionalBonusScheduleData(int AdditionalBonusScheduleId, int regionId)
        {
            using (var _context = new jkDatabaseEntities())
            {
                CommissionAdditionalBonusSchedule oCommissionAdditionalBonusSchedule = _context.CommissionAdditionalBonusSchedules.FirstOrDefault(o => o.CommissionAdditionalBonusScheduleId == AdditionalBonusScheduleId);
                if (oCommissionAdditionalBonusSchedule != null)
                {
                    _context.CommissionAdditionalBonusSchedules.Remove(oCommissionAdditionalBonusSchedule);
                    _context.SaveChanges();
                }

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId ", regionId);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetAdditionalBonusSchedule", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return multipleresult.Read<AdditionalBonusScheduleViewModel>().ToList();
                        }
                    }
                }

            }
            return new List<AdditionalBonusScheduleViewModel>();
        }

        public decimal GetAdditionalBonusAmount(decimal ContractAmount, int regionId)
        {
            decimal retval = 0;
            using (var _context = new jkDatabaseEntities())
            {
                CommissionAdditionalBonusSchedule oCommissionAdditionalBonusSchedule = _context.CommissionAdditionalBonusSchedules.FirstOrDefault(o => (decimal)o.RangeStartAmount <= ContractAmount && (decimal)o.RangeEndAmount >= ContractAmount);
                if (oCommissionAdditionalBonusSchedule != null)
                {
                    retval = (decimal)oCommissionAdditionalBonusSchedule.Amount;
                }
            }
            return retval;
        }


        public CommissionScheduleListViewModel GetCommissionScheduleListData(int regionId, int periodId)
        {
            CommissionScheduleListViewModel oCommissionScheduleListViewModel = new CommissionScheduleListViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@PeriodId", periodId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionScheduleList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommissionScheduleListViewModel.lstSchedules = multipleresult.Read<CommissionScheduleViewModel>().ToList();
                        oCommissionScheduleListViewModel.lstContracts = multipleresult.Read<CommissionScheduleViewModel>().ToList();
                        return oCommissionScheduleListViewModel;
                    }
                }
            }
            oCommissionScheduleListViewModel = new CommissionScheduleListViewModel();
            oCommissionScheduleListViewModel.lstSchedules = new List<CommissionScheduleViewModel>();
            oCommissionScheduleListViewModel.lstContracts = new List<CommissionScheduleViewModel>();

            return oCommissionScheduleListViewModel;
        }

        public List<SalesPersonCommSchViewModel> GetSalesPersonCommSchListData(int regionId)
        {
            List<SalesPersonCommSchViewModel> lstSalesPersonCommissionSchedule = new List<SalesPersonCommSchViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetSalesPersonCommSchList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstSalesPersonCommissionSchedule = multipleresult.Read<SalesPersonCommSchViewModel>().ToList();
                        return lstSalesPersonCommissionSchedule;
                    }
                }
            }
            lstSalesPersonCommissionSchedule = new List<SalesPersonCommSchViewModel>();

            return lstSalesPersonCommissionSchedule;
        }
        public List<SalesPersonBonusCommissionScheduleViewModel> GetSalesPersonCommSchBonusListData(int regionId)
        {
            List<SalesPersonBonusCommissionScheduleViewModel> lstSalesPersonBonusCommissionSchedule = new List<SalesPersonBonusCommissionScheduleViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetSalesPersonBonusCommSchList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstSalesPersonBonusCommissionSchedule = multipleresult.Read<SalesPersonBonusCommissionScheduleViewModel>().ToList();
                        return lstSalesPersonBonusCommissionSchedule;
                    }
                }
            }
            lstSalesPersonBonusCommissionSchedule = new List<SalesPersonBonusCommissionScheduleViewModel>();
            return lstSalesPersonBonusCommissionSchedule;
        }
        public SalesPersonBonusCommissionScheduleViewModel GetSalesPersonBonusCommissionScheduleData(int SalesPersonBonusCommissionScheduleId, int regionId)
        {
            SalesPersonBonusCommissionScheduleViewModel oSalesPersonBonusCommissionSchedule = new SalesPersonBonusCommissionScheduleViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SalesPersonBonusCommissionScheduleId", SalesPersonBonusCommissionScheduleId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetSalesPersonBonusCommissionSchedule", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oSalesPersonBonusCommissionSchedule = multipleresult.Read<SalesPersonBonusCommissionScheduleViewModel>().ToList().FirstOrDefault();
                        if (oSalesPersonBonusCommissionSchedule != null)
                            return oSalesPersonBonusCommissionSchedule;
                    }
                }
            }
            oSalesPersonBonusCommissionSchedule = new SalesPersonBonusCommissionScheduleViewModel();

            return oSalesPersonBonusCommissionSchedule;
        }



        public SalesPersonCommSchViewModel GetSalesPersonCommSchData(int SalePersonId, int regionId)
        {
            SalesPersonCommSchViewModel lstSalesPersonCommissionSchedule = new SalesPersonCommSchViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SalePersonId", regionId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetSalesPersonCommSch", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstSalesPersonCommissionSchedule = multipleresult.Read<SalesPersonCommSchViewModel>().ToList().FirstOrDefault();
                        return lstSalesPersonCommissionSchedule;
                    }
                }
            }
            lstSalesPersonCommissionSchedule = new SalesPersonCommSchViewModel();

            return lstSalesPersonCommissionSchedule;
        }

        public bool CheckSalesPersonCommSchData(int SalesPersonCommSchId, int SalesPersonId, int ContractTypeId, int RegionId)
        {
            SalesPersonCommSchViewModel oSalesPersonCommSchViewModel = new SalesPersonCommSchViewModel();
            using (var _context = new jkDatabaseEntities())
            {

                if (_context.SalesPersonCommissionSchedules.Where(o => o.SalesPersonCommissionScheduleId != SalesPersonCommSchId
                 && o.SalesPersonId == SalesPersonId && o.ContractTypeListId == ContractTypeId && o.RegionId == RegionId).ToList().Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool CheckSalesPersonBonusCommSchData(int SalesPersonBonusCommissionScheduleId, int SalesPersonId, int ContractTypeId, int RegionId)
        {
            SalesPersonCommSchViewModel oSalesPersonCommSchViewModel = new SalesPersonCommSchViewModel();
            using (var _context = new jkDatabaseEntities())
            {

                if (_context.SalesPersonBonusCommissionSchedules.Where(o => o.SalesPersonBonusCommissionScheduleId != SalesPersonBonusCommissionScheduleId
                 && o.SalesPersonId == SalesPersonId && o.ContractTypeListId == ContractTypeId && o.RegionId == RegionId).ToList().Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }




        public SalesPersonCommSchViewModel GetSalesPersonCommSchData(int SalesPersonCommSchId)
        {
            SalesPersonCommSchViewModel oSalesPersonCommSchViewModel = new SalesPersonCommSchViewModel();
            using (var _context = new jkDatabaseEntities())
            {

                SalesPersonCommissionSchedule oSalesPersonCommissionSchedules = _context.SalesPersonCommissionSchedules.FirstOrDefault(o => o.SalesPersonCommissionScheduleId == SalesPersonCommSchId);
                if (oSalesPersonCommissionSchedules != null)
                {
                    oSalesPersonCommSchViewModel.CommissionCompensationScheduleId = (int)oSalesPersonCommissionSchedules.CommissionCompensationScheduleId;
                    oSalesPersonCommSchViewModel.EffectiveDate = oSalesPersonCommissionSchedules.EffectiveDate;
                    oSalesPersonCommSchViewModel.IsActive = oSalesPersonCommissionSchedules.IsActive;
                    oSalesPersonCommSchViewModel.RegionId = (int)oSalesPersonCommissionSchedules.RegionId;
                    oSalesPersonCommSchViewModel.SalesPersonCommSchId = oSalesPersonCommissionSchedules.SalesPersonCommissionScheduleId;
                    oSalesPersonCommSchViewModel.SalesPersonId = (int)oSalesPersonCommissionSchedules.SalesPersonId;
                    oSalesPersonCommSchViewModel.StatusListId = (int)oSalesPersonCommissionSchedules.StatusListId;
                }
            }
            return oSalesPersonCommSchViewModel;
        }


        public List<CommissionPaymentScheduleViewModel> GetCommissionPaymentScheduleListData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionPaymentScheduleList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionPaymentScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionPaymentScheduleViewModel>();
        }

        public bool DeleteCommissionScheduleListData(int CommissionScheduleId, int regionId)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CommissionScheduleId", CommissionScheduleId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_DeleteCommissionScheduleList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool DeleteCommissionScheduleContractListData(int ContarctId, int regionId)
        {

            using (var _context = new jkDatabaseEntities())
            {

                var _lstCC = _context.Contracts.SingleOrDefault(o => o.ContractId == ContarctId);
                if (_lstCC != null)
                {
                    _lstCC.CommissionTransactionStatusListId = 3;
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public List<CommissionScheduleViewModel> DeleteCommissionPaymentScheduleListData(int CommissionPaymentScheduleId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CommissionPaymentScheduleId", CommissionPaymentScheduleId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_DeleteCommissionPaymentScheduleList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionScheduleViewModel>();
        }


        public CommissionScheduleViewModel GetCommissionScheduleData(int CommissionScheduleId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCompensationTypeList", commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionScheduleViewModel>().ToList().FirstOrDefault();
                    }
                }
            }
            return new CommissionScheduleViewModel();
        }

        public List<CommissionCompensationScheduleViewModel> GetCommissionCompensationScheduleDropdown(int regionId)
        {
            List<CommissionCompensationScheduleViewModel> lstddlList = new List<CommissionCompensationScheduleViewModel>();
            using (var _context = new jkDatabaseEntities())
            {

                var _lstCC = _context.CommissionCompensationSchedules.Where(o => o.RegionId == regionId).ToList();
                CommissionCompensationScheduleViewModel obj;
                foreach (var item in _lstCC)
                {
                    obj = new CommissionCompensationScheduleViewModel();
                    obj.CommissionCompensationScheduleId = item.CommissionCompensationScheduleId;
                    obj.Description = item.Description;

                    lstddlList.Add(obj);
                }
            }
            return lstddlList;
        }

        public List<CommissionAdditionalBonusScheduleViewModel> GetCommissionAdditionalBonusScheduleDropdown(int regionId)
        {
            List<CommissionAdditionalBonusScheduleViewModel> lstddlList = new List<CommissionAdditionalBonusScheduleViewModel>();
            using (var _context = new jkDatabaseEntities())
            {

                var _lstCC = _context.CommissionAdditionalBonusSchedules.Where(o => o.RegionId == regionId).ToList();
                CommissionAdditionalBonusScheduleViewModel obj;
                foreach (var item in _lstCC)
                {
                    obj = new CommissionAdditionalBonusScheduleViewModel();
                    obj.CommissionAdditionalBonusScheduleId = item.CommissionAdditionalBonusScheduleId;
                    obj.Description = "$ " + item.RangeStartAmount + " to $ " + item.RangeEndAmount + " - (Bonus $ " + item.Amount + ")";

                    lstddlList.Add(obj);
                }
            }
            return lstddlList;
        }



        public List<CurrentScheduledCommissionViewModel> GetCurrentScheduledCommissionData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCurrentScheduledCommissionList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CurrentScheduledCommissionViewModel>().ToList();
                    }
                }
            }
            return new List<CurrentScheduledCommissionViewModel>();
        }

        public List<CurrentScheduledCommissionViewModel> GetCurrentScheduledCommissionReviewData(int regionId, int periodId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@PeriodId", periodId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCurrentScheduledCommissionList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CurrentScheduledCommissionViewModel>().ToList();
                    }
                }
            }
            return new List<CurrentScheduledCommissionViewModel>();
        }



        public ScheduledCommissionGenerateViewModel GetCurrentScheduledCommissionGenerateData(int regionId, int periodId)
        {
            ScheduledCommissionGenerateViewModel Item = new ScheduledCommissionGenerateViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@PeriodId", periodId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCurrentScheduledCommissionGenerateList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        Item.lstScheduledCommission = multipleresult.Read<CurrentScheduledCommissionViewModel>().ToList();
                        Item.lstScheduledCommissionProcessed = multipleresult.Read<CurrentPaymentHistoryCommissionViewModel>().ToList();
                        return Item;
                    }
                }
            }
            Item.lstScheduledCommission = new List<CurrentScheduledCommissionViewModel>();
            Item.lstScheduledCommissionProcessed = new List<CurrentPaymentHistoryCommissionViewModel>();
            return Item;
        }

        public bool GenerateCurrentScheduledCommissionData(int regionId, int periodId)
        {
            bool retVal = true;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@PeriodId", periodId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GenerateCurrentScheduledCommission", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return retVal;
                    }
                }
            }
            retVal = false;
            return retVal;
        }

        public bool ImportCommissionScheduleList(int regionId)
        {
            bool retVal = true;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_ImportCommissionScheduleList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return retVal;
                    }
                }
            }
            retVal = false;
            return retVal;
        }

        public bool InsertCommissionScheduleData(CommissionScheduleViewModel _inputData)
        {
            bool retVal = false;
            using (var _context = new jkDatabaseEntities())
            {

                CommissionSchedule oCommissionSchedule = _context.CommissionSchedules.FirstOrDefault(o => o.CommissionScheduleId == _inputData.CommissionScheduleId);

                CommissionCompensationSchedule oCommissionCompensationSchedule = _context.CommissionCompensationSchedules.FirstOrDefault(o => o.CommissionCompensationScheduleId == _inputData.CommissionCompensationScheduleId);
                CommissionPaymentSchedule CommissionPaymentSchedule = _context.CommissionPaymentSchedules.FirstOrDefault(o => o.CommissionPaymentScheduleId == oCommissionCompensationSchedule.CommissionPaymentScheduleId);
                int TotalPayout = (int)(CommissionPaymentSchedule.PaymentScheduleTypeId == 1 ? 1 :
                        ((CommissionPaymentSchedule.Term4_EndMonth > 0 ? CommissionPaymentSchedule.Term4_EndMonth :
                        (CommissionPaymentSchedule.Term3_EndMonth > 0 ? CommissionPaymentSchedule.Term3_EndMonth :
                        (CommissionPaymentSchedule.Term2_EndMonth > 0 ? CommissionPaymentSchedule.Term2_EndMonth :
                        (CommissionPaymentSchedule.Term1_EndMonth > 0 ? CommissionPaymentSchedule.Term1_EndMonth : 1))))));


                if (oCommissionSchedule != null)
                {
                    oCommissionSchedule.CommissionCompensationScheduleId = _inputData.CommissionCompensationScheduleId;
                    oCommissionSchedule.CompensationTypeListId = _inputData.CompensationTypeListId;
                    oCommissionSchedule.ContractId = _inputData.ContractId;
                    oCommissionSchedule.ContractAmount = _inputData.ContractAmount;
                    oCommissionSchedule.ContractStartDate = _inputData.ContractStartDate;
                    oCommissionSchedule.CustomerId = _inputData.CustomerId;
                    oCommissionSchedule.Description = _inputData.Description;
                    oCommissionSchedule.IsActive = _inputData.IsActive;
                    oCommissionSchedule.RegionId = _inputData.RegionId;
                    oCommissionSchedule.ModifiedBy = _inputData.CreatedBy;
                    oCommissionSchedule.ModifiedDate = _inputData.CreatedDate;
                    oCommissionSchedule.Notes = _inputData.Notes;                    
                    oCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oCommissionSchedule.HasAdditionalBonus = (_inputData.BonusAmount > 0 ? true : false);
                    oCommissionSchedule.BonusAmount = _inputData.BonusAmount;
                    oCommissionSchedule.BonusDescription = _inputData.BonusDescription;
                    oCommissionSchedule.BonusExplanation = _inputData.BonusExplanation;

                    _context.SaveChanges();

                    retVal = true;
                }
                else
                {
                    oCommissionSchedule = new CommissionSchedule();
                    oCommissionSchedule.CommissionCompensationScheduleId = _inputData.CommissionCompensationScheduleId;
                    oCommissionSchedule.CompensationTypeListId = _inputData.CompensationTypeListId;
                    oCommissionSchedule.ContractId = _inputData.ContractId;
                    oCommissionSchedule.ContractAmount = _inputData.ContractAmount;
                    oCommissionSchedule.ContractStartDate = _inputData.ContractStartDate;
                    oCommissionSchedule.CustomerId = _inputData.CustomerId;
                    oCommissionSchedule.Description = _inputData.Description;
                    oCommissionSchedule.IsActive = _inputData.IsActive;
                    oCommissionSchedule.RegionId = _inputData.RegionId;
                    oCommissionSchedule.ModifiedBy = _inputData.CreatedBy;
                    oCommissionSchedule.ModifiedDate = _inputData.CreatedDate;
                    oCommissionSchedule.Notes = _inputData.Notes;
                    oCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oCommissionSchedule.HasAdditionalBonus = (_inputData.BonusAmount > 0 ? true : false);
                    oCommissionSchedule.BonusAmount = _inputData.BonusAmount;
                    oCommissionSchedule.BonusDescription = _inputData.BonusDescription;
                    oCommissionSchedule.BonusExplanation = _inputData.BonusExplanation;
                    oCommissionSchedule.CurrentPayoutNum = 0;
                    oCommissionSchedule.TotalPayoutNum = TotalPayout;
                    oCommissionSchedule.CreatedBy = _inputData.CreatedBy;
                    oCommissionSchedule.CreatedDate = _inputData.CreatedDate;

                    _context.CommissionSchedules.Add(oCommissionSchedule);
                    _context.SaveChanges();

                    retVal = true;
                }

            }



            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var parmas = new DynamicParameters();
            //    parmas.Add("@CommissionScheduleId", _inputData.CommissionScheduleId);
            //    parmas.Add("@CustomerId", _inputData.CustomerId);
            //    parmas.Add("@ContractAmount", _inputData.ContractAmount);
            //    parmas.Add("@CompensationTypeListId", _inputData.CompensationTypeListId);
            //    parmas.Add("@CommissionCompensationScheduleId", _inputData.CommissionCompensationScheduleId);
            //    parmas.Add("@ContractId", _inputData.ContractId);
            //    parmas.Add("@SalesPersonId", _inputData.SalesPersonId);
            //    parmas.Add("@Notes", _inputData.Notes);
            //    parmas.Add("@Description", _inputData.Description);
            //    parmas.Add("@StatusListId ", _inputData.StatusListId);
            //    parmas.Add("@IsActive", _inputData.IsActive);
            //    parmas.Add("@UserId ", _inputData.CreatedBy);
            //    parmas.Add("@RegionId ", _inputData.RegionId);
            //    parmas.Add("@BonusDescription ", _inputData.BonusDescription);
            //    parmas.Add("@BonusAmount ", _inputData.BonusAmount);
            //    parmas.Add("@BonusExplanation ", _inputData.BonusExplanation);
            //    parmas.Add("@ContractStartDate ", _inputData.ContractStartDate);

            //    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_InsertUpdateCommissionSchedule", parmas, commandType: CommandType.StoredProcedure))
            //    {
            //        if (multipleresult != null)
            //        {
            //            return true;
            //        }
            //    }
            //}
            return retVal;
        }

        public List<SalesPersonCommSchViewModel> DeleteSalesPersonCommSchListData(int SalesPersonCommSchId, int regionId)
        {
            using (var _context = new jkDatabaseEntities())
            {
                SalesPersonCommissionSchedule oSalesPersonCommissionSchedule = _context.SalesPersonCommissionSchedules.FirstOrDefault(o => o.SalesPersonCommissionScheduleId == SalesPersonCommSchId);
                if (oSalesPersonCommissionSchedule != null)
                {
                    _context.SalesPersonCommissionSchedules.Remove(oSalesPersonCommissionSchedule);
                    _context.SaveChanges();
                }
            }
            return GetSalesPersonCommSchListData(regionId);
        }

        public List<SalesPersonBonusCommissionScheduleViewModel> DeleteSalesPersonBonusCommSchListData(int SalesPersonBonusCommissionScheduleId, int regionId)
        {
            using (var _context = new jkDatabaseEntities())
            {
                SalesPersonBonusCommissionSchedule oSalesPersonBonusCommissionSchedule = _context.SalesPersonBonusCommissionSchedules.FirstOrDefault(o => o.SalesPersonBonusCommissionScheduleId == SalesPersonBonusCommissionScheduleId);
                if (oSalesPersonBonusCommissionSchedule != null)
                {
                    _context.SalesPersonBonusCommissionSchedules.Remove(oSalesPersonBonusCommissionSchedule);
                    _context.SaveChanges();
                }
            }
            return GetSalesPersonCommSchBonusListData(regionId);
        }
        public List<SalesPersonCommSchViewModel> InsertSalesPersonCommSchData(SalesPersonCommSchViewModel _inputData)
        {
            List<SalesPersonCommSchViewModel> lstSalesPersonCommSchViewModel = new List<SalesPersonCommSchViewModel>();

            using (var _context = new jkDatabaseEntities())
            {
                SalesPersonCommissionSchedule oSalesPersonCommissionSchedule = _context.SalesPersonCommissionSchedules.FirstOrDefault(o => o.SalesPersonCommissionScheduleId == _inputData.SalesPersonCommSchId);
                if (oSalesPersonCommissionSchedule != null)
                {
                    oSalesPersonCommissionSchedule.CommissionCompensationScheduleId = _inputData.CommissionCompensationScheduleId;
                    oSalesPersonCommissionSchedule.CreatedBy = _inputData.CreatedBy;
                    oSalesPersonCommissionSchedule.CreatedDate = _inputData.CreatedDate;
                    oSalesPersonCommissionSchedule.EffectiveDate = _inputData.EffectiveDate;
                    oSalesPersonCommissionSchedule.IsActive = _inputData.IsActive;
                    oSalesPersonCommissionSchedule.RegionId = _inputData.RegionId;
                    oSalesPersonCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oSalesPersonCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oSalesPersonCommissionSchedule.ContractTypeListId = _inputData.ContractTypeId;
                    _context.SaveChanges();
                }
                else
                {
                    oSalesPersonCommissionSchedule = new SalesPersonCommissionSchedule();
                    oSalesPersonCommissionSchedule.CommissionCompensationScheduleId = _inputData.CommissionCompensationScheduleId;
                    oSalesPersonCommissionSchedule.CreatedBy = _inputData.CreatedBy;
                    oSalesPersonCommissionSchedule.CreatedDate = _inputData.CreatedDate;
                    oSalesPersonCommissionSchedule.EffectiveDate = _inputData.EffectiveDate;
                    oSalesPersonCommissionSchedule.IsActive = _inputData.IsActive;
                    oSalesPersonCommissionSchedule.RegionId = _inputData.RegionId;
                    oSalesPersonCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oSalesPersonCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oSalesPersonCommissionSchedule.ContractTypeListId = _inputData.ContractTypeId;
                    _context.SalesPersonCommissionSchedules.Add(oSalesPersonCommissionSchedule);
                    _context.SaveChanges();

                }

            }
            return GetSalesPersonCommSchListData(_inputData.RegionId);
        }
        public List<SalesPersonBonusCommissionScheduleViewModel> InsertSalesPersonBonusCommSchData(SalesPersonBonusCommissionScheduleViewModel _inputData)
        {
            using (var _context = new jkDatabaseEntities())
            {
                SalesPersonBonusCommissionSchedule oSalesPersonBonusCommissionSchedule = _context.SalesPersonBonusCommissionSchedules.FirstOrDefault(o => o.SalesPersonBonusCommissionScheduleId == _inputData.SalesPersonBonusCommissionScheduleId);
                if (oSalesPersonBonusCommissionSchedule != null)
                {
                    oSalesPersonBonusCommissionSchedule.CommissionAdditionalBonusScheduleId = _inputData.CommissionAdditionalBonusScheduleId;
                    oSalesPersonBonusCommissionSchedule.CreatedBy = _inputData.CreatedBy;
                    oSalesPersonBonusCommissionSchedule.CreatedDate = _inputData.CreatedDate;
                    oSalesPersonBonusCommissionSchedule.EffectiveDate = _inputData.EffectiveDate;
                    oSalesPersonBonusCommissionSchedule.IsActive = _inputData.IsActive;
                    oSalesPersonBonusCommissionSchedule.RegionId = _inputData.RegionId;
                    oSalesPersonBonusCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oSalesPersonBonusCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oSalesPersonBonusCommissionSchedule.ContractTypeListId = _inputData.ContractTypeId;
                    oSalesPersonBonusCommissionSchedule.BonusAmountTypeId = _inputData.BonusAmountTypeId;
                    oSalesPersonBonusCommissionSchedule.BonusAmount = _inputData.BonusAmount;
                    _context.SaveChanges();
                }
                else
                {
                    oSalesPersonBonusCommissionSchedule = new SalesPersonBonusCommissionSchedule();
                    oSalesPersonBonusCommissionSchedule.CommissionAdditionalBonusScheduleId = _inputData.CommissionAdditionalBonusScheduleId;
                    oSalesPersonBonusCommissionSchedule.CreatedBy = _inputData.CreatedBy;
                    oSalesPersonBonusCommissionSchedule.CreatedDate = _inputData.CreatedDate;
                    oSalesPersonBonusCommissionSchedule.EffectiveDate = _inputData.EffectiveDate;
                    oSalesPersonBonusCommissionSchedule.IsActive = _inputData.IsActive;
                    oSalesPersonBonusCommissionSchedule.RegionId = _inputData.RegionId;
                    oSalesPersonBonusCommissionSchedule.SalesPersonId = _inputData.SalesPersonId;
                    oSalesPersonBonusCommissionSchedule.StatusListId = _inputData.StatusListId;
                    oSalesPersonBonusCommissionSchedule.ContractTypeListId = _inputData.ContractTypeId;
                    oSalesPersonBonusCommissionSchedule.BonusAmountTypeId = _inputData.BonusAmountTypeId;
                    oSalesPersonBonusCommissionSchedule.BonusAmount = _inputData.BonusAmount;
                    _context.SalesPersonBonusCommissionSchedules.Add(oSalesPersonBonusCommissionSchedule);
                    _context.SaveChanges();

                }
            }
            return GetSalesPersonCommSchBonusListData(_inputData.RegionId);
        }

        public List<CommissionPaymentScheduleViewModel> InsertCommissionPaymentScheduleData(CommissionPaymentScheduleViewModel _inputData)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CommissionPaymentScheduleId", _inputData.CommissionPaymentScheduleId);
                parmas.Add("@Description", _inputData.Description);
                parmas.Add("@PaymentScheduleTypeId", _inputData.PaymentScheduleTypeId);
                parmas.Add("@Amount", _inputData.Amount);
                parmas.Add("@Term1_StartMonth", _inputData.Term1_StartMonth);
                parmas.Add("@Term1_EndMonth", _inputData.Term1_EndMonth);
                parmas.Add("@Term1_Percent", _inputData.Term1_Percent);
                parmas.Add("@Term2_StartMonth", _inputData.Term2_StartMonth);
                parmas.Add("@Term2_EndMonth", _inputData.Term2_EndMonth);
                parmas.Add("@Term2_Percent", _inputData.Term2_Percent);
                parmas.Add("@Term3_StartMonth", _inputData.Term3_StartMonth);
                parmas.Add("@Term3_EndMonth", _inputData.Term3_EndMonth);
                parmas.Add("@Term3_Percent", _inputData.Term3_Percent);
                parmas.Add("@Term4_StartMonth", _inputData.Term4_StartMonth);
                parmas.Add("@Term4_EndMonth", _inputData.Term4_EndMonth);
                parmas.Add("@Term4_Percent", _inputData.Term4_Percent);
                parmas.Add("@StatusListId", _inputData.StatusListId);
                parmas.Add("@IsActive", _inputData.IsActive);
                parmas.Add("@UserId", _inputData.CreatedBy);
                parmas.Add("@RegionId", _inputData.RegionId);


                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_InsertUpdateCommissionPaymentSchedule", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CommissionPaymentScheduleViewModel>().ToList();
                    }
                }
            }
            return new List<CommissionPaymentScheduleViewModel>();
        }
        public List<BonusViewModel> InsertBonusData(BonusViewModel _inputData)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@BonusId", _inputData.BonusId);
                parmas.Add("@SaleAE_UserId", _inputData.SaleAE_UserId);
                parmas.Add("@PeriodId", _inputData.PeriodId);
                parmas.Add("@UserId ", _inputData.CreatedBy);
                parmas.Add("@RegionId ", _inputData.RegionId);
                parmas.Add("@BonusDescription ", _inputData.BonusDescription);
                parmas.Add("@BonusAmount ", _inputData.BonusAmount);
                parmas.Add("@BonusExplanation ", _inputData.BonusExplanation);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_InsertUpdateBonus", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<BonusViewModel>().ToList();
                    }
                }
            }
            return new List<BonusViewModel>();
        }

        public List<BonusViewModel> GetBonusListData(int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetBonusList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<BonusViewModel>().ToList();
                    }
                }
            }
            return new List<BonusViewModel>();
        }
        public List<BonusViewModel> DeleteBonusListData(int BonusId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@BonusId", BonusId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_DeleteBonusList", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<BonusViewModel>().ToList();
                    }
                }
            }
            return new List<BonusViewModel>();
        }
        public DashboardModel GetManagementDashboardData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            DashboardModel model = new DashboardModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_ManagementDashboardData @QueryType, @RegionId,@StartDate,@EndDate,NULL,NULL,@BillMonth,@BillYear";

                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                decimal TotalRevenue = conn.Query<decimal>(query, new
                {
                    QueryType = 1,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalRevenue = TotalRevenue > 0 ? TotalRevenue : 0;

                int TotalCustomer = conn.Query<int>(query, new
                {
                    QueryType = 2,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalCustomer = TotalCustomer > 0 ? TotalCustomer : 0;

                int TotalFranchisee = conn.Query<int>(query, new
                {
                    QueryType = 3,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalFranchisee = TotalFranchisee > 0 ? TotalFranchisee : 0;


                decimal TotalFranchiseeRevenue = conn.Query<decimal>(query, new
                {
                    QueryType = 4,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalFranchiseeRevenue = TotalFranchiseeRevenue > 0 ? TotalFranchiseeRevenue : 0;


                decimal TotalFranchiseeDeduction = conn.Query<decimal>(query, new
                {
                    QueryType = 5,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalFranchiseeDeduction = TotalFranchiseeDeduction > 0 ? TotalFranchiseeDeduction : 0;



                decimal TotalPayment = conn.Query<decimal>(query, new
                {
                    QueryType = 6,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalPayment = TotalPayment > 0 ? TotalPayment : 0;


                return model;
            }
        }

        public IEnumerable<DashboardModel> GetTopTenRevenueByAccountTypeChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportForManagement 5,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetCancelVsNewCustomerDetail(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_CustomerReportDetailsForManagement 1,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }

                return conn.Query<ChartDetailsViewModel>(query, new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    Flag = flag,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }


        public List<ObligationReportViewModel> getObligationList(string RegionIds)
        {
            List<ObligationReportViewModel> lstFranchiseeObligation = new List<ObligationReportViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@regionIds", RegionIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_ObligationList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeObligation = multipleresult.Read<ObligationReportViewModel>().ToList();
                    }
                }
            }
            return lstFranchiseeObligation;
        }

        public List<NegativeDueListReportViewModel> GetNegativeDueReportData(string regionIds = "", DateTime? from = null, DateTime? to = null)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", regionIds);
                parmas.Add("@FromDate", from);
                parmas.Add("@ToDate", to);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_NegativeDueReport", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<NegativeDueListReportViewModel>().ToList();
                    }
                }
            }
            return new List<NegativeDueListReportViewModel>();
        }

        public List<CurrentScheduledCommissionViewModel> BindCommissionsEarnedReportData(int month, int year, int userId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@Month", month);
                parmas.Add("@Year", year);
                parmas.Add("@UserId", userId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionsEarnedReport", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CurrentScheduledCommissionViewModel>().ToList();
                    }
                }
            }
            return new List<CurrentScheduledCommissionViewModel>();
        }

        public List<CurrentPaymentHistoryCommissionViewModel> BindPaymentHistoryReportData(int month, int year, int userId, int regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@Month", month);
                parmas.Add("@Year", year);
                parmas.Add("@UserId", userId);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_M_GetCommissionPaymentHistoryReport", param: parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CurrentPaymentHistoryCommissionViewModel>().ToList();
                    }
                }
            }
            return new List<CurrentPaymentHistoryCommissionViewModel>();
        }
    }
}