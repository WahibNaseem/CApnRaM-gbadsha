IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateTemplateAreaItemToArea]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaItemToArea]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaItemToArea]  
(
	@TemplateAreaId INT,
	@TemplateAreaItemId INT = 0,
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

	DECLARE @TemplateAreaItemMappingId INT = NULL

	SELECT 
		@TemplateAreaItemMappingId = TemplateAreaItemMapping.TemplateAreaItemMappingId 
	FROM 
		[dbo].[TemplateAreaItemMapping] TemplateAreaItemMapping WITH (NOLOCK) 
	WHERE 
		TemplateAreaItemMapping.TemplateAreaItemId = @TemplateAreaItemId AND 
		TemplateAreaItemMapping.TemplateAreaId = @TemplateAreaId AND
		TemplateAreaItemMapping.IsEnable = 1

	UPDATE  
		[dbo].[TemplateAreaItemMapping]  
	SET
		[dbo].[TemplateAreaItemMapping].TemplateAreaId = COALESCE(@TemplateAreaId, [dbo].[TemplateAreaItemMapping].TemplateAreaId),
		[dbo].[TemplateAreaItemMapping].TemplateAreaItemId = COALESCE(@TemplateAreaItemId, [dbo].[TemplateAreaItemMapping].TemplateAreaItemId),
		[dbo].[TemplateAreaItemMapping].IsEnable = COALESCE(@IsEnable, [dbo].[TemplateAreaItemMapping].IsEnable),
		[dbo].[TemplateAreaItemMapping].IsDelete = COALESCE(@IsDelete, [dbo].[TemplateAreaItemMapping].IsDelete),
		[dbo].[TemplateAreaItemMapping].CreatedBy = COALESCE(@CreatedBy, [dbo].[TemplateAreaItemMapping].CreatedBy),  
		[dbo].[TemplateAreaItemMapping].CreatedOn = COALESCE(@CreatedDate, [dbo].[TemplateAreaItemMapping].CreatedOn),  
		[dbo].[TemplateAreaItemMapping].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[TemplateAreaItemMapping].ModifiedBy),  
		[dbo].[TemplateAreaItemMapping].ModifiedOn = COALESCE(@ModifiedDate, [dbo].[TemplateAreaItemMapping].ModifiedOn)  
	WHERE  
		[dbo].[TemplateAreaItemMapping].TemplateAreaItemMappingId = @TemplateAreaItemMappingId

	IF @@ROWCOUNT = 0
		BEGIN
		INSERT INTO [dbo].[TemplateAreaItemMapping]
		(
			[dbo].[TemplateAreaItemMapping].TemplateAreaId,
			[dbo].[TemplateAreaItemMapping].TemplateAreaItemId,
			[dbo].[TemplateAreaItemMapping].IsEnable,
			[dbo].[TemplateAreaItemMapping].IsDelete,
			[dbo].[TemplateAreaItemMapping].CreatedBy,
			[dbo].[TemplateAreaItemMapping].CreatedOn,
			[dbo].[TemplateAreaItemMapping].ModifiedBy,
			[dbo].[TemplateAreaItemMapping].ModifiedOn
		)
		VALUES
		(
			@TemplateAreaId,
			@TemplateAreaItemId,
			@IsEnable,
			@IsDelete, 
			@CreatedBy,  
			@CreatedDate,  
			@ModifiedBy,  
			@ModifiedDate
		)
		END

	EXEC sp_GetTemplateAreaItemListByArea @TemplateAreaId, 1

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