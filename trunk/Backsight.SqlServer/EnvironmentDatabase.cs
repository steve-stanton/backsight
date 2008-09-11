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
using System.Data.SqlClient;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.SqlServer
{
    public class EnvironmentDatabase : EnvironmentData, IEnvironmentContainer
    {
        #region Class data

        /// <summary>
        /// The connection string for this database.
        /// </summary>
        readonly string m_ConnectionString;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EnvironmentDatabase</c> with the specified connection string
        /// </summary>
        public EnvironmentDatabase(string connectionString) : base()
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("No database connection string");

            m_ConnectionString = connectionString;
            AdapterFactory.ConnectionString = m_ConnectionString;
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            this.Name = String.Format(@"{0}\{1}", csb.DataSource, csb.InitialCatalog);
            Read();
        }

        #endregion

        #region IEnvironmentContainer Members

        public void Read()
        {
            this.Data.Load();
        }

        public void Write()
        {
            this.Data.Save(m_ConnectionString);
        }

        #endregion

        /// <summary>
        /// Replaces the content of this database with the content of some other container.
        /// </summary>
        /// <param name="ed">The environment data to copy into this database</param>
        public void Replace(EnvironmentData ed)
        {
            AdapterFactory.ConnectionString = m_ConnectionString;
            TableFactory tf = new TableFactory();

            try
            {
                // Disable all foreign key constraints
                tf.EnableForeignKeys(false);

                Transaction.Execute(delegate
                {
                    // Get rid of everything in this database.
                    tf.RemoveAll();

                    // Add the entire content of the specified container
                    tf.Import(ed.Data);
                });

                Data.Clear();
                Data.Load();
            }

            finally
            {
                // Restore all foreign key constraints
                tf.EnableForeignKeys(true);
            }
        }
    }
}
