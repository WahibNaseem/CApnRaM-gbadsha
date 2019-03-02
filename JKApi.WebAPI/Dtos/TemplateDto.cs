using System.Collections.Generic;
using JKViewModels;

namespace JKApi.WebAPI.Dtos
{
    // ======================================================================================
    #region Request

    public class TemplateAreaRequestDto
    {
        public int InspectionFormId { get; set; }
        public int TemplateAreaId { get; set; }
        public string AreaName { get; set; }
    }

    #endregion
    // ======================================================================================
    #region Response

    public class TemplateAreaResponseDto
    {
        public int TemplateAreaId { get; set; }
        public string AreaName { get; set; }
    }

    public class TemplateAreaItemResponseDto
    {
        public int TemplateAreaId { get; set; }
        public int TemplateAreaItemId { get; set; }
        public string ItemName { get; set; }
        public FormItemType FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public bool IsDirty { get; set; }
        public bool IsRequired { get; set; }
    }

    #endregion
    // ======================================================================================
}