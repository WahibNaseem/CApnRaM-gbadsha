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
    
    public partial class portal_spGet_FP_InvoiceSearchListForCreditInvoice_Result
    {
        public int BillingPayId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string DetailDescription { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string InvoiceNo { get; set; }
    }
}