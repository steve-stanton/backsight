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
    /// <written by="Steve Stanton" on="07-AUG-2007" />
    /// <summary>
    /// A translation file used to map NTX feature codes with Backsight entity types.
    /// </summary>
    class TranslationFile
    {
        #region Class data

        /// <summary>
        /// The number of physical files that have been loaded.
        /// </summary>
        int m_NumLoad;

        /// <summary>
        /// The translations read from the file.
        /// </summary>
        List<Translation<IEntity>> m_Translations;

        /// <summary>
        /// The last translation that was returned.
        /// </summary>
        Translation<IEntity> m_LastTranslation;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new translation file with nothing in it.
        /// </summary>
        internal TranslationFile()
        {
            m_Translations = new List<Translation<IEntity>>();
            m_LastTranslation = null;
            m_NumLoad = 0;
        }

        #endregion

        /// <summary>
        /// Loads a translation file.
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        internal void Load(string fileName)
        {
            m_NumLoad++;

            using (StreamReader sr = File.OpenText(fileName))
            {
                Load(sr);
            }
        }

        /// <summary>
        /// Loads a translation file. Lines that don't contain the "=" character
        /// will be ignored.
        /// </summary>
        /// <param name="sr">The stream to read in.</param>
        /// <exception cref="Exception">If a line is read that contains more than
        /// one "=" character.</exception>
        void Load(StreamReader sr)
        {
            string line;
            char[] sep = new char[] { '=' };
            IEntity[] ents = EnvironmentContainer.Current.EntityTypes;

            while ((line=sr.ReadLine())!=null)
            {
                if (line.IndexOf('=') >= 0)
                {
                    string[] items = line.Split(sep);
                    if (items.Length != 2)
                        throw new Exception("Unexpected data: "+line);

                    string featureCode = items[0].Trim();
                    string entName = items[1].Trim();

                    // Attempt to find the entity type
                    IEntity ent = Array.Find<IEntity>(ents, delegate(IEntity e)
                        { return String.Compare(entName, e.Name, true)==0; });

                    Translation<IEntity> t = new Translation<IEntity>(featureCode, ent, m_NumLoad);
                    m_Translations.Add(t);
                }
            }
        }

        /// <summary>
        /// Translates a feature code
        /// </summary>
        /// <param name="fc">The feature code to translate</param>
        /// <returns>The corresponding entity type (null if the specified feature code
        /// doesn't match anything previously loaded)</returns>
        internal IEntity Translate(string fc)
        {
            // Check the last translation we returned, since there's a chance the
            // subsequent items are the same.
            if (m_LastTranslation!=null && m_LastTranslation.IsMatch(fc))
                return m_LastTranslation.Output;

            foreach (Translation<IEntity> t in m_Translations)
            {
                if (t.IsMatch(fc))
                {
                    m_LastTranslation = t;
                    return t.Output;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a user-specified entity translation to this file.
        /// </summary>
        /// <param name="fc">The feature code</param>
        /// <param name="e">The corresponding entity type</param>
        /// <exception cref="ArgumentNullException">If the feature code is null (or blank),
        /// or the entity type is null.</exception>
        /// <exception cref="ArgumentException">If the specified feature code matches
        /// a translation item that was previously loaded.</exception>
        internal void Add(string fc, IEntity e)
        {
            if (String.IsNullOrEmpty(fc) || e==null)
                throw new ArgumentNullException();

            if (Translate(fc)!=null)
                throw new ArgumentException("Duplicate feature code "+fc);

            Translation<IEntity> t = new Translation<IEntity>(fc, e);
            m_Translations.Add(t);
        }

        /// <summary>
        /// The number of entries where the feature code is associated with a null
        /// entity type.
        /// </summary>
        internal int BadCount
        {
            get
            {
                int nBad = 0;

                foreach (Translation<IEntity> t in m_Translations)
                {
                    if (t.Output==null)
                        nBad++;
                }

                return nBad;
            }
        }

        /// <summary>
        /// Has the content of this translation file been saved to disk? False if
        /// a call to <c>Add</c> has been made, or more than one call to <c>Load</c>.
        /// True if <c>Load</c> has been called just once.
        /// </summary>
        internal bool IsSaved
        {
            get
            {
                if (m_NumLoad>1)
                    return true;

                foreach (Translation<IEntity> t in m_Translations)
                {
                    if (t.FileNum==0)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Writes translation info to an output file.
        /// </summary>
        /// <param name="fileName">The name of the file to create</param>
        internal void Save(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                m_NumLoad = 1;
                Save(sw);
            }
        }


        /// <summary>
        /// Writes translation info to an output stream.
        /// </summary>
        /// <param name="fileName">The stream to write to</param>
        void Save(StreamWriter sw)
        {
            foreach (Translation<IEntity> t in m_Translations)
            {
                string e = (t.Output==null ? String.Empty : t.Output.Name);
                string s = String.Format("{0,-12}={1}", t.Input, e);
                sw.WriteLine(s);
                t.FileNum = m_NumLoad;
            }
        }
    }
}
