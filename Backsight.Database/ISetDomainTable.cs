namespace Backsight.Database;

public interface ISetDomainTable : ISetter
{
    string TableName { set; }
}