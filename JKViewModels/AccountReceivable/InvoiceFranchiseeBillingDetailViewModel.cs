using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class InvoiceFranchiseeBillingDetailViewModel
    {
        public int BillingPayId { get; set; } 
        public int FranchiseeId { get; set; } 
        public string FranchiseeNo { get; set; } 
        public string FranchiseeName { get; set; }
        public string Description { get; set; }
        public decimal? ContractAmount { get; set; }
        public DateTime? ContractDate { get; set; } 
    }
}
