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
    
    public partial class FindersFee
    {
        public int FindersFeeId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> DistributionId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> FindersFeeTypeListId { get; set; }
        public Nullable<decimal> Factor { get; set; }
        public Nullable<decimal> DownPayPercentage { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<decimal> ContractBillingAmount { get; set; }
        public Nullable<decimal> TotalAdjustmentAmount { get; set; }
        public Nullable<decimal> PayableOnAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> DownPaymentAmount { get; set; }
        public Nullable<bool> IncludeDownPayInFirstPay { get; set; }
        public Nullable<decimal> FinancedAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<int> TotalNumOfpayments { get; set; }
        public Nullable<int> NumOfPaymentsPaid { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public Nullable<bool> DownPaymentPaid { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> MultiTenantOccupancyAmount { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> SequenceNum { get; set; }
        public Nullable<int> ImpId { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> MonthlyPaymentPercentage { get; set; }
        public string FindersFeeNumber { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Franchisee Franchisee { get; set; }
    }
}
