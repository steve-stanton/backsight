// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Operations;

namespace Backsight.Editor.Xml
{
    public partial class LineSubdivisionType : ISerializableEdit
    {
        //public LineSubdivisionType()
        //{
        //}

        ///// <summary>
        ///// Initializes a new instance of the <see cref="LineSubdivisionType"/> class.
        ///// </summary>
        ///// <param name="op">The editing operation that needs to be serialized.</param>
        //internal LineSubdivisionType(LineSubdivisionOperation op)
        //    : base(op)
        //{
        //    this.Line = op.Parent.DataId;

        //    MeasuredLineFeature[] sections = op.Sections;
        //    this.Span = new SpanType[sections.Length];

        //    for (int i=0; i<sections.Length; i++)
        //    {
        //        MeasuredLineFeature mf = sections[i];
        //        SpanType st = new SpanType();
        //        st.Length = new DistanceType(mf.ObservedLength);
        //        st.LineId = mf.Line.DataId;

        //        if (i<(sections.Length-1))
        //            st.EndPoint = new CalculatedFeatureType(mf.Line.EndPoint);

        //        this.Span[i] = st;
        //    }            
        //}

        /// <summary>
        /// Loads this editing operation into a session
        /// </summary>
        /// <param name="s">The session the editing operation should be appended to</param>
        /// <returns>The editing operation that was loaded</returns>
        internal override Operation LoadOperation(Session s)
        {
            return new LineSubdivisionOperation(s, this);
        }
    }
}
