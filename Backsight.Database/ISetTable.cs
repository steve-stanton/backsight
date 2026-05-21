namespace Backsight.Database;

public interface ISetTable : ISetter
{
    string TableName { set; }
    string IdColumnName { set; }
}