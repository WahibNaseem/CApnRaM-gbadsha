IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetNearbyCustomerListByFranchisee]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[sp_GetNearbyCustomerListByFranchisee]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetNearbyCustomerListByFranchisee]
(
  @FranchiseeId INT = 0,
  @Latitude DECIMAL(18,8) = 0,
  @Longitude DECIMAL(18,8) = 0,
  @Distance DECIMAL(18,2) = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

  SET NOCOUNT ON;

  CREATE TABLE #Temp_Table_CustomersByFranchiseeId
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

  INSERT INTO #Temp_Table_CustomersByFranchiseeId
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
      Contract.CustomerId = Customer.CustomerId AND
      Contract.IsActive = 1 AND
      Customer.StatusListId = 1
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
    #Temp_Table_CustomersByFranchiseeId ResultTable WITH (NOLOCK)
  LEFT JOIN [dbo].[Distribution] Distribution WITH (NOLOCK)  ON Distribution.ContractId = ResultTable.ContractId AND Distribution.IsActive = 1
  LEFT JOIN [dbo].[ContractDetail] ContractDetail WITH (NOLOCK)  ON ContractDetail.ContractId = ResultTable.ContractId AND ContractDetail.IsActive = 1
  LEFT JOIN [dbo].[Contact] Contact WITH (NOLOCK)  ON Contact.ClassId = ResultTable.CustomerId AND Contact.TypeListId = 1 AND Contact.ContactTypeListId = 6 AND Contact.IsActive = 1
  LEFT JOIN [dbo].[Phone] Phone WITH (NOLOCK) ON Phone.ClassId = ResultTable.CustomerId AND Phone.TypeListId = 1 AND Phone.ContactTypeListId = 1 AND Phone.IsActive = 1
  LEFT JOIN [dbo].[Email] Email WITH (NOLOCK) ON Email.ClassId = ResultTable.CustomerId AND Email.TypeListId = 1 AND Email.ContactTypeListId = 1 AND Email.IsActive = 1
  LEFT JOIN [dbo].[ContractTypeList] ContractTypeList WITH (NOLOCK) ON ContractTypeList.ContractTypeListId = ResultTable.ContractTypeListId
  LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON AccountTypeList.AccountTypeListId = ResultTable.AccountTypeListId
  LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON ServiceTypeList.ServiceTypeListId = ContractDetail.ServiceTypeListId
  LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = ResultTable.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 1 AND Address.IsActive = 1 
  WHERE
  	Distribution.FranchiseeId = @FranchiseeId AND
    dbo.CalcDistanceBetweenLocations(@Latitude, @Longitude, Address.Latitude, Address.Longitude, 0) <= @Distance
  ORDER BY
	dbo.CalcDistanceBetweenLocations(@Latitude, @Longitude, Address.Latitude, Address.Longitude, 0) ASC

  -- Remove temp tables
  DROP TABLE #Temp_Table_CustomersByFranchiseeId

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