using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeBillSettingViewModel
    {
        public int FranchiseeBillSettingsId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public bool Chargeback { get; set; }
        public bool BBPAdministrationFee { get; set; }
        public bool AccountRebate { get; set; }
        public bool GenerateReport { get; set; }
        public Nullable<bool> Incorporated { get; set; }
        public Nullable<bool> InHouse { get; set; }
        public string OwnerSSN { get; set; }
        public Nullable<int> IdentfierTypeListId { get; set; }
        public string IdentifierId { get; set; }
        public string PayeeName { get; set; }
        public string C1099Name { get; set; }
        public bool Print1099 { get; set; }
        public Nullable<int> CountyTaxAuthorityListId { get; set; }
    }
}
