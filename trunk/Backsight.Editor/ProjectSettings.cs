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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Backsight.Environment;
using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-APR-2008" />
    /// <summary>
    /// Current settings for a Backsight project.
    /// <para/>
    /// This class hold transient properties relating to the Cadastral Editor application -
    /// things like the position for the last draw, as well as editing defaults that the user has
    /// the ability to respecify. The important thing to remember is that only the most recent project
    /// settings are persisted. Thus, no edit should rely implicitly on these properties (the persistent
    /// version of each edit must be able to stand alone).
    /// </summary>
    [XmlRoot]
    public class ProjectSettings
    {
        #region Class data

        /// <summary>
        /// Have changes been made to the values stored in this instance?
        /// Set to <c>true</c> on a call to <see cref="Set"/>. Set to <c>false</c>
        /// on a call to <see cref="WriteXML"/>.
        /// </summary>
        bool m_IsChanged;

        /// <summary>
        /// Any database connection string
        /// </summary>
        string m_ConnectionString;

        /// <summary>
        /// Information about the area that was last drawn.
        /// </summary>
        DrawInfo m_DrawInfo;

        /// <summary>
        /// Current display units
        /// </summary>
        DistanceUnitType m_DisplayUnit;

        /// <summary>
        /// Current data entry units
        /// </summary>
        DistanceUnitType m_EntryUnit;

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        bool m_AutoNumber;

        /// <summary>
        /// Scale denominator at which labels (text) will start to be drawn.
        /// </summary>
        double m_ShowLabelScale;

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        double m_ShowPointScale;

        /// <summary>
        /// Height of point symbols, in meters on the ground.
        /// </summary>
        double m_PointHeight;

        /// <summary>
        /// Should intersection points be drawn? Relevant only if points
        /// are drawn at the current display scale (see the <see cref="ShowPointScale"/>
        /// property).
        /// </summary>
        bool m_AreIntersectionsDrawn;

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        uint m_MapScale;

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        LineAnnotationStyle m_Annotation;

        /// <summary>
        /// The ID of the default entity type for points (0 if undefined)
        /// </summary>
        int m_DefaultPointType;

        /// <summary>
        /// The ID of the default entity type for lines (0 if undefined)
        /// </summary>
        int m_DefaultLineType;

        /// <summary>
        /// The ID of the default entity type for polygon labels (0 if undefined)
        /// </summary>
        int m_DefaultPolygonType;

        /// <summary>
        /// The ID of the default entity type for text (0 if undefined)
        /// </summary>
        int m_DefaultTextType;

        /// <summary>
        /// Increment value used by the application's splash screen (to ensure smooth
        /// progress bar on load)
        /// </summary>
        string m_SplashIncrement;

        /// <summary>
        /// The percentage completion values that correspond to each increment
        /// </summary>
        string m_SplashPercents;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization mechanism)
        /// </summary>
        public ProjectSettings()
        {
            m_ConnectionString = String.Empty;
            m_DrawInfo = new DrawInfo(0.0, 0.0, 0.0);
            m_DisplayUnit = DistanceUnitType.AsEntered;
            m_EntryUnit = DistanceUnitType.Meters;
            m_AutoNumber = true;
            m_ShowLabelScale = 2000.0;
            m_ShowPointScale = 2000.0;
            m_PointHeight = 2.0;
            m_AreIntersectionsDrawn = false;
            m_MapScale = 2000;
            m_Annotation = new LineAnnotationStyle();
            m_IsChanged = false;
            m_DefaultPointType = 0;
            m_DefaultLineType = 0;
            m_DefaultPolygonType = 0;
            m_DefaultTextType = 0;
            m_SplashIncrement = String.Empty;
            m_SplashPercents = String.Empty;
        }

        #endregion

        /// <summary>
        /// Method called whenever values of this class are changed. This just ensures
        /// that <see cref="m_IsChanged"/> gets set.
        /// </summary>
        /// <typeparam name="T">The type of value that's being changed</typeparam>
        /// <param name="value">The value to assign</param>
        /// <returns>The supplied value</returns>
        T Set<T>(T value)
        {
            m_IsChanged = true;
            return value;
        }

        /// <summary>
        /// Reads project settings from an XML file.
        /// </summary>
        /// <param name="fileName">The file spec for the input data</param>
        /// <returns>The data read from the input file</returns>
        public static ProjectSettings CreateInstance(string fileName)
        {
            // If the file doesn't already exist, create something. The file won't have any defaults for entity types, because
            // we don't know the map layer here - they'll get defined when the map layer is picked up by Project.LoadDataFiles.
            if (!File.Exists(fileName))
                new ProjectSettings().WriteXML(fileName);

            XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
            using (TextReader reader = new StreamReader(fileName))
            {
                ProjectSettings result = (ProjectSettings)xs.Deserialize(reader);
                result.m_IsChanged = false;
                return result;
            }
        }

        /// <summary>
        /// Writes project information to an XML file.
        /// </summary>
        /// <param name="fileName">The output file (to create)</param>
        public void WriteXML(string fileName)
        {
            // Create the directory if it doesn't already exist
            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                xs.Serialize(writer, this);
                m_IsChanged = false;
            }
        }

        /// <summary>
        /// The database connection string
        /// </summary>
        [XmlElement]
        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = Set<string>(value); }
        }

        /// <summary>
        /// Information about the area that was last drawn.
        /// </summary>
        [XmlElement]
        public DrawInfo LastDraw
        {
            get { return m_DrawInfo; }
            set { m_DrawInfo = Set<DrawInfo>(value); }
        }

        /// <summary>
        /// Current display units
        /// </summary>
        [XmlElement("DisplayUnit")]
        public DistanceUnitType DisplayUnitType
        {
            get { return m_DisplayUnit; }
            set { m_DisplayUnit = Set<DistanceUnitType>(value); }
        }

        /// <summary>
        /// Current data entry units
        /// </summary>
        [XmlElement("EntryUnit")]
        public DistanceUnitType EntryUnitType
        {
            get { return m_EntryUnit; }
            set { m_EntryUnit = Set<DistanceUnitType>(value); }
        }

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        [XmlElement("AutoNumber")]
        public bool IsAutoNumber
        {
            get { return m_AutoNumber; }
            set { m_AutoNumber = Set<bool>(value); }
        }

        /// <summary>
        /// Scale denominator at which labels (text) will start to be drawn.
        /// </summary>
        [XmlElement("LabelScale")]
        public double ShowLabelScale
        {
            get { return m_ShowLabelScale; }
            set { m_ShowLabelScale = Set<double>(value); }
        }

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        [XmlElement("PointScale")]
        public double ShowPointScale
        {
            get { return m_ShowPointScale; }
            set { m_ShowPointScale = Set<double>(value); }
        }

        /// <summary>
        /// Height of point symbols, in meters on the ground.
        /// </summary>
        [XmlElement]
        public double PointHeight
        {
            get { return m_PointHeight; }
            set { m_PointHeight = Set<double>(value); }
        }

        /// <summary>
        /// Should intersection points be drawn? Relevant only if points
        /// are drawn at the current display scale (see the <see cref="ShowPointScale"/> property).
        /// </summary>
        [XmlElement("IntersectionsDrawn")]
        public bool AreIntersectionsDrawn
        {
            get { return m_AreIntersectionsDrawn; }
            set { m_AreIntersectionsDrawn = Set<bool>(value); }
        }

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        [XmlElement]
        public uint NominalMapScale
        {
            get { return m_MapScale; }
            set { m_MapScale = Set<uint>(value); }
        }

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        [XmlElement]
        public LineAnnotationStyle LineAnnotation
        {
            get { return m_Annotation; }
            set { m_Annotation = Set<LineAnnotationStyle>(value); }
        }

        /// <summary>
        /// The ID of the default entity type for points (0 if undefined)
        /// </summary>
        [XmlElement]
        public int DefaultPointType
        {
            get { return m_DefaultPointType; }
            set { m_DefaultPointType = Set<int>(value); }
        }

        /// <summary>
        /// The ID of the default entity type for lines (0 if undefined)
        /// </summary>
        [XmlElement]
        public int DefaultLineType
        {
            get { return m_DefaultLineType; }
            set { m_DefaultLineType = Set<int>(value); }
        }

        /// <summary>
        /// The ID of the default entity type for polygons (0 if undefined)
        /// </summary>
        [XmlElement]
        public int DefaultPolygonType
        {
            get { return m_DefaultPolygonType; }
            set { m_DefaultPolygonType = Set<int>(value); }
        }

        /// <summary>
        /// The ID of the default entity type for text (0 if undefined)
        /// </summary>
        [XmlElement]
        public int DefaultTextType
        {
            get { return m_DefaultTextType; }
            set { m_DefaultTextType = Set<int>(value); }
        }

        /// <summary>
        /// Has the information recorded in this instance been saved to disk?
        /// </summary>
        internal bool IsSaved
        {
            get { return !m_IsChanged; }
        }

        [XmlElement]
        public string SplashIncrement
        {
            get { return m_SplashIncrement; }
            set { m_SplashIncrement = Set<string>(value); }
        }

        [XmlElement]
        public string SplashPercents
        {
            get { return m_SplashPercents; }
            set { m_SplashPercents = Set<string>(value); }
        }

        /// <summary>
        /// Ensures default entity types have been defined.
        /// </summary>
        /// <param name="layer">The map layer for the project</param>
        internal void SetEntityTypeDefaults(ILayer layer)
        {
            if (layer == null)
            {
                Trace.WriteLine("ProjectSettings.SetEntityTypeDefaults: Undefined layer");
                return;
            }

            if (DefaultPointType == 0)
                DefaultPointType = layer.DefaultPointType.Id;

            if (DefaultLineType == 0)
                DefaultLineType = layer.DefaultLineType.Id;

            if (DefaultPolygonType == 0)
                DefaultPolygonType = layer.DefaultPolygonType.Id;

            if (DefaultTextType == 0)
                DefaultTextType = layer.DefaultTextType.Id;
        }
    }
}
