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
    
    public partial class CreditTemp
    {
        public int CreditTempId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public Nullable<int> CreditReasonListId { get; set; }
        public string CreditDescription { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}