//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.DAL
{
    using System;
    
    public partial class sp_GetTemplateAreaItemListByArea_Result
    {
        public int TemplateAreaItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> FormItemType { get; set; }
        public string FormItemValue { get; set; }
        public Nullable<bool> IsDirty { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public bool IsEnable { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}