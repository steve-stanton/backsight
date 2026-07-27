using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Fronts an instance of some object that implements <c>IIdGroup</c>.
/// </summary>
class IdGroupFacade : EnvironmentItemFacade<IIdGroup>, IIdGroup
{
    internal IdGroupFacade(IIdGroup data) : base(data)
    {
    }

    public override string ToString()
    {
        return Name;
    }

    public string Name => Data.Name;

    public int LowestId => Data.LowestId;

    public int HighestId => Data.HighestId;

    public int MaxUsedId => Data.MaxUsedId;

    public int PacketSize => Data.PacketSize;

    public string KeyFormat => Data.KeyFormat;

    public bool HasCheckDigit => Data.HasCheckDigit;

    public IEntity[] EntityTypes => Data.EntityTypes;
}