using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{
    public class CacheKeyName
    {
        public const string FileTypeList = "FileTypeList";
        public const string GetTransactionTypeList = "GetTransactionTypeList";
        public const string GetFranchiseStatusList = "GetFranchiseStatusList";
        public const string GetCrmAccountCustomerLeadListModels = "GetCrmAccountCustomerLeadListModels";
        public const string GetCrmAccountCustomerPotentialListModels = "GetCrmAccountCustomerPotentialListModels";
        public const string GetCrmTaskTypes = "GetCrmTaskTypes";
        public const string GetCrmStages = "GetCrmStages";
        public const string GetCrmStageStatus = "GetCrmStageStatus";
        public const string GetCrmIndustryTypes = "GetCrmIndustryTypes";
        public const string GetCrmProviderSources = "GetCrmProviderSources";
        public const string GetCrmProviderTypes = "GetCrmProviderTypes";
        public const string GetCrmAccountTypes = "GetCrmAccountTypes";
        public const string GetCrmActivityOutcomeTypes = "GetCrmActivityOutcomeTypes";
        public const string GetCrmActivityTypes = "GetCrmActivityTypes";
        public const string GetCrmTimeLineTypes = "GetCrmTimeLineTypes";

        #region Customer Module

        public const string Customer_GetFrequencyList = "Customer_GetFrequencyList";
        public const string Customer_getCAccountType = "Customer_getCAccountType";
        public const string Customer_getUsStatesList = "Customer_getUsStatesList";
        public const string Customer_getTaxAuthority = "Customer_getTaxAuthority";
        public const string Customer_getTermDate = "Customer_getTermDate";
        public const string Customer_getInvoiceDate = "Customer_getInvoiceDate";
        public const string Customer_getARStatusList = "Customer_getARStatusList";
        public const string Customer_getARStatusResonList = "Customer_getARStatusReasonList";
        public const string Customer_getStatusList = "Customer_getStatusList";

        #endregion

        #region Enums

        public const string AccountTypeList_GetAll = "AccountTypeList_GetAll";
        public const string ContactTypeList_GetAll = "ContactTypeList_GetAll";
        public const string ServiceTypeList_GetAll = "ServiceTypeList_GetAll";
        
        #endregion

        #region Distribution

        public const string Distribution_GetAll = "Distribution_GetAll";
        public const string Distribution_GetAll_ByContractDetailId = "Distribution_GetAll_ByContractDetailId";
        public const string Distribution_GetAll_ByCustomerId = "Distribution_GetAll_ByCustomerId";
        public const string Distribution_GetAll_ByFranchiseeId = "Distribution_GetAll_ByFranchiseeId";

        #endregion

        #region Inspection

        public const string Inspection_GetAll_InspectionStatus = "Inspection_GetAll_InspectionStatus";

        #endregion

        #region Common
        public const string DropDownValues = "DropDownValues";
        #endregion

        #region Potential
        public const string Potential_GetAccountTypeList = "Potential_GetAccountTypeList";
        public const string Potential_ProviderTypeList = "Potential_ProviderTypeList";
        public const string Potential_ProviderSourceList = "Potential_ProviderSourceList";
        public const string Potential_BiddingPurposeTypeList = "Potential_BiddingPurposeTypeList";
        public const string Potential_FollowUpPurposeTypeList = "Potential_FollowUpPurposeTypeList";
        public const string Potential_CloseType = "Potential_CloseType";
        public const string Potential_State = "Potential_State";
        public const string Potential_ServiceTypeListModel = "Potential_ServiceTypeListModel";
        public const string Potential_FrequencyListModel = "Potential_FrequencyListModel";
        public const string Potential_CleanFrequencyListModel = "Potential_CleanFrequencyListModel";
        public const string Potential_QualifiedLeadStageStatusList = "Potential_QualifiedLeadStageStatusList";
        #endregion
    }
}
