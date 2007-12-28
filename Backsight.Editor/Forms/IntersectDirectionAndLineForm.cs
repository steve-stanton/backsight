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
using System.Drawing;

using Backsight.Editor.Operations;
using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" was="CdIntersectDirLine" />
    /// <summary>
    /// Dialog for the Intersect - Direction and Line command.
    /// </summary>
    /// <remarks>This was formerly the CdIntersectDirLine dialog, which was a CPropertySheet
    /// containing a CdGetDir, CdGetLine, and a CdIntersect object.</remarks>
    internal partial class IntersectDirectionAndLineForm : IntersectForm
    {
        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectDirectionAndLineForm</c>
        /// </summary>
        /// <param name="cmd">The command displaying this dialog (not null)</param>
        /// <param name="title">The string to display in the form's title bar</param>
        public IntersectDirectionAndLineForm(CommandUI cmd, string title)
            : base(cmd, title)
        {
            InitializeComponent();
        }

        #endregion
    }
}

