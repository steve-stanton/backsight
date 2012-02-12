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
    /// <written by="Steve Stanton" on="23-JAN-2012"/>
    /// <summary>
    /// Event data for a new project
    /// </summary>
    class NewProjectEvent : Change
    {
        #region Constants

        /// <summary>
        /// The name of the file holding information for brand new projects.
        /// </summary>
        internal const string FileName = "00000001.txt";

        #endregion


        #region Class data

        /// <summary>
        /// A unique ID for the project.
        /// </summary>
        internal Guid ProjectId { get; set; }

        /// <summary>
        /// The user-perceived name of the project.
        /// </summary>
        internal string ProjectName { get; set; }

        /// <summary>
        /// The ID of the map layer the project is associated with.
        /// </summary>
        internal int LayerId { get; set; }

        /// <summary>
        /// The name of the default coordinate system.
        /// </summary>
        internal string DefaultSystem { get; set; }

        /// <summary>
        /// The login name of the user who created the project.
        /// </summary>
        internal string UserName { get; set; }

        /// <summary>
        /// The name of the computer where the project was created.
        /// </summary>
        internal string MachineName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectEvent"/> class
        /// with default values for all properties.
        /// </summary>
        /// <remarks>Note that the sequence number passed down to the base class
        /// must be consistent with the <see cref="FileName"/> constant.
        /// </remarks>
        internal NewProjectEvent()
            : base(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectEvent"/> class.
        /// </summary>
        /// <param name="ed">The mechanism for reading back content.</param>
        internal NewProjectEvent(EditDeserializer ed)
            : base(ed)
        {
            this.ProjectId = new Guid(ed.ReadString(DataField.ProjectId));
            this.ProjectName = ed.ReadString(DataField.ProjectName);
            this.LayerId = ed.ReadInt32(DataField.LayerId);
            this.DefaultSystem = ed.ReadString(DataField.CoordinateSystem);
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

            es.WriteString(DataField.ProjectId, this.ProjectId.ToString().ToUpper());
            es.WriteString(DataField.ProjectName, this.ProjectName);
            es.WriteInt32(DataField.LayerId, this.LayerId);
            es.WriteString(DataField.CoordinateSystem, this.DefaultSystem);
            es.WriteString(DataField.UserName, this.UserName);
            es.WriteString(DataField.MachineName, this.MachineName);
        }
    }
}
