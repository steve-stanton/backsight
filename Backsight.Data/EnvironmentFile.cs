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
using System.Diagnostics;

using EnvData = Backsight.Data.BacksightDataSet;
using Backsight.Environment;

namespace Backsight.Data
{
	/// <written by="Steve Stanton" on="08-MAR-2007" />
    /// <summary>
    /// Information relating to the Backsight operating environment that is stored
    /// in an XML file.
    /// </summary>
    public class EnvironmentFile : EnvironmentData, IEnvironmentContainer
    {
        #region Class data

        /// <summary>
        /// The full path of the file holding the data (may be blank if the file
        /// has never been saved)
        /// </summary>
        private string m_Path;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>EnvironmentFile</c> with undefined file name, and
        /// initializes the associated dataset. Before attempting to save the file,
        /// you must assign the path using the <c>Path</c> property.
        /// </summary>
        public EnvironmentFile() : base()
        {
            this.Path = String.Empty;
            //base.Initialize();
        }

        /// <summary>
        /// Creates a new <c>EnvironmentFile</c> with the specified name and loads it.
        /// </summary>
        /// <param name="path">The full path for the file (not null)</param>
        /// <exception cref="ArgumentNullException">If the supplied path is null</exception>
        public EnvironmentFile(string path) : base()
        {
            this.Path = path;
            Read();
        }

        /// <summary>
        /// Creates a new <c>EnvironmentFile</c> with the specified name and the supplied data.
        /// This constructor is used when copying information out of a database.
        /// </summary>
        /// <param name="path">The full path for the file (not null)</param>
        /// <param name="edb">The data to use (might be data coming from a database)</param>
        public EnvironmentFile(string path, EnvironmentData edb)
            : base(edb)
        {
            this.Path = path;
        }

        #endregion

        /// <summary>
        /// The full path of the file holding the data (blank if the file
        /// has never been saved). Not null.
        /// </summary>
        public string Path
        {
            get { return m_Path; }

            set
            {
                if (value==null)
                    throw new ArgumentNullException();

                m_Path = value;

                string name = System.IO.Path.GetFileName(value);
                if (!String.IsNullOrEmpty(name))
                    this.Name = name;
            }
        }

        public void Read()
        {
            if (String.IsNullOrEmpty(m_Path))
                throw new InvalidOperationException("Input path hasn't been defined");

            try
            {
                this.Data.Clear();
                this.Data.EnforceConstraints = false;
                this.Data.ReadXml(m_Path);
                this.Data.AcceptChanges();
            }

            catch (Exception ex)
            {
                int junk = 0;
            }
        }

        public void Write()
        {
            if (String.IsNullOrEmpty(this.Name))
                throw new InvalidOperationException("Output path hasn't been defined");

            this.Data.WriteXml(m_Path);
            //this.Data.AcceptChanges();
        }
    }
}
