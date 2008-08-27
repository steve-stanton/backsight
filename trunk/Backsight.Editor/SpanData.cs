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
    /// <written by="Steve Stanton" on="25-MAR-2008"/>
    /// <summary>
    /// Information for defining a single span in a connection path. A span
    /// is part of a <see cref="Leg"/>.
    /// </summary>
    class SpanData
    {
        #region Class data

        /// <summary>
        /// The observed distance for the span. May be null when dealing
        /// with a cul-de-sac that was specified with center point and central angle).
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// The feature created for the span. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// </summary>
        Feature m_Feature;

        /// <summary>
        /// Flag bits relating to the span.
        /// </summary>
        LegItemFlag m_Switches;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>SpanData</c> with everything set to null.
        /// </summary>
        internal SpanData()
        {
            m_Distance = null;
            m_Feature = null;
            m_Switches = LegItemFlag.Null;
        }

        #endregion

        /// <summary>
        /// The observed distances for the span. May be null when dealing
        /// with a cul-de-sac that was specified with center point and central angle).
        /// </summary>
        internal Distance ObservedDistance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }

        /// <summary>
        /// The feature created for the span. This can
        /// be a <c>LineFeature</c>, <c>PointFeature</c>, or <c>null</c>.
        /// </summary>
        internal Feature CreatedFeature
        {
            get { return m_Feature; }
            set { m_Feature = value; }
        }

        /// <summary>
        /// Flag bits relating to the span.
        /// </summary>
        internal LegItemFlag Flags
        {
            get { return m_Switches; }
            set { m_Switches = value; }
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(LegItemFlag flag, bool setting)
        {
            if (setting)
                m_Switches |= flag;
            else
                m_Switches &= (~flag);
        }

        /// <summary>
        /// Does this span involve the creation of a line feature?
        /// </summary>
        internal bool HasLine
        {
            get
            {
                if ((m_Switches & LegItemFlag.MissConnect)!=0 || IsOmitPoint)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Does this span involve the creation of a terminating point feature?
        /// </summary>
        internal bool HasEndPoint
        {
            get { return !IsOmitPoint; }
        }

        /// <summary>
        /// Is this span marked as a miss-connect (meaning that a line should not be
        /// added to the map).
        /// </summary>
        internal bool IsMissConnect
        {
            get { return (m_Switches & LegItemFlag.MissConnect)!=0; }
            set { SetFlag(LegItemFlag.MissConnect, value); }
        }

        /// <summary>
        /// Should the end point for this span be omitted (determined via
        /// the relevant flag bit in the <see cref="Flags"/> property).
        /// </summary>
        internal bool IsOmitPoint
        {
            get { return (m_Switches & LegItemFlag.OmitPoint)!=0; }
            set { SetFlag(LegItemFlag.OmitPoint, value); }
        }

        /*
        /// <summary>
        /// Does this span appear at the start of a deflection (determined via
        /// the relevant flag bit in the <see cref="Flags"/> property).
        /// This can be true ONLY for the first span in a straight leg.
        /// </summary>
        internal bool IsDeflection
        {
            get { return (m_Switches & LegItemFlag.Deflection)!=0; }
            set { SetFlag(LegItemFlag.Deflection, value); }
        }
        */

        /*
        /// <summary>
        /// The leg this span is part of is staggered, and this span represents the first face. 
        /// This switch will be set ONLY for the first span in a leg.
        /// </summary>
        internal bool IsFace1
        {
            get { return (m_Switches & LegItemFlag.Face1)!=0; }
            set { SetFlag(LegItemFlag.Face1, value); }
        }
        */

        /*
        /// <summary>
        /// The leg this span is part of is staggered, and this span represents the second face. 
        /// This switch will be set ONLY for the first span in a leg.
        /// </summary>
        internal bool IsFace2
        {
            get { return (m_Switches & LegItemFlag.Face2)!=0; }
            set { SetFlag(LegItemFlag.Face2, value); }
        }
        */

        /// <summary>
        /// Is this span regarded as a miss-connect that should be replaced with a
        /// new line upon rollforward?
        /// </summary>
        internal bool IsNewSpan
        {
            get { return ((m_Switches & LegItemFlag.NewLine)!=0); }
            set { SetFlag(LegItemFlag.NewLine, value); }
        }
    }
}
