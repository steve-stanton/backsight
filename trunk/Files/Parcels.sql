USE [Backsight]
GO

CREATE TABLE [dbo].[PlanParcelData]
(
	[PIN] [varchar](9) NOT NULL,
	[PART] [char](2) NULL,
	[Lot_ID] [varchar](5) NULL,
	[Block_ID] [varchar](4) NULL,
	[Plan_ID] [varchar](7) NOT NULL,
	[Parcel_Type] [char](1) NULL,
	[Plan_Type] [char](1) NOT NULL,
	[Original_Issuing_LTO] [char](2) NULL,
    
    CONSTRAINT [PlanParcelData_PK] PRIMARY KEY NONCLUSTERED ([PIN] ASC)
    WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]

) ON [PRIMARY]
GO
