// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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

using Backsight.Editor.Database;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog used to publish recent edits. This doesn't do that much - it's main purpose
    /// is to explain what publishing is about.
    /// </summary>
    public partial class PublishForm : Form
    {
        public PublishForm()
        {
            InitializeComponent();
        }

        private void PublishForm_Shown(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private void ShowInfo()
        {
            // Display the last revision number
            EditingController ec = EditingController.Current;
            Job job = ec.Job;
            User user = ec.User;
            uint lastRev = UserJobData.GetLastRevision(job, user);
            lastRevisionLabel.Text = lastRev.ToString();

            // Display the number of edits that haven't been published. Enable the
            // Publish button if it's non-zero (it's disabled by default).
            CadastralMapModel cmm = CadastralMapModel.Current;
            uint numEdit = cmm.GetUnpublishedEditCount();
            numEditLabel.Text = numEdit.ToString();
            publishButton.Enabled = (numEdit>0);
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            // Save changes
            EditingController.Current.Publish();

            // Refresh info on screen
            ShowInfo();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}