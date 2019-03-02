IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_UpdateInspectionFormItemHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_UpdateInspectionFormItemHistory]
GO

CREATE PROCEDURE [dbo].[sp_UpdateInspectionFormItemHistory]
(
	@InspectionFormItemHistoryId INT = 0,
	@FormItemValue NVARCHAR(MAX),
	@IsDirty BIT = 0,
	@ModifiedBy INT = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	UPDATE
		[dbo].[InspectionFormItemHistory]
	SET
		[dbo].[InspectionFormItemHistory].FormItemValue = COALESCE(@FormItemValue, [dbo].[InspectionFormItemHistory].FormItemValue),
		[dbo].[InspectionFormItemHistory].IsDirty = @IsDirty,
		[dbo].[InspectionFormItemHistory].ModifiedBy = @ModifiedBy,
		[dbo].[InspectionFormItemHistory].ModifiedDate = GETDATE()
	WHERE
		[dbo].[InspectionFormItemHistory].InspectionFormItemHistoryId = @InspectionFormItemHistoryId

	EXEC sp_GetInspectionFormItemHistory @InspectionFormItemHistoryId, 1

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