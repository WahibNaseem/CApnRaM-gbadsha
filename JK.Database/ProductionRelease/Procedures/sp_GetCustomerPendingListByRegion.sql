IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetCustomerPendingListByRegion]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[sp_GetCustomerPendingListByRegion]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetCustomerPendingListByRegion]
(
  @RegionId INT = 0,
  @PageNo INT = 1,
  @PageSize INT = 10,
  @SortColumn NVARCHAR(20) = '',
  @SortOrder NVARCHAR(4) ='asc',
  @SearchText NVARCHAR(200) = ''
)
AS

BEGIN TRANSACTION;

BEGIN TRY

  SET NOCOUNT ON;

  IF @PageNo <= 1 SET @PageNo = 1
  IF @PageSize <= 0 SET @PageSize = 10

  CREATE TABLE #Temp_Table_CustomersByRegionId
  (
	RegionId INT NOT NULL,
    CustomerId INT NOT NULL,
    CustomerNumber NVARCHAR(25) NULL,
    CustomerName NVARCHAR(125) NULL,
    ContractId INT NULL,
    AddressId INT NULL,
    SoldById INT NULL,
    PurchaseOrderNo NVARCHAR(20) NULL,
    ContractTypeListId INT NULL,
    AccountTypeListId INT NULL,
    StartDate DATETIME NULL,
    ExpirationDate DATETIME NULL,
    ContractDescription VARCHAR(MAX) NULL,
    ContractTermMonth INT NULL,
    Amount DECIMAL(18,2) NULL
  )

  -- Insert data into the first table
  INSERT INTO #Temp_Table_CustomersByRegionId
    SELECT
      Customer.RegionId,
      Customer.CustomerId,
      Customer.CustomerNo,
      Customer.Name,
      Contract.ContractId,
      Contract.AddressId,
      Contract.SoldById,
      Contract.PurchaseOrderNo,
      Contract.ContractTypeListId,
      Contract.AccountTypeListId,
      Contract.StartDate,
      Contract.ExpirationDate,
      Contract.ContractDescription,
      Contract.ContractTermMonth,
      Contract.Amount
    FROM
      [dbo].[Contract] Contract WITH (NOLOCK),
      [dbo].[Customer] Customer WITH (NOLOCK)
    WHERE
	  Customer.RegionId = @RegionId AND
      Contract.CustomerId = Customer.CustomerId AND
      Contract.IsActive = 1 AND
      Customer.StatusListId = 38
    ORDER BY
      Customer.CustomerId

  SELECT
    ResultTable.RegionId,
    Distribution.FranchiseeId,
    ResultTable.CustomerId,
    ResultTable.CustomerNumber,
    ResultTable.CustomerName,
    ResultTable.ContractId,	
    ContractDetail.ContractDetailId,
    ResultTable.SoldById,
    ResultTable.PurchaseOrderNo AS PurchaseOrderNumber,
    ResultTable.ContractTypeListId,
    ContractTypeList.Name AS ContractType,
    ServiceTypeList.ServiceTypeListId,
    ServiceTypeList.Name AS ServiceType,
    ResultTable.AccountTypeListId,
    AccountTypeList.Name AS AccountType,
    ResultTable.StartDate,
    ResultTable.ExpirationDate,
    ResultTable.ContractDescription,
    ResultTable.ContractTermMonth,
    ResultTable.Amount,
    TRY_PARSE(ContractDetail.SquareFootage AS DECIMAL(18,2) USING 'en-US') AS SquareFootage,
    ContractDetail.CleanTimes,
    ContractDetail.Mon,
    ContractDetail.Tues AS Tue,
    ContractDetail.Wed,
    ContractDetail.Thur AS Thu,
    ContractDetail.Fri,
    ContractDetail.Sat,
    ContractDetail.Sun,
    ContractDetail.StartTime,
    ContractDetail.EndTime,
    Contact.Name AS PrimaryContact,
    Phone.Phone AS PrimaryContactPhone,
    Phone.PhoneExt AS PrimaryContactPhoneExt,
    Address.AddressId,
    Address.Address1,
    Address.Address2,
    Address.City,
    Address.StateName AS State,
    Address.PostalCode AS ZipCode,
    Address.Latitude,
    Address.Longitude
  FROM
    #Temp_Table_CustomersByRegionId ResultTable WITH (NOLOCK)
  LEFT JOIN [dbo].[Distribution] Distribution WITH (NOLOCK)  ON Distribution.ContractId = ResultTable.ContractId AND Distribution.IsActive = 1
  LEFT JOIN [dbo].[ContractDetail] ContractDetail WITH (NOLOCK)  ON ContractDetail.ContractId = ResultTable.ContractId AND ContractDetail.IsActive = 1
  LEFT JOIN [dbo].[Contact] Contact WITH (NOLOCK)  ON Contact.ClassId = ResultTable.CustomerId AND Contact.TypeListId = 1 AND Contact.ContactTypeListId = 6 AND Contact.IsActive = 1
  LEFT JOIN [dbo].[Phone] Phone WITH (NOLOCK) ON Phone.ClassId = ResultTable.CustomerId AND Phone.TypeListId = 1 AND Phone.ContactTypeListId = 1 AND Phone.IsActive = 1
  LEFT JOIN [dbo].[Email] Email WITH (NOLOCK) ON Email.ClassId = ResultTable.CustomerId AND Email.TypeListId = 1 AND Email.ContactTypeListId = 1 AND Email.IsActive = 1
  LEFT JOIN [dbo].[ContractTypeList] ContractTypeList WITH (NOLOCK) ON ContractTypeList.ContractTypeListId = ResultTable.ContractTypeListId
  LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON AccountTypeList.AccountTypeListId = ResultTable.AccountTypeListId
  LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON ServiceTypeList.ServiceTypeListId = ContractDetail.ServiceTypeListId
  LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = ResultTable.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 1 AND Address.IsActive = 1 
  ORDER BY
    CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='asc') THEN ResultTable.RegionId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'RegionId' AND LOWER(@SortOrder)='desc') THEN ResultTable.RegionId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='asc') THEN Distribution.FranchiseeId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'FranchiseeId' AND LOWER(@SortOrder)='desc') THEN Distribution.FranchiseeId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='asc') THEN ResultTable.CustomerId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerId' AND LOWER(@SortOrder)='desc') THEN ResultTable.CustomerId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='asc') THEN ResultTable.CustomerNumber END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerNumber' AND LOWER(@SortOrder)='desc') THEN ResultTable.CustomerNumber END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='asc') THEN ResultTable.CustomerName END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CustomerName' AND LOWER(@SortOrder)='desc') THEN ResultTable.CustomerName END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractId' AND LOWER(@SortOrder)='asc') THEN ResultTable.ContractId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractId' AND LOWER(@SortOrder)='desc') THEN ResultTable.ContractId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'SoldById' AND LOWER(@SortOrder)='asc') THEN ResultTable.SoldById END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'SoldById' AND LOWER(@SortOrder)='desc') THEN ResultTable.SoldById END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PurchaseOrderNumber' AND LOWER(@SortOrder)='asc') THEN ResultTable.PurchaseOrderNo END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PurchaseOrderNumber' AND LOWER(@SortOrder)='desc') THEN ResultTable.PurchaseOrderNo END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTypeListId' AND LOWER(@SortOrder)='asc') THEN ResultTable.ContractTypeListId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTypeListId' AND LOWER(@SortOrder)='desc') THEN ResultTable.ContractTypeListId END DESC,
	CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='asc') THEN ContractDetail.ServiceTypeListId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceTypeListId' AND LOWER(@SortOrder)='desc') THEN ContractDetail.ServiceTypeListId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='asc') THEN ResultTable.AccountTypeListId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'AccountTypeListId' AND LOWER(@SortOrder)='desc') THEN ResultTable.AccountTypeListId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractType' AND LOWER(@SortOrder)='asc') THEN ContractTypeList.Name END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractType' AND LOWER(@SortOrder)='desc') THEN ContractTypeList.Name END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='asc') THEN AccountTypeList.Name END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'AccountType' AND LOWER(@SortOrder)='desc') THEN AccountTypeList.Name END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'StartDate' AND LOWER(@SortOrder)='asc') THEN ResultTable.StartDate END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'StartDate' AND LOWER(@SortOrder)='desc') THEN ResultTable.StartDate END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ExpirationDate' AND LOWER(@SortOrder)='asc') THEN ResultTable.ExpirationDate END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ExpirationDate' AND LOWER(@SortOrder)='desc') THEN ResultTable.ExpirationDate END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDescription' AND LOWER(@SortOrder)='asc') THEN ResultTable.ContractDescription END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDescription' AND LOWER(@SortOrder)='desc') THEN ResultTable.ContractDescription END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTermMonth' AND LOWER(@SortOrder)='asc') THEN ResultTable.ContractTermMonth END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractTermMonth' AND LOWER(@SortOrder)='desc') THEN ResultTable.ContractTermMonth END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Amount' AND LOWER(@SortOrder)='asc') THEN ResultTable.Amount END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Amount' AND LOWER(@SortOrder)='desc') THEN ResultTable.Amount END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDetailId' AND LOWER(@SortOrder)='asc') THEN ContractDetail.ContractDetailId END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ContractDetailId' AND LOWER(@SortOrder)='desc') THEN ContractDetail.ContractDetailId END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='asc') THEN ServiceTypeList.Name END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'ServiceType' AND LOWER(@SortOrder)='desc') THEN ServiceTypeList.Name END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'SquareFootage' AND LOWER(@SortOrder)='asc') THEN ContractDetail.SquareFootage END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'SquareFootage' AND LOWER(@SortOrder)='desc') THEN ContractDetail.SquareFootage END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CleanTimes' AND LOWER(@SortOrder)='asc') THEN ContractDetail.CleanTimes END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'CleanTimes' AND LOWER(@SortOrder)='desc') THEN ContractDetail.CleanTimes END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Mon' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Mon END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Mon' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Mon END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Tue' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Tues END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Tue' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Tues END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Wed' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Wed END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Wed' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Wed END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Thu' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Thur END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Thu' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Thur END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Fri' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Fri END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Fri' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Fri END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Sat' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Sat END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Sat' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Sat END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Sun' AND LOWER(@SortOrder)='asc') THEN ContractDetail.Sun END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'Sun' AND LOWER(@SortOrder)='desc') THEN ContractDetail.Sun END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'StartTime' AND LOWER(@SortOrder)='asc') THEN ContractDetail.StartTime END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'StartTime' AND LOWER(@SortOrder)='desc') THEN ContractDetail.StartTime END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'EndTime' AND LOWER(@SortOrder)='asc') THEN ContractDetail.EndTime END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'EndTime' AND LOWER(@SortOrder)='desc') THEN ContractDetail.EndTime END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='asc') THEN Contact.Name END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContact' AND LOWER(@SortOrder)='desc') THEN Contact.Name END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='asc') THEN Phone.Phone END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhone' AND LOWER(@SortOrder)='desc') THEN Phone.Phone END DESC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='asc') THEN Phone.PhoneExt END ASC,
    CASE WHEN (ISNULL(@SortColumn,'') = 'PrimaryContactPhoneExt' AND LOWER(@SortOrder)='desc') THEN Phone.PhoneExt END DESC,
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
    CASE WHEN (ISNULL(@SortColumn,'') = '') THEN ResultTable.RegionId END ASC
  OFFSET (@PageNo-1) * @PageSize ROWS
  FETCH NEXT @PageSize ROWS ONLY

  -- Remove temp tables
  DROP TABLE #Temp_Table_CustomersByRegionId

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