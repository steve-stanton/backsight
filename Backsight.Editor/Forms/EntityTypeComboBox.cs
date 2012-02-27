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
using System.Windows.Forms;

using Backsight.Environment;

namespace Backsight.Editor.Forms
{
    /// <written by="Steve Stanton" on="11-DEC-2007" />
    /// <summary>
    /// A combo that lists entity types.
    /// </summary>
    /// <seealso cref="IEntity"/>
    public partial class EntityTypeComboBox : ComboBox
    {
        #region Class data

        /// <summary>
        /// Should entity types with blank names be displayed?
        /// </summary>
        bool m_ShowBlankEntityType;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty <c>EntityTypeComboBox</c>. Before making use of the
        /// combo, you need to load it via a call to the <see cref="Load"/> method.
        /// </summary>
        public EntityTypeComboBox()
        {
            m_ShowBlankEntityType = true;
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Loads this combo with entity types relating to the current
        /// editing layer, sorting the list by entity type name.
        /// </summary>
        /// <param name="type">The type of spatial object the entity types must be associated with</param>
        /// <returns>The default entity type in the combo (if any)</returns>
        public IEntity Load(SpatialType type)
        {
            ILayer layer = EditingController.Current.ActiveLayer;
            return Load(type, layer);            
        }

        /// <summary>
        /// Loads this combo with entity types relating to a specific
        /// editing layer, sorting the list by entity type name.
        /// </summary>
        /// <param name="type">The spatial type(s) of interest</param>
        /// <param name="layer">The layer of interest</param>
        /// <returns>The default entity type in the combo (if any)</returns>
        public IEntity Load(SpatialType type, ILayer layer)
        {
            IEntity[] entities = EnvironmentContainer.EntityTypes(type, layer);
            Array.Sort<IEntity>(entities, delegate(IEntity a, IEntity b)
                                    { return a.Name.CompareTo(b.Name); });

            // If we're not supposed to show blank entities, weed them out
            if (!m_ShowBlankEntityType)
                entities = Array.FindAll(entities, e => e.Name.Length>0);

            this.DataSource = entities;

            //IEntity ent = EnvironmentContainer.GetDefaultEntity(type, layer);
            IEntity ent = CadastralMapModel.Current.GetDefaultEntity(type);
            if (ent==null)
                return null;

            // The objects representing the default may be in a different address
            // space, so ensure we return the item from the combo.
            ent = Array.Find<IEntity>(entities, e => (e.Name == ent.Name));
            this.SelectedItem = ent;
            return ent;
        }

        /// <summary>
        /// Selects the item that has the same ID as the supplied entity type. A prior
        /// call to <see cref="Load"/> is required.
        /// </summary>
        /// <param name="e">The entity type of interest (null to select the entity type
        /// with ID==0)</param>
        /// <returns>The entity type selected from the combo (null if the combo doesn't
        /// contain an entity type with a matching ID)</returns>
        /// <remarks>The entity types that act as the datasource for the combo may be
        /// in a different address space from the supplied entity type. For example, the
        /// supplied entity type could be a facade that is part of a map model, whereas
        /// the datasource contains entity types that are obtained from the current
        /// environment container.</remarks>
        public IEntity SelectEntity(IEntity e)
        {
            int id = (e==null ? 0 : e.Id);

            IEntity[] entities = (DataSource as IEntity[]);
            if (entities==null)
                return null;

            // The objects representing the default may be in a different address
            // space, so ensure we return the item from the combo.
            IEntity ent = Array.Find<IEntity>(entities, a => (id == a.Id));
            this.SelectedItem = ent;
            return ent;
        }

        /// <summary>
        /// The currently selected entity type (null if the entity type has an ID of 0,
        /// indicating the blank entity type).
        /// </summary>
        public IEntity SelectedEntityType
        {
            get
            {
                IEntity e = (this.SelectedItem as IEntity);
                if (e==null || e.Id==0)
                    return null;
                else
                    return e;
            }
        }

        /// <summary>
        /// Should entity types with blank names be displayed?
        /// </summary>
        public bool ShowBlankEntityType
        {
            get { return m_ShowBlankEntityType; }
            set { m_ShowBlankEntityType = value; }
        }
    }
}
