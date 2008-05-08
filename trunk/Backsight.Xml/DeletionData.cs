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
    [XmlRoot("Deletion")]
    public class DeletionData : OperationData
    {
        #region Class data

        /// <summary>
        /// The IDs of the deleted features
        /// </summary>
        public Guid[] Deletions;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor needed for serialization mechanism
        /// </summary>
        private DeletionData()
        {
        }

        /// <summary>
        /// Creates new <c>DeletionData</c> in preparation for serialization
        /// </summary>
        /// <param name="data">The data to take note of</param>
        private DeletionData(IDeletion data)
        {
            Deletions = data.Deletions;
        }

        #endregion

        /// <summary>
        /// Writes editing data to an XML file.
        /// </summary>
        /// <param name="data">The data to write out</param>
        /// <param name="file">The name of the file to create</param>
        public static void WriteXml(IDeletion data, string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DeletionData));
            using (StreamWriter s = File.CreateText(file))
            {
                xs.Serialize(s, new DeletionData(data));
            }
        }
    }
}
