using JKViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContractViewModel
    {
        public int ContractId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> SoldById { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Nullable<int> ContractTypeListId { get; set; }
        public bool  Qualified { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        //public Nullable<int> ContractTermListId { get; set; }
        public Nullable<int> ContractTermMonth { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<int> Terms { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> AmountSubjectToFee { get; set; }
        public Nullable<decimal> InitialCleanAmount { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<int> ContractStatusReasonListId { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string ContractDescription { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> AgreementTypeListId { get; set; }
    }
}
