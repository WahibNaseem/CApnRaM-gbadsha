using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class FileTypeListViewModel
    {
        public int FileTypeListId { get; set; }
        public string Name { get; set; }
        public Nullable<int> TypeListId { get; set; }

        public Nullable<bool> IsCustomerRequired { get; set; }
        public Nullable<int> CustomerOrderBy { get; set; }
        public Nullable<bool> IsFranchiseeRequired { get; set; }
        public Nullable<int> FranchiseeOrderBy { get; set; }
    }
}
