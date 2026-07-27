namespace Backsight.Model;

/// <written by="Steve Stanton" on="13-JUN-2008" />
/// <summary>
/// An ID that corresponds to an item within an <see cref="IdGroup"/>
/// </summary>
/// <seealso cref="ForeignId"/>
class NativeId : FeatureId
{
    /// <summary>
    /// Gets the check digit for a numeric key. 
    /// </summary>
    /// <param name="num">The numeric key</param>
    /// <returns></returns>
    internal static uint GetCheckDigit(uint num)
    {
        uint val = num;
        uint total;			// The total for one iteration

        for (; val>9; val=total)
        {
            for (total=0; val!=0; val /= 10)
            {
                total += (val % 10);
            }
        }

        return val;
    }

    /// <summary>
    /// The associated ID group
    /// </summary>
    readonly IdGroup m_Group;

    /// <summary>
    /// The undecorated ID value (excluding any prefix or suffix or check digit).
    /// </summary>
    readonly uint m_Key;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeId"/> class.
    /// </summary>
    /// <param name="group">The ID group that <paramref name="key"/> is part of.</param>
    /// <param name="key">The raw ID value that identifies a feature.</param>
    internal NativeId(IdGroup group, uint key)
    {
        m_Group = group;
        m_Key = key;
    }

    /// <summary>
    /// The user-perceived ID value (may be decorated with adornments specified via the associated ID group).
    /// </summary>
    internal override string FormattedKey => m_Group.FormatId(m_Key);

    /// <summary>
    /// The undecorated ID value (excluding any prefix or suffix or check digit).
    /// </summary>
    internal override uint RawId => m_Key;

    /// <summary>
    /// The associated ID group
    /// </summary>
    internal IdGroup IdGroup => m_Group;
}