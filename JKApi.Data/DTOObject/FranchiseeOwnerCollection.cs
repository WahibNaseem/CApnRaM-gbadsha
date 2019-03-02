using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
    public class FranchiseeOwnerCollection
    {
        public FranchiseeOwnerCollection()
        {
            OwnerInfo = new Contact();
            OwnerInfoAddress = new Address();
            OwnerInfoPhone = new Phone();
            OwnerInfoEmail = new Email();

        }
        public Contact OwnerInfo { get; set; }
        public Address OwnerInfoAddress { get; set; }
        public Phone OwnerInfoPhone { get; set; }
        public Email OwnerInfoEmail { get; set; }
    }

    public class FranchiseeOwnerCollectionTemp
    {
        public FranchiseeOwnerCollectionTemp()
        {
            OwnerInfo_Temp = new Contact_Temp();
            OwnerInfoAddress_Temp = new Address_Temp();
            OwnerInfoPhone_Temp = new Phone_Temp();
            OwnerInfoEmail_Temp = new Email_Temp();

        }
        public Contact_Temp OwnerInfo_Temp { get; set; }
        public Address_Temp OwnerInfoAddress_Temp { get; set; }
        public Phone_Temp OwnerInfoPhone_Temp { get; set; }
        public Email_Temp OwnerInfoEmail_Temp { get; set; }
    }
}
