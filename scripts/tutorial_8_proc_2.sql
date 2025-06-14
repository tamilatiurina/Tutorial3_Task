CREATE PROCEDURE AddSmartwatch
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @BatteryPercentage INT
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

        -- Insert into Smartwatch table
        INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
        VALUES (@BatteryPercentage, @DeviceId);

        COMMIT TRANSACTION;
        SELECT RowVersion FROM @InsertedInfo;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
