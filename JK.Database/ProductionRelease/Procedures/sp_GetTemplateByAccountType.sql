IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_GetTemplateByAccountType]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[sp_GetTemplateByAccountType]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetTemplateByAccountType]
(
	@AccountTypeListId INT = 0,
	@IsEnable BIT = 1
)
AS

BEGIN TRANSACTION;

BEGIN TRY

	SET NOCOUNT ON;

	CREATE TABLE #Temp_Table_TemplateByAccountType
	(
		FormTemplateId INT NOT NULL,
		AccountTypeListId INT NOT NULL,
		ServiceTypeListId INT NULL,
		FormTemplateTypeId INT NULL,
		AccountTypeListName NVARCHAR(128) NULL,
		ServiceTypeListName NVARCHAR(128) NULL,
		FormTemplateName NVARCHAR(128) NULL,
		FormName NVARCHAR(128) NULL,
		Description NVARCHAR(MAX) NULL,
		CreatedBy INT NULL,
		CreatedDate DATETIME NULL,
		ModifiedBy INT NULL,
		ModifiedDate DATETIME NULL
	)

	INSERT INTO #Temp_Table_TemplateByAccountType
		SELECT TOP 1
			FormTemplate.FormTemplateId,
			FormTemplate.AccountTypeListId,
			FormTemplate.ServiceTypeListId,
			FormTemplate.FormTemplateTypeId,
			AccountTypeList.Name AS AccountTypeListName,
			ServiceTypeList.Name AS ServiceTypeListName,
			FormTemplateType.Name AS FormTemplateName,
			FormTemplate.FormName,
			FormTemplate.Description,
			FormTemplate.CreatedBy,
			FormTemplate.CreatedDate,
			FormTemplate.ModifiedBy,
			FormTemplate.ModifiedDate
		FROM
			[dbo].[FormTemplate] FormTemplate WITH (NOLOCK)
		LEFT JOIN [dbo].[AccountTypeList] AccountTypeList WITH (NOLOCK) ON FormTemplate.AccountTypeListId = AccountTypeList.AccountTypeListId
		LEFT JOIN [dbo].[ServiceTypeList] ServiceTypeList WITH (NOLOCK) ON FormTemplate.ServiceTypeListId = ServiceTypeList.ServiceTypeListid
		LEFT JOIN [dbo].[FormTemplateType] FormTemplateType WITH (NOLOCK) ON FormTemplate.FormTemplateTypeId = FormTemplateType.FormTemplateTypeId
		WHERE
			FormTemplate.AccountTypeListId = @AccountTypeListId AND
			FormTemplate.IsEnable = @IsEnable

	SELECT * FROM #Temp_Table_TemplateByAccountType

	DECLARE @FormTemplateId INT = 0
	SELECT @FormTemplateId = #Temp_Table_TemplateByAccountType.FormTemplateId FROM #Temp_Table_TemplateByAccountType WITH (NOLOCK)

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
			TemplateArea.IsDelete = ~@IsEnable AND
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

	DROP TABLE #Temp_Table_TemplateByAccountType
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