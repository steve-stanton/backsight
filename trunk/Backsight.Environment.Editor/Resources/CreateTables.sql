USE [$(DBNAME)]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- Create the database schema
--

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'ced')
EXEC sys.sp_executesql N'CREATE SCHEMA [ced] AUTHORIZATION [dbo]'

GO

--
-- Create tables
--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Fonts]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Fonts]
(
	[FontId] [int] NOT NULL,
	[TypeFace] [varchar](24) NOT NULL,
	[PointSize] [real] NOT NULL,
	[IsBold] [char](1) NOT NULL,
	[IsItalic] [char](1) NOT NULL,
	[IsUnderline] [char](1) NOT NULL,
	[FontFile] [varchar](50) NOT NULL,
	
	CONSTRAINT [FontKey] PRIMARY KEY CLUSTERED ([FontId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdFree]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[IdFree]
(
	[GroupId] [int] NOT NULL,
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,

	CONSTRAINT [IdFreeKey] PRIMARY KEY CLUSTERED ([GroupId] ASC, [LowestId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdGroups]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[IdGroups]
(
	[GroupId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,
	[PacketSize] [int] NOT NULL,
	[CheckDigit] [char](1) NOT NULL,
	[KeyFormat] [varchar](8) NOT NULL,

	CONSTRAINT [IdGroupKey] PRIMARY KEY CLUSTERED ([GroupId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Properties]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Properties]
(
	[Name] [varchar](50) NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[Description] [varchar](200) NOT NULL,

	 CONSTRAINT [PropertyKey] PRIMARY KEY CLUSTERED ([Name] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Schemas]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Schemas]
(
	[SchemaId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[TableName] [varchar](100) NOT NULL,

	CONSTRAINT [SchemaKey] PRIMARY KEY CLUSTERED ([SchemaId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[SysId]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[SysId]
(
	[LastId] [int] NOT NULL,
	
	CONSTRAINT [PK_SysId] PRIMARY KEY CLUSTERED ([LastId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Themes]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Themes]
(
	[ThemeId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	
	CONSTRAINT [ThemeKey] PRIMARY KEY CLUSTERED ([ThemeId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Templates]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Templates]
(
	[TemplateId] [int] NOT NULL,
	[Name] [varchar](80) NOT NULL,
	[TemplateFormat] [varchar](500) NOT NULL,
	
	CONSTRAINT [TemplateKey] PRIMARY KEY CLUSTERED ([TemplateId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Users]') AND type in (N'U'))
BEGIN
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
CREATE TABLE [ced].[Zones]
(
	[ZoneId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	
	CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED ([ZoneId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[LastRevision]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[LastRevision]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RevisionTime] [datetime] NOT NULL
	
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Layers]') AND type in (N'U'))
BEGIN
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

	CONSTRAINT [LayerKey] PRIMARY KEY CLUSTERED ([LayerId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[EntityTypeSchemas]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[EntityTypeSchemas]
(
	[EntityId] [int] NOT NULL,
	[SchemaId] [int] NOT NULL,
	
	CONSTRAINT [EntitySchemaKey] PRIMARY KEY CLUSTERED ([EntityId] ASC, [SchemaId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[EntityTypes]') AND type in (N'U'))
BEGIN
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
	
	CONSTRAINT [EntityKey] PRIMARY KEY CLUSTERED ([EntityId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[IdAllocations]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[IdAllocations]
(
	[LowestId] [int] NOT NULL,
	[HighestId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[TimeAllocated] [datetime] NOT NULL,
	[NumUsed] [int] NOT NULL,

	CONSTRAINT [IdAllocationKey] PRIMARY KEY CLUSTERED ([LowestId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Jobs]') AND type in (N'U'))
BEGIN
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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[SchemaTemplates]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[SchemaTemplates]
(
	[SchemaId] [int] NOT NULL,
	[TemplateId] [int] NOT NULL,
	
	CONSTRAINT [SchemaTemplateKey] PRIMARY KEY CLUSTERED ([SchemaId] ASC, [TemplateId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[UserJobs]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[UserJobs]
(
	[UserId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[LastRevision] [int] NOT NULL,
	
	CONSTRAINT [PK_UserJobs] PRIMARY KEY CLUSTERED ([UserId] ASC, [JobId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Sessions]') AND type in (N'U'))
BEGIN
CREATE TABLE [ced].[Sessions]
(
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Revision] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[NumItem] [int] NOT NULL,
	
	CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC)
		WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
		
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ced].[Edits]') AND type in (N'U'))
BEGIN
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

--
-- Define simple checks
--

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
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[SchemaCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Schemas]'))
ALTER TABLE [ced].[Schemas]  WITH CHECK ADD  CONSTRAINT [SchemaCheck1] CHECK  (([SchemaId]>(0)))
GO
ALTER TABLE [ced].[Schemas] CHECK CONSTRAINT [SchemaCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[ThemeCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Themes]'))
ALTER TABLE [ced].[Themes]  WITH NOCHECK ADD  CONSTRAINT [ThemeCheck1] CHECK  (([ThemeId]>=(0)))
GO
ALTER TABLE [ced].[Themes] CHECK CONSTRAINT [ThemeCheck1]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[ced].[TemplateCheck1]') AND parent_object_id = OBJECT_ID(N'[ced].[Templates]'))
ALTER TABLE [ced].[Templates]  WITH CHECK ADD  CONSTRAINT [TemplateCheck1] CHECK  (([TemplateId]>=(0)))
GO
ALTER TABLE [ced].[Templates] CHECK CONSTRAINT [TemplateCheck1]
GO



--
-- Insert initial rows (to support foreign keys, because I don't want to make important
-- columns nullable)
--

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
