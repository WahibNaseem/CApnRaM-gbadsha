using System;
using System.Collections.Generic;
using JKViewModels;
using JKViewModels.Common;

namespace JKApi.WebAPI.Dtos
{
    // ======================================================================================
    #region Request
    // ======================================================================================

    public class InspectionFormListByRegionRequestDto : PageRequestDto
    {
        public int RegionId { get; set; }
    }

    public class InspectionFormListByFranchiseeRequestDto : PageRequestDto
    {
        public int FranchiseeId { get; set; }
    }

    public class InspectionFormListByCustomerDto : PageRequestDto
    {
        public int CustomerId { get; set; }
    }

    public class InspectionFormListByJobDto : PageRequestDto
    {
        public int JobId { get; set; }
    }

    public class InspectionFormUpdateRequestDto : IRequestDto
    {
        public int InspectionFormId { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public int ServiceTypeListId { get; set; }
        public int AccountTypeListId { get; set; }
        public InspectionStatus InspectionStatusId { get; set; }
        public DateTime? CallDate { get; set; }
        public DateTime? RecordedDate { get; set; }
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
        public IList<InspectionFormSectionResponseDto> Sections { get; set; }
        public InspectionFormUpdateRequestDto()
        {
            Sections = new List<InspectionFormSectionResponseDto>();
        }
    }

    public class FormByInspectionRequestDto : IRequestDto
    {
        public int InspectionId { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    public class InspectionFormReportRequestDto : IRequestDto
    {
        public int InspectionFormId { get; set; }
    }

    public class InspectionFormRequestDto : IRequestDto
    {
        public int InspectionId { get; set; }
        public IList<InspectionFormSectionResponseDto> FormSections { get; set; }
        public InspectionFormRequestDto()
        {
            FormSections = new List<InspectionFormSectionResponseDto>();
        }
    }

    public class InspectionAddByAccountTypeRequestDto : IRequestDto
    {
        public int AccountTypeListId { get; set; }
        public int JobId { get; set; }
    }

    public class InspectionFormSectionUpdateRequestDto : IRequestDto
    {
        public int InspectionFormId { get; set; }
        public int InspectionFormSectionId { get; set; }
        public bool SectionAutoFail { get; set; }
        public string SectionAutoFailReason { get; set; }
        public IList<InspectionFormItemUpdateRequestDto> FormItems { get; set; }
        public InspectionFormSectionUpdateRequestDto()
        {
            FormItems = new List<InspectionFormItemUpdateRequestDto>();
        }
    }

    public class InspectionFormItemUpdateRequestDto : IRequestDto
    {
        public int InspectionFormItemId { get; set; }
        public string FormItemValue { get; set; }
        public bool IsRequired { get; set; }
    }

    #endregion
    // ======================================================================================
    #region Response
    // ======================================================================================

    public class InspectionFormResponseDto
    {
        public int InspectionFormId { get; set; }
        public int InspectionId { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountType { get; set; }
        public string FormName { get; set; }
        public string Description { get; set; }
        public decimal PassPoints { get; set; }
        public decimal FailPoints { get; set; }
        public decimal NeedImprovementPoints { get; set; }
        public decimal ScorePercent { get; set; }
        public string SignatureUrl { get; set; }
        public IList<InspectionFormSectionResponseDto> Sections { get; set; }
        public InspectionFormResponseDto()
        {
            Sections = new List<InspectionFormSectionResponseDto>();
        }
    }

    public class InspectionFormSectionResponseDto : IResponseDto
    {
        public int InspectionFormSectionId { get; set; }
        public int InspectionFormId { get; set; }
        public int SectionOrder { get; set; }
        public string SectionName { get; set; }
        public int SectionStatus { get; set; }
        public int NumberOfItems { get; set; }
        public decimal ScorePercent { get; set; }
        public decimal PassPoints { get; set; }
        public decimal FailPoints { get; set; }
        public decimal NeedImprovementPoints { get; set; }
        public bool SectionAutoFail { get; set; }
        public string SectionAutoFailReason { get; set; }

        public IList<InspectionFormItemResponseDto> Items { get; set; }
        public InspectionFormSectionResponseDto()
        {
            Items = new List<InspectionFormItemResponseDto>();
        }
    }

    public class InspectionFormItemResponseDto : IResponseDto
    {
        public int InspectionFormItemId { get; set; }
        public int InspectionFormSectionId { get; set; }
        public int FormItemOrder { get; set; }
        public FormItemType FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public bool IsDirty { get; set; }
        public bool IsRequired { get; set; }
    }

    #endregion
    // ======================================================================================
}