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
using System.Data.SqlClient;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.SqlServer
{
    public class EnvironmentDatabase : EnvironmentData, IEnvironmentContainer
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EnvironmentDatabase</c> with the specified connection string
        /// </summary>
        public EnvironmentDatabase(string connectionString) : base()
        {
            AdapterFactory.ConnectionString = connectionString;
            Read();

            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            this.Name = String.Format(@"{0}\{1}", csb.DataSource, csb.InitialCatalog);
        }

        #endregion

        #region IEnvironmentContainer Members

        public void Read()
        {
            this.Data.Load();
        }

        public void Write()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ReleaseId(int id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public override int ReserveId()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
