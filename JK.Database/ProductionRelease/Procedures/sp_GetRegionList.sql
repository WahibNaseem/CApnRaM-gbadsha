IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetRegionList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetRegionList]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetRegionList]  
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
		Region.RegionId,
		Region.Name,
		Region.Acronym,
		Region.Corporate,
		Region.Status,
		Region.Test,
		Region.Displayname AS DisplayName,
		Region.ReportName,
		Region.Address,
		Region.Address2,
		Region.Address3,
		Region.City,
		Region.State,
		Region.PostalCode AS ZipCode,
		Region.Country,
		Region.Phone,
		Region.Phone1,
		Region.Phone2,
		Region.AhPhone,
		Region.Fax,
		Region.Email,
		Region.RemitSameAsMain,
		Region.International,
		Region.DBname,
		Region.LockboxId,
		Region.Number,
		Region.company_no AS CompanyNumber,
		Region.CreatedBy,
		Region.CreateDate AS CreatedDate,
		Region.ModifiedBy,
		Region.ModifiedDate
	FROM
		[dbo].[Region] Region WITH (NOLOCK)
	WHERE
		Region.Status = @IsEnable
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN Region.RegionId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN Region.RegionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Name' AND LOWER(@SortOrder)='asc') THEN Region.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Name' AND LOWER(@SortOrder)='desc') THEN Region.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Acronym' AND LOWER(@SortOrder)='asc') THEN Region.Acronym END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Acronym' AND LOWER(@SortOrder)='desc') THEN Region.Acronym END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Corporate' AND LOWER(@SortOrder)='asc') THEN Region.Corporate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Corporate' AND LOWER(@SortOrder)='desc') THEN Region.Corporate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Status' AND LOWER(@SortOrder)='asc') THEN Region.Status END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Status' AND LOWER(@SortOrder)='desc') THEN Region.Status END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Test' AND LOWER(@SortOrder)='asc') THEN Region.Test END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Test' AND LOWER(@SortOrder)='desc') THEN Region.Test END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DisplayName' AND LOWER(@SortOrder)='asc') THEN Region.Displayname END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DisplayName' AND LOWER(@SortOrder)='desc') THEN Region.Displayname END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ReportName' AND LOWER(@SortOrder)='asc') THEN Region.ReportName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ReportName' AND LOWER(@SortOrder)='desc') THEN Region.ReportName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address' AND LOWER(@SortOrder)='asc') THEN Region.Address END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address' AND LOWER(@SortOrder)='desc') THEN Region.Address END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address2' AND LOWER(@SortOrder)='asc') THEN Region.Address2 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address2' AND LOWER(@SortOrder)='desc') THEN Region.Address2 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address3' AND LOWER(@SortOrder)='asc') THEN Region.Address3 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address3' AND LOWER(@SortOrder)='desc') THEN Region.Address3 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'City' AND LOWER(@SortOrder)='asc') THEN Region.City END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'City' AND LOWER(@SortOrder)='desc') THEN Region.City END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'State' AND LOWER(@SortOrder)='asc') THEN Region.State END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'State' AND LOWER(@SortOrder)='desc') THEN Region.State END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ZipCode' AND LOWER(@SortOrder)='asc') THEN Region.PostalCode END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ZipCode' AND LOWER(@SortOrder)='desc') THEN Region.PostalCode END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Country' AND LOWER(@SortOrder)='asc') THEN Region.Country END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Country' AND LOWER(@SortOrder)='desc') THEN Region.Country END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone' AND LOWER(@SortOrder)='asc') THEN Region.Phone END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone' AND LOWER(@SortOrder)='desc') THEN Region.Phone END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone1' AND LOWER(@SortOrder)='asc') THEN Region.Phone1 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone1' AND LOWER(@SortOrder)='desc') THEN Region.Phone1 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone2' AND LOWER(@SortOrder)='asc') THEN Region.Phone2 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Phone2' AND LOWER(@SortOrder)='desc') THEN Region.Phone2 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AhPhone' AND LOWER(@SortOrder)='asc') THEN Region.AhPhone END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AhPhone' AND LOWER(@SortOrder)='desc') THEN Region.AhPhone END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Fax' AND LOWER(@SortOrder)='asc') THEN Region.Fax END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Fax' AND LOWER(@SortOrder)='desc') THEN Region.Fax END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Email' AND LOWER(@SortOrder)='asc') THEN Region.Email END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Email' AND LOWER(@SortOrder)='desc') THEN Region.Email END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RemitSameAsMain' AND LOWER(@SortOrder)='asc') THEN Region.RemitSameAsMain END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RemitSameAsMain' AND LOWER(@SortOrder)='desc') THEN Region.RemitSameAsMain END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'International' AND LOWER(@SortOrder)='asc') THEN Region.International END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'International' AND LOWER(@SortOrder)='desc') THEN Region.International END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DBname' AND LOWER(@SortOrder)='asc') THEN Region.DBname END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DBname' AND LOWER(@SortOrder)='desc') THEN Region.DBname END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LockboxId' AND LOWER(@SortOrder)='asc') THEN Region.LockboxId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LockboxId' AND LOWER(@SortOrder)='desc') THEN Region.LockboxId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Number' AND LOWER(@SortOrder)='asc') THEN Region.Number END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Number' AND LOWER(@SortOrder)='desc') THEN Region.Number END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CompanyNumber' AND LOWER(@SortOrder)='asc') THEN Region.company_no END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CompanyNumber' AND LOWER(@SortOrder)='desc') THEN Region.company_no END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN Region.CreatedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN Region.CreatedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN Region.CreateDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN Region.CreateDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN Region.ModifiedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN Region.ModifiedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN Region.ModifiedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN Region.ModifiedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN Region.RegionId END ASC
		
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
