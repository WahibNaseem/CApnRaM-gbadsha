IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[common_AddOrUpdateFile]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[common_AddOrUpdateFile]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[common_AddOrUpdateFile]
(
	@FileId INT = 0,
	@RegionId INT = 0,
	@ClassId INT = NULL,
	@FileType INT = NULL,
	@FileName NVARCHAR(128) = NULL,
	@ContentType NVARCHAR(16) = NULL,
	@FileUrl NVARCHAR(MAX) = NULL,
	@IsEnable BIT = 1,
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

	IF @RegionId < 1 SET @RegionId = NULL
	IF @ClassId < 1 SET @ClassId = NULL
	IF @FileType < 1 SET @FileType = NULL

	UPDATE 
		[dbo].[File]
	SET
		[dbo].[File].RegionId = COALESCE(@RegionId, [dbo].[File].RegionId),
		[dbo].[File].ClassId = COALESCE(@ClassId, [dbo].[File].ClassId),
		[dbo].[File].FileTypeId = COALESCE(@FileType, [dbo].[File].FileTypeId),
		[dbo].[File].FileName = COALESCE(@FileName, [dbo].[File].FileName),
		[dbo].[File].ContentType = COALESCE(@ContentType, [dbo].[File].ContentType),
		[dbo].[File].FileUrl = COALESCE(@FileUrl, [dbo].[File].FileUrl),
		[dbo].[File].IsEnable = @IsEnable,
		[dbo].[File].CreatedBy = COALESCE(@CreatedBy, [dbo].[File].CreatedBy),
		[dbo].[File].CreatedDate = COALESCE(@CreatedDate, [dbo].[File].CreatedDate),
		[dbo].[File].ModifiedBy = @ModifiedBy,
		[dbo].[File].ModifiedDate = COALESCE(@ModifiedDate, [dbo].[File].ModifiedDate)
	WHERE
		[dbo].[File].FileId = @FileId

	IF @@ROWCOUNT = 0
		BEGIN
		INSERT INTO [dbo].[File] 
		(
			[dbo].[File].RegionId,
			[dbo].[File].ClassId,
			[dbo].[File].FileTypeId,
			[dbo].[File].FileName,
			[dbo].[File].ContentType,
			[dbo].[File].IsEnable,
			[dbo].[File].CreatedBy,
			[dbo].[File].CreatedDate,
			[dbo].[File].ModifiedBy,
			[dbo].[File].ModifiedDate
		)
		VALUES
		(
			@RegionId,
			@ClassId,
			@FileType,
			@FileName,
			@ContentType,
			@IsEnable,
			@CreatedBy,
			@CreatedDate,
			@ModifiedBy,
			@ModifiedDate
		)
		SELECT @FileId = SCOPE_IDENTITY()
		END

	EXEC common_GetFile @FileId

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
        ROLLBACK TRANSACTION UPSERT;
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION UPSERT;
GO