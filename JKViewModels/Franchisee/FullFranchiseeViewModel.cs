using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.Customer;
using JKViewModels.Common;
using JKViewModels.Franchise;

namespace JKViewModels.Franchisee
{
    public class FullFranchiseeViewModel
    {

        public FullFranchiseeViewModel()
        {
            BusinessInfo = new FranchiseeViewModel();
            BusinessInfoAddress = new AddressViewModel();
            BusinessInfoPhone = new PhoneViewModel();
            BusinessInfoEmail = new EmailViewModel();

            ContactInfo = new ContactViewModel();
            ContactInfoAddress = new AddressViewModel();
            ContactInfoPhone = new PhoneViewModel();
            ContactInfoEmail = new EmailViewModel();

            PayeeInfo = new FranchiseeBillSettingViewModel();
            PayeeInfoAddress = new AddressViewModel();

            ACHBankInfo = new ACHBankViewModel();
            FullfillmentInfo = new FranchiseeFullfillmentViewModel();
            ContractInfo = new FranchiseeContractViewModel();
            PayeeIdentification = new IdentificationViewModel();


            FranchiseeOwner = new FranchiseeOwner();
            FranchiseeOwners = new List<FranchiseeOwner>();
            BillSettings = new FranchiseeBillSettingViewModel();

            FranchiseeFeeListFeeRateTypeListCollectionViewModel = new FullFranchiseeFeeListFeeRateTypeListCollectionViewModel();

            Status = new StatusViewModel();

            FranchiseeFeeConfigurationInfo = new FranchiseeFeeConfigurationViewModel();
        }

        //BusinessInfo
        public FranchiseeViewModel BusinessInfo { get; set; }
        public AddressViewModel BusinessInfoAddress { get; set; }
        public PhoneViewModel BusinessInfoPhone { get; set; }
        public EmailViewModel BusinessInfoEmail { get; set; }

        public StatusViewModel BusinessInfoStatus { get; set; }

        //ContactInfo
        public ContactViewModel ContactInfo { get; set; }
        public AddressViewModel ContactInfoAddress { get; set; }
        public PhoneViewModel ContactInfoPhone { get; set; }
        public EmailViewModel ContactInfoEmail { get; set; }




        //PayeeInfo
        public FranchiseeBillSettingViewModel PayeeInfo { get; set; }
        public AddressViewModel PayeeInfoAddress { get; set; }

        public ACHBankViewModel ACHBankInfo { get; set; }

        public FranchiseeFullfillmentViewModel FullfillmentInfo { get; set; }

        public FranchiseeContractViewModel ContractInfo { get; set; }

       


        public IdentificationViewModel PayeeIdentification { get; set; }



        //OwnerInfo
        public FranchiseeOwner FranchiseeOwner { get; set; }

        public List<FranchiseeOwner> FranchiseeOwners { get; set; }

        public FranchiseOwnersList FranchiseOwnerList { get; set; }

        public List<FranchiseOwnersList> FranchiseOwnersList { get; set; }

        public FranchiseeBillSettingViewModel BillSettings { get; set; }

        public FullFranchiseeFeeListFeeRateTypeListCollectionViewModel FranchiseeFeeListFeeRateTypeListCollectionViewModel { get; set; }
        public List<FeeFranchiseeFeeRateTypeListCollectionViewModel> FeeFranchiseeFeeRateTypeListCollectionViewModel { get; set; }

        public StatusViewModel Status { get; set; }

        public int? ButtonType { get; set; }

        public int? PayeeIdentificationTypeId { get; set; }
        public int? FranchiseeIdByOwner { get; set; }

        public FranchiseeFeeConfigurationViewModel FranchiseeFeeConfigurationInfo { get; set; }


    }

    public class FranchiseeOwner
    {
        public FranchiseeOwner()
        {
            OwnerInfo = new ContactViewModel();
            OwnerInfoAddress = new AddressViewModel();
            OwnerInfoPhone = new PhoneViewModel();
            OwnerInfoEmail = new EmailViewModel();
            OwnerIdentification = new IdentificationViewModel();

        }
        public ContactViewModel OwnerInfo { get; set; }
        public AddressViewModel OwnerInfoAddress { get; set; }
        public PhoneViewModel OwnerInfoPhone { get; set; }
        public EmailViewModel OwnerInfoEmail { get; set; }

        public IdentificationViewModel OwnerIdentification { get; set; }
    }

    public class FranchiseOwnersList
    {
        public int FranchiseeOwnerListId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Nullable<int> StateListId { get; set; }
        public string PostalCode { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
 