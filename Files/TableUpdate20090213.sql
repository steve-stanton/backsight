USE [Backsight]
GO

CREATE TABLE [ced].[DomainTables]
(
  [DomainId] [int] NOT NULL,
  [TableName] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DomainTables] PRIMARY KEY CLUSTERED ([DomainId] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [ced].[ColumnDomains]
(
  [TableId] [int] NOT NULL,
  [ColumnName] [varchar](100) NOT NULL,
  [DomainId] [int] NOT NULL,

  CONSTRAINT [PK_ColumnDomains] PRIMARY KEY CLUSTERED ([TableId] ASC, [ColumnName] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

ALTER TABLE [ced].[ColumnDomains]
ADD CONSTRAINT [FK_ColumnDomains_DomainTables] FOREIGN KEY([DomainId])
REFERENCES [ced].[DomainTables] ([DomainId])
GO

ALTER TABLE [ced].[ColumnDomains]
ADD CONSTRAINT [FK_ColumnDomains_Schemas] FOREIGN KEY([TableId])
REFERENCES [ced].[Schemas] ([SchemaId])
GO

-- Modify Schemas table

ALTER TABLE [ced].[Schemas]
DROP COLUMN [Name]
GO

ALTER TABLE [ced].[Schemas]
ADD [IdColumnName] [varchar](100) NOT NULL CONSTRAINT DF_Schemas_IdColumnName DEFAULT ('PIN')
GO

