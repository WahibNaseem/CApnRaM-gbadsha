using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMFranchiseContractTypeListViewModel
    {
        public int FranchiseeContractTypeListId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? BusinessAmount { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? Interest { get; set; }
        public int? NoOfPayments { get; set; }
        public int? DaysToFullfill { get; set; }
    }
}
