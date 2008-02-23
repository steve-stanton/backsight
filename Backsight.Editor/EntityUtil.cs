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
using System.Drawing;

using Backsight.Editor.Forms;
using Backsight.Environment;
using Backsight.Data;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityUtil"/>
    /// <summary>
    /// Utility functions relating to entity types. This sort of object
    /// act as a conduit to methods provided by the <see cref="EntityFile"/> and
    /// <see cref="StyleFile"/> classes.
    /// </summary>
    class EntityUtil
    {
        #region Static

        /// <summary>
        /// Information about symbology
        /// </summary>
        static StyleFile s_StyleFile;

        /// <summary>
        /// Information about derived entity types
        /// </summary>
        static EntityFile s_EntityFile;

        /// <summary>
        /// The default black style
        /// </summary>
        static Style s_BlackStyle = new Style(Color.Black);

        /// <summary>
        /// The default black style for construction lines
        /// </summary>
        static Style s_BlackDottedLineStyle = new LineStyle(Color.Black, 0); // should be PS_DOT
	
        #endregion

        #region Class data

        // None

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EntityUtil</c>
        /// </summary>
        internal EntityUtil()
        {
        }

        #endregion

        /// <summary>
        /// Opens entity utility functions. This opens an entity translation file
        /// referred to via the Backsight property <see cref="PropertyNaming.EntityFile"/>
        /// (if there is such a definition). It then reads the file into an
        /// <see cref="EntityFile"/> object for later use.
        /// <para/>
        /// Also opens a style file referred to via the property
        /// <see cref="PropertyNaming.StyleFile"/> (if there is such a definition).
        /// It then reads the file into a <see cref="StyleFile"/> object for later use.
        /// </summary>
        /// <returns>True if an entity file and/or style file was loaded.</returns>
        /// <remarks>
        /// This function should be called near the start of the application. To release
        /// the resources, a subsequent call to <see cref="Close"/> should be made.
        /// </remarks>
        internal bool Open()
        {
            // Ensure any files previously loaded have been released.
            Close();

            s_EntityFile = OpenEntityFile();
            s_StyleFile = OpenStyleFile();

            return (s_EntityFile!=null || s_StyleFile!=null);
        }

        /// <summary>
        /// Opens an entity translation file
        /// referred to via the Backsight property <see cref="PropertyNaming.EntityFile"/>
        /// (if there is such a definition). It then reads the file into an
        /// <see cref="EntityFile"/> object for later use.
        /// </summary>
        /// <returns>The loaded entity translation file (or null if no such file, or
        /// it could not be loaded).</returns>
        EntityFile OpenEntityFile()
        {
            // Get the translation (if any) of the property
            // that refers to the entity file.
            IProperty prop = EnvironmentContainer.FindPropertyByName(PropertyNaming.EntityFile);
            string entfile = (prop == null ? null : prop.Value);
            if (String.IsNullOrEmpty(entfile))
                return null;

            // Open the entity file.
            using (StreamReader sr = File.OpenText(entfile))
            {
                // Create a new entity file object, and ask it to load the file.
                EntityFile file = new EntityFile();
                int nblock = file.Create(sr);

                // Return the file if it loaded ok
                return (nblock > 0 ? file : null);
            }
        }

        /// <summary>
        /// Also opens a style file referred to via the property
        /// <see cref="PropertyNaming.StyleFile"/> (if there is such a definition).
        /// It then reads the file into a <see cref="StyleFile"/> object for later use.
        /// </summary>
        /// <returns>The loaded style file (or null if no such file, or it could
        /// not be loaded). If the environment variable is undefined, you get back
        /// an object that represents an empty style file.</returns>
        StyleFile OpenStyleFile()
        {
            // Get the translation (if any) of the property
            // that refers to the standard style file.
            IProperty prop = EnvironmentContainer.FindPropertyByName(PropertyNaming.StyleFile);
            string stdfile = (prop == null ? null : prop.Value);
            if (String.IsNullOrEmpty(stdfile))
                return new StyleFile();

            // Create a new style file object, and ask it to load the file.
            StyleFile file = new StyleFile();
            int nStyle = file.Create(stdfile);

            // Return the file if it loaded ok
            return (nStyle > 0 ? file : null);
        }

        /// <summary>
        /// Releases any resources that were allocated via a prior
        /// call to <see cref="Open"/>
        /// </summary>
        internal void Close()
        {
            s_EntityFile = null;
            s_StyleFile = null;
        }

        /// <summary>
        /// Returns the name of a derived entity type associated with a line.
        /// If an entity file has been loaded, this may be based on the entity
        /// types of the adjacent polygons. Failing that, the result will be
        /// the same as the name of the line's "normal" entity type.
        /// </summary>
        /// <param name="line">The line to process.</param>
        /// <param name="theme">The theme of interest.</param>
        /// <returns>The name of the derived entity type (may be blank)</returns>
        internal static string GetDerivedType(IDivider line, ITheme theme)
        {
            // If we have an entity file, and it can return a derived type,
            // that's us done.
            if (s_EntityFile != null)
            {
                string dervEntName = s_EntityFile.GetDerivedType(line, theme);
                if (dervEntName != null)
                    return dervEntName;
            }

            // Get the line's entity type (if any).
            IEntity lineEnt = line.Line.EntityType;
            return (lineEnt == null ? String.Empty : lineEnt.Name);
        }

        /// <summary>
        /// Returns the name of the un-derived entity type associated with a derived
        /// entity type. If the supplied entity type is not derived, you get back
        /// the name of what you supplied.
        /// </summary>
        /// <param name="dervEnt">The (maybe) derived entity type.</param>
        /// <returns>The name of the un-derived entity type (may be null, but
        /// only if you supply a null type).</returns>
        internal static string GetUnderivedType(IEntity dervEnt)
        {
            // Return if supplied with a null entity type.
            if (dervEnt == null)
                return null;

            string dervEntName = dervEnt.Name;

            // If we have an entity file, and it can return the
            // un-derived type, that's us done.
            if (s_EntityFile != null)
            {
                string ent = s_EntityFile.GetUnderivedType(dervEntName);
                if (ent != null)
                    return ent;
            }

            // Just return the name of the supplied entity type as the
            // un-derived type.
            return dervEntName;
        }

        /// <summary>
        /// Returns the color for the specified entity type
        /// </summary>
        /// <param name="ent">The entity type.</param>
        /// <returns>The corresponding color</returns>
        internal static Color GetColor(IEntity ent)
        {
            return GetStyle(ent).Color;
        }

        /// <summary>
        /// Returns the style for the specified entity type
        /// </summary>
        /// <param name="ent">The entity type (may be null)</param>
        /// <returns>The corresponding style (never null)</returns>
        internal static Style GetStyle(IEntity ent)
        {
            if (s_StyleFile == null || ent == null)
                return s_BlackStyle;

            Style style = s_StyleFile.GetStyle(ent.Name);
            if (style != null)
                return style;

            return s_BlackStyle;
        }

        /// <summary>
        /// Returns the style for the specified line
        /// </summary>
        /// <param name="line">The feature to get the style for</param>
        /// <param name="theme">The editing theme</param>
        /// <returns>The corresponding style (never null)</returns>
        internal static Style GetStyle(IDivider line, ITheme theme)
        {
            if (s_StyleFile == null)
                return s_BlackStyle;

            // Get the name of the (possibly derived) entity type
            string entName = GetDerivedType(line, theme);

            // Nothing at all means we should have a construction
            // line, which is always symbolized as a black dotted line.
            if (entName.Length == 0)
                return s_BlackDottedLineStyle;

            // Try to get a style using the name of the derived
            // entity type.
            Style style = s_StyleFile.GetStyle(entName);
            if (style != null)
                return style;

            // If we didn't get anything, look for a style that
            // refers to the line's base theme.
            ILayer t = line.Line.BaseLayer;
            if (t != null)
            {
                style = s_StyleFile.GetStyle(t.Name);
                if (style != null)
                    return style;
            }

            // Return the default style
            return s_BlackStyle;
        }

        /// <summary>
        /// Returns the style for the specified feature
        /// </summary>
        /// <param name="f">The feature to get the style for</param>
        /// <returns>The corresponding style (never null)</returns>
        internal static Style GetStyle(Feature f)
        {
            if (s_StyleFile == null)
                return s_BlackStyle;

            // Get the name of the entity type
            IEntity ent = f.EntityType;
            if (ent == null)
                return s_BlackStyle;

            // Try to get a style using the name of the derived
            // entity type.
            Style style = s_StyleFile.GetStyle(ent.Name);
            if (style != null)
                return style;

            // If we didn't get anything, look for a style that
            // refers to the line's base theme.
            ILayer t = f.BaseLayer;
            if (t != null)
            {
                style = s_StyleFile.GetStyle(t.Name);
                if (style != null)
                    return style;
            }

            // Return the default style
            return s_BlackStyle;
        }

        /// <summary>
        /// Is a customized style file being used?
        /// </summary>
        /// <returns></returns>
        /*
        internal static bool IsStyleCustomized()
        {
            if (s_StyleFile == null)
                return false;

            string spec = s_StyleFile.Spec;
            if (String.IsNullOrEmpty(spec))
                return false;

            IProperty prop = EnvironmentContainer.FindPropertyByName(PropertyNaming.StyleFile);
            string stdSpec = (prop == null ? null : prop.Value);
            if (stdSpec == null)
                stdSpec = String.Empty;

            return (spec != stdSpec);
        }
         */
    }
}
