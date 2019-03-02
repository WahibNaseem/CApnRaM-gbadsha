using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    public class NegativeDueViewModel
    {
        public int NegativeDueId { get; set; }
        public int FranchiseeId { get; set; }
        public string RegionName { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public string FranchiseeNo { get; set; }

    }

    public class NegativeDueModel
    {
        public int NegativeDueId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> ApplySourceId { get; set; }
        public Nullable<bool> Rollover { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AppliedAmount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> TransactionNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ImpId { get; set; }
    }

    public class NegativeDueListReportViewModel
    {
        public int NegativeDueId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<DateTime> TransactionDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AppliedAmount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string FranchiseeNo { get; set; }
        public string Name { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<bool> Rollover { get; set; }
    }
}
