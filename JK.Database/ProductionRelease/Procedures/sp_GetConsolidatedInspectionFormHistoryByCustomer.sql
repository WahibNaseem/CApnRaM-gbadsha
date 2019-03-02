IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetConsolidatedInspectionFormHistoryByCustomer]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetConsolidatedInspectionFormHistoryByCustomer]
GO  

CREATE PROCEDURE [dbo].[sp_GetConsolidatedInspectionFormHistoryByCustomer]      
(
	@CustomerId INT = 0,
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

	DECLARE @TotalItems INT
	SET @TotalItems = (SELECT 
						COUNT(InspectionFormItemHistory.InspectionFormItemHistoryId)
					   FROM
						[dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK),
						[dbo].[InspectionFormSectionHistory] InspectionFormSectionHistory WITH (NOLOCK),
						[dbo].[InspectionFormItemHistory] InspectionFormItemHistory WITH (NOLOCK)
					   WHERE
						InspectionFormHistory.InspectionFormHistoryId = InspectionFormSectionHistory.InspectionFormHistoryId AND
						InspectionFormSectionHistory.InspectionFormSectionHistoryId = InspectionFormItemHistory.InspectionFormSectionHistoryId AND
						InspectionFormHistory.CustomerId = @CustomerId AND
						InspectionFormHistory.IsEnable = @IsEnable)


	DECLARE @TotalPass INT
	SET @TotalPass = (SELECT COUNT(InspectionFormHistory.PassPoints) 
	FROM InspectionFormHistory InspectionFormHistory WITH (NOLOCK)
	WHERE InspectionFormHistory.PassPoints >= 75 AND InspectionFormHistory.CustomerId = @CustomerId	
	)
	DECLARE @TotalFail INT
	SET @TotalFail = (SELECT COUNT(Ins.FailPoints) FROM InspectionFormHistory Ins WITH (NOLOCK) 
	WHERE Ins.FailPoints < 75 AND Ins.CustomerId = @CustomerId	
	)

	SELECT
		(SELECT TOP 1 CustomerId FROM [dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK) 
		WHERE InspectionFormHistory.CustomerId = @CustomerId 
		ORDER BY InspectionFormHistory.InspectionFormHistoryId DESC) AS CustomerId,
		(SELECT TOP 1 CustomerNo FROM [dbo].[Customer] Customer WITH (NOLOCK) 
		WHERE Customer.CustomerId = @CustomerId) AS CustomerNumber,
		(SELECT TOP 1 Name FROM [dbo].[Customer] Customer WITH (NOLOCK) 
		WHERE Customer.CustomerId = @CustomerId) AS CustomerName,
		(SELECT TOP 1 RegionId FROM [dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK) 
		WHERE InspectionFormHistory.CustomerId = @CustomerId
		ORDER BY InspectionFormHistory.InspectionFormHistoryId DESC) AS RegionId,
		(SUM(InspectionFormHistory.PassPoints) / @TotalItems) * 100 AS TotalPassPoints,
		(SUM(InspectionFormHistory.FailPoints) / @TotalItems) * 100 AS TotalFailPoints,
		(SUM(InspectionFormHistory.NeedImprovementPoints) / @TotalItems) * 100 AS TotalNeedImprovementPoints,
		AVG(InspectionFormHistory.ScorePercent) AS AverageScorePercent,
		MAX(InspectionFormHistory.RecordedDate) AS LastInspectionDate,
		@TotalPass AS PassCount,
		@TotalFail AS FailCount,
		COUNT(InspectionFormHistory.InspectionFormHistoryId) AS TotalInspections,
		(SELECT TOP 1 InspectedBy FROM [dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK) 
		WHERE InspectionFormHistory.CustomerId = @CustomerId 
		ORDER BY InspectionFormHistory.InspectionFormHistoryId DESC) AS LastInspectedBy
	FROM
		[dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK)
	WHERE
		InspectionFormHistory.CustomerId = @CustomerId AND
		InspectionFormHistory.IsEnable = @IsEnable
		 
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