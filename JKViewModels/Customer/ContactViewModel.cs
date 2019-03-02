using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContactViewModel
    {

        public int ContactId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ContactTypeListId { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        //public string LastName { get; set; }
        public string Title { get; set; }
        public string Attention { get; set; }
        //public string DisplayName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Name2 { get; set; }
    }
}
