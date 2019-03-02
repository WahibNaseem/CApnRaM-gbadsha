IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetAllContracts]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[sp_GetAllContracts]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAllContracts]
AS

BEGIN TRANSACTION;

BEGIN TRY

  SET NOCOUNT ON;

  -- This table holds all the contracts for active customers
  CREATE TABLE #Temp_Table_Contracts
  (
    ContractId INT NOT NULL,
    RegionId INT,
    CustomerId INT NOT NULL,
    AddressId INT,
    SoldById INT,
    PurchaseOrderNo NVARCHAR(20) NULL,
    CustomerName NVARCHAR(125),
    ContractTypeListId INT,
    AccountTypeListId INT,
    StartDate DATETIME,
    ExpirationDate DATETIME,
    ContractDescription VARCHAR(MAX),
    ContractTermMonth INT,
    Amount DECIMAL(18,2),
  )

  INSERT INTO #Temp_Table_Contracts
    SELECT
      Contract.ContractId,
      Customer.RegionId,
      Customer.CustomerId,
      Contract.AddressId,
      Contract.SoldById,
      Contract.PurchaseOrderNo,
      Customer.Name,
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

    SELECT
      Distribution.DistributionId,
      ResultTable.RegionId,
      Distribution.FranchiseeId,
      ResultTable.ContractId,
      ContractDetail.ContractDetailId,
      ResultTable.CustomerId,
      ResultTable.SoldById,
      ResultTable.PurchaseOrderNo AS PurchaseOrderNumber,
      ResultTable.CustomerName,
      Contact.Name AS PrimaryContact,
      Phone.Phone AS PrimaryContactPhone,
      Phone.PhoneExt AS PrimaryContactPhoneExt,
      ResultTable.ContractTypeListId,
      ContractTypeList.Name AS ContractType,
      ResultTable.AccountTypeListId,
      AccountTypeList.Name AS AccountType,
      ContractDetail.ServiceTypeListId,
      ServiceTypeList.Name AS ServiceType,
      ResultTable.StartDate,
      ResultTable.ExpirationDate,
      ResultTable.ContractDescription,
      ResultTable.ContractTermMonth,
      ResultTable.Amount,
      TRY_PARSE(ContractDetail.SquareFootage AS DECIMAL(18,2) USING 'en-US') AS SquareFootage,
      ContractDetail.CleanTimes,
      ContractDetail.Mon,
      ContractDetail.Tues,
      ContractDetail.Wed,
      ContractDetail.Thur,
      ContractDetail.Fri,
      ContractDetail.Sat,
      ContractDetail.Sun,
      ContractDetail.StartTime,
      ContractDetail.EndTime,
	  ResultTable.AddressId,
      Address.Address1,
      Address.Address2,
      Address.City,
      Address.StateName AS State,
      Address.PostalCode AS ZipCode,
      Address.Latitude,
      Address.Longitude
    FROM
      #Temp_Table_Contracts ResultTable WITH (NOLOCK)
    LEFT JOIN [dbo].[Distribution] Distribution WITH (NOLOCK)  ON Distribution.ContractId = ResultTable.ContractId AND Distribution.IsActive = 1 -- Main active distribution
    LEFT JOIN [dbo].[ContractDetail] ContractDetail WITH (NOLOCK)  ON ContractDetail.ContractId = ResultTable.ContractId AND ContractDetail.IsActive = 1 -- Main active contract detail
    LEFT JOIN [dbo].[Contact] Contact WITH (NOLOCK)  ON Contact.ClassId = ResultTable.CustomerId AND Contact.TypeListId = 1 AND Contact.ContactTypeListId = 1 AND Contact.IsActive = 1 -- Main active contact
    LEFT JOIN [dbo].[Phone] Phone WITH (NOLOCK) ON Phone.ClassId = ResultTable.CustomerId AND Phone.TypeListId = 1 AND Phone.ContactTypeListId = 1 AND Phone.IsActive = 1 -- Main active phone
    LEFT JOIN [dbo].[ContractTypeList] ContractTypeList WITH (NOLOCK) ON ContractTypeList.ContractTypeListId = ResultTable.ContractTypeListId
    LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON AccountTypeList.AccountTypeListId = ResultTable.AccountTypeListId
    LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON ServiceTypeList.ServiceTypeListId = ContractDetail.ServiceTypeListId
    LEFT JOIN [dbo].[Address] Address WITH (NOLOCK) ON Address.ClassId = ResultTable.CustomerId AND Address.TypeListId = 1 AND Address.ContactTypeListId = 3 AND Address.IsActive = 1 -- Physical active address

  -- Remove the temp tables
  DROP TABLE #Temp_Table_Contracts

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