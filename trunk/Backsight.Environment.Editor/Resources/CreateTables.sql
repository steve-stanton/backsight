SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT ON
GO

--
-- Create the database schema
--

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'ced')
BEGIN
PRINT 'CREATE SCHEMA ced';
EXEC sys.sp_executesql N'CREATE SCHEMA [ced] AUTHORIZATION [dbo]'
END

GO

--
-- Create tables
--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[ColumnDomains]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE ColumnDomains';
CREATE TABLE [ced].[ColumnDomains]
(
  [TableId] [int] NOT NULL,
  [ColumnName] [varchar](100) NOT NULL,
  [DomainId] [int] NOT NULL,

  CONSTRAINT [PK_ColumnDomains] PRIMARY KEY CLUSTERED ([TableId] ASC, [ColumnName] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[DomainTables]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE DomainTables';
CREATE TABLE [ced].[DomainTables]
(
  [DomainId] [int] NOT NULL,
  [TableName] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DomainTables] PRIMARY KEY CLUSTERED ([DomainId] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
  
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Edits]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Edits';
CREATE TABLE [ced].[Edits]
(
	[SessionId] [int] NOT NULL,
	[EditSequence] [int] NOT NULL,
	[Data] [xml] NOT NULL,

	CONSTRAINT [PK_Edits] PRIMARY KEY CLUSTERED ([SessionId] ASC, [EditSequence] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[EntityTypes]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE EntityTypes';
CREATE TABLE [ced].[EntityTypes]
(
	[EntityId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[IsPoint] [char](1) NOT NULL,
	[IsLine] [char](1) NOT NULL,
	[IsLineTopological] [char](1) NOT NULL,
	[IsPolygon] [char](1) NOT NULL,
	[IsText] [char](1) NOT NULL,
	[FontId] [int] NOT NULL,
	[LayerId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[IsLineTrimmed] [char](1) NOT NULL CONSTRAINT DF_EntityTypes_1 DEFAULT ('n'),
	
	CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED ([EntityId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE EntityTypeSchemas';
CREATE TABLE [ced].[EntityTypeSchemas]
(
	[EntityId] [int] NOT NULL,
	[SchemaId] [int] NOT NULL,
	
	CONSTRAINT [PK_EntityTypeSchemas] PRIMARY KEY CLUSTERED ([EntityId] ASC, [SchemaId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Fonts]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Fonts';
CREATE TABLE [ced].[Fonts]
(
	[FontId] [int] NOT NULL,
	[TypeFace] [varchar](24) NOT NULL,
	[PointSize] [real] NOT NULL,
	[IsBold] [char](1) NOT NULL,
	[IsItalic] [char](1) NOT NULL,
	[IsUnderline] [char](1) NOT NULL,
	[FontFile] [varchar](50) NOT NULL,
	
	CONSTRAINT [PK_Fonts] PRIMARY KEY CLUSTERED ([FontId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdAllocations]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE IdAllocations';
CREATE TABLE [ced].[IdAllocations]
(
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TimeAllocated] [datetime] NOT NULL,
	[NumUsed] [int] NOT NULL,

	CONSTRAINT [PK_IdAllocations] PRIMARY KEY CLUSTERED ([LowestId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdFree]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE IdFree';
CREATE TABLE [ced].[IdFree]
(
	[GroupId] [int] NOT NULL,
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,

	CONSTRAINT [PK_IdFree] PRIMARY KEY CLUSTERED ([GroupId] ASC, [LowestId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdGroups]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE IdGroups';
CREATE TABLE [ced].[IdGroups]
(
	[GroupId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,
	[PacketSize] [int] NOT NULL,
	[CheckDigit] [char](1) NOT NULL,
	[KeyFormat] [varchar](8) NOT NULL,

	CONSTRAINT [PK_IdGroups] PRIMARY KEY CLUSTERED ([GroupId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Jobs]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Jobs';
CREATE TABLE [ced].[Jobs]
(
	[JobId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ZoneId] [int] NOT NULL,
	[LayerId] [int] NOT NULL,
	
	CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED ([JobId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Layers]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Layers';
CREATE TABLE [ced].[Layers]
(
	[LayerId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ThemeId] [int] NOT NULL,
	[ThemeSequence] [int] NOT NULL,
	[DefaultPointId] [int] NOT NULL,
	[DefaultLineId] [int] NOT NULL,
	[DefaultPolygonId] [int] NOT NULL,
	[DefaultTextId] [int] NOT NULL,

	CONSTRAINT [PK_Layers] PRIMARY KEY CLUSTERED ([LayerId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Properties]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Properties';
CREATE TABLE [ced].[Properties]
(
	[Name] [varchar](50) NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[Description] [varchar](200) NOT NULL,

	 CONSTRAINT [PK_Properties] PRIMARY KEY CLUSTERED ([Name] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Schemas]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Schemas';
CREATE TABLE [ced].[Schemas]
(
	[SchemaId] [int] NOT NULL,
	[TableName] [varchar](100) NOT NULL,
	[IdColumnName] [varchar](100) NOT NULL,

	CONSTRAINT [PK_Schemas] PRIMARY KEY CLUSTERED ([SchemaId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[SchemaTemplates]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE SchemaTemplates';
CREATE TABLE [ced].[SchemaTemplates]
(
	[SchemaId] [int] NOT NULL,
	[TemplateId] [int] NOT NULL,
	
	CONSTRAINT [PK_SchemaTemplates] PRIMARY KEY CLUSTERED ([SchemaId] ASC, [TemplateId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Sessions]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Sessions';
CREATE TABLE [ced].[Sessions]
(
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[NumItem] [int] NOT NULL,
	
	CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[SysId]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE SysId';
CREATE TABLE [ced].[SysId]
(
	[LastId] [int] NOT NULL,
	
	CONSTRAINT [PK_SysId] PRIMARY KEY CLUSTERED ([LastId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Templates]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Templates';
CREATE TABLE [ced].[Templates]
(
	[TemplateId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[TemplateFormat] [varchar](500) NOT NULL,
	
	CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED ([TemplateId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Themes]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Themes';
CREATE TABLE [ced].[Themes]
(
	[ThemeId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	
	CONSTRAINT [PK_Themes] PRIMARY KEY CLUSTERED ([ThemeId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Users]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Users';
CREATE TABLE [ced].[Users]
(
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,

	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Zones]') AND type in (N'U'))
BEGIN
PRINT 'CREATE TABLE Zones';
CREATE TABLE [ced].[Zones]
(
	[ZoneId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	
	CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED ([ZoneId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO


--
-- Define simple checks
--

PRINT 'Adding simple checks...';

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeSchemaCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]'))
ALTER TABLE [ced].[EntityTypeSchemas]  WITH CHECK ADD  CONSTRAINT [EntityTypeSchemaCheck1] CHECK  (([EntityId]>(0)))
GO
ALTER TABLE [ced].[EntityTypeSchemas] CHECK CONSTRAINT [EntityTypeSchemaCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeSchemaCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]'))
ALTER TABLE [ced].[EntityTypeSchemas]  WITH CHECK ADD  CONSTRAINT [EntityTypeSchemaCheck2] CHECK  (([SchemaId]>(0)))
GO
ALTER TABLE [ced].[EntityTypeSchemas] CHECK CONSTRAINT [EntityTypeSchemaCheck2]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck1] CHECK  (([EntityId]>=(0)))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck2] CHECK  (([IsPoint]='n' OR [IsPoint]='y'))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck3] CHECK  (([IsLine]='n' OR [IsLine]='y'))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck3]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck4]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck4] CHECK  (([IsLineTopological]='n' OR [IsLineTopological]='y'))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck4]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck5]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck5] CHECK  (([IsPolygon]='n' OR [IsPolygon]='y'))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck5]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck6]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck6] CHECK  (([IsText]='n' OR [IsText]='y'))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck6]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck7]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck7] CHECK  (([FontId]>=(0)))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck7]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeCheck8]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH NOCHECK ADD  CONSTRAINT [EntityTypeCheck8] CHECK  (([GroupId]>=(0)))
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [EntityTypeCheck8]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[FontCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Fonts]'))
ALTER TABLE [ced].[Fonts]  WITH NOCHECK ADD  CONSTRAINT [FontCheck1] CHECK  (([FontId]>=(0)))
GO
ALTER TABLE [ced].[Fonts] CHECK CONSTRAINT [FontCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[FontCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[Fonts]'))
ALTER TABLE [ced].[Fonts]  WITH NOCHECK ADD  CONSTRAINT [FontCheck2] CHECK  (([IsBold]='n' OR [IsBold]='y'))
GO
ALTER TABLE [ced].[Fonts] CHECK CONSTRAINT [FontCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[FontCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[Fonts]'))
ALTER TABLE [ced].[Fonts]  WITH NOCHECK ADD  CONSTRAINT [FontCheck3] CHECK  (([IsItalic]='n' OR [IsItalic]='y'))
GO
ALTER TABLE [ced].[Fonts] CHECK CONSTRAINT [FontCheck3]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[FontCheck4]') AND parent_object_id = OBJECT_ID(N'[ced].[Fonts]'))
ALTER TABLE [ced].[Fonts]  WITH NOCHECK ADD  CONSTRAINT [FontCheck4] CHECK  (([IsUnderline]='n' OR [IsUnderline]='y'))
GO
ALTER TABLE [ced].[Fonts] CHECK CONSTRAINT [FontCheck4]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdAllocationCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [IdAllocationCheck1] CHECK  (([LowestId]>(0)))
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [IdAllocationCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdAllocationCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [IdAllocationCheck2] CHECK  (([HighestId]>(0)))
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [IdAllocationCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdAllocationCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [IdAllocationCheck3] CHECK  (([GroupId]>(0)))
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [IdAllocationCheck3]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdAllocationCheck4]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [IdAllocationCheck4] CHECK  (([NumUsed]>=(0)))
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [IdAllocationCheck4]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdFreeCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[IdFree]'))
ALTER TABLE [ced].[IdFree]  WITH CHECK ADD  CONSTRAINT [IdFreeCheck1] CHECK  (([GroupId]>(0)))
GO
ALTER TABLE [ced].[IdFree] CHECK CONSTRAINT [IdFreeCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdFreeCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[IdFree]'))
ALTER TABLE [ced].[IdFree]  WITH CHECK ADD  CONSTRAINT [IdFreeCheck2] CHECK  (([LowestId]>(0)))
GO
ALTER TABLE [ced].[IdFree] CHECK CONSTRAINT [IdFreeCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdFreeCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[IdFree]'))
ALTER TABLE [ced].[IdFree]  WITH CHECK ADD  CONSTRAINT [IdFreeCheck3] CHECK  (([HighestId]>(0)))
GO
ALTER TABLE [ced].[IdFree] CHECK CONSTRAINT [IdFreeCheck3]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdGroupCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[IdGroups]'))
ALTER TABLE [ced].[IdGroups]  WITH NOCHECK ADD  CONSTRAINT [IdGroupCheck1] CHECK  (([GroupId]>=(0)))
GO
ALTER TABLE [ced].[IdGroups] CHECK CONSTRAINT [IdGroupCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdGroupCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[IdGroups]'))
ALTER TABLE [ced].[IdGroups]  WITH NOCHECK ADD  CONSTRAINT [IdGroupCheck2] CHECK  (([LowestId]>=(0)))
GO
ALTER TABLE [ced].[IdGroups] CHECK CONSTRAINT [IdGroupCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdGroupCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[IdGroups]'))
ALTER TABLE [ced].[IdGroups]  WITH NOCHECK ADD  CONSTRAINT [IdGroupCheck3] CHECK  (([HighestId]>=(0)))
GO
ALTER TABLE [ced].[IdGroups] CHECK CONSTRAINT [IdGroupCheck3]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdGroupCheck4]') AND parent_object_id = OBJECT_ID(N'[ced].[IdGroups]'))
ALTER TABLE [ced].[IdGroups]  WITH NOCHECK ADD  CONSTRAINT [IdGroupCheck4] CHECK  (([PacketSize]>=(0)))
GO
ALTER TABLE [ced].[IdGroups] CHECK CONSTRAINT [IdGroupCheck4]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[IdGroupCheck5]') AND parent_object_id = OBJECT_ID(N'[ced].[IdGroups]'))
ALTER TABLE [ced].[IdGroups]  WITH NOCHECK ADD  CONSTRAINT [IdGroupCheck5] CHECK  (([CheckDigit]='n' OR [CheckDigit]='y'))
GO
ALTER TABLE [ced].[IdGroups] CHECK CONSTRAINT [IdGroupCheck5]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[LayerCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH NOCHECK ADD  CONSTRAINT [LayerCheck1] CHECK  (([LayerId]>=(0)))
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [LayerCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[LayerCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH NOCHECK ADD  CONSTRAINT [LayerCheck2] CHECK  (([ThemeId]>=(0)))
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [LayerCheck2]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[LayerCheck3]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH NOCHECK ADD  CONSTRAINT [LayerCheck3] CHECK  (([ThemeSequence]>=(0)))
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [LayerCheck3]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[SchemaCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Schemas]'))
ALTER TABLE [ced].[Schemas]  WITH CHECK ADD  CONSTRAINT [SchemaCheck1] CHECK  (([SchemaId]>(0)))
GO
ALTER TABLE [ced].[Schemas] CHECK CONSTRAINT [SchemaCheck1]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[SchemaTemplateCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[SchemaTemplates]'))
ALTER TABLE [ced].[SchemaTemplates]  WITH CHECK ADD  CONSTRAINT [SchemaTemplateCheck1] CHECK  (([SchemaId]>(0)))
GO
ALTER TABLE [ced].[SchemaTemplates] CHECK CONSTRAINT [SchemaTemplateCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[SchemaTemplateCheck2]') AND parent_object_id = OBJECT_ID(N'[ced].[SchemaTemplates]'))
ALTER TABLE [ced].[SchemaTemplates]  WITH CHECK ADD  CONSTRAINT [SchemaTemplateCheck2] CHECK  (([TemplateId]>(0)))
GO
ALTER TABLE [ced].[SchemaTemplates] CHECK CONSTRAINT [SchemaTemplateCheck2]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[TemplateCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Templates]'))
ALTER TABLE [ced].[Templates]  WITH CHECK ADD  CONSTRAINT [TemplateCheck1] CHECK  (([TemplateId]>=(0)))
GO
ALTER TABLE [ced].[Templates] CHECK CONSTRAINT [TemplateCheck1]
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[ThemeCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Themes]'))
ALTER TABLE [ced].[Themes]  WITH NOCHECK ADD  CONSTRAINT [ThemeCheck1] CHECK  (([ThemeId]>=(0)))
GO
ALTER TABLE [ced].[Themes] CHECK CONSTRAINT [ThemeCheck1]
GO

--
-- Insert initial rows (to support foreign keys, because I don't want to make important
-- columns nullable)
--

PRINT 'Inserting initial rows...';

INSERT INTO [ced].[Fonts] ([FontId], [TypeFace], [PointSize], [IsBold], [IsItalic], [IsUnderline], [FontFile])
VALUES (0, '', 0.0, 'n', 'n', 'n', '')
GO

INSERT INTO [ced].[IdGroups] ([GroupId], [Name], [LowestId], [HighestId], [PacketSize], [CheckDigit], [KeyFormat])
VALUES (0, '', 0, 0, 0, 'n', '{0}')
GO

--
-- It's more convenient if the empty entity type is associated with all spatial types (when loading things like
-- a combobox, it means the blank entity type will show by default).
--
INSERT INTO [ced].[EntityTypes] ([EntityId], [Name], [IsPoint], [IsLine], [IsLineTopological], [IsPolygon], [IsText], [FontId], [LayerId], [GroupId])
VALUES (0, '', 'y', 'y', 'n', 'y', 'y', 0, 0, 0)
GO

INSERT INTO [ced].[Themes] ([ThemeId], [Name])
VALUES (0, '')
GO

INSERT INTO [ced].[Layers] ([LayerId], [Name], [ThemeId], [ThemeSequence], [DefaultPointId], [DefaultLineId], [DefaultPolygonId], [DefaultTextId])
VALUES(0, '', 0, 0, 0, 0, 0, 0)
GO

--
-- Add initial row for generating environment-related IDs
--

INSERT INTO [ced].[SysId] ([LastId]) VALUES (0)
GO

--
-- Define foreign keys
--

PRINT 'Adding foreign keys...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Edit_Session]') AND parent_object_id = OBJECT_ID(N'[ced].[Edits]'))
ALTER TABLE [ced].[Edits]  WITH CHECK ADD  CONSTRAINT [FK_Edit_Session] FOREIGN KEY([SessionId])
REFERENCES [ced].[Sessions] ([SessionId])
ON DELETE CASCADE
GO
ALTER TABLE [ced].[Edits] CHECK CONSTRAINT [FK_Edit_Session]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_EntityType_Font]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH CHECK ADD  CONSTRAINT [FK_EntityType_Font] FOREIGN KEY([FontId])
REFERENCES [ced].[Fonts] ([FontId])
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [FK_EntityType_Font]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_EntityType_IdGroup]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH CHECK ADD  CONSTRAINT [FK_EntityType_IdGroup] FOREIGN KEY([GroupId])
REFERENCES [ced].[IdGroups] ([GroupId])
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [FK_EntityType_IdGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_EntityType_Layer]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypes]'))
ALTER TABLE [ced].[EntityTypes]  WITH CHECK ADD  CONSTRAINT [FK_EntityType_Layer] FOREIGN KEY([LayerId])
REFERENCES [ced].[Layers] ([LayerId])
GO
ALTER TABLE [ced].[EntityTypes] CHECK CONSTRAINT [FK_EntityType_Layer]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_EntityTypeSchema_EntityType]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]'))
ALTER TABLE [ced].[EntityTypeSchemas]  WITH CHECK ADD  CONSTRAINT [FK_EntityTypeSchema_EntityType] FOREIGN KEY([EntityId])
REFERENCES [ced].[EntityTypes] ([EntityId])
GO
ALTER TABLE [ced].[EntityTypeSchemas] CHECK CONSTRAINT [FK_EntityTypeSchema_EntityType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_EntityTypeSchema_Schema]') AND parent_object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]'))
ALTER TABLE [ced].[EntityTypeSchemas]  WITH CHECK ADD  CONSTRAINT [FK_EntityTypeSchema_Schema] FOREIGN KEY([SchemaId])
REFERENCES [ced].[Schemas] ([SchemaId])
GO
ALTER TABLE [ced].[EntityTypeSchemas] CHECK CONSTRAINT [FK_EntityTypeSchema_Schema]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_IdAllocation_IdGroup]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [FK_IdAllocation_IdGroup] FOREIGN KEY([GroupId])
REFERENCES [ced].[IdGroups] ([GroupId])
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [FK_IdAllocation_IdGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_IdAllocation_Job]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [FK_IdAllocation_Job] FOREIGN KEY([JobId])
REFERENCES [ced].[Jobs] ([JobId])
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [FK_IdAllocation_Job]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_IdAllocation_User]') AND parent_object_id = OBJECT_ID(N'[ced].[IdAllocations]'))
ALTER TABLE [ced].[IdAllocations]  WITH CHECK ADD  CONSTRAINT [FK_IdAllocation_User] FOREIGN KEY([UserId])
REFERENCES [ced].[Users] ([UserId])
GO
ALTER TABLE [ced].[IdAllocations] CHECK CONSTRAINT [FK_IdAllocation_User]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_IdFree_IdGroup]') AND parent_object_id = OBJECT_ID(N'[ced].[IdFree]'))
ALTER TABLE [ced].[IdFree]  WITH CHECK ADD  CONSTRAINT [FK_IdFree_IdGroup] FOREIGN KEY([GroupId])
REFERENCES [ced].[IdGroups] ([GroupId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Job_Layer]') AND parent_object_id = OBJECT_ID(N'[ced].[Jobs]'))
ALTER TABLE [ced].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Job_Layer] FOREIGN KEY([LayerId])
REFERENCES [ced].[Layers] ([LayerId])
GO
ALTER TABLE [ced].[Jobs] CHECK CONSTRAINT [FK_Job_Layer]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Job_Zone]') AND parent_object_id = OBJECT_ID(N'[ced].[Jobs]'))
ALTER TABLE [ced].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Job_Zone] FOREIGN KEY([ZoneId])
REFERENCES [ced].[Zones] ([ZoneId])
GO
ALTER TABLE [ced].[Jobs] CHECK CONSTRAINT [FK_Job_Zone]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Layer_EntityType1]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH CHECK ADD  CONSTRAINT [FK_Layer_EntityType1] FOREIGN KEY([DefaultPointId])
REFERENCES [ced].[EntityTypes] ([EntityId])
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [FK_Layer_EntityType1]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Layer_EntityType2]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH CHECK ADD  CONSTRAINT [FK_Layer_EntityType2] FOREIGN KEY([DefaultLineId])
REFERENCES [ced].[EntityTypes] ([EntityId])
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [FK_Layer_EntityType2]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Layer_EntityType3]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH CHECK ADD  CONSTRAINT [FK_Layer_EntityType3] FOREIGN KEY([DefaultPolygonId])
REFERENCES [ced].[EntityTypes] ([EntityId])
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [FK_Layer_EntityType3]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Layer_EntityType4]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH CHECK ADD  CONSTRAINT [FK_Layer_EntityType4] FOREIGN KEY([DefaultTextId])
REFERENCES [ced].[EntityTypes] ([EntityId])
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [FK_Layer_EntityType4]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Layer_Theme]') AND parent_object_id = OBJECT_ID(N'[ced].[Layers]'))
ALTER TABLE [ced].[Layers]  WITH CHECK ADD  CONSTRAINT [FK_Layer_Theme] FOREIGN KEY([ThemeId])
REFERENCES [ced].[Themes] ([ThemeId])
GO
ALTER TABLE [ced].[Layers] CHECK CONSTRAINT [FK_Layer_Theme]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_SchemaTemplate_Schema]') AND parent_object_id = OBJECT_ID(N'[ced].[SchemaTemplates]'))
ALTER TABLE [ced].[SchemaTemplates]  WITH CHECK ADD  CONSTRAINT [FK_SchemaTemplate_Schema] FOREIGN KEY([SchemaId])
REFERENCES [ced].[Schemas] ([SchemaId])
GO
ALTER TABLE [ced].[SchemaTemplates] CHECK CONSTRAINT [FK_SchemaTemplate_Schema]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_SchemaTemplate_Template]') AND parent_object_id = OBJECT_ID(N'[ced].[SchemaTemplates]'))
ALTER TABLE [ced].[SchemaTemplates]  WITH CHECK ADD  CONSTRAINT [FK_SchemaTemplate_Template] FOREIGN KEY([TemplateId])
REFERENCES [ced].[Templates] ([TemplateId])
GO
ALTER TABLE [ced].[SchemaTemplates] CHECK CONSTRAINT [FK_SchemaTemplate_Template]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Session_Job]') AND parent_object_id = OBJECT_ID(N'[ced].[Sessions]'))
ALTER TABLE [ced].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Session_Job] FOREIGN KEY([JobId])
REFERENCES [ced].[Jobs] ([JobId])
GO
ALTER TABLE [ced].[Sessions] CHECK CONSTRAINT [FK_Session_Job]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_Session_User]') AND parent_object_id = OBJECT_ID(N'[ced].[Sessions]'))
ALTER TABLE [ced].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Session_User] FOREIGN KEY([UserId])
REFERENCES [ced].[Users] ([UserId])
GO
ALTER TABLE [ced].[Sessions] CHECK CONSTRAINT [FK_Session_User]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_ColumnDomains_DomainTables]') AND parent_object_id = OBJECT_ID(N'[ced].[ColumnDomains]'))
ALTER TABLE [ced].[ColumnDomains] ADD CONSTRAINT [FK_ColumnDomains_DomainTables] FOREIGN KEY([DomainId])
REFERENCES [ced].[DomainTables] ([DomainId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_ColumnDomains_Schemas]') AND parent_object_id = OBJECT_ID(N'[ced].[ColumnDomains]'))
ALTER TABLE [ced].[ColumnDomains] ADD CONSTRAINT [FK_ColumnDomains_Schemas] FOREIGN KEY([TableId])
REFERENCES [ced].[Schemas] ([SchemaId])
GO
