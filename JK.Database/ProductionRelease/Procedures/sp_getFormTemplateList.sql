IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplate]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_getFormTemplateList]
	@FormTemplateId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--GET ALL Template List
	Select DISTINCT
	FT.FormTemplateId,
	FT.AccountTypeListId,
	FT.ServiceTypeListId,
	FT.FormTemplateTypeId,
	FT.FormName,
	FT.Description,
	ATL.Name as AccountTypeList,
	STL.Name as ServiceTypeList,
	FTT.Name as FormTemplateType
	From [dbo].[FormTemplate] FT 
	inner join AccountTypeList ATL ON ATL.AccountTypeListId = FT.AccountTypeListId
	inner join ServiceTypeList STL ON STL.ServiceTypeListId = FT.ServiceTypeListId
	inner join FormTemplateType FTT ON FTT.FormTemplateTypeId = FT.FormTemplateTypeId
	where FT.IsEnable = 1 and FT.IsDelete = 0 and (@FormTemplateId = 0 OR FT.FormTemplateId = @FormTemplateId)

	--GET ALL AREA List
	select DISTINCT
	FTAM.FormTemplateAreaMappingId,
	FTAM.FormTemplateId,
	FTAM.TemplateAreaId,
	TA.AreaName
	from 
	[dbo].[FormTemplateAreaMapping] FTAM
	INNER JOIN [dbo].[TemplateArea] TA ON TA.TemplateAreaId = FTAM.TemplateAreaId
	where FTAM.IsEnable = 1 and FTAM.IsDelete = 0 and (@FormTemplateId = 0 OR FTAM.FormTemplateId = @FormTemplateId)


	--GET ALL Item List
	Select DISTINCT
	TAIM.TemplateAreaId,
	TAI.TemplateAreaItemId,
	TAI.ItemName,
	TAI.FormItemType,
	TAI.FormItemValue,
	TAI.IsDirty,
	TAI.IsRequired
	FROM [TemplateAreaItemMapping] TAIM 
	inner join [TemplateAreaItem] TAI ON TAI.TemplateAreaItemId = TAIM.TemplateAreaItemId
	where TAIM.IsEnable = 1 and TAIM.IsDelete = 0 --and (@FormTemplateId = 0 OR TAIM.FormTemplateId = @FormTemplateId)

END
