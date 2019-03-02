using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class OwnerViewModel
    {
        public int Id { get; set; }
        public int FranchiseId { get; set; }
        public string EIN { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string AddressCont { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Ext { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int TaxAuthorityId { get; set; }
        public int PayeeSameAsOwner { get; set; }
        public int Print1099 { get; set; }
        public int InCorporated { get; set; }

        private List<OwnerViewModel> _owners { get; set; }
    }
}
