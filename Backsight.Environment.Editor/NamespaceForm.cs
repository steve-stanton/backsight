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

namespace Backsight.Environment.Editor
{
    /// <summary>
    /// Dialog for working with Backsight "namespaces" (named areas of space)
    /// </summary>
    public partial class NamespaceForm : Form
    {
        private readonly IEditNamespace m_Edit;

        #region Constructors

        internal NamespaceForm()
            : this(null)
        {
        }

        internal NamespaceForm(IEditNamespace edit)
        {
            InitializeComponent();

            m_Edit = edit;
            if (m_Edit == null)
            {
                IEnvironmentFactory f = EnvironmentContainer.Factory;
                m_Edit = f.CreateNamespace();
            }

            m_Edit.BeginEdit();
        }

        #endregion

    }
}