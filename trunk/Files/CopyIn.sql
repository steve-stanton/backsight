BULK INSERT [dbo].[PlanParcelData] FROM 'C:\Temp\A10.txt'
WITH (DATAFILETYPE='char');
GO

bcp Backsight.dbo.PlanParcelData in A10.txt -f PlanParcelData.xml -T -S localhost\sqlexpress