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
    
    public partial class portal_spGet_C_UnprintedManualCheckList_Result
    {
        public int ManualCheckId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string PayTo { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Memo { get; set; }
        public string CheckType { get; set; }
        public int APBillId { get; set; }
    }
}