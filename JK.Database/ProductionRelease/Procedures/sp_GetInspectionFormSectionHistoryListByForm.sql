IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormSectionHistoryListByForm]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormSectionHistoryListByForm]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormSectionHistoryListByForm]    
(
	@InspectionFormHistoryId INT = 0,  
	@IsEnable BIT = 1,
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'
)  
AS  
  
BEGIN TRANSACTION;  
  
BEGIN TRY
	
	SET NOCOUNT ON;
	
	SELECT  
		InspectionFormSectionHistory.InspectionFormSectionHistoryId AS InspectionFormSectionId,
		InspectionFormSectionHistory.InspectionFormHistoryId, 
		InspectionFormSectionHistory.SectionOrder,  
		InspectionFormSectionHistory.SectionName,  
		InspectionFormSectionHistory.SectionStatus,  
		InspectionFormSectionHistory.ScorePercent,  
		InspectionFormSectionHistory.PassPoints,  
		InspectionFormSectionHistory.FailPoints,  
		InspectionFormSectionHistory.NeedImprovementPoints,
		InspectionFormSectionHistory.SectionAutoFail,
		InspectionFormSectionHistory.SectionAutoFailReason,
		COUNT(InspectionFormItemHistory.InspectionFormSectionHistoryId) AS NumberOfItems,
		InspectionFormSectionHistory.IsEnable,  
		InspectionFormSectionHistory.CreatedBy,  
		InspectionFormSectionHistory.CreatedDate,  
		InspectionFormSectionHistory.ModifiedBy,  
		InspectionFormSectionHistory.ModifiedDate  
	FROM
		[dbo].[InspectionFormHistory] InspectionFormHistory WITH (NOLOCK),
		[dbo].[InspectionFormSectionHistory] InspectionFormSectionHistory WITH (NOLOCK)
	LEFT JOIN [dbo].[InspectionFormItemHistory] InspectionFormItemHistory WITH (NOLOCK) ON InspectionFormItemHistory.InspectionFormSectionHistoryId = InspectionFormSectionHistory.InspectionFormSectionHistoryId  
	WHERE  
		InspectionFormSectionHistory.InspectionFormHistoryId = @InspectionFormHistoryId AND
		InspectionFormSectionHistory.InspectionFormHistoryId = InspectionFormHistory.InspectionFormHistoryId AND  
		InspectionFormSectionHistory.IsEnable = @IsEnable
	GROUP BY  
		InspectionFormSectionHistory.InspectionFormSectionHistoryId,
		InspectionFormSectionHistory.InspectionFormHistoryId,
		InspectionFormSectionHistory.SectionOrder,  
		InspectionFormSectionHistory.SectionName,  
		InspectionFormSectionHistory.SectionStatus,  
		InspectionFormSectionHistory.ScorePercent,  
		InspectionFormSectionHistory.PassPoints,  
		InspectionFormSectionHistory.FailPoints,  
		InspectionFormSectionHistory.NeedImprovementPoints,
		InspectionFormSectionHistory.SectionAutoFail,
		InspectionFormSectionHistory.SectionAutoFailReason,
		InspectionFormSectionHistory.IsEnable,  
		InspectionFormSectionHistory.CreatedBy,  
		InspectionFormSectionHistory.CreatedDate,  
		InspectionFormSectionHistory.ModifiedBy,  
		InspectionFormSectionHistory.ModifiedDate 
	ORDER BY  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionHistoryId' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.InspectionFormSectionHistoryId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionHistoryId' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.InspectionFormSectionHistoryId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormHistoryId' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.InspectionFormHistoryId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormHistoryId' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.InspectionFormHistoryId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionOrder' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.SectionOrder END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionOrder' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.SectionOrder END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionName' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.SectionName END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionName' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.SectionName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionStatus' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.SectionStatus END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionStatus' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.SectionStatus END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.ScorePercent END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.ScorePercent END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.PassPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.PassPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.FailPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.FailPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.NeedImprovementPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.NeedImprovementPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFail' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.SectionAutoFail END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFail' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.SectionAutoFail END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFailReason' AND LOWER(@SortOrder)='asc') THEN InspectionFormSectionHistory.SectionAutoFailReason END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFailReason' AND LOWER(@SortOrder)='desc') THEN InspectionFormSectionHistory.SectionAutoFailReason END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionFormSectionHistory.InspectionFormSectionHistoryId END ASC
   
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