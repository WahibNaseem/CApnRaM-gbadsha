using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CollectionsCallLogModel 
    {
        public int CollectionsCallLogId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<System.DateTime> CallDate { get; set; }
        public string CallTime { get; set; }
        public Nullable<int> Internal { get; set; }
        public Nullable<int> InitiatedBy { get; set; }
        public Nullable<int> StatusResultListId { get; set; }
        public string SpokeWith { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> CallBack { get; set; }
        public Nullable<int> FollowUpBy { get; set; }
        public string EmailNotesTo { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public string status { get; set; }
        public string strCallBack { get; set; }
        public int[] arrFranchiseIds { get; set; }
        public bool boolInternal { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDescription { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime InvoiceDate { get; set; }
       public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public Nullable<int> CallLogInitiatedByTypeListId { get; set; }
        public Nullable<bool> IsCallBack { get; set; }
        public Nullable<System.TimeSpan> CallBackTime { get; set; }        
    }

    public class CollectionsCallLogList
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string InvoiceDescription { get; set; }
        public decimal OriginalTotal { get; set; }
        public decimal BalanceTotal { get; set; }
        
    }

    public class IncreaseDecreaseHistoryModel
    {
        public int IncreaseDecreaseHistoryId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int MaintenanceTypeListId { get; set; }
        public string MaintenanceTypeListName { get; set; }
        public string MaintenanceDetailTypeListName { get; set; }
        public string IncreaseDecreaseTypeListName { get; set; }
        public int MaintenanceTempId { get; set; }
        public int MaintenanceDetailTypeListId { get; set; }
        public int SourceId { get; set; }
        public string  SourceType { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public int ClassId { get; set; }
        public int IncreaseDecreaseTypeListId { get; set; }
        public Nullable<DateTime> TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal CPIPresent { get; set; }
        public int PeriodId { get; set; }

        public decimal DetailPrevAmt { get; set; }
        public decimal DetailVarAmt { get; set; }
        public decimal DetailNewAmt { get; set; }
        public decimal ContractPrevAmt { get; set; }
        public decimal ContractVarAmt { get; set; }
        public decimal ContractNewAmt { get; set; }

    }
}
