using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class OfficeContactViewModel
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
