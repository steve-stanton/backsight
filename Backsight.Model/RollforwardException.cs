namespace Backsight.Model;

/// <written by="Steve Stanton" on="13-JUN-2007" />
/// <summary>
/// Exception indicating that an editing operation failed to rollforward. This type
/// of exception should be raised only by implementations of the <c>Operation.CalculateGeometry</c>
/// method when performing an update. A <c>RollforwardException</c> is not meant to indicate an
/// application error; it refers to a problem that the user could rectify by altering the
/// observations supplied as part of a map update.
/// </summary>
class RollforwardException : Exception
{
    /// <summary>
    /// The operation that failed to rollforward
    /// </summary>
    readonly Operation m_Problem;

    /// <summary>
    /// Initializes a new instance of the <see cref="RollforwardException"/> class.
    /// </summary>
    /// <param name="op">TThe operation that failed to rollforward.</param>
    /// <param name="msg">Any message that may provide further information (may be blank or null)</param>
    internal RollforwardException(Operation op, string msg)
        : base(msg)
    {
        m_Problem = op;
    }

    /// <summary>
    /// The operation that failed to rollforward
    /// </summary>
    internal Operation Problem => m_Problem;
}