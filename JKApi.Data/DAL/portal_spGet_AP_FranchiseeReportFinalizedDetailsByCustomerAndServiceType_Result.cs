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
    
    public partial class portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTypeGroupListName { get; set; }
        public int DisplayOrder { get; set; }
        public int DisplaySubReport { get; set; }
        public Nullable<int> Resell { get; set; }
        public Nullable<decimal> Subtotal { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> CreatedBy { get; set; }
    }
}