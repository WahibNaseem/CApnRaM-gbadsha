IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[portal_FOM_GetFranchiseeById]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[portal_FOM_GetFranchiseeById]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[portal_FOM_GetFranchiseeById]
(
	@FranchiseeId INT = 0
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	DECLARE @CurrentTimeStamp DATETIME = GETDATE()

	CREATE TABLE #Temp_Table_FranchiseeByFranchiseeId
	(
		FranchiseeId INT NOT NULL,
		DlrCode VARCHAR(10) NOT NULL,
		FranchiseeName VARCHAR(150),
		FranchiseeType INT,
		FranchiseeStatus INT,
		RegionId INT,
		IsEnable BIT,
		CreatedBy INT,
		CreatedDate DATETIME,
		ModifiedBy INT,
		ModifiedDate DATETIME
	)

	-- Insert data into the first table
	INSERT INTO #Temp_Table_FranchiseeByFranchiseeId
		SELECT TOP 1
			Franchisee.FranchiseeId,
			Franchisee.FranchiseeNo AS DlrCode,
			Franchisee.Name AS FranchiseeName,
			Franchisee.FranchiseeTypeListId AS FranchiseeType,
			Franchisee.StatusListId AS FranchiseeStatus,
			Franchisee.RegionId,
			Franchisee.IsActive AS IsEnable,
			Franchisee.CreatedBy,
			Franchisee.CreatedDate,
			Franchisee.ModifiedBy,
			Franchisee.ModifiedDate
		FROM
			[dbo].[Distribution] Distribution WITH (NOLOCK),
			[dbo].[Franchisee] Franchisee WITH (NOLOCK)
		WHERE
			Distribution.FranchiseeId = Franchisee.FranchiseeId AND Franchisee.StatusListId = 9 AND
			Franchisee.FranchiseeId = @FranchiseeId AND
			Distribution.IsActive = 1

		SELECT 
			ResultTable.FranchiseeId,
			ResultTable.DlrCode,
			ResultTable.FranchiseeName,
			ResultTable.FranchiseeType,
			FranchiseeTypeList.Name AS FranchiseeTypeName,
			Phone.Phone,
			RIGHT(Identification.IdentifierNumer,4) AS Ssn,
			ResultTable.FranchiseeStatus,
			ResultTable.RegionId,
			ResultTable.IsEnable,
			ResultTable.CreatedBy,
			ResultTable.CreatedDate,
			ResultTable.ModifiedBy,
			ResultTable.ModifiedDate
		FROM
			#Temp_Table_FranchiseeByFranchiseeId ResultTable WITH (NOLOCK)
		LEFT JOIN [dbo].[FranchiseeTypeList] FranchiseeTypeList WITH (NOLOCK) ON FranchiseeTypeList.FranchiseeTypeListId = ResultTable.FranchiseeType
		LEFT JOIN [dbo].[Phone] Phone WITH (NOLOCK) ON Phone.ClassId = ResultTable.FranchiseeId AND Phone.TypeListId = 2 AND Phone.ContactTypeListId = 1 AND Phone.IsActive = 1
		LEFT JOIN [dbo].[Identification] Identification WITH (NOLOCK) ON Identification.ClassId = ResultTable.FranchiseeId AND Identification.TypeListId = 2
	
	-- Remove temp tables
	DROP TABLE #Temp_Table_FranchiseeByFranchiseeId

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