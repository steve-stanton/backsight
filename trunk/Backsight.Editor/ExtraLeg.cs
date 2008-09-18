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
    /// <written by="Steve Stanton" on="27-MAR-2000" was="CeExtraLeg" />
    /// <summary>
    /// An extra leg in a connection path. This sort of leg is used to handle
    /// staggered property lots.
    /// </summary>
    class ExtraLeg : Leg
    {
        #region Class data

        /// <summary>
        /// The editing operation that contains this leg.
        /// </summary>
        PathOperation m_Parent;

        /// <summary>
        /// The leg that this extra leg coincides with.
        /// </summary>
        Leg m_Base;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ExtraLeg</c>
        /// <param name="parent">The editing operation that contains this leg.</param>
        /// <param name="baseLeg">The leg this one is based on.</param>
        /// <param name="dists">The observed distances for this leg.</param>
        /// </summary>
        internal ExtraLeg(PathOperation parent, Leg baseLeg, Distance[] dists)
            : base(dists.Length)
        {
            m_Parent = parent;
            m_Base = baseLeg;
            
            // Hold the distance observations in the base class.
            for (int i=0; i<dists.Length; i++)
                SetDistance(dists[i], i, LegItemFlag.Null);
        }

        #endregion

        /// <summary>
        /// The circle (if any) that this leg falls on.
        /// </summary>
        public override Circle Circle // ILeg
        {
            get { return m_Base.Circle; }
        }

        /// <summary>
        /// The total observed length of this leg, on the ground.
        /// </summary>
        public override ILength Length // ILeg
        {
            get
            {
                double m = GetTotal();
                return new Length(m);
            }
        }

        /// <summary>
        /// The position at the center of the circle that this leg lies on.
        /// </summary>
        internal override IPosition Center
        {
            get { return m_Base.Center; }
        }

        /// <summary>
        /// Defines a string with the observations that make up this leg. This
        /// implementation just returns an empty string! This function is used when
        /// recalling a connection path. By returning an empty string, it means you
        /// don't actually recall the extra face.
        /// </summary>
        internal override string GetDataString(DistanceUnit defaultEntryUnit)
        {
            return String.Empty;
        }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances (default=1.0).</param>
        public override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            // Do nothing. Projecting to the end is done when the
            // base leg is processed. Extra legs are based on those
            // results.
        }

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        public override void Render(ISpatialDisplay display, ref IPosition pos, ref double bearing, double sfac)
        {
            // Do nothing.
        }

        /// <summary>
        /// Draws a previously saved leg.
        /// </summary>
        /// <param name="preview">True if the path should be drawn in preview
        /// mode (i.e. in the normal construction colour, with miss-connects
        /// shown as dotted lines).</param>
        internal override void Draw(bool preview)
        {
            // Do nothing.
        }

        /// <summary>
        /// Saves features for this leg.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="createdPoints">Newly created point features</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        internal override void Save(PathOperation op, List<PointFeature> createdPoints,
                                    ref IPosition terminal, ref double bearing, double sfac)
        {
        }

        /// <summary>
        /// Rollforward this leg.
        /// </summary>
        /// <param name="insert">The point of the end of any new insert that immediately
        /// precedes this leg. This will be updated if this leg also ends with a new insert
        /// (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="terminal">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <returns></returns>
        internal override bool Rollforward(ref PointFeature insert, PathOperation op,
                                            ref IPosition terminal, ref double bearing, double sfac)
        {
            return false;
        }

        /// <summary>
        /// Rollforward this leg (special version).
        /// </summary>
        /// <param name="insert">The location of the end of any new insert that immediately
        /// precedes this leg. This will be updated if this leg also ends with a new insert
        /// (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="spos">The position for the start of the leg.</param>
        /// <param name="epos">The position for the end of the leg.</param>
        /// <returns></returns>
        internal bool Rollforward(ref IPointGeometry insert, PathOperation op,
                                    IPosition spos, IPosition epos)
        {
            return m_Base.RollforwardFace(ref insert, op, this, spos, epos);
        }

        /// <summary>
        /// Saves features for a second face that is based on this leg.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="face">The extra face to create features for.</param>
        /// <returns>True if created ok.</returns>
        internal override bool SaveFace(PathOperation op, ExtraLeg face)
        {
            // Do nothing. Extra legs can't have yet another 2nd face.
            return false;
        }

        /// <summary>
        /// Rollforward the second face of this leg.
        /// </summary>
        /// <param name="insert">The location of the end of any new insert that immediately
        /// precedes this leg. This will be updated if this leg also ends with a new insert
        /// (if not, it will be returned as a null value).</param>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <param name="face">The second face.</param>
        /// <param name="spos">The new position for the start of this leg.</param>
        /// <param name="epos">The new position for the end of this leg.</param>
        /// <returns>Trueif rolled forward ok.</returns>
        /// <remarks>
        /// The start and end positions passed in should correspond to where THIS leg
        /// currently ends. They are passed in because this leg may contain miss-connects
        /// (and maybe even missing end points). So it would be tricky trying trying to
        /// work it out now.
        /// </remarks>
        internal override bool RollforwardFace(ref IPointGeometry insert, PathOperation op,
                                                ExtraLeg face, IPosition spos, IPosition epos)
        {
            // Do nothing. Extra legs can't have yet another 2nd face.
            return false;
        }

        /// <summary>
        /// Creates features for this leg.
        /// </summary>
        /// <param name="op">The connection path that this leg belongs to.</param>
        /// <returns>True if created ok.</returns>
        internal bool MakeFeatures(PathOperation op)
        {
            // Turn it over to the base leg.
            return m_Base.SaveFace(op, this);
        }

        /// <summary>
        /// Writes the attributes for this leg.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        protected override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteString("Base", m_Parent.DataId);
            writer.WriteInt("Leg", m_Parent.GetLegIndex(m_Base));
        }

        /// <summary>
        /// Reads the attributes for this leg.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        protected override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);
            string baseId = reader.ReadString("Base");
            int legIndex = reader.ReadInt("Leg");

            // Locate the original edit in the model, then find the leg
            // within that edit. Leave it for now, since I'm not 100% that
            // extra legs will even be handled via this class.

            throw new NotImplementedException("ExtraLeg.ReadAttributes");
        }
    }
}
