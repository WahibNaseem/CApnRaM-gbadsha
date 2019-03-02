IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplateAreaItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplateAreaItem]
GO  

CREATE PROCEDURE [dbo].[sp_GetTemplateAreaItem]      
(    
	@TemplateAreaItemId INT = 0,    
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;    
    
	SELECT
		TemplateAreaItem.TemplateAreaItemId,
		TemplateAreaItem.ItemName,
		TemplateAreaItem.FormItemType,
		TemplateAreaItem.FormItemValue,
		TemplateAreaItem.IsDirty,
		TemplateAreaItem.IsRequired,
		TemplateAreaItem.IsEnable,
		TemplateAreaItem.IsDelete,
		TemplateAreaItem.CreatedBy,
		TemplateAreaItem.CreatedOn AS CreatedDate,
		TemplateAreaItem.ModifiedBy,
		TemplateAreaItem.ModifiedOn AS ModifiedDate  
	FROM
		[dbo].[TemplateAreaItem] TemplateAreaItem WITH (NOLOCK) 
	WHERE
		TemplateAreaItem.TemplateAreaItemId = @TemplateAreaItemId AND
		TemplateAreaItem.IsEnable = @IsEnable 
		 
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