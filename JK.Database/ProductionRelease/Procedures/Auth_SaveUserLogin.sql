/*****************************************************************************************
[dbo].[Auth_SaveUserLogin] 

Purpose:  This stored procedure allows to add/edit the FMS user.

Change History
========================
12/05/2017 RP - Add the stored procedure

*****************************************************************************************/

IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[Auth_SaveUserLogin]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[Auth_SaveUserLogin]
GO

USE [jkBuf]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Auth_SaveUserLogin] 
(
	@UserId					int=null,
	@UserName				varchar(200) = NULL,
	@PasswordHash			nvarchar(500) = NULL,
	@IsFirstTimeLogin		datetime = NULL,
	@GroupId				int = NULL,
	@FirstName				varchar(100) = NULL,
	@LastName				varchar(100) = NULL,
	@Email					varchar(200) = NULL,
	@Phone					varchar(100) = NULL,
	@Addres					varchar(200) = NULL,
	@City					varchar(100) = NULL,
	@State					varchar(100) = NULL,
	@Zipcode				varchar(10) = NULL,
	@DeparmentId			int = NULL,
	@Title					varchar(100) = NULL,
	@IsEnable				bit = NULL,
	@CreatedBy				int = NULL,
	@ActionType				VARCHAR(1)='S',
	@RoleIds				nvarchar(max) = null,
	@RegionIds				nvarchar(max) = null,
	@OutlookUsername		nvarchar(max) = null,
	@OutlookPassword		nvarchar(max) = null,
	@ErrorCode				INT OUTPUT,
	@ErrorMessage			Nvarchar(max) OUTPUT
)
AS

set @ErrorCode	= 0
set @ErrorMessage	=''

DECLARE @Salt UNIQUEIDENTIFIER=NEWID(); 

Set @PasswordHash = HASHBYTES('SHA2_512', @PasswordHash + CAST(@Salt AS NVARCHAR(36)))

IF @ActionType = 'I'
BEGIN

if exists (select 1 from AuthUserLogin where Username = @UserName) 
begin
	set @ErrorCode	= 51
	set @ErrorMessage	='This username already used, please try with diff name.'
	return;
end

INSERT INTO dbo.AuthUserLogin
(UserName,PasswordHash,IsFirstTimeLogin,Salt,GroupId,FirstName,LastName,Email,Phone,Addres,City,State,Zipcode,DeparmentId,Title,IsEnable,IsDelete,CreatedBy,CreatedOn,OutlookUsername, OutlookPassword)
SELECT @UserName,@PasswordHash,@IsFirstTimeLogin,@Salt,@GroupId,@FirstName,@LastName,@Email,@Phone,@Addres,@City,@State,@Zipcode,@DeparmentId,@Title,@IsEnable,0,@CreatedBy,getdate(),@OutlookUsername,@OutlookPassword

SET @UserId= IDENT_CURRENT('AuthUserLogin') 

END
 
IF @ActionType = 'U'
BEGIN

if exists (select 1 from AuthUserLogin where Username = @UserName and UserId != @UserId) 
begin
	set @ErrorCode	= 51
	set @ErrorMessage	='This username already used, please with try diff name.'
	return;
end

UPDATE dbo.AuthUserLogin
   SET UserName             = @UserName,
       --PasswordHash         = @PasswordHash,
       IsFirstTimeLogin     = @IsFirstTimeLogin,
       --Salt                 = @Salt,
       GroupId              = @GroupId,
       FirstName            = @FirstName,
       LastName             = @LastName,
       Email                = @Email,
       Phone                = @Phone,
       Addres               = @Addres,
       City                 = @City,
       State                = @State,
       Zipcode              = @Zipcode,
       DeparmentId          = @DeparmentId,
       Title                = @Title,
       IsEnable             = @IsEnable,
       ModifiedBy           = @CreatedBy,
       ModifiedOn           = GETDATE(),
	   OutlookUsername		= @OutlookUsername,
	   OutlookPassword		= @OutlookPassword
 WHERE UserId = @UserId
 
 
END
 
IF @ActionType = 'D'
BEGIN
 
UPDATE dbo.AuthUserLogin
   SET IsDelete = 1,
	 ModifiedBy           = @CreatedBy,
	 ModifiedOn           = GETDATE()
    WHERE UserId = @UserId
 
 
END

IF @ActionType = 'U' or @ActionType = 'I'
BEGIN
	delete from AuthUserRegion where UserId = @UserId 

	delete from AuthUserRole where UserId = @UserId

	insert into AuthUserRole (UserId,RoleId,IsEnable,IsDelete,CreatedBy,CreatedOn)	
	select @UserId, items, 1,0 , @CreatedBy, Getdate()  from [dbo].[Split] (@RoleIds,',') where items != '' 

	insert into AuthUserRegion (UserId,RegionId,IsEnable,IsDelete,CreatedBy,CreatedOn)	
	select @UserId, IIF(items=0,null,items), 1,0 , @CreatedBy, Getdate()  from [dbo].[Split] (@RegionIds,',')  where items != '' 

	SET @ActionType ='S'
END



IF @ActionType = 'S'
BEGIN

set @RegionIds = (SELECT cast(IIF(RegionId is null,0,RegionId) as varchar)+','  FROM AuthUserRegion where UserId =  @UserId FOR XML PATH (''))

set @RoleIds = (SELECT cast(RoleId as varchar)+','  FROM AuthUserRole where UserId =  @UserId FOR XML PATH (''))

SELECT UserId, UserName,PasswordHash,IsFirstTimeLogin,Salt,GroupId,FirstName,LastName,Email,Phone as PhoneNumber,Addres as Address,City,State,Zipcode,DeparmentId,Title,IsEnable,IsDelete,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,@RegionIds as RegionIds, @RoleIds as RoleIds, 
OutlookUsername, OutlookPassword
FROM dbo.AuthUserLogin
WHERE UserId = @UserId
And IsEnable = 1 and IsDelete = 0
 
END
