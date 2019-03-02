using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class ACHBankViewModel
    {
        public int ACHBankId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> RoutingNumber { get; set; }
        public Nullable<decimal> AccountNumber { get; set; }
        public string Descrption { get; set; }
        public string RemittanceNotes { get; set; }
    }
}
