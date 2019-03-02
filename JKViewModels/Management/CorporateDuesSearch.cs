using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class CorporateDuesSearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
    } 

    public class CorporateDuesListItem
    {
        //public string Id { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string billmonth { get; set; }
        public string billyear { get; set; }
        public string typeid { get; set; }
        public string groupid { get; set; }
        public string sortorder { get; set; }
    }
    
}
