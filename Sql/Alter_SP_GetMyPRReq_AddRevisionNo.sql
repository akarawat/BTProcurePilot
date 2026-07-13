-- Adds revision_no to the result set of SP_GetMyPRReq so
-- Controllers/DashboardsController.cs -> GetMyPRReq can read it.
-- Run this AFTER Sql/AddRevisionNo_PRHeader.sql has added the column to PRHeader.
-- =============================================
-- Author:  <Author,,Name>
-- Create date: <Create Date,,>
-- Description: <Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetMyPRReq]
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
 --CASE WHEN prstatus = 0 THEN 'Draft'
 -- WHEN prstatus = 1 THEN 'PR Submited'
 -- WHEN prstatus = 2 THEN 'On-Process'
 -- WHEN prstatus = 3 THEN 'PR Approved'
 -- WHEN prstatus = 5 THEN 'Procure reject'
 -- WHEN prstatus = 6 THEN 'Approval reject'
 -- WHEN prstatus = 7 THEN 'Owner cancel'
 -- WHEN prstatus = 9 THEN 'Approval reject'
 -- WHEN prstatus = 10 THEN 'Success PR. received'
 -- ELSE 'Nothing'
 --end
 --as prstatus_txt,

 [dbo].[FUNC_MSGPRStatusWithPO](codelog, prstatus, pr_recvpono) as prstatus_txt,

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
 ISNULL(revision_no, 0) as revision_no
 FROM [dbo].[PRHeader]
 WHERE empcode = @emp_code
 AND prstatus != 7
 order by reqDate desc;

END

--exec SP_GetMyPRReq 'S03434';
--SELECT codelog, pr_recvpono, * FROM [dbo].[PRHeader]
