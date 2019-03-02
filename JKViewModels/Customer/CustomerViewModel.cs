using JKViewModels.Franchise;
using JKViewModels.Generic;
using System;
using System.Collections.Generic;

namespace JKViewModels.Customer
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public bool NationalAccount { get; set; }
        public bool Reference { get; set; }
        public bool Parent { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> ContractTypeListId { get; set; }        
        public Nullable<bool> IsActive { get; set; }

        public int RegionId { get; set; }

        public int LogId { get; set; }

    }
}