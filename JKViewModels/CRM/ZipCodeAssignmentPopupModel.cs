using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class ZipCodeAssignmentPopupModel
    {
        public List<int> TerritoryAssignmentIds { get; set; }
        public int CurrentTerritoryId { get; set; }
        public int TerritoryId { get; set; }
        public List<TerritoryViewModel> TerritoryList { get; set; }
    }
}
