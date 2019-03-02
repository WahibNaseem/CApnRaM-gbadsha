IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetNearbyLeadListByRegion]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetNearbyLeadListByRegion]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetNearbyLeadListByRegion]
(
	@RegionId INT = 0,
	@Latitude DECIMAL(18,8) = 0,
	@Longitude DECIMAL(18,8) = 0,
	@Distance DECIMAL(18,2) = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		CRMCustomer.CRM_AccountCustomerDetailId AS AccountCustomerDetailId,
		CRMCustomer.CRM_AccountId AS AccountId,
		CRMCustomer.CompanyName,
		CRMCustomer.CompanyEmailAddress,
		CRMCustomer.CompanyPhoneNumber,
		CRMCustomer.CompanyFaxNumber,
		CRMCustomer.NumberOfEmployees,
		CRMCustomer.NumberOfLocations,
		CRMCustomer.BudgetAmount,
		CRMCustomer.AccountTypeListId,
		CRMCustomer.GeneralNote,
		CRMCustomer.CompanyWebsite,
		CRMCustomer.Callback,
		CRMCustomer.AMOrPM,
		CRMCustomer.Ext,
		CRMCustomer.Title,
		CRMCustomer.SicCode,
		CRMCustomer.SalesVolume,
		CRMCustomer.LineofBusiness,
		CRMCustomer.SqFt AS SquareFoot,
		CRMCustomer.StartDate,
		CRMCustomer.Purpose,
		CRMCustomer.RegionId,
		CRMCustomer.Attmpt_count AS AttemptCount,
		CRMCustomer.lst_attmpt AS LastAttempt,
		CRMCustomer.cb_ampm AS ContactByAmOrPm,
		CRMCustomer.cb_time AS ContactTime,
		CRMCustomer.contextprice AS ContextPrice,
		CRMCustomer.lastcontact AS LastContact,
		CRMCustomer.status AS Status,
		CRMCustomer.callbackby AS CallbackBy,
		CRMCustomer.callbackdate AS CallbackDate,
		CRMCustomer.lastmaildate AS LastMailDate,
		CRMCustomer.contractterm AS ContractTerm,
		CRMCustomer.solddate AS SoldDate,
		CRMCustomer.accttype AS AccountType,
		CRMCustomer.CRM_CallResultId AS CallResultId,
		CRMCustomer.SpokeWith,
		CRMCustomer.CRM_NoteTypeId AS NoteTypeId,
		CRMCustomer.CRM_SalePossibilityTypeId AS SalePossibilityTypeId,
		CRMCustomer.ContractExpire,
		CRMCustomer.TerritoryId,
		CRMCustomer.SicDescription,
		CRMCustomer.AssigneeId,
		CRMCustomer.StageStatus,
		CRMCustomer.Stage,
		CRMCustomer.ProviderSource,
		CRMCustomer.ProviderType,
		CRMCustomer.ContactPhoneNumber,
		CRMCustomer.ContactEmail,
		CRMCustomer.ContactName,
		CRMCustomer.CreatedBy,
		CRMCustomer.CreatedDate,
		CRMCustomer.ModifiedBy,
		CRMCustomer.ModifiedDate,
		CRMCustomer.CompanyAddressLine1 AS Address1,
		CRMCustomer.CompanyAddressLine2 AS Address2,
		CRMCustomer.CompanyAddressLine3 AS Address3,
		CRMCustomer.CompanyCity AS City,
		CRMCustomer.CompanyCounty AS County,
		CRMCustomer.CompanyState AS State,
		CRMCustomer.CompanyZipCode AS ZipCode,
		CRMCustomer.CompanyLatitude AS Latitude,
		CRMCustomer.CompanyLongitude AS Longitude
	FROM
		[dbo].[CRM_AccountCustomerDetail] CRMCustomer WITH (NOLOCK)
	WHERE
		CRMCustomer.RegionId = @RegionId AND
		dbo.CalcDistanceBetweenLocations(@Latitude, @Longitude, CRMCustomer.CompanyLatitude, CRMCustomer.CompanyLongitude, 0) <= @Distance
	ORDER BY
		dbo.CalcDistanceBetweenLocations(@Latitude, @Longitude, CRMCustomer.CompanyLatitude, CRMCustomer.CompanyLongitude, 0) ASC

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