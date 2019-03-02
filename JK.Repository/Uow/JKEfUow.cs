using JK.Repository.Contracts;
using JK.Repository.Helpers;
using JKApi.Core;
using JKApi.Data.DAL;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace JK.Repository.Uow
{
    public class JKEfUow : IJKEfUow, IDisposable
    {
        private NLogger _NLogger = new NLogger();
        protected IRepositoryProvider RepositoryProvider { get; set; }
        private jkDatabaseEntities DbContext { get; set; }

        public JKEfUow(IRepositoryProvider repositoryProvider)
        {

            CreateDbContext();

            repositoryProvider.DbContext = DbContext;

            RepositoryProvider = repositoryProvider;
        }

        public void Commit()
        {
            try
            {
                int Status = DbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {


                foreach (var validationError in dbEx.EntityValidationErrors.ToList())
                {
                    string _entityName = validationError.Entry.Entity.ToString();

                    foreach (var validationError1 in validationError.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError1.PropertyName, validationError1.ErrorMessage);
                        _NLogger.Error(String.Format("Entity:{2}, Property: {0} Error: {1}", validationError1.PropertyName, validationError1.ErrorMessage, _entityName));
                    }
                }

                //foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                //{

                //    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                //    _NLogger.Error("Requested SummaryData is null or empty");
                //}
            }
        }




        //public Itbl_AddressRepository TblAddress
        //{
        //    get { return GetRepo<Itbl_AddressRepository>(); }
        //}
        //public Itbl_AddressTypeRepository TblAddressType
        //{
        //    get { return GetRepo<Itbl_AddressTypeRepository>(); }
        //}
        //public Itbl_AP_RegisterVoidRepository RegisterVoid
        //{
        //    get { return GetRepo<Itbl_AP_RegisterVoidRepository>(); }
        //}
        //public Itbl_AP_VendorRepository Vendor
        //{
        //    get { return GetRepo<Itbl_AP_VendorRepository>(); }
        //}
        //public Itbl_AR_CreditDetailRepository CreditDetail
        //{
        //    get { return GetRepo<Itbl_AR_CreditDetailRepository>(); }
        //}
        //public Itbl_AR_CreditFTrxRepository CreditFTrx
        //{
        //    get { return GetRepo<Itbl_AR_CreditFTrxRepository>(); }
        //}
        //public Itbl_AR_CreditRepository ARCredit
        //{
        //    get { return GetRepo<Itbl_AR_CreditRepository>(); }
        //}
        //public Itbl_AR_InvoiceAddressRepository InvoiceAddress
        //{
        //    get { return GetRepo<Itbl_AR_InvoiceAddressRepository>(); }
        //}

        //public Itbl_AR_InvoiceDetailDescriptionRepository InvoiceDetailDescription
        //{
        //    get { return GetRepo<Itbl_AR_InvoiceDetailDescriptionRepository>(); }
        //}
        //public Itbl_AR_InvoiceDetailRepository InvoiceDetail
        //{
        //    get { return GetRepo<Itbl_AR_InvoiceDetailRepository>(); }
        //}

        //public Itbl_AR_InvoiceHeaderRepository InvoiceHeader
        //{
        //    get { return GetRepo<Itbl_AR_InvoiceHeaderRepository>(); }
        //}
        //public Itbl_AR_Lockbox_histRepository Lockboxhist
        //{
        //    get { return GetRepo<Itbl_AR_Lockbox_histRepository>(); }
        //}
        //public Itbl_AR_LockboxRepository Lockbox
        //{
        //    get { return GetRepo<Itbl_AR_LockboxRepository>(); }
        //}

        //public Itbl_AR_OtherDepositsRepository OtherDeposits
        //{
        //    get { return GetRepo<Itbl_AR_OtherDepositsRepository>(); }
        //}
        //public Itbl_AR_PaymentChargebackDistributionRepository PaymentChargebackDistribution
        //{
        //    get { return GetRepo<Itbl_AR_PaymentChargebackDistributionRepository>(); }
        //}
        //public Itbl_AR_PaymentChargebackFeeRepository PaymentChargebackFee
        //{
        //    get { return GetRepo<Itbl_AR_PaymentChargebackFeeRepository>(); }
        //}

        //public Itbl_AR_PaymentChargebackRepository PaymentChargeback
        //{
        //    get { return GetRepo<Itbl_AR_PaymentChargebackRepository>(); }
        //}

        //public Itbl_AR_PaymentChargebackTaxRepository PaymentChargebackTax
        //{
        //    get { return GetRepo<Itbl_AR_PaymentChargebackTaxRepository>(); }
        //}

        //public Itbl_AR_PaymentDetailRepository PaymentDetail
        //{
        //    get { return GetRepo<Itbl_AR_PaymentDetailRepository>(); }
        //}

        //public Itbl_AR_PaymentFTrxRepository PaymentFTrx
        //{
        //    get { return GetRepo<Itbl_AR_PaymentFTrxRepository>(); }
        //}
        //public Itbl_AR_PaymentMoveReasonRepository PaymentMoveReason
        //{
        //    get { return GetRepo<Itbl_AR_PaymentMoveReasonRepository>(); }
        //}
        //public Itbl_AR_PaymentRepository ArPayment
        //{
        //    get { return GetRepo<Itbl_AR_PaymentRepository>(); }
        //}
        //public Itbl_balanceRepository Balance
        //{
        //    get { return GetRepo<Itbl_balanceRepository>(); }
        //}
        //public Itbl_C_AddressRepository tbl_C_Address
        //{
        //    get { return GetRepo<Itbl_C_AddressRepository>(); }
        //}
        //public Itbl_C_BillingAddressRepository BillingAddress
        //{
        //    get { return GetRepo<Itbl_C_BillingAddressRepository>(); }
        //}
        //public Itbl_C_BillRunConfigRepository BillRunConfig
        //{
        //    get { return GetRepo<Itbl_C_BillRunConfigRepository>(); }
        //}
        //public Itbl_C_BillSettingsRepository BillSettings
        //{
        //    get { return GetRepo<Itbl_C_BillSettingsRepository>(); }
        //}
        //public Itbl_C_CallBacksRepository CallBack
        //{
        //    get { return GetRepo<Itbl_C_CallBacksRepository>(); }
        //}

        //public Itbl_C_ClaimRepository Claim
        //{
        //    get { return GetRepo<Itbl_C_ClaimRepository>(); }
        //}
        //public Itbl_C_CollectionCallsRepository CollectionCalls
        //{
        //    get { return GetRepo<Itbl_C_CollectionCallsRepository>(); }
        //}

        //public Itbl_C_ComplaintFranchise_xrefRepository ComplaintFranchisexref
        //{
        //    get { return GetRepo<Itbl_C_ComplaintFranchise_xrefRepository>(); }
        //}
        //public Itbl_C_ComplaintRepository Complaint
        //{
        //    get { return GetRepo<Itbl_C_ComplaintRepository>(); }
        //}
        //public Itbl_C_ConsolidatedInvoiceDetailRepository ConsolidatedInvoiceDetail
        //{
        //    get { return GetRepo<Itbl_C_ConsolidatedInvoiceDetailRepository>(); }
        //}
        //public Itbl_C_ConsolidatedInvoiceRepository ConsolidatedInvoice
        //{
        //    get { return GetRepo<Itbl_C_ConsolidatedInvoiceRepository>(); }
        //}
        //public Itbl_C_ContactRepository tbl_C_Contact
        //{
        //    get { return GetRepo<Itbl_C_ContactRepository>(); }
        //}
        //public Itbl_C_Contract_AdjHistRepository ContractAdjHist
        //{
        //    get { return GetRepo<Itbl_C_Contract_AdjHistRepository>(); }
        //}

        //public Itbl_C_ContractDetailDescriptionRepository tbl_C_ContractDetailDescription
        //{
        //    get { return GetRepo<Itbl_C_ContractDetailDescriptionRepository>(); }
        //}
        //public Itbl_C_ContractDetailIncrDecrRepository ContractDetailIncrDecr
        //{
        //    get { return GetRepo<Itbl_C_ContractDetailIncrDecrRepository>(); }
        //}
        //public Itbl_C_ContractDetailServiceFeeRepository ContractDetailServiceFee
        //{
        //    get { return GetRepo<Itbl_C_ContractDetailServiceFeeRepository>(); }
        //}
        //public Itbl_C_ContractDetailServiceRepository ContractDetailService
        //{
        //    get { return GetRepo<Itbl_C_ContractDetailServiceRepository>(); }
        //}
        //public Itbl_C_ContractFeeRepository ContractFee
        //{
        //    get { return GetRepo<Itbl_C_ContractFeeRepository>(); }
        //}
        //public Itbl_C_ContractIncrDecrRepository ContractIncrDecr
        //{
        //    get { return GetRepo<Itbl_C_ContractIncrDecrRepository>(); }
        //}
        //public Itbl_C_ContractRepository tbl_C_Contract
        //{
        //    get { return GetRepo<Itbl_C_ContractRepository>(); }
        //}
        //public Itbl_C_ContractStatus_histRepository ContractStatushist
        //{
        //    get { return GetRepo<Itbl_C_ContractStatus_histRepository>(); }
        //}
        //public Itbl_C_CustomerJKUser_refRepository CustomerJKUserref
        //{
        //    get { return GetRepo<Itbl_C_CustomerJKUser_refRepository>(); }
        //}
        //public Itbl_C_EmailRepository tbl_C_Email
        //{
        //    get { return GetRepo<Itbl_C_EmailRepository>(); }
        //}
        //public Itbl_C_InformationRepository CustomerInformation
        //{
        //    get { return GetRepo<Itbl_C_InformationRepository>(); }
        //}
        //public Itbl_C_InformationStatusHistRepository InformationStatusHist
        //{
        //    get { return GetRepo<Itbl_C_InformationStatusHistRepository>(); }
        //}

        //public Itbl_C_noteRepository Note
        //{
        //    get { return GetRepo<Itbl_C_noteRepository>(); }
        //}
        //public Itbl_C_PaymentsRepository Payments
        //{
        //    get { return GetRepo<Itbl_C_PaymentsRepository>(); }
        //}
        //public Itbl_C_PhoneRepository tbl_C_Phone
        //{
        //    get { return GetRepo<Itbl_C_PhoneRepository>(); }
        //}

        //public Itbl_C_ServiceLogAreaRepository ServiceLogArea
        //{
        //    get { return GetRepo<Itbl_C_ServiceLogAreaRepository>(); }
        //}

        //public Itbl_C_ServiceLogRepository ServiceLog
        //{
        //    get { return GetRepo<Itbl_C_ServiceLogRepository>(); }
        //}
        //public Itbl_C_StatusRepository tbl_C_Status
        //{
        //    get { return GetRepo<Itbl_C_StatusRepository>(); }
        //}
        //public Itbl_C_TrxBatchRepository TrxBatch
        //{
        //    get { return GetRepo<Itbl_C_TrxBatchRepository>(); }
        //}
        //public Itbl_C_TrxCreditDetailRepository TrxCreditDetail
        //{
        //    get { return GetRepo<Itbl_C_TrxCreditDetailRepository>(); }
        //}

        //public Itbl_C_TrxDescriptionRepository TrxDescription
        //{
        //    get { return GetRepo<Itbl_C_TrxDescriptionRepository>(); }
        //}

        //public Itbl_C_TrxDescriptionTmpRepository TrxDescriptionTmp
        //{
        //    get { return GetRepo<Itbl_C_TrxDescriptionTmpRepository>(); }
        //}
        //public Itbl_C_TrxDistributionHistoryRepository TrxDistributionHistory
        //{
        //    get { return GetRepo<Itbl_C_TrxDistributionHistoryRepository>(); }
        //}
        //public Itbl_C_TrxDistributionIncrDecrRepository TrxDistributionIncrDecr
        //{
        //    get { return GetRepo<Itbl_C_TrxDistributionIncrDecrRepository>(); }
        //}
        //public Itbl_C_TrxDistributionRepository TrxDistribution
        //{
        //    get { return GetRepo<Itbl_C_TrxDistributionRepository>(); }
        //}

        //public Itbl_C_TrxFeeRepository TrxFee
        //{
        //    get { return GetRepo<Itbl_C_TrxFeeRepository>(); }
        //}


        //public Itbl_C_TrxMarkUpRepository TrxMarkUp
        //{
        //    get { return GetRepo<Itbl_C_TrxMarkUpRepository>(); }
        //}


        //public Itbl_C_TrxRecurringFeeRepository TrxRecurringFee
        //{
        //    get { return GetRepo<Itbl_C_TrxRecurringFeeRepository>(); }
        //}
        //public Itbl_C_TrxRecurringIncrDecrRepository TrxRecurringIncrDecr
        //{
        //    get { return GetRepo<Itbl_C_TrxRecurringIncrDecrRepository>(); }
        //}


        //public Itbl_C_TrxRecurringTaxRepository TrxRecurringTax
        //{
        //    get { return GetRepo<Itbl_C_TrxRecurringTaxRepository>(); }
        //}
        //public Itbl_C_TrxRepository Trx
        //{
        //    get { return GetRepo<Itbl_C_TrxRepository>(); }
        //}

        //public Itbl_C_TrxTaxRepository TrxTax
        //{
        //    get { return GetRepo<Itbl_C_TrxTaxRepository>(); }
        //}
        //public Itbl_C_TrxTmpRepository TrxTmp
        //{
        //    get { return GetRepo<Itbl_C_TrxTmpRepository>(); }
        //}

        //public Itbl_Chk_DetailsRepository Details
        //{
        //    get { return GetRepo<Itbl_Chk_DetailsRepository>(); }
        //}
        //public Itbl_Chk_HeaderRepository Header
        //{
        //    get { return GetRepo<Itbl_Chk_HeaderRepository>(); }
        //}
        //public Itbl_Chk_ManualRepository Manual
        //{
        //    get { return GetRepo<Itbl_Chk_ManualRepository>(); }
        //}
        //public Itbl_ContactCommRepository ContactComm
        //{
        //    get { return GetRepo<Itbl_ContactCommRepository>(); }
        //}
        //public Itbl_ContactCommTypeRepository ContactCommType
        //{
        //    get { return GetRepo<Itbl_ContactCommTypeRepository>(); }
        //}
        //public Itbl_ContactRepository Contactc
        //{
        //    get { return GetRepo<Itbl_ContactRepository>(); }
        //}
        //public Itbl_ContactTypeRepository ContactType
        //{
        //    get { return GetRepo<Itbl_ContactTypeRepository>(); }
        //}




        //public Itbl_date_typeRepository Datetype
        //{
        //    get { return GetRepo<Itbl_date_typeRepository>(); }
        //}

        //public Itbl_F_ChargebackCreditFeeRepository ChargebackCreditFee
        //{
        //    get { return GetRepo<Itbl_F_ChargebackCreditFeeRepository>(); }
        //}
        //public Itbl_F_ChargebackCreditRepository ChargebackCredit
        //{
        //    get { return GetRepo<Itbl_F_ChargebackCreditRepository>(); }
        //}

        //public Itbl_F_ChargebackCreditTaxRepository ChargebackCreditTax
        //{
        //    get { return GetRepo<Itbl_F_ChargebackCreditTaxRepository>(); }
        //}
        //public Itbl_F_ChargebackFeeRepository ChargebackFee
        //{
        //    get { return GetRepo<Itbl_F_ChargebackFeeRepository>(); }
        //}
        //public Itbl_F_ChargebackRepository Chargeback
        //{
        //    get { return GetRepo<Itbl_F_ChargebackRepository>(); }
        //}
        //public Itbl_F_ChargebackTaxRepository ChargebackTax
        //{
        //    get { return GetRepo<Itbl_F_ChargebackTaxRepository>(); }
        //}
        //public Itbl_F_ChargebackTurnAroundRepository ChargebackTurnAround
        //{
        //    get { return GetRepo<Itbl_F_ChargebackTurnAroundRepository>(); }
        //}
        //public Itbl_F_ContactRepository ContactRepository
        //{
        //    get { return GetRepo<Itbl_F_ContactRepository>(); }
        //}
        //public Itbl_F_CustomerxrefRepository Customerxref
        //{
        //    get { return GetRepo<Itbl_F_CustomerxrefRepository>(); }
        //}
        //public Itbl_F_DistributionRepository Distribution
        //{
        //    get { return GetRepo<Itbl_F_DistributionRepository>(); }
        //}

        //public Itbl_F_FeeOverrideRepository FeeOverride
        //{
        //    get { return GetRepo<Itbl_F_FeeOverrideRepository>(); }
        //}
        //public Itbl_F_FeeRepository Fee
        //{
        //    get { return GetRepo<Itbl_F_FeeRepository>(); }
        //}
        //public Itbl_F_FinderFeeAdjustmentRepository FinderFeeAdjustment
        //{
        //    get { return GetRepo<Itbl_F_FinderFeeAdjustmentRepository>(); }
        //}
        //public Itbl_F_FindersFeeHistRepository FindersFeeHist
        //{
        //    get { return GetRepo<Itbl_F_FindersFeeHistRepository>(); }
        //}
        //public Itbl_F_FindersFeeRepository FindersFee
        //{
        //    get { return GetRepo<Itbl_F_FindersFeeRepository>(); }
        //}
        //public Itbl_F_FindersFeeStatusHistRepository FindersFeeStatusHist
        //{
        //    get { return GetRepo<Itbl_F_FindersFeeStatusHistRepository>(); }
        //}
        //public Itbl_F_FulfillmentHistRepository FulfillmentHist
        //{
        //    get { return GetRepo<Itbl_F_FulfillmentHistRepository>(); }
        //}
        //public Itbl_F_FulfillmentRepository Fulfillment
        //{
        //    get { return GetRepo<Itbl_F_FulfillmentRepository>(); }
        //}

        //public Itbl_F_InformationHistRepository InformationHist
        //{
        //    get { return GetRepo<Itbl_F_InformationHistRepository>(); }
        //}

        //public Itbl_F_InformationRepository Informations
        //{
        //    get { return GetRepo<Itbl_F_InformationRepository>(); }
        //}
        //public Itbl_F_InformationStatusHistRepository InformationsStatusHist
        //{
        //    get { return GetRepo<Itbl_F_InformationStatusHistRepository>(); }
        //}
        //public Itbl_F_LeaseRepository Lease
        //{
        //    get { return GetRepo<Itbl_F_LeaseRepository>(); }
        //}
        //public Itbl_F_LeaseStatusHistRepository LeaseStatusHist
        //{
        //    get { return GetRepo<Itbl_F_LeaseStatusHistRepository>(); }
        //}
        //public Itbl_F_LogRepository LogRepository
        //{
        //    get { return GetRepo<Itbl_F_LogRepository>(); }
        //}
        //public Itbl_F_MonthlyBillingRepository MonthlyBilling
        //{
        //    get { return GetRepo<Itbl_F_MonthlyBillingRepository>(); }
        //}
        //public Itbl_F_MonthlyCustomerAccountTotalsRepository MonthlyCustomerAccountTotals
        //{
        //    get { return GetRepo<Itbl_F_MonthlyCustomerAccountTotalsRepository>(); }
        //}
        //public Itbl_F_MonthlyDeductionDetailsRepository MonthlyDeductionDetails
        //{
        //    get { return GetRepo<Itbl_F_MonthlyDeductionDetailsRepository>(); }
        //}
        //public Itbl_F_MonthlyDeductionRepository MonthlyDeduction
        //{
        //    get { return GetRepo<Itbl_F_MonthlyDeductionRepository>(); }
        //}
        //public Itbl_F_MonthlyTaxRepository MonthlyTax
        //{
        //    get { return GetRepo<Itbl_F_MonthlyTaxRepository>(); }
        //}
        //public Itbl_F_MonthlyTotalBillingDetailRepository MonthlyTotalBillingDetail
        //{
        //    get { return GetRepo<Itbl_F_MonthlyTotalBillingDetailRepository>(); }
        //}
        //public Itbl_F_MonthlyTotalRepository MonthlyTotal
        //{
        //    get { return GetRepo<Itbl_F_MonthlyTotalRepository>(); }
        //}
        //public Itbl_F_NegativeDueCreditRepository NegativeDueCredit
        //{
        //    get { return GetRepo<Itbl_F_NegativeDueCreditRepository>(); }
        //}
        //public Itbl_F_NegativeDuePaymentRepository NegativeDuePayment
        //{
        //    get { return GetRepo<Itbl_F_NegativeDuePaymentRepository>(); }
        //}
        //public Itbl_F_NegativeDueRepository NegativeDue
        //{
        //    get { return GetRepo<Itbl_F_NegativeDueRepository>(); }
        //}

        //public Itbl_F_NoteRepository Notes
        //{
        //    get { return GetRepo<Itbl_F_NoteRepository>(); }
        //}
        //public Itbl_F_NoteStatusHistRepository NoteStatusHist
        //{
        //    get { return GetRepo<Itbl_F_NoteStatusHistRepository>(); }
        //}
        //public Itbl_F_OfferRepository Offer
        //{
        //    get { return GetRepo<Itbl_F_OfferRepository>(); }
        //}
        //public Itbl_F_OwnerRepository Owner
        //{
        //    get { return GetRepo<Itbl_F_OwnerRepository>(); }
        //}
        //public Itbl_F_PayeeRepository Payee
        //{
        //    get { return GetRepo<Itbl_F_PayeeRepository>(); }
        //}
        //public Itbl_F_PlanTypeRepository PlanType
        //{
        //    get { return GetRepo<Itbl_F_PlanTypeRepository>(); }
        //}
        //public Itbl_F_RebateRepository Rebate
        //{
        //    get { return GetRepo<Itbl_F_RebateRepository>(); }
        //}
        //public Itbl_F_ReportCustomersRepository ReportCustomers
        //{
        //    get { return GetRepo<Itbl_F_ReportCustomersRepository>(); }
        //}

        //public Itbl_F_TraverseTrxRepository TraverseTrx
        //{
        //    get { return GetRepo<Itbl_F_TraverseTrxRepository>(); }
        //}



        //public Itbl_F_TrxFeeRepository TrxsFee
        //{
        //    get { return GetRepo<Itbl_F_TrxFeeRepository>(); }
        //}

        //public Itbl_F_TrxFeeTmpRepository TrxFeeTmp
        //{
        //    get { return GetRepo<Itbl_F_TrxFeeTmpRepository>(); }
        //}
        //public Itbl_F_TrxRecurringRepository TrxRecurring
        //{
        //    get { return GetRepo<Itbl_F_TrxRecurringRepository>(); }
        //}
        //public Itbl_F_TrxRecurringStatusHistRepository TrxRecurringStatusHist
        //{
        //    get { return GetRepo<Itbl_F_TrxRecurringStatusHistRepository>(); }
        //}
        //public Itbl_F_TrxRepository Trxs
        //{
        //    get { return GetRepo<Itbl_F_TrxRepository>(); }
        //}

        //public Itbl_F_TrxTaxRepository TrxTaxs
        //{
        //    get { return GetRepo<Itbl_F_TrxTaxRepository>(); }
        //}
        //public Itbl_F_TrxTaxTmpRepository TrxTaxTmp
        //{
        //    get { return GetRepo<Itbl_F_TrxTaxTmpRepository>(); }
        //}
        //public Itbl_F_TrxTmpRepository TrxTmps
        //{
        //    get { return GetRepo<Itbl_F_TrxTmpRepository>(); }
        //}
        //public Itbl_GL_Chart_of_AccountRepository ChartofAccount
        //{
        //    get { return GetRepo<Itbl_GL_Chart_of_AccountRepository>(); }
        //}
        //public Itbl_GL_FinancialTransactionRepository FinancialTransaction
        //{
        //    get { return GetRepo<Itbl_GL_FinancialTransactionRepository>(); }
        //}
        //public Itbl_GL_LedgerRepository Ledger
        //{
        //    get { return GetRepo<Itbl_GL_LedgerRepository>(); }
        //}

        //public Itbl_GL_PartyTypeRepository PartyType
        //{
        //    get { return GetRepo<Itbl_GL_PartyTypeRepository>(); }
        //}
        //public Itbl_includeRepository Include
        //{
        //    get { return GetRepo<Itbl_includeRepository>(); }
        //}
        //public Itbl_LeaseRepository Leases
        //{
        //    get { return GetRepo<Itbl_LeaseRepository>(); }
        //}
        //public Itbl_order_byRepository Ordeby
        //{
        //    get { return GetRepo<Itbl_order_byRepository>(); }
        //}

        //public Itbl_R_DeductionPayeeRepository DeductionPayee
        //{
        //    get { return GetRepo<Itbl_R_DeductionPayeeRepository>(); }
        //}
        //public Itbl_R_MonthlyDeductionsPaidRepository MonthlyDeductionsPaid
        //{
        //    get { return GetRepo<Itbl_R_MonthlyDeductionsPaidRepository>(); }
        //}
        //public Itbl_R_MonthlyDeductionsRepository MonthlyDeductions
        //{
        //    get { return GetRepo<Itbl_R_MonthlyDeductionsRepository>(); }
        //}
        //public Itbl_R_MonthlyTaxRepository MonthlyTaxs
        //{
        //    get { return GetRepo<Itbl_R_MonthlyTaxRepository>(); }
        //}
        //public Itbl_Register_2BAKRepository Register2BAK
        //{
        //    get { return GetRepo<Itbl_Register_2BAKRepository>(); }
        //}
        //public Itbl_Register_2Repository Register2
        //{
        //    get { return GetRepo<Itbl_Register_2Repository>(); }
        //}
        //public Itbl_RegisterDetails_2BAKRepository RegisterDetails2BAK
        //{
        //    get { return GetRepo<Itbl_RegisterDetails_2BAKRepository>(); }
        //}
        //public Itbl_RegisterDetails_2Repository RegisterDetails2
        //{
        //    get { return GetRepo<Itbl_RegisterDetails_2Repository>(); }
        //}
        //public Itbl_search_byRepository Searchby
        //{
        //    get { return GetRepo<Itbl_search_byRepository>(); }
        //}
        //public Itbl_Sys_AccountRebateRepository AccountRebate
        //{
        //    get { return GetRepo<Itbl_Sys_AccountRebateRepository>(); }
        //}
        //public Itbl_Sys_BBPAdminFeeRepository BBPAdminFee
        //{
        //    get { return GetRepo<Itbl_Sys_BBPAdminFeeRepository>(); }
        //}
        //public Itbl_sys_C_ServiceLogAreaRepository ServiceLogAreas
        //{
        //    get { return GetRepo<Itbl_sys_C_ServiceLogAreaRepository>(); }
        //}
        //public Itbl_Sys_ConfigurationRepository Configuration
        //{
        //    get { return GetRepo<Itbl_Sys_ConfigurationRepository>(); }
        //}
        //public Itbl_Sys_CountyTaxAuthorityRepository CountyTaxAuthority
        //{
        //    get { return GetRepo<Itbl_Sys_CountyTaxAuthorityRepository>(); }
        //}
        //public Itbl_Sys_CustomerInfoTypeRepository CustomerInfoType
        //{
        //    get { return GetRepo<Itbl_Sys_CustomerInfoTypeRepository>(); }
        //}
        //public Itbl_Sys_FeeRepository SysFee
        //{
        //    get { return GetRepo<Itbl_Sys_FeeRepository>(); }
        //}
        //public Itbl_Sys_LettersRepository SysLetters
        //{
        //    get { return GetRepo<Itbl_Sys_LettersRepository>(); }
        //}

        //public Itbl_sys_PaymentTypeGroupRepository PaymentTypeGroup
        //{
        //    get { return GetRepo<Itbl_sys_PaymentTypeGroupRepository>(); }
        //}
        //public Itbl_sys_PaymentTypeRepository PaymentType
        //{
        //    get { return GetRepo<Itbl_sys_PaymentTypeRepository>(); }
        //}
        //public Itbl_sys_RegisterTypeRepository RegisterType
        //{
        //    get { return GetRepo<Itbl_sys_RegisterTypeRepository>(); }
        //}
        //public Itbl_Sys_StatusReasonRepository StatusReason
        //{
        //    get { return GetRepo<Itbl_Sys_StatusReasonRepository>(); }
        //}
        //public Itbl_Sys_StatusRepository SysStatus
        //{
        //    get { return GetRepo<Itbl_Sys_StatusRepository>(); }
        //}
        //public Itbl_sys_TransactionTypeGroupRepository TransactionTypeGroup
        //{
        //    get { return GetRepo<Itbl_sys_TransactionTypeGroupRepository>(); }
        //}

        //public ItmpftrxtrxsRepository Itmpftrxtrxs
        //{
        //    get { return GetRepo<ItmpftrxtrxsRepository>(); }
        //}
        //public ItmppaymentRepository Itmppayment
        //{
        //    get { return GetRepo<ItmppaymentRepository>(); }
        //}

        //public ItmpsprdRepository Itmpsprd
        //{
        //    get { return GetRepo<ItmpsprdRepository>(); }
        //}





        public ICustomerRepository Customer
        {
            get { return GetRepo<ICustomerRepository>(); }
        }

        public IServiceCallLogRepository ServiceCallLog
        {
            get { return GetRepo<IServiceCallLogRepository>(); }
        }
        //public ICallLogAssociateRepository CallLogAssociate
        //{
        //    get { return GetRepo<ICallLogAssociateRepository>(); }
        //}
        /*public ICollectionsCallLogRepository CollectionsCallLog
        {
            get { return GetRepo<ICollectionsCallLogRepository>(); }
        }*/

        public IPhoneRepository Phone
        {
            get { return GetRepo<IPhoneRepository>(); }
        }

        public IAddressRepository Address
        {
            get { return GetRepo<IAddressRepository>(); }
        }

        public ICountryCodeListRepository CountryCodeList
        {
            get { return GetRepo<ICountryCodeListRepository>(); }
        }

        public ITypeListRepository TypeList
        {
            get { return GetRepo<ITypeListRepository>(); }
        }

        public IContactTypeListRepository ContactTypeList
        {
            get { return GetRepo<IContactTypeListRepository>(); }
        }

        public IStateListRepository StateList
        {
            get { return GetRepo<IStateListRepository>(); }
        }

        public IStatusRepository Status
        {
            get { return GetRepo<IStatusRepository>(); }
        }

        public IEmailRepository Email
        {
            get { return GetRepo<IEmailRepository>(); }
        }

        public IContactRepository Contact
        {
            get { return GetRepo<IContactRepository>(); }
        }

        public IStatusListRepository StatusList
        {
            get { return GetRepo<IStatusListRepository>(); }
        }

        public IStatusReasonListRepository StatusReasonList
        {
            get { return GetRepo<IStatusReasonListRepository>(); }
        }

        public IBillSettingRepository BillSetting
        {
            get { return GetRepo<IBillSettingRepository>(); }
        }

        public ICountyTaxAuthorityListRepository CountyTaxAuthorityList
        {
            get { return GetRepo<ICountyTaxAuthorityListRepository>(); }
        }

        public IPayTypeListRepository PayTypeList
        {
            get { return GetRepo<IPayTypeListRepository>(); }
        }

        public IARStatuRepository ARStatu
        {
            get { return GetRepo<IARStatuRepository>(); }
        }
        public IARStatusReasonListRepository ARStatusReasonList
        {
            get { return GetRepo<IARStatusReasonListRepository>(); }
        }
        public IContractRepository Contract
        {
            get { return GetRepo<IContractRepository>(); }
        }

        public IDistributionRepository Distribution
        {
            get { return GetRepo<IDistributionRepository>(); }
        }

        public IFinderFeeRepository FinderFee
        {
            get { return GetRepo<IFinderFeeRepository>(); }
        }

        public IContractStatusListRepository ContractStatusList
        {
            get { return GetRepo<IContractStatusListRepository>(); }
        }
        public ICustomerLogRepository CustomerLog
        {
            get { return GetRepo<ICustomerLogRepository>(); }
        }

        public IContractDetailRepository ContractDetail
        {
            get { return GetRepo<IContractDetailRepository>(); }
        }
        //public IContractDetailDescriptionRepository ContractDetailDescription
        //{
        //    get { return GetRepo<IContractDetailDescriptionRepository>(); }
        //}
        public IFrequencyListRepository FrequencyList
        {
            get { return GetRepo<IFrequencyListRepository>(); }
        }
        public IServiceTypeListRepository ServiceTypeList
        {
            get { return GetRepo<IServiceTypeListRepository>(); }
        }

        public IContractTypeListRepository ContractTypeList
        {
            get { return GetRepo<IContractTypeListRepository>(); }
        }



        public IFranchiseeRepository Franchisee
        {
            get { return GetRepo<IFranchiseeRepository>(); }
        }
        public IFranchiseeBillSettingsRepository FranchiseeBillSettings
        {
            get { return GetRepo<IFranchiseeBillSettingsRepository>(); }
        }
        public IIdentifierTypeListRepository IdentifierTypeList
        {
            get { return GetRepo<IIdentifierTypeListRepository>(); }
        }
        public IACHBankRepository ACHBank
        {
            get { return GetRepo<IACHBankRepository>(); }
        }
        public IFranchiseeFullfillmentRepository FranchiseeFullfillment
        {
            get { return GetRepo<IFranchiseeFullfillmentRepository>(); }
        }
        public IFranchiseeContractRepository FranchiseeContract
        {
            get { return GetRepo<IFranchiseeContractRepository>(); }
        }
        public IFranchiseeContractTypeListRepository FranchiseeContractTypeList
        {
            get { return GetRepo<IFranchiseeContractTypeListRepository>(); }
        }
        public IFranchiseeFeeRepository FranchiseeFee
        {
            get { return GetRepo<IFranchiseeFeeRepository>(); }
        }
        public IFeesRepository Fees
        {
            get { return GetRepo<IFeesRepository>(); }
        }
        public ICusFeesRepository CusFees
        {
            get { return GetRepo<ICusFeesRepository>(); }
        }
        public IFeeRateRepository FeeRate
        {
            get { return GetRepo<IFeeRateRepository>(); }
        }


        public ILedgerAcctRepository LedgerAcct
        {
            get { return GetRepo<ILedgerAcctRepository>(); }
        }
        public ILedgerSubAcctRepository LedgerSubAcct
        {
            get { return GetRepo<ILedgerSubAcctRepository>(); }
        }
        public IGeneralLedgerRepository GeneralLedger
        {
            get { return GetRepo<IGeneralLedgerRepository>(); }
        }
        public IGLAccountTypeListRepository GLAccountTypeList
        {
            get { return GetRepo<IGLAccountTypeListRepository>(); }
        }
        public IMasterTrxRepository MasterTrx
        {
            get { return GetRepo<IMasterTrxRepository>(); }
        }
        public IMasterTrxTypeListRepository MasterTrxTypeList
        {
            get { return GetRepo<IMasterTrxTypeListRepository>(); }
        }
        public IMasterTrxStatusListRepository MasterTrxStatusList
        {
            get { return GetRepo<IMasterTrxStatusListRepository>(); }
        }
        public IMasterTrxDetailRepository MasterTrxDetail
        {
            get { return GetRepo<IMasterTrxDetailRepository>(); }
        }
        //public IMasterTrxDetailDescriptionRepository MasterTrxDetailDescription
        //{
        //    get { return GetRepo<IMasterTrxDetailDescriptionRepository>(); }
        //}
        public IMasterTrxTaxRepository MasterTrxTax
        {
            get { return GetRepo<IMasterTrxTaxRepository>(); }
        }
        public IInvoiceRepository Invoice
        {
            get { return GetRepo<IInvoiceRepository>(); }
        }
        public IInvoiceTypeListRepository InvoiceTypeList
        {
            get { return GetRepo<IInvoiceTypeListRepository>(); }
        }
        public IInvoiceMessageRepository InvoiceMessage
        {
            get { return GetRepo<IInvoiceMessageRepository>(); }
        }

        public IPaymentRepository Payment
        {
            get { return GetRepo<IPaymentRepository>(); }
        }

        public IPaymentMethodListRepository PaymentMethodList
        {
            get { return GetRepo<IPaymentMethodListRepository>(); }
        }

        public ICreditRepository Credit
        {
            get { return GetRepo<ICreditRepository>(); }
        }
        public ICreditReasonListRepository CreditReasonList
        {
            get { return GetRepo<ICreditReasonListRepository>(); }
        }

        public IRegionConfigurationRepository RegionConfiguration
        {
            get { return GetRepo<IRegionConfigurationRepository>(); }
        }


        public IFeeConfigurationRepository FranchiseeFeeConfiguration
        {
            get { return GetRepo<IFeeConfigurationRepository>(); }
        }

        public IAccountingFeeRebateRepository AccountingFeeRebate
        {
            get { return GetRepo<IAccountingFeeRebateRepository>(); }
        }

        public IFranchiseeFeeListRepository FranchiseeFeeList
        {
            get { return GetRepo<IFranchiseeFeeListRepository>(); }
        }
        public IFeeRateTypeListRepository FeeRateTypeList
        {
            get { return GetRepo<IFeeRateTypeListRepository>(); }
        }



        //public ImstrRegionRepository mstrRegion
        //{
        //    get { return GetRepo<ImstrRegionRepository>(); }
        //}

        public IAccountTypeListRepository AccountTypeList
        {
            get { return GetRepo<IAccountTypeListRepository>(); }
        }

        //public IContractTermListRepository ContractTermList
        //{
        //    get { return GetRepo<IContractTermListRepository>(); }
        //}


        public IContractStatusReasonListRepository ContractStatusReasonList
        {
            get { return GetRepo<IContractStatusReasonListRepository>(); }
        }

        public IBankRepository Bank
        {
            get { return GetRepo<IBankRepository>(); }
        }

        public ILeaseRepository Lease
        {
            get { return GetRepo<ILeaseRepository>(); }
        }
        public IAuthUserLoginRepository AuthUserLogin
        {
            get
            {
                return GetRepo<IAuthUserLoginRepository>();
            }
        }
        public IAgreementTypeListRepository AgreementTypeList
        {
            get { return GetRepo<IAgreementTypeListRepository>(); }
        }

        public IValidationItemRepository ValidationItem
        {
            get { return GetRepo<IValidationItemRepository>(); }
        }

        public IValidationRepository Validation
        {
            get { return GetRepo<IValidationRepository>(); }
        }

        #region Coporate Accounting Company 
        public ICompanyRepository Company
        {           
           get { return GetRepo <ICompanyRepository>(); }            
        }

        #endregion

        #region CRM RepositoryUOW
        //public Itbl_CRM_AccountTypeRepository CRMAccountType
        //{
        //    get { return GetRepo<Itbl_CRM_AccountTypeRepository>(); }
        //}
        //public Itbl_CRM_ActivityRepository CRMActivity
        //{
        //    get { return GetRepo<Itbl_CRM_ActivityRepository>(); }
        //}
        //public Itbl_CRM_CurrentProviderRepository CRMCurrentProvider
        //{
        //    get { return GetRepo<Itbl_CRM_CurrentProviderRepository>(); }
        //}
        //public Itbl_CRM_LeadRepository CRMLead
        //{
        //    get { return GetRepo<Itbl_CRM_LeadRepository>(); }
        //}
        //public Itbl_CRM_LeadSourceRepository CRMLeadSource
        //{
        //    get { return GetRepo<Itbl_CRM_LeadSourceRepository>(); }
        //}
        //public Itbl_CRM_LeadStatusRepository CRMLeadStatus
        //{
        //    get { return GetRepo<Itbl_CRM_LeadStatusRepository>(); }
        //}
        //public Itbl_CRM_TerritoryRepository CRMTerritory
        //{
        //    get { return GetRepo<Itbl_CRM_TerritoryRepository>(); }
        //}

        public ICRM_ActivityRepository CRM_Activity
        {
            get { return GetRepo<ICRM_ActivityRepository>(); }
        }
        public ICRM_TimeLineRepository CRM_TimeLine
        {
            get { return GetRepo<ICRM_TimeLineRepository>(); }
        }
        public ICRM_AccountRepository CRM_Account
        {
            get { return GetRepo<ICRM_AccountRepository>(); }
        }
        public ICRM_TaskRepository CRM_Task
        {
            get { return GetRepo<ICRM_TaskRepository>(); }
        }
        public ICRM_TaskTypeRepository CRM_TaskType
        {
            get { return GetRepo<ICRM_TaskTypeRepository>(); }
        }
        public ICRM_QuotationRepository CRM_Quotation
        {
            get { return GetRepo<ICRM_QuotationRepository>(); }
        }
        public ICRM_ScheduleRepository CRM_Schedule
        {
            get { return GetRepo<ICRM_ScheduleRepository>(); }
        }
        public ICRM_NoteRepository CRM_Note
        {
            get { return GetRepo<ICRM_NoteRepository>(); }
        }
        public IAuthDepartmentRepository AuthDepartment
        {
            get { return GetRepo<IAuthDepartmentRepository>(); }
        }
        public ICRM_StageRepository CRM_Stage
        {
            get { return GetRepo<ICRM_StageRepository>(); }
        }
        public ICRM_StageStatusRepository CRM_StageStatus
        {
            get { return GetRepo<ICRM_StageStatusRepository>(); }
        }
        public ICRM_IndustryTypeRepository CRM_IndustryType
        {
            get { return GetRepo<ICRM_IndustryTypeRepository>(); }
        }
        public ICRM_ProviderSourceRepository CRM_ProviderSource
        {
            get { return GetRepo<ICRM_ProviderSourceRepository>(); }
        }
        public ICRM_ProviderTypeRepository CRM_ProviderType
        {
            get { return GetRepo<ICRM_ProviderTypeRepository>(); }
        }
        public ICRM_AccountCustomerDetailRepository CRM_AccountCustomerDetail
        {
            get { return GetRepo<ICRM_AccountCustomerDetailRepository>(); }
        }
        public ICRM_AccountFranchiseDetailRepository CRM_AccountFranchiseDetail
        {
            get { return GetRepo<ICRM_AccountFranchiseDetailRepository>(); }
        }
        public ICRM_AccountTypeRepository CRM_AccountType
        {
            get { return GetRepo<ICRM_AccountTypeRepository>(); }
        }
        public ICRM_ActivityOutcomeTypeRepository CRM_ActivityOutcomeType
        {
            get { return GetRepo<ICRM_ActivityOutcomeTypeRepository>(); }
        }
        public ICRM_ActivityTypeRepository CRM_ActivityType
        {
            get { return GetRepo<ICRM_ActivityTypeRepository>(); }
        }
        public ICRM_TimeLineTypeRepository CRM_TimeLineType
        {
            get { return GetRepo<ICRM_TimeLineTypeRepository>(); }
        }

        public ICRM_DocumentRepository CRM_Document
        {
            get { return GetRepo<ICRM_DocumentRepository>(); }
        }

        public ICRM_InitialCommunicationRepository CRM_InitialCommunication
        {
            get { return GetRepo<ICRM_InitialCommunicationRepository>(); }
        }

        public ICRM_FvPresentationRepository CRM_FvPresentation
        {
            get { return GetRepo<ICRM_FvPresentationRepository>(); }
        }

        public ICRM_BiddingRepository CRM_Bidding
        {
            get { return GetRepo<ICRM_BiddingRepository>(); }
        }
        public ICRM_PdAppointmentRepository CRM_PdAppointment
        {
            get { return GetRepo<ICRM_PdAppointmentRepository>(); }
        }
        public ICRM_FollowUpRepository CRM_FollowUp
        {
            get { return GetRepo<ICRM_FollowUpRepository>(); }
        }
        public ICRM_CloseRepository CRM_Close
        {
            get { return GetRepo<ICRM_CloseRepository>(); }
        }

        public ICRM_StageStatusScheduleRepository CRM_StageStatusSchedule
        {
            get { return GetRepo<ICRM_StageStatusScheduleRepository>(); }
        }
        public ICRM_LeadGenerationRepository CRM_LeadGeneration
        {
            get { return GetRepo<ICRM_LeadGenerationRepository>(); }
        }

        public ICRM_ContactRepository CRM_Contact
        {
            get { return GetRepo<ICRM_ContactRepository>(); }
        }
        public ICRM_ContactTypeRepository CRM_ContactType
        {
            get { return GetRepo<ICRM_ContactTypeRepository>(); }
        }

        public ICRM_FranchiseContractRepository CRM_FranchiseContract
        {
            get { return GetRepo<ICRM_FranchiseContractRepository>(); }
        }

        public IIdentificationRepository Identification
        {
            get { return GetRepo<IIdentificationRepository>(); }
        }
        public ICRMCloseTempDocumentRepository CRM_CloseTempDocument
        {
            get { return GetRepo<ICRMCloseTempDocumentRepository>(); }
        }
        public ICRMTerritoryRepository CRM_Territory
        {
            get { return GetRepo<ICRMTerritoryRepository>(); }
        }
        public ICRMTerritoryAssignmentRepository CRM_Territory_Assignment
        {
            get { return GetRepo<ICRMTerritoryAssignmentRepository>(); }
        }
        public ICRMTerritoryNewRepository CRM_Territory_New
        {
            get { return GetRepo<ICRMTerritoryNewRepository>(); }
        }
        public ICRMTerritoryAssignmentNewRepository CRM_Territory_Assignment_New
        {
            get { return GetRepo<ICRMTerritoryAssignmentNewRepository>(); }
        }
        public ICRMSalesTerritoryAssignmentRepository CRM_SalesTerritory_Assignment
        {
            get { return GetRepo<ICRMSalesTerritoryAssignmentRepository>(); }
        }
        public ICRM_ScheduleTypeRepository CRM_ScheduleType
        {
            get { return GetRepo<ICRM_ScheduleTypeRepository>(); }
        }
        public ICRM_CallResultRepository CRM_CallResult
        {
            get { return GetRepo<ICRM_CallResultRepository>(); }
        }
        public ICRM_NoteTypeRepository CRM_NoteType
        {
            get { return GetRepo<ICRM_NoteTypeRepository>(); }
        }
        public ICRM_SalePossibilityTypeRepository CRM_SalePossibilityType
        {
            get { return GetRepo<ICRM_SalePossibilityTypeRepository>(); }
        }

        public ICRM_FranchiseFollowUpRepository CRM_FranchiseFollowUp
        {
            get { return GetRepo<ICRM_FranchiseFollowUpRepository>(); }
        }
        public ICRM_SignAgreementRepository CRM_SignAgreement
        {
            get { return GetRepo<ICRM_SignAgreementRepository>(); }
        }

        public ICRM_PurposeTypeRepository CRM_PurposeType
        {
            get { return GetRepo<ICRM_PurposeTypeRepository>(); }
        }
        public ICRM_ReasonTypeRepository CRM_ReasonType
        {
            get { return GetRepo<ICRM_ReasonTypeRepository>(); }
        }
        public ICRM_CloseTypeRepository CRM_CloseType
        {
            get { return GetRepo<ICRM_CloseTypeRepository>(); }
        }
        public ICRM_CallLogRepository CRM_CallLog
        {
            get { return GetRepo<ICRM_CallLogRepository>(); }
        }
        #endregion

        #region JKEfUow > Dynamic Forms

        public IFormItemTemplateRepository FormItemTemplateRepository => GetRepo<IFormItemTemplateRepository>();
        public IFormItemTypeRepository FormItemTypeRepository => GetRepo<IFormItemTypeRepository>();
        public IFormTemplateRepository FormTemplateRepository => GetRepo<IFormTemplateRepository>();
        public IFormTemplateTypeRepository FormTemplateTypeRepository => GetRepo<IFormTemplateTypeRepository>();

        #endregion

        #region Distribution

        public IDistributionRepository DistributionRepository => GetRepo<IDistributionRepository>();

        #endregion

        #region Inspection

        //public IInspectionRepository InspectionRepository => GetRepo<IInspectionRepository>();
        public IInspectionFormRepository InspectionFormRepository => GetRepo<IInspectionFormRepository>();
        public IInspectionFormItemRepository InspectionFormItemRepository => GetRepo<IInspectionFormItemRepository>();
        public IInspectionStatusRepository InspectionStatusRepository => GetRepo<IInspectionStatusRepository>();

        public ITransactionNumberConfigRepository TransactionNumberConfig
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        public ICSActivityRepository CSActivity
        {
            get { return GetRepo<ICSActivityRepository>(); }
        }
        public ICSstageRepository CSstage
        {
            get { return GetRepo<ICSstageRepository>(); }
        }

        public IFranchisee_TempRepository Franchisee_Temp
        {
            get { return GetRepo<IFranchisee_TempRepository>(); }
        }
        public IAddress_TempRepository Address_Temp
        {
            get { return GetRepo<IAddress_TempRepository>(); }
        }
        public IPhone_TempRepository Phone_Temp
        {
            get { return GetRepo<IPhone_TempRepository>(); }
        }
        public IEmail_TempRepository Email_Temp
        {
            get { return GetRepo<IEmail_TempRepository>(); }
        }
        public IContact_TempRepository Contact_Temp
        {
            get { return GetRepo<IContact_TempRepository>(); }
        }
        public IFranchiseeBillSettings_TempRepository FranchiseeBillSettings_Temp
        {
            get { return GetRepo<IFranchiseeBillSettings_TempRepository>(); }
        }
        public IIdentification_TempRepository Identification_Temp
        {
            get { return GetRepo<IIdentification_TempRepository>(); }
        }
        public IACHBank_TempRepository ACHBank_Temp
        {
            get { return GetRepo<IACHBank_TempRepository>(); }
        }
        public IFranchiseeContract_TempRepository FranchiseeContract_Temp
        {
            get { return GetRepo<IFranchiseeContract_TempRepository>(); }
        }
        public IFranchiseeFee_TempRepository FranchiseeFee_Temp
        {
            get { return GetRepo<IFranchiseeFee_TempRepository>(); }
        }

        public IFeeConfiguration_TempRepository FeeConfiguration_Temp
        {
            get { return GetRepo<IFeeConfiguration_TempRepository>(); }
        }
        public IFranchiseeFullfillment_TempRepository FranchiseeFullfillment_Temp
        {
            get { return GetRepo<IFranchiseeFullfillment_TempRepository>(); }
        }

        public IFranchiseeOwnerListRepository FranchiseeOwnerList
        {
            get { return GetRepo<IFranchiseeOwnerListRepository>(); }
        }

        public IFranchiseeOwnerList_TempRepository FranchiseeOwnerList_Temp
        {
            get { return GetRepo<IFranchiseeOwnerList_TempRepository>(); }
        }

        public ICSAccountWalkThursFormFieldRepository CSAccountWalkThursFormField
        {
            get { return GetRepo<ICSAccountWalkThursFormFieldRepository>(); }
        }

        public ICSAccountWalkThursFormFieldDetailRepository CSAccountWalkThursFormFieldDetail
        {
            get { return GetRepo<ICSAccountWalkThursFormFieldDetailRepository>(); }
        }


        public ICMR_StageStartEndRepository CMR_StageStartEnd => GetRepo<ICMR_StageStartEndRepository>();

        private T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        protected void CreateDbContext()
        {

            DbContext = new jkDatabaseEntities();

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = true;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = true;

            //DbContext.Configuration.ValidateOnSaveEnabled = true;

            //DbContext.Configuration.AutoDetectChangesEnabled = true;

        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion

    }
}
