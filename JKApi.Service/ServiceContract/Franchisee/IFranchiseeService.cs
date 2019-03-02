using JKApi.Data.DAL;
using JKApi.Data.DTOObject;
using JKViewModels;
using JKViewModels.Company;
using JKViewModels.Customer;
using JKViewModels.Franchise;
using JKViewModels.Franchisee;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.ServiceContract.Franchisee
{
    public interface IFranchiseeService
    {
        #region Franchisee Calls

        IQueryable<JKApi.Data.DAL.Franchisee> GetFranchisee();
        IQueryable<JKApi.Data.DAL.Franchisee_Temp> GetFranchiseeTemp();

        IQueryable<JKApi.Data.DAL.Address> GetAddress(int id);

        JKApi.Data.DAL.Franchisee GetFranchiseeById(int id);
        JKApi.Data.DAL.Franchisee_Temp GetFranchiseeById_Temp(int id);

        JKApi.Data.DAL.Franchisee SaveFranchisee(JKApi.Data.DAL.Franchisee Franchisee, bool isStatusAdd = true);
        JKApi.Data.DAL.Franchisee_Temp SaveFranchisee_Temp(JKApi.Data.DAL.Franchisee_Temp Franchisee, bool isStatusAdd = true);

        JKApi.Data.DAL.Franchisee DeleteFranchisee(int id);

        JKApi.Data.DAL.Franchisee_Temp DeleteFranchisee_Temp(int id);

        List<FranchiseeOwner> GetFranchiseeOwnerById(int id, int TypeListId, int ContactTypeListId);
        List<FranchiseeOwner> GetFranchiseeOwnerByIdTemp(int id, int TypeListId, int ContactTypeListId);

        IEnumerable<FullFranchiseeViewModel> GetFranchiseeDetailsByStatus(int StatusId, int TypeListId, int ContactTypeListId);


        IEnumerable<FranchiseeListViewModel> GetFranchiseeList(string status = null, int? regionId = null);
        IEnumerable<FranchiseeListViewModel> GetSearchFranchiseeList(string s,string sdt , string edt, int ptId, string statusIds, string  regionId);

        FranchiseeDetailViewModel GetFranchiseeDetail(int FranchiseeId);
        FranchiseeDetailViewModel GetFranchiseeDetailTemp(int FranchiseeId);
        TaxRateViewModel GetTaxRateDetail(int ClassId, int TypeListId, int AddressId = 0);
        List<FindersFeeScheduleViewModel> GetFindersFeeScheduleListData();
        portal_spGet_AR_FranchiseeDetail_Result GetFranchiseeDetailData(int FranchiseeId);

        List<FranchiseeTransactionTypeList> GetFranchiseeTransactionTypeList();
        List<ServiceTypeList> GetServiceTypeList();
        List<StatusList> GetStatusList();
        List<JKApi.Data.DAL.Franchisee> GetFranchiseeByParentId(int id);
        JKApi.Data.DAL.Franchisee UpdateFranchiseePrefix(JKApi.Data.DAL.Franchisee Franchisee);
        JKApi.Data.DAL.Franchisee UpdateFranchiseeParentId(JKApi.Data.DAL.Franchisee Franchisee);
        List<FranchiseeManualTrxCreditReasonList> GetAll_ReasonList();

        bool CreateFranchiseeManualTrasactionSave(FranchiseeTransactionViewModel inputData);
        FranchiseeTransactionViewModel GetFranchiseeManualTrasactionForEdit(int Id);
        bool GetFranchiseeManualTrasactionForDelete(int Id);
        List<VendorViewModel> TRVGetVendorList(int RegionId);

        List<VendorViewModel> GetVendorList(int RegionId);
        List<FranchiseeFeeViewModel> GetFranchiseeFee(int franchiseeId);
        bool SaveFranchiseeManualTrasactionForEdit(FranchiseeTransactionViewModel inputData,bool IsApprove);
        bool SaveFranchiseeChargeBackCredit(FranchiseeChargebackCreditViewModel inputData, bool IsApprove);
        bool SaveFranchiseeManualTrasactionForApproved(string Id, bool IsApproved, string note);


        string getfranchiseeno();

        RegionConfiguration GetRegionConfigurationbyId(int id);

        RegionConfiguration SaveRegionConfiguration(RegionConfiguration RegionConfiguration);

        List<SearchDateList> GetAll_OptionList();

        List<TransactionStatusList> GetAll_TransactionStatusList();
         List<FindersFeeTypeList> GetAll_FindersFeeTypeList();
        List<FranchiseeSearchModel> GetSearchFranchisee(string searchText);
        IEnumerable<FranchiseeListViewModel> GetFranchiseeListData(int RegionId);



        List<FranchiseePendingMaintenanceListViewModel> GetFranchiseePendingMaintenanceList();
        bool GetFranchiseePendingMaintenanceApproved(string ids, bool IsApproved);
        CommonFranchiseeCustomerViewModel GetFranchiseeCustomerDistributionDataForEdit(int MaintenanceTempId);

        portal_spGet_AR_FranchiseeSupplybyId_Result GetFranchiseebySupplyId(int Id, int RegionId);
        #endregion



        #region FranchiseeBillSettings Calls

        IQueryable<FranchiseeBillSetting> GetFranchiseeBillSettings();
        IQueryable<FranchiseeBillSettings_Temp> GetFranchiseeBillSettingsTemp();
        FranchiseeDashboardModel GetFranchiseeDashboardData(string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        IEnumerable<FranchiseeDashboardModel> GetFranchiseRevenueByMonthChartData(int recordNumber, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        FranchiseeBillSetting GetFranchiseeBillSettingsById(int id);
        FranchiseeBillSettings_Temp  GetFranchiseeBillSettingsByIdTemp(int id);

        FranchiseeBillSetting SaveFranchiseeBillSettings(FranchiseeBillSetting FranchiseeBillSettings);
        FranchiseeBillSettings_Temp SaveFranchiseeBillSettings_Temp(FranchiseeBillSettings_Temp FranchiseeBillSettings);

        FranchiseeBillSetting DeleteFranchiseeBillSettings(int id);
        IEnumerable<FranchiseeDashboardModel> GetRevenueWiseTopFranchiseeChartData(int recordNumber, string regionId, DateTime? spnStartDate, DateTime? spnEndDate, int? billMonth, int? billYear);
        
        #endregion

        IEnumerable<FinderFeeBillDetailViewModel> GetFinderFeeBillDetail(string TrNo);

        IEnumerable<LeaseBillDetailViewModel> GetLeaseBillReportDetail(string TrNo);
        FranchiseOwnersList GetFranchiseeOwnerListByOwnerId_Temp(int Id);
        FranchiseeOwnerList GetFranchiseeOwnerList(int? _classid, int _Contactid);
        FranchiseeOwnerList SaveFranchiseeOwnerList(FranchiseeOwnerList FranchiseeOwnerList);
        #region IdentifierTypeList Calls

        IQueryable<IdentifierTypeList> GetIdentifierTypeList();

        IdentifierTypeList GetIdentifierTypeListById(int id);

        IdentifierTypeList SaveIdentifierTypeList(IdentifierTypeList IdentifierTypeList);

        IdentifierTypeList DeleteIdentifierTypeList(int id);



        #endregion

        #region ACHBank Calls

        IQueryable<ACHBank> GetACHBank();
        IQueryable<ACHBank_Temp> GetACHBankTemp();

        ACHBank GetACHBankById(int id);

        ACHBank SaveACHBank(ACHBank ACHBank);
        ACHBank_Temp SaveACHBank_Temp(ACHBank_Temp ACHBank);
        

        ACHBank DeleteACHBank(int id);



        #endregion

        #region FranchiseeFullfillment Calls

        IQueryable<FranchiseeFullfillment> GetFranchiseeFullfillment();
        IQueryable<FranchiseeFullfillment_Temp> GetFranchiseeFullfillmentTemp();

        FranchiseeFullfillment GetFranchiseeFullfillmentById(int id);

        FranchiseeFullfillment SaveFranchiseeFullfillment(FranchiseeFullfillment FranchiseeFullfillment);
        FranchiseeFullfillment_Temp SaveFranchiseeFullfillmentTemp(FranchiseeFullfillment_Temp FranchiseeFullfillment);

        FranchiseeFullfillment DeleteFranchiseeFullfillment(int id);

        void UpdateLegalComplianceNote(int LegalComplianceStatuId, string LegalComplianceNote);

        int GetFullfillmentWithFranchisee(int FranchiseeId);

        #endregion



        #region FranchiseeContract Calls

        IQueryable<FranchiseeContract> GetFranchiseeContract();
        IQueryable<FranchiseeContract_Temp> GetFranchiseeContractTemp();

        FranchiseeContract GetFranchiseeContractById(int id);

        FranchiseeContract SaveFranchiseeContract(FranchiseeContract FranchiseeContract);
        FranchiseeContract_Temp SaveFranchiseeContract_Temp(FranchiseeContract_Temp FranchiseeContract);

        FeeConfiguration SaveFranchiseeFeeConfiguration(FeeConfiguration Feeconfiguration, int UserId);

        FranchiseeContract DeleteFranchiseeContract(int id);



        #endregion



        #region FranchiseeContractTypeList Calls

        IQueryable<FranchiseeContractTypeList> GetFranchiseeContractTypeList();

        FranchiseeContractTypeList GetFranchiseeContractTypeListById(int id);

        FranchiseeContractTypeList SaveFranchiseeContractTypeList(FranchiseeContractTypeList FranchiseeContractTypeList);

        FranchiseeContractTypeList DeleteFranchiseeContractTypeList(int id);



        #endregion

        #region FranchiseeFee Calls

        IQueryable<FranchiseeFee> GetFranchiseeFee();

        FranchiseeFee GetFranchiseeFeeById(int id);
        FranchiseeFee_Temp GetFranchiseeFeeById_Temp(int id);

        FranchiseeFee SaveFranchiseeFee(FranchiseeFee Fee);
        FranchiseeFee_Temp SaveFranchiseeFee_Temp(FranchiseeFee_Temp Fee);

        FranchiseeFee DeleteFranchiseeFee(int id);
        FranchiseeFee_Temp DeleteFranchiseeFee_Temp(int id);

        List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollectionAll(int FranchiseeId);

        void AddDefaultFranchiseeFees(int FranchiseeId, int FeesId, decimal Amount);

        void AddDefaultFranchiseeFees_Temp(int FranchiseeId, int FeesId, decimal Amount);

        #endregion


        #region Fees Calls

        IQueryable<CusFee> GetFees();

        CusFee GetFeesById(int id);

        CusFee SaveFees(CusFee Fees);

        CusFee DeleteFees(int id);



        #endregion

        #region FeeRate Calls

        IQueryable<FeeRate> GetFeeRate();

        FeeRate GetFeeRateById(int id);

        FeeRate SaveFeeRate(FeeRate FeeRate);

        FeeRate DeleteFeeRate(int id);



        #endregion

        #region FranchiseeFeeList Calls
        IQueryable<FranchiseeFeeList> GetFranchiseeFeeList();

        FranchiseeFeeList GetFranchiseeFeeListById(int id);

        FranchiseeFeeList SaveFranchiseeFeeList(FranchiseeFeeList FranchiseeFeeList);

        FranchiseeFeeList DeleteFranchiseeFeeList(int id);

        List<FranchiseeFeeListFeeRateTypeListCollectionViewModel> GetFranchiseeFeeListFeeRateTypeListCollection();

        List<FeesViewModel> GetFeeListwithFranchiseeId(int FranchiseeId);

        List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollection(int FranchiseeId);
        List<FeeFranchiseeFeeRateTypeListCollectionViewModel> GetFeeListCollection_Temp(int FranchiseeId);

        FeeConfiguration GetFranchiseeFeeConfigurationById(int FranchiseeId);
        IQueryable<FeeConfiguration> GetFranchiseeFeeConfiguration();
        IQueryable<FeeConfiguration_Temp> GetFranchiseeFeeConfiguration_Temp();
        #endregion

        #region FeeRateTypeList Calls

        IQueryable<FeeRateTypeList> GetFeeRateTypeList();

        FeeRateTypeList GetFeeRateTypeListById(int id);

        FeeRateTypeList SaveFeeRateTypeList(FeeRateTypeList FeeRateTypeList);

        FeeRateTypeList DeleteFeeRateTypeList(int id);

        #endregion

        #region Lease
        Lease SaveLease(Lease Lease);
        LeaseViewModel GetLeaseModel(int Id);

        #endregion

        #region Franchisee Distribution

        FranchiseeDistributionDetailsModel GetFranchiseeDistributionDetails(int FranchiseeId);
        CommonFranchiseeCustomerViewModel GetFranchiseeCustomerDistributionData(int CustomerId, int FranchiseeId, int ContractDetailDistributionLineNo);
        bool InsertFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData, int TypeListId);
        bool InsertFranchiseeCustomerOnlyDistributionDetail(CommonFranchiseeCustomerViewModel InputData, int TypeListId);
        bool SaveFindersFeeDetails(CommonFranchiseeCustomerViewModel InputData, int TypeListId);
        bool SaveFindersFeeDetailsOnlyFF(CommonFranchiseeCustomerViewModel InputData, int TypeListId);
        bool UpdateFranchiseeCustomerDistributionDetail(CommonFranchiseeCustomerViewModel InputData);

        #endregion

        IEnumerable<FMTDetailViewModel> GetFMTDetail(string TrNo);
        IQueryable<MasterTrxTypeList> GetUsMasterTrxTypeList();
        List<portal_spGet_F_FindersFeeList_Result> GetFinderFeeList(string regionIds, string statusIds);
        List<portal_spGet_F_FindersFeeDetailList_Result> GetFinderFeeDetailList(int franchiseeid, string regionIds, string statusIds);
        List<FranchiseeFinderFeeDetailViewModel> GetFranchiseeFinderFeeDetailList(int regionid, int franchiseeid);

        List<portal_spGet_F_GetFinderFeeCustomersList_Result> GetFinderFeeCustomersList(int regionid, int franchiseeid,string statuslistid);
        
        List<FinderFeeCustomerListStatusViewModel> GetFinderFeeCustomerListStatus(int regionid , int franchiseid);


        int UpdateApproveReject(int FranchiseeId, string Note, int StatusListId,string valIds);
        int UpdateFranchiseeTempStatus(int FranchiseeId, string Note, int StatusListId);
        
        void savePendingMessageForLegal(string message, int customerID, int status);

        void savePendingMessage(string message, int customerID, int status);

        List<FranchiseeCustomerModel> GetFranchiseeCustomerList(int FranchiseeId);
                
        FranchiseeCustomerModel GetCustomerDetails(int CustomerId);
        IEnumerable<MasterTrxTypeList> GetMasterTrxTypeListForFranchise();
        IEnumerable<FranchiseeWiseTransactionViewModel> GetAllFranchiseTransactions(string franchiseName, int? franchiseId, int? regionId, int? recordNumber, int? masterTrxTypeId, DateTime? spnStartDate, DateTime? spnEndDate, int month, int year);
        IEnumerable<portal_spGet_F_ChargebackListForFranchisee_Result> GetFranchiseeChargebacks(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate);
        IEnumerable<JKApi.Data.DAL.Franchisee> GetRegionWiseFranchaise(string franchiseName, int? regionId);
        decimal GetFranchiseeBalanceAsOfDate(int? franchiseId, DateTime? spnStartDate);
        List<VendorInvoiceList> GetVendorInvoiceList(string statusListIds = "", string regionId = "");
        FranchiseeBillingPayInfoViewModel GetFranchiseeBillingPayInfoByInvoiceNo(int RegionId, string InvoiceNo, int FranchiseeId);
        IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> GetFranchiseeChargebacksData(int? franchiseId, DateTime? spnStartDate, DateTime? spnEndDate);
        Status GetLegalComplianceStatusByFranchiseeid(int Franchiseeid, int StatusListId);
        IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> ChargeBackDetailPopUp(string trNo);
        List<portal_spGet_C_DistributionWithNoFinderFee_Result> GetCustomerDistributionWithNoFinderFee(int CustomerId); 
        bool CheckFranchiseeIsExist(string Name);
        bool CheckFranchiseeIsExistTemp(string Name);

        CommonFranchiseeAccountHistoryReportViewModel GetFranchiseeAccountHistoryReport(int FranchiseeId);

        List<FranchiseeRevenuesResultViewModel> GetFranchiseeRevenuesReportData(string regionId, int periodid);
        DataTable GetFranchiseeDeductionReportData(string regionId, int periodid);
        RemitToViewModel GetRemitToForRegion(int regionId);

        int CheckOnlyFranchiseeNamePhoneIsExist(string Name, string Phone);
        List<StatusList> GetAll_StatusList();
        List<StatusReasonList> GetAll_StatusReasonList(int TypeListId);
        FranchiseeMaintenanceViewModel GetFranchiseeMaintenanceViewModelById(int id);
        bool InsertUpdateFranchiseeMaintenanceTemp(FranchiseeMaintenanceViewModel inputData);
        EditFranchiseeMaintenanceViewModel GetFranchiseeMaintenanceTemp(int Id);
        bool UpdateFranchiseeMaintenanceTemp(EditFranchiseeMaintenanceViewModel inputData);
        List<portal_spGet_AP_AvailableFranchiseeReportFinalizedPeriods_Result> GetAvailableFranchiseeReportFinalizedPeriods(string RegionId);
        portal_spGet_AP_FinalizedFranchiseeReportList_Result GetFinalizedFranchiseeReport(int? billMonth = null, int? billYear = null, string regionIds = null, int? frid=null);
        int GetFranchiseeLeaseStatus(int Id);
        List<DeclineReasonList> GetDeclineReasonListList();

        int SaveFranchiseeOfferingData(Offering model);
        List<FranchiseeObligationViewModel> GetFranchiseeObligationList(int Id);
        ManualTransactionViewModel EditManualTrasaction(string TrxNo);
        bool UpdateFranchiseeManualTrasaction(ManualTransactionViewModel inputData);
        FranchiseeOwnerList_Temp SaveFranchiseeOwnerList_Temp(FranchiseeOwnerList_Temp FranchiseeOwnerList_Temp);
        int? getCheckPeriod(int PeriodClsId);

        List<FranCallModel> GetFranchiseeCall(int FranchiseeId, int CustomerId);
        int SaveFranCallsDetails(int FranchiseeId, string InitiatedBy, int StatusResultListId, string SpokeWith, string CallAction, string CallBack, string CallBackTime, string Comments, int CustomerId,int IsCallBack);
        FranchiseeBasicInfo GetFranchiseeBasicInfo(int FranchiseeId);
        int SaveFranchiseeFeeConfigurationData(int FeeConfigurationInfoId, int ClassId, decimal MinimumAmoun, string EffectiveDate);
        int SaveFranchiseeFeeConfigurationData_Temp(int FeeConfigurationInfoId, int ClassId, decimal MinimumAmoun, string EffectiveDate);
        int RemoveFeeConfigurationRecord(int Id);
        TaxRateViewModel GetTaxRateDetailForFranchiseeSupply(int RegionId);
        string GetFranchiseeStatus(int Id);
        void UpdateFranchiseeDetails(int FranchiseeId, string Name);
        void UpdateFranchiseeStatus(int FranchiseeId, int StatusListId);
        void UpdateFranchiseeStatus_Temp(int FranchiseeId, int StatusListId);
        void moveFeeConfigurationDataOldToNewFranchisee(int FranchiseeId, int NewFranchiseeId);
        int MoveFranchiseeTempDataToRealTable(int FranchiseeId);
        FranchiseeOwnerList_Temp GetFranchiseeOwnerList_Temp(int _Contactid);

        List<FranchiseOwnersList> GetFranchiseeOwnerListById(int id);
        FranchiseOwnersList GetFranchiseeOwnerListByOwnerId(int Id);
        List<FranchiseOwnersList> GetFranchiseeOwnerListByIdTemp(int id);

    }
}
