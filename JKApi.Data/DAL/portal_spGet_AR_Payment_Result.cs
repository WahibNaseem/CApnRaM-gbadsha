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
    
    public partial class portal_spGet_AR_Payment_Result
    {
        public int id { get; set; }
        public Nullable<int> appliedtoinvoiceId { get; set; }
        public string checkno { get; set; }
        public Nullable<System.DateTime> checkdate { get; set; }
        public Nullable<decimal> subamount { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<int> type { get; set; }
        public string notes { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public Nullable<int> reversed { get; set; }
        public Nullable<int> appliedtopaymentid { get; set; }
        public Nullable<int> billmonth { get; set; }
        public Nullable<int> billyear { get; set; }
        public Nullable<int> subtype { get; set; }
        public Nullable<int> transferred { get; set; }
        public Nullable<int> transferredid { get; set; }
        public Nullable<int> transferredby { get; set; }
        public Nullable<System.DateTime> transferreddate { get; set; }
        public string DescriptionNotes { get; set; }
    }
}
