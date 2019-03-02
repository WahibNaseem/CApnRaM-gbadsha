using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class FCFeeModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class EmailSettingsModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class BPPAdminFeeModel
    {
        public string Id { get; set; }
        public string StartAmount { get; set; }
        public string EndAmount { get; set; }
        public string Amount { get; set; }
    }

    public class TransactionNumberConfigModel
    {
        public string Id { get; set; }
        public string NamePre { get; set; }
        public string Number { get; set; }
    }

}
