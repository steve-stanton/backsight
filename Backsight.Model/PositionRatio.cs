using System.Diagnostics;

namespace Backsight.Model;

/// <summary>
/// The position ratio of a point along a line (expressed as a value between 0 and 1 billion).
/// </summary>
public static class PositionRatio
{
    /// <summary>
    /// The max value stored for <c>m_PositionRatio</c>
    /// </summary>
    const uint MAX_POSITION_RATIO = 1000000000;

    /// <summary>
    /// Obtains the position ratio for a position that is coincident with a line.
    /// </summary>
    /// <param name="line">The line the position is coincident with</param>
    /// <param name="posn">The position on the line</param>
    /// <returns>The position ratio of the position, expressed in the numeric range
    /// expected by this editing operation.</returns>
    /// <exception cref="ArgumentException">If the position does not appear to coincide
    /// with the supplied line.</exception>
    public static uint GetValue(LineFeature line, IPosition posn)
    {
        // Get the distance to the supplied position (confirming that it does fall on the line)
        LineGeometry g = line.LineGeometry;
        double lineLen = g.Length.Meters;
        double posnLen = g.GetLength(posn).Meters;
        if (posnLen < 0.0)
            throw new ArgumentException("Position does not appear to coincide with line.");

        // Express the position as a position ratio in the range [0,1 billion]
        double prat = posnLen/lineLen;
        uint result = (uint)(prat * (double)MAX_POSITION_RATIO);
        Debug.Assert(result <= MAX_POSITION_RATIO);
        return result;
    }

    /// <summary>
    /// Does the value returned by a <see cref="GetValue"/> represent a position that is
    /// more than halfway along the line?
    /// </summary>
    /// <param name="value">The position ratio value to test.</param>
    /// <returns>True if the value means the position is more than halfway along the line.</returns>
    public static bool IsMoreThanMidway(uint value)
    {
        return value > MAX_POSITION_RATIO/2;
    }

    /// <summary>
    /// Calculates the actual distance along a line based on the supplied position ratio.
    /// </summary>
    /// <param name="line">The line of interest.</param>
    /// <param name="value">A position ratio that has been calculated (presumably for the same line).</param>
    /// <returns>The distance (in meters) that corresponds to the supplied position ratio.</returns>
    public static double GetDistance(LineFeature line, uint value)
    {
        double len = line.Length.Meters;
        return len * (double)(value) / (double)MAX_POSITION_RATIO;
    }
}