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
using Microsoft.Win32;

namespace Backsight
{
	/// <written by="Steve Stanton" on="30-MAR-2007" />
    /// <summary>
    /// A user-specific setting that should be accessible across applications.
    /// </summary>
    /// <remarks>
    /// For the sake of simplicity, this just makes use of the Windows registry.
    /// Another more .net-centric implementation might involve System.IO.IsolatedStorage
    /// (it looks interesting, but I got deterred when I started reading about things
    /// like signed assemblies, app domains, and miscellaneous other crap).
    /// </remarks>
    public static class GlobalUserSetting
    {
        /// <summary>
        /// The last database connection string (blank if never defined)
        /// </summary>
        public static string LastConnection
        {
            get { return Read("LastConnection"); }
            set { Write("LastConnection", value); }
        }

        /// <summary>
        /// The registry key for user-specific settings.
        /// </summary>
        static string s_UserRoot = @"HKEY_CURRENT_USER\Software\Backsight";

        /// <summary>
        /// Saves a global setting
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="val">The value to save</param>
        public static void Write(string settingName, string val)
        {
            Registry.SetValue(s_UserRoot, settingName, val);
        }

        /// <summary>
        /// Retrieves a global setting
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <returns>The saved value (blank if the setting has never been written)</returns>
        public static string Read(string settingName)
        {
            object o = Registry.GetValue(s_UserRoot, settingName, String.Empty);
            return (o==null ? String.Empty : o.ToString());
        }
    }
}
