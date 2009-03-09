/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Backsight.Environment;
using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="13-JUL-1997" />
    /// <summary>
    /// A point feature (e.g. control point, any sort of computed point). A point feature must
    /// exist at both ends of every <see cref="LineFeature"/>.
    /// </summary>
    class PointFeature : Feature, IPoint, ITerminal
    {
        #region Class data

        /// <summary>
        /// The geometry for this point. Could conceivably be shared by more
        /// than one point (although it is expected that 99% of points will not
        /// be shared). This value could be null while in the process of deserializing
        /// data.
        /// </summary>
        Node m_Geom;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public PointFeature()
        {
        }

        /// <summary>
        /// Creates a new <c>PointFeature</c> with geometry that isn't shared
        /// with any other point.
        /// </summary>
        /// <param name="g">The geometry for the point (may be null, although this is only really
        /// expected during deserialization)</param>
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        internal PointFeature(PointGeometry g, IEntity e, Operation creator)
            : base(e, creator)
        {
            if (g == null)
                m_Geom = null;
            else
                m_Geom = new Node(this, g);
        }

        /// <summary>
        /// Creates a new <c>PointFeature</c> that is coincident with an existing
        /// point. The new point will share the geometry of the existing point.
        /// </summary>
        /// <param name="f">The point feature that the new point coincides with (not null)</param>
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        internal PointFeature(PointFeature f, IEntity e, Operation creator)
            : base(e, creator)
        {
            if (f == null)
                throw new ArgumentNullException("Cannot create shared point feature");

            m_Geom = f.m_Geom;
            m_Geom.AttachPoint(this);
        }

        #endregion

        public double X
        {
            get { return m_Geom.X; }
        }

        public double Y
        {
            get { return m_Geom.Y; }
        }

        public bool IsAt(IPosition p, double tol)
        {
            return Position.IsCoincident(this, p, tol);
        }

        public bool IsCoincident(PointFeature p)
        {
            return m_Geom.IsCoincident(p.Geometry);
        }

        public override SpatialType SpatialType
        {
            get { return SpatialType.Point; }
        }

        /// <summary>
        /// Try to find an attached circle that has a specific radius.
        ///	This method is intended for locations that appear at the center
        ///	of a circle.
        /// </summary>
        /// <param name="radius">The radius to search for (0 or less to
        /// search for ANY circle). A fixed tolerance of 0.001 meters on
        /// the ground is used.</param>
        /// <returns>The attached circle (null if no such circle).</returns>
        internal Circle GetCircle(double radius)
        {
            List<IFeatureDependent> deps = this.Dependents;
            if (deps == null)
                return null;

            foreach (IFeatureDependent d in deps)
            {
                if (d is Circle)
                {
                    Circle c = (d as Circle);
                    if (radius<Double.Epsilon)
                        return c;

                    if (Math.Abs(c.Radius - radius) < 0.001)
                        return c;
                }
            }

            return null;
        }

        /// <summary>
        /// The circles centered at this point
        /// </summary>
        /// <returns>The circles with a center that coincides with this point</returns>
        internal Circle[] GetCircles()
        {
            List<Circle> result = new List<Circle>();

            foreach (IFeatureDependent d in Dependents)
            {
                if (d is Circle)
                    result.Add((Circle)d);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Moves the location of this point.
        /// </summary>
        /// <param name="to">The new position.</param>
        /// <returns>True if the geometry associated with this point was moved. False
        /// if the move wasn't necessary.</returns>
        internal bool Move(IPosition to)
        {
            PointGeometry g = PointGeometry.Create(to);
            return Move(g);
        }

        internal bool Move(PointGeometry to)
        {
            // Just return if the new location is at the same position
            // as the old location.
            IPointGeometry g = this.Geometry;
            if (g.IsCoincident(to))
                return false;

            // If the point coincides with an interior vertex on a
            // multi-segment, update the multi-segment to refer to a
            // new location... ignore

            // Remove this point from spatial index
            //CadastralMapModel map = this.MapModel;
            //IEditSpatialIndex index = (IEditSpatialIndex)map.Index;
            //index.Remove(this);

            // Notify all dependents that this point is about to move (and remove
            // this feature from spatial index)
            PreMove();

            // Move the geometry for this feature and re-insert into the index
            //(g as PointGeometry).Move(to);
            // alternatively, just assign the supplied geometry ?
            PointFeature[] pts = m_Geom.Points;
            m_Geom = new Node(pts, to);
            //index.Add(this);

            // Notify all dependents that this point has been moved (and add
            // this feature back into the spatial index)
            PostMove();

            return true;
        }

        /// <summary>
        /// The geometry defining the position of this feature.
        /// </summary>
        public IPointGeometry Geometry // IPoint
        {
            get { return m_Geom; }
        }

        /// <summary>
        /// The geometry defining the position of this feature.
        /// </summary>
        internal PointGeometry PointGeometry
        {
            get { return m_Geom; }
            set
            {
                if (value==null)
                    throw new ArgumentNullException();

                m_Geom = new Node(this, value);
            }
        }

        /// <summary>
        /// The geometry defining the position of this feature.
        /// </summary>
        internal Node Node
        {
            get { return m_Geom; }
            set { m_Geom = value; }
        }

        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (!IsTrimPoint())
                m_Geom.Render(display, style);
        }

        /// <summary>
        /// Does this point occur at the end of a trimmed dangling line?
        /// </summary>
        bool IsTrimPoint()
        {
            // The point has to be marked as trimmed
            if (!IsTrimmed)
                return false;

            // There has to be exactly one incident boundary
            IDivider[] divs = IncidentDividers();
            if (divs.Length!=1)
                return false;

            // The divider must be invisible
            return !divs[0].IsVisible;
        }

        public override IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

        public override ILength Distance(IPosition point)
        {
            return m_Geom.Distance(point);
        }

        public bool IsCoincident(IPointGeometry p)
        {
            return m_Geom.IsCoincident(p);
        }

        public ILength Easting
        {
            get { return m_Geom.Easting; }
        }

        public ILength Northing
        {
            get { return m_Geom.Northing; }
        }

        public IDivider[] IncidentDividers()
        {
            List<IDivider> result = new List<IDivider>(4); // 4 is frequently the number in the result

            foreach (IFeatureDependent fd in Dependents)
            {
                if (fd is LineFeature)
                {
                    LineFeature line = (fd as LineFeature);
                    Topology t = line.Topology;

                    if (t!=null)
                    {
                        if (line.StartPoint.IsCoincident(this))
                        {
                            IDivider d = t.FirstDivider;
                            if (!d.IsOverlap)
                                result.Add(d);
                        }
                        else if (line.EndPoint.IsCoincident(this))
                        {
                            IDivider d = t.LastDivider;
                            if (!d.IsOverlap)
                                result.Add(d);
                        }
                        else
                        {
                            // Cover a situation where this point is referenced to a line that
                            // passes through (intersection).
                            Debug.Assert(t is SectionTopologyList);
                            SectionTopologyList sections = (t as SectionTopologyList);
                            sections.AddIncidentDividers(result, this);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Arbitarily changes the position of this point. For use only in a last resort.
        /// <b>This method does not update the spatial index</b>. 
        /// </summary>
        /// <param name="newPosition">The new position</param>
        /// <exception cref="InvalidOperationException">If the point is referenced by any
        /// other objects.</exception>
        /*
        internal void ChangePosition(PointGeometry newPosition)
        {
            if (HasDependents)
                throw new InvalidOperationException("Cannot move point that has dependents");

            PointFeature[] pts = m_Geom.Points;
            m_Geom = new Node(pts, newPosition);
        }
        */

        /// <summary>
        /// Returns formatted position of this point.
        /// </summary>
        /// <returns>Formatted position of this point.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", DataId, m_Geom.ToString());
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            // Just output position as attributes (yes, I know geometry could theoretically
            // contain things like an "M" value, but I'd rather have a straightfoward XML schema).
            // This isn't really significant here, but it matters in the ReadContent method.

            if (m_Geom.FirstPoint == this)
            {
                if (m_Geom.PointCount > 1)
                    writer.WriteUnsignedInt("PointCount", m_Geom.PointCount);
            }
            else
                writer.WriteFeatureReference("FirstPoint", m_Geom.FirstPoint);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            if (m_Geom.FirstPoint == this)
            {
                // The Node class is NOT expected to override the PointGeometry implementation
                //writer.WriteElement(
                m_Geom.WriteAttributes(writer);
            }
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);

            // Since we didn't write out the geometry as an element, we need to cover
            // the types of geometry that are supported.
            if (reader.HasAttribute("Z"))
            {
                // Just a matter of defining a PointGeometry3D class, but I'm too lazy
                // to do it today.
                throw new NotSupportedException("Points will elevation are not currently supported");
            }
            else
            {
                // If this point shares geometry with a preceding point, grab the geometry from there
                if (reader.HasAttribute("FirstPoint"))
                {
                    PointFeature p = reader.ReadFeatureByReference<PointFeature>("FirstPoint");
                    m_Geom = p.m_Geom;
                    m_Geom.AttachPoint(this);
                }
                else
                {
                    PointGeometry g = new PointGeometry();
                    g.ReadAttributes(reader);

                    //uint pointCount = Math.Max(1, reader.ReadUnsignedInt("PointCount"));
                    m_Geom = new Node(this, g);
                }
            }
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);
        }
    }
}
