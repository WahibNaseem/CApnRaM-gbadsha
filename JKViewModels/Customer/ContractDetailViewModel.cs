using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContractDetailViewModel
    {
        public int ContractDetailId { get; set; }
        public Nullable<int> ContractId { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public Nullable<int> BillingFrequencyListId { get; set; }
        public Nullable<int> LineNumber { get; set; }
        public string SquareFootage { get; set; }
        public Nullable<int> CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tues { get; set; }
        public bool Wed { get; set; }
        public bool Thur { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public int CleanFrequencyListId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public bool BPPAdmin { get; set; }
        public bool SeparateInvoice { get; set; }
        public bool SubjectToFees { get; set; }
        public bool CPIIncrease { get; set; }
        public bool AccountRebate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime>  StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string ServiceTypeName { get; set; }
        //public Nullable<bool> IsActive { get; set; }
        //public Nullable<int> separateInvoice { get; set; }
        //public Nullable<int> CPIMonthApplied { get; set; }
        //public Nullable<decimal> CPIPercent { get; set; }
        public bool TaxExcempt { get; set; }
        public bool WeekEnd { get; set; }
    }

    //public class ContartDetailSpResultViewModel
    //{
    //    public int ContractDetailId { get; set; }
    //    public Nullable<int> ContractId { get; set; }
    //    public Nullable<int> ServiceTypeListId { get; set; }
    //    public Nullable<int> FrequencyListId { get; set; }
    //    public Nullable<int> LineNumber { get; set; }
    //    public string SquareFootage { get; set; }
    //    public Nullable<int> CleanTimes { get; set; }
    //    public Nullable<int> Mon { get; set; }
    //    public Nullable<int> Tues { get; set; }
    //    public Nullable<int> Wed { get; set; }
    //    public Nullable<int> Thur { get; set; }
    //    public Nullable<int> Fr { get; set; }
    //    public string CleanFrequency { get; set; }
    //    public Nullable<decimal> Amount { get; set; }
    //    public Nullable<bool> BPPAdmin { get; set; }
    //    public Nullable<bool> SeparateInvoice { get; set; }
    //    public Nullable<bool> SubjectToFees { get; set; }
    //    public Nullable<bool> CPIIncrease { get; set; }
    //    public Nullable<bool> AccountRebate { get; set; }
    //    public Nullable<int> CreatedBy { get; set; }
    //    public Nullable<System.DateTime> CreatedDate { get; set; }
    //    public string Description { get; set; }
    //}
}
