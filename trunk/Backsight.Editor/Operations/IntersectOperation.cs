// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersect" />
    /// <summary>
    /// An intersect is a COGO operation used to generate a point where two
    /// lines intersect.
    /// </summary>
    abstract class IntersectOperation : Operation
    {
        #region Class data

        // No data

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectOperation"/> class.
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        protected IntersectOperation(Session s, uint sequence)
            : base(s, sequence)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// </summary>
        protected IntersectOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
        }

        #endregion

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        abstract internal PointFeature IntersectionPoint { get; } // was GetpIntersect

        /// <summary>
        /// Was the intersection created at it's default position?
        /// </summary>
        abstract internal bool IsDefault { get; }

        /// <summary>
        /// A point feature that is close to the intersection (for use when relocating
        /// the intersection as part of rollforward processing).
        /// </summary>
        abstract internal PointFeature ClosePoint { get; }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// This implementation always returns null.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Adds to list of features created by this operation. This appends the
        /// supplied line (if it's not null) to the results, and may also add the
        /// point at the start of the line (this covers a situation where an offset
        /// to the start of the line was specified). Note that the end point of the
        /// line is not checked, since that should correspond to the intersect point.
        /// </summary>
        /// <param name="line">A line created by this operation (may be null)</param>
        /// <param name="result">The list to append to</param>
        /// <remarks>This method may be unecessary - I suspect the UI may block an
        /// attempt to add a line when an offset is involved, need to check.</remarks>
        protected void AddCreatedFeatures(LineFeature line, List<Feature> result)
        {
            if (line!=null)
            {
                result.Add(line);

                PointFeature start = line.StartPoint;
                if (Object.ReferenceEquals(start.Creator, this))
                    result.Add(start);
            }
        }
    }
}
