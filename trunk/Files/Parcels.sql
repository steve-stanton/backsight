USE [Backsight]
GO

CREATE TABLE [ced].[Parcels]
(
  [FeatureId] [varchar](10) NOT NULL,
  [Part] [char](2) NOT NULL,
  [LotId] [varchar](5) NOT NULL,
  [BlockId] [varchar](4) NOT NULL,
  [PlanId] [varchar](7) NOT NULL,
  [ParcelType] [char](1) NOT NULL,
  [PlanType] [char](1) NOT NULL,
  [OriginalIssuingLTO] [char](2) NOT NULL,

  CONSTRAINT [Parcels_PK] PRIMARY KEY NONCLUSTERED ([FeatureId] ASC)
  WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO
