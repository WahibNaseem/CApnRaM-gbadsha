using System;
using JKViewModels.Common;

namespace JKViewModels.Contract
{
    public class ContractModel : BaseEntityModel
    {
        public int DistributionId { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        public int CustomerId { get; set; }
        public int SoldById { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public int ContractTypeListId { get; set; }
        public string ContractType { get; set; }
        public int ServiceTypeListId { get; set; }
        public string ServiceType { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ContractDescription { get; set; }
        public int ContractTermMonth { get; set; }
        public decimal? Amount { get; set; }
        public decimal? SquareFootage { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AddressModel Address { get; set; }
    }
}
