IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetStateList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetStateList]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetStateList]  
(
	@IsEnable BIT = 1,
	@SortColumn NVARCHAR(20) = '',
	@SortOrder NVARCHAR(4) ='asc',
	@SearchText NVARCHAR(200) = ''
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		StateList.StateListId,
		StateList.Name,
		StateList.abbr AS Abbr,
		StateList.CountryCodeListId,
		StateList.LedgerAcctId,
		StateList.LedgerSubAcctId,
		StateList.isActive,
		StateList.CreatedBy,
		StateList.CreatedDate
	FROM
		[dbo].[StateList] StateList WITH (NOLOCK)
	WHERE
		StateList.isActive = @IsEnable
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'StateListId' AND LOWER(@SortOrder)='asc') THEN StateList.StateListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StateListId' AND LOWER(@SortOrder)='desc') THEN StateList.StateListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Name' AND LOWER(@SortOrder)='asc') THEN StateList.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Name' AND LOWER(@SortOrder)='desc') THEN StateList.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Abbr' AND LOWER(@SortOrder)='asc') THEN StateList.abbr END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Abbr' AND LOWER(@SortOrder)='desc') THEN StateList.abbr END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CountryCodeListId' AND LOWER(@SortOrder)='asc') THEN StateList.CountryCodeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CountryCodeListId' AND LOWER(@SortOrder)='desc') THEN StateList.CountryCodeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LedgerAcctId' AND LOWER(@SortOrder)='asc') THEN StateList.LedgerAcctId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LedgerAcctId' AND LOWER(@SortOrder)='desc') THEN StateList.LedgerAcctId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LedgerSubAcctId' AND LOWER(@SortOrder)='asc') THEN StateList.LedgerSubAcctId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LedgerSubAcctId' AND LOWER(@SortOrder)='desc') THEN StateList.LedgerSubAcctId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'isActive' AND LOWER(@SortOrder)='asc') THEN StateList.isActive END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'isActive' AND LOWER(@SortOrder)='desc') THEN StateList.isActive END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN StateList.CreatedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN StateList.CreatedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN StateList.CreatedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN StateList.CreatedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN StateList.StateListId END ASC
		
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
	[