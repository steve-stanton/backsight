DECLARE @xsd varchar(MAX)
SELECT @xsd = xmlCol FROM OPENROWSET(Bulk 'C:\\temp\\EditingSchema.xsd', SINGLE_CLOB) as results(xmlCol)
create xml schema collection BacksightSchemaCollection AS @xsd

insert into test (id,data) values (1, '<?xml version="1.0"?>
<Thing xmlns="http://Backsight/Test.xsd" A="1"/>
')