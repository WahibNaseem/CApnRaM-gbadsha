using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class LockboxEDIDataViewModel
    {
        public int LockboxEDIDetailId { get; set; }

        public int LockboxEDIId { get; set; }
        public DateTime LockboxDate { get; set; }
        public string PriorityCode { get; set; }

        public string Destination { get; set; }

        public string Origin { get; set; }

        public string YYMMDD { get; set; }

        public string HHMM { get; set; }

        public string ReferenceCode { get; set; }

        public string ServiceType { get; set; }

        public string RecordSize { get; set; }

        public string BlockSize { get; set; }

        public string FormatCodeUncompressed { get; set; }

        public string BatchNumber { get; set; }

        public string LockboxNumber { get; set; }

        public string ItemNumber { get; set; }

        public Nullable<decimal> DollarAmount { get; set; }

        public string TransitRoutingNumber { get; set; }

        public string AccountNumber { get; set; }

        public string CheckNumber { get; set; }

        public string CustomerNo { get; set; }

        public Nullable<int> CustomerId { get; set; }

        public Nullable<int> InvoiceId { get; set; }

        public string InvoiceNo { get; set; }

        public string TypeofOverFlowingRecord { get; set; }

        public string SequenceNumber { get; set; }

        public string LastOverflowIndicator { get; set; }

        public Nullable<int> TotalItems { get; set; }

        public Nullable<decimal> TotalDollars { get; set; }

        public string RecordCount { get; set; }

        public string CustomerName { get; set; }

        public Nullable<int> RegionId { get; set; }

        public string RegionName { get; set; }

        public Nullable<int> RegionNo { get; set; }

        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> InvoiceOAmount { get; set; }

        public Nullable<decimal> ApplyAmount { get; set; }

        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> OverflowAmount { get; set; }

        public string BankName { get; set; }

        public string BankState { get; set; }

        public string RegionBank { get; set; }

        public Nullable<System.DateTime> InvoiceDate { get; set; }

        public Nullable<int> RowNo { get; set; }

        public Nullable<bool> IsProcessed { get; set; }
        public Nullable<System.DateTime> ProcessedOn { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsODeposit { get; set; }
        public string DepositReason { get; set; }
        public Nullable<int> DepositServiceTypeListId { get; set; }
        public string DepositPayeeType { get; set; }
        public Nullable<int> DepositPayeeId { get; set; }
        public string DepositPayeeName { get; set; }
        public string DepositPayeeNo { get; set; }
    }

    public class LockboxEDIData
    {
        public int LockboxEDIId { get; set; }
        public string LockboxNumber { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string RegionName { get; set; }
        public Nullable<decimal> LockboxAmount { get; set; }
        public Nullable<decimal> LockboxApplyAmount { get; set; }
        public Nullable<decimal> LockboxExecptionAmount { get; set; }
        public List<LockboxEDIDataMatchedViewModel> LockboxEDIDataMatched { get; set; }
        public List<LockboxEDIDataUnMatchedViewModel> LockboxEDIDataUnmatched { get; set; }
    }
    public class LockboxEDIDataMatchedViewModel
    {
        public int MatchedId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> ApplyAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public int LockboxEDIDetailId { get; set; }
        public Nullable<int> RowNo { get; set; }
        public Nullable<bool> IsODeposit { get; set; }
    }
    public class LockboxEDIDataUnMatchedViewModel
    {
        public int UnMatchedId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<decimal> ApplyAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public int LockboxEDIDetailId { get; set; }
        public Nullable<int> RowNo { get; set; }
        public Nullable<bool> IsDeposit { get; set; }
    }


    public class LockboxPendingViewModel
    {
        public int LockboxId { get; set; } //(int, not null)
        public int RegionId { get; set; } //(int, null)
        public string RegionName { get; set; } //(varchar(250), null)
        public DateTime LockboxDate { get; set; } //(date, null)
        public string LockboxNumber { get; set; } //(varchar(50), null)
        public string YYMMDD { get; set; } //(varchar(50), null)
        public string LockboxFileName { get; set; } //(nvarchar(250), null)
        public decimal MatchedAmount { get; set; } //(decimal(38,2), null)
        public decimal UnmatchedAmount { get; set; } //(decimal(38,2), null)
        public decimal LockboxAmount { get; set; } //(decimal(18,2), null)
        public int Status { get; set; } //(int, null)
    }

}
