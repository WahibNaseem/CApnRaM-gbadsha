IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormSection]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormSection]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormSection]    
(
	@InspectionFormSectionId INT = 0,  
	@IsEnable BIT = 1  
)  
AS  
  
BEGIN TRANSACTION;  
  
BEGIN TRY
	
	SET NOCOUNT ON;
	
	SELECT  
		InspectionFormSection.InspectionFormSectionId,  
		InspectionFormSection.SectionOrder,  
		InspectionFormSection.SectionName,  
		InspectionFormSection.SectionStatus,  
		COUNT(InspectionFormItem.InspectionFormSectionId) AS NumberOfItems,  
		InspectionFormSection.ScorePercent,  
		InspectionFormSection.PassPoints,  
		InspectionFormSection.FailPoints,  
		InspectionFormSection.NeedImprovementPoints,  
		InspectionFormSection.IsEnable,  
		InspectionFormSection.CreatedBy,  
		InspectionFormSection.CreatedDate,  
		InspectionFormSection.ModifiedBy,  
		InspectionFormSection.ModifiedDate  
	FROM  
		[dbo].[InspectionFormSection] InspectionFormSection WITH (NOLOCK)  
	LEFT JOIN [dbo].[InspectionFormItem] InspectionFormItem WITH (NOLOCK) ON InspectionFormItem.InspectionFormSectionId = InspectionFormSection.InspectionFormSectionId  
	WHERE  
		InspectionFormSection.InspectionFormSectionId = @InspectionFormSectionId AND  
		InspectionFormSection.IsEnable = @IsEnable  
	GROUP BY  
		InspectionFormSection.InspectionFormSectionId,  
		InspectionFormSection.SectionOrder,  
		InspectionFormSection.SectionName,  
		InspectionFormSection.SectionStatus,  
		InspectionFormSection.ScorePercent,  
		InspectionFormSection.PassPoints,  
		InspectionFormSection.FailPoints,  
		InspectionFormSection.NeedImprovementPoints,  
		InspectionFormSection.IsEnable,  
		InspectionFormSection.CreatedBy,  
		InspectionFormSection.CreatedDate,  
		InspectionFormSection.ModifiedBy,  
		InspectionFormSection.ModifiedDate  
   
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