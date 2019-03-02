using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class EmailViewModel
    {
        public int EmailId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ContactTypeListId { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }


    }
}
