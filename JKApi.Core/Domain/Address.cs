using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Business.Domain
{
    public class Address : IDomainBase
    {
        public int? ID { get; set; }

        public int? ContactID { get; set; }

        public Enumeration.AddressType AddressType { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string Street3 { get; set; }

        public string Street4 { get; set; }

        public string City { get; set; }

        public int UsStateType { get; set; }

        public int State { get; set; }

        public int CountryType { get; set; }

        public int Country { get; set; }

        public string ZipCode { get; set; }

        public string Province { get; set; }

        public bool IsActive { get; set; }

        public Address()
        {
            ID = 0;
            Street1 = "";
            Street2 = "";
            Street3 = "";
            Street4 = "";
            UsStateType = 1;
            CountryType = 1;
            Province = "";
            IsActive = true;
        }
    }
}
