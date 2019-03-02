﻿IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplateItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplateItem]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplateItem] 
set @ErrorCode	= 0
BEGIN TRANSACTION Tran_Template_SaveTemplateItem
BEGIN TRY

IF @ActionType = 'I'
BEGIN
 
INSERT INTO TemplateAreaItem
 CASE WHEN @FormItemType = 40 THEN
'{"Items": [],"Label": "'+@ItemName+'","Rating": 1,"Status": 0,"Text": "","ShowText": 0,"ShowItems": 0}'
ELSE '{"Label":"'+@ItemName+'","Status":0,"Text":""}' END, 0,0

set @TemplateItemId= IDENT_CURRENT('TemplateAreaItem') 

END
 
IF @ActionType = 'U'
BEGIN
 

UPDATE TemplateAreaItem
'{"Items": [],"Label": "'+@ItemName+'","Rating": 1,"Status": 0,"Text": "","ShowText": 0,"ShowItems": 0}'
ELSE '{"Label":"'+@ItemName+'","Status":0,"Text":""}' END,
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE TemplateAreaItem
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN
 
SELECT * FROM dbo.TemplateAreaItem
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT * FROM dbo.TemplateAreaItem
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