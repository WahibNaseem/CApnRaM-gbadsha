IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplateAreaItemList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplateAreaItemList]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetTemplateAreaItemList]    
( 
	@IsEnable BIT = 1,  
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'  
)  
AS  
  
BEGIN TRANSACTION;  
  
	BEGIN TRY  
  
	SET NOCOUNT ON;  
  
	SELECT
		TemplateAreaItem.TemplateAreaItemId,
		TemplateAreaItem.ItemName,
		TemplateAreaItem.FormItemType,
		TemplateAreaItem.FormItemValue,
		TemplateAreaItem.IsDirty,
		TemplateAreaItem.IsRequired,
		TemplateAreaItem.IsEnable,
		TemplateAreaItem.IsDelete,
		TemplateAreaItem.CreatedBy,
		TemplateAreaItem.CreatedOn AS CreatedDate,
		TemplateAreaItem.ModifiedBy,
		TemplateAreaItem.ModifiedOn AS ModifiedDate 
	FROM  
		[dbo].[TemplateArea] TemplateArea WITH (NOLOCK),
		[dbo].[TemplateAreaItemMapping] TemplateAreaItemMapping WITH (NOLOCK),
		[dbo].[TemplateAreaItem] TemplateAreaItem WITH (NOLOCK)
	WHERE
		TemplateAreaItemMapping.TemplateAreaItemId = TemplateAreaItem.TemplateAreaItemId AND
		TemplateAreaItemMapping.TemplateAreaId = TemplateArea.TemplateAreaId AND
		TemplateArea.IsEnable = @IsEnable AND
		TemplateAreaItemMapping.IsEnable = 1 AND
		TemplateAreaItem.IsEnable = 1
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'TemplateAreaItemId' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.TemplateAreaItemId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'TemplateAreaItemId' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.TemplateAreaItemId END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ItemName' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.ItemName END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ItemName' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.ItemName END DESC, 
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.FormItemType END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.FormItemType END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.FormItemValue END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.FormItemValue END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.IsDirty END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.IsDirty END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.IsRequired END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.IsRequired END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.IsEnable END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.IsEnable END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.CreatedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.CreatedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.CreatedOn END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.CreatedOn END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.ModifiedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.ModifiedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN TemplateAreaItem.ModifiedOn END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN TemplateAreaItem.ModifiedOn END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN TemplateAreaItem.TemplateAreaItemId END ASC  
  
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