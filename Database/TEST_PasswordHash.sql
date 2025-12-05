-- Test para verificar el hash de contraseña
USE ControlGastosDB;
GO

DECLARE @Password NVARCHAR(100) = 'AdminJosbel2024!';

-- Hash en hexadecimal
DECLARE @HashHex NVARCHAR(500);
SET @HashHex = CONVERT(NVARCHAR(500), HASHBYTES('SHA2_256', @Password), 2);

-- Hash en Base64
DECLARE @PasswordHashBytes VARBINARY(32);
SET @PasswordHashBytes = HASHBYTES('SHA2_256', @Password);
DECLARE @HashBase64 NVARCHAR(500);
SET @HashBase64 = CAST('' AS XML).value('xs:base64Binary(sql:variable("@PasswordHashBytes"))', 'NVARCHAR(500)');

PRINT 'Password: ' + @Password;
PRINT 'Hash (Hex): ' + @HashHex;
PRINT 'Hash (Base64): ' + @HashBase64;
PRINT '';

-- Ver qué hay en la BD
SELECT
    Email,
    PasswordHash,
    LEN(PasswordHash) AS HashLength
FROM Usuario
WHERE Email = 'pdmas287@gmail.com';

GO
