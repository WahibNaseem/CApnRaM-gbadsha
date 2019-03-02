using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.CRM
{
    public class CRMContactViewModel
    {
        public int CRM_ContactId { get; set; }
        public int? CRM_AccountCustomerDetailId { get; set; }
        public int? CRM_AccountFranchiseDetailId { get; set; }
        public int? ContactTypeId { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public int? CRM_FvPresentationId { get; set; }
        public int? CRM_PdAppointmentId { get; set; }
        public int? CRM_BiddingId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
    public class ContactType
    {
        public int ContactTypeId { get; set; }
        public string Name { get; set; }
    }
    

    public class CRMNewContactViewModel
    {
        public int? ContactID { get; set; }

        [Required(ErrorMessage = "Contact Type is required."), Display(Name = "Contact Type")]
        public int ContactTypeId { get; set; }

        [Required(ErrorMessage = "Full Name is required."), Display(Name = "Full Name"), StringLength(100, ErrorMessage = "Full Name cannot be more than 100 characters.")]
        public string FullName { get; set; }

        [Display(Name = "Company"), StringLength(100, ErrorMessage = "Company cannot be more than 100 characters.")]
        public string Company { get; set; }

        [Display(Name = "Job Title"), StringLength(80, ErrorMessage = "Job Title cannot be more than 60 characters.")]
        public string Jobtitle { get; set; }

        [Display(Name = "File As")]
        public string FileAs { get; set; }

        [Required(ErrorMessage = "Email Id is required."), Display(Name = "Email Id"), EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Display As"), StringLength(100, ErrorMessage = "Display As cannot be more than 100 characters.")]
        public string DisplayAs { get; set; }

        [Display(Name = "Web Page Address"), StringLength(200, ErrorMessage = "Web Page Address cannot be more than 200 characters.")]
        public string WebPageAddress { get; set; }

        [Display(Name = "IM Address"), StringLength(200, ErrorMessage = "IM Address cannot be more than 200 characters.")]
        public string IMAddress { get; set; }

        [Display(Name = "Business Phone"), Phone(ErrorMessage = "Invalid Phone Number."), Required(ErrorMessage = "Business Phone is required.")]
        public string BusinessPhone { get; set; }

        [Display(Name = "Home Phone"), DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number.")]
        public string HomePhone { get; set; }

        [Display(Name = "Business Fax Phone"), DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number.")]
        public string BusinessFaxPhone { get; set; }

        [Display(Name = "Mobile Phone"), Phone(ErrorMessage = "Invalid Phone Number.")]
        public string MobilePhone { get; set; }

        [Display(Name = "Business Address"), StringLength(300, ErrorMessage = "Business Address cannot be more than 300 characters.")]
        public string BusinessAddress { get; set; }

        public bool IsMailingAddress { get; set; }
        public static IEnumerable<ContactType> ContactTypes = new List<ContactType>{
            new ContactType {
            ContactTypeId = 1,
            Name = ""
            },
            new ContactType {
            ContactTypeId = 2,
            Name = ""
            }};
    }

    public class CRMContactTypeViewModel
    {
        public int ContactTypeId { get; set; }
        public string ContactTypeName { get; set; }
        public string ContactTypeTagName { get; set; }
    }
}
