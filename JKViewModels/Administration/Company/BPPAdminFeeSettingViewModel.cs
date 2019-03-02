using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Company
{
    public class BPPAdminFeeSettingViewModel
    {
        public int Id { get; set; }
        public decimal StartAmount { get; set; }
        public decimal EndAmount { get; set; }
        public int Amount { get; set; }
    }
}
