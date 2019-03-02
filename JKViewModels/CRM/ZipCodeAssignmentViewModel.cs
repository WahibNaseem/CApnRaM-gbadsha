using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class ZipCodeAssignmentViewModel
    {
        public List<TerritoryViewModel> TerritoryList { get; set; }
        public List<ZipCodeModel> ZipCodeList { get; set; }
    }
    public class ZipCodeModel
    {
        public string ZipCode { get; set; }
        public int TerritoryAssignmentId { get; set; }
        public int? TerritoryId { get; set; }
    }
}
