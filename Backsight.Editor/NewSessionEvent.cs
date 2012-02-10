// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="04-FEB-2012"/>
    /// <summary>
    /// Event data for a new editing session
    /// </summary>
    class NewSessionEvent : Change
    {
        #region Class data

        /// <summary>
        /// The login name of the user running the session.
        /// </summary>
        internal string UserName { get; set; }

        /// <summary>
        /// The name of the computer where the project was created.
        /// </summary>
        internal string MachineName { get; set; }

        /// <summary>
        /// When was the last edit performed?
        /// </summary>
        internal DateTime EndTime { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSessionEvent"/> class.
        /// </summary>
        internal NewSessionEvent(uint id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSessionEvent"/> class.
        /// </summary>
        /// <param name="ed">The mechanism for reading back content.</param>
        internal NewSessionEvent(EditDeserializer ed)
            : base(ed)
        {
            this.UserName = ed.ReadString(DataField.UserName);
            this.MachineName = ed.ReadString(DataField.MachineName);
        }

        #endregion

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="es">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer es)
        {
            base.WriteData(es);

            es.WriteString(DataField.UserName, this.UserName);
            es.WriteString(DataField.MachineName, this.MachineName);
        }
    }
}
