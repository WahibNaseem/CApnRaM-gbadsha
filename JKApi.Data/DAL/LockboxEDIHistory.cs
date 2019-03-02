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
    
    public partial class LockboxEDIHistory
    {
        public int LockboxEDIHistoryId { get; set; }
        public int LockboxEDIId { get; set; }
        public string LockboxFileName { get; set; }
        public Nullable<System.DateTime> LockboxDate { get; set; }
        public string LockboxRaw { get; set; }
        public string LockboxNumber { get; set; }
        public string RecordType { get; set; }
        public string PriorityCode { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string YYMMDD { get; set; }
        public string HHMM { get; set; }
        public string ReferenceCode { get; set; }
        public string ServiceType { get; set; }
        public string RecordSize { get; set; }
        public string BlockSize { get; set; }
        public string BatchNumber { get; set; }
        public string FormatCodeUncompressed { get; set; }
        public string ItemNumber { get; set; }
        public string SequenceNumber { get; set; }
        public Nullable<decimal> DollarAmount { get; set; }
        public string TransitRoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string CustomerNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<decimal> ApplyAmount { get; set; }
        public string TypeofOverFlowingRecord { get; set; }
        public string LastOverflowIndicator { get; set; }
        public string TotalItems { get; set; }
        public Nullable<decimal> TotalDollars { get; set; }
        public string RegionBankName { get; set; }
        public string BankName { get; set; }
        public string BankState { get; set; }
        public string RecordCount { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RegionName { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> InvoiceBalanceAmount { get; set; }
        public Nullable<decimal> InvoiceDate { get; set; }
        public Nullable<decimal> OverflowAmount { get; set; }
        public Nullable<int> RowNo { get; set; }
        public Nullable<bool> HasException { get; set; }
        public Nullable<bool> IsProcessed { get; set; }
        public Nullable<System.DateTime> ProcessedOn { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<bool> IsODeposit { get; set; }
        public string DepositReason { get; set; }
        public Nullable<int> DepositServiceTypeListId { get; set; }
        public string DepositPayeeType { get; set; }
        public Nullable<int> DepositPayeeId { get; set; }
        public string DepositPayeeName { get; set; }
        public string DepositPayeeNo { get; set; }
    }
}
