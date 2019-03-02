IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormSectionHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormSectionHistory]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormSectionHistory]    
(
	@InspectionFormSectionHistoryId INT = 0,  
	@IsEnable BIT = 1  
)  
AS  
  
BEGIN TRANSACTION;  
  
BEGIN TRY
	
	SET NOCOUNT ON;
	
	SELECT  
		InspectionFormSectionHistory.InspectionFormSectionHistoryId AS InspectionFormSectionId,  
		InspectionFormSectionHistory.SectionOrder,  
		InspectionFormSectionHistory.SectionName,  
		InspectionFormSectionHistory.SectionStatus,  
		COUNT(InspectionFormItemHistory.InspectionFormSectionHistoryId) AS NumberOfItems,  
		InspectionFormSectionHistory.ScorePercent,  
		InspectionFormSectionHistory.PassPoints,  
		InspectionFormSectionHistory.FailPoints,  
		InspectionFormSectionHistory.NeedImprovementPoints,  
		InspectionFormSectionHistory.IsEnable,  
		InspectionFormSectionHistory.CreatedBy,  
		InspectionFormSectionHistory.CreatedDate,  
		InspectionFormSectionHistory.ModifiedBy,  
		InspectionFormSectionHistory.ModifiedDate  
	FROM  
		[dbo].[InspectionFormSectionHistory] InspectionFormSectionHistory WITH (NOLOCK)  
	LEFT JOIN [dbo].[InspectionFormItemHistory] InspectionFormItemHistory WITH (NOLOCK) ON InspectionFormItemHistory.InspectionFormSectionHistoryId = InspectionFormSectionHistory.InspectionFormSectionHistoryId  
	WHERE  
		InspectionFormSectionHistory.InspectionFormSectionHistoryId = @InspectionFormSectionHistoryId AND  
		InspectionFormSectionHistory.IsEnable = @IsEnable  
	GROUP BY  
		InspectionFormSectionHistory.InspectionFormSectionHistoryId,  
		InspectionFormSectionHistory.SectionOrder,  
		InspectionFormSectionHistory.SectionName,  
		InspectionFormSectionHistory.SectionStatus,  
		InspectionFormSectionHistory.ScorePercent,  
		InspectionFormSectionHistory.PassPoints,  
		InspectionFormSectionHistory.FailPoints,  
		InspectionFormSectionHistory.NeedImprovementPoints,  
		InspectionFormSectionHistory.IsEnable,  
		InspectionFormSectionHistory.CreatedBy,  
		InspectionFormSectionHistory.CreatedDate,  
		InspectionFormSectionHistory.ModifiedBy,  
		InspectionFormSectionHistory.ModifiedDate  
   
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