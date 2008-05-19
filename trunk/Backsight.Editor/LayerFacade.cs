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
using System.Runtime.Serialization;

using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="22-MAR-2007" />
    /// <summary>
    /// Fronts an instance of some object that implements <c>ILayer</c>.
    /// </summary>
    [Serializable]
    class LayerFacade : EnvironmentItemFacade<ILayer>, ILayer
    {
        internal LayerFacade(ILayer data) : base(data)
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

        public IEntity DefaultPointType
        {
            get { return (this.Data==null ? null : this.Data.DefaultPointType); }
        }

        public IEntity DefaultLineType
        {
            get { return (this.Data==null ? null : this.Data.DefaultLineType); }
        }

        public IEntity DefaultTextType
        {
            get { return (this.Data==null ? null : this.Data.DefaultTextType); }
        }

        public IEntity DefaultPolygonType
        {
            get { return (this.Data==null ? null : this.Data.DefaultPolygonType); }
        }

        public ITheme Theme
        {
            get { return (this.Data==null ? null : this.Data.Theme); }
        }

        public int ThemeSequence
        {
            get { return (this.Data==null ? 0 : this.Data.ThemeSequence); }
        }

        [OnDeserialized]
        void GetEnvironmentData(StreamingContext context)
        {
            this.Data = EnvironmentContainer.FindLayerById(this.Id);
        }
    }
}
