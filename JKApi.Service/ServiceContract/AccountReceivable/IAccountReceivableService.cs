using JK.Repository.Uow;
using JKApi.Data.DAL;
using JKApi.Service.AccountReceivable;
using JKViewModels.AccountReceivable;
using JKViewModels.Company;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JKApi.Service.ServiceContract.AccountReceivable
{
    public interface IAccountReceivableService
    {
        #region Payment Service
        List<GetCustomerViewModwl> GetCustomers(string namePrefix, string InvoicePrefix);
        List<SearchDateList> GetAll_SearchDateList();
        List<PaymentMethodList> GetAll_PaymentMethodList();
        List<AdjustmentReasonList> GetAll_AdjustmentReasonList();
        List<ARInvoiceListViewModel> GetInvoiceListWithSearchForPayment(string regionId, string searchtext, bool consolidated, string OCValue, int month, int year);
        decimal GetCustomerCreditBalance(int customerId);
        bool InsertApplyOverflowPaymentTransaction(FullManualPaymentViewModel inputdata);

        bool InsertManualPaymentTransactionInTemp(FullManualPaymentViewModel inputdata);
        bool InsertManualPaymentTransactionUpdated(FullManualPaymentViewModel inputdata);
        bool InsertManualPaymentTransaction(ManualPaymentTransactionViewModel inputdata);
        IEnumerable<PaymentListViewModel> GetPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null, int month = 0, int year = 0);
        IEnumerable<portal_spGet_AR_PendingPaymentList_Result> GetPendingPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null);
        IEnumerable<portal_spGet_AR_PendingPaymentList_Result> GetPendingtempPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null);
        #endregion

        #region Credit
        List<CreditReasonList> GetAll_CreditReasonList(bool isTax);
        List<ARInvoiceListViewModel> GetInvoiceListWithSearchForCredit(string regionId, DateTime? fromdate, DateTime? todate, string searchtext, bool? closed, bool? consolidated, int sb, int CustomerId = 0);
        List<ARLBInvoiceListViewModel> GetOpenInvoiceListForLockbox(int regionId, string DateFrom, string DateTo, string transactionStatusListId = "4,7", bool consolidated = false);
        List<ARCustomerWithCreditListViewModel> GetCreditList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool closed, bool consolidated);
        List<ARCustomerWithCreditListViewModel> GetPendingCreditList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool consolidated);
        List<ARCustomerWithCreditListViewModel> GetPendingCreditTempList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool consolidated);

        CreditDetailViewModel GetCreditDetailForInvoiceN(int invoiceid);
        CreditDetailViewModel GetTaxCreditDetailForInvoice(int invoiceId);
        bool InsertTaxCreditDetailForInvoiceTransaction(int _invoiceId, decimal _ApplyTaxAmount, DateTime _TrxDate, string TrxDesc);
        CreditDetailViewModel GetCreditDetailForCreditN(int creditId);
        CreditDetailViewModel GetCreditDetailForCreditTempN(int creditId);

        CreditDetailViewModel GetCreditDetailForTaxCreditTempN(int creditId);

        CreditDetailViewModel GetCreditDetailForInvoicePayment(int invoiceId);
        CreditDetailViewModel GetCreditDetailForInvoice(int invoiceid);
        CreditDetailViewModel GetCreditDetailForCredit(int creditId);
        bool InsertOrUpdateCreditTransaction(CreditTransactionViewModel inputdata);
        bool InsertOrUpdateCreditTransactionInTemp(CreditTransactionViewModel vm);
        bool InsertOrUpdateBalanceAdjustment(CreditTransactionViewModel vm, int MasterTraTypeListId, out int outMastertrxId);
        bool InsertOrUpdateBalanceAdjustmentRefund(List<CreditTransactionViewModel> vm, int mastertrxid);
        int InsertUpdateCustomerCreditTransactionMaintenanceTemp(CreditTransactionViewModel vm, int MaintenanceTempId);
        bool ApproveCredit(int creditId, int status, string note);
        bool ApproveTempCredit(int creditId, int status, string note = "");
        bool DeleteCredit(int creditId);
        bool DeleteCreditTemp(int creditId);
        bool InsertOrUpdateCreditTaxTransactionInTemp(int _invoiceId, decimal _ApplyTaxAmount, DateTime _TrxDate, string TrxDesc, int reasonListId);

        #endregion

        #region Bill Run Calls
        bool GetBillValidated(int month, int year, int batchid);
        BillRunSummaryDetailViewModel GetBillRunSummaryDetail(int month, int year, int batchid, string selectedRegionId = "");
        BillRunSummaryDetailViewModel GenerateInvoiceBillRun(int month, int year, string selectedRegionId = "");
        bool GetUndoBillRun(int batchid, int month, int year, string selectedRegionId);
        List<ARInvoiceListViewModel> GetOpenInvoiceListWithSearch(int month, int year, string searchtext, int invoicetypelistid, string regionId = "");
        //List<ARInvoiceListViewModel> GetInvoiceListWithSearch(int month, int year, string searchtext, int invoicetypelistid, string regionId = "");
        List<ARInvoiceListViewModel> GetInvoiceListWithSearch(int month, int year, string searchtext, int invoicetypelistid, string regionId = "", string sDate = "", string eDate = "");
        List<ARInvoiceListViewModel> GetInvoiceListWithSearch(int month, int year, string searchtext, int filterby, int searchBy, string searchValue, bool eomOnly, bool consolidatedInvoice, int invoicetypelistid, string regionId = "");
        List<InvoiceListViewModel> GetInvoiceList(string regionId, string sDate, string eDate);
        InvoiceDetailViewModel GetInvoiceDetail(int invoiceid);
        InvoiceDetailViewModel GetInvoiceDetailData(int invoiceid);
        ConsolidatedInvoiceDetailViewModel GetConsolidatedInvoiceDetail(int consolidatedInvoiceid);
        bool InvoiceRevert(int invoiceid);
        List<EmailViewModel> GetCustomerEbillEmails(int customerid);
        List<ContractDetailServiceTypeList> GetContractDetailServiceTypeList();
        List<CustomerSearchModel> GetCustomerListData(string searchtext);
        bool InsertCustomerTransaction(CustomerTransactionCommonViewModel inputdata, bool IsMaintenanceTrasaction = false);
        bool DeleteManualInvoice(int MasterTmpTrxId);
        Data.DAL.portal_spGet_AR_CustomerDetail_Result GetCustomerDetailData(int customerid);
        IEnumerable<JKApi.Data.DAL.Franchisee> GetCustomerDistributionDetailFranchiseedata(int customerid);
        List<JKApi.Data.DAL.Franchisee> GetFranchiseeListData(string searchtext);
        List<JKApi.Data.DAL.Franchisee> GetFranchiseeListData();
        #endregion
        ManualInvoiceDetailViewModel GetManualInvoiceDetail(int MasterTmpTrxId);
        List<portal_spGet_AR_GenerateInvoiceList_Result> GetGenerateInvoiceList(int? regionId = null);
        bool GenerateInvoice(int mastertemptrxid, int status);

        //int UploadLockbox(FullLockboxStandardViewModel inputdata);
        LockboxEDI AlreadyExistFileUploadLockboxUpdated(string _line);
        Region GetRegionByLockboxNumber(string lockboxnumber);
        void UploadLockboxDate(int lockboxId, DateTime chkDate);
        List<LockboxEDIDataViewModel> UploadLockboxUpdated(List<CommonTransmissionViewModel> inputdata);
        bool HaveMultipleInvoiceDistribution(int invoiceId);
        List<LockboxEDIDataViewModel> GetLockboxData(int LockboxId);
        List<LockboxPendingViewModel> GetLockboxPendingListData(int regionId);
        LockboxEDIData GetLockboxDataDetail(int LockboxId);
        CustomerCreditDetailsPopupModel CustomerCreditDetailPopup(int CreditId);
        PaymentDetailsPopupModel PaymentDetailPopup(int paymentId, bool IsPending = false);
        PaymentDetailsPopupModel PaymentDetailPopupFromTemp(int paymentId, bool IsPending = false);
        bool ReversalPayment(int Id);
        int UpdatePaymentDetailPopup(int Id, DateTime PaymentDate, int PaymentType, string PaymentNo, decimal Amount, string Note, bool onlyDetail, FullManualPaymentViewModel inputdata);
        PaymentDetailPrintViewModel GetPaymentDetailPrint(int paymentId);
        CreditDetailPrintViewModel GetCreditDetailPrint(int creditId);
        //bool UpdateLockboxDetail(int LockboxId, int LockboxEDIHistoryId, int CustomerId, int InvoiceId, decimal InvoiceAmount, decimal ApplyAmount, bool IsProcessed,decimal OverflowAmount);
        bool UpdateLockboxDetail(int LockboxId, int LockboxEDIHistoryId, string CheckNumber, int CustomerId, int InvoiceId, decimal InvoiceAmount, decimal BalanceAmount,
            decimal ApplyAmount, decimal OverflowAmount, int StatusListId, bool IsNEW,
            bool IsODeposit = false, string DepositReason = "", string DepositPayeeType = "", int DepositServiceTypeListId = -1, int DepositPayeeId = -1, string DepositPayeeName = "", string DepositPayeeNo = "");

        bool UpdateLockboxDetailCheckInactive(int LockboxId, string CheckNumber);
        bool ProcessLockboxPayment(int LockboxId);
        bool ProcessPayment(string ChaqueNumber, int InvoiceId, int CustomerId, decimal InvoiceAmount, decimal ApplyAmount, int PaymentMethodListId, int TransactionStatusListId, List<PartialLockboxPaymentItemViewModel> lstPartialLockboxPaymentItem, DateTime? TRDate = null, decimal? TCheckAmount = 0, decimal OverflowAmount = 0);
        bool ProcessLockboxOtherDeposit(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int DepositServiceTypeListId, decimal ApplyAmount, string ChaqueNumber, int LockboxId);
        bool ProcessCheckbookPaymentLockbox(DateTime TransactionDate, decimal CheckAmount, int LockboxId);
        bool InsertLockboxEDIProcessed(int LockboxId, string ChaqueNumber, int InvoiceId, int CustomerId, decimal ApplyAmount);

        bool InsertOtherDeposit(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int DepositServiceTypeListId, decimal ApplyAmount, string ChaqueNumber,
            string PayeeName, string PayeeNumber);

        List<MonthlyBillRunResultViewModel> GenerateMonthlyBillRun(int month, int year, string selectdRegionIds = "");
        List<portal_spCreate_AR_MonthlyBillRunGenerateList_Result> GetMonthlyBillRunData(int month, int year);
        List<MonthlyBillRunResultViewModel> GetMonthlyBillRunResultData(int month, int year, string selectedRegionIds = "");
        List<AgingReportViewModel> AgingReport(AgingReportViewModel agingReportViewModel);
        List<AgingReportViewModel> AgingDataForCollectionCall(int CustomerId);
        List<AgingViewModel> getAgingList(string franchiseid, string searchby, string searchvalue, string agingdate, string paymentdate, string months, string orderby, string include, string balance, string nonchargebackonly);
        List<ARLogViewModel> getARLogList(string dateType, string startDate, string endDate);
        void savePendingMessage(string message, int customerID, int status, int MasterTmpTrxId);
        IEnumerable<ARCustomerWithCreditListViewModel> GetCustomerWiseCreditList(string regionIds, DateTime? spnStartDate = null, DateTime? spnEndDate = null, int month = 0, int year = 0);
        List<ARLogListFinalViewModel> GetARLogListData(string regionIds, DateTime? CreatedOn);
        IEnumerable<ARCustomerWithCreditListViewModel> GetCreditListWithSearch(int month, int year, int searchBy, string searchValue);
        void saveCommonPendingMessage(string message, int status, int MasterTmpTrxId, string EntrySource, int ClassId, int TypeListId, int MasterTrxTypeListId, int HeaderId);
        List<int> GetPaymentIdsforApprove(int paymnetId);
        List<ARInvoiceListViewModel> GetInvoiceWithSearchForManualPayment(string rgId, string OCValue, string st = "", int sb = 0);
        decimal GetCustomerBalance(string Customerno);
        List<Overflow> GetCustomerOverflowBalance(string Customerno, out decimal BalanceAMT);
        List<Overflow> GetCustomerOverflowBalanceById(int CustomerId, out decimal BalanceAMT);
        List<ARInvoiceListViewModel> GetInvoiceWithSearchForBalanceAdjustment(string rgId, string OCValue, decimal amountFrom, decimal amountTo, string st = "", int sb = 0, int servicet = 0);

        List<OverPaymentListViewModel> OverPaymentReportData(string regionIds, DateTime? FromDate, DateTime? ToDate, string SearchText);
        bool HaveChargebackforInvoiceId(int InvoiceId);
        //ClosedPeriod
        ClosedPeriodViewModel GetClosedPeriodForClose(int regionId);
        bool UpdateStatusClosedPeriod(int PeriodClosedId);
        string GetCustomerBillingEmail(int Id);
        IEnumerable<InvoiceDetailsForNumber> GetInvoiceStatusWiseInvoiceNumber(int month, int year, string searchtext, int v, string r);
        IEnumerable<PastDueViewModel> GetAllPastDueStatement(DateTime? reportDate, int? monthsToInclude, string regionIds);

        PastDueStatementDetailModel GetPastDueStatementDetailsPopup(int Id, DateTime? reportDate);
        RegionSetting GetAdditionalBilling(int RegId);


        OverPaymentCustomerInvoiceViewModel GetOverPaymentCustomerInvoiceDetail(int invoiceId, int OverflowId = 0, decimal OverflowAmount = 0);
        bool InsertGeneralLedgerTrx(int MasterTrxTypeListId, decimal Amount, decimal TaxAmount, DateTime TrxDate, int RegionId, int? MasterTrxId, int? AccountTypeListId);

        bool PostPayment2GeneralLedgerTrx_Checkbook(int _regionId);
    }
}
