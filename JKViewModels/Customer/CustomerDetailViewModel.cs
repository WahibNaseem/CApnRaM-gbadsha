using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerDetailViewModel
    {
        ////Main Information
        //public string Account_Type { get; set; } = string.Empty;
        //public string Address { get; set; } = string.Empty;
        //public string City { get; set; } = string.Empty;
        //public string Phone { get; set; } = string.Empty;
        //public string Fax { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;

        ////Contact Information
        //public string ContactName { get; set; } = string.Empty;
        //public string Title { get; set; } = string.Empty;
        //public string CustomerEmail { get; set; } = string.Empty;
        //public string CustomerPhone { get; set; } = string.Empty;
        //public string CustomerCell { get; set; } = string.Empty;
        //public string PhoneExtension { get; set; } = string.Empty;
        //public string  DisplayName { get; set; }
        //public decimal Amount { get; set; }

        //Initial Information

        public string CustomerId { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string RegionId { get; set; }
        //Main Information
        public string Account_Type { get; set; }
        public string ContractTypeList { get; set; }
        public string ContractTypeListId { get; set; }

        public string Address { get; set; }

        //For second line City and state
        public string Address2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string Ext { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        //Contact Information
        public string ContactName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerCell { get; set; } = string.Empty;
        public string PhoneExtension { get; set; } = string.Empty;
        public string DisplayName { get; set; }
        public decimal Amount { get; set; }

        public int MaintenanceId { get; set; }
        public string MaintenanceTypeName { get; set; }
        public string StatusName { get; set; }
        public decimal Balance { get; set; }
        public string StatusDate { get; set; }
        public string ResumeDate { get; set; }

        public int LogId { get; set; }
        public string LogMessage { get; set; }

        public string LogMessageColor { get; set; }
    }
}
