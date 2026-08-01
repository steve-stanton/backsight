namespace Backsight.Model;

/// <written by="Steve Stanton" on="04-SEP-2007" />
/// <summary>
/// The geometric parameters relating to a <see cref="Ring"/>
/// </summary>
public class RingMetrics
{
    /// <summary>
    /// Area in square meters. If the ring is an instance of <c>Polygon</c>, this
    /// will exclude the area of any islands.
    /// </summary>
    double m_Area;

    /// <summary>
    /// The spatial extent of the ring.
    /// </summary>
    readonly Window m_Window;

    /// <summary>
    /// Creates a new <c>RingMetrics</c> object for the supplied faces.
    /// </summary>
    /// <param name="edge">The faces defining a ring</param>
    internal RingMetrics(List<Face> edge)
    {
        m_Area = 0.0;
        m_Window = new Window();

        foreach(Face face in edge)
        {
            IDivider d = face.Divider;
            bool isLeft = face.IsLeft;

            double area, length;
            IWindow awin;
            d.LineGeometry.GetGeometry(out awin, out area, out length);
            m_Window.Union(awin);

            if (face.IsLeft)
                m_Area -= area;
            else
                m_Area += area;
        }
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy">The metrics to (shallow) copy</param>
    protected RingMetrics(RingMetrics copy)
    {
        m_Area = copy.m_Area;
        m_Window = copy.m_Window;
    }

    /// <summary>
    /// The signed area of this ring (less than zero if this ring forms the outer edge
    /// of an island).
    /// </summary>
    internal double SignedArea => m_Area;

    /// <summary>
    /// The area of this ring, in square meters on the mapping projection. In the case
    /// of rings that represent the outer edge of islands, the stored area will be less
    /// than zero. However, the <c>Island</c> class overrides this property so that it
    /// will always come back as a positive value. In other words, do not use an area
    /// test to see whether a ring is an island - instead, check whether it is an instance
    /// of <see cref="Island"/>
    /// </summary>
    public virtual double Area => m_Area;

    /// <summary>
    /// Arbitrarily sets the ring area to the supplied value.
    /// </summary>
    /// <param name="value">The value to assign to <c>m_Area</c>. For rings that are instances
    /// of <see cref="Polygon"/>, this must be greater than zero. For rings that are instances
    /// of <see cref="Island"/>, this must be less than zero.</param>
    /// <remarks>I wanted to set the area via the <c>Area</c> property, but the compiler
    /// complains if you try to declare a setter as protected while the getter is internal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">If the supplied <paramref name="value"/>
    /// has a sign that's inconsistent with the ring type.</exception>
    protected void SetArea(double value)
    {
        if ((this is Polygon) && value<0.0)
            throw new ArgumentOutOfRangeException("Polygon area must be greater than zero");

        if ((this is Island) && value>0.0)
            throw new ArgumentOutOfRangeException("Island area must be less than zero");

        m_Area = value;
    }

    /// <summary>
    /// The spatial extent of this object. This is a stored value, so it's cheap to get.
    /// </summary>
    public IWindow Extent => m_Window;
}