using System;
using System.Collections.Generic;
using JKViewModels.Common;


namespace JKViewModels.Job
{
    public class JobModel : BaseEntityModel
    {
        public int JobId { get; set; }
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }
        public int DistributionId { get; set; }
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public int SoldById { get; set; }
        public int ContractTypeListId { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public string ContractType { get; set; }
        public string AccountType { get; set; }
        public string ServiceType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ContractDescription { get; set; }
        public int ContractTermMonth { get; set; }
        public decimal Amount { get; set; }
        public decimal SquareFootage { get; set; }
        public int CleanTimes { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public int AssignedTeamId { get; set; }
        public int AssignedEmployeeId { get; set; }
        public string AssignedTemplate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AddressModel Address { get; set; }
    }

    public class JobDTOModel 
    {
        public int JobId { get; set; }
        public int DistributionId { get; set; }
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        public int CustomerId { get; set; }
        public int ContractTypeListId { get; set; }
        public int AccountTypeListId { get; set; }
        public int ServiceTypeListId { get; set; }
        public int SoldById { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryContact { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactPhoneExt { get; set; }
        public string ContractType { get; set; }
        public string AccountType { get; set; }
        public string ServiceType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ContractDescription { get; set; }
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

        public bool IsValid()
        {
            return DistributionId > 0 && ContractId > 0 && ContractDetailId > 0 && CustomerId > 0;
        }
    }

    public class JobListViewModel
    {
        //public int EmpId { get; set; }

        //public string FirstName { get; set; }

        //public string LastName { get; set; }

        //public string AddressLine1 { get; set; }

        //public int UserId { get; set; }

        //public string UserName { get; set; }

        //public string Email { get; set; }

        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? OrderStateId { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string OrderCode { get; set; }

        public string OrderTitle { get; set; }

        public string Description { get; set; }

        public Double? ServiceFee { get; set; }

        public string Instructions { get; set; }

        public string CustomerName { get; set; }

        public string OrderStateName { get; set; }

        public string CustomerAddress { get; set; }

    }

    public class ViewJobListModel : PaggingModel
    {
        public ViewJobListModel()
        {
            JobList = new List<JobListViewModel>();
        }

        public string SearchText { get; set; }

        public int ActiveStatus { get; set; }

        public List<JobListViewModel> JobList { get; set; }
    }
}
