using Dapper;
using JKApi.Data.DAL;
using JKApi.Service.AccountPayable;
using JKApi.Service.Helper;
using JKApi.Service.Helper.Extension;
using JKApi.Service.Service.Administration.Company;
using JKApi.Service.ServiceContract.Company;
using JKViewModels.Administration.Company;
using JKViewModels.Administration.System;
using JKViewModels.Company;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;



namespace JKApi.Service.Service.Company
{
    public class CompanyService : BaseService, ICompanyService
    {
        CompanyViewModel compViewModel = new CompanyViewModel();

        public string CheckNumber { get; private set; }

        //RemitToViewModel remitViewModel = new RemitToViewModel();

        public CompanyService(JK.Repository.Uow.IJKEfUow uow)
        {
            Uow = uow;
        }

        public CompanyService()
        { }

        public IQueryable<Data.DAL.Company> GetAll_Company3rdParty()
        {
            return Uow.Company.GetAll();
        }
        public Data.DAL.Company Get_Company3rdParty(int id)
        {
            return Uow.Company.GetById(id);
        }
        public Data.DAL.Company Save_Company3rdParty(Data.DAL.Company company3rdparty)
        {
            var ID = company3rdparty.CompanyId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                company3rdparty.CreatedDate = DateTime.Now;
                Uow.Company.Add(company3rdparty);
            }

            else //Update existing entry
            {
                Uow.Company.Update(company3rdparty);
            }
            Uow.Commit();
            return company3rdparty;
        }

        public List<Company3rdPartyListViewModel> GetAll_Company3rdParty(string regionId = "", int companyid = -1)
        {
            var company3rdpartyvm = new List<Company3rdPartyListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                if (companyid > 0)
                    parmas.Add("@companyid", companyid);
                parmas.Add("@regionid", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_Company3rdPartyList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        company3rdpartyvm = multipleresult.Read<Company3rdPartyListViewModel>().ToList();
                    }
                }

            }
            return company3rdpartyvm;

        }

        public List<CompanyViewModel> GetAddress(int CompanyId)
        {
            string sqlStr = "SELECT C.AddressId, CompanyId,'' AS CompanyNo,CompanyName AS [Name],CompanyName AS Title, RegionId, A.Address1 as [Address], A.City, A.StateName as [State], A.PostalCode as Zip " +
            " FROM Company C WITH(NOLOCK) " +
            "left join[Address] A  WITH(NOLOCK)  on A.AddressId = C.AddressId " +
            "WHERE CompanyId = " + CompanyId;

            List<CompanyViewModel> rvm = new List<CompanyViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var companyData = conn.Query(sqlStr).ToList();
                rvm = SqlMapper.Query<CompanyViewModel>(
                                conn, sqlStr).ToList();
            }

            return rvm;
        }

        public List<LadgerAccountViewModel> GetChartofAccounts(string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            List<LadgerAccountViewModel> lstLadgerAccountViewModel = new List<LadgerAccountViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", (selectedRegion == "null" ? "0" : selectedRegion));
                parmas.Add("@StartDate", StartDate);
                parmas.Add("@EndDate", EndDate);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_GetChartofAccounts", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstLadgerAccountViewModel = multipleresult.Read<LadgerAccountViewModel>().ToList();
                        List<LadgerSubAccountViewModel> lstLadgerSubAccountViewModel = multipleresult.Read<LadgerSubAccountViewModel>().ToList();

                        foreach (var item in lstLadgerAccountViewModel)
                        {
                            item.LadgerSubAccounts = lstLadgerSubAccountViewModel.Where(o => o.AccountId == item.AccountId).ToList();
                        }
                    }
                }
            }
            return lstLadgerAccountViewModel.OrderBy(l => l.AccountNumber).ToList();
        }
        public static List<ConfigSettingViewModel> GetSettings()
        {

            string spName = "spGet_Co_Settings";
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
                        configSetting.Id = Convert.ToInt32(reader["RegionConfigurationId"]);
                        configSetting.Name = reader["name"].ToString().Trim();
                        configSetting.Value = reader["value"].ToString().Trim();
                        configSetting.ValueType = Convert.ToInt32(reader["valuetype"]);
                        configSetting.StatusId = Convert.ToInt32(reader["status"]);
                        configSetting.Applied = Convert.ToInt32(reader["applied"]);
                        configSetting.Grouptype = Convert.ToInt32(reader["rcgroup"]);
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

        public static List<BPPAdminFeeSettingViewModel> GetBPPAdminFeeSettings()
        {
            string spName = "spget_Co_BPPAdminFeeSetting";
            List<BPPAdminFeeSettingViewModel> feeSettingList = new List<BPPAdminFeeSettingViewModel>();

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
                        BPPAdminFeeSettingViewModel feeSetting = new BPPAdminFeeSettingViewModel();
                        feeSetting.Id = Convert.ToInt32(reader["Id"]);
                        feeSetting.StartAmount = Convert.ToDecimal(reader["startamount"]);
                        feeSetting.EndAmount = Convert.ToDecimal(reader["endamount"]);
                        feeSetting.Amount = Convert.ToInt32(reader["amount"]);

                        feeSettingList.Add(feeSetting);
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

            return feeSettingList;

        }

        public static List<JKApi.Data.DAL.TransactionNumberConfig> GetTransactionIdentifierList()

        {
            using (Data.DAL.jkDatabaseEntities context = new jkDatabaseEntities())
            {


                var lsTrxIdentifier = context.TransactionNumberConfigs.ToList();
                return lsTrxIdentifier;
            }
        }

        public JKViewModels.Administration.Company.TransactionNumberConfigViewModel GetTransactionNumberConfig(int TransactionType, int RegionId)
        {
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel TransactionNumberConfigObj = null;

            using (Data.DAL.jkDatabaseEntities context = new jkDatabaseEntities())
            {

                var TrxNumConfig = context.portal_spGet_Region_TransactionNumberConfig(TransactionType, RegionId);
                portal_spGet_Region_TransactionNumberConfig_Result res = context.portal_spGet_Region_TransactionNumberConfig(TransactionType, RegionId).FirstOrDefault();

                if (res != null)
                {
                    TransactionNumberConfigObj = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
                    TransactionNumberConfigObj.TransactionNumberConfigId = res.TransactionNumberConfigId;
                    TransactionNumberConfigObj.RegionId = res.RegionId;
                    TransactionNumberConfigObj.RegionNumber = res.RegionNumber;
                    TransactionNumberConfigObj.MasterTrxTypeListId = res.MasterTrxTypeListId;
                    TransactionNumberConfigObj.Prefix = res.Prefix;
                    TransactionNumberConfigObj.LastNumber = res.LastNumber;
                }
            }


            return TransactionNumberConfigObj;
        }

        public string GetNextTransactionNumberConfig(int TransactionType, int RegionId, DateTime TrxDate)
        {
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel TransactionNumberConfigObj = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (Data.DAL.jkDatabaseEntities context = new jkDatabaseEntities())
            {

                //var TrxNumConfig = context.portal_spGet_Region_TransactionNumberConfig(TransactionType, RegionId);
                List<portal_spGet_Region_TransactionNumberConfig_Result> lst = context.portal_spGet_Region_TransactionNumberConfig(TransactionType, RegionId).ToList();
                var result = lst.FirstOrDefault();
                TransactionNumberConfigObj.RegionNumber = result?.RegionNumber;
                TransactionNumberConfigObj.Prefix = result?.Prefix;
                TransactionNumberConfigObj.LastNumber = result?.LastNumber;

            }
            string TransactionNumber = TransactionNumberConfigObj.Prefix.Trim() + TrxDate.Month.ToString() + TrxDate.ToString("yy") + TransactionNumberConfigObj.RegionNumber.ToString().PadLeft(3, '0') + (TransactionNumberConfigObj.LastNumber + 1).ToString().PadLeft(3, '0');
            return TransactionNumber;
        }





        public static ConfigSettingViewModel GetSetting(int SettingType)
        {
            List<ConfigSettingViewModel> os = GetSettings();
            if (os.Find(m => m.Id == SettingType) != null)
            {
                return os.Single(m => m.Id == SettingType);
            }

            return null;

        }


        public Data.DAL.TransactionNumberConfig SaveTransactionNumberConfig(Data.DAL.TransactionNumberConfig TrxNumConfig)
        {

            using (var context = new jkDatabaseEntities())
            {

                JKApi.Data.DAL.TransactionNumberConfig trxNumConfig = context.TransactionNumberConfigs.SingleOrDefault(o => o.TransactionNumberConfigId == TrxNumConfig.TransactionNumberConfigId);
                trxNumConfig.TransactionNumberConfigId = TrxNumConfig.TransactionNumberConfigId;
                trxNumConfig.LastNumber = TrxNumConfig.LastNumber;
                context.SaveChanges();
            }

            return TrxNumConfig;

        }



        public int Save(int Id, string Name, string Acronym, string Corporate, string StatusId, string Test, string DisplayName, string ReportName, string Address, string City, string State, string Zip, string Phone, string RemitSameAsMain, string userid)
        {
            string spName = "spSave_Region";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@name", UtilityService.StaticScrubTextForDb(Name));
                cmd.Parameters.AddWithValue("@acronym", UtilityService.StaticScrubTextForDb(Acronym));
                cmd.Parameters.AddWithValue("@corporate", Corporate);
                cmd.Parameters.AddWithValue("@status", StatusId);
                cmd.Parameters.AddWithValue("@test", Test);
                cmd.Parameters.AddWithValue("@displayname", UtilityService.StaticScrubTextForDb(DisplayName));
                cmd.Parameters.AddWithValue("@reportname", UtilityService.StaticScrubTextForDb(ReportName));
                cmd.Parameters.AddWithValue("@address", UtilityService.StaticScrubTextForDb(Address));
                cmd.Parameters.AddWithValue("@city", UtilityService.StaticScrubTextForDb(City));
                cmd.Parameters.AddWithValue("@state", UtilityService.StaticScrubTextForDb(State));
                cmd.Parameters.AddWithValue("@postalcode", UtilityService.StaticScrubTextForDb(Zip));
                cmd.Parameters.AddWithValue("@phone", UtilityService.StaticScrubTextForDb(Phone));
                cmd.Parameters.AddWithValue("@remitsameasmain", RemitSameAsMain);
                cmd.Parameters.AddWithValue("@userid", userid);

                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;

                return Convert.ToInt32(result);
            }
        }

        public int SaveConfigurationSetting(int id, string value, int userid)
        {
            string spName = "spsave_Co_ConfigurationSetting";

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

            string spName = "spsave_Co_FeeSetting";

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
            string spName = "spsave_Co_BBPAFeeSetting";

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


        public int Clear(int RegionId)
        {
            string spName = "spDelete_RegionRemitTo";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", RegionId);
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                var result = returnParameter.Value;
                return Convert.ToInt32(result);

            }


        }


        public CompanyViewModel Load(int id)
        {
            if (id == -1) { return null; }

            string spName = "spGet_Region";

            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {

                        compViewModel.Id = Convert.ToInt32(reader["Regionid"].ToString());
                        compViewModel.Name = reader["name"].ToString().Trim();
                        compViewModel.Acronym = reader["acronym"].ToString().Trim();
                        compViewModel.Corporate = Convert.ToInt32(reader["corporate"].ToString());
                        compViewModel.Test = Convert.ToInt32(reader["test"].ToString());
                        compViewModel.DisplayName = reader["displayname"].ToString().Trim();
                        compViewModel.ReportName = reader["reportname"].ToString().Trim();
                        compViewModel.Address = reader["address"].ToString().Trim();
                        compViewModel.City = reader["city"].ToString().Trim();
                        compViewModel.State = reader["state"].ToString().Trim();
                        compViewModel.Zip = reader["postalcode"].ToString().Trim();
                        compViewModel.Phone = reader["phone"].ToString().Trim();
                        compViewModel.RemitSameAsMain = Convert.ToInt32(reader["remitsameasmain"].ToString());
                        compViewModel.StatusId = Convert.ToInt32(reader["status"].ToString());
                        compViewModel.CreatedBy = Convert.ToInt32(reader["createdby"].ToString());
                        compViewModel.CreateDate = (DateTime)reader["createdate"];
                        compViewModel.ModifiedBy = Convert.ToInt32(reader["modifiedby"].ToString());
                        compViewModel.ModifiedDate = (DateTime)reader["modifieddate"];

                        if (compViewModel.RemitSameAsMain == Constants.Yes)
                        {
                            compViewModel.RemitToLocation.Address = compViewModel.Address;
                            compViewModel.RemitToLocation.City = compViewModel.City;
                            compViewModel.RemitToLocation.State = compViewModel.State;
                            compViewModel.RemitToLocation.Zip = compViewModel.Zip;
                        }
                        else
                        {
                            compViewModel.RemitToLocation = RemitService.Load(compViewModel.Id);
                        }



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

                return compViewModel;
            }
        }


        public List<LadgerAccountDetailViewResult> GetLedgerMasterTrasactionList(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            List<LadgerAccountDetailViewResult> lstLadgerAccountViewModel = new List<LadgerAccountDetailViewResult>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LedgerAccountId", LedgerId);
                parmas.Add("@RegionIds", (selectedRegion == "null" ? "0" : selectedRegion));

                if (month > 0 && year > 0)
                {
                    parmas.Add("@StartDate", month + "/01/" + year);
                    parmas.Add("@EndDate", month + "/" + DateTime.DaysInMonth(year, month) + "/" + year);
                }
                else
                {
                    parmas.Add("@StartDate", StartDate);
                    parmas.Add("@EndDate", EndDate);
                }

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CO_GetLedgerAccountTransaction", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstLadgerAccountViewModel = multipleresult.Read<LadgerAccountDetailViewResult>().ToList();
                    }
                }
            }
            return lstLadgerAccountViewModel;


            ////selectedRegion, StartDate, EndDate, month, year
            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    if (isSubLedgerAccount)
            //        return context.vw_MasterTrasactionList.Where(o => o.LedgerSubAcctId == LedgerId).ToList();
            //    else
            //        return context.vw_MasterTrasactionList.Where(o => o.LedgerAcctId == LedgerId).ToList();

            //}
        }

        public List<MasterTrasactionListAllData> GetLedgerMasterTrasactionListAllData(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            List<MasterTrasactionListAllData> lstLadgerAccountViewModel = new List<MasterTrasactionListAllData>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LedgerAccountId", LedgerId);
                parmas.Add("@RegionIds", (selectedRegion == "null" ? "0" : selectedRegion));

                if (month > 0 && year > 0)
                {
                    parmas.Add("@StartDate", month + "/01/" + year);
                    parmas.Add("@EndDate", month + "/" + DateTime.DaysInMonth(year, month) + "/" + year);
                }
                else
                {
                    parmas.Add("@StartDate", StartDate);
                    parmas.Add("@EndDate", EndDate);
                }

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CO_GetLedgerAccountTransactionAllData", parmas, commandType: CommandType.StoredProcedure, commandTimeout: 180))
                {
                    if (multipleresult != null)
                    {
                        lstLadgerAccountViewModel = multipleresult.Read<MasterTrasactionListAllData>().ToList();
                    }
                }
            }
            return lstLadgerAccountViewModel;

        }

        public List<MasterTrasactionListAllData> GetLedgerMasterTrasactionListAllDataExcel(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0)
        {
            List<MasterTrasactionListAllData> lstLadgerAccountViewModel = new List<MasterTrasactionListAllData>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LedgerAccountId", LedgerId);
                parmas.Add("@RegionIds", (selectedRegion == "null" ? "0" : selectedRegion));

                if (month > 0 && year > 0)
                {
                    parmas.Add("@StartDate", month + "/01/" + year);
                    parmas.Add("@EndDate", month + "/" + DateTime.DaysInMonth(year, month) + "/" + year);
                }
                else
                {
                    parmas.Add("@StartDate", StartDate);
                    parmas.Add("@EndDate", EndDate);
                }

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CO_GetLedgerAccountTransactionAllDataExcel", parmas, commandType: CommandType.StoredProcedure, commandTimeout: 180))
                {
                    if (multipleresult != null)
                    {
                        lstLadgerAccountViewModel = multipleresult.Read<MasterTrasactionListAllData>().ToList();
                    }
                }
            }
            return lstLadgerAccountViewModel;

        }


        public List<portal_spGet_C_DepositList_Result> GetDepositList(DateTime from, DateTime to)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.portal_spGet_C_DepositList(this.SelectedRegionId, from, to).ToList();
                return results;
            }
        }

        public List<DepositType> GetDepositTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = context.DepositTypes.ToList();
                return qry;
            }
        }

        public CheckLayoutViewModel ConvertCheckLayoutToViewModel(CheckLayout obj)
        {
            if (obj == null)
                return null;

            CheckLayoutViewModel clvm = new CheckLayoutViewModel();
            clvm.Id = obj.CheckLayoutId;
            clvm.Name = obj.Name;
            clvm.Position = obj.Position;
            clvm.X = obj.X;
            clvm.Y = obj.Y;
            clvm.CreatedBy = obj.CreatedBy;
            clvm.CreateDate = obj.CreatedDate;
            clvm.ModifiedBy = obj.ModifiedBy;
            clvm.ModifiedDate = obj.ModifiedDate;

            List<CheckLayoutElementViewModel> clevms = new List<CheckLayoutElementViewModel>();

            foreach (var elem in obj.CheckLayoutElements)
            {
                CheckLayoutElementViewModel clevm = new CheckLayoutElementViewModel();
                clevm.Id = elem.CheckLayoutElementId;
                clevm.ElementType = elem.CheckLayoutElementTypeList.Name;
                clevm.ElementTypeId = elem.CheckLayoutElementTypeListId;
                clevm.X = elem.X;
                clevm.Y = elem.Y;
                clevm.IsActive = elem.IsActive;
                clevm.CreatedBy = elem.CreatedBy;
                clevm.CreateDate = elem.CreatedDate;
                clevm.ModifiedBy = elem.ModifiedBy;
                clevm.ModifiedDate = elem.ModifiedDate;
                clevms.Add(clevm);
            }

            clvm.Elements = clevms;

            return clvm;
        }

        public CheckCalibrationViewModel ConvertCheckCalibrationToViewModel(CheckCalibration obj)
        {
            if (obj == null)
                return null;

            CheckCalibrationViewModel vm = new CheckCalibrationViewModel();
            vm.Id = obj.CheckCalibrationId;
            vm.ShiftX = obj.ShiftX;
            vm.ShiftY = obj.ShiftY;
            vm.CreatedBy = obj.CreatedBy;
            vm.CreateDate = obj.CreatedDate;
            vm.ModifiedBy = obj.ModifiedBy;
            vm.ModifiedDate = obj.ModifiedDate;

            return vm;
        }

        public CheckLayoutViewModel GetCheckLayout(int id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkLayout = context.CheckLayouts.Where(o => o.CheckLayoutId == id).FirstOrDefault();
                return ConvertCheckLayoutToViewModel(checkLayout);
            }
        }

        public CheckLayoutViewModel GetDefaultCheckLayoutForRegion()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkLayoutRegion = context.CheckLayoutRegions.Where(o => o.RegionId == this.SelectedRegionId && o.IsDefault == true).FirstOrDefault();
                if (checkLayoutRegion == null)
                    return null;
                CheckLayoutViewModel vm = ConvertCheckLayoutToViewModel(checkLayoutRegion.CheckLayout);
                vm.IsDefault = true;
                return vm;
            }
        }

        public List<CheckLayoutViewModel> GetCheckLayoutsForRegion()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CheckLayoutViewModel> clvms = new List<CheckLayoutViewModel>();

                var assocs = context.CheckLayoutRegions.Where(o => o.RegionId == SelectedRegionId || SelectedRegionId == 0).OrderByDescending(o => o.IsDefault).ThenByDescending(o => o.CheckLayoutId);

                foreach (var assoc in assocs)
                {
                    CheckLayoutViewModel clvm = ConvertCheckLayoutToViewModel(assoc.CheckLayout);
                    clvm.IsDefault = assoc.IsDefault;
                    clvms.Add(clvm);
                }

                return clvms;
            }
        }

        public bool InsertOrUpdateCheckLayout(CheckLayoutViewModel vm)
        {
            bool result = true;

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CheckLayout layout = null;
                if (vm.Id != -1)
                    layout = context.CheckLayouts.Where(o => o.CheckLayoutId == vm.Id).FirstOrDefault();

                if (layout != null)
                {
                    layout.ModifiedBy = vm.ModifiedBy;
                    layout.ModifiedDate = vm.ModifiedDate;
                    context.Entry(layout).State = EntityState.Modified;
                }
                else
                {
                    layout = new CheckLayout();
                    layout.CheckLayoutId = -1;
                    layout.CreatedBy = vm.CreatedBy;
                    layout.CreatedDate = vm.CreateDate;
                }

                layout.Name = vm.Name;
                layout.Position = vm.Position;
                layout.X = vm.X;
                layout.Y = vm.Y;

                foreach (var elemObj in vm.Elements)
                {
                    CheckLayoutElement element = null;
                    if (elemObj.Id != -1)
                        element = layout.CheckLayoutElements.Where(o => o.CheckLayoutElementId == elemObj.Id).FirstOrDefault();

                    if (element != null)
                    {
                        element.ModifiedBy = vm.ModifiedBy;
                        element.ModifiedDate = vm.ModifiedDate;
                        context.Entry(element).State = EntityState.Modified;
                    }
                    else
                    {
                        element = new CheckLayoutElement();
                        element.CheckLayoutElementId = -1;
                        element.CheckLayoutElementTypeListId = elemObj.ElementTypeId;

                        element.CreatedBy = vm.CreatedBy;
                        element.CreatedDate = vm.CreateDate;
                    }

                    element.X = elemObj.X;
                    element.Y = elemObj.Y;
                    element.ImageFilename = elemObj.ImageFilename;
                    element.IsActive = elemObj.IsActive;

                    if (element.CheckLayoutElementId == -1)
                    {
                        layout.CheckLayoutElements.Add(element);
                    }
                }

                if (layout.CheckLayoutId == -1)
                    context.CheckLayouts.Add(layout);

                result = result && (context.SaveChanges() == 0);

                CheckLayoutRegion assoc = context.CheckLayoutRegions.Where(o => o.CheckLayoutId == layout.CheckLayoutId && (o.RegionId == SelectedRegionId || SelectedRegionId == 0)).FirstOrDefault();
                if (assoc == null)
                {
                    assoc = new CheckLayoutRegion();
                    assoc.CheckLayoutRegionId = -1;
                    assoc.CheckLayoutId = layout.CheckLayoutId;
                    assoc.RegionId = this.SelectedRegionId;
                    assoc.CreatedBy = vm.CreatedBy;
                    assoc.CreatedDate = vm.CreateDate;

                    context.CheckLayoutRegions.Add(assoc);
                    result = result && (context.SaveChanges() == 0);
                }

                bool hasDefaultLayout = context.CheckLayoutRegions.Any(o => (o.RegionId == SelectedRegionId || SelectedRegionId == 0) && o.IsDefault == true);

                if (!hasDefaultLayout)
                    context.portal_spUpdate_C_DefaultCheckLayoutForRegion(assoc.RegionId, assoc.CheckLayoutId);

            }

            return result;
        }

        public bool InsertOrUpdateCheckCalibration(CheckCalibrationViewModel vm)
        {
            bool result = true;

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CheckCalibration obj = null;
                if (vm.Id != -1)
                    obj = context.CheckCalibrations.Where(o => o.CheckCalibrationId == vm.Id).FirstOrDefault();

                if (obj != null)
                {
                    obj.ModifiedBy = vm.ModifiedBy;
                    obj.ModifiedDate = vm.ModifiedDate;
                    context.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    obj = new CheckCalibration();
                    obj.CheckCalibrationId = -1;
                    obj.CreatedBy = vm.CreatedBy;
                    obj.CreatedDate = vm.CreateDate;
                }

                obj.RegionId = this.SelectedRegionId;
                obj.ShiftX = vm.ShiftX;
                obj.ShiftY = vm.ShiftY;

                if (obj.CheckCalibrationId == -1)
                    context.CheckCalibrations.Add(obj);

                result = result && (context.SaveChanges() == 0);
            }

            return result;
        }

        public List<CheckLayoutElementTypeList> GetCheckLayoutElementTypeList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.CheckLayoutElementTypeLists.OrderBy(o => o.DisplayOrder).ToList();
            }
        }

        public CheckCalibrationViewModel GetCheckCalibration(int id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkCalibration = context.CheckCalibrations.Where(o => o.CheckCalibrationId == id).FirstOrDefault();
                return ConvertCheckCalibrationToViewModel(checkCalibration);
            }
        }

        public CheckCalibrationViewModel GetCheckCalibrationForRegion()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkCalibration = context.CheckCalibrations.Where(o => o.RegionId == this.SelectedRegionId).FirstOrDefault();
                return ConvertCheckCalibrationToViewModel(checkCalibration);
            }
        }

        public CheckViewModel GetCheckDetailsSample()
        {
            CheckViewModel vm = new CheckViewModel();

            vm.RegionName = "JANI-KING OF COLORADO, INC.";
            vm.RegionAccountType = "SPECIAL TRUST ACCOUNT";
            vm.RegionAddress1 = "9000 E. Chenango Ave, Suite 102";
            vm.RegionAddress2 = "Greenwood Village, CO 80111";

            vm.BankName = "Wells Fargo Bank, N.A.";
            vm.BankRegion = "Denver";
            vm.BankAddress1 = "PO Box 5247";
            vm.BankAddress2 = "Denver, CO 80274";
            vm.BankPhone = "(303) 853-5308";
            vm.BankNumber = "23-7/1020";

            vm.PayeeNumber = "191376";
            vm.PayeeName = "OSCAR SOTO HOLGUIN, an Authorized Franchisee";
            vm.PayeeAddress1 = "8931 COLORADO BLVD. BLDG. #8, APT. #104";
            vm.PayeeAddress2 = "THORNTON, CO 80229";

            vm.CheckNumber = "90033716";
            vm.Date = "05/05/2014";
            vm.PayText = "One Hundred Ninety Six and 67/100 DOLLARS";
            vm.PayDollars = "$196.67";

            vm.RoutingNumber = "102000076";
            vm.AccountNumber = "018238712";

            vm.CheckLayout = GetDefaultCheckLayoutForRegion();

            return vm;
        }

        public decimal? GetCurrentBankBalance(int bankId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.fn_GetBankBalance(bankId).FirstOrDefault();
                if (results == null)
                    return null;

                return results.Balance;
            }
        }

        public List<portal_spGet_C_BankStatementDetails_Result> GetBankStatementDetails(int bankId, DateTime startDate, DateTime endDate)
        {
            CheckViewModel vm = new CheckViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.portal_spGet_C_BankStatementDetails(bankId, startDate, endDate).ToList();
                return results;
            }
        }

        public List<portal_spGet_BankStatment_Result> GetBankStatementDetailList(string RegionId, DateTime startDate, DateTime endDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.portal_spGet_BankStatment(RegionId ?? SelectedRegionId.ToString(), startDate, endDate).ToList();
                return results;
            }
        }

        public CheckViewModel GetCheckDetails(int id)
        {
            CheckViewModel vm = new CheckViewModel();

            vm.CheckLayout = GetDefaultCheckLayoutForRegion();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                portal_spGet_C_CheckDetails_Result result = context.portal_spGet_C_CheckDetails(id).FirstOrDefault();

                if (result == null)
                    return vm;

                vm.RegionName = result.RegionName;
                vm.RegionAccountType = result.RegionAccountType;
                vm.RegionAddress1 = result.RegionAddress1;
                vm.RegionAddress2 = result.RegionAddress2;

                vm.BankName = result.BankName;
                vm.BankRegion = result.BankRegion;
                vm.BankAddress1 = result.BankAddress1;
                vm.BankAddress2 = result.BankAddress2;
                vm.BankPhone = result.BankPhone;
                vm.BankNumber = result.BankNumber;

                vm.PayeeNumber = result.PayeeNumber;
                vm.PayeeName = result.TypeListId == 2 ? result.PayeeName + ", an Authorized Franchisee" : result.PayeeName;
                vm.PayeeAddress1 = result.PayeeAddress1;
                vm.PayeeAddress2 = result.PayeeAddress2;

                vm.CheckNumber = result.CheckNumber;
                vm.Date = (result.CheckDate != null) ? ((DateTime)result.CheckDate).ToString("M/dd/yyyy") : "";
                vm.BillMonth = (int)result.BillMonth;
                vm.BillYear = (int)result.BillYear;
                vm.TypeListId = (int)result.TypeListId;


                decimal amt = 0.00M;
                if (result.CheckAmount != null)
                    amt = (decimal)result.CheckAmount;

                vm.PayText = string.Format("{0} and {1:00}/100 DOLLARS", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vm.NumberToWords((int)amt)), (int)((amt % 1.0M) * 100));
                vm.PayDollars = string.Format("{0:C}", amt);

                vm.RoutingNumber = result.RoutingNumber;
                vm.AccountNumber = result.AccountNumber;

                vm.CheckType = result.CheckType;
            }

            return vm;
        }

        public CheckViewModelFinalizedReport GetCheckDetailsFinalizedReport(int id)
        {
            CheckViewModelFinalizedReport vm = new CheckViewModelFinalizedReport();

            vm.CheckLayout = GetDefaultCheckLayoutForRegion();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                portal_spGet_C_CheckDetails_Result result = context.portal_spGet_C_CheckDetails(id).FirstOrDefault();

                if (result == null)
                    return vm;

                vm.RegionName = result.RegionName;
                vm.RegionAccountType = result.RegionAccountType;
                vm.RegionAddress1 = result.RegionAddress1;
                vm.RegionAddress2 = result.RegionAddress2;

                vm.BankName = result.BankName;
                vm.BankRegion = result.BankRegion;
                vm.BankAddress1 = result.BankAddress1;
                vm.BankAddress2 = result.BankAddress2;
                vm.BankPhone = result.BankPhone;
                vm.BankNumber = result.BankNumber;

                vm.PayeeNumber = result.PayeeNumber;
                vm.PayeeName = result.TypeListId == 2 ? result.PayeeName + ", an Authorized Franchisee" : result.PayeeName;
                vm.PayeeAddress1 = result.PayeeAddress1;
                vm.PayeeAddress2 = result.PayeeAddress2;

                vm.CheckNumber = result.CheckNumber;
                vm.Date = (result.CheckDate != null) ? ((DateTime)result.CheckDate).ToString("M/dd/yyyy") : "";
                vm.BillMonth = (int)result.BillMonth;
                vm.BillYear = (int)result.BillYear;
                vm.TypeListId = (int)result.TypeListId;

                decimal amt = 0.00M;
                if (result.CheckAmount != null)
                    amt = (decimal)result.CheckAmount;

                vm.PayText = string.Format("{0} and {1:00}/100 DOLLARS", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vm.NumberToWords((int)amt)), (int)((amt % 1.0M) * 100));
                vm.PayDollars = string.Format("{0:C}", amt);

                vm.RoutingNumber = result.RoutingNumber;
                vm.AccountNumber = result.AccountNumber;

                vm.CheckType = result.CheckType;
            }

            return vm;
        }



        public APBillCheckViewModel GetAPBillCheck(int checkBookId)
        {
            APBillCheckViewModel apBillViewModel = new APBillCheckViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                var apBill = context.APBills.Where(o => o.APBillId == (int)checkBook.SourceId).FirstOrDefault();
                if (apBill == null)
                    return null;

                var masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == (int)apBill.MasterTrxId).FirstOrDefault();

                apBillViewModel.CheckMemo = masterTrxDetail.DetailDescription;
                apBillViewModel.APBillCheck = apBill;

                return apBillViewModel;

            }
        }


        public ManualCheck GetManualCheckForCheck(int checkBookId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                var apBill = context.APBills.Where(o => o.APBillId == (int)checkBook.SourceId).FirstOrDefault();
                if (apBill == null || apBill.ManualCheckId == null)
                    return null;

                var manualCheck = context.ManualChecks.Where(o => o.ManualCheckId == (int)apBill.ManualCheckId).FirstOrDefault();
                return manualCheck;
            }
        }


        public List<RegionViewModel> GetSelectedRegionData(int RegionId)
        {
            string sqlStr = "SELECT * FROM Region where RegionId = " + RegionId;
            List<RegionViewModel> rvm = new List<RegionViewModel>();


            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var regionData = conn.Query(sqlStr).ToList();
                rvm = SqlMapper.Query<RegionViewModel>(
                                conn, sqlStr).ToList();
            }

            return rvm;

        }


        public int InsertDeposit(DepositViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel transactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            transactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(22, inputdata.RegionId);

            string nextTrxNumber = null;
            if (transactionNumberConfigViewModel != null)
                nextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(22, inputdata.RegionId, (DateTime)inputdata.CreatedDate);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Bank bank = context.Banks.Where(o => o.BankId == inputdata.BankId).FirstOrDefault();
                if (bank == null || bank.LedgerAcctId == null)
                    return -1;

                // deposit mastertrx

                MasterTrx masterTrx = new MasterTrx();
                masterTrx.MasterTrxTypeListId = 22; // deposit
                masterTrx.ClassId = inputdata.ClassId;
                masterTrx.TypeListId = inputdata.TypeListId; // 1 = customer, 2 = franchisee
                masterTrx.RegionId = inputdata.RegionId;
                masterTrx.TrxDate = inputdata.DepositDate;
                masterTrx.BillMonth = inputdata.DepositDate.Month;
                masterTrx.BillYear = inputdata.DepositDate.Year;
                masterTrx.StatusId = 1; // open
                masterTrx.CreatedBy = inputdata.CreatedBy;
                masterTrx.CreatedDate = inputdata.CreatedDate;

                context.MasterTrxes.Add(masterTrx);
                context.SaveChanges();

                // deposit

                Deposit deposit = new Deposit();
                deposit.MasterTrxId = masterTrx.MasterTrxId;
                deposit.RegionId = inputdata.RegionId;
                deposit.TypeListId = inputdata.TypeListId;
                deposit.ClassId = inputdata.ClassId;
                deposit.DepositTypeId = inputdata.DepositTypeId;
                deposit.TransactionDate = inputdata.DepositDate;
                deposit.TransactionStatusListId = 6; // paid
                deposit.TransactionNumber = nextTrxNumber;
                deposit.Description = inputdata.Description;
                deposit.Amount = inputdata.Amount;
                deposit.CreatedBy = inputdata.CreatedBy;
                deposit.CreatedDate = inputdata.CreatedDate;

                context.Deposits.Add(deposit);
                context.SaveChanges();

                if (transactionNumberConfigViewModel != null)
                {
                    transactionNumberConfigViewModel.LastNumber = transactionNumberConfigViewModel.LastNumber + 1;
                    CompanySvc.SaveTransactionNumberConfig(transactionNumberConfigViewModel.ToModel<TransactionNumberConfig, TransactionNumberConfigViewModel>()).ToModel<TransactionNumberConfigViewModel, TransactionNumberConfig>();
                }

                // master trx detail

                MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                masterTrxDetail.MasterTrxTypeListId = 22; // deposit
                masterTrxDetail.HeaderId = deposit.DepositId;
                masterTrxDetail.RegionId = inputdata.RegionId;
                masterTrxDetail.DetailDescription = inputdata.Description;
                if (inputdata.DepositTypeId == 11)
                    masterTrxDetail.AmountTypeListId = 2;
                else
                    masterTrxDetail.AmountTypeListId = 1; // debit
                masterTrxDetail.FeesDetail = false;
                masterTrxDetail.TaxDetail = false;
                masterTrxDetail.Total = inputdata.Amount;

                masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                masterTrxDetail.IsDelete = false;
                masterTrxDetail.Transactiondate = inputdata.CreatedDate;
                context.MasterTrxDetails.Add(masterTrxDetail);
                context.SaveChanges();

                // checkbook

                CheckBook cb = new CheckBook();
                cb.FundTypeListId = 3; // check -- maybe temporary if user can select deposit method in the future
                cb.BankId = inputdata.BankId;
                cb.TypeListId = inputdata.TypeListId;
                cb.ClassId = inputdata.ClassId;
                cb.RegionId = inputdata.RegionId;
                cb.MasterTrxId = masterTrx.MasterTrxId;
                cb.BillMonth = masterTrx.BillMonth;
                cb.BillYear = masterTrx.BillYear;
                cb.ReferenceNumber = inputdata.ReferenceNo;
                cb.Notes = inputdata.Description;
                cb.TransactionStatusListId = 6; // paid
                cb.SourceTypeListId = 22; // deposit
                cb.SourceId = deposit.DepositId;
                cb.CreatedBy = inputdata.CreatedBy;
                cb.CreatedDate = inputdata.CreatedDate;

                context.CheckBooks.Add(cb);
                context.SaveChanges();

                // general ledger -- debit from Undeposit, Pre-Post Bank Account

                GeneralLedger gl1 = new GeneralLedger();
                gl1.MasterTrxId = masterTrx.MasterTrxId;
                gl1.LedgerAcctId = 8; // Undeposit, Pre-Post Bank Account
                gl1.Credit = 0;
                gl1.Debit = inputdata.Amount;
                gl1.IsDelete = false;

                context.GeneralLedgers.Add(gl1);
                context.SaveChanges();

                // general ledger -- credit to bank's ledger acct

                GeneralLedger gl2 = new GeneralLedger();
                gl2.MasterTrxId = masterTrx.MasterTrxId;
                gl2.LedgerAcctId = bank.LedgerAcctId;
                gl2.Credit = inputdata.Amount;
                gl2.Debit = 0;
                gl2.IsDelete = false;

                context.GeneralLedgers.Add(gl2);
                context.SaveChanges();

                return deposit.DepositId;
            }
        }

        public int InsertManualCheck(ManualCheckViewModel inputdata)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                // manual check

                ManualCheck check = new ManualCheck();
                check.RegionId = inputdata.RegionId;
                check.TypeListId = inputdata.TypeListId;
                check.ClassId = inputdata.ClassId;
                check.AddressId = inputdata.AddressId;
                check.BankId = inputdata.BankId;
                check.BillMonth = inputdata.BillMonth;
                check.BillYear = inputdata.BillYear;
                check.CheckDate = inputdata.CheckDate;
                check.Memo = inputdata.Memo;
                check.IsPrintMemo = inputdata.PrintMemo;
                check.Amount = inputdata.Amount;
                check.CreatedBy = inputdata.CreatedBy;
                check.CreatedDate = inputdata.CreatedDate;

                context.ManualChecks.Add(check);
                context.SaveChanges();

                return check.ManualCheckId;
            }
        }

        public List<GenericPayee> SearchPayeeList(string namePrefix, int limit = 0)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var customers = context.Customers.Where(x => (x.RegionId == this.SelectedRegionId || this.SelectedRegionId == 0) &&
                    x.Name.Contains(namePrefix)).Take(limit);
                var franchisees = context.Franchisees.Where(x => (x.RegionId == this.SelectedRegionId || this.SelectedRegionId == 0) &&
                    x.Name.Contains(namePrefix)).Take(limit);

                var payees = new List<GenericPayee>();
                foreach (var o in customers)
                    payees.Add(new GenericPayee(o.Name, 1, o.CustomerId, o.RegionId ?? -1));
                foreach (var o in franchisees)
                    payees.Add(new GenericPayee(o.Name, 2, o.FranchiseeId, o.RegionId ?? -1));

                if (limit > 0)
                    return payees.Take(limit).ToList();
                else
                    return payees;
            }
        }

        public List<portal_spGet_AP_PendingUnprintedManualCheckList_Result> GetPendingUnprintedManualCheckList(int RegionId, int trxStatus)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.portal_spGet_AP_PendingUnprintedManualCheckList(RegionId, trxStatus).ToList();
                return results;
            }
        }


        public List<portal_spGet_C_UnprintedManualCheckList_Result> GetUnprintedManualCheckList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var results = context.portal_spGet_C_UnprintedManualCheckList(this.SelectedRegionId).ToList();

                return results;
            }
        }

        public List<CheckBookTransactionTypeList> GetAll_checkbookTransactionTypeList()
        {
            List<CheckBookTransactionTypeList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.CheckBookTransactionTypeLists.Where(x => x.Payment == true).ToList();
                return data;
            }
        }


        public List<CompanySearchModel> GetSearchCompanies(string searchText)
        {
            List<CompanySearchModel> lstCompanySearch = new List<CompanySearchModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SearchText", searchText);
                parmas.Add("@RegionId", SelectedRegionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_COM_CompanySearch", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstCompanySearch = multipleresult.Read<CompanySearchModel>().ToList();
                    }
                }
            }
            return lstCompanySearch;
        }

        public int UpdtedRegionRemitTo(JKApi.Data.DAL.Region objRegion, RemitTo onjRemitTo)
        {
            using (var context = new jkDatabaseEntities())
            {
                var RegionData = context.Regions.Where(w => w.RegionId == objRegion.RegionId);
                if (RegionData != null)
                {
                    JKApi.Data.DAL.Region model = new Data.DAL.Region();
                    model = RegionData.FirstOrDefault();
                    model.Displayname = objRegion.Displayname;
                    model.ReportName = objRegion.ReportName;
                    model.Address = objRegion.Address;
                    model.City = objRegion.City;
                    model.State = objRegion.State;
                    model.PostalCode = objRegion.PostalCode;
                    model.Phone = objRegion.Phone;
                    context.SaveChanges();
                }

                var RemitToData = context.RemitToes.Where(w => w.RegionId == onjRemitTo.RegionId);
                if (RemitToData != null)
                {
                    RemitTo RemitToesModel = new RemitTo();
                    RemitToesModel = RemitToData.FirstOrDefault();
                    RemitToesModel.Address = objRegion.Address;
                    RemitToesModel.City = objRegion.City;
                    RemitToesModel.State = objRegion.State;
                    RemitToesModel.PostalCode = objRegion.PostalCode;
                    context.SaveChanges();
                }
            }
            return 1;

        }

        public int UpdateConfigurationValue(int Id, string ConfigValue)
        {
            if (Id > 0)
            {
                SaveConfigurationData(Id, ConfigValue);
                return Id;
            }
            return 0;
        }

        public int SaveConfigurationData(int id, string value)
        {
            string spName = "spsave_Sys_Configuration";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@valuetype", id);
                cmd.Parameters.AddWithValue("@value", value);
                //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                //returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                //var result = returnParameter.Value;
                return Convert.ToInt32(id);
            }
        }
        public int SaveRegionConfigurationData(int id, string value)
        {
            string spName = "spsave_RegionConfigurationData";
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegionConfigurationId", id);
                cmd.Parameters.AddWithValue("@value", value);
                //var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                //returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                //var result = returnParameter.Value;
                return Convert.ToInt32(id);
            }
        }
        public Data.DAL.TransactionNumberConfig SaveTransactionNumberConfigDetails(Data.DAL.TransactionNumberConfig TrxNumConfig)
        {

            using (var context = new jkDatabaseEntities())
            {
                JKApi.Data.DAL.TransactionNumberConfig trxNumConfig = context.TransactionNumberConfigs.SingleOrDefault(o => o.TransactionNumberConfigId == TrxNumConfig.TransactionNumberConfigId);
                //trxNumConfig.Name = TrxNumConfig.Name;
                trxNumConfig.Prefix = TrxNumConfig.Prefix;
                trxNumConfig.LastNumber = TrxNumConfig.LastNumber;
                context.SaveChanges();
            }

            return TrxNumConfig;

        }

        public bool InsertBankTrx(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int PaymentTrxTypeId, decimal ApplyAmount, string ChaqueNumber,
           string PayeeName, string PayeeNumber)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();

                    parmas.Add("@TransactionDate", TransactionDate);
                    parmas.Add("@PayeeId", PayeeId);
                    parmas.Add("@PayeeType", PayeeType);
                    parmas.Add("@DepositReason", DepositReason);
                    parmas.Add("@PaymentTrxTypeId", PaymentTrxTypeId);
                    parmas.Add("@ApplyAmount", ApplyAmount);
                    parmas.Add("@ChaqueNumber", ChaqueNumber);
                    parmas.Add("@CreatedBy", LoginUserId);
                    parmas.Add("@PayeeName", PayeeName);
                    parmas.Add("@PayeeNumber", PayeeNumber);
                    parmas.Add("@LockboxId", 0);
                    parmas.Add("@RegionId", SelectedRegionId);

                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_BankTrxEntry", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return true;
                        }
                    }
                }


                return true;
            }
        }


        public bool SaveCustomerParentChildSetting(int ParentId, string ChildIds, bool IsThirdParty = false, string ConsolidatedInvoice = "")
        {
            bool _retVal = true;

            using (var context = new jkDatabaseEntities())
            {
                List<string> _ChildCustomerLIST = ChildIds.Split(',').AsList<string>();
                List<string> _ConsolidatedInvoiceLIST = ConsolidatedInvoice.Split(',').AsList<string>();
                int _rowId = 0;
                foreach (string _childId in _ChildCustomerLIST)
                {
                    if (!String.IsNullOrEmpty(_childId))
                    {
                        if (!IsThirdParty)
                        {
                            if (int.Parse(_childId) != ParentId)
                            {

                                JKApi.Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(c => c.CustomerId.ToString() == _childId);
                                if (oCustomer != null)
                                {
                                    oCustomer.ParentId = ParentId;
                                    context.SaveChanges();
                                }

                                JKApi.Data.DAL.Customer oCustomerP = context.Customers.SingleOrDefault(c => c.CustomerId == ParentId);
                                if (oCustomerP != null)
                                {
                                    oCustomerP.Parent = true;
                                    context.SaveChanges();
                                }


                                JKApi.Data.DAL.BillSetting oBillSetting = context.BillSettings.Where(c => c.CustomerId.ToString() == _childId && (c.IsActive != null ? (int)c.IsActive : 0) == 1).FirstOrDefault();
                                if (oBillSetting != null)
                                {
                                    oBillSetting.ConsolidatedInvoice = bool.Parse(String.IsNullOrEmpty(_ConsolidatedInvoiceLIST[_rowId].ToString()) ? "false" : _ConsolidatedInvoiceLIST[_rowId].ToString());
                                    context.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            JKApi.Data.DAL.Customer oCustomer = context.Customers.SingleOrDefault(c => c.CustomerId == int.Parse(_childId));
                            if (oCustomer != null)
                            {
                                oCustomer.ThirdParty_CompanyId = ParentId;
                                context.SaveChanges();
                            }
                        }
                    }
                    _rowId++;
                }
            }
            return _retVal;
        }

        public bool SavePaymentGroup(string GroupName, string CustomerIds)
        {
            bool _retVal = true;

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                ////Insert PaymentGroup
                //PaymentGroup oPaymentGroup = new PaymentGroup();
                //oPaymentGroup.Name = GroupName;
                //oPaymentGroup.CreatedBy = LoginUserId;
                //oPaymentGroup.CreatedDate = DateTime.Now;
                //oPaymentGroup.ModifiedBy = LoginUserId;
                //oPaymentGroup.ModifiedDate = DateTime.Now;
                //oPaymentGroup.IsDelete = false;
                //context.PaymentGroups.Add(oPaymentGroup);
                //context.SaveChanges();

                //PaymentGroupCustomer oCustomer;
                ////Insert PaymentGroup Customers
                //foreach (string _childId in CustomerIds.Split(','))
                //{
                //    if (!String.IsNullOrEmpty(_childId))
                //    {
                //        oCustomer = new PaymentGroupCustomer();
                //        oCustomer.CustomerId = int.Parse(_childId.Trim());
                //        oCustomer.PaymentGroupId = oPaymentGroup.PaymentGroupId;
                //        oCustomer.IsDelete = false;
                //        context.PaymentGroupCustomers.Add(oCustomer);
                //        context.SaveChanges();
                //    }
                //}
                var parmas = new DynamicParameters();

                parmas.Add("@GroupName", GroupName);
                parmas.Add("@CustomerIds", CustomerIds);
                parmas.Add("@CreatedBy", LoginUserId);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_sp_Insert_paymentGroup", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return true;
                    }
                }

            }
            return _retVal;
        }


        public List<PaidInvoiceSearchOrderByList> PaidInvoiceSearchOrderByList()
        {
            List<PaidInvoiceSearchOrderByList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.PaidInvoiceSearchOrderByLists.ToList();
                return data;
            }
        }


        /*************************************************************************/
        /*9-20-2018 : Maria: Function routes if ecom pull data from JKFMSDEV
         *                   if fms live pull data from Traverse                */
        /************************************************************************/
        public List<TRPaidInvoicesViewModel> GetPaidInvoicesList(string FromDate, string ToDate, string FromCheck, string ToCheck, string FromInvoice, string ToInvoice, string FromVendor, string OrderBy, int RegionId)
        {

            List<TRPaidInvoicesViewModel> lstVendor = new List<TRPaidInvoicesViewModel>();
            List<TRPaidInvoicesViewModel.VendorInvoiceViewModel> lstFranVendorList = new List<TRPaidInvoicesViewModel.VendorInvoiceViewModel>();
            bool isLocal = HttpContext.Current.Request.IsLocal;
            string curURL = HttpContext.Current.Request.Url.AbsoluteUri;

            if (isLocal || curURL.Contains("http://fms.ecom.bid"))
            {

                using (var context = new jkDatabaseEntities())
                {
                    //TransactionStatusListId == 6 = PAID
                    lstFranVendorList = context.VendorInvoices.Where(v => v.TransactionStatusListId == 6).Select(i => new TRPaidInvoicesViewModel.VendorInvoiceViewModel
                    {
                        VendorId = i.VendorCode.ToString(),
                        InvoiceNum = i.VendorInvoiceNo,
                        InvoiceDate = (DateTime)i.TransactionDate,
                        CheckDate = (DateTime)i.TransactionDate,
                        CheckNum = "",
                        ChkGrossAmtDue = (decimal)i.GrossTotal,
                        ChkDiscAmt = (decimal)0.00,
                        ChkNetPaid = (decimal)0.00

                    }).ToList();

                    lstVendor = lstFranVendorList.MapEnumerable<TRPaidInvoicesViewModel, TRPaidInvoicesViewModel.VendorInvoiceViewModel>().ToList();


                    return lstVendor;
                }
            }
            else
            {
                lstVendor = GetPaidInvoices(FromDate, ToDate, FromCheck, ToCheck, FromInvoice, ToInvoice, FromVendor, OrderBy, RegionId);
            }

            return lstVendor;

        }


        public List<TRPaidInvoicesViewModel> GetPaidInvoices(string FromDate, string ToDate, string FromCheck, string ToCheck, string FromInvoice, string ToInvoice, string FromVendor, string OrderBy, int RegionId)
        {
            string ToVendor = "";
            string Currency = "";
            string glPeriod = "";
            string fiscalYear = "";
            int optCheckCutoff = 1;
            string SpText = "spJKAPPaidInvoices";
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (FromDate == null)
            {
                FromDate = "1/1/1900";
            }

            if (FromVendor == "")
            {
                ToVendor = "zzzzzzzzzz";
            }
            else
            {
                ToVendor = FromVendor;
            }

            if (FromCheck == "")
            {
                ToCheck = "zzzzzzzzzz";
            }

            if (FromInvoice == "")
            {
                ToInvoice = "zzzzzzzzzz";
            }


            List<TRPaidInvoicesViewModel> lstPaidInv = new List<TRPaidInvoicesViewModel>();
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            using (var context = new jkDatabaseEntities())
            {
                region = context.Regions.SingleOrDefault(r => r.RegionId == RegionId);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["TrvDBConnection"].ConnectionString;
            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(SpText, conn);
                SqlDataAdapter sqladapter = new SqlDataAdapter();
                DataTable dataTable = new DataTable();

                cmd.Parameters.Add(new SqlParameter("@VendorIDFrom ", FromVendor));
                cmd.Parameters.Add(new SqlParameter("@VendorIDThru ", ToVendor));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNumFrom ", FromInvoice));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNumThru ", ToInvoice));
                cmd.Parameters.Add(new SqlParameter("@InvoiceDateFrom ", FromDate));
                cmd.Parameters.Add(new SqlParameter("@InvoiceDateThru ", ToDate));
                cmd.Parameters.Add(new SqlParameter("@CheckNoFrom ", FromCheck));
                cmd.Parameters.Add(new SqlParameter("@CheckNoThru ", ToCheck));
                cmd.Parameters.Add(new SqlParameter("@CurrencyId ", Currency));
                cmd.Parameters.Add(new SqlParameter("@GlPeriod ", glPeriod));
                cmd.Parameters.Add(new SqlParameter("@FiscalYear ", fiscalYear));
                cmd.Parameters.Add(new SqlParameter("@optCheckCutoff ", optCheckCutoff));
                cmd.Parameters.Add(new SqlParameter("@OrderBy ", OrderBy));
                cmd.CommandType = CommandType.StoredProcedure;
                sqladapter.SelectCommand = cmd;
                sqladapter.Fill(dataTable);

                adapter = new SqlDataAdapter(cmd);

                if (dataTable != null)
                {
                    foreach (DataRow keyRow in dataTable.Rows)
                    {
                        lstPaidInv.Add(new TRPaidInvoicesViewModel
                        {
                            VendorId = keyRow["vendorid"].ToString(),
                            InvoiceNum = keyRow["InvoiceNum"].ToString(),
                            InvoiceDate = Convert.ToDateTime(keyRow["InvoiceDate"].ToString()),
                            CheckDate = Convert.ToDateTime(keyRow["CheckDate"].ToString()),
                            CheckNum = keyRow["CheckNum"].ToString(),
                            ChkGrossAmtDue = Convert.ToDecimal(keyRow["ChkGrossAmtDue"].ToString()),
                            ChkDiscAmt = Convert.ToDecimal(keyRow["ChkDiscAmt"].ToString()),
                            ChkNetPaid = Convert.ToDecimal(keyRow["ChkNetPaid"].ToString()),

                        });
                    }
                }

                conn.Close();
                return lstPaidInv;

            }

        }



        public List<TROpenInvoicesViewModel> GetTROpenInvoices(string fromDate, string toDate, int regionId, string vendorId = " ")
        {
            string toVendorId = "";
            string Currency = "";
            string glPeriod = "";
            string fiscalYear = "";
            int optCheckCutoff = 1;
            string discountDueDate = "1/1/1900";
            //            SqlDataAdapter adapter;
            //            DataSet ds = new DataSet();

            if (fromDate == null)
            {
                fromDate = "1/1/1900";
            }

            if (vendorId == " ")
            {
                toVendorId = "zzzzzzzzzz";
            }
            else
            {
                toVendorId = vendorId;
            }



            List<TROpenInvoicesViewModel> lstOpenInvoices = new List<TROpenInvoicesViewModel>();
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            using (var context = new jkDatabaseEntities())
            {
                region = context.Regions.SingleOrDefault(r => r.RegionId == regionId);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["TrvDBConnection"].ConnectionString;
            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var queryParams = new DynamicParameters();
                queryParams.Add("@VendorIDFrom ", vendorId);
                queryParams.Add("@VendorIDThru ", toVendorId);
                queryParams.Add("@StartDueDate ", fromDate);
                queryParams.Add("@EndDueDate ", toDate);
                queryParams.Add("@DiscountDueDate ", discountDueDate);
                using (var resultData = conn.QueryMultiple("dbo.sp_FMS_JKAPOpenInvoices", queryParams, commandType: CommandType.StoredProcedure))
                {
                    if (resultData != null)
                    {
                        lstOpenInvoices = resultData.Read<TROpenInvoicesViewModel>().ToList();
                    }
                    else
                    {
                        lstOpenInvoices = null;
                    }

                }
            }
            return lstOpenInvoices;
        }


        /*************************************************************************/
        /*9-20-2018 : Maria: Function routes if ecom pull data from JKFMSDEV
         *                   if fms live pull data from Traverse                */
        /************************************************************************/
        public List<TRPaidInvoicesViewModel> GetInvoiceSearchList(string InvoiceNum, int RegionId)
        {

            List<TRPaidInvoicesViewModel> lstVendor = new List<TRPaidInvoicesViewModel>();
            List<TRPaidInvoicesViewModel.VendorInvoiceViewModel> lstFranVendorList = new List<TRPaidInvoicesViewModel.VendorInvoiceViewModel>();
            bool isLocal = HttpContext.Current.Request.IsLocal;
            string curURL = HttpContext.Current.Request.Url.AbsoluteUri;

            if (isLocal || curURL.Contains("http://fms.ecom.bid"))
            {

                using (var context = new jkDatabaseEntities())
                {
                    lstFranVendorList = context.VendorInvoices.Where(v => v.VendorInvoiceNo == InvoiceNum).Select(i => new TRPaidInvoicesViewModel.VendorInvoiceViewModel
                    {
                        VendorId = i.VendorCode.ToString(),
                        InvoiceNum = i.VendorInvoiceNo,
                        InvoiceDate = (DateTime)i.TransactionDate,
                        CheckDate = (DateTime)i.TransactionDate,
                        CheckNum = "",
                        ChkGrossAmtDue = (decimal)i.GrossTotal,
                        ChkDiscAmt = (decimal)0.00,
                        ChkNetPaid = (decimal)0.00

                    }).ToList();

                    lstVendor = lstFranVendorList.MapEnumerable<TRPaidInvoicesViewModel, TRPaidInvoicesViewModel.VendorInvoiceViewModel>().ToList();


                    return lstVendor;
                }
            }
            else
            {
                lstVendor = GetVendorInvoice(InvoiceNum, RegionId);
            }

            return lstVendor;

        }


        public List<TRPaidInvoicesViewModel> GetVendorInvoice(string InvoiceNum, int RegionId)
        {

            string SpText = "spJKSearchInvoice";
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            List<TRPaidInvoicesViewModel> lstPaidInv = new List<TRPaidInvoicesViewModel>();
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            using (var context = new jkDatabaseEntities())
            {
                region = context.Regions.SingleOrDefault(r => r.RegionId == RegionId);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["TrvDBConnection"].ConnectionString;
            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(SpText, conn);
                SqlDataAdapter sqladapter = new SqlDataAdapter();
                DataTable dataTable = new DataTable();

                cmd.Parameters.Add(new SqlParameter("@invoicenum ", InvoiceNum));
                cmd.CommandType = CommandType.StoredProcedure;
                sqladapter.SelectCommand = cmd;
                sqladapter.Fill(dataTable);

                adapter = new SqlDataAdapter(cmd);

                if (dataTable != null)
                {
                    foreach (DataRow keyRow in dataTable.Rows)
                    {
                        lstPaidInv.Add(new TRPaidInvoicesViewModel
                        {

                            InvoiceNum = keyRow["invoicenum"].ToString(),
                            InvoiceDate = Convert.ToDateTime(keyRow["invoicedate"].ToString()),
                            VendorName = keyRow["name"].ToString(),
                            CheckNum = keyRow["checknum"].ToString(),
                            CheckDate = Convert.ToDateTime(keyRow["checkdate"].ToString()),
                            CheckAmount = Convert.ToDecimal(keyRow["checkamt"].ToString()),

                        });
                    }
                }

                conn.Close();
                return lstPaidInv;

            }

        }


        /// <summary>
        /// Retrieves Regular Account Detail and Balance from Traverse Database
        /// This is a Jani-King Corp Regions specific process
        /// </summary>
        /// <author>
        /// German Sosa 
        /// </author>
        /// <param name="RegionId">Corporate Region ID</param>
        /// <param name="MonthPeriod">Month Period</param>
        /// <param name="YearPeriod">Year Period</param>
        /// <returns>Returns List object with detailed bank account transaction detail and balance</returns>
        public List<RegularCheckRegisterViewModel> GetRegularAccountTransactions(int RegionId, int MonthPeriod, int YearPeriod)
        {
            List<RegularCheckRegisterViewModel> lstTransactions = new List<RegularCheckRegisterViewModel>();
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            var context = new jkDatabaseEntities();
            region = context.Regions.SingleOrDefault(r => r.RegionId == RegionId);

            string connectionString = SQLHelper.ConnectionStringTraverse;

            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);
            int numberOfDays = System.DateTime.DaysInMonth(YearPeriod, MonthPeriod);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var queryParams = new DynamicParameters();
                queryParams.Add("@GLAcct", "1100");
                queryParams.Add("@month", MonthPeriod);
                queryParams.Add("@year", YearPeriod);
                queryParams.Add("@StartDate", MonthPeriod + "/1/" + YearPeriod);
                queryParams.Add("@EndDate", MonthPeriod + "/" + numberOfDays + "/" + YearPeriod);
                queryParams.Add("@UnpostedYn", 0);
                queryParams.Add("@OrderBy", "transdate");
                using (var resultData = conn.QueryMultiple("dbo.sp_FMS_JKBankAccountRegister", queryParams, commandType: CommandType.StoredProcedure))
                {
                    if (resultData != null)
                    {
                        lstTransactions = resultData.Read<RegularCheckRegisterViewModel>().ToList();
                    }
                    else
                    {
                        lstTransactions = null;
                    }

                }
            }
            return lstTransactions;
        }

        public List<RegularCheckRegisterDetailViewModel> GetRegularAccountTransactionDetail(int RegionId, string CheckNumber)
        {
            List<RegularCheckRegisterDetailViewModel> lstTransactions = new List<RegularCheckRegisterDetailViewModel>();
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            var context = new jkDatabaseEntities();
            region = context.Regions.SingleOrDefault(r => r.RegionId == RegionId);

            string connectionString = SQLHelper.ConnectionStringTraverse;

            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CheckNumber", CheckNumber);
                using (var multipleresult = conn.QueryMultiple("dbo.sp_FMS_JKBankAccountRegisterSearch", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstTransactions = multipleresult.Read<RegularCheckRegisterDetailViewModel>().ToList();
                    }
                }
            }
            return lstTransactions;
        }
    }



    public class GenericPayee
    {
        public string Name { get; set; }
        public int TypeListId { get; set; }
        public int ClassId { get; set; }
        public int RegionId { get; set; }

        public GenericPayee(string name, int typeListId, int classId, int regionId)
        {
            this.Name = name;
            this.TypeListId = typeListId;
            this.ClassId = classId;
            this.RegionId = regionId;
        }
    }

    public class DepositViewModel
    {
        public int RegionId { get; set; }
        public int TypeListId { get; set; }
        public int ClassId { get; set; }

        public int BankId { get; set; }
        public int DepositTypeId { get; set; }
        public DateTime DepositDate { get; set; }

        public string Description { get; set; }
        public string ReferenceNo { get; set; }
        public decimal Amount { get; set; }

        public int? CreatedBy;
        public DateTime? CreatedDate;
        public int? ModifiedBy;
        public DateTime? ModifiedDate;
    }

    public class ManualCheckViewModel
    {
        public int RegionId { get; set; }
        public int TypeListId { get; set; }
        public int ClassId { get; set; }

        public int AddressId { get; set; }

        public int BankId { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public DateTime? CheckDate { get; set; }

        public string Memo { get; set; }
        public bool PrintMemo { get; set; }
        public decimal Amount { get; set; }

        public int? CreatedBy;
        public DateTime? CreatedDate;
        public int? ModifiedBy;
        public DateTime? ModifiedDate;
    }

    public class CheckConfigurationViewModel
    {
        public List<CheckLayoutViewModel> CheckLayouts { get; set; }
        public CheckCalibrationViewModel Calibration { get; set; }

        public CheckLayoutViewModel DefaultCheckLayout { get { return CheckLayouts.Where(o => o.IsDefault).FirstOrDefault() ?? CheckLayouts.FirstOrDefault(); } }
    }

    public class CheckLayoutViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public List<CheckLayoutElementViewModel> Elements { get; set; }
        public bool IsDefault { get; set; }

        public int? CreatedBy;
        public int? ModifiedBy;
        public DateTime? CreateDate;
        public DateTime? ModifiedDate;
    }

    public class CheckLayoutElementViewModel
    {
        public int Id { get; set; }
        public int ElementTypeId { get; set; }
        public string ElementType { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public string ImageFilename { get; set; }
        public bool IsActive { get; set; }

        public int? CreatedBy;
        public int? ModifiedBy;
        public DateTime? CreateDate;
        public DateTime? ModifiedDate;
    }

    public class CheckCalibrationViewModel
    {
        public int Id { get; set; }

        public double ShiftX { get; set; }
        public double ShiftY { get; set; }

        public int? CreatedBy;
        public int? ModifiedBy;
        public DateTime? CreateDate;
        public DateTime? ModifiedDate;
    }

    public class CheckViewModel
    {
        public string RegionName { get; set; }
        public string RegionAccountType { get; set; }
        public string RegionAddress1 { get; set; }
        public string RegionAddress2 { get; set; }

        public string BankName { get; set; }
        public string BankRegion { get; set; }
        public string BankAddress1 { get; set; }
        public string BankAddress2 { get; set; }
        public string BankPhone { get; set; }
        public string BankNumber { get; set; }

        public string PayeeNumber { get; set; }
        public string PayeeName { get; set; }
        public string PayeeAddress1 { get; set; }
        public string PayeeAddress2 { get; set; }

        public string LogoFilename { get; set; }
        public string SignatureFilename { get; set; }

        public string CheckNumber { get; set; }
        public string Date { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }

        public string PayText { get; set; }
        public string PayDollars { get; set; }

        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }

        public string CheckType { get; set; }
        public int TypeListId { get; set; }

        public int CheckTemplate { get; set; }

        public bool IsCheckSystemGenerated { get; set; }
        public FranchiseeReportDetailsViewModel FranchiseeReport { get; set; }
        public TurnAroundDetailsViewModel TurnAround { get; set; }

        public AccountingFeeRebateDetailsViewModel AccountingFeeebateDetails { get; set; }
        public ManualCheck ManualCheck { get; set; }

        public CheckLayoutViewModel CheckLayout { get; set; }

        public CheckCalibrationViewModel Calibration { get; set; }

        public APBillCheckViewModel APBillViewModel { get; set; }

        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }


    }

    public class APBillCheckViewModel
    {
        public APBill APBillCheck { get; set; }
        public string CheckMemo { get; set; }
    }

    public class CheckViewModelFinalizedReport
    {
        public string RegionName { get; set; }
        public string RegionAccountType { get; set; }
        public string RegionAddress1 { get; set; }
        public string RegionAddress2 { get; set; }

        public string BankName { get; set; }
        public string BankRegion { get; set; }
        public string BankAddress1 { get; set; }
        public string BankAddress2 { get; set; }
        public string BankPhone { get; set; }
        public string BankNumber { get; set; }

        public string PayeeNumber { get; set; }
        public string PayeeName { get; set; }
        public string PayeeAddress1 { get; set; }
        public string PayeeAddress2 { get; set; }

        public string LogoFilename { get; set; }
        public string SignatureFilename { get; set; }

        public string CheckNumber { get; set; }
        public string Date { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }

        public string PayText { get; set; }
        public string PayDollars { get; set; }

        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }

        public string CheckType { get; set; }
        public int TypeListId { get; set; }
        public FranchiseeReportFinalizedDetailsViewModel FranchiseeReport { get; set; }
        public ManualCheck ManualCheck { get; set; }

        public CheckLayoutViewModel CheckLayout { get; set; }

        public CheckCalibrationViewModel Calibration { get; set; }

        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }

    public partial class MasterTrasactionListAllData
    {
        public int MasterTrxId { get; set; }
        public Nullable<int> TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionDescription { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public string TypeListName { get; set; }
        public Nullable<int> LedgerAccountId { get; set; }
        public string LedgerAcctNumber { get; set; }
        public string LedgerAcctName { get; set; }
        public Nullable<int> LedgerSubAcctId { get; set; }
        public string LedgerSubAcctNumber { get; set; }
        public string LedgerSubAcctName { get; set; }
        public string Payee_Payer { get; set; }
        public Nullable<int> ParentLedgerAccountId { get; set; }


        public string Number { get; set; }

        public string GLName { get; set; }

    }
}