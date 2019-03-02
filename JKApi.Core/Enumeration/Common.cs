using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Business.Enumeration
{
    public enum UserType
    {
        [Description("ADMIN")]
        ADMIN = 1,

        [Description("USER")]
        USER,

        [Description("CSR")]
        CSR,

        [Description("VIEWER")]
        VIEWER
    }

    public enum AddressType
    {
        [Description("SHIPPING")]
        SHIPPING = 1,

        [Description("BILLING")]
        BILLING,
    }
    public enum SearchIn
    {

        [Description("Name")]
        Name = 0,
        [Description("Number")]
        Number = 1,
        [Description("Address")]
        Address = 3,
        [Description("City")]
        City = 4,
        [Description("ZipCode")]
        ZipCode = 5,

        [Description("Account Type")]
        AccountType = 7,
        [Description("Contract Amount")]
        ContractAmount = 8,
        [Description("Contact")]
        Contact = 9,
        [Description("Operations Mgr.")]
        OperationsMgr = 10

    }

    //Filterby for Trasactions
    public enum FilterBy
    {
        [Description("None")]
        None = 1,
        [Description("Available for Revenue Adjustment Only")]
        AvailableforRevenueAdjustmentOnly = 2,
        [Description("Bill Run Transactions Only")]
        BillRunTransactionsOnly = 4,
        [Description("Manual Transactions Only")]
        ManualTransactionsOnly = 5,

    }

    public enum SearchBy
    {
        [Description("Select One")]
        SelectOne = 0,
        [Description("Customer Name")]
        CustomerName = 7,
        [Description("Customer Number")]
        CustomerNumber = 5,
        [Description("Invoice Number")]
        InvoiceNumber = 2,
        [Description("Franchisee Number")]
        FranchiseeNumber = 10,

    }

    public enum TypeList
    {
        Customer = 1,
        Franchisee = 2,
        Owner = 3,
        Contract = 4,
        Distribution = 5,
        Offering = 6,
        Lease = 7,
        CancellationPending = 17,
        CRM_AccountCustomer = 18,
        CustomerServicesCallLogStatus = 19,
        ComplaintStage = 20,
        Schedules = 21,
        FailedInspection = 22
    }

    public enum MaintenanceTypeList
    {
        CustomerStatusMaintenance = 8,
        FranchiseeStatusMaintenance = 17
        
    }

public enum InitiatedByList
    {
        JK = 0,
        Customer = 1,
        Franachisee = 2,

    }

    public enum ContactTypeList
    {
        Main = 1,
        Billing = 2,
        [Description("Physical Location")]
        PhysicalLocation = 3,
        Payee = 4,
        Owner = 5,
        Contact = 6,
        BillingContact = 7,
        EBillEmail = 8
    }

    public enum ButtonType
    {
        Save = 1,
        Continue = 2,
        Back = 3,
        Cancel = 4,
        Submit = 5
    }


    public enum IdentifierTypeList
    {
        EIN = 1,
        SSN = 2,
    }

    public enum FranchiseeStatusList
    {
        Active = 9,
        CTDB = 10,
        NonRenewed = 11,
        Pending = 12,
        Repurchased = 13,
        Terminated = 14,
        Transferred = 15,
        Rejected = 36,
        LegalCompliancePending = 37,
        Inactive = 56,
        PendingTransfer = 66
    }

    public class AppConstants
    {
        public const string ContactType_Main = "Main";
        public const string ContactType_Billing = "Billing";
        public const string ContactType_PhysicalLocation = "Physical Location";
        public const string ContactType_Payee = "Payee";
        public const string ContactType_Owner = "Owner";
    }
    public enum FeeRateTypeList
    {
        Percentage = 1,
        Amount = 6,
    }

    public enum CallLogAssociateTypeList
    {
        CollectionCallLog = 1,
        CustomerServiceCallLog = 2
    }
    public enum MasterTrxTypeList
    {
        CustomerInvoice = 1,
        CustomerPayment = 2,
        CustomerCredit = 3,
        Chargeback = 10,
        FranchiseeManualTransaction = 14,
        FindersFeeBill = 15,
    }

    public enum CustomerStatusList
    {
        Active = 1,
        Cancelled = 2,
        Suspended = 3,
        Pending = 4,
        Inactive = 6,
        Transferred = 7,
        Unknown = 8,
        Declined =19,
        Send =40,
        Accepted=41,
        Rejected = 35,
        RegionOperation = 38,
        RegionAccounting = 39,
        NotifytheFranchiseeOwne = 58,
        ContacttheCustomer = 59,
        InspecttheAccount = 60,
        DefectnotifytoFranchisee = 61,
        LetertotheCustomer = 62,
        ReInspecttherAccount = 63,
        FollowupBackontrack = 64,
        CancallationPending = 65,
        Open = 67,
        InProcess = 68,
        Closed = 69,
        ReOpen = 70,
        ComplaintLogged = 71,
        ActionsFollowUp = 72,
        FollowUp = 73,
        CompletionClosed = 74,
        CustomerServiceOperations = 75,
        CustomerSales = 76,
        FranchiseeSales = 77,
        FIComplaintLogged = 78,
        FIActionsFollowUp = 79,
        FIFollowUp = 80,
        FICompletionClosed = 81,
    }

    public enum ServiceCallLogTypeList
    {
        //Regular = 1,
        //NewAccount = 2,
        //Transfer = 3,
        //Complaint = 4,
        //FollowUp = 5,
        //Inspection = 6,
        //ContactEvalution = 7,
        //FailedInspection = 8,
        //Cancellation = 9,
        //Request = 10,
        //PendingCancellation = 11,
        //ClearAtRisk = 12,
        //Increase = 13,
        //Decrease = 14,
        //Suspension = 15,
        //FaxAComment = 16,
        //MissedClean = 17,
        //ComplaintResolved = 18,
        //ClaimIncident = 19,
        //ClearClaimIncident = 20,
        //EmailedInvoice = 21

        Regular = 1,
        Cancellation = 2,
        ClaimIncident = 3,
        ClearAtRisk = 4,
        ClearClaimIncident = 5,
        Complaint = 6,
        ComplaintResolved = 7,
        ContactEvalution = 8,
        Decrease = 9,
        EmailedInvoice = 10,
        FailedInspection = 11,
        FaxAComment = 12,
        FollowUp = 13,
        Increase = 14,
        Inspection = 15,        
        Misc = 16,
        MissedClean = 17,
        NewAccount = 18,                               
        PendingCancellation = 19,
        Request = 20,
        Rountinecall = 21,
        Suspension = 22,                
        Transfer = 23,
    }
    public enum Fee
    {
        AccountingFee = 1,
        TechnologyFee=9,
        AdvertisingFee = 10,
        Royalty = 17,
        AdditionalBillingROCommission = 19,
        BusinessProtection = 23,
        BPPAdminFee = 31,
        BPPAdminFee2 = 32,
        MinimumRoyalty = 33,
        MinimumRoyaltyTotal = 34
    }
    public enum ContractTypeList
    {
        Recurring = 1,
        OneTime = 2,
        Variable = 3
    }
    public enum FileTypeList
    {
        AccountAnalysis = 1,
        BidSheet = 2,
        Cancellation = 3,
        CleaningSchedule = 4,
        Decrease = 5,
        MaintAgreement = 6,
        NewStart = 7,
        OneTimeClean = 8,
        Suspension = 9,
        Transfer = 10,
        TaxExemptCertifucate = 11,
        Other = 12,
        WBECertification = 22,
        LettertoWrightsCorner = 23,
        AuthorizationforMark = 24,
        AmendmentofAgreement = 25,
        MarkAlvordApp = 26,
        FranchiseAgreement = 27,
        NYSWC = 28,
        InsurancePolicy = 29,
        TheftIssueRandy = 30,
        FranchiseQuestionarie = 31,
        SignedAcknowledgementofDisclosure = 32,
        FranchiseDisclosureDocument = 33,
        NDA = 34,
        ExecutedAgreement = 35,
        CleaningScheduleExecuted = 36,
        BidSheetPDF = 37,
        Increase = 38,
        AccountWalkThurs = 39,
    }
}
