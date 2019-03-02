using JKApi.Service.Helper;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Service.Helper.Extension;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Franchisee;
using JK.Repository.Uow;
using JKApi.Data.DTOObject;
using JKViewModels.Franchisee;
using MoreLinq;
using System.Web.Script.Serialization;
using Dapper;
using JKViewModels.Company;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using JKViewModels;
using System.Web.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Web;
using System.Configuration;

namespace JKApi.Service.Service.Region
{
    public class FranchiseeService : BaseService, IFranchiseeService
    {


        #region ConstructorCalls
        static JKApi.Data.DAL.jkDatabaseEntities jkEntityModel = new Data.DAL.jkDatabaseEntities();
        public FranchiseeService(IJKEfUow uow)
        {
            Uow = uow;
        }

        public FranchiseeService()
        {
        }

        #endregion

        #region Service Calls



        #region Franchisee

        public IQueryable<JKApi.Data.DAL.Franchisee> GetFranchisee()
        {
            var qry = Uow.Franchisee.GetAll();
            return qry;
        }
        public IQueryable<JKApi.Data.DAL.Franchisee_Temp> GetFranchiseeTemp()
        {
            var qry = Uow.Franchisee_Temp.GetAll();
            return qry;
        }

        public IQueryable<JKApi.Data.DAL.Address> GetAddress(int id)
        {
            var qry = Uow.Address.GetAll().Where(x => x.ClassId == id && x.TypeListId == 2);
            return qry;
        }

        public JKApi.Data.DAL.Franchisee GetFranchiseeById(int id)
        {
            return Uow.Franchisee.GetById(id);
        }
        public JKApi.Data.DAL.Franchisee_Temp GetFranchiseeById_Temp(int id)
        {
            return Uow.Franchisee_Temp.GetById(id);
        }

        public List<Franchisee> GetFranchiseeByParentId(int id)
        {
            return Uow.Franchisee.GetAll().Where(x=> x.ParentId == id).ToList();
        }

        public JKApi.Data.DAL.Franchisee SaveFranchisee(JKApi.Data.DAL.Franchisee Franchisee, bool isStatusAdd = true)
        {
            var ID = Franchisee.FranchiseeId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                //Add New Franchisee
                //Franchisee.RegionId = SelectedRegionId;
                Franchisee.CreatedDate = DateTime.Now;
                Franchisee.IsActive = true;
                Franchisee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                Uow.Franchisee.Add(Franchisee);
                Uow.Commit();

                if (isStatusAdd)
                {
                    //Add Franchisee Status
                    Status oStatus = new Status();
                    oStatus.ClassId = Franchisee.FranchiseeId;
                    oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                    oStatus.CreatedDate = DateTime.Now;
                    oStatus.IsActive = true;
                    oStatus.StatusDate = DateTime.Now;
                    oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                    oStatus.StatusListId = 37;
                    Uow.Status.Add(oStatus);
                    Uow.Commit();

                    //Update Franchisee
                    Franchisee.StatusId = oStatus.StatusId;
                    Franchisee.StatusListId = oStatus.StatusListId;
                    Uow.Franchisee.Update(Franchisee);
                    Uow.Commit();
                }

            }
            else //update existing entry
            {
                var model = Uow.Franchisee.GetById(ID);
                if (model != null)
                {
                    model.Name = Franchisee.Name;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = -1;
                    Uow.Franchisee.Update(model);
                    Uow.Commit();
                }
            }
            return Franchisee;
        }

        public JKApi.Data.DAL.Franchisee_Temp SaveFranchisee_Temp(JKApi.Data.DAL.Franchisee_Temp Franchisee, bool isStatusAdd = true)
        {
            var ID = Franchisee.FranchiseeId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                //Add New Franchisee
                //Franchisee.RegionId = SelectedRegionId;
                Franchisee.CreatedDate = DateTime.Now;
                Franchisee.IsActive = true;
                Franchisee.IsDelete = false;
                Franchisee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                Uow.Franchisee_Temp.Add(Franchisee);
                Uow.Commit();

                if (isStatusAdd)
                {
                    //Add Franchisee Status
                    Status oStatus = new Status();
                    oStatus.ClassId = Franchisee.FranchiseeId;
                    oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                    oStatus.CreatedDate = DateTime.Now;
                    oStatus.IsActive = true;
                    oStatus.StatusDate = DateTime.Now;
                    oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                    oStatus.StatusListId = 37;
                    Uow.Status.Add(oStatus);
                    Uow.Commit();

                    //Update Franchisee
                    Franchisee.StatusId = oStatus.StatusId;
                    Franchisee.StatusListId = oStatus.StatusListId;
                    Uow.Franchisee_Temp.Update(Franchisee);
                    Uow.Commit();
                }

            }
            else //update existing entry
            {
                var model = Uow.Franchisee_Temp.GetById(ID);
                if (model != null)
                {
                    model.Name = Franchisee.Name;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = -1;
                    Uow.Franchisee_Temp.Update(model);
                    Uow.Commit();
                }
            }
            return Franchisee;
        }

        public JKApi.Data.DAL.Franchisee UpdateFranchiseePrefix(JKApi.Data.DAL.Franchisee Franchisee)
        {
            var ID = Franchisee.FranchiseeId;
            var isNew = ID == 0;
            {
                var model = Uow.Franchisee.GetById(ID);
                if (model != null)
                {
                    model.FranchiseeTypeListId = Franchisee.FranchiseeTypeListId;
                    model.dlr_id = Franchisee.dlr_id;
                    model.IsActive = Franchisee.IsActive;
                    model.Perfix = Franchisee.Perfix;
                    //model.ParentId = ID;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = -1;
                    Uow.Franchisee.Update(model);
                    Uow.Commit();
                }
            }
            return Franchisee;
        }

        public JKApi.Data.DAL.Franchisee UpdateFranchiseeParentId(JKApi.Data.DAL.Franchisee Franchisee)
        {
            var ID = Franchisee.FranchiseeId;
            var isNew = ID == 0;
            {
                var model = Uow.Franchisee.GetById(ID);
                if (model != null)
                {
                    model.ParentId = Franchisee.ParentId;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = -1;
                    Uow.Franchisee.Update(model);
                    Uow.Commit();
                }
            }
            return Franchisee;
        }

        public JKApi.Data.DAL.Franchisee DeleteFranchisee(int id)
        {
            var entity = Uow.Franchisee.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.Franchisee.Update(entity);
            Uow.Commit();
            return entity;
        }
        public JKApi.Data.DAL.Franchisee_Temp DeleteFranchisee_Temp(int FranchiseeId)
        {
            var entity = Uow.Franchisee_Temp.GetById(FranchiseeId);

            //Add Franchisee Status
            Status oStatus = new Status();
            oStatus.ClassId = FranchiseeId;
            oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
            oStatus.CreatedDate = DateTime.Now;
            oStatus.IsActive = true;
            oStatus.StatusDate = DateTime.Now;
            oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            oStatus.StatusListId = (int)JKApi.Business.Enumeration.FranchiseeStatusList.Inactive;
            Uow.Status.Add(oStatus);
            Uow.Commit();

            entity.StatusId = oStatus.StatusId;
            entity.StatusListId = oStatus.StatusListId;
            entity.IsActive = false;
            entity.IsDelete = true;
            Uow.Franchisee_Temp.Update(entity);
            Uow.Commit();
            return entity;
        }

        public string getfranchiseeno()
        {
            var regionno = Uow.RegionConfiguration.GetAll().Where(x => x.RegionConfigurationId == 34).Select(x => new { x.value }).FirstOrDefault();
            var franchiseeindex = Uow.RegionConfiguration.GetAll().Where(x => x.RegionConfigurationId == 4).Select(x => new { x.value }).FirstOrDefault();
            string franchiseeno = regionno.value + "" + franchiseeindex.value;
            return franchiseeno;
        }

        public RegionConfiguration GetRegionConfigurationbyId(int id)
        {
            return Uow.RegionConfiguration.GetById(id);
        }

        public RegionConfiguration SaveRegionConfiguration(RegionConfiguration RegionConfiguration)
        {
            var ID = RegionConfiguration.RegionConfigurationId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.RegionConfiguration.Add(RegionConfiguration);
                Uow.Commit();
            }
            else //update existing entry
            {
                Uow.RegionConfiguration.Update(RegionConfiguration);
                Uow.Commit();
            }
            return RegionConfiguration;
        }

        public List<SearchDateList> GetAll_OptionList()
        {
            List<SearchDateList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.SearchDateLists.ToList();
                //data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId }).ToList();
                return data;
            }
        }

        public List<TransactionStatusList> GetAll_TransactionStatusList()
        {
            List<TransactionStatusList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.TransactionStatusLists.ToList();
                return data;
            }
        }

        public List<FindersFeeTypeList> GetAll_FindersFeeTypeList()
        {
            List<FindersFeeTypeList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.FindersFeeTypeLists.Where(o => o.FindersFeeTypeListId != 9).ToList();
                return data;
            }
        }



        #endregion


        #region FranchiseeBillSettings

        public IQueryable<FranchiseeBillSetting> GetFranchiseeBillSettings()
        {
            var qry = Uow.FranchiseeBillSettings.GetAll();
            return qry;
        }

        public IQueryable<FranchiseeBillSettings_Temp> GetFranchiseeBillSettingsTemp()
        {
            var qry = Uow.FranchiseeBillSettings_Temp.GetAll();
            return qry;
        }

        public FranchiseeBillSetting GetFranchiseeBillSettingsById(int id)
        {
            return Uow.FranchiseeBillSettings.GetById(id);
        }
        public FranchiseeBillSettings_Temp GetFranchiseeBillSettingsByIdTemp(int id)
        {
            return Uow.FranchiseeBillSettings_Temp.GetById(id);
        }

        public FranchiseeBillSetting SaveFranchiseeBillSettings(FranchiseeBillSetting FranchiseeBillSettings)
        {
            var ID = FranchiseeBillSettings.FranchiseeBillSettingsId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeBillSettings.Add(FranchiseeBillSettings);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeBillSettings.Update(FranchiseeBillSettings);
                Uow.Commit();
            }

            return FranchiseeBillSettings;
        }

        public FranchiseeBillSettings_Temp SaveFranchiseeBillSettings_Temp(FranchiseeBillSettings_Temp FranchiseeBillSettings)
        {
            var ID = FranchiseeBillSettings.FranchiseeBillSettingsId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeBillSettings_Temp.Add(FranchiseeBillSettings);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeBillSettings_Temp.Update(FranchiseeBillSettings);
                Uow.Commit();
            }

            return FranchiseeBillSettings;
        }

        public FranchiseeBillSetting DeleteFranchiseeBillSettings(int id)
        {
            var entity = Uow.FranchiseeBillSettings.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FranchiseeBillSettings.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion

        #region IdentifierTypeList

        public IQueryable<IdentifierTypeList> GetIdentifierTypeList()
        {
            var qry = Uow.IdentifierTypeList.GetAll();
            return qry;
        }

        public IdentifierTypeList GetIdentifierTypeListById(int id)
        {
            return Uow.IdentifierTypeList.GetById(id);
        }

        public IdentifierTypeList SaveIdentifierTypeList(IdentifierTypeList IdentifierTypeList)
        {
            var ID = IdentifierTypeList.IdentifierTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.IdentifierTypeList.Add(IdentifierTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.IdentifierTypeList.Update(IdentifierTypeList);
                Uow.Commit();
            }

            return IdentifierTypeList;
        }

        public IdentifierTypeList DeleteIdentifierTypeList(int id)
        {
            var entity = Uow.IdentifierTypeList.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.IdentifierTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion


        #region ACHBank

        public IQueryable<ACHBank> GetACHBank()
        {
            var qry = Uow.ACHBank.GetAll();
            return qry;
        }

        public IQueryable<ACHBank_Temp> GetACHBankTemp()
        {
            var qry = Uow.ACHBank_Temp.GetAll();
            return qry;
        }

        public ACHBank GetACHBankById(int id)
        {
            return Uow.ACHBank.GetById(id);
        }

        public ACHBank SaveACHBank(ACHBank ACHBank)
        {
            var ID = ACHBank.ACHBankId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ACHBank.Add(ACHBank);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ACHBank.Update(ACHBank);
                Uow.Commit();
            }

            return ACHBank;
        }

        public ACHBank_Temp SaveACHBank_Temp(ACHBank_Temp ACHBank)
        {
            var ID = ACHBank.ACHBankId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.ACHBank_Temp.Add(ACHBank);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.ACHBank_Temp.Update(ACHBank);
                Uow.Commit();
            }

            return ACHBank;
        }

        public ACHBank DeleteACHBank(int id)
        {
            var entity = Uow.ACHBank.GetById(id);


            // Need a Column for soft Delete
            // entity.IsDelete = true;
            Uow.ACHBank.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion



        #region FranchiseeFullfillment

        public IQueryable<FranchiseeFullfillment> GetFranchiseeFullfillment()
        {
            var qry = Uow.FranchiseeFullfillment.GetAll();
            return qry;
        }

        public IQueryable<FranchiseeFullfillment_Temp> GetFranchiseeFullfillmentTemp()
        {
            var qry = Uow.FranchiseeFullfillment_Temp.GetAll();
            return qry;
        }

        public FranchiseeFullfillment GetFranchiseeFullfillmentById(int id)
        {
            return Uow.FranchiseeFullfillment.GetById(id);
        }

        public FranchiseeFullfillment SaveFranchiseeFullfillment(FranchiseeFullfillment FranchiseeFullfillment)
        {
            var ID = FranchiseeFullfillment.FranchiseeFullfillmentId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeFullfillment.Add(FranchiseeFullfillment);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeFullfillment.Update(FranchiseeFullfillment);
                Uow.Commit();
            }

            return FranchiseeFullfillment;
        }

        public FranchiseeFullfillment_Temp SaveFranchiseeFullfillmentTemp(FranchiseeFullfillment_Temp FranchiseeFullfillment)
        {
            var ID = FranchiseeFullfillment.FranchiseeFullfillmentId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeFullfillment_Temp.Add(FranchiseeFullfillment);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeFullfillment_Temp.Update(FranchiseeFullfillment);
                Uow.Commit();
            }

            return FranchiseeFullfillment;
        }

        public FranchiseeFullfillment DeleteFranchiseeFullfillment(int id)
        {
            var entity = Uow.FranchiseeFullfillment.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FranchiseeFullfillment.Update(entity);
            Uow.Commit();
            return entity;
        }

        public int GetFullfillmentWithFranchisee(int FranchiseeId)
        {
            var context = new jkDatabaseEntities();
            var data = context.FranchiseeFullfillments.Where(w => w.FranchiseeId == FranchiseeId);
            if (data == null || data.Count() == 0)
            {
                return 0;
            }
            else
            {
                return data.FirstOrDefault().FranchiseeFullfillmentId;
            }
        }

        #endregion


        #region FranchiseeContract

        public IQueryable<FranchiseeContract> GetFranchiseeContract()
        {
            var qry = Uow.FranchiseeContract.GetAll();
            return qry;
        }
        public IQueryable<FranchiseeContract_Temp> GetFranchiseeContractTemp()
        {
            var qry = Uow.FranchiseeContract_Temp.GetAll();
            return qry;
        }

        public FranchiseeOwnerList GetFranchiseeOwnerList(int? _classid, int _Contactid)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerLists.Where(x => x.FranchiseeId == _classid).FirstOrDefault();
            }
        }

        public FranchiseeOwnerList_Temp GetFranchiseeOwnerList_Temp(int _Contactid)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerList_Temp.Where(x => x.FranchiseeId == _Contactid).FirstOrDefault();
            }
        }

        public FranchiseOwnersList GetFranchiseeOwnerListByOwnerId(int Id)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerLists.Where(f => f.FranchiseeOwnerListId == Id).MapEnumerable<FranchiseOwnersList, FranchiseeOwnerList>().ToList().FirstOrDefault();
            }
        }

        public FranchiseOwnersList GetFranchiseeOwnerListByOwnerId_Temp(int Id)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerList_Temp.Where(f => f.FranchiseeOwnerListId == Id).MapEnumerable<FranchiseOwnersList, FranchiseeOwnerList_Temp>().ToList().FirstOrDefault();
            }
        }

        public FranchiseeOwnerList SaveFranchiseeOwnerList(FranchiseeOwnerList FranchiseeOwnerList)
        {
            var ID = FranchiseeOwnerList.FranchiseeOwnerListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeOwnerList.Add(FranchiseeOwnerList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeOwnerList.Update(FranchiseeOwnerList);
                Uow.Commit();
            }

            return FranchiseeOwnerList;
        }


        public FranchiseeOwnerList_Temp SaveFranchiseeOwnerList_Temp(FranchiseeOwnerList_Temp FranchiseeOwnerList_Temp)
        {
            var ID = FranchiseeOwnerList_Temp.FranchiseeOwnerListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeOwnerList_Temp.Add(FranchiseeOwnerList_Temp);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeOwnerList_Temp.Update(FranchiseeOwnerList_Temp);
                Uow.Commit();
            }

            return FranchiseeOwnerList_Temp;
        }

        public FranchiseeContract GetFranchiseeContractById(int id)
        {
            return Uow.FranchiseeContract.GetById(id);
        }

        public FeeConfiguration SaveFranchiseeFeeConfiguration(FeeConfiguration Feeconfiguration, int UserId)
        {
            var Id = Feeconfiguration.FeeConfigurationId;
            var isNew = Id == 0;

            if (isNew)
            {
                Feeconfiguration.CreatedBy = UserId;
                Feeconfiguration.CreatedDate = DateTime.Now;
                Uow.FranchiseeFeeConfiguration.Add(Feeconfiguration);
                Uow.Commit();
            }
            else
            {
                var entity = Uow.FranchiseeFeeConfiguration.GetById(Feeconfiguration.FeeConfigurationId);

                if (entity.MinimumAmount != Feeconfiguration.MinimumAmount || entity.EffectiveDate != entity.EffectiveDate)
                {
                    entity.IsActive = false;
                    entity.ModifiedBy = UserId;
                    entity.ModifiedDate = DateTime.Now;
                    Uow.FranchiseeFeeConfiguration.Update(entity);
                    Uow.Commit();

                    Feeconfiguration.IsActive = true;
                    Feeconfiguration.FeeConfigurationId = 0;
                    Feeconfiguration.CreatedBy = UserId;
                    Feeconfiguration.CreatedDate = DateTime.Now;
                    Uow.FranchiseeFeeConfiguration.Add(Feeconfiguration);
                    Uow.Commit();
                }
            }





            return Feeconfiguration;
        }

        public int SaveFranchiseeFeeConfigurationData(int FeeConfigurationInfoId, int ClassId, decimal MinimumAmoun, string EffectiveDate)
        {
            int Id = 0;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FeeConfigurationId", FeeConfigurationInfoId);
                parmas.Add("@ClassId", ClassId);
                parmas.Add("@MinimumAmoun", MinimumAmoun);
                parmas.Add("@EffectiveDate", EffectiveDate);
                parmas.Add("@LoginuserID", LoginUserId);
                parmas.Add("@RegionId", SelectedRegionId);
                parmas.Add("@FeeId", (int)JKApi.Business.Enumeration.Fee.Royalty);
                parmas.Add("@TypeListId", (int)JKApi.Business.Enumeration.TypeList.Franchisee);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spPost_F_UpdateFeeConfigurationData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        Id = multipleresult.Read<int>().FirstOrDefault();
                    }
                }
            }
            return Id;
        }
        public int SaveFranchiseeFeeConfigurationData_Temp(int FeeConfigurationInfoId, int ClassId, decimal MinimumAmoun, string EffectiveDate)
        {
            int Id = 0;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FeeConfigurationId", FeeConfigurationInfoId);
                parmas.Add("@ClassId", ClassId);
                parmas.Add("@MinimumAmoun", MinimumAmoun);
                parmas.Add("@EffectiveDate", EffectiveDate);
                parmas.Add("@LoginuserID", LoginUserId);
                parmas.Add("@RegionId", SelectedRegionId);
                parmas.Add("@FeeId", (int)JKApi.Business.Enumeration.Fee.Royalty);
                parmas.Add("@TypeListId", (int)JKApi.Business.Enumeration.TypeList.Franchisee);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spPost_F_UpdateFeeConfiguration_TempData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        Id = multipleresult.Read<int>().FirstOrDefault();
                    }
                }
            }
            return Id;
        }

        public int RemoveFeeConfigurationRecord(int Id)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FeeConfigurationId", Id);
                parmas.Add("@ModifiedBy", LoginUserId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spPost_F_RemoveFeeConfigurationRecord", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        Id = multipleresult.Read<int>().FirstOrDefault();
                    }
                }
            }
            return Id;
        }

        public FranchiseeContract SaveFranchiseeContract(FranchiseeContract FranchiseeContract)
        {
            var ID = FranchiseeContract.FranchiseeContractId;
            var isNew = ID == 0;

            //add new entry
            if (isNew)
            {
                Uow.FranchiseeContract.Add(FranchiseeContract);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeContract.Update(FranchiseeContract);
                Uow.Commit();
            }

            return FranchiseeContract;
        }
        public FranchiseeContract_Temp SaveFranchiseeContract_Temp(FranchiseeContract_Temp FranchiseeContract)
        {
            var ID = FranchiseeContract.FranchiseeContractId;
            var isNew = ID == 0;

            //add new entry
            if (isNew)
            {
                Uow.FranchiseeContract_Temp.Add(FranchiseeContract);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeContract_Temp.Update(FranchiseeContract);
                Uow.Commit();
            }

            return FranchiseeContract;
        }

        public FranchiseeContract DeleteFranchiseeContract(int id)
        {
            var entity = Uow.FranchiseeContract.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FranchiseeContract.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion



        #region FranchiseeContractTypeList

        public IQueryable<FranchiseeContractTypeList> GetFranchiseeContractTypeList()
        {
            var qry = Uow.FranchiseeContractTypeList.GetAll();
            return qry;
        }

        public FranchiseeContractTypeList GetFranchiseeContractTypeListById(int id)
        {
            return Uow.FranchiseeContractTypeList.GetById(id);
        }

        public FranchiseeContractTypeList SaveFranchiseeContractTypeList(FranchiseeContractTypeList FranchiseeContractTypeList)
        {
            var ID = FranchiseeContractTypeList.FranchiseeContractTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeContractTypeList.Add(FranchiseeContractTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeContractTypeList.Update(FranchiseeContractTypeList);
                Uow.Commit();
            }

            return FranchiseeContractTypeList;
        }

        public FranchiseeContractTypeList DeleteFranchiseeContractTypeList(int id)
        {
            var entity = Uow.FranchiseeContractTypeList.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FranchiseeContractTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion


        #region FranchiseeFee

        public IQueryable<FranchiseeFee> GetFranchiseeFee()
        {
            var qry = Uow.Fees.GetAll();
            return qry;
        }

        public FranchiseeFee GetFranchiseeFeeById(int id)
        {
            return Uow.Fees.GetById(id);
        }
        public FranchiseeFee_Temp GetFranchiseeFeeById_Temp(int id)
        {
            return Uow.FranchiseeFee_Temp.GetById(id);
        }

        public FranchiseeFee SaveFranchiseeFee(FranchiseeFee Fee)
        {
            var ID = Fee.FranchiseeFeeId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Fees.Add(Fee);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Fees.Update(Fee);
                Uow.Commit();
            }

            return Fee;
        }
        public FranchiseeFee_Temp SaveFranchiseeFee_Temp(FranchiseeFee_Temp Fee)
        {
            var ID = Fee.FranchiseeFeeId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeFee_Temp.Add(Fee);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeFee_Temp.Update(Fee);
                Uow.Commit();
            }

            return Fee;
        }

        public FranchiseeFee DeleteFranchiseeFee(int id)
        {
            var entity = Uow.Fees.GetById(id);

            entity.IsDelete = true;
            entity.IsActive = false;
            Uow.Fees.Update(entity);
            Uow.Commit();
            return entity;
        }
        public FranchiseeFee_Temp DeleteFranchiseeFee_Temp(int id)
        {
            var entity = Uow.FranchiseeFee_Temp.GetById(id);

            entity.IsDelete = true;
            entity.IsActive = false;
            Uow.FranchiseeFee_Temp.Update(entity);
            Uow.Commit();
            return entity;
        }
        public void AddDefaultFranchiseeFees(int FranchiseeId, int FeesId, decimal Amount)
        {

            var context = new jkDatabaseEntities();
            var data = context.FranchiseeFees.Where(w => w.FeesId == FeesId && w.FranchiseeId == FranchiseeId);
            var franchisee = context.Franchisees.Where(f => f.FranchiseeId == FranchiseeId).FirstOrDefault();
            var fee = context.Fees.Where(f => f.FeeId == FeesId).FirstOrDefault();

            if (franchisee == null || fee == null)
                return;

            // todo: update this when RegionConfiguration is changed for multiple regions

            var config = context.RegionConfigurations.Where(c => c.name == fee.Name).FirstOrDefault();

            if (config != null)
            {
                decimal decRes;
                Amount = decimal.TryParse(config.value, out decRes) ? decRes : Amount;
            }

            if (data == null || data.Count() == 0)
            {
                // Add Default
                FranchiseeFee FranchiseeFee = new FranchiseeFee();
                FranchiseeFee.FranchiseeId = FranchiseeId;
                FranchiseeFee.FeeRateType = 1; // Percentage
                FranchiseeFee.FeesId = FeesId;
                FranchiseeFee.Amount = Amount;
                FranchiseeFee.IsActive = true;
                FranchiseeFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                FranchiseeFee.CreatedDate = DateTime.Now;
                FranchiseeFee.IsDelete = false;
                SaveFranchiseeFee(FranchiseeFee);
            }
        }

        public void AddDefaultFranchiseeFees_Temp(int FranchiseeId, int FeesId, decimal Amount)
        {

            var context = new jkDatabaseEntities();
            var data = context.FranchiseeFee_Temp.Where(w => w.FeesId == FeesId && w.FranchiseeId == FranchiseeId);
            var franchisee = context.Franchisee_Temp.Where(f => f.FranchiseeId == FranchiseeId).FirstOrDefault();
            var fee = context.Fees.Where(f => f.FeeId == FeesId).FirstOrDefault();

            if (franchisee == null || fee == null)
                return;

            // todo: update this when RegionConfiguration is changed for multiple regions

            var config = context.RegionConfigurations.Where(c => c.name == fee.Name).FirstOrDefault();

            if (config != null)
            {
                decimal decRes;
                Amount = decimal.TryParse(config.value, out decRes) ? decRes : Amount;
            }

            if (data == null || data.Count() == 0)
            {
                // Add Default
                FranchiseeFee_Temp FranchiseeFee = new FranchiseeFee_Temp();
                FranchiseeFee.FranchiseeId = FranchiseeId;
                FranchiseeFee.FeeRateType = 1; // Percentage
                FranchiseeFee.FeesId = FeesId;
                FranchiseeFee.Amount = Amount;
                FranchiseeFee.IsActive = true;
                FranchiseeFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                FranchiseeFee.CreatedDate = DateTime.Now;
                FranchiseeFee.IsDelete = false;
                SaveFranchiseeFee_Temp(FranchiseeFee);
            }
        }

        public List<FeesViewModel> GetFeeListwithFranchiseeId(int FranchiseeId)
        {
            List<FeesViewModel> FranchiseeFeeViewModelList = new List<FeesViewModel>();
            var model = Uow.Fees.GetAll().Where(w => w.FranchiseeId == FranchiseeId);
            foreach (var item in model)
            {

                FeesViewModel modelFranchiseeFeeViewModel = new FeesViewModel();
                modelFranchiseeFeeViewModel = item.ToModel<FeesViewModel, FranchiseeFee>();
                FranchiseeFeeViewModelList.Add(modelFranchiseeFeeViewModel);
            }
            return FranchiseeFeeViewModelList;
        }

        public List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollection(int FranchiseeId)
        {
            //MstrFeeListCollection
            using (var context = new jkDatabaseEntities())
            {
                List<FeeFranchiseeFeeRateTypeListCollectionViewModel> FeeFranchiseeFeeRateTypeListCollectionViewModel = new List<FeeFranchiseeFeeRateTypeListCollectionViewModel>();

                var result = (from f in context.FranchiseeFees
                              join frl in context.FranchiseeFeeLists on f.FeesId equals frl.FranchiseeFeeListId
                              join frt in context.FeeRateTypeLists on f.FeeRateType equals frt.FeeRateTypeListId
                              select new MstrFeeListCollection
                              {
                                  Fee = f,
                                  FranchiseeFeeList = frl,
                                  FeeRateTypeList = frt
                              }).Where(w => w.Fee.FranchiseeId == FranchiseeId && w.Fee.IsDelete == false).ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FeeFranchiseeFeeRateTypeListCollectionViewModel feeFranchiseeFeeRateTypeListCollectionViewModel = new FeeFranchiseeFeeRateTypeListCollectionViewModel();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FeesList = item.Fee.ToModel<FeesViewModel, FranchiseeFee>();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FeeRateTypeList = item.FeeRateTypeList.ToModel<FeeRateTypeListViewModel, FeeRateTypeList>();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FranchiseeFeeList = item.FranchiseeFeeList.ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
                        FeeFranchiseeFeeRateTypeListCollectionViewModel.Add(feeFranchiseeFeeRateTypeListCollectionViewModel);
                    }
                }
                return FeeFranchiseeFeeRateTypeListCollectionViewModel;
            }
        }
        public List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollection_Temp(int FranchiseeId)
        {
            //MstrFeeListCollection
            using (var context = new jkDatabaseEntities())
            {
                List<FeeFranchiseeFeeRateTypeListCollectionViewModel> FeeFranchiseeFeeRateTypeListCollectionViewModel = new List<FeeFranchiseeFeeRateTypeListCollectionViewModel>();

                var result = (from f in context.FranchiseeFee_Temp
                              join frl in context.FranchiseeFeeLists on f.FeesId equals frl.FranchiseeFeeListId
                              join frt in context.FeeRateTypeLists on f.FeeRateType equals frt.FeeRateTypeListId
                              select new MstrFeeListCollection_Temp
                              {
                                  Fee_Temp = f,
                                  FranchiseeFeeList = frl,
                                  FeeRateTypeList = frt
                              }).Where(w => w.Fee_Temp.FranchiseeId == FranchiseeId && w.Fee_Temp.IsDelete == false).ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FeeFranchiseeFeeRateTypeListCollectionViewModel feeFranchiseeFeeRateTypeListCollectionViewModel = new FeeFranchiseeFeeRateTypeListCollectionViewModel();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FeesList = item.Fee_Temp.ToModel<FeesViewModel, FranchiseeFee_Temp>();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FeeRateTypeList = item.FeeRateTypeList.ToModel<FeeRateTypeListViewModel, FeeRateTypeList>();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FranchiseeFeeList = item.FranchiseeFeeList.ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
                        FeeFranchiseeFeeRateTypeListCollectionViewModel.Add(feeFranchiseeFeeRateTypeListCollectionViewModel);
                    }
                }
                return FeeFranchiseeFeeRateTypeListCollectionViewModel;
            }
        }

        public List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollectionAll(int FranchiseeId)
        {
            //MstrFeeListCollection
            using (var context = new jkDatabaseEntities())
            {
                List<FeeFranchiseeFeeRateTypeListCollectionViewModel> FeeFranchiseeFeeRateTypeListCollectionViewModel = new List<FeeFranchiseeFeeRateTypeListCollectionViewModel>();

                var result = (from f in context.FranchiseeFeeLists
                              join fr in context.FeeRateTypeLists on f.FeeRateTypeListId equals fr.FeeRateTypeListId
                              join rgn in context.RegionConfigurations on f.Name equals rgn.name
                              select new MstrFeeListCollection
                              {
                                  FranchiseeFeeList = f,
                                  FeeRateTypeList = fr
                              }).ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FeeFranchiseeFeeRateTypeListCollectionViewModel feeFranchiseeFeeRateTypeListCollectionViewModel = new FeeFranchiseeFeeRateTypeListCollectionViewModel();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FeeRateTypeList = item.FeeRateTypeList.ToModel<FeeRateTypeListViewModel, FeeRateTypeList>();
                        feeFranchiseeFeeRateTypeListCollectionViewModel.FranchiseeFeeList = item.FranchiseeFeeList.ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
                        FeeFranchiseeFeeRateTypeListCollectionViewModel.Add(feeFranchiseeFeeRateTypeListCollectionViewModel);
                    }
                }
                return FeeFranchiseeFeeRateTypeListCollectionViewModel;
            }
        }





        #endregion



        #region Fees

        public IQueryable<CusFee> GetFees()
        {
            var qry = Uow.CusFees.GetAll();
            return qry;
        }

        public CusFee GetFeesById(int id)
        {
            return Uow.CusFees.GetById(id);
        }

        public CusFee SaveFees(CusFee Fees)
        {
            var ID = Fees.FeesId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.CusFees.Add(Fees);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.CusFees.Update(Fees);
                Uow.Commit();
            }

            return Fees;
        }

        public CusFee DeleteFees(int id)
        {
            var entity = Uow.CusFees.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.CusFees.Update(entity);
            Uow.Commit();
            return entity;
        }


        public IQueryable<FeeConfiguration> GetFranchiseeFeeConfiguration()
        {
            var qry = Uow.FranchiseeFeeConfiguration.GetAll();
            return qry;
        }

        public IQueryable<FeeConfiguration_Temp> GetFranchiseeFeeConfiguration_Temp()
        {
            var qry = Uow.FeeConfiguration_Temp.GetAll();
            return qry;
        }

        public FeeConfiguration GetFranchiseeFeeConfigurationById(int FranchiseeId)
        {
            return Uow.FranchiseeFeeConfiguration.GetById(FranchiseeId);
        }



        #endregion




        #region FeeRate

        public IQueryable<FeeRate> GetFeeRate()
        {
            var qry = Uow.FeeRate.GetAll();
            return qry;
        }

        public FeeRate GetFeeRateById(int id)
        {
            return Uow.FeeRate.GetById(id);
        }

        public FeeRate SaveFeeRate(FeeRate FeeRate)
        {
            var ID = FeeRate.FeeRateId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FeeRate.Add(FeeRate);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FeeRate.Update(FeeRate);
                Uow.Commit();
            }

            return FeeRate;
        }

        public FeeRate DeleteFeeRate(int id)
        {
            var entity = Uow.FeeRate.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FeeRate.Update(entity);
            Uow.Commit();
            return entity;
        }


        #endregion


        #region FranchiseeFeeList 

        public IQueryable<FranchiseeFeeList> GetFranchiseeFeeList()
        {
            var qry = Uow.FranchiseeFeeList.GetAll();
            return qry;
        }

        public FranchiseeFeeList GetFranchiseeFeeListById(int id)
        {
            return Uow.FranchiseeFeeList.GetById(id);
        }

        public FranchiseeFeeList SaveFranchiseeFeeList(FranchiseeFeeList FranchiseeFeeList)
        {
            var ID = FranchiseeFeeList.FranchiseeFeeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FranchiseeFeeList.Add(FranchiseeFeeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FranchiseeFeeList.Update(FranchiseeFeeList);
                Uow.Commit();
            }

            return FranchiseeFeeList;
        }

        public FranchiseeFeeList DeleteFranchiseeFeeList(int id)
        {
            var entity = Uow.FranchiseeFeeList.GetById(id);
            entity.IsDelete = true;
            entity.IsActive = false;
            Uow.FranchiseeFeeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        #endregion

        #region FeeRateTypeList

        public IQueryable<FeeRateTypeList> GetFeeRateTypeList()
        {
            var qry = Uow.FeeRateTypeList.GetAll();
            return qry;
        }

        public FeeRateTypeList GetFeeRateTypeListById(int id)
        {
            return Uow.FeeRateTypeList.GetById(id);
        }

        public FeeRateTypeList SaveFeeRateTypeList(FeeRateTypeList FeeRateTypeList)
        {
            var ID = FeeRateTypeList.FeeRateTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.FeeRateTypeList.Add(FeeRateTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.FeeRateTypeList.Update(FeeRateTypeList);
                Uow.Commit();
            }

            return FeeRateTypeList;
        }

        public FeeRateTypeList DeleteFeeRateTypeList(int id)
        {
            var entity = Uow.FeeRateTypeList.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.FeeRateTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public List<FranchiseeFeeListFeeRateTypeListCollectionViewModel> GetFranchiseeFeeListFeeRateTypeListCollection()
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeFeeListFeeRateTypeListCollectionViewModel> FranchiseeFeeListFeeRateTypeListCollectionViewModel = new List<FranchiseeFeeListFeeRateTypeListCollectionViewModel>();
                var result = (from ffl in context.FranchiseeFeeLists
                              join frt in context.FeeRateTypeLists on ffl.FeeRateTypeListId equals frt.FeeRateTypeListId
                              select new MstrRegionCollection
                              {
                                  FranchiseeFeeList = ffl,
                                  FeeRateTypeList = frt
                              }).ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FranchiseeFeeListFeeRateTypeListCollectionViewModel franchiseeFeeListFeeRateTypeListCollectionViewModel = new FranchiseeFeeListFeeRateTypeListCollectionViewModel();
                        franchiseeFeeListFeeRateTypeListCollectionViewModel.FeeRateTypeList = item.FeeRateTypeList.ToModel<FeeRateTypeListViewModel, FeeRateTypeList>();
                        franchiseeFeeListFeeRateTypeListCollectionViewModel.FranchiseeFeeList = item.FranchiseeFeeList.ToModel<FranchiseeFeeListViewModel, FranchiseeFeeList>();
                        FranchiseeFeeListFeeRateTypeListCollectionViewModel.Add(franchiseeFeeListFeeRateTypeListCollectionViewModel);
                    }
                }
                return FranchiseeFeeListFeeRateTypeListCollectionViewModel;
            }
        }

        public List<FranchiseeOwner> GetFranchiseeOwnerById(int id, int TypeListId, int ContactTypeListId)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeOwner> FranchiseeOwnerViewModel = new List<FranchiseeOwner>();
                var result = (from fc in context.Contacts
                              where fc.ClassId == id && fc.TypeListId == TypeListId && fc.ContactTypeListId == ContactTypeListId
                              join fp in context.Phones on fc.ContactId equals fp.ClassId
                              where fp.TypeListId == TypeListId && fp.ContactTypeListId == ContactTypeListId

                              select new FranchiseeOwnerCollection
                              {
                                  OwnerInfo = fc,
                                  OwnerInfoPhone = fp
                              }).Distinct().ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FranchiseeOwner FranchiseeOwner = new FranchiseeOwner();
                        FranchiseeOwner.OwnerInfo = item.OwnerInfo.ToModel<ContactViewModel, Contact>();
                        FranchiseeOwner.OwnerInfoPhone = item.OwnerInfoPhone.ToModel<PhoneViewModel, Phone>();
                        FranchiseeOwnerViewModel.Add(FranchiseeOwner);
                    }
                }
                return FranchiseeOwnerViewModel;
            }
        }
        public List<FranchiseeOwner> GetFranchiseeOwnerByIdTemp(int id, int TypeListId, int ContactTypeListId)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeOwner> FranchiseeOwnerViewModel = new List<FranchiseeOwner>();
                var result = (from fc in context.Contact_Temp
                              where fc.ClassId == id && fc.TypeListId == TypeListId && fc.ContactTypeListId == ContactTypeListId
                              join fp in context.Phone_Temp on fc.ContactId equals fp.ClassId
                              where fp.TypeListId == TypeListId && fp.ContactTypeListId == ContactTypeListId

                              select new FranchiseeOwnerCollectionTemp
                              {
                                  OwnerInfo_Temp = fc,
                                  OwnerInfoPhone_Temp = fp
                              }).Distinct().ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FranchiseeOwner FranchiseeOwner = new FranchiseeOwner();
                        FranchiseeOwner.OwnerInfo = item.OwnerInfo_Temp.ToModel<ContactViewModel, Contact_Temp>();
                        FranchiseeOwner.OwnerInfoPhone = item.OwnerInfoPhone_Temp.ToModel<PhoneViewModel, Phone_Temp>();
                        FranchiseeOwnerViewModel.Add(FranchiseeOwner);
                    }
                }
                return FranchiseeOwnerViewModel;
            }
        }

        public List<FranchiseOwnersList> GetFranchiseeOwnerListById(int id)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerLists.Where(x => x.FranchiseeId == id).MapEnumerable<FranchiseOwnersList, FranchiseeOwnerList>().ToList();
            }
        }

        public List<FranchiseOwnersList> GetFranchiseeOwnerListByIdTemp(int id)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeOwnerList_Temp.Where(x => x.FranchiseeId == id).MapEnumerable<FranchiseOwnersList, FranchiseeOwnerList_Temp>().ToList();
            }
        }


        public IEnumerable<FullFranchiseeViewModel> GetFranchiseeDetailsByStatus(int StatusId, int TypeListId, int ContactTypeListId)
        {


            List<FranchiseeListViewModel> FranchiseeViewModelLst = new List<FranchiseeListViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("portal_Usp_GetFranchiseeList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                //if (ds.Tables[0].Rows.Count>0)
                //{
                //    FranchiseeViewModelLst = ds.Tables[0].AsEnumerable().Select(r => new FranchiseeViewModelList
                //    {
                //        Name = r.Field<string>("Name"),
                //        Age = r.Field<int>("Age")
                //    });
                //}
            }


            using (var context = new jkDatabaseEntities())
            {
                List<FullFranchiseeViewModel> FullFranchiseeViewModel = new List<FullFranchiseeViewModel>();
                var result = (from f in context.Franchisees
                              join fp in context.Phones on f.FranchiseeId equals fp.ClassId
                              into temp1
                              from t3 in temp1.DefaultIfEmpty()
                              join fa in context.Addresses.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on f.FranchiseeId equals fa.ClassId
                              into temp2
                              from t4 in temp2.DefaultIfEmpty()
                              join fe in context.Emails.Where(one => one.TypeListId == TypeListId && one.ContactTypeListId == ContactTypeListId) on f.FranchiseeId equals fe.ClassId
                              into temp3
                              from t5 in temp3.DefaultIfEmpty()
                              join sl in context.Status.Where(one => one.TypeListId == TypeListId) on f.FranchiseeId equals sl.ClassId
                              into temp4
                              from t6 in temp4.DefaultIfEmpty()
                              select new FranchiseeCollection
                              {
                                  BusinessInfo = f,
                                  BusinessInfoPhone = t3,
                                  BusinessInfoAddress = t4,
                                  BusinessInfoEmail = t5,
                                  BusinessInfoStatus = t6

                              }).OrderByDescending(one => one.BusinessInfo.FranchiseeId).DistinctBy(one => one.BusinessInfo).ToList();
                if (result != null && result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        FullFranchiseeViewModel fullFranchiseeViewModel = new FullFranchiseeViewModel();
                        fullFranchiseeViewModel.BusinessInfo = item.BusinessInfo.ToModel<FranchiseeViewModel, JKApi.Data.DAL.Franchisee>();
                        fullFranchiseeViewModel.BusinessInfoPhone = item.BusinessInfoPhone.ToModel<PhoneViewModel, Phone>();
                        fullFranchiseeViewModel.BusinessInfoEmail = item.BusinessInfoEmail.ToModel<EmailViewModel, Email>();
                        fullFranchiseeViewModel.BusinessInfoStatus = item.BusinessInfoStatus.ToModel<StatusViewModel, Status>();
                        fullFranchiseeViewModel.BusinessInfoAddress = item.BusinessInfoAddress.ToModel<AddressViewModel, Address>();
                        FullFranchiseeViewModel.Add(fullFranchiseeViewModel);
                    }
                }
                return FullFranchiseeViewModel;
            }


        }

        public IEnumerable<FranchiseeListViewModel> GetFranchiseeList(string status = null, int? regionId = null)
        {
            List<FranchiseeListViewModel> FranchiseeViewModelLst = new List<FranchiseeListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId ?? SelectedRegionId);
                parmas.Add("@StatusIds", status);
                using (var result = conn.QueryMultiple("dbo.portal_spGet_F_GetFranchiseeList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (result != null)
                    {
                        FranchiseeViewModelLst = result.Read<FranchiseeListViewModel>().ToList();
                    }
                }
            }
            return FranchiseeViewModelLst;

           
            //using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            //{
            //    con.Open();
            //    SqlCommand cmd = new SqlCommand("portal_spGet_F_GetFranchiseeList", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.Add(new SqlParameter("@RegionId", regionId ?? SelectedRegionId));
            //    cmd.Parameters.Add(new SqlParameter("@StatusIds", status));
            //    SqlDataAdapter adap = new SqlDataAdapter(cmd);
            //    DataSet ds = new DataSet();
            //    adap.Fill(ds);
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        FranchiseeViewModelLst = ds.Tables[0].AsEnumerable().Select(r => new FranchiseeListViewModel
            //        {

            //            FranchiseeId = r.Field<int>("FranchiseeId"),
            //            FranchiseeNo = r.Field<string>("FranchiseeNo"),
            //            Name = r.Field<string>("Name"),
            //            Address1 = string.Concat(r.Field<string>("Address1") + ", " + r.Field<string>("City") + ", " + r.Field<string>("StateName") + ", " + r.Field<string>("PostalCode")),
            //            Phone = r.Field<string>("Phone"),
            //            DistributionAmount = r.Field<decimal>("DistributionAmount"),
            //            RegionName = r.Field<string>("RegionName"),
            //            StatusListId = r.Field<int?>("StatusListId"),
            //            StatusName = r.Field<string>("StatusName"),
            //            IsTemp = r.Field<int>("IsTemp")
            //        }).ToList();
            //    }
            //}
            //return FranchiseeViewModelLst;
        }

        public IEnumerable<FranchiseeListViewModel> GetSearchFranchiseeList(string s, string sdt, string edt, int ptId, string statusIds, string regionId)
        {
            List<FranchiseeListViewModel> FranchiseeViewModelLst = new List<FranchiseeListViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("portal_spGet_F_GetSearchFranchiseeList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@regionIds", (regionId != "" ? regionId : SelectedRegionId.ToString())));
                cmd.Parameters.Add(new SqlParameter("@statusIds", statusIds));
                cmd.Parameters.Add(new SqlParameter("@search", s));
                cmd.Parameters.Add(new SqlParameter("@startDate", sdt));
                cmd.Parameters.Add(new SqlParameter("@endDate", edt));
                cmd.Parameters.Add(new SqlParameter("@planId", ptId));
                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    FranchiseeViewModelLst = ds.Tables[0].AsEnumerable().Select(r => new FranchiseeListViewModel
                    {
                        RowNo = r.Field<int>("RowNo"),
                        FranchiseeId = r.Field<int>("FranchiseeId"),
                        FranchiseeNo = r.Field<string>("FranchiseeNo"),
                        Name = r.Field<string>("Name"),
                        Address1 = string.Concat(r.Field<string>("Address1") + ", " + r.Field<string>("City") + ", " + r.Field<string>("StateName") + ", " + r.Field<string>("PostalCode")),
                        Phone = r.Field<string>("Phone"),
                        DistributionAmount = r.Field<decimal>("DistributionAmount"),
                        RegionName = r.Field<string>("RegionName"),
                        StatusListId = r.Field<int?>("StatusListId"),
                        StatusName = r.Field<string>("StatusName"),

                    }).ToList();
                }
            }
            return FranchiseeViewModelLst;
        }

        public IEnumerable<FranchiseeListViewModel> GetFranchiseeListData(int RegionId)
        {


            List<FranchiseeListViewModel> FranchiseeViewModelLst = new List<FranchiseeListViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("portal_Usp_GetFranchiseeList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@RegionId", RegionId));
                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    FranchiseeViewModelLst = ds.Tables[0].AsEnumerable().Select(r => new FranchiseeListViewModel
                    {

                        FranchiseeId = r.Field<int>("FranchiseeId"),
                        FranchiseeNo = r.Field<string>("FranchiseeNo"),
                        Name = r.Field<string>("Name"),
                        Address1 = string.Concat(r.Field<string>("Address1") + ", " + r.Field<string>("City") + ", " + r.Field<string>("StateName") + ", " + r.Field<string>("PostalCode")),
                        Phone = r.Field<string>("Phone"),
                        DistributionAmount = r.Field<decimal>("DistributionAmount"),
                        RegionName = r.Field<string>("RegionName")
                    }).ToList();
                }
            }
            return FranchiseeViewModelLst;
        }


        public List<FindersFeeScheduleViewModel> GetFindersFeeScheduleListData()
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FindersFeeSchedules.MapEnumerable<FindersFeeScheduleViewModel, FindersFeeSchedule>().ToList();
            }
        }

        public FranchiseeDetailViewModel GetFranchiseeDetail(int FranchiseeId)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.vw_F_FranchiseeDetail.Where(f => f.FranchiseeId == FranchiseeId).MapEnumerable<FranchiseeDetailViewModel, vw_F_FranchiseeDetail>().ToList().FirstOrDefault();
            }
        }
        public FranchiseeDetailViewModel GetFranchiseeDetailTemp(int FranchiseeId)
        {
            FranchiseeDetailViewModel FranchiseeDetailViewModel = new FranchiseeDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", FranchiseeId);
                using (var result = conn.QueryMultiple("dbo.portal_spGet_F_GetFranchiseeTempList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (result != null)
                    {
                        FranchiseeDetailViewModel = result.Read<FranchiseeDetailViewModel>().FirstOrDefault();
                    }
                }
            }
            return FranchiseeDetailViewModel;
        }
        public TaxRateViewModel GetTaxRateDetail(int ClassId, int TypeListId, int AddressId = 0)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.TaxRates.Where(f => f.classId == ClassId && f.TypeListId == 2).MapEnumerable<TaxRateViewModel, TaxRate>().ToList().FirstOrDefault();
            }
        }

        public TaxRateViewModel GetTaxRateDetailForFranchiseeSupply(int RegionId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var taxId = context.RegionSettings.Where(x => x.RegionId == RegionId && x.RegionConfigurationId == 43).ToList();
                if (taxId != null && taxId.Count > 0)
                {
                    int TaxRateId = Convert.ToInt32(taxId[0].Value);
                    return context.TaxRates.Where(f => f.TaxRateId == TaxRateId).MapEnumerable<TaxRateViewModel, TaxRate>().ToList().FirstOrDefault();
                }
                return new TaxRateViewModel();
            }
        }

        public portal_spGet_AR_FranchiseeDetail_Result GetFranchiseeDetailData(int FranchiseeId)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.portal_spGet_AR_FranchiseeDetail(FranchiseeId).FirstOrDefault();
            }
        }

        public portal_spGet_AR_FranchiseeSupplybyId_Result GetFranchiseebySupplyId(int Id, int RegionId)
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.portal_spGet_AR_FranchiseeSupplybyId(Id, RegionId).FirstOrDefault();
            }
        }

        //public List<ServiceTypeList> GetServiceTypeList()
        //{
        //    using (var context = new jkDatabaseEntities())
        //    {
        //        return context.ServiceTypeLists.ToList();
        //    }
        //}

        public List<ServiceTypeList> GetServiceTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {

                if (!String.IsNullOrEmpty(ClaimView.GetCLAIM_PERSON_INFORMATION().RoleId.ToString()))
                {
                    int roleId = ClaimView.GetCLAIM_PERSON_INFORMATION().Roles.FirstOrDefault().RoleId;
                    var result = (from sl in context.ServiceTypeLists
                                  join fp in context.RoleServiceTypeLists on sl.ServiceTypeListid equals fp.ServiceTypeListid
                                  where fp.RoleId == roleId && fp.IsShow == true
                                  select sl).ToList<ServiceTypeList>();

                    return result;
                }
                else
                {
                    return new List<ServiceTypeList>();
                }



                //return context.ServiceTypeLists.ToList();
            }
        }

        public List<FranchiseeTransactionTypeList> GetFranchiseeTransactionTypeList()
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.FranchiseeTransactionTypeLists.ToList();
            }
        }

        public List<FranchiseeManualTrxCreditReasonList> GetAll_ReasonList()
        {
            List<FranchiseeManualTrxCreditReasonList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.FranchiseeManualTrxCreditReasonLists.ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        public List<StatusList> GetStatusList()
        {
            using (var context = new jkDatabaseEntities())
            {
                return context.StatusLists.ToList();
            }
        }

        public bool CreateFranchiseeManualTrasactionSave(FranchiseeTransactionViewModel inputData)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeManualTransactionTemp oFranchiseeTransaction = new FranchiseeManualTransactionTemp();

                oFranchiseeTransaction.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                oFranchiseeTransaction.CreatedDate = DateTime.Now;
                oFranchiseeTransaction.Description = inputData.Description;
                oFranchiseeTransaction.FranchiseeId = inputData.FranchiseeId;
                oFranchiseeTransaction.ServiceTypeListId = inputData.ServiceTypeListId;
                oFranchiseeTransaction.GrossTotal = inputData.GrossTotal;
                oFranchiseeTransaction.ItemAmount = inputData.ItemAmount;
                oFranchiseeTransaction.MasterTrxTypeListId = inputData.MasterTrxTypeListId;
                oFranchiseeTransaction.FranchiseeManualTrxCreditReasonListId = inputData.FranchiseeManualTrxCreditReasonListId;
                oFranchiseeTransaction.NumOfPayments = inputData.NumOfPayments;
                oFranchiseeTransaction.PaymentsBilled = inputData.PaymentsBilled;
                oFranchiseeTransaction.Quantity = inputData.Quantity;
                oFranchiseeTransaction.RegionId = SelectedRegionId;
                oFranchiseeTransaction.StartDate = inputData.StartDate;
                oFranchiseeTransaction.Subtotal = inputData.Subtotal;
                oFranchiseeTransaction.Tax = inputData.Tax;
                oFranchiseeTransaction.Total = inputData.Total;
                oFranchiseeTransaction.TransactionDate = inputData.TransactionDate;
                oFranchiseeTransaction.TransactionStatusListId = inputData.StatusListId;
                oFranchiseeTransaction.IsActive = true;
                oFranchiseeTransaction.FranchsieeTrxTypeListId = inputData.FranchsieeTrxTypeListId;
                if (oFranchiseeTransaction.FranchsieeTrxTypeListId == 3)
                {
                    oFranchiseeTransaction.PaymentNo = inputData.PaymentNo;
                }
                oFranchiseeTransaction.ReSell = inputData.ReSell;
                oFranchiseeTransaction.VendorCode = inputData.VendorCode;
                oFranchiseeTransaction.VendorInvoiceNumber = inputData.VendorInvoiceNumber;
                oFranchiseeTransaction.VendorInvoiceDate = inputData.VendorInvoiceDate;
                oFranchiseeTransaction.IsCredit = inputData.IsCredit;
                oFranchiseeTransaction.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();
                context.FranchiseeManualTransactionTemps.Add(oFranchiseeTransaction);
                context.SaveChanges();


                return true;
            }
        }

        public bool UpdateFranchiseeManualTrasaction(ManualTransactionViewModel inputData)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeManualTransaction _FMT = context.FranchiseeManualTransactions.Where(x => x.FranchiseeManualTransactionId == inputData.FranchiseeManualTransactionId).FirstOrDefault();
                if (_FMT != null)
                {
                    _FMT.ServiceTypeListId = inputData.ServiceTypeListId;
                    //_FMT.TransactionDate = inputData.TransactionDate;
                    _FMT.ReSell = inputData.ReSell;
                    _FMT.VendorId = inputData.VendorId;
                    _FMT.VendorInvoiceNumber = inputData.VendorInvoiceNumber;
                    _FMT.VendorInvoiceDate = inputData.VendorInvoiceDate;
                    context.SaveChanges();
                }

                MasterTrxDetail _MTD = context.MasterTrxDetails.Where(o => o.MasterTrxDetailId == inputData.MastertrxDetailId).FirstOrDefault();
                if (_MTD != null)
                {
                    _MTD.Quantity = inputData.Quantity;
                    _MTD.TotalTax = inputData.TotalTax;
                    _MTD.Total = inputData.Total;
                    _MTD.UnitPrice = inputData.DistributionAmount;
                    _MTD.ExtendedPrice = inputData.ExtendedPrice;
                    _MTD.DetailDescription = inputData.DetailDescription;
                    context.SaveChanges();
                }

                return true;
            }
        }





        public bool SaveFranchiseeManualTrasactionForEdit(FranchiseeTransactionViewModel inputData, bool IsApprove)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeManualTransactionTemp oFranchiseeTransaction = context.FranchiseeManualTransactionTemps.SingleOrDefault(o => o.FranchiseeManualTransactionTempId == inputData.FranchiseeManualTransactionTempId);
                if (oFranchiseeTransaction != null)
                {
                    oFranchiseeTransaction.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    oFranchiseeTransaction.CreatedDate = DateTime.Now;
                    oFranchiseeTransaction.Description = inputData.Description;
                    oFranchiseeTransaction.FranchiseeId = inputData.FranchiseeId;
                    oFranchiseeTransaction.ServiceTypeListId = inputData.ServiceTypeListId;
                    oFranchiseeTransaction.GrossTotal = inputData.GrossTotal;
                    oFranchiseeTransaction.ItemAmount = inputData.ItemAmount;
                    if (inputData.FranchsieeTrxTypeListId == 3)
                    {
                        oFranchiseeTransaction.MasterTrxTypeListId = 62;
                    }
                    else
                    {
                        oFranchiseeTransaction.MasterTrxTypeListId = inputData.MasterTrxTypeListId;
                    }
                    oFranchiseeTransaction.FranchiseeManualTrxCreditReasonListId = inputData.FranchiseeManualTrxCreditReasonListId;
                    oFranchiseeTransaction.NumOfPayments = inputData.NumOfPayments;
                    oFranchiseeTransaction.PaymentsBilled = inputData.PaymentsBilled;
                    oFranchiseeTransaction.Quantity = inputData.Quantity;
                    oFranchiseeTransaction.RegionId = SelectedRegionId;
                    oFranchiseeTransaction.StartDate = inputData.StartDate;
                    oFranchiseeTransaction.Subtotal = inputData.Subtotal;
                    oFranchiseeTransaction.Tax = inputData.Tax;
                    oFranchiseeTransaction.Total = inputData.Total;
                    oFranchiseeTransaction.TransactionDate = inputData.TransactionDate;
                    oFranchiseeTransaction.TransactionStatusListId = inputData.StatusListId;
                    oFranchiseeTransaction.IsActive = true;
                    oFranchiseeTransaction.ReSell = inputData.ReSell;
                    oFranchiseeTransaction.VendorId = inputData.VendorId;
                    oFranchiseeTransaction.VendorInvoiceNumber = inputData.VendorInvoiceNumber;
                    oFranchiseeTransaction.VendorInvoiceDate = inputData.VendorInvoiceDate;
                    oFranchiseeTransaction.IsCredit = inputData.IsCredit;
                    if (inputData.FranchsieeTrxTypeListId == 1)
                    {
                        oFranchiseeTransaction.PaymentNo = "";
                    }
                    if (inputData.FranchsieeTrxTypeListId == 3)
                    {
                        oFranchiseeTransaction.PaymentNo = inputData.PaymentNo;
                    }
                    //if (oFranchiseeTransaction.FranchsieeTrxTypeListId == 3)
                    //{
                    //    oFranchiseeTransaction.PaymentNo = inputData.PaymentNo;
                    //}
                    context.SaveChanges();

                    if (IsApprove)
                    {
                        if (oFranchiseeTransaction.FranchsieeTrxTypeListId == 3)
                        {
                            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                            {
                                var parmas = new DynamicParameters();
                                parmas.Add("@FranchiseeTransactionId", oFranchiseeTransaction.FranchiseeManualTransactionTempId);
                                parmas.Add("@RegionId", 0);
                                parmas.Add("@CreatedBy", int.Parse(ClaimView.GetCLAIM_USERID()));
                                parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_FirenchiseeManualTrxBillGenerate", parmas, commandType: CommandType.StoredProcedure))
                                {
                                    oFranchiseeTransaction.IsActive = false;
                                    context.SaveChanges();
                                }

                            }
                        }
                        else
                        {
                            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                            {
                                var parmas = new DynamicParameters();
                                parmas.Add("@FranchiseeTransactionId", oFranchiseeTransaction.FranchiseeManualTransactionTempId);
                                parmas.Add("@RegionId", 0);
                                parmas.Add("@CreatedBy", int.Parse(ClaimView.GetCLAIM_USERID()));
                                parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_FirenchiseeManualBillGenerate", parmas, commandType: CommandType.StoredProcedure))
                                {
                                    oFranchiseeTransaction.IsActive = false;
                                    context.SaveChanges();
                                }

                            }
                        }
                    }


                }
                return true;
            }
        }

        public bool SaveFranchiseeChargeBackCredit(FranchiseeChargebackCreditViewModel inputData, bool IsApprove)
        {
            using (var context = new jkDatabaseEntities())
            {
                int _Month = Convert.ToDateTime(inputData.TransactionDate).Month;
                int _Year = Convert.ToDateTime(inputData.TransactionDate).Year;
                Period period = context.Periods.SingleOrDefault(p => p.BillMonth == _Month && p.BillYear == _Year);

                Franchisee oFranchisee = context.Franchisees.SingleOrDefault(o => o.FranchiseeId == inputData.FranchiseeId);
                inputData.RegionId = oFranchisee.RegionId;


                var CBCCount = context.Chargebacks.Where(o => o.BillingPayId == inputData.BillingPayId).Count();
                string _LastChar = "";
                if (CBCCount > 0)
                    _LastChar = ((char)(65 + CBCCount - 1)).ToString();

                Chargeback oChargeback = new Chargeback();
                oChargeback.BillingPayId = inputData.BillingPayId;
                oChargeback.CreatedBy = LoginUserId;
                oChargeback.CreatedDate = DateTime.Now;
                oChargeback.FranchiseeId = inputData.FranchiseeId;
                oChargeback.PeriodId = period.PeriodId;
                oChargeback.RegionId = inputData.RegionId;
                oChargeback.TransactionDate = inputData.TransactionDate;
                oChargeback.TransactionNumber = "CBC" + inputData.InvoiceNo.Trim() + _LastChar;
                oChargeback.TransactionStatusListId = 3;
                oChargeback.ServiceTypeListId = inputData.ServiceTypeListId;
                context.Chargebacks.Add(oChargeback);
                context.SaveChanges();


                MasterTrx masterTrx = new MasterTrx();
                masterTrx.BillMonth = period.BillMonth;
                masterTrx.BillYear = period.BillYear;
                masterTrx.ClassId = inputData.FranchiseeId;
                masterTrx.CreatedBy = LoginUserId;
                masterTrx.CreatedDate = DateTime.Now;
                masterTrx.HeaderId = oChargeback.ChargebackId;
                masterTrx.MasterTrxTypeListId = 10;
                masterTrx.PeriodId = period.PeriodId;
                masterTrx.RegionId = inputData.RegionId;
                masterTrx.StatusId = 3;
                masterTrx.TrxDate = inputData.TransactionDate;
                masterTrx.TypeListId = 2;
                context.MasterTrxes.Add(masterTrx);
                context.SaveChanges();

                oChargeback.MasterTrxId = masterTrx.MasterTrxId;
                context.SaveChanges();

                MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                masterTrxDetail.AmountTypeListId = 1;
                masterTrxDetail.ClassId = inputData.FranchiseeId;
                masterTrxDetail.CreatedBy = LoginUserId;
                masterTrxDetail.CreatedDate = DateTime.Now;
                masterTrxDetail.DetailDescription = inputData.Description;
                masterTrxDetail.FeesDetail = true;
                masterTrxDetail.FRDeduction = false;
                masterTrxDetail.FRRevenues = false;
                masterTrxDetail.HeaderId = oChargeback.ChargebackId;
                masterTrxDetail.InvoiceId = inputData.InvoiceId;
                masterTrxDetail.IsDelete = false;
                masterTrxDetail.LineNo = 1;
                masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                masterTrxDetail.MasterTrxTypeListId = 10;
                masterTrxDetail.PeriodId = period.PeriodId;
                masterTrxDetail.Quantity = 1;
                masterTrxDetail.RegionId = inputData.RegionId;
                masterTrxDetail.ServiceTypeListId = inputData.ServiceTypeListId;
                masterTrxDetail.SourceId = inputData.ChargebackId;
                masterTrxDetail.SourceTypeListId = 5;
                masterTrxDetail.FRDeduction = true;
                masterTrxDetail.FRRevenues = false;
                masterTrxDetail.TaxDetail = false;
                masterTrxDetail.Total = inputData.Total;
                masterTrxDetail.TotalFee = inputData.Fee;
                masterTrxDetail.TotalTax = 0;
                masterTrxDetail.Transactiondate = inputData.TransactionDate;
                masterTrxDetail.TypelistId = 2;
                masterTrxDetail.UnitPrice = inputData.Amount;
                masterTrxDetail.ExtendedPrice = inputData.Amount;
                context.MasterTrxDetails.Add(masterTrxDetail);
                context.SaveChanges();

                List<FranchiseeFee> franchiseeFees = context.FranchiseeFees.Where(fr => fr.FranchiseeId == inputData.FranchiseeId).ToList();

                /*Commented 6-21-2018 - do not insert fees in MasterTrxFeeDetail*/
                /*
                MasterTrxFeeDetail masterTrxFeeDetail;
                decimal _amount = (decimal)inputData.Amount;
                foreach (FranchiseeFee o in franchiseeFees)
                {                    
                    masterTrxFeeDetail = new MasterTrxFeeDetail();
                    if (o.FeeRateType == 1)
                        masterTrxFeeDetail.Amount = _amount * o.Amount / 100;
                    else
                        masterTrxFeeDetail.Amount = o.Amount;
                    masterTrxFeeDetail.AmountTypeListId = 2;
                    masterTrxFeeDetail.BillingPayId = inputData.BillingPayId;
                    masterTrxFeeDetail.CreatedBy = LoginUserId;
                    masterTrxFeeDetail.CreatedDate = DateTime.Now;
                    masterTrxFeeDetail.FeeId = o.FeesId;
                    masterTrxFeeDetail.FeePercentage = o.Amount;
                    masterTrxFeeDetail.FranchiseeId = o.FranchiseeId;
                    masterTrxFeeDetail.FRDeduction = false;
                    masterTrxFeeDetail.FRRevenues = false;
                    masterTrxFeeDetail.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                    masterTrxFeeDetail.PeriodId = period.PeriodId;
                    masterTrxFeeDetail.RegionId = inputData.RegionId;
                    context.MasterTrxFeeDetails.Add(masterTrxFeeDetail);
                }
                */
                context.SaveChanges();



                //    if (IsApprove)
                //    {
                //        if (oFranchiseeTransaction.FranchsieeTrxTypeListId == 3)
                //        {
                //            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                //            {
                //                var parmas = new DynamicParameters();
                //                parmas.Add("@FranchiseeTransactionId", oFranchiseeTransaction.FranchiseeManualTransactionTempId);
                //                parmas.Add("@RegionId", 0);
                //                parmas.Add("@CreatedBy", int.Parse(ClaimView.GetCLAIM_USERID()));
                //                parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_FirenchiseeManualTrxBillGenerate", parmas, commandType: CommandType.StoredProcedure))
                //                {
                //                    oFranchiseeTransaction.IsActive = false;
                //                    context.SaveChanges();
                //                }

                //            }
                //        }
                //        else
                //        {
                //            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                //            {
                //                var parmas = new DynamicParameters();
                //                parmas.Add("@FranchiseeTransactionId", oFranchiseeTransaction.FranchiseeManualTransactionTempId);
                //                parmas.Add("@RegionId", 0);
                //                parmas.Add("@CreatedBy", int.Parse(ClaimView.GetCLAIM_USERID()));
                //                parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_FirenchiseeManualBillGenerate", parmas, commandType: CommandType.StoredProcedure))
                //                {
                //                    oFranchiseeTransaction.IsActive = false;
                //                    context.SaveChanges();
                //                }

                //            }
                //        }
                //    }



                return true;
            }
        }

        public bool SaveFranchiseeManualTrasactionForApproved(string Id, bool IsApproved, string note)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeManualTransactionTemp oFranchiseeTransaction = context.FranchiseeManualTransactionTemps.Where(o => o.FranchiseeManualTransactionTempId.ToString() == Id).FirstOrDefault();
                if (oFranchiseeTransaction != null)
                {
                    if (IsApproved)
                    {
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@FranchiseeTransactionId", oFranchiseeTransaction.FranchiseeManualTransactionTempId);
                            parmas.Add("@RegionId", 0);
                            parmas.Add("@CreatedBy", int.Parse(ClaimView.GetCLAIM_USERID()));
                            parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                            using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_FirenchiseeManualBillGenerate", parmas, commandType: CommandType.StoredProcedure))
                            {
                                oFranchiseeTransaction.IsActive = false;
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        oFranchiseeTransaction.IsActive = false;
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        public FranchiseeTransactionViewModel GetFranchiseeManualTrasactionForEdit(int Id)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeTransactionViewModel oFranchiseeTransaction = context.FranchiseeManualTransactionTemps.Where(o => o.FranchiseeManualTransactionTempId == Id).MapEnumerable<FranchiseeTransactionViewModel, FranchiseeManualTransactionTemp>().ToList().FirstOrDefault();
                return oFranchiseeTransaction;
            }
        }

        public bool GetFranchiseeManualTrasactionForDelete(int Id)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeManualTransactionTemp oFranchiseeTransaction = context.FranchiseeManualTransactionTemps.SingleOrDefault(o => o.FranchiseeManualTransactionTempId == Id);
                if (oFranchiseeTransaction != null)
                {
                    oFranchiseeTransaction.IsActive = false;
                    oFranchiseeTransaction.TransactionStatusListId = 13;
                    context.SaveChanges();
                    return true;
                }
                return false;

            }
        }

        public List<VendorViewModel> GetVendorList(int RegionId)
        {

            List<VendorViewModel> lstVendor = new List<VendorViewModel>();
            bool isLocal = HttpContext.Current.Request.IsLocal;
            string curURL = HttpContext.Current.Request.Url.AbsoluteUri;

            if (isLocal || curURL.Contains("http://fms.ecom.bid"))
            {

                using (var context = new jkDatabaseEntities())
                {
                    lstVendor = context.Vendors.MapEnumerable<VendorViewModel, Vendor>().ToList();
                    return lstVendor;
                }
            }
            else
            {
                lstVendor = TRVGetVendorList(RegionId);
            }

            return lstVendor;

        }

        public List<VendorViewModel> TRVGetVendorList(int RegionId)
        {
            JKApi.Data.DAL.Region region = new Data.DAL.Region();

            using (var context = new jkDatabaseEntities())
            {
                region = context.Regions.SingleOrDefault(r => r.RegionId == RegionId);
            }


            string connectionString = ConfigurationManager.ConnectionStrings["TrvDBConnection"].ConnectionString;

            connectionString = connectionString.Replace("{CATALOG}", region.TRVDBName);

            List<VendorViewModel> lstVendor = new List<VendorViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                string CommandText = "Select vendorid, name from tblapvendor ";
                CommandText = CommandText + " WHERE [glacct] = @glacct ";
                CommandText = CommandText + " and [vendorholdyn] = @vendorholdyn ";

                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(CommandText, conn);
                da.SelectCommand.Parameters.AddWithValue("@glacct", "5201");
                da.SelectCommand.Parameters.AddWithValue("@vendorholdyn", 0);

                DataTable dataTable = new DataTable();
                da.Fill(dataTable);

                lstVendor.Add(new VendorViewModel
                {
                    Code = "PFO",
                    Name = "Purchase from Office"
                });

                if (dataTable != null)
                {
                    foreach (DataRow keyRow in dataTable.Rows)
                    {
                        lstVendor.Add(new VendorViewModel
                        {
                            Code = keyRow["vendorid"].ToString(),
                            Name = keyRow["name"].ToString()
                        });
                    }
                }

                conn.Close();


            }


            return lstVendor;

        }

        public List<FranchiseeFeeViewModel> GetFranchiseeFee(int franchiseeId)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeFeeViewModel> lstVendor = context.FranchiseeFees.Where(f => f.FranchiseeId == franchiseeId).MapEnumerable<FranchiseeFeeViewModel, FranchiseeFee>().ToList();
                return lstVendor;
            }
        }


        public List<StatusList> GetAll_StatusList()
        {

            return jkEntityModel.StatusLists.AsNoTracking().ToList();
        }

        public List<StatusReasonList> GetAll_StatusReasonList(int TypeListId)
        {
            return jkEntityModel.StatusReasonLists.AsNoTracking().Where(r => r.TypeListId == TypeListId).ToList();
        }

        public FranchiseeMaintenanceViewModel GetFranchiseeMaintenanceViewModelById(int id)
        {
            FranchiseeMaintenanceViewModel oFranchiseeMaintenanceViewModel = new FranchiseeMaintenanceViewModel();
            using (var context = new jkDatabaseEntities())
            {
                List<portal_spGet_FranchiseeMaintenanceList_Result> lstFranchiseeMaintenanceList = jkEntityModel.portal_spGet_FranchiseeMaintenanceList(id).ToList();
                if (lstFranchiseeMaintenanceList.Count > 0)
                {
                    portal_spGet_FranchiseeMaintenanceList_Result ob = lstFranchiseeMaintenanceList.FirstOrDefault();
                    oFranchiseeMaintenanceViewModel.FranchiseeId = ob.FranchiseeId;
                    oFranchiseeMaintenanceViewModel.FranchiseeName = ob.FranchiseeName;
                    oFranchiseeMaintenanceViewModel.FranchiseeNo = ob.FranchiseeNo;
                    oFranchiseeMaintenanceViewModel.StatusId = ob.StatusId;
                    oFranchiseeMaintenanceViewModel.StatusListId = ob.StatusListId;
                    oFranchiseeMaintenanceViewModel.StatusListName = ob.StatusListName;
                    oFranchiseeMaintenanceViewModel.ReasonListId = ob.ReasonListId;
                    oFranchiseeMaintenanceViewModel.ReasonListName = ob.ReasonListName;
                    oFranchiseeMaintenanceViewModel.StatusDate = ob.StatusDate;
                    oFranchiseeMaintenanceViewModel.StatusNotes = ob.StatusNotes;
                    oFranchiseeMaintenanceViewModel.ResumeDate = ob.ResumeDate;
                    oFranchiseeMaintenanceViewModel.LastServiceDate = ob.LastServiceDate;
                    oFranchiseeMaintenanceViewModel.IsActive = ob.IsActive;
                    oFranchiseeMaintenanceViewModel.CreatedBy = ob.CreatedBy;
                    oFranchiseeMaintenanceViewModel.CreatedDate = ob.CreatedDate;




                }

                return oFranchiseeMaintenanceViewModel;
            }
        }

        public EditFranchiseeMaintenanceViewModel GetFranchiseeMaintenanceTemp(int Id)
        {
            EditFranchiseeMaintenanceViewModel edit = new EditFranchiseeMaintenanceViewModel();
            jkEntityModel.Configuration.ProxyCreationEnabled = false;
            var data = jkEntityModel.MaintenanceTemps.Where(r => r.MaintenanceTempId == Id).FirstOrDefault();
            if (data != null)
            {
                edit.StatusListId = data.StatusListId;
                edit.ClassId = data.ClassId;
                edit.Reason = data.Reason;
                edit.ResumeDate = data.ResumeDate;
                edit.EffectiveDate = data.EffectiveDate;
                edit.LastServiceDate = data.LastServiceDate;
                edit.StatusReasonListId = data.StatusReasonListId;
                edit.Comments = data.Comments;
            }
            jkEntityModel.Configuration.ProxyCreationEnabled = true;
            return edit;
        }

        //public List<FranchiseeMaintenanceViewModel> GetFranchiseeMaintenanceViewModelById(int id)
        //{
        //    FranchiseeMaintenanceViewModel oFranchiseeMaintenanceViewModel = new FranchiseeMaintenanceViewModel();
        //    using (var context = new jkDatabaseEntities())
        //    {
        //        List<FranchiseeMaintenanceViewModel> lstFranchiseeSearch = new List<FranchiseeMaintenanceViewModel>();
        //        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
        //        {
        //            var parmas = new DynamicParameters();
        //            parmas.Add("@FranchiseeId", id);
        //            using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_FranchiseeMaintenanceList", parmas, commandType: CommandType.StoredProcedure))
        //            {
        //                if (multipleresult != null)
        //                {
        //                    lstFranchiseeSearch = multipleresult.Read<FranchiseeMaintenanceViewModel>().ToList();


        //                }
        //            }
        //        }
        //        return lstFranchiseeSearch;


        //    }
        //}

        #region :: Franchise Distribution :: 

        public FranchiseeDistributionDetailsModel GetFranchiseeDistributionDetails(int FranchiseeId)
        {
            using (var context = new jkDatabaseEntities())
            {
                FranchiseeDistributionDetailsModel FranchiseeDistributionModel = new FranchiseeDistributionDetailsModel();
                var FranchiseeData = context.Franchisees.Where(w => w.FranchiseeId == FranchiseeId).FirstOrDefault();
                FranchiseeDistributionModel.FranchiseeNo = (FranchiseeData != null ? FranchiseeData.FranchiseeNo : string.Empty);
                FranchiseeDistributionModel.FranchiseName = (FranchiseeData != null ? FranchiseeData.Name : string.Empty);
                var result = (from dst in context.Distributions
                              join cnt in context.ContractDetails on dst.ContractDetailId equals cnt.ContractDetailId
                              join ct in context.Customers on dst.CustomerId equals ct.CustomerId
                              join st in context.ServiceTypeLists on cnt.ServiceTypeListId equals st.ServiceTypeListid
                              where dst.FranchiseeId == FranchiseeId && dst.isActive == true
                              select new FranchiseeDistributionModel
                              {
                                  CustomerName = ct.Name,
                                  CustomerNo = ct.CustomerNo,
                                  ServiceType = st.name,
                                  DistributionAmount = (dst.Amount.HasValue ? dst.Amount.Value : 0),
                                  StartDate = dst.StartDate
                              }).Distinct().ToList();
                if (result != null && result.Count() > 0)
                {
                    FranchiseeDistributionModel.FranchiseeDistributionList = result;
                    FranchiseeDistributionModel.TotalDistribution = Convert.ToDecimal(result.Sum(d => d.DistributionAmount));
                }
                else
                {
                    FranchiseeDistributionModel.FranchiseeDistributionList = new List<FranchiseeDistributionModel>();
                    FranchiseeDistributionModel.TotalDistribution = 0;
                }
                FranchiseeDistributionModel.FranchiseeId = FranchiseeId;
                return FranchiseeDistributionModel;
            }
        }

        public List<portal_spGet_C_DistributionWithNoFinderFee_Result> GetCustomerDistributionWithNoFinderFee(int CustomerId)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<portal_spGet_C_DistributionWithNoFinderFee_Result> FranDistributionWithNoFinderFee = context.portal_spGet_C_DistributionWithNoFinderFee(CustomerId).ToList();
                return FranDistributionWithNoFinderFee;
            }



        }


        public CommonFranchiseeCustomerViewModel GetFranchiseeCustomerDistributionData(int CustomerId, int FranchiseeId, int ContractDetailDistributionLineNo = -1)
        {

            CommonFranchiseeCustomerViewModel oCommon = new CommonFranchiseeCustomerViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@CustomerId", CustomerId);
                parmas.Add("@FranchiseeId", FranchiseeId);
                parmas.Add("@ContractDetailDistributionLineNo", ContractDetailDistributionLineNo == 0 ? 1 : ContractDetailDistributionLineNo);
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

                        /*Franchise Distribution Should not empty and 
                         * Franchise Distribution list count should be greater
                         * then 0
                         */
                        if (lstFindersFee.Count > 0)
                        {
                            if (oCommon.FindersFee.StartDate == null)
                            {
                                oCommon.FindersFee.StartDate = oCommon.lstFranchiseeDistribution[0]?.StartDate;
                            }
                        }
                        if (oCommon.lstFranchiseeDistribution.Count > 0)
                            if (oCommon.lstFranchiseeDistribution[0]?.StartDate != null)
                                oCommon.CustomerDetail.EffectiveDate = oCommon.lstFranchiseeDistribution[0]?.StartDate;


                    }
                }
            }

            return oCommon;

        }

        public bool InsertFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData, int TypeListId)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {
                MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();


                if (InputData.CustomerDetail != null)
                {

                    bool isMaintenance = true;
                    if (InputData.CustomerDetail.StatusListId == 38 || InputData.CustomerDetail.StatusListId == 39 || InputData.CustomerDetail.StatusListId == 4 || InputData.CustomerDetail.StatusListId == 0)
                    {
                        isMaintenance = false;
                    }

                    if (isMaintenance)
                    {
                        oMaintenanceTemp = new MaintenanceTemp();
                        if (TypeListId == 1)
                        {
                            oMaintenanceTemp.ClassId = InputData.CustomerDetail.CustomerId;
                            oMaintenanceTemp.TypeListId = 1;
                            oMaintenanceTemp.StatusListId = 12;
                            oMaintenanceTemp.MaintenanceTypeListId = 7;
                        }
                        else
                        {
                            oMaintenanceTemp.ClassId = InputData.CustomerDetail.FranchiseeId;
                            oMaintenanceTemp.TypeListId = 2;
                            oMaintenanceTemp.StatusListId = 12;
                            oMaintenanceTemp.MaintenanceTypeListId = 6;
                        }
                        //oMaintenanceTemp.StatusReasonListId = InputData.CustomerDetail.StatusReasonListId;
                        oMaintenanceTemp.EffectiveDate = DateTime.Now;// InputData.CustomerDetail.EffectiveDate;
                        oMaintenanceTemp.IsActive = true;
                        oMaintenanceTemp.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        oMaintenanceTemp.CreatedDate = DateTime.Now;
                        oMaintenanceTemp.RegionId = InputData.CustomerDetail.RegionId;
                        oMaintenanceTemp.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());
                        context.MaintenanceTemps.Add(oMaintenanceTemp);
                        context.SaveChanges();


                        //Distribution
                        MaintenanceTempDetail oMaintenanceTempDetail_Distribution;
                        if (InputData.lstFranchiseeDistribution != null)
                        {
                            if (InputData.lstFranchiseeDistribution.Count > 1 || InputData.lstFranchiseeDistribution.Count == 1)
                            {
                                foreach (FCFranchiseeDistributionViewModel oFCFranchiseeDistributionViewModel in InputData.lstFranchiseeDistribution)
                                {

                                    oMaintenanceTempDetail_Distribution = new MaintenanceTempDetail();
                                    oMaintenanceTempDetail_Distribution.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                    oMaintenanceTempDetail_Distribution.MaintenanceDetailTypeListId = 3; //Contract Detail
                                    oMaintenanceTempDetail_Distribution.CustomerId = InputData.CustomerDetail.CustomerId;
                                    oMaintenanceTempDetail_Distribution.ContractId = InputData.CustomerDetail.ContractId;

                                    oMaintenanceTempDetail_Distribution.DistributionId = oFCFranchiseeDistributionViewModel.DistributionId;
                                    oMaintenanceTempDetail_Distribution.DContractDetailId = oFCFranchiseeDistributionViewModel.ContractDetailId;
                                    oMaintenanceTempDetail_Distribution.DFranchiseeId = oFCFranchiseeDistributionViewModel.FranchiseeId;
                                    oMaintenanceTempDetail_Distribution.DAmount = oFCFranchiseeDistributionViewModel.Amount;

                                    oMaintenanceTempDetail_Distribution.DFeeAmount = 0;
                                    oMaintenanceTempDetail_Distribution.DTotalAmount = oFCFranchiseeDistributionViewModel.Amount;
                                    oMaintenanceTempDetail_Distribution.DInvoiceId = 0;

                                    oMaintenanceTempDetail_Distribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_Distribution);
                                    context.SaveChanges();


                                    //Franchisee Distribution Fee
                                    MaintenanceTempDetail oMaintenanceTempDetail_FranchiseeDistributionFee;
                                    foreach (FCFranchiseeDistributionFeeViewModel oFindersFeeAdjustment in InputData.lstFranchiseeDistributionFee.Where(k => k.ContractDetailId == oFCFranchiseeDistributionViewModel.ContractDetailId && k.FranchiseeId == oFCFranchiseeDistributionViewModel.FranchiseeId))
                                    {

                                        oMaintenanceTempDetail_FranchiseeDistributionFee = new MaintenanceTempDetail();
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.MaintenanceDetailTypeListId = 4; //Contract Detail
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CustomerId = InputData.CustomerDetail.CustomerId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.ContractId = InputData.CustomerDetail.ContractId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFranchiseeId = oFindersFeeAdjustment.FranchiseeId;// oMaintenanceTempDetail_Distribution.DFranchiseeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DContractDetailId = oFindersFeeAdjustment.ContractDetailId;// oMaintenanceTempDetail_Distribution.DContractDetailId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.FeeId = oFindersFeeAdjustment.FeeId.ToString();// oMaintenanceTempDetail_Distribution.FeeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFFeeRateTypeListId = oFindersFeeAdjustment.FeeRateTypeListId;// oMaintenanceTempDetail_Distribution.DFFeeRateTypeListId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFAmount = oFindersFeeAdjustment.Amount;// oMaintenanceTempDetail_Distribution.DFAmount;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CreatedDate = DateTime.Now;
                                        context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FranchiseeDistributionFee);
                                        context.SaveChanges();
                                    }

                                }
                            }
                            else
                            {
                                if (InputData.FranchiseeDistribution != null)
                                {
                                    oMaintenanceTempDetail_Distribution = new MaintenanceTempDetail();
                                    oMaintenanceTempDetail_Distribution.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                    oMaintenanceTempDetail_Distribution.MaintenanceDetailTypeListId = 3; //Contract Detail
                                    oMaintenanceTempDetail_Distribution.CustomerId = InputData.CustomerDetail.CustomerId;
                                    oMaintenanceTempDetail_Distribution.ContractId = InputData.CustomerDetail.ContractId;

                                    oMaintenanceTempDetail_Distribution.DistributionId = InputData.FranchiseeDistribution.DistributionId;
                                    oMaintenanceTempDetail_Distribution.DContractDetailId = InputData.FranchiseeDistribution.ContractDetailId;
                                    oMaintenanceTempDetail_Distribution.DFranchiseeId = InputData.FranchiseeDistribution.FranchiseeId;
                                    oMaintenanceTempDetail_Distribution.DAmount = InputData.FranchiseeDistribution.Amount;

                                    oMaintenanceTempDetail_Distribution.DFeeAmount = 0;
                                    oMaintenanceTempDetail_Distribution.DTotalAmount = InputData.FranchiseeDistribution.Amount;
                                    oMaintenanceTempDetail_Distribution.DInvoiceId = 0;

                                    oMaintenanceTempDetail_Distribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_Distribution);
                                    context.SaveChanges();


                                    //Franchisee Distribution Fee
                                    MaintenanceTempDetail oMaintenanceTempDetail_FranchiseeDistributionFee;
                                    foreach (FCFranchiseeDistributionFeeViewModel oFindersFeeAdjustment in InputData.lstFranchiseeDistributionFee)
                                    {

                                        oMaintenanceTempDetail_FranchiseeDistributionFee = new MaintenanceTempDetail();
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.MaintenanceDetailTypeListId = 4; //Contract Detail
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CustomerId = InputData.CustomerDetail.CustomerId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.ContractId = InputData.CustomerDetail.ContractId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFranchiseeId = oFindersFeeAdjustment.FranchiseeId;// oMaintenanceTempDetail_Distribution.DFranchiseeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DContractDetailId = oFindersFeeAdjustment.ContractDetailId;// oMaintenanceTempDetail_Distribution.DContractDetailId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.FeeId = oFindersFeeAdjustment.FeeId.ToString();// oMaintenanceTempDetail_Distribution.FeeId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFFeeRateTypeListId = oFindersFeeAdjustment.FeeRateTypeListId;// oMaintenanceTempDetail_Distribution.DFFeeRateTypeListId;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.DFAmount = oFindersFeeAdjustment.Amount;// oMaintenanceTempDetail_Distribution.DFAmount;
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                        oMaintenanceTempDetail_FranchiseeDistributionFee.CreatedDate = DateTime.Now;
                                        context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FranchiseeDistributionFee);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }

                    }
                    //Finder Fee
                    MaintenanceTempDetail oMaintenanceTempDetail_FinderFee;
                    FindersFee finderFee;
                    Status status;

                    if (InputData.FindersFee.FindersFeeId == 0)
                    {
                        status = new Status();
                        status.ClassId = InputData.CustomerDetail.FranchiseeId;
                        status.StatusListId = 39;
                        status.TypeListId = 2;
                        context.Status.Add(status);
                        context.SaveChanges();

                        finderFee = new FindersFee();
                        finderFee.StatusId = status.StatusId;
                        finderFee.StatusListId = status.StatusListId;
                        finderFee.CustomerId = InputData.CustomerDetail.CustomerId;
                        finderFee.FranchiseeId = InputData.CustomerDetail.FranchiseeId;

                        int _disId = (InputData.FranchiseeDistribution != null ? InputData.FranchiseeDistribution.DistributionId : 0);
                        if (_disId == 0)
                        {
                            var disObject = context.Distributions.Where(d => d.CustomerId == InputData.CustomerDetail.CustomerId && d.FranchiseeId == InputData.CustomerDetail.FranchiseeId && d.isActive == true).FirstOrDefault();
                            _disId = disObject != null ? disObject.DistributionId : 0;
                        }

                        finderFee.DistributionId = _disId;
                        finderFee.FindersFeeId = InputData.FindersFee.FindersFeeId;
                        finderFee.StartDate = InputData.FindersFee.StartDate;
                        finderFee.ResumeDate = InputData.FindersFee.ResumeDate != null ? InputData.FindersFee.ResumeDate : InputData.FranchiseeDistribution.StartDate;
                        finderFee.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                        finderFee.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                        finderFee.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                        finderFee.Factor = InputData.FindersFee.Factor!=null? InputData.FindersFee.Factor:0;
                        finderFee.DownPayPercentage = InputData.FindersFee.DownPayPercentage;

                        finderFee.Interest = InputData.FindersFee.Interest!=null? InputData.FindersFee.Interest:0;
                        finderFee.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                        finderFee.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                        finderFee.Notes = InputData.FindersFee.Notes;
                        finderFee.FinancedAmount = InputData.FindersFee.FinancedAmount;
                        finderFee.DownPaymentAmount = InputData.FindersFee.DownPaymentAmount;

                        if (finderFee.FindersFeeId == 0 && finderFee.DownPaymentAmount > 0)
                        {
                            finderFee.DownPaymentPaid = false;
                            finderFee.NumOfPaymentsPaid = -1;
                        }

                        finderFee.TotalAmount = InputData.FindersFee.TotalAmount;
                        finderFee.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                        finderFee.PaidAmount = InputData.FindersFee.PaidAmount!=null? InputData.FindersFee.PaidAmount:0;
                        finderFee.BalanceAmount = InputData.FindersFee.BalanceAmount == 0 ? InputData.FindersFee.TotalAmount : InputData.FindersFee.BalanceAmount;
                        finderFee.PayableOnAmount = InputData.FindersFee.PayableOnAmount!=null? InputData.FindersFee.PayableOnAmount:0;
                        finderFee.Notes = !String.IsNullOrEmpty(InputData.FindersFee.Notes) ? InputData.FindersFee.Notes : "";
                        finderFee.InterestAmount = InputData.FindersFee.InterestAmount!=null? InputData.FindersFee.InterestAmount:0;
                        finderFee.MonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                        finderFee.IncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                        finderFee.Description = InputData.FindersFee.Description;
                        finderFee.RegionId = SelectedRegionId;
                        finderFee.ImpId = 0;
                        finderFee.FindersFeeNumber = "";
                        finderFee.BalanceAmount = 0;
                        finderFee.PayableOnAmount = 0;
                        finderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        finderFee.CreatedDate = DateTime.Now;
                        finderFee.ModifiedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        finderFee.ModifiedDate = DateTime.Now;
                        context.FindersFees.Add(finderFee);
                        context.SaveChanges();
                        if (isMaintenance)
                        {
                            oMaintenanceTempDetail_FinderFee = new MaintenanceTempDetail();
                            oMaintenanceTempDetail_FinderFee.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                            oMaintenanceTempDetail_FinderFee.MaintenanceDetailTypeListId = 5; //Contract Detail
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

                            oMaintenanceTempDetail_FinderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            oMaintenanceTempDetail_FinderFee.CreatedDate = DateTime.Now;
                            context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFee);
                            context.SaveChanges();


                            //Finder Fee Adjustment
                            MaintenanceTempDetail oMaintenanceTempDetail_FinderFeeAdjustment;
                            FindersFeeAdjustment finderFeeAdjustment;
                            if (InputData.lstFindersFeeAdjustment != null)
                            {
                                

                                foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                                {

                                    finderFeeAdjustment = new FindersFeeAdjustment();
                                    finderFeeAdjustment.FindersFeeAdjustmentId = oMaintenanceTemp.MaintenanceTempId;
                                    finderFeeAdjustment.FranchiseeId = InputData.CustomerDetail.FranchiseeId;
                                    finderFeeAdjustment.FindersFeeId = finderFee.FindersFeeId;
                                    finderFeeAdjustment.Amount = oFindersFeeAdjustment.Amount;
                                    finderFeeAdjustment.Description = oFindersFeeAdjustment.Description;
                                    finderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    finderFeeAdjustment.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                                    finderFeeAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    finderFeeAdjustment.CreatedDate = DateTime.Now;
                                    context.FindersFeeAdjustments.Add(finderFeeAdjustment);
                                    context.SaveChanges();

                                    oMaintenanceTempDetail_FinderFeeAdjustment = new MaintenanceTempDetail();
                                    oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceDetailTypeListId = 6; //Contract Detail
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CustomerId = InputData.CustomerDetail.CustomerId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.ContractId = InputData.CustomerDetail.ContractId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;

                                    oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;


                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;
                                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFeeAdjustment);
                                    context.SaveChanges();
                                }
                            }
                        }

                    }
                    else
                    {
                        FindersFee finderFeeTbl = context.FindersFees.Where(o => o.FindersFeeId == InputData.FindersFee.FindersFeeId).FirstOrDefault();
                        finderFeeTbl.StatusListId = InputData.FindersFee.StatusListId;
                        finderFeeTbl.StartDate = InputData.FindersFee.StartDate;
                        finderFeeTbl.ResumeDate = InputData.FindersFee.ResumeDate;
                        finderFeeTbl.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                        finderFeeTbl.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                        finderFeeTbl.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                        //finderFeeTbl.Factor = InputData.FindersFee.Factor;
                        finderFeeTbl.DownPayPercentage = InputData.FindersFee.DownPayPercentage;
                        finderFeeTbl.DownPaymentAmount = InputData.FindersFee.DownPaymentAmount;

                        finderFeeTbl.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                        finderFeeTbl.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                        finderFeeTbl.MonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                        finderFeeTbl.FinancedAmount = InputData.FindersFee.FinancedAmount;
                        finderFeeTbl.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                        finderFeeTbl.Notes = InputData.FindersFee.Notes;
                        finderFeeTbl.ModifiedBy = this.LoginUserId;
                        finderFeeTbl.ModifiedDate = DateTime.Now;

                        if (finderFeeTbl.FindersFeeId == 0 && finderFeeTbl.DownPaymentAmount > 0)
                        {
                            finderFeeTbl.DownPaymentPaid = false;
                            finderFeeTbl.NumOfPaymentsPaid = -1;
                        }
                        finderFeeTbl.TotalAmount = InputData.FindersFee.TotalAmount;
                        finderFeeTbl.PayableOnAmount = InputData.FindersFee.PayableOnAmount;
                        finderFeeTbl.InterestAmount = 0;
                        finderFeeTbl.Interest = 0;
                        finderFeeTbl.IncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                        finderFeeTbl.Description = InputData.FindersFee.Description;
                        finderFeeTbl.BalanceAmount = (finderFeeTbl.TotalAmount != null ? finderFeeTbl.TotalAmount : 0) - (finderFeeTbl.PaidAmount != null ? finderFeeTbl.PaidAmount : 0);

                        context.SaveChanges();

                        if (InputData.lstFindersFeeAdjustment != null)
                        {

                            List<FindersFeeAdjustment> oldFFA = context.FindersFeeAdjustments.Where(f => f.FindersFeeId == finderFeeTbl.FindersFeeId).ToList();
                            List<FindersFeeAdjustment> oldFFADelete = new List<FindersFeeAdjustment>();

                            foreach (var itemFFA in oldFFA)
                            {   
                                if (InputData.lstFindersFeeAdjustment.Where(p => p.FindersFeeAdjustmentId == itemFFA.FindersFeeAdjustmentId).Count()== 0)
                                {
                                    oldFFADelete.Add(itemFFA);
                                }
                            }
                            context.FindersFeeAdjustments.RemoveRange(oldFFADelete);

                            foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                            {

                                FindersFeeAdjustment ffAdjustment = context.FindersFeeAdjustments.Where(o => o.FindersFeeAdjustmentId == oFindersFeeAdjustment.FindersFeeAdjustmentId).FirstOrDefault();
                                if (ffAdjustment != null)
                                {
                                    ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    ffAdjustment.FranchiseeId = oFindersFeeAdjustment.FranchiseeId;
                                    ffAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                                    ffAdjustment.Amount = oFindersFeeAdjustment.Amount;
                                    ffAdjustment.Description = oFindersFeeAdjustment.Description;
                                    ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    ffAdjustment.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                                    ffAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    ffAdjustment.CreatedDate = DateTime.Now;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    FindersFeeAdjustment ffAdjustment1 = new FindersFeeAdjustment();
                                    ffAdjustment1.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    ffAdjustment1.FranchiseeId = oFindersFeeAdjustment.FranchiseeId;
                                    ffAdjustment1.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                                    ffAdjustment1.Amount = oFindersFeeAdjustment.Amount;
                                    ffAdjustment1.Description = oFindersFeeAdjustment.Description;
                                    ffAdjustment1.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    ffAdjustment1.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                                    ffAdjustment1.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    ffAdjustment1.CreatedDate = DateTime.Now;
                                    context.FindersFeeAdjustments.Add(ffAdjustment1);
                                    context.SaveChanges();
                                }

                            }
                        }

                    }
                    #region ::  Distribution Data ::

                    if (InputData.lstFranchiseeDistribution.Count > 0)
                    {

                        int CustomerId = InputData.CustomerDetail.CustomerId;
                        int ContractDetailId = InputData.lstFranchiseeDistribution.FirstOrDefault().ContractDetailId;
                        JKApi.Data.DAL.Distribution oDistribution = new Data.DAL.Distribution();
                        using (jkDatabaseEntities _context = new jkDatabaseEntities())
                        {
                            oDistribution = _context.Distributions.Where(o => o.CustomerId == CustomerId && o.ContractDetailId == ContractDetailId).FirstOrDefault();
                        }

                        //var datamodel = context.Distributions.Where(w => w.CustomerId == InputData.CustomerDetail.CustomerId && w.ContractDetailId == InputData.lstFranchiseeDistribution.FirstOrDefault().ContractDetailId);
                        if (oDistribution == null)
                        {
                            JKApi.Data.DAL.Distribution objDistribution = new Data.DAL.Distribution();
                            objDistribution.CustomerId = InputData.CustomerDetail.CustomerId;
                            objDistribution.ContractDetailId = InputData.lstFranchiseeDistribution.FirstOrDefault().ContractDetailId;
                            objDistribution.FranchiseeId = InputData.CustomerDetail.FranchiseeId;
                            objDistribution.DetailLineNumber = Convert.ToInt32(InputData.lstFranchiseeDistribution.FirstOrDefault().DetailLineNumber);
                            objDistribution.Amount = InputData.lstFranchiseeDistribution.FirstOrDefault().Amount;
                            objDistribution.StartDate = (InputData.CustomerDetail.EffectiveDate != null ? InputData.CustomerDetail.EffectiveDate : InputData.CustomerDetail.StartDate);
                            objDistribution.EndDate = InputData.CustomerDetail.ExpirationDate;
                            objDistribution.isActive = true;
                            objDistribution.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                            objDistribution.ContractId = InputData.CustomerDetail.ContractId;
                            objDistribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            objDistribution.CreatedDate = DateTime.Now;
                            context.Distributions.Add(objDistribution);
                            context.SaveChanges();

                            //Status new record for Distribution
                            Status Diststatus = new Status();
                            Diststatus.ClassId = objDistribution.DistributionId;
                            Diststatus.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                            Diststatus.TypeListId = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Distribution);
                            Diststatus.IsActive = true;
                            Diststatus.CreatedBy = "-1";
                            Diststatus.CreatedDate = DateTime.Now;
                            context.Status.Add(Diststatus);
                            context.SaveChanges();
                            //Update distribution status
                            objDistribution.StatusId = Diststatus.StatusId;
                            context.SaveChanges();

                            //Distribution Fees Detaisl                  
                            foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                            {
                                JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                objDistributionFees.DistributionId = objDistribution.DistributionId;
                                objDistributionFees.FeeId = Item.FeeId.ToString();
                                objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                objDistributionFees.Amount = Item.Amount;
                                objDistributionFees.IsActive = true;
                                objDistributionFees.CreatedBy = -1;
                                objDistributionFees.CreatedDate = DateTime.Now;
                                context.DistributionFees.Add(objDistributionFees);
                                context.SaveChanges();

                            }
                        }
                        else
                        {
                            if (InputData.lstFranchiseeDistributionFee.Count() > 0)
                            {
                                //Distribution Fees Detaisl                  
                                foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                                {
                                    var model = context.DistributionFees.Where(w => w.DistributionId == oDistribution.DistributionId && w.FeeId == Item.FeeId.ToString());
                                    if (model != null && model.Count() > 0)
                                    {
                                        JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                        objDistributionFees = model.FirstOrDefault();
                                        objDistributionFees.Amount = Item.Amount;
                                        //context.DistributionFees.Add(objDistributionFees);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                        objDistributionFees.DistributionId = oDistribution.DistributionId;
                                        objDistributionFees.FeeId = Item.FeeId.ToString();
                                        objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                        objDistributionFees.Amount = Item.Amount;
                                        objDistributionFees.IsActive = true;
                                        objDistributionFees.CreatedBy = -1;
                                        objDistributionFees.CreatedDate = DateTime.Now;
                                        context.DistributionFees.Add(objDistributionFees);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            return retVal;

        }

        public bool InsertFranchiseeCustomerOnlyDistributionDetail(CommonFranchiseeCustomerViewModel InputData, int TypeListId)
        {
            bool retVal = true;
            using (var context = new jkDatabaseEntities())
            {

                if (InputData.CustomerDetail != null)
                {




                    #region ::  Distribution Data ::

                    if (InputData.lstFranchiseeDistribution != null)
                    {

                        var isAll = InputData.lstFranchiseeDistribution.Where(o => o.ContractDetailId == -1);

                        int CustomerId = InputData.CustomerDetail.CustomerId;
                        int FranchiseeId = InputData.CustomerDetail.FranchiseeId;

                        if (isAll.Count() > 0)
                        {
                            var lineDetails = GetFranchiseeCustomerDistributionData(CustomerId, FranchiseeId);
                            if (lineDetails != null)
                            {
                                var oldLineData = lineDetails.lstContractDetail;
                                if (oldLineData.Count() > 0)
                                {
                                    foreach (var item in oldLineData)
                                    {
                                        //int CustomerId = InputData.CustomerDetail.CustomerId;
                                        int ContractDetailId = item.ContractDetailId;
                                        JKApi.Data.DAL.Distribution oDistribution = new Data.DAL.Distribution();
                                        using (jkDatabaseEntities _context = new jkDatabaseEntities())
                                        {
                                            oDistribution = _context.Distributions.Where(o => o.CustomerId == CustomerId && o.ContractDetailId == ContractDetailId).FirstOrDefault();
                                        }
                                        //var datamodel = context.Distributions.Where(w => w.CustomerId == InputData.CustomerDetail.CustomerId && w.ContractDetailId == InputData.lstFranchiseeDistribution.FirstOrDefault().ContractDetailId);


                                        if (oDistribution == null)
                                        {
                                            JKApi.Data.DAL.Distribution objDistribution = new Data.DAL.Distribution();
                                            objDistribution.CustomerId = CustomerId;
                                            objDistribution.ContractDetailId = item.ContractDetailId;
                                            objDistribution.FranchiseeId = InputData.lstFranchiseeDistribution.FirstOrDefault().FranchiseeId;
                                            objDistribution.DetailLineNumber = item.LineNumber;
                                            objDistribution.Amount = item.Amount;
                                            objDistribution.StartDate = InputData.CustomerDetail.EffectiveDate;
                                            objDistribution.EndDate = InputData.CustomerDetail.ExpirationDate;
                                            objDistribution.isActive = true;
                                            objDistribution.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                                            objDistribution.ContractId = InputData.CustomerDetail.ContractId;
                                            objDistribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                            objDistribution.CreatedDate = DateTime.Now;
                                            context.Distributions.Add(objDistribution);
                                            context.SaveChanges();

                                            //Status new record for Distribution
                                            Status Diststatus = new Status();
                                            Diststatus.ClassId = objDistribution.DistributionId;
                                            Diststatus.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                                            Diststatus.TypeListId = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Distribution);
                                            Diststatus.IsActive = true;
                                            Diststatus.CreatedBy = "-1";
                                            Diststatus.CreatedDate = DateTime.Now;
                                            context.Status.Add(Diststatus);
                                            context.SaveChanges();
                                            //Update distribution status
                                            objDistribution.StatusId = Diststatus.StatusId;
                                            context.SaveChanges();

                                            //Distribution Fees Detais                  
                                            foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                                            {
                                                JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                                objDistributionFees.DistributionId = objDistribution.DistributionId;
                                                objDistributionFees.FeeId = Item.FeeId.ToString();
                                                objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                                objDistributionFees.Amount = Item.Amount;
                                                objDistributionFees.IsActive = true;
                                                objDistributionFees.CreatedBy = -1;
                                                objDistributionFees.CreatedDate = DateTime.Now;
                                                context.DistributionFees.Add(objDistributionFees);
                                                context.SaveChanges();

                                            }
                                        }
                                        else
                                        {
                                            JKApi.Data.DAL.Distribution objDistributionModel = new Data.DAL.Distribution();
                                            objDistributionModel = oDistribution;
                                            objDistributionModel.StartDate = InputData.CustomerDetail.EffectiveDate;
                                            objDistributionModel.FranchiseeId = InputData.lstFranchiseeDistribution.FirstOrDefault().FranchiseeId;
                                            objDistributionModel.Amount = item.Amount;
                                            context.Entry(objDistributionModel).State = EntityState.Modified;
                                            context.SaveChanges();


                                            if (InputData.lstFranchiseeDistributionFee.Count() > 0)
                                            {
                                                //Distribution Fees Detais                  
                                                foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                                                {
                                                    var model = context.DistributionFees.Where(w => w.DistributionId == oDistribution.DistributionId && w.FeeId == Item.FeeId.ToString());
                                                    if (model != null && model.Count() > 0)
                                                    {
                                                        JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                                        objDistributionFees = model.FirstOrDefault();
                                                        objDistributionFees.Amount = Item.Amount;
                                                        //context.DistributionFees.Add(objDistributionFees);
                                                        context.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                                        objDistributionFees.DistributionId = oDistribution.DistributionId;
                                                        objDistributionFees.FeeId = Item.FeeId.ToString();
                                                        objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                                        objDistributionFees.Amount = Item.Amount;
                                                        objDistributionFees.IsActive = true;
                                                        objDistributionFees.CreatedBy = -1;
                                                        objDistributionFees.CreatedDate = DateTime.Now;
                                                        context.DistributionFees.Add(objDistributionFees);
                                                        context.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            foreach (var item in InputData.lstFranchiseeDistribution)
                            {
                                //int CustomerId = InputData.CustomerDetail.CustomerId;
                                int ContractDetailId = item.ContractDetailId;
                                JKApi.Data.DAL.Distribution oDistribution = new Data.DAL.Distribution();
                                using (jkDatabaseEntities _context = new jkDatabaseEntities())
                                {
                                    oDistribution = _context.Distributions.Where(o => o.CustomerId == CustomerId && o.ContractDetailId == ContractDetailId).FirstOrDefault();
                                }
                                //var datamodel = context.Distributions.Where(w => w.CustomerId == InputData.CustomerDetail.CustomerId && w.ContractDetailId == InputData.lstFranchiseeDistribution.FirstOrDefault().ContractDetailId);


                                if (oDistribution == null)
                                {
                                    JKApi.Data.DAL.Distribution objDistribution = new Data.DAL.Distribution();
                                    objDistribution.CustomerId = CustomerId;
                                    objDistribution.ContractDetailId = item.ContractDetailId;
                                    objDistribution.FranchiseeId = item.FranchiseeId;
                                    objDistribution.DetailLineNumber = Convert.ToInt32(item.DetailLineNumber);
                                    objDistribution.Amount = item.Amount;
                                    objDistribution.StartDate = InputData.CustomerDetail.EffectiveDate;
                                    objDistribution.EndDate = InputData.CustomerDetail.ExpirationDate;
                                    objDistribution.isActive = true;
                                    objDistribution.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                                    objDistribution.ContractId = InputData.CustomerDetail.ContractId;
                                    objDistribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    objDistribution.CreatedDate = DateTime.Now;
                                    context.Distributions.Add(objDistribution);
                                    context.SaveChanges();

                                    //Status new record for Distribution
                                    Status Diststatus = new Status();
                                    Diststatus.ClassId = objDistribution.DistributionId;
                                    Diststatus.StatusListId = Convert.ToInt32(JKApi.Business.Enumeration.CustomerStatusList.RegionOperation);
                                    Diststatus.TypeListId = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Distribution);
                                    Diststatus.IsActive = true;
                                    Diststatus.CreatedBy = "-1";
                                    Diststatus.CreatedDate = DateTime.Now;
                                    context.Status.Add(Diststatus);
                                    context.SaveChanges();
                                    //Update distribution status
                                    objDistribution.StatusId = Diststatus.StatusId;
                                    context.SaveChanges();

                                    //Distribution Fees Detais                  
                                    foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                                    {
                                        JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                        objDistributionFees.DistributionId = objDistribution.DistributionId;
                                        objDistributionFees.FeeId = Item.FeeId.ToString();
                                        objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                        objDistributionFees.Amount = Item.Amount;
                                        objDistributionFees.IsActive = true;
                                        objDistributionFees.CreatedBy = -1;
                                        objDistributionFees.CreatedDate = DateTime.Now;
                                        context.DistributionFees.Add(objDistributionFees);
                                        context.SaveChanges();

                                    }
                                }
                                else
                                {

                                    JKApi.Data.DAL.Distribution objDistributionModel = new Data.DAL.Distribution();
                                    objDistributionModel = oDistribution;
                                    objDistributionModel.StartDate = objDistributionModel.StartDate != null ? objDistributionModel.StartDate : InputData.CustomerDetail.EffectiveDate;
                                    objDistributionModel.FranchiseeId = item.FranchiseeId;
                                    objDistributionModel.Amount = item.Amount;
                                    context.Entry(objDistributionModel).State = EntityState.Modified;
                                    context.SaveChanges();


                                    if (InputData.lstFranchiseeDistributionFee.Count() > 0)
                                    {
                                        //Distribution Fees Detais                  
                                        foreach (FCFranchiseeDistributionFeeViewModel Item in InputData.lstFranchiseeDistributionFee)
                                        {
                                            var model = context.DistributionFees.Where(w => w.DistributionId == oDistribution.DistributionId && w.FeeId == Item.FeeId.ToString());
                                            if (model != null && model.Count() > 0)
                                            {
                                                JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                                objDistributionFees = model.FirstOrDefault();
                                                objDistributionFees.Amount = Item.Amount;
                                                //context.DistributionFees.Add(objDistributionFees);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                JKApi.Data.DAL.DistributionFee objDistributionFees = new JKApi.Data.DAL.DistributionFee();
                                                objDistributionFees.DistributionId = oDistribution.DistributionId;
                                                objDistributionFees.FeeId = Item.FeeId.ToString();
                                                objDistributionFees.FeeRateTypeListId = Item.FeeRateTypeListId;
                                                objDistributionFees.Amount = Item.Amount;
                                                objDistributionFees.IsActive = true;
                                                objDistributionFees.CreatedBy = -1;
                                                objDistributionFees.CreatedDate = DateTime.Now;
                                                context.DistributionFees.Add(objDistributionFees);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }


                        }

                    }
                    #endregion

                }
            }
            return retVal;
        }
        public bool SaveFindersFeeDetails(CommonFranchiseeCustomerViewModel InputData, int TypeListId)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {
                //Finder Fee
                MaintenanceTempDetail oMaintenanceTempDetail_FinderFee;
                FindersFee finderFee;
                Status status;

                if (InputData.FindersFee.FindersFeeId == 0)
                {
                    status = new Status();
                    status.ClassId = InputData.CustomerDetail.FranchiseeId;
                    status.StatusListId = 39;
                    status.TypeListId = 2;
                    context.Status.Add(status);
                    context.SaveChanges();

                    finderFee = new FindersFee();
                    finderFee.StatusId = status.StatusId;
                    finderFee.StatusListId = status.StatusListId;
                    finderFee.CustomerId = InputData.CustomerDetail.CustomerId;
                    finderFee.FranchiseeId = InputData.CustomerDetail.FranchiseeId;
                    finderFee.DistributionId = InputData.FindersFee.DistributionId;
                    finderFee.FindersFeeId = InputData.FindersFee.FindersFeeId;
                    finderFee.StartDate = InputData.FindersFee.StartDate;
                    finderFee.ResumeDate = InputData.FindersFee.ResumeDate != null ? InputData.FindersFee.ResumeDate : InputData.FranchiseeDistribution.StartDate;
                    finderFee.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    finderFee.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    finderFee.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    finderFee.Factor = InputData.FindersFee.Factor!=null? InputData.FindersFee.Factor:0;
                    finderFee.DownPayPercentage = InputData.FindersFee.DownPayPercentage;
                    finderFee.Interest = InputData.FindersFee.Interest!=null? InputData.FindersFee.Interest:0;
                    finderFee.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    finderFee.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    finderFee.Notes = InputData.FindersFee.Notes;
                    finderFee.FinancedAmount = InputData.FindersFee.FinancedAmount;
                    finderFee.DownPaymentAmount = InputData.FindersFee.DownPaymentAmount;

                    if (finderFee.FindersFeeId == 0 && finderFee.DownPaymentAmount > 0)
                    {
                        finderFee.DownPaymentPaid = false;
                        finderFee.NumOfPaymentsPaid = -1;
                    }

                    finderFee.TotalAmount = InputData.FindersFee.TotalAmount;
                    finderFee.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                    finderFee.PaidAmount = InputData.FindersFee.PaidAmount!=null? InputData.FindersFee.PaidAmount:0;
                    finderFee.BalanceAmount = InputData.FindersFee.BalanceAmount == 0 ? InputData.FindersFee.TotalAmount : InputData.FindersFee.BalanceAmount;
                    finderFee.PayableOnAmount = InputData.FindersFee.PayableOnAmount;
                    finderFee.Notes = InputData.FindersFee.Notes;
                    finderFee.InterestAmount = InputData.FindersFee.InterestAmount!=null? InputData.FindersFee.InterestAmount:0;
                    finderFee.MonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                    finderFee.IncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                    finderFee.Description = InputData.FindersFee.Description;

                    finderFee.ImpId = 0;
                    finderFee.FindersFeeNumber = "";
                    finderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    finderFee.CreatedDate = DateTime.Now;
                    finderFee.ModifiedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    finderFee.ModifiedDate = DateTime.Now;
                    finderFee.IsActive = true;
                    context.FindersFees.Add(finderFee);
                    context.SaveChanges();



                }
                else
                {
                    FindersFee finderFeeTbl = context.FindersFees.Where(o => o.FindersFeeId == InputData.FindersFee.FindersFeeId).FirstOrDefault();

                    finderFeeTbl.StartDate = InputData.FindersFee.StartDate;
                    finderFeeTbl.ResumeDate = InputData.FindersFee.ResumeDate;
                    finderFeeTbl.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    finderFeeTbl.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    finderFeeTbl.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    finderFeeTbl.Factor = InputData.FindersFee.Factor;
                    finderFeeTbl.DownPayPercentage = InputData.FindersFee.DownPayPercentage;
                    finderFeeTbl.Interest = InputData.FindersFee.Interest;
                    finderFeeTbl.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    finderFeeTbl.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    finderFeeTbl.FinancedAmount = InputData.FindersFee.FinancedAmount;
                    finderFeeTbl.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                    finderFeeTbl.Notes = InputData.FindersFee.Notes;
                    finderFeeTbl.ModifiedBy = this.LoginUserId;
                    finderFeeTbl.ModifiedDate = DateTime.Now;
                    context.Entry(finderFeeTbl).State = EntityState.Modified;
                    context.SaveChanges();

                    #region :: FeeAdjustment :: 

                    //if (InputData.lstFindersFeeAdjustment != null)
                    //{
                    //    foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                    //    {

                    //        FindersFeeAdjustment ffAdjustment = context.FindersFeeAdjustments.Where(o => o.FindersFeeAdjustmentId == oFindersFeeAdjustment.FindersFeeAdjustmentId).FirstOrDefault();
                    //        ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                    //        ffAdjustment.FranchiseeId = oFindersFeeAdjustment.FranchiseeId;
                    //        ffAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                    //        ffAdjustment.Amount = oFindersFeeAdjustment.Amount;
                    //        ffAdjustment.Description = oFindersFeeAdjustment.Description;
                    //        ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                    //        ffAdjustment.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                    //        ffAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    //        ffAdjustment.CreatedDate = DateTime.Now;
                    //        context.FindersFeeAdjustments.Add(ffAdjustment);
                    //        context.SaveChanges();


                    //    }
                    //}
                    #endregion

                }
            }
            return retVal;
        }

        public bool SaveFindersFeeDetailsOnlyFF(CommonFranchiseeCustomerViewModel InputData, int TypeListId)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {
                //Finder Fee

                FindersFee finderFee;
                Status status;

                if (InputData.FindersFee.FindersFeeId == 0)
                {

                    status = new Status();
                    status.ClassId = InputData.CustomerDetail.FranchiseeId;
                    status.StatusListId = 39;
                    status.TypeListId = 2;
                    context.Status.Add(status);
                    context.SaveChanges();

                    finderFee = new FindersFee();
                    finderFee.StatusId = status.StatusId;
                    finderFee.StatusListId = 55;// status.StatusListId;
                    finderFee.FindersFeeId = InputData.FindersFee.FindersFeeId;
                    finderFee.StartDate = InputData.FindersFee.StartDate;
                    finderFee.ResumeDate = InputData.FindersFee.ResumeDate != null ? InputData.FindersFee.ResumeDate : InputData.FranchiseeDistribution.StartDate;
                    finderFee.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    finderFee.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    finderFee.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    finderFee.Factor = InputData.FindersFee.Factor != null ? InputData.FindersFee.Factor : 0;
                    finderFee.DownPayPercentage = InputData.FindersFee.DownPayPercentage;

                    finderFee.Interest = InputData.FindersFee.Interest != null ? InputData.FindersFee.Interest : 0;
                    finderFee.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    finderFee.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    finderFee.Notes = InputData.FindersFee.Notes;
                    finderFee.FinancedAmount = InputData.FindersFee.FinancedAmount;
                    finderFee.DownPaymentAmount = InputData.FindersFee.DownPaymentAmount;

                    if (finderFee.FindersFeeId == 0 && finderFee.DownPaymentAmount >    0)
                    {
                        finderFee.DownPaymentPaid = false;
                        finderFee.NumOfPaymentsPaid = -1;
                    }

                    finderFee.TotalAmount = InputData.FindersFee.TotalAmount;
                    finderFee.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount!=null? InputData.FindersFee.MultiTenantOccupancyAmount:0;
                    finderFee.PaidAmount = InputData.FindersFee.PaidAmount != null ? InputData.FindersFee.PaidAmount : 0;
                    finderFee.BalanceAmount = InputData.FindersFee.BalanceAmount == 0 ? InputData.FindersFee.TotalAmount : InputData.FindersFee.BalanceAmount;
                    finderFee.PayableOnAmount = InputData.FindersFee.PayableOnAmount != null ? InputData.FindersFee.PayableOnAmount : 0;
                    finderFee.Notes = !String.IsNullOrEmpty(InputData.FindersFee.Notes) ? InputData.FindersFee.Notes : "";
                    finderFee.InterestAmount = InputData.FindersFee.InterestAmount != null ? InputData.FindersFee.InterestAmount : 0;
                    finderFee.MonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                    finderFee.IncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                    finderFee.Description = InputData.FindersFee.Description;
                    finderFee.RegionId = SelectedRegionId;
                    finderFee.ImpId = 0;
                    finderFee.FindersFeeNumber = "";
                    finderFee.BalanceAmount = 0;
                    finderFee.PayableOnAmount = 0;
                    finderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    finderFee.CreatedDate = DateTime.Now;
                    finderFee.ModifiedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                    finderFee.ModifiedDate = DateTime.Now;


                    Data.DAL.Distribution distribution = context.Distributions.FirstOrDefault(b => b.DistributionId == InputData.FindersFee.DistributionId);

                    finderFee.FranchiseeId = InputData.FindersFee.FranchiseeId>0? InputData.FindersFee.FranchiseeId: distribution.FranchiseeId;
                    finderFee.CustomerId = InputData.FindersFee.CustomerId>0 ? InputData.FindersFee.CustomerId : distribution.CustomerId;
                    finderFee.DistributionId = InputData.FindersFee.DistributionId;
                    finderFee.IsActive = true;
                    finderFee.SequenceNum = 0;                    

                    context.FindersFees.Add(finderFee);
                    context.SaveChanges();

                    #region //Finder Fee Adjustment

                    FindersFeeAdjustment finderFeeAdjustment;
                    if (InputData.lstFindersFeeAdjustment != null)
                    {
                        foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                        {

                            finderFeeAdjustment = new FindersFeeAdjustment();
                            //finderFeeAdjustment.FindersFeeAdjustmentId = oMaintenanceTemp.MaintenanceTempId;
                            finderFeeAdjustment.FranchiseeId = InputData.CustomerDetail.FranchiseeId;
                            finderFeeAdjustment.FindersFeeId = finderFee.FindersFeeId;
                            finderFeeAdjustment.Amount = oFindersFeeAdjustment.Amount;
                            finderFeeAdjustment.Description = oFindersFeeAdjustment.Description;
                            finderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                            finderFeeAdjustment.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                            finderFeeAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            finderFeeAdjustment.CreatedDate = DateTime.Now;
                            context.FindersFeeAdjustments.Add(finderFeeAdjustment);
                            context.SaveChanges();

                        }
                    }
                    #endregion

                }
                else
                {
                    FindersFee finderFeeTbl = context.FindersFees.Where(o => o.FindersFeeId == InputData.FindersFee.FindersFeeId).FirstOrDefault();



                    finderFeeTbl.CustomerId = InputData.CustomerDetail.CustomerId;
                    finderFeeTbl.FranchiseeId = InputData.CustomerDetail.FranchiseeId;
                    finderFeeTbl.DistributionId = InputData.FindersFee.DistributionId;
                    finderFeeTbl.FindersFeeId = InputData.FindersFee.FindersFeeId;
                    finderFeeTbl.StartDate = InputData.FindersFee.StartDate != null ? InputData.FindersFee.StartDate : InputData.FranchiseeDistribution.StartDate;
                    finderFeeTbl.ResumeDate = InputData.FindersFee.ResumeDate != null ? InputData.FindersFee.ResumeDate : InputData.FranchiseeDistribution.StartDate;
                    finderFeeTbl.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    finderFeeTbl.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    finderFeeTbl.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    finderFeeTbl.Factor = InputData.FindersFee.Factor;
                    finderFeeTbl.DownPayPercentage = InputData.FindersFee.DownPayPercentage;
                    finderFeeTbl.Interest = InputData.FindersFee.Interest;
                    finderFeeTbl.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    finderFeeTbl.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    finderFeeTbl.Notes = InputData.FindersFee.Notes;
                    finderFeeTbl.FinancedAmount = InputData.FindersFee.FinancedAmount;
                    finderFeeTbl.DownPaymentAmount = InputData.FindersFee.DownPaymentAmount;
                    finderFeeTbl.TotalAmount = InputData.FindersFee.TotalAmount;
                    finderFeeTbl.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount != null ? InputData.FindersFee.MultiTenantOccupancyAmount : 0;
                    finderFeeTbl.PaidAmount = InputData.FindersFee.PaidAmount != null ? InputData.FindersFee.PaidAmount : 0;
                    finderFeeTbl.BalanceAmount = (InputData.FindersFee.BalanceAmount == 0 || InputData.FindersFee.BalanceAmount == null) ? ((InputData.FindersFee.TotalAmount != null ? InputData.FindersFee.TotalAmount : 0) - (InputData.FindersFee.PaidAmount != null ? InputData.FindersFee.PaidAmount : 0)) : InputData.FindersFee.BalanceAmount;
                    finderFeeTbl.PayableOnAmount = InputData.FindersFee.PayableOnAmount;
                    finderFeeTbl.Notes = InputData.FindersFee.Notes;
                    finderFeeTbl.InterestAmount = InputData.FindersFee.InterestAmount;
                    finderFeeTbl.MonthlyPaymentPercentage = InputData.FindersFee.MonthlyPaymentPercentage;
                    finderFeeTbl.IncludeDownPayInFirstPay = InputData.FindersFee.IncludeDownPayInFirstPay;
                    finderFeeTbl.Description = InputData.FindersFee.Description;
                    finderFeeTbl.RegionId = SelectedRegionId;
                    finderFeeTbl.SequenceNum = finderFeeTbl.SequenceNum != null ? finderFeeTbl.SequenceNum : 0;
                    //finderFeeTbl.StartDate = InputData.FindersFee.StartDate;
                    //finderFeeTbl.ResumeDate = InputData.FindersFee.ResumeDate;
                    //finderFeeTbl.FindersFeeTypeListId = InputData.FindersFee.FindersFeeTypeListId;
                    //finderFeeTbl.ContractBillingAmount = InputData.FindersFee.ContractBillingAmount;
                    //finderFeeTbl.TotalAdjustmentAmount = InputData.FindersFee.TotalAdjustmentAmount;
                    //finderFeeTbl.Factor = InputData.FindersFee.Factor;
                    //finderFeeTbl.DownPayPercentage = InputData.FindersFee.DownPayPercentage;
                    //finderFeeTbl.Interest = InputData.FindersFee.Interest;
                    //finderFeeTbl.TotalNumOfpayments = InputData.FindersFee.TotalNumOfpayments;
                    //finderFeeTbl.MonthlyPaymentAmount = InputData.FindersFee.MonthlyPaymentAmount;
                    //finderFeeTbl.FinancedAmount = InputData.FindersFee.FinancedAmount;
                    //finderFeeTbl.MultiTenantOccupancyAmount = InputData.FindersFee.MultiTenantOccupancyAmount;
                    //finderFeeTbl.Notes = InputData.FindersFee.Notes;
                    finderFeeTbl.ModifiedBy = this.LoginUserId;
                    finderFeeTbl.ModifiedDate = DateTime.Now;
                    context.Entry(finderFeeTbl).State = EntityState.Modified;
                    context.SaveChanges();

                    #region :: FeeAdjustment :: 

                    if (InputData.lstFindersFeeAdjustment != null)
                    {
                        foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                        {

                            FindersFeeAdjustment ffAdjustment = context.FindersFeeAdjustments.Where(o => o.FindersFeeAdjustmentId == oFindersFeeAdjustment.FindersFeeAdjustmentId).FirstOrDefault();
                            if (ffAdjustment == null)
                                ffAdjustment = new FindersFeeAdjustment();

                            ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                            ffAdjustment.FranchiseeId = oFindersFeeAdjustment.FranchiseeId;
                            ffAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                            ffAdjustment.Amount = oFindersFeeAdjustment.Amount;
                            ffAdjustment.Description = oFindersFeeAdjustment.Description;
                            ffAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                            ffAdjustment.FindersFeeAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;

                            ffAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            ffAdjustment.CreatedDate = DateTime.Now;
                            context.FindersFeeAdjustments.Add(ffAdjustment);
                            context.SaveChanges();
                        }
                    }
                    #endregion

                }
            }
            return retVal;
        }
        public bool UpdateFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData)
        {
            bool retVal = true;

            using (var context = new jkDatabaseEntities())
            {

                MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();
                if (InputData.CustomerDetail != null)
                {
                    MaintenanceTemp oMaintenanceTemp = context.MaintenanceTemps.SingleOrDefault(o => o.MaintenanceTempId == InputData.CustomerDetail.MaintenanceTempId);
                    if (oMaintenanceTemp != null)
                    {
                        //oMaintenanceTemp.ClassId = InputData.CustomerDetail.FranchiseeId;
                        //oMaintenanceTemp.TypeListId = 2;
                        //oMaintenanceTemp.StatusListId = InputData.CustomerDetail.StatusListId;
                        //oMaintenanceTemp.StatusReasonListId = InputData.CustomerDetail.StatusReasonListId;
                        //oMaintenanceTemp.MaintenanceTypeListId = 6;
                        oMaintenanceTemp.EffectiveDate = InputData.CustomerDetail.EffectiveDate;
                        oMaintenanceTemp.IsActive = true;
                        oMaintenanceTemp.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        oMaintenanceTemp.CreatedDate = DateTime.Now;

                        oMaintenanceTemp.RegionId = InputData.CustomerDetail.RegionId;
                        context.SaveChanges();


                        if (InputData.lstFranchiseeDistribution.Count > 1 || InputData.lstFranchiseeDistribution.Count == 1)
                        {
                            foreach (FCFranchiseeDistributionViewModel oFCFranchiseeDistributionViewModel in InputData.lstFranchiseeDistribution)
                            {
                                MaintenanceTempDetail oMaintenanceTempDetail_Distribution = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oFCFranchiseeDistributionViewModel.MaintenanceTempDetailId);
                                if (oMaintenanceTempDetail_Distribution != null)
                                {
                                    oMaintenanceTempDetail_Distribution.DAmount = oFCFranchiseeDistributionViewModel.Amount;
                                    oMaintenanceTempDetail_Distribution.DTotalAmount = oFCFranchiseeDistributionViewModel.Amount;
                                    oMaintenanceTempDetail_Distribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                                    context.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            MaintenanceTempDetail oMaintenanceTempDetail_Distribution = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == InputData.FranchiseeDistribution.MaintenanceTempDetailId);
                            if (InputData.FranchiseeDistribution != null && oMaintenanceTempDetail_Distribution != null)
                            {
                                oMaintenanceTempDetail_Distribution.DAmount = InputData.FranchiseeDistribution.Amount;
                                oMaintenanceTempDetail_Distribution.DTotalAmount = InputData.FranchiseeDistribution.Amount;
                                oMaintenanceTempDetail_Distribution.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                oMaintenanceTempDetail_Distribution.CreatedDate = DateTime.Now;
                                context.SaveChanges();
                            }
                        }
                        //Distribution

                        //Finder Fee
                        if (InputData.FindersFee != null)
                        {
                            MaintenanceTempDetail oMaintenanceTempDetail_FinderFee = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == InputData.FindersFee.MaintenanceTempDetailId);

                            oMaintenanceTempDetail_FinderFee.FFStartDate = InputData.FindersFee.StartDate;
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
                            oMaintenanceTempDetail_FinderFee.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                            oMaintenanceTempDetail_FinderFee.CreatedDate = DateTime.Now;
                            context.SaveChanges();
                        }

                        //Finder Fee Adjustment
                        if (InputData.lstFindersFeeAdjustment != null)
                        {
                            foreach (FCFindersFeeAdjustmentViewModel oFindersFeeAdjustment in InputData.lstFindersFeeAdjustment)
                            {
                                MaintenanceTempDetail oMaintenanceTempDetail_FinderFeeAdjustment = context.MaintenanceTempDetails.SingleOrDefault(o => o.MaintenanceTempDetailId == oFindersFeeAdjustment.MaintenanceTempDetailId);
                                if (oMaintenanceTempDetail_FinderFeeAdjustment != null)
                                {

                                    oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;

                                    context.SaveChanges();
                                }
                                else
                                {
                                    oMaintenanceTempDetail_FinderFeeAdjustment = new MaintenanceTempDetail();
                                    oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.MaintenanceDetailTypeListId = 6; //Contract Detail
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CustomerId = InputData.CustomerDetail.CustomerId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.ContractId = InputData.CustomerDetail.ContractId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.DFranchiseeId = InputData.CustomerDetail.FranchiseeId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeId = oFindersFeeAdjustment.FindersFeeId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAAmount = oFindersFeeAdjustment.Amount;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFADescription = oFindersFeeAdjustment.Description;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FindersFeeAdjustmentId = oFindersFeeAdjustment.FindersFeeAdjustmentId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.FFAdjustmentTypeListId = oFindersFeeAdjustment.FindersFeeAdjustmentTypeListId;
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                                    oMaintenanceTempDetail_FinderFeeAdjustment.CreatedDate = DateTime.Now;
                                    context.MaintenanceTempDetails.Add(oMaintenanceTempDetail_FinderFeeAdjustment);
                                    context.SaveChanges();

                                }
                            }
                        }
                    }
                }
            }

            return retVal;

        }

        public bool UpdateFranchiseeMaintenanceTemp(EditFranchiseeMaintenanceViewModel inputData)
        {
            bool retVal = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                var lstMaintenanceTemp = context.MaintenanceTemps.Where(o => o.ClassId == inputData.ClassId && o.TypeListId == 2).FirstOrDefault();
                if (lstMaintenanceTemp != null)
                {
                    lstMaintenanceTemp.StatusListId = inputData.StatusListId;
                    lstMaintenanceTemp.LastServiceDate = inputData.LastServiceDate;
                    lstMaintenanceTemp.ResumeDate = inputData.ResumeDate;
                    lstMaintenanceTemp.EffectiveDate = inputData.EffectiveDate;
                    lstMaintenanceTemp.StatusReasonListId = inputData.StatusReasonListId;
                    lstMaintenanceTemp.Comments = inputData.Comments;
                    lstMaintenanceTemp.MaintenanceStatusListId = 1;
                    context.SaveChanges();
                }
            }
            return retVal;
        }

        public bool InsertUpdateFranchiseeMaintenanceTemp(FranchiseeMaintenanceViewModel inputData)
        {
            bool retVal = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                List<MaintenanceTemp> lstMaintenanceTemp = context.MaintenanceTemps.Where(o => o.ClassId == inputData.FranchiseeId && o.TypeListId == 2).ToList();

                foreach (MaintenanceTemp ob in lstMaintenanceTemp)
                {
                    ob.IsActive = false;
                }
                context.SaveChanges();

                MaintenanceTemp oMaintenanceTemp = new MaintenanceTemp();
                oMaintenanceTemp.ClassId = inputData.FranchiseeId;
                oMaintenanceTemp.TypeListId = 2;
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
                oMaintenanceTemp.MaintenanceTypeListId = 0;
                oMaintenanceTemp.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());
                context.MaintenanceTemps.Add(oMaintenanceTemp);
                context.SaveChanges();

            }
            return retVal;
        }

        //maintenance pending 
        public List<FranchiseePendingMaintenanceListViewModel> GetFranchiseePendingMaintenanceList()
        {
            List<FranchiseePendingMaintenanceListViewModel> lstFranchiseePendingMaintenanceList = new List<FranchiseePendingMaintenanceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", SelectedRegionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_FranchiseePendingMaintenanceList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseePendingMaintenanceList = multipleresult.Read<FranchiseePendingMaintenanceListViewModel>().ToList();
                    }
                }

            }
            return lstFranchiseePendingMaintenanceList;
        }


        public bool GetFranchiseePendingMaintenanceApproved(string ids, bool IsApproved)
        {
            List<FranchiseePendingMaintenanceListViewModel> lstFranchiseePendingMaintenanceList = new List<FranchiseePendingMaintenanceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                foreach (string _id in ids.Split(','))
                {
                    if (_id.Trim() != "")
                    {
                        if (IsApproved)
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@MaintenanceTempId", _id);
                            parmas.Add("@UserId", LoginUserId);

                            using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_F_CustomerFranchiseeDistributionApproval", parmas, commandType: CommandType.StoredProcedure))
                            {
                                if (multipleresult != null)
                                {
                                    //lstFranchiseePendingMaintenanceList = multipleresult.Read<FranchiseePendingMaintenanceListViewModel>().ToList();
                                }
                            }
                        }
                        else
                        {
                            using (jkDatabaseEntities context = new jkDatabaseEntities())
                            {
                                MaintenanceTemp oMaintenanceTemp = context.MaintenanceTemps.SingleOrDefault(o => o.MaintenanceTempId.ToString() == _id);
                                if (oMaintenanceTemp != null)
                                {
                                    oMaintenanceTemp.MaintenanceStatusListId = 2;
                                    oMaintenanceTemp.ModifiedBy = LoginUserId;
                                    oMaintenanceTemp.ModifiedDate = DateTime.Now;
                                    context.SaveChanges();
                                }
                            }

                        }
                    }
                }

            }
            return true;
        }


        public CommonFranchiseeCustomerViewModel GetFranchiseeCustomerDistributionDataForEdit(int MaintenanceTempId)
        {

            CommonFranchiseeCustomerViewModel oCommon = new CommonFranchiseeCustomerViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MaintenanceTempId", MaintenanceTempId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_F_FranchiseeCustomerDetailForEdit", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommon.CustomerDetail = multipleresult.Read<FCDetailViewModel>().ToList().FirstOrDefault();
                        oCommon.lstContractDetail = multipleresult.Read<FCContractDetailViewModel>().ToList();
                        oCommon.lstFranchiseeDistribution = multipleresult.Read<FCFranchiseeDistributionViewModel>().ToList();
                        oCommon.FranchiseeDistribution = oCommon.lstFranchiseeDistribution.FirstOrDefault();

                        oCommon.lstFranchiseeDistributionFee = multipleresult.Read<FCFranchiseeDistributionFeeViewModel>().ToList();

                        List<FCFindersFeeViewModel> lstFindersFee = multipleresult.Read<FCFindersFeeViewModel>().ToList();
                        oCommon.FindersFee = lstFindersFee.FirstOrDefault() != null ? lstFindersFee.FirstOrDefault() : new FCFindersFeeViewModel();
                        oCommon.lstFindersFeeAdjustment = multipleresult.Read<FCFindersFeeAdjustmentViewModel>().ToList();
                    }
                }
            }

            return oCommon;

        }


        #endregion


        public List<FranchiseeSearchModel> GetSearchFranchisee(string searchText)
        {
            List<FranchiseeSearchModel> lstFranchiseeSearch = new List<FranchiseeSearchModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@SearchText", searchText);
                parmas.Add("@RegionId", SelectedRegionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_FranchiseeSearch", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeSearch = multipleresult.Read<FranchiseeSearchModel>().ToList();
                    }
                }
            }
            return lstFranchiseeSearch;
        }

        public ManualTransactionViewModel EditManualTrasaction(string TrxNo)
        {
            ManualTransactionViewModel lstFranchiseeSearch = new ManualTransactionViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@transactionNumber", TrxNo);
                using (var multipleresult = conn.QueryMultiple("dbo.spGet_Franchisee_EditManualTransaction", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeSearch = multipleresult.Read<ManualTransactionViewModel>().FirstOrDefault();
                    }
                }
            }
            return lstFranchiseeSearch;
        }

        #endregion

        #region Lease
        public Lease SaveLease(Lease Lease)
        {
            var ID = Lease.LeaseId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Lease.Add(Lease);
                Uow.Commit();

                jkDatabaseEntities context = new jkDatabaseEntities();

                Data.DAL.Status oStatus = new Data.DAL.Status();
                oStatus.ClassId = Lease.ClassId;
                oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Lease;
                oStatus.StatusListId = 21;
                oStatus.StatusDate = DateTime.Now;
                oStatus.LastServiceDate = DateTime.Now;
                oStatus.IsActive = true;
                oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                context.Status.Add(oStatus);
                context.SaveChanges();

                int statusid = oStatus.StatusId;
                Data.DAL.Lease oLease = context.Leases.SingleOrDefault(o => o.LeaseId == Lease.LeaseId);
                if (oLease != null)
                {
                    oLease.StatusId = statusid;
                    oLease.StatusListId = oStatus.StatusListId;
                    context.SaveChanges();
                }

            }
            else //update existing entry
            {

                Uow.Lease.Update(Lease);
                Uow.Commit();
            }

            return Lease;
        }

        public LeaseViewModel GetLeaseModel(int Id)
        {
            LeaseViewModel Model = new LeaseViewModel();
            var dataModel = Uow.Lease.GetById(Id);
            if (dataModel != null)
            {
                Model = dataModel.ToModel<LeaseViewModel, JKApi.Data.DAL.Lease>();
            }
            return Model;
        }

        #endregion

        public IQueryable<MasterTrxTypeList> GetUsMasterTrxTypeList()
        {
            var qry = Uow.MasterTrxTypeList.GetAll().Where(x => x.TypeListId == 2);
            return qry;
        }
        public FullFranchiseeViewModel GetFranchiseeDetailsById(int id)
        {
            FullFranchiseeViewModel FullFranchiseeViewModel = new FullFranchiseeViewModel();

            int BusinessTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int BusinessContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int ContactContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation);

            int OwnerTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);
            int OwnerContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
            int FranachiseeTypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Franchisee);

            int PayeeContactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Payee);


            int CustomerTypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
            //int CustomerMainContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
            //int CustomerBillingContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Billing;
            //int CustomerContactInformationContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation;
            //int CustomerBillingInformationContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Payee;

            //int CustomerMainContactType2ListId = (int)JKApi.Business.Enumeration.ContactTypeList.Contact;
            //int CustomerBillingContactType2ListId = (int)JKApi.Business.Enumeration.ContactTypeList.BillingContact;

            using (var context = new jkDatabaseEntities())
            {

                var result = (from c in context.Franchisees
                              where c.FranchiseeId == id
                              join mc in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == ContactContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals mc.ClassId
                              into temp0
                              from t0 in temp0.DefaultIfEmpty()
                              join mp in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == ContactContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals mp.ClassId
                              into temp1
                              from t1 in temp1.DefaultIfEmpty()
                              join ma in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == ContactContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals ma.ClassId
                              into temp2
                              from t2 in temp2.DefaultIfEmpty()
                              join me in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == ContactContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals me.ClassId
                              into temp3
                              from t3 in temp3.DefaultIfEmpty()


                                  //join bc in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bc.ClassId
                                  //into temp4
                                  //from t4 in temp4.DefaultIfEmpty()
                                  //join ba in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals ba.ClassId
                                  //into temp6
                                  //from t6 in temp6.DefaultIfEmpty()
                                  //join cic in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerContactInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals cic.ClassId
                                  //into temp8
                                  //from t8 in temp8.DefaultIfEmpty()

                              join cip in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == BusinessContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals cip.ClassId
                              into temp9
                              from t9 in temp9.DefaultIfEmpty()
                              join cia in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == BusinessContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals cia.ClassId
                              into temp10
                              from t10 in temp10.DefaultIfEmpty()
                              join cie in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == BusinessContactTypeList && one.ClassId == id && one.IsActive == true) on c.FranchiseeId equals cie.ClassId
                              into temp11
                              from t11 in temp11.DefaultIfEmpty()


                                  //join bic in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bic.ClassId
                                  //into temp12
                                  //from t12 in temp12.DefaultIfEmpty()
                                  //join bip in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bip.ClassId
                                  //into temp13
                                  //from t13 in temp13.DefaultIfEmpty()
                                  //join bia in context.Addresses.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bia.ClassId
                                  //into temp14
                                  //from t14 in temp14.DefaultIfEmpty()
                                  //join bie in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingInformationContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bie.ClassId
                                  //into temp15
                                  //from t15 in temp15.DefaultIfEmpty()
                                  //join bs in context.BillSettings.Where(one => one.CustomerId == id && one.IsActive == 1) on c.CustomerId equals bs.CustomerId
                                  //into temp16
                                  //from t16 in temp16.DefaultIfEmpty()
                                  //join cc in context.Contracts.Where(one => one.CustomerId == id) on c.CustomerId equals cc.CustomerId
                                  //into temp17
                                  //from t17 in temp17.DefaultIfEmpty()
                                  //join cd in context.ContractDetails on t17.ContractId equals cd.ContractId
                                  //into temp18
                                  //from t18 in temp18.DefaultIfEmpty()


                                  //join mct in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerMainContactType2ListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals mct.ClassId
                                  //into temp19
                                  //from t19 in temp19.DefaultIfEmpty()

                                  //join bct in context.Contacts.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactType2ListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bct.ClassId
                                  //into temp20
                                  //from t20 in temp20.DefaultIfEmpty()

                                  //join bp in context.Phones.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals bp.ClassId
                                  //into temp21
                                  //from t21 in temp21.DefaultIfEmpty()
                                  //join be in context.Emails.Where(one => one.TypeListId == CustomerTypeListId && one.ContactTypeListId == CustomerBillingContactTypeListId && one.ClassId == id && one.IsActive == true) on c.CustomerId equals be.ClassId
                                  //into temp22
                                  //from t22 in temp22.DefaultIfEmpty()


                              where c.RegionId == SelectedRegionId || SelectedRegionId == 0
                              select new FranchiseeCollection
                              {
                                  BusinessInfo = c,
                                  ContactInfo = t0,
                                  ContactInfoPhone = t1,
                                  ContactInfoAddress = t2,
                                  ContactInfoEmail = t3,
                                  //BillingContact = t4,
                                  //BillingAddress = t6,
                                  //ContactInformation = t8,
                                  BusinessInfoPhone = t9,
                                  BusinessInfoAddress = t10,
                                  BusinessInfoEmail = t11,
                                  //BillingInformation = t12,
                                  //BillingInformationPhone = t13,
                                  //BillingInformationAddress = t14,
                                  //BillingInformationEmail = t15,
                                  //BillSetting = t16,
                                  //Contract = t17,
                                  //ContractDetail = t18,
                                  //MainPhone2 = t19,
                                  //BillingContactInformation2 = t20,
                                  //BillingPhone = t21,
                                  //BillingEmail = t22

                              }).FirstOrDefault();
                if (result != null)
                {
                    FullFranchiseeViewModel.BusinessInfo = result.BusinessInfo.ToModel<FranchiseeViewModel, JKApi.Data.DAL.Franchisee>();
                    FullFranchiseeViewModel.ContactInfo = result.ContactInfo.ToModel<ContactViewModel, Contact>();
                    FullFranchiseeViewModel.ContactInfoPhone = result.ContactInfoPhone.ToModel<PhoneViewModel, Phone>();
                    FullFranchiseeViewModel.ContactInfoAddress = result.ContactInfoAddress.ToModel<AddressViewModel, Address>();
                    FullFranchiseeViewModel.ContactInfoEmail = result.ContactInfoEmail.ToModel<EmailViewModel, Email>();
                    FullFranchiseeViewModel.BusinessInfoPhone = result.BusinessInfoPhone.ToModel<PhoneViewModel, Phone>();
                    FullFranchiseeViewModel.BusinessInfoAddress = result.BusinessInfoAddress.ToModel<AddressViewModel, Address>();
                    FullFranchiseeViewModel.BusinessInfoEmail = result.BusinessInfoEmail.ToModel<EmailViewModel, Email>();

                    //CustomerViewModel.ContactInformationAddress = result.ContactInformationAddress.ToModel<AddressViewModel, Address>();
                    //CustomerViewModel.ContactInformationPhone = result.ContactInformationPhone.ToModel<PhoneViewModel, Phone>();
                    //CustomerViewModel.ContactInformationEmail = result.ContactInformationEmail.ToModel<EmailViewModel, Email>();
                    //CustomerViewModel.BillingInformation = result.BillingInformation.ToModel<ContactViewModel, Contact>();
                    //CustomerViewModel.BillingInformationAddress = result.BillingInformationAddress.ToModel<AddressViewModel, Address>();
                    //CustomerViewModel.BillingInformationPhone = result.BillingInformationPhone.ToModel<PhoneViewModel, Phone>();
                    //CustomerViewModel.BillingInformationEmail = result.BillingInformationEmail.ToModel<EmailViewModel, Email>();
                    //CustomerViewModel.BillSetting = result.BillSetting.ToModel<BillSettingViewModel, BillSetting>();
                    //CustomerViewModel.Contract = result.Contract.ToModel<ContractViewModel, Data.DAL.Contract>();
                    //CustomerViewModel.ContractDetail = result.ContractDetail.ToModel<ContractDetailViewModel, ContractDetail>();
                    //CustomerViewModel.MainPhone2 = result.MainPhone2.ToModel<PhoneViewModel, Phone>();
                    //CustomerViewModel.BillingContactInformation2 = result.BillingContactInformation2.ToModel<ContactViewModel, Contact>();
                    //CustomerViewModel.BillingPhone = result.BillingPhone.ToModel<PhoneViewModel, Phone>();
                    //CustomerViewModel.BillingEmail = result.BillingEmail.ToModel<EmailViewModel, Email>();

                }
                return FullFranchiseeViewModel;
            }
        }


        #region FinderFee
        public List<portal_spGet_F_FindersFeeList_Result> GetFinderFeeList(string regionIds, string statusIds)
        {
            if (statusIds != "")
            {
                var regionIdList = regionIds.Split(new char[] { ',' }).ToList();

                using (var context = new jkDatabaseEntities())
                {
                    return context.portal_spGet_F_FindersFeeList(regionIds, statusIds).ToList();
                }
            }
            else
                return new List<portal_spGet_F_FindersFeeList_Result>();
        }
        public List<portal_spGet_F_FindersFeeDetailList_Result> GetFinderFeeDetailList(int franchiseeid, string regionIds, string statusIds)
        {
            using (var context = new jkDatabaseEntities())
            {
                if (statusIds != "")
                    return context.portal_spGet_F_FindersFeeDetailList(franchiseeid, regionIds, statusIds).ToList();
                else
                    return new List<portal_spGet_F_FindersFeeDetailList_Result>();
                //if (regionid == 0)
                //{
                //    return context.vw_F_FinderFeeDetailList.Where(o => o.FranchiseeId == franchiseeid).ToList();
                //}
                //else
                //{
                //    return context.vw_F_FinderFeeDetailList.Where(o => o.RegionId == regionid && o.FranchiseeId == franchiseeid).ToList();
                //}
            }
        }
        public List<portal_spGet_F_GetFinderFeeCustomersList_Result> GetFinderFeeCustomersList(int regionid, int franchiseeid, string statuslistid)
        {
            List<portal_spGet_F_GetFinderFeeCustomersList_Result> lstResult = new List<portal_spGet_F_GetFinderFeeCustomersList_Result>();
            using (var context = new jkDatabaseEntities())
            {
                lstResult = context.portal_spGet_F_GetFinderFeeCustomersList(regionid, franchiseeid, statuslistid).ToList();
                return lstResult;
            }
        }

        public List<FinderFeeCustomerListStatusViewModel> GetFinderFeeCustomerListStatus(int regionid, int franchiseid)
        {
            var finderfeecustomerliststatus = new List<FinderFeeCustomerListStatusViewModel>();
            using (var con = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@Region", regionid);
                parmas.Add("@FranchiseId", franchiseid);
                using (var result = con.QueryMultiple("dbo.portal_spGet_F_GetFinderFeeCustomerListStatus", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (result != null)
                    {
                        finderfeecustomerliststatus = result.Read<FinderFeeCustomerListStatusViewModel>().ToList();
                    }
                }
            }
            return finderfeecustomerliststatus;
        }

        public List<FranchiseeFinderFeeDetailViewModel> GetFranchiseeFinderFeeDetailList(int regionid, int franchiseeid)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeFinderFeeDetailViewModel> lstFranchiseeFinderFeeDetailViewModel = new List<FranchiseeFinderFeeDetailViewModel>();
                if (regionid == 0)
                {
                    lstFranchiseeFinderFeeDetailViewModel = context.vw_F_FinderFeeDetailList.Where(o => o.FranchiseeId == franchiseeid).MapEnumerable<FranchiseeFinderFeeDetailViewModel, vw_F_FinderFeeDetailList>().ToList();
                }
                else
                {
                    lstFranchiseeFinderFeeDetailViewModel = context.vw_F_FinderFeeDetailList.Where(o => o.RegionId == regionid && o.FranchiseeId == franchiseeid).MapEnumerable<FranchiseeFinderFeeDetailViewModel, vw_F_FinderFeeDetailList>().ToList();
                }

                if (lstFranchiseeFinderFeeDetailViewModel.Count > 0)
                {
                    return lstFranchiseeFinderFeeDetailViewModel;
                }
                else
                {
                    FranchiseeFinderFeeDetailViewModel oFranchiseeFinderFeeDetailViewModel = new FranchiseeFinderFeeDetailViewModel();

                    Franchisee oFranchisee = context.Franchisees.SingleOrDefault(f => f.FranchiseeId == franchiseeid);
                    if (oFranchisee != null)
                    {
                        oFranchiseeFinderFeeDetailViewModel.FranchiseeId = oFranchisee.FranchiseeId;
                        oFranchiseeFinderFeeDetailViewModel.FranchiseeNo = oFranchisee.FranchiseeNo;
                        oFranchiseeFinderFeeDetailViewModel.FranchiseeName = oFranchisee.Name;

                        oFranchiseeFinderFeeDetailViewModel.CustomerId = 0;
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



        #endregion

        #region Customer Status Update Approve/Reject  
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

        public int UpdateApproveReject(int FranchiseeId, string Note, int StatusListId, string valIds)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var Franchisee = GetFranchiseeById(FranchiseeId);
            if (Franchisee != null)
            {
                int StatusId = (Franchisee.StatusId.HasValue ? Franchisee.StatusId.Value : 0);
                if (StatusId > 0)
                {
                    var StatusData = GetStatusById(StatusId);
                    if (StatusData != null)
                    {
                        #region :: Franchisee  ::

                        // Update Status as Inactive                         
                        StatusData.IsActive = false;
                        SaveStatus(StatusData);

                        // Add New Status as Approve/Reject
                        Status Statusmodel = new Status();
                        Statusmodel.ClassId = FranchiseeId;
                        Statusmodel.StatusListId = StatusListId;
                        Statusmodel.StatusDate = DateTime.Now;
                        Statusmodel.StatusNotes = Note;
                        Statusmodel.IsActive = true;
                        Statusmodel.CreatedBy = LoginUserId.ToString();
                        Statusmodel.CreatedDate = DateTime.Now;
                        Statusmodel.TypeListId = (int)Business.Enumeration.TypeList.Franchisee;
                        Uow.Status.Add(Statusmodel);
                        Uow.Commit();

                        //Updat Customer Status
                        Franchisee.StatusId = Statusmodel.StatusId;
                        Franchisee.StatusListId = StatusListId;

                        if (Franchisee.IsActive == null)
                            Franchisee.IsActive = true; // not sure if this should be here but there was an issue with IsActive=null for new franchisees

                        Uow.Franchisee.Update(Franchisee);
                        Uow.Commit();

                        #endregion

                        if (StatusListId == 12) // only Pending status with validation item data save 
                        {
                            #region :: validation update :: 

                            if (!string.IsNullOrEmpty(valIds?.Trim()))
                            {
                                string[] arrId = valIds.Split(',');
                                foreach (var item in arrId)
                                {
                                    if (item != "")
                                    {
                                        Validation Validationmodel = new Validation();
                                        Validationmodel.ClassId = FranchiseeId;
                                        Validationmodel.ValidationItemId = Convert.ToInt32(item);
                                        Validationmodel.TypeListId = (int)Business.Enumeration.TypeList.Franchisee;
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
            }
            return FranchiseeId;
        }

        public int UpdateFranchiseeTempStatus(int FranchiseeId, string Note, int StatusListId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var Franchisee = GetFranchiseeById_Temp(FranchiseeId);
            if (Franchisee != null)
            {
                // Add New Status as Approve/Reject
                Status Statusmodel = new Status();
                Statusmodel.ClassId = FranchiseeId;
                Statusmodel.StatusListId = StatusListId;
                Statusmodel.StatusDate = DateTime.Now;
                Statusmodel.StatusNotes = Note;
                Statusmodel.IsActive = true;
                Statusmodel.CreatedBy = LoginUserId.ToString();
                Statusmodel.CreatedDate = DateTime.Now;
                Statusmodel.TypeListId = (int)Business.Enumeration.TypeList.Franchisee;
                Uow.Status.Add(Statusmodel);
                Uow.Commit();

                //Updat Customer Status
                Franchisee.StatusId = Statusmodel.StatusId;
                Franchisee.StatusListId = StatusListId;

                Uow.Franchisee_Temp.Update(Franchisee);
                Uow.Commit();

            }
            return FranchiseeId;
        }

        public Status GetLegalComplianceStatusByFranchiseeid(int Franchiseeid, int StatusListId)
        {
            Status StatusModel = new Status();
            var model = Uow.Status.GetAll().Where(w => w.ClassId == Franchiseeid && w.StatusListId == StatusListId && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee);
            if (model != null && model.Count() > 0)
            {
                StatusModel = model.FirstOrDefault();
            }
            return StatusModel;
        }
        public void UpdateLegalComplianceNote(int LegalComplianceStatuId, string LegalComplianceNote)
        {
            Status StatusModel = new Status();
            var model = Uow.Status.GetById(LegalComplianceStatuId);
            if (model != null)
            {
                model.StatusNotes = LegalComplianceNote;
                Uow.Status.Update(model);
                Uow.Commit();
            }
        }

        public void savePendingMessageForLegal(string message, int customerID, int status)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.SpInsertPendingData(int.Parse(ClaimView.GetCLAIM_USERID()), message, status == 14 ? true : false, status == 9 ? true : false, customerID, DateTime.Now, "FranchiseeLegal", null
                    , null, null, null, null, null);
            }
        }

        public void savePendingMessage(string message, int customerID, int status)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.SpInsertPendingData(int.Parse(ClaimView.GetCLAIM_USERID()), message, status == 14 ? true : false, status == 9 ? true : false, customerID, DateTime.Now, "Franchisee", null
                    , null, null, null, null, null);
            }
        }

        #endregion


        public List<FranchiseeCustomerModel> GetFranchiseeCustomerList(int FranchiseeId)
        {
            using (var context = new jkDatabaseEntities())
            {
                List<FranchiseeCustomerModel> FranchiseeCustomerModel = new List<FranchiseeCustomerModel>();
                var result = (from dst in context.Distributions
                              join ct in context.Customers on dst.CustomerId equals ct.CustomerId
                              where dst.FranchiseeId == FranchiseeId && dst.isActive == true
                              select new FranchiseeCustomerModel
                              {
                                  CustomerId = ct.CustomerId,
                                  Name = ct.Name,
                                  CustomerNo = ct.CustomerNo
                              }).Distinct().ToList();
                if (result != null && result.Count() > 0)
                {
                    FranchiseeCustomerModel = result;
                }

                return FranchiseeCustomerModel;
            }
        }
        public FranchiseeCustomerModel GetCustomerDetails(int CustomerId)
        {
            FranchiseeCustomerModel FranchiseeCustomerModel = new FranchiseeCustomerModel();
            using (var context = new jkDatabaseEntities())
            {
                var response = context.portal_spGet_CustomerDetail(CustomerId);
                foreach (var a in response.ToList())
                {
                    FranchiseeCustomerModel.CustomerId = (a.CustomerId.HasValue ? a.CustomerId.Value : 0);
                    FranchiseeCustomerModel.Name = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    FranchiseeCustomerModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    FranchiseeCustomerModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    FranchiseeCustomerModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                }
            }
            return FranchiseeCustomerModel;
        }

        public IEnumerable<FinderFeeBillDetailViewModel> GetFinderFeeBillDetail(string TrNo)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.spGet_Franchisee_Finderfee_BillDetailByFranchisee(TrNo).MapEnumerable<FinderFeeBillDetailViewModel, spGet_Franchisee_Finderfee_BillDetailByFranchisee_Result>();
            }
        }

        public IEnumerable<FMTDetailViewModel> GetFMTDetail(string TrNo)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.spGet_Franchisee_ManualTransaction(TrNo).MapEnumerable<FMTDetailViewModel, spGet_Franchisee_ManualTransaction_Result>();
            }
        }

        public IEnumerable<LeaseBillDetailViewModel> GetLeaseBillReportDetail(string TrNo)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.portal_spGet_F_LeaseBillReport_By_FranchiseeId(TrNo).MapEnumerable<LeaseBillDetailViewModel, portal_spGet_F_LeaseBillReport_By_FranchiseeId_Result>();
            }
        }
        public bool CheckFranchiseeIsExist(string Name)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Franchisees.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                return true;
            }
            return false;
        }
        public bool CheckFranchiseeIsExistTemp(string Name)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Franchisee_Temp.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
        #region Franchise Report.....
        public IEnumerable<MasterTrxTypeList> GetMasterTrxTypeListForFranchise()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_MasterTrxTypeListForFranchise";

                return conn.Query<MasterTrxTypeList>(query);
            }
        }

        public IEnumerable<FranchiseeWiseTransactionViewModel> GetAllFranchiseTransactions(string franchiseName, int? franchiseId, int? regionId, int? recordNumber, int? masterTrxTypeId, DateTime? spnStartDate, DateTime? spnEndDate, int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_FranchiseWiseTransactionReport @FranchiseName,@FranchiseId,@RegionId,@RecordNumber,@MasterTrxTypeId,@SpnStartDate,@SpnEndDate,@WhereConditionInner,@WhereConditionOuter,@BillMonth, @BillYear";
                var whereConditionInner = new StringBuilder(); //All condition except page number (recordNumber)
                var whereConditionOuter = new StringBuilder(); //page number (recordNumber)

                return conn.Query<FranchiseeWiseTransactionViewModel>(query, new
                {
                    FranchiseName = franchiseName,
                    FranchiseId = franchiseId > 0 ? franchiseId : null,
                    RegionId = regionId > 0 ? regionId : null,
                    RecordNumber = recordNumber > 0 ? recordNumber : null,
                    MasterTrxTypeId = masterTrxTypeId > 0 ? masterTrxTypeId : null,
                    SpnStartDate = spnStartDate,
                    SpnEndDate = spnEndDate,
                    WhereConditionInner = whereConditionInner.ToString(),
                    WhereConditionOuter = whereConditionOuter.ToString(),
                    BillMonth = month,
                    BillYear = year,

                });
            }
        }

        public IEnumerable<portal_spGetChargeBackList_Result> GetChargeBackList(int? regionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGetChargeBackList(regionId ?? SelectedRegionId).ToList();
                return list;
            }
        }

        public IEnumerable<portal_spGet_F_ChargebackListForFranchisee_Result> GetFranchiseeChargebacks(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGet_F_ChargebackListForFranchisee(franchiseId, spnStartDate, spnEndDate).ToList();
                return list;
            }
        }

        public IEnumerable<Franchisee> GetRegionWiseFranchaise(string franchiseName, int? regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = new StringBuilder();
                franchiseName = String.IsNullOrEmpty(franchiseName) ? "'%%'" : "'%" + franchiseName.ToLower() + "%'";
                query.Append("SELECT FranchiseeId,FranchiseeNo,Name,StatusId,StatusListId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RegionId,dlr_id FROM dbo.Franchisee WHERE LOWER(Name+FranchiseeNo) LIKE " + franchiseName + " ");
                if (regionId > 0)
                {
                    query.Append(" AND RegionId=" + regionId + " ");
                }

                return conn.Query<Franchisee>(query.ToString());
            }
        }

        public IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> GetFranchiseeChargebacksData(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = "EXEC portal_spGet_F_FranchiseeWiseChargebackReport @FranchiseeId,@StartDate,@EndDate";
                var result = conn.Query<portal_spGet_F_ChargebackListForFranchiseeViewModel>(query.ToString(), new
                {
                    FranchiseeId = franchiseId,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate
                });

                return result;
            }
        }

        public decimal GetFranchiseeBalanceAsOfDate(int? franchiseId, DateTime? spnStartDate)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = "SELECT * FROM fn_GetFranchiseeBalanceAsOfDate (@FranchiseId,@AsOfDate)";
                var result = conn.Query<FranchiseeInitialBalanceViewModel>(query.ToString(), new
                {
                    FranchiseId = franchiseId,
                    AsOfDate = spnStartDate
                }).FirstOrDefault();

                return result != null ? result.Balance : 0;
            }
        }


        public List<VendorInvoiceList> GetVendorInvoiceList(string statusListIds = "", string regionId = "")
        {
            List<VendorInvoiceList> lstVendorInvoiceList = new List<VendorInvoiceList>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@StatusListId", statusListIds);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_CO_VendorSearchList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstVendorInvoiceList = multipleresult.Read<VendorInvoiceList>().ToList();
                    }
                }
            }
            return lstVendorInvoiceList;
        }

        public FranchiseeBillingPayInfoViewModel GetFranchiseeBillingPayInfoByInvoiceNo(int RegionId, string InvoiceNo, int FranchiseeId)
        {
            FranchiseeBillingPayInfoViewModel oFranchiseeBillingPayInfoViewModel = new FranchiseeBillingPayInfoViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", RegionId);
                parmas.Add("@FranchiseeId", FranchiseeId);
                parmas.Add("@InvoiceNo", InvoiceNo);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_GETFranchiseeBillingPayIdByInvoiceNo", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oFranchiseeBillingPayInfoViewModel = multipleresult.Read<FranchiseeBillingPayInfoViewModel>().ToList().FirstOrDefault();
                        oFranchiseeBillingPayInfoViewModel.lstFranchiseeFee = multipleresult.Read<FranchiseeFeeViewModel>().ToList();
                    }
                }
            }
            return oFranchiseeBillingPayInfoViewModel;
        }

        public IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> ChargeBackDetailPopUp(string trNo)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = "EXEC portal_spGet_F_FranchiseeChargebackDetailsForPopup @TransactionNumber";
                var result = conn.Query<portal_spGet_F_ChargebackListForFranchiseeViewModel>(query.ToString(), new
                {
                    TransactionNumber = trNo
                });

                return result;
            }
        }

        public CommonFranchiseeAccountHistoryReportViewModel GetFranchiseeAccountHistoryReport(int FranchiseeId)
        {
            CommonFranchiseeAccountHistoryReportViewModel oCommonFranchiseeAccountHistory = new CommonFranchiseeAccountHistoryReportViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", FranchiseeId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_GetFranchiseeAccountHistoryReport", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        oCommonFranchiseeAccountHistory.FranchiseeDetail = multipleresult.Read<FranchiseeDetailViewModel>().ToList().FirstOrDefault();
                        oCommonFranchiseeAccountHistory.lstFranchiseeAccountHistoryReport = multipleresult.Read<FranchiseeAccountHistoryReportViewModel>().ToList();
                    }
                    else
                    {
                        oCommonFranchiseeAccountHistory.FranchiseeDetail = new FranchiseeDetailViewModel();
                        oCommonFranchiseeAccountHistory.lstFranchiseeAccountHistoryReport = new List<FranchiseeAccountHistoryReportViewModel>();
                    }
                }
                return oCommonFranchiseeAccountHistory;
            }
        }

        public List<FranchiseeObligationViewModel> GetFranchiseeObligationList(int Id)
        {
            List<FranchiseeObligationViewModel> lstFranchiseeObligation = new List<FranchiseeObligationViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", Id);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_ObligationListForFranchisee", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeObligation = multipleresult.Read<FranchiseeObligationViewModel>().ToList();
                    }
                }
            }
            return lstFranchiseeObligation;
        }


        public List<FranchiseeRevenuesResultViewModel> GetFranchiseeRevenuesReportData(string regionId, int periodid)
        {
            List<FranchiseeRevenuesResultViewModel> lstFranchiseeRevenue = new List<FranchiseeRevenuesResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@PeriodId", periodid);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_GetFranchiseeRevenuesResult", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstFranchiseeRevenue = multipleresult.Read<FranchiseeRevenuesResultViewModel>().ToList();
                    }
                }
            }
            return lstFranchiseeRevenue;
        }

        public DataTable GetFranchiseeDeductionReportData(string regionId, int periodid)
        {

            List<FranchiseeListViewModel> FranchiseeViewModelLst = new List<FranchiseeListViewModel>();
            using (SqlConnection con = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("dbo.portal_spGet_F_GetFranchiseeDeductionData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString()));
                cmd.Parameters.Add(new SqlParameter("@PeriodId", periodid));
                SqlDataAdapter adap = new SqlDataAdapter(cmd);



                //con.Open();
                //SqlCommand cmd = new SqlCommand(string.Format("exec  '{1}',{0}", (), periodid), con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //SqlDataAdapter adap = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                return ds.Tables[0];

                //if (ds.Tables[0].Rows.Count>0)
                //{
                //    FranchiseeViewModelLst = ds.Tables[0].AsEnumerable().Select(r => new FranchiseeViewModelList
                //    {
                //        Name = r.Field<string>("Name"),
                //        Age = r.Field<int>("Age")
                //    });
                //}
            }
            //using (IDbConnection db = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{
            //    var parmas = new DynamicParameters();
            //    parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
            //    parmas.Add("@PeriodId", periodid);
            //    var multipleresult = db.Query("dbo.portal_spGet_F_GetFranchiseeDeductionData", parmas, commandType: CommandType.StoredProcedure).ToList<dynamic>();

            //    //return multipleresult.ToList();
            //    DataTable dt = ToDataTableG(multipleresult);                
            //    return dt;
            //}



            //DataTable lstFranchiseeRevenue = new DataTable();
            //using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //{

            //    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_F_GetFranchiseeDeductionData", parmas, commandType: CommandType.StoredProcedure))
            //    {
            //        if (multipleresult != null)
            //        {
            //            lstFranchiseeRevenue = multipleresult.Read();
            //        }
            //    }
            //}
            //return lstFranchiseeRevenue;
        }
        private DataTable ToDataTableG<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public FranchiseeDashboardModel GetFranchiseeDashboardData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var model = new FranchiseeDashboardModel();
                var query = @"EXEC spGet_FR_DashboardData @QueryType,@RegionId,@StartDate,@EndDate,NULL,@BillMonth,@BillYear"; //1 for chart one data
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }
                decimal totalRevenue = conn.Query<decimal>(query, new
                {
                    QueryType = 1,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();
                model.TotalRevenue = totalRevenue > 0 ? totalRevenue : 0;

                decimal totalDeduction = conn.Query<decimal>(query, new
                {
                    QueryType = 2,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalDeduction = totalDeduction > 0 ? totalDeduction : 0;

                int totalActiveFranchisee = conn.Query<int>(query, new
                {
                    QueryType = 3,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalActiveFranchisee = totalActiveFranchisee > 0 ? totalActiveFranchisee : 0;

                int totalNewFranchisee = conn.Query<int>(query, new
                {
                    QueryType = 4,
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    BillMonth = billMonth,
                    BillYear = billYear
                }).FirstOrDefault();

                model.TotalNewFranchisee = totalNewFranchisee > 0 ? totalNewFranchisee : 0;

                return model;
            }
        }

        public IEnumerable<FranchiseeDashboardModel> GetRevenueWiseTopFranchiseeChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_FR_ChartDataForDashboard 1,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
                {
                    regionIds = "0";
                }
                return conn.Query<FranchiseeDashboardModel>(query, new
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
                return vm;
            }
        }

        public IEnumerable<FranchiseeDashboardModel> GetFranchiseRevenueByMonthChartData(int recordNumber, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC spGet_FR_ChartDataForDashboard 2,@RegionId,@StartDate,@EndDate,@RowNumber,@BillMonth,@BillYear";
                if (string.IsNullOrEmpty(regionId) || regionId == "null" || regionId == null)
                {
                    regionId = "0";
                }
                return conn.Query<FranchiseeDashboardModel>(query, new
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

        /*
        IQueryable<Franchisee> IFranchiseeService.GetFranchisee()
        {
            throw new NotImplementedException();
        }

        IQueryable<Address> IFranchiseeService.GetAddress(int id)
        {
            throw new NotImplementedException();
        }

        Franchisee IFranchiseeService.GetFranchiseeById(int id)
        {
            throw new NotImplementedException();
        }

        Franchisee IFranchiseeService.SaveFranchisee(Franchisee Franchisee)
        {
            throw new NotImplementedException();
        }

        Franchisee IFranchiseeService.DeleteFranchisee(int id)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeOwner> IFranchiseeService.GetFranchiseeOwnerById(int id, int TypeListId, int ContactTypeListId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FullFranchiseeViewModel> IFranchiseeService.GetFranchiseeDetailsByStatus(int StatusId, int TypeListId, int ContactTypeListId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FranchiseeListViewModel> IFranchiseeService.GetFranchiseeList(string status, int? regionId)
        {
            throw new NotImplementedException();
        }

        FranchiseeDetailViewModel IFranchiseeService.GetFranchiseeDetail(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        TaxRateViewModel IFranchiseeService.GetTaxRateDetail(int ClassId, int TypeListId, int AddressId)
        {
            throw new NotImplementedException();
        }

        List<FindersFeeScheduleViewModel> IFranchiseeService.GetFindersFeeScheduleListData()
        {
            throw new NotImplementedException();
        }

        portal_spGet_AR_FranchiseeDetail_Result IFranchiseeService.GetFranchiseeDetailData(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeTransactionTypeList> IFranchiseeService.GetFranchiseeTransactionTypeList()
        {
            throw new NotImplementedException();
        }

        List<ServiceTypeList> IFranchiseeService.GetServiceTypeList()
        {
            throw new NotImplementedException();
        }

        List<StatusList> IFranchiseeService.GetStatusList()
        {
            throw new NotImplementedException();
        }

        List<FranchiseeManualTrxCreditReasonList> IFranchiseeService.GetAll_ReasonList()
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.CreateFranchiseeManualTrasactionSave(FranchiseeTransactionViewModel inputData)
        {
            throw new NotImplementedException();
        }

        FranchiseeTransactionViewModel IFranchiseeService.GetFranchiseeManualTrasactionForEdit(int Id)
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.GetFranchiseeManualTrasactionForDelete(int Id)
        {
            throw new NotImplementedException();
        }

        List<VendorViewModel> IFranchiseeService.GetVendorList()
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.SaveFranchiseeManualTrasactionForEdit(FranchiseeTransactionViewModel inputData, bool IsApprove)
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.SaveFranchiseeManualTrasactionForApproved(string Id, bool IsApproved)
        {
            throw new NotImplementedException();
        }

        string IFranchiseeService.getfranchiseeno()
        {
            throw new NotImplementedException();
        }

        RegionConfiguration IFranchiseeService.GetRegionConfigurationbyId(int id)
        {
            throw new NotImplementedException();
        }

        RegionConfiguration IFranchiseeService.SaveRegionConfiguration(RegionConfiguration RegionConfiguration)
        {
            throw new NotImplementedException();
        }

        List<SearchDateList> IFranchiseeService.GetAll_OptionList()
        {
            throw new NotImplementedException();
        }

        List<TransactionStatusList> IFranchiseeService.GetAll_TransactionStatusList()
        {
            throw new NotImplementedException();
        }

        List<FindersFeeTypeList> IFranchiseeService.GetAll_FindersFeeTypeList()
        {
            throw new NotImplementedException();
        }

        List<FranchiseeSearchModel> IFranchiseeService.GetSearchFranchisee(string searchText)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FranchiseeListViewModel> IFranchiseeService.GetFranchiseeListData(int RegionId)
        {
            throw new NotImplementedException();
        }

        List<FranchiseePendingMaintenanceListViewModel> IFranchiseeService.GetFranchiseePendingMaintenanceList()
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.GetFranchiseePendingMaintenanceApproved(string ids, bool IsApproved)
        {
            throw new NotImplementedException();
        }

        CommonFranchiseeCustomerViewModel IFranchiseeService.GetFranchiseeCustomerDistributionDataForEdit(int MaintenanceTempId)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeBillSetting> IFranchiseeService.GetFranchiseeBillSettings()
        {
            throw new NotImplementedException();
        }

        FranchiseeDashboardModel IFranchiseeService.GetFranchiseeDashboardData(string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FranchiseeDashboardModel> IFranchiseeService.GetFranchiseRevenueByMonthChartData(int recordNumber, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            throw new NotImplementedException();
        }

        FranchiseeBillSetting IFranchiseeService.GetFranchiseeBillSettingsById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeBillSetting IFranchiseeService.SaveFranchiseeBillSettings(FranchiseeBillSetting FranchiseeBillSettings)
        {
            throw new NotImplementedException();
        }

        FranchiseeBillSetting IFranchiseeService.DeleteFranchiseeBillSettings(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FranchiseeDashboardModel> IFranchiseeService.GetRevenueWiseTopFranchiseeChartData(int recordNumber, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FinderFeeBillDetailViewModel> IFranchiseeService.GetFinderFeeBillDetail(string TrNo)
        {
            throw new NotImplementedException();
        }

        IEnumerable<LeaseBillDetailViewModel> IFranchiseeService.GetLeaseBillReportDetail(string TrNo)
        {
            throw new NotImplementedException();
        }

        IQueryable<IdentifierTypeList> IFranchiseeService.GetIdentifierTypeList()
        {
            throw new NotImplementedException();
        }

        IdentifierTypeList IFranchiseeService.GetIdentifierTypeListById(int id)
        {
            throw new NotImplementedException();
        }

        IdentifierTypeList IFranchiseeService.SaveIdentifierTypeList(IdentifierTypeList IdentifierTypeList)
        {
            throw new NotImplementedException();
        }

        IdentifierTypeList IFranchiseeService.DeleteIdentifierTypeList(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<ACHBank> IFranchiseeService.GetACHBank()
        {
            throw new NotImplementedException();
        }

        ACHBank IFranchiseeService.GetACHBankById(int id)
        {
            throw new NotImplementedException();
        }

        ACHBank IFranchiseeService.SaveACHBank(ACHBank ACHBank)
        {
            throw new NotImplementedException();
        }

        ACHBank IFranchiseeService.DeleteACHBank(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeFullfillment> IFranchiseeService.GetFranchiseeFullfillment()
        {
            throw new NotImplementedException();
        }

        FranchiseeFullfillment IFranchiseeService.GetFranchiseeFullfillmentById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeFullfillment IFranchiseeService.SaveFranchiseeFullfillment(FranchiseeFullfillment FranchiseeFullfillment)
        {
            throw new NotImplementedException();
        }

        FranchiseeFullfillment IFranchiseeService.DeleteFranchiseeFullfillment(int id)
        {
            throw new NotImplementedException();
        }

        int IFranchiseeService.GetFullfillmentWithFranchisee(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeContract> IFranchiseeService.GetFranchiseeContract()
        {
            throw new NotImplementedException();
        }

        FranchiseeContract IFranchiseeService.GetFranchiseeContractById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeContract IFranchiseeService.SaveFranchiseeContract(FranchiseeContract FranchiseeContract)
        {
            throw new NotImplementedException();
        }

        FranchiseeContract IFranchiseeService.DeleteFranchiseeContract(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeContractTypeList> IFranchiseeService.GetFranchiseeContractTypeList()
        {
            throw new NotImplementedException();
        }

        FranchiseeContractTypeList IFranchiseeService.GetFranchiseeContractTypeListById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeContractTypeList IFranchiseeService.SaveFranchiseeContractTypeList(FranchiseeContractTypeList FranchiseeContractTypeList)
        {
            throw new NotImplementedException();
        }

        FranchiseeContractTypeList IFranchiseeService.DeleteFranchiseeContractTypeList(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeFee> IFranchiseeService.GetFranchiseeFee()
        {
            throw new NotImplementedException();
        }

        FranchiseeFee IFranchiseeService.GetFranchiseeFeeById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeFee IFranchiseeService.SaveFranchiseeFee(FranchiseeFee Fee)
        {
            throw new NotImplementedException();
        }

        FranchiseeFee IFranchiseeService.DeleteFranchiseeFee(int id)
        {
            throw new NotImplementedException();
        }

        List<FeeFranchiseeFeeRateTypeListCollectionViewModel> IFranchiseeService.GetFeeListCollectionAll(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        void IFranchiseeService.AddDefaultFranchiseeFees(int FranchiseeId, int FeesId, decimal Amount)
        {
            throw new NotImplementedException();
        }

        IQueryable<CusFee> IFranchiseeService.GetFees()
        {
            throw new NotImplementedException();
        }

        CusFee IFranchiseeService.GetFeesById(int id)
        {
            throw new NotImplementedException();
        }

        CusFee IFranchiseeService.SaveFees(CusFee Fees)
        {
            throw new NotImplementedException();
        }

        CusFee IFranchiseeService.DeleteFees(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<FeeRate> IFranchiseeService.GetFeeRate()
        {
            throw new NotImplementedException();
        }

        FeeRate IFranchiseeService.GetFeeRateById(int id)
        {
            throw new NotImplementedException();
        }

        FeeRate IFranchiseeService.SaveFeeRate(FeeRate FeeRate)
        {
            throw new NotImplementedException();
        }

        FeeRate IFranchiseeService.DeleteFeeRate(int id)
        {
            throw new NotImplementedException();
        }

        IQueryable<FranchiseeFeeList> IFranchiseeService.GetFranchiseeFeeList()
        {
            throw new NotImplementedException();
        }

        FranchiseeFeeList IFranchiseeService.GetFranchiseeFeeListById(int id)
        {
            throw new NotImplementedException();
        }

        FranchiseeFeeList IFranchiseeService.SaveFranchiseeFeeList(FranchiseeFeeList FranchiseeFeeList)
        {
            throw new NotImplementedException();
        }

        FranchiseeFeeList IFranchiseeService.DeleteFranchiseeFeeList(int id)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeFeeListFeeRateTypeListCollectionViewModel> IFranchiseeService.GetFranchiseeFeeListFeeRateTypeListCollection()
        {
            throw new NotImplementedException();
        }

        List<FeesViewModel> IFranchiseeService.GetFeeListwithFranchiseeId(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        List<FeeFranchiseeFeeRateTypeListCollectionViewModel> IFranchiseeService.GetFeeListCollection(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        IQueryable<FeeRateTypeList> IFranchiseeService.GetFeeRateTypeList()
        {
            throw new NotImplementedException();
        }

        FeeRateTypeList IFranchiseeService.GetFeeRateTypeListById(int id)
        {
            throw new NotImplementedException();
        }

        FeeRateTypeList IFranchiseeService.SaveFeeRateTypeList(FeeRateTypeList FeeRateTypeList)
        {
            throw new NotImplementedException();
        }

        FeeRateTypeList IFranchiseeService.DeleteFeeRateTypeList(int id)
        {
            throw new NotImplementedException();
        }

        Lease IFranchiseeService.SaveLease(Lease Lease)
        {
            throw new NotImplementedException();
        }

        FranchiseeDistributionDetailsModel IFranchiseeService.GetFranchiseeDistributionDetails(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        CommonFranchiseeCustomerViewModel IFranchiseeService.GetFranchiseeCustomerDistributionData(int CustomerId, int FranchiseeId, int ContractDetailDistributionLineNo)
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.InsertFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData, int TypeListId)
        {
            throw new NotImplementedException();
        }

        bool IFranchiseeService.UpdateFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData)
        {
            throw new NotImplementedException();
        }

        IEnumerable<FMTDetailViewModel> IFranchiseeService.GetFMTDetail(string TrNo)
        {
            throw new NotImplementedException();
        }

        IQueryable<MasterTrxTypeList> IFranchiseeService.GetUsMasterTrxTypeList()
        {
            throw new NotImplementedException();
        }

        List<portal_spGet_F_FindersFeeList_Result> IFranchiseeService.GetFinderFeeList(string regionIds, string statusIds)
        {
            throw new NotImplementedException();
        }

        List<portal_spGet_F_FindersFeeDetailList_Result> IFranchiseeService.GetFinderFeeDetailList(int franchiseeid, string regionIds, string statusIds)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeFinderFeeDetailViewModel> IFranchiseeService.GetFranchiseeFinderFeeDetailList(int regionid, int franchiseeid)
        {
            throw new NotImplementedException();
        }

        List<portal_spGet_F_GetFinderFeeCustomersList_Result> IFranchiseeService.GetFinderFeeCustomersList(int regionid)
        {
            throw new NotImplementedException();
        }

        int IFranchiseeService.UpdateApproveReject(int FranchiseeId, string Note, int StatusListId)
        {
            throw new NotImplementedException();
        }

        void IFranchiseeService.savePendingMessageForLegal(string message, int customerID, int status)
        {
            throw new NotImplementedException();
        }

        void IFranchiseeService.savePendingMessage(string message, int customerID, int status)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeCustomerModel> IFranchiseeService.GetFranchiseeCustomerList(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        FranchiseeCustomerModel IFranchiseeService.GetCustomerDetails(int CustomerId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<MasterTrxTypeList> IFranchiseeService.GetMasterTrxTypeListForFranchise()
        {
            throw new NotImplementedException();
        }

        IEnumerable<FranchiseeWiseTransactionViewModel> IFranchiseeService.GetAllFranchiseTransactions(string franchiseName, int? franchiseId, int? regionId, int? recordNumber, int? masterTrxTypeId, DateTime? spnStartDate, DateTime? spnEndDate, int month, int year)
        {
            throw new NotImplementedException();
        }

        IEnumerable<portal_spGet_F_ChargebackListForFranchisee_Result> IFranchiseeService.GetFranchiseeChargebacks(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Franchisee> IFranchiseeService.GetRegionWiseFranchaise(string franchiseName, int? regionId)
        {
            throw new NotImplementedException();
        }

        decimal IFranchiseeService.GetFranchiseeBalanceAsOfDate(int? franchiseId, DateTime? spnStartDate)
        {
            throw new NotImplementedException();
        }

        List<VendorInvoiceList> IFranchiseeService.GetVendorInvoiceList(string statusListIds, string regionId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> IFranchiseeService.GetFranchiseeChargebacksData(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate)
        {
            throw new NotImplementedException();
        }

        Status IFranchiseeService.GetLegalComplianceStatusByFranchiseeid(int Franchiseeid, int StatusListId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> IFranchiseeService.ChargeBackDetailPopUp(string trNo)
        {
            throw new NotImplementedException();
        }



        bool IFranchiseeService.CheckFranchiseeIsExist(string Name)
        {
            throw new NotImplementedException();
        }

        CommonFranchiseeAccountHistoryReportViewModel IFranchiseeService.GetFranchiseeAccountHistoryReport(int FranchiseeId)
        {
            throw new NotImplementedException();
        }

        List<FranchiseeRevenuesResultViewModel> IFranchiseeService.GetFranchiseeRevenuesReportData(string regionId, int periodid)
        {
            throw new NotImplementedException();
        }

        DataTable IFranchiseeService.GetFranchiseeDeductionReportData(string regionId, int periodid)
        {
            throw new NotImplementedException();
        }

        RemitToViewModel IFranchiseeService.GetRemitToForRegion(int regionId)
        {
            throw new NotImplementedException();
        }

       */

        //public DataTable ToDataTable<T>(List<T> items)
        //{

        //    PropertyDescriptorCollection properties =
        //   TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable();
        //    foreach (PropertyDescriptor prop in properties)
        //        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    foreach (T item in items)
        //    {
        //        DataRow row = table.NewRow();
        //        foreach (PropertyDescriptor prop in properties)
        //            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        //        table.Rows.Add(row);
        //    }
        //    return table;
        //}

        #endregion Franchise Report.....

        public int CheckOnlyFranchiseeNamePhoneIsExist(string Name, string Phone)
        {

            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.Franchisees.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            if (data != null && data.Count() > 0)
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return data.FirstOrDefault().FranchiseeId;
                }
                else
                {
                    return data.FirstOrDefault().FranchiseeId;
                }
            }
            else
            {
                var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
                if (PhoneModel != null && PhoneModel.Count() > 0)
                {
                    return -3;
                }
                return -1;
            }

            //jkDatabaseEntities context = new jkDatabaseEntities();
            //var data = context.Franchisees.Where(w => w.Name.ToLower().Trim() == Name.ToLower().Trim() && w.IsActive == true && (w.RegionId == 0 || w.RegionId == SelectedRegionId));
            //if (data != null && data.Count() > 0)
            //{
            //    var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
            //    if (PhoneModel != null && PhoneModel.Count() > 0)
            //    {
            //        return data.FirstOrDefault().FranchiseeId;
            //    }
            //    else
            //    {
            //        return -2;
            //    }
            //}
            //else
            //{
            //    var PhoneModel = context.Phones.Where(w => w.Phone1.Trim() == Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim() && w.IsActive == true && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Main);
            //    if (PhoneModel != null && PhoneModel.Count() > 0)
            //    {
            //        return -3;
            //    }
            //    return -1;
            //}

        }

        public List<portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods_Result> GetAvailableFranchiseeReportFinalizedPeriods(string RegionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods_Result> lstFinalizedPeriods = context.portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods(RegionId).ToList();
                return lstFinalizedPeriods;
            }

        }

        public portal_spGet_AP_FinalizedFranchiseeReportList_Result GetFinalizedFranchiseeReport(int? billMonth = null, int? billYear = null, string regionIds = null, int? frid = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var resultList = context.portal_spGet_AP_FinalizedFranchiseeReportList(regionIds ?? SelectedRegionId.ToString(), billMonth, billYear).ToList();
                return resultList.Where(i => i.FranchiseeId == frid).FirstOrDefault();
            }
        }

        public int? getCheckPeriod(int PeriodClsId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var periodClosed = context.PeriodCloseds.Where(x => x.PeriodClosedId == PeriodClsId).FirstOrDefault();
                var period = context.Periods.Where(o => o.PeriodId == periodClosed.PeriodId).FirstOrDefault();
                return period.BillMonth;
            }
        }

        public int GetFranchiseeLeaseStatus(int Id)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                int StatusId = 0;
                var data = context.Leases.Where(w => w.ClassId == Id && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee);
                if (data != null && data.Count() > 0)
                {
                    if (data.Where(f => f.StatusListId == 21).FirstOrDefault() != null)
                    {
                        StatusId = 21; //Active status
                    }
                    else if (data.Where(f => f.StatusListId == 22).FirstOrDefault() != null)
                    {
                        StatusId = 22; //Complete status
                    }
                    else if (data.Where(f => f.StatusListId == 23).FirstOrDefault() != null)
                    {
                        StatusId = 23; //Stopped status
                    }
                    else if (data.Where(f => f.StatusListId == 24).FirstOrDefault() != null)
                    {
                        StatusId = 24; //Transferred status
                    }
                }
                return StatusId;
            }
        }
        public List<DeclineReasonList> GetDeclineReasonListList()
        {
            List<DeclineReasonList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.DeclineReasonLists.ToList();
                return data;
            }
        }
        public int SaveFranchiseeOfferingData(Offering model)
        {
            using (var context = new jkDatabaseEntities())
            {
                if (model.OfferingId == 0)
                {
                    context.Offerings.Add(model);
                    context.SaveChanges();
                }
                else
                {
                    context.SaveChanges();
                }
                return model.OfferingId;
            }
        }
        public string GetFranchiseeStatus(int Id)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            string strname = "";
            var result = (from fc in context.Franchisees
                          join srl in context.StatusLists on fc.StatusListId equals srl.StatusListId
                          where fc.FranchiseeId == Id
                          select new { srl.Name }).FirstOrDefault();
            if (result != null)
            {
                strname = result.Name.ToString();
            }

            return strname;
        }

        #region :: Franchisee Call :: 
        public List<FranCallModel> GetFranchiseeCall(int FranchiseeId, int CustomerId)
        {
            jkDatabaseEntities context = new Data.DAL.jkDatabaseEntities();
            List<FranCallModel> List = new List<FranCallModel>();

            var result = (from fc in context.FranCalls
                          join srl in context.StatusResultLists on fc.StatusResultListId equals srl.StatusResultListId
                          where fc.FranchiseeId == FranchiseeId && fc.CustomerId == CustomerId
                          select new FranCallModel
                          {
                              FranCallId = fc.FranCallId,
                              RegionId = fc.RegionId,
                              FranchiseeId = fc.FranchiseeId,
                              call_date = fc.call_date,
                              call_time = fc.call_time,
                              call_stat = fc.call_stat,
                              StatusResultListId = fc.StatusResultListId,
                              StatusResultListName = srl.Name,
                              spoke_with = fc.spoke_with,
                              call_back = fc.call_back,
                              call_btime = fc.call_btime,
                              action = fc.action,
                              action_otr = fc.action_otr,
                              init_call = fc.init_call,
                              comments = fc.comments,
                          }).OrderByDescending(o => o.FranCallId).ToList();

            //var DataModel = context.FranCalls.Where(w => w.FranchiseeId == FranchiseeId);
            if (result != null && result.Count() > 0)
            {
                List = result;
                //List= DataModel.MapEnumerable<FranCallModel, FranCall>().ToList();
            }
            return List;
        }

        public int SaveFranCallsDetails(int FranchiseeId, string InitiatedBy, int StatusResultListId, string SpokeWith, string CallAction, string CallBack, string CallBackTime, string Comments, int CustomerId, int IsCallBack)
        {
            jkDatabaseEntities context = new Data.DAL.jkDatabaseEntities();

            FranCall model = new FranCall();
            model.RegionId = SelectedRegionId;
            model.FranchiseeId = FranchiseeId;
            model.CustomerId = CustomerId;
            //if (CallBack != "" && CallBack != null)
            //{
            //    model.call_date = Convert.ToDateTime(CallBack);
            //}
            //if (CallBackTime != "" && CallBackTime != null)
            //{
            //    TimeSpan timeT = DateTime.Parse(CallBackTime).TimeOfDay;
            //    model.call_time = timeT;
            //}
            //model.call_date = DateTime.Now.Date;
            //model.call_time = DateTime.Now.Date.TimeOfDay;
            if (CallBack != "" && CallBack != null)
            {
                model.call_back = CallBack;
            }
            if (CallBackTime != "" && CallBackTime != null)
            {
                model.call_btime = CallBackTime;
            }
            model.call_date = DateTime.Now;
            model.call_time = DateTime.Now.TimeOfDay;
            model.StatusResultListId = StatusResultListId;
            model.spoke_with = SpokeWith;
            model.action = CallAction;
            model.init_call = InitiatedBy.ToString();
            model.comments = Comments;
            model.IsCallBack = (IsCallBack == 1 ? true : false);
            context.FranCalls.Add(model);
            context.SaveChanges();
            return model.FranCallId;
        }

        public FranchiseeBasicInfo GetFranchiseeBasicInfo(int FranchiseeId)
        {
            jkDatabaseEntities context = new Data.DAL.jkDatabaseEntities();
            FranchiseeBasicInfo model = new FranchiseeBasicInfo();
            var FranData = GetFranchiseeById(FranchiseeId);
            if (FranData != null)
            {
                model.FranchiseeNo = FranData.FranchiseeNo;
                model.FranchiseeName = FranData.Name;
            }
            var PhonMode = context.Phones.Where(w => w.ClassId == FranchiseeId && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Contact && w.IsActive == true).OrderByDescending(o => o.PhoneId).FirstOrDefault();
            if (PhonMode != null)
            {
                model.FranchiseePhone = PhonMode.Phone1;
            }
            var ContMode = context.Contacts.Where(w => w.ClassId == FranchiseeId && w.TypeListId == (int)JKApi.Business.Enumeration.TypeList.Franchisee && w.ContactTypeListId == (int)JKApi.Business.Enumeration.ContactTypeList.Contact && w.IsActive == true).OrderByDescending(o => o.ContactId).FirstOrDefault();
            if (ContMode != null)
            {
                model.FranchiseeContact = ContMode.Name;
            }
            return model;
        }
        #endregion

        public void UpdateFranchiseeDetails(int FranchiseeId, string Name)
        {
            var model = Uow.Franchisee.GetById(FranchiseeId);
            if (model != null)
            {
                model.Name = Name;
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = -1;
                Uow.Franchisee.Update(model);
                Uow.Commit();
            }
        }
        public void UpdateFranchiseeStatus(int FranchiseeId, int StatusListId)
        {
            if (FranchiseeId > 0)
            {
                var model = Uow.Franchisee.GetById(FranchiseeId);
                if (model != null)
                {
                    //Add Franchisee Status
                    Status oStatus = new Status();
                    oStatus.ClassId = FranchiseeId;
                    oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                    oStatus.CreatedDate = DateTime.Now;
                    oStatus.IsActive = true;
                    oStatus.StatusDate = DateTime.Now;
                    oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                    oStatus.StatusListId = StatusListId;
                    Uow.Status.Add(oStatus);
                    Uow.Commit();

                    //Update Franchisee
                    model.StatusId = oStatus.StatusId;
                    model.StatusListId = oStatus.StatusListId;
                    Uow.Franchisee.Update(model);
                    Uow.Commit();
                }
            }
        }
        public void UpdateFranchiseeStatus_Temp(int FranchiseeId, int StatusListId)
        {
            if (FranchiseeId > 0)
            {
                var model = Uow.Franchisee_Temp.GetById(FranchiseeId);
                if (model != null)
                {
                    //Add Franchisee Status
                    Status oStatus = new Status();
                    oStatus.ClassId = FranchiseeId;
                    oStatus.CreatedBy = ClaimView.GetCLAIM_USERID();
                    oStatus.CreatedDate = DateTime.Now;
                    oStatus.IsActive = true;
                    oStatus.StatusDate = DateTime.Now;
                    oStatus.TypeListId = (int)JKApi.Business.Enumeration.TypeList.Franchisee;
                    oStatus.StatusListId = StatusListId;
                    Uow.Status.Add(oStatus);
                    Uow.Commit();

                    //Update Franchisee
                    model.StatusId = oStatus.StatusId;
                    model.StatusListId = oStatus.StatusListId;
                    Uow.Franchisee_Temp.Update(model);
                    Uow.Commit();
                }
            }
        }

        public void moveFeeConfigurationDataOldToNewFranchisee(int FranchiseeId, int NewFranchiseeId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", FranchiseeId);
                parmas.Add("@NewFranchiseeId", NewFranchiseeId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spPost_F_MoveFeeConfigurationDataOldToNewFranchisee", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        multipleresult.Read<int>().FirstOrDefault();
                    }
                }
            }
        }
        public int MoveFranchiseeTempDataToRealTable(int FranchiseeId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@FranchiseeId", FranchiseeId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spPost_F_MoveFranchiseeTempDataToRealTable", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        FranchiseeId = multipleresult.Read<int>().FirstOrDefault();
                    }
                }
            }
            return FranchiseeId;
        }
    }
}