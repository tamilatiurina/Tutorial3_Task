CREATE PROCEDURE AddEmbedded
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @IpAddress VARCHAR(50),
    @NetworkName VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert into Device table
        DECLARE @InsertedInfo TABLE (RowVersion ROWVERSION);

        -- Insert into Device and capture RowVersion
        INSERT INTO Device (Id, Name, Enabled)
        OUTPUT INSERTED.RowVersion INTO @InsertedInfo
        VALUES (@DeviceId, @Name, @IsEnabled);

        -- Insert into Embedded table
        INSERT INTO Embedded (IpAddress, NetworkName, DeviceId)
        VALUES (@IpAddress, @NetworkName, @DeviceId);

        COMMIT TRANSACTION;
        SELECT RowVersion FROM @InsertedInfo;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

    DROP PROCEDURE AddEmbedded;
