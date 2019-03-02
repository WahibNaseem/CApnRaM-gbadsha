using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JKViewModels.Administration.Company
{
   
    
    public partial class TransactionNumberConfigViewModel
    {
        public int TransactionNumberConfigId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> RegionNumber { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public Nullable<long> LastNumber { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
       
        public Nullable<int> ModifiedBy { get; set; }
       
    }
}
