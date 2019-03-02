using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class NoteViewModel
    {
        public int Id { get; set; }
        public string CNote { get; set; }

        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
