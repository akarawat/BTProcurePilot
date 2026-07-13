-- Adds revision_dt_txt to the result set of SP_GetMyPRReq so
-- Controllers/DashboardsController.cs -> GetMyPRReq can read it.
-- Run this AFTER Sql/AddRevisionDt_PRHeader.sql has added the column to PRHeader.
-- This SP already had revision_no added by Alter_SP_GetMyPRReq_AddRevisionNo.sql.
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
 ISNULL(revision_no, 0) as revision_no,
 ISNULL(format(revision_dt, 'dd/MM/yyyy'), '') as revision_dt_txt
 FROM [dbo].[PRHeader]
 WHERE empcode = @emp_code
 AND prstatus != 7
 order by reqDate desc;

END

--exec SP_GetMyPRReq 'S03434';
--SELECT codelog, pr_recvpono, * FROM [dbo].[PRHeader]
