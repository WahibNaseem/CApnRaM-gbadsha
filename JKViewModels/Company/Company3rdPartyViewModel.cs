using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
   public class Company3rdPartyViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> BankId { get; set; }
        public string MainPhone { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Number { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Email { get; set; }
    }
}
