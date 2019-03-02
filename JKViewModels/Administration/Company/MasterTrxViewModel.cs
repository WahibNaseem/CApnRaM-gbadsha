using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Company
{
    public class MasterTrxViewModel
    {
        public int MasterTrxId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        
    }
}
