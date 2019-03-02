using JKApi.Data.DAL;
using JKViewModels;
using JKViewModels.CRM;
using JKViewModels.CRM.CRMSPModels;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JKViewModels.Inspection;

namespace JKApi.Service.ServiceContract.CRM
{
    public interface ICRM_Service
    {
        #region CRM_AccountService > Queries 
        IQueryable<CRM_CallLog> GetAll_CRM_CallLog();
        IQueryable<CRM_CloseType> GetAll_CRM_CloseType();
        IQueryable<CRM_ReasonType> GetAll_CRM_ReasonType();
        IQueryable<CRM_PurposeType> GetAll_CRM_PurposeType();
        IQueryable<CRM_Account> GetAll_CRM_Account();
        IQueryable<CRM_Territory_New> GetAll_CRM_Territory_New();
        int GetCRMTerritoryId(int SelectedRegionId, string Name);
        IQueryable<CRM_Territory_Assignment_New> GetAll_CRM_Territory_Assignment_New();
        IQueryable<CRM_LeadGeneration> GetAll_CRM_LeadGeneration();
        IQueryable<CRM_FranchiseFollowUp> GetAll_CRM_FranchiseFollowUp();
        IQueryable<CRM_SignAgreement> GetAll_CRM_SignAgreement();
        IQueryable<CRM_FranchiseContract> GetAll_CRM_FranchiseContract();
        IQueryable<CRM_CloseTempDocument> GetAll_CRM_CloseTempDocument();
        IQueryable<CRM_Territory> GetAll_CRM_Territory();
        IQueryable<CRM_CallResult> GetAll_CRM_CallResult();
        IQueryable<CRM_NoteType> GetAll_CRM_NoteType();
        IQueryable<CRM_SalePossibilityType> GetAll_CRM_SalePossibilityType();
        IQueryable<CRM_ScheduleType> GetAll_CRM_ScheduleType();
        IQueryable<CRM_Territory_Assignment> GetAll_CRM_TerriAssignment();
        IQueryable<CRM_SalesTerritory_Assignment> GetAll_CRM_SalesTerriAssignment();
        IQueryable<CRM_Contact> GetAll_CRM_Contact();
        IQueryable<CRM_ContactType> GetAll_CRM_ContactType();
        IQueryable<CRM_Activity> GetAll_CRM_Activity();
        IQueryable<CRM_AccountCustomerDetail> GetAll_CRM_AccountCustomerDetail();
        IQueryable<CRM_AccountFranchiseDetail> GetAll_CRM_AccountFranchiseDetail();
        IQueryable<CRM_TimeLine> GetAll_CRM_TimeLine();
        IQueryable<CRM_Note> GetAll_CRM_Note();
        IQueryable<CRM_Schedule> GetAll_CRM_Schedule();
        List<CRM_Schedule> GetAll_CRM_ScheduleWithCustomer(int ClassId);
        IQueryable<CRM_StageStatusSchedule> GetAll_CRM_StageStatusSchedule();
        IQueryable<CRM_Document> GetAll_CRM_Document();
        IQueryable<CRM_InitialCommunication> GetAll_CRM_InitialCommunication();
        IQueryable<CRM_FvPresentation> GetAll_CRM_FvPresentation();
        IQueryable<CRM_Bidding> GetAll_CRM_Bidding();
        IQueryable<CRM_Quotation> GetAll_CRM_Quotation();
        IQueryable<CRM_Task> GetAll_CRM_Task();
        IQueryable<CRM_TaskType> GetAll_CRM_TaskType();
        IQueryable<CRM_Stage> GetAll_CRM_Stage();
        IQueryable<CRM_StageStatus> GetAll_CRM_StageStatus();
        IQueryable<CRM_IndustryType> GetAll_CRM_IndustryType();
        IQueryable<CRM_ProviderSource> GetAll_CRM_ProviderSource();
        IQueryable<CRM_ProviderType> GetAll_CRM_ProviderType();
        CRM_ProviderType GetCRM_ProviderTypeById(int Id);
        IQueryable<CRM_AccountType> GetAll_CRM_AccountType();
        IQueryable<CRM_ActivityOutcomeType> GetAll_CRM_ActivityOutcomeType();
        IQueryable<CRM_ActivityType> GetAll_CRM_ActivityType();
        IQueryable<CRM_TimeLineType> GetAll_CRM_TimeLineType();

        #endregion

        #region CRM_AccountService > Queries Get by Id
        CRM_CallLog GetCRM_CallLogbyId(int id);
        CRM_CloseType GetCRM_CloseTypebyId(int id);
        CRM_ReasonType GetCRM_ReasonTypebyId(int id);
        CRM_PurposeType GetCRM_PurposeTypebyId(int id);
        CRM_Account GetCRM_AccountbyId(int id);
        CRM_LeadGeneration GetCRM_LeadGenerationById(int id);
        CRM_FranchiseFollowUp GetCRM_FranchiseFollowUpById(int id);
        CRM_SignAgreement GetCRM_SignAgreementById(int id);
        CRM_FranchiseContract GetCRM_FranchiseContractById(int id);
        CRM_Territory GetCRM_TerritoryById(int id);
        CRM_CallResult GetCRM_CallResultById(int id);
        CRM_NoteType GetCRM_NoteTypeById(int id);
        CRM_SalePossibilityType GetCRM_SalePossibilityTypeById(int id);
        CRM_ScheduleType GetCRM_ScheduleTypeById(int id);
        CRM_Territory_Assignment GetCRM_TerriAssignmentyById(int id);
        CRM_SalesTerritory_Assignment GetCRM_SalesTerriAssignmentyById(int id);
        CRM_CloseTempDocument GetCRM_CloseTempDocumentById(int id);
        CRM_Contact GetCRM_ContactById(int id);
        CRM_ContactType GetCRM_ContactTypeById(int id);
        CRM_Note GetCRM_Note_ByAccountCustomerNoteId(int id);
        CRM_Activity GetCRM_ActivityId(int id);
        CRM_Task GetCRM_TaskbyId(int id);
        CRM_Quotation GetCRM_QuotationbyId(int id);
        CRM_Schedule GetCRM_SchedulebyId(int id);
        CRM_AccountCustomerDetail GetCRM_AccountCustomerDetailbyId(int id);
        CRM_Document GetCRM_DocumentById(int id);
        CRM_InitialCommunication GetCRM_InitialCommunicationById(int id);
        CRM_FvPresentation GetCRM_FvPresentationBydId(int id);
        CRM_Bidding GetCRM_BiddingById(int id);

        #endregion

        #region CRM_AccountService > Queries Get by Other Fields

        CRM_Schedule GetCRM_Schedule_ByOutlookAppointmentGuid(Guid guid);

        CRM_AccountCustomerDetail GetCRM_AccountCustomerDetail_ByEmail(string email);
        CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetail_ByEmail(string email);

        #endregion

        #region CRM_AccountService > Save/Update
        CRM_CallLog SaveCRM_CallLog(CRM_CallLog CRM_CallLog);
        CRM_CloseType SaveCRM_CloseType(CRM_CloseType CRM_CloseType);
        CRM_ReasonType SaveCRM_ReasonType(CRM_ReasonType CRM_ReasonType);
        CRM_PurposeType SaveCRM_PurposeType(CRM_PurposeType CRM_PurposeType);
        CRM_Account SaveCRM_Account(CRM_Account CRM_Account);
        CRM_LeadGeneration SaveCRM_LeadGeneration(CRM_LeadGeneration CRM_LeadGeneration);
        CRM_FranchiseFollowUp SaveCRM_FranchiseFollowUp(CRM_FranchiseFollowUp CRM_FranchiseFollowUp);
        CRM_SignAgreement SaveCRM_SignAgreement(CRM_SignAgreement CRM_SignAgreement);
        CRM_FranchiseContract SaveCRM_FranchiseContract(CRM_FranchiseContract CRM_FranchiseContract);
        CRM_CloseTempDocument SaveCRM_CloseTempDocument(CRM_CloseTempDocument CRM_CloseTempDocument);
        CRM_Territory SaveCRM_Territory(CRM_Territory CRM_Territory);
        CRM_CallResult SaveCRM_CallResult(CRM_CallResult CRM_CallResult);
        CRM_NoteType SaveCRM_NoteType(CRM_NoteType CRM_NoteType);
        CRM_SalePossibilityType SaveCRM_SalePossibilityType(CRM_SalePossibilityType CRM_SalePossibilityType);

        CRM_ScheduleType SaveCRM_ScheduleType(CRM_ScheduleType CRM_ScheduleType);
        CRM_Territory_Assignment SaveCRM_TerriAssignment(CRM_Territory_Assignment CRM_TerriAssignment);
        CRM_SalesTerritory_Assignment SaveCRM_SalesTerriAssignment(CRM_SalesTerritory_Assignment CRM_SalesTerriAssignment);
        CRM_Contact SaveCRM_Contact(CRM_Contact CRM_Contact);
        CRM_ContactType SaveCRM_ContactType(CRM_ContactType CRM_ContactType);
        CRM_Activity SaveCRM_Activity(CRM_Activity CRM_Activity);
        CRM_AccountCustomerDetail SaveCRM_AccountCustomerDetail(CRM_AccountCustomerDetail CRM_AccountCustomerDetail);
        void UpdateCRM_AccountCustomerDetail_Coordinate(CRM_AccountCustomerDetail CRM_AccountCustomerDetail);
        CRM_AccountFranchiseDetail SaveCRM_AccountFranchiseDetail(CRM_AccountFranchiseDetail CRM_AccountFranchiseDetail);
        CRM_TimeLine SaveCRM_TimeLine(CRM_TimeLine CRM_TimeLine);
        CRM_Note SaveCRM_Note(CRM_Note CRM_Note);
        CRM_Schedule SaveCRM_Schedule(CRM_Schedule CRM_Schedule);
        CRM_Document SaveCRM_Document(CRM_Document CRM_Document);
        CRM_Document GetCRMDocumentWithAccountCustomer_FileType(int CRM_AccountCustomerDetailId, int FileTypeListId);
        void DeleteCRM_Document(int id);

        CRM_Document GetUploadDocumentById(int id);
        CRM_InitialCommunication SaveCRM_InitialCommunication(CRM_InitialCommunication CRM_InitialCommunication);
        CRM_FvPresentation SaveCRM_FvPresentation(CRM_FvPresentation CRM_fvPresentation);
        CRM_Bidding SaveCRM_Bidding(CRM_Bidding CRM_Bidding);
        void UpdateInActiveCRMLeadStageData(int CRM_AccountCustomerDetailId);
        CRM_PdAppointment SaveCRM_PdAppointment(CRM_PdAppointment CRM_PdAppointment);
        CRM_FollowUp SaveCRM_FollowUp(CRM_FollowUp CRM_FollowUp);
        CRM_Close SaveCRM_Close(CRM_Close CRM_Close);
        CRM_StageStatusSchedule SaveCRM_StageStatusSchedule(CRM_StageStatusSchedule CRM_StageStatusSchedule);
        CRM_Quotation SaveCRM_Quotation(CRM_Quotation CRM_Quotation);
        CRM_Task SaveCRM_Task(CRM_Task CRM_Task);
        CRM_TaskType SaveCRM_TaskType(CRM_TaskType CRM_TaskType);
        CRM_Stage SaveCRM_Stage(CRM_Stage CRM_Stage);
        CRM_StageStatus SaveCRM_StageStatus(CRM_StageStatus CRM_StageStatus);
        CRM_IndustryType SaveCRM_IndustryType(CRM_IndustryType CRM_IndustryType);
        CRM_ProviderSource SaveCRM_ProviderSource(CRM_ProviderSource CRM_ProviderSource);
        CRM_ProviderType SaveCRM_ProviderType(CRM_ProviderType CRM_ProviderType);
        CRM_AccountType SaveCRM_AccountType(CRM_AccountType CRM_AccountType);
        CRM_ActivityOutcomeType SaveCRM_ActivityOutcomeType(CRM_ActivityOutcomeType CRM_ActivityOutcomeType);
        CRM_ActivityType SaveCRM_ActivityType(CRM_ActivityType CRM_ActivityType);
        CRM_TimeLineType SaveCRM_TimeLineType(CRM_TimeLineType CRM_TimeLineType);

        #endregion

        #region CRM_AccountService > Delete

        void DeleteCRM_CallLog(int id);
        void DeleteCRM_CloseType(int id);
        void DeleteCRM_ReasonType(int id);
        void DeleteCRM_PurposeType(int id);
        void DeleteCRM_Account(int id);
        void DeleteCRM_LeadGeneration(int id);
        void DeleteCRM_FranchiseFollowUp(int id);
        void DeleteCRM_SignAgreement(int id);
        void DeleteCRM_FranchiseContract(int id);
        void DeleteCRM_Territory(int id);
        void DeleteCRM_CallResult(int id);

        void DeleteCRM_ScheduleType(int id);
        void DeleteCRM_TerriAssignment(int id);
        void DeleteCRM_SalesTerriAssignment(int id);
        void DeleteCRM_Contact(int id);
        void DeleteCRM_ContactType(int id);
        void DeleteCRM_Activity(int id);
        void DeleteCRM_AccountCustomerDetail(int id);
        void DeleteCRM_AccountFranchiseDetail(int id);
        void DeleteCRM_TimeLine(int id);
        void DeleteCRM_Note(int id);
        void DeleteCRM_Schedule(int id);
        void DeleteCRM_Quotation(int id);
        void DeleteCRM_Task(int id);
        void DeleteCRM_TaskType(int id);
        void DeleteCRM_Stage(int id);
        void DeleteCRM_StageStatus(int id);
        void DeleteCRM_IndustryType(int id);
        void DeleteCRM_ProviderSource(int id);
        void DeleteCRM_ProviderType(int id);
        void DeleteCRM_AccountType(int id);
        void DeleteCRM_ActivityOutcomeType(int id);
        void DeleteCRM_ActivityType(int id);
        void DeleteCRM_TimeLineType(int id);

        #endregion

        #region AccountService > Custom Calls

        List<CRM_spGet_PotentialList_Result> CRM_LeadCustomerSearch(CRMSearchModel crmSearchModel);
        List<CRMPotentialFranchiseeViewModel> CRM_LeadFranchiseSearch(CRMSearchModel crmSearchModel);
        CRM_InitialCommunication GetCRM_InitialCommunication_ByAccountFranchiseId(int id);
        CRM_FranchiseContract GetCRM_FranchiseContract_ByAccountFranchiseId(int id);
        CRM_SignAgreement GetCRM_SignAgreement_ByAccountFranchiseId(int id);
        CRM_FranchiseFollowUp GetCRM_FranchiseFollowUp_ByAccountFranchiseDetailId(int id);
        List<CRM_Account> GetAll_CRM_Account_ByFranchiseDetail(int CRM_Stage);
        List<CRM_Account> GetAll_CRM_Account_ByStage(int crmStage);
        List<CRM_Account> GetAll_CRM_Account_ByStageStatus(int crmStageStatus);
        List<CRMPotentialCustomerViewModel> GetAll_CRM_PotentialCustomer(int choice, int type = 0, int user = 0, int region = 0, int loginUserId = 0);
        CRMPotentialCustomerListViewModel GetPotentialCustomerList(CRMPotentialCustomerListViewModel model);
        List<CRMPotentialCustomerViewModel> GetAll_CRM_PotentialCustomer_SearchRegionSalesPerson(int choice, int type = 0, int user = 0, string region = null, int loginUserId = 0);
        List<CRMPotentialFranchiseeViewModel> GetAll_CRM_PotentialFranchisee(int choice, int type = 0, int user = 0, int region = 0);
        string Get_CRM_PreviousOrNextLead(int choice, int leadId);
        CRM_Account AddNewCustomerAccount(string firstName, string lastName, string phoneNumber, string emailAddress, int? providerType, int? providerSource, string accountType, int userId, int regionId, string Note = "");
        CRM_InitialCommunication AddNewInitalData(int accountid, int AccountcustomerDetailId, string contactPerson, int interestedInProposal, DateTime availableToMeet, int purpose, string note, int userId, int RegionId);

        void DeleteAllCRMSalesTerritoryAssignment();
        void SaveAllCRMSalesTerritoryAssignment(SaveTerritoryAssignmentViewModel model);
        void UpdateMultiple_CRM_TerriAssignmentNew(ZipCodeAssignmentPopupModel model);
        void AddZipCode(AddZipCodePopupModel model);
        void AddTerritory(AddTerritoryPopupModel model);
        CRM_Territory_Assignment_New GetCRM_Territory_Assignment_NewByZipCode(string zipCode);
        CRM_Territory_New GetCRM_Territory_NewByNameandRegionID(string Name, int RegionID);
        string GetCRM_StageStatusName(int type);

        string GetCRM_StageName(int type);

        string GetCRM_ProviderSourceName(int type);

        string GetCRM_ProviderTypeName(int type);

        string GetCRM_AccountTypeName(int type);

        string GetCRM_ActivityTypeName(int type);

        string GetCRM_ActivityOutComeTypeName(int type);

        string GetCRM_IndustryTypeName(int type);

        string GetCRM_TaskTypeName(int type);

        string GetCRM_TimeLineTypeName(int type);
        string GetCRM_StateAbbr(int id);
        string GetCRM_StateName(string abbr);
        CRM_CloseTempDocument GetCRM_CRM_CloseTempDocument_ByAccountCustomerDetailId(int id);
        List<CRM_Activity> GetCRM_Activity_ByAccountCustomerDetailId(int id);

        List<CRM_Schedule> GetCRM_Schedule_ByAccountCustomerDetailId(int? id);

        List<AuthRoleUserViewModel> Get_AuthUserLogin(int roleid, int? territoryId = null);
        List<CRMScheduleUserHierarchy> Get_AuthUserLogin_Potential(int roleid, int loginUserId = 0, int regionId = 0);
        List<CRM_Bidding> GetCRM_Bidding_ByAccountCustomerDetailId(int id);

        List<CRM_FvPresentation> GetCRM_FvPresentation_ByAccountCustomerDetailId(int id);

        List<CRM_InitialCommunication> GetCRM_InitialCommunication_ByAccountCustomerDetailId(int id);

        List<CRM_Document> GetCRM_Document_ByAccountCustomerDetailId(int id);
        List<CRM_Document> GetCRM_Document_ByAccountFranchiseDetailId(int id);

        List<CRM_StageStatusSchedule> GetCRM_StageStatusSchedules_ByAccountCustomerDetailById(int? id);

        List<CRM_StageStatusSchedule> GetCRM_StageStatusSchedules_ByAccountFranchiseDetailById(int? id);

        CRM_InitialCommunication Get_InitialCommunication_ByAccountCustomerDetailById(int id);
        CRM_InitialCommunication GetCRM_InitialCommunication_ByAccountFranchiseDetailById(int id);
        CRM_Contact Get_Contact_ByAccountCustomerDetailById(int id);

        CRM_FvPresentation Get_fvPresentation_ByAccountCustomerDetailById(int id);

        CRM_Bidding Get_Bidding_ByAccountCustomerDetailById(int id);

        CRM_StageStatusSchedule Get_StageStatusSchedule_PdAppoint(int id);

        CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetailbyId(int id);

        CRM_PdAppointment Get_PdAppointment_ByAccountCustomerDetailById(int id);

        CRM_FollowUp Get_FollowUp_ByAccountCustomerDetailById(int id);
        CRM_Close Get_Close_ByAccountCustomerDetailById(int id);

        CRM_AccountFranchiseDetail GetCRM_AccountFranchiseDetail_ByAccountId(int id);

        CRM_AccountCustomerDetail GetCRM_AccountCustomerDetail_ByAccountId(int id);

        CRM_LeadGeneration GetCRM_LeadGeneration_ByAccountFranchiseId(int id);

        CRM_Document GetCRM_Document_LastRecord();

        List<CRM_Note> GetCRM_Note_ByAccountCustomerDetailId(int id);

        List<CRM_Schedule> GetCRM_Schedule_ByAccountFranchiseDetailId(int id);

        List<CRM_Activity> GetCRM_Activity_ByAccountFranchiseDetailId(int id);

        List<CRM_Note> GetCRM_Note_ByAccountFranchiseDetailId(int id);

        int? GetCRM_ProviderTypeIndex(string name);

        int? GetCRM_ProviderSourceIndex(string name);

        List<CRM_Account> GetAll_CRM_Account_ByStageStatusQualifiedLead();

        int? GetCRM_StageStatusIndex(string name);
        DataTable DuplicateRecordInExcel(DataTable dt);
        int ImportExcelData(DataTable dt);
        CRM_AccountCustomerDetail ImportFranchiseExcelData(DataTable dt);
        List<AuthDepartment> GetDepartment();
        #endregion

        #region CRM_Contacts > Queries

        List<CRMContactTypeViewModel> GetContactTypes();

        #endregion

        #region Lead ReAssign

        List<SalesUser> GetAll_CRM_ReAssignLeadUserList();

        List<CRM_ReAssignTerritory> CRM_spGet_LeadReAssignTerritoryListByUserId(int UserId);

        LeadReAssignViewModel GetAll_CRM_ReAssignLeadList(int UserId, int TerritoryId);

        List<SalesUser> CRM_spGet_LeadReAssignUsersListByTerritoryId(int TerritoryId);

        bool AssignLeadsToUser(List<LeadListView> lstLeads);
        #endregion

        #region CRM Call Log

        ViewCallLogListModel GetCallLogListByAccount(ViewCallLogListModel model);
        ViewCallLogListModel GetCallLogListByAccountCustomerDetail(ViewCallLogListModel model);
        CallLogModel GetCallLog(int callLogId);
        CallLogModel AddOrUpdateCallLog(CallLogModel model);
        List<CRM_CallLogListModel> GetAll_CRMCallLog(int accountId);

        #endregion

        IQueryable<ServiceTypeList> GetServiceTypeList();

        IQueryable<FrequencyList> GetFrequencyList();

        List<JKViewModels.Customer.CleanFrequencyListViewModel> GetCleanFrequencyList();

        StageSettingViewModel GetStageSettingList(StageSettingViewModel model, string StageType = "");

        StageSettingViewModel SaveStageSetting(StageSettingViewModel model);

        CRMScheduleUserCalendarViewModel GetCRMScheduleByUser(int loginUserId, int regionId = 0, string lstUser = null, string lstType = null);

        IQueryable<AuthUserLogin> GetAllUserList();

        List<CRMScheduleUserHierarchy> GetAllUserRelation();

        List<CRMScheduleUserHierarchy> SaveUserRelation(CRMScheduleUserHierarchy userRelation, int loginId);
        CRM_Schedule SaveCRM_ScheduleData(CRM_Schedule schedule);
        IEnumerable<CRM_PurposeTypeModel> GetAllPurposeType(int statusListId);
        AuthUserLogin GetAuthUserLoginById(int userId);
    }

    public static class CRM_ServiceConstants
    {
        public const string Key_Act = "act";
        public const string Key_AhlaDirectory = "ahladirectory";
        public const string Key_AccuData = "accudata";
        public const string Key_AvaiatechPPC = "avaiatechppc";
        public const string Key_Appointment = "appointment";

        public const string Key_Busy = "busy";
        public const string Key_BizList = "bizList";
        public const string Key_BusinessJournalBkOfLists = "businessjournalbkoflists";
        public const string Key_Bidding = "bidding";


        public const string Key_Customer = "customer";
        public const string Key_Call = "call";
        public const string Key_CallIn = "callIn";
        public const string Key_ColdCall = "coldcall";
        public const string Key_Corporate = "corporate";
        public const string Key_CrissCross = "crisscross";
        public const string Key_Connected = "connected";
        public const string Key_CommettoBuy = "commettobuy";
        public const string Key_Close = "close";
        public const string Key_CallBack = "callback";
        public const string Key_Contacted = "contacted";

        public const string Key_DunnAndBradstreet = "dunnandbradstreet";

        public const string Key_Email = "email";

        public const string Key_FranchiseAccounting = "franchiseaccounting";
        public const string Key_Franchise = "franchise";
        public const string Key_FvPresentation = "fvpresentation";
        public const string Key_FollowUp = "followup";
        public const string Key_FranchiseContract = "franchisecontract";

        public const string Key_GoLeadsCom = "goleadscom";
        public const string Key_GoldMine = "goldmine";

        public const string Key_Haines = "haines";
        public const string Key_Hoovers = "hoovers";
        public const string Key_HangUp = "hangup";

        public const string Key_Imported = "imported";
        public const string Key_InfoUsa = "infousa";
        public const string Key_InsideProspects = "insideprospects";
        public const string Key_InActiveCustomer = "inactivecustomer";
        public const string Key_InHouse = "inhouse";
        public const string Key_IntialCommunication = "intialcommunication";

        public const string Key_JunkLead = "junklead";
        public const string Key_JaniKingVehicle = "janikingvehicle";

        public const string Key_Lead = "lead";
        public const string Key_LeadNeedsToCallOrSetupMeeting = "leadneedstocallorsetupmeeting";
        public const string Key_LeadListService = "leadlistservice";
        public const string Key_LeftVoiceMail = "leftvoicemail";
        public const string Key_LocalChamber = "localchamber";
        public const string Key_LeadGeneration = "leadgeneration";
        public const string Key_LeftMessage = "leftmessage";
        public const string Key_LetterSent = "lettersent";

        public const string key_Meeting = "meeting";

        public const string Key_NewLead = "newlead";
        public const string Key_NewPotential = "newpotential";
        public const string Key_NewCustomer = "newcustomer";
        public const string Key_NeedFollowup = "needfollowup";
        public const string Key_NeedsRequote = "needsrequote";
        public const string Key_NoAnswer = "noanswer";
        public const string Key_Negotiation = "negotiation";
        public const string Key_NeedAssessment = "needassessment";
        public const string Key_NotQualified = "notqualified";

        public const string Key_OutSource = "outsource";
        public const string Key_Other = "other";

        public const string Key_Potential = "potential";
        public const string Key_Prestentation = "prestentation";
        public const string Key_PdAppointment = "pdappointment";
        public const string Key_PreviousCustomer = "previouscustomer";
        public const string Key_PotentialInquary = "potentialinquary";



        public const string Key_QuotationNotAccepted = "quotationnotaccepted";
        public const string Key_QuotationAccepted = "quotationaccepted";
        public const string Key_QuotationSent = "quotationsent";
        public const string Key_QualifiedLead = "qualifiedlead";

        public const string Key_ReadyForQuotation = "readyforquotation";
        public const string Key_Restaurant = "restaurant";
        public const string Key_Retail = "retail";
        public const string Key_RetirementCenter = "retirementCenter";
        public const string Key_Referral = "referral";
        public const string Key_RegionalOffice = "regionaloffice";
        public const string Key_ReferredToOps = "referredtoops";


        public const string Key_Stage_Customer = "customer";
        public const string Key_SchoolK12 = "schoolk12";
        public const string Key_ShoppingCenter = "shoppingcenter";
        public const string Key_ShowRoom = "showroom";
        public const string Key_SkatingRink = "skatingrink";
        public const string Key_Spa = "spa";
        public const string Key_SportsStadium = "sportsstadium";
        public const string Key_StadiumEventsOther = "stadiumeventsother";
        public const string Key_StorageUnit = "storageunit";
        public const string Key_Sugery = "sugery";
        public const string Key_SaleGenie = "salegenie";
        public const string Key_Sorkins = "sorkins";
        public const string Key_SignAgreement = "signagreement";
        public const string Key_SoldToLegal = "soldtolegal";

        public const string Key_Technology = "technology";
        public const string Key_TelePhoneCompany = "telephonecompany";
        public const string Key_TheaterArts = "theaterarts";
        public const string Key_TimeShare = "timeshare";
        public const string Key_TrainingCenters = "trainingcenters";
        public const string Key_TransporationCo = "transporationco";
        public const string Key_Telemarketing = "telemarketing";
        public const string Key_TradeShow = "tradeshow";
        public const string Key_TurnDownLetter = "turndownletter";

        public const string Key_UnqualifiedLead = "unqualifiedlead";
        public const string Key_Unknown = "unknown";
        public const string Key_University = "university";

        public const string Key_WrongNumber = "wrongnumber";
        public const string Key_WebSite = "website";
        public const string Key_Wrong = "wrong";

        public const string Key_YellowPages = "yellowPages";
        public const string Key_Sold = "sold";






    }
}
