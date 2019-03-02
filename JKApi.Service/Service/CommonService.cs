using Dapper;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service;
using JKViewModels;
using JKViewModels.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service
{
    public interface ICommonService
    {
        string Customer_SuspendedTOActive();
        MailMessageTemplateModel GetEmailTemplate(string TemplateName);
        List<DropDownModel> DropDownListByName(string Name);

        List<Data.DAL.Region> GetRegionList();
        List<PeriodViewModel> GetPeriodList();
        List<DashboardQuickLinkModel> GetDashboardQuickLinks();

        List<PendingDashboardDataModel> GetDashboardPendingData(Int32? UID);
        List<PendingDashboardTasksDataModel> GetDashboardPendingTasksData(Int32? UID);
        void ChildViewMessageDashboard(int id);
        CRMDashboardModel GetCRMDashBoard();
        CommonDashboardViewModel GetDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetDashboardChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetDashboardMonthRevenueDataChartData();
        IEnumerable<DashboardModel> GetDashboardAccountTypeWiseChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetReceivableByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetTopRevenueWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetInvoiceDueByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetReceivableAndPayment();
        IEnumerable<DashboardModel> GetTopPaymentWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        bool CommonInsertNotification(int NotificationTypeListId, string MessageText, bool IsApproved, int ClassId, int TypeListId, int? MasterTrxTypeListId, int? HeaderId, int? MasterTrxId,int? UserId);
        IEnumerable<ChartDetailsViewModel> GetReceivableByAgeCategoryDetailsData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetDashboardRevenueChart(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);



        List<FileTypeModel> GetFileTypeList();
        FileTypeModel GetFileType(int fileType);

        List<DropDownModel> GetPeriodDropDownValues(string Name, int PeriodClosedId);

        CommonGMapAddressViewModel GetGMapAddresses(bool active, bool inactive, int regionId);
        bool UpdateGMapAddress(int AddressId, decimal _lat, decimal _long);
        CommonManagementDashboardViewModel GetAllManagementDashboardData( DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds=null, string flag=null);
        CommonManagementDashboardViewModel GetAllOfficeOverviewData(DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds, string flag);
        IEnumerable<DynamicReportViewModel> GetAllDynamicReportList();
        IEnumerable<DynamicReportColumnListViewModel> GetDynamicReportColumnList(int reportId);
        DynamicReportViewModel GetAllDynamicReportDetails(int reportId);
        DataTable GetDynamicReportData(string columnList, string procedureName, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear, int? rowNumber);
    }

    public class CommonService : BaseService, ICommonService
    {
        CacheProvider _cacheProvider = CacheProvider.Instance;
        public List<DropDownModel> DropDownListByName(string Name)
        {
            List<DropDownModel> objDropDownListItem = new List<DropDownModel>();

            //Check Cache if not exist then execute inside code
            //if (!_cacheProvider.Contains(CacheKeyName.DropDownValues + Name))
            //{
                SqlParameter[] parmList = {
                                      new SqlParameter("@DropDownType",Name),
                                     };

                using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_GetDropDownValues, parmList))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])     
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objDropDownListItem.Add(GetDataRowToEntity<DropDownModel>(dr));
                        }
                    }

                    //Set Cache value according to DDL Name
                    _cacheProvider.Set(CacheKeyName.DropDownValues + Name, objDropDownListItem);
                }

            //}
            return (List<DropDownModel>)_cacheProvider.Get(CacheKeyName.DropDownValues + Name);

        }

        public List<DropDownModel> GetPeriodDropDownValues(string Name, int PeriodClosedId)
        {
            List<DropDownModel> objDropDownListItem = new List<DropDownModel>();


            SqlParameter[] parmList = {
                                      new SqlParameter("@DropDownType",Name),
                                      new SqlParameter("@PeriodClosedId",PeriodClosedId),

                                     };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_GetPeriodDropDownValues, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])     
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        objDropDownListItem.Add(GetDataRowToEntity<DropDownModel>(dr));
                    }
                }

            }


            return objDropDownListItem;

        }

        public MailMessageTemplateModel GetEmailTemplate(string TemplateName)
        {
            MailMessageTemplateModel objItem = new MailMessageTemplateModel();

            SqlParameter[] parmList = {
                                      new SqlParameter("@TemplateName",TemplateName),
                                     };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_GetEMailTemplate, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    objItem = GetDataRowToEntity<MailMessageTemplateModel>(ds.Tables[0].Rows[0]);

                }
            }

            return objItem;

        }


        public List<Data.DAL.Region> GetRegionList()
        {
            List<Data.DAL.Region> objRegionList = new List<Data.DAL.Region>();
            var RegionList = ClaimView.GETCLAIM_REGIONLIST().OrderBy(r => r.Name);

            foreach (var regionInfoViewModel in RegionList)
            {
                objRegionList.Add(regionInfoViewModel.ToModel<Region, RegionInfoViewModel>());
            }

            return objRegionList;// _ClaimView.GETCLAIM_REGIONLIST().ToModel<List<Data.DAL.Region>,List<RegionInfoViewModel>>();
        }

        public List<PeriodViewModel> GetPeriodList()
        {
            using (var context = new jkDatabaseEntities())
            {
                DateTime date = DateTime.UtcNow;
                List<PeriodViewModel> data = context.Periods.Where(x => x.BillYear <= date.Year).OrderByDescending(x => x.BillYear).MapEnumerable<PeriodViewModel, Period>().ToList();
                return data;
            }
        }


        public List<DashboardQuickLinkModel> GetDashboardQuickLinks()
        {
            List<DashboardQuickLinkModel> model = new List<DashboardQuickLinkModel>();

            SqlParameter[] parmList = {
                                      new SqlParameter("@RegionId",SelectedRegionId),
                                      new SqlParameter("@PeriodId",Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID()))
        };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_DashboardQuickLinks, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])     
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.Add(GetDataRowToEntity<DashboardQuickLinkModel>(dr));
                    }
                }
            }

            return model;
        }

        public List<PendingDashboardDataModel> GetDashboardPendingData(Int32? UID)
        {
            List<PendingDashboardDataModel> model = new List<PendingDashboardDataModel>();

            SqlParameter[] parmList = {
                                      new SqlParameter("@UID",UID),
                                     };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_DashboardPendingData, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])     
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.Add(GetDataRowToEntity<PendingDashboardDataModel>(dr));
                    }
                }
            }

            return model;
        }
        public void ChildViewMessageDashboard(int id)
        {
            List<PendingDashboardDataModel> model = new List<PendingDashboardDataModel>();

            SqlParameter[] parmList = {
                                      new SqlParameter("@ID",id),
                                     };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_Sp_Update_ChildViewMessageDashboard, parmList))
            {

            }
        }


        public CRMDashboardModel GetCRMDashBoard()
        {
            using (var context = new jkDatabaseEntities())
            {
                List<CRMDashboardModel> model = context.CRM_common_CRMDashboard(SelectedRegionId, SelectedUserId).MapEnumerable<CRMDashboardModel, CRM_common_CRMDashboard_Result>().ToList();
                if (model.Count > 0)
                    return model[0];
                return null;
            }
        }

        public CommonDashboardViewModel GetDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            CommonDashboardViewModel model = new CommonDashboardViewModel();
            DashboardModel dashboardModel = new DashboardModel();
            List<DashboardModel> lstBillingBreakdownBySizeChartData = new List<DashboardModel>();
            List<DashboardModel> lstDashboardAccountTypeWiseChartData = new List<DashboardModel>();
            List<DashboardModel> lstRevenueWiseTopCustomerChartData = new List<DashboardModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }
                var parmas = new DynamicParameters();
                parmas.Add("@QueryType", 0);
                parmas.Add("@RegionId", regionId);
                parmas.Add("@StartDate", spnStartDate);
                parmas.Add("@EndDate", spnEndDate);
                parmas.Add("@BillMonth", billMonth);
                parmas.Add("@BillYear", billYear);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_CommonDashboardData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        dashboardModel = multipleresult.Read<DashboardModel>().ToList().FirstOrDefault();
                        lstBillingBreakdownBySizeChartData = multipleresult.Read<DashboardModel>().ToList();
                        lstRevenueWiseTopCustomerChartData = multipleresult.Read<DashboardModel>().ToList();
                        lstDashboardAccountTypeWiseChartData = multipleresult.Read<DashboardModel>().ToList();
                    }
                }
                model.DashboardModel = dashboardModel;
                model.BillingBreakdownBySizeChartData = lstBillingBreakdownBySizeChartData;
                model.RevenueWiseTopCustomerChartData = lstRevenueWiseTopCustomerChartData;
                model.DashboardAccountTypeWiseChartData = lstDashboardAccountTypeWiseChartData;

                return model;
            }



           
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var query = @"EXEC spGet_DashboardData @QueryType, @RegionId,@StartDate,@EndDate,@BillMonth,@BillYear";

            //    //Querytype 1=> Total Account Receivable
            //    //Querytype 2=> Total Payment for total account receivable
            //    //Querytype 3=> New Order 
            //    //Querytype 4=> Customer
            //    //Querytype 5=> Franchisee
            //    //Querytype 6=> Total Payment
            //    int? regionId = 0;
            //    if (SelectedRegionId == 0)
            //    {
            //        regionId = null;
            //    }
            //    else
            //    {
            //        regionId = SelectedRegionId;
            //    }

            //    decimal TotalAccountReceivable = conn.Query<decimal>(query, new
            //    {
            //        QueryType = 1,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalAccountReceivable = TotalAccountReceivable > 0 ? TotalAccountReceivable : 0;

            //    decimal TotalRevenue = conn.Query<decimal>(query, new
            //    {
            //        QueryType = 2,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalRevenue = TotalRevenue > 0 ? TotalRevenue : 0;

            //    int TotalInvoices = conn.Query<int>(query, new
            //    {
            //        QueryType = 3,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalInvoices = TotalInvoices > 0 ? TotalInvoices : 0;

            //    int TotalCustomer = conn.Query<int>(query, new
            //    {
            //        QueryType = 4,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalCustomer = TotalCustomer > 0 ? TotalCustomer : 0;

            //    int TotalFranchisee = conn.Query<int>(query, new
            //    {
            //        QueryType = 5,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalFranchisee = TotalFranchisee > 0 ? TotalFranchisee : 0;

            //    decimal TotalPayment = conn.Query<decimal>(query, new
            //    {
            //        QueryType = 6,
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    }).FirstOrDefault();

            //    model.TotalPayment = TotalPayment > 0 ? TotalPayment : 0;

            //    return model;
            //}

        }

        public IEnumerable<DashboardModel> GetDashboardChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_DashboardChartData 1,@RegionId,@StartDate,@EndDate,@BillMonth,@BillYear"; //1 for chart one data
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetDashboardMonthRevenueDataChartData()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_DashboardChartData 2,@RegionId,NULL,NULL,NULL,NULL"; //2 for chart two monthly revenue data
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId
                });
            }
        }

        public IEnumerable<DashboardModel> GetDashboardAccountTypeWiseChartData(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {

            List<DashboardModel> lstDashboardModel = new List<DashboardModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }
                var parmas = new DynamicParameters();
                parmas.Add("@QueryType", 3);
                parmas.Add("@RegionId", regionId);
                parmas.Add("@StartDate", spnStartDate);
                parmas.Add("@EndDate", spnEndDate);
                parmas.Add("@BillMonth", billMonth);
                parmas.Add("@BillYear", billYear);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_DashboardChartData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstDashboardModel = multipleresult.Read<DashboardModel>().ToList();
                    }
                }
                return lstDashboardModel;
            }


            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var query = @"EXEC spGet_DashboardChartData 3,@RegionId,@StartDate,@EndDate,@BillMonth,@BillYear"; //3 for chart three Account type wise chart data
            //    int? regionId = 0;
            //    if (SelectedRegionId == 0)
            //    {
            //        regionId = null;
            //    }
            //    else
            //    {
            //        regionId = SelectedRegionId;
            //    }

            //    return conn.Query<DashboardModel>(query, new
            //    {
            //        RegionId = regionId,
            //        StartDate = spnStartDate,
            //        EndDate = spnEndDate,
            //        BillMonth = billMonth,
            //        BillYear = billYear
            //    });
            //}
        }

        public IEnumerable<DashboardModel> GetReceivableByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_AR_DashboardChartData 1,@RegionId,@StartDate,@EndDate,NULL,@BillMonth,@BillYear"; //1 for Revenue Receivable By Age Category
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetTopRevenueWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_AR_DashboardChartData 2,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear"; //2 for Top Revenue Wise Customer
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetInvoiceDueByAgeCategory(DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_AR_DashboardChartData 3,@RegionId,@StartDate,@EndDate,NULL,@BillMonth,@BillYear"; //3 for Invoice Due By Age Category
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }

        public IEnumerable<DashboardModel> GetReceivableAndPayment()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_AR_DashboardChartData 4,@RegionId,NULL,NULL,NULL,NULL,NULL,@BillMonth,@BillYear"; //4 for Receivable And Payment Last 13 Months
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId
                });
            }
        }

        public IEnumerable<DashboardModel> GetTopPaymentWiseCustomer(int? recordNumber, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_AR_DashboardChartData 5,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                int? regionId = 0;
                if (SelectedRegionId == 0)
                {
                    regionId = null;
                }
                else
                {
                    regionId = SelectedRegionId;
                }

                return conn.Query<DashboardModel>(query, new
                {
                    RegionId = regionId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    RowNumber = recordNumber,
                    BillMonth = billMonth,
                    BillYear = billYear
                });
            }
        }
        public bool CommonInsertNotification(int NotificationTypeListId, string MessageText, bool IsApproved, int ClassId, int TypeListId, int? MasterTrxTypeListId, int? HeaderId, int? MasterTrxId, int? UserId)
        {
            bool retVal = false;

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SenderId", UserId);
                parmas.Add("@NotificationTypeListId", NotificationTypeListId);
                parmas.Add("@MessageText", MessageText);
                parmas.Add("@ClassId", ClassId);
                parmas.Add("@TypeListId", TypeListId);
                parmas.Add("@MasterTrxTypeListId", MasterTrxTypeListId);
                parmas.Add("@HeaderId", HeaderId);
                parmas.Add("@MasterTrxId", MasterTrxId);
                parmas.Add("@IsApproved", IsApproved);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_common_InsertNotification", parmas, commandType: CommandType.StoredProcedure))
                {
                    retVal = true;
                }
                return retVal;
            }
        }

        public IEnumerable<ChartDetailsViewModel> GetReceivableByAgeCategoryDetailsData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_OfficeOverviewReportDetailsData 4,@RegionId,@StartDate,@EndDate,@Flag,@BillMonth,@BillYear";
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

        public IEnumerable<DashboardModel> GetDashboardRevenueChart(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_RevenueReportDataForManagement 5,@RegionId,@StartDate,@EndDate,NULL,NULL,@BillMonth,@BillYear";
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

        public List<FileTypeModel> GetFileTypeList()
        {
            if (!_cacheProvider.Contains(CacheKeyName.FileTypeList))
            {
                var listModel = new List<FileTypeModel>();
                using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                   CommandType.StoredProcedure, DBConstants.common_GetFileTypeList))
                {
                    if (dataset == null || dataset.Tables.Count == 0) return listModel;
                    foreach (DataRow dataRow in dataset.Tables[0].Rows)
                    {
                        var fileType = GetDataRowToEntity<FileTypeModel>(dataRow);
                        if (fileType != null) listModel.Add(fileType);
                    }
                }
                _cacheProvider.Set(CacheKeyName.FileTypeList, listModel);
            }

            return (List<FileTypeModel>)_cacheProvider.Get(CacheKeyName.FileTypeList);
        }

        public FileTypeModel GetFileType(int fileType)
        {
            return GetFileTypeList().FirstOrDefault(x => x.FileType == fileType);
        }

        public List<PendingDashboardTasksDataModel> GetDashboardPendingTasksData(int? UID)
        {
            List<PendingDashboardTasksDataModel> model = new List<PendingDashboardTasksDataModel>();

            SqlParameter[] parmList = {
                                      new SqlParameter("@UID",UID),
                                     };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.common_DashboardPendingTasksData, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])     
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.Add(GetDataRowToEntity<PendingDashboardTasksDataModel>(dr));
                    }
                }
            }

            return model;
        }




        public CommonGMapAddressViewModel GetGMapAddresses(bool active, bool inactive, int regionId)
        {
            CommonGMapAddressViewModel oCommon = new CommonGMapAddressViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@Active", active);
                parmas.Add("@Inactive", inactive);
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_Common_GetAddressForLocationByRegion", parmas, commandType: CommandType.StoredProcedure))
                {
                    oCommon.lstAllData = multipleresult.Read<GMapAddressViewModel>().ToList();
                    oCommon.lstDistinctAddress = multipleresult.Read<GMapAddressViewModel>().ToList();
                }
                return oCommon;
            }
        }

        public bool UpdateGMapAddress(int AddressId, decimal _lat, decimal _long)
        {
            var retVal = true;
            try
            {
                using (var context = new jkDatabaseEntities())
                {
                    var oAddress = context.Addresses.SingleOrDefault(a => a.AddressId == AddressId);
                    if (oAddress != null)
                    {
                        oAddress.Latitude = _lat;// decimal.Parse(destination_latLong.results[0].geometry.location.lat.ToString());
                        oAddress.Longitude = _long;// decimal.Parse(destination_latLong.results[0].geometry.location.lng.ToString());
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                retVal = false;
            }

            return retVal;
        }


        #region Scheduler API
        public string Customer_SuspendedTOActive()
        {
            string returnId = string.Empty;

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, "portal_sp_Customer_SuspendedTOActive"))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }

            return returnId;
        }

        public CommonManagementDashboardViewModel GetAllManagementDashboardData(DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds, string flag)
        {
            CommonManagementDashboardViewModel model = new CommonManagementDashboardViewModel();
            DashboardModel dashboardModel = new DashboardModel();
            List<DashboardModel> lstBillingAccountBreakdownBySize = new List<DashboardModel>();
            List<DashboardModel> lstTopRevenuedAccountType = new List<DashboardModel>();
            List<DashboardModel> lstTopRevenuedCustomers = new List<DashboardModel>();
            List<DashboardModel> lstMonthlyRevenues = new List<DashboardModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {   
                if (string.IsNullOrEmpty(regionIds) || regionIds.ToLower()=="null") {
                    if (SelectedRegionId == 0)
                    {
                        regionIds = "0";
                    }
                    else
                    {
                        regionIds = SelectedRegionId.ToString();
                    }
                } 

                var parmas = new DynamicParameters();
                
                parmas.Add("@RegionId", regionIds);
                parmas.Add("@StartDate", spnStartDate);
                parmas.Add("@EndDate", spnEndDate);
                parmas.Add("@MonthToAdd", monthToAdd);
                parmas.Add("@YearToAdd", yearToAdd);
                parmas.Add("@BillMonth", billMonth);
                parmas.Add("@BillYear", billYear);
                parmas.Add("@RowNumber", rowNumber);
                parmas.Add("@Flag", flag);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_ManagementCommonDashboardData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        dashboardModel = multipleresult.Read<DashboardModel>().ToList().FirstOrDefault();
                        lstBillingAccountBreakdownBySize = multipleresult.Read<DashboardModel>().ToList();
                        lstTopRevenuedAccountType = multipleresult.Read<DashboardModel>().ToList();
                        lstTopRevenuedCustomers = multipleresult.Read<DashboardModel>().ToList();
                        lstMonthlyRevenues = multipleresult.Read<DashboardModel>().ToList();
                    }
                }
                model.DashboardModel = dashboardModel;
                model.BillingAccountBreakdownBySize = lstBillingAccountBreakdownBySize;
                model.TopRevenuedAccountType = lstTopRevenuedAccountType;
                model.TopRevenuedCustomers = lstTopRevenuedCustomers;
                model.MonthlyRevenues = lstMonthlyRevenues;

                return model;
            }
        }

        public CommonManagementDashboardViewModel GetAllOfficeOverviewData(DateTime? spnStartDate, DateTime? spnEndDate, int? monthToAdd, int? yearToAdd, int? billMonth, int? billYear, int? rowNumber, string regionIds, string flag)
        {
            CommonManagementDashboardViewModel model = new CommonManagementDashboardViewModel();
          
            List<DashboardModel> lstBillingAccountBreakdownBySize = new List<DashboardModel>();
            List<DashboardModel> lstBillingAccountBreakdownByContract = new List<DashboardModel>();
            List<DashboardModel> lstTopRevenuedAccountType = new List<DashboardModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                if (string.IsNullOrEmpty(regionIds) || regionIds.ToLower() == "null")
                {
                    if (SelectedRegionId == 0)
                    {
                        regionIds = "0";
                    }
                    else
                    {
                        regionIds = SelectedRegionId.ToString();
                    }
                }

                var parmas = new DynamicParameters();

                parmas.Add("@RegionId", regionIds);
                parmas.Add("@StartDate", spnStartDate);
                parmas.Add("@EndDate", spnEndDate);
                parmas.Add("@MonthToAdd", monthToAdd);
                parmas.Add("@YearToAdd", yearToAdd);
                parmas.Add("@BillMonth", billMonth);
                parmas.Add("@BillYear", billYear);
                parmas.Add("@RowNumber", rowNumber);
                parmas.Add("@Flag", flag);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_OfficeOverviewCommonChartData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstBillingAccountBreakdownBySize = multipleresult.Read<DashboardModel>().ToList();
                        lstBillingAccountBreakdownByContract = multipleresult.Read<DashboardModel>().ToList();
                        lstTopRevenuedAccountType = multipleresult.Read<DashboardModel>().ToList();
                    }
                }
                model.BillingAccountBreakdownBySize = lstBillingAccountBreakdownBySize;
                model.BillingAccountBreakdownByContract = lstBillingAccountBreakdownByContract;
                model.TopRevenuedAccountType = lstTopRevenuedAccountType;

                return model;
            }
        }

        public IEnumerable<DynamicReportViewModel> GetAllDynamicReportList()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC common_GetDynamicReport 1,null";
               
                return conn.Query<DynamicReportViewModel>(query);
            }
        }
        public IEnumerable<DynamicReportColumnListViewModel> GetDynamicReportColumnList(int reportId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC common_GetDynamicReport 2,@ReportId";

                return conn.Query<DynamicReportColumnListViewModel>(query,new {
                    ReportId=reportId
                });
            }
        }

        public DynamicReportViewModel GetAllDynamicReportDetails(int reportId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC common_GetDynamicReport 3,@ReportId";

                return conn.Query<DynamicReportViewModel>(query, new
                {
                    ReportId = reportId
                }).FirstOrDefault();
            }
        }

        public DataTable GetDynamicReportData(string columnList, string procedureName, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear,int? rowNumber)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(procedureName, conn);

                cmd.Parameters.AddWithValue("@ColumnList", columnList);
                cmd.Parameters.AddWithValue("@RegionId", regionIds);
                cmd.Parameters.AddWithValue("@StartDate", spnStartDate);
                cmd.Parameters.AddWithValue("@EndDate", spnEndDate);
                cmd.Parameters.AddWithValue("@RowNumber", rowNumber);
                cmd.Parameters.AddWithValue("@BillMonth", billMonth);
                cmd.Parameters.AddWithValue("@BillYear", billYear);

                cmd.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }

            
        }
        #endregion
    }


}
