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
using System.IO;

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="24-APR-2007" />
    /// <summary>
    /// The name for a Backsight map model
    /// </summary>
    class ModelFileName
    {
        #region Class data

        /// <summary>
        /// The name for this model (including the full path)
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Was the name for this model system-generated?
        /// </summary>
        private bool m_IsTempName;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty file with a system-generated file name
        /// </summary>
        internal ModelFileName()
        {
            m_Name = Path.GetTempFileName();
            m_IsTempName = true;
        }

        /// <summary>
        /// Creates a model name that refers to an existing file.
        /// </summary>
        /// <param name="name">The name of the model file (including full path)</param>
        internal ModelFileName(string name)
        {
            if (!File.Exists(name))
            {
                string msg = String.Format("Model file '{0}' does not exist", name);
                throw new ArgumentException(msg);
            }

            m_Name = name;
            m_IsTempName = false;
        }

        #endregion

        /// <summary>
        /// Is the model name system-generated?
        /// </summary>
        internal bool IsTempName
        {
            get { return m_IsTempName; }
        }

        /// <summary>
        /// The file name for for the model (including the path). Setting a new name
        /// will also rename any model file.
        /// </summary>
        internal string Name
        {
            get { return m_Name; }

            set
            {
                File.Move(m_Name, value);
                m_Name = value;
                m_IsTempName = false;

                Settings.Default.LastMap = m_Name;
                Settings.Default.Save();
            }
        }
    }
}
