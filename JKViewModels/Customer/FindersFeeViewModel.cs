using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class FindersFeeViewModel
    {
        public int FindersFeeId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> DistributionId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public Nullable<int> FindersFeeTypeListId { get; set; }
        public Nullable<decimal> Factor { get; set; }
        public Nullable<decimal> DownPayPercentage { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<decimal> Billing { get; set; }
        public Nullable<decimal> Adjustment { get; set; }
        public Nullable<decimal> PayableOn { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> DownPayment { get; set; }
        public Nullable<decimal> AmountFinanced { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<int> NumOfpayments { get; set; }
        public Nullable<int> PaymentsBilled { get; set; }
        public Nullable<decimal> MonthlyPayment { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }
        public Nullable<decimal> MultiTenantOccupancy { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
