IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_GetTemplate]
GO

CREATE PROCEDURE [dbo].[sp_GetTemplate]
(
	@FormTemplateId INT = 0,
	@IsEnable BIT = 1
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	SELECT
		FormTemplate.FormTemplateId,
		FormTemplate.AccountTypeListId,
		FormTemplate.ServiceTypeListId,
		FormTemplate.FormTemplateTypeId,
		AccountTypeList.Name AS AccountTypeListName,
		ServiceTypeList.Name AS ServiceTypeListName,
		FormTemplateType.Name AS FormTemplateName,
		FormTemplate.FormName,
		FormTemplate.Description,
		FormTemplate.IsEnable,
		FormTemplate.IsDelete,
		FormTemplate.CreatedBy,
		FormTemplate.CreatedDate,
		FormTemplate.ModifiedBy,
		FormTemplate.ModifiedDate
	FROM
		[dbo].[FormTemplate] FormTemplate WITH (NOLOCK)
	LEFT JOIN [dbo].[AccountTypeList] AS AccountTypeList ON FormTemplate.AccountTypeListId = AccountTypeList.AccountTypeListId
	LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList ON FormTemplate.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
	LEFT JOIN [dbo].[FormTemplateType] FormTemplateType ON FormTemplate.FormTemplateTypeId = FormTemplateType.FormTemplateTypeId
	WHERE
		FormTemplate.IsEnable = @IsEnable AND
		FormTemplate.FormTemplateId = @FormTemplateId

	CREATE TABLE #Temp_Table_TemplateAreas
	(
		TemplateAreaId INT NOT NULL,
		AreaName NVARCHAR(100) NOT NULL,
		IsEnable BIT,
		IsDelete BIT,
		CreatedBy INT,
		CreatedDate DATETIME,
		ModifiedBy INT,
		ModifiedDate DATETIME
	)

	INSERT INTO #Temp_Table_TemplateAreas
		SELECT
			TemplateArea.TemplateAreaId,
			TemplateArea.AreaName,
			TemplateArea.IsEnable,
			TemplateArea.IsDelete,
			TemplateArea.CreatedBy,
			TemplateArea.CreatedOn AS CreatedDate,
			TemplateArea.ModifiedBy,
			TemplateArea.ModifiedOn AS ModifiedDate
		FROM
			[dbo].[TemplateArea] TemplateArea WITH (NOLOCK),
			[dbo].[FormTemplateAreaMapping] FormTemplateAreaMapping WITH (NOLOCK)
		WHERE
			TemplateArea.IsEnable = @IsEnable AND
			FormTemplateAreaMapping.FormTemplateId = @FormTemplateId AND
			FormTemplateAreaMapping.TemplateAreaId = TemplateArea.TemplateAreaId AND
			FormTemplateAreaMapping.IsEnable = 1

	SELECT * FROM #Temp_Table_TemplateAreas

	SELECT
		#Temp_Table_TemplateAreas.TemplateAreaId,
		TemplateAreaItem.TemplateAreaItemId,
		TemplateAreaItem.ItemName,
		TemplateAreaItem.FormItemType,
		TemplateAreaItem.FormItemValue,
		TemplateAreaItem.IsDirty,
		TemplateAreaItem.IsRequired,
		TemplateAreaItem.IsEnable,
		TemplateAreaItem.IsDelete,
		TemplateAreaItem.CreatedBy,
		TemplateAreaItem.CreatedOn AS CreatedDate,
		TemplateAreaItem.ModifiedBy,
		TemplateAreaItem.ModifiedOn AS ModifiedDate
	FROM
		#Temp_Table_TemplateAreas,
		[dbo].[TemplateAreaItem] TemplateAreaItem WITH (NOLOCK),
		[dbo].[TemplateAreaItemMapping] TemplateAreaItemMapping WITH (NOLOCK)
	WHERE
		TemplateAreaItem.IsEnable = @IsEnable AND
		TemplateAreaItemMapping.TemplateAreaItemId = TemplateAreaItem.TemplateAreaItemId AND
		TemplateAreaItemMapping.TemplateAreaId = #Temp_Table_TemplateAreas.TemplateAreaId AND
		TemplateAreaItemMapping.IsEnable = 1

	DROP TABLE #Temp_Table_TemplateAreas

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