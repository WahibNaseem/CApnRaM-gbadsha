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
    
    public partial class portal_spGet_AR_InvoiceSearchListForCredit_Result
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<bool> EBill { get; set; }
        public Nullable<bool> PrintInvoice { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public int HasMultipleLineItems { get; set; }
    }
}
