IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormItemHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormItemHistory]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormItemHistory]      
(    
	@InspectionFormItemHistoryId INT = 0,    
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;    
    
	SELECT
		InspectionFormItemHistory.InspectionFormItemHistoryId AS InspectionFormItemId,
		InspectionFormItemHistory.InspectionFormSectionHistoryId,
		InspectionFormItemHistory.FormItemType,
		InspectionFormItemHistory.FormItemOrder,
		InspectionFormItemHistory.FormItemValue,
		InspectionFormItemHistory.IsDirty,
		InspectionFormItemHistory.IsRequired,
		InspectionFormItemHistory.IsEnable,
		InspectionFormItemHistory.IsDelete,
		InspectionFormItemHistory.CreatedBy,
		InspectionFormItemHistory.CreatedDate,
		InspectionFormItemHistory.ModifiedBy,
		InspectionFormItemHistory.ModifiedDate
	FROM
		[dbo].[InspectionFormSectionHistory] InspectionFormSectionHistory WITH (NOLOCK),
		[dbo].[InspectionFormItemHistory] InspectionFormItemHistory WITH (NOLOCK) 
	WHERE
		InspectionFormItemHistory.InspectionFormSectionHistoryId = InspectionFormSectionHistory.InspectionFormSectionHistoryId AND
		InspectionFormItemHistory.InspectionFormItemHistoryId = @InspectionFormItemHistoryId AND
		InspectionFormItemHistory.IsEnable = @IsEnable 
		 
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