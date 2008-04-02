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

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Dialog that lets the user specify an item of miscellaneous text.
    /// </summary>
    partial class NewTextForm : Form
    {
        #region Class data

        #endregion

        #region Constructors

        internal NewTextForm()
        {
            InitializeComponent();
        }

        #endregion

        internal string GetText()
        {
            return null;
        }

        internal IEntity EntityType
        {
            get { return null; }
        }
    }
}