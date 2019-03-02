using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class PhoneViewModel
    {
        public int PhoneId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ContactTypeListId { get; set; }
        public string Phone { get; set; }
        public string Phone1 { get; set; }
        public string PhoneExt { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public Nullable<int> CountryCodeListId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

    }
}
