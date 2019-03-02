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
    
    public partial class Bank
    {
        public int BankId { get; set; }
        public Nullable<int> LockBoxId { get; set; }
        public Nullable<int> BankTypeListId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BankDescription { get; set; }
        public string BankNumber { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> NextCheckNumber { get; set; }
        public string TransitCode { get; set; }
        public Nullable<int> LedgerAcctId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string Address2 { get; set; }
    }
}