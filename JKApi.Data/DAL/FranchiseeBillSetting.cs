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
    
    public partial class FranchiseeBillSetting
    {
        public int FranchiseeBillSettingsId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<bool> Chargeback { get; set; }
        public Nullable<bool> BBPAdministrationFee { get; set; }
        public Nullable<bool> AccountRebate { get; set; }
        public Nullable<bool> GenerateReport { get; set; }
        public Nullable<bool> Incorporated { get; set; }
        public Nullable<bool> InHouse { get; set; }
        public string C1099Name { get; set; }
        public Nullable<bool> Print1099 { get; set; }
        public Nullable<int> CountyTaxAuthorityListId { get; set; }
        public string PayeeName { get; set; }
        public Nullable<int> ChargebackMethodListId { get; set; }
        public string PayeeName2 { get; set; }
    
        public virtual Franchisee Franchisee { get; set; }
    }
}
