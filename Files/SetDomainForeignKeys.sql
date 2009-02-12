USE [Backsight]
GO

-- Define foreign key constraints for data tables that refer to domains


ALTER TABLE [dbo].[ParishLotParcelData]
ADD CONSTRAINT FK_ParishLotParcelData_DOMAIN_Part_of_Cadastral_Polygon FOREIGN KEY (PART)
REFERENCES dbo.DOMAIN_Part_of_Cadastral_Polygon (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_Part_of_Cadastral_Polygon FOREIGN KEY (PART)
REFERENCES dbo.DOMAIN_Part_of_Cadastral_Polygon (ShortValue)	
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Part_of_Cadastral_Polygon FOREIGN KEY (PART)
REFERENCES dbo.DOMAIN_Part_of_Cadastral_Polygon (ShortValue)	
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Issuing_LTOs FOREIGN KEY (Original_Issuing_LTO)
REFERENCES dbo.DOMAIN_Issuing_LTOs (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_HALVES_of_DLS_QS_Polygons FOREIGN KEY (HALF)
REFERENCES dbo.DOMAIN_HALVES_of_DLS_QS_Polygons (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_DLS_PARCEL_TYPES FOREIGN KEY (DLS_PARCEL_TYPE)
REFERENCES dbo.DOMAIN_DLS_PARCEL_TYPES (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_Quarter_Sections FOREIGN KEY (QS_Value)
REFERENCES dbo.DOMAIN_Quarter_Sections (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_Range_Values FOREIGN KEY (Range_Value)
REFERENCES dbo.DOMAIN_Range_Values (ShortValue)	
GO

ALTER TABLE [dbo].[DLSParcelData]
ADD CONSTRAINT FK_DLSParcelData_DOMAIN_Meridian_Values FOREIGN KEY (Meridian)
REFERENCES dbo.DOMAIN_Meridian_Values (ShortValue)	
GO

ALTER TABLE [dbo].[ParishLotParcelData]
ADD CONSTRAINT FK_ParishLotParcelData_DOMAIN_Parish_Values FOREIGN KEY (Parish)
REFERENCES dbo.DOMAIN_Parish_Values (ShortValue)	
GO

ALTER TABLE [dbo].[ParishLotParcelData]
ADD CONSTRAINT FK_ParishLotParcelData_DOMAIN_Parish_Lot_Types FOREIGN KEY (Par_Lot_Type)
REFERENCES dbo.DOMAIN_Parish_Lot_Types (ShortValue)	
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Parcel_Types FOREIGN KEY (Parcel_Type)
REFERENCES dbo.DOMAIN_Parcel_Types (ShortValue)	
GO

ALTER TABLE [dbo].[PlanParcelData]
ADD CONSTRAINT FK_PlanParcelData_DOMAIN_Plan_Types FOREIGN KEY (Plan_Type)
REFERENCES dbo.DOMAIN_Plan_Types (ShortValue)	
GO

ALTER TABLE [dbo].[WaterBodyData]
ADD CONSTRAINT FK_WaterBodyData_DOMAIN_WaterBodyTypes FOREIGN KEY (WB_Type)
REFERENCES dbo.DOMAIN_WaterBodyTypes (ShortValue)	
GO
