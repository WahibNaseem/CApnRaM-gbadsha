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
    
    public partial class VendorInvoice
    {
        public int VendorInvoiceId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> FranchisesTransactionTypeListId { get; set; }
        public string VendorInvoiceNo { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> ExtendedAmount { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> GrossTotal { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public string VendorCode { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
    }
}
