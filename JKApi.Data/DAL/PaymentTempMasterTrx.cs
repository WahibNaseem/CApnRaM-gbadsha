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
    
    public partial class PaymentTempMasterTrx
    {
        public int PaymentTempMasterTrxId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> BatchId { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public Nullable<System.DateTime> TrxDate { get; set; }
        public Nullable<int> MasterTrxImpId { get; set; }
        public Nullable<int> HeaderId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}