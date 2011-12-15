using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Backsight.Editor.FileStore
{
    class IdFolder
    {
        #region Class data

        /// <summary>
        /// The path to the folder where ID information is stored.
        /// </summary>
        readonly string m_FolderName;

        /// <summary>
        /// The ID allocations that have been made.
        /// </summary>
        readonly List<IdAllocationInfo> m_Allocations;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdFolder"/> class.
        /// </summary>
        /// <param name="sessionId">A numeric ID for the session</param>
        /// <param name="folderName">The path to the folder where the edits were loaded from.</param>
        /// <param name="edits">Operations (if any) that were performed during the session.</param>
        internal IdFolder(string folderName)
        {
            m_FolderName = folderName;
            m_Allocations = new List<IdAllocationInfo>();

            // If the IDs folder doesn't exist, create it now.
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        #endregion

        string GetIdAllocationsSpec()
        {
            return Path.Combine(m_FolderName, "allocations.txt");
        }

        internal IdAllocationInfo[] GetIdAllocations()
        {
            string fileName = GetIdAllocationsSpec();
            if (!File.Exists(fileName))
                return new IdAllocationInfo[0];

            using (TextReader reader = new StreamReader(fileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(IdAllocationInfo[]));
                return (IdAllocationInfo[])xs.Deserialize(reader);
            }
        }
    }
}
