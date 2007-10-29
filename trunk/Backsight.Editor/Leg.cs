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

using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeLeg" />
    /// <summary>
    /// A leg in a connection path. This is the base class for <see cref="StraightLeg"/>
    /// and <see cref="CircularLeg"/>.
    /// </summary>
    [Serializable]
    abstract class Leg
    {
        #region Class data

        /// <summary>
        /// The observed distances for this leg. May be empty (for cul-de-sacs
        /// that have no observed spans)
        /// </summary>
        Distance[] m_Distances;

        /// <summary>
        /// The features that correspond to each observed distance. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// Contains Max(1, m_Distances.Length) elements.
        /// </summary>
        Feature[] m_Creations;

        /// <summary>
        /// Array of switches on each span (one for each observed distance).
        /// Null if there aren't any switches.
        /// </summary>
        LegItemFlag[] m_Switches;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Leg</c> with everything set to null.
        /// </summary>
        protected Leg()
        {
        }

        protected Leg(int nspan)
        {
            // Allocate an array of feature pointers (always at least ONE).
            // And an array for switches. And distances.
            if (nspan == 0)
            {
                m_Distances = null;
                m_Creations = new Feature[1];
                m_Switches = null;
            }
            else
            {
                m_Distances = new Distance[nspan];
                m_Creations = new Feature[nspan];
                m_Switches = new LegItemFlag[nspan];
            }
        }

        #endregion

        abstract internal Circle Circle { get; }
        abstract internal ILength Length { get; }
        abstract internal IPosition Center { get; }
        abstract internal string DataString { get; }
        abstract internal void Project (ref IPosition pos, ref double bearing, double sfac);
        abstract internal bool Save (PathOperation op, ref IPosition terminal, ref double bearing, double sfac);
        abstract internal bool Rollforward (ref IPointGeometry insert, PathOperation op,
                                                ref IPosition terminal, ref double bearing, double sfac);
        abstract internal bool SaveFace (PathOperation op, ExtraLeg face);
        abstract internal bool RollforwardFace (ref IPointGeometry insert, PathOperation op, ExtraLeg face,
                                                    IPosition spos, IPosition epos);

        ushort Count
        {
            get { return (ushort)m_Distances.Length; }
        }

        bool HasEndPoint(ushort index)
        {
            if (index >= Count || (m_Switches[index] & LegItemFlag.OmitPoint) != 0)
                return false;
            else
                return true;
        }

        bool IsDeflection
        {
            get { return (m_Switches != null && (m_Switches[0] & LegItemFlag.Deflection) != 0); }
        }

        bool IsCurve
        {
            get { return (this.Circle!=null); }
        }

        bool IsStaggered
        {
            get { return (FaceNumber != 0); }
        }

        uint FaceNumber
        {
            get
            {
                if (m_Switches == null)
                    return 0;

                if ((m_Switches[0] & LegItemFlag.Face1) != 0)
                    return 1;

                if ((m_Switches[0] & LegItemFlag.Face2) != 0)
                    return 2;

                return 0;
            }
        }

        /// <summary>
        /// Sets the distance of a specific span in this leg.
        /// </summary>
        /// <param name="distance">The distance to assign.</param>
        /// <param name="index">The index of the distance [0,m_NumSpan-1]</param>
        /// <param name="qualifier"></param>
        /// <returns>True if index was valid.</returns>
        protected bool SetDistance(Distance distance, int index, LegItemFlag qualifier)
        {
            // Return if index is out of range.
            if (index<0 || index>=m_Distances.Length)
                return false;

            // Remember any qualifier.
            if (qualifier != 0)
                m_Switches[index] |= qualifier;

            // Assign the distance
            m_Distances[index] = distance;
            return true;
        }

        /// <summary>
        /// Gets the total observed length of this leg
        /// </summary>
        /// <returns>The sum of the observed lengths for this leg, in meters on the ground</returns>
        protected double GetTotal()
        {
            double total = 0.0;

            foreach(Distance d in m_Distances)
                total += d.Meters;

            return total;
        }

    }
}
