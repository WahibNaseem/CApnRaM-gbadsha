IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[portal_spGet_CustomerDetail]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[portal_spGet_CustomerDetail]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[portal_spGet_CustomerDetail]  
   
 @ID int  =174075
  
AS  
BEGIN  
  
Declare @CustomerDetail table(CustomerId int,CustomerNo varchar(10),CustomerName varchar(150),AccountType varchar(100),MainAddress varchar(200),  
       Address2 varchar(100),Phone varchar(25),Ext varchar(10),Fax varchar(25),EmailAddress varchar(50),Cell varchar(20),  
       ContactName varchar(150),ContactTitle varchar(100),Amount varchar(20),RegionId int,
	   MaintenanceId int,MaintenanceTypeName varchar(200),StatusName varchar(200),StatusDate Datetime,ContractType varchar(200),ContractTypeListId int,ResumeDate Datetime)  
  
  
Insert into @CustomerDetail (CustomerId ,CustomerNo,CustomerName,RegionId,StatusName,StatusDate,ResumeDate)  
 Select C.CustomerId,C.CustomerNo,C.Name,C.RegionId,slType.Name As StatusName,st.StatusDate,st.ResumeDate  from  Customer C
 INNER JOIN [dbo].[StatusList] slType on slType.StatusListId = C.StatusListId
 INNER JOIN [dbo].[Status] st on st.StatusId = c.StatusId and st.IsActive = 1   
 Where CustomerID = @ID  
  
  
  
--Update b set MainAddress = a.Address1+' '+ isnull(a.Address2,'')+',', Address2 = City+', '+ case Lower(s.Name) when 'new york' then 'NY' else s.Name end  +' '+PostalCode  
Update b set MainAddress = a.Address1 +',', Address2 = City+', '+ case Lower(s.Name) when 'new york' then 'NY' else s.Name end  +' '+PostalCode  
from StateList as s   
  inner join [Address] as a on s.StateListId = a.StateListId  
  inner join @CustomerDetail b on a.Classid=b.CustomerID where a.isActive = 1  
    
  
Update b set   
Phone =   
case   
When  len(a.Phone)=10 and isnull(a.Phone,'')<>'' then  
 '('+ SUBSTRING ( a.Phone ,1 , 3 ) +') ' +  SUBSTRING ( a.Phone ,4 , 3 ) +'-'+ SUBSTRING ( a.Phone ,7,len(a.Phone) )  
  else a.Phone  
end,  
Ext=isNUll(PhoneExt,''),  
Fax=case   
When  len(a.Fax)=10 and isnull(a.Fax,'')<>''  then  
 '('+ SUBSTRING ( a.Fax ,1 , 3 ) +') ' +  SUBSTRING ( a.Fax ,4 , 3 ) +'-'+ SUBSTRING ( a.Fax ,7 , len(a.Fax))  
  else a.Fax  
end,  
b.Cell =  
isnull(case   
When  len(a.Cell)=10 and isnull(a.Cell,'')<>''  then  
 '('+ SUBSTRING ( a.Cell ,1 , 3 ) +') ' +  SUBSTRING ( a.Cell ,4 , 3 ) +'-'+ SUBSTRING ( a.Cell ,7 , len(a.Cell)) else a.Cell  
end,'')  
from Phone as a   
  inner join @CustomerDetail b on a.Classid=b.CustomerID  where a.TypeListId = 1 and a.ContactTypeListId = 1  and a.isActive = 1
  
  
Update b set EmailAddress = a.EmailAddress  
from  [Email] as a  
  inner join @CustomerDetail b on a.Classid=b.CustomerID   where a.TypeListId = 1 and a.ContactTypeListId = 6 and a.isActive = 1
  
Update b set ContactName =a.Name,ContactTitle = a.Title 
from  [Contact] as a  
  inner join @CustomerDetail b on a.Classid=b.CustomerID where a.TypeListId = 1 and a.ContactTypeListId = 6 

Update b set AccountType = t.Name  
from    
  AcountTypeList as t   
  inner join Contact as a on t.AcountTypeListId =  a.TypeListId  
  inner join @CustomerDetail b on a.Classid=b.CustomerID  
    
Update b set AccountType = t.Name  
from    
  AcountTypeList as t   
  inner join Contract as a on t.AcountTypeListId =  a.AccountTypeListId  
  inner join @CustomerDetail b on a.CustomerID=b.CustomerID
  
Update b set ContractType = CT.Name  , ContractTypeListId = CT.ContractTypeListId
from    
  ContractTypeList as CT   
  inner join Contract as a on CT.ContractTypeListId =  a.ContractTypeListId  
  inner join @CustomerDetail b on a.CustomerID=b.CustomerID   
  
Update b set Amount= a.Amount  
from    
  Contract as a   
  inner join @CustomerDetail b on a.CustomerID = b. CustomerID  and a.IsActive = 1 


  Update b set MaintenanceId= MT.MaintenanceTempId, MaintenanceTypeName = MTL.[Name]
  from    
  MaintenanceTemp as MT   
  inner join @CustomerDetail b on MT.ClassId=b. CustomerID and MT.TypeListId = 1 and MT.IsActive = 1 
  inner join MaintenanceTypeList MTL ON MTL.MaintenanceTypeListId=MT.MaintenanceTypeListId 
  
  
Select * from @CustomerDetail  
  
  
End  