using JK.Repository.Contracts;

namespace JK.Repository.Uow
{
    public interface IJKEfUow
    {
        // Save pending changes to the data store.
        void Commit();

        // Repositories   

        ICustomerRepository Customer { get; }
        IServiceCallLogRepository ServiceCallLog { get; }
       // ICallLogAssociateRepository CallLogAssociate { get; }
        //ICollectionsCallLogRepository CollectionsCallLog { get; }
        IPhoneRepository Phone { get; }
        IAddressRepository Address { get; }
        ICountryCodeListRepository CountryCodeList { get; }
        ITypeListRepository TypeList { get; }
        IContactTypeListRepository ContactTypeList { get; }
        IStateListRepository StateList { get; }
        IStatusRepository Status { get; }
        IEmailRepository Email { get; }
        IContactRepository Contact { get; }
        IStatusListRepository StatusList { get; }
        IStatusReasonListRepository StatusReasonList { get; }
        IBillSettingRepository BillSetting { get; }
        ICountyTaxAuthorityListRepository CountyTaxAuthorityList { get; }
        IPayTypeListRepository PayTypeList { get; }
        IARStatuRepository ARStatu { get; }
        IARStatusReasonListRepository ARStatusReasonList { get; }
        IContractRepository Contract { get; }
        IContractStatusListRepository ContractStatusList { get; }
        ICustomerLogRepository CustomerLog { get; }
        IContractDetailRepository ContractDetail { get; }
        //IContractDetailDescriptionRepository ContractDetailDescription { get; }
        IFrequencyListRepository FrequencyList { get; }
        IServiceTypeListRepository ServiceTypeList { get; }
        IContractTypeListRepository ContractTypeList { get; }
        IFranchiseeRepository Franchisee { get; }
        IFranchiseeBillSettingsRepository FranchiseeBillSettings { get; }
        IIdentifierTypeListRepository IdentifierTypeList { get; }
        IACHBankRepository ACHBank { get; }
        IFranchiseeFullfillmentRepository FranchiseeFullfillment { get; }
        IFranchiseeContractRepository FranchiseeContract { get; }
        IFranchiseeContractTypeListRepository FranchiseeContractTypeList { get; }
        IFranchiseeFeeRepository FranchiseeFee { get; }
        IFeeConfigurationRepository FranchiseeFeeConfiguration { get; }
        IAccountingFeeRebateRepository AccountingFeeRebate { get; }
        IFeesRepository Fees { get; }
        ICusFeesRepository CusFees { get; }
        IFeeRateRepository FeeRate { get; }
        IIdentificationRepository Identification { get; }
        IAccountTypeListRepository AccountTypeList { get; }
        //IContractTermListRepository ContractTermList { get; }
        IContractStatusReasonListRepository ContractStatusReasonList { get; }
        IRegionConfigurationRepository RegionConfiguration { get; }
        IBankRepository Bank { get; }
        ILeaseRepository Lease { get; }
        IDistributionRepository Distribution { get; }
        IAuthUserLoginRepository AuthUserLogin { get; }
        IAuthDepartmentRepository AuthDepartment { get; }

        IAgreementTypeListRepository AgreementTypeList { get; }
        IFranchiseeOwnerListRepository FranchiseeOwnerList { get; }
        IValidationItemRepository ValidationItem { get; }
        IValidationRepository Validation { get; }
        IFinderFeeRepository FinderFee { get; }
        #region CustomerInvoice
        ILedgerAcctRepository LedgerAcct { get; }
        ILedgerSubAcctRepository LedgerSubAcct { get; }
        IGeneralLedgerRepository GeneralLedger { get; }
        IGLAccountTypeListRepository GLAccountTypeList { get; }
        IMasterTrxRepository MasterTrx { get; }
        IMasterTrxTypeListRepository MasterTrxTypeList { get; }
        IMasterTrxStatusListRepository MasterTrxStatusList { get; }
        IMasterTrxDetailRepository MasterTrxDetail { get; }
        //IMasterTrxDetailDescriptionRepository MasterTrxDetailDescription { get; }
        IMasterTrxTaxRepository MasterTrxTax { get; }
        IInvoiceRepository Invoice { get; }
        IInvoiceTypeListRepository InvoiceTypeList { get; }
        IInvoiceMessageRepository InvoiceMessage { get; }
        ITransactionNumberConfigRepository TransactionNumberConfig { get; }
        #endregion

        #region CustomerInvoivePayment
        IPaymentRepository Payment { get; }
        IPaymentMethodListRepository PaymentMethodList { get; }

        #endregion

        #region CustomerInvoiceCredit
        ICreditRepository Credit { get; }

        ICreditReasonListRepository CreditReasonList { get; }

        #endregion

        #region Coporate Accounting Company 
        ICompanyRepository Company { get; }

       #endregion

        #region FranchiseeFee

        IFranchiseeFeeListRepository FranchiseeFeeList { get; }
        IFeeRateTypeListRepository FeeRateTypeList { get; }

        #endregion

        #region Region

       // ImstrRegionRepository mstrRegion { get; }



        #endregion         

        #region CRM RepositoryUOW

        ICRM_ActivityRepository CRM_Activity { get; }
        ICRM_AccountRepository CRM_Account { get; }
        ICRM_AccountCustomerDetailRepository CRM_AccountCustomerDetail { get; }
        ICRM_AccountFranchiseDetailRepository CRM_AccountFranchiseDetail { get; }
        ICRM_TimeLineRepository CRM_TimeLine { get; }
        ICRM_NoteRepository CRM_Note { get; }
        ICRM_ScheduleRepository CRM_Schedule { get; }
        ICRM_QuotationRepository CRM_Quotation { get; }
        ICRM_TaskRepository CRM_Task { get; }
        ICRM_TaskTypeRepository CRM_TaskType { get; }
        ICRM_StageRepository CRM_Stage { get; }
        ICRM_StageStatusRepository CRM_StageStatus { get; }
        ICRM_IndustryTypeRepository CRM_IndustryType { get; }
        ICRM_ProviderSourceRepository CRM_ProviderSource { get; }
        ICRM_ProviderTypeRepository CRM_ProviderType { get; }
        ICRM_AccountTypeRepository CRM_AccountType { get; }
        ICRM_ActivityOutcomeTypeRepository CRM_ActivityOutcomeType { get; }
        ICRM_ActivityTypeRepository CRM_ActivityType { get; }
        ICRM_TimeLineTypeRepository CRM_TimeLineType { get; }
        ICRM_DocumentRepository CRM_Document { get; }
        ICRM_InitialCommunicationRepository CRM_InitialCommunication { get; }
        ICRM_FvPresentationRepository CRM_FvPresentation { get; }
        ICRM_BiddingRepository CRM_Bidding { get; }
        ICRM_PdAppointmentRepository CRM_PdAppointment { get; }
        ICRM_FollowUpRepository CRM_FollowUp { get; }
        ICRM_CloseRepository CRM_Close { get; }
        ICRM_StageStatusScheduleRepository CRM_StageStatusSchedule { get; }
        ICRM_LeadGenerationRepository CRM_LeadGeneration { get; }
        ICRM_ContactRepository CRM_Contact { get; }
        ICRM_ContactTypeRepository CRM_ContactType { get; }
        ICRMCloseTempDocumentRepository CRM_CloseTempDocument { get; }
        ICRMTerritoryRepository CRM_Territory { get; }
        ICRMTerritoryNewRepository CRM_Territory_New { get; }
        ICRMTerritoryAssignmentNewRepository CRM_Territory_Assignment_New { get; }
        ICRMTerritoryAssignmentRepository CRM_Territory_Assignment { get; }
        ICRMSalesTerritoryAssignmentRepository CRM_SalesTerritory_Assignment { get; }
        ICRM_ScheduleTypeRepository CRM_ScheduleType { get; }
        ICRM_FranchiseContractRepository CRM_FranchiseContract { get; }
        ICRM_CallResultRepository CRM_CallResult { get; }
        ICRM_NoteTypeRepository CRM_NoteType { get; }
        ICRM_SalePossibilityTypeRepository CRM_SalePossibilityType { get; }
        ICRM_FranchiseFollowUpRepository CRM_FranchiseFollowUp { get; }
        ICRM_SignAgreementRepository CRM_SignAgreement { get; }
        ICRM_PurposeTypeRepository CRM_PurposeType { get; }
        ICRM_ReasonTypeRepository CRM_ReasonType { get; }
        ICRM_CloseTypeRepository CRM_CloseType { get; }
        ICRM_CallLogRepository CRM_CallLog { get; }

        #endregion

        #region Dynamic Forms

        IFormItemTemplateRepository FormItemTemplateRepository { get; }
        IFormItemTypeRepository FormItemTypeRepository { get; }
        IFormTemplateRepository FormTemplateRepository { get; }
        IFormTemplateTypeRepository FormTemplateTypeRepository { get; }

        #endregion

        #region Distribution

        IDistributionRepository DistributionRepository { get; }

        #endregion

        #region Inspection

        //IInspectionRepository InspectionRepository { get; }
        IInspectionFormRepository InspectionFormRepository { get; }
        IInspectionFormItemRepository InspectionFormItemRepository { get; }
        IInspectionStatusRepository InspectionStatusRepository { get; }

        #endregion

        ICMR_StageStartEndRepository CMR_StageStartEnd { get; }

        ICSActivityRepository CSActivity { get; }
        ICSstageRepository CSstage { get; }

        IFranchisee_TempRepository Franchisee_Temp { get; }
        IAddress_TempRepository Address_Temp { get; }
        IPhone_TempRepository Phone_Temp { get; }
        IEmail_TempRepository Email_Temp { get; }
        IContact_TempRepository Contact_Temp { get; }
        IFranchiseeBillSettings_TempRepository FranchiseeBillSettings_Temp { get; }
        IIdentification_TempRepository Identification_Temp { get; }
        IACHBank_TempRepository ACHBank_Temp { get; }
        IFranchiseeContract_TempRepository FranchiseeContract_Temp { get; }        
        IFranchiseeFee_TempRepository FranchiseeFee_Temp { get; }
        IFeeConfiguration_TempRepository FeeConfiguration_Temp { get; }
        IFranchiseeFullfillment_TempRepository FranchiseeFullfillment_Temp { get; }
        IFranchiseeOwnerList_TempRepository FranchiseeOwnerList_Temp { get; }
        ICSAccountWalkThursFormFieldRepository CSAccountWalkThursFormField { get; }
        ICSAccountWalkThursFormFieldDetailRepository CSAccountWalkThursFormFieldDetail { get; }
    }
}
