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

using Backsight.Editor.Operations;


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
        /// The operation holding the changes that are being propagated (not null).
        /// </summary>
        readonly UpdateOperation m_Update;

        /// <summary>
        /// The edits that have been processed via a call to <see cref="Recalculate"/>.
        /// </summary>
        readonly List<Operation> m_RecalculatedEdits;

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
        /// <param name="uop">The operation holding the changes that are being propagated (not null).</param>
        internal UpdateEditingContext(UpdateOperation uop)
        {
            m_Update = uop;
            m_RecalculatedEdits = new List<Operation>();
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
        /// Recalculates the geometry for an edit.
        /// </summary>
        /// <param name="op">The edit to recalculate</param>
        internal void Recalculate(Operation op)
        {
            m_RecalculatedEdits.Add(op);

            // Remove from spatial index
            op.RemoveFromIndex();

            // Re-calculate the geometry for created features
            op.CalculateGeometry(this);
            op.ToCalculate = false;

            // Re-index
            op.AddToIndex();

            // Mark any lines as moved. In the case of lines that previously intersected
            // anything, remove any topological construct, and replace afresh
            Feature[] fa = op.Features;
            foreach (Feature f in fa)
            {
                LineFeature line = (f as LineFeature);
                if (line != null && line.IsTopological)
                {
                    line.SwitchTopology(); // turn off

                    // Turn back on (but avoid possible problem with MarkPolygons)
                    //line.SwitchTopology();
                    line.SetTopology(true);
                    line.IsMoved = true;
                }
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

                foreach (Operation op in m_RecalculatedEdits)
                    op.RemoveFromIndex();

                foreach (KeyValuePair<PointFeature, PointGeometry> kvp in m_Changes)
                    kvp.Key.ApplyPointGeometry(this, kvp.Value);

                foreach (Operation op in m_RecalculatedEdits)
                    op.AddToIndex();
            }

            finally
            {
                m_IsReverting = false;
            }
        }

        /// <summary>
        /// The operation holding the changes that are being propagated (not null).
        /// </summary>
        internal UpdateOperation UpdateSource
        {
            get { return m_Update; }
        }
    }
}
