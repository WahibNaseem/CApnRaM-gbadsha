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
    
    public partial class CRM_AccountFranchiseDetail
    {
        public int CRM_AccountFranchiseDetailId { get; set; }
        public Nullable<int> CRM_AccountId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string CellNumber { get; set; }
        public string FaxNumber { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string Employer { get; set; }
        public string Position { get; set; }
        public Nullable<int> LeadSource { get; set; }
        public Nullable<int> JkFull { get; set; }
        public Nullable<decimal> AmtToInvest { get; set; }
        public Nullable<System.DateTime> InfoSentDate { get; set; }
        public Nullable<System.DateTime> DisclosedDate { get; set; }
        public Nullable<System.DateTime> C5DayPaperDate { get; set; }
        public Nullable<System.DateTime> EstCloseDate { get; set; }
        public Nullable<System.DateTime> SoldDate { get; set; }
        public string FranPlan { get; set; }
        public string SoldBy { get; set; }
        public Nullable<decimal> FranAmount { get; set; }
        public Nullable<decimal> DownAmount { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        public string Representative { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> Purpose { get; set; }
        public Nullable<bool> AMorPM { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CRM_CallResultId { get; set; }
        public string SpokeWith { get; set; }
        public Nullable<int> CRM_NoteTypeId { get; set; }
        public string CRM_Note { get; set; }
        public Nullable<int> AsigneeId { get; set; }
        public Nullable<int> Stage { get; set; }
        public Nullable<int> StageStatus { get; set; }
        public Nullable<int> ProviderSource { get; set; }
        public Nullable<int> ProviderType { get; set; }
        public string ContactName { get; set; }
    }
}