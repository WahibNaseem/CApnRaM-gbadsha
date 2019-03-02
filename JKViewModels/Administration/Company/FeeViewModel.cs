using System;

namespace JKViewModels.Administration.Company
{
    public class FeeViewModel
    {

        public int StatusId = 1;
        public string Status = "0";
        public int CreatedBy;
        public int ModifiedBy;
        public DateTime CreateDate;
        public DateTime ModifiedDate;

        public object _object { get; set; }

        public int Id { get; set; }
        public int RelatedToId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Rate { get; set; }
        public int RateType { get; set; }
        public int FeeType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public MinimumViewModel min { get; set; }

        public Double StartRange = 0.00;
        public Double EndRange = 999999.99;

        //-- this should be set to a comma-delimited string of all the fee related to the contract detail
        //-- it is used to save any changes made...
        public string FeeIdList { get; set; }
        

    }
}
