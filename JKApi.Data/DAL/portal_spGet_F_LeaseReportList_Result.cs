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
    
    public partial class portal_spGet_F_LeaseReportList_Result
    {
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string TransactionNumber { get; set; }
        public string LeaseNumber { get; set; }
        public string Description { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string InstallmentNumber { get; set; }
        public Nullable<int> InstallmentCount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public string TransactionStatus { get; set; }
        public string RegionName { get; set; }
    }
}
