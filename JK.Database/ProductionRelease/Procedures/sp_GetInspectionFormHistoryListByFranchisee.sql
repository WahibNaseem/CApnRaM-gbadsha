IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormHistoryListByFranchisee]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormHistoryListByFranchisee]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormHistoryListByFranchisee]      
(
	@FranchiseeId INT = 0,
	@IsEnable BIT = 1,  
	@PageNo INT = 1,  
	@PageSize INT = 1,  
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'  
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

	IF @PageNo <= 1 SET @PageNo = 1
	IF @PageSize <= 0 SET @PageSize = 10
    
	SELECT
		InspectionFormHistory.InspectionFormHistoryId AS InspectionFormId,
		InspectionFormHistory.JobId,
		InspectionFormHistory.CustomerId,
		InspectionFormHistory.RegionId,
		InspectionFormHistory.FranchiseeId,
		Job.CustomerName,
		Customer.CustomerNo AS CustomerNumber,
		InspectionFormHistory.ServiceTypeListId,
		ServiceTypeList.name AS ServiceType,
		InspectionFormHistory.AccountTypeListId,
		AccountTypeList.Name AS AccountType,
		InspectionFormHistory.InspectionStatusId,
		Job.PrimaryContact,
		Job.PrimaryContactPhone,
		Job.PrimaryContactPhoneExt,
		Job.CleanTimes,
		Job.Mon,
		Job.Tue,
		Job.Wed,
		Job.Thu,
		Job.Fri,
		Job.Sat,
		Job.Sun,
		InspectionFormHistory.CallDate,
		InspectionFormHistory.RecordedDate,
		InspectionFormHistory.UploadedDate,
		InspectionFormHistory.IsCompleted,
		InspectionFormHistory.InspectedBy,
		InspectionFormHistory.InspectorId,
		InspectionFormHistory.FormName,
		InspectionFormHistory.Description,
		InspectionFormHistory.PassPoints,
		InspectionFormHistory.FailPoints,
		InspectionFormHistory.NeedImprovementPoints,
		InspectionFormHistory.ScorePercent,
		InspectionFormHistory.SignatureUrl,
		InspectionFormHistory.IsEnable,
		InspectionFormHistory.IsDelete,
		InspectionFormHistory.CreatedBy,
		InspectionFormHistory.CreatedDate,
		InspectionFormHistory.ModifiedBy,
		InspectionFormHistory.ModifiedDate,
		Address.AddressId,  
		Address.Address1,  
		Address.Address2,  
		Address.City,  
		Address.StateName AS State,    
		Address.PostalCode AS ZipCode,  
		Address.Latitude,  
		Address.Longitude, 
		ISNULL(COUNT(*) Over(), 0) AS TotalRecords
	FROM
		[dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK)
	LEFT JOIN [dbo].[Job] Job WITH (NOLOCK) ON Job.JobId = InspectionFormHistory.JobId
	LEFT JOIN [dbo].[Customer] Customer WITH (NOLOCK) ON Customer.CustomerId = InspectionFormHistory.CustomerId
	LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON InspectionFormHistory.AccountTypeListId = AccountTypeList.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON InspectionFormHistory.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
	LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = Job.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address
	WHERE
		InspectionFormHistory.FranchiseeId = @FranchiseeId AND
		InspectionFormHistory.IsEnable = @IsEnable
	ORDER BY
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.InspectionFormHistoryId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.InspectionFormHistoryId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.JobId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.JobId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.CustomerId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.CustomerId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.RegionId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.RegionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.FranchiseeId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.FranchiseeId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='asc') THEN Job.CustomerName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='desc') THEN Job.CustomerName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='asc') THEN Customer.CustomerNo END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='desc') THEN Customer.CustomerNo END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.ServiceTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.ServiceTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='asc') THEN ServiceTypeList.name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='desc') THEN ServiceTypeList.name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.AccountTypeListId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.AccountTypeListId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='asc') THEN AccountTypeList.Name END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='desc') THEN AccountTypeList.Name END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionStatusId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.InspectionStatusId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionStatusId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.InspectionStatusId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContact END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContact END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContactPhone END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContactPhone END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='asc') THEN Job.PrimaryContactPhoneExt END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='desc') THEN Job.PrimaryContactPhoneExt END DESC,  
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
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.CallDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.CallDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RecordedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.RecordedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'RecordedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.RecordedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'UploadedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.UploadedDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'UploadedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.UploadedDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsCompleted' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.IsCompleted END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsCompleted' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.IsCompleted END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectedBy' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.InspectedBy END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectedBy' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.InspectedBy END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectorId' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.InspectorId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectorId' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.InspectorId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.FormName END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.FormName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.Description END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.Description END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.PassPoints END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.PassPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.FailPoints END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.FailPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.NeedImprovementPoints END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.NeedImprovementPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.ScorePercent END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.ScorePercent END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SignatureUrl' AND LOWER(@SortOrder)='asc') THEN InspectionFormHistory.SignatureUrl END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SignatureUrl' AND LOWER(@SortOrder)='desc') THEN InspectionFormHistory.SignatureUrl END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AddressId' AND LOWER(@SortOrder)='asc') THEN Address.AddressId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'AddressId' AND LOWER(@SortOrder)='desc') THEN Address.AddressId END DESC,  
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
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionFormHistory.InspectionFormHistoryId END ASC
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