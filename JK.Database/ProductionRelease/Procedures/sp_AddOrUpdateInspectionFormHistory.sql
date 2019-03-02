IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionFormHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormHistory]
GO  
 
CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormHistory]  
(
	@InspectionFormHistoryId INT = 0,
	@JobId INT = 0,
	@CustomerId INT = 0,
	@RegionId INT = 0,
	@FranchiseeId INT = 0,
	@ServiceTypeListId INT = 0,
	@AccountTypeListId INT = 0,
	@InspectionStatusId INT = 0,
	@CallDate DATETIME = NULL,
	@RecordedDate DATETIME = NULL,
	@UploadedDate DATETIME = NULL,
	@IsCompleted BIT = 0,
	@InspectedBy NVARCHAR(128) = NULL,
	@InspectorId INT = 0,
	@FormName NVARCHAR(1024) = NULL,
	@Description NVARCHAR(MAX) = NULL, 
	@PassPoints DECIMAL(18,2) = NULL,  
	@FailPoints DECIMAL(18,2) = NULL,  
	@NeedImprovementPoints DECIMAL(18,2) = NULL,
	@ScorePercent DECIMAL(18,2) = NULL,
	@SignatureUrl NVARCHAR(1024) = NULL,
	@IsEnable BIT = 1,
	@IsDelete BIT = 0, 
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
  
	IF @JobId < 1 SET @JobId = NULL
	IF @CustomerId < 1 SET @CustomerId = NULL
	IF @RegionId < 1 SET @RegionId = NULL
	IF @FranchiseeId < 1 SET @FranchiseeId = NULL
	IF @ServiceTypeListId < 1 SET @ServiceTypeListId = NULL
	IF @AccountTypeListId < 1 SET @AccountTypeListId = NULL
	IF @InspectionStatusId < 1 SET @InspectionStatusId = NULL
	IF @PassPoints < 1 SET @PassPoints = NULL
	IF @FailPoints < 1 SET @FailPoints = NULL
	IF @NeedImprovementPoints < 1 SET @NeedImprovementPoints = NULL
	IF @ScorePercent < 1 SET @ScorePercent = NULL
	IF @InspectorId < 1 SET @InspectorId = NULL
   
	UPDATE
		[dbo].[InspectionFormHistory]  
	SET
		[dbo].[InspectionFormHistory].JobId = COALESCE(@JobId, [dbo].[InspectionFormHistory].JobId),
		[dbo].[InspectionFormHistory].CustomerId = COALESCE(@CustomerId, [dbo].[InspectionFormHistory].CustomerId),
		[dbo].[InspectionFormHistory].RegionId = COALESCE(@RegionId, [dbo].[InspectionFormHistory].RegionId),
		[dbo].[InspectionFormHistory].FranchiseeId = COALESCE(@FranchiseeId, [dbo].[InspectionFormHistory].FranchiseeId),
		[dbo].[InspectionFormHistory].ServiceTypeListId = COALESCE(@ServiceTypeListId, [dbo].[InspectionFormHistory].ServiceTypeListId),
		[dbo].[InspectionFormHistory].AccountTypeListId = COALESCE(@AccountTypeListId, [dbo].[InspectionFormHistory].AccountTypeListId),
		[dbo].[InspectionFormHistory].InspectionStatusId = COALESCE(@InspectionStatusId, [dbo].[InspectionFormHistory].InspectionStatusId),
		[dbo].[InspectionFormHistory].CallDate = COALESCE(@CallDate, [dbo].[InspectionFormHistory].CallDate),
		[dbo].[InspectionFormHistory].RecordedDate = COALESCE(@RecordedDate, [dbo].[InspectionFormHistory].RecordedDate),
		[dbo].[InspectionFormHistory].UploadedDate = COALESCE(@UploadedDate, [dbo].[InspectionFormHistory].UploadedDate),
		[dbo].[InspectionFormHistory].IsCompleted = COALESCE(@IsCompleted, [dbo].[InspectionFormHistory].IsCompleted),
		[dbo].[InspectionFormHistory].InspectedBy = COALESCE(@InspectedBy, [dbo].[InspectionFormHistory].InspectedBy),
		[dbo].[InspectionFormHistory].InspectorId = COALESCE(@InspectorId, [dbo].[InspectionFormHistory].InspectorId),
		[dbo].[InspectionFormHistory].FormName = COALESCE(@FormName, [dbo].[InspectionFormHistory].FormName),  
		[dbo].[InspectionFormHistory].Description = COALESCE(@Description, [dbo].[InspectionFormHistory].Description),  
		[dbo].[InspectionFormHistory].ScorePercent = COALESCE(@ScorePercent, [dbo].[InspectionFormHistory].ScorePercent),  
		[dbo].[InspectionFormHistory].PassPoints = COALESCE(@PassPoints, [dbo].[InspectionFormHistory].PassPoints),  
		[dbo].[InspectionFormHistory].FailPoints = COALESCE(@FailPoints, [dbo].[InspectionFormHistory].FailPoints),  
		[dbo].[InspectionFormHistory].NeedImprovementPoints = COALESCE(@NeedImprovementPoints, [dbo].[InspectionFormHistory].NeedImprovementPoints),
		[dbo].[InspectionFormHistory].SignatureUrl = COALESCE(@SignatureUrl, [dbo].[InspectionFormHistory].SignatureUrl),
		[dbo].[InspectionFormHistory].IsEnable = COALESCE(@IsEnable, [dbo].[InspectionFormHistory].IsEnable),  
		[dbo].[InspectionFormHistory].IsDelete = COALESCE(@IsDelete, [dbo].[InspectionFormHistory].IsDelete), 
		[dbo].[InspectionFormHistory].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionFormHistory].CreatedBy),  
		[dbo].[InspectionFormHistory].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionFormHistory].CreatedDate),  
		[dbo].[InspectionFormHistory].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionFormHistory].ModifiedBy),  
		[dbo].[InspectionFormHistory].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionFormHistory].ModifiedDate)  
	WHERE  
		[dbo].[InspectionFormHistory].InspectionFormHistoryId = @InspectionFormHistoryId  
  
	IF @@ROWCOUNT = 0
		BEGIN  
		INSERT INTO [dbo].[InspectionFormHistory]
		(
			[dbo].[InspectionFormHistory].JobId,
			[dbo].[InspectionFormHistory].CustomerId,
			[dbo].[InspectionFormHistory].RegionId,
			[dbo].[InspectionFormHistory].FranchiseeId,
			[dbo].[InspectionFormHistory].ServiceTypeListId,  
			[dbo].[InspectionFormHistory].AccountTypeListId,
			[dbo].[InspectionFormHistory].InspectionStatusId,
			[dbo].[InspectionFormHistory].CallDate,
			[dbo].[InspectionFormHistory].RecordedDate,
			[dbo].[InspectionFormHistory].UploadedDate,
			[dbo].[InspectionFormHistory].IsCompleted,
			[dbo].[InspectionFormHistory].InspectedBy,
			[dbo].[InspectionFormHistory].InspectorId,
			[dbo].[InspectionFormHistory].FormName,  
			[dbo].[InspectionFormHistory].Description,  
			[dbo].[InspectionFormHistory].ScorePercent,  
			[dbo].[InspectionFormHistory].PassPoints,
			[dbo].[InspectionFormHistory].FailPoints,  
			[dbo].[InspectionFormHistory].NeedImprovementPoints,  
			[dbo].[InspectionFormHistory].SignatureUrl,  
			[dbo].[InspectionFormHistory].IsEnable,  
			[dbo].[InspectionFormHistory].IsDelete, 
			[dbo].[InspectionFormHistory].CreatedBy,  
			[dbo].[InspectionFormHistory].CreatedDate,  
			[dbo].[InspectionFormHistory].ModifiedBy,  
			[dbo].[InspectionFormHistory].ModifiedDate
		)  
		VALUES
		(  
			@JobId,
			@CustomerId,
			@RegionId,
			@FranchiseeId,
			@ServiceTypeListId,
			@AccountTypeListId,
			@InspectionStatusId,
			@CallDate,
			@RecordedDate,
			@UploadedDate,
			@IsCompleted,
			@InspectedBy,
			@InspectorId,
			@FormName,
			@Description,
			@ScorePercent,
			@PassPoints,  
			@FailPoints,  
			@NeedImprovementPoints,
			@SignatureUrl,
			@IsEnable,
			@IsDelete, 
			@CreatedBy,  
			@CreatedDate,  
			@ModifiedBy,  
			@ModifiedDate
		)
		SELECT @InspectionFormHistoryId = SCOPE_IDENTITY()
		END

	EXEC sp_GetInspectionFormHistory @InspectionFormHistoryId, 1 

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
GO