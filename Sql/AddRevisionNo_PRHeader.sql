-- Adds revision tracking to PRHeader.
-- revision_no starts at 0 and is intended to be incremented by whichever
-- stored procedure eventually handles "resubmit after reject" (not yet built).
USE [BT_PROCURE]
GO

ALTER TABLE [dbo].[PRHeader] ADD [revision_no] [int] NOT NULL
    CONSTRAINT [DF_PRHeader_revision_no] DEFAULT ((0))
GO

-- Run Sql/Alter_SP_GetPRDocno_AddRevisionNo.sql next to expose the new
-- column through SP_GetPRDocno (used by GetPRByDocNo). Until that SP is
-- updated, GetPRByDocNo falls back to 0 safely.
