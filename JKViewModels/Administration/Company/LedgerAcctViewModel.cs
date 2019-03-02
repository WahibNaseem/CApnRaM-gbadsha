using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Company
{
    public class LedgerAcctViewModel
    {
        public int LedgerAcctId { get; set; }
        public Nullable<int> GLAccountTypeListId { get; set; }
        public string GL_Number { get; set; }
        public string GL_Name { get; set; }
        public string Description { get; set; }
    }
}
