DECLARE @xsd varchar(MAX)
SELECT @xsd = xmlCol FROM OPENROWSET(Bulk 'C:\\temp\\EditingSchema.xsd', SINGLE_CLOB) as results(xmlCol)
create xml schema collection BacksightSchemaCollection AS @xsd

insert into test (id,data) values (1, '<?xml version="1.0"?>
<Thing xmlns="http://Backsight/Test.xsd" A="1"/>
')


=============================================


USE [Backsight]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[bsp_GetNextRevision]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Result int;
	DELETE FROM [dbo].[LastRevision];
	INSERT INTO [dbo].[LastRevision] (RevisionTime) VALUES (GETDATE());
	SET @Result = SCOPE_IDENTITY();
	RETURN @Result;
END

=============================================

DECLARE @rev AS int;
exec @rev = dbo.bsp_GetNextRevision;
PRINT @rev;

using (SqlConnection conn = new SqlConnection(connectionString))
{
    using (SqlCommand cmd = new SqlCommand("dbo.TestReturn"))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@Invalue", 3));

        SqlParameter returnValue = new SqlParameter("@Return_Value", DbType.Int32);
        returnValue.Direction = ParameterDirection.ReturnValue;

        cmd.Parameters.Add(returnValue);

        conn.Open();
        cmd.Connection = conn;

        cmd.ExecuteNonQuery();
        int count = Int32.Parse(cmd.Parameters["@Return_Value"].Value.ToString());
        Response.Write("<p>Return Code: " + count.ToString());
        conn.Close();
    }
}