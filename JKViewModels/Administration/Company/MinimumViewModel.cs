using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Administration.Company
{
    public class MinimumViewModel
    {
        public int Id { get; set; }
        public int FeeId { get; set; }
        public string FeeName { get; set; }
        public int FranchiseeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }

        public int StatusId = 1;
        public string Status = "0";
        public int CreatedBy;
        public int ModifiedBy;
        public DateTime CreateDate;
        public DateTime ModifiedDate;

    }
}
