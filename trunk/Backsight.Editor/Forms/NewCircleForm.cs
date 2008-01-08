/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
    /// Dialog for the <see cref="NewCircleUI"/>
    /// </summary>
    partial class NewCircleForm : Form
    {
        readonly NewCircleUI m_Cmd;

        internal NewCircleForm(NewCircleUI cmd)
        {
            InitializeComponent();

            m_Cmd = cmd;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            m_Cmd.DialAbort(this);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //m_Cmd.DialFinish(this);
        }
    }
}