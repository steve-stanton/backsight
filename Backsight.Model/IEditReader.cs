namespace Backsight.Model;

/// <written by="Steve Stanton" on="31-OCT-2011" />
/// <summary>
/// Methods that may be used to load the description of edits (previously written using
/// an implementation of <see cref="IEditWriter"/>).
/// <para/>
/// Implemented by <see cref="TextEditReader"/>.
/// </summary>
interface IEditReader
{
    /// <summary>
    /// Reads the next byte.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The byte value that was read.</returns>
    byte ReadByte(string name);

    /// <summary>
    /// Reads a 4-byte signed integer.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The 4-byte value that was read.</returns>
    int ReadInt32(string name);

    /// <summary>
    /// Reads a 4-byte unsigned integer.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The 4-byte unsigned value that was read.</returns>
    uint ReadUInt32(string name);

    /// <summary>
    /// Reads an 8-byte signed integer.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The 8-byte value that was read.</returns>
    long ReadInt64(string name);

    /// <summary>
    /// Reads an eight-byte floating-point value.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The 8-byte floating-point value that was read.</returns>
    double ReadDouble(string name);

    /// <summary>
    /// Reads a four-byte floating-point value.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The 4-byte floating-point value that was read.</returns>
    float ReadSingle(string name);

    /// <summary>
    /// Reads a one-byte boolean value.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The boolean value that was read.</returns>
    bool ReadBool(string name);

    /// <summary>
    /// Reads a string.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The string that was read (null if nothing follows the name)</returns>
    string? ReadString(string name);

    /// <summary>
    /// Reads a timestamp.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The timestamp that was read.</returns>
    DateTime ReadDateTime(string name);

    /// <summary>
    /// Reads an internal ID.
    /// </summary>
    /// <param name="name">A name tag associated with the value</param>
    /// <returns>The ID that was read.</returns>
    InternalIdValue ReadInternalId(string name);

    /// <summary>
    /// Reads any text that precedes the data values for an object.
    /// </summary>
    void ReadBeginObject();

    /// <summary>
    /// Reads any text that should follow the data values for an object.
    /// </summary>
    void ReadEndObject();

    /// <summary>
    /// Checks whether the next data item has a specific name tag. Make a call to any
    /// <c>Read</c> method to actually advance.
    /// </summary>
    /// <param name="name">The name tag to look for</param>
    /// <returns>True if the next data item has the specified name tag</returns>
    bool IsNextField(string name);
}