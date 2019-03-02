using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class FranchiseeViewModel
    {

        #region constants

        public static int StatusActive = 1;
        public static int StatusPending = 2;
        public static int StatusCease_to_do_Business = 3;
        public static int StatusMerged = 4;
        public static int StatusNon_Renewal = 5;
        public static int StatusRe_Purchased = 6;
        public static int StatusTerminated = 7;
        public static int StatusTransferred = 8;
        public static int StatusImportedInactive = 9;

        public static int SearchByFranchiseeName = 1;
        public static int SearchByFranchiseeNumber = 2;
        public static int SearchByLeaseNumber = 3;

        #endregion

        public int Id { get; set; }
        public string EIN { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressCont { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Ext { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Name1099 { get; set; }
        public DateTime SignDate { get; set; }
        public int BBPAdministrationFee { get; set; }
        public int Term { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ChargeBack { get; set; }
        public int ChargeBackBPPAdmin { get; set; }
        public int GenerateReport { get; set; }
        public string PlanType { get; set; }
        public decimal PlanAmount { get; set; }
        public decimal DownPayment { get; set; }
        public int NoOfPayments { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal InitialBillingAmount { get; set; }
        public int InCorporated { get; set; }
        public int Print1099 { get; set; }
        public int BPPAdmin { get; set; }
        public int TaxAuthorityId { get; set; }
        public int PayeeSameAs { get; set; }
        public int FinderFeeTypeId { get; set; }
        public decimal Interest { get; set; }
        public string BusinessLicense { get; set; }
        public int AccountRebate { get; set; }

        public decimal FranDetailResponsible { get; set; }
        public string PayeeName { get; set; }
        public string PayeeAddress { get; set; }
        public string PayeeAddressCont { get; set; }
        public string PayeeCity { get; set; }
        public string PayeeState { get; set; }
        public string PayeePostalCode { get; set; }
        public int RateID { get; set; }
        public decimal Rate { get; set; }

        //public int custfranid;
        //public string custfranname;
        //public string custfrannumber;

        public DateTime LegalObligationStartDate { get; set; }
        public DateTime LegalObligationFulfilledDate { get; set; }
        public decimal LegalObligationDue { get; set; }

        public decimal CustomerResponsibility { get; set; }

        public DateTime StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public DateTime ResumeDate { get; set; }
        public int StatusReasonId { get; set; }
        public decimal OldRateAmount { get; set; }
        public decimal NewRateAmount { get; set; }

        private List<OwnerViewModel> _owners { get; set; }


    }
}
