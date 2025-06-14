CREATE PROCEDURE AddPersonalComputer
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @OperationSystem VARCHAR(50)
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

        -- Insert into PersonalComputer table
        INSERT INTO PersonalComputer (OperationSystem, DeviceId)
        VALUES (@OperationSystem, @DeviceId);

        COMMIT TRANSACTION;
        SELECT RowVersion FROM @InsertedInfo;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
