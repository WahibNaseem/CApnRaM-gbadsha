IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplateAreaList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplateAreaList]
GO  

CREATE PROCEDURE [dbo].[sp_GetTemplateAreaList]      
(      
	@IsEnable BIT = 1,
	@SortColumn NVARCHAR(20) = '',
	@SortOrder NVARCHAR(4) ='asc'
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;    
    
	SELECT
		TemplateArea.TemplateAreaId,
		TemplateArea.AreaName,
		TemplateArea.IsEnable,
		TemplateArea.IsDelete,
		TemplateArea.CreatedBy,
		TemplateArea.CreatedOn AS CreatedDate,
		TemplateArea.ModifiedBy,
		TemplateArea.ModifiedOn AS ModifiedDate
	FROM
		[dbo].[TemplateArea] TemplateArea WITH (NOLOCK)
	WHERE
		TemplateArea.IsEnable = @IsEnable 
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'TemplateAreaId' AND LOWER(@SortOrder)='asc') THEN TemplateArea.TemplateAreaId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'TemplateAreaId' AND LOWER(@SortOrder)='desc') THEN TemplateArea.TemplateAreaId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AreaName' AND LOWER(@SortOrder)='asc') THEN TemplateArea.AreaName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AreaName' AND LOWER(@SortOrder)='desc') THEN TemplateArea.AreaName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='asc') THEN TemplateArea.IsEnable END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='desc') THEN TemplateArea.IsEnable END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDelete' AND LOWER(@SortOrder)='asc') THEN TemplateArea.IsDelete END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDelete' AND LOWER(@SortOrder)='desc') THEN TemplateArea.IsDelete END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN TemplateArea.CreatedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN TemplateArea.CreatedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN TemplateArea.CreatedOn END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN TemplateArea.CreatedOn END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN TemplateArea.ModifiedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN TemplateArea.ModifiedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN TemplateArea.ModifiedOn END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN TemplateArea.ModifiedOn END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN TemplateArea.TemplateAreaId END ASC
		 
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