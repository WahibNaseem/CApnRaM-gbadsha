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
    
    public partial class portal_spGet_AR_PaymentChargeback_Result
    {
        public int id { get; set; }
        public Nullable<int> paymentdetailid { get; set; }
        public Nullable<int> appliedtochargebackid { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<decimal> feetotal { get; set; }
        public Nullable<decimal> taxtotal { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public Nullable<int> checksgenerated { get; set; }
        public Nullable<System.DateTime> checksposteddate { get; set; }
        public Nullable<int> checkdetailid { get; set; }
        public Nullable<int> RevenueAdjustmentChargebackId { get; set; }
    }
}