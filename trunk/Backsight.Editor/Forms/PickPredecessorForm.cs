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
using System.Windows.Forms;
using System.Collections.Generic;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="21-APR-2009" was="CdOpList" />
    /// <summary>
    /// Dialog for selecting a specific line for update (from a list of
    /// predecessor lines).
    /// </summary>
    /// <seealso cref="PickEditForm"/>
    partial class PickPredecessorForm : Form
    {
        #region Class data

        /// <summary>
        /// The lines to select from
        /// </summary>
        readonly LineFeature[] m_Lines;

        /// <summary>
        /// The selected line
        /// </summary>
        LineFeature m_Selection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PickPredecessorForm"/> class.
        /// </summary>
        /// <param name="lines">The predecessors to display</param>
        /// <param name="canSelect">Can the user select a specific line?</param>
        internal PickPredecessorForm(LineFeature[] lines, bool canSelect)
        {
            InitializeComponent();
            okButton.Visible = canSelect;
            m_Lines = lines;
        }

        #endregion
    }
}