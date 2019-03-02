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
    
    public partial class vw_ContractDetail
    {
        public int ContractDetailId { get; set; }
        public Nullable<int> ContractId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public string ServiceTypeName { get; set; }
        public Nullable<int> BillingFrequencyListId { get; set; }
        public string BillingFrequencyListName { get; set; }
        public Nullable<int> LineNumber { get; set; }
        public string SquareFootage { get; set; }
        public Nullable<int> CleanTimes { get; set; }
        public Nullable<int> CleanFrequencyListId { get; set; }
        public string CleanFrequencyName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> BPPAdmin { get; set; }
        public string Separateinvoice { get; set; }
        public string CPIIncrease { get; set; }
        public string AccountRebate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string Mon { get; set; }
        public string Tues { get; set; }
        public string Wed { get; set; }
        public string Thur { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public Nullable<int> ContractTermMonth { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public string isActive { get; set; }
        public string AccountTypeName { get; set; }
    }
}