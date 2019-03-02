using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Common
{
    public class CommonGMapAddressViewModel
    {
        public List<GMapAddressViewModel> lstAllData { get; set; }
        public List<GMapAddressViewModel> lstDistinctAddress { get; set; }
    }
    public class GMapAddressViewModel
    {
        public int AddressId { get; set; }
        public string CAddress { get; set; }
    }
}
