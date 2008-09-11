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

using Microsoft.Win32;

namespace Backsight.Data
{
    /// <summary>
    /// The connection string for the last database accessed by the <c>Cadastral Editor</c>
    /// or the <c>Environment Editor</c>.
    /// </summary>
    public static class LastDatabase
    {
        /// <summary>
        /// The key name of the registry entry that holds database-wide settings.
        /// </summary>
        static string REGISTRY_BASEKEY = Registry.LocalMachine + @"\Software\Backsight";

        /// <summary>
        /// The connection string last used to access the database (blank if a database
        /// has never been accessed)
        /// </summary>
        public static string ConnectionString
        {
            get { return Read("LastDatabase"); }
            set { Write("LastDatabase", value); }
        }

        /// <summary>
        /// Saves a global setting
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="val">The value to save</param>
        static void Write(string settingName, string val)
        {
            Registry.SetValue(REGISTRY_BASEKEY, settingName, val);
        }

        /// <summary>
        /// Retrieves a global setting
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <returns>The saved value (blank if the setting has never been written)</returns>
        static string Read(string settingName)
        {
            object o = Registry.GetValue(REGISTRY_BASEKEY, settingName, String.Empty);
            return (o==null ? String.Empty : o.ToString());
        }
    }
}
