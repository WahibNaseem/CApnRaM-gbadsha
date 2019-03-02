using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Helper
{
    public class Constants
    {
        public const int OutputCacheExpireInSecond = 0;

        public static String App_Name = "Web App";

        public static int DefaultCellWidth = 925;

        public static int ImportedData = 0;

        public static int ImportedUserBy = -1;
        public static int ImportedReadOnly = -2;

        public static int Empty = -1;
        public static int AllRecords = -2;

        public static int ReportDivider = -9;


        // cj - 7/13/2016 -- a sp will return a 0 if it fails
        public static int DB_Error = 0;

        public static int OfferDeclined = 0;
        public static int OfferAccepted = 1;

        public static int ConfigurationValueTypeString = 0;
        public static int ConfigurationValueTypeNumeric = 1;
        public static int ConfigurationValueTypeYesNo = 2;
        public static int ConfigurationValueTypeCurrency = 3;
        public static int ConfigurationValueTypePercentage = 4;
        public static int ConfigurationValueTypeMessage = 5;
        public static int ConfigurationValueTypeLink = 6;

        public static string Invoice = "invoice";
        public static string SplitTrx = "splittrx";

        public static int FranchiseeReports = 1;
        public static int MonthlyTotal = 2;
        public static int MonthlyTotalContractBilling = 3;

        public static int Yes = 1;
        public static int No = 0;

        public static int TemporaryTransaction = 0;

        public static int ReadOnly = 1;
        public static int NotReadonly = 0;

        public static int Disabled = 1;
        public static int NotDisabled = 0;


        public static int Credit = -1;
        public static int DeleteCredit = 1;

        public static int TurnAroundChecksNotGenerated = -1;
        public static int TurnAroundChecksGenerated = 0;
        public static int TurnAroundChecksPosted = 1;

        public static int NegativeDueAll = -1;
        public static int NegativeDueOpen = 1;
        public static int NegativeDueClosed = 2;

        public static string NegativeDueStatusOpen = "Open";
        public static string NegativeDueStatusClosed = "Closed";


        public static int CustomerSupplies = 29;
        public static int TEMPORARY_COMMISSION = 28;

        public static int FeeTypeAdvertising = 10;
        public static int FeeTypeTechnology = 9;
        public static int FeeTypeAccounting = 1;
        public static int FeeTypeRoyalty = 17;
        public static int FeeTypeAdditionalBillingByOffice = 19;
        public static int FeeTypeBusinessProtection = 23;

        public static int SearchByHeaderId = -1;
        public static int SearchByInvoiceNo = -2;
        public static int SearchByCustomer = -3;
        public static int SearchbyCustomerCTRXID = -4;
        public static int SearchbyIdentityID = -5;
        public static int SearchbyDetailID = -6;
        public static int SearchByPaymentID = -7;
        public static int SearchByCreditID = -8;
        public static int SearchPaymentDetail = -9;

        public static int ARApplyToBalanceAndTax = -1;
        public static int ARApplyToBalanceOnly = -2;
        public static int ARApplyToTaxBalanceOnly = -3;

        public static int InvoiceSearchByCustomerNo = 5;

        public static int CollectionsCallStatusOther = 5;

        public static string CustomerTaxAuthLocation = "tbl_C_BillSettings";
        public static string FranchiseeTaxAuthLocation = "tbl_F_Information";
        public static string CustomerID = "CustomerID";
        public static string FranchiseeID = "id";

        public static int TroubleTransferWithFee = 1;
        public static int TroubleTransferWithoutFee = 2;

        //MG -- 9/19/2014 -- Added 3 lines below for Contract increase decrease log.
        public static int Increase = 1;
        public static int Decrease = 2;

        //MG -- 9/19/2014 -- Added 3 lines below for DETAIL increase decrease log.
        public static int IncreaseDecreaseCPI = 1;
        public static int IncreaseDetail = 2;
        public static int DecreaseDetail = 3;

        //public static int DeleteFTRXById = -1;
        //public static int DeleteFTRX = -2;
        //public static int DeleteFTRXByCTRXId = -2;

        public static string StringTrue = "1";
        public static string StringFalse = "0";

        public static int Active = 1;
        public static int Inactive = 0;
        public static int Cancelled = 2;
        public static int Suspended = 3;
        public static int Pending = 4;
        public static int CeaseToDoBusiness = 5;
        public static int Hold = 6;
        public static int Stop = 7;
        public static int Paid = 8;
        public static int Transfer = 9;


        //MG - 8/5/2014 - added
        public static string ActiveStatus = "Active";
        public static string HoldStatus = "On Hold";
        public static string PaidStatus = "Paid";
        public static string StopStatus = "Stopped";
        public static string TransferStatus = "Transferred";
        public static string PendingStatus = "Pending";


        //MG -- 10-15-2014 -- Bill COnfig table columns
        public static string ChargeBackGenerated = "chargebackgenerated";

        //MG - 8/5/2014 - commented
        //public static int FranchiseRecurringTrxStatusActive = 1;
        //public static int FranchiseRecurringTrxStatusOnHold = 2;
        //public static int FranchiseRecurringTrxStatusPaid = 3;
        //public static int FranchiseRecurringTrxStatusStopped = 4;

        //public static int LeaseStatusActiveValue = 1;
        //public static int LeaseStatusHoldValue = 2;
        //public static int LeaseStatusPaidValue = 3;
        //public static int LeaseStatusStopValue = 4;
        //public static int LeaseStatusTransferValue = 5;

        //public static string LeaseStatusActive = "Active";
        //public static string LeaseStatusHold = "On Hold";
        //public static string LeaseStatusPaid = "Paid";
        //public static string LeaseStatusStop = "Stopped";
        //public static string LeaseStatusTransfer = "Transferred";


        public static int TermNet30 = 1;
        public static int TermNet40 = 2;
        public static int TermNet60 = 3;

        public static int RatePercent = 0;
        public static int RateFlat = 1;

        public static int OwnerPrimary = 1;
        public static int Owner = 0;

        public static int CreditValue = 1;
        public static int DebitValue = 0;

        public static String CreditDescription = "Credit";
        public static String Debit = "Debit";

        public static char[] SplitterComma = { ',' };
        public static char[] SplitterBar = { '|' };
        public static char[] SplitterPound = { '#' };
        public static char[] SplitterDash = { '-' };
        public static char[] SplitterForwardSlash = { '/' };
        public static char[] SplitterColon = { ':' };
        public static char[] SplitterSemiColon = { ';' };
        public static char[] SplitterEqual = { '=' };

        //public static char[] periodchar = {'.'};

        public static string NullString = null;
        public static String Bar = "|";
        public static String Pound = "#";
        public static String Dash = "-";
        public static String ForwardSlash = "/";
        public static String Apostrophe = "`";
        public static String PercentSign = "%";
        public static String Colon = ":";
        public static String SemiColon = ";";
        public static String QuestionMark = "?";
        public static String Equal = "=";
        public static String Asterisk = "*";

        public static string Period = ".";

        public static String NoAddtlBillByOfficeFee = "";

        public static int Checked = 1;
        public static int Unchecked = 0;

        public static int Login_Succeeded = 7;
        public static int Login_Failed = 6;
        public static int Data_Returned = 5;
        public static int No_Data_Returned = 4;
        public static int Need_Region = 3;
        public static int Shut_Down = 2;
        public static int Success = 0;
        public static int Failed = -1;
        public static int FailedwRedirect = -2; //-- should be deprecated
        public static int SQLCallFailed = -1;
        public static int False = 0;
        public static int True = 1;

        public static int TypeInvoice = 1;
        public static int TypeInvoiceConsolidated = 2;
        public static int TypePastDueStatement = 3;
        public static int TypeChargebackLetter = 4;

        public static int TypeBeginningofMonthInvoioceDate = 1;
        public static int TypeEndofMonthInvoioceDate = 2;

        //public static string DirectoryPath = HttpContext.Current.Server.MapPath(jSystemIO.TempDirectory).ToString() + Guid.NewGuid().ToString() + AppCore.Period + jSystemIO.FileExtPDF;

        //-- ajax return indexes
        public static int AjaxReturnIndexResult = 0;
        public static int AjaxReturnIndexCallType = 1;
        public static int AjaxReturnIndexConfirmCommand = 2;
        public static int AjaxReturnIndexMessage = 3;
        public static int AjaxReturnIndexMessageWindowHeight = 4;
        public static int AjaxReturnIndexParams = 5;

        //-- ajax return call types
        public static string AjaxCallValidate = "v";
        public static string AjaxCallSearchFranchise = "sf";
        public static string AjaxCallSearchCustomer = "sc";
        public static string AjaxCallSearchInvoice = "si";
        public static string AjaxCallSave = "s";
        public static string AjaxCallDelete = "d";
        public static string AjaxCallLoadTax = "lt";
        public static string ajaxCallTypeRoundTax = "rt";
        public static string AjaxCallLoadPayment = "lp";
        public static string AjaxCallConfirmSaveBillingPeriod = "csbp";

        //--??????
        public static string AjaxCallFranchiseeDistributionTax = "fdt";
        public static string AjaxCallDeletePayment = "dp";
        public static string AjaxCallApplyOverPayment = "aop";
        public static string AjaxCallApplyMultipleFranOverPayment = "amop";
        public static string AjaxReturnCallAddtlBillingOfficeFee = "bof";
        public static string AjaxCallRedirect = "are";
        public static string AjaxCallResubmit = "aresub";

        //-- ajax return call commands.... 
        //-- NEED TO RENAME "AjaxReturnCommandZero" NEEDS TO BE MORE DESCRIPTIVE YET STANDARD...
        public static string AjaxReturnCommandCancel = "0";
        public static string AjaxReturnCommandZero = "0";

        public static String Query_Info_Table = "queryinfo";

        public static int Required = 0;
        public static int Optional = 1;
        public static int RequiredNotZero = 2;

        public static string AlignmentCenter = "center";

        public static int InputTypePhone = 0;
        public static int InputTypeDate = 2;
        public static int InputTypeText = 3;
        public static int InputTypeEmail = 4;
        public static int InputTypePostalCode = 5;
        public static int InputTypeCurrency = 6;
        public static int InputTypeCredit = 7;
        public static int InputTypeDecimal = 8;
        public static int InputTypeInteger = 9;
        public static int InputTypePercent = 11;



        public const int CheckTypeFranchiseeDue = 1;
        public const int CheckTypeAccountRebate = 2;
        public const int CheckTypeTurnAround = 3;
        public const int CheckTypeNegativeDueTransfer = 4;
        public const int CheckTypeTestAccountRebate = 5;
        public const int CheckTypeMonthlyTurnAround = 7;


        public const int RegisterTypeRefundCheck = 30;

        public const int RegisterTypeGeneratedAuto = 1;
        public const int RegisterTypeGeneratedManual = 2;


        public static string ObjectCustomer = "Customer";
        public static string ObjectFranchisee = "Franchsiee";

        public static int PayToTypeCustomer = 1;
        public static int PayToTypeFranchisee = 2;

        /* NEED TO REMOVED.....*/
        // public static int InputTypeNumeric = 1;
        public static int InputTypeFee = 10;

        public static String IconNone = "";
        public static String IconAdd = "add.png";
        public static String IconEdit = "edit.png";
        public static String IconDelete = "delete.png";
        public static String IconUmbrella = "umbrella.jpg";
        public static String IconBack = "back.gif";
        public static String IconContacts = "contacts.png";
        public static String IconDetail = "detail.png";
        public static String IconUser = "user.gif";
        public static String IconZoomIn = "zoomin.gif";
        public static String IconCheck = "checkmark.gif";
        public static String IconEmpty = "empty.png";
        public static String IconStatusActive = "active.png";
        public static String IconStatusInactive = "inactive.png";
        public static String IconView = "view.gif";
        public static String IconOffer = "offer.png";
        public static String IconSplit = "split.jpg";
        public static String IconHistory = "history.png";
        public static String IconFoxPro = "foxpro.png";
        public static String IconNotesInternal = "internal.gif";
        public static String IconNotesNotInternal = "notinternal.png";
        public static String IconVoid = "void.png";
        public static String IconPrint = "print.gif";
        public static String IconStop = "stop.png";
        public static String IconVoided = "void.gif";
        public static String IconReconciled = "reconciled.png";
        public static String IconCheckBoxOn = "checkboxon.png";
        public static String IconDistribution = "Distribution.png";
        public static String IconChargedBack = "chargedback.png";
        public static String IconActive = "active.png";
        public static String IconHold = "hold.gif";
        public static String IconInactive = "inactive.png";
        public static String IconPending = "pending.gif";

        public static int ParentTypeCustomer = 1;
        public static int ParentTypeFranchise = 2;
        public static int ParentTypeCustomerContractDetail = 3;

        public static String MsgRecordDeleted = "Record was successfully deleted.";
        public static String MsgRecordSaved = "Record was successfully saved.";
        public static String MsgNoRecords = "No records found.";
        public static String MsgFranchiseeAlreadyAdded = "Franchisee was already added. Please enter another franchisee number.";
        public static String MsgComplete = "Process has completed.";
        public static String MsgUpdateComplete = "Records have been successfuly finalized.";
        public static String MsgNoOpenInvoice = "No open invoices found.";
        public static String MsgNoInvoicesFound = "No invoices found.";
        public static String MsgErrorOccured = "An Error has occured.";

        public static String MsgUndoRecords = "Checks have been generated. Please Undo the checks.";
        public static String MsgInvalidBillingPeriod = "There has been an error while generating the checks. Invalid billing period.";
        public static String MsgInvalidRegionInfo = "There has been an error while generating the checks. Invalid region information";
        public static String MsgInvalidBankInfo = "There has been an error while generating the checks. There is no check number or bank information";
        public static String MsgRecordsProcessed = "Records were successfully processed.";
        public static String MsgRecordsPendingToBePosted = "There are checks pending to be posted. Please post or undo the checks before generating checks for another entry.";
        public static String MsgRecordsInvalidCheckNum = "There has been an error while posting the checks. A check number has been reused. Please contact your system administrator.";
        public static String MsgRecordsInvalidFormatCheckNum = "There has been an error while generating the checks. The maximun check number in the register has an invalid format (00000001).";
        public static String MsgNoCurrentBillPeriodFound = "No open billing period found.";
        public static String MsgNoCurrentBillPeriodNotValid = "The billing period is not valid.";
        public static String MsgNoCurrentBillPeriodHasBeenSaved = "Billing period has been set.";
        public static String MsgNoCurrentBillPeriodFinalized = "Franchisee Reports have been finalized.";
        public static String MsgNoCurrentBillPeriodClosed = "Billing period [BILLMONTH]/[BILLYEAR] has been closed.";
        public static String MsgErrorMonthlyTurnAroundCheck = "There are two billing periods available for monthly checks. Please contact your system administrator.";



        public static String TransactionTypeValueInvoice = "I";
        public static String TransactionTypeValueCredit = "C";

        public static String SearchTypeName = "Name";
        public static String SearchTypeNumber = "Number";
        public static String SearchTypePhoneNumber = "Phone Number";
        public static String SearchTypeAddress = "Address";
        public static String SearchTypeCity = "City";
        public static String SearchTypeZipCode = "Zip Code";
        public static String SearchTypeCounty = "County";
        public static String SearchTypeAccountType = "Account Type";
        public static String SearchTypeContractAmount = "Contract Amount";
        public static String SearchTypeContact = "Contact";
        public static String SearchTypeOperationsMgr = "Operations Mgr.";

        public static int SearchTypeNameValue = 0;
        public static int SearchTypeNumberValue = 1;
        public static int SearchTypePhoneNumberValue = 2;
        public static int SearchTypeAddressValue = 3;
        public static int SearchTypeCityValue = 4;
        public static int SearchTypeZipCodeValue = 5;
        public static int SearchTypeCountyValue = 6;
        public static int SearchTypeAccountTypeValue = 7;
        public static int SearchTypeContractAmountValue = 8;
        public static int SearchTypeContactValue = 9;
        public static int SearchTypeOperationsMgrValue = 10;

        public static int DeleteFeeByID = 1;
        public static int DeleteFeeByRecurringId = 2;

        public static int FrequencyTypeUnknown = 3;

        public static String TaxTransactionTypeLease = "l";
        public static String TaxTransactionTypeSupply = "s";
        public static String TaxTransactionTypeContract = "c";

        public static int SearchTypeIncreaseDecreaseIncrease = 4;

        public static int NoteTypePurchasePlan = 6;
        public static int NoteTypeInitialBusinessPayment = 31;
        public static int NoteTypeOther = 33;


        public static int RegisterTypeOtherDeposits = 23;

        //public static int FranTransactionTypeLease = 11;

        public static String TransClassTypeSupply = "S";
        public static String TransClassTypeExtraWork = "E";
        public static String TransClassTypeRegularBilling = "B";
        public static String TransClassTypeInitialClean = "I";
        public static String TransClassTypeOneTimeClean = "O";

        public static int FeeRoyalty = 1;

        public static int ARStatusNormal = 3;

        public static int AgreementJaniKing = 1;
        public static int AgreementCustomer = 2;

        public static String ObjectTypeCustomer = "jCustomer";
        public static String ObjectTypeFranchisee = "jFranchisee";

        public static int ListOptionDividor = 99999;

        public static int RecordNotEditable = -2;

        public static int OtherDepositLockboxStatus = -99;
        public static int RegularPaymentLockboxStatus = 9;

        /*11-18-2015 MG added since manual payments were being applied twice when converting all 9's to 0's*/
        public static int ManualPaymentLockboxStatus = 8;

        public static int ChargeBackSearchByFTRX = 2;


        public static int sysSMTPServer = 1;
        public static int sysSMTPSendUsing = 2;
        public static int sysSMTPConnectionTimeout = 3;
        public static int sysBCCEmailAddress = 4;
        public static int sysFromEmailAddress = 5;
        public static int sysWorkingFolder = 6;
        public static int sysDebugging = 7;
        public static int sysCriticalToEmailAddress = 8;
        public static int sysCriticalFromEmailAddress = 9;
        public static int sysNonCriticalToEmailAddress = 10;
        public static int sysNonCriticalFromEmailAddress = 11;
        public static int sysWebAppNetURL = 12;
        public static int sysWebAppASPURL = 13;
        public static int sysFoxProFRADBPath = 14;
        public static int sysCorporateRegion = 15;
        public static int sysDevelopment = 16;
        public static int sysTestEmail = 16;

        public static int sysMainOfficeName = 17;
        public static int sysMainOfficeAddress = 18;
        public static int sysMainOfficeCity = 19;
        public static int sysMainOfficeState = 20;
        public static int sysMainOfficeZip = 21;
        public static int sysTraverse = 21;       /*jkcontrol variable*/
        public static int sysEmailBody = 22;
        public static int sysContractBillingDesc = 23;
        public static int sysIncludeBillingPeriodDesc = 24;
        public static int sysUseFirstDetailDesc = 25;
        public static int sysMainOfficePhone = 22;
        public static int sysMainOfficeTollFee = 23;
        public static int sysErrorLog = 17;
        //public static int sysUpload = 25;
        public static int sysCorporateAccountingToEmailAddress = 25;


        #region Security Constants

        public static int modDEVELOPER = 0;
        public static int modADMINISTRATOR = 1;
        public static int modMONTHLY_REPORTS = 2;
        public static int modFRANCHISE_INFORMATION = 3;
        public static int modFRANCHISE_TRANSACTION = 4;
        public static int modFRANCHISE_CALL = 5;
        public static int modCUSTOMER_INFORMATION = 6;
        public static int modCUSTOMER_TRANSACTION = 7;
        public static int modACCOUNTS_RECEIVABLE = 8;
        public static int modCUSTOMER_SERVICE = 9;
        public static int modCOLLECTIONS = 10;
        public static int modFRANCHISE_SALES = 11;
        public static int modCUSTOMER_SALES = 12;
        public static int modCORPORATE_ACCOUNTING = 13;
        public static int modCALENDAR = 14;
        public static int modCOMMISSIONS = 15;
        public static int modTELEMARKETING = 16;
        public static int modLEADS_DESK = 17;
        public static int modMASTER_FRANCHISE = 18;
        public static int modCORPORATE_DESK = 19;
        public static int modDOCUMENTS = 20;
        public static int modMANAGEMENT = 21;

        public static int MODULE_COUNT = 22;

        public static int LEVEL_1 = 1;
        public static int LEVEL_2 = 2;
        public static int LEVEL_3 = 3;
        public static int LEVEL_4 = 4;
        public static int LEVEL_5 = 5;
        public static int LEVEL_6 = 6;
        public static int LEVEL_7 = 7;
        public static int LEVEL_8 = 8;
        public static int LEVEL_9 = 9;
        public static int LEVEL_10 = 10;

        public static string SESSION_STATE = "d0jd830w513";
        public static string SESSION_STATE_EXPIRATION = "d51d8qe5e2";


        public static string COOKIE_USER_PROFILE_UID = "pl152ki";
        public static string USER_ID = "pl152ki";
        public static string PRIMARY_REGION_ID = "teycb";
        public static string ACCESS_TIME = "ikoe973058";
        public static string LAST_ACTIVITY = "zndil";
        public static string POWER_USER = "ujdqw1f";
        public static string MULTI_REGION_ACCESS = "kil444doj";
        public static string USER_NAME = "oke10rr867bx";
        public static string USER_FULLNAME = "a98462ujk";
        public static string USER_GROUP = "qa6ws2ed";
        public static string USER_SECURITY_RIGHTS = "ikdicmj";

        public static string REGION_ID = "poiu245yt";
        public static string REGION_NAME = "cmdnjuf";
        public static string REGION_DATABASE = "uehudjre";

        //-- temporary vars... these will be removed as needed based on the replacement of foxpro data
        public static string TMP_REGION_FP_DATAPATH = "bn44rj532mdb";

        //-- end temporary vars...

        //Bill month bill year
        public static string BILL_MONTH = "htllbm11";
        public static string BILL_YEAR = "rallby11";

        public static string BILLING_PERIOD_COUNT = "bpchc3";

        public static string LIST_PAGE = "uhdijekd";
        public static string SQL_TRAVERSE = "yfhuerjidk";

        private string _pl152ki = "UID";
        public string pl152ki { get { return _pl152ki; } }
        private string _teycb = "PRIMARY_REGION_ID";
        public string teycb { get { return _teycb; } }
        private string _ikoe973058 = "ACCESS_TIME";
        public string ikoe973058 { get { return _ikoe973058; } }
        private string _zndil = "LAST_ACTIVITY";
        public string zndil { get { return _zndil; } }
        private string _ujdqw1f = "POWER_USER";
        public string ujdqw1f { get { return _ujdqw1f; } }
        private string _kil444doj = "MULTI_REGION_ACCESS";
        public string kil444doj { get { return _kil444doj; } }
        private string _oke10rr867bx = "USER_NAME";
        public string oke10rr867bx { get { return _oke10rr867bx; } }
        private string _98462ujk = "USER_FULLNAME";
        public string a98462ujk { get { return _98462ujk; } }
        private string _ikdicmj = "RIGHTS";
        public string ikdicmj { get { return _ikdicmj; } }
        private string _poiu245yt = "REGION_ID";
        public string poiu245yt { get { return _poiu245yt; } }
        private string _cmdnjuf = "REGION_NAME";
        public string cmdnjuf { get { return _cmdnjuf; } }
        private string _uehudjre = "REGION_DATABASE";
        public string uehudjre { get { return _uehudjre; } }
        private string _bn44rj532mdb = "TMP_REGION_FP_DATAPATH";
        public string bn44rj532mdb { get { return _bn44rj532mdb; } }
        private string _htllbm11 = "BILL_MONTH";
        public string htllbm11 { get { return _htllbm11; } }
        private string _rallby11 = "BILL_YEAR";
        public string rallby11 { get { return _rallby11; } }
        private string _uhdijekd = "LIST_PAGE";
        public string uhdijekd { get { return _uhdijekd; } }
        private string _yfhuerjidk = "SQL_TRAVERSE";
        public string yfhuerjidk { get { return _yfhuerjidk; } }

        public static int MODULE_LEVEL = 0;
        public static int INDIVIDUAL_TASK_ONLY = -1;
        public static int ENCRYPT = -1;
        public static int DECRYPT = 1;

        public static int MOBILE_ACCESS = 3;

        // administrator        
        public static int ADMINISTRATOR_RESCHEDULE_RIGHTS = 0;
        public static int ADMINISTRATOR_MAINTAIN_HR = 1;
        public static int ADMINISTRATOR_MARKETING = 2;
        public static int ADMINISTRATOR_MOBILE_ACCESS = 3;
        public static int ADMINISTRATOR_COMPANY_SETUP = 4;
        public static int ADMINISTRATOR_REGION_SETUP = 5;

        // customer information
        public static int CTrx_POST = 0;

        // customer information  
        public static int CUSTOMER_ARSTATUS_NO_BANKRUPTCY = 0;
        public static int CUSTOMER_ARSTATUS_WITH_BANKRUPTCY = 1;
        public static int CUSTOMER_INSPECTION_LOCATION = 2;
        public static int CUSTOMER_LINK_NATIONAL_ACCOUNT = 3;
        public static int CUSTOMER_VIEW_BILLING = 4;
        public static int CUSTOMER_SET_DISTRIBUTION = 5;
        public static int CUSTOMER_SETUP_CONTRACT = 6;
        public static int CUSTOMER_ACTIVATE = 7;

        // customer service
        public static int CUST_SERVICE_ADDRESS_CALL_BACK = 0;
        public static int CUST_SERVICE_EMAIL_CALL_NOTES = 1;


        // franchise
        public static int FRANCHISE_FINDER_FEE = 0;
        public static int FRANCHISE_SET_STATUS = 1;

        // franchise transactions        
        public static int FTrx_POST = 0;
        public static int FTrx_EDIT = 1;
        public static int FTrx_DELETE = 2;
        public static int FTrx_ADD = 3;
        public static int FTrx_EDIT_FEES = 4;

        public static string Mogo_Credentials_Table = "credentials";
        public static string Mogo_fld_Credentials_Value = "value";

        public static int Mogo_Credentials_UserId = 0;
        public static int Mogo_Credentials_RegionId = 1;
        public static int Mogo_Credentials_SessionTicks = 2;

        public static int Link_AccessType_Customer_Id = 0;
        public static int Link_AccessType_Franchise_Id = 1;
        public static int Link_AccessType_Franchise_Lead_Id = 2;
        public static int Link_AccessType_Customer_Lead_Id = 3;


        public static int Page_Access_Customer_Evaluation = 0;

        #endregion

        #region UI
        public static string CheckboxEmptyValue = "-1000";

        public static int CellWidthFull = 0;
        public static int CellWidthDefalut = -1;

        public static int MessageTypeWarning = 0;
        public static int MessageTypeNotification = 1;
        public static int MessageTypeError = 2;

        public static string NO_MATCHES = "No Matches Found.";

        #endregion
    }
}
