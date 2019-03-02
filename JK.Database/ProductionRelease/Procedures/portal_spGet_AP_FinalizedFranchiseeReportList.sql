/*****************************************************************************************
[dbo].[portal_spGet_AP_FinalizedFranchiseeReportList] 

Purpose:  This Stored Procedure is used to return finalized franchisee report main info 
	for a given Region

Change History
========================
12/07/2017 TP - Add the stored procedure

*****************************************************************************************/

IF EXISTS (SELECT * FROM sysobjects WITH (NOLOCK) WHERE id = object_id(N'[dbo].[portal_spGet_AP_FinalizedFranchiseeReportList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
  DROP PROCEDURE [dbo].[portal_spGet_AP_FinalizedFranchiseeReportList]
GO

USE [jkBuf]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[portal_spGet_AP_FinalizedFranchiseeReportList]
	@RegionId int,
	@BillMonth int,
	@BillYear int
AS
BEGIN

SELECT FR.* FROM FranchiseeReport FR
LEFT JOIN APBill APB ON APB.FranchiseeReportId = FR.FranchiseeReportId
	WHERE FR.RegionId = IIF(@RegionId = 0, FR.RegionId, @RegionId) AND FR.BillMonth = ISNULL(@BillMonth, FR.BillMonth) AND FR.BillYear = ISNULL(@BillYear, FR.BillYear) 
	AND FR.IsFinalized = 1 AND APB.APBillId IS NULL

END
