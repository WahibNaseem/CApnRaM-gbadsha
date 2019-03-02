using System;
using System.Collections.Generic;
using JKViewModels.Common;

namespace JKViewModels.CRM
{
   public class CRMPotentialCustomerViewModel : BaseEntityModel
    {
        public int CRM_AccountId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int assigneeId { get; set; }
        public int Regionid { get; set; }
        public int CRM_BiddingId { get; set; }
        public int CRM_CloseId { get; set; }
        public string StageStatusName { get; set; }
        public int StageStatus { get; set; }
        public string Firstname { get; set; }
        public string PhoneNumber { get; set; }
        public int CRM_AccountCustomerDetailId { get; set; }
        public string CompanyName { get; set; }   
        public string Title { get; set; }
        public string SalesVolume { get; set; }
        public string LineofBusiness { get; set; }
        public string SqFt { get; set; }
        public string SpokeWITH { get; set; }
        public int CRM_CallResultId { get; set; }
        public DateTime? CallBack { get; set; }
        public int AccountTypeListId { get; set; }
        public string AccountTypeName { get; set; }
        public int NumberOfLocations { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal ContractAmount { get; set; }
        public string LeftDay { get; set; }
    }

    public class CRMPotentialCustomerListViewModel : PaggingModel
    {
        public int UserId { get; set; }
        public int LoginUserId { get; set; }
        public CrmFilterChoice Choice { get; set; }
        public int Type { get; set; }
        public IList<CRMPotentialCustomerViewModel> PotentialCustomerList { get; set; }

        public CRMPotentialCustomerListViewModel()
        {
            PotentialCustomerList = new List<CRMPotentialCustomerViewModel>();
        }
    }
}
