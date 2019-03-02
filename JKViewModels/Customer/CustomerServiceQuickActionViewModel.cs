using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerServiceQuickActionViewModel
    {
        public int NewAccountCount { get; set; }
        public int CallBackCount { get; set; }
        public int TransferCount { get; set; }
        public int ComplaintCount { get; set; }
        public int FailedInspectionCount { get; set; }
        public int PendingCancellationCount { get; set; }
    }
}
