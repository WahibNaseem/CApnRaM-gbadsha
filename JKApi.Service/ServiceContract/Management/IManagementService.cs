using JKApi.Data.DAL;
using JKApi.Data.DTOObject;
using JKViewModels;
using JKViewModels.AccountsPayable;
using JKViewModels.Commission;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.ServiceContract.Management
{
    public interface IManagementService
    {
        IEnumerable<CorporateDuesSearch> GetMonthListItem();
        IEnumerable<CorporateDuesSearch> GetYearListItem();
        List<CorporateDuesListItem> getCorporateDuesList(string month, string year);
        List<DeductionsListItem> getDeductionsList(string month, string year);
        List<DroListItem> getDroList(string month, string year);
        List<StarecapListItem> getstarecapList(string month, string year);
        List<RevenueListItem> getrevenueList(string month, string year);
        IEnumerable<JKApi.Data.DAL.Franchisee> GetLeaseList(string id);
        List<portal_spGet_F_Information_Result> GetLeasePayment(int id);
        List<portal_spget_F_LeaseHistory_New_Result> GetLeaseList_All(int id);
        List<portal_spGet_R_RevenueList_Result> GetRevenueList_All(int billmonth,int billyear);
        List<portal_spGet_R_MonthlyTax_Result> GetMonthlyTax_All(int billmonth, int billyear, int userid);
        List<LeaseListViewModel> getrevenueList(int searchby, string searchvalue, string transactionstartdate, string transactionenddate);

        IEnumerable<portal_spget_F_LeaseHistory_all_Result> GetLeaseList_All(string id);
        IEnumerable<portal_spget_F_LeaseHistory_Specific_Result> GetLeaseList_Specific();
        List<LeaseListViewModel> getLeasePaymentsList(int searchby, string searchvalue, string transactionstartdate, string transactionenddate);
        IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownData(int flagId,int? regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetBillingBreakdownByContractChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetBillingBreakdownBySizeChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRevenueByMonthChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRevenueByYearChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRevenueWiseTopCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        List<CorporationDuesReportViewModel> GetCorporationDuesReportData(string RegionIds, int Periodid);
        List<NegativeDueReportViewModel> GetNegativeDueCollectedReportData(string RegionIds,string fromMonth, string fromYear, string toMonth, string toYear);
        IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownBySizeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetBillingAccountBreakdownByContractData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        List<Period> getPeriod(int PeriodId);
		List<GetPercentPaidByDateReport> GetPercentageDailyReportbyMonthly(string[] RegionIds, string StartDate, string EndDate);
        IEnumerable<DashboardModel> GetAccountTypeWiseChartData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetBillingAccountRevenueByAccountTypeData(string flagId, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRevenueAndPaymentChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRegionWiseYearRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRegionWiseMonthlyRevenueChartData(int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetTopCustomerRevenueChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetRevenueWiseTopCustomerChartDataDetails(string flag, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetNewAndCanceledCustomerChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetTopPaymentWiseCustomer(int? recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetGrossAndContractBillingRevenueChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetMonthlyDataForASelectedYearChartData(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByCustomerData(string flag, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByAccountTypeData(string flag, int monthToAdd,string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetMonthlyRevenueDetailsByAccountTypeAndCustomerData(string flag, int accountTypeListId, int monthToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetRegionWiseYearRevenueComparisonChartData(int yearToAdd, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<ChartDetailsViewModel> GetCancelVsNewCustomerDetail(string flag, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);


        //Commission     
        







        List<CompensationTypeViewModel> GetCompensationTypeListData(int regionId);
        List<PaymentScheduleTypeViewModel> GetPaymentScheduleTypeListData();
        List<CompensationTypeViewModel> DeleteCompensationTypeListData(int CompensationTypeListId, int regionId);
        CommissionScheduleListViewModel GetCommissionScheduleListData(int regionId, int periodId);
        List<SalesPersonCommSchViewModel> GetSalesPersonCommSchListData(int regionId);
        List<SalesPersonBonusCommissionScheduleViewModel> GetSalesPersonCommSchBonusListData(int regionId);
        
        List<CommissionPaymentScheduleViewModel> GetCommissionPaymentScheduleListData(int regionId);
        List<BonusViewModel> GetBonusListData(int regionId);
        bool DeleteCommissionScheduleListData(int CommissionScheduleId, int regionId);
        List<SalesPersonCommSchViewModel> DeleteSalesPersonCommSchListData(int SalesPersonCommSchId, int regionId);

        List<SalesPersonBonusCommissionScheduleViewModel> DeleteSalesPersonBonusCommSchListData(int SalesPersonBonusCommissionScheduleId, int regionId);
        SalesPersonBonusCommissionScheduleViewModel GetSalesPersonBonusCommissionScheduleData(int SalesPersonBonusCommissionScheduleId, int regionId);
        SalesPersonCommSchViewModel GetSalesPersonCommSchData(int SalePersonId, int regionId);
        bool CheckSalesPersonCommSchData(int SalesPersonCommSchId, int SalesPersonId, int ContractTypeId, int RegionId);
        bool CheckSalesPersonBonusCommSchData(int SalesPersonBonusCommissionScheduleId, int SalesPersonId, int ContractTypeId, int RegionId);
        SalesPersonCommSchViewModel GetSalesPersonCommSchData(int SalesPersonCommSchId);
        bool DeleteCommissionScheduleContractListData(int ContractId, int regionId);
        List<CommissionScheduleViewModel> DeleteCommissionPaymentScheduleListData(int CommissionPaymentScheduleId, int regionId);
        List<BonusViewModel> DeleteBonusListData(int BonusId, int regionId);
        CommissionCustomerDetailViewModel GetCommissionScheduleCustomerData(int customerId);
        List<UserViewModel> GetCommissionScheduleSalesPerssonList(int regionId);
        List<UserViewModel> GetCommSchSalesPerssonList(int regionId);
        CommissionScheduleViewModel GetCommissionScheduleData(int CommissionScheduleId);
        List<CommissionCompensationScheduleViewModel> GetCommissionCompensationScheduleDropdown(int regionId);
        List<CommissionAdditionalBonusScheduleViewModel> GetCommissionAdditionalBonusScheduleDropdown(int regionId);
        List<CurrentScheduledCommissionViewModel> GetCurrentScheduledCommissionData(int regionId); 
        List<CurrentScheduledCommissionViewModel> GetCurrentScheduledCommissionReviewData(int regionId,int periodId);
        ScheduledCommissionGenerateViewModel GetCurrentScheduledCommissionGenerateData(int regionId, int periodId);
        bool GenerateCurrentScheduledCommissionData(int regionId,int periodId);
        bool ImportCommissionScheduleList(int regionId);
        List<CompensationTypeViewModel> InsertCompensationTypeListData(CompensationTypeViewModel _inputData);
        bool InsertCommissionScheduleData(CommissionScheduleViewModel _inputData);
        List<SalesPersonCommSchViewModel> InsertSalesPersonCommSchData(SalesPersonCommSchViewModel _inputData);
        List<SalesPersonBonusCommissionScheduleViewModel> InsertSalesPersonBonusCommSchData(SalesPersonBonusCommissionScheduleViewModel _inputData);
        List<CommissionPaymentScheduleViewModel> InsertCommissionPaymentScheduleData(CommissionPaymentScheduleViewModel _inputData);
        List<BonusViewModel> InsertBonusData(BonusViewModel _inputData);

        List<CommissionPaymentScheduleViewModel> GetCommissionPaymentPlanData(int regionId);        
        List<CommissionCompensationScheduleViewModel> GetCommissionCompensationScheduleData(int regionId);
        List<CommissionCompensationScheduleViewModel> DeleteCommissionCompensationScheduleData(int CompensationScheduleId, int regionId);        
        List<CommissionCompensationScheduleViewModel> InsertCommissionCompensationScheduleData(CommissionCompensationScheduleViewModel _inputData);

        List<AdditionalBonusScheduleViewModel> GetCommissionAdditionalBonusScheduleData(int regionId);
        List<AdditionalBonusScheduleViewModel> InsertCommissionAdditionalBonusScheduleData(AdditionalBonusScheduleViewModel _inputData);
        List<AdditionalBonusScheduleViewModel> DeleteCommissionAdditionalBonusScheduleData(int AdditionalBonusScheduleId, int regionId);
        decimal GetAdditionalBonusAmount(decimal ContractAmount, int regionId);
        DashboardModel GetManagementDashboardData(string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<DashboardModel> GetTopTenRevenueByAccountTypeChartData(int recordNumber, string regionIds, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        List<ObligationReportViewModel> getObligationList(string RegionIds);
        List<NegativeDueListReportViewModel> GetNegativeDueReportData(string regionIds = "", DateTime? from = null, DateTime? to = null);

        List<CurrentScheduledCommissionViewModel> BindCommissionsEarnedReportData(int month, int year, int userId, int regionId);
        List<CurrentPaymentHistoryCommissionViewModel> BindPaymentHistoryReportData(int month, int year, int userId, int regionId);


    }
}
