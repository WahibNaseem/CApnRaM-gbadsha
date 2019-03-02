using System;
using System.Collections.Generic;

namespace JKApi.WebAPI.Dtos.Inspection
{
    // ======================================================================================
    #region Request
    // ======================================================================================

    /// <summary>
    /// InspectionRequestDto
    /// </summary>
    public class InspectionRequestDto : IRequestDto
    {
        /// <summary>
        /// ContractDetailId
        /// </summary>
        public int ContractDetailId { get; set; }
    }

    /// <summary>
    /// InspectionFormRequestDto
    /// </summary>
    public class InspectionFormRequestDto : IRequestDto
    {
        /// <summary>
        /// InspectionId
        /// </summary>
        public int InspectionId { get; set; }
    }

    #endregion

    // ======================================================================================
    #region Response
    // ======================================================================================

    /// <summary>
    /// InspectionResponseDto
    /// </summary>
    public class InspectionResponseDto : IResponseDto
    {
        /// <summary>
        /// InspectionId
        /// </summary>
        public int InspectionId { get; set; }

        /// <summary>
        /// InspectionStatus
        /// </summary>
        public int InspectionStatus { get; set; }

        /// <summary>
        /// CallDate
        /// </summary>
        public DateTime? CallDate { get; set; }

        /// <summary>
        /// RecordedDate
        /// </summary>
        public DateTime? RecordedDate { get; set; }

        /// <summary>
        /// UploadedDate
        /// </summary>
        public DateTime? UploadedDate { get; set; }

        /// <summary>
        /// ScorePercent
        /// </summary>
        public double ScorePercent { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// CustomerEvaluation
        /// </summary>
        public string CustomerEvaluation { get; set; }
    }

    /// <summary>
    /// InspectionFormResponseDto
    /// </summary>
    public class InspectionFormResponseDto : IResponseDto
    {
        /// <summary>
        /// InspectionFormId
        /// </summary>
        public int InspectionFormId { get; set; }

        /// <summary>
        /// ServiceType
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// FormName
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// FormItems
        /// </summary>
        public IList<InspectionFormItemResponseDto> FormItems { get; set; }

        /// <summary>
        /// Construct a new InspectionFormResponseDto
        /// </summary>
        public InspectionFormResponseDto()
        {
            FormItems = new List<InspectionFormItemResponseDto>();
        }
    }

    /// <summary>
    /// InspectionFormItemResponseDto
    /// </summary>
    public class InspectionFormItemResponseDto : IResponseDto
    {
        /// <summary>
        /// InspectionFormItemId
        /// </summary>
        public int InspectionFormItemId { get; set; }

        /// <summary>
        /// FormValue
        /// </summary>
        public string FormValue { get; set; }

        /// <summary>
        /// ItemOrder
        /// </summary>
        public int ItemOrder { get; set; }

        /// <summary>
        /// SectionOrder
        /// </summary>
        public int SectionOrder { get; set; }

        /// <summary>
        /// SectionName
        /// </summary>
        public string SectionName { get; set; }
    }

    #endregion

    // ======================================================================================
}