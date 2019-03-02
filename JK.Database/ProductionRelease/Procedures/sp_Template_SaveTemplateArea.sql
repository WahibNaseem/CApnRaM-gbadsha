﻿IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplateArea]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplateArea]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplateArea]
set @ErrorCode	= 0
BEGIN TRANSACTION Tran_Template_SaveTemplateArea
BEGIN TRY

IF @ActionType = 'I'
BEGIN

if exists (select 1 from TemplateArea where AreaName = @AreaName) 
begin
	set @ErrorCode	= 51
	return;
end

INSERT INTO TemplateArea

set @TemplateAreaId= IDENT_CURRENT('TemplateArea') 

END
 
IF @ActionType = 'U'
BEGIN

if exists (select 1 from TemplateArea where AreaName = @AreaName and TemplateAreaId != @TemplateAreaId) 
begin
	set @ErrorCode	= 51
	return;
end

UPDATE dbo.TemplateArea
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE dbo.TemplateArea
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	delete from TemplateAreaItemMapping where TemplateAreaId = @TemplateAreaId 
 
	insert into TemplateAreaItemMapping (TemplateAreaId,TemplateAreaItemId,IsEnable,IsDelete,CreatedBy,CreatedOn)	
	select @TemplateAreaId, items, 1,0 , @CreatedBy, Getdate()  from [dbo].[Split] (@ItemIds,',')  where items != '' 

	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN
 
SELECT A.*,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaItemId as varchar)+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemIds
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT A.*,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaItemId as varchar)+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemIds
 --TemplateAreaId = @TemplateAreaId  And 
 IsEnable = 1 and IsDelete = 0
 
END


COMMIT TRANSACTION Tran_Template_SaveTemplateArea

END TRY
BEGIN CATCH
	set @ErrorCode = ERROR_NUMBER()
	set @ErrorMessage = ERROR_MESSAGE()
	ROLLBACK TRANSACTION Tran_Template_SaveTemplateArea
END CATCH  


 