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
    
    public partial class CCTransaction
    {
        public int ID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string BatchID { get; set; }
        public Nullable<int> invoiceID { get; set; }
        public Nullable<int> Last4CCNo { get; set; }
        public Nullable<int> GatewayID { get; set; }
        public string TransactionID { get; set; }
        public Nullable<int> ApprovedID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> CreateByID { get; set; }
        public string Status { get; set; }
        public Nullable<int> RecordType { get; set; }
    
        public virtual Invoice Invoice { get; set; }
    }
}
