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
using System.Collections.Generic;

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="08-MAR-2007" />
    /// <summary>
    /// Methods relating to the object that holds items associated with the Backsight
    /// operating environment. This holds a reference to the one (and only)
    /// <see cref="IEnvironmentContainer"/> that an application is associated with.
    /// It also acts as a dumping ground for static methods that have something to do
    /// with the environment classes.
    /// </summary>
    public static class EnvironmentContainer
    {
        /// <summary>
        /// The one (and only) environment container.
        /// </summary>
        private static IEnvironmentContainer s_Container = null;

        /// <summary>
        /// The one (and only) container for environment-related information.
        /// </summary>
        public static IEnvironmentContainer Current
        {
            get { return s_Container; }
            set { s_Container = value; }
        }

        /// <summary>
        /// Small shortcut to <c>Current.Factory</c> (null if a container hasn't been defined).
        /// </summary>
        public static IEnvironmentFactory Factory
        {
            get { return (s_Container==null ? null : s_Container.Factory); }
        }

        /// <summary>
        /// The entity types that relate to the specified spatial type, regardless of the
        /// mapping layer they may be restricted to.
        /// </summary>
        /// <param name="t">The type(s) of interest</param>
        /// <returns>The entity types associated with the specified spatial type.</returns>
        public static IEntity[] EntityTypes(SpatialType t)
        {
            return Filter(s_Container.EntityTypes, t, null);
        }

        /// <summary>
        /// The entity types that relate to the specified spatial type and mapping layer.
        /// </summary>
        /// <param name="t">The type(s) of interest</param>
        /// <param name="layer">The layer of interest (null for all layers)</param>
        /// <returns>The entity types associated with the specified spatial type and layer
        /// (i.e. entities that either refer explicitly to the layer, or which can be
        /// used on all layers).
        /// </returns>
        public static IEntity[] EntityTypes(SpatialType t, ILayer layer)
        {
            return Filter(s_Container.EntityTypes, t, layer);
        }

        /// <summary>
        /// The schemas that relate to the specified spatial type and mapping layer.
        /// </summary>
        /// <param name="t">The type(s) of interest</param>
        /// <param name="layer">The layer of interest (null for all layers)</param>
        /// <returns>The schemas associated with the entity types that apply to the specified
        /// spatial type and mapping layer</returns>
        public static ITable[] Schemas(SpatialType t, ILayer layer)
        {
            List<ITable> result = new List<ITable>();
            IEntity[] ents = EntityTypes(t, layer);

            foreach (IEntity e in ents)
            {
                ITable[] entSchemas = e.DefaultTables;
                foreach (ITable schema in entSchemas)
                {
                    if (!result.Contains(schema))
                        result.Add(schema);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Restricts the supplied array to those entity types where certain
        /// spatial type(s) are valid.
        /// </summary>
        /// <param name="ents">The array to restrict</param>
        /// <param name="t">The spatial type(s) of interest</param>
        /// <returns>The entity types that are associated with the spatial type</returns>
        public static IEntity[] Filter(IEntity[] ents, SpatialType t)
        {
            return Array.FindAll<IEntity>(ents, delegate(IEntity e)
                            { return e.IsValid(t); });
        }

        /// <summary>
        /// Restricts the supplied array to those entity types where certain
        /// spatial type(s) are valid, and that are associated with a specific map layer.
        /// </summary>
        /// <param name="ents">The array to restrict</param>
        /// <param name="t">The spatial type(s) of interest</param>
        /// <param name="layer">The layer of interest (null for all layers)</param>
        /// <returns>The entity types that meet the filtering criteria</returns>
        public static IEntity[] Filter(IEntity[] ents, SpatialType t, ILayer layer)
        {
            List<IEntity> result = new List<IEntity>(ents.Length);
            int layerId = (layer==null ? 0 : layer.Id);

            foreach (IEntity e in ents)
            {
                if (!e.IsValid(t))
                    continue;

                // If a specific layer is required...
                if (layerId!=0)
                {
                    // Skip if the entity type is associated with a specific layer,
                    // and it's not the required one.
                    ILayer entLayer = e.Layer;
                    if (entLayer!=null && entLayer.Id!=layerId)
                        continue;

                }

                result.Add(e);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Locates a map layer based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required layer</param>
        /// <returns>The corresponding layer (null if not found)</returns>
        public static ILayer FindLayerById(int id)
        {
            ILayer[] layers = s_Container.Layers;
            return Array.Find<ILayer>(layers, delegate(ILayer a) { return a.Id==id; });
        }

        /// <summary>
        /// Locates an entity type based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required entity type</param>
        /// <returns>The corresponding entity type (null if not found)</returns>
        public static IEntity FindEntityById(int id)
        {
            IEntity[] ents = s_Container.EntityTypes;
            return Array.Find<IEntity>(ents, delegate(IEntity e) { return e.Id==id; });
        }

        /// <summary>
        /// Locate the entity type with an ID==0.
        /// </summary>
        /// <returns>The "blank" entity type</returns>
        public static IEntity FindBlankEntity()
        {
            return FindEntityById(0);
        }

        /// <summary>
        /// Locates a Backsight property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns></returns>
        public static IProperty FindPropertyByName(string propertyName)
        {
            IProperty[] props = s_Container.Properties;
            return Array.Find<IProperty>(props, delegate(IProperty p) { return p.Name == propertyName; });
        }

        /// <summary>
        /// Locates font information based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required font</param>
        /// <returns>The corresponding font information  (null if not found)</returns>
        public static IFont FindFontById(int id)
        {
            IFont[] fonts = s_Container.Fonts;
            return Array.Find<IFont>(fonts, delegate(IFont f) { return f.Id == id; });
        }

        /// <summary>
        /// Locates a text template based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required template</param>
        /// <returns>The corresponding template (null if not found)</returns>
        public static ITemplate FindTemplateById(int id)
        {
            ITemplate[] templates = s_Container.Templates;
            return Array.Find<ITemplate>(templates, delegate(ITemplate t) { return t.Id == id; });
        }

        /// <summary>
        /// Locates a map theme based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required theme</param>
        /// <returns>The corresponding theme (null if not found)</returns>
        public static ITheme FindThemeById(int id)
        {
            ITheme[] themes = s_Container.Themes;
            return Array.Find<ITheme>(themes, delegate(ITheme t) { return t.Id == id; });
        }

        /// <summary>
        /// Locates an ID group based on it's unique ID
        /// </summary>
        /// <param name="id">The ID of the required ID group</param>
        /// <returns>The corresponding ID group (null if not found)</returns>
        public static IIdGroup FindIdGroupById(int id)
        {
            IIdGroup[] idGroups = s_Container.IdGroups;
            return Array.Find<IIdGroup>(idGroups, delegate(IIdGroup g) { return g.Id == id; });
        }
    }
}
