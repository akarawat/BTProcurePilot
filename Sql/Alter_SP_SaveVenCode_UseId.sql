-- Switches SP_SaveVenCode to match by the new Vendor.id primary key instead
-- of matching by VenCode. This also fixes a real bug in the original SP:
-- it had "SET VenCode = VenCode" (self-assignment) instead of
-- "VenCode = @VenCode", so editing the Vendor Code field in the UI never
-- actually persisted the new code.
-- Run this AFTER Sql/AddPK_Vendor.sql.
CREATE OR ALTER PROCEDURE [dbo].[SP_SaveVenCode]
	@id uniqueidentifier,
	@VenName nvarchar(255),
	@VenCode varchar(15),
	@Vencurrency varchar(15) = null,
	@flag int output
AS
BEGIN
	SET NOCOUNT ON;
	SET @flag = 0;
	UPDATE Vendor SET VenCode = @VenCode, VenName = @VenName,
		Vencurrency = @Vencurrency
	WHERE id = @id
	SET @flag = 1;
END
