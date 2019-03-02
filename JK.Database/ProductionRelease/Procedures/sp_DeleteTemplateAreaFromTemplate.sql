IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_DeleteTemplateAreaFromTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_DeleteTemplateAreaFromTemplate]
GO  

CREATE PROCEDURE [dbo].[sp_DeleteTemplateAreaFromTemplate]  
(
	@FormTemplateId INT = 0,
	@TemplateAreaId INT = 0
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

	DELETE FROM  
		[dbo].[FormTemplateAreaMapping]
	WHERE  
		[dbo].[FormTemplateAreaMapping].FormTemplateAreaMappingId = @FormTemplateAreaMappingId

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