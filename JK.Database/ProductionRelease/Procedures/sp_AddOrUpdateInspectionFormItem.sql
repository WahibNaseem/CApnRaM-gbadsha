IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateInspectionFormItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormItem]
GO  

CREATE PROCEDURE [dbo].[sp_AddOrUpdateInspectionFormItem]  
(  
	@InspectionFormItemId INT = 0,  
	@InspectionFormSectionId INT = 0,  
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

	IF @InspectionFormSectionId < 1 SET @InspectionFormSectionId = NULL  
	IF @FormItemOrder < 1 SET @FormItemOrder = NULL  

	UPDATE  
		[dbo].[InspectionFormItem]  
	SET  
		[dbo].[InspectionFormItem].InspectionFormSectionId = COALESCE(@InspectionFormSectionId, [dbo].[InspectionFormItem].InspectionFormSectionId),
		[dbo].[InspectionFormItem].FormItemType = COALESCE(@FormItemType, [dbo].[InspectionFormItem].FormItemType),
		[dbo].[InspectionFormItem].FormItemOrder = COALESCE(@FormItemOrder, [dbo].[InspectionFormItem].FormItemOrder),
		[dbo].[InspectionFormItem].FormItemValue = COALESCE(@FormItemValue, [dbo].[InspectionFormItem].FormItemValue),
		[dbo].[InspectionFormItem].IsDirty = COALESCE(@IsDirty, [dbo].[InspectionFormItem].IsDirty),
		[dbo].[InspectionFormItem].IsRequired = COALESCE(@IsRequired, [dbo].[InspectionFormItem].IsRequired),
		[dbo].[InspectionFormItem].CreatedBy = COALESCE(@CreatedBy, [dbo].[InspectionFormItem].CreatedBy),  
		[dbo].[InspectionFormItem].CreatedDate = COALESCE(@CreatedDate, [dbo].[InspectionFormItem].CreatedDate),  
		[dbo].[InspectionFormItem].ModifiedBy = COALESCE(@ModifiedBy, [dbo].[InspectionFormItem].ModifiedBy),  
		[dbo].[InspectionFormItem].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[InspectionFormItem].ModifiedDate)  
	WHERE  
		[dbo].[InspectionFormItem].InspectionFormItemId = @InspectionFormItemId  

	IF @@ROWCOUNT = 0  
	BEGIN  
	INSERT INTO [dbo].[InspectionFormItem]  
	(
		[dbo].[InspectionFormItem].InspectionFormSectionId,
		[dbo].[InspectionFormItem].FormItemType,
		[dbo].[InspectionFormItem].FormItemOrder,
		[dbo].[InspectionFormItem].FormItemValue,
		[dbo].[InspectionFormItem].IsDirty,
		[dbo].[InspectionFormItem].IsRequired,
		[dbo].[InspectionFormItem].IsEnable,
		[dbo].[InspectionFormItem].IsDelete,
		[dbo].[InspectionFormItem].CreatedBy,
		[dbo].[InspectionFormItem].CreatedDate,
		[dbo].[InspectionFormItem].ModifiedBy,
		[dbo].[InspectionFormItem].ModifiedDate  
	  )  
	VALUES  
	( 
		@InspectionFormSectionId,  
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
	SELECT @InspectionFormItemId = SCOPE_IDENTITY()
	END  

	EXEC sp_GetInspectionFormItem @InspectionFormItemId, 1  

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