-- Maps a PRHeader employee code (appEmp/countEmp/authEmp/empcode, e.g. 'S01544')
-- to the Windows SAM account name (e.g. 'martin.r') that the DigitalSign
-- API's /api/signature-registry/image/{samAccount} endpoint expects.
-- Confirmed source: [BT_HR].[dbo].[onl_TBADUsers].SAMACC
USE [BT_PROCURE]
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_GetEmpSamAccount]
	@emp_code VARCHAR(25)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT SAMACC, DISPNAME, DISPNAME_TH
	FROM [BT_HR].[dbo].[onl_TBADUsers]
	WHERE emp_code = @emp_code;
END
GO
