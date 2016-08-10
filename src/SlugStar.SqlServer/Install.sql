

-- Create the database schema if it doesn't exists
IF NOT EXISTS (SELECT [schema_id] FROM [sys].[schemas] WHERE [name] = '$(SCHEMA_NAME)')
BEGIN
    EXEC (N'CREATE SCHEMA [$(SCHEMA_NAME)]');
    PRINT 'Created database schema [$(SCHEMA_NAME)]';
END
ELSE
    PRINT 'Database schema [$(SCHEMA_NAME)] already exists';
    
DECLARE @SCHEMA_ID int;
SELECT @SCHEMA_ID = [schema_id] FROM [sys].[schemas] WHERE [name] = '$(SCHEMA_NAME)';

-- Create the [$(HangFireSchema)].Schema table if not exists
IF NOT EXISTS(SELECT [object_id] FROM [sys].[tables] 
    WHERE [name] = '$(TABLE_NAME)' AND [schema_id] = @SCHEMA_ID)
BEGIN
	CREATE TABLE [$(SCHEMA_NAME)].[$(TABLE_NAME)] (
	  [Slug] [varchar](500) NOT NULL,
	  [Created] [datetime] NOT NULL,
	  CONSTRAINT [PK_Slugs] PRIMARY KEY CLUSTERED
	  (
	  [Slug] ASC
	  ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
    PRINT 'Created table [$(SCHEMA_NAME)].[$(TABLE_NAME)]';
END
ELSE
    PRINT 'Table [$(SCHEMA_NAME)].[$(TABLE_NAME)] already exists';
