using System.Linq;
using JKApi.Data.DAL;
using System.Collections.Generic;
using JKViewModels;
using JKViewModels.Inspection;

namespace JKApi.Service.ServiceContract.Inspection
{
    public interface IInspectionService
    {
        #region IInspectionService > GetAll

        //IQueryable<Data.DAL.Inspection> GetAll_Inspection();
        IQueryable<InspectionForm> GetAll_InspectionForm();
        IQueryable<InspectionFormItem> GetAll_InspectionFormItem();
        IQueryable<InspectionStatu> GetAll_InspectionStatus();

        #endregion

        #region IInspectionService > Get

        //Data.DAL.Inspection Get_Inspection(int id);
        InspectionForm Get_InspectionForm(int id);
        InspectionFormItem Get_InspectionFormItem(int id);
        InspectionStatu Get_InspectionStatus(int id);
        //List<JKApi.Data.DAL.Inspection> getInspections();

        #endregion

        #region IInspectionService > AddOrUpdate

        //Data.DAL.Inspection SaveOrUpdate_Inspection(Data.DAL.Inspection inspection);
        InspectionForm SaveOrUpdate_InspectionForm(InspectionForm inspectionForm);
        InspectionFormItem SaveOrUpdate_InspectionFormItem(InspectionFormItem inspectionFormItem);
        InspectionStatu SaveOrUpdate_InspectionStatus(InspectionStatu inspectionStatus);

        #endregion

        #region IInspectionService > Delete

        //void Delete_Inspection(int id);
        void Delete_InspectionForm(int id);
        void Delete_InspectionFormItem(int id);
        void Delete_InspectionStatus(int id);

        #endregion

        #region Template List 
        List<FromTemplateAPIModel> GetFormTemplateList(int FormTemplateId);

        List<AccountTypeListAPIModel> AccountTypeList();
        #endregion

        #region Inspection Stored Procedures

        IList<InspectionFormModel> GetInspectionFormList();
        ViewInspectionFormListModel GetInspectionFormListByJob(ViewInspectionFormListModel model);
        ViewInspectionFormListModel GetInspectionFormListByCustomer(ViewInspectionFormListModel model);
        ViewInspectionFormListModel GetInspectionFormListByRegion(ViewInspectionFormListModel model);
        ViewInspectionFormListModel GetInspectionFormListByFranchisee(ViewInspectionFormListModel model);
        InspectionFormModel GetInspectionForm(int inspectionFormId);
        InspectionFormModel GetCurrentInspectionFormByCustomer(int customerId);
        InspectionFormModel GetDetailInspectionForm(int inspectionFormId);
        InspectionFormModel AddOrUpdateInspectionForm(InspectionFormModel model);
        InspectionFormModel UpdateInspectionDetailForm(InspectionFormModel model);
        InspectionFormModel CompleteInspectionForm(InspectionFormModel model);

        IList<InspectionFormSectionModel> GetInspectionFormSectionListByForm(int inspectionFormId);
        InspectionFormSectionModel AddOrUpdateInspectionFormSection(InspectionFormSectionModel model);
        InspectionFormSectionModel AddInpsectionFormSectionFromTemplateArea(int inspectionFormId, TemplateAreaModel model);
        void DeleteInspectionFormSection(int inspectionFormSectionId);

        IList<InspectionFormItemModel> GetInspectionFormItemListBySection(int inspectionFormSectionId);
        InspectionFormItemModel GetInspectionFormItem(int inspectionFormItemId);
        InspectionFormItemModel AddOrUpdateInpsectionFormItem(InspectionFormItemModel model);
        ViewInspectionFormItemListModel UpdateInspectionFormItemList(ViewInspectionFormItemListModel listModel);
        void DeleteInspectionFormItem(int inspectionFormItemId);

        IList<InspectionFormModel> GetInspectionFormHistoryList();
        ViewInspectionFormListModel GetInspectionFormHistoryListByJob(ViewInspectionFormListModel model);
        ViewInspectionFormListModel GetInspectionFormHistoryListByCustomer(ViewInspectionFormListModel model);
        InspectionFormHistoryModel GetConsolidatedInpsectionFormHistoryByCustomer(int customerId);
        ViewInspectionFormListModel GetInspectionFormHistoryListByRegion(ViewInspectionFormListModel model);
        ViewInspectionFormListModel GetInspectionFormHistoryListByFranchisee(ViewInspectionFormListModel model);
        InspectionFormModel GetInspectionFormHistory(int inspectionFormId);
        InspectionFormModel AddInspectionFormHistory(InspectionFormModel model);

        IList<InspectionFormSectionModel> GetInspectionFormSectionListByFormHistory(int inspectionFormId);
        InspectionFormSectionModel AddInspectionFormSectionHistory(InspectionFormSectionModel model);

        IList<InspectionFormItemModel> GetInspectionFormItemHistoryListBySection(int inspectionFormSectionId);
        InspectionFormItemModel GetInspectionFormItemHistory(int inspectionFormItemId);
        InspectionFormItemModel AddInpsectionFormItemHistory(InspectionFormItemModel model);
        InspectionFormModel GetDetailInspectionFormHistory(int inspectionFormId);

        #endregion

    }
}
