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
using System.Xml.Serialization;
using System.IO;

namespace Backsight.Xml
{
    [XmlRoot("AttachPoint")]
    public class AttachPointData : OperationData
    {
        #region Class data

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        [XmlAttribute]
        public Guid Line;

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        [XmlAttribute]
        public uint PositionRatio;

        /// <summary>
        /// The point that was created (without any geometry)
        /// </summary>
        [XmlElement]
        public FeatureData Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor needed for serialization mechanism
        /// </summary>
        private AttachPointData()
        {
        }

        /// <summary>
        /// Creates new <c>AttachPointData</c> in preparation for serialization
        /// </summary>
        /// <param name="data">The data to take note of</param>
        private AttachPointData(IAttachPoint data)
        {
            Line = data.Line;
            PositionRatio = data.PositionRatio;
            Point = data.Point;
        }

        #endregion

        /// <summary>
        /// Writes editing data to an XML file.
        /// </summary>
        /// <param name="data">The data to write out</param>
        /// <param name="file">The name of the file to create</param>
        public static void WriteXml(IAttachPoint data, string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AttachPointData));
            using (StreamWriter s = File.CreateText(file))
            {
                xs.Serialize(s, new AttachPointData(data));
            }
        }

    }
}
