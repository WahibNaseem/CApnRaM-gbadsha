IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetCrmCallLog]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetCrmCallLog]
GO  

CREATE PROCEDURE [dbo].[sp_GetCrmCallLog]      
(
	@CallLogId INT = 0,
	@IsEnable BIT = 1
)    
AS    
    
BEGIN TRANSACTION;    
    
BEGIN TRY

	SET NOCOUNT ON;

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
		CallLog.CRM_CallLogId = @CallLogId
		 
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