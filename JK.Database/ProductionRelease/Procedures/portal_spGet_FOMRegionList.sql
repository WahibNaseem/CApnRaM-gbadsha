IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[portal_spGet_FOMRegionList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[portal_spGet_FOMRegionList]
GO

USE [jkbufdev]
GO
/****** Object:  StoredProcedure [dbo].[portal_spGet_FOMRegionList]    Script Date: 6/23/2018 12:27:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[portal_spGet_FOMRegionList]

as
	
begin
select R.RegionId, R.Name from   [dbo].[Region] R where Corporate = 1 and Status = 1
end