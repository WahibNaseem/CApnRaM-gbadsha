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
    
    public partial class CRM_Account
    {
        public int CRM_AccountId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> FranchiseId { get; set; }
        public Nullable<int> AssigneeId { get; set; }
        public Nullable<int> ReporterId { get; set; }
        public Nullable<int> AccountType { get; set; }
        public Nullable<int> Stage { get; set; }
        public Nullable<int> StageStatus { get; set; }
        public Nullable<int> ProviderSource { get; set; }
        public Nullable<int> ProviderType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CRMImpId { get; set; }
        public string ContactName { get; set; }
        public string salutation { get; set; }
        public string currentven { get; set; }
        public string leadsource { get; set; }
        public string Accttype { get; set; }
        public string Comments { get; set; }
        public Nullable<int> fpleadId { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
