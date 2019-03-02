IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormItem]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormItem]      
(    
	@InspectionFormItemId INT = 0,    
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;    
    
	SELECT
		InspectionFormItem.InspectionFormItemId,
		InspectionFormItem.InspectionFormSectionId,
		InspectionFormItem.FormItemType,
		InspectionFormItem.FormItemOrder,
		InspectionFormItem.FormItemValue,
		InspectionFormItem.IsDirty,
		InspectionFormItem.IsRequired,
		InspectionFormItem.IsEnable,
		InspectionFormItem.IsDelete,
		InspectionFormItem.CreatedBy,
		InspectionFormItem.CreatedDate,
		InspectionFormItem.ModifiedBy,
		InspectionFormItem.ModifiedDate
	FROM
		[dbo].[InspectionFormSection] InspectionFormSection WITH (NOLOCK),
		[dbo].[InspectionFormItem] InspectionFormItem WITH (NOLOCK) 
	WHERE
		InspectionFormItem.InspectionFormSectionId = InspectionFormSection.InspectionFormSectionId AND
		InspectionFormItem.InspectionFormItemId = @InspectionFormItemId AND
		InspectionFormItem.IsEnable = @IsEnable 
		 
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