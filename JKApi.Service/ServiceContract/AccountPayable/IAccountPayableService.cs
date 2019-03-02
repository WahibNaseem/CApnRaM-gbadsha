using JK.Repository.Uow;
using JKApi.Data.DAL;
using JKApi.Service.AccountPayable;
using JKViewModels.AccountsPayable;
using JKViewModels.Franchise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JKApi.Service.ServiceContract.AccountPayable
{
    public interface IAccountPayableService
    {
        List<SearchDateList> GetAll_SearchDateList();
        List<CheckBookTransactionTypeList> GetAll_CheckTypeList();
        List<FPBillingPay> GetFPInvoiceListWithSearchForPayment(string searchtext, bool consolidated, int? regionId, int month, int year);
        #region Chargeback
        List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> GetInvoiceFranchiseeListForChargeback(int periodid, string regionIds);
        List<portal_spGet_AP_TurnAroundSummaryList_Result> GetTurnAroundSummaryList(DateTime PaymentDate, string regionIds);
        List<portal_spGet_AP_TurnAroundDetailsList_Result> GetTurnAroundDetailsList(DateTime PaymentDatePaymentDate, string RegionIds);
        List<portal_spGet_AP_TurnAroundListSummary_Result> GetTurnAroundListSummary(string regionIds, DateTime PaymentDateFrom, DateTime PaymentDateTo);



        List<portal_spGet_AP_InvoiceFranchiseeSummaryForChargeback_Result> GetInvoiceFranchiseeSummaryForChargeback(int periodid, string regionIds = null);
        string InsertFranchiseeChargebackTransaction(List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> inputdata, DateTime TrxDate);

        string InsertFranchiseeTurnAroundTransaction(List<portal_spGet_AP_TurnAroundDetailsList_Result> inputdata, int TARPeriodId, DateTime TransactionDate);
        IEnumerable<portal_spGetChargeBackList_Result> GetChargeBackList(int? regionId);
        #endregion

        #region Franchisee Pay
        void GenerateFranchiseeReports(int billMonth, int billYear, int? regionId = null);
        List<portal_spGet_AP_GeneratedFranchiseeReportList_Result> GetGeneratedFranchiseeReportList(int? billMonth, int? billYear, string regionIds = null);
        List<portal_spGet_AP_FinalizedFranchiseeReportList_Result> GetFinalizedFranchiseeReportList(int? billMonth, int? billYear, string regionIds = null);
        List<portal_spGet_AP_FinalizedFranchiseeReportPeriods_Result> GetFinalizedFranchiseeReportPeriods(int? regionId = null);

        TurnAroundDetailsViewModel GetTurnAroundDetailsForCheck(int checkBookId);

        FranchiseeReportDetailsViewModel GetFranchiseeReportDetailsForCheck(int checkBookId);
        FranchiseeReportDetailsViewModel GetFranchiseeReportDetailsForAPBill(int apBillId);

        FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportFinalizedDetailsForCheck(int checkBookId);
        FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportFinalizedDetailsForAPBill(int apBillId);

        FranchiseeReportDetailsViewModel GetFranchiseeReportDetails(int franchiseeReportId);
        FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportDetailsFinalized(int franchiseeReportId);
        List<portal_spGet_AP_FranchiseeReportDeductions_Result> GetFranchiseeReportDeductions(int franchiseeReportId);
        bool DeleteGeneratedFranchiseeReport(int franchiseeReportId);
        bool ClearGeneratedFranchiseeReport(string RegionIds, int PeriodId);
        bool FinalizeFranchiseeReport(int franchiseeReportId);
        bool FinalizeFranchiseeReportCreate(string selectedRegionId, int periodId, string franchiseeReportId, int userId);

        //List<portal_spGet_AP_FranchiseeTurnaroundCheckList_Result> GetFranchiseeTurnaroundCheckList(string regionId, int? FranchiseeId);
        //List<portal_spGet_AP_FranchiseeTurnaroundCheckDetails_Result> GetFranchiseeTurnaroundCheckDetails(string regionId, int? FranchiseeId);

        //List<portal_spGet_AP_FranchiseeTurnaroundCheckList_Result> GetFranchiseeTurnaroundCheckList(int? regionId, DateTime startDate, DateTime endDate, int TurnAroundCheckType);
        //List<portal_spGet_AP_FranchiseeTurnaroundCheckDetails_Result> GetFranchiseeTurnaroundCheckDetails(int? regionId, int franchiseeId, DateTime startDate, DateTime endDate);
        int InsertFranchiseeTurnaroundCheck(FranchiseeTurnaroundCheckViewModel inputdata);
        List<portal_spGet_AP_APBillListByType_Result> GetAPBillListForCheckType(int? regionId, int checkTypeId, int TransactionStausId, string APBillIst = "");
        List<portal_spGet_AP_APBillListById_Result> GetAPBillListForCheckById(int? regionId, string APBillListId);

        List<portal_spGet_AP_OpenNotPrintedAPBillListByType_Result> GetOpenNotPrintedAPBillListForCheckType(int? regionId, int checkTypeId);

        int InsertAPBillTransactionForFranchiseeReport(int franchiseeReportId);
        int InsertAPBillTransaction(APBillTransactionViewModel inputdata);
        int InsertCheckBookFromAPBill(int apBillId, int bankId, DateTime trxDate);

        int UpdateOpenNotPrintedAPBill(int APBillId);
        FranchiseBillingDetailViewModel GetFranchiseBillingDetails(int billPayId);
        FranchiseBillingDetailViewModel GetFranchiseBillingDetailsWithbillno(string billno);
        IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> GetFranchiseeWiseChargebackSummaryOrDetailsResult(string regionIds, bool isSummaryView, DateTime? spnStartDate, DateTime? spnEndDate, int month, int year, string CBTrx,int PeriodId);
        List<ChargebackHistoryReportViewModel> GetChargebackHistoryReportList(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0);
        List<ChargebackHistoryReportSummaryViewModel> GetChargebackHistorySummaryReportList(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0);
        #endregion

        IEnumerable<portal_spGet_AP_TurnAroundList_Result> GetTurnAroundListList(string regionIds = "", DateTime? from = null, DateTime? to = null);
        IEnumerable<portal_spGet_AP_TurnAroundListSummary_Result> GetTurnAroundListListSummary(string regionIds = "", DateTime? from = null, DateTime? to = null);
        List<NegativeDueViewModel> GetNagativeDue(int franchiseeStatus, string regionIds = "");
        bool InsertFranchiseeManualTrasactionFromWriteCheck(DateTime TransactionDate, int ClassId, int TypeListId, decimal Amount, string FMDescription,int checkTypeListId);
        bool InsertFranchiseeManualTrasactionFromWriteCheck(List<int> cbList);
        bool AddNegativeDue(decimal ParAmt, int NdId);
        bool AddNegativeDueRoll(int NdId);
        bool AddNegativeDueFullPayment(int NdId);
        AccountingFeeRebateFullViewModel GetAccountingFeeRebate(string RegionId);
       string InsertFranchiseeAccountingFeeRebate(List<AccountingFeeRebateFullViewModel> inputdata, int AccFeeRebatePeriodId);
        AccountingFeeRebateDetailsViewModel GetAccountingFeeRebateDetailsForCheck(int checkBookId);
        bool UpdatePeriodClosed(int PeriodClosedId);
        Period GetPeriod(int PeriodId);
        PeriodClosed GetPeriodClosed(int PeriodClosedId);
        PeriodClosed GetPreviousPeriod(int PeriodClosedId, int RegionId);

        List<portal_spGet_AP_PendingAPBillListByType_Result> GetPendingAPBillListForCheckType(int? regionId, int? @CheckbookTransactionTypeListId, int trxStatus, string APNillList = "");
        int AccountingFeeRebateWorker(int apBillId, string TransactionStatus);
        void CheckSourceDataWorker(int apBillId, int CheckBookTransactionTypeListId, string TransactionStatus);
        int UndoCheck(int apBillId);

        bool IsCheckSystemGenerated(int CheckBookTransactionTypeId);

    }
}
