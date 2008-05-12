USE [Backsight]
GO
/****** Object:  Table [dbo].[Edits]    Script Date: 05/12/2008 13:40:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DECLARE @xsd varchar(MAX)
SELECT @xsd = xmlCol FROM OPENROWSET(Bulk 'C:\\temp\\EditingSchema.xsd', SINGLE_CLOB) as results(xmlCol)
create xml schema collection BacksightSchemaCollection AS @xsd
GO

CREATE TABLE [dbo].[Edits](
	[SessionId] [int] NOT NULL,
	[EditSequence] [int] NOT NULL,
	[Data] [xml] (CONTENT [dbo].[BacksightSchemaCollection]) NULL,
 CONSTRAINT [PK_Edits] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC,
	[EditSequence] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Jobs](
	[JobId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ZoneId] [int] NOT NULL,
	[LayerId] [int] NOT NULL,
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Sessions](
	[SessionId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[RevisionId] [int],
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[NumEdit] [int] NOT NULL,
	[NumFeature] [int] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[Edits]  WITH CHECK ADD  CONSTRAINT [FK_Edits_Sessions] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Sessions] ([SessionId])
GO
ALTER TABLE [dbo].[Edits] CHECK CONSTRAINT [FK_Edits_Sessions]
GO
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Jobs_Layers] FOREIGN KEY([LayerId])
REFERENCES [dbo].[Layer] ([LayerId])
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_Jobs_Layers]
GO
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Jobs_Zones] FOREIGN KEY([ZoneId])
REFERENCES [dbo].[Zones] ([ZoneId])
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_Jobs_Zones]
GO
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Jobs] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Jobs]
GO
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Revision] FOREIGN KEY([RevisionId])
REFERENCES [dbo].[Sessions] ([SessionId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Revision]
GO
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Users]