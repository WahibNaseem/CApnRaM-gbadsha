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
    
    public partial class LeaseBill
    {
        public int LeaseBillId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public string LeaseBillNo { get; set; }
        public Nullable<System.DateTime> LeaseBillDate { get; set; }
        public Nullable<int> LeaseId { get; set; }
        public string PaymentNumber { get; set; }
        public Nullable<int> TotalNumOfPayments { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string LeaseSerialNo { get; set; }
        public Nullable<int> LeaseLastPaymentNo { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<bool> DisplayLeaseBillMessage { get; set; }
        public Nullable<int> LeaseBillImpId { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
    }
}