IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetCrmCallLogListByAccountCustomerDetail]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetCrmCallLogListByAccountCustomerDetail]
GO  

CREATE PROCEDURE [dbo].[sp_GetCrmCallLogListByAccountCustomerDetail]      
(
	@AccountCustomerDetailId INT = 0,
	@PageNo INT = 1,  
	@PageSize INT = 10,  
	@SortColumn NVARCHAR(20) = '',  
	@SortOrder NVARCHAR(4) = 'asc'  
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

	IF @PageNo <= 1 SET @PageNo = 1
	IF @PageSize <= 0 SET @PageSize = 10

	SELECT
		CallLog.CRM_CallLogId AS CallLogId,
		CallLog.CRM_AccountId AS AccountId,
		CallLog.CRM_AccountCustomerDetailId AS AccountCustomerDetailId,
		CallLog.CRM_LeadSource AS LeadSource,
		CallLog.CRM_CallResultId AS CallResultId,
		CallLog.StageStatus,
		CallLog.CRM_NoteTypeId AS NoteTypeId,
		CallLog.SpokeWith,
		CallLog.Note,
		CallLog.CallLogDate,
		CallLog.CallBack,
		CallLog.CallBackTime,
		CallLog.CreatedBy,
		CallLog.CreatedDate,
		CallLog.ModifiedBy,
		CallLog.ModifiedDate
	FROM
		[dbo].[CRM_CallLog] CallLog WITH (NOLOCK)
	WHERE
		CallLog.CRM_AccountCustomerDetailId = @AccountCustomerDetailId
	ORDER BY
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallLogId' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_CallLogId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallLogId' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_CallLogId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountId' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_AccountId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountId' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_AccountId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountCustomerDetailId' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_AccountCustomerDetailId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'AccountCustomerDetailId' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_AccountCustomerDetailId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LeadSource' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_LeadSource END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'LeadSource' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_LeadSource END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallResultId' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_CallResultId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallResultId' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_CallResultId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StageStatus' AND LOWER(@SortOrder)='asc') THEN CallLog.StageStatus END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'StageStatus' AND LOWER(@SortOrder)='desc') THEN CallLog.StageStatus END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NoteTypeId' AND LOWER(@SortOrder)='asc') THEN CallLog.CRM_NoteTypeId END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'NoteTypeId' AND LOWER(@SortOrder)='desc') THEN CallLog.CRM_NoteTypeId END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SpokeWith' AND LOWER(@SortOrder)='asc') THEN CallLog.SpokeWith END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'SpokeWith' AND LOWER(@SortOrder)='desc') THEN CallLog.SpokeWith END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Note' AND LOWER(@SortOrder)='asc') THEN CallLog.Note END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'Note' AND LOWER(@SortOrder)='desc') THEN CallLog.Note END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallLogDate' AND LOWER(@SortOrder)='asc') THEN CallLog.CallLogDate END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallLogDate' AND LOWER(@SortOrder)='desc') THEN CallLog.CallLogDate END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallBack' AND LOWER(@SortOrder)='asc') THEN CallLog.CallBack END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallBack' AND LOWER(@SortOrder)='desc') THEN CallLog.CallBack END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallBackTime' AND LOWER(@SortOrder)='asc') THEN CallLog.CallBackTime END ASC,
		CASE WHEN (ISNULL(@SortColumn,'') = 'CallBackTime' AND LOWER(@SortOrder)='desc') THEN CallLog.CallBackTime END DESC,
		CASE WHEN (ISNULL(@SortColumn,'') = '') THEN CallLog.CRM_CallLogId END ASC
	OFFSET (@PageNo-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
		 
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