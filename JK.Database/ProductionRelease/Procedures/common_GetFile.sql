IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) where id = object_id(N'[dbo].[common_GetFile]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[common_GetFile] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[common_GetFile]
(
	@FileId INT = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	SELECT TOP 1
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
		[dbo].[File].FileId = @FileId

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