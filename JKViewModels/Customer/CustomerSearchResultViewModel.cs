using JKViewModels.Franchise;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;

namespace JKViewModels.Customer
{    
    public class CustomerSearchResultViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public string AccountTypeListName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string RegionName { get; set; }
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }

        public string LogMessage { get; set; }
    }

    public class CustomerSearchResultViewModelListModel
    {
        public CustomerSearchResultViewModelListModel()
        {
            lstCustomerSearchResultViewModel = new List<CustomerSearchResultViewModel>();
        }

        public List<CustomerSearchResultViewModel> lstCustomerSearchResultViewModel { get; set; }
        public int RegionId { get; set; }
        public string StatusIds { get; set; }
    }
}