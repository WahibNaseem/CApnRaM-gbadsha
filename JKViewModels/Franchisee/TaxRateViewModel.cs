using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class TaxRateViewModel
    {
        public int TaxRateId { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<decimal> ContractTaxRate { get; set; }
        public Nullable<decimal> LeaseTaxRate { get; set; }
        public Nullable<decimal> SupplyTaxRate { get; set; }
        public Nullable<decimal> taxSales { get; set; }
        public Nullable<decimal> taxUse { get; set; }
        public Nullable<decimal> stateSalesTax { get; set; }
        public Nullable<decimal> stateUseTax { get; set; }
        public Nullable<decimal> citySalesTax { get; set; }
        public Nullable<decimal> cityUseTax { get; set; }
        public Nullable<decimal> countySalesTax { get; set; }
        public Nullable<decimal> countyUseTax { get; set; }
        public Nullable<decimal> districtSalesTax { get; set; }
        public Nullable<decimal> districtUseTax { get; set; }
        public Nullable<int> classId { get; set; }
        public Nullable<int> TypeListId { get; set; }
    }
}
