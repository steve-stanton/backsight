// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// Changes arising while the a <see cref="CadastralMapModel"/> is being updated (handling
    /// revised information represented by the <see cref="UpdateOperation"/> class).
    /// </summary>
    /// <seealso cref="StartupEditingContext"/>
    class UpdateEditingContext : EditingContext
    {
        #region Class data

        /// <summary>
        /// Changes made to the position of point features. The key is the ID of the feature.
        /// The value could conceivably be null.
        /// </summary>
        readonly Dictionary<PointFeature, PointGeometry> m_Changes;

        /// <summary>
        /// Are changes being reverted?
        /// </summary>
        bool m_IsReverting;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEditingContext"/> class.
        /// </summary>
        internal UpdateEditingContext()
        {
            m_Changes = new Dictionary<PointFeature, PointGeometry>();
            m_IsReverting = false;
        }

        #endregion

        /// <summary>
        /// Remembers a modification to the position of a point.
        /// </summary>
        /// <param name="point">The point that is about to be changed</param>
        internal override void RegisterChange(PointFeature point)
        {
            if (!m_IsReverting)
            {
                if (!m_Changes.ContainsKey(point))
                    m_Changes.Add(point, point.PointGeometry);
            }
        }

        /// <summary>
        /// Reverts all changes recorded as part of this editing context.
        /// </summary>
        internal void RevertChanges()
        {
            if (m_IsReverting)
                throw new InvalidOperationException("Changes are already being undone");

            try
            {
                m_IsReverting = true;

                foreach (KeyValuePair<PointFeature, PointGeometry> kvp in m_Changes)
                {
                    kvp.Key.ApplyPointGeometry(this, kvp.Value);
                }
            }

            finally
            {
                m_IsReverting = false;
            }
        }
    }
}
