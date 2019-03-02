IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateAccountWalkThru]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateAccountWalkThru]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateAccountWalkThru]      
(
	@AccountWalkThruItemId INT = 0,
	@AccountWalkThruType INT = NULL,
	@CustomerId INT = NULL,
	@FranchiseeId INT = NULL,
	@FieldValue BIT = 0,
	@FieldText NVARCHAR(400) = NULL,
	@FileUrl NVARCHAR(400) = NULL,
	@IsEnable BIT = 1,
	@CreatedBy INT = 0,
	@CreatedDate DATETIME = NULL,
	@ModifiedBy INT = 0,
	@ModifiedDate DATETIME = NULL
)    
AS    
    
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION UPSERT 
    
BEGIN TRY

	SET NOCOUNT ON

	IF @AccountWalkThruType < 1 SET @AccountWalkThruType = NULL
	IF @CustomerId < 1 SET @CustomerId = NULL
	IF @FranchiseeId < 1 SET @FranchiseeId = NULL
	IF @AccountWalkThruType < 1 SET @AccountWalkThruType = 1

	UPDATE 
		[dbo].[CSAccountWalkThursFormFieldDetail]
	SET
		[dbo].[CSAccountWalkThursFormFieldDetail].CSAccountWalkThursFormFieldId = COALESCE(@AccountWalkThruType, [dbo].[CSAccountWalkThursFormFieldDetail].CSAccountWalkThursFormFieldId),
		[dbo].[CSAccountWalkThursFormFieldDetail].CustomerId = COALESCE(@CustomerId, [dbo].[CSAccountWalkThursFormFieldDetail].CustomerId),
		[dbo].[CSAccountWalkThursFormFieldDetail].FranchiseeId = COALESCE(@FranchiseeId, [dbo].[CSAccountWalkThursFormFieldDetail].FranchiseeId),
		[dbo].[CSAccountWalkThursFormFieldDetail].FieldValue = COALESCE(@FieldValue, [dbo].[CSAccountWalkThursFormFieldDetail].FieldValue),
		[dbo].[CSAccountWalkThursFormFieldDetail].FieldText = COALESCE(@FieldText, [dbo].[CSAccountWalkThursFormFieldDetail].FieldText),
		[dbo].[CSAccountWalkThursFormFieldDetail].FileUrl = COALESCE(@FileUrl, [dbo].[CSAccountWalkThursFormFieldDetail].FileUrl),
		[dbo].[CSAccountWalkThursFormFieldDetail].IsActive = @IsEnable,
		[dbo].[CSAccountWalkThursFormFieldDetail].CreatedBy = COALESCE(@CreatedBy, [dbo].[CSAccountWalkThursFormFieldDetail].CreatedBy),
		[dbo].[CSAccountWalkThursFormFieldDetail].CreatedDate = COALESCE(@CreatedDate, [dbo].[CSAccountWalkThursFormFieldDetail].CreatedDate),
		[dbo].[CSAccountWalkThursFormFieldDetail].ModifiedBy = @ModifiedBy,
		[dbo].[CSAccountWalkThursFormFieldDetail].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[CSAccountWalkThursFormFieldDetail].ModifiedDate)
	WHERE
		[dbo].[CSAccountWalkThursFormFieldDetail].CSAccountWalkThursFormFieldDetailId = @AccountWalkThruItemId

	IF @@ROWCOUNT = 0
		BEGIN
		INSERT INTO [dbo].[CSAccountWalkThursFormFieldDetail]
		(
			[dbo].[CSAccountWalkThursFormFieldDetail].CSAccountWalkThursFormFieldId,
			[dbo].[CSAccountWalkThursFormFieldDetail].CustomerId,
			[dbo].[CSAccountWalkThursFormFieldDetail].FranchiseeId,
			[dbo].[CSAccountWalkThursFormFieldDetail].FieldValue,
			[dbo].[CSAccountWalkThursFormFieldDetail].FieldText,
			[dbo].[CSAccountWalkThursFormFieldDetail].FileUrl,
			[dbo].[CSAccountWalkThursFormFieldDetail].IsActive,
			[dbo].[CSAccountWalkThursFormFieldDetail].CreatedBy,
			[dbo].[CSAccountWalkThursFormFieldDetail].CreatedDate,
			[dbo].[CSAccountWalkThursFormFieldDetail].ModifiedBy,
			[dbo].[CSAccountWalkThursFormFieldDetail].ModifiedDate
		)
		VALUES
		(
			@AccountWalkThruType,
			@CustomerId,
			@FranchiseeId,
			@FieldValue,
			@FieldText,
			@FileUrl,
			@IsEnable,
			@CreatedBy,
			@CreatedDate,
			@ModifiedBy,
			@ModifiedDate
		) 
		SELECT @AccountWalkThruItemId = SCOPE_IDENTITY()
		END

	EXEC sp_GetAccountWalkThruItemById @AccountWalkThruItemId, 1
		 
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
        ROLLBACK TRANSACTION UPSERT;
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION UPSERT;
GO