using System.Collections.Generic;

namespace JKViewModels.Administration.Company
{
    public class FullLedgerAccountViewModel
    {
        public CommonLadgerAccountViewModel CLadgerAccount { get; set; }

        public GLAccountTypeListViewModel GLAccountTypeList { get; set; }
        public List<CommonLadgerAccountViewModel> GeneralLadgerAccountList { get; set; }
    }
}
