IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormSectionListByForm]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormSectionListByForm]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormSectionListByForm]    
(
	@InspectionFormId INT = 0,  
	@IsEnable BIT = 1,
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'
)  
AS  
  
BEGIN TRANSACTION;  
  
BEGIN TRY
	
	SET NOCOUNT ON;
	
	SELECT  
		InspectionFormSection.InspectionFormSectionId,
		InspectionFormSection.InspectionFormId, 
		InspectionFormSection.SectionOrder,  
		InspectionFormSection.SectionName,  
		InspectionFormSection.SectionStatus,  
		InspectionFormSection.ScorePercent,  
		InspectionFormSection.PassPoints,  
		InspectionFormSection.FailPoints,  
		InspectionFormSection.NeedImprovementPoints,
		InspectionFormSection.SectionAutoFail,
		InspectionFormSection.SectionAutoFailReason,
		COUNT(InspectionFormItem.InspectionFormSectionId) AS NumberOfItems,
		InspectionFormSection.IsEnable,  
		InspectionFormSection.CreatedBy,  
		InspectionFormSection.CreatedDate,  
		InspectionFormSection.ModifiedBy,  
		InspectionFormSection.ModifiedDate  
	FROM
		[dbo].[InspectionForm] InspectionForm WITH (NOLOCK),
		[dbo].[InspectionFormSection] InspectionFormSection WITH (NOLOCK)
	LEFT JOIN [dbo].[InspectionFormItem] InspectionFormItem WITH (NOLOCK) ON InspectionFormItem.InspectionFormSectionId = InspectionFormSection.InspectionFormSectionId  
	WHERE  
		InspectionFormSection.InspectionFormId = @InspectionFormId AND
		InspectionFormSection.InspectionFormId = InspectionForm.InspectionFormId AND  
		InspectionFormSection.IsEnable = @IsEnable
	GROUP BY  
		InspectionFormSection.InspectionFormSectionId,
		InspectionFormSection.InspectionFormId,
		InspectionFormSection.SectionOrder,  
		InspectionFormSection.SectionName,  
		InspectionFormSection.SectionStatus,  
		InspectionFormSection.ScorePercent,  
		InspectionFormSection.PassPoints,  
		InspectionFormSection.FailPoints,  
		InspectionFormSection.NeedImprovementPoints,
		InspectionFormSection.SectionAutoFail,
		InspectionFormSection.SectionAutoFailReason,
		InspectionFormSection.IsEnable,  
		InspectionFormSection.CreatedBy,  
		InspectionFormSection.CreatedDate,  
		InspectionFormSection.ModifiedBy,  
		InspectionFormSection.ModifiedDate 
	ORDER BY  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionId' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.InspectionFormSectionId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionId' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.InspectionFormSectionId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.InspectionFormId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormId' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.InspectionFormId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionOrder' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.SectionOrder END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionOrder' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.SectionOrder END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionName' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.SectionName END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionName' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.SectionName END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionStatus' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.SectionStatus END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionStatus' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.SectionStatus END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.ScorePercent END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ScorePercent' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.ScorePercent END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.PassPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'PassPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.PassPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.FailPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FailPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.FailPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.NeedImprovementPoints END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'NeedImprovementPoints' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.NeedImprovementPoints END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFail' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.SectionAutoFail END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFail' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.SectionAutoFail END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFailReason' AND LOWER(@SortOrder)='asc') THEN InspectionFormSection.SectionAutoFailReason END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'SectionAutoFailReason' AND LOWER(@SortOrder)='desc') THEN InspectionFormSection.SectionAutoFailReason END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionFormSection.InspectionFormSectionId END ASC
   
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