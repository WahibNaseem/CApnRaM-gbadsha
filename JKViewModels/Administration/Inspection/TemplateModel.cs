using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public enum FormItemType
    {
        FormItemTypeUnknown = 0,
        FormItemTypeCheckList = 9,
        FormItemTypeCheckIn = 10,
        FormItemTypeLabel = 21,
        FormItemTypeOptions = 25,
        FormItemTypePersonName = 27,
        FormItemTypePhotoCollection = 29,
        FormItemTypePhotoComment = 30,
        FormItemTypeTextArea = 38,
        FormItemTypeInspection = 40
    }

    public class FormTemplateViewModel : PaggingModel
    {
        public string SearchText { get; set; }
        public int ActiveStatus { get; set; }
        public List<FormTemplateModel> FormTemplateList { get; set; }

        public FormTemplateViewModel()
        {
            FormTemplateList = new List<FormTemplateModel>();
        }
    }

    public class FormTemplateModel : BaseModel
    {
        public int FormTemplateId { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public int FormTemplateTypeId { get; set; }
        public string AccountTypeListName { get; set; }
        public string ServiceTypeListName { get; set; }
        public string FormTemplateName { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
    }

    public class FormTemplateDetailModel : FormTemplateModel
    {
        public IList<TemplateAreaModel> Areas { get; set; }

        public FormTemplateDetailModel()
        {
            Areas = new List<TemplateAreaModel>();
        }
    }

    public class TemplateAreaModel : BaseEntityModel
    {
        public int TemplateAreaId { get; set; }
        public string AreaName { get; set; }
        public string ItemName { get; set; }
        public string ItemIds { get; set; }
        public IList<TemplateAreaItemModel> Items { get; set; }

        public TemplateAreaModel()
        {
            Items = new List<TemplateAreaItemModel>();
        }
    }

    public class TemplateAreaItemModel : BaseModel
    {
        public int TemplateAreaId { get; set; }
        public int TemplateAreaItemId { get; set; }
        public string ItemName { get; set; }
        public FormItemType FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public bool IsDirty { get; set; }
        public bool IsRequired { get; set; }
    }

    #region Old Codes

    public class TemplateViewModel
    {
        public TemplateViewModel()
        {
            lstFormType = new List<FormType>();
            lstTemplateType = new List<TemplateType>();
            lstTemplateDetailListViewModel = new List<TemplateDetailListViewModel>();
            itemTemplateDetailListViewModel = new TemplateDetailListViewModel();
        }
        public int TemplateId { get; set; }
        public int TemplateTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string JobIconUrl { get; set; }
        public string TemplateTypeName { get; set; }
        public List<FormType> lstFormType { get; set; }
        public List<TemplateType> lstTemplateType { get; set; }
        public List<TemplateDetailListViewModel> lstTemplateDetailListViewModel { get; set; }
        public TemplateDetailListViewModel itemTemplateDetailListViewModel { get; set; }
        public int PageStepNo { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public string AccountTypeName { get; set; }
        public string ServiceTypeName { get; set; }
    }

    public class TemplateListViewModel
    {
        public int TemplateId { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public int TemplateTypeId { get; set; }
        public string AccountTypeName { get; set; }
        public string ServiceTypeName { get; set; }
        public string TemplateTypeName { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public int HeaderCount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class TemplateDetailModel : TemplateListViewModel
    {
        
    }

    public class TemplateDetailListViewModel
    {
        public int TemplateDetailId { get; set; }
        public int TemplateId { get; set; }
        public string HeaderName { get; set; }
        public int? HeaderOrder { get; set; }
        public string TemplateName { get; set; }
        public int HeaderFormItemCount { get; set; }
        public int FormTypeId { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
        public string LableName { get; set; }
        public string FormTypeName { get; set; }
    }

    public class ControlView
    {
        public string FormType { get; set; }
        public string FormValue { get; set; }

        public string ControlId { get; set; }
    }

    public class FormType
    {
        public FormType()
        {

        }

        public int FormTypeId { get; set; }

        public int? Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MasterValue { get; set; }

    }

    public partial class TemplateType : BaseModel
    {
        public TemplateType()
        {

        }
        public int TemplateTypeId { get; set; }
        public int? Type { get; set; }
        public string Name { get; set; }

    }

    public class FormCustomItemTemplateModel : BaseModel
    {
        public int FormCustomItemTemplateId { get; set; }

        public int? FormItemTypeId { get; set; }

        public int? FormTemplateTypeId { get; set; }

        public string Value { get; set; }

        public int ItemOrder { get; set; }

        public string SectionName { get; set; }

        public int SectionOrder { get; set; }

        public string ActionType { get; set; }

    }

    public class FormItemTemplateModel
    {
        public int FormItemTemplateId { get; set; }

        public int FormTemplateId { get; set; }

        public int? FormItemTypeId { get; set; }

        public int? FormCustomItemTemplateId { get; set; }

        public string FormValue { get; set; }

        public int? ItemOrder { get; set; }

        public int? SectionOrder { get; set; }

        public string SectionName { get; set; }

    }

    public class InspectionSaveModel : BaseModel
    {
        public int InspectionId { get; set; }

        public int? ContractDetailId { get; set; }

        public int? InspectorId { get; set; }

        public int InspectionStatusId { get; set; }

        public DateTime? CallDate { get; set; }

        public DateTime? RecordedDate { get; set; }

        public DateTime? UploadedDate { get; set; }

        public int? CustomerRating { get; set; }

        public decimal? ScorePercent { get; set; }

        public string Action { get; set; }

        public string CustomerEvaluation { get; set; }

        public int FormTemplateId { get; set; }
    }

    public class InspectionSaveFormModel : BaseModel
    {
        public InspectionSaveFormModel()
        {
            lstInspectionSaveFormItem = new List<InspectionSaveFormItem>();
        }
        public int InspectionFormId { get; set; }

        public int? InspectionId { get; set; }

        public int ServiceTypeListId { get; set; }

        public string FormName { get; set; }

        public string Description { get; set; }

        public List<InspectionSaveFormItem> lstInspectionSaveFormItem { get; set; }

    }

    public class InspectionSaveFormItem : BaseModel
    {
        public int InspectionFormItemId { get; set; }

        public int? InspectionFormId { get; set; }

        public int FormItemTypeId { get; set; }

        public string FormValue { get; set; }

        public int? ItemOrder { get; set; }

        public int? SectionOrder { get; set; }

        public string SectionName { get; set; }

    }

    public class TemplateAreaViewModel : PaggingModel
    {
        public TemplateAreaViewModel()
        {
            templateAreaList = new List<TemplateAreaModel>();
        }

        public List<TemplateAreaModel> templateAreaList { get; set; }

        public int TemplateAreaId { get; set; }

        public string AreaName { get; set; }

        public string ItemName { get; set; }

        public string ItemIds { get; set; }

        public string ActionType { get; set; }
    }

    public class TemplateItemModel : BaseModel
    {
        public int TemplateAreaItemId { get; set; }
        public string ItemName { get; set; }
        public string ActionType { get; set; }
        public int FormItemType { get; set; }
    }

    public class TemplateItemViewModel : PaggingModel
    {
        public TemplateItemViewModel()
        {
            templateItemList = new List<TemplateItemModel>();
        }

        public List<TemplateItemModel> templateItemList { get; set; }

        public int TemplateAreaItemId { get; set; }

        public string ItemName { get; set; }

        public string ActionType { get; set; }

        public int FormItemType { get; set; }
    }

    public class NewFormTemplateModel : BaseModel
    {
        public int FormTemplateId { get; set; }

        public int AccountTypeListId { get; set; }

        public int ServiceTypeListId { get; set; }

        public int FormTemplateTypeId { get; set; }

        public string FormName { get; set; }

        public string Description { get; set; }

        public string AreaName { get; set; }

        public string AreaIds { get; set; }

        public string AccountTypeName { get; set; }

        public string ServiceTypeName { get; set; }

        public string TemplateTypeName { get; set; }

        public string Question { get; set; }

        public string QuestionIds { get; set; }

    }

    public class NewFormTemplateViewModel : PaggingModel
    {
        public NewFormTemplateViewModel()
        {
            listFormTemplateModel = new List<NewFormTemplateModel>();
        }

        public List<NewFormTemplateModel> listFormTemplateModel { get; set; }


        public string ActionType { get; set; }

        public int FormTemplateId { get; set; }

        public int AccountTypeListId { get; set; }

        public int ServiceTypeListId { get; set; }

        public int FormTemplateTypeId { get; set; }

        public string FormName { get; set; }

        public string Description { get; set; }

        public string AreaName { get; set; }

        public string AreaIds { get; set; }

        public string AccountTypeName { get; set; }

        public string ServiceTypeName { get; set; }

        public string TemplateTypeName { get; set; }

        public string Question { get; set; }

        public string QuestionIds { get; set; }
    }


    #region Template API 
    public class FromTemplateAPIModel
    {
        public FromTemplateAPIModel()
        {
            ListFromTemplateArea = new List<FromTemplateAreaAPIModel>();
        }
        public int FormTemplateId { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public int FormTemplateTypeId { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public string AccountTypeList { get; set; }
        public string ServiceTypeList { get; set; }
        public string FormTemplateType { get; set; }

        public List<FromTemplateAreaAPIModel> ListFromTemplateArea { get; set; }
    }

    public class FromTemplateAreaAPIModel
    {
        public FromTemplateAreaAPIModel()
        {
            ListFromTemplateItem = new List<FromTemplateItemAPIModel>();
        }

        public int FormTemplateAreaMappingId { get; set; }
        public int FormTemplateId { get; set; }
        public int TemplateAreaId { get; set; }
        public string AreaName { get; set; }

        public List<FromTemplateItemAPIModel> ListFromTemplateItem { get; set; }

    }

    public class FromTemplateItemAPIModel
    {
        public int TemplateAreaId { get; set; }
        public int TemplateAreaItemId { get; set; }
        public string ItemName { get; set; }
        public int FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public bool IsDirty { get; set; }
        public bool IsRequired { get; set; }

    }

    public class AccountTypeListAPIModel
    {
        public int AccountTypeListId { get; set; }

        public string Name { get; set; }


    }



    public class TemplateQuestionViewModel : PaggingModel
    {
        public TemplateQuestionViewModel()
        {
            TemplateQuestionList = new List<TemplateQuestionModel>();
        }

        public List<TemplateQuestionModel> TemplateQuestionList { get; set; }

        public int TemplateQuestionId { get; set; }

        public string Question { get; set; }

        public string QuestionType { get; set; }

        public string ActionType { get; set; }
    }

    public class TemplateQuestionModel : BaseModel
    {
        public int TemplateQuestionId { get; set; }

        public string Question { get; set; }

        public string QuestionType { get; set; }


        public string ActionType { get; set; }
    }
    #endregion


    #region Inspection Result 
    public class InspectionResultModel : BaseModel
    {
        public InspectionResultModel()
        {
            ListFromTemplateArea = new List<FromTemplateAreaAPIModel>();
        }

        public int InspectionResultId { get; set; }
        public int InspectionId { get; set; }

        public int AccountTypeListId { get; set; }

        public string AccountType { get; set; }

        public string ContactName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string AcctType { get; set; }

        public string Frequency { get; set; }

        public string FranchiseeName { get; set; }

        public int InspectorId { get; set; }

        public string InspectorName { get; set; }

        public List<FromTemplateAreaAPIModel> ListFromTemplateArea { get; set; }

        public byte[] customerSignature { get; set; }
    }
    #endregion

    #endregion

}
