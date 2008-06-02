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
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="26-MAR-2008"/>
    /// <summary>
    /// Information about a connection path. This is a transient class that
    /// acts as a helper for the <see cref="PathForm"/> dialog. It's sort of a
    /// half-way between the fairly unstructured world of the dialog class, and the
    /// regimented world of the operation class.
    /// </summary>
    class PathData
    {
        #region Class data

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        readonly PointFeature m_From;

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        readonly PointFeature m_To;

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        readonly List<ILeg> m_Legs;

        /// <summary>
        /// Has the <see cref="Adjust"/> method been successfully called. If true, the values
        /// for <see cref="m_Rotation"/> and <see cref="m_ScaleFactor"/> are meaningful.
        /// </summary>
        bool m_IsAdjusted;

        /// <summary>
        /// Rotation for path (in radians)
        /// </summary>
        double m_Rotation;

        /// <summary>
        /// Scaling to apply to path distances
        /// </summary>
        double m_ScaleFactor;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PathData</c> object
        /// </summary>
        /// <param name="from">The point where the path starts.</param>
        /// <param name="to">The point where the path ends.</param>
        internal PathData(PointFeature from, PointFeature to)
        {
            m_From = from;
            m_To = to;
            m_Legs = new List<ILeg>();

            m_IsAdjusted = false;
            m_Rotation = 0.0;
            m_ScaleFactor = 0.0;
        }

        /// <summary>
        /// Create a new <c>PathData</c> object that corresponds to a previously
        /// saved connection path. For consistency with the other constructor, this
        /// does not attempt to adjust the path (the Rotation and ScaleFactory properties
        /// will retain zero values unless a call is made to Adjust).
        /// </summary>
        /// <param name="pop">The saved connection path</param>
        internal PathData(PathOperation pop)
        {
            m_From = pop.StartPoint;
            m_To = pop.EndPoint;

            Leg[] legs = pop.GetLegs();
            m_Legs = new List<ILeg>(legs);

            m_IsAdjusted = false;
            m_Rotation = 0.0;
            m_ScaleFactor = 0.0;
        }

        #endregion

        /// <summary>
        /// The point where the path starts.
        /// </summary>
        internal PointFeature FromPoint
        {
            get { return m_From; }
        }

        /// <summary>
        /// The point where the path ends.
        /// </summary>
        internal PointFeature ToPoint
        {
            get { return m_To; }
        }

        /// <summary>
        /// The legs that make up the path
        /// </summary>
        internal ILeg[] GetLegs()
        {
            return m_Legs.ToArray();
        }

        /// <summary>
        /// Draws the path on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        internal void Render(ISpatialDisplay display)
        {
            if (!EnsureAdjusted())
                return;

            // Do nothing if the scale factor is undefined.
            if (Math.Abs(m_ScaleFactor) < MathConstants.TINY)
                return;

            // Initialize position to the start of the path.
            IPosition gotend = new Position(m_From);

            // Initial bearing is whatever the rotation is.
            double bearing = m_Rotation;

            // Get each leg to draw itself
            foreach (ILeg leg in m_Legs)
                leg.Render(display, ref gotend, ref bearing, m_ScaleFactor);

            // Re-draw the terminal points to ensure that their color is on top.
            DrawEnds(display);
        }

        /// <summary>
        /// Draws the end points for this path.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        void DrawEnds(ISpatialDisplay display)
        {
            if (m_From!=null)
                m_From.Draw(display, Color.DarkBlue);

            if (m_To!=null)
                m_To.Draw(display, Color.LightBlue);
        }

        /// <summary>
        /// Ensures the Adjust method has been called.
        /// </summary>
        /// <returns>True if path data has been successfully adjusted. False if a
        /// call to <c>Adjust</c> failed.</returns>
        bool EnsureAdjusted()
        {
            if (m_IsAdjusted)
                return true;

            double de;				// Misclosure in eastings
            double dn;				// Misclosure in northings
            double prec;			// Precision
            double length;			// Total observed length
            double rotation;		// Rotation for adjustment
            double sfac;			// Adjustment scaling factor

            Adjust(out dn, out de, out prec, out length, out rotation, out sfac);
            return m_IsAdjusted;
        }

        /// <summary>
        /// Creates path data using a collection of path items.
        /// </summary>
        /// <param name="items">The items that define the path.</param>
        internal void Create(PathItem[] items)
        {
            // Count the number of legs.
            int numLeg = CountLegs(items);
            if (numLeg==0)
                throw new Exception("PathData.Create -- No connection legs");

            m_Legs.Capacity = numLeg;

            // Create each leg.

            int legnum=0;       // Current leg number
            int nexti=0;        // Index of the start of the next leg

            for (int si=0; si<items.Length; si=nexti)
            {
                // Skip if no leg number (could be new units spec).
                if (items[si].LegNumber==0)
                {
                    nexti = si+1;
                    continue;
                }

                // Confirm the leg count is valid.
                if (legnum+1>numLeg)
                    throw new Exception("PathData.Create -- Bad number of path legs.");

                // Create the leg.
                ILeg newLeg;
                if (items[si].ItemType == PathItemType.BC)
                    newLeg = CreateCircularLeg(items, si, out nexti);
                else
                    newLeg = CreateStraightLeg(items, si, out nexti);

                // Exit if we failed to create the leg.
                if (newLeg==null)
                    throw new Exception("PathData.Create -- Unable to create leg");

                m_Legs.Add(newLeg);
            }

            // Confirm we created the number of legs we expected.
            if (numLeg!=m_Legs.Count)
                throw new Exception("PathData.Create -- Unexpected number of legs");
        }

        /// <summary>
        /// Counts the number of legs for this path.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <returns>The number of legs.</returns>
        int CountLegs(PathItem[] items)
        {
            // Each path item contains a leg number, arranged sequentially.
            int nleg=0;

            foreach (PathItem item in items)
                nleg = Math.Max(nleg, item.LegNumber);

            return nleg;
        }

        /// <summary>
        /// Creates a circular leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        ILeg CreateCircularLeg(PathItem[] items, int si, out int nexti)
        {
            // Confirm that the first item refers to the BC.
            if (items[si].ItemType != PathItemType.BC)
                throw new Exception("PathOperation.CreateCircularLeg - Not starting at BC");

            // The BC has to be followed by at least 3 items: angle, radius
            // and EC (add an extra 1 to account for 0-based indexing).
            if (items.Length < si+4)
                throw new Exception("PathOperation.CreateCircularLeg - Insufficient curve data");

            double bangle = 0.0;		// Angle at BC
            double cangle = 0.0;		// Central angle
            double eangle = 0.0;		// Angle at EC
            bool twoangles = false;	    // True if bangle & eangle are both defined.
            bool clockwise = true;		// True if curve is clockwise
            int irad = 0;				// Index of the radius item
            bool cul = false;			// True if cul-de-sac case

            // Point to item following the BC.
            nexti = si+1;
            PathItemType type = items[nexti].ItemType;

            // If the angle following the BC is a central angle
            if (type==PathItemType.CentralAngle)
            {
                // We have a cul-de-sac
                cul = true;

                // Get the central angle.
                cangle = items[nexti].Value;
                nexti++;
            }
            else if (type==PathItemType.BcAngle)
            {
                // Get the entry angle.
                bangle = items[nexti].Value;
                nexti++;

                // Does an exit angle follow?
                if (items[nexti].ItemType == PathItemType.EcAngle)
                {
                    eangle = items[nexti].Value;
                    twoangles = true;
                    nexti++;
                }
            }
            else
            {
                // The field after the BC HAS to be an angle.
                throw new Exception("Angle does not follow BC");
            }

            // Must be followed by radius.
            if (items[nexti].ItemType != PathItemType.Radius)
                throw new Exception("Radius does not follow angle");

            // Get the radius
            Distance radius = items[nexti].GetDistance();
            irad = nexti;
            nexti++;

            // The item after the radius indicates whether the curve is counterclockwise.
            if (items[nexti].ItemType == PathItemType.CounterClockwise)
            {
                nexti++;
                clockwise = false;
            }

            // Get the leg ID.
            int legnum = items[si].LegNumber;

            // How many distances have we got?
            int ndist = 0;
            for (; nexti<items.Length && items[nexti].LegNumber==legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            CircularLeg leg = new CircularLeg(radius, clockwise, ndist);

            // Set the entry angle or the central angle, depending on what we have.
            if (cul)
                leg.SetCentralAngle(cangle);
            else
                leg.SetEntryAngle(bangle);

            // Assign second angle if we have one.
            if (twoangles)
                leg.SetExitAngle(eangle);

            // Assign each distance, starting one after the radius.
            ndist = 0;
            for (int i = irad + 1; i < nexti; i++)
            {
                Distance dist = items[i].GetDistance();
                if (dist != null)
                {
                    // See if there is a qualifier after the distance
                    LegItemFlag qual = LegItemFlag.Null;
                    if (i + 1 < nexti)
                    {
                        PathItemType nexttype = items[i + 1].ItemType;
                        if (nexttype == PathItemType.MissConnect)
                            qual = LegItemFlag.MissConnect;
                        if (nexttype == PathItemType.OmitPoint)
                            qual = LegItemFlag.OmitPoint;
                    }

                    leg.SetDistance(dist, ndist, qual);
                    ndist++;
                }
            }

            // Return the new leg.
            return leg;
        }

        /// <summary>
        /// Creates a straight leg.
        /// </summary>
        /// <param name="items">Array of path items.</param>
        /// <param name="si">Index to the item where the leg data starts.</param>
        /// <param name="nexti">Index of the item where the next leg starts.</param>
        /// <returns>The new leg.</returns>
        ILeg CreateStraightLeg(PathItem[] items, int si, out int nexti)
        {
            // Get the leg ID.
            int legnum = items[si].LegNumber;

            // How many distances have we got?
            int ndist = 0;
            for (nexti=si; nexti<items.Length && items[nexti].LegNumber==legnum; nexti++)
            {
                if (items[nexti].IsDistance)
                    ndist++;
            }

            // Create the leg.
            StraightLeg leg = new StraightLeg(ndist);

            // Assign each distance.
            ndist = 0;
            for (int i=si; i<nexti; i++)
            {
                Distance d = items[i].GetDistance();
                if (d!=null)
                {
                    // See if there is a qualifier after the distance
                    LegItemFlag qual = LegItemFlag.Null;
                    if ((i+1) < nexti)
                    {
                        PathItemType nexttype = items[i+1].ItemType;

                        if (nexttype==PathItemType.MissConnect)
                            qual = LegItemFlag.MissConnect;

                        if (nexttype==PathItemType.OmitPoint)
                            qual = LegItemFlag.OmitPoint;
                    }

                    leg.SetDistance(d, ndist, qual);
                    ndist++;
                }

            }

            // If the first item is an angle, remember it as part of the leg.
            if (items[si].ItemType == PathItemType.Angle)
                leg.StartAngle = items[si].Value;
            else if (items[si].ItemType == PathItemType.Deflection)
                leg.SetDeflection(items[si].Value);

            // Return a reference to the new leg
            return leg;
        }

        /// <summary>
        /// Adjusts the path (Helmert adjustment).
        /// </summary>
        /// <param name="dN">Misclosure in northing.</param>
        /// <param name="dE">Misclosure in easting.</param>
        /// <param name="precision">Precision denominator (zero if no adjustment needed).</param>
        /// <param name="length">Total observed length.</param>
        /// <param name="rotation">The clockwise rotation to apply (in radians).</param>
        /// <param name="sfac">The scaling factor to apply.</param>
        /// <returns>True if adjusted ok.</returns>
        internal bool Adjust(out double dN, out double dE, out double precision, out double length,
                    out double rotation, out double sfac)
        {
            dN = dE = precision = length = rotation = 0.0;
            sfac = 1.0;

            // Initialize position to the start of the path, corresponding to the initial
            // un-adjusted end point.
            IPosition start = m_From;
            IPosition gotend = new Position(m_From);

            // Initial bearing is due north.
            double bearing = 0.0;

            // Go through each leg, updating the end position, and getting
            // the total path length.
            foreach (ILeg leg in m_Legs)
            {
                length += leg.Length.Meters;
                leg.Project(ref gotend, ref bearing, sfac);
            }

            // Get the bearing and distance of the end point we ended up with.
            double gotbear = Geom.BearingInRadians(m_From, gotend);
            double gotdist = Geom.Distance(m_From, gotend);

            // Get the bearing and distance we want.
            double wantbear = Geom.BearingInRadians(m_From, m_To);
            double wantdist = Geom.Distance(m_From, m_To);

            // Figure out the rotation.
            rotation = wantbear-gotbear;

            // Rotate the end point we got.
            gotend = Geom.Rotate(m_From, gotend, new RadianValue(rotation));

            // Calculate the line scale factor.
            double linefac = m_From.MapModel.CoordinateSystem.GetLineScaleFactor(m_From, gotend);

            // Figure out where the rotated end point ends up when we apply the line scale factor.
            gotend = Geom.Polar(m_From, wantbear, gotdist*linefac);

            // What misclosure do we have?
            dN = gotend.Y - m_To.Y;
            dE = gotend.X - m_To.X;
            double delta = Math.Sqrt(dN*dN + dE*dE);

            // What's the precision denominator (use a value of 0 to denote an exact match).
            if (delta > MathConstants.TINY)
                precision = wantdist/delta;
            else
                precision = 0.0;

            // Figure out the scale factor for the adjustment (use a value of 0 if the start and end
            // points are coincident). The distances here have NOT been adjusted for the line scale factor.
            if (gotdist > MathConstants.TINY)
                sfac = wantdist/gotdist;
            else
                sfac = 0.0;

            // Remember the rotation and scaling factor
            m_IsAdjusted = true;
            m_Rotation = rotation;
            m_ScaleFactor = sfac;

            return true;
        }
    }
}
