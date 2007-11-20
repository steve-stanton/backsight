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
//using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="18-NOV-1999" was="CeTextCheck" />
    /// <summary>
    /// The result of a check on an item of text.
    /// </summary>
    class TextCheck : CheckItem
    {
        #region Static

        /// <summary>
        /// Performs checks for a text label.
        /// </summary>
        /// <param name="label">The label to check.</param>
        /// <returns>The problem(s) that were found.</returns>
        internal static CheckType CheckLabel(TextFeature label)
        {
            CheckType types = CheckType.Null;
            Polygon p = label.Container;

            if (p==null)
                types |= CheckType.NoPolygonForLabel;
            else
            {
                // Does the polygon point back? If not, we've got a multi-label.
                if (p.LabelCount>1 && p.Label!=label)
                    types |= CheckType.MultiLabel;
            }

            // Does the label have at least one row of attribute data?
            FeatureId fid = label.Id;
            if (fid==null || fid.RowCount==0)
                types |= CheckType.NoAttributes;

            return types;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The text the check relates to (never null).
        /// </summary>
        readonly TextFeature m_Label;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>TextCheck</c> that relates to the specified text.
        /// </summary>
        /// <param name="label">The text the check relates to (not null).</param>
        /// <param name="types">The type(s) of check this item corresponds to</param>
        /// <exception cref="ArgumentNullException">If <paramref name="label"/> is null</exception>
        internal TextCheck(TextFeature label, CheckType types)
            : base(types)
        {
            if (label==null)
                throw new ArgumentNullException();

            m_Label = label;
        }

        #endregion

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void PaintOut(ISpatialDisplay display, CheckType newTypes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override CheckType ReCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override string Explanation
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override IPosition Position
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        internal override void Select()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
