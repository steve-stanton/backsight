USE [Backsight]
GO

-- Get rid of old domain tables

DROP TABLE [dbo].[DOMAIN_DLS_PARCEL_TYPES]
GO
DROP TABLE [dbo].[DOMAIN_HALVES_of_DLS_QS_Polygons]
GO
DROP TABLE [dbo].[DOMAIN_Issuing_LTOs]
GO
DROP TABLE [dbo].[DOMAIN_Meridian_Values]
GO
DROP TABLE [dbo].[DOMAIN_Parcel_Types]
GO
DROP TABLE [dbo].[DOMAIN_Parish_Lot_Types]
GO
DROP TABLE [dbo].[DOMAIN_Parish_Values]
GO
DROP TABLE [dbo].[DOMAIN_Part_of_Cadastral_Polygon]
GO
DROP TABLE [dbo].[DOMAIN_Quarter_Sections]
GO
DROP TABLE [dbo].[DOMAIN_WaterBodyTypes]
GO
DROP TABLE [dbo].[DOMAIN_Plan_Types]
GO
DROP TABLE [dbo].[DOMAIN_Range_Values]
GO


-- Create new domain tables (they need to be reloaded later)

CREATE TABLE [dbo].[DOMAIN_DLS_PARCEL_TYPES]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_DLS_PARCEL_TYPES_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_DLS_PARCEL_TYPES] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_HALVES_of_DLS_QS_Polygons]
(
  [ShortValue] [char](1) NOT NULL CONSTRAINT [DF_DOMAIN_HALVES_of_DLS_QS_Polygons_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_HALVES_of_DLS_QS_Polygons] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Issuing_LTOs]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_Issuing_LTOs_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Issuing_LTOs] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Meridian_Values]
(
  [ShortValue] [char](1) NOT NULL CONSTRAINT [DF_DOMAIN_Meridian_Values_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Meridian_Values] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parcel_Types]
(
  [ShortValue] [char](1) NOT NULL CONSTRAINT [DF_DOMAIN_Parcel_Types_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Parcel_Types] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parish_Lot_Types]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_Parish_Lot_Types_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Parish_Lot_Types] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parish_Values]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_Parish_Values_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Parish_Values] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Part_of_Cadastral_Polygon]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_Part_of_Cadastral_Polygon_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Part_of_Cadastral_Polygon] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Quarter_Sections]
(
  [ShortValue] [char](2) NOT NULL CONSTRAINT [DF_DOMAIN_Quarter_Sections_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Quarter_Sections] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_WaterBodyTypes]
(
  [ShortValue] [char](1) NOT NULL CONSTRAINT [DF_DOMAIN_WaterBodyTypes_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_WaterBodyTypes] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Plan_Types]
(
  [ShortValue] [char](1) NOT NULL CONSTRAINT [DF_DOMAIN_Plan_Types_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Plan_Types] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Range_Values]
(
  [ShortValue] [char](3) NOT NULL CONSTRAINT [DF_DOMAIN_Range_Values_ShortValue]  DEFAULT (''),
  [LongValue] [varchar](50) NOT NULL,
  [Description] [varchar](100) NOT NULL,

  CONSTRAINT [PK_DOMAIN_Range_Values] PRIMARY KEY CLUSTERED ([ShortValue] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO


