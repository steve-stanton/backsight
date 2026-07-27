namespace Backsight.Model;

/// <written by="Steve Stanton" on="02-NOV-2007" />
/// <summary>
/// Line geometry that <b>cannot</b> act as the base for <see cref="SectionGeometry"/>
/// </summary>
abstract class UnsectionedLineGeometry : LineGeometry
{
    /// <summary>
    /// Creates a new <c>UnsectionedLineGeometry</c> using the supplied terminals.
    /// </summary>
    /// <param name="start">The start of the line.</param>
    /// <param name="end">The end of the line.</param>
    protected UnsectionedLineGeometry(ITerminal start, ITerminal end)
        : base(start, end)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsectionedLineGeometry"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    protected UnsectionedLineGeometry(EditDeserializer editDeserializer)
        : base(editDeserializer)
    {
    }

    /// <summary>
    /// The geometry that acts as the base for this one is <c>this</c>
    /// </summary>
    internal override UnsectionedLineGeometry SectionBase => this;

    /// <summary>
    /// The line geometry that corresponds to a section of a line.
    /// </summary>
    /// <param name="s">The required section</param>
    /// <returns>The corresponding geometry for the section</returns>
    internal abstract UnsectionedLineGeometry Section(ISection s);
}