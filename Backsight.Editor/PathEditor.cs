// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;

using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    class PathEditor
    {
        #region Class data

        /// <summary>
        /// Working copy of the legs in the connection path.
        /// </summary>
        readonly List<Leg> m_Legs;

        /// <summary>
        /// The changes that have been made to the path.
        /// </summary>
        readonly List<LegEdit> m_Changes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PathEditor"/> class.
        /// </summary>
        /// <param name="legs">A working copy of the legs that may be modified.</param>
        internal PathEditor(Leg[] legs)
        {
            m_Legs = new List<Leg>(legs);
            m_Changes = new List<LegEdit>();
        }

        #endregion

        #region Private classes

        private class LegEdit
        {
            /// <summary>
            /// The 0-based array index of the leg that was changed.
            /// </summary>
            /// <remarks>
            /// When you are dealing with a collection of edits, the leg and span index values recorded
            /// for any one edit are dependent on the preceding edits. Suppose for example that you start
            /// out with just 1 leg, and perform 3 edits: update the distance of the first span, insert
            /// a new span at the start of the leg, and then re-update the distance of the span that
            /// was previously at the start of the leg.
            /// <para/>
            /// When you perform the first update, the edit will refer to span 0. The second edit
            /// inserts a new span with array index 0, but the change already recorded remains the
            /// same. When you perform the third edit, the span index will be 1.
            /// </remarks>
            readonly uint m_LegIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="LegEdit"/> class.
            /// </summary>
            /// <param name="legIndex">The 0-based array index of the leg that was changed (may refer to a leg
            /// that was inserted earlier in a sequence of related edits).</param>
            protected LegEdit(uint legIndex)
            {
                m_LegIndex = legIndex;
            }

            internal virtual void Apply(List<Leg> legs)
            {
                throw new NotImplementedException();
            }

            protected uint LegIndex
            {
                get { return m_LegIndex; }
            }
        }

        /// <summary>
        /// Update to the distance associated with a span.
        /// </summary>
        private class SpanUpdate : LegEdit
        {
            /// <summary>
            /// The 0-based array index of the span within a leg that was changed (may refer to a span
            /// that was inserted earlier in a sequence of related edits).
            /// </summary>
            readonly uint m_SpanIndex;

            /// <summary>
            /// The revised observation.
            /// </summary>
            Distance m_Value;

            internal SpanUpdate(uint legIndex, uint spanIndex, Distance value)
                : base(legIndex)
            {
                m_SpanIndex = spanIndex;
                m_Value = value;
            }

            internal override void Apply(List<Leg> legs)
            {
                //Leg leg = legs[(int)LegIndex];
                //SpanInfo spanInfo = leg.GetSpanData((int)m_SpanIndex);
                //spanInfo.ObservedDistance = m_Value;
            }
        }

        /// <summary>
        /// A newly inserted span
        /// </summary>
        private class SpanInsert : SpanUpdate
        {
            SpanInsert(uint legIndex, uint spanIndex, Distance value)
                : base(legIndex, spanIndex, value)
            {
            }
        }

        /// <summary>
        /// Change to the angle at the start of a straight leg.
        /// </summary>
        private class StraightLegUpdate : LegEdit
        {
            /// <summary>
            /// Angle at the start of the leg (signed). 
            /// </summary>
            double m_StartAngle;

            /// <summary>
            /// Is the angle at the start of this a deflection?
            /// </summary>
            bool m_IsDeflection;

            internal StraightLegUpdate(uint legIndex, double startAngle, bool isDeflection)
                : base(legIndex)
            {
                m_StartAngle = startAngle;
                m_IsDeflection = isDeflection;
            }
        }

        private class LegBreak : LegEdit
        {
            /// <summary>
            /// The number of spans that should be retained in the portion of the leg that precedes
            /// the break (must be a value greater than zero).
            /// </summary>
            readonly uint m_NumSpanBeforeBreak;

            internal LegBreak(uint legIndex, uint numSpanBeforeBreak)
                : base(legIndex)
            {
                m_NumSpanBeforeBreak = numSpanBeforeBreak;
            }
        }

        /// <summary>
        /// Change to the parameters describing a circular arc.
        /// </summary>
        private class CircularLegUpdate : LegEdit
        {
            /// <summary>
            /// Observations defining the shape of the circular leg.
            /// </summary>
            readonly CircularLegMetrics m_Metrics;

            internal CircularLegUpdate(uint legIndex, CircularLegMetrics metrics)
                : base(legIndex)
            {
                m_Metrics = metrics;
            }
        }

        /// <summary>
        /// Flip annotations for all spans on a leg.
        /// </summary>
        private class LegFlipAnnotations : LegEdit
        {
            internal LegFlipAnnotations(uint legIndex)
                : base(legIndex)
            {
            }
        }

        /// <summary>
        /// Definition of a new face (staggered leg)
        /// </summary>
        private class NewFace : LegEdit
        {
            // The leg index refers to the leg that the new face is coincident with (the new face
            // will be inserted after that).

            NewFace(uint legIndex)
                : base(legIndex)
            {
            }
        }

        #endregion

        internal void UpdateSpan(int legIndex, int spanIndex, Distance value)
        {
            var edit = new SpanUpdate((uint)legIndex, (uint)spanIndex, value);
            edit.Apply(m_Legs);
            m_Changes.Add(edit);            
        }

        /// <summary>
        /// Inserts an extra distance into the path.
        /// </summary>
        /// <param name="newdist">The new distance to insert.</param>
        /// <param name="curdist">A distance that this leg already knows about.</param>
        /// <param name="isBefore">Should the new distance go before the existing one?</param>
        internal void InsertSpan(int legIndex, Distance newdist, Distance curdist, bool isBefore)
        {
            /*
            Leg leg = m_Legs[legIndex];
            int spanIndex = leg.GetIndex(curdist);
            var edit = new SpanInsert((uint)legIndex, 
             */
        }

        internal void SetStartAngle(int legIndex, double startAngle, bool isDeflection)
        {
            var edit = new StraightLegUpdate((uint)legIndex, startAngle, isDeflection);
            edit.Apply(m_Legs);
            m_Changes.Add(edit);
        }
        
        internal void BreakLeg(int legIndex, int numSpanBeforeBreak)
        {
            var edit = new LegBreak((uint)legIndex, (uint)numSpanBeforeBreak);
            edit.Apply(m_Legs);
            m_Changes.Add(edit);
        }

        internal void SetArcMetrics(int legIndex, CircularLegMetrics metrics)
        {
            var edit = new CircularLegUpdate((uint)legIndex, metrics);
            edit.Apply(m_Legs);
            m_Changes.Add(edit);
        }

        internal void FlipLegAnnotations(int legIndex)
        {
            var edit = new LegFlipAnnotations((uint)legIndex);
            edit.Apply(m_Legs);
            m_Changes.Add(edit);
        }
    }
}
