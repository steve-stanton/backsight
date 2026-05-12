// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

using System.Data;
using Backsight.Environment;

namespace Backsight.Data;

public partial class BacksightDataSet
{
    /// <summary>
    /// The content of a domain table.
    /// </summary>
    /// <remarks>The class name is consistent with the naming conventions used throughout this project,
    /// but may be confusing here, since <c>DomainTableRow</c> suggests a single row in a domain table.
    /// What it actually represents is the complete table.</remarks>
    public partial class DomainTableRow : IEditDomainTable
    {
        /// <summary>
        /// The data for the domain table. The key is the lookup value, the
        /// value is the expanded value. Lazy loaded on the first call to the
        /// <see cref="Lookup"/> method.
        /// </summary>
        Dictionary<string, string> m_Data;

        public override string ToString()
        {
            return TableName;
        }

        public void FinishEdit()
        {
            if (IsAdded(this))
                this.EndEdit();
            else
                this.tableDomainTable.AddDomainTableRow(this);
        }

        public static DomainTableRow CreateDomainTableRow(BacksightDataSet ds)
        {
            DomainTableRow result = ds.DomainTable.NewDomainTableRow();
            result.SetDefaultValues();
            return result;
        }

        internal void SetDefaultValues()
        {
            DomainId = 0;
            TableName = String.Empty;
        }

        public int Id
        {
            get { return DomainId; }
        }


        public string Lookup(string shortValue)
        {
            throw new NotImplementedException();
            /*
            if (m_Data == null)
                m_Data = LoadDomainTable();

            string result;

            if (m_Data.TryGetValue(shortValue, out result))
                return result;
            else
                return String.Empty;
                */
        }

        /*
        Dictionary<string, string> LoadDomainTable(string connectionString)
        {
            IDataServer ds = new DataServer(connectionString);
            Dictionary<string, string> result = new Dictionary<string, string>();
            DataTable table = ds.ExecuteSelect("SELECT [ShortValue], [LongValue] FROM " + TableName);

            foreach (DataRow row in table.Select())
            {
                string key = row[0].ToString();
                string val = row[1].ToString();
                result.Add(key, val);
            }

            return result;
        }
*/
        
        public string[] GetLookupValues()
        {
            throw new NotImplementedException();
            /*
            if (m_Data == null)
                m_Data = LoadDomainTable();

            string[] result = new string[m_Data.Count];

            int i = 0;
            foreach (string s in m_Data.Keys)
            {
                result[i] = s;
                i++;
            }

            return result;
            */
        }
    }
}