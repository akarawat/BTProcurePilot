-- Allows adding multiple vendors with a blank VenCode. Previously the
-- uniqueness check (WHERE VenCode = @VenCode) matched every existing
-- blank-VenCode row too, so only the first vendor without a code could
-- ever be added — every one after that silently failed (@flag stayed 0).
-- Run this AFTER Sql/AddPK_Vendor.sql (id column must exist).
CREATE OR ALTER PROCEDURE [dbo].[SP_InsAddNewVendor]
	@VenName nvarchar(255),
	@VenCode varchar(15),
	@Vencurrency varchar(15) = null,
	@flag int output
AS
BEGIN
	SET NOCOUNT ON;
	SET @flag = 0;
	IF (@VenCode IS NULL OR @VenCode = '') OR NOT EXISTS (
        SELECT 1 FROM [dbo].[Vendor]
        WHERE VenCode = @VenCode
    )
	BEGIN
		INSERT INTO Vendor ([VenName], [VenCode], [Vencurrency])
		VALUES (@VenName, @VenCode, @Vencurrency);
		SET @flag = 1;
	END
END
