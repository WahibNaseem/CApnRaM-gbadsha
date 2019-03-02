using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class PaymentScheduleTypeViewModel
    {
        public int CommissionPaymentScheduleTypeId { get; set; } //(int, not null)
        public string Name { get; set; } //(varchar(500), null)
        public bool IsActive { get; set; } //(bit, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(int, null)      
    }

}
