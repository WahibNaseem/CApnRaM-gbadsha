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
    
    public partial class portal_spGet_AR_CustomerWithCreditList_Result
    {
        public int CreditId { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> CreditDate { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public decimal InvAmount { get; set; }
        public decimal CrdAmount { get; set; }
        public string Other { get; set; }
    }
}
