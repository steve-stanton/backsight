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
using System.Windows.Forms;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog for listing the edits in a session.
    /// </summary>
    public partial class SessionForm : Form
    {
        #region Class data

        /// <summary>
        /// The data for the operations grid
        /// </summary>
        readonly BindingSource m_Binding;

        /// <summary>
        /// The session of interest
        /// </summary>
        readonly Session m_Session;

        /// <summary>
        /// The features currently drawn
        /// </summary>
        Feature[] m_Draw;

        #endregion

        #region Constructors

        internal SessionForm(Session s)
        {
            if (s==null)
                throw new ArgumentNullException();

            InitializeComponent();
            m_Session = s;
            m_Binding = new BindingSource();
            m_Draw = new Feature[0];
        }

        #endregion

        private void SessionForm_Shown(object sender, EventArgs e)
        {
            // Load the list of operations that were performed in the
            // session (in reverse order).
            Operation[] ops = m_Session.GetOperations(true);
            m_Binding.DataSource = ops;
            grid.AutoGenerateColumns = false;
            grid.DataSource = m_Binding;

            /*
	while ( pos ) {

		CeOperation* pop = (CeOperation*)ops.GetPrev(pos);

		features.Remove();
		const UINT4 nFeat = pop->GetFeatures(features);

		const CeObjectList* const pMore = pop->GetMore();
		const UINT4 nMore = (pMore ? pMore->GetCount() : 0);

		str.Format("[%d] %s (%d+%d)",pop->GetSequence(),
									 pop->GetpTitle(),
									 nFeat,nMore);

		// If it's an import, append the file spec.
		CString extra;
		switch ( pop->GetType() ) {

		case CEOP_DATA_IMPORT: {

			CeImport* pImport = dynamic_cast<CeImport*>(pop);
			extra.Format(" (%s)",pImport->GetFile());
			break;
		}

		case CEOP_GET_BACKGROUND: {
			CeGetBackground* pImport = dynamic_cast<CeGetBackground*>(pop);
			extra.Format(" (%s)",pImport->GetFile());
			break;
		}

		} // end switch
             */
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            // If we previously drew something, ensure the screen is back to normal
            if (m_Draw.Length>0)
            {
                EditingController.Current.RefreshAllDisplays();
                m_Draw = new Feature[0];
            }

            // Get the features (if any) created by the newly selected operation.
            Operation op = GetSelectedOperation();
            if (op!=null)
                m_Draw = op.Features;
        }

        Operation GetSelectedOperation()
        {
            DataGridViewSelectedRowCollection sel = grid.SelectedRows;
            if (sel.Count==0)
                return null;

            DataGridViewRow row = sel[0];
            return (m_Binding[row.Index] as Operation);
        }

        private void SessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_Draw.Length>0)
                EditingController.Current.RefreshAllDisplays();
        }

        private void detailsButton_Click(object sender, EventArgs e)
        {
            Operation op = GetSelectedOperation();
            if (op==null)
                MessageBox.Show("You must first select an edit");
            else
                ShowDetails(op);
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Operation op = GetSelectedOperation();
            if (op!=null)
                ShowDetails(op);
        }

        void ShowDetails(Operation op)
        {
            EditDetailsForm dial = new EditDetailsForm(op);
            dial.ShowDialog();
            dial.Dispose();
        }
    }
}