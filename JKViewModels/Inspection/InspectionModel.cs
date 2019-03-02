using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JKViewModels.Common;

namespace JKViewModels.Inspection
{
    public class InspectionModel : BaseEntityModel
    {
        public int InspectionId { get; set; }
        public int RegionId { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        public int InspectorId { get; set; }
        public InspectionStatus InspectionStatus { get; set; }
        public string InspectedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CallDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? RecordedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UploadedDate { get; set; }
        public double ScorePercent { get; set; }
        public double PassPoints { get; set; }
        public double FailPoints { get; set; }
        public double NeedImprovementPoints { get; set; }
        public string Action { get; set; }
        public string CustomerEvaluation { get; set; }
        public int CustomerRating { get; set; }
        public bool IsCompleted { get; set; }
        public int AddressId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string AccountType { get; set; }
        public string ServiceType { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public AddressModel Address { get; set; }
    }

    public class InspectionFormModel : BaseEntityModel
    {
        public int InspectionFormId { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public int AddressId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountType { get; set; }
        public InspectionStatus InspectionStatusId { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CallDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? RecordedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? UploadedDate { get; set; }
        public bool IsCompleted { get; set; }
        public string InspectedBy { get; set; }
        public int InspectorId { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public decimal PassPoints { get; set; }
        public decimal FailPoints { get; set; }
        public decimal NeedImprovementPoints { get; set; }
        public decimal ScorePercent { get; set; }
        public string SignatureUrl { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public AddressModel Address { get; set; }
        public IList<InspectionFormSectionModel> Sections { get; set; }

        public InspectionFormModel()
        {
            Sections = new List<InspectionFormSectionModel>();
        }
    }

    public class InspectionFormSectionModel : BaseEntityModel
    {
        public int InspectionFormSectionId { get; set; }
        public int InspectionFormId { get; set; }
        public int SectionOrder { get; set; }
        public string SectionName { get; set; }
        public int SectionStatus { get; set; }
        public decimal NeedImprovementPoints { get; set; }
        public decimal ScorePercent { get; set; }
        public decimal PassPoints { get; set; }
        public decimal FailPoints { get; set; }
        public bool SectionAutoFail { get; set; }
        public string SectionAutoFailReason { get; set; }
        public IList<InspectionFormItemModel> Items { get; set; }

        public InspectionFormSectionModel()
        {
            Items = new List<InspectionFormItemModel>();
        }
    }

    public class InspectionFormItemModel : BaseEntityModel
    {
        public int InspectionFormItemId { get; set; }
        public int InspectionFormSectionId { get; set; }
        public int FormItemOrder { get; set; }
        public FormItemType FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public bool IsDirty { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ViewInspectionListModel : PaggingModel
    {
        public string SearchText { get; set; }
        public int ActiveStatus { get; set; }
        public List<InspectionModel> InspectionList { get; set; }

        public ViewInspectionListModel()
        {
            InspectionList = new List<InspectionModel>();
        }
    }

    public class ListModel : BaseModel
    {
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }

    public class ViewInspectionFormListModel : PaggingModel
    {
        public List<InspectionFormModel> InspectionFormList { get; set; }

        public ViewInspectionFormListModel()
        {
            InspectionFormList = new List<InspectionFormModel>();
        }
    }

    public class ViewInspectionFormItemListModel : ListModel
    {
        public int InspectionFormId { get; set; }
        public int InspectionFormSectionId { get; set; }
        public bool SectionAutoFail { get; set; }
        public string SectionAutoFailReason { get; set; }
        public List<InspectionFormItemModel> InspectionFormItemList { get; set; }

        public ViewInspectionFormItemListModel()
        {
            InspectionFormItemList = new List<InspectionFormItemModel>();
        }
    }

    public class InspectionFormDataModel : BaseModel
    {
        public int InspectionFormId { get; set; }
        public int InspectionId { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountType { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public decimal? PassPoints { get; set; }
        public decimal? FailPoints { get; set; }
        public decimal? NeedImprovementPoints { get; set; }
        public decimal? ScorePercent { get; set; }
        public string SignatureUrl { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public string FranchiseeName { get; set; }
        public IList<InspectionFormSectionModel> Sections { get; set; }

        public InspectionFormDataModel()
        {
            Sections = new List<InspectionFormSectionModel>();
        }
    }

    public class InspectionFormHistoryModel : BaseEntityModel
    {
        public int RegionId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public int TotalInspections { get; set; }
        public decimal TotalPassPoints { get; set; }
        public decimal TotalFailPoints { get; set; }
        public decimal TotalNeedImprovementPoints { get; set; }
        public decimal AverageScorePercent { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? LastInspectionDate { get; set; }
        public string LastInspectedBy { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
    }
}
