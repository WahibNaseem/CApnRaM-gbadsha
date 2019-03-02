using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class CommissionPaymentScheduleViewModel
    {
        //CommissionPaymentScheduleId int
        public int CommissionPaymentScheduleId { get; set; }
        //Decription  varchar(500)
        public string Description { get; set; }
        //PaymentScheduleTypeId int
        public int PaymentScheduleTypeId { get; set; }
        public string PaymentScheduleTypeDescription { get; set; }
        //Amount  decimal (18, 2)
        public decimal Amount { get; set; }
        //Term1_StartMonth int
        public int Term1_StartMonth { get; set; }
        //Term1_EndMonth  int
        public int Term1_EndMonth { get; set; }
        //Term1_Percent   decimal (18, 2)
        public decimal Term1_Percent { get; set; }
        //Term2_StartMonth int
        public int Term2_StartMonth { get; set; }
        //Term2_EndMonth  int
        public int Term2_EndMonth { get; set; }
        //Term2_Percent   decimal (18, 2)
        public decimal Term2_Percent { get; set; }
        //Term3_StartMonth int
        public int Term3_StartMonth { get; set; }
        //Term3_EndMonth  int
        public int Term3_EndMonth { get; set; }
        //Term3_Percent   decimal (18, 2)
        public decimal Term3_Percent { get; set; }
        //Term4_StartMonth int
        public int Term4_StartMonth { get; set; }
        //Term4_EndMonth  int
        public int Term4_EndMonth { get; set; }
        //Term4_Percent   decimal (18, 2)
        public decimal Term4_Percent { get; set; }
        //StatusListId int
        public int StatusListId { get; set; }
        public string StatusListName { get; set; }
        //RegionId    int
        public int RegionId { get; set; }
        //IsActive    bit
        public bool IsActive { get; set; }
        //CreatedBy   int
        public int CreatedBy { get; set; }
        //CreatedDate datetime
        public DateTime? CreatedDate { get; set; }
        //ModifiedBy  int
        public int ModifiedBy { get; set; }
        //ModifiedDate    datetime
        public DateTime? ModifiedDate { get; set; }

       
    }
}
