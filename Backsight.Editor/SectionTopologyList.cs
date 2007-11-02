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
        /// the next section.
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
    }
}
