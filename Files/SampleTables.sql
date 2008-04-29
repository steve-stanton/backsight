USE [Backsight]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

-- Data that describes the survey monument placed in the field

CREATE TABLE [MonumentData]
(
  Id int not null,
  Monument char(2) not null, -- Type of Monument placed at this point

  CONSTRAINT [PK_MonumentData] PRIMARY KEY CLUSTERED ([Id] ASC)
  WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]
Go

-- List of known monument types

CREATE TABLE [MonumentType]
(
  Monument char(2) not null,
  [[Name]] varchar(50) not null,
  Description varchar(200) not null,

  CONSTRAINT [PK_MonumentType] PRIMARY KEY CLUSTERED ([Monument] ASC)
  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]

ALTER TABLE [dbo].[MonumentData] WITH CHECK ADD CONSTRAINT [FK_Monument_Domain] FOREIGN KEY([Monument])
REFERENCES [dbo].[MonumentType] ([Monument])
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
(' ', 'UNDEFINED', 'Monument is currently not set')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('A', 'Manitoba Government Survey Post in Pipe', 'Standard brass cap cemented in the top of a 1 inch diameter by four feet long iron pipe driven into the ground to a depth of at least 3 feet')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('B', 'Manitoba Government Survey Post in Concrete', 'Standard brass cap set flush with the top of a concrete slab one foot square three feet to four feet in depth - to top of the monument being approximately four inches above ground level')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('C', 'Manitoba Government Survey Post in Concrete', 'Standard brass cap cemented into a hole drilled into rock - the bottom of the cap being flush with the rock level')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('D', 'Solid iron post 1 1/8"', 'Solid iron post 1 1/8" x 1 1/8" - minimum 48" long - marked M.L.S.')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('E', 'Solid iron post 1"', 'Solid iron post 1" x 1" - minimum 36" long - marked M.L.S.')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('F', 'Solid iron post 3/4" - sq. top', 'Solid iron post 3/4" x 3/4" - minimum 30" long - or a 30" long hollow tube with 3/4" x 3/4" x 4" square top')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('G', 'Solid iron post 3/4" - tri. top', 'Solid iron post 3/4" x 3/4" - minimum 30" long - or a 30" long hollow tube with 3/4" x 3/4" x 4" triangular top')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('A1', 'I.P.', 'Iron Post')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('A2', 'Wo.P.', 'Wooden Post')
GO

INSERT INTO [MonumentType] (Monument, [[Name]], Description) VALUES
('A3', 'M.P.', 'Marker Post')
GO

INSERT INTO [MonumentType] (Monument, [Name], Description) VALUES
('A4', 'C.L.S.P.', 'Canada Land Survey Post (Brass Cap)')
GO

INSERT INTO [MonumentType] (Monument, [Name], Description) VALUES
('A5', 'D.L.S.P.', 'Dominion Land Survey Post (Old Standard Iron Post)')
GO

-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


-- Points which have been used as control points for cadastral mapping

CREATE TABLE [ControlData]
(
  Id int not null,
  Location varchar(12) not null, -- General location of point
  Datum varchar(9) not null, -- Datum/Adjustment of Point Computation

  CONSTRAINT [PK_ControlData] PRIMARY KEY CLUSTERED ([Id] ASC)
  WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]
Go


-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

-- +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
