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
    using System.Collections.Generic;
    
    public partial class CRM_FollowUp
    {
        public int CRM_FollowUpId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public Nullable<int> CRM_AccountFranchiseDetailId { get; set; }
        public Nullable<int> Close { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PurposeId { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}