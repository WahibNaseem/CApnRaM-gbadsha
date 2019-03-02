IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionFormSectionHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormSectionHistory]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormSectionHistory]  
(  
	@InspectionFormSectionHistoryId INT = 0,  
	@InspectionFormHistoryId INT = 0,  
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

	IF @InspectionFormHistoryId < 1 SET @InspectionFormHistoryId = NULL  
	IF @SectionOrder < 1 SET @SectionOrder = NULL  

	UPDATE  
		[dbo].[InspectionFormSectionHistory]  
	SET  
		[dbo].[InspectionFormSectionHistory].InspectionFormHistoryId = COALESCE(@InspectionFormHistoryId, [dbo].[InspectionFormSectionHistory].InspectionFormHistoryId),  
		[dbo].[InspectionFormSectionHistory].SectionOrder = COALESCE(@SectionOrder, [dbo].[InspectionFormSectionHistory].SectionOrder),  
		[dbo].[InspectionFormSectionHistory].SectionName = COALESCE(@SectionName, [dbo].[InspectionFormSectionHistory].SectionName),  
		[dbo].[InspectionFormSectionHistory].SectionStatus = COALESCE(@SectionStatus, [dbo].[InspectionFormSectionHistory].SectionStatus),  
		[dbo].[InspectionFormSectionHistory].ScorePercent = COALESCE(@ScorePercent, [dbo].[InspectionFormSectionHistory].ScorePercent),  
		[dbo].[InspectionFormSectionHistory].PassPoints = COALESCE(@PassPoints, [dbo].[InspectionFormSectionHistory].PassPoints),  
		[dbo].[InspectionFormSectionHistory].FailPoints = COALESCE(@FailPoints, [dbo].[InspectionFormSectionHistory].FailPoints),  
		[dbo].[InspectionFormSectionHistory].NeedImprovementPoints = COALESCE(@NeedImprovementPoints, [dbo].[InspectionFormSectionHistory].NeedImprovementPoints),  
		[dbo].[InspectionFormSectionHistory].SectionAutoFail = COALESCE(@SectionAutoFail, [dbo].[InspectionFormSectionHistory].SectionAutoFail),  
		[dbo].[InspectionFormSectionHistory].SectionAutoFailReason = COALESCE(@SectionAutoFailReason, [dbo].[InspectionFormSectionHistory].SectionAutoFailReason),  
		[dbo].[InspectionFormSectionHistory].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionFormSectionHistory].CreatedBy),  
		[dbo].[InspectionFormSectionHistory].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionFormSectionHistory].CreatedDate),  
		[dbo].[InspectionFormSectionHistory].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionFormSectionHistory].ModifiedBy),  
		[dbo].[InspectionFormSectionHistory].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionFormSectionHistory].ModifiedDate)  
	WHERE  
		[dbo].[InspectionFormSectionHistory].InspectionFormSectionHistoryId = @InspectionFormSectionHistoryId  

	IF @@ROWCOUNT = 0  
	BEGIN  
		INSERT INTO [dbo].[InspectionFormSectionHistory]  
		(
			[dbo].[InspectionFormSectionHistory].InspectionFormHistoryId,
			[dbo].[InspectionFormSectionHistory].SectionOrder,
			[dbo].[InspectionFormSectionHistory].SectionName,
			[dbo].[InspectionFormSectionHistory].SectionStatus,
			[dbo].[InspectionFormSectionHistory].ScorePercent,
			[dbo].[InspectionFormSectionHistory].PassPoints,
			[dbo].[InspectionFormSectionHistory].FailPoints,
			[dbo].[InspectionFormSectionHistory].NeedImprovementPoints,
			[dbo].[InspectionFormSectionHistory].SectionAutoFail,
			[dbo].[InspectionFormSectionHistory].SectionAutoFailReason,
			[dbo].[InspectionFormSectionHistory].IsEnable,
			[dbo].[InspectionFormSectionHistory].IsDelete,
			[dbo].[InspectionFormSectionHistory].CreatedBy,
			[dbo].[InspectionFormSectionHistory].CreatedDate,
			[dbo].[InspectionFormSectionHistory].ModifiedBy,
			[dbo].[InspectionFormSectionHistory].ModifiedDate  
		  )  
		VALUES  
		(
			@InspectionFormHistoryId,
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
		SELECT @InspectionFormSectionHistoryId = SCOPE_IDENTITY()
	END  

	EXEC sp_GetInspectionFormSectionHistory @InspectionFormSectionHistoryId, 1  

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