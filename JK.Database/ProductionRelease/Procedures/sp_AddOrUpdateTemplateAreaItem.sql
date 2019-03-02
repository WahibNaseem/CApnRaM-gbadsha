IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateTemplateAreaItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaItem]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateTemplateAreaItem]  
(  
	@TemplateAreaItemId INT = 0,  
	@ItemName NVARCHAR(100) = NULL,  
	@FormItemType INT = NULL,
	@FormItemValue NVARCHAR(MAX) = NULL,
	@IsRequired BIT = 0,
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

	UPDATE  
		[dbo].[TemplateAreaItem]  
	SET
		[dbo].[TemplateAreaItem].ItemName = COALESCE(@ItemName, [dbo].[TemplateAreaItem].ItemName),
		[dbo].[TemplateAreaItem].FormItemType = COALESCE(@FormItemType, [dbo].[TemplateAreaItem].FormItemType),
		[dbo].[TemplateAreaItem].FormItemValue = COALESCE(@FormItemValue, [dbo].[TemplateAreaItem].FormItemValue),
		[dbo].[TemplateAreaItem].IsRequired = COALESCE(@IsRequired, [dbo].[TemplateAreaItem].IsRequired),
		[dbo].[TemplateAreaItem].IsEnable = COALESCE(@IsEnable, [dbo].[TemplateAreaItem].IsEnable),
		[dbo].[TemplateAreaItem].IsDelete = COALESCE(@IsDelete, [dbo].[TemplateAreaItem].IsDelete),
		[dbo].[TemplateAreaItem].CreatedBy = COALESCE(@CreatedBy, [dbo].[TemplateAreaItem].CreatedBy),  
		[dbo].[TemplateAreaItem].CreatedOn = COALESCE(@CreatedDate, [dbo].[TemplateAreaItem].CreatedOn),  
		[dbo].[TemplateAreaItem].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[TemplateAreaItem].ModifiedBy),  
		[dbo].[TemplateAreaItem].ModifiedOn = COALESCE(@ModifiedDate, [dbo].[TemplateAreaItem].ModifiedOn)  
	WHERE  
		[dbo].[TemplateAreaItem].TemplateAreaItemId = @TemplateAreaItemId  

	IF @@ROWCOUNT = 0  
	BEGIN  
	INSERT INTO [dbo].[TemplateAreaItem]  
	(
		[dbo].[TemplateAreaItem].TemplateAreaItemId,
		[dbo].[TemplateAreaItem].ItemName,
		[dbo].[TemplateAreaItem].FormItemType,
		[dbo].[TemplateAreaItem].FormItemValue,
		[dbo].[TemplateAreaItem].IsRequired,
		[dbo].[TemplateAreaItem].IsEnable,
		[dbo].[TemplateAreaItem].IsDelete,
		[dbo].[TemplateAreaItem].CreatedBy,
		[dbo].[TemplateAreaItem].CreatedOn,
		[dbo].[TemplateAreaItem].ModifiedBy,
		[dbo].[TemplateAreaItem].ModifiedOn  
	  )  
	VALUES  
	( 
		@TemplateAreaItemId,  
		@ItemName,
		@FormItemType,
		@FormItemValue,
		@IsRequired,
		@IsEnable,
		@IsDelete, 
		@CreatedBy,  
		@CreatedDate,  
		@ModifiedBy,  
		@ModifiedDate
	)
	SELECT @TemplateAreaItemId = SCOPE_IDENTITY()
	END  

	EXEC sp_GetTemplateAreaItem @TemplateAreaItemId, 1  

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