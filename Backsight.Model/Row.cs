using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// A row of miscellaneous attribute data
/// </summary>
class Row : IPossibleList<Row>
{
    /// <summary>
    /// The ID for the row
    /// </summary>
    readonly FeatureId m_Id;

    /// <summary>
    /// The data for the row 
    /// </summary>
    readonly AttributeRecord m_Record;

    /// <summary>
    /// Initializes a new instance of the <see cref="Row"/> class,
    /// forming a two-way association with the ID
    /// </summary>
    /// <param name="id">The ID for the row (not null). Modified to refer to the newly created <c>Row</c> object.</param>
    /// <param name="data">Data for the row (not null).</param>
    /// <exception cref="ArgumentNullException">If any parameter is null</exception>
    internal Row(FeatureId id, AttributeRecord data)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(data);

        m_Id = id;
        m_Record = data;

        // Relate the ID to this row
        id.AddReference(this);
    }

    public int Count => 1;

    public Row this[int index]
    {
        get
        {
            if (index!=0)
                throw new ArgumentOutOfRangeException();

            return this;
        }
    }

    public IEnumerator<Row> GetEnumerator()
    {
        yield return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator(); // the other one
    }

    public IPossibleList<Row> Add(Row thing)
    {
        return new BasicList<Row>(this, thing);
    }

    public IPossibleList<Row> Remove(Row thing)
    {
        if (!Object.ReferenceEquals(this, thing))
            throw new ArgumentException();

        return null;
    }

    /// <summary>
    /// The ID for the row
    /// </summary>
    internal FeatureId Id => m_Id;

    /// <summary>
    /// The definition of the table this row is part of 
    /// </summary>
    internal ITable Table => m_Record.Table;

    /// <summary>
    /// The data for the row 
    /// </summary>
    internal AttributeRecord Record => m_Record;
}