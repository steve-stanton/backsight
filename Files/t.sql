DECLARE @xsd varchar(MAX)
SELECT @xsd = xmlCol FROM OPENROWSET(Bulk 'C:\\temp\\TestSchema.xsd', SINGLE_CLOB) as results(xmlCol)
create xml schema collection TestSchemaCollection AS @xsd
GO


