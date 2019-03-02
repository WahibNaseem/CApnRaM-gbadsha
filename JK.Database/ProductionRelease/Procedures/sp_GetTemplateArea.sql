IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplateArea]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplateArea]
GO  

CREATE PROCEDURE [dbo].[sp_GetTemplateArea]      
(    
	@TemplateAreaId INT = 0,    
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;    
    
	SELECT
		TemplateArea.TemplateAreaId,
		TemplateArea.AreaName,
		TemplateArea.IsEnable,
		TemplateArea.IsDelete,
		TemplateArea.CreatedBy,
		TemplateArea.CreatedOn AS CreatedDate,
		TemplateArea.ModifiedBy,
		TemplateArea.ModifiedOn AS ModifiedDate
	FROM
		[dbo].[TemplateArea] TemplateArea WITH (NOLOCK)
	WHERE
		TemplateArea.TemplateAreaId = @TemplateAreaId AND
		TemplateArea.IsEnable = @IsEnable 
		 
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