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
    /// file type <c>.ced-xml</c>). The expectation is that the file type will be
    /// associated with the Cadastral Editor application so that the application
    /// can be launched with a double-click.
    /// </summary>
    [XmlRoot]
    public class JobFile
    {
        #region Constants

        /// <summary>
        /// The file extension for job files is ".ced-xml"
        /// </summary>
        public const string TYPE = ".ced-xml";

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

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization mechanism)
        /// </summary>
        public JobFile()
        {
            m_ConnectionString = String.Empty;
            m_JobId = 0;
            m_DrawInfo = new DrawInfo(0.0, 0.0, 0.0);
        }

        #endregion

        /// <summary>
        /// Reads job information from an XML file.
        /// </summary>
        /// <param name="fileName">The file spec for the input data</param>
        /// <returns>The data read from the input file</returns>
        public static JobFile CreateInstance(string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(JobFile));
            using (TextReader reader = new StreamReader(fileName))
            {
                return (JobFile)xs.Deserialize(reader);
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

            XmlSerializer xs = new XmlSerializer(typeof(JobFile));
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
    }
}
