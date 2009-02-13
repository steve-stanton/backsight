USE [Backsight]
GO

CREATE TABLE [ced].[DomainTables]
(
  [DomainId] [int] NOT NULL,
  [TableName] [varchar](100) NOT NULL,
  [LookupColumnName] [varchar](100) NOT NULL,
  [ValueColumnName] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DomainTables] PRIMARY KEY CLUSTERED ([DomainId] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [ced].[TableDomains]
(
  [TableId] [int] NOT NULL,
  [ColumnName] [varchar](100) NOT NULL,
  [DomainId] [int] NOT NULL,

  CONSTRAINT [PK_TableDomains] PRIMARY KEY CLUSTERED ([TableId] ASC, [ColumnName] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

ALTER TABLE [ced].[TableDomains]
ADD CONSTRAINT [FK_TableDomains_DomainTables] FOREIGN KEY([DomainId])
REFERENCES [ced].[DomainTables] ([DomainId])
GO

ALTER TABLE [ced].[TableDomains]
ADD CONSTRAINT [FK_TableDomains_Schemas] FOREIGN KEY([TableId])
REFERENCES [ced].[Schemas] ([SchemaId])
GO

