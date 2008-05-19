/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Xml.Serialization;

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-APR-2008" />
    /// <summary>
    /// Information about a mapping job that gets saved as a disk file (with the
    /// file type <c>.cedx</c>). The expectation is that the file type will be
    /// associated with the Cadastral Editor application so that the application
    /// can be launched with a double-click.
    /// </summary>
    [XmlRoot]
    public class JobFileInfo
    {
        #region Constants

        /// <summary>
        /// The file extension for job files is ".cedx"
        /// </summary>
        public const string TYPE = ".cedx";

        #endregion

        #region Class data

        /// <summary>
        /// The database connection string
        /// </summary>
        string m_ConnectionString;

        /// <summary>
        /// The ID of the job that should be accessed (0 if not known)
        /// </summary>
        int m_JobId;

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

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization mechanism)
        /// </summary>
        public JobFileInfo()
        {
            m_ConnectionString = String.Empty;
            m_JobId = 0;
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
        }

        #endregion

        /// <summary>
        /// Reads job information from an XML file.
        /// </summary>
        /// <param name="fileName">The file spec for the input data</param>
        /// <returns>The data read from the input file</returns>
        public static JobFileInfo CreateInstance(string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(JobFileInfo));
            using (TextReader reader = new StreamReader(fileName))
            {
                return (JobFileInfo)xs.Deserialize(reader);
            }
        }

        /// <summary>
        /// Writes job information to an XML file.
        /// </summary>
        /// <param name="fileName">The output file (to create)</param>
        public void WriteXML(string fileName)
        {
            // Create the directory if it doesn't already exist
            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XmlSerializer xs = new XmlSerializer(typeof(JobFileInfo));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                xs.Serialize(writer, this);
            }
        }

        /// <summary>
        /// The database connection string
        /// </summary>
        [XmlAttribute]
        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        /// <summary>
        /// The ID of the job that should be accessed (0 if not known)
        /// </summary>
        [XmlAttribute]
        public int JobId
        {
            get { return m_JobId; }
            set { m_JobId = value; }
        }

        /// <summary>
        /// Information about the area that was last drawn.
        /// </summary>
        [XmlElement]
        public DrawInfo LastDraw
        {
            get { return m_DrawInfo; }
            set { m_DrawInfo = value; }
        }

        /// <summary>
        /// Current display units
        /// </summary>
        [XmlAttribute("DisplayUnit")]
        public DistanceUnitType DisplayUnitType
        {
            get { return m_DisplayUnit; }
            set { m_DisplayUnit = value; }
        }

        /// <summary>
        /// Current data entry units
        /// </summary>
        [XmlAttribute("EntryUnit")]
        public DistanceUnitType EntryUnitType
        {
            get { return m_EntryUnit; }
            set { m_EntryUnit = value; }
        }

        /// <summary>
        /// Should feature IDs be assigned automatically? (false if the user must specify).
        /// </summary>
        [XmlAttribute("AutoNumber")]
        public bool IsAutoNumber
        {
            get { return m_AutoNumber; }
            set { m_AutoNumber = value; }
        }

        /// <summary>
        /// Scale denominator at which labels (text) will start to be drawn.
        /// </summary>
        [XmlAttribute("LabelScale")]
        public double ShowLabelScale
        {
            get { return m_ShowLabelScale; }
            set { m_ShowLabelScale = value; }
        }

        /// <summary>
        /// Scale denominator at which points will start to be drawn.
        /// </summary>
        [XmlAttribute("PointScale")]
        public double ShowPointScale
        {
            get { return m_ShowPointScale; }
            set { m_ShowPointScale = value; }
        }

        /// <summary>
        /// Height of point symbols, in meters on the ground.
        /// </summary>
        [XmlAttribute]
        public double PointHeight
        {
            get { return m_PointHeight; }
            set { m_PointHeight = value; }
        }

        /// <summary>
        /// Should intersection points be drawn? Relevant only if points
        /// are drawn at the current display scale (see the <see cref="ShowPointScale"/>
        /// property).
        /// </summary>
        [XmlAttribute("IntersectionsDrawn")]
        public bool AreIntersectionsDrawn
        {
            get { return m_AreIntersectionsDrawn; }
            set { m_AreIntersectionsDrawn = value; }
        }

        /// <summary>
        /// The nominal map scale, for use in converting the size of fonts.
        /// </summary>
        [XmlAttribute]
        public uint NominalMapScale
        {
            get { return m_MapScale; }
            set { m_MapScale = value; }
        }

        /// <summary>
        /// The style for annotating lines with distances (and angles)
        /// </summary>
        [XmlElement]
        public LineAnnotationStyle LineAnnotation
        {
            get { return m_Annotation; }
        }
    }
}
