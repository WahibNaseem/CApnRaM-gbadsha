IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateCrmCallLog]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateCrmCallLog]
GO  
 
CREATE PROCEDURE [dbo].[sp_AddOrUpdateCrmCallLog]  
(
	@CallLogId INT = 0,
	@AccountId INT = 0,
	@AccountCustomerDetailId INT = 0,
	@LeadSource INT = 0,
	@CallResultId INT = 0,
	@StageStatus INT = 0,
	@NoteTypeId INT = 0,
	@SpokeWith NVARCHAR(50) = NULL,
	@Note NVARCHAR(250) = NULL,
	@CallLogDate DATETIME = NULL,
	@CallBack DATETIME = NULL,
	@CallBackTime DATETIME = NULL,
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
  
	IF @AccountId < 1 SET @AccountId = NULL
	IF @AccountCustomerDetailId < 1 SET @AccountCustomerDetailId = NULL
	IF @LeadSource < 1 SET @LeadSource = NULL
	IF @CallResultId < 1 SET @CallResultId = NULL
	IF @StageStatus < 1 SET @StageStatus = NULL
	IF @NoteTypeId < 1 SET @NoteTypeId = NULL
   
	UPDATE
		[dbo].[CRM_CallLog]  
	SET
		[dbo].[CRM_CallLog].CRM_AccountId = COALESCE(@AccountId, [dbo].[CRM_CallLog].CRM_AccountId),
		[dbo].[CRM_CallLog].CRM_AccountCustomerDetailId = COALESCE(@AccountCustomerDetailId, [dbo].[CRM_CallLog].CRM_AccountCustomerDetailId),
		[dbo].[CRM_CallLog].CRM_LeadSource = COALESCE(@LeadSource, [dbo].[CRM_CallLog].CRM_LeadSource),
		[dbo].[CRM_CallLog].CRM_CallResultId = COALESCE(@CallResultId, [dbo].[CRM_CallLog].CRM_CallResultId),
		[dbo].[CRM_CallLog].StageStatus = COALESCE(@StageStatus, [dbo].[CRM_CallLog].StageStatus),
		[dbo].[CRM_CallLog].CRM_NoteTypeId = COALESCE(@NoteTypeId, [dbo].[CRM_CallLog].CRM_NoteTypeId),
		[dbo].[CRM_CallLog].SpokeWith = COALESCE(@SpokeWith, [dbo].[CRM_CallLog].SpokeWith),
		[dbo].[CRM_CallLog].Note = COALESCE(@Note, [dbo].[CRM_CallLog].Note),
		[dbo].[CRM_CallLog].CallLogDate = COALESCE(@CallLogDate, [dbo].[CRM_CallLog].CallLogDate),
		[dbo].[CRM_CallLog].CallBack = COALESCE(@CallBack, [dbo].[CRM_CallLog].CallBack),
		[dbo].[CRM_CallLog].CallBackTime = COALESCE(@CallBackTime, [dbo].[CRM_CallLog].CallBackTime),
		[dbo].[CRM_CallLog].CreatedBy = COALESCE(@CreatedBy, [dbo].[CRM_CallLog].CreatedBy),  
		[dbo].[CRM_CallLog].CreatedDate = COALESCE(@CreatedDate, [dbo].[CRM_CallLog].CreatedDate),  
		[dbo].[CRM_CallLog].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[CRM_CallLog].ModifiedBy),  
		[dbo].[CRM_CallLog].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[CRM_CallLog].ModifiedDate)  
	WHERE  
		[dbo].[CRM_CallLog].CRM_CallLogId = @CallLogId  
  
	IF @@ROWCOUNT = 0
		BEGIN  
		INSERT INTO [dbo].[CRM_CallLog]
		(
			[dbo].[CRM_CallLog].CRM_AccountId,
			[dbo].[CRM_CallLog].CRM_AccountCustomerDetailId,
			[dbo].[CRM_CallLog].CRM_LeadSource,
			[dbo].[CRM_CallLog].CRM_CallResultId,
			[dbo].[CRM_CallLog].StageStatus,
			[dbo].[CRM_CallLog].CRM_NoteTypeId,
			[dbo].[CRM_CallLog].SpokeWith,
			[dbo].[CRM_CallLog].Note,
			[dbo].[CRM_CallLog].CallLogDate,
			[dbo].[CRM_CallLog].CallBack,
			[dbo].[CRM_CallLog].CallBackTime,
			[dbo].[CRM_CallLog].CreatedBy,  
			[dbo].[CRM_CallLog].CreatedDate,  
			[dbo].[CRM_CallLog].ModifiedBy,  
			[dbo].[CRM_CallLog].ModifiedDate 
		)  
		VALUES
		(   
			@AccountId,
			@AccountCustomerDetailId,
			@LeadSource,
			@CallResultId,
			@StageStatus,
			@NoteTypeId,
			@SpokeWith,
			@Note,
			@CallLogDate,
			@CallBack,
			@CallBackTime,
			@CreatedBy,  
			@CreatedDate,  
			@ModifiedBy,  
			@ModifiedDate
		)
		SELECT @CallLogId = SCOPE_IDENTITY()
		END

	EXEC sp_GetCrmCallLog @CallLogId, 1 

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