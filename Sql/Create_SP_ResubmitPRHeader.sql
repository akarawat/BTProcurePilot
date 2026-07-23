-- New, standalone resubmit process — deliberately does NOT touch
-- SP_UpdatePRApprovalStatusV200 or SP_UpdateProcStatus (the existing
-- approve/reject procedures). This feature is scoped to exactly 2 roles:
-- Procurement (Reject, via the pre-existing procure_flag 7/9 action) and
-- Requester (Resend, via this SP). No Approver (Section Head/MGR/MD)
-- involvement — the client only shows the Resend button, and only calls
-- this SP, when procure_flag is 7 or 9.
--
-- Behavior:
--   - revision_no += 1, revision_dt = now
--   - procure_flag / flagm_proc reset to 0 so the PR re-enters the
--     Procurement queue directly (SP_GetProcureAllPR only filters on
--     prstatus <> 7, so this alone is enough to resurface it there)
--   - codelog = '102' so FUNC_MSGPRStatus renders "PR. Resend" as the status text
--   - appFlag / appFlag2 / countFlag / authFlag are never referenced —
--     this SP has no Approver-related logic at all
-- Run this AFTER Sql/AddRevisionNo_PRHeader.sql and Sql/AddRevisionDt_PRHeader.sql.
CREATE OR ALTER PROCEDURE [dbo].[SP_ResubmitPRHeader]
  @prno VARCHAR(50),
  @approx_type INT = NULL,
  @approx_dt DATETIME = NULL,
  @invcreditno VARCHAR(50) = NULL,
  @purpose_type INT = NULL,
  @ref_docs VARCHAR(50) = NULL,
  @pr_reason NVARCHAR(255) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @id UNIQUEIDENTIFIER;
  DECLARE @PRE_REVISION_NO INT;
  SELECT @id = id, @PRE_REVISION_NO = revision_no FROM PRHeader WHERE prno = @prno;

  UPDATE PRHeader SET
    approx_type = @approx_type,
    approx_dt = @approx_dt,
    invcreditno = @invcreditno,
    purpose_type = @purpose_type,
    ref_docs = @ref_docs,
    pr_reason = @pr_reason,
    procure_flag = 0,
    flagm_proc = 0,
    revision_no = ISNULL(@PRE_REVISION_NO, 0) + 1,
    revision_dt = GETDATE(),
    update_dt = GETDATE(),
    codelog = '102'
  WHERE id = @id;
END
