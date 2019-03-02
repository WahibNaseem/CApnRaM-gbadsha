IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetInspectionFormItemHistoryBySection]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetInspectionFormItemHistoryBySection]
GO  
  
CREATE PROCEDURE [dbo].[sp_GetInspectionFormItemHistoryBySection]    
(
	@InspectionFormSectionHistoryId INT = 0,  
	@IsEnable BIT = 1,  
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'  
)  
AS  
  
BEGIN TRANSACTION;  
  
	BEGIN TRY  
  
	SET NOCOUNT ON;  
  
	SELECT
		InspectionFormItemHistory.InspectionFormItemHistoryId AS InspectionFormItemId,  
		InspectionFormItemHistory.InspectionFormSectionHistoryId, 
		InspectionFormItemHistory.FormItemType,  
		InspectionFormItemHistory.FormItemOrder,  
		InspectionFormItemHistory.FormItemValue,  
		InspectionFormItemHistory.IsDirty,  
		InspectionFormItemHistory.IsRequired,  
		InspectionFormItemHistory.IsEnable,  
		InspectionFormItemHistory.CreatedBy,  
		InspectionFormItemHistory.CreatedDate,  
		InspectionFormItemHistory.ModifiedBy,  
		InspectionFormItemHistory.ModifiedDate  
	FROM  
		[dbo].[InspectionFormItemHistory] InspectionFormItemHistory WITH (NOLOCK)  
	WHERE  
		InspectionFormItemHistory.InspectionFormSectionHistoryId = @InspectionFormSectionHistoryId AND  
		InspectionFormItemHistory.IsEnable = @IsEnable  
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormItemHistoryId' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.InspectionFormItemHistoryId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormItemHistoryId' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.InspectionFormItemHistoryId END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionHistoryId' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.InspectionFormSectionHistoryId END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'InspectionFormSectionHistoryId' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.InspectionFormSectionHistoryId END DESC, 
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.FormItemType END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemType' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.FormItemType END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemOrder' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.FormItemOrder END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemOrder' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.FormItemOrder END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.FormItemValue END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'FormItemValue' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.FormItemValue END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.IsDirty END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsDirty' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.IsDirty END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.IsRequired END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsRequired' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.IsRequired END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.IsEnable END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'IsEnable' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.IsEnable END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.CreatedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedBy' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.CreatedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.CreatedDate END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'CreatedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.CreatedDate END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.ModifiedBy END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedBy' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.ModifiedBy END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='asc') THEN InspectionFormItemHistory.ModifiedDate END ASC,  
		CASE WHEN (ISNULL(@SortColumn,'') = 'ModifiedDate' AND LOWER(@SortOrder)='desc') THEN InspectionFormItemHistory.ModifiedDate END DESC,  
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN InspectionFormItemHistory.InspectionFormItemHistoryId END ASC  
  
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