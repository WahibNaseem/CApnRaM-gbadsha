using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class AddZipCodePopupModel
    {
        public int TerritoryId { get; set; }
        public string ZipCode { get; set; }
        public List<TerritoryViewModel> TerritoryList { get; set; }
    }
}
