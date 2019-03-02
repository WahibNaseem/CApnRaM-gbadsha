IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetJobList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetJobList]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetJobList]  
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
		Job.JobId,
		Job.DistributionId,
		Job.RegionId,
		Job.FranchiseeId,
		Job.ContractId,
		Job.ContractDetailId,
		Job.CustomerId,
		Job.SoldById,
		Job.PurchaseOrderNumber,
		Job.CustomerName,
		Job.PrimaryContact,
		Job.PrimaryContactPhone,
		Job.PrimaryContactPhoneExt,
		Job.ContractTypeListId,
		Job.AccountTypeListId,
		Job.ServiceTypeListId,
		Job.StartDate,
		Job.ExpirationDate,
		Job.Amount,
		Job.ContractDescription,
		Job.ContractTermMonth,
		Job.SquareFootage,
		Job.CleanTimes,
		Job.Mon,
		Job.Tue,
		Job.Wed,
		Job.Thu,
		Job.Fri,
		Job.Sat,
		Job.Sun,
		Job.StartTime,
		Job.EndTime,
		Job.AssignedTeamId,
		Job.AssignedEmployeeId,
		Job.AssignedTemplate,
		Job.IsEnable,
		Job.CreatedBy,
		Job.CreatedDate,
		Job.ModifiedBy,
		Job.ModifiedDate,
		Customer.CustomerNo AS CustomerNumber,
		ContractTypeList.Name AS ContractType,
		AccountTypeList.Name AS AccountType,
		ServiceTypeList.name AS ServiceType,
		Address.AddressId,
		Address.Address1,
		Address.Address2,
		Address.City,
		Address.StateName AS State,
		Address.PostalCode AS ZipCode,
		Address.Latitude,
		Address.Longitude
	FROM
		[dbo].[Job] Job WITH (NOLOCK)
	LEFT JOIN [dbo].[Customer] Customer WITH (NOLOCK) ON Customer.CustomerId = Job.CustomerId
	LEFT JOIN [dbo].[ContractTypeList] ContractTypeList WITH (NOLOCK) ON ContractTypeList.ContractTypeListId = Job.ContractTypeListId
	LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON AccountTypeList.AccountTypeListId = Job.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON ServiceTypeList.ServiceTypeListid = Job.ServiceTypeListId
	LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = Job.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address
	WHERE
		Job.IsEnable = @IsEnable
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='asc') THEN Job.JobId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='desc') THEN Job.JobId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DistributionId' AND LOWER(@SortOrder)='asc') THEN Job.DistributionId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'DistributionId' AND LOWER(@SortOrder)='desc') THEN Job.DistributionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN Job.RegionId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN Job.RegionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='asc') THEN Job.FranchiseeId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='desc') THEN Job.FranchiseeId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractId' AND LOWER(@SortOrder)='asc') THEN Job.ContractId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractId' AND LOWER(@SortOrder)='desc') THEN Job.ContractId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDetailId' AND LOWER(@SortOrder)='asc') THEN Job.ContractDetailId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDetailId' AND LOWER(@SortOrder)='desc') THEN Job.ContractDetailId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='asc') THEN Job.CustomerId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='desc') THEN Job.CustomerId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SoldById' AND LOWER(@SortOrder)='asc') THEN Job.SoldById END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SoldById' AND LOWER(@SortOrder)='desc') THEN Job.SoldById END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PurchaseOrderNumber' AND LOWER(@SortOrder)='asc') THEN Job.PurchaseOrderNumber END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PurchaseOrderNumber' AND LOWER(@SortOrder)='desc') THEN Job.PurchaseOrderNumber END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='asc') THEN Job.CustomerName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='desc') THEN Job.CustomerName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContact END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContact END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContactPhone END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContactPhone END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContactPhoneExt END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContactPhoneExt END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractType' AND LOWER(@SortOrder)='asc') THEN Job.ContractTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractType' AND LOWER(@SortOrder)='desc') THEN Job.ContractTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='asc') THEN Job.AccountTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='desc') THEN Job.AccountTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='asc') THEN Job.ServiceTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='desc') THEN Job.ServiceTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StartDate' AND LOWER(@SortOrder)='asc') THEN Job.StartDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StartDate' AND LOWER(@SortOrder)='desc') THEN Job.StartDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ExpirationDate' AND LOWER(@SortOrder)='asc') THEN Job.ExpirationDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ExpirationDate' AND LOWER(@SortOrder)='desc') THEN Job.ExpirationDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Amount' AND LOWER(@SortOrder)='asc') THEN Job.Amount END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Amount' AND LOWER(@SortOrder)='desc') THEN Job.Amount END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDescription' AND LOWER(@SortOrder)='asc') THEN Job.ContractDescription END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDescription' AND LOWER(@SortOrder)='desc') THEN Job.ContractDescription END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTermMonth' AND LOWER(@SortOrder)='asc') THEN Job.ContractTermMonth END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTermMonth' AND LOWER(@SortOrder)='desc') THEN Job.ContractTermMonth END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SquareFootage' AND LOWER(@SortOrder)='asc') THEN Job.SquareFootage END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SquareFootage' AND LOWER(@SortOrder)='desc') THEN Job.SquareFootage END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CleanTimes' AND LOWER(@SortOrder)='asc') THEN Job.CleanTimes END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CleanTimes' AND LOWER(@SortOrder)='desc') THEN Job.CleanTimes END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Mon' AND LOWER(@SortOrder)='asc') THEN Job.Mon END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Mon' AND LOWER(@SortOrder)='desc') THEN Job.Mon END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Tue' AND LOWER(@SortOrder)='asc') THEN Job.Tue END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Tue' AND LOWER(@SortOrder)='desc') THEN Job.Tue END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Wed' AND LOWER(@SortOrder)='asc') THEN Job.Wed END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Wed' AND LOWER(@SortOrder)='desc') THEN Job.Wed END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Thu' AND LOWER(@SortOrder)='asc') THEN Job.Thu END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Thu' AND LOWER(@SortOrder)='desc') THEN Job.Thu END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Fri' AND LOWER(@SortOrder)='asc') THEN Job.Fri END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Fri' AND LOWER(@SortOrder)='desc') THEN Job.Fri END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Sat' AND LOWER(@SortOrder)='asc') THEN Job.Sat END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Sat' AND LOWER(@SortOrder)='desc') THEN Job.Sat END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Sun' AND LOWER(@SortOrder)='asc') THEN Job.Sun END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Sun' AND LOWER(@SortOrder)='desc') THEN Job.Sun END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StartTime' AND LOWER(@SortOrder)='asc') THEN Job.StartTime END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StartTime' AND LOWER(@SortOrder)='desc') THEN Job.StartTime END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'EndTime' AND LOWER(@SortOrder)='asc') THEN Job.EndTime END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'EndTime' AND LOWER(@SortOrder)='desc') THEN Job.EndTime END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedTeamId' AND LOWER(@SortOrder)='asc') THEN Job.AssignedTeamId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedTeamId' AND LOWER(@SortOrder)='desc') THEN Job.AssignedTeamId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedEmployeeId' AND LOWER(@SortOrder)='asc') THEN Job.AssignedEmployeeId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedEmployeeId' AND LOWER(@SortOrder)='desc') THEN Job.AssignedEmployeeId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedTemplate' AND LOWER(@SortOrder)='asc') THEN Job.AssignedTemplate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AssignedTemplate' AND LOWER(@SortOrder)='desc') THEN Job.AssignedTemplate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AddressId' AND LOWER(@SortOrder)='asc') THEN Job.AddressId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AddressId' AND LOWER(@SortOrder)='desc') THEN Job.AddressId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address1' AND LOWER(@SortOrder)='asc') THEN Address.Address1 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address1' AND LOWER(@SortOrder)='desc') THEN Address.Address1 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address2' AND LOWER(@SortOrder)='asc') THEN Address.Address2 END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Address2' AND LOWER(@SortOrder)='desc') THEN Address.Address2 END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'City' AND LOWER(@SortOrder)='asc') THEN Address.City END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'City' AND LOWER(@SortOrder)='desc') THEN Address.City END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'State' AND LOWER(@SortOrder)='asc') THEN Address.StateName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'State' AND LOWER(@SortOrder)='desc') THEN Address.StateName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ZipCode' AND LOWER(@SortOrder)='asc') THEN Address.PostalCode END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ZipCode' AND LOWER(@SortOrder)='desc') THEN Address.PostalCode END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Latitude' AND LOWER(@SortOrder)='asc') THEN Address.Latitude END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Latitude' AND LOWER(@SortOrder)='desc') THEN Address.Latitude END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Longitude' AND LOWER(@SortOrder)='asc') THEN Address.Longitude END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Longitude' AND LOWER(@SortOrder)='desc') THEN Address.Longitude END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN Job.JobId END ASC
		
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
