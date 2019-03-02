IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetAccountWalkThruItemListByCustomer]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetAccountWalkThruItemListByCustomer]
GO  

CREATE PROCEDURE [dbo].[sp_GetAccountWalkThruItemListByCustomer]      
(
	@CustomerId INT = 0,
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		AccountWalkThruFormFieldDetail.CSAccountWalkThursFormFieldDetailId AS AccountWalkThruItemId,
		AccountWalkThruFormField.CSAccountWalkThursFormFieldId AS AccountWalkThruType,
		AccountWalkThruFormFieldDetail.CustomerId,
		AccountWalkThruFormFieldDetail.FranchiseeId,
		AccountWalkThruFormField.Title,
		AccountWalkThruFormFieldDetail.FieldValue,
		AccountWalkThruFormFieldDetail.FieldText,
		AccountWalkThruFormFieldDetail.FileUrl,
		AccountWalkThruFormFieldDetail.IsActive AS IsEnable,
		AccountWalkThruFormFieldDetail.CreatedBy,
		AccountWalkThruFormFieldDetail.CreatedDate,
		AccountWalkThruFormFieldDetail.ModifiedBy,
		AccountWalkThruFormFieldDetail.ModifiedDate
	FROM
		[dbo].[CSAccountWalkThursFormField] AccountWalkThruFormField WITH (NOLOCK), 
		[dbo].[CSAccountWalkThursFormFieldDetail] AccountWalkThruFormFieldDetail WITH (NOLOCK)
	WHERE
		AccountWalkThruFormFieldDetail.CustomerId = @CustomerId AND
		AccountWalkThruFormFieldDetail.CSAccountWalkThursFormFieldId = AccountWalkThruFormField.CSAccountWalkThursFormFieldId AND
		AccountWalkThruFormField.IsActive = @IsEnable AND
		AccountWalkThruFormFieldDetail.IsActive = @IsEnable
		 
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