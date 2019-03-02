namespace JKViewModels.Common
{
    public enum MasterDropName
    {
        AuthDepartment,
        AuthGroup,
        AuthRole,
        Region,
        AuthUserLogin,
        AuthRoleType,
        StatusList,
        CustomerStatusList,
        FranchiseeStatusList,
        TStatusList,
        CustomerStatusReasonList,
        FranchiseeStatusReasonList,
        FeatureType,
        CommissionStatusList,
        CustomerTransferStatusReasonList
    }

    public enum PeriodDropDownName
    {
        MonthlyBillRun,
        FranchiseeReport,
        ChargebackFinalized
    }


    public enum RoleType
    {
        SuperAdmin,
        Admin,
        Executive,
        Marketing
    }

    public enum HMenu
    {
        Dashboard,
        AccountReceivable,
        AccountsPayable,
        Customer,
        Company,
        CustomerService,
        Franchise,
        Administration,
        Management,
        CRMDashboard,
        CustomerSales,
        CRMLeadFranchise,
        CRMAdministration,
        BIDashboard,
        BIAdministration,
        BIManagement,
        CRMSchedules,
        CRMContacts
    }

    public enum ARPermission
    {
        Customer_New_Pending,
        Customer_Maintenance_Pending,
        Customer_Transactions_Pending,
        Account_Invoice_Pending,
        Account_Credit_Pending,
        Account_Payment_Pending,
        Finalize_Franchisee_Monthly_Reports,
        Franchisee_Chargeback_Process,
        Turnaround_Payment_Process
    }

    public enum EDPermission
    {
        Payment_Detail_Popup,
        Payment_Pending_Approval_Detail_Popup,
        Credit_Detail_Popup,
        Credit_Pending_Approval_Detail_Popup,
        Franchisee_Manual_Transaction_Detail_Popup
    }

    public enum MessageNameModel
    {
        FranchiseSecion1
    }

    public enum FeatureNameModel
    {
        New_Customer_Submit,
        New_Invoice_Create,
        Franchise_Section2,
        Franchise_Section3,
        Leads_Potential_Assignee,
        Potential_Price_Approver,
        Invoice_Ebill,
        Approve_Pending,
        Reject_Pending
    }

    public enum InspectionStatus
    {
        Unknown = 0,
        Pass = 1,
        Fail = 2,
        NeedImprovement = 3
    }

    public enum priceapproved
    {
        Sales_Manager = 1,
        Region_Director = 2,
        Corporation_VP = 3,
        President = 4
    }

    public enum InspectionCompletionStatus
    {
        Unknown = 0,
        Pending = 1,
        Completed = 2
    }

    public enum AccountWalkThruType
    {
        LightSwitches = 1,
        BreakerPanel = 2,
        ContactOffice = 3,
        StorageArea = 4,
        WaterSource = 5,
        TrashDisposal = 6,
        Recycling = 7,
        AccountSupplies = 8,
        Entry = 9,
        AlarmSystem = 10,
        RestroomPaperDispeners = 11,
        SecurityProcedures = 12,
        EmergencyNamesPhones1 = 13,
        EmergencyNamesPhones2 = 14,
        ProblemConcernComments = 15,
        SignedByMaintenance = 16,
        SignedPricePage = 17,
        SignedCleaningSchedule = 18,
        AnalysisOfAccount = 19,
        AccountBidSheet = 20,
        UploadDocument = 21,
        EmailToCustomerService = 22,
        BusinessCardAttached = 23,
        FranchiseAccounting = 24,
        AeSignatures = 25,
        OpSignatures = 26,
        RdSignatures = 27,
        FoSignatures = 28
    }

    public enum CrmFilterChoice
    {
        NewLead = 0,
        QualifiedLead = 1,
        UnqualifiedLead = 2,
        PotentialLead = 3,
        CloseLead = 4,
        CallbackLead = 5
    }
}
