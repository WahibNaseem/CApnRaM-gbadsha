IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_DeleteTemplateAreaItemFromArea]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_DeleteTemplateAreaItemFromArea]
GO  

CREATE PROCEDURE [dbo].[sp_DeleteTemplateAreaItemFromArea]  
(
	@TemplateAreaId INT = 0,
	@TemplateAreaItemId INT = 0
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

	DELETE FROM  
		[dbo].[TemplateAreaItemMapping]
	WHERE  
		[dbo].[TemplateAreaItemMapping].TemplateAreaItemMappingId = @TemplateAreaItemMappingId

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