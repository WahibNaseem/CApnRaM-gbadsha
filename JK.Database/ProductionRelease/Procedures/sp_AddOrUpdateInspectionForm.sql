IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionForm]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionForm]
GO  
 
CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionForm]  
(
	@InspectionFormId INT = 0,
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
	IF @InspectorId < 1 SET @InspectorId = NULL
   
	UPDATE
		[dbo].[InspectionForm]  
	SET
		[dbo].[InspectionForm].JobId = COALESCE(@JobId, [dbo].[InspectionForm].JobId),
		[dbo].[InspectionForm].CustomerId = COALESCE(@CustomerId, [dbo].[InspectionForm].CustomerId),
		[dbo].[InspectionForm].RegionId = COALESCE(@RegionId, [dbo].[InspectionForm].RegionId),
		[dbo].[InspectionForm].FranchiseeId = COALESCE(@FranchiseeId, [dbo].[InspectionForm].FranchiseeId),
		[dbo].[InspectionForm].ServiceTypeListId = COALESCE(@ServiceTypeListId, [dbo].[InspectionForm].ServiceTypeListId),
		[dbo].[InspectionForm].AccountTypeListId = COALESCE(@AccountTypeListId, [dbo].[InspectionForm].AccountTypeListId),
		[dbo].[InspectionForm].InspectionStatusId = COALESCE(@InspectionStatusId, [dbo].[InspectionForm].InspectionStatusId),
		[dbo].[InspectionForm].CallDate = COALESCE(@CallDate, [dbo].[InspectionForm].CallDate),
		[dbo].[InspectionForm].RecordedDate = @RecordedDate,
		[dbo].[InspectionForm].UploadedDate = @UploadedDate,
		[dbo].[InspectionForm].IsCompleted = COALESCE(@IsCompleted, [dbo].[InspectionForm].IsCompleted),
		[dbo].[InspectionForm].InspectedBy = @InspectedBy,
		[dbo].[InspectionForm].InspectorId = COALESCE(@InspectorId, [dbo].[InspectionForm].InspectorId),
		[dbo].[InspectionForm].FormName = COALESCE(@FormName, [dbo].[InspectionForm].FormName),  
		[dbo].[InspectionForm].Description = COALESCE(@Description, [dbo].[InspectionForm].Description),  
		[dbo].[InspectionForm].ScorePercent = COALESCE(@ScorePercent, [dbo].[InspectionForm].ScorePercent),  
		[dbo].[InspectionForm].PassPoints = COALESCE(@PassPoints, [dbo].[InspectionForm].PassPoints),  
		[dbo].[InspectionForm].FailPoints = COALESCE(@FailPoints, [dbo].[InspectionForm].FailPoints),  
		[dbo].[InspectionForm].NeedImprovementPoints = COALESCE(@NeedImprovementPoints, [dbo].[InspectionForm].NeedImprovementPoints),
		[dbo].[InspectionForm].SignatureUrl = @SignatureUrl,
		[dbo].[InspectionForm].IsEnable = COALESCE(@IsEnable, [dbo].[InspectionForm].IsEnable),  
		[dbo].[InspectionForm].IsDelete = COALESCE(@IsDelete, [dbo].[InspectionForm].IsDelete), 
		[dbo].[InspectionForm].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionForm].CreatedBy),  
		[dbo].[InspectionForm].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionForm].CreatedDate),  
		[dbo].[InspectionForm].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionForm].ModifiedBy),  
		[dbo].[InspectionForm].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionForm].ModifiedDate)  
	WHERE  
		[dbo].[InspectionForm].InspectionFormId = @InspectionFormId  
  
	IF @@ROWCOUNT = 0
		BEGIN  
		INSERT INTO [dbo].[InspectionForm]
		(
			[dbo].[InspectionForm].JobId,
			[dbo].[InspectionForm].CustomerId,
			[dbo].[InspectionForm].RegionId,
			[dbo].[InspectionForm].FranchiseeId,
			[dbo].[InspectionForm].ServiceTypeListId,  
			[dbo].[InspectionForm].AccountTypeListId,
			[dbo].[InspectionForm].InspectionStatusId,
			[dbo].[InspectionForm].CallDate,
			[dbo].[InspectionForm].RecordedDate,
			[dbo].[InspectionForm].UploadedDate,
			[dbo].[InspectionForm].IsCompleted,
			[dbo].[InspectionForm].InspectedBy,
			[dbo].[InspectionForm].InspectorId,
			[dbo].[InspectionForm].FormName,  
			[dbo].[InspectionForm].Description,  
			[dbo].[InspectionForm].ScorePercent,  
			[dbo].[InspectionForm].PassPoints,
			[dbo].[InspectionForm].FailPoints,  
			[dbo].[InspectionForm].NeedImprovementPoints,  
			[dbo].[InspectionForm].SignatureUrl,  
			[dbo].[InspectionForm].IsEnable,  
			[dbo].[InspectionForm].IsDelete, 
			[dbo].[InspectionForm].CreatedBy,  
			[dbo].[InspectionForm].CreatedDate,  
			[dbo].[InspectionForm].ModifiedBy,  
			[dbo].[InspectionForm].ModifiedDate
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
		SELECT @InspectionFormId = SCOPE_IDENTITY()
		END

	EXEC sp_GetInspectionForm @InspectionFormId, 1 

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