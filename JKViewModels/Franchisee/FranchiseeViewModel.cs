using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string Name { get; set; }
        public Nullable<int> IdentifierTypeList { get; set; }
        public int StatusListId { get; set; }
        public int RegionId { get; set; }
        public int ParentId { get; set; }
    }
    
    public class FranchiseeDetailViewModel
    {
        public int FranchiseeId { get; set; }
        public string RegionName { get; set; }
        public string FranchiseeNo { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Ext { get; set; }
        public string Fax { get; set; }
        public string Cell { get; set; }
        public string EmailAddress { get; set; }
        public string ContactName { get; set; }
        public string ContactType { get; set; }
        public string CPhone { get; set; }
        public string CExt { get; set; }
        public string CFax { get; set; }
        public string CCell { get; set; }
        public string CEmailAddress { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> BBPAdministrationFee { get; set; }
        public Nullable<bool> Chargeback { get; set; }
        public Nullable<bool> Incorporated { get; set; }
        public Nullable<bool> Print1099 { get; set; }
        public Nullable<int> Term { get; set; }
        public Nullable<System.DateTime> DateSign { get; set; }
        public string StatusName { get; set; }
        public string PlanType { get; set; }
        public List<PendingDashboardDataModel> MessageData { get; set; }
        public int USERID { get; set; }
        public Nullable<System.DateTime> LastRenewedContractDate { get; set; }
    }

    public class FranchiseeListViewModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string Name { get; set; }
        public string Distribution { get; set; }
        public int AddressId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ContactTypeListId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public Nullable<int> StateListId { get; set; }
        public string PostalCode { get; set; }
        public Nullable<int> CountyTaxAuthorityListId { get; set; }
        public Nullable<bool> AddIsActive { get; set; }
        public int PhoneId { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public Nullable<int> CountryCodeListId { get; set; }
        public Nullable<bool> phIsActive { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public Nullable<int> ReasonListId { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string StatusNotes { get; set; }
        public Nullable<System.DateTime> ResumeDate { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<bool> stsIsActive { get; set; }
        public string EmailAddress { get; set; }
        public string ContactType { get; set; }
        public string ContactName { get; set; }
        public string Amount { get; set; }
        public decimal DistributionAmount { get; set; }
        public string RegionName { get; set; }
        public string StatusName { get; set; }
        public string StateName { get; set; }
        public int IsTemp { get; set; }
        public int RowNo { get; set; }
    }

    //public class FranchiseeViewModelList
    //{
    //    public int FranchiseeId { get; set; }       
    //    public string FranchiseeNo { get; set; }
    //    public string Name { get; set; }
    //    public string Distribution { get; set; }
    //    public int AddressId { get; set; }
    //    public Nullable<int> ClassId { get; set; }
    //    public Nullable<int> TypeListId { get; set; }
    //    public Nullable<int> ContactTypeListId { get; set; }
    //    public string Address1 { get; set; }
    //    public string Address2 { get; set; }
    //    public string City { get; set; }
    //    public Nullable<int> StateListId { get; set; }
    //    public string PostalCode { get; set; }
    //    public Nullable<int> CountyTaxAuthorityListId { get; set; }
    //    public Nullable<bool> AddIsActive { get; set; }        
    //    public int PhoneId { get; set; }
    //    public string Phone { get; set; }
    //    public string PhoneExt { get; set; }    
    //    public string Cell { get; set; }
    //    public string Fax { get; set; }
    //    public Nullable<int> CountryCodeListId { get; set; }
    //    public Nullable<bool> phIsActive { get; set; }
    //    public int StatusId { get; set; }       
    //    public Nullable<int> StatusListId { get; set; }
    //    public Nullable<int> ReasonListId { get; set; }
    //    public Nullable<System.DateTime> StatusDate { get; set; }
    //    public string StatusNotes { get; set; }
    //    public Nullable<System.DateTime> ResumeDate { get; set; }
    //    public Nullable<System.DateTime> LastServiceDate { get; set; }
    //    public Nullable<bool> stsIsActive { get; set; }
    //    public string EmailAddress { get; set; }
    //    public string ContactType { get; set; }
    //    public string ContactName { get; set; }
    //    public string Amount { get; set; }
    //}

    public class FranchiseeDistributionDetailsModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseName { get; set; }
        public decimal TotalDistribution { get; set; }
        public List<FranchiseeDistributionModel> FranchiseeDistributionList { get; set; }
    }
    public class FranchiseeDistributionModel
    {
        public int ID { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string ServiceType { get; set; }
        public decimal DistributionAmount { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
    }

    public class FranchiseeCustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
    }

    public class FranchiseeFeeConfigurationModel
    {
        public int FeeConfigurationId { get; set; }
        public int ClassId { get; set; }
        public int TypeListId { get; set; }
        public int FeeId { get; set; }
        public int MinimumAmount { get; set; }
        public int StatusId { get; set; }
        public int StatusListId { get; set; }
        public int IsActive { get; set; }
        public int RegionId { get; set; }
        public int CreatedBy { get; set; }
        public int CreatedDate { get; set; }
        public int IsDelete { get; set; }
        public int FeeConfigurationImpId { get; set; }
    }
    public class FranchiseeBasicInfo
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeContact { get; set; }
        public string FranchiseePhone{ get; set; }
    }

}
