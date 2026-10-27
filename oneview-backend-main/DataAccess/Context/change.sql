#1
-- Issue: 39
DROP TABLE IF EXISTS Versions;
CREATE TABLE Versions (
Id int PRIMARY KEY IDENTITY(1,1),
VersionNo int UNIQUE NOT NULL,
IssueID nvarchar(50) NOT NULL,
ReleaseDate datetime2 NOT NULL
);


#2
-- Issue: 39
CREATE TABLE Customers(
    Id int PRIMARY KEY IDENTITY(1,1),
    CustomerID VARCHAR(50) UNIQUE NOT NULL,
    [Name] VARCHAR(50) not null,
    [Address] VARCHAR(50) not null,
    PhoneNumber VARCHAR(50),
    Email VARCHAR(50),
    VATNumber VARCHAR(50),
    DeactivationDate DATETIME
);

#3
-- Issue: 71
INSERT INTO Customers (CustomerID, [Name], [Address], PhoneNumber, Email ,VATNumber, DeactivationDate)
VALUES
('ABENA', 'ABENA', 'Egelund 35 6200 Aabenraa', '74311818', 'info@abena.dk ', '25682742', NULL),
('OUH', 'OUH', 'J. B. Winsløws Vej 4, 5000 Odense', '66113333', 'info@ouh.dk', '29190909', NULL),
('OK', 'OK', 'Åhave Parkvej 11, 8260 Viby J', '70102033', 'info@ok.dk', '39170418', NULL );


#4
-- Issue: 93
BEGIN TRAN
DECLARE @UqKey SYSNAME;
DECLARE @Sql NVARCHAR(500);

-- Find UNIQUE constraint
SELECT @UqKey = name
FROM sys.key_constraints
WHERE parent_object_id = OBJECT_ID('dbo.Customers')
  AND type = 'UQ';

-- If constraint if found then do changes
IF @UqKey IS NOT NULL
    BEGIN
        SET @Sql = N'ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [' + @UqKey + N'];';
        EXEC(@Sql);
        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [CustomerID] NVARCHAR(255) NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [Name] NVARCHAR(255) NOT NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [Address] NVARCHAR(255) NOT NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [PhoneNumber] NVARCHAR(255) NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [Email] NVARCHAR(255) NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [VATNumber] NVARCHAR(255) NULL;

        ALTER TABLE [dbo].[Customers]
        ALTER COLUMN [DeactivationDate] DATETIME NULL;

        ALTER TABLE [dbo].[Customers]
        ADD CONSTRAINT [UQ_Customers_CustomerID] UNIQUE NONCLUSTERED ([CustomerID]);
        PRINT 'Changes has been done'
    END
ELSE
    BEGIN
        PRINT 'Changes could not be done — UNIQUE constraint not found.';
        ROLLBACK TRAN;
        RAISERROR('UNIQUE constraint not found — migration aborted.', 16, 1);
        RETURN;
    END
COMMIT TRAN;

#5
-- Issue: 109
begin tran
create table [dbo].[Contact] (

Id INT IDENTITY(1,1) PRIMARY KEY,
fk_CustomerId nvarchar(255) not null,
[Name] nvarchar(255) not null,
[Title] nvarchar(255),
[PhoneNumber] nvarchar(255),
[Email] nvarchar(255),

CONSTRAINT fk_CustomerId_Customers
FOREIGN KEY ([fk_CustomerId]) REFERENCES [dbo].[Customers](CustomerId)
)
commit tran;

#6
-- Issue: 93
BEGIN TRAN
    CREATE TABLE [dbo].[ContextType] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TypeName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(255) NULL
    );
COMMIT TRAN;

#7
-- Issue: 93
BEGIN TRAN
    CREATE TABLE [dbo].[Context] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [fk_CustomerId] INT NULL,
    [fk_ContextTypeId] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [ReferenceLink] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_Context_CreatedAt DEFAULT(GETDATE()),
    [LastUpdated] DATETIME2 NOT NULL CONSTRAINT DF_Context_LastUpdated DEFAULT(GETDATE()),

    CONSTRAINT FK_Context_Customer
        FOREIGN KEY ([fk_CustomerId]) REFERENCES [dbo].[Customers]([Id]),

    CONSTRAINT FK_Context_ContextType
        FOREIGN KEY ([fk_ContextTypeId]) REFERENCES [dbo].[ContextType]([Id])
    );
COMMIT TRAN;

#8
-- Issue: 93
BEGIN TRAN
    CREATE TABLE [dbo].[ContextDataType] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(255) NULL
    );

    CREATE TABLE [dbo].[ContextData] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [fk_ContextId] INT NOT NULL,
    [fk_ContextDataTypeId] INT NOT NULL,
    [KeyName] NVARCHAR(255) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,

    CONSTRAINT FK_ContextData_Context
        FOREIGN KEY ([fk_ContextId]) REFERENCES [dbo].[Context]([Id]),

    CONSTRAINT FK_ContextData_Type
        FOREIGN KEY ([fk_ContextDataTypeId]) REFERENCES [dbo].[ContextDataType]([Id])
    );
COMMIT TRAN;

#9
-- Issue: 93
BEGIN TRAN
ALTER TABLE [dbo].[ContextType]
ADD CONSTRAINT UQ_ContextType_TypeName UNIQUE (TypeName);
COMMIT TRAN;

BEGIN TRAN
ALTER TABLE [dbo].[ContextDataType]
ADD CONSTRAINT UQ_ContextDataType_Name UNIQUE (Name);
COMMIT TRAN;

INSERT INTO ContextType (TypeName, Description) VALUES
('File', 'Represents a file reference and its metadata'),
('FreshdeskTicket', 'Represents a Freshdesk ticket reference or ticket metadata'),
('KeeperCredential', 'Represents a reference to a Keeper credential or stored secret');

INSERT INTO ContextDataType (Name, Description) VALUES
('FileSize', 'Size of the file in bytes'),
('LastModified', 'Timestamp of last modification'),
('FileExtension', 'The file extension, e.g., pdf, xml, txt, png, etc.');

#10
-- Issue: 112
BEGIN TRAN
    CREATE TABLE Labels (
        Id INT PRIMARY KEY IDENTITY(1,1),
        fk_CustomerId INT NOT NULL,
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(255),

        CONSTRAINT FK_Labels_Customer
            FOREIGN KEY (fk_CustomerId)
            REFERENCES Customers(Id)
    );
COMMIT TRAN

#11
-- Issue: 112
BEGIN TRAN
    CREATE TABLE LabelData (
        Id INT PRIMARY KEY IDENTITY(1,1),
        fk_LabelId INT NOT NULL,
        fk_ContextId INT NOT NULL,
        Value NVARCHAR(255),

        CONSTRAINT FK_LabelData_Label
            FOREIGN KEY (fk_LabelId)
            REFERENCES Labels(Id),

        CONSTRAINT FK_LabelData_Context
            FOREIGN KEY (fk_ContextId)
            REFERENCES Context(Id)
    );
COMMIT TRAN

#12
-- Issue: 112

CREATE VIEW vw_Label
AS

SELECT
    l.Id,
    cu.CustomerID       AS 'CustomerId',
    ct.Id               AS 'ContextId',
    l.Name,
    l.[Description],
    ld.[Value]
FROM Labels l
LEFT JOIN Customers cu on cu.Id = l.fk_CustomerId
LEFT JOIN LabelData ld on l.Id = ld.fk_LabelId
LEFT JOIN Context ct on ct.Id = ld.fk_ContextId;


#13
-- Issue: 112

CREATE TRIGGER trg_vw_Label_Insert
ON vw_Label
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------
    -- Only one row in inserted
    -----------------------------------------

    IF (SELECT COUNT(*) FROM inserted) > 1
        BEGIN
            RAISERROR('ERROR: Only single row insert is allowed', 16,1);
            RETURN;
        END

    DECLARE 
        @LabelId INT,
        @CustomerId INT,
        @ContextId INT,
        @Name NVARCHAR(255),
        @Description NVARCHAR(255),
        @Value NVARCHAR(255);


    -----------------------------------------
    -- Read data from INSERTED
    -----------------------------------------
    SELECT 
        @Name = i.Name,
        @Description = Description,
        @Value = Value,
        @ContextId = ContextID,
        @CustomerId = cu.Id
    FROM inserted i
    JOIN Customers cu ON cu.CustomerId = i.CustomerId;


    -----------------------------------------
    -- Validate if label already exists
    -----------------------------------------
    SELECT @LabelId = Id
    FROM Labels
    WHERE fk_CustomerId = @CustomerId
        AND Name = @Name;

    IF @LabelId IS NOT NULL
    BEGIN
        RAISERROR (
            'Label "%s" already exists for CustomerID %d. Insert rejected.',
            16, 1, @Name, @CustomerId
        );
        RETURN;
    END


    -----------------------------------------
    -- Validate relation between customer and context
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM Context
            WHERE Id = @ContextId
                AND fk_CustomerId = @CustomerId
        )
        BEGIN
            RAISERROR (
                'ContextID %d does not belong to CustomerID %d. Insert rejected.',
                16, 1, @ContextId, @CustomerId
            );
            RETURN;
        END
    END


    -----------------------------------------
    -- Insert new unique label
    -----------------------------------------
    INSERT INTO Labels (fk_CustomerId, Name, Description)
    VALUES (@CustomerId, @Name, @Description);

    SET @LabelId = SCOPE_IDENTITY();


    -----------------------------------------
    -- Insert date with valid relation
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        INSERT INTO LabelData (fk_LabelId, fk_ContextId, Value)
        VALUES (@LabelId, @ContextId, @Value);
    END

END;

#14
-- Issue: 112

CREATE TRIGGER trg_vw_Label_Update
ON vw_Label
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------
    -- Only one row in update
    -----------------------------------------
    IF (SELECT COUNT(*) FROM inserted) > 1
        BEGIN
            RAISERROR('ERROR: Only single row update is allowed', 16,1);
            RETURN;
        END


    DECLARE
        @LabelId INT,
        @CustomerId INT,
        @ContextId INT,
        @Name NVARCHAR(255),
        @Description NVARCHAR(255),
        @Value NVARCHAR(255);

    -----------------------------------------
    -- Read data from Inserted
    -----------------------------------------
    SELECT
        @LabelId = i.Id,
        @Name = i.Name,
        @Description = i.Description,
        @Value = i.Value,
        @ContextId = i.ContextID,
        @CustomerId = cu.Id
    FROM inserted i
    JOIN Customers cu ON cu.CustomerID = i.CustomerID;


    -----------------------------------------
    -- Validate that update doesn't break unique rule
    -----------------------------------------
    IF EXISTS (
        SELECT 1
        FROM Labels
        WHERE fk_CustomerId = @CustomerId
            AND Name = @Name
            AND Id <> @LabelId     
    )
    BEGIN
        RAISERROR(
            'Label "%s" already exists for CustomerID %d. Update rejected.',
            16, 1, @Name, @CustomerId
        );
        RETURN;
    END


    -----------------------------------------
    -- Validate relation between customerID and ContextID
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM Context
            WHERE Id = @ContextId
                AND fk_CustomerId = @CustomerId
        )
        BEGIN
            RAISERROR(
                'ContextID %d does not belong to CustomerID %d. Update rejected.',
                16, 1, @ContextId, @CustomerId
            );
            RETURN;
        END
    END


    -----------------------------------------
    -- Update Labels
    -----------------------------------------
    UPDATE Labels
    SET
        Name = @Name,
        Description = @Description,
        fk_CustomerId = @CustomerId
    WHERE Id = @LabelId;


    -----------------------------------------
    -- 4. Update or create
    -----------------------------------------

    -- If exists do update
    UPDATE LabelData
    SET
        fk_ContextId = @ContextId,
        Value = @Value
    WHERE fk_LabelId = @LabelId;

    -- If not exists insert new
    IF NOT EXISTS (
        SELECT 1 FROM LabelData WHERE fk_LabelId = @LabelId
    )
    AND @ContextId IS NOT NULL
    BEGIN
        INSERT INTO LabelData (fk_LabelId, fk_ContextId, Value)
        VALUES (@LabelId, @ContextId, @Value);
    END

END;

#15
-- Issue: 112

CREATE TRIGGER trg_vw_Label_Delete
ON vw_Label
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------
    -- Delete label data first
    -----------------------------------------
    DELETE FROM LabelData
    WHERE fk_LabelId IN (SELECT Id FROM deleted);

    -----------------------------------------
    -- Delete label
    -----------------------------------------
    DELETE FROM Labels
    WHERE Id IN (SELECT Id FROM deleted);
END;

#16
-- Issue: 112
BEGIN TRAN
    INSERT INTO vw_Label (CustomerId, [Name], [Description], [Value])
    VALUES
    ('ABENA', 'Label 1', 'Label 1 description', 'Label value');

    -- Trigger does only support single row insert
    INSERT INTO vw_Label (CustomerId,Name)
      VALUES
      ('ABENA', 'Label 2');

    INSERT INTO vw_Label (CustomerId,Name)
      VALUES
      ('ABENA', 'Label 3');

    INSERT INTO vw_Label (CustomerId,Name)
      VALUES
      ('ABENA', 'Label 4');
COMMIT TRAN

#17
-- Issue: 112
BEGIN TRAN
UPDATE vw_Label 
  SET   [Description] = 'Label 01 Description',
        [value] = 'Lavel Value 01'
    WHERE customerID = 'ABENA' AND [name] = 'Label 1'
COMMIT TRAN

#18
-- Issue: 50
CREATE TABLE Users(
    Id int PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(255) UNIQUE NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    [Role] NVARCHAR(255) NOT NULL
);

#19
-- Issue: 50
INSERT INTO Users(Username, [Password], [Role])
VALUES
('user','pass','USER');

#20
-- Issue: 120
ALTER TRIGGER trg_vw_Label_Insert
ON vw_Label
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------
    -- Only one row in inserted
    -----------------------------------------

    IF (SELECT COUNT(*) FROM inserted) > 1
        BEGIN
            RAISERROR('ERROR: Only single row insert is allowed', 16,1);
            RETURN;
        END

    DECLARE 
        @LabelId INT,
        @CustomerId INT,
        @ContextId INT,
        @Name NVARCHAR(255),
        @Description NVARCHAR(255),
        @Value NVARCHAR(255);


    -----------------------------------------
    -- Read data from INSERTED
    -----------------------------------------
    SELECT 
        @Name = i.Name,
        @Description = Description,
        @Value = Value,
        @ContextId = ContextID,
        @CustomerId = cu.Id
    FROM inserted i
    JOIN Customers cu ON cu.CustomerId = i.CustomerId;


    -----------------------------------------
    -- Validate if label already exists
    -----------------------------------------
    SELECT @LabelId = Id
    FROM Labels
    WHERE fk_CustomerId = @CustomerId
        AND Name = @Name;

    IF @LabelId IS NOT NULL
    BEGIN
        RAISERROR (
            'Label "%s" already exists for CustomerID %d. Insert rejected.',
            16, 1, @Name, @CustomerId
        );
        RETURN;
    END


    -----------------------------------------
    -- Validate relation between customer and context
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM Context
            WHERE Id = @ContextId
                AND fk_CustomerId = @CustomerId
        )
        BEGIN
            RAISERROR (
                'ContextID %d does not belong to CustomerID %d. Insert rejected.',
                16, 1, @ContextId, @CustomerId
            );
            RETURN;
        END
    END


    -----------------------------------------
    -- Insert new unique label
    -----------------------------------------
    INSERT INTO Labels (fk_CustomerId, Name, Description)
    VALUES (@CustomerId, @Name, @Description);

    SET @LabelId = SCOPE_IDENTITY();


    -----------------------------------------
    -- Insert date with valid relation
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        INSERT INTO LabelData (fk_LabelId, fk_ContextId, Value)
        VALUES (@LabelId, @ContextId, @Value);
    END
    -----------------------------------------
    -- EF Core requires the identity returned
    -----------------------------------------
    SELECT Id 
    FROM Labels 
    WHERE Id = @LabelId;
END;

#21
-- Issue: 120
ALTER TRIGGER [dbo].[trg_vw_Label_Update]
ON [dbo].[vw_Label]
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -----------------------------------------
    -- Only one row in update
    -----------------------------------------
    IF (SELECT COUNT(*) FROM inserted) > 1
        BEGIN
            RAISERROR('ERROR: Only single row update is allowed', 16,1);
            RETURN;
        END


    DECLARE
        @LabelId INT,
        @CustomerId INT,
        @ContextId INT,
        @Name NVARCHAR(255),
        @Description NVARCHAR(255),
        @Value NVARCHAR(255);

    -----------------------------------------
    -- Read data from Inserted
    -----------------------------------------
    SELECT
        @LabelId = i.Id,
        @Name = i.Name,
        @Description = i.Description,
        @Value = i.Value,
        @ContextId = i.ContextID,
        @CustomerId = cu.Id
    FROM inserted i
    JOIN Customers cu ON cu.CustomerID = i.CustomerID;


    -----------------------------------------
    -- Validate that update doesn't break unique rule
    -----------------------------------------
    IF EXISTS (
        SELECT 1
        FROM Labels
        WHERE fk_CustomerId = @CustomerId
            AND Name = @Name
            AND Id <> @LabelId     
    )
    BEGIN
        RAISERROR(
            'Label "%s" already exists for CustomerID %d. Update rejected.',
            16, 1, @Name, @CustomerId
        );
        RETURN;
    END


    -----------------------------------------
    -- Validate relation between customerID and ContextID
    -----------------------------------------
    IF @ContextId IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM Context
            WHERE Id = @ContextId
                AND fk_CustomerId = @CustomerId
        )
        BEGIN
            RAISERROR(
                'ContextID %d does not belong to CustomerID %d. Update rejected.',
                16, 1, @ContextId, @CustomerId
            );
            RETURN;
        END
    END


    -----------------------------------------
    -- Update Labels
    -----------------------------------------
    UPDATE Labels
    SET
        Name = @Name,
        Description = @Description,
        fk_CustomerId = @CustomerId
    WHERE Id = @LabelId;


    -----------------------------------------
    -- 4. Update or create
    -----------------------------------------
    -- if contextid is null
    if @ContextId IS NULL
        BEGIN
            DELETE FROM LabelData 
            WHERE fk_LabelId = @LabelId 
            RETURN;
        END

    -- If exists do update
    UPDATE LabelData
    SET
        fk_ContextId = @ContextId,
        Value = @Value
    WHERE fk_LabelId = @LabelId;

    -- If not exists insert new
    IF NOT EXISTS (
        SELECT 1 FROM LabelData WHERE fk_LabelId = @LabelId
    )
    AND @ContextId IS NOT NULL
        BEGIN
            INSERT INTO LabelData (fk_LabelId, fk_ContextId, Value)
            VALUES (@LabelId, @ContextId, @Value);
        END
END;

#22
-- Issue: 120
BEGIN TRAN
    INSERT INTO Context(fk_CustomerId,fk_ContextTypeId,[Name])
    VALUES
    (2,1,'Testing Context');

    INSERT INTO ContextData(fk_ContextId,fk_ContextDataTypeId,KeyName,[Value])
    VALUES
    (1,1,'Context Data','Context Data Value');
COMMIT TRAN;

#23
-- Issue: 50
-- Older database images recorded changes 18/19 without the Users table.
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id int PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(255) UNIQUE NOT NULL,
        [Password] NVARCHAR(255) NOT NULL,
        [Role] NVARCHAR(255) NOT NULL
    );

    INSERT INTO dbo.Users (Username, [Password], [Role])
    VALUES ('user', 'pass', 'USER');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
INSERT INTO Users(Username, [Password], [Role])
VALUES
('admin','adminpass','ADMIN');
