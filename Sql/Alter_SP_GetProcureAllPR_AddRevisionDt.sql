-- Adds revision_dt_txt to the result set of SP_GetProcureAllPR so
-- Controllers/CTLAdminController.cs -> GetAllPRData can read it.
-- Run this AFTER Sql/AddRevisionDt_PRHeader.sql has added the column to PRHeader.
-- This SP already had revision_no added by Alter_SP_GetProcureAllPR_AddRevisionNo.sql.
-- =============================================
-- Author:  <Author,,Name>
-- Create date: <Create Date,,>
-- Description: <Description,,>
-- =============================================

ALTER PROCEDURE [dbo].[SP_GetProcureAllPR]

AS
BEGIN
 SET NOCOUNT ON;
 select codelog,
 prstatus,
 [dbo].[FUNC_MSGPRStatusWithPO](codelog, prstatus, pr_recvpono) as prstatus_txt,
 CASE WHEN (projectno = '' OR projectno IS NULL OR projectno = '-') THEN ''
 ELSE concat('[', projectno, '] ', [dbo].[FUNC_GetProjName](projectno))  END AS projectno,
 prno,
 ISNULL(format(approx_dt,'dd/MM/yyyy'), '') as approx_dt_txt,
 ISNULL(format(update_dt,'dd/MM/yyyy'), '') as update_dt_txt,
 [dbo].[FUNC_GetEmpFName](empcode) as empcode_txt,
 pr_reason,
 ISNULL(format(pr_recvdt,'dd/MM/yyyy'), '') as pr_recvdt_txt,
 pr_recvpono,
 reqDepCode,
 ISNULL(format(create_dt,'dd/MM/yyyy'), '') as create_dt_txt,
 procure_remark,
 purpose_type,
 create_dt
 ,
 CASE WHEN (LEN(appEmp) <> 0 AND appFlag = 0) THEN 'Pending'
  WHEN (LEN(appEmp) <> 0 AND appFlag = 1) THEN 'Approved'
 ELSE '' END as appEmp_txt,

 CASE WHEN (LEN(appEmp2) <> 0 AND appFlag2 = 0) THEN 'Pending'
  WHEN (LEN(appEmp2) <> 0 AND appFlag2 = 1) THEN 'Approved'
 ELSE '' END as appEmp2_txt,

 CASE WHEN (LEN(countEmp) <> 0 AND countFlag = 0) THEN 'Pending'
  WHEN (LEN(countEmp) <> 0 AND countFlag = 1) THEN 'Approved'
 ELSE '' END as countFlag_txt,

 CASE WHEN (LEN(authEmp) <> 0 AND authFlag = 0) THEN 'Pending'
  WHEN (LEN(authEmp) <> 0 AND authFlag = 1) THEN 'Approved'
 ELSE '' END as authEmp_txt
 , [dbo].[FUNC_SUMTotalDetail](id,1) as total_disc
 , [dbo].[FUNC_SUMTotalDetail](id,2) as total_exp
 , prcurrency
 , ISNULL(revision_no, 0) as revision_no
 , ISNULL(format(revision_dt, 'dd/MM/yyyy'), '') as revision_dt_txt
 from PRHeader
 where prstatus != 7
 order by update_dt desc;

END
