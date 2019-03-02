using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
    public class FranchiseeCollection
    {
        public FranchiseeCollection()
        {
            BusinessInfo = new Franchisee();
            BusinessInfoAddress = new Address();
            BusinessInfoPhone = new Phone();
            BusinessInfoEmail = new Email();

            ContactInfo = new Contact();
            ContactInfoAddress = new Address();
            ContactInfoPhone = new Phone();
            ContactInfoEmail = new Email();

            PayeeInfo = new FranchiseeBillSetting();
            PayeeInfoAddress = new Address();

            ACHBankInfo = new ACHBank();
            FullfillmentInfo = new FranchiseeFullfillment();
            ContractInfo = new FranchiseeContract();
            PayeeIdentification = new Identification();

             
            BillSettings = new FranchiseeBillSetting();

        }
        //BusinessInfo
        public Franchisee BusinessInfo { get; set; }
        public Address BusinessInfoAddress { get; set; }
        public Phone BusinessInfoPhone { get; set; }
        public Email BusinessInfoEmail { get; set; }
        public Status BusinessInfoStatus { get; set; }

        //ContactInfo
        public Contact ContactInfo { get; set; }
        public Address ContactInfoAddress { get; set; }
        public Phone ContactInfoPhone { get; set; }
        public Email ContactInfoEmail { get; set; }




        //PayeeInfo
        public FranchiseeBillSetting PayeeInfo { get; set; }
        public Address PayeeInfoAddress { get; set; }

        public ACHBank ACHBankInfo { get; set; }

        public FranchiseeFullfillment FullfillmentInfo { get; set; }

        public FranchiseeContract ContractInfo { get; set; }

        public Identification PayeeIdentification { get; set; }



        //OwnerInfo 
        public FranchiseeBillSetting BillSettings { get; set; } 
    } 
}
