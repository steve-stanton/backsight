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
using System.Drawing;

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a line that consists of at least two sections.
    /// </summary>
    [Serializable]
    class SectionTopologyList : Topology
    {
        #region Class data

        /// <summary>
        /// The topological sections for a line, ordered along the line such that
        /// the tail terminal of each section is the same as the head terminal of
        /// the next section. In normal situations, each divider will be an instance of
        /// <see cref="SectionDivider"/>. However, in situations where topological lines
        /// overlap, a divider may actually be an instance of <see cref="SectionOverlap"/>
        /// <para/>
        /// Each section must refer to the same <c>LineFeature</c> as the instance
        /// of <c>SectionTopologyList</c> that contains it.
        /// </summary>
        readonly List<IDivider> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SectionTopologyList</c> that relates to the specified line,
        /// but with an empty list of sections.
        /// </summary>
        /// <param name="line">The line that any topological sections must relate to.</param>
        internal SectionTopologyList(LineFeature line)
            : base(line)
        {
            m_Sections = new List<IDivider>();
        }

        /// <summary>
        /// Creates a new <c>SectionTopologyList</c> that relates to the specified line,
        /// and with the specified list of sections.
        /// </summary>
        /// <param name="line">The line that any topological sections relate to.</param>
        /// <param name="sections">The topological sections (must all refer to <paramref name="line"/>)</param>
        /// <exception cref="ArgumentException">If any of the specified sections do not
        /// relate to <paramref name="line"/>
        /// </exception>
        internal SectionTopologyList(LineFeature line, List<IDivider> sections)
            : base(line)
        {
            foreach (IDivider d in sections)
            {
                if (d.Line != line)
                    throw new ArgumentException("Topological section refers to inconsistent line");
            }

            m_Sections = sections;
        }

        #endregion

        /// <summary>
        /// The divider at the start of the associated line
        /// (assuming that the section list is complete)
        /// Null if no dividers have been added to the list.
        /// </summary>
        internal override IDivider FirstDivider
        {
            get { return (m_Sections.Count==0 ? null : m_Sections[0]); }
        }

        /// <summary>
        /// The divider at the end of the associated line
        /// (assuming that the section list is complete).
        /// Null if no dividers have been added to the list.
        /// </summary>
        internal override IDivider LastDivider
        {
            get
            {
                int nSection = m_Sections.Count;
                return (nSection==0 ? null : m_Sections[nSection-1]);
            }
        }

        /// <summary>
        /// Returns an enumerator that identifies each divider in this topology.
        /// </summary>
        /// <returns>The instances of <see cref="SectionTopology"/> that are stored
        /// in the list</returns>
        public override IEnumerator<IDivider> GetEnumerator()
        {
            return m_Sections.GetEnumerator();
        }

        public override string  ToString()
        {
            return "(more than one boundary section)";
        }

        /// <summary>
        /// Replaces topology associated with this instance. This gets called when the
        /// topology is getting cut up at intersections.
        /// </summary>
        /// <param name="oldDivider">The divider that's being replaced</param>
        /// <param name="newDividers">The dividers to insert in place of <paramref name="oldDivider"/></param>
        /// <exception cref="ArgumentException">If <paramref name="oldDivider"/> is not
        /// part of this list</exception>
        internal void ReplaceDivider(IDivider oldDivider, List<IDivider> newDividers)
        {
            int index = m_Sections.IndexOf(oldDivider);
            if (index<0)
                throw new ArgumentException("SectionTopologyList.ReplaceDivider - Cannot locate topological section");

            m_Sections.RemoveAt(index);
            m_Sections.InsertRange(index, newDividers);
        }

        /// <summary>
        /// Performs any processing when the line associated with this topology
        /// is being de-activated.  This should mark adjacent polygons for deletion, and
        /// remove line references from any intersections.
        /// </summary>
        internal override void OnLineDeactivation()
        {
            // By default, we don't cleanup the starting terminal of each section. Unless
            // the preceding section was an overlap (since intersections aren't cross-referenced
            // to overlaps).
            bool doFrom = false;

            foreach (SectionTopology s in m_Sections)
            {
                if (s.IsOverlap)
                {
                    doFrom = true;
                }
                else
                {
                    Topology.MarkPolygons(s);

                    // The starting terminal only needs to be cleaned up if the preceding
                    // section was an overlap.
                    if (doFrom)
                    {
                        OnLineDeactivation(s.From);
                        doFrom = false;
                    }

                    OnLineDeactivation(s.To);
                }                
            }
        }

        /// <summary>
        /// Performs any cleanup of a terminal on deactivation of a line. This will only
        /// do stuff for terminals that are line intersections.
        /// </summary>
        /// <param name="t">The terminal to process</param>
        void OnLineDeactivation(ITerminal t)
        {
            if (t is Intersection)
            {
                Intersection x = (t as Intersection);
                x.OnLineDeactivation(this.Line);
            }
        }

        /// <summary>
        /// Merges divider sections that are incident on the specified intersection. This gets
        /// called when one of the lines causing an intersection is being removed (deactivated).
        /// </summary>
        /// <param name="x">An intersection that is being removed</param>
        internal override void MergeSections(Intersection x)
        {
            for (int i=0; i<(m_Sections.Count-1); i++)
            {
                IDivider a = m_Sections[i];
                if (a.To == x)
                {
                    IDivider b = m_Sections[i+1];

                    // If either divider is an overlap, it would be BAD to merge
                    if (a is SectionDivider && b is SectionDivider)
                    {
                        m_Sections.RemoveAt(i+1);
                        m_Sections[i] = new SectionDivider(Line, a.From, b.To);
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Locates the divider that is closest to the specified position.
        /// </summary>
        /// <param name="p">The position of interest</param>
        /// <returns>The closest section (null if there are no sections in this list)</returns>
        internal IDivider FindClosestSection(IPosition p)
        {
            if (m_Sections.Count==0)
                return null;

            IDivider result = m_Sections[0];
            double minDist = result.LineGeometry.Distance(p).Meters;

            for (int i=1; i<m_Sections.Count; i++)
            {
                double d = m_Sections[i].LineGeometry.Distance(p).Meters;
                if (d < minDist)
                {
                    minDist = d;
                    result = m_Sections[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the sections in this list can be trimmed using the
        /// <see cref="TrimLineOperation"/>
        /// </summary>
        /// <returns>True if either end dangles, and we would be left with something
        /// after the trim.</returns>
        internal bool CanTrim()
        {
            // Can't trim if we don't have at least two sections
            if (m_Sections.Count<2)
                return false;

            // Get the sections at the ends.
            IDivider s = this.FirstDivider;
            IDivider e = this.LastDivider;
            Debug.Assert(s!=e);

            // Return if neither end is dangling
            bool sDangle = Topology.IsDangle(s, s.From);
            bool eDangle = Topology.IsDangle(e, e.To);
            if (!(sDangle || eDangle))
                return false;

            // Can't process if both ends are dangling and the common point is coincident
            if (sDangle && eDangle && s.To.IsCoincident(e.From))
                return false;

            return true;
        }

        /// <summary>
        /// Draws sections on the specified display, excluding dangling ends.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal void RenderTrimmed(ISpatialDisplay display, IDrawStyle style)
        {
            // If we're highlighting, dangling portions should be drawn dotted
            IDrawStyle dangleStyle = (style is HighlightStyle ? new DottedStyle(Color.Red) : null);

            bool sDangle = Line.IsStartDangle();
            bool eDangle = Line.IsEndDangle();

            int last = m_Sections.Count-1;
            for (int i=0; i<=last; i++)
            {
                if ((i==0 && !sDangle) || (i>0 && i<last) || (i==last && !eDangle))
                {
                    IDivider d = m_Sections[i];
                    d.LineGeometry.Render(display, style);
                }
                else if (dangleStyle!=null && ((i==0 && sDangle) || (i==last && eDangle)))
                {
                    IDivider d = m_Sections[i];
                    d.LineGeometry.Render(display, dangleStyle);
                }
            }
        }

        /// <summary>
        /// Will a divider associated with this topology be drawn by the <see cref="RenderTrimmed"/>
        /// method?
        /// </summary>
        /// <param name="d">One of the dividers in this list</param>
        /// <returns>True if the divider coincides with a dangling portion of the line.</returns>
        internal bool IsVisible(IDivider d)
        {
            if (Object.ReferenceEquals(d, FirstDivider))
                return !Line.IsStartDangle();

            if (Object.ReferenceEquals(d, LastDivider))
                return !Line.IsEndDangle();

            return true;
        }
    }
}
