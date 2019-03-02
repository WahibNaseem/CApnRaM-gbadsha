IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionFormSection]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormSection]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormSection]  
(  
	@InspectionFormSectionId INT = 0,  
	@InspectionFormId INT = 0,  
	@SectionOrder INT = NULL,  
	@SectionName NVARCHAR(128) = NULL,  
	@SectionStatus INT = 0,  
	@ScorePercent DECIMAL(18,2) = NULL,  
	@PassPoints DECIMAL(18,2) = NULL,  
	@FailPoints DECIMAL(18,2) = NULL,  
	@NeedImprovementPoints DECIMAL(18,2) = NULL,  
	@SectionAutoFail BIT = 0,  
	@SectionAutoFailReason NVARCHAR(MAX) = NULL,  
	@CreatedBy INT = NULL,  
	@CreatedDate DATETIME = NULL,  
	@ModifiedBy INT = NULL,  
	@ModifiedDate DATETIME = NULL  
)  
AS  

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRANSACTION UPSERT  

BEGIN TRY  

	SET NOCOUNT ON;  

	IF @InspectionFormId < 1 SET @InspectionFormId = NULL  
	IF @SectionOrder < 1 SET @SectionOrder = NULL  

	UPDATE  
		[dbo].[InspectionFormSection]  
	SET  
		[dbo].[InspectionFormSection].InspectionFormId = COALESCE(@InspectionFormId, [dbo].[InspectionFormSection].InspectionFormId),  
		[dbo].[InspectionFormSection].SectionOrder = COALESCE(@SectionOrder, [dbo].[InspectionFormSection].SectionOrder),  
		[dbo].[InspectionFormSection].SectionName = COALESCE(@SectionName, [dbo].[InspectionFormSection].SectionName),  
		[dbo].[InspectionFormSection].SectionStatus = COALESCE(@SectionStatus, [dbo].[InspectionFormSection].SectionStatus),  
		[dbo].[InspectionFormSection].ScorePercent = COALESCE(@ScorePercent, [dbo].[InspectionFormSection].ScorePercent),  
		[dbo].[InspectionFormSection].PassPoints = COALESCE(@PassPoints, [dbo].[InspectionFormSection].PassPoints),  
		[dbo].[InspectionFormSection].FailPoints = COALESCE(@FailPoints, [dbo].[InspectionFormSection].FailPoints),  
		[dbo].[InspectionFormSection].NeedImprovementPoints = COALESCE(@NeedImprovementPoints, [dbo].[InspectionFormSection].NeedImprovementPoints),  
		[dbo].[InspectionFormSection].SectionAutoFail = COALESCE(@SectionAutoFail, [dbo].[InspectionFormSection].SectionAutoFail),  
		[dbo].[InspectionFormSection].SectionAutoFailReason = COALESCE(@SectionAutoFailReason, [dbo].[InspectionFormSection].SectionAutoFailReason),  
		[dbo].[InspectionFormSection].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionFormSection].CreatedBy),  
		[dbo].[InspectionFormSection].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionFormSection].CreatedDate),  
		[dbo].[InspectionFormSection].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionFormSection].ModifiedBy),  
		[dbo].[InspectionFormSection].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionFormSection].ModifiedDate)  
	WHERE  
		[dbo].[InspectionFormSection].InspectionFormSectionId = @InspectionFormSectionId  

	IF @@ROWCOUNT = 0  
	BEGIN  
		INSERT INTO [dbo].[InspectionFormSection]  
		(
			[dbo].[InspectionFormSection].InspectionFormId,
			[dbo].[InspectionFormSection].SectionOrder,
			[dbo].[InspectionFormSection].SectionName,
			[dbo].[InspectionFormSection].SectionStatus,
			[dbo].[InspectionFormSection].ScorePercent,
			[dbo].[InspectionFormSection].PassPoints,
			[dbo].[InspectionFormSection].FailPoints,
			[dbo].[InspectionFormSection].NeedImprovementPoints,
			[dbo].[InspectionFormSection].SectionAutoFail,
			[dbo].[InspectionFormSection].SectionAutoFailReason,
			[dbo].[InspectionFormSection].IsEnable,
			[dbo].[InspectionFormSection].IsDelete,
			[dbo].[InspectionFormSection].CreatedBy,
			[dbo].[InspectionFormSection].CreatedDate,
			[dbo].[InspectionFormSection].ModifiedBy,
			[dbo].[InspectionFormSection].ModifiedDate  
		  )  
		VALUES  
		(
			@InspectionFormId,
			@SectionOrder,
			@SectionName,
			@SectionStatus,
			@ScorePercent,
			@PassPoints,
			@FailPoints,
			@NeedImprovementPoints,
			@SectionAutoFail,
			@SectionAutoFailReason,
			1,
			0,
			@CreatedBy,
			@CreatedDate,
			@ModifiedBy,
			@ModifiedDate  
		)
		SELECT @InspectionFormSectionId = SCOPE_IDENTITY()
	END  

	EXEC sp_GetInspectionFormSection @InspectionFormSectionId, 1  

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