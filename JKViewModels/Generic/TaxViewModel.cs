using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Generic
{
    public class TaxViewModel
    {
        private object _object { get; set; }

        public int Id { get; set; }
        public int TrxId { get; set; }
        public int TaxAuthoryId { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public int UserId { get; set; }

        //public int RelatedToId;
        //public int TaxId;
        // public double ContractRate = 0;
        //public double SupplyRate = 0;
        //public double LeaseRate = 0;

        //-- this should be set to a comma-delimited string of all the fee related to the contract detail
        //-- it is used to save any changes made...
        public string TaxList { get; set; }

        private string _stmtload { get; set; }
        private string _stmtdelete { get; set; }
        private string _stmtsave { get; set; }

    }
}
