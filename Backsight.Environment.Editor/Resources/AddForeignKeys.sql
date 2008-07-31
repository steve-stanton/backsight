SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_UserJob_Job]') AND parent_object_id = OBJECT_ID(N'[ced].[UserJobs]'))
ALTER TABLE [ced].[UserJobs]  WITH CHECK ADD  CONSTRAINT [FK_UserJob_Job] FOREIGN KEY([JobId])
REFERENCES [ced].[Jobs] ([JobId])
GO
ALTER TABLE [ced].[UserJobs] CHECK CONSTRAINT [FK_UserJob_Job]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[ced].[FK_UserJob_User]') AND parent_object_id = OBJECT_ID(N'[ced].[UserJobs]'))
ALTER TABLE [ced].[UserJobs]  WITH CHECK ADD  CONSTRAINT [FK_UserJob_User] FOREIGN KEY([UserId])
REFERENCES [ced].[Users] ([UserId])
GO
ALTER TABLE [ced].[UserJobs] CHECK CONSTRAINT [FK_UserJob_User]
GO
