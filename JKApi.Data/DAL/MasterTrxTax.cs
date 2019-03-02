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
    
    public partial class MasterTrxTax
    {
        public int MasterTrxTaxId { get; set; }
        public Nullable<int> MasterTrxDetailId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> TaxRateId { get; set; }
        public Nullable<decimal> TaxratePercentage { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> AmountTypeListId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<bool> FRRevenues { get; set; }
        public Nullable<bool> FRDeduction { get; set; }
        public string County { get; set; }
        public string CountyTaxCode { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
