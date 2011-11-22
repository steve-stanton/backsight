// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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

using System.IO;
using System.Reflection;

using Backsight.Data;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="22-NOV-2011"/>
    /// <summary>
    /// A readonly environment definition that corresponds to an XML file included as an
    /// application resource file. The resource matches the format that is produced when
    /// you export a definition from the Environment Editor application.
    /// </summary>
    /// <remarks>Embedding an environment is meant to provide an easy entry path for potential users
    /// who want to play about with Backsight (means you don't have to worry about where to stick an
    /// environment definition). If someone seriously intends to work with Backsight, they will probably
    /// want to devise their own environment definition (using the Environment Editor application).
    /// </remarks>
    class EnvironmentResource : EnvironmentData, IEnvironmentContainer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentResource"/> class using
        /// the environment that is embedded in this assembly.
        /// </summary>
        internal EnvironmentResource()
        {
            Read();
        }

        #endregion


        #region IEnvironmentContainer Members

        /// <summary>
        /// Loads this container with environment-related data.
        /// </summary>
        public void Read()
        {
            this.Data.Clear();
            Assembly a = Assembly.GetExecutingAssembly();

            using (Stream s = a.GetManifestResourceStream("Backsight.Editor.Resources.DefaultEnvironment.xml"))
            {
                this.Data.ReadXml(s);
            }

            this.Data.AcceptChanges();
        }

        /// <summary>
        /// Saves the content of this container.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown always</exception>
        public void Write()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
