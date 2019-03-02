IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_AddOrUpdateJob]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[sp_AddOrUpdateJob]
GO 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AddOrUpdateJob]
(
  @JobId INT = 0,
  @DistributionId INT = NULL,
  @RegionId INT = NULL,
  @FranchiseeId INT = NULL,
  @ContractId INT = NULL,
  @ContractDetailId INT = NULL,
  @CustomerId INT = NULL,
  @AddressId INT = NULL,
  @SoldById INT = NULL,
  @PurchaseOrderNumber NVARCHAR(128) = NULL,
  @CustomerName NVARCHAR(128) = NULL,
  @PrimaryContact NVARCHAR(128) = NULL,
  @PrimaryContactPhone NVARCHAR(128) = NULL,
  @PrimaryContactPhoneExt NVARCHAR(128) = NULL,
  @ContractTypeListId INT = NULL,
  @AccountTypeListId INT = NULL,
  @ServiceTypeListId INT = NULL,
  @StartDate DATETIME = NULL,
  @ExpirationDate DATETIME = NULL,
  @ContractDescription NVARCHAR(MAX) = NULL,
  @Amount VARCHAR(12) = NULL,
  @SquareFootage VARCHAR(12) = NULL,
  @CleanTimes INT = NULL,
  @Mon BIT = NULL,
  @Tue BIT = NULL,
  @Wed BIT = NULL,
  @Thu BIT = NULL,
  @Fri BIT = NULL,
  @Sat BIT = NULL,
  @Sun BIT = NULL,
  @StartTime DATETIME = NULL,
  @EndTime DATETIME = NULL,
  @AssignedTeamId INT = NULL,
  @AssignedEmployeeId INT = NULL,
  @AssignedTemplate NVARCHAR(MAX) = NULL
)
AS

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

BEGIN TRANSACTION UPSERT

BEGIN TRY
  
  SET NOCOUNT ON;
  
  IF @DistributionId < 1 SET @DistributionId = NULL
  IF @RegionId < 1 SET @RegionId = NULL
  IF @FranchiseeId < 1 SET @FranchiseeId = NULL
  IF @ContractId < 1 SET @ContractId = NULL
  IF @ContractDetailId < 1 SET @ContractDetailId = NULL
  IF @ContractTypeListId < 1 SET @ContractTypeListId = NULL
  IF @AccountTypeListId < 1 SET @AccountTypeListId = NULL
  IF @ServiceTypeListId < 1 SET @ServiceTypeListId = NULL
  IF @AddressId < 1 SET @AddressId = NULL
  IF @AssignedTeamId < 1 SET @AssignedTeamId = NULL
  IF @AssignedEmployeeId < 1 SET @AssignedEmployeeId = NULL


  UPDATE 
    [dbo].[Job]
  SET
    [dbo].[Job].AddressId = COALESCE(@AddressId, [dbo].[Job].AddressId),
    [dbo].[Job].SoldById = COALESCE(@SoldById, [dbo].[Job].SoldById),
    [dbo].[Job].PurchaseOrderNumber = COALESCE(@PurchaseOrderNumber, [dbo].[Job].PurchaseOrderNumber),
    [dbo].[Job].CustomerName = COALESCE(@CustomerName, [dbo].[Job].CustomerName),
    [dbo].[Job].PrimaryContact = COALESCE(@PrimaryContact, [dbo].[Job].PrimaryContact),
    [dbo].[Job].PrimaryContactPhone = COALESCE(@PrimaryContactPhoneExt, [dbo].[Job].PrimaryContactPhone),
    [dbo].[Job].PrimaryContactPhoneExt = COALESCE(@PrimaryContactPhoneExt, [dbo].[Job].PrimaryContactPhoneExt),
    [dbo].[Job].ContractTypeListId = COALESCE(@ContractTypeListId, [dbo].[Job].ContractTypeListId),
    [dbo].[Job].AccountTypeListId = COALESCE(@AccountTypeListId, [dbo].[Job].AccountTypeListId),
    [dbo].[Job].ServiceTypeListId = COALESCE(@ServiceTypeListId, [dbo].[Job].ServiceTypeListId),
    [dbo].[Job].StartDate = COALESCE(@StartDate, [dbo].[Job].StartDate),
    [dbo].[Job].ExpirationDate = COALESCE(@ExpirationDate, [dbo].[Job].ExpirationDate), 
    [dbo].[Job].ContractDescription = COALESCE(@ContractDescription, [dbo].[Job].ContractDescription),
    [dbo].[Job].Amount = COALESCE(@Amount, [dbo].[Job].Amount),
    [dbo].[Job].SquareFootage = COALESCE(@SquareFootage, [dbo].[Job].SquareFootage),
    [dbo].[Job].CleanTimes = COALESCE(@CleanTimes, [dbo].[Job].CleanTimes),
    [dbo].[Job].Mon = COALESCE(@Mon, [dbo].[Job].Mon),
    [dbo].[Job].Tue = COALESCE(@Tue, [dbo].[Job].Tue),
    [dbo].[Job].Wed = COALESCE(@Wed, [dbo].[Job].Wed),
    [dbo].[Job].Thu = COALESCE(@Thu, [dbo].[Job].Thu),
    [dbo].[Job].Fri = COALESCE(@Fri, [dbo].[Job].Fri),
    [dbo].[Job].Sat = COALESCE(@Sat, [dbo].[Job].Sat),
    [dbo].[Job].Sun = COALESCE(@Sun, [dbo].[Job].Sun),
    [dbo].[Job].StartTime = COALESCE(@StartTime, [dbo].[Job].StartTime),
    [dbo].[Job].EndTime = COALESCE(@EndTime, [dbo].[Job].EndTime),
    [dbo].[Job].AssignedTeamId = COALESCE(@AssignedTeamId, [dbo].[Job].AssignedTeamId),
    [dbo].[Job].AssignedEmployeeId = COALESCE(@AssignedEmployeeId, [dbo].[Job].AssignedEmployeeId),
    [dbo].[Job].AssignedTemplate = COALESCE(@AssignedTemplate, [dbo].[Job].AssignedTemplate)
  WHERE
    [dbo].[Job].JobId = @JobId

  IF @@ROWCOUNT = 0
    BEGIN
    INSERT INTO [dbo].[Job]
    (
      [dbo].[Job].AddressId,
      [dbo].[Job].DistributionId,
      [dbo].[Job].RegionId,
      [dbo].[Job].FranchiseeId,
      [dbo].[Job].ContractId,
      [dbo].[Job].ContractDetailId,
      [dbo].[Job].CustomerId,
      [dbo].[Job].SoldById,
      [dbo].[Job].PurchaseOrderNumber,
      [dbo].[Job].CustomerName,
      [dbo].[Job].PrimaryContact,
      [dbo].[Job].PrimaryContactPhone,
      [dbo].[Job].PrimaryContactPhoneExt,
      [dbo].[Job].ContractTypeListId,
      [dbo].[Job].AccountTypeListId,
      [dbo].[Job].ServiceTypeListId,
      [dbo].[Job].StartDate,
      [dbo].[Job].ExpirationDate,
      [dbo].[Job].ContractDescription,
      [dbo].[Job].Amount,
      [dbo].[Job].SquareFootage,
      [dbo].[Job].CleanTimes,
      [dbo].[Job].Mon,
      [dbo].[Job].Tue,
      [dbo].[Job].Wed,
      [dbo].[Job].Thu,
      [dbo].[Job].Fri,
      [dbo].[Job].Sat,
      [dbo].[Job].Sun,
      [dbo].[Job].StartTime,
      [dbo].[Job].EndTime,
      [dbo].[Job].AssignedTeamId,
      [dbo].[Job].AssignedEmployeeId,
      [dbo].[Job].AssignedTemplate,
      [dbo].[Job].IsEnable
    )
    VALUES
    (
      @AddressId,
      @DistributionId,
      @RegionId,
      @FranchiseeId,
      @ContractId,
      @ContractDetailId,
      @CustomerId,
      @SoldById,
      @PurchaseOrderNumber,
      @CustomerName,
      @PrimaryContact,
      @PrimaryContactPhone,
      @PrimaryContactPhoneExt,
      @ContractTypeListId,
      @AccountTypeListId,
      @ServiceTypeListId,
      @StartDate,
      @ExpirationDate,
      @ContractDescription,
      @Amount,
      @SquareFootage,
      @CleanTimes,
      @Mon,
      @Tue,
      @Wed,
      @Thu,
      @Fri,
      @Sat,
      @Sun,
      @StartTime,
      @EndTime,
      @AssignedTeamId,
      @AssignedEmployeeId,
      @AssignedTemplate,
      1
	)
    SELECT @JobId = SCOPE_IDENTITY()
    END

    EXEC sp_GetJob @JobId, 1

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
        ROLLBACK TRANSACTION UPSERT;
END CATCH

IF @@TRANCOUNT > 0
  COMMIT TRANSACTION UPSERT;
