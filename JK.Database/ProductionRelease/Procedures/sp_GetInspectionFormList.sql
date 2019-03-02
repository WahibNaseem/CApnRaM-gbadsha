IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormList]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormList]      
(
	@IsEnable BIT = 1 
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;
    
	SELECT
		InspectionForm.InspectionFormId,
		InspectionForm.JobId,
		InspectionForm.CustomerId,
		InspectionForm.RegionId,
		InspectionForm.FranchiseeId,
		Job.CustomerName,
		Customer.CustomerNo AS CustomerNumber,
		InspectionForm.ServiceTypeListId,
		ServiceTypeList.name AS ServiceType,
		InspectionForm.AccountTypeListId,
		AccountTypeList.Name AS AccountType,
		InspectionForm.InspectionStatusId,
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
		InspectionForm.CallDate,
		InspectionForm.RecordedDate,
		InspectionForm.UploadedDate,
		InspectionForm.IsCompleted,
		InspectionForm.InspectedBy,
		InspectionForm.InspectorId,
		InspectionForm.FormName,
		InspectionForm.Description,
		InspectionForm.PassPoints,
		InspectionForm.FailPoints,
		InspectionForm.NeedImprovementPoints,
		InspectionForm.ScorePercent,
		InspectionForm.SignatureUrl,
		InspectionForm.IsEnable,
		InspectionForm.IsDelete,
		InspectionForm.CreatedBy,
		InspectionForm.CreatedDate,
		InspectionForm.ModifiedBy,
		InspectionForm.ModifiedDate,
		Address.AddressId,  
		Address.Address1,  
		Address.Address2,  
		Address.City,  
		Address.StateName AS State,    
		Address.PostalCode AS ZipCode,  
		Address.Latitude,  
		Address.Longitude,
		ISNULL(COUNT(*) Over(), 0) AS TotalRecords
	FROM
		[dbo].[InspectionForm] InspectionForm WITH (NOLOCK) 
	LEFT JOIN [dbo].[Job] Job WITH (NOLOCK) ON Job.JobId = InspectionForm.JobId
	LEFT JOIN [dbo].[Customer] Customer WITH (NOLOCK) ON Customer.CustomerId = InspectionForm.CustomerId
	LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON InspectionForm.AccountTypeListId = AccountTypeList.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON InspectionForm.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
	LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = Job.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address
	WHERE
		InspectionForm.IsEnable = @IsEnable
		 
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