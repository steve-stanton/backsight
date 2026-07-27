using System.Diagnostics;
using Backsight.Environment;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="13-JUL-1997" />
/// <summary>
/// An ID acts as a cross-reference that allows for a many-to-many relationship
/// between map features and rows in associated database tables.
/// </summary>
abstract class FeatureId
{
    /// <summary>
    /// Either a reference to the single feature that has this key, or a reference to
    /// a list of multiple features that have the same key.
    /// </summary>
    IPossibleList<Feature>? m_Features;

    /// <summary>
    /// Either a reference to the single row that has this key, or a reference to
    /// a list of multiple rows that have the same key.
    /// <para/>
    /// There is currently no constraint that says the feature ID column has to be a unique
    /// key within any given table (although that may well be desirable). In situations where
    /// the ID is associated with more than one row, it is therefore possible that
    /// multiple rows come from the same table.
    /// </summary>
    IPossibleList<Row>? m_Rows;

    protected FeatureId()
    {
        m_Features = null;
        m_Rows = null;
    }

    /// <summary>
    /// Relates this ID to the specified feature (and vice versa)
    /// </summary>
    /// <param name="f">The feature that has this ID</param>
    internal void Add(Feature f)
    {
        if (f.FeatureId is not null)
        {
            if (f.FeatureId == this)
                return;

            // TODO: Is this saying that the FeatureId does NOT provide a many:many association (as
            // suggested in the class preamble)?
            f.FeatureId.Cut(f);
        }

        AddReference(f);
        f.SetId(this);
    }

    /// <summary>
    /// Removes the association of this ID with the specified feature
    /// </summary>
    /// <param name="f">The feature that should be assigned a null ID</param>
    internal void Cut(Feature f)
    {
        Debug.Assert(f.FeatureId == this);
        CutReference(f);
        f.SetId(null);
    }

    public override string ToString()
    {
        return FormattedKey;
    }

    /// <summary>
    /// The user-perceived ID value
    /// </summary>
    internal abstract string FormattedKey { get; }

    /// <summary>
    /// The undecorated native ID value (excluding any prefix or suffix or check digit).
    /// This implementation always returns a value of 0. The derived <see cref="NativeId"/>
    /// class provides an override.
    /// </summary>
    internal virtual uint RawId => 0;

    public bool IsInactive => m_Rows is null && m_Features is null;

    internal IPossibleList<Row>? Rows => m_Rows;

    internal int RowCount => m_Rows?.Count ?? 0;

    /// <summary>
    /// Adds a reference from this ID to a row. 
    /// </summary>
    /// <param name="row">The row to point to.</param>
    internal void AddReference(Row row)
    {
        m_Rows = m_Rows?.Add(row) ?? row;

        // Check whether any associated features are instances of TextFeature that
        // have RowTextContent geometry (a placeholder class that is meant to exist
        // only during deserialization from the database). If so, see whether the
        // geometry can now be replaced with the "proper" RowTextGeometry.

        if (m_Features != null)
        {
            foreach (var tf in m_Features.OfType<TextFeature>())
            {
                if (tf.TextGeometry is RowTextContent content)
                {
                    if (content.TableId == row.Table.Id)
                        tf.TextGeometry = new RowTextGeometry(row, content); 
                }
            }
        }
    }

    /// <summary>
    /// Adds a reference from this ID to a spatial feature.
    /// </summary>
    /// <param name="feature">The feature to point to.</param>
    public void AddReference(Feature feature)
    {
        m_Features = m_Features is null ? feature : m_Features.Add(feature);
    }

    /// <summary>
    /// Cuts a reference from this ID to a spatial feature. 
    /// </summary>
    /// <param name="feature">The feature to cut.</param>
    public void CutReference(Feature feature)
    {
        m_Features = m_Features?.Remove(feature);
    }

    /// <summary>
    /// Gets any labels associated with a row to remove themselves from
    /// the spatial index. This should be done just before the row is about to
    /// be changed in some way (a call to <see cref="AddIndex"/> should
    /// be made following the change).
    /// </summary>
    /// <param name="row">The row that is about to change.</param>
    void RemoveIndex(Row row)
    {
        if (m_Features is not null)
        {
            foreach(Feature f in m_Features)
                f.RemoveIndex(row);
        }
    }

    /// <summary>
    /// Gets any labels associated with a row to add themselves into the
    /// spatial index. This should be done after a row has just been changed in
    /// some way.
    ///
    /// This call should be made at some point soon after a prior call
    /// to <c>FeatureId.RemoveIndex</c>
    /// </summary>
    /// <param name="row">The row that has been changed.</param>
    void AddIndex(Row row)
    {
        if (m_Features is not null)
        {
            foreach(Feature f in m_Features)
                f.AddIndex(row);
        }
    }

    /// <summary>
    /// Checks whether this ID is associated with a row of attribute data from
    /// a specific table.
    /// </summary>
    /// <param name="t">The table of interest</param>
    /// <returns>True if this ID object is already associated with the specified table</returns>
    internal bool RefersToTable(ITable t)
    {
        if (m_Rows is not null)
        {
            int tid = t.Id;
            return m_Rows.Any(r => r.Table.Id == tid);
        }

        return false;
    }

    /// <summary>
    /// Locates any text features associated with this ID that have
    /// <see cref="RowTextGeometry"/>.
    /// </summary>
    /// <returns>Any text features associated with this ID that
    /// have a geometry that's dependent on database attributes. May be
    /// an empty array (but never null)</returns>
    internal TextFeature[] GetRowText()
    {
        if (m_Features is null)
            return [];
        
        var result = new List<TextFeature>();

        foreach (var tf in m_Features.OfType<TextFeature>())
        {
            if (tf.TextGeometry is RowTextGeometry)
                result.Add(tf);
        }

        return result.ToArray();
    }
}