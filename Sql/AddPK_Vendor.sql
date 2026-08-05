-- The Vendor table has no primary key at all, so every UPDATE/DELETE in
-- SP_SaveVenCode / SP_DeleteVenCode matches rows by VenCode (a plain
-- nvarchar column with no uniqueness enforced) — if two rows ever share a
-- VenCode, editing/deleting one silently edits/deletes both.
-- Adds a real GUID primary key, matching every other table in this app
-- (PRHeader.id, PRItemDetail.id, PRSuggVendor.id, ...).
-- NEWID() as the default is evaluated per-row for existing data, so each
-- existing vendor row gets its own unique id — safe to run as-is.
USE [BT_PROCURE]
GO

ALTER TABLE [dbo].[Vendor] ADD [id] [uniqueidentifier] NOT NULL
    CONSTRAINT [DF_Vendor_id] DEFAULT (NEWID())
GO

ALTER TABLE [dbo].[Vendor] ADD CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED ([id] ASC)
GO
