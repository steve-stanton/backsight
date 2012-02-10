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
using System.Drawing;

using Backsight.Editor.Operations;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="06-MAY-2009" was="CdOpList" />
    /// <summary>
    /// Dialog for listing a series of edits (used to list dependencies from
    /// <see cref="UpdateForm"/>).
    /// </summary>
    /// <seealso cref="PickPredecessorForm"/>
    partial class ListOperationsForm : Form
    {
        #region Class data

        /// <summary>
        /// The edits to display
        /// </summary>
        readonly Operation[] m_Edits;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListOperationsForm"/> class.
        /// </summary>
        /// <param name="edits">The edits to display</param>
        internal ListOperationsForm(Operation[] edits)
        {
            InitializeComponent();
            m_Edits = edits;
        }

        #endregion

        private void ListOperationsForm_Shown(object sender, EventArgs e)
        {
            grid.RowCount = m_Edits.Length;

            for (int i=0; i<m_Edits.Length; i++)
            {
                Operation op = m_Edits[i];
                DataGridViewRow row = grid.Rows[i];

                row.Tag = op;
                row.Cells["opIdColumn"].Value = op.InternalId;
                row.Cells["operationColumn"].Value = op.Name;
                row.Cells["createdColumn"].Value = op.Session.StartTime.ToShortDateString();
                row.Cells["editorColumn"].Value = op.Session.User;

                // If the line was created by a connection path, display the precision.
                if (op is PathOperation)
                {
                    PathOperation path = (PathOperation)op;
                    row.Cells["precisionColumn"].Value = path.GetPrecision();
                }
            }

            grid.CurrentCell = null;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}