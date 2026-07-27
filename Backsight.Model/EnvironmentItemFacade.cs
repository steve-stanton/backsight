using System.ComponentModel;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Fronts an instance of some object that implements an interface that extends
/// <c>IEnvironmentItem</c>. Whereas the facade is considered to be something
/// that needs to be persisted, the associated object is not.
/// <para>
/// The facade does not know how to obtain the associated object. Upon re-retrieval
/// of a facade from wherever it is stored, the associated object must be re-defined
/// by higher-level application logic (if you don't do this, the properties exposed
/// by the facade will correspond to default values).
/// </para></summary>
/// <typeparam name="D">The type for the associated object</typeparam>
class EnvironmentItemFacade<D> : DataStub where D : IEnvironmentItem
{
    /// <summary>
    /// Utility method for searching a list for a specific ID
    /// </summary>
    /// <param name="list">The list to search</param>
    /// <param name="id">The ID to look for</param>
    /// <returns>The first matching item in the list (null if not found)</returns>
    internal static D FindById(IList<D> list, int id)
    {
        foreach (D d in list)
        {
            if (d.Id == id)
                return d;
        }

        return default(D);
    }

    /// <summary>
    /// The object to delegate to. Must be set using the <c>Data</c> property
    /// before working with the facade (otherwise results will correspond to
    /// default values).
    /// </summary>
    private D m_Data;

    /// <summary>
    /// Creates a brand new <c>SchemaItemFacade</c> that corresponds to some
    /// object retrieved from the schema database.
    /// </summary>
    /// <param name="data">The object this facade will delegate to.</param>
    /// <returns>A new (persistent) facade that fronts the supplied object.</returns>
    /// <exception cref="ArgumentNullException">If the delegate is null, or has
    /// an ID of zero.</exception>
    protected EnvironmentItemFacade(D data)
        : base(data.Id)
    {
        m_Data = data ?? throw new ArgumentNullException();

        // If we've been supplied a facade, drill down to get to the real data.
        while (m_Data is EnvironmentItemFacade<D> f)
        {
            m_Data = f.Data;
        }
    }

    /// <summary>
    /// The object to delegate to. If you don't set the property before working with
    /// this facade, all other properties will have their default values.
    /// </summary>
    [Browsable(false)]
    public D Data
    {
        get => m_Data;
        set => m_Data = value;
    }
}