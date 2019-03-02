using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FindersFeeScheduleViewModel
    {
        public int FindersFeeScheduleId { get; set; } //(int, not null)
        public int FindersFeeTypeListId { get; set; } //(int, null)
        public decimal Factor { get; set; } //(decimal(8,2), null)
        public decimal DownPayPercentage { get; set; } //(decimal(8,2), null)
        public decimal MonthlyPercentage { get; set; } //(decimal(8,2), null)
        public decimal StartAmount { get; set; } //(decimal(12,2), null)
        public decimal EndAmount { get; set; } //(decimal(12,2), null)
        public int NumOfPayments { get; set; } //(int, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(datetime, null)
        public int? ModifiedBy { get; set; } //(int, null)
        public DateTime? ModifiedDate { get; set; } //(datetime, null)
    }
}
