USE [BT_PROCURE]
GO
/****** Object:  UserDefinedFunction [dbo].[FUNC_MSGPRStatus]    Script Date: 15-07-2026 4:29:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[FUNC_MSGPRStatus] 
(
	@codelog varchar(15), 
	@prstatus int 
)
RETURNS varchar(255) 
AS
BEGIN
	
	DECLARE @rMsg varchar(255);
	
	--IF @codelog = '101' BEGIN SET @rMsg = 'PR. Submitted' END 
	--ELSE IF @codelog = '102' BEGIN SET @rMsg = 'PR. Resend' END 
	--ELSE IF @codelog = '20411' BEGIN SET @rMsg = 'Approval approved' END 
	--ELSE IF @codelog = '20729' BEGIN SET @rMsg = 'Manager Reject' END 
	--ELSE IF @codelog = '20719' BEGIN SET @rMsg = 'Approval Reject' END 
	--ELSE IF @codelog = '20521' BEGIN SET @rMsg = 'Manager Approved' END 
	--ELSE IF @codelog = '20632' BEGIN SET @rMsg = 'Manager PR Approved' END 
	--ELSE IF @codelog = '20739' BEGIN SET @rMsg = 'MD. Rejected' END 
	--ELSE IF @codelog = '200' BEGIN SET @rMsg = 'Send mail to procure' END 
	--ELSE IF @codelog = '201' BEGIN SET @rMsg = 'Send mail to procure' END 
	--ELSE IF @codelog = '202' BEGIN SET @rMsg = 'Procure PR Received' END 
	--ELSE IF @codelog = '203' BEGIN SET @rMsg = 'Procure PR Reject' END 
	--ELSE IF @codelog = '100' AND @prstatus = 0 BEGIN SET @rMsg = 'Draft' END 
	--ELSE IF @codelog = '100' AND @prstatus = 1 BEGIN SET @rMsg = 'Submited' END 
	--ELSE IF @codelog = '100' AND @prstatus = 7 BEGIN SET @rMsg = 'Owner Cancel' END 
	--ELSE IF @codelog = '700' BEGIN SET @rMsg = 'Owner Cancel' END 
	--ELSE IF @codelog IS NULL BEGIN SET @rMsg = '-' END 

	IF @codelog = '101' BEGIN SET @rMsg = 'PR. Submitted' END 
	ELSE IF @codelog = '102' BEGIN SET @rMsg = 'PR. Resend' END 
	ELSE IF @codelog = '20411' AND @prstatus <> 2 BEGIN SET @rMsg = 'Pending' END
	ELSE IF @codelog = '20411' AND @prstatus = 2  BEGIN SET @rMsg = 'Send mail to procure' END --'Approval approved' END 
	ELSE IF @codelog = '20729' BEGIN SET @rMsg = 'Manager Reject' END --'Manager Reject' END 
	ELSE IF @codelog = '20719' BEGIN SET @rMsg = 'Approval Reject' END 
	ELSE IF @codelog = '20521' BEGIN SET @rMsg = 'Pending' END --'Manager Approved' END 
	ELSE IF @codelog = '20632' BEGIN SET @rMsg = 'Pending' END  --'Manager PR Approved' END 
	ELSE IF @codelog = '20739' BEGIN SET @rMsg = 'MD. Rejected' END 
	ELSE IF @codelog = '200' BEGIN SET @rMsg = 'Send mail to procure' END 
	ELSE IF @codelog = '201' BEGIN SET @rMsg = 'Send mail to procure' END 
	ELSE IF @codelog = '202' BEGIN SET @rMsg = 'Procure PR Received' END 
	ELSE IF @codelog = '203' BEGIN SET @rMsg = 'Procure PR Reject' END 
	ELSE IF @codelog = '100' AND @prstatus = 0 BEGIN SET @rMsg = 'Draft' END 
	ELSE IF @codelog = '100' AND @prstatus = 1 BEGIN SET @rMsg = 'Submited' END 
	ELSE IF @codelog = '100' AND @prstatus = 7 BEGIN SET @rMsg = 'Owner Cancel' END 
	ELSE IF @codelog = '700' BEGIN SET @rMsg = 'Owner Cancel' END 
	--ELSE IF @codelog IS NULL BEGIN SET @rMsg = '-' END 
	ELSE IF @codelog IS NULL BEGIN SET @rMsg = 'Pending for approval' END 
	ELSE BEGIN SET @rMsg = '-' END 
    
	--101 = PR. Submitted
--20411 = Approval approved
--20729 = Manager Reject
--20719 = Approval Reject
--20521 = Manager Approved
--20739 = MD. Rejected
--200 = MD. Approved / Send mail to procure
	
	RETURN ISNULL(@rMsg, 'Draft');

END

--PRINT [dbo].[FUNC_MSGPRStatus]('200');

SELECT dbo.FUNC_MSGPRStatusWithPO('102', 1, '')
