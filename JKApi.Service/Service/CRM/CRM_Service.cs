using Dapper;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.Helper.Extension;
using JKApi.Service.ServiceContract.CRM;
using JKViewModels;
using JKViewModels.CRM;
using JKViewModels.CRM.CRMSPModels;
using JKViewModels.Customer;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;

namespace JKApi.Service.Service.CRM
{
    public class CRM_Service : BaseService, ICRM_Service
    {
        private readonly NLogger _nLogger;
        private readonly ICacheProvider _cacheProvider;

        #region CRM_AccountService > Constructor

        public CRM_Service(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
            _nLogger = NLogger.Instance;
        }

        #endregion

        #region CRM_AccountService > Queries

        public IQueryable<CRM_Account> GetAll_CRM_Account()
        {
            return Uow.CRM_Account.GetAll();
        }
        public IQueryable<CRM_CallLog> GetAll_CRM_CallLog()
        {
            return Uow.CRM_CallLog.GetAll();
        }
        public IQueryable<CRM_CloseType> GetAll_CRM_CloseType()
        {
            return Uow.CRM_CloseType.GetAll();
        }
        public IQueryable<CRM_ReasonType> GetAll_CRM_ReasonType()
        {
            return Uow.CRM_ReasonType.GetAll();
        }
        public IQueryable<CRM_PurposeType> GetAll_CRM_PurposeType()
        {
            return Uow.CRM_PurposeType.GetAll();
        }
        public IQueryable<CRM_LeadGeneration> GetAll_CRM_LeadGeneration()
        {
            return Uow.CRM_LeadGeneration.GetAll();
        }

        public IQueryable<CRM_FranchiseFollowUp> GetAll_CRM_FranchiseFollowUp()
        {
            return Uow.CRM_FranchiseFollowUp.GetAll();
        }
        public IQueryable<CRM_SignAgreement> GetAll_CRM_SignAgreement()
        {
            return Uow.CRM_SignAgreement.GetAll();
        }
        public IQueryable<CRM_FranchiseContract> GetAll_CRM_FranchiseContract()
        {
            return Uow.CRM_FranchiseContract.GetAll();
        }
        public IQueryable<CRM_Territory> GetAll_CRM_Territory()
        {
            return Uow.CRM_Territory.GetAll();
        }
        public IQueryable<CRM_CallResult> GetAll_CRM_CallResult()
        {
            return Uow.CRM_CallResult.GetAll();
        }
        public IQueryable<CRM_NoteType> GetAll_CRM_NoteType()
        {
            return Uow.CRM_NoteType.GetAll();
        }
        public IQueryable<CRM_SalePossibilityType> GetAll_CRM_SalePossibilityType()
        {
            return Uow.CRM_SalePossibilityType.GetAll();
        }
        public IQueryable<CRM_ScheduleType> GetAll_CRM_ScheduleType()
        {
            return Uow.CRM_ScheduleType.GetAll();
        }

        public IQueryable<CRM_Territory_Assignment> GetAll_CRM_TerriAssignment()
        {
            return Uow.CRM_Territory_Assignment.GetAll();
        }

        public void UpdateMultiple_CRM_TerriAssignmentNew(ZipCodeAssignmentPopupModel model)
        {
            foreach (int id in model.TerritoryAssignmentIds)
            {
                CRM_Territory_Assignment_New assignment = Uow.CRM_Territory_Assignment_New.GetById(id);
                assignment.CRM_TerritoryId = model.TerritoryId;
                Uow.CRM_Territory_Assignment_New.Update(assignment);
                Uow.Commit();
            }
        }

        public IQueryable<CRM_Territory_New> GetAll_CRM_Territory_New()
        {
            return Uow.CRM_Territory_New.GetAll();
        }
        public int GetCRMTerritoryId(int SelectedRegionId, string Name)
        {
            int Id = 0;
            jkDatabaseEntities context = new jkDatabaseEntities();
            var data = context.CRM_Territory_New.Where(w => w.RegionId == SelectedRegionId && w.Name == Name);
            if (data != null && data.Count() > 0)
            {
                Id = data.FirstOrDefault().CRM_TerritoryId;
            }
            return Id;
        }

        public IQueryable<CRM_Territory_Assignment_New> GetAll_CRM_Territory_Assignment_New()
        {
            return Uow.CRM_Territory_Assignment_New.GetAll();
        }
        public void AddCRMSalesTerritoryAssignment(CRM_SalesTerritory_Assignment assignment)
        {
            Uow.CRM_SalesTerritory_Assignment.Add(assignment);
            Uow.Commit();
        }
        public void DeleteAllCRMSalesTerritoryAssignment()
        {
            //using (var context = new jkDatabaseEntities())
            //{
            //    context.Database.ExecuteSqlCommand("TRUNCATE TABLE CRM_SalesTerritory_Assignment");
            //}


        }
        public void SaveAllCRMSalesTerritoryAssignment(SaveTerritoryAssignmentViewModel model)
        {
            //var getAll = Uow.CRM_SalesTerritory_Assignment.GetAll().ToList();

            var getAll = (from a in Uow.CRM_SalesTerritory_Assignment.GetAll().ToList()
                          join b in Uow.CRM_Territory_New.GetAll().ToList() on a.CRM_TerritoryId equals b.CRM_TerritoryId
                          where b.RegionId == SelectedRegionId
                          select new CRM_SalesTerritory_Assignment { UserId = a.UserId, CRM_TerritoryId = a.CRM_TerritoryId, CRM_SalesTerriAssignmentId = a.CRM_SalesTerriAssignmentId }
                                     ).ToList();

            foreach (var territoryAssignment in getAll)
            {
                if (model != null && model.TerritoryAssignmentList != null && !model.TerritoryAssignmentList.Where(X => X.TerritoryID == territoryAssignment.CRM_TerritoryId && X.UserID == territoryAssignment.UserId).Any())
                {
                    Uow.CRM_SalesTerritory_Assignment.Delete(territoryAssignment.CRM_SalesTerriAssignmentId);
                    Uow.Commit();
                }
            }

            if (model != null && model.TerritoryAssignmentList != null)
            {
                foreach (TerritoryAssignmentModel territoryAssignment in model.TerritoryAssignmentList)
                {
                    if (!getAll.Where(X => X.CRM_TerritoryId == territoryAssignment.TerritoryID && X.UserId == territoryAssignment.UserID).Any())
                    {
                        Uow.CRM_SalesTerritory_Assignment.Add(new CRM_SalesTerritory_Assignment { CreatedOn = DateTime.Now, CreatedBy = LoginUserId, CRM_TerritoryId = territoryAssignment.TerritoryID, UserId = territoryAssignment.UserID });
                        Uow.Commit();
                    }
                }
            }

        }
        public void AddZipCode(AddZipCodePopupModel model)
        {
            if (model != null)
            {
                Uow.CRM_Territory_Assignment_New.Add(new CRM_Territory_Assignment_New { CRM_TerritoryId = model.TerritoryId, ZipCode = model.ZipCode });
                Uow.Commit();
            }
        }
        public void AddTerritory(AddTerritoryPopupModel model)
        {
            if (model != null)
            {
                Uow.CRM_Territory_New.Add(new CRM_Territory_New { Name = model.Name, Description = model.Name, RegionId = SelectedRegionId, IsActive = true, Type = null, });
                Uow.Commit();
            }
        }
        public CRM_Territory_Assignment_New GetCRM_Territory_Assignment_NewByZipCode(string zipCode)
        {
            if (!String.IsNullOrEmpty(zipCode))
            {
                return Uow.CRM_Territory_Assignment_New.GetAll().FirstOrDefault(x => x.ZipCode == zipCode);
            }
            return null;
        }
        public CRM_Territory_New GetCRM_Territory_NewByNameandRegionID(string Name, int RegionID)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                return Uow.CRM_Territory_New.GetAll().Where(x => x.Name == Name && x.RegionId == RegionID).FirstOrDefault();
            }
            return null;
        }
        public IQueryable<CRM_SalesTerritory_Assignment> GetAll_CRM_SalesTerriAssignment()
        {
            return Uow.CRM_SalesTerritory_Assignment.GetAll();
        }
        public IQueryable<CRM_CloseTempDocument> GetAll_CRM_CloseTempDocument()
        {
            return Uow.CRM_CloseTempDocument.GetAll();
        }
        public IQueryable<CRM_Contact> GetAll_CRM_Contact()
        {
            return Uow.CRM_Contact.GetAll();
        }
        public IQueryable<CRM_ContactType> GetAll_CRM_ContactType()
        {
            return Uow.CRM_ContactType.GetAll();
        }

        public IQueryable<CRM_Activity> GetAll_CRM_Activity()
        {
            return Uow.CRM_Activity.GetAll();
        }

        public IQueryable<CRM_TimeLine> GetAll_CRM_TimeLine()
        {
            return Uow.CRM_TimeLine.GetAll();
        }

        public IQueryable<CRM_Note> GetAll_CRM_Note()
        {
            return Uow.CRM_Note.GetAll();
        }

        public IQueryable<CRM_Schedule> GetAll_CRM_Schedule()
        {
            return Uow.CRM_Schedule.GetAll();
        }

        public List<CRM_Schedule> GetAll_CRM_ScheduleWithCustomer(int ClassId)
        {
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Schedule.Where(w=>w.ClassId== ClassId).ToList();
        }

        public IQueryable<CRM_StageStatusSchedule> GetAll_CRM_StageStatusSchedule()
        {
            return Uow.CRM_StageStatusSchedule.GetAll();
        }
        public IQueryable<CRM_Document> GetAll_CRM_Document()
        {
            return Uow.CRM_Document.GetAll();
        }
        public IQueryable<CRM_InitialCommunication> GetAll_CRM_InitialCommunication()
        {
            return Uow.CRM_InitialCommunication.GetAll();
        }
        public IQueryable<CRM_FvPresentation> GetAll_CRM_FvPresentation()
        {
            return Uow.CRM_FvPresentation.GetAll();
        }
        public IQueryable<CRM_Bidding> GetAll_CRM_Bidding()
        {
            return Uow.CRM_Bidding.GetAll();
        }

        public IQueryable<CRM_PdAppointment> GetAll_CRM_PdAppointment()
        {
            return Uow.CRM_PdAppointment.GetAll();
        }

        public IQueryable<CRM_FollowUp> GetAll_CRM_FollowUp()
        {
            return Uow.CRM_FollowUp.GetAll();
        }

        public IQueryable<CRM_Close> GetAll_CRM_Close()
        {
            return Uow.CRM_Close.GetAll();
        }

        //public IQueryable<CRM_StageStatusSchedule> GetAll_CRM_StageStatusSchedule()
        //{
        //    return Uow.CRM_StageStatusSchedule.GetAll();
        //}
        public IQueryable<CRM_Quotation> GetAll_CRM_Quotation()
        {
            return Uow.CRM_Quotation.GetAll();
        }

        public IQueryable<CRM_Task> GetAll_CRM_Task()
        {
            return Uow.CRM_Task.GetAll();
        }

        public IQueryable<CRM_TaskType> GetAll_CRM_TaskType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmTaskTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmTaskTypes, Uow.CRM_TaskType.GetAll());
            }
            return (IQueryable<CRM_TaskType>)_cacheProvider.Get(CacheKeyName.GetCrmTaskTypes);
        }

        public IQueryable<CRM_Stage> GetAll_CRM_Stage()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmStages))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmStages, Uow.CRM_Stage.GetAll());
            }
            return (IQueryable<CRM_Stage>)_cacheProvider.Get(CacheKeyName.GetCrmStages);
        }

        public IQueryable<CRM_StageStatus> GetAll_CRM_StageStatus()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmStageStatus))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmStageStatus, Uow.CRM_StageStatus.GetAll());
            }
            return (IQueryable<CRM_StageStatus>)_cacheProvider.Get(CacheKeyName.GetCrmStageStatus);
        }

        public IQueryable<CRM_IndustryType> GetAll_CRM_IndustryType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmIndustryTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmIndustryTypes, Uow.CRM_IndustryType.GetAll());
            }
            return (IQueryable<CRM_IndustryType>)_cacheProvider.Get(CacheKeyName.GetCrmIndustryTypes);
        }

        public IQueryable<CRM_ProviderSource> GetAll_CRM_ProviderSource()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmProviderSources))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmProviderSources, Uow.CRM_ProviderSource.GetAll());
            }
            return (IQueryable<CRM_ProviderSource>)_cacheProvider.Get(CacheKeyName.GetCrmProviderSources);
        }

        public IQueryable<CRM_ProviderType> GetAll_CRM_ProviderType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmProviderTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmProviderTypes, Uow.CRM_ProviderType.GetAll());
            }
            return (IQueryable<CRM_ProviderType>)_cacheProvider.Get(CacheKeyName.GetCrmProviderTypes);
        }

        public IQueryable<CRM_AccountCustomerDetail> GetAll_CRM_AccountCustomerDetail()
        {
            return Uow.CRM_AccountCustomerDetail.GetAll();
        }

        public IQueryable<CRM_AccountFranchiseDetail> GetAll_CRM_AccountFranchiseDetail()
        {
            return Uow.CRM_AccountFranchiseDetail.GetAll();
        }
        public IQueryable<CRM_AccountType> GetAll_CRM_AccountType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmAccountTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmAccountTypes, Uow.CRM_AccountType.GetAll());
            }
            return (IQueryable<CRM_AccountType>)_cacheProvider.Get(CacheKeyName.GetCrmAccountTypes);
        }

        public IQueryable<CRM_ActivityOutcomeType> GetAll_CRM_ActivityOutcomeType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmActivityOutcomeTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmActivityOutcomeTypes, Uow.CRM_ActivityOutcomeType.GetAll());
            }
            return (IQueryable<CRM_ActivityOutcomeType>)_cacheProvider.Get(CacheKeyName.GetCrmActivityOutcomeTypes);
        }

        public IQueryable<CRM_ActivityType> GetAll_CRM_ActivityType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmActivityTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmActivityTypes, Uow.CRM_ActivityType.GetAll());
            }
            return (IQueryable<CRM_ActivityType>)_cacheProvider.Get(CacheKeyName.GetCrmActivityTypes);
        }

        public IQueryable<CRM_TimeLineType> GetAll_CRM_TimeLineType()
        {
            if (!_cacheProvider.Contains(CacheKeyName.GetCrmTimeLineTypes))
            {
                _cacheProvider.Set(CacheKeyName.GetCrmTimeLineTypes, Uow.CRM_TimeLineType.GetAll());
            }
            return (IQueryable<CRM_TimeLineType>)_cacheProvider.Get(CacheKeyName.GetCrmTimeLineTypes);
        }

        #endregion

        #region CRM_AccountService > Queries Get by Id

        public CRM_CallLog GetCRM_CallLogbyId(int id)
        {
            return Uow.CRM_CallLog.GetById(id);
        }
        public CRM_CloseType GetCRM_CloseTypebyId(int id)
        {
            return Uow.CRM_CloseType.GetById(id);
        }
        public CRM_ReasonType GetCRM_ReasonTypebyId(int id)
        {
            return Uow.CRM_ReasonType.GetById(id);
        }
        public CRM_PurposeType GetCRM_PurposeTypebyId(int id)
        {
            return Uow.CRM_PurposeType.GetById(id);
        }

        public CRM_Account GetCRM_AccountbyId(int id)
        {
            return Uow.CRM_Account.GetById(id);
        }

        public CRM_LeadGeneration GetCRM_LeadGenerationById(int id)
        {
            return Uow.CRM_LeadGeneration.GetById(id);
        }

        public CRM_FranchiseFollowUp GetCRM_FranchiseFollowUpById(int id)
        {
            return Uow.CRM_FranchiseFollowUp.GetById(id);
        }
        public CRM_SignAgreement GetCRM_SignAgreementById(int id)
        {
            return Uow.CRM_SignAgreement.GetById(id);
        }
        public CRM_FranchiseContract GetCRM_FranchiseContractById(int id)
        {
            return Uow.CRM_FranchiseContract.GetById(id);
        }

        public CRM_CloseTempDocument GetCRM_CloseTempDocumentById(int id)
        {
            return Uow.CRM_CloseTempDocument.GetById(id);
        }

        public CRM_Contact GetCRM_ContactById(int id)
        {
            return Uow.CRM_Contact.GetById(id);
        }

        public CRM_Territory GetCRM_TerritoryById(int id)
        {
            return Uow.CRM_Territory.GetById(id);
        }
        public CRM_CallResult GetCRM_CallResultById(int id)
        {
            return Uow.CRM_CallResult.GetById(id);
        }
        public CRM_NoteType GetCRM_NoteTypeById(int id)
        {
            return Uow.CRM_NoteType.GetById(id);
        }
        public CRM_SalePossibilityType GetCRM_SalePossibilityTypeById(int id)
        {
            return Uow.CRM_SalePossibilityType.GetById(id);
        }
        public CRM_ScheduleType GetCRM_ScheduleTypeById(int id)
        {
            return Uow.CRM_ScheduleType.GetById(id);
        }

        public CRM_Territory_Assignment GetCRM_TerriAssignmentyById(int id)
        {
            return Uow.CRM_Territory_Assignment.GetById(id);
        }

        public CRM_SalesTerritory_Assignment GetCRM_SalesTerriAssignmentyById(int id)
        {
            return Uow.CRM_SalesTerritory_Assignment.GetById(id);
        }

        public CRM_ContactType GetCRM_ContactTypeById(int id)
        {
            return Uow.CRM_ContactType.GetById(id);
        }

        public CRM_Note GetCRM_Note_ByAccountCustomerNoteId(int id)
        {
            return Uow.CRM_Note.GetById(id);
        }

        public CRM_Activity GetCRM_ActivityId(int id)
        {
            return Uow.CRM_Activity.GetById(id);
        }

        public CRM_Task GetCRM_TaskbyId(int id)
        {
            return Uow.CRM_Task.GetById(id);
        }

        public CRM_Quotation GetCRM_QuotationbyId(int id)
        {
            return Uow.CRM_Quotation.GetById(id);
        }

        public CRM_Schedule GetCRM_SchedulebyId(int id)
        {
            return Uow.CRM_Schedule.GetById(id);
        }

        public CRM_AccountCustomerDetail GetCRM_AccountCustomerDetailbyId(int id)
        {
            return Uow.CRM_AccountCustomerDetail.GetById(id);

        }

        public CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetailbyId(int id)
        {
            return Uow.CRM_AccountFranchiseDetail.GetById(id);
        }

        public CRM_Document GetCRM_DocumentById(int id)
        {
            return Uow.CRM_Document.GetById(id);
        }

        public CRM_InitialCommunication GetCRM_InitialCommunicationById(int id)
        {
            return Uow.CRM_InitialCommunication.GetById(id);
        }

        public CRM_FvPresentation GetCRM_FvPresentationBydId(int id)
        {
            return Uow.CRM_FvPresentation.GetById(id);
        }

        public CRM_Bidding GetCRM_BiddingById(int id)
        {
            return Uow.CRM_Bidding.GetById(id);
        }

        public CRM_PdAppointment GetCRM_PdAppointmentById(int id)
        {
            return Uow.CRM_PdAppointment.GetById(id);
        }
        public CRM_FollowUp GetCRM_FollowUpById(int id)
        {
            return Uow.CRM_FollowUp.GetById(id);
        }
        public CRM_Close GetCRM_CloseById(int id)
        {
            return Uow.CRM_Close.GetById(id);
        }
        public CRM_StageStatusSchedule GetCRM_StageStatusScheduleById(int id)
        {
            return Uow.CRM_StageStatusSchedule.GetById(id);
        }

        #endregion

        #region CRM_AccountService > Save/Update

        public CRM_CallLog SaveCRM_CallLog(CRM_CallLog CRM_CallLog)
        {
            var ID = CRM_CallLog.CRM_CallLogId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_CallLog.CreatedDate = DateTime.Now;
                Uow.CRM_CallLog.Add(CRM_CallLog);
            }

            else //Update existing entry
            {
                CRM_CallLog.ModifiedDate = DateTime.Now;
                Uow.CRM_CallLog.Update(CRM_CallLog);
            }
            Uow.Commit();
            return CRM_CallLog;
        }
        public CRM_CloseType SaveCRM_CloseType(CRM_CloseType CRM_CloseType)
        {
            var ID = CRM_CloseType.CRM_CloseTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_CloseType.CreatedDate = DateTime.Now;
                Uow.CRM_CloseType.Add(CRM_CloseType);
            }

            else //Update existing entry
            {
                CRM_CloseType.ModifiedDate = DateTime.Now;
                Uow.CRM_CloseType.Update(CRM_CloseType);
            }
            Uow.Commit();
            return CRM_CloseType;
        }
        public CRM_ReasonType SaveCRM_ReasonType(CRM_ReasonType CRM_ReasonType)
        {
            var ID = CRM_ReasonType.CRM_ReasonTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ReasonType.CreatedDate = DateTime.Now;
                Uow.CRM_ReasonType.Add(CRM_ReasonType);
            }

            else //Update existing entry
            {
                CRM_ReasonType.ModifiedDate = DateTime.Now;
                Uow.CRM_ReasonType.Update(CRM_ReasonType);
            }
            Uow.Commit();
            return CRM_ReasonType;
        }
        public CRM_PurposeType SaveCRM_PurposeType(CRM_PurposeType CRM_PurposeType)
        {
            var ID = CRM_PurposeType.CRM_PurposeTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_PurposeType.CreatedDate = DateTime.Now;
                Uow.CRM_PurposeType.Add(CRM_PurposeType);
            }

            else //Update existing entry
            {
                CRM_PurposeType.ModifiedDate = DateTime.Now;
                Uow.CRM_PurposeType.Update(CRM_PurposeType);
            }
            Uow.Commit();
            return CRM_PurposeType;
        }

        public CRM_Account SaveCRM_Account(CRM_Account CRM_Account)
        {

            var ID = CRM_Account.CRM_AccountId;
            CRM_Account.RegionId = CRM_Account.RegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Account.CreatedDate = DateTime.Now;
                Uow.CRM_Account.Add(CRM_Account);
            }
            else //Update existing entry
            {
                CRM_StageStartEnd stageModel = new CRM_StageStartEnd()
                {
                    CreatedDate = DateTime.Now,
                    CreatedBy = CRM_Account.CreatedBy,
                    CRM_AccountId = CRM_Account.CRM_AccountId,
                    CRM_StageStatusId = CRM_Account.StageStatus,
                    StartDate = CRM_Account.ModifiedDate == null ? DateTime.Now : CRM_Account.ModifiedDate,
                    EndDate = DateTime.Now
                };

                CRM_Account.ModifiedDate = DateTime.Now;
                Uow.CRM_Account.Update(CRM_Account);
                //if (CRM_Account.StageStatus > 20)
                //{
                //    //var modelData = Uow.CMR_StageStartEnd.GetAll().Where(x => x.CRM_AccountId == CRM_Account.CRM_AccountId && x.CRM_StageStatusId == CRM_Account.StageStatus).ToList();
                //    //if (modelData.Count == 0)
                //    Uow.CMR_StageStartEnd.Add(stageModel);
                //}
            }
            Uow.Commit();
            return CRM_Account;
        }

        public CRM_LeadGeneration SaveCRM_LeadGeneration(CRM_LeadGeneration CRM_LeadGeneration)
        {
            var ID = CRM_LeadGeneration.CRM_LeadGenerationId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_LeadGeneration.CreatedDate = DateTime.Now;
                Uow.CRM_LeadGeneration.Add(CRM_LeadGeneration);
            }
            else //Update existing entry
            {
                CRM_LeadGeneration.ModifiedDate = DateTime.Now;
                Uow.CRM_LeadGeneration.Update(CRM_LeadGeneration);
            }
            Uow.Commit();
            return CRM_LeadGeneration;
        }

        public CRM_FranchiseFollowUp SaveCRM_FranchiseFollowUp(CRM_FranchiseFollowUp CRM_FranchiseFollowUp)
        {
            var ID = CRM_FranchiseFollowUp.CRM_FranchiseFollowUpId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_FranchiseFollowUp.CreatedDate = DateTime.Now;
                Uow.CRM_FranchiseFollowUp.Add(CRM_FranchiseFollowUp);
            }
            else //Update existing entry
            {
                CRM_FranchiseFollowUp.ModifiedDate = DateTime.Now;
                Uow.CRM_FranchiseFollowUp.Update(CRM_FranchiseFollowUp);
            }
            Uow.Commit();
            return CRM_FranchiseFollowUp;
        }

        public CRM_SignAgreement SaveCRM_SignAgreement(CRM_SignAgreement CRM_SignAgreement)
        {
            var ID = CRM_SignAgreement.CRM_SignAgreementId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_SignAgreement.CreatedDate = DateTime.Now;
                Uow.CRM_SignAgreement.Add(CRM_SignAgreement);
            }
            else //Update existing entry
            {
                CRM_SignAgreement.ModifiedDate = DateTime.Now;
                Uow.CRM_SignAgreement.Update(CRM_SignAgreement);
            }
            Uow.Commit();
            return CRM_SignAgreement;
        }

        public CRM_FranchiseContract SaveCRM_FranchiseContract(CRM_FranchiseContract CRM_FranchiseContract)
        {
            var ID = CRM_FranchiseContract.CRM_FranchiseContractId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_FranchiseContract.CreatedDate = DateTime.Now;
                Uow.CRM_FranchiseContract.Add(CRM_FranchiseContract);
            }
            else //Update existing entry
            {
                CRM_FranchiseContract.ModifiedDate = DateTime.Now;
                Uow.CRM_FranchiseContract.Update(CRM_FranchiseContract);
            }
            Uow.Commit();
            return CRM_FranchiseContract;
        }

        public CRM_CloseTempDocument SaveCRM_CloseTempDocument(CRM_CloseTempDocument CRM_CloseTempDocument)
        {
            var ID = CRM_CloseTempDocument.CRM_CloseTempDocumentId;
            CRM_CloseTempDocument.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_CloseTempDocument.CreatedDate = DateTime.Now;
                Uow.CRM_CloseTempDocument.Add(CRM_CloseTempDocument);
            }
            else //Update existing entry
            {
                CRM_CloseTempDocument.ModifiedDate = DateTime.Now;
                Uow.CRM_CloseTempDocument.Update(CRM_CloseTempDocument);
            }
            Uow.Commit();
            return CRM_CloseTempDocument;
        }

        public CRM_Territory SaveCRM_Territory(CRM_Territory CRM_Territory)
        {
            var ID = CRM_Territory.CRM_TerritoryId;
            CRM_Territory.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_Territory.Add(CRM_Territory);
            }
            else //Update existing entry
            {
                Uow.CRM_Territory.Update(CRM_Territory);
            }
            Uow.Commit();
            return CRM_Territory;
        }

        public CRM_CallResult SaveCRM_CallResult(CRM_CallResult CRM_CallResult)
        {
            var ID = CRM_CallResult.CRM_CallResultId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_CallResult.Add(CRM_CallResult);
            }
            else //Update existing entry
            {
                Uow.CRM_CallResult.Update(CRM_CallResult);
            }
            Uow.Commit();
            return CRM_CallResult;
        }
        public CRM_NoteType SaveCRM_NoteType(CRM_NoteType CRM_NoteType)
        {
            var ID = CRM_NoteType.CRM_NoteTypeId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_NoteType.Add(CRM_NoteType);
            }
            else //Update existing entry
            {
                Uow.CRM_NoteType.Update(CRM_NoteType);
            }
            Uow.Commit();
            return CRM_NoteType;
        }
        public CRM_SalePossibilityType SaveCRM_SalePossibilityType(CRM_SalePossibilityType CRM_SalePossibilityType)
        {
            var ID = CRM_SalePossibilityType.CRM_SalePossibilityTypeId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_SalePossibilityType.Add(CRM_SalePossibilityType);
            }
            else //Update existing entry
            {
                Uow.CRM_SalePossibilityType.Update(CRM_SalePossibilityType);
            }
            Uow.Commit();
            return CRM_SalePossibilityType;
        }
        public CRM_ScheduleType SaveCRM_ScheduleType(CRM_ScheduleType CRM_ScheduleType)
        {
            var ID = CRM_ScheduleType.CRM_ScheduleTypeId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_ScheduleType.Add(CRM_ScheduleType);
            }
            else //Update existing entry
            {
                Uow.CRM_ScheduleType.Update(CRM_ScheduleType);
            }
            Uow.Commit();
            return CRM_ScheduleType;
        }
        public CRM_Territory_Assignment SaveCRM_TerriAssignment(CRM_Territory_Assignment CRM_TerriAssignment)
        {
            var ID = CRM_TerriAssignment.CRM_TerriAssignmentId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_Territory_Assignment.Add(CRM_TerriAssignment);
            }
            else //Update existing entry
            {
                Uow.CRM_Territory_Assignment.Update(CRM_TerriAssignment);
            }
            Uow.Commit();
            return CRM_TerriAssignment;
        }
        public CRM_SalesTerritory_Assignment SaveCRM_SalesTerriAssignment(CRM_SalesTerritory_Assignment CRM_SalesTerriAssignment)
        {
            var ID = CRM_SalesTerriAssignment.CRM_SalesTerriAssignmentId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                Uow.CRM_SalesTerritory_Assignment.Add(CRM_SalesTerriAssignment);
            }
            else //Update existing entry
            {
                Uow.CRM_SalesTerritory_Assignment.Update(CRM_SalesTerriAssignment);
            }
            Uow.Commit();
            return CRM_SalesTerriAssignment;
        }
        public CRM_Contact SaveCRM_Contact(CRM_Contact CRM_Contact)
        {
            var ID = CRM_Contact.CRM_ContactId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Contact.CreatedDate = DateTime.Now;
                Uow.CRM_Contact.Add(CRM_Contact);
            }
            else //Update existing entry
            {
                CRM_Contact.ModifiedDate = DateTime.Now;
                Uow.CRM_Contact.Update(CRM_Contact);
            }
            Uow.Commit();
            return CRM_Contact;
        }

        public CRM_ContactType SaveCRM_ContactType(CRM_ContactType CRM_ContactType)
        {
            var ID = CRM_ContactType.CRM_ContactTypeId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ContactType.CreatedDate = DateTime.Now;
                Uow.CRM_ContactType.Add(CRM_ContactType);
            }
            else //Update existing entry
            {
                CRM_ContactType.ModifiedDate = DateTime.Now;
                Uow.CRM_ContactType.Update(CRM_ContactType);
            }
            Uow.Commit();
            return CRM_ContactType;
        }

        public CRM_AccountCustomerDetail SaveCRM_AccountCustomerDetail(CRM_AccountCustomerDetail CRM_AccountCustomerDetail)
        {
            var ID = CRM_AccountCustomerDetail.CRM_AccountCustomerDetailId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_AccountCustomerDetail.CreatedDate = DateTime.Now;
                Uow.CRM_AccountCustomerDetail.Add(CRM_AccountCustomerDetail);
            }
            else //Update existing entry
            {
                CRM_AccountCustomerDetail.ModifiedDate = DateTime.Now;
                Uow.CRM_AccountCustomerDetail.Update(CRM_AccountCustomerDetail);
            }
            Uow.Commit();
            return CRM_AccountCustomerDetail;
        }

        public void UpdateCRM_AccountCustomerDetail_Coordinate(CRM_AccountCustomerDetail CRM_AccountCustomerDetail)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@CRM_AccountCustomerDetailId", CRM_AccountCustomerDetail.CRM_AccountCustomerDetailId),
                new SqlParameter("@Latitude", CRM_AccountCustomerDetail.CompanyLatitude),
                new SqlParameter("@Longitude", CRM_AccountCustomerDetail.CompanyLongitude)
            };

            SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                CommandType.StoredProcedure, DBConstants.sp_Update_CRM_AccountCustomerDetail_Coordinate, parameters);
        }

        public CRM_AccountFranchiseDetail SaveCRM_AccountFranchiseDetail(CRM_AccountFranchiseDetail CRM_AccountFranchiseDetail)
        {
            var ID = CRM_AccountFranchiseDetail.CRM_AccountFranchiseDetailId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_AccountFranchiseDetail.CreatedDate = DateTime.Now;
                Uow.CRM_AccountFranchiseDetail.Add(CRM_AccountFranchiseDetail);
            }
            else //Update existing entry
            {
                CRM_AccountFranchiseDetail.ModifiedDate = DateTime.Now;
                Uow.CRM_AccountFranchiseDetail.Update(CRM_AccountFranchiseDetail);
            }
            Uow.Commit();
            return CRM_AccountFranchiseDetail;
        }

        public CRM_Note SaveCRM_Note(CRM_Note CRM_Note)
        {
            var ID = CRM_Note.CRM_NoteId;
            CRM_Note.RegionId = CRM_Note.RegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Note.CreatedDate = DateTime.Now;
                Uow.CRM_Note.Add(CRM_Note);
            }
            else //Update existing entry
            {
                CRM_Note.ModifiedDate = DateTime.Now;

                Uow.CRM_Note.Update(CRM_Note);
            }
            Uow.Commit();
            return CRM_Note;
        }

        public CRM_Activity SaveCRM_Activity(CRM_Activity CRM_Activity)
        {
            var ID = CRM_Activity.CRM_ActivityId;
            CRM_Activity.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Activity.CreatedDate = DateTime.Now;
                Uow.CRM_Activity.Add(CRM_Activity);
            }
            else //Update existing entry
            {
                CRM_Activity.ModifiedDate = DateTime.Now;
                Uow.CRM_Activity.Update(CRM_Activity);
            }
            Uow.Commit();
            return CRM_Activity;
        }

        public CRM_Task SaveCRM_Task(CRM_Task CRM_Task)
        {
            var ID = CRM_Task.CRM_TaskId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Task.CreatedDate = DateTime.Now;
                Uow.CRM_Task.Add(CRM_Task);
            }
            else //Update existing entry
            {
                CRM_Task.ModifiedDate = DateTime.Now;
                Uow.CRM_Task.Update(CRM_Task);
            }
            Uow.Commit();
            return CRM_Task;
        }

        public CRM_Schedule SaveCRM_Schedule(CRM_Schedule CRM_Schedule)
        {
            var ID = CRM_Schedule.CRM_ScheduleId;
            //CRM_Schedule.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Schedule.CreatedDate = DateTime.Now;
                Uow.CRM_Schedule.Add(CRM_Schedule);
            }
            else //Update existing entry
            {
                CRM_Schedule.ModifiedDate = DateTime.Now;
                Uow.CRM_Schedule.Update(CRM_Schedule);
            }
            Uow.Commit();
            return CRM_Schedule;
        }
        public CRM_Document SaveCRM_Document(CRM_Document CRM_Document)
        {
            var ID = CRM_Document.CRM_DocumentId;
            //CRM_Document.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Document.CreatedBy = 0;
                CRM_Document.CreatedDate = DateTime.Now;

                Uow.CRM_Document.Add(CRM_Document);
            }
            else  //Update Existing Entry
            {
                CRM_Document.ModifiedBy = 0;
                CRM_Document.ModifiedDate = DateTime.Now;

                Uow.CRM_Document.Update(CRM_Document);
            }
            Uow.Commit();
            return CRM_Document;
        }

        public CRM_Document GetCRMDocumentWithAccountCustomer_FileType(int CRM_AccountCustomerDetailId, int FileTypeListId)
        {
            var modelData = Uow.CRM_Document.GetAll().Where(w => w.CRM_AccountCustomerDetailId == CRM_AccountCustomerDetailId && w.FileTypeListId == FileTypeListId);
            if (modelData != null)
            {
                return modelData.FirstOrDefault();
            }
            return new CRM_Document();

        }

        public CRM_InitialCommunication SaveCRM_InitialCommunication(CRM_InitialCommunication CRM_InitialCommunication)
        {
            var ID = CRM_InitialCommunication.CRM_InitialCommunicationId;
            //CRM_InitialCommunication.RegionId = CRM_InitialCommunication.RegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_InitialCommunication.CreatedDate = DateTime.Now;
                Uow.CRM_InitialCommunication.Add(CRM_InitialCommunication);
            }
            else //Update Existing Entry
            {
                CRM_InitialCommunication.ModifiedDate = DateTime.Now;
                Uow.CRM_InitialCommunication.Update(CRM_InitialCommunication);
            }
            Uow.Commit();
            return CRM_InitialCommunication;
        }
        public CRM_FvPresentation SaveCRM_FvPresentation(CRM_FvPresentation CRM_fvPresentation)
        {
            var ID = CRM_fvPresentation.CRM_FvPresentationId;
            //CRM_fvPresentation.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_fvPresentation.CreatedDate = DateTime.Now;
                Uow.CRM_FvPresentation.Add(CRM_fvPresentation);
            }
            else //Update Existing Entry
            {
                CRM_fvPresentation.ModifiedDate = DateTime.Now;
                Uow.CRM_FvPresentation.Update(CRM_fvPresentation);
            }
            Uow.Commit();
            return CRM_fvPresentation;
        }
        public CRM_Bidding SaveCRM_Bidding(CRM_Bidding CRM_Bidding)
        {
            var ID = CRM_Bidding.CRM_BiddingId;
            //CRM_Bidding.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Bidding.CreatedDate = DateTime.Now;
                Uow.CRM_Bidding.Add(CRM_Bidding);
            }
            else  //Update Existing Entry
            {
                CRM_Bidding.ModifiedDate = DateTime.Now;
                Uow.CRM_Bidding.Update(CRM_Bidding);
            }
            Uow.Commit();
            return CRM_Bidding;
        }
        public CRM_PdAppointment SaveCRM_PdAppointment(CRM_PdAppointment CRM_PdAppointment)
        {
            var ID = CRM_PdAppointment.CRM_PdAppointmentId;
            //CRM_PdAppointment.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_PdAppointment.CreatedDate = DateTime.Now;
                Uow.CRM_PdAppointment.Add(CRM_PdAppointment);
            }
            else //Update Existing Entry
            {
                CRM_PdAppointment.ModifiedDate = DateTime.Now;
                Uow.CRM_PdAppointment.Update(CRM_PdAppointment);
            }
            Uow.Commit();
            return CRM_PdAppointment;
        }
        public CRM_FollowUp SaveCRM_FollowUp(CRM_FollowUp CRM_FollowUp)
        {
            var ID = CRM_FollowUp.CRM_FollowUpId;
            //CRM_FollowUp.RegionId = SelectedRegionId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_FollowUp.CreatedDate = DateTime.Now;
                Uow.CRM_FollowUp.Add(CRM_FollowUp);
            }
            else // Update Existing Entry
            {
                CRM_FollowUp.ModifiedDate = DateTime.Now;
                Uow.CRM_FollowUp.Update(CRM_FollowUp);
            }
            Uow.Commit();
            return CRM_FollowUp;
        }
        public CRM_Close SaveCRM_Close(CRM_Close CRM_Close)
        {

            //get Old Close Records
            var temp_CRM_close = Uow.CRM_Close.GetAll().Where(x => x.CRM_AccountCustomerDetailId == CRM_Close.CRM_AccountCustomerDetailId & x.IsActive == true).ToList();

            foreach (var item in temp_CRM_close)
            {
                item.ModifiedDate = DateTime.Now;
                item.IsActive = false;
                Uow.CRM_Close.Update(item);
            }

            var ID = CRM_Close.CRM_CloseId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Close.CreatedDate = DateTime.Now;
                Uow.CRM_Close.Add(CRM_Close);
            }
            else // Update Existing Entry
            {
                CRM_Close.ModifiedDate = DateTime.Now;
                Uow.CRM_Close.Update(CRM_Close);
            }
            Uow.Commit();
            return CRM_Close;
        }
        public CRM_StageStatusSchedule SaveCRM_StageStatusSchedule(CRM_StageStatusSchedule CRM_StageStatusSchedule)
        {
            var ID = CRM_StageStatusSchedule.CRM_StageStatusScheduleId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_StageStatusSchedule.CreatedDate = DateTime.Now;
                Uow.CRM_StageStatusSchedule.Add(CRM_StageStatusSchedule);
            }
            else //Update Existing Entry
            {
                CRM_StageStatusSchedule.ModifiedDate = DateTime.Now;
                Uow.CRM_StageStatusSchedule.Update(CRM_StageStatusSchedule);
            }
            Uow.Commit();
            return CRM_StageStatusSchedule;
        }

        public CRM_Quotation SaveCRM_Quotation(CRM_Quotation CRM_Quotation)
        {
            var ID = CRM_Quotation.CRM_QuotationId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Quotation.CreatedDate = DateTime.Now;
                Uow.CRM_Quotation.Add(CRM_Quotation);
            }
            else //Update existing entry
            {
                CRM_Quotation.ModifiedDate = DateTime.Now;
                Uow.CRM_Quotation.Update(CRM_Quotation);
            }
            Uow.Commit();
            return CRM_Quotation;
        }

        public CRM_TaskType SaveCRM_TaskType(CRM_TaskType CRM_TaskType)
        {
            var ID = CRM_TaskType.CRM_TaskTypeId;



            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_TaskType.CreatedDate = DateTime.Now;
                Uow.CRM_TaskType.Add(CRM_TaskType);
            }
            else //Update existing entry
            {
                CRM_TaskType.ModifiedDate = DateTime.Now;
                Uow.CRM_TaskType.Update(CRM_TaskType);
            }
            Uow.Commit();
            return CRM_TaskType;
        }

        public CRM_Stage SaveCRM_Stage(CRM_Stage CRM_Stage)
        {
            var ID = CRM_Stage.CRM_StageId;

            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_Stage.CreatedDate = DateTime.Now;
                Uow.CRM_Stage.Add(CRM_Stage);
            }
            else //Update existing entry
            {
                CRM_Stage.ModifiedDate = DateTime.Now;
                Uow.CRM_Stage.Update(CRM_Stage);
            }
            Uow.Commit();
            return CRM_Stage;
        }

        public CRM_StageStatus SaveCRM_StageStatus(CRM_StageStatus CRM_StageStatus)
        {
            var ID = CRM_StageStatus.CRM_StageStatusId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_StageStatus.CreatedDate = DateTime.Now;
                Uow.CRM_StageStatus.Add(CRM_StageStatus);
            }
            else //Update existing entry
            {
                CRM_StageStatus.ModifiedDate = DateTime.Now;
                Uow.CRM_StageStatus.Update(CRM_StageStatus);
            }
            Uow.Commit();
            return CRM_StageStatus;
        }

        public CRM_IndustryType SaveCRM_IndustryType(CRM_IndustryType CRM_IndustryType)
        {
            var ID = CRM_IndustryType.CRM_IndustryTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_IndustryType.CreatedDate = DateTime.Now;
                Uow.CRM_IndustryType.Add(CRM_IndustryType);
            }
            else //Update existing entry
            {
                CRM_IndustryType.ModifiedDate = DateTime.Now;
                Uow.CRM_IndustryType.Update(CRM_IndustryType);
            }
            Uow.Commit();
            return CRM_IndustryType;
        }

        public CRM_ProviderSource SaveCRM_ProviderSource(CRM_ProviderSource CRM_ProviderSource)
        {
            var ID = CRM_ProviderSource.CRM_ProviderSourceId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ProviderSource.CreatedDate = DateTime.Now;
                Uow.CRM_ProviderSource.Add(CRM_ProviderSource);
            }
            else //Update existing entry
            {
                CRM_ProviderSource.ModifiedDate = DateTime.Now;
                Uow.CRM_ProviderSource.Update(CRM_ProviderSource);
            }
            Uow.Commit();
            return CRM_ProviderSource;
        }

        public CRM_ProviderType SaveCRM_ProviderType(CRM_ProviderType CRM_ProviderType)
        {
            var ID = CRM_ProviderType.CRM_ProviderTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ProviderType.CreatedDate = DateTime.Now;
                Uow.CRM_ProviderType.Add(CRM_ProviderType);
            }
            else //Update existing entry
            {
                CRM_ProviderType.ModifiedDate = DateTime.Now;
                Uow.CRM_ProviderType.Update(CRM_ProviderType);
            }
            Uow.Commit();
            return CRM_ProviderType;
        }

        public CRM_AccountType SaveCRM_AccountType(CRM_AccountType CRM_AccountType)
        {
            var ID = CRM_AccountType.CRM_AccountTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_AccountType.CreatedDate = DateTime.Now;
                Uow.CRM_AccountType.Add(CRM_AccountType);
            }
            else //Update existing entry
            {
                CRM_AccountType.ModifiedDate = DateTime.Now;
                Uow.CRM_AccountType.Update(CRM_AccountType);
            }
            Uow.Commit();
            return CRM_AccountType;
        }

        public CRM_ActivityOutcomeType SaveCRM_ActivityOutcomeType(CRM_ActivityOutcomeType CRM_ActivityOutcomeType)
        {
            var ID = CRM_ActivityOutcomeType.CRM_ActivityOutcomeTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ActivityOutcomeType.CreatedDate = DateTime.Now;
                Uow.CRM_ActivityOutcomeType.Add(CRM_ActivityOutcomeType);
            }
            else //Update existing entry
            {
                CRM_ActivityOutcomeType.ModifiedDate = DateTime.Now;
                Uow.CRM_ActivityOutcomeType.Update(CRM_ActivityOutcomeType);
            }
            Uow.Commit();
            return CRM_ActivityOutcomeType;
        }

        public CRM_ActivityType SaveCRM_ActivityType(CRM_ActivityType CRM_ActivityType)
        {
            var ID = CRM_ActivityType.CRM_ActivityTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_ActivityType.CreatedDate = DateTime.Now;
                Uow.CRM_ActivityType.Add(CRM_ActivityType);
            }
            else //Update existing entry
            {
                CRM_ActivityType.ModifiedDate = DateTime.Now;
                Uow.CRM_ActivityType.Update(CRM_ActivityType);
            }
            Uow.Commit();
            return CRM_ActivityType;
        }

        public CRM_TimeLineType SaveCRM_TimeLineType(CRM_TimeLineType CRM_TimeLineType)
        {
            var ID = CRM_TimeLineType.CRM_TimeLineTypeId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_TimeLineType.CreatedDate = DateTime.Now;
                Uow.CRM_TimeLineType.Add(CRM_TimeLineType);
            }
            else //Update existing entry
            {
                CRM_TimeLineType.ModifiedDate = DateTime.Now;
                Uow.CRM_TimeLineType.Update(CRM_TimeLineType);
            }
            Uow.Commit();
            return CRM_TimeLineType;
        }

        public CRM_TimeLine SaveCRM_TimeLine(CRM_TimeLine CRM_TimeLine)
        {
            var ID = CRM_TimeLine.CRM_TimeLineId;
            var isNew = ID == 0;
            //Add New Entry
            if (isNew)
            {
                CRM_TimeLine.CreatedDate = DateTime.Now;
                Uow.CRM_TimeLine.Add(CRM_TimeLine);
            }
            else //Update existing entry
            {
                CRM_TimeLine.ModifiedDate = DateTime.Now;
                Uow.CRM_TimeLine.Update(CRM_TimeLine);
            }
            Uow.Commit();
            return CRM_TimeLine;
        }

        #endregion

        #region CRM_AccountService > Delete

        public void DeleteCRM_CallLog(int id)
        {
            var entity = Uow.CRM_CallLog.GetById(id);
            Uow.CRM_CallLog.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_CloseType(int id)
        {
            var entity = Uow.CRM_CloseType.GetById(id);
            Uow.CRM_CloseType.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_ReasonType(int id)
        {
            var entity = Uow.CRM_ReasonType.GetById(id);
            Uow.CRM_ReasonType.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_PurposeType(int id)
        {
            var entity = Uow.CRM_PurposeType.GetById(id);
            Uow.CRM_PurposeType.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_Account(int id)
        {
            var entity = Uow.CRM_Account.GetById(id);
            Uow.CRM_Account.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_LeadGeneration(int id)
        {
            var entity = Uow.CRM_LeadGeneration.GetById(id);
            Uow.CRM_LeadGeneration.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_FranchiseFollowUp(int id)
        {
            var entity = Uow.CRM_FranchiseFollowUp.GetById(id);
            Uow.CRM_FranchiseFollowUp.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_SignAgreement(int id)
        {
            var entity = Uow.CRM_SignAgreement.GetById(id);
            Uow.CRM_SignAgreement.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_FranchiseContract(int id)
        {
            var entity = Uow.CRM_FranchiseContract.GetById(id);
            Uow.CRM_FranchiseContract.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_Territory(int id)
        {
            var entity = Uow.CRM_Territory.GetById(id);
            Uow.CRM_Territory.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_CallResult(int id)
        {
            var entity = Uow.CRM_CallResult.GetById(id);
            Uow.CRM_CallResult.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_ScheduleType(int id)
        {
            var entity = Uow.CRM_ScheduleType.GetById(id);
            Uow.CRM_ScheduleType.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_TerriAssignment(int id)
        {
            var entity = Uow.CRM_Territory_Assignment.GetById(id);
            Uow.CRM_Territory_Assignment.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_SalesTerriAssignment(int id)
        {
            var entity = Uow.CRM_SalesTerritory_Assignment.GetById(id);
            Uow.CRM_SalesTerritory_Assignment.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_Contact(int id)
        {
            var entity = Uow.CRM_Contact.GetById(id);
            Uow.CRM_Contact.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_ContactType(int id)
        {
            var entity = Uow.CRM_ContactType.GetById(id);
            Uow.CRM_ContactType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Activity(int id)
        {
            var entity = Uow.CRM_Activity.GetById(id);
            Uow.CRM_Activity.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_AccountCustomerDetail(int id)
        {
            var entity = Uow.CRM_AccountCustomerDetail.GetById(id);
            Uow.CRM_AccountCustomerDetail.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_AccountFranchiseDetail(int id)
        {
            var entity = Uow.CRM_AccountFranchiseDetail.GetById(id);
            Uow.CRM_AccountFranchiseDetail.Delete(entity);
        }

        public void DeleteCRM_TimeLine(int id)
        {
            var entity = Uow.CRM_TimeLine.GetById(id);
            Uow.CRM_TimeLine.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Note(int id)
        {
            var entity = Uow.CRM_Note.GetById(id);
            Uow.CRM_Note.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Schedule(int id)
        {
            var entity = Uow.CRM_Schedule.GetById(id);
            Uow.CRM_Schedule.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Document(int id)
        {
            var entity = Uow.CRM_Document.GetById(id);
            Uow.CRM_Document.Delete(entity);
            Uow.Commit();
        }

        public CRM_Document GetUploadDocumentById(int id)
        {
            return Uow.CRM_Document.GetById(id);

        }
        public void DeleteCRM_InitialCommunication(int id)
        {
            var entity = Uow.CRM_InitialCommunication.GetById(id);
            Uow.CRM_InitialCommunication.Delete(entity);
            Uow.Commit();
        }
        public void DeleteCRM_FvPresentation(int id)
        {
            var entity = Uow.CRM_FvPresentation.GetById(id);
            Uow.CRM_FvPresentation.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Bidding(int id)
        {
            var entity = Uow.CRM_Bidding.GetById(id);
            Uow.CRM_Bidding.Update(entity);
            Uow.Commit();
        }

        public void DeleteCRM_PdAppointment(int id)
        {
            var entity = Uow.CRM_PdAppointment.GetById(id);
            Uow.CRM_PdAppointment.Update(entity);
            Uow.Commit();
        }
        public void DeleteCRM_FollowUp(int id)
        {
            var entity = Uow.CRM_FollowUp.GetById(id);
            Uow.CRM_FollowUp.Update(entity);
            Uow.Commit();
        }
        public void DeleteCRM_Close(int id)
        {
            var entity = Uow.CRM_Close.GetById(id);
            Uow.CRM_Close.Update(entity);
            Uow.Commit();
        }
        public void DeleteCRM_StageStatusSchedule(int id)
        {
            var entity = Uow.CRM_StageStatusSchedule.GetById(id);
            Uow.CRM_StageStatusSchedule.Update(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Quotation(int id)
        {
            var entity = Uow.CRM_Quotation.GetById(id);
            Uow.CRM_Quotation.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Task(int id)
        {
            var entity = Uow.CRM_Task.GetById(id);
            Uow.CRM_Task.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_TaskType(int id)
        {
            var entity = Uow.CRM_TaskType.GetById(id);
            Uow.CRM_TaskType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_Stage(int id)
        {
            var entity = Uow.CRM_Stage.GetById(id);
            Uow.CRM_Stage.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_StageStatus(int id)
        {
            var entity = Uow.CRM_StageStatus.GetById(id);
            Uow.CRM_StageStatus.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_IndustryType(int id)
        {
            var entity = Uow.CRM_IndustryType.GetById(id);
            Uow.CRM_IndustryType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_ProviderSource(int id)
        {
            var entity = Uow.CRM_ProviderSource.GetById(id);
            Uow.CRM_ProviderSource.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_ProviderType(int id)
        {
            var entity = Uow.CRM_ProviderType.GetById(id);
            Uow.CRM_ProviderType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_AccountType(int id)
        {
            var entity = Uow.CRM_AccountType.GetById(id);
            Uow.CRM_AccountType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_ActivityOutcomeType(int id)
        {
            var entity = Uow.CRM_ActivityOutcomeType.GetById(id);
            Uow.CRM_ActivityOutcomeType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_ActivityType(int id)
        {
            var entity = Uow.CRM_ActivityType.GetById(id);
            Uow.CRM_ActivityType.Delete(entity);
            Uow.Commit();
        }

        public void DeleteCRM_TimeLineType(int id)
        {
            var entity = Uow.CRM_TimeLineType.GetById(id);
            Uow.CRM_TimeLineType.Delete(entity);
            Uow.Commit();
        }

        #endregion

        #region AccountService > Custom Calls

        public List<CRM_spGet_PotentialList_Result> CRM_LeadCustomerSearch(CRMSearchModel crmSearchModel)
        {
            string filter = string.Empty;
            using (var context = new jkDatabaseEntities())
            {
                if (crmSearchModel.CrmLeadSearch.AccountType > 0)
                {
                    filter += " and ACTCUSDTL.AccountTypeListId=" + crmSearchModel.CrmLeadSearch.AccountType;

                }
                /*3 TerritoryId Exception is 3*/
                if (crmSearchModel.CrmLeadSearch.Exception)
                {
                    filter += " and ACTCUSDTL.TerritoryId=" + 3;
                }
                /*Lead StageStatus for CallBack = 26*/
                if (crmSearchModel.CrmLeadSearch.IncludeCallBackPending)
                {
                    filter += " and ACT.StageStatus=" + 26;
                }

                /* filter += " and LeadOnly=" + (crmSearchModel.CrmCustomerLeadSearch.LeadOnly ? 1 : 0);  */

                if (crmSearchModel.CrmLeadSearch.Source > 0)
                {
                    filter += " and ACT.ProviderSource=" + crmSearchModel.CrmLeadSearch.Source;
                }
                if (crmSearchModel.CrmLeadSearch.Status > 0)
                {
                    filter += " and ACT.StageStatus=" + crmSearchModel.CrmLeadSearch.Status;
                }
                if (crmSearchModel.CrmLeadSearch.Territory > 0)
                {
                    filter += " and ACTCUSDTL.TerritoryId=" + crmSearchModel.CrmLeadSearch.Territory;
                }
                if (crmSearchModel.CrmLeadSearch.SortBy != null)
                {
                    switch (crmSearchModel.CrmLeadSearch.SortBy)
                    {
                        case "Budget Amount":
                            filter += " and order By ACTCUSDTL.BudgetAmount";
                            break;
                        case "City":
                            filter += " and order By ACTCUSDTL.CompanyCity";
                            break;
                        case "Contact Person":
                            filter += " and order By=" + "'" + crmSearchModel.CrmLeadSearch.SortBy + "'";
                            break;
                        case "Contract Amount":
                            filter += " and order By CC.ContractAmount";
                            break;
                        case "Create Date":
                            filter += " and order By ACTCUSDTL.CreatedDate";
                            break;
                        case "E-Mail":
                            filter += " and order By ACT.EmailAddress";
                            break;
                        case "Lead ID":
                            filter += " and order By ACT.CRM_AccountId";
                            break;
                        case "Proposal Amount":
                            filter += " and order By ACTCUSDTL.BudgetAmount";
                            break;
                        case "Square Footage":
                            filter += " and order By ACTCUSDTL.SqFt";
                            break;
                        case "Street":
                            filter += " and order By ACTCUSDTL.CompanyAddressLine1";
                            break;
                        case "Zip Code":
                            filter += " and order By ACTCUSDTL.CompanyZipCode";
                            break;

                    }
                }

                foreach (var item in crmSearchModel.SearchField)
                {
                    if (item.Text != null)
                    {
                        if (item.SearchField == "ACTCUSDTL.BudgetAmount" || item.SearchField == "CC.ContractAmount" || item.SearchField == "ACTCUSDTL.BudgetAmount" || item.SearchField == "ACT.CRM_AccountId")
                        {
                            filter += " " + (item.Condition ? "AND" : "OR") + " " + item.SearchField + (item.Operation == 1 ? "=" : (item.Operation == 2 ? ">" : "<")) + "" + item.Text + "";
                        }
                        else
                        {
                            filter += " " + (item.Condition ? "AND" : "OR") + " " + item.SearchField + (item.Operation == 1 ? "=" : (item.Operation == 2 ? ">" : "<")) + "'" + item.Text + "'";
                        }
                    }
                }
                var spParam = new SqlParameter
                {
                    ParameterName = "@filter",
                    Value = filter
                };

                return context.Database.SqlQuery<CRM_spGet_PotentialList_Result>("exec CRM_spGet_CustomerLeadSearch @filter", spParam).ToList<CRM_spGet_PotentialList_Result>();
            }

        }
        public List<CRMPotentialFranchiseeViewModel> CRM_LeadFranchiseSearch(CRMSearchModel crmSearchModel)
        {
            string filter = string.Empty;
            using (var context = new jkDatabaseEntities())
            {


                /*Lead StageStatus for CallBack = 26*/
                if (crmSearchModel.CrmLeadSearch.IncludeCallBackPending)
                {
                    filter += " and ACT.StageStatus=" + 26;
                }

                /* filter += " and LeadOnly=" + (crmSearchModel.CrmCustomerLeadSearch.LeadOnly ? 1 : 0);  */

                if (crmSearchModel.CrmLeadSearch.Source > 0)
                {
                    filter += " and ACT.ProviderSource=" + crmSearchModel.CrmLeadSearch.Source;
                }
                if (crmSearchModel.CrmLeadSearch.Status > 0)
                {
                    filter += " and ACT.StageStatus=" + crmSearchModel.CrmLeadSearch.Status;
                }

                if (crmSearchModel.CrmLeadSearch.SortBy != null)
                {
                    switch (crmSearchModel.CrmLeadSearch.SortBy)
                    {
                        case "Budget Amount":
                            filter += " and order By ACTCUSDTL.AmtToInvest";
                            break;
                        case "City":
                            filter += " and order By ACTCUSDTL.City";
                            break;
                        case "Contact Person":
                            filter += " and order By=" + "'" + crmSearchModel.CrmLeadSearch.SortBy + "'";
                            break;
                        case "Contract Amount":
                            filter += " and order By CC.ContractAmount";
                            break;
                        case "Create Date":
                            filter += " and order By ACTCUSDTL.CreatedDate";
                            break;
                        case "E-Mail":
                            filter += " and order By ACTCUSDTL.EmailAddress";
                            break;
                        case "Lead ID":
                            filter += " and order By ACT.CRM_AccountId";
                            break;
                        case "Street":
                            filter += " and order By ACTCUSDTL.Address1";
                            break;
                        case "Zip Code":
                            filter += " and order By ACTCUSDTL.ZipCode";
                            break;
                    }
                }

                foreach (var item in crmSearchModel.SearchField)
                {
                    if (item.Text != null)
                    {
                        if (item.SearchField == "ACTCUSDTL.AmtToInvest" || item.SearchField == "CC.ContractAmount" || item.SearchField == "ACT.CRM_AccountId")
                        {
                            filter += " " + (item.Condition ? "AND" : "OR") + " " + item.SearchField + (item.Operation == 1 ? "=" : (item.Operation == 2 ? ">" : "<")) + "" + item.Text + "";
                        }
                        else
                        {
                            filter += " " + (item.Condition ? "AND" : "OR") + " " + item.SearchField + (item.Operation == 1 ? "=" : (item.Operation == 2 ? ">" : "<")) + "'" + item.Text + "'";
                        }
                    }
                }
                var spParam = new SqlParameter
                {
                    ParameterName = "@filter",
                    Value = filter
                };

                return context.Database.SqlQuery<CRMPotentialFranchiseeViewModel>("exec CRM_spGet_FranchiseLeadSearch @filter", spParam).ToList<CRMPotentialFranchiseeViewModel>();
            }
        }

        public CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetail_ByAccountId(int id)
        {
            return Uow.CRM_AccountFranchiseDetail.GetAll().FirstOrDefault(x => x.CRM_AccountId == id);
        }
        public CRM_CloseTempDocument GetCRM_CRM_CloseTempDocument_ByAccountCustomerDetailId(int id)
        {
            return Uow.CRM_CloseTempDocument.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_FranchiseFollowUp GetCRM_FranchiseFollowUp_ByAccountFranchiseDetailId(int id)
        {
            return Uow.CRM_FranchiseFollowUp.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }
        public CRM_AccountCustomerDetail GetCRM_AccountCustomerDetail_ByAccountId(int id)
        {
            //return Uow.CRM_AccountCustomerDetail.GetAll().FirstOrDefault(x => x.CRM_AccountId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_AccountCustomerDetail.FirstOrDefault(x => x.CRM_AccountId == id);
        }

        public CRM_AccountCustomerDetail GetCRM_AccountCustomerDetail_ByEmail(string email)
        {
            return Uow.CRM_AccountCustomerDetail.GetAll().FirstOrDefault(x => x.CompanyEmailAddress == email);
        }

        public CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetail_ByEmail(string email)
        {
            return Uow.CRM_AccountFranchiseDetail.GetAll().FirstOrDefault(x => x.EmailAddress == email);
        }

        public CRM_Schedule GetCRM_Schedule_ByOutlookAppointmentGuid(Guid guid)
        {
            return Uow.CRM_Schedule.GetAll().FirstOrDefault(x => x.OutlookAppointmentGuid == guid);
        }

        public CRM_InitialCommunication Get_InitialCommunication_ByAccountCustomerDetailById(int id)
        {
            //return Uow.CRM_InitialCommunication.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_InitialCommunication.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }
        public CRM_InitialCommunication GetCRM_InitialCommunication_ByAccountFranchiseDetailById(int id)
        {
            return Uow.CRM_InitialCommunication.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }
        public CRM_Contact Get_Contact_ByAccountCustomerDetailById(int id)
        {
            return Uow.CRM_Contact.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }
        public List<AuthRoleUserViewModel> Get_AuthUserLogin(int roleid, int? territoryId = null)
        {
            using (var context = new jkDatabaseEntities())
            {
                var salesperson = context.CRM_spGet_SalesPerson(roleid, territoryId).GroupBy(x => x.UserId)
                                 .Select(grp => grp.First()).MapEnumerable<AuthRoleUserViewModel, CRM_spGet_SalesPerson_Result>().ToList();
                return salesperson;
            }
        }
        public List<CRMScheduleUserHierarchy> Get_AuthUserLogin_Potential(int roleid, int loginUserId = 0, int regionId = 0)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@roleid", roleid);
                parmas.Add("@loginUserId", loginUserId);
                parmas.Add("@regionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.spGetCRMUserByParentUserId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CRMScheduleUserHierarchy>().ToList();
                    }
                }
                return new List<CRMScheduleUserHierarchy>();
            }

        }

        public void UpdateInActiveCRMLeadStageData(int CRM_AccountCustomerDetailId)
        {

            using (var context = new jkDatabaseEntities())
            {
                var lstCRM_Bidding = context.CRM_Bidding.Where(o => o.CRM_AccountCustomerDetailId == CRM_AccountCustomerDetailId).ToList();
                foreach (var item in lstCRM_Bidding)
                {
                    item.IsActive = false;
                }

                var lstCRM_PdAppointment = context.CRM_PdAppointment.Where(o => o.CRM_AccountCustomerDetailId == CRM_AccountCustomerDetailId).ToList();
                foreach (var item in lstCRM_PdAppointment)
                {
                    item.IsActive = false;
                }

                var lstCRM_Close = context.CRM_Close.Where(o => o.CRM_AccountCustomerDetailId == CRM_AccountCustomerDetailId).ToList();
                foreach (var item in lstCRM_Close)
                {
                    item.IsActive = false;
                }

                var lstCRM_FollowUp = context.CRM_FollowUp.Where(o => o.CRM_AccountCustomerDetailId == CRM_AccountCustomerDetailId).ToList();
                foreach (var item in lstCRM_FollowUp)
                {
                    item.IsActive = false;
                }
                context.SaveChanges();
            }
        }

        public CRM_LeadGeneration GetCRM_LeadGeneration_ByAccountFranchiseId(int id)
        {
            return Uow.CRM_LeadGeneration.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }

        public CRM_FranchiseContract GetCRM_FranchiseContract_ByAccountFranchiseId(int id)
        {
            return Uow.CRM_FranchiseContract.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }

        public CRM_SignAgreement GetCRM_SignAgreement_ByAccountFranchiseId(int id)
        {
            return Uow.CRM_SignAgreement.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }

        public CRM_InitialCommunication GetCRM_InitialCommunication_ByAccountFranchiseId(int id)
        {
            return Uow.CRM_InitialCommunication.GetAll().FirstOrDefault(x => x.CRM_AccountFranchiseDetailId == id);
        }
        public CRM_FvPresentation Get_fvPresentation_ByAccountCustomerDetailById(int id)
        {
            //return Uow.CRM_FvPresentation.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_FvPresentation.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_Bidding Get_Bidding_ByAccountCustomerDetailById(int id)
        {
            //return Uow.CRM_Bidding.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Bidding.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_PdAppointment Get_PdAppointment_ByAccountCustomerDetailById(int id)
        {
            // return Uow.CRM_PdAppointment.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_PdAppointment.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_FollowUp Get_FollowUp_ByAccountCustomerDetailById(int id)
        {
            //return Uow.CRM_FollowUp.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_FollowUp.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_Close Get_Close_ByAccountCustomerDetailById(int id)
        {
            // return Uow.CRM_Close.GetAll().FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Close.FirstOrDefault(x => x.CRM_AccountCustomerDetailId == id);
        }

        public CRM_StageStatusSchedule Get_StageStatusSchedule_PdAppoint(int id)
        {
            return Uow.CRM_StageStatusSchedule.GetAll().FirstOrDefault(x => x.CRM_PdAppointmentId == id);
        }

        public List<CRM_Activity> GetCRM_Activity_ByAccountCustomerDetailId(int id)
        {
            //return Uow.CRM_Activity.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Activity.Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_Activity> GetCRM_Activity_ByAccountFranchiseDetailId(int id)
        {
            return Uow.CRM_Activity.GetAll().Where(x => x.CRM_AccountFranchiseDetailId == id).ToList();
        }

        public List<CRM_Document> GetCRM_Document_ByAccountCustomerDetailId(int id)
        {
            //return Uow.CRM_Document.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Document.Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_Document> GetCRM_Document_ByAccountFranchiseDetailId(int id)
        {
            return Uow.CRM_Document.GetAll().Where(x => x.CRM_AccountFranchiseDetailId == id).ToList();
        }

        public List<CRM_StageStatusSchedule> GetCRM_StageStatusSchedules_ByAccountCustomerDetailById(int? id)
        {
            return Uow.CRM_StageStatusSchedule.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_StageStatusSchedule> GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById(int? id)
        {
            return Uow.CRM_StageStatusSchedule.GetAll().Where(x => x.CRM_AccountFranchiseDetailId == id).ToList();
        }

        public List<CRM_InitialCommunication> GetCRM_InitialCommunication_ByAccountCustomerDetailId(int id)
        {
            return Uow.CRM_InitialCommunication.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_FvPresentation> GetCRM_FvPresentation_ByAccountCustomerDetailId(int id)
        {
            return Uow.CRM_FvPresentation.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_Bidding> GetCRM_Bidding_ByAccountCustomerDetailId(int id)
        {
            return Uow.CRM_Bidding.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_Note> GetCRM_Note_ByAccountCustomerDetailId(int id)
        {
            //return Uow.CRM_Note.GetAll().Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
            jkDatabaseEntities context = new jkDatabaseEntities();
            return context.CRM_Note.Where(x => x.CRM_AccountCustomerDetailId == id).ToList();
        }

        public List<CRM_Note> GetCRM_Note_ByAccountFranchiseDetailId(int id)
        {
            return Uow.CRM_Note.GetAll().Where(x => x.CRM_AccountFranchiseDetailId == id).ToList();
        }
        public List<AuthDepartment> GetDepartment()
        {
            return Uow.AuthDepartment.GetAll().ToList();
        }
        public List<CRM_Schedule> GetCRM_Schedule_ByAccountCustomerDetailId(int? id)
        {
            //return Uow.CRM_Schedule.GetAll().Where(x => x.ClassId == id).ToList();
            jkDatabaseEntities context = new jkDatabaseEntities();
            //var listo = context.CRM_Schedule.Where(x => x.ClassId == id).ToList();
            //return listo;
            return context.CRM_Schedule.Where(x => x.ClassId == id).ToList();
        }

        public List<CRM_Schedule> GetCRM_Schedule_ByAccountFranchiseDetailId(int id)
        {
            return Uow.CRM_Schedule.GetAll().Where(x => x.CRM_AccountFranchiseDetailId == id).ToList();
        }

        public List<CRM_Account> GetAll_CRM_Account_ByFranchiseDetail(int CRM_Stage)
        {
            using (var context = new jkDatabaseEntities())
            {
                var CRM_Account = context.CRM_Account.Where(x => x.Stage == CRM_Stage && x.AccountType == (int)AccountType.Franchise && x.RegionId == SelectedRegionId).ToList();
                return CRM_Account;
            }
            //return Uow.CRM_Account..Where(x => x.Stage == CRM_Stage).ToList();
        }

        public List<CRM_Account> GetAll_CRM_Account_ByStage(int CRM_Stage)
        {
            return Uow.CRM_Account.GetAll().Where(x => x.Stage == CRM_Stage).ToList();
        }

        public List<CRM_Account> GetAll_CRM_Account_ByStageStatus(int CRM_StageStatus)
        {
            return Uow.CRM_Account.GetAll().Where(x => x.StageStatus == CRM_StageStatus).ToList();
        }

        public CRM_InitialCommunication AddNewInitalData(int accountid, int AccountcustomerDetailId, string contactPerson, int interestedInProposal, DateTime availableToMeet, int purpose, string note, int userId, int RegionId)
        {
            //Save data into CRM_InitialCommunication
            var crmInital = new CRM_InitialCommunication();

            crmInital.CRM_AccountCustomerDetailId = AccountcustomerDetailId;
            crmInital.ContactPerson = contactPerson;
            crmInital.InterestedInPerposal = Convert.ToBoolean(interestedInProposal);
            crmInital.StartDate = availableToMeet;
            crmInital.PURPOSE = purpose;
            crmInital.Note = note;
            crmInital.CreatedDate = DateTime.Now;
            crmInital.ModifiedDate = DateTime.Now;
            crmInital.RegionId = RegionId;

            SaveCRM_InitialCommunication(crmInital);

            //update AccountCustomer
            var accountcustomer = GetCRM_AccountbyId(accountid);
            if (accountcustomer != null)
            {
                accountcustomer.StageStatus = (int)StageStatusType.FvPresentation;
                SaveCRM_Account(accountcustomer);
            }
            return crmInital;
        }

        public CRM_Account AddNewCustomerAccount(string companyname, string contactname, string phoneNumber, string emailAddress, int? providerType, int? providerSource, string accountType, int userId, int regionId, string Note = "")
        {
            // Store data in CRM_Account
            var acc = new CRM_Account();
            if (accountType.Equals(CRM_ServiceConstants.Key_Customer))
            {

                acc.AccountType = GetAll_CRM_AccountType().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_Customer)?.Type;
                acc.ContactName = contactname;
                //acc.LastName = lastName;
                //acc.EmailAddress = emailAddress;
                acc.PhoneNumber = phoneNumber;
                acc.ProviderType = providerType;
                acc.ProviderSource = providerSource;
                acc.EmailAddress = emailAddress;
                acc.RegionId = regionId;
                acc.Stage = GetAll_CRM_Stage().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_Lead)?.Type;
                acc.StageStatus = GetAll_CRM_StageStatus().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_NewLead)?.Type;
                acc.CreatedBy = userId;
                acc.CreatedDate = DateTime.Now;
                acc.ModifiedBy = userId;
                acc.ModifiedDate = DateTime.Now;

            }
            if (accountType.Equals(CRM_ServiceConstants.Key_Franchise))
            {
                acc.AccountType = GetAll_CRM_AccountType().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_Franchise)?.Type;
                acc.ContactName = contactname;
                //acc.LastName = lastName;
                //acc.EmailAddress = emailAddress;
                acc.PhoneNumber = phoneNumber;
                acc.ProviderType = providerType;
                acc.ProviderSource = providerSource;
                acc.RegionId = regionId;
                acc.Stage = GetAll_CRM_Stage().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_Lead)?.Type;
                acc.StageStatus = GetAll_CRM_StageStatus().FirstOrDefault(x => x.Name == CRM_ServiceConstants.Key_NewLead)?.Type;
                acc.CreatedBy = userId;
                acc.CreatedDate = DateTime.Now;
                acc.ModifiedBy = userId;
                acc.ModifiedDate = DateTime.Now;
            }
            SaveCRM_Account(acc);

            if (accountType.Equals(CRM_ServiceConstants.Key_Customer))
            {
                // Store data In Customer detail
                var accCustomer = new CRM_AccountCustomerDetail
                {
                    CRM_AccountId = acc.CRM_AccountId,
                    RegionId = regionId,
                    CompanyName = companyname,
                    CompanyEmailAddress = emailAddress,
                    CompanyPhoneNumber = phoneNumber,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now
                };
                SaveCRM_AccountCustomerDetail(accCustomer);

                //Save Note Data
                if (Note != null && Note != "")
                {
                    CRM_Note CRM_Noteobj = new CRM_Note();
                    CRM_Noteobj.CRM_AccountCustomerDetailId = accCustomer.CRM_AccountCustomerDetailId;
                    CRM_Noteobj.Description = Note;
                    CRM_Noteobj.CreatedDate = DateTime.Now;
                    CRM_Noteobj.CreatedBy = userId;
                    CRM_Noteobj.RegionId = regionId;
                    SaveCRM_Note(CRM_Noteobj);
                }
            }
            if (accountType.Equals(CRM_ServiceConstants.Key_Franchise))
            {
                // Store data In Franchise detail
                var accFranchise = new CRM_AccountFranchiseDetail
                {
                    CRM_AccountId = acc.CRM_AccountId,
                    FranchiseeName = companyname,
                    EmailAddress = emailAddress,
                    CellNumber = phoneNumber,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now,
                    RegionId = regionId
                };
                SaveCRM_AccountFranchiseDetail(accFranchise);

                //Save Note Data
                if (Note != null && Note != "")
                {
                    CRM_Note CRM_Noteobj = new CRM_Note();
                    CRM_Noteobj.CRM_AccountFranchiseDetailId = accFranchise.CRM_AccountFranchiseDetailId;
                    CRM_Noteobj.Description = Note;
                    CRM_Noteobj.CreatedDate = DateTime.Now;
                    CRM_Noteobj.CreatedBy = userId;
                    CRM_Noteobj.RegionId = regionId;
                    SaveCRM_Note(CRM_Noteobj);
                }
            }

            //_nLogger.Debug($"Store new customer account companyname={companyname} contactname={contactname} " +
            //               $"phonenumber={phoneNumber} email={emailAddress} providertype={providerType} providersource={providerSource} accountType={accountType}");

            return acc;
        }

        public CRM_Document GetCRM_Document_LastRecord()
        {
            var lastDocument = Uow.CRM_Document.GetAll().Max(x => x.CRM_DocumentId);
            return Uow.CRM_Document.GetById(lastDocument);
        }

        public List<CRMPotentialCustomerViewModel> GetAll_CRM_PotentialCustomer(int choice, int type = 0, int user = 0, int region = 0, int loginUserId = 0)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@type", type);
            parameters.Add("@region", region);
            parameters.Add("@user", user);
            parameters.Add("@choice", choice);
            parameters.Add("@LoginUserId", loginUserId);
            parameters.Add("@PageNo", 1);
            parameters.Add("@PageSize", int.MaxValue);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.CRM_spGet_PotentialList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var potentialCustomerList = multipleResult?.Read<CRMPotentialCustomerViewModel>().ToList();
                return potentialCustomerList;
            }
        }

        public CRMPotentialCustomerListViewModel GetPotentialCustomerList(CRMPotentialCustomerListViewModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@type", model.Type);
            parameters.Add("@region", model.RegionId);
            parameters.Add("@user", model.UserId);
            parameters.Add("@choice", model.Choice);
            parameters.Add("@LoginUserId", model.LoginUserId);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.CRM_spGet_PotentialList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var potentialCustomerList = multipleResult?.Read<CRMPotentialCustomerViewModel>().ToList();
                if (potentialCustomerList != null) model.PotentialCustomerList = potentialCustomerList;
                return model;
            }
        }

        public List<CRMPotentialCustomerViewModel> GetAll_CRM_PotentialCustomer_SearchRegionSalesPerson(int choice, int type = 0, int user = 0, string region = null, int loginUserId = 0)
        {
            using (var context = new jkDatabaseEntities())
            {
                var potentialcustomer = context.CRM_spGet_PotentialList(type, region, user, choice, loginUserId,1,100).MapEnumerable<CRMPotentialCustomerViewModel, CRM_spGet_PotentialList_Result>().ToList();
                return potentialcustomer;
            }
        }

        public List<CRMPotentialFranchiseeViewModel> GetAll_CRM_PotentialFranchisee(int choice, int type = 0, int user = 0, int region = 0)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@type", type);
                parmas.Add("@region", region);
                parmas.Add("@user", user);
                parmas.Add("@choice", choice);
                using (var multipleresult = conn.QueryMultiple("dbo.CRM_spGet_PotentialListforFranchisee", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CRMPotentialFranchiseeViewModel>().ToList();
                    }
                }

            }
            return new List<CRMPotentialFranchiseeViewModel>();
            //using (var context = new jkDatabaseEntities())
            //{
            //    var potentialcustomer = context.CRM_spGet_PotentialList(type, region, user, choice).MapEnumerable<CRMPotentialCustomerViewModel, CRM_spGet_PotentialList_Result>().ToList();
            //    return potentialcustomer;
            //}
        }

        public string Get_CRM_PreviousOrNextLead(int choice, int leadId)
        {
            var spName = "CRM_spGet_PreviousNext";
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@choice", choice);
                cmd.Parameters.AddWithValue("@lead_id", leadId);

                conn.Open();
                var returnValue = cmd.ExecuteScalar();
                if (returnValue != null)
                {
                    conn.Close();
                    return returnValue.ToString();
                }
                conn.Close();

            }

            /* using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
             {
                 var parmas = new DynamicParameters();
                 parmas.Add("@choice", choice);
                 parmas.Add("@lead_id", leadId);
                 var value = conn.Query<string>("dbo.CRM_spGet_PreviousNext", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToString();
                 if (value != null)
                     return value;
             } */

            return "";
        }

        public string GetCRM_StageStatusName(int type)
        {
            return GetAll_CRM_StageStatus().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_StageName(int type)
        {
            return GetAll_CRM_Stage().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_ProviderSourceName(int type)
        {
            return GetAll_CRM_ProviderSource().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_ProviderTypeName(int type)
        {
            return GetAll_CRM_ProviderType().FirstOrDefault(x => x.Type == type)?.Name;
        }
        public CRM_ProviderType GetCRM_ProviderTypeById(int Id)
        {
            return GetAll_CRM_ProviderType().FirstOrDefault(x => x.CRM_ProviderTypeId == Id);
        }

        public string GetCRM_AccountTypeName(int type)
        {
            return GetAll_CRM_AccountType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_ActivityTypeName(int type)
        {
            return GetAll_CRM_ActivityType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_ActivityOutComeTypeName(int type)
        {
            return GetAll_CRM_ActivityOutcomeType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_IndustryTypeName(int type)
        {
            return GetAll_CRM_IndustryType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_TaskTypeName(int type)
        {
            return GetAll_CRM_TaskType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_TimeLineTypeName(int type)
        {
            return GetAll_CRM_TimeLineType().FirstOrDefault(x => x.Type == type)?.Name;
        }

        public string GetCRM_StateAbbr(int id)
        {
            return Uow.StateList.GetAll().Where(x => x.StateListId == id).Select(x => x.abbr).FirstOrDefault();
        }
        public string GetCRM_StateName(string abbr)
        {
            return Uow.StateList.GetAll().Where(x => x.abbr == abbr).Select(x => x.StateListId).FirstOrDefault().ToString();
        }

        public int? GetCRM_ProviderTypeIndex(string name)
        {
            return GetAll_CRM_ProviderType().FirstOrDefault(x => x.Name == name)?.Type;
        }

        public int? GetCRM_ProviderSourceIndex(string name)
        {
            return GetAll_CRM_ProviderSource().FirstOrDefault(x => x.Name == name)?.Type;
        }

        public int? GetCRM_StageStatusIndex(string name)
        {
            var a = GetAll_CRM_StageStatus().FirstOrDefault(x => x.Name == name)?.Type;
            return a;
        }

        public List<CRM_Account> GetAll_CRM_Account_ByStageStatusQualifiedLead()
        {
            var newlead = GetCRM_StageStatusIndex(CRM_ServiceConstants.Key_NewLead);
            var unqlead = GetCRM_StageStatusIndex(CRM_ServiceConstants.Key_UnqualifiedLead);
            var junklead = GetCRM_StageStatusIndex(CRM_ServiceConstants.Key_JunkLead);
            var a = Uow.CRM_Account.GetAll().Where(x => x.StageStatus != newlead && x.StageStatus != unqlead && x.StageStatus != junklead).ToList();
            return a;
        }

        public int ImportExcelData(DataTable dt)
        {
            
            //foreach (DataRow row in dt.Rows)
            //{
            //    using (jkDatabaseEntities context = new jkDatabaseEntities())
            //    {
            //        var data = context.crm_spImport_Lead(1, 1, 1, 1, 1, DateTime.Now, DateTime.Now, row["First"].ToString(), row["Last"].ToString(), row["Phone"].ToString(), row["email"].ToString(), row["Company"].ToString(), row["Address1"].ToString(), row["Address2"].ToString(), row["Address3"].ToString(), row["City"].ToString(), row["State"].ToString(), row["Zip"].ToString(), row["email"].ToString(), row["Phone"].ToString(), row["Web Address"].ToString(), row["Ext"].ToString(), row["Title"].ToString(), row["SicCode"].ToString(), row["Sales Volume"].ToString(), row["Sq#Ft#"].ToString(), row["Line of Business"].ToString());
            //    }
            //}

            /*Temporary Code*/
            //using (jkDatabaseEntities jkentities = new jkDatabaseEntities())
            //{
            //    var data = jkentities.CRM_Tmpterritoryimpdata.ToList();
            //    if (data != null)
            //    {
            //        jkentities.CRM_Tmpterritoryimpdata.RemoveRange(data);
            //        jkentities.SaveChanges();
            //    }
            //}

            //using (SqlBulkCopy sqlcopy = new SqlBulkCopy(SQLHelper.ConnectionStringTransaction))
            //{
            //    sqlcopy.DestinationTableName = "CRM_Tmpterritoryimpdata";
            //    sqlcopy.ColumnMappings.Add("company_no", "company_no");
            //    sqlcopy.ColumnMappings.Add("grpname", "grpname");
            //    sqlcopy.ColumnMappings.Add("zips", "zips");
            //    sqlcopy.BatchSize = 1000;
            //    sqlcopy.BulkCopyTimeout = 0;
            //    sqlcopy.WriteToServer(dt);
            //}

            //using (jkDatabaseEntities jkentities = new jkDatabaseEntities())
            //{
            //    var data = jkentities.CRM_Tmpterritoryimpdata.ToList();
            //    if(data != null){
            //        jkentities.CRM_TerritoryImport();
            //    }
            //}


            /*Use This Method to Upload AccountCustomer Data*/

            //Use this Method to insert Customer 
            /* using (jkDatabaseEntities db = new jkDatabaseEntities())
             {
                 var data = db.crm_tempallregiondata.ToList();

                 if (data != null)
                 {
                     db.crm_tempallregiondata.RemoveRange(data);
                     db.SaveChanges();
                 }
             }

             using (SqlBulkCopy sqlcopy = new SqlBulkCopy(SQLHelper.ConnectionStringTransaction))
             {
                 sqlcopy.DestinationTableName = "crm_tempallregiondata";
                 sqlcopy.ColumnMappings.Add("company_no", "company_no");
                 sqlcopy.ColumnMappings.Add("regioncode", "regioncode");
                 sqlcopy.ColumnMappings.Add("enteredby", "enteredby");
                 sqlcopy.ColumnMappings.Add("assignedto", "assignedto"); 
                 sqlcopy.ColumnMappings.Add("createddate", "createddate");
                 sqlcopy.ColumnMappings.Add("leadsource", "leadsource");
                 sqlcopy.ColumnMappings.Add("company", "company");
                 sqlcopy.ColumnMappings.Add("accttype", "accttype");
                 sqlcopy.ColumnMappings.Add("sqrft", "sqrft");
                 sqlcopy.ColumnMappings.Add("salutation", "salutation");
                 sqlcopy.ColumnMappings.Add("fname", "fname");
                 sqlcopy.ColumnMappings.Add("lname", "lname");
                 sqlcopy.ColumnMappings.Add("title", "title");
                 sqlcopy.ColumnMappings.Add("addr1", "addr1");
                 sqlcopy.ColumnMappings.Add("addr2", "addr2"); 
                 sqlcopy.ColumnMappings.Add("city", "city");
                 sqlcopy.ColumnMappings.Add("state", "state");
                 sqlcopy.ColumnMappings.Add("zip", "zip");         
                 sqlcopy.ColumnMappings.Add("phone", "phone");
                 sqlcopy.ColumnMappings.Add("ext", "ext");
                 sqlcopy.ColumnMappings.Add("fax", "fax");
                 sqlcopy.ColumnMappings.Add("celphone", "celphone");
                 sqlcopy.ColumnMappings.Add("email", "email");
                 sqlcopy.ColumnMappings.Add("mailoutinc", "mailoutinc");
                 sqlcopy.ColumnMappings.Add("solddate", "solddate");
                 sqlcopy.ColumnMappings.Add("startdate", "startdate");


                 sqlcopy.ColumnMappings.Add("budgetamount", "budgetamount");
                 sqlcopy.ColumnMappings.Add("contractterm", "contractterm");
                 sqlcopy.ColumnMappings.Add("initclean", "initclean"); 
                 sqlcopy.ColumnMappings.Add("lastmaildate", "lastmaildate");
                 sqlcopy.ColumnMappings.Add("callbackdate", "callbackdate");
                 sqlcopy.ColumnMappings.Add("callbackby", "callbackby");
                 sqlcopy.ColumnMappings.Add("status", "status");
                 sqlcopy.ColumnMappings.Add("lastcontact", "lastcontact");
                 sqlcopy.ColumnMappings.Add("currentven", "currentven");
                 sqlcopy.ColumnMappings.Add("siccode", "siccode");


                 sqlcopy.ColumnMappings.Add("trandate", "trandate");


                 sqlcopy.ColumnMappings.Add("totlocal", "totlocal");
                 sqlcopy.ColumnMappings.Add("contextprice", "contextprice");
                 sqlcopy.ColumnMappings.Add("cb_time", "cb_time");
                 sqlcopy.ColumnMappings.Add("cb_ampm", "cb_ampm");
                 sqlcopy.ColumnMappings.Add("lst_attmpt", "lst_attmpt");
                 sqlcopy.ColumnMappings.Add("attmpt_count", "attmpt_count");
                 sqlcopy.ColumnMappings.Add("notes", "notes");
                 sqlcopy.BatchSize = 1000;
                 sqlcopy.BulkCopyTimeout = 0;
                 sqlcopy.WriteToServer(dt);


             } 

            using (jkDatabaseEntities db = new jkDatabaseEntities())
            {
                var data = db.CRM_tempImportData.ToList();
                if (data != null)
                {
                    db.CRM_Import();
                }
            }  */

            /*Map the Data*/
            using (SqlBulkCopy sqlcopy = new SqlBulkCopy(SQLHelper.ConnectionStringTransaction))
            {
                sqlcopy.DestinationTableName = "CRM_tempImportData";
                sqlcopy.ColumnMappings.Add("company", "Company");
                sqlcopy.ColumnMappings.Add("addr1", "Address1");
                sqlcopy.ColumnMappings.Add("city", "City");
                sqlcopy.ColumnMappings.Add("state", "State");
                sqlcopy.ColumnMappings.Add("country", "Country");
                sqlcopy.ColumnMappings.Add("zip", "Zip");
                sqlcopy.ColumnMappings.Add("phone", "Phone");
                sqlcopy.ColumnMappings.Add("Title", "Title");
                sqlcopy.ColumnMappings.Add("fname", "FirstName");
                sqlcopy.ColumnMappings.Add("lname", "LastName");

                sqlcopy.ColumnMappings.Add("siccode", "SicCode");
                //if (dt.Columns.Contains("ext"))
                //{
                //    sqlcopy.ColumnMappings.Add("ext", "Ext");
                //}
                if (dt.Columns.Contains("fax"))
                {
                    sqlcopy.ColumnMappings.Add("fax", "FaxNumber");
                }

                if (dt.Columns.Contains("email"))
                {
                    sqlcopy.ColumnMappings.Add("email", "email");
                }

                if (dt.Columns.Contains("addr2"))
                {
                    sqlcopy.ColumnMappings.Add("addr2", "Address2");
                }
                if (dt.Columns.Contains("addr3"))
                {
                    sqlcopy.ColumnMappings.Add("addr3", "Address3");
                }

                if (dt.Columns.Contains("sqrft"))
                {
                    sqlcopy.ColumnMappings.Add("sqrft", "SqFt");
                }
                if (dt.Columns.Contains("celphone"))
                {
                    sqlcopy.ColumnMappings.Add("celphone", "CellPhone");
                }
                if (dt.Columns.Contains("assignedto"))
                {
                    sqlcopy.ColumnMappings.Add("assignedto", "AssignedTo");
                }
                if (dt.Columns.Contains("enteredby"))
                {
                    sqlcopy.ColumnMappings.Add("enteredby", "EnteredBy");
                }
                if (dt.Columns.Contains("leadsource"))
                {
                    sqlcopy.ColumnMappings.Add("leadsource", "LeadSource");
                }
                if (dt.Columns.Contains("accttype"))
                {
                    sqlcopy.ColumnMappings.Add("accttype", "AccountType");
                }

                if (dt.Columns.Contains("budgetamt"))
                {
                    sqlcopy.ColumnMappings.Add("budgetamt", "BudgetAmount");
                }
                //if (dt.Columns.Contains("contractam"))
                //{
                //    sqlcopy.ColumnMappings.Add("contractam", "ContractTam");
                //}
                //if (dt.Columns.Contains("initclean"))
                //{
                //    sqlcopy.ColumnMappings.Add("initclean", "InitClean");
                //}
                //if (dt.Columns.Contains("heardfrom"))
                //{
                //    sqlcopy.ColumnMappings.Add("heardfrom", "HeardFrom");
                //}
                //if (dt.Columns.Contains("callbdate"))
                //{
                //    sqlcopy.ColumnMappings.Add("callbdate", "CallBackDate");
                //}
                //if (dt.Columns.Contains("callbackby"))
                //{
                //    sqlcopy.ColumnMappings.Add("callbackby", "CallBackBy");
                //}
                //if (dt.Columns.Contains("status"))
                //{
                //    sqlcopy.ColumnMappings.Add("status", "Status");
                //}
                //if (dt.Columns.Contains("lastcontac"))
                //{
                //    sqlcopy.ColumnMappings.Add("lastcontac", "LastContact");
                //}
                if (dt.Columns.Contains("currentven"))
                {
                    sqlcopy.ColumnMappings.Add("currentven", "CurrentProvider");
                }
                //if (dt.Columns.Contains("lst_attmpt"))
                //{
                //    sqlcopy.ColumnMappings.Add("lst_attmpt", "LastAttempt");
                //}
                //if (dt.Columns.Contains("company_no"))
                //{
                //    sqlcopy.ColumnMappings.Add("company_no", "Company_no");
                //}
                //if (dt.Columns.Contains("regioncode"))
                //{
                //    sqlcopy.ColumnMappings.Add("regioncode", "RegionCode");
                //}
                if (dt.Columns.Contains("id"))
                {
                    sqlcopy.ColumnMappings.Add("id", "LeadId");
                }
                if (dt.Columns.Contains("leadnotes"))
                {
                    sqlcopy.ColumnMappings.Add("leadnotes", "LeadNote");
                }

                /* Format the columns with respective to Table in the Database */
              /*  dt.Columns.Cast<DataColumn>().ToList()
                    .ForEach(x => sqlcopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(x.ColumnName, x.ColumnName)));*/

                sqlcopy.BatchSize = 1000;
                sqlcopy.BulkCopyTimeout = 0;
                sqlcopy.WriteToServer(dt);
            }

            using (jkDatabaseEntities db = new jkDatabaseEntities())
            {
                var data = db.CRM_tempImportData.ToList();
                if (data != null)
                {
                    ObjectParameter duplicateCount = new ObjectParameter("DuplicateCount", typeof(int)); //Create Object parameter to receive a output value.It will behave like output parameter  
                    db.CRM_Import(duplicateCount);
                    return Convert.ToInt32(duplicateCount.Value);
                }
            }
            //  Thread MyThread = new Thread(MyCallbackFunction);
            // MyThread.Start();

            return 0;
        }

        public DataTable DuplicateRecordInExcel(DataTable dt)
        {
            using (var context = new jkDatabaseEntities())
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    string companyName = Convert.ToString(dt.Rows[i]["Business Name"]);
                    if (!String.IsNullOrEmpty(companyName))
                    {
                        CRM_AccountCustomerDetail customerDetail = context.CRM_AccountCustomerDetail.FirstOrDefault(o => o.CompanyName.ToLower() == companyName.ToLower());
                        if (customerDetail != null)
                        {
                            dt.Rows[i].Delete();
                        }
                    }
                }
                dt.AcceptChanges();
            }
            return dt;
        }

        public CRM_AccountCustomerDetail ImportFranchiseExcelData(DataTable dt)
        {
            /*Use This Method to Upload AccountFranchise Data*/

            using (jkDatabaseEntities db = new jkDatabaseEntities())
            {
                var data = db.CRM_FranchiseTempData.ToList();

                if (data != null)
                {
                    db.CRM_FranchiseTempData.RemoveRange(data);
                    db.SaveChanges();
                }
            }

            using (SqlBulkCopy sqlcopy = new SqlBulkCopy(SQLHelper.ConnectionStringTransaction))
            {
                sqlcopy.DestinationTableName = "CRM_FranchiseTempData";
                sqlcopy.ColumnMappings.Add("company_no", "company_no");
                sqlcopy.ColumnMappings.Add("regionid", "regionid");
                sqlcopy.ColumnMappings.Add("createdate", "createdate");
                sqlcopy.ColumnMappings.Add("salutation", "salutation");
                sqlcopy.ColumnMappings.Add("fname", "fname");
                sqlcopy.ColumnMappings.Add("lname", "lname");
                sqlcopy.ColumnMappings.Add("addr", "addr");
                sqlcopy.ColumnMappings.Add("addr2", "addr2");
                sqlcopy.ColumnMappings.Add("addr3", "addr3");
                sqlcopy.ColumnMappings.Add("city", "city");
                sqlcopy.ColumnMappings.Add("zip", "zip");
                sqlcopy.ColumnMappings.Add("hphone", "hphone");
                sqlcopy.ColumnMappings.Add("wphone", "wphone");
                sqlcopy.ColumnMappings.Add("ext", "ext");
                sqlcopy.ColumnMappings.Add("celphone", "celphone");
                sqlcopy.ColumnMappings.Add("email", "email");
                sqlcopy.ColumnMappings.Add("leadsource", "leadsource");
                sqlcopy.ColumnMappings.Add("initialcon", "initialcon");
                sqlcopy.ColumnMappings.Add("infosent", "infosent");
                sqlcopy.ColumnMappings.Add("country", "country");
                sqlcopy.ColumnMappings.Add("countryint", "countryint");
                sqlcopy.ColumnMappings.Add("lastcontdt", "lastcontdt");
                sqlcopy.ColumnMappings.Add("lastcontby", "lastcontby");
                sqlcopy.ColumnMappings.Add("callbdate", "callbdate");
                sqlcopy.ColumnMappings.Add("callbby", "callbby");


                sqlcopy.ColumnMappings.Add("disclosed", "disclosed");
                sqlcopy.ColumnMappings.Add("datecanbuy", "datecanbuy");
                sqlcopy.ColumnMappings.Add("estclosedat", "estclosedat");
                sqlcopy.ColumnMappings.Add("fivedpaper", "fivedpaper");
                sqlcopy.ColumnMappings.Add("status", "status");
                sqlcopy.ColumnMappings.Add("solddate", "solddate");
                sqlcopy.ColumnMappings.Add("soldby", "soldby");
                sqlcopy.ColumnMappings.Add("franplan", "franplan");
                sqlcopy.ColumnMappings.Add("franamt", "franamt");
                sqlcopy.ColumnMappings.Add("downamt", "downamt");


                sqlcopy.ColumnMappings.Add("employer", "employer");


                sqlcopy.ColumnMappings.Add("position", "position");
                sqlcopy.ColumnMappings.Add("schtraindte", "schtraindte");
                sqlcopy.ColumnMappings.Add("jkfullpart", "jkfullpart");
                sqlcopy.ColumnMappings.Add("cash2invst", "cash2invst");
                sqlcopy.ColumnMappings.Add("state", "state");
                sqlcopy.ColumnMappings.Add("f_id", "f_id");
                sqlcopy.ColumnMappings.Add("corpgen", "corpgen");
                sqlcopy.ColumnMappings.Add("mi", "mi");

                sqlcopy.ColumnMappings.Add("mailoutinc", "mailoutinc");
                sqlcopy.ColumnMappings.Add("fphone", "fphone");
                sqlcopy.ColumnMappings.Add("heardfrom", "heardfrom");
                sqlcopy.ColumnMappings.Add("lastmailda", "lastmailda");
                sqlcopy.ColumnMappings.Add("trandate", "trandate");
                sqlcopy.ColumnMappings.Add("investrang", "investrang");
                sqlcopy.ColumnMappings.Add("mastrequest", "mastrequest");
                sqlcopy.ColumnMappings.Add("leadnotes", "leadnotes");
                sqlcopy.BatchSize = 1000;
                sqlcopy.BulkCopyTimeout = 0;
                sqlcopy.WriteToServer(dt);


            }


            return null;
        }

        public static void MyCallbackFunction()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CRM_tempImportData> Data = context.CRM_tempImportData.ToList();
                foreach (var item in Data)
                {
                    //context.crm_spImport_Lead(1, 1, 1, 1, 1, DateTime.Now, DateTime.Now, item.FirstName, item.LastName, item.Phone, item.email, item.Company, item.Address1, item.Address2, item.Address3, item.City, item.State, item.Zip, item.email, item.Phone, item.WebAddress, item.Ext, item.Title, item.SicCode, item.SalesVolume, item.SqFt, item.LineofBusiness);

                    // Store data in CRM_Account
                    var acc = new CRM_Account
                    {
                        AccountType = 1,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        EmailAddress = item.email,
                        PhoneNumber = item.Phone,
                        ProviderType = 19,
                        ProviderSource = 1,
                        Stage = 1,
                        StageStatus = 1,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    context.CRM_Account.Add(acc);
                    context.SaveChanges();

                    // Store data In Customer detail
                    var accCustomer = new CRM_AccountCustomerDetail
                    {
                        CRM_AccountId = acc.CRM_AccountId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CompanyName = item.Company,
                        CompanyAddressLine1 = item.Address1,
                        CompanyAddressLine2 = item.Address2,
                        CompanyAddressLine3 = item.Address3,
                        CompanyCity = item.City,
                        CompanyState = item.State,
                        CompanyZipCode = item.Zip,
                        CompanyPhoneNumber = item.Phone,
                        Ext = item.Ext,
                        Title = item.Title,
                        CompanyEmailAddress = item.email,
                        SicCode = item.SicCode,
                        SalesVolume = item.SalesVolume,
                        LineofBusiness = item.LineofBusiness,
                        SqFt = item.SqFt,
                        CompanyWebSite = item.WebAddress

                    };
                    context.CRM_AccountCustomerDetail.Add(accCustomer);
                    context.SaveChanges();
                }
            }

        }

        public IQueryable<ServiceTypeList> GetServiceTypeList()
        {
            var qry = Uow.ServiceTypeList.GetAll();
            return qry;
        }

        public IQueryable<FrequencyList> GetFrequencyList()
        {
            var qry = Uow.FrequencyList.GetAll();
            return qry;
        }

        public List<JKViewModels.Customer.CleanFrequencyListViewModel> GetCleanFrequencyList()
        {
            using (var context = new jkDatabaseEntities())
            {
                var qry = context.CleanFrequencyLists.MapEnumerable<JKViewModels.Customer.CleanFrequencyListViewModel, CleanFrequencyList>().ToList();
                return qry;
            }
        }

        public CRMScheduleUserCalendarViewModel GetCRMScheduleByUser(int loginUserId, int regionId = 0, string lstUser = null, string lstType = null)
        {
            CRMScheduleUserCalendarViewModel _calendar = new CRMScheduleUserCalendarViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@loginUserId", loginUserId);
                parmas.Add("@regionId", regionId);
                parmas.Add("@lstUser", lstUser);
                parmas.Add("@lstType", lstType);
                using (var multipleresult = conn.QueryMultiple("dbo.spGetCRMScheduleByUserHierarchy", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        _calendar.lstCRMUserHierarchy = multipleresult.Read<CRMScheduleUserHierarchy>().ToList();
                        _calendar.lstCRMScheduleUserCalender = multipleresult.Read<CRMScheduleViewModel>().ToList();
                    }
                }
            }
            return _calendar;
        }

        #endregion

        #region CRM_Contacts > Queries

        public List<CRMContactTypeViewModel> GetContactTypes()
        {
            var result = new List<CRMContactTypeViewModel>();

            try
            {

            }
            catch (Exception)
            {

            }

            return result;
        }

        #endregion

        #region CRM Stage Setting
        public StageSettingViewModel GetStageSettingList(StageSettingViewModel model, string StageType = "")
        {
            model.StageSettingList = new List<StageSettingModel>();

            SqlParameter[] parmList = {
            new SqlParameter("@StateType",StageType)
            };

            //Get all the active user without super admin.
            using (DataSet userDS = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.CRM_StageSettingList, parmList))
            {
                if (userDS != null && userDS.Tables != null && userDS.Tables.Count > 0)
                {
                    DataTable ContentDT = userDS.Tables[0];
                    if (ContentDT != null && ContentDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ContentDT.Rows)
                        {
                            model.StageSettingList.Add(GetDataRowToEntity<StageSettingModel>(dr));
                        }
                    }
                }
            }

            return model;
        }

        public StageSettingViewModel SaveStageSetting(StageSettingViewModel model)
        {
            int ErrorCode = 0;
            string ErrorMessage = string.Empty;

            SqlParameter pErrorCode = new SqlParameter("@ErrorCode", ErrorCode); pErrorCode.Direction = ParameterDirection.Output;
            SqlParameter pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage); pErrorMessage.Direction = ParameterDirection.Output; pErrorMessage.Size = 8000;
            SqlParameter[] parmList = {
            new SqlParameter("@CRM_StageTimeCalculationId",model.CRM_StageTimeCalculationId),
             new SqlParameter("@CRM_StageStatusId",model.CRM_StageStatusId),
            new SqlParameter("@DayLeft",model.DayLeft),
            new SqlParameter("@HourLeft",model.HourLeft),
            new SqlParameter("@MinuteLeft",model.MinuteLeft),
            new SqlParameter("@IsEnable",model.IsEnable),
            new SqlParameter("@CreatedBy",model.CreatedBy),
            new SqlParameter("@ActionType",model.ActionType),
            pErrorCode,
            pErrorMessage,
            };

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, DBConstants.CRM_SaveStageSetting, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {        //common function for Row to Entity GetDataRowToEntity<EntityModel>(ds.Table[0].Rows[0])       }    
                    model = GetDataRowToEntity<StageSettingViewModel>(ds.Tables[0].Rows[0]);
                }

                if (Convert.ToInt32(pErrorCode.Value) > 0)
                    model.objErrorModel.Add(new ErrorModel { ErrorCode = Convert.ToInt32(pErrorCode.Value), ErrorMessage = Convert.ToString(pErrorMessage.Value) });

            }
            return model;
        }

        public IQueryable<AuthUserLogin> GetAllUserList()
        {
            return Uow.AuthUserLogin.GetAll();
        }

        public List<CRMScheduleUserHierarchy> GetAllUserRelation()
        {
            List<CRMScheduleUserHierarchy> _user = new List<CRMScheduleUserHierarchy>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                using (var multipleresult = conn.QueryMultiple("dbo.crm_spGet_GetAllUserRelationList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        _user = multipleresult.Read<CRMScheduleUserHierarchy>().ToList();
                    }
                }
            }
            return _user;
        }

        public List<CRMScheduleUserHierarchy> SaveUserRelation(CRMScheduleUserHierarchy userRelation, int loginId)
        {
            List<CRMScheduleUserHierarchy> _user = new List<CRMScheduleUserHierarchy>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@ParentUserId", userRelation.ParentUserId);
                parmas.Add("@UserId", userRelation.UserId);
                parmas.Add("@RegionId", userRelation.RegionId);
                parmas.Add("@Id", userRelation.Id);
                parmas.Add("@LstUserId", userRelation.lstUserId);
                parmas.Add("@LstRegionId", userRelation.lstRegionId);
                parmas.Add("@CreatedBy", loginId);
                parmas.Add("@Condition", userRelation.Condition);

                using (var multipleresult = conn.QueryMultiple("dbo.crm_sp_InsertUpdateUserRelation", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        _user = multipleresult.Read<CRMScheduleUserHierarchy>().ToList();
                    }
                }
            }

            return _user;
        }
        #endregion

        #region Lead ReAssign


        /// <summary>
        /// Get Sales Users
        /// </summary>
        /// <returns></returns>
        public List<SalesUser> GetAll_CRM_ReAssignLeadUserList()
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {


                using (var multipleresult = conn.QueryMultiple("dbo.CRM_spGet_LeadReAssignSalesUserList", commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<SalesUser>().ToList();
                    }
                }
            }
            return new List<SalesUser>();
        }
        /// <summary>
        /// Get Territories By UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<CRM_ReAssignTerritory> CRM_spGet_LeadReAssignTerritoryListByUserId(int UserId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@UserId", UserId);

                using (var multipleresult = conn.QueryMultiple("dbo.CRM_spGet_LeadReAssignTerritoryListByUserId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<CRM_ReAssignTerritory>().ToList();
                    }
                }
            }
            return new List<CRM_ReAssignTerritory>();

        }

        public LeadReAssignViewModel GetAll_CRM_ReAssignLeadList(int UserId, int TerritoryId)
        {
            LeadReAssignViewModel result = new LeadReAssignViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@UserId", UserId);
                parmas.Add("@TerritoryId", TerritoryId);

                using (var multipleresult = conn.QueryMultiple("dbo.CRM_spGet_LeadReAssignLeadsData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        result.LeadReAssignLeadList = multipleresult.Read<CRMLeadListViewModel>().ToList();
                        result.LeadReAssignSalesUser = multipleresult.Read<SalesUser>().ToList();
                        return result;
                    }
                }

            }
            return new LeadReAssignViewModel();
        }

        public List<SalesUser> CRM_spGet_LeadReAssignUsersListByTerritoryId(int TerritoryId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@TerritoryId", TerritoryId);

                using (var multipleresult = conn.QueryMultiple("dbo.CRM_spGet_LeadReAssignUsersListByTerritoryId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return multipleresult.Read<SalesUser>().ToList();
                    }
                }
            }
            return new List<SalesUser>();
        }

        public bool AssignLeadsToUser(List<LeadListView> lstLeads)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("CRM_spSave_ReassignData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LeadsAssignTable", lstLeads.ToDataTable());
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public CRM_Schedule SaveCRM_ScheduleData(CRM_Schedule model)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                try
                {
                    JKApi.Data.DAL.CRM_Schedule crm_schedule = context.CRM_Schedule.SingleOrDefault(o => o.CRM_ScheduleId == model.CRM_ScheduleId);
                    var id = 0;
                    if (crm_schedule == null)
                    {
                        crm_schedule = new Data.DAL.CRM_Schedule();
                    }
                    else
                    {
                        id = crm_schedule.CRM_ScheduleId;
                    }

                    crm_schedule.CRM_ScheduleId = model.CRM_ScheduleId;
                    crm_schedule.Title = model.Title;
                    crm_schedule.Description = model.Description;
                    crm_schedule.StartDate = model.StartDate;
                    crm_schedule.CRM_AccountFranchiseDetailId = model.CRM_AccountFranchiseDetailId;
                    crm_schedule.IsFromOutlook = model.IsFromOutlook;
                    crm_schedule.OutlookSyncDate = model.OutlookSyncDate;
                    crm_schedule.Location = model.Location;
                    crm_schedule.EndDate = model.EndDate;
                    crm_schedule.IsAllDay = model.IsAllDay;
                    crm_schedule.CRM_ScheduleTypeId = model.CRM_ScheduleTypeId;
                    crm_schedule.CRM_StageStatusType = model.CRM_StageStatusType;
                    crm_schedule.RegionId = model.RegionId <= 0 ? SelectedRegionId : model.RegionId;
                    crm_schedule.AuthUserLoginId = model.AuthUserLoginId;
                    crm_schedule.PurposeId = model.PurposeId;
                    crm_schedule.Purpose = model.Purpose;
                    crm_schedule.IsActive = true;
                    crm_schedule.ClassId = model.ClassId;
                    crm_schedule.TypeListId = model.TypeListId;
                    crm_schedule.TypeListId = model.TypeListId <= 0 ? 1 : model.TypeListId;

                    if (id == 0)
                    {
                        crm_schedule.CreatedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        crm_schedule.CreatedDate = DateTime.Now;
                        context.CRM_Schedule.Add(crm_schedule);
                    }
                    else
                    {

                        crm_schedule.CreatedDate = crm_schedule.CreatedDate;
                        crm_schedule.CreatedBy = crm_schedule.CreatedBy;
                        crm_schedule.ModifiedBy = int.Parse(ClaimView.GetCLAIM_USERID());
                        crm_schedule.ModifiedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                    return crm_schedule;
                }
                catch (Exception e)
                {
                    return null;
                }

            }


        }

        public IEnumerable<CRM_PurposeTypeModel> GetAllPurposeType(int statusListId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                try
                {
                    var query = @"EXEC CRM_Get_ScheduleReport 4,null,null,null,null,null,null,@StatusListId";
                    return conn.Query<CRM_PurposeTypeModel>(query, new
                    {
                        StatusListId = statusListId
                    });
                }
                catch
                {
                    return null;
                }

            }
        }
        #endregion

        #region Call Log List

        public List<CRM_CallLogListModel> GetAll_CRMCallLog(int accountId)
        {
            List<CRM_CallLogListModel> _calllog = new List<CRM_CallLogListModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@accountId", accountId);


                using (var multipleresult = conn.QueryMultiple("dbo.crm_spGet_CallLogList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        _calllog = multipleresult.Read<CRM_CallLogListModel>().ToList();
                    }
                }
            }

            return _calllog;
        }

        public ViewCallLogListModel GetCallLogListByAccount(ViewCallLogListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AccountId", model.AccountId);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCrmCallLogListByAccount, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var callLogList = multipleResult?.Read<CallLogModel>().ToList();
                if (callLogList != null) model.CallLogList = callLogList;
                return model;
            }
        }

        public ViewCallLogListModel GetCallLogListByAccountCustomerDetail(ViewCallLogListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AccountCustomerDetailId", model.AccountCustomerDetailId);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetCrmCallLogListByAccountCustomerDetail, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var callLogList = multipleResult?.Read<CallLogModel>().ToList();
                if (callLogList != null) model.CallLogList = callLogList;
                return model;
            }
        }

        public CallLogModel GetCallLog(int callLogId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CallLogId", callLogId);

            using (var result = FmsDbConn.QuerySingleOrDefault<CallLogModel>(DBConstants.sp_GetCrmCallLog, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public CallLogModel AddOrUpdateCallLog(CallLogModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CallLogId", model.CallLogId);
            parameters.Add("@AccountId", model.AccountId);
            parameters.Add("@AccountCustomerDetailId", model.AccountCustomerDetailId);
            parameters.Add("@LeadSource", model.LeadSource);
            parameters.Add("@CallResultId", model.CallResultId);
            parameters.Add("@StageStatus", model.StageStatus);
            parameters.Add("@NoteTypeId", model.NoteTypeId);
            parameters.Add("@SpokeWith", model.SpokeWith);
            parameters.Add("@Note", model.Note);
            parameters.Add("@CallLogDate", model.CallLogDate);
            parameters.Add("@CallBack", model.CallBack);
            parameters.Add("@CallBackTime", model.CallBackTime);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate ?? DateTime.Now);

            using (var result = FmsDbConn.QuerySingleOrDefault<CallLogModel>(DBConstants.sp_AddOrUpdateCrmCallLog, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public AuthUserLogin GetAuthUserLoginById(int userId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                try
                {
                    var query = @"EXEC CRM_Get_ScheduleReport 5,null,null,null,null,null,null,@StatusListId";
                    return conn.Query<AuthUserLogin>(query, new { StatusListId = userId }).FirstOrDefault();
                }
                catch
                {
                    return null;
                }

            }
        }

        #endregion
    }
}