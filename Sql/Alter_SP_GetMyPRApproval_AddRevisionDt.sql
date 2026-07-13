-- Adds revision_dt_txt to the result set of SP_GetMyPRApproval so
-- Controllers/DashboardsController.cs -> GetMyPRApproval can read it.
-- Run this AFTER Sql/AddRevisionDt_PRHeader.sql has added the column to PRHeader.
-- This SP already had revision_no added by Alter_SP_GetMyPRApproval_AddRevisionNo.sql.
-- =============================================
-- Author:  <Author,,Name>
-- Create date: <Create Date,,>
-- Description: <Description,,>
/*
CASE WHEN prstatus = 0 THEN 'Draft'
WHEN prstatus = 1 THEN 'PR Submit', 'Waiting'
WHEN prstatus = 2 THEN 'On-Process'
WHEN prstatus = 3 THEN 'PR Approved'
WHEN prstatus = 5 THEN 'Procure reject'
WHEN prstatus = 6 THEN 'Approval reject'
WHEN prstatus = 7 THEN 'Owner cancel'
WHEN prstatus = 9 THEN 'Success PO.No. retreived'
*/
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetMyPRApproval]
 @emp_code VARCHAR(10)
AS
BEGIN
 SET NOCOUNT ON;
 declare @count_draft int;
 declare @count_ongoing int;
 declare @count_reject int;
 declare @count_approved int;
 SELECT @count_draft = count(1) FROM [dbo].[PRHeader] WHERE empcode = @emp_code and prstatus = 0;
 SELECT @count_ongoing = count(1) FROM [dbo].[PRHeader] WHERE empcode = @emp_code and prstatus in (1, 2, 3);
 SELECT @count_reject = count(1) FROM [dbo].[PRHeader] WHERE empcode = @emp_code and prstatus in (5, 6);
 SELECT @count_approved = count(1) FROM [dbo].[PRHeader] WHERE empcode = @emp_code and prstatus = 9;

 SELECT prno, reqDate, format(reqDate, 'dd/MM/yyyy') as reqDate_txt,
 prstatus,
 [dbo].[FUNC_MSGPRStatus](codelog, prstatus) as prstatus_txt,

 @count_draft as count_draft,
 @count_ongoing as count_ongoing,
 @count_reject as count_reject,
 @count_approved as count_approved,
 pr_recvpono,
 format(pr_recvdt, 'dd/MM/yyyy') as pr_recvdt_txt,
 procure_remark,
 create_dt,
 ISNULL(remarkEmp, '') as remarkEmp,
 ISNULL(remarkCount, '') as remarkCount,
 ISNULL(remarkAuth, '') as remarkAuth,
 purpose_type,
 projectno,
 appEmp, appEmp2, countEmp, authEmp,
 appFlag, appFlag2, countFlag, authFlag,
 ISNULL(revision_no, 0) as revision_no,
 ISNULL(format(revision_dt, 'dd/MM/yyyy'), '') as revision_dt_txt
 FROM [dbo].[PRHeader]
 WHERE
 ((appEmp = @emp_code OR appEmp2 = @emp_code) OR countEmp = @emp_code OR authEmp = @emp_code)
 AND prstatus != 7
 order by appFlag, appFlag2, countFlag, authFlag asc;

END

--exec SP_GetMyPRApproval 'S00365';
