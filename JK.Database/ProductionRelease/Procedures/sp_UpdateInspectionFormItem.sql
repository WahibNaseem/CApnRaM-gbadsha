IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_UpdateInspectionFormItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_UpdateInspectionFormItem]
GO

CREATE PROCEDURE [dbo].[sp_UpdateInspectionFormItem]
(
	@InspectionFormItemId INT = 0,
	@FormItemValue NVARCHAR(MAX),
	@IsDirty BIT = 0,
	@ModifiedBy INT = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	UPDATE
		[dbo].[InspectionFormItem]
	SET
		[dbo].[InspectionFormItem].FormItemValue = COALESCE(@FormItemValue, [dbo].[InspectionFormItem].FormItemValue),
		[dbo].[InspectionFormItem].IsDirty = @IsDirty,
		[dbo].[InspectionFormItem].ModifiedBy = @ModifiedBy,
		[dbo].[InspectionFormItem].ModifiedDate = GETDATE()
	WHERE
		[dbo].[InspectionFormItem].InspectionFormItemId = @InspectionFormItemId

	SELECT
		InspectionFormItem.InspectionFormItemId,
		InspectionFormItem.InspectionFormSectionId,
		InspectionFormItem.FormItemType,
		InspectionFormItem.FormItemOrder,
		InspectionFormItem.FormItemValue,
		InspectionFormItem.IsDirty,
		InspectionFormItem.IsRequired,
		InspectionFormItem.IsEnable,
		InspectionFormItem.CreatedBy,
		InspectionFormItem.CreatedDate,
		InspectionFormItem.ModifiedBy,
		InspectionFormItem.ModifiedDate
	FROM
		[dbo].[InspectionFormItem] InspectionFormItem WITH (NOLOCK)
	WHERE
		InspectionFormItem.InspectionFormItemId = @InspectionFormItemId

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