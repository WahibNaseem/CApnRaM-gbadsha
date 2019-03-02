IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Update_CRM_AccountCustomerDetail_Coordinate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Update_CRM_AccountCustomerDetail_Coordinate]
GO
  
CREATE PROCEDURE [dbo].[sp_Update_CRM_AccountCustomerDetail_Coordinate]     
(  
	@CRM_AccountCustomerDetailId INT = 0,   
	@Latitude DECIMAL(18,8) = NULL,
	@Longitude DECIMAL(18,8) = NULL   
)  
AS  
  
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRANSACTION UPSERT  

BEGIN TRY  

	SET NOCOUNT ON;  

	UPDATE
		[dbo].[CRM_AccountCustomerDetail]  
	SET
		[dbo].[CRM_AccountCustomerDetail].CompanyLatitude = @Latitude,
		[dbo].[CRM_AccountCustomerDetail].CompanyLongitude = @Longitude,
		[dbo].[CRM_AccountCustomerDetail].ModifiedDate = GETDATE()
	WHERE  
		[dbo].[CRM_AccountCustomerDetail].CRM_AccountCustomerDetailId = @CRM_AccountCustomerDetailId  

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