namespace Backsight.Model;

/// <written by="Steve Stanton" on="13-JUN-2008" />
/// <summary>
/// An ID that has been imported from some alien data source.
/// </summary>
/// <seealso cref="NativeId"/>
class ForeignId : FeatureId
{
    /// <summary>
    /// The foreign key used to identify a feature
    /// </summary>
    readonly string m_Key;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignId"/> class.
    /// </summary>
    /// <param name="key">The foreign key used to identify a feature</param>
    internal ForeignId(string key)
    {
        m_Key = key;
    }

    /// <summary>
    /// The user-perceived ID value
    /// </summary>
    internal override string FormattedKey => m_Key;
}