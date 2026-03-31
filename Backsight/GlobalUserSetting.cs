// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

using System.Diagnostics;
using System.Text.Json;

namespace Backsight;

/// <written by="Steve Stanton" on="30-MAR-2007" />
/// <summary>
/// A user-specific setting that should be accessible across applications.
/// </summary>
public static class GlobalUserSetting
{
    private static string? s_SettingsPath = null;
    private static Dictionary<string, string>? s_Settings = null;

    private static Dictionary<string, string> GetSettings()
    {
        if (s_Settings is not null)
            return s_Settings;

        // Ensure the Backsight folder is present
        if (s_SettingsPath is null)
        {
            var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            var folder = Path.Combine(appData, "Backsight");
            Directory.CreateDirectory(folder);
            s_SettingsPath = Path.Combine(folder, "Settings.json");
        }

        if (File.Exists(s_SettingsPath))
        {
            var json = File.ReadAllText(s_SettingsPath);
            s_Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        else
        {
            s_Settings = new Dictionary<string, string>();
            File.WriteAllText(s_SettingsPath, JsonSerializer.Serialize(s_Settings));
        }
        
        if (s_Settings is null)
            throw new ApplicationException("Failed to load settings");
        
        return s_Settings;
    }

    /// <summary>
    /// Saves a global setting
    /// </summary>
    /// <param name="settingName">The name of the setting</param>
    /// <param name="val">The value to save</param>
    public static void Write(string settingName, string val)
    {
        var settings = GetSettings();
        Debug.Assert(s_SettingsPath is not null);
        
        settings[settingName] = val;
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(s_SettingsPath, JsonSerializer.Serialize(settings, options));
    }

    /// <summary>
    /// Retrieves a global setting
    /// </summary>
    /// <param name="settingName">The name of the setting</param>
    /// <returns>The saved value (blank if the setting has never been written)</returns>
    public static string Read(string settingName)
    {
        var settings = GetSettings();
        return settings.GetValueOrDefault(settingName, String.Empty);
    }

    /// <summary>
    /// Retrieves a global setting and converts to integer.
    /// </summary>
    /// <param name="settingName">The name of the setting</param>
    /// <param name="defaultValue">The default value for the setting</param>
    /// <returns>The parsed integer (equals <param name="defaultValue"/> if it's not
    /// defined, or it couldn't be parsed)</returns>
    public static int ReadInt(string settingName, int defaultValue)
    {
        string s = Read(settingName);
        return s.Length == 0 ? defaultValue : Int32.Parse(s);
    }

    /// <summary>
    /// Saves a global setting that corresponds to an int value.
    /// </summary>
    /// <param name="settingName">The name of the setting</param>
    /// <param name="val">The value to save</param>
    public static void WriteInt(string settingName, int val)
    {
        Write(settingName, val.ToString());
    }
}