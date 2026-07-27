namespace Backsight.Model;

class ForwardRefArrayItem
{
    internal int ArrayIndex { get; private set; }
    internal InternalIdValue InternalId { get; private set; }
    internal Feature Feature { get; set; }

    internal ForwardRefArrayItem(InternalIdValue id, int arrayIndex)
    {
        InternalId = id;
        ArrayIndex = arrayIndex;
        Feature = null;
    }        
}