IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplates]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplates]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetTemplates]
(
	@IsEnable BIT = 1,
	@PageNo INT = 1,
	@PageSize INT = 10,
	@SortColumn NVARCHAR(20) = '',
	@SortOrder NVARCHAR(4) ='asc',
	@SearchText NVARCHAR(200) = ''	
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	IF @PageNo <= 1 SET @PageNo = 1
	IF @PageSize <= 0 SET @PageSize = 10

	SELECT
		FormTemplate.FormTemplateId,
		FormTemplate.AccountTypeListId,
		FormTemplate.ServiceTypeListId,
		FormTemplate.FormTemplateTypeId,
		AccountTypeList.Name AS AccountTypeListName,
		ServiceTypeList.Name AS ServiceTypeListName,
		FormTemplateType.Name AS FormTemplateName,
		FormTemplate.FormName,
		FormTemplate.Description,
		FormTemplate.CreatedBy,
		FormTemplate.CreatedDate,
		FormTemplate.ModifiedBy,
		FormTemplate.ModifiedDate,
		ISNULL(COUNT(*) Over(), 0) AS TotalRecords
	FROM
		[dbo].[FormTemplate] FormTemplate
	LEFT JOIN [dbo].[AccountTypeList] AS AccountTypeList ON FormTemplate.AccountTypeListId = AccountTypeList.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList ON FormTemplate.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
	LEFT JOIN [dbo].[FormTemplateType] FormTemplateType ON FormTemplate.FormTemplateTypeId = FormTemplateType.FormTemplateTypeId
	WHERE
		FormTemplate.IsEnable = @IsEnable
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormTemplateId' AND LOWER(@SortOrder)='asc') THEN FormTemplate.FormTemplateId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormTemplateId' AND LOWER(@SortOrder)='desc') THEN FormTemplate.FormTemplateId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='asc') THEN FormTemplate.AccountTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='desc') THEN FormTemplate.AccountTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='asc') THEN FormTemplate.ServiceTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='desc') THEN FormTemplate.ServiceTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListName' AND LOWER(@SortOrder)='asc') THEN AccountTypeList.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListName' AND LOWER(@SortOrder)='desc') THEN AccountTypeList.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListName' AND LOWER(@SortOrder)='asc') THEN ServiceTypeList.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListName' AND LOWER(@SortOrder)='desc') THEN ServiceTypeList.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormTemplateName' AND LOWER(@SortOrder)='asc') THEN FormTemplateType.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormTemplateName' AND LOWER(@SortOrder)='desc') THEN FormTemplateType.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='asc') THEN FormTemplate.FormName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='desc') THEN FormTemplate.FormName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='asc') THEN FormTemplate.Description END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='desc') THEN FormTemplate.Description END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN FormTemplate.FormTemplateId END ASC
	OFFSET (@PageNo-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY

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
GO