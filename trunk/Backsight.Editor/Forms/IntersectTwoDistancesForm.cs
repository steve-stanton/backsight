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
    /// <written by="Steve Stanton" was="CdIntersectDist" />
    /// <summary>
    /// Dialog for the Intersect - Two Distances command.
    /// </summary>
    /// <remarks>This was formerly the CdIntersectDist dialog, which was a CPropertySheet
    /// containing two CdGetDist objects and a CdIntersectTwo object.</remarks>
    partial class IntersectTwoDistancesForm : Form, IIntersectDialog
    {
        #region Class data

        /// <summary>
        /// The command displaying this dialog (either an instance of <see cref="IntersectUI"/>
        /// or <see cref="UpdateUI"/>)
        /// </summary>
        readonly CommandUI m_Cmd;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectTwoDistancesForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        internal IntersectTwoDistancesForm(CommandUI cmd, string title)
        {
            InitializeComponent();
            this.Text = title;
            m_Cmd = cmd;
        }

        #endregion
    }
}