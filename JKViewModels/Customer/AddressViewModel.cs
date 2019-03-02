using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.JKControl;

namespace JKViewModels.Customer
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }

        public Nullable<int> ClassId { get; set; }

        public Nullable<int> TypeListId { get; set; }

        public Nullable<int> ContactTypeListId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public Nullable<int> StateListId { get; set; }

        public string PostalCode { get; set; }

        public Nullable<int> CountyTaxAuthorityListId { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<int> CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string StateName { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }

        public string FullAddress
        {
            get
            {
                string addressLine = string.IsNullOrEmpty(Address2) ? "{0}" : "{0} {1}";

                return string.Format(addressLine + ", {2}, {3} {4}", Address1, Address2, City, StateName, PostalCode);
            }
        }
    }
}
