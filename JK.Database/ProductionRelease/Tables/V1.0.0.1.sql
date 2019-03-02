/**
 * Module: CRM
 * Table: Create CRM_Account table
 * Created By: Divesh 
 */
GO
CREATE TABLE [dbo].[CRM_Account](
	[CRM_AccountId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[FranchiseId] [int] NULL,
	[FirstName] [nvarchar](20) NULL,
	[LastName] [nvarchar](20) NULL,
	[MiddleInitial] [nvarchar](20) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[EmailAddress] [nvarchar](50) NULL,
	[AssigneeId] [int] NULL,
	[ReporterId] [int] NULL,
	[AccountType] [int] NULL,
	[Stage] [int] NULL,
	[StageStatus] [int] NULL,
	[ProviderSource] [int] NULL,
	[ProviderType] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CRM_Account] PRIMARY KEY CLUSTERED 
(
	[CRM_AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create CRM_AccounFranchiseTempData table
 * Created By: Wahib
 */
GO
CREATE TABLE [dbo].[CRM_FranchiseTempData](
	[CRM_FranchiseTempDataId] [int] IDENTITY(1,1) NOT NULL,
	[company_no] [nvarchar](Max) NULL,
	[regionId] [nvarchar](MAX) NULL,
	[createdate] [nvarchar](MAX) NULL,
	[salutation] [nvarchar](MAX) NULL,
	[fname] [nvarchar](MAX) NULL,
	[lname] [nvarchar](MAX) NULL,
	[addr] [nvarchar](MAX) NULL,
	[addr2] [nvarchar] (MAX) NULL,
	[addr3] [nvarchar](MAX) NULL,
	[city] [nvarchar](MAX) NULL,
	[zip] [nvarchar](MAX) NULL,
	[hphone] [nvarchar](MAX) NULL,
	[wphone] [nvarchar](MAX) NULL,
	[ext] [nvarchar](MAX) NULL,
	[celphone] [nvarchar](MAX) NULL,
	[email] [nvarchar](MAX) NULL,
	[leadsource] [nvarchar](MAX) NULL,
	[initialcon] [nvarchar](MAX) NULL,
	
	[infosent] [nvarchar](MAX) NULL,
	[country] [nvarchar](MAX) NULL,
	[countryint] [nvarchar](MAX) NULL,
	[lastcontdt] [nvarchar](MAX) NULL,
	[lastcontby] [nvarchar](MAX) NULL,
	[callbadte] [nvarchar](MAX) NULL,
	[callby] [nvarchar](MAX) NULL,
	[disclosed] [nvarchar](MAX) NULL,
	[datecanbuy] [nvarchar](MAX) NULL,
	[estclosedat] [nvarchar](MAX) NULL,

	
	[fivedpaper] [nvarchar](MAX) NULL,
	[status] [nvarchar](MAX) NULL,
	[solddate] [nvarchar](MAX) NULL,
	[soldby] [nvarchar](MAX) NULL,
	[franby] [nvarchar](MAX) NULL,
	[franamt] [nvarchar](MAX) NULL,
	[downamt] [nvarchar](MAX) NULL,
	[employer] [nvarchar](MAX) NULL,
	[position] [nvarchar](MAX) NULL,
	[schtraindte] [nvarchar](MAX) NULL,

	
	[jkfullpart] [nvarchar](MAX) NULL,
	[cash2invst] [nvarchar](MAX) NULL,
	[state] [nvarchar](MAX) NULL,
	[f_id] [nvarchar](MAX) NULL,
	[corpgen] [nvarchar](MAX) NULL,
	[mi] [nvarchar](MAX) NULL,
	[mailoutinc] [nvarchar](MAX) NULL,
	[fphone] [nvarchar](MAX) NULL,
	[heardfrom] [nvarchar](MAX) NULL,
	[lastmailda] [nvarchar](MAX) NULL,
	
	[trandate] [nvarchar](MAX) NULL,
	[investrang] [nvarchar](MAX) NULL,
	[mastrequest] [nvarchar](MAX) NULL,
	[leadnotes] [nvarchar](MAX) NULL
 CONSTRAINT [PK_CRM_FranchiseTempDataId] PRIMARY KEY CLUSTERED 
(
	[CRM_FranchiseTempDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create [CRM_CloseType] table
 * Created By: Wahib Naseem 
 */
GO
CREATE TABLE [CRM_CloseType] (
  [CRM_CloseTypeId][int] IDENTITY(1,1) NOT NULL,
  [Name] [NVARCHAR](100) NOT NULL,
  [CRMFollowUp] [bit] NULL,
  [CRMFranchiseFollowUp] [bit] NULL,  
  [CreatedBy]				[int]		NULL,
  [CreatedDate]				[datetime]NULL,
  [ModifiedBy]				[int]		NULL,
  [ModifiedDate]			[datetime]	NULL,
  CONSTRAINT [PK_CRM_CloseType] PRIMARY KEY CLUSTERED 
(
	[CRM_CloseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create [CRM_ReasonType] table
 * Created By: Wahib Naseem 
 */
GO
CREATE TABLE [CRM_ReasonType] (
  [CRM_ReasonTypeId][int] IDENTITY(1,1) NOT NULL,
  [Name] [NVARCHAR](100) NOT NULL,   
  [CreatedBy]				[int]		NULL,
  [CreatedDate]				[datetime]NULL,
  [ModifiedBy]				[int]		NULL,
  [ModifiedDate]			[datetime]	NULL,
  CONSTRAINT [PK_CRM_ReasonType] PRIMARY KEY CLUSTERED 
(
	[CRM_ReasonTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]






/**
 * Module: CRM
 * Table: Create CRM_TimeLine table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_TimeLine] (
  [CRM_TimeLineId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountId] [int]  NOT NULL,
  [TimeLineType] [int] NULL,
  [Title] [nvarchar](50) NULL,
  [Description] [nvarchar](500) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_TimeLine] PRIMARY KEY CLUSTERED 
(
	[CRM_TimeLineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_AccountCustomerDetail table
 * Created By: Divesh 
 */
 GO
CREATE TABLE [dbo].[CRM_AccountCustomerDetail](
	[CRM_AccountCustomerDetailId] [int] IDENTITY(1,1) NOT NULL,
	[CRM_AccountId] [int] NULL,
	[CompanyName] [nvarchar](50) NULL,
	[CompanyAddressLine1] [nvarchar](200) NULL,
	[CompanyAddressLine2] [nvarchar](200) NULL,
	[CompanyCity] [nvarchar](20) NULL,
	[CompanyCounty] [nvarchar](20) NULL,
	[CompanyState] [nvarchar](20) NULL,
	[CompanyZipCode] [nvarchar](20) NULL,
	[CompanyEmailAddress] [nvarchar](50) NULL,
	[CompanyPhoneNumber] [nvarchar](20) NULL,
	[CompanyFaxNumber] [nvarchar](20) NULL,
	[NumberOfEmployees] [int] NULL,
	[NumberOfLocations] [int] NULL,
	[BudgetAmount] [decimal](18, 0) NULL,
	[IndustryType] [int] NULL,
	[Callback] [datetime] NULL,
	[AMorPM] [bit] NULL,
	[GeneralNote] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CRM_AccountCustomerDetail] PRIMARY KEY CLUSTERED 
(
	[CRM_AccountCustomerDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Note table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Note] (
  [CRM_NoteId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [Title] [nvarchar](150) NULL,
  [Description] [nvarchar](500) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Note] PRIMARY KEY CLUSTERED 
(
	[CRM_NoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Activity table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Activity] (
  [CRM_ActivityId][int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [ActivityType] [int] NULL,
  [OutComeType] [int] NULL,
  [Note] [nvarchar](500) NULL,
  [TimeStamp] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [CreatedDate] [datetime] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Activity] PRIMARY KEY CLUSTERED 
(
	[CRM_ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

	/**
 * Module: CRM
 * Table: Create CRM_NoteType table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_NoteType (
  [CRM_NoteTypeId] [int] IDENTITY(1,1) NOT NULL,  
  [Type] [int] NULL,
  [Name] [nvarchar](30) NULL,  
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_NoteTypeId] PRIMARY KEY CLUSTERED 
(
	[CRM_NoteTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

	/**
 * Module: CRM
 * Table: Create CRM_SalePossibilityType table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_SalePossibilityType (
  [CRM_SalePossibilityTypeId] [int] IDENTITY(1,1) NOT NULL,  
  [Type] [int] NULL,
  [Name] [nvarchar](30) NULL,  
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_SalePossibilityTypeId] PRIMARY KEY CLUSTERED 
(
	[CRM_SalePossibilityTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



	/**
 * Module: CRM
 * Table: Create CRM_FranchiseFollowUp table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_FranchiseFollowUp (
  [CRM_FranchiseFollowUpId] [int] IDENTITY(1,1) NOT NULL,  
  [DiscloseAdditional] [bit] NULL,
  [StatusCreationConfirmed] [BIT] NULL,  
  [NotifyNextTraining] [BIT] NULL,
  [KeepActive] [bit] NULL, 
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_FranchiseFollowUpId] PRIMARY KEY CLUSTERED 
(
	[CRM_FranchiseFollowUpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




/**
 * Module: CRM
 * Table: Create [CRM_FranchiseContract] table
 * Created By: Wahib Naseem 
 */
GO
CREATE TABLE [CRM_FranchiseContract] (
  [CRM_FranchiseContractId][int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NOT NULL,
  [PresentFranchiseDisclosure] [bit] NULL,
  [CompletedQuestionnaire] [bit] NULL,
  [SignAgreement] [bit] NULL,
  [CompanyCreated] [int] NULL,
  [AllPrincipleClosed] [bit] NULL, 
  [CompleteFranchiseApplication] [bit] NULL,
  [RegionId][bit] NULL,
  [CreatedBy] [int] NULL,
  [CreatedDate] [datetime] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_FranchiseContract] PRIMARY KEY CLUSTERED 
(
	[CRM_FranchiseContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create [CRM_PurposeType] table
 * Created By: Wahib Naseem 
 */
GO
CREATE TABLE [CRM_PurposeType] (
  [CRM_PurposeTypeId][int] IDENTITY(1,1) NOT NULL,
  [Name] [NVARCHAR](100) NOT NULL,
  [CRMStageStatusId]				[int]	NULL, 
  [CreatedBy]				[int]		NULL,
  [CreatedDate]				[datetime]NULL,
  [ModifiedBy]				[int]		NULL,
  [ModifiedDate]			[datetime]	NULL,
  CONSTRAINT [PK_CRM_PurposeType] PRIMARY KEY CLUSTERED 
(
	[CRM_PurposeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create [CRM_SignAgreement] table
 * Created By: Wahib Naseem 
 */
GO
CREATE TABLE [CRM_SignAgreement] (
  [CRM_SignAgreementId][int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NOT NULL,
  [DateSign]				[datetime]	NULL,
  [Term]					[decimal]	NULL,
  [ExpDate]					[datetime]	NULL,
  [PlanType]				[int]		NULL,
  [PlanAmount]				[decimal]	NULL, 
  [IBAmount]				[decimal]	NULL,
  [DownPayment]				[decimal]	NULL,
  [Interest]				[decimal]	NULL,
  [PaymentAmount]			[decimal]	NULL,
  [NoOfPayments]			[decimal]	NULL,
  [CurrentPayment]			[decimal]	NULL,
  [PaymentStartDate]		[datetime]	NULL, 
  [TriggerAmount]			[decimal]	NULL,
  [legalOblStart]		    [datetime]	NULL,   
  [LegalOblEnd]				[datetime]	NULL,
  [LegalOblDue]				[nvarchar] (25)	NULL,
  [Note]                    [NVARCHAR] (250) NULL,
  [CreatedBy]				[int]		NULL,
  [CreatedDate]				[datetime]NULL,
  [ModifiedBy]				[int]		NULL,
  [ModifiedDate]			[datetime]	NULL,
  CONSTRAINT [PK_CRM_SignAgreement] PRIMARY KEY CLUSTERED 
(
	[CRM_SignAgreementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




/**
 * Module: CRM
 * Table: Create CRM_Schedule table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Schedule] (
  [CRM_ScheduleId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [Title] [nvarchar](150) NULL,
  [Description] [nvarchar](500) NULL,
  [StartDate] [datetime] NULL,
  [Duration] [decimal] NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Schedule] PRIMARY KEY CLUSTERED 
(
	[CRM_ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Task table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Task] (
  [CRM_TaskId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [Title] [nvarchar](150) NULL,
  [Description] [nvarchar](500) NULL,
  [DueDateTime] [datetime] NULL,
  [TaskType] [int] NULL,
  [Assignee ] [int] NULL,
  [EmailReminder] [datetime] NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_Task] PRIMARY KEY CLUSTERED 
(
	[CRM_TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Quotation table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Quotation] (
  [CRM_QuotationId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [Name] [nvarchar](50) NULL,
  [Amount] [decimal] NULL,
  [CloseDate] [datetime] NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Quotation] PRIMARY KEY CLUSTERED 
(
	[CRM_QuotationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_ProviderType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_ProviderType] (
  [CRM_ProviderTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_ProviderType] PRIMARY KEY CLUSTERED 
(
	[CRM_ProviderTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



/**
 * Module: CRM
 * Table: Create CRM_ScheduleType table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_ScheduleType] (
  [CRM_ScheduleTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_ScheduleType] PRIMARY KEY CLUSTERED 
(
	[CRM_ScheduleTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_AccountType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_AccountType] (
  [CRM_AccountTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_AccountType] PRIMARY KEY CLUSTERED 
(
	[CRM_AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_ActivityType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_ActivityType] (
  [CRM_ActivityTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_ActivityType] PRIMARY KEY CLUSTERED 
(
	[CRM_ActivityTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_ActivityOutcomeType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_ActivityOutcomeType] (
  [CRM_ActivityOutcomeTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_ActivityOutcomeType] PRIMARY KEY CLUSTERED 
(
	[CRM_ActivityOutcomeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_IndustryType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_IndustryType] (
  [CRM_IndustryTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_IndustryType] PRIMARY KEY CLUSTERED 
(
	[CRM_IndustryTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_TimeLineType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_TimeLineType] (
  [CRM_TimeLineTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_TimeLineType] PRIMARY KEY CLUSTERED 
(
	[CRM_TimeLineTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create CRM_Document table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_Document] (
  [CRM_DocumentId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NULL,
  [File_Name] [nvarchar](50) NULL,
  [Description] [nvarchar](500) NULL,
  [Document_FilePath] [nvarchar](1000) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Document] PRIMARY KEY CLUSTERED 
(
	[CRM_DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_InitialCommunication table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_InitialCommunication] (
  [CRM_InitialCommunicationId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NULL,
  [ContactPerson] [nvarchar](30) NULL,
  [InterestedInPerposal] [bit] NULL,
  [AvailableToMeet] [datetime] NULL,
  [Note] [nvarchar](100) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_InitialCommunication] PRIMARY KEY CLUSTERED 
(
	[CRM_InitialCommunicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: CRM
 * Table: Create CRM_Bidding table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_Bidding] (
  [CRM_BiddingId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NULL,
  [AnalysisWorkBook] [bit] NULL,
  [MonthlyPrice] [bit] NULL,
  [PriceApproved] [int] NULL,
  [IfBidOver] [bit] NULL,
  [IncludePrice] [decimal] NULL,
  [Note][nvarchar](1000) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_Bidding] PRIMARY KEY CLUSTERED 
(
	[CRM_BiddingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



/**
 * Module: CRM
 * Table: Create CRM_FvPresentation table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_FvPresentation (
  [CRM_CRM_FvPresentationId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_AccountCustomerDetailId] [int] NOT NULL,
  [CRM_AccountFranchiseDetailId] [int] NULL,
  [MeasureContactPerson] [nvarchar](30) NULL,
  [MeasureFacility] [float] NULL,
  [NumberOfFloors] [int] NULL,
  [Frequency] [int] NULL,
  [CleaningDay] [int] NULL,
  [ServiceLevel][int] NULL,
  [BudgetAmount][decimal] NULL,
  [Note][nvarchar] (1000) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_FvPresentation] PRIMARY KEY CLUSTERED 
(
	[CRM_CRM_FvPresentationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_LeadGeneration table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_LeadGeneration (
  [CRM_LeadGenerationId] [int] IDENTITY(1,1) NOT NULL,  
  [CRM_AccountFranchiseDetailId] [int] NULL,
  [Name] [nvarchar](30) NULL,
  [Address] [nvarchar](50) NULL,
  [City] [nvarchar](25) NULL,
  [State] [nvarchar](25) NULL,
  [ZipCode] [nvarchar](25) NULL,
  [ContactPerson][nvarchar](25) NULL,
  [PhoneNumber][nvarchar](25) NULL,
  [Note][nvarchar] (1000) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_LeadGeneration] PRIMARY KEY CLUSTERED 
(
	[CRM_LeadGenerationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_CallResult table
 * Created By: Wahib 
 */
GO
CREATE TABLE CRM_CallResult (
  [CRM_CallResultId] [int] IDENTITY(1,1) NOT NULL,  
  [Type] [int] NULL,
  [Name] [nvarchar](30) NULL,  
  [CreatedDate] [datetime] NULL,
  [CreatedBy][int] NULL,
  [ModifiedBy][int] NULL,
  [ModifiedDate] [datetime] NULL,
  CONSTRAINT [PK_CRM_CallResultId] PRIMARY KEY CLUSTERED 
(
	[CRM_CallResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



/**
 * Module: CRM
 * Table: Create CRM_TaskType table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_TaskType] (
  [CRM_TaskTypeId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_TaskType] PRIMARY KEY CLUSTERED 
(
	[CRM_TaskTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Stage table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_Stage] (
  [CRM_StageId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_Stage] PRIMARY KEY CLUSTERED 
(
	[CRM_StageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_StageStatus table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_StageStatus] (
  [CRM_StageStatusId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_CRM_StageStatus] PRIMARY KEY CLUSTERED 
(
	[CRM_StageStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Territory table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_Territory] (
  [CRM_TerritoryId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [Description] [nvarchar](250) NULL,
  [RegionId] [int] NULL,
  [IsActive] [bit] NULL,  
   CONSTRAINT [PK_CRM_Territory] PRIMARY KEY CLUSTERED 
(
	[CRM_TerritoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_Territory_Assignment table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_Territory_Assignment] (
  [CRM_TerriAssignmentId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_TerritoryId] [int] NULL,
  [ZipCode] [nvarchar](25) NULL,    
   CONSTRAINT [PK_CRM_Territory_Assignment] PRIMARY KEY CLUSTERED 
(
	[CRM_TerriAssignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_SalesTerritory_Assignment table
 * Created By: Wahib 
 */
GO
CREATE TABLE [CRM_SalesTerritory_Assignment] (
  [CRM_SalesTerriAssignmentId] [int] IDENTITY(1,1) NOT NULL,
  [CRM_TerritoryId] [int] NULL,
  [UserId] [int] NULL,    
   CONSTRAINT [PK_CRM_SalesTerritory_Assignment] PRIMARY KEY CLUSTERED 
(
	[CRM_SalesTerriAssignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_ProviderSource table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_ProviderSource] (
  [CRM_ProviderSourceId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] datetime,
   CONSTRAINT [PK_CRM_ProviderSource] PRIMARY KEY CLUSTERED 
(
	[CRM_ProviderSourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_AccountFranchiseDetail
 * Created By: Divesh 
 */
GO
CREATE TABLE [dbo].[CRM_AccountFranchiseDetail](
	[CRM_AccountCustomerDetailId] [int] IDENTITY(1,1) NOT NULL,
	[CRM_AccountId] [int] NULL,
	[StreetLine1] [nvarchar](200) NULL,
	[StreetLine2] [nvarchar](200) NULL,
	[City] [nvarchar](20) NULL,
	[County] [nvarchar](20) NULL,
	[State] [nvarchar](20) NULL,
	[ZipCode] [nvarchar](20) NULL,
	[EmailAddress] [nvarchar](50) NULL,
	[CellNumber] [nvarchar](20) NULL,
	[FaxNumber] [nvarchar](20) NULL,
	[HomeNumber] [nvarchar](20) NULL,
	[WorkNumber] [nvarchar](20) NULL,
	[Employer] [nvarchar](50) NULL,
	[Position] [nvarchar](50) NULL,
	[LeadSource] [int] NULL,
	[JkFull] [int] NULL,
	[AmtToInvest] [decimal](18, 0) NULL,
	[InfoSentDate] [datetime] NULL,
	[DisclosedDate] [datetime] NULL,
	[5DayPaperDate] [datetime] NULL,
	[EstCloseDate] [datetime] NULL,
	[SoldDate] [datetime] NULL,
	[FranPlan]  [nvarchar](50) NULL,
	[SoldBy] [nvarchar](50) NULL,
	[FranAmount] [decimal](18, 0) NULL,
	[DownAmount] [decimal](18, 0) NULL,
	[CallBack] [datetime] NULL,
	[Representative] [nvarchar](50) NULL,
	[Notes] [nvarchar](200) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_CRM_AccountFranchiseDetail] PRIMARY KEY CLUSTERED 
(
	[CRM_AccountCustomerDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/**
 * Module: CRM
 * Table: Create CRM_ProviderSource table
 * Created By: Divesh 
 */
GO
CREATE TABLE [CRM_LeadSource] (
  [CRM_LeadSourceId] [int] IDENTITY(1,1) NOT NULL,
  [Type] [int] NULL,
  [Name] [nvarchar](50) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [int] NULL,
  [ModifiedBy] [int] NULL,
  [ModifiedDate] datetime,
   CONSTRAINT [PK_CRM_LeadSource] PRIMARY KEY CLUSTERED 
(
	[CRM_LeadSourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
/**
 * Module: CRM
 * FK_CRM_Note_CRM_AccountCustomerDetail: CRM_AccountCustomerDetail -> CRM_Note
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_Note]  WITH CHECK ADD  CONSTRAINT [FK_CRM_Note_CRM_AccountCustomerDetail] FOREIGN KEY([CRM_AccountCustomerDetailId])
REFERENCES [dbo].[CRM_AccountCustomerDetail] ([CRM_AccountCustomerDetailId])
GO
ALTER TABLE [dbo].[CRM_Note] CHECK CONSTRAINT [FK_CRM_Note_CRM_AccountCustomerDetail]


/**
 * Module: CRM
 * FK_CRM_Activity_CRM_AccountCustomerDetail: CRM_AccountCustomerDetail -> CRM_Activity
 * Created By: Divesh 
 */
 GO
ALTER TABLE [dbo].[CRM_Activity]  WITH CHECK ADD  CONSTRAINT [FK_CRM_Activity_CRM_AccountCustomerDetail] FOREIGN KEY([CRM_AccountCustomerDetailId])
REFERENCES [dbo].[CRM_AccountCustomerDetail] ([CRM_AccountCustomerDetailId])
GO
ALTER TABLE [dbo].[CRM_Activity] CHECK CONSTRAINT [FK_CRM_Activity_CRM_AccountCustomerDetail]

/**
 * Module: CRM
 * FK_CRM_Schedule_CRM_AccountCustomerDetail: CRM_AccountCustomerDetail -> CRM_Schedule
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_Schedule]  WITH CHECK ADD  CONSTRAINT [FK_CRM_Schedule_CRM_AccountCustomerDetail] FOREIGN KEY([CRM_AccountCustomerDetailId])
REFERENCES [dbo].[CRM_AccountCustomerDetail] ([CRM_AccountCustomerDetailId])
GO
ALTER TABLE [dbo].[CRM_Schedule] CHECK CONSTRAINT [FK_CRM_Schedule_CRM_AccountCustomerDetail]

/**
 * Module: CRM
 * FK_CRM_Task_CRM_AccountCustomerDetail: CRM_AccountCustomerDetail -> CRM_Task
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_Task]  WITH CHECK ADD  CONSTRAINT [FK_CRM_Task_CRM_AccountCustomerDetail] FOREIGN KEY([CRM_AccountCustomerDetailId])
REFERENCES [dbo].[CRM_AccountCustomerDetail] ([CRM_AccountCustomerDetailId])
GO
ALTER TABLE [dbo].[CRM_Task] CHECK CONSTRAINT [FK_CRM_Task_CRM_AccountCustomerDetail]

/**
 * Module: CRM
 * FK_CRM_Quotation_CRM_AccountCustomerDetail: CRM_AccountCustomerDetail -> CRM_Quotation
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_Quotation]  WITH CHECK ADD  CONSTRAINT [FK_CRM_Quotation_CRM_AccountCustomerDetail] FOREIGN KEY([CRM_AccountCustomerDetailId])
REFERENCES [dbo].[CRM_AccountCustomerDetail] ([CRM_AccountCustomerDetailId])
GO
ALTER TABLE [dbo].[CRM_Quotation] CHECK CONSTRAINT [FK_CRM_Quotation_CRM_AccountCustomerDetail]

/**
 * Module: CRM
 * FK_CRM_AccountCustomerDetail_CRM_Account : CRM_Account -> CRM_AccountCustomerDetail
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_AccountCustomerDetail]  WITH CHECK ADD  CONSTRAINT [FK_CRM_AccountCustomerDetail_CRM_Account] FOREIGN KEY([CRM_AccountId])
REFERENCES [dbo].[CRM_Account] ([CRM_AccountId])
GO

ALTER TABLE [dbo].[CRM_AccountCustomerDetail] CHECK CONSTRAINT [FK_CRM_AccountCustomerDetail_CRM_Account]

/**
 * Module: CRM
 * FK_CRM_AccountFranchiseDetail_CRM_Account : CRM_Account -> CRM_AccountCustomerDetail
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_AccountFranchiseDetail]  WITH CHECK ADD  CONSTRAINT [FK_CRM_AccountFranchiseDetail_CRM_Account] FOREIGN KEY([CRM_AccountId])
REFERENCES [dbo].[CRM_Account] ([CRM_AccountId])
GO

ALTER TABLE [dbo].[CRM_AccountFranchiseDetail] CHECK CONSTRAINT [FK_CRM_AccountFranchiseDetail_CRM_Account]

/**
 * Module: CRM
 * FK_CRM_TimeLine_CRM_Account : CRM_Account -> CRM_TimeLine
 * Created By: Divesh 
 */
GO
ALTER TABLE [dbo].[CRM_TimeLine]  WITH CHECK ADD  CONSTRAINT [FK_CRM_TimeLine_CRM_Account] FOREIGN KEY([CRM_AccountId])
REFERENCES [dbo].[CRM_Account] ([CRM_AccountId])
GO
ALTER TABLE [dbo].[CRM_TimeLine] CHECK CONSTRAINT [FK_CRM_TimeLine_CRM_Account]

/**
 * Module: CRM
 * Table: Create CRMContactType table
 * Created By: Krishan
 */
GO
CREATE TABLE [dbo].[CRMContactType] (
    [CRMContactTypeId] INT           NOT NULL,
    [Name]             VARCHAR (200) NOT NULL,
    [IsActive]         BIT           CONSTRAINT [DF_CRMContactType_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_CRMContactType_CreatedBy] DEFAULT (getdate()) NOT NULL,
    [Updatedy]         INT           NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_CRMContactType] PRIMARY KEY CLUSTERED ([CRMContactTypeId] ASC)
);

/**
 * Module: CRM
 * Table: Create CRMContact table
 * Created By: Krishan
 */
GO
CREATE TABLE [dbo].[CRMContact] (
    [CRMContactId]     INT           IDENTITY (1, 1) NOT NULL,
    [CRMContactTypeId] INT           NOT NULL,
    [UserId]           INT           NOT NULL,
    [RegionId]         INT           NOT NULL,
    [FullName]         VARCHAR (200) NOT NULL,
    [Company]          VARCHAR (200) NULL,
    [Jobtitle]         VARCHAR (100) NULL,
    [FileAs]           VARCHAR (100) NULL,
    [Email]            VARCHAR (200) NOT NULL,
    [DisplayAs]        VARCHAR (200) NULL,
    [WebPageAddress]   VARCHAR (500) NULL,
    [IMAddress]        VARCHAR (100) NULL,
    [BusinessPhone]    VARCHAR (20)  NOT NULL,
    [HomePhone]        VARCHAR (20)  NULL,
    [BusinessFaxPhone] VARCHAR (20)  NULL,
    [MobilePhone]      VARCHAR (20)  NULL,
    [BusinessAddress]  VARCHAR (500) NULL,
    [IsMailingAddress] BIT           CONSTRAINT [DF_CRMContact_IsMailingAddress] DEFAULT ((0)) NOT NULL,
    [IsActive]         BIT           DEFAULT ((1)) NOT NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedOn]        DATETIME      CONSTRAINT [DF_CRMContact_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]        INT           NULL,
    [UpdatedOn]        DATETIME      NULL,
    CONSTRAINT [PK_CRMContact] PRIMARY KEY CLUSTERED ([CRMContactId] ASC),
    CONSTRAINT [FK_CRMContact_CRMContactType_CRMContactTypeId] FOREIGN KEY ([CRMContactTypeId]) REFERENCES [dbo].[CRMContactType] ([CRMContactTypeId]),
    CONSTRAINT [FK_CRMContact_AuthUserLogin_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AuthUserLogin] ([UserId]),
    CONSTRAINT [FK_CRMContact_Region_RegionId] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId])
);

/**
 * Module: FMS
 * Table: CheckbookTransactionTypeList
 * Created By:  
 */
GO
CREATE TABLE [CheckBookTransactionTypeList] (
[CheckBookTransactionTypeListId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](125) NULL,
	[DisplayType] [varchar](125) NULL,
	[TypeListId] [int] NULL,
	[IsSystemGenerated] [bit] NULL,
	[CheckBookAmountTypeListId] [int] NULL,
	[IsManual] [bit] NULL,
	[Code] [nvarchar](5) NULL,
	[MasterTrxTypeListId] [int] NULL,
	[ServiceTypeListId] [int] NULL,
	[Deposit] [bit] NULL,
	[Payment] [bit] NULL,
	[CheckTemplateId] [int] NULL,
 CONSTRAINT [PK_CheckBookTransactionTypeList] PRIMARY KEY CLUSTERED 
(
	[CheckBookTransactionTypeListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]


/**
 * Module: FMS
 * Table: Region
 * Created By:  
 */

GO
CREATE TABLE [dbo].[Region](
	[RegionId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NULL,
	[Acronym] [varchar](8) NULL,
	[Corporate] [int] NULL,
	[Status] [int] NULL,
	[Test] [int] NULL,
	[Displayname] [varchar](50) NULL,
	[ReportName] [varchar](50) NULL,
	[Address] [varchar](60) NULL,
	[City] [varchar](40) NULL,
	[State] [char](2) NULL,
	[PostalCode] [char](10) NULL,
	[RemitSameAsMain] [int] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Phone] [char](15) NULL,
	[International] [int] NULL,
	[Address2] [varchar](45) NULL,
	[Address3] [varchar](45) NULL,
	[Phone1] [varchar](20) NULL,
	[Phone2] [varchar](20) NULL,
	[AhPhone] [varchar](20) NULL,
	[Fax] [varchar](20) NULL,
	[Director] [varchar](30) NULL,
	[Email] [varchar](30) NULL,
	[Country] [varchar](30) NULL,
	[DBname] [char](5) NULL,
	[LockboxId] [char](10) NULL,
	[Number] [int] NULL,
	[company_no] [nvarchar](max) NULL,
	[TRVDBName] [nvarchar](max) NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/**
 * Module: FMS
 * Table: Company
 * Created By:  
 */
CREATE TABLE [dbo].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](max) NULL,
	[TypeListId] [int] NULL,
	[AddressId] [int] NULL,
	[RegionId] [int] NULL,
	[BankId] [int] NULL,
	[PhoneID] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[Number] [nvarchar](100) NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
