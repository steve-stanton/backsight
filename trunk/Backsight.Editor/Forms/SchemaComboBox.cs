/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="18-APR-2008" />
    /// <summary>
    /// A combo that lists feature schemas (tables)
    /// </summary>
    /// <seealso cref="ITable"/>
    public partial class SchemaComboBox : ComboBox
    {
        /// <summary>
        /// Creates an empty <c>SchemaComboBox</c>. Before making use of the
        /// combo, you need to load it via a call to the <see cref="Load"/> method.
        /// </summary>
        public SchemaComboBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads this combo with schemas relating to the current
        /// editing layer, sorting the list by table name.
        /// </summary>
        /// <param name="type">The type of spatial object the schemas must be associated with</param>
        /// <returns>The default schema in the combo (if any)</returns>
        public ITable Load(SpatialType type)
        {
            ILayer layer = EditingController.Current.ActiveLayer;
            return Load(type, layer);
        }

        /// <summary>
        /// Loads this combo with schemas relating to a specific entity type
        /// </summary>
        /// <param name="ent">The entity type</param>
        /// <returns>The first schema associated with the entity type (null if the
        /// entity type isn't associated with anything).</returns>
        public ITable Load(IEntity ent)
        {
            if (ent==null)
            {
                this.DataSource = null;
                return null;
            }

            ITable[] schemas = ent.DefaultTables;
            Array.Sort<ITable>(schemas, delegate(ITable a, ITable b)
                                    { return a.TableName.CompareTo(b.TableName); });
            this.DataSource = schemas;

            if (schemas.Length > 0)
            {
                this.SelectedItem = schemas[0];
                return schemas[0];
            }

            return null;
        }

        /// <summary>
        /// Loads this combo with schemas relating to a specific editing layer, sorting the list by table name.
        /// </summary>
        /// <param name="type">The spatial type(s) of interest</param>
        /// <param name="layer">The layer of interest</param>
        /// <returns>The default schema in the combo (if any)</returns>
        public ITable Load(SpatialType type, ILayer layer)
        {
            ITable[] schemas = EnvironmentContainer.Schemas(type, layer);
            Array.Sort<ITable>(schemas, delegate(ITable a, ITable b)
                                    { return a.TableName.CompareTo(b.TableName); });
            this.DataSource = schemas;

            // If a specific geometric type was supplied, & the specified
            // layer has a default entity type, scroll to the first schema for that entity

            if (type == 0)
                return null;

            IEntity ent = EnvironmentContainer.GetDefaultEntity(type, layer);
            if (ent!=null)
            {
                ITable[] tabs = ent.DefaultTables;
                if (tabs.Length > 0)
                {
                    this.SelectedItem = tabs[0];
                    return tabs[0];
                }
            }

            return null;
        }

        /// <summary>
        /// The currently selected table
        /// </summary>
        public ITable SelectedTable
        {
            get { return (this.SelectedItem as ITable); }
        }

        /// <summary>
        /// Clears this combo (sets the <see cref="DataSource"/> property to null).
        /// </summary>
        public void Clear()
        {
            DataSource = null;
        }
    }
}
