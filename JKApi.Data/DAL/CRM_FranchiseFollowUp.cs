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
    
    public partial class CRM_FranchiseFollowUp
    {
        public int CRM_FranchiseFollowUpId { get; set; }
        public Nullable<bool> DiscloseAdditional { get; set; }
        public Nullable<bool> StatusCreationConfirmed { get; set; }
        public Nullable<bool> NotifyNextTraining { get; set; }
        public Nullable<bool> KeepActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CRM_AccountFranchiseDetailId { get; set; }
        public string Note { get; set; }
    }
}
