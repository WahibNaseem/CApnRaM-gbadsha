IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplateItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplateItem]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplateItem] @TemplateItemId		int,@ItemName           nvarchar(100) = NULL,@FormItemType       int = 0,@IsEnable			bit = NULL,@CreatedBy			int = NULL,@ActionType			VARCHAR(2),@ErrorCode			INT OUTPUT,@ErrorMessage		Nvarchar(max) OUTPUTAS
set @ErrorCode	= 0set @ErrorMessage	=''
BEGIN TRANSACTION Tran_Template_SaveTemplateItem
BEGIN TRY

IF @ActionType = 'I'
BEGIN
 
INSERT INTO TemplateAreaItem(ItemName,IsEnable,IsDelete,CreatedBy,CreatedOn,FormItemType,FormItemValue,IsDirty,IsRequired)SELECT @ItemName,@IsEnable,0,@CreatedBy,GETdate(),@FormItemType,
 CASE WHEN @FormItemType = 40 THEN
'{"Items": [],"Label": "'+@ItemName+'","Rating": 1,"Status": 0,"Text": "","ShowText": 0,"ShowItems": 0}'
ELSE '{"Label":"'+@ItemName+'","Status":0,"Text":""}' END, 0,0

set @TemplateItemId= IDENT_CURRENT('TemplateAreaItem') 

END
 
IF @ActionType = 'U'
BEGIN
 

UPDATE TemplateAreaItem   SET ItemName           = @ItemName,	   FormItemType	  = @FormItemType,	   FormItemValue  = CASE WHEN @FormItemType = 40 THEN
'{"Items": [],"Label": "'+@ItemName+'","Rating": 1,"Status": 0,"Text": "","ShowText": 0,"ShowItems": 0}'
ELSE '{"Label":"'+@ItemName+'","Status":0,"Text":""}' END,	   	   --IsDirty		  = IsDirty,	   --IsRequired	  = IsRequired,       ModifiedBy     = @CreatedBy,       ModifiedOn     = GETdate() WHERE TemplateAreaItemId = @TemplateItemId 
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE TemplateAreaItem   SET IsDelete = 1,	 ModifiedBy           = @CreatedBy,	 ModifiedOn           = GETDATE()	  WHERE TemplateAreaItemId = @TemplateItemId 
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN
 
SELECT * FROM dbo.TemplateAreaItem WHERE TemplateAreaItemId = @TemplateItemId And IsEnable = 1 and IsDelete = 0
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT * FROM dbo.TemplateAreaItem WHERE 
 --TemplateAreaId = @TemplateItemId  And 
 IsEnable = 1 and IsDelete = 0
 
END


COMMIT TRANSACTION Tran_Template_SaveTemplateItem

END TRY
BEGIN CATCH
	set @ErrorCode = ERROR_NUMBER()
	set @ErrorMessage = ERROR_MESSAGE()
	ROLLBACK TRANSACTION Tran_Template_SaveTemplateItem
END CATCH  