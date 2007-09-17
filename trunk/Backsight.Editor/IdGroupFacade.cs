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

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// Fronts an instance of some object that implements <c>IIdGroup</c>.
    /// </summary>
    [Serializable]
    class IdGroupFacade : EnvironmentItemFacade<IIdGroup>, IIdGroup
    {
        internal IdGroupFacade(IIdGroup data) : base(data)
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name
        {
            get { return (this.Data==null ? String.Empty : this.Data.Name); }
        }

        public int LowestId
        {
            get { return (this.Data==null ? 0 : this.Data.LowestId); }
        }

        public int HighestId
        {
            get { return (this.Data==null ? 0 : this.Data.HighestId); }
        }

        public int PacketSize
        {
            get { return (this.Data==null ? 0 : this.Data.PacketSize); }
        }

        public string KeyFormat
        {
            get { return (this.Data==null ? String.Empty : this.Data.KeyFormat); }
        }

        public bool HasCheckDigit
        {
            get { return (this.Data==null ? false : this.Data.HasCheckDigit); }
        }

        public IEntity[] EntityTypes
        {
            get { return (this.Data==null ? null : this.Data.EntityTypes); }
        }
    }
}
