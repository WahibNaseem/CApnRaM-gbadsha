using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeFullfillmentViewModel
    {
        public int FranchiseeFullfillmentId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<System.DateTime> BackgroundCheckDate { get; set; }
        public Nullable<System.DateTime> BusinessProtectionDate { get; set; }
        public Nullable<System.DateTime> TrainingDate { get; set; }
        public Nullable<System.DateTime> BusinessLicensedate { get; set; }
        public Nullable<System.DateTime> EquipmentAndSupplyDate { get; set; }
    }

    public class ParentFranchiseeTabModel
    {
        public int FranchiseeId { get; set; }
        public int NewFranchiseeId { get; set; }
        public FranchiseeTab1 FranchiseeTab1 { get; set; }
        public FranchiseeTab2 FranchiseeTab2 { get; set; }
        public FranchiseeTab4 FranchiseeTab4 { get; set; }
        public FranchiseeTab5 FranchiseeTab5 { get; set; }

        public int ParentId { get; set; }
         
    }

    public class FranchiseeTab1
    {         
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string PostalCode { get; set; }
        public int PhoneId { get; set; }
        public string Phone1 { get; set; }
        public int EmailId { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }        
    }

    public class FranchiseeTab2
    {
        public  int FranchiseeId { get; set; }
        public  int ContactId { get; set; }
        public  string Name { get; set; }
        public  int AddressId { get; set; }
        public  string Address1 { get; set; }
        public  string Address2 { get; set; }
        public  string City { get; set; }
        public  string StateId { get; set; }
        public  string PostalCode { get; set; }
        public  int PhoneId { get; set; }
        public  string Phone1 { get; set; }
        public  string PhoneExt { get; set; }
        public  string Cell { get; set; }
        public  int EmailId { get; set; }
        public string EmailAddress { get; set; }        
    }

    public class FranchiseeTab4
    {
        public int FranchiseeId { get; set; }
        public int BillSettingsId { get; set; }
        public int IdentificationId { get; set; }
        public int IdentifierTypeListId { get; set; }
        public string IdentifierNumer { get; set; }
        public bool Incorporated { get; set; }
        public string PayeeName { get; set; }
        public string Name1099 { get; set; }
        public bool Print1099 { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string PostalCode { get; set; }
        public int ACHBankId { get; set; }
        public string BankName { get; set; }
        public decimal Routing { get; set; }
        public decimal Account { get; set; }
        public string Description { get; set; }
        public string RemittanceNotes { get; set; }
    }

    public class FranchiseeTab5
    {
        public int BillSettingsId { get; set; }
        public bool Chargeback { get; set; }
        public bool BBPAdministrationFee { get; set; }
        public bool AccountRebate { get; set; }
        public bool GenerateReport { get; set; }
    }
}
