using System;

namespace JKViewModels.Administration.Company
{
    public class CompanyViewModel
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public int Corporate { get; set; }
        public int Test { get; set; }
        public string DisplayName { get; set; }
        public string ReportName { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int RemitSameAsMain { get; set; }

        public RemitToViewModel RemitToLocation { get; set; }

        public int StatusId = 1;
        public string Status = "0";
        public int CreatedBy;
        public int ModifiedBy;
        public DateTime CreateDate;
        public DateTime ModifiedDate;

        public static int coRegionCode = 2;
        public static int coCustomerIndex = 3;
        public static int coFranchiseeIndex = 4;
        public static int coChargebackInvoices = 6;
        public static int coRoyaltyonClientSupplies = 7;
        public static int coLeasesTaxUpfront = 8;
        public static int coCPIIncrease = 9;
        public static int coCustomerContractDefaultTerm = 12;
        public static int coMinimumSalesAmount = 13;
        public static int coFinderFeeFactor = 14;
        public static int coOfficeCode = 19;
        public static int coFederalIDNumber = 20;
        public static int coControlNumber = 21;
        public static int coInvoiceNumber = 23;
        public static int coRegionId = 24;
        public static int coInitialOne_TimeClean = 25;
        public static int coCustomerSupplyMarkUp = 26;
        public static int coInvoiceMessage = 27;
        public static int coCancelFFFee = 28;
        public static int coTroubleTransferFee = 29;
        public static int coFromEmailAddress = 30;
        public static int coCCEmailAddress = 31;
        public static int coBCCEmailAddress = 32;
    }
}
