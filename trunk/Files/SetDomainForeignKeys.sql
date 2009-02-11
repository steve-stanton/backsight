USE [Backsight]
GO

xx <DOMAIN: "PART of Cadastral Polygon" = Part_of_Cadastral_Polygon - PART in "DLS Parcel Data" "Parish Lot Parcel Data" "Survey, Director of Survey & Condominum Plan Parcel Data"
xx <DOMAIN: "Issuing LTOs" = Issuing_LTOs - Original Issuing LTO in "Survey, Director of Survey & Condominum Plan Parcel Data"
xx <DOMAIN: "HALVES of DLS QS Polygons" = HALVES_of_DLS_QS_Polygons - HALF in "DLS Parcel Data"
xx <DOMAIN: "DLS PARCEL TYPES" = DLS_PARCEL_TYPES - DLS PARCEL TYPE in "DLS Parcel Data"
xx <DOMAIN: "Quarter Sections" = Quarter_Sections - QS VALUE in "DLS Parcel Data"


xx <DOMAIN: "Range Values" = Range_Values - RANGE VALUE in "DLS Parcel Data"
- DLSParcelData.Range_value is varchar(50) ??


ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_Meridian_Values FOREIGN KEY (Meridian)
REFERENCES dbo.DOMAIN_Meridian_Values (ShortValue)	
GO

--

ALTER TABLE [dbo].[ParishLotParcelData]
ALTER COLUMN [Parish] char(2) NOT NULL
GO

ALTER TABLE [dbo].[ParishLotParcelData]
ADD CONSTRAINT FK_ParishLotParcelData_DOMAIN_Parish_Values FOREIGN KEY (Parish)
REFERENCES dbo.DOMAIN_Parish_Values (ShortValue)	
GO

-- 

ALTER TABLE [dbo].[ParishLotParcelData]
ALTER COLUMN [Par_Lot_Type] char(2) NULL
GO

ALTER TABLE [dbo].[ParishLotParcelData]
ADD CONSTRAINT FK_ParishLotParcelData_DOMAIN_Parish_Lot_Types FOREIGN KEY (Par_Lot_Type)
REFERENCES dbo.DOMAIN_Parish_Lot_Types (ShortValue)	
GO

--

UPDATE [dbo].[PlanParcelData]
SET [Parcel_Type] = ''
WHERE [Parcel_Type] IS NULL
GO

ALTER TABLE [dbo].[PlanParcelData]
ALTER COLUMN [Parcel_Type] char(1) NOT NULL
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT [DF_PlanParcelData_Parcel_Type] DEFAULT '' FOR [Parcel_Type]
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Parcel_Types FOREIGN KEY (Parcel_Type)
REFERENCES dbo.DOMAIN_Parcel_Types (ShortValue)	
GO

--

ALTER TABLE dbo.PlanParcelData
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Plan_Types FOREIGN KEY (Plan_Type)
REFERENCES dbo.DOMAIN_Plan_Types (ShortValue)	
GO

ALTER TABLE dbo.WaterBodyData
ADD CONSTRAINT FK_WaterBodyData_DOMAIN_WaterBodyTypes FOREIGN KEY (WB_Type)
REFERENCES dbo.DOMAIN_WaterBodyTypes (ShortValue)	
GO
