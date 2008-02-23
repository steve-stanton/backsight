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
using System.Collections.Generic;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-1999" was="CeEntityFile"/>
    /// <summary>
    /// Object that holds the content of an entity file (a file that holds the
    /// definition of derived entity types).
    /// </summary>
    class EntityFile
    {
        #region Class data

        /// <summary>
        /// Translation blocks
        /// </summary>
        EntityBlock[] m_Blocks;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. After calling this function, the object must be initialized
        /// with <see cref="Create"/>.
        /// </summary>
        internal EntityFile()
        {
            m_Blocks = null;
        }

        #endregion

        /// <summary>
        /// Resets data members to their initial values.
        /// </summary>
        void ResetContents()
        {
            m_Blocks = null;
        }

        /// <summary>
        /// Initializes this entity file object with the content of a file.
        /// </summary>
        /// <param name="sr">The file to read the definition from.</param>
        /// <returns>The number of translation blocks that were loaded.</returns>
        internal int Create(StreamReader sr)
        {
            // If we previously created stuff, get rid of it.
            ResetContents();

            // While we have not reached the end of file, search for a
            // record that contains the "BEGIN" keyword. As we find each
            // one, create an EntityBlock object, and ask it to load
            // the block.

            List<EntityBlock> blocks = new List<EntityBlock>();
            string buf;

            while ((buf=sr.ReadLine())!=null)
            {
                // Trim off any leading and trailing whitespace.
                string str = buf.Trim();

                // Skip blank records.
                if (str.Length==0)
                    continue;

                // If we have the BEGIN keyword, create an additional
                // EntityBlock object and load it.
                if (String.Compare(str, "BEGIN", true)==0)
                {
                    try
                    {
                        EntityBlock block = new EntityBlock();
                        block.Create(sr);
                        blocks.Add(block);
                    }

                    catch { }
                }
            }

            // Return the number of translation blocks that we loaded.
            m_Blocks = blocks.ToArray();
            return m_Blocks.Length;
        }

        /// <summary>
        /// Tries to return the name of a derived entity type for a specific line.
        /// </summary>
        /// <param name="line">The line to process.</param>
        /// <param name="theme">The theme of interest (in the same address space as
        /// the map that contains the line).</param>
        /// <returns>The name of the derived entity type (null if not found).</returns>
        internal string GetDerivedType(IDivider line, ITheme theme)
        {
            // Return if the line does not have an entity type (it SHOULD do)
            IEntity ent = line.Line.EntityType;
            if (ent==null)
                return null;

            // Get the translation block (if any) that refers to the line's entity type.
            string entName = ent.Name;
            string theName = theme.Name;
            EntityBlock block = Array.Find<EntityBlock>(m_Blocks,
                delegate(EntityBlock eb) { return eb.IsMatch(entName, theName); });

            // Return if the line's entity type is not associated with a translation.
            if (block==null)
                return null;

            // Get the polygons on either side of the line (the REAL
            // user-perceived polygons, not phantoms).
            Polygon leftPol = line.Left.RealPolygon;
            Polygon rightPol = line.Right.RealPolygon;

            // Get the entity types for the adjacent polygons (if any).
            IEntity leftEnt = (leftPol==null ? null : leftPol.EntityType);
            IEntity rightEnt = (rightPol==null ? null : rightPol.EntityType);

            if (leftEnt!=null || rightEnt!=null)
            {
                string e1 = (leftEnt==null ? String.Empty : leftEnt.Name);
                string e2 = (rightEnt==null ? String.Empty : rightEnt.Name);

                return block.GetDerivedType(e1, e2);
            }

            return null;
        }

        /// <summary>
        /// Tries to return the name of an un-derived entity type associated with an
        /// entity type that may be derived.
        /// </summary>
        /// <param name="dervEntName">The name of a (maybe) derived entity type.</param>
        /// <returns>The un-derived entity type (null if it is not derived, as far as this
        /// entity file is concerned).</returns>
        internal string GetUnderivedType(string dervEntName)
        {
            // Go through each translation block, looking for a match.
            if (m_Blocks!=null)
            {
                foreach (EntityBlock b in m_Blocks)
                {
                    string ent = b.GetUnderivedType(dervEntName);
                    if (ent!=null)
                        return ent;
                }
            }

            // The supplied entity type is not derived.
            return null;
        }
    }
}
