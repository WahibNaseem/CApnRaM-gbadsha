using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Generic
{
    public class FeeViewModel
    {
        public int _contractdetailid {get; set;}

        public int Id {get; set;}
        public int recurringTrxFee {get; set;}
        public int ParentId {get; set;}
        public int FeeId {get; set;}
        public string FeeName {get; set;}
        public decimal Amount {get; set;}
        public int AmountType {get; set;}
        public int FeeType {get; set;}

        //-- this should be set to a comma-delimited string of all the fee related to the contract detail
        //-- it is used to save any changes made...
        public string FeeList {get; set;}
    }
}
