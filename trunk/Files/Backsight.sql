USE [Backsight]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05/07/2008 23:24:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Revisions]    Script Date: 05/07/2008 23:24:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Revisions](
	[RevisionId] [int] NOT NULL,
	[CommitTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Revisions] PRIMARY KEY CLUSTERED 
(
	[RevisionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Layers]    Script Date: 05/07/2008 23:24:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Layers](
	[LayerId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Layers] PRIMARY KEY CLUSTERED 
(
	[LayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LayerTree]    Script Date: 05/07/2008 23:24:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LayerTree](
	[LayerId] [int] NOT NULL,
	[ParentLayerId] [int] NOT NULL,
 CONSTRAINT [PK_LayerTree] PRIMARY KEY CLUSTERED 
(
	[LayerId] ASC,
	[ParentLayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Zones]    Script Date: 05/07/2008 23:24:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Zones](
	[ZoneId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED 
(
	[ZoneId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Sessions]    Script Date: 05/07/2008 23:24:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sessions](
	[SessionId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[RevisionId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[NumEdit] [int] NOT NULL,
	[NumFeature] [int] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Edits]    Script Date: 05/07/2008 23:24:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Edits](
	[SessionId] [int] NOT NULL,
	[EditSequence] [int] NOT NULL,
	[Data] [xml] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Jobs]    Script Date: 05/07/2008 23:24:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Jobs](
	[JobId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ZoneId] [int] NOT NULL,
	[LayerId] [int] NOT NULL,
 CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_Edits_Sessions]    Script Date: 05/07/2008 23:24:12 ******/
ALTER TABLE [dbo].[Edits]  WITH CHECK ADD  CONSTRAINT [FK_Edits_Sessions] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Sessions] ([SessionId])
GO
ALTER TABLE [dbo].[Edits] CHECK CONSTRAINT [FK_Edits_Sessions]
GO
/****** Object:  ForeignKey [FK_Jobs_Layer]    Script Date: 05/07/2008 23:24:15 ******/
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Jobs_Layer] FOREIGN KEY([LayerId])
REFERENCES [dbo].[Layers] ([LayerId])
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_Jobs_Layer]
GO
/****** Object:  ForeignKey [FK_Jobs_Zones]    Script Date: 05/07/2008 23:24:15 ******/
ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_Jobs_Zones] FOREIGN KEY([ZoneId])
REFERENCES [dbo].[Zones] ([ZoneId])
GO
ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_Jobs_Zones]
GO
/****** Object:  ForeignKey [FK_Sessions_Jobs]    Script Date: 05/07/2008 23:24:22 ******/
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Jobs] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Jobs]
GO
/****** Object:  ForeignKey [FK_Sessions_Revisions]    Script Date: 05/07/2008 23:24:22 ******/
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Revisions] FOREIGN KEY([RevisionId])
REFERENCES [dbo].[Revisions] ([RevisionId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Revisions]
GO
/****** Object:  ForeignKey [FK_Sessions_Users]    Script Date: 05/07/2008 23:24:22 ******/
ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Users]
GO
