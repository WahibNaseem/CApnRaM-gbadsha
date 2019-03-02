﻿IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplate]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplate]
@AccountTypeListId			int=0,
@ServiceTypeListId			int=0,
@FormTemplateTypeId			int=0,
@FormName					nvarchar(max)=null,
@Description				nvarchar(max)=null,
set @ErrorCode	= 0
BEGIN TRANSACTION Tran_Template_SaveTemplate
BEGIN TRY

IF @ActionType = 'I'
BEGIN

if exists (select 1 from FormTemplate where FormName = @FormName) 
begin
	set @ErrorCode	= 51
	return;
end

INSERT INTO FormTemplate

set @FormTemplateId= IDENT_CURRENT('FormTemplate') 

END
 
IF @ActionType = 'U'
BEGIN

if exists (select 1 from FormTemplate where FormName = @FormName and FormTemplateId != @FormTemplateId) 
begin
	set @ErrorCode	= 51
	return;
end

UPDATE dbo.FormTemplate
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE dbo.FormTemplate
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
 
SELECT FT.*, 
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaId as varchar)+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaIds,
reverse(stuff(reverse((SELECT cast(AI.TemplateQuestionId as varchar)+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as QuestionIds
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT FT.*, 
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaId as varchar)+','  FROM FormTemplateAreaMapping AIM Inner join TemplateArea AI ON AIM.TemplateAreaId = AI.TemplateAreaId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as AreaIds,
reverse(stuff(reverse((SELECT cast(AI.TemplateQuestionId as varchar)+','  FROM FormTemplateQuestionMapping AIM Inner join TemplateQuestion AI ON AIM.TemplateQuestionId = AI.TemplateQuestionId and AIM.FormTemplateId = FT.FormTemplateId  FOR XML PATH (''))), 1, 1, '')) as QuestionIds
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


 



 