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
    
    public partial class CRM_Close
    {
        public int CRM_CloseId { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public Nullable<int> CRM_AccountFranchiseDetailId { get; set; }
        public Nullable<bool> HaveBackgroundCheck { get; set; }
        public Nullable<bool> SignedAgreement { get; set; }
        public Nullable<bool> DocumentSalesCRM { get; set; }
        public Nullable<bool> NotifyAccountPlacement { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> Mon { get; set; }
        public Nullable<bool> Fri { get; set; }
        public Nullable<bool> Sat { get; set; }
        public Nullable<bool> Sun { get; set; }
        public Nullable<bool> Wed { get; set; }
        public Nullable<bool> Thu { get; set; }
        public Nullable<bool> Tue { get; set; }
        public Nullable<decimal> PropAmount { get; set; }
        public Nullable<int> InitialClean { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<decimal> InitialCleanAmount { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ContractTerm { get; set; }
        public Nullable<int> ServiceType { get; set; }
        public Nullable<int> BillingFrequency { get; set; }
        public Nullable<int> CleanTime { get; set; }
        public Nullable<int> CleanFrequency { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> Weekend { get; set; }
    }
}
