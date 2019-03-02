using JKApi.Data.DAL;
using JKApi.Data.DTOObject;
using JKApi.Service.Service.Customer;
using JKViewModels.AccountReceivable;
using JKViewModels.Common;
using JKViewModels.CRM;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Franchisee;
using JKViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JKApi.Service.ServiceContract.Customer
{
    public interface ICustomerService
    {
        IQueryable<JKApi.Data.DAL.Customer> GetCustomers();
        List<JKApi.Data.DAL.Customer> GetCustomersList();
        List<CustomerUploadDocumentViewModel> GetCUploadDocument(int classid, int typelistid);
        bool SaveUploadDocument(int _ClassId, int _TypeListId, int _FileTypeListId, string _FilePath, string _FileName, string _FileExt, int _FileSize, bool IsNew = false);
        UploadDocument GetUploadDocumenwithFileType(int CustomerId, int FileTypeListId, int TypeListId);
        void DeleteUploadDocument(int id);
        UploadDocument GetUploadDocumentById(int id);

        //Dictionary<string, string> _GetStatusList();

        //Dictionary<string, string> GetUsStatesList();

        List<ARStatu> getARStatusList();

        List<InvoiceDateList> getInvoiceDate();
        List<InvoiceTermList> getTermDate();

        //Dictionary<string, string> GetCAccountType();

        //Dictionary<string, string> _GetFrequencyList();

        CustomerViewModel loadCustomer(int id);

        //List<Data.JkControl.vw_sys_State> GetStatesList();
        CustomerServiceQuickActionViewModel GetCustomerServiceQuickAction(int selectedRegion);
        IEnumerable<CustomerCollection> GetCustomerByStatus(int StatusId, int TypeListId, int ContactTypeListId);
        List<CustomerSearchResultViewModel> GetCustomerSearchList(int StatusId, int ContactTypeListId, string StatusListIds = "");
        List<CustomerSearchResultViewModel> GetCustomerSearchList(int contactTypeListId, string statusListIds = "", string regionId = "");
        List<CustomerPendingMaintenanceListViewModel> GetCustomerPendingMaintenanceList(string selectedRegion);
        FullCustomerViewModel GetCustomerDetailsByIdWithActiveData(int id);
        List<StatusList> GetAll_StatusList();
        List<MaintenanceTypeList> GetAll_MaintenanceTypeList(int typeListid);

        List<ServiceCallLog> ServiceCallLogList(int Id, DateTime? startDate, DateTime? endDate, int month, int year);

        List<StatusList> GetAll_StatusListByTypeListId(int TypeListId);
        List<StatusReasonList> GetAll_StatusReasonList(int TypeListId);
        JKApi.Data.DAL.Customer GetCustomerById(int id);

        JKApi.Data.DAL.Customer SaveCustomers(JKApi.Data.DAL.Customer Customer);
        Email SaveEBill_Emails(Email Email);

        void CreateCustomerInvoice(int customerId, int regionId, int userId);
        void UpdateCustomerInactive(int customerId, int userId);

        void DeleteCustomer(int id);

        bool CheckCustomerIsExist(string Name);

        
        int CheckCustomerNamePhoneIsExist(string Name, string Phone, string APISelectedRegionId="");

        int CheckOnlyCustomerNamePhoneIsExist(string Name, string Phone);
        int CheckFranchiseeNamePhoneIsExist(string Name, string Phone);
        FullCustomerViewModel GetCustomerDetailsById(int id);

        List<SearchDateList> GetAll_OptionList();
        List<FranchiseeTypeList> GetAll_FranchiseeTypeList();
        CustomerMaintenanceViewModel GetCustomerMaintenanceDetailsById(int id);

        FullCustomerViewModel GetCustomerDetailsByIdWithActive(int id);

        string getCustomerNo(int RegionId);
        CustomerMaintenanceApproval CustomerMaintenanceApprovalByTempMaintenanceID(int MasterCustomerTransferTempId);
        List<ARInvoiceListViewModel> GetInvoiceListWithSearchForPayment(int customerId, string OpenClose);

        List<portal_spGet_AP_TaxRateAPI_Result> GetTaxrateList();
        List<Address> GetAddressList(int classId, int typelistId);
        List<portal_spGet_AP_TaxRateAPI_Result> GetTaxrateList(int classId, int typelistId);
        RegionSetting SaveRegionConfiguration(RegionSetting RegionSetting);

        RegionSetting GetRegionConfigurationbyId(int id,int RegionId);
        string GetStatesName(int id);
        RemitToViewModel GetRemitToForRegion(int regionId);
        IQueryable<JKApi.Data.DAL.Customer> GetSearchCustomers();
        List<JKApi.Data.DAL.Customer> GetCustomerListData(string searchtext);

        List<CustomerSearchModel> GetSearchCustomer(string searchText);
        string hasExistManintenance(int customerid);
        List<portal_spGet_C_CancellationInvoiceList_Result> GetCancellationInvoiceList(int customerId, int month, int year);

        bool InsertUpdateCustomerMaintenance(CustomerMaintenanceViewModel inputData);
        int InsertUpdateCustomerMaintenanceTemp(CustomerMaintenanceViewModel inputData);

        portal_spGet_C_Distribution_Result GetDistributionDetailData(int customerid);
        IQueryable<MasterTrxTypeList> GetMasterTrxTypeListsForCustomers();

        string GetStateName(int stateid);

        List<DistributionFee> GetDistributionFeeData(int id);
        List<FranchiseeDistributionFeesViewModel> GetFranchiseeDistributionFeesData(int FranchiseeId, int ContractDetailId, int DistributionId = 0);
        bool SaveFranchiseeDistributionFee(List<FranchiseeDistributionFeesViewModel> lstFranchiseeDistributionFee);
        List<vw_Fee> GetFeeData();

        List<ServiceCallLog> GetServiceLog(int id);
        ServiceCallLog GetServiceCallLogById(int Id);
        string GetStatus(int? id);
        List<CollectionsCallLog> GetCollectionLog(int id);
        List<CheckBookTransactionTypeList> GetManualCheckBookTransactionTypeList();
        //List<Data.DAL.Region> GetRegionList();
        List<ARInvoiceListViewModel> GetInvoiceListWithSearchForCredit(int _customerId);
        List<CreditReasonList> GetAll_ReasonList();

        IQueryable<JKApi.Data.DAL.Address> GetAddressByCustomerId(int id);

        void savePendingMessage(string message, int customerID, int status);

        #region Company
        List<BankTypeList> GetBankTypeList();
        List<Bank> GetBanksForRegion();
        List<Bank> GetBanksForRegion(int regionId);
        #endregion

        #region PhoneCalls

        IQueryable<Phone> GetPhone();
        IQueryable<Phone_Temp> GetPhoneTemp();

        Phone GetPhoneById(int id);

        Phone SavePhone(Phone Phone);
        Phone_Temp SavePhone_Temp(Phone_Temp Phone_Temp);

        void DeletePhone(int id);

        Phone AddNewPhoneOldInactive(Phone Phone);
        Phone_Temp AddNewPhoneOldInactiveTemp(Phone_Temp Phone);

        #endregion

        #region AddressCalls

        List<JKApi.Data.DAL.Address> GetAddressList();

        IQueryable<Address> GetAddress();
        IQueryable<Address_Temp> GetAddressTemp();

        Address GetAddressById(int id);

        Address SaveAddress(Address Address);
        Address_Temp SaveAddress_Temp(Address_Temp Address_Temp);

        void DeleteAddress(int id);

        Address AddNewAddressOldInactive(Address Address);
        Address_Temp AddNewAddressOldInactiveTemp(Address_Temp Address);
        #endregion

        #region CountryCodeList Calls

        IQueryable<CountryCodeList> GetCountryCodeList();

        CountryCodeList GetCountryCodeListById(int id);

        CountryCodeList SaveCountryCodeList(CountryCodeList CountryCodeList);

        CountryCodeList DeleteCountryCodeList(int id);



        #endregion

        #region TypeList Calls

        IQueryable<TypeList> GetTypeList();

        TypeList GetTypeListById(int id);

        TypeList SaveTypeList(TypeList TypeList);

        TypeList DeleteTypeList(int id);



        #endregion

        #region ContactTypeList Calls

        IQueryable<ContactTypeList> GetContactTypeList();

        ContactTypeList GetContactTypeListById(int id);

        ContactTypeList SaveContactTypeList(ContactTypeList ContactTypeList);

        ContactTypeList DeleteContactTypeList(int id);



        #endregion

        #region StateList Calls

        IQueryable<StateList> GetStateList();

        StateList GetStateListById(int id);

        int GetStateId(string state);

        StateList SaveStateList(StateList StateList);

        StateList DeleteStateList(int id);



        #endregion

        #region Status Calls

        IQueryable<Status> GetStatus();

        Status GetStatusById(int id);

        Status SaveStatus(Status Status);

        Status DeleteStatus(int id);



        #endregion

        #region Email Calls

        IQueryable<Email> GetEmail();
        IQueryable<Email_Temp> GetEmailTemp();

        Email GetEmailById(int id);

        Email SaveEmail(Email Email);
        Email_Temp SaveEmail_Temp(Email_Temp Email_Temp);

        Email DeleteEmail(int id);

        Email AddNewEmailOldInactive(Email Email);
        Email_Temp AddNewEmailOldInactiveTemp(Email_Temp Email);

        #endregion

        #region StatusList Calls

        IQueryable<StatusList> GetStatusList();
       
        StatusList GetStatusListById(int id);
        List<TransactionStatusList> GetTrasactionStatusList();

        StatusList SaveStatusList(StatusList StatusList);

        StatusList DeleteStatusList(int id);



        #endregion

        #region Contact Calls

        IQueryable<Contact> GetContact();
        IQueryable<Contact_Temp> GetContactTemp();
        Contact GetContactById(int id);

        Contact SaveContact(Contact Contact);
        Contact_Temp SaveContact_Temp(Contact_Temp Contact_Temp);

        Contact DeleteContact(int id);

        Contact AddNewContactOldInactive(Contact Contact);
        Contact_Temp AddNewContactOldInactiveTemp(Contact_Temp Contact);

        #endregion

        #region StatusReasonList Calls

        IQueryable<StatusReasonList> GetStatusReasonList();

        StatusReasonList GetStatusReasonListById(int id);

        StatusReasonList SaveStatusReasonList(StatusReasonList StatusReasonList);

        StatusReasonList DeleteStatusReasonList(int id);



        #endregion

        #region BillSetting Calls

        IQueryable<BillSetting> GetBillSetting();

        BillSetting GetBillSettingById(int id);

        BillSetting SaveBillSetting(BillSetting BillSetting);

        BillSetting DeleteBillSetting(int id);

        BillSetting AddNewBillSettingOldInactive(BillSetting BillSetting);

        BillSetting GetBillSettingWithCustomer(int CustomerId);
        #endregion

        #region CountyTaxAuthorityList Calls

        IQueryable<CountyTaxAuthorityList> GetCountyTaxAuthorityList();

        CountyTaxAuthorityList GetCountyTaxAuthorityListById(int id);

        CountyTaxAuthorityList SaveCountyTaxAuthorityList(CountyTaxAuthorityList CountyTaxAuthorityList);

        CountyTaxAuthorityList DeleteCountyTaxAuthorityList(int id);



        #endregion

        #region PayTypeList Calls

        IQueryable<PayTypeList> GetPayTypeList();

        PayTypeList GetPayTypeListById(int id);

        PayTypeList SavePayTypeList(PayTypeList PayTypeList);

        PayTypeList DeletePayTypeList(int id);



        #endregion

        #region ARStatu Calls

        IQueryable<ARStatu> GetARStatu();

        ARStatu GetARStatuById(int id);

        ARStatu SaveARStatu(ARStatu ARStatu);

        ARStatu DeleteARStatu(int id);

        ARStatu AddNewARStatuOldInactive(ARStatu ARStatu);

        #endregion

        #region ARStatusReasonList Calls

        IQueryable<ARStatusReasonList> GetARStatusReasonList();

        ARStatusReasonList GetARStatusReasonListById(int id);

        ARStatusReasonList SaveARStatusReasonList(ARStatusReasonList ARStatusReasonList);

        ARStatusReasonList DeleteARStatusReasonList(int id);



        #endregion

        #region Contract Calls

        IQueryable<Data.DAL.Contract> GetContract();

        Data.DAL.Contract GetContractById(int id);

        Data.DAL.Contract SaveContract(Data.DAL.Contract Contract);

        Data.DAL.Contract DeleteContract(int id);

        Data.DAL.Contract GetContractByCustomerId(int CustomerId);

        #endregion

        #region ContractStatusList Calls

        IQueryable<ContractStatusList> GetContractStatusList();

        ContractStatusList GetContractStatusListById(int id);

        ContractStatusList SaveContractStatusList(ContractStatusList ContractStatusList);

        ContractStatusList DeleteContractStatusList(int id);



        #endregion

        #region CustomerLog Calls

        IQueryable<CustomerLog> GetCustomerLog();

        CustomerLog GetCustomerLogById(int id);

        CustomerLog SaveCustomerLog(CustomerLog CustomerLog);

        CustomerLog DeleteCustomerLog(int id);



        #endregion

        #region ContractDetail Calls

        IQueryable<ContractDetail> GetContractDetail();
        List<FileTypeListViewModel> GetFileTypeList(int typelistid);
        ContractDetail GetContractDetailById(int id);

        ContractDetail SaveContractDetail(ContractDetail ContractDetail);

        ContractDetail DeleteContractDetail(int id);
        bool HasContractAndContractDetailByCustomerId(int CustomerId);
        bool HasDocumentByCustomerId(int CustomerId, int statusid);
        IEnumerable<ContractDetail> GetContractDetailResult(FullCustomerViewModel model);

        IEnumerable<ContractDetail> GetContractDetailResultModel(FullCustomerViewModel model, int Id);

        ContractDetail SaveContractDetailbyModel(ContractDetail ContractDetail);



        int UpdateLineNo(int ContractId);
        int getLineNo(int? id);

        #endregion

        #region FrequencyList Calls

        IQueryable<FrequencyList> GetFrequencyList();
        List<CleanFrequencyListViewModel> GetCleanFrequencyList();

        Dictionary<string, string> _GetCleanFrequencyList();
        Dictionary<string, string> _GetAccountTypeList();
        Dictionary<string, string> _GetStateList();

        FrequencyList GetFrequencyListById(int id);

        FrequencyList SaveFrequencyList(FrequencyList FrequencyList);

        FrequencyList DeleteFrequencyList(int id);



        #endregion

        #region ServiceTypeList Calls

        IQueryable<ServiceTypeList> GetServiceTypeList();

        ServiceTypeList GetServiceTypeListById(int id);

        ServiceTypeList SaveServiceTypeList(ServiceTypeList ServiceTypeList);

        ServiceTypeList DeleteServiceTypeList(int id);

        List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogCustomersList(int CustID);

        List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogList();
        bool SaveServiceCallLog(ServiceCallLog serviceCallLogModel);
        bool UpdateServiceCallLogDetails(ServiceCallLog serviceCallLogModel);
        bool ServiceCallLogDetailsUpdatePopup(ServiceCallLog serviceCallLogModel);
        bool SaveCollectionCallLog(CollectionsCallLogModel collectionCallLogModel);
        Dictionary<string, string> getFrancisesByCustomerId(int custId);
        List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogLists();
        List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogParticularCustomersList(int CustID);
        List<JKViewModels.Customer.CollectionsCallLogList> GetCustomerInvoice(int CustID);

        #endregion


        #region ContractTypeList Calls

        IQueryable<ContractTypeList> GetContractTypeList();

        ContractTypeList GetContractTypeListById(int id);

        ContractTypeList SaveContractTypeList(ContractTypeList ContractTypeList);

        ContractTypeList DeleteContractTypeList(int id);
        //List<Data.JkControl.vw_sys_term> getTermDate();
        Dictionary<string, string> getTaxAuthority();
        Dictionary<string, string> _GetStatusList(int typelistid);

        #endregion

        #region  Agreement Type List

        IQueryable<AgreementTypeList> GetAgreementTypeList();
        AgreementTypeList GetAgreementTypeListById(int id);

        #endregion

        #region Identification Calls

        IQueryable<Identification> GetIdentification();
        IQueryable<Identification_Temp> GetIdentificationTemp();

        Identification GetIdentificationById(int id);

        Identification SaveIdentification(Identification Identification);
        Identification_Temp SaveIdentification_Temp(Identification_Temp Identification);
         

        Identification DeleteIdentification(int id);



        #endregion

        #region AccountTypeList Calls

        IQueryable<AccountTypeList> GetAccountTypeList();

        AccountTypeList GetAccountTypeListById(int id);

        AccountTypeList SaveAccountTypeList(AccountTypeList AccountTypeList);

        AccountTypeList DeleteAccountTypeList(int id);



        #endregion

        #region Customer Account History Calls

        CustomerAccountHistoryViewModel GetCustomerAccountHistory(int customerId, DateTime startDate);

        #endregion

        #region ContractTermList Calls

        //IQueryable<ContractTermList> GetContractTermList();

        //ContractTermList GetContractTermListById(int id);

        //ContractTermList SaveContractTermList(ContractTermList ContractTermList);

        //ContractTermList DeleteContractTermList(int id);



        #endregion

        #region ContractStatusReasonList Calls

        IQueryable<ContractStatusReasonList> GetContractStatusReasonList();

        ContractStatusReasonList GetContractStatusReasonListById(int id);

        ContractStatusReasonList SaveContractStatusReasonList(ContractStatusReasonList ContractStatusReasonList);

        ContractStatusReasonList DeleteContractStatusReasonList(int id);
        CustomerContractViewModel GetCustomerContractByCustomerId(int id);
        CustomerContractDetailViewModel GetCustomerContractDetailByCustomerId(int ContractDetailId,int ContractId);



        #endregion

        #region Company
        JKApi.Data.DAL.Bank SaveBank(JKApi.Data.DAL.Bank Bank);
        #endregion

        #region  serviceCalls
        List<ServiceCallLogAreaList> GetServiceCallLogAreaList();
        List<UserLoginModel> GetUserOfDefaultRegion(int RegionId, bool isActive);
        List<ServiceCallLogTypeList> GetServiceCallLogTypeList();
        List<StatusResultList> GetStatusResultList();
        //Customer Detail Trasaction LIst 
        #endregion
        List<CustomerDetailTransactionViewModel> GetCustomerDetailTransactions(int customerId, int typeId, DateTime startDate, DateTime endDate);
        decimal GetCustomerBalanceAsOfDate(int customerId, DateTime? asOfDate);

        CFranchiseeDistributionViewModel GetCustomerFranchiseeDistribution(int customerId);
        CustomerFranchiseeDistributionViewModel GetCustomerFranchiseeDistributionData(int customerId);
        CFranchiseeDistributionViewModel GetFranchiseeDistribution(int FranchiseeId);
        bool SaveFranchiseeDistribution(List<FranchiseeDistribution> lstFranchiseeDistribution);
        bool SaveFranchiseeDetailsDistribution(List<FranchiseeDistribution> lstFranchiseeDistribution);

        Data.DAL.Distribution GetCustomerDistribution(int CustomerId);
        List<Data.DAL.Distribution> GetCustomerDistributionList(int CustomerId);
        CustomerDistributionDetailsModel GetCustomerDistributionDetails(int CustomerId);
        SpGetCustomerDetailsByCustomerID_Result SpGetCustomerDetailsByCustomerID(int ClassID);

        List<FranchiseeFinderFeeDetailViewModel> GetCustomerFinderFeeDetailList(int regionid, int customerid);
        FFinderFeeDetailFullViewModel GetCCFinderFeeDetail(int regionid, int findersfeeid);
        List<FindersFeeAdjustmentTypeListViewModel> GetFindersFeeAdjustmentTypeList();

        FCFindersFeeViewModel GetFindersFeewithCustomerId(int CustomerId,int DistributionId);

        List<JKViewModels.Customer.CollectionsCallLogModel> GetServiceCollectionCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext);
        List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext, int statusid);
        List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomersListForSearch(int CustID, DateTime fromdate, DateTime todate, string searchtext, string statusid);
        JKViewModels.Customer.ServiceCallLogModel GetServiceCallLogCustomersListForSearch(int ServiceCallLogId);
        List<JKViewModels.Customer.ServiceCallListViewModel> GetServiceCallListForSearch(string regionId, DateTime fromdate, DateTime todate, string Statusids, string TypeIds, int InitiatedBy = 0, int CreatedBy=0,int IsCallBack= 0);
        List<JKViewModels.Customer.ServiceCallListViewModel> GetServiceCallbackListForSearch(string regionId, DateTime fromdate, DateTime todate, string Statusids, string TypeIds, int InitiatedBy = 0, int CreatedBy = 0, int IsCallBack = 0, string fltdate = "", string ServiceStatusList = "");

        CustomerDetailInfoSummaryModel GetServiceCallListForSearch(int custid);
        //Transfer 
        List<CalanderDatesModel> GetCalanderDates(DateTime _from, DateTime _to);
        Invoice GetInvoiceDetailForCN(int _InvoiceId);
        RevenueDistributionInvoiceDetailViewModel GetRevenueDistributionDetail(int invoiceid);
        List<InvoiceRevenueDistributionDetailViewModel> CheckInvoiceForRevenueDistributionDetail(int _CustomerId, int ContractDetailId, DateTime _EffectiveDate);
        List<InvoiceRevenueDistributionDetailViewModel> CheckInvoiceForDecreaseCreditDetail(int _CustomerId, DateTime _EffectiveDate);
        bool InsertCustomerTransferData(List<CommonRevenueDistributionDetailViewModel> DistributionDetail, List<CommonRevenueDistributionFeeDetailViewModel> FeeDetail, CommonFranchiseeCustomerViewModel model);

        List<CustomerTransferPendingViewModel> GetCustomerTransferPendingList(string regionId = "");
        CommonCustomerrTransferPendingDetailViewModel GetCustomerrTransferPendingDetailData(int MasterCustomerTransferTempId);
        CommonCustomerMaintenanceDetailViewModel GetCustomerMaintenanceDetailDataPP(int MasterCustomerTransferTempId);
        bool GetCustomerMaintenanceDataDelete(int MasterCustomerTransferTempId);
        CommonCustomerrTransferPendingDetailViewModel GetCustomerrTransferPendingApproval(int MasterCustomerTransferTempId, bool IsApprove = true);
        CommonCustomerMaintenanceDetailViewModel GetCustomerMaintenanceDetailPPApproval(int MasterCustomerTempId,bool IsApprove);
        List<IncreaseInvoiceItemViewModel> GetCustomerIncreaseInvoice(int customerid, DateTime effectivedate, int monthDays, int workingDays, decimal applyamount);

        CommonCustomerIncreaseDecreaseDetailViewModel GetCustomerIncreaseDecreaseDetailData(int CustomerId);
        bool InsertCustomerIncreaseDecreaseDetail(CommonCustomerIncreaseDecreaseDetailViewModel InputData, List<CreditTransactionViewModel> lstCreditTransaction);


        CommonCustomerIncreaseDecreaseDetailViewModel GetCustomerIncreaseDecreaseDetailDataForEdit(int MaintenanceTempId);
        bool UpdateCustomerIncreaseDecreaseDetailApprove(CommonCustomerIncreaseDecreaseDetailViewModel InputData, bool IsApprove = true);

        // ======================================================================================
        #region FOM
        // ======================================================================================

        List<CustomerModel> GetCustomerList();
        List<CustomerModel> GetCustomerListByRegion(int regionId, int pageSize, int page);
        List<CustomerModel> GetCustomerListByFranchisee(int franchiseeId, int pageSize, int page);
        List<CustomerModel> GetNearByCustomerListByFranchisee(int franchiseeId, double latitude, double longitude, double distance);
        List<CustomerModel> GetNearByCustomerListByRegion(int regionId, double latitude, double longitude, double distance);
        List<CustomerLeadModel> GetNearByLeadListByRegion(int regionId, double latitude, double longitude, double distance);
        CustomerLeadModel GetLeadDetail(int leadDetailId);
        List<CustomerModel> GetCustomerPendingListByRegion(int regionId, int pageSize, int page);
        CustomerModel GetCustomer(int customerId);
        List<AccountWalkThruItemModel> GetAccountWalkThruItemListByCustomer(int customerId);
        AccountWalkThruItemModel GetAccountWalkThruItemById(int accountWalkThruItemId);
        AccountWalkThruItemModel AddOrUpdateAccountWalkThruItem(AccountWalkThruItemModel model);
        CustomerSimpleViewModel GetCustomerN(int customerId);

        #endregion
        // ======================================================================================
        int UpdateApproveReject(int CustomerId, string Note, int StatusListId, string valIds);
        int UpdatePendingStatus(int CustomerId, string Note, int StatusListId);
        int UpdateRejectCustomerMaintenance(int MaintenanceTempId,string Reason, int MaintenanceTypeListId);
        int UpdateApprovalCustomerMaintenance(int MaintenanceTempId);


        int UpdateStatus(int CustomerId, string Note, int OldStatusListId, int NewStatusListId, string valIds);
        int UpdateStatusToNextStep(int CustomerId, string Note, int StatusListId, string valIds);

        int SaveServiceCallLogDetailsByFORM(ServiceCallLogModel ServiceCallLogModel);

        // Distribution Report List
        List<DistributionReportViewModel> DistributionReportList(string searchtext, int status, string regionId);

        List<portal_spGet_C_GetNewCustomersReport_Result> NewCustomersReportList(string searchtext, int status, string sdata, string edate, int regionId, string regionIds);
        IEnumerable<CustomerMaintenanceViewModel> GetRegionWiseCustomer(string customerName);
        IEnumerable<CustomerDetailTransactionWithStatusViewModel> GetAllCustomerTransactions(int? customerId, int? typeId, DateTime? startDate, DateTime? endDate, int month, int year);

        //CRM Upload Document
        IEnumerable<JKViewModels.CRM.CRMDocumentViewModel> GetCRMDocumentByAccountCustomerDetailId(int AccountCustomerDetailId, int TypeListId);
        IEnumerable<JKViewModels.CRM.CRMDocumentViewModel> GetCRMDocumentByAccountFranchiseDetailId(int AccountFranchiseDetailId, int TypeListId);


        //Account Offring Result 
        CommonAccountOfferingViewModel GetAccountOfferingResult(int CustomerId, string FranchiseeType, decimal Distance, string sDate, string eDate);
        List<AccountOfferingResponceViewModel> InsertOfferingData(int CustomerId, string FranchiseeIds, DateTime exDate, DateTime extime, string note);
        Email GetEmailAddrress(int classid, int typelistid);
        List<CRMAccountOfferingListViewModel> GetAccountOfferingData(string regionid, string statusid);
        List<CRMAccountOfferingListViewModel> GetAccountOfferingDataWithFranchiseeId(int FranchiseeId);
        List<ValidationItem> ValidationItemListStatus(int StatusListId, int TypeListId);
        List<CustomerFranchiseeDistribution> GetCustomerFranchiseeForNewAccountForm(int CustomerId);

        List<Validation> GetValidationByClassId(int ClassId, int StatusListId);

        Status GetStatusByClassId(int ClassId, int StatusListId);
        AccountAcceptanceInfoViewModel GetAccountAcceptanceInfoOffer(int CustomerId);
        List<CustomerRevenuesResultViewModel> GetCustomerRevenuesReportData(string regions, int periodid);
        CustomerForSettingViewModel GetAllCustomerForSetting(bool IsThirdParty, int regionId = 0, int isAll = 0);
        List<CustomerSearchModel> GetCustomerSettingAllPerentChildList(int type, int regionId = 0, int ParentId = 0, string StatusListIds = "1", int IsAll = 0);
        List<CustomerSearchModel> GetAllCustomerChild(int custId, int regionId = 0);
        List<CustomerSearchResultModel> GetCustomerSearchResultList(int RegionId, string Search, decimal AmountTo, decimal AmountFrom, decimal SqrFtTo, decimal SqrFtFrom, string Orderby, int Status, string regionIds = "", int ctypeId = 0);
        List<JKViewModels.Customer.ServiceCallLogModel> GetServiceCallLogCustomerSearchResultDetails(int CustID, string startDate, string endDate);
        List<CustomerSearchCancallationPendingResultViewModel> GetCustomerSearchCancallationPendingList(string statusListIds = "0", string regionId = "");
        List<CustomerSearchCancallationPendingResultViewModel> GetCustomerSearchCancallationPendingListNew(string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0);
        List<CustomerSearchCancallationPendingResultViewModel> GetCustomerServiceComplainListResult(string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0, string StageStatusIds = "");
        List<CustomerServiceWednesdayReportResultViewModel> GetCustomerServiceWednesdayReport(int Days=0, string statusListIds = "0", string regionId = "", int ServiceLogTypeListId = 0, string GroupByValue ="",string AtLeast = "", string LessThan = "", string SearchAmount="", string AtRisk="");

        IQueryable<CSActivity> GetCSActivityList();
        List<CSActivity> GetCSActivityByClassId(int ClassId, int TypeListId);
        CSActivity GetCSActivityListById(int id);

        IQueryable<CSstage> GetCSstageItemList();
        int SaveCSActivityData(int CustomerId, int StagId, int CSstageId,string Note, string valIds);
        int SaveCSActivityNotifytheFranchiseeOwneData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4,int optitem5);
        int SaveCSActivityContacttheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4,string optitem3EndTime);
        int SaveCSActivityInspecttheAccountData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3);
        int SaveCSActivityDefectnotifytoFranchiseeData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1,int optitem2, string optitem2Note, int optitem3);
        int SaveCSActivityLetertotheCustomerData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem1note, int optitem2, int optitem3, string optitem4Date, string optitem4Time,string optitem4endTime);
        int SaveCSActivityReInspecttherAccountData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem1Note, int optitem2, int optitem3, string optitem3Note);
        int SaveCSActivityFollowupBackontrackData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3);

        int SaveCSActivityComplaintLoggedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityActionsFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityFollowUpData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityCompletionClosedData(int ServiceCalllogId, int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3, List<ValidationItemDataModel> ItemList);

        int SaveCSActivityFIComplaintLoggedData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, int optitem3, int optitem4, int optitem5, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityFIActionsFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem3Date, string optitem3Time, int optitem4, string optitem3EndTime, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityFIFollowUpData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, string optitem2, int optitem3, List<ValidationItemDataModel> ItemList);
        int SaveCSActivityFICompletionClosedData(int CustomerId, int StagId, int CSstageId, string Note, string valIds, int optitem1, int optitem2, string optitem2Note, int optitem3, List<ValidationItemDataModel> ItemList);

        List<CSstage> CSstageListStatus(int StatusListId, int TypeListId, int CustomerId);
        List<CSstage> CSstageListWithCustomerWise(int TypeListId, int CustomerId);
        List<CustomerCancellationActivityModel> GetCustomerCancellationActivityDetails(int CustomerId);
        List<CustomerCancellationActivityModel> GetCustomerComplaintActivityDetails(int CustomerId);
        List<CustomerCancellationActivityModel> GetCustomerFailedInspectionActivityDetails(int CustomerId);
        List<CustomerFranchiseeDistributionModel> GetFranchiseeDistributionWithCustomer(int CustomerId);
        List<DistributionFeesDetailModel> GetDistributionFeesDetail(int DistributionId);

        Dictionary<string, string> GetFrancisesBySearchCustomerId(int custId);

        bool CheckCustomerOfferingExits(int CustomerId, int FranchiseeId);
        bool CustomerOfferAcceptedStatusSave(int OfferingId, DateTime ResponseDateTime ,string  ResponseName,string  ReasonNote);
        bool CustomerOfferDeclineStatusSave(int OfferingId,int DeclineReasonListId, string DeclineReasonNote);
        Offering GetOfferingsById(int OfferingId);
        Status UpdateCustomerStatus(Status model);
        List<AuthUserLogin> GetAllLoginUserList();
        List<IncreaseDecreaseHistoryModel> GetIncreaseDecreaseHistoryList(string regionIds = "", DateTime? from = null, DateTime? to = null, int month = 0, int year = 0,int customerId=0);
        void UpdateTaxRateAddress(int OldAddId, int AddressId, int CustomerId);
        BasicInfoCustomerModel GetBasicInfoCustomer(int Id);        
        Dictionary<int, string> GetCustomerNotes(int CustomerId, int SelectedRegionId, int TypeListId=1);
        int SaveNotesDetail(int Id, int ClassId, string Notes,int _type);
        IEnumerable<CustomerServiceScheduleDataModel> GetCustomerServiceScheduleData(int? customerId, string regionId, int? dayToAdd,int? userId,DateTime? startDate,DateTime? endDate);
        IEnumerable<CustomerServiceScheduleDataModel> GetCRMScheduleData(int customerId, DateTime dateTime, int userId, string regionId, DateTime endDate);
        IEnumerable<CRM_ScheduleTypeModel> GetScheduleTyoeList();
        void SaveCSAccountWalkThursFormFieldDetail(int customerId,int FranchiseeId,List<NewAccountFormFieldModel> List);
        List<CSAccountWalkThursFormFieldDetail> GetCSAccountWalkThursFormFieldDetailWithCustomer(int customerId);
    }
}
