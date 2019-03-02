using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Management
{
    public class ObligationReportViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string Status { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",ApplyFormatInEditMode = true)]
        public DateTime OfferedDate { get; set; }
        public string CustomerName { get; set; }
        public string DaysKept { get; set; }
        public string SalesRep { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal OriginalContractAmount { get; set; }
        public decimal LegalBalanceAmount { get; set; }
        public decimal OtherBalanceAmount { get; set; }
        public string PlanType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSign { get; set; }
        public decimal PlanAmount { get; set; }
        public decimal IBAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LegalObligationStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LegalObligationEndDate { get; set; }
    }
}
