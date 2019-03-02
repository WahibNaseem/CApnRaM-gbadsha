using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
   public class FranchiseeOwnerListModel
    {
        public int FranchiseeOwnerId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> EmailId { get; set; }
        public Nullable<int> PhoneId { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<int> IdentificationId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
