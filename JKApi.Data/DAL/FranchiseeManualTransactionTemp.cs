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
    
    public partial class FranchiseeManualTransactionTemp
    {
        public int FranchiseeManualTransactionTempId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> NumOfPayments { get; set; }
        public Nullable<int> PaymentsBilled { get; set; }
        public Nullable<decimal> GrossTotal { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> ItemAmount { get; set; }
        public Nullable<decimal> Subtotal { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> FranchiseeManualTrxCreditReasonListId { get; set; }
        public Nullable<int> ImpId { get; set; }
        public Nullable<bool> ReSell { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public Nullable<System.DateTime> VendorInvoiceDate { get; set; }
        public Nullable<bool> IsCredit { get; set; }
        public Nullable<int> FranchsieeTrxTypeListId { get; set; }
        public string PaymentNo { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public string VendorCode { get; set; }
    }
}
