using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{

  
    public class AccountingFeeRebateViewModel
    {
        public int AccountingFeeRebateId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> periodId { get; set; }
        public Nullable<decimal> Percentage { get; set; }
        public Nullable<decimal> GrossRevenue { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> TransactionNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }

    }

    public class AccountingFeeRebateFullViewModel
    {
        public int AccountingFeeRebateId { get; set; }
        public decimal Balance { get; set; }

        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<AccountingFeeRebateFullViewModel> AccountingFeeRebateList { get; set; }
        public List<string> AccountingFeeRebateListString { get; set; }
    }



}
