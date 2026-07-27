namespace Backsight.Model;

/// <written by="Steve Stanton" on="31-OCT-2011" />
/// <summary>
/// Something that can be persisted using an implementation of <see cref="EditSerializer"/>.
/// </summary>
/// <remarks>
/// Classes that implement this interface are expected to handle deserialization by
/// providing a constructor that accepts an instance of <see cref="EditDeserializer"/>
/// (the intention is to make it possible to tag class members as <c>readonly</c> where
/// that is applicable). Unfortunately, you can't specify constructors as part of an interface.
/// </remarks>
interface IPersistent
{
    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    void WriteData(EditSerializer editSerializer);
}