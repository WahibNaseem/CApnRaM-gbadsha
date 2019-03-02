IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateFormTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateFormTemplate]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AddOrUpdateFormTemplate]
(
	@FormTemplateId INT = 0,
	@AccountTypeListId INT = NULL,
	@ServiceTypeListId INT = NULL,
	@FormTemplateTypeId INT = NULL,
	@FormName NVARCHAR(500) = NULL,
	@Description NVARCHAR(MAX) = NULL,
	@IsEnable BIT = 1,
	@IsDelete BIT = 0,
	@CreatedBy INT = NULL,
	@CreatedDate DATETIME = NULL,
	@ModifiedBy INT = NULL,
	@ModifiedDate DATETIME = NULL
)
AS

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION

BEGIN TRY

	SET NOCOUNT ON;

	IF @AccountTypeListId < 1 SET @AccountTypeListId = NULL
	IF @ServiceTypeListId < 1 SET @ServiceTypeListId = NULL
	IF @FormTemplateTypeId < 1 SET @FormTemplateTypeId = NULL

	UPDATE
		[dbo].[FormTemplate]
	SET
		[dbo].[FormTemplate].AccountTypeListId = COALESCE(@AccountTypeListId, [dbo].[FormTemplate].AccountTypeListId),
		[dbo].[FormTemplate].ServiceTypeListId = COALESCE(@ServiceTypeListId, [dbo].[FormTemplate].ServiceTypeListId),
		[dbo].[FormTemplate].FormTemplateTypeId = COALESCE(@FormTemplateTypeId, [dbo].[FormTemplate].FormTemplateTypeId),
		[dbo].[FormTemplate].FormName = COALESCE(@FormName, [dbo].[FormTemplate].FormName),
		[dbo].[FormTemplate].Description = COALESCE(@Description, [dbo].[FormTemplate].Description),
		[dbo].[FormTemplate].IsEnable = COALESCE(@IsEnable, [dbo].[FormTemplate].IsEnable),
		[dbo].[FormTemplate].IsDelete = COALESCE(@IsDelete, [dbo].[FormTemplate].IsDelete),
		[dbo].[FormTemplate].ModifiedBy = COALESCE(@ServiceTypeListId, [dbo].[FormTemplate].ModifiedBy),
		[dbo].[FormTemplate].ModifiedDate = COALESCE(@ModifiedDate, GETDATE())
	WHERE
		[dbo].[FormTemplate].FormTemplateId = @FormTemplateId

	IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO [dbo].[FormTemplate]
				(
					[dbo].[FormTemplate].AccountTypeListId,
					[dbo].[FormTemplate].ServiceTypeListId,
					[dbo].[FormTemplate].FormTemplateTypeId,
					[dbo].[FormTemplate].FormName,
					[dbo].[FormTemplate].Description,
					[dbo].[FormTemplate].IsEnable,
					[dbo].[FormTemplate].IsDelete,
					[dbo].[FormTemplate].CreatedBy,
					[dbo].[FormTemplate].CreatedDate,
					[dbo].[FormTemplate].ModifiedBy,
					[dbo].[FormTemplate].ModifiedDate
				)
			VALUES
				(
					@AccountTypeListId,
					@ServiceTypeListId,
					@FormTemplateTypeId,
					@FormName,
					@Description,
					@IsEnable,
					@IsDelete,
					NULL,
					GETDATE(),
					NULL,
					GETDATE()
				)
			SELECT @FormTemplateId = SCOPE_IDENTITY()
		END

		EXEC sp_GetTemplate @FormTemplateId, 1

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