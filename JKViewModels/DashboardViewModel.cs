using JKViewModels.CRM;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    

        public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            dashboardModel = new DashboardModel();
            crmdashboardModel = new CRMDashboardModel();
            DashboardModelForBlock = new DashboardModel();
            CustomerServiceQuickActionModel = new CustomerServiceQuickActionViewModel();

        }
        public DashboardModel dashboardModel { get; set; }
        public CRMDashboardModel crmdashboardModel { get; set; }
        public DashboardModel DashboardModelForBlock { get; set; }
        public CustomerServiceQuickActionViewModel CustomerServiceQuickActionModel { get; set; }
    }

    public class DashboardModel
    {
        public DashboardModel()
        {
            lstQuickLinks = new List<DashboardQuickLinkModel>();
        }
        public decimal TotalRevenue { get; set; }
        public decimal TotalAccountReceivable { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalFranchisee { get; set; }
        public int TotalNewCustomers { get; set; }
        public int BillYear { get; set; }
        public int BillMonth { get; set; }
        public string RangeName { get; set; }
        public int TotalAccount { get; set; }
        public int AccountTypeListId { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal PaymentForCurrentRevenue { get; set; }
        public decimal PaymentOthersRevenue { get; set; }

        public string ColorCode { get; set; }
        public int Flag { get; set; }
        public string RegionName { get; set; }

        public int Start { get; set; }
        public int Duration { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int Ranking { get; set; }
        public int PeriodId { get; set; }
        public string RegionId { get; set; } //needed string value though the region id is int

        public decimal TotalGross { get; set; }
        public decimal TotalContractBilling { get; set; }
        public decimal TotalFranchiseeDeduction { get; set; }

        public decimal TotalFranchiseeRevenue { get; set; }

        public List<DashboardQuickLinkModel> lstQuickLinks { get; set; }

        public List<PendingDashboardDataModel> lstPendingData { get; set; }
    }
    public class RegionWiseRevenueComparison
    {
        public RegionWiseRevenueComparison()
        {
            regionWiseYearlyDetails = new List<DashboardModel>();
        }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string Year { get; set; }
        public string ColorCode { get; set; }
        public int TotalRegions { get; set; }
        public List<DashboardModel> regionWiseYearlyDetails { get; set; }
    }

    public class CustomertForChart
    {
        public CustomertForChart()
        {
            customertChartDetailsData = new List<CustomertChartDetailsData>();
        }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public List<CustomertChartDetailsData> customertChartDetailsData { get; set; }
    }
    public class CustomertChartDetailsData
    {
        public decimal TotalRevenue { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public int Ranking { get; set; }
        public int PeriodId { get; set; }
        public string RangeName { get; set; }
        public string ColorCode { get; set; }
        public decimal TotalCancel { get; set; }
        public decimal TotalNew { get; set; }

    }


    public class ChartDetailsViewModel
    {
        public decimal Total { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFee { get; set; }
        public decimal ExtendedPrice { get; set; }
        public int MasterTrxId { get; set; }
        public int MasterTrxTypeListId { get; set; }
        public string DetailDescription { get; set; }
        public int ClassId { get; set; }
        public int RegionId { get; set; }
        public int AmountTypeListId { get; set; }
        public int MasterTrxStatusId { get; set; }
        public int BillYear { get; set; }
        public int BillMonth { get; set; }
        public int InvoiceId { get; set; }
        public int Flag { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string RegionName { get; set; }
        public string MasterTrxType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public int FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string FranchiseeNo { get; set; }
        public string StatusName { get; set; }
        public DateTime TrxDate { get; set; }
        public string RangeName { get; set; }
        public string AccountTypeListName { get; set; }
        public int AccountTypeListId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }


    public class GrantChartViewModel
    {
        public GrantChartViewModel()
        {
            GrantChartDetailsViewModel = new List<GrantChartDetailsViewModel>();
        }
        public string category { get; set; }
        public List<GrantChartDetailsViewModel> GrantChartDetailsViewModel { get; set; }

    }

    public class GrantChartDetailsViewModel
    {
        public int start { get; set; }
        public int duration { get; set; }
        public string color { get; set; }
        public string task { get; set; }
    }


    public class CRMDashboardModel
    {
        public int? InitialCommunication { get; set; }
        public int? FvPresentation { get; set; }
        public int? Bidding { get; set; }
        public int? PdAppointment { get; set; }
        public int? Followup { get; set; }
        public int? Sold { get; set; }
        public int? Closing { get; set; }
        public Nullable<decimal> InitialCommunicationAmount { get; set; }
        public Nullable<decimal> FvPresentationAmount { get; set; }
        public Nullable<decimal> BiddingAmount { get; set; }
        public Nullable<decimal> PdAppointmentAmount { get; set; }
        public Nullable<decimal> FollowupAmount { get; set; }
        public Nullable<decimal> SoldAmount { get; set; }
        public Nullable<decimal> ClosingAmount { get; set; }
    }

    public class FranchiseeDashboardModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalAccountReceivable { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalFranchisee { get; set; }
        public int TotalNewCustomers { get; set; }
        public int BillYear { get; set; }
        public int BillMonth { get; set; }
        public string RangeName { get; set; }
        public int TotalAccount { get; set; }
        public int AccountTypeListId { get; set; }
        public decimal TotalPayment { get; set; }
        public string ColorCode { get; set; }
        public int Flag { get; set; }
        public string RegionName { get; set; }

        public int Start { get; set; }
        public int Duration { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public int Ranking { get; set; }
        public int PeriodId { get; set; }
        public string RegionId { get; set; }

        public decimal TotalGross { get; set; }
        public decimal TotalContractBilling { get; set; }
        public decimal TotalDeduction { get; set; }
        public int TotalActiveFranchisee { get; set; }
        public int TotalNewFranchisee { get; set; }

    }

}
