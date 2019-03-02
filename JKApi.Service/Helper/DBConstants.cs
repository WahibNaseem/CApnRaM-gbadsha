namespace JKApi.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class DBConstants
    {
        #region Stored Procedure Keys

        public const string sp_TotalRecords_Key = "TotalRecords";
        public const string sp_ErrorNumber_Key = "ErrorNumber";
        public const string sp_ErrorSeverity = "ErrorSeverity";
        public const string sp_ErrorState_Key = "ErrorState";
        public const string sp_ErrorLine_Key = "ErrorLine";
        public const string sp_ErrorMessage_Key = "ErrorMessage";
        public const string sp_SortOrderAscending_Key = "asc";
        public const string sp_SortOrderDesceding_Key = "desc";

        #endregion

        #region Common
        public const string common_GetEMailTemplate = "common_GetEMailTemplate";
        public const string common_GetDropDownValues = "common_GetDropDownValues";
        public const string common_GetPeriodDropDownValues = "common_GetPeriodDropDownValues";
        public const string common_DashboardQuickLinks = "common_DashboardQuickLinks";
        public const string common_DashboardPendingData = "SpGetAllPendingDataForDashbord";
        public const string common_Sp_Update_ChildViewMessageDashboard = "Sp_Update_ChildViewMessageDashboard";
        public const string common_DashboardPendingTasksData = "spGetCRMScheduleByUserId";

        public const string common_AddOrUpdateFile = "common_AddOrUpdateFile";
        public const string common_GetFileListByClassAndType = "common_GetFileListByClassAndType";
        public const string common_GetFile = "common_GetFile";
        public const string common_GetFileTypeList = "common_GetFileTypeList";
        public const string CRM_UploadDocumentFileName = "CRM_UploadDocumentFileName";

        #endregion

        #region JK Controls

        public const string Auth_UserLogin = "Auth_UserLogin";
        public const string Auth_SaveUserLogin = "Auth_SaveUserLogin";
        public const string Auth_UserPasswordReset = "Auth_UserPasswordReset";

        #endregion

        #region User 

        public const string Auth_UserList = "Auth_UserList";
        public const string admin_UserDelete = "admin_UserDelete";
        public const string Auth_UserDefaultRegionList = "AuthUser_spGet_DefaultRegion";
        public const string Auth_UpdateIsAccExec = "Auth_UpdateIsAccExec";

        #endregion

        #region Group

        public const string Auth_GroupList = "Auth_GroupList";
        public const string Auth_SaveGroup = "Auth_SaveGroup";

        #endregion

        #region Role

        public const string Auth_RoleList = "Auth_RoleList";
        public const string Auth_SaveRole = "Auth_SaveRole";

        #endregion

        #region StageSetting

        public const string CRM_StageSettingList = "CRM_StageSettingList";
        public const string CRM_SaveStageSetting = "CRM_SaveStageSetting";

        #endregion

        #region Menu

        public const string Auth_FillAllMenu = "Auth_FillAllMenu";
        public const string Auth_GetAllMenu = "Auth_GetAllMenu";
        public const string Auth_GetMenuById = "Auth_GetMenuById";
        public const string Auth_InsertUpdateMenu = "Auth_InsertUpdateMenu";
        public const string Auth_ActiveDeactiveMenu = "Auth_ActiveDeactiveMenu";
        public const string Auth_SearchMenu = "Auth_SearchMenu";

        #endregion

        #region Assign Menu

        public const string Auth_GetAssignMenusByAccess = "Auth_GetAssignMenusByAccess";
        public const string Auth_GetRolebasedMenuAccessDetail = "Auth_GetRolebasedMenuAccessDetail";
        public const string Auth_InsertUpdateAssignMenu = "Auth_InsertUpdateAssignMenu";
        public const string Auth_GetARRolebasedMenuAccessDetail = "Auth_GetARRolebasedMenuAccessDetail";
        public const string Auth_InsertUpdateAssignARMenu = "Auth_InsertUpdateAssignARMenu";
        public const string Auth_GetEDRolebasedMenuAccessDetail = "Auth_GetEDRolebasedMenuAccessDetail";
        public const string Auth_InsertUpdateAssignEDMenu = "Auth_InsertUpdateAssignEDMenu";
        #endregion

        #region Contract

        public const string sp_GetAllContracts = "sp_GetAllContracts";
        public const string sp_GetAllContractsByFranchisee = "sp_GetAllContractsByFranchisee";

        #endregion

        #region Customer

        public const string GetCustomerDetailsByIdWithActive = "GetCustomerDetailsByIdWithActive";
        public const string sp_GetCustomerList = "sp_GetCustomerList";
        public const string sp_GetCustomerListByRegion = "sp_GetCustomerListByRegion";
        public const string sp_GetCustomerListByFranchisee = "sp_GetCustomerListByFranchisee";
        public const string sp_GetNearbyCustomerListByRegion = "sp_GetNearbyCustomerListByRegion";
        public const string sp_GetNearByCustomerListByFranchisee = "sp_GetNearByCustomerListByFranchisee";
        public const string sp_getNearByLeadListByRegion = "sp_getNearByLeadListByRegion";
        public const string sp_GetLeadListByRegion = "sp_GetLeadListByRegion";
        public const string sp_GetLeadDetail = "sp_GetLeadDetail";
        public const string sp_Update_CRM_AccountCustomerDetail_Coordinate = "sp_Update_CRM_AccountCustomerDetail_Coordinate";
        public const string sp_GetCustomerPendingListByRegion = "sp_GetCustomerPendingListByRegion";
        public const string sp_GetCustomer = "sp_GetCustomer";
        public const string sp_GetAccountWalkThruItemListByCustomer = "sp_GetAccountWalkThruItemListByCustomer";
        public const string sp_GetAccountWalkThruItemById = "sp_GetAccountWalkThruItemById";
        public const string sp_AddOrUpdateAccountWalkThru = "sp_AddOrUpdateAccountWalkThru";

        #endregion

        #region Inspection 

        public const string sp_Administration_Inspection_SaveCustomTemplate = "sp_Administration_Inspection_SaveCustomTemplate";
        public const string sp_Administration_Inspection_SaveTemplate = "sp_Administration_Inspection_SaveTemplate";
        public const string sp_Administration_Inspection_SaveInspection = "sp_Administration_Inspection_SaveInspection";

        public const string sp_GetInspectionFormList = "sp_GetInspectionFormList";
        public const string sp_GetInspectionFormListByCustomer = "sp_GetInspectionFormListByCustomer";
        public const string sp_GetInspectionFormListByJob = "sp_GetInspectionFormListByJob";
        public const string sp_GetInspectionFormListByRegion = "sp_GetInspectionFormListByRegion";
        public const string sp_GetInspectionFormListByFranchisee = "sp_GetInspectionFormListByFranchisee";
        public const string sp_GetInspectionForm = "sp_GetInspectionForm";
        public const string sp_AddOrUpdateInspectionForm = "sp_AddOrUpdateInspectionForm";

        public const string sp_GetInspectionFormSectionListByForm = "sp_GetInspectionFormSectionListByForm";
        public const string sp_GetInspectionFormSection = "sp_GetInspectionFormSection";
        public const string sp_AddOrUpdateInspectionFormSection = "sp_AddOrUpdateInspectionFormSection";
        public const string sp_GetInspectionFormItemBySection = "sp_GetInspectionFormItemBySection";
        public const string sp_GetInspectionFormItem = "sp_GetInspectionFormItem";
        public const string sp_AddOrUpdateInspectionFormItem = "sp_AddOrUpdateInspectionFormItem";
        public const string sp_UpdateInspectionFormItem = "sp_UpdateInspectionFormItem";
        public const string sp_DeleteInspectionFormSection = "sp_DeleteInspectionFormSection";
        public const string sp_DeleteInspectionFormItem = "sp_DeleteInspectionFormItem";

        public const string sp_GetInspectionFormHistoryList = "sp_GetInspectionFormHistoryList";
        public const string sp_GetInspectionFormHistoryListByCustomer = "sp_GetInspectionFormHistoryListByCustomer";
        public const string sp_GetConsolidatedInspectionFormHistoryByCustomer = "sp_GetConsolidatedInspectionFormHistoryByCustomer";
        public const string sp_GetInspectionFormHistoryListByJob = "sp_GetInspectionFormHistoryListByJob";
        public const string sp_GetInspectionFormHistoryListByRegion = "sp_GetInspectionFormHistoryListByRegion";
        public const string sp_GetInspectionFormHistoryListByFranchisee = "sp_GetInspectionFormHistoryListByFranchisee";
        public const string sp_GetInspectionFormHistory = "sp_GetInspectionFormHistory";
        public const string sp_AddOrUpdateInspectionFormHistory = "sp_AddOrUpdateInspectionFormHistory";

        public const string sp_GetInspectionFormSectionHistoryListByForm = "sp_GetInspectionFormSectionHistoryListByForm";
        public const string sp_GetInspectionFormSectionHistory = "sp_GetInspectionFormSectionHistory";
        public const string sp_AddOrUpdateInspectionFormSectionHistory = "sp_AddOrUpdateInspectionFormSectionHistory";
        public const string sp_GetInspectionFormItemHistoryBySection = "sp_GetInspectionFormItemHistoryBySection";
        public const string sp_GetInspectionFormItemHistory = "sp_GetInspectionFormItemHistory";
        public const string sp_AddOrUpdateInspectionFormItemHistory = "sp_AddOrUpdateInspectionFormItemHistory";
        public const string sp_UpdateInspectionFormItemHistory = "sp_UpdateInspectionFormItemHistory";
        public const string sp_DeleteInspectionFormSectionHistory = "sp_DeleteInspectionFormSectionHistory";
        public const string sp_DeleteInspectionFormItemHistory = "sp_DeleteInspectionFormItemHistory";

        #endregion

        #region Template

        public const string admin_GetTemplateList = "admin_GetTemplateList";
        public const string admin_GetTemplateForId = "admin_GetTemplateForId";
        public const string sp_Template_SaveTemplateArea = "sp_Template_SaveTemplateArea";
        public const string sp_Template_SaveTemplateItem = "sp_Template_SaveTemplateItem";
        public const string sp_Template_SaveTemplateQuestion = "sp_Template_SaveTemplateQuestion";
        public const string sp_Template_SaveTemplate = "sp_Template_SaveTemplate";
        public const string sp_getFormTemplateList = "sp_getFormTemplateList";
        public const string sp_getAccountTypeList = "sp_getAccountTypeList";
        public const string sp_GetTemplates = "sp_GetTemplates";
        public const string sp_GetTemplate = "sp_GetTemplate";
        public const string sp_GetTemplateByAccountType = "sp_GetTemplateByAccountType";
        public const string sp_AddOrUpdateFormTemplate = "sp_AddOrUpdateFormTemplate";
        public const string sp_DeleteTemplate = "sp_DeleteTemplate";
        public const string sp_GetTemplateAreaList = "sp_GetTemplateAreaList";
        public const string sp_GetTemplateArea = "sp_GetTemplateArea";
        public const string sp_AddOrUpdateTemplateAreaToForm = "sp_AddOrUpdateTemplateAreaToForm";
        public const string sp_DeleteTemplateAreaFromTemplate = "sp_DeleteTemplateAreaFromTemplate";
        public const string sp_GetTemplateAreaItemList = "sp_GetTemplateAreaItemList";
        public const string sp_GetTemplateAreaItem = "sp_GetTemplateAreaItem";
        public const string sp_GetTemplateAreaItemByArea = "sp_GetTemplateAreaItemByArea";
        public const string sp_AddOrUpdateTemplateAreaItem = "sp_AddOrUpdateTemplateAreaItem";
        public const string sp_AddOrUpdateTemplateAreaItemToArea = "sp_AddOrUpdateTemplateAreaItemToArea";
        public const string sp_DeleteTemplateAreaItem = "sp_DeleteTemplateAreaItem";
        public const string sp_DeleteTemplateAreaItemFromArea = "sp_DeleteTemplateAreaItemFromArea";

        #endregion

        #region Feature EMailid

        public const string spSave_Sys_FeatureEmail = "spSave_Sys_FeatureEmail";

        #endregion

        #region Job

        public const string sp_GetJobListByRegion = "sp_GetJobListByRegion";
        public const string sp_GetJobListByFranchisee = "sp_GetJobListByFranchisee";
        public const string sp_GetJobListByCustomer = "sp_GetJobListByCustomer";
        public const string sp_GetJobList = "sp_GetJobList";
        public const string sp_AddOrUpdateJob = "sp_AddOrUpdateJob";
        public const string sp_UpdateJob = "sp_UpdateJob";
        public const string sp_GetJob = "sp_GetJob";

        #endregion

        #region CRM

        public const string sp_GetCrmCallLogListByAccount = "sp_GetCrmCallLogListByAccount";
        public const string sp_GetCrmCallLogListByAccountCustomerDetail = "sp_GetCrmCallLogListByAccountCustomerDetail";
        public const string sp_GetCrmCallLog = "sp_GetCrmCallLog";
        public const string sp_AddOrUpdateCrmCallLog = "sp_AddOrUpdateCrmCallLog";
        public const string CRM_spGet_PotentialList = "CRM_spGet_PotentialList";

        #endregion
    }
}
