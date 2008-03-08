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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="21-JAN-1998" was="CeCircularLeg" />
    /// <summary>
    /// A circular leg in a connection path.
    /// </summary>
    [Serializable]
    class CircularLeg : Leg
    {
        #region Class data

        /// <summary>
        /// First angle. Either at the BC, or a central angle. In radians. It's
        /// a central angle if the FLG_CULDESAC is set.
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
        /// The circle that this leg sits on.
        /// </summary>
        Circle m_Circle;

        /// <summary>
        /// Flag bits
        /// </summary>
        CircularLegFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        CircularLeg()
            : base(0)
        {
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;
            m_Radius = null;
            m_Circle = null;
            m_Flag = 0;
        }

        /// <summary>
        /// Creates a new <c>CircularLeg</c> with no spans.
        /// </summary>
        /// <param name="radius">The radius of the circular leg.</param>
        /// <param name="clockwise">True if the curve is clockwise.</param>
        /// <param name="span">The number of spans on the curve.</param>
        CircularLeg(Distance radius, bool clockwise, int nspan)
            : base(nspan)
        {
            // Angles were not specified.
            m_Angle1 = 0.0;
            m_Angle2 = 0.0;

            // Start out with undefined flag.
            m_Flag = 0;

            // Remember the radius.
            m_Radius = radius;

            // The circle for this leg won't be known till we create a span.
            m_Circle = null;

            // Remember if its NOT a clockwise curve.
            if (!clockwise)
                m_Flag |= CircularLegFlag.CounterClockwise;
        }

        #endregion

        /*
        private:
            virtual void		SaveSpan			( CeLocation*& pInsert
                                                    , const CePath& path
                                                    , CeCircularSpan& span
                                                    , const UINT2 index );
            virtual void		SetCuldesac			( const LOGICAL isculdesac );
         */

        /// <summary>
        /// The circle that this leg sits on.
        /// </summary>
        internal override Circle Circle
        {
            get { return m_Circle; }
        }

        /// <summary>
        /// Is the leg directed clockwise?
        /// </summary>
        internal bool IsClockwise
        {
            get { return (m_Flag & CircularLegFlag.CounterClockwise)==0; }
        }

        /// <summary>
        /// The observed radius, in meters
        /// </summary>
        internal double Radius
        {
            get { return (m_Radius==null ? 0.0 : m_Radius.Meters); }
        }

        /// <summary>
        /// Observed radius.
        /// </summary>
        internal Distance ObservedRadius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// Is the leg flagged as a cul-de-sac?
        /// </summary>
        internal bool IsCulDeSac
        {
            get { return (m_Flag & CircularLegFlag.CulDeSac)!=0; }
        }

        /// <summary>
        /// The total length of this leg, in meters on the ground.
        /// </summary>
        internal override ILength Length
        {
            get
            {
                // If we have a cul-de-sac, we can determine the length using
                // just the central angle & the radius. Otherwise ask the base
                // class to return the total observed length.
                if (IsCulDeSac)
                {
                    double radius = m_Radius.Meters;
                    return new Length((MathConstants.PIMUL2 - m_Angle1) * radius);
                }
                else
                    return new Length(base.GetTotal());
            }
        }

        /// <summary>
        /// Returns the position at the center of the circle that this leg lies on.
        /// </summary>
        internal override IPosition Center 
        {
            get
            {
                if (m_Circle!=null)
                    return m_Circle.Center;
                else
                    return null;
            }
        }

        internal override string DataString
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override void Project(ref IPosition pos, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Draw(ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Draw(bool preview)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Save(Backsight.Editor.Operations.PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool Rollforward(ref IPointGeometry insert, Backsight.Editor.Operations.PathOperation op, ref IPosition terminal, ref double bearing, double sfac)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool SaveFace(Backsight.Editor.Operations.PathOperation op, ExtraLeg face)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override bool RollforwardFace(ref IPointGeometry insert, Backsight.Editor.Operations.PathOperation op, ExtraLeg face, IPosition spos, IPosition epos)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
