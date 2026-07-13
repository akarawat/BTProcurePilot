-- Adds the "last revised" timestamp alongside revision_no.
-- Separate from update_dt because update_dt changes on every PRHeader
-- update, not only when a revision (resubmit-after-reject) happens.
-- Like revision_no, this stays NULL until the resubmit flow (not yet
-- built) starts setting it.
USE [BT_PROCURE]
GO

ALTER TABLE [dbo].[PRHeader] ADD [revision_dt] [datetime] NULL
GO
