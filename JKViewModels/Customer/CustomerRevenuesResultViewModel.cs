using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerRevenuesResultViewModel
    {
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string RegionName { get; set; }
        public string RangeName { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalTax { get; set; }
        public int RegionId { get; set; }

        public decimal TotalBill { get; set; }
        public decimal ContractAmount { get; set; }
    }
}
