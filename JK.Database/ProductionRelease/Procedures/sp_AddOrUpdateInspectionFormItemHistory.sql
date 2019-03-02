IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionFormItemHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormItemHistory]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormItemHistory]  
(  
	@InspectionFormItemHistoryId INT = 0,  
	@InspectionFormSectionHistoryId INT = 0,  
	@FormItemType INT = NULL,
	@FormItemOrder INT = 0,
	@FormItemValue NVARCHAR(MAX) = NULL,
	@IsDirty BIT = 0,
	@IsRequired BIT = 0,  
	@CreatedBy INT = NULL,  
	@CreatedDate DATETIME = NULL,  
	@ModifiedBy INT = NULL,  
	@ModifiedDate DATETIME = NULL  
)  
AS  

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRANSACTION UPSERT  

BEGIN TRY  

	SET NOCOUNT ON;  

	IF @InspectionFormSectionHistoryId < 1 SET @InspectionFormSectionHistoryId = NULL  
	IF @FormItemOrder < 1 SET @FormItemOrder = NULL  

	UPDATE  
		[dbo].[InspectionFormItemHistory]  
	SET  
		[dbo].[InspectionFormItemHistory].InspectionFormSectionHistoryId = COALESCE(@InspectionFormSectionHistoryId, [dbo].[InspectionFormItemHistory].InspectionFormSectionHistoryId),
		[dbo].[InspectionFormItemHistory].FormItemType = COALESCE(@FormItemType, [dbo].[InspectionFormItemHistory].FormItemType),
		[dbo].[InspectionFormItemHistory].FormItemOrder = COALESCE(@FormItemOrder, [dbo].[InspectionFormItemHistory].FormItemOrder),
		[dbo].[InspectionFormItemHistory].FormItemValue = COALESCE(@FormItemValue, [dbo].[InspectionFormItemHistory].FormItemValue),
		[dbo].[InspectionFormItemHistory].IsDirty = COALESCE(@IsDirty, [dbo].[InspectionFormItemHistory].IsDirty),
		[dbo].[InspectionFormItemHistory].IsRequired = COALESCE(@IsRequired, [dbo].[InspectionFormItemHistory].IsRequired),
		[dbo].[InspectionFormItemHistory].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionFormItemHistory].CreatedBy),  
		[dbo].[InspectionFormItemHistory].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionFormItemHistory].CreatedDate),  
		[dbo].[InspectionFormItemHistory].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionFormItemHistory].ModifiedBy),  
		[dbo].[InspectionFormItemHistory].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionFormItemHistory].ModifiedDate)  
	WHERE  
		[dbo].[InspectionFormItemHistory].InspectionFormItemHistoryId = @InspectionFormItemHistoryId  

	IF @@ROWCOUNT = 0  
	BEGIN  
	INSERT INTO [dbo].[InspectionFormItemHistory]  
	(
		[dbo].[InspectionFormItemHistory].InspectionFormSectionHistoryId,
		[dbo].[InspectionFormItemHistory].FormItemType,
		[dbo].[InspectionFormItemHistory].FormItemOrder,
		[dbo].[InspectionFormItemHistory].FormItemValue,
		[dbo].[InspectionFormItemHistory].IsDirty,
		[dbo].[InspectionFormItemHistory].IsRequired,
		[dbo].[InspectionFormItemHistory].IsEnable,
		[dbo].[InspectionFormItemHistory].IsDelete,
		[dbo].[InspectionFormItemHistory].CreatedBy,
		[dbo].[InspectionFormItemHistory].CreatedDate,
		[dbo].[InspectionFormItemHistory].ModifiedBy,
		[dbo].[InspectionFormItemHistory].ModifiedDate  
	  )  
	VALUES  
	( 
		@InspectionFormSectionHistoryId,  
		@FormItemType,
		@FormItemOrder,
		@FormItemValue,
		@IsDirty,
		@IsRequired,  
		1,
		0,
		@CreatedBy,  
		@CreatedDate,  
		@ModifiedBy,  
		@ModifiedDate
	)
	SELECT @InspectionFormItemHistoryId = SCOPE_IDENTITY()
	END  

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