using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeBillingPayInfoViewModel
    {
        public int BillingPayId { get; set; }
        public int InvoiceId { get; set; }
        public int ChargebackId { get; set; }
        public decimal ChargebackAmount { get; set; }
        public decimal ApplyAmount { get; set; }
        public int RegionId { get; set; }
        public List<FranchiseeFeeViewModel> lstFranchiseeFee { get; set; }

    }
}
