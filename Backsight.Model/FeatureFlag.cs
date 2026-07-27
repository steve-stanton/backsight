namespace Backsight.Model;

/// <summary>
/// Flag bits for the <c>Feature</c> class
/// </summary>
[Flags]
enum FeatureFlag : ushort
{
    /// <summary>
    /// Feature is being rolled back (user is undoing the operation that
    /// created a feature)
    /// </summary>
    Undoing=0x0001,

    /// <summary>
    /// Feature has been superseded. This flag gets set if a feature should no longer
    /// appear on the current editing layer.
    /// </summary>
    Inactive=0x0002,

    /// <summary>
    /// Feature has moved in rollforward
    /// </summary>
    Moved=0x0010,

    /// <summary>
    /// Is this feature topological?
    /// </summary>
    Topol=0x0080,

    /// <summary>
    /// Does line represent a staggered property face?
    /// </summary>
    Void=0x0200,

    /// <summary>
    /// Does the feature's ID come from a foreign source (i.e. import)?
    /// </summary>
    ForeignId=0x0400,

    /// <summary>
    /// Is the feature locked (i.e. should not be edited).
    /// </summary>
    Locked=0x0800,

    /// <summary>
    /// Topology completely defined
    /// </summary>
    Built=0x1000,

    /// <summary>
    /// Line object marked as trimmed
    /// </summary>
    Trim=0x2000,

    /// <summary>
    /// Is the feature spatially indexed?
    /// </summary>
    Indexed=0x4000,
};