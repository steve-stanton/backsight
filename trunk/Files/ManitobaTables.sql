USE [Backsight]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DOMAIN_DLS_PARCEL_TYPES]
(
  [Field_Value] [char](2) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](50) NOT NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_HALVES_of_DLS_QS_Polygons]
(
  [Field_Value] [char](1) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](80) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Issuing_LTOs]
(
  [Field Value] [char](2) NULL,
  [Field Name] [varchar](50) NOT NULL,
  [Field Description] [varchar](50) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Meridian_Values]
(
  [Field_Value] [char](1) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](80) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parcel_Types]
(
  [Field_Value] [char](1) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](80) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parish_Lot_Types]
(
  [Field_Value] [char](2) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](50) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Parish_Values]
(
  [Field_Value] [char](2) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](50) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Part_of_Cadastral_Polygon]
(
  [Field Value] [char](2) NULL,
  [Field Name] [varchar](50) NOT NULL,
  [Field Description] [varchar](50) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[CertificateofTitleParcelData]
(
  [PIN] [char](9) NOT NULL,
  [Certificate_of_Title_Name] [varchar](12) NULL,
  [Parcel_ID] [varchar](3) NULL,

  CONSTRAINT [CertificateofTitleParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Quarter_Sections]
(
  [Field_Value] [char](2) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](50) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[WaterBodyData]
(
  [PIN] [char](9) NOT NULL,
  [WB_Name] [varchar](40) NOT NULL,
  [WB_Type] [char](1) NOT NULL,

  CONSTRAINT [WaterBodyData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_WaterBodyTypes]
(
  [Field_Value] [char](1) NOT NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](80) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[JudgesOrderParcelData]
(
  [PIN] [char](9) NOT NULL,
  [Judges_Order_ID] [varchar](12) NOT NULL,

  CONSTRAINT [JudgesOrderParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[LegalInstrumentParcelData]
(
  [PIN] [char](9) NOT NULL,
  [Legal_Instrument_ID] [varchar](11) NOT NULL,

  CONSTRAINT [LegalInstrumentParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[ParishLotParcelData]
(
  [PIN] [char](9) NOT NULL,
  [PART] [varchar](2) NULL,
  [Par_Lot_ID] [varchar](5) NULL,
  [Par_Lot_Type] [varchar](2) NULL,
  [Parish] [varchar](2) NOT NULL,

  CONSTRAINT [ParishLotParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[PropertyMapPolygonData]
(
  [PIN] [char](9) NOT NULL,
  [Name] [varchar](25) NOT NULL,

  CONSTRAINT [aaaaaPropertyMapPolygonData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[PublicLaneData]
(
  [PIN] [char](9) NOT NULL,
  [NAME] [varchar](8) NULL,

  CONSTRAINT [PublicLaneData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[PublicWalkData]
(
  [PIN] [char](9) NOT NULL,
  [NAME] [varchar](8) NULL,

  CONSTRAINT [PublicWalkData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[StreetData]
(
  [PIN] [char](9) NOT NULL,
  [Street_Name] [varchar](40) NOT NULL,

  CONSTRAINT [StreetData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[PlanParcelData]
(
  [PIN] [char](9) NOT NULL,
  [PART] [char](2) NULL,
  [Lot_ID] [varchar](5) NULL,
  [Block_ID] [varchar](4) NULL,
  [Plan_ID] [varchar](7) NOT NULL,
  [Parcel_Type] [char](1) NULL,
  [Plan_Type] [char](1) NOT NULL,
  [Original_Issuing_LTO] [char](2) NULL,

  CONSTRAINT [PlanParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Plan_Types]
(
  [Field Value] [char](1) NULL,
  [Field Name] [varchar](50) NOT NULL,
  [Field Description] [varchar](80) NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DOMAIN_Range_Values]
(
  [Field_Value] [char](3) NULL,
  [Field_Name] [varchar](50) NOT NULL,
  [Field_Description] [varchar](50) NOT NULL
)
ON [PRIMARY]
GO

CREATE TABLE [dbo].[DLSParcelData]
(
  [PIN] [char](9) NOT NULL,
  [PART] [char](2) NULL,
  [HALF] [varchar](50) NULL,
  [DLS_PARCEL_TYPE] [char](1) NULL,
  [DLS_LOT_num] [varchar](3) NULL,
  [DLS_LEGAL_SUB_DIVISION_num] [int] NOT NULL DEFAULT 0,
  [QS_VALUE] [char](2) NULL,
  [SECTION_num] [int] NOT NULL DEFAULT 0,
  [TOWNSHIP_num] [int] NOT NULL DEFAULT 0,
  [RANGE_VALUE] [varchar](50) NULL,
  [MERIDIAN] [char](1) NULL,

  CONSTRAINT [DLSParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
  WITH (PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[DLSParcelData] WITH CHECK ADD CONSTRAINT [CK_DLSParcelData_DLS_LEGAL_SUB_DIVISION_num]
CHECK ([DLS_LEGAL_SUB_DIVISION_num]>=0 AND [DLS_LEGAL_SUB_DIVISION_num]<=16)
GO

ALTER TABLE [dbo].[DLSParcelData] WITH CHECK ADD CONSTRAINT [CK_DLSParcelData_SECTION_num]
CHECK (SECTION_num]>=0 AND [SECTION_num]<=36)
GO

ALTER TABLE [dbo].[DLSParcelData] WITH CHECK ADD CONSTRAINT [CK_DLSParcelData_TOWNSHIP_num]
CHECK ([TOWNSHIP_num]>=1 AND [TOWNSHIP_num]<=126)
GO
