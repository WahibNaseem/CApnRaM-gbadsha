using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.General
{
   public  class CPIViewModel
    {
        public int id { get; set; }
        public int billmonth { get; set; }
        public int billyear { get; set; }
        public decimal percent { get; set; }
        public string description { get; set; }
        public Nullable<int> applied { get; set; }
        public int userid { get; set; }
        public int createdby { get; set; }
        public int modifiedby { get; set; }
    }
}
