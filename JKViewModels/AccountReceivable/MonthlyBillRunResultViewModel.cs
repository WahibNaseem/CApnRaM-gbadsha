using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class MonthlyBillRunResultViewModel
    {
        public int BatchId { get; set; }
        public long BatchNumber { get; set; }
        public int RegionId { get; set; } 
        public string RegionName { get; set; }
        public int MasterTrxTypeListId { get; set; }
        public string MasterTrxTypeListName { get; set; }
        public int NumberOfCount { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
    }

    
}
