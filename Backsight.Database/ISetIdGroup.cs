namespace Backsight.Database;

public interface ISetIdGroup : ISetter
{
    string Name { set; }
    int LowestId { set; }
    int HighestId { set; }
    int PacketSize { set; }
    string KeyFormat { set; }
    bool HasCheckDigit { set; }
}