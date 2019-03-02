using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.JKControl;

namespace JKViewModels.Company
{
    public class BankDepositViewModel
    {
        public int RegionId { get; set; }
        public int TypeListId { get; set; }
        public int ClassId { get; set; }

        public int BankId { get; set; }
        public int DepositTypeId { get; set; }
        public DateTime DepositDate { get; set; }

        public string Description { get; set; }
        public string ReferenceNo { get; set; }
        public decimal Amount { get; set; }

        public int? CreatedBy;
        public DateTime? CreatedDate;
        public int? ModifiedBy;
        public DateTime? ModifiedDate;
    }
}
