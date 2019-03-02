using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Common
{
    public class IdentificationViewModel
    {
        public int IdentificationId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ContactTypeListId { get; set; }
        public Nullable<int> IdentifierTypeListId { get; set; }
        public string IdentifierNumer { get; set; }
    }
}
