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

        PointGeometry m_Geom;

        #endregion

        #region Constructors

        public static PointFeature Create(IPosition p, IEntity e, Operation creator)
        {
            if (p==null)
                throw new ArgumentNullException("Position for new point feature cannot be null");

            if (e==null)
                throw new ArgumentNullException("Entity type for new point feature cannot be null");

            IPointGeometry g = (p as IPointGeometry);
            if (g==null)
                g = new PointGeometry(p);

            return new PointFeature(g, e, creator);
        }

        /// <summary>
        /// Creates a new <c>PointFeature</c>
        /// </summary>
        /// <param name="g">The geometry for the point (not null)</param>
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        public PointFeature(IPointGeometry g, IEntity e, Operation creator)
            : base(e, creator)
        {
            m_Geom = (PointGeometry)g;
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
        /// <param name="radius">The radius to search for (null to
        /// search for ANY circle). A fixed tolerance of 0.001 meters on
        /// the ground is used.</param>
        /// <returns>The attached circle (null if no such circle).</returns>
        internal Circle GetCircle(ILength radius)
        {
            List<IFeatureDependent> deps = this.Dependents;
            if (deps == null)
                return null;

            foreach (IFeatureDependent d in deps)
            {
                if (d is Circle)
                {
                    Circle c = (d as Circle);
                    if (radius==null)
                        return c;

                    if (Math.Abs(c.Radius.Meters - radius.Meters) < 0.001)
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
            PointGeometry g = (to as PointGeometry);
            if (g==null)
                g = new PointGeometry(to);

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
            m_Geom = to;
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
                        else
                        {
                            Debug.Assert(line.EndPoint.IsCoincident(this));
                            IDivider d = t.LastDivider;
                            if (!d.IsOverlap)
                                result.Add(d);
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
        internal void ChangePosition(IPointGeometry newPosition)
        {
            if (HasDependents)
                throw new InvalidOperationException("Cannot move point that has dependents");

            m_Geom = PointGeometry.Create(newPosition);
        }

        /// <summary>
        /// Returns formatted position of this point.
        /// </summary>
        /// <returns>Formatted position of this point.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", DataId, m_Geom.ToString());
        }

        /// <summary>
        /// Goes through each line that is incident on this point, and mark adjacent
        /// polygons for deletion.
        /// </summary>
        public void MarkPolygons() // ITerminal
        {
            IDivider[] da = this.IncidentDividers();

            foreach (IDivider d in da)
                Topology.MarkPolygons(d);
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);
            writer.WriteElement("Geometry", m_Geom);
        }
    }
}
