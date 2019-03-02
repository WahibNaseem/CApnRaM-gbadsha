IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplate]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplate]@FormTemplateId				int=0,
@AccountTypeListId			int=0,
@ServiceTypeListId			int=0,
@FormTemplateTypeId			int=0,
@FormName					nvarchar(max)=null,
@Description				nvarchar(max)=null,@AreaIds			nvarchar(max) = NULL,@QuestionIds			nvarchar(max) = NULL,@IsEnable			bit = NULL,@CreatedBy			int = NULL,@ActionType			VARCHAR(2),@ErrorCode			INT OUTPUT,@ErrorMessage		Nvarchar(max) OUTPUTAS
set @ErrorCode	= 0set @ErrorMessage	=''
BEGIN TRANSACTION Tran_Template_SaveTemplate
BEGIN TRY

IF @ActionType = 'I'
BEGIN

if exists (select 1 from FormTemplate where FormName = @FormName) 
begin
	set @ErrorCode	= 51	set @ErrorMessage	='This Template name already used, please try with diff name.'
	return;
end

INSERT INTO FormTemplate(AccountTypeListId,ServiceTypeListId,FormTemplateTypeId,FormName,Description,IsEnable,IsDelete,CreatedBy,CreatedDate)SELECT @AccountTypeListId,@ServiceTypeListId,@FormTemplateTypeId,@FormName,@Description,@IsEnable,0,@CreatedBy,GETdate()

set @FormTemplateId= IDENT_CURRENT('FormTemplate') 

END
 
IF @ActionType = 'U'
BEGIN

if exists (select 1 from FormTemplate where FormName = @FormName and FormTemplateId != @FormTemplateId) 
begin
	set @ErrorCode	= 51	set @ErrorMessage	='This Template name already used, please try with diff name.'
	return;
end

UPDATE dbo.FormTemplate   SET AccountTypeListId=@AccountTypeListId,   ServiceTypeListId=@ServiceTypeListId,   FormTemplateTypeId=@FormTemplateTypeId,   FormName=@FormName,   Description=@Description,       ModifiedBy     = @CreatedBy,       ModifiedDate     = GETdate() WHERE FormTemplateId = @FormTemplateId 
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE dbo.FormTemplate   SET IsDelete = 1,	 ModifiedBy           = @CreatedBy,	 ModifiedDate           = GETDATE()	  WHERE FormTemplateId = @FormTemplateId 
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	delete from FormTemplateAreaMapping where FormTemplateId = @FormTemplateId 
 
	insert into FormTemplateAreaMapping (FormTemplateId,TemplateAreaId,IsEnable,IsDelete,CreatedBy,CreatedOn)	
	select @FormTemplateId, items, 1,0 , @CreatedBy, Getdate()  from [dbo].[Split] (@AreaIds,',')  where items != '' 

	delete from FormTemplateQuestionMapping where FormTemplateId = @FormTemplateId 
 
	insert into FormTemplateQuestionMapping (FormTemplateId,TemplateQuestionId,IsEnable,IsDelete,CreatedBy,CreatedOn)	
	select @FormTemplateId, items, 1,0 , @CreatedBy, Getdate()  from [dbo].[Split] (@QuestionIds,',')  where items != '' 

	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN
 
SELECT FT.*, AT.Name as AccountTypeName, ST.Name as ServiceTypeName, FTT.Name as TemplateTypeName,reverse(stuff(reverse((SELECT AI.AreaName+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaName,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaId as varchar)+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaIds,reverse(stuff(reverse((SELECT AI.Question+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as Question,
reverse(stuff(reverse((SELECT cast(AI.TemplateQuestionId as varchar)+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as QuestionIdsFROM dbo.FormTemplate FTInner join AccountTypeList AT ON AT.AccountTypeListId = FT.AccountTypeListIdInner join ServiceTypeList ST ON ST.ServiceTypeListId = FT.ServiceTypeListIdInner join FormTemplateType FTT ON FTT.FormTemplateTypeId = FT.FormTemplateTypeId WHERE FormTemplateId = @FormTemplateId And FT.IsEnable = 1 and FT.IsDelete = 0
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT FT.*, AT.Name as AccountTypeName, ST.Name as ServiceTypeName, FTT.Name as TemplateTypeName,reverse(stuff(reverse((SELECT AI.AreaName+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaName,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaId as varchar)+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaIds,reverse(stuff(reverse((SELECT AI.Question+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as Question,
reverse(stuff(reverse((SELECT cast(AI.TemplateQuestionId as varchar)+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as QuestionIdsFROM dbo.FormTemplate FTInner join AccountTypeList AT ON AT.AccountTypeListId = FT.AccountTypeListIdInner join ServiceTypeList ST ON ST.ServiceTypeListId = FT.ServiceTypeListIdInner join FormTemplateType FTT ON FTT.FormTemplateTypeId = FT.FormTemplateTypeId WHERE  --FormTemplateId = @FormTemplateId And 
 FT.IsEnable = 1 and FT.IsDelete = 0
 ORder By FormTemplateId desc
 
END


COMMIT TRANSACTION Tran_Template_SaveTemplate

END TRY
BEGIN CATCH
	set @ErrorCode = ERROR_NUMBER()
	set @ErrorMessage = ERROR_MESSAGE()
	ROLLBACK TRANSACTION Tran_Template_SaveTemplate
END CATCH  


 



 