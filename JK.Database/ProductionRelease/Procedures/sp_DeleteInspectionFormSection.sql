IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_DeleteInspectionFormSection]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_DeleteInspectionFormSection]
GO  

CREATE PROCEDURE [dbo].[sp_DeleteInspectionFormSection]  
(  
	@InspectionFormSectionId INT = 0
)  
AS  

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRANSACTION UPSERT  

BEGIN TRY  

	SET NOCOUNT ON;
	
	DELETE FROM
		[dbo].[InspectionFormSection]
	WHERE
		[dbo].[InspectionFormSection].InspectionFormSectionId = @InspectionFormSectionId
		
	DELETE FROM
		[dbo].[InspectionFormItem]
	WHERE
		[dbo].[InspectionFormItem].InspectionFormSectionId = @InspectionFormSectionId 

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