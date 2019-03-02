using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
    public class CustomerCollection
    {

        public CustomerCollection()
        {
            Customer = new Customer();
            CurrentAddress = new Address();
            PermanentAddress = new Address(); 
            MainContact = new Contact();
            MainAddress = new Address();
            MainPhone = new Phone();
            MainEmail = new Email();
            BillingPhone = new Phone();
            BillingEmail = new Email();
            BillingContact = new Contact();
            BillingAddress = new Address();
            ContactInformation = new Contact();
            ContactInformationAddress = new Address();
            ContactInformationPhone = new Phone();
            ContactInformationEmail = new Email();
            BillingInformation = new Contact();
            BillingInformationAddress = new Address();
            BillingInformationPhone = new Phone();
            BillingInformationEmail = new Email();
            BillSetting = new BillSetting();
            Contract = new Contract();
            ContractDetail = new ContractDetail();
            AccountTypeList = new AccountTypeList();
            StateList = new StateList();

            MainPhone2 = new Phone();
            BillingContactInformation2 = new Contact();
        }
        public Customer Customer { get; set; }
        public Address CurrentAddress { get; set; }
        public Address PermanentAddress { get; set; }


         
        public List<Franchisee> Franchisees { get; set; }
        public List<Franchisee> _custfranchisees { get; set; } 



        public Contact MainContact { get; set; }
        public Address MainAddress { get; set; }
        public Phone MainPhone { get; set; }       
        public Email MainEmail { get; set; }
        public Contact BillingContact { get; set; }
        public Address BillingAddress { get; set; }
        public Phone BillingPhone { get; set; }
        public Email BillingEmail { get; set; }
        public Contact ContactInformation { get; set; }
        public Address ContactInformationAddress { get; set; }
        public Phone ContactInformationPhone { get; set; }
        public Email ContactInformationEmail { get; set; }
        public Contact BillingInformation { get; set; }
        public Address BillingInformationAddress { get; set; }
        public Phone BillingInformationPhone { get; set; }
        public Email BillingInformationEmail { get; set; }

        public BillSetting BillSetting { get; set; }

        // Customer Contract

        public Contract Contract { get; set; }

        public ContractDetail ContractDetail { get; set; }


        public AccountTypeList AccountTypeList { get; set; }

        public StateList StateList { get; set; }
        public int? ButtonType { get; set; }

        public Phone MainPhone2 { get; set; }
        public Contact BillingContactInformation2 { get; set; }
    }
}
