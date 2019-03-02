IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormHistory]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormHistory]      
(
	@InspectionFormHistoryId INT = 0,
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		InspectionFormHistory.InspectionFormHistoryId AS InspectionFormId,
		InspectionFormHistory.JobId,
        InspectionFormHistory.CustomerId,
        InspectionFormHistory.RegionId,
        InspectionFormHistory.FranchiseeId,
        Job.CustomerName,
        Customer.CustomerNo AS CustomerNumber,
        InspectionFormHistory.ServiceTypeListId,
        ServiceTypeList.name AS ServiceType,
        InspectionFormHistory.AccountTypeListId,
        AccountTypeList.Name AS AccountType,
        InspectionFormHistory.InspectionStatusId,
        Job.PrimaryContact,
        Job.PrimaryContactPhone,
        Job.PrimaryContactPhoneExt,
        Job.CleanTimes,
        Job.Mon,
        Job.Tue,
        Job.Wed,
        Job.Thu,
        Job.Fri,
        Job.Sat,
        Job.Sun,
        InspectionFormHistory.CallDate,
        InspectionFormHistory.RecordedDate,
        InspectionFormHistory.UploadedDate,
        InspectionFormHistory.IsCompleted,
        InspectionFormHistory.InspectedBy,
        InspectionFormHistory.InspectorId,
        InspectionFormHistory.FormName,
        InspectionFormHistory.Description,
        InspectionFormHistory.PassPoints,
        InspectionFormHistory.FailPoints,
        InspectionFormHistory.NeedImprovementPoints,
        InspectionFormHistory.ScorePercent,
        InspectionFormHistory.SignatureUrl,
        InspectionFormHistory.IsEnable,
        InspectionFormHistory.IsDelete,
        InspectionFormHistory.CreatedBy,
        InspectionFormHistory.CreatedDate,
        InspectionFormHistory.ModifiedBy,
        InspectionFormHistory.ModifiedDate,
		Address.AddressId,  
        Address.Address1,  
        Address.Address2,  
        Address.City,  
        Address.StateName AS State,    
        Address.PostalCode AS ZipCode,  
        Address.Latitude,  
        Address.Longitude
	FROM
		[dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK)
	LEFT JOIN [dbo].[Job] Job WITH (NOLOCK) ON Job.JobId = InspectionFormHistory.JobId
	LEFT JOIN [dbo].[Customer] Customer WITH (NOLOCK) ON Customer.CustomerId = InspectionFormHistory.CustomerId
	LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON InspectionFormHistory.AccountTypeListId = AccountTypeList.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON InspectionFormHistory.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
	LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = Job.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address
	WHERE
		InspectionFormHistory.InspectionFormHistoryId = @InspectionFormHistoryId AND
		InspectionFormHistory.IsEnable = @IsEnable
		 
END TRY    
    
BEGIN CATCH
	SELECT
		ERROR_NUMBER() AS ErrorNumber,
		ERROR_SEVERITY() AS ErrorSeverity,
		ERROR_STATE() AS ErrorState,
		ERROR_PROCEDURE() AS ErrorProcedure,
		ERROR_LINE() AS ErrorLine,
		ERROR_MESSAGE() AS ErrorMessage;
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION;