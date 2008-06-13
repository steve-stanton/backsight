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

namespace Ntx
{
    /// <summary>
    /// Some sort of spatial feature. This contains important items from
    /// NTX descriptor records (plus the key (ID) field that NTX stuffs into
    /// so-called super-descriptors).
    /// </summary>
    abstract public class Feature
    {
        #region Class data

        /// <summary>
        /// Data code [PDCODE]
        /// </summary>
        int m_Type;

        /// <summary>
        /// Theme number [PDTMNO]
        /// </summary>
	    int m_Theme;

        /// <summary>
        /// User number [PDSRNO]
        /// </summary>
	    int m_UserNum;

        /// <summary>
        /// Feature code [PDFC]
        /// </summary>
	    string m_FeatureCode;

        /// <summary>
        /// Source ID [PDSRID]
        /// </summary>
	    string m_SourceId;

        /// <summary>
        /// Indexing key [PDKEY]
        /// </summary>
	    string m_Key;

        /// <summary>
        /// NW corner of cover [PDMAXY,PDMINX]
        /// </summary>
	    Position m_NorthWest;

        /// <summary>
        /// SE corner of cover [PDMINY,PDMAXX]
        /// </summary>
	    Position m_SouthEast;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Feature</c> with default values (zeros and
        /// blank strings).
        /// </summary>
        protected Feature()
        {
            m_Theme = 0;
            m_UserNum = 0;
            m_Type = (int)Ntx.DataType.None;
            m_NorthWest = null;
            m_SouthEast = null;
        	m_FeatureCode = String.Empty;
	        m_SourceId = String.Empty;
	        m_Key = String.Empty;
        }

        #endregion

        /// <summary>
        /// The feature code (what this thing signifies in the real world).
        /// </summary>
        public string FeatureCode
        {
            get { return m_FeatureCode; }
            internal set { m_FeatureCode = value; }
        }

        /// <summary>
        /// A source ID, meant to indicate where the data came from.
        /// </summary>
        internal string SourceId
        {
            get { return m_SourceId; }
            set { m_SourceId = value; }
        }

        /// <summary>
        /// A key (user-perceived ID) for this feature. This key would typically
        /// act as a primary key in an associated database table where additional
        /// attributes get held.
        /// </summary>
        public string Key
        {
            get { return m_Key; }
            internal set { m_Key = value; }
        }

        /// <summary>
        /// The "user" number for this feature (in this context, "user" means that
        /// the number is intended to have some significance to the user). It invariably
        /// matches the theme number, and can be confusing if it doesn't. It exists
        /// mainly because it was part of the data structure in the 1980s (and earlier).
        /// </summary>
        internal int UserNum
        {
            get { return m_UserNum; }
            set { m_UserNum = value; }
        }

        /// <summary>
        /// A theme number (indicating a mapping layer). When dealing with topological
        /// data, features with a specific theme are meant to form a single topological
        /// coverage (without any overlapping polygons).
        /// </summary>
        internal int Theme
        {
            get { return m_Theme; }
            set { m_Theme = value; }
        }

        /// <summary>
        /// What sort of data is this. See the <see cref="DataType"/> enum.
        /// </summary>
        internal int DataType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        /// <summary>
        /// The min-X & max-Y position of the window enclosing this feature.
        /// </summary>
        /// <remarks>Can't remember why I picked the NW corner. It would make more
        /// sense to pick the SW corner (min-X & min-Y)</remarks>
        public Position NorthWest
        {
            get { return  m_NorthWest; }
            set { m_NorthWest = value; }
        }

        /// <summary>
        /// The max-X & min-Y position of the window enclosing this feature.
        /// </summary>
        /// <remarks>Can't remember why I picked the SE corner. It would make more
        /// sense to pick the NE corner (max-X & max-Y)</remarks>
        public Position SouthEast
        {
            get { return m_SouthEast; }
            set { m_SouthEast = value; }
        }
    }
}
