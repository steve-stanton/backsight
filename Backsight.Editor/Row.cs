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
using System.Collections.Generic;

namespace Backsight.Editor
{
    [Serializable]
    class Row : IPossibleList<Row>
    {
        // The ID for the row
        private FeatureId m_RowId;

        // The definition of the schema
        //private ISchema m_Schema;

        // Data for each column
        private object[] m_Data;

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

        #region IRow Members

        /*
        public ISchema Schema
        {
            get { return m_Schema; }
        }
        */

        internal FeatureId Id
        {
            get { return m_RowId; }
            set { m_RowId = value; }
        }

        #endregion

        /*
        internal bool IsSameData(ISchema schema, object[] data)
        {
            throw new NotImplementedException();
        }
         */
    }
}
