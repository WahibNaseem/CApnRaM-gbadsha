IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateTemplateAreaToForm]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaToForm]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaToForm]  
(
	@FormTemplateId INT = 0,
	@TemplateAreaId INT = 0,
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

	DECLARE @FormTemplateAreaMappingId INT = NULL

	SELECT 
		@FormTemplateAreaMappingId = FormTemplateAreaMapping.FormTemplateAreaMappingId 
	FROM 
		[dbo].[FormTemplateAreaMapping] FormTemplateAreaMapping WITH (NOLOCK) 
	WHERE 
		FormTemplateAreaMapping.FormTemplateId = @FormTemplateId AND 
		FormTemplateAreaMapping.TemplateAreaId = @TemplateAreaId AND
		FormTemplateAreaMapping.IsEnable = 1

	UPDATE  
		[dbo].[FormTemplateAreaMapping]  
	SET
		[dbo].[FormTemplateAreaMapping].FormTemplateId = COALESCE(@FormTemplateId, [dbo].[FormTemplateAreaMapping].FormTemplateId),
		[dbo].[FormTemplateAreaMapping].TemplateAreaId = COALESCE(@TemplateAreaId, [dbo].[FormTemplateAreaMapping].TemplateAreaId),
		[dbo].[FormTemplateAreaMapping].IsEnable = COALESCE(@IsEnable, [dbo].[FormTemplateAreaMapping].IsEnable),
		[dbo].[FormTemplateAreaMapping].IsDelete = COALESCE(@IsDelete, [dbo].[FormTemplateAreaMapping].IsDelete),
		[dbo].[FormTemplateAreaMapping].CreatedBy = COALESCE(@CreatedBy, [dbo].[FormTemplateAreaMapping].CreatedBy),  
		[dbo].[FormTemplateAreaMapping].CreatedOn = COALESCE(@CreatedDate, [dbo].[FormTemplateAreaMapping].CreatedOn),  
		[dbo].[FormTemplateAreaMapping].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[FormTemplateAreaMapping].ModifiedBy),  
		[dbo].[FormTemplateAreaMapping].ModifiedOn = COALESCE(@ModifiedDate, [dbo].[FormTemplateAreaMapping].ModifiedOn)  
	WHERE  
		[dbo].[FormTemplateAreaMapping].FormTemplateAreaMappingId = @FormTemplateAreaMappingId

	IF @@ROWCOUNT = 0
		BEGIN
		INSERT INTO [dbo].[FormTemplateAreaMapping]
		(
			[dbo].[FormTemplateAreaMapping].FormTemplateId,
			[dbo].[FormTemplateAreaMapping].TemplateAreaId,
			[dbo].[FormTemplateAreaMapping].IsEnable,
			[dbo].[FormTemplateAreaMapping].IsDelete,
			[dbo].[FormTemplateAreaMapping].CreatedBy,
			[dbo].[FormTemplateAreaMapping].CreatedOn,
			[dbo].[FormTemplateAreaMapping].ModifiedBy,
			[dbo].[FormTemplateAreaMapping].ModifiedOn
		)
		VALUES
		(
			@FormTemplateId,
			@TemplateAreaId,
			@IsEnable,
			@IsDelete, 
			@CreatedBy,  
			@CreatedDate,  
			@ModifiedBy,  
			@ModifiedDate
		)
		END

	EXEC sp_GetTemplateArea @TemplateAreaId, 1

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