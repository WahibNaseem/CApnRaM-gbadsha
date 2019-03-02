IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[sp_Template_SaveTemplateArea]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[sp_Template_SaveTemplateArea]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Template_SaveTemplateArea]@TemplateAreaId		int ,@ItemIds			nvarchar(max) = NULL,@AreaName           nvarchar(100) = NULL,@IsEnable			bit = NULL,@CreatedBy			int = NULL,@ActionType			VARCHAR(2),@ErrorCode			INT OUTPUT,@ErrorMessage		Nvarchar(max) OUTPUTAS
set @ErrorCode	= 0set @ErrorMessage	=''
BEGIN TRANSACTION Tran_Template_SaveTemplateArea
BEGIN TRY

IF @ActionType = 'I'
BEGIN

if exists (select 1 from TemplateArea where AreaName = @AreaName) 
begin
	set @ErrorCode	= 51	set @ErrorMessage	='This Area name already used, please try with diff name.'
	return;
end

INSERT INTO TemplateArea(AreaName,IsEnable,IsDelete,CreatedBy,CreatedOn)SELECT @AreaName,@IsEnable,0,@CreatedBy,GETdate()

set @TemplateAreaId= IDENT_CURRENT('TemplateArea') 

END
 
IF @ActionType = 'U'
BEGIN

if exists (select 1 from TemplateArea where AreaName = @AreaName and TemplateAreaId != @TemplateAreaId) 
begin
	set @ErrorCode	= 51	set @ErrorMessage	='This TemplateArea name already used, please try with diff name.'
	return;
end

UPDATE dbo.TemplateArea   SET AreaName           = @AreaName,       ModifiedBy     = @CreatedBy,       ModifiedOn     = GETdate() WHERE TemplateAreaId = @TemplateAreaId 
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE dbo.TemplateArea   SET IsDelete = 1,	 ModifiedBy           = @CreatedBy,	 ModifiedOn           = GETDATE()	  WHERE TemplateAreaId = @TemplateAreaId 
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
 
SELECT A.*,reverse(stuff(reverse((SELECT AI.ItemName+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemName,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaItemId as varchar)+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemIdsFROM dbo.TemplateArea A WHERE TemplateAreaId = @TemplateAreaId And IsEnable = 1 and IsDelete = 0
 
END


IF @ActionType = 'SA'
BEGIN
 
SELECT A.*,reverse(stuff(reverse((SELECT AI.ItemName+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemName,
reverse(stuff(reverse((SELECT cast(AI.TemplateAreaItemId as varchar)+','  FROM TemplateAreaItemMapping AIM Inner join TemplateAreaItem AI ON AIM.TemplateAreaItemId = AI.TemplateAreaItemId and AIM.TemplateAreaId = A.TemplateAreaId  FOR XML PATH (''))), 1, 1, '')) as ItemIdsFROM dbo.TemplateArea A WHERE 
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


 