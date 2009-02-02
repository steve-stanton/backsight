// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Collections.Generic;
using System.Data;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// A row of miscellaneous attribute data
    /// </summary>
    class Row : IPossibleList<Row>
    {
        #region Class data

        /// <summary>
        /// The ID for the row
        /// </summary>
        readonly FeatureId m_Id;

        /// <summary>
        /// The definition of the table this row is part of 
        /// </summary>
        readonly ITable m_Table;

        /// <summary>
        /// The data for the row 
        /// </summary>
        readonly DataRow m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class,
        /// forming a two-way association with the ID
        /// </summary>
        /// <param name="id">The ID for the row (not null). Modified to refer to
        /// the newly created <c>Row</c> object.</param>
        /// <param name="table">The definition of the table this row is part of (not null).</param>
        /// <param name="data">Data for the row (not null).</param>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        internal Row(FeatureId id, ITable table, DataRow data)
        {
            if (id == null || table == null || data == null)
                throw new ArgumentNullException();

            m_Id = id;
            m_Table = table;
            m_Data = data;

            // Relate the ID to this row
            id.AddReference(this);
        }

        #endregion

        #region Implement IPossibleList<Row>

        public int Count { get { return 1; } }

        public Row this[int index]
        {
            get
            {
                if (index!=0)
                    throw new ArgumentOutOfRangeException();

                return this;
            }
        }

        public IEnumerator<Row> GetEnumerator()
        {
            yield return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // the other one
        }

        public IPossibleList<Row> Add(Row thing)
        {
            return new BasicList<Row>(this, thing);
        }

        public IPossibleList<Row> Remove(Row thing)
        {
            if (!Object.ReferenceEquals(this, thing))
                throw new ArgumentException();

            return null;
        }

        #endregion

        internal Row Extract(ExTranslation xref, Feature xfeat)
        {
            throw new NotImplementedException("Row.Extract");
        }

        /// <summary>
        /// The ID for the row
        /// </summary>
        internal FeatureId Id
        {
            get { return m_Id; }
        }

        /// <summary>
        /// The definition of the table this row is part of 
        /// </summary>
        internal ITable Table
        {
            get { return m_Table; }
        }

        /// <summary>
        /// The data for the row 
        /// </summary>
        internal DataRow Data
        {
            get { return m_Data; }
        }
    }
}
