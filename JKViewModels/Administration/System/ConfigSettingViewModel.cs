using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.System
{
   public class ConfigSettingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int ValueType { get; set; }
        public int Applied { get; set; }
        public int Grouptype { get; set; }

        //-- the status of the object
        public int StatusId { get; set; }
        
    }
}
