using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.JKControl;

namespace JKViewModels.Company
{
    public class BankViewModel
    {
        public int BankId { get; set; }
        public Nullable<int> LockBoxId { get; set; }
        public Nullable<int> BankTypeListId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string BankDescription { get; set; }
        public string BankNumber { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
