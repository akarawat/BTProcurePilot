-- Adds revision_no to the result set of SP_GetPRDocno so
-- Controllers/CTLAdminController.cs -> GetPRByDocNo can read it.
-- Run this AFTER Sql/AddRevisionNo_PRHeader.sql has added the column to PRHeader.
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- V106
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetPRDocno]
	@docno VARCHAR(25)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
	id,
	prno,
	projectno,
	CASE WHEN (projectno = '' OR projectno IS NULL OR projectno = '-') THEN ''
	ELSE concat('[', projectno, '] ', [dbo].[FUNC_GetProjName](projectno))  END AS projectname,
	empcode,
	[dbo].[FUNC_GetEmpFName](empcode) as empcode_txt,
	approx_type,
	approx_dt,
	format(approx_dt, 'dd/MM/yyyy') as  approx_dt_txt,
	invcreditno,
	purpose_type,
	ref_docs,
	pr_reason,
	pr_recvdt,
	format(pr_recvdt, 'dd/MM/yyyy') as pr_recvdt_txt,
	pr_recvpono,
	attach_flag,
	reqDepCode,
	reqDate,
	reqFlag,
	appEmp,
	appDate,
	[dbo].[FUNC_GetEmpCodeFName](appEmp) as appEmp_txt,
	format(appDate, 'dd/MM/yyyy HH:mm') as appDate_txt,
	appFlag,

	appEmp2,
	appDate2,
	[dbo].[FUNC_GetEmpCodeFName](appEmp2) as appEmp2_txt,
	CASE WHEN appDate2 IS NOT NULL THEN format(appDate2, 'dd/MM/yyyy HH:mm')
		ELSE '' END
	AS appDate2_txt,
	appFlag2,

	countEmp,
	countDate,
	[dbo].[FUNC_GetEmpCodeFName](countEmp) as countEmp_txt,
	format(countDate, 'dd/MM/yyyy HH:mm') as countDate_txt,
	countFlag,
	authEmp,
	authDate,
	[dbo].[FUNC_GetEmpCodeFName](authEmp) as authEmp_txt,
	format(authDate, 'dd/MM/yyyy HH:mm') as authDate_txt,
	authFlag,
	prstatus,
	create_dt,
	update_dt,
	format(update_dt, 'dd/MM/yyyy HH:mm') as update_dt_txt,
	format(reqDate, 'dd/MM/yyyy') as reqDate_txt,

	--CASE WHEN prstatus = 0 THEN 'Draft'
	--	WHEN prstatus = 1 THEN 'PR Submited'
	--	WHEN prstatus = 2 THEN 'On-Process'
	--	WHEN prstatus = 3 THEN 'PR Approved'
	--	WHEN prstatus = 5 THEN 'Procure reject'
	--	WHEN prstatus = 6 THEN 'Approval reject'
	--	WHEN prstatus = 7 THEN 'Owner cancel'
	--	WHEN prstatus = 9 THEN 'Approval reject'
	--	WHEN prstatus = 10 THEN 'Success PR. received'
	--	ELSE 'Nothing'
	--end
	--as prstatus_txt,

	[dbo].[FUNC_MSGPRStatusWithPO](codelog, prstatus, pr_recvpono) as prstatus_txt,

	pub_remark,
	[dbo].[FUNC_GetPRCurrency] (prno) as prcurrency,

	CASE WHEN empcode IS NOT NULL THEN [dbo].[FUNC_GetEmailEmpCode](empcode) ELSE '' END as reqEmail,
	CASE WHEN appEmp IS NOT NULL THEN [dbo].[FUNC_GetEmailEmpCode](appEmp) ELSE '' END as appEmail,
	CASE WHEN appEmp2 IS NOT NULL THEN [dbo].[FUNC_GetEmailEmpCode](appEmp2) ELSE '' END as appEmail2,
	CASE WHEN countEmp IS NOT NULL THEN [dbo].[FUNC_GetEmailEmpCode](countEmp) ELSE '' END as CountEmail,
	CASE WHEN authEmp IS NOT NULL THEN [dbo].[FUNC_GetEmailEmpCode](authEmp) ELSE '' END as authEmail,

	CASE
		WHEN LEN(authEmp) > 0 AND authFlag = 1 AND LEN(countEmp) > 0 AND countFlag = 1 AND LEN(appEmp) > 0 AND appFlag = 1 THEN 35
		WHEN LEN(authEmp) > 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag = 1 AND LEN(appEmp) > 0 AND appFlag = 1 THEN 33
		WHEN LEN(authEmp) > 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag = 0 AND LEN(appEmp) > 0 AND appFlag = 1 THEN 32
		WHEN LEN(authEmp) > 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag IN (0,9) AND LEN(appEmp) > 0 AND appFlag IN (0,9) THEN 31

		WHEN LEN(authEmp) = 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag = 1 AND LEN(appEmp) > 0 AND appFlag = 1 THEN 25
		WHEN LEN(authEmp) = 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag = 0 AND LEN(appEmp) > 0 AND appFlag = 1 THEN 22
		WHEN LEN(authEmp) = 0 AND authFlag = 0 AND LEN(countEmp) > 0 AND countFlag IN (0,9) AND LEN(appEmp) > 0 AND appFlag IN (0,9) THEN 21

		WHEN LEN(authEmp) = 0 AND authFlag = 0 AND LEN(countEmp) = 0 AND countFlag = 0 AND LEN(appEmp) > 1 AND appFlag = 1 THEN 15
		WHEN (LEN(authEmp) = 0 AND authFlag = 0 AND LEN(countEmp) = 0 AND countFlag = 0 AND LEN(appEmp) >= 0 AND appFlag IN (0,9)) OR LEN(appEmp) >= 0 AND appFlag IS NULL THEN 11

		ELSE 0
	END AS approve_step,

	ISNULL(flagm_proc, 0) as flagm_proc,
	ISNULL(procure_flag, 0) as procure_flag,
	procure_remark,
	ISNULL(codelog, '') as codelog,
	ISNULL(remarkEmp, '') as remarkEmp,
	ISNULL(remarkCount, '') as remarkCount,
	ISNULL(remarkAuth, '') as remarkAuth,
	total_disc,
	quo_return,

	ISNULL(remarkEmp2, '') as remarkEmp2,
	ISNULL(revision_no, 0) as revision_no

	FROM [dbo].[PRHeader]
	WHERE prno = @docno;

END

--SELECT * FROM [dbo].[PRHeader] where prno = '420-250819006';
--exec SP_GetPRDocno '420-250807002';
--exec SP_GetPRDocno '450-251128001';

--SELECT LEN(authEmp), LEN(countEmp), LEN(appEmp) FROM [dbo].[PRHeader];
--SELECT authEmp, authFlag, countEmp, countFlag, appEmp, appFlag FROM [dbo].[PRHeader] where prno = '420-250825001';
