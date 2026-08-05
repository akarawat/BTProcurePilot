-- Switches SP_DeleteVenCode to match by the new Vendor.id primary key
-- instead of matching (and deleting) every row with a matching VenCode.
-- Run this AFTER Sql/AddPK_Vendor.sql.
CREATE OR ALTER PROCEDURE [dbo].[SP_DeleteVenCode]
	@id uniqueidentifier,
	@flag int output
AS

BEGIN
	SET NOCOUNT ON;
	SET @flag = 0;

	BEGIN
		DELETE Vendor
		WHERE id = @id
		SET @flag = 1;
	END

END
