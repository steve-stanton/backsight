namespace Backsight.Model;

/// <summary>
/// Some sort of object that is tagged with something
/// </summary>
/// <typeparam name="S">The type of the object</typeparam>
/// <typeparam name="T">The type for the tag</typeparam>
class TaggedObject<S,T>
{
    readonly S m_Thing;
    readonly T m_Tag;

    internal TaggedObject(S thing, T tag)
    {
        m_Thing = thing;
        m_Tag = tag;
    }

    internal T Tag => m_Tag;

    internal S Thing => m_Thing;
}