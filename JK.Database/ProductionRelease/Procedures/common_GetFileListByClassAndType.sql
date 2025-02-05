IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[common_GetFileListByClassAndType]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[common_GetFileListByClassAndType] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[common_GetFileListByClassAndType]
(
	@ClassId INT = 0,
	@FileType INT = 0,
	@IsEnable BIT = 1,
	@SortColumn NVARCHAR(20) = '',
	@SortOrder NVARCHAR(4) = 'asc'
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		[dbo].[File].FileId,
		[dbo].[File].RegionId,
		[dbo].[File].ClassId,
		[dbo].[File].FileTypeId AS FileType,
		[dbo].[File].FileName,
		[dbo].[File].ContentType,
		[dbo].[File].FileUrl,
		[dbo].[File].IsEnable,
		[dbo].[File].CreatedBy,
		[dbo].[File].CreatedDate,
		[dbo].[File].ModifiedBy,
		[dbo].[File].ModifiedDate
	FROM
		[dbo].[File] WITH (NOLOCK)
	WHERE
		[dbo].[File].ClassId = @ClassId AND
		[dbo].[File].FileTypeId = @FileType AND
		[dbo].[File].IsEnable = @IsEnable
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileId' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].FileId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileId' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].FileId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].RegionId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].RegionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ClassId' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].ClassId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ClassId' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].ClassId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileType' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].FileTypeId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileType' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].FileTypeId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileName' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].FileName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileName' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].FileName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContentType' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].ContentType END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContentType' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].ContentType END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileUrl' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].FileUrl END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FileUrl' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].FileUrl END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].IsEnable END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].IsEnable END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].CreatedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].CreatedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].CreatedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].CreatedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].ModifiedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].ModifiedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN [dbo].[File].ModifiedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN [dbo].[File].ModifiedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN [dbo].[File].FileId END ASC
		
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