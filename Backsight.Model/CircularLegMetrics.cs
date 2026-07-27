using Backsight.Model.Observations;

namespace Backsight.Model;

/// <summary>
/// The observations for an instance of <see cref="CircularLeg"/>
/// </summary>
class CircularLegMetrics
{
    /// <summary>
    /// First angle. Either at the BC, or a central angle. In radians. It's
    /// a central angle if the <see cref="IsCulDeSac"/> property is true.
    /// </summary>
    double m_Angle1;

    /// <summary>
    /// The angle at the EC (in radians). This will only be defined if the FLG_TWOANGLES
    /// flag bit is set (if not set, this value will be 0.0).
    /// </summary>
    double m_Angle2;

    /// <summary>
    /// Observed radius.
    /// </summary>
    Distance m_Radius;

    /// <summary>
    /// Flag bits
    /// </summary>
    CircularLegFlag m_Flag;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularLegMetrics"/> class with
    /// undefined angles (both set to zero).
    /// </summary>
    /// <param name="radius">The observed radius.</param>
    /// <param name="isClockwise">Is the leg directed clockwise?</param>
    internal CircularLegMetrics(Distance radius, bool isClockwise)
    {
        m_Angle1 = m_Angle2 = 0.0;
        m_Radius = radius;

        // Remember if its NOT a clockwise arc.
        if (!isClockwise)
            m_Flag |= CircularLegFlag.CounterClockwise;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularLegMetrics"/> class
    /// for a cul-de-sac.
    /// </summary>
    /// <param name="radius">The observed radius.</param>
    /// <param name="isClockwise">Is the leg directed clockwise?</param>
    /// <param name="centralAngle">The central angle, in radians.</param>
    internal CircularLegMetrics(Distance radius, bool isClockwise, double centralAngle)
        : this(radius, isClockwise)
    {
        SetCentralAngle(centralAngle);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularLegMetrics"/> class
    /// for a "normal" circular arc.
    /// </summary>
    /// <param name="radius">The observed radius.</param>
    /// <param name="isClockwise">Is the leg directed clockwise?</param>
    /// <param name="entryAngle">The angle at the BC, in radians.</param>
    /// <param name="exitAngle">The angle at the EC, in radians.</param>
    internal CircularLegMetrics(Distance radius, bool isClockwise, double entryAngle, double exitAngle)
        : this(radius, isClockwise)
    {
        SetEntryAngle(entryAngle);
        SetExitAngle(exitAngle);
    }

    /// <summary>
    /// Is the leg directed clockwise?
    /// </summary>
    internal bool IsClockwise
    {
        get => (m_Flag & CircularLegFlag.CounterClockwise) == 0;
        set => SetFlag(CircularLegFlag.CounterClockwise, !value);
    }

    /// <summary>
    /// Observed radius.
    /// </summary>
    internal Distance ObservedRadius => m_Radius;

    /// <summary>
    /// Records the radius of this leg.
    /// </summary>
    /// <param name="radius">The radius to assign.</param>
    internal void SetRadius(Distance radius)
    {
        m_Radius = radius;
    }

    /// <summary>
    /// The central angle for this leg (assuming the <see cref="IsCulDeSac"/> property is true)
    /// </summary>
    internal double CentralAngle => m_Angle1;

    /// <summary>
    /// Sets the central angle for this leg. The leg will be flagged as being a cul-de-sac. 
    /// </summary>
    /// <param name="cangle">The central angle, in radians.</param>
    internal void SetCentralAngle(double cangle)
    {
        // Store the central angle.
        m_Angle1 = cangle;

        // The other angle is unused.
        m_Angle2 = 0.0;
        IsTwoAngles = false;

        // This leg is a cul-de-sac
        IsCulDeSac = true;
    }

    /// <summary>
    /// The entry angle for this leg (assuming it isn't a cul-de-sac -
    /// see the <see cref="IsCulDeSac"/> property)
    /// </summary>
    internal double EntryAngle => m_Angle1;

    /// <summary>
    /// Sets the entry (BC) angle. Note that when setting both the entry
    /// and exit angles, this function should be called BEFORE a call to
    /// <see cref="SetExitAngle"/>. This function should NOT be called
    /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
    /// </summary>
    /// <param name="bangle">The angle to assign, in radians.</param>
    internal void SetEntryAngle(double bangle)
    {
        // Store the specified angle.
        m_Angle1 = bangle;

        // This leg is not a cul-de-sac.
        IsCulDeSac = false;
    }

    /// <summary>
    /// The exit angle for this leg (assuming it isn't a cul-de-sac -
    /// see the <see cref="IsCulDeSac"/> property)
    /// </summary>
    internal double ExitAngle => IsTwoAngles ? m_Angle2 : m_Angle1;

    /// <summary>
    /// Sets the exit (BC) angle. Note that when setting both the entry and
    /// exit angles, this function should be called AFTER a call to
    /// <see cref="SetEntryAngle"/>. This function should NOT be called
    /// for cul-de-sacs (use <see cref="SetCentralAngle"/> instead).
    /// </summary>
    /// <param name="eangle">The angle to assign, in radians.</param>
    internal void SetExitAngle(double eangle)
    {
        // If the angle is the same as the entry angle, store an
        // undefined exit angle, and set the flag bit to indicate
        // that only the entry angle is valid.

        if (Math.Abs(m_Angle1 - eangle) < MathConstants.TINY)
        {
            m_Angle2 = 0.0;
            IsTwoAngles = false;
        }
        else
        {
            m_Angle2 = eangle;
            IsTwoAngles = true;
        }

        // This leg is not a cul-de-sac.
        IsCulDeSac = false;
    }

    /// <summary>
    /// Does this leg have two angles?
    /// </summary>
    internal bool IsTwoAngles
    {
        get => (m_Flag & CircularLegFlag.TwoAngles) != 0;
        private set => SetFlag(CircularLegFlag.TwoAngles, value);
    }

    /// <summary>
    /// Is the leg flagged as a cul-de-sac?
    /// </summary>
    internal bool IsCulDeSac
    {
        get => (m_Flag & CircularLegFlag.CulDeSac) != 0;
        private set => SetFlag(CircularLegFlag.CulDeSac, value);
    }

    /// <summary>
    /// Sets flag bit(s)
    /// </summary>
    /// <param name="flag">The flag bit(s) to set</param>
    /// <param name="setting">True to set, false to clear</param>
    void SetFlag(CircularLegFlag flag, bool setting)
    {
        if (setting)
            m_Flag |= flag;
        else
            m_Flag &= (~flag);
    }
}