IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[CRM_spGet_PotentialList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[CRM_spGet_PotentialList] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CRM_spGet_PotentialList]  
(
	@type int = 0,  
 	@region VARCHAR(50) = 2,  
 	@user int = 0,  
 	@choice int = 0,
 	@LoginUserId INT = 8,
 	@PageNo INT = 1,
	@PageSize INT = 10
)
AS 
BEGIN  

 	SET NOCOUNT ON;  
  
	CREATE TABLE #USER( USERID INT)

	IF(ISNULL(@user,0) = 0)
	BEGIN
	PRINT '1'
		INSERT INTO #USER
		SELECT UserId FROM fn_GetUserRelationHierarchy(@LoginUserId,@region) REG
		WHERE REG.USERID = @LoginUserId OR (REG.REGIONID IN (SELECT CAST(ITEMS AS INT) FROM Split(@region,',')) OR ISNULL(@region,'')='')
	END
	ELSE 
	BEGIN 
		IF(ISNULL(@user,0) = ISNULL(@LoginUserId,0))
		BEGIN
		PRINT '2'
			INSERT INTO #USER
			SELECT UserId FROM fn_GetUserRelationHierarchy(@LoginUserId,@region) REG
			WHERE (REG.REGIONID IN (SELECT CAST(ITEMS AS INT) FROM Split(@region,',')) OR ISNULL(@region,'')='')
		END
		ELSE 
		BEGIN 
		PRINT '3'
			INSERT INTO #USER
			SELECT UserId FROM fn_GetUserRelationHierarchy(@LoginUserId,@region) REG WHERE Userid = @User
			AND (REG.REGIONID IN (SELECT CAST(ITEMS AS INT) FROM Split(@region,',')) OR ISNULL(@region,'')='')
		END
	END

	IF @PageNo <= 1 SET @PageNo = 1
  	IF @PageSize <= 0 SET @PageSize = 10

	--QUERY TO GET ALL QUALIFIED LEAD   
 	IF(@choice = 1)  
  	BEGIN  
	    SELECT 
	    	AUL.FirstName AS FName, 
	    	AUL.LastName AS LName,
	    	AUL.UserName,
	    	AUL.Email,  
	    	ACT.CRM_AccountId, 
	    	ACT.assigneeId, 
	    	ACT.Regionid, 
	    	CB.CRM_BiddingId, 
	    	CC.CRM_CloseId, 
	    	ST.NAME AS StageStatusName, 
	    	ACT.StageStatus, 
	    	ACT.ContactName as Firstname, 
	    	ACT.PhoneNumber, 
	    	ACTCUSDTL.CRM_AccountCustomerDetailId, 
	    	ACTCUSDTL.CompanyName,
	  		ACTCUSDTL.Title, 
	  		ACTCUSDTL.SalesVolume, 
	  		ACTCUSDTL.LineofBusiness, 
	  		ACTCUSDTL.SqFt, 
	  		ACTCUSDTL.AccountTypeListId, 
	  		ACTCUSDTL.SpokeWITH,ACTCUSDTL.CRM_CallResultId, 
	  		ACTCUSDTL.CallBack, ACTCUSDTL.CRM_NoteTypeId, 
	  		ACTCUSDTL.CRM_SalePossibilityTypeId, 
	  		ACTCUSDTL.ContractExpire, IT.Name AS AccountTypeName , 
	  		ACTCUSDTL.NumberOfLocations, 
	  		ACTCUSDTL.BudgetAmount,CB.MonthlyPrice, 
	  		CC.ContractAmount,
	  		[dbo].[fn_CalculatingTimeDifference] (ACT.createdDate,GETDATE(),'left') as LeftDay
		FROM CRM_ACCOUNT AS ACT WITH(NOLOCK)
	    JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID AND ACT.STAGE = 1 AND ACT.ACCOUNTTYPE = 1 AND ACT.STAGESTATUS = 5 	
	    JOIN CRM_STAGESTATUS AS ST ON ST.TYPE = ACT.STAGESTATUS 
	    LEFT  JOIN AccountTypeList AS IT ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId 
	    LEFT JOIN CRM_Bidding  AS CB ON CB.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID
	    LEFT JOIN CRM_Close  AS CC ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID 
		LEFT JOIN [AuthUserLogin] AUL ON ACT.ASSIGNEEID = AUL.USERID 	
		LEFT JOIN CRM_Territory_Assignment_New AS CTAN ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId 
		WHERE (CTN.REGIONID = @region OR @region = 0 OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') 
		ORDER BY ACT.CRM_AccountId DESC
		OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
  	END  
  
  	--QUERY TO GET UNQUALIFIED LEAD  LIST 
  	ELSE IF(@choice = 2)  
  	BEGIN  
		SELECT 
			AUL.FirstName As FName, 
			AUL.LastName as LName,
			AUL.UserName,AUL.Email,  
			ACT.CRM_AccountId, 
			ACT.assigneeId, 
			ACT.Regionid, 
			CB.CRM_BiddingId, 
			CC.CRM_CloseId,
			ST.NAME AS StageStatusName, 
			ACT.StageStatus, 
			ACT.ContactName as Firstname, 
			ACT.PhoneNumber,
			ACTCUSDTL.CRM_AccountCustomerDetailId, 
			ACTCUSDTL.CompanyName, 
			ACTCUSDTL.Title, 
			ACTCUSDTL.SalesVolume, 
			ACTCUSDTL.LineofBusiness, 
			ACTCUSDTL.SqFt, 
			ACTCUSDTL.AccountTypeListId, 
			ACTCUSDTL.SpokeWITH,
			ACTCUSDTL.CRM_CallResultId, 
			ACTCUSDTL.CallBack,  
			ACTCUSDTL.CRM_NoteTypeId,
			ACTCUSDTL.CRM_SalePossibilityTypeId, 
			ACTCUSDTL.ContractExpire, 
			IT.Name AS AccountTypeName, 
			ACTCUSDTL.NumberOfLocations, 
			ACTCUSDTL.BudgetAmount,
			CB.MonthlyPrice, 
			CC.ContractAmount, 
			[dbo].[fn_CalculatingTimeDifference] (ACT.createdDate,GETDATE(),'left') as LeftDay
	    FROM CRM_ACCOUNT AS ACT   WITH(NOLOCK)
		JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID  
		AND ACT.ACCOUNTTYPE = 1 AND ( ACT.STAGESTATUS = 3 OR ACT.STAGESTATUS = 4)
		JOIN CRM_STAGESTATUS AS ST ON ST.TYPE = ACT.STAGESTATUS  
		LEFT  JOIN AccountTypeList AS IT ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId  
		Left JOIN CRM_Bidding  AS CB ON CB.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID  AND ISNULL(CB.IsActive,0) = 1 
		Left JOIN CRM_Close  AS CC ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID  AND ISNULL(CC.IsActive,0) = 1 
		LEFT JOIN [AuthUserLogin] AUL ON ACT.ASSIGNEEID = AUL.USERID
		LEFT JOIN CRM_Territory_Assignment_New AS CTAN ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId  
	    WHERE (CTN.REGIONID = @region OR @region = 0 OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') 
	    ORDER BY ACT.CRM_AccountId DESC
	    OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
	
  	END  
  
  	--QUERY TO GET ALL POTENTIAL LEAD LIST
    ELSE IF(@choice = 3)  
    BEGIN  
       
		DECLARE @potential INT=NULL,@fvpresentation INT = NULL, @bidding INT = NULL, @pdappointment INT = NULL, @followup INT = NULL,
		@close INT = NULL,@callback INT = NULL,@leadgeneration INT = NULL, @franchisedisclosure INT = NULL,@signagreement  INT = NULL,@sold INT = NULL

		SELECT TOP(1) @potential = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 20 				--20	potential
		SELECT TOP(1) @fvpresentation = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 21 		--21	fvpresentation
		SELECT TOP(1) @bidding = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 22 				--22	bidding
		SELECT TOP(1) @pdappointment = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 23			--23	pdappointment
		SELECT TOP(1) @followup = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 24				--24	followup
		SELECT TOP(1) @close = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 25					--25	close
		SELECT TOP(1) @callback = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 26				--26	callback
		SELECT TOP(1) @leadgeneration = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 27			--27	leadgeneration	
		SELECT TOP(1) @franchisedisclosure = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 28	--28	franchisedisclosure
		SELECT TOP(1) @signagreement = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 29			--29	signagreement
		SELECT TOP(1) @sold = (DayLeft*24*60) + (HourLeft*60) + MinuteLeft FROM [dbo].[CRM_StageDateCalculation] WHERE CRM_StageStatusId = 30					--30	sold

	    SELECT 
	    	AUL.FirstName As FName, 
	    	AUL.LastName as LName,
	    	AUL.UserName,AUL.Email,  
	    	ACT.CRM_AccountId, 
	    	ACT.assigneeId, 
	    	ACT.Regionid, 
	    	CB.CRM_BiddingId, 
	    	CC.CRM_CloseId, 
	    	ST.NAME AS StageStatusName, 
	    	ACT.StageStatus, 
	    	ACT.ContactName as Firstname, 
	    	ACT.PhoneNumber, 
	    	ACTCUSDTL.CRM_AccountCustomerDetailId, 
	    	ACTCUSDTL.CompanyName, 
	    	ACTCUSDTL.Title, 
	    	ACTCUSDTL.SalesVolume, 
	    	ACTCUSDTL.LineofBusiness, 
	    	ACTCUSDTL.SqFt, 
	    	ACTCUSDTL.AccountTypeListId, 
	    	ACTCUSDTL.SpokeWITH,
	    	ACTCUSDTL.CRM_CallResultId,
	    	ACTCUSDTL.CallBack,  
	    	ACTCUSDTL.CRM_NoteTypeId, 
	    	ACTCUSDTL.CRM_SalePossibilityTypeId, 
	    	ACTCUSDTL.ContractExpire, IT.Name AS AccountTypeName, 
	    	ACTCUSDTL.NumberOfLocations, 
	    	ACTCUSDTL.BudgetAmount,
	    	CB.MonthlyPrice, 
	    	CC.ContractAmount,
	    	[dbo].[fn_CalculatingTimeDifference] (
	    		getdate(), 
	    		DATEADD(MINUTE,
				CASE 
				WHEN ACT.STAGESTATUS = 20 THEN @potential 
				WHEN ACT.STAGESTATUS = 21 THEN @fvpresentation 
				WHEN ACT.STAGESTATUS = 22 THEN @bidding 
				WHEN ACT.STAGESTATUS = 23 THEN @pdappointment 
				WHEN ACT.STAGESTATUS = 24 THEN @followup 
				WHEN ACT.STAGESTATUS = 25 THEN @close 
				WHEN ACT.STAGESTATUS = 26 THEN @callback 
				WHEN ACT.STAGESTATUS = 27 THEN @leadgeneration 
				WHEN ACT.STAGESTATUS = 28 THEN @franchisedisclosure 
				WHEN ACT.STAGESTATUS = 29 THEN @signagreement 
				WHEN ACT.STAGESTATUS = 30 THEN @sold 
				ELSE 0 END,
				isnull(ACT.ModifiedDate,GETDATE())),
				'left'
			) as LeftDay
		FROM CRM_ACCOUNT AS ACT  WITH(NOLOCK)
	    JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID 
		AND ACT.STAGE = 2 AND ACCOUNTTYPE = 1 --AND( ACT.ASSIGNEEID = @user OR @user = 0)
	    JOIN CRM_STAGESTATUS AS ST ON ST.TYPE = ACT.STAGESTATUS 
		AND  (ST.TYPE = @type OR @type = 0) 	 
	    LEFT  JOIN AccountTypeList AS IT ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId 
	    LEFT JOIN CRM_Bidding  AS CB ON CB.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID AND ISNULL(CB.IsActive,0) = 1  
	    LEFT JOIN CRM_Close  AS CC ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID   AND ISNULL(CC.IsActive,0) = 1 
		LEFT JOIN [AuthUserLogin] AUL ON ACT.ASSIGNEEID = AUL.USERID 
		LEFT JOIN CRM_Territory_Assignment_New AS CTAN ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId  
	    WHERE (CTN.REGIONID IN (SELECT CAST(ITEMS AS INT) FROM SPLIT(@region,',')) OR ISNULL(@region,'') = '' OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') AND (ACT.ASSIGNEEID IN (SELECT * FROM #USER)) 
	    ORDER BY ACT.CRM_AccountId DESC
	    OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
  	END 

   	--QUERY TO GET Close LEAD LIST 
  	ELSE IF(@choice = 4)  
  	BEGIN  
   
	    SELECT 
	    	AUL.FirstName As FName, 
	    	AUL.LastName as LName,
	    	AUL.UserName,AUL.Email, 
	    	ACT.CRM_AccountId, 
	    	ACT.assigneeId, 
	    	ACT.Regionid, 
	    	CB.CRM_BiddingId, 
	    	CC.CRM_CloseId, 
	    	ST.NAME AS StageStatusName, 
	    	ACT.StageStatus, 
	    	ACT.ContactName as Firstname, 
	    	ACT.PhoneNumber, 
	    	ACTCUSDTL.CRM_AccountCustomerDetailId, 
	    	ACTCUSDTL.CompanyName,
	    	ACTCUSDTL.Title, 
	    	ACTCUSDTL.SalesVolume, 
	    	ACTCUSDTL.LineofBusiness, 
	    	ACTCUSDTL.SqFt, 
	    	ACTCUSDTL.AccountTypeListId, 
	    	ACTCUSDTL.SpokeWITH,
	    	ACTCUSDTL.CRM_CallResultId,
	    	ACTCUSDTL.CallBack, 
	    	ACTCUSDTL.CRM_NoteTypeId, 
	    	ACTCUSDTL.CRM_SalePossibilityTypeId, 
	    	ACTCUSDTL.ContractExpire, 
	    	IT.Name AS AccountTypeName, 
	    	ACTCUSDTL.NumberOfLocations, 
	    	ACTCUSDTL.BudgetAmount,
	    	CB.MonthlyPrice, 
	    	CC.ContractAmount,
	    	[dbo].[fn_CalculatingTimeDifference] (ACT.createdDate,GETDATE(),'left') as LeftDay
	    FROM CRM_ACCOUNT AS ACT  WITH(NOLOCK)
	    JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID  
		AND ACT.STAGE = 3 AND ACT.ACCOUNTTYPE = 1 AND ( ACT.STAGESTATUS = 25)
	    JOIN CRM_STAGESTATUS AS ST ON ST.TYPE = ACT.STAGESTATUS  	
	    LEFT  JOIN AccountTypeList AS IT ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId  
	    LEFT JOIN CRM_Bidding  AS CB ON CB.CRM_BiddingId = (SELECT TOP 1 CRM_BiddingId FROM CRM_Bidding WHERE CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID AND ISNULL(IsActive,0) = 1)
	    LEFT JOIN CRM_Close  AS CC ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID  AND ISNULL(CC.IsActive,0) = 1 
		LEFT JOIN [AuthUserLogin] AUL ON ACT.ASSIGNEEID = AUL.USERID
		LEFT JOIN CRM_Territory_Assignment_New AS CTAN ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId  
	    WHERE (CTN.REGIONID = @region OR @region = 0 OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') 
	    ORDER BY ACT.CRM_AccountId DESC
	    OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
  	END 

  	--QUERY TO GET CALL BACK LEAD LIST
  	ELSE IF(@choice = 5)
  	BEGIN
  		SELECT 
  			AUL.FirstName As FName, 
	  		AUL.LastName as LName,
	  		AUL.UserName,
	  		AUL.Email, 
	  		ACT.CRM_AccountId, 
	  		ACT.assigneeId, 
	  		ACT.Regionid, 
	  		CB.CRM_BiddingId, 
	  		CC.CRM_CloseId, 
	  		ST.NAME AS StageStatusName, 
	  		ACT.StageStatus, 
	  		ACT.ContactName as Firstname, 
	  		ACT.PhoneNumber, 
	  		ACTCUSDTL.CRM_AccountCustomerDetailId, 
	  		ACTCUSDTL.CompanyName,
	    	ACTCUSDTL.Title, 
	    	ACTCUSDTL.SalesVolume, 
	    	ACTCUSDTL.LineofBusiness, 
	    	ACTCUSDTL.SqFt, 
	    	ACTCUSDTL.AccountTypeListId, 
	    	ACTCUSDTL.SpokeWITH,
	    	ACTCUSDTL.CRM_CallResultId,
	    	ACTCUSDTL.CallBack,  
	    	ACTCUSDTL.CRM_NoteTypeId, 
	    	ACTCUSDTL.CRM_SalePossibilityTypeId, 
	    	ACTCUSDTL.ContractExpire, 
	    	IT.Name AS AccountTypeName, 
	    	ACTCUSDTL.NumberOfLocations, 
	    	ACTCUSDTL.BudgetAmount,CB.MonthlyPrice, 
	    	CC.ContractAmount,[dbo].[fn_CalculatingTimeDifference] (ACT.createdDate,GETDATE(),'left') as LeftDay
	    FROM CRM_ACCOUNT AS ACT  WITH(NOLOCK)
	    JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID  
		AND ACT.STAGE = 1 AND ACT.ACCOUNTTYPE = 1 AND ( ACT.STAGESTATUS = 26)	 
	    JOIN CRM_STAGESTATUS AS ST ON ST.TYPE = ACT.STAGESTATUS   
	    LEFT  JOIN AccountTypeList AS IT ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId  
	    Left JOIN CRM_Bidding  AS CB ON CB.CRM_BiddingId = (SELECT TOP 1 CRM_BiddingId FROM CRM_Bidding WHERE CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID AND ISNULL(IsActive,0) = 1)
	    Left JOIN CRM_Close  AS CC ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID  AND ISNULL(CC.IsActive,0) = 1 
		LEFT JOIN [AuthUserLogin] AUL ON ACT.ASSIGNEEID = AUL.USERID    
		LEFT JOIN CRM_Territory_Assignment_New AS CTAN ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId  
		WHERE (CTN.REGIONID = @region OR @region = 0 OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') 
		ORDER BY ACT.CRM_AccountId DESC
		OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
  	END 
  
  	--QUERY TO GET ALL NEW LEAD LIST 
	ELSE   
  	BEGIN  
	    SELECT 
	    	AUL.FirstName As FName, 
	    	AUL.LastName as LName,
	    	AUL.UserName,
	    	AUL.Email, 
	    	ACT.CRM_AccountId, 
	    	ACT.assigneeId, 
	    	ACT.Regionid, 
	    	CB.CRM_BiddingId, 
	    	CC.CRM_CloseId, 
	    	ST.NAME AS StageStatusName, 
	    	ACT.StageStatus, 
	    	ACT.ContactName as Firstname, 
	    	ACT.PhoneNumber, 
	    	ACTCUSDTL.CRM_AccountCustomerDetailId, 
	    	ACTCUSDTL.CompanyName,
	    	ACTCUSDTL.Title, 
	    	ACTCUSDTL.SalesVolume, 
	    	ACTCUSDTL.LineofBusiness, 
	    	ACTCUSDTL.SqFt, 
	    	ACTCUSDTL.AccountTypeListId, 
	    	ACTCUSDTL.SpokeWITH,
	    	ACTCUSDTL.CRM_CallResultId,
	    	ACTCUSDTL.CallBack, 

	    	ACTCUSDTL.CRM_NoteTypeId, 
	    	ACTCUSDTL.CRM_SalePossibilityTypeId, 
	    	ACTCUSDTL.ContractExpire, 
	    	IT.Name AS AccountTypeName, 
	    	ACTCUSDTL.NumberOfLocations, 
	    	ACTCUSDTL.BudgetAmount,
	    	CB.MonthlyPrice, 
	    	CC.ContractAmount,
	    	[dbo].[fn_CalculatingTimeDifference] (ACT.createdDate,GETDATE(),'left') as LeftDay
		FROM CRM_ACCOUNT AS ACT  WITH(NOLOCK)
	    INNER JOIN CRM_ACCOUNTCUSTOMERDETAIL AS ACTCUSDTL WITH(NOLOCK) ON ACT.CRM_ACCOUNTID = ACTCUSDTL.CRM_ACCOUNTID AND ACT.STAGE = 1 AND ACT.STAGESTATUS = 1 AND ACT.ACCOUNTTYPE = 1    
	    JOIN CRM_STAGESTATUS AS ST WITH(NOLOCK) ON ST.TYPE = ACT.STAGESTATUS
	    LEFT  JOIN AccountTypeList AS IT WITH(NOLOCK) ON IT.AccountTypeListId= ACTCUSDTL.AccountTypeListId  
	    LEFT JOIN CRM_Bidding  AS CB WITH(NOLOCK) ON CB.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID AND ISNULL(CB.IsActive,0) = 1 	 
	    LEFT JOIN CRM_Close  AS CC WITH(NOLOCK) ON CC.CRM_ACCOUNTCUSTOMERDETAILID = ACTCUSDTL.CRM_ACCOUNTCUSTOMERDETAILID AND ISNULL(CC.IsActive,0) = 1 
		LEFT JOIN [AuthUserLogin] AUL WITH(NOLOCK) ON ACT.ASSIGNEEID = AUL.USERID  	
	    LEFT JOIN CRM_Territory_Assignment_New AS CTAN WITH(NOLOCK) ON CTAN.ZipCode = ACTCUSDTL.CompanyZipCode  
		LEFT JOIN CRM_Territory_New AS CTN WITH(NOLOCK) ON CTN.CRM_TerritoryId = CTAN.CRM_TerritoryId 
		WHERE (CTN.REGIONID = @region OR @region = 0 OR ISNULL(ACTCUSDTL.CompanyZipCode,'') = '') 
		ORDER BY ACT.CRM_AccountId DESC
		OFFSET (@PageNo-1) * @PageSize ROWS
  		FETCH NEXT @PageSize ROWS ONLY
  	END  
  
END
