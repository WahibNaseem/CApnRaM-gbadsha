IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetJob]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetJob]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetJob]  
(
	@JobId INT = 0,
	@IsEnable BIT = 1
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
		Job.JobId = @JobId AND
		Job.IsEnable = @IsEnable
		
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
