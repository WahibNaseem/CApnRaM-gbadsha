IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormListByFranchisee]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
    DROP PROCEDURE [dbo].[sp_GetInspectionFormListByFranchisee]
GO  

CREATE PROCEDURE [dbo].[sp_GetInspectionFormListByFranchisee]      
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
        InspectionForm.InspectionFormId,
        InspectionForm.JobId,
        InspectionForm.CustomerId,
        InspectionForm.RegionId,
        InspectionForm.FranchiseeId,
        Job.CustomerName,
        Customer.CustomerNo AS CustomerNumber,
        InspectionForm.ServiceTypeListId,
        ServiceTypeList.name AS ServiceType,
        InspectionForm.AccountTypeListId,
        AccountTypeList.Name AS AccountType,
        InspectionForm.InspectionStatusId,
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
        InspectionForm.CallDate,
        InspectionForm.RecordedDate,
        InspectionForm.UploadedDate,
        InspectionForm.IsCompleted,
        InspectionForm.InspectedBy,
        InspectionForm.InspectorId,
        InspectionForm.FormName,
        InspectionForm.Description,
        InspectionForm.PassPoints,
        InspectionForm.FailPoints,
        InspectionForm.NeedImprovementPoints,
        InspectionForm.ScorePercent,
        InspectionForm.SignatureUrl,
        InspectionForm.IsEnable,
        InspectionForm.IsDelete,
        InspectionForm.CreatedBy,
        InspectionForm.CreatedDate,
        InspectionForm.ModifiedBy,
        InspectionForm.ModifiedDate,
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
        [dbo].[InspectionForm] InspectionForm WITH (NOLOCK)
    LEFT JOIN [dbo].[Job] Job WITH (NOLOCK) ON Job.JobId = InspectionForm.JobId
    LEFT JOIN [dbo].[Customer] Customer WITH (NOLOCK) ON Customer.CustomerId = InspectionForm.CustomerId
    LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON InspectionForm.AccountTypeListId = AccountTypeList.AccountTypeListId
    LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON InspectionForm.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
    LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = Job.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address
    WHERE
        InspectionForm.FranchiseeId = @FranchiseeId AND
        InspectionForm.IsEnable = @IsEnable
    ORDER BY
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.InspectionFormId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.InspectionFormId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.JobId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'JobId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.JobId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.CustomerId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.CustomerId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.RegionId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.RegionId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.FranchiseeId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.FranchiseeId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='asc') THEN Job.CustomerName END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='desc') THEN Job.CustomerName END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='asc') THEN Customer.CustomerNo END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='desc') THEN Customer.CustomerNo END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.ServiceTypeListId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.ServiceTypeListId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='asc') THEN ServiceTypeList.name END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='desc') THEN ServiceTypeList.name END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.AccountTypeListId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.AccountTypeListId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='asc') THEN AccountTypeList.Name END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='desc') THEN AccountTypeList.Name END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionStatusId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.InspectionStatusId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionStatusId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.InspectionStatusId END DESC,
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
        CASE WHEN (ISNULL(@SortColumn,'') = 'CallDate' AND LOWER(@SortOrder)='asc') THEN InspectionForm.CallDate END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'CallDate' AND LOWER(@SortOrder)='desc') THEN InspectionForm.CallDate END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'RecordedDate' AND LOWER(@SortOrder)='asc') THEN InspectionForm.RecordedDate END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'RecordedDate' AND LOWER(@SortOrder)='desc') THEN InspectionForm.RecordedDate END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'UploadedDate' AND LOWER(@SortOrder)='asc') THEN InspectionForm.UploadedDate END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'UploadedDate' AND LOWER(@SortOrder)='desc') THEN InspectionForm.UploadedDate END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'IsCompleted' AND LOWER(@SortOrder)='asc') THEN InspectionForm.IsCompleted END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'IsCompleted' AND LOWER(@SortOrder)='desc') THEN InspectionForm.IsCompleted END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectedBy' AND LOWER(@SortOrder)='asc') THEN InspectionForm.InspectedBy END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectedBy' AND LOWER(@SortOrder)='desc') THEN InspectionForm.InspectedBy END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectorId' AND LOWER(@SortOrder)='asc') THEN InspectionForm.InspectorId END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'InspectorId' AND LOWER(@SortOrder)='desc') THEN InspectionForm.InspectorId END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='asc') THEN InspectionForm.FormName END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FormName' AND LOWER(@SortOrder)='desc') THEN InspectionForm.FormName END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='asc') THEN InspectionForm.Description END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'Description' AND LOWER(@SortOrder)='desc') THEN InspectionForm.Description END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='asc') THEN InspectionForm.PassPoints END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='desc') THEN InspectionForm.PassPoints END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='asc') THEN InspectionForm.FailPoints END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='desc') THEN InspectionForm.FailPoints END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='asc') THEN InspectionForm.NeedImprovementPoints END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='desc') THEN InspectionForm.NeedImprovementPoints END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='asc') THEN InspectionForm.ScorePercent END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='desc') THEN InspectionForm.ScorePercent END DESC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'SignatureUrl' AND LOWER(@SortOrder)='asc') THEN InspectionForm.SignatureUrl END ASC,
        CASE WHEN (ISNULL(@SortColumn,'') = 'SignatureUrl' AND LOWER(@SortOrder)='desc') THEN InspectionForm.SignatureUrl END DESC,
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
        CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionForm.InspectionFormId END ASC
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