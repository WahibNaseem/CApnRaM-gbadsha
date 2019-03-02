using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class LockboxImportViewModel
    {
        [DisplayName("Check Date")]
        public DateTime checkDate { get; set; }
        public string filename { get; set; }
    }




    public class CommonTransmissionViewModel
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }        
        public int LockboxEDIId { get; set; }
        public string LockboxFileName { get; set; }
        public DateTime LockboxDate { get; set; }
        public string LockboxData { get; set; }
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
        public decimal DollarAmount { get; set; } //DollarAmount
        public string TransitRoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string CustomerNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal ApplyAmount { get; set; }
        public string TypeofOverFlowingRecord { get; set; }
        public string LastOverflowIndicator { get; set; }
        public string TotalItems { get; set; }
        public decimal TotalDollars { get; set; }
        public string RegionBankName { get; set; }
        public string BankName { get; set; }
        public string BankState { get; set; }
        public string RecordCount { get; set; }
    }



}