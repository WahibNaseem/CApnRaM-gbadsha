using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public partial class FranchiseeFinderFeeDetailViewModel
    {
        public int FindersFeeId { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<int> FindersFeeTypeListId { get; set; }
        public string FindersFeeTypeListName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> TotalNumOfpayments { get; set; }
        public Nullable<int> NumOfPaymentsPaid { get; set; }
        public Nullable<decimal> TotalFinderFee { get; set; }
        public Nullable<decimal> TotalDownPayment { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }
        public Nullable<int> RegionId { get; set; }
    }
}
