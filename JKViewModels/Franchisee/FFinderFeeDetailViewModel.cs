using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public partial class FFinderFeeDetailViewModel
    {
        public int FranchiseeId { get; set; }

        public string FranchiseeNo { get; set; }

        public string FranchiseeName { get; set; }

        public string F_Address { get; set; }

        public string F_City { get; set; }

        public string F_StateName { get; set; }

        public string F_PostalCode { get; set; }

        public int CustomerId { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string CAddress { get; set; }

        public string C_City { get; set; }

        public string C_StateName { get; set; }

        public string C_PostalCode { get; set; }

        public int FindersFeeId { get; set; }

        public Nullable<int> TransactionStatusListId { get; set; }

        public string TransactionStatusListName { get; set; }

        public Nullable<System.DateTime> StartDate { get; set; }

        public Nullable<System.DateTime> ResumeDate { get; set; }

        public string Description { get; set; }

        public int FindersFeeTypeListId { get; set; }

        public string FindersFeeTypeListName { get; set; }

        public Nullable<decimal> ContractBillingAmount { get; set; }

        public Nullable<decimal> TotalAdjustmentAmount { get; set; }

        public Nullable<decimal> Factor { get; set; }

        public Nullable<decimal> DownPayPercentage { get; set; }

        public Nullable<decimal> Interest { get; set; }

        public Nullable<int> TotalNumOfpayments { get; set; }

        public Nullable<decimal> MonthlyPaymentAmount { get; set; }

        public Nullable<decimal> FinancedAmount { get; set; }

        public Nullable<decimal> DownPaymentAmount { get; set; }

        public Nullable<decimal> TotalAmount { get; set; }

        public string Notes { get; set; }

        public Nullable<decimal> MultiTenantOccupancyAmount { get; set; }

       

        public Nullable<decimal> PaidAmount { get; set; }

        public Nullable<decimal> BalanceAmount { get; set; }

        public Nullable<int> RegionId { get; set; }
    }

    public partial class FFinderFeeDetailFullViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string F_Address { get; set; }
        public string F_City { get; set; }
        public string F_StateName { get; set; }
        public string F_PostalCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CAddress { get; set; }
        public string C_City { get; set; }
        public string C_StateName { get; set; }
        public string C_PostalCode { get; set; }
        public int FindersFeeId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public string TransactionStatusListName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public string Description { get; set; }
        public int FindersFeeTypeListId { get; set; }
        public string FindersFeeTypeListName { get; set; }
        public Nullable<decimal> ContractBillingAmount { get; set; }
        public Nullable<decimal> TotalAdjustmentAmount { get; set; }
        public Nullable<decimal> Factor { get; set; }
        public Nullable<decimal> DownPayPercentage { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<int> TotalNumOfpayments { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public Nullable<decimal> FinancedAmount { get; set; }
        public Nullable<decimal> DownPaymentAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> MultiTenantOccupancyAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<decimal> PayableOnAmount { get; set; }
        public List<FindersFeeAdjustmentViewModel> FindersFeeAdjustments { get; set; }
    }
}
