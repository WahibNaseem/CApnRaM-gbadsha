using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public Nullable<int> MenuTypeId { get; set; }
        public Nullable<DateTime> DateCreated { get; set; }
        public Nullable<DateTime> DateModified { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> MenuParentId { get; set; }
        public string PageLink { get; set; }
        public string Icon { get; set; }
    }
}
