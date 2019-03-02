IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormItemBySection]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormItemBySection]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormItemBySection]    
(
	@InspectionFormSectionId INT = 0,  
	@IsEnable BIT = 1,  
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'  
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
		InspectionFormItem.CreatedBy,  
		InspectionFormItem.CreatedDate,  
		InspectionFormItem.ModifiedBy,  
		InspectionFormItem.ModifiedDate  
	FROM  
		[dbo].[InspectionFormItem] InspectionFormItem WITH (NOLOCK)  
	WHERE  
		InspectionFormItem.InspectionFormSectionId = @InspectionFormSectionId AND  
		InspectionFormItem.IsEnable = @IsEnable  
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormItemId' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.InspectionFormItemId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormItemId' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.InspectionFormItemId END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionId' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.InspectionFormSectionId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionId' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.InspectionFormSectionId END DESC, 
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.FormItemType END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.FormItemType END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemOrder' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.FormItemOrder END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemOrder' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.FormItemOrder END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.FormItemValue END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.FormItemValue END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.IsDirty END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.IsDirty END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.IsRequired END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.IsRequired END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.IsEnable END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.IsEnable END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.CreatedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.CreatedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.CreatedDate END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.CreatedDate END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.ModifiedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.ModifiedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormItem.ModifiedDate END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormItem.ModifiedDate END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionFormItem.InspectionFormItemId END ASC  
  
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