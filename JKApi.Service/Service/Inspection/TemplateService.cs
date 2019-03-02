using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using JKApi.Service.ServiceContract.Inspection;
using JKViewModels;
using Dapper;
using JKApi.Data.DTOObject;
using JKApi.Data.DAL;
using System.Linq;
using JKViewModels.Common;
using JKViewModels.Inspection;
using JKViewModels.Job;

namespace JKApi.Service.Service.Inspection
{
    public interface ITemplateService
    {
        FormTemplateViewModel GetTemplates(FormTemplateViewModel viewModel);
        FormTemplateDetailModel GetTemplate(int formTemplateId);
        FormTemplateDetailModel GetTemplateByAccountType(int accountTypeListId);
        FormTemplateModel AddOrUpdateFormTemplate(FormTemplateModel model);
        void DeleteFormTemplate(int formTemplateId);
        List<TemplateAreaModel> GetTemplateAreaList();
        TemplateAreaModel GetTemplateArea(int templateAreaId);
        TemplateAreaModel AddOrupdateTemplateAreaModel(int formTemplateId, int templateAreaId);
        void DeleteTemplateAreaFromTemplate(int formTemplateId, TemplateAreaModel model);
        IList<TemplateAreaItemModel> GetTemplateAreaItemList();
        TemplateAreaModel GetTemplateAreaItemByArea(int templateAreaId); 
        IList<TemplateAreaItemModel> GetTemplateAreaItemListByArea(int templateAreaId);
        TemplateAreaItemModel GetTemplateAreaItem(int templateAreaItemId);
        IList<TemplateAreaItemModel> AddOrUpdateTemplateAreaItemToArea(TemplateAreaItemModel model);
        void DeleteTemplateAreaItem(int templateAreaItemId);
        void DeleteTemplateAreaItemFromArea(TemplateAreaItemModel model);
        InspectionFormModel AssignInspectionFromTemplate(FormTemplateDetailModel templateForm, JobModel job);
        List<sp_GetTemplateAreaItemListByArea_Result> GetItemList(int templateAreaId);
    }

    public class TemplateService : BaseService, ITemplateService
    {
        private readonly IInspectionService _inspectionService;

        public TemplateService(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        public FormTemplateViewModel GetTemplates(FormTemplateViewModel viewModel)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@IsEnable", viewModel.IsEnable),
                new SqlParameter("@PageNo", viewModel.CurrentPage),
                new SqlParameter("@PageSize", viewModel.PageSize),
                new SqlParameter("@SortColumn", viewModel.SortBy),
                new SqlParameter("@SortOrder", viewModel.SortOrder),
                new SqlParameter("@SearchText", viewModel.SearchText)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplates, parameters))
            {
                viewModel.FormTemplateList = new List<FormTemplateModel>();
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return viewModel;
                foreach (DataRow dataRow in dataset.Tables[0].Rows)
                {
                    var model = GetDataRowToEntity<FormTemplateModel>(dataRow);
                    if (model != null)
                    {
                        viewModel.FormTemplateList.Add(model);
                    }
                }
            }

            return viewModel;
        }

        public FormTemplateDetailModel GetTemplate(int formTemplateId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@FormTemplateId", formTemplateId),
                new SqlParameter("@IsEnable", true),
            };

            FormTemplateDetailModel template;

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplate, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0) return null;
                template = GetDataRowToEntity<FormTemplateDetailModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
                if (template != null)
                {
                    foreach (DataRow dataRow1 in dataset.Tables[1].Rows)
                    {
                        var area = GetDataRowToEntity<TemplateAreaModel>(dataRow1);
                        if (area != null && area.IsEnable)
                        {
                            foreach (DataRow dataRow2 in dataset.Tables[2].Rows)
                            {
                                var item = GetDataRowToEntity<TemplateAreaItemModel>(dataRow2);
                                if (item != null && item.TemplateAreaId == area.TemplateAreaId && item.IsEnable)
                                {
                                    area.Items.Add(item);
                                }
                            }
                            template.Areas.Add(area);
                        }
                    }
                }
            }

            return template;
        }

        public FormTemplateDetailModel GetTemplateByAccountType(int accountTypeListId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@AccountTypeListId", accountTypeListId),
                new SqlParameter("@IsEnable", true),
            };

            FormTemplateDetailModel template;

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateByAccountType, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0) return null;
                template = GetDataRowToEntity<FormTemplateDetailModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
                if (template != null)
                {
                    foreach (DataRow dataRow1 in dataset.Tables[1].Rows)
                    {
                        var area = GetDataRowToEntity<TemplateAreaModel>(dataRow1);
                        if (area != null && area.IsEnable)
                        {
                            foreach (DataRow dataRow2 in dataset.Tables[2].Rows)
                            {
                                var item = GetDataRowToEntity<TemplateAreaItemModel>(dataRow2);
                                if (item != null && item.TemplateAreaId == area.TemplateAreaId && item.IsEnable)
                                {
                                    area.Items.Add(item);
                                }
                            }
                            template.Areas.Add(area);
                        }
                    }
                }
            }

            return template;
        }

        public FormTemplateModel AddOrUpdateFormTemplate(FormTemplateModel model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@FormTemplateId", model.FormTemplateId),
                new SqlParameter("@AccountTypeListId", model.AccountTypeListId),
                new SqlParameter("@ServiceTypeListId", 1),
                new SqlParameter("@FormTemplateTypeId", model.FormTemplateTypeId),
                new SqlParameter("@FormName", model.FormName),
                new SqlParameter("@Description", model.Description),
                new SqlParameter("@IsEnable", true),
                new SqlParameter("@CreatedDate", model.CreatedOn),
                new SqlParameter("@ModifiedDate", model.ModifiedOn)
            };

            FormTemplateModel template;

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                CommandType.StoredProcedure, DBConstants.sp_AddOrUpdateFormTemplate, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0) return null;
                template = GetDataRowToEntity<FormTemplateModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
            }

            return template;
        }

        public void DeleteFormTemplate(int formTemplateId)
        {
            var template = GetTemplate(formTemplateId);

            foreach (var area in template.Areas)
            {
                DeleteTemplateAreaFromTemplate(formTemplateId, area);
            }

            SqlParameter[] parameters =
            {
                new SqlParameter("@FormTemplateId", formTemplateId)
            };

            SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                DBConstants.sp_DeleteTemplate, parameters);
        }

        public List<TemplateAreaModel> GetTemplateAreaList()
        {
            var models = new List<TemplateAreaModel>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@IsEnable", true)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateAreaList, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return models;
                foreach (DataRow dataRow in dataset.Tables[0].Rows)
                {
                    var model = GetDataRowToEntity<TemplateAreaModel>(dataRow);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }
            }

            return models;
        }

        public TemplateAreaModel GetTemplateArea(int templateAreaId)
        {
            TemplateAreaModel area;

            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaId", templateAreaId),
                new SqlParameter("@IsEnable", true)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateArea, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0) return null;
                area = GetDataRowToEntity<TemplateAreaModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
            }

            return area;
        }

        public TemplateAreaModel AddOrupdateTemplateAreaModel(int formTemplateId, int templateAreaId)
        {
            TemplateAreaModel result = null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@FormTemplateId", formTemplateId),
                new SqlParameter("@TemplateAreaId", templateAreaId),
                new SqlParameter("@IsEnable", true),
                new SqlParameter("@CreatedBy",LoginUserId),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@ModifiedDate", DateTime.Now),
                new SqlParameter("@ModifiedBy",-1)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_AddOrUpdateTemplateAreaToForm, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return null;
                result = GetDataRowToEntity<TemplateAreaModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
            }

            return result;
        }

        public void DeleteTemplateAreaFromTemplate(int formTemplateId, TemplateAreaModel model)
        {
            foreach (var item in model.Items)
            {
                DeleteTemplateAreaItemFromArea(item);
            }

            SqlParameter[] parameters =
            {
                new SqlParameter("@FormTemplateId", formTemplateId),
                new SqlParameter("@TemplateAreaId", model.TemplateAreaId)
            };

            SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                DBConstants.sp_DeleteTemplateAreaFromTemplate, parameters);
        }

        public IList<TemplateAreaItemModel> GetTemplateAreaItemList()
        {
            var models = new List<TemplateAreaItemModel>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@IsEnable", true)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateAreaItemList, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return models;
                foreach (DataRow dataRow in dataset.Tables[0].Rows)
                {
                    var model = GetDataRowToEntity<TemplateAreaItemModel>(dataRow);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }
            }

            return models;
        }

        public TemplateAreaModel GetTemplateAreaItemByArea(int templateAreaId)
        {
            var area = GetTemplateArea(templateAreaId);

            if (area != null)
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@TemplateAreaId", templateAreaId),
                    new SqlParameter("@IsEnable", true)
                };

                using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                        CommandType.StoredProcedure, DBConstants.sp_GetTemplateAreaItemByArea, parameters))
                {
                    area.Items = new List<TemplateAreaItemModel>();
                    if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return area;
                    foreach (DataRow dataRow in dataset.Tables[0].Rows)
                    {
                        var item = GetDataRowToEntity<TemplateAreaItemModel>(dataRow);
                        if (item != null)
                        {
                            item.TemplateAreaId = templateAreaId;
                            area.Items.Add(item);
                        }
                    }
                }
            }

            return area;
        }

        public IList<TemplateAreaItemModel> GetTemplateAreaItemListByArea(int templateAreaId)
        {
            IList<TemplateAreaItemModel> items = new List<TemplateAreaItemModel>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaId", templateAreaId),
                new SqlParameter("@IsEnable", true)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateAreaItemByArea, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return items;
                foreach (DataRow dataRow in dataset.Tables[0].Rows)
                {
                    var item = GetDataRowToEntity<TemplateAreaItemModel>(dataRow);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public TemplateAreaItemModel GetTemplateAreaItem(int templateAreaItemId)
        {
            TemplateAreaItemModel item;

            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaItemId", templateAreaItemId),
                new SqlParameter("@IsEnable", true)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_GetTemplateAreaItem, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0) return null;
                item = GetDataRowToEntity<TemplateAreaItemModel>(dataset.Tables[0].Rows.Count == 1 ? dataset.Tables[0].Rows[0] : null);
            }

            return item;
        }

        public IList<TemplateAreaItemModel> AddOrUpdateTemplateAreaItemToArea(TemplateAreaItemModel model)
        {
            IList<TemplateAreaItemModel> items = new List<TemplateAreaItemModel>();

            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaId", model.TemplateAreaId),
                new SqlParameter("@TemplateAreaItemId", model.TemplateAreaItemId),
                new SqlParameter("@IsEnable", true),
                new SqlParameter("@CreatedBy",LoginUserId),
                 new SqlParameter("@CreatedDate",DateTime.Now),
                  new SqlParameter("@ModifiedBy",-1),
                   new SqlParameter("@ModifiedDate",DateTime.Now)
            };

            using (var dataset = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction,
                    CommandType.StoredProcedure, DBConstants.sp_AddOrUpdateTemplateAreaItemToArea, parameters))
            {
                if (dataset == null || dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0) return items;
                foreach (DataRow dataRow in dataset.Tables[0].Rows)
                {
                    var item = GetDataRowToEntity<TemplateAreaItemModel>(dataRow);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public InspectionFormModel AssignInspectionFromTemplate(FormTemplateDetailModel templateForm, JobModel job)
        {
            InspectionFormModel inspectionForm = null;

            if (templateForm != null && job != null)
            {
                inspectionForm = _inspectionService.AddOrUpdateInspectionForm(new InspectionFormModel
                {
                    JobId = job.JobId,
                    CustomerId = job.CustomerId,
                    RegionId = job.RegionId,
                    FranchiseeId = job.FranchiseeId,
                    ServiceTypeListId = job.ServiceTypeListId,
                    AccountTypeListId = templateForm.AccountTypeListId,
                    InspectionStatusId = InspectionStatus.Unknown,
                    IsCompleted = false,
                    FormName = templateForm.FormName,
                    Description = templateForm.Description,
                    ScorePercent = 0,
                    PassPoints = 0,
                    FailPoints = 0,
                    NeedImprovementPoints = 0,
                    SignatureUrl = null,
                    IsEnable = true,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                });

                if (inspectionForm != null)
                {
                    for (var i = 0; i < templateForm.Areas.Count; i++)
                    {
                        var area = templateForm.Areas[i];
                        var section = _inspectionService.AddOrUpdateInspectionFormSection(new InspectionFormSectionModel
                        {
                            InspectionFormId = inspectionForm.InspectionFormId,
                            SectionOrder = i + 1,
                            SectionName = area.AreaName,
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
                        if (section != null)
                        {
                            for (var j = 0; j < area.Items.Count; j++)
                            {
                                var areaItem = area.Items[j];
                                var item = _inspectionService.AddOrUpdateInpsectionFormItem(new InspectionFormItemModel
                                {
                                    InspectionFormSectionId = section.InspectionFormSectionId,
                                    FormItemType = areaItem.FormItemType,
                                    FormItemOrder = j + 1,
                                    FormItemValue = areaItem.FormItemValue,
                                    IsDirty = false,
                                    IsRequired = true,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now
                                });
                                if (item != null)
                                {
                                    section.Items.Add(item);
                                }
                            }
                            inspectionForm.Sections.Add(section);
                        }
                    }
                }
            }

            return inspectionForm;
        }

        public void DeleteTemplateAreaItem(int templateAreaItemId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaItemId", templateAreaItemId)
            };

            SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                DBConstants.sp_DeleteTemplateAreaItem, parameters);
        }

        public void DeleteTemplateAreaItemFromArea(TemplateAreaItemModel model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateAreaId", model.TemplateAreaId),
                new SqlParameter("@TemplateAreaItemId", model.TemplateAreaItemId)
            };

            SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure,
                DBConstants.sp_DeleteTemplateAreaItemFromArea, parameters);
        }

        public List<sp_GetTemplateAreaItemListByArea_Result> GetItemList(int templateAreaId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<sp_GetTemplateAreaItemListByArea_Result> lstAreaItem = context.sp_GetTemplateAreaItemListByArea(templateAreaId,true,"","asc").ToList();
                return lstAreaItem;
            }
        }
    }
}
