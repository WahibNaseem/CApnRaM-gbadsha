using System;
using System.Linq;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.Inspection;
using JKViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using Dapper;
using JKViewModels.Common;
using JKViewModels.Inspection;
using FormItemType = JKViewModels.FormItemType;

namespace JKApi.Service.Service.Inspection
{
    public class InspectionService : BaseService, IInspectionService
    {
        private readonly ICacheProvider _cacheProvider;

        #region InspectionService > Constructor

        public InspectionService(IJKEfUow uow, ICacheProvider cacheProvider)
        {
            Uow = uow;
            _cacheProvider = cacheProvider;
        }

        #endregion

        #region InspectionService > GetAll

        //public IQueryable<Data.DAL.Inspection> GetAll_Inspection()
        //{
        //    return Uow.InspectionRepository.GetAll();
        //}

        public IQueryable<InspectionForm> GetAll_InspectionForm()
        {
            return Uow.InspectionFormRepository.GetAll();
        }

        public IQueryable<InspectionFormItem> GetAll_InspectionFormItem()
        {
            return Uow.InspectionFormItemRepository.GetAll();
        }

        public IQueryable<InspectionStatu> GetAll_InspectionStatus()
        {
            if (!_cacheProvider.Contains(CacheKeyName.Inspection_GetAll_InspectionStatus))
            {
                _cacheProvider.Set(CacheKeyName.Inspection_GetAll_InspectionStatus,
                    Uow.InspectionStatusRepository.GetAll());
            }
            return (IQueryable<InspectionStatu>) _cacheProvider.Get(CacheKeyName.Inspection_GetAll_InspectionStatus);
        }

        #endregion

        #region InspectionService > Get

        //public Data.DAL.Inspection Get_Inspection(int id)
        //{
        //    return Uow.InspectionRepository.GetById(id);
        //}

        public InspectionForm Get_InspectionForm(int id)
        {
            return Uow.InspectionFormRepository.GetById(id);
        }

        public InspectionFormItem Get_InspectionFormItem(int id)
        {
            return Uow.InspectionFormItemRepository.GetById(id);
        }

        public InspectionStatu Get_InspectionStatus(int id)
        {
            if (!_cacheProvider.Contains(CacheKeyName.Inspection_GetAll_InspectionStatus))
            {
                return Uow.InspectionStatusRepository.GetById(id);
            }
            var inspectionStatus =
                (IQueryable<InspectionStatu>) _cacheProvider.Get(CacheKeyName.Inspection_GetAll_InspectionStatus);
            return inspectionStatus.FirstOrDefault(x => x.InspectionStatusId == id);
        }

        #endregion

        #region InspectionService > SaveOrUpdate

        //public Data.DAL.Inspection SaveOrUpdate_Inspection(Data.DAL.Inspection inspection)
        //{
        //    var isNew = inspection.InspectionId == 0;
        //    if (isNew)
        //    {
        //        Uow.InspectionRepository.Add(inspection);
        //    }
        //    else
        //    {
        //        Uow.InspectionRepository.Update(inspection);
        //    }
        //    Uow.Commit();
        //    return inspection;
        //}

        public InspectionForm SaveOrUpdate_InspectionForm(InspectionForm inspectionForm)
        {
            var isNew = inspectionForm.InspectionFormId == 0;
            if (isNew)
            {
                Uow.InspectionFormRepository.Add(inspectionForm);
            }
            else
            {
                Uow.InspectionFormRepository.Update(inspectionForm);
            }
            Uow.Commit();
            return inspectionForm;
        }

        public InspectionFormItem SaveOrUpdate_InspectionFormItem(InspectionFormItem inspectionFormItem)
        {
            var isNew = inspectionFormItem.InspectionFormItemId == 0;
            if (isNew)
            {
                Uow.InspectionFormItemRepository.Add(inspectionFormItem);
            }
            else
            {
                Uow.InspectionFormItemRepository.Update(inspectionFormItem);
            }
            Uow.Commit();
            return inspectionFormItem;
        }

        public InspectionStatu SaveOrUpdate_InspectionStatus(InspectionStatu inspectionStatus)
        {
            var isNew = inspectionStatus.InspectionStatusId == 0;
            if (isNew)
            {
                Uow.InspectionStatusRepository.Add(inspectionStatus);
            }
            else
            {
                Uow.InspectionStatusRepository.Update(inspectionStatus);
            }
            _cacheProvider.Remove(CacheKeyName.Inspection_GetAll_InspectionStatus);
            Uow.Commit();
            return inspectionStatus;
        }

        #endregion

        #region InspectionService > Delete

        //public void Delete_Inspection(int id)
        //{
        //    Uow.InspectionRepository.Delete(id);
        //    Uow.Commit();
        //}

        public void Delete_InspectionForm(int id)
        {
            Uow.InspectionFormRepository.Delete(id);
            Uow.Commit();
        }

        public void Delete_InspectionFormItem(int id)
        {
            Uow.InspectionFormItemRepository.Delete(id);
            Uow.Commit();
        }

        public void Delete_InspectionStatus(int id)
        {
            Uow.InspectionStatusRepository.Delete(id);
            Uow.Commit();
        }

        #endregion

        #region Template 

        public List<FromTemplateAPIModel> GetFormTemplateList(int formTemplateId)
        {
            List<FromTemplateAPIModel> model = new List<FromTemplateAPIModel>();

            SqlParameter[] parmList = {new SqlParameter("@FormTemplateId", formTemplateId)};

            using (
                DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                    DBConstants.sp_getFormTemplateList, parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.Add(GetDataRowToEntity<FromTemplateAPIModel>(dr));
                    }
                }

                List<FromTemplateAreaAPIModel> AreaModel = new List<FromTemplateAreaAPIModel>();
                if (ds != null && ds.Tables.Count > 1)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        AreaModel.Add(GetDataRowToEntity<FromTemplateAreaAPIModel>(dr));
                    }
                }

                List<FromTemplateItemAPIModel> ItemModel = new List<FromTemplateItemAPIModel>();
                if (ds != null && ds.Tables.Count > 2)
                {
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        ItemModel.Add(GetDataRowToEntity<FromTemplateItemAPIModel>(dr));
                    }
                }

                foreach (FromTemplateAPIModel item in model)
                {
                    item.ListFromTemplateArea = AreaModel.Where(x => x.FormTemplateId == item.FormTemplateId).ToList();

                    foreach (FromTemplateAreaAPIModel AreaItem in item.ListFromTemplateArea)
                    {
                        AreaItem.ListFromTemplateItem =
                            ItemModel.Where(x => x.TemplateAreaId == AreaItem.TemplateAreaId).ToList();
                    }
                }
            }
            return model;
        }

        public List<AccountTypeListAPIModel> AccountTypeList()
        {
            List<AccountTypeListAPIModel> model = new List<AccountTypeListAPIModel>();

            using (
                DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                    DBConstants.sp_getAccountTypeList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        model.Add(GetDataRowToEntity<AccountTypeListAPIModel>(dr));
                    }
                }
            }
            return model;
        }

        #endregion

        #region Inspection API

        // ======================================================================================
        #region InspectionService > Private
        // ======================================================================================

        public void _updateInspectionFormSectionInfo(int inspectionFormSectionId, int inspectionFormId,
            bool sectionAutoFail, string sectionAutoFailReason)
        {
            var formItems = GetInspectionFormItemListBySection(inspectionFormSectionId);
            var totalPoints = 0;
            var totalRequired = 0;
            var passPoints = 0.0;
            var failPoints = 0.0;
            var needImprovementPoints = 0.0;
            var sectionStatus = InspectionCompletionStatus.Unknown;
            var scorePercent = 0.0;

            if (sectionAutoFail)
            {
                passPoints = 0;
                needImprovementPoints = 0;
                sectionStatus = InspectionCompletionStatus.Completed;
                foreach (var formItem in formItems)
                {
                    if (formItem.IsRequired)
                    {
                        totalRequired++;
                    }
                }
                failPoints = 1 * totalRequired;
            }
            else
            {
                foreach (var formItem in formItems)
                {
                    if (formItem.IsDirty && formItem.IsRequired && formItem.FormItemType == FormItemType.FormItemTypeInspection)
                    {
                        totalPoints++;
                        try
                        {
                            dynamic formItemValue = JsonConvert.DeserializeObject<dynamic>(formItem.FormItemValue);
                            var status = formItemValue.Status;
                            if (status != InspectionStatus.Unknown) totalRequired++;
                            if (status == InspectionStatus.Pass) passPoints += 1;
                            else if (status == InspectionStatus.Fail) failPoints += 1;
                            else if (status == InspectionStatus.NeedImprovement) needImprovementPoints += 0.5;
                        }
                        catch (Exception ex)
                        {
                            NLogger.Error($"Exception: {ex}");
                        }
                    }
                }

                var totalScore = passPoints + needImprovementPoints;
                scorePercent = (totalPoints > 0 ? totalScore / totalPoints : 0) * 100;

                if (totalPoints > 0)
                {
                    sectionStatus = totalRequired == totalPoints 
                        ? InspectionCompletionStatus.Completed : InspectionCompletionStatus.Pending;
                }
            }

            AddOrUpdateInspectionFormSection(new InspectionFormSectionModel
            {
                InspectionFormSectionId = inspectionFormSectionId,
                PassPoints = Convert.ToDecimal(passPoints),
                FailPoints = Convert.ToDecimal(failPoints),
                NeedImprovementPoints = Convert.ToDecimal(needImprovementPoints),
                ScorePercent = Convert.ToDecimal(scorePercent),
                SectionStatus = (int)sectionStatus,
                SectionAutoFail = sectionAutoFail,
                SectionAutoFailReason = sectionAutoFailReason,
                IsEnable = true,
                IsDelete = false
            });

            _updateInspectionScore(inspectionFormId);
        }

        private void _updateInspectionScore(int inspectionFormId)
        {
            var sections = GetInspectionFormSectionListByForm(inspectionFormId);

            var passPoints = 0.0;
            var failPoints = 0.0;
            var needImprovementPoints = 0.0;
            var totalScorePercent = 0.0;

            foreach (var section in sections)
            {
                passPoints += (double)section.PassPoints;
                failPoints += (double)section.FailPoints;
                needImprovementPoints += (double)section.NeedImprovementPoints;
                totalScorePercent += (double)section.ScorePercent;
            }

            var count = sections.Count;
            var scorePercent = count > 0 ? totalScorePercent / count : 0.0f;
            InspectionStatus inspectionStatus;

            if (scorePercent < 80)
            {
                inspectionStatus = InspectionStatus.Fail;
            }
            else if (scorePercent >= 80 && scorePercent < 89)
            {
                inspectionStatus = InspectionStatus.NeedImprovement;
            }
            else
            {
                inspectionStatus = InspectionStatus.Pass;
            }

            AddOrUpdateInspectionForm(new InspectionFormModel
            {
                InspectionFormId = inspectionFormId,
                InspectionStatusId = inspectionStatus,
                PassPoints = Convert.ToDecimal(passPoints),
                FailPoints = Convert.ToDecimal(failPoints),
                NeedImprovementPoints = Convert.ToDecimal(needImprovementPoints),
                ScorePercent = Convert.ToDecimal(scorePercent),
                IsEnable = true,
                IsDelete = false,
                ModifiedDate = DateTime.Now
            });
        }

        private InspectionFormModel _resetInspectionForm(int inspectionFormId)
        {
            var inspectionForm = GetDetailInspectionForm(inspectionFormId);
            if (inspectionForm != null)
            {
                inspectionForm.InspectionStatusId = InspectionStatus.Unknown;
                inspectionForm.CallDate = DateTime.Today;
                inspectionForm.RecordedDate = null;
                inspectionForm.UploadedDate = null;
                inspectionForm.IsCompleted = false;
                inspectionForm.InspectedBy = string.Empty;
                inspectionForm.PassPoints = 0;
                inspectionForm.FailPoints = 0;
                inspectionForm.NeedImprovementPoints = 0;
                inspectionForm.ScorePercent = (decimal)0.0;
                inspectionForm.SignatureUrl = string.Empty;
                inspectionForm.IsEnable = true;
                inspectionForm.IsDelete = false;
                inspectionForm.ModifiedDate = DateTime.Now;
                foreach (var section in inspectionForm.Sections)
                {

                    section.SectionAutoFail = false;
                    section.SectionAutoFailReason = "{\"Reasons\":[{\"Label\":\"Area does not meet standard\",\"Selected\":true},{\"Label\":\"Other reasons\",\"Selected\":false}]}";
                    section.SectionStatus = 0;
                    section.ScorePercent = 0;
                    section.PassPoints = 0;
                    section.FailPoints = 0;
                    section.NeedImprovementPoints = 0;
                    section.IsEnable = true;
                    section.IsDelete = false;
                    section.ModifiedDate = DateTime.Now;

                    foreach (var item in section.Items)
                    {
                        item.IsDirty = false;
                        item.IsEnable = true;
                        item.IsDelete = false;
                        item.ModifiedDate = DateTime.Now;
                        if (item.FormItemType == FormItemType.FormItemTypeInspection)
                        {
                            dynamic formItemValue = JsonConvert.DeserializeObject<dynamic>(item.FormItemValue);
                            var label = formItemValue.Label;
                            item.FormItemValue = $"{{\"Text\":\"\",\"Label\":\"{label}\",\"Status\":0,\"Rating\":0,\"Items\":[]}}";
                            item.IsRequired = true;
                        }
                        else if (item.FormItemType == FormItemType.FormItemTypePhotoCollection)
                        {
                            item.FormItemValue = "{\"Label\":\"Photos\",\"Items\":[]}";
                        }
                        else if (item.FormItemType == FormItemType.FormItemTypeTextArea)
                        {
                            item.FormItemValue = "{\"Label\":\"Comment\",\"Text\":null}";
                        }
                        AddOrUpdateInpsectionFormItem(item);
                    }
                    AddOrUpdateInspectionFormSection(section);
                }
                inspectionForm = AddOrUpdateInspectionForm(inspectionForm);
            }
            return inspectionForm;
        }

        #endregion
        // ======================================================================================
        #region InspectionService > Contracts
        // ======================================================================================

        ////////////////////////////////////////////////////////////
        /// INSPECTION
        ////////////////////////////////////////////////////////////

        public IList<InspectionFormModel> GetInspectionFormList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
            }
        }

        public ViewInspectionFormListModel GetInspectionFormListByJob(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@JobId", model.JobId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormListByJob, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList =  multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormListByCustomer(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormListByCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormListByRegion(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", model.RegionId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormListByFranchisee(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", model.FranchiseeId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormListByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public InspectionFormModel GetInspectionForm(int inspectionFormId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormId", inspectionFormId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionForm, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var result =  multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").First();

                if (result != null)
                {
                    result.Sections = GetInspectionFormSectionListByForm(result.InspectionFormId);
                }

                return result;
            }
        }

        public InspectionFormModel GetCurrentInspectionFormByCustomer(int customerId)
        {
            var inspectionFormList = GetInspectionFormListByCustomer(new ViewInspectionFormListModel
            {
                CustomerId = customerId,
                IsEnable = true,
                CurrentPage = 1,
                PageSize = 1
            }).InspectionFormList;
            return inspectionFormList.Count > 0 ? inspectionFormList.First() : null;
        }

        public InspectionFormModel GetDetailInspectionForm(int inspectionFormId)
        {
            var result = GetInspectionForm(inspectionFormId);
            if (result != null)
            {
                result.Sections = GetInspectionFormSectionListByForm(result.InspectionFormId);
                foreach (var section in result.Sections)
                {
                    section.Items = GetInspectionFormItemListBySection(section.InspectionFormSectionId);
                }
            }
            return result;
        }

        public InspectionFormModel AddOrUpdateInspectionForm(InspectionFormModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormId", model.InspectionFormId);
            parameters.Add("@JobId", model.JobId);
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@RegionId", model.RegionId);
            parameters.Add("@FranchiseeId", model.FranchiseeId);
            parameters.Add("@ServiceTypeListId", model.ServiceTypeListId);
            parameters.Add("@AccountTypeListId", model.AccountTypeListId);
            parameters.Add("@InspectionStatusId", model.InspectionStatusId);
            parameters.Add("@CallDate", model.CallDate);
            parameters.Add("@RecordedDate", model.RecordedDate);
            parameters.Add("@UploadedDate", model.UploadedDate);
            parameters.Add("@IsCompleted", model.IsCompleted);
            parameters.Add("@InspectedBy", model.InspectedBy);
            parameters.Add("@InspectorId", model.InspectorId);
            parameters.Add("@FormName", model.FormName);
            parameters.Add("@Description", model.Description);
            parameters.Add("@ScorePercent", model.ScorePercent);
            parameters.Add("@PassPoints", model.PassPoints);
            parameters.Add("@FailPoints", model.FailPoints);
            parameters.Add("@NeedImprovementPoints", model.NeedImprovementPoints);
            parameters.Add("@SignatureUrl", model.SignatureUrl);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@IsDelete", model.IsDelete);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_AddOrUpdateInspectionForm, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var result = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").First();

                if (result != null)
                {
                    result.Sections = GetInspectionFormSectionListByForm(result.InspectionFormId);
                    foreach (var section in result.Sections)
                    {
                        section.Items = GetInspectionFormItemListBySection(section.InspectionFormSectionId);
                    }
                }
                return result;
            }
        }

        public InspectionFormModel UpdateInspectionDetailForm(InspectionFormModel model)
        {
            if (model.InspectionFormId == 0)
            {
                throw new Exception($"Form with id={model.InspectionFormId} is not found.");
            }

            foreach (var section in model.Sections)
            {
                UpdateInspectionFormItemList(new ViewInspectionFormItemListModel
                {
                    InspectionFormId = model.InspectionFormId,
                    InspectionFormSectionId = section.InspectionFormSectionId
                });
            }

            return AddOrUpdateInspectionForm(model);
        }

        public InspectionFormModel CompleteInspectionForm(InspectionFormModel model)
        {
            // STEP 1: UPDATE THE RECORDS
            if (model.IsCompleted == false) model.IsCompleted = true;
            if (model.RecordedDate == null) model.RecordedDate = DateTime.Now;
            if (model.UploadedDate == null) model.UploadedDate = DateTime.Now;
            var inspectionForm = AddOrUpdateInspectionForm(model);
         
            if (inspectionForm != null)
            {
                // STEP 2: COPY TO HISTORY
                var inspectionFormHistory = AddInspectionFormHistory(inspectionForm);
                if (inspectionFormHistory != null)
                {
                    foreach (var section in inspectionForm.Sections)
                    {
                        section.InspectionFormId = inspectionFormHistory.InspectionFormId;
                        var sectionHistory = AddInspectionFormSectionHistory(section);
                        if (sectionHistory != null)
                        {
                            foreach (var item in section.Items)
                            {
                                item.InspectionFormSectionId = sectionHistory.InspectionFormSectionId;
                                var itemHistory = AddInpsectionFormItemHistory(item);
                                if (itemHistory != null)
                                {
                                    sectionHistory.Items.Add(itemHistory);
                                }
                            }
                        }
                    }
                }

                // STEP 3: RESET THE CURRENT ONE
                inspectionForm = _resetInspectionForm(inspectionForm.InspectionFormId);
            }

            return inspectionForm;
        }

        public IList<InspectionFormSectionModel> GetInspectionFormSectionListByForm(int inspectionFormId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormId", inspectionFormId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormSectionListByForm, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormSectionModel>().ToList();
            }
        }

        public InspectionFormSectionModel AddOrUpdateInspectionFormSection(InspectionFormSectionModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormSectionId", model.InspectionFormSectionId);
            parameters.Add("@InspectionFormId", model.InspectionFormId);
            parameters.Add("@SectionOrder", model.SectionOrder);
            parameters.Add("@SectionName", model.SectionName);
            parameters.Add("@SectionStatus", model.SectionStatus);
            parameters.Add("@ScorePercent", model.ScorePercent);
            parameters.Add("@PassPoints", model.PassPoints);
            parameters.Add("@FailPoints", model.FailPoints);
            parameters.Add("@NeedImprovementPoints", model.NeedImprovementPoints);
            parameters.Add("@SectionAutoFail", model.SectionAutoFail);
            parameters.Add("@SectionAutoFailReason", model.SectionAutoFailReason);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormSectionModel>(DBConstants.sp_AddOrUpdateInspectionFormSection, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public InspectionFormSectionModel AddInpsectionFormSectionFromTemplateArea(int inspectionFormId, TemplateAreaModel model)
        {
            var sections = GetInspectionFormSectionListByForm(inspectionFormId);
            var order = sections.Count + 1;
            return AddOrUpdateInspectionFormSection(new InspectionFormSectionModel
            {
                InspectionFormSectionId = 0,
                InspectionFormId = inspectionFormId,
                SectionOrder = order,
                SectionName = model.AreaName,
                SectionStatus = 0,
                ScorePercent = 0,
                PassPoints = 0,
                FailPoints = 0,
                NeedImprovementPoints = 0,
                SectionAutoFail = false,
                SectionAutoFailReason = "{\"Reasons\":[{\"Label\":\"Area does not meet standard\",\"Selected\":true},{\"Label\":\"Other reasons\",\"Selected\":false}]}",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });
        }

        public void DeleteInspectionFormSection(int inspectionFormSectionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormSectionId", inspectionFormSectionId);

            FmsDbConn.Execute(DBConstants.sp_DeleteInspectionFormSection, parameters,
                commandType: CommandType.StoredProcedure);
        }

        public IList<InspectionFormItemModel> GetInspectionFormItemListBySection(int inspectionFormSectionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormSectionId", inspectionFormSectionId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormItemBySection, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormItemModel>().ToList();
            }
        }

        public InspectionFormItemModel GetInspectionFormItem(int inspectionFormItemId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormItemId", inspectionFormItemId);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormItemModel>(DBConstants.sp_GetInspectionFormItem, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public InspectionFormItemModel AddOrUpdateInpsectionFormItem(InspectionFormItemModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormItemId", model.InspectionFormItemId);
            parameters.Add("@InspectionFormSectionId", model.InspectionFormSectionId);
            parameters.Add("@FormItemType", model.FormItemType);
            parameters.Add("@FormItemOrder", model.FormItemOrder);
            parameters.Add("@FormItemValue", model.FormItemValue);
            parameters.Add("@IsDirty", model.IsDirty);
            parameters.Add("@IsRequired", model.IsRequired);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormItemModel>(DBConstants.sp_AddOrUpdateInspectionFormItem, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public ViewInspectionFormItemListModel UpdateInspectionFormItemList(ViewInspectionFormItemListModel listModel)
        {
            var resultModel = new ViewInspectionFormItemListModel
            {
                InspectionFormId = listModel.InspectionFormId,
                InspectionFormSectionId = listModel.InspectionFormSectionId
            };

            foreach (var formItem in listModel.InspectionFormItemList)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@InspectionFormItemId", formItem.InspectionFormItemId);
                parameters.Add("@FormItemValue", formItem.FormItemValue);
                parameters.Add("@IsDirty", formItem.IsDirty);
                parameters.Add("@IsRequired", formItem.IsRequired);
                parameters.Add("@ModifiedDate", DateTime.Now);

                using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormItemModel>(DBConstants.sp_AddOrUpdateInspectionFormItem, parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    if (result != null)
                    {
                        resultModel.InspectionFormItemList.Add(result);
                    }
                }
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                if (listModel.InspectionFormSectionId > 0)
                {
                    _updateInspectionFormSectionInfo(listModel.InspectionFormSectionId, listModel.InspectionFormId, 
                        listModel.SectionAutoFail, listModel.SectionAutoFailReason);
                }
            }, null);
            
            return resultModel;
        }

        public void DeleteInspectionFormItem(int inspectionFormItemId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormItemId", inspectionFormItemId);

            FmsDbConn.Execute(DBConstants.sp_DeleteInspectionFormItem, parameters,
                commandType: CommandType.StoredProcedure);
        }

        ////////////////////////////////////////////////////////////
        /// INSPECTION HISTORY
        ////////////////////////////////////////////////////////////

        public IList<InspectionFormModel> GetInspectionFormHistoryList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistoryList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
            }
        }

        public ViewInspectionFormListModel GetInspectionFormHistoryListByJob(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@JobId", model.JobId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistoryListByJob, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormHistoryListByCustomer(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistoryListByCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public InspectionFormHistoryModel GetConsolidatedInpsectionFormHistoryByCustomer(int customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", customerId);
            parameters.Add("@IsEnable", true);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormHistoryModel>(DBConstants.sp_GetConsolidatedInspectionFormHistoryByCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormHistoryListByRegion(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", model.RegionId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistoryListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public ViewInspectionFormListModel GetInspectionFormHistoryListByFranchisee(ViewInspectionFormListModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", model.FranchiseeId);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@PageNo", model.CurrentPage);
            parameters.Add("@PageSize", model.PageSize);
            parameters.Add("@SortColumn", model.SortBy);
            parameters.Add("@SortOrder", model.SortOrder);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistoryListByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var inspectionFormList = multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").ToList();
                if (inspectionFormList != null) model.InspectionFormList = inspectionFormList;
                return model;
            }
        }

        public InspectionFormModel GetInspectionFormHistory(int inspectionFormId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormHistoryId", inspectionFormId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormHistory, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var result =  multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").First();

                if (result != null)
                {
                    result.Sections = GetInspectionFormSectionListByFormHistory(result.InspectionFormId);
                }

                return result;
            }
        }

        public InspectionFormModel GetDetailInspectionFormHistory(int inspectionFormId)
        {
            var result = GetInspectionFormHistory(inspectionFormId);
            if (result != null)
            {
                result.Sections = GetInspectionFormSectionListByFormHistory(result.InspectionFormId);
                foreach (var section in result.Sections)
                {
                    section.Items = GetInspectionFormItemHistoryListBySection(section.InspectionFormSectionId);
                }
            }
            return result;
        }

        public InspectionFormModel AddInspectionFormHistory(InspectionFormModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormHistoryId", 0);
            parameters.Add("@JobId", model.JobId);
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@RegionId", model.RegionId);
            parameters.Add("@FranchiseeId", model.FranchiseeId);
            parameters.Add("@ServiceTypeListId", model.ServiceTypeListId);
            parameters.Add("@AccountTypeListId", model.AccountTypeListId);
            parameters.Add("@InspectionStatusId", model.InspectionStatusId);
            parameters.Add("@CallDate", model.CallDate);
            parameters.Add("@RecordedDate", model.RecordedDate);
            parameters.Add("@UploadedDate", model.UploadedDate);
            parameters.Add("@IsCompleted", model.IsCompleted);
            parameters.Add("@InspectedBy", model.InspectedBy);
            parameters.Add("@InspectorId", model.InspectorId);
            parameters.Add("@FormName", model.FormName);
            parameters.Add("@Description", model.Description);
            parameters.Add("@ScorePercent", model.ScorePercent);
            parameters.Add("@PassPoints", model.PassPoints);
            parameters.Add("@FailPoints", model.FailPoints);
            parameters.Add("@NeedImprovementPoints", model.NeedImprovementPoints);
            parameters.Add("@SignatureUrl", model.SignatureUrl);
            parameters.Add("@IsEnable", model.IsEnable);
            parameters.Add("@IsDelete", model.IsDelete);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", DateTime.Now);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_AddOrUpdateInspectionFormHistory, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormModel, AddressModel, InspectionFormModel>((inspection, address) =>
                {
                    inspection.Address = address;
                    return inspection;
                }, "AddressId").First();
            }
        }

        public IList<InspectionFormSectionModel> GetInspectionFormSectionListByFormHistory(int inspectionFormId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormHistoryId", inspectionFormId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormSectionHistoryListByForm, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormSectionModel>().ToList();
            }
        }

        public InspectionFormSectionModel AddInspectionFormSectionHistory(InspectionFormSectionModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormSectionHistoryId", 0);
            parameters.Add("@InspectionFormHistoryId", model.InspectionFormId);
            parameters.Add("@SectionOrder", model.SectionOrder);
            parameters.Add("@SectionName", model.SectionName);
            parameters.Add("@SectionStatus", model.SectionStatus);
            parameters.Add("@ScorePercent", model.ScorePercent);
            parameters.Add("@PassPoints", model.PassPoints);
            parameters.Add("@FailPoints", model.FailPoints);
            parameters.Add("@NeedImprovementPoints", model.NeedImprovementPoints);
            parameters.Add("@SectionAutoFail", model.SectionAutoFail);
            parameters.Add("@SectionAutoFailReason", model.SectionAutoFailReason);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", DateTime.Now);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormSectionModel>(DBConstants.sp_AddOrUpdateInspectionFormSectionHistory, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public IList<InspectionFormItemModel> GetInspectionFormItemHistoryListBySection(int inspectionFormSectionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormSectionHistoryId", inspectionFormSectionId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetInspectionFormItemHistoryBySection, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<InspectionFormItemModel>().ToList();
            }
        }

        public InspectionFormItemModel GetInspectionFormItemHistory(int inspectionFormItemId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormItemHistoryId", inspectionFormItemId);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormItemModel>(DBConstants.sp_GetInspectionFormItemHistory, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public InspectionFormItemModel AddInpsectionFormItemHistory(InspectionFormItemModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InspectionFormItemHistoryId", 0);
            parameters.Add("@InspectionFormSectionHistoryId", model.InspectionFormSectionId);
            parameters.Add("@FormItemType", model.FormItemType);
            parameters.Add("@FormItemOrder", model.FormItemOrder);
            parameters.Add("@FormItemValue", model.FormItemValue);
            parameters.Add("@IsDirty", model.IsDirty);
            parameters.Add("@IsRequired", model.IsRequired);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", DateTime.Now);

            using (var result = FmsDbConn.QuerySingleOrDefault<InspectionFormItemModel>(DBConstants.sp_AddOrUpdateInspectionFormItemHistory, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        #endregion
        // ======================================================================================

        #endregion

        //public List<JKApi.Data.DAL.Inspection> getInspections()
        //{
        //    var response = Uow.InspectionRepository.GetAll().ToList();
        //    return response;
        //}
    }
}