using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.Administration.Company;
using JKApi.Service.Service.Administration.Company;
using JKApi.Service.Service.Company;
using JKViewModels.Company;

namespace JKApi.Service.ServiceContract.Company
{
    public interface ICompanyService
    {
        IQueryable<Data.DAL.Company> GetAll_Company3rdParty();
        Data.DAL.Company Get_Company3rdParty(int id);
        Data.DAL.Company Save_Company3rdParty(Data.DAL.Company company3rdparty);
        List<Company3rdPartyListViewModel> GetAll_Company3rdParty(string regionId = "", int companyid = -1);


        List<MasterTrasactionListAllData> GetLedgerMasterTrasactionListAllDataExcel(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0);
        List<LadgerAccountDetailViewResult> GetLedgerMasterTrasactionList(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0);
        List<MasterTrasactionListAllData> GetLedgerMasterTrasactionListAllData(int LedgerId, bool isSubLedgerAccount, string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0);
        List<GenericPayee> SearchPayeeList(string namePrefix, int limit);

        CheckLayoutViewModel GetCheckLayout(int id);
        CheckLayoutViewModel GetDefaultCheckLayoutForRegion();
        List<CheckLayoutViewModel> GetCheckLayoutsForRegion();
        bool InsertOrUpdateCheckLayout(CheckLayoutViewModel vm);
        CheckCalibrationViewModel GetCheckCalibration(int id);
        CheckCalibrationViewModel GetCheckCalibrationForRegion();
        bool InsertOrUpdateCheckCalibration(CheckCalibrationViewModel vm);
        CheckViewModel GetCheckDetailsSample();
        CheckViewModel GetCheckDetails(int id);
        CheckViewModelFinalizedReport GetCheckDetailsFinalizedReport(int id);
        decimal? GetCurrentBankBalance(int bankId);
        List<portal_spGet_C_BankStatementDetails_Result> GetBankStatementDetails(int bankId, DateTime startDate, DateTime endDate);

        List<portal_spGet_C_DepositList_Result> GetDepositList(DateTime from, DateTime to);
        List<DepositType> GetDepositTypeList();
        int InsertDeposit(DepositViewModel inputdata);

        ManualCheck GetManualCheckForCheck(int checkBookId);
        List<portal_spGet_C_UnprintedManualCheckList_Result> GetUnprintedManualCheckList();
        int InsertManualCheck(ManualCheckViewModel inputdata);
        List<portal_spGet_BankStatment_Result> GetBankStatementDetailList(string RegionId, DateTime startDate, DateTime endDate);

        List<LadgerAccountViewModel> GetChartofAccounts(string selectedRegion, DateTime? StartDate, DateTime? EndDate, int month = 0, int year = 0);

        List<CompanySearchModel> GetSearchCompanies(string searchText);
        List<CheckBookTransactionTypeList> GetAll_checkbookTransactionTypeList();
        bool InsertBankTrx(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int PaymentTrxTypeId, decimal ApplyAmount, string ChaqueNumber,
           string PayeeName, string PayeeNumber);
        bool SaveCustomerParentChildSetting(int ParentId, string ChildIds,bool IsThirdParty=false,string ConsolidatedInvoice="");
        bool SavePaymentGroup(string GroupName, string CustomerIds);

        List<portal_spGet_AP_PendingUnprintedManualCheckList_Result> GetPendingUnprintedManualCheckList(int RegionId, int trxStatus);
        List<TRPaidInvoicesViewModel> GetPaidInvoices(string FromDate, string ToDate, string FromCheck, string ToCheck, string FromInvoice, string ToInvoice, string FromVendor, string OrderBy, int RegionId);

        List<PaidInvoiceSearchOrderByList> PaidInvoiceSearchOrderByList();
        List<TRPaidInvoicesViewModel> GetPaidInvoicesList(string FromDate, string ToDate, string FromCheck, string ToCheck, string FromInvoice, string ToInvoice, string FromVendor, string OrderBy, int RegionId);

        List<TRPaidInvoicesViewModel> GetInvoiceSearchList(string InvoiceNum, int RegionId);
        List<TRPaidInvoicesViewModel> GetVendorInvoice(string InvoiceNum, int RegionId);

        List<RegularCheckRegisterViewModel> GetRegularAccountTransactions(int RegionId, int MonthPeriod, int YearPeriod);

        List<RegularCheckRegisterDetailViewModel> GetRegularAccountTransactionDetail(int RegionId, string CheckNumber);
        
        List<TROpenInvoicesViewModel> GetTROpenInvoices(string fromDate, string toDate, int regionId, string VendorId);

        APBillCheckViewModel GetAPBillCheck(int checkBookId);

        List<RegionViewModel> GetSelectedRegionData(int RegionId);

        List<CompanyViewModel> GetAddress(int CompanyId);

    }
}
