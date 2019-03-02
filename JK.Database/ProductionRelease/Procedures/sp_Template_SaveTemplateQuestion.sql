IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplateQuestion]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplateQuestion]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplateQuestion]
@TemplateQuestionId		int ,
@Question           nvarchar(100) = NULL,
@QuestionType           nvarchar(100) = NULL,
@IsEnable			bit = NULL,
@CreatedBy			int = NULL,
@ActionType			VARCHAR(2),
@ErrorCode			INT OUTPUT,
@ErrorMessage		Nvarchar(max) OUTPUT
AS

set @ErrorCode	= 0
set @ErrorMessage	=''

BEGIN TRANSACTION Tran_Template_Question
BEGIN TRY

IF @ActionType = 'I'
BEGIN


INSERT INTO TemplateQuestion
(Question,QuestionType,IsEnable,IsDelete,CreatedBy,CreatedOn)
SELECT @Question,@QuestionType,@IsEnable,0,@CreatedBy,GETdate()

set @TemplateQuestionId= IDENT_CURRENT('TemplateQuestion') 

END
 
IF @ActionType = 'U'
BEGIN


UPDATE TemplateQuestion
   SET Question           = @Question,
	   QuestionType	  = @QuestionType,
       ModifiedBy     = @CreatedBy,
       ModifiedOn     = GETdate()
 WHERE TemplateQuestionId = @TemplateQuestionId
 
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE TemplateQuestion
   SET 
   IsDelete = 1,
       ModifiedBy     = @CreatedBy,
       ModifiedOn     = GETdate()
 WHERE TemplateQuestionId = @TemplateQuestionId
 
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN
 
SELECT * FROM dbo.TemplateQuestion
 WHERE TemplateQuestionId = @TemplateQuestionId
 And IsEnable = 1 and IsDelete = 0
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT * FROM dbo.TemplateQuestion
 WHERE 
 --TemplateQuestionId = @TemplateQuestionId And 
 IsEnable = 1 and IsDelete = 0
 
 
END


COMMIT TRANSACTION Tran_Template_Question

END TRY
BEGIN CATCH
	set @ErrorCode = ERROR_NUMBER()
	set @ErrorMessage = ERROR_MESSAGE()
	ROLLBACK TRANSACTION Tran_Template_Question
END CATCH  