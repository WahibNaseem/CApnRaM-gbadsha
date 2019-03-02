using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContactTypeListViewModel
    {
        public int ContactTypeListId { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    }
}
