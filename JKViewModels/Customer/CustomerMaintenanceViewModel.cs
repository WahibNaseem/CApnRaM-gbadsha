using JKViewModels.Franchise;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;

namespace JKViewModels.Customer
{
    public class CustomerMaintenanceViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        //Customer Info
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerPincode { get; set; }

        //Customer Billing Info
        public string CustomerBillingName { get; set; }
        public string CustomerBillingAddress { get; set; }
        public string CustomerBillingCity { get; set; }
        public string CustomerBillingState { get; set; }
        public string CustomerBillingPincode { get; set; }

        //Customer Status Info
        public int? StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public string StatusListName { get; set; }
        public Nullable<int> ReasonListId { get; set; }
        public string ReasonListName { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        //Customer FindersFee Info
        public List<CustomerStatusFindersFeeViewModel> FindersFee { get; set; }
    }


    public class CustomerStatusFindersFeeViewModel
    {

        public int? FindersFeeId { get; set; }
        public int? LineNumber { get; set; }
        public int? ServiceTypeListId { get; set; }
        public string ServiceTypeListName { get; set; }        
        public Nullable<int> FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public Nullable<System.DateTime> FindersFeeStopDate { get; set; }
        public bool FindersFeeHasCancellationFee { get; set; }

        public string FindersFeeNumber { get; set; }
        public Nullable<int> TotalNumOfpayments { get; set; }
        public Nullable<int> NumOfPaymentsPaid { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Description { get; set; }
    }
}