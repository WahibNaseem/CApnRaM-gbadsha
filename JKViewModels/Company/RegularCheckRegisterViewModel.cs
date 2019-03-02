using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class RegularCheckRegisterViewModel
    {
        public int id { get; set; }
        public string SourceId { get; set; }
        public Nullable<System.DateTime> transdate { get; set; }
        public string descr { get; set; }
        public string reference { get; set; } //(varchar(125), null)
        public double dramountfgn { get; set; } //(int, null)
        public double cramountfgn { get; set; } //(int, null)
        public double balance { get; set; } //(int, null)
    }

}


