using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMSignAgreementViewModel
    {
        public int CRM_SignAgreementId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public bool? SignFranchiseAgreement { get; set; }
        public bool? GuaranteesSigned { get; set; }
        public bool? RequiredDocument { get; set; }
        public bool? LegalBackGround { get; set; }
        public DateTime? DateSign { get; set; }
        public decimal? Term { get; set; }
        public DateTime? ExpDate { get; set; }
        public int? PlanType { get; set; }
        public decimal? PlanAmount { get; set; }
        public decimal? IBAmount { get; set; }
        public decimal? DownPayment { get; set; }
        public decimal? Interest { get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? NoOfPayments { get; set; }
        public decimal? CurrentPayment { get; set; }
        public DateTime? PaymentStartDate { get; set; }
        public decimal? TriggerAmount { get; set; }
        public DateTime? legalOblStart { get; set; }
        public DateTime? LegalOblEnd { get; set; }
        public string LegalOblDue { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
