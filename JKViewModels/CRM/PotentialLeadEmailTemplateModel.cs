using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class PotentialLeadEmailTemplateModel
    {
        public string UserName { get; set; }
        public string LeadNo { get; set; }
        public string CompanyName { get; set; }
        public string ContactPhone { get; set; }
        public string AccountType { get; set; }
        public string BudgetAmount { get; set; }
        public string Schedule { get; set; }
        public string Purpose { get; set; }
        public string Note { get; set; }
    }
}
