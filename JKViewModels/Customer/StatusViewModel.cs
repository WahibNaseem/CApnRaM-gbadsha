using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class StatusViewModel
    {
        public int StatusId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> ReasonListId { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
}
